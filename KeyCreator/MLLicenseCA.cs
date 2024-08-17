using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyCreator
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
            get => m_Email;
            set => m_Email = value;
        }

        public override DateTime ExpiryDate
        {
            get => m_ExpiryDate;
            set => m_ExpiryDate = value;
        }

        public bool IsWellFormed
        {
            get
            {
                if (Name != null && Organization != null && Email != null &&
                    !LicenseType.Equals(MLLicenseType.Invalid) && m_Signature != null)
                {
                    return true;
                }

                return false;
            }
        }

        public override string LicenseKey => ToEncodedString();

        public override MLLicenseType LicenseType
        {
            get => m_LicenseType;
            set => m_LicenseType = value;
        }

        public override string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public override string Organization
        {
            get => m_Organization;
            set => m_Organization = value;
        }

        public string Product
        {
            get => m_sProduct;
            set => m_sProduct = value;
        }

        public Guid SerialNumber
        {
            get => m_guidSerialNumber;
            set => m_guidSerialNumber = value;
        }

        public string Signature
        {
            get => m_Signature;
            set => m_Signature = value;
        }

        public int TotalPages
        {
            get => m_iTotalPages;
            set => m_iTotalPages = value;
        }

        public MLLicenseCA() { }

        public MLLicenseCA(string sEncoded)
        {
            try
            {
                FromEncodedString(sEncoded);
            }
            catch (Exception exception)
            {
                m_LicenseType = MLLicenseType.Invalid;
            }
        }

        public override void Dispose()
        {
            m_Signature = "";
            m_Name = "";
            m_Organization = null;
            m_Email = "";
            m_ExpiryDate = new DateTime(1974, 6, 11);
        }

        private string FromBase64String(string sEncoded)
        {
            var numArray = Convert.FromBase64String(sEncoded);
            return Encoding.UTF8.GetString(numArray);
        }

        public bool FromEncodedString(string sEncoded)
        {
            var str = sEncoded.Trim();
            var strArrays = str.Split(new char[] { '?' });
            if (strArrays == null || (int)strArrays.Length != 10)
            {
                return false;
            }

            m_Name = FromBase64String(strArrays[0]);
            m_Organization = FromBase64String(strArrays[1]);
            m_Email = FromBase64String(strArrays[2]);
            m_sProduct = FromBase64String(strArrays[3]);
            m_LicenseType = (MLLicenseType)Enum.Parse(typeof(MLLicenseType), FromBase64String(strArrays[4]));
            m_iTotalPages = Convert.ToInt32(FromBase64String(strArrays[5]), 10);
            Options = Convert.ToInt64(FromBase64String(strArrays[6]), 10);
            var str1 = FromBase64String(strArrays[7]);
            m_ExpiryDate =
                DateTime.ParseExact(str1, "dd-MMM-yyyy", new CultureInfo("en-US", false), DateTimeStyles.None);
            var str2 = FromBase64String(strArrays[8]);
            m_guidSerialNumber = new Guid(str2);
            m_Signature = FromBase64String(strArrays[9]);
            return true;
        }

        internal ICryptoTransform GetDecryptor()
        {
            var rijndaelManaged = new RijndaelManaged();
            var aSCIIEncoding = new ASCIIEncoding();
            var bytes = aSCIIEncoding.GetBytes(Signature.Substring(0, 16));
            return rijndaelManaged.CreateDecryptor(bytes,
                aSCIIEncoding.GetBytes((Email.Length >= 16
                    ? Email.Substring(0, 16)
                    : Email.PadLeft(16, '0'))));
        }

        internal ICryptoTransform GetEncryptor()
        {
            var rijndaelManaged = new RijndaelManaged();
            var aSCIIEncoding = new ASCIIEncoding();
            var bytes = aSCIIEncoding.GetBytes(Signature.Substring(0, 16));
            return rijndaelManaged.CreateEncryptor(bytes,
                aSCIIEncoding.GetBytes((Email.Length >= 16
                    ? Email.Substring(0, 16)
                    : Email.PadLeft(16, '0'))));
        }

        public override string[] GetLicenseInfo()
        {
            var strArrays = new string[]
            {
                string.Concat("License: ", LicenseType.ToString()), "Registered To: ",
                string.Concat("   Name: ", Name), string.Concat("   Email: ", Email),
                string.Concat("   Organization: ", Organization), null
            };
            var expiryDate = ExpiryDate;
            strArrays[5] = string.Concat("Expiry Date: ", expiryDate.ToString("dd-MMM-yyyy"));
            return strArrays;
        }

        public void SaveToFile(string sFile)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(sFile, FileMode.Create, FileAccess.Write, FileShare.Read);
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(LicenseKey);
                streamWriter.Flush();
                streamWriter.Close();
                fileStream = null;
            }
            catch (Exception exception)
            {
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
            if (!IsWellFormed)
            {
                return
                    "The license is not valid.  Please contact Metalogix (info@metalogix.net) to acquire a valid license.";
            }

            var base64String = ToBase64String(Name);
            var str = ToBase64String(Organization);
            var base64String1 = ToBase64String(Email);
            var str1 = ToBase64String(Product);
            var base64String2 = ToBase64String(LicenseType.ToString());
            var str2 = ToBase64String(Convert.ToString(TotalPages, 10));
            var base64String3 = ToBase64String(Convert.ToString(Options, 10));
            var expiryDate = ExpiryDate;
            var str3 = expiryDate.ToString("dd-MMM-yyyy", new CultureInfo("en-US", false));
            var base64String4 = ToBase64String(str3);
            var str4 = ToBase64String(m_guidSerialNumber.ToString());
            var base64String5 = ToBase64String(Signature);
            var chr = '?';
            var objArray = new object[]
            {
                base64String, chr, str, chr, base64String1, chr, str1, chr, base64String2, chr, str2, chr,
                base64String3, chr, base64String4, chr, str4, chr, base64String5
            };
            return string.Concat(objArray);
        }

        public override string ToString()
        {
            var name = new object[]
            {
                Name, "\r\n", Organization, "\r\n", Email, "\r\n", Product, "\r\n",
                LicenseType, "\r\n", TotalPages, "\r\n", Options, "\r\n",
                m_guidSerialNumber.ToString(), "\r\n", null
            };
            var expiryDate = ExpiryDate;
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
