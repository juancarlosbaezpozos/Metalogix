using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.AutoDetect.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class _sListWithTime
    {
        private string internalNameField;

        private DateTime lastModifiedField;

        private bool isEmptyField;

        public string InternalName
        {
            get { return this.internalNameField; }
            set { this.internalNameField = value; }
        }

        public bool IsEmpty
        {
            get { return this.isEmptyField; }
            set { this.isEmptyField = value; }
        }

        public DateTime LastModified
        {
            get { return this.lastModifiedField; }
            set { this.lastModifiedField = value; }
        }

        public _sListWithTime()
        {
        }
    }
}