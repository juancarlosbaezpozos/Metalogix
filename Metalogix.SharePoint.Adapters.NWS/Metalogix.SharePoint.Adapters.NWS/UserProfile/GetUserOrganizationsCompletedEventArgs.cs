using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetUserOrganizationsCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public OrganizationProfileData[] Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (OrganizationProfileData[])this.results[0];
            }
        }

        internal GetUserOrganizationsCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}