using System;
using System.Threading;

namespace PreEmptive.SoS.Client.Messages
{
    public class FeatureCorrelationPartitionID : IFeatureCorrelationPartitionID
    {
        public FeatureCorrelationPartitionID()
        {
        }

        public string GetPartitionID()
        {
            return Thread.CurrentThread.GetHashCode().ToString();
        }
    }
}