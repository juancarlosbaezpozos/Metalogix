using System;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.DB
{
    public class SPListTemplate
    {
        private XmlNode m_listTemplateNode;

        private XmlNode m_fieldsNode;

        private XmlNode m_viewNode;

        public int BaseType
        {
            get
            {
                if (this.m_listTemplateNode.Attributes["BaseType"] == null)
                {
                    return -1;
                }

                return Convert.ToInt32(this.m_listTemplateNode.Attributes["BaseType"].Value);
            }
        }

        public XmlNode FieldsXML
        {
            get { return this.m_fieldsNode; }
            set { this.m_fieldsNode = value; }
        }

        public string Name
        {
            get { return this.m_listTemplateNode.Attributes["Name"].Value; }
        }

        public int Type
        {
            get { return Convert.ToInt32(this.m_listTemplateNode.Attributes["Type"].Value); }
        }

        public XmlNode ViewXML
        {
            get { return this.m_viewNode; }
            set { this.m_viewNode = value; }
        }

        public SPListTemplate(string sListTemplateXML, string sFieldsXML, string sViewXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sListTemplateXML);
            this.m_listTemplateNode = xmlDocument.FirstChild;
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(sViewXML);
            this.m_viewNode = xmlDocument1.FirstChild;
            XmlDocument xmlDocument2 = new XmlDocument();
            xmlDocument2.LoadXml(sFieldsXML);
            this.m_fieldsNode = xmlDocument2.FirstChild;
        }

        public override string ToString()
        {
            object[] name = new object[] { this.Name, "(", this.Type, ")" };
            return string.Concat(name);
        }
    }
}