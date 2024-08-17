using Metalogix.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Metalogix.Licensing.SK
{
    public sealed class MLLicenseSK : MLLicenseSKBase
    {
        private MLLicenseType? _lastType;

        public override string Email
        {
            get { return ""; }
        }

        public override DateTime ExpiryDate
        {
            get
            {
                if (this.LicenseType == MLLicenseType.Evaluation)
                {
                    return this._installInfo.ExpirationDate;
                }

                if (this._status == null)
                {
                    return DateTime.MaxValue;
                }

                return this.StatusInternal.ExpirationDate;
            }
        }

        public Metalogix.Licensing.SK.LicenseStatus LicenseStatus
        {
            get { return this.StatusInternal; }
        }

        public override string Name
        {
            get { return ""; }
        }

        public override string Organization
        {
            get { return ""; }
        }

        private Metalogix.Licensing.SK.LicenseStatus StatusInternal
        {
            get { return this._status as Metalogix.Licensing.SK.LicenseStatus; }
            set { this._status = value; }
        }

        public MLLicenseSK()
        {
            this.StatusInternal = new Metalogix.Licensing.SK.LicenseStatus();
            this.StatusInternal.StatusChanged += new EventHandler(this._status_StatusChanged);
        }

        public MLLicenseSK(string key) : base(key)
        {
        }

        private void _status_StatusChanged(object sender, EventArgs e)
        {
            this.CheckForStatusChanged();
        }

        private void CheckForStatusChanged()
        {
            if (!this._lastType.HasValue)
            {
                this._lastType = new MLLicenseType?(this.LicenseType);
                return;
            }

            MLLicenseType? nullable = this._lastType;
            MLLicenseType licenseType = this.LicenseType;
            if ((nullable.GetValueOrDefault() != licenseType ? true : !nullable.HasValue))
            {
                this._lastType = new MLLicenseType?(this.LicenseType);
                base.FireStatusChanged();
            }
        }

        private static Metalogix.Licensing.SK.LicenseStatus CheckLicenseKeyOnLine(MLLicenseSK lic, LicenseProxy proxy)
        {
            if (lic == null)
            {
                throw new ArgumentNullException("lic");
            }

            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            if (!lic.IsSet)
            {
                throw new Exception("License key not set.");
            }

            object[] licenseServerUrl = new object[]
            {
                SKLP.Get.InitData.LicenseServerUrl, lic.LicenseKey,
                Convert.ToInt32(lic.StatusInternal.MigratedByteSize / (long)1073741824), MLLicenseSK.GetIPAddress()
            };
            string str = string.Format("{0}?KEYVALUE={1}&MBX={2}&IP={3}", licenseServerUrl);
            Trace.WriteLine(string.Concat("MLLicenseProviderSK >> CheckLicenseKeyOnLine: start ", str));
            WebClient webClient = new WebClient();
            if (!proxy.Enabled)
            {
                WebRequest.DefaultWebProxy = null;
                Trace.WriteLine("MLLicenseProviderSK >> CheckLicenseKeyOnLine>> no proxy server.");
            }
            else
            {
                string[] server = new string[]
                {
                    "MLLicenseProviderSK >> CheckLicenseKeyOnLine >> Trying proxy server = ", proxy.Server, "; Port = ",
                    proxy.Port, "; User = ", proxy.User
                };
                Trace.WriteLine(string.Concat(server));
                WebProxy webProxy = new WebProxy(string.Concat("http://", proxy.Server, ":", proxy.Port), true)
                {
                    Credentials = new NetworkCredential(proxy.User, proxy.Pass)
                };
                WebRequest.DefaultWebProxy = webProxy;
                object[] address = new object[]
                {
                    "MLLicenseProviderSK >> CheckLicenseKeyOnLine >> Using proxy >> Server = ", webProxy.Address,
                    "; User = ", proxy.User
                };
                Trace.WriteLine(string.Concat(address));
            }

            webClient.Credentials = CredentialCache.DefaultCredentials;
            byte[] numArray = webClient.DownloadData(str);
            if (numArray == null)
            {
                Trace.WriteLine(
                    "MLLicenseProviderSK >> CheckLicenseKeyOnLine >> Error: Can not download data from license server ");
                throw new Exception("Can not download data from license server.");
            }

            string str1 = Encoding.ASCII.GetString(numArray);
            Trace.WriteLine(string.Concat("License Manager >> CheckLicenseKeyOnLine >> ", str1));
            DateTime now = DateTime.Now;
            str1 = string.Concat(str1, "|", now.ToString("yyyyMMdd"));
            Metalogix.Licensing.SK.LicenseStatus licenseStatu = new Metalogix.Licensing.SK.LicenseStatus(str1);
            licenseStatu.Load(true);
            if (licenseStatu.StatusCode < SKLicenseStatus.Valid)
            {
                Trace.WriteLine(string.Format(
                    "MLLicenseProviderSK >> CheckLicenseKeyOnLine >> Error: incorrect server response '{0}'", str1));
                throw new Exception("Incorrect server response.");
            }

            Trace.WriteLine(string.Concat("MLLicenseProviderSK >> CheckLicenseKeyOnLine: finished successfully (",
                licenseStatu, ")"));
            return licenseStatu;
        }

        public override ILicenseStatus CheckOnline(LicenseProxy proxy)
        {
            return MLLicenseSK.CheckLicenseKeyOnLine(this, proxy);
        }

        public override void Dispose()
        {
        }

        private static string GetIPAddress()
        {
            List<string> strs = new List<string>();
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            for (int i = 0; i < (int)addressList.Length; i++)
            {
                strs.Add(addressList[i].ToString());
            }

            strs.Sort();
            string str = "";
            foreach (string str1 in strs)
            {
                str = string.Concat(str, str1, ",");
            }

            return str.TrimEnd(new char[] { ',' });
        }

        protected override MLLicenseType GetLicenseType(bool throwException)
        {
            if (this._type == MLLicenseType.Invalid)
            {
                if (this._installInfo == null)
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "Installation corrupted, installation informations are missing.");
                }

                if (this._installInfo == null || !(DateTime.Now.Date <= this._installInfo.ExpirationDate.Date))
                {
                    if (this._installInfo != null)
                    {
                        MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                            "Trial period has expired, please set a license key to continue.");
                    }

                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "Incorrect license key.");
                    return MLLicenseType.Invalid;
                }

                if (this._status != null && this.StatusInternal.MigratedItemsCount < SKLP.Get.InitData.TrialItemLimit)
                {
                    return MLLicenseType.Evaluation;
                }

                MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                    "You have reached the trial item limit, please set a license key to continue.");
                return MLLicenseType.Invalid;
            }

            if (this._type == MLLicenseType.Invalid || this._status == null)
            {
                if (this._type == MLLicenseType.Invalid)
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "License is not valid.");
                }

                return this._type;
            }

            long licensedStorageOrAccounts = (long)this.StatusInternal.LicensedStorageOrAccounts * (long)1073741824;
            if (!this.StatusInternal.IsUnlimitedStorage &&
                this.StatusInternal.UsedStorageOrUsers > licensedStorageOrAccounts)
            {
                MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                    "You have reached the payed item limit, please set a license key to continue.");
                return MLLicenseType.Invalid;
            }

            switch (this._status.StatusCode)
            {
                case SKLicenseStatus.Valid:
                {
                    return MLLicenseType.Commercial;
                }
                case SKLicenseStatus.Invalid:
                case SKLicenseStatus.Unreserved:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was determined as invalid by the license server.");
                    return MLLicenseType.Invalid;
                }
                case SKLicenseStatus.Disabled:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was disabled on the license server, please contact the support for details.");
                    return MLLicenseType.Invalid;
                }
                case SKLicenseStatus.Expired:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "License key has expired.");
                    return MLLicenseType.Invalid;
                }
                case SKLicenseStatus.Evaluation:
                {
                    return MLLicenseType.Evaluation;
                }
                default:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was determined as invalid by the license server.");
                    return MLLicenseType.Invalid;
                }
            }
        }

        public override void Load()
        {
            try
            {
                this._installInfo.Load();
                this.StatusInternal.Load();
                string str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseKey") as string;
                if (str != null)
                {
                    base.SetKey(str);
                }
                else
                {
                    Trace.WriteLine(string.Format("MLLicenseSK >> Load: Registry key '{0}' doesn`t extists.",
                        string.Concat(SKLP.Get.InitData.RegistryBase, "\\LicenseKey")));
                }
            }
            catch (LicenseHackedException licenseHackedException)
            {
                throw;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("MLLicenseSK >> Load: ", exception));
                throw new Exception("Unable to load license key.", exception);
            }
        }

        public override void Save()
        {
            try
            {
                this.StatusInternal.Save();
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseKey", (base.IsSet ? this.LicenseKey : ""));
                Trace.WriteLine("MLLicenseSK >> Save >> License key were set successfully.");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("MLLicenseSK >> Unable to set the license key. ", exception));
                throw new Exception("Unable to save license key.", exception);
            }
        }

        internal override void SetStatus(ILicenseStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException("Status");
            }

            if (!(status is Metalogix.Licensing.SK.LicenseStatus))
            {
                throw new Exception("Incorrect license status.");
            }

            if (this.StatusInternal != null)
            {
                this.StatusInternal.StatusChanged -= new EventHandler(this._status_StatusChanged);
            }

            this.StatusInternal = (Metalogix.Licensing.SK.LicenseStatus)status;
            this.StatusInternal.StatusChanged += new EventHandler(this._status_StatusChanged);
            this.StatusInternal.Synchronized = true;
            this.CheckForStatusChanged();
        }

        internal override void SetStatus(ILicenseStatus status, bool isNewStatus)
        {
            if (!(status is Metalogix.Licensing.SK.LicenseStatus))
            {
                throw new Exception("Incorrect license status.");
            }

            if (isNewStatus)
            {
                ((Metalogix.Licensing.SK.LicenseStatus)status).MigratedByteSize = (long)0;
                ((Metalogix.Licensing.SK.LicenseStatus)status).MigratedItemsCount = 0;
            }

            this.SetStatus(status);
        }
    }
}