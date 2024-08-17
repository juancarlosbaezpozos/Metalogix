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
    public class MembershipData
    {
        private MembershipSource sourceField;

        private MemberGroupData memberGroupField;

        private string groupField;

        private string displayNameField;

        private Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy privacyField;

        private string mailNicknameField;

        private string urlField;

        private long idField;

        private long memberGroupIDField;

        public string DisplayName
        {
            get { return this.displayNameField; }
            set { this.displayNameField = value; }
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

        public string MailNickname
        {
            get { return this.mailNicknameField; }
            set { this.mailNicknameField = value; }
        }

        public MemberGroupData MemberGroup
        {
            get { return this.memberGroupField; }
            set { this.memberGroupField = value; }
        }

        public long MemberGroupID
        {
            get { return this.memberGroupIDField; }
            set { this.memberGroupIDField = value; }
        }

        public Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy Privacy
        {
            get { return this.privacyField; }
            set { this.privacyField = value; }
        }

        public MembershipSource Source
        {
            get { return this.sourceField; }
            set { this.sourceField = value; }
        }

        public string Url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        public MembershipData()
        {
        }
    }
}