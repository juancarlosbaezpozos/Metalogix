using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class NetworkAdapterInformation
    {
        private string networkInterfaceIPAddressField;

        private long speedField;

        private long maxSpeedField;

        private string mACAddressField;

        private string netConnectionIDField;

        private bool dHCPEnabledField;

        public bool DHCPEnabled
        {
            get { return this.dHCPEnabledField; }
            set { this.dHCPEnabledField = value; }
        }

        public string MACAddress
        {
            get { return this.mACAddressField; }
            set { this.mACAddressField = value; }
        }

        public long MaxSpeed
        {
            get { return this.maxSpeedField; }
            set { this.maxSpeedField = value; }
        }

        public string NetConnectionID
        {
            get { return this.netConnectionIDField; }
            set { this.netConnectionIDField = value; }
        }

        public string NetworkInterfaceIPAddress
        {
            get { return this.networkInterfaceIPAddressField; }
            set { this.networkInterfaceIPAddressField = value; }
        }

        public long Speed
        {
            get { return this.speedField; }
            set { this.speedField = value; }
        }

        public NetworkAdapterInformation()
        {
        }
    }
}