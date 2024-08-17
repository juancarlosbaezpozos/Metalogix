using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Jobs;
using Metalogix.Jobs.Adapters;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Metalogix.Actions.Remoting.Database
{
    public class AgentDb : JobHistorySqlServerDb, IDisposable, IJobHistoryAdapter, IAgentDb
    {
        private const string GetAllAgentsQuery =
            "SELECT agents.*, agentDetail.Details FROM (SELECT AgentID, MAX(TimeStamp) AS MaxTime FROM AgentJobHistory GROUP BY AgentID ) agentDetailTemp INNER JOIN AgentJobHistory agentDetail ON agentDetail.AgentID = agentDetailTemp.AgentID AND agentDetail.TimeStamp = agentDetailTemp.MaxTime RIGHT OUTER JOIN Agents agents ON agents.AgentID = agentDetail.AgentID";

        private readonly string _getAllAvailableAgentsQuery = string.Format("{0} WHERE agents.Status = 'Available'",
            "SELECT agents.*, agentDetail.Details FROM (SELECT AgentID, MAX(TimeStamp) AS MaxTime FROM AgentJobHistory GROUP BY AgentID ) agentDetailTemp INNER JOIN AgentJobHistory agentDetail ON agentDetail.AgentID = agentDetailTemp.AgentID AND agentDetail.TimeStamp = agentDetailTemp.MaxTime RIGHT OUTER JOIN Agents agents ON agents.AgentID = agentDetail.AgentID");

        private readonly string _productName = ApplicationData.GetProductName();

        private static string _encryptionKey;

        public new string AdapterType
        {
            get { return JobHistoryAdapterType.Agent.ToString(); }
        }

        public AgentDb(string connectionString) : base(connectionString)
        {
        }

        public Agent Add(Agent agent)
        {
            Agent agent1;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    StringBuilder stringBuilder1 = new StringBuilder();
                    string[] parameters = this.GetParameters();
                    for (int i = 0; i < (int)parameters.Length; i++)
                    {
                        string str = parameters[i];
                        stringBuilder.Append(string.Concat(str, ", "));
                        stringBuilder1.Append(string.Format("@{0}, ", str));
                    }

                    string str1 = stringBuilder.ToString().Trim().TrimEnd(new char[] { ',' });
                    string str2 = stringBuilder1.ToString().Trim();
                    char[] chrArray = new char[] { ',' };
                    string str3 = string.Format("INSERT INTO Agents ({0}) VALUES ({1})", str1, str2.TrimEnd(chrArray));
                    object[] agentID = new object[]
                    {
                        Constants.AgentID, Constants.Timestamp, Constants.Details,
                        this.GetUpdatedColumnName(Constants.AgentID), this.GetUpdatedColumnName(Constants.Timestamp),
                        this.GetUpdatedColumnName(Constants.Details)
                    };
                    string str4 = string.Format("INSERT INTO AgentJobHistory ({0}, {1}, {2}) VALUES ({3}, {4}, {5})",
                        agentID);
                    string str5 = string.Format("{0} ; {1}", str3, str4);
                    using (SqlCommand sqlCommand = new SqlCommand(str5, connection))
                    {
                        this.AddParams(sqlCommand, agent);
                        SqlParameterCollection sqlParameterCollection = sqlCommand.Parameters;
                        string updatedColumnName = this.GetUpdatedColumnName(Constants.Timestamp);
                        DateTime now = DateTime.Now;
                        sqlParameterCollection.AddWithValue(updatedColumnName, now.ToUniversalTime());
                        SqlParameterCollection parameters1 = sqlCommand.Parameters;
                        string updatedColumnName1 = this.GetUpdatedColumnName(Constants.Details);
                        KeyValuePair<DateTime, string> keyValuePair =
                            agent.Details.First<KeyValuePair<DateTime, string>>();
                        parameters1.AddWithValue(updatedColumnName1, keyValuePair.Value);
                        if (sqlCommand.ExecuteNonQuery() > 0)
                        {
                            agent1 = agent;
                            return agent1;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }

                return null;
            }

            return agent1;
        }

        private void AddJobParams(string[] parameters, SqlCommand sqlCommand_0, object[] values, string jobID)
        {
            object value;
            object universalTime;
            sqlCommand_0.Parameters.AddWithValue("@Status", JobUtils.GetStringValue(Convert.ToString(values[0])));
            sqlCommand_0.Parameters.AddWithValue("@StatusMessage",
                JobUtils.GetStringValue(Convert.ToString(values[1]), 255));
            SqlParameter sqlParameter = sqlCommand_0.Parameters.Add("@Started", OleDbType.Date);
            if (values[2] == null)
            {
                value = DBNull.Value;
            }
            else
            {
                value = ((DateTime)values[2]).ToUniversalTime();
            }

            sqlParameter.Value = value;
            SqlParameter sqlParameter1 = sqlCommand_0.Parameters.Add("@Finished", OleDbType.Date);
            if (values[3] == null)
            {
                universalTime = DBNull.Value;
            }
            else
            {
                universalTime = ((DateTime)values[3]).ToUniversalTime();
            }

            sqlParameter1.Value = universalTime;
            sqlCommand_0.Parameters.Add("@LicensedDataUsed", OleDbType.BigInt).Value = values[4];
            sqlCommand_0.Parameters.AddWithValue("@ResultsSummary",
                JobUtils.GetStringValue(Convert.ToString(values[5]), 255));
            sqlCommand_0.Parameters.AddWithValue("@JobID", jobID);
        }

        public bool AddLog(Guid agentID, string message)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    object[] objArray = new object[]
                    {
                        Constants.AgentID, Constants.Timestamp, Constants.Details,
                        this.GetUpdatedColumnName(Constants.AgentID), this.GetUpdatedColumnName(Constants.Timestamp),
                        this.GetUpdatedColumnName(Constants.Details)
                    };
                    using (SqlCommand sqlCommand =
                           new SqlCommand(
                               string.Format("INSERT INTO AgentJobHistory ({0}, {1}, {2}) VALUES ({3}, {4}, {5})",
                                   objArray), connection))
                    {
                        sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.AgentID), agentID);
                        SqlParameterCollection parameters = sqlCommand.Parameters;
                        string updatedColumnName = this.GetUpdatedColumnName(Constants.Timestamp);
                        DateTime now = DateTime.Now;
                        parameters.AddWithValue(updatedColumnName, now.ToUniversalTime());
                        sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.Details), message);
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }

        private void AddParams(SqlCommand sqlCommand_0, Agent agent)
        {
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.AgentID), agent.AgentID);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.MachineIP), agent.MachineIP);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.MachineName), agent.MachineName);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.OSVersion), agent.OSVersion);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.CMVersion), agent.CMVersion);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.UserName), agent.UserName);
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.Password),
                EncryptionUtil.EncryptByRijndaelKey(agent.Password, AgentDb._encryptionKey));
            sqlCommand_0.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.Status), agent.Status.ToString());
        }

        public override void CreateDbIfNotExists()
        {
            base.CreateDbIfNotExists();
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    using (SqlCommand sqlCommand = connection.CreateCommand())
                    {
                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[Agents]') IS NULL BEGIN CREATE TABLE Agents(AgentID uniqueidentifier unique not null, MachineIP nvarchar (100), MachineName nvarchar (500), OSVersion nvarchar (500), CMVersion nvarchar (500), Username nvarchar (500), Password nvarchar (1000), Status nvarchar (500)); CREATE UNIQUE INDEX idxAgentID ON Agents(AgentID); END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[AgentJobHistory]') IS NULL BEGIN CREATE TABLE AgentJobHistory(AgentID uniqueidentifier not null, TimeStamp datetime, Details nvarchar(MAX)); CREATE INDEX idxParentAgentID ON AgentJobHistory(AgentID); END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[AgentUserProfile]') IS NULL BEGIN CREATE TABLE AgentUserProfile(Name nvarchar (128), Value uniqueidentifier not null);END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "SELECT COUNT(*) FROM AgentUserProfile Where Name=@Name";
                        sqlCommand.Parameters.AddWithValue("@Name", "RemotePowerShellUser");
                        if ((int)sqlCommand.ExecuteScalar() == 0)
                        {
                            sqlCommand.CommandText = "INSERT INTO AgentUserProfile(Name,Value) VALUES(@Name,@Value)";
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@Value", Guid.NewGuid());
                            sqlCommand.ExecuteNonQuery();
                        }

                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[AgentSettings]') IS NULL BEGIN CREATE TABLE AgentSettings(Name nvarchar(500), Value nvarchar(1000)); END";
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public bool Delete(Guid agentID)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str =
                        string.Format(
                            "DELETE FROM AgentJobHistory WHERE AgentID = '{0}' ; DELETE FROM Agents WHERE AgentID = '{0}'",
                            agentID);
                    using (SqlCommand sqlCommand = new SqlCommand(str, connection))
                    {
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }

        public List<Agent> GetAll()
        {
            List<Agent> agents;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    agents = this.SetRemoteContext(connection,
                        "SELECT agents.*, agentDetail.Details FROM (SELECT AgentID, MAX(TimeStamp) AS MaxTime FROM AgentJobHistory GROUP BY AgentID ) agentDetailTemp INNER JOIN AgentJobHistory agentDetail ON agentDetail.AgentID = agentDetailTemp.AgentID AND agentDetail.TimeStamp = agentDetailTemp.MaxTime RIGHT OUTER JOIN Agents agents ON agents.AgentID = agentDetail.AgentID");
                }
                finally
                {
                    connection.Close();
                }
            }

            return agents;
        }

        public List<Agent> GetAllAvailableAgents()
        {
            List<Agent> agents;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    agents = this.SetRemoteContext(connection, this._getAllAvailableAgentsQuery);
                }
                finally
                {
                    connection.Close();
                }
            }

            return agents;
        }

        private string GetEncryptionKey()
        {
            string str;
            string str1 = string.Format("SELECT * FROM AgentSettings WHERE Name = '{0}'", Constants.Key);
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(str1, connection))
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read())
                            {
                                str = Convert.ToString(sqlDataReader[Constants.KeyValue]);
                                return str;
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }

                return null;
            }

            return str;
        }

        public List<Job> GetJobs(ActionStatus status)
        {
            List<Job> jobs;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str =
                        string.Format(
                            "SELECT * FROM JobHistory WHERE Status = 'Queued' AND (Edition = '{0}' OR Edition IS NULL) ORDER BY 'Created' ASC",
                            this._productName);
                    if (status == ActionStatus.Running)
                    {
                        str = string.Format(
                            "SELECT * FROM JobHistory WHERE (Status = 'Running' AND MachineName != '{0}') AND (Edition = '{1}' OR Edition IS NULL) ORDER BY 'Created' ASC",
                            Environment.MachineName, this._productName);
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(str, connection))
                    {
                        DataTable dataTable = new DataTable();
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            sqlDataAdapter.Fill(dataTable);
                        }

                        List<Job> jobs1 = new List<Job>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            JobUtils.ConvertTimeFieldsToLocalTime(row, JobUtils.DateTimeFieldsInJobHistory);
                            jobs1.Add(new Job(row));
                        }

                        jobs = jobs1;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return jobs;
        }

        public List<KeyValuePair<DateTime, string>> GetLogDetails(Guid agentID, bool isLatestEntry,
            bool sortAsc = false)
        {
            List<KeyValuePair<DateTime, string>> keyValuePairs;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str = (sortAsc ? "Asc" : "Desc");
                    object[] objArray = new object[]
                        { (isLatestEntry ? "TOP (1)" : string.Empty), agentID, Constants.Timestamp, str };
                    using (SqlCommand sqlCommand =
                           new SqlCommand(
                               string.Format(
                                   "SELECT {0} TimeStamp, Details FROM AgentJobHistory WHERE AgentID = '{1}'  order by {2} {3}",
                                   objArray), connection))
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            keyValuePairs = new List<KeyValuePair<DateTime, string>>();
                            while (sqlDataReader.Read())
                            {
                                string str1 = Convert.ToString(sqlDataReader[Constants.Details]);
                                if (sqlDataReader[Constants.Timestamp] is DBNull)
                                {
                                    continue;
                                }

                                DateTime item = (DateTime)sqlDataReader[Constants.Timestamp];
                                keyValuePairs.Add(new KeyValuePair<DateTime, string>(item.ToLocalTime(), str1));
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return keyValuePairs;
        }

        private string[] GetParameters()
        {
            string[] agentID = new string[]
            {
                Constants.AgentID, Constants.MachineIP, Constants.MachineName, Constants.OSVersion, Constants.CMVersion,
                Constants.UserName, Constants.Password, Constants.Status
            };
            return agentID;
        }

        private string GetUpdatedColumnName(string columnName)
        {
            return string.Format("@{0}", columnName);
        }

        private void InitializeEncryptionKey()
        {
            AgentDb._encryptionKey = this.GetEncryptionKey();
            if (string.IsNullOrEmpty(AgentDb._encryptionKey))
            {
                using (SqlConnection connection = base.Connection)
                {
                    try
                    {
                        object[] keyName = new object[]
                        {
                            Constants.KeyName, Constants.KeyValue, this.GetUpdatedColumnName(Constants.KeyName),
                            this.GetUpdatedColumnName(Constants.KeyValue)
                        };
                        using (SqlCommand sqlCommand =
                               new SqlCommand(
                                   string.Format("INSERT INTO AgentSettings ({0}, {1}) VALUES ({2}, {3})", keyName),
                                   connection))
                        {
                            AgentDb._encryptionKey = EncryptionUtil.GenerateEncryptionKey();
                            sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.KeyName),
                                Constants.Key);
                            sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.KeyValue),
                                AgentDb._encryptionKey);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public override void Open()
        {
            this.CreateDbIfNotExists();
            this.InitializeEncryptionKey();
        }

        public bool SetJobAsFailed(string jobID, string errorMessage)
        {
            base.DeleteLogItems(jobID);
            string str = "Error running job remotely";
            LogItem logItem = new LogItem(str, str, string.Empty, string.Empty, ActionOperationStatus.Failed)
            {
                Information = "Please review details",
                Details = errorMessage
            };
            base.AddLogItem(jobID, logItem);
            string[] strArrays = new string[]
                { "Status", "StatusMessage", "Started", "Finished", "LicensedDataUsed", "ResultsSummary" };
            object[] universalTime = new object[] { ActionStatus.Failed, "Failed", null, null, null, null };
            universalTime[2] = DateTime.Now.ToUniversalTime();
            universalTime[3] = DateTime.Now.ToUniversalTime();
            universalTime[4] = 0;
            universalTime[5] = str;
            this.UpdateJob(jobID, strArrays, universalTime);
            return true;
        }

        private List<Agent> SetRemoteContext(SqlConnection conn, string sqlQuery)
        {
            List<Agent> agents;
            using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    agents = new List<Agent>();
                    while (sqlDataReader.Read())
                    {
                        List<KeyValuePair<DateTime, string>> keyValuePairs = new List<KeyValuePair<DateTime, string>>();
                        string str = Convert.ToString(sqlDataReader[Constants.Details]);
                        keyValuePairs.Add(new KeyValuePair<DateTime, string>(DateTime.Now.ToLocalTime(),
                            (str.Length > 250 ? string.Concat(str.Substring(0, 250), "...") : str)));
                        AgentStatus agentStatu = (AgentStatus)Enum.Parse(typeof(AgentStatus),
                            Convert.ToString(sqlDataReader[Constants.Status]));
                        Agent agent = new Agent(new Guid(Convert.ToString(sqlDataReader[Constants.AgentID])),
                            Convert.ToString(sqlDataReader[Constants.MachineIP]),
                            Convert.ToString(sqlDataReader[Constants.MachineName]),
                            Convert.ToString(sqlDataReader[Constants.OSVersion]),
                            Convert.ToString(sqlDataReader[Constants.CMVersion]),
                            Convert.ToString(sqlDataReader[Constants.UserName]),
                            EncryptionUtil.DecryptByRijndaelKey(Convert.ToString(sqlDataReader[Constants.Password]),
                                AgentDb._encryptionKey), agentStatu, keyValuePairs);
                        agents.Add(agent);
                    }
                }
            }

            return agents;
        }

        public bool UpdateCMVersion(Agent agent)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str = string.Format("Update Agents SET CMVersion = '{0}' WHERE AgentID = '{1}';",
                        agent.CMVersion, agent.AgentID);
                    using (SqlCommand sqlCommand = new SqlCommand(str, connection))
                    {
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }

        public bool UpdateCredentials(Agent agent)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    object[] userName = new object[]
                    {
                        Constants.UserName, this.GetUpdatedColumnName(Constants.UserName), Constants.Password,
                        this.GetUpdatedColumnName(Constants.Password), Constants.AgentID,
                        this.GetUpdatedColumnName(Constants.AgentID)
                    };
                    using (SqlCommand sqlCommand =
                           new SqlCommand(
                               string.Format("UPDATE Agents SET {0} = {1}, {2} = {3} WHERE {4} = {5}", userName),
                               connection))
                    {
                        sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.AgentID), agent.AgentID);
                        sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.UserName),
                            agent.UserName);
                        sqlCommand.Parameters.AddWithValue(this.GetUpdatedColumnName(Constants.Password),
                            EncryptionUtil.EncryptByRijndaelKey(agent.Password, AgentDb._encryptionKey));
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }

        public void UpdateJob(string jobID, string[] parameters, object[] values)
        {
            if ((int)parameters.Length == 0)
            {
                return;
            }

            using (SqlConnection connection = base.Connection)
            {
                DateTime now = DateTime.Now;
                AgentDb agentDb = this;
                agentDb.tsOpening = agentDb.tsOpening + (DateTime.Now - now);
                DateTime dateTime = DateTime.Now;
                StringBuilder stringBuilder = new StringBuilder("Update JobHistory Set ");
                bool flag = true;
                string[] strArrays = parameters;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(str);
                    stringBuilder.Append(string.Format("=@{0}", str.Replace("[", "").Replace("]", "")));
                }

                stringBuilder.Append(" where JobID=@JobID");
                SqlCommand sqlCommand = new SqlCommand(stringBuilder.ToString(), connection);
                this.AddJobParams(parameters, sqlCommand, values, jobID);
                sqlCommand.ExecuteNonQuery();
                AgentDb now1 = this;
                now1.tsJobs = now1.tsJobs + (DateTime.Now - dateTime);
                now = DateTime.Now;
                AgentDb agentDb1 = this;
                agentDb1.tsClosing = agentDb1.tsClosing + (DateTime.Now - now);
                this.iJobTicker++;
                connection.Close();
            }
        }

        public bool UpdateOSVersion(Agent agent)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str = string.Format("Update Agents SET OSVersion = '{0}' WHERE AgentID = '{1}'",
                        agent.OSVersion, agent.AgentID);
                    using (SqlCommand sqlCommand = new SqlCommand(str, connection))
                    {
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }

        public bool UpdateStatus(Agent agent)
        {
            bool flag;
            using (SqlConnection connection = base.Connection)
            {
                try
                {
                    string str = string.Format("Update Agents SET Status = '{0}' Where AgentID = '{1}';",
                        agent.Status.ToString(), agent.AgentID);
                    using (SqlCommand sqlCommand = new SqlCommand(str, connection))
                    {
                        flag = sqlCommand.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return flag;
        }
    }
}