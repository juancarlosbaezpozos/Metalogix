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

namespace Metalogix.SharePoint.Adapters.NWS.ListsSPV2
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Lists : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetListCollectionOperationCompleted;

        private SendOrPostCallback DeleteListOperationCompleted;

        private SendOrPostCallback UpdateListOperationCompleted;

        private SendOrPostCallback AddListOperationCompleted;

        private SendOrPostCallback GetListAndViewOperationCompleted;

        private SendOrPostCallback GetListOperationCompleted;

        private SendOrPostCallback DeleteAttachmentOperationCompleted;

        private SendOrPostCallback GetAttachmentCollectionOperationCompleted;

        private SendOrPostCallback AddAttachmentOperationCompleted;

        private SendOrPostCallback UpdateListItemsOperationCompleted;

        private SendOrPostCallback GetListItemChangesOperationCompleted;

        private SendOrPostCallback GetListItemsOperationCompleted;

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
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_ListsV2_Lists;
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

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListItems",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions)
        {
            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions };
            return (XmlNode)base.Invoke("GetListItems", objArray)[0];
        }

        public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions)
        {
            this.GetListItemsAsync(listName, viewName, query, viewFields, rowLimit, queryOptions, null);
        }

        public void GetListItemsAsync(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, object userState)
        {
            if (this.GetListItemsOperationCompleted == null)
            {
                this.GetListItemsOperationCompleted = new SendOrPostCallback(this.OnGetListItemsOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions };
            base.InvokeAsync("GetListItems", objArray, this.GetListItemsOperationCompleted, userState);
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

        public event AddListCompletedEventHandler AddListCompleted;

        public event DeleteAttachmentCompletedEventHandler DeleteAttachmentCompleted;

        public event DeleteListCompletedEventHandler DeleteListCompleted;

        public event GetAttachmentCollectionCompletedEventHandler GetAttachmentCollectionCompleted;

        public event GetListAndViewCompletedEventHandler GetListAndViewCompleted;

        public event GetListCollectionCompletedEventHandler GetListCollectionCompleted;

        public event GetListCompletedEventHandler GetListCompleted;

        public event GetListItemChangesCompletedEventHandler GetListItemChangesCompleted;

        public event GetListItemsCompletedEventHandler GetListItemsCompleted;

        public event UpdateListCompletedEventHandler UpdateListCompleted;

        public event UpdateListItemsCompletedEventHandler UpdateListItemsCompleted;
    }
}