using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting.MappingFiles;
using Metalogix.Core;
using Metalogix.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalogix.Actions.Remoting
{
    public class RemoteWorker : IRemoteWorker
    {
        private const string ProgramDataFolderPathCommand = "$env:ALLUSERSPROFILE";

        private const string OSInformationPathCommand = "(Get-WmiObject -class Win32_OperatingSystem).Caption";

        private const string ApplicationDataFolderPathCommand = "$env:APPDATA";

        private const string ProductConsole = "Content Matrix Console";

        private const string TestPowerScriptFileName = "TestRemotePowerShell.ps1";

        private const string TestPowerScriptCommand = "Test-WSMan";

        private readonly object _sync = new object();

        private readonly string _programDataPowerScriptFilePath =
            string.Concat(Utils.MetalogixTempPath, "ProgramDataRemotePowerShell.ps1");

        private readonly string OSInformationPowerScriptFilePath =
            string.Concat(Utils.MetalogixTempPath, "OSInformationRemotePowerShell.ps1");

        private readonly string _applicationDataPowerShellScriptFilePath =
            string.Concat(Utils.MetalogixTempPath, "ApplicationMappingsRemotePowerShell.ps1");

        private readonly string _tempScriptFilePath =
            string.Concat(Utils.MetalogixTempPath, "TestRemotePowerShell.ps1");

        private StringBuilder _processOutputData;

        private StringBuilder _processErrorData;

        public Metalogix.Actions.Remoting.Agent Agent { get; set; }

        public RemoteWorker(Metalogix.Actions.Remoting.Agent agent)
        {
            this.Agent = agent;
        }

        public bool AddRemoveCredentials(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmdkey.exe", command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            ProcessStartInfo processStartInfo1 = processStartInfo;
            Process process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo1
            };
            Process process1 = process;
            process1.Start();
            if (process1.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds) && process1.ExitCode == 0)
            {
                return true;
            }

            return false;
        }

        public bool Connect()
        {
            if (!Utils.ExportTestPowerShellScript("Test-WSMan", this._tempScriptFilePath))
            {
                Logging.LogMessageToMetalogixGlobalLogFile("Test PS Script cannot be exported locally.");
            }
            else
            {
                if (!this.AddRemoveCredentials(string.Format("/add:{0} /user:{1} /pass:{2}", this.Agent.MachineName,
                        this.Agent.UserName, this.Agent.Password)))
                {
                    string str =
                        string.Format(
                            "An error occurred while saving credentials in the Windows Vault for agent '{0}'.",
                            this.Agent.MachineName);
                    Logging.LogMessageToMetalogixGlobalLogFile(str);
                    return false;
                }

                string str1 = FileUtils.CopyScript(this._tempScriptFilePath, this.Agent.MachineName, true, null, false,
                    this.Agent.UserName, this.Agent.Password);
                if (!string.IsNullOrEmpty(str1))
                {
                    Process process = this.RunScriptUsingPsExec(str1,
                        "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass -file \"{0}\"\"", false);
                    if (process.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds) && process.ExitCode == 0)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public bool CopyApplicationMappingFiles(string targetFolderPath)
        {
            List<Mapping> applicationMappings = ApplicationMappingFiles.ApplicationMappings;
            string str = Path.Combine(ApplicationData.ApplicationDataFolder, ApplicationData.CompanyFolderName);
            try
            {
                foreach (Mapping applicationMapping in applicationMappings)
                {
                    string str1 = string.Format("{0} - {1}", "Content Matrix Console", applicationMapping.Name);
                    string str2 = Path.Combine(str, str1);
                    string str3 = Path.Combine(targetFolderPath, str1);
                    try
                    {
                        foreach (string file in applicationMapping.Files)
                        {
                            string str4 = Path.Combine(str2, file);
                            string str5 = Path.Combine(str3, file);
                            if (!File.Exists(str4))
                            {
                                continue;
                            }

                            this.CopyFile(str4, str5, true);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.LogMessage(exception,
                            string.Format("An error occurred while copying mapping files for : {0}.", str1));
                    }
                }
            }
            catch (Exception exception1)
            {
                this.LogMessage(exception1, "An error occurred while copying Application Mappings Files.");
                return false;
            }

            return true;
        }

        public bool CopyFile(string sourceFilePath, string targetFilePath, bool overWriteFiles = false)
        {
            bool flag;
            try
            {
                string str = string.Format("\\\\{0}", this.Agent.MachineName);
                targetFilePath = Path.Combine(str, targetFilePath.Replace(":", "$"));
                FileUtils.CopyScript(sourceFilePath, this.Agent.MachineName, false, targetFilePath, overWriteFiles,
                    this.Agent.UserName, this.Agent.Password);
                flag = true;
            }
            catch (Exception exception)
            {
                this.LogMessage(exception,
                    string.Format("An error occurred while copying file at path : {0}", targetFilePath));
                flag = false;
            }

            return flag;
        }

        public string GetOSVersion()
        {
            try
            {
                if (Utils.ExportTestPowerShellScript("(Get-WmiObject -class Win32_OperatingSystem).Caption",
                        this.OSInformationPowerScriptFilePath))
                {
                    return this.RunScript(this.OSInformationPowerScriptFilePath, false, true);
                }
            }
            catch (Exception exception)
            {
                this.LogMessage(exception, "An error occurred while retrieving Operating System Version.");
            }

            return string.Empty;
        }

        public string GetSystemFolderPath(Environment.SpecialFolder systemFolder)
        {
            string empty = string.Empty;
            string str = string.Empty;
            string empty1 = string.Empty;
            if (systemFolder == Environment.SpecialFolder.CommonApplicationData)
            {
                empty = "$env:ALLUSERSPROFILE";
                str = this._programDataPowerScriptFilePath;
                empty1 = "An error occurred while retrieving target Program Data folder path.";
            }
            else if (systemFolder == Environment.SpecialFolder.ApplicationData)
            {
                empty = "$env:APPDATA";
                str = this._applicationDataPowerShellScriptFilePath;
                empty1 = "An error occurred while retrieving target Application Mappings folder path.";
            }

            try
            {
                if (Utils.ExportTestPowerShellScript(empty, str))
                {
                    string str1 = this.RunScript(str, false, true);
                    str1 = Regex.Replace(str1, "\\t|\\n|\\r", string.Empty);
                    return Path.Combine(str1, ApplicationData.CompanyFolderName);
                }
            }
            catch (Exception exception)
            {
                this.LogMessage(exception, empty1);
            }

            return string.Empty;
        }

        public bool InstallSetup(string filePath)
        {
            bool flag;
            try
            {
                Process process = this.RunScriptUsingPsExec(filePath, "\"{0}\" /silent", false);
                flag = (!process.WaitForExit((int)TimeSpan.FromMinutes(15).TotalMilliseconds) || process.ExitCode != 0
                    ? false
                    : true);
            }
            catch (Exception exception)
            {
                this.LogMessage(exception, "An error occurred while installing setup.");
                flag = false;
            }

            return flag;
        }

        public bool IsAvailable()
        {
            return !Utils.HasRunningRemotePowerShell(this.Agent);
        }

        private void LogMessage(string message)
        {
            this.Agent.Parent.AddLog(this.Agent.AgentID, message);
        }

        private void LogMessage(Exception exception_0, string message)
        {
            message = string.Format("{0}. Error: {1}", message, exception_0);
            this.LogMessage(message);
        }

        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.Data))
            {
                this._processErrorData.AppendLine(e.Data);
            }
        }

        private void process_Exited(object sender, EventArgs eventArgs_0, bool isJob, string jobID)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            lock (this._sync)
            {
                if (isJob)
                {
                    Job job = null;
                    if (!string.IsNullOrEmpty(jobID))
                    {
                        job = this.Agent.Parent.GetJob(jobID);
                    }

                    if (this._processOutputData != null && this._processErrorData != null)
                    {
                        if (string.IsNullOrEmpty(this._processOutputData.ToString()) &&
                            !string.IsNullOrEmpty(this._processErrorData.ToString()))
                        {
                            string[] strArrays = Convert.ToString(this._processErrorData).Split(new char[] { '\n' });
                            string str = string.Concat(strArrays.Skip<string>(3).ToArray<string>());
                            this.SetAgentStatusToError(jobID, job, str);
                        }
                        else if (string.IsNullOrEmpty(this._processOutputData.ToString()) || !this._processOutputData
                                     .ToString().StartsWith("Windows PowerShell Version 3.0 or later"))
                        {
                            this.Agent.Parent.UpdateStatus(this.Agent, AgentStatus.Available);
                        }
                        else
                        {
                            this.SetAgentStatusToError(jobID, job, this._processOutputData.ToString());
                        }
                    }

                    this.LogMessage((job == null
                        ? "Job has finished."
                        : string.Format("Job '{0} (JobId: {1})' has finished.", job.Title, jobID)));
                    this._processOutputData = null;
                    this._processErrorData = null;
                }
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.Data))
            {
                this._processOutputData.AppendLine(e.Data);
            }
        }

        public string RunCommand(string command)
        {
            string empty = string.Empty;
            Process process = this.RunScriptUsingPsExec(command,
                "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass {0}", false);
            empty = (!process.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds) || process.ExitCode != 0
                ? process.StandardError.ReadToEnd()
                : process.StandardOutput.ReadToEnd());
            return Regex.Replace(empty, "\\t|\\n|\\r", string.Empty);
        }

        public string RunScript(string scriptFile, bool isJob = false)
        {
            return this.RunScript(scriptFile, isJob, false);
        }

        public string RunScript(string scriptFile, bool isJob = false, bool waitForExit = false)
        {
            string empty;
            try
            {
                string str = FileUtils.CopyScript(scriptFile, this.Agent.MachineName, true, null, false,
                    this.Agent.UserName, this.Agent.Password);
                if (string.IsNullOrEmpty(str))
                {
                    throw new FileNotFoundException(string.Format("File : {0} does not exists in agent.", scriptFile));
                }

                Process process = this.RunScriptUsingPsExec(str,
                    "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass -file \"{0}\"\"", isJob);
                FileUtils.DeleteScript(scriptFile);
                if (!isJob)
                {
                    empty = (!waitForExit || !process.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds) ||
                             process.ExitCode != 0
                        ? process.StandardOutput.ReadToEnd()
                        : process.StandardOutput.ReadToEnd());
                }
                else
                {
                    empty = string.Empty;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.LogMessage(exception, "An error occurred while running script.");
                if (exception is IOException)
                {
                    this.Agent.Parent.UpdateStatus(this.Agent, AgentStatus.Error);
                }

                if (isJob)
                {
                    string str1 = string.Format("Error:{0}{1}Stack Trace:{2}", exception.Message, Environment.NewLine,
                        exception.StackTrace);
                    this.SetJobStatusAsFailed(Path.GetFileNameWithoutExtension(scriptFile), str1);
                }

                FileUtils.CreateDirectory(Utils.MetalogixDeadScriptsPath, null, null);
                File.Move(scriptFile,
                    string.Concat(Utils.MetalogixDeadScriptsPath, "\\", Path.GetFileName(scriptFile)));
                return null;
            }

            return empty;
        }

        private Process RunScriptUsingPsExec(string scriptFile, string commandName, bool isJob = false)
        {
            Process process;
            lock (this._sync)
            {
                string empty = string.Empty;
                StringBuilder stringBuilder = (new StringBuilder()).Append("\"")
                    .AppendFormat("\\\\{0}", this.Agent.MachineName).Append("\" ").Append("-accepteula ");
                if (!string.IsNullOrEmpty(this.Agent.UserName))
                {
                    stringBuilder.AppendFormat("-u {0} ", this.Agent.UserName);
                    stringBuilder.AppendFormat("-p {0} ", this.Agent.Password);
                }

                stringBuilder.Append("-h ");
                stringBuilder.AppendFormat(commandName, scriptFile);
                ProcessStartInfo processStartInfo =
                    new ProcessStartInfo(Utils.PowerShellExeTool, stringBuilder.ToString())
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                ProcessStartInfo processStartInfo1 = processStartInfo;
                Process process1 = new Process()
                {
                    EnableRaisingEvents = true
                };
                if (isJob)
                {
                    empty = Path.GetFileNameWithoutExtension(scriptFile);
                    this.LogMessage(string.Format("Job '{0}' has started.", empty));
                    process1.OutputDataReceived += new DataReceivedEventHandler(this.process_OutputDataReceived);
                    process1.ErrorDataReceived += new DataReceivedEventHandler(this.process_ErrorDataReceived);
                }

                process1.Exited += new EventHandler((object sender, EventArgs e) =>
                    this.process_Exited(sender, e, isJob, empty));
                process1.StartInfo = processStartInfo1;
                process1.Start();
                if (isJob)
                {
                    this._processOutputData = new StringBuilder();
                    this._processErrorData = new StringBuilder();
                    process1.BeginOutputReadLine();
                    process1.BeginErrorReadLine();
                }

                process = process1;
            }

            return process;
        }

        private void SetAgentStatusToError(string jobId, Job currentJob, string errorMessage)
        {
            this.LogMessage(errorMessage);
            if (!string.IsNullOrEmpty(jobId) && currentJob != null && currentJob.Status == ActionStatus.Running)
            {
                this.SetJobStatusAsFailed(jobId, string.Format("Error:{0}", errorMessage));
            }

            bool flag = (errorMessage.StartsWith("add-pssnapin", StringComparison.InvariantCultureIgnoreCase)
                ? true
                : errorMessage.StartsWith("Windows PowerShell Version 3.0 or later",
                    StringComparison.InvariantCultureIgnoreCase));
            this.Agent.Parent.UpdateStatus(this.Agent, (flag ? AgentStatus.Error : AgentStatus.Available));
        }

        private void SetJobStatusAsFailed(string jobId, string errorMessage)
        {
            this.Agent.Parent.SetJobAsFailed(jobId, errorMessage);
        }
    }
}