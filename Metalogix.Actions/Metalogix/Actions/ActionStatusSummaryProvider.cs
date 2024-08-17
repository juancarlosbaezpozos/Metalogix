using Metalogix.Actions.Properties;
using Metalogix.Jobs;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions
{
    public class ActionStatusSummaryProvider
    {
        public ActionStatusSummaryProvider()
        {
        }

        public virtual IEnumerable<KeyValuePair<string, long>> GetStatusSummary(Job job_0)
        {
            if (job_0 == null || job_0.Log == null)
            {
                return new List<KeyValuePair<string, long>>();
            }

            LogItemCollection log = job_0.Log;
            List<KeyValuePair<string, long>> keyValuePairs = new List<KeyValuePair<string, long>>(3)
            {
                new KeyValuePair<string, long>(Resources.StatusSummary_Successes, (long)log.Completions),
                new KeyValuePair<string, long>(Resources.StatusSummary_Warnings, (long)log.Warnings),
                new KeyValuePair<string, long>(Resources.StatusSummary_Failures, (long)log.Failures)
            };
            if (this.ShouldShowSkippedItems(job_0))
            {
                keyValuePairs.Add(new KeyValuePair<string, long>(Resources.StatusSummary_Skipped, (long)log.Skipped));
            }

            return keyValuePairs;
        }

        public static ActionStatusSummaryProvider GetSummaryProvider(Metalogix.Actions.Action action)
        {
            if (action == null)
            {
                return new ActionStatusSummaryProvider();
            }

            return ActionStatusSummaryProvider.GetSummaryProvider(action.GetType());
        }

        public static ActionStatusSummaryProvider GetSummaryProvider(Type type)
        {
            if (type == null)
            {
                return new ActionStatusSummaryProvider();
            }

            StatusSummaryProviderAttribute
                attributeFromType = StatusSummaryProviderAttribute.GetAttributeFromType(type);
            if (attributeFromType == null)
            {
                return new ActionStatusSummaryProvider();
            }

            if (!typeof(ActionStatusSummaryProvider).IsAssignableFrom(attributeFromType.ProviderType))
            {
                throw new Exception(string.Format("{0} is not a valid action status summary provider type.",
                    attributeFromType.ProviderType.FullName));
            }

            return (ActionStatusSummaryProvider)Activator.CreateInstance(attributeFromType.ProviderType);
        }

        protected virtual bool ShouldShowSkippedItems(Job job_0)
        {
            return job_0.Log.Skipped > 0;
        }
    }
}