using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Diagnostics;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class PerformanceProbeMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private PerformanceCounter ramCounter;

        private PerformanceCounter cpuCounter;

        private string name;

        public double MemoryMBAvailable
        {
            get { return (double)this.ramCounter.NextValue(); }
        }

        public double MemoryMBUsedByProcess
        {
            get { return (double)Process.GetCurrentProcess().WorkingSet / (double)((int)Math.Pow(1024, 2)); }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                string str;
                string str1;
                if (value == null)
                {
                    str = null;
                }
                else
                {
                    str = value.Trim();
                }

                string str2 = str;
                if (str2 == null || string.Empty.Equals(str2))
                {
                    str1 = null;
                }
                else
                {
                    str1 = str2;
                }

                this.name = str1;
            }
        }

        public double PercentCPUUtilization
        {
            get { return (double)this.cpuCounter.NextValue(); }
        }

        public PerformanceProbeMessage() : this(null)
        {
        }

        public PerformanceProbeMessage(string name)
        {
            this.Name = name;
            base.Event = new PreEmptive.SoS.Client.Messages.EventInformation()
            {
                Code = "Performance.Probe"
            };
            ((PreEmptive.SoS.Client.MessageProxies.PerformanceProbeMessage)base.Proxy).MemoryMBUsedByProcess =
                this.MemoryMBUsedByProcess;
            try
            {
                this.cpuCounter = new PerformanceCounter("Process", "% Processor Time",
                    Process.GetCurrentProcess().ProcessName, true);
                this.ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                ((PreEmptive.SoS.Client.MessageProxies.PerformanceProbeMessage)base.Proxy).MemoryMBAvailable =
                    this.MemoryMBAvailable;
                ((PreEmptive.SoS.Client.MessageProxies.PerformanceProbeMessage)base.Proxy).PercentCPUUtilization =
                    this.PercentCPUUtilization;
            }
            catch (Exception exception)
            {
            }
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.PerformanceProbeMessage();
        }

        internal override void FillInProxy()
        {
            base.FillInProxy();
            ((PreEmptive.SoS.Client.MessageProxies.PerformanceProbeMessage)base.Proxy).Name = this.name;
        }
    }
}