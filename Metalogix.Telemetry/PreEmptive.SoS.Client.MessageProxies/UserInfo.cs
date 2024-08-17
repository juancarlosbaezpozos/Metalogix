using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class UserInfo
    {
        private bool isAdministratorField;

        private string nameField;

        public bool IsAdministrator
        {
            get { return this.isAdministratorField; }
            set { this.isAdministratorField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public UserInfo()
        {
        }
    }
}