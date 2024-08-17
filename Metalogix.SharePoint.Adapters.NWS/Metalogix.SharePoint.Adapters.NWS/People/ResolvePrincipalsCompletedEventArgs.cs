using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.People
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class ResolvePrincipalsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public PrincipalInfo[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (PrincipalInfo[])this.results[0];
            }
        }

        internal ResolvePrincipalsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}