using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [Serializable]
    public class FieldsLookUp : Dictionary<string, string>, IXmlSerializable
    {
        public FieldsLookUp()
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                string attribute = reader.GetAttribute("Key");
                base.Add(attribute, reader.GetAttribute("Value"));
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (string key in base.Keys)
            {
                writer.WriteStartElement("LookUp");
                writer.WriteAttributeString("Key", key);
                writer.WriteAttributeString("Value", base[key]);
                writer.WriteEndElement();
            }
        }
    }
}