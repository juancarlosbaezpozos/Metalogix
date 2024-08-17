using Metalogix.Actions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Remoting
{
    public class Agent : ActionOptions
    {
        public Guid AgentID { get; set; }

        public string CMVersion { get; set; }

        public List<KeyValuePair<DateTime, string>> Details { get; set; }

        public string MachineIP { get; set; }

        public string MachineName { get; set; }

        public string OSVersion { get; set; }

        public AgentCollection Parent { get; set; }

        public string Password { get; set; }

        public AgentStatus Status { get; set; }

        public string UserName { get; set; }

        protected Agent()
        {
        }

        public Agent(string machineName, string userName, string passWord)
        {
            this.MachineName = machineName;
            this.UserName = userName;
            this.Password = passWord;
        }

        public Agent(Guid agentID, string machineIP, string machineName, string osVersion, string cmVersion,
            string userName, string passWord, AgentStatus status, List<KeyValuePair<DateTime, string>> details) : this(
            machineName, userName, passWord)
        {
            this.AgentID = agentID;
            this.MachineIP = machineIP;
            this.OSVersion = osVersion;
            this.CMVersion = cmVersion;
            this.Status = status;
            this.Details = details;
        }
    }
}