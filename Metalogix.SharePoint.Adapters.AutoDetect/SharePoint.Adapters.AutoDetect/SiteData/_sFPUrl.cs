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
    public class _sFPUrl
    {
        private string urlField;

        private DateTime lastModifiedField;

        private bool isFolderField;

        public bool IsFolder
        {
            get { return this.isFolderField; }
            set { this.isFolderField = value; }
        }

        public DateTime LastModified
        {
            get { return this.lastModifiedField; }
            set { this.lastModifiedField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        public _sFPUrl()
        {
        }
    }
}