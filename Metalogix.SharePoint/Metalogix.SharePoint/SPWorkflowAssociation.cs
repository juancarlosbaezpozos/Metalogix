using Metalogix.Data;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWorkflowAssociation : IXmlable
	{
		private const string SP2007_APPROVAL_WORKFLOW_TEMPLATEID = "c6964bff-bf8d-41ac-ad5e-b61ec111731c";

		private const string SP2007_DISPOSITION_APPROVAL_TEMPLATEID = "dd19a800-37c1-43c0-816d-f8eb5f4a4145";

		private const string SP2007_COLLECT_SIGNATURES_TEMPLATEID = "2f213931-3b93-4f81-b021-3022434a3114";

		private const string SP2007_COLLECT_FEEDBACK_TEMPLATEID = "46c389a4-6e18-476c-aa17-289b0c79fb8f";

		private const string SP2007_TREE_STATE_WORKFLOW_TEMPLATEID = "c6964bff-bf8d-41ac-ad5e-b61ec111731a";

		private bool m_bSharePointDesignerWorkflow = false;

		private bool _isNintexWorkflow = false;

		private XmlNode m_XML;

		private string m_sScope;

		private SPWorkflowAssociationCollection m_sParentCollection;

		private string[] m_Sp2007OOBWorkflowTemplateIds = new string[] { "c6964bff-bf8d-41ac-ad5e-b61ec111731c", "dd19a800-37c1-43c0-816d-f8eb5f4a4145", "2f213931-3b93-4f81-b021-3022434a3114", "46c389a4-6e18-476c-aa17-289b0c79fb8f", "c6964bff-bf8d-41ac-ad5e-b61ec111731a" };

		private SPWorkflowCollection m_Workflows = null;

		public string BaseId
		{
			get
			{
				string value;
				if (this.XML.Attributes["BaseId"] == null)
				{
					value = null;
				}
				else
				{
					value = this.XML.Attributes["BaseId"].Value;
				}
				return value;
			}
		}

		public DateTime CreatedDate
		{
			get
			{
				DateTime dateTime = Utils.ParseDateAsUtc(this.XML.Attributes["Created"].Value);
				return dateTime;
			}
		}

		public string HistoryListId
		{
			get
			{
				string str;
				str = (this.XML.Attributes["HistoryListId"] == null ? "" : this.XML.Attributes["HistoryListId"].Value);
				return str;
			}
			set
			{
				this.XML.Attributes["HistoryListId"].Value = value;
			}
		}

		public string ID
		{
			get
			{
				return this.XML.Attributes["Id"].Value;
			}
		}

		public bool IsNintexWorkflow
		{
			get
			{
				return this._isNintexWorkflow;
			}
		}

		public bool IsSharePoint2007OOBWorkflowAssociation
		{
			get
			{
				bool flag;
				if (this.XML.Attributes["BaseTemplate"] != null)
				{
					string[] mSp2007OOBWorkflowTemplateIds = this.m_Sp2007OOBWorkflowTemplateIds;
					int num = 0;
					while (num < (int)mSp2007OOBWorkflowTemplateIds.Length)
					{
						string str = mSp2007OOBWorkflowTemplateIds[num];
						if (!this.XML.Attributes["BaseTemplate"].Value.Equals(str, StringComparison.OrdinalIgnoreCase))
						{
							num++;
						}
						else
						{
							flag = true;
							return flag;
						}
					}
				}
				flag = false;
				return flag;
			}
		}

		public bool IsSharePointDesignerWorkflow
		{
			get
			{
				return this.m_bSharePointDesignerWorkflow;
			}
		}

		public string Name
		{
			get
			{
				return this.XML.Attributes["Name"].Value;
			}
		}

		public SPWorkflowAssociationCollection ParentCollection
		{
			get
			{
				return this.m_sParentCollection;
			}
		}

		public string Scope
		{
			get
			{
				return this.m_sScope;
			}
		}

		public string StatusColumn
		{
			get
			{
				return this.XML.Attributes["StatusColumn"].Value;
			}
		}

		public string TasksListId
		{
			get
			{
				string str;
				str = (this.XML.Attributes["TaskListId"] == null ? "" : this.XML.Attributes["TaskListId"].Value);
				return str;
			}
			set
			{
				this.XML.Attributes["TaskListId"].Value = value;
			}
		}

		public SPWorkflowCollection Workflows
		{
			get
			{
				if (this.m_Workflows == null)
				{
					this.m_Workflows = new SPWorkflowCollection(this);
				}
				return this.m_Workflows;
			}
		}

		public XmlNode XML
		{
			get
			{
				return this.m_XML;
			}
			set
			{
				this.m_XML = value;
			}
		}

		public SPWorkflowAssociation(XmlNode ndWfaXml, SPWorkflowAssociationCollection parentCollection, string sScope)
		{
			this.m_sScope = sScope;
			this.m_sParentCollection = parentCollection;
			this.FromXML(ndWfaXml);
		}

		public SPWorkflowAssociation Clone()
		{
			return new SPWorkflowAssociation(this.XML, this.ParentCollection, this.Scope);
		}

		public void FromXML(XmlNode ndXml)
		{
			if ((ndXml.Attributes["BaseTemplate"] == null ? true : ndXml.Attributes["Is2010SharePointDesignerWorkflow"] != null))
			{
				this.m_bSharePointDesignerWorkflow = true;
			}
			this._isNintexWorkflow = ndXml.GetAttributeValueAsBoolean("IsNintex");
			XmlAttribute scope = ndXml.OwnerDocument.CreateAttribute("Scope");
			scope.Value = this.Scope;
			ndXml.Attributes.Append(scope);
			this.XML = ndXml;
		}

		public string ToXML()
		{
			string str;
			StringWriter stringWriter = new StringWriter();
			try
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				try
				{
					this.ToXML(xmlTextWriter);
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						((IDisposable)xmlTextWriter).Dispose();
					}
				}
				str = stringWriter.ToString();
			}
			finally
			{
				if (stringWriter != null)
				{
					((IDisposable)stringWriter).Dispose();
				}
			}
			return str;
		}

		public virtual void ToXML(XmlWriter xmlWriter)
		{
			if (this.m_XML != null)
			{
				xmlWriter.WriteRaw(this.m_XML.OuterXml);
			}
		}
	}
}