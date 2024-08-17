using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetCommonMembershipsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public MembershipData[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (MembershipData[])this.results[0];
            }
        }

        internal GetCommonMembershipsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}