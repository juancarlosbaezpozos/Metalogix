using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointReader : SharePointExtensionsLogger, ISharePointReader, ISharePointAdapterCommand
    {
        protected SharePointReader(SharePointAdapter adapter)
        {
            base.Target = adapter;
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pivotDate, sListID, iItemID, bRecursive };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string FindAlerts()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string FindUniquePermissions()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetAlerts(string sListID, int iItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetAudiences()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetContentTypes(string sListId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { iTemplateId };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public string GetDocumentId(string sDocUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sExternalContentTypeNamespace, sExternalContentTypeName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetExternalContentTypes()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sExternalContentTypeOperationName, string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sExternalContentTypeNamespace, sExternalContentTypeName, sExternalContentTypeOperationName, sListID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetFields(string sListId, bool bGetAllAvailableFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, bGetAllAvailableFields };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFolderPath, itemTypes };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sIDs, sParentFolder };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetGroups()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetLanguagesAndWebTemplates()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetList(string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sParentFolder, bRecursive, itemTypes };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings, getOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID, fields, query, listSettings, getOptions };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID, sFields, sListSettings };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetLists()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetListTemplates()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetMySiteData(string sSiteURL)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetPortalListingGroups()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetPortalListingIDs()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetPortalListings(string sIDList)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sIDList };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sReferencedTaxonomyXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetRoleAssignments(string sListId, int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, iItemId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetRoles(string sListId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public static ISharePointReader GetSharePointReader(SharePointAdapter adapter)
        {
            if (!SharePointAdapter.EnableAdapterLogging)
            {
                return (ISharePointReader)adapter;
            }

            return new SharePointReader(adapter);
        }

        public string GetSharePointVersion()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSite(bool bFetchFullXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bFetchFullXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetSiteCollections()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebAppName };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetSiteQuotaTemplates()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSiteUsers()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSharePointPath };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetSubWebs()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSystemInfo()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId, sTermId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermGroups(string sTermStoreId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermSets(string sTermGroupId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermGroupId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermSetId, bRecursive };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermSetItemId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetTermStores()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetUserFromProfile()
        {
            object[] objArray = null;
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
        }

        public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
        {
            object[] objArray = new object[] { sSiteURL, sLoginName, null };
            string str = (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            sErrors = (string)objArray[2];
            return str;
        }

        public string GetWeb(bool bFetchFullXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bFetchFullXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetWebApplications()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebNavigationSettings()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebNavigationStructure()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { iTemplateId };
            return (byte[])base.ExecuteMethod(name, objArray);
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetWebTemplates()
        {
            return (string)base.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWorkflowAssociations(string sObjectID, string sObjectType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sObjectID, sObjectType };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string GetWorkflows(string sListId, int sItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sItemId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocumentServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string HasUniquePermissions(string listID, int listItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID, listItemID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string HasWorkflows(string sListID, string sItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sItemID };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listId };
            return (string)base.ExecuteMethod(name, objArray);
        }

        string Metalogix.SharePoint.Adapters.ISharePointAdapterCommand.ExecuteCommand(string commandName,
            string commandConfigurationXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { commandName, commandConfigurationXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSearchTerm, sOptionsXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string StoragePointAvailable(string inputXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { inputXml };
            return (string)base.ExecuteMethod(name, objArray);
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSharePointPath };
            return (string)base.ExecuteMethod(name, objArray);
        }
    }
}