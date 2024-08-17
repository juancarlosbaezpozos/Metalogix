using PreEmptive.SoS.Client.MessageProxies;
using System;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public class ApplicationLifeCycleMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private PreEmptive.SoS.Client.Messages.HostInfo host;

        private PreEmptive.SoS.Client.Messages.UserInfo user;

        public PreEmptive.SoS.Client.Messages.HostInfo Host
        {
            get
            {
                if (this.host == null)
                {
                    this.host = new PreEmptive.SoS.Client.Messages.HostInfo();
                }

                return this.host;
            }
            set { this.host = value; }
        }

        public PreEmptive.SoS.Client.Messages.UserInfo User
        {
            get
            {
                if (this.user == null)
                {
                    this.user = new PreEmptive.SoS.Client.Messages.UserInfo();
                }

                return this.user;
            }
            set { this.user = value; }
        }

        public ApplicationLifeCycleMessage()
        {
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new ApplicationLifeCycle();
        }

        internal override void FillInProxy()
        {
            base.FillInProxy();
            if (this.host != null)
            {
                this.host.FillInProxy((ApplicationLifeCycle)base.Proxy);
            }

            if (this.user != null)
            {
                this.user.FillInProxy((ApplicationLifeCycle)base.Proxy);
            }
        }
    }
}