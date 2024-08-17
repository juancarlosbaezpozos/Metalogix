using Metalogix;
using Metalogix.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.DataResolution
{
    public abstract class DataResolver : IXmlable
    {
        protected const string XML_NAME_DATARESOLVER = "DataResolver";

        protected const string XML_NAME_TYPE = "Type";

        private OptionsBase m_options;

        public virtual OptionsBase Options
        {
            get { return this.m_options; }
            set
            {
                this.m_options = value;
                this.FireOptionsChanged();
            }
        }

        protected DataResolver()
        {
        }

        public abstract void ClearAllData();

        public static DataResolver CreatDataResolverFromTypeName(string typeName)
        {
            Type type = Type.GetType(TypeUtils.UpdateType(typeName));
            if (type == null)
            {
                throw new Exception(string.Concat("The data resolver '", typeName, "' could not be found."));
            }

            return (DataResolver)Activator.CreateInstance(type);
        }

        public static DataResolver CreateDataResolver(string dataResolverSettingsXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(dataResolverSettingsXML);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("//DataResolver");
            if (xmlNodes == null)
            {
                throw new ArgumentException("XML provided does not define a data resolver");
            }

            XmlAttribute itemOf = xmlNodes.Attributes["Type"];
            if (itemOf == null)
            {
                throw new Exception(string.Format("The '{0}' attribute cannot be null", "Type"));
            }

            DataResolver dataResolver = DataResolver.CreatDataResolverFromTypeName(itemOf.Value);
            dataResolver.FromXML(xmlNodes);
            return dataResolver;
        }

        public abstract void DeleteDataAtKey(string key);

        protected void FireOptionsChanged()
        {
            this.OnOptionsChanged();
            if (this.OptionsChanged != null)
            {
                this.OptionsChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void FromXML(XmlNode xmlNode)
        {
            if (this.Options != null)
            {
                this.Options.FromXML(xmlNode);
                this.FireOptionsChanged();
            }
        }

        public abstract IEnumerable<string> GetAvailableDataKeys();

        public abstract byte[] GetDataAtKey(string key);

        public virtual string GetStringDataAtKey(string key)
        {
            return Encoding.UTF8.GetString(this.GetDataAtKey(key));
        }

        protected virtual void OnOptionsChanged()
        {
        }

        public string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            this.ToXML(new XmlTextWriter(stringWriter));
            return stringWriter.ToString();
        }

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("DataResolver");
            xmlWriter.WriteAttributeString("Type", this.GetType().AssemblyQualifiedName);
            if (this.Options != null)
            {
                this.Options.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }

        public abstract void WriteDataAtKey(string key, byte[] data);

        public virtual void WriteStringDataAtKey(string key, string data)
        {
            this.WriteDataAtKey(key, Encoding.UTF8.GetBytes(data));
        }

        public event DataResolver.ResolverOptionsChangedHandler OptionsChanged;

        public delegate void ResolverOptionsChangedHandler(object sender, EventArgs e);
    }
}