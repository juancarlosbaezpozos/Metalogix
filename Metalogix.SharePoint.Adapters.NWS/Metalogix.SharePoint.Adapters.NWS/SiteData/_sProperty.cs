using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class _sProperty
    {
        private string nameField;

        private string titleField;

        private string typeField;

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public string Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public _sProperty()
        {
        }
    }
}