using Metalogix.Licensing;
using Metalogix.Licensing.Storage;
using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing.LicenseServer
{
    public sealed class ServerProxySettings : AbstractFile
    {
        public bool Enabled { get; private set; }

        public string Password { get; private set; }

        public int Port { get; private set; }

        public string Server { get; private set; }

        public string UserName { get; private set; }

        public ServerProxySettings(IDataStorage storage) : base(storage)
        {
            this.Load();
        }

        public ServerProxySettings(IDataStorage storage, bool enabled, string serverName, int _port, string userName,
            string password) : base(storage)
        {
            if (enabled && string.IsNullOrEmpty(serverName))
            {
                throw new ArgumentNullException("serverName");
            }

            if (enabled && (_port < 0 || _port > 65535))
            {
                throw new ArgumentException("port");
            }

            if (enabled && userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (enabled && password == null)
            {
                throw new ArgumentNullException("password");
            }

            this.Enabled = enabled;
            this.Server = serverName ?? string.Empty;
            this.Port = _port;
            this.UserName = userName ?? string.Empty;
            this.Password = password ?? string.Empty;
        }

        internal WebProxy GetWebProxy()
        {
            if (!this.Enabled)
            {
                return null;
            }

            object[] server = new object[] { "http://", this.Server, ":", this.Port };
            WebProxy webProxy = new WebProxy(string.Concat(server), true);
            if (string.IsNullOrEmpty(this.UserName))
            {
                webProxy.Credentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                webProxy.Credentials = new NetworkCredential(this.UserName, this.Password);
            }

            return webProxy;
        }

        public override void Load()
        {
            bool exists = base.StorageHandler.Exists;
            if (exists)
            {
                base.StorageHandler.Load();
            }

            this.Enabled = (exists ? bool.Parse(base.StorageHandler.GetValue("Enabled")) : false);
            this.Server = (exists ? base.StorageHandler.GetValue("Server") : string.Empty);
            this.Port = (exists ? int.Parse(base.StorageHandler.GetValue("Port")) : 0);
            this.UserName = (exists ? base.StorageHandler.GetValue("UserName") : string.Empty);
            this.Password = (exists ? base.StorageHandler.GetValue("Password") : string.Empty);
        }

        public override void Save()
        {
            base.StorageHandler.SetValue("Enabled", this.Enabled.ToString());
            base.StorageHandler.SetValue("Port", this.Port.ToString());
            base.StorageHandler.SetValue("Server", this.Server ?? string.Empty);
            base.StorageHandler.SetValue("UserName", this.UserName ?? string.Empty);
            base.StorageHandler.SetValue("Password", this.Password ?? string.Empty);
            base.StorageHandler.Save();
        }

        public override string ToString()
        {
            if (!this.Enabled)
            {
                return "Disabled";
            }

            object[] enabled = new object[] { this.Enabled, this.Server, this.Port, this.UserName, this.Password };
            return string.Format("Enabled: {0}, Server: {1}, Port: {2}, UserName: {3}, Password: {4}", enabled);
        }
    }
}