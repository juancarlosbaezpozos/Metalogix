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
    public class GetUserProfileByIndexResult
    {
        private string nextValueField;

        private PropertyData[] userProfileField;

        private ContactData[] colleaguesField;

        private QuickLinkData[] quickLinksField;

        private PinnedLinkData[] pinnedLinksField;

        private MembershipData[] membershipsField;

        public ContactData[] Colleagues
        {
            get { return this.colleaguesField; }
            set { this.colleaguesField = value; }
        }

        public MembershipData[] Memberships
        {
            get { return this.membershipsField; }
            set { this.membershipsField = value; }
        }

        public string NextValue
        {
            get { return this.nextValueField; }
            set { this.nextValueField = value; }
        }

        public PinnedLinkData[] PinnedLinks
        {
            get { return this.pinnedLinksField; }
            set { this.pinnedLinksField = value; }
        }

        public QuickLinkData[] QuickLinks
        {
            get { return this.quickLinksField; }
            set { this.quickLinksField = value; }
        }

        public PropertyData[] UserProfile
        {
            get { return this.userProfileField; }
            set { this.userProfileField = value; }
        }

        public GetUserProfileByIndexResult()
        {
        }
    }
}