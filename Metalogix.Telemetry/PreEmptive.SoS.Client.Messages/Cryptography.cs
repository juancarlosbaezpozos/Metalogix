using System;
using System.Security.Cryptography;
using System.Text;

namespace PreEmptive.SoS.Client.Messages
{
    public class Cryptography
    {
        internal static string URL;

        static Cryptography()
        {
            Cryptography.URL =
                "message.runtimeintelligence.com/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asmx";
        }

        public Cryptography()
        {
        }

        public static string Hash(string data)
        {
            if (data == null)
            {
                return null;
            }

            SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            bytes = sHA1CryptoServiceProvider.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            byte[] numArray = bytes;
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                byte num = numArray[i];
                stringBuilder.Append(num.ToString("x2").ToLower());
            }

            return stringBuilder.ToString();
        }
    }
}