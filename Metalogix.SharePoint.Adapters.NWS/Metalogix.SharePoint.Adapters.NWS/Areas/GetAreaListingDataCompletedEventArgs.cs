using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetAreaListingDataCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public AreaListingData Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (AreaListingData)this.results[0];
            }
        }

        internal GetAreaListingDataCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}