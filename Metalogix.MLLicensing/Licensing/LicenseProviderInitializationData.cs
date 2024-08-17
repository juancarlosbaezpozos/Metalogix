using Metalogix;
using Metalogix.Core;
using Metalogix.Licensing.Common;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace Metalogix.Licensing
{
    public abstract class LicenseProviderInitializationData
    {
        private LicenseProviderType _providerType;

        private string m_sLicensingAssemblyLocation;

        public string LicensingAssemblyLocation
        {
            get { return this.m_sLicensingAssemblyLocation; }
        }

        public LicenseProviderType ProviderType
        {
            get { return this._providerType; }
        }

        protected LicenseProviderInitializationData(LicenseProviderType providerType)
        {
            this._providerType = providerType;
        }

        private static string CreatePath(string fileName, LicenseStorageLocation location)
        {
            switch (location)
            {
                case LicenseStorageLocation.Common:
                {
                    return Path.Combine(ApplicationData.CommonDataPath, fileName);
                }
                case LicenseStorageLocation.AppData:
                {
                    return Path.Combine(ApplicationData.ApplicationPath, fileName);
                }
                case LicenseStorageLocation.Custom:
                {
                    return fileName;
                }
            }

            throw new Exception(string.Format("Unexpected LicenseStorageLocation '{0}' was set.", location));
        }

        public static LicenseProviderInitializationData GetLicenseProviderInitializationDataFromAssembly(
            Assembly assembly)
        {
            LicenseProviderInitializationData @default = null;
            try
            {
                object[] customAttributes = assembly.GetCustomAttributes(typeof(ProviderTypeAttribute), true);
                if ((int)customAttributes.Length != 0)
                {
                    if (((ProviderTypeAttribute)customAttributes[0]).Type == LicenseProviderType.Common)
                    {
                        @default = LicenseSettings.Default;
                        LicenseSettings migrationLicensingConverter = (LicenseSettings)@default;
                        customAttributes = assembly.GetCustomAttributes(typeof(OldStyleLicenseNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.OldStyleLicensingConverter =
                                new MigrationLicensingConverter(Path.Combine(ApplicationData.ApplicationPath,
                                    ((OldStyleLicenseNameAttribute)customAttributes[0]).OldStyleLicenseName));
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(ProductNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.ProductName =
                                ((ProductNameAttribute)customAttributes[0]).ProductName;
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(ProductAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.Product = ((ProductAttribute)customAttributes[0]).Product;
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(LegacyProductAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.LegacyProduct =
                                ((LegacyProductAttribute)customAttributes[0]).Product;
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(LicenseFileNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            string licenseFileName = ((LicenseFileNameAttribute)customAttributes[0]).LicenseFileName;
                            customAttributes = assembly.GetCustomAttributes(typeof(LicenseFileStorageAttribute), true);
                            if ((int)customAttributes.Length == 0)
                            {
                                migrationLicensingConverter.LicenseFilePath =
                                    Path.Combine(ApplicationData.ApplicationPath, licenseFileName);
                            }
                            else
                            {
                                migrationLicensingConverter.LicenseFilePath =
                                    (((LicenseFileStorageAttribute)customAttributes[0]).LicensePath ==
                                     LicenseStorageLocation.Common
                                        ? Path.Combine(ApplicationData.CommonDataPath, licenseFileName)
                                        : Path.Combine(ApplicationData.ApplicationPath, licenseFileName));
                            }
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(UsageFileNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.UsageFilePathOld = Path.Combine(ApplicationData.ApplicationPath,
                                ((UsageFileNameAttribute)customAttributes[0]).UsageFileName);
                            migrationLicensingConverter.UsageFilePath = Path.Combine(ApplicationData.CommonDataPath,
                                ((UsageFileNameAttribute)customAttributes[0]).UsageFileName);
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(ProxyFilePathAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.ProxyFilePath =
                                LicenseProviderInitializationData.CreatePath(
                                    ((ProxyFilePathAttribute)customAttributes[0]).ProxyFileName,
                                    ((ProxyFilePathAttribute)customAttributes[0]).ProxyFileLocation);
                            migrationLicensingConverter.ProxyStorageProvider =
                                ((ProxyFilePathAttribute)customAttributes[0]).ProxyStorageType;
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(DataUnitNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.DataUnitName =
                                ((DataUnitNameAttribute)customAttributes[0]).DataUnitName;
                        }

                        customAttributes =
                            assembly.GetCustomAttributes(typeof(LicenseDataFormatterNameAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            try
                            {
                                string licenseDataFormatterName =
                                    ((LicenseDataFormatterNameAttribute)customAttributes[0]).LicenseDataFormatterName;
                                Type type = Type.GetType(licenseDataFormatterName);
                                migrationLicensingConverter.LicenseDataFormatter =
                                    Activator.CreateInstance(type) as IFormatter;
                            }
                            catch
                            {
                            }
                        }

                        customAttributes = assembly.GetCustomAttributes(typeof(SecureStorageAttribute), true);
                        if ((int)customAttributes.Length != 0)
                        {
                            migrationLicensingConverter.SecureStorage =
                                ((SecureStorageAttribute)customAttributes[0]).SecureStorageEnabled;
                        }

                        migrationLicensingConverter.ProductVersion = assembly.GetName().Version;
                    }

                    @default.m_sLicensingAssemblyLocation = assembly.Location;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                Metalogix.Core.Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                    "GetLicenseProviderInitializationDataFromAssembly", true);
            }

            return @default;
        }
    }
}