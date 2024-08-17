using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Sites
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetSiteTemplatesCompletedEventArgs : AsyncCompletedEventArgs
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

        public Template[] TemplateList
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (Template[])this.results[1];
            }
        }

        internal GetSiteTemplatesCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}