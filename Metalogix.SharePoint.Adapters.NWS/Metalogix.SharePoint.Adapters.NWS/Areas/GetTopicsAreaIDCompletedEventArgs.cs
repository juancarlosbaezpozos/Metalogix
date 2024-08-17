using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetTopicsAreaIDCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public Guid Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (Guid)this.results[0];
            }
        }

        internal GetTopicsAreaIDCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}