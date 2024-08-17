using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlRoot(Namespace = "http://www.metalogix.com/", IsNullable = false)]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public class ClientVersionHeader : SoapHeader
    {
        private string versionField;

        private XmlAttribute[] anyAttrField;

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return this.anyAttrField; }
            set { this.anyAttrField = value; }
        }

        public string Version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }

        public ClientVersionHeader()
        {
        }
    }
}