using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public abstract class AbstractLicense : License
    {
        protected const int _UNLIMITED_VALUE = -1;

        private LicenseFile _license;

        private Metalogix.Licensing.LicenseServer.LicenseKey _key;

        public LicenseFile _License
        {
            get { return this._license; }
            set { this._license = value; }
        }

        public bool Exists
        {
            get
            {
                if (this._license == null || this._license.License == null)
                {
                    return false;
                }

                return !string.IsNullOrEmpty(this._license.License.Key);
            }
        }

        public Metalogix.Licensing.LicenseServer.LicenseKey KeyValue
        {
            get
            {
                Metalogix.Licensing.LicenseServer.LicenseKey licenseKey = this._key;
                if (licenseKey == null)
                {
                    Metalogix.Licensing.LicenseServer.LicenseKey licenseKey1 =
                        new Metalogix.Licensing.LicenseServer.LicenseKey(this._license.License.Key);
                    Metalogix.Licensing.LicenseServer.LicenseKey licenseKey2 = licenseKey1;
                    this._key = licenseKey1;
                    licenseKey = licenseKey2;
                }

                return licenseKey;
            }
        }

        public override string LicenseKey
        {
            get { return this.KeyValue.ValueFormatted; }
        }

        protected AbstractLicense()
        {
        }

        public sealed override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            Logger.Warning.WriteFormat("AbstractLicense >> Dispose: Entered", new object[0]);
            if (disposing && this._license != null)
            {
                this._license.Dispose();
            }
        }

        ~AbstractLicense()
        {
            this.Dispose(false);
        }

        protected abstract void Initialize();

        protected virtual void Load()
        {
            Logger.Warning.WriteFormat("AbstractLicense >> Load: Entered", new object[0]);
            if (this._license != null)
            {
                this._license.Load();
            }
        }

        public virtual void Save()
        {
            Logger.Warning.WriteFormat("AbstractLicense >> Save: Entered", new object[0]);
            if (this._license != null)
            {
                this._license.Save();
            }
        }

        public abstract void Validate();

        protected virtual void Validate(Product product, string adminAccount, string serverID)
        {
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { product, adminAccount, serverID, null };
            objArray[3] = (this.Exists ? this.LicenseKey : "NULL");
            debug.WriteFormat("AbstractLicense >> Validate: Entered, Product={0}; Admin={1}; Server={2}; Key={3}",
                objArray);
            if (!this.Exists)
            {
                Logger.Warning.Write("AbstractLicense >> Validate: License doesn't exists");
                throw new InvalidLicenseException("License key does not exist.");
            }

            if (!this.KeyValue.IsValid)
            {
                ILogMethods warning = Logger.Warning;
                object[] key = new object[] { this._license.License.Key };
                warning.WriteFormat("AbstractLicense >> Validate: Key not valid Key={0};", key);
                throw new InvalidLicenseException("License key is invalid.");
            }

            if (this.KeyValue.Product != Product.EnterpriseMigrationKey &&
                this.KeyValue.Product != Product.UnifiedContentMatrixKey && !this.KeyValue.Validate(product))
            {
                ILogMethods logMethod = Logger.Warning;
                object[] objArray1 = new object[] { product, this.KeyValue.Product };
                logMethod.WriteFormat(
                    "AbstractLicense >> Validate: Key's product is not valid RequiredProduct={0}; KeyProduct={1}",
                    objArray1);
                throw new InvalidLicenseException("License key is not valid for this product.");
            }

            if (this.KeyValue.Product == Product.EnterpriseMigrationKey)
            {
                bool flag = false;
                CustomField[] fields = this._license.Content.CustomFields.Fields;
                int num = 0;
                while (num < (int)fields.Length)
                {
                    CustomField customField = fields[num];
                    if (!customField.Name.Equals(this.KeyValue.ProductCode.ToString()))
                    {
                        num++;
                    }
                    else
                    {
                        if (bool.TryParse(customField.Value, out flag))
                        {
                            break;
                        }

                        flag = false;
                        break;
                    }
                }

                if (!flag)
                {
                    ILogMethods warning1 = Logger.Warning;
                    object[] objArray2 = new object[] { product, this.KeyValue.Product };
                    warning1.WriteFormat(
                        "AbstractLicense >> Validate: EnterpriseKey's product is not valid RequiredProduct={0}; KeyProduct={1}",
                        objArray2);
                    throw new InvalidLicenseException("License key is not valid for the given product.");
                }
            }

            if (this._license.License.Status == LicenseStatus.Disabled)
            {
                Logger.Warning.Write("AbstractLicense >> Validate: Key was disabled");
                throw new InvalidLicenseException(
                    "License key is disabled. Please contact Support for more information.");
            }

            if (this._license.License.Status == LicenseStatus.Expired)
            {
                ILogMethods logMethod1 = Logger.Warning;
                object[] licenseExpiration = new object[] { this._license.License.LicenseExpiration };
                logMethod1.WriteFormat("AbstractLicense >> Validate: Key has expired on {0};", licenseExpiration);
                throw new InvalidLicenseException("License key has expired.");
            }

            if (this._license.License.Status != LicenseStatus.Valid)
            {
                ILogMethods warning2 = Logger.Warning;
                object[] status = new object[] { this._license.License.Status };
                warning2.WriteFormat("AbstractLicense >> Validate: Key has invalid status {0};", status);
                throw new InvalidLicenseException("License key has invalid status.");
            }

            if (this._license.License.LicenseExpiration < DateTime.Now.ToUniversalTime())
            {
                ILogMethods logMethod2 = Logger.Warning;
                object[] licenseExpiration1 = new object[] { this._license.License.LicenseExpiration };
                logMethod2.WriteFormat("AbstractLicense >> Validate: Key has expired on {0} because of the local time;",
                    licenseExpiration1);
                throw new InvalidLicenseException("License key has expired.");
            }

            if (this._license.License.LicensedAdmins != -1 &&
                this._license.License.UsedAdmins > this._license.License.LicensedAdmins)
            {
                ILogMethods warning3 = Logger.Warning;
                object[] usedAdmins = new object[]
                    { this._license.License.UsedAdmins, this._license.License.LicensedAdmins };
                warning3.WriteFormat(
                    "AbstractLicense >> Validate: Admin seats count reached the allowed: Used={0}; Allowed={1}",
                    usedAdmins);
                throw new InvalidLicenseException("You have reached the allowed administrator seats count.");
            }

            if (this._license.License.LicensedServers != -1 &&
                this._license.License.UsedServers > this._license.License.LicensedServers)
            {
                ILogMethods logMethod3 = Logger.Warning;
                object[] usedServers = new object[]
                    { this._license.License.UsedServers, this._license.License.LicensedServers };
                logMethod3.WriteFormat(
                    "AbstractLicense >> Validate: Servers count reached the allowed: Used={0}; Allowed={1}",
                    usedServers);
                throw new InvalidLicenseException("You have reached the allowed server installation count.");
            }

            if (this._license.License.LicensedData != (long)-1 &&
                this._license.License.UsedData > this._license.License.LicensedData)
            {
                ILogMethods warning4 = Logger.Warning;
                object[] usedData = new object[] { this._license.License.UsedData, this._license.License.LicensedData };
                warning4.WriteFormat(
                    "AbstractLicense >> Validate: Used data reached the allowed: Used={0}; Allowed={1}", usedData);
                throw new InvalidLicenseException("You have reached the allowed data count.");
            }

            ILogMethods debug1 = Logger.Debug;
            object[] objArray3 = new object[] { (this._license.License.LicensedServers == -1 ? "NOT " : "") };
            debug1.WriteFormat("AbstractLicense >> Validate: Server is {0}validated.", objArray3);
            ILogMethods debug2 = Logger.Debug;
            object[] objArray4 = new object[] { (this._license.License.LicensedAdmins == -1 ? "NOT " : "") };
            debug2.WriteFormat("AbstractLicense >> Validate: Admin is {0}validated.", objArray4);
            if (this._license.License.LicensedServers != -1 &&
                string.CompareOrdinal(this._license.ServerID, serverID) != 0)
            {
                ILogMethods logMethod4 = Logger.Warning;
                object[] objArray5 = new object[] { serverID, this._license.ServerID };
                logMethod4.WriteFormat(
                    "AbstractLicense >> Validate: Server name checking failed: Used={0}; Required={1}", objArray5);
                throw new InvalidLicenseException("License key is not valid on the given server.");
            }

            if (this._license.License.LicensedAdmins != -1 &&
                string.CompareOrdinal(this._license.AdminAccount, adminAccount) != 0)
            {
                ILogMethods warning5 = Logger.Warning;
                object[] objArray6 = new object[] { adminAccount, this._license.AdminAccount };
                warning5.WriteFormat("AbstractLicense >> Validate: Admin name checking failed: Used={0}; Required={1}",
                    objArray6);
                throw new InvalidLicenseException("License key is not valid with the given user account.");
            }

            DateTime now = DateTime.Now;
            TimeSpan universalTime = now.ToUniversalTime() - this._license.Content.Updated;
            if (this._license.License.OfflineRevalidationDays != -1 &&
                universalTime.TotalDays > (double)this._license.License.OfflineRevalidationDays)
            {
                ILogMethods logMethod5 = Logger.Warning;
                object[] updated = new object[]
                {
                    this._license.Content.Updated, this._license.License.OfflineRevalidationDays,
                    universalTime.TotalDays
                };
                logMethod5.WriteFormat(
                    "AbstractLicense >> Validate: License revalidation is required, because the allowed offline date is off: LastSync={0}; MaxOfflineDays={1}; CurrentOfflineDays",
                    updated);
                throw new InvalidLicenseException(
                    "You have reached the maximum allowed days without reactivating your license. Please reactivate your license key.");
            }
        }
    }
}