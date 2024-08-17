using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ApplicationInformation
    {
        private string applicationTypeField;

        private Guid idField;

        private string nameField;

        private string versionField;

        public string ApplicationType
        {
            get { return this.applicationTypeField; }
            set { this.applicationTypeField = value; }
        }

        public Guid Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string Version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }

        public ApplicationInformation()
        {
        }
    }
}