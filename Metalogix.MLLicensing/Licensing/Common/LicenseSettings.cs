using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Metalogix.Licensing.Common
{
    public class LicenseSettings : LicenseProviderInitializationData
    {
        public string DataUnitName { get; set; }

        public static LicenseSettings Default
        {
            get
            {
                LicenseSettings licenseSetting = new LicenseSettings()
                {
                    LicenseServerUrl = "https://license.metalogix.com/license/licenseservice.asmx",
                    LicenseOfflineActivationURL = "http://www.metalogix.com/OfflineActivation",
                    LicenseServerUser = "v4user",
                    LicenseServerPass = "fcPEeRu4AHJpAfO",
                    SecureStorage = false,
                    RunCyclicChecking = true,
                    ProxyFilePath = "LicenseServerProxySettings.dat",
                    ProxyStorageProvider = LicenseStorageType.File
                };
                FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl,
                    AccessControlType.Allow);
                FileSystemAccessRule fileSystemAccessRule1 = fileSystemAccessRule;
                licenseSetting.UsageFileAccess = fileSystemAccessRule;
                licenseSetting.LicenseFileAccess = fileSystemAccessRule1;
                return licenseSetting;
            }
        }

        public Metalogix.Licensing.LicenseServer.Product LegacyProduct { get; set; }

        public IFormatter LicenseDataFormatter { get; set; }

        public FileSystemAccessRule LicenseFileAccess { get; private set; }

        public string LicenseFilePath { get; set; }

        public string LicenseOfflineActivationURL { get; set; }

        public string LicenseServerPass { get; set; }

        public string LicenseServerUrl { get; set; }

        public string LicenseServerUser { get; set; }

        public ILicensingConverter OldStyleLicensingConverter { get; set; }

        public Metalogix.Licensing.LicenseServer.Product Product { get; set; }

        public string ProductName { get; set; }

        public Version ProductVersion { get; set; }

        public string ProxyFilePath { get; set; }

        public LicenseStorageType ProxyStorageProvider { get; set; }

        public bool RunCyclicChecking { get; set; }

        public bool SecureStorage { get; set; }

        public FileSystemAccessRule UsageFileAccess { get; private set; }

        public string UsageFilePath { get; set; }

        public string UsageFilePathOld { get; set; }

        public LicenseSettings() : base(LicenseProviderType.Common)
        {
        }
    }
}