using Metalogix.Permissions;
using Metalogix.Utilities;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public static class Utils
    {
        public const string VIDEO_CONTENT_TYPE_ID_2010 =
            "0x0101009148F5A04DDD49CBA7127AADA5FB792B00291D173ECE694D56B19D111489C4369D";

        public const string VIDEO_CONTENT_TYPE_ID_2013 = "0x0120D520A808";

        public const string ASSET_LIBRARY_LIST_TEMPLATE_ID = "851";

        public const string PublishingInfrastructureFeatureNotActivated =
            "The SharePoint Server Publishing Infrastructure feature must be activated at the site collection level before the Publishing feature can be activated.";

        public const string OneNoteBook = "OneNote.Notebook";

        public const string DocumentSetContentTypeID = "0x0120d520";

        public const string DocumentSet = "SharePoint.DocumentSet";

        private const string MetalogixInternalDateFormat = "o";

        private static bool? m_supportsTaxonomy;

        private static string illegalCharactersRegex;

        public static string IllegalCharactersForSiteUrl;

        public static string illegalCharactersForHostHeaderSiteUrl;

        public static string illegalCharactersForHostHeaderSiteUrlRegEx;

        private static System.Collections.ObjectModel.ReadOnlyCollection<char> m_IllegalCharacters;

        private static CultureInfo s_EnglishCulture;

        private readonly static List<Guid> FieldsToIgnoreForManifest;

        private static string _folderLocation;

        private static List<Guid> _languageNaturalWorkflowTemplateIds;

        private static List<string> _languageSpecificWorkflowTemplateIds;

        public static System.Collections.ObjectModel.ReadOnlyCollection<char> IllegalCharacters
        {
            get { return Utils.m_IllegalCharacters; }
        }

        public static Architecture ProcessorArchitectureNumber
        {
            get
            {
                SYSTEM_INFO sYSTEMINFO = new SYSTEM_INFO();
                Metalogix.SharePoint.Adapters.NativeMethods.GetSystemInfo(out sYSTEMINFO);
                return sYSTEMINFO.ProcessorInformation.ProcessorArchitecture;
            }
        }

        public static bool SystemIs64Bit
        {
            get
            {
                SYSTEM_INFO sYSTEMINFO = new SYSTEM_INFO();
                Metalogix.SharePoint.Adapters.NativeMethods.GetSystemInfo(out sYSTEMINFO);
                if (sYSTEMINFO.ProcessorInformation.ProcessorArchitecture !=
                    Architecture.PROCESSOR_ARCHITECTURE_AMD64 &&
                    sYSTEMINFO.ProcessorInformation.ProcessorArchitecture != Architecture.PROCESSOR_ARCHITECTURE_IA64)
                {
                    return false;
                }

                return true;
            }
        }

        static Utils()
        {
            Utils.m_supportsTaxonomy = null;
            Utils.illegalCharactersRegex = "[\\\\\"~#%&*{}/:<>?|+]";
            Utils.IllegalCharactersForSiteUrl = "\\\"~#%&*{}/:<>?|+";
            Utils.illegalCharactersForHostHeaderSiteUrl = "\\\"~!@#$%^&*()`,_<>|?;\\][:'{}+space";
            Utils.illegalCharactersForHostHeaderSiteUrlRegEx = "[\\\\\"~!@#$%^&*()`,_<>|\\s?;\\][:'{}+]";
            Utils.m_IllegalCharacters = new System.Collections.ObjectModel.ReadOnlyCollection<char>(new char[]
                { '~', '#', '%', '&', '*', '{', '}', '\\', ':', '<', '>', '?', '/', '+', '|', '\"' });
            Utils.s_EnglishCulture = new CultureInfo("en-US", false);
            List<Guid> guids = new List<Guid>()
            {
                new Guid("8c06beca-0777-48f7-91c7-6da68bc07b69"),
                new Guid("1df5e554-ec7e-46a6-901d-d85a3881cb18"),
                new Guid("28cf69c5-fa48-462a-b5cd-27b6f9d2bd5f"),
                new Guid("d31655d1-1d5b-4511-95a1-7a09e9b75bf2"),
                new Guid("b824e17e-a1b3-426e-aecf-f0184d900485"),
                new Guid("960ff01f-2b6d-4f1b-9c3f-e19ad8927341"),
                new Guid("6bfaba20-36bf-44b5-a1b2-eb6346d49716"),
                new Guid("e08400f3-c779-4ed2-a18c-ab7f34caa318"),
                new Guid("503f1caa-358e-4918-9094-4a2cdc4bc034"),
                new Guid("dce8262a-3ae9-45aa-aab4-83bd75fb738a"),
                new Guid("bc1a8efb-0f4c-49f8-a38f-7fe22af3d3e0"),
                new Guid("774eab3a-855f-4a34-99da-69dc21043bec"),
                new Guid("6b4e226d-3d88-4a36-808d-a129bf52bccf"),
                new Guid("3881510a-4e4a-4ee8-b102-8ee8e2d0dd4b"),
                new Guid("58014f77-5463-437b-ab67-eec79532da67"),
                new Guid("822c78e3-1ea9-4943-b449-57863ad33ca9"),
                new Guid("50a54da4-1528-4e67-954a-e2d24f1e9efb"),
                new Guid("03e45e84-1992-4d42-9116-26f756012634"),
                new Guid("c042a256-787d-4a6f-8a8a-cf6ab767f12d"),
                new Guid("f3b0adf9-c1a2-4b02-920d-943fba4b3611"),
                new Guid("8f6b6dd8-9357-4019-8172-966fcd502ed2")
            };
            Utils.FieldsToIgnoreForManifest = guids;
            List<Guid> guids1 = new List<Guid>()
            {
                new Guid("c6964bff-bf8d-41ac-ad5e-b61ec111731c"),
                new Guid("46c389a4-6e18-476c-aa17-289b0c79fb8f"),
                new Guid("2f213931-3b93-4f81-b021-3022434a3114"),
                new Guid("dd19a800-37c1-43c0-816d-f8eb5f4a4145"),
                new Guid("c6964bff-bf8d-41ac-ad5e-b61ec111731a"),
                new Guid("b4154df4-cc53-4c4f-adef-1ecf0b7417f6")
            };
            Utils._languageNaturalWorkflowTemplateIds = guids1;
            Utils._languageSpecificWorkflowTemplateIds = new List<string>()
            {
                "e43856d2-1bb4-40ef-b08b-016d89a",
                "8ad4d8f0-93a7-4941-9657-cf3706f",
                "3bfb07cb-5c6a-4266-849b-8d67117",
                "77c71f43-f403-484b-bcb2-303710e"
            };
        }

        public static void AddOrUpdateXmlAttribute(XmlNode ndInputNode, string sAttributeName, string sAttributeValue)
        {
            if (ndInputNode.Attributes[sAttributeName] != null)
            {
                ndInputNode.Attributes[sAttributeName].Value = sAttributeValue;
                return;
            }

            XmlAttribute xmlAttribute = ndInputNode.OwnerDocument.CreateAttribute(sAttributeName);
            xmlAttribute.Value = sAttributeValue;
            ndInputNode.Attributes.Append(xmlAttribute);
        }

        public static string BuildPagesLibraryGuidFetchingQuery(Guid[] idsToFetch)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Query");
            xmlTextWriter.WriteStartElement("Where");
            if (idsToFetch == null || (int)idsToFetch.Length == 0)
            {
                xmlTextWriter.WriteStartElement("Gt");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "ID");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Counter");
                xmlTextWriter.WriteValue("0");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }
            else
            {
                int num = 0;
                for (int i = 0; i < (int)idsToFetch.Length; i++)
                {
                    if (i < (int)idsToFetch.Length - 1)
                    {
                        xmlTextWriter.WriteStartElement("Or");
                        num++;
                    }

                    xmlTextWriter.WriteStartElement("Eq");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "UniqueId");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Guid");
                    xmlTextWriter.WriteString(idsToFetch[i].ToString());
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }

                for (int j = 0; j < num; j++)
                {
                    xmlTextWriter.WriteEndElement();
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("OrderBy");
            xmlTextWriter.WriteAttributeString("Override", "TRUE");
            xmlTextWriter.WriteStartElement("FieldRef");
            xmlTextWriter.WriteAttributeString("Name", "FileDirRef");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("FieldRef");
            xmlTextWriter.WriteAttributeString("Name", "FileLeafRef");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public static string BuildQuery(string sIDs, string sParentFolder, bool bRecursive, bool bIs2003IssuesList,
            ListItemQueryType itemTypes, string[] sOrderByFields)
        {
            if (sParentFolder != null)
            {
                sParentFolder = sParentFolder.TrimStart(new char[] { '/' });
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            bool flag = (sIDs == null ? false : sIDs.Length > 0);
            bool flag1 = (sParentFolder == null ? false : sParentFolder.Length > 0);
            bool flag2 = itemTypes != (ListItemQueryType.ListItem | ListItemQueryType.Folder);
            xmlTextWriter.WriteStartElement("Where");
            if (flag1)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (flag2)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (bIs2003IssuesList)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (!flag)
            {
                xmlTextWriter.WriteStartElement("Gt");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "ID");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Counter");
                xmlTextWriter.WriteValue("0");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }
            else
            {
                string[] strArrays = sIDs.Split(new char[] { ',' });
                int length = (int)strArrays.Length - 1;
                for (int i = 1; i < (int)strArrays.Length; i++)
                {
                    xmlTextWriter.WriteStartElement("Or");
                }

                bool flag3 = false;
                string[] strArrays1 = strArrays;
                for (int j = 0; j < (int)strArrays1.Length; j++)
                {
                    string str = strArrays1[j];
                    int num = str.IndexOf('#');
                    string str1 = (num >= 0 ? str.Substring(0, num) : str);
                    xmlTextWriter.WriteStartElement("Eq");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "ID");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Counter");
                    xmlTextWriter.WriteValue(str1);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                    if (flag3 && length > 0)
                    {
                        xmlTextWriter.WriteEndElement();
                        length--;
                    }

                    flag3 = true;
                }
            }

            if (bIs2003IssuesList)
            {
                xmlTextWriter.WriteStartElement("Neq");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "IsCurrent");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Boolean");
                xmlTextWriter.WriteValue("False");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }

            if (flag2)
            {
                string str2 = null;
                if (itemTypes == ListItemQueryType.Folder)
                {
                    str2 = "1";
                }
                else if (itemTypes == ListItemQueryType.ListItem)
                {
                    str2 = "0";
                }

                if (str2 != null)
                {
                    xmlTextWriter.WriteStartElement("Eq");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "FSObjType");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Counter");
                    xmlTextWriter.WriteValue(str2);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }
            }

            if (flag1)
            {
                if (bRecursive)
                {
                    xmlTextWriter.WriteStartElement("Or");
                    xmlTextWriter.WriteStartElement("BeginsWith");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "FileDirRef");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Text");
                    xmlTextWriter.WriteValue(string.Concat(sParentFolder, "/"));
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteStartElement("Eq");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "FileDirRef");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Text");
                xmlTextWriter.WriteValue(sParentFolder);
                xmlTextWriter.WriteEndElement();
                if (bRecursive)
                {
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            if (sOrderByFields == null || (int)sOrderByFields.Length < 1)
            {
                sOrderByFields = new string[] { "FileDirRef", "FileLeafRef" };
            }

            stringBuilder.Append("<OrderBy Override='TRUE'>");
            string[] strArrays2 = sOrderByFields;
            for (int k = 0; k < (int)strArrays2.Length; k++)
            {
                string str3 = strArrays2[k];
                stringBuilder.Append("<FieldRef Ascending = 'TRUE' Name = '");
                stringBuilder.Append(str3);
                stringBuilder.Append("' />");
            }

            stringBuilder.Append("</OrderBy>");
            return stringBuilder.ToString();
        }

        public static bool CheckAdditionalLookupField(XmlNode xmlNode)
        {
            string attributeValueAsString = xmlNode.GetAttributeValueAsString("Type");
            bool attributeValueAsBoolean = xmlNode.GetAttributeValueAsBoolean("ReadOnly");
            bool flag = xmlNode.GetAttributeValueAsBoolean("Hidden");
            string str = xmlNode.GetAttributeValueAsString("FieldRef");
            if (!attributeValueAsString.Equals("Lookup", StringComparison.InvariantCultureIgnoreCase) &&
                !attributeValueAsString.Equals("LookupMulti", StringComparison.InvariantCultureIgnoreCase) ||
                string.IsNullOrEmpty(str) || !attributeValueAsBoolean || flag)
            {
                return false;
            }

            return true;
        }

        public static void CheckUpdateListItemsResultForErrors(XmlNode result)
        {
            XmlNode xmlNodes = XmlUtility.RunXPathQuerySelectSingle(result, "//sp:ErrorText[text() != '0x00000000']");
            if (xmlNodes != null)
            {
                string str = string.Concat("Batch Failure. Code: ", xmlNodes.InnerText);
                XmlNode xmlNodes1 = XmlUtility.RunXPathQuerySelectSingle(result, "//sp:ErrorText");
                if (xmlNodes1 != null)
                {
                    str = string.Concat(str, " Error: ", xmlNodes1.InnerText);
                }

                throw new Exception(str);
            }
        }

        public static string CleanLocalizationReferences(string sSource)
        {
            return Regex.Replace(sSource, "\\$Resources(NoEncode)?:\\s?(\\S+\\s?,\\s?)?(?<VarName>\\S+);", "${VarName}",
                RegexOptions.IgnoreCase);
        }

        public static string CleanSharePointURL(string sURL)
        {
            string str = sURL;
            string str1 = "_";
            Regex regex = new Regex(Utils.illegalCharactersRegex);
            if (regex.IsMatch(str))
            {
                str = regex.Replace(str, str1);
            }

            if (str.StartsWith("_", StringComparison.Ordinal))
            {
                str = string.Concat("a", str);
            }

            return str;
        }

        public static string CombineUrls(string sPrefix, string sSuffix)
        {
            string str;
            string str1;
            string str2 = null;
            string str3 = null;
            string str4 = null;
            Utils.GetUrlPathOverlap(sPrefix, sSuffix, out str2, out str3, out str4);
            if (!string.IsNullOrEmpty(str2))
            {
                char[] chrArray = new char[] { '/' };
                str = string.Concat(str2.TrimEnd(chrArray), '/');
            }
            else
            {
                str = "";
            }

            if (!string.IsNullOrEmpty(str3))
            {
                char[] chrArray1 = new char[] { '/' };
                str1 = string.Concat(str3.Trim(chrArray1), '/');
            }
            else
            {
                str1 = "";
            }

            return string.Concat(str, str1, (!string.IsNullOrEmpty(str4) ? str4.TrimStart(new char[] { '/' }) : ""));
        }

        public static string ConvertClaimStringUserToWinOrFormsUser(string claimString)
        {
            if (string.IsNullOrEmpty(claimString) ||
                !claimString.StartsWith("i:0#.w|", StringComparison.InvariantCulture) &&
                !claimString.StartsWith("i:0#.f|", StringComparison.InvariantCulture))
            {
                return claimString;
            }

            int num = claimString.IndexOf('|');
            int num1 = claimString.LastIndexOf('|');
            string str = claimString.Substring(num1 + 1);
            str = str.Replace("%2C", ",");
            str = str.Replace("%3A", ":");
            str = str.Replace("%3B", ";");
            str = str.Replace("%0A", "\n");
            str = str.Replace("%0D", "\r");
            str = str.Replace("%7C", "|");
            str = str.Replace("%25", "%");
            if (num == num1)
            {
                return str;
            }

            string str1 = claimString.Substring(num + 1, num1 - (num + 1));
            return string.Format("{0}:{1}", str1, str);
        }

        public static string ConvertHtmlLinkToWikiLink(object wikiFieldValue)
        {
            string str;
            try
            {
                string str1 = Convert.ToString(wikiFieldValue);
                foreach (Match match in Regex.Matches(str1, "\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase))
                {
                    if (string.IsNullOrEmpty(match.Value))
                    {
                        continue;
                    }

                    string str2 = match.Value.Replace("[[", "\\[[");
                    str2 = str2.Replace("]]", "\\]]");
                    str1 = str1.Replace(match.Value, str2);
                }

                str = str1;
            }
            catch (Exception exception)
            {
                str = Convert.ToString(wikiFieldValue);
            }

            return str;
        }

        public static string ConvertWebPartIDToGuid(string sWebPartID)
        {
            if (string.IsNullOrEmpty(sWebPartID))
            {
                return null;
            }

            Match match = Regex.Match(sWebPartID,
                "(?<first>[a-zA-Z0-9]{8})(-|_)(?<second>[a-zA-Z0-9]{4})(-|_)(?<third>[a-zA-Z0-9]{4})(-|_)(?<fourth>[a-zA-Z0-9]{4})(-|_)(?<fifth>[a-zA-Z0-9]{12})");
            if (match == null || !match.Success)
            {
                return null;
            }

            string[] value = new string[]
            {
                match.Groups["first"].Value, "-", match.Groups["second"].Value, "-", match.Groups["third"].Value, "-",
                match.Groups["fourth"].Value, "-", match.Groups["fifth"].Value
            };
            return string.Concat(value);
        }

        public static string ConvertWinOrFormsUserToClaimString(string sLoginName)
        {
            string str;
            string str1;
            if (string.IsNullOrEmpty(sLoginName) || sLoginName.Contains("|"))
            {
                return sLoginName;
            }

            int num = sLoginName.IndexOf(':');
            if (num >= 0)
            {
                string str2 = sLoginName.Substring(0, num);
                str = string.Concat("i:0#.f|", str2, "|");
                str1 = sLoginName.Substring(num + 1);
            }
            else
            {
                str = "i:0#.w|";
                str1 = sLoginName;
            }

            str1 = str1.Replace("%", "%25");
            str1 = str1.Replace(",", "%2C");
            str1 = str1.Replace(":", "%3A");
            str1 = str1.Replace(";", "%3B");
            str1 = str1.Replace("\n", "%0A");
            str1 = str1.Replace("\r", "%0D");
            str1 = str1.Replace("|", "%7C");
            return string.Concat(str, str1).ToLowerInvariant();
        }

        public static string CorrectFieldReferencesInListViews(string listFieldsSchemaXml,
            string listViewsSchemaXml = null)
        {
            if (string.IsNullOrEmpty(listFieldsSchemaXml))
            {
                throw new ArgumentNullException("listFieldsSchemaXml", "Cannot be null or empty");
            }

            if (string.IsNullOrEmpty(listViewsSchemaXml))
            {
                throw new ArgumentNullException("listViewsSchemaXml", "Cannot be null or empty");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(listFieldsSchemaXml);
            XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("//Fields/Field");
            if (xmlNodeLists.Count == 0)
            {
                return listViewsSchemaXml;
            }

            Dictionary<string, string> strs = new Dictionary<string, string>();
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                string attributeValueAsString = xmlNodes.GetAttributeValueAsString("StaticName");
                string str = xmlNodes.GetAttributeValueAsString("Name");
                if (string.Equals(attributeValueAsString, str, StringComparison.Ordinal))
                {
                    continue;
                }

                strs.Add(attributeValueAsString, str);
            }

            if (strs.Count == 0)
            {
                return listViewsSchemaXml;
            }

            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(listViewsSchemaXml);
            XmlNodeList xmlNodeLists1 = xmlDocument1.SelectNodes("//Views/View");
            if (xmlNodeLists1.Count == 0)
            {
                return listViewsSchemaXml;
            }

            foreach (XmlNode xmlNodes1 in xmlNodeLists1)
            {
                XmlNodeList xmlNodeLists2 = xmlNodes1.SelectNodes("//FieldRef");
                if (xmlNodeLists2.Count == 0)
                {
                    continue;
                }

                foreach (XmlNode item in xmlNodeLists2)
                {
                    string attributeValueAsString1 = item.GetAttributeValueAsString("Name");
                    if (string.IsNullOrEmpty(attributeValueAsString1) || !strs.ContainsKey(attributeValueAsString1))
                    {
                        continue;
                    }

                    item.Attributes["Name"].Value = strs[attributeValueAsString1];
                }
            }

            return xmlDocument1.OuterXml;
        }

        public static string CorrectRootSiteCollectionTitle(string sWebXml)
        {
            string outerXml = sWebXml;
            try
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXml);
                if (xmlNode.Attributes["ServerRelativeUrl"] != null &&
                    xmlNode.Attributes["ServerRelativeUrl"].Value == "/" && xmlNode.Attributes["Name"] != null)
                {
                    if (xmlNode.Attributes["Title"] == null || string.IsNullOrEmpty(xmlNode.Attributes["Title"].Value))
                    {
                        xmlNode.Attributes["Name"].Value = "AutoGeneratedNameFromRootSiteCollection";
                    }
                    else
                    {
                        xmlNode.Attributes["Name"].Value = Utils.RectifyName(
                            xmlNode.Attributes["Title"].Value.Replace(" ", "_"), '\u005F', null, RectifyClass.SPWeb);
                    }

                    outerXml = xmlNode.OuterXml;
                }
            }
            catch
            {
                outerXml = sWebXml;
            }

            return outerXml;
        }

        public static string EnsureFieldNameSafety(string sFieldName)
        {
            if (sFieldName == XmlConvert.EncodeLocalName(sFieldName))
            {
                return sFieldName;
            }

            for (int i = sFieldName.LastIndexOf("_x", StringComparison.InvariantCulture);
                 i >= 0;
                 i = sFieldName.LastIndexOf("_x", StringComparison.InvariantCulture))
            {
                if (i + 7 <= sFieldName.Length)
                {
                    return sFieldName;
                }

                sFieldName = sFieldName.Remove(i);
            }

            return sFieldName;
        }

        public static string EnsureFieldRefsIncludesFields(string sFieldRefs, string[] fieldsToEnsure)
        {
            if (string.IsNullOrEmpty(sFieldRefs))
            {
                return sFieldRefs;
            }

            bool flag = false;
            if (sFieldRefs.StartsWith("<fieldref", StringComparison.OrdinalIgnoreCase))
            {
                flag = true;
                sFieldRefs = string.Concat("<ViewFields>", sFieldRefs, "</ViewFields>");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFieldRefs);
            XmlNode documentElement = xmlDocument.DocumentElement;
            Utils.EnsureFieldRefsIncludesFields(documentElement, fieldsToEnsure);
            if (flag)
            {
                return documentElement.InnerXml;
            }

            return documentElement.OuterXml;
        }

        public static void EnsureFieldRefsIncludesFields(XmlNode fieldRefs, string[] fieldsToEnsure)
        {
            string[] strArrays = fieldsToEnsure;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                XmlNode xmlNodes = fieldRefs.SelectSingleNode(string.Concat("./FieldRef[@Name='", str, "']"));
                if (xmlNodes == null)
                {
                    xmlNodes = fieldRefs.OwnerDocument.CreateElement("FieldRef");
                    XmlAttribute xmlAttribute = xmlNodes.OwnerDocument.CreateAttribute("Name");
                    xmlAttribute.Value = str;
                    xmlNodes.Attributes.Append(xmlAttribute);
                    fieldRefs.AppendChild(xmlNodes);
                }
            }
        }

        public static string EnumerableToString(IEnumerable collection, string sDelimiter)
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            foreach (object obj in collection)
            {
                stringBuilder.Append(string.Concat((stringBuilder.Length > 0 ? sDelimiter : ""), obj.ToString()));
            }

            return stringBuilder.ToString();
        }

        public static string FormatDate(DateTime dt)
        {
            return dt.ToString("o", Utils.s_EnglishCulture);
        }

        public static string FormatDateToUTC(DateTime dt)
        {
            return Utils.FormatDateToUTC(dt, TimeZoneInformation.GetLocalTimeZone());
        }

        public static string FormatDateToUTC(DateTime dt, TimeZoneInformation timeZone)
        {
            if (timeZone == null)
            {
                dt = Utils.MakeTrueUTCDateTime(dt);
            }
            else
            {
                dt = timeZone.LocalTimeToUtc(dt);
            }

            return Utils.FormatDate(dt);
        }

        public static string GetCommonPathPrefix(string sFirstPath, string sSecondPath, out string sFirstPathSuffix,
            out string sSecondPathSuffix)
        {
            if (string.IsNullOrEmpty(sFirstPath))
            {
                sFirstPathSuffix = "";
                sSecondPathSuffix = sSecondPath;
                return "";
            }

            if (string.IsNullOrEmpty(sSecondPath))
            {
                sFirstPathSuffix = sFirstPath;
                sSecondPathSuffix = "";
                return "";
            }

            sFirstPath = sFirstPath.Trim().Trim(new char[] { '/' });
            sSecondPath = sSecondPath.Trim().Trim(new char[] { '/' });
            string str = "";
            int num = -1;
            bool flag = false;
            do
            {
                flag = false;
                num = sFirstPath.IndexOf('/', (num >= 0 ? num + 1 : 0));
                string str1 = (num >= 0 ? string.Concat(sFirstPath.Substring(0, num), "/") : sFirstPath);
                if (!sSecondPath.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                str = str1;
                flag = true;
            } while (flag && num >= 0);

            sFirstPathSuffix = sFirstPath.Substring(str.Length);
            sSecondPathSuffix = sSecondPath.Substring(str.Length);
            return str;
        }

        public static List<string> GetContentTypeIDAndParentIDs(string contentTypeID)
        {
            List<string> strs = new List<string>()
            {
                contentTypeID
            };
            while (Utils.GetContentTypeIDIsSubContentType(contentTypeID))
            {
                contentTypeID = contentTypeID.Substring(0, contentTypeID.Length - 34);
                strs.Add(contentTypeID);
            }

            return strs;
        }

        public static bool GetContentTypeIDIsSubContentType(string contentTypeID)
        {
            return Regex.IsMatch(contentTypeID, "0{2}[0-9a-fA-F]{32}$");
        }

        public static void GetExceptionMessage(Exception exception, ref StringBuilder message)
        {
            if (exception != null)
            {
                bool flag = string.IsNullOrEmpty(exception.StackTrace);
                StringBuilder stringBuilder = message;
                object[] newLine = new object[] { exception.Message, null, null, null, null };
                newLine[1] = (flag ? string.Empty : Environment.NewLine);
                newLine[2] = (flag ? string.Empty : "Stack Trace: ");
                newLine[3] = (flag ? string.Empty : exception.StackTrace);
                newLine[4] = Environment.NewLine;
                stringBuilder.AppendLine(string.Format("{0}{1}{2}{3}{4}", newLine));
                foreach (DictionaryEntry datum in exception.Data)
                {
                    object[] key = new object[] { datum.Key, Environment.NewLine, datum.Value, Environment.NewLine };
                    message.AppendLine(string.Format("Data Item '{0}':{1}{2}{3}", key));
                }

                if (exception.InnerException != null)
                {
                    message.AppendLine("Inner Exception:");
                    Utils.GetExceptionMessage(exception.InnerException, ref message);
                }
            }
        }

        public static void GetExceptionMessageAndDetail(Exception exception, out string exceptionMessage,
            out string exceptionStackDetails)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            Utils.GetExceptionMessageAndDetailRecursively(exception, ref stringBuilder, ref stringBuilder1);
            exceptionMessage = stringBuilder.ToString();
            exceptionStackDetails = stringBuilder1.ToString();
        }

        public static Metalogix.SharePoint.Adapters.ExceptionDetail GetExceptionMessageAndDetail(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder();
            Utils.GetExceptionMessageAndDetailRecursively(exception, ref stringBuilder, ref stringBuilder1);
            PropertyInfo property = exception.GetType().GetProperty("HResult");
            object obj = (property == null || !property.CanRead ? null : property.GetValue(exception, null));
            return new Metalogix.SharePoint.Adapters.ExceptionDetail(stringBuilder.ToString(),
                stringBuilder1.ToString(), (obj != null ? (int)obj : 0));
        }

        private static void GetExceptionMessageAndDetailRecursively(Exception exception, ref StringBuilder message,
            ref StringBuilder details)
        {
            if (exception != null)
            {
                message.AppendLine(string.Format("{0}{1}{2}", exception.Message, Environment.NewLine,
                    (details == null
                        ? string.Format("Type: {0}{1}", exception.GetType().ToString(), Environment.NewLine)
                        : string.Empty)));
                if (details != null)
                {
                    if (!string.IsNullOrEmpty(exception.StackTrace))
                    {
                        details.AppendLine(string.Format("Stack: {0}{1}", exception.StackTrace, Environment.NewLine));
                    }

                    details.AppendLine(string.Format("Type: {0}{1}", exception.GetType().ToString(),
                        Environment.NewLine));
                    foreach (DictionaryEntry datum in exception.Data)
                    {
                        object[] key = new object[]
                            { datum.Key, Environment.NewLine, datum.Value, Environment.NewLine };
                        details.AppendLine(string.Format("Data Item '{0}':{1}{2}{3}", key));
                    }
                }

                if (exception.InnerException != null)
                {
                    message.Append("Inner Exception: ");
                    if (details != null && !string.IsNullOrEmpty(exception.InnerException.StackTrace))
                    {
                        details.Append("Inner Exception ");
                    }

                    Utils.GetExceptionMessageAndDetailRecursively(exception.InnerException, ref message, ref details);
                }
            }
        }

        public static string GetExceptionMessageOnly(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = null;
            Utils.GetExceptionMessageAndDetailRecursively(exception, ref stringBuilder, ref stringBuilder1);
            return stringBuilder.ToString();
        }

        public static XmlNode GetFieldNodeById(XmlNodeList fields, Guid fieldId)
        {
            XmlNode xmlNodes;
            try
            {
                foreach (XmlNode field in fields)
                {
                    if (field.Attributes["ID"] == null)
                    {
                        continue;
                    }

                    string value = field.Attributes["ID"].Value;
                    if (!Utils.IsGuid(value) || !fieldId.Equals(new Guid(value)))
                    {
                        continue;
                    }

                    xmlNodes = field;
                    return xmlNodes;
                }

                xmlNodes = null;
            }
            catch
            {
                xmlNodes = null;
            }

            return xmlNodes;
        }

        public static string GetFileNameFromPath(string sFilePath, bool bWithExtension)
        {
            if (sFilePath == null)
            {
                return null;
            }

            char[] chrArray = new char[] { '/', '\\' };
            int num = sFilePath.LastIndexOfAny(chrArray);
            string str = sFilePath;
            if (num >= 0)
            {
                str = (num >= sFilePath.Length - 1 ? "" : sFilePath.Substring(num + 1));
            }

            if (!bWithExtension)
            {
                int num1 = str.LastIndexOf('.');
                if (num1 >= 0)
                {
                    str = str.Substring(0, num1);
                }
            }

            return str;
        }

        public static Guid[] GetGuidCollectionUnion(IEnumerable<Guid> collection1, IEnumerable<Guid> collection2)
        {
            List<Guid> guids = new List<Guid>();
            if (collection1 != null)
            {
                foreach (Guid guid in collection1)
                {
                    if (guids.Contains(guid))
                    {
                        continue;
                    }

                    guids.Add(guid);
                }
            }

            if (collection2 != null)
            {
                foreach (Guid guid1 in collection2)
                {
                    if (guids.Contains(guid1))
                    {
                        continue;
                    }

                    guids.Add(guid1);
                }
            }

            return guids.ToArray();
        }

        private static RegistryKey GetISAPDirectoryRegistryKey()
        {
            RegistryKey registryKey =
                Registry.LocalMachine.OpenSubKey((Utils.SystemIs64Bit
                    ? "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint"
                    : "SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint")) ??
                Registry.LocalMachine.OpenSubKey(
                    "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint16.0");
            return registryKey;
        }

        public static string GetNameFromURL(string sURL)
        {
            if (sURL == null)
            {
                return null;
            }

            if (sURL.LastIndexOf("/", StringComparison.Ordinal) < 0)
            {
                return sURL;
            }

            return sURL.Substring(sURL.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        public static bool GetQueryIsUsingThresholdApprovedSorting(string sCamlQuery)
        {
            if (!sCamlQuery.StartsWith("<query", StringComparison.OrdinalIgnoreCase))
            {
                sCamlQuery = string.Concat("<Query>", sCamlQuery, "</Query>");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sCamlQuery);
            return Utils.GetQueryIsUsingThresholdApprovedSorting(xmlDocument.DocumentElement);
        }

        public static bool GetQueryIsUsingThresholdApprovedSorting(XmlNode query)
        {
            XmlNodeList xmlNodeLists = query.SelectNodes(".//Query/OrderBy/FieldRef");
            if (xmlNodeLists.Count == 1)
            {
                XmlAttribute itemOf = xmlNodeLists[0].Attributes["Name"];
                if (itemOf != null && itemOf.Value.Equals("ID"))
                {
                    return true;
                }
            }
            else if (xmlNodeLists.Count == 2)
            {
                XmlNode xmlNodes = xmlNodeLists[0];
                XmlAttribute xmlAttribute = xmlNodes.Attributes["Name"];
                if (xmlAttribute == null || !xmlAttribute.Value.Equals("FileDirRef"))
                {
                    return false;
                }

                xmlNodes = xmlNodeLists[1];
                xmlAttribute = xmlNodes.Attributes["Name"];
                if (xmlAttribute != null && xmlAttribute.Value.Equals("FileLeafRef"))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] GetQueryOrderByFields(string sCamlQuery)
        {
            if (!sCamlQuery.StartsWith("<query", StringComparison.OrdinalIgnoreCase))
            {
                sCamlQuery = string.Concat("<Query>", sCamlQuery, "</Query>");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sCamlQuery);
            return Utils.GetQueryOrderByFields(xmlDocument.DocumentElement);
        }

        public static string[] GetQueryOrderByFields(XmlNode query)
        {
            XmlNodeList xmlNodeLists = query.SelectNodes("./OrderBy/FieldRef");
            List<string> strs = new List<string>(xmlNodeLists.Count);
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                XmlAttribute itemOf = xmlNodes.Attributes["Name"];
                if (itemOf == null)
                {
                    continue;
                }

                strs.Add(itemOf.Value);
            }

            return strs.ToArray();
        }

        public static string GetServerPart(string sUrl)
        {
            if (sUrl == null)
            {
                return null;
            }

            int num = sUrl.IndexOf("/", "https://".Length, StringComparison.Ordinal);
            if (num < 0)
            {
                return sUrl;
            }

            return sUrl.Substring(0, num);
        }

        public static string GetServerRelativeUrlPart(string sUrl)
        {
            if (sUrl == null)
            {
                return null;
            }

            int num = sUrl.IndexOf("/", "https://".Length, StringComparison.Ordinal);
            if (num < 0)
            {
                return "/";
            }

            return sUrl.Substring(num);
        }

        public static string GetSharePointIsapiFolderFromRegistry()
        {
            if (!string.IsNullOrEmpty(Utils._folderLocation))
            {
                return Utils._folderLocation;
            }

            using (RegistryKey sAPDirectoryRegistryKey = Utils.GetISAPDirectoryRegistryKey())
            {
                Utils._folderLocation = sAPDirectoryRegistryKey.GetValue("").ToString();
            }

            return Utils._folderLocation;
        }

        public static SqlConnection GetSQLConnection(string sSqlServer, Credentials creds)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder["Data Source"] = sSqlServer;
            if (!creds.IsDefault)
            {
                sqlConnectionStringBuilder.UserID = creds.UserName;
                sqlConnectionStringBuilder.Password = creds.Password.ToInsecureString();
            }
            else
            {
                sqlConnectionStringBuilder["integrated Security"] = true;
            }

            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }

        public static void GetUrlPathOverlap(string sStartUrl, string sEndUrl, out string sPreOverlap,
            out string sOverlap, out string sPostOverlap)
        {
            string str = sStartUrl.Replace('\\', '/').TrimEnd(new char[] { '/' });
            string str1 = sEndUrl.Replace('\\', '/').Trim(new char[] { '/' });
            string lower = str.ToLower();
            string lower1 = str1.ToLower();
            int num = -1;
            int length = -1;
            if (!lower1.StartsWith(lower, StringComparison.InvariantCultureIgnoreCase))
            {
                int num1 = lower.LastIndexOf('/');
                while (num1 >= 0)
                {
                    string str2 = lower.Substring(num1 + 1);
                    if (!lower1.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase) ||
                        lower1.Length != str2.Length && lower1[str2.Length] != '/')
                    {
                        num1 = (num1 > 0 ? lower.LastIndexOf('/', num1 - 1) : -1);
                    }
                    else
                    {
                        num = num1 + 1;
                        length = str2.Length;
                        break;
                    }
                }
            }
            else
            {
                num = 0;
                length = str.Length;
            }

            if (num < 0 || length <= 0)
            {
                sPreOverlap = sStartUrl;
                sOverlap = "";
                sPostOverlap = sEndUrl;
                return;
            }

            sPreOverlap = str.Substring(0, num);
            sOverlap = str.Substring(num, length);
            sPostOverlap = str1.Substring(length);
        }

        public static string GetWorkflowName(string workflowName)
        {
            workflowName = Utils.RemoveAssociationInformationFromWorkflowName(workflowName);
            return Utils.RemovePreviousVersionTextFromWorkflowName(workflowName);
        }

        public static int GetWorkflowUIVersionId(string internalName)
        {
            if (!string.IsNullOrEmpty(internalName))
            {
                string str = internalName.Substring(0, internalName.Length - 2);
                if (str.LastIndexOf('.') > 0)
                {
                    int num = 0;
                    str = str.Substring(str.LastIndexOf('.') + 1);
                    if (int.TryParse(str, out num))
                    {
                        return num;
                    }
                }
            }

            return 0;
        }

        public static bool HasTaxonomySupport()
        {
            if (!Utils.m_supportsTaxonomy.HasValue)
            {
                Type type = Type.GetType(
                    "Microsoft.SharePoint.Taxonomy.TaxonomySession, Microsoft.SharePoint.Taxonomy, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
                Utils.m_supportsTaxonomy = new bool?(type != null);
            }

            return Utils.m_supportsTaxonomy.Value;
        }

        public static byte[] HexToDecimal(string sHex)
        {
            if (sHex == null || sHex.Length <= 0)
            {
                return null;
            }

            int num = (sHex.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0);
            int num1 = Convert.ToInt32(Math.Ceiling((double)(sHex.Length - num) / 2));
            byte[] numArray = new byte[num1];
            int num2 = 0;
            for (int i = num; i <= sHex.Length - 2; i += 2)
            {
                string str = sHex.Substring(i, 2);
                numArray[num2] = Convert.ToByte(str, 16);
                num2++;
            }

            return numArray;
        }

        public static string HexToText(string sHex)
        {
            if (string.IsNullOrEmpty(sHex))
            {
                return sHex;
            }

            int num = (sHex.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0);
            int num1 = Convert.ToInt32(Math.Ceiling((double)(sHex.Length - num) / 2));
            byte[] numArray = new byte[num1];
            int num2 = 0;
            for (int i = num; i < sHex.Length - 1; i += 2)
            {
                string str = sHex.Substring(i, 2);
                numArray[num2] = Convert.ToByte(str, 16);
                num2++;
            }

            return Encoding.UTF8.GetString(numArray);
        }

        public static bool IsAdditionalLookupColumn(XmlDocument xmlDocument, Guid fieldId,
            string targetListFieldXml = null)
        {
            XmlNode xmlNodes =
                xmlDocument.SelectSingleNode(string.Concat("./Fields/Field[@ID='", fieldId.ToString("B"), "']"));
            if (xmlNodes == null && !string.IsNullOrEmpty(targetListFieldXml))
            {
                xmlDocument.LoadXml(targetListFieldXml);
                xmlNodes = xmlDocument.SelectSingleNode(string.Concat("./Fields/Field[@ID='", fieldId.ToString("B"),
                    "']"));
            }

            if (xmlNodes == null)
            {
                return false;
            }

            return Utils.CheckAdditionalLookupField(xmlNodes);
        }

        public static bool IsDocumentWikiPage(XmlNode listItemXML, string fieldPrefix = "")
        {
            if (listItemXML.Attributes[string.Concat(fieldPrefix, "WikiField")] != null && listItemXML
                    .GetAttributeValueAsString(string.Concat(fieldPrefix, "File_x0020_Type"))
                    .Equals("aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsGuid(string sGUID)
        {
            bool flag;
            try
            {
                if (string.IsNullOrEmpty(sGUID) || sGUID.Length != 36 && sGUID.Length != 38)
                {
                    flag = false;
                }
                else
                {
                    Guid guid = new Guid(sGUID);
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        public static bool IsGUID(string s)
        {
            Guid empty;
            if (!s.StartsWith("{", StringComparison.Ordinal))
            {
                s = string.Concat("{", s);
            }

            if (!s.EndsWith("}", StringComparison.Ordinal))
            {
                s = string.Concat(s, "}");
            }

            if (string.IsNullOrEmpty(s))
            {
                empty = Guid.Empty;
                return false;
            }

            if (Metalogix.SharePoint.Adapters.NativeMethods.CLSIDFromString(s, out empty) >= 0)
            {
                return true;
            }

            empty = Guid.Empty;
            return false;
        }

        public static bool IsOOBWorkflowAssociation(Guid workflowTemplateId, int languageID,
            bool isCheckforGloballyReusableWorkflowTemplate = false)
        {
            bool flag;
            if (Utils._languageNaturalWorkflowTemplateIds.Contains(workflowTemplateId))
            {
                return true;
            }

            List<string>.Enumerator enumerator = Utils._languageSpecificWorkflowTemplateIds.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (!isCheckforGloballyReusableWorkflowTemplate)
                    {
                        string str = string.Concat(current, languageID.ToString("X5"));
                        if (!(new Guid(str)).Equals(workflowTemplateId))
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }
                    else
                    {
                        if (!workflowTemplateId.ToString().StartsWith(current.ToString(),
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }
                }

                return false;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return flag;
        }

        public static bool IsRecurringWorkflowXml(XmlDocument configFileXmlDoc)
        {
            XmlNode xmlNodes = configFileXmlDoc.DocumentElement.SelectSingleNode(".//Template");
            if (xmlNodes != null && xmlNodes.Attributes["Category"] != null)
            {
                string value = xmlNodes.Attributes["Category"].Value;
                if (!value.Equals("List", StringComparison.OrdinalIgnoreCase) &&
                    !value.Equals("Site", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsValidHostHeaderURL(string siteURL)
        {
            Uri uri;
            string host = siteURL;
            if (!Uri.TryCreate(siteURL, UriKind.Absolute, out uri))
            {
                return false;
            }

            host = uri.Host;
            return Utils.IsValidSharePointURL(host, true);
        }

        public static bool IsValidSharePointURL(string siteURL, bool isHostHeaderSiteURL = false)
        {
            if ((new Regex((isHostHeaderSiteURL
                    ? Utils.illegalCharactersForHostHeaderSiteUrlRegEx
                    : Utils.illegalCharactersRegex))).IsMatch(siteURL))
            {
                return false;
            }

            return true;
        }

        public static bool IsVideoFile(string listTemplate, string sourceContentTypeID,
            string targetContentTypeID = null)
        {
            if (string.IsNullOrEmpty(targetContentTypeID) && !string.IsNullOrEmpty(sourceContentTypeID) &&
                (sourceContentTypeID.StartsWith(
                     "0x0101009148F5A04DDD49CBA7127AADA5FB792B00291D173ECE694D56B19D111489C4369D",
                     StringComparison.InvariantCultureIgnoreCase) ||
                 sourceContentTypeID.StartsWith("0x0120D520A808", StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            if (sourceContentTypeID.StartsWith(
                    "0x0101009148F5A04DDD49CBA7127AADA5FB792B00291D173ECE694D56B19D111489C4369D",
                    StringComparison.InvariantCultureIgnoreCase) &&
                targetContentTypeID.StartsWith("0x0120D520A808", StringComparison.InvariantCultureIgnoreCase) &&
                listTemplate.Equals("851", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsWritableColumn(string sFieldInternalName, bool bFieldReadOnly, string sFieldTypeAsString,
            int iListBaseTemplate, bool bItemIsFolder, bool bIsBDCField, bool isAzure = false)
        {
            bool flag;
            bool flag1;
            bool flag2;
            bool flag3 = (!isAzure
                ? false
                : sFieldInternalName.Equals("HTML_x0020_File_x0020_Type", StringComparison.InvariantCultureIgnoreCase));
            int num = 108;
            int num1 = 880;
            if (!bItemIsFolder && iListBaseTemplate != num &&
                (sFieldInternalName == "ContentType" || sFieldInternalName == "ContentTypeId") ||
                sFieldInternalName == "PublishingPageLayout" || sFieldInternalName == "Author" ||
                sFieldInternalName == "Editor" || sFieldInternalName == "Modified_x0020_By" ||
                sFieldInternalName == "Created" || sFieldInternalName == "Modified" ||
                sFieldInternalName == "InstanceID" || sFieldInternalName == "NameOverloaded" ||
                iListBaseTemplate == num && sFieldInternalName == "DiscussionLastUpdated" || bIsBDCField)
            {
                flag = true;
            }
            else if (iListBaseTemplate != num1)
            {
                flag = false;
            }
            else
            {
                flag = (sFieldInternalName == "Member" ||
                        string.Equals(sFieldInternalName, "NumberOfBestResponses",
                            StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals(sFieldInternalName, "ReputationScore",
                            StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals(sFieldInternalName, "NumberOfReplies",
                            StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals(sFieldInternalName, "NumberOfDiscussions",
                            StringComparison.InvariantCultureIgnoreCase)
                    ? true
                    : string.Equals(sFieldInternalName, "LastActivity", StringComparison.InvariantCultureIgnoreCase));
            }

            bool flag4 = flag;
            if (bFieldReadOnly && sFieldInternalName != "WorkflowOutcome" && sFieldInternalName != "WorkflowLink" ||
                sFieldTypeAsString == "Attachments")
            {
                flag1 = true;
            }
            else if (iListBaseTemplate != num)
            {
                flag1 = false;
            }
            else
            {
                flag1 = (sFieldInternalName == "ParentFolderId" || sFieldInternalName == "Order"
                    ? true
                    : sFieldInternalName == "ThreadIndex");
            }

            bool flag5 = flag1;
            bool flag6 = (sFieldInternalName == "MetaInfo" || sFieldInternalName == "FileLeafRef" ||
                          sFieldInternalName == "_FileRef" || sFieldInternalName == "UniqueId" ||
                          sFieldInternalName == "CheckedOutTitle" || sFieldInternalName == "LinkFilenameNoMenu" ||
                          sFieldInternalName == "LinkFilename" || sFieldInternalName == "LinkCheckedOutTitle" ||
                          sFieldInternalName == "LinkTitle" || sFieldInternalName == "LinkTitleNoMenu" ||
                          sFieldInternalName == "FileType" || sFieldInternalName == "ImageSize" ||
                          sFieldInternalName == "ImageThumbnailDisplay" || sFieldInternalName == "PreviewOnForm" ||
                          sFieldInternalName == "Thumbnail" || sFieldInternalName == "LargeThumbnail" ||
                          sFieldInternalName == "ThumbnailOnForm"
                ? true
                : sFieldInternalName == "Attachments");
            bool flag7 = sFieldTypeAsString == "PageSeparator";
            if (iListBaseTemplate != num)
            {
                flag2 = false;
            }
            else
            {
                flag2 = (sFieldInternalName.Equals("MemberLookup", StringComparison.InvariantCultureIgnoreCase)
                    ? true
                    : sFieldInternalName.Equals("DescendantLikesCount", StringComparison.InvariantCultureIgnoreCase));
            }

            bool flag8 = flag2;
            if (flag5 && !flag4 && !flag8 && !flag3 || flag6)
            {
                return false;
            }

            return !flag7;
        }

        public static bool IsWritableColumnForManifest(string sFieldInternalName, bool bFieldReadOnly,
            string sFieldTypeAsString, int iListBaseTemplate, bool bItemIsFolder, bool bIsBDCField, Guid guid)
        {
            if (Utils.FieldsToIgnoreForManifest.Contains(guid))
            {
                return false;
            }

            return Utils.IsWritableColumn(sFieldInternalName, bFieldReadOnly, sFieldTypeAsString, iListBaseTemplate,
                bItemIsFolder, bIsBDCField, true);
        }

        public static string JoinUrl(string sStart, string sEnd)
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            char[] chrArray = new char[] { '/' };
            stringBuilder.Append(sStart.TrimEnd(chrArray));
            stringBuilder.Append('/');
            string str = sEnd.TrimStart(new char[] { '/' });
            sEnd = str;
            stringBuilder.Append(str);
            return stringBuilder.ToString();
        }

        public static void LogExceptionDetails(Exception ex, string methodName, string className,
            string category = null)
        {
            object[] managedThreadId = new object[]
                { Thread.CurrentThread.ManagedThreadId, methodName, className, Convert.ToString(ex) };
            Trace.WriteLine(string.Format("{0}, {1}, {2}, {3}", managedThreadId), category);
        }

        public static void LogMessageDetails(string message, string methodName, string className,
            string category = null)
        {
            object[] managedThreadId = new object[]
                { Thread.CurrentThread.ManagedThreadId, methodName, message, className };
            Trace.WriteLine(string.Format("{0}, {1}, {2}, {3}", managedThreadId), category);
        }

        public static DateTime MakeTrueUTCDateTime(DateTime date)
        {
            return new DateTime(date.Ticks, DateTimeKind.Utc);
        }

        public static DateTime ParseDate(string sDateTime)
        {
            return Utils.ParseDate(sDateTime, null);
        }

        public static DateTime ParseDate(string sDateTime, TimeZoneInformation timeZone)
        {
            DateTime localTime =
                DateTime.ParseExact(sDateTime, "o", Utils.s_EnglishCulture, DateTimeStyles.AssumeUniversal);
            if (timeZone != null)
            {
                localTime = timeZone.UtcToLocalTime(localTime.ToUniversalTime());
            }

            return localTime;
        }

        public static DateTime ParseDateAsUtc(string sDateTime)
        {
            return Utils.ParseDate(sDateTime).ToUniversalTime();
        }

        public static void ParseUrlForLeafName(string sUrl, out string sDirName, out string sLeafName)
        {
            int num = sUrl.LastIndexOf("/", StringComparison.Ordinal);
            sDirName = (num >= 0 ? sUrl.Substring(0, num).Trim(new char[] { '/' }) : "");
            sLeafName = (num >= 0 ? sUrl.Substring(num + 1).Trim(new char[] { '/' }) : sUrl);
        }

        public static string RectifyName(string sName, char cReplacement, SharePointAdapter adapter,
            RectifyClass rectifyClass)
        {
            string str;
            if (!string.IsNullOrEmpty(sName))
            {
                switch (rectifyClass)
                {
                    case RectifyClass.SPFolder:
                    {
                        char[] chrArray = new char[]
                        {
                            '~', '#', '%', '&', '*', '{', '}', '|', '\\', ':', '\"', '<', '>', '?', '/', '\t', '\n',
                            '\r'
                        };
                        for (int i = 0; i < (int)chrArray.Length; i++)
                        {
                            if (cReplacement == chrArray[i])
                            {
                                throw new ArgumentException(
                                    string.Concat("Replacement character \"", cReplacement, "\" is also illegal."),
                                    "cReplacement");
                            }

                            sName = sName.Replace(chrArray[i], cReplacement);
                        }

                        char[] chrArray1 = new char[] { '.', ' ' };
                        sName = sName.TrimStart(chrArray1);
                        if (adapter == null || !adapter.SharePointVersion.IsSharePoint2007)
                        {
                            str = sName.TrimEnd(new char[] { '.' });
                        }
                        else
                        {
                            char[] chrArray2 = new char[] { '.', ':' };
                            str = sName.TrimEnd(chrArray2);
                        }

                        sName = str;
                        while (sName.Contains(".."))
                        {
                            sName = sName.Replace("..", ".");
                        }

                        while (sName.Contains("  "))
                        {
                            sName = sName.Replace("  ", " ");
                        }

                        break;
                    }
                    case RectifyClass.SPList:
                    {
                        break;
                    }
                    case RectifyClass.SPListItem:
                    {
                        char[] chrArray3 = new char[]
                        {
                            '~', '#', '%', '&', '*', '{', '}', '|', '\\', ':', '\"', '<', '>', '?', '/', '\t', '\n',
                            '\r'
                        };
                        for (int j = 0; j < (int)chrArray3.Length; j++)
                        {
                            if (cReplacement == chrArray3[j])
                            {
                                throw new ArgumentException(
                                    string.Concat("Replacement character \"", cReplacement, "\" is also illegal."),
                                    "cReplacement");
                            }

                            sName = sName.Replace(chrArray3[j], cReplacement);
                        }

                        char[] chrArray4 = new char[] { '.', ' ' };
                        sName = sName.TrimStart(chrArray4);
                        sName = sName.TrimEnd(new char[] { '.' });
                        while (sName.Contains(".."))
                        {
                            sName = sName.Replace("..", ".");
                        }

                        break;
                    }
                    case RectifyClass.SPWeb:
                    {
                        char[] chrArray5 = new char[]
                        {
                            '~', '#', '%', '&', '*', '+', '{', '}', '|', '\\', ':', '\"', '<', '>', '?', '/', '\t',
                            '\n', '\r'
                        };
                        for (int k = 0; k < (int)chrArray5.Length; k++)
                        {
                            if (cReplacement == chrArray5[k])
                            {
                                throw new ArgumentException(
                                    string.Concat("Replacement character \"", cReplacement, "\" is also illegal."),
                                    "cReplacement");
                            }

                            sName = sName.Replace(chrArray5[k], cReplacement);
                        }

                        char[] chrArray6 = new char[] { '\u005F', '.', ' ' };
                        sName = sName.TrimStart(chrArray6);
                        sName = sName.TrimEnd(new char[] { '.' });
                        while (sName.Contains(".."))
                        {
                            sName = sName.Replace("..", ".");
                        }

                        while (sName.Contains("  "))
                        {
                            sName = sName.Replace("  ", " ");
                        }

                        if (!sName.EndsWith(".com", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }

                        sName = string.Format("{0}_com", sName.Remove(sName.Length - 4));
                        break;
                    }
                    default:
                    {
                        char[] chrArray7 = new char[]
                            { '~', '#', '%', '&', '*', '{', '}', '\\', ':', '<', '>', '?', '/', '+', '|', '\"' };
                        for (int l = 0; l < (int)chrArray7.Length; l++)
                        {
                            if (cReplacement == chrArray7[l])
                            {
                                throw new ArgumentException(
                                    string.Concat("Replacement character \"", cReplacement, "\" is also illegal."),
                                    "cReplacement");
                            }

                            sName = sName.Replace(chrArray7[l], cReplacement);
                        }

                        char[] chrArray8 = new char[] { '\u005F', '.', ' ', '\t', '\n', '\r' };
                        sName = sName.TrimStart(chrArray8);
                        char[] chrArray9 = new char[] { '.', ' ', '\t', '\n', '\r' };
                        sName = sName.TrimEnd(chrArray9);
                        while (sName.Contains(".."))
                        {
                            sName = sName.Replace("..", ".");
                        }

                        break;
                    }
                }
            }

            return sName;
        }

        public static string RectifyName(string sName, char cReplacement)
        {
            if (string.IsNullOrEmpty(sName))
            {
                return sName;
            }

            foreach (char illegalCharacter in Utils.IllegalCharacters)
            {
                sName = sName.Replace(illegalCharacter, cReplacement);
            }

            while (sName.Contains(".."))
            {
                sName = sName.Replace("..", ".");
            }

            char[] chrArray = new char[]
            {
                '.', '\u005F', '\t', '\n', '\v', '\f', '\r', ' ', '\u0085', '\u00A0', '\u1680', '\u180E', '\u2000',
                '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A',
                '\u2028', '\u2029', '\u202F', '\u205F', '\u3000'
            };
            sName = sName.Trim(chrArray);
            if (sName.EndsWith(".com", StringComparison.InvariantCultureIgnoreCase))
            {
                sName = string.Format("{0}_com", sName.Remove(sName.Length - 4));
            }

            return sName;
        }

        public static string RectifyPath(string sPath, char cReplacement, SharePointAdapter adapter,
            RectifyClass rectifyClass)
        {
            if (string.IsNullOrEmpty(sPath))
            {
                return string.Empty;
            }

            char[] chrArray = new char[]
            {
                '/', '\\', '\t', '\n', '\v', '\f', '\r', ' ', '\u0085', '\u00A0', '\u1680', '\u180E', '\u2000',
                '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A',
                '\u2028', '\u2029', '\u202F', '\u205F', '\u3000'
            };
            string str = sPath.Trim(chrArray);
            char[] chrArray1 = new char[] { '/', '\\' };
            string[] strArrays = str.Split(chrArray1);
            StringBuilder stringBuilder = new StringBuilder(str.Length);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                strArrays[i] = Utils.RectifyName(strArrays[i], cReplacement, adapter, rectifyClass);
                if (string.IsNullOrEmpty(strArrays[i]))
                {
                    strArrays[i] = "folder";
                }

                stringBuilder.Append(string.Concat(strArrays[i], "/"));
            }

            return stringBuilder.ToString().TrimEnd(new char[] { '/' });
        }

        public static string RectifyPath(string sPath, char cReplacement)
        {
            if (string.IsNullOrEmpty(sPath))
            {
                return string.Empty;
            }

            char[] chrArray = new char[]
            {
                '/', '\\', '\t', '\n', '\v', '\f', '\r', ' ', '\u0085', '\u00A0', '\u1680', '\u180E', '\u2000',
                '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A',
                '\u2028', '\u2029', '\u202F', '\u205F', '\u3000'
            };
            sPath = sPath.Trim(chrArray);
            char[] chrArray1 = new char[] { '/', '\\' };
            string[] strArrays = sPath.Split(chrArray1);
            StringBuilder stringBuilder = new StringBuilder(sPath.Length);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                strArrays[i] = Utils.RectifyName(strArrays[i], cReplacement);
                if (string.IsNullOrEmpty(strArrays[i]))
                {
                    strArrays[i] = "folder";
                }

                stringBuilder.Append(string.Concat(strArrays[i], "/"));
            }

            return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
        }

        public static string RemoveAssociationInformationFromWorkflowName(string workflowName)
        {
            if (workflowName.Contains("<Cfg") && workflowName.IndexOf("\n", StringComparison.Ordinal) > 0)
            {
                workflowName = workflowName.Substring(0, workflowName.IndexOf("\n", StringComparison.Ordinal));
            }

            return workflowName;
        }

        public static string RemovePreviousVersionTextFromWorkflowName(string workflowName)
        {
            string str = "(Previous Version:";
            if (workflowName.LastIndexOf(str, StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                workflowName = workflowName
                    .Substring(0, workflowName.LastIndexOf(str, StringComparison.InvariantCultureIgnoreCase)).Trim();
            }

            return workflowName;
        }

        public static string ReplaceOrderByInQuery(string sCamlQuery, string[] newOrderingFields)
        {
            bool flag = false;
            if (!sCamlQuery.StartsWith("<query", StringComparison.OrdinalIgnoreCase))
            {
                flag = true;
                sCamlQuery = string.Concat("<Query>", sCamlQuery, "</Query>");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sCamlQuery);
            XmlNode documentElement = xmlDocument.DocumentElement;
            Utils.ReplaceOrderByInQuery(documentElement, newOrderingFields);
            if (flag)
            {
                return documentElement.InnerXml;
            }

            return documentElement.OuterXml;
        }

        public static void ReplaceOrderByInQuery(XmlNode query, string[] newOrderingFields)
        {
            XmlNode xmlNodes = query.SelectSingleNode("./OrderBy");
            if (xmlNodes == null)
            {
                xmlNodes = query.OwnerDocument.CreateElement("OrderBy");
                XmlAttribute xmlAttribute = query.OwnerDocument.CreateAttribute("Override");
                xmlNodes.Attributes.Append(xmlAttribute);
                xmlAttribute.Value = "TRUE";
                query.AppendChild(xmlNodes);
            }
            else
            {
                while (xmlNodes.ChildNodes.Count > 0)
                {
                    xmlNodes.RemoveChild(xmlNodes.ChildNodes[0]);
                }
            }

            if (newOrderingFields != null)
            {
                string[] strArrays = newOrderingFields;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    XmlNode xmlNodes1 = query.OwnerDocument.CreateElement("FieldRef");
                    XmlAttribute xmlAttribute1 = query.OwnerDocument.CreateAttribute("Ascending");
                    XmlAttribute xmlAttribute2 = query.OwnerDocument.CreateAttribute("Name");
                    xmlNodes1.Attributes.Append(xmlAttribute1);
                    xmlNodes1.Attributes.Append(xmlAttribute2);
                    xmlAttribute1.Value = "TRUE";
                    xmlAttribute2.Value = str;
                    xmlNodes.AppendChild(xmlNodes1);
                }
            }
        }

        public static bool ResponseIsRedirect(HttpWebResponse response)
        {
            if (response == null)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Found)
            {
                return true;
            }

            return response.StatusCode == HttpStatusCode.TemporaryRedirect;
        }

        public static string RetriveFirstValuefromColleciton(string fieldValues, string seperatorValue)
        {
            if (fieldValues.IndexOf(seperatorValue, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                string[] strArrays = new string[] { seperatorValue };
                string[] strArrays1 = fieldValues.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
                if ((int)strArrays1.Length > 0)
                {
                    return strArrays1[0];
                }
            }

            return fieldValues;
        }

        public static string SidToString(byte[] sid)
        {
            if (sid == null || (int)sid.Length == 0)
            {
                return null;
            }

            return (new SecurityIdentifier(sid, 0)).Value;
        }

        public static Guid[] SplitWebMetaInfoGuidList(string sGuidList)
        {
            string str;
            if (string.IsNullOrEmpty(sGuidList))
            {
                return new Guid[0];
            }

            string[] strArrays = sGuidList.Split(new char[] { ';' });
            List<Guid> guids = new List<Guid>();
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str1 = strArrays1[i];
                if (str1 == null)
                {
                    str = null;
                }
                else
                {
                    str = str1.Trim();
                }

                string str2 = str;
                if (!string.IsNullOrEmpty(str2))
                {
                    guids.Add(new Guid(str2));
                }
            }

            return guids.ToArray();
        }

        public static bool SwitchUserNameFormat(string sUserName, out string sSwitchedUserName)
        {
            if (string.IsNullOrEmpty(sUserName))
            {
                sSwitchedUserName = sUserName;
                return false;
            }

            if (!sUserName.Contains("|"))
            {
                sSwitchedUserName = Utils.ConvertWinOrFormsUserToClaimString(sUserName);
                return sSwitchedUserName.Contains("|");
            }

            sSwitchedUserName = Utils.ConvertClaimStringUserToWinOrFormsUser(sUserName);
            return !sSwitchedUserName.Contains("|");
        }

        public static bool TryParseDate(string sDateTime, out DateTime dtOutput)
        {
            bool flag;
            try
            {
                dtOutput = Utils.ParseDate(sDateTime);
                flag = true;
            }
            catch
            {
                dtOutput = DateTime.Now;
                flag = false;
            }

            return flag;
        }

        public static bool TryParseDate(string sDateTime, TimeZoneInformation timeZone, out DateTime dtOutput)
        {
            bool flag;
            try
            {
                dtOutput = Utils.ParseDate(sDateTime, timeZone);
                flag = true;
            }
            catch
            {
                dtOutput = DateTime.Now;
                flag = false;
            }

            return flag;
        }

        public static bool TryParseDateAsUtc(string sDateTime, out DateTime dtOutput)
        {
            bool flag;
            try
            {
                dtOutput = Utils.ParseDateAsUtc(sDateTime);
                flag = true;
            }
            catch
            {
                dtOutput = DateTime.UtcNow;
                flag = false;
            }

            return flag;
        }

        public static Type TypeFromString(string type)
        {
            string upper = type.ToUpper();
            if (upper.Equals("TEXT"))
            {
                return typeof(string);
            }

            if (upper.Equals("NOTE"))
            {
                return typeof(string);
            }

            if (upper.Equals("DATETIME"))
            {
                return typeof(DateTime);
            }

            if (upper.Equals("FILE"))
            {
                return typeof(string);
            }

            if (upper.Equals("NUMBER"))
            {
                return typeof(float);
            }

            if (upper.Equals("INTEGER"))
            {
                return typeof(int);
            }

            if (upper.Equals("URL"))
            {
                return typeof(string);
            }

            if (upper.Equals("ATTACHMENTS"))
            {
                return typeof(string);
            }

            if (upper.Equals("CHOICE"))
            {
                return typeof(string);
            }

            if (upper.Equals("LOOKUP"))
            {
                return typeof(string);
            }

            if (upper.Equals("COMPUTED"))
            {
                return typeof(string);
            }

            if (upper.Equals("IMAGE"))
            {
                return typeof(string);
            }

            return typeof(string);
        }
    }
}