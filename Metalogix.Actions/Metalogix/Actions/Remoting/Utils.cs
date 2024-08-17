using Metalogix;
using Metalogix.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Metalogix.Actions.Remoting
{
    public static class Utils
    {
        private const string PsNotFoundMessage = "process powershell was not found";

        public const string PowerShellExeCommandNameWithoutFileParameter =
            "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass {0}";

        public const string PowerShellExeCommandName =
            "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass -file \"{0}\"\"";

        public const string PowerShellExeCommandNameWithoutFile =
            "cmd /c \"echo . | powershell.exe -noprofile -executionpolicy bypass {0}";

        public const string GetServiceStatus = "(Get-Service -ServiceName \"{0}\").Status";

        public const string StartService = "(Get-Service -ServiceName \"{0}\") | start-service";

        public const string GetOSVersion = "(Get-WmiObject -class Win32_OperatingSystem).Version";

        public const string GetDotNetFramework =
            "($result = Get-Item 'HKLM:\\\\SOFTWARE\\\\Microsoft\\\\NET Framework Setup\\\\NDP\\\\v4\\\\Full').GetValue('Release')";

        public const string GetOSType = "(Get-WmiObject -class Win32_OperatingSystem).ProductType";

        public const string AddCredentialsToVaultCommand = "/add:{0} /user:{1} /pass:{2}";

        public const string RemoveCredentialsToVaultCommand = "/delete:{0}";

        public const string GetPSVersion = "$PSVersionTable.PSVersion.Major";

        public static string PowerShellVersionCheck;

        public static string PowerShellListTool;

        public static string PowerShellExeTool;

        public readonly static string MetalogixTempPath;

        public readonly static string MetalogixDeadScriptsPath;

        static Utils()
        {
            Utils.PowerShellVersionCheck =
                "if ( $PsVersionTable.PSVersion.Major -lt 3 ) { Write-Host \"Windows PowerShell Version 3.0 or later needs to be installed in order to execute Content Matrix PowerShell scripts.\"; exit; }";
            Utils.PowerShellListTool =
                string.Concat(Path.Combine(Path.GetDirectoryName(ApplicationData.MainAssembly.Location), "Tools"),
                    "\\pslist.exe");
            Utils.PowerShellExeTool =
                string.Concat(Path.Combine(Path.GetDirectoryName(ApplicationData.MainAssembly.Location), "Tools"),
                    "\\PsExec.exe");
            Utils.MetalogixTempPath = RemotingConfigurationVariables.RemotePowerShellScriptFilesLocation;
            Utils.MetalogixDeadScriptsPath = string.Concat(Utils.MetalogixTempPath, "\\DeadScripts");
        }

        public static bool ExportTestPowerShellScript(string powerShellCommand, string scriptFilePath)
        {
            bool flag;
            try
            {
                if (!File.Exists(scriptFilePath))
                {
                    FileUtils.CreateDirectory(Utils.MetalogixTempPath, null, null);
                    File.WriteAllText(scriptFilePath, powerShellCommand);
                }

                flag = true;
            }
            catch (Exception exception)
            {
                Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                    "An error occurred while exporting test PS script locally.", true);
                return false;
            }

            return flag;
        }

        public static bool HasRunningRemotePowerShell(Agent context)
        {
            StringBuilder stringBuilder = (new StringBuilder()).Append("\"")
                .AppendFormat("\\\\{0}", context.MachineName).Append("\" ").Append("-accepteula ");
            if (!string.IsNullOrEmpty(context.UserName))
            {
                stringBuilder.AppendFormat("-u {0} ", context.UserName);
                stringBuilder.AppendFormat("-p {0} ", context.Password);
            }

            stringBuilder.Append("-e powershell");
            ProcessStartInfo processStartInfo = new ProcessStartInfo(Utils.PowerShellListTool, stringBuilder.ToString())
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            Process process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new Exception("Fatal exception: process is null.");
            }

            process.WaitForExit();
            string end = process.StandardOutput.ReadToEnd();
            if (string.IsNullOrEmpty(end))
            {
                return false;
            }

            return end.IndexOf("process powershell was not found", StringComparison.OrdinalIgnoreCase) == -1;
        }

        public static bool IsDotNetFramework452(IRemoteWorker worker)
        {
            int num;
            string str =
                worker.RunCommand(
                    "($result = Get-Item 'HKLM:\\\\SOFTWARE\\\\Microsoft\\\\NET Framework Setup\\\\NDP\\\\v4\\\\Full').GetValue('Release')");
            if (string.IsNullOrEmpty(str) || !int.TryParse(str, out num))
            {
                return false;
            }

            return num >= 379893;
        }
    }
}