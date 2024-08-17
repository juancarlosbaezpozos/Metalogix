using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetCommentsOfUserCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public SocialCommentDetail[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (SocialCommentDetail[])this.results[0];
            }
        }

        internal GetCommentsOfUserCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}