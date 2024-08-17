using Metalogix.Licensing.Cryptography;
using Metalogix.Licensing.Logging;
using Microsoft.Win32;
using System;
using System.Threading;

namespace Metalogix.Licensing.Storage
{
    public class RegistryDataStorage : IDataStorage, IDisposable
    {
        private const string C_GLOBAL_REGISTRY_DATA_STORAGE_MUTEX = "Global\\MetalogixRegistryDataStorageMutex";

        private readonly string _keyName;

        private readonly string _valueName;

        private readonly bool _isSecure;

        private ValueCollection _values;

        public bool Exists
        {
            get
            {
                bool flag = false;
                bool flag1;
                using (RegistryKey key = RegistryDataStorage.GetKey(this._keyName, false))
                {
                    object value = null;
                    if (key != null)
                    {
                        using (Mutex mutex = new Mutex(false, "Global\\MetalogixRegistryDataStorageMutex", out flag))
                        {
                            try
                            {
                                mutex.WaitOne();
                                value = key.GetValue(this._valueName);
                            }
                            finally
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }

                    flag1 = (key == null ? false : value != null);
                }

                return flag1;
            }
        }

        public RegistryDataStorage(string keyName, string valueName, bool isSecure)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException("keyName");
            }

            if (string.IsNullOrEmpty(valueName))
            {
                throw new ArgumentNullException("valueName");
            }

            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { keyName, valueName, isSecure };
            debug.WriteFormat("RegistryDataStorage >> Ctor: Entered with data: Key={0}; Value={1}; IsSecure={2}",
                objArray);
            this._keyName = keyName;
            this._valueName = valueName;
            this._isSecure = isSecure;
            this._values = new ValueCollection(null);
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

        ~RegistryDataStorage()
        {
            this.Dispose(false);
        }

        private static RegistryKey GetKey(string name, bool writable)
        {
            RegistryKey classesRoot;
            bool flag = false;
            RegistryKey registryKey;
            object[] objArray;
            ILogMethods warning;
            char[] chrArray = new char[] { '\\', ' ' };
            string str = name.Trim(chrArray);
            char[] chrArray1 = new char[] { '\\' };
            string[] strArrays = str.Split(chrArray1, StringSplitOptions.RemoveEmptyEntries);
            if ((int)strArrays.Length < 2)
            {
                Logger.Warning.WriteFormat("RegistryDataStorage >> GetKey: Failed, too short path '{0}'",
                    new object[] { name });
                throw new ArgumentException(
                    "Invalid registry base key. The base key must include the registry root followed by the sub path ('HKEY_LOCAL_MACHINE\\SOFTWARE\\...').");
            }

            string upper = strArrays[0].ToUpper();
            string str1 = upper;
            if (upper != null)
            {
                if (str1 == "HKEY_CLASSES_ROOT")
                {
                    classesRoot = Registry.ClassesRoot;
                }
                else if (str1 == "HKEY_CURRENT_USER")
                {
                    classesRoot = Registry.CurrentUser;
                }
                else if (str1 == "HKEY_LOCAL_MACHINE")
                {
                    classesRoot = Registry.LocalMachine;
                }
                else if (str1 == "HKEY_USERS")
                {
                    classesRoot = Registry.Users;
                }
                else
                {
                    if (str1 != "HKEY_CURRENT_CONFIG")
                    {
                        warning = Logger.Warning;
                        objArray = new object[] { strArrays[0], name };
                        warning.WriteFormat("RegistryDataStorage >> GetKey: Invalid root path '{0}', fullPath={1}",
                            objArray);
                        throw new Exception(
                            "Incorrect registry root. The base key must include the registry root followed by the sub path ('HKEY_LOCAL_MACHINE\\SOFTWARE\\...').");
                    }

                    classesRoot = Registry.CurrentConfig;
                }

                string str2 = string.Join("\\", strArrays, 1, (int)strArrays.Length - 1);
                using (classesRoot)
                {
                    RegistryKey registryKey1 = classesRoot.OpenSubKey(str2, writable);
                    if (writable && registryKey1 == null)
                    {
                        using (Mutex mutex = new Mutex(false, "Global\\MetalogixRegistryDataStorageMutex", out flag))
                        {
                            try
                            {
                                mutex.WaitOne();
                                registryKey1 = classesRoot.CreateSubKey(str2);
                            }
                            finally
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }

                    registryKey = registryKey1;
                }

                return registryKey;
            }

            warning = Logger.Warning;
            objArray = new object[] { strArrays[0], name };
            warning.WriteFormat("RegistryDataStorage >> GetKey: Invalid root path '{0}', fullPath={1}", objArray);
            throw new Exception(
                "Incorrect registry root. The base key must include the registry root followed by the sub path ('HKEY_LOCAL_MACHINE\\SOFTWARE\\...').");
        }

        public string GetValue()
        {
            Logger.Debug.Write("RegistryDataStorage >> GetValue");
            return this._values.RawValue;
        }

        public string GetValue(string name)
        {
            string item = this._values[name];
            Logger.Debug.WriteFormat("RegistryDataStorage >> GetValue: Key={0}; Val={1}", new object[] { name, item });
            return item;
        }

        public void Load()
        {
            bool flag = false;
            Logger.Debug.Write("RegistryDataStorage >> Load: Entered");
            using (RegistryKey key = RegistryDataStorage.GetKey(this._keyName, false))
            {
                string value = null;
                if (key != null)
                {
                    using (Mutex mutex = new Mutex(false, "Global\\MetalogixRegistryDataStorageMutex", out flag))
                    {
                        try
                        {
                            mutex.WaitOne();
                            value = key.GetValue(this._valueName) as string;
                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }
                    }
                }

                value = (!this._isSecure || value == null ? value : Crypter.Decrypt(value));
                Logger.Debug.WriteFormat("RegistryDataStorage >> Load: Values loaded={0}", new object[] { value });
                this._values = new ValueCollection(value);
            }
        }

        public void Save()
        {
            bool flag = false;
            Logger.Debug.Write("RegistryDataStorage >> Save: Entered");
            if (!this._values.IsDirty)
            {
                Logger.Debug.Write("RegistryDataStorage >> Save: skipped, not dirty.");
                return;
            }

            ILogMethods debug = Logger.Debug;
            object[] str = new object[] { this._keyName, this._valueName, this._isSecure, this._values.ToString() };
            debug.WriteFormat("RegistryDataStorage >> Save: saving to Key={0}; Val={1}; IsSec={2}; Values={3}", str);
            using (RegistryKey key = RegistryDataStorage.GetKey(this._keyName, true))
            {
                string str1 = (this._isSecure ? Crypter.Encrypt(this._values.ToString()) : this._values.ToString());
                using (Mutex mutex = new Mutex(false, "Global\\MetalogixRegistryDataStorageMutex", out flag))
                {
                    try
                    {
                        mutex.WaitOne();
                        key.SetValue(this._valueName, str1, RegistryValueKind.String);
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }

            Logger.Debug.Write("RegistryDataStorage >> Save: successfull.");
        }

        public void SetValue(string name, string value)
        {
            Logger.Debug.WriteFormat("RegistryDataStorage >> SetValue: Key={0}; Val={1}", new object[] { name, value });
            this._values[name] = value;
        }
    }
}