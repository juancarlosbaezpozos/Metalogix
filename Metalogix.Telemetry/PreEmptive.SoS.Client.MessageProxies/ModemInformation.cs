using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ModemInformation
    {
        private string modelField;

        private string deviceTypeField;

        public string DeviceType
        {
            get { return this.deviceTypeField; }
            set { this.deviceTypeField = value; }
        }

        public string Model
        {
            get { return this.modelField; }
            set { this.modelField = value; }
        }

        public ModemInformation()
        {
        }
    }
}