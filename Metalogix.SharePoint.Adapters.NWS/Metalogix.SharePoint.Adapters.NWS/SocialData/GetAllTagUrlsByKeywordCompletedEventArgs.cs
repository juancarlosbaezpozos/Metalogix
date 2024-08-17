using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetAllTagUrlsByKeywordCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public SocialUrlDetail[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (SocialUrlDetail[])this.results[0];
            }
        }

        internal GetAllTagUrlsByKeywordCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}