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
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Lists
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Lists : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetListOperationCompleted;

        private SendOrPostCallback GetListAndViewOperationCompleted;

        private SendOrPostCallback DeleteListOperationCompleted;

        private SendOrPostCallback AddListOperationCompleted;

        private SendOrPostCallback AddListFromFeatureOperationCompleted;

        private SendOrPostCallback UpdateListOperationCompleted;

        private SendOrPostCallback GetListCollectionOperationCompleted;

        private SendOrPostCallback GetListItemsOperationCompleted;

        private SendOrPostCallback GetListItemChangesOperationCompleted;

        private SendOrPostCallback GetListItemChangesSinceTokenOperationCompleted;

        private SendOrPostCallback UpdateListItemsOperationCompleted;

        private SendOrPostCallback AddDiscussionBoardItemOperationCompleted;

        private SendOrPostCallback GetVersionCollectionOperationCompleted;

        private SendOrPostCallback AddAttachmentOperationCompleted;

        private SendOrPostCallback GetAttachmentCollectionOperationCompleted;

        private SendOrPostCallback DeleteAttachmentOperationCompleted;

        private SendOrPostCallback CheckOutFileOperationCompleted;

        private SendOrPostCallback UndoCheckOutOperationCompleted;

        private SendOrPostCallback CheckInFileOperationCompleted;

        private SendOrPostCallback GetListContentTypesOperationCompleted;

        private SendOrPostCallback GetListContentTypeOperationCompleted;

        private SendOrPostCallback CreateContentTypeOperationCompleted;

        private SendOrPostCallback UpdateContentTypeOperationCompleted;

        private SendOrPostCallback DeleteContentTypeOperationCompleted;

        private SendOrPostCallback UpdateContentTypeXmlDocumentOperationCompleted;

        private SendOrPostCallback UpdateContentTypesXmlDocumentOperationCompleted;

        private SendOrPostCallback DeleteContentTypeXmlDocumentOperationCompleted;

        private SendOrPostCallback ApplyContentTypeToListOperationCompleted;

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

        public Lists()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Lists_Lists;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddAttachment",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddAttachment(string listName, string listItemID, string fileName,
            [XmlElement(DataType = "base64Binary")] byte[] attachment)
        {
            object[] objArray = new object[] { listName, listItemID, fileName, attachment };
            return (string)base.Invoke("AddAttachment", objArray)[0];
        }

        public void AddAttachmentAsync(string listName, string listItemID, string fileName, byte[] attachment)
        {
            this.AddAttachmentAsync(listName, listItemID, fileName, attachment, null);
        }

        public void AddAttachmentAsync(string listName, string listItemID, string fileName, byte[] attachment,
            object userState)
        {
            if (this.AddAttachmentOperationCompleted == null)
            {
                this.AddAttachmentOperationCompleted = new SendOrPostCallback(this.OnAddAttachmentOperationCompleted);
            }

            object[] objArray = new object[] { listName, listItemID, fileName, attachment };
            base.InvokeAsync("AddAttachment", objArray, this.AddAttachmentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddDiscussionBoardItem",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddDiscussionBoardItem(string listName, [XmlElement(DataType = "base64Binary")] byte[] message)
        {
            object[] objArray = new object[] { listName, message };
            return (XmlNode)base.Invoke("AddDiscussionBoardItem", objArray)[0];
        }

        public void AddDiscussionBoardItemAsync(string listName, byte[] message)
        {
            this.AddDiscussionBoardItemAsync(listName, message, null);
        }

        public void AddDiscussionBoardItemAsync(string listName, byte[] message, object userState)
        {
            if (this.AddDiscussionBoardItemOperationCompleted == null)
            {
                this.AddDiscussionBoardItemOperationCompleted =
                    new SendOrPostCallback(this.OnAddDiscussionBoardItemOperationCompleted);
            }

            object[] objArray = new object[] { listName, message };
            base.InvokeAsync("AddDiscussionBoardItem", objArray, this.AddDiscussionBoardItemOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddList(string listName, string description, int templateID)
        {
            object[] objArray = new object[] { listName, description, templateID };
            return (XmlNode)base.Invoke("AddList", objArray)[0];
        }

        public void AddListAsync(string listName, string description, int templateID)
        {
            this.AddListAsync(listName, description, templateID, null);
        }

        public void AddListAsync(string listName, string description, int templateID, object userState)
        {
            if (this.AddListOperationCompleted == null)
            {
                this.AddListOperationCompleted = new SendOrPostCallback(this.OnAddListOperationCompleted);
            }

            object[] objArray = new object[] { listName, description, templateID };
            base.InvokeAsync("AddList", objArray, this.AddListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddListFromFeature",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddListFromFeature(string listName, string description, Guid featureID, int templateID)
        {
            object[] objArray = new object[] { listName, description, featureID, templateID };
            return (XmlNode)base.Invoke("AddListFromFeature", objArray)[0];
        }

        public void AddListFromFeatureAsync(string listName, string description, Guid featureID, int templateID)
        {
            this.AddListFromFeatureAsync(listName, description, featureID, templateID, null);
        }

        public void AddListFromFeatureAsync(string listName, string description, Guid featureID, int templateID,
            object userState)
        {
            if (this.AddListFromFeatureOperationCompleted == null)
            {
                this.AddListFromFeatureOperationCompleted =
                    new SendOrPostCallback(this.OnAddListFromFeatureOperationCompleted);
            }

            object[] objArray = new object[] { listName, description, featureID, templateID };
            base.InvokeAsync("AddListFromFeature", objArray, this.AddListFromFeatureOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/ApplyContentTypeToList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode ApplyContentTypeToList(string webUrl, string contentTypeId, string listName)
        {
            object[] objArray = new object[] { webUrl, contentTypeId, listName };
            return (XmlNode)base.Invoke("ApplyContentTypeToList", objArray)[0];
        }

        public void ApplyContentTypeToListAsync(string webUrl, string contentTypeId, string listName)
        {
            this.ApplyContentTypeToListAsync(webUrl, contentTypeId, listName, null);
        }

        public void ApplyContentTypeToListAsync(string webUrl, string contentTypeId, string listName, object userState)
        {
            if (this.ApplyContentTypeToListOperationCompleted == null)
            {
                this.ApplyContentTypeToListOperationCompleted =
                    new SendOrPostCallback(this.OnApplyContentTypeToListOperationCompleted);
            }

            object[] objArray = new object[] { webUrl, contentTypeId, listName };
            base.InvokeAsync("ApplyContentTypeToList", objArray, this.ApplyContentTypeToListOperationCompleted,
                userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CheckInFile",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool CheckInFile(string pageUrl, string comment, string CheckinType)
        {
            object[] objArray = new object[] { pageUrl, comment, CheckinType };
            return (bool)base.Invoke("CheckInFile", objArray)[0];
        }

        public void CheckInFileAsync(string pageUrl, string comment, string CheckinType)
        {
            this.CheckInFileAsync(pageUrl, comment, CheckinType, null);
        }

        public void CheckInFileAsync(string pageUrl, string comment, string CheckinType, object userState)
        {
            if (this.CheckInFileOperationCompleted == null)
            {
                this.CheckInFileOperationCompleted = new SendOrPostCallback(this.OnCheckInFileOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, comment, CheckinType };
            base.InvokeAsync("CheckInFile", objArray, this.CheckInFileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CheckOutFile",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool CheckOutFile(string pageUrl, string checkoutToLocal, string lastmodified)
        {
            object[] objArray = new object[] { pageUrl, checkoutToLocal, lastmodified };
            return (bool)base.Invoke("CheckOutFile", objArray)[0];
        }

        public void CheckOutFileAsync(string pageUrl, string checkoutToLocal, string lastmodified)
        {
            this.CheckOutFileAsync(pageUrl, checkoutToLocal, lastmodified, null);
        }

        public void CheckOutFileAsync(string pageUrl, string checkoutToLocal, string lastmodified, object userState)
        {
            if (this.CheckOutFileOperationCompleted == null)
            {
                this.CheckOutFileOperationCompleted = new SendOrPostCallback(this.OnCheckOutFileOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, checkoutToLocal, lastmodified };
            base.InvokeAsync("CheckOutFile", objArray, this.CheckOutFileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CreateContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CreateContentType(string listName, string displayName, string parentType, XmlNode fields,
            XmlNode contentTypeProperties, string addToView)
        {
            object[] objArray = new object[]
                { listName, displayName, parentType, fields, contentTypeProperties, addToView };
            return (string)base.Invoke("CreateContentType", objArray)[0];
        }

        public void CreateContentTypeAsync(string listName, string displayName, string parentType, XmlNode fields,
            XmlNode contentTypeProperties, string addToView)
        {
            this.CreateContentTypeAsync(listName, displayName, parentType, fields, contentTypeProperties, addToView,
                null);
        }

        public void CreateContentTypeAsync(string listName, string displayName, string parentType, XmlNode fields,
            XmlNode contentTypeProperties, string addToView, object userState)
        {
            if (this.CreateContentTypeOperationCompleted == null)
            {
                this.CreateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnCreateContentTypeOperationCompleted);
            }

            object[] objArray = new object[]
                { listName, displayName, parentType, fields, contentTypeProperties, addToView };
            base.InvokeAsync("CreateContentType", objArray, this.CreateContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteAttachment",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteAttachment(string listName, string listItemID, string url)
        {
            object[] objArray = new object[] { listName, listItemID, url };
            base.Invoke("DeleteAttachment", objArray);
        }

        public void DeleteAttachmentAsync(string listName, string listItemID, string url)
        {
            this.DeleteAttachmentAsync(listName, listItemID, url, null);
        }

        public void DeleteAttachmentAsync(string listName, string listItemID, string url, object userState)
        {
            if (this.DeleteAttachmentOperationCompleted == null)
            {
                this.DeleteAttachmentOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteAttachmentOperationCompleted);
            }

            object[] objArray = new object[] { listName, listItemID, url };
            base.InvokeAsync("DeleteAttachment", objArray, this.DeleteAttachmentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode DeleteContentType(string listName, string contentTypeId)
        {
            object[] objArray = new object[] { listName, contentTypeId };
            return (XmlNode)base.Invoke("DeleteContentType", objArray)[0];
        }

        public void DeleteContentTypeAsync(string listName, string contentTypeId)
        {
            this.DeleteContentTypeAsync(listName, contentTypeId, null);
        }

        public void DeleteContentTypeAsync(string listName, string contentTypeId, object userState)
        {
            if (this.DeleteContentTypeOperationCompleted == null)
            {
                this.DeleteContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { listName, contentTypeId };
            base.InvokeAsync("DeleteContentType", objArray, this.DeleteContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteContentTypeXmlDocument",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode DeleteContentTypeXmlDocument(string listName, string contentTypeId, string documentUri)
        {
            object[] objArray = new object[] { listName, contentTypeId, documentUri };
            return (XmlNode)base.Invoke("DeleteContentTypeXmlDocument", objArray)[0];
        }

        public void DeleteContentTypeXmlDocumentAsync(string listName, string contentTypeId, string documentUri)
        {
            this.DeleteContentTypeXmlDocumentAsync(listName, contentTypeId, documentUri, null);
        }

        public void DeleteContentTypeXmlDocumentAsync(string listName, string contentTypeId, string documentUri,
            object userState)
        {
            if (this.DeleteContentTypeXmlDocumentOperationCompleted == null)
            {
                this.DeleteContentTypeXmlDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteContentTypeXmlDocumentOperationCompleted);
            }

            object[] objArray = new object[] { listName, contentTypeId, documentUri };
            base.InvokeAsync("DeleteContentTypeXmlDocument", objArray,
                this.DeleteContentTypeXmlDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteList(string listName)
        {
            object[] objArray = new object[] { listName };
            base.Invoke("DeleteList", objArray);
        }

        public void DeleteListAsync(string listName)
        {
            this.DeleteListAsync(listName, null);
        }

        public void DeleteListAsync(string listName, object userState)
        {
            if (this.DeleteListOperationCompleted == null)
            {
                this.DeleteListOperationCompleted = new SendOrPostCallback(this.OnDeleteListOperationCompleted);
            }

            object[] objArray = new object[] { listName };
            base.InvokeAsync("DeleteList", objArray, this.DeleteListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetAttachmentCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetAttachmentCollection(string listName, string listItemID)
        {
            object[] objArray = new object[] { listName, listItemID };
            return (XmlNode)base.Invoke("GetAttachmentCollection", objArray)[0];
        }

        public void GetAttachmentCollectionAsync(string listName, string listItemID)
        {
            this.GetAttachmentCollectionAsync(listName, listItemID, null);
        }

        public void GetAttachmentCollectionAsync(string listName, string listItemID, object userState)
        {
            if (this.GetAttachmentCollectionOperationCompleted == null)
            {
                this.GetAttachmentCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetAttachmentCollectionOperationCompleted);
            }

            object[] objArray = new object[] { listName, listItemID };
            base.InvokeAsync("GetAttachmentCollection", objArray, this.GetAttachmentCollectionOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetList(string listName)
        {
            object[] objArray = new object[] { listName };
            return (XmlNode)base.Invoke("GetList", objArray)[0];
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListAndView",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListAndView(string listName, string viewName)
        {
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)base.Invoke("GetListAndView", objArray)[0];
        }

        public void GetListAndViewAsync(string listName, string viewName)
        {
            this.GetListAndViewAsync(listName, viewName, null);
        }

        public void GetListAndViewAsync(string listName, string viewName, object userState)
        {
            if (this.GetListAndViewOperationCompleted == null)
            {
                this.GetListAndViewOperationCompleted = new SendOrPostCallback(this.OnGetListAndViewOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName };
            base.InvokeAsync("GetListAndView", objArray, this.GetListAndViewOperationCompleted, userState);
        }

        public void GetListAsync(string listName)
        {
            this.GetListAsync(listName, null);
        }

        public void GetListAsync(string listName, object userState)
        {
            if (this.GetListOperationCompleted == null)
            {
                this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);
            }

            object[] objArray = new object[] { listName };
            base.InvokeAsync("GetList", objArray, this.GetListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListCollection()
        {
            object[] objArray = base.Invoke("GetListCollection", new object[0]);
            return (XmlNode)objArray[0];
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListContentType(string listName, string contentTypeId)
        {
            object[] objArray = new object[] { listName, contentTypeId };
            return (XmlNode)base.Invoke("GetListContentType", objArray)[0];
        }

        public void GetListContentTypeAsync(string listName, string contentTypeId)
        {
            this.GetListContentTypeAsync(listName, contentTypeId, null);
        }

        public void GetListContentTypeAsync(string listName, string contentTypeId, object userState)
        {
            if (this.GetListContentTypeOperationCompleted == null)
            {
                this.GetListContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnGetListContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { listName, contentTypeId };
            base.InvokeAsync("GetListContentType", objArray, this.GetListContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListContentTypes",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListContentTypes(string listName, string contentTypeId)
        {
            object[] objArray = new object[] { listName, contentTypeId };
            return (XmlNode)base.Invoke("GetListContentTypes", objArray)[0];
        }

        public void GetListContentTypesAsync(string listName, string contentTypeId)
        {
            this.GetListContentTypesAsync(listName, contentTypeId, null);
        }

        public void GetListContentTypesAsync(string listName, string contentTypeId, object userState)
        {
            if (this.GetListContentTypesOperationCompleted == null)
            {
                this.GetListContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnGetListContentTypesOperationCompleted);
            }

            object[] objArray = new object[] { listName, contentTypeId };
            base.InvokeAsync("GetListContentTypes", objArray, this.GetListContentTypesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItemChanges",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListItemChanges(string listName, XmlNode viewFields, string since, XmlNode contains)
        {
            object[] objArray = new object[] { listName, viewFields, since, contains };
            return (XmlNode)base.Invoke("GetListItemChanges", objArray)[0];
        }

        public void GetListItemChangesAsync(string listName, XmlNode viewFields, string since, XmlNode contains)
        {
            this.GetListItemChangesAsync(listName, viewFields, since, contains, null);
        }

        public void GetListItemChangesAsync(string listName, XmlNode viewFields, string since, XmlNode contains,
            object userState)
        {
            if (this.GetListItemChangesOperationCompleted == null)
            {
                this.GetListItemChangesOperationCompleted =
                    new SendOrPostCallback(this.OnGetListItemChangesOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewFields, since, contains };
            base.InvokeAsync("GetListItemChanges", objArray, this.GetListItemChangesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItemChangesSinceToken",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListItemChangesSinceToken(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains)
        {
            object[] objArray = new object[]
                { listName, viewName, query, viewFields, rowLimit, queryOptions, changeToken, contains };
            return (XmlNode)base.Invoke("GetListItemChangesSinceToken", objArray)[0];
        }

        public void GetListItemChangesSinceTokenAsync(string listName, string viewName, XmlNode query,
            XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains)
        {
            this.GetListItemChangesSinceTokenAsync(listName, viewName, query, viewFields, rowLimit, queryOptions,
                changeToken, contains, null);
        }

        public void GetListItemChangesSinceTokenAsync(string listName, string viewName, XmlNode query,
            XmlNode viewFields, string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains,
            object userState)
        {
            if (this.GetListItemChangesSinceTokenOperationCompleted == null)
            {
                this.GetListItemChangesSinceTokenOperationCompleted =
                    new SendOrPostCallback(this.OnGetListItemChangesSinceTokenOperationCompleted);
            }

            object[] objArray = new object[]
                { listName, viewName, query, viewFields, rowLimit, queryOptions, changeToken, contains };
            base.InvokeAsync("GetListItemChangesSinceToken", objArray,
                this.GetListItemChangesSinceTokenOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItems",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string webID)
        {
            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions, webID };
            return (XmlNode)base.Invoke("GetListItems", objArray)[0];
        }

        public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string webID)
        {
            this.GetListItemsAsync(listName, viewName, query, viewFields, rowLimit, queryOptions, webID, null);
        }

        public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string webID, object userState)
        {
            if (this.GetListItemsOperationCompleted == null)
            {
                this.GetListItemsOperationCompleted = new SendOrPostCallback(this.OnGetListItemsOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions, webID };
            base.InvokeAsync("GetListItems", objArray, this.GetListItemsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetVersionCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetVersionCollection(string strlistID, string strlistItemID, string strFieldName)
        {
            object[] objArray = new object[] { strlistID, strlistItemID, strFieldName };
            return (XmlNode)base.Invoke("GetVersionCollection", objArray)[0];
        }

        public void GetVersionCollectionAsync(string strlistID, string strlistItemID, string strFieldName)
        {
            this.GetVersionCollectionAsync(strlistID, strlistItemID, strFieldName, null);
        }

        public void GetVersionCollectionAsync(string strlistID, string strlistItemID, string strFieldName,
            object userState)
        {
            if (this.GetVersionCollectionOperationCompleted == null)
            {
                this.GetVersionCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetVersionCollectionOperationCompleted);
            }

            object[] objArray = new object[] { strlistID, strlistItemID, strFieldName };
            base.InvokeAsync("GetVersionCollection", objArray, this.GetVersionCollectionOperationCompleted, userState);
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

        private void OnAddAttachmentOperationCompleted(object arg)
        {
            if (this.AddAttachmentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddAttachmentCompleted(this,
                    new AddAttachmentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddDiscussionBoardItemOperationCompleted(object arg)
        {
            if (this.AddDiscussionBoardItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddDiscussionBoardItemCompleted(this,
                    new AddDiscussionBoardItemCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddListFromFeatureOperationCompleted(object arg)
        {
            if (this.AddListFromFeatureCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddListFromFeatureCompleted(this,
                    new AddListFromFeatureCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddListOperationCompleted(object arg)
        {
            if (this.AddListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddListCompleted(this,
                    new AddListCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnApplyContentTypeToListOperationCompleted(object arg)
        {
            if (this.ApplyContentTypeToListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ApplyContentTypeToListCompleted(this,
                    new ApplyContentTypeToListCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCheckInFileOperationCompleted(object arg)
        {
            if (this.CheckInFileCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CheckInFileCompleted(this,
                    new CheckInFileCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCheckOutFileOperationCompleted(object arg)
        {
            if (this.CheckOutFileCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CheckOutFileCompleted(this,
                    new CheckOutFileCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateContentTypeOperationCompleted(object arg)
        {
            if (this.CreateContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateContentTypeCompleted(this,
                    new CreateContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteAttachmentOperationCompleted(object arg)
        {
            if (this.DeleteAttachmentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAttachmentCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteContentTypeOperationCompleted(object arg)
        {
            if (this.DeleteContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteContentTypeCompleted(this,
                    new DeleteContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteContentTypeXmlDocumentOperationCompleted(object arg)
        {
            if (this.DeleteContentTypeXmlDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteContentTypeXmlDocumentCompleted(this,
                    new DeleteContentTypeXmlDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteListOperationCompleted(object arg)
        {
            if (this.DeleteListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteListCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAttachmentCollectionOperationCompleted(object arg)
        {
            if (this.GetAttachmentCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAttachmentCollectionCompleted(this,
                    new GetAttachmentCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListAndViewOperationCompleted(object arg)
        {
            if (this.GetListAndViewCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListAndViewCompleted(this,
                    new GetListAndViewCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
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

        private void OnGetListContentTypeOperationCompleted(object arg)
        {
            if (this.GetListContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListContentTypeCompleted(this,
                    new GetListContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListContentTypesOperationCompleted(object arg)
        {
            if (this.GetListContentTypesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListContentTypesCompleted(this,
                    new GetListContentTypesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListItemChangesOperationCompleted(object arg)
        {
            if (this.GetListItemChangesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemChangesCompleted(this,
                    new GetListItemChangesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListItemChangesSinceTokenOperationCompleted(object arg)
        {
            if (this.GetListItemChangesSinceTokenCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemChangesSinceTokenCompleted(this,
                    new GetListItemChangesSinceTokenCompletedEventArgs(invokeCompletedEventArg.Results,
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

        private void OnGetVersionCollectionOperationCompleted(object arg)
        {
            if (this.GetVersionCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetVersionCollectionCompleted(this,
                    new GetVersionCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUndoCheckOutOperationCompleted(object arg)
        {
            if (this.UndoCheckOutCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UndoCheckOutCompleted(this,
                    new UndoCheckOutCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateContentTypeOperationCompleted(object arg)
        {
            if (this.UpdateContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateContentTypeCompleted(this,
                    new UpdateContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateContentTypesXmlDocumentOperationCompleted(object arg)
        {
            if (this.UpdateContentTypesXmlDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateContentTypesXmlDocumentCompleted(this,
                    new UpdateContentTypesXmlDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateContentTypeXmlDocumentOperationCompleted(object arg)
        {
            if (this.UpdateContentTypeXmlDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateContentTypeXmlDocumentCompleted(this,
                    new UpdateContentTypeXmlDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateListItemsOperationCompleted(object arg)
        {
            if (this.UpdateListItemsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateListItemsCompleted(this,
                    new UpdateListItemsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateListOperationCompleted(object arg)
        {
            if (this.UpdateListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateListCompleted(this,
                    new UpdateListCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UndoCheckOut",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool UndoCheckOut(string pageUrl)
        {
            object[] objArray = new object[] { pageUrl };
            return (bool)base.Invoke("UndoCheckOut", objArray)[0];
        }

        public void UndoCheckOutAsync(string pageUrl)
        {
            this.UndoCheckOutAsync(pageUrl, null);
        }

        public void UndoCheckOutAsync(string pageUrl, object userState)
        {
            if (this.UndoCheckOutOperationCompleted == null)
            {
                this.UndoCheckOutOperationCompleted = new SendOrPostCallback(this.OnUndoCheckOutOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl };
            base.InvokeAsync("UndoCheckOut", objArray, this.UndoCheckOutOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateContentType(string listName, string contentTypeId, XmlNode contentTypeProperties,
            XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView)
        {
            object[] objArray = new object[]
                { listName, contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields, addToView };
            return (XmlNode)base.Invoke("UpdateContentType", objArray)[0];
        }

        public void UpdateContentTypeAsync(string listName, string contentTypeId, XmlNode contentTypeProperties,
            XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView)
        {
            this.UpdateContentTypeAsync(listName, contentTypeId, contentTypeProperties, newFields, updateFields,
                deleteFields, addToView, null);
        }

        public void UpdateContentTypeAsync(string listName, string contentTypeId, XmlNode contentTypeProperties,
            XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView, object userState)
        {
            if (this.UpdateContentTypeOperationCompleted == null)
            {
                this.UpdateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateContentTypeOperationCompleted);
            }

            object[] objArray = new object[]
                { listName, contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields, addToView };
            base.InvokeAsync("UpdateContentType", objArray, this.UpdateContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentTypesXmlDocument",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateContentTypesXmlDocument(string listName, XmlNode newDocument)
        {
            object[] objArray = new object[] { listName, newDocument };
            return (XmlNode)base.Invoke("UpdateContentTypesXmlDocument", objArray)[0];
        }

        public void UpdateContentTypesXmlDocumentAsync(string listName, XmlNode newDocument)
        {
            this.UpdateContentTypesXmlDocumentAsync(listName, newDocument, null);
        }

        public void UpdateContentTypesXmlDocumentAsync(string listName, XmlNode newDocument, object userState)
        {
            if (this.UpdateContentTypesXmlDocumentOperationCompleted == null)
            {
                this.UpdateContentTypesXmlDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateContentTypesXmlDocumentOperationCompleted);
            }

            object[] objArray = new object[] { listName, newDocument };
            base.InvokeAsync("UpdateContentTypesXmlDocument", objArray,
                this.UpdateContentTypesXmlDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentTypeXmlDocument",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateContentTypeXmlDocument(string listName, string contentTypeId, XmlNode newDocument)
        {
            object[] objArray = new object[] { listName, contentTypeId, newDocument };
            return (XmlNode)base.Invoke("UpdateContentTypeXmlDocument", objArray)[0];
        }

        public void UpdateContentTypeXmlDocumentAsync(string listName, string contentTypeId, XmlNode newDocument)
        {
            this.UpdateContentTypeXmlDocumentAsync(listName, contentTypeId, newDocument, null);
        }

        public void UpdateContentTypeXmlDocumentAsync(string listName, string contentTypeId, XmlNode newDocument,
            object userState)
        {
            if (this.UpdateContentTypeXmlDocumentOperationCompleted == null)
            {
                this.UpdateContentTypeXmlDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateContentTypeXmlDocumentOperationCompleted);
            }

            object[] objArray = new object[] { listName, contentTypeId, newDocument };
            base.InvokeAsync("UpdateContentTypeXmlDocument", objArray,
                this.UpdateContentTypeXmlDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateList",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateList(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields,
            XmlNode deleteFields, string listVersion)
        {
            object[] objArray = new object[]
                { listName, listProperties, newFields, updateFields, deleteFields, listVersion };
            return (XmlNode)base.Invoke("UpdateList", objArray)[0];
        }

        public void UpdateListAsync(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields,
            XmlNode deleteFields, string listVersion)
        {
            this.UpdateListAsync(listName, listProperties, newFields, updateFields, deleteFields, listVersion, null);
        }

        public void UpdateListAsync(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields,
            XmlNode deleteFields, string listVersion, object userState)
        {
            if (this.UpdateListOperationCompleted == null)
            {
                this.UpdateListOperationCompleted = new SendOrPostCallback(this.OnUpdateListOperationCompleted);
            }

            object[] objArray = new object[]
                { listName, listProperties, newFields, updateFields, deleteFields, listVersion };
            base.InvokeAsync("UpdateList", objArray, this.UpdateListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateListItems",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateListItems(string listName, XmlNode updates)
        {
            object[] objArray = new object[] { listName, updates };
            return (XmlNode)base.Invoke("UpdateListItems", objArray)[0];
        }

        public void UpdateListItemsAsync(string listName, XmlNode updates)
        {
            this.UpdateListItemsAsync(listName, updates, null);
        }

        public void UpdateListItemsAsync(string listName, XmlNode updates, object userState)
        {
            if (this.UpdateListItemsOperationCompleted == null)
            {
                this.UpdateListItemsOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateListItemsOperationCompleted);
            }

            object[] objArray = new object[] { listName, updates };
            base.InvokeAsync("UpdateListItems", objArray, this.UpdateListItemsOperationCompleted, userState);
        }

        public event AddAttachmentCompletedEventHandler AddAttachmentCompleted;

        public event AddDiscussionBoardItemCompletedEventHandler AddDiscussionBoardItemCompleted;

        public event AddListCompletedEventHandler AddListCompleted;

        public event AddListFromFeatureCompletedEventHandler AddListFromFeatureCompleted;

        public event ApplyContentTypeToListCompletedEventHandler ApplyContentTypeToListCompleted;

        public event CheckInFileCompletedEventHandler CheckInFileCompleted;

        public event CheckOutFileCompletedEventHandler CheckOutFileCompleted;

        public event CreateContentTypeCompletedEventHandler CreateContentTypeCompleted;

        public event DeleteAttachmentCompletedEventHandler DeleteAttachmentCompleted;

        public event DeleteContentTypeCompletedEventHandler DeleteContentTypeCompleted;

        public event DeleteContentTypeXmlDocumentCompletedEventHandler DeleteContentTypeXmlDocumentCompleted;

        public event DeleteListCompletedEventHandler DeleteListCompleted;

        public event GetAttachmentCollectionCompletedEventHandler GetAttachmentCollectionCompleted;

        public event GetListAndViewCompletedEventHandler GetListAndViewCompleted;

        public event GetListCollectionCompletedEventHandler GetListCollectionCompleted;

        public event GetListCompletedEventHandler GetListCompleted;

        public event GetListContentTypeCompletedEventHandler GetListContentTypeCompleted;

        public event GetListContentTypesCompletedEventHandler GetListContentTypesCompleted;

        public event GetListItemChangesCompletedEventHandler GetListItemChangesCompleted;

        public event GetListItemChangesSinceTokenCompletedEventHandler GetListItemChangesSinceTokenCompleted;

        public event GetListItemsCompletedEventHandler GetListItemsCompleted;

        public event GetVersionCollectionCompletedEventHandler GetVersionCollectionCompleted;

        public event UndoCheckOutCompletedEventHandler UndoCheckOutCompleted;

        public event UpdateContentTypeCompletedEventHandler UpdateContentTypeCompleted;

        public event UpdateContentTypesXmlDocumentCompletedEventHandler UpdateContentTypesXmlDocumentCompleted;

        public event UpdateContentTypeXmlDocumentCompletedEventHandler UpdateContentTypeXmlDocumentCompleted;

        public event UpdateListCompletedEventHandler UpdateListCompleted;

        public event UpdateListItemsCompletedEventHandler UpdateListItemsCompleted;
    }
}