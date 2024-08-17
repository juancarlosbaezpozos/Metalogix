using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    public class GetPartnerCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public PartnerInfo Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (PartnerInfo)this.results[0];
            }
        }

        internal GetPartnerCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}