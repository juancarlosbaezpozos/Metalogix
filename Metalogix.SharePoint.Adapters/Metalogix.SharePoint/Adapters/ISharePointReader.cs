using System;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters
{
    [ServiceContract]
    public interface ISharePointReader : ISharePointAdapterCommand
    {
        [OperationContract]
        string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive);

        [OperationContract]
        string FindAlerts();

        [OperationContract]
        string FindUniquePermissions();

        [OperationContract]
        string GetAlerts(string sListID, int sItemID);

        [OperationContract]
        string GetAttachments(string sListID, int iItemID);

        [OperationContract]
        string GetAudiences();

        [OperationContract]
        string GetContentTypes(string sListId);

        [OperationContract]
        byte[] GetDashboardPageTemplate(int iTemplateId);

        [OperationContract]
        byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel);

        [OperationContract]
        byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel);

        [OperationContract]
        string GetDocumentId(string sDocUrl);

        [OperationContract]
        byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion);

        [OperationContract]
        byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion);

        [OperationContract]
        string GetExternalContentTypeOperations(string sExternalContentTypeNamespace, string sExternalContentTypeName);

        [OperationContract]
        string GetExternalContentTypes();

        [OperationContract]
        string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sReadListOperation, string sListID);

        [OperationContract]
        string GetFields(string sListID, bool bGetAllAvailableFields);

        [OperationContract]
        string GetFiles(string sFolderPath, ListItemQueryType itemTypes);

        [OperationContract]
        string GetFolders(string sListID, string sIDs, string sParentFolder);

        [OperationContract]
        string GetGroups();

        [OperationContract]
        string GetLanguagesAndWebTemplates();

        [OperationContract]
        string GetList(string sListID);

        [OperationContract]
        string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes);

        [OperationContract]
        string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions);

        [OperationContract]
        string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions);

        [OperationContract]
        string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings);

        [OperationContract]
        string GetLists();

        [OperationContract]
        string GetListTemplates();

        [OperationContract]
        string GetMySiteData(string sSiteURL);

        [OperationContract]
        string GetPortalListingGroups();

        [OperationContract]
        string GetPortalListingIDs();

        [OperationContract]
        string GetPortalListings(string sIDList);

        [OperationContract]
        string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml);

        [OperationContract]
        string GetRoleAssignments(string sListID, int iItemId);

        [OperationContract]
        string GetRoles(string sListId);

        [OperationContract]
        string GetSharePointVersion();

        [OperationContract]
        string GetSite(bool bFetchFullXml);

        [OperationContract]
        string GetSiteCollections();

        [OperationContract]
        string GetSiteCollectionsOnWebApp(string sWebAppName);

        [OperationContract]
        string GetSiteQuotaTemplates();

        [OperationContract]
        string GetSiteUsers();

        [OperationContract]
        string GetStoragePointProfileConfiguration(string sSharePointPath);

        [OperationContract]
        string GetSubWebs();

        [OperationContract]
        string GetSystemInfo();

        [OperationContract]
        string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId, string sTermId);

        [OperationContract]
        string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId);

        [OperationContract]
        string GetTermGroups(string sTermStoreId);

        [OperationContract]
        string GetTermSetCollection(string sTermStoreId, string sTermGroupId);

        [OperationContract]
        string GetTermSets(string sTermGroupId);

        [OperationContract]
        string GetTermsFromTermSet(string sTermSetId, bool bRecursive);

        [OperationContract]
        string GetTermsFromTermSetItem(string sTermSetItemId);

        [OperationContract]
        string GetTermStores();

        [OperationContract]
        string GetUserFromProfile();

        [OperationContract]
        string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors);

        [OperationContract]
        string GetWeb(bool bFetchFullXml);

        [OperationContract]
        string GetWebApplications();

        [OperationContract]
        string GetWebNavigationSettings();

        [OperationContract]
        string GetWebNavigationStructure();

        [OperationContract]
        string GetWebPartPage(string sWebPartPageServerRelativeUrl);

        [OperationContract]
        byte[] GetWebPartPageTemplate(int iTemplateId);

        [OperationContract]
        string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl);

        [OperationContract]
        string GetWebTemplates();

        [OperationContract]
        string GetWorkflowAssociations(string sObjectID, string sObjectScope);

        [OperationContract]
        string GetWorkflows(string sListID, int iItemID);

        [OperationContract]
        string HasDocument(string sDocumentServerRelativeUrl);

        [OperationContract]
        string HasUniquePermissions(string listID, int listItemID);

        [OperationContract]
        string HasWebParts(string sWebPartPageServerRelativeUrl);

        [OperationContract]
        string HasWorkflows(string sListID, string sItemID);

        [OperationContract]
        string IsListContainsInfoPathOrAspxItem(string listId);

        [OperationContract]
        string SearchForDocument(string sSearchTerm, string sOptionsXml);

        [OperationContract]
        string StoragePointAvailable(string inputXml);

        [OperationContract]
        string StoragePointProfileConfigured(string sSharePointPath);
    }
}