using System;
using System.Runtime.InteropServices;

namespace Metalogix.Permissions
{
    internal static class NativeMethods
    {
        internal const int LOGON32_LOGON_INTERACTIVE = 2;

        internal const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        internal static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool RevertToSelf();
    }
}