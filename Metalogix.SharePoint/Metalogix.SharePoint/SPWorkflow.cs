using Metalogix.Data;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWorkflow : IXmlable
	{
		private XmlNode m_XML;

		private string m_sTemplateId;

		private string m_sParentItemId;

		private string m_sParentListId;

		public string ID
		{
			get
			{
				return this.XML.Attributes["Id"].Value;
			}
			set
			{
				this.XML.Attributes["Id"].Value = value;
			}
		}

		public string InternalState
		{
			get
			{
				return this.XML.Attributes["InternalState"].Value;
			}
			set
			{
				this.XML.Attributes["InternalState"].Value = value;
			}
		}

		public bool IsRunning
		{
			get
			{
				bool flag;
				flag = (((SPWorkflowState)Enum.Parse(typeof(SPWorkflowState), this.XML.Attributes["InternalState"].Value.ToString()) & SPWorkflowState.Running) == SPWorkflowState.None ? false : true);
				return flag;
			}
		}

		public string ParentItemGUID
		{
			get
			{
				return this.XML.Attributes["ItemGUID"].Value;
			}
			set
			{
				this.XML.Attributes["ItemGUID"].Value = value;
			}
		}

		public string ParentListId
		{
			get
			{
				return this.m_sParentListId;
			}
		}

		public string ParentWebId
		{
			get
			{
				return this.XML.Attributes["WebId"].Value;
			}
			set
			{
				this.XML.Attributes["WebId"].Value = value;
			}
		}

		public string TemplateId
		{
			get
			{
				return this.XML.Attributes["TemplateId"].Value;
			}
			set
			{
				this.XML.Attributes["TemplateId"].Value = value;
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

		public SPWorkflow(XmlNode ndWfaXml)
		{
			this.FromXML(ndWfaXml);
		}

		public SPWorkflow Clone()
		{
			return new SPWorkflow(this.XML);
		}

		public void FromXML(XmlNode ndXml)
		{
			this.m_sParentItemId = ndXml.Attributes["ItemGUID"].Value;
			this.m_sParentListId = ndXml.Attributes["ListId"].Value;
			this.m_sTemplateId = ndXml.Attributes["TemplateId"].Value;
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