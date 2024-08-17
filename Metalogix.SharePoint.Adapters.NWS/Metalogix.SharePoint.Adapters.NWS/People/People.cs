using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.People
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "PeopleSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class People : SoapHttpClientProtocol
    {
        private SendOrPostCallback IsClaimsModeOperationCompleted;

        private SendOrPostCallback ResolvePrincipalsOperationCompleted;

        private SendOrPostCallback SearchPrincipalsOperationCompleted;

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

        public People()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_People_People;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/IsClaimsMode",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool IsClaimsMode()
        {
            object[] objArray = base.Invoke("IsClaimsMode", new object[0]);
            return (bool)objArray[0];
        }

        public void IsClaimsModeAsync()
        {
            this.IsClaimsModeAsync(null);
        }

        public void IsClaimsModeAsync(object userState)
        {
            if (this.IsClaimsModeOperationCompleted == null)
            {
                this.IsClaimsModeOperationCompleted = new SendOrPostCallback(this.OnIsClaimsModeOperationCompleted);
            }

            base.InvokeAsync("IsClaimsMode", new object[0], this.IsClaimsModeOperationCompleted, userState);
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

        private void OnIsClaimsModeOperationCompleted(object arg)
        {
            if (this.IsClaimsModeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.IsClaimsModeCompleted(this,
                    new IsClaimsModeCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnResolvePrincipalsOperationCompleted(object arg)
        {
            if (this.ResolvePrincipalsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ResolvePrincipalsCompleted(this,
                    new ResolvePrincipalsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSearchPrincipalsOperationCompleted(object arg)
        {
            if (this.SearchPrincipalsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SearchPrincipalsCompleted(this,
                    new SearchPrincipalsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/ResolvePrincipals",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlArrayItem(IsNullable = false)]
        public PrincipalInfo[] ResolvePrincipals(string[] principalKeys, SPPrincipalType principalType,
            bool addToUserInfoList)
        {
            object[] objArray = new object[] { principalKeys, principalType, addToUserInfoList };
            return (PrincipalInfo[])base.Invoke("ResolvePrincipals", objArray)[0];
        }

        public void ResolvePrincipalsAsync(string[] principalKeys, SPPrincipalType principalType,
            bool addToUserInfoList)
        {
            this.ResolvePrincipalsAsync(principalKeys, principalType, addToUserInfoList, null);
        }

        public void ResolvePrincipalsAsync(string[] principalKeys, SPPrincipalType principalType,
            bool addToUserInfoList, object userState)
        {
            if (this.ResolvePrincipalsOperationCompleted == null)
            {
                this.ResolvePrincipalsOperationCompleted =
                    new SendOrPostCallback(this.OnResolvePrincipalsOperationCompleted);
            }

            object[] objArray = new object[] { principalKeys, principalType, addToUserInfoList };
            base.InvokeAsync("ResolvePrincipals", objArray, this.ResolvePrincipalsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/SearchPrincipals",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlArrayItem(IsNullable = false)]
        public PrincipalInfo[] SearchPrincipals(string searchText, int maxResults, SPPrincipalType principalType)
        {
            object[] objArray = new object[] { searchText, maxResults, principalType };
            return (PrincipalInfo[])base.Invoke("SearchPrincipals", objArray)[0];
        }

        public void SearchPrincipalsAsync(string searchText, int maxResults, SPPrincipalType principalType)
        {
            this.SearchPrincipalsAsync(searchText, maxResults, principalType, null);
        }

        public void SearchPrincipalsAsync(string searchText, int maxResults, SPPrincipalType principalType,
            object userState)
        {
            if (this.SearchPrincipalsOperationCompleted == null)
            {
                this.SearchPrincipalsOperationCompleted =
                    new SendOrPostCallback(this.OnSearchPrincipalsOperationCompleted);
            }

            object[] objArray = new object[] { searchText, maxResults, principalType };
            base.InvokeAsync("SearchPrincipals", objArray, this.SearchPrincipalsOperationCompleted, userState);
        }

        public event IsClaimsModeCompletedEventHandler IsClaimsModeCompleted;

        public event ResolvePrincipalsCompletedEventHandler ResolvePrincipalsCompleted;

        public event SearchPrincipalsCompletedEventHandler SearchPrincipalsCompleted;
    }
}