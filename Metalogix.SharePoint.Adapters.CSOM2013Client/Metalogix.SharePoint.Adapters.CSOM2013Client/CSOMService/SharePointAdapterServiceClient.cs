using Metalogix.SharePoint.Adapters;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class SharePointAdapterServiceClient :
        ClientBase<Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService.ISharePointAdapterService>,
        Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService.ISharePointAdapterService
    {
        public SharePointAdapterServiceClient()
        {
        }

        public SharePointAdapterServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public SharePointAdapterServiceClient(string endpointConfigurationName, string remoteAddress) : base(
            endpointConfigurationName, remoteAddress)
        {
        }

        public SharePointAdapterServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(
            endpointConfigurationName, remoteAddress)
        {
        }

        public SharePointAdapterServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding,
            remoteAddress)
        {
        }

        public string ActivateReusableWorkflowTemplates()
        {
            return base.Channel.ActivateReusableWorkflowTemplates();
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            return base.Channel.AddAlerts(sSiteUrl, sWebId, sAlertXML);
        }

        public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options)
        {
            return base.Channel.AddDocument(sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml,
                Options);
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            return base.Channel.AddDocumentOptimistically(listId, listName, folderPath, fileXml, fileContents, options,
                ref fieldsLookupCache);
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            return base.Channel.AddDocumentSetVersions(listName, listItemID, updatedTargetMetaInfo);
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            return base.Channel.AddDocumentTemplatetoContentType(docTemplate, cTypeXml, url);
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            return base.Channel.AddFields(sListID, sFieldXML);
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
        {
            return base.Channel.AddFileToFolder(sFileXML, fileContents, Options);
        }

        public string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options)
        {
            return base.Channel.AddFolder(sListID, sParentFolder, sFolderXML, Options);
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            return base.Channel.AddFolderOptimistically(listId, listName, folderPath, folderXml, options,
                ref fieldsLookupCache);
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            return base.Channel.AddFolderToFolder(sFolderXML);
        }

        public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            return base.Channel.AddFormTemplateToContentType(targetListId, docTemplate, cTypeXml, changedLookupFields);
        }

        public string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile)
        {
            return base.Channel.AddList(sListXML, Options, documentTemplateFile);
        }

        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options)
        {
            return base.Channel.AddListItem(sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents,
                listSettingsXml, Options);
        }

        public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
        {
            return base.Channel.AddOrUpdateAudience(sAudienceXml, options);
        }

        public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
        {
            return base.Channel.AddOrUpdateContentType(sContentTypeXML, sParentContentTypeName);
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            return base.Channel.AddOrUpdateGroup(sGroupXml);
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            return base.Channel.AddOrUpdateRole(sName, sDescription, lPermissionMask);
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            return base.Channel.AddReferencedTaxonomyData(sReferencedTaxonomyXML);
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            return base.Channel.AddReusedTerms(sTargetTermStoreGuid, sParentTermCollectionXML);
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            return base.Channel.AddRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions addSiteCollOptions)
        {
            return base.Channel.AddSiteCollection(sWebApp, sSiteCollectionXML, addSiteCollOptions);
        }

        public string AddSiteUser(string sUserXML, AddUserOptions options)
        {
            return base.Channel.AddSiteUser(sUserXML, options);
        }

        public string AddTerm(string termXml)
        {
            return base.Channel.AddTerm(termXml);
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            return base.Channel.AddTermGroup(targetTermStoreGuid, termGroupXml, includeGroupXmlInResult);
        }

        public string AddTermSet(string termSetXml)
        {
            return base.Channel.AddTermSet(termSetXml);
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            return base.Channel.AddTermstoreLanguages(sTargetTermStoreGuid, sLangaugesXML);
        }

        public string AddView(string sListID, string sViewXML)
        {
            return base.Channel.AddView(sListID, sViewXML);
        }

        public string AddWeb(string sWebXML, AddWebOptions addOptions)
        {
            return base.Channel.AddWeb(sWebXML, addOptions);
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            return base.Channel.AddWebParts(sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent);
        }

        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            return base.Channel.AddWorkflow(sListId, sWorkflowXml);
        }

        public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            return base.Channel.AddWorkflowAssociation(sListId, sWorkflowXml, bAllowDBWriting);
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            return base.Channel.AnalyzeChurn(pivotDate, sListID, iItemID, bRecursive);
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            return base.Channel.Apply2013Theme(colorPaletteUrl, spFontUrl, bgImageUrl);
        }

        public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            return base.Channel.ApplyOrUpdateContentType(sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType);
        }

        public string BeginCompilingAllAudiences()
        {
            return base.Channel.BeginCompilingAllAudiences();
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, AddDocumentOptions options)
        {
            return base.Channel.CatalogDocumentToStoragePointFileShareEndpoint(sNetworkPath, sListID, sFolder,
                sListItemXml, options);
        }

        public void CheckConnection()
        {
            base.Channel.CheckConnection();
        }

        public string CloseFileCopySession(Guid sessionId)
        {
            return base.Channel.CloseFileCopySession(sessionId);
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            return base.Channel.CloseWebParts(sWebPartPageServerRelativeUrl);
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            return base.Channel.ConfigureStoragePointFileShareEndpointAndProfile(sNetworkPath, sSharePointPath);
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            return base.Channel.CorrectDefaultPageVersions(sListID, sFolder, sListItemXML);
        }

        public string DeleteAllAudiences(string inputXml)
        {
            return base.Channel.DeleteAllAudiences(inputXml);
        }

        public string DeleteAudience(string sAudienceName)
        {
            return base.Channel.DeleteAudience(sAudienceName);
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            return base.Channel.DeleteContentTypes(sListID, contentTypeIDs);
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            return base.Channel.DeleteFolder(sListID, iListItemID, sFolder);
        }

        public string DeleteGroup(string sGroupName)
        {
            return base.Channel.DeleteGroup(sGroupName);
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            return base.Channel.DeleteItem(sListID, iListItemID);
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            return base.Channel.DeleteItems(sListID, bDeleteAllItems, sIDs);
        }

        public string DeleteList(string sListID)
        {
            return base.Channel.DeleteList(sListID);
        }

        public string DeleteMigrationJob(string jobConfiguration)
        {
            return base.Channel.DeleteMigrationJob(jobConfiguration);
        }

        public string DeleteRole(string sRoleName)
        {
            return base.Channel.DeleteRole(sRoleName);
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            return base.Channel.DeleteRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            return base.Channel.DeleteSiteCollection(sSiteURL, sWebApp);
        }

        public string DeleteSP2013Workflows(string configurationXml)
        {
            return base.Channel.DeleteSP2013Workflows(configurationXml);
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            return base.Channel.DeleteWeb(sServerRelativeUrl);
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            return base.Channel.DeleteWebPart(sWebPartPageServerRelativeUrl, sWebPartId);
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            return base.Channel.DeleteWebParts(sWebPartPageServerRelativeUrl);
        }

        public string DisableValidationSettings(string listID)
        {
            return base.Channel.DisableValidationSettings(listID);
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            return base.Channel.EnableValidationSettings(validationNodeFieldsXml);
        }

        public void EndAdapterService()
        {
            base.Channel.EndAdapterService();
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            return base.Channel.ExecuteCommand(commandName, commandConfigurationXml);
        }

        public string FindAlerts()
        {
            return base.Channel.FindAlerts();
        }

        public string FindUniquePermissions()
        {
            return base.Channel.FindUniquePermissions();
        }

        public string GetAdapterConfiguration()
        {
            return base.Channel.GetAdapterConfiguration();
        }

        public string GetAlerts(string sListID, int sItemID)
        {
            return base.Channel.GetAlerts(sListID, sItemID);
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            return base.Channel.GetAttachments(sListID, iItemID);
        }

        public string GetAudiences()
        {
            return base.Channel.GetAudiences();
        }

        public string GetContentTypes(string sListId)
        {
            return base.Channel.GetContentTypes(sListId);
        }

        public IList<Cookie> GetCookieManagerCookies()
        {
            return base.Channel.GetCookieManagerCookies();
        }

        public bool GetCookieManagerIsActive()
        {
            return base.Channel.GetCookieManagerIsActive();
        }

        public bool GetCookieManagerLocksCookies()
        {
            return base.Channel.GetCookieManagerLocksCookies();
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            return base.Channel.GetDashboardPageTemplate(iTemplateId);
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            return base.Channel.GetDocument(sDocID, sFileDirRef, sFileLeafRef, iLevel);
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            return base.Channel.GetDocumentBlobRef(sDocID, sFileDirRef, sFileLeafRef, iLevel);
        }

        public string GetDocumentId(string sDocUrl)
        {
            return base.Channel.GetDocumentId(sDocUrl);
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            return base.Channel.GetDocumentVersion(sDocID, sFileDirRef, sFileLeafRef, iVersion);
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            return base.Channel.GetDocumentVersionBlobRef(sDocID, sFileDirRef, sFileLeafRef, iVersion);
        }

        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            return base.Channel.GetExternalContentTypeOperations(sExternalContentTypeNamespace,
                sExternalContentTypeName);
        }

        public string GetExternalContentTypes()
        {
            return base.Channel.GetExternalContentTypes();
        }

        public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sReadListOperation, string sListID)
        {
            return base.Channel.GetExternalItems(sExternalContentTypeNamespace, sExternalContentTypeName,
                sReadListOperation, sListID);
        }

        public string GetFields(string sListID, bool bGetAllAvailableFields)
        {
            return base.Channel.GetFields(sListID, bGetAllAvailableFields);
        }

        public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
        {
            return base.Channel.GetFiles(sFolderPath, itemTypes);
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            return base.Channel.GetFolders(sListID, sIDs, sParentFolder);
        }

        public string GetGroups()
        {
            return base.Channel.GetGroups();
        }

        public string GetLanguagesAndWebTemplates()
        {
            return base.Channel.GetLanguagesAndWebTemplates();
        }

        public string GetList(string sListID)
        {
            return base.Channel.GetList(sListID);
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
        {
            return base.Channel.GetListItemIDs(sListID, sParentFolder, bRecursive, itemTypes);
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
        {
            return base.Channel.GetListItems(sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes,
                sListSettings, getOptions);
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions)
        {
            return base.Channel.GetListItemsByQuery(listID, fields, query, listSettings, getOptions);
        }

        public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
        {
            return base.Channel.GetListItemVersions(sListID, iItemID, sFields, sListSettings);
        }

        public string GetLists()
        {
            return base.Channel.GetLists();
        }

        public string GetListTemplates()
        {
            return base.Channel.GetListTemplates();
        }

        public string GetMigrationJobStatus(string jobConfiguration)
        {
            return base.Channel.GetMigrationJobStatus(jobConfiguration);
        }

        public string GetMySiteData(string sSiteURL)
        {
            return base.Channel.GetMySiteData(sSiteURL);
        }

        public string GetPersonalSite(string email)
        {
            return base.Channel.GetPersonalSite(email);
        }

        public string GetPortalListingGroups()
        {
            return base.Channel.GetPortalListingGroups();
        }

        public string GetPortalListingIDs()
        {
            return base.Channel.GetPortalListingIDs();
        }

        public string GetPortalListings(string sIDList)
        {
            return base.Channel.GetPortalListings(sIDList);
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            return base.Channel.GetReferencedTaxonomyFullXml(sReferencedTaxonomyXml);
        }

        public string GetRoleAssignments(string sListID, int iItemId)
        {
            return base.Channel.GetRoleAssignments(sListID, iItemId);
        }

        public string GetRoles(string sListId)
        {
            return base.Channel.GetRoles(sListId);
        }

        public string GetServerHealth()
        {
            return base.Channel.GetServerHealth();
        }

        public string GetSharePointVersion()
        {
            return base.Channel.GetSharePointVersion();
        }

        public string GetSite(bool bFetchFullXml)
        {
            return base.Channel.GetSite(bFetchFullXml);
        }

        public string GetSiteCollections()
        {
            return base.Channel.GetSiteCollections();
        }

        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            return base.Channel.GetSiteCollectionsOnWebApp(sWebAppName);
        }

        public string GetSiteQuotaTemplates()
        {
            return base.Channel.GetSiteQuotaTemplates();
        }

        public string GetSiteUsers()
        {
            return base.Channel.GetSiteUsers();
        }

        public string GetSP2013Workflows(string configurationXml)
        {
            return base.Channel.GetSP2013Workflows(configurationXml);
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            return base.Channel.GetStoragePointProfileConfiguration(sSharePointPath);
        }

        public string GetSubWebs()
        {
            return base.Channel.GetSubWebs();
        }

        public string GetSystemInfo()
        {
            return base.Channel.GetSystemInfo();
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            return base.Channel.GetTermCollectionFromTerm(sTermStoreId, sTermGroupId, sTermSetId, sTermId);
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            return base.Channel.GetTermCollectionFromTermSet(sTermStoreId, sTermGroupId, sTermSetId);
        }

        public string GetTermGroups(string sTermStoreId)
        {
            return base.Channel.GetTermGroups(sTermStoreId);
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            return base.Channel.GetTermSetCollection(sTermStoreId, sTermGroupId);
        }

        public string GetTermSets(string sTermGroupId)
        {
            return base.Channel.GetTermSets(sTermGroupId);
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            return base.Channel.GetTermsFromTermSet(sTermSetId, bRecursive);
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            return base.Channel.GetTermsFromTermSetItem(sTermSetItemId);
        }

        public string GetTermStores()
        {
            return base.Channel.GetTermStores();
        }

        public string GetUserFromProfile()
        {
            return base.Channel.GetUserFromProfile();
        }

        public string GetUserProfiles(out string sErrors, string sSiteURL, string sLoginName)
        {
            return base.Channel.GetUserProfiles(out sErrors, sSiteURL, sLoginName);
        }

        public string GetWeb(bool bFetchFullXml)
        {
            return base.Channel.GetWeb(bFetchFullXml);
        }

        public string GetWebApplications()
        {
            return base.Channel.GetWebApplications();
        }

        public string GetWebNavigationSettings()
        {
            return base.Channel.GetWebNavigationSettings();
        }

        public string GetWebNavigationStructure()
        {
            return base.Channel.GetWebNavigationStructure();
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            return base.Channel.GetWebPartPage(sWebPartPageServerRelativeUrl);
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            return base.Channel.GetWebPartPageTemplate(iTemplateId);
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            return base.Channel.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
        }

        public string GetWebTemplates()
        {
            return base.Channel.GetWebTemplates();
        }

        public string GetWorkflowAssociations(string sObjectID, string sObjectScope)
        {
            return base.Channel.GetWorkflowAssociations(sObjectID, sObjectScope);
        }

        public string GetWorkflows(string sListID, int iItemID)
        {
            return base.Channel.GetWorkflows(sListID, iItemID);
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            return base.Channel.HasDocument(sDocumentServerRelativeUrl);
        }

        public bool HasPersonalSite(string email)
        {
            return base.Channel.HasPersonalSite(email);
        }

        public string HasUniquePermissions(string listID, int listItemID)
        {
            return base.Channel.HasUniquePermissions(listID, listItemID);
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            return base.Channel.HasWebParts(sWebPartPageServerRelativeUrl);
        }

        public string HasWorkflows(string sListID, string sItemID)
        {
            return base.Channel.HasWorkflows(sListID, sItemID);
        }

        public void InitializeAdapterConfiguration(string sXml)
        {
            base.Channel.InitializeAdapterConfiguration(sXml);
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            return base.Channel.IsListContainsInfoPathOrAspxItem(listId);
        }

        public string IsWorkflowServicesInstanceAvailable()
        {
            return base.Channel.IsWorkflowServicesInstanceAvailable();
        }

        public string MigrateSP2013Workflows(string configurationXml)
        {
            return base.Channel.MigrateSP2013Workflows(configurationXml);
        }

        public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
        {
            return base.Channel.ModifyWebNavigationSettings(sWebXML, ModNavOptions);
        }

        public Guid OpenFileCopySession(StreamType streamType, int retentionTime)
        {
            return base.Channel.OpenFileCopySession(streamType, retentionTime);
        }

        public string ProvisionMigrationContainer()
        {
            return base.Channel.ProvisionMigrationContainer();
        }

        public string ProvisionMigrationQueue()
        {
            return base.Channel.ProvisionMigrationQueue();
        }

        public void ProvisionPersonalSites(string[] emails)
        {
            base.Channel.ProvisionPersonalSites(emails);
        }

        public byte[] ReadChunk(Guid sessionId, long bytesToRead)
        {
            return base.Channel.ReadChunk(sessionId, bytesToRead);
        }

        public void RemovePersonalSite(string email)
        {
            base.Channel.RemovePersonalSite(email);
        }

        public string ReorderContentTypes(string sListID, string[] sContentTypes)
        {
            return base.Channel.ReorderContentTypes(sListID, sContentTypes);
        }

        public string RequestMigrationJob(string jobConfiguration, bool isMicrosoftCustomer, byte[] key = null)
        {
            return base.Channel.RequestMigrationJob(jobConfiguration, isMicrosoftCustomer, key);
        }

        public string ResolvePrincipals(string principal)
        {
            return base.Channel.ResolvePrincipals(principal);
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            return base.Channel.SearchForDocument(sSearchTerm, sOptionsXml);
        }

        public void SetCookieManagerCookies(IList<Cookie> cookies)
        {
            base.Channel.SetCookieManagerCookies(cookies);
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            return base.Channel.SetDocumentParsing(bParserEnabled);
        }

        public string SetMasterPage(string sWebXML)
        {
            return base.Channel.SetMasterPage(sWebXML);
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            return base.Channel.SetUserProfile(sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound);
        }

        public string SetWelcomePage(string WelcomePage)
        {
            return base.Channel.SetWelcomePage(WelcomePage);
        }

        public string StoragePointAvailable(string inputXml)
        {
            return base.Channel.StoragePointAvailable(inputXml);
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            return base.Channel.StoragePointProfileConfigured(sSharePointPath);
        }

        public void UpdateCookieManagerCookies()
        {
            base.Channel.UpdateCookieManagerCookies();
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions)
        {
            return base.Channel.UpdateDocument(sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents,
                updateOptions);
        }

        public string UpdateGroupQuickLaunch(string value)
        {
            return base.Channel.UpdateGroupQuickLaunch(value);
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile)
        {
            return base.Channel.UpdateList(sListID, sListXML, sViewXml, updateOptions, documentTemplateFile);
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions)
        {
            return base.Channel.UpdateListItem(sListID, sParentFolder, iItemID, sListItemXML, attachmentNames,
                attachmentContents, updateOptions);
        }

        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            return base.Channel.UpdateListItemStatus(bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemID,
                sCheckinComment, sPublishComment, sApprovalComment);
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            return base.Channel.UpdateSiteCollectionSettings(sUpdateXml, updateSiteCollectionOptions);
        }

        public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
        {
            return base.Channel.UpdateWeb(sWebXML, updateOptions);
        }

        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            return base.Channel.UpdateWebNavigationStructure(sUpdateXml);
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            return base.Channel.ValidateUserInfo(sUserIdentifier, bCanBeDomainGroup);
        }

        public void WriteChunk(Guid sessionId, byte[] data)
        {
            base.Channel.WriteChunk(sessionId, data);
        }
    }
}