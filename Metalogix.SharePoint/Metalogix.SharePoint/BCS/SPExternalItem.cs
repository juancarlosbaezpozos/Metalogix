using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalItem : IEquatable<SPExternalItem>
	{
		private readonly string m_sBdcIdentity;

		private readonly string m_sIdentity;

		private SPExternalItemProperty[] m_properties;

		public string BdcIdentity
		{
			get
			{
				return this.m_sBdcIdentity;
			}
		}

		public SPExternalItemProperty[] IdentifierProperties
		{
			get
			{
				List<SPExternalItemProperty> sPExternalItemProperties = new List<SPExternalItemProperty>();
				SPExternalItemProperty[] mProperties = this.m_properties;
				for (int i = 0; i < (int)mProperties.Length; i++)
				{
					SPExternalItemProperty sPExternalItemProperty = mProperties[i];
					if (sPExternalItemProperty.IsIdentifier)
					{
						sPExternalItemProperties.Add(sPExternalItemProperty);
					}
				}
				return sPExternalItemProperties.ToArray();
			}
		}

		public string Identity
		{
			get
			{
				return this.m_sIdentity;
			}
		}

		public string this[string sPropertyName]
		{
			get
			{
				string value;
				SPExternalItemProperty[] mProperties = this.m_properties;
				int num = 0;
				while (true)
				{
					if (num < (int)mProperties.Length)
					{
						SPExternalItemProperty sPExternalItemProperty = mProperties[num];
						if (!(sPExternalItemProperty.Name == sPropertyName))
						{
							num++;
						}
						else
						{
							value = sPExternalItemProperty.Value;
							break;
						}
					}
					else
					{
						value = null;
						break;
					}
				}
				return value;
			}
		}

		public SPExternalItemProperty[] Properties
		{
			get
			{
				return this.m_properties;
			}
		}

		internal SPExternalItem(string sIdentity, string sBdcIdentity, SPExternalItemProperty[] properties)
		{
			this.m_sIdentity = sIdentity;
			this.m_sBdcIdentity = sBdcIdentity;
			this.m_properties = properties;
		}

		public bool Equals(SPExternalItem other)
		{
			bool flag;
			flag = (other != null ? this.Identity.Equals(other.Identity, StringComparison.InvariantCultureIgnoreCase) : false);
			return flag;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			flag = (obj is SPExternalItem ? this.Equals((SPExternalItem)obj) : false);
			return flag;
		}

		public override int GetHashCode()
		{
			return this.Identity.GetHashCode();
		}

		internal static SPExternalItem ParseExternalItemFromXml(XmlNode nodeItem)
		{
			if (nodeItem == null)
			{
				throw new ArgumentNullException("nodeItem");
			}
			XmlAttribute itemOf = nodeItem.Attributes["BdcIdentity"];
			if (itemOf == null)
			{
				throw new ArgumentException("External Item node must contain a BdcIdentity attribute");
			}
			XmlAttribute xmlAttribute = nodeItem.Attributes["Identity"];
			if (xmlAttribute == null)
			{
				throw new ArgumentException("External Item node must contain a Identity attribute");
			}
			XmlNodeList xmlNodeLists = nodeItem.SelectNodes("./SPExternalItemProperty");
			List<SPExternalItemProperty> sPExternalItemProperties = new List<SPExternalItemProperty>(xmlNodeLists.Count);
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				sPExternalItemProperties.Add(SPExternalItemProperty.ParseSPExternalItemPropertyFromXml(xmlNodes));
			}
			SPExternalItem sPExternalItem = new SPExternalItem(xmlAttribute.Value, itemOf.Value, sPExternalItemProperties.ToArray());
			return sPExternalItem;
		}
	}
}