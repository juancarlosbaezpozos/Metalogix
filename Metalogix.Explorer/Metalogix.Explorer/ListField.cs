using Metalogix;
using Metalogix.Data;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Explorer
{
    public class ListField : TypedField, Field, IXmlable
    {
        private string m_sName;

        private string m_sDisplayName;

        private Type m_type;

        public string DisplayName
        {
            get { return JustDecompileGenerated_get_DisplayName(); }
            set { JustDecompileGenerated_set_DisplayName(value); }
        }

        public string JustDecompileGenerated_get_DisplayName()
        {
            return this.m_sDisplayName;
        }

        private void JustDecompileGenerated_set_DisplayName(string value)
        {
            this.m_sDisplayName = value;
        }

        public string Name
        {
            get { return JustDecompileGenerated_get_Name(); }
            set { JustDecompileGenerated_set_Name(value); }
        }

        public string JustDecompileGenerated_get_Name()
        {
            return this.m_sName;
        }

        private void JustDecompileGenerated_set_Name(string value)
        {
            this.m_sName = value;
        }

        public Type UnderlyingType
        {
            get { return JustDecompileGenerated_get_UnderlyingType(); }
            set { JustDecompileGenerated_set_UnderlyingType(value); }
        }

        public Type JustDecompileGenerated_get_UnderlyingType()
        {
            return this.m_type;
        }

        private void JustDecompileGenerated_set_UnderlyingType(Type value)
        {
            this.m_type = value;
        }

        public ListField(string sName, string sDisplayName, Type type)
        {
            this.m_sName = sName;
            this.m_sDisplayName = sDisplayName;
            this.m_type = type;
        }

        public ListField(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
                this.FromXML(xmlNode);
            }
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = (xmlNode.Name == "ListField" ? xmlNode : xmlNode.SelectSingleNode(".//ListField"));
            if (xmlNodes != null)
            {
                XmlAttribute itemOf = xmlNodes.Attributes["DisplayName"];
                if (itemOf != null)
                {
                    this.DisplayName = itemOf.Value;
                }

                itemOf = xmlNodes.Attributes["Name"];
                if (itemOf != null)
                {
                    this.Name = itemOf.Value;
                }

                itemOf = xmlNodes.Attributes["UnderlyingType"];
                if (itemOf != null)
                {
                    this.UnderlyingType = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                }
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.DisplayName))
            {
                return this.DisplayName;
            }

            return this.Name;
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
            xmlWriter.WriteStartElement("ListField");
            xmlWriter.WriteAttributeString("DisplayName", this.DisplayName);
            xmlWriter.WriteAttributeString("Name", this.Name);
            if (this.UnderlyingType != null)
            {
                xmlWriter.WriteAttributeString("UnderlyingType", this.UnderlyingType.AssemblyQualifiedName);
            }

            xmlWriter.WriteEndElement();
        }
    }
}