using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetWebCompletedEventArgs : AsyncCompletedEventArgs
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

        public string strRoles
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string)this.results[5];
            }
        }

        public _sWebMetadata sWebMetadata
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sWebMetadata)this.results[1];
            }
        }

        public _sFPUrl[] vFPUrls
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sFPUrl[])this.results[4];
            }
        }

        public _sListWithTime[] vLists
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (_sListWithTime[])this.results[3];
            }
        }

        public string[] vRolesGroups
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string[])this.results[7];
            }
        }

        public string[] vRolesUsers
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (string[])this.results[6];
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

        internal GetWebCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}