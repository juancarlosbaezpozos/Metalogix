using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class MemoryInformation
    {
        private int totalPhysicalMemoryField;

        private int speedField;

        private int capacityField;

        public int Capacity
        {
            get { return this.capacityField; }
            set { this.capacityField = value; }
        }

        public int Speed
        {
            get { return this.speedField; }
            set { this.speedField = value; }
        }

        public int TotalPhysicalMemory
        {
            get { return this.totalPhysicalMemoryField; }
            set { this.totalPhysicalMemoryField = value; }
        }

        public MemoryInformation()
        {
        }
    }
}