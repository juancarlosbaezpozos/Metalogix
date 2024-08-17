using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService")]
    public class QuickLinkData
    {
        private string nameField;

        private string groupField;

        private Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy privacyField;

        private string urlField;

        private long idField;

        public string Group
        {
            get { return this.groupField; }
            set { this.groupField = value; }
        }

        public long ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy Privacy
        {
            get { return this.privacyField; }
            set { this.privacyField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        public QuickLinkData()
        {
        }
    }
}