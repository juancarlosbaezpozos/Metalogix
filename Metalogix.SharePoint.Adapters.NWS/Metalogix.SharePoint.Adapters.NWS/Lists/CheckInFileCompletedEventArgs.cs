using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Lists
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class CheckInFileCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public bool Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (bool)this.results[0];
            }
        }

        internal CheckInFileCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}