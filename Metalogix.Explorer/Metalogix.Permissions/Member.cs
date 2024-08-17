using Metalogix.Data;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class Member : IXmlable
    {
        protected XmlNode m_XML;

        public virtual string XML
        {
            get
            {
                if (this.m_XML == null)
                {
                    return "";
                }

                return this.m_XML.OuterXml;
            }
        }

        public Member(XmlNode xml)
        {
            this.m_XML = xml;
        }

        public virtual XmlNode GetNode()
        {
            return this.m_XML;
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

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            if (this.m_XML != null)
            {
                xmlWriter.WriteRaw(this.m_XML.OuterXml);
            }
        }
    }
}