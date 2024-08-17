using System;
using System.Diagnostics;
using System.IO;

namespace Metalogix.Actions.Remoting
{
    public static class FileUtils
    {
        public static void CopyFile(string sourceFilePath, string targetFilePath, bool overWrite,
            string userName = null, string password = null)
        {
            if (userName == null || password == null)
            {
                File.Copy(sourceFilePath, targetFilePath, overWrite);
                return;
            }

            FileUtils.SetConnection(targetFilePath, userName, password);
            File.Copy(sourceFilePath, targetFilePath, overWrite);
            FileUtils.Disconnect(targetFilePath);
        }

        public static string CopyScript(string sourceFilePath, string targetMachineName, bool useTempPath,
            string targetFilePath = null, bool overWriteFiles = false, string userName = null, string password = null)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            if (useTempPath)
            {
                targetMachineName = string.Concat("\\\\", targetMachineName);
                string str = Path.Combine(targetMachineName, Utils.MetalogixTempPath.Replace(":", "$"));
                FileUtils.CreateDirectory(str, userName, password);
                if (fileName != null)
                {
                    targetFilePath = Path.Combine(str, fileName);
                }
            }
            else if (targetFilePath != null && fileName != null)
            {
                string str1 = targetFilePath.Replace(fileName, string.Empty);
                FileUtils.CreateDirectory(str1, userName, password);
            }

            if (overWriteFiles)
            {
                if (targetFilePath != null)
                {
                    FileUtils.CopyFile(sourceFilePath, targetFilePath, true, userName, password);
                }
            }
            else if (!File.Exists(targetFilePath) && targetFilePath != null)
            {
                FileUtils.CopyFile(sourceFilePath, targetFilePath, false, userName, password);
            }

            return targetFilePath;
        }

        public static void CreateDirectory(string targetDirectoryPath, string userName = null, string password = null)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                if (userName != null && password != null)
                {
                    FileUtils.SetConnection(targetDirectoryPath, userName, password);
                    Directory.CreateDirectory(targetDirectoryPath);
                    FileUtils.Disconnect(targetDirectoryPath);
                    return;
                }

                Directory.CreateDirectory(targetDirectoryPath);
            }
        }

        public static void DeleteScript(string scriptFile)
        {
            File.Delete(scriptFile);
        }

        private static void Disconnect(string targetDirectoryPath)
        {
            targetDirectoryPath = Path.GetDirectoryName(targetDirectoryPath);
            if (targetDirectoryPath != null)
            {
                targetDirectoryPath = targetDirectoryPath.Trim();
            }

            FileUtils.ExecuteCommand(string.Concat("NET USE ", targetDirectoryPath, " /delete"));
        }

        private static void ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", string.Concat("/C ", command))
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);
            if (process != null)
            {
                process.WaitForExit();
                process.Close();
            }
        }

        private static void SetConnection(string targetDirectoryPath, string userName, string password)
        {
            targetDirectoryPath = Path.GetDirectoryName(targetDirectoryPath);
            if (targetDirectoryPath != null)
            {
                targetDirectoryPath = targetDirectoryPath.Trim();
            }

            string[] strArrays = new string[] { "NET USE ", targetDirectoryPath, " /user:", userName, " ", password };
            FileUtils.ExecuteCommand(string.Concat(strArrays));
        }
    }
}