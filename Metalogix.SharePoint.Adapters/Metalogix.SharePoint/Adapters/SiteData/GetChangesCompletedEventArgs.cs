using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetChangesCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string CurrentChangeId
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[2];
            }
        }

        public string LastChangeId
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[1];
            }
        }

        public bool moreChanges
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (bool)this.results[3];
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

        internal GetChangesCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}