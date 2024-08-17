using Metalogix.SharePoint.Adapters.MLWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.18408")]
    [WebServiceBinding(Name = "MLSPExtensionsWebServiceSoap", Namespace = "http://www.metalogix.net/")]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AdapterOptions))]
    public class MLSPExtensionsWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback AddWebOperationCompleted;

        private SendOrPostCallback UpdateWebOperationCompleted;

        private SendOrPostCallback AddListOperationCompleted;

        private SendOrPostCallback AddViewOperationCompleted;

        private SendOrPostCallback UpdateListOperationCompleted;

        private SendOrPostCallback AddOrUpdateContentTypeOperationCompleted;

        private SendOrPostCallback ApplyOrUpdateContentTypeOperationCompleted;

        private SendOrPostCallback ReorderContentTypesOperationCompleted;

        private SendOrPostCallback AddFieldsOperationCompleted;

        private SendOrPostCallback DeleteItemOperationCompleted;

        private SendOrPostCallback DeleteItemsOperationCompleted;

        private SendOrPostCallback DeleteFolderOperationCompleted;

        private SendOrPostCallback DeleteListOperationCompleted;

        private SendOrPostCallback DeleteWebOperationCompleted;

        private SendOrPostCallback AddFolderOperationCompleted;

        private SendOrPostCallback AddFolderToFolderOperationCompleted;

        private SendOrPostCallback AddListItemOperationCompleted;

        private SendOrPostCallback UpdateListItemOperationCompleted;

        private SendOrPostCallback AddDocumentOperationCompleted;

        private SendOrPostCallback AddFileToFolderOperationCompleted;

        private SendOrPostCallback UpdateDocumentOperationCompleted;

        private SendOrPostCallback AddSiteUserOperationCompleted;

        private SendOrPostCallback AddRoleAssignmentOperationCompleted;

        private SendOrPostCallback AddOrUpdateRoleOperationCompleted;

        private SendOrPostCallback AddOrUpdateGroupOperationCompleted;

        private SendOrPostCallback UpdateGroupQuickLaunchOperationCompleted;

        private SendOrPostCallback AddAlertsOperationCompleted;

        private SendOrPostCallback AddWorkflowAssociationOperationCompleted;

        private SendOrPostCallback ActivateReusableWorkflowTemplatesOperationCompleted;

        private SendOrPostCallback AddWorkflowOperationCompleted;

        private SendOrPostCallback BeginCompilingAllAudiencesOperationCompleted;

        private SendOrPostCallback DeleteAllAudiencesOperationCompleted;

        private SendOrPostCallback DeleteAudienceOperationCompleted;

        private SendOrPostCallback DeleteRoleAssignmentOperationCompleted;

        private SendOrPostCallback DeleteRoleOperationCompleted;

        private SendOrPostCallback DeleteGroupOperationCompleted;

        private SendOrPostCallback DeleteContentTypesOperationCompleted;

        private SendOrPostCallback AddWebPartsOperationCompleted;

        private SendOrPostCallback DeleteWebPartOperationCompleted;

        private SendOrPostCallback DeleteWebPartsOperationCompleted;

        private SendOrPostCallback CloseWebPartsOperationCompleted;

        private SendOrPostCallback UpdateListItemStatusOperationCompleted;

        private SendOrPostCallback SetWelcomePageOperationCompleted;

        private SendOrPostCallback SetMasterPageOperationCompleted;

        private SendOrPostCallback CorrectDefaultPageVersionsOperationCompleted;

        private SendOrPostCallback ValidateUserInfoOperationCompleted;

        private SendOrPostCallback GetWebApplicationsOperationCompleted;

        private SendOrPostCallback GetLanguagesAndWebTemplatesOperationCompleted;

        private SendOrPostCallback GetSiteQuotaTemplatesOperationCompleted;

        private SendOrPostCallback AddSiteCollectionOperationCompleted;

        private SendOrPostCallback UpdateSiteCollectionSettingsOperationCompleted;

        private SendOrPostCallback DeleteSiteCollectionOperationCompleted;

        private SendOrPostCallback AddDocumentTemplatetoContentTypeOperationCompleted;

        private SendOrPostCallback UpdateWebNavigationStructureOperationCompleted;

        private SendOrPostCallback SetDocumentParsingOperationCompleted;

        private SendOrPostCallback ModifyWebNavigationSettingsOperationCompleted;

        private SendOrPostCallback GetUserProfilesOperationCompleted;

        private SendOrPostCallback GetAudiencesOperationCompleted;

        private SendOrPostCallback SetUserProfileOperationCompleted;

        private SendOrPostCallback AddOrUpdateAudienceOperationCompleted;

        private SendOrPostCallback GetMySiteDataOperationCompleted;

        private SendOrPostCallback AddFormTemplateToContentTypeOperationCompleted;

        private SendOrPostCallback Apply2013ThemeOperationCompleted;

        private SendOrPostCallback AddTermstoreLanguagesOperationCompleted;

        private SendOrPostCallback AddTermGroupOperationCompleted;

        private SendOrPostCallback AddReferencedTaxonomyDataOperationCompleted;

        private SendOrPostCallback AddReusedTermsOperationCompleted;

        private SendOrPostCallback AddTermSetOperationCompleted;

        private SendOrPostCallback AddTermOperationCompleted;

        private SendOrPostCallback GetSiteCollectionsOperationCompleted;

        private SendOrPostCallback GetSiteCollectionsOnWebAppOperationCompleted;

        private SendOrPostCallback GetSiteOperationCompleted;

        private SendOrPostCallback GetWebOperationCompleted;

        private SendOrPostCallback GetWebNavigationSettingsOperationCompleted;

        private SendOrPostCallback GetWebNavigationStructureOperationCompleted;

        private SendOrPostCallback GetWebTemplatesOperationCompleted;

        private SendOrPostCallback GetSubWebsOperationCompleted;

        private SendOrPostCallback GetListsOperationCompleted;

        private SendOrPostCallback GetListOperationCompleted;

        private SendOrPostCallback GetFoldersOperationCompleted;

        private SendOrPostCallback GetListItemIDsOperationCompleted;

        private SendOrPostCallback GetListItemsOperationCompleted;

        private SendOrPostCallback GetListItemsByQueryOperationCompleted;

        private SendOrPostCallback GetListItemVersionsOperationCompleted;

        private SendOrPostCallback GetAttachmentsOperationCompleted;

        private SendOrPostCallback GetFieldsOperationCompleted;

        private SendOrPostCallback GetSiteUsersOperationCompleted;

        private SendOrPostCallback GetListTemplatesOperationCompleted;

        private SendOrPostCallback GetDocumentOperationCompleted;

        private SendOrPostCallback GetDocumentVersionOperationCompleted;

        private SendOrPostCallback GetDocumentBlobRefOperationCompleted;

        private SendOrPostCallback GetDocumentVersionBlobRefOperationCompleted;

        private SendOrPostCallback SearchForDocumentOperationCompleted;

        private SendOrPostCallback AnalyzeChurnOperationCompleted;

        private SendOrPostCallback FindUniquePermissionsOperationCompleted;

        private SendOrPostCallback FindAlertsOperationCompleted;

        private SendOrPostCallback GetAlertsOperationCompleted;

        private SendOrPostCallback GetSystemInfoOperationCompleted;

        private SendOrPostCallback GetSharePointVersionOperationCompleted;

        private SendOrPostCallback GetGroupsOperationCompleted;

        private SendOrPostCallback GetRolesOperationCompleted;

        private SendOrPostCallback GetRoleAssignmentsOperationCompleted;

        private SendOrPostCallback GetContentTypesOperationCompleted;

        private SendOrPostCallback HasDocumentOperationCompleted;

        private SendOrPostCallback GetWebPartPageOperationCompleted;

        private SendOrPostCallback HasWebPartsOperationCompleted;

        private SendOrPostCallback GetDocumentIdOperationCompleted;

        private SendOrPostCallback GetWebPartPageTemplateOperationCompleted;

        private SendOrPostCallback GetDashboardPageTemplateOperationCompleted;

        private SendOrPostCallback GetWebPartsOnPageOperationCompleted;

        private SendOrPostCallback GetUserFromProfileOperationCompleted;

        private SendOrPostCallback GetWorkflowsOperationCompleted;

        private SendOrPostCallback GetWorkflowAssociationsOperationCompleted;

        private SendOrPostCallback HasWorkflowsOperationCompleted;

        private SendOrPostCallback HasUniquePermissionsOperationCompleted;

        private SendOrPostCallback IsListContainsInfoPathOrAspxItemOperationCompleted;

        private SendOrPostCallback GetTermStoresOperationCompleted;

        private SendOrPostCallback GetTermGroupsOperationCompleted;

        private SendOrPostCallback GetTermSetsOperationCompleted;

        private SendOrPostCallback GetTermsFromTermSetOperationCompleted;

        private SendOrPostCallback GetTermsFromTermSetItemOperationCompleted;

        private SendOrPostCallback GetExternalContentTypesOperationCompleted;

        private SendOrPostCallback GetExternalItemsOperationCompleted;

        private SendOrPostCallback GetExternalContentTypeOperationsOperationCompleted;

        private SendOrPostCallback GetPortalListingGroupsOperationCompleted;

        private SendOrPostCallback GetPortalListingIDsOperationCompleted;

        private SendOrPostCallback GetPortalListingsOperationCompleted;

        private SendOrPostCallback GetTermSetCollectionOperationCompleted;

        private SendOrPostCallback GetTermCollectionFromTermSetOperationCompleted;

        private SendOrPostCallback GetTermCollectionFromTermOperationCompleted;

        private SendOrPostCallback GetReferencedTaxonomyFullXmlOperationCompleted;

        private SendOrPostCallback GetFilesOperationCompleted;

        private SendOrPostCallback StoragePointAvailableOperationCompleted;

        private SendOrPostCallback GetStoragePointProfileConfigurationOperationCompleted;

        private SendOrPostCallback StoragePointProfileConfiguredOperationCompleted;

        private SendOrPostCallback ConfigureStoragePointFileShareEndpointAndProfileOperationCompleted;

        private SendOrPostCallback CatalogDocumentToStoragePointFileShareEndpointOperationCompleted;

        private SendOrPostCallback AddListItemChunkedOperationCompleted;

        private SendOrPostCallback UpdateListItemChunkedOperationCompleted;

        private SendOrPostCallback AddDocumentChunkedOperationCompleted;

        private SendOrPostCallback UpdateDocumentChunkedOperationCompleted;

        private SendOrPostCallback GetDocumentChunkedOperationCompleted;

        private SendOrPostCallback GetDocumentVersionChunkedOperationCompleted;

        private SendOrPostCallback OpenFileCopySessionOperationCompleted;

        private SendOrPostCallback WriteChunkOperationCompleted;

        private SendOrPostCallback ReadChunkOperationCompleted;

        private SendOrPostCallback CloseFileCopySessionOperationCompleted;

        private SendOrPostCallback AddDocumentOptimisticallyOperationCompleted;

        private SendOrPostCallback AddFolderOptimisticallyOperationCompleted;

        private SendOrPostCallback GetSP2013WorkflowsOperationCompleted;

        private SendOrPostCallback MigrateSP2013WorkflowsOperationCompleted;

        private SendOrPostCallback DeleteSP2013WorkflowsOperationCompleted;

        private SendOrPostCallback IsWorkflowServicesInstanceAvailableOperationCompleted;

        private SendOrPostCallback ExecuteCommandOperationCompleted;

        private SendOrPostCallback GetLockedSitesOperationCompleted;

        private SendOrPostCallback GetAddInsOperationCompleted;

        private SendOrPostCallback GetSiteSolutionsBinaryOperationCompleted;

        private SendOrPostCallback GetFarmSandboxSolutionsOperationCompleted;

        private SendOrPostCallback IsAppWebPartOnPageOperationCompleted;

        private SendOrPostCallback GetFarmSolutionsOperationCompleted;

        private SendOrPostCallback GetFarmSolutionBinaryOperationCompleted;

        private SendOrPostCallback GetListWorkflowRunning2010OperationCompleted;

        private SendOrPostCallback GetBrowserFileHandlingOperationCompleted;

        private SendOrPostCallback GetBcsApplicationsOperationCompleted;

        private SendOrPostCallback GetCustomProfilePropertyMappingOperationCompleted;

        private SendOrPostCallback GetInfopathsOperationCompleted;

        private SendOrPostCallback GetSecureStorageApplicationsOperationCompleted;

        private SendOrPostCallback GetWebApplicationPoliciesOperationCompleted;

        private SendOrPostCallback GetWorkflowAssociation2013OperationCompleted;

        private SendOrPostCallback GetWorkflowAssociation2010OperationCompleted;

        private SendOrPostCallback GetWorkflowRunning2013OperationCompleted;

        private SendOrPostCallback GetWorkflowRunning2010OperationCompleted;

        private SendOrPostCallback GetFileVersionsOperationCompleted;

        private SendOrPostCallback GetUserProfilePropertiesUsageOperationCompleted;

        private SendOrPostCallback GetAdImportDcMappingsOperationCompleted;

        private SendOrPostCallback GetFarmServerDetailsOperationCompleted;

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

        public MLSPExtensionsWebService()
        {
            this.Url = Settings.Default
                .Metalogix_SharePoint_Adapters_MLWS_MLSPExtensionsWebServiceRef_MLSPExtensionsWebService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://www.metalogix.net/ActivateReusableWorkflowTemplates",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ActivateReusableWorkflowTemplates()
        {
            object[] objArray = base.Invoke("ActivateReusableWorkflowTemplates", new object[0]);
            return (string)objArray[0];
        }

        public void ActivateReusableWorkflowTemplatesAsync()
        {
            this.ActivateReusableWorkflowTemplatesAsync(null);
        }

        public void ActivateReusableWorkflowTemplatesAsync(object userState)
        {
            if (this.ActivateReusableWorkflowTemplatesOperationCompleted == null)
            {
                this.ActivateReusableWorkflowTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnActivateReusableWorkflowTemplatesOperationCompleted);
            }

            base.InvokeAsync("ActivateReusableWorkflowTemplates", new object[0],
                this.ActivateReusableWorkflowTemplatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddAlerts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            object[] objArray = new object[] { sSiteUrl, sWebId, sAlertXML };
            return (string)base.Invoke("AddAlerts", objArray)[0];
        }

        public void AddAlertsAsync(string sSiteUrl, string sWebId, string sAlertXML)
        {
            this.AddAlertsAsync(sSiteUrl, sWebId, sAlertXML, null);
        }

        public void AddAlertsAsync(string sSiteUrl, string sWebId, string sAlertXML, object userState)
        {
            if (this.AddAlertsOperationCompleted == null)
            {
                this.AddAlertsOperationCompleted = new SendOrPostCallback(this.OnAddAlertsOperationCompleted);
            }

            object[] objArray = new object[] { sSiteUrl, sWebId, sAlertXML };
            base.InvokeAsync("AddAlerts", objArray, this.AddAlertsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddDocument", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddDocument(string sListID, string sParentFolder, string sListItemXML,
            [XmlElement(DataType = "base64Binary")] byte[] fileContents, string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml, Options };
            return (string)base.Invoke("AddDocument", objArray)[0];
        }

        public void AddDocumentAsync(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            this.AddDocumentAsync(sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml, Options, null);
        }

        public void AddDocumentAsync(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options, object userState)
        {
            if (this.AddDocumentOperationCompleted == null)
            {
                this.AddDocumentOperationCompleted = new SendOrPostCallback(this.OnAddDocumentOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml, Options };
            base.InvokeAsync("AddDocument", objArray, this.AddDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddDocumentChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddDocumentChunked(string sListID, string sParentFolder, string sListItemXML,
            [XmlElement(DataType = "base64Binary")] byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, isContentMoved, Options };
            return (string)base.Invoke("AddDocumentChunked", objArray)[0];
        }

        public void AddDocumentChunkedAsync(string sListID, string sParentFolder, string sListItemXML,
            byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            this.AddDocumentChunkedAsync(sListID, sParentFolder, sListItemXML, fileContents, isContentMoved, Options,
                null);
        }

        public void AddDocumentChunkedAsync(string sListID, string sParentFolder, string sListItemXML,
            byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options, object userState)
        {
            if (this.AddDocumentChunkedOperationCompleted == null)
            {
                this.AddDocumentChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnAddDocumentChunkedOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, isContentMoved, Options };
            base.InvokeAsync("AddDocumentChunked", objArray, this.AddDocumentChunkedOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddDocumentOptimistically",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            [XmlElement(DataType = "base64Binary")] byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options,
            ref DataSet fieldsLookupCache)
        {
            object[] objArray = new object[]
                { listId, listName, folderPath, fileXml, fileContents, options, fieldsLookupCache };
            object[] objArray1 = base.Invoke("AddDocumentOptimistically", objArray);
            fieldsLookupCache = (DataSet)objArray1[1];
            return (string)objArray1[0];
        }

        public void AddDocumentOptimisticallyAsync(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options,
            DataSet fieldsLookupCache)
        {
            this.AddDocumentOptimisticallyAsync(listId, listName, folderPath, fileXml, fileContents, options,
                fieldsLookupCache, null);
        }

        public void AddDocumentOptimisticallyAsync(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options,
            DataSet fieldsLookupCache, object userState)
        {
            if (this.AddDocumentOptimisticallyOperationCompleted == null)
            {
                this.AddDocumentOptimisticallyOperationCompleted =
                    new SendOrPostCallback(this.OnAddDocumentOptimisticallyOperationCompleted);
            }

            object[] objArray = new object[]
                { listId, listName, folderPath, fileXml, fileContents, options, fieldsLookupCache };
            base.InvokeAsync("AddDocumentOptimistically", objArray, this.AddDocumentOptimisticallyOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddDocumentTemplatetoContentType",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddDocumentTemplatetoContentType([XmlElement(DataType = "base64Binary")] byte[] docTemplate,
            string cTypeXml, string url)
        {
            object[] objArray = new object[] { docTemplate, cTypeXml, url };
            return (string)base.Invoke("AddDocumentTemplatetoContentType", objArray)[0];
        }

        public void AddDocumentTemplatetoContentTypeAsync(byte[] docTemplate, string cTypeXml, string url)
        {
            this.AddDocumentTemplatetoContentTypeAsync(docTemplate, cTypeXml, url, null);
        }

        public void AddDocumentTemplatetoContentTypeAsync(byte[] docTemplate, string cTypeXml, string url,
            object userState)
        {
            if (this.AddDocumentTemplatetoContentTypeOperationCompleted == null)
            {
                this.AddDocumentTemplatetoContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnAddDocumentTemplatetoContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { docTemplate, cTypeXml, url };
            base.InvokeAsync("AddDocumentTemplatetoContentType", objArray,
                this.AddDocumentTemplatetoContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFields", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFields(string sListID, string sFieldXML)
        {
            object[] objArray = new object[] { sListID, sFieldXML };
            return (string)base.Invoke("AddFields", objArray)[0];
        }

        public void AddFieldsAsync(string sListID, string sFieldXML)
        {
            this.AddFieldsAsync(sListID, sFieldXML, null);
        }

        public void AddFieldsAsync(string sListID, string sFieldXML, object userState)
        {
            if (this.AddFieldsOperationCompleted == null)
            {
                this.AddFieldsOperationCompleted = new SendOrPostCallback(this.OnAddFieldsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sFieldXML };
            base.InvokeAsync("AddFields", objArray, this.AddFieldsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFileToFolder", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFileToFolder(string sFileXML, [XmlElement(DataType = "base64Binary")] byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            object[] objArray = new object[] { sFileXML, fileContents, Options };
            return (string)base.Invoke("AddFileToFolder", objArray)[0];
        }

        public void AddFileToFolderAsync(string sFileXML, byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options)
        {
            this.AddFileToFolderAsync(sFileXML, fileContents, Options, null);
        }

        public void AddFileToFolderAsync(string sFileXML, byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions Options, object userState)
        {
            if (this.AddFileToFolderOperationCompleted == null)
            {
                this.AddFileToFolderOperationCompleted =
                    new SendOrPostCallback(this.OnAddFileToFolderOperationCompleted);
            }

            object[] objArray = new object[] { sFileXML, fileContents, Options };
            base.InvokeAsync("AddFileToFolder", objArray, this.AddFileToFolderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFolder", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFolder(string sListID, string sParentFolder, string sFolderXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions Options)
        {
            object[] objArray = new object[] { sListID, sParentFolder, sFolderXML, Options };
            return (string)base.Invoke("AddFolder", objArray)[0];
        }

        public void AddFolderAsync(string sListID, string sParentFolder, string sFolderXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions Options)
        {
            this.AddFolderAsync(sListID, sParentFolder, sFolderXML, Options, null);
        }

        public void AddFolderAsync(string sListID, string sParentFolder, string sFolderXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions Options, object userState)
        {
            if (this.AddFolderOperationCompleted == null)
            {
                this.AddFolderOperationCompleted = new SendOrPostCallback(this.OnAddFolderOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sParentFolder, sFolderXML, Options };
            base.InvokeAsync("AddFolder", objArray, this.AddFolderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFolderOptimistically",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions options,
            ref DataSet fieldsLookupCache)
        {
            object[] objArray = new object[] { listId, listName, folderPath, folderXml, options, fieldsLookupCache };
            object[] objArray1 = base.Invoke("AddFolderOptimistically", objArray);
            fieldsLookupCache = (DataSet)objArray1[1];
            return (string)objArray1[0];
        }

        public void AddFolderOptimisticallyAsync(Guid listId, string listName, string folderPath, string folderXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions options,
            DataSet fieldsLookupCache)
        {
            this.AddFolderOptimisticallyAsync(listId, listName, folderPath, folderXml, options, fieldsLookupCache,
                null);
        }

        public void AddFolderOptimisticallyAsync(Guid listId, string listName, string folderPath, string folderXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions options,
            DataSet fieldsLookupCache, object userState)
        {
            if (this.AddFolderOptimisticallyOperationCompleted == null)
            {
                this.AddFolderOptimisticallyOperationCompleted =
                    new SendOrPostCallback(this.OnAddFolderOptimisticallyOperationCompleted);
            }

            object[] objArray = new object[] { listId, listName, folderPath, folderXml, options, fieldsLookupCache };
            base.InvokeAsync("AddFolderOptimistically", objArray, this.AddFolderOptimisticallyOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFolderToFolder",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFolderToFolder(string sFolderXML)
        {
            object[] objArray = new object[] { sFolderXML };
            return (string)base.Invoke("AddFolderToFolder", objArray)[0];
        }

        public void AddFolderToFolderAsync(string sFolderXML)
        {
            this.AddFolderToFolderAsync(sFolderXML, null);
        }

        public void AddFolderToFolderAsync(string sFolderXML, object userState)
        {
            if (this.AddFolderToFolderOperationCompleted == null)
            {
                this.AddFolderToFolderOperationCompleted =
                    new SendOrPostCallback(this.OnAddFolderToFolderOperationCompleted);
            }

            object[] objArray = new object[] { sFolderXML };
            base.InvokeAsync("AddFolderToFolder", objArray, this.AddFolderToFolderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddFormTemplateToContentType",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddFormTemplateToContentType(string targetListId,
            [XmlElement(DataType = "base64Binary")] byte[] docTemplate, string cTypeXml, string changedLookupFields)
        {
            object[] objArray = new object[] { targetListId, docTemplate, cTypeXml, changedLookupFields };
            return (string)base.Invoke("AddFormTemplateToContentType", objArray)[0];
        }

        public void AddFormTemplateToContentTypeAsync(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            this.AddFormTemplateToContentTypeAsync(targetListId, docTemplate, cTypeXml, changedLookupFields, null);
        }

        public void AddFormTemplateToContentTypeAsync(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields, object userState)
        {
            if (this.AddFormTemplateToContentTypeOperationCompleted == null)
            {
                this.AddFormTemplateToContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnAddFormTemplateToContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { targetListId, docTemplate, cTypeXml, changedLookupFields };
            base.InvokeAsync("AddFormTemplateToContentType", objArray,
                this.AddFormTemplateToContentTypeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddList", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddList(string sListXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListOptions Options,
            [XmlElement(DataType = "base64Binary")] byte[] documentTemplateFile)
        {
            object[] objArray = new object[] { sListXML, Options, documentTemplateFile };
            return (string)base.Invoke("AddList", objArray)[0];
        }

        public void AddListAsync(string sListXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListOptions Options,
            byte[] documentTemplateFile)
        {
            this.AddListAsync(sListXML, Options, documentTemplateFile, null);
        }

        public void AddListAsync(string sListXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListOptions Options,
            byte[] documentTemplateFile, object userState)
        {
            if (this.AddListOperationCompleted == null)
            {
                this.AddListOperationCompleted = new SendOrPostCallback(this.OnAddListOperationCompleted);
            }

            object[] objArray = new object[] { sListXML, Options, documentTemplateFile };
            base.InvokeAsync("AddList", objArray, this.AddListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddListItem", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options)
        {
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, listSettingsXml, Options
            };
            return (string)base.Invoke("AddListItem", objArray)[0];
        }

        public void AddListItemAsync(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options)
        {
            this.AddListItemAsync(sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents,
                listSettingsXml, Options, null);
        }

        public void AddListItemAsync(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, string listSettingsXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options, object userState)
        {
            if (this.AddListItemOperationCompleted == null)
            {
                this.AddListItemOperationCompleted = new SendOrPostCallback(this.OnAddListItemOperationCompleted);
            }

            object[] objArray = new object[]
            {
                sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, listSettingsXml, Options
            };
            base.InvokeAsync("AddListItem", objArray, this.AddListItemOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddListItemChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddListItemChunked(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, isContentMoved, Options };
            return (string)base.Invoke("AddListItemChunked", objArray)[0];
        }

        public void AddListItemChunkedAsync(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options)
        {
            this.AddListItemChunkedAsync(sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents,
                isContentMoved, Options, null);
        }

        public void AddListItemChunkedAsync(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions Options, object userState)
        {
            if (this.AddListItemChunkedOperationCompleted == null)
            {
                this.AddListItemChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnAddListItemChunkedOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, isContentMoved, Options };
            base.InvokeAsync("AddListItemChunked", objArray, this.AddListItemChunkedOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddOrUpdateAudience",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddOrUpdateAudience(string sAudienceXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddAudienceOptions options)
        {
            object[] objArray = new object[] { sAudienceXml, options };
            return (string)base.Invoke("AddOrUpdateAudience", objArray)[0];
        }

        public void AddOrUpdateAudienceAsync(string sAudienceXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddAudienceOptions options)
        {
            this.AddOrUpdateAudienceAsync(sAudienceXml, options, null);
        }

        public void AddOrUpdateAudienceAsync(string sAudienceXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddAudienceOptions options, object userState)
        {
            if (this.AddOrUpdateAudienceOperationCompleted == null)
            {
                this.AddOrUpdateAudienceOperationCompleted =
                    new SendOrPostCallback(this.OnAddOrUpdateAudienceOperationCompleted);
            }

            object[] objArray = new object[] { sAudienceXml, options };
            base.InvokeAsync("AddOrUpdateAudience", objArray, this.AddOrUpdateAudienceOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddOrUpdateContentType",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
        {
            object[] objArray = new object[] { sContentTypeXML, sParentContentTypeName };
            return (string)base.Invoke("AddOrUpdateContentType", objArray)[0];
        }

        public void AddOrUpdateContentTypeAsync(string sContentTypeXML, string sParentContentTypeName)
        {
            this.AddOrUpdateContentTypeAsync(sContentTypeXML, sParentContentTypeName, null);
        }

        public void AddOrUpdateContentTypeAsync(string sContentTypeXML, string sParentContentTypeName, object userState)
        {
            if (this.AddOrUpdateContentTypeOperationCompleted == null)
            {
                this.AddOrUpdateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnAddOrUpdateContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { sContentTypeXML, sParentContentTypeName };
            base.InvokeAsync("AddOrUpdateContentType", objArray, this.AddOrUpdateContentTypeOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddOrUpdateGroup", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddOrUpdateGroup(string sGroupXml)
        {
            object[] objArray = new object[] { sGroupXml };
            return (string)base.Invoke("AddOrUpdateGroup", objArray)[0];
        }

        public void AddOrUpdateGroupAsync(string sGroupXml)
        {
            this.AddOrUpdateGroupAsync(sGroupXml, null);
        }

        public void AddOrUpdateGroupAsync(string sGroupXml, object userState)
        {
            if (this.AddOrUpdateGroupOperationCompleted == null)
            {
                this.AddOrUpdateGroupOperationCompleted =
                    new SendOrPostCallback(this.OnAddOrUpdateGroupOperationCompleted);
            }

            object[] objArray = new object[] { sGroupXml };
            base.InvokeAsync("AddOrUpdateGroup", objArray, this.AddOrUpdateGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddOrUpdateRole", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            object[] objArray = new object[] { sName, sDescription, lPermissionMask };
            return (string)base.Invoke("AddOrUpdateRole", objArray)[0];
        }

        public void AddOrUpdateRoleAsync(string sName, string sDescription, long lPermissionMask)
        {
            this.AddOrUpdateRoleAsync(sName, sDescription, lPermissionMask, null);
        }

        public void AddOrUpdateRoleAsync(string sName, string sDescription, long lPermissionMask, object userState)
        {
            if (this.AddOrUpdateRoleOperationCompleted == null)
            {
                this.AddOrUpdateRoleOperationCompleted =
                    new SendOrPostCallback(this.OnAddOrUpdateRoleOperationCompleted);
            }

            object[] objArray = new object[] { sName, sDescription, lPermissionMask };
            base.InvokeAsync("AddOrUpdateRole", objArray, this.AddOrUpdateRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddReferencedTaxonomyData",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            object[] objArray = new object[] { sReferencedTaxonomyXML };
            return (string)base.Invoke("AddReferencedTaxonomyData", objArray)[0];
        }

        public void AddReferencedTaxonomyDataAsync(string sReferencedTaxonomyXML)
        {
            this.AddReferencedTaxonomyDataAsync(sReferencedTaxonomyXML, null);
        }

        public void AddReferencedTaxonomyDataAsync(string sReferencedTaxonomyXML, object userState)
        {
            if (this.AddReferencedTaxonomyDataOperationCompleted == null)
            {
                this.AddReferencedTaxonomyDataOperationCompleted =
                    new SendOrPostCallback(this.OnAddReferencedTaxonomyDataOperationCompleted);
            }

            object[] objArray = new object[] { sReferencedTaxonomyXML };
            base.InvokeAsync("AddReferencedTaxonomyData", objArray, this.AddReferencedTaxonomyDataOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddReusedTerms", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            object[] objArray = new object[] { sTargetTermStoreGuid, sParentTermCollectionXML };
            return (string)base.Invoke("AddReusedTerms", objArray)[0];
        }

        public void AddReusedTermsAsync(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            this.AddReusedTermsAsync(sTargetTermStoreGuid, sParentTermCollectionXML, null);
        }

        public void AddReusedTermsAsync(string sTargetTermStoreGuid, string sParentTermCollectionXML, object userState)
        {
            if (this.AddReusedTermsOperationCompleted == null)
            {
                this.AddReusedTermsOperationCompleted = new SendOrPostCallback(this.OnAddReusedTermsOperationCompleted);
            }

            object[] objArray = new object[] { sTargetTermStoreGuid, sParentTermCollectionXML };
            base.InvokeAsync("AddReusedTerms", objArray, this.AddReusedTermsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddRoleAssignment",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)base.Invoke("AddRoleAssignment", objArray)[0];
        }

        public void AddRoleAssignmentAsync(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            this.AddRoleAssignmentAsync(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId, null);
        }

        public void AddRoleAssignmentAsync(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId, object userState)
        {
            if (this.AddRoleAssignmentOperationCompleted == null)
            {
                this.AddRoleAssignmentOperationCompleted =
                    new SendOrPostCallback(this.OnAddRoleAssignmentOperationCompleted);
            }

            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            base.InvokeAsync("AddRoleAssignment", objArray, this.AddRoleAssignmentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddSiteCollection",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddSiteCollectionOptions AddSiteCollOptions)
        {
            object[] objArray = new object[] { sWebApp, sSiteCollectionXML, AddSiteCollOptions };
            return (string)base.Invoke("AddSiteCollection", objArray)[0];
        }

        public void AddSiteCollectionAsync(string sWebApp, string sSiteCollectionXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddSiteCollectionOptions AddSiteCollOptions)
        {
            this.AddSiteCollectionAsync(sWebApp, sSiteCollectionXML, AddSiteCollOptions, null);
        }

        public void AddSiteCollectionAsync(string sWebApp, string sSiteCollectionXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddSiteCollectionOptions AddSiteCollOptions,
            object userState)
        {
            if (this.AddSiteCollectionOperationCompleted == null)
            {
                this.AddSiteCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnAddSiteCollectionOperationCompleted);
            }

            object[] objArray = new object[] { sWebApp, sSiteCollectionXML, AddSiteCollOptions };
            base.InvokeAsync("AddSiteCollection", objArray, this.AddSiteCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddSiteUser", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddSiteUser(string sUserXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddUserOptions options)
        {
            object[] objArray = new object[] { sUserXML, options };
            return (string)base.Invoke("AddSiteUser", objArray)[0];
        }

        public void AddSiteUserAsync(string sUserXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddUserOptions options)
        {
            this.AddSiteUserAsync(sUserXML, options, null);
        }

        public void AddSiteUserAsync(string sUserXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddUserOptions options, object userState)
        {
            if (this.AddSiteUserOperationCompleted == null)
            {
                this.AddSiteUserOperationCompleted = new SendOrPostCallback(this.OnAddSiteUserOperationCompleted);
            }

            object[] objArray = new object[] { sUserXML, options };
            base.InvokeAsync("AddSiteUser", objArray, this.AddSiteUserOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddTerm", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddTerm(string termXml)
        {
            object[] objArray = new object[] { termXml };
            return (string)base.Invoke("AddTerm", objArray)[0];
        }

        public void AddTermAsync(string termXml)
        {
            this.AddTermAsync(termXml, null);
        }

        public void AddTermAsync(string termXml, object userState)
        {
            if (this.AddTermOperationCompleted == null)
            {
                this.AddTermOperationCompleted = new SendOrPostCallback(this.OnAddTermOperationCompleted);
            }

            object[] objArray = new object[] { termXml };
            base.InvokeAsync("AddTerm", objArray, this.AddTermOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddTermGroup", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            object[] objArray = new object[] { targetTermStoreGuid, termGroupXml, includeGroupXmlInResult };
            return (string)base.Invoke("AddTermGroup", objArray)[0];
        }

        public void AddTermGroupAsync(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            this.AddTermGroupAsync(targetTermStoreGuid, termGroupXml, includeGroupXmlInResult, null);
        }

        public void AddTermGroupAsync(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult,
            object userState)
        {
            if (this.AddTermGroupOperationCompleted == null)
            {
                this.AddTermGroupOperationCompleted = new SendOrPostCallback(this.OnAddTermGroupOperationCompleted);
            }

            object[] objArray = new object[] { targetTermStoreGuid, termGroupXml, includeGroupXmlInResult };
            base.InvokeAsync("AddTermGroup", objArray, this.AddTermGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddTermSet", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddTermSet(string termSetXml)
        {
            object[] objArray = new object[] { termSetXml };
            return (string)base.Invoke("AddTermSet", objArray)[0];
        }

        public void AddTermSetAsync(string termSetXml)
        {
            this.AddTermSetAsync(termSetXml, null);
        }

        public void AddTermSetAsync(string termSetXml, object userState)
        {
            if (this.AddTermSetOperationCompleted == null)
            {
                this.AddTermSetOperationCompleted = new SendOrPostCallback(this.OnAddTermSetOperationCompleted);
            }

            object[] objArray = new object[] { termSetXml };
            base.InvokeAsync("AddTermSet", objArray, this.AddTermSetOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddTermstoreLanguages",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            object[] objArray = new object[] { sTargetTermStoreGuid, sLangaugesXML };
            return (string)base.Invoke("AddTermstoreLanguages", objArray)[0];
        }

        public void AddTermstoreLanguagesAsync(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            this.AddTermstoreLanguagesAsync(sTargetTermStoreGuid, sLangaugesXML, null);
        }

        public void AddTermstoreLanguagesAsync(string sTargetTermStoreGuid, string sLangaugesXML, object userState)
        {
            if (this.AddTermstoreLanguagesOperationCompleted == null)
            {
                this.AddTermstoreLanguagesOperationCompleted =
                    new SendOrPostCallback(this.OnAddTermstoreLanguagesOperationCompleted);
            }

            object[] objArray = new object[] { sTargetTermStoreGuid, sLangaugesXML };
            base.InvokeAsync("AddTermstoreLanguages", objArray, this.AddTermstoreLanguagesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddView", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddView(string sListID, string sViewXML)
        {
            object[] objArray = new object[] { sListID, sViewXML };
            return (string)base.Invoke("AddView", objArray)[0];
        }

        public void AddViewAsync(string sListID, string sViewXML)
        {
            this.AddViewAsync(sListID, sViewXML, null);
        }

        public void AddViewAsync(string sListID, string sViewXML, object userState)
        {
            if (this.AddViewOperationCompleted == null)
            {
                this.AddViewOperationCompleted = new SendOrPostCallback(this.OnAddViewOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sViewXML };
            base.InvokeAsync("AddView", objArray, this.AddViewOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddWeb", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddWeb(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddWebOptions addOptions)
        {
            object[] objArray = new object[] { sWebXML, addOptions };
            return (string)base.Invoke("AddWeb", objArray)[0];
        }

        public void AddWebAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddWebOptions addOptions)
        {
            this.AddWebAsync(sWebXML, addOptions, null);
        }

        public void AddWebAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddWebOptions addOptions, object userState)
        {
            if (this.AddWebOperationCompleted == null)
            {
                this.AddWebOperationCompleted = new SendOrPostCallback(this.OnAddWebOperationCompleted);
            }

            object[] objArray = new object[] { sWebXML, addOptions };
            base.InvokeAsync("AddWeb", objArray, this.AddWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddWebParts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            object[] objArray = new object[] { sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent };
            return (string)base.Invoke("AddWebParts", objArray)[0];
        }

        public void AddWebPartsAsync(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            this.AddWebPartsAsync(sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent, null);
        }

        public void AddWebPartsAsync(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent, object userState)
        {
            if (this.AddWebPartsOperationCompleted == null)
            {
                this.AddWebPartsOperationCompleted = new SendOrPostCallback(this.OnAddWebPartsOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent };
            base.InvokeAsync("AddWebParts", objArray, this.AddWebPartsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddWorkflow", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            object[] objArray = new object[] { sListId, sWorkflowXml };
            return (string)base.Invoke("AddWorkflow", objArray)[0];
        }

        [SoapDocumentMethod("http://www.metalogix.net/AddWorkflowAssociation",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            object[] objArray = new object[] { sListId, sWorkflowXml, bAllowDBWriting };
            return (string)base.Invoke("AddWorkflowAssociation", objArray)[0];
        }

        public void AddWorkflowAssociationAsync(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            this.AddWorkflowAssociationAsync(sListId, sWorkflowXml, bAllowDBWriting, null);
        }

        public void AddWorkflowAssociationAsync(string sListId, string sWorkflowXml, bool bAllowDBWriting,
            object userState)
        {
            if (this.AddWorkflowAssociationOperationCompleted == null)
            {
                this.AddWorkflowAssociationOperationCompleted =
                    new SendOrPostCallback(this.OnAddWorkflowAssociationOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sWorkflowXml, bAllowDBWriting };
            base.InvokeAsync("AddWorkflowAssociation", objArray, this.AddWorkflowAssociationOperationCompleted,
                userState);
        }

        public void AddWorkflowAsync(string sListId, string sWorkflowXml)
        {
            this.AddWorkflowAsync(sListId, sWorkflowXml, null);
        }

        public void AddWorkflowAsync(string sListId, string sWorkflowXml, object userState)
        {
            if (this.AddWorkflowOperationCompleted == null)
            {
                this.AddWorkflowOperationCompleted = new SendOrPostCallback(this.OnAddWorkflowOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sWorkflowXml };
            base.InvokeAsync("AddWorkflow", objArray, this.AddWorkflowOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/AnalyzeChurn", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            object[] objArray = new object[] { pivotDate, sListID, iItemID, bRecursive };
            return (string)base.Invoke("AnalyzeChurn", objArray)[0];
        }

        public void AnalyzeChurnAsync(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            this.AnalyzeChurnAsync(pivotDate, sListID, iItemID, bRecursive, null);
        }

        public void AnalyzeChurnAsync(DateTime pivotDate, string sListID, int iItemID, bool bRecursive,
            object userState)
        {
            if (this.AnalyzeChurnOperationCompleted == null)
            {
                this.AnalyzeChurnOperationCompleted = new SendOrPostCallback(this.OnAnalyzeChurnOperationCompleted);
            }

            object[] objArray = new object[] { pivotDate, sListID, iItemID, bRecursive };
            base.InvokeAsync("AnalyzeChurn", objArray, this.AnalyzeChurnOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/Apply2013Theme", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            object[] objArray = new object[] { colorPaletteUrl, spFontUrl, bgImageUrl };
            return (string)base.Invoke("Apply2013Theme", objArray)[0];
        }

        public void Apply2013ThemeAsync(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            this.Apply2013ThemeAsync(colorPaletteUrl, spFontUrl, bgImageUrl, null);
        }

        public void Apply2013ThemeAsync(string colorPaletteUrl, string spFontUrl, string bgImageUrl, object userState)
        {
            if (this.Apply2013ThemeOperationCompleted == null)
            {
                this.Apply2013ThemeOperationCompleted = new SendOrPostCallback(this.OnApply2013ThemeOperationCompleted);
            }

            object[] objArray = new object[] { colorPaletteUrl, spFontUrl, bgImageUrl };
            base.InvokeAsync("Apply2013Theme", objArray, this.Apply2013ThemeOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ApplyOrUpdateContentType",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            object[] objArray = new object[] { sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType };
            return (string)base.Invoke("ApplyOrUpdateContentType", objArray)[0];
        }

        public void ApplyOrUpdateContentTypeAsync(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            this.ApplyOrUpdateContentTypeAsync(sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType, null);
        }

        public void ApplyOrUpdateContentTypeAsync(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType, object userState)
        {
            if (this.ApplyOrUpdateContentTypeOperationCompleted == null)
            {
                this.ApplyOrUpdateContentTypeOperationCompleted =
                    new SendOrPostCallback(this.OnApplyOrUpdateContentTypeOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType };
            base.InvokeAsync("ApplyOrUpdateContentType", objArray, this.ApplyOrUpdateContentTypeOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/BeginCompilingAllAudiences",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string BeginCompilingAllAudiences()
        {
            object[] objArray = base.Invoke("BeginCompilingAllAudiences", new object[0]);
            return (string)objArray[0];
        }

        public void BeginCompilingAllAudiencesAsync()
        {
            this.BeginCompilingAllAudiencesAsync(null);
        }

        public void BeginCompilingAllAudiencesAsync(object userState)
        {
            if (this.BeginCompilingAllAudiencesOperationCompleted == null)
            {
                this.BeginCompilingAllAudiencesOperationCompleted =
                    new SendOrPostCallback(this.OnBeginCompilingAllAudiencesOperationCompleted);
            }

            base.InvokeAsync("BeginCompilingAllAudiences", new object[0],
                this.BeginCompilingAllAudiencesOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/CatalogDocumentToStoragePointFileShareEndpoint",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options)
        {
            object[] objArray = new object[] { sNetworkPath, sListID, sFolder, sListItemXml, options };
            return (string)base.Invoke("CatalogDocumentToStoragePointFileShareEndpoint", objArray)[0];
        }

        public void CatalogDocumentToStoragePointFileShareEndpointAsync(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options)
        {
            this.CatalogDocumentToStoragePointFileShareEndpointAsync(sNetworkPath, sListID, sFolder, sListItemXml,
                options, null);
        }

        public void CatalogDocumentToStoragePointFileShareEndpointAsync(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions options, object userState)
        {
            if (this.CatalogDocumentToStoragePointFileShareEndpointOperationCompleted == null)
            {
                this.CatalogDocumentToStoragePointFileShareEndpointOperationCompleted =
                    new SendOrPostCallback(this.OnCatalogDocumentToStoragePointFileShareEndpointOperationCompleted);
            }

            object[] objArray = new object[] { sNetworkPath, sListID, sFolder, sListItemXml, options };
            base.InvokeAsync("CatalogDocumentToStoragePointFileShareEndpoint", objArray,
                this.CatalogDocumentToStoragePointFileShareEndpointOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/CloseFileCopySession",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CloseFileCopySession(Guid sessionId)
        {
            object[] objArray = new object[] { sessionId };
            return (string)base.Invoke("CloseFileCopySession", objArray)[0];
        }

        public void CloseFileCopySessionAsync(Guid sessionId)
        {
            this.CloseFileCopySessionAsync(sessionId, null);
        }

        public void CloseFileCopySessionAsync(Guid sessionId, object userState)
        {
            if (this.CloseFileCopySessionOperationCompleted == null)
            {
                this.CloseFileCopySessionOperationCompleted =
                    new SendOrPostCallback(this.OnCloseFileCopySessionOperationCompleted);
            }

            object[] objArray = new object[] { sessionId };
            base.InvokeAsync("CloseFileCopySession", objArray, this.CloseFileCopySessionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/CloseWebParts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.Invoke("CloseWebParts", objArray)[0];
        }

        public void CloseWebPartsAsync(string sWebPartPageServerRelativeUrl)
        {
            this.CloseWebPartsAsync(sWebPartPageServerRelativeUrl, null);
        }

        public void CloseWebPartsAsync(string sWebPartPageServerRelativeUrl, object userState)
        {
            if (this.CloseWebPartsOperationCompleted == null)
            {
                this.CloseWebPartsOperationCompleted = new SendOrPostCallback(this.OnCloseWebPartsOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            base.InvokeAsync("CloseWebParts", objArray, this.CloseWebPartsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ConfigureStoragePointFileShareEndpointAndProfile",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            object[] objArray = new object[] { sNetworkPath, sSharePointPath };
            return (string)base.Invoke("ConfigureStoragePointFileShareEndpointAndProfile", objArray)[0];
        }

        public void ConfigureStoragePointFileShareEndpointAndProfileAsync(string sNetworkPath, string sSharePointPath)
        {
            this.ConfigureStoragePointFileShareEndpointAndProfileAsync(sNetworkPath, sSharePointPath, null);
        }

        public void ConfigureStoragePointFileShareEndpointAndProfileAsync(string sNetworkPath, string sSharePointPath,
            object userState)
        {
            if (this.ConfigureStoragePointFileShareEndpointAndProfileOperationCompleted == null)
            {
                this.ConfigureStoragePointFileShareEndpointAndProfileOperationCompleted =
                    new SendOrPostCallback(this.OnConfigureStoragePointFileShareEndpointAndProfileOperationCompleted);
            }

            object[] objArray = new object[] { sNetworkPath, sSharePointPath };
            base.InvokeAsync("ConfigureStoragePointFileShareEndpointAndProfile", objArray,
                this.ConfigureStoragePointFileShareEndpointAndProfileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/CorrectDefaultPageVersions",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            object[] objArray = new object[] { sListID, sFolder, sListItemXML };
            return (string)base.Invoke("CorrectDefaultPageVersions", objArray)[0];
        }

        public void CorrectDefaultPageVersionsAsync(string sListID, string sFolder, string sListItemXML)
        {
            this.CorrectDefaultPageVersionsAsync(sListID, sFolder, sListItemXML, null);
        }

        public void CorrectDefaultPageVersionsAsync(string sListID, string sFolder, string sListItemXML,
            object userState)
        {
            if (this.CorrectDefaultPageVersionsOperationCompleted == null)
            {
                this.CorrectDefaultPageVersionsOperationCompleted =
                    new SendOrPostCallback(this.OnCorrectDefaultPageVersionsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sFolder, sListItemXML };
            base.InvokeAsync("CorrectDefaultPageVersions", objArray, this.CorrectDefaultPageVersionsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteAllAudiences",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteAllAudiences(string inputXml)
        {
            object[] objArray = new object[] { inputXml };
            return (string)base.Invoke("DeleteAllAudiences", objArray)[0];
        }

        public void DeleteAllAudiencesAsync(string inputXml)
        {
            this.DeleteAllAudiencesAsync(inputXml, null);
        }

        public void DeleteAllAudiencesAsync(string inputXml, object userState)
        {
            if (this.DeleteAllAudiencesOperationCompleted == null)
            {
                this.DeleteAllAudiencesOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteAllAudiencesOperationCompleted);
            }

            object[] objArray = new object[] { inputXml };
            base.InvokeAsync("DeleteAllAudiences", objArray, this.DeleteAllAudiencesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteAudience", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteAudience(string sAudienceName)
        {
            object[] objArray = new object[] { sAudienceName };
            return (string)base.Invoke("DeleteAudience", objArray)[0];
        }

        public void DeleteAudienceAsync(string sAudienceName)
        {
            this.DeleteAudienceAsync(sAudienceName, null);
        }

        public void DeleteAudienceAsync(string sAudienceName, object userState)
        {
            if (this.DeleteAudienceOperationCompleted == null)
            {
                this.DeleteAudienceOperationCompleted = new SendOrPostCallback(this.OnDeleteAudienceOperationCompleted);
            }

            object[] objArray = new object[] { sAudienceName };
            base.InvokeAsync("DeleteAudience", objArray, this.DeleteAudienceOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteContentTypes",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            object[] objArray = new object[] { sListID, contentTypeIDs };
            return (string)base.Invoke("DeleteContentTypes", objArray)[0];
        }

        public void DeleteContentTypesAsync(string sListID, string[] contentTypeIDs)
        {
            this.DeleteContentTypesAsync(sListID, contentTypeIDs, null);
        }

        public void DeleteContentTypesAsync(string sListID, string[] contentTypeIDs, object userState)
        {
            if (this.DeleteContentTypesOperationCompleted == null)
            {
                this.DeleteContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteContentTypesOperationCompleted);
            }

            object[] objArray = new object[] { sListID, contentTypeIDs };
            base.InvokeAsync("DeleteContentTypes", objArray, this.DeleteContentTypesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteFolder", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            object[] objArray = new object[] { sListID, iListItemID, sFolder };
            return (string)base.Invoke("DeleteFolder", objArray)[0];
        }

        public void DeleteFolderAsync(string sListID, int iListItemID, string sFolder)
        {
            this.DeleteFolderAsync(sListID, iListItemID, sFolder, null);
        }

        public void DeleteFolderAsync(string sListID, int iListItemID, string sFolder, object userState)
        {
            if (this.DeleteFolderOperationCompleted == null)
            {
                this.DeleteFolderOperationCompleted = new SendOrPostCallback(this.OnDeleteFolderOperationCompleted);
            }

            object[] objArray = new object[] { sListID, iListItemID, sFolder };
            base.InvokeAsync("DeleteFolder", objArray, this.DeleteFolderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteGroup", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteGroup(string sGroupName)
        {
            object[] objArray = new object[] { sGroupName };
            return (string)base.Invoke("DeleteGroup", objArray)[0];
        }

        public void DeleteGroupAsync(string sGroupName)
        {
            this.DeleteGroupAsync(sGroupName, null);
        }

        public void DeleteGroupAsync(string sGroupName, object userState)
        {
            if (this.DeleteGroupOperationCompleted == null)
            {
                this.DeleteGroupOperationCompleted = new SendOrPostCallback(this.OnDeleteGroupOperationCompleted);
            }

            object[] objArray = new object[] { sGroupName };
            base.InvokeAsync("DeleteGroup", objArray, this.DeleteGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteItem", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteItem(string sListID, int iListItemID)
        {
            object[] objArray = new object[] { sListID, iListItemID };
            return (string)base.Invoke("DeleteItem", objArray)[0];
        }

        public void DeleteItemAsync(string sListID, int iListItemID)
        {
            this.DeleteItemAsync(sListID, iListItemID, null);
        }

        public void DeleteItemAsync(string sListID, int iListItemID, object userState)
        {
            if (this.DeleteItemOperationCompleted == null)
            {
                this.DeleteItemOperationCompleted = new SendOrPostCallback(this.OnDeleteItemOperationCompleted);
            }

            object[] objArray = new object[] { sListID, iListItemID };
            base.InvokeAsync("DeleteItem", objArray, this.DeleteItemOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteItems", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            object[] objArray = new object[] { sListID, bDeleteAllItems, sIDs };
            return (string)base.Invoke("DeleteItems", objArray)[0];
        }

        public void DeleteItemsAsync(string sListID, bool bDeleteAllItems, string sIDs)
        {
            this.DeleteItemsAsync(sListID, bDeleteAllItems, sIDs, null);
        }

        public void DeleteItemsAsync(string sListID, bool bDeleteAllItems, string sIDs, object userState)
        {
            if (this.DeleteItemsOperationCompleted == null)
            {
                this.DeleteItemsOperationCompleted = new SendOrPostCallback(this.OnDeleteItemsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, bDeleteAllItems, sIDs };
            base.InvokeAsync("DeleteItems", objArray, this.DeleteItemsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteList", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteList(string sListID)
        {
            object[] objArray = new object[] { sListID };
            return (string)base.Invoke("DeleteList", objArray)[0];
        }

        public void DeleteListAsync(string sListID)
        {
            this.DeleteListAsync(sListID, null);
        }

        public void DeleteListAsync(string sListID, object userState)
        {
            if (this.DeleteListOperationCompleted == null)
            {
                this.DeleteListOperationCompleted = new SendOrPostCallback(this.OnDeleteListOperationCompleted);
            }

            object[] objArray = new object[] { sListID };
            base.InvokeAsync("DeleteList", objArray, this.DeleteListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteRole", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteRole(string sRoleName)
        {
            object[] objArray = new object[] { sRoleName };
            return (string)base.Invoke("DeleteRole", objArray)[0];
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteRoleAssignment",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)base.Invoke("DeleteRoleAssignment", objArray)[0];
        }

        public void DeleteRoleAssignmentAsync(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            this.DeleteRoleAssignmentAsync(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId, null);
        }

        public void DeleteRoleAssignmentAsync(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId, object userState)
        {
            if (this.DeleteRoleAssignmentOperationCompleted == null)
            {
                this.DeleteRoleAssignmentOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteRoleAssignmentOperationCompleted);
            }

            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            base.InvokeAsync("DeleteRoleAssignment", objArray, this.DeleteRoleAssignmentOperationCompleted, userState);
        }

        public void DeleteRoleAsync(string sRoleName)
        {
            this.DeleteRoleAsync(sRoleName, null);
        }

        public void DeleteRoleAsync(string sRoleName, object userState)
        {
            if (this.DeleteRoleOperationCompleted == null)
            {
                this.DeleteRoleOperationCompleted = new SendOrPostCallback(this.OnDeleteRoleOperationCompleted);
            }

            object[] objArray = new object[] { sRoleName };
            base.InvokeAsync("DeleteRole", objArray, this.DeleteRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteSiteCollection",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            object[] objArray = new object[] { sSiteURL, sWebApp };
            return (string)base.Invoke("DeleteSiteCollection", objArray)[0];
        }

        public void DeleteSiteCollectionAsync(string sSiteURL, string sWebApp)
        {
            this.DeleteSiteCollectionAsync(sSiteURL, sWebApp, null);
        }

        public void DeleteSiteCollectionAsync(string sSiteURL, string sWebApp, object userState)
        {
            if (this.DeleteSiteCollectionOperationCompleted == null)
            {
                this.DeleteSiteCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteSiteCollectionOperationCompleted);
            }

            object[] objArray = new object[] { sSiteURL, sWebApp };
            base.InvokeAsync("DeleteSiteCollection", objArray, this.DeleteSiteCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteSP2013Workflows",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteSP2013Workflows(string configurationXml)
        {
            object[] objArray = new object[] { configurationXml };
            return (string)base.Invoke("DeleteSP2013Workflows", objArray)[0];
        }

        public void DeleteSP2013WorkflowsAsync(string configurationXml)
        {
            this.DeleteSP2013WorkflowsAsync(configurationXml, null);
        }

        public void DeleteSP2013WorkflowsAsync(string configurationXml, object userState)
        {
            if (this.DeleteSP2013WorkflowsOperationCompleted == null)
            {
                this.DeleteSP2013WorkflowsOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteSP2013WorkflowsOperationCompleted);
            }

            object[] objArray = new object[] { configurationXml };
            base.InvokeAsync("DeleteSP2013Workflows", objArray, this.DeleteSP2013WorkflowsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteWeb", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteWeb(string sServerRelativeUrl)
        {
            object[] objArray = new object[] { sServerRelativeUrl };
            return (string)base.Invoke("DeleteWeb", objArray)[0];
        }

        public void DeleteWebAsync(string sServerRelativeUrl)
        {
            this.DeleteWebAsync(sServerRelativeUrl, null);
        }

        public void DeleteWebAsync(string sServerRelativeUrl, object userState)
        {
            if (this.DeleteWebOperationCompleted == null)
            {
                this.DeleteWebOperationCompleted = new SendOrPostCallback(this.OnDeleteWebOperationCompleted);
            }

            object[] objArray = new object[] { sServerRelativeUrl };
            base.InvokeAsync("DeleteWeb", objArray, this.DeleteWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteWebPart", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl, sWebPartId };
            return (string)base.Invoke("DeleteWebPart", objArray)[0];
        }

        public void DeleteWebPartAsync(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            this.DeleteWebPartAsync(sWebPartPageServerRelativeUrl, sWebPartId, null);
        }

        public void DeleteWebPartAsync(string sWebPartPageServerRelativeUrl, string sWebPartId, object userState)
        {
            if (this.DeleteWebPartOperationCompleted == null)
            {
                this.DeleteWebPartOperationCompleted = new SendOrPostCallback(this.OnDeleteWebPartOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl, sWebPartId };
            base.InvokeAsync("DeleteWebPart", objArray, this.DeleteWebPartOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/DeleteWebParts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.Invoke("DeleteWebParts", objArray)[0];
        }

        public void DeleteWebPartsAsync(string sWebPartPageServerRelativeUrl)
        {
            this.DeleteWebPartsAsync(sWebPartPageServerRelativeUrl, null);
        }

        public void DeleteWebPartsAsync(string sWebPartPageServerRelativeUrl, object userState)
        {
            if (this.DeleteWebPartsOperationCompleted == null)
            {
                this.DeleteWebPartsOperationCompleted = new SendOrPostCallback(this.OnDeleteWebPartsOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            base.InvokeAsync("DeleteWebParts", objArray, this.DeleteWebPartsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ExecuteCommand", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            object[] objArray = new object[] { commandName, commandConfigurationXml };
            return (string)base.Invoke("ExecuteCommand", objArray)[0];
        }

        public void ExecuteCommandAsync(string commandName, string commandConfigurationXml)
        {
            this.ExecuteCommandAsync(commandName, commandConfigurationXml, null);
        }

        public void ExecuteCommandAsync(string commandName, string commandConfigurationXml, object userState)
        {
            if (this.ExecuteCommandOperationCompleted == null)
            {
                this.ExecuteCommandOperationCompleted = new SendOrPostCallback(this.OnExecuteCommandOperationCompleted);
            }

            object[] objArray = new object[] { commandName, commandConfigurationXml };
            base.InvokeAsync("ExecuteCommand", objArray, this.ExecuteCommandOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/FindAlerts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string FindAlerts()
        {
            object[] objArray = base.Invoke("FindAlerts", new object[0]);
            return (string)objArray[0];
        }

        public void FindAlertsAsync()
        {
            this.FindAlertsAsync(null);
        }

        public void FindAlertsAsync(object userState)
        {
            if (this.FindAlertsOperationCompleted == null)
            {
                this.FindAlertsOperationCompleted = new SendOrPostCallback(this.OnFindAlertsOperationCompleted);
            }

            base.InvokeAsync("FindAlerts", new object[0], this.FindAlertsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/FindUniquePermissions",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string FindUniquePermissions()
        {
            object[] objArray = base.Invoke("FindUniquePermissions", new object[0]);
            return (string)objArray[0];
        }

        public void FindUniquePermissionsAsync()
        {
            this.FindUniquePermissionsAsync(null);
        }

        public void FindUniquePermissionsAsync(object userState)
        {
            if (this.FindUniquePermissionsOperationCompleted == null)
            {
                this.FindUniquePermissionsOperationCompleted =
                    new SendOrPostCallback(this.OnFindUniquePermissionsOperationCompleted);
            }

            base.InvokeAsync("FindUniquePermissions", new object[0], this.FindUniquePermissionsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetAddIns", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAddIns(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetAddIns", objArray)[0];
        }

        public void GetAddInsAsync(string options)
        {
            this.GetAddInsAsync(options, null);
        }

        public void GetAddInsAsync(string options, object userState)
        {
            if (this.GetAddInsOperationCompleted == null)
            {
                this.GetAddInsOperationCompleted = new SendOrPostCallback(this.OnGetAddInsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetAddIns", objArray, this.GetAddInsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetAdImportDcMappings",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAdImportDcMappings(string profileDbConnectionString, string connName, string connType,
            string options)
        {
            object[] objArray = new object[] { profileDbConnectionString, connName, connType, options };
            return (string)base.Invoke("GetAdImportDcMappings", objArray)[0];
        }

        public void GetAdImportDcMappingsAsync(string profileDbConnectionString, string connName, string connType,
            string options)
        {
            this.GetAdImportDcMappingsAsync(profileDbConnectionString, connName, connType, options, null);
        }

        public void GetAdImportDcMappingsAsync(string profileDbConnectionString, string connName, string connType,
            string options, object userState)
        {
            if (this.GetAdImportDcMappingsOperationCompleted == null)
            {
                this.GetAdImportDcMappingsOperationCompleted =
                    new SendOrPostCallback(this.OnGetAdImportDcMappingsOperationCompleted);
            }

            object[] objArray = new object[] { profileDbConnectionString, connName, connType, options };
            base.InvokeAsync("GetAdImportDcMappings", objArray, this.GetAdImportDcMappingsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetAlerts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAlerts(string sListID, int iItemID)
        {
            object[] objArray = new object[] { sListID, iItemID };
            return (string)base.Invoke("GetAlerts", objArray)[0];
        }

        public void GetAlertsAsync(string sListID, int iItemID)
        {
            this.GetAlertsAsync(sListID, iItemID, null);
        }

        public void GetAlertsAsync(string sListID, int iItemID, object userState)
        {
            if (this.GetAlertsOperationCompleted == null)
            {
                this.GetAlertsOperationCompleted = new SendOrPostCallback(this.OnGetAlertsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, iItemID };
            base.InvokeAsync("GetAlerts", objArray, this.GetAlertsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetAttachments", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAttachments(string sListID, int iItemID)
        {
            object[] objArray = new object[] { sListID, iItemID };
            return (string)base.Invoke("GetAttachments", objArray)[0];
        }

        public void GetAttachmentsAsync(string sListID, int iItemID)
        {
            this.GetAttachmentsAsync(sListID, iItemID, null);
        }

        public void GetAttachmentsAsync(string sListID, int iItemID, object userState)
        {
            if (this.GetAttachmentsOperationCompleted == null)
            {
                this.GetAttachmentsOperationCompleted = new SendOrPostCallback(this.OnGetAttachmentsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, iItemID };
            base.InvokeAsync("GetAttachments", objArray, this.GetAttachmentsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetAudiences", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetAudiences()
        {
            object[] objArray = base.Invoke("GetAudiences", new object[0]);
            return (string)objArray[0];
        }

        public void GetAudiencesAsync()
        {
            this.GetAudiencesAsync(null);
        }

        public void GetAudiencesAsync(object userState)
        {
            if (this.GetAudiencesOperationCompleted == null)
            {
                this.GetAudiencesOperationCompleted = new SendOrPostCallback(this.OnGetAudiencesOperationCompleted);
            }

            base.InvokeAsync("GetAudiences", new object[0], this.GetAudiencesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetBcsApplications",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetBcsApplications(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetBcsApplications", objArray)[0];
        }

        public void GetBcsApplicationsAsync(string options)
        {
            this.GetBcsApplicationsAsync(options, null);
        }

        public void GetBcsApplicationsAsync(string options, object userState)
        {
            if (this.GetBcsApplicationsOperationCompleted == null)
            {
                this.GetBcsApplicationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetBcsApplicationsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetBcsApplications", objArray, this.GetBcsApplicationsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetBrowserFileHandling",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetBrowserFileHandling(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetBrowserFileHandling", objArray)[0];
        }

        public void GetBrowserFileHandlingAsync(string options)
        {
            this.GetBrowserFileHandlingAsync(options, null);
        }

        public void GetBrowserFileHandlingAsync(string options, object userState)
        {
            if (this.GetBrowserFileHandlingOperationCompleted == null)
            {
                this.GetBrowserFileHandlingOperationCompleted =
                    new SendOrPostCallback(this.OnGetBrowserFileHandlingOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetBrowserFileHandling", objArray, this.GetBrowserFileHandlingOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetContentTypes", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetContentTypes(string sListId)
        {
            object[] objArray = new object[] { sListId };
            return (string)base.Invoke("GetContentTypes", objArray)[0];
        }

        public void GetContentTypesAsync(string sListId)
        {
            this.GetContentTypesAsync(sListId, null);
        }

        public void GetContentTypesAsync(string sListId, object userState)
        {
            if (this.GetContentTypesOperationCompleted == null)
            {
                this.GetContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnGetContentTypesOperationCompleted);
            }

            object[] objArray = new object[] { sListId };
            base.InvokeAsync("GetContentTypes", objArray, this.GetContentTypesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetCustomProfilePropertyMapping",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetCustomProfilePropertyMapping(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetCustomProfilePropertyMapping", objArray)[0];
        }

        public void GetCustomProfilePropertyMappingAsync(string options)
        {
            this.GetCustomProfilePropertyMappingAsync(options, null);
        }

        public void GetCustomProfilePropertyMappingAsync(string options, object userState)
        {
            if (this.GetCustomProfilePropertyMappingOperationCompleted == null)
            {
                this.GetCustomProfilePropertyMappingOperationCompleted =
                    new SendOrPostCallback(this.OnGetCustomProfilePropertyMappingOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetCustomProfilePropertyMapping", objArray,
                this.GetCustomProfilePropertyMappingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDashboardPageTemplate",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            object[] objArray = new object[] { iTemplateId };
            return (byte[])base.Invoke("GetDashboardPageTemplate", objArray)[0];
        }

        public void GetDashboardPageTemplateAsync(int iTemplateId)
        {
            this.GetDashboardPageTemplateAsync(iTemplateId, null);
        }

        public void GetDashboardPageTemplateAsync(int iTemplateId, object userState)
        {
            if (this.GetDashboardPageTemplateOperationCompleted == null)
            {
                this.GetDashboardPageTemplateOperationCompleted =
                    new SendOrPostCallback(this.OnGetDashboardPageTemplateOperationCompleted);
            }

            object[] objArray = new object[] { iTemplateId };
            base.InvokeAsync("GetDashboardPageTemplate", objArray, this.GetDashboardPageTemplateOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocument", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])base.Invoke("GetDocument", objArray)[0];
        }

        public void GetDocumentAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            this.GetDocumentAsync(sDocID, sFileDirRef, sFileLeafRef, iLevel, null);
        }

        public void GetDocumentAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel,
            object userState)
        {
            if (this.GetDocumentOperationCompleted == null)
            {
                this.GetDocumentOperationCompleted = new SendOrPostCallback(this.OnGetDocumentOperationCompleted);
            }

            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            base.InvokeAsync("GetDocument", objArray, this.GetDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentBlobRef",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])base.Invoke("GetDocumentBlobRef", objArray)[0];
        }

        public void GetDocumentBlobRefAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            this.GetDocumentBlobRefAsync(sDocID, sFileDirRef, sFileLeafRef, iLevel, null);
        }

        public void GetDocumentBlobRefAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel,
            object userState)
        {
            if (this.GetDocumentBlobRefOperationCompleted == null)
            {
                this.GetDocumentBlobRefOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentBlobRefOperationCompleted);
            }

            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            base.InvokeAsync("GetDocumentBlobRef", objArray, this.GetDocumentBlobRefOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocumentChunked(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel, streamType, iRetentionTime };
            return (byte[])base.Invoke("GetDocumentChunked", objArray)[0];
        }

        public void GetDocumentChunkedAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            this.GetDocumentChunkedAsync(sDocID, sFileDirRef, sFileLeafRef, iLevel, streamType, iRetentionTime, null);
        }

        public void GetDocumentChunkedAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime,
            object userState)
        {
            if (this.GetDocumentChunkedOperationCompleted == null)
            {
                this.GetDocumentChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentChunkedOperationCompleted);
            }

            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel, streamType, iRetentionTime };
            base.InvokeAsync("GetDocumentChunked", objArray, this.GetDocumentChunkedOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentId", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetDocumentId(string sDocUrl)
        {
            object[] objArray = new object[] { sDocUrl };
            return (string)base.Invoke("GetDocumentId", objArray)[0];
        }

        public void GetDocumentIdAsync(string sDocUrl)
        {
            this.GetDocumentIdAsync(sDocUrl, null);
        }

        public void GetDocumentIdAsync(string sDocUrl, object userState)
        {
            if (this.GetDocumentIdOperationCompleted == null)
            {
                this.GetDocumentIdOperationCompleted = new SendOrPostCallback(this.OnGetDocumentIdOperationCompleted);
            }

            object[] objArray = new object[] { sDocUrl };
            base.InvokeAsync("GetDocumentId", objArray, this.GetDocumentIdOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentVersion",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])base.Invoke("GetDocumentVersion", objArray)[0];
        }

        public void GetDocumentVersionAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            this.GetDocumentVersionAsync(sDocID, sFileDirRef, sFileLeafRef, iVersion, null);
        }

        public void GetDocumentVersionAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion,
            object userState)
        {
            if (this.GetDocumentVersionOperationCompleted == null)
            {
                this.GetDocumentVersionOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentVersionOperationCompleted);
            }

            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            base.InvokeAsync("GetDocumentVersion", objArray, this.GetDocumentVersionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentVersionBlobRef",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])base.Invoke("GetDocumentVersionBlobRef", objArray)[0];
        }

        public void GetDocumentVersionBlobRefAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            this.GetDocumentVersionBlobRefAsync(sDocID, sFileDirRef, sFileLeafRef, iVersion, null);
        }

        public void GetDocumentVersionBlobRefAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion,
            object userState)
        {
            if (this.GetDocumentVersionBlobRefOperationCompleted == null)
            {
                this.GetDocumentVersionBlobRefOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentVersionBlobRefOperationCompleted);
            }

            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            base.InvokeAsync("GetDocumentVersionBlobRef", objArray, this.GetDocumentVersionBlobRefOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetDocumentVersionChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetDocumentVersionChunked(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            object[] objArray = new object[]
                { sDocID, sFileDirRef, sFileLeafRef, iVersion, streamType, iRetentionTime };
            return (byte[])base.Invoke("GetDocumentVersionChunked", objArray)[0];
        }

        public void GetDocumentVersionChunkedAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            this.GetDocumentVersionChunkedAsync(sDocID, sFileDirRef, sFileLeafRef, iVersion, streamType, iRetentionTime,
                null);
        }

        public void GetDocumentVersionChunkedAsync(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime,
            object userState)
        {
            if (this.GetDocumentVersionChunkedOperationCompleted == null)
            {
                this.GetDocumentVersionChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnGetDocumentVersionChunkedOperationCompleted);
            }

            object[] objArray = new object[]
                { sDocID, sFileDirRef, sFileLeafRef, iVersion, streamType, iRetentionTime };
            base.InvokeAsync("GetDocumentVersionChunked", objArray, this.GetDocumentVersionChunkedOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetExternalContentTypeOperations",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            object[] objArray = new object[] { sExternalContentTypeNamespace, sExternalContentTypeName };
            return (string)base.Invoke("GetExternalContentTypeOperations", objArray)[0];
        }

        public void GetExternalContentTypeOperationsAsync(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            this.GetExternalContentTypeOperationsAsync(sExternalContentTypeNamespace, sExternalContentTypeName, null);
        }

        public void GetExternalContentTypeOperationsAsync(string sExternalContentTypeNamespace,
            string sExternalContentTypeName, object userState)
        {
            if (this.GetExternalContentTypeOperationsOperationCompleted == null)
            {
                this.GetExternalContentTypeOperationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetExternalContentTypeOperationsOperationCompleted);
            }

            object[] objArray = new object[] { sExternalContentTypeNamespace, sExternalContentTypeName };
            base.InvokeAsync("GetExternalContentTypeOperations", objArray,
                this.GetExternalContentTypeOperationsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetExternalContentTypes",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetExternalContentTypes()
        {
            object[] objArray = base.Invoke("GetExternalContentTypes", new object[0]);
            return (string)objArray[0];
        }

        public void GetExternalContentTypesAsync()
        {
            this.GetExternalContentTypesAsync(null);
        }

        public void GetExternalContentTypesAsync(object userState)
        {
            if (this.GetExternalContentTypesOperationCompleted == null)
            {
                this.GetExternalContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnGetExternalContentTypesOperationCompleted);
            }

            base.InvokeAsync("GetExternalContentTypes", new object[0], this.GetExternalContentTypesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetExternalItems", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sExternalContentTypeOperationName, string sListID)
        {
            object[] objArray = new object[]
                { sExternalContentTypeNamespace, sExternalContentTypeName, sExternalContentTypeOperationName, sListID };
            return (string)base.Invoke("GetExternalItems", objArray)[0];
        }

        public void GetExternalItemsAsync(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sExternalContentTypeOperationName, string sListID)
        {
            this.GetExternalItemsAsync(sExternalContentTypeNamespace, sExternalContentTypeName,
                sExternalContentTypeOperationName, sListID, null);
        }

        public void GetExternalItemsAsync(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sExternalContentTypeOperationName, string sListID, object userState)
        {
            if (this.GetExternalItemsOperationCompleted == null)
            {
                this.GetExternalItemsOperationCompleted =
                    new SendOrPostCallback(this.OnGetExternalItemsOperationCompleted);
            }

            object[] objArray = new object[]
                { sExternalContentTypeNamespace, sExternalContentTypeName, sExternalContentTypeOperationName, sListID };
            base.InvokeAsync("GetExternalItems", objArray, this.GetExternalItemsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFarmSandboxSolutions",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFarmSandboxSolutions(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetFarmSandboxSolutions", objArray)[0];
        }

        public void GetFarmSandboxSolutionsAsync(string options)
        {
            this.GetFarmSandboxSolutionsAsync(options, null);
        }

        public void GetFarmSandboxSolutionsAsync(string options, object userState)
        {
            if (this.GetFarmSandboxSolutionsOperationCompleted == null)
            {
                this.GetFarmSandboxSolutionsOperationCompleted =
                    new SendOrPostCallback(this.OnGetFarmSandboxSolutionsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetFarmSandboxSolutions", objArray, this.GetFarmSandboxSolutionsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFarmServerDetails",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFarmServerDetails(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetFarmServerDetails", objArray)[0];
        }

        public void GetFarmServerDetailsAsync(string options)
        {
            this.GetFarmServerDetailsAsync(options, null);
        }

        public void GetFarmServerDetailsAsync(string options, object userState)
        {
            if (this.GetFarmServerDetailsOperationCompleted == null)
            {
                this.GetFarmServerDetailsOperationCompleted =
                    new SendOrPostCallback(this.OnGetFarmServerDetailsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetFarmServerDetails", objArray, this.GetFarmServerDetailsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFarmSolutionBinary",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetFarmSolutionBinary(string solutionName)
        {
            object[] objArray = new object[] { solutionName };
            return (byte[])base.Invoke("GetFarmSolutionBinary", objArray)[0];
        }

        public void GetFarmSolutionBinaryAsync(string solutionName)
        {
            this.GetFarmSolutionBinaryAsync(solutionName, null);
        }

        public void GetFarmSolutionBinaryAsync(string solutionName, object userState)
        {
            if (this.GetFarmSolutionBinaryOperationCompleted == null)
            {
                this.GetFarmSolutionBinaryOperationCompleted =
                    new SendOrPostCallback(this.OnGetFarmSolutionBinaryOperationCompleted);
            }

            object[] objArray = new object[] { solutionName };
            base.InvokeAsync("GetFarmSolutionBinary", objArray, this.GetFarmSolutionBinaryOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFarmSolutions", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFarmSolutions(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetFarmSolutions", objArray)[0];
        }

        public void GetFarmSolutionsAsync(string options)
        {
            this.GetFarmSolutionsAsync(options, null);
        }

        public void GetFarmSolutionsAsync(string options, object userState)
        {
            if (this.GetFarmSolutionsOperationCompleted == null)
            {
                this.GetFarmSolutionsOperationCompleted =
                    new SendOrPostCallback(this.OnGetFarmSolutionsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetFarmSolutions", objArray, this.GetFarmSolutionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFields", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFields(string sListID, bool bGetAllAvailableFields)
        {
            object[] objArray = new object[] { sListID, bGetAllAvailableFields };
            return (string)base.Invoke("GetFields", objArray)[0];
        }

        public void GetFieldsAsync(string sListID, bool bGetAllAvailableFields)
        {
            this.GetFieldsAsync(sListID, bGetAllAvailableFields, null);
        }

        public void GetFieldsAsync(string sListID, bool bGetAllAvailableFields, object userState)
        {
            if (this.GetFieldsOperationCompleted == null)
            {
                this.GetFieldsOperationCompleted = new SendOrPostCallback(this.OnGetFieldsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, bGetAllAvailableFields };
            base.InvokeAsync("GetFields", objArray, this.GetFieldsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFiles", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFiles(string sFolderPath,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes)
        {
            object[] objArray = new object[] { sFolderPath, itemTypes };
            return (string)base.Invoke("GetFiles", objArray)[0];
        }

        public void GetFilesAsync(string sFolderPath,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes)
        {
            this.GetFilesAsync(sFolderPath, itemTypes, null);
        }

        public void GetFilesAsync(string sFolderPath,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes,
            object userState)
        {
            if (this.GetFilesOperationCompleted == null)
            {
                this.GetFilesOperationCompleted = new SendOrPostCallback(this.OnGetFilesOperationCompleted);
            }

            object[] objArray = new object[] { sFolderPath, itemTypes };
            base.InvokeAsync("GetFiles", objArray, this.GetFilesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFileVersions", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFileVersions(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetFileVersions", objArray)[0];
        }

        public void GetFileVersionsAsync(string options)
        {
            this.GetFileVersionsAsync(options, null);
        }

        public void GetFileVersionsAsync(string options, object userState)
        {
            if (this.GetFileVersionsOperationCompleted == null)
            {
                this.GetFileVersionsOperationCompleted =
                    new SendOrPostCallback(this.OnGetFileVersionsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetFileVersions", objArray, this.GetFileVersionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetFolders", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            object[] objArray = new object[] { sListID, sIDs, sParentFolder };
            return (string)base.Invoke("GetFolders", objArray)[0];
        }

        public void GetFoldersAsync(string sListID, string sIDs, string sParentFolder)
        {
            this.GetFoldersAsync(sListID, sIDs, sParentFolder, null);
        }

        public void GetFoldersAsync(string sListID, string sIDs, string sParentFolder, object userState)
        {
            if (this.GetFoldersOperationCompleted == null)
            {
                this.GetFoldersOperationCompleted = new SendOrPostCallback(this.OnGetFoldersOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sIDs, sParentFolder };
            base.InvokeAsync("GetFolders", objArray, this.GetFoldersOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetGroups", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetGroups()
        {
            object[] objArray = base.Invoke("GetGroups", new object[0]);
            return (string)objArray[0];
        }

        public void GetGroupsAsync()
        {
            this.GetGroupsAsync(null);
        }

        public void GetGroupsAsync(object userState)
        {
            if (this.GetGroupsOperationCompleted == null)
            {
                this.GetGroupsOperationCompleted = new SendOrPostCallback(this.OnGetGroupsOperationCompleted);
            }

            base.InvokeAsync("GetGroups", new object[0], this.GetGroupsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetInfopaths", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetInfopaths(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetInfopaths", objArray)[0];
        }

        public void GetInfopathsAsync(string options)
        {
            this.GetInfopathsAsync(options, null);
        }

        public void GetInfopathsAsync(string options, object userState)
        {
            if (this.GetInfopathsOperationCompleted == null)
            {
                this.GetInfopathsOperationCompleted = new SendOrPostCallback(this.OnGetInfopathsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetInfopaths", objArray, this.GetInfopathsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetLanguagesAndWebTemplates",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetLanguagesAndWebTemplates()
        {
            object[] objArray = base.Invoke("GetLanguagesAndWebTemplates", new object[0]);
            return (string)objArray[0];
        }

        public void GetLanguagesAndWebTemplatesAsync()
        {
            this.GetLanguagesAndWebTemplatesAsync(null);
        }

        public void GetLanguagesAndWebTemplatesAsync(object userState)
        {
            if (this.GetLanguagesAndWebTemplatesOperationCompleted == null)
            {
                this.GetLanguagesAndWebTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnGetLanguagesAndWebTemplatesOperationCompleted);
            }

            base.InvokeAsync("GetLanguagesAndWebTemplates", new object[0],
                this.GetLanguagesAndWebTemplatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetList", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetList(string sListID)
        {
            object[] objArray = new object[] { sListID };
            return (string)base.Invoke("GetList", objArray)[0];
        }

        public void GetListAsync(string sListID)
        {
            this.GetListAsync(sListID, null);
        }

        public void GetListAsync(string sListID, object userState)
        {
            if (this.GetListOperationCompleted == null)
            {
                this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);
            }

            object[] objArray = new object[] { sListID };
            base.InvokeAsync("GetList", objArray, this.GetListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetListItemIDs", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes)
        {
            object[] objArray = new object[] { sListID, sParentFolder, bRecursive, itemTypes };
            return (string)base.Invoke("GetListItemIDs", objArray)[0];
        }

        public void GetListItemIDsAsync(string sListID, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes)
        {
            this.GetListItemIDsAsync(sListID, sParentFolder, bRecursive, itemTypes, null);
        }

        public void GetListItemIDsAsync(string sListID, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes,
            object userState)
        {
            if (this.GetListItemIDsOperationCompleted == null)
            {
                this.GetListItemIDsOperationCompleted = new SendOrPostCallback(this.OnGetListItemIDsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sParentFolder, bRecursive, itemTypes };
            base.InvokeAsync("GetListItemIDs", objArray, this.GetListItemIDsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetListItems", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes,
            string sListSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions)
        {
            object[] objArray = new object[]
                { sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings, getOptions };
            return (string)base.Invoke("GetListItems", objArray)[0];
        }

        public void GetListItemsAsync(string sListID, string sIDs, string sFields, string sParentFolder,
            bool bRecursive, Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes,
            string sListSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions)
        {
            this.GetListItemsAsync(sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings,
                getOptions, null);
        }

        public void GetListItemsAsync(string sListID, string sIDs, string sFields, string sParentFolder,
            bool bRecursive, Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ListItemQueryType itemTypes,
            string sListSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions,
            object userState)
        {
            if (this.GetListItemsOperationCompleted == null)
            {
                this.GetListItemsOperationCompleted = new SendOrPostCallback(this.OnGetListItemsOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings, getOptions };
            base.InvokeAsync("GetListItems", objArray, this.GetListItemsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetListItemsByQuery",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions)
        {
            object[] objArray = new object[] { listID, fields, query, listSettings, getOptions };
            return (string)base.Invoke("GetListItemsByQuery", objArray)[0];
        }

        public void GetListItemsByQueryAsync(string listID, string fields, string query, string listSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions)
        {
            this.GetListItemsByQueryAsync(listID, fields, query, listSettings, getOptions, null);
        }

        public void GetListItemsByQueryAsync(string listID, string fields, string query, string listSettings,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.GetListItemOptions getOptions,
            object userState)
        {
            if (this.GetListItemsByQueryOperationCompleted == null)
            {
                this.GetListItemsByQueryOperationCompleted =
                    new SendOrPostCallback(this.OnGetListItemsByQueryOperationCompleted);
            }

            object[] objArray = new object[] { listID, fields, query, listSettings, getOptions };
            base.InvokeAsync("GetListItemsByQuery", objArray, this.GetListItemsByQueryOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetListItemVersions",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
        {
            object[] objArray = new object[] { sListID, iItemID, sFields, sListSettings };
            return (string)base.Invoke("GetListItemVersions", objArray)[0];
        }

        public void GetListItemVersionsAsync(string sListID, int iItemID, string sFields, string sListSettings)
        {
            this.GetListItemVersionsAsync(sListID, iItemID, sFields, sListSettings, null);
        }

        public void GetListItemVersionsAsync(string sListID, int iItemID, string sFields, string sListSettings,
            object userState)
        {
            if (this.GetListItemVersionsOperationCompleted == null)
            {
                this.GetListItemVersionsOperationCompleted =
                    new SendOrPostCallback(this.OnGetListItemVersionsOperationCompleted);
            }

            object[] objArray = new object[] { sListID, iItemID, sFields, sListSettings };
            base.InvokeAsync("GetListItemVersions", objArray, this.GetListItemVersionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetLists", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetLists()
        {
            object[] objArray = base.Invoke("GetLists", new object[0]);
            return (string)objArray[0];
        }

        public void GetListsAsync()
        {
            this.GetListsAsync(null);
        }

        public void GetListsAsync(object userState)
        {
            if (this.GetListsOperationCompleted == null)
            {
                this.GetListsOperationCompleted = new SendOrPostCallback(this.OnGetListsOperationCompleted);
            }

            base.InvokeAsync("GetLists", new object[0], this.GetListsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetListTemplates", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListTemplates()
        {
            object[] objArray = base.Invoke("GetListTemplates", new object[0]);
            return (string)objArray[0];
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

        [SoapDocumentMethod("http://www.metalogix.net/GetListWorkflowRunning2010",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListWorkflowRunning2010(string listName)
        {
            object[] objArray = new object[] { listName };
            return (string)base.Invoke("GetListWorkflowRunning2010", objArray)[0];
        }

        public void GetListWorkflowRunning2010Async(string listName)
        {
            this.GetListWorkflowRunning2010Async(listName, null);
        }

        public void GetListWorkflowRunning2010Async(string listName, object userState)
        {
            if (this.GetListWorkflowRunning2010OperationCompleted == null)
            {
                this.GetListWorkflowRunning2010OperationCompleted =
                    new SendOrPostCallback(this.OnGetListWorkflowRunning2010OperationCompleted);
            }

            object[] objArray = new object[] { listName };
            base.InvokeAsync("GetListWorkflowRunning2010", objArray, this.GetListWorkflowRunning2010OperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetLockedSites", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetLockedSites(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetLockedSites", objArray)[0];
        }

        public void GetLockedSitesAsync(string options)
        {
            this.GetLockedSitesAsync(options, null);
        }

        public void GetLockedSitesAsync(string options, object userState)
        {
            if (this.GetLockedSitesOperationCompleted == null)
            {
                this.GetLockedSitesOperationCompleted = new SendOrPostCallback(this.OnGetLockedSitesOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetLockedSites", objArray, this.GetLockedSitesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetMySiteData", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetMySiteData(string sSiteURL)
        {
            object[] objArray = new object[] { sSiteURL };
            return (string)base.Invoke("GetMySiteData", objArray)[0];
        }

        public void GetMySiteDataAsync(string sSiteURL)
        {
            this.GetMySiteDataAsync(sSiteURL, null);
        }

        public void GetMySiteDataAsync(string sSiteURL, object userState)
        {
            if (this.GetMySiteDataOperationCompleted == null)
            {
                this.GetMySiteDataOperationCompleted = new SendOrPostCallback(this.OnGetMySiteDataOperationCompleted);
            }

            object[] objArray = new object[] { sSiteURL };
            base.InvokeAsync("GetMySiteData", objArray, this.GetMySiteDataOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetPortalListingGroups",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetPortalListingGroups()
        {
            object[] objArray = base.Invoke("GetPortalListingGroups", new object[0]);
            return (string)objArray[0];
        }

        public void GetPortalListingGroupsAsync()
        {
            this.GetPortalListingGroupsAsync(null);
        }

        public void GetPortalListingGroupsAsync(object userState)
        {
            if (this.GetPortalListingGroupsOperationCompleted == null)
            {
                this.GetPortalListingGroupsOperationCompleted =
                    new SendOrPostCallback(this.OnGetPortalListingGroupsOperationCompleted);
            }

            base.InvokeAsync("GetPortalListingGroups", new object[0], this.GetPortalListingGroupsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetPortalListingIDs",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetPortalListingIDs()
        {
            object[] objArray = base.Invoke("GetPortalListingIDs", new object[0]);
            return (string)objArray[0];
        }

        public void GetPortalListingIDsAsync()
        {
            this.GetPortalListingIDsAsync(null);
        }

        public void GetPortalListingIDsAsync(object userState)
        {
            if (this.GetPortalListingIDsOperationCompleted == null)
            {
                this.GetPortalListingIDsOperationCompleted =
                    new SendOrPostCallback(this.OnGetPortalListingIDsOperationCompleted);
            }

            base.InvokeAsync("GetPortalListingIDs", new object[0], this.GetPortalListingIDsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetPortalListings",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetPortalListings(string sIDList)
        {
            object[] objArray = new object[] { sIDList };
            return (string)base.Invoke("GetPortalListings", objArray)[0];
        }

        public void GetPortalListingsAsync(string sIDList)
        {
            this.GetPortalListingsAsync(sIDList, null);
        }

        public void GetPortalListingsAsync(string sIDList, object userState)
        {
            if (this.GetPortalListingsOperationCompleted == null)
            {
                this.GetPortalListingsOperationCompleted =
                    new SendOrPostCallback(this.OnGetPortalListingsOperationCompleted);
            }

            object[] objArray = new object[] { sIDList };
            base.InvokeAsync("GetPortalListings", objArray, this.GetPortalListingsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetReferencedTaxonomyFullXml",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            object[] objArray = new object[] { sReferencedTaxonomyXml };
            return (string)base.Invoke("GetReferencedTaxonomyFullXml", objArray)[0];
        }

        public void GetReferencedTaxonomyFullXmlAsync(string sReferencedTaxonomyXml)
        {
            this.GetReferencedTaxonomyFullXmlAsync(sReferencedTaxonomyXml, null);
        }

        public void GetReferencedTaxonomyFullXmlAsync(string sReferencedTaxonomyXml, object userState)
        {
            if (this.GetReferencedTaxonomyFullXmlOperationCompleted == null)
            {
                this.GetReferencedTaxonomyFullXmlOperationCompleted =
                    new SendOrPostCallback(this.OnGetReferencedTaxonomyFullXmlOperationCompleted);
            }

            object[] objArray = new object[] { sReferencedTaxonomyXml };
            base.InvokeAsync("GetReferencedTaxonomyFullXml", objArray,
                this.GetReferencedTaxonomyFullXmlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetRoleAssignments",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetRoleAssignments(string sListId, int iItemId)
        {
            object[] objArray = new object[] { sListId, iItemId };
            return (string)base.Invoke("GetRoleAssignments", objArray)[0];
        }

        public void GetRoleAssignmentsAsync(string sListId, int iItemId)
        {
            this.GetRoleAssignmentsAsync(sListId, iItemId, null);
        }

        public void GetRoleAssignmentsAsync(string sListId, int iItemId, object userState)
        {
            if (this.GetRoleAssignmentsOperationCompleted == null)
            {
                this.GetRoleAssignmentsOperationCompleted =
                    new SendOrPostCallback(this.OnGetRoleAssignmentsOperationCompleted);
            }

            object[] objArray = new object[] { sListId, iItemId };
            base.InvokeAsync("GetRoleAssignments", objArray, this.GetRoleAssignmentsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetRoles", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetRoles(string sListId)
        {
            object[] objArray = new object[] { sListId };
            return (string)base.Invoke("GetRoles", objArray)[0];
        }

        public void GetRolesAsync(string sListId)
        {
            this.GetRolesAsync(sListId, null);
        }

        public void GetRolesAsync(string sListId, object userState)
        {
            if (this.GetRolesOperationCompleted == null)
            {
                this.GetRolesOperationCompleted = new SendOrPostCallback(this.OnGetRolesOperationCompleted);
            }

            object[] objArray = new object[] { sListId };
            base.InvokeAsync("GetRoles", objArray, this.GetRolesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSecureStorageApplications",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSecureStorageApplications(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetSecureStorageApplications", objArray)[0];
        }

        public void GetSecureStorageApplicationsAsync(string options)
        {
            this.GetSecureStorageApplicationsAsync(options, null);
        }

        public void GetSecureStorageApplicationsAsync(string options, object userState)
        {
            if (this.GetSecureStorageApplicationsOperationCompleted == null)
            {
                this.GetSecureStorageApplicationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetSecureStorageApplicationsOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetSecureStorageApplications", objArray,
                this.GetSecureStorageApplicationsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSharePointVersion",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSharePointVersion()
        {
            object[] objArray = base.Invoke("GetSharePointVersion", new object[0]);
            return (string)objArray[0];
        }

        public void GetSharePointVersionAsync()
        {
            this.GetSharePointVersionAsync(null);
        }

        public void GetSharePointVersionAsync(object userState)
        {
            if (this.GetSharePointVersionOperationCompleted == null)
            {
                this.GetSharePointVersionOperationCompleted =
                    new SendOrPostCallback(this.OnGetSharePointVersionOperationCompleted);
            }

            base.InvokeAsync("GetSharePointVersion", new object[0], this.GetSharePointVersionOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSite", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSite(bool bFetchFullXml)
        {
            object[] objArray = new object[] { bFetchFullXml };
            return (string)base.Invoke("GetSite", objArray)[0];
        }

        public void GetSiteAsync(bool bFetchFullXml)
        {
            this.GetSiteAsync(bFetchFullXml, null);
        }

        public void GetSiteAsync(bool bFetchFullXml, object userState)
        {
            if (this.GetSiteOperationCompleted == null)
            {
                this.GetSiteOperationCompleted = new SendOrPostCallback(this.OnGetSiteOperationCompleted);
            }

            object[] objArray = new object[] { bFetchFullXml };
            base.InvokeAsync("GetSite", objArray, this.GetSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSiteCollections",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSiteCollections()
        {
            object[] objArray = base.Invoke("GetSiteCollections", new object[0]);
            return (string)objArray[0];
        }

        public void GetSiteCollectionsAsync()
        {
            this.GetSiteCollectionsAsync(null);
        }

        public void GetSiteCollectionsAsync(object userState)
        {
            if (this.GetSiteCollectionsOperationCompleted == null)
            {
                this.GetSiteCollectionsOperationCompleted =
                    new SendOrPostCallback(this.OnGetSiteCollectionsOperationCompleted);
            }

            base.InvokeAsync("GetSiteCollections", new object[0], this.GetSiteCollectionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSiteCollectionsOnWebApp",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            object[] objArray = new object[] { sWebAppName };
            return (string)base.Invoke("GetSiteCollectionsOnWebApp", objArray)[0];
        }

        public void GetSiteCollectionsOnWebAppAsync(string sWebAppName)
        {
            this.GetSiteCollectionsOnWebAppAsync(sWebAppName, null);
        }

        public void GetSiteCollectionsOnWebAppAsync(string sWebAppName, object userState)
        {
            if (this.GetSiteCollectionsOnWebAppOperationCompleted == null)
            {
                this.GetSiteCollectionsOnWebAppOperationCompleted =
                    new SendOrPostCallback(this.OnGetSiteCollectionsOnWebAppOperationCompleted);
            }

            object[] objArray = new object[] { sWebAppName };
            base.InvokeAsync("GetSiteCollectionsOnWebApp", objArray, this.GetSiteCollectionsOnWebAppOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSiteQuotaTemplates",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSiteQuotaTemplates()
        {
            object[] objArray = base.Invoke("GetSiteQuotaTemplates", new object[0]);
            return (string)objArray[0];
        }

        public void GetSiteQuotaTemplatesAsync()
        {
            this.GetSiteQuotaTemplatesAsync(null);
        }

        public void GetSiteQuotaTemplatesAsync(object userState)
        {
            if (this.GetSiteQuotaTemplatesOperationCompleted == null)
            {
                this.GetSiteQuotaTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnGetSiteQuotaTemplatesOperationCompleted);
            }

            base.InvokeAsync("GetSiteQuotaTemplates", new object[0], this.GetSiteQuotaTemplatesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSiteSolutionsBinary",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetSiteSolutionsBinary(string itemId)
        {
            object[] objArray = new object[] { itemId };
            return (byte[])base.Invoke("GetSiteSolutionsBinary", objArray)[0];
        }

        public void GetSiteSolutionsBinaryAsync(string itemId)
        {
            this.GetSiteSolutionsBinaryAsync(itemId, null);
        }

        public void GetSiteSolutionsBinaryAsync(string itemId, object userState)
        {
            if (this.GetSiteSolutionsBinaryOperationCompleted == null)
            {
                this.GetSiteSolutionsBinaryOperationCompleted =
                    new SendOrPostCallback(this.OnGetSiteSolutionsBinaryOperationCompleted);
            }

            object[] objArray = new object[] { itemId };
            base.InvokeAsync("GetSiteSolutionsBinary", objArray, this.GetSiteSolutionsBinaryOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSiteUsers", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSiteUsers()
        {
            object[] objArray = base.Invoke("GetSiteUsers", new object[0]);
            return (string)objArray[0];
        }

        public void GetSiteUsersAsync()
        {
            this.GetSiteUsersAsync(null);
        }

        public void GetSiteUsersAsync(object userState)
        {
            if (this.GetSiteUsersOperationCompleted == null)
            {
                this.GetSiteUsersOperationCompleted = new SendOrPostCallback(this.OnGetSiteUsersOperationCompleted);
            }

            base.InvokeAsync("GetSiteUsers", new object[0], this.GetSiteUsersOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSP2013Workflows",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSP2013Workflows(string configurationXml)
        {
            object[] objArray = new object[] { configurationXml };
            return (string)base.Invoke("GetSP2013Workflows", objArray)[0];
        }

        public void GetSP2013WorkflowsAsync(string configurationXml)
        {
            this.GetSP2013WorkflowsAsync(configurationXml, null);
        }

        public void GetSP2013WorkflowsAsync(string configurationXml, object userState)
        {
            if (this.GetSP2013WorkflowsOperationCompleted == null)
            {
                this.GetSP2013WorkflowsOperationCompleted =
                    new SendOrPostCallback(this.OnGetSP2013WorkflowsOperationCompleted);
            }

            object[] objArray = new object[] { configurationXml };
            base.InvokeAsync("GetSP2013Workflows", objArray, this.GetSP2013WorkflowsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetStoragePointProfileConfiguration",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            object[] objArray = new object[] { sSharePointPath };
            return (string)base.Invoke("GetStoragePointProfileConfiguration", objArray)[0];
        }

        public void GetStoragePointProfileConfigurationAsync(string sSharePointPath)
        {
            this.GetStoragePointProfileConfigurationAsync(sSharePointPath, null);
        }

        public void GetStoragePointProfileConfigurationAsync(string sSharePointPath, object userState)
        {
            if (this.GetStoragePointProfileConfigurationOperationCompleted == null)
            {
                this.GetStoragePointProfileConfigurationOperationCompleted =
                    new SendOrPostCallback(this.OnGetStoragePointProfileConfigurationOperationCompleted);
            }

            object[] objArray = new object[] { sSharePointPath };
            base.InvokeAsync("GetStoragePointProfileConfiguration", objArray,
                this.GetStoragePointProfileConfigurationOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSubWebs", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSubWebs()
        {
            object[] objArray = base.Invoke("GetSubWebs", new object[0]);
            return (string)objArray[0];
        }

        public void GetSubWebsAsync()
        {
            this.GetSubWebsAsync(null);
        }

        public void GetSubWebsAsync(object userState)
        {
            if (this.GetSubWebsOperationCompleted == null)
            {
                this.GetSubWebsOperationCompleted = new SendOrPostCallback(this.OnGetSubWebsOperationCompleted);
            }

            base.InvokeAsync("GetSubWebs", new object[0], this.GetSubWebsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetSystemInfo", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetSystemInfo()
        {
            object[] objArray = base.Invoke("GetSystemInfo", new object[0]);
            return (string)objArray[0];
        }

        public void GetSystemInfoAsync()
        {
            this.GetSystemInfoAsync(null);
        }

        public void GetSystemInfoAsync(object userState)
        {
            if (this.GetSystemInfoOperationCompleted == null)
            {
                this.GetSystemInfoOperationCompleted = new SendOrPostCallback(this.OnGetSystemInfoOperationCompleted);
            }

            base.InvokeAsync("GetSystemInfo", new object[0], this.GetSystemInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermCollectionFromTerm",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId, sTermId };
            return (string)base.Invoke("GetTermCollectionFromTerm", objArray)[0];
        }

        public void GetTermCollectionFromTermAsync(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            this.GetTermCollectionFromTermAsync(sTermStoreId, sTermGroupId, sTermSetId, sTermId, null);
        }

        public void GetTermCollectionFromTermAsync(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId, object userState)
        {
            if (this.GetTermCollectionFromTermOperationCompleted == null)
            {
                this.GetTermCollectionFromTermOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermCollectionFromTermOperationCompleted);
            }

            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId, sTermId };
            base.InvokeAsync("GetTermCollectionFromTerm", objArray, this.GetTermCollectionFromTermOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermCollectionFromTermSet",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId };
            return (string)base.Invoke("GetTermCollectionFromTermSet", objArray)[0];
        }

        public void GetTermCollectionFromTermSetAsync(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            this.GetTermCollectionFromTermSetAsync(sTermStoreId, sTermGroupId, sTermSetId, null);
        }

        public void GetTermCollectionFromTermSetAsync(string sTermStoreId, string sTermGroupId, string sTermSetId,
            object userState)
        {
            if (this.GetTermCollectionFromTermSetOperationCompleted == null)
            {
                this.GetTermCollectionFromTermSetOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermCollectionFromTermSetOperationCompleted);
            }

            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId };
            base.InvokeAsync("GetTermCollectionFromTermSet", objArray,
                this.GetTermCollectionFromTermSetOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermGroups", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermGroups(string sTermStoreId)
        {
            object[] objArray = new object[] { sTermStoreId };
            return (string)base.Invoke("GetTermGroups", objArray)[0];
        }

        public void GetTermGroupsAsync(string sTermStoreId)
        {
            this.GetTermGroupsAsync(sTermStoreId, null);
        }

        public void GetTermGroupsAsync(string sTermStoreId, object userState)
        {
            if (this.GetTermGroupsOperationCompleted == null)
            {
                this.GetTermGroupsOperationCompleted = new SendOrPostCallback(this.OnGetTermGroupsOperationCompleted);
            }

            object[] objArray = new object[] { sTermStoreId };
            base.InvokeAsync("GetTermGroups", objArray, this.GetTermGroupsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermSetCollection",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            object[] objArray = new object[] { sTermStoreId, sTermGroupId };
            return (string)base.Invoke("GetTermSetCollection", objArray)[0];
        }

        public void GetTermSetCollectionAsync(string sTermStoreId, string sTermGroupId)
        {
            this.GetTermSetCollectionAsync(sTermStoreId, sTermGroupId, null);
        }

        public void GetTermSetCollectionAsync(string sTermStoreId, string sTermGroupId, object userState)
        {
            if (this.GetTermSetCollectionOperationCompleted == null)
            {
                this.GetTermSetCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermSetCollectionOperationCompleted);
            }

            object[] objArray = new object[] { sTermStoreId, sTermGroupId };
            base.InvokeAsync("GetTermSetCollection", objArray, this.GetTermSetCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermSets", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermSets(string sTermGroupId)
        {
            object[] objArray = new object[] { sTermGroupId };
            return (string)base.Invoke("GetTermSets", objArray)[0];
        }

        public void GetTermSetsAsync(string sTermGroupId)
        {
            this.GetTermSetsAsync(sTermGroupId, null);
        }

        public void GetTermSetsAsync(string sTermGroupId, object userState)
        {
            if (this.GetTermSetsOperationCompleted == null)
            {
                this.GetTermSetsOperationCompleted = new SendOrPostCallback(this.OnGetTermSetsOperationCompleted);
            }

            object[] objArray = new object[] { sTermGroupId };
            base.InvokeAsync("GetTermSets", objArray, this.GetTermSetsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermsFromTermSet",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            object[] objArray = new object[] { sTermSetId, bRecursive };
            return (string)base.Invoke("GetTermsFromTermSet", objArray)[0];
        }

        public void GetTermsFromTermSetAsync(string sTermSetId, bool bRecursive)
        {
            this.GetTermsFromTermSetAsync(sTermSetId, bRecursive, null);
        }

        public void GetTermsFromTermSetAsync(string sTermSetId, bool bRecursive, object userState)
        {
            if (this.GetTermsFromTermSetOperationCompleted == null)
            {
                this.GetTermsFromTermSetOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermsFromTermSetOperationCompleted);
            }

            object[] objArray = new object[] { sTermSetId, bRecursive };
            base.InvokeAsync("GetTermsFromTermSet", objArray, this.GetTermsFromTermSetOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermsFromTermSetItem",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            object[] objArray = new object[] { sTermSetItemId };
            return (string)base.Invoke("GetTermsFromTermSetItem", objArray)[0];
        }

        public void GetTermsFromTermSetItemAsync(string sTermSetItemId)
        {
            this.GetTermsFromTermSetItemAsync(sTermSetItemId, null);
        }

        public void GetTermsFromTermSetItemAsync(string sTermSetItemId, object userState)
        {
            if (this.GetTermsFromTermSetItemOperationCompleted == null)
            {
                this.GetTermsFromTermSetItemOperationCompleted =
                    new SendOrPostCallback(this.OnGetTermsFromTermSetItemOperationCompleted);
            }

            object[] objArray = new object[] { sTermSetItemId };
            base.InvokeAsync("GetTermsFromTermSetItem", objArray, this.GetTermsFromTermSetItemOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetTermStores", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTermStores()
        {
            object[] objArray = base.Invoke("GetTermStores", new object[0]);
            return (string)objArray[0];
        }

        public void GetTermStoresAsync()
        {
            this.GetTermStoresAsync(null);
        }

        public void GetTermStoresAsync(object userState)
        {
            if (this.GetTermStoresOperationCompleted == null)
            {
                this.GetTermStoresOperationCompleted = new SendOrPostCallback(this.OnGetTermStoresOperationCompleted);
            }

            base.InvokeAsync("GetTermStores", new object[0], this.GetTermStoresOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetUserFromProfile",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetUserFromProfile()
        {
            object[] objArray = base.Invoke("GetUserFromProfile", new object[0]);
            return (string)objArray[0];
        }

        public void GetUserFromProfileAsync()
        {
            this.GetUserFromProfileAsync(null);
        }

        public void GetUserFromProfileAsync(object userState)
        {
            if (this.GetUserFromProfileOperationCompleted == null)
            {
                this.GetUserFromProfileOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserFromProfileOperationCompleted);
            }

            base.InvokeAsync("GetUserFromProfile", new object[0], this.GetUserFromProfileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetUserProfilePropertiesUsage",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetUserProfilePropertiesUsage(string profileDbConnectionString, string options)
        {
            object[] objArray = new object[] { profileDbConnectionString, options };
            return (string)base.Invoke("GetUserProfilePropertiesUsage", objArray)[0];
        }

        public void GetUserProfilePropertiesUsageAsync(string profileDbConnectionString, string options)
        {
            this.GetUserProfilePropertiesUsageAsync(profileDbConnectionString, options, null);
        }

        public void GetUserProfilePropertiesUsageAsync(string profileDbConnectionString, string options,
            object userState)
        {
            if (this.GetUserProfilePropertiesUsageOperationCompleted == null)
            {
                this.GetUserProfilePropertiesUsageOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfilePropertiesUsageOperationCompleted);
            }

            object[] objArray = new object[] { profileDbConnectionString, options };
            base.InvokeAsync("GetUserProfilePropertiesUsage", objArray,
                this.GetUserProfilePropertiesUsageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetUserProfiles", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
        {
            object[] objArray = new object[] { sSiteURL, sLoginName };
            object[] objArray1 = base.Invoke("GetUserProfiles", objArray);
            sErrors = (string)objArray1[1];
            return (string)objArray1[0];
        }

        public void GetUserProfilesAsync(string sSiteURL, string sLoginName)
        {
            this.GetUserProfilesAsync(sSiteURL, sLoginName, null);
        }

        public void GetUserProfilesAsync(string sSiteURL, string sLoginName, object userState)
        {
            if (this.GetUserProfilesOperationCompleted == null)
            {
                this.GetUserProfilesOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfilesOperationCompleted);
            }

            object[] objArray = new object[] { sSiteURL, sLoginName };
            base.InvokeAsync("GetUserProfiles", objArray, this.GetUserProfilesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWeb", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWeb(bool bFetchFullXml)
        {
            object[] objArray = new object[] { bFetchFullXml };
            return (string)base.Invoke("GetWeb", objArray)[0];
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebApplicationPolicies",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebApplicationPolicies(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetWebApplicationPolicies", objArray)[0];
        }

        public void GetWebApplicationPoliciesAsync(string options)
        {
            this.GetWebApplicationPoliciesAsync(options, null);
        }

        public void GetWebApplicationPoliciesAsync(string options, object userState)
        {
            if (this.GetWebApplicationPoliciesOperationCompleted == null)
            {
                this.GetWebApplicationPoliciesOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebApplicationPoliciesOperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetWebApplicationPolicies", objArray, this.GetWebApplicationPoliciesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebApplications",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebApplications()
        {
            object[] objArray = base.Invoke("GetWebApplications", new object[0]);
            return (string)objArray[0];
        }

        public void GetWebApplicationsAsync()
        {
            this.GetWebApplicationsAsync(null);
        }

        public void GetWebApplicationsAsync(object userState)
        {
            if (this.GetWebApplicationsOperationCompleted == null)
            {
                this.GetWebApplicationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebApplicationsOperationCompleted);
            }

            base.InvokeAsync("GetWebApplications", new object[0], this.GetWebApplicationsOperationCompleted, userState);
        }

        public void GetWebAsync(bool bFetchFullXml)
        {
            this.GetWebAsync(bFetchFullXml, null);
        }

        public void GetWebAsync(bool bFetchFullXml, object userState)
        {
            if (this.GetWebOperationCompleted == null)
            {
                this.GetWebOperationCompleted = new SendOrPostCallback(this.OnGetWebOperationCompleted);
            }

            object[] objArray = new object[] { bFetchFullXml };
            base.InvokeAsync("GetWeb", objArray, this.GetWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebNavigationSettings",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebNavigationSettings()
        {
            object[] objArray = base.Invoke("GetWebNavigationSettings", new object[0]);
            return (string)objArray[0];
        }

        public void GetWebNavigationSettingsAsync()
        {
            this.GetWebNavigationSettingsAsync(null);
        }

        public void GetWebNavigationSettingsAsync(object userState)
        {
            if (this.GetWebNavigationSettingsOperationCompleted == null)
            {
                this.GetWebNavigationSettingsOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebNavigationSettingsOperationCompleted);
            }

            base.InvokeAsync("GetWebNavigationSettings", new object[0], this.GetWebNavigationSettingsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebNavigationStructure",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebNavigationStructure()
        {
            object[] objArray = base.Invoke("GetWebNavigationStructure", new object[0]);
            return (string)objArray[0];
        }

        public void GetWebNavigationStructureAsync()
        {
            this.GetWebNavigationStructureAsync(null);
        }

        public void GetWebNavigationStructureAsync(object userState)
        {
            if (this.GetWebNavigationStructureOperationCompleted == null)
            {
                this.GetWebNavigationStructureOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebNavigationStructureOperationCompleted);
            }

            base.InvokeAsync("GetWebNavigationStructure", new object[0],
                this.GetWebNavigationStructureOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebPartPage", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.Invoke("GetWebPartPage", objArray)[0];
        }

        public void GetWebPartPageAsync(string sWebPartPageServerRelativeUrl)
        {
            this.GetWebPartPageAsync(sWebPartPageServerRelativeUrl, null);
        }

        public void GetWebPartPageAsync(string sWebPartPageServerRelativeUrl, object userState)
        {
            if (this.GetWebPartPageOperationCompleted == null)
            {
                this.GetWebPartPageOperationCompleted = new SendOrPostCallback(this.OnGetWebPartPageOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            base.InvokeAsync("GetWebPartPage", objArray, this.GetWebPartPageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebPartPageTemplate",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            object[] objArray = new object[] { iTemplateId };
            return (byte[])base.Invoke("GetWebPartPageTemplate", objArray)[0];
        }

        public void GetWebPartPageTemplateAsync(int iTemplateId)
        {
            this.GetWebPartPageTemplateAsync(iTemplateId, null);
        }

        public void GetWebPartPageTemplateAsync(int iTemplateId, object userState)
        {
            if (this.GetWebPartPageTemplateOperationCompleted == null)
            {
                this.GetWebPartPageTemplateOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartPageTemplateOperationCompleted);
            }

            object[] objArray = new object[] { iTemplateId };
            base.InvokeAsync("GetWebPartPageTemplate", objArray, this.GetWebPartPageTemplateOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebPartsOnPage",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.Invoke("GetWebPartsOnPage", objArray)[0];
        }

        public void GetWebPartsOnPageAsync(string sWebPartPageServerRelativeUrl)
        {
            this.GetWebPartsOnPageAsync(sWebPartPageServerRelativeUrl, null);
        }

        public void GetWebPartsOnPageAsync(string sWebPartPageServerRelativeUrl, object userState)
        {
            if (this.GetWebPartsOnPageOperationCompleted == null)
            {
                this.GetWebPartsOnPageOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebPartsOnPageOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            base.InvokeAsync("GetWebPartsOnPage", objArray, this.GetWebPartsOnPageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWebTemplates", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWebTemplates()
        {
            object[] objArray = base.Invoke("GetWebTemplates", new object[0]);
            return (string)objArray[0];
        }

        public void GetWebTemplatesAsync()
        {
            this.GetWebTemplatesAsync(null);
        }

        public void GetWebTemplatesAsync(object userState)
        {
            if (this.GetWebTemplatesOperationCompleted == null)
            {
                this.GetWebTemplatesOperationCompleted =
                    new SendOrPostCallback(this.OnGetWebTemplatesOperationCompleted);
            }

            base.InvokeAsync("GetWebTemplates", new object[0], this.GetWebTemplatesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflowAssociation2010",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflowAssociation2010(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetWorkflowAssociation2010", objArray)[0];
        }

        public void GetWorkflowAssociation2010Async(string options)
        {
            this.GetWorkflowAssociation2010Async(options, null);
        }

        public void GetWorkflowAssociation2010Async(string options, object userState)
        {
            if (this.GetWorkflowAssociation2010OperationCompleted == null)
            {
                this.GetWorkflowAssociation2010OperationCompleted =
                    new SendOrPostCallback(this.OnGetWorkflowAssociation2010OperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetWorkflowAssociation2010", objArray, this.GetWorkflowAssociation2010OperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflowAssociation2013",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflowAssociation2013(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetWorkflowAssociation2013", objArray)[0];
        }

        public void GetWorkflowAssociation2013Async(string options)
        {
            this.GetWorkflowAssociation2013Async(options, null);
        }

        public void GetWorkflowAssociation2013Async(string options, object userState)
        {
            if (this.GetWorkflowAssociation2013OperationCompleted == null)
            {
                this.GetWorkflowAssociation2013OperationCompleted =
                    new SendOrPostCallback(this.OnGetWorkflowAssociation2013OperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetWorkflowAssociation2013", objArray, this.GetWorkflowAssociation2013OperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflowAssociations",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflowAssociations(string sObjectID, string sObjectType)
        {
            object[] objArray = new object[] { sObjectID, sObjectType };
            return (string)base.Invoke("GetWorkflowAssociations", objArray)[0];
        }

        public void GetWorkflowAssociationsAsync(string sObjectID, string sObjectType)
        {
            this.GetWorkflowAssociationsAsync(sObjectID, sObjectType, null);
        }

        public void GetWorkflowAssociationsAsync(string sObjectID, string sObjectType, object userState)
        {
            if (this.GetWorkflowAssociationsOperationCompleted == null)
            {
                this.GetWorkflowAssociationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetWorkflowAssociationsOperationCompleted);
            }

            object[] objArray = new object[] { sObjectID, sObjectType };
            base.InvokeAsync("GetWorkflowAssociations", objArray, this.GetWorkflowAssociationsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflowRunning2010",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflowRunning2010(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetWorkflowRunning2010", objArray)[0];
        }

        public void GetWorkflowRunning2010Async(string options)
        {
            this.GetWorkflowRunning2010Async(options, null);
        }

        public void GetWorkflowRunning2010Async(string options, object userState)
        {
            if (this.GetWorkflowRunning2010OperationCompleted == null)
            {
                this.GetWorkflowRunning2010OperationCompleted =
                    new SendOrPostCallback(this.OnGetWorkflowRunning2010OperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetWorkflowRunning2010", objArray, this.GetWorkflowRunning2010OperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflowRunning2013",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflowRunning2013(string options)
        {
            object[] objArray = new object[] { options };
            return (string)base.Invoke("GetWorkflowRunning2013", objArray)[0];
        }

        public void GetWorkflowRunning2013Async(string options)
        {
            this.GetWorkflowRunning2013Async(options, null);
        }

        public void GetWorkflowRunning2013Async(string options, object userState)
        {
            if (this.GetWorkflowRunning2013OperationCompleted == null)
            {
                this.GetWorkflowRunning2013OperationCompleted =
                    new SendOrPostCallback(this.OnGetWorkflowRunning2013OperationCompleted);
            }

            object[] objArray = new object[] { options };
            base.InvokeAsync("GetWorkflowRunning2013", objArray, this.GetWorkflowRunning2013OperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/GetWorkflows", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetWorkflows(string sListId, int sItemId)
        {
            object[] objArray = new object[] { sListId, sItemId };
            return (string)base.Invoke("GetWorkflows", objArray)[0];
        }

        public void GetWorkflowsAsync(string sListId, int sItemId)
        {
            this.GetWorkflowsAsync(sListId, sItemId, null);
        }

        public void GetWorkflowsAsync(string sListId, int sItemId, object userState)
        {
            if (this.GetWorkflowsOperationCompleted == null)
            {
                this.GetWorkflowsOperationCompleted = new SendOrPostCallback(this.OnGetWorkflowsOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sItemId };
            base.InvokeAsync("GetWorkflows", objArray, this.GetWorkflowsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/HasDocument", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            object[] objArray = new object[] { sDocumentServerRelativeUrl };
            return (string)base.Invoke("HasDocument", objArray)[0];
        }

        public void HasDocumentAsync(string sDocumentServerRelativeUrl)
        {
            this.HasDocumentAsync(sDocumentServerRelativeUrl, null);
        }

        public void HasDocumentAsync(string sDocumentServerRelativeUrl, object userState)
        {
            if (this.HasDocumentOperationCompleted == null)
            {
                this.HasDocumentOperationCompleted = new SendOrPostCallback(this.OnHasDocumentOperationCompleted);
            }

            object[] objArray = new object[] { sDocumentServerRelativeUrl };
            base.InvokeAsync("HasDocument", objArray, this.HasDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/HasUniquePermissions",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string HasUniquePermissions(string listID, int listItemID)
        {
            object[] objArray = new object[] { listID, listItemID };
            return (string)base.Invoke("HasUniquePermissions", objArray)[0];
        }

        public void HasUniquePermissionsAsync(string listID, int listItemID)
        {
            this.HasUniquePermissionsAsync(listID, listItemID, null);
        }

        public void HasUniquePermissionsAsync(string listID, int listItemID, object userState)
        {
            if (this.HasUniquePermissionsOperationCompleted == null)
            {
                this.HasUniquePermissionsOperationCompleted =
                    new SendOrPostCallback(this.OnHasUniquePermissionsOperationCompleted);
            }

            object[] objArray = new object[] { listID, listItemID };
            base.InvokeAsync("HasUniquePermissions", objArray, this.HasUniquePermissionsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/HasWebParts", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.Invoke("HasWebParts", objArray)[0];
        }

        public void HasWebPartsAsync(string sWebPartPageServerRelativeUrl)
        {
            this.HasWebPartsAsync(sWebPartPageServerRelativeUrl, null);
        }

        public void HasWebPartsAsync(string sWebPartPageServerRelativeUrl, object userState)
        {
            if (this.HasWebPartsOperationCompleted == null)
            {
                this.HasWebPartsOperationCompleted = new SendOrPostCallback(this.OnHasWebPartsOperationCompleted);
            }

            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            base.InvokeAsync("HasWebParts", objArray, this.HasWebPartsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/HasWorkflows", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string HasWorkflows(string sListId, string sItemId)
        {
            object[] objArray = new object[] { sListId, sItemId };
            return (string)base.Invoke("HasWorkflows", objArray)[0];
        }

        public void HasWorkflowsAsync(string sListId, string sItemId)
        {
            this.HasWorkflowsAsync(sListId, sItemId, null);
        }

        public void HasWorkflowsAsync(string sListId, string sItemId, object userState)
        {
            if (this.HasWorkflowsOperationCompleted == null)
            {
                this.HasWorkflowsOperationCompleted = new SendOrPostCallback(this.OnHasWorkflowsOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sItemId };
            base.InvokeAsync("HasWorkflows", objArray, this.HasWorkflowsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/IsAppWebPartOnPage",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string IsAppWebPartOnPage(Guid appProductId, string itemUrl)
        {
            object[] objArray = new object[] { appProductId, itemUrl };
            return (string)base.Invoke("IsAppWebPartOnPage", objArray)[0];
        }

        public void IsAppWebPartOnPageAsync(Guid appProductId, string itemUrl)
        {
            this.IsAppWebPartOnPageAsync(appProductId, itemUrl, null);
        }

        public void IsAppWebPartOnPageAsync(Guid appProductId, string itemUrl, object userState)
        {
            if (this.IsAppWebPartOnPageOperationCompleted == null)
            {
                this.IsAppWebPartOnPageOperationCompleted =
                    new SendOrPostCallback(this.OnIsAppWebPartOnPageOperationCompleted);
            }

            object[] objArray = new object[] { appProductId, itemUrl };
            base.InvokeAsync("IsAppWebPartOnPage", objArray, this.IsAppWebPartOnPageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/IsListContainsInfoPathOrAspxItem",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            object[] objArray = new object[] { listId };
            return (string)base.Invoke("IsListContainsInfoPathOrAspxItem", objArray)[0];
        }

        public void IsListContainsInfoPathOrAspxItemAsync(string listId)
        {
            this.IsListContainsInfoPathOrAspxItemAsync(listId, null);
        }

        public void IsListContainsInfoPathOrAspxItemAsync(string listId, object userState)
        {
            if (this.IsListContainsInfoPathOrAspxItemOperationCompleted == null)
            {
                this.IsListContainsInfoPathOrAspxItemOperationCompleted =
                    new SendOrPostCallback(this.OnIsListContainsInfoPathOrAspxItemOperationCompleted);
            }

            object[] objArray = new object[] { listId };
            base.InvokeAsync("IsListContainsInfoPathOrAspxItem", objArray,
                this.IsListContainsInfoPathOrAspxItemOperationCompleted, userState);
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

        [SoapDocumentMethod("http://www.metalogix.net/IsWorkflowServicesInstanceAvailable",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string IsWorkflowServicesInstanceAvailable()
        {
            object[] objArray = base.Invoke("IsWorkflowServicesInstanceAvailable", new object[0]);
            return (string)objArray[0];
        }

        public void IsWorkflowServicesInstanceAvailableAsync()
        {
            this.IsWorkflowServicesInstanceAvailableAsync(null);
        }

        public void IsWorkflowServicesInstanceAvailableAsync(object userState)
        {
            if (this.IsWorkflowServicesInstanceAvailableOperationCompleted == null)
            {
                this.IsWorkflowServicesInstanceAvailableOperationCompleted =
                    new SendOrPostCallback(this.OnIsWorkflowServicesInstanceAvailableOperationCompleted);
            }

            base.InvokeAsync("IsWorkflowServicesInstanceAvailable", new object[0],
                this.IsWorkflowServicesInstanceAvailableOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/MigrateSP2013Workflows",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string MigrateSP2013Workflows(string configurationXml)
        {
            object[] objArray = new object[] { configurationXml };
            return (string)base.Invoke("MigrateSP2013Workflows", objArray)[0];
        }

        public void MigrateSP2013WorkflowsAsync(string configurationXml)
        {
            this.MigrateSP2013WorkflowsAsync(configurationXml, null);
        }

        public void MigrateSP2013WorkflowsAsync(string configurationXml, object userState)
        {
            if (this.MigrateSP2013WorkflowsOperationCompleted == null)
            {
                this.MigrateSP2013WorkflowsOperationCompleted =
                    new SendOrPostCallback(this.OnMigrateSP2013WorkflowsOperationCompleted);
            }

            object[] objArray = new object[] { configurationXml };
            base.InvokeAsync("MigrateSP2013Workflows", objArray, this.MigrateSP2013WorkflowsOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ModifyWebNavigationSettings",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ModifyWebNavigationSettings(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ModifyNavigationOptions ModNavOptions)
        {
            object[] objArray = new object[] { sWebXML, ModNavOptions };
            return (string)base.Invoke("ModifyWebNavigationSettings", objArray)[0];
        }

        public void ModifyWebNavigationSettingsAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ModifyNavigationOptions ModNavOptions)
        {
            this.ModifyWebNavigationSettingsAsync(sWebXML, ModNavOptions, null);
        }

        public void ModifyWebNavigationSettingsAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.ModifyNavigationOptions ModNavOptions,
            object userState)
        {
            if (this.ModifyWebNavigationSettingsOperationCompleted == null)
            {
                this.ModifyWebNavigationSettingsOperationCompleted =
                    new SendOrPostCallback(this.OnModifyWebNavigationSettingsOperationCompleted);
            }

            object[] objArray = new object[] { sWebXML, ModNavOptions };
            base.InvokeAsync("ModifyWebNavigationSettings", objArray,
                this.ModifyWebNavigationSettingsOperationCompleted, userState);
        }

        private void OnActivateReusableWorkflowTemplatesOperationCompleted(object arg)
        {
            if (this.ActivateReusableWorkflowTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ActivateReusableWorkflowTemplatesCompleted(this,
                    new ActivateReusableWorkflowTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddAlertsOperationCompleted(object arg)
        {
            if (this.AddAlertsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddAlertsCompleted(this,
                    new AddAlertsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddDocumentChunkedOperationCompleted(object arg)
        {
            if (this.AddDocumentChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddDocumentChunkedCompleted(this,
                    new AddDocumentChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddDocumentOperationCompleted(object arg)
        {
            if (this.AddDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddDocumentCompleted(this,
                    new AddDocumentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddDocumentOptimisticallyOperationCompleted(object arg)
        {
            if (this.AddDocumentOptimisticallyCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddDocumentOptimisticallyCompleted(this,
                    new AddDocumentOptimisticallyCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddDocumentTemplatetoContentTypeOperationCompleted(object arg)
        {
            if (this.AddDocumentTemplatetoContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddDocumentTemplatetoContentTypeCompleted(this,
                    new AddDocumentTemplatetoContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFieldsOperationCompleted(object arg)
        {
            if (this.AddFieldsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFieldsCompleted(this,
                    new AddFieldsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFileToFolderOperationCompleted(object arg)
        {
            if (this.AddFileToFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFileToFolderCompleted(this,
                    new AddFileToFolderCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFolderOperationCompleted(object arg)
        {
            if (this.AddFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFolderCompleted(this,
                    new AddFolderCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFolderOptimisticallyOperationCompleted(object arg)
        {
            if (this.AddFolderOptimisticallyCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFolderOptimisticallyCompleted(this,
                    new AddFolderOptimisticallyCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFolderToFolderOperationCompleted(object arg)
        {
            if (this.AddFolderToFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFolderToFolderCompleted(this,
                    new AddFolderToFolderCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddFormTemplateToContentTypeOperationCompleted(object arg)
        {
            if (this.AddFormTemplateToContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddFormTemplateToContentTypeCompleted(this,
                    new AddFormTemplateToContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddListItemChunkedOperationCompleted(object arg)
        {
            if (this.AddListItemChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddListItemChunkedCompleted(this,
                    new AddListItemChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddListItemOperationCompleted(object arg)
        {
            if (this.AddListItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddListItemCompleted(this,
                    new AddListItemCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
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

        private void OnAddOrUpdateAudienceOperationCompleted(object arg)
        {
            if (this.AddOrUpdateAudienceCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddOrUpdateAudienceCompleted(this,
                    new AddOrUpdateAudienceCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddOrUpdateContentTypeOperationCompleted(object arg)
        {
            if (this.AddOrUpdateContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddOrUpdateContentTypeCompleted(this,
                    new AddOrUpdateContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddOrUpdateGroupOperationCompleted(object arg)
        {
            if (this.AddOrUpdateGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddOrUpdateGroupCompleted(this,
                    new AddOrUpdateGroupCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddOrUpdateRoleOperationCompleted(object arg)
        {
            if (this.AddOrUpdateRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddOrUpdateRoleCompleted(this,
                    new AddOrUpdateRoleCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddReferencedTaxonomyDataOperationCompleted(object arg)
        {
            if (this.AddReferencedTaxonomyDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddReferencedTaxonomyDataCompleted(this,
                    new AddReferencedTaxonomyDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddReusedTermsOperationCompleted(object arg)
        {
            if (this.AddReusedTermsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddReusedTermsCompleted(this,
                    new AddReusedTermsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddRoleAssignmentOperationCompleted(object arg)
        {
            if (this.AddRoleAssignmentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddRoleAssignmentCompleted(this,
                    new AddRoleAssignmentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddSiteCollectionOperationCompleted(object arg)
        {
            if (this.AddSiteCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddSiteCollectionCompleted(this,
                    new AddSiteCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddSiteUserOperationCompleted(object arg)
        {
            if (this.AddSiteUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddSiteUserCompleted(this,
                    new AddSiteUserCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTermGroupOperationCompleted(object arg)
        {
            if (this.AddTermGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTermGroupCompleted(this,
                    new AddTermGroupCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTermOperationCompleted(object arg)
        {
            if (this.AddTermCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTermCompleted(this,
                    new AddTermCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTermSetOperationCompleted(object arg)
        {
            if (this.AddTermSetCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTermSetCompleted(this,
                    new AddTermSetCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTermstoreLanguagesOperationCompleted(object arg)
        {
            if (this.AddTermstoreLanguagesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTermstoreLanguagesCompleted(this,
                    new AddTermstoreLanguagesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
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

        private void OnAddWebOperationCompleted(object arg)
        {
            if (this.AddWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWebCompleted(this,
                    new AddWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddWebPartsOperationCompleted(object arg)
        {
            if (this.AddWebPartsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWebPartsCompleted(this,
                    new AddWebPartsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddWorkflowAssociationOperationCompleted(object arg)
        {
            if (this.AddWorkflowAssociationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWorkflowAssociationCompleted(this,
                    new AddWorkflowAssociationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddWorkflowOperationCompleted(object arg)
        {
            if (this.AddWorkflowCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddWorkflowCompleted(this,
                    new AddWorkflowCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAnalyzeChurnOperationCompleted(object arg)
        {
            if (this.AnalyzeChurnCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AnalyzeChurnCompleted(this,
                    new AnalyzeChurnCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnApply2013ThemeOperationCompleted(object arg)
        {
            if (this.Apply2013ThemeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.Apply2013ThemeCompleted(this,
                    new Apply2013ThemeCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnApplyOrUpdateContentTypeOperationCompleted(object arg)
        {
            if (this.ApplyOrUpdateContentTypeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ApplyOrUpdateContentTypeCompleted(this,
                    new ApplyOrUpdateContentTypeCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnBeginCompilingAllAudiencesOperationCompleted(object arg)
        {
            if (this.BeginCompilingAllAudiencesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.BeginCompilingAllAudiencesCompleted(this,
                    new BeginCompilingAllAudiencesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCatalogDocumentToStoragePointFileShareEndpointOperationCompleted(object arg)
        {
            if (this.CatalogDocumentToStoragePointFileShareEndpointCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CatalogDocumentToStoragePointFileShareEndpointCompleted(this,
                    new CatalogDocumentToStoragePointFileShareEndpointCompletedEventArgs(
                        invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCloseFileCopySessionOperationCompleted(object arg)
        {
            if (this.CloseFileCopySessionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CloseFileCopySessionCompleted(this,
                    new CloseFileCopySessionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCloseWebPartsOperationCompleted(object arg)
        {
            if (this.CloseWebPartsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CloseWebPartsCompleted(this,
                    new CloseWebPartsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnConfigureStoragePointFileShareEndpointAndProfileOperationCompleted(object arg)
        {
            if (this.ConfigureStoragePointFileShareEndpointAndProfileCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ConfigureStoragePointFileShareEndpointAndProfileCompleted(this,
                    new ConfigureStoragePointFileShareEndpointAndProfileCompletedEventArgs(
                        invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCorrectDefaultPageVersionsOperationCompleted(object arg)
        {
            if (this.CorrectDefaultPageVersionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CorrectDefaultPageVersionsCompleted(this,
                    new CorrectDefaultPageVersionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteAllAudiencesOperationCompleted(object arg)
        {
            if (this.DeleteAllAudiencesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAllAudiencesCompleted(this,
                    new DeleteAllAudiencesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteAudienceOperationCompleted(object arg)
        {
            if (this.DeleteAudienceCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteAudienceCompleted(this,
                    new DeleteAudienceCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteContentTypesOperationCompleted(object arg)
        {
            if (this.DeleteContentTypesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteContentTypesCompleted(this,
                    new DeleteContentTypesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteFolderOperationCompleted(object arg)
        {
            if (this.DeleteFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteFolderCompleted(this,
                    new DeleteFolderCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteGroupOperationCompleted(object arg)
        {
            if (this.DeleteGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteGroupCompleted(this,
                    new DeleteGroupCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteItemOperationCompleted(object arg)
        {
            if (this.DeleteItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteItemCompleted(this,
                    new DeleteItemCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteItemsOperationCompleted(object arg)
        {
            if (this.DeleteItemsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteItemsCompleted(this,
                    new DeleteItemsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteListOperationCompleted(object arg)
        {
            if (this.DeleteListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteListCompleted(this,
                    new DeleteListCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteRoleAssignmentOperationCompleted(object arg)
        {
            if (this.DeleteRoleAssignmentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteRoleAssignmentCompleted(this,
                    new DeleteRoleAssignmentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteRoleOperationCompleted(object arg)
        {
            if (this.DeleteRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteRoleCompleted(this,
                    new DeleteRoleCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteSiteCollectionOperationCompleted(object arg)
        {
            if (this.DeleteSiteCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteSiteCollectionCompleted(this,
                    new DeleteSiteCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteSP2013WorkflowsOperationCompleted(object arg)
        {
            if (this.DeleteSP2013WorkflowsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteSP2013WorkflowsCompleted(this,
                    new DeleteSP2013WorkflowsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteWebOperationCompleted(object arg)
        {
            if (this.DeleteWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteWebCompleted(this,
                    new DeleteWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteWebPartOperationCompleted(object arg)
        {
            if (this.DeleteWebPartCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteWebPartCompleted(this,
                    new DeleteWebPartCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteWebPartsOperationCompleted(object arg)
        {
            if (this.DeleteWebPartsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteWebPartsCompleted(this,
                    new DeleteWebPartsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnExecuteCommandOperationCompleted(object arg)
        {
            if (this.ExecuteCommandCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ExecuteCommandCompleted(this,
                    new ExecuteCommandCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnFindAlertsOperationCompleted(object arg)
        {
            if (this.FindAlertsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.FindAlertsCompleted(this,
                    new FindAlertsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnFindUniquePermissionsOperationCompleted(object arg)
        {
            if (this.FindUniquePermissionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.FindUniquePermissionsCompleted(this,
                    new FindUniquePermissionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAddInsOperationCompleted(object arg)
        {
            if (this.GetAddInsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAddInsCompleted(this,
                    new GetAddInsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAdImportDcMappingsOperationCompleted(object arg)
        {
            if (this.GetAdImportDcMappingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAdImportDcMappingsCompleted(this,
                    new GetAdImportDcMappingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAlertsOperationCompleted(object arg)
        {
            if (this.GetAlertsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAlertsCompleted(this,
                    new GetAlertsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
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

        private void OnGetAudiencesOperationCompleted(object arg)
        {
            if (this.GetAudiencesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAudiencesCompleted(this,
                    new GetAudiencesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetBcsApplicationsOperationCompleted(object arg)
        {
            if (this.GetBcsApplicationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetBcsApplicationsCompleted(this,
                    new GetBcsApplicationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetBrowserFileHandlingOperationCompleted(object arg)
        {
            if (this.GetBrowserFileHandlingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetBrowserFileHandlingCompleted(this,
                    new GetBrowserFileHandlingCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
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

        private void OnGetCustomProfilePropertyMappingOperationCompleted(object arg)
        {
            if (this.GetCustomProfilePropertyMappingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCustomProfilePropertyMappingCompleted(this,
                    new GetCustomProfilePropertyMappingCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDashboardPageTemplateOperationCompleted(object arg)
        {
            if (this.GetDashboardPageTemplateCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDashboardPageTemplateCompleted(this,
                    new GetDashboardPageTemplateCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentBlobRefOperationCompleted(object arg)
        {
            if (this.GetDocumentBlobRefCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentBlobRefCompleted(this,
                    new GetDocumentBlobRefCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentChunkedOperationCompleted(object arg)
        {
            if (this.GetDocumentChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentChunkedCompleted(this,
                    new GetDocumentChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentIdOperationCompleted(object arg)
        {
            if (this.GetDocumentIdCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentIdCompleted(this,
                    new GetDocumentIdCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentOperationCompleted(object arg)
        {
            if (this.GetDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentCompleted(this,
                    new GetDocumentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentVersionBlobRefOperationCompleted(object arg)
        {
            if (this.GetDocumentVersionBlobRefCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentVersionBlobRefCompleted(this,
                    new GetDocumentVersionBlobRefCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentVersionChunkedOperationCompleted(object arg)
        {
            if (this.GetDocumentVersionChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentVersionChunkedCompleted(this,
                    new GetDocumentVersionChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetDocumentVersionOperationCompleted(object arg)
        {
            if (this.GetDocumentVersionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetDocumentVersionCompleted(this,
                    new GetDocumentVersionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetExternalContentTypeOperationsOperationCompleted(object arg)
        {
            if (this.GetExternalContentTypeOperationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetExternalContentTypeOperationsCompleted(this,
                    new GetExternalContentTypeOperationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetExternalContentTypesOperationCompleted(object arg)
        {
            if (this.GetExternalContentTypesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetExternalContentTypesCompleted(this,
                    new GetExternalContentTypesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetExternalItemsOperationCompleted(object arg)
        {
            if (this.GetExternalItemsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetExternalItemsCompleted(this,
                    new GetExternalItemsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFarmSandboxSolutionsOperationCompleted(object arg)
        {
            if (this.GetFarmSandboxSolutionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFarmSandboxSolutionsCompleted(this,
                    new GetFarmSandboxSolutionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFarmServerDetailsOperationCompleted(object arg)
        {
            if (this.GetFarmServerDetailsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFarmServerDetailsCompleted(this,
                    new GetFarmServerDetailsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFarmSolutionBinaryOperationCompleted(object arg)
        {
            if (this.GetFarmSolutionBinaryCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFarmSolutionBinaryCompleted(this,
                    new GetFarmSolutionBinaryCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFarmSolutionsOperationCompleted(object arg)
        {
            if (this.GetFarmSolutionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFarmSolutionsCompleted(this,
                    new GetFarmSolutionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFieldsOperationCompleted(object arg)
        {
            if (this.GetFieldsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFieldsCompleted(this,
                    new GetFieldsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFilesOperationCompleted(object arg)
        {
            if (this.GetFilesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFilesCompleted(this,
                    new GetFilesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFileVersionsOperationCompleted(object arg)
        {
            if (this.GetFileVersionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFileVersionsCompleted(this,
                    new GetFileVersionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetFoldersOperationCompleted(object arg)
        {
            if (this.GetFoldersCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetFoldersCompleted(this,
                    new GetFoldersCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupsOperationCompleted(object arg)
        {
            if (this.GetGroupsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupsCompleted(this,
                    new GetGroupsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetInfopathsOperationCompleted(object arg)
        {
            if (this.GetInfopathsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetInfopathsCompleted(this,
                    new GetInfopathsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLanguagesAndWebTemplatesOperationCompleted(object arg)
        {
            if (this.GetLanguagesAndWebTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLanguagesAndWebTemplatesCompleted(this,
                    new GetLanguagesAndWebTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListItemIDsOperationCompleted(object arg)
        {
            if (this.GetListItemIDsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemIDsCompleted(this,
                    new GetListItemIDsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListItemsByQueryOperationCompleted(object arg)
        {
            if (this.GetListItemsByQueryCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemsByQueryCompleted(this,
                    new GetListItemsByQueryCompletedEventArgs(invokeCompletedEventArg.Results,
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

        private void OnGetListItemVersionsOperationCompleted(object arg)
        {
            if (this.GetListItemVersionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListItemVersionsCompleted(this,
                    new GetListItemVersionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
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

        private void OnGetListsOperationCompleted(object arg)
        {
            if (this.GetListsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListsCompleted(this,
                    new GetListsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
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

        private void OnGetListWorkflowRunning2010OperationCompleted(object arg)
        {
            if (this.GetListWorkflowRunning2010Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListWorkflowRunning2010Completed(this,
                    new GetListWorkflowRunning2010CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLockedSitesOperationCompleted(object arg)
        {
            if (this.GetLockedSitesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLockedSitesCompleted(this,
                    new GetLockedSitesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetMySiteDataOperationCompleted(object arg)
        {
            if (this.GetMySiteDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetMySiteDataCompleted(this,
                    new GetMySiteDataCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPortalListingGroupsOperationCompleted(object arg)
        {
            if (this.GetPortalListingGroupsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPortalListingGroupsCompleted(this,
                    new GetPortalListingGroupsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPortalListingIDsOperationCompleted(object arg)
        {
            if (this.GetPortalListingIDsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPortalListingIDsCompleted(this,
                    new GetPortalListingIDsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPortalListingsOperationCompleted(object arg)
        {
            if (this.GetPortalListingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPortalListingsCompleted(this,
                    new GetPortalListingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetReferencedTaxonomyFullXmlOperationCompleted(object arg)
        {
            if (this.GetReferencedTaxonomyFullXmlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetReferencedTaxonomyFullXmlCompleted(this,
                    new GetReferencedTaxonomyFullXmlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleAssignmentsOperationCompleted(object arg)
        {
            if (this.GetRoleAssignmentsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleAssignmentsCompleted(this,
                    new GetRoleAssignmentsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRolesOperationCompleted(object arg)
        {
            if (this.GetRolesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRolesCompleted(this,
                    new GetRolesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSecureStorageApplicationsOperationCompleted(object arg)
        {
            if (this.GetSecureStorageApplicationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSecureStorageApplicationsCompleted(this,
                    new GetSecureStorageApplicationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSharePointVersionOperationCompleted(object arg)
        {
            if (this.GetSharePointVersionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSharePointVersionCompleted(this,
                    new GetSharePointVersionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteCollectionsOnWebAppOperationCompleted(object arg)
        {
            if (this.GetSiteCollectionsOnWebAppCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteCollectionsOnWebAppCompleted(this,
                    new GetSiteCollectionsOnWebAppCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteCollectionsOperationCompleted(object arg)
        {
            if (this.GetSiteCollectionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteCollectionsCompleted(this,
                    new GetSiteCollectionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
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

        private void OnGetSiteQuotaTemplatesOperationCompleted(object arg)
        {
            if (this.GetSiteQuotaTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteQuotaTemplatesCompleted(this,
                    new GetSiteQuotaTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteSolutionsBinaryOperationCompleted(object arg)
        {
            if (this.GetSiteSolutionsBinaryCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteSolutionsBinaryCompleted(this,
                    new GetSiteSolutionsBinaryCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSiteUsersOperationCompleted(object arg)
        {
            if (this.GetSiteUsersCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSiteUsersCompleted(this,
                    new GetSiteUsersCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSP2013WorkflowsOperationCompleted(object arg)
        {
            if (this.GetSP2013WorkflowsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSP2013WorkflowsCompleted(this,
                    new GetSP2013WorkflowsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetStoragePointProfileConfigurationOperationCompleted(object arg)
        {
            if (this.GetStoragePointProfileConfigurationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetStoragePointProfileConfigurationCompleted(this,
                    new GetStoragePointProfileConfigurationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSubWebsOperationCompleted(object arg)
        {
            if (this.GetSubWebsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSubWebsCompleted(this,
                    new GetSubWebsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSystemInfoOperationCompleted(object arg)
        {
            if (this.GetSystemInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSystemInfoCompleted(this,
                    new GetSystemInfoCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermCollectionFromTermOperationCompleted(object arg)
        {
            if (this.GetTermCollectionFromTermCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermCollectionFromTermCompleted(this,
                    new GetTermCollectionFromTermCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermCollectionFromTermSetOperationCompleted(object arg)
        {
            if (this.GetTermCollectionFromTermSetCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermCollectionFromTermSetCompleted(this,
                    new GetTermCollectionFromTermSetCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermGroupsOperationCompleted(object arg)
        {
            if (this.GetTermGroupsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermGroupsCompleted(this,
                    new GetTermGroupsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermSetCollectionOperationCompleted(object arg)
        {
            if (this.GetTermSetCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermSetCollectionCompleted(this,
                    new GetTermSetCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
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

        private void OnGetTermsFromTermSetItemOperationCompleted(object arg)
        {
            if (this.GetTermsFromTermSetItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermsFromTermSetItemCompleted(this,
                    new GetTermsFromTermSetItemCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermsFromTermSetOperationCompleted(object arg)
        {
            if (this.GetTermsFromTermSetCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermsFromTermSetCompleted(this,
                    new GetTermsFromTermSetCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTermStoresOperationCompleted(object arg)
        {
            if (this.GetTermStoresCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTermStoresCompleted(this,
                    new GetTermStoresCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserFromProfileOperationCompleted(object arg)
        {
            if (this.GetUserFromProfileCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserFromProfileCompleted(this,
                    new GetUserFromProfileCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfilePropertiesUsageOperationCompleted(object arg)
        {
            if (this.GetUserProfilePropertiesUsageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfilePropertiesUsageCompleted(this,
                    new GetUserProfilePropertiesUsageCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfilesOperationCompleted(object arg)
        {
            if (this.GetUserProfilesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfilesCompleted(this,
                    new GetUserProfilesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebApplicationPoliciesOperationCompleted(object arg)
        {
            if (this.GetWebApplicationPoliciesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebApplicationPoliciesCompleted(this,
                    new GetWebApplicationPoliciesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebApplicationsOperationCompleted(object arg)
        {
            if (this.GetWebApplicationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebApplicationsCompleted(this,
                    new GetWebApplicationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebNavigationSettingsOperationCompleted(object arg)
        {
            if (this.GetWebNavigationSettingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebNavigationSettingsCompleted(this,
                    new GetWebNavigationSettingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebNavigationStructureOperationCompleted(object arg)
        {
            if (this.GetWebNavigationStructureCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebNavigationStructureCompleted(this,
                    new GetWebNavigationStructureCompletedEventArgs(invokeCompletedEventArg.Results,
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

        private void OnGetWebPartPageTemplateOperationCompleted(object arg)
        {
            if (this.GetWebPartPageTemplateCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartPageTemplateCompleted(this,
                    new GetWebPartPageTemplateCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebPartsOnPageOperationCompleted(object arg)
        {
            if (this.GetWebPartsOnPageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebPartsOnPageCompleted(this,
                    new GetWebPartsOnPageCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWebTemplatesOperationCompleted(object arg)
        {
            if (this.GetWebTemplatesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWebTemplatesCompleted(this,
                    new GetWebTemplatesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowAssociation2010OperationCompleted(object arg)
        {
            if (this.GetWorkflowAssociation2010Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowAssociation2010Completed(this,
                    new GetWorkflowAssociation2010CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowAssociation2013OperationCompleted(object arg)
        {
            if (this.GetWorkflowAssociation2013Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowAssociation2013Completed(this,
                    new GetWorkflowAssociation2013CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowAssociationsOperationCompleted(object arg)
        {
            if (this.GetWorkflowAssociationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowAssociationsCompleted(this,
                    new GetWorkflowAssociationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowRunning2010OperationCompleted(object arg)
        {
            if (this.GetWorkflowRunning2010Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowRunning2010Completed(this,
                    new GetWorkflowRunning2010CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowRunning2013OperationCompleted(object arg)
        {
            if (this.GetWorkflowRunning2013Completed != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowRunning2013Completed(this,
                    new GetWorkflowRunning2013CompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetWorkflowsOperationCompleted(object arg)
        {
            if (this.GetWorkflowsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetWorkflowsCompleted(this,
                    new GetWorkflowsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnHasDocumentOperationCompleted(object arg)
        {
            if (this.HasDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.HasDocumentCompleted(this,
                    new HasDocumentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnHasUniquePermissionsOperationCompleted(object arg)
        {
            if (this.HasUniquePermissionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.HasUniquePermissionsCompleted(this,
                    new HasUniquePermissionsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnHasWebPartsOperationCompleted(object arg)
        {
            if (this.HasWebPartsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.HasWebPartsCompleted(this,
                    new HasWebPartsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnHasWorkflowsOperationCompleted(object arg)
        {
            if (this.HasWorkflowsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.HasWorkflowsCompleted(this,
                    new HasWorkflowsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnIsAppWebPartOnPageOperationCompleted(object arg)
        {
            if (this.IsAppWebPartOnPageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.IsAppWebPartOnPageCompleted(this,
                    new IsAppWebPartOnPageCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnIsListContainsInfoPathOrAspxItemOperationCompleted(object arg)
        {
            if (this.IsListContainsInfoPathOrAspxItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.IsListContainsInfoPathOrAspxItemCompleted(this,
                    new IsListContainsInfoPathOrAspxItemCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnIsWorkflowServicesInstanceAvailableOperationCompleted(object arg)
        {
            if (this.IsWorkflowServicesInstanceAvailableCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.IsWorkflowServicesInstanceAvailableCompleted(this,
                    new IsWorkflowServicesInstanceAvailableCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnMigrateSP2013WorkflowsOperationCompleted(object arg)
        {
            if (this.MigrateSP2013WorkflowsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.MigrateSP2013WorkflowsCompleted(this,
                    new MigrateSP2013WorkflowsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnModifyWebNavigationSettingsOperationCompleted(object arg)
        {
            if (this.ModifyWebNavigationSettingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ModifyWebNavigationSettingsCompleted(this,
                    new ModifyWebNavigationSettingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnOpenFileCopySessionOperationCompleted(object arg)
        {
            if (this.OpenFileCopySessionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.OpenFileCopySessionCompleted(this,
                    new OpenFileCopySessionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnReadChunkOperationCompleted(object arg)
        {
            if (this.ReadChunkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ReadChunkCompleted(this,
                    new ReadChunkCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnReorderContentTypesOperationCompleted(object arg)
        {
            if (this.ReorderContentTypesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ReorderContentTypesCompleted(this,
                    new ReorderContentTypesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSearchForDocumentOperationCompleted(object arg)
        {
            if (this.SearchForDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SearchForDocumentCompleted(this,
                    new SearchForDocumentCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetDocumentParsingOperationCompleted(object arg)
        {
            if (this.SetDocumentParsingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetDocumentParsingCompleted(this,
                    new SetDocumentParsingCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetMasterPageOperationCompleted(object arg)
        {
            if (this.SetMasterPageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetMasterPageCompleted(this,
                    new SetMasterPageCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetUserProfileOperationCompleted(object arg)
        {
            if (this.SetUserProfileCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetUserProfileCompleted(this,
                    new SetUserProfileCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetWelcomePageOperationCompleted(object arg)
        {
            if (this.SetWelcomePageCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetWelcomePageCompleted(this,
                    new SetWelcomePageCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnStoragePointAvailableOperationCompleted(object arg)
        {
            if (this.StoragePointAvailableCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.StoragePointAvailableCompleted(this,
                    new StoragePointAvailableCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnStoragePointProfileConfiguredOperationCompleted(object arg)
        {
            if (this.StoragePointProfileConfiguredCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.StoragePointProfileConfiguredCompleted(this,
                    new StoragePointProfileConfiguredCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateDocumentChunkedOperationCompleted(object arg)
        {
            if (this.UpdateDocumentChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateDocumentChunkedCompleted(this,
                    new UpdateDocumentChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateDocumentOperationCompleted(object arg)
        {
            if (this.UpdateDocumentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateDocumentCompleted(this,
                    new UpdateDocumentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateGroupQuickLaunchOperationCompleted(object arg)
        {
            if (this.UpdateGroupQuickLaunchCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateGroupQuickLaunchCompleted(this,
                    new UpdateGroupQuickLaunchCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateListItemChunkedOperationCompleted(object arg)
        {
            if (this.UpdateListItemChunkedCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateListItemChunkedCompleted(this,
                    new UpdateListItemChunkedCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateListItemOperationCompleted(object arg)
        {
            if (this.UpdateListItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateListItemCompleted(this,
                    new UpdateListItemCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateListItemStatusOperationCompleted(object arg)
        {
            if (this.UpdateListItemStatusCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateListItemStatusCompleted(this,
                    new UpdateListItemStatusCompletedEventArgs(invokeCompletedEventArg.Results,
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

        private void OnUpdateSiteCollectionSettingsOperationCompleted(object arg)
        {
            if (this.UpdateSiteCollectionSettingsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateSiteCollectionSettingsCompleted(this,
                    new UpdateSiteCollectionSettingsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateWebNavigationStructureOperationCompleted(object arg)
        {
            if (this.UpdateWebNavigationStructureCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateWebNavigationStructureCompleted(this,
                    new UpdateWebNavigationStructureCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateWebOperationCompleted(object arg)
        {
            if (this.UpdateWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateWebCompleted(this,
                    new UpdateWebCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnValidateUserInfoOperationCompleted(object arg)
        {
            if (this.ValidateUserInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ValidateUserInfoCompleted(this,
                    new ValidateUserInfoCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnWriteChunkOperationCompleted(object arg)
        {
            if (this.WriteChunkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.WriteChunkCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://www.metalogix.net/OpenFileCopySession",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Guid OpenFileCopySession(
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            object[] objArray = new object[] { streamType, iRetentionTime };
            return (Guid)base.Invoke("OpenFileCopySession", objArray)[0];
        }

        public void OpenFileCopySessionAsync(
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime)
        {
            this.OpenFileCopySessionAsync(streamType, iRetentionTime, null);
        }

        public void OpenFileCopySessionAsync(
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType, int iRetentionTime,
            object userState)
        {
            if (this.OpenFileCopySessionOperationCompleted == null)
            {
                this.OpenFileCopySessionOperationCompleted =
                    new SendOrPostCallback(this.OnOpenFileCopySessionOperationCompleted);
            }

            object[] objArray = new object[] { streamType, iRetentionTime };
            base.InvokeAsync("OpenFileCopySession", objArray, this.OpenFileCopySessionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ReadChunk", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(DataType = "base64Binary")]
        public byte[] ReadChunk(Guid sessionId, long bytesToRead)
        {
            object[] objArray = new object[] { sessionId, bytesToRead };
            return (byte[])base.Invoke("ReadChunk", objArray)[0];
        }

        public void ReadChunkAsync(Guid sessionId, long bytesToRead)
        {
            this.ReadChunkAsync(sessionId, bytesToRead, null);
        }

        public void ReadChunkAsync(Guid sessionId, long bytesToRead, object userState)
        {
            if (this.ReadChunkOperationCompleted == null)
            {
                this.ReadChunkOperationCompleted = new SendOrPostCallback(this.OnReadChunkOperationCompleted);
            }

            object[] objArray = new object[] { sessionId, bytesToRead };
            base.InvokeAsync("ReadChunk", objArray, this.ReadChunkOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ReorderContentTypes",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ReorderContentTypes(string sListId, string[] sContentTypeNames)
        {
            object[] objArray = new object[] { sListId, sContentTypeNames };
            return (string)base.Invoke("ReorderContentTypes", objArray)[0];
        }

        public void ReorderContentTypesAsync(string sListId, string[] sContentTypeNames)
        {
            this.ReorderContentTypesAsync(sListId, sContentTypeNames, null);
        }

        public void ReorderContentTypesAsync(string sListId, string[] sContentTypeNames, object userState)
        {
            if (this.ReorderContentTypesOperationCompleted == null)
            {
                this.ReorderContentTypesOperationCompleted =
                    new SendOrPostCallback(this.OnReorderContentTypesOperationCompleted);
            }

            object[] objArray = new object[] { sListId, sContentTypeNames };
            base.InvokeAsync("ReorderContentTypes", objArray, this.ReorderContentTypesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/SearchForDocument",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            object[] objArray = new object[] { sSearchTerm, sOptionsXml };
            return (string)base.Invoke("SearchForDocument", objArray)[0];
        }

        public void SearchForDocumentAsync(string sSearchTerm, string sOptionsXml)
        {
            this.SearchForDocumentAsync(sSearchTerm, sOptionsXml, null);
        }

        public void SearchForDocumentAsync(string sSearchTerm, string sOptionsXml, object userState)
        {
            if (this.SearchForDocumentOperationCompleted == null)
            {
                this.SearchForDocumentOperationCompleted =
                    new SendOrPostCallback(this.OnSearchForDocumentOperationCompleted);
            }

            object[] objArray = new object[] { sSearchTerm, sOptionsXml };
            base.InvokeAsync("SearchForDocument", objArray, this.SearchForDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/SetDocumentParsing",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string SetDocumentParsing(bool bParserEnabled)
        {
            object[] objArray = new object[] { bParserEnabled };
            return (string)base.Invoke("SetDocumentParsing", objArray)[0];
        }

        public void SetDocumentParsingAsync(bool bParserEnabled)
        {
            this.SetDocumentParsingAsync(bParserEnabled, null);
        }

        public void SetDocumentParsingAsync(bool bParserEnabled, object userState)
        {
            if (this.SetDocumentParsingOperationCompleted == null)
            {
                this.SetDocumentParsingOperationCompleted =
                    new SendOrPostCallback(this.OnSetDocumentParsingOperationCompleted);
            }

            object[] objArray = new object[] { bParserEnabled };
            base.InvokeAsync("SetDocumentParsing", objArray, this.SetDocumentParsingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/SetMasterPage", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string SetMasterPage(string sWebXML)
        {
            object[] objArray = new object[] { sWebXML };
            return (string)base.Invoke("SetMasterPage", objArray)[0];
        }

        public void SetMasterPageAsync(string sWebXML)
        {
            this.SetMasterPageAsync(sWebXML, null);
        }

        public void SetMasterPageAsync(string sWebXML, object userState)
        {
            if (this.SetMasterPageOperationCompleted == null)
            {
                this.SetMasterPageOperationCompleted = new SendOrPostCallback(this.OnSetMasterPageOperationCompleted);
            }

            object[] objArray = new object[] { sWebXML };
            base.InvokeAsync("SetMasterPage", objArray, this.SetMasterPageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/SetUserProfile", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            object[] objArray = new object[] { sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound };
            return (string)base.Invoke("SetUserProfile", objArray)[0];
        }

        public void SetUserProfileAsync(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            this.SetUserProfileAsync(sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound, null);
        }

        public void SetUserProfileAsync(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound,
            object userState)
        {
            if (this.SetUserProfileOperationCompleted == null)
            {
                this.SetUserProfileOperationCompleted = new SendOrPostCallback(this.OnSetUserProfileOperationCompleted);
            }

            object[] objArray = new object[] { sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound };
            base.InvokeAsync("SetUserProfile", objArray, this.SetUserProfileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/SetWelcomePage", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string SetWelcomePage(string WelcomePage)
        {
            object[] welcomePage = new object[] { WelcomePage };
            return (string)base.Invoke("SetWelcomePage", welcomePage)[0];
        }

        public void SetWelcomePageAsync(string WelcomePage)
        {
            this.SetWelcomePageAsync(WelcomePage, null);
        }

        public void SetWelcomePageAsync(string WelcomePage, object userState)
        {
            if (this.SetWelcomePageOperationCompleted == null)
            {
                this.SetWelcomePageOperationCompleted = new SendOrPostCallback(this.OnSetWelcomePageOperationCompleted);
            }

            object[] welcomePage = new object[] { WelcomePage };
            base.InvokeAsync("SetWelcomePage", welcomePage, this.SetWelcomePageOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/StoragePointAvailable",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string StoragePointAvailable(string inputXml)
        {
            object[] objArray = new object[] { inputXml };
            return (string)base.Invoke("StoragePointAvailable", objArray)[0];
        }

        public void StoragePointAvailableAsync(string inputXml)
        {
            this.StoragePointAvailableAsync(inputXml, null);
        }

        public void StoragePointAvailableAsync(string inputXml, object userState)
        {
            if (this.StoragePointAvailableOperationCompleted == null)
            {
                this.StoragePointAvailableOperationCompleted =
                    new SendOrPostCallback(this.OnStoragePointAvailableOperationCompleted);
            }

            object[] objArray = new object[] { inputXml };
            base.InvokeAsync("StoragePointAvailable", objArray, this.StoragePointAvailableOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/StoragePointProfileConfigured",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            object[] objArray = new object[] { sSharePointPath };
            return (string)base.Invoke("StoragePointProfileConfigured", objArray)[0];
        }

        public void StoragePointProfileConfiguredAsync(string sSharePointPath)
        {
            this.StoragePointProfileConfiguredAsync(sSharePointPath, null);
        }

        public void StoragePointProfileConfiguredAsync(string sSharePointPath, object userState)
        {
            if (this.StoragePointProfileConfiguredOperationCompleted == null)
            {
                this.StoragePointProfileConfiguredOperationCompleted =
                    new SendOrPostCallback(this.OnStoragePointProfileConfiguredOperationCompleted);
            }

            object[] objArray = new object[] { sSharePointPath };
            base.InvokeAsync("StoragePointProfileConfigured", objArray,
                this.StoragePointProfileConfiguredOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateDocument", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            [XmlElement(DataType = "base64Binary")] byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, Options };
            return (string)base.Invoke("UpdateDocument", objArray)[0];
        }

        public void UpdateDocumentAsync(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options)
        {
            this.UpdateDocumentAsync(sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, Options, null);
        }

        public void UpdateDocumentAsync(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options,
            object userState)
        {
            if (this.UpdateDocumentOperationCompleted == null)
            {
                this.UpdateDocumentOperationCompleted = new SendOrPostCallback(this.OnUpdateDocumentOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, Options };
            base.InvokeAsync("UpdateDocument", objArray, this.UpdateDocumentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateDocumentChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateDocumentChunked(string sListID, string sParentFolder, string sFileLeafRef,
            string sListItemXML, [XmlElement(DataType = "base64Binary")] byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, isContentMoved, Options };
            return (string)base.Invoke("UpdateDocumentChunked", objArray)[0];
        }

        public void UpdateDocumentChunkedAsync(string sListID, string sParentFolder, string sFileLeafRef,
            string sListItemXML, byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options)
        {
            this.UpdateDocumentChunkedAsync(sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents,
                isContentMoved, Options, null);
        }

        public void UpdateDocumentChunkedAsync(string sListID, string sParentFolder, string sFileLeafRef,
            string sListItemXML, byte[] fileContents, bool isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions Options,
            object userState)
        {
            if (this.UpdateDocumentChunkedOperationCompleted == null)
            {
                this.UpdateDocumentChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateDocumentChunkedOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, isContentMoved, Options };
            base.InvokeAsync("UpdateDocumentChunked", objArray, this.UpdateDocumentChunkedOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateGroupQuickLaunch",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateGroupQuickLaunch(string sGroupVals)
        {
            object[] objArray = new object[] { sGroupVals };
            return (string)base.Invoke("UpdateGroupQuickLaunch", objArray)[0];
        }

        public void UpdateGroupQuickLaunchAsync(string sGroupVals)
        {
            this.UpdateGroupQuickLaunchAsync(sGroupVals, null);
        }

        public void UpdateGroupQuickLaunchAsync(string sGroupVals, object userState)
        {
            if (this.UpdateGroupQuickLaunchOperationCompleted == null)
            {
                this.UpdateGroupQuickLaunchOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateGroupQuickLaunchOperationCompleted);
            }

            object[] objArray = new object[] { sGroupVals };
            base.InvokeAsync("UpdateGroupQuickLaunch", objArray, this.UpdateGroupQuickLaunchOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateList", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateList(string sListID, string sListXML, string sViewXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListOptions Options,
            [XmlElement(DataType = "base64Binary")] byte[] documentTemplateFile)
        {
            object[] objArray = new object[] { sListID, sListXML, sViewXml, Options, documentTemplateFile };
            return (string)base.Invoke("UpdateList", objArray)[0];
        }

        public void UpdateListAsync(string sListID, string sListXML, string sViewXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListOptions Options,
            byte[] documentTemplateFile)
        {
            this.UpdateListAsync(sListID, sListXML, sViewXml, Options, documentTemplateFile, null);
        }

        public void UpdateListAsync(string sListID, string sListXML, string sViewXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListOptions Options,
            byte[] documentTemplateFile, object userState)
        {
            if (this.UpdateListOperationCompleted == null)
            {
                this.UpdateListOperationCompleted = new SendOrPostCallback(this.OnUpdateListOperationCompleted);
            }

            object[] objArray = new object[] { sListID, sListXML, sViewXml, Options, documentTemplateFile };
            base.InvokeAsync("UpdateList", objArray, this.UpdateListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateListItem", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options)
        {
            object[] objArray = new object[]
                { sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents, Options };
            return (string)base.Invoke("UpdateListItem", objArray)[0];
        }

        public void UpdateListItemAsync(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options)
        {
            this.UpdateListItemAsync(sListID, sParentFolder, iItemID, sListItemXML, attachementNames,
                attachmentContents, Options, null);
        }

        public void UpdateListItemAsync(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options,
            object userState)
        {
            if (this.UpdateListItemOperationCompleted == null)
            {
                this.UpdateListItemOperationCompleted = new SendOrPostCallback(this.OnUpdateListItemOperationCompleted);
            }

            object[] objArray = new object[]
                { sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents, Options };
            base.InvokeAsync("UpdateListItem", objArray, this.UpdateListItemOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateListItemChunked",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateListItemChunked(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options)
        {
            object[] objArray = new object[]
            {
                sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents, isContentMoved,
                Options
            };
            return (string)base.Invoke("UpdateListItemChunked", objArray)[0];
        }

        public void UpdateListItemChunkedAsync(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options)
        {
            this.UpdateListItemChunkedAsync(sListID, sParentFolder, iItemID, sListItemXML, attachementNames,
                attachmentContents, isContentMoved, Options, null);
        }

        public void UpdateListItemChunkedAsync(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, bool[] isContentMoved,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions Options,
            object userState)
        {
            if (this.UpdateListItemChunkedOperationCompleted == null)
            {
                this.UpdateListItemChunkedOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateListItemChunkedOperationCompleted);
            }

            object[] objArray = new object[]
            {
                sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents, isContentMoved,
                Options
            };
            base.InvokeAsync("UpdateListItemChunked", objArray, this.UpdateListItemChunkedOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateListItemStatus",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            object[] objArray = new object[]
            {
                bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemID, sCheckinComment, sPublishComment,
                sApprovalComment
            };
            return (string)base.Invoke("UpdateListItemStatus", objArray)[0];
        }

        public void UpdateListItemStatusAsync(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            this.UpdateListItemStatusAsync(bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemID, sCheckinComment,
                sPublishComment, sApprovalComment, null);
        }

        public void UpdateListItemStatusAsync(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment,
            object userState)
        {
            if (this.UpdateListItemStatusOperationCompleted == null)
            {
                this.UpdateListItemStatusOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateListItemStatusOperationCompleted);
            }

            object[] objArray = new object[]
            {
                bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemID, sCheckinComment, sPublishComment,
                sApprovalComment
            };
            base.InvokeAsync("UpdateListItemStatus", objArray, this.UpdateListItemStatusOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateSiteCollectionSettings",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateSiteCollectionSettings(string sUpdateXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateSiteCollectionOptions
                updateSiteCollectionOptions)
        {
            object[] objArray = new object[] { sUpdateXml, updateSiteCollectionOptions };
            return (string)base.Invoke("UpdateSiteCollectionSettings", objArray)[0];
        }

        public void UpdateSiteCollectionSettingsAsync(string sUpdateXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateSiteCollectionOptions
                updateSiteCollectionOptions)
        {
            this.UpdateSiteCollectionSettingsAsync(sUpdateXml, updateSiteCollectionOptions, null);
        }

        public void UpdateSiteCollectionSettingsAsync(string sUpdateXml,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateSiteCollectionOptions
                updateSiteCollectionOptions, object userState)
        {
            if (this.UpdateSiteCollectionSettingsOperationCompleted == null)
            {
                this.UpdateSiteCollectionSettingsOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateSiteCollectionSettingsOperationCompleted);
            }

            object[] objArray = new object[] { sUpdateXml, updateSiteCollectionOptions };
            base.InvokeAsync("UpdateSiteCollectionSettings", objArray,
                this.UpdateSiteCollectionSettingsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateWeb", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateWeb(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateWebOptions updateOptions)
        {
            object[] objArray = new object[] { sWebXML, updateOptions };
            return (string)base.Invoke("UpdateWeb", objArray)[0];
        }

        public void UpdateWebAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateWebOptions updateOptions)
        {
            this.UpdateWebAsync(sWebXML, updateOptions, null);
        }

        public void UpdateWebAsync(string sWebXML,
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateWebOptions updateOptions,
            object userState)
        {
            if (this.UpdateWebOperationCompleted == null)
            {
                this.UpdateWebOperationCompleted = new SendOrPostCallback(this.OnUpdateWebOperationCompleted);
            }

            object[] objArray = new object[] { sWebXML, updateOptions };
            base.InvokeAsync("UpdateWeb", objArray, this.UpdateWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/UpdateWebNavigationStructure",
            RequestNamespace = "http://www.metalogix.net/", ResponseNamespace = "http://www.metalogix.net/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            object[] objArray = new object[] { sUpdateXml };
            return (string)base.Invoke("UpdateWebNavigationStructure", objArray)[0];
        }

        public void UpdateWebNavigationStructureAsync(string sUpdateXml)
        {
            this.UpdateWebNavigationStructureAsync(sUpdateXml, null);
        }

        public void UpdateWebNavigationStructureAsync(string sUpdateXml, object userState)
        {
            if (this.UpdateWebNavigationStructureOperationCompleted == null)
            {
                this.UpdateWebNavigationStructureOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateWebNavigationStructureOperationCompleted);
            }

            object[] objArray = new object[] { sUpdateXml };
            base.InvokeAsync("UpdateWebNavigationStructure", objArray,
                this.UpdateWebNavigationStructureOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/ValidateUserInfo", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            object[] objArray = new object[] { sUserIdentifier, bCanBeDomainGroup };
            return (string)base.Invoke("ValidateUserInfo", objArray)[0];
        }

        public void ValidateUserInfoAsync(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            this.ValidateUserInfoAsync(sUserIdentifier, bCanBeDomainGroup, null);
        }

        public void ValidateUserInfoAsync(string sUserIdentifier, bool bCanBeDomainGroup, object userState)
        {
            if (this.ValidateUserInfoOperationCompleted == null)
            {
                this.ValidateUserInfoOperationCompleted =
                    new SendOrPostCallback(this.OnValidateUserInfoOperationCompleted);
            }

            object[] objArray = new object[] { sUserIdentifier, bCanBeDomainGroup };
            base.InvokeAsync("ValidateUserInfo", objArray, this.ValidateUserInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.net/WriteChunk", RequestNamespace = "http://www.metalogix.net/",
            ResponseNamespace = "http://www.metalogix.net/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void WriteChunk(Guid sessionId, [XmlElement(DataType = "base64Binary")] byte[] data)
        {
            object[] objArray = new object[] { sessionId, data };
            base.Invoke("WriteChunk", objArray);
        }

        public void WriteChunkAsync(Guid sessionId, byte[] data)
        {
            this.WriteChunkAsync(sessionId, data, null);
        }

        public void WriteChunkAsync(Guid sessionId, byte[] data, object userState)
        {
            if (this.WriteChunkOperationCompleted == null)
            {
                this.WriteChunkOperationCompleted = new SendOrPostCallback(this.OnWriteChunkOperationCompleted);
            }

            object[] objArray = new object[] { sessionId, data };
            base.InvokeAsync("WriteChunk", objArray, this.WriteChunkOperationCompleted, userState);
        }

        public event ActivateReusableWorkflowTemplatesCompletedEventHandler ActivateReusableWorkflowTemplatesCompleted;

        public event AddAlertsCompletedEventHandler AddAlertsCompleted;

        public event AddDocumentChunkedCompletedEventHandler AddDocumentChunkedCompleted;

        public event AddDocumentCompletedEventHandler AddDocumentCompleted;

        public event AddDocumentOptimisticallyCompletedEventHandler AddDocumentOptimisticallyCompleted;

        public event AddDocumentTemplatetoContentTypeCompletedEventHandler AddDocumentTemplatetoContentTypeCompleted;

        public event AddFieldsCompletedEventHandler AddFieldsCompleted;

        public event AddFileToFolderCompletedEventHandler AddFileToFolderCompleted;

        public event AddFolderCompletedEventHandler AddFolderCompleted;

        public event AddFolderOptimisticallyCompletedEventHandler AddFolderOptimisticallyCompleted;

        public event AddFolderToFolderCompletedEventHandler AddFolderToFolderCompleted;

        public event AddFormTemplateToContentTypeCompletedEventHandler AddFormTemplateToContentTypeCompleted;

        public event AddListCompletedEventHandler AddListCompleted;

        public event AddListItemChunkedCompletedEventHandler AddListItemChunkedCompleted;

        public event AddListItemCompletedEventHandler AddListItemCompleted;

        public event AddOrUpdateAudienceCompletedEventHandler AddOrUpdateAudienceCompleted;

        public event AddOrUpdateContentTypeCompletedEventHandler AddOrUpdateContentTypeCompleted;

        public event AddOrUpdateGroupCompletedEventHandler AddOrUpdateGroupCompleted;

        public event AddOrUpdateRoleCompletedEventHandler AddOrUpdateRoleCompleted;

        public event AddReferencedTaxonomyDataCompletedEventHandler AddReferencedTaxonomyDataCompleted;

        public event AddReusedTermsCompletedEventHandler AddReusedTermsCompleted;

        public event AddRoleAssignmentCompletedEventHandler AddRoleAssignmentCompleted;

        public event AddSiteCollectionCompletedEventHandler AddSiteCollectionCompleted;

        public event AddSiteUserCompletedEventHandler AddSiteUserCompleted;

        public event AddTermCompletedEventHandler AddTermCompleted;

        public event AddTermGroupCompletedEventHandler AddTermGroupCompleted;

        public event AddTermSetCompletedEventHandler AddTermSetCompleted;

        public event AddTermstoreLanguagesCompletedEventHandler AddTermstoreLanguagesCompleted;

        public event AddViewCompletedEventHandler AddViewCompleted;

        public event AddWebCompletedEventHandler AddWebCompleted;

        public event AddWebPartsCompletedEventHandler AddWebPartsCompleted;

        public event AddWorkflowAssociationCompletedEventHandler AddWorkflowAssociationCompleted;

        public event AddWorkflowCompletedEventHandler AddWorkflowCompleted;

        public event AnalyzeChurnCompletedEventHandler AnalyzeChurnCompleted;

        public event Apply2013ThemeCompletedEventHandler Apply2013ThemeCompleted;

        public event ApplyOrUpdateContentTypeCompletedEventHandler ApplyOrUpdateContentTypeCompleted;

        public event BeginCompilingAllAudiencesCompletedEventHandler BeginCompilingAllAudiencesCompleted;

        public event CatalogDocumentToStoragePointFileShareEndpointCompletedEventHandler
            CatalogDocumentToStoragePointFileShareEndpointCompleted;

        public event CloseFileCopySessionCompletedEventHandler CloseFileCopySessionCompleted;

        public event CloseWebPartsCompletedEventHandler CloseWebPartsCompleted;

        public event ConfigureStoragePointFileShareEndpointAndProfileCompletedEventHandler
            ConfigureStoragePointFileShareEndpointAndProfileCompleted;

        public event CorrectDefaultPageVersionsCompletedEventHandler CorrectDefaultPageVersionsCompleted;

        public event DeleteAllAudiencesCompletedEventHandler DeleteAllAudiencesCompleted;

        public event DeleteAudienceCompletedEventHandler DeleteAudienceCompleted;

        public event DeleteContentTypesCompletedEventHandler DeleteContentTypesCompleted;

        public event DeleteFolderCompletedEventHandler DeleteFolderCompleted;

        public event DeleteGroupCompletedEventHandler DeleteGroupCompleted;

        public event DeleteItemCompletedEventHandler DeleteItemCompleted;

        public event DeleteItemsCompletedEventHandler DeleteItemsCompleted;

        public event DeleteListCompletedEventHandler DeleteListCompleted;

        public event DeleteRoleAssignmentCompletedEventHandler DeleteRoleAssignmentCompleted;

        public event DeleteRoleCompletedEventHandler DeleteRoleCompleted;

        public event DeleteSiteCollectionCompletedEventHandler DeleteSiteCollectionCompleted;

        public event DeleteSP2013WorkflowsCompletedEventHandler DeleteSP2013WorkflowsCompleted;

        public event DeleteWebCompletedEventHandler DeleteWebCompleted;

        public event DeleteWebPartCompletedEventHandler DeleteWebPartCompleted;

        public event DeleteWebPartsCompletedEventHandler DeleteWebPartsCompleted;

        public event ExecuteCommandCompletedEventHandler ExecuteCommandCompleted;

        public event FindAlertsCompletedEventHandler FindAlertsCompleted;

        public event FindUniquePermissionsCompletedEventHandler FindUniquePermissionsCompleted;

        public event GetAddInsCompletedEventHandler GetAddInsCompleted;

        public event GetAdImportDcMappingsCompletedEventHandler GetAdImportDcMappingsCompleted;

        public event GetAlertsCompletedEventHandler GetAlertsCompleted;

        public event GetAttachmentsCompletedEventHandler GetAttachmentsCompleted;

        public event GetAudiencesCompletedEventHandler GetAudiencesCompleted;

        public event GetBcsApplicationsCompletedEventHandler GetBcsApplicationsCompleted;

        public event GetBrowserFileHandlingCompletedEventHandler GetBrowserFileHandlingCompleted;

        public event GetContentTypesCompletedEventHandler GetContentTypesCompleted;

        public event GetCustomProfilePropertyMappingCompletedEventHandler GetCustomProfilePropertyMappingCompleted;

        public event GetDashboardPageTemplateCompletedEventHandler GetDashboardPageTemplateCompleted;

        public event GetDocumentBlobRefCompletedEventHandler GetDocumentBlobRefCompleted;

        public event GetDocumentChunkedCompletedEventHandler GetDocumentChunkedCompleted;

        public event GetDocumentCompletedEventHandler GetDocumentCompleted;

        public event GetDocumentIdCompletedEventHandler GetDocumentIdCompleted;

        public event GetDocumentVersionBlobRefCompletedEventHandler GetDocumentVersionBlobRefCompleted;

        public event GetDocumentVersionChunkedCompletedEventHandler GetDocumentVersionChunkedCompleted;

        public event GetDocumentVersionCompletedEventHandler GetDocumentVersionCompleted;

        public event GetExternalContentTypeOperationsCompletedEventHandler GetExternalContentTypeOperationsCompleted;

        public event GetExternalContentTypesCompletedEventHandler GetExternalContentTypesCompleted;

        public event GetExternalItemsCompletedEventHandler GetExternalItemsCompleted;

        public event GetFarmSandboxSolutionsCompletedEventHandler GetFarmSandboxSolutionsCompleted;

        public event GetFarmServerDetailsCompletedEventHandler GetFarmServerDetailsCompleted;

        public event GetFarmSolutionBinaryCompletedEventHandler GetFarmSolutionBinaryCompleted;

        public event GetFarmSolutionsCompletedEventHandler GetFarmSolutionsCompleted;

        public event GetFieldsCompletedEventHandler GetFieldsCompleted;

        public event GetFilesCompletedEventHandler GetFilesCompleted;

        public event GetFileVersionsCompletedEventHandler GetFileVersionsCompleted;

        public event GetFoldersCompletedEventHandler GetFoldersCompleted;

        public event GetGroupsCompletedEventHandler GetGroupsCompleted;

        public event GetInfopathsCompletedEventHandler GetInfopathsCompleted;

        public event GetLanguagesAndWebTemplatesCompletedEventHandler GetLanguagesAndWebTemplatesCompleted;

        public event GetListCompletedEventHandler GetListCompleted;

        public event GetListItemIDsCompletedEventHandler GetListItemIDsCompleted;

        public event GetListItemsByQueryCompletedEventHandler GetListItemsByQueryCompleted;

        public event GetListItemsCompletedEventHandler GetListItemsCompleted;

        public event GetListItemVersionsCompletedEventHandler GetListItemVersionsCompleted;

        public event GetListsCompletedEventHandler GetListsCompleted;

        public event GetListTemplatesCompletedEventHandler GetListTemplatesCompleted;

        public event GetListWorkflowRunning2010CompletedEventHandler GetListWorkflowRunning2010Completed;

        public event GetLockedSitesCompletedEventHandler GetLockedSitesCompleted;

        public event GetMySiteDataCompletedEventHandler GetMySiteDataCompleted;

        public event GetPortalListingGroupsCompletedEventHandler GetPortalListingGroupsCompleted;

        public event GetPortalListingIDsCompletedEventHandler GetPortalListingIDsCompleted;

        public event GetPortalListingsCompletedEventHandler GetPortalListingsCompleted;

        public event GetReferencedTaxonomyFullXmlCompletedEventHandler GetReferencedTaxonomyFullXmlCompleted;

        public event GetRoleAssignmentsCompletedEventHandler GetRoleAssignmentsCompleted;

        public event GetRolesCompletedEventHandler GetRolesCompleted;

        public event GetSecureStorageApplicationsCompletedEventHandler GetSecureStorageApplicationsCompleted;

        public event GetSharePointVersionCompletedEventHandler GetSharePointVersionCompleted;

        public event GetSiteCollectionsCompletedEventHandler GetSiteCollectionsCompleted;

        public event GetSiteCollectionsOnWebAppCompletedEventHandler GetSiteCollectionsOnWebAppCompleted;

        public event GetSiteCompletedEventHandler GetSiteCompleted;

        public event GetSiteQuotaTemplatesCompletedEventHandler GetSiteQuotaTemplatesCompleted;

        public event GetSiteSolutionsBinaryCompletedEventHandler GetSiteSolutionsBinaryCompleted;

        public event GetSiteUsersCompletedEventHandler GetSiteUsersCompleted;

        public event GetSP2013WorkflowsCompletedEventHandler GetSP2013WorkflowsCompleted;

        public event GetStoragePointProfileConfigurationCompletedEventHandler
            GetStoragePointProfileConfigurationCompleted;

        public event GetSubWebsCompletedEventHandler GetSubWebsCompleted;

        public event GetSystemInfoCompletedEventHandler GetSystemInfoCompleted;

        public event GetTermCollectionFromTermCompletedEventHandler GetTermCollectionFromTermCompleted;

        public event GetTermCollectionFromTermSetCompletedEventHandler GetTermCollectionFromTermSetCompleted;

        public event GetTermGroupsCompletedEventHandler GetTermGroupsCompleted;

        public event GetTermSetCollectionCompletedEventHandler GetTermSetCollectionCompleted;

        public event GetTermSetsCompletedEventHandler GetTermSetsCompleted;

        public event GetTermsFromTermSetCompletedEventHandler GetTermsFromTermSetCompleted;

        public event GetTermsFromTermSetItemCompletedEventHandler GetTermsFromTermSetItemCompleted;

        public event GetTermStoresCompletedEventHandler GetTermStoresCompleted;

        public event GetUserFromProfileCompletedEventHandler GetUserFromProfileCompleted;

        public event GetUserProfilePropertiesUsageCompletedEventHandler GetUserProfilePropertiesUsageCompleted;

        public event GetUserProfilesCompletedEventHandler GetUserProfilesCompleted;

        public event GetWebApplicationPoliciesCompletedEventHandler GetWebApplicationPoliciesCompleted;

        public event GetWebApplicationsCompletedEventHandler GetWebApplicationsCompleted;

        public event GetWebCompletedEventHandler GetWebCompleted;

        public event GetWebNavigationSettingsCompletedEventHandler GetWebNavigationSettingsCompleted;

        public event GetWebNavigationStructureCompletedEventHandler GetWebNavigationStructureCompleted;

        public event GetWebPartPageCompletedEventHandler GetWebPartPageCompleted;

        public event GetWebPartPageTemplateCompletedEventHandler GetWebPartPageTemplateCompleted;

        public event GetWebPartsOnPageCompletedEventHandler GetWebPartsOnPageCompleted;

        public event GetWebTemplatesCompletedEventHandler GetWebTemplatesCompleted;

        public event GetWorkflowAssociation2010CompletedEventHandler GetWorkflowAssociation2010Completed;

        public event GetWorkflowAssociation2013CompletedEventHandler GetWorkflowAssociation2013Completed;

        public event GetWorkflowAssociationsCompletedEventHandler GetWorkflowAssociationsCompleted;

        public event GetWorkflowRunning2010CompletedEventHandler GetWorkflowRunning2010Completed;

        public event GetWorkflowRunning2013CompletedEventHandler GetWorkflowRunning2013Completed;

        public event GetWorkflowsCompletedEventHandler GetWorkflowsCompleted;

        public event HasDocumentCompletedEventHandler HasDocumentCompleted;

        public event HasUniquePermissionsCompletedEventHandler HasUniquePermissionsCompleted;

        public event HasWebPartsCompletedEventHandler HasWebPartsCompleted;

        public event HasWorkflowsCompletedEventHandler HasWorkflowsCompleted;

        public event IsAppWebPartOnPageCompletedEventHandler IsAppWebPartOnPageCompleted;

        public event IsListContainsInfoPathOrAspxItemCompletedEventHandler IsListContainsInfoPathOrAspxItemCompleted;

        public event IsWorkflowServicesInstanceAvailableCompletedEventHandler
            IsWorkflowServicesInstanceAvailableCompleted;

        public event MigrateSP2013WorkflowsCompletedEventHandler MigrateSP2013WorkflowsCompleted;

        public event ModifyWebNavigationSettingsCompletedEventHandler ModifyWebNavigationSettingsCompleted;

        public event OpenFileCopySessionCompletedEventHandler OpenFileCopySessionCompleted;

        public event ReadChunkCompletedEventHandler ReadChunkCompleted;

        public event ReorderContentTypesCompletedEventHandler ReorderContentTypesCompleted;

        public event SearchForDocumentCompletedEventHandler SearchForDocumentCompleted;

        public event SetDocumentParsingCompletedEventHandler SetDocumentParsingCompleted;

        public event SetMasterPageCompletedEventHandler SetMasterPageCompleted;

        public event SetUserProfileCompletedEventHandler SetUserProfileCompleted;

        public event SetWelcomePageCompletedEventHandler SetWelcomePageCompleted;

        public event StoragePointAvailableCompletedEventHandler StoragePointAvailableCompleted;

        public event StoragePointProfileConfiguredCompletedEventHandler StoragePointProfileConfiguredCompleted;

        public event UpdateDocumentChunkedCompletedEventHandler UpdateDocumentChunkedCompleted;

        public event UpdateDocumentCompletedEventHandler UpdateDocumentCompleted;

        public event UpdateGroupQuickLaunchCompletedEventHandler UpdateGroupQuickLaunchCompleted;

        public event UpdateListCompletedEventHandler UpdateListCompleted;

        public event UpdateListItemChunkedCompletedEventHandler UpdateListItemChunkedCompleted;

        public event UpdateListItemCompletedEventHandler UpdateListItemCompleted;

        public event UpdateListItemStatusCompletedEventHandler UpdateListItemStatusCompleted;

        public event UpdateSiteCollectionSettingsCompletedEventHandler UpdateSiteCollectionSettingsCompleted;

        public event UpdateWebCompletedEventHandler UpdateWebCompleted;

        public event UpdateWebNavigationStructureCompletedEventHandler UpdateWebNavigationStructureCompleted;

        public event ValidateUserInfoCompletedEventHandler ValidateUserInfoCompleted;

        public event WriteChunkCompletedEventHandler WriteChunkCompleted;
    }
}