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
	public class SPExternalItemCollection : List<SPExternalItem>
	{
		private SPWeb m_parentWeb;

		private SPExternalContentType m_parentExternalContentType;

		private string m_sFinderOperationName = null;

		private readonly string m_sListID;

		public string FinderOperationName
		{
			get
			{
				return this.m_sFinderOperationName;
			}
		}

		public SPExternalItem this[string sIdentity]
		{
			get
			{
				SPExternalItem sPExternalItem;
				if (sIdentity == null)
				{
					throw new ArgumentOutOfRangeException("sIdentity", "Specified Identity cannot be null");
				}
				foreach (SPExternalItem sPExternalItem1 in this)
				{
					if (!(!sIdentity.StartsWith("__") ? true : !sIdentity.Equals(sPExternalItem1.BdcIdentity, StringComparison.InvariantCultureIgnoreCase)))
					{
						sPExternalItem = sPExternalItem1;
						return sPExternalItem;
					}
					else if (sIdentity.Equals(sPExternalItem1.Identity))
					{
						sPExternalItem = sPExternalItem1;
						return sPExternalItem;
					}
				}
				sPExternalItem = null;
				return sPExternalItem;
			}
		}

		public string ListID
		{
			get
			{
				return this.m_sListID;
			}
		}

		public SPExternalContentType ParentExternalContentType
		{
			get
			{
				return this.m_parentExternalContentType;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		internal SPExternalItemCollection(SPExternalContentType parentExternalContentType) : this(parentExternalContentType, null, null)
		{
		}

		internal SPExternalItemCollection(SPExternalContentType parentExternalContentType, string sFinderOperationName, string listID)
		{
			string str;
			this.m_parentWeb = parentExternalContentType.ParentWeb;
			this.m_parentExternalContentType = parentExternalContentType;
			if (string.IsNullOrEmpty(sFinderOperationName))
			{
				str = null;
			}
			else
			{
				str = sFinderOperationName;
			}
			this.m_sFinderOperationName = str;
			this.m_sListID = listID;
			this.FetchData();
		}

		public void FetchData()
		{
			base.Clear();
			string externalItems = this.m_parentWeb.Adapter.Reader.GetExternalItems(this.m_parentExternalContentType.Namespace, this.m_parentExternalContentType.Name, this.m_sFinderOperationName, this.m_sListID);
			foreach (SPExternalItem sPExternalItem in SPExternalItemCollection.ParseExternalItemsFromXml(externalItems, this.m_parentWeb))
			{
				base.Add(sPExternalItem);
			}
		}

		private static IEnumerable<SPExternalItem> ParseExternalItemsFromXml(string sCollectionXml, SPWeb parentWeb)
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
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("/SPExternalItemCollection/SPExternalItem"))
			{
				yield return SPExternalItem.ParseExternalItemFromXml(xmlNodes);
			}
		}
	}
}