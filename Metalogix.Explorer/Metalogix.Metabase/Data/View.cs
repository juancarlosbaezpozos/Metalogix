using Metalogix.Metabase;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class View
    {
        private string m_strName = string.Empty;

        private ViewPropertyList m_viewPropertyList;

        private ViewList m_parentList;

        private bool m_bShowGridLines = true;

        private bool m_bIsSelected;

        public bool IsSelected
        {
            get { return this.m_bIsSelected; }
            set
            {
                if (value)
                {
                    foreach (View parentList in this.ParentList)
                    {
                        parentList.IsSelected = false;
                    }
                }

                this.m_bIsSelected = value;
            }
        }

        public string Name
        {
            get { return this.m_strName; }
            set
            {
                if (value == null || string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentNullException("value");
                }

                this.m_strName = value;
            }
        }

        public ViewList ParentList
        {
            get { return this.m_parentList; }
        }

        public Workspace ParentWorkspace
        {
            get { return this.m_parentList.ParentWorkspace; }
        }

        public bool ShowGridLines
        {
            get { return this.m_bShowGridLines; }
            set { this.m_bShowGridLines = value; }
        }

        public ViewPropertyList ViewProperties
        {
            get { return this.m_viewPropertyList; }
        }

        public View(ViewList parentList, PropertyDescriptorCollection baseProperties, XmlNode viewNode)
        {
            if (parentList == null)
            {
                throw new ArgumentNullException("parentList");
            }

            if (baseProperties == null)
            {
                throw new ArgumentNullException("baseProperties");
            }

            this.m_parentList = parentList;
            XmlNode xmlNodes = null;
            if (viewNode != null)
            {
                XmlNode xmlNodes1 = viewNode.SelectSingleNode("./Name");
                if (xmlNodes1 != null)
                {
                    this.m_strName = xmlNodes1.InnerText;
                }

                XmlNode xmlNodes2 = viewNode.SelectSingleNode("./ShowGridLines");
                if (xmlNodes2 != null)
                {
                    this.m_bShowGridLines = (xmlNodes2.InnerText == "False" ? false : true);
                }

                XmlNode xmlNodes3 = viewNode.SelectSingleNode("./IsSelected");
                if (xmlNodes3 != null)
                {
                    this.m_bIsSelected = (xmlNodes3.InnerText == "False" ? false : true);
                }

                xmlNodes = viewNode.SelectSingleNode("./ViewPropertyList");
            }

            this.m_viewPropertyList = new ViewPropertyList(baseProperties, xmlNodes);
        }

        public override string ToString()
        {
            return this.Name;
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
            xmlWriter.WriteStartElement("View");
            xmlWriter.WriteStartElement("Name");
            xmlWriter.WriteCData(this.Name);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteElementString("ShowGridLines", this.ShowGridLines.ToString());
            xmlWriter.WriteElementString("IsSelected", this.IsSelected.ToString());
            this.ViewProperties.ToXml(xmlWriter);
            xmlWriter.WriteEndElement();
        }
    }
}