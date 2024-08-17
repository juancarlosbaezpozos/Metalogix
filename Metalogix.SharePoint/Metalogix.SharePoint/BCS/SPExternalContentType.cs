using Metalogix.SharePoint;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalContentType : IEquatable<SPExternalContentType>
	{
		private SPWeb m_parentWeb;

		private string m_sName;

		private string m_sNamespace;

		private string m_sDisplayName;

		private string m_sExternalDataSourceName;

		private string[] m_fieldNames;

		private string ITEM_CACHE_LOCK = "";

		private Dictionary<string, SPExternalItemCollection> m_externalItemCollectionCache = new Dictionary<string, SPExternalItemCollection>();

		private SPExternalContentTypeOperationCollection m_operations = null;

		public string DisplayName
		{
			get
			{
				return this.m_sDisplayName;
			}
		}

		public string ExternalDataSource
		{
			get
			{
				return this.m_sExternalDataSourceName;
			}
		}

		public string[] FieldNames
		{
			get
			{
				return this.m_fieldNames;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public string Namespace
		{
			get
			{
				return this.m_sNamespace;
			}
		}

		public SPExternalContentTypeOperationCollection Operations
		{
			get
			{
				if (this.m_operations == null)
				{
					this.m_operations = new SPExternalContentTypeOperationCollection(this);
				}
				return this.m_operations;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		private SPExternalContentType(SPWeb parentWeb, string sNamespace, string sInternalName, string sDisplayName, string sExternalDataSource, string[] fieldNames)
		{
			this.m_parentWeb = parentWeb;
			this.m_sName = sInternalName;
			this.m_sNamespace = sNamespace;
			this.m_sDisplayName = sDisplayName;
			this.m_sExternalDataSourceName = sExternalDataSource;
			this.m_fieldNames = fieldNames;
		}

		internal static SPExternalContentType CreateFromTerseData(SPWeb parentWeb, string externalDataSource, string entityName, string entityNamespace)
		{
			SPExternalContentType sPExternalContentType = new SPExternalContentType(parentWeb, entityNamespace, entityName, entityName, externalDataSource, new string[0]);
			return sPExternalContentType;
		}

		public bool Equals(SPExternalContentType other)
		{
			return this.Equals(other, true);
		}

	    public bool Equals(SPExternalContentType other, bool compareWeb)
	    {
	        return other != null && (this.m_sName == null == (other.m_sName == null) && this.m_sNamespace == null == (other.m_sNamespace == null) && this.m_sExternalDataSourceName == null == (other.m_sExternalDataSourceName == null) && this.m_sDisplayName == null == (other.m_sDisplayName == null)) && (!compareWeb || this.m_parentWeb == null == (other.m_parentWeb == null)) && ((this.m_sName == null || this.m_sName.Equals(other.m_sName, StringComparison.InvariantCultureIgnoreCase)) && (this.m_sNamespace == null || this.m_sNamespace.Equals(other.m_sNamespace, StringComparison.InvariantCultureIgnoreCase)) && (this.m_sExternalDataSourceName == null || this.m_sExternalDataSourceName.Equals(other.m_sExternalDataSourceName, StringComparison.InvariantCultureIgnoreCase))) && (!compareWeb || this.m_parentWeb == null || this.m_parentWeb.RootWebGUID == null || this.m_parentWeb.RootWebGUID.Equals(other.m_parentWeb.RootWebGUID, StringComparison.InvariantCultureIgnoreCase));
	    }

        public override bool Equals(object obj)
		{
			bool flag;
			flag = (obj is SPExternalContentType ? this.Equals((SPExternalContentType)obj) : false);
			return flag;
		}

		public SPExternalItemCollection GetExternalItems(string sFinderMethodName)
		{
			return this.GetExternalItems(sFinderMethodName, null);
		}

		public SPExternalItemCollection GetExternalItems(string sFinderMethodName, string listID)
		{
			if (string.IsNullOrEmpty(sFinderMethodName))
			{
				sFinderMethodName = string.Empty;
			}
			lock (this.ITEM_CACHE_LOCK)
			{
				if (!this.m_externalItemCollectionCache.ContainsKey(sFinderMethodName))
				{
					SPExternalItemCollection sPExternalItemCollection = new SPExternalItemCollection(this, sFinderMethodName, listID);
					this.m_externalItemCollectionCache.Add(sFinderMethodName, sPExternalItemCollection);
				}
			}
			return this.m_externalItemCollectionCache[sFinderMethodName];
		}

		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_sName);
			stringBuilder.Append(this.m_sNamespace);
			stringBuilder.Append(this.m_sExternalDataSourceName);
			stringBuilder.Append(this.m_sDisplayName);
			if (this.m_parentWeb != null)
			{
				stringBuilder.Append(this.m_parentWeb.RootWebGUID);
			}
			return stringBuilder.ToString().GetHashCode();
		}

		internal static SPExternalContentType ParseExternalContentTypeFromXml(XmlNode node, SPWeb parentWeb)
		{
			XmlAttribute itemOf = node.Attributes["Name"];
			if (itemOf == null)
			{
				throw new ArgumentException("Provided node does not contain a Name attribute");
			}
			XmlAttribute xmlAttribute = node.Attributes["Namespace"];
			if (xmlAttribute == null)
			{
				throw new ArgumentException("Provided node does not contain a Namespace attribute");
			}
			XmlAttribute itemOf1 = node.Attributes["DisplayName"];
			if (itemOf1 == null)
			{
				throw new ArgumentException("Provided node does not contain a DisplayName attribute");
			}
			XmlAttribute xmlAttribute1 = node.Attributes["ExternalDataSource"];
			if (xmlAttribute1 == null)
			{
				throw new ArgumentException("Provided node does not contain a ExternalDataSource attribute");
			}
			XmlAttribute itemOf2 = node.Attributes["Fields"];
			if (itemOf2 == null)
			{
				throw new ArgumentException("Provided node does not contain a Fields attribute");
			}
			string[] strArrays = (itemOf2.Value == null ? new string[0] : itemOf2.Value.Split(new char[] { ';' }));
			SPExternalContentType sPExternalContentType = new SPExternalContentType(parentWeb, xmlAttribute.Value, itemOf.Value, itemOf1.Value, xmlAttribute1.Value, strArrays);
			return sPExternalContentType;
		}

		private class _XmlNames
		{
			public const string NAME = "Name";

			public const string DISPLAY_NAME = "DisplayName";

			public const string NAMESPACE = "Namespace";

			public const string EXTERNAL_DATA_SOURCE = "ExternalDataSource";

			public const string FIELDS = "Fields";

			public _XmlNames()
			{
			}
		}
	}
}