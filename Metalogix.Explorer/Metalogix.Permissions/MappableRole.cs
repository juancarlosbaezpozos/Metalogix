using Metalogix;
using System;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class MappableRole : Role
    {
        protected Role m_targetRole;

        public virtual Role TargetRole
        {
            get { return this.m_targetRole; }
            set { this.m_targetRole = value; }
        }

        public MappableRole()
        {
        }

        public MappableRole(XmlNode xml) : base(xml)
        {
            if (xml != null)
            {
                this.FromXML(xml);
            }
        }

        public virtual void FromXML(XmlNode xml)
        {
            XmlNode xmlNodes = xml.SelectSingleNode("//MappableRole");
            if (xmlNodes != null)
            {
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./SourceRole");
                this.m_XML = xmlNodes1.FirstChild.CloneNode(true);
                xmlNodes1 = xmlNodes.SelectSingleNode("./TargetRole");
                if (xmlNodes1 != null)
                {
                    XmlAttribute itemOf = xmlNodes1.Attributes["Type"];
                    if (itemOf != null)
                    {
                        Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                        object[] objArray = new object[] { xmlNodes1.FirstChild.CloneNode(true) };
                        this.m_targetRole = (Role)Activator.CreateInstance(type, objArray);
                    }
                }
            }
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("MappableRole");
            xmlWriter.WriteStartElement("SourceRole");
            base.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
            if (this.m_targetRole != null)
            {
                xmlWriter.WriteStartElement("TargetRole");
                xmlWriter.WriteAttributeString("Type", this.m_targetRole.GetType().AssemblyQualifiedName);
                xmlWriter.WriteRaw(this.m_targetRole.ToXML());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}