using Metalogix.Actions.Incremental.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Database.Adapters
{
    public abstract class MappingDatabaseAdapter : IMappingsDatabaseAdapter
    {
        protected const string DATASOURCE = "DataSource=";

        protected const string MAXDATABASESIZE = "Max Database Size=4091";

        protected const string DEFAULT_WORKSPACE_NAME = "Workspace 1";

        private readonly object _lock = new object();

        private string _adapterContext;

        private Metalogix.Actions.Incremental.Database.AdapterCallWrapper _adapterWrapper;

        private bool _isMappingExist;

        public Metalogix.Actions.Incremental.Database.AdapterCallWrapper AdapterCallWrapper
        {
            get { return this._adapterWrapper; }
        }

        public string AdapterContext
        {
            get { return this._adapterContext; }
        }

        public abstract string AdapterType { get; }

        protected MappingDatabaseAdapter(string context,
            Metalogix.Actions.Incremental.Database.AdapterCallWrapper wrapper)
        {
            this._adapterContext = context;
            this._adapterWrapper = wrapper;
        }

        private void AddCommandParameter(DbCommand command, string name, string value)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = name;
            if (!name.Equals("@ExtendedProperties", StringComparison.InvariantCultureIgnoreCase) ||
                !string.IsNullOrEmpty(value))
            {
                dbParameter.Value = value;
            }
            else
            {
                dbParameter.Value = DBNull.Value;
            }

            command.Parameters.Add(dbParameter);
        }

        protected void AddExtendedPropertiesColumn(DbConnection connection)
        {
            using (DbCommand dbCommand = connection.CreateCommand())
            {
                dbCommand.Connection = connection;
                dbCommand.CommandText =
                    "SELECT 1 AS ExtendedPropertiesColumnCount FROM INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME = 'Mappings') AND (COLUMN_NAME = 'ExtendedProperties')";
                if (Convert.ToInt16(dbCommand.ExecuteScalar()) == 0)
                {
                    dbCommand.CommandText = "ALTER TABLE Mappings Add ExtendedProperties nvarchar(500)";
                    dbCommand.ExecuteNonQuery();
                }
            }
        }

        public void AddOrUpdateMapping(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties)
        {
            lock (this._lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.InsertOrUpdateMapping(sourceID, sourceURL, targetID, targetURL, targetType,
                        extendedProperties)));
            }
        }

        private void CheckMappingExist(DbCommand command, DbConnection connection)
        {
            this._isMappingExist = false;
            command.CommandText =
                "SELECT COUNT(*) FROM MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";
            try
            {
                this.Open(connection);
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    this._isMappingExist = true;
                }
            }
            finally
            {
                this.Close(connection);
            }
        }

        private void Close(DbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => connection.Close()));
            }
        }

        protected void CreateSchema(DbConnection connection)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                using (DbCommand dbCommand = connection.CreateCommand())
                {
                    dbCommand.Connection = connection;
                    dbCommand.CommandText =
                        "CREATE TABLE Mappings (\r\n                SourceID nvarchar(100) NOT NULL,\r\n                SourceURL nvarchar(500) NOT NULL,\r\n                TargetID nvarchar(100) NOT NULL,\r\n                TargetURL nvarchar(500) NOT NULL,\r\n                TargetType nvarchar(200) ,\r\n                ExtendedProperties nvarchar(500)               \r\n                )";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText = "CREATE INDEX IX_SOURCE_ENTRY ON Mappings (SourceID, SourceURL)";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                }
            }));
        }

        public IEnumerable<Mapping> GetMappingsInScope(string sourceScopeUrl, string targetScopeUrl, string targetType)
        {
            string str;
            List<Mapping> mappings = new List<Mapping>();
            using (DbConnection sQLConnection = this.GetSQLConnection())
            {
                using (DbCommand dbCommand = sQLConnection.CreateCommand())
                {
                    this.AddCommandParameter(dbCommand, "@SourceURL", string.Concat(sourceScopeUrl, '%'));
                    this.AddCommandParameter(dbCommand, "@TargetURL", string.Concat(targetScopeUrl, '%'));
                    this.AddCommandParameter(dbCommand, "@TargetType", targetType);
                    dbCommand.CommandText =
                        "SELECT * from MAPPINGS WHERE SourceURL LIKE @SourceURL AND TargetURL LIKE @TargetURL AND TargetType = @TargetType";
                    try
                    {
                        this.Open(sQLConnection);
                        using (DbDataReader dbDataReaders = dbCommand.ExecuteReader())
                        {
                            int ordinal = dbDataReaders.GetOrdinal("SourceID");
                            int num = dbDataReaders.GetOrdinal("SourceURL");
                            int ordinal1 = dbDataReaders.GetOrdinal("TargetID");
                            int num1 = dbDataReaders.GetOrdinal("TargetURL");
                            int ordinal2 = dbDataReaders.GetOrdinal("TargetType");
                            while (dbDataReaders.Read())
                            {
                                Mapping mapping = new Mapping()
                                {
                                    SourceId = dbDataReaders.GetString(ordinal),
                                    SourceUrl = dbDataReaders.GetString(num),
                                    TargetId = dbDataReaders.GetString(ordinal1),
                                    TargetUrl = dbDataReaders.GetString(num1)
                                };
                                Mapping mapping1 = mapping;
                                if (dbDataReaders.IsDBNull(ordinal2))
                                {
                                    str = null;
                                }
                                else
                                {
                                    str = dbDataReaders.GetString(ordinal2);
                                }

                                mapping1.TargetType = str;
                                mappings.Add(mapping);
                            }
                        }
                    }
                    finally
                    {
                        this.Close(sQLConnection);
                    }
                }
            }

            return mappings;
        }

        protected abstract DbConnection GetSQLConnection();

        public TargetMapping GetTargetMapping(string sourceID, string sourceURL, string extendedProperties)
        {
            string str;
            TargetMapping targetMapping = null;
            using (DbConnection sQLConnection = this.GetSQLConnection())
            {
                using (DbCommand dbCommand = sQLConnection.CreateCommand())
                {
                    this.AddCommandParameter(dbCommand, "@SourceID", sourceID);
                    this.AddCommandParameter(dbCommand, "@SourceURL", sourceURL);
                    this.AddCommandParameter(dbCommand, "@ExtendedProperties", extendedProperties);
                    dbCommand.CommandText =
                        "SELECT * from MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";
                    try
                    {
                        this.Open(sQLConnection);
                        using (DbDataReader dbDataReaders = dbCommand.ExecuteReader())
                        {
                            if (dbDataReaders.Read())
                            {
                                targetMapping = new TargetMapping();
                                int ordinal = dbDataReaders.GetOrdinal("TargetID");
                                targetMapping.ID = dbDataReaders.GetString(ordinal);
                                ordinal = dbDataReaders.GetOrdinal("TargetURL");
                                targetMapping.URL = dbDataReaders.GetString(ordinal);
                                ordinal = dbDataReaders.GetOrdinal("TargetType");
                                TargetMapping targetMapping1 = targetMapping;
                                if (dbDataReaders.IsDBNull(ordinal))
                                {
                                    str = null;
                                }
                                else
                                {
                                    str = dbDataReaders.GetString(ordinal);
                                }

                                targetMapping1.Type = str;
                            }
                        }
                    }
                    finally
                    {
                        this.Close(sQLConnection);
                    }
                }
            }

            return targetMapping;
        }

        protected abstract DbConnection Initialize();

        private void InsertOrUpdateMapping(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties)
        {
            using (DbConnection sQLConnection = this.GetSQLConnection())
            {
                using (DbCommand dbCommand = sQLConnection.CreateCommand())
                {
                    this.AddCommandParameter(dbCommand, "@SourceID", sourceID);
                    this.AddCommandParameter(dbCommand, "@SourceURL", sourceURL);
                    this.AddCommandParameter(dbCommand, "@ExtendedProperties", extendedProperties);
                    this.CheckMappingExist(dbCommand, sQLConnection);
                    this.AddCommandParameter(dbCommand, "@TargetID", targetID);
                    this.AddCommandParameter(dbCommand, "@TargetURL", targetURL);
                    this.AddCommandParameter(dbCommand, "@TargetType", targetType);
                    if (!this._isMappingExist)
                    {
                        dbCommand.CommandText =
                            "INSERT INTO MAPPINGS VALUES (@SourceID, @SourceURL, @TargetID, @TargetURL, @TargetType, @ExtendedProperties)";
                    }
                    else
                    {
                        dbCommand.CommandText =
                            "UPDATE MAPPINGS SET TargetID=@TargetID, TargetURL=@TargetURL, TargetType=@TargetType, ExtendedProperties = @ExtendedProperties WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";
                    }

                    try
                    {
                        this.Open(sQLConnection);
                        dbCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        this.Close(sQLConnection);
                    }
                }
            }
        }

        public bool IsMappingEntryExist(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties)
        {
            bool flag = false;
            using (DbConnection sQLConnection = this.GetSQLConnection())
            {
                using (DbCommand dbCommand = sQLConnection.CreateCommand())
                {
                    this.AddCommandParameter(dbCommand, "@SourceID", sourceID);
                    this.AddCommandParameter(dbCommand, "@SourceURL", sourceURL);
                    this.AddCommandParameter(dbCommand, "@TargetID", targetID);
                    this.AddCommandParameter(dbCommand, "@TargetURL", targetURL);
                    this.AddCommandParameter(dbCommand, "@TargetType", targetType);
                    this.AddCommandParameter(dbCommand, "@ExtendedProperties", extendedProperties);
                    dbCommand.CommandText =
                        "SELECT COUNT(*) from MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND TargetID = @TargetID AND TargetURL = @targetURL AND TargetType = @TargetType AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";
                    try
                    {
                        this.Open(sQLConnection);
                        if (Convert.ToInt16(dbCommand.ExecuteScalar()) > 0)
                        {
                            flag = true;
                        }
                    }
                    finally
                    {
                        this.Close(sQLConnection);
                    }
                }
            }

            return flag;
        }

        private void Open(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open && connection.Database != null &&
                connection.DataSource != null)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => connection.Open()));
            }
        }

        public void ProvisionDatabase()
        {
            DbConnection dbConnection = null;
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => dbConnection = this.Initialize()));
            if (dbConnection == null)
            {
                throw new NullReferenceException("Database adapter cannot be null.");
            }

            if (dbConnection.State == ConnectionState.Broken || dbConnection.State == ConnectionState.Closed)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => dbConnection.Open()));
            }
        }

        protected void UpdateTargetColumn(DbConnection connection)
        {
            using (DbCommand dbCommand = connection.CreateCommand())
            {
                dbCommand.Connection = connection;
                dbCommand.CommandText =
                    "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Mappings' AND COLUMN_NAME = 'TargetURL'";
                if (Convert.ToInt16(dbCommand.ExecuteScalar()) <= 256)
                {
                    dbCommand.CommandText = "ALTER TABLE Mappings ALTER COLUMN TargetUrl nvarchar(500)";
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
    }
}