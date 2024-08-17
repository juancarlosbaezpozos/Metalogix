using PreEmptive.SoS.Client.MessageProxies;
using System;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public class EventInformation
    {
        private string code;

        private PrivacySettings privacySetting;

        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        public PrivacySettings PrivacySetting
        {
            get { return this.privacySetting; }
            set { this.privacySetting = value; }
        }

        public EventInformation()
        {
        }

        protected virtual PreEmptive.SoS.Client.MessageProxies.EventInformation CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.EventInformation();
        }

        internal virtual void FillInProxy(PreEmptive.SoS.Client.MessageProxies.Message msgProxy)
        {
            if (msgProxy.Event == null)
            {
                msgProxy.Event = this.CreateProxy();
            }

            msgProxy.Event.Code = this.Code;
            msgProxy.Event.PrivacySetting = this.PrivacySetting;
        }
    }
}