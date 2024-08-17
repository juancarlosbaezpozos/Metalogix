using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalContentTypeOperation : IEquatable<SPExternalContentTypeOperation>
	{
		private SPExternalContentType m_parentExternalContentType;

		private string m_sName;

		private string m_sDisplayName;

		private string m_sType;

		private SPExternalContentTypeOperationFilter[] m_filters;

		private string[] m_fields;

		public string DisplayName
		{
			get
			{
				return this.m_sDisplayName;
			}
		}

		public string[] Fields
		{
			get
			{
				return this.m_fields;
			}
		}

		public SPExternalContentTypeOperationFilter[] Filters
		{
			get
			{
				return this.m_filters;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public SPExternalContentType ParentExternalContentType
		{
			get
			{
				return this.m_parentExternalContentType;
			}
		}

		public string Type
		{
			get
			{
				return this.m_sType;
			}
		}

		private SPExternalContentTypeOperation(SPExternalContentType parentExternalContentType, string sName, string sDisplayName, string sType, string[] fields, SPExternalContentTypeOperationFilter[] filters)
		{
			if (parentExternalContentType == null)
			{
				throw new ArgumentNullException("parentExternalContentType");
			}
			if (sName == null)
			{
				throw new ArgumentNullException("sName");
			}
			if (sDisplayName == null)
			{
				throw new ArgumentNullException("sDisplayName");
			}
			if (sType == null)
			{
				throw new ArgumentNullException("sType");
			}
			if (fields == null)
			{
				throw new ArgumentNullException("fields");
			}
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			this.m_sName = sName;
			this.m_sDisplayName = sDisplayName;
			this.m_sType = sType;
			this.m_filters = filters;
			this.m_fields = fields;
			this.m_parentExternalContentType = parentExternalContentType;
		}

		public bool Equals(SPExternalContentTypeOperation other)
		{
			bool flag;
			if (other != null)
			{
				flag = (!this.m_sName.Equals(other.m_sName) ? false : this.m_parentExternalContentType.Equals(other.m_parentExternalContentType));
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		internal static SPExternalContentTypeOperation ParseOperationFromXml(XmlNode nodeOperation, SPExternalContentType parentExternalContentType)
		{
			if (nodeOperation == null)
			{
				throw new ArgumentNullException("nodeOperation");
			}
			XmlAttribute itemOf = nodeOperation.Attributes["Name"];
			if (itemOf == null)
			{
				throw new ArgumentException("Operation node must contain a Name attribute");
			}
			XmlAttribute xmlAttribute = nodeOperation.Attributes["Type"];
			if (itemOf == null)
			{
				throw new ArgumentException("Operation node must contain a Type attribute");
			}
			XmlAttribute itemOf1 = nodeOperation.Attributes["DisplayName"];
			string str = (itemOf1 == null ? itemOf.Value : itemOf1.Value);
			XmlNodeList xmlNodeLists = nodeOperation.SelectNodes("./OperationFilters/Filter");
			List<SPExternalContentTypeOperationFilter> sPExternalContentTypeOperationFilters = new List<SPExternalContentTypeOperationFilter>();
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				SPExternalContentTypeOperationFilter sPExternalContentTypeOperationFilter = SPExternalContentTypeOperationFilter.ParseFilterFromXml(xmlNodes);
				if (sPExternalContentTypeOperationFilter != null)
				{
					sPExternalContentTypeOperationFilters.Add(sPExternalContentTypeOperationFilter);
				}
			}
			XmlNodeList xmlNodeLists1 = nodeOperation.SelectNodes("./Fields/Field");
			List<string> strs = new List<string>();
			foreach (XmlNode xmlNodes1 in xmlNodeLists1)
			{
				XmlAttribute xmlAttribute1 = xmlNodes1.Attributes["Name"];
				if (xmlAttribute1 != null)
				{
					strs.Add(xmlAttribute1.Value);
				}
			}
			SPExternalContentTypeOperation sPExternalContentTypeOperation = new SPExternalContentTypeOperation(parentExternalContentType, itemOf.Value, itemOf1.Value, xmlAttribute.Value, strs.ToArray(), sPExternalContentTypeOperationFilters.ToArray());
			return sPExternalContentTypeOperation;
		}
	}
}