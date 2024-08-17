using Metalogix;
using Metalogix.Licensing.Cryptography;
using System;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    public class LicenseProxy
    {
        private const string _REGISTRY_KEY = "LicenseProxy";

        protected bool _enabled;

        protected string _server = "";

        protected string _port = "";

        protected string _user = "";

        protected string _pass = "";

        public bool Enabled
        {
            get { return this._enabled; }
            set { this._enabled = value; }
        }

        public string Pass
        {
            get { return this._pass; }
            set { this._pass = value; }
        }

        public string Port
        {
            get { return this._port; }
            set { this._port = value; }
        }

        public string Server
        {
            get { return this._server; }
            set { this._server = value; }
        }

        public string User
        {
            get { return this._user; }
            set { this._user = value; }
        }

        public LicenseProxy()
        {
        }

        public LicenseProxy(bool enabled, string server, string port, string user, string password)
        {
            this._enabled = enabled;
            this._server = server;
            this._port = port;
            this._user = user;
            this._pass = password;
        }

        internal void Load()
        {
            try
            {
                string str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseProxy") as string;
                if (str != null)
                {
                    Logger.Debug.WriteFormat("LicenseProxy >> Load: Parsing proxy '{0}'", new object[] { str });
                    str = Crypter.Decrypt(str);
                    string[] strArrays = str.Split(new char[] { '|' });
                    this.Enabled = Convert.ToBoolean(strArrays[0]);
                    this.Server = strArrays[1];
                    this.Port = strArrays[2];
                    this.User = strArrays[3];
                    this.Pass = strArrays[4];
                    Logger.Debug.WriteFormat("LicenseProxy >> Load: Proxy sucessfully loaded '{0}'",
                        new object[] { this });
                }
                else
                {
                    ILogMethods debug = Logger.Debug;
                    object[] objArray = new object[]
                        { string.Concat(SKLP.Get.InitData.RegistryBase, "\\LicenseProxy") };
                    debug.WriteFormat("LicenseProxy >> Load: Registry key '{0}' doesn`t extists.", objArray);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("LicenseProxy >> Load: ", exception));
                throw new Exception("Unable to load proxy settings", exception);
            }
        }

        internal void Save()
        {
            try
            {
                object[] enabled = new object[]
                    { this.Enabled, "|", this.Server, "|", this.Port, "|", this.User, "|", this.Pass };
                string str = Crypter.Encrypt(string.Concat(enabled));
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseProxy", str);
                Trace.WriteLine("LicenseProxy >> Save >> Proxy settings were set successfully.");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("LicenseProxy >> Unable to set the proxy settings. ", exception));
                throw new Exception("Unable to save proxy settings", exception);
            }
        }

        public override string ToString()
        {
            object[] objArray = new object[]
                { this._enabled, this._server, this._port, this._user, !string.IsNullOrEmpty(this._pass) };
            return string.Format("Enabled={0}, Server={1}, Port={2}, User={3}, Pass set={4}", objArray);
        }
    }
}