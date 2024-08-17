using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointAllChildLists", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.ChildLists.ico")]
	[MenuText("2:Paste Site Content {0-Paste} > All Child Lists...")]
	[Name("Paste all child lists")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SubActionTypes(new Type[] { typeof(CopyContentTypesAction), typeof(CopyWorkflowAssociationsAction), typeof(CopySiteColumnsAction), typeof(PasteListAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteSiteLists : PasteAction<PasteSiteListsOptions>
	{
		public PasteSiteLists()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			foreach (SPConnection targetSelection in targetSelections)
			{
				if (targetSelection == null || targetSelection.Status != ConnectionStatus.Invalid)
				{
					continue;
				}
				flag = false;
				return flag;
			}
			SPWeb item = sourceSelections[0] as SPWeb;
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPWeb current = (SPWeb)enumerator.Current;
					if (item.DisplayUrl != current.DisplayUrl)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
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

		private void CopyGloballyReusableWorkflowTemplateDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			CopyGloballyReusableWFItemsAction copyGloballyReusableWFItemsAction = new CopyGloballyReusableWFItemsAction();
			copyGloballyReusableWFItemsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(copyGloballyReusableWFItemsAction);
			SPListCollection sPListCollection = oParams[0] as SPListCollection;
			SPWeb sPWeb = oParams[1] as SPWeb;
			SPList sPList = oParams[2] as SPList;
			try
			{
				if (sPList != null)
				{
					if (!base.CheckForAbort())
					{
						object[] objArray = new object[] { sPList, sPWeb };
						copyGloballyReusableWFItemsAction.RunAsSubAction(objArray, new ActionContext(sPList, sPWeb), null);
					}
					else
					{
						return;
					}
				}
			}
			finally
			{
				if (sPList != null)
				{
					sPList.Dispose();
				}
				sPListCollection.ParentWeb.Dispose();
				sPWeb.Dispose();
			}
		}

		private void CopyLists(SPListCollection sourceListCollection, SPWeb targetWeb, bool bIsCopyRoot, bool bCopySiteColumns, bool bCopyContentTypes)
		{
			SPFieldCollection sPFieldCollections;
			PasteListAction pasteListAction = new PasteListAction();
			pasteListAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteListAction);
			using (targetWeb)
			{
				pasteListAction.ListTransformerDefinition.BeginTransformation(pasteListAction, sourceListCollection, targetWeb.Lists, pasteListAction.Options.Transformers);
				SPFieldCollection sPFieldCollections1 = null;
				try
				{
					if (bCopySiteColumns)
					{
						sPFieldCollections = (bIsCopyRoot ? sourceListCollection.ParentWeb.GetAvailableColumns(false) : sourceListCollection.ParentWeb.GetSiteColumns(false));
					}
					else
					{
						sPFieldCollections = null;
					}
					sPFieldCollections1 = sPFieldCollections;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem = new LogItem("Copying Site Columns", sourceListCollection.ParentWeb.Name, sourceListCollection.ParentWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					logItem.Exception = exception;
					base.FireOperationFinished(logItem);
				}
				IList<SPList> dependencies = this.GetDependencies(targetWeb, sourceListCollection, sPFieldCollections1);
				foreach (SPList dependency in dependencies)
				{
					if (!base.CheckForAbort())
					{
						pasteListAction.CopyList(dependency, targetWeb, false, false, true);
					}
					else
					{
						return;
					}
				}
				if (!base.CheckForAbort())
				{
					if (bCopySiteColumns && sPFieldCollections1 != null)
					{
						SPFieldCollection sPFieldCollections2 = (bIsCopyRoot ? targetWeb.AvailableColumns : targetWeb.SiteColumns);
						CopySiteColumnsAction copySiteColumnsAction = new CopySiteColumnsAction();
						copySiteColumnsAction.Options.SetFromOptions(this.Options);
						this.ConnectSubaction(copySiteColumnsAction);
						object[] siteFieldsFilterExpression = new object[] { sPFieldCollections1, base.SharePointOptions.SiteFieldsFilterExpression, sPFieldCollections2, base.SharePointOptions.TermstoreNameMappingTable, false };
						copySiteColumnsAction.RunAsSubAction(siteFieldsFilterExpression, new ActionContext(sourceListCollection.ParentWeb, targetWeb), null);
					}
					if (!base.CheckForAbort())
					{
						if (bCopyContentTypes)
						{
							CopyContentTypesAction copyContentTypesAction = new CopyContentTypesAction();
							copyContentTypesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
							base.SubActions.Add(copyContentTypesAction);
							object[] contentTypes = new object[] { sourceListCollection.ParentWeb.ContentTypes, targetWeb.ContentTypes };
							copyContentTypesAction.RunAsSubAction(contentTypes, new ActionContext(sourceListCollection.ParentWeb, targetWeb), null);
							if (base.CheckForAbort())
							{
								return;
							}
						}
						base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyMasterPageGalleryAction).Name, targetWeb.ID), false, true);
						base.SharePointOptions.CopyWebPartsAtItemsLevel = false;
						base.WebPartPagesNotCopiedAtItemsLevel.Clear();
						bool parserEnabled = true;
						SPList sPList = null;
						SPList sPList1 = null;
						SPList sPList2 = null;
						SPList sPList3 = null;
						try
						{
							if (base.SharePointOptions.DisableDocumentParsing)
							{
								parserEnabled = targetWeb.ParserEnabled;
								if (parserEnabled)
								{
									targetWeb.SetDocumentParsing(false);
								}
							}
							List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
							foreach (SPList sPList4 in sourceListCollection)
							{
								if (base.CheckForAbort())
								{
									return;
								}
								else if (sPList4.Name == "Pages" && sPList4.BaseTemplate == ListTemplateType.O12Pages)
								{
									sPList = sPList4;
								}
								else if (sPList4.Name == "Workflows" && sPList4.BaseTemplate == ListTemplateType.NoCodeWorkflows)
								{
									sPList1 = sPList4;
								}
								else if (sPList4.Name == "NintexWorkflows" && sPList4.BaseTemplate == ListTemplateType.NintexWorkflows)
								{
									sPList2 = sPList4;
								}
								else if (!sPList4.Name.Equals("wfpub", StringComparison.InvariantCultureIgnoreCase) || sPList4.BaseTemplate != ListTemplateType.NoCodePublicWorkflow)
								{
									if (sPList4.Name.Equals("wfsvc", StringComparison.OrdinalIgnoreCase) && sPList4.BaseTemplate == ListTemplateType.WorkflowService)
									{
										continue;
									}
									if (!dependencies.Contains(sPList4))
									{
										TaskDefinition taskDefinition = pasteListAction.CopyList(sPList4, targetWeb, false, false, false);
										if (taskDefinition == null)
										{
											continue;
										}
										taskDefinitions.Add(taskDefinition);
									}
									else
									{
										pasteListAction.SharePointOptions.OverwriteLists = false;
										pasteListAction.SharePointOptions.UpdateLists = true;
										PasteListOptions sharePointOptions = pasteListAction.SharePointOptions;
										sharePointOptions.UpdateListOptionsBitField = sharePointOptions.UpdateListOptionsBitField | 23;
										pasteListAction.SharePointOptions.CheckModifiedDatesForLists = false;
										pasteListAction.SharePointOptions.PreserveItemIDs = true;
										TaskDefinition taskDefinition1 = pasteListAction.CopyList(sPList4, targetWeb, false, false, false);
										if (taskDefinition1 != null)
										{
											taskDefinitions.Add(taskDefinition1);
										}
										pasteListAction.SharePointOptions.OverwriteLists = base.SharePointOptions.OverwriteLists;
										pasteListAction.SharePointOptions.UpdateLists = base.SharePointOptions.UpdateLists;
										pasteListAction.SharePointOptions.UpdateListOptionsBitField = base.SharePointOptions.UpdateListOptionsBitField;
										pasteListAction.SharePointOptions.CheckModifiedDatesForLists = base.SharePointOptions.CheckModifiedDatesForLists;
									}
								}
								else
								{
									sPList3 = sPList4;
								}
							}
							if (sPList != null)
							{
								if (!base.CheckForAbort())
								{
									taskDefinitions.Add(pasteListAction.CopyList(sPList, targetWeb, false, false, false));
								}
								else
								{
									return;
								}
							}
							if (!bIsCopyRoot)
							{
								sourceListCollection.ParentWeb.Collapse();
								targetWeb.Collapse();
							}
							base.ThreadManager.WaitForTasks(taskDefinitions);
						}
						finally
						{
							if (base.SharePointOptions.DisableDocumentParsing && parserEnabled)
							{
								targetWeb.SetDocumentParsing(true);
							}
							base.SetAllListItemCopyCompletedBufferKeysForWeb(targetWeb);
						}
						pasteListAction.ListTransformerDefinition.EndTransformation(pasteListAction, sourceListCollection, targetWeb.Lists, pasteListAction.Options.Transformers);
						PasteActionUtils.CopyItemLevelWebParts(pasteListAction, pasteListAction.SharePointOptions, base.WebPartPagesNotCopiedAtItemsLevel);
						List<TaskDefinition> taskDefinitions1 = new List<TaskDefinition>();
						if (base.SharePointOptions.CopyGloballyReusableWorkflowTemplates)
						{
							Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
							object[] objArray = new object[] { sourceListCollection, targetWeb, sPList3 };
							taskDefinitions1.Add(threadManager.QueueBufferedTask("CopyGloballyReusableWorkflowTemplateFiles", objArray, new ThreadedOperationDelegate(this.CopyGloballyReusableWorkflowTemplateDelegate)));
						}
						if (base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations)
						{
							Metalogix.Threading.ThreadManager threadManager1 = base.ThreadManager;
							object[] objArray1 = new object[] { sourceListCollection, sPList1, sPList2, targetWeb };
							taskDefinitions1.Add(threadManager1.QueueBufferedTask("CopyWorkflowTemplateFiles", objArray1, new ThreadedOperationDelegate(this.CopySharePointDesignerWorkflowAssociationsTaskDelegate)));
						}
						if (base.SharePointOptions.CopyListOOBWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations)
						{
							Metalogix.Threading.ThreadManager threadManager2 = base.ThreadManager;
							string name = typeof(CopyWorkflowAssociationsAction).Name;
							object[] objArray2 = new object[] { sourceListCollection, targetWeb };
							taskDefinitions1.Add(threadManager2.QueueBufferedTask(name, objArray2, new ThreadedOperationDelegate(this.CopyOOBWorkflowAssociationsTaskDelegate)));
						}
						if (targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && targetWeb.Template.ID.Equals(62) && !targetWeb.Adapter.IsNws)
						{
							this.UpdateWebPropertiesForCommunityWebparts(targetWeb);
						}
						if (AdapterConfigurationVariables.MigrateLanguageSettings && AdapterConfigurationVariables.MigrateLanguageSettingForViews && sourceListCollection.ParentWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && !sourceListCollection.ParentWeb.Adapter.IsDB && !sourceListCollection.ParentWeb.Adapter.IsNws && !sourceListCollection.ParentWeb.Adapter.IsClientOM && targetWeb.Adapter.SharePointVersion.IsSharePoint2016OrLater && targetWeb.Adapter.IsClientOM)
						{
							pasteListAction.CopyLanguageResourcesForView(sourceListCollection.ParentWeb, targetWeb);
						}
					}
				}
			}
		}

		private void CopyOOBWorkflowAssociationsTaskDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			SPListCollection sPListCollection = oParams[0] as SPListCollection;
			SPWeb sPWeb = oParams[1] as SPWeb;
			try
			{
				foreach (SPList sPList in sPListCollection)
				{
					if (!base.CheckForAbort())
					{
						SPList listByGuid = null;
						try
						{
							try
							{
								SPListCollection lists = sPWeb.Lists;
								Guid item = base.GuidMappings[new Guid(sPList.ID)];
								listByGuid = lists.GetListByGuid(item.ToString());
							}
							catch
							{
								string name = sPList.Name;
								if (base.SharePointOptions.RenameSpecificNodes)
								{
									name = MigrationUtils.GetRenamedListName(sPList, sPWeb, base.SharePointOptions.TaskCollection);
								}
								listByGuid = sPWeb.Lists[name];
								if (listByGuid == null)
								{
									continue;
								}
							}
							if (listByGuid != null)
							{
								CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
								copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
								base.SubActions.Add(copyWorkflowAssociationsAction);
								if (base.SharePointOptions.CopyListOOBWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations)
								{
									object[] objArray = new object[] { sPList, listByGuid, sPWeb };
									copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPList, sPWeb), null);
								}
								if (base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations)
								{
									foreach (SPContentType contentType in sPList.ContentTypes)
									{
										SPContentType contentTypeByName = listByGuid.ContentTypes.GetContentTypeByName(contentType.Name);
										if (contentTypeByName == null || sPList.DisplayUrl == listByGuid.DisplayUrl && contentType.ContentTypeID == contentTypeByName.ContentTypeID)
										{
											continue;
										}
										object[] objArray1 = new object[] { contentType, contentTypeByName, listByGuid };
										copyWorkflowAssociationsAction.RunAsSubAction(objArray1, new ActionContext(sPList, sPWeb), null);
									}
								}
							}
						}
						finally
						{
							if (sPList != null)
							{
								sPList.Dispose();
							}
							if (listByGuid != null)
							{
								listByGuid.Dispose();
							}
						}
					}
					else
					{
						return;
					}
				}
			}
			finally
			{
				sPListCollection.ParentWeb.Dispose();
				sPWeb.Dispose();
			}
		}

		private void CopySharePointDesignerWorkflowAssociationsTaskDelegate(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			PasteListAction pasteListAction = new PasteListAction();
			pasteListAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteListAction);
			SPListCollection sPListCollection = oParams[0] as SPListCollection;
			SPList sPList = oParams[1] as SPList;
			SPList sPList1 = oParams[2] as SPList;
			SPWeb sPWeb = oParams[3] as SPWeb;
			try
			{
				if (sPList != null)
				{
					if (!base.CheckForAbort())
					{
						if (sPList != null)
						{
							base.ThreadManager.WaitForTask(pasteListAction.CopyList(sPList, sPWeb, false, true, false));
						}
						if (!sPWeb.Adapter.IsClientOM && sPList1 != null)
						{
							base.ThreadManager.WaitForTask(pasteListAction.CopyList(sPList1, sPWeb, false, true, false));
						}
						if (sPListCollection.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
						{
							if (!base.CheckForAbort())
							{
								CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
								base.SubActions.Add(copyWorkflowAssociationsAction);
								copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
								object[] objArray = new object[] { sPWeb };
								copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPList, sPWeb), null);
							}
							else
							{
								return;
							}
						}
					}
					else
					{
						return;
					}
				}
			}
			finally
			{
				if (sPList != null)
				{
					sPList.Dispose();
				}
				if (sPList1 != null)
				{
					sPList1.Dispose();
				}
				sPListCollection.ParentWeb.Dispose();
				sPWeb.Dispose();
			}
		}

		private IList<SPList> GetDependencies(SPWeb targetWeb, SPListCollection sourceListCollection, SPFieldCollection siteColumns)
		{
			IList<SPList> sPLists = new List<SPList>();
			foreach (SPList sPList in sourceListCollection)
			{
				if ((!base.SharePointOptions.FilterLists ? false : !base.SharePointOptions.ListFilterExpression.Evaluate(sPList, new CompareDatesInUtc())))
				{
					continue;
				}
				SPList sPList1 = null;
				if (MigrationUtils.ListIsBeingPreserved(sPList, targetWeb, base.SharePointOptions, out sPList1))
				{
					continue;
				}
				this.GetListDependencies(sPList, sPLists, sourceListCollection);
			}
			if (siteColumns != null && siteColumns.HasDependencies)
			{
				SPList[] dependencies = siteColumns.GetDependencies();
				for (int i = 0; i < (int)dependencies.Length; i++)
				{
					SPList sPList2 = dependencies[i];
					if (!sPLists.Contains(sPList2))
					{
						sPLists.Add(sPList2);
					}
				}
			}
			return sPLists;
		}

		private void GetKpiListDependencies(SPList list, IList<SPList> dependentListsCollection, SPListCollection sourceListCollection)
		{
			try
			{
				if (list.BaseTemplate == ListTemplateType.KpiList && list.Items.Any<Node>())
				{
					foreach (Node item in list.Items)
					{
						string str = item["DataSource"];
						if (string.IsNullOrEmpty(str))
						{
							continue;
						}
						char[] chrArray = new char[] { ',' };
						string str1 = HttpUtility.UrlDecode(str.Split(chrArray)[0]).Trim();
						foreach (SPList sPList in sourceListCollection)
						{
							string displayUrl = sPList.DisplayUrl;
							char[] chrArray1 = new char[] { '/' };
							if (!str1.StartsWith(string.Format("{0}/", displayUrl.TrimEnd(chrArray1)), StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
							if (dependentListsCollection.FirstOrDefault<SPList>((SPList x) => x.DisplayUrl.Equals(sPList.DisplayUrl, StringComparison.InvariantCultureIgnoreCase)) != null)
							{
								continue;
							}
							dependentListsCollection.Add(sPList);
							break;
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("Error occured while searching Kpi list dependencies. Error: {0}", exception));
			}
		}

		private void GetListDependencies(SPList list, IList<SPList> dependentLists, SPListCollection sourceListCollection)
		{
			this.GetLookUpDependencies(list, dependentLists);
			this.GetKpiListDependencies(list, dependentLists, sourceListCollection);
		}

		private void GetLookUpDependencies(SPList list, IList<SPList> dependentLists)
		{
			try
			{
				SPFieldCollection fields = list.Fields as SPFieldCollection;
				if (fields != null && fields.HasDependencies)
				{
					SPList[] dependencies = fields.GetDependencies();
					for (int i = 0; i < (int)dependencies.Length; i++)
					{
						SPList sPList = dependencies[i];
						if (!dependentLists.Contains(sPList))
						{
							dependentLists.Add(sPList);
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("Error occured while searching look up dependencies. Error: {0}", exception));
			}
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
						SPListCollection lists = item.Lists;
						this.CopyLists(lists, sPWeb, true, true, base.SharePointOptions.CopyContentTypes);
						base.ThreadManager.SetBufferedTasks(base.GetWebPartCopyBufferKey(sPWeb), false, false);
					}
					finally
					{
						sPWeb.Dispose();
					}
				}
				base.StartCommonWorkflowBufferedTasks();
				base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
				base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 5)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopyLists(oParams[0] as SPListCollection, oParams[1] as SPWeb, (bool)oParams[2], (bool)oParams[3], (bool)oParams[4]);
		}

		private void UpdateWebPropertiesForCommunityWebparts(SPWeb targetWeb)
		{
			try
			{
				string serverRelativeUrl = targetWeb.ServerRelativeUrl;
				SPList listByServerRelativeUrl = targetWeb.Lists.GetListByServerRelativeUrl(string.Format("{0}{1}", serverRelativeUrl, "/Lists/Community Discussion"));
				SPList sPList = targetWeb.Lists.GetListByServerRelativeUrl(string.Format("{0}{1}", serverRelativeUrl, "/Lists/Members"));
				StringBuilder stringBuilder = new StringBuilder(1024);
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
				{
					xmlWriter.WriteStartElement("CommunitySiteProperties");
					if (sPList != null)
					{
						xmlWriter.WriteAttributeString("Community_MembersCount", sPList.Items.Count.ToString());
					}
					if (listByServerRelativeUrl != null && !targetWeb.Adapter.IsClientOM)
					{
						int count = listByServerRelativeUrl.Items.Count;
						xmlWriter.WriteAttributeString("Community_TopicsCount", count.ToString());
						int itemCount = listByServerRelativeUrl.ItemCount - count;
						xmlWriter.WriteAttributeString("Community_RepliesCount", itemCount.ToString());
					}
					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
				}
				targetWeb.Adapter.Writer.UpdateWeb(stringBuilder.ToString(), new UpdateWebOptions());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Format("Error occurred while updating community site properties for web '{0}'. Error: {1}", targetWeb.Url, exception));
			}
		}
	}
}