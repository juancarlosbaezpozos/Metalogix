using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class SetFormsForListItemCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public DesignCheckerInformation Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (DesignCheckerInformation)this.results[0];
            }
        }

        internal SetFormsForListItemCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}