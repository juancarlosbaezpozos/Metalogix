using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointListTemplateGallery", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.MasterPageGallery.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Site Objects {0-Paste} > List Template Gallery...")]
	[Name("Paste List Template Gallery")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPSite))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPSite))]
	[UsesStickySettings(true)]
	public class CopyListTemplateGalleryAction : PasteAction<PasteListTemplateGalleryOptions>
	{
		public CopyListTemplateGalleryAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			SPSite item = sourceSelections[0] as SPSite;
			SPSite sPSite = targetSelections[0] as SPSite;
			if (!(item.UIVersion != sPSite.UIVersion) && sPSite.Status != ConnectionStatus.Invalid && !(item.DisplayUrl == sPSite.DisplayUrl))
			{
				return true;
			}
			return false;
		}

		private void CopyListTemplates(SPListCollection sourceListCollection, SPWeb targetWeb)
		{
			PasteListAction pasteListAction = new PasteListAction();
			pasteListAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			pasteListAction.SharePointOptions.OverwriteLists = false;
			base.SubActions.Add(pasteListAction);
			pasteListAction.ListTransformerDefinition.BeginTransformation(pasteListAction, sourceListCollection, targetWeb.Lists, pasteListAction.Options.Transformers);
			if (base.CheckForAbort())
			{
				return;
			}
			try
			{
				List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
				foreach (Node node in sourceListCollection)
				{
					if (!base.CheckForAbort())
					{
						TaskDefinition taskDefinition = pasteListAction.CopyList(node as SPList, targetWeb, false, false, false);
						if (taskDefinition == null)
						{
							continue;
						}
						taskDefinitions.Add(taskDefinition);
					}
					else
					{
						return;
					}
				}
				base.ThreadManager.WaitForTasks(taskDefinitions);
			}
			finally
			{
				base.SetAllListItemCopyCompletedBufferKeysForWeb(targetWeb);
			}
			pasteListAction.ListTransformerDefinition.EndTransformation(pasteListAction, sourceListCollection, targetWeb.Lists, pasteListAction.Options.Transformers);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					Node[] nodeArray = new Node[] { sPWeb };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					try
					{
						this.CopyListTemplates(new SPListCollection(item)
						{
							item.ListTemplateGallery
						}, sPWeb);
					}
					finally
					{
						sPWeb.Dispose();
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			this.CopyListTemplates(new SPListCollection(sPWeb)
			{
				sPWeb.ListTemplateGallery
			}, sPWeb1);
		}
	}
}