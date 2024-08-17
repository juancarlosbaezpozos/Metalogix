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
    public class SocialCommentDetail : SocialDataDetail
    {
        private string commentField;

        private bool isHighPriorityField;

        public string Comment
        {
            get { return this.commentField; }
            set { this.commentField = value; }
        }

        public bool IsHighPriority
        {
            get { return this.isHighPriorityField; }
            set { this.isHighPriorityField = value; }
        }

        public SocialCommentDetail()
        {
        }
    }
}