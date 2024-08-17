using System;
using System.IO;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointExtensionsLogger
    {
        private SharePointAdapter m_target;

        private static TextWriter s_logFile;

        private static TextWriter LogFile
        {
            get
            {
                if (SharePointExtensionsLogger.s_logFile == null)
                {
                    string adapterLogFileLocation = SharePointAdapter.AdapterLogFileLocation;
                    DirectoryInfo directoryInfo = new DirectoryInfo(adapterLogFileLocation);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    object[] str = new object[] { "AdapterLog_", null, null, null, null };
                    str[1] = DateTime.Now.ToString("yyyyMMdd");
                    str[2] = "_";
                    str[3] = Guid.NewGuid();
                    str[4] = ".txt";
                    string str1 = string.Concat(str);
                    SharePointExtensionsLogger.s_logFile =
                        new StreamWriter(string.Concat(adapterLogFileLocation, "\\", str1));
                }

                return SharePointExtensionsLogger.s_logFile;
            }
        }

        protected SharePointAdapter Target
        {
            get { return this.m_target; }
            set { this.m_target = value; }
        }

        static SharePointExtensionsLogger()
        {
        }

        public SharePointExtensionsLogger()
        {
        }

        protected object ExecuteMethod(string sMethodName, object[] parameters)
        {
            object obj;
            DateTime now = DateTime.Now;
            int num = 0;
            if (parameters != null)
            {
                object[] objArray = parameters;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    num += SharePointExtensionsLogger.SizeOf(objArray[i]);
                }
            }

            try
            {
                MethodInfo method = this.m_target.GetType().GetMethod(sMethodName);
                object obj1 = method.Invoke(this.m_target, parameters);
                num += SharePointExtensionsLogger.SizeOf(obj1);
                TimeSpan timeSpan = DateTime.Now.Subtract(now);
                object[] adapterShortName = new object[]
                {
                    this.m_target.AdapterShortName, "\t", sMethodName, "\t", this.m_target.ServerDisplayName, "\t",
                    now.ToString(), "\t", timeSpan.TotalMilliseconds, "\t", num
                };
                SharePointExtensionsLogger.WriteLine(string.Concat(adapterShortName));
                obj = obj1;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string[] strArrays = new string[]
                {
                    this.m_target.AdapterShortName, "\t", sMethodName, "\t", this.m_target.ServerDisplayName, "\t",
                    now.ToString(), "\t\t\t", exception.Message
                };
                SharePointExtensionsLogger.WriteLine(string.Concat(strArrays));
                throw SharePointExtensionsLogger.GetExecutedMethodException(exception);
            }

            return obj;
        }

        private static Exception GetExecutedMethodException(Exception ex)
        {
            if (!(ex is TargetInvocationException) || ex.InnerException == null)
            {
                return ex;
            }

            return ex.InnerException;
        }

        private static int SizeOf(object variable)
        {
            if (variable == null)
            {
                return 0;
            }

            if (variable.GetType() == typeof(string))
            {
                return ((string)variable).Length;
            }

            if (!variable.GetType().IsAssignableFrom(typeof(byte[])))
            {
                return 0;
            }

            return (int)((byte[])variable).Length;
        }

        private static void WriteLine(string sLine)
        {
            SharePointExtensionsLogger.LogFile.WriteLine(sLine);
            SharePointExtensionsLogger.LogFile.Flush();
        }
    }
}