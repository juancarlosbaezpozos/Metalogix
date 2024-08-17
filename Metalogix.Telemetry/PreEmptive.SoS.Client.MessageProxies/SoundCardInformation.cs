using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class SoundCardInformation
    {
        private string productNameField;

        private string manufacturerField;

        public string Manufacturer
        {
            get { return this.manufacturerField; }
            set { this.manufacturerField = value; }
        }

        public string ProductName
        {
            get { return this.productNameField; }
            set { this.productNameField = value; }
        }

        public SoundCardInformation()
        {
        }
    }
}