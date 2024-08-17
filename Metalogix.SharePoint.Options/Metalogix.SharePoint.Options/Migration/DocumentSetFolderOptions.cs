using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration
{
	public class DocumentSetFolderOptions : IXmlable
	{
		private string m_sContentTypeName;

		private FilterExpression m_folderFilter;

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

		public FilterExpression FolderFilter
		{
			get
			{
				return this.m_folderFilter;
			}
			set
			{
				this.m_folderFilter = value;
			}
		}

		public DocumentSetFolderOptions(string sContentTypeName)
		{
			this.m_sContentTypeName = sContentTypeName;
		}

		public DocumentSetFolderOptions(XmlNode node)
		{
			this.FromXML(node);
		}

		public DocumentSetFolderOptions Clone()
		{
			return new DocumentSetFolderOptions(XmlUtility.StringToXmlNode(this.ToXML()));
		}

		public void FromXML(XmlNode node)
		{
			XmlNode xmlNodes = node.SelectSingleNode(".//FolderToDocumentSetOptions");
			xmlNodes = (xmlNodes == null ? node : xmlNodes);
			XmlAttribute itemOf = xmlNodes.Attributes["ContentTypeName"];
			if (itemOf != null)
			{
				this.ContentTypeName = itemOf.Value;
			}
			XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./FolderFilter");
			if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
			{
				this.FolderFilter = (FilterExpression)FilterExpression.ParseExpression(xmlNodes1.ChildNodes[0]);
			}
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
			xmlWriter.WriteStartElement("FolderToDocumentSetOptions");
			xmlWriter.WriteAttributeString("ContentTypeName", this.ContentTypeName);
			if (this.FolderFilter != null)
			{
				xmlWriter.WriteStartElement("FolderFilter");
				this.FolderFilter.ToXML(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
	}
}