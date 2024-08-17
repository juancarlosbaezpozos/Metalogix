using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebPartV2 : SPWebPart
	{
		private const string V2_TYPE_NAME = "TypeName";

		private const string V2_ASSEMBLY = "Assembly";

		public override string Assembly
		{
			get
			{
				if (this.m_sAssembly == null)
				{
					this.m_sAssembly = this.GetValue("Assembly");
				}
				return this.m_sAssembly;
			}
		}

		public override string CatalogIconImageUrl
		{
			get
			{
				return base["PartImageLarge"];
			}
			set
			{
				base["PartImageLarge"] = value;
			}
		}

		public override string HelpUrl
		{
			get
			{
				return base["HelpLink"];
			}
			set
			{
				base["HelpLink"] = value;
			}
		}

		public override string TitleIconUrl
		{
			get
			{
				return base["PartImageSmall"];
			}
			set
			{
				base["PartImageSmall"] = value;
			}
		}

		public override string TitleUrl
		{
			get
			{
				return base["DetailLink"];
			}
			set
			{
				base["DetailLink"] = value;
			}
		}

		public override string TypeName
		{
			get
			{
				if (this.m_sTypeName == null)
				{
					this.m_sTypeName = this.GetValue("TypeName");
				}
				return this.m_sTypeName;
			}
		}

		public SPWebPartV2(XmlNode webPartXml) : base(webPartXml)
		{
		}

		public SPWebPartV2(string sWebPartXml) : base(sWebPartXml)
		{
		}

		public override SPWebPart Clone()
		{
			return new SPWebPartV2(this.m_Xml);
		}

		protected override void DeletePropertyFromWebPart(string sPropertyName)
		{
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(string.Concat("./*[local-name() = '", sPropertyName, "']"));
			if (xmlNodes != null)
			{
				this.m_Xml.RemoveChild(xmlNodes);
			}
		}

		protected override string GetValue(string sPropertyName)
		{
			string innerText;
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(string.Concat("./*[local-name() = '", sPropertyName, "']"));
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
			XmlNode xmlNodes = this.m_Xml.SelectSingleNode(string.Concat("./*[local-name() = '", sPropertyName, "']"));
			if (xmlNodes != null)
			{
				xmlNodes.InnerText = sValue;
			}
			else
			{
				xmlNodes = this.m_Xml.OwnerDocument.CreateElement(sPropertyName, this.m_Xml.NamespaceURI);
				xmlNodes.InnerText = sValue;
				this.m_Xml.AppendChild(xmlNodes);
			}
		}
	}
}