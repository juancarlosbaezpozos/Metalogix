using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices")]
    public class DesignCheckerInformation
    {
        private string applicationIdField;

        private int lcidField;

        private CategoryType[] categoriesField;

        private Message[] messagesField;

        public string ApplicationId
        {
            get { return this.applicationIdField; }
            set { this.applicationIdField = value; }
        }

        [XmlArrayItem("Category")]
        public CategoryType[] Categories
        {
            get { return this.categoriesField; }
            set { this.categoriesField = value; }
        }

        public int Lcid
        {
            get { return this.lcidField; }
            set { this.lcidField = value; }
        }

        public Message[] Messages
        {
            get { return this.messagesField; }
            set { this.messagesField = value; }
        }

        public DesignCheckerInformation()
        {
        }
    }
}