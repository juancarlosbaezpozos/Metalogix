using Metalogix.Jobs;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions
{
    public class CompletionDetailsOrderProvider
    {
        private List<string> _orderingList = new List<string>();

        protected virtual List<string> OrderingList
        {
            get { return this._orderingList; }
        }

        public CompletionDetailsOrderProvider()
        {
        }

        public static CompletionDetailsOrderProvider GetOrderProvider(Metalogix.Actions.Action action)
        {
            if (action == null)
            {
                return new CompletionDetailsOrderProvider();
            }

            return CompletionDetailsOrderProvider.GetOrderProvider(action.GetType());
        }

        public static CompletionDetailsOrderProvider GetOrderProvider(Type type)
        {
            if (type == null)
            {
                return new CompletionDetailsOrderProvider();
            }

            CompletionDetailsOrderProviderAttribute attributeFromType =
                CompletionDetailsOrderProviderAttribute.GetAttributeFromType(type);
            if (attributeFromType == null)
            {
                return new CompletionDetailsOrderProvider();
            }

            if (!typeof(CompletionDetailsOrderProvider).IsAssignableFrom(attributeFromType.ProviderType))
            {
                throw new Exception(string.Format("{0} is not a valid completion detail order provider type.",
                    attributeFromType.ProviderType.FullName));
            }

            return (CompletionDetailsOrderProvider)Activator.CreateInstance(attributeFromType.ProviderType);
        }

        public virtual IEnumerable<KeyValuePair<string, long>> OrderCompletionDetails(Job job_0,
            Dictionary<string, long> details)
        {
            if (this.OrderingList == null || this.OrderingList.Count == 0)
            {
                return details;
            }

            List<KeyValuePair<string, long>> keyValuePairs = new List<KeyValuePair<string, long>>();
            List<KeyValuePair<string, long>> keyValuePairs1 = new List<KeyValuePair<string, long>>();
            foreach (KeyValuePair<string, long> detail in details)
            {
                int num = this.OrderingList.IndexOf(detail.Key);
                if (num < 0)
                {
                    keyValuePairs1.Add(detail);
                }
                else if (num >= keyValuePairs.Count)
                {
                    keyValuePairs.Add(detail);
                }
                else
                {
                    keyValuePairs.Insert(num, detail);
                }
            }

            keyValuePairs.AddRange(keyValuePairs1);
            return keyValuePairs;
        }
    }
}