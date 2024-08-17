using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.18408")]
    public class DeleteSP2013WorkflowsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[0];
            }
        }

        internal DeleteSP2013WorkflowsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}