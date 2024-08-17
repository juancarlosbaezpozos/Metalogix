using Metalogix;
using Metalogix.Actions;
using Metalogix.Core;
using Metalogix.Explorer;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Metalogix.Jobs.Adapters
{
    public class JobHistorySqlCeDb : IDisposable, IJobHistoryAdapter
    {
        public int iLogItemTicker;

        public int iJobTicker;

        public TimeSpan tsLogItems;

        public TimeSpan tsJobs;

        public TimeSpan tsOpening;

        public TimeSpan tsClosing;

        private ReaderWriterLockSlim _connectionLock = new ReaderWriterLockSlim();

        private readonly static string _productName;

        private SqlCeConnection m_connection;

        public string AdapterContext
        {
            get { return JustDecompileGenerated_get_AdapterContext(); }
            set { JustDecompileGenerated_set_AdapterContext(value); }
        }

        private string JustDecompileGenerated_AdapterContext_k__BackingField;

        public string JustDecompileGenerated_get_AdapterContext()
        {
            return this.JustDecompileGenerated_AdapterContext_k__BackingField;
        }

        private void JustDecompileGenerated_set_AdapterContext(string value)
        {
            this.JustDecompileGenerated_AdapterContext_k__BackingField = value;
        }

        public string AdapterType
        {
            get { return JobHistoryAdapterType.SqlCe.ToString(); }
        }

        private SqlCeConnection Connection
        {
            get
            {
                if (this.m_connection == null)
                {
                    throw new ObjectDisposedException("Attempted to access a closed connection");
                }

                if (this.m_connection.State == ConnectionState.Closed)
                {
                    lock (this.m_connection)
                    {
                        if (this.m_connection.State == ConnectionState.Closed)
                        {
                            this.m_connection.Open();
                        }
                    }
                }

                return this.m_connection;
            }
            set
            {
                if (this.m_connection != null)
                {
                    this.m_connection.Close();
                    this.m_connection.Dispose();
                }

                this.m_connection = value;
            }
        }

        static JobHistorySqlCeDb()
        {
            JobHistorySqlCeDb._productName = ApplicationData.GetProductName();
        }

        internal JobHistorySqlCeDb(string sFileName)
        {
            this.AdapterContext = sFileName;
        }

        public void AddJob(Job job_0)
        {
            this.AddJob(job_0, this.GetParameters());
        }

        public void AddJob(Job job_0, string[] sParams)
        {
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                StringBuilder stringBuilder = new StringBuilder("(");
                StringBuilder stringBuilder1 = new StringBuilder("(");
                string[] strArrays = sParams;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    stringBuilder.Append(string.Concat(str, ", "));
                    stringBuilder1.Append("?, ");
                }

                stringBuilder.Append("Edition, ");
                stringBuilder1.Append(string.Concat("'", JobHistorySqlCeDb._productName, "', "));
                stringBuilder.Append("JobID)");
                stringBuilder1.Append("?)");
                object[] objArray = new object[] { "Insert into Jobs ", stringBuilder, " values ", stringBuilder1 };
                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(string.Concat(objArray), connection))
                {
                    this.AddJobParams(sParams, sqlCeCommand, job_0);
                    sqlCeCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        private void AddJobParams(string[] sParams, SqlCeCommand sqlCeCommand_0, Job job_0)
        {
            object value;
            object started;
            object stringValue;
            string actionXml = job_0.GetActionXml();
            if (sParams.Any<string>((string sParam) => sParam == "[Action]"))
            {
                SqlCeParameter sqlCeParameter = sqlCeCommand_0.Parameters.Add("@Action", SqlDbType.NText);
                if (actionXml == null)
                {
                    stringValue = DBNull.Value;
                }
                else
                {
                    stringValue = JobUtils.GetStringValue(actionXml);
                }

                sqlCeParameter.Value = stringValue;
            }

            if (sParams.Any<string>((string sParam) => sParam == "ResultsSummary"))
            {
                sqlCeCommand_0.Parameters.Add("ResultsSummary", SqlDbType.NVarChar, 256).Value =
                    JobUtils.GetStringValue(job_0.ResultsSummary, 255);
            }

            if (sParams.Any<string>((string sParam) => sParam == "Status"))
            {
                sqlCeCommand_0.Parameters.Add("Status", SqlDbType.NVarChar, 20).Value =
                    JobUtils.GetStringValue(job_0.Status.ToString(), 20);
            }

            if (sParams.Any<string>((string sParam) => sParam == "StatusMessage"))
            {
                sqlCeCommand_0.Parameters.Add("StatusMessage", SqlDbType.NVarChar, 256).Value =
                    JobUtils.GetStringValue(job_0.StatusMessage, 255);
            }

            if (sParams.Any<string>((string sParam) => sParam == "Title"))
            {
                sqlCeCommand_0.Parameters.Add("Title", SqlDbType.NVarChar, 256).Value =
                    JobUtils.GetStringValue(job_0.Title, 255);
            }

            if (sParams.Any<string>((string sParam) => sParam == "Source"))
            {
                sqlCeCommand_0.Parameters.Add("Source", SqlDbType.NVarChar, 256).Value =
                    JobUtils.GetStringValue(job_0.Source, 255);
            }

            if (sParams.Any<string>((string sParam) => sParam == "SourceXml"))
            {
                sqlCeCommand_0.Parameters.Add("@SourceXml", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(job_0.SourceXml);
            }

            if (sParams.Any<string>((string sParam) => sParam == "Target"))
            {
                sqlCeCommand_0.Parameters.Add("Target", SqlDbType.NVarChar, 256).Value =
                    JobUtils.GetStringValue(job_0.Target, 255);
            }

            if (sParams.Any<string>((string sParam) => sParam == "TargetXml"))
            {
                sqlCeCommand_0.Parameters.Add("@TargetXml", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(job_0.TargetXml);
            }

            if (sParams.Any<string>((string sParam) => sParam == "Started"))
            {
                SqlCeParameter sqlCeParameter1 = sqlCeCommand_0.Parameters.Add("@Started", OleDbType.Date);
                if (!job_0.Started.HasValue)
                {
                    started = DBNull.Value;
                }
                else
                {
                    started = job_0.Started;
                }

                sqlCeParameter1.Value = started;
            }

            if (sParams.Any<string>((string sParam) => sParam == "Finished"))
            {
                SqlCeParameter sqlCeParameter2 = sqlCeCommand_0.Parameters.Add("@Finished", OleDbType.Date);
                if (!job_0.Finished.HasValue)
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = job_0.Finished;
                }

                sqlCeParameter2.Value = value;
            }

            if (sParams.Any<string>((string sParam) => sParam == "Created"))
            {
                sqlCeCommand_0.Parameters.Add("@Created", OleDbType.Date).Value = job_0.Created;
            }

            if (sParams.Any<string>((string sParam) => sParam == "LicensedDataUsed"))
            {
                sqlCeCommand_0.Parameters.Add("@LicensedDataUsed", OleDbType.BigInt).Value = job_0.LicenseDataUsed;
            }

            if (sParams.Any<string>((string sParam) => sParam == "UserName"))
            {
                sqlCeCommand_0.Parameters.AddWithValue("@UserName", JobUtils.GetStringValue(job_0.UserName));
            }

            if (sParams.Any<string>((string sParam) => sParam == "MachineName"))
            {
                sqlCeCommand_0.Parameters.AddWithValue("@MachineName", JobUtils.GetStringValue(job_0.MachineName));
            }

            if (sParams.Any<string>((string sParam) => sParam == "CreatedBy"))
            {
                sqlCeCommand_0.Parameters.AddWithValue("@CreatedBy", JobUtils.GetStringValue(job_0.CreatedBy));
            }

            sqlCeCommand_0.Parameters.AddWithValue("JobID", job_0.JobID);
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
            this._connectionLock.EnterReadLock();
            try
            {
                try
                {
                    SqlCeConnection connection = this.Connection;
                    StringBuilder stringBuilder = new StringBuilder("(");
                    StringBuilder stringBuilder1 = new StringBuilder("(");
                    string[] strArrays = sParams;
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str = strArrays[i];
                        stringBuilder.Append(string.Concat(str, ", "));
                        stringBuilder1.Append("?, ");
                    }

                    stringBuilder.Append("JobID, LogItemID)");
                    stringBuilder1.Append("?,?)");
                    object[] objArray = new object[]
                        { "Insert into LogItems ", stringBuilder, " values ", stringBuilder1 };
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(string.Concat(objArray), connection))
                    {
                        this.AddLogItemParams(sqlCeCommand, sJobID, logItem);
                        sqlCeCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception exception)
                {
                    Logging.LogExceptionToTextFileWithEventLogBackup(
                        new Exception("Unable to add a new log item. This may be because your log file is full",
                            exception), "AddLogItem", true);
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        private void AddLogItemParams(SqlCeCommand sqlCeCommand_0, string sJobID, LogItem logItem)
        {
            if (sqlCeCommand_0.CommandText.Contains("Details"))
            {
                sqlCeCommand_0.Parameters.Add("Details", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(logItem.Details);
            }

            if (sqlCeCommand_0.CommandText.Contains("Source,") || sqlCeCommand_0.CommandText.Contains("Source=?"))
            {
                sqlCeCommand_0.Parameters.Add("Source", SqlDbType.NVarChar, 500).Value =
                    JobUtils.GetStringValue(logItem.Source, 500);
            }

            if (sqlCeCommand_0.CommandText.Contains("Target,") || sqlCeCommand_0.CommandText.Contains("Target=?"))
            {
                sqlCeCommand_0.Parameters.Add("Target", SqlDbType.NVarChar, 500).Value =
                    JobUtils.GetStringValue(logItem.Target, 500);
            }

            if (sqlCeCommand_0.CommandText.Contains("SourceContent"))
            {
                sqlCeCommand_0.Parameters.Add("SourceContent", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(logItem.SourceContent);
            }

            if (sqlCeCommand_0.CommandText.Contains("TargetContent"))
            {
                sqlCeCommand_0.Parameters.Add("TargetContent", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(logItem.TargetContent);
            }

            if (sqlCeCommand_0.CommandText.Contains("Information"))
            {
                sqlCeCommand_0.Parameters.Add("Information", SqlDbType.NText).Value =
                    JobUtils.GetStringValue(logItem.Information);
            }

            if (sqlCeCommand_0.CommandText.Contains("Operation"))
            {
                sqlCeCommand_0.Parameters.Add("Operation", SqlDbType.NVarChar, 50).Value =
                    JobUtils.GetStringValue(logItem.Operation, 50);
            }

            if (sqlCeCommand_0.CommandText.Contains("ItemName"))
            {
                sqlCeCommand_0.Parameters.Add("ItemName", SqlDbType.NVarChar, 150).Value =
                    JobUtils.GetStringValue(logItem.ItemName, 150);
            }

            if (sqlCeCommand_0.CommandText.Contains("Status"))
            {
                sqlCeCommand_0.Parameters.Add("Status", SqlDbType.NVarChar, 20).Value =
                    JobUtils.GetStringValue(logItem.Status.ToString(), 20);
            }

            if (sqlCeCommand_0.CommandText.Contains("[TimeStamp]"))
            {
                sqlCeCommand_0.Parameters.Add("[TimeStamp]", OleDbType.Date).Value = logItem.TimeStamp;
            }

            if (sqlCeCommand_0.CommandText.Contains("[FinishedTime]"))
            {
                sqlCeCommand_0.Parameters.Add("[FinishedTime]", OleDbType.Date).Value = logItem.FinishedTime;
            }

            if (sqlCeCommand_0.CommandText.Contains("LicensedDataUsed"))
            {
                sqlCeCommand_0.Parameters.Add("LicensedDataUsed", OleDbType.BigInt).Value = logItem.LicenseDataUsed;
            }

            sqlCeCommand_0.Parameters.AddWithValue("JobID", sJobID);
            sqlCeCommand_0.Parameters.AddWithValue("LogItemID", logItem.ID);
        }

        public void Close()
        {
            this._connectionLock.EnterWriteLock();
            try
            {
                this.Connection = null;
            }
            finally
            {
                this._connectionLock.ExitWriteLock();
            }
        }

        private static void CopyStream(Stream stream_0, Stream dest)
        {
            if (stream_0 == null || dest == null)
            {
                throw new Exception("Internal error: cannot copy a null stream");
            }

            byte[] numArray = new byte[8192];
            while (true)
            {
                int num = stream_0.Read(numArray, 0, (int)numArray.Length);
                int num1 = num;
                if (num <= 0)
                {
                    break;
                }

                dest.Write(numArray, 0, num1);
            }
        }

        private void CreateDb(string connString)
        {
            try
            {
                using (SqlCeEngine sqlCeEngine = new SqlCeEngine(connString))
                {
                    sqlCeEngine.CreateDatabase();
                }

                using (SqlCeConnection sqlCeConnection = new SqlCeConnection(connString))
                {
                    sqlCeConnection.Open();
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(
                               "create table Jobs(JobID nvarchar (40) unique not null, Title nvarchar(256), Source nvarchar(256), Target nvarchar(256), Status nvarchar(20), StatusMessage nvarchar(256), Created datetime, ResultsSummary nvarchar(256), LicensedDataUsed bigint, Started datetime, Finished datetime, Action ntext, SourceXml ntext, TargetXml ntext, UserName nvarchar(100), MachineName nvarchar(100), CreatedBy nvarchar(100), Edition nvarchar(256))",
                               sqlCeConnection))
                    {
                        sqlCeCommand.ExecuteNonQuery();
                        sqlCeCommand.CommandText = "create unique index idxJobID on Jobs(JobID)";
                        sqlCeCommand.ExecuteNonQuery();
                        sqlCeCommand.CommandText =
                            "create table LogItems(JobID nvarchar (40) not null, LogItemID nvarchar(40) unique not null, TimeStamp datetime, FinishedTime datetime, Status nvarchar (20), Operation nvarchar(50), ItemName nvarchar(150), Source nvarchar(500), Target nvarchar(500), Information ntext, Details ntext, SourceContent ntext, TargetContent ntext, LicensedDataUsed bigint)";
                        sqlCeCommand.ExecuteNonQuery();
                        sqlCeCommand.CommandText = "create unique index idxLogItemID on LogItems(LogItemID)";
                        sqlCeCommand.ExecuteNonQuery();
                        sqlCeCommand.CommandText = "create index idxParentJobID on LogItems(JobID)";
                        sqlCeCommand.ExecuteNonQuery();
                        sqlCeConnection.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Could not create job history database: ", exception.Message));
            }
        }

        public void DeleteJobs(string sJobIDs)
        {
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                string str = string.Concat("Delete From Jobs where JobID in (", sJobIDs, ")");
                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                {
                    sqlCeCommand.ExecuteNonQuery();
                }

                string str1 = string.Concat("Delete From LogItems where JobID in (", sJobIDs, ")");
                using (SqlCeCommand sqlCeCommand1 = new SqlCeCommand(str1, connection))
                {
                    sqlCeCommand1.ExecuteNonQuery();
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        public void DeleteLogItems(string sJobID)
        {
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                string str = string.Concat("Delete From LogItems where JobID in ('", sJobID, "')");
                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                {
                    sqlCeCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            this.Dispose(false);
        }

        private void Dispose(bool bDisposedByFinalizer)
        {
            this.Close();
            if (!bDisposedByFinalizer)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~JobHistorySqlCeDb()
        {
            this.Dispose(true);
        }

        public Job GetJob(string sJobID)
        {
            Job job;
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                using (DataTable dataTable = new DataTable())
                {
                    string str = string.Concat("Select * from Jobs where JobID = '", sJobID, "'");
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                    {
                        using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                        {
                            sqlCeDataAdapter.Fill(dataTable);
                        }

                        if (dataTable.Rows.Count != 0)
                        {
                            job = new Job(dataTable.Rows[0]);
                        }
                        else
                        {
                            job = null;
                        }
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }

            return job;
        }

        public List<Job> GetJobs(params string[] jobIds)
        {
            SqlCeConnection connection = null;
            List<Job> jobs = null;
            this._connectionLock.EnterReadLock();
            try
            {
                connection = this.Connection;
                string str = null;
                if ((int)jobIds.Length <= 0)
                {
                    str = string.Format("SELECT * FROM Jobs WHERE Edition = '{0}' OR Edition IS NULL ORDER BY Created",
                        JobHistorySqlCeDb._productName);
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string[] strArrays = jobIds;
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = strArrays[i];
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(" OR ");
                        }

                        stringBuilder.AppendFormat("JobID='{0}'", str1);
                    }

                    str = string.Format(
                        "SELECT * FROM Jobs WHERE ({0}) AND (Edition = '{1}' OR Edition IS NULL) ORDER BY Created",
                        stringBuilder.ToString(), JobHistorySqlCeDb._productName);
                }

                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                {
                    DataTable dataTable = new DataTable();
                    using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                    {
                        sqlCeDataAdapter.Fill(dataTable);
                    }

                    jobs = new List<Job>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        jobs.Add(new Job(row));
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }

            return jobs;
        }

        public LogItem GetLogItem(Job job_0, string sLogItemID)
        {
            LogItem logItem;
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                using (DataTable dataTable = new DataTable())
                {
                    using (SqlCeCommand sqlCeCommand =
                           new SqlCeCommand("Select * from LogItems where JobID = ? And LogItemID= ? ", connection))
                    {
                        sqlCeCommand.Parameters.Add(new SqlCeParameter("JobID", job_0.JobID));
                        sqlCeCommand.Parameters.Add(new SqlCeParameter("LogItemID", sLogItemID));
                        using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                        {
                            sqlCeDataAdapter.Fill(dataTable);
                        }

                        if (dataTable.Rows.Count != 0)
                        {
                            logItem = new LogItem(dataTable.Rows[0], job_0.JobHistoryDb);
                        }
                        else
                        {
                            logItem = null;
                        }
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }

            return logItem;
        }

        public void GetLogItemDetails(string sLogItemID, LogItem item)
        {
            string str;
            string str1;
            string str2;
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                using (DataTable dataTable = new DataTable())
                {
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(
                               "Select Details, SourceContent, TargetContent from LogItems Where LogItemID = ?",
                               connection))
                    {
                        sqlCeCommand.Parameters.Add(new SqlCeParameter("LogItemID", sLogItemID));
                        using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                        {
                            sqlCeDataAdapter.Fill(dataTable);
                        }

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
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        public List<LogItem> GetLogItems(Job job_0)
        {
            List<LogItem> logItems;
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                using (DataTable dataTable = new DataTable())
                {
                    string str =
                        string.Concat(
                            "Select JobID, LogItemID, Source, Target, Information, Operation, ItemName, Status, [TimeStamp], [FinishedTime], LicensedDataUsed from LogItems Where JobID In ('",
                            job_0.JobID, "') ORDER BY [TimeStamp]");
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                    {
                        using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                        {
                            sqlCeDataAdapter.Fill(dataTable);
                        }

                        List<LogItem> logItems1 = new List<LogItem>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            LogItem logItem = new LogItem(row, job_0.JobHistoryDb);
                            if (job_0.Action != null)
                            {
                                logItem.ActionLicensingUnit = job_0.Action.LicensingUnit;
                                logItem.ActionLicensingDescriptor = job_0.Action.LicensingDescriptor;
                            }

                            logItems1.Add(logItem);
                        }

                        logItems = logItems1;
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
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

        public void Open()
        {
            string str = string.Concat("DataSource=\"", this.AdapterContext, "\"; Max Database Size = 4090");
            if (!File.Exists(this.AdapterContext))
            {
                this.CreateDb(str);
            }

            this.TestConnection(str);
        }

        private void TestConnection(string connString)
        {
            this._connectionLock.EnterWriteLock();
            try
            {
                SqlCeConnection connection = SqlCeUtilities.GetConnection(connString, true);
                this.Connection = connection;
                using (DataTable dataTable = new DataTable())
                {
                    using (SqlCeCommand sqlCeCommand = new SqlCeCommand(
                               "Select Table_Name, Column_Name from Information_Schema.Columns where (((Table_Name = 'Jobs' or Table_Name = 'LogItems') and Column_Name = 'LicensedDataUsed') or (Table_Name = 'LogItems' and Column_Name = 'FinishedTime'))",
                               connection))
                    {
                        using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                        {
                            sqlCeDataAdapter.Fill(dataTable);
                        }

                        if (dataTable.Rows.Count != 3)
                        {
                            string str = "Alter Table Jobs Add Column LicensedDataUsed BIGINT";
                            string str1 =
                                "Alter Table LogItems Add\nColumn LicensedDataUsed BIGINT, \nColumn FinishedTime datetime\n";
                            using (SqlCeCommand sqlCeCommand1 =
                                   new SqlCeCommand(str, connection, connection.BeginTransaction()))
                            {
                                sqlCeCommand1.ExecuteNonQuery();
                                sqlCeCommand1.CommandText = str1;
                                sqlCeCommand1.ExecuteNonQuery();
                                sqlCeCommand1.Transaction.Commit();
                            }
                        }

                        sqlCeCommand.CommandText =
                            "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME = 'Jobs') AND (COLUMN_NAME = 'UserName' OR COLUMN_NAME = 'MachineName' OR COLUMN_NAME = 'CreatedBy')";
                        if (Convert.ToInt16(sqlCeCommand.ExecuteScalar()) == 0)
                        {
                            sqlCeCommand.CommandText =
                                "ALTER TABLE Jobs ADD COLUMN UserName nvarchar(100), COLUMN MachineName nvarchar(100), COLUMN CreatedBy nvarchar(100)";
                            sqlCeCommand.ExecuteNonQuery();
                        }

                        sqlCeCommand.CommandText =
                            "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME = 'Jobs') AND (COLUMN_NAME = 'Edition')";
                        if (Convert.ToInt16(sqlCeCommand.ExecuteScalar()) == 0)
                        {
                            sqlCeCommand.CommandText = "ALTER TABLE Jobs ADD COLUMN Edition nvarchar(256)";
                            sqlCeCommand.ExecuteNonQuery();
                        }

                        sqlCeCommand.CommandText =
                            "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LogItems' AND COLUMN_NAME = 'Target'";
                        if (Convert.ToInt16(sqlCeCommand.ExecuteScalar()) == 255)
                        {
                            sqlCeCommand.CommandText = "ALTER TABLE LogItems ALTER COLUMN Target nvarchar(500);";
                            sqlCeCommand.ExecuteNonQuery();
                        }

                        sqlCeCommand.CommandText =
                            "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LogItems' AND COLUMN_NAME = 'Source'";
                        if (Convert.ToInt16(sqlCeCommand.ExecuteScalar()) == 255)
                        {
                            sqlCeCommand.CommandText = "ALTER TABLE LogItems ALTER COLUMN Source nvarchar(500);";
                            sqlCeCommand.ExecuteNonQuery();
                        }

                        sqlCeCommand.CommandText =
                            "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LogItems' AND COLUMN_NAME = 'ItemName'";
                        if (Convert.ToInt16(sqlCeCommand.ExecuteScalar()) == 50)
                        {
                            sqlCeCommand.CommandText = "ALTER TABLE LogItems ALTER COLUMN ItemName nvarchar(150);";
                            sqlCeCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            finally
            {
                this._connectionLock.ExitWriteLock();
            }
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

            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                StringBuilder stringBuilder = new StringBuilder("Update Jobs Set ");
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
                    stringBuilder.Append("=?");
                }

                stringBuilder.Append(" where JobID=?");
                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(stringBuilder.ToString(), connection))
                {
                    this.AddJobParams(sParams, sqlCeCommand, job_0);
                    sqlCeCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        public void UpdateJobAction(Job[] jobs, string sActionXML)
        {
            this._connectionLock.EnterReadLock();
            try
            {
                SqlCeConnection connection = this.Connection;
                StringBuilder stringBuilder = new StringBuilder((int)jobs.Length * 2 - 1);
                Job[] jobArray = jobs;
                for (int i = 0; i < (int)jobArray.Length; i++)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append("?");
                }

                string str = string.Concat("Update Jobs Set [Action]=? where JobID in (", stringBuilder, ")");
                using (SqlCeCommand sqlCeCommand = new SqlCeCommand(str, connection))
                {
                    sqlCeCommand.Parameters.Add("@Action", OleDbType.LongVarWChar).Value = sActionXML;
                    int num = 0;
                    Job[] jobArray1 = jobs;
                    for (int j = 0; j < (int)jobArray1.Length; j++)
                    {
                        Job job = jobArray1[j];
                        string str1 = string.Concat("JobID", num.ToString());
                        sqlCeCommand.Parameters.AddWithValue(str1, job.JobID);
                        num++;
                    }

                    sqlCeCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem)
        {
            string[] strArrays = new string[]
            {
                "Details", "Source", "Target", "SourceContent", "TargetContent", "Information", "Operation", "ItemName",
                "Status", "[TimeStamp]", "[FinishedTime]", "LicensedDataUsed"
            };
            this.UpdateLogItem(sJobID, sLogItemID, logItem, strArrays);
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem, string[] sParams)
        {
            this._connectionLock.EnterReadLock();
            try
            {
                try
                {
                    if ((int)sParams.Length != 0)
                    {
                        SqlCeConnection connection = this.Connection;
                        StringBuilder stringBuilder = new StringBuilder("Update LogItems Set ");
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
                            stringBuilder.Append("=?");
                        }

                        stringBuilder.Append(" Where JobID=? And LogItemID=?");
                        using (SqlCeCommand sqlCeCommand = new SqlCeCommand(stringBuilder.ToString(), connection))
                        {
                            this.AddLogItemParams(sqlCeCommand, sJobID, logItem);
                            sqlCeCommand.ExecuteNonQuery();
                        }

                        logItem.ClearDetails();
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Logging.LogExceptionToTextFileWithEventLogBackup(
                        new Exception("Unable to update log item. This may be because your log file is full",
                            exception), "UpdateLogItem", true);
                }
            }
            finally
            {
                this._connectionLock.ExitReadLock();
            }
        }
    }
}