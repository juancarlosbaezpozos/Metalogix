using Metalogix.Licensing.Logging;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Metalogix.Licensing.LicenseServer
{
    public class LicenseKey
    {
        private static readonly Regex _keySyntaxPattern;

        public bool IsValid { get; private set; }

        public string[] Parts
        {
            get
            {
                if (!this.IsValid)
                {
                    return new string[0];
                }

                return this.ValueFormatted.Split(new char[] { '-' });
            }
        }

        public Metalogix.Licensing.LicenseServer.Product Product { get; private set; }

        public int ProductCode { get; private set; }

        public string Value { get; private set; }

        public string ValueFormatted
        {
            get
            {
                if (!this.IsValid)
                {
                    return string.Empty;
                }

                string upper = this.Value.Replace("-", "").Trim().ToUpper();
                return upper.Insert(20, "-").Insert(15, "-").Insert(10, "-").Insert(5, "-");
            }
        }

        static LicenseKey()
        {
            LicenseKey._keySyntaxPattern =
                new Regex("^[2-9A-FKM\\-]{25,29}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public LicenseKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            this.Parse(key);
        }

        public override bool Equals(object obj)
        {
            LicenseKey licenseKey = obj as LicenseKey;
            if (licenseKey == null)
            {
                return this.Equals(obj);
            }

            return licenseKey.ValueFormatted.Equals(this.ValueFormatted, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.ValueFormatted.GetHashCode();
        }

        private void Parse(string key)
        {
            this.Product = Metalogix.Licensing.LicenseServer.Product.Unknown;
            this.ProductCode = 0;
            this.IsValid = false;
            this.Value = key;
            Logger.Debug.WriteFormat("LicenseKey >> Parse: Parsing license key '{0}'", new object[] { key });
            if (!LicenseKey._keySyntaxPattern.IsMatch(key))
            {
                Logger.Warning.WriteFormat("LicenseKey >> Parse: License key has wrong syntax '{0}'",
                    new object[] { key });
                return;
            }

            Logger.Debug.WriteFormat("LicenseKey >> Parse: License key '{0}' syntax OK", new object[] { key });
            key = key.Replace("-", "").Trim().ToUpper();
            if (!LicenseKey.ValidateChecksum(key))
            {
                Logger.Warning.WriteFormat("LicenseKey >> Parse: License key has wrong checksum '{0}'",
                    new object[] { key });
                return;
            }

            Logger.Debug.WriteFormat("LicenseKey >> Parse: License key '{0}' checksum OK", new object[] { key });
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { key.Substring(0, 3) };
            debug.WriteFormat("LicenseKey >> Parse: Converting product code '{0}'", objArray);
            this.ProductCode = Convert.ToInt16(key.Substring(0, 3));
            foreach (Metalogix.Licensing.LicenseServer.Product value in Enum.GetValues(
                         typeof(Metalogix.Licensing.LicenseServer.Product)))
            {
                if ((int)value != this.ProductCode)
                {
                    continue;
                }

                this.Product = value;
                break;
            }

            ILogMethods logMethod = Logger.Debug;
            object[] product = new object[] { this.Product };
            logMethod.WriteFormat("LicenseKey >> Parse: License key product='{0}'", product);
            this.IsValid = true;
        }

        public bool Validate(Metalogix.Licensing.LicenseServer.Product prod)
        {
            if (!this.IsValid)
            {
                return false;
            }

            bool product = this.Product == prod;
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { prod, this.Product, product };
            debug.WriteFormat("LicenseKey >> Validate: Required prod='{0}'; Current prod={1}; IsValid={2}", objArray);
            return product;
        }

        public bool Validate(int prodCode)
        {
            if (!this.IsValid)
            {
                return false;
            }

            bool productCode = this.ProductCode == prodCode;
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { prodCode, this.ProductCode, productCode };
            debug.WriteFormat("LicenseKey >> Validate: Required prod='{0}'; Current prod={1}; IsValid={2}", objArray);
            return productCode;
        }

        private static bool ValidateChecksum(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            string str = key.Substring(3, 2);
            str = str.Replace('M', '0');
            int num = Convert.ToInt32(str.Replace('K', '1'));
            string str1 = key.Substring(5);
            str1 = str1.Replace('M', '0');
            str1 = str1.Replace('K', '1');
            int num1 = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                int num2 = Convert.ToInt32(str1.Substring(i, 1), 16);
                num1 += num2;
            }

            return num == num1 % 100;
        }
    }
}