using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.ServiceProcess;

namespace Metalogix.Database
{
    public static class DatabaseBrowser
    {
        public static SQLDatabase CreateSQLDatabase(string connectionString, string databaseName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand =
                       new SqlCommand(string.Concat("CREATE DATABASE ", databaseName), sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }

            return new SQLDatabase(null)
            {
                Name = databaseName
            };
        }

        public static void DeleteSQLDatabase(string connectionString, string databaseName)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand =
                       new SqlCommand(string.Concat("DROP DATABASE ", databaseName), sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static ArrayList GetLocalSQLServers()
        {
            ArrayList arrayLists = new ArrayList();
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; i < (int)services.Length; i++)
                {
                    ServiceController serviceController = services[i];
                    if (serviceController.ServiceName.StartsWith("MSSQL$", StringComparison.InvariantCulture))
                    {
                        string str = string.Concat(Environment.MachineName, "\\",
                            serviceController.ServiceName.Substring(6));
                        arrayLists.Add(str);
                    }
                    else if (serviceController.ServiceName.Equals("MSSQLSERVER", StringComparison.InvariantCulture))
                    {
                        arrayLists.Add(Environment.MachineName);
                    }
                }
            }
            catch (Exception exception)
            {
            }

            return arrayLists;
        }

        public static ArrayList GetNetworkSQLServers()
        {
            string str;
            string str1;
            ArrayList arrayLists = new ArrayList();
            try
            {
                foreach (DataRow row in SqlDataSourceEnumerator.Instance.GetDataSources().Rows)
                {
                    if (row[0] is DBNull)
                    {
                        str = null;
                    }
                    else
                    {
                        str = row[0].ToString();
                    }

                    string str2 = str;
                    if (row[1] is DBNull)
                    {
                        str1 = null;
                    }
                    else
                    {
                        str1 = row[1].ToString();
                    }

                    string str3 = str1;
                    arrayLists.Add(string.Concat(str2,
                        (string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(str3.Trim())
                            ? ""
                            : string.Concat("\\", str3))));
                }

                arrayLists.Sort();
            }
            catch (Exception exception)
            {
            }

            return arrayLists;
        }

        public static SQLDatabaseCollection GetSQLDatabases(string connectionString)
        {
            SQLDatabaseCollection sQLDatabaseCollection = new SQLDatabaseCollection(null);
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("Select Name from SysDatabases", sqlConnection))
                {
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dataTable);
                    }
                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                SQLDatabase sQLDatabase = new SQLDatabase(null)
                {
                    Name = (string)row["Name"]
                };
                sQLDatabaseCollection.Add(sQLDatabase);
            }

            return sQLDatabaseCollection;
        }
    }
}