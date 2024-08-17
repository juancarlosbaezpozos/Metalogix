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
    public class DeletedSocialCommentDetail : DeletedSocialDataDetail
    {
        private DateTime lastModifiedTimeField;

        public DateTime LastModifiedTime
        {
            get { return this.lastModifiedTimeField; }
            set { this.lastModifiedTimeField = value; }
        }

        public DeletedSocialCommentDetail()
        {
        }
    }
}