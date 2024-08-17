using Metalogix;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace Metalogix.Licensing.SK
{
    public abstract class MLLicenseSKBase : MLLicense
    {
        protected const string _REGISTRY_KEY = "LicenseKey";

        private readonly static Regex _regex;

        private string _key;

        protected MLLicenseType _type;

        protected Products _product;

        protected ILicenseStatus _status;

        protected InstallInfo _installInfo;

        public override string Email
        {
            get { return ""; }
        }

        public bool IsSet
        {
            get { return !string.IsNullOrEmpty(this._key); }
        }

        public override string LicenseKey
        {
            get { return this._key; }
        }

        public override MLLicenseType LicenseType
        {
            get { return this.GetLicenseType(false); }
        }

        public override string Name
        {
            get { return ""; }
        }

        public override string Organization
        {
            get { return ""; }
        }

        public Products Product
        {
            get { return this._product; }
        }

        public ILicenseStatus Status
        {
            get { return this._status; }
        }

        static MLLicenseSKBase()
        {
            MLLicenseSKBase._regex = new Regex("[2-9A-FKM]{25}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        protected MLLicenseSKBase()
        {
            this._installInfo = new InstallInfo();
        }

        protected MLLicenseSKBase(string key)
        {
            this.SetKey(key);
        }

        public abstract ILicenseStatus CheckOnline(LicenseProxy proxy);

        public override void Dispose()
        {
        }

        protected void FireStatusChanged()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, EventArgs.Empty);
            }
        }

        private static int GetChecksum(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            string str = key.Substring(3, 2);
            str = str.Replace('M', '0');
            return Convert.ToInt32(str.Replace('K', '1'));
        }

        protected abstract MLLicenseType GetLicenseType(bool throwException);

        public abstract void Load();

        public abstract void Save();

        protected void SetKey(string key)
        {
            bool flag = false;
            try
            {
                Logger.Debug.WriteFormat("MLLicenseSKBase >> SetKey: Setting new license key {0}",
                    new object[] { key });
                if (string.IsNullOrEmpty(key))
                {
                    Logger.Warning.WriteFormat("MLLicenseSKBase >> SetKey: License key is empty", new object[0]);
                }
                else
                {
                    string str = key.Trim().Replace("-", "").Replace(" ", "");
                    if (!string.IsNullOrEmpty(str) && str.Length == 25 && MLLicenseSKBase._regex.IsMatch(str) &&
                        MLLicenseSKBase.ValidateChecksum(str))
                    {
                        flag = true;
                    }

                    Logger.Debug.WriteFormat("MLLicenseSKBase >> SetKey: Key is legal = {0}", new object[] { flag });
                    if (flag)
                    {
                        int num = Convert.ToInt16(str.Substring(0, 3));
                        foreach (Products value in Enum.GetValues(typeof(Products)))
                        {
                            if ((int)value != num)
                            {
                                continue;
                            }

                            this._product = value;
                            break;
                        }

                        ILogMethods debug = Logger.Debug;
                        object[] product = new object[] { this._product, this._product == SKLP.Get.InitData.Product };
                        debug.WriteFormat("MLLicenseSKBase >> SetKey: Key product = {0} (is legal {1})", product);
                    }
                }
            }
            catch
            {
                flag = false;
            }

            this._type = (flag ? MLLicenseType.Commercial : MLLicenseType.Invalid);
            if (key != null)
            {
                this._key = key.Trim();
            }
        }

        internal abstract void SetStatus(ILicenseStatus status);

        internal abstract void SetStatus(ILicenseStatus status, bool isNewStatus);

        protected static void ThrowLicenseExceptionIfSet(bool throwException, string message)
        {
            if (throwException)
            {
                throw new LicenseException(null, null, message);
            }
        }

        public override void Validate(string sProductCode)
        {
            if (!string.IsNullOrEmpty(sProductCode) && this.IsSet &&
                (Products)Enum.Parse(typeof(Products), sProductCode) != this._product)
            {
                MLLicenseSKBase.ThrowLicenseExceptionIfSet(true, "The given license is not correct for this product.");
            }

            this.GetLicenseType(true);
        }

        private static bool ValidateChecksum(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            bool flag = false;
            int checksum = MLLicenseSKBase.GetChecksum(key);
            string str = key.Substring(5);
            str = str.Replace('M', '0');
            str = str.Replace('K', '1');
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int num1 = Convert.ToInt32(str.Substring(i, 1), 16);
                num += num1;
            }

            if (checksum == num % 100)
            {
                flag = true;
            }

            if (!flag)
            {
                Logger.Warning.WriteFormat("MLLicenseSKBase >> ValidateChecksum: Checksum is INcorrect.",
                    new object[0]);
            }
            else
            {
                Logger.Debug.WriteFormat("MLLicenseSKBase >> ValidateChecksum: Checksum is correct.", new object[0]);
            }

            return flag;
        }

        internal event EventHandler StatusChanged;
    }
}