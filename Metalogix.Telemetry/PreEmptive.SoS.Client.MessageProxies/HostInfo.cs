using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class HostInfo
    {
        private string runtimeVersionField;

        private string iPAddressField;

        private string nameField;

        private OSInformation osField;

        public string IPAddress
        {
            get { return this.iPAddressField; }
            set { this.iPAddressField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public OSInformation OS
        {
            get { return this.osField; }
            set { this.osField = value; }
        }

        public string RuntimeVersion
        {
            get { return this.runtimeVersionField; }
            set { this.runtimeVersionField = value; }
        }

        public HostInfo()
        {
        }
    }
}