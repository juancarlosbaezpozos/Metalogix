using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Authentication
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class LoginResult
    {
        private string cookieNameField;

        private LoginErrorCode errorCodeField;

        public string CookieName
        {
            get { return this.cookieNameField; }
            set { this.cookieNameField = value; }
        }

        public LoginErrorCode ErrorCode
        {
            get { return this.errorCodeField; }
            set { this.errorCodeField = value; }
        }

        public LoginResult()
        {
        }
    }
}