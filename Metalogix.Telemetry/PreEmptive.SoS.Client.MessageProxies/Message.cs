using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlInclude(typeof(ApplicationLifeCycle))]
    [XmlInclude(typeof(FaultMessage))]
    [XmlInclude(typeof(FeatureMessage))]
    [XmlInclude(typeof(PerformanceProbeMessage))]
    [XmlInclude(typeof(SecurityMessage))]
    [XmlInclude(typeof(SessionLifeCycle))]
    [XmlInclude(typeof(SystemProfileMessage))]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public abstract class Message
    {
        private Guid sessionIdField;

        private EventInformation eventField;

        private ExtendedKey[] extendedInformationField;

        private BinaryInformation binaryField;

        private DateTime timeStampUtcField;

        private Guid idField;

        public BinaryInformation Binary
        {
            get { return this.binaryField; }
            set { this.binaryField = value; }
        }

        public EventInformation Event
        {
            get { return this.eventField; }
            set { this.eventField = value; }
        }

        public ExtendedKey[] ExtendedInformation
        {
            get { return this.extendedInformationField; }
            set { this.extendedInformationField = value; }
        }

        public Guid Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public Guid SessionId
        {
            get { return this.sessionIdField; }
            set { this.sessionIdField = value; }
        }

        public DateTime TimeStampUtc
        {
            get { return this.timeStampUtcField; }
            set { this.timeStampUtcField = value; }
        }

        protected Message()
        {
        }
    }
}