using Metalogix.Core.OperationLog;
using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Workflow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration.Nintex.Mappings
{
	public class WorkflowDataMapper : IMapper
	{
		private const string GuidPattern = "^[{(]?[0-9a-zA-Z]{8}[-]?([0-9a-zA-Z]{4}[-]?){3}[0-9a-zA-Z]{12}[)}]?$";

		public Dictionary<Guid, Guid> GuidMappings
		{
			get;
			set;
		}

		public Metalogix.SharePoint.Migration.LinkCorrector LinkCorrector
		{
			get;
			set;
		}

		public HashSet<Guid> MissingGuids
		{
			get;
			private set;
		}

		public SPWeb TargetWeb
		{
			get;
			set;
		}

		public WorkflowDataMapper()
		{
			this.GuidMappings = new Dictionary<Guid, Guid>();
			this.MissingGuids = new HashSet<Guid>();
			this.LinkCorrector = new Metalogix.SharePoint.Migration.LinkCorrector();
		}

		private void MapAttributes(OperationReporting opReport, List<WorkflowActivityAttribute> workflowActivityAttributes, XmlNode activityNode, string activityName)
		{
			foreach (WorkflowActivityAttribute workflowActivityAttribute in workflowActivityAttributes)
			{
				string str = (workflowActivityAttribute.Operation.Equals(WorkflowActivityAttributeOperation.UrlMapping) ? string.Format(".//Name[text()='{0}']/following-sibling::Value/PrimitiveValue/Value/string", workflowActivityAttribute.Name) : string.Format(".//{0}", workflowActivityAttribute.Name));
				XmlNodeList xmlNodeLists = activityNode.SelectNodes(str);
				if (xmlNodeLists == null)
				{
					continue;
				}
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					switch (workflowActivityAttribute.Operation)
					{
						case WorkflowActivityAttributeOperation.Guid:
						{
							this.MapGuid(xmlNodes, opReport, str, activityName);
							continue;
						}
						case WorkflowActivityAttributeOperation.UserMapping:
						{
							this.MapUser(xmlNodes, opReport, activityName);
							continue;
						}
						case WorkflowActivityAttributeOperation.UrlMapping:
						{
							this.MapUrl(xmlNodes, opReport, activityName);
							continue;
						}
						default:
						{
							continue;
						}
					}
				}
			}
		}

		private void MapGuid(XmlNode node, OperationReporting opReport, string xpath, string activityName = null)
		{
			if (!Regex.IsMatch(node.InnerText, "^[{(]?[0-9a-zA-Z]{8}[-]?([0-9a-zA-Z]{4}[-]?){3}[0-9a-zA-Z]{12}[)}]?$"))
			{
				return;
			}
			Guid guid = new Guid(node.InnerText);
			if (this.GuidMappings.ContainsKey(guid))
			{
				Guid item = this.GuidMappings[guid];
				node.InnerText = item.ToString("D");
				return;
			}
			if (!this.MissingGuids.Contains(guid))
			{
				this.MissingGuids.Add(guid);
			}
			if (!string.IsNullOrEmpty(activityName))
			{
				string str = string.Format("Activity Name: {0}, ListId: {1}", activityName, guid);
				opReport.LogWarning(str, string.Empty);
				return;
			}
			char[] chrArray = new char[] { '/' };
			string str1 = string.Format("{0}: {1}", xpath.Trim(chrArray), guid);
			opReport.LogInformation(str1, string.Empty);
		}

		private void MapGuids(XmlDocument doc, OperationReporting opReport, string xpath)
		{
			if (doc.DocumentElement != null)
			{
				XmlNodeList xmlNodeLists = doc.DocumentElement.SelectNodes(xpath);
				if (xmlNodeLists != null)
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						this.MapGuid(xmlNodes, opReport, xpath, null);
					}
				}
			}
		}

		private void MapUrl(XmlNode node, OperationReporting opReport, string activityName)
		{
			string innerText = node.InnerText;
			if (!string.IsNullOrEmpty(innerText))
			{
				string str = this.LinkCorrector.CorrectUrl(innerText);
				if (!innerText.Equals(str, StringComparison.InvariantCultureIgnoreCase))
				{
					node.InnerText = str;
					return;
				}
				string str1 = string.Format("Activity Name: {0}, Url: {1}", activityName, innerText);
				opReport.LogWarning(str1, string.Empty);
			}
		}

		private void MapUser(XmlNode node, OperationReporting opReport, string activityName)
		{
			string innerText = node.InnerText;
			if (!string.IsNullOrEmpty(innerText))
			{
				string str = this.MapUser(innerText);
				if (!string.IsNullOrEmpty(str))
				{
					node.InnerText = str;
					return;
				}
				string str1 = string.Format("Activity Name: {0}, User: {1}", activityName, innerText);
				opReport.LogWarning(str1, string.Empty);
			}
		}

		private string MapUser(string userName)
		{
			string empty = string.Empty;
			ListSummaryItem mapListSummaryItem = SPGlobalMappings.GetMapListSummaryItem(userName, this.TargetWeb.Adapter.SharePointVersion.IsSharePointOnline);
			if (mapListSummaryItem != null && this.TargetWeb.SiteUsers.Contains(mapListSummaryItem.Target.Target))
			{
				empty = mapListSummaryItem.Target.Target;
			}
			return empty;
		}

		private void MapWorkflowDataXml(XmlDocument xdoc, OperationReporting opReport)
		{
			try
			{
				XmlNodeList xmlNodeLists = xdoc.SelectNodes("//Configuration");
				if (xmlNodeLists != null && xmlNodeLists.Count > 0)
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						string empty = string.Empty;
						try
						{
							(new XmlDocument()).LoadXml(xmlNodes.OuterXml);
							XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//ServerInfo/ClassName");
							if (xmlNodes1 != null && WorkflowRespository.Activities.NintexOnlineWorkflowActivityNames.Contains(xmlNodes1.InnerText))
							{
								WorkflowActivity item = WorkflowRespository.Activities[xmlNodes1.InnerText];
								XmlNode xmlNodes2 = xmlNodes.SelectSingleNode(".//Name");
								if (xmlNodes2 != null)
								{
									empty = xmlNodes2.InnerText;
								}
								this.MapAttributes(opReport, item.Attributes, xmlNodes, empty);
							}
						}
						catch (Exception exception)
						{
							opReport.LogError(exception, string.Format("An error occured while replacing Guid in activity '{0}'", empty));
						}
					}
				}
			}
			catch (Exception exception1)
			{
				opReport.LogError(exception1, "An error occured while replacing Guid in Nintex nwp file");
			}
		}

		public HashSet<Guid> UpdateFile(string file, OperationReporting opReport)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(file);
				this.MapGuids(xmlDocument, opReport, "//ListId");
				this.MapGuids(xmlDocument, opReport, "//FieldId");
				this.MapWorkflowDataXml(xmlDocument, opReport);
				xmlDocument.Save(file);
			}
			catch (Exception exception)
			{
				opReport.LogError(exception, "An error occurred while replacing GUID in the activities.");
			}
			return this.MissingGuids;
		}
	}
}