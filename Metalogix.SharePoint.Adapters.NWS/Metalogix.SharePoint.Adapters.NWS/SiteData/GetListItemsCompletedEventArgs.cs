using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetListItemsCompletedEventArgs : AsyncCompletedEventArgs
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

        internal GetListItemsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}