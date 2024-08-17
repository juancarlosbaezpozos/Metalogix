using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ExceptionInformation
    {
        private int sequenceField;

        private string typeField;

        private string messageField;

        private StackEntry[] stackTraceField;

        public string Message
        {
            get { return this.messageField; }
            set { this.messageField = value; }
        }

        public int Sequence
        {
            get { return this.sequenceField; }
            set { this.sequenceField = value; }
        }

        public StackEntry[] StackTrace
        {
            get { return this.stackTraceField; }
            set { this.stackTraceField = value; }
        }

        public string Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public ExceptionInformation()
        {
        }
    }
}