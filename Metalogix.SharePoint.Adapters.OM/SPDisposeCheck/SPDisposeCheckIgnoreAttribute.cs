using System;

namespace SPDisposeCheck
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class SPDisposeCheckIgnoreAttribute : Attribute
    {
        private SPDisposeCheckID _id;

        private string _reason;

        public SPDisposeCheckID Id
        {
            get { return this._id; }
            set { this._id = this.Id; }
        }

        public string Reason
        {
            get { return this._reason; }
            set { this._reason = this.Reason; }
        }

        public SPDisposeCheckIgnoreAttribute(SPDisposeCheckID Id, string Reason)
        {
            this._id = Id;
            this._reason = Reason;
        }
    }
}