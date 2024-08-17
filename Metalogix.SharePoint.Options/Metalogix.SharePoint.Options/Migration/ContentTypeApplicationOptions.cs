using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.SharePoint;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration
{
	public class ContentTypeApplicationOptions : IXmlable
	{
		private string m_sContentTypeName;

		private bool m_bMakeDefault;

		private IFilterExpression m_mapItemsFilter;

		public string ContentTypeName
		{
			get
			{
				return this.m_sContentTypeName;
			}
			set
			{
				this.m_sContentTypeName = value;
			}
		}

		public bool MakeDefault
		{
			get
			{
				return this.m_bMakeDefault;
			}
			set
			{
				this.m_bMakeDefault = value;
			}
		}

		public IFilterExpression MapItemsFilter
		{
			get
			{
				return this.m_mapItemsFilter;
			}
			set
			{
				this.m_mapItemsFilter = value;
			}
		}

		public ContentTypeApplicationOptions(string sContentTypeName)
		{
			this.m_sContentTypeName = sContentTypeName;
		}

		public ContentTypeApplicationOptions(XmlNode node)
		{
			this.FromXML(node);
		}

		public ContentTypeApplicationOptions Clone()
		{
			return new ContentTypeApplicationOptions(XmlUtility.StringToXmlNode(this.ToXML()));
		}

		public void FromXML(XmlNode node)
		{
			XmlNode xmlNodes = node.SelectSingleNode(".//ContentTypeApplicationOptions");
			xmlNodes = (xmlNodes == null ? node : xmlNodes);
			XmlAttribute itemOf = xmlNodes.Attributes["ContentTypeName"];
			if (itemOf != null)
			{
				this.ContentTypeName = itemOf.Value;
			}
			itemOf = xmlNodes.Attributes["MakeDefault"];
			if (itemOf != null)
			{
				this.MakeDefault = bool.Parse(itemOf.Value);
			}
			XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./MapItemsFilter");
			if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
			{
				this.MapItemsFilter = FilterExpression.ParseExpression(xmlNodes1.ChildNodes[0]);
			}
		}

		public bool ItemIsMapped(SPListItem item)
		{
			if (this.MapItemsFilter == null)
			{
				return false;
			}
			return this.MapItemsFilter.Evaluate(item, new CompareDatesInUtc());
		}

		public string ToXML()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			this.ToXML(xmlTextWriter);
			xmlTextWriter.Flush();
			return stringBuilder.ToString();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ContentTypeApplicationOptions");
			xmlWriter.WriteAttributeString("ContentTypeName", this.ContentTypeName);
			xmlWriter.WriteAttributeString("MakeDefault", this.MakeDefault.ToString());
			if (this.MapItemsFilter != null)
			{
				xmlWriter.WriteStartElement("MapItemsFilter");
				this.MapItemsFilter.ToXML(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
	}
}