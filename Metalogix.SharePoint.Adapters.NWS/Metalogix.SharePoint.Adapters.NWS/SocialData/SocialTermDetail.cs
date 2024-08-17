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
    public class SocialTermDetail
    {
        private TermDetail termField;

        private long countField;

        public long Count
        {
            get { return this.countField; }
            set { this.countField = value; }
        }

        public TermDetail Term
        {
            get { return this.termField; }
            set { this.termField = value; }
        }

        public SocialTermDetail()
        {
        }
    }
}