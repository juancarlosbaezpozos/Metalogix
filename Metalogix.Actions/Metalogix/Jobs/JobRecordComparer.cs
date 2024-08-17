using System;
using System.Collections.Generic;

namespace Metalogix.Jobs
{
    public class JobRecordComparer : Comparer<JobRecord>
    {
        public JobRecordComparer()
        {
        }

        public override int Compare(JobRecord jobRecord_0, JobRecord jobRecord_1)
        {
            return (int)(jobRecord_1.Cost - jobRecord_0.Cost);
        }
    }
}