using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class ServerHealthInformation
    {
        public int? HealthScore { get; set; }

        public ServerHealthInformation()
        {
        }

        public void FromXml(XmlNode xmlNode)
        {
            XmlAttribute itemOf = xmlNode.Attributes["HealthScore"];
            if (itemOf != null)
            {
                this.HealthScore = new int?(int.Parse(itemOf.Value));
            }
        }

        public string ToXml()
        {
            StringWriter stringWriter = new StringWriter();
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSetting))
            {
                xmlWriter.WriteStartElement("ServerHealth");
                if (this.HealthScore.HasValue)
                {
                    xmlWriter.WriteAttributeString("HealthScore", this.HealthScore.Value.ToString());
                }

                xmlWriter.WriteEndElement();
            }

            return stringWriter.ToString();
        }
    }
}