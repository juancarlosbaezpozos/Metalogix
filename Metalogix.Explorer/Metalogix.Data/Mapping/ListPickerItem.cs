using Metalogix;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Data.Mapping
{
    public sealed class ListPickerItem : IXmlable
    {
        private string m_group;

        private string m_target;

        private string m_targetType;

        private object m_tag;

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

        public bool IsNew
        {
            get
            {
                if (this.Tag == null)
                {
                    return false;
                }

                return this.Tag == this;
            }
        }

        public object Tag
        {
            get { return this.m_tag; }
            set { this.m_tag = value; }
        }

        public string Target
        {
            get { return this.m_target; }
            set { this.m_target = value; }
        }

        public string TargetType
        {
            get { return this.m_targetType; }
            set { this.m_targetType = value; }
        }

        public ListPickerItem()
        {
        }

        public ListPickerItem(XmlNode node)
        {
            if (node != null)
            {
                this.FromXML(node);
            }
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("//ListPickerItem");
            if (xmlNodes != null)
            {
                this.Group = xmlNodes.Attributes["Group"].Value;
                this.Target = xmlNodes.Attributes["Target"].Value;
                this.TargetType = xmlNodes.Attributes["TargetType"].Value;
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./CustomColumns");
                if (xmlNodes1 != null)
                {
                    this.CustomColumns.FromXML(xmlNodes1.FirstChild.CloneNode(true));
                }

                xmlNodes1 = xmlNodes.SelectSingleNode("./Tag");
                if (xmlNodes1 != null)
                {
                    if (bool.Parse(xmlNodes1.Attributes["IsNew"].Value))
                    {
                        this.Tag = this;
                        return;
                    }

                    Type type = Type.GetType(TypeUtils.UpdateType(xmlNodes1.Attributes["Type"].Value));
                    if (type != null)
                    {
                        if (xmlNodes1.HasChildNodes && xmlNodes1.FirstChild.NodeType != XmlNodeType.Text)
                        {
                            object[] objArray = new object[] { xmlNodes1.FirstChild.CloneNode(true) };
                            this.Tag = Activator.CreateInstance(type, objArray);
                            return;
                        }

                        if (xmlNodes1.HasChildNodes)
                        {
                            this.Tag = (type.IsSubclassOf(typeof(Enum))
                                ? Enum.Parse(type, xmlNodes1.InnerText)
                                : Convert.ChangeType(xmlNodes1.InnerText, type));
                        }
                    }
                }
            }
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
            xmlWriter.WriteStartElement("ListPickerItem");
            xmlWriter.WriteAttributeString("Group", this.Group);
            xmlWriter.WriteAttributeString("Target", this.Target);
            xmlWriter.WriteAttributeString("TargetType", this.TargetType);
            xmlWriter.WriteStartElement("CustomColumns");
            this.CustomColumns.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
            if (this.Tag != null)
            {
                xmlWriter.WriteStartElement("Tag");
                xmlWriter.WriteAttributeString("IsNew", this.IsNew.ToString());
                if (!this.IsNew)
                {
                    xmlWriter.WriteAttributeString("Type", this.Tag.GetType().AssemblyQualifiedName);
                    if (this.Tag is IXmlable)
                    {
                        (this.Tag as IXmlable).ToXML(xmlWriter);
                    }
                    else if (this.Tag is IConvertible)
                    {
                        xmlWriter.WriteValue(Convert.ChangeType(this.Tag, typeof(string)));
                    }
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}