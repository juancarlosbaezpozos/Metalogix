using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace Metalogix.SharePoint.Database
{
    public static class DatabaseRestorationManager
    {
        private readonly static string TEMPORARY_DATABASE_PREFIX;

        static DatabaseRestorationManager()
        {
            DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX = "ML_TEMP_DB_";
        }

        public static bool BakFilesAreConsistent(DatabaseBackup firstBackup, DatabaseBackup secondBackup)
        {
            return ((firstBackup.SourceDatabaseName != secondBackup.SourceDatabaseName ? true : !(firstBackup.SourceServerName == secondBackup.SourceServerName)) ? false : true);
        }

        private static bool BakFilesAreConsistent(Dictionary<string, List<DatabaseBackup>> backupsOrderedByFile, out string sError)
        {
            bool flag;
            sError = null;
            if (backupsOrderedByFile.Count != 0)
            {
                DatabaseBackup item = null;
                foreach (string key in backupsOrderedByFile.Keys)
                {
                    List<DatabaseBackup> databaseBackups = backupsOrderedByFile[key];
                    if (databaseBackups.Count > 0)
                    {
                        if (item == null)
                        {
                            item = databaseBackups[0];
                        }
                        if (!DatabaseRestorationManager.BakFilesAreConsistent(item, databaseBackups[0]))
                        {
                            object[] sourceFile = new object[] { item.SourceFile, databaseBackups[0].SourceFile, item.SourceServerName, item.SourceDatabaseName, databaseBackups[0].SourceServerName, databaseBackups[0].SourceDatabaseName };
                            sError = string.Format("The backup files '{0}' and '{1}' are inconsistent because they do not share the same source server and database. It is not possible to connect to the backups in this situation.\n\nThe backup '{0}' has server = '{2}' and database = '{3}', while the backup '{1}' has server = '{4}' and database = '{5}'.", sourceFile);
                            flag = false;
                            return flag;
                        }
                    }
                }
                flag = true;
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        private static RestoredDatabaseData ConnectToMdfDatabase(string sSqlServer, string sBackupFilePath, string sLogFilePath, Credentials creds, DatabaseRestorationDialogs dialogs)
        {
            object obj = null;
            RestoredDatabaseData restoredDatabaseDatum;
            object[] objArray;
            RestoredDatabaseData restoredDatabaseDatum1 = null;
            string str = "CREATE DATABASE {0} ON ( NAME = {0}, FILENAME = '{1}' ) {2} FOR ATTACH{3}";
            string str1 = "LOG ON ( NAME = {0}, FILENAME = '{1}' )";
            FileInfo fileInfo = new FileInfo(sBackupFilePath);
            string fileNameFromPath = Utils.GetFileNameFromPath(sBackupFilePath, false);
            string str2 = DatabaseRestorationManager.GenerateTempDatabaseName(fileNameFromPath, fileInfo.LastWriteTime);
            str2 = str2.Replace("-", "");
            bool flag = sLogFilePath == null;
            bool flag1 = true;
            if (dialogs != null)
            {
                ShowDialogPromptAction connectToMdfDatabaseDialog = dialogs.ConnectToMdfDatabaseDialog;
                objArray = new object[] { sBackupFilePath, sSqlServer, str2, flag };
                flag1 = connectToMdfDatabaseDialog(out obj, objArray);
            }
            if (flag1)
            {
                string str3 = "";
                string str4 = "";
                if (flag)
                {
                    str4 = "_REBUILD_LOG";
                }
                else
                {
                    str3 = string.Format(str1, string.Concat(str2, "_LOG"), sLogFilePath);
                }
                objArray = new object[] { str2, sBackupFilePath, str3, str4 };
                string str5 = string.Format(str, objArray);
                SqlConnection sQLConnection = DatabaseRestorationManager.GetSQLConnection(sSqlServer, creds);
                try
                {
                    SqlDataReader sqlDataReader = null;
                    try
                    {
                        try
                        {
                            SqlCommand sqlCommand = new SqlCommand(str5, sQLConnection);
                            sQLConnection.Open();
                            sqlCommand.ExecuteNonQuery();
                            sQLConnection.ChangeDatabase(str2);
                            SqlCommand sqlCommand1 = new SqlCommand("sp_helpfilegroup", sQLConnection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            sqlCommand1.Parameters.Add(new SqlParameter("@filegroupname", "PRIMARY"));
                            sqlDataReader = sqlCommand1.ExecuteReader();
                            sqlDataReader.NextResult();
                            string item = "";
                            while (sqlDataReader.Read())
                            {
                                if (fileNameFromPath == Utils.GetFileNameFromPath((string)sqlDataReader["filename"], false))
                                {
                                    item = (string)sqlDataReader["file_in_group"];
                                    break;
                                }
                            }
                            sqlDataReader.Close();
                            restoredDatabaseDatum1 = new RestoredDatabaseData()
                            {
                                Credentials = creds,
                                DatabaseName = str2,
                                ServerName = sSqlServer,
                                SourceDatabaseName = item,
                                SourceServerName = "",
                                BackupType = RestoredBackupType.Mdf
                            };
                        }
                        catch (Exception exception1)
                        {
                            Exception exception = exception1;
                            string str6 = string.Format("Could not get SQL Server '{0}' to attach database file '{1}'. Error: {2}", sSqlServer, sBackupFilePath, exception.Message);
                            throw new Exception(str6, exception);
                        }
                    }
                    finally
                    {
                        if ((sqlDataReader == null ? false : !sqlDataReader.IsClosed))
                        {
                            sqlDataReader.Close();
                        }
                        sQLConnection.Close();
                    }
                }
                finally
                {
                    if (sQLConnection != null)
                    {
                        ((IDisposable)sQLConnection).Dispose();
                    }
                }
                restoredDatabaseDatum = restoredDatabaseDatum1;
            }
            else
            {
                restoredDatabaseDatum = restoredDatabaseDatum1;
            }
            return restoredDatabaseDatum;
        }

        private static SqlConnectionStringBuilder CreateSqlConnection(string sqlServer, string userName, string password)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder["Data Source"] = sqlServer;
            if (!string.IsNullOrEmpty(userName))
            {
                sqlConnectionStringBuilder.UserID = userName;
                sqlConnectionStringBuilder.Password = password;
            }
            else
            {
                sqlConnectionStringBuilder["integrated Security"] = true;
            }
            sqlConnectionStringBuilder.AsynchronousProcessing = true;
            sqlConnectionStringBuilder.InitialCatalog = "master";
            return sqlConnectionStringBuilder;
        }

        private static SqlConnectionStringBuilder CreateSqlConnection(RestoreParameters parameters)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            string str = parameters.sSqlServer;
            string str1 = parameters.sUserId;
            string str2 = parameters.sPassword;
            bool flag = parameters.bSqlAuthentication;
            sqlConnectionStringBuilder["Data Source"] = str;
            if (str1 != null)
            {
                if (flag)
                {
                    sqlConnectionStringBuilder.NetworkLibrary = "DBMSSOCN";
                }
                sqlConnectionStringBuilder.UserID = str1;
                sqlConnectionStringBuilder.Password = str2;
            }
            else
            {
                sqlConnectionStringBuilder["integrated Security"] = true;
            }
            sqlConnectionStringBuilder.AsynchronousProcessing = true;
            sqlConnectionStringBuilder.InitialCatalog = "master";
            return sqlConnectionStringBuilder;
        }

        public static bool DeleteDatabase(string sSqlServer, string sDatabaseName)
        {
            return DatabaseRestorationManager.DeleteDatabase(sSqlServer, sDatabaseName, new Credentials());
        }

        public static bool DeleteDatabase(string sSqlServer, string sDatabaseName, Credentials creds)
        {
            bool flag;
            try
            {
                SqlConnection sQLConnection = DatabaseRestorationManager.GetSQLConnection(sSqlServer, creds);
                try
                {
                    sQLConnection.Open();
                    (new SqlCommand("USE Master", sQLConnection)).ExecuteNonQuery();
                    string str = string.Format("IF EXISTS (SELECT * FROM sys.databases where name = '{0}') DROP DATABASE [{0}]", sDatabaseName);
                    (new SqlCommand(str, sQLConnection)).ExecuteNonQuery();
                    sQLConnection.Close();
                    flag = true;
                }
                finally
                {
                    if (sQLConnection != null)
                    {
                        ((IDisposable)sQLConnection).Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Failed to delete database: ", exception.Message));
            }
            return flag;
        }

        private static List<DatabaseBackup> DetermineNecessaryBackupsForRestore(List<DatabaseBackup> backups, DatabaseBackup chosenBackup)
        {
            object[] name;
            List<DatabaseBackup> databaseBackups = new List<DatabaseBackup>();
            DatabaseBackup.SortBackups(backups, DatabaseBackup.BackupSortingOrder.EarliestToMostRecent);
            int num = backups.IndexOf(chosenBackup);
            if (num < 0)
            {
                name = new object[] { chosenBackup.Name, chosenBackup.SourceDatabaseName, chosenBackup.SourceServerName, chosenBackup.SourceFile };
                throw new DatabaseRestorationManager.BackupIsNotInSetException(string.Format("The chosen backup '{0}' from source database '{1}' on server '{2}' does not appear to be in the backup file(s) chosen. Please contact support@metalogix.net for assistance in this issue. [Source filename='{3}']", name));
            }
            if (chosenBackup.Type != DatabaseBackup.BackupType.Full)
            {
                int num1 = num;
                while (true)
                {
                    if ((num1 < 0 ? true : backups[num1].Type != DatabaseBackup.BackupType.TransactionLog))
                    {
                        break;
                    }
                    databaseBackups.Add(backups[num1]);
                    num1--;
                }
                if (num1 < 0)
                {
                    throw new DatabaseRestorationManager.NoFullBackupException("Differential and transaction log backups need to be based off of a full backup. No full backup that is earlier than the chosen backup could be found in the chosen backup files. Make sure that all of the Sharepoint database backup files have been included.");
                }
                if (backups[num1].Type != DatabaseBackup.BackupType.Full)
                {
                    if (backups[num1].Type != DatabaseBackup.BackupType.Differential)
                    {
                        name = new object[] { chosenBackup.Name, chosenBackup.SourceDatabaseName, chosenBackup.SourceServerName, chosenBackup.SourceFile };
                        throw new DatabaseRestorationManager.UnknownBackupTypeException(string.Format("An unknown backup type was encountered while trying to find a full or differential backup during restoration. The name of the backup is '{0}' and it has source database '{1}' on server '{2}'. [Source filename='{3}']", name));
                    }
                    databaseBackups.Add(backups[num1]);
                    int num2 = num1 - 1;
                    while (true)
                    {
                        if ((num2 < 0 ? true : backups[num2].Type == DatabaseBackup.BackupType.Full))
                        {
                            break;
                        }
                        num2--;
                    }
                    if (num2 < 0)
                    {
                        throw new DatabaseRestorationManager.NoFullBackupException("Differential and transaction log backups need to be based off of a full backup. No full backup that is earlier than the chosen differential backup could be found in your chosen backup files. Make sure that all of the Sharepoint database backup files have been included.");
                    }
                    databaseBackups.Add(backups[num2]);
                }
                else
                {
                    databaseBackups.Add(backups[num1]);
                }
            }
            else
            {
                databaseBackups.Add(chosenBackup);
            }
            DatabaseBackup.SortBackups(databaseBackups, DatabaseBackup.BackupSortingOrder.EarliestToMostRecent);
            return databaseBackups;
        }

        private static bool DoesLogFileExist(string sBackupFilePath, out string sLogFilePath)
        {
            sLogFilePath = string.Concat(sBackupFilePath.Remove(sBackupFilePath.Length - 4), "_log.ldf");
            return (new FileInfo(sLogFilePath)).Exists;
        }

        private static string GeneratePointInTimeString(DateTime pointInTime)
        {
            string str = pointInTime.ToString("G").Replace("/", "").Replace(" ", "").Replace(":", "").Replace("_", "");
            return str;
        }

        private static string GenerateTempDatabaseName(string sDatabaseName, DateTime pointInTime)
        {
            Guid guid = Guid.NewGuid();
            string[] tEMPORARYDATABASEPREFIX = new string[] { DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX, sDatabaseName, "_", DatabaseRestorationManager.GeneratePointInTimeString(pointInTime), "_", guid.ToString() };
            return string.Concat(tEMPORARYDATABASEPREFIX);
        }

        public static List<DatabaseBackup> GetDatabaseBackupsFromFile(string sSqlServer, string sBackupFilePath, Credentials creds)
        {
            List<DatabaseBackup> databaseBackups;
            try
            {
                SqlConnection sQLConnection = DatabaseRestorationManager.GetSQLConnection(sSqlServer, creds);
                try
                {
                    SqlDataReader sqlDataReader = null;
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand(string.Concat("RESTORE HEADERONLY FROM DISK='", sBackupFilePath, "'"), sQLConnection);
                        sQLConnection.Open();
                        sqlDataReader = sqlCommand.ExecuteReader();
                        List<DatabaseBackup> databaseBackups1 = new List<DatabaseBackup>();
                        while (sqlDataReader.Read())
                        {
                            DatabaseBackup databaseBackup = new DatabaseBackup()
                            {
                                Name = sqlDataReader["BackupName"].ToString(),
                                Description = sqlDataReader["BackupDescription"].ToString(),
                                Position = int.Parse(sqlDataReader["Position"].ToString()),
                                StartDate = (DateTime)sqlDataReader["BackupStartDate"],
                                FinishDate = (DateTime)sqlDataReader["BackupFinishDate"],
                                SourceFile = sBackupFilePath,
                                SourceDatabaseName = sqlDataReader["DatabaseName"].ToString(),
                                SourceServerName = sqlDataReader["ServerName"].ToString()
                            };
                            string str = sqlDataReader["BackupType"].ToString();
                            if (str != null)
                            {
                                if (str == "1")
                                {
                                    databaseBackup.Type = DatabaseBackup.BackupType.Full;
                                    goto Label0;
                                }
                                else if (str == "2")
                                {
                                    databaseBackup.Type = DatabaseBackup.BackupType.TransactionLog;
                                    goto Label0;
                                }
                                else
                                {
                                    if (str != "5")
                                    {
                                        goto Label2;
                                    }
                                    databaseBackup.Type = DatabaseBackup.BackupType.Differential;
                                    goto Label0;
                                }
                            }
                            Label2:
                            databaseBackup.Type = DatabaseBackup.BackupType.Unknown;
                            Label0:
                            databaseBackups1.Add(databaseBackup);
                        }
                        databaseBackups = databaseBackups1;
                    }
                    finally
                    {
                        if ((sqlDataReader == null ? false : !sqlDataReader.IsClosed))
                        {
                            sqlDataReader.Close();
                        }
                        sQLConnection.Close();
                    }
                }
                finally
                {
                    if (sQLConnection != null)
                    {
                        ((IDisposable)sQLConnection).Dispose();
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (!exception.Message.Contains("Cannot open backup device"))
                {
                    throw exception;
                }
                string[] strArrays = new string[] { "SQL server \"", sSqlServer, "\" cannot access backup file \"", sBackupFilePath, "\" either because the file does not exist or the server does not have access permissions to the file." };
                throw new Exception(string.Concat(strArrays), exception);
            }
            return databaseBackups;
        }

        private static List<DatabaseBackup> GetDatabaseBackupVersions(string sSqlServer, string sBackupFilePath)
        {
            return DatabaseRestorationManager.GetDatabaseBackupsFromFile(sSqlServer, sBackupFilePath, new Credentials());
        }

        private static List<DatabaseBackup> GetDatabaseBackupVersions(string sSqlServer, List<string> backupFilePaths, Credentials creds, DatabaseRestorationDialogs dialogs)
        {
            Dictionary<string, List<DatabaseBackup>> dictionary = new Dictionary<string, List<DatabaseBackup>>();
            foreach (string current in backupFilePaths)
            {
                dictionary.Add(current, DatabaseRestorationManager.GetDatabaseBackupsFromFile(sSqlServer, current, creds));
            }
            List<DatabaseBackup> list = new List<DatabaseBackup>();
            if (backupFilePaths.Count > 1)
            {
                string message = "";
                if (!DatabaseRestorationManager.BakFilesAreConsistent(dictionary, out message))
                {
                    throw new Exception(message);
                }
                foreach (string current in dictionary.Keys)
                {
                    list.AddRange(dictionary[current]);
                }
            }
            else
            {
                list = DatabaseRestorationManager.GetDatabaseBackupsFromFile(sSqlServer, backupFilePaths[0], creds);
            }
            DatabaseBackup chosenBackup;
            List<DatabaseBackup> result;
            if (dialogs == null)
            {
                DatabaseBackup.SortBackups(list, DatabaseBackup.BackupSortingOrder.MostRecentToEarliest);
                chosenBackup = list[0];
            }
            else
            {
                object obj;
                if (!dialogs.GetDatabaseBackupVersionsDialog(out obj, new object[]
                {
                    sSqlServer,
                    creds,
                    list
                }))
                {
                    result = null;
                    return result;
                }
                chosenBackup = (DatabaseBackup)obj;
            }
            List<DatabaseBackup> list2 = DatabaseRestorationManager.DetermineNecessaryBackupsForRestore(list, chosenBackup);
            result = list2;
            return result;
        }

        private static string GetDatabaseNameFromFile(string sSqlServer, string sBackupFilePath, Credentials creds)
        {
            string str;
            SqlConnection sQLConnection = DatabaseRestorationManager.GetSQLConnection(sSqlServer, creds);
            try
            {
                SqlDataReader sqlDataReader = null;
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(string.Concat("RESTORE FILELISTONLY FROM DISK='", sBackupFilePath, "'"), sQLConnection);
                    sQLConnection.Open();
                    sqlDataReader = sqlCommand.ExecuteReader();
                    string str1 = null;
                    while (sqlDataReader.Read())
                    {
                        if (sqlDataReader["FileGroupName"].ToString() == "PRIMARY")
                        {
                            str1 = sqlDataReader["LogicalName"].ToString();
                            break;
                        }
                    }
                    str = str1;
                }
                finally
                {
                    if ((sqlDataReader == null ? false : !sqlDataReader.IsClosed))
                    {
                        sqlDataReader.Close();
                    }
                    sQLConnection.Close();
                }
            }
            finally
            {
                if (sQLConnection != null)
                {
                    ((IDisposable)sQLConnection).Dispose();
                }
            }
            return str;
        }

        private static List<string> GetDatabases(string sSqlServer, Credentials creds)
        {
            List<string> strs;
            SqlConnection sQLConnection = DatabaseRestorationManager.GetSQLConnection(sSqlServer, creds);
            try
            {
                SqlDataReader sqlDataReader = null;
                try
                {
                    SqlCommand sqlCommand = new SqlCommand("sp_databases", sQLConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    List<string> strs1 = new List<string>();
                    sQLConnection.Open();
                    sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        strs1.Add(sqlDataReader["DATABASE_NAME"].ToString());
                    }
                    strs = strs1;
                }
                finally
                {
                    if ((sqlDataReader == null ? false : !sqlDataReader.IsClosed))
                    {
                        sqlDataReader.Close();
                    }
                    sQLConnection.Close();
                }
            }
            finally
            {
                if (sQLConnection != null)
                {
                    ((IDisposable)sQLConnection).Dispose();
                }
            }
            return strs;
        }

        private static string GetExistingTempDatabase(string sSqlServer, string sBackupFilePath, DateTime pointInTime, Credentials creds)
        {
            string str;
            string str1;
            string str2;
            string str3;
            try
            {
                string databaseNameFromFile = DatabaseRestorationManager.GetDatabaseNameFromFile(sSqlServer, sBackupFilePath, creds);
                string str4 = DatabaseRestorationManager.GeneratePointInTimeString(pointInTime);
                foreach (string databasis in DatabaseRestorationManager.GetDatabases(sSqlServer, creds))
                {
                    if ((!databasis.StartsWith(DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX) ? false : !databasis.EndsWith("_")))
                    {
                        if (databasis.StartsWith(string.Concat(DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX, databaseNameFromFile)))
                        {
                            try
                            {
                                DatabaseRestorationManager.SplitTempDatabaseName(databasis, out str, out str2, out str1);
                                if (str4 == str2)
                                {
                                    str3 = databasis;
                                    return str3;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                str3 = null;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                message = message.Replace("Operating system error 5(error not found).\r\nRESTORE FILELIST is terminating abnormally.", string.Concat("The account under which the selected MSSQLServer is running does not have sufficient privileges on the device file to be restored.\n To restore this backup, please grant the MSSQLServer account rights on ", sBackupFilePath, "."));
                throw new Exception(string.Concat("Could not browse backup file: ", message));
            }
            return str3;
        }

        public static string GetPhysicalFileName(string sFileType, string sDatabaseLogFileLocation, string sDatabaseDataFileLocation, string sNewDatabaseName, string sLogicalName, bool bUseRedGate)
        {
            string str = (sFileType == "L" ? sDatabaseLogFileLocation : sDatabaseDataFileLocation);
            string str1 = (sFileType == "L" ? string.Concat((bUseRedGate ? "V" : ""), "LDF") : string.Concat((bUseRedGate ? "V" : ""), "MDF"));
            string[] strArrays = new string[] { str, "\\", sNewDatabaseName, sLogicalName, ".", str1 };
            return string.Concat(strArrays);
        }

        public static void GetRestorationFileInformation(SqlConnectionStringBuilder builder, DatabaseBackup dbBackup, List<FileDescription> fileList, ref string databaseName)
        {
            SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
            try
            {
                SqlDataReader sqlDataReader = null;
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(string.Concat("RESTORE FILELISTONLY FROM DISK='", dbBackup.SourceFile, "' WITH FILE=", dbBackup.Position.ToString()), sqlConnection)
                    {
                        CommandTimeout = 0
                    };
                    sqlConnection.Open();
                    IAsyncResult asyncResult = sqlCommand.BeginExecuteReader();
                    while (!asyncResult.IsCompleted)
                    {
                    }
                    sqlDataReader = sqlCommand.EndExecuteReader(asyncResult);
                    while (sqlDataReader.Read())
                    {
                        string str = sqlDataReader["PhysicalName"].ToString();
                        string str1 = str.Substring(0, str.LastIndexOf("\\") + 1);
                        string str2 = str.Substring(str.LastIndexOf("\\") + 1);
                        long? item = (long?)sqlDataReader["Size"];
                        string str3 = string.Concat(str1, databaseName, str2);
                        FileDescription fileDescription = new FileDescription(sqlDataReader["LogicalName"].ToString(), str3, item, sqlDataReader["Type"].ToString());
                        if (!fileList.Contains(fileDescription))
                        {
                            fileList.Add(fileDescription);
                        }
                        if (sqlDataReader["FileGroupName"].ToString() == "PRIMARY")
                        {
                            databaseName = sqlDataReader["LogicalName"].ToString();
                        }
                    }
                }
                finally
                {
                    if ((sqlDataReader == null ? false : !sqlDataReader.IsClosed))
                    {
                        sqlDataReader.Close();
                    }
                    sqlConnection.Close();
                }
            }
            finally
            {
                if (sqlConnection != null)
                {
                    ((IDisposable)sqlConnection).Dispose();
                }
            }
        }

        private static SqlConnection GetSQLConnection(string sSqlServer, Credentials creds)
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

        public static void InitializeFileLocations(SqlConnectionStringBuilder builder, ref string dataFileLocation, ref string logFileLocation, DatabaseRestorationDialogs dialogs)
        {
            object obj = null;
            SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
            try
            {
                SqlCommand sqlCommand = new SqlCommand("declare @SmoDefaultFile nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'DefaultData', @SmoDefaultFile OUTPUT\ndeclare @SmoDefaultLog nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'DefaultLog', @SmoDefaultLog OUTPUT\ndeclare @SmoSetupPath nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Setup', N'SQLPath', @SmoSetupPath OUTPUT\nSELECT @SmoDefaultFile AS [DefaultFile], @SmoDefaultLog  AS [DefaultLog], @SmoSetupPath AS [SetupPath]\n", sqlConnection);
                try
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    try
                    {
                        DataTable dataTable = new DataTable("FileLocations");
                        sqlDataAdapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 0)
                        {
                            if (!dialogs.InitializeFileLocationsDialog(out obj, new object[0]))
                            {
                                throw new ArgumentException("Could not restore backup: Could not determine default locaiton for SQL file location. ");
                            }
                        }
                        dataFileLocation = dataTable.Rows[0]["DefaultFile"].ToString();
                        logFileLocation = dataTable.Rows[0]["DefaultLog"].ToString();
                        string str = string.Concat(dataTable.Rows[0]["SetupPath"].ToString(), "\\DATA");
                        if ((dataFileLocation == null ? true : dataFileLocation == string.Empty))
                        {
                            dataFileLocation = str;
                        }
                        if ((logFileLocation == null ? true : logFileLocation == string.Empty))
                        {
                            logFileLocation = str;
                        }
                    }
                    finally
                    {
                        if (sqlDataAdapter != null)
                        {
                            ((IDisposable)sqlDataAdapter).Dispose();
                        }
                    }
                }
                finally
                {
                    if (sqlCommand != null)
                    {
                        ((IDisposable)sqlCommand).Dispose();
                    }
                }
            }
            finally
            {
                if (sqlConnection != null)
                {
                    ((IDisposable)sqlConnection).Dispose();
                }
            }
        }

        private static bool MdfFileIsReadOnly(string sMdfFilePath)
        {
            return (new FileInfo(sMdfFilePath)).IsReadOnly;
        }

        public static RestoredDatabaseData Open(string sSqlServer, string sBackupFilePath, DatabaseRestorationDialogs dialogs)
        {
            string[] strArrays = new string[] { sBackupFilePath };
            return DatabaseRestorationManager.Open(sSqlServer, strArrays, new Credentials(), dialogs);
        }

        public static RestoredDatabaseData Open(string sSqlServer, string[] backupFilePaths, Credentials creds, DatabaseRestorationDialogs dialogs)
        {
            RestoredDatabaseData restoredDatabaseDatum = null;
            restoredDatabaseDatum = (((int)backupFilePaths.Length != 1 ? true : !backupFilePaths[0].EndsWith(".mdf")) ? DatabaseRestorationManager.RestoreDatabaseFromBak(sSqlServer, new List<string>(backupFilePaths), creds, dialogs) : DatabaseRestorationManager.RestoreDatabaseFromMdf(sSqlServer, backupFilePaths[0], creds, dialogs));
            return restoredDatabaseDatum;
        }

        public static void RestoreDatabase(SqlCommand restoreCommand, SqlCommand finalRestoreCommand, object oParameters, DatabaseBackup dbBackup, bool bLastFile, SqlInfoMessageEventHandler messageHandler, Semaphore semaCancelLock, ref bool isCancelling, out string[] restoredFilesList)
        {
            DatabaseRestorationManager.RestoreDatabase(DatabaseRestorationManager.CreateSqlConnection((RestoreParameters)oParameters), restoreCommand, finalRestoreCommand, oParameters, dbBackup, bLastFile, messageHandler, semaCancelLock, ref isCancelling, out restoredFilesList);
        }

        public static void RestoreDatabase(SqlConnectionStringBuilder builder, SqlCommand restoreCommand, SqlCommand finalRestoreCommand, object oParameters, DatabaseBackup dbBackup, bool bLastFile, SqlInfoMessageEventHandler messageHandler, Semaphore semaCancelLock, ref bool isCancelling, out string[] restoredFilesList)
        {
            Exception exception;
            if (dbBackup.Type == DatabaseBackup.BackupType.Unknown)
            {
                throw new Exception(string.Concat("Unknown backup type for backup: ", dbBackup.Name));
            }
            RestoreParameters restoreParameter = (RestoreParameters)oParameters;
            string str = restoreParameter.sNewDatabaseName;
            DataTable dataTable = new DataTable("FileList");
            SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(string.Concat("RESTORE FILELISTONLY FROM DISK='", dbBackup.SourceFile, "' WITH FILE=", dbBackup.Position.ToString()), sqlConnection)
                {
                    CommandTimeout = AdapterConfigurationVariables.SQLQueryTimeoutTime
                };
                (new SqlDataAdapter(sqlCommand)).Fill(dataTable);
            }
            finally
            {
                if (sqlConnection != null)
                {
                    ((IDisposable)sqlConnection).Dispose();
                }
            }
            List<string> strs = new List<string>();
            string[] sourceFile = new string[] { "RESTORE ", null, null, null, null, null, null, null, null };
            sourceFile[1] = (dbBackup.Type == DatabaseBackup.BackupType.TransactionLog ? "LOG" : "DATABASE");
            sourceFile[2] = " [";
            sourceFile[3] = str;
            sourceFile[4] = "] FROM DISK='";
            sourceFile[5] = dbBackup.SourceFile;
            sourceFile[6] = "' WITH FILE=";
            sourceFile[7] = dbBackup.Position.ToString();
            sourceFile[8] = ",";
            string str1 = string.Concat(sourceFile);
            str1 = ((!bLastFile ? true : dbBackup.Type == DatabaseBackup.BackupType.TransactionLog) ? string.Concat(str1, " NORECOVERY,") : string.Concat(str1, " RECOVERY,"));
            foreach (DataRow row in dataTable.Rows)
            {
                if (!str1.EndsWith(","))
                {
                    str1 = string.Concat(str1, ",");
                }
                string str2 = row["LogicalName"].ToString();
                string physicalFileName = DatabaseRestorationManager.GetPhysicalFileName(row["Type"].ToString(), restoreParameter.sDatabaseLogFileLocation, restoreParameter.sDatabaseDataFileLocation, str, str2, restoreParameter.UseRedGate);
                strs.Add(physicalFileName);
                string str3 = str1;
                sourceFile = new string[] { str3, " MOVE '", str2, "' TO '", physicalFileName, "'" };
                str1 = string.Concat(sourceFile);
            }
            str1 = string.Concat(str1, ", STATS=1, REPLACE");
            restoredFilesList = strs.ToArray();
            sqlConnection = new SqlConnection(builder.ConnectionString);
            try
            {
                restoreCommand = new SqlCommand(str1, sqlConnection)
                {
                    CommandTimeout = 0
                };
                sqlConnection.InfoMessage += messageHandler;
                sqlConnection.Open();
                semaCancelLock.WaitOne();
                IAsyncResult asyncResult = restoreCommand.BeginExecuteReader();
                try
                {
                    semaCancelLock.Release();
                    while (!asyncResult.IsCompleted)
                    {
                    }
                }
                finally
                {
                    try
                    {
                        restoreCommand.EndExecuteReader(asyncResult);
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        if (!isCancelling)
                        {
                            throw exception;
                        }
                    }
                }
            }
            finally
            {
                if (sqlConnection != null)
                {
                    ((IDisposable)sqlConnection).Dispose();
                }
            }
            if ((dbBackup.Type != DatabaseBackup.BackupType.TransactionLog ? false : bLastFile))
            {
                sqlConnection = new SqlConnection(builder.ConnectionString);
                try
                {
                    finalRestoreCommand = new SqlCommand(string.Concat("RESTORE DATABASE [", str, "] WITH RECOVERY"), sqlConnection)
                    {
                        CommandTimeout = 0
                    };
                    sqlConnection.Open();
                    semaCancelLock.WaitOne();
                    IAsyncResult asyncResult1 = finalRestoreCommand.BeginExecuteReader();
                    try
                    {
                        semaCancelLock.Release();
                        while (!asyncResult1.IsCompleted)
                        {
                        }
                    }
                    finally
                    {
                        try
                        {
                            finalRestoreCommand.EndExecuteReader(asyncResult1);
                        }
                        catch (Exception exception2)
                        {
                            exception = exception2;
                            if (!isCancelling)
                            {
                                throw exception;
                            }
                        }
                    }
                }
                finally
                {
                    if (sqlConnection != null)
                    {
                        ((IDisposable)sqlConnection).Dispose();
                    }
                }
            }
        }

        private static RestoredDatabaseData RestoreDatabaseFromBak(string sSqlServer, List<string> backupFilePaths, Credentials creds, DatabaseRestorationDialogs dialogs)
        {
            RestoredDatabaseData restoredDatabaseDatum;
            bool item;
            object obj = null;
            RestoredDatabaseData restoredDatabaseDatum1;
            string userName;
            List<DatabaseBackup> databaseBackupVersions = null;
            databaseBackupVersions = DatabaseRestorationManager.GetDatabaseBackupVersions(sSqlServer, backupFilePaths, creds, dialogs);
            if ((databaseBackupVersions == null ? false : databaseBackupVersions.Count > 0))
            {
                DatabaseBackup databaseBackup = databaseBackupVersions[databaseBackupVersions.Count - 1];
                string sourceServerName = databaseBackup.SourceServerName;
                string sourceDatabaseName = databaseBackup.SourceDatabaseName;
                DateTime startDate = databaseBackup.StartDate;
                string existingTempDatabase = DatabaseRestorationManager.GetExistingTempDatabase(sSqlServer, backupFilePaths[0], startDate, creds);
                if (existingTempDatabase == null)
                {
                    string empty = string.Empty;
                    string str = string.Empty;
                    string str1 = DatabaseRestorationManager.GenerateTempDatabaseName(DatabaseRestorationManager.GetDatabaseNameFromFile(sSqlServer, backupFilePaths[0], creds), startDate);
                    if (dialogs == null)
                    {
                        item = false;
                        string str2 = sSqlServer;
                        if (creds.IsDefault)
                        {
                            userName = null;
                        }
                        else
                        {
                            userName = creds.UserName;
                        }
                        SqlConnectionStringBuilder sqlConnectionStringBuilder = DatabaseRestorationManager.CreateSqlConnection(str2, userName, creds.Password.ToInsecureString());
                        DatabaseRestorationManager.InitializeFileLocations(sqlConnectionStringBuilder, ref empty, ref str, dialogs);
                        foreach (DatabaseBackup databaseBackupVersion in databaseBackupVersions)
                        {
                            DatabaseRestorationManager.GetRestorationFileInformation(sqlConnectionStringBuilder, databaseBackupVersion, new List<FileDescription>(), ref str1);
                        }
                    }
                    else
                    {
                        ShowDialogPromptAction restoreDatabaseFromBakDialog = dialogs.RestoreDatabaseFromBakDialog;
                        object[] objArray = new object[] { sSqlServer, str1, databaseBackupVersions, creds };
                        if (!restoreDatabaseFromBakDialog(out obj, objArray))
                        {
                            restoredDatabaseDatum1 = null;
                            return restoredDatabaseDatum1;
                        }
                        Dictionary<string, object> strs = (Dictionary<string, object>)obj;
                        empty = (string)strs["DataFileLocation"];
                        str = (string)strs["LogFileLocation"];
                        item = (bool)strs["UseRedGate"];
                    }
                    if (DatabaseRestorationManager.RunRestore(sSqlServer, str1, empty, str, databaseBackupVersions, creds, dialogs, item))
                    {
                        restoredDatabaseDatum = new RestoredDatabaseData()
                        {
                            ServerName = sSqlServer,
                            DatabaseName = str1,
                            Credentials = creds,
                            SourceServerName = sourceServerName,
                            SourceDatabaseName = sourceDatabaseName,
                            BackupType = RestoredBackupType.Bak
                        };
                        restoredDatabaseDatum1 = restoredDatabaseDatum;
                        return restoredDatabaseDatum1;
                    }
                }
                else
                {
                    restoredDatabaseDatum = new RestoredDatabaseData()
                    {
                        ServerName = sSqlServer,
                        DatabaseName = existingTempDatabase,
                        Credentials = creds,
                        SourceServerName = sourceServerName,
                        SourceDatabaseName = sourceDatabaseName,
                        BackupType = RestoredBackupType.Bak
                    };
                    restoredDatabaseDatum1 = restoredDatabaseDatum;
                    return restoredDatabaseDatum1;
                }
            }
            restoredDatabaseDatum1 = null;
            return restoredDatabaseDatum1;
        }

        private static RestoredDatabaseData RestoreDatabaseFromMdf(string sSqlServer, string sBackupFilePath, Credentials creds, DatabaseRestorationDialogs dialogs)
        {
            object obj = null;
            object[] objArray;
            bool flag;
            RestoredDatabaseData mdfDatabase = null;
            string str = null;
            if (DatabaseRestorationManager.DoesLogFileExist(sBackupFilePath, out str))
            {
                mdfDatabase = DatabaseRestorationManager.ConnectToMdfDatabase(sSqlServer, sBackupFilePath, str, creds, dialogs);
            }
            else if (!DatabaseRestorationManager.MdfFileIsReadOnly(sBackupFilePath))
            {
                if (dialogs == null)
                {
                    flag = false;
                }
                else
                {
                    ShowDialogPromptAction restoreDatabaseFromMdfDialog = dialogs.RestoreDatabaseFromMdfDialog;
                    objArray = new object[] { sBackupFilePath, str };
                    flag = !restoreDatabaseFromMdfDialog(out obj, objArray);
                }
                if (!flag)
                {
                    mdfDatabase = DatabaseRestorationManager.ConnectToMdfDatabase(sSqlServer, sBackupFilePath, null, creds, dialogs);
                }
            }
            else
            {
                string[] strArrays = new string[] { "The database file '", sBackupFilePath, "' is read-only and the expected logfile could not be found. The expected logfile location is '", str, "'.\n\nWithout a logfile, the only way to connect to the database file is to create a new logfile. Creating a new logfile requires write access to the database file. For further information about this process, please contact support@metalogix.net." };
                string str1 = string.Concat(strArrays);
                if (dialogs == null)
                {
                    throw new FileLoadException(str1);
                }
                ShowDialogPromptAction showDialogPromptAction = dialogs.RestoreDatabaseFromMdfDialog;
                objArray = new object[] { str1 };
                showDialogPromptAction(out obj, objArray);
            }
            return mdfDatabase;
        }

        public static bool RunRestore(string sSqlServer, string sNewDatabaseName, string sDataFileLocation, string sLogFileLocation, List<DatabaseBackup> dbBackups, Credentials creds, DatabaseRestorationDialogs dialogs, bool bUseRedGate)
        {
            string[] strArrays;
            object obj = null;
            bool flag;
            string userName;
            string insecureString;
            try
            {
                if (dialogs != null)
                {
                    ShowDialogPromptAction runRestoreDialog = dialogs.RunRestoreDialog;
                    object[] objArray = new object[] { sSqlServer, sNewDatabaseName, sDataFileLocation, sLogFileLocation, dbBackups, creds, bUseRedGate };
                    if (!runRestoreDialog(out obj, objArray))
                    {
                        DatabaseRestorationManager.DeleteDatabase(sSqlServer, sNewDatabaseName, creds);
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    RestoreParameters restoreParameter = new RestoreParameters()
                    {
                        bSqlAuthentication = false,
                        databaseBackups = dbBackups,
                        sDatabaseDataFileLocation = sDataFileLocation,
                        sDatabaseLogFileLocation = sLogFileLocation,
                        sNewDatabaseName = sNewDatabaseName,
                        UseRedGate = bUseRedGate
                    };
                    if (creds.IsDefault)
                    {
                        userName = null;
                    }
                    else
                    {
                        userName = creds.UserName;
                    }
                    restoreParameter.sUserId = userName;
                    if (creds.IsDefault)
                    {
                        insecureString = null;
                    }
                    else
                    {
                        insecureString = creds.Password.ToInsecureString();
                    }
                    restoreParameter.sPassword = insecureString;
                    SqlConnectionStringBuilder sqlConnectionStringBuilder = DatabaseRestorationManager.CreateSqlConnection(restoreParameter);
                    int num = 1;
                    Semaphore semaphore = new Semaphore(1, 1);
                    foreach (DatabaseBackup dbBackup in dbBackups)
                    {
                        bool flag1 = false;
                        DatabaseRestorationManager.RestoreDatabase(sqlConnectionStringBuilder, new SqlCommand(), new SqlCommand(), restoreParameter, dbBackup, num == restoreParameter.databaseBackups.Count, null, semaphore, ref flag1, out strArrays);
                    }
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Failed to restore database: ", exception.Message));
            }
            return flag;
        }

        private static void SplitTempDatabaseName(string sTempDatabase, out string sDatabaseName, out string sPointInTime, out string sGuid)
        {
            int num = sTempDatabase.LastIndexOf("_");
            int num1 = sTempDatabase.Substring(0, num).LastIndexOf("_");
            sDatabaseName = sTempDatabase.Substring(DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX.Length, num1 - DatabaseRestorationManager.TEMPORARY_DATABASE_PREFIX.Length - 1);
            sPointInTime = sTempDatabase.Substring(num1 + 1, num - num1 - 1);
            sGuid = sTempDatabase.Substring(num + 1);
        }

        public class BackupIsNotInSetException : Exception
        {
            public BackupIsNotInSetException(string sMessage) : base(sMessage)
            {
            }
        }

        public class NoFullBackupException : Exception
        {
            public NoFullBackupException(string sMessage) : base(sMessage)
            {
            }
        }

        public class UnknownBackupTypeException : Exception
        {
            public UnknownBackupTypeException(string sMessage) : base(sMessage)
            {
            }
        }
    }
}