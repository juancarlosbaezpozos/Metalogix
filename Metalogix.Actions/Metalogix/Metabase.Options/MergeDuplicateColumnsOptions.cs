using Metalogix.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.Metabase.Options
{
    public class MergeDuplicateColumnsOptions : ActionOptions
    {
        private List<string> m_listColNames = new List<string>();

        private string m_sColumnOutName = string.Empty;

        private string m_sSeparator = string.Empty;

        public string ColumnOut
        {
            get { return this.m_sColumnOutName; }
            set { this.m_sColumnOutName = value; }
        }

        public List<string> ColumnsIn
        {
            get { return this.m_listColNames; }
        }

        public string Separator
        {
            get { return this.m_sSeparator; }
            set { this.m_sSeparator = value; }
        }

        public MergeDuplicateColumnsOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = xmlNode.SelectSingleNode("./CopyColumnsOptions");
            if (xmlNodes == null)
            {
                return;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./ColumnsIn");
            if (xmlNodes1 != null)
            {
                foreach (XmlNode childNode in xmlNodes1.ChildNodes)
                {
                    if (childNode == null)
                    {
                        continue;
                    }

                    XmlAttribute itemOf = childNode.Attributes["Name"];
                    if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
                    {
                        continue;
                    }

                    this.m_listColNames.Add(itemOf.Value);
                }
            }

            XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./ColumnOut");
            if (xmlNodes2 != null)
            {
                XmlAttribute xmlAttribute = xmlNodes2.Attributes["Name"];
                if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value))
                {
                    this.m_sColumnOutName = xmlAttribute.Value;
                }
            }

            XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./Separator");
            if (xmlNodes3 != null)
            {
                XmlAttribute itemOf1 = xmlNodes3.Attributes["Name"];
                if (itemOf1 != null && !string.IsNullOrEmpty(itemOf1.Value))
                {
                    this.m_sSeparator = itemOf1.Value;
                }
            }
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("CopyColumnsOptions");
            xmlTextWriter.WriteStartElement("ColumnsIn");
            foreach (string mListColName in this.m_listColNames)
            {
                xmlTextWriter.WriteStartElement("Column");
                xmlTextWriter.WriteAttributeString("Name", mListColName);
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("ColumnOut");
            xmlTextWriter.WriteAttributeString("Name", this.m_sColumnOutName);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("Separator");
            xmlTextWriter.WriteAttributeString("Name", this.m_sSeparator);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
        }

        private struct XmlNames
        {
            public const string OPTIONS = "CopyColumnsOptions";

            public const string COLUMNSIN = "ColumnsIn";

            public const string COLUMN = "Column";

            public const string NAME = "Name";

            public const string COLUMNOUT = "ColumnOut";

            public const string SEPARATOR = "Separator";
        }
    }
}