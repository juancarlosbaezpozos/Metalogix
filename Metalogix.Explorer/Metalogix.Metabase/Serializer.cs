using Metalogix.Metabase.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.Metabase
{
    public class Serializer
    {
        private static string s_lock;

        private static Serializer s_serializer;

        private Dictionary<string, Type> m_dictTypes = new Dictionary<string, Type>(16);

        public static Serializer Instance
        {
            get
            {
                lock (Serializer.s_lock)
                {
                    if (Serializer.s_serializer == null)
                    {
                        Serializer.s_serializer = new Serializer();
                    }
                }

                return Serializer.s_serializer;
            }
        }

        static Serializer()
        {
            Serializer.s_lock = string.Empty;
        }

        private Serializer()
        {
        }

        public object Copy(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (!this.IsRegistered(obj.GetType()))
            {
                return null;
            }

            string str = this.Serialize(obj);
            return this.Deserialize(str, obj.GetType().FullName);
        }

        public object Deserialize(string sObj, string sType)
        {
            Type type;
            if (!this.m_dictTypes.TryGetValue(sType, out type))
            {
                return null;
            }

            return this.Deserialize(sObj, type);
        }

        public object Deserialize(string sObj, Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (typeof(IXmlableV3).IsAssignableFrom(type))
            {
                IXmlableV3 xmlableV3 = (IXmlableV3)Activator.CreateInstance(type);
                xmlableV3.FromXml(sObj);
                return xmlableV3;
            }

            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(sObj));
            return (new XmlSerializer(type)).Deserialize(xmlTextReader);
        }

        public Type GetType(string sTypeFullName)
        {
            if (!this.m_dictTypes.ContainsKey(sTypeFullName))
            {
                return null;
            }

            return this.m_dictTypes[sTypeFullName];
        }

        public bool IsRegistered(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return this.m_dictTypes.ContainsKey(type.FullName);
        }

        public void RegisterType(Type type)
        {
            if (type == null)
            {
                return;
            }

            if (!this.m_dictTypes.ContainsKey(type.FullName))
            {
                this.m_dictTypes[type.FullName] = type;
            }
        }

        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            IXmlableV3 xmlableV3 = obj as IXmlableV3;
            if (xmlableV3 != null)
            {
                return xmlableV3.ToXml();
            }

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            (new XmlSerializer(obj.GetType())).Serialize(xmlTextWriter, obj);
            xmlTextWriter.Flush();
            stringWriter.Flush();
            return stringWriter.ToString();
        }
    }
}