using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Metalogix.Jobs.Reporting.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Metalogix.Jobs.Reporting
{
    public static class ExcelReport
    {
        private static BackgroundWorker _backgroundWorker;

        private readonly static UInt32Value DefaultStyle;

        private readonly static UInt32Value DateTimeStyle;

        private readonly static UInt32Value TimeSpanStyle;

        static ExcelReport()
        {
            ExcelReport.DefaultStyle = 5;
            ExcelReport.DateTimeStyle = 13;
            ExcelReport.TimeSpanStyle = 9;
        }

        // Metalogix.Jobs.Reporting.ExcelReport
        private static void Create(string outputDirectoryPath, string outputFileName, bool overwrite, DataSource dataSource, string connectionString, IEnumerable<string> jobId, bool async)
        {
            if (async)
            {
                ExcelReport._backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = false,
                    WorkerReportsProgress = true
                };
                ExcelReport._backgroundWorker.DoWork += delegate (object sender, DoWorkEventArgs args)
                {
                    ExcelReport.BackGroundWorkerArguments backGroundWorkerArguments = (ExcelReport.BackGroundWorkerArguments)args.Argument;
                    ExcelReport.Create(backGroundWorkerArguments.OutputDirectoryPath, backGroundWorkerArguments.OutputFileName, backGroundWorkerArguments.Overwrite, backGroundWorkerArguments.DataSource, backGroundWorkerArguments.ConnectionString, backGroundWorkerArguments.JobId);
                };
                ExcelReport._backgroundWorker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs args)
                {
                    ExcelReport.OnProgressChanged(args.ProgressPercentage);
                };
                ExcelReport._backgroundWorker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs args)
                {
                    ExcelReport.OnExportComplete(args.Error);
                };
                ExcelReport._backgroundWorker.RunWorkerAsync(new ExcelReport.BackGroundWorkerArguments
                {
                    OutputDirectoryPath = outputDirectoryPath,
                    OutputFileName = outputFileName,
                    Overwrite = overwrite,
                    DataSource = dataSource,
                    ConnectionString = connectionString,
                    JobId = jobId
                });
                return;
            }
            ExcelReport.Create(outputDirectoryPath, outputFileName, overwrite, dataSource, connectionString, jobId);
            ExcelReport.OnExportComplete(null);
        }

        // Metalogix.Jobs.Reporting.ExcelReport
        private static void Create(string outputDirectoryPath, string outputFileName, bool overwrite, DataSource dataSource, string connectionString, IEnumerable<string> jobIds)
        {
            if (!outputFileName.EndsWith(".xlsx"))
            {
                throw new ArgumentException("Output file name must end in .xlsx");
            }
            if (string.IsNullOrEmpty(outputDirectoryPath))
            {
                outputDirectoryPath = Environment.CurrentDirectory;
            }
            string path = Path.Combine(outputDirectoryPath, outputFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            KeyValuePair<string, int> keyValuePair = ExcelReport.ExtractNonVersionedFilePath(fileNameWithoutExtension);
            string arg = Path.Combine(outputDirectoryPath, keyValuePair.Key);
            int num = keyValuePair.Value;
            if (!overwrite)
            {
                while (File.Exists(path))
                {
                    path = string.Format("{0}({1}){2}", arg, ++num, extension);
                }
            }
            using (Stream manifestResourceStream = Assembly.GetAssembly(typeof(ExcelReport)).GetManifestResourceStream("Metalogix.Jobs.Reporting.Templates.JobHistory.xlsx"))
            {
                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                File.WriteAllBytes(path, array);
            }
            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileStream, true))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    SharedStringTablePart sharedStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().Any<SharedStringTablePart>() ? workbookPart.GetPartsOfType<SharedStringTablePart>().First<SharedStringTablePart>() : workbookPart.AddNewPart<SharedStringTablePart>();
                    DbConnection dbConnection;
                    if (dataSource == DataSource.SqlServer)
                    {
                        dbConnection = new SqlConnection(connectionString);
                    }
                    else
                    {
                        dbConnection = new SqlCeConnection(connectionString);
                    }
                    using (dbConnection)
                    {
                        using (DbCommand dbCommand = dbConnection.CreateCommand())
                        {
                            dbConnection.Open();
                            ExcelReport.ProcessJobs(dbCommand, workbookPart, sharedStringPart, jobIds);
                            ExcelReport.ProcessLogItems(dbCommand, workbookPart, sharedStringPart, jobIds);
                        }
                    }
                    workbookPart.Workbook.Save();
                }
            }
        }

        public static void CreateFromSqlCeFile(string sqlCeDbFilePath, string outputDirectoryPath, string outputFileName = "JobHistory.xlsx", bool overwrite = false, IEnumerable<string> jobIds = null, bool async = false)
        {
            if (!File.Exists(sqlCeDbFilePath))
            {
                throw new ArgumentException("The SQL CE file path is not valid");
            }
            string str = string.Format("Data Source={0};Max Database Size=4091", sqlCeDbFilePath);
            ExcelReport.Create(outputDirectoryPath, outputFileName, overwrite, DataSource.SqlCe, str, jobIds, async);
        }

        public static void CreateFromSqlServer(string serverName, string databaseName, SqlAuthenticationType sqlAuthenticationType, string outputDirectoryPath, string outputFileName = "JobHistory.xlsx", bool overwrite = false, IEnumerable<string> jobIds = null, string userName = null, string password = null, bool async = false)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                throw new ArgumentException("Server Name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("Database Name cannot be null or empty");
            }
            if (sqlAuthenticationType == SqlAuthenticationType.SqlServer)
            {
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("Username cannot be null or empty if SQL Server authentication is being used");
                }
                if (password == null)
                {
                    throw new ArgumentException("Password cannot be null if SQL Server authentication is being used");
                }
            }
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = serverName,
                InitialCatalog = databaseName,
                IntegratedSecurity = sqlAuthenticationType == SqlAuthenticationType.Windows
            };
            SqlConnectionStringBuilder sqlConnectionStringBuilder1 = sqlConnectionStringBuilder;
            if (sqlAuthenticationType == SqlAuthenticationType.SqlServer)
            {
                if (userName != null)
                {
                    sqlConnectionStringBuilder1.UserID = userName;
                }
                if (password != null)
                {
                    sqlConnectionStringBuilder1.Password = password;
                }
            }
            ExcelReport.Create(outputDirectoryPath, outputFileName, overwrite, DataSource.SqlServer, sqlConnectionStringBuilder1.ConnectionString, jobIds, async);
        }

        public static void CreateFromSqlServer(string sqlServerConnectionString, string outputDirectoryPath, string outputFileName = "JobHistory.xlsx", bool overwrite = false, IEnumerable<string> jobIds = null, bool async = false)
        {
            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                throw new ArgumentException("Connection String cannot be null or empty");
            }
            ExcelReport.Create(outputDirectoryPath, outputFileName, overwrite, DataSource.SqlServer, sqlServerConnectionString, jobIds, async);
        }

        internal static DocumentFormat.OpenXml.Spreadsheet.Row CreateJobRow(Metalogix.Jobs.Reporting.Job job, UInt32Value rowIndex, SharedStringTablePart sharedStringTablePart)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row()
            {
                RowIndex = rowIndex
            };
            OpenXmlElement[] openXmlElementArrays = new OpenXmlElement[] { ExcelUtilities.CreateInlineStringCell(job.JobId, "A", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, job.Title, "B", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.Source, "C", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.Target, "D", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, job.Status, "E", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, job.StatusMessage, "F", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateDateCell(job.Created, "G", rowIndex, ExcelReport.DateTimeStyle), ExcelUtilities.CreateInlineStringCell(job.ResultsSummary, "H", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateNumberCell(job.LicensedDataUsed, "I", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateDateCell(job.Started, "J", rowIndex, ExcelReport.DateTimeStyle), ExcelUtilities.CreateDateCell(job.Finished, "K", rowIndex, ExcelReport.DateTimeStyle), ExcelUtilities.CreateDateCell(job.Duration, "L", rowIndex, ExcelReport.TimeSpanStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, job.Action, "M", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.SourceXml, "N", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.TargetXml, "O", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.UserName, "P", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.MachineName, "Q", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(job.CreatedBy, "R", rowIndex, ExcelReport.DefaultStyle) };
            row.Append(openXmlElementArrays);
            return row;
        }

        internal static DocumentFormat.OpenXml.Spreadsheet.Row CreateLogItemRow(LogItem logItem, UInt32Value rowIndex, SharedStringTablePart sharedStringTablePart)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row()
            {
                RowIndex = rowIndex
            };
            OpenXmlElement[] openXmlElementArrays = new OpenXmlElement[] { ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, logItem.JobId, "A", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.LogItemId, "B", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateDateCell(logItem.TimeStamp, "C", rowIndex, ExcelReport.DateTimeStyle), ExcelUtilities.CreateNumberCell((long)logItem.Month, "D", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateNumberCell((long)logItem.Day, "E", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateNumberCell((long)logItem.Hour, "F", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateNumberCell((long)logItem.Minute, "G", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateDateCell(logItem.FinishedTime, "H", rowIndex, ExcelReport.DateTimeStyle), ExcelUtilities.CreateDateCell(logItem.Duration, "I", rowIndex, ExcelReport.TimeSpanStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, logItem.Status, "J", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateSharedStringCell(sharedStringTablePart, logItem.Operation, "K", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.ItemName, "L", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.Source, "M", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.Target, "N", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.Information, "O", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.Details, "P", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.SourceContent, "Q", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateInlineStringCell(logItem.TargetContent, "R", rowIndex, ExcelReport.DefaultStyle), ExcelUtilities.CreateNumberCell(logItem.LicensedDataUsed, "S", rowIndex, ExcelReport.DefaultStyle) };
            row.Append(openXmlElementArrays);
            return row;
        }

        private static KeyValuePair<string, int> ExtractNonVersionedFilePath(string filePath)
        {
            int num = 1;
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File Path cannot be null or empty");
            }
            int num1 = filePath.LastIndexOf('(');
            if (!filePath.TrimEnd(new char[0]).EndsWith(")") || num1 < 0)
            {
                return new KeyValuePair<string, int>(filePath, num);
            }
            int length = filePath.Length - num1 - 2;
            if (int.TryParse(filePath.Substring(num1, length), out num))
            {
                return new KeyValuePair<string, int>(filePath, num);
            }
            return new KeyValuePair<string, int>(filePath.Substring(0, length), num);
        }

        public static void OnExportComplete(Exception exception)
        {
            if (ExcelReport.ExportComplete != null)
            {
                ExcelReport.ExportComplete(exception);
            }
        }

        public static void OnProgressChanged(int percentageComplete)
        {
            if (ExcelReport.ProgressChanged != null)
            {
                ExcelReport.ProgressChanged(percentageComplete);
            }
        }

        private static void ProcessJobs(DbCommand command, WorkbookPart workbookPart, SharedStringTablePart sharedStringPart, IEnumerable<string> jobIds = null)
        {
            bool flag = command is SqlCeCommand;
            string str = (flag ? "[Jobs]" : "[JobHistory]");
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First<Sheet>((Sheet s) => s.Name == "Jobs");
            Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
            SheetData firstChild = worksheet.GetFirstChild<SheetData>();
            string str1 = string.Concat("SELECT [JobID] ,[Title] ,[Source] ,[Target] ,[Status] ,[StatusMessage] ,[Created] ,[ResultsSummary] ,[LicensedDataUsed] ,[Started] ,[Finished] ,[Action] ,[SourceXml] ,[TargetXml], [UserName], [MachineName], [CreatedBy] FROM ", str);
            if (jobIds != null)
            {
                StringBuilder stringBuilder = new StringBuilder(" WHERE [JobID] IN (");
                foreach (string str2 in
                    from jId in jobIds
                    where !string.IsNullOrEmpty(jId)
                    select jId)
                {
                    stringBuilder.AppendFormat("'{0}',", str2);
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append(")");
                str1 = string.Concat(str1, stringBuilder.ToString());
            }
            command.CommandText = str1;
            UInt32Value uInt32Value = 1;
            using (DbDataReader dbDataReaders = command.ExecuteReader())
            {
                while (dbDataReaders.Read())
                {
                    uInt32Value++;
                    DateTime localTime = dbDataReaders.SafeGetValue<DateTime>(9);
                    DateTime dateTime = dbDataReaders.SafeGetValue<DateTime>(10);
                    DateTime localTime1 = dbDataReaders.SafeGetValue<DateTime>(6);
                    if (!flag)
                    {
                        localTime = localTime.ToLocalTime();
                        dateTime = dateTime.ToLocalTime();
                        localTime1 = localTime1.ToLocalTime();
                    }
                    Metalogix.Jobs.Reporting.Job job = new Metalogix.Jobs.Reporting.Job()
                    {
                        JobId = dbDataReaders.SafeGetValue<string>(0),
                        Title = dbDataReaders.SafeGetValue<string>(1),
                        Source = dbDataReaders.SafeGetValue<string>(2),
                        Target = dbDataReaders.SafeGetValue<string>(3),
                        Status = dbDataReaders.SafeGetValue<string>(4),
                        StatusMessage = dbDataReaders.SafeGetValue<string>(5),
                        ResultsSummary = dbDataReaders.SafeGetValue<string>(7),
                        LicensedDataUsed = dbDataReaders.SafeGetValue<long>(8),
                        Started = localTime,
                        Finished = dateTime,
                        Created = localTime1,
                        Action = dbDataReaders.SafeGetValue<string>(11),
                        SourceXml = dbDataReaders.SafeGetValue<string>(12),
                        TargetXml = dbDataReaders.SafeGetValue<string>(13),
                        UserName = dbDataReaders.SafeGetValue<string>(14),
                        MachineName = dbDataReaders.SafeGetValue<string>(15),
                        CreatedBy = dbDataReaders.SafeGetValue<string>(16)
                    };
                    DocumentFormat.OpenXml.Spreadsheet.Row row = ExcelReport.CreateJobRow(job, uInt32Value, sharedStringPart);
                    firstChild.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(row);
                }
            }
            worksheet.Save();
        }

        private static void ProcessLogItems(DbCommand command, WorkbookPart workbookPart, SharedStringTablePart sharedStringPart, IEnumerable<string> jobIds = null)
        {
            bool flag = command is SqlCeCommand;
            string str = (flag ? "[LogItems]" : "[JobLogItems]");
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().First<Sheet>((Sheet s) => s.Name == "LogItems");
            Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
            SheetData firstChild = worksheet.GetFirstChild<SheetData>();
            string str1 = string.Concat("SELECT COUNT(*) FROM ", str);
            string str2 = string.Concat("SELECT [JobID],[LogItemID],[TimeStamp],[FinishedTime],[Status],[Operation],[ItemName],[Source],[Target],[Information],[Details],[SourceContent] ,[TargetContent] ,[LicensedDataUsed] FROM ", str);
            if (jobIds != null)
            {
                StringBuilder stringBuilder = new StringBuilder(" WHERE [JobID] IN (");
                foreach (string str3 in
                    from jId in jobIds
                    where !string.IsNullOrEmpty(jId)
                    select jId)
                {
                    stringBuilder.AppendFormat("'{0}',", str3);
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append(")");
                str2 = string.Concat(str2, stringBuilder.ToString());
                str1 = string.Concat(str1, stringBuilder.ToString());
            }
            command.CommandText = str1;
            int num = (int)command.ExecuteScalar();
            command.CommandText = str2;
            UInt32Value uInt32Value = 1;
            using (DbDataReader dbDataReaders = command.ExecuteReader())
            {
                while (dbDataReaders.Read())
                {
                    uInt32Value++;
                    DateTime localTime = dbDataReaders.SafeGetValue<DateTime>(2);
                    DateTime dateTime = dbDataReaders.SafeGetValue<DateTime>(3);
                    if (!flag)
                    {
                        localTime = localTime.ToLocalTime();
                        dateTime = dateTime.ToLocalTime();
                    }
                    LogItem logItem = new LogItem()
                    {
                        JobId = dbDataReaders.SafeGetValue<string>(0),
                        LogItemId = dbDataReaders.SafeGetValue<string>(1),
                        TimeStamp = localTime,
                        FinishedTime = dateTime,
                        Status = dbDataReaders.SafeGetValue<string>(4),
                        Operation = dbDataReaders.SafeGetValue<string>(5),
                        ItemName = dbDataReaders.SafeGetValue<string>(6),
                        Source = dbDataReaders.SafeGetValue<string>(7),
                        Target = dbDataReaders.SafeGetValue<string>(8),
                        Information = dbDataReaders.SafeGetValue<string>(9),
                        Details = dbDataReaders.SafeGetValue<string>(10),
                        SourceContent = dbDataReaders.SafeGetValue<string>(11),
                        TargetContent = dbDataReaders.SafeGetValue<string>(12),
                        LicensedDataUsed = dbDataReaders.SafeGetValue<long>(13)
                    };
                    DocumentFormat.OpenXml.Spreadsheet.Row row = ExcelReport.CreateLogItemRow(logItem, uInt32Value, sharedStringPart);
                    firstChild.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(row);
                    if (ExcelReport._backgroundWorker == null || ExcelReport.ProgressChanged == null)
                    {
                        continue;
                    }
                    int num1 = (int)((float)((float)uInt32Value) / (float)num * 100f);
                    ExcelReport._backgroundWorker.ReportProgress(num1);
                }
            }
            ExcelReport.OnProgressChanged(100);
            worksheet.Save();
        }

        public static event Action<Exception> ExportComplete;

        public static event ExcelReport.ProgressChangedEventHandler ProgressChanged;

        private struct BackGroundWorkerArguments
        {
            public string OutputDirectoryPath;

            public string OutputFileName;

            public bool Overwrite;

            public DataSource DataSource;

            public string ConnectionString;

            public IEnumerable<string> JobId;
        }

        public delegate void ProgressChangedEventHandler(int percentageComplete);
    }
}