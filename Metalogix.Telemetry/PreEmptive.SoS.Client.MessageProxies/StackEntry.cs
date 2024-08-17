using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class StackEntry
    {
        private int sequenceField;

        private string typeField;

        private string methodField;

        private long lineField;

        private string signatureField;

        private string fileField;

        public string File
        {
            get { return this.fileField; }
            set { this.fileField = value; }
        }

        public long Line
        {
            get { return this.lineField; }
            set { this.lineField = value; }
        }

        public string Method
        {
            get { return this.methodField; }
            set { this.methodField = value; }
        }

        public int Sequence
        {
            get { return this.sequenceField; }
            set { this.sequenceField = value; }
        }

        public string Signature
        {
            get { return this.signatureField; }
            set { this.signatureField = value; }
        }

        public string Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public StackEntry()
        {
        }
    }
}