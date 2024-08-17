using System;

namespace PreEmptive.SoS.Client.Cache
{
    internal class DefaultFlowController : IFlowController
    {
        int PreEmptive.SoS.Client.Cache.IFlowController.CacheSizeSetPoint
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.InitialInterval
        {
            get { return 10000; }
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.MaximumInterval
        {
            get { return 60000; }
        }

        public DefaultFlowController()
        {
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.Control(int numMessages, double deltaT)
        {
            IFlowController flowController = this;
            if (flowController == null)
            {
                return 0;
            }

            return flowController.InitialInterval;
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.messagesToDrop(int currentMessageCount, bool serviceAlive)
        {
            return 0;
        }
    }
}