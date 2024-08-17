using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.6.1055.0")]
    public class GetCustomersCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public Customer[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (Customer[])this.results[0];
            }
        }

        internal GetCustomersCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}