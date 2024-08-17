using Metalogix;
using Metalogix.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Metalogix.Core
{
    public static class Logging
    {
        private const string GLOBAL_LOGGING_MUTEX_NAME = "Global\\MetalogixGlobalLoggingMutex";

        public const string GLOBAL_LOG_FILENAME = "MetalogixGlobalLog.txt";

        private const string DEFAULT_EVENT_HEADING = "[Exception]";

        private const string EVENT_SOURCE_NAME = "Metalogix Global Logging";

        private const string EVENT_LOG_TO_USE = "Application";

        private const string DATETIME_FORMAT = "yyyy-MM-dd HH.mm.ss.ffff";

        private const int MUTEX_TIMEOUT_MS = 60000;

        public static void CreateDirectory(string path)
        {
            bool flag;
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path is null or empty");
            }

            DirectoryInfo directoryInfo = Directory.CreateDirectory(path);
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
            FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier,
                FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly, AccessControlType.Allow);
            accessControl.ModifyAccessRule(AccessControlModification.Add, fileSystemAccessRule, out flag);
            directoryInfo.SetAccessControl(accessControl);
        }

        public static void LogExceptionToEventLog(Exception exception, string information)
        {
            try
            {
                string str = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff");
                StringBuilder stringBuilder = new StringBuilder(1024);
                stringBuilder.AppendLine(string.Format("{0}: User={1}, Domain={2}", str, Environment.UserName,
                    Environment.UserDomainName));
                stringBuilder.AppendLine(string.Format("{0}: {1}", str, information ?? "[Exception]"));
                ExceptionUtils.GetExceptionMessage(exception, stringBuilder);
                Logging.LogMessageToEventLog(stringBuilder.ToString(), EventLogEntryType.Error,
                    LogEventTypeId.Exception);
            }
            catch (Exception exception1)
            {
            }
        }

        public static void LogExceptionToTextFileWithEventLogBackup(Exception exception, string information,
            bool logMessageInEventLog = true)
        {
            bool flag = false;
            StringBuilder stringBuilder = null;
            try
            {
                string str = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff");
                stringBuilder = new StringBuilder(1024);
                stringBuilder.AppendLine(
                    "-----------------------------------------------------------------------------------------------------------");
                stringBuilder.AppendLine(string.Format("{0}: User={1}, Domain={2}", str, Environment.UserName,
                    Environment.UserDomainName));
                stringBuilder.AppendLine(string.Format("{0}: {1}", str, information));
                ExceptionUtils.GetExceptionMessage(exception, stringBuilder);
                string str1 = Path.Combine(ApplicationData.CommonDataPath, "MetalogixGlobalLog.txt");
                using (Mutex mutex = new Mutex(false, "Global\\MetalogixGlobalLoggingMutex", out flag))
                {
                    try
                    {
                        if (!mutex.WaitOne(60000))
                        {
                            throw new TimeoutException("Metalogix Global Mutex WaitOne");
                        }

                        if (!Directory.Exists(ApplicationData.CommonDataPath))
                        {
                            Logging.CreateDirectory(ApplicationData.CommonDataPath);
                        }

                        File.AppendAllText(str1, stringBuilder.ToString());
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            catch (Exception exception2)
            {
                Exception exception1 = exception2;
                if (!logMessageInEventLog)
                {
                    Trace.WriteLine(string.Format("Error occurred while logging message. Error '{0}'",
                        exception1.ToString()));
                }
                else
                {
                    if (stringBuilder != null)
                    {
                        Logging.LogMessageToEventLog(stringBuilder.ToString(), EventLogEntryType.Error,
                            LogEventTypeId.Exception);
                    }

                    Logging.LogExceptionToEventLog(exception1,
                        "Logging to global log file - LogExceptionToTextFileWithEventLogBackup");
                }
            }
        }

        public static void LogMessageToEventLog(string message, EventLogEntryType eventLogEntryType,
            LogEventTypeId eventId)
        {
            try
            {
                if (!EventLog.SourceExists("Metalogix Global Logging"))
                {
                    EventLog.CreateEventSource("Metalogix Global Logging", "Application");
                }

                EventLog.WriteEntry("Metalogix Global Logging", message, eventLogEntryType, (int)eventId);
            }
            catch (Exception exception)
            {
            }
        }

        public static void LogMessageToMetalogixGlobalLogFile(string information)
        {
            Logging.LogExceptionToTextFileWithEventLogBackup(null, information, false);
        }
    }
}