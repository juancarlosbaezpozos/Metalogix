using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "AuthenticationSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Authentication : SoapHttpClientProtocol
    {
        private SendOrPostCallback LoginOperationCompleted;

        private SendOrPostCallback ModeOperationCompleted;

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

        public Authentication()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Extensions_Authentication_Authentication;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/Login",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public LoginResult Login(string username, string password)
        {
            object[] objArray = new object[] { username, password };
            return (LoginResult)base.Invoke("Login", objArray)[0];
        }

        public void LoginAsync(string username, string password)
        {
            this.LoginAsync(username, password, null);
        }

        public void LoginAsync(string username, string password, object userState)
        {
            if (this.LoginOperationCompleted == null)
            {
                this.LoginOperationCompleted = new SendOrPostCallback(this.OnLoginOperationCompleted);
            }

            object[] objArray = new object[] { username, password };
            base.InvokeAsync("Login", objArray, this.LoginOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/Mode",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public AuthenticationMode Mode()
        {
            object[] objArray = base.Invoke("Mode", new object[0]);
            return (AuthenticationMode)objArray[0];
        }

        public void ModeAsync()
        {
            this.ModeAsync(null);
        }

        public void ModeAsync(object userState)
        {
            if (this.ModeOperationCompleted == null)
            {
                this.ModeOperationCompleted = new SendOrPostCallback(this.OnModeOperationCompleted);
            }

            base.InvokeAsync("Mode", new object[0], this.ModeOperationCompleted, userState);
        }

        private void OnLoginOperationCompleted(object arg)
        {
            if (this.LoginCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.LoginCompleted(this,
                    new LoginCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnModeOperationCompleted(object arg)
        {
            if (this.ModeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ModeCompleted(this,
                    new ModeCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        public event LoginCompletedEventHandler LoginCompleted;

        public event ModeCompletedEventHandler ModeCompleted;
    }
}