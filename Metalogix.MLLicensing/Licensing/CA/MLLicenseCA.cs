using Metalogix.Licensing;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Licensing.CA
{
    public sealed class MLLicenseCA : MLLicense
    {
        public const string LICENSE_FILENAME = "Product.lic";

        private string m_Name;

        private string m_Organization;

        private string m_Email;

        private string m_sProduct;

        private string m_Signature;

        private DateTime m_ExpiryDate;

        private int m_iTotalPages;

        private Guid m_guidSerialNumber;

        private MLLicenseType m_LicenseType;

        public long Options;

        public override string Email
        {
            get { return this.m_Email; }
        }

        public override DateTime ExpiryDate
        {
            get { return this.m_ExpiryDate; }
        }

        public bool IsWellFormed
        {
            get
            {
                if (this.Name != null && this.Organization != null && this.Email != null &&
                    !this.LicenseType.Equals(MLLicenseType.Invalid) && this.m_Signature != null)
                {
                    return true;
                }

                return false;
            }
        }

        public override string LicenseKey
        {
            get { return this.ToEncodedString(); }
        }

        public override MLLicenseType LicenseType
        {
            get { return this.m_LicenseType; }
        }

        public override string Name
        {
            get { return this.m_Name; }
        }

        public override string Organization
        {
            get { return this.m_Organization; }
        }

        public string Product
        {
            get { return this.m_sProduct; }
        }

        public Guid SerialNumber
        {
            get { return this.m_guidSerialNumber; }
        }

        public string Signature
        {
            get { return this.m_Signature; }
        }

        public int TotalPages
        {
            get { return this.m_iTotalPages; }
        }

        public MLLicenseCA(string sEncoded)
        {
            try
            {
                this.FromEncodedString(sEncoded);
            }
            catch (Exception exception)
            {
                this.m_LicenseType = MLLicenseType.Invalid;
            }
        }

        public override void Dispose()
        {
            this.m_Signature = "";
            this.m_Name = "";
            this.m_Organization = null;
            this.m_Email = "";
            this.m_ExpiryDate = new DateTime(1974, 6, 11);
        }

        private string FromBase64String(string sEncoded)
        {
            byte[] numArray = Convert.FromBase64String(sEncoded);
            return Encoding.UTF8.GetString(numArray);
        }

        public bool FromEncodedString(string sEncoded)
        {
            string str = sEncoded.Trim();
            string[] strArrays = str.Split(new char[] { '?' });
            if (strArrays == null || (int)strArrays.Length != 10)
            {
                return false;
            }

            this.m_Name = this.FromBase64String(strArrays[0]);
            this.m_Organization = this.FromBase64String(strArrays[1]);
            this.m_Email = this.FromBase64String(strArrays[2]);
            this.m_sProduct = this.FromBase64String(strArrays[3]);
            this.m_LicenseType = (MLLicenseType)Enum.Parse(typeof(MLLicenseType), this.FromBase64String(strArrays[4]));
            this.m_iTotalPages = Convert.ToInt32(this.FromBase64String(strArrays[5]), 10);
            this.Options = Convert.ToInt64(this.FromBase64String(strArrays[6]), 10);
            string str1 = this.FromBase64String(strArrays[7]);
            this.m_ExpiryDate =
                DateTime.ParseExact(str1, "dd-MMM-yyyy", new CultureInfo("en-US", false), DateTimeStyles.None);
            string str2 = this.FromBase64String(strArrays[8]);
            this.m_guidSerialNumber = new Guid(str2);
            this.m_Signature = this.FromBase64String(strArrays[9]);
            return true;
        }

        internal ICryptoTransform GetDecryptor()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] bytes = aSCIIEncoding.GetBytes(this.Signature.Substring(0, 16));
            return rijndaelManaged.CreateDecryptor(bytes,
                aSCIIEncoding.GetBytes((this.Email.Length >= 16
                    ? this.Email.Substring(0, 16)
                    : this.Email.PadLeft(16, '0'))));
        }

        internal ICryptoTransform GetEncryptor()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] bytes = aSCIIEncoding.GetBytes(this.Signature.Substring(0, 16));
            return rijndaelManaged.CreateEncryptor(bytes,
                aSCIIEncoding.GetBytes((this.Email.Length >= 16
                    ? this.Email.Substring(0, 16)
                    : this.Email.PadLeft(16, '0'))));
        }

        public override string[] GetLicenseInfo()
        {
            string[] strArrays = new string[]
            {
                string.Concat("License: ", this.LicenseType.ToString()), "Registered To: ",
                string.Concat("   Name: ", this.Name), string.Concat("   Email: ", this.Email),
                string.Concat("   Organization: ", this.Organization), null
            };
            DateTime expiryDate = this.ExpiryDate;
            strArrays[5] = string.Concat("Expiry Date: ", expiryDate.ToString("dd-MMM-yyyy"));
            return strArrays;
        }

        public void SaveToFile(string sFile)
        {
            FileStream fileStream = null;
            try
            {
                try
                {
                    fileStream = new FileStream(sFile, FileMode.Create, FileAccess.Write, FileShare.Read);
                    StreamWriter streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write(this.LicenseKey);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fileStream = null;
                }
                catch (Exception exception)
                {
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        private string ToBase64String(string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        public string ToEncodedString()
        {
            if (!this.IsWellFormed)
            {
                return
                    "The license is not valid.  Please contact Metalogix (info@metalogix.net) to acquire a valid license.";
            }

            string base64String = this.ToBase64String(this.Name);
            string str = this.ToBase64String(this.Organization);
            string base64String1 = this.ToBase64String(this.Email);
            string str1 = this.ToBase64String(this.Product);
            string base64String2 = this.ToBase64String(this.LicenseType.ToString());
            string str2 = this.ToBase64String(Convert.ToString(this.TotalPages, 10));
            string base64String3 = this.ToBase64String(Convert.ToString(this.Options, 10));
            DateTime expiryDate = this.ExpiryDate;
            string str3 = expiryDate.ToString("dd-MMM-yyyy", new CultureInfo("en-US", false));
            string base64String4 = this.ToBase64String(str3);
            string str4 = this.ToBase64String(this.m_guidSerialNumber.ToString());
            string base64String5 = this.ToBase64String(this.Signature);
            char chr = '?';
            object[] objArray = new object[]
            {
                base64String, chr, str, chr, base64String1, chr, str1, chr, base64String2, chr, str2, chr,
                base64String3, chr, base64String4, chr, str4, chr, base64String5
            };
            return string.Concat(objArray);
        }

        public override string ToString()
        {
            object[] name = new object[]
            {
                this.Name, "\r\n", this.Organization, "\r\n", this.Email, "\r\n", this.Product, "\r\n",
                this.LicenseType, "\r\n", this.TotalPages, "\r\n", this.Options, "\r\n",
                this.m_guidSerialNumber.ToString(), "\r\n", null
            };
            DateTime expiryDate = this.ExpiryDate;
            name[16] = expiryDate.ToString("dd-MMM-yyyy", new CultureInfo(""));
            return string.Concat(name);
        }

        public override void Validate(string sProductCode)
        {
            //if (!MLLicenseProviderCA.CheckSignature(this.ToString(), this.Signature))
            //{
            //    throw new LicenseException(null, null, "The license signature is not valid");
            //}

            //if (this.ExpiryDate < DateTime.Now)
            //{
            //    throw new LicenseException(null, null, "The license has expired");
            //}

            //if (sProductCode != this.Product)
            //{
            //    throw new LicenseException(null, null, "The type's assembly product does not match license file");
            //}
        }
    }
}