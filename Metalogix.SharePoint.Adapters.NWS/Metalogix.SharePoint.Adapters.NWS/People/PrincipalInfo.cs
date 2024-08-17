using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.People
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.18034")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class PrincipalInfo
    {
        private string accountNameField;

        private int userInfoIDField;

        private string displayNameField;

        private string emailField;

        private string departmentField;

        private string titleField;

        private bool isResolvedField;

        private PrincipalInfo[] moreMatchesField;

        private SPPrincipalType principalTypeField;

        public string AccountName
        {
            get { return this.accountNameField; }
            set { this.accountNameField = value; }
        }

        public string Department
        {
            get { return this.departmentField; }
            set { this.departmentField = value; }
        }

        public string DisplayName
        {
            get { return this.displayNameField; }
            set { this.displayNameField = value; }
        }

        public string Email
        {
            get { return this.emailField; }
            set { this.emailField = value; }
        }

        public bool IsResolved
        {
            get { return this.isResolvedField; }
            set { this.isResolvedField = value; }
        }

        [XmlArrayItem(IsNullable = false)]
        public PrincipalInfo[] MoreMatches
        {
            get { return this.moreMatchesField; }
            set { this.moreMatchesField = value; }
        }

        public SPPrincipalType PrincipalType
        {
            get { return this.principalTypeField; }
            set { this.principalTypeField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public int UserInfoID
        {
            get { return this.userInfoIDField; }
            set { this.userInfoIDField = value; }
        }

        public PrincipalInfo()
        {
        }
    }
}