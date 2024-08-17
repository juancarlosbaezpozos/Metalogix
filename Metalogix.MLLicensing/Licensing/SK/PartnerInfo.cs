using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = "http://license.metalogix.com/webservices/")]
    public class PartnerInfo
    {
        private int idField;

        private string partnerIdField;

        private string companyNameField;

        private string addressField;

        private string contactNameField;

        private string contactTelField;

        private string contactMailField;

        private string trialQuoteUrlField;

        private string paidQuoteUrlField;

        private Guid? crmPartnerGuidField;

        public string Address
        {
            get { return this.addressField; }
            set { this.addressField = value; }
        }

        public string CompanyName
        {
            get { return this.companyNameField; }
            set { this.companyNameField = value; }
        }

        public string ContactMail
        {
            get { return this.contactMailField; }
            set { this.contactMailField = value; }
        }

        public string ContactName
        {
            get { return this.contactNameField; }
            set { this.contactNameField = value; }
        }

        public string ContactTel
        {
            get { return this.contactTelField; }
            set { this.contactTelField = value; }
        }

        [XmlElement(IsNullable = true)]
        public Guid? CrmPartnerGuid
        {
            get { return this.crmPartnerGuidField; }
            set { this.crmPartnerGuidField = value; }
        }

        public int Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string PaidQuoteUrl
        {
            get { return this.paidQuoteUrlField; }
            set { this.paidQuoteUrlField = value; }
        }

        public string PartnerId
        {
            get { return this.partnerIdField; }
            set { this.partnerIdField = value; }
        }

        public string TrialQuoteUrl
        {
            get { return this.trialQuoteUrlField; }
            set { this.trialQuoteUrlField = value; }
        }

        public PartnerInfo()
        {
        }
    }
}