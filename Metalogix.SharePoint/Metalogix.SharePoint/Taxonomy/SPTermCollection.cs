using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
    public class SPTermCollection : SPTaxonomyItemCollection<SPTerm>
    {
        private readonly object _termCollectionLock = new object();

        private SPTermSetItem _parentTermSetItem = null;

        private SPTermSet _parentTermSet = null;

        private SPTerm _parentTerm = null;

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public SPTerm this[string termName]
        {
            get
            {
                SPTerm sPTerm = null;
                foreach (SPTerm sPTerm1 in (IEnumerable<SPTerm>)this)
                {
                    if ((string.Equals(sPTerm1.Name, termName, StringComparison.OrdinalIgnoreCase) ? true : sPTerm1.DoesLabelMatch(termName)))
                    {
                        sPTerm = sPTerm1;
                        break;
                    }
                }
                return sPTerm;
            }
        }

        private SPTermCollection()
        {
        }

        internal SPTermCollection(SPTermSetItem parentTermSetItem)
        {
            SPTerm sPTerm;
            if (parentTermSetItem == null)
            {
                throw new ArgumentNullException("parentTermSetItem");
            }
            this._parentTermSetItem = parentTermSetItem;
            this._parentTermSet = (parentTermSetItem is SPTermSet ? (SPTermSet)parentTermSetItem : ((SPTerm)parentTermSetItem).TermSet);
            if (parentTermSetItem is SPTerm)
            {
                sPTerm = (SPTerm)parentTermSetItem;
            }
            else
            {
                sPTerm = null;
            }
            this._parentTerm = sPTerm;
        }

        public TaxonomyOperationResult Add(string termName, string description)
        {
            TaxonomyOperationResult taxonomyOperationResult;
            if (string.IsNullOrEmpty(termName))
            {
                throw new ArgumentNullException(string.Format("termName is null or empty. Unable to add term to termset {0} within group {1}", this._parentTermSet.Name, this._parentTermSet.TermGroup.Name));
            }
            if (this[termName] == null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                try
                {
                    xmlTextWriter.Formatting = Formatting.None;
                    xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTerm.ToString());
                    xmlTextWriter.WriteAttributeString(TaxonomyFields.Name.ToString(), termName);
                    xmlTextWriter.WriteAttributeString(TaxonomyFields.Description.ToString(), (description == null ? string.Empty : description));
                    string str = TaxonomyFields.TermStoreId.ToString();
                    Guid id = this._parentTermSet.TermStore.Id;
                    xmlTextWriter.WriteAttributeString(str, id.ToString());
                    xmlTextWriter.WriteAttributeString(TaxonomyFields.TermGroupName.ToString(), this._parentTermSet.TermGroup.Name);
                    xmlTextWriter.WriteAttributeString(TaxonomyFields.TermSetName.ToString(), this._parentTermSet.Name);
                    string str1 = TaxonomyFields.TermSetId.ToString();
                    id = this._parentTermSet.Id;
                    xmlTextWriter.WriteAttributeString(str1, id.ToString());
                    if (this._parentTerm != null)
                    {
                        string str2 = TaxonomyFields.ParentTermId.ToString();
                        id = this._parentTerm.Id;
                        xmlTextWriter.WriteAttributeString(str2, id.ToString());
                    }
                    xmlTextWriter.WriteEndElement();
                }
                finally
                {
                    if (xmlTextWriter != null)
                    {
                        ((IDisposable)xmlTextWriter).Dispose();
                    }
                }
                string str3 = this._parentTermSet.Connection.Adapter.Writer.AddTerm(stringBuilder.ToString());
                TaxonomyOperationResult taxonomyOperationResult1 = new TaxonomyOperationResult(TaxonomyClassType.SPTerm, str3);
                if (!string.IsNullOrEmpty(taxonomyOperationResult1.ObjectXml))
                {
                    SPTerm sPTerm = new SPTerm(XmlUtility.StringToXmlNode(taxonomyOperationResult1.ObjectXml), this._parentTermSetItem);
                    if (this[sPTerm.Name] == null)
                    {
                        base.AddToCollection(sPTerm, true);
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
            lock (this._termCollectionLock)
            {
                string termCollectionXml = this._parentTermSetItem.GetTermCollectionXml();
                if (base.Count > 0)
                {
                    base.Clear(true);
                }
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(termCollectionXml);
                XmlNode xmlNodes = xmlDocument.SelectSingleNode(string.Format("/{0}", TaxonomyClassType.SPTermCollection.ToString()));
                this.ParseChildTerms(xmlNodes, this._parentTermSetItem, this, this._parentTermSetItem is SPTermSet);
            }
        }

        internal static SPTermCollection GetAllRecursiveTerms(SPTermSet parentTermSet)
        {
            if (parentTermSet == null)
            {
                throw new ArgumentNullException("parentTermSet");
            }
            DateTime now = DateTime.Now;
            ISharePointReader reader = parentTermSet.Connection.Adapter.Reader;
            Guid id = parentTermSet.Id;
            string termsFromTermSet = reader.GetTermsFromTermSet(id.ToString(), true);
            DateTime.Now.Subtract(now);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(termsFromTermSet);
            Dictionary<string, List<XmlNode>> strs = null;
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("/SPTermCollection");
            SPTermCollection sPTermCollection = new SPTermCollection();
            SPTermCollection.ParseTermCollectionForTermSetItem(xmlNodes, parentTermSet, null, ref sPTermCollection, ref strs);
            DateTime.Now.Subtract(now);
            return sPTermCollection;
        }

        private void ParseChildTerms(XmlNode node, SPTermSetItem parentTermSetItem, SPTermCollection termCollection, bool sourceIsTermSet)
        {
            XmlNodeList xmlNodeLists = node.SelectNodes(string.Format("./{0}", TaxonomyClassType.SPTerm.ToString()));
            SPTermSet sPTermSet = (parentTermSetItem is SPTermSet ? parentTermSetItem as SPTermSet : (parentTermSetItem as SPTerm).TermSet);
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                SPTerm sPTerm = new SPTerm(xmlNodes, parentTermSetItem);
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(string.Format("./{0}", TaxonomyClassType.SPTermCollection.ToString()));
                if ((!sourceIsTermSet || xmlNodes1 != null ? xmlNodes1 != null : true))
                {
                    SPTermCollection sPTermCollection = new SPTermCollection(sPTerm);
                    sPTerm.Terms = sPTermCollection;
                    if (xmlNodes1 != null)
                    {
                        this.ParseChildTerms(xmlNodes1, sPTerm, sPTermCollection, sourceIsTermSet);
                    }
                }
                termCollection.Add(sPTerm, true);
            }
        }

        private static void ParseTermCollectionForTermSetItem(XmlNode nodeCollection, SPTermSet parentTermSet, SPTerm parentTerm, ref SPTermCollection copyTermsToCollection, ref Dictionary<string, List<XmlNode>> termNodeIndex)
        {
            if (nodeCollection == null)
            {
                throw new ArgumentNullException("nodeCollection");
            }
            if (parentTermSet == null)
            {
                throw new ArgumentNullException("parentTermSet");
            }
            SPTermCollection sPTermCollection = new SPTermCollection();
            SPTerm[] sPTermArray = SPTermCollection.ParseTermsFromCollectionXml(nodeCollection, parentTermSet, parentTerm, ref termNodeIndex);
            for (int i = 0; i < (int)sPTermArray.Length; i++)
            {
                SPTerm sPTerm = sPTermArray[i];
                SPTermCollection.ParseTermCollectionForTermSetItem(nodeCollection, parentTermSet, sPTerm, ref copyTermsToCollection, ref termNodeIndex);
                sPTermCollection.Add(sPTerm, true);
                if (copyTermsToCollection != null)
                {
                    copyTermsToCollection.Add(sPTerm, true);
                }
            }
            if (parentTerm != null)
            {
                parentTerm.Terms = sPTermCollection;
            }
            else
            {
                parentTermSet.Terms = sPTermCollection;
            }
        }

        // Metalogix.SharePoint.Taxonomy.SPTermCollection
        private static SPTerm[] ParseTermsFromCollectionXml(XmlNode nodeCollection, SPTermSet parentTermSet, SPTerm parentTerm, ref Dictionary<string, List<XmlNode>> termNodeIndex)
        {
            if (nodeCollection == null)
            {
                throw new ArgumentNullException("nodeCollection");
            }
            if (parentTermSet == null)
            {
                throw new ArgumentNullException("parentTermSet");
            }
            string key = (parentTerm == null) ? string.Empty : parentTerm.Id.ToString();
            if (termNodeIndex == null)
            {
                DateTime now = DateTime.Now;
                termNodeIndex = new Dictionary<string, List<XmlNode>>();
                XmlNodeList xmlNodeList = nodeCollection.SelectNodes(string.Format("./{0}", TaxonomyClassType.SPTerm.ToString()));
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    XmlAttribute xmlAttribute = xmlNode.Attributes[TaxonomyFields.ParentTermId.ToString()];
                    if (xmlAttribute != null)
                    {
                        string value = xmlAttribute.Value;
                        if (!termNodeIndex.ContainsKey(value))
                        {
                            termNodeIndex.Add(value, new List<XmlNode>());
                        }
                        termNodeIndex[value].Add(xmlNode);
                    }
                }
            }
            List<XmlNode> list;
            if (termNodeIndex.ContainsKey(key))
            {
                list = termNodeIndex[key];
            }
            else
            {
                list = new List<XmlNode>();
            }
            List<SPTerm> list2 = new List<SPTerm>();
            foreach (XmlNode xmlNode in list)
            {
                SPTerm item = new SPTerm(xmlNode, parentTermSet, parentTerm);
                list2.Add(item);
            }
            return list2.ToArray();
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(TaxonomyClassType.SPTermCollection.ToString());
            foreach (SPTerm sPTerm in (IEnumerable<SPTerm>)this)
            {
                sPTerm.ToXML(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }
    }
}