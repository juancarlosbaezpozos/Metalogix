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

namespace Metalogix.SharePoint.Adapters.NWS.Webs
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "WebsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Webs : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetWebCollectionOperationCompleted;

        private SendOrPostCallback GetWebOperationCompleted;

        private SendOrPostCallback GetListTemplatesOperationCompleted;

        private SendOrPostCallback GetAllSubWebCollectionOperationCompleted;

        private SendOrPostCallback WebUrlFromPageUrlOperationCompleted;

        private SendOrPostCallback GetContentTypesOperationCompleted;

        private SendOrPostCallback GetContentTypeOperationCompleted;

        private SendOrPostCallback CreateContentTypeOperationCompleted;

        private SendOrPostCallback UpdateContentTypeOperationCompleted;

        private SendOrPostCallback DeleteContentTypeOperationCompleted;

        private SendOrPostCallback UpdateContentTypeXmlDocumentOperationCompleted;

        private SendOrPostCallback RemoveContentTypeXmlDocumentOperationCompleted;

        private SendOrPostCallback GetColumnsOperationCompleted;

        private SendOrPostCallback UpdateColumnsOperationCompleted;

        private SendOrPostCallback GetCustomizedPageStatusOperationCompleted;

        private SendOrPostCallback RevertFileContentStreamOperationCompleted;

        private SendOrPostCallback RevertAllFileContentStreamsOperationCompleted;

        private SendOrPostCallback CustomizeCssOperationCompleted;

        private SendOrPostCallback RevertCssOperationCompleted;

        private SendOrPostCallback GetActivatedFeaturesOperationCompleted;

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

        public Webs()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Webs_Webs;
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

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CreateContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CreateContentType(string displayName, string parentType, XmlNode newFields,
            XmlNode contentTypeProperties)
        {
            object[] objArray = new object[] { displayName, parentType, newFields, contentTypeProperties };
            return (string)base.Invoke("CreateContentType", objArray)[0];
        }

        public void CreateContentTypeAsync(string displayName, string parentType, XmlNode newFields,
            XmlNode contentTypeProperties)
        {
            this.CreateContentTypeAsync(displayName, parentType, newFields, contentTypeProperties, null);
        }

        public void CreateContentTypeAsync(string displayName, string parentType, XmlNode newFields,
            XmlNode contentTypeProperties, object userState)
        {
            if (this.CreateContentTypeOperationCompleted == null)
            {
                this.CreateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnCreateContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { displayName, parentType, newFields, contentTypeProperties };
            base.InvokeAsync("CreateContentType", objArray, this.CreateContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/CustomizeCss",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void CustomizeCss(string cssFile)
        {
            object[] objArray = new object[] { cssFile };
            base.Invoke("CustomizeCss", objArray);
        }

        public void CustomizeCssAsync(string cssFile)
        {
            this.CustomizeCssAsync(cssFile, null);
        }

        public void CustomizeCssAsync(string cssFile, object userState)
        {
            if (this.CustomizeCssOperationCompleted == null)
            {
                this.CustomizeCssOperationCompleted = new SendOrPostCallback(this.OnCustomizeCssOperationCompleted);
            }

            object[] objArray = new object[] { cssFile };
            base.InvokeAsync("CustomizeCss", objArray, this.CustomizeCssOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/DeleteContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode DeleteContentType(string contentTypeId)
        {
            object[] objArray = new object[] { contentTypeId };
            return (XmlNode)base.Invoke("DeleteContentType", objArray)[0];
        }

        public void DeleteContentTypeAsync(string contentTypeId)
        {
            this.DeleteContentTypeAsync(contentTypeId, null);
        }

        public void DeleteContentTypeAsync(string contentTypeId, object userState)
        {
            if (this.DeleteContentTypeOperationCompleted == null)
            {
                this.DeleteContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { contentTypeId };
            base.InvokeAsync("DeleteContentType", objArray, this.DeleteContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetActivatedFeatures",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetActivatedFeatures()
        {
            object[] objArray = base.Invoke("GetActivatedFeatures", new object[0]);
            return (string)objArray[0];
        }

        public void GetActivatedFeaturesAsync()
        {
            this.GetActivatedFeaturesAsync(null);
        }

        public void GetActivatedFeaturesAsync(object userState)
        {
            if (this.GetActivatedFeaturesOperationCompleted == null)
            {
                this.GetActivatedFeaturesOperationCompleted =
                    new SendOrPostCallback(this.OnGetActivatedFeaturesOperationCompleted);
            }

            base.InvokeAsync("GetActivatedFeatures", new object[0], this.GetActivatedFeaturesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetAllSubWebCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetAllSubWebCollection()
        {
            object[] objArray = base.Invoke("GetAllSubWebCollection", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetAllSubWebCollectionAsync()
        {
            this.GetAllSubWebCollectionAsync(null);
        }

        public void GetAllSubWebCollectionAsync(object userState)
        {
            if (this.GetAllSubWebCollectionOperationCompleted == null)
            {
                this.GetAllSubWebCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetAllSubWebCollectionOperationCompleted);
            }

            base.InvokeAsync("GetAllSubWebCollection", new object[0], this.GetAllSubWebCollectionOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetColumns",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetColumns()
        {
            object[] objArray = base.Invoke("GetColumns", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetColumnsAsync()
        {
            this.GetColumnsAsync(null);
        }

        public void GetColumnsAsync(object userState)
        {
            if (this.GetColumnsOperationCompleted == null)
            {
                this.GetColumnsOperationCompleted = new SendOrPostCallback(this.OnGetColumnsOperationCompleted);
            }

            base.InvokeAsync("GetColumns", new object[0], this.GetColumnsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetContentType(string contentTypeId)
        {
            object[] objArray = new object[] { contentTypeId };
            return (XmlNode)base.Invoke("GetContentType", objArray)[0];
        }

        public void GetContentTypeAsync(string contentTypeId)
        {
            this.GetContentTypeAsync(contentTypeId, null);
        }

        public void GetContentTypeAsync(string contentTypeId, object userState)
        {
            if (this.GetContentTypeOperationCompleted == null)
            {
                this.GetContentTypeOperationCompleted = new SendOrPostCallback(this.OnGetContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { contentTypeId };
            base.InvokeAsync("GetContentType", objArray, this.GetContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetContentTypes",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetContentTypes()
        {
            object[] objArray = base.Invoke("GetContentTypes", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetContentTypesAsync()
        {
            this.GetContentTypesAsync(null);
        }

        public void GetContentTypesAsync(object userState)
        {
            if (this.GetContentTypesOperationCompleted == null)
            {
                this.GetContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnGetContentTypesOperationCompleted);
            }

            base.InvokeAsync("GetContentTypes", new object[0], this.GetContentTypesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetCustomizedPageStatus",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public CustomizedPageStatus GetCustomizedPageStatus(string fileUrl)
        {
            object[] objArray = new object[] { fileUrl };
            return (CustomizedPageStatus)base.Invoke("GetCustomizedPageStatus", objArray)[0];
        }

        public void GetCustomizedPageStatusAsync(string fileUrl)
        {
            this.GetCustomizedPageStatusAsync(fileUrl, null);
        }

        public void GetCustomizedPageStatusAsync(string fileUrl, object userState)
        {
            if (this.GetCustomizedPageStatusOperationCompleted == null)
            {
                this.GetCustomizedPageStatusOperationCompleted =
                    new SendOrPostCallback(this.OnGetCustomizedPageStatusOperationCompleted);
            }

            object[] objArray = new object[] { fileUrl };
            base.InvokeAsync("GetCustomizedPageStatus", objArray, this.GetCustomizedPageStatusOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetListTemplates",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetListTemplates()
        {
            object[] objArray = base.Invoke("GetListTemplates", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetListTemplatesAsync()
        {
            this.GetListTemplatesAsync(null);
        }

        public void GetListTemplatesAsync(object userState)
        {
            if (this.GetListTemplatesOperationCompleted == null)
            {
                this.GetListTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnGetListTemplatesOperationCompleted);
            }

            base.InvokeAsync("GetListTemplates", new object[0], this.GetListTemplatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetWeb(string webUrl)
        {
            object[] objArray = new object[] { webUrl };
            return (XmlNode)base.Invoke("GetWeb", objArray)[0];
        }

        public void GetWebAsync(string webUrl)
        {
            this.GetWebAsync(webUrl, null);
        }

        public void GetWebAsync(string webUrl, object userState)
        {
            if (this.GetWebOperationCompleted == null)
            {
                this.GetWebOperationCompleted = new SendOrPostCallback(this.OnGetWebOperationCompleted);
            }

            object[] objArray = new object[] { webUrl };
            base.InvokeAsync("GetWeb", objArray, this.GetWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/GetWebCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetWebCollection()
        {
            object[] objArray = base.Invoke("GetWebCollection", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetWebCollectionAsync()
        {
            this.GetWebCollectionAsync(null);
        }

        public void GetWebCollectionAsync(object userState)
        {
            if (this.GetWebCollectionOperationCompleted == null)
            {
                this.GetWebCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebCollectionOperationCompleted);
            }

            base.InvokeAsync("GetWebCollection", new object[0], this.GetWebCollectionOperationCompleted, userState);
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

        private void OnCustomizeCssOperationCompleted(object arg)
        {
            if (this.CustomizeCssCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CustomizeCssCompleted(this,
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

        private void OnGetActivatedFeaturesOperationCompleted(object arg)
        {
            if (this.GetActivatedFeaturesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetActivatedFeaturesCompleted(this,
                    new GetActivatedFeaturesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllSubWebCollectionOperationCompleted(object arg)
        {
            if (this.GetAllSubWebCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllSubWebCollectionCompleted(this,
                    new GetAllSubWebCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetColumnsOperationCompleted(object arg)
        {
            if (this.GetColumnsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetColumnsCompleted(this,
                    new GetColumnsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetContentTypeOperationCompleted(object arg)
        {
            if (this.GetContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetContentTypeCompleted(this,
                    new GetContentTypeCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetContentTypesOperationCompleted(object arg)
        {
            if (this.GetContentTypesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetContentTypesCompleted(this,
                    new GetContentTypesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCustomizedPageStatusOperationCompleted(object arg)
        {
            if (this.GetCustomizedPageStatusCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCustomizedPageStatusCompleted(this,
                    new GetCustomizedPageStatusCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListTemplatesOperationCompleted(object arg)
        {
            if (this.GetListTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListTemplatesCompleted(this,
                    new GetListTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebCollectionOperationCompleted(object arg)
        {
            if (this.GetWebCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebCollectionCompleted(this,
                    new GetWebCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
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

        private void OnRemoveContentTypeXmlDocumentOperationCompleted(object arg)
        {
            if (this.RemoveContentTypeXmlDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveContentTypeXmlDocumentCompleted(this,
                    new RemoveContentTypeXmlDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRevertAllFileContentStreamsOperationCompleted(object arg)
        {
            if (this.RevertAllFileContentStreamsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RevertAllFileContentStreamsCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRevertCssOperationCompleted(object arg)
        {
            if (this.RevertCssCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RevertCssCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRevertFileContentStreamOperationCompleted(object arg)
        {
            if (this.RevertFileContentStreamCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RevertFileContentStreamCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateColumnsOperationCompleted(object arg)
        {
            if (this.UpdateColumnsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateColumnsCompleted(this,
                    new UpdateColumnsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
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

        private void OnWebUrlFromPageUrlOperationCompleted(object arg)
        {
            if (this.WebUrlFromPageUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.WebUrlFromPageUrlCompleted(this,
                    new WebUrlFromPageUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/RemoveContentTypeXmlDocument",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode RemoveContentTypeXmlDocument(string contentTypeId, string documentUri)
        {
            object[] objArray = new object[] { contentTypeId, documentUri };
            return (XmlNode)base.Invoke("RemoveContentTypeXmlDocument", objArray)[0];
        }

        public void RemoveContentTypeXmlDocumentAsync(string contentTypeId, string documentUri)
        {
            this.RemoveContentTypeXmlDocumentAsync(contentTypeId, documentUri, null);
        }

        public void RemoveContentTypeXmlDocumentAsync(string contentTypeId, string documentUri, object userState)
        {
            if (this.RemoveContentTypeXmlDocumentOperationCompleted == null)
            {
                this.RemoveContentTypeXmlDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveContentTypeXmlDocumentOperationCompleted);
            }

            object[] objArray = new object[] { contentTypeId, documentUri };
            base.InvokeAsync("RemoveContentTypeXmlDocument", objArray,
                this.RemoveContentTypeXmlDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/RevertAllFileContentStreams",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RevertAllFileContentStreams()
        {
            base.Invoke("RevertAllFileContentStreams", new object[0]);
        }

        public void RevertAllFileContentStreamsAsync()
        {
            this.RevertAllFileContentStreamsAsync(null);
        }

        public void RevertAllFileContentStreamsAsync(object userState)
        {
            if (this.RevertAllFileContentStreamsOperationCompleted == null)
            {
                this.RevertAllFileContentStreamsOperationCompleted =
                    new SendOrPostCallback(this.OnRevertAllFileContentStreamsOperationCompleted);
            }

            base.InvokeAsync("RevertAllFileContentStreams", new object[0],
                this.RevertAllFileContentStreamsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/RevertCss",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RevertCss(string cssFile)
        {
            object[] objArray = new object[] { cssFile };
            base.Invoke("RevertCss", objArray);
        }

        public void RevertCssAsync(string cssFile)
        {
            this.RevertCssAsync(cssFile, null);
        }

        public void RevertCssAsync(string cssFile, object userState)
        {
            if (this.RevertCssOperationCompleted == null)
            {
                this.RevertCssOperationCompleted = new SendOrPostCallback(this.OnRevertCssOperationCompleted);
            }

            object[] objArray = new object[] { cssFile };
            base.InvokeAsync("RevertCss", objArray, this.RevertCssOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/RevertFileContentStream",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RevertFileContentStream(string fileUrl)
        {
            object[] objArray = new object[] { fileUrl };
            base.Invoke("RevertFileContentStream", objArray);
        }

        public void RevertFileContentStreamAsync(string fileUrl)
        {
            this.RevertFileContentStreamAsync(fileUrl, null);
        }

        public void RevertFileContentStreamAsync(string fileUrl, object userState)
        {
            if (this.RevertFileContentStreamOperationCompleted == null)
            {
                this.RevertFileContentStreamOperationCompleted =
                    new SendOrPostCallback(this.OnRevertFileContentStreamOperationCompleted);
            }

            object[] objArray = new object[] { fileUrl };
            base.InvokeAsync("RevertFileContentStream", objArray, this.RevertFileContentStreamOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateColumns",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateColumns(XmlNode newFields, XmlNode updateFields, XmlNode deleteFields)
        {
            object[] objArray = new object[] { newFields, updateFields, deleteFields };
            return (XmlNode)base.Invoke("UpdateColumns", objArray)[0];
        }

        public void UpdateColumnsAsync(XmlNode newFields, XmlNode updateFields, XmlNode deleteFields)
        {
            this.UpdateColumnsAsync(newFields, updateFields, deleteFields, null);
        }

        public void UpdateColumnsAsync(XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, object userState)
        {
            if (this.UpdateColumnsOperationCompleted == null)
            {
                this.UpdateColumnsOperationCompleted = new SendOrPostCallback(this.OnUpdateColumnsOperationCompleted);
            }

            object[] objArray = new object[] { newFields, updateFields, deleteFields };
            base.InvokeAsync("UpdateColumns", objArray, this.UpdateColumnsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentType",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateContentType(string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            object[] objArray = new object[]
                { contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields };
            return (XmlNode)base.Invoke("UpdateContentType", objArray)[0];
        }

        public void UpdateContentTypeAsync(string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            this.UpdateContentTypeAsync(contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields,
                null);
        }

        public void UpdateContentTypeAsync(string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields, object userState)
        {
            if (this.UpdateContentTypeOperationCompleted == null)
            {
                this.UpdateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateContentTypeOperationCompleted);
            }

            object[] objArray = new object[]
                { contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields };
            base.InvokeAsync("UpdateContentType", objArray, this.UpdateContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/UpdateContentTypeXmlDocument",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateContentTypeXmlDocument(string contentTypeId, XmlNode newDocument)
        {
            object[] objArray = new object[] { contentTypeId, newDocument };
            return (XmlNode)base.Invoke("UpdateContentTypeXmlDocument", objArray)[0];
        }

        public void UpdateContentTypeXmlDocumentAsync(string contentTypeId, XmlNode newDocument)
        {
            this.UpdateContentTypeXmlDocumentAsync(contentTypeId, newDocument, null);
        }

        public void UpdateContentTypeXmlDocumentAsync(string contentTypeId, XmlNode newDocument, object userState)
        {
            if (this.UpdateContentTypeXmlDocumentOperationCompleted == null)
            {
                this.UpdateContentTypeXmlDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateContentTypeXmlDocumentOperationCompleted);
            }

            object[] objArray = new object[] { contentTypeId, newDocument };
            base.InvokeAsync("UpdateContentTypeXmlDocument", objArray,
                this.UpdateContentTypeXmlDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/WebUrlFromPageUrl",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string WebUrlFromPageUrl(string pageUrl)
        {
            object[] objArray = new object[] { pageUrl };
            return (string)base.Invoke("WebUrlFromPageUrl", objArray)[0];
        }

        public void WebUrlFromPageUrlAsync(string pageUrl)
        {
            this.WebUrlFromPageUrlAsync(pageUrl, null);
        }

        public void WebUrlFromPageUrlAsync(string pageUrl, object userState)
        {
            if (this.WebUrlFromPageUrlOperationCompleted == null)
            {
                this.WebUrlFromPageUrlOperationCompleted =
                    new SendOrPostCallback(this.OnWebUrlFromPageUrlOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl };
            base.InvokeAsync("WebUrlFromPageUrl", objArray, this.WebUrlFromPageUrlOperationCompleted, userState);
        }

        public event CreateContentTypeCompletedEventHandler CreateContentTypeCompleted;

        public event CustomizeCssCompletedEventHandler CustomizeCssCompleted;

        public event DeleteContentTypeCompletedEventHandler DeleteContentTypeCompleted;

        public event GetActivatedFeaturesCompletedEventHandler GetActivatedFeaturesCompleted;

        public event GetAllSubWebCollectionCompletedEventHandler GetAllSubWebCollectionCompleted;

        public event GetColumnsCompletedEventHandler GetColumnsCompleted;

        public event GetContentTypeCompletedEventHandler GetContentTypeCompleted;

        public event GetContentTypesCompletedEventHandler GetContentTypesCompleted;

        public event GetCustomizedPageStatusCompletedEventHandler GetCustomizedPageStatusCompleted;

        public event GetListTemplatesCompletedEventHandler GetListTemplatesCompleted;

        public event GetWebCollectionCompletedEventHandler GetWebCollectionCompleted;

        public event GetWebCompletedEventHandler GetWebCompleted;

        public event RemoveContentTypeXmlDocumentCompletedEventHandler RemoveContentTypeXmlDocumentCompleted;

        public event RevertAllFileContentStreamsCompletedEventHandler RevertAllFileContentStreamsCompleted;

        public event RevertCssCompletedEventHandler RevertCssCompleted;

        public event RevertFileContentStreamCompletedEventHandler RevertFileContentStreamCompleted;

        public event UpdateColumnsCompletedEventHandler UpdateColumnsCompleted;

        public event UpdateContentTypeCompletedEventHandler UpdateContentTypeCompleted;

        public event UpdateContentTypeXmlDocumentCompletedEventHandler UpdateContentTypeXmlDocumentCompleted;

        public event WebUrlFromPageUrlCompletedEventHandler WebUrlFromPageUrlCompleted;
    }
}