using System;

namespace PreEmptive.SoS.Client.Cache
{
    public interface IFlowController
    {
        int CacheSizeSetPoint { get; set; }

        int InitialInterval { get; }

        int MaximumInterval { get; }

        int Control(int numMessages, double deltaT);

        int messagesToDrop(int currentMessageCount, bool serviceAlive);
    }
}