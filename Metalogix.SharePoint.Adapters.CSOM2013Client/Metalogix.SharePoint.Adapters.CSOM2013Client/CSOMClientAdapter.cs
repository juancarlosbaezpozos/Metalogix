using Metalogix;
using Metalogix.Core;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.CSOM2013Client.Aspects;
using Metalogix.SharePoint.Adapters.CSOM2013Client.CSOM2013ServiceManager;
using Metalogix.SharePoint.Adapters.CSOM2013Client.CSOMService;
using Metalogix.SharePoint.Adapters.CSOM2013Client.Properties;
using Metalogix.SharePoint.Adapters.NWS;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.Utilities;
using Microsoft.Win32;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client
{
    [AdapterDisplayName("Remote Connection (SharePoint Client Side Object Model 2013)")]
    [AdapterShortName("CSOM")]
    [AdapterSupports(AdapterSupportedFlags.SiteScope | AdapterSupportedFlags.TenantScope |
                     AdapterSupportedFlags.CurrentLicense)]
    [MenuOrder(5)]
    [ShowInMenu(true)]
    public class CSOMClientAdapter : SharePointAdapter, ISharePointReader, ISharePointWriter, IServerHealthMonitor,
        IMySitesConnector, ISP2013WorkflowAdapter, IMigrationPipeline, ISharePointAdapterCommand
    {
        private const int SERVICE_STARTUP_WAIT_TIME = 60;

        private static List<SharePointAdapterServiceClient> s_maintainedServices;

        private static object s_oLockMaintainedServices;

        private static bool s_AppOwned;

        private static object s_oLockServiceProcess;

        private static bool? s_DotNet4Available;

        private static object s_lockDotNet4Available;

        private static bool? m_FipsClr2;

        private static object s_serviceManagerLock;

        private static ServiceManagerClient s_serviceManager;

        private string m_sUrl;

        private CSOMClientAdapter.ServerHealthString _serverHealthString;

        private Credentials m_credentials = new Credentials();

        private object m_oLockService = new object();

        private SharePointAdapterServiceClient m_service;

        private bool _lastServiceFetchFailed;

        private CSOMServiceCookieManager _serviceCookieManager;

        private bool? _supportsTaxonomy = null;

        private string m_sWebID;

        private string m_SharePointVersionString;

        private object _sharePointVersionLock = new object();

        public override CookieManager CookieManager
        {
            get
            {
                if (this._lastServiceFetchFailed)
                {
                    return base.CookieManager;
                }

                return this._serviceCookieManager;
            }
            set
            {
                if (value == null)
                {
                    this._serviceCookieManager = null;
                }
                else if (base.CookieManager != value)
                {
                    this._serviceCookieManager = new CSOMServiceCookieManager(this);
                    if (this.m_service != null && value.HasCookie)
                    {
                        this._serviceCookieManager.SetCookies(value.Cookies);
                        this.Service.SetCookieManagerCookies(value.Cookies);
                    }
                }

                base.CookieManager = value;
            }
        }

        public override Credentials Credentials
        {
            get { return this.m_credentials; }
            set { this.m_credentials = value; }
        }

        private static bool DotNet4Available
        {
            get
            {
                if (!CSOMClientAdapter.s_DotNet4Available.HasValue)
                {
                    lock (CSOMClientAdapter.s_lockDotNet4Available)
                    {
                        if (!CSOMClientAdapter.s_DotNet4Available.HasValue)
                        {
                            using (RegistryKey registryKey =
                                   Registry.LocalMachine.OpenSubKey(
                                       "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Client"))
                            {
                                if (registryKey == null)
                                {
                                    CSOMClientAdapter.s_DotNet4Available = new bool?(false);
                                }
                                else if ((int)registryKey.GetValue("Install", 0) == 1)
                                {
                                    CSOMClientAdapter.s_DotNet4Available = new bool?(true);
                                }
                            }
                        }
                    }
                }

                return CSOMClientAdapter.s_DotNet4Available.Value;
            }
        }

        public override ExternalizationSupport ExternalizationSupport
        {
            get { return ExternalizationSupport.NotSupported; }
        }

        public override bool IsClientOM
        {
            get { return true; }
        }

        public override bool IsPortal2003Connection
        {
            get { return false; }
        }

        public override ISharePointReader Reader
        {
            get { return SharePointReader.GetSharePointReader(this); }
        }

        public override string Server
        {
            get { return Utils.GetServerPart(this.Url); }
        }

        public override string ServerRelativeUrl
        {
            get { return CSOMClientAdapter.GetServerRelativeUrl(this.Url); }
        }

        public override string ServerType
        {
            get { return "Remote Connection"; }
        }

        public override string ServerUrl
        {
            get { return Utils.GetServerPart(this.Url); }
        }

        private SharePointAdapterServiceClient Service
        {
            get
            {
                lock (this.m_oLockService)
                {
                    if (this.m_service == null)
                    {
                        string channelXML = this.GetChannelXML();
                        SharePointAdapterServiceClient sharePointAdapterServiceClient = null;
                        try
                        {
                            sharePointAdapterServiceClient =
                                new SharePointAdapterServiceClient(BindingProperties.ServiceBindingData,
                                    BindingProperties.CSOMServiceAddress);
                            sharePointAdapterServiceClient.ChannelFactory.Endpoint.Behaviors.Add(new ClientBehaviour());
                            sharePointAdapterServiceClient.Open();
                            sharePointAdapterServiceClient.InnerChannel.Faulted +=
                                new EventHandler(this.InnerChannel_Faulted);
                            sharePointAdapterServiceClient.InitializeAdapterConfiguration(channelXML);
                        }
                        catch (EndpointNotFoundException endpointNotFoundException1)
                        {
                            EndpointNotFoundException endpointNotFoundException = endpointNotFoundException1;
                            Utils.LogExceptionDetails(endpointNotFoundException, MethodBase.GetCurrentMethod().Name,
                                MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                            if (!CSOMClientAdapter.EnsureServiceEndpointIsRunning())
                            {
                                this.m_service = null;
                                this._lastServiceFetchFailed = true;
                                throw new ServiceNotAvailableException();
                            }

                            sharePointAdapterServiceClient =
                                new SharePointAdapterServiceClient(BindingProperties.ServiceBindingData,
                                    BindingProperties.CSOMServiceAddress);
                            sharePointAdapterServiceClient.ChannelFactory.Endpoint.Behaviors.Add(new ClientBehaviour());
                            sharePointAdapterServiceClient.Open();
                            sharePointAdapterServiceClient.InnerChannel.Faulted +=
                                new EventHandler(this.InnerChannel_Faulted);
                            sharePointAdapterServiceClient.InitializeAdapterConfiguration(channelXML);
                        }

                        if (sharePointAdapterServiceClient != null)
                        {
                            lock (CSOMClientAdapter.s_oLockMaintainedServices)
                            {
                                CSOMClientAdapter.s_maintainedServices.Add(sharePointAdapterServiceClient);
                            }

                            if (base.CookieManager != null && base.CookieManager.HasCookie)
                            {
                                sharePointAdapterServiceClient.SetCookieManagerCookies(base.CookieManager.Cookies);
                            }

                            this.m_service = sharePointAdapterServiceClient;
                        }
                    }
                }

                return this.m_service;
            }
        }

        private static ServiceManagerClient ServiceManager
        {
            get
            {
                ServiceManagerClient sServiceManager;
                lock (CSOMClientAdapter.s_serviceManagerLock)
                {
                    if (CSOMClientAdapter.s_serviceManager == null)
                    {
                        CSOMClientAdapter.EnsureServiceEndpointIsRunning();
                    }

                    sServiceManager = CSOMClientAdapter.s_serviceManager;
                }

                return sServiceManager;
            }
        }

        public override bool SupportsTaxonomy
        {
            get
            {
                if (!this._supportsTaxonomy.HasValue)
                {
                    string referencedTaxonomyFullXml = this.Reader.GetReferencedTaxonomyFullXml(null);
                    this._supportsTaxonomy = new bool?(referencedTaxonomyFullXml != null);
                }

                return this._supportsTaxonomy.Value;
            }
        }

        public override bool SupportsWorkflows
        {
            get { return true; }
        }

        public override string Url
        {
            get { return this.m_sUrl; }
            set
            {
                if (value == null)
                {
                    throw new Exception("Invalid Url. Value cannot be null");
                }

                if (value.ToLower().StartsWith("http://") || value.ToLower().StartsWith("https://"))
                {
                    this.m_sUrl = value;
                    return;
                }

                this.m_sUrl = string.Concat(this.Server, CSOMClientAdapter.GetServerRelativeUrl(value));
            }
        }

        public override string WebID
        {
            get
            {
                if (this.m_sWebID == null)
                {
                    string web = this.Reader.GetWeb(false);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(web);
                    XmlNode xmlNodes = xmlDocument.SelectSingleNode("//Web/@ID");
                    if (xmlNodes != null)
                    {
                        this.m_sWebID = xmlNodes.Value;
                    }
                }

                return this.m_sWebID;
            }
            set { this.m_sWebID = value; }
        }

        public override ISharePointWriter Writer
        {
            get
            {
                if (base.IsReadOnly())
                {
                    return null;
                }

                return SharePointWriter.GetSharePointWriter(this);
            }
        }

        static CSOMClientAdapter()
        {
            CSOMClientAdapter.s_maintainedServices = new List<SharePointAdapterServiceClient>();
            CSOMClientAdapter.s_oLockMaintainedServices = new object();
            CSOMClientAdapter.s_AppOwned = false;
            CSOMClientAdapter.s_oLockServiceProcess = new object();
            CSOMClientAdapter.s_DotNet4Available = null;
            CSOMClientAdapter.s_lockDotNet4Available = new object();
            CSOMClientAdapter.s_serviceManagerLock = new object();
            CSOMClientAdapter.s_serviceManager = null;
        }

        public CSOMClientAdapter()
        {
            this._serverHealthString = new CSOMClientAdapter.ServerHealthString();
        }

        public CSOMClientAdapter(string sSiteUrl, Credentials credentials)
        {
            this.m_sUrl = sSiteUrl;
            this.m_credentials = credentials;
        }

        public static void EndServiceEndpoint()
        {
            ServiceManagerClient serviceManagerClient = new ServiceManagerClient(BindingProperties.ManagerBindingData,
                BindingProperties.ManagerAddress);
            try
            {
                CSOMClientAdapter.ReleaseServiceManager();
                serviceManagerClient.EndService();
                serviceManagerClient.Close();
            }
            catch (EndpointNotFoundException endpointNotFoundException1)
            {
                EndpointNotFoundException endpointNotFoundException = endpointNotFoundException1;
                Utils.LogExceptionDetails(endpointNotFoundException, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
            }
        }

        private static bool EnsureServiceEndpointIsRunning()
        {
            bool flag;
            if (LicensingUtils.GetLevel() != CompatibilityLevel.Current)
            {
                return false;
            }

            if (!CSOMClientAdapter.DotNet4Available)
            {
                return false;
            }

            if (CSOMClientAdapter.IsFipsEnabledAndEntryExEisOnClr2())
            {
                return false;
            }

            lock (CSOMClientAdapter.s_oLockServiceProcess)
            {
                try
                {
                    CSOMClientAdapter.s_serviceManager = new ServiceManagerClient(BindingProperties.ManagerBindingData,
                        BindingProperties.ManagerAddress);
                    CSOMClientAdapter.s_serviceManager.Open();
                }
                catch (EndpointNotFoundException endpointNotFoundException1)
                {
                    EndpointNotFoundException endpointNotFoundException = endpointNotFoundException1;
                    Utils.LogExceptionDetails(endpointNotFoundException, MethodBase.GetCurrentMethod().Name,
                        MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                    flag = CSOMClientAdapter.InstantiateServiceEndpoint(CSOMClientAdapter.s_AppOwned);
                    return flag;
                }

                return true;
            }

            return flag;
        }

        public static void EnsureServiceEndpointIsRunningForApplication()
        {
            CSOMClientAdapter.s_AppOwned = true;
            CSOMClientAdapter.EnsureServiceEndpointIsRunning();
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.ExecuteCommand(commandName, commandConfigurationXml); }, true);
            return returnValue;
        }

        public string FindAlerts()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.FindAlerts(); }, true);
            return returnValue;
        }

        public string FindUniquePermissions()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.FindUniquePermissions(); }, true);
            return returnValue;
        }

        public string GetAlerts(string sListID, int sItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetAlerts(sListID, sItemID); }, true);
            return returnValue;
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetAttachments(sListID, iItemID); }, true);
            return returnValue;
        }

        public string GetAudiences()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetAudiences(); }, true);
            return returnValue;
        }

        private string GetChannelXML()
        {
            if (base.CredentialsAreDefault)
            {
                return this.ToXML();
            }

            XmlNode xmlNode = XmlUtility.StringToXmlNode(this.ToXML());
            if (!this.Credentials.SavePassword)
            {
                XmlAttribute insecureString = xmlNode.OwnerDocument.CreateAttribute("Password");
                insecureString.Value = this.Credentials.Password.ToInsecureString();
                xmlNode.Attributes.Append(insecureString);
                XmlAttribute str = xmlNode.OwnerDocument.CreateAttribute("SavePassword");
                str.Value = this.Credentials.SavePassword.ToString();
                xmlNode.Attributes.Append(str);
            }

            return xmlNode.OuterXml;
        }

        public string GetContentTypes(string sListId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetContentTypes(sListId); }, true);
            return returnValue;
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetDashboardPageTemplate(iTemplateId); },
                true);
            return returnValue;
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetDocument(sDocID, sFileDirRef, sFileLeafRef, iLevel); }, true);
            return returnValue;
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetDocumentBlobRef(sDocID, sFileDirRef, sFileLeafRef, iLevel); },
                true);
            return returnValue;
        }

        public string GetDocumentId(string sDocUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetDocumentId(sDocUrl); }, true);
            return returnValue;
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetDocumentVersion(sDocID, sFileDirRef, sFileLeafRef, iVersion);
                }, true);
            return returnValue;
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetDocumentVersionBlobRef(sDocID, sFileDirRef, sFileLeafRef, iVersion);
                }, true);
            return returnValue;
        }

        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.GetExternalContentTypeOperations(sExternalContentTypeNamespace,
                            sExternalContentTypeName);
                }, true);
            return returnValue;
        }

        public string GetExternalContentTypes()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetExternalContentTypes(); }, true);
            return returnValue;
        }

        public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sReadListOperation, string sListID)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetExternalItems(sExternalContentTypeNamespace, sExternalContentTypeName,
                        sReadListOperation, sListID);
                }, true);
            return returnValue;
        }

        public string GetFields(string sListID, bool bGetAllAvailableFields)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetFields(sListID, bGetAllAvailableFields); },
                true);
            return returnValue;
        }

        public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetFiles(sFolderPath, itemTypes); }, true);
            return returnValue;
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetFolders(sListID, sIDs, sParentFolder); },
                true);
            return returnValue;
        }

        public string GetGroups()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetGroups(); }, true);
            return returnValue;
        }

        public string GetLanguagesAndWebTemplates()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetLanguagesAndWebTemplates(); }, true);
            return returnValue;
        }

        public string GetList(string sListID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetList(sListID); }, true);
            return returnValue;
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetListItemIDs(sListID, sParentFolder, bRecursive, itemTypes); },
                true);
            return returnValue;
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetListItems(sListID, sIDs, sFields, sParentFolder, bRecursive,
                        itemTypes, sListSettings, getOptions);
                }, true);
            return returnValue;
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetListItemsByQuery(listID, fields, query, listSettings, getOptions);
                }, true);
            return returnValue;
        }

        public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetListItemVersions(sListID, iItemID, sFields, sListSettings); },
                true);
            return returnValue;
        }

        public string GetLists()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetLists(); }, true);
            return returnValue;
        }

        public string GetListTemplates()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetListTemplates(); }, true);
            return returnValue;
        }

        public string GetMigrationJobStatus(string jobConfiguration)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetMigrationJobStatus(jobConfiguration); },
                true);
            return returnValue;
        }

        public string GetMySiteData(string sSiteURL)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetMySiteData(sSiteURL); }, true);
            return returnValue;
        }

        public string GetPersonalSite(string email)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetPersonalSite(email); }, true);
            return returnValue;
        }

        public string GetPortalListingGroups()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetPortalListingGroups(); }, true);
            return returnValue;
        }

        public string GetPortalListingIDs()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetPortalListingIDs(); }, true);
            return returnValue;
        }

        public string GetPortalListings(string sIDList)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetPortalListings(sIDList); }, true);
            return returnValue;
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetReferencedTaxonomyFullXml(sReferencedTaxonomyXml); }, true);
            return returnValue;
        }

        public string GetRoleAssignments(string sListID, int iItemId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetRoleAssignments(sListID, iItemId); }, true);
            return returnValue;
        }

        public string GetRoles(string sListId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetRoles(sListId); }, true);
            return returnValue;
        }

        public string GetServerHealth()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetServerHealth(); }, true);
            return returnValue;
        }

        private static string GetServerRelativeUrl(string sUrl)
        {
            if (!sUrl.ToLower().StartsWith("http://") && !sUrl.ToLower().StartsWith("https://"))
            {
                return sUrl;
            }

            int index = sUrl.IndexOf("/", 8);
            if (index < 0)
            {
                return "";
            }

            return sUrl.Substring(index);
        }

        public override string GetServerVersion()
        {
            return this.GetSharePointVersion();
        }

        public IList<Cookie> GetServiceCookieManagerCookies()
        {
            IList<Cookie> returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetCookieManagerCookies(); }, false);
            return returnValue;
        }

        public bool GetServiceCookieManagerIsActive()
        {
            bool returnValue = false;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetCookieManagerIsActive(); }, false);
            return returnValue;
        }

        public bool GetServiceCookieManagerLocksCookies()
        {
            bool returnValue = false;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetCookieManagerLocksCookies(); }, false);
            return returnValue;
        }

        public static Cookie GetSharePointOnlineCookie(string url, string userName, string password)
        {
            string returnValue = null;
            CallStaticServiceMethod(delegate
            {
                returnValue = ServiceManager.GetSharePointOnlineCookie(url, userName, password);
            });
            return AuthenticationUtilities.MakeCookie(returnValue, url);
        }

        public string GetSharePointVersion()
        {
            if (this.m_SharePointVersionString == null)
            {
                lock (this._sharePointVersionLock)
                {
                    if (this.m_SharePointVersionString == null)
                    {
                        try
                        {
                            this.m_SharePointVersionString = this.Service.GetSharePointVersion();
                        }
                        catch (ServiceNotAvailableException serviceNotAvailableException1)
                        {
                            ServiceNotAvailableException serviceNotAvailableException = serviceNotAvailableException1;
                            Utils.LogExceptionDetails(serviceNotAvailableException, MethodBase.GetCurrentMethod().Name,
                                MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                            this.m_SharePointVersionString = NWSAdapter.GetVersionStringFromRPC(this);
                        }
                    }
                }
            }

            return this.m_SharePointVersionString;
        }

        public string GetSite(bool bFetchFullXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSite(bFetchFullXml); }, true);
            return returnValue;
        }

        public string GetSiteCollections()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSiteCollections(); }, true);
            return returnValue;
        }

        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSiteCollectionsOnWebApp(sWebAppName); },
                true);
            return returnValue;
        }

        public string GetSiteQuotaTemplates()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSiteQuotaTemplates(); }, true);
            return returnValue;
        }

        public string GetSiteUsers()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSiteUsers(); }, true);
            return returnValue;
        }

        public string GetSP2013Workflows(string configurationXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSP2013Workflows(configurationXml); }, true);
            return returnValue;
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetStoragePointProfileConfiguration(sSharePointPath); }, true);
            return returnValue;
        }

        public string GetSubWebs()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSubWebs(); }, true);
            return returnValue;
        }

        public string GetSystemInfo()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetSystemInfo(); }, true);
            return returnValue;
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.GetTermCollectionFromTerm(sTermStoreId, sTermGroupId, sTermSetId, sTermId);
                }, true);
            return returnValue;
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.GetTermCollectionFromTermSet(sTermStoreId, sTermGroupId, sTermSetId);
                }, true);
            return returnValue;
        }

        public string GetTermGroups(string sTermStoreId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetTermGroups(sTermStoreId); }, true);
            return returnValue;
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetTermSetCollection(sTermStoreId, sTermGroupId); }, true);
            return returnValue;
        }

        public string GetTermSets(string sTermGroupId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetTermSets(sTermGroupId); }, true);
            return returnValue;
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetTermsFromTermSet(sTermSetId, bRecursive); },
                true);
            return returnValue;
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetTermsFromTermSetItem(sTermSetItemId); },
                true);
            return returnValue;
        }

        public string GetTermStores()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetTermStores(); }, true);
            return returnValue;
        }

        public string GetUserFromProfile()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetUserFromProfile(); }, true);
            return returnValue;
        }

        public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
        {
            string returnValue = null;
            string substituteOutErrors = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetUserProfiles(out substituteOutErrors, sSiteURL, sLoginName); },
                true);
            sErrors = substituteOutErrors;
            return returnValue;
        }

        public string GetWeb(bool bFetchFullXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWeb(bFetchFullXml); }, true);
            return returnValue;
        }

        public string GetWebApplications()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWebApplications(); }, true);
            return returnValue;
        }

        public string GetWebNavigationSettings()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWebNavigationSettings(); }, true);
            return returnValue;
        }

        public string GetWebNavigationStructure()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWebNavigationStructure(); }, true);
            return returnValue;
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetWebPartPage(sWebPartPageServerRelativeUrl); }, true);
            return returnValue;
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            byte[] returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWebPartPageTemplate(iTemplateId); }, true);
            return returnValue;
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetWebPartsOnPage(sWebPartPageServerRelativeUrl); }, true);
            return returnValue;
        }

        public string GetWebTemplates()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWebTemplates(); }, true);
            return returnValue;
        }

        public string GetWorkflowAssociations(string sObjectID, string sObjectScope)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.GetWorkflowAssociations(sObjectID, sObjectScope); }, true);
            return returnValue;
        }

        public string GetWorkflows(string sListID, int iItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.GetWorkflows(sListID, iItemID); }, true);
            return returnValue;
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.HasDocument(sDocumentServerRelativeUrl); },
                true);
            return returnValue;
        }

        public bool HasPersonalSite(string email)
        {
            bool returnValue = false;
            this.CallServiceMethod(delegate { returnValue = this.Service.HasPersonalSite(email); }, true);
            return returnValue;
        }

        public string HasUniquePermissions(string listID, int listItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.HasUniquePermissions(listID, listItemID); },
                true);
            return returnValue;
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.HasWebParts(sWebPartPageServerRelativeUrl); },
                true);
            return returnValue;
        }

        public string HasWorkflows(string sListID, string sItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.HasWorkflows(sListID, sItemID); }, true);
            return returnValue;
        }

        private void InnerChannel_Faulted(object sender, EventArgs e)
        {
        }

        private static bool InstantiateServiceEndpoint(bool bRequireManualShutdown)
        {
            bool flag;
            if (!CSOMClientAdapter.DotNet4Available)
            {
                return false;
            }

            bool flag1 = false;
            DirectoryInfo parent = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent;
            DirectoryInfo[] catalogDirectories = Catalogs.CatalogDirectories;
            if (!catalogDirectories.Any<DirectoryInfo>((DirectoryInfo dir) => dir.FullName == parent.FullName))
            {
                catalogDirectories = (new List<DirectoryInfo>(catalogDirectories)
                {
                    parent
                }).ToArray();
            }

            FileInfo fileInfo = null;
            DirectoryInfo[] directoryInfoArray = Catalogs.CatalogDirectories;
            int num = 0;
            while (num < (int)directoryInfoArray.Length)
            {
                DirectoryInfo directoryInfo = directoryInfoArray[num];
                fileInfo = new FileInfo(string.Concat(directoryInfo.FullName,
                    "\\CSOM2013Service\\Metalogix.SharePoint.Adapters.CSOM2013Service.exe"));
                if (!fileInfo.Exists)
                {
                    num++;
                }
                else
                {
                    flag1 = true;
                    break;
                }
            }

            if (!flag1)
            {
                return false;
            }

            using (Process process = new Process())
            {
                process.StartInfo.WorkingDirectory = fileInfo.DirectoryName;
                process.StartInfo.FileName = fileInfo.Name;
                process.Start();
                bool flag2 = false;
                int num1 = 0;
                while (!flag2 && !process.HasExited && num1 < 60)
                {
                    num1++;
                    flag2 = process.WaitForInputIdle(1000);
                }

                if (!flag2)
                {
                    flag = false;
                    return flag;
                }
                else if (bRequireManualShutdown)
                {
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(CSOMClientAdapter.ProcessExiting);
                }
            }

            try
            {
                CSOMClientAdapter.s_serviceManager = new ServiceManagerClient(BindingProperties.ManagerBindingData,
                    BindingProperties.ManagerAddress);
                CSOMClientAdapter.s_serviceManager.Open();
                CSOMClientAdapter.s_serviceManager.InitializeService(
                    ConfigurationVariables.GetSerializedConfigurationVariables(ResourceScope.UserSpecific |
                                                                               ResourceScope.EnvironmentSpecific),
                    bRequireManualShutdown);
                return true;
            }
            catch (EndpointNotFoundException endpointNotFoundException1)
            {
                EndpointNotFoundException endpointNotFoundException = endpointNotFoundException1;
                Utils.LogExceptionDetails(endpointNotFoundException, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                flag = false;
            }

            return flag;
        }

        public static bool IsFipsEnabledAndEntryExEisOnClr2()
        {
            object value;
            if (CSOMClientAdapter.m_FipsClr2.HasValue)
            {
                return CSOMClientAdapter.m_FipsClr2.Value;
            }

            try
            {
                using (RegistryKey registryKey =
                       Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Lsa\\FipsAlgorithmPolicy"))
                {
                    value = registryKey.GetValue("Enabled");
                }

                if (value != null)
                {
                    CSOMClientAdapter.m_FipsClr2 = new bool?(((int)value != 1 ? false : Environment.Version.Major < 4));
                }
                else
                {
                    CSOMClientAdapter.m_FipsClr2 = new bool?(false);
                    return CSOMClientAdapter.m_FipsClr2.Value;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Utils.LogExceptionDetails(exception, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                CSOMClientAdapter.m_FipsClr2 = new bool?(false);
            }

            return CSOMClientAdapter.m_FipsClr2.Value;
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.IsListContainsInfoPathOrAspxItem(listId); },
                true);
            return returnValue;
        }

        public string IsWorkflowServicesInstanceAvailable()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.IsWorkflowServicesInstanceAvailable(); },
                true);
            return returnValue;
        }

        public string MigrateSP2013Workflows(string configurationXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.MigrateSP2013Workflows(configurationXml); },
                true);
            return returnValue;
        }

        public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.ModifyWebNavigationSettings(sWebXML, ModNavOptions); }, true);
            return returnValue;
        }

        private static void ProcessExiting(object sender, EventArgs e)
        {
            CSOMClientAdapter.EndServiceEndpoint();
        }

        public string ProvisionMigrationContainer()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.ProvisionMigrationContainer(); }, true);
            return returnValue;
        }

        public string ProvisionMigrationQueue()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.ProvisionMigrationQueue(); }, true);
            return returnValue;
        }

        public void ProvisionPersonalSites(string[] emails)
        {
            this.CallServiceMethod(() => this.Service.ProvisionPersonalSites(emails), true);
        }

        //**************
        private void CallServiceMethod(ServiceMethodDelegate serviceMethod, bool useCookieLock = true)
        {
            if ((useCookieLock && base.HasActiveCookieManager) && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                bool flag = false;

                try
                {
                    serviceMethod();
                }
                catch (CommunicationException)
                {
                    flag = true;
                }

                if (flag)
                {
                    this.ReleaseService();
                    serviceMethod();
                }
            }
            finally
            {
                if ((useCookieLock && base.HasActiveCookieManager) && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }
        }

        private static void CallStaticServiceMethod(ServiceMethodDelegate serviceMethod)
        {
            bool flag = false;

            try
            {
                serviceMethod();
            }
            catch (CommunicationException)
            {
                flag = true;
            }

            if (flag)
            {
                ReleaseServiceManager();
                serviceMethod();
            }
        }

        internal void ReleaseService()
        {
            lock (this.m_oLockService)
            {
                if (this.m_service != null)
                {
                    try
                    {
                        if (this.m_service.InnerChannel.State == CommunicationState.Opened)
                        {
                            try
                            {
                                this.m_service.EndAdapterService();
                                this.m_service.Close();
                            }
                            catch (CommunicationException)
                            {
                                this.m_service.Abort();
                            }
                            catch (TimeoutException)
                            {
                                this.m_service.Abort();
                            }
                        }
                    }
                    catch
                    {
                        this.m_service.Abort();
                        throw;
                    }
                    finally
                    {
                        lock (s_oLockMaintainedServices)
                        {
                            if (s_maintainedServices.Contains(this.m_service))
                            {
                                s_maintainedServices.Remove(this.m_service);
                            }
                        }

                        this.m_service.InnerChannel.Faulted -= new EventHandler(this.InnerChannel_Faulted);
                        this.m_service = null;
                    }
                }
            }
        }

        public static void ReleaseServiceManager()
        {
            lock (s_oLockServiceProcess)
            {
                if (s_serviceManager != null)
                {
                    try
                    {
                        if (s_serviceManager.InnerChannel.State == CommunicationState.Opened)
                        {
                            try
                            {
                                s_serviceManager.Close();
                            }
                            catch (CommunicationException)
                            {
                                s_serviceManager.Abort();
                            }
                            catch (TimeoutException)
                            {
                                s_serviceManager.Abort();
                            }
                        }
                    }
                    catch
                    {
                        s_serviceManager.Abort();
                        throw;
                    }
                    finally
                    {
                        s_serviceManager = null;
                    }
                }
            }
        }

        //********************

        public string ActivateReusableWorkflowTemplates()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.ActivateReusableWorkflowTemplates(); }, true);
            return returnValue;
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddAlerts(sSiteUrl, sWebId, sAlertXML); },
                true);
            return returnValue;
        }

        public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddDocument(sListID, sParentFolder, sListItemXML, fileContents,
                        listSettingsXml, Options);
                }, true);
            return returnValue;
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            return this.Service.AddDocumentOptimistically(listId, listName, folderPath, fileXml, fileContents, options,
                ref fieldsLookupCache);
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddDocumentSetVersions(listName, listItemID, updatedTargetMetaInfo);
                }, true);
            return returnValue;
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddDocumentTemplatetoContentType(docTemplate, cTypeXml, url); },
                true);
            return returnValue;
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddFields(sListID, sFieldXML); }, true);
            return returnValue;
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddFileToFolder(sFileXML, fileContents, Options); }, true);
            return returnValue;
        }

        public string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddFolder(sListID, sParentFolder, sFolderXML, Options); }, true);
            return returnValue;
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            return this.Service.AddFolderOptimistically(listId, listName, folderPath, folderXml, options,
                ref fieldsLookupCache);
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddFolderToFolder(sFolderXML); }, true);
            return returnValue;
        }

        public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate
            {
                returnValue =
                    this.Service.AddFormTemplateToContentType(targetListId, docTemplate, cTypeXml, changedLookupFields);
            }, true);
            return returnValue;
        }

        public string AddList(string sListXML, AddListOptions Options, byte[] documentTemplateFile)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddList(sListXML, Options, documentTemplateFile); }, true);
            return returnValue;
        }

        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachmentNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddListItem(sListID, sParentFolder, sListItemXML, attachmentNames,
                        attachmentContents, listSettingsXml, Options);
                }, true);
            return returnValue;
        }

        public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddOrUpdateAudience(sAudienceXml, options); },
                true);
            return returnValue;
        }

        public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddOrUpdateContentType(sContentTypeXML, sParentContentTypeName);
                }, true);
            return returnValue;
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddOrUpdateGroup(sGroupXml); }, true);
            return returnValue;
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddOrUpdateRole(sName, sDescription, lPermissionMask); }, true);
            return returnValue;
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddReferencedTaxonomyData(sReferencedTaxonomyXML); }, true);
            return returnValue;
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddReusedTerms(sTargetTermStoreGuid, sParentTermCollectionXML); },
                true);
            return returnValue;
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
                }, true);
            return returnValue;
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions addSiteCollOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddSiteCollection(sWebApp, sSiteCollectionXML, addSiteCollOptions);
                }, true);
            return returnValue;
        }

        public string AddSiteUser(string sUserXML, AddUserOptions options)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddSiteUser(sUserXML, options); }, true);
            return returnValue;
        }

        public string AddTerm(string termXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddTerm(termXml); }, true);
            return returnValue;
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.AddTermGroup(targetTermStoreGuid, termGroupXml, includeGroupXmlInResult);
                }, true);
            return returnValue;
        }

        public string AddTermSet(string termSetXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddTermSet(termSetXml); }, true);
            return returnValue;
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddTermstoreLanguages(sTargetTermStoreGuid, sLangaugesXML); },
                true);
            return returnValue;
        }

        public string AddView(string sListID, string sViewXML)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddView(sListID, sViewXML); }, true);
            return returnValue;
        }

        public string AddWeb(string sWebXML, AddWebOptions addOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddWeb(sWebXML, addOptions); }, true);
            return returnValue;
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate
            {
                returnValue = this.Service.AddWebParts(sWebPartsXml, sWebPartPageServerRelativeUrl,
                    sEmbeddedHtmlContent);
            }, true);
            return returnValue;
        }

        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.AddWorkflow(sListId, sWorkflowXml); }, true);
            return returnValue;
        }

        public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AddWorkflowAssociation(sListId, sWorkflowXml, bAllowDBWriting); },
                true);
            return returnValue;
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.AnalyzeChurn(pivotDate, sListID, iItemID, bRecursive); }, true);
            return returnValue;
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.Apply2013Theme(colorPaletteUrl, spFontUrl, bgImageUrl); }, true);
            return returnValue;
        }

        public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.ApplyOrUpdateContentType(sListId, sContentTypeName, sFieldXML,
                        bMakeDefaultContentType);
                }, true);
            return returnValue;
        }

        public string BeginCompilingAllAudiences()
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.BeginCompilingAllAudiences(); }, true);
            return returnValue;
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, AddDocumentOptions options)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.CatalogDocumentToStoragePointFileShareEndpoint(sNetworkPath, sListID, sFolder,
                            sListItemXml, options);
                }, true);
            return returnValue;
        }

        public override void CheckConnection()
        {
            string str;
            string str1;
            if (LicensingUtils.GetLevel() == CompatibilityLevel.Legacy)
            {
                throw new MLLicenseException(Metalogix.SharePoint.Adapters.Properties.Resources
                    .SharePoint_Not_Supported_By_License);
            }

            if (CSOMClientAdapter.IsFipsEnabledAndEntryExEisOnClr2())
            {
                throw new NotSupportedException(
                    "Cannot connect using Client Side Object Model Adapter as underlying framework is not compliant with Federal Information Processing Standard (FIPS) enabled on this machine");
            }

            if (!base.CredentialsAreDefault && this.Credentials.Password.IsNullOrEmpty())
            {
                throw new UnauthorizedAccessException("A password is required");
            }

            try
            {
                this.ReleaseService();
                this.Service.CheckConnection();
                if (this.ConnectionScope == ConnectionScope.Tenant)
                {
                    this.Service.GetWebApplications();
                }

                string adapterConfiguration = this.Service.GetAdapterConfiguration();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(adapterConfiguration);
                this.Url = xmlDocument.DocumentElement.Attributes["Url"].Value;
                this.ServerAdapterConfiguration.Load();
            }
            catch (ServiceNotAvailableException serviceNotAvailableException1)
            {
                ServiceNotAvailableException serviceNotAvailableException = serviceNotAvailableException1;
                Utils.LogExceptionDetails(serviceNotAvailableException, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                try
                {
                    WebServiceUtils.TestRemoteSharepoint(this, out str, out str1);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    Utils.LogExceptionDetails(exception, MethodBase.GetCurrentMethod().Name,
                        MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                    if (exception.Message.Contains("timed out") || exception.Message.Contains("resolved"))
                    {
                        throw new ServerProblem(exception.Message);
                    }

                    string[] url = new string[]
                    {
                        "Could not connect to remote SharePoint server: '", this.Url, "'. ", exception.Message, ": ",
                        exception.StackTrace
                    };
                    throw new NoSharePoint(string.Concat(url));
                }

                if (!base.SharePointVersion.IsSharePoint2013OrLater)
                {
                    if (base.SharePointVersion.VersionNumberString != "0.0.0.0")
                    {
                        throw new NotSupportedException(Metalogix.SharePoint.Adapters.CSOM2013Client.Properties
                            .Resources.Older_SharePoint_Detected);
                    }

                    throw new NoSharePoint(string.Concat("Could not find site on the remote SharePoint server: '",
                        this.Url, "'. "));
                }

                throw;
            }
        }

        public override SharePointAdapter Clone()
        {
            CSOMClientAdapter cSOMClientAdapter = new CSOMClientAdapter();
            cSOMClientAdapter.CloneFrom(this, true);
            return cSOMClientAdapter;
        }

        public override SharePointAdapter CloneForNewSiteCollection()
        {
            CSOMClientAdapter cSOMClientAdapter = new CSOMClientAdapter();
            cSOMClientAdapter.CloneFrom(this, false);
            return cSOMClientAdapter;
        }

        public void CloneFrom(CSOMClientAdapter newAdapter, bool bIncludeSiteCollectionSpecificProperties)
        {
            this.IsReadOnlyAdapter = newAdapter.IsReadOnlyAdapter;
            base.IsDataLimitExceededForContentUnderMgmt = newAdapter.IsDataLimitExceededForContentUnderMgmt;
            this.m_sUrl = newAdapter.m_sUrl;
            this.m_credentials = newAdapter.m_credentials;
            base.AzureAdGraphCredentials = newAdapter.AzureAdGraphCredentials;
            this.AdapterProxy = newAdapter.AdapterProxy;
            this.IncludedCertificates = newAdapter.IncludedCertificates;
            if (bIncludeSiteCollectionSpecificProperties)
            {
                this.AuthenticationInitializer = newAdapter.AuthenticationInitializer;
                if (newAdapter.HasActiveCookieManager)
                {
                    newAdapter.CookieManager.EnsureCookies();
                }

                this.CookieManager = newAdapter.CookieManager;
            }

            base.SetSystemInfo(newAdapter.SystemInfo.Clone());
            base.SetSharePointVersion(newAdapter.SharePointVersion.Clone());
            this._serverHealthString = newAdapter._serverHealthString;
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.CloseWebParts(sWebPartPageServerRelativeUrl); }, true);
            return returnValue;
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.ConfigureStoragePointFileShareEndpointAndProfile(sNetworkPath, sSharePointPath);
                }, true);
            return returnValue;
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.CorrectDefaultPageVersions(sListID, sFolder, sListItemXML); },
                true);
            return returnValue;
        }

        public string DeleteAllAudiences(string inputXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteAllAudiences(inputXml); }, true);
            return returnValue;
        }

        public string DeleteAudience(string sAudienceName)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteAudience(sAudienceName); }, true);
            return returnValue;
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteContentTypes(sListID, contentTypeIDs); },
                true);
            return returnValue;
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteFolder(sListID, iListItemID, sFolder); },
                true);
            return returnValue;
        }

        public string DeleteGroup(string sGroupName)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteGroup(sGroupName); }, true);
            return returnValue;
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteItem(sListID, iListItemID); }, true);
            return returnValue;
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteItems(sListID, bDeleteAllItems, sIDs); },
                true);
            return returnValue;
        }

        public string DeleteList(string sListID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteList(sListID); }, true);
            return returnValue;
        }

        public string DeleteMigrationJob(string jobConfiguration)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteMigrationJob(jobConfiguration); }, true);
            return returnValue;
        }

        public string DeleteRole(string sRoleName)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteRole(sRoleName); }, true);
            return returnValue;
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.DeleteRoleAssignment(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
                }, true);
            return returnValue;
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteSiteCollection(sSiteURL, sWebApp); },
                true);
            return returnValue;
        }

        public string DeleteSP2013Workflows(string configurationXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteSP2013Workflows(configurationXml); },
                true);
            return returnValue;
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DeleteWeb(sServerRelativeUrl); }, true);
            return returnValue;
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.DeleteWebPart(sWebPartPageServerRelativeUrl, sWebPartId);
                }, true);
            return returnValue;
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.DeleteWebParts(sWebPartPageServerRelativeUrl); }, true);
            return returnValue;
        }

        public string DisableValidationSettings(string listID)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.DisableValidationSettings(listID); }, true);
            return returnValue;
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.EnableValidationSettings(validationNodeFieldsXml); }, true);
            return returnValue;
        }


        public void RemovePersonalSite(string email)
        {
            this.CallServiceMethod(() => this.Service.RemovePersonalSite(email), true);
        }

        public string ReorderContentTypes(string sListID, string[] sContentTypes)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.ReorderContentTypes(sListID, sContentTypes); },
                true);
            return returnValue;
        }

        public string RequestMigrationJob(string jobConfiguration, bool isMicrosoftCustomer,
            byte[] encryptionKey = null)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue =
                        this.Service.RequestMigrationJob(jobConfiguration, isMicrosoftCustomer, encryptionKey);
                }, true);
            return returnValue;
        }

        public string ResolvePrincipals(string principal)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.ResolvePrincipals(principal); }, true);
            return returnValue;
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.SearchForDocument(sSearchTerm, sOptionsXml); },
                true);
            return returnValue;
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.SetDocumentParsing(bParserEnabled); }, true);
            return returnValue;
        }

        public string SetMasterPage(string sWebXML)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.SetMasterPage(sWebXML); }, true);
            return returnValue;
        }

        public void SetServiceCookieManagerCookies(IList<Cookie> cookies)
        {
            this.CallServiceMethod(() => this.Service.SetCookieManagerCookies(cookies), true);
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.SetUserProfile(sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound);
                }, true);
            return returnValue;
        }

        public string SetWelcomePage(string WelcomePage)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.SetWelcomePage(WelcomePage); }, true);
            return returnValue;
        }

        public string StoragePointAvailable(string inputXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.StoragePointAvailable(inputXml); }, true);
            return returnValue;
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.StoragePointProfileConfigured(sSharePointPath); }, true);
            return returnValue;
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate
            {
                returnValue = this.Service.UpdateDocument(sListID, sParentFolder, sFileLeafRef, sListItemXML,
                    fileContents, updateOptions);
            }, true);
            return returnValue;
        }

        public string UpdateGroupQuickLaunch(string value)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.UpdateGroupQuickLaunch(value); }, true);
            return returnValue;
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.UpdateList(sListID, sListXML, sViewXml, updateOptions,
                        documentTemplateFile);
                }, true);
            return returnValue;
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate
            {
                returnValue = this.Service.UpdateListItem(sListID, sParentFolder, iItemID, sListItemXML,
                    attachmentNames, attachmentContents, updateOptions);
            }, true);
            return returnValue;
        }

        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate
            {
                returnValue = this.Service.UpdateListItemStatus(bPublish, bCheckin, bApprove, sItemXML, sListXML,
                    sItemID, sCheckinComment, sPublishComment, sApprovalComment);
            }, true);
            return returnValue;
        }

        public void UpdateServiceCookieManagerCookies()
        {
            this.CallServiceMethod(() => this.Service.UpdateCookieManagerCookies(), true);
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate
                {
                    returnValue = this.Service.UpdateSiteCollectionSettings(sUpdateXml, updateSiteCollectionOptions);
                }, true);
            return returnValue;
        }

        public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.UpdateWeb(sWebXML, updateOptions); }, true);
            return returnValue;
        }

        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            string returnValue = null;
            this.CallServiceMethod(delegate { returnValue = this.Service.UpdateWebNavigationStructure(sUpdateXml); },
                true);
            return returnValue;
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            string returnValue = null;
            this.CallServiceMethod(
                delegate { returnValue = this.Service.ValidateUserInfo(sUserIdentifier, bCanBeDomainGroup); }, true);
            return returnValue;
        }

        //********************

        protected override void Dispose(bool bDisposing)
        {
            base.Dispose(bDisposing);
            if (bDisposing)
            {
                this.ReleaseService();
            }
        }

        private class ServerHealthString
        {
            public string XmlString { get; set; }

            public ServerHealthString()
            {
            }
        }

        private delegate void ServiceMethodDelegate();
    }
}