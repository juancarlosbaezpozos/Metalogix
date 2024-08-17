using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = "http://license.metalogix.com/webservices/")]
    public class LicenseInfo
    {
        private object[] serversField;

        private LicStatus dbStatusField;

        private int idField;

        private PamProduct productField;

        private string custAddressField;

        private int mbxLicensedField;

        private string iPsField;

        private string iPExternalField;

        private object binaryField;

        private string creatorField;

        private string managerField;

        private string custNameField;

        private string projectField;

        private DateTime creationField;

        private DateTime expirationField;

        private string contNameField;

        private string contTelField;

        private string contMailField;

        private string keyValueField;

        private int mbxUsedField;

        private LicStatus statusField;

        private DateTime? lastRequestField;

        private string productStringField;

        private LicType licenseTypeField;

        public object Binary
        {
            get { return this.binaryField; }
            set { this.binaryField = value; }
        }

        public string ContMail
        {
            get { return this.contMailField; }
            set { this.contMailField = value; }
        }

        public string ContName
        {
            get { return this.contNameField; }
            set { this.contNameField = value; }
        }

        public string ContTel
        {
            get { return this.contTelField; }
            set { this.contTelField = value; }
        }

        public DateTime Creation
        {
            get { return this.creationField; }
            set { this.creationField = value; }
        }

        public string Creator
        {
            get { return this.creatorField; }
            set { this.creatorField = value; }
        }

        public string CustAddress
        {
            get { return this.custAddressField; }
            set { this.custAddressField = value; }
        }

        public string CustName
        {
            get { return this.custNameField; }
            set { this.custNameField = value; }
        }

        public LicStatus DbStatus
        {
            get { return this.dbStatusField; }
            set { this.dbStatusField = value; }
        }

        public DateTime Expiration
        {
            get { return this.expirationField; }
            set { this.expirationField = value; }
        }

        public int ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string IPExternal
        {
            get { return this.iPExternalField; }
            set { this.iPExternalField = value; }
        }

        public string IPs
        {
            get { return this.iPsField; }
            set { this.iPsField = value; }
        }

        public string KeyValue
        {
            get { return this.keyValueField; }
            set { this.keyValueField = value; }
        }

        [XmlElement(IsNullable = true)]
        public DateTime? LastRequest
        {
            get { return this.lastRequestField; }
            set { this.lastRequestField = value; }
        }

        public LicType LicenseType
        {
            get { return this.licenseTypeField; }
            set { this.licenseTypeField = value; }
        }

        public string Manager
        {
            get { return this.managerField; }
            set { this.managerField = value; }
        }

        public int MbxLicensed
        {
            get { return this.mbxLicensedField; }
            set { this.mbxLicensedField = value; }
        }

        public int MbxUsed
        {
            get { return this.mbxUsedField; }
            set { this.mbxUsedField = value; }
        }

        public PamProduct Product
        {
            get { return this.productField; }
            set { this.productField = value; }
        }

        public string ProductString
        {
            get { return this.productStringField; }
            set { this.productStringField = value; }
        }

        public string Project
        {
            get { return this.projectField; }
            set { this.projectField = value; }
        }

        public object[] Servers
        {
            get { return this.serversField; }
            set { this.serversField = value; }
        }

        public LicStatus Status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        public LicenseInfo()
        {
        }
    }
}