using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class SecurityMessage : Message
    {
        private string attackOriginField;

        public string AttackOrigin
        {
            get { return this.attackOriginField; }
            set { this.attackOriginField = value; }
        }

        public SecurityMessage()
        {
        }
    }
}