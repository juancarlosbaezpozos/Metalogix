using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ApplicationLifeCycle : Message
    {
        private UserInfo userField;

        private HostInfo hostField;

        public HostInfo Host
        {
            get { return this.hostField; }
            set { this.hostField = value; }
        }

        public UserInfo User
        {
            get { return this.userField; }
            set { this.userField = value; }
        }

        public ApplicationLifeCycle()
        {
        }
    }
}