using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetInCommonCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public InCommonData Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (InCommonData)this.results[0];
            }
        }

        internal GetInCommonCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}