using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetURLSegmentsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public bool Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (bool)this.results[0];
            }
        }

        public string strBucketID
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[2];
            }
        }

        public string strItemID
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[4];
            }
        }

        public string strListID
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[3];
            }
        }

        public string strWebID
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[1];
            }
        }

        internal GetURLSegmentsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}