using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Database.StatusProviders;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Database;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Database;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Database
{
	[Batchable(false, "Restore in batch...")]
	[Image("Metalogix.SharePoint.Actions.Icons.Database.RestoreFromDatabase.ico")]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.SRM)]
	[MenuText("Restore...{0-Restore}")]
	[Name("Restore")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[StatusLabelProvider(typeof(RestorationStatusLabelProvider))]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPNode))]
	[UsesStickySettings(false)]
	public class RestoreAction : Metalogix.Actions.Action<RestoreOptions>
	{
		public override string LicensingDescriptor
		{
			get
			{
				return Resources.Bytes_Restored;
			}
		}

		public override string LicensingUnit
		{
			get
			{
				return "GB";
			}
		}

		protected override IThreadingStrategy ThreadingStrategy
		{
			get
			{
				return StaticThreadingStrategy.Instance;
			}
		}

		public RestoreAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			bool flag1 = base.AppliesTo(sourceSelections, targetSelections);
			if (base.GetType().IsSubclassOf(typeof(RestoreAction)))
			{
				return flag1;
			}
			if (flag1)
			{
				IEnumerator enumerator = ((this.ActionOptions.Configured ? sourceSelections : targetSelections)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SPNode current = (SPNode)enumerator.Current;
						if (current.Adapter.GetType().Name != "DBAdapter")
						{
							flag = false;
							return flag;
						}
						else if (current.Parent != null)
						{
							SPWeb parentWeb = null;
							if (typeof(SPWeb).IsAssignableFrom(current.GetType()))
							{
								if (LicensingUtils.GetEdition(base.GetValidatedCommonLicense()) == Edition.StoragePoint)
								{
									parentWeb = (SPWeb)current;
								}
							}
							else if (typeof(SPFolder).IsAssignableFrom(current.GetType()))
							{
								parentWeb = ((SPFolder)current).ParentList.ParentWeb;
							}
							else if (typeof(SPListItem).IsAssignableFrom(current.GetType()))
							{
								parentWeb = ((SPListItem)current).ParentList.ParentWeb;
							}
							if (parentWeb != null)
							{
								parentWeb.RootWeb.Adapter.ServerDisplayName.ToUpper();
							}
							else
							{
								flag = false;
								return flag;
							}
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					return flag1;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return flag;
			}
			return flag1;
		}

		public static Node MapByDatabase(Node sourceNode, out Node matchedNode, out string sWebApplicationUrl)
		{
			string str;
			Node node;
			Node parent;
			SPServer sPServer = null;
			Node nodeByPath = null;
			SPWeb parentWeb = null;
			matchedNode = null;
			char[] chrArray = new char[] { '/' };
			sWebApplicationUrl = null;
			try
			{
				if (typeof(SPWeb).IsAssignableFrom(sourceNode.GetType()))
				{
					parentWeb = (SPWeb)sourceNode;
				}
				else if (typeof(SPFolder).IsAssignableFrom(sourceNode.GetType()))
				{
					parentWeb = ((SPFolder)sourceNode).ParentList.ParentWeb;
				}
				else if (typeof(SPListItem).IsAssignableFrom(sourceNode.GetType()))
				{
					parentWeb = ((SPListItem)sourceNode).ParentList.ParentWeb;
				}
				if (parentWeb == null)
				{
					throw new ArgumentException("Source node is not a known SharePoint node");
				}
				string str1 = string.Concat(parentWeb.DatabaseServer.ToUpper(), ".", parentWeb.DatabaseName.ToUpper());
				if (DatabaseSettings.OpenedBackups.ContainsKey(str1))
				{
					string str2 = DatabaseSettings.OpenedBackups[str1].ToString();
					if (!(sourceNode is SPSite))
					{
						sourceNode = sourceNode.Parent;
						List<SPWeb> sPWebs = new List<SPWeb>();
						foreach (SPConnection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
						{
							try
							{
								if (activeConnection.Status == ConnectionStatus.Valid && activeConnection.Adapter.Writer != null)
								{
									SPWeb sPWeb = activeConnection as SPWeb;
									if (sPWeb != null && sourceNode.ServerRelativeUrl.Trim(chrArray).StartsWith(sPWeb.ServerRelativeUrl.Trim(chrArray)) && string.Concat(sPWeb.DatabaseServer.ToUpper(), ".", sPWeb.DatabaseName.ToUpper()) == str2)
									{
										sWebApplicationUrl = sPWeb.Adapter.Server;
										if (sourceNode.ServerRelativeUrl.Trim(chrArray) != sPWeb.ServerRelativeUrl.Trim(chrArray))
										{
											sPWebs.Add(sPWeb);
										}
										else
										{
											matchedNode = sourceNode;
											node = sPWeb;
											return node;
										}
									}
									if (activeConnection is SPServer)
									{
										SPServer sPServer1 = (SPServer)activeConnection;
										bool flag = false;
										sWebApplicationUrl = sPServer1.Adapter.Server;
										foreach (SPWebApplication webApplication in sPServer1.WebApplications)
										{
											if (webApplication.ContentDatabases.Find((SPContentDatabase contentDB) => string.Equals(contentDB.DatabaseString, str2, StringComparison.InvariantCultureIgnoreCase)) == null)
											{
												continue;
											}
											flag = true;
											sWebApplicationUrl = webApplication.Url;
											break;
										}
										if (flag)
										{
											foreach (SPSite site in sPServer1.Sites)
											{
												try
												{
													if (site.Url.StartsWith(sWebApplicationUrl) && sourceNode.ServerRelativeUrl.Trim(chrArray).StartsWith(site.ServerRelativeUrl.Trim(chrArray)))
													{
														sPServer = sPServer1;
														if (site.ServerRelativeUrl.Trim(chrArray) != sourceNode.ServerRelativeUrl.Trim(chrArray))
														{
															sPWebs.Add(site);
														}
														else
														{
															matchedNode = sourceNode;
															node = site;
															return node;
														}
													}
												}
												catch (Exception exception)
												{
												}
											}
										}
									}
								}
							}
							catch (Exception exception1)
							{
							}
						}
						List<SPWeb>.Enumerator enumerator = sPWebs.GetEnumerator();
						try
						{
							do
							{
								if (!enumerator.MoveNext())
								{
									break;
								}
								SPWeb current = enumerator.Current;
								Node node1 = sourceNode;
								while (nodeByPath == null && node1 != null)
								{
									matchedNode = node1;
									if (node1.ServerRelativeUrl.Trim(chrArray) != current.ServerRelativeUrl.Trim(chrArray))
									{
										str = (!node1.Url.StartsWith(string.Concat(((SPNode)node1).Adapter.Server, "/")) ? node1.ServerRelativeUrl.Trim(chrArray).Substring(current.ServerRelativeUrl.Trim(chrArray).Length) : node1.Url.Substring(((SPNode)node1).Adapter.Server.Length).Trim(chrArray).Substring(current.ServerRelativeUrl.Trim(chrArray).Length));
										nodeByPath = current.GetNodeByPath(str);
										if (node1.Parent == null || node1.Parent is SPServer)
										{
											parent = null;
										}
										else
										{
											parent = node1.Parent;
										}
										node1 = parent;
									}
									else
									{
										nodeByPath = current;
									}
								}
							}
							while (nodeByPath == null);
						}
						finally
						{
							((IDisposable)enumerator).Dispose();
						}
						if (nodeByPath == null && sPServer != null && parentWeb.RootWeb is SPSite)
						{
							matchedNode = sourceNode;
							nodeByPath = sPServer;
						}
					}
					else
					{
						foreach (SPConnection sPConnection in Metalogix.Explorer.Settings.ActiveConnections)
						{
							if (sPConnection.Status != ConnectionStatus.Valid || sPConnection.Adapter.Writer == null || !(sPConnection is SPServer))
							{
								continue;
							}
							try
							{
								SPServer sPServer2 = (SPServer)sPConnection;
								bool flag1 = false;
								sWebApplicationUrl = sPServer2.Adapter.Server;
								foreach (SPWebApplication sPWebApplication in sPServer2.WebApplications)
								{
									if (sPWebApplication.ContentDatabases.Find((SPContentDatabase contentDB) => string.Equals(contentDB.DatabaseString, str2, StringComparison.InvariantCultureIgnoreCase)) == null)
									{
										continue;
									}
									flag1 = true;
									sWebApplicationUrl = sPWebApplication.Url;
									break;
								}
								if (flag1)
								{
									matchedNode = sourceNode.Parent;
									node = sPServer2;
									return node;
								}
							}
							catch (Exception exception2)
							{
							}
						}
					}
				}
				if (matchedNode == null)
				{
					matchedNode = sourceNode;
				}
				return nodeByPath;
			}
			catch (Exception exception3)
			{
				if (matchedNode == null)
				{
					matchedNode = sourceNode;
				}
				return nodeByPath;
			}
			return node;
		}

		private void RestoreWorkflows(object[] oParams)
		{
			PasteListAction pasteListAction = oParams[0] as PasteListAction;
			SPList sPList = oParams[1] as SPList;
			SPWeb sPWeb = oParams[2] as SPWeb;
			try
			{
				if (this.ActionOptions.CopyListOOBWorkflowAssociations || this.ActionOptions.CopyListSharePointDesignerNintexWorkflowAssociations)
				{
					CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
					copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(this.ActionOptions);
					SPListCollection lists = sPWeb.Lists;
					Guid item = pasteListAction.GuidMappings[new Guid(sPList.ID)];
					SPList listByGuid = lists.GetListByGuid(item.ToString());
					pasteListAction.SubActions.Add(copyWorkflowAssociationsAction);
					object[] objArray = new object[] { sPList, listByGuid, sPWeb };
					copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPList.ParentWeb, sPWeb), null);
				}
			}
			finally
			{
				sPList.Dispose();
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Node node;
			LogItem logItem;
			List<Node> nodes = new List<Node>();
			Node item = (Node)source[0];
			Node item1 = (Node)target[0];
			/*if (item is SPListItem)
			{
				MLLicense license = (MLLicense)ActionLicenseProvider.Instance.GetLicense(base.GetType(), this);
				if ((license != null && license.LicenseType == MLLicenseType.Evaluation))
				{
					throw new Exception("Item-level Restore is not enabled in evaluation mode.");
				}
			}*/
			node = (!this.ActionOptions.IncludePath ? this.ActionOptions.LegalMatchedLocation.GetNode() : this.ActionOptions.AlgorithmMatchedLocation.GetNode());
			Node parent = item.Parent;
			while (parent.DisplayUrl != node.DisplayUrl)
			{
				nodes.Insert(0, parent);
				parent = parent.Parent;
				if (parent != null)
				{
					continue;
				}
				throw new ArgumentException("The target is not a descendant of the match point.");
			}
			foreach (Node node1 in nodes)
			{
				if (typeof(SPSite).IsAssignableFrom(node1.GetType()) && item1 is SPServer)
				{
					logItem = new LogItem("Create Restoration Site Collection", node1.Name, node1.DisplayUrl, item1.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					try
					{
						SPServer sPServer = (SPServer)item1;
						string name = null;
						foreach (SPWebApplication webApplication in sPServer.WebApplications)
						{
							if (webApplication.Url != this.ActionOptions.WebApplicationUrl)
							{
								continue;
							}
							name = webApplication.Name;
							break;
						}
						AddSiteCollectionOptions addSiteCollectionOption = new AddSiteCollectionOptions()
						{
							ValidateOwners = true
						};
						item1 = sPServer.Sites.AddSiteCollection(name, node1.XML, addSiteCollectionOption);
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.Exception = exception;
						logItem.Details = string.Concat(exception.Message, "\n", exception.StackTrace);
						logItem.Status = ActionOperationStatus.Failed;
					}
					base.FireOperationFinished(logItem);
				}
				else if (typeof(SPWeb).IsAssignableFrom(node1.GetType()))
				{
					logItem = new LogItem("Create Restoration Site", node1.Name, node1.DisplayUrl, item1.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					try
					{
						item1 = ((SPWeb)item1).SubWebs.AddWeb(node1.XML, new AddWebOptions(), null);
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						logItem.Exception = exception2;
						logItem.Details = string.Concat(exception2.Message, "\n", exception2.StackTrace);
						logItem.Status = ActionOperationStatus.Failed;
					}
					base.FireOperationFinished(logItem);
				}
				else if (!typeof(SPList).IsAssignableFrom(node1.GetType()))
				{
					if (typeof(SPFolder) != node1.GetType())
					{
						continue;
					}
					if (((SPFolder)node1).ParentList is SPDiscussionList)
					{
						break;
					}
					logItem = new LogItem("Create Restoration Folder", node1.Name, node1.DisplayUrl, item1.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					item1 = ((SPFolder)item1).SubFolders.AddFolder(node1.XML, new AddFolderOptions(), AddFolderMode.Comprehensive);
					try
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception5)
					{
						Exception exception4 = exception5;
						logItem.Exception = exception4;
						logItem.Details = string.Concat(exception4.Message, "\n", exception4.StackTrace);
						logItem.Status = ActionOperationStatus.Failed;
					}
					base.FireOperationFinished(logItem);
				}
				else
				{
					logItem = new LogItem("Create Restoration List", node1.Name, node1.DisplayUrl, item1.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					item1 = ((SPWeb)item1).Lists.AddList(node1.XML, new AddListOptions());
					try
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception7)
					{
						Exception exception6 = exception7;
						logItem.Exception = exception6;
						logItem.Details = string.Concat(exception6.Message, "\n", exception6.StackTrace);
						logItem.Status = ActionOperationStatus.Failed;
					}
					base.FireOperationFinished(logItem);
				}
			}
			this.ActionOptions.ShallowCopyExternalizedData = false;
			this.ActionOptions.OverwriteSites = true;
			this.ActionOptions.OverwriteLists = true;
			this.ActionOptions.OverwriteFolders = true;
			this.ActionOptions.CopyWebPartsAtItemsLevel = true;
			this.ActionOptions.CopySiteQuota = true;
			this.ActionOptions.CopySiteAdmins = true;
			this.ActionOptions.CopySiteColumns = true;
			this.ActionOptions.ApplyThemeToWeb = true;
			this.ActionOptions.CopyContentTypes = true;
			this.ActionOptions.CopyVersions = true;
			this.ActionOptions.CopySitePermissions = true;
			this.ActionOptions.CopyRootPermissions = false;
			this.ActionOptions.MapRolesByName = true;
			this.ActionOptions.MapGroupsByName = true;
			this.ActionOptions.ClearRoleAssignments = true;
			this.ActionOptions.PreserveItemIDs = true;
			this.ActionOptions.PreserveDocumentIDs = true;
			this.ActionOptions.CopySitePermissions = true;
			this.ActionOptions.CopyListPermissions = true;
			this.ActionOptions.CopyFolderPermissions = true;
			this.ActionOptions.CopyItemPermissions = true;
			this.ActionOptions.RunNavigationStructureCopy = true;
			this.ActionOptions.CopyNavigation = true;
			this.ActionOptions.CopyCurrentNavigation = true;
			this.ActionOptions.CopyGlobalNavigation = true;
			this.ActionOptions.CopyWorkflowInstanceData = true;
			this.ActionOptions.CopyInProgressWorkflows = false;
			this.ActionOptions.CopyListOOBWorkflowAssociations = true;
			this.ActionOptions.CopyListSharePointDesignerNintexWorkflowAssociations = true;
			this.ActionOptions.CopyContentTypeOOBWorkflowAssociations = true;
			this.ActionOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = true;
			this.ActionOptions.CopyWebOOBWorkflowAssociations = true;
			this.ActionOptions.CopyWebSharePointDesignerNintexWorkflowAssociations = true;
			this.ActionOptions.CopyNintexDatabaseEntries = true;
			Metalogix.Actions.Action pasteSiteCollectionAction = null;
			Edition edition = LicensingUtils.GetEdition(base.GetValidatedCommonLicense());
			if (typeof(SPSite) == item.GetType() && edition == Edition.StoragePoint)
			{
				SPServer sPServer1 = (SPServer)item1;
				SPWeb sPWeb = (SPWeb)item;
				string str = null;
				foreach (SPWebApplication sPWebApplication in sPServer1.WebApplications)
				{
					if (sPWebApplication.Url != this.ActionOptions.WebApplicationUrl)
					{
						continue;
					}
					str = sPWebApplication.Name;
					break;
				}
				pasteSiteCollectionAction = new PasteSiteCollectionAction();
				this.ActionOptions.URL = item.ServerRelativeUrl;
				this.ActionOptions.WebApplicationName = str;
				SPLanguage sPLanguage = sPServer1.Languages[sPWeb.Language.ToString()];
				if (sPLanguage == null && sPServer1.Languages.Count > 0)
				{
					sPLanguage = sPServer1.Languages[0];
				}
				if (sPLanguage != null)
				{
					this.ActionOptions.LanguageCode = sPLanguage.LCID.ToString();
				}
			}
			else if (typeof(SPWeb) == item.GetType() && edition == Edition.StoragePoint)
			{
				pasteSiteCollectionAction = new PasteSiteAction();
			}
			else if (typeof(SPList).IsAssignableFrom(item.GetType()))
			{
				pasteSiteCollectionAction = new PasteListAction();
			}
			else if (typeof(SPFolder).IsAssignableFrom(item.GetType()))
			{
				if (!(((SPFolder)item).ParentList is SPDiscussionList))
				{
					pasteSiteCollectionAction = new PasteFolderAction();
				}
				else
				{
					foreach (SPDiscussionItem discussionItem in ((SPDiscussionList)((SPFolder)item).ParentList).DiscussionItems)
					{
						pasteSiteCollectionAction = new PasteListItemAction();
						if (string.Concat(discussionItem.FileDirRef, "/", discussionItem.FileLeafRef) != item.ServerRelativeUrl)
						{
							continue;
						}
						SPList parentList = ((SPFolder)item).ParentList;
						SPFolder sPFolder = (SPFolder)item;
						SPListItem[] sPListItemArray = new SPListItem[] { discussionItem };
						source = new SPListItemCollection(parentList, sPFolder, sPListItemArray);
					}
				}
			}
			else if (typeof(SPListItem).IsAssignableFrom(item.GetType()))
			{
				if (!(((SPListItem)item).ParentList is SPDiscussionList) || ((SPListItem)item).ParentFolder == ((SPListItem)item).ParentList)
				{
					pasteSiteCollectionAction = new PasteListItemAction();
				}
				else
				{
					pasteSiteCollectionAction = new PasteAllListItemsAction();
					PasteListItemOptions sharePointOptions = ((PasteAllListItemsAction)pasteSiteCollectionAction).SharePointOptions;
					sharePointOptions.SetFromOptions(this.Options);
					sharePointOptions.FilterItems = true;
					FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.Or);
					SPDiscussionItem[] thread = ((SPDiscussionList)((SPListItem)item).ParentList).GetThread(((SPListItem)item).ID);
					SPDiscussionItem[] sPDiscussionItemArray = thread;
					for (int i = 0; i < (int)sPDiscussionItemArray.Length; i++)
					{
						SPDiscussionItem sPDiscussionItem = sPDiscussionItemArray[i];
						filterExpressionList.Add(new FilterExpression(FilterOperand.Equals, typeof(SPListItem), "Ordering", sPDiscussionItem.Ordering, false, false));
					}
					filterExpressionList.Add(new FilterExpression(FilterOperand.StartsWith, typeof(SPListItem), "Ordering", thread[(int)thread.Length - 1].Ordering, false, false));
					this.ActionOptions.ItemFilterExpression = filterExpressionList;
					this.ActionOptions.FilterItems = true;
					SPNode[] sPNodeArray = new SPNode[] { ((SPListItem)item).ParentList };
					source = new NodeCollection(sPNodeArray);
				}
			}
			if (pasteSiteCollectionAction != null)
			{
				pasteSiteCollectionAction.Options.SetFromOptions(this.ActionOptions);
				base.SubActions.Add(pasteSiteCollectionAction);
				foreach (Node node2 in source)
				{
					if (pasteSiteCollectionAction.GetType() == typeof(PasteListItemAction))
					{
						SPListItem sPListItem = node2 as SPListItem;
						if (sPListItem == null)
						{
							continue;
						}
						SPList sPList = sPListItem.ParentList;
						SPFolder parentFolder = sPListItem.ParentFolder;
						Node[] nodeArray = new Node[] { sPListItem };
						SPListItemCollection sPListItemCollection = new SPListItemCollection(sPList, parentFolder, nodeArray);
						object[] objArray = new object[] { sPListItemCollection, item1 as SPFolder, null, true };
						pasteSiteCollectionAction.RunAsSubAction(objArray, new ActionContext(sPListItem.ParentList.ParentWeb, ((SPFolder)item1).ParentList.ParentWeb), null);
					}
					else if (pasteSiteCollectionAction.GetType() == typeof(PasteSiteCollectionAction))
					{
						SPWeb sPWeb1 = node2 as SPWeb;
						if (sPWeb1 == null)
						{
							continue;
						}
						((PasteSiteCollectionAction)pasteSiteCollectionAction).InitializeWorkflow();
						((PasteSiteCollectionAction)pasteSiteCollectionAction).CopySiteCollection(sPWeb1, item1 as SPServer);
						((PasteSiteCollectionAction)pasteSiteCollectionAction).CopySiteNavigationStructure(item1 as SPServer);
						((PasteSiteCollectionAction)pasteSiteCollectionAction).StartCommonWorkflowBufferedTasks();
					}
					else if (pasteSiteCollectionAction.GetType() == typeof(PasteSiteAction))
					{
						SPWeb sPWeb2 = node2 as SPWeb;
						if (sPWeb2 == null)
						{
							continue;
						}
						((PasteSiteAction)pasteSiteCollectionAction).InitializeWorkflow();
						TaskDefinition taskDefinition = ((PasteSiteAction)pasteSiteCollectionAction).CopySite(sPWeb2, item1 as SPWeb, true);
						base.ThreadManager.WaitForTask(taskDefinition);
						((PasteSiteAction)pasteSiteCollectionAction).CopySiteNavigationStructure(item1 as SPWeb);
						((PasteSiteAction)pasteSiteCollectionAction).StartCommonWorkflowBufferedTasks();
					}
					else if (pasteSiteCollectionAction.GetType() != typeof(PasteListAction))
					{
						if (pasteSiteCollectionAction.GetType() != typeof(PasteFolderAction))
						{
							continue;
						}
						SPFolder sPFolder1 = node2 as SPFolder;
						if (sPFolder1 == null)
						{
							continue;
						}
						object[] objArray1 = new object[] { sPFolder1, item1 as SPFolder, true };
						pasteSiteCollectionAction.RunAsSubAction(objArray1, new ActionContext(sPFolder1.ParentList.ParentWeb, ((SPFolder)item1).ParentList.ParentWeb), null);
					}
					else
					{
						SPList sPList1 = node2 as SPList;
						if (sPList1 == null)
						{
							continue;
						}
						string str1 = string.Concat("MultiselectListContent", sPList1.ID);
						PasteListAction pasteListAction = (PasteListAction)pasteSiteCollectionAction;
						SPWeb sPWeb3 = item1 as SPWeb;
						TaskDefinition taskDefinition1 = pasteListAction.CopyList(sPList1, sPWeb3, true, false, false);
						Metalogix.Threading.ThreadManager threadManager = pasteListAction.ThreadManager;
						object[] objArray2 = new object[] { pasteListAction, sPList1, sPWeb3 };
						threadManager.QueueBufferedTask(str1, objArray2, new ThreadedOperationDelegate(this.RestoreWorkflows));
						pasteListAction.ThreadManager.DelayedSetBufferedTasks(taskDefinition1, str1, false, false);
						pasteListAction.ThreadManager.WaitForTask(taskDefinition1);
						pasteListAction.StartCommonWorkflowBufferedTasks();
					}
				}
			}
		}
	}
}