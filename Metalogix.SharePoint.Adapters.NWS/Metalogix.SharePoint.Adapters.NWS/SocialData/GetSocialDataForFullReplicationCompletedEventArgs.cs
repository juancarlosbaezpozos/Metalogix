using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetSocialDataForFullReplicationCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public SocialReplicationData Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (SocialReplicationData)this.results[0];
            }
        }

        internal GetSocialDataForFullReplicationCompletedEventArgs(object[] results, Exception exception,
            bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}