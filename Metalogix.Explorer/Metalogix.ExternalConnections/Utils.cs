using Metalogix.Permissions;
using Metalogix.Utilities;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Metalogix.ExternalConnections
{
    public static class Utils
    {
        private static Type s_type;

        private static int? s_iWebServiceTimeoutTime;

        private static int? s_iSQLQueryTimeoutTime;

        public static int SQLQueryTimeoutTime
        {
            get
            {
                int num;
                if (!Utils.s_iSQLQueryTimeoutTime.HasValue)
                {
                    if (Utils.GetEnvironmentVariableValue<int>("SQLQueryTimeoutTime", out num))
                    {
                        Utils.s_iSQLQueryTimeoutTime = new int?(num);
                    }

                    if (!Utils.s_iSQLQueryTimeoutTime.HasValue || Utils.s_iSQLQueryTimeoutTime.Value < 0)
                    {
                        Utils.s_iSQLQueryTimeoutTime = new int?(30);
                    }
                }

                return Utils.s_iSQLQueryTimeoutTime.Value;
            }
        }

        public static int WebServiceTimeoutTime
        {
            get
            {
                int num;
                if (!Utils.s_iWebServiceTimeoutTime.HasValue)
                {
                    if (Utils.GetEnvironmentVariableValue<int>("WebServiceTimeoutTime", out num))
                    {
                        Utils.s_iWebServiceTimeoutTime = new int?(num * 1000);
                    }

                    if (!Utils.s_iWebServiceTimeoutTime.HasValue || Utils.s_iWebServiceTimeoutTime.Value < 0)
                    {
                        Utils.s_iWebServiceTimeoutTime = new int?(100000);
                    }
                }

                return Utils.s_iWebServiceTimeoutTime.Value;
            }
        }

        static Utils()
        {
            Utils.s_type = null;
            Utils.s_iWebServiceTimeoutTime = null;
            Utils.s_iSQLQueryTimeoutTime = null;
        }

        internal static Type GetEnvironmentVariablesType()
        {
            if (Utils.s_type == null)
            {
                try
                {
                    StackFrame[] frames = (new StackTrace()).GetFrames();
                    Assembly assembly = null;
                    StackFrame[] stackFrameArray = frames;
                    int num = 0;
                    while (num < (int)stackFrameArray.Length)
                    {
                        Assembly assembly1 = stackFrameArray[num].GetMethod().DeclaringType.Assembly;
                        if (assembly1.ManifestModule.Name != "Metalogix.SharePoint.dll")
                        {
                            num++;
                        }
                        else
                        {
                            assembly = assembly1;
                            break;
                        }
                    }

                    if (assembly != null)
                    {
                        Type type = assembly.GetType("Metalogix.SharePoint.ConfigurationVariables");
                        if (type != null)
                        {
                            Utils.s_type = type;
                        }
                    }
                }
                catch
                {
                }
            }

            return Utils.s_type;
        }

        public static bool GetEnvironmentVariableValue<T>(string sVariableName, out T oValue)
        {
            oValue = default(T);
            try
            {
                Type environmentVariablesType = Utils.GetEnvironmentVariablesType();
                if (environmentVariablesType != null)
                {
                    PropertyInfo property =
                        environmentVariablesType.GetProperty(sVariableName, BindingFlags.Static | BindingFlags.Public);
                    if (property != null)
                    {
                        oValue = (T)property.GetValue(null, null);
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public static SqlCommand GetSqlCommand(string sCommandString, SqlConnection connection)
        {
            return new SqlCommand(sCommandString, connection)
            {
                CommandTimeout = Utils.SQLQueryTimeoutTime
            };
        }

        public static SqlConnection GetSQLConnection(string sSqlServer, Credentials creds)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder["Data Source"] = sSqlServer;
            if (!creds.IsDefault)
            {
                sqlConnectionStringBuilder.UserID = creds.UserName;
                sqlConnectionStringBuilder.Password = creds.Password.ToInsecureString();
            }
            else
            {
                sqlConnectionStringBuilder["integrated Security"] = true;
            }

            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }

        public static bool IsNintexDatabase(SqlConnection conn, out bool bIsConfigDB, out string sErrors)
        {
            bool flag;
            sErrors = null;
            bIsConfigDB = false;
            try
            {
                try
                {
                    conn.Open();
                    SqlCommand sqlCommand = Utils.GetSqlCommand(
                        "Select col_length('HumanWorkflow', 'HumanWorkflowID') as HumanWorkflow, col_length('Activities', 'ActivityID') as Activities, col_length('WorkflowInstance', 'InstanceID') as WorkflowInstance",
                        conn);
                    DataTable dataTable = new DataTable("Results");
                    (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
                    if (dataTable.Rows.Count == 1)
                    {
                        DataRow item = dataTable.Rows[0];
                        if (item["HumanWorkflow"] is DBNull || item["WorkflowInstance"] is DBNull)
                        {
                            sErrors = "SQL Server Database is not a Nintex Database.";
                            flag = false;
                        }
                        else
                        {
                            if (!(item["Activities"] is DBNull))
                            {
                                bIsConfigDB = true;
                            }

                            flag = true;
                        }
                    }
                    else
                    {
                        sErrors = "Could not connect to SQL server.";
                        flag = false;
                    }
                }
                catch (Exception exception)
                {
                    sErrors = exception.Message;
                    flag = false;
                }
            }
            finally
            {
                conn.Close();
            }

            return flag;
        }
    }
}