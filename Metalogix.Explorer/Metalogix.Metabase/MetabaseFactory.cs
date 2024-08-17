using Metalogix.Metabase.Adapters;
using Metalogix.Metabase.Interfaces;
using System;
using System.IO;

namespace Metalogix.Metabase
{
    public sealed class MetabaseFactory
    {
        public MetabaseFactory()
        {
        }

        public static MetabaseConnection ConnectToExistingMetabase(DatabaseAdapterType adapterType,
            string adapterContext)
        {
            return MetabaseFactory.ConnectToExistingMetabase(adapterType.ToString(), adapterContext,
                ConfigurationVariables.DefaultMetabaseCallWrapper);
        }

        public static MetabaseConnection ConnectToExistingMetabase(string adapterType, string adapterContext)
        {
            return MetabaseFactory.ConnectToExistingMetabase(adapterType, adapterContext,
                ConfigurationVariables.DefaultMetabaseCallWrapper);
        }

        public static MetabaseConnection ConnectToExistingMetabase(string adapterType, string adapterContext,
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

            IMetabaseAdapter databaseSqlServerAdapter = null;
            string empty = string.Empty;
            string str = adapterType;
            string str1 = str;
            if (str == null)
            {
                empty = string.Concat("'", adapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'.");
                return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
            }
            else if (str1 == "SqlServer")
            {
                try
                {
                    databaseSqlServerAdapter = new DatabaseSqlServerAdapter(adapterContext, callWrapper);
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
                    return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
                }

                if (!File.Exists(adapterContext))
                {
                    empty = string.Concat("File not found: '", adapterContext, "'");
                }
                else
                {
                    try
                    {
                        databaseSqlServerAdapter = new DatabaseSqlCeAdapter(adapterContext, callWrapper);
                    }
                    catch (Exception exception1)
                    {
                        empty = exception1.Message;
                    }
                }
            }

            return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
        }

        public static MetabaseConnection CreateDefaultMetabaseConnection()
        {
            string defaultMetabaseAdapter = ConfigurationVariables.DefaultMetabaseAdapter;
            string sQLMetabaseContext = null;
            string str = defaultMetabaseAdapter;
            string str1 = str;
            if (str != null)
            {
                if (str1 == "SqlServer")
                {
                    sQLMetabaseContext = ConfigurationVariables.SQLMetabaseContext;
                }
                else
                {
                    if (str1 != "SqlCe")
                    {
                        throw new ArgumentOutOfRangeException(string.Concat("'", defaultMetabaseAdapter,
                            "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
                    }

                    sQLMetabaseContext = ConfigurationVariables.FileMetabaseContext;
                    if (ConfigurationVariables.AutoProvisionNewMetabaseFile)
                    {
                        sQLMetabaseContext = Path.Combine(sQLMetabaseContext, string.Concat(Guid.NewGuid(), ".sdf"));
                    }
                }

                return MetabaseFactory.CreateNewMetabaseConnection(defaultMetabaseAdapter, sQLMetabaseContext);
            }

            throw new ArgumentOutOfRangeException(string.Concat("'", defaultMetabaseAdapter,
                "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
        }

        public static MetabaseConnection CreateNewMetabaseConnection(DatabaseAdapterType adapterType,
            string adapterContext)
        {
            return MetabaseFactory.CreateNewMetabaseConnection(adapterType.ToString(), adapterContext,
                ConfigurationVariables.DefaultMetabaseCallWrapper);
        }

        public static MetabaseConnection CreateNewMetabaseConnection(string adapterType, string adapterContext)
        {
            return MetabaseFactory.CreateNewMetabaseConnection(adapterType, adapterContext,
                ConfigurationVariables.DefaultMetabaseCallWrapper);
        }

        public static MetabaseConnection CreateNewMetabaseConnection(string adapterType, string adapterContext,
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

            IMetabaseAdapter databaseSqlServerAdapter = null;
            string empty = string.Empty;
            string str = adapterType;
            string str1 = str;
            if (str == null)
            {
                empty = string.Concat("'", adapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'.");
                return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
            }
            else if (str1 == "SqlServer")
            {
                try
                {
                    databaseSqlServerAdapter = new DatabaseSqlServerAdapter(adapterContext, callWrapper);
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
                    return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
                }

                try
                {
                    databaseSqlServerAdapter = new DatabaseSqlCeAdapter(adapterContext, callWrapper);
                }
                catch (Exception exception1)
                {
                    empty = exception1.Message;
                }
            }

            return new MetabaseConnection(databaseSqlServerAdapter, adapterType, adapterContext, empty);
        }
    }
}