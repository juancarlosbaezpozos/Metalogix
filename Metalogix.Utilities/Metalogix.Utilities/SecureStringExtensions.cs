using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Metalogix.Utilities
{
    public static class SecureStringExtensions
    {
        public static bool IsNullOrEmpty(this SecureString value)
        {
            if (value == null)
            {
                return true;
            }

            return value.Length == 0;
        }

        private static string StandardizeInputString(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            input = input.Trim();
            return input;
        }

        public static string ToInsecureString(this SecureString input)
        {
            string empty = string.Empty;
            if (input.IsNullOrEmpty())
            {
                return empty;
            }

            IntPtr bSTR = Marshal.SecureStringToBSTR(input);
            try
            {
                empty = Marshal.PtrToStringBSTR(bSTR);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bSTR);
            }

            return empty;
        }

        public static SecureString ToSecureString(this string input)
        {
            input = input.StandardizeInputString();
            SecureString secureString = new SecureString();
            string str = input;
            for (int i = 0; i < str.Length; i++)
            {
                secureString.AppendChar(str[i]);
            }

            secureString.MakeReadOnly();
            return secureString;
        }
    }
}