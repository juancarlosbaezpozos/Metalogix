using Metalogix.Metabase.DataTypes;
using Metalogix.Utilities;
using Microsoft.XmlDiffPatch;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase
{
    public class MetabaseUtility
    {
        private static XmlDiffOptions s_defaultXmlDiffOptions;

        static MetabaseUtility()
        {
            MetabaseUtility.s_defaultXmlDiffOptions = XmlDiffOptions.IgnoreChildOrder | XmlDiffOptions.IgnoreComments |
                                                      XmlDiffOptions.IgnorePI | XmlDiffOptions.IgnoreNamespaces |
                                                      XmlDiffOptions.IgnoreXmlDecl | XmlDiffOptions.IgnoreDtd;
        }

        public MetabaseUtility()
        {
        }

        public static string GetXMLDiff(XmlNode sourceNode, XmlNode targetNode)
        {
            return MetabaseUtility.GetXMLDiff(sourceNode, targetNode, MetabaseUtility.s_defaultXmlDiffOptions);
        }

        public static string GetXMLDiff(XmlNode sourceNode, XmlNode targetNode, XmlDiffOptions options)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            MetabaseUtility.GetXMLDiff(sourceNode, targetNode, options, xmlTextWriter);
            return stringBuilder.ToString();
        }

        public static bool GetXMLDiff(XmlNode sourceNode, XmlNode targetNode, XmlWriter output)
        {
            return MetabaseUtility.GetXMLDiff(sourceNode, targetNode, MetabaseUtility.s_defaultXmlDiffOptions, output);
        }

        public static bool GetXMLDiff(XmlNode sourceNode, XmlNode targetNode, XmlDiffOptions options, XmlWriter output)
        {
            XmlDiff xmlDiff = new XmlDiff(options)
            {
                Algorithm = XmlDiffAlgorithm.Fast
            };
            return xmlDiff.Compare(sourceNode, targetNode, output);
        }

        public static XmlNode PatchNode(XmlNode sourceNode, string sDiffGram)
        {
            return MetabaseUtility.PatchNode(sourceNode, new XmlTextReader(new StringReader(sDiffGram)));
        }

        public static XmlNode PatchNode(XmlNode sourceNode, XmlReader diffGram)
        {
            (new XmlPatch()).Patch(ref sourceNode, diffGram);
            return sourceNode;
        }

        public static XmlNode ProcessEditScript(XmlNode sourceNode, Record record, string sPropertyName)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceNode.OuterXml);
            try
            {
                RecordPropertyValue item = record.Properties[sPropertyName];
                if (item != null)
                {
                    TextMoniker value = item.Value as TextMoniker;
                    if (value != null)
                    {
                        string fullText = value.GetFullText();
                        if (!string.IsNullOrEmpty(fullText))
                        {
                            foreach (XmlNode xmlNodes in XmlUtility.StringToXmlNode(fullText)
                                         .SelectNodes(".//EditScript"))
                            {
                                xmlNode = MetabaseUtility.PatchNode(xmlNode, xmlNodes.InnerText);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
            }

            return xmlNode;
        }

        public static void SaveXMLDiffAsProperty(XmlNode sourceNode, XmlNode targetNode, Record record,
            string sPropertyName, bool bReplaceChanges)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!MetabaseUtility.GetXMLDiff(sourceNode, targetNode, new XmlTextWriter(new StringWriter(stringBuilder))))
            {
                PropertyDescriptor propertyDescriptor =
                    record.ParentWorkspace.EnsureProperty(sPropertyName, typeof(TextMoniker), "EditScripts");
                string outerXml = null;
                TextMoniker value = propertyDescriptor.GetValue(record) as TextMoniker;
                string fullText = null;
                if (!bReplaceChanges)
                {
                    fullText = value.GetFullText();
                    if (string.IsNullOrEmpty(fullText))
                    {
                        fullText = "<EditScriptCollection></EditScriptCollection>";
                    }
                }
                else
                {
                    fullText = "<EditScriptCollection></EditScriptCollection>";
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(fullText);
                XmlNode xmlNodes = xmlDocument.SelectSingleNode("//EditScriptCollection");
                if (xmlNodes != null)
                {
                    XmlElement str = xmlDocument.CreateElement("EditScript");
                    str.InnerText = stringBuilder.ToString();
                    xmlNodes.AppendChild(str);
                    outerXml = xmlDocument.OuterXml;
                }

                value.SetFullText(outerXml);
            }
        }
    }
}