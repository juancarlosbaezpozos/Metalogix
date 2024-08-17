using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting.Database;
using Metalogix.Core;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace Metalogix.Actions.Remoting
{
    public class RemoteJobScheduler
    {
        private Queue<string> _queuedJobs;

        private readonly object _sync = new object();

        private readonly System.Timers.Timer _timer;

        private readonly AgentCollection _agents;

        private List<Job> _jobs = new List<Job>();

        private readonly IAgentDb _agentDb;

        private static RemoteJobScheduler _instance;

        public static RemoteJobScheduler Instance
        {
            get
            {
                if (RemoteJobScheduler._instance == null)
                {
                    RemoteJobScheduler._instance = new RemoteJobScheduler();
                }

                return RemoteJobScheduler._instance;
            }
            set { RemoteJobScheduler._instance = value; }
        }

        public bool IsJobRunning
        {
            get { return this._timer.Enabled; }
        }

        public List<Job> Jobs
        {
            get { return this._jobs; }
        }

        private RemoteJobScheduler()
        {
            this._queuedJobs = new Queue<string>();
            System.Timers.Timer timer =
                new System.Timers.Timer((double)RemotingConfigurationVariables.RemotePowerShellTimerInterval)
                {
                    AutoReset = true
                };
            this._timer = timer;
            this._agentDb = new AgentDb(JobsSettings.AdapterContext.ToInsecureString());
            AgentCollection agentCollection = new AgentCollection(this._agentDb);
            agentCollection.FetchData();
            this._agents = agentCollection;
            this._timer.Elapsed += new ElapsedEventHandler(this.ProcessJobQueue);
        }

        public void AddRunningJobs()
        {
            List<Job> jobs = this.GetJobs(ActionStatus.Running);
            if (jobs != null && jobs.Count > 0)
            {
                foreach (Job job in jobs)
                {
                    if (this._jobs.Any<Job>((Job job_1) => job_1.JobID == job.JobID))
                    {
                        continue;
                    }

                    this._jobs.Add(job);
                }
            }
        }

        private void EnqueueJobScript(List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                string jobScriptPath = this.GetJobScriptPath(job);
                if (string.IsNullOrEmpty(jobScriptPath) || this._queuedJobs.Contains(jobScriptPath))
                {
                    continue;
                }

                this.QueueJob(jobScriptPath, job);
            }
        }

        private IRemoteWorker GetAvailableWorker()
        {
            IRemoteWorker remoteWorker;
            List<Agent> availableAgents = this._agents.GetAvailableAgents();
            if (availableAgents != null && availableAgents.Count > 0)
            {
                List<Agent>.Enumerator enumerator = availableAgents.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        Agent current = enumerator.Current;
                        current.Parent = this._agents;
                        remoteWorker = new RemoteWorker(current);
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return remoteWorker;
            }

            return null;
        }

        public List<Job> GetJobs(ActionStatus status)
        {
            return this._agents.GetJobs(status);
        }

        private string GetJobScriptPath(Job job_0)
        {
            string str = Path.Combine(Utils.MetalogixTempPath, string.Concat(job_0.JobID, ".ps1"));
            if (File.Exists(str))
            {
                return str;
            }

            return null;
        }

        public bool HasBusyAgent()
        {
            bool flag;
            List<Agent>.Enumerator enumerator = this._agentDb.GetAll().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Status != AgentStatus.Busy)
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return flag;
        }

        public void ProcessJobQueue(object sender, ElapsedEventArgs e)
        {
            lock (this._sync)
            {
                this.EnqueueJobScript(this.GetJobs(ActionStatus.Queued));
                if (this._queuedJobs.Count != 0)
                {
                    IRemoteWorker availableWorker = this.GetAvailableWorker();
                    if (availableWorker != null)
                    {
                        Logging.LogMessageToMetalogixGlobalLogFile(string.Format("Found available worker '{0}'",
                            availableWorker.Agent.MachineName));
                        string str = this._queuedJobs.Dequeue();
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
                        this.UpdateJobInfo(fileNameWithoutExtension, ActionStatus.Running,
                            availableWorker.Agent.MachineName);
                        this._agents.UpdateStatus(availableWorker.Agent, AgentStatus.Busy);
                        availableWorker.RunScript(str, true);
                    }
                    else
                    {
                        Logging.LogMessageToMetalogixGlobalLogFile("No worker is available currently");
                    }
                }
                else
                {
                    Logging.LogMessageToMetalogixGlobalLogFile("No script files are in queue");
                    this.StopTimer();
                }
            }
        }

        private void QueueJob(string script, Job job_0)
        {
            lock (this._sync)
            {
                this._queuedJobs.Enqueue(script);
                this._jobs.Add(job_0);
            }
        }

        public void RefreshAgent(Agent agent)
        {
            if (agent == null)
            {
                return;
            }

            AgentStatus agentStatu = AgentStatus.Error;
            RemoteWorker remoteWorker = new RemoteWorker(agent);
            try
            {
                try
                {
                    string str = "Refreshed Successfully.";
                    try
                    {
                        if (!string.IsNullOrEmpty(agent.CMVersion) && remoteWorker.Connect())
                        {
                            agentStatu = (remoteWorker.IsAvailable() ? AgentStatus.Available : AgentStatus.Busy);
                        }
                    }
                    catch (Exception exception)
                    {
                        str = string.Format("An error occurred while refreshing agent. Error '{0}'.", exception);
                    }

                    LogHelper.LogMessage(agent, str);
                    if (agent.Status != agentStatu)
                    {
                        agent.Parent.UpdateStatus(agent, agentStatu);
                    }
                }
                catch (Exception exception2)
                {
                    Exception exception1 = exception2;
                    string str1 = "An error occurred while refreshing agent.";
                    GlobalServices.ErrorHandler.HandleException("Refresh Agent", str1, exception1, ErrorIcon.Error);
                    LogHelper.LogMessage(agent, exception1, str1);
                }
            }
            finally
            {
                agent.Parent.UpdateLatestLogOnUI(agent);
            }
        }

        public void RefreshAllAgents()
        {
            foreach (Agent list in this._agents.GetList())
            {
                this.RefreshAgent(list);
            }
        }

        public void RefreshJobs(bool forceReload = true)
        {
            List<Job> jobs = new List<Job>();
            foreach (Job _job in this._jobs)
            {
                Job job = this._agentDb.GetJob(_job.JobID);
                if (job == null)
                {
                    continue;
                }

                jobs.Add(job);
            }

            this._jobs = jobs;
            if (forceReload && this.JobListChanged != null)
            {
                this.JobListChanged(null, null);
            }
        }

        public void RemoveJob(Job job_0)
        {
            IEnumerable<Job> jobID =
                from tempJob in this._jobs
                where tempJob.JobID == job_0.JobID
                select tempJob;
            if (jobID.Any<Job>())
            {
                this._jobs.Remove(jobID.First<Job>());
                string str = this.GetJobScriptPath(job_0);
                lock (this._sync)
                {
                    this._queuedJobs = new Queue<string>(
                        from jobScriptPath in this._queuedJobs
                        where jobScriptPath != str
                        select jobScriptPath);
                }

                if (!string.IsNullOrEmpty(str))
                {
                    Metalogix.Actions.Remoting.FileUtils.DeleteScript(string.Concat(Utils.MetalogixTempPath, "\\",
                        job_0.JobID, (PowerShellUtils.IsPowerShellInstalled ? ".ps1" : ".txt")));
                    if (this._agents.GetJob(job_0.JobID) != null)
                    {
                        this.UpdateJobInfo(job_0.JobID, ActionStatus.NotRunning, null);
                    }
                }

                if (this.JobListChanged != null)
                {
                    this.JobListChanged(null, null);
                }
            }
        }

        public void Run()
        {
            Logging.LogMessageToMetalogixGlobalLogFile("Job Processing Started.");
            this.StartTimer();
        }

        private void StartTimer()
        {
            this._timer.Start();
            Logging.LogMessageToMetalogixGlobalLogFile("Job Scheduler Started.");
        }

        private void StopTimer()
        {
            this._timer.Stop();
            Logging.LogMessageToMetalogixGlobalLogFile("Job Scheduler Stopped.");
        }

        public void UpdateJobInfo(string jobId, ActionStatus status, string machineName = null)
        {
            this._agents.UpdateJobInfo(this._agents.GetJob(jobId), status, machineName);
        }

        public event EventHandler JobListChanged;
    }
}