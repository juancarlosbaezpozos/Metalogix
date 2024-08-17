using Metalogix.Licensing.Cryptography;
using Metalogix.Licensing.Logging;
using System;

namespace Metalogix.Licensing.Storage
{
    public class StringDataStorage : IDataStorage, IDisposable
    {
        private bool _isSecure;

        private ValueCollection _values;

        private readonly string _data;

        private readonly Encryption _encryptionMethod;

        public bool Exists
        {
            get { return this._data != null; }
        }

        public bool IsSecure
        {
            get { return this._isSecure; }
            set { this._isSecure = value; }
        }

        public string Value
        {
            get
            {
                if (!this._isSecure)
                {
                    return this._values.ToString();
                }

                return Crypter.Encrypt(this._values.ToString(), this._encryptionMethod);
            }
        }

        public StringDataStorage(bool isSecure) : this(isSecure, Encryption.Preferred)
        {
        }

        public StringDataStorage(bool isSecure, Encryption encryptionMethod)
        {
            Logger.Debug.WriteFormat("StringDataStorage >> Ctor: Entered with data: IsSecure={0}",
                new object[] { isSecure });
            this._encryptionMethod = encryptionMethod;
            this._isSecure = isSecure;
            this._values = new ValueCollection(null);
        }

        public StringDataStorage(bool isSecure, string data) : this(isSecure, Encryption.Preferred, data)
        {
        }

        public StringDataStorage(bool isSecure, Encryption encryptionMethod, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { data, isSecure };
            debug.WriteFormat("StringDataStorage >> Ctor: Entered with data: Data={0}; IsSecure={1}", objArray);
            this._encryptionMethod = encryptionMethod;
            this._isSecure = isSecure;
            this._data = data;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Save();
            }
        }

        ~StringDataStorage()
        {
            this.Dispose(false);
        }

        public string GetValue()
        {
            Logger.Debug.Write("StringDataStorage >> GetValue");
            return this._values.RawValue;
        }

        public string GetValue(string name)
        {
            string item = this._values[name];
            Logger.Debug.WriteFormat("StringDataStorage >> GetValue: Key={0}; Val={1}", new object[] { name, item });
            return item;
        }

        public void Load()
        {
            Logger.Debug.Write("StringDataStorage >> Load: Entered");
            this._values =
                new ValueCollection((!this._isSecure || this._data == null ? this._data : Crypter.Decrypt(this._data)));
        }

        public void Save()
        {
        }

        public void SetValue(string name, string value)
        {
            Logger.Debug.WriteFormat("StringDataStorage >> SetValue: Key={0}; Val={1}", new object[] { name, value });
            this._values[name] = value;
        }
    }
}