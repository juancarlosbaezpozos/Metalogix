using Metalogix.Actions;
using System;
using System.Collections.Generic;

namespace Metalogix.Jobs
{
    public class JobRecord
    {
        private static char[] s_propertyDelimiter;

        private static char[] s_valueDelimiter;

        private Metalogix.Jobs.Job m_job;

        private long? m_lCost = null;

        public long Cost
        {
            get
            {
                if (!this.m_lCost.HasValue)
                {
                    this.m_lCost = new long?(this.m_job.Action.GetAnalysisCost(this.ParseAnalysis()));
                }

                return this.m_lCost.Value;
            }
        }

        public Metalogix.Jobs.Job Job
        {
            get { return this.m_job; }
        }

        static JobRecord()
        {
            JobRecord.s_propertyDelimiter = new char[] { ';' };
            JobRecord.s_valueDelimiter = new char[] { '-' };
        }

        public JobRecord(Metalogix.Jobs.Job job_0)
        {
            this.m_job = job_0;
        }

        public Dictionary<string, string> ParseAnalysis()
        {
            Dictionary<string, string> strs = null;
            string title = this.m_job.Title;
            int num = title.IndexOf("::");
            if (num >= 0)
            {
                strs = new Dictionary<string, string>();
                title = title.Substring(num + 2);
                if (!title.Contains("Analysis is unavailable"))
                {
                    string[] strArrays = title.Split(JobRecord.s_propertyDelimiter);
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string[] strArrays1 = strArrays[i].Split(JobRecord.s_valueDelimiter);
                        if ((int)strArrays1.Length == 2)
                        {
                            strs.Add(strArrays1[0].Trim(), strArrays1[1].Trim());
                        }
                        else if ((int)strArrays1.Length == 1)
                        {
                            strs.Add(strArrays1[0].Trim(), "");
                        }
                    }
                }
            }
            else
            {
                strs = this.m_job.Analyze(null);
            }

            return strs;
        }
    }
}