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
    public class Association
    {
        private string subscriptionIDField;

        private Guid subscriptionGUIDField;

        private AssociationStatus statusField;

        private long seatsField;

        private DateTime? associatedField;

        private string companyField;

        private string currencyField;

        private string estMinFYFContribField;

        private string monthlyNetUnitPriceField;

        private string subscriptionTypeField;

        private string partnerIdField;

        [XmlElement(IsNullable = true)]
        public DateTime? Associated
        {
            get { return this.associatedField; }
            set { this.associatedField = value; }
        }

        public string Company
        {
            get { return this.companyField; }
            set { this.companyField = value; }
        }

        public string Currency
        {
            get { return this.currencyField; }
            set { this.currencyField = value; }
        }

        public string EstMinFYFContrib
        {
            get { return this.estMinFYFContribField; }
            set { this.estMinFYFContribField = value; }
        }

        public string MonthlyNetUnitPrice
        {
            get { return this.monthlyNetUnitPriceField; }
            set { this.monthlyNetUnitPriceField = value; }
        }

        public string PartnerId
        {
            get { return this.partnerIdField; }
            set { this.partnerIdField = value; }
        }

        public long Seats
        {
            get { return this.seatsField; }
            set { this.seatsField = value; }
        }

        public AssociationStatus Status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        public Guid SubscriptionGUID
        {
            get { return this.subscriptionGUIDField; }
            set { this.subscriptionGUIDField = value; }
        }

        public string SubscriptionID
        {
            get { return this.subscriptionIDField; }
            set { this.subscriptionIDField = value; }
        }

        public string SubscriptionType
        {
            get { return this.subscriptionTypeField; }
            set { this.subscriptionTypeField = value; }
        }

        public Association()
        {
        }

        internal static Association Create(BPOSSubAssoc assoc)
        {
            AssociationStatus associationStatu;
            switch (assoc.Status)
            {
                case BPOSSubscriptionStatus.NonPartner:
                {
                    associationStatu = AssociationStatus.NPOR;
                    break;
                }
                case BPOSSubscriptionStatus.Partner:
                {
                    associationStatu = AssociationStatus.POR;
                    break;
                }
                case BPOSSubscriptionStatus.PartnerPending:
                {
                    associationStatu = AssociationStatus.Pending;
                    break;
                }
                case BPOSSubscriptionStatus.Invalid:
                case BPOSSubscriptionStatus.Unknown:
                {
                    associationStatu = AssociationStatus.Invalid;
                    break;
                }
                case BPOSSubscriptionStatus.Purchased:
                {
                    associationStatu = AssociationStatus.Purchased;
                    break;
                }
                default:
                {
                    throw new Exception("Invalid status presented in Association");
                }
            }

            Association association = new Association()
            {
                SubscriptionGUID = new Guid(assoc.Guid),
                SubscriptionID = assoc.ID,
                Seats = assoc.Seats,
                Associated = assoc.AssociationDate,
                Status = associationStatu
            };
            return association;
        }
    }
}