using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.6.1055.0")]
    public class UpdateLicenseCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public LicenseInfoResponse Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (LicenseInfoResponse)this.results[0];
            }
        }

        internal UpdateLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}