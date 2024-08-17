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
	public class SPExternalContentTypeOperationCollection : List<SPExternalContentTypeOperation>
	{
		private SPWeb m_parentWeb;

		private SPExternalContentType m_parentExternalContentType;

		public SPExternalContentTypeOperation this[string sOperationName]
		{
			get
			{
				SPExternalContentTypeOperation sPExternalContentTypeOperation;
				if (!string.IsNullOrEmpty(sOperationName))
				{
					foreach (SPExternalContentTypeOperation sPExternalContentTypeOperation1 in this)
					{
						if (sOperationName.Equals(sPExternalContentTypeOperation1.Name))
						{
							sPExternalContentTypeOperation = sPExternalContentTypeOperation1;
							return sPExternalContentTypeOperation;
						}
					}
					sPExternalContentTypeOperation = null;
				}
				else
				{
					sPExternalContentTypeOperation = null;
				}
				return sPExternalContentTypeOperation;
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

		internal SPExternalContentTypeOperationCollection(SPExternalContentType parentExternalContentType)
		{
			this.m_parentWeb = parentExternalContentType.ParentWeb;
			this.m_parentExternalContentType = parentExternalContentType;
			this.FetchData();
		}

		public void FetchData()
		{
			base.Clear();
			string externalContentTypeOperations = this.m_parentWeb.Adapter.Reader.GetExternalContentTypeOperations(this.m_parentExternalContentType.Namespace, this.m_parentExternalContentType.Name);
			foreach (SPExternalContentTypeOperation sPExternalContentTypeOperation in SPExternalContentTypeOperationCollection.ParseExternalContentTypeOperationsFromXml(externalContentTypeOperations, this.m_parentExternalContentType))
			{
				base.Add(sPExternalContentTypeOperation);
			}
		}

		private static IEnumerable<SPExternalContentTypeOperation> ParseExternalContentTypeOperationsFromXml(string sCollectionXml, SPExternalContentType parentExternalContentType)
		{
			if (sCollectionXml == null)
			{
				throw new ArgumentNullException("sCollectionXml");
			}
			if (parentExternalContentType == null)
			{
				throw new ArgumentNullException("parentExternalContentType");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sCollectionXml);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("/SPExternalContentTypeOperationCollection/SPExternalContentTypeOperation"))
			{
				yield return SPExternalContentTypeOperation.ParseOperationFromXml(xmlNodes, parentExternalContentType);
			}
		}
	}
}