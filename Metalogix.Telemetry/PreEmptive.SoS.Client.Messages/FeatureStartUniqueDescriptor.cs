using System;

namespace PreEmptive.SoS.Client.Messages
{
    public class FeatureStartUniqueDescriptor
    {
        private readonly string _partitionID;

        private readonly Guid _guid;

        public Guid GUID
        {
            get { return this._guid; }
        }

        public string PartitionID
        {
            get { return this._partitionID; }
        }

        public FeatureStartUniqueDescriptor(string partitionID, Guid guid)
        {
            this._guid = guid;
            this._partitionID = partitionID;
        }
    }
}