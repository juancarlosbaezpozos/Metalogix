using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebPartV3 : SPWebPart
	{
		public override string Assembly
		{
			get
			{
				if (this.m_sAssembly == null)
				{
					XmlNode xmlNodes = this.m_Xml.SelectSingleNode("./*[local-name() = 'metaData']/*[local-name() = 'type']/@name");
					if (xmlNodes != null)
					{
						int num = xmlNodes.InnerText.IndexOf(",");
						this.m_sAssembly = (num >= 0 ? xmlNodes.InnerText.Substring(num + 1) : xmlNodes.InnerText);
					}
				}
				return this.m_sAssembly;
			}
		}

		public override string CatalogIconImageUrl
		{
			get
			{
				return base["CatalogIconImageUrl"];
			}
			set
			{
				base["CatalogIconImageUrl"] = value;
			}
		}

		public override string HelpUrl
		{
			get
			{
				return base["HelpUrl"];
			}
			set
			{
				base["HelpUrl"] = value;
			}
		}

		public override string TitleIconUrl
		{
			get
			{
				return base["TitleIconImageUrl"];
			}
			set
			{
				base["TitleIconImageUrl"] = value;
			}
		}

		public override string TitleUrl
		{
			get
			{
				return base["TitleUrl"];
			}
			set
			{
				base["TitleUrl"] = value;
			}
		}

		public override string TypeName
		{
			get
			{
				if (this.m_sTypeName == null)
				{
					XmlNode xmlNodes = this.m_Xml.SelectSingleNode(".//*[local-name() = 'metaData']/*[local-name() = 'type']/@name");
					if (xmlNodes != null)
					{
						int num = xmlNodes.InnerText.IndexOf(",");
						this.m_sTypeName = (num >= 0 ? xmlNodes.InnerText.Substring(0, num) : xmlNodes.InnerText);
					}
				}
				return this.m_sTypeName;
			}
		}

		public SPWebPartV3(XmlNode webPartXml) : base(webPartXml)
		{
		}

		public SPWebPartV3(string sWebPartXml) : base(sWebPartXml)
		{
		}

		public override SPWebPart Clone()
		{
			return new SPWebPartV3(this.m_Xml);
		}

		protected override void DeletePropertyFromWebPart(string sPropertyName)
		{
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(".//*[local-name() = 'properties']");
			if (xmlNodes != null)
			{
				XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(string.Concat("./*[local-name() = 'property'][@name = '", sPropertyName, "']"));
				if (xmlNodes1 != null)
				{
					xmlNodes.RemoveChild(xmlNodes1);
				}
			}
		}

		protected override string GetValue(string sPropertyName)
		{
			string innerText;
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(string.Concat(".//*[local-name() = 'property'][@name='", sPropertyName, "']"));
			if (xmlNodes == null)
			{
				innerText = null;
			}
			else
			{
				innerText = xmlNodes.InnerText;
			}
			return innerText;
		}

		protected override void SetValue(string sPropertyName, string sValue)
		{
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(string.Concat(".//*[local-name() = 'property'][@name='", sPropertyName, "']"));
			if (xmlNodes != null)
			{
				xmlNodes.InnerText = sValue;
			}
			else
			{
				XmlNode xmlNodes1 = this.m_Xml.SelectSingleNode(".//*[local-name() = 'properties']");
				if (xmlNodes1 != null)
				{
					XmlNode xmlNodes2 = xmlNodes1.OwnerDocument.CreateElement("property");
					XmlAttribute xmlAttribute = xmlNodes1.OwnerDocument.CreateAttribute("name");
					xmlAttribute.Value = sPropertyName;
					xmlNodes2.Attributes.Append(xmlAttribute);
					xmlNodes2.InnerText = sValue;
					xmlNodes1.AppendChild(xmlNodes2);
				}
			}
		}
	}
}