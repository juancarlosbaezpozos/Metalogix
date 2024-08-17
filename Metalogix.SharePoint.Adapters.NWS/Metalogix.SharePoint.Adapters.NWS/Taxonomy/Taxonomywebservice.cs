using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS.Taxonomy
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "Taxonomy web serviceSoap",
        Namespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/")]
    public class Taxonomywebservice : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetTermSetsOperationCompleted;

        private SendOrPostCallback GetTermsByLabelOperationCompleted;

        private SendOrPostCallback AddTermsOperationCompleted;

        private SendOrPostCallback GetKeywordTermsByGuidsOperationCompleted;

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

        public Taxonomywebservice()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Taxonomy_Taxonomy_x0020_web_x0020_service;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/taxonomy/soap/AddTerms",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddTerms(Guid sharedServiceId, Guid termSetId, int lcid, string newTerms)
        {
            object[] objArray = new object[] { sharedServiceId, termSetId, lcid, newTerms };
            return (string)base.Invoke("AddTerms", objArray)[0];
        }

        public void AddTermsAsync(Guid sharedServiceId, Guid termSetId, int lcid, string newTerms)
        {
            this.AddTermsAsync(sharedServiceId, termSetId, lcid, newTerms, null);
        }

        public void AddTermsAsync(Guid sharedServiceId, Guid termSetId, int lcid, string newTerms, object userState)
        {
            if (this.AddTermsOperationCompleted == null)
            {
                this.AddTermsOperationCompleted = new SendOrPostCallback(this.OnAddTermsOperationCompleted);
            }

            object[] objArray = new object[] { sharedServiceId, termSetId, lcid, newTerms };
            base.InvokeAsync("AddTerms", objArray, this.AddTermsOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/taxonomy/soap/GetKeywordTermsByGuids",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetKeywordTermsByGuids(string termIds, int lcid)
        {
            object[] objArray = new object[] { termIds, lcid };
            return (string)base.Invoke("GetKeywordTermsByGuids", objArray)[0];
        }

        public void GetKeywordTermsByGuidsAsync(string termIds, int lcid)
        {
            this.GetKeywordTermsByGuidsAsync(termIds, lcid, null);
        }

        public void GetKeywordTermsByGuidsAsync(string termIds, int lcid, object userState)
        {
            if (this.GetKeywordTermsByGuidsOperationCompleted == null)
            {
                this.GetKeywordTermsByGuidsOperationCompleted =
                    new SendOrPostCallback(this.OnGetKeywordTermsByGuidsOperationCompleted);
            }

            object[] objArray = new object[] { termIds, lcid };
            base.InvokeAsync("GetKeywordTermsByGuids", objArray, this.GetKeywordTermsByGuidsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/taxonomy/soap/GetTermsByLabel",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermsByLabel(string label, int lcid, StringMatchOption matchOption, int resultCollectionSize,
            string termIds, bool addIfNotFound)
        {
            object[] objArray = new object[] { label, lcid, matchOption, resultCollectionSize, termIds, addIfNotFound };
            return (string)base.Invoke("GetTermsByLabel", objArray)[0];
        }

        public void GetTermsByLabelAsync(string label, int lcid, StringMatchOption matchOption,
            int resultCollectionSize, string termIds, bool addIfNotFound)
        {
            this.GetTermsByLabelAsync(label, lcid, matchOption, resultCollectionSize, termIds, addIfNotFound, null);
        }

        public void GetTermsByLabelAsync(string label, int lcid, StringMatchOption matchOption,
            int resultCollectionSize, string termIds, bool addIfNotFound, object userState)
        {
            if (this.GetTermsByLabelOperationCompleted == null)
            {
                this.GetTermsByLabelOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermsByLabelOperationCompleted);
            }

            object[] objArray = new object[] { label, lcid, matchOption, resultCollectionSize, termIds, addIfNotFound };
            base.InvokeAsync("GetTermsByLabel", objArray, this.GetTermsByLabelOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/taxonomy/soap/GetTermSets",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/taxonomy/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermSets(string sharedServiceIds, string termSetIds, int lcid, string clientTimeStamps,
            string clientVersions, out string serverTermSetTimeStampXml)
        {
            object[] objArray = new object[] { sharedServiceIds, termSetIds, lcid, clientTimeStamps, clientVersions };
            object[] objArray1 = base.Invoke("GetTermSets", objArray);
            serverTermSetTimeStampXml = (string)objArray1[1];
            return (string)objArray1[0];
        }

        public void GetTermSetsAsync(string sharedServiceIds, string termSetIds, int lcid, string clientTimeStamps,
            string clientVersions)
        {
            this.GetTermSetsAsync(sharedServiceIds, termSetIds, lcid, clientTimeStamps, clientVersions, null);
        }

        public void GetTermSetsAsync(string sharedServiceIds, string termSetIds, int lcid, string clientTimeStamps,
            string clientVersions, object userState)
        {
            if (this.GetTermSetsOperationCompleted == null)
            {
                this.GetTermSetsOperationCompleted = new SendOrPostCallback(this.OnGetTermSetsOperationCompleted);
            }

            object[] objArray = new object[] { sharedServiceIds, termSetIds, lcid, clientTimeStamps, clientVersions };
            base.InvokeAsync("GetTermSets", objArray, this.GetTermSetsOperationCompleted, userState);
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

        private void OnAddTermsOperationCompleted(object arg)
        {
            if (this.AddTermsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTermsCompleted(this,
                    new AddTermsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetKeywordTermsByGuidsOperationCompleted(object arg)
        {
            if (this.GetKeywordTermsByGuidsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetKeywordTermsByGuidsCompleted(this,
                    new GetKeywordTermsByGuidsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermsByLabelOperationCompleted(object arg)
        {
            if (this.GetTermsByLabelCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermsByLabelCompleted(this,
                    new GetTermsByLabelCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermSetsOperationCompleted(object arg)
        {
            if (this.GetTermSetsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermSetsCompleted(this,
                    new GetTermSetsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        public event AddTermsCompletedEventHandler AddTermsCompleted;

        public event GetKeywordTermsByGuidsCompletedEventHandler GetKeywordTermsByGuidsCompleted;

        public event GetTermsByLabelCompletedEventHandler GetTermsByLabelCompleted;

        public event GetTermSetsCompletedEventHandler GetTermSetsCompleted;
    }
}