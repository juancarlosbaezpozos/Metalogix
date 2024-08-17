using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.AutoDetect.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetListCompletedEventArgs : AsyncCompletedEventArgs
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

        public _sListMetadata sListMetadata
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sListMetadata)this.results[1];
            }
        }

        public _sProperty[] vProperties
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sProperty[])this.results[2];
            }
        }

        internal GetListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}