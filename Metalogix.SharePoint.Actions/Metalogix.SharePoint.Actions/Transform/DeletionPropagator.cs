using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class DeletionPropagator : PreconfiguredTransformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		public override string Name
		{
			get
			{
				return "Delete Propagation";
			}
		}

		public DeletionPropagator()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			bool flag = this.CheckPreservingItems(sources.ParentSPList, action);
			SPFolder parentFolder = targets.ParentFolder as SPFolder;
			int? nullable = null;
			SPDiscussionItemCollection sPDiscussionItemCollection = sources as SPDiscussionItemCollection;
			if (sPDiscussionItemCollection != null)
			{
				nullable = new int?(sPDiscussionItemCollection.SubjectID);
			}
			if (flag && action.SharePointOptions.PropagateItemDeletions && !nullable.HasValue)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						StringBuilder stringBuilder = new StringBuilder();
						List<SPListItem> sPListItems = new List<SPListItem>();
						for (int i = 0; i < targets.Count; i++)
						{
							SPListItem item = (SPListItem)targets[i];
							SPListItem itemByID = null;
							if (item.ItemType == SPListItemType.Folder || item.ParentList.IsDocumentLibrary)
							{
								foreach (SPListItem source in sources)
								{
									if (source.ItemType != item.ItemType)
									{
										continue;
									}
									string parentRelativePath = source.ParentRelativePath;
									string fileLeafRef = source.FileLeafRef;
									bool anticipatedFileLeafRefAndPath = this.GetAnticipatedFileLeafRefAndPath(action, source, ref fileLeafRef, ref parentRelativePath, source.ItemType == SPListItemType.Folder, parentFolder);
									if (source.ItemType == SPListItemType.Folder && anticipatedFileLeafRefAndPath)
									{
										string str = (string.IsNullOrEmpty(parentRelativePath) ? fileLeafRef : string.Concat(parentRelativePath, "/", fileLeafRef));
										action.LinkCorrector.AddMapping(source.ServerRelativeFolderLeafRef, string.Concat(parentFolder.ServerRelativeUrl, "/", str));
										action.LinkCorrector.AddMapping(source.ParentFolder.ServerRelativeUrl, parentFolder.ServerRelativeUrl);
									}
									if (!item.FileLeafRef.Equals(fileLeafRef, StringComparison.CurrentCultureIgnoreCase) || !item.ParentRelativePath.Equals(parentRelativePath, StringComparison.CurrentCultureIgnoreCase))
									{
										continue;
									}
									itemByID = source;
									break;
								}
							}
							else
							{
								itemByID = sources.GetItemByID(item.ID);
							}
							if (itemByID == null)
							{
								sPListItems.Add(item);
							}
						}
						if (sPListItems.Count > 0 && !MigrationUtils.IsListWithDefaultItems(targets.ParentSPList))
						{
							logItem = new LogItem("Propagating deletions", string.Concat(sPListItems.Count, " item", (sPListItems.Count > 1 ? "s" : "")), ((SPFolder)sources.ParentFolder).DisplayUrl, parentFolder.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							targets.DeleteItems(sPListItems.ToArray());
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem != null)
						{
							logItem.Exception = exception;
						}
					}
				}
				finally
				{
					if (logItem != null)
					{
						if (logItem.Status == ActionOperationStatus.Running)
						{
							logItem.Status = ActionOperationStatus.Completed;
						}
						base.FireOperationFinished(logItem);
					}
				}
			}
		}

		protected bool CheckPreservingItems(SPList sourceList, PasteListItemAction action)
		{
			if (action.SharePointOptions.ItemCopyingMode != ListItemCopyMode.Preserve)
			{
				return false;
			}
			if (sourceList.IsDocumentLibrary)
			{
				return true;
			}
			return action.SharePointOptions.PreserveItemIDs;
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		protected bool GetAnticipatedFileLeafRefAndPath(PasteListItemAction action, SPListItem sourceItem, ref string sFileLeafRef, ref string sParentFolderPath, bool bIsFolder, SPFolder targetFolder)
		{
			bool flag = false;
			if (action.SharePointOptions.RenameSpecificNodes)
			{
				if (!string.IsNullOrEmpty(sParentFolderPath))
				{
					SPFolder parentFolder = sourceItem.ParentFolder;
					if (parentFolder != null)
					{
						string serverRelativeUrl = parentFolder.ServerRelativeUrl;
						sParentFolderPath = action.LinkCorrector.CorrectUrl(string.Concat(serverRelativeUrl, "/", sParentFolderPath));
						serverRelativeUrl = action.LinkCorrector.CorrectUrl(serverRelativeUrl);
						sParentFolderPath = sParentFolderPath.Substring(serverRelativeUrl.Length + 1);
						sParentFolderPath = action.LinkCorrector.DecodeURL(sParentFolderPath);
					}
				}
				if (sourceItem.ItemType == SPListItemType.Folder)
				{
					foreach (TransformationTask transformationTask in action.SharePointOptions.TaskCollection.TransformationTasks)
					{
						if (!transformationTask.ChangeOperations.ContainsKey("FileLeafRef") || !transformationTask.ApplyTo.Evaluate(sourceItem, new CompareDatesInUtc()))
						{
							continue;
						}
						sFileLeafRef = transformationTask.ChangeOperations["FileLeafRef"];
						flag = true;
					}
				}
			}
			return flag;
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			return dataObject;
		}
	}
}