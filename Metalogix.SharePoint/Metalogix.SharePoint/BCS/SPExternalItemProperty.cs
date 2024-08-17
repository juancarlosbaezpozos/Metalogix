using System;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalItemProperty
	{
		private readonly string m_sName;

		private readonly string m_sValue;

		private readonly bool m_bIsIdentifier;

		public bool IsIdentifier
		{
			get
			{
				return this.m_bIsIdentifier;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public string Value
		{
			get
			{
				return this.m_sValue;
			}
		}

		private SPExternalItemProperty(string sName, string sValue, bool bIsIdentifier)
		{
			this.m_sName = sName;
			this.m_sValue = sValue;
			this.m_bIsIdentifier = bIsIdentifier;
		}

		internal static SPExternalItemProperty ParseSPExternalItemPropertyFromXml(XmlNode nodeProperty)
		{
			bool flag;
			if (nodeProperty == null)
			{
				throw new ArgumentNullException("nodeProperty");
			}
			XmlAttribute itemOf = nodeProperty.Attributes["Name"];
			if (itemOf == null)
			{
				throw new ArgumentException("External Item Property node must contain a Name attribute.");
			}
			XmlAttribute xmlAttribute = nodeProperty.Attributes["Value"];
			if (xmlAttribute == null)
			{
				throw new ArgumentException("External Item Property node must contain a Value attribute.");
			}
			XmlAttribute itemOf1 = nodeProperty.Attributes["Identifier"];
			if (itemOf1 == null)
			{
				throw new ArgumentException("External Item Property node must contain a Value attribute.");
			}
			bool.TryParse(itemOf1.Value, out flag);
			return new SPExternalItemProperty(itemOf.Value, xmlAttribute.Value, flag);
		}
	}
}