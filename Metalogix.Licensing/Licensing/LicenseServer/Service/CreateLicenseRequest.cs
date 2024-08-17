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
    public class CreateLicenseRequest
    {
        private int productCodeField;

        private LicenseCreateInfo licenseField;

        private Metalogix.Licensing.LicenseServer.Service.Customer customerField;

        private Metalogix.Licensing.LicenseServer.Service.CustomFields customFieldsField;

        public Metalogix.Licensing.LicenseServer.Service.Customer Customer
        {
            get { return this.customerField; }
            set { this.customerField = value; }
        }

        public Metalogix.Licensing.LicenseServer.Service.CustomFields CustomFields
        {
            get { return this.customFieldsField; }
            set { this.customFieldsField = value; }
        }

        public LicenseCreateInfo License
        {
            get { return this.licenseField; }
            set { this.licenseField = value; }
        }

        public int ProductCode
        {
            get { return this.productCodeField; }
            set { this.productCodeField = value; }
        }

        public CreateLicenseRequest()
        {
        }
    }
}