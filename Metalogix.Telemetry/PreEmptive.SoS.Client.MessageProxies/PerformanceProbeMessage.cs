using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class PerformanceProbeMessage : Message
    {
        private double percentCPUUtilizationField;

        private double memoryMBAvailableField;

        private double memoryMBUsedByProcessField;

        private string nameField;

        public double MemoryMBAvailable
        {
            get { return this.memoryMBAvailableField; }
            set { this.memoryMBAvailableField = value; }
        }

        public double MemoryMBUsedByProcess
        {
            get { return this.memoryMBUsedByProcessField; }
            set { this.memoryMBUsedByProcessField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public double PercentCPUUtilization
        {
            get { return this.percentCPUUtilizationField; }
            set { this.percentCPUUtilizationField = value; }
        }

        public PerformanceProbeMessage()
        {
        }
    }
}