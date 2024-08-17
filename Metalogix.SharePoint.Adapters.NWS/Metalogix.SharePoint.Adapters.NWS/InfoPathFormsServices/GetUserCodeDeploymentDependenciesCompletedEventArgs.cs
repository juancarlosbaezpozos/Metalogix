using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetUserCodeDeploymentDependenciesCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public UserSolutionActivationStatus Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (UserSolutionActivationStatus)this.results[0];
            }
        }

        internal GetUserCodeDeploymentDependenciesCompletedEventArgs(object[] results, Exception exception,
            bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}