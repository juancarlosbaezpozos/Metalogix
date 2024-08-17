using Metalogix;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metalogix.DataResolution
{
    public class TemporaryTableDataResolver : DataResolver<OptionsBase>
    {
        private ThreadSafeSerializableTable<string, string> _dataTable =
            new ThreadSafeSerializableTable<string, string>();

        public TemporaryTableDataResolver()
        {
        }

        public override void ClearAllData()
        {
            this._dataTable.Clear();
        }

        public override void DeleteDataAtKey(string key)
        {
            if (this._dataTable.ContainsKey(key))
            {
                this._dataTable.Remove(key);
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

        public override void WriteDataAtKey(string key, byte[] data)
        {
            this.WriteStringDataAtKey(key, Encoding.Unicode.GetString(data));
        }

        public override void WriteStringDataAtKey(string key, string data)
        {
            if (this._dataTable.ContainsKey(key))
            {
                this._dataTable[key] = data;
                return;
            }

            this._dataTable.Add(key, data);
        }
    }
}