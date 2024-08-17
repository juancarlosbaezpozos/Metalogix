using Metalogix.Licensing;
using System;

namespace Metalogix.Licensing.SK
{
    public sealed class LicenseProviderInitializationDataSK : LicenseProviderInitializationData
    {
        private Products _product;

        private string _licenseServerUrl;

        private string _checkSumKeyName;

        private string _installInfoHiddenKey;

        private string _licenseRegistryBase;

        private int _licenseCheckingPeriod;

        private int _trialItemLimit;

        private string _setLicenseText;

        public string CheckSumKeyName
        {
            get { return this._checkSumKeyName; }
            set { this._checkSumKeyName = value; }
        }

        public string InstallInfoHiddenKey
        {
            get { return this._installInfoHiddenKey; }
            set { this._installInfoHiddenKey = value; }
        }

        public int LicenseCheckingPeriod
        {
            get { return this._licenseCheckingPeriod; }
            set { this._licenseCheckingPeriod = value; }
        }

        public string LicenseServerUrl
        {
            get { return this._licenseServerUrl; }
            set { this._licenseServerUrl = value; }
        }

        public Products Product
        {
            get { return this._product; }
            set { this._product = value; }
        }

        public string RegistryBase
        {
            get { return this._licenseRegistryBase; }
            set { this._licenseRegistryBase = value; }
        }

        public string SetLicenseText
        {
            get { return this._setLicenseText; }
            set { this._setLicenseText = value; }
        }

        public int TrialItemLimit
        {
            get { return this._trialItemLimit; }
            set { this._trialItemLimit = value; }
        }

        public LicenseProviderInitializationDataSK() : base(LicenseProviderType.SK)
        {
        }
    }
}