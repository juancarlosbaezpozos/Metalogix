using Metalogix.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.Metabase.Options
{
    public class ExportToCSVOptions : ActionOptions
    {
        private readonly List<string> m_listColumnsToExport = new List<string>();

        private bool m_bIncludeTypes;

        private string m_sTargetFilename;

        public List<string> ColumnsToExport
        {
            get { return this.m_listColumnsToExport; }
        }

        public bool IncludeTypes
        {
            get { return this.m_bIncludeTypes; }
            set { this.m_bIncludeTypes = value; }
        }

        public string TargetFilename
        {
            get { return this.m_sTargetFilename; }
            set { this.m_sTargetFilename = value; }
        }

        public ExportToCSVOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            bool flag;
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = xmlNode.SelectSingleNode("//ExportToCSVOptions");
            if (xmlNodes == null)
            {
                return;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("//ColumnsToExport");
            if (xmlNodes1 != null)
            {
                this.m_listColumnsToExport.Clear();
                foreach (XmlNode xmlNodes2 in xmlNodes1.SelectNodes("./Column"))
                {
                    if (xmlNodes2 == null)
                    {
                        continue;
                    }

                    XmlAttribute itemOf = xmlNodes2.Attributes["Name"];
                    if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
                    {
                        continue;
                    }

                    this.m_listColumnsToExport.Add(itemOf.Value);
                }
            }

            XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("//TargetFilename");
            if (xmlNodes3 != null)
            {
                this.m_sTargetFilename = xmlNodes3.InnerText;
            }

            XmlNode xmlNodes4 = xmlNodes.SelectSingleNode("//IncludeTypes");
            if (xmlNodes4 != null && bool.TryParse(xmlNodes4.InnerText, out flag))
            {
                this.m_bIncludeTypes = flag;
            }
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("ExportToCSVOptions");
            if (this.m_listColumnsToExport != null)
            {
                xmlTextWriter.WriteStartElement("ColumnsToExport");
                foreach (string mListColumnsToExport in this.m_listColumnsToExport)
                {
                    xmlTextWriter.WriteStartElement("Column");
                    xmlTextWriter.WriteAttributeString("Name", mListColumnsToExport);
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteElementString("TargetFilename", this.m_sTargetFilename);
            xmlTextWriter.WriteElementString("IncludeTypes", this.m_bIncludeTypes.ToString());
            xmlTextWriter.WriteEndElement();
        }

        private static class XmlNames
        {
            public const string OPTIONS = "ExportToCSVOptions";

            public const string COLUMNS_TO_EXPORT = "ColumnsToExport";

            public const string COLUMN = "Column";

            public const string NAME = "Name";

            public const string TARGET_FILENAME = "TargetFilename";

            public const string INCLUDE_TYPES = "IncludeTypes";
        }
    }
}