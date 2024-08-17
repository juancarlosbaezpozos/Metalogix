using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Common;
using Metalogix.SharePoint.Common.Enums;
using Metalogix.SharePoint.Common.Workflow2013;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.Workflow2013;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Name("Paste Workflow Associations")]
	[ShowInMenus(false)]
	public class CopyWorkflowAssociationsAction : PasteAction<CopyWorkflowAssociationOptions>
	{
		private const string _TERMINATEWORKFLOW = "TerminateWorkflow";

		private const string _STATUSCOLUMNNAME = "StatusColumnName";

		private const string _ADDSTATUSFIELDTOVIEWS = "AddStatusFieldToViews";

		private const string _STATUSCOLUMNTEXTNAME = "StatusColumnTextName";

		private const string STATUS_COLUMN_INTERNAL_NAME = "StatusColumnInternalName";

		public CopyWorkflowAssociationsAction()
		{
		}

		private void ActivateReusableWorkflowTemplates(SPWeb targetWeb)
		{
			LogItem logItem = null;
			try
			{
				logItem = new LogItem("Activating all reusable workflow definitions", "", "", targetWeb.Url, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				targetWeb.Adapter.Writer.ActivateReusableWorkflowTemplates();
				if (base.SharePointOptions.Verbose)
				{
					logItem.SourceContent = targetWeb.XML;
					logItem.TargetContent = "";
				}
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				logItem.SourceContent = targetWeb.XML;
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}

		private void AddNewWorkflowAssociationBaseIDToGuidMapper(string sOldWorkflowID, string sNewWorkflowAssociationXML)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sNewWorkflowAssociationXML);
			if (xmlNode != null && xmlNode.Attributes["BaseId"] != null && !base.WorkflowMappings.ContainsKey(sOldWorkflowID))
			{
				base.WorkflowMappings.Add(sOldWorkflowID, xmlNode.Attributes["BaseId"].Value);
			}
		}

		private void AddNewWorkflowIDToGuidMapper(string sOldWorkflowID, string sNewWorkflowXML)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sNewWorkflowXML);
			if (xmlNode.Attributes["Id"] != null && !base.WorkflowMappings.ContainsKey(sOldWorkflowID))
			{
				base.WorkflowMappings.Add(sOldWorkflowID, xmlNode.Attributes["Id"].Value);
			}
		}

		private void CopyNintexWorkflowDatabaseEntries(SPWeb sourceNode, SPWeb targetNode, SPWorkflowAssociation wfa, List<string> workflowInstanceIDList)
		{
			if (sourceNode.GetExternalConnectionsOfType<NintexExternalConnection>(true).Count < 1 || targetNode.GetExternalConnectionsOfType<NintexExternalConnection>(true).Count < 1)
			{
				return;
			}
			LogItem logItem = null;
			try
			{
				logItem = new LogItem("Scanning for Nintex database entries...", "", sourceNode.DisplayUrl, targetNode.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				NintexExternalConnection externalNintexConfigurationDatabase = Metalogix.SharePoint.ExternalConnections.Utils.GetExternalNintexConfigurationDatabase(targetNode);
				if (externalNintexConfigurationDatabase == null)
				{
					logItem.Information = "No target external Nintex configuration database specified. If this workflow is a Nintex based workflow, please ensure a target configuration database is connected.";
					logItem.Status = ActionOperationStatus.Completed;
					base.FireOperationFinished(logItem);
				}
				if (workflowInstanceIDList != null && workflowInstanceIDList.Count > 0)
				{
					LogItem logItem1 = null;
					try
					{
						logItem1 = new LogItem("Copying Nintex workflow instance database entries", "", sourceNode.DisplayUrl, targetNode.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem1);
						string nintexWorkflowInstanceDatabaseEntries = this.GetNintexWorkflowInstanceDatabaseEntries(sourceNode, workflowInstanceIDList);
						if (!string.IsNullOrEmpty(nintexWorkflowInstanceDatabaseEntries))
						{
							foreach (XmlNode str in XmlUtility.StringToXmlNode(nintexWorkflowInstanceDatabaseEntries).SelectNodes(".//WorkflowInstance"))
							{
								LogItem logItem2 = null;
								try
								{
									logItem2 = new LogItem("Copying Nintex workflow instance database entry", str.Attributes["WorkflowInstanceID"].Value, sourceNode.DisplayUrl, targetNode.DisplayUrl, ActionOperationStatus.Running);
									base.FireOperationStarted(logItem2);
									if (targetNode.Adapter.SharePointVersion.IsSharePoint2010OrLater && str.Attributes["WorkflowData"] != null)
									{
										str.Attributes["WorkflowData"].Value = DBNull.Value.ToString();
									}
									this.MapNintexWorkflowData(str, targetNode.RootSiteGUID);
									externalNintexConfigurationDatabase.SetNintexWorkflowInstanceData(str.OuterXml, targetNode.Adapter.SharePointVersion.IsSharePoint2010OrLater);
									logItem2.Status = ActionOperationStatus.Completed;
									base.FireOperationFinished(logItem2);
								}
								catch (Exception exception)
								{
									logItem2.Exception = exception;
									base.FireOperationFinished(logItem2);
								}
							}
						}
						logItem1.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem1);
					}
					catch (Exception exception1)
					{
						logItem1.Exception = exception1;
						base.FireOperationFinished(logItem1);
					}
				}
				if (sourceNode.Adapter.SharePointVersion.IsSharePoint2010OrLater)
				{
					LogItem logItem3 = null;
					try
					{
						logItem3 = new LogItem("Copying Nintex workflow association database entry", wfa.Name, sourceNode.DisplayUrl, targetNode.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem3);
						NintexExternalConnection nintexExternalConnection = Metalogix.SharePoint.ExternalConnections.Utils.GetExternalNintexConfigurationDatabase(sourceNode);
						if (nintexExternalConnection == null)
						{
							logItem3.Status = ActionOperationStatus.Warning;
							logItem3.Information = "No Nintex configuration database found in the source external connections list.";
							base.FireOperationFinished(logItem3);
						}
						else
						{
							string nintexWorkflowAssociationData = nintexExternalConnection.GetNintexWorkflowAssociationData(wfa.BaseId);
							if (nintexWorkflowAssociationData == null)
							{
								if (sourceNode.Adapter.AdapterShortName == "DB")
								{
									logItem3.Status = ActionOperationStatus.Completed;
								}
								else
								{
									logItem3.Status = ActionOperationStatus.Warning;
									logItem3.Information = "The Nintex configuration database did not contain an entry corresponding to this workflow association.";
								}
								base.FireOperationFinished(logItem3);
							}
							else
							{
								this.MapNintexWorkflowAssociationData(ref nintexWorkflowAssociationData, targetNode.RootSiteGUID);
								externalNintexConfigurationDatabase.SetNintexWorkflowAssociationData(nintexWorkflowAssociationData);
								logItem3.Status = ActionOperationStatus.Completed;
								base.FireOperationFinished(logItem3);
							}
						}
					}
					catch (Exception exception2)
					{
						logItem3.Exception = exception2;
						base.FireOperationFinished(logItem3);
					}
				}
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception3)
			{
				logItem.Exception = exception3;
				base.FireOperationFinished(logItem);
			}
		}

		private void CopySP2013Workflows(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!sourceWeb.IsSharePoint2013WorkflowsAvailable || sourceWeb.Adapter.IsMEWS && sourceWeb.IsReadOnly)
			{
				return;
			}
			string str = SP2013Utils.CreateSp2013WorkflowConfigXml(Guid.Empty, null);
			OperationReportingResultObject<List<SP2013WorkflowSubscription>> sP2013Workflows = sourceWeb.SP2013WorkflowCollection.GetSP2013Workflows(str);
			if (sP2013Workflows.ErrorOccured || sP2013Workflows.ResultObject == null)
			{
				throw new OperationReportingException("Failed to fetch SharePoint 2013 workflows.", sP2013Workflows.GetAllErrorsAsString);
			}
			List<SP2013WorkflowSubscription> resultObject = sP2013Workflows.ResultObject;
			if (resultObject.Count > 0 && !targetWeb.IsSharePoint2013WorkflowsAvailable)
			{
				LogItem logItem = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.AddSP2013WorkflowLogItem, sourceWeb.Title, sourceWeb.Url, targetWeb.Url, ActionOperationStatus.Skipped)
				{
					Information = string.Format("Skipping workflow association copy as the source site '{0}' has SharePoint 2013 style workflow associations but the target does not support them", sourceWeb.Title)
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
				return;
			}
			foreach (SP2013WorkflowSubscription sP2013WorkflowSubscription in resultObject)
			{
				if (!base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations || sP2013WorkflowSubscription.AssociatedSP2013WorkflowDefinition.RestrictToType != SP2013WorkflowScopeType.Site)
				{
					continue;
				}
				SP2013WorkflowScopeType restrictToType = sP2013WorkflowSubscription.AssociatedSP2013WorkflowDefinition.RestrictToType;
				if (!base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations && restrictToType == SP2013WorkflowScopeType.Site || !base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations && restrictToType == SP2013WorkflowScopeType.List)
				{
					continue;
				}
				LogItem logItem1 = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.AddSP2013WorkflowLogItem, sP2013WorkflowSubscription.Name, sourceWeb.Url, targetWeb.Url, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem1);
				SP2013WorkflowSubscription sP2013WorkflowSubscription1 = sP2013WorkflowSubscription.Clone();
				try
				{
					try
					{
						this.MapWorkflowAssociationData(sP2013WorkflowSubscription1);
						string str1 = SP2013Utils.CreateSp2013WorkflowConfigXml(Guid.Empty, sP2013WorkflowSubscription1);
						this.ParseOpResult(targetWeb.SP2013WorkflowCollection.MigrateSP2013Workflows(str1), logItem1);
						base.LogTelemetryForWorkflows("SPD_Workflows");
					}
					catch (Exception exception)
					{
						logItem1.Exception = exception;
						logItem1.SourceContent = SP2013Utils.Serialize<SP2013WorkflowSubscription>(sP2013WorkflowSubscription1);
					}
				}
				finally
				{
					base.FireOperationFinished(logItem1);
				}
			}
		}

		private void CopySP2013Workflows(SPList sourceList, SPList targetList, SPWeb targetWeb)
		{
			if (!sourceList.ParentWeb.IsSharePoint2013WorkflowsAvailable || sourceList.Adapter.IsMEWS && sourceList.ParentWeb.IsReadOnly)
			{
				return;
			}
			string str = SP2013Utils.CreateSp2013WorkflowConfigXml(new Guid(sourceList.ID), null);
			OperationReportingResultObject<List<SP2013WorkflowSubscription>> sP2013Workflows = sourceList.SP2013WorkflowCollection.GetSP2013Workflows(str);
			if (sP2013Workflows.ErrorOccured || sP2013Workflows.ResultObject == null)
			{
				throw new OperationReportingException("Failed to fetch SharePoint 2013 workflows.", sP2013Workflows.GetAllErrorsAsString);
			}
			List<SP2013WorkflowSubscription> resultObject = sP2013Workflows.ResultObject;
			if (resultObject.Count > 0 && !targetWeb.IsSharePoint2013WorkflowsAvailable)
			{
				LogItem logItem = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.AddSP2013WorkflowLogItem, sourceList.Title, sourceList.Url, targetList.Url, ActionOperationStatus.Skipped)
				{
					Information = string.Format("Skipping workflow association copy as the source list '{0}' has SharePoint 2013 style workflow associations but the target does not support them", sourceList.Title)
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
				return;
			}
			foreach (SP2013WorkflowSubscription sP2013WorkflowSubscription in resultObject)
			{
				if (!base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || sP2013WorkflowSubscription.AssociatedSP2013WorkflowDefinition.RestrictToType != SP2013WorkflowScopeType.List)
				{
					continue;
				}
				LogItem pleaseReviewDetails = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.AddSP2013WorkflowLogItem, sP2013WorkflowSubscription.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running);
				SP2013WorkflowSubscription sP2013WorkflowSubscription1 = sP2013WorkflowSubscription.Clone();
				try
				{
					base.FireOperationStarted(pleaseReviewDetails);
					this.MapWorkflowAssociationData(sP2013WorkflowSubscription1);
					string str1 = SP2013Utils.CreateSp2013WorkflowConfigXml(new Guid(targetList.ID), sP2013WorkflowSubscription1);
					OperationReportingResultObject<bool> operationReportingResultObject = targetList.SP2013WorkflowCollection.MigrateSP2013Workflows(str1);
					if (operationReportingResultObject.ErrorOccured)
					{
						pleaseReviewDetails.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
						pleaseReviewDetails.Details = operationReportingResultObject.GetAllErrorsAsString;
						pleaseReviewDetails.Status = ActionOperationStatus.Warning;
					}
					this.ParseOpResult(operationReportingResultObject, pleaseReviewDetails);
					base.FireOperationFinished(pleaseReviewDetails);
					base.LogTelemetryForWorkflows("SPD_Workflows");
				}
				catch (Exception exception)
				{
					pleaseReviewDetails.Exception = exception;
					pleaseReviewDetails.SourceContent = SP2013Utils.Serialize<SP2013WorkflowSubscription>(sP2013WorkflowSubscription1);
					base.FireOperationFinished(pleaseReviewDetails);
				}
			}
		}

		private void CopyWorkflowAssociations(SPList sourceList, SPList targetList, SPWeb targetWeb)
		{
			try
			{
				try
				{
					string columnValueFromWfXmlString = null;
					string str = null;
					sourceList.IncludePreviousWorkflowVersions = (!base.SharePointOptions.CopyPreviousVersionOfWorkflowInstances ? false : base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations);
					SPWorkflowAssociationCollection sPWorkflowAssociationCollection = new SPWorkflowAssociationCollection((
						from workflow in sourceList.WorkflowAssociations
						orderby workflow.BaseId, workflow.CreatedDate
						select workflow).ToArray<SPWorkflowAssociation>());
					foreach (SPWorkflowAssociation sPWorkflowAssociation in sPWorkflowAssociationCollection)
					{
						List<string> strs = null;
						if (!base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations && sPWorkflowAssociation.IsSharePointDesignerWorkflow || !base.SharePointOptions.CopyListOOBWorkflowAssociations && !sPWorkflowAssociation.IsSharePointDesignerWorkflow || sPWorkflowAssociation.IsNintexWorkflow && targetWeb.Adapter.IsClientOM)
						{
							continue;
						}
						SPWorkflowAssociation sPWorkflowAssociation1 = sPWorkflowAssociation.Clone();
						LogItem logItem = null;
						try
						{
							logItem = new LogItem("Adding List Workflow Association", sPWorkflowAssociation1.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							if (!sourceList.Adapter.SharePointVersion.IsSharePoint2007 || !targetList.Adapter.SharePointVersion.IsSharePoint2010OrLater || !sPWorkflowAssociation.IsSharePoint2007OOBWorkflowAssociation || targetWeb.HasSharePoint2007WorkflowsFeature)
							{
								this.MapWorkflowAssociationData(sPWorkflowAssociation1, sourceList.ParentWeb, targetWeb);
								this.MapOOBWorkflowAssociationStatusField(sPWorkflowAssociation1, sourceList, targetWeb);
								this.MapOOBWorkflowAssociationUsers(sPWorkflowAssociation1, targetList.ParentWeb);
								string str1 = targetList.Adapter.Writer.AddWorkflowAssociation(targetList.ID, sPWorkflowAssociation1.XML.OuterXml, SharePointConfigurationVariables.AllowDBWriting);
								this.AddNewWorkflowAssociationBaseIDToGuidMapper(sPWorkflowAssociation1.BaseId, str1);
								if (!string.IsNullOrEmpty(str1) && SharePointConfigurationVariables.AllowDBWriting && base.SharePointOptions.CopyWorkflowInstanceData)
								{
									strs = new List<string>();
									OperationReportingResult operationReportingResult = sPWorkflowAssociation1.Workflows.FetchData();
									if (operationReportingResult.ErrorOccured)
									{
										LogItem pleaseReviewDetails = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.WorkflowInstances, sPWorkflowAssociation1.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running);
										base.FireOperationStarted(pleaseReviewDetails);
										pleaseReviewDetails.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
										pleaseReviewDetails.Details = operationReportingResult.GetAllErrorsAsString;
										pleaseReviewDetails.Status = ActionOperationStatus.Warning;
										base.FireOperationFinished(pleaseReviewDetails);
									}
									if (sPWorkflowAssociation1.Workflows.Count > 0)
									{
										string workflowName = Metalogix.SharePoint.Adapters.Utils.GetWorkflowName(sPWorkflowAssociation1.Name);
										if (string.IsNullOrEmpty(str) || !str.Equals(workflowName, StringComparison.InvariantCultureIgnoreCase))
										{
											columnValueFromWfXmlString = null;
											str = workflowName;
										}
										foreach (SPWorkflow sPWorkflow in sPWorkflowAssociation1.Workflows)
										{
											string d = sPWorkflow.ID;
											SPWorkflow sPWorkflow1 = sPWorkflow.Clone();
											if (!base.SharePointOptions.CopyInProgressWorkflows && sPWorkflow1.IsRunning)
											{
												Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(sPWorkflow.XML, "TerminateWorkflow", true.ToString());
											}
											if (columnValueFromWfXmlString == null)
											{
												this.UpdateStatusColumn(sPWorkflow1, sPWorkflowAssociation1.XML.Attributes["StatusColumnName"].Value, targetList);
											}
											else
											{
												this.UpdateStatusColumn(sPWorkflow1, columnValueFromWfXmlString, null);
											}
											string str2 = this.CopyWorkflowInstance(sPWorkflow1, targetList, sPWorkflowAssociation1, str1, sourceList.Url, strs);
											if (str2 == null)
											{
												continue;
											}
											columnValueFromWfXmlString = this.GetColumnValueFromWfXmlString(str2, "StatusColumnName");
										}
									}
								}
								if (base.SharePointOptions.CopyNintexDatabaseEntries && sourceList.Adapter.AdapterShortName == "DB" || base.SharePointOptions.CopyNintexDatabaseEntries && sourceList.Adapter.AdapterShortName != "DB" && sPWorkflowAssociation1.IsSharePointDesignerWorkflow)
								{
									this.CopyNintexWorkflowDatabaseEntries(sourceList.ParentWeb, targetList.ParentWeb, sPWorkflowAssociation1, strs);
								}
								if (base.SharePointOptions.Verbose)
								{
									logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
									logItem.TargetContent = str1;
								}
								logItem.Status = ActionOperationStatus.Completed;
								base.FireOperationFinished(logItem);
								this.LogWorkflowTelemetryInformation(sPWorkflowAssociation, str1, sourceList.ParentWeb.Language);
							}
							else
							{
								logItem.Status = ActionOperationStatus.Warning;
								logItem.Information = Metalogix.SharePoint.Adapters.Properties.Resources.UnableToMigrateListWorkflowAssociation;
								base.FireOperationFinished(logItem);
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							if (exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToRetrieveWorkflowTemplate) || exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToFindWorkflowAssociationInList))
							{
								logItem.Status = ActionOperationStatus.Warning;
								logItem.Information = exception.Message;
							}
							else
							{
								logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
								logItem.Exception = exception;
							}
							base.FireOperationFinished(logItem);
						}
					}
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					LogItem logItem1 = new LogItem("Retrieving List Workflow Association", sourceList.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Failed)
					{
						Exception = exception2,
						Status = ActionOperationStatus.Failed,
						Information = exception2.Message
					};
					base.FireOperationStarted(logItem1);
					base.FireOperationFinished(logItem1);
				}
			}
			finally
			{
				sourceList.Dispose();
				targetList.Dispose();
			}
		}

		private void CopyWorkflowAssociations(SPContentType sourceCT, SPContentType targetCT, SPList targetList)
		{
			try
			{
				string columnValueFromWfXmlString = null;
				string str = null;
				string str1 = null;
				sourceCT.IncludePreviousWorkflowVersions = (!base.SharePointOptions.CopyPreviousVersionOfWorkflowInstances ? false : base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations);
				SPWorkflowAssociationCollection sPWorkflowAssociationCollection = new SPWorkflowAssociationCollection((
					from workflow in sourceCT.WorkflowAssociations
					orderby workflow.BaseId, workflow.CreatedDate
					select workflow).ToArray<SPWorkflowAssociation>());
				foreach (SPWorkflowAssociation sPWorkflowAssociation in sPWorkflowAssociationCollection)
				{
					List<string> strs = null;
					SPWeb parentWeb = targetCT.ParentCollection.ParentWeb;
					if (!base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations && sPWorkflowAssociation.IsSharePointDesignerWorkflow || !base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations && !sPWorkflowAssociation.IsSharePointDesignerWorkflow || parentWeb.Adapter.IsClientOM && sPWorkflowAssociation.IsNintexWorkflow)
					{
						continue;
					}
					SPWorkflowAssociation sPWorkflowAssociation1 = sPWorkflowAssociation.Clone();
					LogItem logItem = null;
					logItem = new LogItem(string.Format("Adding Content Type Workflow Association", (targetList != null ? "[List]" : string.Empty)), sPWorkflowAssociation1.Name, string.Format("{0} ({1})", sourceCT.Name, sourceCT.ParentCollection.ParentWeb.Url), string.Format("{0} ({1})", targetCT.Name, (targetList != null ? targetList.Url : targetCT.ParentCollection.ParentWeb.Url)), ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					try
					{
						try
						{
							if (targetList != null && base.GuidMappings.ContainsKey(new Guid(sPWorkflowAssociation1.XML.Attributes["ContentTypeParentListId"].Value)))
							{
								XmlAttribute itemOf = sPWorkflowAssociation1.XML.Attributes["ContentTypeParentListId"];
								Guid item = base.GuidMappings[new Guid(sPWorkflowAssociation1.XML.Attributes["ContentTypeParentListId"].Value)];
								itemOf.Value = item.ToString();
							}
							this.MapWorkflowAssociationData(sPWorkflowAssociation1, sourceCT.ParentCollection.ParentWeb, parentWeb);
							this.MapOOBWorkflowAssociationStatusField(sPWorkflowAssociation1, sourceCT.ParentCollection.ParentList, parentWeb);
							this.MapOOBWorkflowAssociationUsers(sPWorkflowAssociation1, parentWeb);
							string workflowName = Metalogix.SharePoint.Adapters.Utils.GetWorkflowName(sPWorkflowAssociation1.Name);
							if (string.IsNullOrEmpty(str1) || !str1.Equals(workflowName, StringComparison.InvariantCultureIgnoreCase))
							{
								columnValueFromWfXmlString = null;
								str = null;
								str1 = workflowName;
							}
							Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(sPWorkflowAssociation1.XML, "StatusColumnInternalName", str);
							string str2 = parentWeb.Adapter.Writer.AddWorkflowAssociation(targetCT.ContentTypeID, sPWorkflowAssociation1.XML.OuterXml, SharePointConfigurationVariables.AllowDBWriting);
							this.AddNewWorkflowAssociationBaseIDToGuidMapper(sPWorkflowAssociation1.BaseId, str2);
							if (!string.IsNullOrEmpty(str2) && SharePointConfigurationVariables.AllowDBWriting && base.SharePointOptions.CopyWorkflowInstanceData && targetList != null)
							{
								strs = new List<string>();
								OperationReportingResult operationReportingResult = sPWorkflowAssociation1.Workflows.FetchData();
								if (operationReportingResult.ErrorOccured)
								{
									LogItem pleaseReviewDetails = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.WorkflowInstances, sPWorkflowAssociation1.Name, string.Format("{0} ({1})", sourceCT.Name, sourceCT.ParentCollection.ParentWeb.Url), string.Format("{0} ({1})", targetCT.Name, (targetList != null ? targetList.Url : targetCT.ParentCollection.ParentWeb.Url)), ActionOperationStatus.Running);
									base.FireOperationStarted(pleaseReviewDetails);
									pleaseReviewDetails.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
									pleaseReviewDetails.Details = operationReportingResult.GetAllErrorsAsString;
									pleaseReviewDetails.Status = ActionOperationStatus.Warning;
									base.FireOperationFinished(pleaseReviewDetails);
								}
								if (sPWorkflowAssociation1.Workflows.Count > 0)
								{
									foreach (SPWorkflow sPWorkflow in sPWorkflowAssociation1.Workflows)
									{
										SPWorkflow sPWorkflow1 = sPWorkflow.Clone();
										if (!base.SharePointOptions.CopyInProgressWorkflows && sPWorkflow1.IsRunning)
										{
											Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(sPWorkflow.XML, "TerminateWorkflow", true.ToString());
										}
										if (columnValueFromWfXmlString == null)
										{
											this.UpdateStatusColumn(sPWorkflow1, sPWorkflowAssociation1.XML.Attributes["StatusColumnName"].Value, targetList);
										}
										else
										{
											this.UpdateStatusColumn(sPWorkflow1, columnValueFromWfXmlString, null);
										}
										string str3 = this.CopyWorkflowInstance(sPWorkflow1, targetList, sPWorkflowAssociation1, str2, targetCT, strs);
										if (str3 == null)
										{
											continue;
										}
										columnValueFromWfXmlString = this.GetColumnValueFromWfXmlString(str3, "StatusColumnName");
										if (!string.IsNullOrEmpty(str))
										{
											continue;
										}
										str = this.GetColumnValueFromWfXmlString(str3, "StatusColumnInternalName");
									}
								}
							}
							if (base.SharePointOptions.CopyNintexDatabaseEntries && sourceCT.ParentCollection.ParentWeb.Adapter.AdapterShortName == "DB" || base.SharePointOptions.CopyNintexDatabaseEntries && sourceCT.ParentCollection.ParentWeb.Adapter.AdapterShortName != "DB" && sPWorkflowAssociation1.IsSharePointDesignerWorkflow)
							{
								this.CopyNintexWorkflowDatabaseEntries(sourceCT.ParentCollection.ParentWeb, targetCT.ParentCollection.ParentWeb, sPWorkflowAssociation1, strs);
							}
							if (base.SharePointOptions.Verbose)
							{
								logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
								logItem.TargetContent = str2;
							}
							logItem.Status = ActionOperationStatus.Completed;
							this.LogWorkflowTelemetryInformation(sPWorkflowAssociation, str2, sourceCT.ParentCollection.ParentWeb.Language);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							if (exception.Message.Contains(Metalogix.SharePoint.Properties.Resources.MsgCannotFindContentType))
							{
								logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
								logItem.Status = ActionOperationStatus.Skipped;
								logItem.Information = exception.Message;
								logItem.Details = exception.StackTrace;
							}
							else if (exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToRetrieveWorkflowTemplate) || exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToFindWorkflowAssociationInList))
							{
								logItem.Status = ActionOperationStatus.Warning;
								logItem.Information = exception.Message;
								if (targetCT.ParentCollection.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && !targetCT.ParentCollection.ParentWeb.HasSharePoint2007WorkflowsFeature)
								{
									logItem.Information = logItem.Information.Replace(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToRetrieveWorkflowTemplateReplace, Metalogix.SharePoint.Adapters.Properties.Resources.SharePoint2007WorkflowFeatureNotActivated);
								}
							}
							else if (!exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToUpdateAlreadyExistsSharePointError))
							{
								logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
								logItem.Exception = exception;
							}
							else
							{
								logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
								logItem.Status = ActionOperationStatus.Warning;
								logItem.Information = exception.Message;
								logItem.Details = exception.StackTrace;
							}
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				LogItem logItem1 = new LogItem(string.Format("Retrieving Content Type Workflow Association", (targetList != null ? "[List]" : string.Empty)), sourceCT.Name, string.Format("{0} ({1})", sourceCT.Name, sourceCT.ParentCollection.ParentWeb.Url), string.Format("{0} ({1})", targetCT.Name, (targetList != null ? targetList.Url : targetCT.ParentCollection.ParentWeb.Url)), ActionOperationStatus.Failed)
				{
					Exception = exception2,
					Status = ActionOperationStatus.Failed,
					Information = exception2.Message
				};
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
			}
		}

		private void CopyWorkflowAssociations(SPWeb sourceWeb, SPWeb targetWeb)
		{
			try
			{
				sourceWeb.IncludePreviousWorkflowVersions = (!base.SharePointOptions.CopyPreviousVersionOfWorkflowInstances ? false : base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations);
				SPWorkflowAssociationCollection sPWorkflowAssociationCollection = new SPWorkflowAssociationCollection((
					from workflow in sourceWeb.WorkflowAssociations
					orderby workflow.BaseId, workflow.CreatedDate
					select workflow).ToArray<SPWorkflowAssociation>());
				foreach (SPWorkflowAssociation sPWorkflowAssociation in sPWorkflowAssociationCollection)
				{
					List<string> strs = null;
					if (!base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations && sPWorkflowAssociation.IsSharePointDesignerWorkflow || !base.SharePointOptions.CopyWebOOBWorkflowAssociations && !sPWorkflowAssociation.IsSharePointDesignerWorkflow || sPWorkflowAssociation.IsNintexWorkflow && targetWeb.Adapter.IsClientOM)
					{
						continue;
					}
					SPWorkflowAssociation sPWorkflowAssociation1 = sPWorkflowAssociation.Clone();
					LogItem logItem = null;
					try
					{
						logItem = new LogItem("Adding Web Workflow Association", sPWorkflowAssociation1.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						this.MapWorkflowAssociationData(sPWorkflowAssociation1, sourceWeb, targetWeb);
						this.MapOOBWorkflowAssociationUsers(sPWorkflowAssociation1, targetWeb);
						string str = targetWeb.Adapter.Writer.AddWorkflowAssociation(targetWeb.ID, sPWorkflowAssociation1.XML.OuterXml, SharePointConfigurationVariables.AllowDBWriting);
						this.AddNewWorkflowAssociationBaseIDToGuidMapper(sPWorkflowAssociation1.BaseId, str);
						if (!string.IsNullOrEmpty(str) && SharePointConfigurationVariables.AllowDBWriting && base.SharePointOptions.CopyWorkflowInstanceData)
						{
							strs = new List<string>();
							OperationReportingResult operationReportingResult = sPWorkflowAssociation1.Workflows.FetchData();
							if (operationReportingResult.ErrorOccured)
							{
								LogItem pleaseReviewDetails = new LogItem(Metalogix.SharePoint.Adapters.Properties.Resources.WorkflowInstances, sPWorkflowAssociation1.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(pleaseReviewDetails);
								pleaseReviewDetails.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
								pleaseReviewDetails.Details = operationReportingResult.GetAllErrorsAsString;
								pleaseReviewDetails.Status = ActionOperationStatus.Warning;
								base.FireOperationFinished(pleaseReviewDetails);
							}
							foreach (SPWorkflow sPWorkflow in sPWorkflowAssociation1.Workflows)
							{
								string d = sPWorkflow.ID;
								SPWorkflow sPWorkflow1 = sPWorkflow.Clone();
								if (!base.SharePointOptions.CopyInProgressWorkflows && sPWorkflow1.IsRunning)
								{
									Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(sPWorkflow.XML, "TerminateWorkflow", true.ToString());
								}
								this.CopyWorkflowInstance(sPWorkflow1, targetWeb, sPWorkflowAssociation1, str, sourceWeb.DisplayUrl, strs);
							}
						}
						if (base.SharePointOptions.CopyNintexDatabaseEntries && sourceWeb.Adapter.AdapterShortName == "DB" || base.SharePointOptions.CopyNintexDatabaseEntries && sourceWeb.Adapter.AdapterShortName != "DB" && sPWorkflowAssociation1.IsSharePointDesignerWorkflow)
						{
							this.CopyNintexWorkflowDatabaseEntries(sourceWeb, targetWeb, sPWorkflowAssociation1, strs);
						}
						if (base.SharePointOptions.Verbose)
						{
							logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
							logItem.TargetContent = str;
						}
						logItem.Status = ActionOperationStatus.Completed;
						base.FireOperationFinished(logItem);
						this.LogWorkflowTelemetryInformation(sPWorkflowAssociation, str, sourceWeb.Language);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToRetrieveWorkflowTemplate) || exception.Message.Contains(Metalogix.SharePoint.Adapters.Properties.Resources.UnableToFindWorkflowAssociationInList))
						{
							logItem.Status = ActionOperationStatus.Warning;
							logItem.Information = exception.Message;
						}
						else
						{
							logItem.SourceContent = sPWorkflowAssociation1.XML.OuterXml;
							logItem.Exception = exception;
						}
						base.FireOperationFinished(logItem);
					}
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				LogItem logItem1 = new LogItem("Retrieving Web Workflow Association", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Failed)
				{
					Exception = exception2,
					Status = ActionOperationStatus.Failed,
					Information = exception2.Message
				};
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
			}
		}

		private string CopyWorkflowInstance(SPWorkflow workflow, object target, SPWorkflowAssociation wfa, string sNewWorkflowAssociationXml, object oContentTypeOrSourceLocation, List<string> WorkflowIDList)
		{
			LogItem logItem = null;
			SPList sPList = target as SPList;
			string str = null;
			string str1 = null;
			try
			{
				string d = workflow.ID;
				if (sPList == null)
				{
					SPWeb sPWeb = target as SPWeb;
					logItem = new LogItem("Adding Web Workflow Instance", workflow.XML.Attributes["Id"].Value, oContentTypeOrSourceLocation.ToString(), sPWeb.Url, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					str1 = this.MapWorkflowData(workflow, null, wfa, sNewWorkflowAssociationXml, oContentTypeOrSourceLocation);
					if (str1 == null)
					{
						str = sPWeb.Adapter.Writer.AddWorkflow(sPWeb.ID, workflow.XML.OuterXml);
						this.AddNewWorkflowIDToGuidMapper(d, str);
					}
				}
				else
				{
					logItem = new LogItem("Adding Item Workflow Instance", workflow.XML.Attributes["Id"].Value, oContentTypeOrSourceLocation.ToString(), sPList.Url, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					str1 = this.MapWorkflowData(workflow, sPList, wfa, sNewWorkflowAssociationXml, oContentTypeOrSourceLocation);
					if (str1 == null)
					{
						str = sPList.Adapter.Writer.AddWorkflow(sPList.ID, workflow.XML.OuterXml);
						this.AddNewWorkflowIDToGuidMapper(d, str);
					}
				}
				if (str == null || str1 != null)
				{
					logItem.TargetContent = str;
					logItem.Status = ActionOperationStatus.Warning;
					logItem.Information = (str1 != null ? str1 : "The workflow instance could not be added to the database, please ensure the Metalogix.SharePoint.Adapters.DB.dll is present in the GAC.");
				}
				else
				{
					WorkflowIDList.Add(d);
					logItem.TargetContent = str;
					logItem.Status = ActionOperationStatus.Completed;
				}
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (exception is IndexOutOfRangeException)
				{
					logItem.Exception = exception;
					logItem.Information = string.Concat("Index out of range exception thrown, please ensure all relevant data for this workflow exists on the target. Message: ", exception.Message);
				}
				else if (!exception.Message.Contains("WFA Column creation succeeded with ColName"))
				{
					logItem.Exception = exception;
				}
				else
				{
					logItem.Exception = exception;
					return this.ExtractIntoXmlNewColumnNameFromErrorMessage(exception.Message);
				}
				base.FireOperationFinished(logItem);
			}
			return str;
		}

		private string ExtractIntoXmlNewColumnNameFromErrorMessage(string sErrorMessage)
		{
			int num = sErrorMessage.IndexOf("ColName:<") + 9;
			string str = sErrorMessage.Substring(num, sErrorMessage.Length - num);
			str = str.Substring(0, str.IndexOf(">"));
			return string.Concat("<Workflow StatusColumnName=\"", str, "\"/>");
		}

		private string GetColumnValueFromWfXmlString(string workflowXml, string columnName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(workflowXml);
			return xmlDocument.DocumentElement.GetAttributeValueAsString(columnName);
		}

		private string GetNintexWorkflowInstanceDatabaseEntries(SPNode sourceNode, List<string> workflowInstanceIDList)
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement("ExternalConnectionCollections");
			foreach (NintexExternalConnection value in sourceNode.GetExternalConnectionsOfType<NintexExternalConnection>(true).Values)
			{
				try
				{
					xmlTextWriter.WriteRaw(value.GetNintexWorkflowInstanceData(workflowInstanceIDList, sourceNode.Adapter.SharePointVersion.IsSharePoint2010OrLater));
				}
				catch
				{
				}
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		private string GetTargetField(Guid sourceFieldId, SPList sourceList, SPWeb targetWeb)
		{
			string str;
			try
			{
				if (!base.GuidMappings.ContainsKey(sourceFieldId))
				{
					SPField sPField = (
						from SPField column in sourceList.FieldCollection
						where column.ID == sourceFieldId
						select column).FirstOrDefault<SPField>();
					if (sPField != null)
					{
						string targetList = this.GetTargetList(sourceList.ID, sourceList.ParentWeb, targetWeb);
						SPList listByGuid = targetWeb.Lists.GetListByGuid(targetList);
						if (listByGuid == null)
						{
							LogItem logItem = new LogItem("Fetching target list", sourceList.Name, sourceList.ParentWeb.Url, targetWeb.Url, ActionOperationStatus.Warning);
							base.FireOperationStarted(logItem);
							logItem.Information = string.Format("List '{0}' is not avaiable on target.", sourceList.Name);
							base.FireOperationFinished(logItem);
						}
						else
						{
							SPField sPField1 = (
								from SPField column in listByGuid.FieldCollection
								where column.Name == sPField.Name
								select column).FirstOrDefault<SPField>();
							if (sPField1 == null)
							{
								LogItem logItem1 = new LogItem("Copy Workflow Fields", sPField.Name, sourceList.ParentWeb.Url, targetWeb.Url, ActionOperationStatus.Warning);
								base.FireOperationStarted(logItem1);
								logItem1.Information = string.Format("Workflow field '{0}' is not avaiable on target.", sPField.Name);
								base.FireOperationFinished(logItem1);
							}
							else
							{
								base.GuidMappings.Add(sourceFieldId, sPField1.ID);
								str = sPField1.ID.ToString();
								return str;
							}
						}
					}
					return sourceFieldId.ToString();
				}
				else
				{
					str = base.GuidMappings[sourceFieldId].ToString();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem stackTrace = new LogItem("Copy Workflows", sourceList.Name, sourceList.ParentWeb.Url, targetWeb.Url, ActionOperationStatus.Running);
				base.FireOperationStarted(stackTrace);
				stackTrace.Exception = exception;
				stackTrace.Information = string.Format("An error occurred while fetching mapped target field id for the source field id '{0}'. Error : {1}", sourceFieldId, exception.Message);
				stackTrace.Details = exception.StackTrace;
				base.FireOperationFinished(stackTrace);
				return sourceFieldId.ToString();
			}
			return str;
		}

		private string GetTargetList(string sourceListId, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string targetList = SPUtils.GetTargetList(base.GuidMappings, sourceListId, sourceWeb, targetWeb);
			if (!base.GuidMappings.ContainsKey(new Guid(sourceListId)))
			{
				base.GuidMappings.Add(new Guid(sourceListId), new Guid(targetList));
			}
			if (sourceListId.Equals(targetList, StringComparison.InvariantCultureIgnoreCase))
			{
				LogItem logItem = new LogItem("Fetching target list", sourceWeb.Name, sourceWeb.Url, targetWeb.Url, ActionOperationStatus.Warning);
				base.FireOperationStarted(logItem);
				logItem.Information = string.Format("Source List Id '{0}' is not avaiable on target.", sourceListId);
				base.FireOperationFinished(logItem);
			}
			return targetList;
		}

		private bool IsUserWithNonePermissionsMapped(SPWorkflowAssociation workflowAssociation, SPWeb targetWeb, Hashtable uniqueUsers, IEnumerable<XElement> collection, IEnumerable<XElement> personsDisplayName, bool isUserMapped)
		{
			SecurityPrincipalCollection siteUsers = null;
			string scope = workflowAssociation.Scope;
			string str = scope;
			if (scope != null)
			{
				if (str == "List")
				{
					siteUsers = workflowAssociation.ParentCollection.ParentList.ParentWeb.SiteUsers;
				}
				else if (str == "ContentType")
				{
					siteUsers = workflowAssociation.ParentCollection.ParentContentType.ParentCollection.ParentWeb.SiteUsers;
				}
				else if (str == "Web")
				{
					siteUsers = workflowAssociation.ParentCollection.ParentWeb.SiteUsers;
				}
			}
			base.EnsurePrincipalExistence(SPUtils.GetReferencedPrincipals(uniqueUsers, siteUsers, false), targetWeb);
			foreach (string key in uniqueUsers.Keys)
			{
				XElement xElement = null;
				string str1 = base.MapPrincipal(key);
				if (string.Equals(key, str1, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				IEnumerable<XElement> xElements = 
					from s in collection
					where s.Value.ToString().Equals(key, StringComparison.InvariantCultureIgnoreCase)
					select s;
				foreach (XElement xElement1 in xElements)
				{
					if (personsDisplayName.Count<XElement>() > 0)
					{
						string str2 = key.Substring(key.LastIndexOf('|') + 1);
						xElement = (
							from s in personsDisplayName
							where s.Value.ToString().Equals(str2, StringComparison.InvariantCultureIgnoreCase)
							select s).FirstOrDefault<XElement>();
					}
					this.SetUserAccountIDAndDisplayName(xElement, xElement1, key, str1, targetWeb.Adapter.SharePointVersion.IsSharePointOnline);
					isUserMapped = true;
				}
			}
			return isUserMapped;
		}

		private void LogWorkflowTelemetryInformation(SPWorkflowAssociation workflowAssociation, string newWorkflowAssociationXml, int language)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(newWorkflowAssociationXml);
			Guid guid = new Guid(xmlDocument.FirstChild.GetAttributeValueAsString("BaseTemplate"));
			string str = (Metalogix.SharePoint.Adapters.Utils.IsOOBWorkflowAssociation(guid, language, false) ? "OOB_Workflows" : "SPD_Workflows");
			if (!str.Equals("OOB_Workflows"))
			{
				bool attributeValueAsBoolean = xmlDocument.FirstChild.GetAttributeValueAsBoolean("IsNintex");
				if (workflowAssociation.IsNintexWorkflow || attributeValueAsBoolean)
				{
					str = "Nintex_Workflows";
				}
			}
			base.LogTelemetryForWorkflows(str);
		}

		private void MapGenericNintexData(XmlNode ndWorkflowDatabaseEntry, string targetSiteGUID, string sDCase)
		{
			if (ndWorkflowDatabaseEntry.Attributes[string.Concat("WebApplicationI", sDCase)] != null && !string.IsNullOrEmpty(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebApplicationI", sDCase)].Value) && base.GuidMappings.ContainsKey(new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebApplicationI", sDCase)].Value)))
			{
				XmlAttribute itemOf = ndWorkflowDatabaseEntry.Attributes[string.Concat("WebApplicationI", sDCase)];
				Guid item = base.GuidMappings[new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebApplicationI", sDCase)].Value)];
				itemOf.Value = item.ToString().ToUpper();
			}
			if (ndWorkflowDatabaseEntry.Attributes[string.Concat("SiteI", sDCase)] != null)
			{
				ndWorkflowDatabaseEntry.Attributes[string.Concat("SiteI", sDCase)].Value = targetSiteGUID.ToUpper();
			}
			if (ndWorkflowDatabaseEntry.Attributes[string.Concat("WebI", sDCase)] != null && !string.IsNullOrEmpty(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebI", sDCase)].Value) && base.GuidMappings.ContainsKey(new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebI", sDCase)].Value)))
			{
				XmlAttribute upper = ndWorkflowDatabaseEntry.Attributes[string.Concat("WebI", sDCase)];
				Guid guid = base.GuidMappings[new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("WebI", sDCase)].Value)];
				upper.Value = guid.ToString().ToUpper();
			}
			if (ndWorkflowDatabaseEntry.Attributes[string.Concat("ListI", sDCase)] != null && !string.IsNullOrEmpty(ndWorkflowDatabaseEntry.Attributes[string.Concat("ListI", sDCase)].Value) && base.GuidMappings.ContainsKey(new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("ListI", sDCase)].Value)))
			{
				XmlAttribute xmlAttribute = ndWorkflowDatabaseEntry.Attributes[string.Concat("ListI", sDCase)];
				Guid item1 = base.GuidMappings[new Guid(ndWorkflowDatabaseEntry.Attributes[string.Concat("ListI", sDCase)].Value)];
				xmlAttribute.Value = item1.ToString().ToUpper();
			}
			if (ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)] != null && !string.IsNullOrEmpty(ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value) && base.WorkflowMappings.ContainsKey(string.Concat("{", ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value.ToUpper(), "}")))
			{
				ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value = base.WorkflowMappings[string.Concat("{", ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value.ToUpper(), "}")].Replace("{", "");
				ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value = ndWorkflowDatabaseEntry.Attributes[string.Concat("WorkflowI", sDCase)].Value.Replace("}", "");
			}
		}

		private void MapNintexWorkflowAssociationData(ref string sNintexWorkflowDatabaseEntries, string targetSiteGUID)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sNintexWorkflowDatabaseEntries);
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				this.MapGenericNintexData(childNode, targetSiteGUID, "d");
				if (childNode.Attributes["Author"] == null || string.IsNullOrEmpty(childNode.Attributes["Author"].Value))
				{
					continue;
				}
				childNode.Attributes["Author"].Value = base.MapPrincipal(childNode.Attributes["Author"].Value);
			}
			sNintexWorkflowDatabaseEntries = xmlNode.OuterXml;
		}

		private void MapNintexWorkflowData(XmlNode ndNintexWorkflowInstanceDatabaseEntry, string targetSiteGUID)
		{
			if (!base.SharePointOptions.CopyInProgressWorkflows && ndNintexWorkflowInstanceDatabaseEntry.Attributes["State"] != null && ndNintexWorkflowInstanceDatabaseEntry.Attributes["State"].Value == 2.ToString())
			{
				ndNintexWorkflowInstanceDatabaseEntry.Attributes["State"].Value = 8.ToString();
			}
			this.MapGenericNintexData(ndNintexWorkflowInstanceDatabaseEntry, targetSiteGUID, "D");
			if (ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInitiator"] != null && !string.IsNullOrEmpty(ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInitiator"].Value))
			{
				ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInitiator"].Value = base.MapPrincipal(ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInitiator"].Value);
			}
			if (ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInstanceID"] != null && !string.IsNullOrEmpty(ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInstanceID"].Value) && base.WorkflowMappings.ContainsKey(ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInstanceID"].Value))
			{
				ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInstanceID"].Value = base.WorkflowMappings[ndNintexWorkflowInstanceDatabaseEntry.Attributes["WorkflowInstanceID"].Value];
			}
			if (ndNintexWorkflowInstanceDatabaseEntry.Attributes["TaskListID"] != null && !string.IsNullOrEmpty(ndNintexWorkflowInstanceDatabaseEntry.Attributes["TaskListID"].Value) && base.GuidMappings.ContainsKey(new Guid(ndNintexWorkflowInstanceDatabaseEntry.Attributes["TaskListID"].Value)))
			{
				XmlAttribute itemOf = ndNintexWorkflowInstanceDatabaseEntry.Attributes["TaskListID"];
				Guid item = base.GuidMappings[new Guid(ndNintexWorkflowInstanceDatabaseEntry.Attributes["TaskListID"].Value)];
				itemOf.Value = item.ToString().ToUpper();
			}
			if (ndNintexWorkflowInstanceDatabaseEntry.Attributes["HistoryListID"] != null && !string.IsNullOrEmpty(ndNintexWorkflowInstanceDatabaseEntry.Attributes["HistoryListID"].Value) && base.GuidMappings.ContainsKey(new Guid(ndNintexWorkflowInstanceDatabaseEntry.Attributes["HistoryListID"].Value)))
			{
				XmlAttribute upper = ndNintexWorkflowInstanceDatabaseEntry.Attributes["HistoryListID"];
				Guid guid = base.GuidMappings[new Guid(ndNintexWorkflowInstanceDatabaseEntry.Attributes["HistoryListID"].Value)];
				upper.Value = guid.ToString().ToUpper();
			}
		}

		private void MapOOBWorkflowAssociationStatusField(SPWorkflowAssociation workflowAssociation, SPList sourceList, SPWeb targetWeb)
		{
			XmlAttribute itemOf = workflowAssociation.XML.Attributes["BaseTemplate"];
			if (itemOf != null)
			{
				string value = itemOf.Value;
				if (!string.IsNullOrEmpty(value) && value.Equals("c6964bff-bf8d-41ac-ad5e-b61ec111731a", StringComparison.InvariantCultureIgnoreCase))
				{
					bool flag = false;
					XmlAttribute xmlAttribute = workflowAssociation.XML.Attributes["AssociationData"];
					if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value))
					{
						XDocument xDocument = XDocument.Parse(xmlAttribute.Value);
						List<string> strs = new List<string>()
						{
							"StatusField"
						};
						IEnumerable<XElement> xElements = 
							from p in xDocument.Descendants()
							where strs.Contains(p.Name.LocalName)
							select p;
						foreach (XElement xElement in xElements)
						{
							if (string.IsNullOrEmpty(xElement.Value))
							{
								continue;
							}
							Guid guid = new Guid(xElement.Value);
							string targetField = this.GetTargetField(guid, sourceList, targetWeb);
							if (string.IsNullOrEmpty(targetField))
							{
								continue;
							}
							Guid guid1 = new Guid(targetField);
							if (guid == guid1)
							{
								continue;
							}
							xElement.SetValue(guid1.ToString("B").ToUpper());
							flag = true;
						}
						if (flag && workflowAssociation.XML.Attributes["AssociationData"] != null)
						{
							workflowAssociation.XML.Attributes["AssociationData"].Value = xDocument.ToString(SaveOptions.DisableFormatting);
						}
					}
				}
			}
		}

		private void MapOOBWorkflowAssociationUsers(SPWorkflowAssociation workflowAssociation, SPWeb targetWeb)
		{
			bool flag = false;
			XmlAttribute itemOf = workflowAssociation.XML.Attributes["AssociationData"];
			if (itemOf != null && !string.IsNullOrEmpty(itemOf.Value))
			{
				Hashtable hashtables = new Hashtable();
				XDocument xDocument = XDocument.Parse(itemOf.Value);
				List<string> strs = new List<string>()
				{
					"AssignedToField",
					"AssignedToField2",
					"CustomAssignedTo",
					"CustomAssignedTo2"
				};
				IEnumerable<XElement> xElements = 
					from p in xDocument.Descendants()
					where strs.Contains(p.Name.LocalName)
					select p;
				foreach (XElement xElement in xElements)
				{
					if (string.IsNullOrEmpty(xElement.Value))
					{
						continue;
					}
					string str = base.MapPrincipal(xElement.Value);
					if (string.Equals(xElement.Value, str, StringComparison.OrdinalIgnoreCase))
					{
						if (hashtables.ContainsKey(str))
						{
							continue;
						}
						hashtables.Add(str, str);
					}
					else
					{
						xElement.SetValue(str);
						flag = true;
					}
				}
				IEnumerable<XElement> xElements1 = 
					from p in xDocument.Descendants()
					where p.Name.LocalName == "Person"
					select p;
				IEnumerable<XElement> xElements2 = 
					from p in xElements1.Descendants<XElement>()
					where p.Name.LocalName == "DisplayName"
					select p;
				IEnumerable<XElement> xElements3 = 
					from p in xElements1.Descendants<XElement>()
					where p.Name.LocalName == "AccountId"
					select p;
				foreach (XElement xElement1 in xElements1)
				{
					XElement xElement2 = (
						from element in xElement1.Elements()
						where element.Name.LocalName == "AccountId"
						select element).FirstOrDefault<XElement>();
					if (xElement2 == null)
					{
						continue;
					}
					string value = xElement2.Value;
					string str1 = base.MapPrincipal(value);
					if (string.Equals(value, str1, StringComparison.OrdinalIgnoreCase))
					{
						if (hashtables.ContainsKey(str1))
						{
							continue;
						}
						hashtables.Add(str1, str1);
					}
					else
					{
						XElement xElement3 = (
							from element in xElement1.Elements()
							where element.Name.LocalName == "DisplayName"
							select element).FirstOrDefault<XElement>();
						this.SetUserAccountIDAndDisplayName(xElement3, xElement2, value, str1, targetWeb.Adapter.SharePointVersion.IsSharePointOnline);
						flag = true;
					}
				}
				if (hashtables.Count > 0)
				{
					flag = (xElements1.Count<XElement>() <= 0 ? this.IsUserWithNonePermissionsMapped(workflowAssociation, targetWeb, hashtables, xElements, xElements2, flag) : this.IsUserWithNonePermissionsMapped(workflowAssociation, targetWeb, hashtables, xElements3, xElements2, flag));
				}
				if (flag && workflowAssociation.XML.Attributes["AssociationData"] != null && workflowAssociation.XML.Attributes["AssociationData"] != null)
				{
					workflowAssociation.XML.Attributes["AssociationData"].Value = xDocument.ToString(SaveOptions.DisableFormatting);
				}
			}
		}

		private void MapWorkflowAssociationData(SPWorkflowAssociation wfa, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!string.IsNullOrEmpty(wfa.HistoryListId))
			{
				wfa.HistoryListId = this.GetTargetList(wfa.HistoryListId, sourceWeb, targetWeb);
			}
			if (!string.IsNullOrEmpty(wfa.TasksListId))
			{
				wfa.TasksListId = this.GetTargetList(wfa.TasksListId, sourceWeb, targetWeb);
			}
			if (wfa.XML.Attributes["BaseTemplate"] != null)
			{
				if (base.WorkflowMappings.ContainsKey(string.Concat("{", wfa.XML.Attributes["BaseTemplate"].Value.ToUpper(), "}")))
				{
					wfa.XML.Attributes["BaseTemplate"].Value = base.WorkflowMappings[string.Concat("{", wfa.XML.Attributes["BaseTemplate"].Value.ToUpper(), "}")];
					return;
				}
				string str = base.LinkCorrector.MapGuid(wfa.XML.Attributes["BaseTemplate"].Value);
				if (!string.IsNullOrEmpty(str))
				{
					wfa.XML.Attributes["BaseTemplate"].Value = str;
				}
			}
		}

		private void MapWorkflowAssociationData(SP2013WorkflowSubscription wfs)
		{
			string str;
			string empty;
			if (!string.IsNullOrEmpty(wfs.HistoryListId))
			{
				SP2013WorkflowSubscription sP2013WorkflowSubscription = wfs;
				if (base.GuidMappings.ContainsKey(new Guid(wfs.HistoryListId)))
				{
					Guid item = base.GuidMappings[new Guid(wfs.HistoryListId)];
					empty = item.ToString();
				}
				else
				{
					empty = string.Empty;
				}
				sP2013WorkflowSubscription.HistoryListId = empty;
			}
			if (!string.IsNullOrEmpty(wfs.TaskListId))
			{
				SP2013WorkflowSubscription sP2013WorkflowSubscription1 = wfs;
				if (base.GuidMappings.ContainsKey(new Guid(wfs.TaskListId)))
				{
					Guid guid = base.GuidMappings[new Guid(wfs.TaskListId)];
					str = guid.ToString();
				}
				else
				{
					str = string.Empty;
				}
				sP2013WorkflowSubscription1.TaskListId = str;
			}
			if (base.GuidMappings.ContainsKey(wfs.EventSourceId))
			{
				wfs.EventSourceId = base.GuidMappings[wfs.EventSourceId];
			}
			SP2013WorkflowDefinition associatedSP2013WorkflowDefinition = wfs.AssociatedSP2013WorkflowDefinition;
			if (base.GuidMappings.ContainsKey(associatedSP2013WorkflowDefinition.RestrictToScope))
			{
				associatedSP2013WorkflowDefinition.RestrictToScope = base.GuidMappings[associatedSP2013WorkflowDefinition.RestrictToScope];
			}
		}

		private string MapWorkflowData(SPWorkflow wf, SPList targetList, SPWorkflowAssociation wfa, string sParentWfa, object oTarget)
		{
			if (targetList == null)
			{
				Guid item = base.GuidMappings[new Guid(wf.ParentWebId)];
				wf.ParentWebId = item.ToString();
			}
			else
			{
				if (!base.WorkflowItemMappings.ContainsKey(wf.ParentItemGUID))
				{
					return "Guid mappings do not contain workflow parent item GUID. This is most commonly caused by the parent item failing to copy during the migration, corrupted workflows, or a corrupted item containing workflows.";
				}
				wf.ParentItemGUID = base.WorkflowItemMappings[wf.ParentItemGUID];
			}
			SPContentType sPContentType = oTarget as SPContentType;
			if (!string.IsNullOrEmpty(sParentWfa))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(sParentWfa);
				wf.TemplateId = xmlDocument.DocumentElement.Attributes["Id"].Value;
				if (xmlDocument.DocumentElement.Attributes["Modified"] != null && xmlDocument.DocumentElement.Attributes["Created"] != null)
				{
					XmlAttribute value = wf.XML.OwnerDocument.CreateAttribute("WFACreated");
					XmlAttribute xmlAttribute = wf.XML.OwnerDocument.CreateAttribute("WFAModified");
					value.Value = wfa.XML.Attributes["Created"].Value;
					xmlAttribute.Value = wfa.XML.Attributes["Modified"].Value;
					wf.XML.Attributes.Append(value);
					wf.XML.Attributes.Append(xmlAttribute);
				}
			}
			if (sPContentType != null)
			{
				XmlAttribute str = wf.XML.OwnerDocument.CreateAttribute("TargetContentTypeId");
				str.Value = sPContentType.ContentTypeID.ToString();
				wf.XML.Attributes.Append(str);
			}
			XmlAttribute itemOf = wf.XML.Attributes["ListId"];
			Guid guid = base.GuidMappings[new Guid(wf.XML.Attributes["ListId"].Value)];
			itemOf.Value = guid.ToString();
			string value1 = wf.XML.Attributes["TaskListId"].Value;
			XmlAttribute itemOf1 = wf.XML.Attributes["TaskListId"];
			Guid item1 = base.GuidMappings[new Guid(wf.XML.Attributes["TaskListId"].Value)];
			itemOf1.Value = item1.ToString();
			foreach (XmlNode xmlNodes in wf.XML.SelectNodes(".//Event"))
			{
				if (xmlNodes.Attributes["HostId"] != null && xmlNodes.Attributes["HostId"].Value == value1)
				{
					xmlNodes.Attributes["HostId"].Value = wf.XML.Attributes["TaskListId"].Value;
				}
				if (xmlNodes.Attributes["ContextObjectId"] != null)
				{
					xmlNodes.Attributes["ContextObjectId"].Value = base.WorkflowItemMappings[xmlNodes.Attributes["ContextObjectId"].Value];
				}
				if (xmlNodes.Attributes["ContextCollectionId"] == null || xmlNodes.Attributes["ContextCollectionId"].Value == wf.XML.Attributes["Id"].Value)
				{
					continue;
				}
				xmlNodes.Attributes["ContextCollectionId"].Value = base.WorkflowItemMappings[xmlNodes.Attributes["ContextCollectionId"].Value];
			}
			if (wf.XML.Attributes["Modifications"] != null && !string.IsNullOrEmpty(wf.XML.Attributes["Modifications"].Value))
			{
				XmlDocument xmlDocument1 = new XmlDocument();
				xmlDocument1.LoadXml(wf.XML.Attributes["Modifications"].Value);
				foreach (XmlNode xmlNodes1 in xmlDocument1.DocumentElement.SelectNodes(".//SubId | .//Id"))
				{
				}
			}
			string attributeValueAsString = wf.XML.GetAttributeValueAsString("Author");
			if (!string.IsNullOrEmpty(attributeValueAsString))
			{
				wf.XML.Attributes["Author"].Value = base.MapPrincipal(attributeValueAsString);
			}
			return null;
		}

		private void ParseOpResult(OperationReportingResult opResult, LogItem logItem)
		{
			logItem.Status = ActionOperationStatus.Completed;
			if (opResult.ErrorOccured)
			{
				logItem.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
				logItem.Details = opResult.GetAllErrorsAsString;
				logItem.Status = ActionOperationStatus.Warning;
				return;
			}
			if (opResult.WarningOccured)
			{
				logItem.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
				logItem.Details = opResult.GetAllWarningsAsString;
				logItem.Status = ActionOperationStatus.Warning;
				return;
			}
			if (opResult.HasInformation)
			{
				logItem.Information = Metalogix.SharePoint.Adapters.Properties.Resources.PleaseReviewDetails;
				logItem.Details = opResult.AllInformationAsString;
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 1)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			if (oParams[0] is SPContentType)
			{
				this.CopyWorkflowAssociations(oParams[0] as SPContentType, oParams[1] as SPContentType, oParams[2] as SPList);
				return;
			}
			if (!(oParams[0] is SPWeb))
			{
				this.CopyWorkflowAssociations(oParams[0] as SPList, oParams[1] as SPList, oParams[2] as SPWeb);
				this.CopySP2013Workflows(oParams[0] as SPList, oParams[1] as SPList, oParams[2] as SPWeb);
				return;
			}
			if ((int)oParams.Length == 1)
			{
				this.ActivateReusableWorkflowTemplates(oParams[0] as SPWeb);
				return;
			}
			this.CopyWorkflowAssociations(oParams[0] as SPWeb, oParams[1] as SPWeb);
			this.CopySP2013Workflows(oParams[0] as SPWeb, oParams[1] as SPWeb);
		}

		private void SetUserAccountIDAndDisplayName(XElement displayName, XElement accountId, string originalUserAccount, string mappedUserAccount, bool isTargetSharePointOnline)
		{
			accountId.SetValue(mappedUserAccount);
			if (displayName != null)
			{
				ListSummaryItem mapListSummaryItem = SPGlobalMappings.GetMapListSummaryItem(originalUserAccount, isTargetSharePointOnline);
				if (mapListSummaryItem == null || mapListSummaryItem.Target == null || mapListSummaryItem.Target.Tag == null)
				{
					return;
				}
				SPUser tag = mapListSummaryItem.Target.Tag as SPUser;
				if (tag == null)
				{
					return;
				}
				displayName.SetValue(tag.Name);
			}
		}

		private void UpdateStatusColumn(SPWorkflow wf, string sColName, SPList targetList)
		{
			Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(wf.XML, "StatusColumnName", sColName);
			if (targetList != null)
			{
				if (wf.XML.Attributes["AddStatusFieldToViews"] == null)
				{
					Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(wf.XML, "AddStatusFieldToViews", true.ToString());
				}
				if (wf.XML.Attributes["StatusColumnTextName"] == null)
				{
					Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(wf.XML, "StatusColumnTextName", sColName);
				}
				targetList.RefreshFields(true);
				bool flag = false;
				foreach (SPField field in targetList.Fields)
				{
					if (field.Name != sColName)
					{
						continue;
					}
					Metalogix.SharePoint.Adapters.Utils.AddOrUpdateXmlAttribute(wf.XML, "StatusColumnName", field.FieldXML.Attributes["ColName"].Value);
					flag = true;
					break;
				}
				this.UpdateXmlWithFieldViewEntries(wf.XML, targetList.ID);
				if (!flag)
				{
					XmlAttribute xmlAttribute = wf.XML.OwnerDocument.CreateAttribute("CreateColumn");
					xmlAttribute.Value = "true";
					wf.XML.Attributes.Append(xmlAttribute);
				}
			}
		}

		private void UpdateXmlWithFieldViewEntries(XmlNode ndWfXml, string sListId)
		{
			if (base.WorkflowViewMappings.ContainsKey(sListId) && ndWfXml.Attributes["StatusColumnTextName"] != null && base.WorkflowViewMappings[sListId].ContainsKey(ndWfXml.Attributes["StatusColumnTextName"].Value) && ndWfXml.Attributes["StatusColumnViews"] == null)
			{
				XmlAttribute item = ndWfXml.OwnerDocument.CreateAttribute("StatusColumnViews");
				item.Value = base.WorkflowViewMappings[sListId][ndWfXml.Attributes["StatusColumnTextName"].Value];
				ndWfXml.Attributes.Append(item);
			}
		}
	}
}