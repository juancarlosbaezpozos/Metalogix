using Metalogix.Data;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.DataStructures
{
    public class ComparisonOptions : IXmlable
    {
        private CompareLevel m_level = CompareLevel.Moderate;

        public CompareLevel Level
        {
            get { return this.m_level; }
            set { this.m_level = value; }
        }

        public ComparisonOptions()
        {
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlAttribute itemOf = xmlNode.SelectSingleNode(".//ComparisonOptions").Attributes["ComparisonLevel"];
            if (itemOf != null)
            {
                this.m_level = (CompareLevel)Enum.Parse(typeof(CompareLevel), itemOf.Value);
            }
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ComparisonOptions");
            xmlWriter.WriteAttributeString("ComparisonLevel", this.Level.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}