using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metalogix.Core.ConfigVariables
{
    public class DatabaseTableDataResolver : DataResolver<DatabaseTableDataResolverOptions>
    {
        private IConfigDatabaseAdapter _adapter;

        private ThreadSafeSerializableTable<string, string> _dataTable;

        public DatabaseTableDataResolver(DatabaseTableDataResolverOptions options)
        {
            base.ResolverOptions = options;
        }

        public override void ClearAllData()
        {
        }

        public override void DeleteDataAtKey(string key)
        {
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

            return Encoding.UTF8.GetBytes(str);
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

        private void Initialize()
        {
            SqlServerConfigDatabaseAdapter sqlServerConfigDatabaseAdapter = new SqlServerConfigDatabaseAdapter()
            {
                ConnectionString = base.ResolverOptions.ConnectionString
            };
            this._adapter = sqlServerConfigDatabaseAdapter;
            this._adapter.InitializeAdapter();
            this.LoadDataTable();
        }

        private void LoadDataTable()
        {
            this._dataTable = new ThreadSafeSerializableTable<string, string>();
            foreach (KeyValuePair<string, string> variable in this._adapter.GetVariables(base.ResolverOptions.Scope))
            {
                this._dataTable.Add(variable);
            }
        }

        protected override void OnOptionsChanged()
        {
            this.Initialize();
        }

        public override void WriteDataAtKey(string key, byte[] data)
        {
            this.WriteStringDataAtKey(key, Encoding.UTF8.GetString(data));
        }

        public override void WriteStringDataAtKey(string key, string data)
        {
            if (!this._dataTable.ContainsKey(key))
            {
                this._adapter.AddVariable(base.ResolverOptions.Scope, key, data);
                this._dataTable.Add(key, data);
                return;
            }

            this._adapter.UpdateVariable(base.ResolverOptions.Scope, key, data);
            this._dataTable[key] = data;
        }
    }
}