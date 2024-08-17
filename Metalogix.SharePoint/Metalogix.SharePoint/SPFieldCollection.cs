using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
    public class SPFieldCollection : FieldCollection, IEnumerable<SPField>, IEnumerable
    {
        private readonly static string[] PUBLISHING_INFRASTRUCTURE_SITE_COLUMN_GUIDS;

        private XmlNode m_fieldsXML;

        private SPWeb m_parentWeb;

        private SPList m_parentList;

        private Dictionary<Guid, SPField> m_fieldsById;

        private Dictionary<string, SPField> m_fieldsByName;

        private FieldsLookUp m_fieldsSchemaByName;

        public int Count
        {
            get
            {
                return this.m_fieldsByName.Count;
            }
        }

        private XmlNode FieldsXML
        {
            get
            {
                return this.m_fieldsXML;
            }
        }

        public bool HasDependencies
        {
            get
            {
                XmlNode fieldsXML = this.FieldsXML;
                XmlNodeList xmlNodeLists = fieldsXML.SelectNodes("//Field[@Type='Lookup']");
                XmlNodeList xmlNodeLists1 = fieldsXML.SelectNodes("//Field[@Type='LookupMulti']");
                return ((xmlNodeLists.Count > 0 ? false : xmlNodeLists1.Count <= 0) ? false : true);
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

        public bool TaxonomyFieldsExist
        {
            get
            {
                return this.m_fieldsXML.SelectNodes("//Field[@Type='TaxonomyFieldType' or @Type='TaxonomyFieldTypeMulti']").Count > 0;
            }
        }

        public string XML
        {
            get
            {
                return this.m_fieldsXML.OuterXml;
            }
        }

        static SPFieldCollection()
        {
            string[] strArrays = new string[] { "b1996002-9167-45e5-a4df-b2c41c6723c7", "3de94b06-4120-41a5-b907-88773e493458", "7581e709-5d87-42e7-9fe6-698ef5e86dd3", "983f490b-fc53-4820-9354-e8de646b4b82", "7546ad0d-6c33-4501-b470-fb3003ca14ba", "0f800910-b30d-4c8f-b011-8189b2297094", "27761311-936a-40ba-80cd-ca5e7a540a36", "4689a812-320e-4623-aab9-10ad68941126", "51d39414-03dc-4bd0-b777-d3e20cb350f7", "d4a6af1d-c6d7-4045-8def-cefa25b9ec30", "59cd571e-e2d9-485d-bb5d-e70d12f8d0b7", "de38f937-8578-435e-8cd3-50be3ea59253", "a932ec3f-94c1-48b1-b6dc-41aaa6eb7e54", "3894ec3f-4674-4924-a440-8872bec40cf9", "890e9d41-5a0e-4988-87bf-0fb9d80f60df", "a990e64f-faa3-49c1-aafa-885fda79de62", "d211d750-4fe6-4d92-90e8-eb16dff196c8", "773ed051-58db-4ff2-879b-08b21ab001e0", "c80f535b-a430-4273-8f4f-f3e95507b62a", "dc47d55f-9bf9-494a-8d5b-e619214dd19a", "b8abfc64-c2bd-4c88-8cef-b040c1b9d8c0", "70b38565-a310-4546-84a7-709cfdc140cf", "61cbb965-1e04-4273-b658-eedaa662f48d", "d8f18167-7cff-4c4e-bdbe-e7b0f01678f3", "188ce56c-61e0-4d2a-9d3e-7561390668f7", "ac57186e-e90b-4711-a038-b6c6a62a57dc", "9550e77a-4d10-464f-bc0c-102d5b1aec42", "5b4d927c-d383-496b-bc79-1e61bd383019", "92bba27e-eef6-41aa-b728-6dd9caf2bde2", "914fdb80-7d4f-4500-bf4c-ce46ad7484a4", "f55c4d88-1f2e-4ad9-aaa8-819af4ee7ee8", "c79dba91-e60b-400e-973d-c6d06f192720", "766da693-38e5-4b1b-997f-e830b6dfcc7b", "e977ed93-da24-4fcc-b77d-ac34eea7288f", "75bed596-0661-4edd-9724-1d607ab8d3b5", "3a4b7f98-8d14-4800-8bf5-9ad1dd6a82ee", "32e03f99-6949-466a-a4a6-057c21d4b516", "db03cb99-cf1e-40b8-adc7-913f7181dac3", "5a14d1ab-1513-48c7-97b3-657a5ba6c742", "84cd09bd-85a9-461f-86e3-4c3c1738ad6b", "18f165be-6285-4a57-b3ab-4e9f913d299f", "82dd22bf-433e-4260-b26e-5b8360dd9105", "b510aac1-bba3-4652-ab70-2d756c29540f", "bdd1b3c3-18db-4acf-a963-e70ef4227fbc", "d3429cc9-adc4-439b-84a8-5679070f84cb", "543bc2cf-1f30-488e-8f25-6fe3b689d9ac", "aea1a4dd-0f19-417d-8721-95a1d28762ab", "0a90b5e8-185a-4dec-bf3c-e60aae08373f", "66f500e9-7955-49ab-abb1-663621727d10", "71316cea-40a0-49f3-8659-f0cefdbdbd4f", "89587dfd-b9ca-4fae-8eb9-ba779e917d48", "b3525efe-59b5-4f0f-b1e4-6e26cb6ef6aa" };
            SPFieldCollection.PUBLISHING_INFRASTRUCTURE_SITE_COLUMN_GUIDS = strArrays;
        }

        public SPFieldCollection(SPList parentList, XmlNode fieldsXML)
        {
            this.m_parentList = parentList;
            this.m_parentWeb = parentList.ParentWeb;
            this.m_fieldsXML = fieldsXML;
            this.m_fieldsById = new Dictionary<Guid, SPField>();
            this.m_fieldsByName = new Dictionary<string, SPField>();
            this.m_fieldsSchemaByName = new FieldsLookUp();
            this.ResetFields();
        }

        public SPFieldCollection(SPWeb parentWeb, XmlNode fieldsXML)
        {
            this.m_parentWeb = parentWeb;
            this.m_fieldsXML = fieldsXML;
            this.m_fieldsById = new Dictionary<Guid, SPField>();
            this.m_fieldsByName = new Dictionary<string, SPField>();
            this.m_fieldsSchemaByName = new FieldsLookUp();
            this.ResetFields();
        }

        public SPField AddField(string sFieldXML)
        {
            XmlDocument xmlDocument;
            XmlNode xmlNodes;
            string d;
            string value = null;
            if (AdapterConfigurationVariables.MigrateLanguageSettings)
            {
                sFieldXML = XmlUtility.AddLanguageSettingsAttribute(sFieldXML, "Field", null);
            }
            if (!(this.m_parentList == null ? true : !this.m_parentList.WriteVirtually))
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFieldXML);
                xmlNodes = xmlDocument.SelectSingleNode("//Field");
                value = xmlNodes.Attributes["Name"].Value;
                this.UpdateListFieldsVirtually(xmlNodes);
            }
            else if ((this.m_parentWeb == null ? true : !this.m_parentWeb.WriteVirtually))
            {
                if (this.m_parentWeb.Adapter.Writer == null)
                {
                    throw new Exception(Resources.TargetIsReadOnly);
                }
                if (this.m_parentList != null)
                {
                    d = this.m_parentList.ID;
                }
                else
                {
                    d = null;
                }
                string str = d;
                string d1 = this.m_parentWeb.ID;
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFieldXML);
                xmlNodes = xmlDocument.SelectSingleNode("//Field");
                value = xmlNodes.Attributes["Name"].Value;
                string str1 = this.m_parentWeb.Adapter.Writer.AddFields(str, string.Concat("<FieldCollection>", xmlNodes.OuterXml, "</FieldCollection>"));
                xmlDocument.LoadXml(str1);
                this.m_fieldsXML = xmlDocument.DocumentElement;
                this.ResetFields();
                this.NotifyFieldCollectionChanged(this.m_fieldsXML);
            }
            else
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFieldXML);
                xmlNodes = xmlDocument.SelectSingleNode("//Field");
                value = xmlNodes.Attributes["Name"].Value;
                this.UpdateSiteColumnsVirtually(xmlDocument.DocumentElement);
            }
            return this.GetFieldByNames(null, value);
        }

        public SPFieldCollection AddFieldCollection(string sFieldXML)
        {
            string d;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFieldXML);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<FieldCollection>");
            Dictionary<string, SPField> strs = new Dictionary<string, SPField>();
            foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Field"))
            {
                SPField sPField = new SPField(xmlNodes);
                if (!strs.ContainsKey(sPField.Name))
                {
                    strs.Add(sPField.Name, sPField);
                    string outerXml = xmlNodes.OuterXml;
                    if (AdapterConfigurationVariables.MigrateLanguageSettings)
                    {
                        outerXml = XmlUtility.AddLanguageSettingsAttribute(outerXml, "Field", null);
                    }
                    stringBuilder.Append(outerXml);
                }
            }
            stringBuilder.Append("</FieldCollection>");
            if (!(this.m_parentList == null ? true : !this.m_parentList.WriteVirtually))
            {
                this.UpdateListFieldsVirtually(XmlUtility.StringToXmlNode(stringBuilder.ToString()));
            }
            else if ((this.m_parentWeb == null ? true : !this.m_parentWeb.WriteVirtually))
            {
                if (this.m_parentWeb.Adapter.Writer == null)
                {
                    throw new Exception("The underlying SharePoint adapter does not support write operations");
                }
                if (this.m_parentList != null)
                {
                    d = this.m_parentList.ID;
                }
                else
                {
                    d = null;
                }
                string str = d;
                string d1 = this.m_parentWeb.ID;
                string str1 = this.m_parentWeb.Adapter.Writer.AddFields(str, stringBuilder.ToString());
                xmlDocument.LoadXml(str1);
                this.m_fieldsXML = xmlDocument.DocumentElement;
                this.ResetFields();
                this.NotifyFieldCollectionChanged(this.m_fieldsXML);
            }
            else
            {
                this.UpdateSiteColumnsVirtually(XmlUtility.StringToXmlNode(stringBuilder.ToString()));
            }
            return this.GetFieldsByIdOrName(strs.Values);
        }

        private static void AddListColumnAttribute(XmlNode fieldNode, SPFieldCollection sourceFieldCollection)
        {
            if ((sourceFieldCollection == null ? false : fieldNode.OwnerDocument != null))
            {
                XmlAttribute str = fieldNode.OwnerDocument.CreateAttribute("IsListColumn");
                Guid attributeValueAsGuid = fieldNode.GetAttributeValueAsGuid("ID");
                bool flag = !sourceFieldCollection.FieldIDExists(attributeValueAsGuid);
                str.Value = flag.ToString();
                fieldNode.Attributes.Append(str);
            }
        }

        public bool FieldDisplayNameExists(string displayName)
        {
            bool flag;
            foreach (SPField value in this.m_fieldsByName.Values)
            {
                if (string.Equals(displayName, value.DisplayName))
                {
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        public bool FieldIDExists(Guid fieldId)
        {
            return this.m_fieldsById.ContainsKey(fieldId);
        }

        public bool FieldNameExists(string fieldName)
        {
            return this.m_fieldsByName.ContainsKey(fieldName);
        }

        private void FilterFieldXml(XmlNode fieldsNode, IFilterExpression SiteFieldsFilter, bool removePublishingInfrastructureSiteColumns)
        {
            bool flag;
            List<string> strs = null;
            if (removePublishingInfrastructureSiteColumns)
            {
                strs = new List<string>(SPFieldCollection.PUBLISHING_INFRASTRUCTURE_SITE_COLUMN_GUIDS);
            }
            foreach (XmlNode xmlNodes in fieldsNode.SelectNodes("./Field"))
            {
                string lower = null;
                if (removePublishingInfrastructureSiteColumns)
                {
                    XmlAttribute itemOf = xmlNodes.Attributes["ID"];
                    if (itemOf != null)
                    {
                        string value = itemOf.Value;
                        char[] chrArray = new char[] { '{', '}' };
                        lower = value.Trim(chrArray).ToLower();
                    }
                }
                if (!SiteFieldsFilter.Evaluate(new SPField(xmlNodes), new CompareDatesInUtc()))
                {
                    flag = false;
                }
                else
                {
                    flag = (string.IsNullOrEmpty(lower) ? true : !strs.Contains(lower));
                }
                if (!flag)
                {
                    fieldsNode.RemoveChild(xmlNodes);
                }
            }
        }

        private void FilterManagedMetadataNoteFieldXml(XmlNode fieldsNode)
        {
            if (fieldsNode == null)
            {
                throw new ArgumentNullException("fieldsNode");
            }
            foreach (SPField taxonomyField in this.GetTaxonomyFields())
            {
                if (taxonomyField.TaxonomyHiddenTextField != Guid.Empty)
                {
                    XmlNode xmlNodes = fieldsNode.SelectSingleNode(string.Format("//Field[@ID='{0}' or @Name='{1}']", taxonomyField.FieldXML.GetAttributeValueAsString("ID"), taxonomyField.FieldXML.GetAttributeValueAsString("Name")));
                    if (xmlNodes != null)
                    {
                        XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//Property[Name='TextField']");
                        if (xmlNodes1 != null)
                        {
                            xmlNodes1.ParentNode.RemoveChild(xmlNodes1);
                        }
                    }
                    SPField fieldById = this.GetFieldById(taxonomyField.TaxonomyHiddenTextField);
                    if (fieldById != null)
                    {
                        XmlNode xmlNodes2 = fieldsNode.SelectSingleNode(string.Format("//Field[@ID='{0}' or @Name='{1}']", fieldById.FieldXML.GetAttributeValueAsString("ID"), fieldById.FieldXML.GetAttributeValueAsString("Name")));
                        if (xmlNodes2 != null)
                        {
                            xmlNodes2.ParentNode.RemoveChild(xmlNodes2);
                        }
                    }
                }
            }
        }

        public string GenerateUniqueDisplayNameFrom(string displayName)
        {
            return this.GenerateUniqueName(displayName, true);
        }

        public string GenerateUniqueFieldNameFrom(string fieldName)
        {
            return this.GenerateUniqueName(fieldName, false);
        }

        private string GenerateUniqueName(string fieldName, bool checkAgainstDisplayName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName is null or empty.");
            }
            StringBuilder stringBuilder = new StringBuilder(48);
            StringBuilder stringBuilder2 = new StringBuilder(48);
            bool flag = false;
            stringBuilder.Append(fieldName.Replace(" ", string.Empty));
            int num = 0;
            do
            {
                stringBuilder2.Length = 0;
                stringBuilder2.Append(string.Format("{0}{1}", stringBuilder, num));
                if (stringBuilder2.Length > 32)
                {
                    int num2 = stringBuilder2.Length - 32;
                    stringBuilder.Length -= num2;
                    stringBuilder2.Length = 0;
                    stringBuilder2.Append(string.Format("{0}{1}", stringBuilder, num));
                }
                if (checkAgainstDisplayName ? this.FieldDisplayNameExists(stringBuilder2.ToString()) : this.FieldNameExists(stringBuilder2.ToString()))
                {
                    num++;
                }
                else
                {
                    flag = true;
                }
            }
            while (!flag);
            return stringBuilder2.ToString();
        }

        public SPList[] GetDependencies()
        {
            ArrayList arrayLists = new ArrayList();
            foreach (XmlNode xmlNodes in this.FieldsXML.SelectNodes("//Field[@Type='Lookup' or @Type='LookupMulti']"))
            {
                if (xmlNodes.Attributes["TargetListName"] != null)
                {
                    SPList item = this.m_parentWeb.Lists[xmlNodes.Attributes["TargetListName"].Value];
                    if ((item == null ? false : !arrayLists.Contains(item)))
                    {
                        arrayLists.Add(item);
                    }
                }
            }
            SPList[] sPListArray = new SPList[arrayLists.Count];
            arrayLists.CopyTo(sPListArray);
            return sPListArray;
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_fieldsByName.Values.GetEnumerator();
        }

        public SPField GetFieldById(Guid fieldId)
        {
            SPField item;
            if (this.FieldIDExists(fieldId))
            {
                item = this.m_fieldsById[fieldId];
            }
            else
            {
                item = null;
            }
            return item;
        }

        public SPField GetFieldByName(string fieldName)
        {
            SPField item;
            if (string.IsNullOrEmpty(fieldName) || !this.FieldNameExists(fieldName))
            {
                item = null;
            }
            else
            {
                item = this.m_fieldsByName[fieldName];
            }
            return item;
        }

        public SPField GetFieldByNames(string sDisplayName, string sName)
        {
            SPField item;
            if (!string.IsNullOrEmpty(sName))
            {
                if (this.m_fieldsByName.ContainsKey(sName))
                {
                    item = this.m_fieldsByName[sName];
                    return item;
                }
            }
            if (!string.IsNullOrEmpty(sDisplayName))
            {
                foreach (SPField sPField in this)
                {
                    if ((sPField.DisplayName == null || !(sPField.DisplayName == sDisplayName) || !(sPField.Type != "Computed") ? false : !sPField.FromBaseType))
                    {
                        item = sPField;
                        return item;
                    }
                }
            }
            item = null;
            return item;
        }

        public SPFieldCollection GetFieldsByIdOrName(IEnumerable<SPField> fields)
        {
            return this.GetFieldsByIdOrName((new List<SPField>(fields)).ToArray());
        }

        public SPFieldCollection GetFieldsByIdOrName(params SPField[] fields)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            try
            {
                xmlTextWriter.WriteStartElement("Fields");
                SPField[] sPFieldArray = fields;
                for (int i = 0; i < (int)sPFieldArray.Length; i++)
                {
                    SPField sPField = sPFieldArray[i];
                    SPField fieldById = this.GetFieldById(sPField.ID) ?? this.GetFieldByName(sPField.Name);
                    if (fieldById != null)
                    {
                        xmlTextWriter.WriteRaw(fieldById.FieldXML.OuterXml);
                    }
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
            string str = stringWriter.ToString();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            return (this.m_parentList != null ? new SPFieldCollection(this.m_parentList, xmlDocument.FirstChild) : new SPFieldCollection(this.m_parentWeb, xmlDocument.FirstChild));
        }

        public SPFieldCollection GetFieldsByIdOrName(string sFieldIds, string sFieldNames)
        {
            return this.GetFieldsByIdOrNames(sFieldIds, sFieldNames, null);
        }

        // Metalogix.SharePoint.SPFieldCollection
        public SPFieldCollection GetFieldsByIdOrNames(string sFieldIds, string sFieldNames, string sFieldDisplayNames)
        {
            Dictionary<Guid, SPField> dictionary = new Dictionary<Guid, SPField>();
            if (!string.IsNullOrEmpty(sFieldIds))
            {
                string[] array = sFieldIds.Split(new char[]
                {
                    ','
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    string g = array[i];
                    Guid fieldId = new Guid(g);
                    SPField sPField = this.GetFieldById(fieldId);
                    if (sPField != null && !dictionary.ContainsKey(sPField.ID))
                    {
                        dictionary.Add(sPField.ID, sPField);
                    }
                }
            }
            if (!string.IsNullOrEmpty(sFieldNames))
            {
                string[] array = sFieldNames.Split(new char[]
                {
                    ','
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    string fieldName = array[i];
                    SPField sPField = this.GetFieldByName(fieldName);
                    if (sPField != null && !dictionary.ContainsKey(sPField.ID))
                    {
                        dictionary.Add(sPField.ID, sPField);
                    }
                }
            }
            if (!string.IsNullOrEmpty(sFieldDisplayNames))
            {
                string[] array = sFieldDisplayNames.Split(new char[]
                {
                    ','
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    string sDisplayName = array[i];
                    SPField sPField = this.GetFieldByNames(sDisplayName, null);
                    if (sPField != null && !dictionary.ContainsKey(sPField.ID))
                    {
                        dictionary.Add(sPField.ID, sPField);
                    }
                }
            }
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
            {
                xmlTextWriter.WriteStartElement("Fields");
                foreach (SPField sPField in dictionary.Values)
                {
                    xmlTextWriter.WriteRaw(sPField.FieldXML.OuterXml);
                }
                xmlTextWriter.WriteEndElement();
            }
            string xml = stringWriter.ToString();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            return (this.m_parentList != null) ? new SPFieldCollection(this.m_parentList, xmlDocument.FirstChild) : new SPFieldCollection(this.m_parentWeb, xmlDocument.FirstChild);
        }

        public SPFieldCollection GetFieldsOfType(string sType)
        {
            XmlNodeList xmlNodeLists = this.FieldsXML.SelectNodes(string.Concat("//Field[@Type='", sType, "']"));
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Fields");
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
            }
            xmlTextWriter.WriteEndElement();
            string str = stringWriter.ToString();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            return new SPFieldCollection(this.m_parentList, xmlDocument.FirstChild);
        }

        public SPFieldCollection GetFieldsOfTypes(string[] types)
        {
            SPFieldCollection sPFieldCollections;
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Fields");
            string[] strArrays = types;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                XmlNodeList xmlNodeLists = this.FieldsXML.SelectNodes(string.Concat("//Field[@Type='", str, "']"));
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                }
            }
            xmlTextWriter.WriteEndElement();
            string str1 = stringWriter.ToString();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str1);
            sPFieldCollections = (this.m_parentList == null ? new SPFieldCollection(this.m_parentWeb, xmlDocument.FirstChild) : new SPFieldCollection(this.m_parentList, xmlDocument.FirstChild));
            return sPFieldCollections;
        }

        public FieldsLookUp GetFieldsSchemaLookup()
        {
            return this.m_fieldsSchemaByName;
        }

        public XmlNode GetFilteredFieldXML(IFilterExpression siteFieldsFilter, bool excludePublishingInfrastructureSiteColumns)
        {
            XmlNode xmlNodes = this.FieldsXML.CloneNode(true);
            if (this.TaxonomyFieldsExist)
            {
                this.FilterManagedMetadataNoteFieldXml(xmlNodes);
            }
            if ((siteFieldsFilter != null ? true : excludePublishingInfrastructureSiteColumns))
            {
                this.FilterFieldXml(xmlNodes, siteFieldsFilter, excludePublishingInfrastructureSiteColumns);
            }
            return xmlNodes;
        }

        public string GetReferencedManagedMetadataForIdStyleMigration()
        {
            string referencedTaxonomyFullXml;
            SPFieldCollection taxonomyFields = this.GetTaxonomyFields();
            if (taxonomyFields.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings);
                try
                {
                    xmlWriter.WriteStartElement("TaxonomyFields");
                    foreach (SPField taxonomyField in taxonomyFields)
                    {
                        taxonomyField.SetReferencedManagedMetadata(xmlWriter, string.Empty);
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                }
                finally
                {
                    if (xmlWriter != null)
                    {
                        ((IDisposable)xmlWriter).Dispose();
                    }
                }
                referencedTaxonomyFullXml = this.ParentWeb.Adapter.Reader.GetReferencedTaxonomyFullXml(stringBuilder.ToString());
            }
            else
            {
                referencedTaxonomyFullXml = string.Empty;
            }
            return referencedTaxonomyFullXml;
        }

        public SPFieldCollection GetTaxonomyFields()
        {
            return this.GetFieldsOfTypes(new string[] { "TaxonomyFieldTypeMulti", "TaxonomyFieldType" });
        }

        public static void MapFieldXmlGuids(XmlNode fieldsNode, Dictionary<Guid, Guid> guidMappings, string targetRootWebGuid, string targetTaxonomyHiddenListGuid, CommonSerializableTable<string, string> termMappingDictionary, Guid parentGuid, TransformationRepository transformationRepository, bool resolveManagedMetadataByName, bool isDocumentLibrary, SPFieldCollection sourceFieldCollection)
        {
            Guid guid;
            XmlNodeList xmlNodeLists = fieldsNode.SelectNodes("//Field[@Type='TaxonomyFieldType' or @Type='TaxonomyFieldTypeMulti' or (@Type='LookupMulti' and (@Name='TaxCatchAllLabel' or @Name='TaxCatchAll'))]");
            string str = TransformationRepository.GenerateKey("RemovedNoteFields", parentGuid.ToString());
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//Property[Name='TextField']");
                if (xmlNodes1 != null)
                {
                    XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode("Value");
                    if (xmlNodes2 != null)
                    {
                        XmlNode xmlNodes3 = fieldsNode.SelectSingleNode(string.Format("//Field[@ID='{0}']", xmlNodes2.InnerText));
                        if (xmlNodes3 != null)
                        {
                            transformationRepository.Add(str, xmlNodes3.GetAttributeValueAsString("ID"), xmlNodes3.GetAttributeValueAsString("Name"));
                            xmlNodes3.ParentNode.RemoveChild(xmlNodes3);
                        }
                    }
                    xmlNodes1.ParentNode.RemoveChild(xmlNodes1);
                }
                if (string.Equals(xmlNodes.GetAttributeValueAsString("Name"), "TaxCatchAllLabel", StringComparison.OrdinalIgnoreCase))
                {
                    transformationRepository.Add(str, xmlNodes.GetAttributeValueAsString("ID"), xmlNodes.GetAttributeValueAsString("Name"));
                }
                if (string.Equals(xmlNodes.GetAttributeValueAsString("Name"), "TaxCatchAll", StringComparison.OrdinalIgnoreCase))
                {
                    transformationRepository.Add(str, xmlNodes.GetAttributeValueAsString("ID"), xmlNodes.GetAttributeValueAsString("Name"));
                }
                if ((!resolveManagedMetadataByName ? true : transformationRepository.Count() <= 1))
                {
                    SPFieldCollection.MapTaxonomyFieldsIdApproach(xmlNodes, termMappingDictionary, guidMappings);
                }
                else
                {
                    SPFieldCollection.MapTaxonomyFields(xmlNodes, transformationRepository);
                }
                XmlAttribute itemOf = xmlNodes.Attributes["List"];
                XmlAttribute xmlAttribute = xmlNodes.Attributes["WebId"];
                if (itemOf != null)
                {
                    itemOf.Value = targetTaxonomyHiddenListGuid;
                }
                if (xmlAttribute != null)
                {
                    xmlAttribute.Value = targetRootWebGuid;
                }
            }
            if (guidMappings != null)
            {
                foreach (XmlNode xmlNodes4 in fieldsNode.SelectNodes("//Field[@Type='Lookup' or @Type='LookupMulti' or @Type='CrossSiteLookup']"))
                {
                    SPFieldCollection.UpdateGuidMappedField(xmlNodes4, guidMappings, parentGuid, isDocumentLibrary);
                }
                XmlNode xmlNodes5 = (fieldsNode.Name == "Fields" ? fieldsNode : fieldsNode.SelectSingleNode("./Fields"));
                if (xmlNodes5 == null)
                {
                    xmlNodes5 = fieldsNode;
                }
                foreach (XmlNode xmlNodes6 in xmlNodes5.SelectNodes("./Field[@SourceID and not(starts-with(@SourceID, 'http://'))]"))
                {
                    SPFieldCollection.AddListColumnAttribute(xmlNodes6, sourceFieldCollection);
                    if (guidMappings.Count > 0)
                    {
                        XmlAttribute itemOf1 = xmlNodes6.Attributes["ID"];
                        if (itemOf1 != null)
                        {
                            guid = new Guid(itemOf1.Value);
                            if (guidMappings.ContainsKey(guid))
                            {
                                itemOf1.Value = string.Concat("{", guidMappings[guid], "}");
                            }
                        }
                    }
                    if (parentGuid != Guid.Empty)
                    {
                        XmlAttribute xmlAttribute1 = xmlNodes6.Attributes["SourceID"];
                        if ((xmlAttribute1 == null ? false : Utils.IsGuid(xmlAttribute1.Value)))
                        {
                            guid = new Guid(xmlAttribute1.Value);
                            if (guid == parentGuid)
                            {
                                xmlNodes6.Attributes.Remove(xmlAttribute1);
                            }
                            else if (guidMappings.ContainsKey(guid))
                            {
                                xmlAttribute1.Value = string.Concat("{", guidMappings[guid], "}");
                            }
                        }
                    }
                }
            }
        }

        private static void MapTaxonomyDefaultValue(XmlNode taxonomyFieldNode, string sourceTermSetId, bool isGuidMappingUsed, TransformationRepository transformationRepository, IDictionary<Guid, Guid> guidMappings = null)
        {
            XmlNode xmlNodes = taxonomyFieldNode.SelectSingleNode("Default");
            if ((xmlNodes == null ? false : xmlNodes.HasChildNodes))
            {
                string[] strArrays = xmlNodes.InnerText.Split(new char[] { '|' });
                string str = strArrays[1];
                string empty = string.Empty;
                if (!isGuidMappingUsed)
                {
                    empty = transformationRepository.GetValueForKey(sourceTermSetId, str);
                }
                else
                {
                    Guid guid = new Guid(str);
                    if ((guid == Guid.Empty ? false : guidMappings.ContainsKey(guid)))
                    {
                        empty = guidMappings[guid].ToString("D");
                    }
                }
                if (!string.IsNullOrEmpty(empty))
                {
                    xmlNodes.InnerText = string.Concat(strArrays[0], '|', empty);
                }
            }
        }

        public static void MapTaxonomyFields(XmlNode taxonomyFieldNode, TransformationRepository transformationRepository)
        {
            if (taxonomyFieldNode == null)
            {
                throw new ArgumentNullException("taxonomyFieldNode");
            }
            XmlNode xmlNodes = taxonomyFieldNode.SelectSingleNode(".//Property[Name='SspId']/Value");
            if (xmlNodes != null)
            {
                string valueForKey = transformationRepository.GetValueForKey("$TERMSTORE$", xmlNodes.InnerText.ToLower());
                if (!string.IsNullOrEmpty(valueForKey))
                {
                    transformationRepository.Add("$TSPKR$", valueForKey, xmlNodes.InnerText.ToLower());
                    xmlNodes.InnerText = valueForKey;
                }
            }
            XmlNode xmlNodes1 = taxonomyFieldNode.SelectSingleNode(".//Property[Name='TermSetId']/Value");
            if (xmlNodes1 != null)
            {
                string lower = xmlNodes1.InnerText.ToLower();
                string str = transformationRepository.GetValueForKey(lower, lower);
                if (!string.IsNullOrEmpty(str))
                {
                    transformationRepository.Add("$TSPKR$", str, lower);
                    xmlNodes1.InnerText = str;
                }
                XmlNode xmlNodes2 = taxonomyFieldNode.SelectSingleNode(".//Property[Name='AnchorId']/Value");
                if (xmlNodes2 != null)
                {
                    string lower1 = xmlNodes2.InnerText.ToLower();
                    string valueForKey1 = transformationRepository.GetValueForKey(lower, lower1);
                    if (!string.IsNullOrEmpty(valueForKey1))
                    {
                        xmlNodes2.InnerText = valueForKey1;
                    }
                }
                SPFieldCollection.MapTaxonomyDefaultValue(taxonomyFieldNode, lower, false, transformationRepository, null);
            }
        }

        public static void MapTaxonomyFieldsIdApproach(XmlNode taxonomyFieldNode, CommonSerializableTable<string, string> termMappingDictionary, IDictionary<Guid, Guid> guidMappings)
        {
            Guid item;
            if (taxonomyFieldNode == null)
            {
                throw new ArgumentNullException("taxonomyFieldNode");
            }
            bool flag = false;
            if ((termMappingDictionary == null ? false : termMappingDictionary.Count > 0))
            {
                XmlNode xmlNodes = taxonomyFieldNode.SelectSingleNode(".//Property[Name='SspId']/Value");
                if ((xmlNodes == null ? false : termMappingDictionary.ContainsKey(xmlNodes.InnerText)))
                {
                    string str = termMappingDictionary[xmlNodes.InnerText];
                    if (str != Guid.Empty.ToString())
                    {
                        xmlNodes.InnerText = str;
                        flag = true;
                    }
                }
            }
            if ((guidMappings == null ? false : guidMappings.Count != 0))
            {
                if (!flag)
                {
                    XmlNode str1 = taxonomyFieldNode.SelectSingleNode(".//Property[Name='SspId']/Value");
                    if (str1 != null)
                    {
                        Guid guid = new Guid(str1.InnerText);
                        if ((guid == Guid.Empty ? false : guidMappings.ContainsKey(guid)))
                        {
                            item = guidMappings[guid];
                            str1.InnerText = item.ToString("D");
                        }
                    }
                }
                XmlNode xmlNodes1 = taxonomyFieldNode.SelectSingleNode(".//Property[Name='TermSetId']/Value");
                if (xmlNodes1 != null)
                {
                    Guid guid1 = new Guid(xmlNodes1.InnerText);
                    if ((guid1 == Guid.Empty ? false : guidMappings.ContainsKey(guid1)))
                    {
                        item = guidMappings[guid1];
                        xmlNodes1.InnerText = item.ToString("D");
                    }
                }
                XmlNode str2 = taxonomyFieldNode.SelectSingleNode(".//Property[Name='AnchorId']/Value");
                if (str2 != null)
                {
                    Guid guid2 = new Guid(str2.InnerText);
                    if ((guid2 == Guid.Empty ? false : guidMappings.ContainsKey(guid2)))
                    {
                        item = guidMappings[guid2];
                        str2.InnerText = item.ToString("D");
                    }
                }
                SPFieldCollection.MapTaxonomyDefaultValue(taxonomyFieldNode, string.Empty, true, null, guidMappings);
            }
        }

        private void NotifyFieldCollectionChanged(XmlNode changedfieldSchemaXml)
        {
            if (this.FieldCollectionChanged != null)
            {
                this.FieldCollectionChanged(this, changedfieldSchemaXml);
            }
        }

        internal void ResetFields(XmlNode fieldsXML)
        {
            this.m_fieldsXML = fieldsXML;
            this.ResetFields();
        }

        private void ResetFields()
        {
            this.m_fieldsById.Clear();
            this.m_fieldsByName.Clear();
            this.m_fieldsSchemaByName.Clear();
            if (this.m_fieldsXML != null)
            {
                XmlNode xmlNodes = this.m_fieldsXML.SelectSingleNode("./Field[@Name='FileLeafRef']");
                if (xmlNodes != null)
                {
                    SPField sPField = new SPField(xmlNodes);
                    if (sPField.ID != Guid.Empty)
                    {
                        this.m_fieldsById.Add(sPField.ID, sPField);
                    }
                    this.m_fieldsByName.Add(sPField.Name, sPField);
                    this.m_fieldsSchemaByName.Add(sPField.Name, xmlNodes.OuterXml);
                }
                foreach (XmlNode xmlNodes1 in this.m_fieldsXML.SelectNodes("./Field"))
                {
                    SPField sPField1 = new SPField(xmlNodes1);
                    if (!(sPField1.Name == "FileLeafRef"))
                    {
                        if ((sPField1.ID == Guid.Empty ? false : !this.m_fieldsById.ContainsKey(sPField1.ID)))
                        {
                            this.m_fieldsById.Add(sPField1.ID, sPField1);
                        }
                        if (!this.m_fieldsByName.ContainsKey(sPField1.Name))
                        {
                            this.m_fieldsByName.Add(sPField1.Name, sPField1);
                            this.m_fieldsSchemaByName.Add(sPField1.Name, sPField1.FieldXML.OuterXml);
                        }
                    }
                }
            }
        }

        IEnumerator<SPField> System.Collections.Generic.IEnumerable<Metalogix.SharePoint.SPField>.GetEnumerator()
        {
            return this.m_fieldsByName.Values.GetEnumerator();
        }

        public SPField[] ToArray()
        {
            SPField[] sPFieldArray = new SPField[this.m_fieldsByName.Count];
            this.m_fieldsByName.Values.CopyTo(sPFieldArray, 0);
            return sPFieldArray;
        }

        internal static void UpdateFieldsXML(XmlNode existingFieldsNode, XmlNode newFieldsNode)
        {
            string str = "./Field[@Name='{0}']";
            foreach (XmlNode xmlNodes in newFieldsNode.SelectNodes("//Field"))
            {
                XmlNode xmlNodes1 = XmlUtility.CloneXMLNodeForTarget(xmlNodes, existingFieldsNode, true);
                XmlNode xmlNodes2 = existingFieldsNode.SelectSingleNode(string.Format(str, xmlNodes.Attributes["Name"]));
                if (xmlNodes2 == null)
                {
                    existingFieldsNode.AppendChild(xmlNodes1);
                }
                else
                {
                    existingFieldsNode.InsertAfter(xmlNodes1, xmlNodes2);
                    existingFieldsNode.RemoveChild(xmlNodes2);
                }
            }
        }

        private static void UpdateGuidMappedField(XmlNode lookupField, Dictionary<Guid, Guid> guidMappings, Guid parentGuid, bool isDocumentLibrary)
        {
            Guid guid;
            Guid item;
            XmlAttribute itemOf = lookupField.Attributes["List"];
            XmlAttribute str = lookupField.Attributes["WebId"];
            bool flag = false;
            if (itemOf != null)
            {
                string value = itemOf.Value;
                try
                {
                    guid = new Guid(value);
                    if (!guidMappings.ContainsKey(guid))
                    {
                        XmlAttribute xmlAttribute = lookupField.Attributes["SourceID"];
                        if ((xmlAttribute == null ? false : Utils.IsGuid(xmlAttribute.Value)))
                        {
                            if (!(new Guid(xmlAttribute.Value) == parentGuid))
                            {
                                XmlAttribute itemOf1 = lookupField.Attributes["ID"];
                                if ((itemOf1 == null || !Utils.IsGuid(itemOf1.Value) ? false : !guidMappings.ContainsKey(new Guid(itemOf1.Value))))
                                {
                                    lookupField.ParentNode.RemoveChild(lookupField);
                                    return;
                                }
                            }
                            else
                            {
                                if (isDocumentLibrary)
                                {
                                    lookupField.ParentNode.RemoveChild(lookupField);
                                }
                                return;
                            }
                        }
                    }
                    else
                    {
                        item = guidMappings[guid];
                        itemOf.Value = string.Concat("{", item.ToString(), "}");
                        flag = true;
                    }
                }
                catch
                {
                }
                if (str != null)
                {
                    value = str.Value;
                    try
                    {
                        guid = new Guid(value);
                        if (guidMappings.ContainsKey(guid))
                        {
                            item = guidMappings[guid];
                            str.Value = item.ToString();
                            flag = true;
                        }
                        else if (flag)
                        {
                            lookupField.Attributes.Remove(str);
                        }
                    }
                    catch
                    {
                    }
                }
                if (flag)
                {
                    lookupField.Attributes.Remove(lookupField.Attributes["TargetListName"]);
                    lookupField.Attributes.Remove(lookupField.Attributes["TargetWebName"]);
                    lookupField.Attributes.Remove(lookupField.Attributes["TargetWebSRURL"]);
                }
            }
        }

        internal void UpdateInternalFieldSchema(string fieldSchemaXml)
        {
            if (!string.IsNullOrEmpty(fieldSchemaXml))
            {
                SPFieldCollection.UpdateFieldsXML(this.m_fieldsXML, XmlUtility.StringToXmlNode(fieldSchemaXml));
                this.ResetFields();
            }
        }

        private void UpdateListFieldsVirtually(XmlNode fieldsNode)
        {
            if (this.m_parentList != null)
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(this.m_parentList.XML);
                SPFieldCollection.UpdateFieldsXML(xmlNode.SelectSingleNode("./Fields"), fieldsNode);
                this.m_parentList.UpdateList(xmlNode.OuterXml, true, true);
            }
        }

        private void UpdateSiteColumnsVirtually(XmlNode fieldsNode)
        {
            if (this.m_parentList == null)
            {
                if (this.m_parentWeb.WriteVirtually)
                {
                    Record virtualRecord = this.m_parentWeb.GetVirtualRecord();
                    this.m_fieldsXML = SPFieldCollection.UpdateSiteColumnsVirtually(this.m_fieldsXML, fieldsNode, virtualRecord);
                    this.ResetFields();
                }
            }
        }

        private static XmlNode UpdateSiteColumnsVirtually(XmlNode originalFieldsNode, XmlNode newFieldsNode, Record virtualRecord)
        {
            SPFieldCollection.UpdateFieldsXML(originalFieldsNode, newFieldsNode);
            PropertyDescriptor propertyDescriptor = virtualRecord.ParentWorkspace.EnsureProperty("Columns", typeof(TextMoniker), "EditScripts");
            string outerXml = null;
            TextMoniker value = propertyDescriptor.GetValue(virtualRecord) as TextMoniker;
            string fullText = value.GetFullText();
            if (string.IsNullOrEmpty(fullText))
            {
                outerXml = (!(newFieldsNode.Name == "FieldCollection") ? string.Format("<FieldCollection>{0}</FieldCollection>", newFieldsNode.OuterXml) : newFieldsNode.OuterXml);
            }
            else
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(fullText);
                SPFieldCollection.UpdateFieldsXML(xmlNode, newFieldsNode);
                outerXml = xmlNode.OuterXml;
            }
            value.SetFullText(outerXml);
            return XmlUtility.StringToXmlNode(outerXml);
        }

        public event SPFieldCollection.FieldCollectionChangeEventHandler FieldCollectionChanged;

        public delegate void FieldCollectionChangeEventHandler(SPFieldCollection sender, XmlNode fieldSchemaXml);
    }
}