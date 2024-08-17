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
    public class GetLicensesRequest
    {
        private int? productCodeField;

        private StringFilter licenseKeyField;

        private Metalogix.Licensing.LicenseServer.Service.LicenseType? licenseTypeField;

        private StringFilter customerField;

        private StringFilter contactNameField;

        private DateFilter expirationField;

        private DateFilter creationField;

        private DateFilter maintenanceExpirationField;

        private int offsetField;

        private int pageSizeField;

        public StringFilter ContactName
        {
            get { return this.contactNameField; }
            set { this.contactNameField = value; }
        }

        public DateFilter Creation
        {
            get { return this.creationField; }
            set { this.creationField = value; }
        }

        public StringFilter Customer
        {
            get { return this.customerField; }
            set { this.customerField = value; }
        }

        public DateFilter Expiration
        {
            get { return this.expirationField; }
            set { this.expirationField = value; }
        }

        public StringFilter LicenseKey
        {
            get { return this.licenseKeyField; }
            set { this.licenseKeyField = value; }
        }

        [XmlElement(IsNullable = true)]
        public Metalogix.Licensing.LicenseServer.Service.LicenseType? LicenseType
        {
            get { return this.licenseTypeField; }
            set { this.licenseTypeField = value; }
        }

        public DateFilter MaintenanceExpiration
        {
            get { return this.maintenanceExpirationField; }
            set { this.maintenanceExpirationField = value; }
        }

        public int Offset
        {
            get { return this.offsetField; }
            set { this.offsetField = value; }
        }

        public int PageSize
        {
            get { return this.pageSizeField; }
            set { this.pageSizeField = value; }
        }

        [XmlElement(IsNullable = true)]
        public int? ProductCode
        {
            get { return this.productCodeField; }
            set { this.productCodeField = value; }
        }

        public GetLicensesRequest()
        {
        }
    }
}