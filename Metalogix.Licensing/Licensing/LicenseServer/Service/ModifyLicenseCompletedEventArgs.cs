using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.6.1055.0")]
    public class ModifyLicenseCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public LicenseInfo Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (LicenseInfo)this.results[0];
            }
        }

        internal ModifyLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}