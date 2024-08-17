using Metalogix.Actions.Incremental.Database.Adapters;
using Metalogix.Core;
using System;

namespace Metalogix.Actions.Incremental.Database
{
    public class MappingDatabaseProvider
    {
        private static MappingDatabaseProvider _instance;

        private MappingDatabase _incrementalDatabase;

        public static MappingDatabaseProvider Instance
        {
            get
            {
                if (MappingDatabaseProvider._instance == null)
                {
                    MappingDatabaseProvider mappingDatabaseProvider = new MappingDatabaseProvider();
                    mappingDatabaseProvider.LoadFromConfig();
                    MappingDatabaseProvider._instance = mappingDatabaseProvider;
                }

                return MappingDatabaseProvider._instance;
            }
        }

        private MappingDatabaseProvider()
        {
        }

        private MappingDatabase CreateMappingDatabase()
        {
            string defaultMappingDBAdapter =
                Metalogix.Actions.Incremental.Database.ConfigurationVariables.DefaultMappingDBAdapter;
            string sQLMappingDBContext = null;
            string str = defaultMappingDBAdapter;
            string str1 = str;
            if (str != null)
            {
                if (str1 == "SqlServer" || str1 == "Agent")
                {
                    sQLMappingDBContext = Metalogix.Actions.Incremental.Database.ConfigurationVariables
                        .SQLMappingDBContext;
                }
                else
                {
                    if (str1 != "SqlCe")
                    {
                        throw new ArgumentOutOfRangeException(string.Concat("'", defaultMappingDBAdapter,
                            "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
                    }

                    sQLMappingDBContext = Metalogix.Actions.Incremental.Database.ConfigurationVariables
                        .FileMappingDBContext;
                }

                return this.CreateMappingDatabase(defaultMappingDBAdapter, sQLMappingDBContext,
                    Metalogix.Actions.Incremental.Database.ConfigurationVariables.DefaultMappingDatabaseCallWrapper);
            }

            throw new ArgumentOutOfRangeException(string.Concat("'", defaultMappingDBAdapter,
                "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
        }

        private MappingDatabase CreateMappingDatabase(string adapterType, string adapterContext,
            AdapterCallWrapper callWrapper)
        {
            if (string.IsNullOrEmpty(adapterType))
            {
                throw new ArgumentNullException("adapterType");
            }

            if (string.IsNullOrEmpty(adapterContext))
            {
                throw new ArgumentNullException("adapterContext");
            }

            if (callWrapper == null)
            {
                throw new ArgumentNullException("callWrapper");
            }

            IMappingsDatabaseAdapter sqlServerDatabaseAdapter = null;
            string empty = string.Empty;
            string str = adapterType;
            string str1 = str;
            if (str == null)
            {
                empty = string.Concat("'", adapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'.");
                if (!string.IsNullOrEmpty(empty))
                {
                    Logging.LogExceptionToTextFileWithEventLogBackup(new Exception(empty), "CreateMappingDatabase",
                        true);
                }

                return new MappingDatabase(sqlServerDatabaseAdapter);
            }
            else if (str1 == "SqlServer")
            {
                try
                {
                    sqlServerDatabaseAdapter = new SqlServerDatabaseAdapter(adapterContext, callWrapper);
                }
                catch (Exception exception)
                {
                    empty = exception.Message;
                }
            }
            else
            {
                if (str1 != "SqlCe")
                {
                    empty = string.Concat("'", adapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'.");
                    if (!string.IsNullOrEmpty(empty))
                    {
                        Logging.LogExceptionToTextFileWithEventLogBackup(new Exception(empty), "CreateMappingDatabase",
                            true);
                    }

                    return new MappingDatabase(sqlServerDatabaseAdapter);
                }

                try
                {
                    sqlServerDatabaseAdapter = new SqlCeDatabaseAdapter(adapterContext, callWrapper);
                }
                catch (Exception exception1)
                {
                    empty = exception1.Message;
                }
            }

            if (!string.IsNullOrEmpty(empty))
            {
                Logging.LogExceptionToTextFileWithEventLogBackup(new Exception(empty), "CreateMappingDatabase", true);
            }

            return new MappingDatabase(sqlServerDatabaseAdapter);
        }

        public MappingDatabase GetIncrementalDatabase()
        {
            return this._incrementalDatabase;
        }

        public void LoadFromConfig()
        {
            this._incrementalDatabase = this.CreateMappingDatabase();
            this._incrementalDatabase.Adapter.ProvisionDatabase();
        }
    }
}