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
    public class SocialRatingAverageDetail
    {
        private string urlField;

        private float averageField;

        private DateTime lastModifiedTimeField;

        private long countField;

        private int currentUserRatingField;

        public float Average
        {
            get { return this.averageField; }
            set { this.averageField = value; }
        }

        public long Count
        {
            get { return this.countField; }
            set { this.countField = value; }
        }

        public int CurrentUserRating
        {
            get { return this.currentUserRatingField; }
            set { this.currentUserRatingField = value; }
        }

        public DateTime LastModifiedTime
        {
            get { return this.lastModifiedTimeField; }
            set { this.lastModifiedTimeField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        public SocialRatingAverageDetail()
        {
        }
    }
}