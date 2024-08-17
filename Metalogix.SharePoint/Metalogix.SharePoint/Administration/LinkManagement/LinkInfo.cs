using System;
using System.Xml;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkInfo
	{
		private readonly string m_sLinkName;

		private string m_sLinkURL;

		public string Name
		{
			get
			{
				return this.m_sLinkName;
			}
		}

		public string URL
		{
			get
			{
				return this.m_sLinkURL;
			}
			set
			{
				this.m_sLinkURL = value;
			}
		}

		private LinkInfo()
		{
		}

		public LinkInfo(string sLinkName, string sLinkURL)
		{
			this.m_sLinkName = sLinkName;
			this.m_sLinkURL = sLinkURL;
		}

		public void ToXml(XmlWriter writer)
		{
			this.ToXml(writer, "LinkInfo");
		}

		public void ToXml(XmlWriter writer, string sElementName)
		{
			if (writer != null)
			{
				writer.WriteStartElement(sElementName);
				writer.WriteStartElement("Name");
				writer.WriteString(this.m_sLinkName);
				writer.WriteEndElement();
				writer.WriteStartElement("URL");
				writer.WriteString(this.m_sLinkURL);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		private struct XmlNames
		{
			public const string Name = "Name";

			public const string URL = "URL";
		}
	}
}