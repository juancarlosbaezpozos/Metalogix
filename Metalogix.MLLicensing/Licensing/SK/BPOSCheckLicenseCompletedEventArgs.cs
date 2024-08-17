using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    public class BPOSCheckLicenseCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public BPOSLicenseInfo Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (BPOSLicenseInfo)this.results[0];
            }
        }

        internal BPOSCheckLicenseCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}