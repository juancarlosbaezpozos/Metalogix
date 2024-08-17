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
    public class OrganizationProfileData
    {
        private string displayNameField;

        private long recordIDField;

        public string DisplayName
        {
            get { return this.displayNameField; }
            set { this.displayNameField = value; }
        }

        public long RecordID
        {
            get { return this.recordIDField; }
            set { this.recordIDField = value; }
        }

        public OrganizationProfileData()
        {
        }
    }
}