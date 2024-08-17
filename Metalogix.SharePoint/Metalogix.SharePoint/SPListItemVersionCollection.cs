using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
    public class SPListItemVersionCollection : ListItemVersionCollection
    {
        private SPListItem m_parentItem;

        private bool m_bHasTempVersions = false;

        public bool HasTempVersions
        {
            get
            {
                return this.m_bHasTempVersions;
            }
        }

        public SPListItemVersionCollection(SPListItem parentItem) : base(parentItem, parentItem.ParentList, parentItem.ParentFolder, null)
        {
            this.m_parentItem = parentItem;
        }

        public SPListItemVersionCollection(XmlNode itemCollectionNode) : base(null, null, null, null)
        {
            this.FromXML(itemCollectionNode);
        }

        public SPListItemVersionCollection(ListItemVersionCollection itemCollection) : base(itemCollection.ParentItem, itemCollection.ParentList, itemCollection.ParentFolder, itemCollection.ToArray())
        {
            this.m_parentItem = (SPListItem)itemCollection.ParentItem;
        }

        public override void Add(Node item)
        {
            throw new Exception("To add a new version add a list item");
        }

        public void FetchData()
        {
            this.FetchData(false);
        }

        public void FetchData(bool bIncludeTempVersions)
        {
            if (this.m_parentItem.CanHaveVersions)
            {
                int noOfLatestVersionsToGet = this.m_parentItem.NoOfLatestVersionsToGet;
                StringBuilder stringBuilder = new StringBuilder(1024);
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                {
                    xmlWriter.WriteStartElement(XmlElementNames.ListItemVersionSettings.ToString());
                    xmlWriter.WriteAttributeString(XmlAttributeNames.NoOfLatestVersionsToGet.ToString(), noOfLatestVersionsToGet.ToString());
                    xmlWriter.WriteRaw(this.m_parentItem.ParentList.ListSettingsXML);
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                }
                string sFields = string.Empty;
                if (this.m_parentItem.Adapter.IsDB && !string.IsNullOrEmpty(this.m_parentItem.ParentList.OriginalFieldsSchemaXML))
                {
                    sFields = this.m_parentItem.ParentList.OriginalFieldsSchemaXML;
                }
                else
                {
                    sFields = this.m_parentItem.ParentList.Fields.XML;
                }
                string listItemVersions = this.m_parentItem.Adapter.Reader.GetListItemVersions(this.m_parentItem.ParentList.ConstantID, this.m_parentItem.ConstantID, sFields, stringBuilder.ToString());
                if (listItemVersions != null)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(listItemVersions);
                    XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//ListItem");
                    int num = 0;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        if ((xmlNode.Attributes["ContentType"] == null || string.IsNullOrEmpty(xmlNode.Attributes["ContentType"].Value)) && xmlNode.Attributes["ContentTypeId"] != null && !string.IsNullOrEmpty(xmlNodeList[num].Attributes["ContentTypeId"].Value))
                        {
                            foreach (SPContentType sPContentType in this.m_parentItem.ParentList.ContentTypes)
                            {
                                if (sPContentType.ContentTypeID == xmlNode.Attributes["ContentTypeId"].Value)
                                {
                                    if (xmlNode.Attributes["ContentType"] == null)
                                    {
                                        XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("ContentType");
                                        xmlAttribute.Value = sPContentType.Name;
                                        xmlNode.Attributes.Append(xmlAttribute);
                                    }
                                    else
                                    {
                                        xmlNode.Attributes["ContentType"].Value = sPContentType.Name;
                                    }
                                    break;
                                }
                            }
                        }
                        SPListItemVersion sPListItemVersion = new SPListItemVersion(this.m_parentItem, this.m_parentItem.ParentList, this.m_parentItem.ParentFolder, xmlNode);
                        if (this.m_parentItem.ParentList.IsDocumentLibrary && num == xmlNodeList.Count - 1)
                        {
                            XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sPListItemVersion.XML);
                            if (xmlNode2.Attributes["CheckoutUser"] != null && xmlNode2.Attributes["CheckoutUser"].Value.Equals(this.m_parentItem.Adapter.Credentials.UserName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }
                        }
                        if (!sPListItemVersion.IsTempVersion || bIncludeTempVersions)
                        {
                            base.AddToCollection(sPListItemVersion);
                        }
                        if (sPListItemVersion.IsTempVersion)
                        {
                            this.m_bHasTempVersions = true;
                        }
                        num++;
                    }
                }
            }
            else
            {
                base.AddToCollection(new SPListItemVersion(this.m_parentItem, this.m_parentItem.ParentList, this.m_parentItem.ParentList, this.m_parentItem.ConstantID));
            }
        }

        public override void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("./ParentItem/Location");
            this.m_parentItem = (SPListItem)(new Location(xmlNodes)).GetNode();
            string[] strArrays = xmlNode.SelectSingleNode("./VersionNumbers").InnerText.Split(new char[] { ',' });
            SPListItemVersionCollection versionHistory = (SPListItemVersionCollection)this.m_parentItem.VersionHistory;
            if (versionHistory != null)
            {
                string[] strArrays1 = strArrays;
                for (int i = 0; i < (int)strArrays1.Length; i++)
                {
                    string str = strArrays1[i];
                    SPListItemVersion versionByVersionString = versionHistory.GetVersionByVersionString(str);
                    if (!(versionByVersionString != null))
                    {
                        string[] serverRelativeUrl = new string[] { "The version with version string '", str, "' could not be found for the SPListItem '", this.m_parentItem.ServerRelativeUrl, "' when trying to create an SPListItemVersionCollection." };
                        throw new Exception(string.Concat(serverRelativeUrl));
                    }
                    base.AddToCollection(versionByVersionString);
                }
            }
            else if ((int)strArrays.Length > 0)
            {
                throw new Exception(string.Concat("XML for an SPListItemVersionCollection was found that has version numbers listed, but the parent item has no version history. The XML is: '", xmlNode.OuterXml, "'."));
            }
        }

        public SPListItemVersion GetVersionByVersionString(string sVersion)
        {
            SPListItemVersion sPListItemVersion;
            sVersion = (sVersion.Contains(".") ? sVersion : string.Concat(sVersion, ".0"));
            foreach (SPListItemVersion sPListItemVersion1 in this)
            {
                if ((sPListItemVersion1.VersionString.Contains(".") ? sPListItemVersion1.VersionString : string.Concat(sPListItemVersion1.VersionString, ".0")) == sVersion)
                {
                    sPListItemVersion = sPListItemVersion1;
                    return sPListItemVersion;
                }
            }
            sPListItemVersion = null;
            return sPListItemVersion;
        }

        private string GetVersionStrings()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            foreach (SPListItemVersion sPListItemVersion in this)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append(sPListItemVersion.VersionString);
            }
            return stringBuilder.ToString();
        }

        public override bool Remove(Node item)
        {
            throw new Exception("To remove a new version remove a list item");
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("SPListItemVersionCollection");
            xmlTextWriter.WriteStartElement("ParentItem");
            this.m_parentItem.Location.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteElementString("VersionNumbers", this.GetVersionStrings());
            xmlTextWriter.WriteEndElement();
        }
    }
}