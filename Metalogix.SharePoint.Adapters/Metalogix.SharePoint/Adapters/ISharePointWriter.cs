using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface ISharePointWriter : ISharePointAdapterCommand
    {
        [OperationContract]
        string ActivateReusableWorkflowTemplates();

        [OperationContract]
        string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML);

        [OperationContract]
        string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options);

        [OperationContract]
        string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache);

        [OperationContract]
        string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo);

        [OperationContract]
        string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url);

        [OperationContract]
        string AddFields(string sListID, string sFieldXML);

        [OperationContract]
        string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options);

        [OperationContract]
        string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options);

        [OperationContract]
        string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache);

        [OperationContract]
        string AddFolderToFolder(string sFolderXML);

        [OperationContract]
        string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields);

        [OperationContract]
        string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile);

        [OperationContract]
        string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options);

        [OperationContract]
        string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options);

        [OperationContract]
        string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName);

        [OperationContract]
        string AddOrUpdateGroup(string sGroupXml);

        [OperationContract]
        string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask);

        [OperationContract]
        string AddReferencedTaxonomyData(string sReferencedTaxonomyXML);

        [OperationContract]
        string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML);

        [OperationContract]
        string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID, int iItemId);

        [OperationContract]
        string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions addSiteCollOptions);

        [OperationContract]
        string AddSiteUser(string sUserXML, AddUserOptions options);

        [OperationContract]
        string AddTerm(string termXml);

        [OperationContract]
        string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult);

        [OperationContract]
        string AddTermSet(string termSetXml);

        [OperationContract]
        string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML);

        [OperationContract]
        string AddView(string sListID, string sViewXML);

        [OperationContract]
        string AddWeb(string sWebXML, AddWebOptions addOptions);

        [OperationContract]
        string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl, string sEmbeddedHtmlContent);

        [OperationContract]
        string AddWorkflow(string sListId, string sWorkflowXml);

        [OperationContract]
        string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting);

        [OperationContract]
        string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl);

        [OperationContract]
        string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType);

        [OperationContract]
        string BeginCompilingAllAudiences();

        [OperationContract]
        string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID, string sFolder,
            string sListItemXml, AddDocumentOptions options);

        [OperationContract]
        string CloseWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract]
        string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath);

        [OperationContract]
        string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML);

        [OperationContract]
        string DeleteAllAudiences(string inputXml);

        [OperationContract]
        string DeleteAudience(string sAudienceName);

        [OperationContract]
        string DeleteContentTypes(string sListID, string[] contentTypeIDs);

        [OperationContract]
        string DeleteFolder(string sListID, int iListItemID, string sFolder);

        [OperationContract]
        string DeleteGroup(string sGroupName);

        [OperationContract]
        string DeleteItem(string sListID, int iListItemID);

        [OperationContract]
        string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs);

        [OperationContract]
        string DeleteList(string sListID);

        [OperationContract]
        string DeleteRole(string sRoleName);

        [OperationContract]
        string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId);

        [OperationContract]
        string DeleteSiteCollection(string sSiteURL, string sWebApp);

        [OperationContract]
        string DeleteWeb(string sServerRelativeUrl);

        [OperationContract]
        string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId);

        [OperationContract]
        string DeleteWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract]
        string DisableValidationSettings(string listID);

        [OperationContract]
        string EnableValidationSettings(string validationNodeFieldsXml);

        [OperationContract]
        string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions);

        [OperationContract]
        string ReorderContentTypes(string sListID, string[] sContentTypes);

        [OperationContract]
        string SetDocumentParsing(bool bParserEnabled);

        [OperationContract]
        string SetMasterPage(string sWebXML);

        [OperationContract]
        string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound);

        [OperationContract]
        string SetWelcomePage(string WelcomePage);

        [OperationContract]
        string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions);

        [OperationContract]
        string UpdateGroupQuickLaunch(string value);

        [OperationContract]
        string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile);

        [OperationContract]
        string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions);

        [OperationContract]
        string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML, string sListXML,
            string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment);

        [OperationContract]
        string UpdateSiteCollectionSettings(string sUpdateXml, UpdateSiteCollectionOptions updateSiteCollectionOptions);

        [OperationContract]
        string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions);

        [OperationContract]
        string UpdateWebNavigationStructure(string sUpdateXml);

        [OperationContract]
        string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup);
    }
}