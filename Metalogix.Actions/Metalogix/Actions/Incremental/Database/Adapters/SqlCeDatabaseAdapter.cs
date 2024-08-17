using Metalogix.Actions.Incremental.Database;
using Metalogix.Explorer;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Database.Adapters
{
    public class SqlCeDatabaseAdapter : MappingDatabaseAdapter
    {
        private string _connectionString;

        public override string AdapterType
        {
            get { return DatabaseAdapterType.SqlCe.ToString(); }
        }

        public SqlCeDatabaseAdapter(string databaseFile,
            Metalogix.Actions.Incremental.Database.AdapterCallWrapper wrapper) : base(databaseFile, wrapper)
        {
            this._connectionString = string.Concat("DataSource=\"", base.AdapterContext, "\";Max Database Size=4091");
        }

        private SqlCeConnection CreateDatabase(string adapterContext)
        {
            string str = adapterContext.Substring(0, adapterContext.LastIndexOf("\\"));
            if (!Directory.Exists(str))
            {
                throw new DirectoryNotFoundException(str);
            }

            if (File.Exists(adapterContext))
            {
                File.Delete(adapterContext);
            }

            SqlCeConnection sqlCeConnection = new SqlCeConnection(this._connectionString);
            (new SqlCeEngine(this._connectionString)).CreateDatabase();
            return sqlCeConnection;
        }

        private SqlCeConnection CreateOrOpenDatabase(string adapterContext)
        {
            if (File.Exists(base.AdapterContext))
            {
                return this.OpenDatabase(adapterContext);
            }

            SqlCeConnection sqlCeConnection = this.CreateDatabase(adapterContext);
            sqlCeConnection.Open();
            base.CreateSchema(sqlCeConnection);
            sqlCeConnection.Close();
            return sqlCeConnection;
        }

        protected override DbConnection GetSQLConnection()
        {
            return SqlCeUtilities.GetConnection(this._connectionString, true);
        }

        protected override DbConnection Initialize()
        {
            DbConnection dbConnection;
            try
            {
                SqlCeConnection sqlCeConnection = null;
                base.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    sqlCeConnection = this.CreateOrOpenDatabase(this.AdapterContext)));
                dbConnection = sqlCeConnection;
            }
            catch (SqlCeException sqlCeException)
            {
                if (sqlCeException.NativeError == 25011)
                {
                    throw new ArgumentException("The specified file is not of a supported type");
                }

                throw;
            }

            return dbConnection;
        }

        private SqlCeConnection OpenDatabase(string adapterContext)
        {
            if (string.IsNullOrEmpty(adapterContext))
            {
                throw new ArgumentNullException("adapterContext", "No file specified");
            }

            if (!File.Exists(adapterContext))
            {
                throw new FileNotFoundException(string.Concat("Cannot find file: ", adapterContext));
            }

            FileAttributes attributes = File.GetAttributes(adapterContext);
            if (FileAttributes.ReadOnly.CompareTo(attributes & FileAttributes.ReadOnly) == 0)
            {
                throw new ReadOnlyException(string.Concat("Mapping database: ", adapterContext,
                    " is readonly.\nPlease either change the file properties or choose a different project database."));
            }

            SqlCeConnection connection = SqlCeUtilities.GetConnection(this._connectionString, true);
            base.UpdateTargetColumn(connection);
            base.AddExtendedPropertiesColumn(connection);
            return connection;
        }
    }
}