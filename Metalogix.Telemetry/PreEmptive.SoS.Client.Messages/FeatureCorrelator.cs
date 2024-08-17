using System;
using System.Collections;

namespace PreEmptive.SoS.Client.Messages
{
    public class FeatureCorrelator
    {
        private Hashtable correlator = new Hashtable();

        private object lockobj = new object();

        private IFeatureCorrelationPartitionID partitionIDCreator;

        public FeatureCorrelator() : this(new FeatureCorrelationPartitionID())
        {
        }

        public FeatureCorrelator(IFeatureCorrelationPartitionID partitionIDCreator)
        {
            this.partitionIDCreator = partitionIDCreator;
        }

        public Guid Add(string name)
        {
            Guid guid;
            lock (this.lockobj)
            {
                if (!this.correlator.ContainsKey(name))
                {
                    this.correlator.Add(name, new ArrayList());
                }

                ArrayList item = (ArrayList)this.correlator[name];
                Guid guid1 = Guid.NewGuid();
                item.Add(new FeatureStartUniqueDescriptor(this.GetPartitionID(), guid1));
                guid = guid1;
            }

            return guid;
        }

        private string GetPartitionID()
        {
            return this.partitionIDCreator.GetPartitionID();
        }

        public Guid Remove(string name)
        {
            Guid gUID;
            lock (this.lockobj)
            {
                if (this.correlator.ContainsKey(name))
                {
                    ArrayList item = (ArrayList)this.correlator[name];
                    if (item.Count != 0)
                    {
                        string partitionID = this.GetPartitionID();
                        int count = item.Count - 1;
                        while (count >= 0)
                        {
                            FeatureStartUniqueDescriptor featureStartUniqueDescriptor =
                                (FeatureStartUniqueDescriptor)item[count];
                            if (featureStartUniqueDescriptor.PartitionID == partitionID)
                            {
                                item.RemoveAt(count);
                                gUID = featureStartUniqueDescriptor.GUID;
                                return gUID;
                            }
                            else
                            {
                                count--;
                            }
                        }

                        FeatureStartUniqueDescriptor item1 = (FeatureStartUniqueDescriptor)item[item.Count - 1];
                        item.RemoveAt(item.Count - 1);
                        gUID = item1.GUID;
                    }
                    else
                    {
                        this.correlator.Remove(name);
                        gUID = Guid.NewGuid();
                    }
                }
                else
                {
                    gUID = Guid.NewGuid();
                }
            }

            return gUID;
        }
    }
}