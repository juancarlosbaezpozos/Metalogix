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
    [XmlInclude(typeof(SocialCommentDetail))]
    [XmlInclude(typeof(SocialRatingDetail))]
    [XmlInclude(typeof(SocialTagDetail))]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    public abstract class SocialDataDetail
    {
        private string urlField;

        private string ownerField;

        private DateTime lastModifiedTimeField;

        private string titleField;

        public DateTime LastModifiedTime
        {
            get { return this.lastModifiedTimeField; }
            set { this.lastModifiedTimeField = value; }
        }

        public string Owner
        {
            get { return this.ownerField; }
            set { this.ownerField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        protected SocialDataDetail()
        {
        }
    }
}