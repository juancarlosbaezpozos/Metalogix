using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using Metalogix.Licensing.Storage;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Licensing
{
    public class LicenseFile : AbstractFile
    {
        private readonly RSACryptoServiceProvider _csp;

        private readonly System.Version _clientVersion;

        public string AdminAccount
        {
            get
            {
                if (this.Content == null)
                {
                    return null;
                }

                return this.Content.Admin;
            }
        }

        public LicenseInfoResponse Content { get; set; }

        public LicenseInfo License
        {
            get
            {
                if (this.Content == null)
                {
                    return null;
                }

                return this.Content.License;
            }
        }

        public string ServerID
        {
            get
            {
                if (this.Content == null)
                {
                    return null;
                }

                return this.Content.Server;
            }
        }

        public long UsedData
        {
            get
            {
                if (this.Content == null)
                {
                    return (long)0;
                }

                return this.Content.UsedData;
            }
        }

        public LicenseFile(IDataStorage storage, string publicKey) : this(storage, publicKey, Tools.ClientVersion)
        {
        }

        public LicenseFile(IDataStorage storage, string publicKey, System.Version clientVersion) : base(storage)
        {
            ILogMethods debug = Logger.Debug;
            object[] type = new object[] { storage.GetType(), clientVersion, !string.IsNullOrEmpty(publicKey) };
            debug.WriteFormat("LicenseFile >> Ctor: storageType={0}; ClientVersion={1} PublicKeySet={2}", type);
            if (!string.IsNullOrEmpty(publicKey))
            {
                this._csp = new RSACryptoServiceProvider();
                this._csp.FromXmlString(publicKey);
            }

            this._clientVersion = clientVersion;
        }

        public override void Load()
        {
            Logger.Debug.Write("LicenseFile >> Load: Entered");
            if (!base.StorageHandler.Exists)
            {
                Logger.Warning.Write("LicenseFile >> Load: Not exists, throwing exception");
                throw new FileNotFoundException("License file doesn't exists.");
            }

            base.StorageHandler.Load();
            LicenseInfoResponse licenseInfoResponse = new LicenseInfoResponse();
            ClientVersionSettings clientVersionSetting = new ClientVersionSettings(base.StorageHandler);
            licenseInfoResponse.Admin = base.StorageHandler.GetValue("Admin");
            licenseInfoResponse.Server = base.StorageHandler.GetValue("Server");
            licenseInfoResponse.UsedData = Convert.ToInt64(base.StorageHandler.GetValue("UsedData"));
            licenseInfoResponse.Updated = Convert.ToDateTime(base.StorageHandler.GetValue("Updated"));
            if (clientVersionSetting.IsCustomerInfoStored)
            {
                licenseInfoResponse.CustomerName = base.StorageHandler.GetValue("CustomerName");
                licenseInfoResponse.ContactName = base.StorageHandler.GetValue("ContactName");
                licenseInfoResponse.ContactEmail = base.StorageHandler.GetValue("ContactEmail");
            }

            if (clientVersionSetting.IsSystemInfoStored)
            {
                licenseInfoResponse.SystemInfo = base.StorageHandler.GetValue("SystemInfo");
            }

            if (clientVersionSetting.IsIsFremiumStored)
            {
                licenseInfoResponse.IsFremium = base.StorageHandler.GetValue("IsFremium");
            }

            licenseInfoResponse.Signature = Convert.FromBase64String(base.StorageHandler.GetValue("Signature"));
            licenseInfoResponse.License = new LicenseInfo()
            {
                Key = base.StorageHandler.GetValue("License.Key"),
                LicensedAdmins = Convert.ToInt32(base.StorageHandler.GetValue("License.LicensedAdmins")),
                LicensedData = Convert.ToInt64(base.StorageHandler.GetValue("License.LicensedData")),
                LicensedServers = Convert.ToInt32(base.StorageHandler.GetValue("License.LicensedServers")),
                LicenseExpiration = Convert.ToDateTime(base.StorageHandler.GetValue("License.LicenseExpiration")),
                MaintenanceExpiration =
                    Convert.ToDateTime(base.StorageHandler.GetValue("License.MaintenanceExpiration")),
                MaxAllowedSetupVersion = base.StorageHandler.GetValue("License.MaxAllowedSetupVersion"),
                OfflineRevalidationDays =
                    Convert.ToInt32(base.StorageHandler.GetValue("License.OfflineRevalidationDays")),
                Status = (LicenseStatus)Enum.Parse(typeof(LicenseStatus),
                    base.StorageHandler.GetValue("License.Status")),
                Type = (LicenseType)Enum.Parse(typeof(LicenseType), base.StorageHandler.GetValue("License.Type")),
                UsedAdmins = Convert.ToInt32(base.StorageHandler.GetValue("License.UsedAdmins")),
                UsedData = Convert.ToInt64(base.StorageHandler.GetValue("License.UsedData")),
                UsedServers = Convert.ToInt32(base.StorageHandler.GetValue("License.UsedServers"))
            };
            string value = base.StorageHandler.GetValue("License.CustomFields");
            if (!string.IsNullOrEmpty(value))
            {
                string[] strArrays = value.Split(new char[] { '|' });
                if ((int)strArrays.Length % 2 != 0)
                {
                    throw new InvalidDataException("CustomFields contains invalid data.");
                }

                licenseInfoResponse.CustomFields = new CustomFields()
                {
                    Fields = new CustomField[(int)strArrays.Length / 2]
                };
                for (int i = 0; i < (int)strArrays.Length; i += 2)
                {
                    CustomField[] fields = licenseInfoResponse.CustomFields.Fields;
                    int num = i / 2;
                    CustomField customField = new CustomField()
                    {
                        Name = strArrays[i].Replace(string.Concat("#", '\u0010', "#"), "|"),
                        Value = strArrays[i + 1].Replace(string.Concat("#", '\u0010', "#"), "|")
                    };
                    fields[num] = customField;
                }
            }

            Logger.Debug.Write("LicenseFile >> Load: Successfully loaded from the Store, validating the data");
            this.ValidateInernal(licenseInfoResponse);
            Logger.Debug.Write("LicenseFile >> Load: Data valid, load OK.");
            this.Content = licenseInfoResponse;
        }

        public override void Save()
        {
            Logger.Debug.Write("LicenseFile >> Save: Entered");
            base.StorageHandler.SetValue("ClientVersion", this._clientVersion.ToString());
            base.StorageHandler.SetValue("Admin", this.Content.Admin);
            base.StorageHandler.SetValue("Server", this.Content.Server);
            base.StorageHandler.SetValue("UsedData", this.Content.UsedData.ToString());
            IDataStorage storageHandler = base.StorageHandler;
            DateTime updated = this.Content.Updated;
            storageHandler.SetValue("Updated", updated.ToString("s"));
            base.StorageHandler.SetValue("CustomerName", this.Content.CustomerName);
            base.StorageHandler.SetValue("ContactName", this.Content.ContactName);
            base.StorageHandler.SetValue("ContactEmail", this.Content.ContactEmail);
            base.StorageHandler.SetValue("Signature", Convert.ToBase64String(this.Content.Signature));
            base.StorageHandler.SetValue("SystemInfo", this.Content.SystemInfo);
            base.StorageHandler.SetValue("IsFremium", this.Content.IsFremium);
            base.StorageHandler.SetValue("License.Key", this.Content.License.Key);
            IDataStorage dataStorage = base.StorageHandler;
            int licensedAdmins = this.Content.License.LicensedAdmins;
            dataStorage.SetValue("License.LicensedAdmins", licensedAdmins.ToString());
            IDataStorage storageHandler1 = base.StorageHandler;
            long licensedData = this.Content.License.LicensedData;
            storageHandler1.SetValue("License.LicensedData", licensedData.ToString());
            IDataStorage dataStorage1 = base.StorageHandler;
            int licensedServers = this.Content.License.LicensedServers;
            dataStorage1.SetValue("License.LicensedServers", licensedServers.ToString());
            IDataStorage storageHandler2 = base.StorageHandler;
            DateTime licenseExpiration = this.Content.License.LicenseExpiration;
            storageHandler2.SetValue("License.LicenseExpiration", licenseExpiration.ToString("s"));
            IDataStorage dataStorage2 = base.StorageHandler;
            DateTime maintenanceExpiration = this.Content.License.MaintenanceExpiration;
            dataStorage2.SetValue("License.MaintenanceExpiration", maintenanceExpiration.ToString("s"));
            base.StorageHandler.SetValue("License.MaxAllowedSetupVersion", this.Content.License.MaxAllowedSetupVersion);
            IDataStorage storageHandler3 = base.StorageHandler;
            int offlineRevalidationDays = this.Content.License.OfflineRevalidationDays;
            storageHandler3.SetValue("License.OfflineRevalidationDays", offlineRevalidationDays.ToString());
            base.StorageHandler.SetValue("License.Status", this.Content.License.Status.ToString());
            base.StorageHandler.SetValue("License.Type", this.Content.License.Type.ToString());
            IDataStorage dataStorage3 = base.StorageHandler;
            int usedAdmins = this.Content.License.UsedAdmins;
            dataStorage3.SetValue("License.UsedAdmins", usedAdmins.ToString());
            IDataStorage storageHandler4 = base.StorageHandler;
            long usedData = this.Content.License.UsedData;
            storageHandler4.SetValue("License.UsedData", usedData.ToString());
            IDataStorage dataStorage4 = base.StorageHandler;
            int usedServers = this.Content.License.UsedServers;
            dataStorage4.SetValue("License.UsedServers", usedServers.ToString());
            if (this.Content.CustomFields != null && this.Content.CustomFields.Fields != null &&
                (int)this.Content.CustomFields.Fields.Length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                CustomField[] fields = this.Content.CustomFields.Fields;
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    CustomField customField = fields[i];
                    stringBuilder.AppendFormat("{0}|{1}|",
                        customField.Name.Replace("|", string.Concat("#", '\u0010', "#")),
                        customField.Value.Replace("|", string.Concat("#", '\u0010', "#")));
                }

                if (stringBuilder.Length > 1)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }

                base.StorageHandler.SetValue("License.CustomFields", stringBuilder.ToString());
            }

            base.StorageHandler.Save();
            Logger.Debug.Write("LicenseFile >> Save: Finished successfully.");
        }

        public void Validate()
        {
            this.ValidateInernal(this.Content);
        }

        private void ValidateInernal(SignedResponse lic)
        {
            if (this._csp == null)
            {
                Logger.Warning.Write("LicenseFile >> ValidateInernal: Public key NOT set, validation will be skipped.");
                return;
            }

            Logger.Debug.Write("LicenseFile >> ValidateInernal: Public key set, validating data.");
            if (!Metalogix.Licensing.LicenseServer.Signature.Validate(lic, this._csp))
            {
                Logger.Warning.Write("LicenseFile >> ValidateInernal: Signature incorrect.");
                throw new InvalidSignatureException();
            }

            Logger.Debug.Write("LicenseFile >> ValidateInernal: Signature OK.");
        }
    }
}