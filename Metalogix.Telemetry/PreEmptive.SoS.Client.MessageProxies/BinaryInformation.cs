using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class BinaryInformation
    {
        private string methodNameField;

        private DateTime modifiedDateField;

        private string typeNameField;

        private Guid idField;

        private string nameField;

        private string versionField;

        public Guid Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string MethodName
        {
            get { return this.methodNameField; }
            set { this.methodNameField = value; }
        }

        public DateTime ModifiedDate
        {
            get { return this.modifiedDateField; }
            set { this.modifiedDateField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string TypeName
        {
            get { return this.typeNameField; }
            set { this.typeNameField = value; }
        }

        public string Version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }

        public BinaryInformation()
        {
        }
    }
}