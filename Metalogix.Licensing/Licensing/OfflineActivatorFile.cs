using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using Metalogix.Licensing.Storage;
using System;

namespace Metalogix.Licensing
{
    public sealed class OfflineActivatorFile : IDisposable
    {
        private const bool _IS_SECURE = true;

        private readonly LicenseFile _responseFile;

        private readonly LicenseInfoRequestFile _requestFile;

        public bool IsActivationRequest
        {
            get { return this._responseFile == null; }
        }

        public LicenseInfoRequest RequestContent
        {
            get
            {
                if (this._requestFile == null)
                {
                    return null;
                }

                return this._requestFile.RequestContent;
            }
        }

        public string RequestString
        {
            get
            {
                if (!this.IsActivationRequest)
                {
                    return null;
                }

                return ((StringDataStorage)this._requestFile.StorageHandler).Value;
            }
        }

        public System.Version RequestVersion
        {
            get
            {
                if (!this.IsActivationRequest)
                {
                    return null;
                }

                return this._requestFile.Version;
            }
        }

        public LicenseInfoResponse ResponseContent
        {
            get
            {
                if (this._responseFile == null)
                {
                    return null;
                }

                return this._responseFile.Content;
            }
        }

        public string ResponseString
        {
            get
            {
                if (this.IsActivationRequest)
                {
                    return null;
                }

                return ((StringDataStorage)this._responseFile.StorageHandler).Value;
            }
        }

        public OfflineActivatorFile(LicenseInfoRequest data)
        {
            Logger.Debug.WriteFormat("OfflineActivatorFile >> Ctor: Construction from request", new object[0]);
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this._requestFile = new LicenseInfoRequestFile(new StringDataStorage(true), data);
            this.Save();
        }

        public OfflineActivatorFile(LicenseInfoResponse data, string publicKey) : this(data, publicKey,
            Tools.ClientVersion)
        {
        }

        public OfflineActivatorFile(LicenseInfoResponse data, string publicKey, System.Version clientVersion)
        {
            Logger.Debug.WriteFormat("OfflineActivatorFile >> Ctor: Construction from response", new object[0]);
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this._responseFile =
                new LicenseFile(
                    new StringDataStorage(true, (new ClientVersionSettings(clientVersion)).EncryptionMethod), publicKey,
                    clientVersion)
                {
                    Content = data
                };
            this.Save();
        }

        public OfflineActivatorFile(string data, string publicKey) : this(data, publicKey, null)
        {
        }

        public OfflineActivatorFile(string data, string publicKey, System.Version clientVersion)
        {
            StringDataStorage stringDataStorage;
            Logger.Debug.WriteFormat("OfflineActivatorFile >> Ctor: Construction from string", new object[0]);
            stringDataStorage = (clientVersion == null
                ? new StringDataStorage(true, data)
                : new StringDataStorage(true, (new ClientVersionSettings(clientVersion)).EncryptionMethod, data));
            stringDataStorage.Load();
            if (!stringDataStorage.Exists || stringDataStorage.GetValue("Key") == null)
            {
                this._responseFile = new LicenseFile(stringDataStorage, publicKey);
            }
            else
            {
                this._requestFile = new LicenseInfoRequestFile(stringDataStorage);
            }

            this.Load();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._requestFile.Dispose();
                this._responseFile.Dispose();
            }
        }

        public void Load()
        {
            Logger.Debug.Write("OfflineActivatorFile >> Load: Entered");
            if (this.IsActivationRequest)
            {
                this._requestFile.Load();
                ILogMethods debug = Logger.Debug;
                object[] key = new object[] { this._requestFile.RequestContent.Key };
                debug.WriteFormat("OfflineActivatorFile >> Load: Successfully loaded from the Store '{0}'.", key);
                return;
            }

            this._responseFile.Load();
            ILogMethods logMethod = Logger.Debug;
            object[] objArray = new object[] { this._responseFile.License.Key };
            logMethod.WriteFormat(
                "OfflineActivatorFile >> Load: Successfully loaded from the Store '{0}', validating the data",
                objArray);
        }

        public void Save()
        {
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { (this.IsActivationRequest ? "request" : "response") };
            debug.WriteFormat("OfflineActivatorFile >> Save: Saving {0}", objArray);
            if (!this.IsActivationRequest)
            {
                this._responseFile.Save();
            }
            else
            {
                this._requestFile.Save();
            }

            ILogMethods logMethod = Logger.Debug;
            object[] objArray1 = new object[]
                { (this.IsActivationRequest ? this.RequestContent.Key : this.ResponseContent.License.Key) };
            logMethod.WriteFormat("OfflineActivatorFile >> Save: Saved successfully {0}", objArray1);
        }
    }
}