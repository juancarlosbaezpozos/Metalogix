using Metalogix.Licensing;
using Metalogix.Licensing.Cryptography;
using Metalogix.Licensing.Logging;
using System;
using System.IO;
using System.Security.AccessControl;

namespace Metalogix.Licensing.Storage
{
    public class FileDataStorage : IDataStorage, IDisposable
    {
        private const string _SAVED_DATE = "sd";

        private string _lockMutexName;

        private readonly string _fileName;

        private readonly string _backupFileName;

        private readonly bool _isSecure;

        private ValueCollection _values;

        private readonly FileSystemAccessRule _access;

        public bool Exists
        {
            get { return File.Exists(this._fileName); }
        }

        public FileDataStorage(string fileName, bool isSecure, FileSystemAccessRule fileSystemAccess) : this(fileName,
            isSecure, fileSystemAccess, null)
        {
        }

        public FileDataStorage(string fileName, bool isSecure, FileSystemAccessRule fileSystemAccess,
            string lockMutexName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            this._lockMutexName = lockMutexName;
            this._fileName = fileName;
            this._backupFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.ChangeExtension(fileName, "bak"));
            this._isSecure = isSecure;
            this._access = fileSystemAccess;
            this._values = new ValueCollection(null);
        }

        private void CreateBackup(string val)
        {
            FileSecurity fileSecurity = new FileSecurity();
            fileSecurity.AddAccessRule(this._access);
            using (FileStream fileStream = new FileStream(this._backupFileName, FileMode.Create, FileSystemRights.Write,
                       FileShare.None, 1024, FileOptions.None, fileSecurity))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(val);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        ~FileDataStorage()
        {
            this.Dispose(false);
        }

        public string GetValue(string valueName)
        {
            string item = this._values[valueName];
            Logger.Debug.WriteFormat("FileDataStorage >> GetValue: Key={0}; Val={1}", new object[] { valueName, item });
            return item;
        }

        public void Load()
        {
            Logger.Debug.Write("FileDataStorage >> Load: Entered");
            FileInfo fileInfo = new FileInfo(this._fileName);
            if (!fileInfo.Exists)
            {
                throw new Exception("The given file doesn't exist.");
            }

            GlobalLock globalLock = null;
            string text;
            try
            {
                globalLock = ((!string.IsNullOrEmpty(this._lockMutexName))
                    ? new GlobalLock(this._lockMutexName)
                    : null);
                using (StreamReader streamReader = new StreamReader(this._fileName))
                {
                    text = streamReader.ReadToEnd();
                }

                string val = text;
                try
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new Exception("Empty license file");
                    }

                    text = (this._isSecure ? Crypter.Decrypt(text) : text);
                    this.CreateBackup(val);
                }
                catch
                {
                    this.RepairUsageFile();
                    this.Load();
                    return;
                }
            }
            finally
            {
                if (globalLock != null)
                {
                    globalLock.Dispose();
                }
            }

            Logger.Debug.WriteFormat("FileDataStorage >> Load: Values loaded={0}", new object[]
            {
                text
            });
            ValueCollection values = new ValueCollection(text);
            this._values = values;
        }

        private void RepairUsageFile()
        {
            if (!File.Exists(this._backupFileName))
            {
                throw new Exception("It is not able to restore Usage.lic file, because its backup does not exist");
            }

            string str = File.ReadAllText(this._backupFileName);
            File.WriteAllText(this._fileName, str);
            File.Delete(this._backupFileName);
        }

        public void Save()
        {
            Logger.Debug.Write("FileDataStorage >> Save: Entered");
            if (!this._values.IsDirty)
            {
                Logger.Debug.Write("FileDataStorage >> Save: skipped, not dirty.");
                return;
            }

            Logger.Debug.WriteFormat("FileDataStorage >> Save: saving to File={0}; IsSec={1}; Values={2}", new object[]
            {
                this._fileName,
                this._isSecure,
                this._values.ToString()
            });
            this._values["sd"] = DateTime.Now.ToFileTimeUtc().ToString();
            string value = this._isSecure ? Crypter.Encrypt(this._values.ToString()) : this._values.ToString();
            GlobalLock globalLock = null;
            try
            {
                globalLock = ((!string.IsNullOrEmpty(this._lockMutexName))
                    ? new GlobalLock(this._lockMutexName)
                    : null);
                string directoryName = Path.GetDirectoryName(this._fileName);
                if (!Directory.Exists(directoryName))
                {
                    DirectorySecurity directorySecurity = new DirectorySecurity();
                    directorySecurity.AddAccessRule(this._access);
                    Directory.CreateDirectory(directoryName, directorySecurity);
                }

                FileSecurity fileSecurity = new FileSecurity();
                fileSecurity.AddAccessRule(this._access);
                using (FileStream fileStream = new FileStream(this._fileName, FileMode.Create, FileSystemRights.Write,
                           FileShare.None, 1024, FileOptions.None, fileSecurity))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(value);
                    }
                }
            }
            finally
            {
                if (globalLock != null)
                {
                    globalLock.Dispose();
                }
            }

            Logger.Debug.Write("FileDataStorage >> Save: successfull.");
        }

        public void SetValue(string name, string value)
        {
            Logger.Debug.WriteFormat("FileDataStorage >> SetValue: Key={0}; Val={1}", new object[] { name, value });
            this._values[name] = value;
        }
    }
}