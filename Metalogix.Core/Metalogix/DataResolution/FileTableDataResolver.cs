using Metalogix;
using Metalogix.Core.Properties;
using Metalogix.DataResolution.Options;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.DataResolution
{
    public class FileTableDataResolver : DataResolver<FileTableOptions>
    {
        private ThreadSafeSerializableTable<string, string> _dataTable;

        public FileTableDataResolver(string fileName)
        {
            base.ResolverOptions.FilePath = fileName;
            this.Load();
        }

        public FileTableDataResolver()
        {
            this._dataTable = new ThreadSafeSerializableTable<string, string>();
        }

        public override void ClearAllData()
        {
            this._dataTable.Clear();
            this.Save();
        }

        public override void DeleteDataAtKey(string key)
        {
            if (this._dataTable.ContainsKey(key))
            {
                this._dataTable.Remove(key);
                this.Save();
            }
        }

        public override IEnumerable<string> GetAvailableDataKeys()
        {
            return this._dataTable.Keys;
        }

        public override byte[] GetDataAtKey(string key)
        {
            string str;
            if (!this._dataTable.TryGetValue(key, out str))
            {
                return new byte[0];
            }

            return Encoding.Unicode.GetBytes(str);
        }

        public override string GetStringDataAtKey(string key)
        {
            string str;
            if (this._dataTable.TryGetValue(key, out str))
            {
                return str;
            }

            return null;
        }

        private void Load()
        {
            string filePath = base.ResolverOptions.FilePath;
            if (!File.Exists(filePath))
            {
                this._dataTable = new ThreadSafeSerializableTable<string, string>();
            }
            else
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(filePath);
                    this._dataTable = new ThreadSafeSerializableTable<string, string>();
                    this._dataTable.FromXML(xmlDocument.DocumentElement);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    GlobalServices.ErrorHandler.HandleException(Resources.FileDataLoadingCaption,
                        string.Format(Resources.FileDataLoadingMessage, filePath), exception, ErrorIcon.Error);
                    this._dataTable = new ThreadSafeSerializableTable<string, string>();
                }
            }
        }

        protected override void OnOptionsChanged()
        {
            this.Load();
        }

        private void Save()
        {
            string filePath = base.ResolverOptions.FilePath;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(this._dataTable.ToXML());
                xmlDocument.Save(filePath);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException(Resources.FileDataSaveError,
                    string.Format(Resources.FileDataSaveError, base.ResolverOptions.FilePath), exception,
                    ErrorIcon.Error);
            }
        }

        public override void WriteDataAtKey(string key, byte[] data)
        {
            this.WriteStringDataAtKey(key, Encoding.Unicode.GetString(data));
        }

        public override void WriteStringDataAtKey(string key, string data)
        {
            if (!this._dataTable.ContainsKey(key))
            {
                this._dataTable.Add(key, data);
            }
            else
            {
                this._dataTable[key] = data;
            }

            this.Save();
        }
    }
}