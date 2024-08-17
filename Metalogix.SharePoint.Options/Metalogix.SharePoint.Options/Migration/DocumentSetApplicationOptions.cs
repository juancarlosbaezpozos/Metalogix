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
	public class DocumentSetApplicationOptions : IXmlable
	{
		private string m_sContentTypeName;

		private string m_sDocSetName;

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

		public string DocSetName
		{
			get
			{
				return this.m_sDocSetName;
			}
			set
			{
				this.m_sDocSetName = value;
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

		public DocumentSetApplicationOptions(string sContentTypeName)
		{
			this.m_sContentTypeName = sContentTypeName;
		}

		public DocumentSetApplicationOptions(XmlNode node)
		{
			this.FromXML(node);
		}

		public DocumentSetApplicationOptions Clone()
		{
			return new DocumentSetApplicationOptions(XmlUtility.StringToXmlNode(this.ToXML()));
		}

		public void FromXML(XmlNode node)
		{
			XmlNode xmlNodes = node.SelectSingleNode(".//DocumentSetApplicationOptions");
			xmlNodes = (xmlNodes == null ? node : xmlNodes);
			XmlAttribute itemOf = xmlNodes.Attributes["ContentTypeName"];
			if (itemOf != null)
			{
				this.ContentTypeName = itemOf.Value;
			}
			itemOf = xmlNodes.Attributes["DocSetName"];
			if (itemOf != null)
			{
				this.DocSetName = itemOf.Value;
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
			xmlWriter.WriteStartElement("DocumentSetApplicationOptions");
			xmlWriter.WriteAttributeString("ContentTypeName", this.ContentTypeName);
			xmlWriter.WriteAttributeString("DocSetName", this.m_sDocSetName);
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