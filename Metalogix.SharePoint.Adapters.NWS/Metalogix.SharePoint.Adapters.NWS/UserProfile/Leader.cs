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
    public class Leader
    {
        private string accountNameField;

        private bool validField;

        private string managerAccountNameField;

        private int reportCountField;

        public string AccountName
        {
            get { return this.accountNameField; }
            set { this.accountNameField = value; }
        }

        public string ManagerAccountName
        {
            get { return this.managerAccountNameField; }
            set { this.managerAccountNameField = value; }
        }

        public int ReportCount
        {
            get { return this.reportCountField; }
            set { this.reportCountField = value; }
        }

        public bool Valid
        {
            get { return this.validField; }
            set { this.validField = value; }
        }

        public Leader()
        {
        }
    }
}