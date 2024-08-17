using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Jobs
{
    public class JobGrouping
    {
        private Dictionary<string, JobRecord> m_jobRecords = new Dictionary<string, JobRecord>();

        public long Cost
        {
            get
            {
                long cost = 0L;
                foreach (JobRecord value in this.m_jobRecords.Values)
                {
                    cost += value.Cost;
                }

                return cost;
            }
        }

        public JobRecord this[string sJobId]
        {
            get
            {
                if (!this.m_jobRecords.ContainsKey(sJobId))
                {
                    return null;
                }

                return this.m_jobRecords[sJobId];
            }
        }

        public JobGrouping()
        {
        }

        public void Add(JobRecord record)
        {
            this.m_jobRecords.Add(record.Job.JobID, record);
        }

        public void AddJobGrouping(JobGrouping grouping)
        {
            this.AddRange(grouping.m_jobRecords.Values);
        }

        public void AddRange(IEnumerable<JobRecord> records)
        {
            foreach (JobRecord record in records)
            {
                if (!this.m_jobRecords.ContainsKey(record.Job.JobID))
                {
                    this.m_jobRecords.Add(record.Job.JobID, record);
                }
                else
                {
                    this.m_jobRecords[record.Job.JobID] = record;
                }
            }
        }

        public static List<JobGrouping> GroupJobs(Job[] jobs, int iGroupNumber)
        {
            List<JobGrouping> jobGroupings = new List<JobGrouping>(iGroupNumber);
            List<JobRecord> jobRecords = new List<JobRecord>();
            Job[] jobArray = jobs;
            for (int i = 0; i < (int)jobArray.Length; i++)
            {
                jobRecords.Add(new JobRecord(jobArray[i]));
            }

            jobRecords.Sort(new JobRecordComparer());
            for (int j = 0; j < iGroupNumber; j++)
            {
                jobGroupings.Add(new JobGrouping());
            }

            foreach (JobRecord jobRecord in jobRecords)
            {
                JobGrouping jobGrouping = null;
                long cost = 9223372036854775807L;
                foreach (JobGrouping jobGrouping1 in jobGroupings)
                {
                    if (jobGrouping1.Cost > cost)
                    {
                        continue;
                    }

                    jobGrouping = jobGrouping1;
                    cost = jobGrouping1.Cost;
                }

                jobGrouping.Add(jobRecord);
            }

            return jobGroupings;
        }

        public Dictionary<string, string> ParseAnalysis()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            foreach (JobRecord value in this.m_jobRecords.Values)
            {
                Dictionary<string, string>.Enumerator enumerator = value.ParseAnalysis().GetEnumerator();
                try
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            KeyValuePair<string, string> current = enumerator.Current;
                            object item = null;
                            if (!strs.ContainsKey(current.Key))
                            {
                                strs.Add(current.Key, current.Value);
                            }
                            else
                            {
                                bool flag = false;
                                item = strs[current.Key];
                                long num = 0L;
                                num = 0L;
                                if (long.TryParse(current.Value, out num))
                                {
                                    long num1 = 0L;
                                    if (long.TryParse(item.ToString(), out num1))
                                    {
                                        item = num + num1;
                                        strs[current.Key] = item.ToString();
                                        flag = true;
                                    }
                                }

                                if (!flag)
                                {
                                    num = Format.ParseFormattedSize(current.Value);
                                    if (num > 0L)
                                    {
                                        long num2 = Format.ParseFormattedSize(item.ToString());
                                        if (num2 > 0L)
                                        {
                                            strs[current.Key] = Format.FormatSize(new long?(num + num2));
                                            break;
                                        }
                                    }
                                }

                                if (!flag)
                                {
                                    double num3 = 0;
                                    if (double.TryParse(current.Value, out num3))
                                    {
                                        double num4 = 0;
                                        if (double.TryParse(item.ToString(), out num4))
                                        {
                                            item = num3 + num4;
                                            strs[current.Key] = item.ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }

            return strs;
        }
    }
}