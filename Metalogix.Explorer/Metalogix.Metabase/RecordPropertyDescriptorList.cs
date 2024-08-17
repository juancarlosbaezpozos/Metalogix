using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase
{
    public class RecordPropertyDescriptorList : PropertyDescriptorCollection
    {
        private Metalogix.Metabase.Workspace m_workspace;

        public Metalogix.Metabase.Workspace Workspace
        {
            get { return this.m_workspace; }
        }

        private RecordPropertyDescriptorList() : base(null)
        {
        }

        public RecordPropertyDescriptorList(string sXml) : this(null, sXml)
        {
        }

        public RecordPropertyDescriptorList(Metalogix.Metabase.Workspace workspace, string sXml) : base(null)
        {
            base.Clear();
            this.m_workspace = workspace;
            if (workspace != null)
            {
                Type baseType = workspace.BaseType;
                if (baseType != null)
                {
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(baseType))
                    {
                        if (!property.IsBrowsable)
                        {
                            continue;
                        }

                        this.Add(property);
                    }
                }
            }

            if (sXml != null && sXml.Length > 0)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sXml);
                this.FromXml(xmlDocument.DocumentElement.SelectSingleNode("//ItemPropertyDescriptorList"));
            }

            base.InternalSort(new RecordPropertyDescriptorComparer());
        }

        public new int Add(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                return -1;
            }

            if (this.Find(pd.Name) != null)
            {
                throw new ArgumentException(string.Concat("Property ", pd.Name, " already exists."));
            }

            return base.Add(pd);
        }

        public void Add(PropertyDescriptor pd, string sCategory)
        {
            if (pd == null)
            {
                return;
            }

            string name = pd.Name;
            PropertyDescriptor propertyDescriptor = this.Find(name);
            if (propertyDescriptor == null)
            {
                this.Add(pd);
                return;
            }

            if (propertyDescriptor.Category == sCategory)
            {
                throw new ArgumentException("A property with the same name already exists.", "pd");
            }

            RecordPropertyDescriptor recordPropertyDescriptor = (RecordPropertyDescriptor)pd;
            recordPropertyDescriptor.SetName(RecordPropertyDescriptorList.BuildCategorizedName(name, sCategory));
            recordPropertyDescriptor.SetDisplayName(name);
            recordPropertyDescriptor.SetCategory(sCategory);
            this.Add(recordPropertyDescriptor);
        }

        private static string BuildCategorizedName(string sName, string sCategory)
        {
            int hashCode = sCategory.GetHashCode();
            return string.Concat((hashCode < 0 ? string.Concat("0", Math.Abs(hashCode)) : hashCode.ToString()), "_",
                sName);
        }

        public PropertyDescriptor Find(string sName)
        {
            PropertyDescriptor propertyDescriptor = base.Find(sName, true);
            if (base.Contains(propertyDescriptor))
            {
                return propertyDescriptor;
            }

            return null;
        }

        public PropertyDescriptor Find(string sName, string sCategory)
        {
            PropertyDescriptor propertyDescriptor = this.Find(sName);
            if (propertyDescriptor == null)
            {
                return null;
            }

            if (propertyDescriptor.Category == sCategory)
            {
                return propertyDescriptor;
            }

            return this.Find(RecordPropertyDescriptorList.BuildCategorizedName(sName, sCategory));
        }

        private void FromXml(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            XmlNodeList xmlNodeLists = xmlNode.SelectNodes("//ItemPropertyDescriptor");
            if (xmlNodeLists == null)
            {
                return;
            }

            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                try
                {
                    RecordPropertyDescriptor recordPropertyDescriptor = new RecordPropertyDescriptor(xmlNodes);
                    this.Add(recordPropertyDescriptor, recordPropertyDescriptor.Category);
                }
                catch (Exception exception)
                {
                }
            }
        }

        public Type GetBaseType()
        {
            return this.m_workspace.BaseType;
        }

        public new void Remove(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                return;
            }

            if (this.Find(pd.Name) == null)
            {
                return;
            }

            this.m_workspace.Records.BatchDeleteProperty(pd);
            base.Remove(pd);
        }

        public string ToXml()
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };
            xmlTextWriter.WriteStartElement("ItemPropertyDescriptorList");
            foreach (PropertyDescriptor propertyDescriptor in this)
            {
                RecordPropertyDescriptor recordPropertyDescriptor = propertyDescriptor as RecordPropertyDescriptor;
                if (recordPropertyDescriptor == null)
                {
                    continue;
                }

                xmlTextWriter.WriteRaw(recordPropertyDescriptor.ToXml());
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            memoryStream.Seek((long)0, SeekOrigin.Begin);
            return (new StreamReader(memoryStream)).ReadToEnd();
        }

        private struct XmlNames
        {
            public const string RECORD_PROPERTY_DESCRIPTOR_LIST = "ItemPropertyDescriptorList";
        }
    }
}