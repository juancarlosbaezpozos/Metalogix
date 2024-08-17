using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.CMWebService
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "CMWebServiceSoap", Namespace = "http://www.metalogix.net/")]
    public class CMWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback IsActivatedOperationCompleted;

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

        public CMWebService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_CMWebSerive_CMWebService;
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

        [SoapDocumentMethod("http://www.metalogix.net/IsActivated", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool IsActivated()
        {
            object[] objArray = base.Invoke("IsActivated", new object[0]);
            return (bool)objArray[0];
        }

        public void IsActivatedAsync()
        {
            this.IsActivatedAsync(null);
        }

        public void IsActivatedAsync(object userState)
        {
            if (this.IsActivatedOperationCompleted == null)
            {
                this.IsActivatedOperationCompleted = new SendOrPostCallback(this.OnIsActivatedOperationCompleted);
            }

            base.InvokeAsync("IsActivated", new object[0], this.IsActivatedOperationCompleted, userState);
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

        private void OnIsActivatedOperationCompleted(object arg)
        {
            if (this.IsActivatedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.IsActivatedCompleted(this,
                    new IsActivatedCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        public event IsActivatedCompletedEventHandler IsActivatedCompleted;
    }
}