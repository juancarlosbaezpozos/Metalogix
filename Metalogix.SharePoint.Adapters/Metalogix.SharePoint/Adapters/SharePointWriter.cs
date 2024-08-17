using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointWriter : SharePointExtensionsLogger, ISharePointWriter, ISharePointAdapterCommand
    {
        protected SharePointWriter(SharePointAdapter adapter)
        {
            base.Target = adapter;
        }

        public string ActivateReusableWorkflowTemplates()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteUrl, sWebId, sAlertXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml, Options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, folderPath, fileXml, fileContents, options, fieldsLookupCache };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, updatedTargetMetaInfo };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { docTemplate, cTypeXml, url };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sFieldXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFileXML, fileContents, Options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sParentFolder, sFolderXML, Options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, folderPath, folderXml, options, fieldsLookupCache };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFolderXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { targetListId, docTemplate, cTypeXml, changedLookupFields };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListXML, Options, documentTemplateFile };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, listSettingsXml, Options
            };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sAudienceXml, options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateContentType(string sContentTypeXml, string sParentContentTypeName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sContentTypeXml, sParentContentTypeName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sGroupXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sName, sDescription, lPermissionMask };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sReferencedTaxonomyXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTargetTermStoreGuid, sParentTermCollectionXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions AddSiteCollOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebApp, sSiteCollectionXML, AddSiteCollOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddSiteUser(string sUserXML, AddUserOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUserXML, options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddTerm(string termXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { termXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { targetTermStoreGuid, termGroupXml, includeGroupXmlInResult };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddTermSet(string termSetXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { termSetXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTargetTermStoreGuid, sLangaugesXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddView(string sListID, string sViewXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sViewXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddWeb(string sWebXML, AddWebOptions addOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, addOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sWorkflowXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sWorkflowXml, bAllowDBWriting };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { colorPaletteUrl, spFontUrl, bgImageUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string BeginCompilingAllAudiences()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, AddDocumentOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sNetworkPath, sListID, sFolder, sListItemXml, options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sNetworkPath, sSharePointPath };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sFolder, sListItemXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteAllAudiences(string inputXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { inputXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteAudience(string sAudienceName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sAudienceName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, contentTypeIDs };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iListItemID, sFolder };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteGroup(string sGroupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sGroupName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iListItemID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, bDeleteAllItems, sIDs };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteList(string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteRole(string sRoleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sRoleName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL, sWebApp };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl, sWebPartId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string DisableValidationSettings(string listID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { validationNodeFieldsXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public static ISharePointWriter GetSharePointWriter(SharePointAdapter adapter)
        {
            if (!SharePointAdapter.EnableAdapterLogging)
            {
                return (ISharePointWriter)adapter;
            }

            return new SharePointWriter(adapter);
        }

        string Metalogix.SharePoint.Adapters.ISharePointAdapterCommand.ExecuteCommand(string commandName,
            string commandConfigurationXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { commandName, commandConfigurationXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, ModNavOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string ReorderContentTypes(string sListId, string[] sContentTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sContentTypes };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bParserEnabled };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string SetMasterPage(string sWebXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string SetWelcomePage(string WelcomePage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] welcomePage = new object[] { WelcomePage };
            return (string)base.ExecuteMethod(name, welcomePage);
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, Options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateGroupQuickLaunch(string value)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { value };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions Options,
            byte[] documentTemplateFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sListXML, sViewXml, Options, documentTemplateFile };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents, UpdateListItemOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents, Options };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemID, sCheckinComment, sPublishComment,
                sApprovalComment
            };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUpdateXml, updateSiteCollectionOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, updateOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string UpdateWebNavigationStructure(string sChangesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sChangesXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUserIdentifier, bCanBeDomainGroup };
            return (string)base.ExecuteMethod(name, objArray);
        }
    }
}