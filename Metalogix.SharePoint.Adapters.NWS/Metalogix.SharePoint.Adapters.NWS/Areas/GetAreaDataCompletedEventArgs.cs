using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetAreaDataCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public AreaData Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (AreaData)this.results[0];
            }
        }

        internal GetAreaDataCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}