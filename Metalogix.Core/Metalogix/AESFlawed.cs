using Metalogix.Utilities;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace Metalogix
{
    internal static class AESFlawed
    {
        private readonly static byte[] DefaultKey;

        private readonly static byte[] DefaultIV;

        static AESFlawed()
        {
            AESFlawed.DefaultKey = new byte[]
            {
                38, 155, 118, 25, 210, 99, 133, 106, 207, 165, 205, 30, 15, 78, 7, 161, 37, 169, 235, 65, 80, 191, 184,
                12, 239, 224, 63, 164, 227, 9, 18, 127
            };
            AESFlawed.DefaultIV = new byte[]
                { 102, 208, 225, 22, 184, 160, 119, 196, 7, 83, 214, 152, 150, 112, 211, 13 };
        }

        internal static SecureString DecryptTextAESProvider(string toDecrypt)
        {
            string end;
            if (string.IsNullOrEmpty(toDecrypt))
            {
                return new SecureString();
            }

            byte[] numArray = Convert.FromBase64String(toDecrypt);
            using (Aes ae = Aes.Create())
            {
                ICryptoTransform cryptoTransform = ae.CreateDecryptor(AESFlawed.DefaultKey, AESFlawed.DefaultIV);
                using (MemoryStream memoryStream = new MemoryStream(numArray))
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            end = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return end.ToSecureString();
        }

        internal static string EncryptTextAESProvider(SecureString toEncrypt)
        {
            byte[] array;
            if (toEncrypt.IsNullOrEmpty())
            {
                return string.Empty;
            }

            using (Aes ae = Aes.Create())
            {
                ICryptoTransform cryptoTransform = ae.CreateEncryptor(AESFlawed.DefaultKey, AESFlawed.DefaultIV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(toEncrypt.ToInsecureString());
                        }
                    }

                    array = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}