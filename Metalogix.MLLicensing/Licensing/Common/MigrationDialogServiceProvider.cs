using Metalogix.Connectivity.Proxy;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using System;

namespace Metalogix.Licensing.Common
{
    public class MigrationDialogServiceProvider : ILicensingDialogServiceProvider
    {
        private readonly MLLicenseProviderCommon _licProvider;

        public bool AllowSettingProxy
        {
            get { return true; }
        }

        public string DataUnitName
        {
            get { return this._licProvider.Settings.DataUnitName; }
        }

        public string DialogTitle
        {
            get { return this._licProvider.Settings.ProductName; }
        }

        public string LicenseKey
        {
            get { return this._licProvider.GetActualLicenseKey(); }
        }

        public string LicenseOfflineActivationURL
        {
            get { return this._licProvider.Settings.LicenseOfflineActivationURL; }
        }

        public Metalogix.Licensing.LicenseServer.Product Product
        {
            get { return this._licProvider.Settings.Product; }
        }

        internal MigrationDialogServiceProvider(MLLicenseProviderCommon licProvider)
        {
            if (licProvider == null)
            {
                throw new ArgumentNullException("licProvider");
            }

            this._licProvider = licProvider;
        }

        public void ActivateLicenseOffline(string activationRequest)
        {
            this._licProvider.ActivateOffline(activationRequest);
        }

        public static MigrationDialogServiceProvider CreateInstance()
        {
            MLLicenseProviderCommon instance = MLLicenseProvider.Instance as MLLicenseProviderCommon;
            if (instance == null)
            {
                return null;
            }

            return MigrationDialogServiceProvider.CreateInstance(instance);
        }

        public static MigrationDialogServiceProvider CreateInstance(MLLicenseProviderCommon licenseProvider)
        {
            return new MigrationDialogServiceProvider(licenseProvider);
        }

        public MLLicenseCommon GetLicenseInformation()
        {
            return (MLLicenseCommon)this._licProvider.GetLicense(null, null, null, true);
        }

        public MLProxy GetLicenseProxy()
        {
            return this._licProvider.GetLicenseProxy();
        }

        public string GetOfflineLicenseActivationData(string key, bool isOldStyle)
        {
            return this._licProvider.GetOfflineActivationData(key, isOldStyle);
        }

        public bool IsKeyLegitimate(string keyPrefix, string key)
        {
            Metalogix.Licensing.LicenseServer.LicenseKey licenseKey =
                new Metalogix.Licensing.LicenseServer.LicenseKey(key);
            if (licenseKey.Product == Metalogix.Licensing.LicenseServer.Product.EnterpriseMigrationKey ||
                licenseKey.Product == Metalogix.Licensing.LicenseServer.Product.UnifiedContentMatrixKey ||
                licenseKey.Product == Metalogix.Licensing.LicenseServer.Product.UnifiedContentMatrixExpressKey)
            {
                return licenseKey.IsValid;
            }

            if (licenseKey.Validate(this._licProvider.Settings.Product))
            {
                return true;
            }

            return licenseKey.Validate(this._licProvider.Settings.LegacyProduct);
        }

        public void SetLicenseProxy(MLProxy proxy)
        {
            this._licProvider.SetLicenseProxy(proxy);
        }

        public void UpdateLicenseKey(string key, bool isOldStyleLicense)
        {
            this._licProvider.UpdateLicenseKey(key, isOldStyleLicense);
        }
    }
}