using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions.Remoting.Database
{
    public interface IAgentDb : IDisposable, IJobHistoryAdapter
    {
        Agent Add(Agent agent);

        bool AddLog(Guid agentID, string message);

        bool Delete(Guid agendID);

        List<Agent> GetAll();

        List<Agent> GetAllAvailableAgents();

        List<Job> GetJobs(ActionStatus status);

        List<KeyValuePair<DateTime, string>> GetLogDetails(Guid agentID, bool isLatestEntry, bool sortAsc = false);

        bool SetJobAsFailed(string jobID, string errorMessage);

        bool UpdateCMVersion(Agent agent);

        bool UpdateCredentials(Agent agent);

        bool UpdateOSVersion(Agent agent);

        bool UpdateStatus(Agent agent);
    }
}