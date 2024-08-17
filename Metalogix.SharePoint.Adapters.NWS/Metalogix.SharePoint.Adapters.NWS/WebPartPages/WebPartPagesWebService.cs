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

namespace Metalogix.SharePoint.Adapters.NWS.WebPartPages
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "WebPartPagesWebServiceSoap", Namespace = "http://microsoft.com/sharepoint/webpartpages")]
    public class WebPartPagesWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetWebPartPageDocumentOperationCompleted;

        private SendOrPostCallback GetWebPartPageOperationCompleted;

        private SendOrPostCallback RenderWebPartForEditOperationCompleted;

        private SendOrPostCallback GetXmlDataFromDataSourceOperationCompleted;

        private SendOrPostCallback GetFormCapabilityFromDataSourceControlOperationCompleted;

        private SendOrPostCallback GetDataFromDataSourceControlOperationCompleted;

        private SendOrPostCallback FetchLegalWorkflowActionsOperationCompleted;

        private SendOrPostCallback ValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted;

        private SendOrPostCallback AssociateWorkflowMarkupOperationCompleted;

        private SendOrPostCallback RemoveWorkflowAssociationOperationCompleted;

        private SendOrPostCallback ConvertWebPartFormatOperationCompleted;

        private SendOrPostCallback GetAssemblyMetaDataOperationCompleted;

        private SendOrPostCallback GetBindingResourceDataOperationCompleted;

        private SendOrPostCallback ExecuteProxyUpdatesOperationCompleted;

        private SendOrPostCallback AddWebPartToZoneOperationCompleted;

        private SendOrPostCallback AddWebPartOperationCompleted;

        private SendOrPostCallback GetWebPartOperationCompleted;

        private SendOrPostCallback GetWebPart2OperationCompleted;

        private SendOrPostCallback GetCustomControlListOperationCompleted;

        private SendOrPostCallback GetSafeAssemblyInfoOperationCompleted;

        private SendOrPostCallback GetWebPartPropertiesOperationCompleted;

        private SendOrPostCallback GetWebPartProperties2OperationCompleted;

        private SendOrPostCallback SaveWebPartOperationCompleted;

        private SendOrPostCallback SaveWebPart2OperationCompleted;

        private SendOrPostCallback DeleteWebPartOperationCompleted;

        private SendOrPostCallback GetWebPartPageConnectionInfoOperationCompleted;

        private SendOrPostCallback GetWebPartCrossPageCompatibilityOperationCompleted;

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

        public WebPartPagesWebService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_WebPartPages_WebPartPagesWebService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/AddWebPart",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid AddWebPart(string pageUrl, string webPartXml, Storage storage)
        {
            object[] objArray = new object[] { pageUrl, webPartXml, storage };
            return (Guid)base.Invoke("AddWebPart", objArray)[0];
        }

        public void AddWebPartAsync(string pageUrl, string webPartXml, Storage storage)
        {
            this.AddWebPartAsync(pageUrl, webPartXml, storage, null);
        }

        public void AddWebPartAsync(string pageUrl, string webPartXml, Storage storage, object userState)
        {
            if (this.AddWebPartOperationCompleted == null)
            {
                this.AddWebPartOperationCompleted = new SendOrPostCallback(this.OnAddWebPartOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, webPartXml, storage };
            base.InvokeAsync("AddWebPart", objArray, this.AddWebPartOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/AddWebPartToZone",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid AddWebPartToZone(string pageUrl, string webPartXml, Storage storage, string zoneId, int zoneIndex)
        {
            object[] objArray = new object[] { pageUrl, webPartXml, storage, zoneId, zoneIndex };
            return (Guid)base.Invoke("AddWebPartToZone", objArray)[0];
        }

        public void AddWebPartToZoneAsync(string pageUrl, string webPartXml, Storage storage, string zoneId,
            int zoneIndex)
        {
            this.AddWebPartToZoneAsync(pageUrl, webPartXml, storage, zoneId, zoneIndex, null);
        }

        public void AddWebPartToZoneAsync(string pageUrl, string webPartXml, Storage storage, string zoneId,
            int zoneIndex, object userState)
        {
            if (this.AddWebPartToZoneOperationCompleted == null)
            {
                this.AddWebPartToZoneOperationCompleted =
                    new SendOrPostCallback(this.OnAddWebPartToZoneOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, webPartXml, storage, zoneId, zoneIndex };
            base.InvokeAsync("AddWebPartToZone", objArray, this.AddWebPartToZoneOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/AssociateWorkflowMarkup",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AssociateWorkflowMarkup(string configUrl, string configVersion)
        {
            object[] objArray = new object[] { configUrl, configVersion };
            return (string)base.Invoke("AssociateWorkflowMarkup", objArray)[0];
        }

        public void AssociateWorkflowMarkupAsync(string configUrl, string configVersion)
        {
            this.AssociateWorkflowMarkupAsync(configUrl, configVersion, null);
        }

        public void AssociateWorkflowMarkupAsync(string configUrl, string configVersion, object userState)
        {
            if (this.AssociateWorkflowMarkupOperationCompleted == null)
            {
                this.AssociateWorkflowMarkupOperationCompleted =
                    new SendOrPostCallback(this.OnAssociateWorkflowMarkupOperationCompleted);
            }

            object[] objArray = new object[] { configUrl, configVersion };
            base.InvokeAsync("AssociateWorkflowMarkup", objArray, this.AssociateWorkflowMarkupOperationCompleted,
                userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/ConvertWebPartFormat",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ConvertWebPartFormat(string inputFormat, FormatConversionOption formatConversionOption)
        {
            object[] objArray = new object[] { inputFormat, formatConversionOption };
            return (string)base.Invoke("ConvertWebPartFormat", objArray)[0];
        }

        public void ConvertWebPartFormatAsync(string inputFormat, FormatConversionOption formatConversionOption)
        {
            this.ConvertWebPartFormatAsync(inputFormat, formatConversionOption, null);
        }

        public void ConvertWebPartFormatAsync(string inputFormat, FormatConversionOption formatConversionOption,
            object userState)
        {
            if (this.ConvertWebPartFormatOperationCompleted == null)
            {
                this.ConvertWebPartFormatOperationCompleted =
                    new SendOrPostCallback(this.OnConvertWebPartFormatOperationCompleted);
            }

            object[] objArray = new object[] { inputFormat, formatConversionOption };
            base.InvokeAsync("ConvertWebPartFormat", objArray, this.ConvertWebPartFormatOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/DeleteWebPart",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteWebPart(string pageUrl, Guid storageKey, Storage storage)
        {
            object[] objArray = new object[] { pageUrl, storageKey, storage };
            base.Invoke("DeleteWebPart", objArray);
        }

        public void DeleteWebPartAsync(string pageUrl, Guid storageKey, Storage storage)
        {
            this.DeleteWebPartAsync(pageUrl, storageKey, storage, null);
        }

        public void DeleteWebPartAsync(string pageUrl, Guid storageKey, Storage storage, object userState)
        {
            if (this.DeleteWebPartOperationCompleted == null)
            {
                this.DeleteWebPartOperationCompleted = new SendOrPostCallback(this.OnDeleteWebPartOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, storageKey, storage };
            base.InvokeAsync("DeleteWebPart", objArray, this.DeleteWebPartOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/ExecuteProxyUpdates",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ExecuteProxyUpdates(string updateData)
        {
            object[] objArray = new object[] { updateData };
            return (string)base.Invoke("ExecuteProxyUpdates", objArray)[0];
        }

        public void ExecuteProxyUpdatesAsync(string updateData)
        {
            this.ExecuteProxyUpdatesAsync(updateData, null);
        }

        public void ExecuteProxyUpdatesAsync(string updateData, object userState)
        {
            if (this.ExecuteProxyUpdatesOperationCompleted == null)
            {
                this.ExecuteProxyUpdatesOperationCompleted =
                    new SendOrPostCallback(this.OnExecuteProxyUpdatesOperationCompleted);
            }

            object[] objArray = new object[] { updateData };
            base.InvokeAsync("ExecuteProxyUpdates", objArray, this.ExecuteProxyUpdatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/FetchLegalWorkflowActions",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string FetchLegalWorkflowActions()
        {
            object[] objArray = base.Invoke("FetchLegalWorkflowActions", new object[0]);
            return (string)objArray[0];
        }

        public void FetchLegalWorkflowActionsAsync()
        {
            this.FetchLegalWorkflowActionsAsync(null);
        }

        public void FetchLegalWorkflowActionsAsync(object userState)
        {
            if (this.FetchLegalWorkflowActionsOperationCompleted == null)
            {
                this.FetchLegalWorkflowActionsOperationCompleted =
                    new SendOrPostCallback(this.OnFetchLegalWorkflowActionsOperationCompleted);
            }

            base.InvokeAsync("FetchLegalWorkflowActions", new object[0],
                this.FetchLegalWorkflowActionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetAssemblyMetaData",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAssemblyMetaData(string assemblyName, string baseTypes)
        {
            object[] objArray = new object[] { assemblyName, baseTypes };
            return (string)base.Invoke("GetAssemblyMetaData", objArray)[0];
        }

        public void GetAssemblyMetaDataAsync(string assemblyName, string baseTypes)
        {
            this.GetAssemblyMetaDataAsync(assemblyName, baseTypes, null);
        }

        public void GetAssemblyMetaDataAsync(string assemblyName, string baseTypes, object userState)
        {
            if (this.GetAssemblyMetaDataOperationCompleted == null)
            {
                this.GetAssemblyMetaDataOperationCompleted =
                    new SendOrPostCallback(this.OnGetAssemblyMetaDataOperationCompleted);
            }

            object[] objArray = new object[] { assemblyName, baseTypes };
            base.InvokeAsync("GetAssemblyMetaData", objArray, this.GetAssemblyMetaDataOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetBindingResourceData",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetBindingResourceData(string ResourceName)
        {
            object[] resourceName = new object[] { ResourceName };
            return (string)base.Invoke("GetBindingResourceData", resourceName)[0];
        }

        public void GetBindingResourceDataAsync(string ResourceName)
        {
            this.GetBindingResourceDataAsync(ResourceName, null);
        }

        public void GetBindingResourceDataAsync(string ResourceName, object userState)
        {
            if (this.GetBindingResourceDataOperationCompleted == null)
            {
                this.GetBindingResourceDataOperationCompleted =
                    new SendOrPostCallback(this.OnGetBindingResourceDataOperationCompleted);
            }

            object[] resourceName = new object[] { ResourceName };
            base.InvokeAsync("GetBindingResourceData", resourceName, this.GetBindingResourceDataOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetCustomControlList",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetCustomControlList()
        {
            object[] objArray = base.Invoke("GetCustomControlList", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetCustomControlListAsync()
        {
            this.GetCustomControlListAsync(null);
        }

        public void GetCustomControlListAsync(object userState)
        {
            if (this.GetCustomControlListOperationCompleted == null)
            {
                this.GetCustomControlListOperationCompleted =
                    new SendOrPostCallback(this.OnGetCustomControlListOperationCompleted);
            }

            base.InvokeAsync("GetCustomControlList", new object[0], this.GetCustomControlListOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetDataFromDataSourceControl",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetDataFromDataSourceControl(string dscXml, string contextUrl)
        {
            object[] objArray = new object[] { dscXml, contextUrl };
            return (string)base.Invoke("GetDataFromDataSourceControl", objArray)[0];
        }

        public void GetDataFromDataSourceControlAsync(string dscXml, string contextUrl)
        {
            this.GetDataFromDataSourceControlAsync(dscXml, contextUrl, null);
        }

        public void GetDataFromDataSourceControlAsync(string dscXml, string contextUrl, object userState)
        {
            if (this.GetDataFromDataSourceControlOperationCompleted == null)
            {
                this.GetDataFromDataSourceControlOperationCompleted =
                    new SendOrPostCallback(this.OnGetDataFromDataSourceControlOperationCompleted);
            }

            object[] objArray = new object[] { dscXml, contextUrl };
            base.InvokeAsync("GetDataFromDataSourceControl", objArray,
                this.GetDataFromDataSourceControlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetFormCapabilityFromDataSourceControl",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFormCapabilityFromDataSourceControl(string dscXml)
        {
            object[] objArray = new object[] { dscXml };
            return (string)base.Invoke("GetFormCapabilityFromDataSourceControl", objArray)[0];
        }

        public void GetFormCapabilityFromDataSourceControlAsync(string dscXml)
        {
            this.GetFormCapabilityFromDataSourceControlAsync(dscXml, null);
        }

        public void GetFormCapabilityFromDataSourceControlAsync(string dscXml, object userState)
        {
            if (this.GetFormCapabilityFromDataSourceControlOperationCompleted == null)
            {
                this.GetFormCapabilityFromDataSourceControlOperationCompleted =
                    new SendOrPostCallback(this.OnGetFormCapabilityFromDataSourceControlOperationCompleted);
            }

            object[] objArray = new object[] { dscXml };
            base.InvokeAsync("GetFormCapabilityFromDataSourceControl", objArray,
                this.GetFormCapabilityFromDataSourceControlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetSafeAssemblyInfo",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetSafeAssemblyInfo()
        {
            object[] objArray = base.Invoke("GetSafeAssemblyInfo", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetSafeAssemblyInfoAsync()
        {
            this.GetSafeAssemblyInfoAsync(null);
        }

        public void GetSafeAssemblyInfoAsync(object userState)
        {
            if (this.GetSafeAssemblyInfoOperationCompleted == null)
            {
                this.GetSafeAssemblyInfoOperationCompleted =
                    new SendOrPostCallback(this.OnGetSafeAssemblyInfoOperationCompleted);
            }

            base.InvokeAsync("GetSafeAssemblyInfo", new object[0], this.GetSafeAssemblyInfoOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPart",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPart(string pageurl, Guid storageKey, Storage storage)
        {
            object[] objArray = new object[] { pageurl, storageKey, storage };
            return (string)base.Invoke("GetWebPart", objArray)[0];
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPart2",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPart2(string pageurl, Guid storageKey, Storage storage, SPWebServiceBehavior behavior)
        {
            object[] objArray = new object[] { pageurl, storageKey, storage, behavior };
            return (string)base.Invoke("GetWebPart2", objArray)[0];
        }

        public void GetWebPart2Async(string pageurl, Guid storageKey, Storage storage, SPWebServiceBehavior behavior)
        {
            this.GetWebPart2Async(pageurl, storageKey, storage, behavior, null);
        }

        public void GetWebPart2Async(string pageurl, Guid storageKey, Storage storage, SPWebServiceBehavior behavior,
            object userState)
        {
            if (this.GetWebPart2OperationCompleted == null)
            {
                this.GetWebPart2OperationCompleted = new SendOrPostCallback(this.OnGetWebPart2OperationCompleted);
            }

            object[] objArray = new object[] { pageurl, storageKey, storage, behavior };
            base.InvokeAsync("GetWebPart2", objArray, this.GetWebPart2OperationCompleted, userState);
        }

        public void GetWebPartAsync(string pageurl, Guid storageKey, Storage storage)
        {
            this.GetWebPartAsync(pageurl, storageKey, storage, null);
        }

        public void GetWebPartAsync(string pageurl, Guid storageKey, Storage storage, object userState)
        {
            if (this.GetWebPartOperationCompleted == null)
            {
                this.GetWebPartOperationCompleted = new SendOrPostCallback(this.OnGetWebPartOperationCompleted);
            }

            object[] objArray = new object[] { pageurl, storageKey, storage };
            base.InvokeAsync("GetWebPart", objArray, this.GetWebPartOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartCrossPageCompatibility",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartCrossPageCompatibility(string sourcePageUrl, string sourcePageContents,
            string targetPageUrl, string targetPageContents, string providerPartID, string lcid)
        {
            object[] objArray = new object[]
                { sourcePageUrl, sourcePageContents, targetPageUrl, targetPageContents, providerPartID, lcid };
            return (string)base.Invoke("GetWebPartCrossPageCompatibility", objArray)[0];
        }

        public void GetWebPartCrossPageCompatibilityAsync(string sourcePageUrl, string sourcePageContents,
            string targetPageUrl, string targetPageContents, string providerPartID, string lcid)
        {
            this.GetWebPartCrossPageCompatibilityAsync(sourcePageUrl, sourcePageContents, targetPageUrl,
                targetPageContents, providerPartID, lcid, null);
        }

        public void GetWebPartCrossPageCompatibilityAsync(string sourcePageUrl, string sourcePageContents,
            string targetPageUrl, string targetPageContents, string providerPartID, string lcid, object userState)
        {
            if (this.GetWebPartCrossPageCompatibilityOperationCompleted == null)
            {
                this.GetWebPartCrossPageCompatibilityOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartCrossPageCompatibilityOperationCompleted);
            }

            object[] objArray = new object[]
                { sourcePageUrl, sourcePageContents, targetPageUrl, targetPageContents, providerPartID, lcid };
            base.InvokeAsync("GetWebPartCrossPageCompatibility", objArray,
                this.GetWebPartCrossPageCompatibilityOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartPage",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartPage(string documentName, SPWebServiceBehavior behavior)
        {
            object[] objArray = new object[] { documentName, behavior };
            return (string)base.Invoke("GetWebPartPage", objArray)[0];
        }

        public void GetWebPartPageAsync(string documentName, SPWebServiceBehavior behavior)
        {
            this.GetWebPartPageAsync(documentName, behavior, null);
        }

        public void GetWebPartPageAsync(string documentName, SPWebServiceBehavior behavior, object userState)
        {
            if (this.GetWebPartPageOperationCompleted == null)
            {
                this.GetWebPartPageOperationCompleted = new SendOrPostCallback(this.OnGetWebPartPageOperationCompleted);
            }

            object[] objArray = new object[] { documentName, behavior };
            base.InvokeAsync("GetWebPartPage", objArray, this.GetWebPartPageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartPageConnectionInfo",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartPageConnectionInfo(string sourcePageUrl, string sourcePageContents,
            string providerPartID, string lcid)
        {
            object[] objArray = new object[] { sourcePageUrl, sourcePageContents, providerPartID, lcid };
            return (string)base.Invoke("GetWebPartPageConnectionInfo", objArray)[0];
        }

        public void GetWebPartPageConnectionInfoAsync(string sourcePageUrl, string sourcePageContents,
            string providerPartID, string lcid)
        {
            this.GetWebPartPageConnectionInfoAsync(sourcePageUrl, sourcePageContents, providerPartID, lcid, null);
        }

        public void GetWebPartPageConnectionInfoAsync(string sourcePageUrl, string sourcePageContents,
            string providerPartID, string lcid, object userState)
        {
            if (this.GetWebPartPageConnectionInfoOperationCompleted == null)
            {
                this.GetWebPartPageConnectionInfoOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartPageConnectionInfoOperationCompleted);
            }

            object[] objArray = new object[] { sourcePageUrl, sourcePageContents, providerPartID, lcid };
            base.InvokeAsync("GetWebPartPageConnectionInfo", objArray,
                this.GetWebPartPageConnectionInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartPageDocument",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartPageDocument(string documentName)
        {
            object[] objArray = new object[] { documentName };
            return (string)base.Invoke("GetWebPartPageDocument", objArray)[0];
        }

        public void GetWebPartPageDocumentAsync(string documentName)
        {
            this.GetWebPartPageDocumentAsync(documentName, null);
        }

        public void GetWebPartPageDocumentAsync(string documentName, object userState)
        {
            if (this.GetWebPartPageDocumentOperationCompleted == null)
            {
                this.GetWebPartPageDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartPageDocumentOperationCompleted);
            }

            object[] objArray = new object[] { documentName };
            base.InvokeAsync("GetWebPartPageDocument", objArray, this.GetWebPartPageDocumentOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartProperties",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetWebPartProperties(string pageUrl, Storage storage)
        {
            object[] objArray = new object[] { pageUrl, storage };
            return (XmlNode)base.Invoke("GetWebPartProperties", objArray)[0];
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetWebPartProperties2",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetWebPartProperties2(string pageUrl, Storage storage, SPWebServiceBehavior behavior)
        {
            object[] objArray = new object[] { pageUrl, storage, behavior };
            return (XmlNode)base.Invoke("GetWebPartProperties2", objArray)[0];
        }

        public void GetWebPartProperties2Async(string pageUrl, Storage storage, SPWebServiceBehavior behavior)
        {
            this.GetWebPartProperties2Async(pageUrl, storage, behavior, null);
        }

        public void GetWebPartProperties2Async(string pageUrl, Storage storage, SPWebServiceBehavior behavior,
            object userState)
        {
            if (this.GetWebPartProperties2OperationCompleted == null)
            {
                this.GetWebPartProperties2OperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartProperties2OperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, storage, behavior };
            base.InvokeAsync("GetWebPartProperties2", objArray, this.GetWebPartProperties2OperationCompleted,
                userState);
        }

        public void GetWebPartPropertiesAsync(string pageUrl, Storage storage)
        {
            this.GetWebPartPropertiesAsync(pageUrl, storage, null);
        }

        public void GetWebPartPropertiesAsync(string pageUrl, Storage storage, object userState)
        {
            if (this.GetWebPartPropertiesOperationCompleted == null)
            {
                this.GetWebPartPropertiesOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartPropertiesOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, storage };
            base.InvokeAsync("GetWebPartProperties", objArray, this.GetWebPartPropertiesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/GetXmlDataFromDataSource",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetXmlDataFromDataSource(string queryXml)
        {
            object[] objArray = new object[] { queryXml };
            return (string)base.Invoke("GetXmlDataFromDataSource", objArray)[0];
        }

        public void GetXmlDataFromDataSourceAsync(string queryXml)
        {
            this.GetXmlDataFromDataSourceAsync(queryXml, null);
        }

        public void GetXmlDataFromDataSourceAsync(string queryXml, object userState)
        {
            if (this.GetXmlDataFromDataSourceOperationCompleted == null)
            {
                this.GetXmlDataFromDataSourceOperationCompleted =
                    new SendOrPostCallback(this.OnGetXmlDataFromDataSourceOperationCompleted);
            }

            object[] objArray = new object[] { queryXml };
            base.InvokeAsync("GetXmlDataFromDataSource", objArray, this.GetXmlDataFromDataSourceOperationCompleted,
                userState);
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

        private void OnAddWebPartOperationCompleted(object arg)
        {
            if (this.AddWebPartCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWebPartCompleted(this,
                    new AddWebPartCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddWebPartToZoneOperationCompleted(object arg)
        {
            if (this.AddWebPartToZoneCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWebPartToZoneCompleted(this,
                    new AddWebPartToZoneCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAssociateWorkflowMarkupOperationCompleted(object arg)
        {
            if (this.AssociateWorkflowMarkupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AssociateWorkflowMarkupCompleted(this,
                    new AssociateWorkflowMarkupCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnConvertWebPartFormatOperationCompleted(object arg)
        {
            if (this.ConvertWebPartFormatCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ConvertWebPartFormatCompleted(this,
                    new ConvertWebPartFormatCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteWebPartOperationCompleted(object arg)
        {
            if (this.DeleteWebPartCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteWebPartCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnExecuteProxyUpdatesOperationCompleted(object arg)
        {
            if (this.ExecuteProxyUpdatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ExecuteProxyUpdatesCompleted(this,
                    new ExecuteProxyUpdatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnFetchLegalWorkflowActionsOperationCompleted(object arg)
        {
            if (this.FetchLegalWorkflowActionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.FetchLegalWorkflowActionsCompleted(this,
                    new FetchLegalWorkflowActionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAssemblyMetaDataOperationCompleted(object arg)
        {
            if (this.GetAssemblyMetaDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAssemblyMetaDataCompleted(this,
                    new GetAssemblyMetaDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetBindingResourceDataOperationCompleted(object arg)
        {
            if (this.GetBindingResourceDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetBindingResourceDataCompleted(this,
                    new GetBindingResourceDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCustomControlListOperationCompleted(object arg)
        {
            if (this.GetCustomControlListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCustomControlListCompleted(this,
                    new GetCustomControlListCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDataFromDataSourceControlOperationCompleted(object arg)
        {
            if (this.GetDataFromDataSourceControlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDataFromDataSourceControlCompleted(this,
                    new GetDataFromDataSourceControlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFormCapabilityFromDataSourceControlOperationCompleted(object arg)
        {
            if (this.GetFormCapabilityFromDataSourceControlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFormCapabilityFromDataSourceControlCompleted(this,
                    new GetFormCapabilityFromDataSourceControlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSafeAssemblyInfoOperationCompleted(object arg)
        {
            if (this.GetSafeAssemblyInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSafeAssemblyInfoCompleted(this,
                    new GetSafeAssemblyInfoCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPart2OperationCompleted(object arg)
        {
            if (this.GetWebPart2Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPart2Completed(this,
                    new GetWebPart2CompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartCrossPageCompatibilityOperationCompleted(object arg)
        {
            if (this.GetWebPartCrossPageCompatibilityCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartCrossPageCompatibilityCompleted(this,
                    new GetWebPartCrossPageCompatibilityCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartOperationCompleted(object arg)
        {
            if (this.GetWebPartCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartCompleted(this,
                    new GetWebPartCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartPageConnectionInfoOperationCompleted(object arg)
        {
            if (this.GetWebPartPageConnectionInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartPageConnectionInfoCompleted(this,
                    new GetWebPartPageConnectionInfoCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartPageDocumentOperationCompleted(object arg)
        {
            if (this.GetWebPartPageDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartPageDocumentCompleted(this,
                    new GetWebPartPageDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartPageOperationCompleted(object arg)
        {
            if (this.GetWebPartPageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartPageCompleted(this,
                    new GetWebPartPageCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartProperties2OperationCompleted(object arg)
        {
            if (this.GetWebPartProperties2Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartProperties2Completed(this,
                    new GetWebPartProperties2CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartPropertiesOperationCompleted(object arg)
        {
            if (this.GetWebPartPropertiesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartPropertiesCompleted(this,
                    new GetWebPartPropertiesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetXmlDataFromDataSourceOperationCompleted(object arg)
        {
            if (this.GetXmlDataFromDataSourceCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetXmlDataFromDataSourceCompleted(this,
                    new GetXmlDataFromDataSourceCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveWorkflowAssociationOperationCompleted(object arg)
        {
            if (this.RemoveWorkflowAssociationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveWorkflowAssociationCompleted(this,
                    new RemoveWorkflowAssociationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRenderWebPartForEditOperationCompleted(object arg)
        {
            if (this.RenderWebPartForEditCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RenderWebPartForEditCompleted(this,
                    new RenderWebPartForEditCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSaveWebPart2OperationCompleted(object arg)
        {
            if (this.SaveWebPart2Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SaveWebPart2Completed(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSaveWebPartOperationCompleted(object arg)
        {
            if (this.SaveWebPartCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SaveWebPartCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted(object arg)
        {
            if (this.ValidateWorkflowMarkupAndCreateSupportObjectsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ValidateWorkflowMarkupAndCreateSupportObjectsCompleted(this,
                    new ValidateWorkflowMarkupAndCreateSupportObjectsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/RemoveWorkflowAssociation",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string RemoveWorkflowAssociation(string configUrl, string configVersion)
        {
            object[] objArray = new object[] { configUrl, configVersion };
            return (string)base.Invoke("RemoveWorkflowAssociation", objArray)[0];
        }

        public void RemoveWorkflowAssociationAsync(string configUrl, string configVersion)
        {
            this.RemoveWorkflowAssociationAsync(configUrl, configVersion, null);
        }

        public void RemoveWorkflowAssociationAsync(string configUrl, string configVersion, object userState)
        {
            if (this.RemoveWorkflowAssociationOperationCompleted == null)
            {
                this.RemoveWorkflowAssociationOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveWorkflowAssociationOperationCompleted);
            }

            object[] objArray = new object[] { configUrl, configVersion };
            base.InvokeAsync("RemoveWorkflowAssociation", objArray, this.RemoveWorkflowAssociationOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/RenderWebPartForEdit",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string RenderWebPartForEdit(string webPartXml)
        {
            object[] objArray = new object[] { webPartXml };
            return (string)base.Invoke("RenderWebPartForEdit", objArray)[0];
        }

        public void RenderWebPartForEditAsync(string webPartXml)
        {
            this.RenderWebPartForEditAsync(webPartXml, null);
        }

        public void RenderWebPartForEditAsync(string webPartXml, object userState)
        {
            if (this.RenderWebPartForEditOperationCompleted == null)
            {
                this.RenderWebPartForEditOperationCompleted =
                    new SendOrPostCallback(this.OnRenderWebPartForEditOperationCompleted);
            }

            object[] objArray = new object[] { webPartXml };
            base.InvokeAsync("RenderWebPartForEdit", objArray, this.RenderWebPartForEditOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/SaveWebPart",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SaveWebPart(string pageUrl, Guid storageKey, string webPartXml, Storage storage)
        {
            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage };
            base.Invoke("SaveWebPart", objArray);
        }

        [SoapDocumentMethod("http://microsoft.com/sharepoint/webpartpages/SaveWebPart2",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SaveWebPart2(string pageUrl, Guid storageKey, string webPartXml, Storage storage,
            bool allowTypeChange)
        {
            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage, allowTypeChange };
            base.Invoke("SaveWebPart2", objArray);
        }

        public void SaveWebPart2Async(string pageUrl, Guid storageKey, string webPartXml, Storage storage,
            bool allowTypeChange)
        {
            this.SaveWebPart2Async(pageUrl, storageKey, webPartXml, storage, allowTypeChange, null);
        }

        public void SaveWebPart2Async(string pageUrl, Guid storageKey, string webPartXml, Storage storage,
            bool allowTypeChange, object userState)
        {
            if (this.SaveWebPart2OperationCompleted == null)
            {
                this.SaveWebPart2OperationCompleted = new SendOrPostCallback(this.OnSaveWebPart2OperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage, allowTypeChange };
            base.InvokeAsync("SaveWebPart2", objArray, this.SaveWebPart2OperationCompleted, userState);
        }

        public void SaveWebPartAsync(string pageUrl, Guid storageKey, string webPartXml, Storage storage)
        {
            this.SaveWebPartAsync(pageUrl, storageKey, webPartXml, storage, null);
        }

        public void SaveWebPartAsync(string pageUrl, Guid storageKey, string webPartXml, Storage storage,
            object userState)
        {
            if (this.SaveWebPartOperationCompleted == null)
            {
                this.SaveWebPartOperationCompleted = new SendOrPostCallback(this.OnSaveWebPartOperationCompleted);
            }

            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage };
            base.InvokeAsync("SaveWebPart", objArray, this.SaveWebPartOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/sharepoint/webpartpages/ValidateWorkflowMarkupAndCreateSupportObjects",
            RequestNamespace = "http://microsoft.com/sharepoint/webpartpages",
            ResponseNamespace = "http://microsoft.com/sharepoint/webpartpages", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ValidateWorkflowMarkupAndCreateSupportObjects(string workflowMarkupText, string rulesText,
            string configBlob, string flag)
        {
            object[] objArray = new object[] { workflowMarkupText, rulesText, configBlob, flag };
            return (string)base.Invoke("ValidateWorkflowMarkupAndCreateSupportObjects", objArray)[0];
        }

        public void ValidateWorkflowMarkupAndCreateSupportObjectsAsync(string workflowMarkupText, string rulesText,
            string configBlob, string flag)
        {
            this.ValidateWorkflowMarkupAndCreateSupportObjectsAsync(workflowMarkupText, rulesText, configBlob, flag,
                null);
        }

        public void ValidateWorkflowMarkupAndCreateSupportObjectsAsync(string workflowMarkupText, string rulesText,
            string configBlob, string flag, object userState)
        {
            if (this.ValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted == null)
            {
                this.ValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted =
                    new SendOrPostCallback(this.OnValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted);
            }

            object[] objArray = new object[] { workflowMarkupText, rulesText, configBlob, flag };
            base.InvokeAsync("ValidateWorkflowMarkupAndCreateSupportObjects", objArray,
                this.ValidateWorkflowMarkupAndCreateSupportObjectsOperationCompleted, userState);
        }

        public event AddWebPartCompletedEventHandler AddWebPartCompleted;

        public event AddWebPartToZoneCompletedEventHandler AddWebPartToZoneCompleted;

        public event AssociateWorkflowMarkupCompletedEventHandler AssociateWorkflowMarkupCompleted;

        public event ConvertWebPartFormatCompletedEventHandler ConvertWebPartFormatCompleted;

        public event DeleteWebPartCompletedEventHandler DeleteWebPartCompleted;

        public event ExecuteProxyUpdatesCompletedEventHandler ExecuteProxyUpdatesCompleted;

        public event FetchLegalWorkflowActionsCompletedEventHandler FetchLegalWorkflowActionsCompleted;

        public event GetAssemblyMetaDataCompletedEventHandler GetAssemblyMetaDataCompleted;

        public event GetBindingResourceDataCompletedEventHandler GetBindingResourceDataCompleted;

        public event GetCustomControlListCompletedEventHandler GetCustomControlListCompleted;

        public event GetDataFromDataSourceControlCompletedEventHandler GetDataFromDataSourceControlCompleted;

        public event GetFormCapabilityFromDataSourceControlCompletedEventHandler
            GetFormCapabilityFromDataSourceControlCompleted;

        public event GetSafeAssemblyInfoCompletedEventHandler GetSafeAssemblyInfoCompleted;

        public event GetWebPart2CompletedEventHandler GetWebPart2Completed;

        public event GetWebPartCompletedEventHandler GetWebPartCompleted;

        public event GetWebPartCrossPageCompatibilityCompletedEventHandler GetWebPartCrossPageCompatibilityCompleted;

        public event GetWebPartPageCompletedEventHandler GetWebPartPageCompleted;

        public event GetWebPartPageConnectionInfoCompletedEventHandler GetWebPartPageConnectionInfoCompleted;

        public event GetWebPartPageDocumentCompletedEventHandler GetWebPartPageDocumentCompleted;

        public event GetWebPartProperties2CompletedEventHandler GetWebPartProperties2Completed;

        public event GetWebPartPropertiesCompletedEventHandler GetWebPartPropertiesCompleted;

        public event GetXmlDataFromDataSourceCompletedEventHandler GetXmlDataFromDataSourceCompleted;

        public event RemoveWorkflowAssociationCompletedEventHandler RemoveWorkflowAssociationCompleted;

        public event RenderWebPartForEditCompletedEventHandler RenderWebPartForEditCompleted;

        public event SaveWebPart2CompletedEventHandler SaveWebPart2Completed;

        public event SaveWebPartCompletedEventHandler SaveWebPartCompleted;

        public event ValidateWorkflowMarkupAndCreateSupportObjectsCompletedEventHandler
            ValidateWorkflowMarkupAndCreateSupportObjectsCompleted;
    }
}