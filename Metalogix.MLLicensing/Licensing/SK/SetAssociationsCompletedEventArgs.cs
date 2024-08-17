using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    public class SetAssociationsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public Association[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (Association[])this.results[0];
            }
        }

        internal SetAssociationsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}