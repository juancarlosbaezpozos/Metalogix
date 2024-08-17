using Metalogix.Licensing;
using Metalogix.Licensing.Cryptography;
using Metalogix.Licensing.Storage;
using System;

namespace Metalogix.Licensing.LicenseServer
{
    internal class ClientVersionSettings
    {
        private readonly System.Version _clientVersion;

        private readonly bool _versionExists;

        private readonly bool _isStrictValidation;

        private readonly static System.Version _v5100;

        private readonly static System.Version _v5102;

        private readonly static System.Version _v6100;

        private readonly static System.Version _v6200;

        public Encryption EncryptionMethod
        {
            get
            {
                if ((this._isStrictValidation && this.VersionExists || !this._isStrictValidation) &&
                    this._clientVersion >= ClientVersionSettings._v5102)
                {
                    return Encryption.TripleDES;
                }

                return Encryption.Rijndael;
            }
        }

        public bool IsCustomerInfoStored
        {
            get
            {
                if ((!this._isStrictValidation || !this.VersionExists) && this._isStrictValidation)
                {
                    return false;
                }

                return this._clientVersion >= ClientVersionSettings._v5100;
            }
        }

        public bool IsIsFremiumStored
        {
            get
            {
                if ((!this._isStrictValidation || !this.VersionExists) && this._isStrictValidation)
                {
                    return false;
                }

                return this._clientVersion >= ClientVersionSettings._v6200;
            }
        }

        public bool IsSystemInfoStored
        {
            get
            {
                if ((!this._isStrictValidation || !this.VersionExists) && this._isStrictValidation)
                {
                    return false;
                }

                return this._clientVersion >= ClientVersionSettings._v6100;
            }
        }

        public bool IsUserDataTiedToServerAndKey
        {
            get
            {
                if ((!this._isStrictValidation || !this.VersionExists) && this._isStrictValidation)
                {
                    return false;
                }

                return this._clientVersion >= ClientVersionSettings._v5100;
            }
        }

        public bool ShouldSendProdCodeAndVersion
        {
            get
            {
                if ((!this._isStrictValidation || !this.VersionExists) && this._isStrictValidation)
                {
                    return false;
                }

                return this._clientVersion >= ClientVersionSettings._v5100;
            }
        }

        public System.Version Version
        {
            get { return this._clientVersion; }
        }

        public bool VersionExists
        {
            get { return this._versionExists; }
        }

        static ClientVersionSettings()
        {
            ClientVersionSettings._v5100 = new System.Version(5, 1, 0, 0);
            ClientVersionSettings._v5102 = new System.Version(5, 1, 0, 2);
            ClientVersionSettings._v6100 = new System.Version(6, 1, 0, 0);
            ClientVersionSettings._v6200 = new System.Version(6, 2, 0, 0);
        }

        public ClientVersionSettings(System.Version version) : this(version, true, false)
        {
        }

        public ClientVersionSettings(IDataStorage storage)
        {
            string value = storage.GetValue("ClientVersion");
            if (string.IsNullOrEmpty(value))
            {
                this._clientVersion = Tools.ClientVersion;
                this._versionExists = false;
                return;
            }

            this._clientVersion = new System.Version(value);
            this._versionExists = true;
        }

        private ClientVersionSettings(System.Version version, bool exists, bool isStrict)
        {
            this._clientVersion = version;
            this._versionExists = exists;
            this._isStrictValidation = isStrict;
        }

        public ClientVersionSettings Strict()
        {
            return new ClientVersionSettings(this._clientVersion, this._versionExists, true);
        }
    }
}