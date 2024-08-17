using Microsoft.Win32;
using System;

namespace Metalogix.Utilities
{
    public static class SharePointUtils
    {
        public const string DocIdField = "ae3e2a36-125d-45d3-9051-744b513536a6";

        public const string DocUrlField = "3b63724f-3418-461f-868b-7706f69b029c";

        public const int CommunityWebTemplateId = 62;

        public const string SharePoint2016InstallEntry =
            "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\16.0";

        public const string SharePoint2013InstallEntry =
            "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\15.0";

        public const string SharePoint2010InstallEntry =
            "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\14.0";

        public const string SharePoint2007InstallEntry =
            "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\12.0";

        public const string SharePoint2003InstallEntry =
            "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\6.0";

        public static bool IsRegistryKeyExists(string sharePointInstallEntry)
        {
            bool flag;
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(sharePointInstallEntry))
            {
                if (registryKey != null)
                {
                    object value = registryKey.GetValue("SharePoint");
                    if (value != null && value.Equals("Installed"))
                    {
                        flag = true;
                        return flag;
                    }
                }

                return false;
            }

            return flag;
        }
    }
}