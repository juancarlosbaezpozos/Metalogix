using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
    [Name("Term Store")]
    [PluralName("Term Stores")]
    public class SPTermStore : IXmlable, IComparable<SPTermStore>, IEquatable<SPTermStore>
    {
        private const string C_ORPHANED_TERMS = "Orphaned Terms";

        public const string C_SITE_COLLECTION_GROUP_IDENTIFIER = "<SCG>";

        private XmlNode _xml;

        private SPConnection _connection;

        private Guid _id;

        private string _name;

        private bool _isOnline;

        private bool _isDefaultSiteCollectionTermStore;

        private bool _isDefaultKeywordsTermStore;

        private SPTermGroupCollection _groups = null;

        private readonly object _groupsLock = new object();

        private volatile Dictionary<Guid, SPTermSet> _termSetLookupCache = null;

        private readonly object _termSetLookupCacheLock = new object();

        private volatile bool _termSetCachePopulating = false;

        public SPConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        public SPTermGroupCollection Groups
        {
            get
            {
                lock (this._groupsLock)
                {
                    if (this._groups == null)
                    {
                        SPTermGroupCollection sPTermGroupCollection = new SPTermGroupCollection(this);
                        sPTermGroupCollection.FetchData();
                        this._groups = sPTermGroupCollection;
                    }
                }
                return this._groups;
            }
        }

        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        public bool IsDefaultKeywordsTermStore
        {
            get
            {
                return this._isDefaultKeywordsTermStore;
            }
        }

        public bool IsDefaultSiteCollectionTermStore
        {
            get
            {
                return this._isDefaultSiteCollectionTermStore;
            }
        }

        public bool IsOnline
        {
            get
            {
                return this._isOnline;
            }
        }

        internal bool IsTermSetCacheInitialised
        {
            get
            {
                return (this._termSetCachePopulating ? false : this._termSetLookupCache != null);
            }
        }

        public string LanguageCollection
        {
            get
            {
                XmlNode xmlNodes = this._xml.SelectSingleNode(TaxonomyClassType.SPLanguageCollection.ToString());
                return (xmlNodes != null ? xmlNodes.OuterXml : string.Empty);
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        private Dictionary<Guid, SPTermSet> TermSetLookupCache
        {
            get
            {
                lock (this._termSetLookupCacheLock)
                {
                    if (this._termSetLookupCache == null)
                    {
                        this._termSetCachePopulating = true;
                        Dictionary<Guid, SPTermSet> guids = new Dictionary<Guid, SPTermSet>();
                        foreach (SPTermGroup group in (IEnumerable<SPTermGroup>)this.Groups)
                        {
                            foreach (SPTermSet termSet in (IEnumerable<SPTermSet>)group.TermSets)
                            {
                                if (!guids.ContainsKey(termSet.Id))
                                {
                                    guids.Add(termSet.Id, termSet);
                                }
                                else
                                {
                                    guids[termSet.Id] = termSet;
                                }
                            }
                        }
                        this._termSetLookupCache = guids;
                        this._termSetCachePopulating = false;
                    }
                }
                return this._termSetLookupCache;
            }
        }

        public SPTermStore(string name)
        {
            this._id = Guid.Empty;
            this._name = name;
        }

        internal SPTermStore(XmlNode termStoreNode, SPConnection connection)
        {
            if (termStoreNode == null)
            {
                throw new ArgumentNullException("termStoreNode");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            this._xml = termStoreNode;
            this._connection = connection;
            this.ParseTermStoreXml(termStoreNode);
        }

        private void BuildListOfReusedTerms(SPTermCollection termCollection, ref List<SPTerm> reusedTermsList)
        {
            foreach (SPTerm sPTerm in (IEnumerable<SPTerm>)termCollection)
            {
                if (sPTerm.IsReused)
                {
                    reusedTermsList.Add(sPTerm);
                }
                if (sPTerm.TermsCount > 0)
                {
                    this.BuildListOfReusedTerms(sPTerm.Terms, ref reusedTermsList);
                }
            }
        }

        public int CompareTo(SPTermStore other)
        {
            int num;
            num = (other != null ? this._id.CompareTo(other._id) : 1);
            return num;
        }

        // Metalogix.SharePoint.Taxonomy.SPTermStore
        public string ConstructReusedTermsCollectionXML(string groupName, IList<string> termSetNames = null)
        {
            List<SPTerm> list = new List<SPTerm>();
            ICollection<SPTermGroup> collection;
            if (string.IsNullOrEmpty(groupName))
            {
                collection = this.Groups;
            }
            else
            {
                collection = new List<SPTermGroup>();
                SPTermGroup sPTermGroup = this.Groups[groupName];
                if (sPTermGroup != null)
                {
                    collection.Add(sPTermGroup);
                }
            }
            foreach (SPTermGroup current in collection)
            {
                foreach (SPTermSet current2 in ((IEnumerable<SPTermSet>)current.TermSets))
                {
                    if (string.IsNullOrEmpty(groupName) || termSetNames == null || termSetNames.Contains(current2.Name))
                    {
                        this.BuildListOfReusedTerms(current2.Terms, ref list);
                    }
                }
            }
            List<SPTerm> list2 = new List<SPTerm>();
            foreach (SPTerm current3 in list)
            {
                SPTerm sourceTerm = current3.SourceTerm;
                if (sourceTerm != null)
                {
                    if (sourceTerm.IsReused && sourceTerm.IsSourceTerm)
                    {
                        if (!list2.Contains(sourceTerm))
                        {
                            list2.Add(sourceTerm);
                        }
                    }
                }
            }
            List<List<SPTerm>> list3 = new List<List<SPTerm>>();
            foreach (SPTerm current4 in list2)
            {
                List<SPTerm> list4 = new List<SPTerm>();
                SPTerm sPTerm = current4;
                bool flag = current4.IsRoot;
                list4.Add(current4);
                while (!flag)
                {
                    SPTerm parent = sPTerm.Parent;
                    if (parent != null)
                    {
                        list4.Insert(0, parent);
                    }
                    if (parent == null || (parent != null && parent.IsRoot))
                    {
                        flag = true;
                    }
                    sPTerm = parent;
                }
                if (flag)
                {
                    list3.Add(list4);
                }
            }
            StringBuilder stringBuilder = new StringBuilder(65536);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.WriteStartElement(TaxonomyClassType.ReusedTermsCollection.ToString());
            foreach (List<SPTerm> current5 in list3)
            {
                xmlTextWriter.WriteStartElement(TaxonomyClassType.ParentTermCollection.ToString());
                foreach (SPTerm current3 in current5)
                {
                    if (!current3.TermSet.TermGroup.IsSystemGroup && current3.TermSet.Name != "Orphaned Terms")
                    {
                        xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTerm.ToString());
                        current3.WriteReusableTermXML(xmlTextWriter);
                        this.ResolveDependencies(list3, current3, xmlTextWriter);
                        xmlTextWriter.WriteEndElement();
                    }
                }
                xmlTextWriter.WriteEndElement();
            }
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Close();
            return stringBuilder.ToString();
        }

        public bool DoesGroupExist(string groupName)
        {
            return this.Groups[groupName] != null;
        }

        public bool Equals(SPTermStore other)
        {
            return (other == null ? false : this._id.Equals(other._id));
        }

        private List<SPTerm> FindDependencyList(List<List<SPTerm>> masterDependencies, SPTerm sourceToFind)
        {
            List<SPTerm> sPTerms = null;
            foreach (List<SPTerm> masterDependency in masterDependencies)
            {
                foreach (SPTerm sPTerm in masterDependency)
                {
                    if ((sPTerm.Id != sourceToFind.Id ? false : sPTerm.IsSourceTerm))
                    {
                        sPTerms = masterDependency;
                        break;
                    }
                }
                if (sPTerms != null)
                {
                    break;
                }
            }
            return sPTerms;
        }

        internal void FindTerm(Guid termGuid, SPTermCollection terms, ref SPTerm result)
        {
            if (result == null)
            {
                result = terms[termGuid];
                if (result == null)
                {
                    foreach (SPTerm term in (IEnumerable<SPTerm>)terms)
                    {
                        if (term.Terms.Count > 0)
                        {
                            this.FindTerm(termGuid, term.Terms, ref result);
                            if (result != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        internal SPTermSet FindTermSet(Guid termSetGuid)
        {
            SPTermSet item = null;
            if (this.TermSetLookupCache.ContainsKey(termSetGuid))
            {
                item = this.TermSetLookupCache[termSetGuid];
            }
            return item;
        }

        public SPTerm GetTerm(Guid groupGuid, Guid termSetGuid, Guid termGuid)
        {
            SPTerm sPTerm = null;
            SPTermCollection terms = this.Groups[groupGuid].TermSets[termSetGuid].Terms;
            this.FindTerm(termGuid, terms, ref sPTerm);
            return sPTerm;
        }

        public SPTerm GetTerm(Guid termSetGuid, Guid termGuid)
        {
            SPTerm sPTerm = null;
            SPTermSet sPTermSet = this.FindTermSet(termSetGuid);
            if (sPTermSet != null)
            {
                this.FindTerm(termGuid, sPTermSet.Terms, ref sPTerm);
            }
            return sPTerm;
        }

        public SPTerm GetTerm(Guid termGuid)
        {
            SPTerm term = null;
            foreach (SPTermGroup group in (IEnumerable<SPTermGroup>)this.Groups)
            {
                foreach (SPTermSet termSet in (IEnumerable<SPTermSet>)group.TermSets)
                {
                    term = this.GetTerm(termSet.Id, termGuid);
                    if (term != null)
                    {
                        break;
                    }
                }
                if (term != null)
                {
                    break;
                }
            }
            return term;
        }

        public IList<string> GetTermHierarchy(Guid termSetGuid, Guid termGuid)
        {
            List<string> strs = new List<string>();
            SPTerm parent = null;
            SPTermSet sPTermSet = this.FindTermSet(termSetGuid);
            if (sPTermSet != null)
            {
                SPTermGroup termGroup = sPTermSet.TermGroup;
                this.FindTerm(termGuid, sPTermSet.Terms, ref parent);
                if (parent != null)
                {
                    strs.Insert(0, parent.Name);
                    while (parent.Parent != null)
                    {
                        parent = parent.Parent;
                        strs.Insert(0, parent.Name);
                    }
                    strs.Insert(0, sPTermSet.Name);
                    strs.Insert(0, (termGroup.IsSiteCollectionGroup ? "<SCG>" : termGroup.Name));
                }
            }
            return strs.AsReadOnly();
        }

        public IList<string> GetTermSetHierarchy(Guid termSetGuid, bool returnSiteCollectionGroupName = false)
        {
            List<string> strs = new List<string>();
            SPTermSet sPTermSet = this.FindTermSet(termSetGuid);
            if (sPTermSet != null)
            {
                SPTermGroup termGroup = sPTermSet.TermGroup;
                strs.Insert(0, sPTermSet.Name);
                strs.Insert(0, (returnSiteCollectionGroupName || !termGroup.IsSiteCollectionGroup ? termGroup.Name : "<SCG>"));
            }
            return strs.AsReadOnly();
        }

        private void ParseTermStoreXml(XmlNode nodeTermStore)
        {
            if (nodeTermStore == null)
            {
                throw new ArgumentNullException("nodeTermStore");
            }
            this._id = nodeTermStore.GetAttributeValueAsGuid(TaxonomyFields.Id.ToString());
            this._name = nodeTermStore.GetAttributeValueAsString(TaxonomyFields.Name.ToString());
            this._isOnline = nodeTermStore.GetAttributeValueAsBoolean(TaxonomyFields.IsOnline.ToString());
            this._isDefaultSiteCollectionTermStore = nodeTermStore.GetAttributeValueAsBoolean(TaxonomyFields.IsDefaultSiteCollectionTermStore.ToString());
            this._isDefaultKeywordsTermStore = nodeTermStore.GetAttributeValueAsBoolean(TaxonomyFields.IsDefaultKeywordsTermStore.ToString());
        }

        private void ResolveDependencies(List<List<SPTerm>> masterDependencies, SPTerm term, XmlWriter xmlWriter)
        {
            if (term != null)
            {
                if ((!term.IsReused ? false : !term.IsSourceTerm))
                {
                    List<SPTerm> sPTerms = this.FindDependencyList(masterDependencies, term.SourceTerm);
                    xmlWriter.WriteStartElement(TaxonomyClassType.DependencyCollection.ToString());
                    foreach (SPTerm sPTerm in sPTerms)
                    {
                        if ((sPTerm.TermSet.TermGroup.IsSystemGroup ? false : sPTerm.TermSet.Name != "Orphaned Terms"))
                        {
                            xmlWriter.WriteStartElement(TaxonomyClassType.SPTerm.ToString());
                            sPTerm.WriteReusableTermXML(xmlWriter);
                            this.ResolveDependencies(masterDependencies, sPTerm, xmlWriter);
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXML(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(TaxonomyClassType.SPTermStore.ToString());
            foreach (XmlAttribute attribute in this._xml.Attributes)
            {
                xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
            }
            this.Groups.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        internal void TryUpdateTermSetCache(IEnumerable<SPTermSet> termSetCollection, bool removeFromCache)
        {
            if (this.IsTermSetCacheInitialised)
            {
                lock (this._termSetLookupCacheLock)
                {
                    foreach (SPTermSet sPTermSet in termSetCollection)
                    {
                        if (this.TermSetLookupCache.ContainsKey(sPTermSet.Id))
                        {
                            if (!removeFromCache)
                            {
                                this.TermSetLookupCache[sPTermSet.Id] = sPTermSet;
                            }
                            else
                            {
                                this.TermSetLookupCache.Remove(sPTermSet.Id);
                            }
                        }
                        else if (!removeFromCache)
                        {
                            this.TermSetLookupCache.Add(sPTermSet.Id, sPTermSet);
                        }
                    }
                }
            }
        }
    }
}