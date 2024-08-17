using System;
using System.Runtime.InteropServices;

namespace Metalogix.SharePoint.Adapters
{
    internal static class NativeMethods
    {
        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int CLSIDFromString(string sz, out Guid clsid);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern void GetSystemInfo(out SYSTEM_INFO systemInfo);
    }
}