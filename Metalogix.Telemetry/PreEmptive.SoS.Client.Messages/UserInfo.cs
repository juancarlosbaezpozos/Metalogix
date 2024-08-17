using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Diagnostics;
using System.Management;
using System.Security.Principal;

namespace PreEmptive.SoS.Client.Messages
{
    public sealed class UserInfo
    {
        private bool hashSensitiveData;

        private string name = PreEmptive.SoS.Client.Messages.UserInfo.UserName;

        private static TraceSwitch traceSwitch;

        private static bool isAdministrator;

        private PreEmptive.SoS.Client.MessageProxies.UserInfo proxy;

        private bool omitPII;

        public bool HashSensitiveData
        {
            get { return this.hashSensitiveData; }
            set { this.hashSensitiveData = value; }
        }

        public static bool IsAdministator
        {
            get
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                PreEmptive.SoS.Client.Messages.UserInfo.isAdministrator =
                    (new WindowsPrincipal(current)).IsInRole(WindowsBuiltInRole.Administrator);
                return PreEmptive.SoS.Client.Messages.UserInfo.isAdministrator;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public bool OmitPersonalInformation
        {
            get { return this.omitPII; }
            set { this.omitPII = value; }
        }

        internal PreEmptive.SoS.Client.MessageProxies.UserInfo Proxy
        {
            get { return this.proxy; }
        }

        private static string UserName
        {
            get
            {
                string userName = Environment.UserName;
                try
                {
                    ManagementObjectSearcher managementObjectSearcher =
                        new ManagementObjectSearcher(string.Concat("SELECT * FROM Win32_ComputerSystem WHERE Name='",
                            Environment.MachineName, "'"));
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        userName = (string)managementObject["UserName"];
                    }
                }
                catch (Exception exception)
                {
                    PreEmptive.SoS.Client.Messages.UserInfo.DisplayError(exception);
                }

                return userName;
            }
        }

        static UserInfo()
        {
            PreEmptive.SoS.Client.Messages.UserInfo.isAdministrator = true;
        }

        public UserInfo()
        {
        }

        private static void DisplayError(Exception exception_0)
        {
            PreEmptive.SoS.Client.Messages.UserInfo.traceSwitch = new TraceSwitch("traceSwitch",
                "This is a trace switch defined in App.config file.");
            Trace.WriteLineIf(PreEmptive.SoS.Client.Messages.UserInfo.traceSwitch.TraceInfo,
                string.Format(exception_0.ToString(), new object[0]));
        }

        internal void FillInProxy(ApplicationLifeCycle applicationLifeCycle_0)
        {
            if (applicationLifeCycle_0.User == null)
            {
                applicationLifeCycle_0.User = new PreEmptive.SoS.Client.MessageProxies.UserInfo();
            }

            string str = null;
            str = (!this.omitPII ? this.Hash(this.name) : InstanceIdUtil.GetInstanceId());
            applicationLifeCycle_0.User.Name = str;
            applicationLifeCycle_0.User.IsAdministrator =
                (this.omitPII ? false : PreEmptive.SoS.Client.Messages.UserInfo.IsAdministator);
            this.proxy = applicationLifeCycle_0.User;
        }

        private string Hash(string data)
        {
            if (!this.hashSensitiveData)
            {
                return data;
            }

            return Cryptography.Hash(data);
        }
    }
}