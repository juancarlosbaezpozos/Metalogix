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
    public class DeletedSocialTagDetail : DeletedSocialDataDetail
    {
        private Guid termIDField;

        public Guid TermID
        {
            get { return this.termIDField; }
            set { this.termIDField = value; }
        }

        public DeletedSocialTagDetail()
        {
        }
    }
}