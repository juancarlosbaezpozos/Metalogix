using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.Expert;
using Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.MLWS
{
    [AdapterDisplayName("Remote Connection (Metalogix SharePoint Extensions Web Service)")]
    [AdapterShortName("WS")]
    [AdapterSupports(AdapterSupportedFlags.SiteScope | AdapterSupportedFlags.WebAppScope |
                     AdapterSupportedFlags.FarmScope | AdapterSupportedFlags.LegacyLicense |
                     AdapterSupportedFlags.CurrentLicense)]
    [MenuOrder(3)]
    [ShowInMenu(true)]
    public class MLWSAdapter : SharePointAdapter, ISharePointReader, ISharePointWriter, IBinaryTransferHandler,
        ISP2013WorkflowAdapter, ISharePointAdapterCommand, IMigrationExpertReports
    {
        private MLSPExtensionsWebService m_webService;

        private string m_sUrl;

        private Metalogix.Permissions.Credentials m_credentials = new Metalogix.Permissions.Credentials();

        private string m_sWebID;

        private static Version s_assemblyVersion;

        private string m_sVersionString;

        private bool m_bDisposed;

        protected static Version AssemblyVersion
        {
            get
            {
                if (MLWSAdapter.s_assemblyVersion == null)
                {
                    MLWSAdapter.s_assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                }

                return MLWSAdapter.s_assemblyVersion;
            }
        }

        public override string ConnectionTypeDisplayString
        {
            get { return "Remote Connection (Metalogix SharePoint Extensions Web Service)"; }
        }

        public override Metalogix.Permissions.Credentials Credentials
        {
            get { return this.m_credentials; }
            set
            {
                this.m_credentials = value;
                this.m_webService = null;
                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.ClearCookie();
                }
            }
        }

        private static string DefaultHostName
        {
            get { return Utils.GetServerPart((new MLSPExtensionsWebService()).Url); }
        }

        public override Metalogix.SharePoint.Adapters.ExternalizationSupport ExternalizationSupport
        {
            get { return Metalogix.SharePoint.Adapters.ExternalizationSupport.Supported; }
        }

        public override bool IsMEWS
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
            get { return Utils.GetServerRelativeUrlPart(this.Url); }
        }

        public override string ServerType
        {
            get { return "Remote Connection"; }
        }

        public override string ServerUrl
        {
            get { return Utils.GetServerPart(this.Url); }
        }

        public override bool SupportsTaxonomy
        {
            get { return base.SystemInfo.HasTaxonomySupport; }
        }

        public override bool SupportsWorkflows
        {
            get { return true; }
        }

        public override string Url
        {
            get { return this.m_sUrl; }
            set { this.SetUrl(value); }
        }

        protected string VersionString
        {
            get
            {
                if (this.m_sVersionString == null)
                {
                    string str = (AdapterConfigurationVariables.ConfiguredWebServiceVersions.ContainsKey(this.Server)
                        ? this.Server
                        : "default");
                    string item = AdapterConfigurationVariables.ConfiguredWebServiceVersions[str];
                    Version version = new Version(item);
                    if (version.Major != MLWSAdapter.AssemblyVersion.Major ||
                        version.Minor != MLWSAdapter.AssemblyVersion.Minor)
                    {
                        string str1 =
                            string.Format(
                                "The web service version specified in the configuration file is incompatible.{0}    Configuration Entry: {1}{0}    Configured Version: {2}{0}",
                                Environment.NewLine, str, item);
                        throw new Exception(str1);
                    }

                    this.m_sVersionString = item;
                }

                return this.m_sVersionString;
            }
            set
            {
                this.m_sVersionString = value;
                this.m_webService = null;
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

        private MLSPExtensionsWebService WebService
        {
            get
            {
                if (this.m_webService == null)
                {
                    this.m_webService = new MLSPExtensionsWebService();
                    string str =
                        ((this.Url == null
                            ? this.m_webService.Url
                            : this.m_webService.Url.Replace(MLWSAdapter.DefaultHostName, this.Url)))
                        .Replace("VERSION", this.VersionString);
                    this.m_webService.Url = str;
                    this.m_webService.Credentials = this.Credentials.NetworkCredentials;
                    this.m_webService.Timeout = AdapterConfigurationVariables.WebServiceTimeoutTime;
                    if (this.AdapterProxy != null)
                    {
                        this.m_webService.Proxy = this.AdapterProxy;
                    }

                    this.IncludedCertificates.CopyCertificatesToCollection(this.m_webService.ClientCertificates);
                }

                return this.m_webService;
            }
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

        static MLWSAdapter()
        {
        }

        public MLWSAdapter(string sUrl, Metalogix.Permissions.Credentials credentials)
        {
            this.m_credentials = credentials;
            this.m_webService = null;
            this.SetUrl(sUrl);
        }

        public MLWSAdapter()
        {
            this.m_credentials = new Metalogix.Permissions.Credentials();
            this.m_webService = null;
        }

        public string ActivateReusableWorkflowTemplates()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteUrl, sWebId, sAlertXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, Metalogix.SharePoint.Adapters.AddDocumentOptions Options)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.AddDocumentChunked(sListID, sParentFolder, sListItemXML, fileContents, Options);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, fileContents, null, this.GetWebServiceOptions(Options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private string AddDocumentChunked(string sListID, string sParentFolder, string sListItemXML,
            byte[] fileContents, Metalogix.SharePoint.Adapters.AddDocumentOptions Options)
        {
            bool flag;
            byte[] numArray = this.TransferContent(fileContents, out flag);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sParentFolder, sListItemXML, numArray, flag, this.GetWebServiceOptions(Options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, Metalogix.SharePoint.Adapters.AddDocumentOptions options,
            ref FieldsLookUp fieldsLookupCache)
        {
            object[] objArray = new object[]
                { listId, listName, folderPath, fileXml, fileContents, this.GetWebServiceOptions(options), null };
            object[] objArray1 = objArray;
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray1);
            return str;
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, updatedTargetMetaInfo };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { docTemplate, cTypeXml, url };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sFieldXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents,
            Metalogix.SharePoint.Adapters.AddDocumentOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFileXML, fileContents, Options };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddFolder(string sListID, string sParentFolder, string sFolderXML,
            Metalogix.SharePoint.Adapters.AddFolderOptions Options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sParentFolder, sFolderXML, this.GetWebServiceOptions(Options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            Metalogix.SharePoint.Adapters.AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            object[] objArray = new object[]
                { listId, listName, folderPath, folderXml, this.GetWebServiceOptions(options), null };
            object[] objArray1 = objArray;
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray1);
            return str;
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFolderXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { targetListId, docTemplate, cTypeXml, changedLookupFields };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddList(string sListXML, Metalogix.SharePoint.Adapters.AddListOptions Options,
            byte[] documentTemplateFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListXML, this.GetWebServiceOptions(Options), documentTemplateFile };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml,
            Metalogix.SharePoint.Adapters.AddListItemOptions Options)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.AddListItemChunked(sListID, sParentFolder, sListItemXML, attachementNames,
                    attachmentContents, Options);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents, null,
                this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private string AddListItemChunked(string sListID, string sParentFolder, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.AddListItemOptions Options)
        {
            bool[] flagArray;
            byte[][] numArray = this.TransferContent(attachmentContents, out flagArray);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sListItemXML, attachementNames, numArray, flagArray,
                this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateAudience(string sAudienceXml, Metalogix.SharePoint.Adapters.AddAudienceOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sAudienceXml, this.GetWebServiceOptions(options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateContentType(string sContentTypeXml, string sParentContentTypeName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sContentTypeXml, sParentContentTypeName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sGroupXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sName, sDescription, lPermissionMask };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sReferencedTaxonomyXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTargetTermStoreGuid, sParentTermCollectionXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            Metalogix.SharePoint.Adapters.AddSiteCollectionOptions AddSiteCollOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebApp, sSiteCollectionXML, null };
            objArray[2] = (AddSiteCollOptions == null ? null : this.GetWebServiceOptions(AddSiteCollOptions));
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddSiteUser(string sUserXML, Metalogix.SharePoint.Adapters.AddUserOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUserXML, this.GetWebServiceOptions(options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddTerm(string termXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { termXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { targetTermStoreGuid, termGroupXml, includeGroupXmlInResult };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddTermSet(string termSetXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { termSetXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTargetTermStoreGuid, sLangaugesXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddView(string sListID, string sViewXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sViewXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddWeb(string sWebXML, Metalogix.SharePoint.Adapters.AddWebOptions addOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, this.GetWebServiceOptions(addOptions) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartsXml, sWebPartPageServerRelativeUrl, sEmbeddedHtmlContent };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sWorkflowXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AddWorkflowAssociation(string sListId, string sWorkflowXml, bool bAllowDBWriting)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sWorkflowXml, bAllowDBWriting };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pivotDate, sListID, iItemID, bRecursive };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { colorPaletteUrl, spFontUrl, bgImageUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string ApplyOrUpdateContentType(string sListId, string sContentTypeName, string sFieldXML,
            bool bMakeDefaultContentType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sContentTypeName, sFieldXML, bMakeDefaultContentType };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string BeginCompilingAllAudiences()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, Metalogix.SharePoint.Adapters.AddDocumentOptions options)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sNetworkPath, sListID, sFolder, sListItemXml, this.GetWebServiceOptions(options) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public override void CheckConnection()
        {
            if (!base.CredentialsAreDefault && this.Credentials.Password.IsNullOrEmpty())
            {
                throw new UnauthorizedAccessException("A password is required");
            }

            if (base.HasActiveCookieManager)
            {
                this.UpdateCookie();
            }

            Version version = null;
            string str = null;
            string str1 = this.WebService.Url.Substring(this.Url.Length);
            try
            {
                version = new Version(this.GetServerVersion());
            }
            catch (WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                if (!Utils.ResponseIsRedirect(response))
                {
                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new NoMLSP(
                            "A matching version of the Metalogix SharePoint Extensions web service is not installed on this server.");
                    }

                    throw;
                }
                else
                {
                    string item = response.Headers["Location"];
                    if (item == null)
                    {
                        throw;
                    }

                    if (!item.EndsWith(str1))
                    {
                        throw;
                    }

                    str = item;
                }
            }

            if (str != null)
            {
                string str2 = str.Substring(0, str.Length - str1.Length);
                this.WebService.Url = str;
                try
                {
                    version = new Version(this.GetServerVersion());
                }
                catch (WebException webException1)
                {
                    throw new NoMLSP(
                        "A matching version of the Metalogix SharePoint Extensions web service is not installed on this server.");
                }

                base.SetUrlForRedirect(str2);
            }

            Version version1 = Assembly.GetExecutingAssembly().GetName().Version;
            if (version == null || version1.Major != version.Major || version1.Minor != version.Minor)
            {
                throw new Exception(string.Concat("Extension mismatch - Expected version: ", version1.ToString(),
                    ", Found version: ", version.ToString()));
            }

            try
            {
                this.GetWeb(false);
            }
            catch (Exception exception)
            {
                if (exception is UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException("Access Denied");
                }

                throw;
            }

            try
            {
                this.ServerAdapterConfiguration.Load();
            }
            catch
            {
            }
        }

        public override SharePointAdapter Clone()
        {
            MLWSAdapter mLWSAdapter = new MLWSAdapter();
            mLWSAdapter.CloneFrom(this, true);
            return mLWSAdapter;
        }

        public override SharePointAdapter CloneForNewSiteCollection()
        {
            MLWSAdapter mLWSAdapter = new MLWSAdapter();
            mLWSAdapter.CloneFrom(this, false);
            return mLWSAdapter;
        }

        private void CloneFrom(MLWSAdapter adapter, bool bIncludeSiteCollectionSpecificProperties)
        {
            this.m_sUrl = adapter.m_sUrl;
            this.IsReadOnlyAdapter = adapter.IsReadOnlyAdapter;
            base.IsDataLimitExceededForContentUnderMgmt = adapter.IsDataLimitExceededForContentUnderMgmt;
            this.m_sVersionString = adapter.m_sVersionString;
            this.m_credentials = adapter.m_credentials;
            this.m_webService = adapter.m_webService;
            this.AdapterProxy = adapter.AdapterProxy;
            this.IncludedCertificates = adapter.IncludedCertificates;
            base.SetSystemInfo(adapter.SystemInfo.Clone());
            base.SetSharePointVersion(adapter.SharePointVersion.Clone());
            if (bIncludeSiteCollectionSpecificProperties)
            {
                this.m_sWebID = adapter.m_sWebID;
                this.CookieManager = adapter.CookieManager;
                this.AuthenticationInitializer = adapter.AuthenticationInitializer;
            }
        }

        public string CloseFileCopySession(Guid sessionId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sessionId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sNetworkPath, sSharePointPath };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sFolder, sListItemXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteAllAudiences(string inputXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { inputXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteAudience(string sAudienceName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sAudienceName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, contentTypeIDs };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iListItemID, sFolder };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteGroup(string sGroupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sGroupName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iListItemID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, bDeleteAllItems, sIDs };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteList(string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteRole(string sRoleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sRoleName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sPrincipalName, bIsGroup, sRoleName, sListID, iItemId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL, sWebApp };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteSP2013Workflows(string configurationXml)
        {
            if (!base.SharePointVersion.IsSharePoint2013OrLater)
            {
                throw new NotSupportedException("This method is only supported for SharePoint version 2013 or later.");
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { configurationXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl, sWebPartId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string DisableValidationSettings(string listID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        protected override void Dispose(bool bDisposing)
        {
            base.Dispose(bDisposing);
            if (!this.m_bDisposed && bDisposing)
            {
                if (this.m_credentials != null)
                {
                    this.m_credentials.Dispose();
                    this.m_credentials = null;
                }

                if (this.m_webService != null)
                {
                    this.m_webService.Dispose();
                    this.m_webService = null;
                }

                this.m_bDisposed = true;
            }
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { validationNodeFieldsXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private void EnsureWebServiceHasCookies()
        {
            if (!base.HasActiveCookieManager || this.WebService.CookieContainer != null)
            {
                return;
            }

            CookieContainer cookieContainer = new CookieContainer();
            this.CookieManager.AddCookiesTo(cookieContainer);
            this.WebService.CookieContainer = cookieContainer;
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            object[] objArray = new object[] { commandName, commandConfigurationXml };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        private object ExecuteMethod(string sMethodName, object[] parameters)
        {
            object obj;
            MethodInfo method = this.WebService.GetType().GetMethod(sMethodName);
            if (method == null)
            {
                throw new Exception(string.Concat("Cannot find method \"", sMethodName, "\" in web service wrapper."));
            }

            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            this.EnsureWebServiceHasCookies();
            try
            {
                int num = 0;
                do
                {
                    try
                    {
                        obj = method.Invoke(this.WebService, parameters);
                        return obj;
                    }
                    catch (Exception exception)
                    {
                        Exception executedMethodException = MLWSAdapter.GetExecutedMethodException(exception);
                        bool flag = false;
                        if (typeof(WebException).IsAssignableFrom(executedMethodException.GetType()))
                        {
                            HttpWebResponse response =
                                ((WebException)executedMethodException).Response as HttpWebResponse;
                            if (response != null)
                            {
                                flag = (response.StatusCode == HttpStatusCode.Forbidden ||
                                        response.StatusCode == HttpStatusCode.Unauthorized
                                    ? true
                                    : response.StatusCode == HttpStatusCode.Found);
                            }
                        }

                        if (!(executedMethodException is SoapException) && !flag)
                        {
                            if (num >= AdapterConfigurationVariables.WebServiceRetriesNumber)
                            {
                                throw executedMethodException;
                            }

                            if (AdapterConfigurationVariables.WebServiceRetriesDelay > 0)
                            {
                                Thread.Sleep(AdapterConfigurationVariables.WebServiceRetriesDelay);
                            }
                        }
                        else if (!base.HasActiveCookieManager || num != 0 ||
                                 !executedMethodException.Message.Contains("0x80070005") && !flag)
                        {
                            if (num >= AdapterConfigurationVariables.WebServiceRetriesNumber ||
                                !executedMethodException.Message.Contains(
                                    "deadlocked on lock resources with another process and has been chosen as the deadlock victim."))
                            {
                                throw executedMethodException;
                            }

                            if (AdapterConfigurationVariables.WebServiceRetriesDelay > 0)
                            {
                                Thread.Sleep(AdapterConfigurationVariables.WebServiceRetriesDelay);
                            }
                        }
                        else
                        {
                            this.UpdateCookie();
                        }
                    }

                    num++;
                } while (num <= AdapterConfigurationVariables.WebServiceRetriesNumber ||
                         base.HasActiveCookieManager && num == 1);

                throw new Exception("Total network call attempts exceeded without an error or a return value.");
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return obj;
        }

        public string FindAlerts()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string FindUniquePermissions()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetAddIns(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetAdImportDcMappings(string profileDbConnectionString, string connName, string connType,
            string options)
        {
            object[] objArray = new object[] { profileDbConnectionString, connName, connType, options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetAlerts(string sListID, int iItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetAudiences()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetBcsApplications(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetBrowserFileHandling(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        private byte[] GetContent(GetDocumentResult result)
        {
            if (!result.IsSessionID)
            {
                return result.GetContent();
            }

            BinaryTransfer binaryTransfer = new BinaryTransfer(this, AdapterConfigurationVariables.ChunkStreamType,
                AdapterConfigurationVariables.ChunkRetentionTime);
            return binaryTransfer.StartRead(result.SessionID);
        }

        public string GetContentTypes(string sListId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetCustomProfilePropertyMapping(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { iTemplateId };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.GetDocumentChunked(sDocID, sFileDirRef, sFileLeafRef, iLevel);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iLevel };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentChunked(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sDocID, sFileDirRef, sFileLeafRef, iLevel, AdapterConfigurationVariables.ChunkStreamType,
                AdapterConfigurationVariables.ChunkRetentionTime
            };
            byte[] numArray = (byte[])this.ExecuteMethod(name, objArray);
            return this.GetContent(new GetDocumentResult(numArray));
        }

        public string GetDocumentId(string sDocUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.GetDocumentVersionChunked(sDocID, sFileDirRef, sFileLeafRef, iVersion);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocID, sFileDirRef, sFileLeafRef, iVersion };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public byte[] GetDocumentVersionChunked(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sDocID, sFileDirRef, sFileLeafRef, iVersion, AdapterConfigurationVariables.ChunkStreamType,
                AdapterConfigurationVariables.ChunkRetentionTime
            };
            byte[] numArray = (byte[])this.ExecuteMethod(name, objArray);
            return this.GetContent(new GetDocumentResult(numArray));
        }

        private static Exception GetExecutedMethodException(Exception ex)
        {
            if (!(ex is TargetInvocationException) || ex.InnerException == null)
            {
                return ex;
            }

            return ex.InnerException;
        }

        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sExternalContentTypeNamespace, sExternalContentTypeName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetExternalContentTypes()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetExternalItems(string sExternalContentTypeNamespace, string sExternalContentTypeName,
            string sExternalContentTypeOperation, string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sExternalContentTypeNamespace, sExternalContentTypeName, sExternalContentTypeOperation, sListID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetFarmSandboxSolutions(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetFarmServerDetails(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public byte[] GetFarmSolutionBinary(string solutionName)
        {
            object[] objArray = new object[] { solutionName };
            byte[] numArray = (byte[])this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return numArray;
        }

        public string GetFarmSolutions(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetFields(string sListId, bool bGetAllAvailableFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, bGetAllAvailableFields };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetFiles(string sFolderPath, Metalogix.SharePoint.Adapters.ListItemQueryType itemTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sFolderPath, itemTypes };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetFileVersions(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sIDs, sParentFolder };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetGroups()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetInfopaths(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetLanguagesAndWebTemplates()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetList(string sListID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.ListItemQueryType itemTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, sParentFolder, bRecursive, itemTypes };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            Metalogix.SharePoint.Adapters.ListItemQueryType itemTypes, string sListSettings,
            Metalogix.SharePoint.Adapters.GetListItemOptions getOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, sListSettings,
                this.GetWebServiceOptions(getOptions)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            Metalogix.SharePoint.Adapters.GetListItemOptions getOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listID, fields, query, listSettings, this.GetWebServiceOptions(getOptions) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetListItemVersions(string sListID, int iItemID, string sFields, string sListSettings)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID, sFields, sListSettings };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetLists()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetListTemplates()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetListWorkflowRunning2010(string listName)
        {
            object[] objArray = new object[] { listName };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetLockedSites(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetMySiteData(string sSiteURL)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetPortalListingGroups()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetPortalListingIDs()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetPortalListings(string sIDList)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sIDList };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sReferencedTaxonomyXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetRoleAssignments(string sListId, int iItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, iItemId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetRoles(string sListId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetSecureStorageApplications(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public override string GetServerVersion()
        {
            Metalogix.SharePoint.Adapters.SystemInfo systemInfo =
                new Metalogix.SharePoint.Adapters.SystemInfo((string)this.ExecuteMethod("GetSystemInfo", null));
            return systemInfo.ExtensionVersion.ToString();
        }

        public string GetSharePointVersion()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSite(bool bFetchFullXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bFetchFullXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetSiteCollections()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebAppName };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetSiteQuotaTemplates()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public byte[] GetSiteSolutionsBinary(string itemId)
        {
            object[] objArray = new object[] { itemId };
            byte[] numArray = (byte[])this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return numArray;
        }

        public string GetSiteUsers()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSP2013Workflows(string configurationXml)
        {
            if (!base.SharePointVersion.IsSharePoint2013OrLater)
            {
                throw new NotSupportedException("This method is only supported for SharePoint version 2013 or later.");
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { configurationXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSharePointPath };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetSubWebs()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetSystemInfo()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId, sTermId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId, sTermSetId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermGroups(string sTermStoreId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermStoreId, sTermGroupId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermSets(string sTermGroupId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermGroupId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermSetId, bRecursive };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sTermSetItemId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetTermStores()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string GetUserFromProfile()
        {
            object[] objArray = null;
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
        }

        public string GetUserProfilePropertiesUsage(string profileDbConnectionString, string options)
        {
            object[] objArray = new object[] { profileDbConnectionString, options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
        {
            object[] objArray = new object[] { sSiteURL, sLoginName, null };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            sErrors = (string)objArray[2];
            return str;
        }

        public string GetWeb(bool bFetchFullXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bFetchFullXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetWebApplicationPolicies(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetWebApplications()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebNavigationSettings()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebNavigationStructure()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { iTemplateId };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public object GetWebServiceOptions(Metalogix.SharePoint.Adapters.AdapterOptions adapterOptions)
        {
            Type type = Type.GetType(string.Concat("Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.",
                adapterOptions.GetType().Name, ", Metalogix.SharePoint.Adapters.MLWS"));
            if (type == null)
            {
                return null;
            }

            object obj = Activator.CreateInstance(type);
            this.SetMatchingProperties(adapterOptions, obj);
            return obj;
        }

        public string GetWebTemplates()
        {
            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWorkflowAssociation2010(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetWorkflowAssociation2013(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetWorkflowAssociations(string sObjectID, string sObjectType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sObjectID, sObjectType };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string GetWorkflowRunning2010(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetWorkflowRunning2013(string options)
        {
            object[] objArray = new object[] { options };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        public string GetWorkflows(string sListID, int iItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListID, iItemID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sDocumentServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string HasUniquePermissions(string listID, int listItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID, listItemID };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebPartPageServerRelativeUrl };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string HasWorkflows(string sListId, string sItemId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sItemId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string IsAppWebPartOnPage(Guid appProductId, string itemUrl)
        {
            object[] objArray = new object[] { appProductId, itemUrl };
            string str = (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, objArray);
            return str;
        }

        private static bool IsFullyQualifiedUrl(string sUrl)
        {
            if (sUrl.ToLower().StartsWith("http://"))
            {
                return true;
            }

            return sUrl.ToLower().StartsWith("https://");
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listId };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string IsWorkflowServicesInstanceAvailable()
        {
            if (!base.SharePointVersion.IsSharePoint2013OrLater)
            {
                throw new NotSupportedException("This method is only supported for SharePoint version 2013 or later.");
            }

            return (string)this.ExecuteMethod(MethodBase.GetCurrentMethod().Name, new object[0]);
        }

        public string MigrateSP2013Workflows(string configurationXml)
        {
            if (!base.SharePointVersion.IsSharePoint2013OrLater)
            {
                throw new NotSupportedException("This method is only supported for SharePoint version 2013 or later.");
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { configurationXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string ModifyWebNavigationSettings(string sWebXML,
            Metalogix.SharePoint.Adapters.ModifyNavigationOptions ModNavOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, this.GetWebServiceOptions(ModNavOptions) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public Guid OpenFileCopySession(Metalogix.SharePoint.Adapters.StreamType streamType, int retentionTime)
        {
            Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType streamType1 =
                (Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType)Enum.Parse(
                    typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.StreamType),
                    streamType.ToString());
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { streamType, retentionTime };
            return (Guid)this.ExecuteMethod(name, objArray);
        }

        public byte[] ReadChunk(Guid sessionId, long bytesToRead)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sessionId, bytesToRead };
            return (byte[])this.ExecuteMethod(name, objArray);
        }

        public string ReorderContentTypes(string sListId, string[] sContentTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sListId, sContentTypes };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSearchTerm, sOptionsXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { bParserEnabled };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string SetMasterPage(string sWebXML)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private void SetMatchingProperties(object oSourceObject, object oTargetObject)
        {
            PropertyInfo[] properties = oTargetObject.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.CanWrite)
                {
                    PropertyInfo property = oSourceObject.GetType()
                        .GetProperty(propertyInfo.Name, propertyInfo.PropertyType);
                    if (property != null)
                    {
                        propertyInfo.SetValue(oTargetObject, property.GetValue(oSourceObject, null), null);
                    }
                }
            }
        }

        private void SetUrl(string sUrl)
        {
            if (sUrl == null)
            {
                throw new Exception("Invalid Url. Value cannot be null");
            }

            if (this.m_sUrl == null && !MLWSAdapter.IsFullyQualifiedUrl(sUrl))
            {
                throw new Exception("Invalid Url");
            }

            this.m_sWebID = null;
            if (!MLWSAdapter.IsFullyQualifiedUrl(sUrl))
            {
                this.m_sUrl = string.Concat(Utils.GetServerPart(this.m_sUrl), sUrl);
            }
            else
            {
                this.m_sUrl = sUrl;
            }

            this.m_webService = null;
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSiteURL, sLoginName, sPropertyXml, bCreateIfNotFound };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string SetWelcomePage(string WelcomePage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] welcomePage = new object[] { WelcomePage };
            return (string)this.ExecuteMethod(name, welcomePage);
        }

        public string StoragePointAvailable(string inputXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { inputXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sSharePointPath };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private byte[] TransferContent(byte[] data, out bool isContentMoved)
        {
            bool[] flagArray;
            if (data == null)
            {
                isContentMoved = false;
                return null;
            }

            byte[][] numArray = new byte[][] { data };
            numArray = this.TransferContent(numArray, out flagArray);
            isContentMoved = flagArray[0];
            return numArray[0];
        }

        private byte[][] TransferContent(byte[][] data, out bool[] isContentMoved)
        {
            BinaryTransfer binaryTransfer = new BinaryTransfer(this, AdapterConfigurationVariables.ChunkStreamType,
                AdapterConfigurationVariables.ChunkRetentionTime);
            int num = (data != null ? (int)data.Length : 0);
            byte[][] bytes = new byte[num][];
            isContentMoved = BinaryTransfer.AnalyzeContents(data);
            for (int i = 0; i < num; i++)
            {
                if (isContentMoved[i])
                {
                    string str = binaryTransfer.StartWrite(data[i]);
                    bytes[i] = Encoding.UTF8.GetBytes(str);
                }
                else
                {
                    bytes[i] = data[i];
                }
            }

            return bytes;
        }

        internal void UpdateCookie()
        {
            if (base.HasActiveCookieManager)
            {
                this.CookieManager.UpdateCookie();
                this.WebService.CookieContainer = new CookieContainer();
                this.CookieManager.AddCookiesTo(this.WebService.CookieContainer);
            }
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, Metalogix.SharePoint.Adapters.UpdateDocumentOptions Options)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.UpdateDocumentChunked(sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents,
                    Options);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sFileLeafRef, sListItemXML, fileContents, this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private string UpdateDocumentChunked(string sListID, string sParentFolder, string sFileLeafRef,
            string sListItemXML, byte[] fileContents, Metalogix.SharePoint.Adapters.UpdateDocumentOptions Options)
        {
            bool flag;
            byte[] numArray = this.TransferContent(fileContents, out flag);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, sFileLeafRef, sListItemXML, numArray, flag, this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateGroupQuickLaunch(string value)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { value };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml,
            Metalogix.SharePoint.Adapters.UpdateListOptions Options, byte[] documentTemplateFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sListID, sListXML, sViewXml, this.GetWebServiceOptions(Options), documentTemplateFile };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.UpdateListItemOptions Options)
        {
            if (AdapterConfigurationVariables.EnableChunkedTransfer)
            {
                return this.UpdateListItemChunked(sListID, sParentFolder, iItemID, sListItemXML, attachementNames,
                    attachmentContents, Options);
            }

            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, iItemID, sListItemXML, attachementNames, attachmentContents,
                this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
        }

        private string UpdateListItemChunked(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachementNames, byte[][] attachmentContents,
            Metalogix.SharePoint.Adapters.UpdateListItemOptions Options)
        {
            bool[] flagArray;
            byte[][] numArray = this.TransferContent(attachmentContents, out flagArray);
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                sListID, sParentFolder, iItemID, sListItemXML, attachementNames, numArray, flagArray,
                this.GetWebServiceOptions(Options)
            };
            return (string)this.ExecuteMethod(name, objArray);
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
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            Metalogix.SharePoint.Adapters.UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUpdateXml, this.GetWebServiceOptions(updateSiteCollectionOptions) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateWeb(string sWebXML, Metalogix.SharePoint.Adapters.UpdateWebOptions updateOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sWebXML, this.GetWebServiceOptions(updateOptions) };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUpdateXml };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sUserIdentifier, bCanBeDomainGroup };
            return (string)this.ExecuteMethod(name, objArray);
        }

        public void WriteChunk(Guid sessionId, byte[] data)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sessionId, data };
            this.ExecuteMethod(name, objArray);
        }
    }
}