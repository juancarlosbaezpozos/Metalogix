using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class AppComponent
    {
        private string nameField;

        private string versionField;

        private string fullNameField;

        public string FullName
        {
            get { return this.fullNameField; }
            set { this.fullNameField = value; }
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

        public AppComponent()
        {
        }
    }
}