using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlInclude(typeof(LicenseInfoResponse))]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public abstract class SignedResponse
    {
        private byte[] signatureField;

        [XmlElement(DataType = "base64Binary")]
        public byte[] Signature
        {
            get { return this.signatureField; }
            set { this.signatureField = value; }
        }

        protected SignedResponse()
        {
        }
    }
}