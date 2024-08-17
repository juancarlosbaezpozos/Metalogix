using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.18408")]
    public class AddDocumentOptimisticallyCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public DataSet fieldsLookupCache
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (DataSet)this.results[1];
            }
        }

        public string Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[0];
            }
        }

        internal AddDocumentOptimisticallyCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}