using Metalogix;
using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.Jobs.Adapters
{
    public class JobHistorySqlServerDb : IDisposable, IJobHistoryAdapter
    {
        public int iLogItemTicker;

        public int iJobTicker;

        public TimeSpan tsLogItems;

        public TimeSpan tsJobs;

        public TimeSpan tsOpening;

        public TimeSpan tsClosing;

        private readonly string _connectionString;

        private static SqlConnection _connection;

        private readonly static string _productName;

        private readonly object _sync = new object();

        private readonly bool _isOrganizer = ApplicationData.IsWeb;

        public string AdapterContext
        {
            get { return this._connectionString; }
        }

        public string AdapterType
        {
            get { return JobHistoryAdapterType.SqlServer.ToString(); }
        }

        public SqlConnection Connection
        {
            get
            {
                SqlConnection sqlConnection;
                lock (this._sync)
                {
                    if (string.IsNullOrEmpty(this._connectionString))
                    {
                        throw new InvalidOperationException(
                            "Cannot connect to Job History Database - connection string not specified.");
                    }

                    if (JobHistorySqlServerDb._connection == null ||
                        string.Compare(JobHistorySqlServerDb._connection.ConnectionString, this._connectionString) != 0)
                    {
                        if (JobHistorySqlServerDb._connection != null)
                        {
                            JobHistorySqlServerDb._connection.Close();
                            JobHistorySqlServerDb._connection.Dispose();
                            JobHistorySqlServerDb._connection = null;
                        }

                        JobHistorySqlServerDb._connection = new SqlConnection(this._connectionString);
                        JobHistorySqlServerDb._connection.Open();
                    }

                    SqlConnection sqlConnection1 = new SqlConnection(this._connectionString);
                    sqlConnection1.Open();
                    sqlConnection = sqlConnection1;
                }

                return sqlConnection;
            }
        }

        public string ConnectionString
        {
            get { return this._connectionString; }
        }

        static JobHistorySqlServerDb()
        {
            JobHistorySqlServerDb._productName = ApplicationData.GetProductName();
        }

        internal JobHistorySqlServerDb(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void AddJob(Job job_0)
        {
            this.AddJob(job_0, this.GetParameters());
        }

        public void AddJob(Job job_0, string[] sParams)
        {
            DateTime now = DateTime.Now;
            using (SqlConnection connection = this.Connection)
            {
                JobHistorySqlServerDb jobHistorySqlServerDb = this;
                jobHistorySqlServerDb.tsOpening = jobHistorySqlServerDb.tsOpening + (DateTime.Now - now);
                DateTime dateTime = DateTime.Now;
                StringBuilder stringBuilder = new StringBuilder("(");
                StringBuilder stringBuilder1 = new StringBuilder("(");
                string[] strArrays = sParams;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    stringBuilder.Append(string.Concat(str, ", "));
                    stringBuilder1.Append(string.Format("@{0}, ", str.Replace("[", "").Replace("]", "")));
                }

                if (!this._isOrganizer)
                {
                    stringBuilder.Append("Edition, ");
                    stringBuilder1.Append(string.Concat("'", JobHistorySqlServerDb._productName, "', "));
                }

                stringBuilder.Append("JobID)");
                stringBuilder1.Append("@JobID)");
                object[] objArray = new object[]
                    { "Insert into JobHistory ", stringBuilder, " values ", stringBuilder1 };
                SqlCommand sqlCommand = new SqlCommand(string.Concat(objArray), connection);
                this.AddJobParams(sParams, sqlCommand, job_0);
                sqlCommand.ExecuteNonQuery();
                JobHistorySqlServerDb now1 = this;
                now1.tsJobs = now1.tsJobs + (DateTime.Now - dateTime);
                now = DateTime.Now;
                JobHistorySqlServerDb jobHistorySqlServerDb1 = this;
                jobHistorySqlServerDb1.tsClosing = jobHistorySqlServerDb1.tsClosing + (DateTime.Now - now);
                this.iJobTicker++;
                connection.Close();
            }
        }

        private void AddJobParams(string[] sParams, SqlCommand sqlCommand_0, Job job_0)
        {
            object value;
            object universalTime;
            object stringValue;
            if (sParams.Any<string>((string sParam) => sParam == "[Action]"))
            {
                SqlParameter sqlParameter = sqlCommand_0.Parameters.Add("@Action", OleDbType.LongVarWChar);
                if (job_0.Action == null)
                {
                    stringValue = DBNull.Value;
                }
                else
                {
                    stringValue = JobUtils.GetStringValue(job_0.Action.ToXML());
                }

                sqlParameter.Value = stringValue;
            }

            if (sParams.Any<string>((string sParam) => sParam == "ResultsSummary"))
            {
                sqlCommand_0.Parameters.AddWithValue("@ResultsSummary",
                    JobUtils.GetStringValue(job_0.ResultsSummary, 255));
            }

            if (sParams.Any<string>((string sParam) => sParam == "Status"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Status", JobUtils.GetStringValue(job_0.Status.ToString()));
            }

            if (sParams.Any<string>((string sParam) => sParam == "StatusMessage"))
            {
                sqlCommand_0.Parameters.AddWithValue("@StatusMessage",
                    JobUtils.GetStringValue(job_0.StatusMessage, 255));
            }

            if (sParams.Any<string>((string sParam) => sParam == "Title"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Title", JobUtils.GetStringValue(job_0.Title, 255));
            }

            if (sParams.Any<string>((string sParam) => sParam == "Source"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Source", JobUtils.GetStringValue(job_0.Source, 255));
            }

            if (sParams.Any<string>((string sParam) => sParam == "SourceXml"))
            {
                sqlCommand_0.Parameters.AddWithValue("@SourceXml", JobUtils.GetStringValue(job_0.SourceXml));
            }

            if (sParams.Any<string>((string sParam) => sParam == "Target"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Target", JobUtils.GetStringValue(job_0.Target, 255));
            }

            if (sParams.Any<string>((string sParam) => sParam == "TargetXml"))
            {
                sqlCommand_0.Parameters.AddWithValue("@TargetXml", JobUtils.GetStringValue(job_0.TargetXml));
            }

            if (sParams.Any<string>((string sParam) => sParam == "Started"))
            {
                SqlParameter sqlParameter1 = sqlCommand_0.Parameters.Add("@Started", OleDbType.Date);
                if (!job_0.Started.HasValue)
                {
                    universalTime = DBNull.Value;
                }
                else
                {
                    universalTime = job_0.Started.Value.ToUniversalTime();
                }

                sqlParameter1.Value = universalTime;
            }

            if (sParams.Any<string>((string sParam) => sParam == "Finished"))
            {
                SqlParameter sqlParameter2 = sqlCommand_0.Parameters.Add("@Finished", OleDbType.Date);
                if (!job_0.Finished.HasValue)
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = job_0.Finished.Value.ToUniversalTime();
                }

                sqlParameter2.Value = value;
            }

            if (sParams.Any<string>((string sParam) => sParam == "Created"))
            {
                sqlCommand_0.Parameters.Add("@Created", OleDbType.Date).Value = job_0.Created.ToUniversalTime();
            }

            if (sParams.Any<string>((string sParam) => sParam == "LicensedDataUsed"))
            {
                sqlCommand_0.Parameters.Add("@LicensedDataUsed", OleDbType.BigInt).Value = job_0.LicenseDataUsed;
            }

            if (sParams.Any<string>((string sParam) => sParam == "UserName"))
            {
                sqlCommand_0.Parameters.AddWithValue("@UserName", JobUtils.GetStringValue(job_0.UserName));
            }

            if (sParams.Any<string>((string sParam) => sParam == "MachineName"))
            {
                sqlCommand_0.Parameters.AddWithValue("@MachineName", JobUtils.GetStringValue(job_0.MachineName));
            }

            if (sParams.Any<string>((string sParam) => sParam == "CreatedBy"))
            {
                sqlCommand_0.Parameters.AddWithValue("@CreatedBy", JobUtils.GetStringValue(job_0.CreatedBy));
            }

            sqlCommand_0.Parameters.AddWithValue("@JobID", job_0.JobID);
        }

        public void AddLogItem(string sJobID, LogItem logItem)
        {
            string[] strArrays = new string[]
            {
                "Details", "Source", "Target", "SourceContent", "TargetContent", "Information", "Operation", "ItemName",
                "Status", "[TimeStamp]", "[FinishedTime]", "LicensedDataUsed"
            };
            this.AddLogItem(sJobID, logItem, strArrays);
        }

        public void AddLogItem(string sJobID, LogItem logItem, string[] sParams)
        {
            DateTime now = DateTime.Now;
            using (SqlConnection connection = this.Connection)
            {
                JobHistorySqlServerDb jobHistorySqlServerDb = this;
                jobHistorySqlServerDb.tsOpening = jobHistorySqlServerDb.tsOpening + (DateTime.Now - now);
                DateTime dateTime = DateTime.Now;
                StringBuilder stringBuilder = new StringBuilder("(");
                StringBuilder stringBuilder1 = new StringBuilder("(");
                string[] strArrays = sParams;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    stringBuilder.Append(string.Concat(str, ", "));
                    stringBuilder1.Append(string.Format("@{0}, ", str.Replace("[", "").Replace("]", "")));
                }

                stringBuilder.Append("JobID, LogItemID)");
                stringBuilder1.Append("@JobID,@LogItemId)");
                object[] objArray = new object[]
                    { "Insert into JobLogItems ", stringBuilder, " values ", stringBuilder1 };
                SqlCommand sqlCommand = new SqlCommand(string.Concat(objArray), connection);
                this.AddLogItemParams(sqlCommand, sJobID, logItem);
                sqlCommand.ExecuteNonQuery();
                JobHistorySqlServerDb now1 = this;
                now1.tsLogItems = now1.tsLogItems + (DateTime.Now - dateTime);
                now = DateTime.Now;
                JobHistorySqlServerDb jobHistorySqlServerDb1 = this;
                jobHistorySqlServerDb1.tsClosing = jobHistorySqlServerDb1.tsClosing + (DateTime.Now - now);
                this.iLogItemTicker++;
                connection.Close();
            }
        }

        private void AddLogItemParams(SqlCommand sqlCommand_0, string sJobID, LogItem logItem)
        {
            if (sqlCommand_0.CommandText.Contains("Details"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Details", JobUtils.GetStringValue(logItem.Details));
            }

            if (sqlCommand_0.CommandText.Contains("Source,") || sqlCommand_0.CommandText.Contains("Source="))
            {
                sqlCommand_0.Parameters.AddWithValue("@Source", JobUtils.GetStringValue(logItem.Source, 500));
            }

            if (sqlCommand_0.CommandText.Contains("Target,") || sqlCommand_0.CommandText.Contains("Target="))
            {
                sqlCommand_0.Parameters.AddWithValue("@Target", JobUtils.GetStringValue(logItem.Target, 500));
            }

            if (sqlCommand_0.CommandText.Contains("SourceContent"))
            {
                sqlCommand_0.Parameters.AddWithValue("@SourceContent", JobUtils.GetStringValue(logItem.SourceContent));
            }

            if (sqlCommand_0.CommandText.Contains("TargetContent"))
            {
                sqlCommand_0.Parameters.AddWithValue("@TargetContent", JobUtils.GetStringValue(logItem.TargetContent));
            }

            if (sqlCommand_0.CommandText.Contains("Information"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Information", JobUtils.GetStringValue(logItem.Information));
            }

            if (sqlCommand_0.CommandText.Contains("Operation"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Operation", JobUtils.GetStringValue(logItem.Operation, 50));
            }

            if (sqlCommand_0.CommandText.Contains("ItemName"))
            {
                sqlCommand_0.Parameters.AddWithValue("@ItemName", JobUtils.GetStringValue(logItem.ItemName, 150));
            }

            if (sqlCommand_0.CommandText.Contains("Status"))
            {
                sqlCommand_0.Parameters.AddWithValue("@Status", logItem.Status.ToString());
            }

            if (sqlCommand_0.CommandText.Contains("[TimeStamp]"))
            {
                sqlCommand_0.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value =
                    logItem.TimeStamp.ToUniversalTime();
            }

            if (sqlCommand_0.CommandText.Contains("[FinishedTime]"))
            {
                sqlCommand_0.Parameters.Add("@FinishedTime", OleDbType.Date).Value =
                    logItem.FinishedTime.ToUniversalTime();
            }

            if (sqlCommand_0.CommandText.Contains("LicensedDataUsed"))
            {
                sqlCommand_0.Parameters.Add("@LicensedDataUsed", OleDbType.BigInt).Value = logItem.LicenseDataUsed;
            }

            sqlCommand_0.Parameters.AddWithValue("@JobID", sJobID);
            sqlCommand_0.Parameters.AddWithValue("@LogItemID", logItem.ID);
        }

        public void Close()
        {
            this.Connection.Close();
        }

        public virtual void CreateDbIfNotExists()
        {
            try
            {
                using (SqlConnection connection = this.Connection)
                {
                    using (SqlCommand sqlCommand = connection.CreateCommand())
                    {
                        if (this._isOrganizer)
                        {
                            sqlCommand.CommandText =
                                "IF OBJECT_ID(N'[JobHistory]') IS NULL BEGIN CREATE TABLE JobHistory(JobID nvarchar (40) unique not null, Title nvarchar (256), Source nvarchar (256), Target nvarchar (256), Status nvarchar (20), StatusMessage nvarchar (256), Created datetime, ResultsSummary nvarchar(256), LicensedDataUsed bigint, Started datetime, Finished datetime, Action ntext, SourceXml ntext, TargetXml ntext, UserName nvarchar(100), MachineName nvarchar(100), CreatedBy nvarchar(100)); CREATE UNIQUE INDEX idxJobID ON JobHistory(JobID); END";
                        }
                        else
                        {
                            sqlCommand.CommandText =
                                "IF OBJECT_ID(N'[JobHistory]') IS NULL BEGIN CREATE TABLE JobHistory(JobID nvarchar (40) unique not null, Title nvarchar (256), Source nvarchar (256), Target nvarchar (256), Status nvarchar (20), StatusMessage nvarchar (256), Created datetime, ResultsSummary nvarchar(256), LicensedDataUsed bigint, Started datetime, Finished datetime, Action ntext, SourceXml ntext, TargetXml ntext, UserName nvarchar(100), MachineName nvarchar(100), CreatedBy nvarchar(100), Edition nvarchar(256)); CREATE UNIQUE INDEX idxJobID ON JobHistory(JobID); END";
                        }

                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF OBJECT_ID(N'[JobLogItems]') IS NULL BEGIN CREATE TABLE JobLogItems(JobID nvarchar (40) not null, LogItemID nvarchar(40) unique not null, TimeStamp datetime, FinishedTime datetime, Status nvarchar (20), Operation nvarchar(50), ItemName nvarchar(150), Source nvarchar(500), Target nvarchar(500), Information ntext, Details ntext, SourceContent ntext, TargetContent ntext, LicensedDataUsed bigint); CREATE UNIQUE INDEX idxLogItemID ON JobLogItems(LogItemID); CREATE INDEX idxParentJobID ON JobLogItems(JobID); END";
                        sqlCommand.ExecuteNonQuery();
                        if (!this._isOrganizer)
                        {
                            sqlCommand.CommandText =
                                "IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'JobHistory'  AND  (COLUMN_NAME = 'Edition')) BEGIN ALTER TABLE JobHistory ADD Edition nvarchar(256)END";
                            sqlCommand.ExecuteNonQuery();
                        }

                        sqlCommand.CommandText =
                            "IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'JobHistory'  AND  (COLUMN_NAME = 'UserName' OR COLUMN_NAME = 'MachineName' OR COLUMN_NAME = 'CreatedBy')) BEGIN ALTER TABLE JobHistory ADD UserName nvarchar(100), MachineName nvarchar(100), CreatedBy nvarchar(100) END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'JobLogItems' AND COLUMN_NAME = 'Target') <= 255 BEGIN ALTER TABLE JobLogItems ALTER COLUMN Target nvarchar(500);END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'JobLogItems' AND COLUMN_NAME = 'Source') <= 255 BEGIN ALTER TABLE JobLogItems ALTER COLUMN Source nvarchar(500);END";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText =
                            "IF (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'JobLogItems' AND COLUMN_NAME = 'ItemName') <= 50 BEGIN ALTER TABLE JobLogItems ALTER COLUMN ItemName nvarchar(150);END";
                        sqlCommand.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Could not create job history database: ", exception.Message));
            }
        }

        public void DeleteJobs(string sJobIDs)
        {
            string[] strArrays = sJobIDs.Split(new char[] { ',' });
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                using (SqlConnection connection = this.Connection)
                {
                    string str1 = string.Concat("Delete From JobLogItems where JobID=", str);
                    (new SqlCommand(str1, connection)).ExecuteNonQuery();
                    connection.Close();
                }

                using (SqlConnection sqlConnection = this.Connection)
                {
                    string str2 = string.Concat("Delete From JobHistory where JobID=", str);
                    (new SqlCommand(str2, sqlConnection)).ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
        }

        public void DeleteLogItems(string sJobID)
        {
            using (SqlConnection connection = this.Connection)
            {
                string str = string.Concat("Delete From JobLogItems where JobID in ('", sJobID, "')");
                (new SqlCommand(str, connection)).ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Dispose()
        {
        }

        public Job GetJob(string sJobID)
        {
            Job job;
            using (SqlConnection connection = this.Connection)
            {
                string str = string.Concat("Select * from JobHistory where JobID = '", sJobID, "'");
                SqlCommand sqlCommand = new SqlCommand(str, connection);
                DataTable dataTable = new DataTable();
                (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
                if (dataTable.Rows.Count != 0)
                {
                    connection.Close();
                    JobUtils.ConvertTimeFieldsToLocalTime(dataTable.Rows[0], JobUtils.DateTimeFieldsInJobHistory);
                    job = new Job(dataTable.Rows[0]);
                }
                else
                {
                    job = null;
                }
            }

            return job;
        }

        public List<Job> GetJobs(params string[] jobIds)
        {
            List<Job> jobs;
            using (SqlConnection connection = this.Connection)
            {
                string str = null;
                if ((int)jobIds.Length <= 0)
                {
                    str = (this._isOrganizer
                        ? "SELECT * FROM JobHistory ORDER BY Created"
                        : string.Format(
                            "SELECT * FROM JobHistory WHERE Edition = '{0}' OR Edition IS NULL ORDER BY Created",
                            JobHistorySqlServerDb._productName));
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string[] strArrays = jobIds;
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = strArrays[i];
                        stringBuilder.Append(string.Concat((stringBuilder.Length > 0 ? ",'" : "'"), str1, "'"));
                    }

                    str = (this._isOrganizer
                        ? string.Format("SELECT * FROM JobHistory WHERE JobID IN ({0}) ORDER BY Created", stringBuilder)
                        : string.Format(
                            "SELECT * FROM JobHistory WHERE JobID IN ({0}) AND (Edition = '{1}' OR Edition IS NULL) ORDER BY Created",
                            stringBuilder, JobHistorySqlServerDb._productName));
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

                    connection.Close();
                    jobs = jobs1;
                }
            }

            return jobs;
        }

        public LogItem GetLogItem(Job job_0, string sLogItemID)
        {
            LogItem logItem;
            using (SqlConnection connection = this.Connection)
            {
                SqlCommand sqlCommand =
                    new SqlCommand("Select * from JobLogItems where JobID = @JobID And LogItemID = @LogItemID ",
                        connection);
                sqlCommand.Parameters.Add(new SqlParameter("@JobID", job_0.JobID));
                sqlCommand.Parameters.Add(new SqlParameter("@LogItemID", sLogItemID));
                DataTable dataTable = new DataTable();
                (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
                if (dataTable.Rows.Count != 0)
                {
                    connection.Close();
                    JobUtils.ConvertTimeFieldsToLocalTime(dataTable.Rows[0], JobUtils.DateTimeFieldsInJobLogItems);
                    logItem = new LogItem(dataTable.Rows[0], job_0.JobHistoryDb);
                }
                else
                {
                    logItem = null;
                }
            }

            return logItem;
        }

        public void GetLogItemDetails(string sLogItemID, LogItem item)
        {
            string str;
            string str1;
            string str2;
            using (SqlConnection connection = this.Connection)
            {
                SqlCommand sqlCommand =
                    new SqlCommand(
                        "Select Details, SourceContent, TargetContent from JobLogItems Where LogItemID = @LogItemID",
                        connection);
                sqlCommand.Parameters.Add(new SqlParameter("@LogItemID", sLogItemID));
                DataTable dataTable = new DataTable();
                (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
                if (dataTable.Rows.Count != 0)
                {
                    LogItem logItem = item;
                    if (dataTable.Rows[0]["Details"] is DBNull)
                    {
                        str = null;
                    }
                    else
                    {
                        str = (string)dataTable.Rows[0]["Details"];
                    }

                    logItem.Details = str;
                    LogItem logItem1 = item;
                    if (dataTable.Rows[0]["SourceContent"] is DBNull)
                    {
                        str1 = null;
                    }
                    else
                    {
                        str1 = (string)dataTable.Rows[0]["SourceContent"];
                    }

                    logItem1.SourceContent = str1;
                    LogItem logItem2 = item;
                    if (dataTable.Rows[0]["TargetContent"] is DBNull)
                    {
                        str2 = null;
                    }
                    else
                    {
                        str2 = (string)dataTable.Rows[0]["TargetContent"];
                    }

                    logItem2.TargetContent = str2;
                    connection.Close();
                }
            }
        }

        public List<LogItem> GetLogItems(Job job_0)
        {
            List<LogItem> logItems;
            using (SqlConnection connection = this.Connection)
            {
                string str =
                    string.Concat(
                        "Select JobID, LogItemID, Source, Target, Information, Operation, ItemName, Status, [TimeStamp] from JobLogItems Where JobID In ('",
                        job_0.JobID, "') ORDER BY [TimeStamp]");
                SqlCommand sqlCommand = new SqlCommand(str, connection);
                DataTable dataTable = new DataTable();
                (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
                List<LogItem> logItems1 = new List<LogItem>();
                foreach (DataRow row in dataTable.Rows)
                {
                    JobUtils.ConvertTimeFieldsToLocalTime(row, JobUtils.TimeStampFieldInJobLogItems);
                    LogItem logItem = new LogItem(row, job_0.JobHistoryDb);
                    if (job_0.Action != null)
                    {
                        logItem.ActionLicensingUnit = job_0.Action.LicensingUnit;
                        logItem.ActionLicensingDescriptor = job_0.Action.LicensingDescriptor;
                    }

                    logItems1.Add(logItem);
                }

                connection.Close();
                logItems = logItems1;
            }

            return logItems;
        }

        private string[] GetParameters()
        {
            string[] strArrays = new string[]
            {
                "[Action]", "ResultsSummary", "Status", "StatusMessage", "Title", "Source", "Target", "SourceXml",
                "TargetXml", "Started", "Finished", "Created", "LicensedDataUsed", "UserName", "MachineName",
                "CreatedBy"
            };
            return strArrays;
        }

        public virtual void Open()
        {
            this.CreateDbIfNotExists();
        }

        public void UpdateJob(Job job_0)
        {
            this.UpdateJob(job_0, this.GetParameters());
        }

        public void UpdateJob(Job job_0, string[] sParams)
        {
            if ((int)sParams.Length == 0)
            {
                return;
            }

            using (SqlConnection connection = this.Connection)
            {
                DateTime now = DateTime.Now;
                JobHistorySqlServerDb jobHistorySqlServerDb = this;
                jobHistorySqlServerDb.tsOpening = jobHistorySqlServerDb.tsOpening + (DateTime.Now - now);
                DateTime dateTime = DateTime.Now;
                StringBuilder stringBuilder = new StringBuilder("Update JobHistory Set ");
                bool flag = true;
                string[] strArrays = sParams;
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
                this.AddJobParams(sParams, sqlCommand, job_0);
                sqlCommand.ExecuteNonQuery();
                JobHistorySqlServerDb now1 = this;
                now1.tsJobs = now1.tsJobs + (DateTime.Now - dateTime);
                now = DateTime.Now;
                JobHistorySqlServerDb jobHistorySqlServerDb1 = this;
                jobHistorySqlServerDb1.tsClosing = jobHistorySqlServerDb1.tsClosing + (DateTime.Now - now);
                this.iJobTicker++;
                connection.Close();
            }
        }

        public void UpdateJobAction(Job[] jobs, string sActionXML)
        {
            using (SqlConnection connection = this.Connection)
            {
                StringBuilder stringBuilder = new StringBuilder((int)jobs.Length * 2 - 1);
                int num = 0;
                Job[] jobArray = jobs;
                for (int i = 0; i < (int)jobArray.Length; i++)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append(string.Concat("@JobID", num.ToString()));
                    num++;
                }

                string str = string.Concat("Update JobHistory Set [Action]=@Action where JobID in (", stringBuilder,
                    ")");
                SqlCommand sqlCommand = new SqlCommand(str, connection);
                sqlCommand.Parameters.Add("@Action", SqlDbType.NText).Value = sActionXML;
                num = 0;
                Job[] jobArray1 = jobs;
                for (int j = 0; j < (int)jobArray1.Length; j++)
                {
                    Job job = jobArray1[j];
                    string str1 = string.Concat("@JobID", num.ToString());
                    sqlCommand.Parameters.AddWithValue(str1, job.JobID);
                    num++;
                }

                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem)
        {
            string[] strArrays = new string[]
            {
                "Details", "Source", "Target", "SourceContent", "TargetContent", "Information", "Operation", "ItemName",
                "Status", "[TimeStamp]"
            };
            strArrays[10] = "[FinishedTime]";
            strArrays[11] = "LicensedDataUsed";
            this.UpdateLogItem(sJobID, sLogItemID, logItem, strArrays);
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem, string[] sParams)
        {
            using (SqlConnection connection = this.Connection)
            {
                if ((int)sParams.Length != 0)
                {
                    DateTime now = DateTime.Now;
                    JobHistorySqlServerDb jobHistorySqlServerDb = this;
                    jobHistorySqlServerDb.tsOpening = jobHistorySqlServerDb.tsOpening + (DateTime.Now - now);
                    DateTime dateTime = DateTime.Now;
                    StringBuilder stringBuilder = new StringBuilder("Update JobLogItems Set ");
                    bool flag = true;
                    string[] strArrays = sParams;
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

                    stringBuilder.Append(" Where JobID=@JobID And LogItemID=@LogItemID");
                    SqlCommand sqlCommand = new SqlCommand(stringBuilder.ToString(), connection);
                    this.AddLogItemParams(sqlCommand, sJobID, logItem);
                    sqlCommand.ExecuteNonQuery();
                    logItem.ClearDetails();
                    JobHistorySqlServerDb now1 = this;
                    now1.tsLogItems = now1.tsLogItems + (DateTime.Now - dateTime);
                    now = DateTime.Now;
                    JobHistorySqlServerDb jobHistorySqlServerDb1 = this;
                    jobHistorySqlServerDb1.tsClosing = jobHistorySqlServerDb1.tsClosing + (DateTime.Now - now);
                    this.iLogItemTicker++;
                    connection.Close();
                }
            }
        }
    }
}