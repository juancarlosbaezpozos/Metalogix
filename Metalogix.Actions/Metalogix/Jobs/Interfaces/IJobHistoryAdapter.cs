using Metalogix.Actions;
using Metalogix.Jobs;
using System;
using System.Collections.Generic;

namespace Metalogix.Jobs.Interfaces
{
    public interface IJobHistoryAdapter : IDisposable
    {
        string AdapterContext { get; }

        string AdapterType { get; }

        void AddJob(Job job_0);

        void AddJob(Job job_0, string[] sParams);

        void AddLogItem(string sJobID, LogItem logItem);

        void AddLogItem(string sJobID, LogItem logItem, string[] sParams);

        void Close();

        void DeleteJobs(string sJobIDs);

        void DeleteLogItems(string sJobID);

        Job GetJob(string sJobID);

        List<Job> GetJobs(params string[] jobIds);

        LogItem GetLogItem(Job job_0, string sLogItemID);

        void GetLogItemDetails(string sLogItemID, LogItem item);

        List<LogItem> GetLogItems(Job job_0);

        void Open();

        void UpdateJob(Job job_0);

        void UpdateJob(Job job_0, string[] sParams);

        void UpdateJobAction(Job[] jobs, string sActionXML);

        void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem);

        void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem, string[] sParams);
    }
}