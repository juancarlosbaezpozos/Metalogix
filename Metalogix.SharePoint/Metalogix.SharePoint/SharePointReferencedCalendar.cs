using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SharePointReferencedCalendar : ReferencedCalendar
	{
		public string CalendarUrl
		{
			get
			{
				if (this.m_referenceXml.Attributes["CalendarUrl"] == null)
				{
					throw new Exception("Attribute not present in the XML.");
				}
				return this.m_referenceXml.Attributes["CalendarUrl"].Value;
			}
			set
			{
				if (this.m_referenceXml.Attributes["CalendarUrl"] == null)
				{
					throw new Exception("Attribute not present in the XML.");
				}
				this.m_referenceXml.Attributes["CalendarUrl"].Value = value;
			}
		}

		public string ListFormUrl
		{
			get
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ListFormUrl"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				string value = this.m_referenceXml.ChildNodes[0].Attributes["ListFormUrl"].Value;
				return value;
			}
			set
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ListFormUrl"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				this.m_referenceXml.ChildNodes[0].Attributes["ListFormUrl"].Value = value;
			}
		}

		public string ListID
		{
			get
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ListId"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				string value = this.m_referenceXml.ChildNodes[0].Attributes["ListId"].Value;
				return value;
			}
			set
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ListId"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				this.m_referenceXml.ChildNodes[0].Attributes["ListId"].Value = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_referenceXml.Attributes["Name"] == null)
				{
					throw new Exception("Attribute not present in the XML.");
				}
				return this.m_referenceXml.Attributes["Name"].Value;
			}
		}

		public string ViewID
		{
			get
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ViewId"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				string value = this.m_referenceXml.ChildNodes[0].Attributes["ViewId"].Value;
				return value;
			}
			set
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["ViewId"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				this.m_referenceXml.ChildNodes[0].Attributes["ViewId"].Value = value;
			}
		}

		public string WebUrl
		{
			get
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["WebUrl"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				string value = this.m_referenceXml.ChildNodes[0].Attributes["WebUrl"].Value;
				return value;
			}
			set
			{
				if ((this.m_referenceXml.ChildNodes.Count == 0 ? true : this.m_referenceXml.ChildNodes[0].Attributes["WebUrl"] == null))
				{
					throw new Exception("Attribute not present in the XML.");
				}
				this.m_referenceXml.ChildNodes[0].Attributes["WebUrl"].Value = value;
			}
		}

		public SharePointReferencedCalendar(XmlNode referenceXml) : base(referenceXml)
		{
		}
	}
}