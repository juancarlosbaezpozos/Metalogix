using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.Versions
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "VersionsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Versions : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetVersionsOperationCompleted;

        private SendOrPostCallback RestoreVersionOperationCompleted;

        private SendOrPostCallback DeleteVersionOperationCompleted;

        private SendOrPostCallback DeleteAllVersionsOperationCompleted;

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

        public Versions()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Versions_Versions;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteAllVersions",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode DeleteAllVersions(string fileName)
        {
            object[] objArray = new object[] { fileName };
            return (XmlNode)base.Invoke("DeleteAllVersions", objArray)[0];
        }

        public void DeleteAllVersionsAsync(string fileName)
        {
            this.DeleteAllVersionsAsync(fileName, null);
        }

        public void DeleteAllVersionsAsync(string fileName, object userState)
        {
            if (this.DeleteAllVersionsOperationCompleted == null)
            {
                this.DeleteAllVersionsOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteAllVersionsOperationCompleted);
            }

            object[] objArray = new object[] { fileName };
            base.InvokeAsync("DeleteAllVersions", objArray, this.DeleteAllVersionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteVersion",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode DeleteVersion(string fileName, string fileVersion)
        {
            object[] objArray = new object[] { fileName, fileVersion };
            return (XmlNode)base.Invoke("DeleteVersion", objArray)[0];
        }

        public void DeleteVersionAsync(string fileName, string fileVersion)
        {
            this.DeleteVersionAsync(fileName, fileVersion, null);
        }

        public void DeleteVersionAsync(string fileName, string fileVersion, object userState)
        {
            if (this.DeleteVersionOperationCompleted == null)
            {
                this.DeleteVersionOperationCompleted = new SendOrPostCallback(this.OnDeleteVersionOperationCompleted);
            }

            object[] objArray = new object[] { fileName, fileVersion };
            base.InvokeAsync("DeleteVersion", objArray, this.DeleteVersionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetVersions",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetVersions(string fileName)
        {
            object[] objArray = new object[] { fileName };
            return (XmlNode)base.Invoke("GetVersions", objArray)[0];
        }

        public void GetVersionsAsync(string fileName)
        {
            this.GetVersionsAsync(fileName, null);
        }

        public void GetVersionsAsync(string fileName, object userState)
        {
            if (this.GetVersionsOperationCompleted == null)
            {
                this.GetVersionsOperationCompleted = new SendOrPostCallback(this.OnGetVersionsOperationCompleted);
            }

            object[] objArray = new object[] { fileName };
            base.InvokeAsync("GetVersions", objArray, this.GetVersionsOperationCompleted, userState);
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

        private void OnDeleteAllVersionsOperationCompleted(object arg)
        {
            if (this.DeleteAllVersionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAllVersionsCompleted(this,
                    new DeleteAllVersionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteVersionOperationCompleted(object arg)
        {
            if (this.DeleteVersionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteVersionCompleted(this,
                    new DeleteVersionCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetVersionsOperationCompleted(object arg)
        {
            if (this.GetVersionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetVersionsCompleted(this,
                    new GetVersionsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnRestoreVersionOperationCompleted(object arg)
        {
            if (this.RestoreVersionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RestoreVersionCompleted(this,
                    new RestoreVersionCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/RestoreVersion",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode RestoreVersion(string fileName, string fileVersion)
        {
            object[] objArray = new object[] { fileName, fileVersion };
            return (XmlNode)base.Invoke("RestoreVersion", objArray)[0];
        }

        public void RestoreVersionAsync(string fileName, string fileVersion)
        {
            this.RestoreVersionAsync(fileName, fileVersion, null);
        }

        public void RestoreVersionAsync(string fileName, string fileVersion, object userState)
        {
            if (this.RestoreVersionOperationCompleted == null)
            {
                this.RestoreVersionOperationCompleted = new SendOrPostCallback(this.OnRestoreVersionOperationCompleted);
            }

            object[] objArray = new object[] { fileName, fileVersion };
            base.InvokeAsync("RestoreVersion", objArray, this.RestoreVersionOperationCompleted, userState);
        }

        public event DeleteAllVersionsCompletedEventHandler DeleteAllVersionsCompleted;

        public event DeleteVersionCompletedEventHandler DeleteVersionCompleted;

        public event GetVersionsCompletedEventHandler GetVersionsCompleted;

        public event RestoreVersionCompletedEventHandler RestoreVersionCompleted;
    }
}