using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ProcessorInformation
    {
        private int currentClockSpeedMhzField;

        private int maxClockSpeedMhzField;

        private string manufacturerField;

        private short addressWidthField;

        private string nameField;

        private string idField;

        public short AddressWidth
        {
            get { return this.addressWidthField; }
            set { this.addressWidthField = value; }
        }

        public int CurrentClockSpeedMhz
        {
            get { return this.currentClockSpeedMhzField; }
            set { this.currentClockSpeedMhzField = value; }
        }

        public string Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string Manufacturer
        {
            get { return this.manufacturerField; }
            set { this.manufacturerField = value; }
        }

        public int MaxClockSpeedMhz
        {
            get { return this.maxClockSpeedMhzField; }
            set { this.maxClockSpeedMhzField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public ProcessorInformation()
        {
        }
    }
}