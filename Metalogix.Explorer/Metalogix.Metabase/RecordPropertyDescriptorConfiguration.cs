using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase
{
    public class RecordPropertyDescriptorConfiguration : IXmlableV3
    {
        private Dictionary<string, string> m_keyValueIndex = new Dictionary<string, string>();

        public string this[string sKey]
        {
            get
            {
                if (!this.m_keyValueIndex.ContainsKey(sKey))
                {
                    return null;
                }

                return this.m_keyValueIndex[sKey];
            }
            set
            {
                if (!RecordPropertyDescriptorConfiguration.IsAlphaNumeric(sKey))
                {
                    throw new ArgumentException("Keys must be alpha-numeric");
                }

                if (value != null)
                {
                    if (this.m_keyValueIndex.ContainsKey(sKey))
                    {
                        this.m_keyValueIndex[sKey] = value;
                        return;
                    }

                    this.m_keyValueIndex.Add(sKey, value);
                }
                else if (this.m_keyValueIndex.ContainsKey(sKey))
                {
                    this.m_keyValueIndex.Remove(sKey);
                    return;
                }
            }
        }

        public IEnumerable<string> Keys
        {
            get { return this.m_keyValueIndex.Keys; }
        }

        public RecordPropertyDescriptorConfiguration()
        {
        }

        public RecordPropertyDescriptorConfiguration(XmlNode nodeConfiguration)
        {
            this.FromXml(nodeConfiguration);
        }

        public void FromXml(string sXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXml(xmlDocument.DocumentElement);
        }

        public void FromXml(XmlNode node)
        {
            this.m_keyValueIndex.Clear();
            if (node == null)
            {
                return;
            }

            XmlNode xmlNodes = node.SelectSingleNode("./Configuration");
            if (xmlNodes == null)
            {
                return;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./Properties");
            if (xmlNodes1 != null)
            {
                foreach (XmlNode childNode in xmlNodes1.ChildNodes)
                {
                    if (childNode == null)
                    {
                        continue;
                    }

                    string localName = childNode.LocalName;
                    string innerText = childNode.InnerText;
                    if (this.m_keyValueIndex.ContainsKey(localName))
                    {
                        continue;
                    }

                    this.m_keyValueIndex.Add(localName, innerText);
                }
            }
        }

        private static bool IsAlphaNumeric(string stringToCheck)
        {
            string str = stringToCheck;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsEqual(IXmlableV3 xmlable)
        {
            return object.ReferenceEquals(this, xmlable);
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("RecordPropertyDescriptorConfiguration");
            this.ToXml(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public void ToXml(XmlWriter xWriter)
        {
            xWriter.WriteStartElement("Configuration");
            xWriter.WriteStartElement("Properties");
            foreach (string key in this.m_keyValueIndex.Keys)
            {
                string item = this.m_keyValueIndex[key];
                if (item == null)
                {
                    continue;
                }

                xWriter.WriteStartElement(key);
                xWriter.WriteString(item);
                xWriter.WriteEndElement();
            }

            xWriter.WriteEndElement();
            xWriter.WriteEndElement();
        }

        private class XmlNames
        {
            public const string RECORD_PROP_CONFIG = "RecordPropertyDescriptorConfiguration";

            public const string CONFIGURATION = "Configuration";

            public const string PROPERTIES = "Properties";

            public XmlNames()
            {
            }
        }
    }
}