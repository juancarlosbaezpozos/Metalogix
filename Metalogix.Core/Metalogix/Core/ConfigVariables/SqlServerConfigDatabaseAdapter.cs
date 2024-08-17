using Metalogix;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;

namespace Metalogix.Core.ConfigVariables
{
    public class SqlServerConfigDatabaseAdapter : IConfigDatabaseAdapter
    {
        private static string _userName;

        private readonly static string _updateQuery;

        public string ConnectionString { get; set; }

        static SqlServerConfigDatabaseAdapter()
        {
            SqlServerConfigDatabaseAdapter._userName = null;
            SqlServerConfigDatabaseAdapter._updateQuery =
                "UPDATE UserData SET Value=@Value WHERE Scope=@Scope AND Name=@Name AND Edition=@Edition AND UserName=@UserName";
        }

        public SqlServerConfigDatabaseAdapter()
        {
        }

        public void AddVariable(string scope, string name, string value)
        {
            ThreadSafeDictionary<string, string> strs = this.FetchProductAndUserName(scope);
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText =
                            "SELECT COUNT(Scope) FROM UserData WHERE Scope=@Scope AND Name=@Name AND Edition=@Edition AND UserName=@UserName";
                        sqlCommand.Parameters.AddWithValue("@Scope", scope);
                        sqlCommand.Parameters.AddWithValue("@Name", name);
                        sqlCommand.Parameters.AddWithValue("@Edition", strs["ProductName"]);
                        sqlCommand.Parameters.AddWithValue("@UserName", strs["UserName"]);
                        int num = (int)sqlCommand.ExecuteScalar();
                        sqlCommand.Parameters.AddWithValue("@Value", value);
                        if (num != 0)
                        {
                            sqlCommand.CommandText = SqlServerConfigDatabaseAdapter._updateQuery;
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            sqlCommand.CommandText =
                                "INSERT INTO UserData(Scope,Name,Value,Edition,UserName) VALUES(@Scope,@Name,@Value,@Edition,@UserName)";
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void ClearVariables(string scope)
        {
        }

        public bool DeleteVariable(string scope, string name)
        {
            return false;
        }

        private ThreadSafeDictionary<string, string> FetchProductAndUserName(string scope)
        {
            ThreadSafeDictionary<string, string> strs = new ThreadSafeDictionary<string, string>()
            {
                { "ProductName", string.Empty },
                { "UserName", string.Empty }
            };
            if (scope.Equals(ResourceScope.ApplicationAndUserSpecific.ToString(),
                    StringComparison.InvariantCultureIgnoreCase) || scope.Equals(
                    ResourceScope.ApplicationSpecific.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                strs["ProductName"] = ApplicationData.GetProductName();
            }

            if ((scope.Equals(ResourceScope.ApplicationAndUserSpecific.ToString(),
                     StringComparison.InvariantCultureIgnoreCase) ||
                 scope.Equals(ResourceScope.UserSpecific.ToString(), StringComparison.InvariantCultureIgnoreCase)) &&
                SqlServerConfigDatabaseAdapter._userName != null)
            {
                strs["UserName"] = SqlServerConfigDatabaseAdapter._userName;
            }

            return strs;
        }

        public IEnumerable<KeyValuePair<string, string>> GetVariables(string scope)
        {
            ThreadSafeDictionary<string, string> strs = this.FetchProductAndUserName(scope);
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand =
                           new SqlCommand(
                               "SELECT * FROM UserData WHERE Scope=@Scope AND Edition=@Edition AND UserName=@UserName",
                               sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Scope", scope);
                        sqlCommand.Parameters.AddWithValue("@Edition", strs["ProductName"]);
                        sqlCommand.Parameters.AddWithValue("@UserName", strs["UserName"]);
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                yield return new KeyValuePair<string, string>(Convert.ToString(sqlDataReader["Name"]),
                                    Convert.ToString(sqlDataReader["Value"]));
                            }
                        }
                    }
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void InitializeAdapter()
        {
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[UserData]') IS NULL BEGIN CREATE TABLE UserData(Scope nvarchar (128), Name nvarchar (256), Value nvarchar(512), Edition nvarchar (256), UserName nvarchar(256), PRIMARY KEY(Scope, Name, Edition, UserName));END";
                        sqlCommand.ExecuteNonQuery();
                    }

                    this.SetUserName(sqlConnection);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void SetUserName(SqlConnection conn)
        {
            using (SqlCommand sqlCommand =
                   new SqlCommand("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AgentUserProfile'",
                       conn))
            {
                if (sqlCommand.ExecuteScalar() == null)
                {
                    WindowsIdentity current = WindowsIdentity.GetCurrent();
                    if (current != null)
                    {
                        SqlServerConfigDatabaseAdapter._userName = current.Name;
                    }
                }
                else
                {
                    using (SqlCommand sqlCommand1 =
                           new SqlCommand("SELECT * FROM AgentUserProfile Where Name=@Name", conn))
                    {
                        sqlCommand1.Parameters.AddWithValue("@Name", "RemotePowerShellUser");
                        using (SqlDataReader sqlDataReader = sqlCommand1.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                SqlServerConfigDatabaseAdapter._userName = Convert.ToString(sqlDataReader["Value"]);
                            }
                        }
                    }
                }
            }
        }

        public bool UpdateVariable(string scope, string name, string value)
        {
            bool flag;
            ThreadSafeDictionary<string, string> strs = this.FetchProductAndUserName(scope);
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = SqlServerConfigDatabaseAdapter._updateQuery;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.Parameters.AddWithValue("@Scope", scope);
                        sqlCommand.Parameters.AddWithValue("@Name", name);
                        sqlCommand.Parameters.AddWithValue("@Value", value);
                        sqlCommand.Parameters.AddWithValue("@Edition", strs["ProductName"]);
                        sqlCommand.Parameters.AddWithValue("@UserName", strs["UserName"]);
                        flag = sqlCommand.ExecuteNonQuery() == 1;
                    }
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return flag;
        }
    }
}