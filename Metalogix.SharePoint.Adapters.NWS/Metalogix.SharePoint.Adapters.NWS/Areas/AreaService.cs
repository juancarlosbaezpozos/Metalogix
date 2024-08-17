using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "AreaServiceSoap",
        Namespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/")]
    public class AreaService : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetHomeAreaIDOperationCompleted;

        private SendOrPostCallback GetNewsAreaIDOperationCompleted;

        private SendOrPostCallback GetTopicsAreaIDOperationCompleted;

        private SendOrPostCallback GetDocumentsAreaIDOperationCompleted;

        private SendOrPostCallback GetSearchAreaIDOperationCompleted;

        private SendOrPostCallback GetMySiteAreaIDOperationCompleted;

        private SendOrPostCallback GetKeywordsAreaIDOperationCompleted;

        private SendOrPostCallback GetSitesDirectoryAreaIDOperationCompleted;

        private SendOrPostCallback GetSubAreasOperationCompleted;

        private SendOrPostCallback GetAreaListingsOperationCompleted;

        private SendOrPostCallback GetAreaDataOperationCompleted;

        private SendOrPostCallback GetAreaListingDataOperationCompleted;

        private SendOrPostCallback CreateAreaOperationCompleted;

        private SendOrPostCallback CreateAreaListingOperationCompleted;

        private SendOrPostCallback SetAreaDataOperationCompleted;

        private SendOrPostCallback SetAreaListingDataOperationCompleted;

        private SendOrPostCallback DeleteAreaOperationCompleted;

        private SendOrPostCallback DeleteAreaListingOperationCompleted;

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

        public AreaService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_AreaService_AreaService;
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

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/CreateArea",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid CreateArea(Guid ParentID, string strName, string strTemplate)
        {
            object[] parentID = new object[] { ParentID, strName, strTemplate };
            return (Guid)base.Invoke("CreateArea", parentID)[0];
        }

        public void CreateAreaAsync(Guid ParentID, string strName, string strTemplate)
        {
            this.CreateAreaAsync(ParentID, strName, strTemplate, null);
        }

        public void CreateAreaAsync(Guid ParentID, string strName, string strTemplate, object userState)
        {
            if (this.CreateAreaOperationCompleted == null)
            {
                this.CreateAreaOperationCompleted = new SendOrPostCallback(this.OnCreateAreaOperationCompleted);
            }

            object[] parentID = new object[] { ParentID, strName, strTemplate };
            base.InvokeAsync("CreateArea", parentID, this.CreateAreaOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/CreateAreaListing",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid CreateAreaListing(Guid ParentID, string strTitle, string strDescription, ListingType type,
            string strUrl)
        {
            object[] parentID = new object[] { ParentID, strTitle, strDescription, type, strUrl };
            return (Guid)base.Invoke("CreateAreaListing", parentID)[0];
        }

        public void CreateAreaListingAsync(Guid ParentID, string strTitle, string strDescription, ListingType type,
            string strUrl)
        {
            this.CreateAreaListingAsync(ParentID, strTitle, strDescription, type, strUrl, null);
        }

        public void CreateAreaListingAsync(Guid ParentID, string strTitle, string strDescription, ListingType type,
            string strUrl, object userState)
        {
            if (this.CreateAreaListingOperationCompleted == null)
            {
                this.CreateAreaListingOperationCompleted =
                    new SendOrPostCallback(this.OnCreateAreaListingOperationCompleted);
            }

            object[] parentID = new object[] { ParentID, strTitle, strDescription, type, strUrl };
            base.InvokeAsync("CreateAreaListing", parentID, this.CreateAreaListingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/DeleteArea",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteArea(Guid AreaID)
        {
            object[] areaID = new object[] { AreaID };
            base.Invoke("DeleteArea", areaID);
        }

        public void DeleteAreaAsync(Guid AreaID)
        {
            this.DeleteAreaAsync(AreaID, null);
        }

        public void DeleteAreaAsync(Guid AreaID, object userState)
        {
            if (this.DeleteAreaOperationCompleted == null)
            {
                this.DeleteAreaOperationCompleted = new SendOrPostCallback(this.OnDeleteAreaOperationCompleted);
            }

            object[] areaID = new object[] { AreaID };
            base.InvokeAsync("DeleteArea", areaID, this.DeleteAreaOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/DeleteAreaListing",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteAreaListing(Guid AreaListingID)
        {
            object[] areaListingID = new object[] { AreaListingID };
            base.Invoke("DeleteAreaListing", areaListingID);
        }

        public void DeleteAreaListingAsync(Guid AreaListingID)
        {
            this.DeleteAreaListingAsync(AreaListingID, null);
        }

        public void DeleteAreaListingAsync(Guid AreaListingID, object userState)
        {
            if (this.DeleteAreaListingOperationCompleted == null)
            {
                this.DeleteAreaListingOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteAreaListingOperationCompleted);
            }

            object[] areaListingID = new object[] { AreaListingID };
            base.InvokeAsync("DeleteAreaListing", areaListingID, this.DeleteAreaListingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetAreaData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AreaData GetAreaData(Guid AreaID)
        {
            object[] areaID = new object[] { AreaID };
            return (AreaData)base.Invoke("GetAreaData", areaID)[0];
        }

        public void GetAreaDataAsync(Guid AreaID)
        {
            this.GetAreaDataAsync(AreaID, null);
        }

        public void GetAreaDataAsync(Guid AreaID, object userState)
        {
            if (this.GetAreaDataOperationCompleted == null)
            {
                this.GetAreaDataOperationCompleted = new SendOrPostCallback(this.OnGetAreaDataOperationCompleted);
            }

            object[] areaID = new object[] { AreaID };
            base.InvokeAsync("GetAreaData", areaID, this.GetAreaDataOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetAreaListingData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AreaListingData GetAreaListingData(Guid AreaListingID)
        {
            object[] areaListingID = new object[] { AreaListingID };
            return (AreaListingData)base.Invoke("GetAreaListingData", areaListingID)[0];
        }

        public void GetAreaListingDataAsync(Guid AreaListingID)
        {
            this.GetAreaListingDataAsync(AreaListingID, null);
        }

        public void GetAreaListingDataAsync(Guid AreaListingID, object userState)
        {
            if (this.GetAreaListingDataOperationCompleted == null)
            {
                this.GetAreaListingDataOperationCompleted =
                    new SendOrPostCallback(this.OnGetAreaListingDataOperationCompleted);
            }

            object[] areaListingID = new object[] { AreaListingID };
            base.InvokeAsync("GetAreaListingData", areaListingID, this.GetAreaListingDataOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetAreaListings",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid[] GetAreaListings(Guid ParentID)
        {
            object[] parentID = new object[] { ParentID };
            return (Guid[])base.Invoke("GetAreaListings", parentID)[0];
        }

        public void GetAreaListingsAsync(Guid ParentID)
        {
            this.GetAreaListingsAsync(ParentID, null);
        }

        public void GetAreaListingsAsync(Guid ParentID, object userState)
        {
            if (this.GetAreaListingsOperationCompleted == null)
            {
                this.GetAreaListingsOperationCompleted =
                    new SendOrPostCallback(this.OnGetAreaListingsOperationCompleted);
            }

            object[] parentID = new object[] { ParentID };
            base.InvokeAsync("GetAreaListings", parentID, this.GetAreaListingsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetDocumentsAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetDocumentsAreaID()
        {
            object[] objArray = base.Invoke("GetDocumentsAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetDocumentsAreaIDAsync()
        {
            this.GetDocumentsAreaIDAsync(null);
        }

        public void GetDocumentsAreaIDAsync(object userState)
        {
            if (this.GetDocumentsAreaIDOperationCompleted == null)
            {
                this.GetDocumentsAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentsAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetDocumentsAreaID", new object[0], this.GetDocumentsAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetHomeAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetHomeAreaID()
        {
            object[] objArray = base.Invoke("GetHomeAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetHomeAreaIDAsync()
        {
            this.GetHomeAreaIDAsync(null);
        }

        public void GetHomeAreaIDAsync(object userState)
        {
            if (this.GetHomeAreaIDOperationCompleted == null)
            {
                this.GetHomeAreaIDOperationCompleted = new SendOrPostCallback(this.OnGetHomeAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetHomeAreaID", new object[0], this.GetHomeAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetKeywordsAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetKeywordsAreaID()
        {
            object[] objArray = base.Invoke("GetKeywordsAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetKeywordsAreaIDAsync()
        {
            this.GetKeywordsAreaIDAsync(null);
        }

        public void GetKeywordsAreaIDAsync(object userState)
        {
            if (this.GetKeywordsAreaIDOperationCompleted == null)
            {
                this.GetKeywordsAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetKeywordsAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetKeywordsAreaID", new object[0], this.GetKeywordsAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetMySiteAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetMySiteAreaID()
        {
            object[] objArray = base.Invoke("GetMySiteAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetMySiteAreaIDAsync()
        {
            this.GetMySiteAreaIDAsync(null);
        }

        public void GetMySiteAreaIDAsync(object userState)
        {
            if (this.GetMySiteAreaIDOperationCompleted == null)
            {
                this.GetMySiteAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetMySiteAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetMySiteAreaID", new object[0], this.GetMySiteAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetNewsAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetNewsAreaID()
        {
            object[] objArray = base.Invoke("GetNewsAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetNewsAreaIDAsync()
        {
            this.GetNewsAreaIDAsync(null);
        }

        public void GetNewsAreaIDAsync(object userState)
        {
            if (this.GetNewsAreaIDOperationCompleted == null)
            {
                this.GetNewsAreaIDOperationCompleted = new SendOrPostCallback(this.OnGetNewsAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetNewsAreaID", new object[0], this.GetNewsAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetSearchAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetSearchAreaID()
        {
            object[] objArray = base.Invoke("GetSearchAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetSearchAreaIDAsync()
        {
            this.GetSearchAreaIDAsync(null);
        }

        public void GetSearchAreaIDAsync(object userState)
        {
            if (this.GetSearchAreaIDOperationCompleted == null)
            {
                this.GetSearchAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetSearchAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetSearchAreaID", new object[0], this.GetSearchAreaIDOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetSitesDirectoryAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetSitesDirectoryAreaID()
        {
            object[] objArray = base.Invoke("GetSitesDirectoryAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetSitesDirectoryAreaIDAsync()
        {
            this.GetSitesDirectoryAreaIDAsync(null);
        }

        public void GetSitesDirectoryAreaIDAsync(object userState)
        {
            if (this.GetSitesDirectoryAreaIDOperationCompleted == null)
            {
                this.GetSitesDirectoryAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetSitesDirectoryAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetSitesDirectoryAreaID", new object[0], this.GetSitesDirectoryAreaIDOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetSubAreas",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid[] GetSubAreas(Guid ParentID)
        {
            object[] parentID = new object[] { ParentID };
            return (Guid[])base.Invoke("GetSubAreas", parentID)[0];
        }

        public void GetSubAreasAsync(Guid ParentID)
        {
            this.GetSubAreasAsync(ParentID, null);
        }

        public void GetSubAreasAsync(Guid ParentID, object userState)
        {
            if (this.GetSubAreasOperationCompleted == null)
            {
                this.GetSubAreasOperationCompleted = new SendOrPostCallback(this.OnGetSubAreasOperationCompleted);
            }

            object[] parentID = new object[] { ParentID };
            base.InvokeAsync("GetSubAreas", parentID, this.GetSubAreasOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/GetTopicsAreaID",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid GetTopicsAreaID()
        {
            object[] objArray = base.Invoke("GetTopicsAreaID", new object[0]);
            return (Guid)objArray[0];
        }

        public void GetTopicsAreaIDAsync()
        {
            this.GetTopicsAreaIDAsync(null);
        }

        public void GetTopicsAreaIDAsync(object userState)
        {
            if (this.GetTopicsAreaIDOperationCompleted == null)
            {
                this.GetTopicsAreaIDOperationCompleted =
                    new SendOrPostCallback(this.OnGetTopicsAreaIDOperationCompleted);
            }

            base.InvokeAsync("GetTopicsAreaID", new object[0], this.GetTopicsAreaIDOperationCompleted, userState);
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

        private void OnCreateAreaListingOperationCompleted(object arg)
        {
            if (this.CreateAreaListingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateAreaListingCompleted(this,
                    new CreateAreaListingCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateAreaOperationCompleted(object arg)
        {
            if (this.CreateAreaCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateAreaCompleted(this,
                    new CreateAreaCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteAreaListingOperationCompleted(object arg)
        {
            if (this.DeleteAreaListingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAreaListingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteAreaOperationCompleted(object arg)
        {
            if (this.DeleteAreaCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAreaCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAreaDataOperationCompleted(object arg)
        {
            if (this.GetAreaDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAreaDataCompleted(this,
                    new GetAreaDataCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAreaListingDataOperationCompleted(object arg)
        {
            if (this.GetAreaListingDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAreaListingDataCompleted(this,
                    new GetAreaListingDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAreaListingsOperationCompleted(object arg)
        {
            if (this.GetAreaListingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAreaListingsCompleted(this,
                    new GetAreaListingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentsAreaIDOperationCompleted(object arg)
        {
            if (this.GetDocumentsAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentsAreaIDCompleted(this,
                    new GetDocumentsAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetHomeAreaIDOperationCompleted(object arg)
        {
            if (this.GetHomeAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetHomeAreaIDCompleted(this,
                    new GetHomeAreaIDCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetKeywordsAreaIDOperationCompleted(object arg)
        {
            if (this.GetKeywordsAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetKeywordsAreaIDCompleted(this,
                    new GetKeywordsAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetMySiteAreaIDOperationCompleted(object arg)
        {
            if (this.GetMySiteAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetMySiteAreaIDCompleted(this,
                    new GetMySiteAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetNewsAreaIDOperationCompleted(object arg)
        {
            if (this.GetNewsAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetNewsAreaIDCompleted(this,
                    new GetNewsAreaIDCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSearchAreaIDOperationCompleted(object arg)
        {
            if (this.GetSearchAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSearchAreaIDCompleted(this,
                    new GetSearchAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSitesDirectoryAreaIDOperationCompleted(object arg)
        {
            if (this.GetSitesDirectoryAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSitesDirectoryAreaIDCompleted(this,
                    new GetSitesDirectoryAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSubAreasOperationCompleted(object arg)
        {
            if (this.GetSubAreasCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSubAreasCompleted(this,
                    new GetSubAreasCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTopicsAreaIDOperationCompleted(object arg)
        {
            if (this.GetTopicsAreaIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTopicsAreaIDCompleted(this,
                    new GetTopicsAreaIDCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetAreaDataOperationCompleted(object arg)
        {
            if (this.SetAreaDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetAreaDataCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetAreaListingDataOperationCompleted(object arg)
        {
            if (this.SetAreaListingDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetAreaListingDataCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/SetAreaData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SetAreaData(Guid AreaID, AreaData ad)
        {
            object[] areaID = new object[] { AreaID, ad };
            base.Invoke("SetAreaData", areaID);
        }

        public void SetAreaDataAsync(Guid AreaID, AreaData ad)
        {
            this.SetAreaDataAsync(AreaID, ad, null);
        }

        public void SetAreaDataAsync(Guid AreaID, AreaData ad, object userState)
        {
            if (this.SetAreaDataOperationCompleted == null)
            {
                this.SetAreaDataOperationCompleted = new SendOrPostCallback(this.OnSetAreaDataOperationCompleted);
            }

            object[] areaID = new object[] { AreaID, ad };
            base.InvokeAsync("SetAreaData", areaID, this.SetAreaDataOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/SetAreaListingData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SetAreaListingData(Guid AreaListingID, AreaListingData ld)
        {
            object[] areaListingID = new object[] { AreaListingID, ld };
            base.Invoke("SetAreaListingData", areaListingID);
        }

        public void SetAreaListingDataAsync(Guid AreaListingID, AreaListingData ld)
        {
            this.SetAreaListingDataAsync(AreaListingID, ld, null);
        }

        public void SetAreaListingDataAsync(Guid AreaListingID, AreaListingData ld, object userState)
        {
            if (this.SetAreaListingDataOperationCompleted == null)
            {
                this.SetAreaListingDataOperationCompleted =
                    new SendOrPostCallback(this.OnSetAreaListingDataOperationCompleted);
            }

            object[] areaListingID = new object[] { AreaListingID, ld };
            base.InvokeAsync("SetAreaListingData", areaListingID, this.SetAreaListingDataOperationCompleted, userState);
        }

        public event CreateAreaCompletedEventHandler CreateAreaCompleted;

        public event CreateAreaListingCompletedEventHandler CreateAreaListingCompleted;

        public event DeleteAreaCompletedEventHandler DeleteAreaCompleted;

        public event DeleteAreaListingCompletedEventHandler DeleteAreaListingCompleted;

        public event GetAreaDataCompletedEventHandler GetAreaDataCompleted;

        public event GetAreaListingDataCompletedEventHandler GetAreaListingDataCompleted;

        public event GetAreaListingsCompletedEventHandler GetAreaListingsCompleted;

        public event GetDocumentsAreaIDCompletedEventHandler GetDocumentsAreaIDCompleted;

        public event GetHomeAreaIDCompletedEventHandler GetHomeAreaIDCompleted;

        public event GetKeywordsAreaIDCompletedEventHandler GetKeywordsAreaIDCompleted;

        public event GetMySiteAreaIDCompletedEventHandler GetMySiteAreaIDCompleted;

        public event GetNewsAreaIDCompletedEventHandler GetNewsAreaIDCompleted;

        public event GetSearchAreaIDCompletedEventHandler GetSearchAreaIDCompleted;

        public event GetSitesDirectoryAreaIDCompletedEventHandler GetSitesDirectoryAreaIDCompleted;

        public event GetSubAreasCompletedEventHandler GetSubAreasCompleted;

        public event GetTopicsAreaIDCompletedEventHandler GetTopicsAreaIDCompleted;

        public event SetAreaDataCompletedEventHandler SetAreaDataCompleted;

        public event SetAreaListingDataCompletedEventHandler SetAreaListingDataCompleted;
    }
}