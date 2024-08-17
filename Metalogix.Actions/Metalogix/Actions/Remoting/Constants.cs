using System;

namespace Metalogix.Actions.Remoting
{
    public static class Constants
    {
        public readonly static string MachineName;

        public readonly static string CMVersion;

        public readonly static string UserName;

        public readonly static string Status;

        public readonly static string Details;

        public readonly static string AgentID;

        public readonly static string MachineIP;

        public readonly static string OSVersion;

        public readonly static string Password;

        public readonly static string Timestamp;

        public readonly static string JobID;

        public readonly static string Title;

        public readonly static string Created;

        public readonly static string Source;

        public readonly static string Target;

        public readonly static string KeyName;

        public readonly static string KeyValue;

        public readonly static string Key;

        static Constants()
        {
            Constants.MachineName = "MachineName";
            Constants.CMVersion = "CMVersion";
            Constants.UserName = "UserName";
            Constants.Status = "Status";
            Constants.Details = "Details";
            Constants.AgentID = "AgentID";
            Constants.MachineIP = "MachineIP";
            Constants.OSVersion = "OSVersion";
            Constants.Password = "Password";
            Constants.Timestamp = "TimeStamp";
            Constants.JobID = "JobID";
            Constants.Title = "Title";
            Constants.Created = "Created";
            Constants.Source = "Source";
            Constants.Target = "Target";
            Constants.KeyName = "Name";
            Constants.KeyValue = "Value";
            Constants.Key = "Key";
        }
    }
}