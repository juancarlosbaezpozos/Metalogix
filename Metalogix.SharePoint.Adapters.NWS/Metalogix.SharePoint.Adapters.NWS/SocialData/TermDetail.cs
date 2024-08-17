using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    public class TermDetail
    {
        private Guid idField;

        private string nameField;

        public Guid Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public TermDetail()
        {
        }
    }
}