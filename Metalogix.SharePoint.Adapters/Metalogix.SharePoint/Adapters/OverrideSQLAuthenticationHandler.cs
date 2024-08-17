using Metalogix;
using Metalogix.Utilities;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class OverrideSQLAuthenticationHandler
    {
        private const string C_GLOBAL_LOGDB_ERROR_MUTEX = "Global\\MetalogixLogDBErrorMutex";

        private bool? _enabled;

        private readonly static object _enabledLock;

        private SecureString _user;

        private SecureString _passWord;

        public string CommonPath { get; private set; }

        public bool Enabled
        {
            get
            {
                if (!this._enabled.HasValue)
                {
                    lock (OverrideSQLAuthenticationHandler._enabledLock)
                    {
                        if (!this._enabled.HasValue)
                        {
                            bool flag = File.Exists(string.Concat(this.CommonPath, this.OverrideFilename));
                            if (flag)
                            {
                                flag = this.PopulateValues();
                            }

                            this._enabled = new bool?(flag);
                        }
                    }
                }

                return this._enabled.Value;
            }
        }

        public string ErrorFilename { get; private set; }

        public int MaxPoolSize { get; private set; }

        public string OverrideFilename { get; private set; }

        public string Password
        {
            get
            {
                if (this._passWord == null)
                {
                    return string.Empty;
                }

                return this._passWord.ToInsecureString();
            }
        }

        public int Timeout { get; private set; }

        public string Username
        {
            get
            {
                if (this._user == null)
                {
                    return string.Empty;
                }

                return this._user.ToInsecureString();
            }
        }

        static OverrideSQLAuthenticationHandler()
        {
            OverrideSQLAuthenticationHandler._enabledLock = new object();
        }

        public OverrideSQLAuthenticationHandler()
        {
            this.CommonPath = string.Format("{0}\\Metalogix\\",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            this.OverrideFilename = "DBOverride.xml";
            this.ErrorFilename = "DBOverrideErrorLog.txt";
            this._user = null;
            this._passWord = null;
            this._enabled = null;
            this.Timeout = 60;
            this.MaxPoolSize = 100;
        }

        public string ConstructOverriddenConnectionString(string originalConnectionString)
        {
            string connectionString = null;
            if (this.Enabled)
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder =
                    new SqlConnectionStringBuilder(originalConnectionString);
                SqlConnectionStringBuilder sqlConnectionStringBuilder1 = new SqlConnectionStringBuilder()
                {
                    UserID = this.Username,
                    Password = this.Password,
                    DataSource = sqlConnectionStringBuilder.DataSource,
                    InitialCatalog = sqlConnectionStringBuilder.InitialCatalog,
                    ConnectTimeout = this.Timeout,
                    MaxPoolSize = this.MaxPoolSize,
                    ApplicationName = typeof(OverrideSQLAuthenticationHandler).Name
                };
                connectionString = sqlConnectionStringBuilder1.ConnectionString;
            }

            return connectionString;
        }

        private void CreateDirectory(string path)
        {
            bool flag;
            DirectoryInfo directoryInfo = Directory.CreateDirectory(path);
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
            FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier,
                FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly, AccessControlType.Allow);
            accessControl.ModifyAccessRule(AccessControlModification.Add, fileSystemAccessRule, out flag);
            directoryInfo.SetAccessControl(accessControl);
        }

        public bool IsConnectable(string connectionString)
        {
            bool flag;
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                this.LogDBError(exception);
                flag = false;
            }

            return flag;
        }

        private void LogDBError(Exception exception)
        {
            bool flag = false;
            try
            {
                if (!Directory.Exists(this.CommonPath))
                {
                    this.CreateDirectory(this.CommonPath);
                }

                StringBuilder stringBuilder = new StringBuilder();
                Utils.GetExceptionMessage(exception, ref stringBuilder);
                using (Mutex mutex = new Mutex(false, "Global\\MetalogixLogDBErrorMutex", out flag))
                {
                    try
                    {
                        mutex.WaitOne();
                        using (StreamWriter streamWriter =
                               new StreamWriter(string.Concat(this.CommonPath, this.ErrorFilename), false))
                        {
                            streamWriter.WriteLine("{0} Log Start Date", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss"));
                            streamWriter.WriteLine();
                            streamWriter.Write(stringBuilder.ToString());
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            catch (Exception exception1)
            {
            }
        }

        private bool PopulateValues()
        {
            bool flag = false;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(string.Concat(this.CommonPath, this.OverrideFilename));
                if (xmlDocument.DocumentElement.Attributes["Username"] != null &&
                    xmlDocument.DocumentElement.Attributes["Password"] != null)
                {
                    this._user = Cryptography.DecryptText(xmlDocument.DocumentElement.Attributes["Username"].Value,
                        Cryptography.ProtectionScope.LocalMachine, null);
                    this._passWord = Cryptography.DecryptText(xmlDocument.DocumentElement.Attributes["Password"].Value,
                        Cryptography.ProtectionScope.LocalMachine, null);
                    if (xmlDocument.DocumentElement.Attributes["Timeout"] != null)
                    {
                        int num = 0;
                        if (int.TryParse(xmlDocument.DocumentElement.Attributes["Timeout"].Value, out num))
                        {
                            this.Timeout = num;
                        }
                    }

                    if (xmlDocument.DocumentElement.Attributes["MaxPoolSize"] != null)
                    {
                        int num1 = 0;
                        if (int.TryParse(xmlDocument.DocumentElement.Attributes["MaxPoolSize"].Value, out num1))
                        {
                            this.MaxPoolSize = num1;
                        }
                    }

                    flag = true;
                }
            }
            catch (Exception exception)
            {
                this.LogDBError(exception);
            }

            return flag;
        }
    }
}