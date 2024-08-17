using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS.Sites
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "SitesSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Sites : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetSiteTemplatesOperationCompleted;

        private SendOrPostCallback GetUpdatedFormDigestOperationCompleted;

        private SendOrPostCallback ExportWebOperationCompleted;

        private SendOrPostCallback ImportWebOperationCompleted;

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

        public Sites()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Sites_Sites;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/ExportWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public int ExportWeb(string jobName, string webUrl, string dataPath, bool includeSubwebs,
            bool includeUserSecurity, bool overWrite, int cabSize)
        {
            object[] objArray = new object[]
                { jobName, webUrl, dataPath, includeSubwebs, includeUserSecurity, overWrite, cabSize };
            return (int)base.Invoke("ExportWeb", objArray)[0];
        }

        public void ExportWebAsync(string jobName, string webUrl, string dataPath, bool includeSubwebs,
            bool includeUserSecurity, bool overWrite, int cabSize)
        {
            this.ExportWebAsync(jobName, webUrl, dataPath, includeSubwebs, includeUserSecurity, overWrite, cabSize,
                null);
        }

        public void ExportWebAsync(string jobName, string webUrl, string dataPath, bool includeSubwebs,
            bool includeUserSecurity, bool overWrite, int cabSize, object userState)
        {
            if (this.ExportWebOperationCompleted == null)
            {
                this.ExportWebOperationCompleted = new SendOrPostCallback(this.OnExportWebOperationCompleted);
            }

            object[] objArray = new object[]
                { jobName, webUrl, dataPath, includeSubwebs, includeUserSecurity, overWrite, cabSize };
            base.InvokeAsync("ExportWeb", objArray, this.ExportWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetSiteTemplates",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetSiteTemplates(uint LCID, out Template[] TemplateList)
        {
            object[] lCID = new object[] { LCID };
            object[] objArray = base.Invoke("GetSiteTemplates", lCID);
            TemplateList = (Template[])objArray[1];
            return (uint)objArray[0];
        }

        public void GetSiteTemplatesAsync(uint LCID)
        {
            this.GetSiteTemplatesAsync(LCID, null);
        }

        public void GetSiteTemplatesAsync(uint LCID, object userState)
        {
            if (this.GetSiteTemplatesOperationCompleted == null)
            {
                this.GetSiteTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnGetSiteTemplatesOperationCompleted);
            }

            object[] lCID = new object[] { LCID };
            base.InvokeAsync("GetSiteTemplates", lCID, this.GetSiteTemplatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetUpdatedFormDigest",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetUpdatedFormDigest()
        {
            object[] objArray = base.Invoke("GetUpdatedFormDigest", new object[0]);
            return (string)objArray[0];
        }

        public void GetUpdatedFormDigestAsync()
        {
            this.GetUpdatedFormDigestAsync(null);
        }

        public void GetUpdatedFormDigestAsync(object userState)
        {
            if (this.GetUpdatedFormDigestOperationCompleted == null)
            {
                this.GetUpdatedFormDigestOperationCompleted =
                    new SendOrPostCallback(this.OnGetUpdatedFormDigestOperationCompleted);
            }

            base.InvokeAsync("GetUpdatedFormDigest", new object[0], this.GetUpdatedFormDigestOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/ImportWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public int ImportWeb(string jobName, string webUrl, string[] dataFiles, string logPath,
            bool includeUserSecurity, bool overWrite)
        {
            object[] objArray = new object[] { jobName, webUrl, dataFiles, logPath, includeUserSecurity, overWrite };
            return (int)base.Invoke("ImportWeb", objArray)[0];
        }

        public void ImportWebAsync(string jobName, string webUrl, string[] dataFiles, string logPath,
            bool includeUserSecurity, bool overWrite)
        {
            this.ImportWebAsync(jobName, webUrl, dataFiles, logPath, includeUserSecurity, overWrite, null);
        }

        public void ImportWebAsync(string jobName, string webUrl, string[] dataFiles, string logPath,
            bool includeUserSecurity, bool overWrite, object userState)
        {
            if (this.ImportWebOperationCompleted == null)
            {
                this.ImportWebOperationCompleted = new SendOrPostCallback(this.OnImportWebOperationCompleted);
            }

            object[] objArray = new object[] { jobName, webUrl, dataFiles, logPath, includeUserSecurity, overWrite };
            base.InvokeAsync("ImportWeb", objArray, this.ImportWebOperationCompleted, userState);
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

        private void OnExportWebOperationCompleted(object arg)
        {
            if (this.ExportWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ExportWebCompleted(this,
                    new ExportWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteTemplatesOperationCompleted(object arg)
        {
            if (this.GetSiteTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteTemplatesCompleted(this,
                    new GetSiteTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUpdatedFormDigestOperationCompleted(object arg)
        {
            if (this.GetUpdatedFormDigestCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUpdatedFormDigestCompleted(this,
                    new GetUpdatedFormDigestCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnImportWebOperationCompleted(object arg)
        {
            if (this.ImportWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ImportWebCompleted(this,
                    new ImportWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        public event ExportWebCompletedEventHandler ExportWebCompleted;

        public event GetSiteTemplatesCompletedEventHandler GetSiteTemplatesCompleted;

        public event GetUpdatedFormDigestCompletedEventHandler GetUpdatedFormDigestCompleted;

        public event ImportWebCompletedEventHandler ImportWebCompleted;
    }
}