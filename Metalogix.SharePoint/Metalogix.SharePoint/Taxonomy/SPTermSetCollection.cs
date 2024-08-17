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
	public class SPTermSetCollection : SPTaxonomyItemCollection<SPTermSet>
	{
		private SPTermGroup _termGroup;

		private readonly object _termSetLock = new object();

		public SPTermSet this[string termSetName]
		{
			get
			{
				SPTermSet sPTermSet = null;
				foreach (SPTermSet sPTermSet1 in (IEnumerable<SPTermSet>)this)
				{
					if (string.Equals(sPTermSet1.Name, termSetName, StringComparison.OrdinalIgnoreCase))
					{
						sPTermSet = sPTermSet1;
						break;
					}
				}
				return sPTermSet;
			}
		}

		internal SPTermSetCollection(SPTermGroup termGroup)
		{
			if (termGroup == null)
			{
				throw new ArgumentNullException("termGroup");
			}
			this._termGroup = termGroup;
		}

		public TaxonomyOperationResult Add(string termSetName, string description)
		{
			TaxonomyOperationResult taxonomyOperationResult;
			if (string.IsNullOrEmpty(termSetName))
			{
				throw new ArgumentNullException(string.Format("termSetName is null or empty. Unable to add termset to group {0}", this._termGroup.Name));
			}
			if (this[termSetName] == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
				try
				{
					xmlTextWriter.Formatting = Formatting.None;
					xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermSet.ToString());
					xmlTextWriter.WriteAttributeString(TaxonomyFields.Name.ToString(), termSetName);
					xmlTextWriter.WriteAttributeString(TaxonomyFields.Description.ToString(), (description == null ? string.Empty : description));
					string str = TaxonomyFields.TermStoreId.ToString();
					Guid id = this._termGroup.TermStore.Id;
					xmlTextWriter.WriteAttributeString(str, id.ToString());
					xmlTextWriter.WriteAttributeString(TaxonomyFields.TermGroupName.ToString(), this._termGroup.Name);
					xmlTextWriter.WriteEndElement();
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						((IDisposable)xmlTextWriter).Dispose();
					}
				}
				string str1 = this._termGroup.Connection.Adapter.Writer.AddTermSet(stringBuilder.ToString());
				TaxonomyOperationResult taxonomyOperationResult1 = new TaxonomyOperationResult(TaxonomyClassType.SPTermSet, str1);
				if (!string.IsNullOrEmpty(taxonomyOperationResult1.ObjectXml))
				{
					SPTermSet sPTermSet = new SPTermSet(XmlUtility.StringToXmlNode(taxonomyOperationResult1.ObjectXml), this._termGroup);
					lock (this._termSetLock)
					{
						if (this[sPTermSet.Name] == null)
						{
							base.AddToCollection(sPTermSet, true);
							if (this._termGroup.TermStore.IsTermSetCacheInitialised)
							{
								this._termGroup.TermStore.TryUpdateTermSetCache(new List<SPTermSet>()
								{
									sPTermSet
								}, false);
							}
						}
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
			lock (this._termSetLock)
			{
				ISharePointReader reader = this._termGroup.Connection.Adapter.Reader;
				Guid id = this._termGroup.TermStore.Id;
				string str = id.ToString();
				id = this._termGroup.Id;
				string termSetCollection = reader.GetTermSetCollection(str, id.ToString());
				if (base.Count > 0)
				{
					if (this._termGroup.TermStore.IsTermSetCacheInitialised)
					{
						this._termGroup.TermStore.TryUpdateTermSetCache(this, true);
					}
					base.Clear(true);
				}
				foreach (SPTermSet sPTermSet in SPTermSetCollection.ParseTermSetsFromXml(termSetCollection, this._termGroup))
				{
					base.Add(sPTermSet, true);
				}
				if (this._termGroup.TermStore.IsTermSetCacheInitialised)
				{
					this._termGroup.TermStore.TryUpdateTermSetCache(this, false);
				}
			}
		}

		private static IEnumerable<SPTermSet> ParseTermSetsFromXml(string termSetCollectionXml, SPTermGroup parentGroup)
		{
			if (termSetCollectionXml == null)
			{
				throw new ArgumentNullException("termSetCollectionXml");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(termSetCollectionXml);
			XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(string.Format("{0}/{1}", TaxonomyClassType.SPTermSetCollection.ToString(), TaxonomyClassType.SPTermSet.ToString()));
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				yield return new SPTermSet(xmlNodes, parentGroup);
			}
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermSetCollection.ToString());
			foreach (SPTermSet sPTermSet in (IEnumerable<SPTermSet>)this)
			{
				sPTermSet.ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}

		public void ToXMLSpecific(XmlWriter xmlWriter, IList<string> termSetNames)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermSetCollection.ToString());
			foreach (SPTermSet sPTermSet in (IEnumerable<SPTermSet>)this)
			{
				if (termSetNames.Contains(sPTermSet.Name))
				{
					sPTermSet.ToXML(xmlWriter);
				}
			}
			xmlWriter.WriteEndElement();
		}
	}
}