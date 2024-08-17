using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Data.Mapping
{
    public sealed class ListSummaryItem : IXmlable
    {
        private string m_group = "";

        private ListPickerItem m_source;

        private ListPickerItem m_target;

        private CommonSerializableTable<string, object> m_customColumns;

        public CommonSerializableTable<string, object> CustomColumns
        {
            get
            {
                if (this.m_customColumns == null)
                {
                    this.m_customColumns = new CommonSerializableTable<string, object>();
                }

                return this.m_customColumns;
            }
            set { this.m_customColumns = value; }
        }

        public string Group
        {
            get { return this.m_group; }
            set { this.m_group = value; }
        }

        public ListPickerItem Source
        {
            get { return this.m_source; }
            set { this.m_source = value; }
        }

        public ListPickerItem Target
        {
            get { return this.m_target; }
            set { this.m_target = value; }
        }

        public ListSummaryItem()
        {
        }

        public ListSummaryItem(XmlNode node)
        {
            if (node != null)
            {
                this.FromXML(node);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ListSummaryItem))
            {
                return false;
            }

            ListSummaryItem listSummaryItem = (ListSummaryItem)obj;
            if (!object.Equals(this.Source, listSummaryItem.Source))
            {
                return false;
            }

            return object.Equals(this.Target, listSummaryItem.Target);
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = (string.Equals(xmlNode.Name, "ListSummaryItem")
                ? xmlNode
                : xmlNode.SelectSingleNode("//ListSummaryItem"));
            if (xmlNodes != null)
            {
                XmlAttribute itemOf = xmlNodes.Attributes["Group"];
                if (itemOf != null)
                {
                    this.Group = itemOf.Value;
                }

                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./CustomColumns");
                if (xmlNodes1 != null)
                {
                    this.CustomColumns.FromXML(xmlNodes1.FirstChild.CloneNode(true));
                }

                xmlNodes1 = xmlNodes.SelectSingleNode("./Source");
                if (xmlNodes1 != null)
                {
                    this.Source = new ListPickerItem(xmlNodes1.CloneNode(true));
                }

                xmlNodes1 = xmlNodes.SelectSingleNode("./Target");
                if (xmlNodes1 != null)
                {
                    this.Target = new ListPickerItem(xmlNodes1.CloneNode(true));
                }
            }
        }

        public override int GetHashCode()
        {
            if (this.Source == null || this.Target == null)
            {
                return this.GetHashCode();
            }

            return this.Source.GetHashCode() + this.Target.GetHashCode();
        }

        public string ToXML()
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    this.ToXML(xmlTextWriter);
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ListSummaryItem");
            xmlWriter.WriteAttributeString("Group", this.Group);
            xmlWriter.WriteStartElement("CustomColumns");
            this.CustomColumns.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
            if (this.Source != null)
            {
                xmlWriter.WriteStartElement("Source");
                this.Source.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            if (this.Target != null)
            {
                xmlWriter.WriteStartElement("Target");
                this.Target.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}