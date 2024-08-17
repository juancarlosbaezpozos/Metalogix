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
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public class GetProductReleaseRequest
    {
        private string licenseKeyField;

        private int productCodeField;

        public string LicenseKey
        {
            get { return this.licenseKeyField; }
            set { this.licenseKeyField = value; }
        }

        public int ProductCode
        {
            get { return this.productCodeField; }
            set { this.productCodeField = value; }
        }

        public GetProductReleaseRequest()
        {
        }
    }
}