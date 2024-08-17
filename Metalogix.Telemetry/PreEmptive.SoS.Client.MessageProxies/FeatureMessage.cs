using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class FeatureMessage : Message
    {
        private Guid groupIdField;

        private string nameField;

        public Guid GroupId
        {
            get { return this.groupIdField; }
            set { this.groupIdField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public FeatureMessage()
        {
        }
    }
}