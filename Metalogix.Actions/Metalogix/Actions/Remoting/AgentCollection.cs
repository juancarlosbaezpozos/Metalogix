using Metalogix.Actions;
using Metalogix.Actions.Remoting.Database;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Metalogix.Actions.Remoting
{
    public class AgentCollection
    {
        private readonly IAgentDb _agentDb;

        private readonly ThreadSafeDictionary<Guid, Agent> _agentIDMappings;

        public AgentCollection(IAgentDb agentDb)
        {
            this._agentDb = agentDb;
            this._agentIDMappings = new ThreadSafeDictionary<Guid, Agent>();
        }

        public Agent Add(Agent agent)
        {
            Agent agent1 = this._agentDb.Add(agent);
            if (agent1 != null)
            {
                agent1.Parent = this;
                this.FireListChanged(ChangeType.ItemAdded, agent);
            }

            return agent1;
        }

        private void AddAgent(Agent agent)
        {
            if (!this._agentIDMappings.ContainsKey(agent.AgentID))
            {
                this._agentIDMappings.Add(agent.AgentID, agent);
            }
        }

        public void AddLog(Guid agentId, string message)
        {
            this._agentDb.AddLog(agentId, message);
            this.FireListChanged(ChangeType.ItemComponentUpdated, null);
        }

        public bool Delete(Agent agent)
        {
            bool flag = this._agentDb.Delete(agent.AgentID);
            bool flag1 = flag;
            if (flag)
            {
                this.FireListChanged(ChangeType.ItemDeleted, agent);
            }

            return flag1;
        }

        public void FetchData()
        {
            this._agentIDMappings.Clear();
            List<Agent> all = this._agentDb.GetAll();
            if (all != null && all.Count > 0)
            {
                foreach (Agent agent in all)
                {
                    agent.Parent = this;
                    this.AddAgent(agent);
                }
            }
        }

        private void FireListChanged(ChangeType changeType, Agent agent)
        {
            if (this.AgentListChanged != null)
            {
                switch (changeType)
                {
                    case ChangeType.ItemAdded:
                    {
                        this.AddAgent(agent);
                        goto case ChangeType.ItemComponentUpdated;
                    }
                    case ChangeType.ItemUpdated:
                    {
                        this.RemoveAgent(agent.AgentID);
                        this.AddAgent(agent);
                        goto case ChangeType.ItemComponentUpdated;
                    }
                    case ChangeType.ItemFinished:
                    case ChangeType.ItemComponentUpdated:
                    {
                        this.AgentListChanged(changeType, null);
                        break;
                    }
                    case ChangeType.ItemDeleted:
                    {
                        this.RemoveAgent(agent.AgentID);
                        goto case ChangeType.ItemComponentUpdated;
                    }
                    default:
                    {
                        goto case ChangeType.ItemComponentUpdated;
                    }
                }
            }
        }

        public List<KeyValuePair<DateTime, string>> GetAgentLogDetails(Guid agentId, bool isLatestEntry,
            bool sortAsc = false)
        {
            return this._agentDb.GetLogDetails(agentId, isLatestEntry, sortAsc);
        }

        public List<Agent> GetAvailableAgents()
        {
            return this._agentDb.GetAllAvailableAgents();
        }

        public Job GetJob(string jobID)
        {
            return this._agentDb.GetJob(jobID);
        }

        public List<Job> GetJobs(ActionStatus status)
        {
            return this._agentDb.GetJobs(status);
        }

        public List<Agent> GetList()
        {
            return this._agentIDMappings.Values.ToList<Agent>();
        }

        public Agent GetRemoteContextFromId(Guid agentId)
        {
            if (!this._agentIDMappings.ContainsKey(agentId))
            {
                return null;
            }

            return this._agentIDMappings[agentId];
        }

        private void RemoveAgent(Guid agentId)
        {
            if (this._agentIDMappings.ContainsKey(agentId))
            {
                this._agentIDMappings.Remove(agentId);
            }
        }

        public bool SetJobAsFailed(string jobID, string errorMessage)
        {
            return this._agentDb.SetJobAsFailed(jobID, errorMessage);
        }

        public bool UpdateCMVersion(Agent agent, string cmVersion)
        {
            agent.CMVersion = cmVersion;
            bool flag = this._agentDb.UpdateCMVersion(agent);
            bool flag1 = flag;
            if (flag)
            {
                this.FireListChanged(ChangeType.ItemUpdated, agent);
            }

            return flag1;
        }

        public bool UpdateCredentials(Agent agent, string userName, string passWord)
        {
            agent.UserName = userName;
            agent.Password = passWord;
            bool flag = this._agentDb.UpdateCredentials(agent);
            bool flag1 = flag;
            if (flag)
            {
                this.FireListChanged(ChangeType.ItemUpdated, agent);
            }

            return flag1;
        }

        public void UpdateJobInfo(Job job_0, ActionStatus status, string machineName)
        {
            job_0.Status = status;
            job_0.StatusMessage = status.ToString();
            if (!string.IsNullOrEmpty(machineName))
            {
                job_0.MachineName = machineName;
            }

            this._agentDb.UpdateJob(job_0);
        }

        public void UpdateLatestLogOnUI(Agent agent)
        {
            List<KeyValuePair<DateTime, string>> agentLogDetails = this.GetAgentLogDetails(agent.AgentID, true, false);
            if (agentLogDetails != null && agentLogDetails.Count > 0)
            {
                if (!Convert.ToString(agent.Details[0]).Equals(agentLogDetails[0].ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    agent.Details.Insert(0, agentLogDetails[0]);
                }

                this.FireListChanged(ChangeType.ItemUpdated, agent);
            }
        }

        public bool UpdateOSVersion(Agent agent, string osVersion)
        {
            agent.OSVersion = osVersion;
            bool flag = this._agentDb.UpdateOSVersion(agent);
            bool flag1 = flag;
            if (flag)
            {
                this.FireListChanged(ChangeType.ItemUpdated, agent);
            }

            return flag1;
        }

        public bool UpdateStatus(Agent agent, AgentStatus status)
        {
            agent.Status = status;
            bool flag = this._agentDb.UpdateStatus(agent);
            bool flag1 = flag;
            if (flag)
            {
                this.FireListChanged(ChangeType.ItemUpdated, agent);
            }

            return flag1;
        }

        public event EventHandler AgentListChanged;
    }
}