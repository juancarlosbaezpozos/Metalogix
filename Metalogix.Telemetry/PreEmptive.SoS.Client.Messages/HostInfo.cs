using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Diagnostics;
using System.Management;
using System.Net;

namespace PreEmptive.SoS.Client.Messages
{
    public sealed class HostInfo
    {
        private string name;

        private static TraceSwitch traceSwitch;

        private string runtimeVersion = Environment.Version.ToString();

        private string osVersion = Environment.OSVersion.ToString();

        private bool hashSensitiveData;

        private OSInformation osinformation_0;

        private PreEmptive.SoS.Client.MessageProxies.HostInfo proxy;

        private string ipAddress;

        private bool omitPII;

        public bool HashSensitiveData
        {
            set { this.hashSensitiveData = value; }
        }

        public string IPAddress
        {
            get
            {
                if (!this.omitPII)
                {
                    try
                    {
                        this.ipAddress = Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
                    }
                    catch (Exception exception)
                    {
                        this.ipAddress = "0.0.0.0";
                    }
                }
                else
                {
                    this.ipAddress = "0.0.0.0";
                }

                return this.ipAddress;
            }
            set { this.ipAddress = value; }
        }

        private static string MachineName
        {
            get
            {
                string machineName = Environment.MachineName;
                try
                {
                    ManagementObjectSearcher managementObjectSearcher =
                        new ManagementObjectSearcher(string.Concat("SELECT * FROM Win32_ComputerSystem WHERE Name='",
                            machineName, "'"));
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        machineName = string.Concat(machineName, ".", (string)managementObject["Domain"]);
                    }
                }
                catch (Exception exception)
                {
                    PreEmptive.SoS.Client.Messages.HostInfo.DisplayError(exception);
                }

                return machineName;
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public bool OmitPersonalInformation
        {
            set { this.omitPII = value; }
        }

        public OSInformation OS
        {
            get
            {
                if (this.osinformation_0 == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")).Get();
                        this.osinformation_0 = new OSInformation();
                        foreach (ManagementObject managementObject in managementObjectCollections)
                        {
                            this.osinformation_0.IsVirtualized = false;
                            this.osinformation_0.Locale = (string)managementObject["Locale"];
                            this.osinformation_0.OsInstallDate =
                                PreEmptive.SoS.Client.Messages.HostInfo.ToDateTime(
                                    (string)managementObject["InstallDate"]);
                            OSInformation osinformation0 = this.osinformation_0;
                            long num = Convert.ToInt64(managementObject["OSLanguage"]);
                            osinformation0.OSLanguage = num.ToString();
                            this.osinformation_0.OsName = (string)managementObject["Caption"];
                            this.osinformation_0.OsProductId =
                                (this.omitPII ? "N/A" : this.Hash((string)managementObject["SerialNumber"]));
                            this.osinformation_0.OsServicePackMajorVersion =
                                Convert.ToInt64(managementObject["ServicePackMajorVersion"]);
                            this.osinformation_0.OsServicePackMinorVersion =
                                Convert.ToInt64(managementObject["ServicePackMinorVersion"]);
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.HostInfo.DisplayError(exception);
                    }
                }

                return this.osinformation_0;
            }
        }

        internal PreEmptive.SoS.Client.MessageProxies.HostInfo Proxy
        {
            get { return this.proxy; }
        }

        public string RuntimeVersion
        {
            get { return this.runtimeVersion; }
            set { this.runtimeVersion = value; }
        }

        public HostInfo()
        {
            this.name = PreEmptive.SoS.Client.Messages.HostInfo.MachineName;
        }

        private static void DisplayError(Exception exception_0)
        {
            PreEmptive.SoS.Client.Messages.HostInfo.traceSwitch = new TraceSwitch("traceSwitch",
                "This is a trace switch defined in App.config file.");
            Trace.WriteLineIf(PreEmptive.SoS.Client.Messages.HostInfo.traceSwitch.TraceInfo,
                string.Format(exception_0.ToString(), new object[0]));
        }

        internal void FillInProxy(ApplicationLifeCycle applicationLifeCycle_0)
        {
            if (applicationLifeCycle_0.Host == null)
            {
                applicationLifeCycle_0.Host = new PreEmptive.SoS.Client.MessageProxies.HostInfo();
            }

            applicationLifeCycle_0.Host.RuntimeVersion = this.runtimeVersion;
            if (applicationLifeCycle_0.Host.OS == null)
            {
                applicationLifeCycle_0.Host.OS = this.OS;
            }

            applicationLifeCycle_0.Host.Name = (this.omitPII ? "N/A" : this.Hash(this.name));
            applicationLifeCycle_0.Host.IPAddress = this.IPAddress;
            this.proxy = applicationLifeCycle_0.Host;
        }

        private string Hash(string data)
        {
            if (!this.hashSensitiveData)
            {
                return data;
            }

            return Cryptography.Hash(data);
        }

        private static DateTime ToDateTime(string dmtfDate)
        {
            int year = DateTime.Now.Year;
            int num = 1;
            int num1 = 1;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            string str = dmtfDate;
            if (string.Empty == str || str == null)
            {
                return DateTime.MinValue;
            }

            if (str.Length != 25)
            {
                return DateTime.MinValue;
            }

            string str1 = str.Substring(0, 4);
            if ("****" != str1)
            {
                year = int.Parse(str1);
            }

            str1 = str.Substring(4, 2);
            if ("**" != str1)
            {
                num = int.Parse(str1);
            }

            str1 = str.Substring(6, 2);
            if ("**" != str1)
            {
                num1 = int.Parse(str1);
            }

            str1 = str.Substring(8, 2);
            if ("**" != str1)
            {
                num2 = int.Parse(str1);
            }

            str1 = str.Substring(10, 2);
            if ("**" != str1)
            {
                num3 = int.Parse(str1);
            }

            str1 = str.Substring(12, 2);
            if ("**" != str1)
            {
                num4 = int.Parse(str1);
            }

            str1 = str.Substring(15, 3);
            if ("***" != str1)
            {
                num5 = int.Parse(str1);
            }

            DateTime dateTime = new DateTime(year, num, num1, num2, num3, num4, num5);
            return dateTime;
        }
    }
}