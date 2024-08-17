using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class BusinessInformation
    {
        private string companyNameField;

        private Guid companyIdField;

        public Guid CompanyId
        {
            get { return this.companyIdField; }
            set { this.companyIdField = value; }
        }

        public string CompanyName
        {
            get { return this.companyNameField; }
            set { this.companyNameField = value; }
        }

        public BusinessInformation()
        {
        }
    }
}