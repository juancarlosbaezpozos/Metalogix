using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Webs
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetCustomizedPageStatusCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public CustomizedPageStatus Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (CustomizedPageStatus)this.results[0];
            }
        }

        internal GetCustomizedPageStatusCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}