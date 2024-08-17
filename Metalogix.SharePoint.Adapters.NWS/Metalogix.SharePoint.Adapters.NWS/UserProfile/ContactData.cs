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
    public class ContactData
    {
        private string accountNameField;

        private Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy privacyField;

        private string nameField;

        private bool isInWorkGroupField;

        private string groupField;

        private string emailField;

        private string titleField;

        private string urlField;

        private Guid userProfileIDField;

        private long idField;

        public string AccountName
        {
            get { return this.accountNameField; }
            set { this.accountNameField = value; }
        }

        public string Email
        {
            get { return this.emailField; }
            set { this.emailField = value; }
        }

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

        public bool IsInWorkGroup
        {
            get { return this.isInWorkGroupField; }
            set { this.isInWorkGroupField = value; }
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

        public Guid UserProfileID
        {
            get { return this.userProfileIDField; }
            set { this.userProfileIDField = value; }
        }

        public ContactData()
        {
        }
    }
}