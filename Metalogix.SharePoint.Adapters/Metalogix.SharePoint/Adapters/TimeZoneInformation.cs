using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public abstract class TimeZoneInformation
    {
        private static Dictionary<int, TimeZoneInformation> s_TimeZones;

        private readonly static object s_TimeZoneReadLockObject;

        public abstract int Bias { get; }

        public abstract int DaylightBias { get; }

        public abstract int ID { get; }

        public abstract string Name { get; }

        public abstract int StandardBias { get; }

        public static Dictionary<int, TimeZoneInformation> TimeZones
        {
            get
            {
                if (TimeZoneInformation.s_TimeZones == null)
                {
                    lock (TimeZoneInformation.s_TimeZoneReadLockObject)
                    {
                        if (TimeZoneInformation.s_TimeZones == null)
                        {
                            TimeZoneInformation.s_TimeZones = TimeZoneInformation.ReadTimeZonesFromFile();
                        }
                    }
                }

                return TimeZoneInformation.s_TimeZones;
            }
        }

        static TimeZoneInformation()
        {
            TimeZoneInformation.s_TimeZones = null;
            TimeZoneInformation.s_TimeZoneReadLockObject = new object();
        }

        protected TimeZoneInformation()
        {
        }

        public static TimeZoneInformation GetLocalTimeZone()
        {
            return new BuiltInTimeZoneInformation(TimeZone.CurrentTimeZone);
        }

        public static TimeZoneInformation GetTimeZone(int iID)
        {
            TimeZoneInformation timeZoneInformation;
            if (iID < 0)
            {
                return TimeZoneInformation.GetLocalTimeZone();
            }

            if (!TimeZoneInformation.TimeZones.TryGetValue(iID, out timeZoneInformation))
            {
                throw new Exception(string.Concat("TimeZone: ", iID, " does not exist"));
            }

            return timeZoneInformation;
        }

        public abstract DateTime LocalTimeToUtc(DateTime local);

        private static Dictionary<int, TimeZoneInformation> ReadTimeZonesFromFile()
        {
            string str;
            XmlDocument xmlDocument = new XmlDocument();
            string str1 = null;
            try
            {
                str = (!Utils.SystemIs64Bit
                    ? "SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint"
                    : "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint");
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(str);
                if (registryKey == null)
                {
                    str = "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint16.0";
                    registryKey = Registry.LocalMachine.OpenSubKey(str);
                }

                if (registryKey != null)
                {
                    str1 = registryKey.GetValue("").ToString();
                    str1 = str1.Trim(new char[] { '\\' });
                    int num = str1.LastIndexOf("\\", StringComparison.Ordinal);
                    str1 = str1.Substring(0, num);
                }
            }
            catch
            {
            }

            if (!string.IsNullOrEmpty(str1))
            {
                str1 = string.Concat(str1, "\\CONFIG\\TIMEZONE.XML");
            }

            if (string.IsNullOrEmpty(str1) || !File.Exists(str1))
            {
                string str2 = "Metalogix.SharePoint.Adapters.TimeZoneFiles.TIMEZONE.XML";
                using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str2))
                {
                    if (manifestResourceStream == null)
                    {
                        throw new Exception(string.Concat("The resource: '", str2,
                            "' did not exist within the assembly: ", Assembly.GetExecutingAssembly().Location));
                    }

                    using (XmlReader xmlTextReader = new XmlTextReader(manifestResourceStream))
                    {
                        xmlDocument.Load(xmlTextReader);
                    }
                }
            }
            else
            {
                xmlDocument.Load(str1);
            }

            Dictionary<int, TimeZoneInformation> nums = new Dictionary<int, TimeZoneInformation>();
            foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//TimeZones/TimeZone"))
            {
                SharePointTimeZoneInformation sharePointTimeZoneInformation =
                    new SharePointTimeZoneInformation(xmlNodes);
                if (nums.ContainsKey(sharePointTimeZoneInformation.ID))
                {
                    continue;
                }

                nums.Add(sharePointTimeZoneInformation.ID, sharePointTimeZoneInformation);
            }

            return nums;
        }

        public abstract DateTime UtcToLocalTime(DateTime utc);
    }
}