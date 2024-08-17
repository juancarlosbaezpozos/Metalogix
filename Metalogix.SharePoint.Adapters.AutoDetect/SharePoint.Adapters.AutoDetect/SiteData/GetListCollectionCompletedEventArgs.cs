using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.AutoDetect.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetListCollectionCompletedEventArgs : AsyncCompletedEventArgs
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

        public _sList[] vLists
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sList[])this.results[1];
            }
        }

        internal GetListCollectionCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}