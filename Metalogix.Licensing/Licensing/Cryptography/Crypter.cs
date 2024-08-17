using System;

namespace Metalogix.Licensing.Cryptography
{
    public static class Crypter
    {
        public static string Decrypt(string s)
        {
            string str;
            try
            {
                str = TripleDES.Decrypt(s);
            }
            catch (Exception exception)
            {
                str = RijndaelSimple.Decrypt(s);
            }

            return str;
        }

        public static string Encrypt(string s)
        {
            return Crypter.Encrypt(s, Encryption.TripleDES);
        }

        public static string Encrypt(string s, Encryption method)
        {
            if (method == Encryption.Rijndael)
            {
                return RijndaelSimple.Encrypt(s);
            }

            return TripleDES.Encrypt(s);
        }
    }
}