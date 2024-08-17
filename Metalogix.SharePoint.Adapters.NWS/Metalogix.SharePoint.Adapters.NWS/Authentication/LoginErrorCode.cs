using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Authentication
{
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public enum LoginErrorCode
    {
        NoError,
        NotInFormsAuthenticationMode,
        PasswordNotMatch
    }
}