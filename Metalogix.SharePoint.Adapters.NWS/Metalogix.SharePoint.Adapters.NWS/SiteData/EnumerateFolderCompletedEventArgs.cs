using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class EnumerateFolderCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public uint Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (uint)this.results[0];
            }
        }

        public _sFPUrl[] vUrls
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sFPUrl[])this.results[1];
            }
        }

        internal EnumerateFolderCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}