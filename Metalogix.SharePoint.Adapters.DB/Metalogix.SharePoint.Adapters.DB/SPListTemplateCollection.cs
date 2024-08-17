using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.DB
{
    public class SPListTemplateCollection
    {
        private const string GLOBAL_TEMPLATE_NAME = "METALOGIXGLOBAL";

        private const string ReqForUserFieldId = "{1BEC4DEB-9524-496d-AF14-7547F0581CEF}";

        private const string ReqByUserFieldId = "{1BEC4AEB-9524-496d-AF14-75400B581CEF}";

        private const string AccessRequestListTemplateId = "160";

        private Dictionary<string, List<SPListTemplate>> m_data = new Dictionary<string, List<SPListTemplate>>();

        private SPListTemplateResourceManager m_templateManager;

        private readonly static Dictionary<string, string> FieldTypeSQLColumnMappings;

        public int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<string, List<SPListTemplate>> mDatum in this.m_data)
                {
                    count += mDatum.Value.Count;
                }

                return count;
            }
        }

        public SPListTemplateResourceManager TemplateResourceManager
        {
            get { return this.m_templateManager; }
        }

        static SPListTemplateCollection()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "AllDayEvent", "bit" },
                { "Attachments", "bit" },
                { "Boolean", "bit" },
                { "Calculated", "sql_variant" },
                { "Choice", "nvarchar" },
                { "ContentTypeId", "varbinary" },
                { "CrossProjectLink", "bit" },
                { "Currency", "float" },
                { "DateTime", "datetime" },
                { "File", "uniqueidentifier" },
                { "GridChoice", "ntext" },
                { "Guid", "uniqueidentifier" },
                { "Integer", "int" },
                { "Lookup", "int" },
                { "LookupMulti", "int" },
                { "ModStat", "int" },
                { "MultiChoice", "ntext" },
                { "MultiColumn", "ntext" },
                { "Note", "ntext" },
                { "Number", "float" },
                { "Recurrence", "bit" },
                { "Text", "nvarchar" },
                { "ThreadIndex", "varbinary" },
                { "Threading", "varchar" },
                { "URL", "nvarchar" },
                { "User", "int" },
                { "UserMulti", "int" },
                { "WorkflowEventType", "int" },
                { "WorkflowStatus", "nvarchar" }
            };
            SPListTemplateCollection.FieldTypeSQLColumnMappings = strs;
        }

        public SPListTemplateCollection(string sOnetXMLFileLocation)
        {
        }

        public SPListTemplateCollection(SPListTemplateResourceManager templateManager)
        {
            this.m_templateManager = templateManager;
            this.LoadOnetXMLFiles();
            if (this.TemplateResourceManager.SharePointVersion.IsSharePoint2007OrLater)
            {
                this.LoadFeatureTemplates();
            }
        }

        private void Add(SPListTemplate newTemplate, string sTemplateScope)
        {
            string upper = sTemplateScope.ToUpper();
            if (Utils.IsGUID(upper))
            {
                char[] chrArray = new char[] { '{', '}' };
                upper = upper.Trim(chrArray);
            }

            if (this.CheckWebTemplateForID(newTemplate.Type, upper) != null)
            {
                return;
            }

            if (newTemplate.BaseType == -1)
            {
                this.AddToDictionaryList(upper, newTemplate);
                return;
            }

            SPListTemplate sPListTemplate = this.FindByTemplateID(newTemplate.BaseType, upper) ??
                                            this.FindByTemplateID(newTemplate.BaseType, "METALOGIXGLOBAL");
            if (sPListTemplate != null)
            {
                XmlNode xmlNodes = sPListTemplate.FieldsXML.Clone();
                XmlNode fieldsXML = newTemplate.FieldsXML;
                newTemplate.FieldsXML = xmlNodes;
                foreach (XmlNode xmlNodes1 in fieldsXML.SelectNodes("//Field"))
                {
                    if (XmlUtility.MatchFirstAttributeValue("Name", xmlNodes1.Attributes["Name"].Value,
                            newTemplate.FieldsXML.SelectNodes("//Field")) != null)
                    {
                        continue;
                    }

                    XmlNode xmlNodes2 = newTemplate.FieldsXML.OwnerDocument.CreateElement("Field");
                    foreach (XmlAttribute attribute in xmlNodes1.Attributes)
                    {
                        XmlAttribute value = newTemplate.FieldsXML.OwnerDocument.CreateAttribute(attribute.Name);
                        value.Value = attribute.Value;
                        xmlNodes2.Attributes.Append(value);
                    }

                    newTemplate.FieldsXML.AppendChild(xmlNodes2);
                }

                foreach (XmlNode childNode in newTemplate.FieldsXML.ChildNodes)
                {
                    if (childNode.Attributes["ColName"] != null)
                    {
                        continue;
                    }

                    string str = childNode.Attributes["Type"].Value;
                    string sQLColumnType = SPListTemplateCollection.GetSQLColumnType(str);
                    if (sQLColumnType == null)
                    {
                        continue;
                    }

                    string sQLColumnName = SPListTemplateCollection.GetSQLColumnName(newTemplate.FieldsXML,
                        sQLColumnType, childNode, Convert.ToString(newTemplate.Type));
                    XmlAttribute xmlAttribute = childNode.OwnerDocument.CreateAttribute("ColName");
                    xmlAttribute.Value = sQLColumnName;
                    childNode.Attributes.Append(xmlAttribute);
                    if (str != "URL")
                    {
                        continue;
                    }

                    string sQLColumnName1 =
                        SPListTemplateCollection.GetSQLColumnName(newTemplate.FieldsXML, sQLColumnType, null, null);
                    XmlAttribute xmlAttribute1 = childNode.OwnerDocument.CreateAttribute("ColName2");
                    xmlAttribute1.Value = sQLColumnName1;
                    childNode.Attributes.Append(xmlAttribute1);
                }
            }

            if (newTemplate.BaseType != 1)
            {
                SPListTemplateCollection.AppendNonDocumentFields(newTemplate.FieldsXML);
            }
            else
            {
                SPListTemplateCollection.AppendDocumentFields(newTemplate.FieldsXML);
            }

            foreach (XmlNode value1 in newTemplate.ViewXML.SelectNodes("//FieldRef"))
            {
                string str1 = value1.Attributes["Name"].Value;
                str1 = XmlConvert.DecodeName(str1);
                XmlNode xmlNodes3 =
                    XmlUtility.MatchFirstAttributeValue("Name", str1, newTemplate.FieldsXML.SelectNodes("//Field"));
                if (xmlNodes3 != null)
                {
                    continue;
                }

                xmlNodes3 = XmlUtility.MatchFirstAttributeValue("DisplayName", str1,
                    newTemplate.FieldsXML.SelectNodes("//Field"));
                if (xmlNodes3 == null)
                {
                    continue;
                }

                value1.Attributes["Name"].Value = xmlNodes3.Attributes["Name"].Value;
            }

            this.AddToDictionaryList(upper, newTemplate);
        }

        private void AddToDictionaryList(string sKey, SPListTemplate value)
        {
            List<SPListTemplate> sPListTemplates;
            if (!this.m_data.ContainsKey(sKey))
            {
                sPListTemplates = new List<SPListTemplate>();
                this.m_data.Add(sKey, sPListTemplates);
            }
            else
            {
                sPListTemplates = this.m_data[sKey];
            }

            sPListTemplates.Add(value);
        }

        private static void AppendDocumentFields(XmlNode xFieldNode)
        {
            string[] strArrays = new string[] { "Type", "Text" };
            string[] strArrays1 = new string[] { "Name", "FileDirRef" };
            string[] strArrays2 = new string[] { "DisplayName", "Path" };
            string[] strArrays3 = new string[] { "MLSystem", "True" };
            string[][] strArrays4 = new string[][] { strArrays1, strArrays, strArrays2, strArrays3 };
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "FileLeafRef";
            strArrays2[1] = "Name";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "FileRef";
            strArrays2[1] = "URL Path";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "CheckedOutTitle";
            strArrays2[1] = "Checked Out To";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "UniqueId";
            strArrays[1] = "Counter";
            strArrays2[1] = "FileID";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            string[] strArrays5 = new string[] { "SrcField", "FileLeafRef" };
            strArrays4 = new string[][] { strArrays1, strArrays, strArrays5, strArrays3 };
            strArrays1[1] = "LinkFilenameNoMenu";
            strArrays[1] = "Computed";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "LinkFilename";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
            strArrays1[1] = "LinkCheckedOutTitle";
            strArrays2[1] = "Checked Out To";
            strArrays5[1] = "CheckedOutTitle";
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays4));
        }

        private static void AppendNonDocumentFields(XmlNode xFieldNode)
        {
            string[][] strArrays = new string[][]
            {
                new string[] { "Name", "LinkTitle" }, new string[] { "Type", "Computed" },
                new string[] { "SrcField", "Title" }, new string[] { "MLSystem", "True" }
            };
            string[][] strArrays1 = strArrays;
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays1));
            string[][] strArrays2 = new string[][]
            {
                new string[] { "Name", "LinkTitleNoMenu" }, new string[] { "Type", "Computed" },
                new string[] { "SrcField", "Title" }, new string[] { "MLSystem", "True" }
            };
            strArrays1 = strArrays2;
            xFieldNode.AppendChild(SPListTemplateCollection.BuildNode(xFieldNode, "Field", strArrays1));
        }

        private static XmlNode BuildNode(XmlNode parentNode, string sName, string[][] sAttributes)
        {
            XmlNode xmlNodes = parentNode.OwnerDocument.CreateElement(sName);
            string[][] strArrays = sAttributes;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string[] strArrays1 = strArrays[i];
                if ((int)strArrays1.Length == 2)
                {
                    XmlAttribute xmlAttribute = xmlNodes.OwnerDocument.CreateAttribute(strArrays1[0]);
                    xmlAttribute.Value = strArrays1[1];
                    xmlNodes.Attributes.Append(xmlAttribute);
                }
            }

            return xmlNodes;
        }

        private SPListTemplate CheckWebTemplateForID(int iTemplateID, string sTemplateScope)
        {
            if (!this.m_data.ContainsKey(sTemplateScope))
            {
                return null;
            }

            return this.m_data[sTemplateScope]
                .FirstOrDefault<SPListTemplate>((SPListTemplate listTemplate) => listTemplate.Type == iTemplateID);
        }

        public SPListTemplate FindByTemplateID(int iTemplateID)
        {
            return this.FindByTemplateID(iTemplateID, "METALOGIXGLOBAL");
        }

        public SPListTemplate FindByTemplateID(int iTemplateID, string sWebTemplate)
        {
            string upper = sWebTemplate.ToUpper();
            if (Utils.IsGUID(upper))
            {
                char[] chrArray = new char[] { '{', '}' };
                upper = upper.Trim(chrArray);
            }

            SPListTemplate sPListTemplate = this.CheckWebTemplateForID(iTemplateID, upper);
            if (sPListTemplate == null && sWebTemplate != "METALOGIXGLOBAL")
            {
                sPListTemplate = this.CheckWebTemplateForID(iTemplateID, "METALOGIXGLOBAL");
            }

            return sPListTemplate;
        }

        public List<SPListTemplate> GetAllTemplates()
        {
            List<SPListTemplate> sPListTemplates = new List<SPListTemplate>(this.Count);
            foreach (KeyValuePair<string, List<SPListTemplate>> mDatum in this.m_data)
            {
                sPListTemplates.AddRange(mDatum.Value);
            }

            return sPListTemplates;
        }

        private static string GetFieldsXMLFromListNode(XmlNode listNode)
        {
            string str;
            using (StringWriter stringWriter = new StringWriter(new StringBuilder(1024)))
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("Fields");
                    foreach (XmlNode xmlNodes in listNode.SelectNodes(
                                 "./*[name()='MetaData']/*[name()='Fields']/*[name()='Field']"))
                    {
                        if (xmlNodes.Attributes["Name"] == null || xmlNodes.Attributes["Type"] == null)
                        {
                            continue;
                        }

                        string value = xmlNodes.Attributes["Type"].Value;
                        if (xmlNodes.Attributes["ColName"] == null &&
                            (value == "Computed" || value == "Lookup" || value == "File"))
                        {
                            continue;
                        }

                        xmlTextWriter.WriteStartElement("Field");
                        xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Name"].Value);
                        xmlTextWriter.WriteAttributeString("Type", xmlNodes.Attributes["Type"].Value);
                        if (xmlNodes.Attributes["ID"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("ID", xmlNodes.Attributes["ID"].Value);
                        }

                        if (xmlNodes.Attributes["ColName"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("ColName", xmlNodes.Attributes["ColName"].Value);
                        }

                        if (xmlNodes.Attributes["Hidden"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Hidden", xmlNodes.Attributes["Hidden"].Value);
                        }

                        if (xmlNodes.Attributes["ReadOnly"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("ReadOnly", xmlNodes.Attributes["ReadOnly"].Value);
                        }

                        if (xmlNodes.Attributes["Required"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Required", xmlNodes.Attributes["Required"].Value);
                        }

                        if (xmlNodes.Attributes["Mult"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Mult", xmlNodes.Attributes["Mult"].Value);
                        }

                        if (xmlNodes.Attributes["List"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("List", xmlNodes.Attributes["List"].Value);
                        }

                        if (xmlNodes.Attributes["FieldRef"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("FieldRef",
                                xmlNodes.GetAttributeValueAsString("FieldRef"));
                        }

                        if (xmlNodes.Attributes["DisplayName"] == null || xmlNodes.Attributes["SourceID"] != null &&
                            !(xmlNodes.Attributes["SourceID"].Value != "http://schemas.microsoft.com/sharepoint/v3"))
                        {
                            string str1 = XmlConvert.DecodeName(xmlNodes.Attributes["Name"].Value);
                            if (str1 != xmlNodes.Attributes["Name"].Value)
                            {
                                xmlTextWriter.WriteAttributeString("DisplayName", str1);
                            }
                        }
                        else
                        {
                            xmlTextWriter.WriteAttributeString("DisplayName", xmlNodes.Attributes["DisplayName"].Value);
                        }

                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        private static string GetListTemplateXMLFromNode(XmlNode listTemplateNode)
        {
            string str;
            using (StringWriter stringWriter = new StringWriter(new StringBuilder(1024)))
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("ListTemplate");
                    xmlTextWriter.WriteAttributeString("Name", listTemplateNode.Attributes["Name"].Value);
                    xmlTextWriter.WriteAttributeString("Type", listTemplateNode.Attributes["Type"].Value);
                    xmlTextWriter.WriteAttributeString("BaseType", listTemplateNode.Attributes["BaseType"].Value);
                    xmlTextWriter.WriteEndElement();
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        private static string GetSQLColumnName(XmlNode fieldNodes, string sSQLColumnType, XmlNode fieldNode = null,
            string templateId = null)
        {
            int num = 0;
            if (fieldNode != null && !string.IsNullOrEmpty(templateId) && templateId.Equals("160"))
            {
                string attributeValueAsString = fieldNode.GetAttributeValueAsString("FieldRef");
                string str = fieldNode.GetAttributeValueAsString("ID");
                if (!string.IsNullOrEmpty(attributeValueAsString) && !string.IsNullOrEmpty(str) &&
                    (str.Equals("{1BEC4DEB-9524-496d-AF14-7547F0581CEF}",
                        StringComparison.InvariantCultureIgnoreCase) || str.Equals(
                        "{1BEC4AEB-9524-496d-AF14-75400B581CEF}", StringComparison.InvariantCultureIgnoreCase)))
                {
                    XmlNode xmlNodes =
                        fieldNodes.SelectSingleNode(string.Concat("//Field[@Name = '", attributeValueAsString, "' ]"));
                    return xmlNodes.GetAttributeValueAsString("ColName");
                }
            }

            XmlNodeList xmlNodeLists =
                fieldNodes.SelectNodes(string.Concat("//Field[contains(@ColName,'", sSQLColumnType, "')]"));
            foreach (XmlNode xmlNodes1 in xmlNodeLists)
            {
                string value = xmlNodes1.Attributes["ColName"].Value;
                string str1 = value.Substring(sSQLColumnType.Length);
                int num1 = Convert.ToInt32(str1);
                if (num1 <= num)
                {
                    continue;
                }

                num = num1;
            }

            xmlNodeLists = fieldNodes.SelectNodes(string.Concat("//Field[contains(@ColName2,'", sSQLColumnType, "')]"));
            foreach (XmlNode xmlNodes2 in xmlNodeLists)
            {
                string value1 = xmlNodes2.Attributes["ColName2"].Value;
                string str2 = value1.Substring(sSQLColumnType.Length);
                int num2 = Convert.ToInt32(str2);
                if (num2 <= num)
                {
                    continue;
                }

                num = num2;
            }

            int num3 = num + 1;
            return string.Concat(sSQLColumnType, num3.ToString());
        }

        private static string GetSQLColumnType(string sSPFieldType)
        {
            return (
                from entry in SPListTemplateCollection.FieldTypeSQLColumnMappings
                where entry.Key == sSPFieldType
                select entry.Value).FirstOrDefault<string>();
        }

        private static string GetViewXMLFromListNode(XmlNode listNode)
        {
            string str;
            using (StringWriter stringWriter = new StringWriter(new StringBuilder(1024)))
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("Views");
                    foreach (XmlNode xmlNodes in listNode.SelectNodes(
                                 "./*[name()='MetaData']/*[name()='Views']/*[name()='View']"))
                    {
                        XmlAttribute itemOf = xmlNodes.Attributes["DisplayName"];
                        string str1 = (itemOf != null ? itemOf.Value : "");
                        if (str1.Contains("$Resources:"))
                        {
                            str1 = Utils.CleanLocalizationReferences(str1).Replace('\u005F', ' ')
                                .TrimEnd(new char[] { ';' });
                        }

                        xmlTextWriter.WriteStartElement("View");
                        xmlTextWriter.WriteAttributeString("DisplayName", str1);
                        xmlTextWriter.WriteAttributeString("Type", xmlNodes.Attributes["Type"].Value);
                        if (xmlNodes.Attributes["Url"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Url", xmlNodes.Attributes["Url"].Value);
                        }

                        if (xmlNodes.Attributes["BaseViewID"] != null)
                        {
                            string value = xmlNodes.Attributes["BaseViewID"].Value;
                            xmlTextWriter.WriteAttributeString("BaseViewID", value);
                            if (value.Equals("0"))
                            {
                                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='ViewHeader']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }

                                xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='ViewBody']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }

                                xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='ViewFooter']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }

                                xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='Toolbar']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }

                                xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='RowLimitExceeded']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }

                                xmlNodes1 = xmlNodes.SelectSingleNode(".//*[name()='ViewEmpty']");
                                if (xmlNodes1 != null)
                                {
                                    xmlTextWriter.WriteRaw(
                                        SPListTemplateCollection.ResolveViewLocalizationReferences(xmlNodes1.OuterXml));
                                }
                            }
                        }

                        XmlNode xmlNodes2 = xmlNodes.SelectSingleNode(".//*[name()='ViewFields']");
                        if (xmlNodes2 != null)
                        {
                            xmlTextWriter.WriteRaw(xmlNodes2.OuterXml);
                        }

                        xmlNodes2 = xmlNodes.SelectSingleNode(".//*[name()='Query']");
                        if (xmlNodes2 != null)
                        {
                            xmlTextWriter.WriteRaw(xmlNodes2.OuterXml);
                        }

                        xmlNodes2 = xmlNodes.SelectSingleNode(".//*[name()='Aggregations']");
                        if (xmlNodes2 != null)
                        {
                            xmlTextWriter.WriteRaw(xmlNodes2.OuterXml);
                        }

                        xmlNodes2 = xmlNodes.SelectSingleNode(".//*[name()='RowLimit']");
                        if (xmlNodes2 != null)
                        {
                            xmlTextWriter.WriteRaw(xmlNodes2.OuterXml);
                        }

                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        private void LoadFeatureTemplates()
        {
            XmlNode documentElement;
            string value;
            string[] templateNames = this.TemplateResourceManager.GetTemplateNames("Features\\\\.*\\\\Feature.xml");
            for (int i = 0; i < (int)templateNames.Length; i++)
            {
                string str = templateNames[i];
                XmlDocument template = this.TemplateResourceManager.GetTemplate(str);
                if (template != null)
                {
                    string str1 = str.Substring(0, str.LastIndexOf("\\") + 1);
                    if (template.DocumentElement.Name == "Feature")
                    {
                        documentElement = template.DocumentElement;
                    }
                    else
                    {
                        documentElement =
                            template.DocumentElement.GetElementsByTagName("Feature",
                                "http://schemas.microsoft.com/sharepoint/")[0];
                    }

                    XmlNode xmlNodes = documentElement;
                    if (xmlNodes.Attributes["Id"] != null)
                    {
                        value = xmlNodes.Attributes["Id"].Value;
                    }
                    else
                    {
                        value = null;
                    }

                    string str2 = value;
                    foreach (XmlNode elementsByTagName in template.DocumentElement.GetElementsByTagName(
                                 "ElementManifest", "http://schemas.microsoft.com/sharepoint/"))
                    {
                        string value1 = elementsByTagName.Attributes["Location"].Value;
                        string str3 = string.Concat(str1, value1);
                        try
                        {
                            XmlDocument xmlDocument = this.TemplateResourceManager.GetTemplate(str3);
                            if (xmlDocument != null)
                            {
                                foreach (XmlNode xmlNodes1 in xmlDocument.SelectNodes("//*[name()='ListTemplate']"))
                                {
                                    string listTemplateXMLFromNode =
                                        SPListTemplateCollection.GetListTemplateXMLFromNode(xmlNodes1);
                                    string fieldsXMLFromListNode = "<Fields/>";
                                    string viewXMLFromListNode = "<Views />";
                                    string templateName = this.TemplateResourceManager.GetTemplateName(
                                        string.Concat("Features\\\\.*\\\\", xmlNodes1.Attributes["Name"].Value,
                                            "\\\\Schema.xml"), str1);
                                    XmlDocument template1 = this.TemplateResourceManager.GetTemplate(templateName);
                                    if (template1 == null)
                                    {
                                        continue;
                                    }

                                    XmlNode xmlNodes2 = template1.SelectSingleNode("//*[name()='List']");
                                    fieldsXMLFromListNode =
                                        SPListTemplateCollection.GetFieldsXMLFromListNode(xmlNodes2);
                                    viewXMLFromListNode = SPListTemplateCollection.GetViewXMLFromListNode(xmlNodes2);
                                    this.Add(
                                        new SPListTemplate(listTemplateXMLFromNode, fieldsXMLFromListNode,
                                            viewXMLFromListNode), str2);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }
            }
        }

        private void LoadOnetXMLFiles()
        {
            string upper;
            string[] templateNames = this.TemplateResourceManager.GetTemplateNames("ONET.XML");
            for (int i = 0; i < (int)templateNames.Length; i++)
            {
                string str = templateNames[i];
                if (!this.TemplateResourceManager.SPS2003DBMode)
                {
                    int num = str.IndexOf("SiteTemplates");
                    if (num < 0)
                    {
                        upper = "METALOGIXGLOBAL";
                    }
                    else
                    {
                        int num1 = str.IndexOf('\\', num) + 1;
                        int num2 = str.IndexOf('\\', num1);
                        upper = str.Substring(num1, num2 - num1).ToUpper();
                    }
                }
                else
                {
                    upper = str;
                    int num3 = upper.IndexOf('\\');
                    if (num3 >= 0)
                    {
                        upper = upper.Substring(0, num3).ToUpper();
                    }
                }

                XmlDocument template = this.TemplateResourceManager.GetTemplate(str);
                if (template != null)
                {
                    try
                    {
                        foreach (XmlNode xmlNodes in template.SelectNodes("//BaseTypes/BaseType"))
                        {
                            using (StringWriter stringWriter = new StringWriter(new StringBuilder(1024)))
                            {
                                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                                {
                                    xmlTextWriter.WriteStartElement("ListTemplate");
                                    xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Title"].Value);
                                    xmlTextWriter.WriteAttributeString("Type", xmlNodes.Attributes["Type"].Value);
                                    xmlTextWriter.WriteEndElement();
                                    string fieldsXMLFromListNode =
                                        SPListTemplateCollection.GetFieldsXMLFromListNode(xmlNodes);
                                    string viewXMLFromListNode =
                                        SPListTemplateCollection.GetViewXMLFromListNode(xmlNodes);
                                    SPListTemplate sPListTemplate = new SPListTemplate(stringWriter.ToString(),
                                        fieldsXMLFromListNode, viewXMLFromListNode);
                                    this.Add(sPListTemplate, upper);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(string.Concat("Could not load base list types from the Onet.XML file: ",
                            exception.Message));
                    }
                }

                foreach (XmlNode xmlNodes1 in template.SelectNodes("//ListTemplates/ListTemplate"))
                {
                    try
                    {
                        string listTemplateXMLFromNode = SPListTemplateCollection.GetListTemplateXMLFromNode(xmlNodes1);
                        string upper1 = upper;
                        if (xmlNodes1.Attributes["Path"] != null)
                        {
                            upper1 = xmlNodes1.Attributes["Path"].Value.ToUpper();
                        }

                        string templateName = null;
                        if (this.TemplateResourceManager.SPS2003DBMode)
                        {
                            string[] strArrays = this.TemplateResourceManager.GetTemplateNames(
                                string.Concat("\\\\Lists\\\\", xmlNodes1.Attributes["Name"].Value, "\\\\Schema.xml"));
                            int num4 = 0;
                            while (num4 < (int)strArrays.Length)
                            {
                                string str1 = strArrays[num4];
                                if (!str1.StartsWith(upper1))
                                {
                                    if (str1.StartsWith("METALOGIXGLOBAL"))
                                    {
                                        templateName = str1;
                                    }

                                    num4++;
                                }
                                else
                                {
                                    templateName = str1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            templateName = this.TemplateResourceManager.GetTemplateName(
                                string.Concat("\\\\Lists\\\\", xmlNodes1.Attributes["Name"].Value, "\\\\Schema.xml"),
                                string.Format("SiteTemplates\\{0}\\", upper1));
                        }

                        if (!string.IsNullOrEmpty(templateName))
                        {
                            XmlDocument xmlDocument = this.TemplateResourceManager.GetTemplate(templateName);
                            if (xmlDocument != null)
                            {
                                XmlNode xmlNodes2 = xmlDocument.SelectSingleNode("//List");
                                string fieldsXMLFromListNode1 =
                                    SPListTemplateCollection.GetFieldsXMLFromListNode(xmlNodes2);
                                string viewXMLFromListNode1 =
                                    SPListTemplateCollection.GetViewXMLFromListNode(xmlNodes2);
                                this.Add(
                                    new SPListTemplate(listTemplateXMLFromNode, fieldsXMLFromListNode1,
                                        viewXMLFromListNode1), upper);
                            }
                        }
                    }
                    catch (Exception exception1)
                    {
                    }
                }
            }
        }

        private static ResourceManager LoadResource(string sResourceFile)
        {
            ResourceManager resourceManager;
            ResourceManager resourceManager1 = null;
            try
            {
                if (!string.IsNullOrEmpty(sResourceFile))
                {
                    resourceManager1 =
                        new ResourceManager(string.Concat("Metalogix.SharePoint.Adapters.DB.Resources.", sResourceFile),
                            Assembly.GetExecutingAssembly());
                }

                return resourceManager1;
            }
            catch
            {
                resourceManager = null;
            }

            return resourceManager;
        }

        internal static string ResolveViewLocalizationReferences(string sSourceWithLocalRefs)
        {
            string str;
            string str1 = sSourceWithLocalRefs;
            MatchCollection matchCollections = Regex.Matches(sSourceWithLocalRefs,
                "\\$Resources(NoEncode)?:\\s?((?<ResourceFile>\\S+?)\\s?,\\s?)?(?<ResourceName>\\S+?)[<;]");
            if (matchCollections.Count <= 0)
            {
                return sSourceWithLocalRefs;
            }

            try
            {
                ResourceManager resourceManager = SPListTemplateCollection.LoadResource("core");
                if (resourceManager != null)
                {
                    ResourceManager item = null;
                    Dictionary<string, ResourceManager> strs = new Dictionary<string, ResourceManager>();
                    foreach (Match match in matchCollections)
                    {
                        try
                        {
                            if (match.Groups["ResourceFile"] == null ||
                                string.IsNullOrEmpty(match.Groups["ResourceFile"].Value) || match.Groups["ResourceFile"]
                                    .Value.Equals("core", StringComparison.OrdinalIgnoreCase))
                            {
                                item = resourceManager;
                            }
                            else
                            {
                                string value = match.Groups["ResourceFile"].Value;
                                if (!strs.ContainsKey(value))
                                {
                                    item = SPListTemplateCollection.LoadResource(value);
                                    if (item == null)
                                    {
                                        item = resourceManager;
                                    }
                                    else
                                    {
                                        strs.Add(value, item);
                                    }
                                }
                                else
                                {
                                    item = strs[value];
                                }
                            }

                            string value1 = match.Groups["ResourceName"].Value;
                            char[] chrArray = new char[] { ',' };
                            string str2 = item.GetString(value1.Trim(chrArray));
                            str1 = str1.Replace(match.Value,
                                string.Concat(str2, (match.Value.EndsWith("<") ? "<" : "")));
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }

                return str1;
            }
            catch (Exception exception1)
            {
                str = sSourceWithLocalRefs;
            }

            return str;
        }
    }
}