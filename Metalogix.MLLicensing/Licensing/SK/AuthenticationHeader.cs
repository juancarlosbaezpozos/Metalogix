using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [Serializable]
    [XmlRoot(Namespace = "http://license.metalogix.com/webservices/", IsNullable = false)]
    [XmlType(Namespace = "http://license.metalogix.com/webservices/")]
    public class AuthenticationHeader : SoapHeader
    {
        private string authenticationStringField;

        private XmlAttribute[] anyAttrField;

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return this.anyAttrField; }
            set { this.anyAttrField = value; }
        }

        public string AuthenticationString
        {
            get { return this.authenticationStringField; }
            set { this.authenticationStringField = value; }
        }

        public AuthenticationHeader()
        {
        }
    }
}