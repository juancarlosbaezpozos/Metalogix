using Metalogix.Licensing;
using Metalogix.Licensing.Cryptography;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Metalogix.Licensing.SK
{
    public class LicenseStatus : ILicenseStatus
    {
        private const string _REGISTRY_KEY = "LicenseCheck";

        private const string _REGISTRY_OTHERDATA_KEY = "LicenseStatus";

        private const string _REGISTRY_HASH_BASE = "Microsoft";

        private const int _UNLIMITED_SIZE = 9999999;

        private DateTime _expires;

        private SKLicenseStatus _status = SKLicenseStatus.Invalid;

        private int _licensedSize;

        private int _usedSizeFromServer;

        private DateTime _lastConnect = DateTime.MinValue;

        private long _migratedByteSize;

        private int _migratedItemsCount;

        private long _bytesOutOfSync;

        private bool _isLoaded;

        public DateTime ExpirationDate
        {
            get { return this._expires; }
        }

        public bool IsLoaded
        {
            get { return this._isLoaded; }
        }

        public bool IsUnlimitedStorage
        {
            get { return this._licensedSize == 9999999; }
        }

        public DateTime LastConnectionDate
        {
            get { return this._lastConnect; }
        }

        public int LicensedStorageOrAccounts
        {
            get { return this._licensedSize; }
        }

        public long MigratedByteSize
        {
            get { return this._migratedByteSize; }
            set
            {
                if (this._migratedByteSize < value)
                {
                    LicenseStatus licenseStatu = this;
                    licenseStatu._bytesOutOfSync = licenseStatu._bytesOutOfSync + (value - this._migratedByteSize);
                }

                this._migratedByteSize = value;
                this.FireStatusChanged();
            }
        }

        public int MigratedItemsCount
        {
            get { return this._migratedItemsCount; }
            set { this._migratedItemsCount = value; }
        }

        internal string RawString
        {
            get
            {
                string[] str = new string[]
                {
                    string.Format("{0:0000}", this._expires.Year), string.Format("{0:00}", this._expires.Month),
                    string.Format("{0:00}", this._expires.Day), null, null, null, null, null, null, null
                };
                str[3] = this._status.ToString("");
                str[4] = string.Format("{0:0000000}", this._licensedSize);
                str[5] = string.Format("{0:0000000}", this._usedSizeFromServer);
                str[6] = "|";
                str[7] = string.Format("{0:0000}", this._lastConnect.Year);
                str[8] = string.Format("{0:00}", this._lastConnect.Month);
                str[9] = string.Format("{0:00}", this._lastConnect.Day);
                return string.Concat(str);
            }
            set
            {
                try
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] strArrays = value.Split(new char[] { '|' });
                        string str = strArrays[0];
                        int num = Convert.ToInt32(str.Substring(0, 4));
                        int num1 = Convert.ToInt32(str.Substring(4, 2));
                        int num2 = Convert.ToInt32(str.Substring(6, 2));
                        this._expires = new DateTime(num, num1, num2, 0, 0, 0, 0);
                        this._status = (SKLicenseStatus)Convert.ToInt32(str.Substring(8, 1));
                        this._licensedSize = Convert.ToInt32(str.Substring(9, 7));
                        this._usedSizeFromServer = Convert.ToInt32(str.Substring(16, 7));
                        if ((int)strArrays.Length <= 1)
                        {
                            this._lastConnect = DateTime.MinValue;
                        }
                        else
                        {
                            num = Convert.ToInt32(strArrays[1].Substring(0, 4));
                            num1 = Convert.ToInt32(strArrays[1].Substring(4, 2));
                            num2 = Convert.ToInt32(strArrays[1].Substring(6, 2));
                            this._lastConnect = new DateTime(num, num1, num2, 0, 0, 0, 0);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("LicenseStatus >> set_RawString: no key was set");
                        this.SetDefaults();
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(string.Concat("LicenseStatus >> set_RawString: ", exception));
                    this.SetDefaults();
                }
            }
        }

        public SKLicenseStatus StatusCode
        {
            get { return this._status; }
        }

        internal bool Synchronized
        {
            get { return this._bytesOutOfSync == (long)0; }
            set
            {
                if (value)
                {
                    this._bytesOutOfSync = (long)0;
                }
            }
        }

        public long UsedStorageOrUsers
        {
            get { return (long)this._usedSizeFromServer * (long)1073741824 + this._bytesOutOfSync; }
        }

        public LicenseStatus() : this("")
        {
        }

        public LicenseStatus(string text)
        {
            this.RawString = text;
        }

        private static string CalculateHash(string str)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(str);
            return BitConverter.ToString(mD5CryptoServiceProvider.ComputeHash(bytes));
        }

        public ILicenseStatus Clone()
        {
            return new LicenseStatus(this.RawString);
        }

        private void FireStatusChanged()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, EventArgs.Empty);
            }
        }

        internal void Load()
        {
            this.Load(false);
        }

        internal void Load(bool localOnly)
        {
            string str;
            try
            {
                this._isLoaded = false;
                this.LoadLocalData();
                if (!localOnly)
                {
                    string str1 = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                        string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                        "LicenseCheck") as string;
                    string str2 = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                        string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, "Microsoft"),
                        SKLP.Get.InitData.CheckSumKeyName) as string;
                    if (str1 != null || str2 != null)
                    {
                        if (str1 != null)
                        {
                            str = LicenseStatus.CalculateHash(str1);
                        }
                        else
                        {
                            str = null;
                        }

                        if (string.Compare(str, str2, StringComparison.Ordinal) != 0)
                        {
                            throw new LicenseHackedException("The application serial number was modified externally.");
                        }

                        this.RawString = Crypter.Decrypt(str1);
                        this._isLoaded = true;
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("LicenseStatus >> Load: Registry key '{0}' doesn`t extists.",
                            string.Concat(SKLP.Get.InitData.RegistryBase, "\\LicenseCheck")));
                    }
                }
            }
            catch (LicenseHackedException licenseHackedException)
            {
                throw;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("LicenseStatus >> Load: ", exception));
                throw new Exception("Unable to load license status", exception);
            }
        }

        private void LoadLocalData()
        {
            string str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                "LicenseStatus") as string;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    string[] strArrays = Crypter.Decrypt(str).Split(new char[] { '|' });
                    this._migratedByteSize = ((int)strArrays.Length > 0 ? Convert.ToInt64(strArrays[0]) : (long)0);
                    this._migratedItemsCount = ((int)strArrays.Length > 1 ? Convert.ToInt32(strArrays[1]) : 0);
                    this._bytesOutOfSync = ((int)strArrays.Length > 2 ? Convert.ToInt64(strArrays[2]) : (long)0);
                }
                catch
                {
                    this._migratedItemsCount = 0;
                    this._migratedByteSize = (long)0;
                    this._bytesOutOfSync = (long)0;
                }
            }
        }

        internal void Save()
        {
            try
            {
                string str = Crypter.Encrypt(this.RawString);
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseCheck", str);
                Trace.WriteLine("LicenseStatus >> Save >> Settings were saved successfully.");
                string str1 = LicenseStatus.CalculateHash(str);
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, "Microsoft"),
                    SKLP.Get.InitData.CheckSumKeyName, str1);
                object[] objArray = new object[]
                    { this._migratedByteSize, "|", this._migratedItemsCount, "|", this._bytesOutOfSync };
                string str2 = Crypter.Encrypt(string.Concat(objArray));
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseStatus", str2);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("LicenseStatus >> Unable to save license status. ", exception));
                throw new Exception("Unable to save license status", exception);
            }
        }

        private void SetDefaults()
        {
            this._status = SKLicenseStatus.Invalid;
            this._licensedSize = 0;
            this._usedSizeFromServer = 0;
            this._expires = DateTime.MinValue;
            this._lastConnect = DateTime.MinValue;
        }

        public override string ToString()
        {
            object[] objArray = new object[]
            {
                this._expires, this._status, this._licensedSize, this._usedSizeFromServer, this._lastConnect,
                this._migratedByteSize, this._migratedItemsCount, this._bytesOutOfSync
            };
            return string.Format(
                "Expires: {0}, Status: {1}, LicensedSize: {2}, UsedSizeFromServer: {3}, LastConnect: {4}, MigratedByteSize: {5}, MigratedItemsCount: {6}, BytesOutOfSync: {7}",
                objArray);
        }

        internal event EventHandler StatusChanged;
    }
}