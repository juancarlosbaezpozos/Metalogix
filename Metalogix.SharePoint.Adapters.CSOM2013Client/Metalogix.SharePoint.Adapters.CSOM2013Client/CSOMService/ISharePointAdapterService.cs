using Metalogix.SharePoint.Adapters;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "CSOMService.ISharePointAdapterService", SessionMode = SessionMode.Required)]
    public interface ISharePointAdapterService
    {
        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/ActivateReusableWorkflowTemplates",
            ReplyAction = "http://tempuri.org/ISharePointWriter/ActivateReusableWorkflowTemplatesResponse")]
        string ActivateReusableWorkflowTemplates();

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddAlerts",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddAlertsResponse")]
        string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddDocument",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddDocumentResponse")]
        string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddDocumentOptimistically",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddDocumentOptimisticallyResponse")]
        string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddDocumentSetVersions",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddDocumentSetVersionsResponse")]
        string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddDocumentTemplatetoContentType",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddDocumentTemplatetoContentTypeResponse")]
        string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFields",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFieldsResponse")]
        string AddFields(string sListID, string sFieldXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFileToFolder",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFileToFolderResponse")]
        string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFolder",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFolderResponse")]
        string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFolderOptimistically",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFolderOptimisticallyResponse")]
        string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFolderToFolder",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFolderToFolderResponse")]
        string AddFolderToFolder(string sFolderXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddFormTemplateToContentType",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddFormTemplateToContentTypeResponse")]
        string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddList",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddListResponse")]
        string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddListItem",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddListItemResponse")]
        string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddOrUpdateAudience",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddOrUpdateAudienceResponse")]
        string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddOrUpdateContentType",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddOrUpdateContentTypeResponse")]
        string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddOrUpdateGroup",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddOrUpdateGroupResponse")]
        string AddOrUpdateGroup(string sGroupXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddOrUpdateRole",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddOrUpdateRoleResponse")]
        string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddReferencedTaxonomyData",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddReferencedTaxonomyDataResponse")]
        string AddReferencedTaxonomyData(string sReferencedTaxonomyXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddReusedTerms",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddReusedTermsResponse")]
        string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddRoleAssignment",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddRoleAssignmentResponse")]
        string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID, int iItemId);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddSiteCollection",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddSiteCollectionResponse")]
        string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions addSiteCollOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddSiteUser",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddSiteUserResponse")]
        string AddSiteUser(string sUserXML, AddUserOptions options);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddTerm",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddTermResponse")]
        string AddTerm(string termXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddTermGroup",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddTermGroupResponse")]
        string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddTermSet",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddTermSetResponse")]
        string AddTermSet(string termSetXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddTermstoreLanguages",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddTermstoreLanguagesResponse")]
        string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddView",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddViewResponse")]
        string AddView(string sListID, string sViewXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddWeb",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddWebResponse")]
        [ServiceKnownType(typeof(AddSiteCollectionOptions))]
        string AddWeb(string sWebXML, AddWebOptions addOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddWebParts",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddWebPartsResponse")]
        string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl, string sEmbeddedHtmlContent);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddWorkflow",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddWorkflowResponse")]
        string AddWorkflow(string sListId, string sWorkflowXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/AddWorkflowAssociation",
            ReplyAction = "http://tempuri.org/ISharePointWriter/AddWorkflowAssociationResponse")]
        string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/AnalyzeChurn",
            ReplyAction = "http://tempuri.org/ISharePointReader/AnalyzeChurnResponse")]
        string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/Apply2013Theme",
            ReplyAction = "http://tempuri.org/ISharePointWriter/Apply2013ThemeResponse")]
        string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/ApplyOrUpdateContentType",
            ReplyAction = "http://tempuri.org/ISharePointWriter/ApplyOrUpdateContentTypeResponse")]
        string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/BeginCompilingAllAudiences",
            ReplyAction = "http://tempuri.org/ISharePointWriter/BeginCompilingAllAudiencesResponse")]
        string BeginCompilingAllAudiences();

        [OperationContract(
            Action = "http://tempuri.org/ISharePointWriter/CatalogDocumentToStoragePointFileShareEndpoint",
            ReplyAction =
                "http://tempuri.org/ISharePointWriter/CatalogDocumentToStoragePointFileShareEndpointResponse")]
        string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID, string sFolder,
            string sListItemXml, AddDocumentOptions options);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/CheckConnection",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/CheckConnectionResponse")]
        void CheckConnection();

        [OperationContract(Action = "http://tempuri.org/IBinaryTransferHandler/CloseFileCopySession",
            ReplyAction = "http://tempuri.org/IBinaryTransferHandler/CloseFileCopySessionResponse")]
        string CloseFileCopySession(Guid sessionId);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/CloseWebParts",
            ReplyAction = "http://tempuri.org/ISharePointWriter/CloseWebPartsResponse")]
        string CloseWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract(
            Action = "http://tempuri.org/ISharePointWriter/ConfigureStoragePointFileShareEndpointAndProfile",
            ReplyAction =
                "http://tempuri.org/ISharePointWriter/ConfigureStoragePointFileShareEndpointAndProfileResponse")]
        string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/CorrectDefaultPageVersions",
            ReplyAction = "http://tempuri.org/ISharePointWriter/CorrectDefaultPageVersionsResponse")]
        string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteAllAudiences",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteAllAudiencesResponse")]
        string DeleteAllAudiences(string inputXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteAudience",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteAudienceResponse")]
        string DeleteAudience(string sAudienceName);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteContentTypes",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteContentTypesResponse")]
        string DeleteContentTypes(string sListID, string[] contentTypeIDs);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteFolder",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteFolderResponse")]
        string DeleteFolder(string sListID, int iListItemID, string sFolder);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteGroup",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteGroupResponse")]
        string DeleteGroup(string sGroupName);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteItem",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteItemResponse")]
        string DeleteItem(string sListID, int iListItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteItems",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteItemsResponse")]
        string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteList",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteListResponse")]
        string DeleteList(string sListID);

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/DeleteMigrationJob",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/DeleteMigrationJobResponse")]
        string DeleteMigrationJob(string jobConfiguration);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteRole",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteRoleResponse")]
        string DeleteRole(string sRoleName);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteRoleAssignment",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteRoleAssignmentResponse")]
        string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteSiteCollection",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteSiteCollectionResponse")]
        string DeleteSiteCollection(string sSiteURL, string sWebApp);

        [OperationContract(Action = "http://tempuri.org/ISP2013WorkflowAdapter/DeleteSP2013Workflows",
            ReplyAction = "http://tempuri.org/ISP2013WorkflowAdapter/DeleteSP2013WorkflowsResponse")]
        string DeleteSP2013Workflows(string configurationXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteWeb",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteWebResponse")]
        string DeleteWeb(string sServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteWebPart",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteWebPartResponse")]
        string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DeleteWebParts",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DeleteWebPartsResponse")]
        string DeleteWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/DisableValidationSettings",
            ReplyAction = "http://tempuri.org/ISharePointWriter/DisableValidationSettingsResponse")]
        string DisableValidationSettings(string listID);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/EnableValidationSettings",
            ReplyAction = "http://tempuri.org/ISharePointWriter/EnableValidationSettingsResponse")]
        string EnableValidationSettings(string validationNodeFieldsXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/EndAdapterService",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/EndAdapterServiceResponse")]
        void EndAdapterService();

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterCommand/ExecuteCommand",
            ReplyAction = "http://tempuri.org/ISharePointAdapterCommand/ExecuteCommandResponse")]
        string ExecuteCommand(string commandName, string commandConfigurationXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/FindAlerts",
            ReplyAction = "http://tempuri.org/ISharePointReader/FindAlertsResponse")]
        string FindAlerts();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/FindUniquePermissions",
            ReplyAction = "http://tempuri.org/ISharePointReader/FindUniquePermissionsResponse")]
        string FindUniquePermissions();

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/GetAdapterConfiguration",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/GetAdapterConfigurationResponse")]
        string GetAdapterConfiguration();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetAlerts",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetAlertsResponse")]
        string GetAlerts(string sListID, int sItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetAttachments",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetAttachmentsResponse")]
        string GetAttachments(string sListID, int iItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetAudiences",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetAudiencesResponse")]
        string GetAudiences();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetContentTypes",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetContentTypesResponse")]
        string GetContentTypes(string sListId);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerCookies",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerCookiesResponse")]
        IList<Cookie> GetCookieManagerCookies();

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerIsActive",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerIsActiveResponse")]
        bool GetCookieManagerIsActive();

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerLocksCookies",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/GetCookieManagerLocksCookiesResponse")]
        bool GetCookieManagerLocksCookies();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDashboardPageTemplate",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDashboardPageTemplateResponse")]
        byte[] GetDashboardPageTemplate(int iTemplateId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDocument",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDocumentResponse")]
        byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDocumentBlobRef",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDocumentBlobRefResponse")]
        byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDocumentId",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDocumentIdResponse")]
        string GetDocumentId(string sDocUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDocumentVersion",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDocumentVersionResponse")]
        byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetDocumentVersionBlobRef",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetDocumentVersionBlobRefResponse")]
        byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetExternalContentTypeOperations",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetExternalContentTypeOperationsResponse")]
        string GetExternalContentTypeOperations(string sExternalContentTypeNamespace, string sExternalContentTypeName);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetExternalContentTypes",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetExternalContentTypesResponse")]
        string GetExternalContentTypes();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetExternalItems",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetExternalItemsResponse")]
        string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sReadListOperation, string sListID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetFields",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetFieldsResponse")]
        string GetFields(string sListID, bool bGetAllAvailableFields);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetFiles",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetFilesResponse")]
        string GetFiles(string sFolderPath, ListItemQueryType itemTypes);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetFolders",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetFoldersResponse")]
        string GetFolders(string sListID, string sIDs, string sParentFolder);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetGroups",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetGroupsResponse")]
        string GetGroups();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetLanguagesAndWebTemplates",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetLanguagesAndWebTemplatesResponse")]
        string GetLanguagesAndWebTemplates();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetList",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListResponse")]
        string GetList(string sListID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetListItemIDs",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListItemIDsResponse")]
        string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetListItems",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListItemsResponse")]
        string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetListItemsByQuery",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListItemsByQueryResponse")]
        string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetListItemVersions",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListItemVersionsResponse")]
        string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetLists",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListsResponse")]
        string GetLists();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetListTemplates",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetListTemplatesResponse")]
        string GetListTemplates();

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/GetMigrationJobStatus",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/GetMigrationJobStatusResponse")]
        string GetMigrationJobStatus(string jobConfiguration);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetMySiteData",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetMySiteDataResponse")]
        string GetMySiteData(string sSiteURL);

        [OperationContract(Action = "http://tempuri.org/IMySitesConnector/GetPersonalSite",
            ReplyAction = "http://tempuri.org/IMySitesConnector/GetPersonalSiteResponse")]
        string GetPersonalSite(string email);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetPortalListingGroups",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetPortalListingGroupsResponse")]
        string GetPortalListingGroups();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetPortalListingIDs",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetPortalListingIDsResponse")]
        string GetPortalListingIDs();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetPortalListings",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetPortalListingsResponse")]
        string GetPortalListings(string sIDList);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetReferencedTaxonomyFullXml",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetReferencedTaxonomyFullXmlResponse")]
        string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetRoleAssignments",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetRoleAssignmentsResponse")]
        string GetRoleAssignments(string sListID, int iItemId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetRoles",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetRolesResponse")]
        string GetRoles(string sListId);

        [OperationContract(Action = "http://tempuri.org/IServerHealthMonitor/GetServerHealth",
            ReplyAction = "http://tempuri.org/IServerHealthMonitor/GetServerHealthResponse")]
        string GetServerHealth();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSharePointVersion",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSharePointVersionResponse")]
        string GetSharePointVersion();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSite",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSiteResponse")]
        string GetSite(bool bFetchFullXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSiteCollections",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSiteCollectionsResponse")]
        string GetSiteCollections();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSiteCollectionsOnWebApp",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSiteCollectionsOnWebAppResponse")]
        string GetSiteCollectionsOnWebApp(string sWebAppName);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSiteQuotaTemplates",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSiteQuotaTemplatesResponse")]
        string GetSiteQuotaTemplates();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSiteUsers",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSiteUsersResponse")]
        string GetSiteUsers();

        [OperationContract(Action = "http://tempuri.org/ISP2013WorkflowAdapter/GetSP2013Workflows",
            ReplyAction = "http://tempuri.org/ISP2013WorkflowAdapter/GetSP2013WorkflowsResponse")]
        string GetSP2013Workflows(string configurationXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetStoragePointProfileConfiguration",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetStoragePointProfileConfigurationResponse")]
        string GetStoragePointProfileConfiguration(string sSharePointPath);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSubWebs",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSubWebsResponse")]
        string GetSubWebs();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetSystemInfo",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetSystemInfoResponse")]
        string GetSystemInfo();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermCollectionFromTerm",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermCollectionFromTermResponse")]
        string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId, string sTermId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermCollectionFromTermSet",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermCollectionFromTermSetResponse")]
        string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermGroups",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermGroupsResponse")]
        string GetTermGroups(string sTermStoreId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermSetCollection",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermSetCollectionResponse")]
        string GetTermSetCollection(string sTermStoreId, string sTermGroupId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermSets",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermSetsResponse")]
        string GetTermSets(string sTermGroupId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermsFromTermSet",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermsFromTermSetResponse")]
        string GetTermsFromTermSet(string sTermSetId, bool bRecursive);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermsFromTermSetItem",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermsFromTermSetItemResponse")]
        string GetTermsFromTermSetItem(string sTermSetItemId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetTermStores",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetTermStoresResponse")]
        string GetTermStores();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetUserFromProfile",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetUserFromProfileResponse")]
        string GetUserFromProfile();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetUserProfiles",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetUserProfilesResponse")]
        string GetUserProfiles(out string sErrors, string sSiteURL, string sLoginName);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWeb",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebResponse")]
        string GetWeb(bool bFetchFullXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebApplications",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebApplicationsResponse")]
        string GetWebApplications();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebNavigationSettings",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebNavigationSettingsResponse")]
        string GetWebNavigationSettings();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebNavigationStructure",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebNavigationStructureResponse")]
        string GetWebNavigationStructure();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebPartPage",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebPartPageResponse")]
        string GetWebPartPage(string sWebPartPageServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebPartPageTemplate",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebPartPageTemplateResponse")]
        byte[] GetWebPartPageTemplate(int iTemplateId);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebPartsOnPage",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebPartsOnPageResponse")]
        string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWebTemplates",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWebTemplatesResponse")]
        string GetWebTemplates();

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWorkflowAssociations",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWorkflowAssociationsResponse")]
        string GetWorkflowAssociations(string sObjectID, string sObjectScope);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/GetWorkflows",
            ReplyAction = "http://tempuri.org/ISharePointReader/GetWorkflowsResponse")]
        string GetWorkflows(string sListID, int iItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/HasDocument",
            ReplyAction = "http://tempuri.org/ISharePointReader/HasDocumentResponse")]
        string HasDocument(string sDocumentServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/IMySitesConnector/HasPersonalSite",
            ReplyAction = "http://tempuri.org/IMySitesConnector/HasPersonalSiteResponse")]
        bool HasPersonalSite(string email);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/HasUniquePermissions",
            ReplyAction = "http://tempuri.org/ISharePointReader/HasUniquePermissionsResponse")]
        string HasUniquePermissions(string listID, int listItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/HasWebParts",
            ReplyAction = "http://tempuri.org/ISharePointReader/HasWebPartsResponse")]
        string HasWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/HasWorkflows",
            ReplyAction = "http://tempuri.org/ISharePointReader/HasWorkflowsResponse")]
        string HasWorkflows(string sListID, string sItemID);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/InitializeAdapterConfiguration",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/InitializeAdapterConfigurationResponse")]
        void InitializeAdapterConfiguration(string sXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/IsListContainsInfoPathOrAspxItem",
            ReplyAction = "http://tempuri.org/ISharePointReader/IsListContainsInfoPathOrAspxItemResponse")]
        string IsListContainsInfoPathOrAspxItem(string listId);

        [OperationContract(Action = "http://tempuri.org/ISP2013WorkflowAdapter/IsWorkflowServicesInstanceAvailable",
            ReplyAction = "http://tempuri.org/ISP2013WorkflowAdapter/IsWorkflowServicesInstanceAvailableResponse")]
        string IsWorkflowServicesInstanceAvailable();

        [OperationContract(Action = "http://tempuri.org/ISP2013WorkflowAdapter/MigrateSP2013Workflows",
            ReplyAction = "http://tempuri.org/ISP2013WorkflowAdapter/MigrateSP2013WorkflowsResponse")]
        string MigrateSP2013Workflows(string configurationXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/ModifyWebNavigationSettings",
            ReplyAction = "http://tempuri.org/ISharePointWriter/ModifyWebNavigationSettingsResponse")]
        string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions);

        [OperationContract(Action = "http://tempuri.org/IBinaryTransferHandler/OpenFileCopySession",
            ReplyAction = "http://tempuri.org/IBinaryTransferHandler/OpenFileCopySessionResponse")]
        Guid OpenFileCopySession(StreamType streamType, int retentionTime);

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/ProvisionMigrationContainer",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/ProvisionMigrationContainerResponse")]
        string ProvisionMigrationContainer();

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/ProvisionMigrationQueue",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/ProvisionMigrationQueueResponse")]
        string ProvisionMigrationQueue();

        [OperationContract(Action = "http://tempuri.org/IMySitesConnector/ProvisionPersonalSites",
            ReplyAction = "http://tempuri.org/IMySitesConnector/ProvisionPersonalSitesResponse")]
        void ProvisionPersonalSites(string[] emails);

        [OperationContract(Action = "http://tempuri.org/IBinaryTransferHandler/ReadChunk",
            ReplyAction = "http://tempuri.org/IBinaryTransferHandler/ReadChunkResponse")]
        byte[] ReadChunk(Guid sessionId, long bytesToRead);

        [OperationContract(Action = "http://tempuri.org/IMySitesConnector/RemovePersonalSite",
            ReplyAction = "http://tempuri.org/IMySitesConnector/RemovePersonalSiteResponse")]
        void RemovePersonalSite(string email);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/ReorderContentTypes",
            ReplyAction = "http://tempuri.org/ISharePointWriter/ReorderContentTypesResponse")]
        string ReorderContentTypes(string sListID, string[] sContentTypes);

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/RequestMigrationJob",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/RequestMigrationJobResponse")]
        string RequestMigrationJob(string jobConfiguration, bool isMicrosoftCustomer, byte[] encryptionKey = null);

        [OperationContract(Action = "http://tempuri.org/IMigrationPipeline/ResolvePrincipals",
            ReplyAction = "http://tempuri.org/IMigrationPipeline/ResolvePrincipalsResponse")]
        string ResolvePrincipals(string principal);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/SearchForDocument",
            ReplyAction = "http://tempuri.org/ISharePointReader/SearchForDocumentResponse")]
        string SearchForDocument(string sSearchTerm, string sOptionsXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/SetCookieManagerCookies",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/SetCookieManagerCookiesResponse")]
        void SetCookieManagerCookies(IList<Cookie> cookies);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/SetDocumentParsing",
            ReplyAction = "http://tempuri.org/ISharePointWriter/SetDocumentParsingResponse")]
        string SetDocumentParsing(bool bParserEnabled);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/SetMasterPage",
            ReplyAction = "http://tempuri.org/ISharePointWriter/SetMasterPageResponse")]
        string SetMasterPage(string sWebXML);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/SetUserProfile",
            ReplyAction = "http://tempuri.org/ISharePointWriter/SetUserProfileResponse")]
        string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/SetWelcomePage",
            ReplyAction = "http://tempuri.org/ISharePointWriter/SetWelcomePageResponse")]
        string SetWelcomePage(string WelcomePage);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/StoragePointAvailable",
            ReplyAction = "http://tempuri.org/ISharePointReader/StoragePointAvailableResponse")]
        string StoragePointAvailable(string inputXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointReader/StoragePointProfileConfigured",
            ReplyAction = "http://tempuri.org/ISharePointReader/StoragePointProfileConfiguredResponse")]
        string StoragePointProfileConfigured(string sSharePointPath);

        [OperationContract(Action = "http://tempuri.org/ISharePointAdapterService/UpdateCookieManagerCookies",
            ReplyAction = "http://tempuri.org/ISharePointAdapterService/UpdateCookieManagerCookiesResponse")]
        void UpdateCookieManagerCookies();

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateDocument",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateDocumentResponse")]
        string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateGroupQuickLaunch",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateGroupQuickLaunchResponse")]
        string UpdateGroupQuickLaunch(string value);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateList",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateListResponse")]
        string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateListItem",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateListItemResponse")]
        string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateListItemStatus",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateListItemStatusResponse")]
        string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML, string sListXML,
            string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateSiteCollectionSettings",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateSiteCollectionSettingsResponse")]
        string UpdateSiteCollectionSettings(string sUpdateXml, UpdateSiteCollectionOptions updateSiteCollectionOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateWeb",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateWebResponse")]
        string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/UpdateWebNavigationStructure",
            ReplyAction = "http://tempuri.org/ISharePointWriter/UpdateWebNavigationStructureResponse")]
        string UpdateWebNavigationStructure(string sUpdateXml);

        [OperationContract(Action = "http://tempuri.org/ISharePointWriter/ValidateUserInfo",
            ReplyAction = "http://tempuri.org/ISharePointWriter/ValidateUserInfoResponse")]
        string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup);

        [OperationContract(Action = "http://tempuri.org/IBinaryTransferHandler/WriteChunk",
            ReplyAction = "http://tempuri.org/IBinaryTransferHandler/WriteChunkResponse")]
        void WriteChunk(Guid sessionId, byte[] data);
    }
}