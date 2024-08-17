using Metalogix;
using Metalogix.Licensing;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace Metalogix.Licensing.SK
{
    public sealed class MLLicenseProviderSK : ILicenseProvider, IDisposable
    {
        private System.Timers.Timer _timer;

        private LicenseProxy _proxy;

        private MLLicenseSKBase _license;

        private readonly LicenseProviderInitializationDataSK _initData;

        private static bool? _isRestrictionDisabled;

        public LicenseProviderInitializationDataSK InitData
        {
            get { return this._initData; }
        }

        public static bool IsRestrictionDisabled
        {
            get
            {
                bool flag;
                if (!MLLicenseProviderSK._isRestrictionDisabled.HasValue)
                {
                    Configuration configuration =
                        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (configuration.AppSettings.Settings["LicenseRestrictionDisabled"] == null ||
                        !bool.TryParse(configuration.AppSettings.Settings["LicenseRestrictionDisabled"].Value,
                            out flag) || !flag)
                    {
                        MLLicenseProviderSK._isRestrictionDisabled = new bool?(false);
                    }
                    else
                    {
                        MLLicenseProviderSK._isRestrictionDisabled = new bool?(true);
                    }
                }

                bool? nullable = MLLicenseProviderSK._isRestrictionDisabled;
                if ((!nullable.GetValueOrDefault() ? true : !nullable.HasValue))
                {
                    return false;
                }

                return true;
            }
        }

        public LicenseProxy Proxy
        {
            get { return this._proxy; }
        }

        public static string RegistrySoftwareBase
        {
            get
            {
                if (IntPtr.Size == 8)
                {
                    return "SOFTWARE\\Wow6432Node\\";
                }

                return "SOFTWARE\\";
            }
        }

        public MLLicenseProviderSK(LicenseProviderInitializationData data)
        {
            if (data == null || !(data is LicenseProviderInitializationDataSK))
            {
                throw new Exception("Incorrect initialization data type");
            }

            this._initData = (LicenseProviderInitializationDataSK)data;
            this._proxy = new LicenseProxy();
            this._license = this.CreateLicense(null);
            this._license.StatusChanged += new EventHandler(this._license_StatusChanged);
        }

        private void _license_StatusChanged(object sender, EventArgs e)
        {
            this.FireUpdated();
        }

        public MLLicenseSKBase CreateLicense(string key)
        {
            if (this._initData.Product != Products.BPOSEX && this._initData.Product != Products.BPOSML)
            {
                if (key == null)
                {
                    return new MLLicenseSK();
                }

                return new MLLicenseSK(key);
            }

            if (key == null)
            {
                return new BPOSLicense();
            }

            return new BPOSLicense(key);
        }

        private void CyclicSynchronization(object sender, ElapsedEventArgs e)
        {
            if (this._license == null || !this._license.IsSet)
            {
                return;
            }

            this._license.Save();
            this.SynchronizeWithLicenseServer();
        }

        public void Dispose()
        {
            try
            {
                this._timer.Stop();
                this._timer.Elapsed -= new ElapsedEventHandler(this.CyclicSynchronization);
                this._timer.Dispose();
                this._license.Save();
            }
            catch (Exception exception)
            {
                Logger.Error.Write(string.Concat("MLLicenseProviderSK >> Dispose: ", exception));
            }
        }

        private void FireUpdated()
        {
            if (this.LicenseUpdated != null)
            {
                this.LicenseUpdated(this, EventArgs.Empty);
            }
        }

        public MLLicenseSKBase GetLicense()
        {
            return this._license;
        }

        public License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            License license;
            string str;
            try
            {
                MLLicenseSKBase mLLicenseSKBase = this.GetLicense();
                if (mLLicenseSKBase == null)
                {
                    throw new LicenseException(type, instance, "License is not set.");
                }

                MLLicenseSKBase mLLicenseSKBase1 = mLLicenseSKBase;
                if (this.InitData != null)
                {
                    str = this.InitData.Product.ToString();
                }
                else
                {
                    str = null;
                }

                mLLicenseSKBase1.Validate(str);
                license = mLLicenseSKBase;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (context.UsageMode == LicenseUsageMode.Runtime && allowExceptions)
                {
                    throw new LicenseException(type, instance, exception.Message, exception);
                }

                return null;
            }

            return license;
        }

        public void Initialize()
        {
            this._proxy.Load();
            this._license.Load();
            this._timer = new System.Timers.Timer((double)this.InitData.LicenseCheckingPeriod);
            this._timer.Elapsed += new ElapsedEventHandler(this.CyclicSynchronization);
            this._timer.Start();
            (new Thread(() => this.CyclicSynchronization(null, null))).Start();
        }

        [Obsolete("Just here for ILicenseProvider support")]
        public bool IsSPOServerIdExist(string serverId)
        {
            return false;
        }

        public bool SetLicense(MLLicenseSKBase newLicense, LicenseProxy proxy)
        {
            string str;
            if (string.IsNullOrEmpty(newLicense.LicenseKey) || newLicense.LicenseType == MLLicenseType.Invalid)
            {
                throw new LicenseException(null, null, "Given license key is invalid.");
            }

            MLLicenseSKBase mLLicenseSKBase = newLicense;
            if (this.InitData != null)
            {
                str = this.InitData.Product.ToString();
            }
            else
            {
                str = null;
            }

            mLLicenseSKBase.Validate(str);
            if (newLicense.Status == null && this._license.Status != null)
            {
                newLicense.SetStatus(this._license.Status.Clone(), true);
            }

            newLicense.SetStatus(newLicense.CheckOnline(proxy), true);
            newLicense.Save();
            this.SetProxy(proxy);
            this._license.StatusChanged -= new EventHandler(this._license_StatusChanged);
            this._license = newLicense;
            this._license.StatusChanged += new EventHandler(this._license_StatusChanged);
            this.FireUpdated();
            return true;
        }

        public void SetProxy(LicenseProxy proxy)
        {
            proxy.Save();
            this._proxy = proxy;
        }

        private void SynchronizeWithLicenseServer()
        {
            try
            {
                ILicenseStatus licenseStatu = this._license.CheckOnline(this._proxy);
                lock (this._license)
                {
                    this._license.SetStatus(licenseStatu);
                    this._license.Save();
                }
            }
            catch (Exception exception)
            {
                Logger.Error.Write(
                    "MLLicenseProviderSK >> SynchronizeWithLicenseServer: failed to synchronize with license server, ",
                    exception);
            }
        }

        [Obsolete("Just here for ILicenseProvider support")]
        public bool Update()
        {
            return false;
        }

        [Obsolete("Just here for ILicenseProvider support")]
        public bool UpdateLicense(long usedData, string serverId, bool isSPO, string tenantUrlAndUser)
        {
            return false;
        }

        public event EventHandler LicenseUpdated;
    }
}