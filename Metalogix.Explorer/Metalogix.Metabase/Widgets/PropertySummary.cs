using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Widgets
{
    public class PropertySummary : PropertyValueSummary
    {
        private PropertySummaryList m_parentList;

        private System.ComponentModel.PropertyDescriptor m_propertyDescriptor;

        private bool m_bCaseSensitive = true;

        private bool m_bShowTotal;

        private bool m_bShowTotalWithChildren = true;

        private bool m_bShowPercentageToParent;

        private bool m_bShowPercentageToTotal;

        private string m_formatString = string.Empty;

        private char[] m_SeparatorChars = new char[] { '/', '\\' };

        public bool CaseSensitive
        {
            get { return this.m_bCaseSensitive; }
            set { this.m_bCaseSensitive = value; }
        }

        public string Format
        {
            get { return this.m_formatString; }
            set { this.m_formatString = value; }
        }

        public PropertySummaryList ParentList
        {
            get { return this.m_parentList; }
            set { this.m_parentList = value; }
        }

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor
        {
            get { return this.m_propertyDescriptor; }
        }

        public override string PropertyStringValue
        {
            get { return this.PropertyDescriptor.Name; }
        }

        public char[] Separators
        {
            get { return this.m_SeparatorChars; }
            set { this.m_SeparatorChars = value; }
        }

        public bool ShowPercentageToParent
        {
            get { return this.m_bShowPercentageToParent; }
            set { this.m_bShowPercentageToParent = value; }
        }

        public bool ShowPercentageToTotal
        {
            get { return this.m_bShowPercentageToTotal; }
            set { this.m_bShowPercentageToTotal = value; }
        }

        public bool ShowTotal
        {
            get { return this.m_bShowTotal; }
            set { this.m_bShowTotal = value; }
        }

        public bool ShowTotalWithChildren
        {
            get { return this.m_bShowTotalWithChildren; }
            set { this.m_bShowTotalWithChildren = value; }
        }

        public override string Text
        {
            get
            {
                string propertyStringValue = "";
                FillFactorAttribute item =
                    (FillFactorAttribute)this.PropertyDescriptor.Attributes[typeof(FillFactorAttribute)];
                if (item == null)
                {
                    propertyStringValue = base.PropertyStringValue;
                }
                else
                {
                    propertyStringValue = (item.Level == FillFactorLevel.Empty
                        ? base.PropertyStringValue
                        : string.Concat(base.PropertyStringValue, " (", item.ToString(), ")"));
                }

                return propertyStringValue;
            }
        }

        public PropertySummary(System.ComponentModel.PropertyDescriptor propertyDescriptor) : base(
            propertyDescriptor.DisplayName, null, null)
        {
            this.m_propertyDescriptor = propertyDescriptor;
            if (this.m_propertyDescriptor.PropertyType == typeof(DateTime))
            {
                this.Format = "yyyy/MMM/dd/h:mm tt";
            }
            else if (this.m_propertyDescriptor.PropertyType == typeof(Url))
            {
                this.Format = "HostAndPath";
            }

            base.RootNode = this;
            if (this.PropertyDescriptor.PropertyType.Equals(typeof(TextMoniker)))
            {
                base.RootNode.Separators = new char[0];
            }
        }

        public void SummarizeValue(object objValue)
        {
            PropertySummary numOccurrences = this;
            numOccurrences.NumOccurrences = numOccurrences.NumOccurrences + 1;
            object value = this.m_propertyDescriptor.GetValue(objValue);
            string str = null;
            if (value != null)
            {
                str = (string.IsNullOrEmpty(this.m_formatString)
                    ? value.ToString()
                    : string.Format(string.Concat("{0:", this.m_formatString, "}"), value));
            }

            base.SummarizeChildValue(str, value);
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXml(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("PropertySummary");
            xmlWriter.WriteStartElement("PropertyName");
            xmlWriter.WriteCData(this.PropertyDescriptor.Name);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteElementString("Format", this.Format);
            xmlWriter.WriteStartElement("Separators");
            xmlWriter.WriteCData(new string(this.Separators));
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}