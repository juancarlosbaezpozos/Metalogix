using Metalogix;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Storage;
using Metalogix.MLLicensing.Properties;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Metalogix.Licensing.Common
{
    public sealed class MigrationLicense : AbstractLicense
    {
        private object m_oLockAllowedProd = new object();

        private object m_oLockUsage = new object();

        private readonly LicenseSettings _settings;

        private DynamicContentFile _usageFile;

        private List<Product> m_oAllowedProducts;

        private bool m_bDataLimitExceeded;

        private Metalogix.Licensing.LicensedSharePointVersions _allowedVersions =
            Metalogix.Licensing.LicensedSharePointVersions.All;

        public static string AdminAccount
        {
            get { return string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName); }
        }

        internal List<Product> AllowedProducts
        {
            get
            {
                int num;
                List<Product> mOAllowedProducts;
                lock (this.m_oLockAllowedProd)
                {
                    if (this.m_oAllowedProducts == null)
                    {
                        if (base.KeyValue.Product != Product.EnterpriseMigrationKey &&
                            base.KeyValue.Product != Product.UnifiedContentMatrixKey &&
                            base.KeyValue.Product != Product.UnifiedContentMatrixExpressKey)
                        {
                            throw new InvalidOperationException(string.Format(
                                "Multiple products are allowed just for the Enterprise and Content Matrix key and not for '{0}'",
                                base.KeyValue.Product));
                        }

                        this.m_oAllowedProducts = new List<Product>();
                        CustomField[] fields = base._License.Content.CustomFields.Fields;
                        for (int i = 0; i < (int)fields.Length; i++)
                        {
                            CustomField customField = fields[i];
                            if (int.TryParse(customField.Name, out num) && Enum.IsDefined(typeof(Product), num))
                            {
                                bool flag = false;
                                if (bool.TryParse(customField.Value, out flag) && flag)
                                {
                                    this.m_oAllowedProducts.Add((Product)Enum.Parse(typeof(Product), customField.Name));
                                }
                            }
                        }

                        mOAllowedProducts = this.m_oAllowedProducts;
                    }
                    else
                    {
                        mOAllowedProducts = this.m_oAllowedProducts;
                    }
                }

                return mOAllowedProducts;
            }
        }

        public string ContactEmail
        {
            get { return base._License.Content.ContactEmail; }
        }

        public string ContactName
        {
            get { return base._License.Content.ContactName; }
        }

        public string CustomerName
        {
            get { return base._License.Content.CustomerName; }
        }

        public bool DataLimitedExceeded
        {
            get
            {
                bool mBDataLimitExceeded;
                try
                {
                    this.Validate();
                    mBDataLimitExceeded = this.m_bDataLimitExceeded;
                }
                catch (Exception exception)
                {
                    mBDataLimitExceeded = true;
                }

                return mBDataLimitExceeded;
            }
        }

        public DateTime Expiration
        {
            get { return base._License.Content.License.LicenseExpiration; }
        }

        public bool IsContentMatrixExpress
        {
            get { return base.KeyValue.Product.IsContentMatrixExpress(); }
        }

        public bool IsDataLimitExceededForContentUnderMgmt
        {
            get
            {
                bool flag = false;
                if (this.LicensedData == (long)-1 || this.SkipWebChecking)
                {
                    return false;
                }

                if (!this.IsLegacyProduct && !this.IsContentMatrixExpress && this.UsedLicensedData > this.LicensedData)
                {
                    flag = true;
                }

                return flag;
            }
        }

        public bool IsLegacyProduct
        {
            get { return base.KeyValue.Product.IsLegacyProduct(); }
        }

        public bool IsValid
        {
            get
            {
                bool flag;
                try
                {
                    this.Validate();
                    flag = true;
                }
                catch (Exception exception)
                {
                    flag = false;
                }

                return flag;
            }
        }

        public DateTime LastUpdate
        {
            get { return base._License.Content.Updated; }
        }

        public int LicensedAdmins
        {
            get { return base._License.License.LicensedAdmins; }
        }

        public long LicensedData
        {
            get { return base._License.Content.License.LicensedData; }
        }

        public int LicensedServers
        {
            get { return base._License.License.LicensedServers; }
        }

        public Metalogix.Licensing.LicensedSharePointVersions LicensedSharePointVersions
        {
            get { return this._allowedVersions; }
        }

        public DateTime MaintenanceExpirationDate
        {
            get { return base._License.Content.License.MaintenanceExpiration; }
        }

        public int OfflineValidityDays
        {
            get
            {
                DateTime now = DateTime.Now;
                TimeSpan universalTime = now.ToUniversalTime() - base._License.Content.Updated;
                return (int)((double)base._License.License.OfflineRevalidationDays -
                             Math.Floor(universalTime.TotalDays));
            }
        }

        public static string ServerID
        {
            get { return LicensingUtils.SystemIDFull; }
        }

        public bool SkipWebChecking
        {
            get
            {
                bool flag;
                if (bool.TryParse(this.GetCustomFieldValue("skipWebChecking"), out flag))
                {
                    return flag;
                }

                return false;
            }
        }

        public Product SpecificProduct
        {
            get { return this._settings.Product; }
        }

        public LicenseType Type
        {
            get { return base._License.Content.License.Type; }
        }

        public double UsagePercentage
        {
            get { return 100 * (double)this.UsedDataFull / (double)this.LicensedData; }
        }

        public int UsedAdmins
        {
            get { return base._License.License.UsedAdmins; }
        }

        public long UsedData
        {
            get
            {
                if (base._License.UsedData > this._usageFile.UsedData)
                {
                    this._usageFile.UsedData = base._License.UsedData;
                }

                return this._usageFile.UsedData;
            }
        }

        public long UsedDataFull
        {
            get
            {
                long usedData = this._usageFile.UsedData - base._License.UsedData;
                return base._License.License.UsedData + (usedData > (long)0 ? usedData : (long)0);
            }
        }

        public long UsedItemCount
        {
            get { return this._usageFile.UsedItems; }
        }

        public long UsedLicensedData
        {
            get { return base._License.Content.License.UsedData; }
        }

        public int UsedServers
        {
            get { return base._License.License.UsedServers; }
        }

        public MigrationLicense(LicenseSettings settings) : this(settings, true)
        {
            this.InitializeLicense();
            base.Load();
            this.ParseLicenseContent();
            this.InitializeUsage(this.LicenseKey, true, false);
        }

        public MigrationLicense(LicenseSettings settings, LicenseInfoResponse data) : this(settings, true)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.InitializeLicense();
            base._License.Content = data;
            this.ParseLicenseContent();
            this.InitializeUsage(this.LicenseKey, false, true);
        }

        public MigrationLicense(LicenseSettings settings, OfflineActivatorFile offlineFile) : this(settings, true)
        {
            if (offlineFile == null)
            {
                throw new ArgumentNullException("offlineFile");
            }

            if (offlineFile.IsActivationRequest)
            {
                throw new ArgumentException(
                    "The given OfflineActivatorFile contains invalid data, content is activation request.",
                    "offlineFile");
            }

            this.InitializeLicense();
            base._License.Content = offlineFile.ResponseContent;
            this.ParseLicenseContent();
            this.InitializeUsage(this.LicenseKey, false, true);
        }

        private MigrationLicense(LicenseSettings settings, bool loadLicense)
        {
            if (settings.ProductVersion == null)
            {
                throw new ArgumentNullException("productVersion");
            }

            if (string.IsNullOrEmpty(settings.UsageFilePath))
            {
                throw new ArgumentNullException("usageFileName");
            }

            if (string.IsNullOrEmpty(settings.LicenseFilePath))
            {
                throw new ArgumentNullException("fileName");
            }

            this._settings = settings;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing && this._usageFile != null)
            {
                this._usageFile.Dispose();
            }

            base.Dispose(disposing);
        }

        public string FormatUsageData(long data)
        {
            if (this._settings.LicenseDataFormatter == null)
            {
                return string.Format("{0} {1}", data, this._settings.DataUnitName);
            }

            return this._settings.LicenseDataFormatter.FormatData(new long?(data), this._settings.DataUnitName);
        }

        private Metalogix.Licensing.LicensedSharePointVersions GetAllowedVersions(string versionPropertyName)
        {
            string customFieldValue = this.GetCustomFieldValue(versionPropertyName);
            if (string.IsNullOrEmpty(customFieldValue))
            {
                return Metalogix.Licensing.LicensedSharePointVersions.All;
            }

            Metalogix.Licensing.LicensedSharePointVersions licensedSharePointVersion =
                Metalogix.Licensing.LicensedSharePointVersions.None;
            char[] chrArray = new char[] { ',' };
            string[] strArrays = customFieldValue.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                try
                {
                    licensedSharePointVersion |=
                        (Metalogix.Licensing.LicensedSharePointVersions)Enum.Parse(
                            typeof(Metalogix.Licensing.LicensedSharePointVersions), str, true);
                }
                catch (ArgumentException argumentException)
                {
                }
            }

            return licensedSharePointVersion;
        }

        public Dictionary<string, string> GetCustomFields()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            if (base._License.Content.CustomFields != null)
            {
                CustomField[] fields = base._License.Content.CustomFields.Fields;
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    CustomField customField = fields[i];
                    strs.Add(customField.Name, customField.Value);
                }
            }

            return strs;
        }

        public string GetCustomFieldValue(string sCustomFieldName)
        {
            if (base._License.Content.CustomFields != null)
            {
                CustomField[] fields = base._License.Content.CustomFields.Fields;
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    CustomField customField = fields[i];
                    if (string.Compare(customField.Name, sCustomFieldName, true) == 0)
                    {
                        return customField.Value;
                    }
                }
            }

            return null;
        }

        public string[] GetLicenseCustomInfo()
        {
            List<string> strs = new List<string>();
            if (base.KeyValue.Product != Product.EnterpriseMigrationKey)
            {
                foreach (KeyValuePair<string, string> customField in this.GetCustomFields())
                {
                    string str = string.Concat("Lic_", customField.Key);
                    string resourceString = SystemUtils.GetResourceString(str);
                    string resourceString1 = SystemUtils.GetResourceString(string.Concat(str, "_", customField.Value));
                    if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(resourceString1))
                    {
                        continue;
                    }

                    strs.Add(string.Format("{0}: {1}", resourceString, resourceString1));
                }
            }
            else
            {
                strs.Add(SystemUtils.GetResourceString("License_Allowed_Editions"));
                foreach (Product allowedProduct in this.AllowedProducts)
                {
                    if (allowedProduct == this._settings.Product)
                    {
                        continue;
                    }

                    strs.Add(string.Format("   {0}",
                        SystemUtils.GetResourceString(string.Concat("Lic_", (int)allowedProduct))));
                }
            }

            return strs.ToArray();
        }

        public string[] GetLicenseInfo()
        {
            List<string> strs = new List<string>();
            string[] strArrays = new string[]
            {
                string.Concat("License: ", this.Type.ToString()), string.Concat("Key: ", this.LicenseKey.ToString())
            };
            strs.AddRange(strArrays);
            if (!string.IsNullOrEmpty(this.ContactName))
            {
                string[] strArrays1 = new string[]
                {
                    "Registered To: ", string.Concat("   Name: ", this.ContactName),
                    string.Concat("   Email: ", this.ContactEmail),
                    string.Concat("   Organization: ", this.CustomerName)
                };
                strs.AddRange(strArrays1);
            }

            List<string> strs1 = strs;
            string[] strArrays2 = new string[]
            {
                string.Concat("Installation ID: ", MigrationLicense.ServerID), null, null, null, null, null, null, null,
                null
            };
            DateTime expiration = this.Expiration;
            strArrays2[1] = string.Concat("Expiry Date: ", expiration.ToString("dd-MMM-yyyy"));
            DateTime maintenanceExpirationDate = this.MaintenanceExpirationDate;
            strArrays2[2] = string.Concat("Maintenance Expiry Date: ",
                maintenanceExpirationDate.ToString("dd-MMM-yyyy"));
            strArrays2[3] = string.Concat("Servers Used: ",
                (this.LicensedServers >= 0
                    ? string.Format("{0} of {1}", this.UsedServers, this.LicensedServers)
                    : "Unlimited"));
            strArrays2[4] = string.Concat("Admin Seats Used: ",
                (this.LicensedAdmins >= 0
                    ? string.Format("{0} of {1}", this.UsedAdmins, this.LicensedAdmins)
                    : "Unlimited"));
            strArrays2[5] = string.Concat("Licensed Size: ",
                (this.LicensedData >= (long)0 ? this.FormatUsageData(this.LicensedData) : "Unlimited"));
            strArrays2[6] = string.Concat("Licensed Data Used: ",
                (this.IsLegacyProduct || this.IsContentMatrixExpress
                    ? this.FormatUsageData(this.UsedDataFull)
                    : this.FormatUsageData(this.UsedLicensedData)));
            strArrays2[7] = string.Concat("Is Legacy Key: ", (this.IsLegacyProduct ? "Yes" : "No"));
            strArrays2[8] = string.Concat("Is Express Edition: ", (this.IsContentMatrixExpress ? "Yes" : "No"));
            strs1.AddRange(strArrays2);
            return strs.ToArray();
        }

        public MLLicense GetMLLicense(MLLicenseProviderCommon provider)
        {
            return new MLLicenseCommon(this, provider);
        }

        public void IncrementLicenseUseValues(long lDataUsedAdded, long lItemsUsedAdded)
        {
            lock (this.m_oLockUsage)
            {
                long usedItems = this._usageFile.UsedItems + lItemsUsedAdded;
                long usedData = this._usageFile.UsedData + lDataUsedAdded;
                if (usedItems < (long)0 || usedItems < this._usageFile.UsedItems)
                {
                    throw new ArgumentException(
                        "UsedItemCount cannot have negative value or cannot be smaller than the previous value.");
                }

                this._usageFile.UsedItems = usedItems;
                if (usedData < (long)0 || usedData < this._usageFile.UsedData)
                {
                    throw new ArgumentException(
                        "UsedData cannot have negative value or cannot be smaller than the previous value.");
                }

                this._usageFile.UsedData = usedData;
            }
        }

        protected override void Initialize()
        {
            throw new NotSupportedException();
        }

        private void InitializeLicense()
        {
            base._License = new LicenseFile(
                new FileDataStorage(this._settings.LicenseFilePath, this._settings.SecureStorage,
                    this._settings.LicenseFileAccess, "MetalogixLicenseFileStorageMutex"),
                "<RSAKeyValue><Modulus>zBQqYBd1a2ck3QAtTRBls4xAaDuhU+QLrJ6wLYlg3KN+tQhLKrTtG6cItOWxQwcRmnFoV7bCuZ5ICMgc+ProYPUW9xEe5SFKsWX7wOai9aAQwpjM3Un1dSCfmLjAADmB32DHnOxZhXB4c6U8XJPK+SANWQtTn3De4aZQ702uxQ8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
        }

        private void InitializeUsage(string licenseKey, bool allowConversion, bool allowCounterReset)
        {
            lock (this.m_oLockUsage)
            {
                this._usageFile = new DynamicContentFile(
                    new FileDataStorage(this._settings.UsageFilePath, this._settings.SecureStorage,
                        this._settings.UsageFileAccess, "MetalogixDynamicContentFileStorageMutex"),
                    LicensingUtils.GetUsageEntryID(licenseKey, this._settings.Product),
                    "MetalogixDynamicContentFileMutex");
                bool flag = (!allowCounterReset || base._License.Content == null
                    ? false
                    : base._License.Content.UsedData == (long)0);
                if (allowConversion && !string.IsNullOrEmpty(this._settings.UsageFilePathOld) &&
                    File.Exists(this._settings.UsageFilePathOld) && !this._usageFile.DataExistsInStorage && !flag)
                {
                    Logger.Debug.Write("MigrationLicense >> InitializeUsage >> Converting usage file.");
                    try
                    {
                        using (DynamicContentFile dynamicContentFile = new DynamicContentFile(
                                   new FileDataStorage(this._settings.UsageFilePathOld, this._settings.SecureStorage,
                                       this._settings.UsageFileAccess, "MetalogixDynamicContentFileStorageMutex"), null,
                                   "MetalogixDynamicContentFileMutex"))
                        {
                            ILogMethods debug = Logger.Debug;
                            object[] usedData = new object[]
                            {
                                dynamicContentFile.UsedData, dynamicContentFile.UsedItems,
                                this._settings.UsageFilePathOld, this._settings.UsageFilePath
                            };
                            debug.WriteFormat(
                                "MigrationLicense >> InitializeUsage >> Moving usage ({0},{1}) info from '{2}' to '{3}'.",
                                usedData);
                            this._usageFile.UsedData = dynamicContentFile.UsedData;
                            this._usageFile.UsedItems = dynamicContentFile.UsedItems;
                            this._usageFile.Save();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Error.Write("MigrationLicense >> InitializeUsage >> Failed to convert the usage file.",
                            exception);
                    }
                }

                if (flag)
                {
                    this._usageFile.UsedData = (long)0;
                    this._usageFile.UsedItems = (long)0;
                    this._usageFile.Save();
                }
            }
        }

        private void ParseLicenseContent()
        {
            bool flag;
            Metalogix.Licensing.LicensedSharePointVersions allowedVersions =
                this.GetAllowedVersions("SharePointVersions");
            if (allowedVersions != Metalogix.Licensing.LicensedSharePointVersions.All)
            {
                this._allowedVersions = allowedVersions;
            }

            allowedVersions = this.GetAllowedVersions("NewSharePointVersions");
            if (allowedVersions != Metalogix.Licensing.LicensedSharePointVersions.All)
            {
                this._allowedVersions |= allowedVersions;
            }

            string customFieldValue = this.GetCustomFieldValue("SharePointOnline");
            bool.TryParse(customFieldValue, out flag);
            if (flag)
            {
                this._allowedVersions |= Metalogix.Licensing.LicensedSharePointVersions.SPOnline;
            }
        }

        public override void Save()
        {
            this.SaveUsageData();
            base.Save();
        }

        public void SaveUsageData()
        {
            //lock (this.m_oLockUsage)
            //{
            //	this._usageFile.Save();
            //}
        }

        public override string ToString()
        {
            object[] status = new object[]
            {
                base._License.License.Status, base._License.License.Type, base._License.License.LicensedData,
                base._License.License.UsedData, base._License.License.LicenseExpiration,
                base._License.License.MaintenanceExpiration
            };
            return string.Format(
                "Status: {0}\r\nType: {1}\r\nLicensed Data: {2}\r\nUsed Data: {3}\r\nExpiration: {4}\r\nMaintenanceExp: {5}",
                status);
        }

        public override void Validate()
        {
            /*if (this.IsLegacyProduct || this.IsContentMatrixExpress)
            {
                this.Validate(this._settings.LegacyProduct, MigrationLicense.AdminAccount, MigrationLicense.ServerID);
                if (base._License.License.LicensedData != (long)-1 && this.UsedDataFull > base._License.License.LicensedData)
                {
                    Logger.Warning.Write("Full licensed data (including OSD) checking failed.");
                    throw new LicenseExpiredException(Resources.Licensed_Data_Exceeded);
                }
            }
            else
            {
                this.Validate(this._settings.Product, MigrationLicense.AdminAccount, MigrationLicense.ServerID);
                if (base._License.License.LicensedData != (long)-1 && this.UsedLicensedData > base._License.License.LicensedData)
                {
                    Logger.Warning.Write("Full licensed data (including OSD) checking failed.");
                    this.m_bDataLimitExceeded = true;
                }
                if (base._License.License.Type != LicenseType.Trial)
                {
                    System.Version version = new System.Version(base._License.License.MaxAllowedSetupVersion);
                    Logger.Debug.Write(string.Format("Checking product version against the maintenance: productVersion={0}; maintenanceVersion={1}", this._settings.ProductVersion, version));
                    if (this._settings.ProductVersion > version)
                    {
                        Logger.Warning.Write("Maintenance checking failed, you cannot use this version of the setup with a current license key.");
                        throw new InvalidLicenseException("The current setup version is not allowed to use with this license, please extend your maintenance period.");
                    }
                }
            }*/
        }

        public void Validate(MigrationLicense currentLicense)
        {
            this.Validate();
            if (currentLicense == null)
            {
                return;
            }

            if (currentLicense._License.License.Type == LicenseType.Trial &&
                base._License.License.Type == LicenseType.Trial && string.Compare(currentLicense.LicenseKey,
                    this.LicenseKey, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new InvalidLicenseException("Updating from Trial license to a Trial license is not allowed.");
            }
        }

        protected override void Validate(Product product, string adminAccount, string serverID)
        {
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { product, adminAccount, serverID, null };
            objArray[3] = (base.Exists ? this.LicenseKey : "NULL");
            debug.WriteFormat("AbstractLicense >> Validate: Entered, Product={0}; Admin={1}; Server={2}; Key={3}",
                objArray);
            if (!base.Exists)
            {
                Logger.Warning.Write("AbstractLicense >> Validate: License doesn't exists");
                throw new InvalidLicenseException("License not exists.");
            }

            if (!base.KeyValue.IsValid)
            {
                ILogMethods warning = Logger.Warning;
                object[] key = new object[] { base._License.License.Key };
                warning.WriteFormat("AbstractLicense >> Validate: Key not valid Key={0};", key);
                throw new InvalidLicenseException("License key is invalid.");
            }

            if (!this.ValidateProduct(product))
            {
                throw new InvalidLicenseException("License key is not valid for the given product.");
            }

            if (base.KeyValue.Product == Product.UnifiedContentMatrixExpressKey &&
                product != Product.UnifiedContentMatrixExpressKey && product != Product.UnifiedContentMatrixKey &&
                product != Product.EnterpriseMigrationKey && !this.AllowedProducts.Contains(product.GetLegacyProduct()))
            {
                ILogMethods logMethod = Logger.Warning;
                object[] objArray1 = new object[] { product, base.KeyValue.Product };
                logMethod.WriteFormat(
                    "AbstractLicense >> Validate: ContentMatrixKey's product is not valid RequiredProduct={0}; KeyProduct={1}",
                    objArray1);
                throw new InvalidLicenseException("License key is not valid for the given product.");
            }

            if (base._License.License.Status == LicenseStatus.Disabled)
            {
                Logger.Warning.Write("AbstractLicense >> Validate: Key was disabled");
                throw new InvalidLicenseException(
                    "License key was disabled on the license server, please contact the support for details.");
            }

            if (base._License.License.Status == LicenseStatus.Expired)
            {
                ILogMethods warning1 = Logger.Warning;
                object[] licenseExpiration = new object[] { base._License.License.LicenseExpiration };
                warning1.WriteFormat("AbstractLicense >> Validate: Key has expired on {0};", licenseExpiration);
                throw new LicenseExpiredException(Resources.License_Expired);
            }

            if (base._License.License.Status != LicenseStatus.Valid)
            {
                ILogMethods logMethod1 = Logger.Warning;
                object[] status = new object[] { base._License.License.Status };
                logMethod1.WriteFormat("AbstractLicense >> Validate: Key has invalid status {0};", status);
                throw new InvalidLicenseException("License key has invalid status.");
            }

            if (base._License.License.LicenseExpiration < DateTime.Now.ToUniversalTime())
            {
                ILogMethods warning2 = Logger.Warning;
                object[] licenseExpiration1 = new object[] { base._License.License.LicenseExpiration };
                warning2.WriteFormat("AbstractLicense >> Validate: Key has expired on {0} because of the local time;",
                    licenseExpiration1);
                throw new LicenseExpiredException(Resources.License_Expired);
            }

            if (base._License.License.LicensedAdmins != -1 &&
                base._License.License.UsedAdmins > base._License.License.LicensedAdmins)
            {
                ILogMethods logMethod2 = Logger.Warning;
                object[] usedAdmins = new object[]
                    { base._License.License.UsedAdmins, base._License.License.LicensedAdmins };
                logMethod2.WriteFormat(
                    "AbstractLicense >> Validate: Admin seats count reached the allowed: Used={0}; Allowed={1}",
                    usedAdmins);
                throw new InvalidLicenseException("You have reached the allowed administrator seats count.");
            }

            if (base._License.License.LicensedServers != -1 &&
                base._License.License.UsedServers > base._License.License.LicensedServers)
            {
                ILogMethods warning3 = Logger.Warning;
                object[] usedServers = new object[]
                    { base._License.License.UsedServers, base._License.License.LicensedServers };
                warning3.WriteFormat(
                    "AbstractLicense >> Validate: Servers count reached the allowed: Used={0}; Allowed={1}",
                    usedServers);
                throw new InvalidLicenseException("You have reached the allowed server installation count.");
            }

            if (base._License.License.LicensedData != (long)-1 &&
                base._License.License.UsedData > base._License.License.LicensedData)
            {
                ILogMethods logMethod3 = Logger.Warning;
                object[] usedData = new object[] { base._License.License.UsedData, base._License.License.LicensedData };
                logMethod3.WriteFormat(
                    "AbstractLicense >> Validate: Used data reached the allowed: Used={0}; Allowed={1}", usedData);
                this.m_bDataLimitExceeded = true;
            }

            ILogMethods debug1 = Logger.Debug;
            object[] objArray2 = new object[] { (base._License.License.LicensedServers == -1 ? "NOT " : "") };
            debug1.WriteFormat("AbstractLicense >> Validate: Server is {0}validated.", objArray2);
            ILogMethods debug2 = Logger.Debug;
            object[] objArray3 = new object[] { (base._License.License.LicensedAdmins == -1 ? "NOT " : "") };
            debug2.WriteFormat("AbstractLicense >> Validate: Admin is {0}validated.", objArray3);
            if (base._License.License.LicensedServers != -1 &&
                !LicensingUtils.IdEquals(base._License.ServerID, serverID))
            {
                ILogMethods warning4 = Logger.Warning;
                object[] objArray4 = new object[] { serverID, base._License.ServerID };
                warning4.WriteFormat("AbstractLicense >> Validate: Server name checking failed: Used={0}; Required={1}",
                    objArray4);
                throw new InvalidLicenseException("License key is not valid on the given server.");
            }

            if (base._License.License.LicensedAdmins != -1 &&
                string.CompareOrdinal(base._License.AdminAccount, adminAccount) != 0)
            {
                ILogMethods logMethod4 = Logger.Warning;
                object[] objArray5 = new object[] { adminAccount, base._License.AdminAccount };
                logMethod4.WriteFormat(
                    "AbstractLicense >> Validate: Admin name checking failed: Used={0}; Required={1}", objArray5);
                throw new InvalidLicenseException("License key is not valid with the given user account.");
            }

            DateTime now = DateTime.Now;
            TimeSpan universalTime = now.ToUniversalTime() - base._License.Content.Updated;
            if (base._License.License.OfflineRevalidationDays != -1 &&
                universalTime.TotalDays > (double)base._License.License.OfflineRevalidationDays)
            {
                ILogMethods warning5 = Logger.Warning;
                object[] updated = new object[]
                {
                    base._License.Content.Updated, base._License.License.OfflineRevalidationDays,
                    universalTime.TotalDays
                };
                warning5.WriteFormat(
                    "AbstractLicense >> Validate: License revalidation is required, because the allowed offline date is off: LastSync={0}; MaxOfflineDays={1}; CurrentOfflineDays",
                    updated);
                throw new InvalidLicenseException(Resources.License_Offline_Validation_Exceeded);
            }
        }

        public void ValidateOffline(MigrationLicense currentLicense)
        {
            this.Validate(currentLicense);
            if (base._License.Content.Updated > DateTime.Now.ToUniversalTime().AddDays(7))
            {
                throw new InvalidLicenseException("The offline activation file cannot be older than 7 days.");
            }
        }

        protected bool ValidateProduct(Product product)
        {
            if (base.KeyValue.Product != Product.EnterpriseMigrationKey &&
                base.KeyValue.Product != Product.UnifiedContentMatrixKey &&
                base.KeyValue.Product != Product.UnifiedContentMatrixExpressKey && !base.KeyValue.Validate(product))
            {
                ILogMethods warning = Logger.Warning;
                object[] objArray = new object[] { product, base.KeyValue.Product };
                warning.WriteFormat(
                    "AbstractLicense >> Validate: Key's product is not valid RequiredProduct={0}; KeyProduct={1}",
                    objArray);
                return false;
            }

            if (base.KeyValue.Product == Product.EnterpriseMigrationKey && product != Product.EnterpriseMigrationKey &&
                !this.AllowedProducts.Contains(product.GetLegacyProduct()))
            {
                ILogMethods logMethod = Logger.Warning;
                object[] objArray1 = new object[] { product, base.KeyValue.Product };
                logMethod.WriteFormat(
                    "AbstractLicense >> Validate: EnterpriseKey's product is not valid RequiredProduct={0}; KeyProduct={1}",
                    objArray1);
                return false;
            }

            if (base.KeyValue.Product != Product.UnifiedContentMatrixKey ||
                product == Product.UnifiedContentMatrixKey || this.AllowedProducts.Contains(product.GetModernProduct()))
            {
                return true;
            }

            ILogMethods warning1 = Logger.Warning;
            object[] objArray2 = new object[] { product, base.KeyValue.Product };
            warning1.WriteFormat(
                "AbstractLicense >> Validate: ContentMatrixKey's product is not valid RequiredProduct={0}; KeyProduct={1}",
                objArray2);
            return false;
        }
    }
}