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
    public class CategoryType
    {
        private Category idField;

        private string labelField;

        private bool hideWarningsByDefaultField;

        public bool HideWarningsByDefault
        {
            get { return this.hideWarningsByDefaultField; }
            set { this.hideWarningsByDefaultField = value; }
        }

        public Category Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string Label
        {
            get { return this.labelField; }
            set { this.labelField = value; }
        }

        public CategoryType()
        {
        }
    }
}