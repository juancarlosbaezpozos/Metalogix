using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Permissions.ico")]
	[LaunchAsJob(true)]
	[Name("Paste Permissions")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SubActionTypes(new Type[] { typeof(PasteRolesAction), typeof(CopyRoleAssignmentsAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	public abstract class CopyPermissionsRecursivelyAction : PasteAction<CopyPermissionsOptions>
	{
		private SPOptimizationNode m_optimizationTree;

		protected bool HasPermissionsOptimization
		{
			get
			{
				return this.m_optimizationTree != null;
			}
		}

		public SPOptimizationNode OptimizationTree
		{
			get
			{
				return this.m_optimizationTree;
			}
		}

		public abstract SharePointObjectScope Scope
		{
			get;
		}

		protected CopyPermissionsRecursivelyAction()
		{
		}

		protected void CopyFolderCollectionRoleAssignments(SPFolder parentSourceFolder, SPFolder parentTargetFolder)
		{
			SPOptimizationNode sPOptimizationNode;
			SPFolderCollection subFolders = parentSourceFolder.SubFolders;
			SPFolderCollection sPFolderCollection = null;
			try
			{
				foreach (SPFolder subFolder in subFolders)
				{
					if (!base.CheckForAbort())
					{
						string displayUrl = null;
						try
						{
							if (this.HasPermissionsOptimization)
							{
								sPOptimizationNode = this.OptimizationTree.Find(subFolder.ServerRelativeUrl);
							}
							else
							{
								sPOptimizationNode = null;
							}
							SPOptimizationNode sPOptimizationNode1 = sPOptimizationNode;
							if (sPFolderCollection == null)
							{
								try
								{
									sPFolderCollection = parentTargetFolder.SubFolders;
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									LogItem logItem = new LogItem("Copy subfolders role assignments", parentSourceFolder.Name, parentSourceFolder.DisplayUrl, parentTargetFolder.DisplayUrl, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
									logItem.Exception = exception;
									logItem.Information = string.Concat("Could not fetch target lists - ", logItem.Information);
									base.FireOperationFinished(logItem);
									break;
								}
							}
							SPFolder item = (SPFolder)sPFolderCollection[subFolder.Name];
							if (item != null)
							{
								displayUrl = item.DisplayUrl;
								this.CopyFolderRoleAssignments(subFolder, item, sPOptimizationNode1);
							}
							else
							{
								LogItem logItem1 = new LogItem("Skipping Folder: No match found", subFolder.Name, subFolder.DisplayUrl, "", ActionOperationStatus.Skipped);
								base.FireOperationStarted(logItem1);
								base.FireOperationFinished(logItem1);
							}
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogItem logItem2 = new LogItem("Copy folder role assignments", subFolder.Name, subFolder.DisplayUrl, displayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem2);
							logItem2.Exception = exception2;
							base.FireOperationFinished(logItem2);
						}
					}
					else
					{
						return;
					}
				}
			}
			catch (Exception exception5)
			{
				Exception exception4 = exception5;
				LogItem logItem3 = new LogItem("Copy subfolders role assignments", parentSourceFolder.Name, parentSourceFolder.DisplayUrl, parentTargetFolder.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem3);
				logItem3.Exception = exception4;
				base.FireOperationFinished(logItem3);
			}
		}

		protected void CopyFolderRoleAssignments(SPFolder sourceFolder, SPFolder targetFolder, SPOptimizationNode node)
		{
			this.CopyFolderRoleAssignments(sourceFolder, targetFolder, node, false);
		}

		protected void CopyFolderRoleAssignments(SPFolder sourceFolder, SPFolder targetFolder, SPOptimizationNode node, bool bIsRootCopy)
		{
			bool flag;
			LogItem logItem = null;
			try
			{
				if (!this.HasPermissionsOptimization || node == null || !node.HasUniqueValues)
				{
					flag = (this.HasPermissionsOptimization ? false : sourceFolder.HasUniquePermissions);
				}
				else
				{
					flag = true;
				}
				bool flag1 = flag;
				if (base.SharePointOptions.CopyFolderPermissions && (flag1 || bIsRootCopy && base.SharePointOptions.CopyRootPermissions))
				{
					logItem = new LogItem("Copying folder role assignments", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyRoleAssignmentsAction);
					object[] objArray = new object[] { sourceFolder, targetFolder, (!base.SharePointOptions.CopyFolderPermissions ? true : !base.SharePointOptions.CopyItemPermissions) };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceFolder.ParentList.ParentWeb, targetFolder.ParentList.ParentWeb), null);
					logItem.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem == null)
				{
					logItem = new LogItem("Copying folder role assignments", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
				}
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			SPListItemCollection terseItems = null;
			SPListItemCollection sPListItemCollection = null;
			if (base.SharePointOptions.CopyItemPermissions)
			{
				GetListItemOptions getListItemOption = new GetListItemOptions()
				{
					IncludePermissionsInheritance = true
				};
				try
				{
					terseItems = sourceFolder.GetTerseItems(false, ListItemQueryType.ListItem, null, getListItemOption);
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					LogItem logItem1 = new LogItem("Copy list item permissions", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
					logItem1.Exception = exception2;
					logItem1.Information = string.Concat("Error getting source items for role assignment migration - ", logItem1.Information);
					base.FireOperationFinished(logItem1);
				}
				try
				{
					sPListItemCollection = targetFolder.GetTerseItems(false, ListItemQueryType.ListItem, null, getListItemOption);
				}
				catch (Exception exception5)
				{
					Exception exception4 = exception5;
					LogItem logItem2 = new LogItem("Copy list item permissions", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem2);
					logItem2.Exception = exception4;
					logItem2.Information = string.Concat("Error getting target items for role assignment migration - ", logItem2.Information);
					base.FireOperationFinished(logItem2);
				}
				if (terseItems != null && sPListItemCollection != null)
				{
					this.ResetTargetListItemCollectionRoleAssignments(terseItems, sPListItemCollection);
				}
				if (base.CheckForAbort())
				{
					return;
				}
			}
			if (base.SharePointOptions.CopyItemPermissions && terseItems != null && sPListItemCollection != null && (!this.HasPermissionsOptimization || node != null && node.Children.Count > 0))
			{
				this.CopyListItemCollectionRoleAssignments(terseItems, sPListItemCollection);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.RecursiveFolders)
			{
				this.CopyFolderCollectionRoleAssignments(sourceFolder, targetFolder);
			}
			sourceFolder.Dispose();
			targetFolder.Dispose();
		}

		protected void CopyListCollectionRoleAssignments(SPWeb sourceWeb, SPWeb targetWeb)
		{
			SPOptimizationNode sPOptimizationNode;
			SPListCollection lists = sourceWeb.Lists;
			SPListCollection sPListCollection = null;
			try
			{
				foreach (SPList list in lists)
				{
					if (!base.CheckForAbort())
					{
						string displayUrl = null;
						try
						{
							if (this.HasPermissionsOptimization)
							{
								sPOptimizationNode = this.OptimizationTree.Find(list.ServerRelativeUrl);
							}
							else
							{
								sPOptimizationNode = null;
							}
							SPOptimizationNode sPOptimizationNode1 = sPOptimizationNode;
							if (sPListCollection == null)
							{
								try
								{
									sPListCollection = targetWeb.Lists;
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									LogItem logItem = new LogItem("Copy lists role assignments", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
									logItem.Exception = exception;
									logItem.Information = string.Concat("Could not fetch target lists - ", logItem.Information);
									base.FireOperationFinished(logItem);
									break;
								}
							}
							SPList item = sPListCollection[list.Name];
							if (item != null)
							{
								displayUrl = item.DisplayUrl;
								this.CopyListRoleAssignments(list, item, sPOptimizationNode1);
							}
							else
							{
								LogItem logItem1 = new LogItem("Skipping List: No match found", list.Name, list.DisplayUrl, "", ActionOperationStatus.Skipped);
								base.FireOperationStarted(logItem1);
								base.FireOperationFinished(logItem1);
							}
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogItem logItem2 = new LogItem("Copy list role assignments", list.Name, list.DisplayUrl, displayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem2);
							logItem2.Exception = exception2;
							base.FireOperationFinished(logItem2);
						}
					}
					else
					{
						return;
					}
				}
			}
			catch (Exception exception5)
			{
				Exception exception4 = exception5;
				LogItem logItem3 = new LogItem("Copy lists role assignments", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem3);
				logItem3.Exception = exception4;
				base.FireOperationFinished(logItem3);
			}
		}

		protected void CopyListItemCollectionRoleAssignments(SPListItemCollection sourceItemCollection, SPListItemCollection targetItemCollection)
		{
			foreach (SPListItem sPListItem in sourceItemCollection)
			{
				string url = null;
				try
				{
					if (base.CheckForAbort())
					{
						return;
					}
					else if (sPListItem.HasUniquePermissions)
					{
						SPListItem sPListItem1 = null;
						sPListItem1 = (!((SPList)sourceItemCollection.ParentList).IsDocumentLibrary || !((SPList)targetItemCollection.ParentList).IsDocumentLibrary || sPListItem.ItemType == SPListItemType.Folder ? targetItemCollection.GetItemByID(sPListItem.ID) : targetItemCollection.GetItemByFileName(sPListItem["FileLeafRef"]));
						if (sPListItem1 != null)
						{
							url = sPListItem1.Url;
							this.CopyListItemRoleAssignments(sPListItem, sPListItem1);
						}
						else
						{
							LogItem logItem = new LogItem("Skipping Item: No match found", sPListItem.Name, sPListItem.DisplayUrl, "", ActionOperationStatus.Skipped);
							base.FireOperationStarted(logItem);
							base.FireOperationFinished(logItem);
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem1 = new LogItem("Copy item role assignments", sPListItem.Name, sPListItem.DisplayUrl, url, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
					logItem1.Exception = exception;
					base.FireOperationFinished(logItem1);
				}
			}
		}

		protected void CopyListItemRoleAssignments(SPListItem sourceListItem, SPListItem targetListItem)
		{
			this.CopyListItemRoleAssignments(sourceListItem, targetListItem, false);
		}

		protected void CopyListItemRoleAssignments(SPListItem sourceItem, SPListItem targetItem, bool bIsRootCopy)
		{
			LogItem logItem = null;
			try
			{
				if (base.SharePointOptions.CopyItemPermissions && sourceItem.HasUniquePermissions || bIsRootCopy && base.SharePointOptions.CopyRootPermissions)
				{
					logItem = new LogItem("Copying item role assignments", sourceItem.Name, sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyRoleAssignmentsAction);
					object[] objArray = new object[] { sourceItem, targetItem, false };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceItem.ParentList.ParentWeb, targetItem.ParentList.ParentWeb), null);
					logItem.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem == null)
				{
					logItem = new LogItem("Copying item role assignments", sourceItem.Name, sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
				}
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}

		protected void CopyListRoleAssignments(SPList sourceList, SPFolder targetList, SPOptimizationNode node)
		{
			this.CopyListRoleAssignments(sourceList, targetList, node, false);
		}

		protected void CopyListRoleAssignments(SPList sourceList, SPFolder targetList, SPOptimizationNode node, bool bIsRootCopy)
		{
			bool flag;
			LogItem logItem = null;
			try
			{
				if (!this.HasPermissionsOptimization || node == null || !node.HasUniqueValues)
				{
					flag = (this.HasPermissionsOptimization ? false : sourceList.HasUniquePermissions);
				}
				else
				{
					flag = true;
				}
				bool flag1 = flag;
				if (base.SharePointOptions.CopyListPermissions && (flag1 || bIsRootCopy && base.SharePointOptions.CopyRootPermissions))
				{
					logItem = new LogItem("Copying list role assignments", sourceList.Name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyRoleAssignmentsAction);
					object[] objArray = new object[] { sourceList, targetList, (!base.SharePointOptions.CopyListPermissions || !base.SharePointOptions.CopyFolderPermissions ? true : !base.SharePointOptions.CopyItemPermissions) };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceList.ParentWeb, targetList.ParentList.ParentWeb), null);
					logItem.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem == null)
				{
					logItem = new LogItem("Copying list role assignments", sourceList.Name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
				}
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			SPListItemCollection terseItems = null;
			SPListItemCollection sPListItemCollection = null;
			if (base.SharePointOptions.CopyItemPermissions)
			{
				GetListItemOptions getListItemOption = new GetListItemOptions()
				{
					IncludePermissionsInheritance = true
				};
				try
				{
					terseItems = sourceList.GetTerseItems(false, ListItemQueryType.ListItem, null, getListItemOption);
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					LogItem logItem1 = new LogItem("Copy list item permissions", sourceList.Name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
					logItem1.Exception = exception2;
					logItem1.Information = string.Concat("Error getting source items for role assignment migration - ", logItem1.Information);
					base.FireOperationFinished(logItem1);
				}
				try
				{
					sPListItemCollection = targetList.GetTerseItems(false, ListItemQueryType.ListItem, null, getListItemOption);
				}
				catch (Exception exception5)
				{
					Exception exception4 = exception5;
					LogItem logItem2 = new LogItem("Copy list item permissions", sourceList.Name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem2);
					logItem2.Exception = exception4;
					logItem2.Information = string.Concat("Error getting target items for role assignment migration - ", logItem2.Information);
					base.FireOperationFinished(logItem2);
				}
				if (terseItems != null && sPListItemCollection != null)
				{
					this.ResetTargetListItemCollectionRoleAssignments(terseItems, sPListItemCollection);
				}
				if (base.CheckForAbort())
				{
					return;
				}
			}
			if ((!this.HasPermissionsOptimization || node != null && node.Children.Count > 0) && base.SharePointOptions.CopyItemPermissions && (!this.HasPermissionsOptimization || node != null && node.Children.HasUniqueValuesAtThisLevel) && terseItems != null && sPListItemCollection != null)
			{
				this.CopyListItemCollectionRoleAssignments(terseItems, sPListItemCollection);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
			{
				this.CopyFolderCollectionRoleAssignments(sourceList, targetList);
			}
			sourceList.Dispose();
			targetList.Dispose();
		}

		protected void CopyWebCollectionRoleAssignments(SPWeb parentSourceWeb, SPWeb parentTargetWeb)
		{
			SPOptimizationNode sPOptimizationNode;
			try
			{
				SPWebCollection subWebs = parentSourceWeb.SubWebs;
				SPWebCollection sPWebCollection = null;
				foreach (SPWeb subWeb in subWebs)
				{
					if (!base.CheckForAbort())
					{
						string displayUrl = null;
						try
						{
							if (this.HasPermissionsOptimization)
							{
								sPOptimizationNode = this.OptimizationTree.Find(subWeb.ServerRelativeUrl);
							}
							else
							{
								sPOptimizationNode = null;
							}
							SPOptimizationNode sPOptimizationNode1 = sPOptimizationNode;
							if (sPWebCollection == null)
							{
								try
								{
									sPWebCollection = parentTargetWeb.SubWebs;
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									LogItem logItem = new LogItem("Copy subweb permissions", parentSourceWeb.Name, parentSourceWeb.DisplayUrl, parentTargetWeb.DisplayUrl, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem);
									logItem.Exception = exception;
									logItem.Information = string.Concat("Could not fetch target webs - ", logItem.Information);
									base.FireOperationFinished(logItem);
									break;
								}
							}
							SPWeb item = (SPWeb)sPWebCollection[subWeb.Name];
							if (item != null)
							{
								displayUrl = item.DisplayUrl;
								this.CopyWebRoleAssignments(subWeb, item, sPOptimizationNode1);
							}
							else
							{
								LogItem logItem1 = new LogItem("Skipping Web: No match found", subWeb.Name, subWeb.DisplayUrl, "", ActionOperationStatus.Skipped);
								base.FireOperationStarted(logItem1);
								base.FireOperationFinished(logItem1);
							}
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogItem logItem2 = new LogItem("Copy web permissions", subWeb.Name, subWeb.DisplayUrl, displayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem2);
							logItem2.Exception = exception2;
							base.FireOperationFinished(logItem2);
						}
					}
					else
					{
						return;
					}
				}
			}
			catch (Exception exception5)
			{
				Exception exception4 = exception5;
				LogItem logItem3 = new LogItem("Copy subweb permissions", parentSourceWeb.Name, parentSourceWeb.DisplayUrl, parentTargetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem3);
				logItem3.Exception = exception4;
				base.FireOperationFinished(logItem3);
			}
		}

		protected void CopyWebRoleAssignments(SPWeb sourceWeb, SPWeb targetWeb, SPOptimizationNode node)
		{
			this.CopyWebRoleAssignments(sourceWeb, targetWeb, node, false);
		}

		protected void CopyWebRoleAssignments(SPWeb sourceWeb, SPWeb targetWeb, SPOptimizationNode node, bool bIsRootCopy)
		{
			bool flag;
			try
			{
				if (base.SharePointOptions.CopyPermissionLevels && (sourceWeb.HasUniqueRoles || bIsRootCopy && base.SharePointOptions.CopyRootPermissions))
				{
					PasteRolesAction pasteRolesAction = new PasteRolesAction();
					base.SubActions.Add(pasteRolesAction);
					pasteRolesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					object[] roles = new object[] { sourceWeb.Roles as SPRoleCollection, targetWeb.Roles as SPRoleCollection };
					pasteRolesAction.RunAsSubAction(roles, new ActionContext(sourceWeb, targetWeb), null);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Copy Permission Levels", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
			LogItem logItem1 = null;
			try
			{
				if (!this.HasPermissionsOptimization || node == null || !node.HasUniqueValues)
				{
					flag = (this.HasPermissionsOptimization ? false : sourceWeb.HasUniquePermissions);
				}
				else
				{
					flag = true;
				}
				bool flag1 = flag;
				if (base.SharePointOptions.CopySitePermissions && (flag1 || bIsRootCopy && base.SharePointOptions.CopyRootPermissions))
				{
					logItem1 = new LogItem("Copying site role assignments", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
					CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyRoleAssignmentsAction);
					object[] objArray = new object[] { sourceWeb, targetWeb, (!base.SharePointOptions.CopySitePermissions || !base.SharePointOptions.CopyListPermissions || !base.SharePointOptions.CopyFolderPermissions || !base.SharePointOptions.CopyItemPermissions ? true : !base.SharePointOptions.RecursiveSites) };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceWeb, targetWeb), null);
					logItem1.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem1);
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				if (logItem1 == null)
				{
					logItem1 = new LogItem("Copying site role assignments", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem1);
				}
				logItem1.Exception = exception2;
				base.FireOperationFinished(logItem1);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.CopyListPermissions || base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
			{
				this.CopyListCollectionRoleAssignments(sourceWeb, targetWeb);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.RecursiveSites)
			{
				this.CopyWebCollectionRoleAssignments(sourceWeb, targetWeb);
			}
			sourceWeb.Dispose();
			targetWeb.Dispose();
		}

		protected void InitializeOptimizationTable(SPNode node)
		{
			this.m_optimizationTree = node.GetPermissionsOptimizationTree();
		}

		protected void ResetTargetListItemCollectionRoleAssignments(SPListItemCollection sourceItemCollection, SPListItemCollection targetItemCollection)
		{
			foreach (SPListItem sPListItem in sourceItemCollection)
			{
				if (!base.CheckForAbort())
				{
					SPListItem sPListItem1 = null;
					sPListItem1 = (!((SPList)sourceItemCollection.ParentList).IsDocumentLibrary || !((SPList)targetItemCollection.ParentList).IsDocumentLibrary || sPListItem.ItemType == SPListItemType.Folder ? targetItemCollection.GetItemByID(sPListItem.ID) : targetItemCollection.GetItemByFileName(sPListItem["FileLeafRef"]));
					if (!(sPListItem1 != null) || sPListItem.HasUniquePermissions || !sPListItem1.HasUniquePermissions)
					{
						continue;
					}
					CopyRoleAssignmentsAction.UpdateListItemRoleInheritance(sPListItem, sPListItem1);
				}
				else
				{
					return;
				}
			}
		}
	}
}