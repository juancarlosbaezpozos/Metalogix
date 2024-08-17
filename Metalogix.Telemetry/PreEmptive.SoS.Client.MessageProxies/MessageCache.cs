using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class MessageCache
    {
        private string instanceIdField;

        private Guid applicationGroupIdField;

        private BusinessInformation businessField;

        private DateTime timeSentUtcField;

        private string apiLanguageField;

        private string apiVersionField;

        private Guid idField;

        private Message[] messagesField;

        private SchemaVersionValue schemaVersionField;

        private ApplicationInformation applicationField;

        public string ApiLanguage
        {
            get { return this.apiLanguageField; }
            set { this.apiLanguageField = value; }
        }

        public string ApiVersion
        {
            get { return this.apiVersionField; }
            set { this.apiVersionField = value; }
        }

        public ApplicationInformation Application
        {
            get { return this.applicationField; }
            set { this.applicationField = value; }
        }

        public Guid ApplicationGroupId
        {
            get { return this.applicationGroupIdField; }
            set { this.applicationGroupIdField = value; }
        }

        public BusinessInformation Business
        {
            get { return this.businessField; }
            set { this.businessField = value; }
        }

        public Guid Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string InstanceId
        {
            get { return this.instanceIdField; }
            set { this.instanceIdField = value; }
        }

        public Message[] Messages
        {
            get { return this.messagesField; }
            set { this.messagesField = value; }
        }

        public SchemaVersionValue SchemaVersion
        {
            get { return this.schemaVersionField; }
            set { this.schemaVersionField = value; }
        }

        public DateTime TimeSentUtc
        {
            get { return this.timeSentUtcField; }
            set { this.timeSentUtcField = value; }
        }

        public MessageCache()
        {
        }
    }
}