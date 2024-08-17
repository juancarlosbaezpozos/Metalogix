using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Metalogix.Utilities
{
    public static class XmlUtility
    {
        public static XmlWriterSettings WriterSettings;

        static XmlUtility()
        {
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true,
                Encoding = Encoding.UTF8
            };
            XmlUtility.WriterSettings = xmlWriterSetting;
        }

        public static string AddLanguageSettingsAttribute(string xml, string objectType, string parentObject = null)
        {
            string str = string.Concat("/", objectType, "/LanguageResources");
            XmlNode xmlNode = XmlUtility.StringToXmlNode(xml);
            if (xmlNode == null)
            {
                return xml;
            }

            XmlNodeList xmlNodeLists = xmlNode.SelectNodes(str);
            if (xmlNodeLists.Count == 0 && !string.IsNullOrEmpty(parentObject))
            {
                xmlNodeLists = xmlNode.SelectNodes(string.Concat("/", parentObject, "/LanguageResources"));
            }

            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                if (xmlNodes == null)
                {
                    continue;
                }

                XmlAttribute xmlAttribute = xmlNodes.OwnerDocument.CreateAttribute("MigrateLanguageSettings");
                xmlAttribute.Value = true.ToString();
                xmlNodes.Attributes.Append(xmlAttribute);
            }

            return xmlNode.OuterXml;
        }

        public static XmlNode CloneXMLNodeForTarget(XmlNode sourceNode, XmlNode targetNode,
            bool includeAttributeNamespace = true)
        {
            XmlNode xmlNodes = targetNode.OwnerDocument.CreateElement(sourceNode.Name);
            foreach (XmlAttribute attribute in sourceNode.Attributes)
            {
                XmlAttribute value = null;
                value = (!includeAttributeNamespace
                    ? targetNode.OwnerDocument.CreateAttribute(attribute.Name)
                    : targetNode.OwnerDocument.CreateAttribute(attribute.Name, attribute.NamespaceURI));
                value.Value = attribute.Value;
                xmlNodes.Attributes.Append(value);
            }

            foreach (XmlNode childNode in sourceNode.ChildNodes)
            {
                if (childNode is XmlText)
                {
                    xmlNodes.AppendChild(targetNode.OwnerDocument.CreateTextNode(childNode.Value));
                }
                else if (childNode is XmlCDataSection)
                {
                    xmlNodes.AppendChild(targetNode.OwnerDocument.CreateCDataSection(childNode.Value));
                }
                else if (!(childNode is XmlComment))
                {
                    XmlUtility.CloneXMLNodeInto(childNode, xmlNodes, includeAttributeNamespace);
                }
                else
                {
                    xmlNodes.AppendChild(targetNode.OwnerDocument.CreateComment(childNode.Value));
                }
            }

            return xmlNodes;
        }

        public static void CloneXMLNodeInto(XmlNode sourceNode, XmlNode targetNode,
            bool includeAttributeNamespace = true)
        {
            XmlNode xmlNodes = XmlUtility.CloneXMLNodeForTarget(sourceNode, targetNode, includeAttributeNamespace);
            targetNode.AppendChild(xmlNodes);
        }

        private static void ConstructPreQueryObjects(XmlNode XmlNodeToQuery, out XmlDocument document,
            out XmlNamespaceManager nameSpaceMngr)
        {
            document = new XmlDocument();
            document.LoadXml(XmlNodeToQuery.OuterXml);
            nameSpaceMngr = new XmlNamespaceManager(document.NameTable);
            nameSpaceMngr.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/soap/");
            nameSpaceMngr.AddNamespace("z", "#RowsetSchema");
            nameSpaceMngr.AddNamespace("y", "http://schemas.microsoft.com/sharepoint/soap/ois/");
            nameSpaceMngr.AddNamespace("w", "http://schemas.microsoft.com/WebPart/v2");
            nameSpaceMngr.AddNamespace("d", "http://schemas.microsoft.com/sharepoint/soap/directory/");
        }

        public static bool ElementsAreEquivalent(XmlElement sourceNode, XmlElement targetNode,
            bool compareInnerXml = true, IList<string> attributesToIgnore = null)
        {
            bool flag;
            IEnumerator enumerator = sourceNode.Attributes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlAttribute current = (XmlAttribute)enumerator.Current;
                    if (attributesToIgnore != null && attributesToIgnore.Contains(current.Name))
                    {
                        continue;
                    }

                    XmlAttribute itemOf = targetNode.Attributes[current.Name];
                    if (itemOf != null && !(current.Value != itemOf.Value))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                if (compareInnerXml && sourceNode.InnerXml != targetNode.InnerXml)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public static string EncodeIllegalCharactersInXmlAttributeValue(string xmlAttributeValue)
        {
            if (xmlAttributeValue == null)
            {
                throw new ArgumentException("cannot be null", "xmlAttributeValue");
            }

            xmlAttributeValue = xmlAttributeValue.Replace("'", "&apos;").Replace("\"", "&quot;").Replace("&", "&amp;")
                .Replace("<", "&lt;").Replace(">", "&gt;");
            return xmlAttributeValue;
        }

        public static string EncodeNameStartChars(string sName)
        {
            if (sName.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '-' }) == 0)
            {
                sName = string.Concat(XmlConvert.EncodeName(sName.Substring(0, 1)), sName.Substring(1));
            }

            if (Regex.IsMatch(sName, "\\s"))
            {
                sName = XmlConvert.EncodeName(sName);
            }

            return sName;
        }

        public static XmlNode FindOrCreateNode(string sXPathQuery, XmlNode node)
        {
            XmlNode xmlNodes;
            XmlNode xmlNodes1 = node.SelectSingleNode(sXPathQuery);
            string str = sXPathQuery;
            string str1 = null;
            while (xmlNodes1 == null)
            {
                int num = str.LastIndexOf('/');
                if (num < 0)
                {
                    return null;
                }

                str = str.Substring(0, num);
                xmlNodes1 = node.SelectSingleNode(str);
                if (xmlNodes1 == null)
                {
                    continue;
                }

                str1 = sXPathQuery.Substring(num).Trim(new char[] { '/' });
            }

            char[] chrArray = new char[] { '=' };
            char[] chrArray1 = new char[] { '\'', '=' };
            while (!string.IsNullOrEmpty(str1))
            {
                string str2 = null;
                int num1 = str1.IndexOf('/');
                if (num1 < 0)
                {
                    str2 = str1;
                    str1 = null;
                }
                else
                {
                    str2 = str1.Substring(0, num1);
                    str1 = str1.Substring(num1 + 1);
                }

                XmlNode xmlNodes2 = null;
                if (!str2.StartsWith("@", StringComparison.Ordinal))
                {
                    XmlAttribute xmlAttribute = null;
                    int num2 = str2.IndexOf("[@", StringComparison.Ordinal);
                    if (num2 >= 0)
                    {
                        try
                        {
                            int num3 = str2.IndexOf("]", num2, StringComparison.Ordinal);
                            string str3 = str2.Substring(num2 + 2, num3 - (num2 + 2));
                            string[] strArrays = str3.Split(chrArray);
                            xmlAttribute = xmlNodes1.OwnerDocument.CreateAttribute(strArrays[0]);
                            if ((int)strArrays.Length == 2)
                            {
                                xmlAttribute.Value = strArrays[1].Trim(chrArray1);
                            }

                            str2 = str2.Substring(0, num2);
                            goto Label0;
                        }
                        catch
                        {
                            xmlNodes = null;
                        }

                        return xmlNodes;
                    }

                    Label0:
                    xmlNodes2 = xmlNodes1.OwnerDocument.CreateElement(str2);
                    if (xmlAttribute != null)
                    {
                        xmlNodes2.Attributes.Append(xmlAttribute);
                    }

                    xmlNodes1.AppendChild(xmlNodes2);
                }
                else
                {
                    xmlNodes2 = xmlNodes1.OwnerDocument.CreateAttribute(str2.Substring(1));
                    xmlNodes1.Attributes.Append(xmlNodes2 as XmlAttribute);
                }

                xmlNodes1 = xmlNodes2;
            }

            return xmlNodes1;
        }

        public static XmlAttribute GetAttribute(XmlNode node, string sSubNodePath, string sAttributeName,
            bool bCaseSensitive)
        {
            XmlAttribute xmlAttribute;
            if (!bCaseSensitive)
            {
                sAttributeName = sAttributeName.ToLower();
            }

            sAttributeName = XmlUtility.EncodeNameStartChars(sAttributeName);
            XmlNode xmlNodes = node;
            if (!string.IsNullOrEmpty(sSubNodePath))
            {
                if (!bCaseSensitive)
                {
                    sSubNodePath = sSubNodePath.ToLower();
                }

                char[] chrArray = new char[] { '/' };
                string[] strArrays = sSubNodePath.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
                int num = 0;
                while (num < (int)strArrays.Length)
                {
                    string str = strArrays[num];
                    bool flag = false;
                    foreach (XmlNode childNode in xmlNodes.ChildNodes)
                    {
                        if ((bCaseSensitive ? childNode.Name : childNode.Name.ToLower()) != str)
                        {
                            continue;
                        }

                        xmlNodes = childNode;
                        flag = true;
                        break;
                    }

                    if (flag)
                    {
                        num++;
                    }
                    else
                    {
                        xmlAttribute = null;
                        return xmlAttribute;
                    }
                }
            }

            IEnumerator enumerator = xmlNodes.Attributes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlAttribute current = (XmlAttribute)enumerator.Current;
                    if ((bCaseSensitive ? current.Name : current.Name.ToLower()) != sAttributeName)
                    {
                        continue;
                    }

                    xmlAttribute = current;
                    return xmlAttribute;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return xmlAttribute;
        }

        public static string GetAttributeValue(XmlNode node, string sAttributeName)
        {
            XmlAttribute itemOf = node.Attributes[sAttributeName];
            if (itemOf == null)
            {
                return null;
            }

            return itemOf.Value;
        }

        public static string GetAttributeValueFromDocument(XmlDocument xDoc, string sXPathQuery)
        {
            XmlNode xmlNodes = xDoc.SelectSingleNode(sXPathQuery);
            if (xmlNodes == null)
            {
                return "";
            }

            return xmlNodes.Value.ToString();
        }

        public static bool GetBooleanAttributeFromXml(XmlNode xml, string sAttributeName, out bool bValue)
        {
            bValue = false;
            if (xml == null)
            {
                return false;
            }

            return XmlUtility.GetBooleanAttributeFromXml(xml.Attributes, sAttributeName, out bValue);
        }

        public static bool GetBooleanAttributeFromXml(XmlAttributeCollection attributes, string sAttributeName,
            out bool bValue)
        {
            bValue = false;
            if (attributes != null && attributes[sAttributeName] != null &&
                bool.TryParse(attributes[sAttributeName].Value, out bValue))
            {
                return true;
            }

            return false;
        }

        public static bool GetIntegerAttributeFromXml(XmlNode xml, string sAttributeName, out int iValue)
        {
            iValue = -1;
            if (xml != null && xml.Attributes[sAttributeName] != null &&
                int.TryParse(xml.Attributes[sAttributeName].Value, out iValue))
            {
                return true;
            }

            return false;
        }

        public static bool IsFieldSiteColumn(XmlNode fieldNode)
        {
            if (fieldNode.Attributes != null && fieldNode.Attributes["IsListColumn"] != null)
            {
                return !fieldNode.GetAttributeValueAsBoolean("IsListColumn");
            }

            return fieldNode.GetAttributeValueAsString("SourceID")
                .StartsWith("http://", StringComparison.OrdinalIgnoreCase);
        }

        public static void MatchAttributeState(XmlNode nodeToModify, XmlNode nodeToMatch, string sAttributeName)
        {
            XmlAttribute itemOf = nodeToMatch.Attributes[sAttributeName];
            XmlAttribute value = nodeToModify.Attributes[sAttributeName];
            if (itemOf == null)
            {
                if (value != null)
                {
                    nodeToModify.Attributes.Remove(value);
                }

                return;
            }

            if (value == null)
            {
                value = nodeToModify.OwnerDocument.CreateAttribute(sAttributeName);
                nodeToModify.Attributes.Append(value);
            }

            value.Value = itemOf.Value;
        }

        public static XmlNode MatchFirstAttributeValue(string sAttributeName, string sAttributeValue,
            XmlNodeList nodesToCheck)
        {
            XmlNode xmlNodes;
            IEnumerator enumerator = nodesToCheck.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    if (current.Attributes[sAttributeName] == null ||
                        !(current.Attributes[sAttributeName].Value == sAttributeValue))
                    {
                        continue;
                    }

                    xmlNodes = current;
                    return xmlNodes;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return xmlNodes;
        }

        public static string MergeTargetFieldSchemaXml(string targetFieldSchemaXml, string sourceFieldSchemaXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(targetFieldSchemaXml);
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(sourceFieldSchemaXml);
            return XmlUtility.MergeTargetFieldSchemaXml(xmlDocument.DocumentElement, xmlDocument1.DocumentElement)
                .OuterXml;
        }

        public static XmlNode MergeTargetFieldSchemaXml(XmlNode targetFieldSchemaXml, XmlNode sourceFieldSchemaXml)
        {
            XmlAttribute itemOf = sourceFieldSchemaXml.Attributes["Type"];
            XmlAttribute xmlAttribute = targetFieldSchemaXml.Attributes["Type"];
            if (!string.Equals(itemOf.Value, xmlAttribute.Value, StringComparison.OrdinalIgnoreCase))
            {
                return sourceFieldSchemaXml;
            }

            foreach (XmlAttribute attribute in targetFieldSchemaXml.Attributes)
            {
                if (sourceFieldSchemaXml.Attributes[attribute.Name] != null)
                {
                    continue;
                }

                XmlAttribute value = sourceFieldSchemaXml.OwnerDocument.CreateAttribute(attribute.Name);
                value.Value = attribute.Value;
                sourceFieldSchemaXml.Attributes.Append(value);
            }

            XmlAttribute itemOf1 = sourceFieldSchemaXml.Attributes["ID"];
            if (itemOf1 != null)
            {
                XmlAttribute xmlAttribute1 = targetFieldSchemaXml.Attributes["ID"];
                if (xmlAttribute1 == null)
                {
                    sourceFieldSchemaXml.Attributes.Remove(itemOf1);
                }
                else
                {
                    itemOf1.Value = xmlAttribute1.Value;
                }
            }

            XmlNodeList xmlNodeLists = targetFieldSchemaXml.SelectNodes("Customization/ArrayOfProperty/Property/Name");
            if (xmlNodeLists.Count > 0)
            {
                XmlNode xmlNodes = sourceFieldSchemaXml.SelectSingleNode("Customization/ArrayOfProperty");
                XmlNodeList xmlNodeLists1 = xmlNodes.SelectNodes("./Property/Name");
                foreach (XmlNode xmlNodes1 in xmlNodeLists)
                {
                    XmlNode parentNode = null;
                    foreach (XmlNode xmlNodes2 in xmlNodeLists1)
                    {
                        if (!xmlNodes2.InnerText.Equals(xmlNodes1.InnerText, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        parentNode = xmlNodes2.ParentNode;
                    }

                    if (parentNode != null)
                    {
                        continue;
                    }

                    XmlNode innerXml = sourceFieldSchemaXml.OwnerDocument.CreateElement(xmlNodes1.ParentNode.Name);
                    innerXml.InnerXml = xmlNodes1.ParentNode.InnerXml;
                    xmlNodes.AppendChild(innerXml);
                }
            }

            return sourceFieldSchemaXml;
        }

        public static string RemoveIllegalCharactersFromXmlAttributeValue(string xmlAttributeValue)
        {
            if (xmlAttributeValue == null)
            {
                throw new ArgumentException("cannot be null", "xmlAttributeValue");
            }

            xmlAttributeValue = xmlAttributeValue.Replace("'", "").Replace("\"", "").Replace("&", "").Replace("<", "")
                .Replace(">", "");
            return xmlAttributeValue;
        }

        public static XmlNodeList RunXPathQuery(XmlNode xmlNodeToQuery, string sXPathQuery)
        {
            XmlDocument xmlDocument;
            XmlNamespaceManager xmlNamespaceManagers;
            XmlUtility.ConstructPreQueryObjects(xmlNodeToQuery, out xmlDocument, out xmlNamespaceManagers);
            return xmlDocument.SelectNodes(sXPathQuery, xmlNamespaceManagers);
        }

        public static XmlNode RunXPathQuerySelectSingle(XmlNode xmlNodeToQuery, string sXPathQuery)
        {
            XmlDocument xmlDocument;
            XmlNamespaceManager xmlNamespaceManagers;
            XmlUtility.ConstructPreQueryObjects(xmlNodeToQuery, out xmlDocument, out xmlNamespaceManagers);
            return xmlDocument.SelectSingleNode(sXPathQuery, xmlNamespaceManagers);
        }

        public static XPathNodeIterator SortXmlNodes(string xml, string xpathToSortByAttribute,
            string pathToNodesToSort, XmlSortOrder order)
        {
            XPathNavigator xPathNavigator = (new XPathDocument(new StringReader(xml))).CreateNavigator();
            XPathExpression xPathExpression = xPathNavigator.Compile(xpathToSortByAttribute);
            xPathExpression.AddSort(pathToNodesToSort, order, XmlCaseOrder.None, "", XmlDataType.Text);
            return xPathNavigator.Select(xPathExpression);
        }

        public static XmlNode StringToXmlNode(string sXml)
        {
            XmlNode firstChild;
            if (string.IsNullOrEmpty(sXml))
            {
                return null;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sXml);
                firstChild = xmlDocument.FirstChild;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                throw new Exception(
                    string.Concat("Converting an XML string to node failed. The input string is:\n '", sXml, "'."),
                    exception.InnerException);
            }

            return firstChild;
        }
    }
}