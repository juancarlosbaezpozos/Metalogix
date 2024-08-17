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
    public class SocialRatingDetail : SocialDataDetail
    {
        private int ratingField;

        public int Rating
        {
            get { return this.ratingField; }
            set { this.ratingField = value; }
        }

        public SocialRatingDetail()
        {
        }
    }
}