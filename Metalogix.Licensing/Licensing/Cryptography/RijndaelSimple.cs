using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Licensing.Cryptography
{
    [Guid("5F0F74EA-E8A4-4166-B19B-493745DCD77F")]
    internal class RijndaelSimple
    {
        private const string _PASS_PHARSE = "Pas5pr@se";

        private const string _SALT_VALUE = "s@1tValue";

        private const string _HASH_ALGORITHM = "SHA1";

        private const int _PASS_ITERATIONS = 2;

        private const string _INIT_VECTOR = "@1B2c3D4e5F6g7H8";

        private const int _KEY_SIZE = 256;

        public RijndaelSimple()
        {
        }

        public static string Decrypt(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            return RijndaelSimple.Decrypt(s, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256);
        }

        public static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm,
            int passwordIterations, string initVector, int keySize)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(initVector);
            byte[] numArray = Encoding.ASCII.GetBytes(saltValue);
            byte[] numArray1 = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes passwordDeriveByte =
                new PasswordDeriveBytes(passPhrase, numArray, hashAlgorithm, passwordIterations);
            byte[] bytes1 = passwordDeriveByte.GetBytes(keySize / 8);
            RijndaelManaged rijndaelManaged = new RijndaelManaged()
            {
                Mode = CipherMode.CBC
            };
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes1, bytes);
            MemoryStream memoryStream = new MemoryStream(numArray1);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
            byte[] numArray2 = new byte[(int)numArray1.Length];
            int num = cryptoStream.Read(numArray2, 0, (int)numArray2.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(numArray2, 0, num);
        }

        public static string Encrypt(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            return RijndaelSimple.Encrypt(s, "Pas5pr@se", "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256);
        }

        public static string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm,
            int passwordIterations, string initVector, int keySize)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(initVector);
            byte[] numArray = Encoding.ASCII.GetBytes(saltValue);
            byte[] bytes1 = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes passwordDeriveByte =
                new PasswordDeriveBytes(passPhrase, numArray, hashAlgorithm, passwordIterations);
            byte[] numArray1 = passwordDeriveByte.GetBytes(keySize / 8);
            RijndaelManaged rijndaelManaged = new RijndaelManaged()
            {
                Mode = CipherMode.CBC
            };
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(numArray1, bytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(bytes1, 0, (int)bytes1.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array);
        }
    }
}