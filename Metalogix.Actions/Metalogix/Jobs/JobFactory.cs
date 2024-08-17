using Metalogix.Actions;
using Metalogix.Actions.Remoting.Database;
using Metalogix.Jobs.Adapters;
using Metalogix.Jobs.Interfaces;
using System;

namespace Metalogix.Jobs
{
    public static class JobFactory
    {
        public static JobHistoryDb CreateJobHistoryDb(JobHistoryAdapterType adapterType, string adapterContext)
        {
            return JobFactory.CreateJobHistoryDb(adapterType.ToString(), adapterContext);
        }

        public static JobHistoryDb CreateJobHistoryDb(string adapterType, string adapterContext)
        {
            return JobFactory.CreateJobHistoryDb(adapterType, adapterContext,
                ActionConfigurationVariables.DefaultJobHistoryCallWrapper);
        }

        public static JobHistoryDb CreateJobHistoryDb(string adapterType, string adapterContext,
            AdapterCallWrapper callWrapper)
        {
            JobHistoryDb jobHistoryDb;
            if (string.IsNullOrEmpty(adapterType))
            {
                throw new ArgumentNullException("adapterType");
            }

            if (string.IsNullOrEmpty(adapterContext))
            {
                throw new ArgumentNullException("adapterContext");
            }

            if (callWrapper == null)
            {
                throw new ArgumentNullException("callWrapper");
            }

            IJobHistoryAdapter jobHistorySqlServerDb = null;
            string str = adapterType;
            string str1 = str;
            if (str != null)
            {
                if (str1 == "SqlServer")
                {
                    jobHistorySqlServerDb = new JobHistorySqlServerDb(adapterContext);
                }
                else if (str1 == "SqlCe")
                {
                    jobHistorySqlServerDb = new JobHistorySqlCeDb(adapterContext);
                }
                else
                {
                    if (str1 != "Agent")
                    {
                        throw new ConditionalDetailException(string.Concat("'", adapterType,
                            "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
                    }

                    jobHistorySqlServerDb = new AgentDb(adapterContext);
                }

                try
                {
                    jobHistoryDb = new JobHistoryDb(jobHistorySqlServerDb, callWrapper);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    throw new ConditionalDetailException(exception.Message, exception);
                }

                return jobHistoryDb;
            }

            throw new ConditionalDetailException(string.Concat("'", adapterType,
                "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
        }

        public static JobHistoryDb CreateJobHistoryDbForFullSql(string connectionString)
        {
            return new JobHistoryDb(new JobHistorySqlServerDb(connectionString),
                ActionConfigurationVariables.DefaultJobHistoryCallWrapper);
        }
    }
}