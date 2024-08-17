using PreEmptive.SoS.Client.Messages;
using System;

namespace PreEmptive.SoS.Client.Cache
{
    public class CacheEventArgs : EventArgs
    {
        private Guid groupId;

        private string instanceId;

        public string InstanceId
        {
            get { return this.instanceId; }
        }

        public Guid LifecycleGroupId
        {
            get { return this.groupId; }
        }

        internal CacheEventArgs()
        {
            this.groupId = Guid.NewGuid();
        }

        internal CacheEventArgs(string instanceId) : this()
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = InstanceIdUtil.GetInstanceId();
            }

            this.instanceId = instanceId;
        }
    }
}