using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.6.1055.0")]
    public class GetLicenseDataCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public LicenseDataResponse Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (LicenseDataResponse)this.results[0];
            }
        }

        internal GetLicenseDataCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}