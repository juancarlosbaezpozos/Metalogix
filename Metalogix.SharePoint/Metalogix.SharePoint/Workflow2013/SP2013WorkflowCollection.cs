using Metalogix.Core.OperationLog;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Common.Enums;
using Metalogix.SharePoint.Common.Workflow2013;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Workflow2013
{
	public class SP2013WorkflowCollection
	{
		private readonly ISP2013WorkflowAdapter adapter;

		private readonly bool isList;

		private OperationReportingResultObject<List<SP2013WorkflowSubscription>> _2013Workflows;

		public SP2013WorkflowCollection(SharePointAdapter adapter, SP2013WorkflowScopeType scopeType)
		{
			this.adapter = adapter.Reader as ISP2013WorkflowAdapter ?? adapter.Writer as ISP2013WorkflowAdapter;
			this.isList = scopeType == SP2013WorkflowScopeType.List;
		}

		private void AddScopeTypeToConfigXml(ref string configurationXml)
		{
			if (!string.IsNullOrEmpty(configurationXml))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(configurationXml);
				if ((xmlNode.OwnerDocument == null ? false : xmlNode.Attributes != null))
				{
					XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(ConfigXMLAttributes.WorkflowScopeType.ToString());
					xmlAttribute.Value = (this.isList ? 1.ToString() : 0.ToString());
					xmlNode.Attributes.Append(xmlAttribute);
				}
				configurationXml = xmlNode.OuterXml;
			}
		}

		public OperationReportingResultObject<bool> DeleteSP2013Workflows(string configurationXml)
		{
			this.AddScopeTypeToConfigXml(ref configurationXml);
			OperationReportingResultObject<bool> operationReportingResultObject = new OperationReportingResultObject<bool>(this.adapter.DeleteSP2013Workflows(configurationXml));
			if (!string.IsNullOrEmpty(operationReportingResultObject.ObjectXml))
			{
				bool flag = true;
				operationReportingResultObject.ResultObject = operationReportingResultObject.ObjectXml.Equals(flag.ToString());
			}
			return operationReportingResultObject;
		}

		public OperationReportingResultObject<List<SP2013WorkflowSubscription>> GetSP2013Workflows(string configurationXml)
		{
			if ((this._2013Workflows == null ? true : this.IsForceRefresh(configurationXml)))
			{
				this.AddScopeTypeToConfigXml(ref configurationXml);
				OperationReportingResultObject<List<SP2013WorkflowSubscription>> operationReportingResultObject = new OperationReportingResultObject<List<SP2013WorkflowSubscription>>(this.adapter.GetSP2013Workflows(configurationXml));
				if (!string.IsNullOrEmpty(operationReportingResultObject.ObjectXml))
				{
					operationReportingResultObject.ResultObject = operationReportingResultObject.ObjectXml.Deserialize<List<SP2013WorkflowSubscription>>();
				}
				this._2013Workflows = operationReportingResultObject;
			}
			return this._2013Workflows;
		}

		private bool IsForceRefresh(string configurationXml)
		{
			bool attributeValueAsBoolean = false;
			if (!string.IsNullOrEmpty(configurationXml))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(configurationXml);
				attributeValueAsBoolean = xmlNode.GetAttributeValueAsBoolean(ConfigXMLAttributes.ForceRefresh.ToString());
			}
			return attributeValueAsBoolean;
		}

		public bool IsWorkflowServicesInstanceAvailable()
		{
			bool flag;
			if (this.adapter != null)
			{
				bool flag1 = false;
				bool.TryParse(this.adapter.IsWorkflowServicesInstanceAvailable(), out flag1);
				flag = flag1;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public OperationReportingResultObject<bool> MigrateSP2013Workflows(string configurationXml)
		{
			this.AddScopeTypeToConfigXml(ref configurationXml);
			OperationReportingResultObject<bool> operationReportingResultObject = new OperationReportingResultObject<bool>(this.adapter.MigrateSP2013Workflows(configurationXml));
			if (!string.IsNullOrEmpty(operationReportingResultObject.ObjectXml))
			{
				bool flag = true;
				operationReportingResultObject.ResultObject = operationReportingResultObject.ObjectXml.Equals(flag.ToString());
			}
			return operationReportingResultObject;
		}
	}
}