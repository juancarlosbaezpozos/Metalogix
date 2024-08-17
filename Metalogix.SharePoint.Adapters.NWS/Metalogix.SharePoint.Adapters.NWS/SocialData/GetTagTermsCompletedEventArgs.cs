using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetTagTermsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public SocialTermDetail[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (SocialTermDetail[])this.results[0];
            }
        }

        internal GetTagTermsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}