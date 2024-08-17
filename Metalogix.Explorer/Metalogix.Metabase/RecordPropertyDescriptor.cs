using Metalogix;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase
{
    public class RecordPropertyDescriptor : PropertyDescriptor, IXmlableV3
    {
        protected const int MAXLENGTH_NAME = 50;

        protected string m_sName;

        protected string m_sDisplayName;

        protected string m_sCategory;

        protected string m_sDescription;

        protected Type m_type;

        protected FillFactorAttribute m_fillFactorAttr = new FillFactorAttribute(0);

        protected object m_oDefaultValue;

        private RecordPropertyDescriptorConfiguration m_configuration = new RecordPropertyDescriptorConfiguration();

        public override AttributeCollection Attributes
        {
            get
            {
                Attribute[] categoryAttribute = new Attribute[]
                {
                    new CategoryAttribute(this.m_sCategory), new DescriptionAttribute(this.m_sDescription),
                    this.m_fillFactorAttr
                };
                return new AttributeCollection(categoryAttribute);
            }
        }

        public override string Category
        {
            get { return this.m_sCategory; }
        }

        public override Type ComponentType
        {
            get { return typeof(object); }
        }

        public RecordPropertyDescriptorConfiguration Configuration
        {
            get { return this.m_configuration; }
        }

        public object DefaultValue
        {
            get { return this.m_oDefaultValue; }
        }

        public override string Description
        {
            get { return this.m_sDescription; }
        }

        public override string DisplayName
        {
            get
            {
                if (this.m_sDisplayName != null && this.m_sDisplayName.Length > 0)
                {
                    return this.m_sDisplayName;
                }

                return this.m_sName;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                if (this.m_type.Equals(typeof(TextMoniker)))
                {
                    return true;
                }

                return false;
            }
        }

        public override string Name
        {
            get { return this.m_sName; }
        }

        public override Type PropertyType
        {
            get { return this.m_type; }
        }

        public RecordPropertyDescriptor(string sXml) : base("empty", null)
        {
            this.FromXml(sXml);
        }

        public RecordPropertyDescriptor(string sName, Type type) : this(sName, type, null)
        {
        }

        public RecordPropertyDescriptor(string sName, Type type, object oDefaultValue) : base(sName, null)
        {
            if (string.IsNullOrEmpty(sName))
            {
                throw new ArgumentNullException("sName", "Property name cannot be null");
            }

            if (sName.Length > 50)
            {
                throw new ArgumentException("Property name cannot be longer than 50 characters");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type", "Property type cannot be null");
            }

            if (oDefaultValue != null && !type.IsInstanceOfType(oDefaultValue))
            {
                throw new ArgumentException("oDefaultValue", "Default value must be null or instance of 'type'");
            }

            this.m_sName = sName;
            this.m_type = type;
            this.m_oDefaultValue = oDefaultValue;
        }

        public RecordPropertyDescriptor(XmlNode node) : base("empty", null)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "Source XML node cannot be null");
            }

            this.FromXml(node);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public void FromXml(XmlNode node)
        {
            if (node == null)
            {
                return;
            }

            XmlNode xmlNodes = node.SelectSingleNode("./Name");
            if (xmlNodes != null)
            {
                this.SetName(xmlNodes.InnerText);
            }

            XmlNode xmlNodes1 = node.SelectSingleNode("./DisplayName");
            if (xmlNodes1 != null)
            {
                this.m_sDisplayName = xmlNodes1.InnerText;
            }

            XmlNode xmlNodes2 = node.SelectSingleNode("./Category");
            if (xmlNodes2 != null)
            {
                this.m_sCategory = xmlNodes2.InnerText;
            }

            XmlNode xmlNodes3 = node.SelectSingleNode("./Description");
            if (xmlNodes3 != null)
            {
                this.m_sDescription = xmlNodes3.InnerText;
            }

            XmlNode xmlNodes4 = node.SelectSingleNode("./Type");
            string str = xmlNodes4.InnerText.Trim();
            if (str.Equals("Metalogix.Metabase.TextMoniker"))
            {
                str = "Metalogix.UI.Metabase.DataTypes.TextMoniker";
            }
            else if (str.Equals("Metalogix.Metabase.Url"))
            {
                str = "Metalogix.UI.Metabase.DataTypes.Url";
            }
            else if (str.Equals("Metalogix.Metabase.SharePointBcsValue"))
            {
                str = "Metalogix.UI.Metabase.DataTypes.SharePointBcsValue, Metalogix.UI.Metabase.DataTypes";
            }
            else if (str.Equals("Metalogix.Metabase.SharePointTaxonomyValue"))
            {
                str = "Metalogix.UI.Metabase.DataTypes.SharePointTaxonomyValue, Metalogix.UI.Metabase.DataTypes";
            }

            str = TypeUtils.UpdateType(str);
            Type type = Serializer.Instance.GetType(str);
            if (type != null)
            {
                this.m_type = type;
            }
            else if (!str.Equals("Metalogix.Data.IUrl"))
            {
                this.m_type = Type.GetType(str);
            }
            else
            {
                this.m_type = typeof(Url);
            }

            if (this.m_type == null)
            {
                throw new ArgumentException(string.Concat("Type (", xmlNodes4.InnerText, ") could not be found."));
            }

            this.m_oDefaultValue = null;
            XmlNode xmlNodes5 = node.SelectSingleNode("./DefaultValue");
            if (xmlNodes5 != null)
            {
                string innerText = xmlNodes5.InnerText;
                this.m_oDefaultValue = Record.DeserializeValue(null, this.m_sName, innerText, this.m_type);
            }

            if (this.m_configuration == null)
            {
                this.m_configuration = new RecordPropertyDescriptorConfiguration();
            }

            XmlNode xmlNodes6 = node.SelectSingleNode("./Configuration");
            if (xmlNodes6 != null)
            {
                this.m_configuration.FromXml(xmlNodes6);
            }
        }

        public void FromXml(string sXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXml(xmlDocument.DocumentElement);
        }

        public override object GetValue(object component)
        {
            Record record = component as Record;
            if (record == null)
            {
                return null;
            }

            return record.GetValue(this.m_sName, this.m_type);
        }

        bool Metalogix.Metabase.Interfaces.IXmlableV3.IsEqual(IXmlableV3 xmlable)
        {
            if (xmlable == null)
            {
                return false;
            }

            return xmlable.ToXml().Equals(((IXmlableV3)this).ToXml());
        }

        public override void ResetValue(object component)
        {
        }

        public void SetCategory(string sCategory)
        {
            this.m_sCategory = sCategory;
        }

        public void SetDescription(string sDescription)
        {
            this.m_sDescription = sDescription;
        }

        public void SetDisplayName(string sDisplayName)
        {
            this.m_sDisplayName = sDisplayName;
        }

        public void SetName(string sName)
        {
            if (string.IsNullOrEmpty(sName))
            {
                throw new ArgumentException("Empty property name.");
            }

            if (sName.Length > 50)
            {
                throw new ArgumentException("Property name cannot be longer than 50 characters");
            }

            this.m_sName = sName;
        }

        public void SetPropertyType(Type type)
        {
            this.m_type = type;
        }

        public override void SetValue(object component, object value)
        {
            Record record = component as Record;
            if (record == null)
            {
                return;
            }

            if (!typeof(TextMoniker).IsAssignableFrom(this.m_type))
            {
                record.SetValue(this.m_sName, value, this.m_type);
                return;
            }

            TextMoniker textMoniker = (TextMoniker)record.GetValue(this.m_sName, this.m_type);
            TextMoniker textMoniker1 = value as TextMoniker;
            if (textMoniker1 != null)
            {
                textMoniker.SetFullText(textMoniker1.GetFullText());
                return;
            }

            if (value == null)
            {
                textMoniker.SetFullText(string.Empty);
                return;
            }

            textMoniker.SetFullText(value.ToString());
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string ToXml()
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 3
            };
            this.ToXml(xmlTextWriter);
            memoryStream.Seek((long)0, SeekOrigin.Begin);
            return (new StreamReader(memoryStream, Encoding.UTF8)).ReadToEnd();
        }

        public void ToXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("ItemPropertyDescriptor");
            RecordPropertyDescriptor.WriteXmlCDATAElement(writer, "Name", this.m_sName);
            RecordPropertyDescriptor.WriteXmlCDATAElement(writer, "DisplayName", this.m_sDisplayName);
            RecordPropertyDescriptor.WriteXmlCDATAElement(writer, "Category", this.m_sCategory);
            RecordPropertyDescriptor.WriteXmlCDATAElement(writer, "Description", this.m_sDescription);
            RecordPropertyDescriptor.WriteXmlCDATAElement(writer, "Type", this.m_type.AssemblyQualifiedName);
            if (this.m_oDefaultValue != null)
            {
                string str = Record.SerializeValue(this.m_oDefaultValue);
                if (!string.IsNullOrEmpty(str))
                {
                    writer.WriteStartElement("DefaultValue");
                    writer.WriteValue(str);
                    writer.WriteEndElement();
                }
            }

            if (this.m_configuration != null)
            {
                writer.WriteStartElement("Configuration");
                this.m_configuration.ToXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();
        }

        private static void WriteXmlCDATAElement(XmlWriter xmlWriter, string sElementName, string sCDataValue)
        {
            if (xmlWriter == null || xmlWriter.WriteState == WriteState.Closed)
            {
                return;
            }

            if (sElementName == null)
            {
                return;
            }

            xmlWriter.WriteStartElement(sElementName);
            if (sCDataValue != null)
            {
                xmlWriter.WriteCData(sCDataValue);
            }

            xmlWriter.WriteEndElement();
        }

        internal struct XmlNames
        {
            public const string ITEM_PROPERTY_DESCRIPTOR = "ItemPropertyDescriptor";

            public const string NAME = "Name";

            public const string DISPLAY_NAME = "DisplayName";

            public const string CATEGORY = "Category";

            public const string DESRIPTION = "Description";

            public const string TYPE = "Type";

            public const string DEFAULT_VALUE = "DefaultValue";

            public const string CONFIGURATION = "Configuration";
        }
    }
}