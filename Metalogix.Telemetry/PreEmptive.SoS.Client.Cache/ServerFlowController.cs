using System;

namespace PreEmptive.SoS.Client.Cache
{
    public class ServerFlowController : IFlowController
    {
        private int step;

        private int dropThreshhold;

        private int maxInterval;

        private int histPtr;

        private readonly static int HISTSIZE;

        private int[] intervalHistory = new int[ServerFlowController.HISTSIZE];

        private int cacheSizeSetPoint = 100;

        int PreEmptive.SoS.Client.Cache.IFlowController.CacheSizeSetPoint
        {
            get { return this.cacheSizeSetPoint; }
            set { this.cacheSizeSetPoint = value; }
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.InitialInterval
        {
            get { return 50; }
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.MaximumInterval
        {
            get { return 60000; }
        }

        static ServerFlowController()
        {
            ServerFlowController.HISTSIZE = 10;
        }

        public ServerFlowController(int cacheSizeSetpoint)
        {
            IFlowController flowController = this;
            if (flowController != null)
            {
                flowController.CacheSizeSetPoint = cacheSizeSetpoint;
                this.step = cacheSizeSetpoint / 5;
                if (this.step == 0)
                {
                    this.step = 1;
                }

                this.dropThreshhold = 3 * cacheSizeSetpoint;
                this.maxInterval = flowController.MaximumInterval;
                this.intervalHistory[0] = flowController.InitialInterval;
            }
        }

        private int GetNextPtr(int int_0)
        {
            if (int_0 != ServerFlowController.HISTSIZE - 1)
            {
                int_0++;
            }
            else
            {
                int_0 = 0;
            }

            return int_0;
        }

        private void IncHistPtr()
        {
            this.histPtr = this.GetNextPtr(this.histPtr);
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.Control(int numMessages, double deltaT)
        {
            int num = this.intervalHistory[this.histPtr];
            int num1 = 0;
            if (numMessages > this.cacheSizeSetPoint * 2)
            {
                num1 = 10;
            }
            else if (numMessages < this.cacheSizeSetPoint - this.step)
            {
                if (num < this.maxInterval)
                {
                    num1 = num + 50;
                }
            }
            else if (numMessages <= this.cacheSizeSetPoint + this.step)
            {
                num1 = num;
            }
            else
            {
                num1 = num - 50;
                if (num1 < 10)
                {
                    num1 = 10;
                }
            }

            this.IncHistPtr();
            this.intervalHistory[this.histPtr] = num1;
            return num1;
        }

        int PreEmptive.SoS.Client.Cache.IFlowController.messagesToDrop(int currentMessageCount, bool serviceAlive)
        {
            if (currentMessageCount <= this.dropThreshhold)
            {
                return 0;
            }

            return currentMessageCount - this.cacheSizeSetPoint;
        }
    }
}