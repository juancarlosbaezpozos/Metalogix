using Metalogix.Actions.Incremental.Database;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Database.Adapters
{
    public class SqlServerDatabaseAdapter : MappingDatabaseAdapter
    {
        public override string AdapterType
        {
            get { return DatabaseAdapterType.SqlServer.ToString(); }
        }

        public SqlServerDatabaseAdapter(string databaseFile,
            Metalogix.Actions.Incremental.Database.AdapterCallWrapper wrapper) : base(databaseFile, wrapper)
        {
        }

        protected override DbConnection GetSQLConnection()
        {
            return new SqlConnection(base.AdapterContext);
        }

        protected override DbConnection Initialize()
        {
            DbConnection dbConnection = null;
            base.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                SqlConnection sqlConnection = null;
                try
                {
                    sqlConnection = new SqlConnection(this.AdapterContext);
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand =
                           new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Mappings'"))
                    {
                        sqlCommand.Connection = sqlConnection;
                        if ((int)sqlCommand.ExecuteScalar() != 0)
                        {
                            base.UpdateTargetColumn(sqlConnection);
                            base.AddExtendedPropertiesColumn(sqlConnection);
                            sqlCommand.CommandText =
                                "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Mappings' AND COLUMN_NAME = 'SourceURL'";
                            if (Convert.ToInt16(sqlCommand.ExecuteScalar()) <= 256)
                            {
                                sqlCommand.CommandText = "ALTER TABLE Mappings ALTER COLUMN SourceURL nvarchar(500);";
                                sqlCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            base.CreateSchema(sqlConnection);
                        }
                    }
                }
                finally
                {
                    if (sqlConnection != null)
                    {
                        sqlConnection.Close();
                        dbConnection = sqlConnection;
                    }
                }
            }));
            return dbConnection;
        }
    }
}