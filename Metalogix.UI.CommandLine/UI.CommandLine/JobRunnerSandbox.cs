using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.Jobs.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Metalogix.UI.CommandLine
{
    internal class JobRunnerSandbox
    {
        private JobHistoryDb _jobsDB;

        private Metalogix.Actions.Action _currentJob;

        public JobRunnerSandbox(string fileName)
        {
            this._jobsDB = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, fileName);
            this._jobsDB.Jobs.ListChanged += new JobCollection.ListChangedHandler(this.JobsDB_ListChanged);
        }

        private void AddJobsByID(IEnumerable<string> ids, ref List<Job> jobs, ref List<string> jobIDs)
        {
            foreach (string id in ids)
            {
                if (jobIDs.Contains(id))
                {
                    continue;
                }
                bool flag = false;
                foreach (Job job in this._jobsDB.Jobs)
                {
                    if (job.JobID != id)
                    {
                        continue;
                    }
                    jobs.Add(job);
                    jobIDs.Add(job.JobID);
                    flag = true;
                    break;
                }
                if (flag)
                {
                    continue;
                }
                throw new Exception(string.Format("Job '{0}' not found.", id));
            }
        }

        private void AddJobsByName(IEnumerable<string> names, ref List<Job> jobs, ref List<string> jobIDs)
        {
            foreach (string name in names)
            {
                bool flag = false;
                foreach (Job job in this._jobsDB.Jobs)
                {
                    if (!(job.Title == name) || jobIDs.Contains(job.JobID))
                    {
                        continue;
                    }
                    jobs.Add(job);
                    jobIDs.Add(job.JobID);
                    flag = true;
                }
                if (flag)
                {
                    continue;
                }
                throw new Exception(string.Format("Job '{0}' not found.", name));
            }
        }

        private void AddJobsByNum(IEnumerable<int> nums, ref List<Job> jobs, ref List<string> jobIDs)
        {
            foreach (int num in nums)
            {
                if (num < 0 || num >= this._jobsDB.Jobs.Count)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Jobnumber '{0}' is out of range", num));
                }
                Job item = this._jobsDB.Jobs[num];
                if (jobIDs.Contains(item.JobID))
                {
                    continue;
                }
                jobs.Add(item);
                jobIDs.Add(item.JobID);
            }
        }

        private void JobsDB_ListChanged(ChangeType changeType, Job[] itemsChanged)
        {
            if (this.OnStateChanged != null)
            {
                this.OnStateChanged(changeType, itemsChanged);
            }
        }

        public void RunJob(JobRunnerSandboxOptions options)
        {
            List<Job> jobs = new List<Job>();
            List<string> strs = new List<string>();
            if (!options.RunAllJobs)
            {
                if (options.JobNums != null)
                {
                    this.AddJobsByNum(options.JobNums, ref jobs, ref strs);
                }
                if (options.JobIDs != null)
                {
                    this.AddJobsByID(options.JobIDs, ref jobs, ref strs);
                }
                if (options.JobNames != null)
                {
                    this.AddJobsByName(options.JobNames, ref jobs, ref strs);
                }
            }
            else
            {
                foreach (Job job in this._jobsDB.Jobs)
                {
                    jobs.Add(job);
                }
            }
            Job[] jobArray = new Job[jobs.Count];
            int num = 0;
            foreach (Job job1 in jobs)
            {
                job1.Clear();
                int num1 = num;
                num = num1 + 1;
                jobArray[num1] = job1;
            }
            this._currentJob = new JobRunner(jobArray);
            Job job2 = new Job(this._currentJob, null, null);
            if (options.RunAsync)
            {
                this._currentJob.RunAsync(job2.SourceList, job2.TargetList);
                return;
            }
            this._currentJob.Run(job2.SourceList, job2.TargetList);
        }

        public event JobCollection.ListChangedHandler OnStateChanged;
    }
}