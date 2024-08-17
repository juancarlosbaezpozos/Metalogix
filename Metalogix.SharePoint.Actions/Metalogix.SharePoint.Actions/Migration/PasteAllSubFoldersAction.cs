using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointFolder", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[Incrementable(true, "Paste All Subfolders Incrementally")]
	[MenuText("Paste Special {0-Paste} > Paste all subfolders...")]
	[MenuTextPlural("Paste Folders... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste All Subfolders")]
	[ShowInMenus(false)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPFolder), false)]
	[SubActionTypes(new Type[] { typeof(CopyRoleAssignmentsAction), typeof(PasteFolderAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder))]
	public class PasteAllSubFoldersAction : PasteAction<PasteFolderOptions>
	{
		private static TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection> s_listTransformerDefinition;

		static PasteAllSubFoldersAction()
		{
			PasteAllSubFoldersAction.s_listTransformerDefinition = new TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection>("SharePoint Lists", true);
		}

		public PasteAllSubFoldersAction()
		{
		}

		private void CopySubFolders(SPFolderCollection sourceSubFolders, SPFolder targetFolder, bool IsContentTypeAlreadyMigrated = false)
		{
			ActionContext actionContext = new ActionContext(sourceSubFolders.ParentFolder, targetFolder);
			PasteFolderAction pasteFolderAction = new PasteFolderAction();
			pasteFolderAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteFolderAction);
			using (targetFolder)
			{
				foreach (SPFolder sourceSubFolder in sourceSubFolders)
				{
					if (!base.CheckForAbort())
					{
						if (sourceSubFolder.DisplayUrl != targetFolder.DisplayUrl)
						{
							object[] objArray = new object[] { sourceSubFolder, targetFolder, false, IsContentTypeAlreadyMigrated };
							pasteFolderAction.RunAsSubAction(objArray, actionContext, null);
						}
						sourceSubFolder.Dispose();
					}
					else
					{
						return;
					}
				}
			}
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(PasteAllSubFoldersAction.s_listTransformerDefinition);
			return supportedDefinitions;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPFolder sPFolder in target)
			{
				bool parserEnabled = true;
				try
				{
					if (base.SharePointOptions.DisableDocumentParsing)
					{
						parserEnabled = sPFolder.ParentList.ParentWeb.ParserEnabled;
						if (parserEnabled)
						{
							sPFolder.ParentList.ParentWeb.SetDocumentParsing(false);
						}
					}
					Node[] nodeArray = new Node[] { sPFolder };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					if (base.SharePointOptions.MapColumns || base.SharePointOptions.ApplyNewContentTypes)
					{
						base.RunPreCopyListUpdate((source[0] is SPList ? (SPList)source[0] : ((SPFolder)source[0]).ParentList), sPFolder, base.SharePointOptions);
					}
					foreach (SPFolder sPFolder1 in source)
					{
						try
						{
							if (!base.CheckForAbort())
							{
								this.CopySubFolders(sPFolder1.SubFolders, sPFolder, false);
							}
							else
							{
								return;
							}
						}
						finally
						{
							sPFolder1.Dispose();
						}
					}
				}
				finally
				{
					sPFolder.Dispose();
					if (base.SharePointOptions.DisableDocumentParsing)
					{
						if (parserEnabled)
						{
							sPFolder.ParentList.ParentWeb.SetDocumentParsing(true);
						}
						sPFolder.ParentList.ParentWeb.Dispose();
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			SPWeb parentWeb;
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format(Resources.ActionOperationMissingParameter, this.Name));
			}
			SPFolder sPFolder = oParams[0] as SPFolder;
			SPFolder sPFolder1 = oParams[1] as SPFolder;
			if (sPFolder1.ParentList != null)
			{
				parentWeb = sPFolder1.ParentList.ParentWeb;
			}
			else
			{
				parentWeb = null;
			}
			SPWeb sPWeb = parentWeb;
			bool flag = false;
			if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
			{
				flag = (bool)oParams[2];
			}
			if (sPFolder == null)
			{
				IXMLAbleList xMLAbleLists = oParams[0] as IXMLAbleList;
				if (xMLAbleLists == null)
				{
					throw new ArgumentException(string.Format(Resources.ActionOperationMissingParameter, this.Name, oParams[0].GetType(), typeof(SPFolderCollection)), "oParams[0]");
				}
				foreach (SPFolder sPFolder2 in xMLAbleLists)
				{
					if (!base.CheckForAbort())
					{
						base.DisableValidationSettings(sPFolder2.ParentList, sPFolder1, this.IsValidationSettingDisablingRequired);
						this.CopySubFolders(sPFolder2.SubFolders, sPFolder1, flag);
						if (sPWeb == null)
						{
							continue;
						}
						base.EnableValidationSettings(sPFolder2.ParentList, sPWeb);
					}
					else
					{
						return;
					}
				}
			}
			else
			{
				base.DisableValidationSettings(sPFolder.ParentList, sPFolder1, this.IsValidationSettingDisablingRequired);
				this.CopySubFolders(sPFolder.SubFolders, sPFolder1, flag);
				if (sPWeb != null)
				{
					base.EnableValidationSettings(sPFolder.ParentList, sPWeb);
					return;
				}
			}
		}
	}
}