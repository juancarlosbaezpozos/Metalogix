using System;

namespace Metalogix.Licensing.SK
{
    public class BPOSPartnerInfo
    {
        private readonly string _id;

        private readonly string _name;

        private readonly string _trialQuoteUrl;

        private readonly string _payedQuoteUrl;

        public string Id
        {
            get { return this._id; }
        }

        public string Name
        {
            get { return this._name; }
        }

        public string PayedQuoteUrl
        {
            get { return this._payedQuoteUrl; }
        }

        public string TrialQuoteUrl
        {
            get { return this._trialQuoteUrl; }
        }

        public BPOSPartnerInfo(string id, string name, string trialQuoteUrl, string payedQuoteUrl)
        {
            this._id = id;
            this._name = name;
            this._trialQuoteUrl = trialQuoteUrl;
            this._payedQuoteUrl = payedQuoteUrl;
        }
    }
}