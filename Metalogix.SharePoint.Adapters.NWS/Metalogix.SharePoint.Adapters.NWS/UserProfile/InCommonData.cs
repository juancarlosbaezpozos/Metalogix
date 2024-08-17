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
    public class InCommonData
    {
        private ContactData managerField;

        private ContactData[] colleaguesField;

        private MembershipData[] membershipsField;

        public ContactData[] Colleagues
        {
            get { return this.colleaguesField; }
            set { this.colleaguesField = value; }
        }

        public ContactData Manager
        {
            get { return this.managerField; }
            set { this.managerField = value; }
        }

        public MembershipData[] Memberships
        {
            get { return this.membershipsField; }
            set { this.membershipsField = value; }
        }

        public InCommonData()
        {
        }
    }
}