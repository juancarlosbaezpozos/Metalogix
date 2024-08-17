using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using Metalogix.Licensing.Storage;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing
{
    internal class LicenseInfoRequestFile : AbstractFile
    {
        public LicenseInfoRequest RequestContent { get; private set; }

        public string RequestString
        {
            get { return ((StringDataStorage)base.StorageHandler).Value; }
        }

        public System.Version Version { get; private set; }

        public LicenseInfoRequestFile(IDataStorage storage, LicenseInfoRequest data) : this(storage)
        {
            Logger.Debug.WriteFormat("LicenseInfoRequestFile >> Ctor: Construction from request", new object[0]);
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.RequestContent = data;
        }

        public LicenseInfoRequestFile(IDataStorage storage) : base(storage)
        {
        }

        public override void Load()
        {
            string value;
            Logger.Debug.Write("LicenseInfoRequestFile >> Load: Entered");
            if (!base.StorageHandler.Exists)
            {
                Logger.Warning.Write("LicenseInfoRequestFile >> Load: Data was not set, throwing exception");
                throw new FileNotFoundException("Offline content was not set.");
            }

            base.StorageHandler.Load();
            ClientVersionSettings clientVersionSetting = new ClientVersionSettings(base.StorageHandler);
            this.Version = clientVersionSetting.Version;
            LicenseInfoRequest licenseInfoRequest = new LicenseInfoRequest()
            {
                Server = base.StorageHandler.GetValue("Server"),
                Admin = base.StorageHandler.GetValue("Admin"),
                Key = base.StorageHandler.GetValue("Key"),
                UsedData = Convert.ToInt64(base.StorageHandler.GetValue("UsedData")),
                ServerCount = (!string.IsNullOrEmpty(base.StorageHandler.GetValue("ServerCount"))
                    ? Convert.ToInt32(base.StorageHandler.GetValue("ServerCount"))
                    : 0)
            };
            if (clientVersionSetting.ShouldSendProdCodeAndVersion)
            {
                licenseInfoRequest.ProductCode = Convert.ToInt32(base.StorageHandler.GetValue("ProductCode"));
                licenseInfoRequest.ProductVersion = base.StorageHandler.GetValue("ProductVersion");
                licenseInfoRequest.Message = base.StorageHandler.GetValue("Message");
            }

            LicenseInfoRequest licenseInfoRequest1 = licenseInfoRequest;
            if (clientVersionSetting.IsSystemInfoStored)
            {
                value = base.StorageHandler.GetValue("SystemInfo");
            }
            else
            {
                value = null;
            }

            licenseInfoRequest1.SystemInfo = value;
            this.RequestContent = licenseInfoRequest;
            Logger.Debug.WriteFormat("LicenseInfoRequestFile >> Load: Successfully loaded from the Store '{0}'.",
                new object[] { licenseInfoRequest.Key });
        }

        public override void Save()
        {
            Logger.Debug.WriteFormat("LicenseInfoRequestFile >> Save: Saving request", new object[0]);
            base.StorageHandler.SetValue("ClientVersion", Tools.ClientVersion.ToString());
            base.StorageHandler.SetValue("Admin", this.RequestContent.Admin);
            base.StorageHandler.SetValue("Key", this.RequestContent.Key);
            base.StorageHandler.SetValue("Server", this.RequestContent.Server);
            base.StorageHandler.SetValue("ServerCount", this.RequestContent.ServerCount.ToString());
            base.StorageHandler.SetValue("UsedData", this.RequestContent.UsedData.ToString());
            base.StorageHandler.SetValue("ProductCode", this.RequestContent.ProductCode.ToString());
            base.StorageHandler.SetValue("ProductVersion", this.RequestContent.ProductVersion);
            base.StorageHandler.SetValue("SystemInfo", this.RequestContent.SystemInfo);
            base.StorageHandler.SetValue("Message", this.RequestContent.Message);
            base.StorageHandler.Save();
            ILogMethods debug = Logger.Debug;
            object[] key = new object[] { this.RequestContent.Key };
            debug.WriteFormat("LicenseInfoRequestFile >> Save: Saved successfully {0}", key);
        }
    }
}