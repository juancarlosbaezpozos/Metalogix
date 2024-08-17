using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetContentCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string lastItemIdOnPage
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[1];
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

        internal GetContentCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}