using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public class SPTermStoreCollection : SPTaxonomyIndexedCollection<SPTermStore>
	{
		private SPConnection _connection;

		private readonly object _termStoreLock = new object();

		private Guid _termCollectionId;

		public SPTermStore DefaultKeywordsTermStore
		{
			get
			{
				SPTermStore sPTermStore = null;
				foreach (SPTermStore sPTermStore1 in (IEnumerable<SPTermStore>)this)
				{
					if ((!sPTermStore1.IsDefaultKeywordsTermStore ? false : sPTermStore1.IsOnline))
					{
						sPTermStore = sPTermStore1;
						break;
					}
				}
				return sPTermStore;
			}
		}

		public SPTermStore DefaultSiteCollectionTermStore
		{
			get
			{
				SPTermStore sPTermStore = null;
				foreach (SPTermStore sPTermStore1 in (IEnumerable<SPTermStore>)this)
				{
					if ((!sPTermStore1.IsDefaultSiteCollectionTermStore ? false : sPTermStore1.IsOnline))
					{
						sPTermStore = sPTermStore1;
						break;
					}
				}
				return sPTermStore;
			}
		}

		public SPTermStore this[string termStoreName]
		{
			get
			{
				SPTermStore sPTermStore = null;
				foreach (SPTermStore sPTermStore1 in (IEnumerable<SPTermStore>)this)
				{
					if (sPTermStore1.Name == termStoreName)
					{
						sPTermStore = sPTermStore1;
						break;
					}
				}
				return sPTermStore;
			}
		}

		public Guid TermCollectionId
		{
			get
			{
				return this._termCollectionId;
			}
		}

		internal SPTermStoreCollection(SPConnection connection) : base("Id")
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this._connection = connection;
			this._termCollectionId = Guid.NewGuid();
		}

		public void FetchData()
		{
			lock (this._termStoreLock)
			{
				string termStores = this._connection.Adapter.Reader.GetTermStores();
				if (base.Count > 0)
				{
					base.Clear(true);
				}
				foreach (SPTermStore sPTermStore in SPTermStoreCollection.ParseTermStoresFromXml(termStores, this._connection))
				{
					base.Add(sPTermStore, true);
				}
			}
		}

		private static IEnumerable<SPTermStore> ParseTermStoresFromXml(string termStoreCollectionXml, SPConnection connection)
		{
			if (termStoreCollectionXml == null)
			{
				throw new ArgumentNullException("termStoreCollectionXml");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(termStoreCollectionXml);
			XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(string.Format("{0}/{1}", TaxonomyClassType.SPTermStoreCollection.ToString(), TaxonomyClassType.SPTermStore.ToString()));
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				yield return new SPTermStore(xmlNodes, connection);
			}
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermStoreCollection.ToString());
			foreach (SPTermStore sPTermStore in (IEnumerable<SPTermStore>)this)
			{
				sPTermStore.ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
	}
}