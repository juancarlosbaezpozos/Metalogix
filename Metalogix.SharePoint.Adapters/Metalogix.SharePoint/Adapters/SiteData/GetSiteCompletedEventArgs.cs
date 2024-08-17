using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetSiteCompletedEventArgs : AsyncCompletedEventArgs
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

        public _sSiteMetadata sSiteMetadata
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sSiteMetadata)this.results[1];
            }
        }

        public string strGroups
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[4];
            }
        }

        public string strUsers
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[3];
            }
        }

        public string[] vGroups
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string[])this.results[5];
            }
        }

        public _sWebWithTime[] vWebs
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sWebWithTime[])this.results[2];
            }
        }

        internal GetSiteCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}