using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ManufacturerInformation
    {
        private string manufacturerField;

        private string modelField;

        public string Manufacturer
        {
            get { return this.manufacturerField; }
            set { this.manufacturerField = value; }
        }

        public string Model
        {
            get { return this.modelField; }
            set { this.modelField = value; }
        }

        public ManufacturerInformation()
        {
        }
    }
}