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
    public class LicenseInfoResponse : SignedResponse
    {
        private LicenseInfo licenseField;

        private long usedDataField;

        private string serverField;

        private string adminField;

        private DateTime updatedField;

        private Metalogix.Licensing.LicenseServer.Service.CustomFields customFieldsField;

        private string customerNameField;

        private string contactNameField;

        private string contactEmailField;

        private string systemInfoField;

        private string isFremiumField;

        public string Admin
        {
            get { return this.adminField; }
            set { this.adminField = value; }
        }

        public string ContactEmail
        {
            get { return this.contactEmailField; }
            set { this.contactEmailField = value; }
        }

        public string ContactName
        {
            get { return this.contactNameField; }
            set { this.contactNameField = value; }
        }

        public string CustomerName
        {
            get { return this.customerNameField; }
            set { this.customerNameField = value; }
        }

        public Metalogix.Licensing.LicenseServer.Service.CustomFields CustomFields
        {
            get { return this.customFieldsField; }
            set { this.customFieldsField = value; }
        }

        public string IsFremium
        {
            get { return this.isFremiumField; }
            set { this.isFremiumField = value; }
        }

        public LicenseInfo License
        {
            get { return this.licenseField; }
            set { this.licenseField = value; }
        }

        public string Server
        {
            get { return this.serverField; }
            set { this.serverField = value; }
        }

        public string SystemInfo
        {
            get { return this.systemInfoField; }
            set { this.systemInfoField = value; }
        }

        public DateTime Updated
        {
            get { return this.updatedField; }
            set { this.updatedField = value; }
        }

        public long UsedData
        {
            get { return this.usedDataField; }
            set { this.usedDataField = value; }
        }

        public LicenseInfoResponse()
        {
        }
    }
}