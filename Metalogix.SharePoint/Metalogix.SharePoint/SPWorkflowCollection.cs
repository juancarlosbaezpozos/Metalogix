using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWorkflowCollection : SerializableList<SPWorkflow>
	{
		private SPWorkflowAssociation m_ParentAssociation;

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsSet
		{
			get
			{
				return false;
			}
		}

		public override SPWorkflow this[SPWorkflow key]
		{
			get
			{
				return this[key];
			}
		}

		public SPWorkflowAssociation ParentAssociation
		{
			get
			{
				return this.m_ParentAssociation;
			}
		}

		public SPWorkflowCollection()
		{
		}

		public SPWorkflowCollection(XmlNode node)
		{
			this.FromXML(node);
		}

		public SPWorkflowCollection(SPWorkflowAssociation parentAssociation)
		{
			this.m_ParentAssociation = parentAssociation;
		}

		public OperationReportingResult FetchData()
		{
			string workflows = null;
			if (this.ParentAssociation.ParentCollection.ParentList == null)
			{
				workflows = (this.ParentAssociation.ParentCollection.ParentWeb == null ? this.ParentAssociation.ParentCollection.ParentContentType.ParentCollection.ParentWeb.Adapter.Reader.GetWorkflows(this.ParentAssociation.ID, 0) : this.ParentAssociation.ParentCollection.ParentWeb.Adapter.Reader.GetWorkflows(this.ParentAssociation.ID, 0));
			}
			else
			{
				workflows = this.ParentAssociation.ParentCollection.ParentList.Adapter.Reader.GetWorkflows(this.ParentAssociation.ID, 0);
			}
			OperationReportingResult operationReportingResult = new OperationReportingResult(workflows);
			if (!string.IsNullOrEmpty(operationReportingResult.ObjectXml))
			{
				base.FromXML(operationReportingResult.ObjectXml);
			}
			return operationReportingResult;
		}

		public override void FromXML(XmlNode xmlNode)
		{
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//Workflow"))
			{
				this.Add(new SPWorkflow(xmlNodes));
			}
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Workflows");
			foreach (SPWorkflow sPWorkflow in this)
			{
				xmlWriter.WriteRaw(sPWorkflow.XML.OuterXml);
			}
			xmlWriter.WriteEndElement();
		}
	}
}