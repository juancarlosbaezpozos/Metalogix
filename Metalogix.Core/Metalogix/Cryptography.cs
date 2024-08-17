using Metalogix.Utilities;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Metalogix
{
    public static class Cryptography
    {
        private readonly static byte[] DefaultEntropy;

        static Cryptography()
        {
            Cryptography.DefaultEntropy = new byte[]
            {
                206, 211, 243, 24, 167, 88, 109, 180, 184, 44, 249, 69, 102, 138, 245, 191, 11, 188, 13, 197, 47, 236,
                225, 198
            };
        }

        private static byte[] Decrypt(byte[] data)
        {
            if (data == null || (int)data.Length == 0)
            {
                return data;
            }

            EnvelopedCms envelopedCm = new EnvelopedCms();
            envelopedCm.Decode(data);
            envelopedCm.Decrypt();
            return Cryptography.VerifyAndRemoveSignature(envelopedCm.ContentInfo.Content);
        }

        public static SecureString DecryptText(string encryptedText)
        {
            SecureString secureString;
            if (string.IsNullOrEmpty(encryptedText))
            {
                return new SecureString();
            }

            if (ApplicationData.IsWeb)
            {
                return AESFlawed.DecryptTextAESProvider(encryptedText);
            }

            if (Cryptography.IsEncryptedUnderCurrentUserContext(encryptedText, out secureString))
            {
                return secureString;
            }

            if (Cryptography.IsEncryptedUnderLocalMachineContext(encryptedText, out secureString))
            {
                return secureString;
            }

            if (!Cryptography.IsEncryptedWithCertifcate(encryptedText, out secureString))
            {
                return new SecureString();
            }

            return secureString;
        }

        public static SecureString DecryptText(string encryptedText, Cryptography.ProtectionScope dataProtectionScope,
            byte[] entropy = null)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return new SecureString();
            }

            switch (dataProtectionScope)
            {
                case Cryptography.ProtectionScope.CurrentUser:
                case Cryptography.ProtectionScope.LocalMachine:
                {
                    DataProtectionScope dataProtectionScope1 =
                        (DataProtectionScope)Enum.Parse(typeof(DataProtectionScope), dataProtectionScope.ToString());
                    return Cryptography.DecryptTextDPAPIProvider(encryptedText, dataProtectionScope1,
                        entropy ?? Cryptography.DefaultEntropy);
                }
                case Cryptography.ProtectionScope.Certificate:
                {
                    return Cryptography.DecryptTextWithCert(encryptedText);
                }
            }

            return null;
        }

        private static SecureString DecryptTextDPAPIProvider(string encryptedData, DataProtectionScope scope,
            byte[] entropy)
        {
            SecureString secureString;
            if (string.IsNullOrEmpty(encryptedData))
            {
                return new SecureString();
            }

            try
            {
                byte[] numArray = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, scope);
                secureString = Encoding.Unicode.GetString(numArray).ToSecureString();
            }
            catch
            {
                secureString = new SecureString();
            }

            return secureString;
        }

        public static SecureString DecryptTextusingAES(string input)
        {
            return AESFlawed.DecryptTextAESProvider(input);
        }

        public static SecureString DecryptTextWithCert(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return new SecureString();
            }

            byte[] numArray = Cryptography.Decrypt(Convert.FromBase64String(encryptedText));
            return Encoding.Unicode.GetString(numArray).ToSecureString();
        }

        private static byte[] EncryptData(byte[] data, X509Certificate2 certificate)
        {
            EnvelopedCms envelopedCm = new EnvelopedCms(new ContentInfo(data));
            envelopedCm.Encrypt(new CmsRecipient(certificate));
            return envelopedCm.Encode();
        }

        public static string EncryptText(SecureString input, Cryptography.ProtectionScope dataProtectionScope = 0,
            byte[] entropy = null)
        {
            entropy = entropy ?? Cryptography.DefaultEntropy;
            if (ApplicationData.IsWeb)
            {
                return AESFlawed.EncryptTextAESProvider(input);
            }

            DataProtectionScope dataProtectionScope1 =
                (DataProtectionScope)Enum.Parse(typeof(DataProtectionScope), dataProtectionScope.ToString());
            return Cryptography.EncryptTextDPAPIProvider(input, dataProtectionScope1, entropy);
        }

        public static string EncryptText(SecureString input, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentException("certificate cannot be null");
            }

            if (input == null)
            {
                throw new ArgumentException("input cannot be null");
            }

            byte[] bytes = Encoding.Unicode.GetBytes(input.ToInsecureString());
            byte[] numArray = Cryptography.SignData(bytes, certificate);
            return Convert.ToBase64String(Cryptography.EncryptData(numArray, certificate));
        }

        private static string EncryptTextDPAPIProvider(SecureString toEncrypt, DataProtectionScope dataProtectionScope,
            byte[] entropy)
        {
            if (toEncrypt == null || toEncrypt.Length == 0)
            {
                return string.Empty;
            }

            byte[] numArray = ProtectedData.Protect(Encoding.Unicode.GetBytes(toEncrypt.ToInsecureString()), entropy,
                dataProtectionScope);
            return Convert.ToBase64String(numArray);
        }

        public static string EncryptTextusingAES(SecureString input)
        {
            return AESFlawed.EncryptTextAESProvider(input);
        }

        public static bool IsEncryptedUnderCurrentUserContext(string encryptedData, out SecureString result)
        {
            result = null;
            if (string.IsNullOrEmpty(encryptedData))
            {
                return false;
            }

            result = Cryptography.DecryptText(encryptedData, Cryptography.ProtectionScope.CurrentUser, null);
            return !result.IsNullOrEmpty();
        }

        public static bool IsEncryptedUnderLocalMachineContext(string encryptedData, out SecureString result)
        {
            result = null;
            if (string.IsNullOrEmpty(encryptedData))
            {
                return false;
            }

            result = Cryptography.DecryptText(encryptedData, Cryptography.ProtectionScope.LocalMachine, null);
            return !result.IsNullOrEmpty();
        }

        public static bool IsEncryptedWithAESProvider(string input, out SecureString result)
        {
            result = null;
            try
            {
                result = AESFlawed.DecryptTextAESProvider(input);
            }
            catch
            {
            }

            return !result.IsNullOrEmpty();
        }

        public static bool IsEncryptedWithCertifcate(string encryptedData, out SecureString result)
        {
            result = null;
            if (string.IsNullOrEmpty(encryptedData))
            {
                return false;
            }

            result = Cryptography.DecryptText(encryptedData, Cryptography.ProtectionScope.Certificate, null);
            return !result.IsNullOrEmpty();
        }

        private static byte[] SignData(byte[] data, X509Certificate2 certificate)
        {
            SignedCms signedCm = new SignedCms(new ContentInfo(data));
            signedCm.ComputeSignature(new CmsSigner(certificate));
            return signedCm.Encode();
        }

        private static byte[] VerifyAndRemoveSignature(byte[] data)
        {
            if (data == null || (int)data.Length == 0)
            {
                return data;
            }

            SignedCms signedCm = new SignedCms();
            signedCm.Decode(data);
            signedCm.CheckSignature(true);
            return signedCm.ContentInfo.Content;
        }

        public enum ProtectionScope
        {
            CurrentUser,
            LocalMachine,
            Certificate
        }
    }
}