using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetSiteAndWebCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public uint Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (uint)this.results[0];
            }
        }

        public string strSite
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[1];
            }
        }

        public string strWeb
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[2];
            }
        }

        internal GetSiteAndWebCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}