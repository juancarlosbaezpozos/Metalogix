using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class VideoControllerInformation
    {
        private int adapterRAMField;

        private string nameField;

        private long currentNumberOfColorsField;

        private string driverVersionField;

        public int AdapterRAM
        {
            get { return this.adapterRAMField; }
            set { this.adapterRAMField = value; }
        }

        public long CurrentNumberOfColors
        {
            get { return this.currentNumberOfColorsField; }
            set { this.currentNumberOfColorsField = value; }
        }

        public string DriverVersion
        {
            get { return this.driverVersionField; }
            set { this.driverVersionField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public VideoControllerInformation()
        {
        }
    }
}