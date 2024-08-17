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
    public class LicenseInfo
    {
        private string keyField;

        private DateTime licenseExpirationField;

        private DateTime maintenanceExpirationField;

        private LicenseType typeField;

        private LicenseStatus statusField;

        private string maxAllowedSetupVersionField;

        private int offlineRevalidationDaysField;

        private long licensedDataField;

        private int licensedServersField;

        private int licensedAdminsField;

        private long usedDataField;

        private int usedServersField;

        private int usedAdminsField;

        public string Key
        {
            get { return this.keyField; }
            set { this.keyField = value; }
        }

        public int LicensedAdmins
        {
            get { return this.licensedAdminsField; }
            set { this.licensedAdminsField = value; }
        }

        public long LicensedData
        {
            get { return this.licensedDataField; }
            set { this.licensedDataField = value; }
        }

        public int LicensedServers
        {
            get { return this.licensedServersField; }
            set { this.licensedServersField = value; }
        }

        public DateTime LicenseExpiration
        {
            get { return this.licenseExpirationField; }
            set { this.licenseExpirationField = value; }
        }

        public DateTime MaintenanceExpiration
        {
            get { return this.maintenanceExpirationField; }
            set { this.maintenanceExpirationField = value; }
        }

        public string MaxAllowedSetupVersion
        {
            get { return this.maxAllowedSetupVersionField; }
            set { this.maxAllowedSetupVersionField = value; }
        }

        public int OfflineRevalidationDays
        {
            get { return this.offlineRevalidationDaysField; }
            set { this.offlineRevalidationDaysField = value; }
        }

        public LicenseStatus Status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        public LicenseType Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public int UsedAdmins
        {
            get { return this.usedAdminsField; }
            set { this.usedAdminsField = value; }
        }

        public long UsedData
        {
            get { return this.usedDataField; }
            set { this.usedDataField = value; }
        }

        public int UsedServers
        {
            get { return this.usedServersField; }
            set { this.usedServersField = value; }
        }

        public LicenseInfo()
        {
        }
    }
}