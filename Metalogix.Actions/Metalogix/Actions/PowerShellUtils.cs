using Metalogix;
using Metalogix.Actions.Remoting;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using Metalogix.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Metalogix.Actions
{
    public static class PowerShellUtils
    {
        private const string SnapinDeclaration =
            "if ( (Get-PSSnapin -Name {0} -ErrorAction SilentlyContinue) -eq $null ) {{ add-pssnapin {0} | out-null }}";

        private static bool? _powerShellInstalled;

        public static bool IsPowerShellInstalled
        {
            get
            {
                bool value = false;
                if (!PowerShellUtils._powerShellInstalled.HasValue)
                {
                    try
                    {
                        RegistryKey registryKey =
                            Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PowerShell\\1");
                        if (registryKey != null && Convert.ToUInt32(registryKey.GetValue("Install")) == 1)
                        {
                            PowerShellUtils._powerShellInstalled = new bool?(true);
                            value = true;
                        }
                    }
                    catch
                    {
                        PowerShellUtils._powerShellInstalled = new bool?(false);
                    }
                }
                else
                {
                    value = PowerShellUtils._powerShellInstalled.Value;
                }

                return value;
            }
        }

        public static void CreatePowerShellScript(Job[] jobs,
            Cryptography.ProtectionScope protectionScope = Cryptography.ProtectionScope.Certificate,
            string outputFilePath = null, X509Certificate2 certificate = null, bool isRemoteJob = false)
        {
            JobHistoryDb jobHistoryFromJobs = PowerShellUtils.GetJobHistoryFromJobs(jobs);
            string scriptFile = PowerShellUtils.GetScriptFile(outputFilePath);
            using (StreamWriter streamWriter = new StreamWriter(scriptFile))
            {
                streamWriter.WriteLine(Utils.PowerShellVersionCheck);
                List<string> list = new List<string>(new string[]
                {
                    "Metalogix.System.Commands"
                });
                list.AddRange((from job_0 in jobs
                    select job_0.Action).SelectMany((Action action_0) => action_0.RequiredSnapins));
                foreach (string current in list.Distinct(StringComparer.CurrentCultureIgnoreCase))
                {
                    streamWriter.WriteLine(
                        "if ( (Get-PSSnapin -Name {0} -ErrorAction SilentlyContinue) -eq $null ) {{ add-pssnapin {0} | out-null }}",
                        current);
                }

                streamWriter.WriteLine();
                for (int i = 0; i < jobs.Length; i++)
                {
                    Job job = jobs[i];
                    string jobID = string.Empty;
                    string text = job.SourceXml;
                    string text2 = job.TargetXml;
                    switch (protectionScope)
                    {
                        case Cryptography.ProtectionScope.LocalMachine:
                            text = PowerShellUtils.ReEncryptAsMachineScope(text);
                            text2 = PowerShellUtils.ReEncryptAsMachineScope(text2);
                            break;
                        case Cryptography.ProtectionScope.Certificate:
                            if (certificate != null)
                            {
                                text = PowerShellUtils.ReEncryptAsCertificateScope(text, certificate);
                                text2 = PowerShellUtils.ReEncryptAsCertificateScope(text2, certificate);
                            }

                            break;
                    }

                    if (isRemoteJob)
                    {
                        jobID = job.JobID;
                    }

                    string powershellCommand = job.Action.GetPowershellCommand(text, text2,
                        PowerShellUtils.GetJobAdapterPowerShellString(jobHistoryFromJobs, protectionScope, certificate),
                        jobID);
                    streamWriter.Write(powershellCommand);
                }

                streamWriter.Flush();
            }
        }

        public static string GetJobAdapterPowerShellString(IJobHistoryAdapter adapter,
            Cryptography.ProtectionScope protectionScope, X509Certificate2 certificate)
        {
            string adapterType = adapter.AdapterType;
            string str = adapterType;
            if (adapterType != null)
            {
                if (str == "Agent")
                {
                    return string.Concat(" -agentdatabase \"",
                        PowerShellUtils.GetSqlServerAdapterPowerShellString(adapter.AdapterContext, protectionScope,
                            certificate), "\"");
                }

                if (str == "SqlServer")
                {
                    return string.Concat(" -jobdatabase \"",
                        PowerShellUtils.GetSqlServerAdapterPowerShellString(adapter.AdapterContext, protectionScope,
                            certificate), "\"");
                }

                if (str == "SqlCe")
                {
                    return string.Concat(" -jobfile \"", adapter.AdapterContext, "\"");
                }
            }

            throw new ArgumentOutOfRangeException(string.Concat("'", adapter.AdapterType,
                "' is not supported. Please use 'Agent' or 'SqlServer' or 'SqlCe'."));
        }

        private static JobHistoryDb GetJobHistoryFromJobs(Job[] jobs)
        {
            if (jobs == null)
            {
                throw new ArgumentNullException("jobs");
            }

            if ((int)jobs.Length == 0)
            {
                throw new Exception("The jobs list is empty.");
            }

            JobHistoryDb jobHistoryDb = jobs[0].JobHistoryDb;
            if (jobs.Any<Job>((Job job_0) => job_0.JobHistoryDb != jobHistoryDb))
            {
                throw new Exception("The jobs must be of the same job history list.");
            }

            return jobHistoryDb;
        }

        private static string GetScriptFile(string scriptFile)
        {
            if (!string.IsNullOrEmpty(scriptFile))
            {
                return scriptFile;
            }

            string tempPath = Path.GetTempPath();
            if (tempPath[tempPath.Length - 1] != '\\')
            {
                tempPath = string.Concat(tempPath, "\\");
            }

            tempPath = string.Concat(tempPath, Guid.NewGuid(),
                (PowerShellUtils.IsPowerShellInstalled ? ".ps1" : ".txt"));
            return tempPath;
        }

        public static string GetSqlServerAdapterPowerShellString(string connectionString,
            Cryptography.ProtectionScope protectionScope, X509Certificate2 certificate)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (!sqlConnectionStringBuilder.IntegratedSecurity)
            {
                switch (protectionScope)
                {
                    case Cryptography.ProtectionScope.CurrentUser:
                    case Cryptography.ProtectionScope.LocalMachine:
                    {
                        sqlConnectionStringBuilder.Password =
                            Cryptography.EncryptTextusingAES(sqlConnectionStringBuilder.Password.ToSecureString());
                        break;
                    }
                    case Cryptography.ProtectionScope.Certificate:
                    {
                        sqlConnectionStringBuilder.Password =
                            Cryptography.EncryptText(sqlConnectionStringBuilder.Password.ToSecureString(), certificate);
                        break;
                    }
                }
            }

            return sqlConnectionStringBuilder.ConnectionString.Replace("\"", "");
        }

        public static bool IsBase64Encoded(string base64String)
        {
            bool flag;
            bool flag1;
            if (bool.TryParse(base64String, out flag))
            {
                return false;
            }

            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                flag1 = true;
            }
            catch (FormatException formatException)
            {
                flag1 = false;
            }

            return flag1;
        }

        public static string ReEncryptAsCertificateScope(string jobXml, X509Certificate2 certificate)
        {
            SecureString secureString;
            XDocument xDocument = XDocument.Parse(jobXml);
            foreach (XElement xElement in xDocument.Descendants("Connection"))
            {
                foreach (XAttribute xAttribute in
                         from attr in xElement.Attributes()
                         where PowerShellUtils.IsBase64Encoded(attr.Value)
                         select attr)
                {
                    if (!Cryptography.IsEncryptedUnderCurrentUserContext(xAttribute.Value, out secureString))
                    {
                        continue;
                    }

                    xAttribute.SetValue(Cryptography.EncryptText(secureString, certificate));
                }
            }

            return xDocument.ToString(SaveOptions.DisableFormatting);
        }

        public static string ReEncryptAsMachineScope(string jobXml)
        {
            SecureString secureString;
            XDocument xDocument = XDocument.Parse(jobXml);
            foreach (XElement xElement in xDocument.Descendants("Connection"))
            {
                foreach (XAttribute xAttribute in
                         from attr in xElement.Attributes()
                         where PowerShellUtils.IsBase64Encoded(attr.Value)
                         select attr)
                {
                    if (!Cryptography.IsEncryptedUnderCurrentUserContext(xAttribute.Value, out secureString))
                    {
                        continue;
                    }

                    xAttribute.SetValue(Cryptography.EncryptText(secureString,
                        Cryptography.ProtectionScope.LocalMachine, null));
                }
            }

            return xDocument.ToString(SaveOptions.DisableFormatting);
        }
    }
}