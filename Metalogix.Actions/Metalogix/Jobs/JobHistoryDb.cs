using Metalogix.Actions;
using Metalogix.Jobs.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs
{
    public class JobHistoryDb : IDisposable, IJobHistoryAdapter
    {
        private readonly IJobHistoryAdapter m_adapter;

        private readonly Metalogix.Jobs.AdapterCallWrapper m_adapterWrapper;

        private JobCollection m_jobHistory;

        private IJobHistoryAdapter Adapter
        {
            get { return this.m_adapter; }
        }

        public Metalogix.Jobs.AdapterCallWrapper AdapterCallWrapper
        {
            get { return this.m_adapterWrapper; }
        }

        public string AdapterContext
        {
            get { return this.Adapter.AdapterContext; }
        }

        public string AdapterType
        {
            get { return this.Adapter.AdapterType; }
        }

        public JobCollection Jobs
        {
            get
            {
                if (this.m_jobHistory == null)
                {
                    this.m_jobHistory = new JobCollection(this);
                    this.m_jobHistory.FetchData();
                }

                return this.m_jobHistory;
            }
            private set
            {
                if (object.ReferenceEquals(this.m_jobHistory, value))
                {
                    return;
                }

                if (this.m_jobHistory != null)
                {
                    this.m_jobHistory.DisconnectEvents();
                }

                this.m_jobHistory = value;
            }
        }

        internal JobHistoryDb(IJobHistoryAdapter adapter) : this(adapter,
            ActionConfigurationVariables.DefaultJobHistoryCallWrapper)
        {
        }

        internal JobHistoryDb(IJobHistoryAdapter adapter, Metalogix.Jobs.AdapterCallWrapper wrapper)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            this.m_adapter = adapter;
            this.m_adapterWrapper = wrapper;
            this.Open();
        }

        public void AddJob(Job job_0)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.AddJob(job_0)));
        }

        public void AddJob(Job job_0, string[] sParams)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.AddJob(job_0, sParams)));
        }

        public void AddLogItem(string sJobID, LogItem logItem)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.AddLogItem(sJobID, logItem)));
        }

        public void AddLogItem(string sJobID, LogItem logItem, string[] sParams)
        {
            this.AdapterCallWrapper(
                new AdapterCallWrapperAction(() => this.Adapter.AddLogItem(sJobID, logItem, sParams)));
        }

        public void Close()
        {
            if (this.m_adapter != null)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.m_adapter.Close()));
            }

            this.Jobs = null;
        }

        public void DeleteJobs(string sJobIDs)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.DeleteJobs(sJobIDs)));
        }

        public void DeleteLogItems(string sJobID)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.DeleteLogItems(sJobID)));
        }

        public void Dispose()
        {
            this.Close();
            if (this.m_adapter != null)
            {
                this.m_adapter.Dispose();
            }
        }

        public Job GetJob(string sJobID)
        {
            Job job = null;
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => job = this.Adapter.GetJob(sJobID)));
            return job;
        }

        public List<Job> GetJobs(params string[] jobIds)
        {
            List<Job> jobs = null;
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => jobs = this.Adapter.GetJobs(jobIds)));
            return jobs;
        }

        public LogItem GetLogItem(Job job_0, string sLogItemID)
        {
            LogItem logItem = null;
            this.AdapterCallWrapper(
                new AdapterCallWrapperAction(() => logItem = this.Adapter.GetLogItem(job_0, sLogItemID)));
            return logItem;
        }

        public void GetLogItemDetails(string sLogItemID, LogItem item)
        {
            this.AdapterCallWrapper(
                new AdapterCallWrapperAction(() => this.Adapter.GetLogItemDetails(sLogItemID, item)));
        }

        public List<LogItem> GetLogItems(Job job_0)
        {
            List<LogItem> logItems = null;
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => logItems = this.Adapter.GetLogItems(job_0)));
            return logItems;
        }

        public void Open()
        {
            if (this.m_adapter != null)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.m_adapter.Open()));
            }

            this.Jobs = null;
        }

        public void UpdateJob(Job job_0)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.UpdateJob(job_0)));
        }

        public void UpdateJob(Job job_0, string[] sParams)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.UpdateJob(job_0, sParams)));
        }

        public void UpdateJobAction(Job[] jobs, string sActionXML)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.Adapter.UpdateJobAction(jobs, sActionXML)));
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem)
        {
            this.AdapterCallWrapper(
                new AdapterCallWrapperAction(() => this.Adapter.UpdateLogItem(sJobID, sLogItemID, logItem)));
        }

        public void UpdateLogItem(string sJobID, string sLogItemID, LogItem logItem, string[] sParams)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                this.Adapter.UpdateLogItem(sJobID, sLogItemID, logItem, sParams)));
        }
    }
}