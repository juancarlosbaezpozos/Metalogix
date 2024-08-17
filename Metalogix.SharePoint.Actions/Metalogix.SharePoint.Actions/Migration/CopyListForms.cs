using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MandatoryTransformers(new Type[] { typeof(FormPageContentUpdater) })]
	[Name("Paste List Forms")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPList), true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPList))]
	public class CopyListForms : PasteAction<PasteListFormsOptions>
	{
		private readonly TransformerDefinition<SPFile, CopyListForms, SPFileCollection, SPFileCollection> _formDataTransformerDefinition = new TransformerDefinition<SPFile, CopyListForms, SPFileCollection, SPFileCollection>("List Form Data", false);

		private readonly static HashSet<string> FormPageFileExtensions;

		static CopyListForms()
		{
			HashSet<string> strs = new HashSet<string>();
			strs.Add(".asp");
			strs.Add(".aspx");
			strs.Add(".ascx");
			strs.Add(".master");
			CopyListForms.FormPageFileExtensions = strs;
		}

		public CopyListForms()
		{
		}

		private void CopyForms(SPList sourceList, SPList targetList)
		{
			List<SPFile> formPages = this.GetFormPages(sourceList, targetList);
			this._formDataTransformerDefinition.BeginTransformation(this, sourceList.FormsFolder.Files, targetList.FormsFolder.Files, this.ActionOptions.Transformers);
			try
			{
				foreach (SPFile formPage in formPages)
				{
					if (!base.CheckForAbort())
					{
						SPFile item = targetList.FormsFolder.Files[formPage.Name];
						if (item != null && formPage.CustomizedPageStatus == SPCustomizedPageStatus.Uncustomized)
						{
							continue;
						}
						if (this.ActionOptions.CopyCustomizedFormPages)
						{
							item = this.CopyFormToTarget(formPage, sourceList, targetList);
						}
						if (item == null || !this.ActionOptions.CopyFormWebParts)
						{
							continue;
						}
						ThreadedOperationDelegate threadedOperationDelegate = new ThreadedOperationDelegate(this.CopyFormWebParts);
						object[] objArray = new object[] { formPage, item };
						TaskDefinition taskDefinition = new TaskDefinition(threadedOperationDelegate, objArray);
						base.ThreadManager.QueueBufferedTask(base.GetWebPartCopyBufferKey(targetList.ParentWeb), taskDefinition);
						base.ThreadManager.QueueBufferedTask("RunActionEndReached", taskDefinition);
					}
					else
					{
						return;
					}
				}
			}
			finally
			{
				this._formDataTransformerDefinition.EndTransformation(this, sourceList.FormsFolder.Files, targetList.FormsFolder.Files, this.ActionOptions.Transformers);
			}
		}

		private SPFile CopyFormToTarget(SPFile sourceForm, SPList sourceList, SPList targetList)
		{
			SPFile sPFile = null;
			LogItem logItem = new LogItem(string.Concat("Copying form page: ", sourceForm.Name), sourceForm.Name, sourceList.FormsFolder.DisplayUrl, targetList.FormsFolder.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					sourceForm = this._formDataTransformerDefinition.Transform(sourceForm, this, sourceList.FormsFolder.Files, targetList.FormsFolder.Files, this.ActionOptions.Transformers);
					if (sourceForm != null)
					{
						sPFile = targetList.FormsFolder.Files.Add(sourceForm);
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						logItem.Status = ActionOperationStatus.Skipped;
						logItem.Details = "Form skipped by transformer.";
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
				if (sourceForm != null)
				{
					sourceForm.ReleaseContent();
				}
			}
			return sPFile;
		}

		private void CopyFormWebParts(object[] inputs)
		{
			SPFile sPFile = inputs[0] as SPFile;
			SPFile sPFile1 = inputs[1] as SPFile;
			LogItem logItem = new LogItem("Copying Web Parts on Form Page", sPFile.Name, sPFile.DisplayUrl, sPFile1.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					SPWebPartPage webPartPage = (new SPWebPartPage()).GetWebPartPage(sPFile.Web, sPFile.ServerRelativeUrl, this);
					SPWebPartPage sPWebPartPage = (new SPWebPartPage()).GetWebPartPage(sPFile1.Web, sPFile1.ServerRelativeUrl, this);
					CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction()
					{
						LinkCorrector = base.LinkCorrector
					};
					copyWebPartsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyWebPartsAction);
					object[] objArray = new object[] { webPartPage, sPWebPartPage, logItem };
					copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(webPartPage.ParentWeb, sPWebPartPage.ParentWeb), null);
					if (logItem.Status != ActionOperationStatus.Failed)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.SourceContent = sPFile.XML;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Exception = exception;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private List<SPFile> GetFormPages(SPList sourceList, SPList targetList)
		{
			List<SPFile> sPFiles = new List<SPFile>();
			foreach (SPFile file in sourceList.FormsFolder.Files)
			{
				bool flag = false;
				foreach (SPView view in targetList.Views)
				{
					if (view.IsWebPartView || !UrlUtils.Equal(UrlUtils.GetAfterLastSlash(view.Url), file.Name))
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag || !this.IsFormPageFile(file.Name))
				{
					continue;
				}
				sPFiles.Add(file);
			}
			return sPFiles;
		}

		private bool IsFormPageFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (string.IsNullOrEmpty(extension))
			{
				return false;
			}
			return CopyListForms.FormPageFileExtensions.Contains(extension.ToLower());
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length != 2)
			{
				throw new ArgumentException(Resources.RunOperationWrongParameterNumber, "oParams");
			}
			SPList sPList = oParams[0] as SPList;
			if (sPList == null)
			{
				throw new ArgumentException(Resources.FirstElementMustBeSPList, "oParams");
			}
			SPList sPList1 = oParams[1] as SPList;
			if (sPList1 == null)
			{
				throw new ArgumentException(Resources.SecondElementMustBeSPList, "oParams");
			}
			this.CopyForms(sPList, sPList1);
			sPList.Dispose();
			sPList1.Dispose();
		}
	}
}