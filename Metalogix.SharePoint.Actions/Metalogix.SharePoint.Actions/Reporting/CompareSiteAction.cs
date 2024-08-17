using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Reporting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Reporting
{
	[Batchable(false, "Compare Site in batch...")]
	[CmdletEnabled(true, "Compare-MLSharePointSite", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Reporting.Compare.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMCPublicFolder | ProductFlags.UnifiedContentMatrixExpressKey | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Compare Site... {1-copy-compare}")]
	[Name("Compare Site")]
	[RequiresFullEdition(false)]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetType(typeof(SPWeb))]
	[UsesStickySettings(false)]
	public class CompareSiteAction : PasteAction<CompareSiteOptions>
	{
		public CompareSiteAction()
		{
		}

	    // Metalogix.SharePoint.Actions.Reporting.CompareSiteAction
	    public bool CompareContentTypeCollection(SPContentTypeCollection sourceContentTypes, SPContentTypeCollection targetContentTypes)
	    {
	        bool flag = true;
	        foreach (SPContentType sPContentType in sourceContentTypes)
	        {
	            if (base.CheckForAbort())
	            {
	                bool result = flag;
	                return result;
	            }
	            SPContentType contentTypeByName = targetContentTypes.GetContentTypeByName(sPContentType.Name);
	            if (contentTypeByName == null)
	            {
	                string sSource = (sourceContentTypes.ParentList != null) ? sourceContentTypes.ParentList.DisplayUrl : sourceContentTypes.ParentWeb.DisplayUrl;
	                LogItem logItem = new LogItem("Comparing Content Types", sPContentType.Name, sSource, null, ActionOperationStatus.MissingOnTarget);
	                logItem.Information = "The content type '" + sPContentType.Name + "' does not exist on the target";
	                if (base.SharePointOptions.Verbose)
	                {
	                    logItem.SourceContent = sPContentType.XML;
	                }
	                base.FireOperationStarted(logItem);
	                if (base.SharePointOptions.HaltIfDifferent)
	                {
	                    bool result = false;
	                    return result;
	                }
	                flag = false;
	            }
	            else
	            {
	                bool flag2 = this.CompareContentTypes(sPContentType, contentTypeByName);
	                flag = (flag && flag2);
	                if (base.SharePointOptions.HaltIfDifferent && !flag)
	                {
	                    bool result = false;
	                    return result;
	                }
	            }
	        }
	        foreach (SPContentType sPContentType2 in targetContentTypes)
	        {
	            if (base.CheckForAbort())
	            {
	                bool result = flag;
	                return result;
	            }
	            if (FilterExpression.Evaluate(sPContentType2, base.SharePointOptions.SiteFilterExpression) && sourceContentTypes.GetContentTypeByName(sPContentType2.Name) == null)
	            {
	                string sTarget = (targetContentTypes.ParentList != null) ? targetContentTypes.ParentList.DisplayUrl : targetContentTypes.ParentWeb.DisplayUrl;
	                LogItem logItem2 = new LogItem("Comparing SubSites", sPContentType2.Name, null, sTarget, ActionOperationStatus.MissingOnSource);
	                logItem2.Information = "The content type '" + sPContentType2.Name + "' does not exist on the source";
	                if (base.SharePointOptions.Verbose)
	                {
	                    logItem2.TargetContent = sPContentType2.XML;
	                }
	                base.FireOperationStarted(logItem2);
	                if (base.SharePointOptions.HaltIfDifferent)
	                {
	                    bool result = false;
	                    return result;
	                }
	                flag = false;
	            }
	        }
	        return flag;
	    }

        public bool CompareContentTypes(SPContentType sourceContentType, SPContentType targetContentType)
		{
			bool flag;
			LogItem logItem = null;
			try
			{
				try
				{
					string str = (sourceContentType.ParentCollection.ParentList != null ? sourceContentType.ParentCollection.ParentList.DisplayUrl : sourceContentType.ParentCollection.ParentWeb.DisplayUrl);
					string str1 = (targetContentType.ParentCollection.ParentList != null ? targetContentType.ParentCollection.ParentList.DisplayUrl : targetContentType.ParentCollection.ParentWeb.DisplayUrl);
					logItem = new LogItem("Comparing Content Types", sourceContentType.Name, str, str1, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					if (!base.SharePointOptions.CheckResults)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						base.CompareNodes(sourceContentType, targetContentType, logItem);
					}
					if (base.SharePointOptions.Verbose)
					{
						logItem.SourceContent = sourceContentType.XML;
						logItem.TargetContent = targetContentType.XML;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Exception = exception;
					flag = false;
					return flag;
				}
				return logItem.Status == ActionOperationStatus.Completed;
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
			return flag;
		}

		public bool CompareDiscussionItemCollection(SPListItemCollection sourceDiscussionItems, SPListItemCollection targetDiscussionItems)
		{
			SPListItemCollection sPListItemCollection;
			SPListItemCollection sPListItemCollection1;
			bool flag;
			LogItem logItem;
			bool flag1;
			SPDiscussionItem item;
			bool flag2 = true;
			if (sourceDiscussionItems.Count < targetDiscussionItems.Count)
			{
				flag = false;
				sPListItemCollection = targetDiscussionItems;
				sPListItemCollection1 = sourceDiscussionItems;
			}
			else
			{
				flag = true;
				sPListItemCollection = sourceDiscussionItems;
				sPListItemCollection1 = targetDiscussionItems;
			}
			int num = 0;
			using (IEnumerator<Node> enumerator = sPListItemCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SPDiscussionItem current = (SPDiscussionItem)enumerator.Current;
					if (!base.CheckForAbort())
					{
						if (current != null && base.SharePointOptions.FilterItems && !FilterExpression.Evaluate(current, base.SharePointOptions.ItemFilterExpression))
						{
							continue;
						}
						if (sPListItemCollection1.Count > num)
						{
							item = (SPDiscussionItem)sPListItemCollection1[num];
						}
						else
						{
							item = null;
						}
						SPDiscussionItem sPDiscussionItem = item;
						if (sPDiscussionItem != null)
						{
							flag2 = (!flag2 ? false : (flag ? this.CompareDiscussionItems(current, sPDiscussionItem) : this.CompareDiscussionItems(sPDiscussionItem, current)));
							if (!base.SharePointOptions.HaltIfDifferent || flag2)
							{
								num++;
							}
							else
							{
								flag1 = flag2;
								return flag1;
							}
						}
						else
						{
							if (!flag)
							{
								logItem = new LogItem("Comparing Discussion Item", current.DisplayName, null, current.DisplayUrl, ActionOperationStatus.MissingOnSource)
								{
									Information = string.Concat("The source discussion '", current.DisplayName, "' does not exist. ")
								};
								if (base.SharePointOptions.Verbose)
								{
									logItem.TargetContent = current.XML;
								}
							}
							else
							{
								logItem = new LogItem("Comparing Discussion Item", current.DisplayName, current.DisplayUrl, null, ActionOperationStatus.MissingOnTarget)
								{
									Information = string.Concat("The target discussion '", current.DisplayName, "' does not exist. ")
								};
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = current.XML;
								}
							}
							base.FireOperationStarted(logItem);
							if (!base.SharePointOptions.HaltIfDifferent)
							{
								flag2 = false;
							}
							else
							{
								flag1 = false;
								return flag1;
							}
						}
					}
					else
					{
						flag1 = flag2;
						return flag1;
					}
				}
				return flag2;
			}
			return flag1;
		}

		public bool CompareDiscussionItems(SPDiscussionItem sourceDiscussion, SPDiscussionItem targetDiscussion)
		{
			bool flag;
			bool status = true;
			LogItem logItem = null;
			try
			{
				try
				{
					logItem = new LogItem("Comparing Discussion", sourceDiscussion.Name, sourceDiscussion.DisplayUrl, targetDiscussion.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					if (!base.SharePointOptions.CheckResults)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						base.CompareNodes(sourceDiscussion, targetDiscussion, logItem);
					}
					if (base.SharePointOptions.Verbose)
					{
						logItem.SourceContent = sourceDiscussion.XML;
						logItem.TargetContent = targetDiscussion.XML;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Information = string.Concat("Exception thrown: ", exception.Message);
					logItem.Details = exception.StackTrace;
					flag = false;
					return flag;
				}
				status = logItem.Status == ActionOperationStatus.Completed;
				if (base.SharePointOptions.HaltIfDifferent && !status)
				{
					return status;
				}
				bool flag1 = this.CompareDiscussionItemCollection(sourceDiscussion.DiscussionItems, targetDiscussion.DiscussionItems);
				status = (!status ? false : flag1);
				return status;
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
			return flag;
		}

	    // Metalogix.SharePoint.Actions.Reporting.CompareSiteAction
	    public bool CompareFolderCollections(SPFolderCollection sourceFolders, SPFolderCollection targetFolders)
	    {
	        bool flag = true;
	        using (IEnumerator<Node> enumerator = sourceFolders.GetEnumerator())
	        {
	            while (enumerator.MoveNext())
	            {
	                SPFolder sPFolder = (SPFolder)enumerator.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPFolder is SPList) || !base.SharePointOptions.FilterLists || FilterExpression.Evaluate(sPFolder, base.SharePointOptions.ListFilterExpression))
	                {
	                    string text = (sPFolder is SPList) ? "List" : "Folder";
	                    SPFolder sPFolder2 = (SPFolder)targetFolders[sPFolder.Name];
	                    if (sPFolder2 == null)
	                    {
	                        LogItem logItem = new LogItem("Comparing " + text, sPFolder.Name, sPFolder.DisplayUrl, null, ActionOperationStatus.MissingOnTarget);
	                        logItem.Information = string.Concat(new string[]
	                        {
	                            "The target ",
	                            text,
	                            ": '",
	                            sPFolder.Name,
	                            "', does not exist. "
	                        });
	                        if (base.SharePointOptions.Verbose)
	                        {
	                            logItem.SourceContent = sPFolder.XML;
	                        }
	                        base.FireOperationStarted(logItem);
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                    else
	                    {
	                        bool flag2 = this.CompareFolders(sPFolder, sPFolder2);
	                        flag = (flag && flag2);
	                        if (base.SharePointOptions.HaltIfDifferent && !flag)
	                        {
	                            bool result = flag;
	                            return result;
	                        }
	                    }
	                }
	            }
	        }
	        using (IEnumerator<Node> enumerator2 = targetFolders.GetEnumerator())
	        {
	            while (enumerator2.MoveNext())
	            {
	                SPFolder sPFolder3 = (SPFolder)enumerator2.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPFolder3 is SPList) || FilterExpression.Evaluate(sPFolder3, base.SharePointOptions.ListFilterExpression))
	                {
	                    string text2 = (sPFolder3 is SPList) ? "List" : "Folder";
	                    if ((SPFolder)sourceFolders[sPFolder3.Name] == null)
	                    {
	                        base.FireOperationStarted(new LogItem("Comparing " + text2, sPFolder3.Name, null, sPFolder3.DisplayUrl, ActionOperationStatus.MissingOnSource)
	                        {
	                            Information = string.Concat(new string[]
	                            {
	                                "The source ",
	                                text2,
	                                ": '",
	                                sPFolder3.Name,
	                                "', does not exist. "
	                            })
	                        });
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                }
	            }
	        }
	        return flag;
	    }

        public bool CompareFolders(SPFolder sourceFolder, SPFolder targetFolder)
		{
			bool flag;
			bool status = true;
			LogItem logItem = null;
			string str = (sourceFolder is SPList ? "List" : "Folder");
			try
			{
				try
				{
					try
					{
						logItem = new LogItem(string.Concat("Comparing ", str), sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						if (!base.SharePointOptions.CheckResults)
						{
							logItem.Status = ActionOperationStatus.Completed;
						}
						else
						{
							base.CompareNodes(sourceFolder, targetFolder, logItem);
						}
						if (base.SharePointOptions.Verbose)
						{
							logItem.SourceContent = sourceFolder.XML;
							logItem.TargetContent = targetFolder.XML;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = string.Concat("Exception thrown: ", exception.Message);
						logItem.Details = exception.StackTrace;
						flag = false;
						return flag;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
				status = logItem.Status == ActionOperationStatus.Completed;
				if (!base.SharePointOptions.HaltIfDifferent || status)
				{
					if (sourceFolder.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard || targetFolder.ParentList.BaseTemplate != ListTemplateType.DiscussionBoard)
					{
						if (base.SharePointOptions.CompareItems)
						{
							SPListItemCollection items = null;
							SPListItemCollection sPListItemCollection = null;
							try
							{
								items = sourceFolder.GetItems(false, ListItemQueryType.ListItem, null);
								sPListItemCollection = targetFolder.GetItems(false, ListItemQueryType.ListItem, null);
							}
							catch (Exception exception3)
							{
								Exception exception2 = exception3;
								LogItem logItem1 = new LogItem("Comparing Items", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem1);
								logItem1.Exception = exception2;
								base.FireOperationFinished(logItem1);
							}
							if (items != null && sPListItemCollection != null)
							{
								bool flag1 = this.CompareListItemCollections(items, sPListItemCollection);
								status = (!status ? false : flag1);
							}
							if (base.SharePointOptions.HaltIfDifferent && !status)
							{
								flag = status;
								return flag;
							}
						}
						if (base.SharePointOptions.CompareFolders)
						{
							SPFolderCollection subFolders = null;
							SPFolderCollection sPFolderCollection = null;
							try
							{
								subFolders = sourceFolder.SubFolders;
								sPFolderCollection = targetFolder.SubFolders;
							}
							catch (Exception exception5)
							{
								Exception exception4 = exception5;
								LogItem logItem2 = new LogItem("Comparing Folders", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem2);
								logItem2.Exception = exception4;
								base.FireOperationFinished(logItem2);
							}
							if (subFolders != null && sPFolderCollection != null)
							{
								bool flag2 = this.CompareFolderCollections(subFolders, sPFolderCollection);
								status = (!status ? false : flag2);
							}
							if (base.SharePointOptions.HaltIfDifferent && !status)
							{
								flag = status;
								return flag;
							}
						}
					}
					else if (base.SharePointOptions.CompareItems)
					{
						SPListItemCollection discussionItems = null;
						SPListItemCollection discussionItems1 = null;
						try
						{
							discussionItems = ((SPDiscussionList)sourceFolder).DiscussionItems;
							discussionItems1 = ((SPDiscussionList)targetFolder).DiscussionItems;
						}
						catch (Exception exception7)
						{
							Exception exception6 = exception7;
							LogItem logItem3 = new LogItem("Comparing Discussion Items", sourceFolder.Name, sourceFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem3);
							logItem3.Exception = exception6;
							base.FireOperationFinished(logItem3);
						}
						if (discussionItems != null && discussionItems1 != null)
						{
							bool flag3 = this.CompareDiscussionItemCollection(discussionItems, discussionItems1);
							status = (!status ? false : flag3);
						}
						if (base.SharePointOptions.HaltIfDifferent && !status)
						{
							flag = status;
							return flag;
						}
					}
					return status;
				}
				else
				{
					flag = status;
				}
			}
			finally
			{
				sourceFolder.Dispose();
				targetFolder.Dispose();
			}
			return flag;
		}

	    // Metalogix.SharePoint.Actions.Reporting.CompareSiteAction
	    public bool CompareItemVersionCollection(SPListItemVersionCollection sourceVersions, SPListItemVersionCollection targetVersions)
	    {
	        bool flag = true;
	        using (IEnumerator<Node> enumerator = sourceVersions.GetEnumerator())
	        {
	            while (enumerator.MoveNext())
	            {
	                SPListItemVersion sPListItemVersion = (SPListItemVersion)enumerator.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPListItemVersion != null) || FilterExpression.Evaluate(sPListItemVersion, base.SharePointOptions.ItemFilterExpression))
	                {
	                    SPListItemVersion versionByVersionString = targetVersions.GetVersionByVersionString(sPListItemVersion.VersionString);
	                    if (versionByVersionString == null)
	                    {
	                        LogItem logItem = new LogItem("Comparing Version", sPListItemVersion.VersionString, sPListItemVersion.DisplayUrl, null, ActionOperationStatus.MissingOnTarget);
	                        logItem.Information = "The target version: '" + sPListItemVersion.VersionString + "', does not exist. ";
	                        if (base.SharePointOptions.Verbose)
	                        {
	                            logItem.SourceContent = sPListItemVersion.XML;
	                        }
	                        base.FireOperationStarted(logItem);
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                    else
	                    {
	                        bool flag2 = this.CompareItemVersions(sPListItemVersion, versionByVersionString);
	                        flag = (flag && flag2);
	                        if (base.SharePointOptions.HaltIfDifferent && !flag)
	                        {
	                            bool result = flag;
	                            return result;
	                        }
	                    }
	                }
	            }
	        }
	        using (IEnumerator<Node> enumerator2 = targetVersions.GetEnumerator())
	        {
	            while (enumerator2.MoveNext())
	            {
	                SPListItemVersion sPListItemVersion2 = (SPListItemVersion)enumerator2.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPListItemVersion2 != null) || FilterExpression.Evaluate(sPListItemVersion2, base.SharePointOptions.SiteFilterExpression))
	                {
	                    SPListItemVersion versionByVersionString2 = sourceVersions.GetVersionByVersionString(sPListItemVersion2.VersionString);
	                    if (versionByVersionString2 == null)
	                    {
	                        LogItem logItem2 = new LogItem("Comparing Version", sPListItemVersion2.VersionString, null, sPListItemVersion2.DisplayUrl, ActionOperationStatus.MissingOnSource);
	                        logItem2.Information = "The source Version: '" + sPListItemVersion2.VersionString + "', does not exist. ";
	                        if (base.SharePointOptions.Verbose)
	                        {
	                            logItem2.TargetContent = sPListItemVersion2.XML;
	                        }
	                        base.FireOperationStarted(logItem2);
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                }
	            }
	        }
	        return flag;
	    }

        public bool CompareItemVersions(SPListItemVersion sourceVersion, SPListItemVersion targetVersion)
		{
			bool flag;
			bool flag1 = true;
			LogItem logItem = null;
			try
			{
				try
				{
					logItem = new LogItem("Comparing Version", sourceVersion.VersionString, sourceVersion.DisplayUrl, targetVersion.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					if (!base.SharePointOptions.CheckResults)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						base.CompareNodes(sourceVersion, targetVersion, logItem);
					}
					if (base.SharePointOptions.Verbose)
					{
						logItem.SourceContent = sourceVersion.XML;
						logItem.TargetContent = targetVersion.XML;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Information = string.Concat("Exception thrown: ", exception.Message);
					logItem.Details = exception.StackTrace;
					flag = false;
					return flag;
				}
				flag1 = (!flag1 ? false : logItem.Status == ActionOperationStatus.Completed);
				return flag1;
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
			return flag;
		}

		public bool CompareListCollections(SPListCollection sourceLists, SPListCollection targetLists)
		{
			return this.CompareFolderCollections(sourceLists, targetLists);
		}
        
	    public bool CompareListItemCollections(SPListItemCollection sourceItems, SPListItemCollection targetItems)
	    {
	        bool flag = true;
	        using (IEnumerator<Node> enumerator = sourceItems.GetEnumerator())
	        {
	            while (enumerator.MoveNext())
	            {
	                SPListItem sPListItem = (SPListItem)enumerator.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPListItem != null) || !base.SharePointOptions.FilterItems || FilterExpression.Evaluate(sPListItem, base.SharePointOptions.ItemFilterExpression))
	                {
	                    SPListItem sPListItem2;
	                    if (((SPList)sourceItems.ParentList).IsDocumentLibrary && ((SPList)targetItems.ParentList).BaseType == ListType.DocumentLibrary && sPListItem.ItemType != SPListItemType.Folder)
	                    {
	                        sPListItem2 = targetItems.GetItemByFileName(sPListItem.FileLeafRef);
	                    }
	                    else
	                    {
	                        sPListItem2 = targetItems.GetItemByID(sPListItem.ID);
	                    }
	                    if (sPListItem2 == null)
	                    {
	                        LogItem logItem = new LogItem("Comparing Item", this.ConstructItemName(sPListItem), sPListItem.DisplayUrl, null, ActionOperationStatus.MissingOnTarget);
	                        logItem.Information = "The target item: '" + sPListItem.Name + "', does not exist. ";
	                        if (base.SharePointOptions.Verbose)
	                        {
	                            logItem.SourceContent = sPListItem.XMLWithVersions;
	                        }
	                        base.FireOperationStarted(logItem);
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                    else
	                    {
	                        bool flag2 = this.CompareListItems(sPListItem, sPListItem2);
	                        flag = (flag && flag2);
	                        if (base.SharePointOptions.HaltIfDifferent && !flag)
	                        {
	                            bool result = flag;
	                            return result;
	                        }
	                    }
	                }
	            }
	        }
	        using (IEnumerator<Node> enumerator2 = targetItems.GetEnumerator())
	        {
	            while (enumerator2.MoveNext())
	            {
	                SPListItem sPListItem3 = (SPListItem)enumerator2.Current;
	                if (base.CheckForAbort())
	                {
	                    bool result = flag;
	                    return result;
	                }
	                if (!(sPListItem3 != null) || FilterExpression.Evaluate(sPListItem3, base.SharePointOptions.ItemFilterExpression))
	                {
	                    SPListItem item;
	                    if (((SPList)sourceItems.ParentList).IsDocumentLibrary && ((SPList)targetItems.ParentList).IsDocumentLibrary)
	                    {
	                        item = sourceItems.GetItemByFileName(sPListItem3.FileLeafRef);
	                    }
	                    else
	                    {
	                        item = sourceItems.GetItemByID(sPListItem3.ID);
	                    }
	                    if (item == null)
	                    {
	                        LogItem logItem2 = new LogItem("Comparing Item", this.ConstructItemName(sPListItem3), null, sPListItem3.DisplayUrl, ActionOperationStatus.MissingOnSource);
	                        logItem2.Information = "The source item: '" + sPListItem3.Name + "', does not exist. ";
	                        if (base.SharePointOptions.Verbose)
	                        {
	                            logItem2.TargetContent = sPListItem3.XML;
	                        }
	                        base.FireOperationStarted(logItem2);
	                        if (base.SharePointOptions.HaltIfDifferent)
	                        {
	                            bool result = false;
	                            return result;
	                        }
	                        flag = false;
	                    }
	                }
	            }
	        }
	        return flag;
	    }

        public bool CompareListItems(SPListItem sourceItem, SPListItem targetItem)
		{
			bool flag;
			bool flag1 = true;
			LogItem logItem = null;
			try
			{
				try
				{
					try
					{
						logItem = new LogItem("Comparing Item", this.ConstructItemName(sourceItem), sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						if (!base.SharePointOptions.CheckResults)
						{
							logItem.Status = ActionOperationStatus.Completed;
						}
						else
						{
							base.CompareNodes(sourceItem, targetItem, logItem);
						}
						if (base.SharePointOptions.Verbose)
						{
							logItem.SourceContent = sourceItem.XMLWithVersions;
							logItem.TargetContent = targetItem.XMLWithVersions;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = string.Concat("Exception thrown: ", exception.Message);
						logItem.Details = exception.StackTrace;
						flag = false;
						return flag;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
				flag1 = (!flag1 ? false : logItem.Status == ActionOperationStatus.Completed);
				if (!base.SharePointOptions.HaltIfDifferent || flag1)
				{
					if (base.SharePointOptions.CompareVersions && sourceItem.CanHaveVersions && targetItem.CanHaveVersions)
					{
						SPListItemVersionCollection versionHistory = null;
						SPListItemVersionCollection sPListItemVersionCollection = null;
						try
						{
							versionHistory = (SPListItemVersionCollection)sourceItem.VersionHistory;
							sPListItemVersionCollection = (SPListItemVersionCollection)targetItem.VersionHistory;
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogItem logItem1 = new LogItem("Comparing Item Versions", sourceItem.Name, sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem1);
							logItem1.Exception = exception2;
							base.FireOperationFinished(logItem1);
						}
						if (versionHistory != null && versionHistory.Count > 1 && sPListItemVersionCollection != null && sPListItemVersionCollection.Count > 1)
						{
							bool flag2 = this.CompareItemVersionCollection(versionHistory, sPListItemVersionCollection);
							flag1 = (!flag1 ? false : flag2);
						}
						if (base.SharePointOptions.HaltIfDifferent && !flag1)
						{
							flag = flag1;
							return flag;
						}
					}
					return flag1;
				}
				else
				{
					flag = flag1;
				}
			}
			finally
			{
				sourceItem.Dispose();
				targetItem.Dispose();
			}
			return flag;
		}

		public bool CompareSubWebs(SPWebCollection sourceSubWebs, SPWebCollection targetSubWebs)
		{
			bool flag;
			bool flag1 = true;
			LogItem xML = null;
			try
			{
				foreach (SPWeb sourceSubWeb in sourceSubWebs)
				{
					try
					{
						if (base.CheckForAbort())
						{
							flag = flag1;
							return flag;
						}
						else if (!base.SharePointOptions.FilterSites || FilterExpression.Evaluate(sourceSubWeb, base.SharePointOptions.SiteFilterExpression))
						{
							SPWeb item = (SPWeb)targetSubWebs[sourceSubWeb.Name];
							if (item != null)
							{
								bool flag2 = this.CompareWebs(sourceSubWeb, item);
								flag1 = (!flag1 ? false : flag2);
								if (base.SharePointOptions.HaltIfDifferent && !flag1)
								{
									flag = false;
									return flag;
								}
							}
							else
							{
								LogItem logItem = new LogItem("Comparing SubSites", sourceSubWeb.Name, sourceSubWeb.DisplayUrl, null, ActionOperationStatus.MissingOnTarget)
								{
									Information = string.Concat("The site: '", sourceSubWeb.Name, "' does not exist on the target")
								};
								xML = logItem;
								if (base.SharePointOptions.Verbose)
								{
									xML.SourceContent = sourceSubWeb.XML;
								}
								base.FireOperationStarted(xML);
								if (!base.SharePointOptions.HaltIfDifferent)
								{
									flag1 = false;
								}
								else
								{
									flag = false;
									return flag;
								}
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.LogCompareSubWebException(sourceSubWeb.Name, sourceSubWebs.ParentWeb.DisplayUrl, targetSubWebs.ParentWeb.DisplayUrl, exception);
					}
				}
				foreach (SPWeb targetSubWeb in targetSubWebs)
				{
					try
					{
						if (base.CheckForAbort())
						{
							flag = flag1;
							return flag;
						}
						else if (FilterExpression.Evaluate(targetSubWeb, base.SharePointOptions.SiteFilterExpression))
						{
							if ((SPWeb)sourceSubWebs[targetSubWeb.Name] == null)
							{
								LogItem logItem1 = new LogItem("Comparing SubSites", targetSubWeb.Name, null, targetSubWeb.DisplayUrl, ActionOperationStatus.MissingOnSource)
								{
									Information = string.Concat("The site: '", targetSubWeb.Name, "' does not exist on the source")
								};
								xML = logItem1;
								if (base.SharePointOptions.Verbose)
								{
									xML.TargetContent = targetSubWeb.XML;
								}
								base.FireOperationStarted(xML);
								if (!base.SharePointOptions.HaltIfDifferent)
								{
									flag1 = false;
								}
								else
								{
									flag = false;
									return flag;
								}
							}
						}
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						this.LogCompareSubWebException(targetSubWeb.Name, sourceSubWebs.ParentWeb.Url, targetSubWebs.ParentWeb.Url, exception2);
					}
				}
				return flag1;
			}
			catch (Exception exception5)
			{
				Exception exception4 = exception5;
				this.LogCompareSubWebException(null, sourceSubWebs.ParentWeb.Url, targetSubWebs.ParentWeb.Url, exception4);
				return flag1;
			}
			return flag;
		}

		public bool CompareWebs(SPWeb sourceWeb, SPWeb targetWeb)
		{
			bool flag;
			bool status = true;
			LogItem logItem = null;
			try
			{
				try
				{
					try
					{
						logItem = new LogItem("Comparing Site", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						if (!base.SharePointOptions.CheckResults)
						{
							logItem.Status = ActionOperationStatus.Completed;
						}
						else
						{
							base.CompareNodes(sourceWeb, targetWeb, logItem);
						}
						if (base.SharePointOptions.Verbose)
						{
							logItem.SourceContent = sourceWeb.XML;
							logItem.TargetContent = targetWeb.XML;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = string.Concat("Exception thrown: ", exception.Message);
						logItem.Details = exception.StackTrace;
						flag = false;
						return flag;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
				status = logItem.Status == ActionOperationStatus.Completed;
				if (!base.SharePointOptions.HaltIfDifferent || status)
				{
					if (base.SharePointOptions.CompareLists)
					{
						SPListCollection lists = null;
						SPListCollection sPListCollection = null;
						try
						{
							lists = sourceWeb.Lists;
							sPListCollection = targetWeb.Lists;
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogItem logItem1 = new LogItem("Comparing Lists", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem1);
							logItem1.Exception = exception2;
							base.FireOperationFinished(logItem1);
						}
						if (lists != null && sPListCollection != null)
						{
							bool flag1 = this.CompareListCollections(lists, sPListCollection);
							status = (!status ? false : flag1);
						}
						if (base.SharePointOptions.HaltIfDifferent && !status)
						{
							flag = status;
							return flag;
						}
					}
					SPWebCollection subWebs = null;
					SPWebCollection sPWebCollection = null;
					try
					{
						subWebs = sourceWeb.SubWebs;
						sPWebCollection = targetWeb.SubWebs;
					}
					catch (Exception exception5)
					{
						Exception exception4 = exception5;
						LogItem logItem2 = new LogItem("Comparing Webs", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem2);
						logItem2.Exception = exception4;
						base.FireOperationFinished(logItem2);
					}
					sourceWeb.Dispose();
					targetWeb.Dispose();
					if (base.SharePointOptions.Recursive && subWebs != null && sPWebCollection != null)
					{
						bool flag2 = this.CompareSubWebs(subWebs, sPWebCollection);
						status = (!status ? false : flag2);
						if (base.SharePointOptions.HaltIfDifferent && !status)
						{
							flag = status;
							return flag;
						}
					}
					return status;
				}
				else
				{
					flag = status;
				}
			}
			finally
			{
				sourceWeb.Dispose();
				targetWeb.Dispose();
			}
			return flag;
		}

		private string ConstructItemName(SPListItem spListItem)
		{
			if (!spListItem.IsNameConverted)
			{
				return spListItem.Name;
			}
			return string.Format("{0},Title=\"{1}\"", spListItem.Name, spListItem.Title);
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			List<string> optionsToIncludeInPowerShell = this.GetOptionsToIncludeInPowerShell();
			PropertyInfo[] optionParameters = base.GetOptionParameters(cmdletOptions);
			for (int i = 0; i < (int)optionParameters.Length; i++)
			{
				PropertyInfo propertyInfo = optionParameters[i];
				if (optionsToIncludeInPowerShell.Contains(propertyInfo.Name))
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		protected virtual List<string> GetOptionsToIncludeInPowerShell()
		{
			List<string> strs = new List<string>()
			{
				"CompareFolders",
				"CompareItems",
				"CompareVersions",
				"CheckResults",
				"Verbose",
				"HaltIfDifferent",
				"FilterLists",
				"ListFilterExpression",
				"FilterItems",
				"ItemFilterExpression",
				"CompareLists",
				"Recursive",
				"FilterSites",
				"SiteFilterExpression"
			};
			return strs;
		}

		private void LogCompareSubWebException(string subWebName, string sourceSubWebsUrl, string targetSubWebsUrl, Exception ex)
		{
			LogItem logItem = new LogItem("Comparing SubWebs", subWebName, sourceSubWebsUrl, targetSubWebsUrl, ActionOperationStatus.Failed);
			base.FireOperationStarted(logItem);
			logItem.Status = ActionOperationStatus.Failed;
			logItem.Information = string.Concat("Exception thrown : ", ex.Message);
			logItem.Details = ex.StackTrace;
			base.FireOperationFinished(logItem);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPWeb item = (SPWeb)source[0];
			SPWeb sPWeb = (SPWeb)target[0];
			this.CompareWebs(item, sPWeb);
		}
	}
}