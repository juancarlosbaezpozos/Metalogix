using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointFolder", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.PasteFolder.png")]
	[Incrementable(true, "Paste Folder Incrementally")]
	[MandatoryTransformers(new Type[] { typeof(DocumentSetsFolderApplicator), typeof(ReferencedFolderDataUpdater), typeof(FolderColumnMapper) })]
	[MenuText("1:Paste Folder... {0-Paste}")]
	[MenuTextPlural("1:Paste Folders... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste Folder")]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPFolder), false)]
	[SubActionTypes(new Type[] { typeof(CopyRoleAssignmentsAction), typeof(PasteAllSubFoldersAction), typeof(PasteAllListItemsAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder))]
	public class PasteFolderAction : PasteAction<PasteFolderOptions>
	{
		private static TransformerDefinition<SPFolder, PasteFolderAction, SPFolderCollection, SPFolderCollection> s_folderTransformerDefinition;

		private static TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection> s_listTransformerDefinition;

		static PasteFolderAction()
		{
			PasteFolderAction.s_folderTransformerDefinition = new TransformerDefinition<SPFolder, PasteFolderAction, SPFolderCollection, SPFolderCollection>("SharePoint Folders", false);
			PasteFolderAction.s_listTransformerDefinition = new TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection>("SharePoint Lists", true);
		}

		public PasteFolderAction()
		{
		}

		private void CopyFolder(SPFolder sourceFolder, SPFolder targetFolder, bool bIsCopyRoot, bool isContentTypeAlreadyMigrated = false)
		{
			SPFolder item;
			if (base.CheckForAbort())
			{
				return;
			}
			if (sourceFolder.ParentList != null && sourceFolder.ParentList.ParentWeb.HasNintextFeature && targetFolder.Adapter.SharePointVersion.IsSharePointOnline && SPUtils.IsNintexWorkflow(sourceFolder.ParentList, sourceFolder.Name))
			{
				return;
			}
			if (!SPUtils.IsOneNoteFeatureEnabled(targetFolder) && SPUtils.IsDefaultOneNoteFolder(sourceFolder))
			{
				LogItem logItem = new LogItem("Skipping Folder", sourceFolder.Name, sourceFolder.DisplayUrl, SPUtils.OneNoteFolderDisplayName(targetFolder, true), ActionOperationStatus.Skipped)
				{
					Information = "Skipping Item because Site Notebook Feature is not activated at Target. To migrate the item, kindly activate Site Notebook feature."
				};
				LogItem logItem1 = logItem;
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
				return;
			}
			SPFolder sPFolder = null;
			bool flag = false;
			bool flag1 = false;
			if (flag)
			{
				if (this.CopyFolderInternalsForIncrementalCopying(sourceFolder, targetFolder, out sPFolder))
				{
					goto Label0;
				}
				LogItem logItem2 = null;
				logItem2 = new LogItem("Skipping Folder", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Skipped)
				{
					Information = "Filtered out by folder filters."
				};
				base.FireOperationStarted(logItem2);
			}
			else
			{
				LogItem xML = null;
				bool flag2 = false;
				string str = null;
				try
				{
					try
					{
						xML = new LogItem("Adding Folder", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
						string str1 = (sourceFolder is SPList ? (sourceFolder as SPList).Title : sourceFolder.Name);
						string str2 = (base.SharePointOptions.RenameSpecificNodes ? MigrationUtils.GetRenamedFolderName(sourceFolder, base.SharePointOptions.TaskCollection) : str1);
						PasteFolderAction pasteFolderAction = this;
						sourceFolder = PasteFolderAction.s_folderTransformerDefinition.Transform(sourceFolder, pasteFolderAction, sourceFolder.SubFolders, targetFolder.SubFolders, this.Options.Transformers);
						if (sourceFolder != null)
						{
							bool flag3 = false;
							bool updateItemOptionsBitField = base.SharePointOptions.UpdateItemOptionsBitField > 0;
							if (str2 != null)
							{
								item = (SPFolder)targetFolder.SubFolders[str2];
							}
							else
							{
								item = null;
							}
							SPFolder sPFolder1 = item;
							if (sPFolder1 != null)
							{
								if (base.SharePointOptions.OverwriteFolders)
								{
									sPFolder1.Delete();
									sPFolder1 = null;
								}
								else if (!updateItemOptionsBitField)
								{
									flag3 = true;
									xML.Operation = "Skipping Folder";
									xML.Information = "Folder skipped because it already exists. Content in the folder may still be copied depending on your folder updating settings.";
									xML.Status = ActionOperationStatus.Skipped;
								}
							}
							base.FireOperationStarted(xML);
							flag2 = true;
							if (!flag3)
							{
								str = sourceFolder.XML;
								str = this.RenameFolder(str, sourceFolder);
							}
							if (sPFolder1 != null)
							{
								sPFolder = sPFolder1;
								if (updateItemOptionsBitField && (base.SharePointOptions.UpdateItemOptionsBitField & 1) > 0)
								{
									sPFolder.UpdateSettings(str);
								}
							}
							else
							{
								AddFolderOptions addFolderOption = new AddFolderOptions()
								{
									Overwrite = false
								};
								if (!targetFolder.ParentList.IsDocumentLibrary)
								{
									addFolderOption.PreserveID = base.SharePointOptions.PreserveItemIDs;
								}
								else
								{
									addFolderOption.PreserveID = (!base.SharePointOptions.PreserveDocumentIDs ? false : SharePointConfigurationVariables.AllowDBWriting);
								}
								sPFolder = targetFolder.SubFolders.AddFolder(str, addFolderOption, AddFolderMode.Comprehensive);
								flag1 = true;
							}
							this.InitializeFolderLinkCorrection(sourceFolder, sPFolder);
							if (!flag3)
							{
								if (!base.SharePointOptions.CheckResults)
								{
									xML.Status = ActionOperationStatus.Completed;
								}
								else
								{
									base.CompareNodes(sourceFolder, sPFolder, xML);
								}
								xML.AddCompletionDetail(Resources.Migration_Detail_FoldersCopied, (long)1);
							}
							if (base.SharePointOptions.Verbose)
							{
								xML.SourceContent = str;
								xML.TargetContent = sPFolder.XML;
							}
						}
						else
						{
							return;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (!flag2)
						{
							base.FireOperationStarted(xML);
						}
						xML.SourceContent = (str != null ? str : "");
						xML.Exception = exception;
						return;
					}
					goto Label0;
				}
				finally
				{
					if (xML != null)
					{
						if (xML.Status == ActionOperationStatus.Running)
						{
							xML.Status = ActionOperationStatus.Completed;
						}
						base.FireOperationFinished(xML);
					}
				}
			}
			return;
		Label0:
			if (base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
			{
				if (!sPFolder.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload || !sPFolder.ParentList.IsMigrationPipelineSupportedForTarget)
				{
					Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
					string str3 = string.Concat(typeof(CopyRoleAssignmentsAction).Name, targetFolder.DirName);
					object[] objArray = new object[] { sourceFolder, sPFolder, flag1, bIsCopyRoot };
					threadManager.QueueBufferedTask(str3, objArray, new ThreadedOperationDelegate(this.CopyFolderPermissions));
				}
				else
				{
					object[] objArray1 = new object[] { sourceFolder, sPFolder, flag1, bIsCopyRoot };
					this.CopyFolderPermissions(objArray1);
				}
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.ApplyNewContentTypes && !base.SharePointOptions.CopyListItems && !isContentTypeAlreadyMigrated)
			{
				base.ApplyNewContentType(sourceFolder.ParentList, targetFolder.ParentList, base.SharePointOptions);
				isContentTypeAlreadyMigrated = true;
			}
			if (base.SharePointOptions.CopyListItems)
			{
				PasteAllListItemsAction pasteAllListItemsAction = new PasteAllListItemsAction();
				pasteAllListItemsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteAllListItemsAction);
				object[] objArray2 = new object[] { sourceFolder, sPFolder };
				pasteAllListItemsAction.RunAsSubAction(objArray2, new ActionContext(sourceFolder, targetFolder), null);
			}
			else if (base.SharePointOptions.CopySubFolders)
			{
				PasteAllSubFoldersAction pasteAllSubFoldersAction = new PasteAllSubFoldersAction();
				pasteAllSubFoldersAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteAllSubFoldersAction);
				object[] objArray3 = new object[] { sourceFolder, sPFolder, isContentTypeAlreadyMigrated };
				pasteAllSubFoldersAction.RunAsSubAction(objArray3, new ActionContext(sourceFolder, targetFolder), null);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (bIsCopyRoot && (!targetFolder.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload || !targetFolder.ParentList.IsMigrationPipelineSupportedForTarget))
			{
				base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, targetFolder.DirName), false, false);
			}
			sourceFolder.Dispose();
			sPFolder.Dispose();
		}

		private bool CopyFolderInternalsForIncrementalCopying(SPFolder sourceFolder, SPFolder targetContainer, out SPFolder existingTargetFolder)
		{
			string value;
			existingTargetFolder = null;
			if (0 <= base.SharePointOptions.FolderFilterExpression.GetExpressionString().IndexOf("modified", StringComparison.OrdinalIgnoreCase))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(this.RenameFolder(sourceFolder.XML, sourceFolder));
				if (xmlNode.Attributes["FileLeafRef"] != null)
				{
					value = xmlNode.Attributes["FileLeafRef"].Value;
				}
				else
				{
					value = null;
				}
				string str = value;
				if (!string.IsNullOrEmpty(str))
				{
					existingTargetFolder = targetContainer.SubFolders[str] as SPFolder;
				}
			}
			return existingTargetFolder != null;
		}

		private void CopyFolderPermissions(object[] oParams)
		{
			bool flag;
			SPFolder sPFolder = oParams[0] as SPFolder;
			SPFolder sPFolder1 = oParams[1] as SPFolder;
			bool flag1 = (bool)oParams[2];
			bool flag2 = (bool)oParams[3];
			if (!base.SharePointOptions.CopyFolderPermissions)
			{
				flag = false;
			}
			else if (flag1)
			{
				flag = true;
			}
			else
			{
				flag = (flag1 || !base.SharePointOptions.CopyItemPermissions && !base.SharePointOptions.CopyFolderPermissions || !base.SharePointOptions.UpdateItems ? false : (base.SharePointOptions.UpdateFolderOptionsBitField & 2) > 0);
			}
			if (flag && (flag2 && base.SharePointOptions.CopyRootPermissions || sPFolder.HasUniquePermissions))
			{
				CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
				base.SubActions.Add(copyRoleAssignmentsAction);
				copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				object[] objArray = new object[] { sPFolder, sPFolder1, !flag1 };
				copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPFolder.ParentList.ParentWeb, sPFolder1.ParentList.ParentWeb), null);
			}
			base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPFolder1.DirName), false, false);
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(PasteFolderAction.s_listTransformerDefinition);
			supportedDefinitions.Add(PasteFolderAction.s_folderTransformerDefinition);
			return supportedDefinitions;
		}

		private void InitializeFolderLinkCorrection(SPFolder sourceFolder, SPFolder targetFolder)
		{
			if (base.SharePointOptions.CorrectingLinks)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
						base.LinkCorrector.PopulateForFolderCopy(sourceFolder, targetFolder, base.SharePointOptions.TaskCollection, base.SharePointOptions.UseComprehensiveLinkCorrection);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem = new LogItem("Initialize Link Corrector for Folder Copy", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						logItem.Exception = exception;
						logItem.Details = exception.StackTrace;
					}
				}
				finally
				{
					if (logItem != null)
					{
						base.FireOperationFinished(logItem);
					}
				}
			}
		}

		private string RenameFolder(string sSourceXml, SPFolder sourceFolder)
		{
			string str = sSourceXml;
			if (base.SharePointOptions.RenameSpecificNodes)
			{
				TransformationTask task = base.SharePointOptions.TaskCollection.GetTask(sourceFolder, new CompareDatesInUtc());
				if (task != null)
				{
					str = task.PerformTransformation(sSourceXml);
				}
			}
			return str;
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
							if (base.CheckForAbort())
							{
								return;
							}
							else if (!(sPFolder1.ParentList is SPDiscussionList))
							{
								PasteFolderAction pasteFolderAction = this;
								PasteFolderAction.s_folderTransformerDefinition.BeginTransformation(pasteFolderAction, sPFolder1.SubFolders, sPFolder.SubFolders, this.Options.Transformers);
								this.CopyFolder(sPFolder1, sPFolder, true, false);
								PasteFolderAction.s_folderTransformerDefinition.EndTransformation(pasteFolderAction, sPFolder1.SubFolders, sPFolder.SubFolders, this.Options.Transformers);
							}
							else
							{
								base.InitializeAudienceMappings(sPFolder1, sPFolder);
								foreach (SPDiscussionItem discussionItem in ((SPDiscussionList)sPFolder1.ParentList).DiscussionItems)
								{
									if (string.Concat(discussionItem.FileDirRef, "/", discussionItem.FileLeafRef) != sPFolder1.ServerRelativeUrl)
									{
										continue;
									}
									PasteListItemAction pasteListItemAction = new PasteListItemAction();
									pasteListItemAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
									SPList parentList = sPFolder1.ParentList;
									SPListItem[] sPListItemArray = new SPListItem[] { discussionItem };
									SPListItemCollection sPListItemCollection = new SPListItemCollection(parentList, sPFolder1, sPListItemArray);
									base.SubActions.Add(pasteListItemAction);
									object[] objArray = new object[] { sPListItemCollection, sPFolder, null, true };
									pasteListItemAction.RunAsSubAction(objArray, new ActionContext(sPFolder1.ParentList.ParentWeb, sPFolder.ParentList.ParentWeb), null);
									break;
								}
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
					SPList sPList = sPFolder.ParentList;
					if (sPList != null)
					{
						sPList.Refresh();
					}
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
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			bool flag = false;
			if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
			{
				flag = (bool)oParams[2];
			}
			bool flag1 = false;
			if ((int)oParams.Length >= 4 && oParams[3] != null && oParams[3] is bool)
			{
				flag1 = (bool)oParams[3];
			}
			SPFolder sPFolder = oParams[1] as SPFolder;
			this.CopyFolder(oParams[0] as SPFolder, sPFolder, flag, flag1);
			SPList parentList = sPFolder.ParentList;
			if (parentList != null)
			{
				parentList.Refresh();
			}
		}
	}
}