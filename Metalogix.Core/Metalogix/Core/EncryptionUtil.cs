using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Core
{
    public class EncryptionUtil
    {
        public EncryptionUtil()
        {
        }

        public static string DecryptByRijndaelKey(string cipherText, string encryptionKey)
        {
            string str;
            byte[] numArray = Convert.FromBase64String(cipherText);
            byte[] array = numArray.Take<byte>(16).ToArray<byte>();
            byte[] array1 = numArray.Skip<byte>(16).Take<byte>((int)numArray.Length - 16).ToArray<byte>();
            SymmetricAlgorithm rijndaelManaged = new RijndaelManaged()
            {
                Key = Convert.FromBase64String(encryptionKey),
                IV = array,
                Padding = PaddingMode.PKCS7
            };
            using (MemoryStream memoryStream = new MemoryStream(array1))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV),
                           CryptoStreamMode.Read))
                {
                    byte[] numArray1 = new byte[(int)array1.Length];
                    int num = cryptoStream.Read(numArray1, 0, (int)numArray1.Length);
                    memoryStream.Close();
                    cryptoStream.Close();
                    str = Encoding.UTF8.GetString(numArray1, 0, num);
                }
            }

            return str;
        }

        public static string EncryptByRijndaelKey(string plainText, string encryptionKey)
        {
            string base64String;
            SymmetricAlgorithm rijndaelManaged = new RijndaelManaged();
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] numArray = EncryptionUtil.Generate256BitsOfRandomEntropy();
            rijndaelManaged.Key = Convert.FromBase64String(encryptionKey);
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                           rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, numArray), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, (int)bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] array = numArray.ToArray<byte>();
                    array = array.Concat<byte>(memoryStream.ToArray()).ToArray<byte>();
                    memoryStream.Close();
                    cryptoStream.Close();
                    base64String = Convert.ToBase64String(array);
                }
            }

            return base64String;
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] numArray = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(numArray);
            return numArray;
        }

        public static string GenerateEncryptionKey()
        {
            SymmetricAlgorithm rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.GenerateKey();
            return Convert.ToBase64String(rijndaelManaged.Key);
        }
    }
}