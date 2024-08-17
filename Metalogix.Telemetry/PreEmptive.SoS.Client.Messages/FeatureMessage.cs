using PreEmptive.SoS.Client.MessageProxies;
using System;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class FeatureMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private string name;

        public Guid GroupId
        {
            get { return ((PreEmptive.SoS.Client.MessageProxies.FeatureMessage)base.Proxy).GroupId; }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value == null || value == string.Empty)
                {
                    throw new ArgumentException("Argument cannot be null or empty", "name");
                }

                this.name = value;
            }
        }

        public FeatureMessage(string name)
        {
            if (name == null || name == string.Empty)
            {
                throw new ArgumentException("Argument cannot be null or empty", "name");
            }

            this.name = name;
            ((PreEmptive.SoS.Client.MessageProxies.FeatureMessage)base.Proxy).GroupId = base.Proxy.Id;
        }

        public void AddToGroup(Guid guid_0)
        {
            if (guid_0 == Guid.Empty)
            {
                throw new ArgumentException("Argument cannot be empty", "id");
            }

            ((PreEmptive.SoS.Client.MessageProxies.FeatureMessage)base.Proxy).GroupId = guid_0;
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.FeatureMessage();
        }

        internal override void FillInProxy()
        {
            base.FillInProxy();
            ((PreEmptive.SoS.Client.MessageProxies.FeatureMessage)base.Proxy).Name = this.name;
        }
    }
}