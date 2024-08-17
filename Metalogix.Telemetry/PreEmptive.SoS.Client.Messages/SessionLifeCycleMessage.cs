using PreEmptive.SoS.Client.MessageProxies;
using System;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public class SessionLifeCycleMessage : PreEmptive.SoS.Client.Messages.Message
    {
        public SessionLifeCycleMessage()
        {
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new SessionLifeCycle();
        }
    }
}