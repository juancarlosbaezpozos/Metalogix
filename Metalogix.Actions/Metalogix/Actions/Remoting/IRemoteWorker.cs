using System;

namespace Metalogix.Actions.Remoting
{
    public interface IRemoteWorker
    {
        Metalogix.Actions.Remoting.Agent Agent { get; set; }

        bool AddRemoveCredentials(string command);

        bool Connect();

        bool CopyApplicationMappingFiles(string targetFolderPath);

        bool CopyFile(string sourceFilePath, string targetFilePath, bool overWriteFiles = false);

        string GetOSVersion();

        string GetSystemFolderPath(Environment.SpecialFolder systemFolder);

        bool InstallSetup(string fileName);

        bool IsAvailable();

        string RunCommand(string command);

        string RunScript(string scriptFile, bool isJob = false);
    }
}