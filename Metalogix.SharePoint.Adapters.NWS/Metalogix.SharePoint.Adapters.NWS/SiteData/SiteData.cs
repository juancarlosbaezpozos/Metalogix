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

namespace Metalogix.SharePoint.Adapters.NWS.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "SiteDataSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class SiteData : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetSiteAndWebOperationCompleted;

        private SendOrPostCallback GetSiteOperationCompleted;

        private SendOrPostCallback GetWebOperationCompleted;

        private SendOrPostCallback GetListOperationCompleted;

        private SendOrPostCallback GetListItemsOperationCompleted;

        private SendOrPostCallback EnumerateFolderOperationCompleted;

        private SendOrPostCallback GetAttachmentsOperationCompleted;

        private SendOrPostCallback GetURLSegmentsOperationCompleted;

        private SendOrPostCallback GetListCollectionOperationCompleted;

        private SendOrPostCallback GetContentOperationCompleted;

        private SendOrPostCallback GetSiteUrlOperationCompleted;

        private SendOrPostCallback GetChangesOperationCompleted;

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

        public SiteData()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_SitesData_SiteData;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/EnumerateFolder",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint EnumerateFolder(string strFolderUrl, [XmlArrayItem(IsNullable = false)] out _sFPUrl[] vUrls)
        {
            object[] objArray = new object[] { strFolderUrl };
            object[] objArray1 = base.Invoke("EnumerateFolder", objArray);
            vUrls = (_sFPUrl[])objArray1[1];
            return (uint)objArray1[0];
        }

        public void EnumerateFolderAsync(string strFolderUrl)
        {
            this.EnumerateFolderAsync(strFolderUrl, null);
        }

        public void EnumerateFolderAsync(string strFolderUrl, object userState)
        {
            if (this.EnumerateFolderOperationCompleted == null)
            {
                this.EnumerateFolderOperationCompleted =
                    new SendOrPostCallback(this.OnEnumerateFolderOperationCompleted);
            }

            object[] objArray = new object[] { strFolderUrl };
            base.InvokeAsync("EnumerateFolder", objArray, this.EnumerateFolderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetAttachments",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetAttachments(string strListName, string strItemId, out string[] vAttachments)
        {
            object[] objArray = new object[] { strListName, strItemId };
            object[] objArray1 = base.Invoke("GetAttachments", objArray);
            vAttachments = (string[])objArray1[1];
            return (uint)objArray1[0];
        }

        public void GetAttachmentsAsync(string strListName, string strItemId)
        {
            this.GetAttachmentsAsync(strListName, strItemId, null);
        }

        public void GetAttachmentsAsync(string strListName, string strItemId, object userState)
        {
            if (this.GetAttachmentsOperationCompleted == null)
            {
                this.GetAttachmentsOperationCompleted = new SendOrPostCallback(this.OnGetAttachmentsOperationCompleted);
            }

            object[] objArray = new object[] { strListName, strItemId };
            base.InvokeAsync("GetAttachments", objArray, this.GetAttachmentsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetChanges",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetChanges(ObjectType objectType, string contentDatabaseId, ref string LastChangeId,
            ref string CurrentChangeId, int Timeout, out bool moreChanges)
        {
            object[] objArray = new object[] { objectType, contentDatabaseId, LastChangeId, CurrentChangeId, Timeout };
            object[] objArray1 = base.Invoke("GetChanges", objArray);
            LastChangeId = (string)objArray1[1];
            CurrentChangeId = (string)objArray1[2];
            moreChanges = (bool)objArray1[3];
            return (string)objArray1[0];
        }

        public void GetChangesAsync(ObjectType objectType, string contentDatabaseId, string LastChangeId,
            string CurrentChangeId, int Timeout)
        {
            this.GetChangesAsync(objectType, contentDatabaseId, LastChangeId, CurrentChangeId, Timeout, null);
        }

        public void GetChangesAsync(ObjectType objectType, string contentDatabaseId, string LastChangeId,
            string CurrentChangeId, int Timeout, object userState)
        {
            if (this.GetChangesOperationCompleted == null)
            {
                this.GetChangesOperationCompleted = new SendOrPostCallback(this.OnGetChangesOperationCompleted);
            }

            object[] objArray = new object[] { objectType, contentDatabaseId, LastChangeId, CurrentChangeId, Timeout };
            base.InvokeAsync("GetChanges", objArray, this.GetChangesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetContent",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetContent(ObjectType objectType, string objectId, string folderUrl, string itemId,
            bool retrieveChildItems, bool securityOnly, ref string lastItemIdOnPage)
        {
            object[] objArray = new object[]
                { objectType, objectId, folderUrl, itemId, retrieveChildItems, securityOnly, lastItemIdOnPage };
            object[] objArray1 = base.Invoke("GetContent", objArray);
            lastItemIdOnPage = (string)objArray1[1];
            return (string)objArray1[0];
        }

        public void GetContentAsync(ObjectType objectType, string objectId, string folderUrl, string itemId,
            bool retrieveChildItems, bool securityOnly, string lastItemIdOnPage)
        {
            this.GetContentAsync(objectType, objectId, folderUrl, itemId, retrieveChildItems, securityOnly,
                lastItemIdOnPage, null);
        }

        public void GetContentAsync(ObjectType objectType, string objectId, string folderUrl, string itemId,
            bool retrieveChildItems, bool securityOnly, string lastItemIdOnPage, object userState)
        {
            if (this.GetContentOperationCompleted == null)
            {
                this.GetContentOperationCompleted = new SendOrPostCallback(this.OnGetContentOperationCompleted);
            }

            object[] objArray = new object[]
                { objectType, objectId, folderUrl, itemId, retrieveChildItems, securityOnly, lastItemIdOnPage };
            base.InvokeAsync("GetContent", objArray, this.GetContentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetList(string strListName, out _sListMetadata sListMetadata,
            [XmlArrayItem(IsNullable = false)] out _sProperty[] vProperties)
        {
            object[] objArray = new object[] { strListName };
            object[] objArray1 = base.Invoke("GetList", objArray);
            sListMetadata = (_sListMetadata)objArray1[1];
            vProperties = (_sProperty[])objArray1[2];
            return (uint)objArray1[0];
        }

        public void GetListAsync(string strListName)
        {
            this.GetListAsync(strListName, null);
        }

        public void GetListAsync(string strListName, object userState)
        {
            if (this.GetListOperationCompleted == null)
            {
                this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);
            }

            object[] objArray = new object[] { strListName };
            base.InvokeAsync("GetList", objArray, this.GetListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetListCollection([XmlArrayItem(IsNullable = false)] out _sList[] vLists)
        {
            object[] objArray = base.Invoke("GetListCollection", new object[0]);
            vLists = (_sList[])objArray[1];
            return (uint)objArray[0];
        }

        public void GetListCollectionAsync()
        {
            this.GetListCollectionAsync(null);
        }

        public void GetListCollectionAsync(object userState)
        {
            if (this.GetListCollectionOperationCompleted == null)
            {
                this.GetListCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetListCollectionOperationCompleted);
            }

            base.InvokeAsync("GetListCollection", new object[0], this.GetListCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItems",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListItems(string strListName, string strQuery, string strViewFields, uint uRowLimit)
        {
            object[] objArray = new object[] { strListName, strQuery, strViewFields, uRowLimit };
            return (string)base.Invoke("GetListItems", objArray)[0];
        }

        public void GetListItemsAsync(string strListName, string strQuery, string strViewFields, uint uRowLimit)
        {
            this.GetListItemsAsync(strListName, strQuery, strViewFields, uRowLimit, null);
        }

        public void GetListItemsAsync(string strListName, string strQuery, string strViewFields, uint uRowLimit,
            object userState)
        {
            if (this.GetListItemsOperationCompleted == null)
            {
                this.GetListItemsOperationCompleted = new SendOrPostCallback(this.OnGetListItemsOperationCompleted);
            }

            object[] objArray = new object[] { strListName, strQuery, strViewFields, uRowLimit };
            base.InvokeAsync("GetListItems", objArray, this.GetListItemsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetSite(out _sSiteMetadata sSiteMetadata,
            [XmlArrayItem(IsNullable = false)] out _sWebWithTime[] vWebs, out string strUsers, out string strGroups,
            out string[] vGroups)
        {
            object[] objArray = base.Invoke("GetSite", new object[0]);
            sSiteMetadata = (_sSiteMetadata)objArray[1];
            vWebs = (_sWebWithTime[])objArray[2];
            strUsers = (string)objArray[3];
            strGroups = (string)objArray[4];
            vGroups = (string[])objArray[5];
            return (uint)objArray[0];
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetSiteAndWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetSiteAndWeb(string strUrl, out string strSite, out string strWeb)
        {
            object[] objArray = new object[] { strUrl };
            object[] objArray1 = base.Invoke("GetSiteAndWeb", objArray);
            strSite = (string)objArray1[1];
            strWeb = (string)objArray1[2];
            return (uint)objArray1[0];
        }

        public void GetSiteAndWebAsync(string strUrl)
        {
            this.GetSiteAndWebAsync(strUrl, null);
        }

        public void GetSiteAndWebAsync(string strUrl, object userState)
        {
            if (this.GetSiteAndWebOperationCompleted == null)
            {
                this.GetSiteAndWebOperationCompleted = new SendOrPostCallback(this.OnGetSiteAndWebOperationCompleted);
            }

            object[] objArray = new object[] { strUrl };
            base.InvokeAsync("GetSiteAndWeb", objArray, this.GetSiteAndWebOperationCompleted, userState);
        }

        public void GetSiteAsync()
        {
            this.GetSiteAsync(null);
        }

        public void GetSiteAsync(object userState)
        {
            if (this.GetSiteOperationCompleted == null)
            {
                this.GetSiteOperationCompleted = new SendOrPostCallback(this.OnGetSiteOperationCompleted);
            }

            base.InvokeAsync("GetSite", new object[0], this.GetSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetSiteUrl",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetSiteUrl(string Url, out string siteUrl, out string siteId)
        {
            object[] url = new object[] { Url };
            object[] objArray = base.Invoke("GetSiteUrl", url);
            siteUrl = (string)objArray[1];
            siteId = (string)objArray[2];
            return (uint)objArray[0];
        }

        public void GetSiteUrlAsync(string Url)
        {
            this.GetSiteUrlAsync(Url, null);
        }

        public void GetSiteUrlAsync(string Url, object userState)
        {
            if (this.GetSiteUrlOperationCompleted == null)
            {
                this.GetSiteUrlOperationCompleted = new SendOrPostCallback(this.OnGetSiteUrlOperationCompleted);
            }

            object[] url = new object[] { Url };
            base.InvokeAsync("GetSiteUrl", url, this.GetSiteUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetURLSegments",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool GetURLSegments(string strURL, out string strWebID, out string strBucketID, out string strListID,
            out string strItemID)
        {
            object[] objArray = new object[] { strURL };
            object[] objArray1 = base.Invoke("GetURLSegments", objArray);
            strWebID = (string)objArray1[1];
            strBucketID = (string)objArray1[2];
            strListID = (string)objArray1[3];
            strItemID = (string)objArray1[4];
            return (bool)objArray1[0];
        }

        public void GetURLSegmentsAsync(string strURL)
        {
            this.GetURLSegmentsAsync(strURL, null);
        }

        public void GetURLSegmentsAsync(string strURL, object userState)
        {
            if (this.GetURLSegmentsOperationCompleted == null)
            {
                this.GetURLSegmentsOperationCompleted = new SendOrPostCallback(this.OnGetURLSegmentsOperationCompleted);
            }

            object[] objArray = new object[] { strURL };
            base.InvokeAsync("GetURLSegments", objArray, this.GetURLSegmentsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public uint GetWeb(out _sWebMetadata sWebMetadata, [XmlArrayItem(IsNullable = false)] out _sWebWithTime[] vWebs,
            [XmlArrayItem(IsNullable = false)] out _sListWithTime[] vLists,
            [XmlArrayItem(IsNullable = false)] out _sFPUrl[] vFPUrls, out string strRoles, out string[] vRolesUsers,
            out string[] vRolesGroups)
        {
            object[] objArray = base.Invoke("GetWeb", new object[0]);
            sWebMetadata = (_sWebMetadata)objArray[1];
            vWebs = (_sWebWithTime[])objArray[2];
            vLists = (_sListWithTime[])objArray[3];
            vFPUrls = (_sFPUrl[])objArray[4];
            strRoles = (string)objArray[5];
            vRolesUsers = (string[])objArray[6];
            vRolesGroups = (string[])objArray[7];
            return (uint)objArray[0];
        }

        public void GetWebAsync()
        {
            this.GetWebAsync(null);
        }

        public void GetWebAsync(object userState)
        {
            if (this.GetWebOperationCompleted == null)
            {
                this.GetWebOperationCompleted = new SendOrPostCallback(this.OnGetWebOperationCompleted);
            }

            base.InvokeAsync("GetWeb", new object[0], this.GetWebOperationCompleted, userState);
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

        private void OnEnumerateFolderOperationCompleted(object arg)
        {
            if (this.EnumerateFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.EnumerateFolderCompleted(this,
                    new EnumerateFolderCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAttachmentsOperationCompleted(object arg)
        {
            if (this.GetAttachmentsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAttachmentsCompleted(this,
                    new GetAttachmentsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetChangesOperationCompleted(object arg)
        {
            if (this.GetChangesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetChangesCompleted(this,
                    new GetChangesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetContentOperationCompleted(object arg)
        {
            if (this.GetContentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetContentCompleted(this,
                    new GetContentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListCollectionOperationCompleted(object arg)
        {
            if (this.GetListCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListCollectionCompleted(this,
                    new GetListCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListItemsOperationCompleted(object arg)
        {
            if (this.GetListItemsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemsCompleted(this,
                    new GetListItemsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListOperationCompleted(object arg)
        {
            if (this.GetListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListCompleted(this,
                    new GetListCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteAndWebOperationCompleted(object arg)
        {
            if (this.GetSiteAndWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteAndWebCompleted(this,
                    new GetSiteAndWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteOperationCompleted(object arg)
        {
            if (this.GetSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteCompleted(this,
                    new GetSiteCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteUrlOperationCompleted(object arg)
        {
            if (this.GetSiteUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteUrlCompleted(this,
                    new GetSiteUrlCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetURLSegmentsOperationCompleted(object arg)
        {
            if (this.GetURLSegmentsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetURLSegmentsCompleted(this,
                    new GetURLSegmentsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebOperationCompleted(object arg)
        {
            if (this.GetWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebCompleted(this,
                    new GetWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        public event EnumerateFolderCompletedEventHandler EnumerateFolderCompleted;

        public event GetAttachmentsCompletedEventHandler GetAttachmentsCompleted;

        public event GetChangesCompletedEventHandler GetChangesCompleted;

        public event GetContentCompletedEventHandler GetContentCompleted;

        public event GetListCollectionCompletedEventHandler GetListCollectionCompleted;

        public event GetListCompletedEventHandler GetListCompleted;

        public event GetListItemsCompletedEventHandler GetListItemsCompleted;

        public event GetSiteAndWebCompletedEventHandler GetSiteAndWebCompleted;

        public event GetSiteCompletedEventHandler GetSiteCompleted;

        public event GetSiteUrlCompletedEventHandler GetSiteUrlCompleted;

        public event GetURLSegmentsCompletedEventHandler GetURLSegmentsCompleted;

        public event GetWebCompletedEventHandler GetWebCompleted;
    }
}