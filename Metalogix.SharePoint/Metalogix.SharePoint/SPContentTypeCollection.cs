using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
    public class SPContentTypeCollection : IXMLAbleList, IXmlable, IEnumerable
    {
        private List<SPContentType> m_data = new List<SPContentType>();

        private SPList m_parentList = null;

        private SPWeb m_parentWeb = null;

        public Type CollectionType
        {
            get
            {
                return typeof(SPContentType);
            }
        }

        public int Count
        {
            get
            {
                return this.m_data.Count;
            }
        }
        
        public SPFieldCollection FieldsAvailable
        {
            get
            {
                SPFieldCollection result;
                if (this.m_parentList != null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlTextWriter.WriteStartElement("Fields");
                    SPFieldCollection fields = this.ParentList.GetFields(false);
                    foreach (SPField sPField in fields)
                    {
                        xmlTextWriter.WriteRaw(sPField.FieldXML.OuterXml);
                    }
                    foreach (SPField sPField in this.m_parentWeb.ContentTypes.FieldsAvailable)
                    {
                        if (fields.GetFieldByNames(sPField.DisplayName, sPField.Name) == null)
                        {
                            xmlTextWriter.WriteRaw(sPField.FieldXML.OuterXml);
                        }
                    }
                    xmlTextWriter.WriteEndElement();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(stringBuilder.ToString());
                    result = new SPFieldCollection(this.ParentWeb, xmlDocument.FirstChild);
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlTextWriter.WriteStartElement("Fields");
                    SPFieldCollection availableColumns = this.m_parentWeb.GetAvailableColumns(false);
                    foreach (SPField sPField in availableColumns)
                    {
                        xmlTextWriter.WriteRaw(sPField.FieldXML.OuterXml);
                    }
                    xmlTextWriter.WriteEndElement();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(stringBuilder.ToString());
                    result = new SPFieldCollection(this.ParentWeb, xmlDocument.FirstChild);
                }
                return result;
            }
        }

        public SPContentType this[string sContentTypeID]
        {
            get
            {
                SPContentType sPContentType;
                if (!string.IsNullOrEmpty(sContentTypeID))
                {
                    foreach (SPContentType sPContentType1 in this)
                    {
                        string contentTypeID = sPContentType1.ContentTypeID;
                        if ((contentTypeID == null ? false : string.Equals(contentTypeID, sContentTypeID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            sPContentType = sPContentType1;
                            return sPContentType;
                        }
                    }
                    sPContentType = null;
                }
                else
                {
                    sPContentType = null;
                }
                return sPContentType;
            }
        }

        public object this[int index]
        {
            get
            {
                return this.m_data[index];
            }
        }

        public SPList ParentList
        {
            get
            {
                return this.m_parentList;
            }
        }

        public SPWeb ParentWeb
        {
            get
            {
                return this.m_parentWeb;
            }
        }

        public SPContentTypeCollection(SPWeb parent)
        {
            this.m_parentWeb = parent;
        }

        public SPContentTypeCollection(SPList parent)
        {
            this.m_parentList = parent;
            this.m_parentWeb = parent.ParentWeb;
        }

        public SPContentType AddOrUpdateContentType(string sContentTypeName, string sContentTypeXml, string sParentContentTypeName)
        {
            XmlNode xmlNodes = null;
            SPContentType sPContentType = this.AddOrUpdateContentType(sContentTypeName, sContentTypeXml, sParentContentTypeName, false, out xmlNodes);
            return sPContentType;
        }

        public SPContentType AddOrUpdateContentType(string sContentTypeName, string sContentTypeXml, string sParentContentTypeName, bool bMakeDefaultContentType, out XmlNode resultNode)
        {
            SPNode parentList;
            if (this.ParentList != null)
            {
                parentList = this.ParentList;
            }
            else
            {
                parentList = this.ParentWeb;
            }
            SPNode sPNode = parentList;
            bool writeVirtually = sPNode.WriteVirtually;
            string xML = null;
            if (writeVirtually)
            {
                xML = this.ToXML();
            }
            else if (this.ParentWeb.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            SPContentType contentTypeByParentName = null;
            if ((this.m_parentList == null ? false : !string.IsNullOrEmpty(sParentContentTypeName)))
            {
                contentTypeByParentName = this.GetContentTypeByParentName(sParentContentTypeName);
            }
            if (contentTypeByParentName == null)
            {
                contentTypeByParentName = this.GetContentTypeByName(sContentTypeName);
            }
            string str = null;
            if (AdapterConfigurationVariables.MigrateLanguageSettings)
            {
                sContentTypeXml = XmlUtility.AddLanguageSettingsAttribute(sContentTypeXml, "ContentType", null);
            }
            if (writeVirtually)
            {
                str = sContentTypeXml;
            }
            else if (this.m_parentList != null)
            {
                if ((contentTypeByParentName != null ? false : this.ParentWeb.ContentTypes.GetContentTypeByName(sParentContentTypeName) == null))
                {
                    throw new Exception(string.Format("Cannot add content type to list. The specified parent content type {0}does not exist on the target site.", (string.IsNullOrEmpty(sParentContentTypeName) ? "" : string.Format("\"{0}\" ", sParentContentTypeName))));
                }
                str = this.ParentWeb.Adapter.Writer.ApplyOrUpdateContentType(this.m_parentList.ConstantID, sParentContentTypeName, sContentTypeXml, bMakeDefaultContentType);
            }
            else
            {
                str = this.ParentWeb.Adapter.Writer.AddOrUpdateContentType(sContentTypeXml, sParentContentTypeName);
            }
            int num = -1;
            if (contentTypeByParentName != null)
            {
                num = this.m_data.IndexOf(contentTypeByParentName);
                this.m_data.Remove(contentTypeByParentName);
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            resultNode = xmlDocument.SelectSingleNode("//Results");
            SPContentType sPContentType = new SPContentType(this, xmlDocument.SelectSingleNode("//ContentType"));
            if (num < 0)
            {
                this.m_data.Add(sPContentType);
            }
            else
            {
                this.m_data.Insert(num, sPContentType);
            }
            if (writeVirtually)
            {
                string xML1 = this.ToXML();
                sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(xML1), "ContentTypes");
            }
            return sPContentType;
        }

        public void DeleteContentType(SPContentType contentType)
        {
            string constantID;
            SPNode parentList;
            if (this.m_parentList == null)
            {
                constantID = null;
            }
            else
            {
                constantID = this.m_parentList.ConstantID;
            }
            string str = constantID;
            if (this.ParentList != null)
            {
                parentList = this.ParentList;
            }
            else
            {
                parentList = this.ParentWeb;
            }
            SPNode sPNode = parentList;
            if (!sPNode.WriteVirtually)
            {
                if (this.ParentWeb.Adapter.Writer == null)
                {
                    throw new Exception(Resources.TargetIsReadOnly);
                }
                ISharePointWriter writer = this.ParentWeb.Adapter.Writer;
                string[] contentTypeID = new string[] { contentType.ContentTypeID };
                writer.DeleteContentTypes(str, contentTypeID);
                this.m_data.Remove(contentType);
            }
            else
            {
                string xML = this.ToXML();
                this.m_data.Remove(contentType);
                string xML1 = this.ToXML();
                sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(xML1), "ContentTypes");
            }
        }

        // Metalogix.SharePoint.SPContentTypeCollection
        public void DeleteContentTypes(IEnumerable<SPContentType> contentTypes)
        {
            string sListID = (this.m_parentList == null) ? null : this.m_parentList.ConstantID;
            List<string> list = new List<string>();
            foreach (SPContentType current in contentTypes)
            {
                list.Add(current.ContentTypeID);
            }
            if (list.Count != 0)
            {
                SPNode sPNode = (this.ParentList != null) ? (SPNode) this.ParentList : this.ParentWeb;
                if (sPNode.WriteVirtually)
                {
                    string sXml = this.ToXML();
                    foreach (SPContentType current in contentTypes)
                    {
                        this.m_data.Remove(current);
                    }
                    string sXml2 = this.ToXML();
                    sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(sXml), XmlUtility.StringToXmlNode(sXml2), "ContentTypes");
                }
                else
                {
                    if (this.ParentWeb.Adapter.Writer == null)
                    {
                        throw new Exception(Resources.TargetIsReadOnly);
                    }
                    this.ParentWeb.Adapter.Writer.DeleteContentTypes(sListID, list.ToArray());
                    foreach (SPContentType current in contentTypes)
                    {
                        this.m_data.Remove(current);
                    }
                }
            }
        }

        public void FetchData()
        {
            string constantID;
            SPNode parentList;
            if (this.ParentList != null)
            {
                constantID = this.ParentList.ConstantID;
            }
            else
            {
                constantID = null;
            }
            string contentTypes = this.ParentWeb.Adapter.Reader.GetContentTypes(constantID);
            if (contentTypes != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(contentTypes);
                if (this.ParentList != null)
                {
                    parentList = this.ParentList;
                }
                else
                {
                    parentList = this.ParentWeb;
                }
                this.FromXML(parentList.AttachVirtualData(xmlDocument.DocumentElement, "ContentTypes"));
            }
        }

        public void FromXML(XmlNode xmlNode)
        {
            this.m_data.Clear();
            foreach (XmlNode xmlNode2 in xmlNode.SelectNodes(".//ContentType"))
            {
                if (xmlNode2.Attributes["Name"] != null)
                {
                    this.m_data.Add(new SPContentType(this, xmlNode2));
                }
                else if (this.ParentList != null && this.ParentWeb != null && xmlNode2.Attributes["ID"] != null)
                {
                    string value = xmlNode2.Attributes["ID"].Value;
                    bool flag = false;
                    foreach (SPContentType current in this.m_data)
                    {
                        if (current.ContentTypeID == value)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        SPContentType sPContentType = this.ParentWeb.ContentTypes[value];
                        if (sPContentType != null)
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(sPContentType.XML);
                            SPContentType sPContentType2 = new SPContentType(this, xmlDocument.SelectSingleNode("//ContentType"));
                            foreach (SPContentType current in this.m_data)
                            {
                                if (current.Name == sPContentType2.Name && current.ContentTypeID.StartsWith(current.ContentTypeID) && current.ContentTypeID.Length == sPContentType2.ContentTypeID.Length + 34)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                this.m_data.Add(sPContentType2);
                            }
                        }
                    }
                }
            }
        }

        public SPContentType GetContentTypeByName(string sContentTypeName)
        {
            SPContentType sPContentType;
            foreach (SPContentType sPContentType1 in this)
            {
                if (sPContentType1.Name == sContentTypeName)
                {
                    sPContentType = sPContentType1;
                    return sPContentType;
                }
            }
            sPContentType = null;
            return sPContentType;
        }

        public SPContentType GetContentTypeByParentName(string sContentTypeParentName)
        {
            SPContentType sPContentType;
            foreach (SPContentType sPContentType1 in this)
            {
                SPContentType parentContentType = sPContentType1.ParentContentType;
                if ((parentContentType == null ? false : parentContentType.Name == sContentTypeParentName))
                {
                    sPContentType = sPContentType1;
                    return sPContentType;
                }
            }
            sPContentType = null;
            return sPContentType;
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_data.GetEnumerator();
        }

        public SPFieldCollection GetReferencedFields()
        {
            SPFieldCollection sPFieldCollections;
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            List<SPField> sPFields = new List<SPField>();
            SPFieldCollection fieldsAvailable = this.FieldsAvailable;
            foreach (SPContentType sPContentType in this)
            {
                foreach (SPField fieldsByIdOrName in fieldsAvailable.GetFieldsByIdOrName(sPContentType.GetFieldReferences()))
                {
                    bool flag = true;
                    foreach (SPField sPField in sPFields)
                    {
                        if (sPField.Name == fieldsByIdOrName.Name)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        sPFields.Add(fieldsByIdOrName);
                    }
                }
            }
            foreach (SPField sPField1 in sPFields)
            {
                xmlTextWriter.WriteRaw(sPField1.FieldXML.OuterXml);
            }
            xmlTextWriter.WriteEndElement();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            sPFieldCollections = (this.m_parentList == null ? new SPFieldCollection(this.m_parentWeb, xmlDocument.FirstChild) : new SPFieldCollection(this.m_parentList, xmlDocument.FirstChild));
            return sPFieldCollections;
        }

        public void Sort()
        {
            this.Sort((SPContentType source, SPContentType target) => (source != target ? string.Compare(source.ToString(), target.ToString()) : 0));
        }

        public void Sort(Comparison<SPContentType> comparison)
        {
            this.m_data.Sort(comparison);
        }

        public SPContentType[] ToArray()
        {
            return this.m_data.ToArray();
        }

        public string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            this.ToXML(new XmlTextWriter(stringWriter));
            return stringWriter.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ContentTypes");
            foreach (SPContentType mDatum in this.m_data)
            {
                xmlWriter.WriteRaw(mDatum.XML);
            }
            xmlWriter.WriteEndElement();
        }
    }
}