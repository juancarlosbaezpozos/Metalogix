using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public class SPTermGroupCollection : SPTaxonomyItemCollection<SPTermGroup>
	{
		private SPTermStore _termStore;

		private readonly object _groupLock = new object();

		public SPTermGroup this[string groupName]
		{
			get
			{
				SPTermGroup sPTermGroup = null;
				foreach (SPTermGroup sPTermGroup1 in (IEnumerable<SPTermGroup>)this)
				{
					if (string.Equals(sPTermGroup1.Name, groupName, StringComparison.OrdinalIgnoreCase))
					{
						sPTermGroup = sPTermGroup1;
						break;
					}
				}
				return sPTermGroup;
			}
		}

		internal SPTermGroupCollection(SPTermStore termStore)
		{
			if (termStore == null)
			{
				throw new ArgumentNullException("termStore");
			}
			this._termStore = termStore;
		}

		public TaxonomyOperationResult Add(string groupName, string description, bool isSiteCollectionGroup = false)
		{
			TaxonomyOperationResult taxonomyOperationResult;
			if (string.IsNullOrEmpty(groupName))
			{
				throw new Exception(string.Format("groupName is null or empty. Unable to add term group to termstore {0}", this._termStore.Name));
			}
			if (this[groupName] == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
				try
				{
					xmlTextWriter.Formatting = Formatting.None;
					xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermGroup.ToString());
					xmlTextWriter.WriteAttributeString(TaxonomyFields.Name.ToString(), groupName);
					xmlTextWriter.WriteAttributeString(TaxonomyFields.Description.ToString(), (description == null ? string.Empty : description));
					bool flag = false;
					xmlTextWriter.WriteAttributeString(TaxonomyFields.IsSystemGroup.ToString(), flag.ToString());
					xmlTextWriter.WriteAttributeString(TaxonomyFields.IsSiteCollectionGroup.ToString(), isSiteCollectionGroup.ToString());
					xmlTextWriter.WriteEndElement();
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						((IDisposable)xmlTextWriter).Dispose();
					}
				}
				ISharePointWriter writer = this._termStore.Connection.Adapter.Writer;
				Guid id = this._termStore.Id;
				string str = writer.AddTermGroup(id.ToString(), stringBuilder.ToString(), true);
				TaxonomyOperationResult taxonomyOperationResult1 = new TaxonomyOperationResult(TaxonomyClassType.SPTermGroup, str);
				if (!string.IsNullOrEmpty(taxonomyOperationResult1.ObjectXml))
				{
					SPTermGroup sPTermGroup = new SPTermGroup(XmlUtility.StringToXmlNode(taxonomyOperationResult1.ObjectXml), this._termStore);
					if (this[sPTermGroup.Name] == null)
					{
						base.AddToCollection(sPTermGroup, true);
					}
				}
				taxonomyOperationResult = taxonomyOperationResult1;
			}
			else
			{
				taxonomyOperationResult = null;
			}
			return taxonomyOperationResult;
		}

		public void FetchData()
		{
			lock (this._groupLock)
			{
				ISharePointReader reader = this._termStore.Connection.Adapter.Reader;
				Guid id = this._termStore.Id;
				string termGroups = reader.GetTermGroups(id.ToString());
				if (base.Count > 0)
				{
					base.Clear(true);
				}
				foreach (SPTermGroup sPTermGroup in SPTermGroupCollection.ParseTermGroupsFromXml(termGroups, this._termStore))
				{
					base.AddToCollection(sPTermGroup, true);
				}
			}
		}

		public SPTermGroup GetLocalSiteCollectionGroup(string siteCollectionId)
		{
			SPTermGroup sPTermGroup = null;
			foreach (SPTermGroup sPTermGroup1 in (IEnumerable<SPTermGroup>)this)
			{
				if ((!sPTermGroup1.IsSiteCollectionGroup ? false : sPTermGroup1.ContainsSiteCollectionAccessId(siteCollectionId)))
				{
					sPTermGroup = sPTermGroup1;
					break;
				}
			}
			return sPTermGroup;
		}

		private static IEnumerable<SPTermGroup> ParseTermGroupsFromXml(string sCollectionXml, SPTermStore parentTermStore)
		{
			if (sCollectionXml == null)
			{
				throw new ArgumentNullException("sCollectionXml");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sCollectionXml);
			XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(string.Format("{0}/{1}", TaxonomyClassType.SPTermGroupCollection.ToString(), TaxonomyClassType.SPTermGroup.ToString()));
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				yield return new SPTermGroup(xmlNodes, parentTermStore);
			}
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermGroupCollection.ToString());
			foreach (SPTermGroup sPTermGroup in (IEnumerable<SPTermGroup>)this)
			{
				sPTermGroup.ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
	}
}