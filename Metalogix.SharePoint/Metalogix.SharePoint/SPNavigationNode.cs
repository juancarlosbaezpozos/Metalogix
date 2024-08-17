using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
    public class SPNavigationNode
    {
        private readonly static string DEFAULT_NAV_NODE_XML;

        private readonly static string[] NAV_NODE_CORE_ATTRIBUTES;

        public readonly static int TOP_NAV_BAR_NODE_ID;

        public readonly static int QUICK_LAUNCH_NODE_ID;

        public readonly static int HOME_NODE_ID;

        private XmlNode m_xml;

        private SPNavigationNode m_parent;

        private SPNavigationNode.SPNavigationNodeProperties m_properties;

        private SPNavigationNode.SPNavigationNodeCollection m_children;

        private List<SPNavigationNode> m_deletedSubNodes;

        private bool m_bChangesMade;

        private bool m_bUpdatesInProgress;

        private bool m_bTitleChanged;

        private bool m_bUrlChanged;

        private bool m_bIsVisibleChanged;

        private bool m_targetChanged;

        private bool m_bOrderIndexChanged;

        private List<string> m_changedProperties;

        protected bool ChangeMade
        {
            get
            {
                return this.m_bChangesMade;
            }
            set
            {
                this.m_bChangesMade = value;
            }
        }

        public SPNavigationNode.SPNavigationNodeCollection Children
        {
            get
            {
                return this.m_children;
            }
        }

        public int ID
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["ID"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("ID");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = "-1";
                }
                return int.Parse(itemOf.Value);
            }
        }

        public bool IsExternal
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["IsExternal"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("IsExternal");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = "True";
                }
                return bool.Parse(itemOf.Value);
            }
        }

        public bool IsHomeNode
        {
            get
            {
                bool flag;
                if ((this.Parent == null || this.Parent.ID != SPNavigationNode.TOP_NAV_BAR_NODE_ID ? false : this.OrderIndex == 0))
                {
                    string url = this.Url;
                    if (!string.IsNullOrEmpty(url))
                    {
                        StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this.RootNode.ParentWeb.Adapter, url);
                        flag = (standardizedUrl.WebRelative == "default.aspx" ? true : standardizedUrl.WebRelative == "");
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        public bool IsSubSiteOrPageNode
        {
            get
            {
                bool flag;
                if ((this.RootNode.ParentWeb.Adapter.SharePointVersion.IsSharePoint2003 ? false : this.Properties.ContainsKey("NodeType")))
                {
                    string item = this.Properties["NodeType"];
                    flag = (item.EndsWith("Area") ? true : item.EndsWith("Page"));
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
        }

        public bool IsSystemNode
        {
            get
            {
                int d = this.ID;
                return (d < 0 ? false : d < 2000);
            }
        }

        public bool IsVisible
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["IsVisible"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("IsVisible");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = "True";
                }
                return bool.Parse(itemOf.Value);
            }
            set
            {
                if (value != this.IsVisible)
                {
                    XmlAttribute itemOf = this.m_xml.Attributes["IsVisible"];
                    itemOf.Value = value.ToString();
                    this.m_bIsVisibleChanged = true;
                    this.ChangesMade();
                }
            }
        }

        public DateTime LastModified
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["LastModified"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("LastModified");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = Utils.FormatDate(DateTime.UtcNow);
                }
                return Utils.ParseDateAsUtc(itemOf.Value);
            }
        }

        public int OrderIndex
        {
            get
            {
                int num;
                num = (this.Parent != null ? this.Parent.Children.IndexOf(this) : 0);
                return num;
            }
            set
            {
                if (this.Parent == null)
                {
                    throw new Exception("Cannot set the order index of the root node.");
                }
                if (this.OrderIndex != value)
                {
                    this.Parent.Children.Move(this, value);
                    this.m_bOrderIndexChanged = true;
                }
            }
        }

        public SPNavigationNode Parent
        {
            get
            {
                return this.m_parent;
            }
        }

        public SPNavigationNode.SPNavigationNodeProperties Properties
        {
            get
            {
                return this.m_properties;
            }
        }

        public SPNavigationRoot RootNode
        {
            get
            {
                SPNavigationRoot sPNavigationRoot;
                sPNavigationRoot = (!(this is SPNavigationRoot) ? this.Parent.RootNode : (SPNavigationRoot)this);
                return sPNavigationRoot;
            }
        }

        public string Target
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["Target"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("Target");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = this.Properties["Target"];
                }
                return itemOf.Value;
            }
            set
            {
                try
                {
                    XmlAttribute itemOf = this.m_xml.Attributes["Target"];
                    itemOf.Value = value.ToString();
                }
                catch
                {
                    this.Properties.Add("Target", value);
                }
                this.m_targetChanged = true;
                this.ChangesMade();
            }
        }

        public string Title
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["Title"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("Title");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = "";
                }
                return itemOf.Value;
            }
            set
            {
                if (value != this.Title)
                {
                    this.m_xml.Attributes["Title"].Value = value;
                    this.m_bTitleChanged = true;
                    this.ChangesMade();
                }
            }
        }

        public string Url
        {
            get
            {
                XmlAttribute itemOf = this.m_xml.Attributes["Url"];
                if (itemOf == null)
                {
                    itemOf = this.m_xml.OwnerDocument.CreateAttribute("Url");
                    this.m_xml.Attributes.Append(itemOf);
                    itemOf.Value = "";
                }
                return itemOf.Value;
            }
            set
            {
                if (value != this.Url)
                {
                    this.m_xml.Attributes["Url"].Value = value;
                    this.m_bUrlChanged = true;
                    this.ChangesMade();
                }
            }
        }

        static SPNavigationNode()
        {
            SPNavigationNode.DEFAULT_NAV_NODE_XML = "<NavNode ID=\"-1\" Title=\"\" Url=\"\" IsExternal=\"False\" IsVisible=\"True\"/>";
            string[] strArrays = new string[] { "ID", "Title", "Url", "IsVisible", "IsExternal", "LastModified" };
            SPNavigationNode.NAV_NODE_CORE_ATTRIBUTES = strArrays;
            SPNavigationNode.TOP_NAV_BAR_NODE_ID = 1002;
            SPNavigationNode.QUICK_LAUNCH_NODE_ID = 1025;
            SPNavigationNode.HOME_NODE_ID = 1000;
        }

        protected SPNavigationNode(SPNavigationNode parent, XmlNode node)
        {
            this.BeginUpdates();
            this.m_parent = parent;
            this.InitializePrivateFieldValues(node);
            this.EndUpdates();
        }

        protected SPNavigationNode(SPNavigationNode parent, string sTitle, string sUrl)
        {
            this.BeginUpdates();
            this.InitializePrivateFieldValues(null);
            this.m_parent = parent;
            this.Title = sTitle;
            this.Url = sUrl;
            this.EndUpdates();
        }

        protected SPNavigationNode()
        {
            this.m_parent = null;
        }

        protected void BeginUpdates()
        {
            this.m_bUpdatesInProgress = true;
        }

        protected void ChangesMade()
        {
            if (!this.m_bUpdatesInProgress)
            {
                this.ChangeMade = true;
            }
        }

        private void ChildNodeDeleted(SPNavigationNode deletedNode)
        {
            if (!this.m_bUpdatesInProgress)
            {
                this.m_deletedSubNodes.Add(deletedNode);
            }
        }

        public void Delete()
        {
            if (this.Parent == null)
            {
                throw new Exception("Root navigation node cannot be deleted.");
            }
            this.Parent.Children.Delete(this);
        }

        protected void EndUpdates()
        {
            this.m_bUpdatesInProgress = false;
        }

        protected void InitializePrivateFieldValues(XmlNode node)
        {
            this.SetXmlNode(node);
            this.m_children = null;
            if (node.ChildNodes.Count <= 0)
            {
                SPNavigationNode.SPNavigationNodeCollection.GetChildNodeCollectionForNavigationNode(this);
            }
            else
            {
                SPNavigationNode.SPNavigationNodeCollection.GetChildNodeCollectionForNavigationNode(this, node.ChildNodes);
            }
            this.m_properties = null;
            SPNavigationNode.SPNavigationNodeProperties.GetPropertiesForNavigationNode(this);
            this.m_deletedSubNodes = new List<SPNavigationNode>();
            this.ChangeMade = false;
            this.m_bUpdatesInProgress = false;
            this.m_bTitleChanged = false;
            this.m_bUrlChanged = false;
            this.m_bIsVisibleChanged = false;
            this.m_bOrderIndexChanged = false;
            this.m_targetChanged = false;
            this.m_changedProperties = new List<string>();
        }

        protected static bool IsCoreAttribute(string sName)
        {
            bool flag;
            string[] nAVNODECOREATTRIBUTES = SPNavigationNode.NAV_NODE_CORE_ATTRIBUTES;
            int num = 0;
            while (true)
            {
                if (num >= (int)nAVNODECOREATTRIBUTES.Length)
                {
                    flag = false;
                    break;
                }
                else if (!(sName == nAVNODECOREATTRIBUTES[num]))
                {
                    num++;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        protected void PropertyChanged(string sPropertyName)
        {
            if (!this.m_bUpdatesInProgress)
            {
                this.ChangeMade = true;
                if (!this.m_changedProperties.Contains(sPropertyName))
                {
                    this.m_changedProperties.Add(sPropertyName);
                }
            }
        }

        private void SetXmlNode(XmlNode node)
        {
            if (node == null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(SPNavigationNode.DEFAULT_NAV_NODE_XML);
                this.m_xml = xmlDocument.DocumentElement;
            }
            else
            {
                this.m_xml = node;
            }
            int d = this.ID;
            bool isExternal = this.IsExternal;
            bool isVisible = this.IsVisible;
            DateTime lastModified = this.LastModified;
            string title = this.Title;
            string url = this.Url;
        }

        protected void UpdateNodeData(XmlNode node)
        {
            this.BeginUpdates();
            this.SetXmlNode(node);
            List<SPNavigationNode> sPNavigationNodes = new List<SPNavigationNode>(this.Children);
            int num = 0;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                string value = childNode.Attributes["ID"].Value;
                SPNavigationNode sPNavigationNode = null;
                foreach (SPNavigationNode sPNavigationNode1 in sPNavigationNodes)
                {
                    if (sPNavigationNode1.m_xml.Attributes["ID"].Value == value)
                    {
                        sPNavigationNode = sPNavigationNode1;
                        break;
                    }
                }
                if (sPNavigationNode == null)
                {
                    this.Children.Insert(childNode, num);
                }
                else
                {
                    sPNavigationNodes.Remove(sPNavigationNode);
                    sPNavigationNode.UpdateNodeData(childNode);
                    this.Children.Move(sPNavigationNode, num);
                }
                num++;
            }
            foreach (SPNavigationNode sPNavigationNode2 in sPNavigationNodes)
            {
                this.Children.Delete(sPNavigationNode2);
            }
            this.m_deletedSubNodes.Clear();
            this.ChangeMade = false;
            this.m_bUpdatesInProgress = false;
            this.m_bTitleChanged = false;
            this.m_bUrlChanged = false;
            this.m_bIsVisibleChanged = false;
            this.m_bOrderIndexChanged = false;
            this.m_targetChanged = false;
            this.m_changedProperties.Clear();
            this.EndUpdates();
        }

        // Metalogix.SharePoint.SPNavigationNode
        protected void WriteChanges(XmlWriter additionsAndUpdatesWriter, XmlWriter deletionsWriter)
        {
            List<SPNavigationNode> list = new List<SPNavigationNode>(this.Children);
            if (this.ChangeMade || this.ID < 0)
            {
                additionsAndUpdatesWriter.WriteStartElement("NavNode");
                List<string> list2 = new List<string>(SPNavigationNode.NAV_NODE_CORE_ATTRIBUTES);
                foreach (XmlAttribute xmlAttribute in this.m_xml.Attributes)
                {
                    if (this.ID >= 0)
                    {
                        if (xmlAttribute.Name == "Title")
                        {
                            if (!this.m_bTitleChanged)
                            {
                                continue;
                            }
                        }
                        else if (xmlAttribute.Name == "Url")
                        {
                            if (!this.m_bUrlChanged)
                            {
                                continue;
                            }
                        }
                        else if (xmlAttribute.Name == "IsVisible")
                        {
                            if (!this.m_bIsVisibleChanged)
                            {
                                continue;
                            }
                        }
                        else if (xmlAttribute.Name == "Target")
                        {
                            if (!this.m_targetChanged)
                            {
                                continue;
                            }
                        }
                        else if (!list2.Contains(xmlAttribute.Name))
                        {
                            if (!this.m_changedProperties.Contains(xmlAttribute.Name))
                            {
                                continue;
                            }
                        }
                    }
                    additionsAndUpdatesWriter.WriteAttributeString(xmlAttribute.Name, xmlAttribute.Value);
                }
                if (this.ID < 0 || this.m_bOrderIndexChanged)
                {
                    additionsAndUpdatesWriter.WriteAttributeString("MLOrderIndex", this.OrderIndex.ToString());
                }
                additionsAndUpdatesWriter.WriteAttributeString("ParentID", (this.Parent == null) ? "-1" : this.Parent.ID.ToString());
                foreach (SPNavigationNode current in this.Children)
                {
                    if (current.ChangeMade || current.ID < 0)
                    {
                        current.WriteChanges(additionsAndUpdatesWriter, deletionsWriter);
                        list.Remove(current);
                    }
                }
                additionsAndUpdatesWriter.WriteEndElement();
            }
            foreach (SPNavigationNode current in this.m_deletedSubNodes)
            {
                if (current.ID >= 0)
                {
                    XmlNode xml = current.m_xml;
                    deletionsWriter.WriteStartElement("NavNode");
                    foreach (XmlAttribute xmlAttribute in xml.Attributes)
                    {
                        deletionsWriter.WriteAttributeString(xmlAttribute.Name, xmlAttribute.Value);
                    }
                    deletionsWriter.WriteAttributeString("ParentID", (this.Parent == null) ? "-1" : this.Parent.ID.ToString());
                    deletionsWriter.WriteEndElement();
                }
            }
            foreach (SPNavigationNode current2 in list)
            {
                current2.WriteChanges(additionsAndUpdatesWriter, deletionsWriter);
            }
        }

        private delegate void EmptyMethodDelegate();

        private delegate void NavigationNodeDelegate(SPNavigationNode node);

        private delegate void PropertyChangedDelegate(string sPropertyName);

        public class SPNavigationNodeCollection : IEnumerable<SPNavigationNode>, IEnumerable
        {
            private SPNavigationNode m_parent;

            private List<SPNavigationNode> m_nodes;

            public int Count
            {
                get
                {
                    return this.m_nodes.Count;
                }
            }

            public SPNavigationNode this[int i]
            {
                get
                {
                    return this.m_nodes[i];
                }
            }

            private SPNavigationNodeCollection(SPNavigationNode parent)
            {
                this.m_parent = parent;
                this.InitializePrivateFields();
            }

            private SPNavigationNodeCollection(SPNavigationNode parent, XmlNodeList nodes)
            {
                this.m_parent = parent;
                this.InitializePrivateFields();
                this.ChildrenFromXml(nodes);
            }

            public SPNavigationNode Add(SPNavigationNode node)
            {
                SPNavigationNode sPNavigationNode = this.CreateNewNavNodeFromOldNode(node);
                this.m_nodes.Add(sPNavigationNode);
                return sPNavigationNode;
            }

            public SPNavigationNode Add(string sTitle, string sUrl)
            {
                SPNavigationNode sPNavigationNode = new SPNavigationNode(this.m_parent, sTitle, sUrl);
                this.m_nodes.Add(sPNavigationNode);
                return sPNavigationNode;
            }

            public SPNavigationNode Add(XmlNode xmlNode)
            {
                SPNavigationNode sPNavigationNode = new SPNavigationNode(this.m_parent, xmlNode);
                this.m_nodes.Add(sPNavigationNode);
                return sPNavigationNode;
            }

            private void ChildrenFromXml(XmlNodeList nodes)
            {
                foreach (XmlNode node in nodes)
                {
                    SPNavigationNode sPNavigationNode = new SPNavigationNode(this.m_parent, node);
                    this.m_nodes.Add(sPNavigationNode);
                }
            }

            private XmlNode CloneXmlNode(XmlNode node)
            {
                return XmlUtility.StringToXmlNode(node.OuterXml);
            }

            private SPNavigationNode CreateNewNavNodeFromOldNode(SPNavigationNode node)
            {
                XmlNode xmlNodes = this.CloneXmlNode(node.m_xml);
                foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("//NavNode"))
                {
                    xmlNodes1.Attributes["ID"].Value = "-1";
                }
                return new SPNavigationNode(this.m_parent, xmlNodes);
            }

            public void Delete(SPNavigationNode node)
            {
                if (this.m_nodes.Remove(node))
                {
                    this.FireNodeDeleted(node);
                }
            }

            private void FireNodeDeleted(SPNavigationNode node)
            {
                if (this.NodeDeleted != null)
                {
                    this.NodeDeleted(node);
                }
            }

            internal static SPNavigationNode.SPNavigationNodeCollection GetChildNodeCollectionForNavigationNode(SPNavigationNode node)
            {
                if (node.m_children == null)
                {
                    node.m_children = new SPNavigationNode.SPNavigationNodeCollection(node);
                    node.m_children.NodeDeleted += new SPNavigationNode.NavigationNodeDelegate(node.ChildNodeDeleted);
                }
                return node.m_children;
            }

            internal static SPNavigationNode.SPNavigationNodeCollection GetChildNodeCollectionForNavigationNode(SPNavigationNode node, XmlNodeList nodes)
            {
                if (node.m_children == null)
                {
                    node.m_children = new SPNavigationNode.SPNavigationNodeCollection(node, nodes);
                    node.m_children.NodeDeleted += new SPNavigationNode.NavigationNodeDelegate(node.ChildNodeDeleted);
                }
                return node.m_children;
            }

            public IEnumerator<SPNavigationNode> GetEnumerator()
            {
                return this.m_nodes.GetEnumerator();
            }

            public int IndexOf(SPNavigationNode node)
            {
                return this.m_nodes.IndexOf(node);
            }

            private void InitializePrivateFields()
            {
                this.m_nodes = new List<SPNavigationNode>();
            }

            public SPNavigationNode Insert(SPNavigationNode node, int index)
            {
                SPNavigationNode sPNavigationNode = this.CreateNewNavNodeFromOldNode(node);
                this.m_nodes.Insert(index, sPNavigationNode);
                return sPNavigationNode;
            }

            public SPNavigationNode Insert(string sTitle, string sUrl, int index)
            {
                SPNavigationNode sPNavigationNode = new SPNavigationNode(this.m_parent, sTitle, sUrl);
                this.m_nodes.Insert(index, sPNavigationNode);
                return sPNavigationNode;
            }

            public SPNavigationNode Insert(XmlNode xmlNode, int index)
            {
                SPNavigationNode sPNavigationNode = new SPNavigationNode(this.m_parent, xmlNode);
                this.m_nodes.Insert(index, sPNavigationNode);
                return sPNavigationNode;
            }

            public void Move(SPNavigationNode node, int newIndex)
            {
                int num = this.m_nodes.IndexOf(node);
                if ((num < 0 ? false : num != newIndex))
                {
                    this.m_nodes.RemoveAt(num);
                    if (newIndex >= this.Count)
                    {
                        this.m_nodes.Add(node);
                    }
                    else
                    {
                        this.m_nodes.Insert(newIndex, node);
                    }
                    node.ChangesMade();
                }
            }

            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.m_nodes.GetEnumerator();
            }

            private event SPNavigationNode.NavigationNodeDelegate NodeDeleted;
        }

        public class SPNavigationNodeProperties
        {
            private SPNavigationNode m_parent;

            public int Count
            {
                get
                {
                    int count = this.m_parent.m_xml.Attributes.Count - (int)SPNavigationNode.NAV_NODE_CORE_ATTRIBUTES.Length;
                    return count;
                }
            }

            public string this[string sName]
            {
                get
                {
                    string str;
                    string value;
                    if (!SPNavigationNode.IsCoreAttribute(sName))
                    {
                        XmlAttribute itemOf = this.m_parent.m_xml.Attributes[sName];
                        if (itemOf == null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = itemOf.Value;
                        }
                        str = value;
                    }
                    else
                    {
                        str = null;
                    }
                    return str;
                }
                set
                {
                    if (!SPNavigationNode.IsCoreAttribute(sName))
                    {
                        if (this.m_parent.m_xml.Attributes[sName].Value != value)
                        {
                            this.m_parent.m_xml.Attributes[sName].Value = value;
                            this.FirePropertiesChanged(sName);
                        }
                    }
                }
            }

            public string[] Keys
            {
                get
                {
                    string[] name = new string[this.Count];
                    int num = 0;
                    foreach (XmlAttribute attribute in this.m_parent.m_xml.Attributes)
                    {
                        if (!SPNavigationNode.IsCoreAttribute(attribute.Name))
                        {
                            name[num] = attribute.Name;
                            num++;
                        }
                    }
                    return name;
                }
            }

            public string[] Values
            {
                get
                {
                    string[] value = new string[this.Count];
                    int num = 0;
                    foreach (XmlAttribute attribute in this.m_parent.m_xml.Attributes)
                    {
                        if (!SPNavigationNode.IsCoreAttribute(attribute.Name))
                        {
                            value[num] = attribute.Value;
                            num++;
                        }
                    }
                    return value;
                }
            }

            private SPNavigationNodeProperties(SPNavigationNode parent)
            {
                this.m_parent = parent;
            }

            public void Add(string sName, string sValue)
            {
                if (SPNavigationNode.IsCoreAttribute(sName))
                {
                    throw new Exception("Cannote set this NavNode property. This is a reserved property name.");
                }
                XmlAttribute xmlAttribute = this.m_parent.m_xml.OwnerDocument.CreateAttribute(sName);
                xmlAttribute.Value = sValue;
                this.m_parent.m_xml.Attributes.Append(xmlAttribute);
                this.FirePropertiesChanged(sName);
            }

            public void Clear()
            {
                List<XmlAttribute> list = new List<XmlAttribute>();
                foreach (XmlAttribute xmlAttribute in this.m_parent.m_xml.Attributes)
                {
                    if (!SPNavigationNode.IsCoreAttribute(xmlAttribute.Name))
                    {
                        list.Add(xmlAttribute);
                    }
                }
                foreach (XmlAttribute xmlAttribute in list)
                {
                    this.m_parent.m_xml.Attributes.Remove(xmlAttribute);
                }
            }

            public bool ContainsKey(string sKey)
            {
                bool flag;
                string[] keys = this.Keys;
                int num = 0;
                while (true)
                {
                    if (num >= (int)keys.Length)
                    {
                        flag = false;
                        break;
                    }
                    else if (!(keys[num] == sKey))
                    {
                        num++;
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }

            private void FirePropertiesChanged(string sPropertyName)
            {
                if (this.PropertiesChanged != null)
                {
                    this.PropertiesChanged(sPropertyName);
                }
            }

            internal static SPNavigationNode.SPNavigationNodeProperties GetPropertiesForNavigationNode(SPNavigationNode node)
            {
                if (node.m_properties == null)
                {
                    node.m_properties = new SPNavigationNode.SPNavigationNodeProperties(node);
                    node.m_properties.PropertiesChanged += new SPNavigationNode.PropertyChangedDelegate(node.PropertyChanged);
                }
                return node.m_properties;
            }

            public void Remove(string sName)
            {
                if (!SPNavigationNode.IsCoreAttribute(sName))
                {
                    XmlAttribute itemOf = this.m_parent.m_xml.Attributes[sName];
                    if (itemOf != null)
                    {
                        this.m_parent.m_xml.Attributes.Remove(itemOf);
                    }
                }
            }

            private event SPNavigationNode.PropertyChangedDelegate PropertiesChanged;
        }
    }
}