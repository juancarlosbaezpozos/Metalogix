using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Licensing.Storage
{
    internal class ValueCollection
    {
        private readonly string _rawValue;

        private bool _isDirty;

        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public bool IsDirty
        {
            get { return this._isDirty; }
        }

        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (!this._values.ContainsKey(name))
                {
                    return null;
                }

                return this._values[name];
            }
            set
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (!this._values.ContainsKey(name))
                {
                    this._values.Add(name, value);
                }
                else
                {
                    if (string.CompareOrdinal(this._values[name], value) == 0)
                    {
                        return;
                    }

                    this._values[name] = value;
                }

                this._isDirty = true;
            }
        }

        public string RawValue
        {
            get { return this._rawValue; }
        }

        public ValueCollection(string values)
        {
            this._rawValue = values;
            this.Parse(this._rawValue);
        }

        private void Parse(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(val);
                if (xmlDocument.DocumentElement != null)
                {
                    foreach (XmlElement documentElement in xmlDocument.DocumentElement)
                    {
                        if (documentElement.Attributes["Name"] == null)
                        {
                            continue;
                        }

                        this._values.Add(documentElement.Attributes["Name"].Value, documentElement.InnerText);
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Concat("Failed to load xml.", exception));
            }
        }

        public string Remove(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!this._values.ContainsKey(name))
            {
                return null;
            }

            string item = this._values[name];
            this._values.Remove(name);
            this._isDirty = true;
            return item;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (TextWriter stringWriter = new StringWriter(stringBuilder))
            {
                using (XmlWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("Values");
                    foreach (KeyValuePair<string, string> _value in this._values)
                    {
                        xmlTextWriter.WriteStartElement("Value");
                        xmlTextWriter.WriteAttributeString("Name", _value.Key);
                        xmlTextWriter.WriteString(_value.Value);
                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                }
            }

            return stringBuilder.ToString();
        }
    }
}