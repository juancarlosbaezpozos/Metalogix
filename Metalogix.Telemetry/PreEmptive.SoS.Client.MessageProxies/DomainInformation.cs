using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class DomainInformation
    {
        private string domainField;

        private string domainRoleField;

        private bool partOfDomainField;

        public string Domain
        {
            get { return this.domainField; }
            set { this.domainField = value; }
        }

        public string DomainRole
        {
            get { return this.domainRoleField; }
            set { this.domainRoleField = value; }
        }

        public bool PartOfDomain
        {
            get { return this.partOfDomainField; }
            set { this.partOfDomainField = value; }
        }

        public DomainInformation()
        {
        }
    }
}