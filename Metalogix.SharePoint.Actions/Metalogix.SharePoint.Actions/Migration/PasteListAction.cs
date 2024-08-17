using Metalogix;
using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Filters;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Common;
using Metalogix.SharePoint.Common.Workflow2013;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.Workflow2013;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[CmdletEnabled(true, "Copy-MLSharePointList", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[Incrementable(true, "Paste List Incrementally")]
	[MandatoryTransformers(new Type[] { typeof(ListColumnMapper), typeof(BDCUpdater), typeof(ListGuidMapper) })]
	[MenuText("1:Paste List... {0-Paste}")]
	[MenuTextPlural("1:Paste Lists... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste List")]
	[RunAsync(true)]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPList))]
	[SubActionTypes(new Type[] { typeof(CopyWorkflowAssociationsAction), typeof(CopyWebPartsAction), typeof(CopyRoleAssignmentsAction), typeof(CopyContentTypesAction), typeof(PasteAllListItemsAction), typeof(PasteAllSubFoldersAction), typeof(CopyListForms) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteListAction : PasteAction<PasteListOptions>
	{
		private bool _isCopyListAction;

		private TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection> listTransformerDefinition = new TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection>("SharePoint Lists", false);

		internal TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection> ListTransformerDefinition
		{
			get
			{
				return listTransformerDefinition;
			}
		}

		public PasteListAction()
		{
		}

		private void AddListMappings(SPList sourceList, SPWeb targetWeb, SPList targetList)
		{
			AddGuidMappings(sourceList.ID, targetList.ID);
			try
			{
				LinkCorrector.AddWebMappings(sourceList.ParentWeb, targetWeb);
			}
			catch (ArgumentException argumentException)
			{
			}
			if (sourceList.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && targetList.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				foreach (SPView view in sourceList.Views)
				{
					if (view.Type != ViewType.Calendar)
					{
						continue;
					}
					var viewByDisplayName = targetList.Views.GetViewByDisplayName(view.DisplayName);
					if (viewByDisplayName == null)
					{
						continue;
					}
					LinkCorrector.AddGuidMapping(view.Name, viewByDisplayName.Name);
				}
			}
			if (SharePointOptions.CorrectingLinks)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						LinkCorrector.Scope = SharePointOptions.LinkCorrectionScope;
						LinkCorrector.PopulateForListCopy(sourceList, targetList, SharePointOptions.TaskCollection, SharePointOptions.UseComprehensiveLinkCorrection);
					}
					catch (Exception exception1)
					{
						var exception = exception1;
						logItem = new LogItem("Initialize Link Corrector for List Copy", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
						FireOperationStarted(logItem);
						logItem.Exception = exception;
						logItem.Details = exception.StackTrace;
					}
				}
				finally
				{
					if (logItem != null)
					{
						FireOperationFinished(logItem);
					}
				}
			}
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!SharePointAction<ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			var enumerator = sourceSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					var current = (SPList)enumerator.Current;
					var enumerator1 = targetSelections.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							var sPWeb = (SPWeb)enumerator1.Current;
							if (!sPWeb.Lists.Contains(current) && !(sPWeb.DisplayUrl == current.Parent.DisplayUrl))
							{
								continue;
							}
							flag = false;
							return flag;
						}
					}
					finally
					{
						var disposable = enumerator1 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				return true;
			}
			finally
			{
				var disposable1 = enumerator as IDisposable;
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
		}

		private void CheckAndTransformProject2003Lists(ref string sListXML, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!sourceWeb.Adapter.SharePointVersion.IsSharePoint2003 || !targetWeb.Adapter.SharePointVersion.IsSharePoint2007)
			{
				return;
			}
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sListXML);
			var firstChild = xmlDocument.FirstChild;
			if (firstChild.Attributes["BaseTemplate"].Value == "1101")
			{
				foreach (SPListTemplate listTemplate in targetWeb.ListTemplates)
				{
					if (listTemplate.TemplateType != (ListTemplateType.Issues | ListTemplateType.IssueTracking))
					{
						continue;
					}
					var attribute = listTemplate.GetAttribute("FeatureId");
					((XmlElement)firstChild).SetAttribute("FeatureId", attribute);
					((XmlElement)firstChild).SetAttribute("BaseType", "0");
					sListXML = firstChild.OuterXml;
					break;
				}
			}
			else if (firstChild.Attributes["BaseTemplate"].Value == "103")
			{
				foreach (SPListTemplate sPListTemplate in targetWeb.ListTemplates)
				{
					if (sPListTemplate.TemplateType != ListTemplateType.Links)
					{
						continue;
					}
					var str = sPListTemplate.GetAttribute("FeatureId");
					((XmlElement)firstChild).SetAttribute("FeatureId", str);
					sListXML = firstChild.OuterXml;
					break;
				}
			}
		}

		private void CopyDefaultColumnSettingsTaskDelegate(object[] oParams)
		{
			var taskDefinition = oParams[0] as TaskDefinition;
			ThreadManager.WaitForTask(taskDefinition);
			var sPList = oParams[1] as SPList;
			var sPList1 = oParams[2] as SPList;
			LogItem logItem = null;
			try
			{
				try
				{
					if (sPList.BaseType == ListType.DocumentLibrary && sPList.Adapter.SharePointVersion.IsSharePoint2010OrLater && !sPList.Adapter.IsNws && !sPList.Adapter.IsDB && sPList1.Adapter.SharePointVersion.IsSharePoint2010OrLater && !sPList1.Adapter.IsNws && !sPList1.Adapter.IsDB)
					{
						logItem = new LogItem("Copying Column Default Value Settings", sPList.Name, sPList.Url, sPList1.Url, ActionOperationStatus.Running);
						FireOperationStarted(logItem);
						var columnDefaultValue = sPList.ColumnDefaultValue;
						if (!string.IsNullOrEmpty(columnDefaultValue))
						{
							var str = MapColumnDefaultSettings(columnDefaultValue, sPList);
							if (!string.IsNullOrEmpty(str))
							{
								var columnDefaultValue1 = new ColumnDefaultValues()
								{
									ListId = sPList1.ID,
									MetadataDefaultSettings = str
								};
								var str1 = SP2013Utils.Serialize<ColumnDefaultValues>(columnDefaultValue1);
								var str2 = sPList1.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.SetColumnDefaultSettings.ToString(), str1);
								var operationReportingResult = new OperationReportingResult(str2);
								if (!operationReportingResult.ErrorOccured)
								{
									if (!operationReportingResult.WarningOccured)
									{
										logItem.Status = ActionOperationStatus.Completed;
									}
									else
									{
										logItem.Status = ActionOperationStatus.Warning;
										logItem.Information = "Please review details";
										logItem.Details = operationReportingResult.GetAllWarningsAsString;
									}
								}
								else if (operationReportingResult.GetAllErrorsAsString.Contains("Could not load file or assembly 'Microsoft.Office.DocumentManagement"))
								{
									logItem.Status = ActionOperationStatus.Warning;
									logItem.Information = "Migration of Column Default Value Settings is not supported for SharePoint Foundation servers";
									logItem.Details = operationReportingResult.GetAllErrorsAsString;
								}
								else if (logItem != null)
								{
									logItem.Status = ActionOperationStatus.Failed;
									logItem.Information = operationReportingResult.GetMessageOfFirstErrorElement;
									logItem.Details = operationReportingResult.AllReportElementsAsString;
								}
							}
						}
					}
				}
				catch (Exception exception1)
				{
					var exception = exception1;
					if (logItem != null)
					{
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Exception = exception;
					}
				}
			}
			finally
			{
				if (logItem != null)
				{
					if (logItem.Status != ActionOperationStatus.Failed)
					{
						logItem.Status = SPUtils.EvaluateLog(logItem);
					}
					FireOperationFinished(logItem);
				}
			}
		}

		private void CopyForms(object[] oParams)
		{
			var taskDefinition = oParams[0] as TaskDefinition;
			if (taskDefinition != null)
			{
				ThreadManager.WaitForTask(taskDefinition);
			}
			if (CheckForAbort())
			{
				return;
			}
			if (!ActionOptions.UpdateLists && !SharePointOptions.OverwriteLists)
			{
				return;
			}
			var copyListForm = new CopyListForms()
			{
				LinkCorrector = LinkCorrector
			};
			copyListForm.SharePointOptions.SetFromOptions(SharePointOptions);
			SubActions.Add(copyListForm);
			var objArray = new object[] { oParams[1], oParams[2] };
			copyListForm.RunAsSubAction(objArray, new ActionContext(oParams[1] as SPList, oParams[2] as SPList), null);
		}

		public void CopyLanguageResourcesForView(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (sourceWeb.LanguageResourcesForViews.Count > 0)
			{
				var logItem = new LogItem("Copying language resources for views", sourceWeb.Name, sourceWeb.Url, targetWeb.Url, ActionOperationStatus.Running);
				FireOperationStarted(logItem);
				var empty = string.Empty;
				var objectXml = string.Empty;
				var flag = false;
				try
				{
					try
					{
						var sourceLanguages = GetSourceLanguages(logItem, sourceWeb.XML, targetWeb);
						if (sourceLanguages == null || sourceLanguages.Count <= 0)
						{
							var logItem1 = logItem;
							logItem1.Information = string.Concat(logItem1.Information, "Source languages are not selected at target. So language resources for list views cannot be copied.");
							logItem.Status = ActionOperationStatus.Warning;
						}
						else
						{
							flag = true;
							var logItem2 = logItem;
							logItem2.Details = string.Concat(logItem2.Details, string.Format("Languages retrieved. Count = {0}", sourceLanguages.Count));
							var flag1 = false;
							if (targetWeb.Adapter.SharePointVersion.IsSharePointOnline)
							{
								empty = targetWeb.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.GetCurrentUserLanguage.ToString(), string.Empty);
								objectXml = (new OperationReportingResult(empty)).ObjectXml;
								var logItem3 = logItem;
								logItem3.Details = string.Concat(logItem3.Details, string.Format(" Original target languages order: '{0}'", objectXml));
							}
							foreach (var sourceLanguage in sourceLanguages)
							{
								flag1 = false;
								if (!targetWeb.Adapter.SharePointVersion.IsSharePointOnline)
								{
									flag1 = true;
								}
								else
								{
									var str = string.Format("{0}#{1}#{2}", sourceLanguage, AdapterConfigurationVariables.LanguageSettingsMaximumInterval, AdapterConfigurationVariables.LanguageSettingsRefreshInterval);
									var str1 = targetWeb.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.SetCurrentUserLanguage.ToString(), str);
									var operationReportingResult = new OperationReportingResult(str1);
									bool.TryParse(operationReportingResult.ObjectXml, out flag1);
								}
								if (!flag1)
								{
									var logItem4 = logItem;
									logItem4.Details = string.Concat(logItem4.Details, string.Format(" Error occurred while setting target language '{0}'", sourceLanguage));
								}
								else
								{
									CopyViewLanguageResources(sourceWeb, targetWeb, sourceLanguage);
								}
							}
							logItem.Status = ActionOperationStatus.Completed;
						}
					}
					catch (Exception exception1)
					{
						var exception = exception1;
						if (logItem != null)
						{
							logItem.Exception = exception;
							logItem.Status = ActionOperationStatus.Failed;
							var logItem5 = logItem;
							logItem5.Details = string.Concat(logItem5.Details, exception.StackTrace);
						}
					}
				}
				finally
				{
					ResetTargetLanguage(targetWeb, objectXml, logItem, flag);
					if (logItem != null)
					{
						sourceWeb.LanguageResourcesForViews.Clear();
						FireOperationFinished(logItem);
					}
				}
			}
		}

		private void CopyLibraryDocumentTemplate(SPList sourceList, SPList targetList)
		{
			string str;
			string str1;
			try
			{
				if (!CheckForAbort())
				{
					var nodeXML = sourceList.GetNodeXML();
					if (sourceList.IsDocumentLibrary)
					{
						if (nodeXML.Attributes["DocTemplateUrl"] != null)
						{
							var value = nodeXML.Attributes["DocTemplateUrl"].Value;
							LogItem logItem = null;
							if (value.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
							{
								var str2 = string.Format(Properties.Resources.FS_CopyingInfoPathTemplate, sourceList.BaseType.ToString());
								if (!targetList.Adapter.SharePointVersion.IsSharePoint2007OrLater)
								{
									logItem = new LogItem(str2, "", sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Failed)
									{
										Information = Properties.Resources.InfoPathMigrationNotSupported
									};
									FireOperationStarted(logItem);
									FireOperationFinished(logItem);
								}
								else
								{
									try
									{
										try
										{
											Utils.ParseUrlForLeafName(value, out str, out str1);
											logItem = new LogItem(str2, str1, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
											FireOperationStarted(logItem);
											var libraryDocumentTemplate = sourceList.GetLibraryDocumentTemplate();
											var uri = new Uri(sourceList.Adapter.ServerUrl);
											var uri1 = new Uri(uri, value);
											var infoPathTemplate = new InfoPathTemplate(libraryDocumentTemplate, uri1.AbsoluteUri);
											var str3 = "";
											libraryDocumentTemplate = infoPathTemplate.GetReLinkedTemplate(LinkCorrector, out str3);
											var xmlNode = XmlUtility.StringToXmlNode(targetList.XML) as XmlElement;
											xmlNode.SetAttribute("BrowserActivatedTemplate", infoPathTemplate.IsTemplateBrowserActivated().ToString());
											targetList.UpdateList(xmlNode.OuterXml, null, false, false, false, libraryDocumentTemplate);
											if (!string.IsNullOrEmpty(str3))
											{
												logItem.Information = Properties.Resources.SuccessReviewDetails;
												logItem.Details = str3;
											}
											logItem.Status = ActionOperationStatus.Completed;
										}
										catch (Exception exception1)
										{
											var exception = exception1;
											if (logItem == null)
											{
												logItem = new LogItem(str2, string.Empty, string.Empty, targetList.DisplayUrl, ActionOperationStatus.Failed);
												FireOperationStarted(logItem);
											}
											logItem.Exception = exception;
										}
									}
									finally
									{
										FireOperationFinished(logItem);
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				if (sourceList != null)
				{
					sourceList.Dispose();
				}
				if (targetList != null)
				{
					targetList.Dispose();
				}
			}
		}

		private void CopyLibraryDocumentTemplate(object[] parameters)
		{
			CopyLibraryDocumentTemplate((SPList)parameters[0], (SPList)parameters[1]);
		}

		public TaskDefinition CopyList(SPList sourceList, SPWeb targetWeb, bool bIsCopyRoot = false, bool bDisablePermissions = false, bool bCopyingForDependency = false)
		{
			TaskDefinition taskDefinition;
			object[] objArray;
			TransformationTaskCollection taskCollection;
			LogItem logItem = null;
			TaskDefinition taskDefinition1 = null;
			var flag = false;
			try
			{
				try
				{
					if (CheckForAbort())
					{
						sourceList.Dispose();
						taskDefinition = taskDefinition1;
						return taskDefinition;
					}
					else if (!string.IsNullOrEmpty(sourceList.Name))
					{
						SPList resultObject = null;
						string outerXml = null;
						var flag1 = false;
						var flag2 = false;
						if (!(!SharePointOptions.FilterLists ? false : !SharePointOptions.ListFilterExpression.Evaluate(sourceList, new CompareDatesInUtc())))
						{
							if (sourceList.BaseTemplate != ListTemplateType.AccessRequest)
							{
								var flag3 = false;
								string xML = null;
								try
								{
									try
									{
										logItem = new LogItem("Fetching list information", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
										SPList sPList = null;
										var flag4 = MigrationUtils.ListIsBeingPreserved(sourceList, targetWeb, SharePointOptions, out sPList);
										var flag5 = false;
										if ((!sourceList.Adapter.IsDB ? false : sourceList.Adapter.SharePointVersion.IsSharePoint2007OrLater))
										{
											if ((!sourceList.IsDocumentLibrary ? false : sourceList.Name.Equals("Style Library")))
											{
												if (targetWeb.PublishingInfrastructureActive && sPList != null)
												{
													flag5 = true;
												}
											}
										}
										var flag6 = false;
										if (ShouldPreserveAssetList(sourceList, targetWeb, sPList))
										{
											flag6 = true;
										}
										var flag7 = false;
										if (!SharePointOptions.CopyListItems)
										{
											var flag8 = (!sourceList.IsDocumentLibrary ? false : sourceList.Name.Equals("Pages"));
											var flag9 = (!sourceList.IsDocumentLibrary ? false : sourceList.Name.Equals("SitePages"));
											var welcomePageUrl = sourceList.ParentWeb.WelcomePageUrl;
											if (!flag8)
											{
												if (flag9 && sourceList.ParentWeb.WikiWelcomePageActive && targetWeb.WikiWelcomePageActive && welcomePageUrl != null && welcomePageUrl.EndsWith("/SitePages/Home.aspx", StringComparison.OrdinalIgnoreCase))
												{
													flag7 = true;
												}
											}
											else if (sourceList.ParentWeb.IsPublishingTemplate && targetWeb.IsPublishingTemplate)
											{
												if (welcomePageUrl != null && welcomePageUrl.EndsWith("/Pages/default.aspx", StringComparison.OrdinalIgnoreCase))
												{
													flag7 = true;
												}
											}
											else if (sourceList.ParentWeb.IsWikiTemplate && targetWeb.IsWikiTemplate && welcomePageUrl != null && welcomePageUrl.EndsWith("/Pages/Home.aspx", StringComparison.OrdinalIgnoreCase))
											{
												flag7 = true;
											}
										}
										flag2 = true;
										string str = null;
										if (!SharePointOptions.UpdateLists)
										{
											flag2 = false;
											str = "List has not been flagged for updating.";
										}
										else if (flag4)
										{
											flag2 = false;
											str = "List is being preserved.";
										}
										else if (sPList == null)
										{
											flag2 = false;
											str = "Target list does not exist.";
										}
										else if (SharePointOptions.CheckModifiedDatesForLists && sourceList.Modified < sPList.Modified)
										{
											flag2 = false;
											str = "Target list is newer than source.";
										}
										var flag10 = false;
										var flag11 = false;
										if (sPList != null)
										{
											if (!SharePointOptions.OverwriteLists)
											{
												if (!flag2)
												{
													flag10 = true;
													logItem.Operation = "Skipping List";
													logItem.Information = str;
													logItem.Status = ActionOperationStatus.Skipped;
												}
											}
											else if (flag6)
											{
												logItem.Operation = "Preserving list";
												logItem.Information = string.Format("'{0}' is being preserved as it contains OneNote folder.", sPList.Title);
											}
											else if (flag5 || flag7)
											{
												logItem.Operation = "Preserving list";
												logItem.Information = string.Format("'{0}' is being preserved as it contains template data.", sPList.Title);
											}
											else
											{
												logItem.Operation = "Overwriting existing list";
												DeleteList(ref sPList);
												flag11 = sPList != null;
											}
										}
										FireOperationStarted(logItem);
										flag = true;
										flag3 = true;
										if (!flag10)
										{
											if (sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater && SharePointOptions.CopyReferencedManagedMetadata)
											{
												EnsureManagedMetadataExistence(sourceList.FieldCollection, sourceList.ParentWeb, targetWeb, SharePointOptions.ResolveManagedMetadataByName);
											}
											if (!bCopyingForDependency)
											{
												sourceList = listTransformerDefinition.Transform(sourceList, this, sourceList.ParentWeb.Lists, targetWeb.Lists, Options.Transformers);
											}
											if (sourceList != null)
											{
												xML = sourceList.XML;
												var str1 = xML;
												var sPList1 = sourceList;
												var sPWeb = targetWeb;
												if (SharePointOptions.RenameSpecificNodes)
												{
													taskCollection = SharePointOptions.TaskCollection;
												}
												else
												{
													taskCollection = null;
												}
												outerXml = MigrationUtils.RenameListInXml(str1, sPList1, sPWeb, taskCollection);
												outerXml = MapListTemplate(outerXml, sourceList.DisplayUrl, targetWeb);
												var xmlDocument = new XmlDocument();
												xmlDocument.LoadXml(outerXml);
												var flag12 = (!sourceList.Adapter.SharePointVersion.IsSharePoint2007OrEarlier || !targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater ? false : sourceList.IsDocumentLibrary);
												var xmlNodes = new List<XmlNode>();
												var xmlNodes1 = xmlDocument.SelectSingleNode(".//Views");
												if (xmlNodes1 != null)
												{
													foreach (XmlNode xmlNodes2 in xmlNodes1)
													{
														if (!flag12 || xmlNodes2.Attributes["BaseViewID"] == null || !xmlNodes2.Attributes["BaseViewID"].Value.Equals("3"))
														{
															if (xmlNodes2.Attributes.GetAttributeValueAsBoolean("Hidden"))
															{
																continue;
															}
															if (!xmlNodes2.Attributes.GetAttributeValueAsBoolean(XmlAttributeNames.MobileView.ToString()))
															{
																var xmlAttribute = xmlDocument.CreateAttribute(XmlAttributeNames.MobileView.ToString());
																xmlAttribute.Value = false.ToString();
																xmlNodes2.Attributes.Append(xmlAttribute);
															}
															if (xmlNodes2.Attributes.GetAttributeValueAsBoolean(XmlAttributeNames.MobileDefaultView.ToString()))
															{
																continue;
															}
															var xmlAttribute1 = xmlDocument.CreateAttribute(XmlAttributeNames.MobileDefaultView.ToString());
															xmlAttribute1.Value = false.ToString();
															xmlNodes2.Attributes.Append(xmlAttribute1);
														}
														else
														{
															xmlNodes.Add(xmlNodes2);
														}
													}
												}
												if (flag12)
												{
													foreach (var xmlNode in xmlNodes)
													{
														xmlNodes1.RemoveChild(xmlNode);
													}
												}
												outerXml = xmlDocument.OuterXml;
												CheckAndTransformProject2003Lists(ref outerXml, sourceList.ParentWeb, targetWeb);
												var flag13 = false;
												if (SharePointOptions.ApplyNewContentTypes && SharePointOptions.ContentTypeApplicationObjects != null)
												{
													foreach (var contentTypeApplicationObject in SharePointOptions.ContentTypeApplicationObjects)
													{
														if (!contentTypeApplicationObject.AppliesTo(sourceList) || contentTypeApplicationObject.Data.Count <= 0)
														{
															continue;
														}
														SetContentTypesEnabled(ref outerXml);
														flag13 = true;
														break;
													}
												}
												if (!flag13 && SharePointOptions.ApplyNewDocumentSets && SharePointOptions.DocumentSetApplicationObjects != null)
												{
													foreach (var documentSetApplicationObject in SharePointOptions.DocumentSetApplicationObjects)
													{
														if (!documentSetApplicationObject.AppliesTo(sourceList) || documentSetApplicationObject.Data.Count <= 0)
														{
															continue;
														}
														SetContentTypesEnabled(ref outerXml);
														break;
													}
												}
												outerXml = LinkCorrector.UpdateLinksInList(sourceList, targetWeb, outerXml);
												if (SharePointOptions.CopyWorkflowInstanceData && (sourceList.BaseTemplate == ListTemplateType.Tasks || sourceList.BaseTemplate == ListTemplateType.TasksWithTimelineAndHierarchy))
												{
													ModifyTaskListXml(ref outerXml, sourceList);
												}
												if (sourceList.Adapter.SharePointVersion.IsSharePoint2013OrLater && sourceList.ParentWeb.IsSharePoint2013WorkflowsAvailable && !sourceList.Adapter.IsDB && !sourceList.Adapter.IsNws && (!sourceList.Adapter.IsMEWS || !sourceList.ParentWeb.IsReadOnly))
												{
													OperationReportingResultObject<List<SP2013WorkflowSubscription>> sP2013Workflows = null;
													var str2 = SP2013Utils.CreateSp2013WorkflowConfigXml(new Guid(sourceList.ID), null);
													try
													{
														sP2013Workflows = sourceList.SP2013WorkflowCollection.GetSP2013Workflows(str2);
													}
													catch (Exception exception)
													{
														Trace.WriteLine(exception.Message);
													}
													if (sP2013Workflows != null && (sP2013Workflows.ErrorOccured || sP2013Workflows.ResultObject == null))
													{
														throw new OperationReportingException(sP2013Workflows.GetAllErrorsAsString, sP2013Workflows.GetAllErrorsAsString);
													}
													Modify2013WorkflowXml(ref outerXml, sP2013Workflows.ResultObject);
												}
											}
											else
											{
												logItem.Status = ActionOperationStatus.Skipped;
												logItem.Information = "List skipped by transformation";
												taskDefinition = taskDefinition1;
												return taskDefinition;
											}
										}
										if (!string.IsNullOrEmpty(outerXml) && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
										{
											outerXml = MigrationUtils.UpdateCheckedOutToFieldRefsInViewsXml(outerXml);
										}
										if (sPList != null)
										{
											resultObject = sPList;
											if (!bCopyingForDependency && (flag11 || flag2))
											{
												if (flag11)
												{
													flag1 = true;
													logItem.Operation = "Adding List";
													FireOperationUpdated(logItem);
												}
												else if (flag2)
												{
													logItem.Operation = "Updating List";
													FireOperationUpdated(logItem);
												}
												var flag14 = (flag11 ? true : (SharePointOptions.UpdateListOptionsBitField & 1) > 0);
												var flag15 = (flag11 ? true : (SharePointOptions.UpdateListOptionsBitField & 2) > 0);
												var flag16 = (flag11 ? true : (SharePointOptions.UpdateListOptionsBitField & 4) > 0);
												var flag17 = (flag14 || flag15 ? true : flag16);
												if (flag15)
												{
													outerXml = ProcessingUserColumn(sourceList, targetWeb, outerXml);
												}
												var xmlNode1 = XmlUtility.StringToXmlNode(outerXml);
												if (sourceList.BaseTemplate == ListTemplateType.ReportLibrary && sourceList.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
												{
													flag16 = false;
													ProcessReportLibraryFieldsForCopy(xmlNode1.SelectSingleNode("./Fields"));
													var logItem1 = new LogItem("Updating Views", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
													{
														Information = "List views cannot be updated for Report Library migrating from 2007 to 2010."
													};
													FireOperationStarted(logItem1);
													FireOperationFinished(logItem1);
												}
												if (!flag11)
												{
													if (!flag14)
													{
														xmlNode1.Attributes.RemoveAll();
													}
													if (!flag15)
													{
														var xmlNodes3 = xmlNode1.SelectSingleNode("./Fields");
														if (xmlNodes3 != null)
														{
															xmlNode1.RemoveChild(xmlNodes3);
														}
													}
													if (!flag16)
													{
														var xmlNodes4 = xmlNode1.SelectSingleNode("./Views");
														if (xmlNodes4 != null)
														{
															xmlNode1.RemoveChild(xmlNodes4);
														}
													}
												}
												if (xmlNode1 != null && (flag11 || flag17))
												{
													var operationReportingResult = resultObject.UpdateList(xmlNode1.OuterXml, flag15, flag16);
													if (operationReportingResult != null && operationReportingResult.ErrorOccured)
													{
														LogFieldsFailureInJobLogs(sourceList, targetWeb, operationReportingResult.GetAllErrorsAsString);
													}
												}
											}
										}
										else
										{
											logItem.Operation = "Adding List";
											FireOperationUpdated(logItem);
											var libraryDocumentTemplate = GetLibraryDocumentTemplate(sourceList);
											var addListOption = new AddListOptions()
											{
												Overwrite = SharePointOptions.OverwriteLists,
												UpdateFieldTypes = true
											};
											if (bCopyingForDependency)
											{
												addListOption.CopyFields = false;
												addListOption.CopyViews = false;
											}
											if (sourceList.BaseTemplate == ListTemplateType.ReportLibrary)
											{
												if (sourceList.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
												{
													addListOption.CopyViews = false;
													var xmlNode2 = XmlUtility.StringToXmlNode(outerXml);
													ProcessReportLibraryFieldsForCopy(xmlNode2.SelectSingleNode("./Fields"));
													outerXml = xmlNode2.OuterXml;
													var logItem2 = new LogItem("Updating Views", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
													{
														Information = "List views cannot be updated for Report Library migrating from 2007 to 2010."
													};
													FireOperationStarted(logItem2);
													FireOperationFinished(logItem2);
												}
											}
											else if (sourceList.BaseTemplate == ListTemplateType.DiscussionBoard && sourceList.Adapter.SharePointVersion.IsSharePoint2003 && targetWeb.Adapter.IsNws)
											{
												addListOption.CopyViews = false;
												var logItem3 = new LogItem("Updating Views", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
												{
													Information = "List views cannot be updated for Discussion Board migrating from 2003 to 2007 or 2010 NWS target."
												};
												FireOperationStarted(logItem3);
												FireOperationFinished(logItem3);
											}
											else if (sourceList.BaseTemplate == ListTemplateType.DiscussionBoard && sourceList.Adapter.SharePointVersion.IsSharePoint2007OrEarlier && targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater)
											{
												addListOption.CopyViews = false;
												var logItem4 = new LogItem("Updating Views", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
												{
													Information = "List views cannot be updated for Discussion Board migrating from 2003/2007 to 2013 or later target."
												};
												FireOperationStarted(logItem4);
												FireOperationFinished(logItem4);
											}
											else if (sourceList.BaseTemplate == ListTemplateType.AssetLibrary && sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater && sourceList.Adapter.IsDB && targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater)
											{
												addListOption.CopyViews = false;
												var logItem5 = new LogItem("Updating Views", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
												{
													Information = "List Views cannot be updated for Asset Library migrating from 2010DB to 2013 or later target."
												};
												FireOperationStarted(logItem5);
												FireOperationFinished(logItem5);
											}
											var flag18 = false;
											if (XmlUtility.GetBooleanAttributeFromXml(sourceList.GetListXML(false), "BrowserActivatedTemplate", out flag18) && flag18 && sourceList.ParentWeb.HasFormsServiceFeature && !targetWeb.HasFormsServiceFeature)
											{
												var logItem6 = new LogItem(logItem.Operation, sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
												{
													Information = "Template will be not browser activated because Forms Service is not activated on target."
												};
												FireOperationStarted(logItem6);
												FireOperationFinished(logItem6);
											}
											outerXml = ProcessingUserColumn(sourceList, targetWeb, outerXml);
											var operationReportingResultObject = targetWeb.Lists.AddList(outerXml, libraryDocumentTemplate, addListOption);
											if (operationReportingResultObject != null)
											{
												resultObject = operationReportingResultObject.ResultObject;
												flag1 = true;
												if (operationReportingResultObject.ErrorOccured)
												{
													LogFieldsFailureInJobLogs(sourceList, targetWeb, operationReportingResultObject.GetAllErrorsAsString);
												}
											}
										}
										if (!flag10)
										{
											if (SharePointOptions.CheckResults && !bCopyingForDependency)
											{
												CompareNodes(sourceList, resultObject, logItem);
											}
											else if (targetWeb.WebTemplateID != 62 || sourceList.BaseTemplate != ListTemplateType.DiscussionBoard)
											{
												logItem.Status = ActionOperationStatus.Completed;
											}
											else
											{
												logItem.Status = ActionOperationStatus.Warning;
												logItem.Information = "Migrating a Community Site’s Discussion list is not fully supported in Content Matrix. There are some properties (hidden and visible) that are not preserved on migration, such as the Best Reply or Editor Comments.";
											}
											logItem.AddCompletionDetail(Properties.Resources.Migration_Detail_ListsCopied, (long)1);
										}
										if (flag1)
										{
											if (resultObject.Adapter.SharePointVersion.IsSharePointOnline && SharePointOptions.UseAzureOffice365Upload && !resultObject.IsMigrationPipelineSupportedForTarget && resultObject.BaseTemplate == ListTemplateType.AssetLibrary)
											{
												logItem.Information = "Publishing Images libraries are migrated to SharePoint Online as Asset Libraries, which are not supported by the Azure migration pipeline. As a result this library and it's contents will be migrated using CSOM";
											}
											var licenseDataUsed = logItem;
											licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + SPObjectSizes.GetObjectSize(sourceList);
										}
										if (SharePointOptions.Verbose)
										{
											logItem.SourceContent = sourceList.XML;
											logItem.TargetContent = resultObject.XML;
										}
									}
									catch (Exception exception2)
									{
										var exception1 = exception2;
										if (!flag3)
										{
											FireOperationStarted(logItem);
										}
										SPList item = null;
										if (outerXml != null)
										{
											var xmlNode3 = XmlUtility.StringToXmlNode(outerXml);
											if (xmlNode3 != null && xmlNode3.Attributes["Name"] != null)
											{
												var value = xmlNode3.Attributes["Name"].Value;
												targetWeb.Lists.FetchData();
												item = targetWeb.Lists[value];
											}
										}
										if (resultObject != null || item == null)
										{
											logItem.Exception = exception1;
											logItem.SourceContent = (xML != null ? xML : "");
											sourceList.Dispose();
											taskDefinition = taskDefinition1;
											return taskDefinition;
										}
										else
										{
											var logItem7 = new LogItem("Updating list information", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
											FireOperationStarted(logItem7);
											logItem7.Exception = exception1;
											logItem7.SourceContent = (xML != null ? xML : "");
											FireOperationFinished(logItem7);
											resultObject = item;
											logItem.Status = ActionOperationStatus.Completed;
											logItem.AddCompletionDetail(Properties.Resources.Migration_Detail_ListsCopied, (long)1);
										}
									}
								}
								finally
								{
									FireOperationFinished(logItem);
								}
							}
							else
							{
								taskDefinition = null;
								return taskDefinition;
							}
						}
						else if (!Is2003IncrementalCopySpecialCase(sourceList, targetWeb, out resultObject))
						{
							LogItem logItem8 = null;
							logItem8 = new LogItem("Skipping List", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Skipped)
							{
								Information = "Filtered out by list filters."
							};
							FireOperationStarted(logItem8);
							sourceList.Dispose();
							taskDefinition = taskDefinition1;
							return taskDefinition;
						}
						AddListMappings(sourceList, targetWeb, resultObject);
						if (!bCopyingForDependency)
						{
							sourceList.IncludePreviousWorkflowVersions = (!SharePointOptions.CopyPreviousVersionOfWorkflowInstances ? false : SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations);
							if (SharePointOptions.CopyWorkflowInstanceData && sourceList.WorkflowAssociations.Count > 0)
							{
								ExtractViewColumnsFromListXml(outerXml, resultObject);
							}
							if (flag1 || flag2 && (SharePointOptions.UpdateListOptionsBitField & 16) > 0)
							{
								CopyListContentTypes(sourceList, resultObject, targetWeb.ContentTypes);
							}
							UpdateContentTypeIdsInListViews(sourceList, resultObject);
							if (resultObject.BaseType == ListType.DocumentLibrary)
							{
								var threadManager = ThreadManager;
								objArray = new object[] { sourceList, resultObject };
								threadManager.QueueBufferedTask("CopyDocumentTemplatesforContentTypes", objArray, CopyLibraryDocumentTemplate);
							}
							if (flag1 || flag2 && (SharePointOptions.UpdateListOptionsBitField & 4) > 0)
							{
								var webPartOption = new WebPartOptions();
								webPartOption.SetFromOptions(SharePointOptions);
								QueueListViewWebPartCopies(sourceList, resultObject, webPartOption);
							}
							if (SharePointOptions.CorrectingLinks && sourceList.Adapter.SharePointVersion.IsSharePoint2010OrLater && resultObject.Adapter.SharePointVersion.IsSharePoint2010OrLater)
							{
								QueueOverlayCalendarLinkCorrections(sourceList, resultObject);
							}
							if (!CheckForAbort())
							{
								var flag19 = false;
								if (sourceList.Adapter.SharePointVersion.IsSharePoint2007OrLater && sourceList.ItemCount == 0 && (!CheckPreservingItems(sourceList, SharePointOptions) || !SharePointOptions.PropagateItemDeletions))
								{
									flag19 = true;
								}
								var flag20 = false;
								if (SharePointOptions.ApplyNewContentTypes && !SharePointOptions.CopyListItems || sourceList.ItemCount == 0)
								{
									ApplyNewContentType(sourceList, resultObject, SharePointOptions);
									flag20 = true;
								}
								if (!bDisablePermissions && (SharePointOptions.CopyListPermissions || !flag19 && (SharePointOptions.CopyFolderPermissions || SharePointOptions.CopyItemPermissions)) && (!resultObject.Adapter.SharePointVersion.IsSharePointOnline || !SharePointOptions.UseAzureOffice365Upload || !sourceList.IsMigrationPipelineSupported || !resultObject.IsMigrationPipelineSupportedForTarget))
								{
									var threadManager1 = ThreadManager;
									var keyFor = PermissionsKeyFormatter.GetKeyFor(resultObject.ParentWeb);
									objArray = new object[] { sourceList, resultObject, flag1, bIsCopyRoot, SharePointOptions.UpdateLists };
									threadManager1.QueueBufferedTask(keyFor, objArray, CopyListPermissionsTaskDelegate);
								}
								if (flag19)
								{
									objArray = new object[] { taskDefinition1, sourceList, resultObject };
									CopyDefaultColumnSettingsTaskDelegate(objArray);
									objArray = new object[] { sourceList, resultObject };
									DisposeListsTaskDelegate(objArray);
								}
								else
								{
									if (SharePointOptions.CopyListItems)
									{
										var objs = new List<object>()
										{
											sourceList,
											resultObject
										};
										if (resultObject.Adapter.SharePointVersion.IsSharePointOnline && SharePointOptions.UseAzureOffice365Upload && sourceList.IsMigrationPipelineSupported && resultObject.IsMigrationPipelineSupportedForTarget)
										{
											if (!bDisablePermissions && SharePointOptions.CopyListPermissions && (flag1 || !flag1 && SharePointOptions.UpdateLists) && (sourceList.HasUniquePermissions || bIsCopyRoot && SharePointOptions.CopyRootPermissions))
											{
												objs.Add(true);
											}
										}
										taskDefinition1 = ThreadManager.QueueTask(objs.ToArray(), CopyListItemsTaskDelegate);
									}
									else if (SharePointOptions.CopySubFolders)
									{
										var threadManager2 = ThreadManager;
										objArray = new object[] { sourceList, resultObject, flag20 };
										taskDefinition1 = threadManager2.QueueTask(objArray, CopySubFoldersTaskDelegate);
									}
									var threadManager3 = ThreadManager;
									var str3 = string.Concat("Dispose", resultObject.ID);
									objArray = new object[] { taskDefinition1, sourceList, resultObject };
									threadManager3.QueueMarshalledTask(str3, objArray, CopyDefaultColumnSettingsTaskDelegate);
									if (!bIsCopyRoot)
									{
										sourceList.Collapse();
										resultObject.Collapse();
									}
									var threadManager4 = ThreadManager;
									var str4 = string.Concat("Dispose", resultObject.ID);
									objArray = new object[] { sourceList, resultObject };
									threadManager4.QueueMarshalledTask(str4, objArray, DisposeListsTaskDelegate);
								}
								if (SharePointOptions.CopyFormWebParts)
								{
									var threadManager5 = ThreadManager;
									objArray = new object[] { taskDefinition1, sourceList, resultObject };
									threadManager5.QueueTask(objArray, CopyForms);
								}
							}
							else
							{
								sourceList.Dispose();
								resultObject.Dispose();
								taskDefinition = taskDefinition1;
								return taskDefinition;
							}
						}
						else
						{
							taskDefinition = taskDefinition1;
							return taskDefinition;
						}
					}
					else
					{
						LogItem logItem9 = null;
						logItem9 = new LogItem("Adding List", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
						{
							Information = "This list cannot be copied because the name of the list is empty or missing. Sometimes this occurs when a list has had its default view deleted or otherwise is missing its default view, but could also be caused by other forms of list corruption."
						};
						FireOperationStarted(logItem9);
						FireOperationFinished(logItem9);
						sourceList.Dispose();
						taskDefinition = taskDefinition1;
						return taskDefinition;
					}
				}
				catch (Exception exception4)
				{
					var exception3 = exception4;
					if (logItem == null)
					{
						logItem = new LogItem("Fetching list information", (sourceList != null ? sourceList.Name : string.Empty), (sourceList != null ? sourceList.DisplayUrl : string.Empty), (targetWeb != null ? targetWeb.DisplayUrl : string.Empty), ActionOperationStatus.Running);
						FireOperationStarted(logItem);
						flag = true;
					}
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Exception = exception3;
					logItem.Information = "Error occured while migrating list";
				}
				return taskDefinition1;
			}
			finally
			{
				if (logItem != null && flag)
				{
					FireOperationFinished(logItem);
				}
			}
			return taskDefinition;
		}

		private void CopyListContentTypes(SPList sourceList, SPList targetList, SPContentTypeCollection targetContentTypes)
		{
			if (!ListAllowsContentTypes(targetList))
			{
				return;
			}
			CopyContentTypesAction copyContentTypesAction = null;
			copyContentTypesAction = new CopyContentTypesAction();
			SubActions.Add(copyContentTypesAction);
			copyContentTypesAction.WorkflowMappings = WorkflowMappings;
			copyContentTypesAction.Options.SetFromOptions(Options);
			var objArray = new object[] { sourceList, targetList, targetContentTypes };
			copyContentTypesAction.RunAsSubAction("CopyListContentTypes", objArray, new ActionContext(sourceList.ParentWeb, targetList.ParentWeb));
		}

		private void CopyListItemsTaskDelegate(object[] oParams)
		{
			var sPList = oParams[0] as SPList;
			var sPList1 = oParams[1] as SPList;
			var baseTemplate = sPList.BaseTemplate == ListTemplateType.Survey;
			var flag = false;
			if (baseTemplate)
			{
				flag = IsAllowMultipleResponseSettingEnabled(sPList1.XML);
				if (!flag)
				{
					UpdateAllowMultipleResponseSetting(sPList1, "True");
				}
			}
			var pasteAllListItemsAction = new PasteAllListItemsAction();
			pasteAllListItemsAction.Options.SetFromOptions(Options);
			pasteAllListItemsAction.IsValidationSettingDisablingRequired = true;
			if ((SharePointOptions.MigrationMode != MigrationMode.Custom || SharePointOptions.ItemCopyingMode != ListItemCopyMode.Preserve) && MigrationUtils.IsListWithDefaultItems(sPList1) && IsListWithPreserveIdOverClientOM(sPList1))
			{
				pasteAllListItemsAction.ActionOptions = MigrationUtils.ChangeItemOptionsToUpdate(pasteAllListItemsAction.ActionOptions, true) as PasteFolderOptions;
			}
			SubActions.Add(pasteAllListItemsAction);
			pasteAllListItemsAction.RunAsSubAction(oParams, new ActionContext(sPList, sPList1), null);
			if (baseTemplate && !flag)
			{
				UpdateAllowMultipleResponseSetting(sPList1, "False");
			}
			if (pasteAllListItemsAction.WebPartPagesNotCopiedAtItemsLevel.Count > 0)
			{
				foreach (var key in pasteAllListItemsAction.WebPartPagesNotCopiedAtItemsLevel.Keys)
				{
					WebPartPagesNotCopiedAtItemsLevel.Add(key, pasteAllListItemsAction.WebPartPagesNotCopiedAtItemsLevel[key]);
				}
				pasteAllListItemsAction.WebPartPagesNotCopiedAtItemsLevel.Clear();
			}
		}

		private void CopyListPermissionsTaskDelegate(object[] oParams)
		{
			bool flag;
			var sPList = oParams[0] as SPList;
			var sPList1 = oParams[1] as SPList;
			var flag1 = (bool)oParams[2];
			var flag2 = (bool)oParams[3];
			var flag3 = (bool)oParams[4];
			if (!SharePointOptions.CopyListPermissions)
			{
				flag = false;
			}
			else if (flag1)
			{
				flag = true;
			}
			else
			{
				flag = (flag1 ? false : flag3);
			}
			if (flag && (sPList.HasUniquePermissions || flag2 && SharePointOptions.CopyRootPermissions))
			{
				var logItem = new LogItem("Copying list permissions", sPList.Name, sPList.DisplayUrl, sPList1.DisplayUrl, ActionOperationStatus.Running);
				FireOperationStarted(logItem);
				try
				{
					var copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
					copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(SharePointOptions);
					SubActions.Add(copyRoleAssignmentsAction);
					var objArray = new object[] { sPList, sPList1, !flag1 };
					copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPList.ParentWeb, sPList1.ParentWeb), null);
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
				FireOperationFinished(logItem);
			}
			if (SharePointOptions.CopyFolderPermissions || SharePointOptions.CopyItemPermissions)
			{
				ThreadManager.SetBufferedTasks(PermissionsKeyFormatter.GetKeyFor(sPList1), false, false);
			}
		}

		private void CopySubFoldersTaskDelegate(object[] oParams)
		{
			var sPList = oParams[0] as SPList;
			var sPList1 = oParams[1] as SPList;
			var flag = false;
			if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
			{
				flag = (bool)oParams[2];
			}
			var pasteAllSubFoldersAction = new PasteAllSubFoldersAction();
			pasteAllSubFoldersAction.SharePointOptions.SetFromOptions(SharePointOptions);
			pasteAllSubFoldersAction.IsValidationSettingDisablingRequired = true;
			SubActions.Add(pasteAllSubFoldersAction);
			var objArray = new object[] { sPList, sPList1, flag };
			pasteAllSubFoldersAction.RunAsSubAction(objArray, new ActionContext(sPList, sPList1), null);
		}

		private void CopyViewLanguageResources(SPWeb sourceWeb, SPWeb targetWeb, string language)
		{
			var str = string.Format("Copying language resources in '{0}' language", language);
			var logItem = new LogItem(str, language, sourceWeb.Url, targetWeb.Url, ActionOperationStatus.Running);
			FireOperationStarted(logItem);
			try
			{
				try
				{
					var languageResourcesForViews = 
						from viewLanguageResource in sourceWeb.LanguageResourcesForViews
						where viewLanguageResource.Language.Equals(language, StringComparison.InvariantCultureIgnoreCase)
						select viewLanguageResource;
					if (!languageResourcesForViews.Any<ViewLanguageResource>())
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						var listViewLanguageResourceConfiguration = new ListViewLanguageResourceConfiguration()
						{
							ViewLanguageResoures = languageResourcesForViews.ToList<ViewLanguageResource>()
						};
						var str1 = SP2013Utils.Serialize<ListViewLanguageResourceConfiguration>(listViewLanguageResourceConfiguration);
						var str2 = targetWeb.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.CopyLanguageResourcesForViews.ToString(), str1);
						logItem = MigrationUtils.GetLogItemDetails(logItem, str2);
					}
				}
				catch (Exception exception1)
				{
					var exception = exception1;
					if (logItem != null)
					{
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						var logItem1 = logItem;
						logItem1.Details = string.Concat(logItem1.Details, exception.StackTrace);
					}
				}
			}
			finally
			{
				if (logItem != null)
				{
					FireOperationFinished(logItem);
				}
			}
		}

		private bool CopyWorkflowsListItems(SPList sourceList, SPWeb targetWeb)
		{
			var flag = false;
			try
			{
				if (sourceList.WorkflowAssociations != null && sourceList.WorkflowAssociations.Any<SPWorkflowAssociation>())
				{
					var listByTitle = sourceList.ParentWeb.Lists.GetListByTitle("Workflows");
					if (listByTitle == null)
					{
						LogErrorOnWorkflowItemsCopy(sourceList.Title, sourceList.DisplayUrl, targetWeb.DisplayUrl, null, "Workflow items cannot be copied to target as source workflows list is missing.");
					}
					else
					{
						var sPList = targetWeb.Lists.GetListByTitle("Workflows");
						if (sPList == null)
						{
							LogErrorOnWorkflowItemsCopy(listByTitle.Title, listByTitle.DisplayUrl, targetWeb.DisplayUrl, null, "Workflow items cannot be copied to target as target workflows list is missing.");
						}
						else
						{
							foreach (var workflowAssociation in sourceList.WorkflowAssociations)
							{
								try
								{
									var actionContext = new ActionContext(listByTitle, sPList);
									var pasteFolderAction = new PasteFolderAction();
									pasteFolderAction.SharePointOptions.SetFromOptions(SharePointOptions);
									SubActions.Add(pasteFolderAction);
									foreach (SPFolder subFolder in listByTitle.SubFolders)
									{
										try
										{
											if (workflowAssociation.Name.Equals(subFolder.Name, StringComparison.InvariantCultureIgnoreCase))
											{
												var objArray = new object[] { subFolder, sPList, false, false };
												pasteFolderAction.RunAsSubAction(objArray, actionContext, null);
												flag = true;
												break;
											}
										}
										catch (Exception exception1)
										{
											var exception = exception1;
											LogErrorOnWorkflowItemsCopy(subFolder.Name, subFolder.DisplayUrl, sPList.DisplayUrl, exception, null);
										}
									}
								}
								catch (Exception exception3)
								{
									var exception2 = exception3;
									LogErrorOnWorkflowItemsCopy(listByTitle.Title, listByTitle.DisplayUrl, sPList.DisplayUrl, exception2, null);
								}
							}
						}
					}
				}
			}
			catch (Exception exception5)
			{
				var exception4 = exception5;
				LogErrorOnWorkflowItemsCopy(sourceList.Title, sourceList.DisplayUrl, targetWeb.DisplayUrl, exception4, null);
			}
			return flag;
		}

		private void CopyWorkflowsTaskDelegate(object[] oParams)
		{
			var sPList = oParams[0] as SPList;
			var sPWeb = oParams[1] as SPWeb;
			if (sPList == null || sPWeb == null)
			{
				return;
			}
			try
			{
				var flag = true;
				if (_isCopyListAction && SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations)
				{
					flag = CopyWorkflowsListItems(sPList, sPWeb);
				}
				if (IsListWorkflowMigration(flag) || IsContentTypeWorkflowMigration())
				{
					var copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
					copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(SharePointOptions);
					var lists = sPWeb.Lists;
					var item = GuidMappings[new Guid(sPList.ID)];
					var listByGuid = lists.GetListByGuid(item.ToString());
					if (listByGuid == null)
					{
						var logItem = new LogItem("Copying Workflow Associations", sPList.DisplayUrl, sPList.ParentWeb.DisplayUrl, sPWeb.DisplayUrl, ActionOperationStatus.Warning);
						FireOperationStarted(logItem);
						logItem.Information = string.Format("Workflow association for source list '{0}' cannot be copied as target list is not available", sPList.DisplayUrl);
						FireOperationFinished(logItem);
					}
					else
					{
						if (IsListWorkflowMigration(flag))
						{
							SubActions.Add(copyWorkflowAssociationsAction);
							var objArray = new object[] { sPList, listByGuid, sPWeb };
							copyWorkflowAssociationsAction.RunAsSubAction(objArray, new ActionContext(sPList.ParentWeb, sPWeb), null);
						}
						if (IsContentTypeWorkflowMigration())
						{
							foreach (SPContentType contentType in sPList.ContentTypes)
							{
								var contentTypeByName = listByGuid.ContentTypes.GetContentTypeByName(contentType.Name);
								if (contentTypeByName == null)
								{
									var logItem1 = new LogItem("Copying Content Type Workflows", contentType.Name, sPList.DisplayUrl, sPWeb.DisplayUrl, ActionOperationStatus.Warning);
									FireOperationStarted(logItem1);
									logItem1.Information = string.Format("Content Type Workflow association for source list '{0}' cannot be copied as target content type is not available", sPList.DisplayUrl);
									FireOperationFinished(logItem1);
								}
								else
								{
									SubActions.Add(copyWorkflowAssociationsAction);
									var objArray1 = new object[] { contentType, contentTypeByName, listByGuid };
									copyWorkflowAssociationsAction.RunAsSubAction(objArray1, new ActionContext(sPList, sPWeb), null);
								}
							}
						}
					}
				}
			}
			finally
			{
				sPList.Dispose();
			}
		}

		private void DeleteList(ref SPList listToDelete)
		{
			if (listToDelete == null)
			{
				return;
			}
			var name = listToDelete.Name;
			var parentWeb = listToDelete.ParentWeb;
			var flag = (parentWeb.Template.ID != 9 ? false : parentWeb.Template.Name.Equals("BLOG#0", StringComparison.OrdinalIgnoreCase));
			var flag1 = false;
			if (!flag || listToDelete.BaseTemplate != ListTemplateType.PictureLibrary)
			{
				try
				{
					try
					{
						if (parentWeb.Lists.DeleteList(listToDelete.ID))
						{
							listToDelete = null;
						}
					}
					catch (Exception exception1)
					{
						var exception = exception1;
						if (exception.Message.IndexOf("does not exist", StringComparison.OrdinalIgnoreCase) != -1 || exception.Message.IndexOf("may have been deleted", StringComparison.OrdinalIgnoreCase) != -1)
						{
							flag1 = true;
						}
					}
				}
				finally
				{
					if (listToDelete != null)
					{
						if (flag1)
						{
							parentWeb.Lists.FetchData();
						}
						listToDelete = MigrationUtils.GetMatchingList(listToDelete, parentWeb);
						if (flag1 && listToDelete != null && parentWeb.Lists.DeleteList(listToDelete.ID))
						{
							listToDelete = null;
						}
					}
				}
			}
			if (listToDelete != null && (!parentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater || !parentWeb.IsPublishingTemplate || listToDelete.Name != "SiteCollectionImages" && listToDelete.Name != "Style Library" && listToDelete.Name != "Relationships List" && !IsListWithPreserveIdOverClientOM(listToDelete)))
			{
				try
				{
					if (!MigrationUtils.IsListWithDefaultItems(listToDelete))
					{
						listToDelete.Items.DeleteAllItems();
					}
				}
				catch
				{
				}
			}
		}

		private void DisposeListsTaskDelegate(object[] oParams)
		{
			var sPList = oParams[0] as SPList;
			var sPList1 = oParams[1] as SPList;
			sPList1.UpdateCurrentNode();
			sPList.Dispose();
			sPList1.Dispose();
		}

		private void ExtractViewColumnsFromListXml(string sTargetListXml, SPList addedContainer)
		{
			var xmlNode = XmlUtility.StringToXmlNode(sTargetListXml);
			if (!WorkflowViewMappings.ContainsKey(addedContainer.ID))
			{
				var strs = new Dictionary<string, string>();
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//View"))
				{
					foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("./ViewFields/FieldRef"))
					{
						var xmlNodes2 = xmlNode.SelectSingleNode(string.Concat("./Fields/Field[@Name='", xmlNodes1.Attributes["Name"].Value, "']"));
						if (xmlNodes2 == null || xmlNodes2.Attributes["Type"] == null || !(xmlNodes2.Attributes["Type"].Value == "WorkflowStatus"))
						{
							continue;
						}
						if (!strs.ContainsKey(xmlNodes1.Attributes["Name"].Value))
						{
							strs.Add(xmlNodes1.Attributes["Name"].Value, xmlNodes.Attributes["DisplayName"].Value);
						}
						else
						{
							var strs1 = strs;
							var strs2 = strs1;
							var value = xmlNodes1.Attributes["Name"].Value;
							var str = value;
							strs1[value] = string.Concat(strs2[str], ";", xmlNodes.Attributes["DisplayName"].Value);
						}
					}
				}
				if (strs.Count > 0)
				{
					WorkflowViewMappings.Add(addedContainer.ID, strs);
				}
			}
		}

		private byte[] GetLibraryDocumentTemplate(SPList sourceList)
		{
			string value;
			string str;
			byte[] document = null;
			if (sourceList.IsDocumentLibrary)
			{
				var xmlNode = XmlUtility.StringToXmlNode(sourceList.XML);
				var itemOf = xmlNode.Attributes["DocTemplateUrl"] != null;
				var flag = xmlNode.Attributes["DocTemplateId"] != null;
				if (itemOf || flag)
				{
					if (flag)
					{
						value = xmlNode.Attributes["DocTemplateId"].Value;
					}
					else
					{
						value = null;
					}
					var str1 = value;
					if (itemOf)
					{
						str = xmlNode.Attributes["DocTemplateUrl"].Value;
					}
					else
					{
						str = null;
					}
					var str2 = str;
					string str3 = null;
					string str4 = null;
					if (!string.IsNullOrEmpty(str2))
					{
						Utils.ParseUrlForLeafName(str2, out str3, out str4);
						document = sourceList.Adapter.Reader.GetDocument(str1, str3, str4, 1);
					}
				}
			}
			return document;
		}

		private List<string> GetSourceLanguages(LogItem operation, string sourceWebXml, SPWeb targetWeb)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sourceWebXml);
			var xmlNodes = xmlDocument.SelectSingleNode("//LanguageResources");
			if (xmlNodes != null && xmlNodes.ChildNodes.Count > 0)
			{
				var strs = new List<string>();
				foreach (XmlNode childNode in xmlNodes.ChildNodes)
				{
					var attributeValueAsString = childNode.GetAttributeValueAsString("Culture");
					if ((new CultureInfo(attributeValueAsString)).LCID == targetWeb.Language)
					{
						continue;
					}
					strs.Add(attributeValueAsString);
				}
				var targetLanguages = GetTargetLanguages(operation, targetWeb);
				if (targetLanguages != null && targetLanguages.Count > 0)
				{
					return strs.Intersect<string>(targetLanguages).ToList<string>();
				}
			}
			return null;
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			var supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(listTransformerDefinition);
			return supportedDefinitions;
		}

		private List<string> GetTargetLanguages(LogItem operation, SPWeb targetWeb)
		{
			try
			{
				var str = targetWeb.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.GetSupportedWebCultures.ToString(), string.Empty);
				var operationReportingResult = new OperationReportingResult(str);
				if (operationReportingResult.ErrorOccured)
				{
					var logItem = operation;
					logItem.Information = string.Concat(logItem.Information, operationReportingResult.GetMessageOfFirstErrorElement);
					operation.Details = operationReportingResult.AllReportElementsAsString;
				}
				else if (!string.IsNullOrEmpty(operationReportingResult.ObjectXml))
				{
					var objectXml = operationReportingResult.ObjectXml;
					var chrArray = new char[] { ',' };
					var list = objectXml.Split(chrArray).Select<string, int>(int.Parse).ToList<int>();
					var strs = new List<string>();
					foreach (var num in list)
					{
						strs.Add(CultureInfo.GetCultureInfo(num).Name);
					}
					return strs;
				}
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				var logItem1 = operation;
				logItem1.Details = string.Concat(logItem1.Details, string.Format("Error occurred while loading target languages. Error '{0}'", exception.ToString()));
			}
			return null;
		}

		private bool Is2003IncrementalCopySpecialCase(SPList sourceList, SPWeb targetWeb, out SPList targetList)
		{
			var flag = false;
			targetList = null;
			var expressionString = SharePointOptions.ListFilterExpression.GetExpressionString();
			if (sourceList.Adapter.SharePointVersion.IsSharePoint2003 && 0 <= expressionString.IndexOf("modified", StringComparison.OrdinalIgnoreCase))
			{
				var renamedListName = MigrationUtils.GetRenamedListName(sourceList, targetWeb, SharePointOptions.TaskCollection);
				if (renamedListName != null)
				{
					targetList = targetWeb.Lists[renamedListName];
				}
				if (targetList != null)
				{
					flag = true;
				}
			}
			return flag;
		}

		private bool IsAllowMultipleResponseSettingEnabled(string targetListXML)
		{
			var attributeValueAsBoolean = false;
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(targetListXML);
			if (xmlDocument.DocumentElement.Attributes["AllowMultiResponses"] != null)
			{
				attributeValueAsBoolean = xmlDocument.DocumentElement.GetAttributeValueAsBoolean("AllowMultiResponses");
			}
			return attributeValueAsBoolean;
		}

		private bool IsContentTypeWorkflowMigration()
		{
			if (!_isCopyListAction)
			{
				return false;
			}
			if (SharePointOptions.CopyContentTypeOOBWorkflowAssociations)
			{
				return true;
			}
			return SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations;
		}

		private bool IsListWithPreserveIdOverClientOM(SPList list)
		{
			if (list.IsDocumentLibrary || !list.Adapter.IsClientOM)
			{
				return false;
			}
			return ActionOptions.PreserveItemIDs;
		}

		private bool IsListWorkflowMigration(bool shouldCopyworkflow)
		{
			if (shouldCopyworkflow && SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations)
			{
				return true;
			}
			return SharePointOptions.CopyListOOBWorkflowAssociations;
		}

		private bool IsSameInternalName(string listColumnInternalName, string workflowInternalName)
		{
			var flag = workflowInternalName.Equals(listColumnInternalName, StringComparison.OrdinalIgnoreCase);
			if (!flag)
			{
				flag = workflowInternalName.Equals(string.Concat("OData_", listColumnInternalName), StringComparison.OrdinalIgnoreCase);
			}
			return flag;
		}

		private bool IsWorkflowInstanceIDBackupFieldExists(SPList sourceList)
		{
			bool flag;
			var enumerator = ((SPFieldCollection)sourceList.Fields).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!((SPField)enumerator.Current).Name.Equals("WorkflowInstanceIDBackup", StringComparison.InvariantCultureIgnoreCase))
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
				var disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		private bool ListAllowsContentTypes(SPList list)
		{
			var allowContentTypes = list.AllowContentTypes;
			if (allowContentTypes.HasValue)
			{
				return allowContentTypes.Value;
			}
			return (list.BaseTemplate == ListTemplateType.Survey || list.BaseTemplate == ListTemplateType.MeetingAgenda || list.BaseTemplate == ListTemplateType.MeetingAttendees || list.BaseTemplate == ListTemplateType.MeetingDecisions || list.BaseTemplate == ListTemplateType.MeetingObjectives || list.BaseTemplate == ListTemplateType.MeetingDecisions ? false : list.BaseTemplate != ListTemplateType.MeetingThingsToBring);
		}

		private void LogErrorOnWorkflowItemsCopy(string sourceObjectName, string sourceObjectDisplayUrl, string targetObjectDisplayUrl, Exception ex, string information = null)
		{
			var logItem = new LogItem("Copying Workflow Items", sourceObjectName, sourceObjectDisplayUrl, targetObjectDisplayUrl, ActionOperationStatus.Failed);
			FireOperationStarted(logItem);
			if (ex == null)
			{
				logItem.Information = information;
				logItem.Status = ActionOperationStatus.Warning;
			}
			else
			{
				logItem.Exception = ex;
				logItem.Details = ex.StackTrace;
			}
			FireOperationFinished(logItem);
		}

		private void LogFieldsFailureInJobLogs(SPList sourceList, SPWeb targetWeb, string message)
		{
			var logItem = new LogItem("Copying List Fields", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Failed);
			FireOperationStarted(logItem);
			logItem.Information = Adapters.Properties.Resources.PleaseReviewDetails;
			logItem.Details = message;
			FireOperationFinished(logItem);
		}

		private string MapColumnDefaultSettings(string metadataDefaultSettings, SPList sourceList)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(metadataDefaultSettings);
			var xmlNodeLists = xmlDocument.SelectNodes("/Result/ObjectXml/MetadataDefaults/a");
			if (xmlNodeLists == null || xmlNodeLists.Count <= 0)
			{
				return null;
			}
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				if (xmlNodes.ChildNodes == null || xmlNodes.ChildNodes.Count <= 0)
				{
					continue;
				}
				foreach (XmlNode childNode in xmlNodes.ChildNodes)
				{
					if (!SharePointOptions.ApplyNewContentTypes || SharePointOptions.ContentTypeApplicationObjects == null)
					{
						continue;
					}
					foreach (var contentTypeApplicationObject in SharePointOptions.ContentTypeApplicationObjects)
					{
						if (!contentTypeApplicationObject.AppliesTo(sourceList) || contentTypeApplicationObject.ColumnMappings == null || !contentTypeApplicationObject.ColumnMappings.ContainsColumnChanges || contentTypeApplicationObject.ColumnMappings.Count <= 0)
						{
							continue;
						}
						var attributeValueAsString = childNode.GetAttributeValueAsString("FieldName");
						if (string.IsNullOrEmpty(attributeValueAsString) || contentTypeApplicationObject.ColumnMappings[attributeValueAsString] == null)
						{
							continue;
						}
						var target = contentTypeApplicationObject.ColumnMappings[attributeValueAsString].Target.Target;
						childNode.Attributes["FieldName"].Value = target;
					}
				}
				if (xmlNodes.Attributes["href"] == null)
				{
					continue;
				}
				var str = LinkCorrector.CorrectUrl(xmlNodes.GetAttributeValueAsString("href"));
				if (string.IsNullOrEmpty(str))
				{
					continue;
				}
				xmlNodes.Attributes["href"].Value = str;
			}
			return xmlDocument.InnerXml;
		}

		private string MapListTemplate(string listXml, string sourceListUrl, SPWeb targetWeb)
		{
			var empty = string.Empty;
			var attributeValueAsString = string.Empty;
			var outerXml = listXml;
			var listTemplateType = ListTemplateType.Empty;
			try
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(listXml);
				empty = xmlDocument.FirstChild.GetAttributeValueAsString("Title");
				attributeValueAsString = xmlDocument.FirstChild.GetAttributeValueAsString("BaseTemplate");
				if (!string.IsNullOrEmpty(attributeValueAsString))
				{
					foreach (SPListTemplate listTemplate in targetWeb.ListTemplates)
					{
						if (!listTemplate.TemplateType.ToString().Equals(attributeValueAsString))
						{
							continue;
						}
						return listXml;
					}
				}
				var str = xmlDocument.FirstChild.GetAttributeValueAsString("BaseType");
				if (!string.IsNullOrEmpty(str))
				{
					switch (Convert.ToInt32(str))
					{
						case 0:
						{
							listTemplateType = ListTemplateType.CustomList;
							goto case 2;
						}
						case 1:
						{
							listTemplateType = ListTemplateType.DocumentLibrary;
							goto case 2;
						}
						case 2:
						{
							if (listTemplateType == ListTemplateType.Empty)
							{
								break;
							}
							xmlDocument.FirstChild.Attributes["BaseTemplate"].Value = Convert.ToString((int)listTemplateType);
							if (xmlDocument.FirstChild.Attributes["FeatureId"] != null)
							{
								xmlDocument.FirstChild.Attributes["FeatureId"].Value = targetWeb.ListTemplates.GetByType(listTemplateType).FeatureId;
							}
							outerXml = xmlDocument.OuterXml;
							var logItem = new LogItem(Properties.Resources.MappingListTemplate, empty, sourceListUrl, targetWeb.DisplayUrl, ActionOperationStatus.Completed)
							{
								Status = ActionOperationStatus.Warning,
								Information = string.Format(Properties.Resources.MappingListTemplateInfoMessage, attributeValueAsString, listTemplateType)
							};
							FireOperationStarted(logItem);
							FireOperationFinished(logItem);
							break;
						}
						case 3:
						{
							listTemplateType = ListTemplateType.DiscussionBoard;
							goto case 2;
						}
						case 4:
						{
							listTemplateType = ListTemplateType.Survey;
							goto case 2;
						}
						case 5:
						{
							listTemplateType = ListTemplateType.Issues;
							goto case 2;
						}
						default:
						{
							goto case 2;
						}
					}
				}
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				var logItem1 = new LogItem(string.Format(Properties.Resources.MappingListTemplateExceptionMessage, attributeValueAsString, listTemplateType), empty, sourceListUrl, targetWeb.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				FireOperationStarted(logItem1);
				FireOperationFinished(logItem1);
			}
			return outerXml;
		}

		private void Modify2013WorkflowXml(ref string targetXml, List<SP2013WorkflowSubscription> workflowSubscriptions)
		{
			var xmlNode = XmlUtility.StringToXmlNode(targetXml);
			var xmlNodes = xmlNode.SelectSingleNode("./Fields");
			var xmlNodes1 = new List<XmlNode>();
			foreach (var workflowSubscription in workflowSubscriptions)
			{
				foreach (XmlNode childNode in xmlNodes.ChildNodes)
				{
					var itemOf = childNode.Attributes["StaticName"];
					if (itemOf == null || !IsSameInternalName(itemOf.Value, workflowSubscription.StatusFieldName))
					{
						continue;
					}
					xmlNodes1.Add(childNode);
				}
			}
			foreach (var xmlNode1 in xmlNodes1)
			{
				xmlNodes.RemoveChild(xmlNode1);
			}
			targetXml = xmlNode.OuterXml;
		}

		private void ModifyTaskListXml(ref string sTargetXml, SPList sourceList)
		{
			var xmlNode = XmlUtility.StringToXmlNode(sTargetXml);
			var xmlNodes = xmlNode.SelectSingleNode("./Fields");
			foreach (SPField field in (SPFieldCollection)sourceList.Fields)
			{
				XmlNode str = null;
				if (field.Name.Equals("WorkflowInstanceID", StringComparison.InvariantCultureIgnoreCase) && IsWorkflowInstanceIDBackupFieldExists(sourceList))
				{
					str = field.FieldXML.Clone();
				}
				if (str == null)
				{
					continue;
				}
				if (str.Attributes["DisplayName"] == null)
				{
					var xmlAttribute = str.OwnerDocument.CreateAttribute("DisplayName");
					xmlAttribute.Value = "WorkflowInstanceIDBackup";
					str.Attributes.Append(xmlAttribute);
				}
				else
				{
					str.Attributes["DisplayName"].Value = "WorkflowInstanceIDBackup";
				}
				if (str.Attributes["Name"] == null)
				{
					var xmlAttribute1 = str.OwnerDocument.CreateAttribute("Name");
					xmlAttribute1.Value = "WorkflowInstanceIDBackup";
					str.Attributes.Append(xmlAttribute1);
				}
				else
				{
					str.Attributes["Name"].Value = "WorkflowInstanceIDBackup";
				}
				if (str.Attributes["StaticName"] == null)
				{
					var xmlAttribute2 = str.OwnerDocument.CreateAttribute("StaticName");
					xmlAttribute2.Value = "WorkflowInstanceIDBackup";
					str.Attributes.Append(xmlAttribute2);
				}
				else
				{
					str.Attributes["StaticName"].Value = "WorkflowInstanceIDBackup";
				}
				if (str.Attributes["Type"] != null)
				{
					str.Attributes["Type"].Value = "Text";
				}
				if (str.Attributes["ID"] != null)
				{
					str.Attributes["ID"].Value = Guid.NewGuid().ToString();
				}
				ColumnMappings.RemoveAttribute(str, "ReadOnly");
				ColumnMappings.AppendNodeToTarget(str, xmlNodes);
				sTargetXml = xmlNode.OuterXml;
			}
		}

		private void OverlayCalendarLinkCorrections(object[] parameters)
		{
			LogItem logItem = null;
			SPView sPView = null;
			SPView sPView1 = null;
			var flag = true;
			try
			{
				try
				{
					if (parameters == null || (int)parameters.Length != 3)
					{
						throw new Exception("Cannot correct calendar overlay definition. Incorrect number of parameters.");
					}
					sPView = parameters[0] as SPView;
					sPView1 = parameters[1] as SPView;
					flag = (parameters[2] is bool ? (bool)parameters[2] : true);
					if (sPView == null || sPView1 == null)
					{
						throw new Exception("Cannot correct calendar overlay definition. Parameters did not contain view definition.");
					}
					logItem = new LogItem("Correcting calendar overlay definition", sPView1.DisplayName, sPView.ParentList.Url, sPView1.ParentList.Url, ActionOperationStatus.Running);
					FireOperationStarted(logItem);
					if (!sPView1.ParentList.Adapter.AdapterShortName.Equals("NW", StringComparison.OrdinalIgnoreCase))
					{
						var sPView2 = sPView.Clone();
						var flag1 = false;
						foreach (var type in sPView2.CalendarSettings.GetTypes<SharePointReferencedCalendar>())
						{
							var str = LinkCorrector.CorrectUrl(type.CalendarUrl);
							var str1 = LinkCorrector.CorrectUrl(type.WebUrl);
							var str2 = LinkCorrector.CorrectUrl(type.ListFormUrl);
							var listID = type.ListID;
							var str3 = LinkCorrector.MapGuid(listID);
							var viewID = type.ViewID;
							var str4 = LinkCorrector.MapGuid(viewID);
							if (string.IsNullOrEmpty(str3) || str3.Equals(listID, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(str4) || str3.Equals(viewID, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str1))
							{
								var logItem1 = new LogItem("Correcting SharePoint calendar overlay definition", sPView1.DisplayName, sPView.ParentList.Url, sPView1.ParentList.Url, ActionOperationStatus.Running);
								FireOperationStarted(logItem1);
								logItem1.Status = ActionOperationStatus.Warning;
								logItem1.Information = string.Format("Unable to add/correct '{0}' SharePoint Overlay. This is an expected behavior as the referenced calendars are outside the migration scope.", type.Name);
								FireOperationFinished(logItem1);
							}
							else
							{
								type.CalendarUrl = str;
								type.WebUrl = str1;
								type.ListFormUrl = str2;
								type.ListID = (new Guid(str3)).ToString("B");
								type.ViewID = (new Guid(str4)).ToString("B");
								flag1 = true;
							}
						}
						var flag2 = false;
						if (sPView1.ParentList.Adapter.IsClientOM && sPView2.CalendarSettings.GetTypes<ExchangeReferencedCalendar>().Count > 0)
						{
							flag2 = true;
						}
						if (flag1 || flag2)
						{
							var outerXml = sPView2.XML.OuterXml;
							outerXml = UpdatingViewXML(sPView1, outerXml);
							sPView1.ParentList.Views.AddOrUpdateView(outerXml);
							logItem.Status = ActionOperationStatus.Completed;
						}
						else
						{
							logItem.Status = ActionOperationStatus.Warning;
							logItem.Information = "Unable to correct overlay definitions. This is an expected behavior if the referenced calendars are outside the migration scope.";
						}
					}
					else
					{
						logItem.Status = ActionOperationStatus.Warning;
						logItem.Information = "Unable to correct overlay definitions because the current target adapter doesn't support it.";
						return;
					}
				}
				catch (Exception exception1)
				{
					var exception = exception1;
					if (logItem == null)
					{
						logItem = new LogItem("Correcting calendar overlay definition", "", "", "", ActionOperationStatus.Failed);
						FireOperationStarted(logItem);
					}
					logItem.Exception = exception;
				}
			}
			finally
			{
				if (flag)
				{
					if (sPView != null && sPView.ParentList != null)
					{
						sPView.ParentList.Dispose();
					}
					if (sPView1 != null && sPView1.ParentList != null)
					{
						sPView1.ParentList.Dispose();
					}
				}
				FireOperationFinished(logItem);
			}
		}

		private string ProcessingUserColumn(SPList sourceList, SPWeb targetWeb, string sTargetXml)
		{
			var logItem = new LogItem("Processing User Column", sourceList.Name, sourceList.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
			try
			{
				var xmlNode = XmlUtility.StringToXmlNode(sTargetXml);
				var xmlNodes = xmlNode.SelectSingleNode("./Fields");
				var str = MigrationUtils.ProcessUserFieldsForCopy(xmlNodes, sourceList.ParentWeb, targetWeb);
				if (!string.IsNullOrEmpty(str))
				{
					FireOperationStarted(logItem);
					logItem.Information = Adapters.Properties.Resources.PleaseReviewDetails;
					logItem.Details = str;
					logItem.Status = ActionOperationStatus.Warning;
					FireOperationFinished(logItem);
				}
				sTargetXml = xmlNode.OuterXml;
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				FireOperationStarted(logItem);
				logItem.Status = ActionOperationStatus.Failed;
				logItem.Exception = exception;
				FireOperationFinished(logItem);
			}
			return sTargetXml;
		}

		private void ProcessReportLibraryFieldsForCopy(XmlNode fieldsNode)
		{
			foreach (var str in new List<string>()
			{
				"ReportName",
				"ReportLink",
				"ReportLinkFilename",
				"ReportCreatedDisplay",
				"ReportCreatedByDisplay",
				"ReportModifiedDisplay",
				"ReportModifiedByDisplay"
			})
			{
				var str1 = string.Concat("//Field[@Name='", str, "']");
				var xmlNodes = fieldsNode.SelectSingleNode(str1);
				if (xmlNodes == null)
				{
					continue;
				}
				var str2 = (xmlNodes.Attributes["Group"] != null ? xmlNodes.Attributes["Group"].Value : string.Empty);
				var str3 = (xmlNodes.Attributes["SourceID"] != null ? xmlNodes.Attributes["SourceID"].Value : string.Empty);
				var flag = false;
				if (!string.IsNullOrEmpty(str3))
				{
					flag = Utils.IsGUID(str3);
				}
				if (!(str2 == "_Hidden") || flag)
				{
					continue;
				}
				fieldsNode.RemoveChild(xmlNodes);
			}
		}

		protected void QueueOverlayCalendarLinkCorrections(SPList sourceList, SPList targetList)
		{
			var sPViews = new List<SPView>();
			foreach (SPView view in sourceList.Views)
			{
				if (view.Type != ViewType.Calendar || view.CalendarSettings.GetTypes<SharePointReferencedCalendar>().Count <= 0 && (!targetList.Adapter.IsClientOM || view.CalendarSettings.GetTypes<ExchangeReferencedCalendar>().Count <= 0))
				{
					continue;
				}
				sPViews.Add(view);
			}
			for (var i = 0; i < sPViews.Count; i++)
			{
				var item = sPViews[i];
				var threadManager = ThreadManager;
				var viewByDisplayName = new object[] { item, targetList.Views.GetViewByDisplayName(item.DisplayName), i == sPViews.Count - 1 };
				threadManager.QueueBufferedTask("CalendarOverlayLinkCorrection", viewByDisplayName, OverlayCalendarLinkCorrections);
			}
		}

		private void ResetTargetLanguage(SPWeb targetWeb, string originalLanguageOrder, LogItem copyLanguageResourcesOperation, bool isCopyLanuageResourcesForViews)
		{
			try
			{
				if (isCopyLanuageResourcesForViews && targetWeb.Adapter.SharePointVersion.IsSharePointOnline)
				{
					var str = string.Format("{0}#{1}#{2}", originalLanguageOrder, AdapterConfigurationVariables.LanguageSettingsMaximumInterval, AdapterConfigurationVariables.LanguageSettingsRefreshInterval);
					targetWeb.Adapter.Writer.ExecuteCommand(SharePointAdapterCommands.SetCurrentUserLanguage.ToString(), str);
				}
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				if (copyLanguageResourcesOperation != null)
				{
					copyLanguageResourcesOperation.Exception = exception;
					copyLanguageResourcesOperation.Status = ActionOperationStatus.Failed;
					var logItem = copyLanguageResourcesOperation;
					logItem.Details = string.Concat(logItem.Details, exception.StackTrace);
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			_isCopyListAction = true;
			foreach (SPWeb sPWeb in target)
			{
				var parserEnabled = true;
				try
				{
					if (SharePointOptions.DisableDocumentParsing)
					{
						parserEnabled = sPWeb.ParserEnabled;
						if (parserEnabled)
						{
							sPWeb.SetDocumentParsing(false);
						}
					}
					var nodeArray = new Node[] { sPWeb };
					InitializeSharePointCopy(source, new NodeCollection(nodeArray), SharePointOptions.ForceRefresh);
					SharePointOptions.CopyWebPartsAtItemsLevel = source.Count <= 1;
					WebPartPagesNotCopiedAtItemsLevel.Clear();
					var taskDefinitions = new List<TaskDefinition>();
					foreach (SPList sPList in source)
					{
						var flag = false;
						TaskDefinition taskDefinition = null;
						var str = string.Concat("MultiselectListContent", sPList.ID);
						try
						{
							try
							{
								listTransformerDefinition.BeginTransformation(this, sPList.ParentWeb.Lists, sPWeb.Lists, Options.Transformers);
								if (!CheckForAbort())
								{
									InitializeAudienceMappings(sPList, sPWeb);
									taskDefinition = CopyList(sPList, sPWeb, true, false, false);
									taskDefinitions.Add(taskDefinition);
									flag = true;
								}
								else
								{
									break;
								}
							}
							catch (Exception exception1)
							{
								var exception = exception1;
								var logItem = new LogItem("Error copying list", "", sPList.DisplayUrl, sPWeb.DisplayUrl, ActionOperationStatus.Failed)
								{
									Exception = exception
								};
								FireOperationStarted(logItem);
								FireOperationFinished(logItem);
								sPList.Dispose();
							}
						}
						finally
						{
							if (flag)
							{
								listTransformerDefinition.EndTransformation(this, sPList.ParentWeb.Lists, sPWeb.Lists, Options.Transformers);
							}
						}
						if (!flag)
						{
							continue;
						}
						var threadManager = ThreadManager;
						var str1 = str;
						var objArray = new object[] { sPList, sPWeb };
						threadManager.QueueBufferedTask(str1, objArray, CopyWorkflowsTaskDelegate);
						ThreadManager.QueueTask(null, (object[] oParams) => {
							ThreadManager.WaitForTask(taskDefinition);
							ThreadManager.SetBufferedTasks(str, false, false);
						});
					}
					ThreadManager.WaitForTasks(taskDefinitions);
					ThreadManager.SetBufferedTasks(PermissionsKeyFormatter.GetKeyFor(sPWeb), false, false);
					PasteActionUtils.CopyItemLevelWebParts(this, SharePointOptions, WebPartPagesNotCopiedAtItemsLevel);
					ThreadManager.SetBufferedTasks(GetWebPartCopyBufferKey(sPWeb), false, false);
					var parentWeb = (source[0] as SPList).ParentWeb;
					if (AdapterConfigurationVariables.MigrateLanguageSettings && AdapterConfigurationVariables.MigrateLanguageSettingForViews && parentWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && !parentWeb.Adapter.IsDB && !parentWeb.Adapter.IsNws && !parentWeb.Adapter.IsClientOM && sPWeb.Adapter.SharePointVersion.IsSharePoint2016OrLater && sPWeb.Adapter.IsClientOM)
					{
						CopyLanguageResourcesForView(parentWeb, sPWeb);
					}
				}
				finally
				{
					if (SharePointOptions.DisableDocumentParsing && parserEnabled)
					{
						sPWeb.SetDocumentParsing(true);
					}
					sPWeb.Dispose();
				}
			}
			StartCommonWorkflowBufferedTasks();
			ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
			ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}

		private bool ShouldPreserveAssetList(SPList sourceList, SPWeb targetWeb, SPList existingTargetList)
		{
			if (!sourceList.Adapter.IsDB && sourceList.Adapter.SharePointVersion.IsSharePoint2013OrLater && sourceList.ParentWeb.HasOneNote2010NotebookFeature && targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater && targetWeb.HasOneNote2010NotebookFeature)
			{
				if ((sourceList.IsDocumentLibrary && sourceList.Name.Equals("SiteAssets")) && existingTargetList != null)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateAllowMultipleResponseSetting(SPList targetList, string isMultipleResponsesAllowed)
		{
			var stringBuilder = new StringBuilder();
			using (TextWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (var xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.WriteStartElement("List");
					xmlTextWriter.WriteAttributeString("AllowMultiResponses", isMultipleResponsesAllowed);
					xmlTextWriter.WriteEndElement();
				}
			}
			targetList.UpdateList(stringBuilder.ToString(), null, false, false, false, null);
		}

		private void UpdateContentTypeIdsInListViews(SPList sourceList, SPList targetList)
		{
			var flag = false;
			foreach (SPView view in sourceList.Views)
			{
				if (view.ContentTypeId == null || view.ContentTypeId.Equals("0x", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				flag = true;
				break;
			}
			if (!flag)
			{
				return;
			}
			var strs = new Dictionary<string, string>();
			foreach (SPContentType contentType in sourceList.ContentTypes)
			{
				var contentTypeByName = targetList.ContentTypes.GetContentTypeByName(contentType.Name);
				if (contentTypeByName == null || strs.ContainsKey(contentType.ContentTypeID))
				{
					continue;
				}
				strs.Add(contentType.ContentTypeID, contentTypeByName.ContentTypeID);
			}
			var strs1 = new Dictionary<string, string>();
			foreach (SPView sPView in sourceList.Views)
			{
				if (sPView.ContentTypeId == null || sPView.ContentTypeId.Equals("0x", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				var contentTypeId = sPView.ContentTypeId;
				if (!strs.ContainsKey(contentTypeId) || strs1.ContainsKey(sPView.DisplayName))
				{
					continue;
				}
				strs1.Add(sPView.DisplayName, strs[contentTypeId]);
			}
			var xmlNodes = new List<XmlNode>();
			foreach (SPView view1 in targetList.Views)
			{
				xmlNodes.Add(view1.XML);
			}
			foreach (var xmlNode in xmlNodes)
			{
				var attributeValue = XmlUtility.GetAttributeValue(xmlNode, "DisplayName");
				if (attributeValue == null || !strs1.ContainsKey(attributeValue) || XmlUtility.GetAttributeValue(xmlNode, "ContentTypeID") == null)
				{
					continue;
				}
				xmlNode.Attributes["ContentTypeID"].Value = strs1[attributeValue];
				targetList.Views.AddOrUpdateView(xmlNode.OuterXml);
			}
		}

		private static string UpdatingViewXML(SPView targetView, string viewXML)
		{
			if (targetView.ParentList.Adapter.IsClientOM)
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(viewXML);
				var xmlNodes = xmlDocument.SelectSingleNode(".//View");
				var str = xmlDocument.CreateAttribute("CopyCalendarOverlays");
				str.Value = true.ToString();
				xmlNodes.Attributes.Append(str);
				viewXML = xmlNodes.OuterXml;
			}
			return viewXML;
		}
	}
}