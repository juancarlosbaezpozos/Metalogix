using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace Metalogix.DataStructures
{
    public sealed class StringCollectionMRU : SerializableList<string>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override string this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public StringCollectionMRU()
        {
        }

        public StringCollectionMRU(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
                this.FromXML(xmlNode);
            }
        }

        public void AddToFront(string sItem)
        {
            if (base.Contains(sItem))
            {
                base.RemoveFromCollection(sItem);
            }

            base.Insert(0, sItem);
        }

        public override void FromXML(XmlNode xmlNode)
        {
            if ((xmlNode.Name == "XmlableSet" ? xmlNode : xmlNode.SelectSingleNode(".//XmlableSet")) != null)
            {
                base.FromXML(xmlNode);
                return;
            }

            foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//Item"))
            {
                string str = null;
                str = (xmlNodes.Attributes["Value"] == null
                    ? xmlNodes.SelectSingleNode("./Value").InnerXml
                    : xmlNodes.Attributes["Value"].Value);
                base.AddToCollection(str);
            }
        }

        public override string ToString()
        {
            if (base.Count <= 0)
            {
                return string.Empty;
            }

            return this[0].ToString();
        }
    }
}