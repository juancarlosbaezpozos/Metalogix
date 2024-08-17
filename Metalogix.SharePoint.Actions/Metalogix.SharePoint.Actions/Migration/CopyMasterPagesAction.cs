using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
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
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MandatoryTransformers(new Type[] { typeof(ReferencedMasterPageDataUpdater), typeof(MasterPageDataUpdater) })]
	[Name("Paste Master Pages")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.ZeroOrMore)]
	[SourceType(typeof(SPListItem), true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPListItemCollection))]
	public class CopyMasterPagesAction : PasteAction<PasteMasterPagesOptions>
	{
		private TransformerDefinition<SPListItem, CopyMasterPagesAction, SPListItemCollection, SPListItemCollection> listTransformerDefinition = new TransformerDefinition<SPListItem, CopyMasterPagesAction, SPListItemCollection, SPListItemCollection>("SharePoint Master Page Gallery", false);

		public CopyMasterPagesAction()
		{
		}

		private string ApprovalStatus(string moderationStatusInteger)
		{
			string str = moderationStatusInteger;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "0")
				{
					return Resources.CopyMasterPagesAction_ApprovalStatus_Approved;
				}
				if (str1 == "1")
				{
					return Resources.CopyMasterPagesAction_ApprovalStatus_Rejected;
				}
				if (str1 == "2")
				{
					return Resources.CopyMasterPagesAction_ApprovalStatus_Pending;
				}
				if (str1 == "3")
				{
					return Resources.CopyMasterPagesAction_ApprovalStatus_Draft;
				}
				if (str1 == "4")
				{
					return Resources.CopyMasterPagesAction_ApprovalStatus_Scheduled;
				}
			}
			return string.Empty;
		}

		private void CopyMasterPagePermissionsTaskDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			SPListItem sPListItem = oParams[0] as SPListItem;
			SPListItem sPListItem1 = oParams[1] as SPListItem;
			if (sPListItem.HasUniquePermissions)
			{
				CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
				copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				if (base.CheckForAbort())
				{
					return;
				}
				base.SubActions.Add(copyRoleAssignmentsAction);
				object[] objArray = new object[] { sPListItem, sPListItem1, false };
				copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPListItem.ParentList.ParentWeb, sPListItem1.ParentList.ParentWeb), null);
			}
			base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPListItem1.FileRef), false, false);
		}

		private void CopyMasterPages(IEnumerable<Node> sourceItems, SPListItemCollection targetCollection)
		{
			string copyMasterPagesActionCopyMasterPagesAddingPageLayout;
			SPListItem sPListItem;
			SPListItemCollection parentCollection;
			if (sourceItems.Count<Node>() == 0)
			{
				sPListItem = null;
			}
			else
			{
				sPListItem = sourceItems.First<Node>() as SPListItem;
			}
			SPListItem sPListItem1 = sPListItem;
			if (sPListItem1 == null)
			{
				parentCollection = null;
			}
			else
			{
				parentCollection = sPListItem1.ParentCollection;
			}
			SPListItemCollection sPListItemCollection = parentCollection;
			if (sPListItemCollection == null)
			{
				return;
			}
			this.listTransformerDefinition.BeginTransformation(this, sPListItemCollection, targetCollection, this.ActionOptions.Transformers);
			foreach (SPListItem sourceItem in sourceItems)
			{
				SPListItem sPListItem2 = sourceItem;
				if (!base.CheckForAbort())
				{
					if (base.SharePointOptions.FilterItems && !base.SharePointOptions.ItemFilterExpression.Evaluate(sPListItem2, new CompareDatesInUtc()))
					{
						continue;
					}
					LogItem logItem = null;
					SPListItem sPListItem3 = null;
					try
					{
						base.EnsurePrincipalExistence(sPListItem2.GetReferencedPrincipals(), targetCollection.ParentSPList.ParentWeb);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string name = null;
						string displayUrl = null;
						if (sourceItem.ParentCollection.ParentFolder != null && sourceItem.ParentCollection.ParentFolder.GetType().IsAssignableFrom(typeof(SPFolder)))
						{
							name = ((SPFolder)sourceItem.ParentCollection.ParentFolder).Name;
							displayUrl = ((SPFolder)sourceItem.ParentCollection.ParentFolder).DisplayUrl;
						}
						else if (sourceItem.ParentList != null)
						{
							name = sourceItem.ParentList.Name;
							displayUrl = sourceItem.ParentList.DisplayUrl;
						}
						LogItem logItem1 = new LogItem(string.Concat("Could not fetch referenced principals: ", exception.Message), name, displayUrl, targetCollection.ParentSPList.DisplayUrl, ActionOperationStatus.Failed)
						{
							Exception = exception
						};
						base.FireOperationStarted(logItem1);
					}
					try
					{
						try
						{
							string str = sPListItem2.Name.Substring(sPListItem2.Name.LastIndexOf(".") + 1);
							string str1 = str;
							string str2 = str1;
							if (str1 != null)
							{
								if (str2 == "aspx")
								{
									copyMasterPagesActionCopyMasterPagesAddingPageLayout = Resources.CopyMasterPagesAction_CopyMasterPages_Adding_Page_Layout;
									goto Label1;
								}
								else
								{
									if (str2 != "master")
									{
										goto Label3;
									}
									copyMasterPagesActionCopyMasterPagesAddingPageLayout = Resources.CopyMasterPagesAction_CopyMasterPages_Adding_Master_Page;
									goto Label1;
								}
							}
						Label3:
							copyMasterPagesActionCopyMasterPagesAddingPageLayout = Resources.CopyMasterPagesAction_CopyMasterPages_Adding_Master_Page_Catalog_Items;
						Label1:
							logItem = new LogItem(copyMasterPagesActionCopyMasterPagesAddingPageLayout, sPListItem2.FileLeafRef, sPListItem2.DisplayUrl, targetCollection.ParentSPList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							byte[] binary = sPListItem2.Binary;
							sPListItem2 = this.listTransformerDefinition.Transform(sPListItem2, this, sourceItem.ParentCollection, targetCollection, this.Options.Transformers);
							if (sPListItem2 != null)
							{
								bool flag = binary == sPListItem2.Binary;
								string xML = sPListItem2.XML;
								if (sPListItem2.HasPublishingPageLayout)
								{
									xML = base.LinkCorrector.UpdateLinksInPublishingPage(sPListItem2, targetCollection.ParentFolder as SPFolder, xML);
								}
								if (binary == null || (int)binary.Length <= 0)
								{
									logItem.Status = ActionOperationStatus.Skipped;
									logItem.Information = Resources.CopyMasterPagesAction_CopyMasterPages_Source_page_is_ghosted;
								}
								else
								{
									AddDocumentOptions addDocumentOption = new AddDocumentOptions();
									if (!(sPListItem2 is SPListItemVersion) || sPListItem2.VersionHistory.Count > 1 && targetCollection.ParentSPList.EnableVersioning && base.SharePointOptions.CopyVersions)
									{
										int? nullable = null;
										SPListItem itemByFileName = targetCollection.GetItemByFileName(sPListItem2.FileLeafRef);
										for (int i = 0; i < sPListItem2.VersionHistory.Count; i++)
										{
											int count = sPListItem2.VersionHistory.Count - 1 - i;
											bool flag1 = (!base.SharePointOptions.CopyMaxVersions ? true : count < base.SharePointOptions.MaximumVersionCount);
											if (base.SharePointOptions.CopyVersions && flag1 || i == sPListItem2.VersionHistory.Count - 1)
											{
												SPListItem item = (SPListItem)sPListItem2.VersionHistory[i];
												if (this.HasNewerVersions(item, itemByFileName))
												{
													string str3 = (nullable.HasValue ? this.UpdateItemID(item.XML, nullable.Value) : this.UpdateItemID(item.XML, this.GetIdFromXml(xML)));
													if (sPListItem2.HasPublishingPageLayout)
													{
														str3 = base.LinkCorrector.UpdateLinksInPublishingPage(sPListItem2, targetCollection.ParentFolder as SPFolder, str3);
													}
													item.SetFullXML(XmlUtility.StringToXmlNode(str3));
													addDocumentOption.PreserveID = base.SharePointOptions.PreserveDocumentIDs;
													addDocumentOption.AllowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
													byte[] numArray = item.Binary;
													if (numArray != null)
													{
														if ((int)numArray.Length == 0)
														{
															throw new FileNotFoundException(Resources.CopyMasterPagesAction_CopyMasterPages_Cannot_complete_this_action__File_not_readable);
														}
														item = this.listTransformerDefinition.Transform(item, this, sourceItem.ParentCollection, targetCollection, this.Options.Transformers);
														numArray = item.Binary;
														if (sPListItem2.Name.EndsWith(".master") && base.SharePointOptions.CopyMasterPages)
														{
															if (base.SharePointOptions.CorrectMasterPageLinks)
															{
																numArray = this.MapMasterPageData(sPListItem2, numArray);
															}
															sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, item.XML, addDocumentOption, numArray, AddDocumentMode.Comprehensive);
														}
														else if (sPListItem2.Name.EndsWith(".aspx") && base.SharePointOptions.CopyPageLayouts)
														{
															sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, item.XML, addDocumentOption, numArray, AddDocumentMode.Comprehensive);
														}
														else if (!sPListItem2.Name.EndsWith(".master") && !sPListItem2.Name.EndsWith(".aspx") && base.SharePointOptions.CopyOtherResources)
														{
															sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, item.XML, addDocumentOption, numArray, AddDocumentMode.Comprehensive);
														}
													}
													else
													{
														LogItem logItem2 = new LogItem(logItem.Operation, item.FileLeafRef, sourceItem.ParentList.DisplayUrl, targetCollection.ParentSPList.DisplayUrl, ActionOperationStatus.Skipped)
														{
															Status = ActionOperationStatus.Skipped,
															Information = string.Format(Resources.CopyMasterPagesAction_CopyMasterPages_SkipDraftHiddenVersionAsBinariesAreUnavailable, item.VersionString, item.FileLeafRef)
														};
														LogItem logItem3 = logItem2;
														base.FireOperationStarted(logItem3);
														base.FireOperationFinished(logItem3);
													}
												}
											}
										}
									}
									else
									{
										addDocumentOption.Overwrite = (base.SharePointOptions.PreserveDocumentIDs ? true : flag);
										addDocumentOption.PreserveID = base.SharePointOptions.PreserveDocumentIDs;
										addDocumentOption.AllowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
										if (sPListItem2.FileLeafRef.Contains(".aspx") && base.SharePointOptions.CopyPageLayouts)
										{
											sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, xML, addDocumentOption, sPListItem2.Binary, AddDocumentMode.Comprehensive);
										}
										if (sPListItem2.FileLeafRef.Contains(".master") && base.SharePointOptions.CopyMasterPages)
										{
											if (base.SharePointOptions.CorrectMasterPageLinks)
											{
												sPListItem2.Binary = this.MapMasterPageData(sPListItem2, sPListItem2.Binary);
											}
											sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, xML, addDocumentOption, sPListItem2.Binary, AddDocumentMode.Comprehensive);
										}
										if (base.SharePointOptions.CopyOtherResources && sPListItem3 == null)
										{
											sPListItem3 = targetCollection.AddDocument(sPListItem2.ParentRelativePath, xML, addDocumentOption, sPListItem2.Binary, AddDocumentMode.Comprehensive);
										}
									}
									if (sPListItem3 != null)
									{
										Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
										string str4 = string.Concat(typeof(CopyRoleAssignmentsAction).Name, sPListItem3.FileDirRef);
										object[] objArray = new object[] { sPListItem2, sPListItem3 };
										threadManager.QueueBufferedTask(str4, objArray, new ThreadedOperationDelegate(this.CopyMasterPagePermissionsTaskDelegate));
									}
									if (!base.SharePointOptions.CheckResults || !(sPListItem3 != null))
									{
										logItem.Status = (sPListItem3 != null ? ActionOperationStatus.Completed : ActionOperationStatus.Skipped);
									}
									else
									{
										base.CompareNodes(sPListItem2, sPListItem3, logItem);
									}
								}
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = sPListItem2.XML;
									if (sPListItem3 != null)
									{
										logItem.TargetContent = sPListItem3.XML;
									}
								}
							}
							else
							{
								logItem.Status = ActionOperationStatus.Skipped;
								continue;
							}
						}
						catch (FileNotFoundException fileNotFoundException1)
						{
							FileNotFoundException fileNotFoundException = fileNotFoundException1;
							logItem.Exception = fileNotFoundException;
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = (!string.IsNullOrEmpty(sPListItem2["_ModerationStatus"]) ? string.Format(Resources.CopyMasterPagesAction_CopyMasterPages_Exception_thrown____0____the_source_page_s_moderation_status_was_set_as___1____so_could_not_be_read_, fileNotFoundException.Message, this.ApprovalStatus(sPListItem2["_ModerationStatus"])) : string.Format(Resources.CopyMasterPagesAction_CopyMasterPages_Exception_thrown____0____please_verify_that_the_source_page_s_moderation_status, fileNotFoundException.Message));
							logItem.Details = fileNotFoundException.StackTrace;
							logItem.SourceContent = sPListItem2.XML;
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							logItem.Exception = exception2;
							logItem.Status = (exception2.Message.Contains("This file is currently associated to an HTML file") ? ActionOperationStatus.Skipped : ActionOperationStatus.Failed);
							logItem.Information = string.Concat("Exception thrown: ", exception2.Message);
							logItem.Details = exception2.StackTrace;
							logItem.SourceContent = sPListItem2.XML;
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
				else
				{
					return;
				}
			}
			this.listTransformerDefinition.EndTransformation(this, sPListItemCollection, targetCollection, this.Options.Transformers);
		}

		private int GetIdFromXml(string itemXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(itemXml);
			XmlNode firstChild = xmlDocument.FirstChild;
			return Convert.ToInt32(firstChild.Attributes["ID"].Value);
		}

		private bool HasNewerVersions(SPListItem sourceItem, SPListItem targetItem)
		{
			bool flag;
			if (sourceItem == null)
			{
				return false;
			}
			if (targetItem == null)
			{
				return true;
			}
			bool flag1 = false;
			bool isDocumentLibrary = sourceItem.ParentList.IsDocumentLibrary;
			string versionString = sourceItem.VersionString;
			string empty = string.Empty;
			try
			{
				empty = targetItem.VersionString;
				if (!string.IsNullOrEmpty(versionString) && !string.IsNullOrEmpty(empty))
				{
					Version version = new Version(versionString);
					Version version1 = new Version(empty);
					flag1 = ((!sourceItem.ParentList.IsDocumentLibrary ? false : targetItem.IsCheckedOut) ? version >= version1 : version > version1);
				}
				return flag1;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!targetItem.ParentList.Adapter.IsClientOM || !targetItem.FileLeafRef.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || !exception.Message.StartsWith("An error occurred during the operation of a service method: Item does not exist.", StringComparison.InvariantCultureIgnoreCase))
				{
					throw;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public byte[] MapMasterPageData(SPListItem sourceItem, byte[] pageBinary)
		{
			string str = (new UTF8Encoding()).GetString(pageBinary);
			string str1 = sourceItem.ParentList.ParentWeb.ServerRelativeUrl.TrimStart(new char[] { '/' });
			if (!string.IsNullOrEmpty(str1))
			{
				string lower = str1.ToLower();
				StringBuilder stringBuilder = new StringBuilder(str);
				if (base.LinkCorrector.SiteCollectionMappings.ContainsKey(lower))
				{
					string str2 = base.LinkCorrector.SiteCollectionMappings[lower].ToString();
					str = stringBuilder.Replace(str1, str2).ToString();
				}
			}
			return Encoding.UTF8.GetBytes(str);
		}

		public override void Run(IXMLAbleList source, IXMLAbleList target)
		{
			IEnumerable<Node> nodes = source.Cast<Node>();
			this.CopyMasterPages(nodes, target[0] as SPListItemCollection);
		}

		protected override void RunOperation(object[] oParams)
		{
			IEnumerable<Node> nodes = oParams[0] as IEnumerable<Node>;
			this.CopyMasterPages(nodes, oParams[1] as SPListItemCollection);
		}

		private string UpdateItemID(string sOriginalItemXml, int iNewItemId)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sOriginalItemXml);
			XmlNode firstChild = xmlDocument.FirstChild;
			XmlNode str = xmlDocument.CreateNode(XmlNodeType.Attribute, "ID", null);
			str.Value = iNewItemId.ToString();
			firstChild.Attributes.SetNamedItem(str);
			return firstChild.OuterXml;
		}
	}
}