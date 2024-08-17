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
    [XmlInclude(typeof(DeletedSocialCommentDetail))]
    [XmlInclude(typeof(DeletedSocialRatingDetail))]
    [XmlInclude(typeof(DeletedSocialTagDetail))]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    public abstract class DeletedSocialDataDetail
    {
        private string urlField;

        private string ownerField;

        private DateTime deletedTimeField;

        public DateTime DeletedTime
        {
            get { return this.deletedTimeField; }
            set { this.deletedTimeField = value; }
        }

        public string Owner
        {
            get { return this.ownerField; }
            set { this.ownerField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        protected DeletedSocialDataDetail()
        {
        }
    }
}