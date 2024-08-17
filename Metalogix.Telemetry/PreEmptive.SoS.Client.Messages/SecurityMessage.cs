using PreEmptive.SoS.Client.MessageProxies;
using System;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class SecurityMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private string attackOrigin;

        public string AttackOrigin
        {
            get { return this.attackOrigin; }
            set { this.attackOrigin = value; }
        }

        public SecurityMessage()
        {
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.SecurityMessage();
        }

        internal override void FillInProxy()
        {
            base.FillInProxy();
            ((PreEmptive.SoS.Client.MessageProxies.SecurityMessage)base.Proxy).AttackOrigin = this.attackOrigin;
        }
    }
}