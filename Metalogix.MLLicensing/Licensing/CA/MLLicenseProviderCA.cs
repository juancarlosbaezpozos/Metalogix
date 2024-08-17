using Metalogix.Licensing;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Metalogix.Licensing.CA
{
    public sealed class MLLicenseProviderCA : LicenseProvider, ILicenseProvider, IDisposable
    {
        public MLLicenseProviderCA()
        {
        }

        public MLLicenseProviderCA(LicenseProviderInitializationData data)
        {
        }

        internal static bool CheckSignature(string sContent, string sSignature)
        {
            bool flag = false;
            try
            {
                byte[] hashValue = MLLicenseProviderCA.GetHashValue(sContent);
                byte[] numArray = Convert.FromBase64String(sSignature);
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
                rSACryptoServiceProvider.FromXmlString(MLLicenseProviderCA.GetPublicKeyXml());
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter =
                    new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("SHA1");
                flag = rSAPKCS1SignatureDeformatter.VerifySignature(hashValue, numArray);
            }
            catch (Exception exception)
            {
            }

            return flag;
        }

        public void Dispose()
        {
        }

        private void FireLicenseUpdated()
        {
            if (this.LicenseUpdated != null)
            {
                this.LicenseUpdated(null, null);
            }
        }

        private static byte[] GetHashValue(string sString)
        {
            byte[] bytes = (new ASCIIEncoding()).GetBytes(sString);
            return HashAlgorithm.Create("SHA1").ComputeHash(bytes);
        }

        public override License GetLicense(LicenseContext context, Type type, object instance, bool bThrowExceptions)
        {
            License license;
            try
            {
                MLLicenseCA mLLicenseCA = null;
                byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();
                byte[] numArray = base.GetType().Assembly.GetName().GetPublicKeyToken();
                if (numArray == null || publicKeyToken == null)
                {
                    throw new LicenseException(type, instance, "The type's assembly is not signed");
                }

                if ((int)numArray.Length != (int)publicKeyToken.Length)
                {
                    throw new LicenseException(type, instance,
                        "The type's assembly does not have a Metalogix signature");
                }

                for (int i = 0; i < (int)numArray.Length; i++)
                {
                    if (numArray[i] != publicKeyToken[i])
                    {
                        throw new LicenseException(type, instance,
                            "The type's assembly does not have a Metalogix signature");
                    }
                }

                string str = null;
                string str1 = null;
                string str2 = null;
                MLLicenseProviderCA.GetLicenseInfoFromType(type, out str, out str1, out str2);
                FileInfo fileInfo = new FileInfo(str2);
                if (!fileInfo.Exists)
                {
                    throw new LicenseException(type, instance, "License file does not exist");
                }

                FileStream fileStream = fileInfo.OpenRead();
                string end = (new StreamReader(fileStream)).ReadToEnd();
                fileStream.Close();
                fileStream = null;
                mLLicenseCA = new MLLicenseCA(end);
                mLLicenseCA.Validate(str1);
                license = mLLicenseCA;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (bThrowExceptions)
                {
                    throw exception;
                }

                license = null;
            }

            return license;
        }

        public static void GetLicenseInfoFromType(Type type, out string sProductName, out string sProductCode,
            out string sLicenseFilePath)
        {
            sProductName = null;
            sProductCode = null;
            sLicenseFilePath = null;
            if (type == null)
            {
                throw new Exception("No type provided");
            }

            Assembly assembly = null;
            if (type.Assembly != Assembly.GetEntryAssembly())
            {
                string location = type.Assembly.Location;
                location = location.Substring(0, location.LastIndexOf('\\'));
                string[] files = Directory.GetFiles(location, "*.exe");
                int num = 0;
                while (num < (int)files.Length)
                {
                    Assembly assembly1 = Assembly.LoadFrom(files[num]);
                    if (assembly1.EntryPoint == null || !assembly1.FullName.EndsWith("1bd76498c7c4cba4"))
                    {
                        num++;
                    }
                    else
                    {
                        assembly = assembly1;
                        break;
                    }
                }
            }
            else
            {
                assembly = type.Assembly;
            }

            if (assembly == null)
            {
                throw new Exception("No licensed executable found");
            }

            Attribute customAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            if (customAttribute != null)
            {
                sProductName = ((AssemblyProductAttribute)customAttribute).Product;
            }

            Attribute attribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute));
            if (attribute != null)
            {
                sProductCode = ((AssemblyTitleAttribute)attribute).Title;
            }

            sLicenseFilePath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "\\Metalogix\\", sProductName, "\\Product.lic");
        }

        private static string GetPublicKeyXml()
        {
            string str =
                "oTScB8gwiljbYupxxlbQCkdeda05lVLi+rIvvVi+//Is6a7BgfIOVMenAHhBz0XESWX+85xoYItXIdY+WBVrj5jHyqAPIciQmM1jT8t2xJp+G3ZEnn55JtEDI1Vo2OyZ2/w4YRnjs9qFJHLMRx7mG9rIZnxj6P4gxVWZBLcitnjLgraJG4FONVmyUEASVT2Ad/IGvxxdH6r5konuSaFjqXpuUVIQGTQ+2gxzcfGoKM/3pjVbLdkq/sot6tpeoPHl7YBb4+XZn2M96bk0ccORY2xiVJO/AUedXdj91l2k9lh9xDvhIPAoceycSvOw2fCh+hXsjQPaZSmGvVz2i7PEDw==";
            string str1 = "AQAB";
            string[] strArrays = new string[]
                { "<RSAKeyValue><Modulus>", str, "</Modulus><Exponent>", str1, "</Exponent></RSAKeyValue>" };
            return string.Concat(strArrays);
        }

        public void Initialize()
        {
        }

        [Obsolete("Just here for ILicenseProvider support")]
        public bool IsSPOServerIdExist(string serverId)
        {
            return false;
        }

        public bool Update()
        {
            this.FireLicenseUpdated();
            return true;
        }

        [Obsolete("Just here for ILicenseProvider support")]
        public bool UpdateLicense(long usedData, string serverId, bool isSPO, string tenantUrlAndUser)
        {
            return false;
        }

        public event EventHandler LicenseUpdated;
    }
}