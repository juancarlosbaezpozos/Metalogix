using Metalogix;
using Metalogix.Licensing.Common;
using Metalogix.Licensing.LicenseServer;
using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Metalogix.Licensing
{
    public class LicensingUtils
    {
        public const string DYNAMIC_CONTENT_FILE_LOCK_NAME = "MetalogixDynamicContentFileMutex";

        public const string DYNAMIC_CONTENT_FILE_STORAGE_LOCK_NAME = "MetalogixDynamicContentFileStorageMutex";

        public const string DYNAMIC_LICENSE_FILE_STORAGE_LOCK_NAME = "MetalogixLicenseFileStorageMutex";

        public const string DYNAMIC_LICENSE_PROXY_FILE_STORAGE_LOCK_NAME = "MetalogixLicenseProxyFileStorageMutex";

        private const int NO_ERROR = 0;

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        private const int ERROR_INVALID_FLAGS = 1004;

        private static string m_sSystemID;

        private static string m_sSystemIDFull;

        private static string SystemID
        {
            get
            {
                if (LicensingUtils.m_sSystemID == null)
                {
                    LicensingUtils.m_sSystemID = LicensingUtils.GetSID(null, Environment.MachineName)
                        .Replace("S-1-5-21-", string.Empty).Replace("-", string.Empty);
                }

                return LicensingUtils.m_sSystemID;
            }
        }

        public static string SystemIDFull
        {
            get
            {
                if (LicensingUtils.m_sSystemIDFull == null)
                {
                    StringBuilder stringBuilder = new StringBuilder(500);
                    LicensingUtils.GetIPAddress(stringBuilder);
                    if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != ',')
                    {
                        stringBuilder.Append(',');
                    }

                    stringBuilder.Append(LicensingUtils.SystemID);
                    if (stringBuilder.Length >= 1000)
                    {
                        stringBuilder.Remove(999, stringBuilder.Length - 999);
                    }

                    LicensingUtils.m_sSystemIDFull = stringBuilder.ToString();
                }

                return LicensingUtils.m_sSystemIDFull;
            }
        }

        public LicensingUtils()
        {
        }

        [DllImport("advapi32", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        private static extern bool ConvertSidToStringSid(byte[] pSID, out IntPtr ptrSid);

        public static Edition GetEdition(MLLicense license)
        {
            Edition edition;
            MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
            if (mLLicenseCommon != null)
            {
                string customFieldValue = mLLicenseCommon.GetCustomFieldValue("Edition");
                if (customFieldValue != null)
                {
                    int num = 0;
                    if (int.TryParse(customFieldValue, out num))
                    {
                        return (Edition)num;
                    }

                    try
                    {
                        edition = (Edition)Enum.Parse(typeof(Edition), customFieldValue);
                    }
                    catch
                    {
                        return Edition.Standard;
                    }

                    return edition;
                }
            }

            return Edition.Standard;
        }

        public static Edition GetEdition()
        {
            return LicensingUtils.GetEdition(
                MLLicenseProvider.Instance.GetLicense(null, null, null, false) as MLLicense);
        }

        private static void GetIPAddress(StringBuilder sb)
        {
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            for (int i = 0; i < (int)addressList.Length; i++)
            {
                sb.Append(addressList[i].ToString());
                sb.Append(",");
            }
        }

        public static CompatibilityLevel GetLevel(License license)
        {
            if (license == null)
            {
                return CompatibilityLevel.Invalid;
            }

            if (!(license is MLLicense))
            {
                return CompatibilityLevel.Current;
            }

            MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
            if (mLLicenseCommon == null)
            {
                return CompatibilityLevel.Legacy;
            }

            if (mLLicenseCommon.IsLegacyProduct)
            {
                return CompatibilityLevel.Legacy;
            }

            return CompatibilityLevel.Current;
        }

        public static CompatibilityLevel GetLevel()
        {
            License license = MLLicenseProvider.Instance.GetLicense(null, null, null, false);
            return LicensingUtils.GetLevel(license);
        }

        public static LicensedSharePointVersions GetLicensedSharePointVersions()
        {
            return LicensingUtils.GetLicensedSharePointVersions(
                MLLicenseProvider.Instance.GetLicense(null, null, null, false) as MLLicense);
        }

        public static LicensedSharePointVersions GetLicensedSharePointVersions(MLLicense license)
        {
            MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
            if (mLLicenseCommon == null)
            {
                return LicensedSharePointVersions.All;
            }

            return mLLicenseCommon.LicensedSharePointVersions;
        }

        public static void GetLicenseFileInfo(out string[] licenseInfo, out string[] customLicenseProperties)
        {
            try
            {
                LicenseSettings licenseProviderInitializationDataFromAssembly =
                    (LicenseSettings)LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(
                        ApplicationData.MainAssembly);
                MigrationLicense migrationLicense = new MigrationLicense(licenseProviderInitializationDataFromAssembly);
                licenseInfo = migrationLicense.GetLicenseInfo();
                customLicenseProperties = migrationLicense.GetLicenseCustomInfo();
            }
            catch (Exception exception)
            {
                licenseInfo = new string[0];
                customLicenseProperties = new string[0];
            }
        }

        private static string GetSID(string systemName, string accountName)
        {
            unsafe
            {
                LicensingUtils.SID_NAME_USE sIDNAMEUSE;
                IntPtr intPtr;
                byte[] numArray = null;
                uint num = 0;
                StringBuilder stringBuilder = new StringBuilder();
                uint capacity = (uint)stringBuilder.Capacity;
                int lastWin32Error = 0;
                if (LicensingUtils.LookupAccountName(systemName, accountName, numArray, ref num, stringBuilder,
                        ref capacity, out sIDNAMEUSE))
                {
                    Logger.Error.WriteFormat("LicensingUtils.GetSID >> Could not find the SID. Error : {0}",
                        new object[0]);
                }
                else
                {
                    lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 122 || lastWin32Error == 1004)
                    {
                        numArray = new byte[num];
                        stringBuilder.EnsureCapacity((int)capacity);
                        lastWin32Error = 0;
                        if (!LicensingUtils.LookupAccountName(null, accountName, numArray, ref num, stringBuilder,
                                ref capacity, out sIDNAMEUSE))
                        {
                            lastWin32Error = Marshal.GetLastWin32Error();
                        }
                    }
                }

                if (lastWin32Error != 0)
                {
                    Logger.Error.WriteFormat("LicensingUtils.GetSID >> Error : {0}", new object[] { lastWin32Error });
                }
                else
                {
                    if (LicensingUtils.ConvertSidToStringSid(numArray, out intPtr))
                    {
                        string stringAuto = Marshal.PtrToStringAuto(intPtr);
                        LicensingUtils.LocalFree(intPtr);
                        return stringAuto;
                    }

                    lastWin32Error = Marshal.GetLastWin32Error();
                    Logger.Error.WriteFormat("LicensingUtils.GetSID >> Could not convert sid to string. Error : {0}",
                        new object[] { lastWin32Error });
                }

                return string.Empty;
            }
        }

        public static string GetUsageEntryID(string licenseKey, Product product)
        {
            return string.Format("|{0}|{1}|{2}", licenseKey, LicensingUtils.SystemID, product.ToString());
        }

        public static bool IdEquals(string local, string server)
        {
            string[] strArrays = local.Split(new char[] { ',' });
            string[] strArrays1 = server.Split(new char[] { ',' });
            string[] strArrays2 = strArrays;
            for (int i = 0; i < (int)strArrays2.Length; i++)
            {
                string str = strArrays2[i];
                string[] strArrays3 = strArrays1;
                for (int j = 0; j < (int)strArrays3.Length; j++)
                {
                    if (str.Equals(strArrays3[j], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsContentMatrixContentUnderMgmt()
        {
            MLLicenseCommon license = MLLicenseProvider.Instance.GetLicense(null, null, null, false) as MLLicenseCommon;
            if (license.IsLegacyProduct)
            {
                return false;
            }

            return !license.IsContentMatrixExpress;
        }

        public static bool IsDataLimitExceededForContentUnderMgmt()
        {
            License license = MLLicenseProvider.Instance.GetLicense(null, null, null, false);
            if (license is MLLicenseCommon)
            {
                return (license as MLLicenseCommon).IsDataLimitExceededForContentUnderMgmt;
            }

            if (license.GetType().Name.Contains("ContentManagerLicense"))
            {
                return false;
            }

            return true;
        }

        public static bool IsLicenseFileExpressEdition()
        {
            bool isContentMatrixExpress;
            try
            {
                LicenseSettings licenseProviderInitializationDataFromAssembly =
                    (LicenseSettings)LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(
                        ApplicationData.MainAssembly);
                isContentMatrixExpress = (new MigrationLicense(licenseProviderInitializationDataFromAssembly))
                    .IsContentMatrixExpress;
            }
            catch (Exception exception)
            {
                isContentMatrixExpress = false;
            }

            return isContentMatrixExpress;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        private static extern bool LookupAccountName(string lpSystemName, string lpAccountName, byte[] Sid,
            ref uint cbSid, StringBuilder ReferencedDomainName, ref uint cchReferencedDomainName,
            out LicensingUtils.SID_NAME_USE peUse);

        private enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup = 2,
            SidTypeDomain = 3,
            SidTypeAlias = 4,
            SidTypeWellKnownGroup = 5,
            SidTypeDeletedAccount = 6,
            SidTypeInvalid = 7,
            SidTypeUnknown = 8,
            SidTypeComputer = 9
        }
    }
}