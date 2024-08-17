using Metalogix.MLLicensing.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.LicenseServerOld
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "LicenseServiceSoap", Namespace = "http://www.metalogix.net/licenseservice")]
    public class LicenseService : SoapHttpClientProtocol
    {
        private SendOrPostCallback LogLicenseActivationOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get { return base.Url; }
            set
            {
                if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly &&
                    !this.IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }

                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get { return base.UseDefaultCredentials; }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public LicenseService()
        {
            this.Url = Settings.Default.Metalogix_System_LicenseServer_LicenseService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }

            System.Uri uri = new System.Uri(url);
            if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        [SoapDocumentMethod("http://www.metalogix.net/licenseservice/LogLicenseActivation",
            RequestNamespace = "http://www.metalogix.net/licenseservice",
            ResponseNamespace = "http://www.metalogix.net/licenseservice", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void LogLicenseActivation(string sName, string sLicenseGuid)
        {
            object[] objArray = new object[] { sName, sLicenseGuid };
            base.Invoke("LogLicenseActivation", objArray);
        }

        public void LogLicenseActivationAsync(string sName, string sLicenseGuid)
        {
            this.LogLicenseActivationAsync(sName, sLicenseGuid, null);
        }

        public void LogLicenseActivationAsync(string sName, string sLicenseGuid, object userState)
        {
            if (this.LogLicenseActivationOperationCompleted == null)
            {
                this.LogLicenseActivationOperationCompleted =
                    new SendOrPostCallback(this.OnLogLicenseActivationOperationCompleted);
            }

            object[] objArray = new object[] { sName, sLicenseGuid };
            base.InvokeAsync("LogLicenseActivation", objArray, this.LogLicenseActivationOperationCompleted, userState);
        }

        private void OnLogLicenseActivationOperationCompleted(object arg)
        {
            if (this.LogLicenseActivationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.LogLicenseActivationCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        public event LogLicenseActivationCompletedEventHandler LogLicenseActivationCompleted;
    }
}