using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalContentTypeCollection : List<SPExternalContentType>
	{
		private SPWeb m_parentWeb;

		public SPExternalContentType this[string sNamespace, string sName]
		{
			get
			{
				SPExternalContentType sPExternalContentType;
				if (sNamespace == null)
				{
					throw new ArgumentNullException("sNamespace");
				}
				if (sName == null)
				{
					throw new ArgumentNullException("sName");
				}
				foreach (SPExternalContentType sPExternalContentType1 in this)
				{
					if ((sPExternalContentType1.Namespace == null ? false : sPExternalContentType1.Name != null))
					{
						if ((!sPExternalContentType1.Namespace.Equals(sNamespace, StringComparison.InvariantCultureIgnoreCase) ? false : sPExternalContentType1.Name.Equals(sName, StringComparison.InvariantCultureIgnoreCase)))
						{
							sPExternalContentType = sPExternalContentType1;
							return sPExternalContentType;
						}
					}
				}
				sPExternalContentType = null;
				return sPExternalContentType;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		internal SPExternalContentTypeCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
			this.FetchData();
		}

		public void FetchData()
		{
			base.Clear();
			string externalContentTypes = this.m_parentWeb.Adapter.Reader.GetExternalContentTypes();
			foreach (SPExternalContentType sPExternalContentType in SPExternalContentTypeCollection.ParseExternalContentTypesFromXml(externalContentTypes, this.m_parentWeb))
			{
				base.Add(sPExternalContentType);
			}
		}

		private static IEnumerable<SPExternalContentType> ParseExternalContentTypesFromXml(string sCollectionXml, SPWeb parentWeb)
		{
			if (sCollectionXml == null)
			{
				throw new ArgumentNullException("sCollectionXml");
			}
			if (parentWeb == null)
			{
				throw new ArgumentNullException("parentWeb");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sCollectionXml);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("/SPExternalContentTypeCollection/SPExternalContentType"))
			{
				yield return SPExternalContentType.ParseExternalContentTypeFromXml(xmlNodes, parentWeb);
			}
		}
	}
}