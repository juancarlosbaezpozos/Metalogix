using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class TerminalServicesInformation
    {
        private bool allowTSConnectionsField;

        private string licensingNameField;

        private string terminalServerModeField;

        public bool AllowTSConnections
        {
            get { return this.allowTSConnectionsField; }
            set { this.allowTSConnectionsField = value; }
        }

        public string LicensingName
        {
            get { return this.licensingNameField; }
            set { this.licensingNameField = value; }
        }

        public string TerminalServerMode
        {
            get { return this.terminalServerModeField; }
            set { this.terminalServerModeField = value; }
        }

        public TerminalServicesInformation()
        {
        }
    }
}