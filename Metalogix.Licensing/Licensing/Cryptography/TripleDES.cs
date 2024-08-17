using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Licensing.Cryptography
{
    internal class TripleDES
    {
        private const string _INIT_VECTOR = "@1B2c3D4";

        private const string _PASS_PHARSE = "Pas5pr@se123456789098765";

        public TripleDES()
        {
        }

        public static string Decrypt(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            return Metalogix.Licensing.Cryptography.TripleDES.Decrypt(s, "Pas5pr@se123456789098765", "@1B2c3D4");
        }

        private static string Decrypt(string data, string key, string initVector)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(initVector);
            byte[] numArray = Encoding.ASCII.GetBytes(key);
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                (new TripleDESCryptoServiceProvider()).CreateDecryptor(numArray, bytes), CryptoStreamMode.Read);
            List<byte> nums = new List<byte>();
            for (int i = cryptoStream.ReadByte(); i != -1; i = cryptoStream.ReadByte())
            {
                nums.Add((byte)i);
            }

            return Encoding.UTF8.GetString(nums.ToArray());
        }

        public static string Encrypt(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            return Metalogix.Licensing.Cryptography.TripleDES.Encrypt(s, "Pas5pr@se123456789098765", "@1B2c3D4");
        }

        private static string Encrypt(string data, string key, string initVector)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(initVector);
            byte[] numArray = Encoding.ASCII.GetBytes(key);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                (new TripleDESCryptoServiceProvider()).CreateEncryptor(numArray, bytes), CryptoStreamMode.Write);
            byte[] bytes1 = Encoding.UTF8.GetBytes(data);
            cryptoStream.Write(bytes1, 0, (int)bytes1.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            cryptoStream.Close();
            memoryStream.Close();
            return Convert.ToBase64String(array);
        }
    }
}