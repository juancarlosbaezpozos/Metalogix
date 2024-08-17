using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.CSOM2013;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	[ServiceBehavior(IncludeExceptionDetailInFaults=true, ConcurrencyMode=ConcurrencyMode.Multiple)]
	public class CSOM2013Service : ISharePointAdapterService, ISharePointReader, ISharePointWriter, IBinaryTransferHandler, IServerHealthMonitor, IMySitesConnector, IMigrationPipeline, ISP2013WorkflowAdapter, ISharePointAdapterCommand
	{
		private object m_oLockAdapter = new object();

		private CSOMAdapter m_adapter;

		private static volatile int s_iRef;

		public CSOMAdapter Adapter
		{
			get
			{
				CSOMAdapter mAdapter;
				lock (this.m_oLockAdapter)
				{
					mAdapter = this.m_adapter;
				}
				return mAdapter;
			}
		}

		static CSOM2013Service()
		{
		}

		public CSOM2013Service()
		{
		}

		public string ActivateReusableWorkflowTemplates()
		{
			return this.Adapter.ActivateReusableWorkflowTemplates();
		}

		public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
		{
			return this.Adapter.AddAlerts(sSiteUrl, sWebId, sAlertXML);
		}

		public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents, string listSettingsXml, AddDocumentOptions Options)
		{
			return this.Adapter.AddDocument(sListID, sParentFolder, sListItemXML, fileContents, listSettingsXml, Options);
		}

		public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml, byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
		{
			return this.Adapter.AddDocumentOptimistically(listId, listName, folderPath, fileXml, fileContents, options, ref fieldsLookupCache);
		}

		public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
		{
			return this.Adapter.AddDocumentSetVersions(listName, listItemID, updatedTargetMetaInfo);
		}

		public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
		{
			return this.Adapter.AddDocumentTemplatetoContentType(docTemplate, cTypeXml, url);
		}

		public string AddFields(string sListID, string sFieldXML)
		{
			return this.Adapter.AddFields(sListID, sFieldXML);
		}

		public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
		{
			return this.Adapter.AddFileToFolder(sFileXML, fileContents, Options);
		}

		public string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options)
		{
			return this.Adapter.AddFolder(sListID, sParentFolder, sFolderXML, Options);
		}

		public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml, AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
		{
			return this.Adapter.AddFolderOptimistically(listId, listName, folderPath, folderXml, options, ref fieldsLookupCache);
		}

		public string AddFolderToFolder(string sFolderXML)
		{
			return this.Adapter.AddFolderToFolder(sFolderXML);
		}

		public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml, string changedLookupFields)
		{
			return this.Adapter.AddFormTemplateToContentType(targetListId, docTemplate, cTypeXml, changedLookupFields);
		}

		public string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile)
		{
			return this.Adapter.AddList(sListXML, Options, documentTemplateFile);
		}

		public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachmentNames, byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options)
		{
			return this.Adapter.AddListItem(sListID, sParentFolder, sListItemXML, attachmentNames, attachmentContents, listSettingsXml, Options);
		}

		public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
		{
			return this.Adapter.AddOrUpdateAudience(sAudienceXml, options);
		}

		public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
		{
			return this.Adapter.AddOrUpdateContentType(sContentTypeXML, sParentContentTypeName);
		}

		public string AddOrUpdateGroup(string sGroupXml)
		{
			return this.Adapter.AddOrUpdateGroup(sGroupXml);
		}

		public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
		{
			return this.Adapter.AddOrUpdateRole(sName, sDescription, lPermissionMask);
		}

		public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
		{
			return this.Adapter.AddReferencedTaxonomyData(sReferencedTaxonomyXML);
		}

		public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
		{
			return this.Adapter.AddReusedTerms(sTargetTermStoreGuid, sParentTermCollectionXML);
		}

		public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID, int iItemId)
		{
			return this.Adapter.AddRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
		}

		public string AddSiteCollection(string sWebApp, string sSiteCollectionXML, AddSiteCollectionOptions addSiteCollOptions)
		{
			return this.Adapter.AddSiteCollection(sWebApp, sSiteCollectionXML, addSiteCollOptions);
		}

		public string AddSiteUser(string sUserXML, AddUserOptions options)
		{
			return this.Adapter.AddSiteUser(sUserXML, options);
		}

		public string AddTerm(string termXml)
		{
			return this.Adapter.AddTerm(termXml);
		}

		public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
		{
			return this.Adapter.AddTermGroup(targetTermStoreGuid, termGroupXml, includeGroupXmlInResult);
		}

		public string AddTermSet(string termSetXml)
		{
			return this.Adapter.AddTermSet(termSetXml);
		}

		public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
		{
			return this.Adapter.AddTermstoreLanguages(sTargetTermStoreGuid, sLangaugesXML);
		}

		public string AddView(string sListID, string sViewXML)
		{
			return this.Adapter.AddView(sListID, sViewXML);
		}

		public string AddWeb(string sWebXML, AddWebOptions addOptions)
		{
			return this.Adapter.AddWeb(sWebXML, addOptions);
		}

		public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl, string sEmbeddedHtmlContent)
		{
			return this.Adapter.AddWebParts(sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent);
		}

		public string AddWorkflow(string sListId, string sWorkflowXml)
		{
			return this.Adapter.AddWorkflow(sListId, sWorkflowXml);
		}

		public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
		{
			return this.Adapter.AddWorkflowAssociation(sListId, sWorkflowXml, bAllowDBWriting);
		}

		public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
		{
			return this.Adapter.AnalyzeChurn(pivotDate, sListID, iItemID, bRecursive);
		}

		public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
		{
			return this.Adapter.Apply2013Theme(colorPaletteUrl, spFontUrl, bgImageUrl);
		}

		public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML, bool bMakeDefaultContentType)
		{
			return this.Adapter.ApplyOrUpdateContentType(sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType);
		}

		public string BeginCompilingAllAudiences()
		{
			return this.Adapter.BeginCompilingAllAudiences();
		}

		public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID, string sFolder, string sListItemXml, AddDocumentOptions options)
		{
			return this.Adapter.CatalogDocumentToStoragePointFileShareEndpoint(sNetworkPath, sListID, sFolder, sListItemXml, options);
		}

		public void CheckConnection()
		{
			this.Adapter.CheckConnection();
		}

		public string CloseFileCopySession(Guid sessionId)
		{
			throw new NotImplementedException(Resources.Method_Not_Implemented);
		}

		public string CloseWebParts(string sWebPartPageServerRelativeUrl)
		{
			return this.Adapter.CloseWebParts(sWebPartPageServerRelativeUrl);
		}

		public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
		{
			return this.Adapter.ConfigureStoragePointFileShareEndpointAndProfile(sNetworkPath, sSharePointPath);
		}

		public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
		{
			return this.Adapter.CorrectDefaultPageVersions(sListID, sFolder, sListItemXML);
		}

		public string DeleteAllAudiences(string inputXml)
		{
			return this.Adapter.DeleteAllAudiences(inputXml);
		}

		public string DeleteAudience(string sAudienceName)
		{
			return this.Adapter.DeleteAudience(sAudienceName);
		}

		public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
		{
			return this.Adapter.DeleteContentTypes(sListID, contentTypeIDs);
		}

		public string DeleteFolder(string sListID, int iListItemID, string sFolder)
		{
			return this.Adapter.DeleteFolder(sListID, iListItemID, sFolder);
		}

		public string DeleteGroup(string sGroupName)
		{
			return this.Adapter.DeleteGroup(sGroupName);
		}

		public string DeleteItem(string sListID, int iListItemID)
		{
			return this.Adapter.DeleteItem(sListID, iListItemID);
		}

		public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
		{
			return this.Adapter.DeleteItems(sListID, bDeleteAllItems, sIDs);
		}

		public string DeleteList(string sListID)
		{
			return this.Adapter.DeleteList(sListID);
		}

		public string DeleteMigrationJob(string jobConfiguration)
		{
			return this.Adapter.DeleteMigrationJob(jobConfiguration);
		}

		public string DeleteRole(string sRoleName)
		{
			return this.Adapter.DeleteRole(sRoleName);
		}

		public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID, int iItemId)
		{
			return this.Adapter.DeleteRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
		}

		public string DeleteSiteCollection(string sSiteURL, string sWebApp)
		{
			return this.Adapter.DeleteSiteCollection(sSiteURL, sWebApp);
		}

		public string DeleteSP2013Workflows(string configurationXml)
		{
			return this.Adapter.DeleteSP2013Workflows(configurationXml);
		}

		public string DeleteWeb(string sServerRelativeUrl)
		{
			return this.Adapter.DeleteWeb(sServerRelativeUrl);
		}

		public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
		{
			return this.Adapter.DeleteWebPart(sWebPartPageServerRelativeUrl, sWebPartId);
		}

		public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
		{
			return this.Adapter.DeleteWebParts(sWebPartPageServerRelativeUrl);
		}

		public string DisableValidationSettings(string listID)
		{
			return this.Adapter.DisableValidationSettings(listID);
		}

		public string EnableValidationSettings(string validationNodeFieldsXml)
		{
			return this.Adapter.EnableValidationSettings(validationNodeFieldsXml);
		}

		public void EndAdapterService()
		{
			lock (this.m_oLockAdapter)
			{
				if (this.m_adapter != null)
				{
					this.m_adapter.Dispose();
					this.m_adapter = null;
					Metalogix.SharePoint.Adapters.CSOM2013Service.CSOM2013Service.s_iRef--;
				}
			}
			if (!ServiceManager.RequireManualShutdown && Metalogix.SharePoint.Adapters.CSOM2013Service.CSOM2013Service.s_iRef <= 0)
			{
				this.EndService();
			}
		}

		public void EndService()
		{
			Application.Exit();
		}

		public string ExecuteCommand(string commandName, string commandConfigurationXml)
		{
			return this.Adapter.ExecuteCommand(commandName, commandConfigurationXml);
		}

		public string FindAlerts()
		{
			return this.Adapter.FindAlerts();
		}

		public string FindUniquePermissions()
		{
			return this.Adapter.FindUniquePermissions();
		}

		public string GetAdapterConfiguration()
		{
			string xML;
			lock (this.m_oLockAdapter)
			{
				if (this.m_adapter != null)
				{
					xML = this.m_adapter.ToXML();
				}
				else
				{
					xML = null;
				}
			}
			return xML;
		}

		public string GetAlerts(string sListID, int sItemID)
		{
			return this.Adapter.GetAlerts(sListID, sItemID);
		}

		public string GetAttachments(string sListID, int iItemID)
		{
			return this.Adapter.GetAttachments(sListID, iItemID);
		}

		public string GetAudiences()
		{
			return this.Adapter.GetAudiences();
		}

		public string GetContentTypes(string sListId)
		{
			return this.Adapter.GetContentTypes(sListId);
		}

		public IList<Cookie> GetCookieManagerCookies()
		{
			if (!this.Adapter.HasActiveCookieManager)
			{
				return new List<Cookie>();
			}
			return this.Adapter.CookieManager.Cookies;
		}

		public bool GetCookieManagerIsActive()
		{
			return this.Adapter.HasActiveCookieManager;
		}

		public bool GetCookieManagerLocksCookies()
		{
			if (!this.Adapter.HasActiveCookieManager)
			{
				return false;
			}
			return this.Adapter.CookieManager.LockCookie;
		}

		public byte[] GetDashboardPageTemplate(int iTemplateId)
		{
			return this.Adapter.GetDashboardPageTemplate(iTemplateId);
		}

		public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
		{
			return this.Adapter.GetDocument(sDocID, sFileDirRef, sFileLeafRef, iLevel);
		}

		public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
		{
			return this.Adapter.GetDocumentBlobRef(sDocID, sFileDirRef, sFileLeafRef, iLevel);
		}

		public string GetDocumentId(string sDocUrl)
		{
			return this.Adapter.GetDocumentId(sDocUrl);
		}

		public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
		{
			return this.Adapter.GetDocumentVersion(sDocID, sFileDirRef, sFileLeafRef, iVersion);
		}

		public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
		{
			return this.Adapter.GetDocumentVersionBlobRef(sDocID, sFileDirRef, sFileLeafRef, iVersion);
		}

		public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace, string sExternalContentTypeName)
		{
			return this.Adapter.GetExternalContentTypeOperations(sExternalContentTypeNamespace, sExternalContentTypeName);
		}

		public string GetExternalContentTypes()
		{
			return this.Adapter.GetExternalContentTypes();
		}

		public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName, string sReadListOperation, string sListID)
		{
			return this.Adapter.GetExternalItems(sExternalContentTypeNamespace, sExternalContentTypeName, sReadListOperation, sListID);
		}

		public string GetFields(string sListID, bool bGetAllAvailableFields)
		{
			return this.Adapter.GetFields(sListID, bGetAllAvailableFields);
		}

		public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
		{
			return this.Adapter.GetFiles(sFolderPath, itemTypes);
		}

		public string GetFolders(string sListID, string sIDs, string sParentFolder)
		{
			return this.Adapter.GetFolders(sListID, sIDs, sParentFolder);
		}

		public string GetGroups()
		{
			return this.Adapter.GetGroups();
		}

		public string GetLanguagesAndWebTemplates()
		{
			return this.Adapter.GetLanguagesAndWebTemplates();
		}

		public string GetList(string sListID)
		{
			return this.Adapter.GetList(sListID);
		}

		public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
		{
			return this.Adapter.GetListItemIDs(sListID, sParentFolder, bRecursive, itemTypes);
		}

		public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
		{
			return this.Adapter.GetListItems(sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings, getOptions);
		}

		public string GetListItemsByQuery(string listID, string fields, string query, string listSettings, GetListItemOptions getOptions)
		{
			return this.Adapter.GetListItemsByQuery(listID, fields, query, listSettings, getOptions);
		}

		public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
		{
			return this.Adapter.GetListItemVersions(sListID, iItemID, sFields, sListSettings);
		}

		public string GetLists()
		{
			return this.Adapter.GetLists();
		}

		public string GetListTemplates()
		{
			return this.Adapter.GetListTemplates();
		}

		public string GetMigrationJobStatus(string jobConfiguration)
		{
			return this.Adapter.GetMigrationJobStatus(jobConfiguration);
		}

		public string GetMySiteData(string sSiteURL)
		{
			return this.Adapter.GetMySiteData(sSiteURL);
		}

		public string GetPersonalSite(string email)
		{
			return this.Adapter.GetPersonalSite(email);
		}

		public string GetPortalListingGroups()
		{
			return this.Adapter.GetPortalListingGroups();
		}

		public string GetPortalListingIDs()
		{
			return this.Adapter.GetPortalListingIDs();
		}

		public string GetPortalListings(string sIDList)
		{
			return this.Adapter.GetPortalListings(sIDList);
		}

		public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
		{
			return this.Adapter.GetReferencedTaxonomyFullXml(sReferencedTaxonomyXml);
		}

		public string GetRoleAssignments(string sListID, int iItemId)
		{
			return this.Adapter.GetRoleAssignments(sListID, iItemId);
		}

		public string GetRoles(string sListId)
		{
			return this.Adapter.GetRoles(sListId);
		}

		public string GetServerHealth()
		{
			return this.Adapter.GetServerHealth();
		}

		public string GetSharePointVersion()
		{
			return this.Adapter.GetSharePointVersion();
		}

		public string GetSite(bool bFetchFullXml)
		{
			return this.Adapter.GetSite(bFetchFullXml);
		}

		public string GetSiteCollections()
		{
			return this.Adapter.GetSiteCollections();
		}

		public string GetSiteCollectionsOnWebApp(string sWebAppName)
		{
			return this.Adapter.GetSiteCollectionsOnWebApp(sWebAppName);
		}

		public string GetSiteQuotaTemplates()
		{
			return this.Adapter.GetSiteQuotaTemplates();
		}

		public string GetSiteUsers()
		{
			return this.Adapter.GetSiteUsers();
		}

		public string GetSP2013Workflows(string configurationXml)
		{
			return this.Adapter.GetSP2013Workflows(configurationXml);
		}

		public string GetStoragePointProfileConfiguration(string sSharePointPath)
		{
			return this.Adapter.GetStoragePointProfileConfiguration(sSharePointPath);
		}

		public string GetSubWebs()
		{
			return this.Adapter.GetSubWebs();
		}

		public string GetSystemInfo()
		{
			return this.Adapter.GetSystemInfo();
		}

		public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId, string sTermId)
		{
			return this.Adapter.GetTermCollectionFromTerm(sTermStoreId, sTermGroupId, sTermSetId, sTermId);
		}

		public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
		{
			return this.Adapter.GetTermCollectionFromTermSet(sTermStoreId, sTermGroupId, sTermSetId);
		}

		public string GetTermGroups(string sTermStoreId)
		{
			return this.Adapter.GetTermGroups(sTermStoreId);
		}

		public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
		{
			return this.Adapter.GetTermSetCollection(sTermStoreId, sTermGroupId);
		}

		public string GetTermSets(string sTermGroupId)
		{
			return this.Adapter.GetTermSets(sTermGroupId);
		}

		public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
		{
			return this.Adapter.GetTermsFromTermSet(sTermSetId, bRecursive);
		}

		public string GetTermsFromTermSetItem(string sTermSetItemId)
		{
			return this.Adapter.GetTermsFromTermSetItem(sTermSetItemId);
		}

		public string GetTermStores()
		{
			return this.Adapter.GetTermStores();
		}

		public string GetUserFromProfile()
		{
			return this.Adapter.GetUserFromProfile();
		}

		public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
		{
			return this.Adapter.GetUserProfiles(sSiteURL, sLoginName, out sErrors);
		}

		public string GetWeb(bool bFetchFullXml)
		{
			return this.Adapter.GetWeb(bFetchFullXml);
		}

		public string GetWebApplications()
		{
			return this.Adapter.GetWebApplications();
		}

		public string GetWebNavigationSettings()
		{
			return this.Adapter.GetWebNavigationSettings();
		}

		public string GetWebNavigationStructure()
		{
			return this.Adapter.GetWebNavigationStructure();
		}

		public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
		{
			return this.Adapter.GetWebPartPage(sWebPartPageServerRelativeUrl);
		}

		public byte[] GetWebPartPageTemplate(int iTemplateId)
		{
			return this.Adapter.GetWebPartPageTemplate(iTemplateId);
		}

		public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
		{
			return this.Adapter.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
		}

		public string GetWebTemplates()
		{
			return this.Adapter.GetWebTemplates();
		}

		public string GetWorkflowAssociations(string sObjectID, string sObjectScope)
		{
			return this.Adapter.GetWorkflowAssociations(sObjectID, sObjectScope);
		}

		public string GetWorkflows(string sListID, int iItemID)
		{
			return this.Adapter.GetWorkflows(sListID, iItemID);
		}

		public string HasDocument(string sDocumentServerRelativeUrl)
		{
			return this.Adapter.HasDocument(sDocumentServerRelativeUrl);
		}

		public bool HasPersonalSite(string email)
		{
			return this.Adapter.HasPersonalSite(email);
		}

		public string HasUniquePermissions(string listID, int listItemID)
		{
			return this.Adapter.HasUniquePermissions(listID, listItemID);
		}

		public string HasWebParts(string sWebPartPageServerRelativeUrl)
		{
			return this.Adapter.HasWebParts(sWebPartPageServerRelativeUrl);
		}

		public string HasWorkflows(string sListID, string sItemID)
		{
			return this.Adapter.HasWorkflows(sListID, sItemID);
		}

		public void InitializeAdapterConfiguration(string sXml)
		{
			lock (this.m_oLockAdapter)
			{
				if (this.m_adapter == null)
				{
					this.m_adapter = new CSOMAdapter();
					this.m_adapter.FromXML(XmlUtility.StringToXmlNode(sXml));
					Metalogix.SharePoint.Adapters.CSOM2013Service.CSOM2013Service.s_iRef++;
				}
			}
		}

		public string IsListContainsInfoPathOrAspxItem(string listId)
		{
			return this.Adapter.IsListContainsInfoPathOrAspxItem(listId);
		}

		public string IsWorkflowServicesInstanceAvailable()
		{
			return this.Adapter.IsWorkflowServicesInstanceAvailable();
		}

		public string MigrateSP2013Workflows(string configurationXml)
		{
			return this.Adapter.MigrateSP2013Workflows(configurationXml);
		}

		public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
		{
			return this.Adapter.ModifyWebNavigationSettings(sWebXML, ModNavOptions);
		}

		public Guid OpenFileCopySession(StreamType streamType, int retentionTime)
		{
			throw new NotImplementedException(Resources.Method_Not_Implemented);
		}

		public string ProvisionMigrationContainer()
		{
			return this.Adapter.ProvisionMigrationContainer();
		}

		public string ProvisionMigrationQueue()
		{
			return this.Adapter.ProvisionMigrationQueue();
		}

		public void ProvisionPersonalSites(string[] emails)
		{
			this.Adapter.ProvisionPersonalSites(emails);
		}

		public byte[] ReadChunk(Guid sessionId, long bytesToRead)
		{
			throw new NotImplementedException(Resources.Method_Not_Implemented);
		}

		public void RemovePersonalSite(string email)
		{
			this.Adapter.RemovePersonalSite(email);
		}

		public string ReorderContentTypes(string sListID, string[] sContentTypes)
		{
			return this.Adapter.ReorderContentTypes(sListID, sContentTypes);
		}

		public string RequestMigrationJob(string jobConfiguration, bool isMicrosoftCustomer, byte[] encryptionKey = null)
		{
			return this.Adapter.RequestMigrationJob(jobConfiguration, isMicrosoftCustomer, encryptionKey);
		}

		public string ResolvePrincipals(string principal)
		{
			return this.Adapter.ResolvePrincipals(principal);
		}

		public string SearchForDocument(string sSearchTerm, string sOptionsXml)
		{
			return this.Adapter.SearchForDocument(sSearchTerm, sOptionsXml);
		}

		public void SetCookieManagerCookies(IList<Cookie> cookies)
		{
			if (this.Adapter.CookieManager != null)
			{
				this.Adapter.CookieManager.SetCookies(cookies);
			}
		}

		public string SetDocumentParsing(bool bParserEnabled)
		{
			return this.Adapter.SetDocumentParsing(bParserEnabled);
		}

		public string SetMasterPage(string sWebXML)
		{
			return this.Adapter.SetMasterPage(sWebXML);
		}

		public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
		{
			return this.Adapter.SetUserProfile(sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound);
		}

		public string SetWelcomePage(string WelcomePage)
		{
			return this.Adapter.SetWelcomePage(WelcomePage);
		}

		public string StoragePointAvailable(string inputXml)
		{
			return this.Adapter.StoragePointAvailable(inputXml);
		}

		public string StoragePointProfileConfigured(string sSharePointPath)
		{
			return this.Adapter.StoragePointProfileConfigured(sSharePointPath);
		}

		public void UpdateCookieManagerCookies()
		{
			if (this.Adapter.HasActiveCookieManager)
			{
				this.Adapter.CookieManager.UpdateCookie();
			}
		}

		public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML, byte[] fileContents, UpdateDocumentOptions updateOptions)
		{
			return this.Adapter.UpdateDocument(sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, updateOptions);
		}

		public string UpdateGroupQuickLaunch(string value)
		{
			return this.Adapter.UpdateGroupQuickLaunch(value);
		}

		public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions, byte[] documentTemplateFile)
		{
			return this.Adapter.UpdateList(sListID, sListXML, sViewXml, updateOptions, documentTemplateFile);
		}

		public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML, string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions)
		{
			return this.Adapter.UpdateListItem(sListID, sParentFolder, iItemID, sListItemXML, attachmentNames, attachmentContents, updateOptions);
		}

		public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML, string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
		{
			return this.Adapter.UpdateListItemStatus(bPublish, bCheckin, bApprove, sItemXML, sListXML, sItemXML, sCheckinComment, sPublishComment, sApprovalComment);
		}

		public string UpdateSiteCollectionSettings(string sUpdateXml, UpdateSiteCollectionOptions updateSiteCollectionOptions)
		{
			return this.Adapter.UpdateSiteCollectionSettings(sUpdateXml, updateSiteCollectionOptions);
		}

		public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
		{
			return this.Adapter.UpdateWeb(sWebXML, updateOptions);
		}

		public string UpdateWebNavigationStructure(string sUpdateXml)
		{
			return this.Adapter.UpdateWebNavigationStructure(sUpdateXml);
		}

		public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
		{
			return this.Adapter.ValidateUserInfo(sUserIdentifier, bCanBeDomainGroup);
		}

		public void WriteChunk(Guid sessionId, byte[] data)
		{
			throw new NotImplementedException(Resources.Method_Not_Implemented);
		}
	}
}