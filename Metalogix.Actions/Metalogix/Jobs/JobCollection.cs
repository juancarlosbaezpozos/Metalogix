using Metalogix;
using Metalogix.Actions;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Metalogix.Jobs
{
    public class JobCollection
    {
        private EventHandler m_On_LicenseUpdated;

        private Metalogix.Jobs.JobHistoryDb m_db;

        private List<Job> m_data = new List<Job>();

        private Queue<Job> m_itemsAdded = new Queue<Job>();

        private Queue<Job> m_itemsUpdated = new Queue<Job>();

        private Queue<Job> m_itemsComponentUpdated = new Queue<Job>();

        private int m_iUpdatingCount;

        private object updateDataLock = new object();

        public int Count
        {
            get { return this.m_data.Count; }
        }

        public bool HasRunningActions
        {
            get
            {
                bool flag;
                IEnumerator enumerator = this.GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            Job current = (Job)enumerator.Current;
                            if (current.Action != null)
                            {
                                if (current.Action.Status == ActionStatus.Running ||
                                    current.Action.Status == ActionStatus.Paused)
                                {
                                    break;
                                }

                                if (current.Action.Status == ActionStatus.Aborting)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    flag = true;
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                return flag;
            }
        }

        public Job this[int iIndex]
        {
            get { return this.m_data[iIndex]; }
        }

        public Metalogix.Jobs.JobHistoryDb JobHistoryDb
        {
            get { return this.m_db; }
        }

        public JobCollection(Metalogix.Jobs.JobHistoryDb jobHistoryDb_0)
        {
            this.m_db = jobHistoryDb_0;
            this.m_On_LicenseUpdated = new EventHandler(this.On_LicenseUpdated);
            MLLicenseProvider.Instance.LicenseUpdated += this.m_On_LicenseUpdated;
        }

        public void Add(Job newJob)
        {
            if (newJob.Parent != null)
            {
                throw new Exception("This item already belongs to a collection.");
            }

            IEnumerable<Job> mData =
                from tempJob in this.m_data
                where tempJob.JobID == newJob.JobID
                select tempJob;
            if (mData.Any<Job>())
            {
                newJob.SetParent(this);
                newJob.Log.Clear();
                return;
            }

            this.m_data.Add(newJob);
            newJob.SetParent(this);
            this.FireListChanged(ChangeType.ItemAdded, newJob);
        }

        public void BeginUpdate()
        {
            this.m_iUpdatingCount++;
        }

        public void DeleteJobs(Job[] jobs)
        {
            string dList = JobCollection.GetIDList(jobs);
            this.JobHistoryDb.DeleteJobs(dList);
            Job[] jobArray = jobs;
            for (int i = 0; i < (int)jobArray.Length; i++)
            {
                Job job = jobArray[i];
                this.m_data.Remove(job);
            }

            this.FireListChanged(ChangeType.ItemDeleted, jobs);
        }

        internal void DisconnectEvents()
        {
            MLLicenseProvider.Instance.LicenseUpdated -= this.m_On_LicenseUpdated;
        }

        public void EndUpdate()
        {
            this.m_iUpdatingCount = (this.m_iUpdatingCount > 0 ? this.m_iUpdatingCount - 1 : 0);
            this.Update();
        }

        public void FetchData()
        {
            this.m_data.Clear();
            this.m_data = this.m_db.GetJobs(new string[0]);
            foreach (Job mDatum in this.m_data)
            {
                mDatum.SetParent(this);
            }

            this.FireListChanged(ChangeType.Reset, new Job[0]);
        }

        internal void FireListChanged(ChangeType changeType, Job item)
        {
            this.FireListChanged(changeType, new Job[] { item });
        }

        internal void FireListChanged(ChangeType changeType, Job[] items)
        {
            if (this.ListChanged != null)
            {
                this.ListChanged(changeType, items);
            }

            lock (this.updateDataLock)
            {
                Job[] jobArray = items;
                for (int i = 0; i < (int)jobArray.Length; i++)
                {
                    Job job = jobArray[i];
                    if (changeType == ChangeType.ItemAdded)
                    {
                        if (!this.m_itemsAdded.Contains(job))
                        {
                            this.m_itemsAdded.Enqueue(job);
                        }
                    }
                    else if (changeType == ChangeType.ItemUpdated)
                    {
                        if (!this.m_itemsUpdated.Contains(job))
                        {
                            this.m_itemsUpdated.Enqueue(job);
                        }
                    }
                    else if (changeType == ChangeType.ItemComponentUpdated &&
                             !this.m_itemsComponentUpdated.Contains(job))
                    {
                        this.m_itemsComponentUpdated.Enqueue(job);
                    }
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_data.GetEnumerator();
        }

        private static string GetIDList(Job[] jobs)
        {
            StringBuilder stringBuilder = new StringBuilder(500);
            Job[] jobArray = jobs;
            for (int i = 0; i < (int)jobArray.Length; i++)
            {
                Job job = jobArray[i];
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append("'");
                stringBuilder.Append(job.JobID);
                stringBuilder.Append("'");
            }

            return stringBuilder.ToString();
        }

        public Job GetJob(string jobID)
        {
            if (string.IsNullOrEmpty(jobID))
            {
                return null;
            }

            return this.m_db.GetJob(jobID);
        }

        public LogItemCollection GetRelatedLogItems(Job[] jobs)
        {
            if (jobs == null || (int)jobs.Length == 0)
            {
                throw new Exception("No jobs were supplied");
            }

            List<LogItem> logItems = new List<LogItem>();
            Job[] jobArray = jobs;
            for (int i = 0; i < (int)jobArray.Length; i++)
            {
                logItems.AddRange(jobArray[i].Log);
            }

            return new LogItemCollection(logItems);
        }

        private void On_LicenseUpdated(object sender, EventArgs e)
        {
            (new Thread(new ThreadStart(this.RevalidateActionLicenses))).Start();
        }

        public void RevalidateActionLicenses()
        {
            foreach (Job job in this)
            {
                job.RevalidateActionLicense();
            }
        }

        public void Update()
        {
            lock (this.updateDataLock)
            {
                if (this.m_iUpdatingCount == 0)
                {
                    while (this.m_itemsAdded.Count > 0)
                    {
                        Job job = this.m_itemsAdded.Dequeue();
                        if (this.JobHistoryDb == null)
                        {
                            continue;
                        }

                        job.Add();
                    }

                    while (this.m_itemsUpdated.Count > 0)
                    {
                        Job job1 = this.m_itemsUpdated.Dequeue();
                        if (this.JobHistoryDb == null)
                        {
                            continue;
                        }

                        job1.Update();
                    }

                    List<Job> jobs = new List<Job>();
                    while (this.m_itemsComponentUpdated.Count > 0)
                    {
                        int num = 0;
                        int count = this.m_itemsComponentUpdated.Count;
                        Metalogix.Actions.Action action = null;
                        while (num < count)
                        {
                            Job job2 = this.m_itemsComponentUpdated.Dequeue();
                            if (action == null)
                            {
                                action = job2.Action;
                                jobs.Add(job2);
                            }
                            else if (!action.Options.Equals(job2.Action.Options))
                            {
                                this.m_itemsComponentUpdated.Enqueue(job2);
                            }
                            else
                            {
                                jobs.Add(job2);
                            }

                            num++;
                        }

                        if (this.JobHistoryDb != null)
                        {
                            Job[] array = jobs.ToArray();
                            string xML = action.ToXML();
                            this.JobHistoryDb.UpdateJobAction(array, xML);
                            Array.ForEach<Job>(array, (Job job_0) => job_0.SetActionXml(xML));
                        }

                        jobs.Clear();
                    }

                    jobs = null;
                }
            }
        }

        public event JobCollection.ListChangedHandler ListChanged;

        public delegate void ListChangedHandler(ChangeType changeType, Job[] itemsChanged);
    }
}