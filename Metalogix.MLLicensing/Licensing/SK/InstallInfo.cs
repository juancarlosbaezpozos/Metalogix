using Metalogix;
using Metalogix.Licensing;
using Metalogix.Licensing.Cryptography;
using System;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    public class InstallInfo
    {
        private const string _REGISTRY_KEY = "Mandatory";

        private DateTime _installDate;

        private DateTime _expirationDate;

        public DateTime ExpirationDate
        {
            get { return this._expirationDate; }
        }

        public DateTime InstallDate
        {
            get { return this._installDate; }
        }

        public InstallInfo()
        {
        }

        internal void Load()
        {
            try
            {
                Logger.Debug.Write("InstallInfo >> Load: started");
                string str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "Mandatory") as string;
                if (str != null)
                {
                    Logger.Debug.Write("InstallInfo >> Load: info found");
                }
                else
                {
                    Trace.WriteLine(string.Format("InstallInfo >> Load: Registry key '{0}' doesn`t exists.",
                        string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase,
                            "\\Mandatory")));
                }

                if (str == null)
                {
                    str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                        string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, "Microsoft\\",
                            SKLP.Get.InitData.InstallInfoHiddenKey), "Mandatory") as string;
                    Logger.Debug.Write("InstallInfo >> Load: trying second location.");
                }

                if (str == null)
                {
                    Trace.WriteLine(string.Format("InstallInfo >> Load: Registry key '{0}' doesn`t exists.",
                        string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, "Microsoft\\",
                            SKLP.Get.InitData.InstallInfoHiddenKey, "\\Mandatory")));
                    throw new Exception("Cannot load install info.");
                }

                str = Crypter.Decrypt(str);
                string[] strArrays = str.Split(new char[] { '|' });
                string str1 = strArrays[0];
                int num = Convert.ToInt32(str1.Substring(0, 4));
                int num1 = Convert.ToInt32(str1.Substring(4, 2));
                int num2 = Convert.ToInt32(str1.Substring(6, 2));
                this._installDate = new DateTime(num, num1, num2, 0, 0, 0, 0);
                str1 = strArrays[1];
                num = Convert.ToInt32(str1.Substring(0, 4));
                num1 = Convert.ToInt32(str1.Substring(4, 2));
                num2 = Convert.ToInt32(str1.Substring(6, 2));
                this._expirationDate = new DateTime(num, num1, num2, 0, 0, 0, 0);
                ILogMethods debug = Logger.Debug;
                object[] objArray = new object[] { this._installDate, this._expirationDate };
                debug.WriteFormat("InstallInfo >> Load: successfully loaded, installDate={0}, expirationDate={1}.",
                    objArray);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Concat("InstallInfo >> Load: ", exception.ToString()));
                throw new LicenseHackedException(
                    "Unable to load install info. The installation was not completed or the registry was tampered with.");
            }
        }
    }
}