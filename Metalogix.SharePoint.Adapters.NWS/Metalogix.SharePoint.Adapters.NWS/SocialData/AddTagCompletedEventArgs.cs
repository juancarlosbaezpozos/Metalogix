using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class AddTagCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public SocialTagDetail Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (SocialTagDetail)this.results[0];
            }
        }

        internal AddTagCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}