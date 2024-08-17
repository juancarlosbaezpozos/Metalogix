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

namespace Metalogix.SharePoint.Adapters.NWS.Views
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "ViewsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Views : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetViewOperationCompleted;

        private SendOrPostCallback GetViewHtmlOperationCompleted;

        private SendOrPostCallback DeleteViewOperationCompleted;

        private SendOrPostCallback AddViewOperationCompleted;

        private SendOrPostCallback GetViewCollectionOperationCompleted;

        private SendOrPostCallback UpdateViewOperationCompleted;

        private SendOrPostCallback UpdateViewHtmlOperationCompleted;

        private SendOrPostCallback UpdateViewHtml2OperationCompleted;

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

        public Views()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Views_Views;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/AddView",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddView(string listName, string viewName, XmlNode viewFields, XmlNode query, XmlNode rowLimit,
            string type, bool makeViewDefault)
        {
            object[] objArray = new object[] { listName, viewName, viewFields, query, rowLimit, type, makeViewDefault };
            return (XmlNode)base.Invoke("AddView", objArray)[0];
        }

        public void AddViewAsync(string listName, string viewName, XmlNode viewFields, XmlNode query, XmlNode rowLimit,
            string type, bool makeViewDefault)
        {
            this.AddViewAsync(listName, viewName, viewFields, query, rowLimit, type, makeViewDefault, null);
        }

        public void AddViewAsync(string listName, string viewName, XmlNode viewFields, XmlNode query, XmlNode rowLimit,
            string type, bool makeViewDefault, object userState)
        {
            if (this.AddViewOperationCompleted == null)
            {
                this.AddViewOperationCompleted = new SendOrPostCallback(this.OnAddViewOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName, viewFields, query, rowLimit, type, makeViewDefault };
            base.InvokeAsync("AddView", objArray, this.AddViewOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteView",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteView(string listName, string viewName)
        {
            object[] objArray = new object[] { listName, viewName };
            base.Invoke("DeleteView", objArray);
        }

        public void DeleteViewAsync(string listName, string viewName)
        {
            this.DeleteViewAsync(listName, viewName, null);
        }

        public void DeleteViewAsync(string listName, string viewName, object userState)
        {
            if (this.DeleteViewOperationCompleted == null)
            {
                this.DeleteViewOperationCompleted = new SendOrPostCallback(this.OnDeleteViewOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName };
            base.InvokeAsync("DeleteView", objArray, this.DeleteViewOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetView",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetView(string listName, string viewName)
        {
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)base.Invoke("GetView", objArray)[0];
        }

        public void GetViewAsync(string listName, string viewName)
        {
            this.GetViewAsync(listName, viewName, null);
        }

        public void GetViewAsync(string listName, string viewName, object userState)
        {
            if (this.GetViewOperationCompleted == null)
            {
                this.GetViewOperationCompleted = new SendOrPostCallback(this.OnGetViewOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName };
            base.InvokeAsync("GetView", objArray, this.GetViewOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetViewCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetViewCollection(string listName)
        {
            object[] objArray = new object[] { listName };
            return (XmlNode)base.Invoke("GetViewCollection", objArray)[0];
        }

        public void GetViewCollectionAsync(string listName)
        {
            this.GetViewCollectionAsync(listName, null);
        }

        public void GetViewCollectionAsync(string listName, object userState)
        {
            if (this.GetViewCollectionOperationCompleted == null)
            {
                this.GetViewCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetViewCollectionOperationCompleted);
            }

            object[] objArray = new object[] { listName };
            base.InvokeAsync("GetViewCollection", objArray, this.GetViewCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetViewHtml",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetViewHtml(string listName, string viewName)
        {
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)base.Invoke("GetViewHtml", objArray)[0];
        }

        public void GetViewHtmlAsync(string listName, string viewName)
        {
            this.GetViewHtmlAsync(listName, viewName, null);
        }

        public void GetViewHtmlAsync(string listName, string viewName, object userState)
        {
            if (this.GetViewHtmlOperationCompleted == null)
            {
                this.GetViewHtmlOperationCompleted = new SendOrPostCallback(this.OnGetViewHtmlOperationCompleted);
            }

            object[] objArray = new object[] { listName, viewName };
            base.InvokeAsync("GetViewHtml", objArray, this.GetViewHtmlOperationCompleted, userState);
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

        private void OnAddViewOperationCompleted(object arg)
        {
            if (this.AddViewCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddViewCompleted(this,
                    new AddViewCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteViewOperationCompleted(object arg)
        {
            if (this.DeleteViewCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteViewCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetViewCollectionOperationCompleted(object arg)
        {
            if (this.GetViewCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetViewCollectionCompleted(this,
                    new GetViewCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetViewHtmlOperationCompleted(object arg)
        {
            if (this.GetViewHtmlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetViewHtmlCompleted(this,
                    new GetViewHtmlCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetViewOperationCompleted(object arg)
        {
            if (this.GetViewCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetViewCompleted(this,
                    new GetViewCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateViewHtml2OperationCompleted(object arg)
        {
            if (this.UpdateViewHtml2Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateViewHtml2Completed(this,
                    new UpdateViewHtml2CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateViewHtmlOperationCompleted(object arg)
        {
            if (this.UpdateViewHtmlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateViewHtmlCompleted(this,
                    new UpdateViewHtmlCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateViewOperationCompleted(object arg)
        {
            if (this.UpdateViewCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateViewCompleted(this,
                    new UpdateViewCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateView",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateView(string listName, string viewName, XmlNode viewProperties, XmlNode query,
            XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            object[] objArray = new object[]
                { listName, viewName, viewProperties, query, viewFields, aggregations, formats, rowLimit };
            return (XmlNode)base.Invoke("UpdateView", objArray)[0];
        }

        public void UpdateViewAsync(string listName, string viewName, XmlNode viewProperties, XmlNode query,
            XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            this.UpdateViewAsync(listName, viewName, viewProperties, query, viewFields, aggregations, formats, rowLimit,
                null);
        }

        public void UpdateViewAsync(string listName, string viewName, XmlNode viewProperties, XmlNode query,
            XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit, object userState)
        {
            if (this.UpdateViewOperationCompleted == null)
            {
                this.UpdateViewOperationCompleted = new SendOrPostCallback(this.OnUpdateViewOperationCompleted);
            }

            object[] objArray = new object[]
                { listName, viewName, viewProperties, query, viewFields, aggregations, formats, rowLimit };
            base.InvokeAsync("UpdateView", objArray, this.UpdateViewOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateViewHtml",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateViewHtml(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit
            };
            return (XmlNode)base.Invoke("UpdateViewHtml", objArray)[0];
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateViewHtml2",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateViewHtml2(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit,
            string openApplicationExtension)
        {
            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit, openApplicationExtension
            };
            return (XmlNode)base.Invoke("UpdateViewHtml2", objArray)[0];
        }

        public void UpdateViewHtml2Async(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit,
            string openApplicationExtension)
        {
            this.UpdateViewHtml2Async(listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter,
                viewEmpty, rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit,
                openApplicationExtension, null);
        }

        public void UpdateViewHtml2Async(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit,
            string openApplicationExtension, object userState)
        {
            if (this.UpdateViewHtml2OperationCompleted == null)
            {
                this.UpdateViewHtml2OperationCompleted =
                    new SendOrPostCallback(this.OnUpdateViewHtml2OperationCompleted);
            }

            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit, openApplicationExtension
            };
            base.InvokeAsync("UpdateViewHtml2", objArray, this.UpdateViewHtml2OperationCompleted, userState);
        }

        public void UpdateViewHtmlAsync(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            this.UpdateViewHtmlAsync(listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter,
                viewEmpty, rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit, null);
        }

        public void UpdateViewHtmlAsync(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit,
            object userState)
        {
            if (this.UpdateViewHtmlOperationCompleted == null)
            {
                this.UpdateViewHtmlOperationCompleted = new SendOrPostCallback(this.OnUpdateViewHtmlOperationCompleted);
            }

            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit
            };
            base.InvokeAsync("UpdateViewHtml", objArray, this.UpdateViewHtmlOperationCompleted, userState);
        }

        public event AddViewCompletedEventHandler AddViewCompleted;

        public event DeleteViewCompletedEventHandler DeleteViewCompleted;

        public event GetViewCollectionCompletedEventHandler GetViewCollectionCompleted;

        public event GetViewCompletedEventHandler GetViewCompleted;

        public event GetViewHtmlCompletedEventHandler GetViewHtmlCompleted;

        public event UpdateViewCompletedEventHandler UpdateViewCompleted;

        public event UpdateViewHtml2CompletedEventHandler UpdateViewHtml2Completed;

        public event UpdateViewHtmlCompletedEventHandler UpdateViewHtmlCompleted;
    }
}