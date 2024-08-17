using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class FaultMessage : Message
    {
        private ExceptionInformation[] exceptionsField;

        private AppComponent[] componentsField;

        private FaultEventType faultEventField;

        private string contactField;

        private string commentField;

        public string Comment
        {
            get { return this.commentField; }
            set { this.commentField = value; }
        }

        public AppComponent[] Components
        {
            get { return this.componentsField; }
            set { this.componentsField = value; }
        }

        public string Contact
        {
            get { return this.contactField; }
            set { this.contactField = value; }
        }

        public ExceptionInformation[] Exceptions
        {
            get { return this.exceptionsField; }
            set { this.exceptionsField = value; }
        }

        public FaultEventType FaultEvent
        {
            get { return this.faultEventField; }
            set { this.faultEventField = value; }
        }

        public FaultMessage()
        {
        }
    }
}