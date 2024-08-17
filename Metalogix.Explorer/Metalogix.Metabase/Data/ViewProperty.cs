using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class ViewProperty
    {
        public static int DEFAULT_COLUMN_WIDTH;

        public static bool DEFAULT_COLUMN_DISPLAY;

        private System.ComponentModel.PropertyDescriptor m_propertyDescriptor;

        private bool m_bIsDisplayed = ViewProperty.DEFAULT_COLUMN_DISPLAY;

        private int m_iColumnWidth = ViewProperty.DEFAULT_COLUMN_WIDTH;

        public int ColumnWidth
        {
            get { return this.m_iColumnWidth; }
            set { this.m_iColumnWidth = value; }
        }

        public bool IsDisplayed
        {
            get { return this.m_bIsDisplayed; }
            set { this.m_bIsDisplayed = value; }
        }

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor
        {
            get { return this.m_propertyDescriptor; }
        }

        static ViewProperty()
        {
            ViewProperty.DEFAULT_COLUMN_WIDTH = 100;
            ViewProperty.DEFAULT_COLUMN_DISPLAY = true;
        }

        public ViewProperty(System.ComponentModel.PropertyDescriptor propertyDescriptor, bool bIsDisplayed,
            int iColumnWidth)
        {
            this.m_propertyDescriptor = propertyDescriptor;
            this.m_bIsDisplayed = bIsDisplayed;
            this.m_iColumnWidth = iColumnWidth;
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(100);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXml(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ViewProperty");
            xmlWriter.WriteStartElement("PropertyName");
            xmlWriter.WriteCData(this.PropertyDescriptor.Name);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteElementString("ColumnWidth", this.ColumnWidth.ToString());
            xmlWriter.WriteElementString("IsDisplayed", this.IsDisplayed.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}