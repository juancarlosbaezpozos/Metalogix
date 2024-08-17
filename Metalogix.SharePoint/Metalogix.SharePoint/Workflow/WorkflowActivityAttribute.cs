using System;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Workflow
{
	public class WorkflowActivityAttribute
	{
		private string name;

		private WorkflowActivityAttributeOperation operation;

		private string _innerXmlNodeName;

		[XmlAttribute]
		public string InnerXmlNodeName
		{
			get
			{
				return this._innerXmlNodeName;
			}
			set
			{
				this._innerXmlNodeName = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute]
		public WorkflowActivityAttributeOperation Operation
		{
			get
			{
				return this.operation;
			}
			set
			{
				this.operation = value;
			}
		}

		internal WorkflowActivityAttribute(string attributeName, WorkflowActivityAttributeOperation attributeOperation, string innerXmlNode) : this(attributeName, attributeOperation)
		{
			this._innerXmlNodeName = innerXmlNode;
		}

		internal WorkflowActivityAttribute(string attributeName, WorkflowActivityAttributeOperation attributeOperation)
		{
			this.name = attributeName;
			this.operation = attributeOperation;
		}

		internal WorkflowActivityAttribute()
		{
			this.name = string.Empty;
			this.operation = WorkflowActivityAttributeOperation.Undefined;
		}
	}
}