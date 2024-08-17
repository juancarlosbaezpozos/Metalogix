using System;

namespace Metalogix.Licensing.SK
{
    public class BPOSSubAssoc
    {
        private readonly string _id;

        private readonly string _guid;

        private readonly long _seats;

        private readonly DateTime? _associationDate;

        private readonly BPOSSubscriptionStatus _status;

        private readonly string _company;

        public DateTime? AssociationDate
        {
            get { return this._associationDate; }
        }

        public string Company
        {
            get { return this._company; }
        }

        public string Guid
        {
            get { return this._guid; }
        }

        public string ID
        {
            get { return this._id; }
        }

        public long Seats
        {
            get { return this._seats; }
        }

        public BPOSSubscriptionStatus Status
        {
            get { return this._status; }
        }

        public BPOSSubAssoc(string id, string guid, long seats) : this(id, guid, seats, null,
            BPOSSubscriptionStatus.Unknown, null)
        {
        }

        public BPOSSubAssoc(string id, string guid, long seats, DateTime? associationDate,
            BPOSSubscriptionStatus status, string company)
        {
            this._id = id;
            this._guid = guid;
            this._seats = seats;
            this._associationDate = associationDate;
            this._status = status;
            this._company = company;
        }

        internal static BPOSSubAssoc Create(Association a)
        {
            BPOSSubscriptionStatus bPOSSubscriptionStatu;
            switch (a.Status)
            {
                case AssociationStatus.NPOR:
                {
                    bPOSSubscriptionStatu = BPOSSubscriptionStatus.NonPartner;
                    break;
                }
                case AssociationStatus.POR:
                {
                    bPOSSubscriptionStatu = BPOSSubscriptionStatus.Partner;
                    break;
                }
                case AssociationStatus.Invalid:
                {
                    bPOSSubscriptionStatu = BPOSSubscriptionStatus.Invalid;
                    break;
                }
                case AssociationStatus.Purchased:
                {
                    bPOSSubscriptionStatu = BPOSSubscriptionStatus.Purchased;
                    break;
                }
                case AssociationStatus.Pending:
                {
                    bPOSSubscriptionStatu = BPOSSubscriptionStatus.PartnerPending;
                    break;
                }
                default:
                {
                    throw new Exception("Invalid status presented in Association");
                }
            }

            string subscriptionID = a.SubscriptionID;
            System.Guid subscriptionGUID = a.SubscriptionGUID;
            return new BPOSSubAssoc(subscriptionID, subscriptionGUID.ToString(), a.Seats, a.Associated,
                bPOSSubscriptionStatu, a.Company);
        }

        public override string ToString()
        {
            object[] objArray = new object[]
                { this._id, this._guid, this._seats, this._associationDate, this._status, this._company };
            return string.Format("ID={0}, Guid={1}, Seats={2}, AssociationDate={3}, Status={4}, Company={5}", objArray);
        }
    }
}