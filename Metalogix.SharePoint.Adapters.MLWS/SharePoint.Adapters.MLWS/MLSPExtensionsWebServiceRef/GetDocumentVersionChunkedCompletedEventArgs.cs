using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.18408")]
    public class GetDocumentVersionChunkedCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public byte[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (byte[])this.results[0];
            }
        }

        internal GetDocumentVersionChunkedCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}