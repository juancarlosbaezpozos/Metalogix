using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    public class GenerateLicenseCompletedEventArgs : AsyncCompletedEventArgs
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

        internal GenerateLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}