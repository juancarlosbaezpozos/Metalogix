using Metalogix;
using System;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class MappableSecurityPrincipal : SecurityPrincipal
    {
        protected SecurityPrincipal m_targetPrincipal;

        public virtual SecurityPrincipal TargetPrincipal
        {
            get { return this.m_targetPrincipal; }
            set { this.m_targetPrincipal = value; }
        }

        public MappableSecurityPrincipal(XmlNode xml) : base(xml)
        {
            if (xml != null)
            {
                this.FromXML(xml);
            }
        }

        public virtual void FromXML(XmlNode xml)
        {
            XmlNode xmlNodes = xml.SelectSingleNode("//MappableSecurityPrincipal");
            if (xmlNodes != null)
            {
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./SourcePrincipal");
                this.m_XML = xmlNodes1.FirstChild.CloneNode(true);
                xmlNodes1 = xmlNodes.SelectSingleNode("./TargetPrincipal");
                if (xmlNodes1 != null)
                {
                    XmlAttribute itemOf = xmlNodes1.Attributes["Type"];
                    if (itemOf != null)
                    {
                        System.Type type = System.Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                        object[] objArray = new object[] { xmlNodes1.FirstChild.CloneNode(true) };
                        this.m_targetPrincipal = (SecurityPrincipal)Activator.CreateInstance(type, objArray);
                    }
                }
            }
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("MappableSecurityPrincipal");
            xmlWriter.WriteStartElement("SourcePrincipal");
            base.ToXML(xmlWriter);
            xmlWriter.WriteEndElement();
            if (this.m_targetPrincipal != null)
            {
                xmlWriter.WriteStartElement("TargetPrincipal");
                xmlWriter.WriteAttributeString("Type", this.m_targetPrincipal.GetType().AssemblyQualifiedName);
                xmlWriter.WriteRaw(this.m_targetPrincipal.ToXML());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}