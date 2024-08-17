using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Utilities;
using System;

namespace Metalogix.Licensing.Common
{
    public class MLLicenseCommon : MLLicense
    {
        private readonly MigrationLicense _license;

        private readonly MLLicenseProviderCommon _provider;

        public bool DataLimitExceeded
        {
            get { return this._license.DataLimitedExceeded; }
        }

        public override string Email
        {
            get { return this._license.ContactEmail; }
        }

        public DateTime ExpirationDate
        {
            get { return this._license.Expiration; }
        }

        public override DateTime ExpiryDate
        {
            get { return this._license.Expiration; }
        }

        public bool IsContentMatrixExpress
        {
            get { return this._license.IsContentMatrixExpress; }
        }

        public bool IsDataLimitExceededForContentUnderMgmt
        {
            get { return this._license.IsDataLimitExceededForContentUnderMgmt; }
        }

        public bool IsLegacyProduct
        {
            get { return this._license.IsLegacyProduct; }
        }

        public int LicensedAdmins
        {
            get { return this._license.LicensedAdmins; }
        }

        public long LicensedData
        {
            get { return this._license.LicensedData; }
        }

        public int LicensedServers
        {
            get { return this._license.LicensedServers; }
        }

        public Metalogix.Licensing.LicensedSharePointVersions LicensedSharePointVersions
        {
            get { return this._license.LicensedSharePointVersions; }
        }

        public override string LicenseKey
        {
            get { return this._license.LicenseKey; }
        }

        public override MLLicenseType LicenseType
        {
            get
            {
                switch (this._license.Type)
                {
                    case Metalogix.Licensing.LicenseServer.Service.LicenseType.Trial:
                    {
                        return MLLicenseType.Evaluation;
                    }
                    case Metalogix.Licensing.LicenseServer.Service.LicenseType.Perpetual:
                    {
                        return MLLicenseType.Commercial;
                    }
                    case Metalogix.Licensing.LicenseServer.Service.LicenseType.Term:
                    {
                        return MLLicenseType.Commercial;
                    }
                }

                throw new Exception(string.Concat("Invalid license type: ", this._license.Type));
            }
        }

        public DateTime MaintenanceExpirationDate
        {
            get { return this._license.MaintenanceExpirationDate; }
        }

        public override string Name
        {
            get { return this._license.ContactName; }
        }

        public int OfflineValidityDays
        {
            get { return this._license.OfflineValidityDays; }
        }

        public override string Organization
        {
            get { return this._license.CustomerName; }
        }

        public Metalogix.Licensing.LicenseServer.Product Product
        {
            get { return this._license.KeyValue.Product; }
        }

        public MLLicenseProviderCommon Provider
        {
            get { return this._provider; }
        }

        public string ServerID
        {
            get { return MigrationLicense.ServerID; }
        }

        public bool SkipWebChecking
        {
            get { return this._license.SkipWebChecking; }
        }

        public Metalogix.Licensing.LicenseServer.Product SpecificProduct
        {
            get { return this._license.SpecificProduct; }
        }

        public ProductFlags SpecificProductFlags
        {
            get { return this._license.SpecificProduct.GetFlagsFromProduct(); }
        }

        public double UsagePercentage
        {
            get { return this._license.UsagePercentage; }
        }

        public int UsedAdmins
        {
            get { return this._license.UsedAdmins; }
        }

        public long UsedData
        {
            get { return this._license.UsedData; }
        }

        public long UsedDataFull
        {
            get { return this._license.UsedDataFull; }
        }

        public long UsedItemCount
        {
            get { return this._license.UsedItemCount; }
        }

        public long UsedLicensedData
        {
            get { return this._license.UsedLicensedData; }
        }

        public int UsedServers
        {
            get { return this._license.UsedServers; }
        }

        internal MLLicenseCommon(MigrationLicense license, MLLicenseProviderCommon provider)
        {
            if (license == null)
            {
                throw new ArgumentNullException("license");
            }

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._license = license;
            this._provider = provider;
        }

        public override void Dispose()
        {
        }

        public string FormatUsageData(long data)
        {
            if (this.Provider.Settings.LicenseDataFormatter == null)
            {
                return string.Format("{0} {1}", data, this.Provider.Settings.DataUnitName);
            }

            return this.Provider.Settings.LicenseDataFormatter.FormatData(new long?(data),
                this.Provider.Settings.DataUnitName);
        }

        public string GetCustomFieldValue(string sCustomFieldName)
        {
            return this._license.GetCustomFieldValue(sCustomFieldName);
        }

        public override string[] GetLicenseCustomInfo()
        {
            return this._license.GetLicenseCustomInfo();
        }

        public override string[] GetLicenseInfo()
        {
            return this._license.GetLicenseInfo();
        }

        public string GetLicenseTypeString()
        {
            return this._license.Type.ToString();
        }

        public void IncrementLicenseUseValues(long lDataUsedAdded, long lItemsUsedAdded)
        {
            this._license.IncrementLicenseUseValues(lDataUsedAdded, lItemsUsedAdded);
        }

        public void SaveUsageData()
        {
            this._license.SaveUsageData();
        }

        public override void Validate(string sProductCode)
        {
            this._license.Validate();
        }
    }
}