using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Authentication
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class ModeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public AuthenticationMode Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (AuthenticationMode)this.results[0];
            }
        }

        internal ModeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(
            exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}