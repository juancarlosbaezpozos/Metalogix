using Metalogix.Core;
using Metalogix.Core.OperationLog;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters.Expert;
using Metalogix.SharePoint.Adapters.OM.Exceptions;
using Metalogix.SharePoint.Adapters.OM.Helper;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Common;
using Metalogix.Utilities;
using Microsoft.Office.Excel.WebUI;
using Microsoft.Office.InfoPath.Server.Administration;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Audience;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Meetings;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.SoapServer;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Workflow;
using Microsoft.Win32;
using SPDisposeCheck;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM
{
    [AdapterDisplayName("Local Connection (SharePoint Object Model)"), AdapterShortName("OM"),
     AdapterSupports(AdapterSupportedFlags.SiteScope | AdapterSupportedFlags.WebAppScope |
                     AdapterSupportedFlags.FarmScope | AdapterSupportedFlags.LegacyLicense |
                     AdapterSupportedFlags.CurrentLicense), MenuOrder(2), ShowInMenu(true)]
    public class OMAdapter : SharePointAdapter, ISharePointReader, ISharePointWriter, ISharePointAdapterCommand,
        IMigrationExpertReports
    {
        private enum WorkflowReusableScope
        {
            Reusable,
            GloballyReusable
        }

        private enum WorkflowScope
        {
            ContentType,
            List,
            Site
        }

        private class VersionString
        {
            private int m_iMajorVersion = 1;

            private int m_iMinorVersion;

            public int MajorVersion
            {
                get { return this.m_iMajorVersion; }
            }

            public int MinorVersion
            {
                get { return this.m_iMinorVersion; }
            }

            public VersionString(string sVersionString)
            {
                try
                {
                    int num = sVersionString.IndexOf(".", StringComparison.Ordinal);
                    string text = (num >= 0) ? sVersionString.Substring(0, num) : sVersionString;
                    string text2 = (num >= 0) ? sVersionString.Substring(num + 1) : "";
                    this.m_iMajorVersion = ((text.Length > 0) ? Convert.ToInt32(text) : 0);
                    this.m_iMinorVersion = ((text2.Length > 0) ? Convert.ToInt32(text2) : 0);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception("Version string was in an invalid format: ' " + sVersionString + "'");
                }
            }

            public override string ToString()
            {
                return this.MajorVersion + "." + this.MinorVersion;
            }
        }

        private class WebPartToEmbed
        {
            private string m_Guid;

            private string m_Zone;

            public string Guid
            {
                get { return this.m_Guid; }
                set { this.m_Guid = value; }
            }

            public string Zone
            {
                get { return this.m_Zone; }
                set { this.m_Zone = value; }
            }

            public WebPartToEmbed(string sGuid, string sZone)
            {
                this.Guid = sGuid;
                this.Zone = sZone;
            }
        }

        public const string DOCUMENT_SET_ID = "0x0120D520";

        private const string WorkflowInstanceItemID = "Microsoft.SharePoint.ActivationProperties.ItemId";

        private const string WorkflowInstanceSiteSettings = "/_layouts/15/workflow.aspx";

        private const string WorkflowInstanceInitiatorUserId =
            "Microsoft.SharePoint.ActivationProperties.InitiatorUserId";

        private const string WorkflowInstanceCurrentItemUrl =
            "Microsoft.SharePoint.ActivationProperties.CurrentItemUrl";

        private const int SLIDE_LIBRARY_TEMPLATE_VALUE = 2100;

        private const int MODERATION_STATUS_APPROVED = 0;

        private const int RETRIES = 5;

        private const string TEMPVERSIONSTRING = "Temporary Version - (To be deleted)";

        private const string c_sIs2010WikiFieldRegex =
            "<a id=\"[0-9]*:[^\"]*:[^\"]*\" class=\"(ms-wikilink|ms-missinglink)\" href=\"[^\"]*\">[^<]*</a>";

        private const string C_INFOPATH_GUID_INTERNAL_NAME_FORMAT =
            "_[0-9a-fA-F]{8}(_[0-9a-fA-F]{4}){3}(_[0-9a-fA-F]{12})";

        private const uint MINIMUM_SHAREPOINT_LIST_VIEW_THRESHOLD = 2000u;

        private const string C_UnableToAccessTermstore =
            "Unable to access the Term Store.Please ensure enough permissions have been granted to the term store and its related service for the migrating account.";

        private const string C_FS_UnableToAccessTermGroup =
            "Unable to access the group '{0}' in Term Store '{1}'.Please ensure enough permissions have been granted to the term store and its related service for the migrating account.";

        private const string WIKI_WEBPART_ZONE = "wpz";

        private const string PUBLISHING_FEATURE_GUID = "f6924d36-2fa8-4f0b-b16d-06b7250180fa";

        private const string RECORD_CENTER_TEMPLATE = "OFFILE";

        private bool? m_bSupportsDocumentSets = null;

        private bool? m_bSupportsFormService = null;

        private static Int32Converter s_IntConverter;

        private static DoubleConverter s_DoubleConverter;

        private static CultureInfo s_CultureInfo;

        public static TimeSpan tsSecurity;

        public static TimeSpan tsDBFetch;

        protected static uint? s_iGetListQueryRowLimit;

        private static Type[] s_updateTypeParameters;

        private volatile int _preventOptimizationVariable;

        private static Guid s_EmptyGuid;

        private static Guid EnterPriseKeywordsFieldId;

        private static bool? supportsColumnDefaultValueSettings;

        private static string _brackets;

        private static object _bracketLock;

        private static bool? m_bSupportsPublishing;

        private static int PAGES_LIBRARY_TEMPLATE_ID;

        private static Type s_publishingType;

        private static bool? isSupportsUserProfile;

        private static Type userProfileManager;

        private static bool? isSPFoundation2013;

        private static bool? isSPFoundation2010WithSearchExpress;

        private static OverrideSQLAuthenticationHandler s_overrideSQLAuthenticationHandler;

        private string m_sUrl;

        private Credentials m_credentials = new Credentials();

        private static object s_oPrimingLock;

        private static bool s_bWebServicesPrimed;

        private static bool? s_bSupportsDBWriting;

        private SharePointAdapter m_dbAdapter;

        private SharePointAdapter _dbReader;

        private bool? m_bClaimsAuthenticationInUse = null;

        private static bool? s_supportsPartialDBWriting;

        private SharePointAdapter m_dbAdapterPartial;

        private IStoragePointSupport m_storagePointAdapter;

        private static Guid C_HASH_TAGS_TERMSET_GUID;

        private static bool? _supportsInfoPath;

        private Guid _xslt_ListView_WebPart_TypeId_SP2010 = new Guid("874f5460-71f9-fecc-e894-e7e858d9713e");

        private Guid _xslt_ListView_WebPart_TypeId_SP2013 = new Guid("A6524906-3FD2-EE4E-23EE-252D3C6E0DC9");

        private Guid _xslt_ListView_WebPart_TypeId_SP2016 = new Guid("35aee725-be5b-7f5c-30f1-fb758cbc1310");

        private static readonly IList<string> CustomWebPartTypes;

        private static Type _excelWebUi;

        private static Type _excelWebUIInternal;

        private string[] fieldsThatSupportEmbedding = new string[]
        {
            "WikiField",
            "PublishingPageContent"
        };

        private static bool? m_bSupportsExcelWebAccessServices;

        private static Guid PUBLISHINGGUID;

        private static Guid RECORDSCENTERGUID;

        private static Guid WIKIHOMEPAGEFEATUREGUID;

        public bool AreDocumentSetsSupported
        {
            get
            {
                if (!this.m_bSupportsDocumentSets.HasValue)
                {
                    bool value = false;
                    try
                    {
                        value = this.CanLoadDocSetsDLL();
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        value = false;
                    }

                    this.m_bSupportsDocumentSets = new bool?(value);
                }

                return this.m_bSupportsDocumentSets.Value;
            }
        }

        private bool IsFormServiceAvailable
        {
            get
            {
                if (!this.m_bSupportsFormService.HasValue)
                {
                    try
                    {
                        this.TryToLoadFormService();
                        this.m_bSupportsFormService = new bool?(true);
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        this.m_bSupportsFormService = new bool?(false);
                    }
                }

                return this.m_bSupportsFormService == true;
            }
        }

        private static bool SupportsColumnDefaultValueSettings
        {
            get
            {
                if (!OMAdapter.supportsColumnDefaultValueSettings.HasValue)
                {
                    try
                    {
                        OMAdapter.supportsColumnDefaultValueSettings =
                            new bool?(OMAdapter.CanLoadOfficeDocumentManagementDLL());
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        OMAdapter.supportsColumnDefaultValueSettings = new bool?(false);
                    }
                }

                return OMAdapter.supportsColumnDefaultValueSettings.Value;
            }
        }

        private static bool SupportsPublishing
        {
            get
            {
                if (!OMAdapter.m_bSupportsPublishing.HasValue)
                {
                    try
                    {
                        OMAdapter.LoadPublishingDLL();
                        OMAdapter.m_bSupportsPublishing = new bool?(true);
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        OMAdapter.m_bSupportsPublishing = new bool?(false);
                    }
                }

                return OMAdapter.m_bSupportsPublishing.Value;
            }
        }

        private static bool SupportsUserProfile
        {
            get
            {
                if (!OMAdapter.isSupportsUserProfile.HasValue)
                {
                    if (OMAdapter.IsSPFoundation2013 || OMAdapter.IsSPFoundation2010WithSearchExpress)
                    {
                        OMAdapter.isSupportsUserProfile = new bool?(false);
                    }
                    else
                    {
                        try
                        {
                            OMAdapter.LoadUserProfileDLL();
                            OMAdapter.isSupportsUserProfile = new bool?(true);
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            OMAdapter.isSupportsUserProfile = new bool?(false);
                        }
                    }
                }

                return OMAdapter.isSupportsUserProfile.Value;
            }
        }

        private static bool IsSPFoundation2013
        {
            get
            {
                if (!OMAdapter.isSPFoundation2013.HasValue)
                {
                    OMAdapter.isSPFoundation2013 = new bool?(false);
                    try
                    {
                        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(
                                   "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\15.0\\WSS\\InstalledProducts\\",
                                   false))
                        {
                            if (registryKey != null && registryKey.ValueCount == 1)
                            {
                                string text = registryKey.GetValue(registryKey.GetValueNames()[0]).ToString();
                                if (text.CompareTo(
                                        SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                            .SharePointFoundation2013)) == 0)
                                {
                                    OMAdapter.isSPFoundation2013 = new bool?(true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }
                }

                return OMAdapter.isSPFoundation2013.Value;
            }
        }

        private static bool IsSPFoundation2010WithSearchExpress
        {
            get
            {
                if (!OMAdapter.isSPFoundation2010WithSearchExpress.HasValue)
                {
                    OMAdapter.isSPFoundation2010WithSearchExpress = new bool?(false);
                    try
                    {
                        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(
                                   "SOFTWARE\\Microsoft\\Shared Tools\\Web Server Extensions\\14.0\\WSS\\InstalledProducts\\",
                                   false))
                        {
                            if (registryKey != null && registryKey.ValueCount > 1)
                            {
                                bool flag = false;
                                bool flag2 = false;
                                bool flag3 = false;
                                string[] valueNames = registryKey.GetValueNames();
                                string[] array = valueNames;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    string name = array[i];
                                    string a = registryKey.GetValue(name) as string;
                                    if (string.Equals(a,
                                            SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                .SharePointFoundation2010), StringComparison.Ordinal))
                                    {
                                        flag = true;
                                    }
                                    else if (string.Equals(a,
                                                 SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                     .SearchServerExpress2010), StringComparison.Ordinal))
                                    {
                                        flag2 = true;
                                    }
                                    else if (string.Equals(a,
                                                 SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                     .SharePointServer2010StandardTrial), StringComparison.Ordinal) ||
                                             string.Equals(a,
                                                 SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                     .SharePointServer2010Standard), StringComparison.Ordinal) ||
                                             string.Equals(a,
                                                 SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                     .SharePointServer2010EnterpriseTrial), StringComparison.Ordinal) ||
                                             string.Equals(a,
                                                 SharePointVersion.GetInstallationRegKey(SharePointVersion.SKU
                                                     .SharePointServer2010Enterprise), StringComparison.Ordinal))
                                    {
                                        flag3 = true;
                                    }
                                }

                                if (flag && flag2 && !flag3)
                                {
                                    OMAdapter.isSPFoundation2010WithSearchExpress = new bool?(true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }
                }

                return OMAdapter.isSPFoundation2010WithSearchExpress.Value;
            }
        }

        public override ExternalizationSupport ExternalizationSupport
        {
            get { return ExternalizationSupport.Supported; }
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

                if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    this.m_sUrl = value;
                    return;
                }

                this.m_sUrl = this.Server + OMAdapter.GetServerRelativeUrl(value);
            }
        }

        public override Credentials Credentials
        {
            get { return this.m_credentials; }
            set { this.m_credentials = value; }
        }

        public override string ServerRelativeUrl
        {
            get { return OMAdapter.GetServerRelativeUrl(this.Url); }
        }

        public override string WebID
        {
            get
            {
                string result;
                using (Context context = this.GetContext())
                {
                    result = context.Web.ID.ToString();
                }

                return result;
            }
            set { }
        }

        public override bool IsPortal2003Connection
        {
            get { return false; }
        }

        public override ISharePointReader Reader
        {
            get { return SharePointReader.GetSharePointReader(this); }
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

        public static bool SupportsDBWriting
        {
            get
            {
                if (!OMAdapter.s_bSupportsDBWriting.HasValue)
                {
                    OMAdapter.s_bSupportsDBWriting = new bool?(OMAdapter.CheckDBWritingAvailability());
                }

                return OMAdapter.s_bSupportsDBWriting.Value;
            }
        }

        public override string ServerUrl
        {
            get { return Utils.GetServerPart(this.Url); }
        }

        public override string Server
        {
            get { return Utils.GetServerPart(this.Url); }
        }

        public override string ServerType
        {
            get { return "Local Connection"; }
        }

        public override SharePointServerAdapterConfig ServerAdapterConfiguration
        {
            get
            {
                if (base.ServerAdapterConfigIsNull)
                {
                    base.ServerAdapterConfiguration = new LocalSharePointServerAdapterConfig(this);
                }

                return base.ServerAdapterConfiguration;
            }
            set { base.ServerAdapterConfiguration = value; }
        }

        public override bool SupportsWorkflows
        {
            get { return true; }
        }

        public override bool SupportsTaxonomy
        {
            get { return base.SystemInfo.HasTaxonomySupport; }
        }

        internal bool ClaimsAuthenticationInUse
        {
            get { return false; }
        }

        public static bool SupportsPartialDBWriting
        {
            get
            {
                if (!OMAdapter.s_supportsPartialDBWriting.HasValue)
                {
                    OMAdapter.s_supportsPartialDBWriting = new bool?(OMAdapter.CheckPartialDBWritingAvailability());
                }

                return OMAdapter.s_supportsPartialDBWriting.Value;
            }
        }

        protected IStoragePointSupport StoragePointAdapter
        {
            get
            {
                if (this.m_storagePointAdapter == null)
                {
                    this.m_storagePointAdapter = OMAdapter.GetStoragePointAdapter();
                }

                return this.m_storagePointAdapter;
            }
        }

        private static bool SupportsInfoPath2010
        {
            get
            {
                if (!OMAdapter._supportsInfoPath.HasValue)
                {
                    try
                    {
                        OMAdapter._supportsInfoPath = new bool?(OMAdapter.CanLoadInfoPath2010ServerDLL());
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        OMAdapter._supportsInfoPath = new bool?(false);
                    }
                }

                return OMAdapter._supportsInfoPath.Value;
            }
        }

        private static bool SupportsExcelWebAccessService
        {
            get
            {
                if (!OMAdapter.m_bSupportsExcelWebAccessServices.HasValue)
                {
                    try
                    {
                        OMAdapter.LoadExcelWebUIDLL();
                        OMAdapter.m_bSupportsExcelWebAccessServices = new bool?(true);
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        OMAdapter.m_bSupportsExcelWebAccessServices = new bool?(false);
                    }
                }

                return OMAdapter.m_bSupportsExcelWebAccessServices.Value;
            }
        }

        private static bool SupportsBcs()
        {
            Type type = Type.GetType(
                "Microsoft.SharePoint.SPBusinessDataField, Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            Type type2 =
                Type.GetType(
                    "Microsoft.BusinessData.MetadataModel.IMetadataCatalog, Microsoft.BusinessData, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            return type != null && type2 != null;
        }

        public string GetExternalContentTypes()
        {
            if (OMAdapter.SupportsBcs())
            {
                return this.GetExternalContentTypeCollection();
            }

            return this.GetEmptyExternalContentTypeCollection();
        }

        private string GetExternalContentTypeCollection()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeCollection");
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyExternalContentTypeCollection()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeCollection");
            xmlTextWriter.WriteAttributeString("BcsSupport", false.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetExternalItems(string sExtContentTypeNamespace, string sExtContentTypeName,
            string sExtContentTypeOperationName, string sListID)
        {
            if (OMAdapter.SupportsBcs())
            {
                return this.GetExternalItemCollection(sExtContentTypeNamespace, sExtContentTypeName,
                    sExtContentTypeOperationName);
            }

            return this.GetEmptyExternalContentTypeCollection();
        }

        private string GetExternalItemCollection(string sExtContentTypeNamespace, string sExtContentTypeName,
            string sExtContentTypeOperationName)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalItemCollection");
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyExternalItemCollection()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalItemCollection");
            xmlTextWriter.WriteAttributeString("BcsSupport", false.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetExternalContentTypeOperations(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            if (OMAdapter.SupportsBcs())
            {
                return this.GetExternalContentTypeOperationCollection(sExternalContentTypeNamespace,
                    sExternalContentTypeName);
            }

            return this.GetEmptyExternalContentTypeOperationCollection();
        }

        private string GetExternalContentTypeOperationCollection(string sExternalContentTypeNamespace,
            string sExternalContentTypeName)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeOperationCollection");
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyExternalContentTypeOperationCollection()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeOperationCollection");
            xmlTextWriter.WriteAttributeString("BcsSupport", false.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public void WriteBCSPropertiesToXml(XmlWriter xmlWriter, SPList list)
        {
        }

        private void WriteValueAsBcsData(SPField targetField, string sValue, SPListItem targetItem)
        {
            OMAdapter.SupportsBcs();
        }

        public Guid CreateExternalList(SPWeb web, string sName, string sDescription, XmlNode listXML)
        {
            throw new Exception("Failed to create external list because it is supported just in SharePoint 2010.");
        }

        private bool CanLoadDocSetsDLL()
        {
            Type type = null;
            return type != null;
        }

        public SPListItem AddDocumentSet(SPList list, string fullFolderPath, XmlNode folderXML, string sName)
        {
            return null;
        }

        public bool WriteDocumentSetSettings(XmlWriter xmlWriter, SPContentType contentType)
        {
            return true;
        }

        public void WriteDocumentSetFieldSettings(XmlWriter xmlWriter, SPContentType contentType, SPFieldLink fieldLink)
        {
        }

        public void UpdateDocumentSetContentType(SPContentType contentType, XmlNode ctNode, SPWeb targetWeb,
            SPList targetList, out List<string> allowedContentTypeFailures)
        {
            allowedContentTypeFailures = new List<string>();
        }

        public string GetLockedSites(string options)
        {
            string lockedSites;
            using (Context context = this.GetContext())
            {
                IMigrationExpertReports dBReader = this.GetDBReader(context.Web);
                lockedSites = dBReader.GetLockedSites(options);
            }

            return lockedSites;
        }

        public string IsAppWebPartOnPage(Guid appProductId, string itemUrl)
        {
            return false.ToString();
        }

        public string GetAddIns(string options)
        {
            return string.Empty;
        }

        public string GetFarmSolutions(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                SPSolutionCollection solutions = SPFarm.Local.Solutions;
                if (solutions != null && solutions.Any<SPSolution>())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                    {
                        xmlWriter.WriteStartElement("FarmSolutions");
                        foreach (SPSolution current in solutions)
                        {
                            this.PopulateFarmSolutionDetails(operationReporting, xmlWriter, current);
                        }

                        xmlWriter.WriteEndElement();
                    }

                    operationReporting.LogObjectXml(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                string text = "An error occurred while retrieving details for Farm Solutions.";
                OMAdapter.LogExceptionDetails(ex,
                    string.Format("MethodName: '{0}' Message: '{1}'", MethodBase.GetCurrentMethod().Name, text), null);
                operationReporting.LogError(ex, text);
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public byte[] GetFarmSolutionBinary(string solutionName)
        {
            byte[] fileData = null;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                string text = string.Empty;
                try
                {
                    SPSolutionCollection solutions = SPFarm.Local.Solutions;
                    List<SPSolution> list = (solutions != null) ? solutions.ToList<SPSolution>() : null;
                    if (list != null && list.Any<SPSolution>())
                    {
                        text = this.SaveFarmSolution(solutionName, list);
                        if (!string.IsNullOrEmpty(text))
                        {
                            fileData = File.ReadAllBytes(text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string arg = string.Format("Failed to fetch contents for farm solution '{0}' with error: '{1}' ",
                        solutionName, ex.Message);
                    OMAdapter.LogExceptionDetails(ex,
                        string.Format("MethodName: '{0}' Message: '{1}'", MethodBase.GetCurrentMethod().Name, arg),
                        null);
                }
                finally
                {
                    this.DeleteSolutionFile(text);
                }
            });
            return fileData;
        }

        public byte[] GetSiteSolutionsBinary(string itemId)
        {
            return null;
        }

        public string GetFarmSandboxSolutions(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            return string.Empty;
        }

        public string GetBrowserFileHandling(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                using (Context context = this.GetContext())
                {
                    IMigrationExpertReports dBReader = this.GetDBReader(context.Web);
                    string browserFileHandling = dBReader.GetBrowserFileHandling(options);
                    OperationReportingResult operationReportingResult =
                        new OperationReportingResult(browserFileHandling);
                    string objectXml = operationReportingResult.ObjectXml;
                    if (operationReportingResult.ErrorOccured)
                    {
                        foreach (ReportingElement current in operationReportingResult.Errors)
                        {
                            operationReporting.LogError(current.Message, current.Detail, current.Stack, 0, 0);
                        }
                    }

                    if (!string.IsNullOrEmpty(objectXml))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(objectXml);
                        XmlNodeList xmlNodeList =
                            xmlDocument.SelectNodes("/BrowserFileHandlingCollection/BrowserFileHandling");
                        if (xmlNodeList != null)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                            {
                                xmlWriter.WriteStartElement("BrowserFileHandlingCollection");
                                foreach (XmlNode node in xmlNodeList)
                                {
                                    Guid attributeValueAsGuid = node.GetAttributeValueAsGuid("WebId");
                                    try
                                    {
                                        using (SPWeb sPWeb = context.Site.OpenWeb(attributeValueAsGuid))
                                        {
                                            if (context.Web.IsRootWeb || (!context.Web.IsRootWeb &&
                                                                          sPWeb.Url.Trim(new char[47])
                                                                              .StartsWith(
                                                                                  context.Web.Url.Trim(new char[47]),
                                                                                  StringComparison.OrdinalIgnoreCase)))
                                            {
                                                xmlWriter.WriteStartElement("BrowserFileHandling");
                                                xmlWriter.WriteAttributeString("SiteId",
                                                    node.GetAttributeValueAsString("SiteId"));
                                                xmlWriter.WriteAttributeString("WebId",
                                                    node.GetAttributeValueAsString("WebId"));
                                                xmlWriter.WriteAttributeString("WebUrl",
                                                    node.GetAttributeValueAsString("WebUrl"));
                                                xmlWriter.WriteAttributeString("FilePath",
                                                    node.GetAttributeValueAsString("FilePath"));
                                                xmlWriter.WriteEndElement();
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        operationReporting.LogError(exception,
                                            string.Format(
                                                "An error occurred while retrieving files for Browser File Handling for web with ID: '{0}'",
                                                attributeValueAsGuid));
                                    }
                                }

                                xmlWriter.WriteEndElement();
                            }

                            operationReporting.LogObjectXml(stringBuilder.ToString());
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                operationReporting.LogError(exception2,
                    "An error occurred while retrieving files for Browser File Handling.");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string GetBcsApplications(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                operationReporting.LogObjectXml(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "An error occurred while retrieving BCS application collection");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string GetCustomProfilePropertyMapping(string options)
        {
            return string.Empty;
        }

        private string GetUserProfileProperty()
        {
            return string.Empty;
        }

        public string GetInfopaths(string options)
        {
            throw new NotImplementedException();
        }

        public string GetSecureStorageApplications(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                {
                    xmlWriter.WriteStartElement("SecureStoreApplicationCollection");
                    xmlWriter.WriteEndElement();
                }

                operationReporting.LogObjectXml(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "An error occurred while retrieving secure store applications");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string GetWebApplicationPolicies(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                {
                    xmlWriter.WriteStartElement("WebApplicationCollection");
                    SPWebService value4 = SPFarm.Local.Services.GetValue<SPWebService>("");
                    using (Context context = this.GetContext())
                    {
                        foreach (SPWebApplication current in value4.WebApplications)
                        {
                            xmlWriter.WriteStartElement("WebApplication");
                            xmlWriter.WriteAttributeString("Name", current.Name);
                            string value2 = current.AlternateUrls.GetResponseUrl(context.Site.Zone, true).Uri
                                .ToString();
                            xmlWriter.WriteAttributeString("URL", value2);
                            xmlWriter.WriteStartElement("Policies");
                            foreach (SPPolicy sPPolicy in current.Policies)
                            {
                                if (sPPolicy != null)
                                {
                                    xmlWriter.WriteStartElement("Policy");
                                    xmlWriter.WriteAttributeString("PolicyUserName", sPPolicy.UserName);
                                    xmlWriter.WriteAttributeString("PolicyDisplayName", sPPolicy.DisplayName);
                                    object[] array = new object[]
                                    {
                                        sPPolicy.PolicyRoleBindings
                                    };
                                    if (array.Any<object>())
                                    {
                                        string[] value3 = Array.ConvertAll<object, string>(array,
                                            (object value) => value.ToString());
                                        xmlWriter.WriteAttributeString("PolicyRoleBinding", string.Join(";", value3));
                                    }

                                    xmlWriter.WriteEndElement();
                                }
                            }

                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteEndElement();
                        }
                    }

                    xmlWriter.WriteEndElement();
                }

                operationReporting.LogObjectXml(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "An error occurred while retrieving web application user policies");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string GetWorkflowAssociation2013(string options)
        {
            return string.Empty;
        }

        public string GetWorkflowAssociation2010(string options)
        {
            return string.Empty;
        }

        public string GetWorkflowRunning2013(string options)
        {
            return string.Empty;
        }

        public string GetWorkflowRunning2010(string options)
        {
            return string.Empty;
        }

        public string GetListWorkflowRunning2010(string listName)
        {
            return string.Empty;
        }

        public string GetFileVersions(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                using (Context context = this.GetContext())
                {
                    IMigrationExpertReports dBReader = this.GetDBReader(context.Web);
                    string fileVersions = dBReader.GetFileVersions(options);
                    OperationReportingResult operationReportingResult = new OperationReportingResult(fileVersions);
                    string objectXml = operationReportingResult.ObjectXml;
                    if (operationReportingResult.ErrorOccured)
                    {
                        foreach (ReportingElement current in operationReportingResult.Errors)
                        {
                            operationReporting.LogError(current.Message, current.Detail, current.Stack, 0, 0);
                        }
                    }

                    if (!string.IsNullOrEmpty(objectXml))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(objectXml);
                        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/FileVersions/FileVersion");
                        if (xmlNodeList != null)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                            {
                                xmlWriter.WriteStartElement("FileVersions");
                                foreach (XmlNode node in xmlNodeList)
                                {
                                    Guid attributeValueAsGuid = node.GetAttributeValueAsGuid("WebId");
                                    try
                                    {
                                        using (SPWeb sPWeb = context.Site.OpenWeb(attributeValueAsGuid))
                                        {
                                            if (context.Web.IsRootWeb || (!context.Web.IsRootWeb &&
                                                                          sPWeb.Url.Trim(new char[47])
                                                                              .StartsWith(
                                                                                  context.Web.Url.Trim(new char[47]),
                                                                                  StringComparison.OrdinalIgnoreCase)))
                                            {
                                                this.GetFileVersionsData(sPWeb, node, xmlWriter);
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        operationReporting.LogError(exception,
                                            string.Format(
                                                "An error occurred while retrieving one of the site having large file versions ID: '{0}'",
                                                attributeValueAsGuid));
                                    }
                                }

                                xmlWriter.WriteEndElement();
                            }

                            operationReporting.LogObjectXml(stringBuilder.ToString());
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                operationReporting.LogError(exception2,
                    "An error occurred while retrieving sites with large file versions");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void GetFileVersionsData(SPWeb web, XmlNode node, XmlWriter xmlWriter)
        {
            Guid guid = node.GetAttributeValueAsGuid("ListId");
            int attributeValueAsInt = node.GetAttributeValueAsInt("VersionCount");
            string value = node.GetAttributeValueAsString("LeafName");
            string attributeValueAsString = node.GetAttributeValueAsString("FilePath");
            string strUrl = attributeValueAsString.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? attributeValueAsString
                : ('/' + attributeValueAsString);
            string value2 = web.Site.MakeFullUrl(strUrl);
            string value3 = "N/A";
            if (guid == web.ID)
            {
                guid = Guid.Empty;
            }

            if (guid != Guid.Empty)
            {
                SPList sPList = web.Lists[guid];
                if (sPList != null)
                {
                    value3 = sPList.Title;
                    if (sPList.BaseType == SPBaseType.GenericList)
                    {
                        SPListItem itemById = sPList.GetItemById(node.GetAttributeValueAsInt("ItemId"));
                        value = itemById.Title;
                    }
                }
            }

            xmlWriter.WriteStartElement("FileVersion");
            xmlWriter.WriteAttributeString("WebUrl", web.Url);
            xmlWriter.WriteAttributeString("FileName", value);
            xmlWriter.WriteAttributeString("VersionsCount", attributeValueAsInt.ToString());
            xmlWriter.WriteAttributeString("ListName", value3);
            xmlWriter.WriteAttributeString("ItemUrl", value2);
            xmlWriter.WriteEndElement();
        }

        public string GetUserProfilePropertiesUsage(string profileDbConnectionString, string options)
        {
            string userProfilePropertiesUsage;
            using (Context context = this.GetContext())
            {
                IMigrationExpertReports dBReader = this.GetDBReader(context.Web);
                userProfilePropertiesUsage =
                    dBReader.GetUserProfilePropertiesUsage(profileDbConnectionString, string.Empty);
            }

            return userProfilePropertiesUsage;
        }

        public string GetAdImportDcMappings(string profileDbConnectionString, string connName, string connType,
            string options)
        {
            string adImportDcMappings;
            using (Context context = this.GetContext())
            {
                IMigrationExpertReports dBReader = this.GetDBReader(context.Web);
                adImportDcMappings =
                    dBReader.GetAdImportDcMappings(profileDbConnectionString, connName, connType, options);
            }

            return adImportDcMappings;
        }

        private void PopulateFarmSolutionDetails(OperationReporting opReport, XmlWriter xmlWriter,
            SPSolution farmSolution)
        {
            try
            {
                xmlWriter.WriteStartElement("FarmSolution");
                xmlWriter.WriteAttributeString("Id", farmSolution.Id.ToString());
                xmlWriter.WriteAttributeString("SolutionId", farmSolution.SolutionId.ToString());
                xmlWriter.WriteAttributeString("Name", farmSolution.Name);
                xmlWriter.WriteAttributeString("DisplayName", farmSolution.DisplayName);
                xmlWriter.WriteAttributeString("SolutionVersion", farmSolution.Version.ToString());
                xmlWriter.WriteAttributeString("DeploymentState", farmSolution.DeploymentState.ToString());
                xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                string text = string.Format("An error occurred while retrieving details for Farm Solution: '{0}'",
                    farmSolution.Name);
                OMAdapter.LogExceptionDetails(ex,
                    string.Format("MethodName: '{0}' Message: '{1}'", MethodBase.GetCurrentMethod().Name, text), null);
                opReport.LogError(ex, text);
            }
        }

        private void DeleteSolutionFile(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                string arg = string.Format("Failed to delete file '{0}' with error: '{1}' ", fileName, ex.Message);
                OMAdapter.LogExceptionDetails(ex,
                    string.Format("MethodName: '{0}' Message: '{1}'", MethodBase.GetCurrentMethod().Name, arg), null);
            }
        }

        private string SaveFarmSolution(string solutionName, List<SPSolution> solutionCollection)
        {
            try
            {
                SPSolution sPSolution = solutionCollection.FirstOrDefault((SPSolution solution) =>
                    solution.Name.Equals(solutionName, StringComparison.OrdinalIgnoreCase));
                string tempPath = Path.GetTempPath();
                string text = tempPath + sPSolution.Name;
                string fileName = Path.GetFileName(text);
                if (!string.IsNullOrEmpty(fileName))
                {
                    sPSolution.SolutionFile.SaveAs(text);
                    return text;
                }
            }
            catch (Exception ex)
            {
                string arg = string.Format("Failed to save solution file '{0}' with error: '{1}' ", solutionName,
                    ex.Message);
                OMAdapter.LogExceptionDetails(ex,
                    string.Format("MethodName: '{0}' Message: '{1}'", MethodBase.GetCurrentMethod().Name, arg), null);
            }

            return string.Empty;
        }

        public string GetFarmServerDetails(string options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                SPServerCollection servers = SPFarm.Local.Servers;
                if (servers != null && servers.Count > 0)
                {
                    this.GetServerRoleCount(servers, operationReporting);
                }
            }
            catch (Exception exception)
            {
                operationReporting.LogError(exception, "An error occurred while retrieving Farm Server details");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private string GetServerRoleCount(SPServerCollection allServers, OperationReporting opResult)
        {
            try
            {
                int num = 0;
                StringBuilder stringBuilder = new StringBuilder();
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                {
                    foreach (SPServer current in allServers)
                    {
                        try
                        {
                            if (current.Role != SPServerRole.SingleServer && current.Role != SPServerRole.WebFrontEnd)
                            {
                                if (current.Role == SPServerRole.Invalid)
                                {
                                    continue;
                                }

                                if (!current.ServiceInstances.Any((SPServiceInstance service) =>
                                        service.Service is SPWebService && service.Status == SPObjectStatus.Online))
                                {
                                    continue;
                                }
                            }

                            num++;
                        }
                        catch (Exception exception)
                        {
                            string detail =
                                string.Format("An error occurred while retrieving Farm Server details for '{0}' server",
                                    current.Name);
                            opResult.LogError(exception, detail);
                        }
                    }

                    int num2 = allServers.Count<SPServer>();
                    xmlWriter.WriteStartElement("FarmServerDetails");
                    xmlWriter.WriteStartElement("FarmServerDetail");
                    xmlWriter.WriteAttributeString("WFECount", num.ToString());
                    xmlWriter.WriteAttributeString("ServerCount", num2.ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }

                opResult.LogObjectXml(stringBuilder.ToString());
            }
            catch (Exception exception2)
            {
                opResult.LogError(exception2, "An error occurred while retrieving Farm Server details");
            }
            finally
            {
                opResult.End();
            }

            return opResult.ResultXml;
        }

        private int UpdateAvailableItemID(SPList list, int newAvailableId)
        {
            int result = -1;
            if (!OMAdapter.SupportsPartialDBWriting)
            {
                return result;
            }

            Func<int> codeBlockToRetry = () =>
                this.GetDBWriterPartial(list.ParentWeb).UpdateAvailableItemID(list.ID.ToString(), newAvailableId);
            return ExecuteWithRetry.AttemptToExecuteWithRetry<int>(codeBlockToRetry, null);
        }

        private int GetNextAvailableID(SPWeb currentWeb, string sListID)
        {
            if (OMAdapter.SupportsPartialDBWriting)
            {
                return this.GetDBWriterPartial(currentWeb).GetNextAvailableID(sListID);
            }

            return -1;
        }

        private void ClearRecycleBinItem(SPList list, int iItemId)
        {
            if (OMAdapter.SupportsPartialDBWriting)
            {
                IDBWriter dBWriterPartial = this.GetDBWriterPartial(list.ParentWeb);
                byte[] array = dBWriterPartial.CheckForDeletedItem(list.ID.ToString(), iItemId);
                if (array != null && array.Length > 0)
                {
                    Guid itemGuid = new Guid(array);
                    SPRecycleBinItem itemFromRecycleBin = OMAdapter.GetItemFromRecycleBin(list, itemGuid);
                    if (itemFromRecycleBin != null)
                    {
                        itemFromRecycleBin.Delete();
                    }
                }
            }
        }

        private void TryToLoadFormService()
        {
            if (typeof(FormsService) == null)
            {
                throw new Exception("InfoPath service is not available.");
            }
        }

        private bool TryBrowserActivateFormTemplate(SPDocumentLibrary library)
        {
            bool result = false;
            try
            {
                if (library.DocumentTemplateUrl.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase) &&
                    this.IsFormServiceAvailable)
                {
                    SPFile file = library.ParentWeb.GetFile(library.DocumentTemplateUrl);
                    result = this.TryBrowserActivateFormTemplate(file);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private bool TryBrowserActivateFormTemplate(SPFile templateFile)
        {
            bool result = false;
            if (templateFile.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    SPFarm local = SPFarm.Local;
                    FormsService value = local.Services.GetValue<FormsService>("");
                    ConverterMessageCollection converterMessageCollection =
                        value.BrowserEnableUserFormTemplate(templateFile);
                    result = (converterMessageCollection.Count <= 0);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            return result;
        }

        private bool IsFormTemplateBrowserActivated(SPDocumentLibrary library)
        {
            bool result = false;
            if (this.IsFormServiceAvailable)
            {
                SPFile file = library.ParentWeb.GetFile(library.DocumentTemplateUrl);
                result = this.IsFormTemplateBrowserActivated(file);
            }

            return result;
        }

        private bool IsFormTemplateBrowserActivated(SPFile templateFile)
        {
            bool result = false;
            try
            {
                SPFarm local = SPFarm.Local;
                FormsService value = local.Services.GetValue<FormsService>("");
                if (value.IsUserFormTemplateBrowserEnabled(templateFile))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        public string AddFormTemplateToContentType(string targetListId, byte[] docTemplate, string cTypeXml,
            string changedLookupFields)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            if (!base.SharePointVersion.IsSharePoint2010OrLater)
            {
                operationReporting.LogWarning("Target environment is not SharePoint 2010 or later", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            if (!this.IsFormServiceAvailable)
            {
                operationReporting.LogWarning("Infopath Forms Service not present", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            try
            {
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    SPList sPList = null;
                    try
                    {
                        sPList = web.Lists[new Guid(targetListId)];
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        operationReporting.LogWarning(
                            string.Format("Unable to obtain targetList using guid {0}", targetListId), ex.Message);
                    }

                    if (sPList != null)
                    {
                        XmlNode xmlNode = XmlUtility.StringToXmlNode(cTypeXml);
                        string value = xmlNode.Attributes["Name"].Value;
                        SPContentType targetContentType = sPList.ContentTypes[value];
                        this.SetFormForContentType(context.Site, sPList, targetContentType, docTemplate,
                            changedLookupFields, operationReporting);
                    }
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError("Exception occured in method AddFormTemplateToContentType", ex2.Message,
                    ex2.StackTrace, 0, 0);
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void SetFormForContentType(SPSite targetSite, SPList targetList, SPContentType targetContentType,
            byte[] formTemplateCabinetBytes, string changedLookupFields, OperationReporting opResult)
        {
            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                Assembly assembly = typeof(FormsService).Assembly;
                Type type = assembly.GetType("Microsoft.Office.InfoPath.Server.Administration.ListItemSolution");
                Type type2 = assembly.GetType("Microsoft.Office.InfoPath.Server.Converter.ConverterLog");
                if (type == null || type2 == null)
                {
                    throw new Exception("Unable to find ListItemSolution or ConverterLog types.");
                }

                MethodInfo method =
                    type.GetMethod("SetFormForContentType", BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                {
                    throw new Exception("Unable to find SetFormForContentType method.");
                }

                object obj = method.Invoke(null, new object[]
                {
                    targetSite,
                    targetList,
                    targetContentType,
                    formTemplateCabinetBytes
                });
                ConstructorInfo constructor = typeof(DesignCheckerInformation).GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
                    {
                        type2,
                        typeof(string),
                        typeof(int)
                    }, null);
                DesignCheckerInformation designCheckerInformation = (DesignCheckerInformation)constructor.Invoke(
                    new object[]
                    {
                        obj,
                        "MMS",
                        1033
                    });
                if (designCheckerInformation.Messages != null && designCheckerInformation.Messages.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (ConverterMessage current in designCheckerInformation.Messages)
                    {
                        stringBuilder.Length = 0;
                        stringBuilder.AppendLine(string.Format("Id:{0}, Feature:{1}, Category:{2}, Type:{3}",
                            new object[]
                            {
                                current.Id.ToString(),
                                current.Component.ToString(),
                                current.DesignCategory.ToString(),
                                current.Type.ToString()
                            }));
                        if (!string.IsNullOrEmpty(current.DetailedMessage))
                        {
                            stringBuilder.AppendLine(string.Format("DetailedMessage:{0}", current.DetailedMessage));
                        }

                        switch (current.Type)
                        {
                            case (ConverterMessage.MessageType)0:
                                opResult.LogError(current.ShortMessage, stringBuilder.ToString(), string.Empty, 0, 0);
                                continue;
                            case (ConverterMessage.MessageType)2:
                                opResult.LogWarning(current.ShortMessage, stringBuilder.ToString());
                                continue;
                        }

                        opResult.LogInformation(current.ShortMessage, stringBuilder.ToString());
                    }
                }
            }
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringBuilder stringBuilder = new StringBuilder();
                SPList list = web.Lists[new Guid(sListID)];
                string text = web.ServerRelativeUrl.Trim(new char[]
                {
                    '/'
                });
                string strUrl = (text.Length > 0 && sParentFolder.StartsWith(text, true, null))
                    ? sParentFolder.Trim(new char[]
                    {
                        '/'
                    }).Substring(text.Length + 1)
                    : sParentFolder;
                SPFolder folder = web.GetFolder(strUrl);
                string query = Utils.BuildQuery(null, sParentFolder, bRecursive, false, itemTypes, new string[]
                {
                    "ID"
                });
                bool value = true;
                bool value2 = true;
                bool flag;
                SPListItemCollection[] listItems = this.GetListItems(list, query, null, new bool?(value2),
                    new bool?(bRecursive), folder, new bool?(value), null, out flag);
                IEnumerable enumerable;
                if (listItems.Length > 1 && !flag)
                {
                    enumerable = listItems[0];
                }
                else
                {
                    enumerable = this.MergeAllItemCollections(listItems, flag, new string[]
                    {
                        "ID"
                    });
                }

                foreach (SPListItem sPListItem in enumerable)
                {
                    string value3 = sPListItem.ID.ToString();
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append(value3);
                }

                result = stringBuilder.ToString();
            }

            return result;
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
        {
            bool flag = false;
            string result;
            using (Context context = this.GetContext())
            {
                if (!flag)
                {
                    SPWeb web = context.Web;
                    string listItemsInternal = this.GetListItemsInternal(sListID, sIDs, sFields, sParentFolder,
                        bRecursive, itemTypes, web, sListSettings, getOptions);
                    result = listItemsInternal;
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        private string GetListItemsInternal(string sListID, string sIDs, string sFields, string sParentFolder,
            bool bRecursive, ListItemQueryType itemTypes, SPWeb currentWeb, string sListSettings,
            GetListItemOptions getOptions)
        {
            bool bSortOrderViolated;
            SPListItemCollection[] listItemCollections = this.GetListItemCollections(sListID, sIDs, sParentFolder,
                bRecursive, itemTypes, currentWeb, sFields, out bSortOrderViolated);
            return this.SerializeListItems(listItemCollections, bSortOrderViolated, !string.IsNullOrEmpty(sIDs),
                sFields, sParentFolder, bRecursive, currentWeb, getOptions);
        }

        private string SerializeListItems(SPListItemCollection[] itemCollections, bool bSortOrderViolated,
            bool optimizeExternalizationFetch, string sFields, string sParentFolder, bool bRecursive, SPWeb currentWeb,
            GetListItemOptions getOptions)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            List<string> list = null;
            if (!string.IsNullOrEmpty(sFields))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFields);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("./Fields/Field");
                list = new List<string>(xmlNodeList.Count);
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    list.Add(xmlNode.Attributes["Name"].Value);
                }
            }

            IEnumerable enumerable;
            if (itemCollections.Length == 1 && !bSortOrderViolated)
            {
                enumerable = itemCollections[0];
            }
            else
            {
                enumerable = this.MergeAllItemCollections(itemCollections, bSortOrderViolated, new string[]
                {
                    "ID"
                });
            }

            xmlTextWriter.WriteStartElement("ListItems");
            if (itemCollections.Length > 0)
            {
                if (list == null)
                {
                    list = new List<string>();
                    SPListItemCollection sPListItemCollection = itemCollections[0];
                    foreach (string current in sPListItemCollection.QueryFieldNames)
                    {
                        string item = current.StartsWith("ows_", StringComparison.InvariantCulture)
                            ? current.Substring(4)
                            : current;
                        list.Add(item);
                    }
                }

                Hashtable htExternalization = null;
                if (getOptions.IncludeExternalizationData)
                {
                    string[] sDocIds = null;
                    if (optimizeExternalizationFetch &&
                        itemCollections[0].List.BaseType == SPBaseType.DocumentLibrary &&
                        this.ItemCollectionsItemCount(itemCollections) <= 500)
                    {
                        List<string> list2 = new List<string>();
                        foreach (SPListItem sPListItem in enumerable)
                        {
                            if (sPListItem.File != null)
                            {
                                list2.Add(sPListItem.File.UniqueId.ToString());
                            }
                        }

                        sDocIds = list2.ToArray();
                    }

                    htExternalization = ((itemCollections[0].List.BaseType == SPBaseType.DocumentLibrary)
                        ? this.GetExternalizationMetadata(currentWeb, sDocIds, sParentFolder, bRecursive)
                        : null);
                }

                foreach (SPListItem item2 in enumerable)
                {
                    this.GetItemXML(currentWeb, xmlTextWriter, list, item2, true, htExternalization, getOptions);
                }
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions)
        {
            string result;
            using (Context context = this.GetContext())
            {
                using (SPWeb web = context.Web)
                {
                    SPList sPList = web.Lists[new Guid(listID)];
                    bool bSortOrderViolated;
                    SPListItemCollection[] listItemCollections =
                        this.GetListItemCollections(sPList, query, fields, out bSortOrderViolated);
                    result = this.SerializeListItems(listItemCollections, bSortOrderViolated, true, fields,
                        sPList.RootFolder.ServerRelativeUrl.Trim(new char[]
                        {
                            '/'
                        }), true, web, getOptions);
                }
            }

            return result;
        }

        private List<SPListItem> MergeAllItemCollections(SPListItemCollection[] itemCollections, bool bSortItems,
            string[] fieldSortOrder)
        {
            if (itemCollections == null)
            {
                return null;
            }

            int num = 0;
            for (int i = 0; i < itemCollections.Length; i++)
            {
                SPListItemCollection sPListItemCollection = itemCollections[i];
                num += sPListItemCollection.Count;
            }

            List<SPListItem> list = new List<SPListItem>(num);
            for (int j = 0; j < itemCollections.Length; j++)
            {
                SPListItemCollection sPListItemCollection2 = itemCollections[j];
                foreach (SPListItem item in ((IEnumerable)sPListItemCollection2))
                {
                    list.Add(item);
                }
            }

            if (bSortItems && fieldSortOrder != null && fieldSortOrder.Length > 0)
            {
                list.Sort(delegate(SPListItem source, SPListItem target)
                {
                    for (int k = 0; k < fieldSortOrder.Length; k++)
                    {
                        object obj = source[fieldSortOrder[k]];
                        object obj2 = target[fieldSortOrder[k]];
                        IComparable comparable = obj as IComparable;
                        IComparable comparable2 = obj2 as IComparable;
                        int num2 = 0;
                        if (comparable != null)
                        {
                            num2 = comparable.CompareTo(obj2);
                        }
                        else if (comparable2 != null)
                        {
                            num2 = -1 * comparable2.CompareTo(obj);
                        }

                        if (num2 != 0)
                        {
                            return num2;
                        }
                    }

                    return 0;
                });
            }

            return list;
        }

        private SPListItemCollection[] GetListItemCollections(string sListID, string sIDs, string sParentFolder,
            bool bRecursive, ListItemQueryType itemTypes, SPWeb currentWeb, string sFields, out bool bOrderingViolated)
        {
            SPList list = currentWeb.Lists[new Guid(sListID)];
            return this.GetListItemCollections(list, sIDs, sParentFolder, bRecursive, itemTypes, sFields,
                out bOrderingViolated);
        }

        private SPListItemCollection[] GetListItemCollections(SPList list, string sIDs, string sParentFolder,
            bool bRecursive, ListItemQueryType itemTypes, string sFields, out bool bOrderingViolated)
        {
            itemTypes = ((list.BaseTemplate == SPListTemplateType.DiscussionBoard)
                ? (itemTypes | ListItemQueryType.Folder)
                : itemTypes);
            string query = Utils.BuildQuery(sIDs, sParentFolder, bRecursive, false, itemTypes, new string[]
            {
                "ID"
            });
            return this.GetListItemCollections(list, query, sFields, out bOrderingViolated);
        }

        private SPListItemCollection[] GetListItemCollections(SPList list, string query, string sFields,
            out bool bOrderingViolated)
        {
            string viewFields = this.MapFieldsStringToSPQueryViewFields(sFields);
            bool value = true;
            return this.GetListItems(list, query, viewFields, new bool?(value), null, null, null, null,
                out bOrderingViolated);
        }

        private SPListItemCollection[] GetListItems(SPList list, string Query, string ViewFields, bool? DatesInUtc,
            bool? ExpandReccurence, SPFolder Folder, bool? ItemIdQuery, int? MeetingInstanceId,
            out bool bOrderingViolated)
        {
            bOrderingViolated = false;
            SPQuery sPQuery = OMAdapter.BuildSPQueryObject(Query, ViewFields, DatesInUtc, ExpandReccurence, Folder,
                ItemIdQuery, MeetingInstanceId);
            if (!OMAdapter.s_iGetListQueryRowLimit.HasValue)
            {
                OMAdapter.s_iGetListQueryRowLimit = new uint?(4294967295u);
            }

            sPQuery.RowLimit = 2000u;
            sPQuery.RowLimit = OMAdapter.s_iGetListQueryRowLimit.Value;
            SPListItemCollection items = list.GetItems(sPQuery);
            int arg_56_0 = items.Count;
            return new SPListItemCollection[]
            {
                items
            };
        }

        private static SPQuery BuildSPQueryObject(string Query, string ViewFields, bool? DatesInUtc,
            bool? ExpandReccurence, SPFolder Folder, bool? ItemIdQuery, int? MeetingInstanceId)
        {
            SPQuery listQuery = OMAdapter.GetListQuery();
            if (!string.IsNullOrEmpty(Query))
            {
                listQuery.Query = Query;
            }

            if (!string.IsNullOrEmpty(ViewFields))
            {
                listQuery.ViewFields = ViewFields;
            }

            if (DatesInUtc.HasValue)
            {
                listQuery.DatesInUtc = DatesInUtc.Value;
            }

            if (ExpandReccurence.HasValue)
            {
                listQuery.ExpandRecurrence = ExpandReccurence.Value;
            }

            if (Folder != null)
            {
                listQuery.Folder = Folder;
            }

            if (ItemIdQuery.HasValue)
            {
                listQuery.ItemIdQuery = ItemIdQuery.Value;
            }

            if (MeetingInstanceId.HasValue)
            {
                listQuery.MeetingInstanceId = MeetingInstanceId.Value;
            }

            if (!OMAdapter.s_iGetListQueryRowLimit.HasValue)
            {
                OMAdapter.s_iGetListQueryRowLimit = new uint?(4294967295u);
            }

            listQuery.RowLimit = 2000u;
            listQuery.RowLimit = OMAdapter.s_iGetListQueryRowLimit.Value;
            return listQuery;
        }

        private static SPQuery GetListQuery()
        {
            if (!OMAdapter.s_iGetListQueryRowLimit.HasValue)
            {
                OMAdapter.s_iGetListQueryRowLimit = new uint?(4294967295u);
            }

            return new SPQuery
            {
                ExpandUserField = false,
                ViewAttributes = "Scope='RecursiveAll'",
                IncludeMandatoryColumns = false,
                MeetingInstanceId = -2,
                //RowLimit = 2000u,
                RowLimit = OMAdapter.s_iGetListQueryRowLimit.Value,
                DatesInUtc = true
            };
        }

        private string MapFieldsStringToSPQueryViewFields(string sQuery)
        {
            if (sQuery != null)
            {
                sQuery = sQuery.Replace("<Fields>", "");
                sQuery = sQuery.Replace("</Fields>", "");
                sQuery = sQuery.Replace("<Field ", "<FieldRef ");
            }

            return sQuery;
        }

        public void GetItemXML(SPWeb currentWeb, XmlWriter xmlWriter, List<string> fieldNamesRequested, SPListItem item,
            bool bDatesInUtc, Hashtable htExternalization, GetListItemOptions getOptions)
        {
            xmlWriter.WriteStartElement("ListItem");
            if (getOptions != null && getOptions.IncludePermissionsInheritance)
            {
                xmlWriter.WriteAttributeString("HasUniquePermissions", item.HasUniqueRoleAssignments.ToString());
            }

            bool flag = false;
            if (getOptions != null && htExternalization != null && getOptions.IncludeExternalizationData)
            {
                xmlWriter.WriteAttributeString("BinaryAvailable", true.ToString());
                flag = this.WriteItemExternalization(item, htExternalization, xmlWriter);
            }

            foreach (string current in fieldNamesRequested)
            {
                try
                {
                    if (!flag || (!(current == "_DocFlags") && !(current == "_FileSize")))
                    {
                        object obj = item[current];
                        string localName = XmlUtility.EncodeNameStartChars(current);
                        if (obj != null)
                        {
                            xmlWriter.WriteAttributeString(localName,
                                this.GetFieldValue(obj, item.Fields.GetFieldByInternalName(current), currentWeb,
                                    bDatesInUtc));
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString(localName, "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (current == "ID")
                    {
                        throw;
                    }
                }
            }

            xmlWriter.WriteEndElement();
        }

        private bool WriteVersionExternalization(SPListItemVersion version, Hashtable htExternalization,
            XmlWriter xmlWriter)
        {
            return version.ListItem.File != null && this.WriteExternalization(version.ListItem.File.UniqueId.ToString(),
                version.VersionId.ToString(), htExternalization, xmlWriter);
        }

        private bool WriteItemExternalization(SPListItem item, Hashtable htExternalization, XmlWriter xmlWriter)
        {
            return item.File != null && this.WriteExternalization(item.File.UniqueId.ToString(),
                item.File.UIVersion.ToString(), htExternalization, xmlWriter);
        }

        private bool WriteExternalization(string sGuid, string sUIVersion, Hashtable htExternalization,
            XmlWriter xmlWriter)
        {
            if (htExternalization != null)
            {
                string key = sGuid + sUIVersion;
                DataRow dataRow = (DataRow)(htExternalization.ContainsKey(key) ? htExternalization[key] : null);
                if (dataRow != null)
                {
                    ExternalizationUtils.WriteExternalizationData(dataRow, xmlWriter);
                    xmlWriter.WriteAttributeString("_DocFlags", dataRow["_DocFlags"].ToString());
                    xmlWriter.WriteAttributeString("_FileSize", dataRow["_FileSize"].ToString());
                    return true;
                }
            }

            xmlWriter.WriteAttributeString("IsExternalized", false.ToString());
            return false;
        }

        private Hashtable GetExternalizationMetadata(SPWeb currentWeb, string[] sDocIds, string sParentFolder,
            bool bRecursive)
        {
            if (OMAdapter.SupportsDBWriting)
            {
                try
                {
                    return this.GetDBWriter(currentWeb).GetExternalizationMetadata(sDocIds, sParentFolder, bRecursive);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            return null;
        }

        private Hashtable GetExternalizationMetadata(SPWeb currentWeb, string sDocId, bool bIncludeVersions)
        {
            if (OMAdapter.SupportsDBWriting)
            {
                try
                {
                    return this.GetDBWriter(currentWeb).GetExternalizationMetadata(sDocId, bIncludeVersions);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            return null;
        }

        public string HasWorkflows(string sListID, string sItemID)
        {
            if (OMAdapter.SupportsPartialDBWriting)
            {
                using (Context context = this.GetContext())
                {
                    IDBWriter dBWriterPartial = this.GetDBWriterPartial(context.Web);
                    string result;
                    if (dBWriterPartial != null)
                    {
                        result = dBWriterPartial.HasWorkflows(sListID, sItemID);
                        return result;
                    }

                    SPList sPList = context.Web.Lists[new Guid(sListID)];
                    SPListItem itemByUniqueId = sPList.GetItemByUniqueId(new Guid(sItemID));
                    if (itemByUniqueId.Workflows.Count > 0)
                    {
                        result = bool.TrueString;
                        return result;
                    }

                    result = bool.FalseString;
                    return result;
                }
            }

            return bool.FalseString;
        }

        public string GetListItemVersions(string listID, int itemID, string fieldsXml, string configurationXml)
        {
            int num = 0;
            if (!string.IsNullOrEmpty(configurationXml))
            {
                XmlNode node = XmlUtility.StringToXmlNode(configurationXml);
                num = node.GetAttributeValueAsInt(XmlAttributeNames.NoOfLatestVersionsToGet.ToString());
            }

            if (num < 0)
            {
                throw new ArgumentOutOfRangeException("noOfLatestVersionsToGet", "value is less than zero");
            }

            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(listID)];
                SPListItem itemById = sPList.GetItemById(itemID);
                List<SPListItemVersion> list = new List<SPListItemVersion>();
                Hashtable htExternalization = (sPList.BaseType == SPBaseType.DocumentLibrary)
                    ? this.GetExternalizationMetadata(context.Web, itemById.File.UniqueId.ToString(), true)
                    : null;
                List<int> skipVersions = new List<int>();
                if (sPList is SPDocumentLibrary)
                {
                    this.Handle2010SpecialVersionCase(num, itemById, skipVersions);
                }

                this.AddRequiredVersions(num, itemById, list, skipVersions);
                if (num > 0)
                {
                    this.EnsureMajorVersionIsPresentForDraftVersion(itemById, list);
                }

                list.Reverse();
                StringBuilder stringBuilder = new StringBuilder(2048);
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
                {
                    xmlWriter.WriteStartElement(XmlElementNames.ListItems.ToString());
                    foreach (SPListItemVersion current in list)
                    {
                        this.SerialiseListItemVersion(current, xmlWriter, htExternalization, web);
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();
                }

                result = stringBuilder.ToString();
            }

            return result;
        }

        private void Handle2010SpecialVersionCase(int noOfLatestVersionsToGet, SPListItem item, List<int> skipVersions)
        {
            int num = 0;
            if (item.File.Versions.Count + 1 < item.Versions.Count)
            {
                for (int i = 1; i < item.Versions.Count; i++)
                {
                    if (noOfLatestVersionsToGet > 0 && num >= noOfLatestVersionsToGet)
                    {
                        return;
                    }

                    bool flag = false;
                    for (int j = 0; j < item.File.Versions.Count; j++)
                    {
                        if (item.Versions[i].VersionId == item.File.Versions[j].ID)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag && item.File.UIVersion != item.Versions[i].VersionId)
                    {
                        skipVersions.Add(i);
                    }

                    if (noOfLatestVersionsToGet > 0 && item.Versions[i].Level != SPFileLevel.Checkout)
                    {
                        num++;
                    }
                }
            }
        }

        private void EnsureMajorVersionIsPresentForDraftVersion(SPListItem item,
            List<SPListItemVersion> versionsToSerialise)
        {
            if (versionsToSerialise.Count > 0)
            {
                SPListItemVersion sPListItemVersion = versionsToSerialise[versionsToSerialise.Count - 1];
                if (sPListItemVersion.Level == SPFileLevel.Draft)
                {
                    int length = sPListItemVersion.VersionLabel.IndexOf(".", StringComparison.Ordinal);
                    string versionLabel =
                        string.Format("{0}.{1}", sPListItemVersion.VersionLabel.Substring(0, length), 0);
                    SPListItemVersion versionFromLabel = item.Versions.GetVersionFromLabel(versionLabel);
                    if (versionFromLabel != null)
                    {
                        versionsToSerialise.Add(versionFromLabel);
                    }
                }
            }
        }

        private void AddRequiredVersions(int noOfLatestVersionsToGet, SPListItem item,
            List<SPListItemVersion> versionsToSerialise, List<int> skipVersions)
        {
            int num = 0;
            for (int i = 0; i < item.Versions.Count; i++)
            {
                if (!skipVersions.Contains(i))
                {
                    if (noOfLatestVersionsToGet > 0 && num >= noOfLatestVersionsToGet)
                    {
                        return;
                    }

                    SPListItemVersion sPListItemVersion = item.Versions[i];
                    versionsToSerialise.Add(sPListItemVersion);
                    if (noOfLatestVersionsToGet > 0 && sPListItemVersion.Level != SPFileLevel.Checkout)
                    {
                        num++;
                    }
                }
            }
        }

        private void SerialiseListItemVersion(SPListItemVersion version, XmlWriter xmlWriter,
            Hashtable htExternalization, SPWeb currentWeb)
        {
            xmlWriter.WriteStartElement("ListItem");
            try
            {
                xmlWriter.WriteAttributeString("_VersionString", version.VersionLabel);
                xmlWriter.WriteAttributeString("_VersionNumber", version.VersionId.ToString());
                xmlWriter.WriteAttributeString("_VersionLevel", ((int)version.Level).ToString());
                xmlWriter.WriteAttributeString("_CheckinComment", this.GetCheckinComment(version));
                xmlWriter.WriteAttributeString("BinaryAvailable", true.ToString());
                bool flag = false;
                try
                {
                    flag = this.WriteVersionExternalization(version, htExternalization, xmlWriter);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }

                foreach (SPField sPField in ((IEnumerable)version.Fields))
                {
                    object obj;
                    if (sPField.InternalName == "File_x0020_Size" && version.ListItem.File != null)
                    {
                        SPFileVersion versionFromID =
                            version.ListItem.File.Versions.GetVersionFromID(version.VersionId);
                        obj = ((versionFromID != null) ? versionFromID.Size : version[sPField.InternalName]);
                    }
                    else
                    {
                        obj = version[sPField.InternalName];
                    }

                    if (sPField.InternalName != "_CheckinComment" && (!flag ||
                                                                      (!(sPField.InternalName == "_DocFlags") &&
                                                                       !(sPField.InternalName == "_FileSize"))))
                    {
                        if (obj != null)
                        {
                            xmlWriter.WriteAttributeString(sPField.InternalName,
                                this.GetFieldValue(obj, sPField, currentWeb, true));
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString(sPField.InternalName, "");
                        }
                    }
                }
            }
            finally
            {
                xmlWriter.WriteEndElement();
            }
        }

        private string GetCheckinComment(SPListItemVersion version)
        {
            if (version.ListItem.File == null)
            {
                return "";
            }

            string result;
            if (this.GetVersionFromLabel(version.ListItem.File.Versions, version.VersionLabel, out result))
            {
                return version.ListItem.File.CheckInComment;
            }

            return result;
        }

        private bool GetVersionFromLabel(SPFileVersionCollection versionCollection, string sVersionLabel,
            out string sCheckInComment)
        {
            foreach (SPFileVersion sPFileVersion in ((IEnumerable)versionCollection))
            {
                if (sPFileVersion.VersionLabel.Equals(sVersionLabel, StringComparison.OrdinalIgnoreCase))
                {
                    if (sPFileVersion.CheckInComment == null)
                    {
                        sCheckInComment = "";
                    }
                    else
                    {
                        sCheckInComment = sPFileVersion.CheckInComment;
                    }

                    return false;
                }
            }

            sCheckInComment = "";
            return true;
        }

        private bool AttemptToSanitiseMetaInfoValue(string originalValue, out string sanitisedValue)
        {
            sanitisedValue = null;
            if (XmlExtensions.IsValidXmlAttributeValue(originalValue))
            {
                return false;
            }

            string[] array = originalValue.Split(new string[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    if (!XmlExtensions.IsValidXmlAttributeValue(array[i]))
                    {
                        string[] array2 = array[i].Split(new string[]
                        {
                            "SW|"
                        }, StringSplitOptions.RemoveEmptyEntries);
                        if (array2.Length == 2 && XmlExtensions.IsValidXmlAttributeValue(array2[0]))
                        {
                            stringBuilder.AppendLine(string.Format("{0}SW|", array2[0]));
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine(array[i]);
                    }
                }

                sanitisedValue = stringBuilder.ToString();
            }
            else
            {
                sanitisedValue = string.Empty;
            }

            return true;
        }

        public string GetFieldValue(object objectValue, SPField field, SPWeb currentWeb, bool bDatesInUtc)
        {
            string text = null;
            SPFieldType sPFieldType = field.Type;
            if (sPFieldType == SPFieldType.Invalid)
            {
                if (field.FieldValueType == typeof(SPFieldLookupValue) ||
                    field.FieldValueType == typeof(SPFieldLookupValueCollection))
                {
                    sPFieldType = SPFieldType.Lookup;
                }
                else if (objectValue != null && typeof(DateTime).IsAssignableFrom(objectValue.GetType()))
                {
                    sPFieldType = SPFieldType.DateTime;
                }
            }

            switch (sPFieldType)
            {
                case SPFieldType.Text:
                case SPFieldType.Note:
                case SPFieldType.URL:
                case SPFieldType.Computed:
                case SPFieldType.GridChoice:
                    text = objectValue.ToString();
                    goto IL_545;
                case SPFieldType.DateTime:
                {
                    DateTime dateTime = (DateTime)objectValue;
                    if (bDatesInUtc)
                    {
                        dateTime = Utils.MakeTrueUTCDateTime(dateTime);
                    }
                    else
                    {
                        dateTime = currentWeb.RegionalSettings.TimeZone.LocalTimeToUTC(dateTime);
                    }

                    text = Utils.FormatDate(dateTime);
                    goto IL_545;
                }
                case SPFieldType.Lookup:
                    if (objectValue.GetType().IsAssignableFrom(typeof(SPFieldLookupValueCollection)))
                    {
                        SPFieldLookupValueCollection sPFieldLookupValueCollection =
                            (SPFieldLookupValueCollection)objectValue;
                        string text2 = null;
                        for (int i = 0; i < sPFieldLookupValueCollection.Count; i++)
                        {
                            SPFieldLookupValue sPFieldLookupValue = sPFieldLookupValueCollection[i];
                            string text3 = sPFieldLookupValue.LookupId.ToString();
                            if (string.IsNullOrEmpty(text2))
                            {
                                text2 = text3;
                            }
                            else
                            {
                                text2 = text2 + ";#" + text3;
                            }
                        }

                        text = text2;
                        goto IL_545;
                    }

                    text = objectValue.ToString();
                    if (field.Id == SPBuiltInFieldId.MetaInfo && !string.IsNullOrEmpty(text))
                    {
                        string text4 = null;
                        if (this.AttemptToSanitiseMetaInfoValue(text, out text4))
                        {
                            text = text4;
                            goto IL_545;
                        }

                        goto IL_545;
                    }
                    else
                    {
                        string[] array = text.Split(new string[]
                        {
                            ";#"
                        }, StringSplitOptions.None);
                        if (array.Length > 1)
                        {
                            SPFieldLookup sPFieldLookup = (SPFieldLookup)field;
                            if (!sPFieldLookup.CanBeDeleted && (string.IsNullOrEmpty(sPFieldLookup.LookupList) ||
                                                                !Utils.IsGUID(sPFieldLookup.LookupList)))
                            {
                                text = array[1];
                            }
                            else
                            {
                                bool flag = true;
                                try
                                {
                                    new Guid(sPFieldLookup.LookupList);
                                }
                                catch (Exception ex)
                                {
                                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                    flag = false;
                                }

                                if (flag)
                                {
                                    text = array[0];
                                }
                                else
                                {
                                    text = array[1];
                                }
                            }
                        }

                        if ((field.InternalName.Equals("FileRef", StringComparison.InvariantCulture) ||
                             field.InternalName.Equals("FileDirRef", StringComparison.InvariantCulture)) &&
                            text != null && text.StartsWith("/", StringComparison.Ordinal))
                        {
                            text = text.TrimStart(new char[]
                            {
                                '/'
                            });
                            goto IL_545;
                        }

                        goto IL_545;
                    }

                    break;
                case SPFieldType.Number:
                case SPFieldType.Currency:
                    if (objectValue.GetType() == typeof(int))
                    {
                        text = OMAdapter.s_IntConverter.ConvertToString(null, OMAdapter.s_CultureInfo, objectValue);
                        goto IL_545;
                    }

                    if (objectValue.GetType() == typeof(double))
                    {
                        text = OMAdapter.s_DoubleConverter.ConvertToString(null, OMAdapter.s_CultureInfo, objectValue);
                        goto IL_545;
                    }

                    goto IL_545;
                case SPFieldType.Calculated:
                {
                    SPFieldCalculated sPFieldCalculated = (SPFieldCalculated)field;
                    text = objectValue.ToString();
                    int num = text.IndexOf(";#", StringComparison.Ordinal);
                    if (num >= 0)
                    {
                        text = text.Substring(num + 2);
                    }

                    if (sPFieldCalculated.OutputType == SPFieldType.DateTime)
                    {
                        DateTime dateTime2;
                        if (DateTime.TryParse(text, out dateTime2))
                        {
                            if (bDatesInUtc)
                            {
                                dateTime2 = dateTime2.ToUniversalTime();
                            }
                            else
                            {
                                dateTime2 = currentWeb.RegionalSettings.TimeZone.LocalTimeToUTC(dateTime2);
                            }

                            text = Utils.FormatDate(dateTime2);
                            goto IL_545;
                        }

                        text = "";
                        goto IL_545;
                    }
                    else
                    {
                        if (sPFieldCalculated.OutputType != SPFieldType.Number &&
                            sPFieldCalculated.OutputType != SPFieldType.Currency)
                        {
                            goto IL_545;
                        }

                        int num2;
                        if (int.TryParse(text, out num2))
                        {
                            text = OMAdapter.s_IntConverter.ConvertToString(null, OMAdapter.s_CultureInfo, num2);
                            goto IL_545;
                        }

                        double num3;
                        if (double.TryParse(text, out num3))
                        {
                            text = OMAdapter.s_DoubleConverter.ConvertToString(null, OMAdapter.s_CultureInfo, num3);
                            goto IL_545;
                        }

                        goto IL_545;
                    }

                    break;
                }
                case SPFieldType.User:
                {
                    if (objectValue != null && objectValue.ToString() == "***")
                    {
                        text = "";
                        goto IL_545;
                    }

                    if (objectValue == null)
                    {
                        goto IL_545;
                    }

                    string[] array2 = objectValue.ToString().Split(new string[]
                    {
                        ";#"
                    }, StringSplitOptions.None);
                    int num4 = -1;
                    if (!int.TryParse(array2[0], out num4))
                    {
                        text = array2[0];
                        goto IL_545;
                    }

                    for (int j = 0; j < array2.Length; j += 2)
                    {
                        int num5 = -1;
                        if (int.TryParse(array2[j], out num5))
                        {
                            SPUser sPUser = null;
                            string text5 = string.Empty;
                            try
                            {
                                sPUser = currentWeb.SiteUsers.GetByID(num5);
                            }
                            catch (Exception)
                            {
                                text5 = this.GetDeletedUserLoginName(currentWeb, num5);
                            }

                            if (string.IsNullOrEmpty(text5))
                            {
                                if (sPUser != null)
                                {
                                    text5 = sPUser.LoginName;
                                }
                                else if (array2.Length > j + 1)
                                {
                                    string[] array3 = array2[j + 1].Split(new string[]
                                    {
                                        ",#"
                                    }, StringSplitOptions.None);
                                    text5 = ((array3.Length >= 5) ? array3[1] : array2[j + 1]);
                                    if (string.IsNullOrEmpty(text5))
                                    {
                                        text5 = objectValue.ToString();
                                    }
                                }
                                else
                                {
                                    text5 = array2[j];
                                }
                            }

                            if (string.IsNullOrEmpty(text))
                            {
                                text = text5;
                            }
                            else
                            {
                                text = text + "," + text5;
                            }
                        }
                    }

                    goto IL_545;
                }
            }

            text = objectValue.ToString();
            if (string.Equals("HTML", field.TypeAsString, StringComparison.InvariantCultureIgnoreCase))
            {
                text = XmlExtensions.ReplaceInvalidEscapeCharInXmlAttributeValue(text);
                return text;
            }

            int num6 = text.IndexOf(";#", StringComparison.Ordinal);
            if (num6 > 0)
            {
                text = text.Substring(num6 + 2);
            }

            IL_545:
            text = XmlExtensions.ReplaceInvalidEscapeCharInXmlAttributeValue(text);
            return text;
        }

        private string GetDeletedUserLoginName(SPWeb web, int userId)
        {
            string result = string.Empty;
            try
            {
                SPQuery query = new SPQuery
                {
                    Query = string.Format(
                        "<Where><And><Eq><FieldRef Name='Deleted' /><Value Type='Boolean'>1</Value></Eq><Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq></And></Where>",
                        userId),
                    ViewFields = "<FieldRef Name='Name' />"
                };
                SPListItemCollection items = web.SiteUserInfoList.GetItems(query);
                if (items != null && items.Count > 0)
                {
                    result = Convert.ToString(items[0]["ows_Name"]);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            byte[] result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                string text = web.ServerRelativeUrl.Trim(new char[]
                {
                    '/'
                });
                string text2 = (text.Length > 0 && sFileDirRef.StartsWith(text, true, null))
                    ? sFileDirRef.Trim(new char[]
                    {
                        '/'
                    }).Substring(text.Length + 1)
                    : sFileDirRef;
                string strUrl;
                if (string.IsNullOrEmpty(text2))
                {
                    strUrl = sFileLeafRef;
                }
                else
                {
                    strUrl = text2 + "/" + sFileLeafRef;
                }

                SPFileLevel sPFileLevel = (SPFileLevel)Enum.Parse(typeof(SPFileLevel), iLevel.ToString());
                SPFile sPFile = (sDocID != null) ? web.GetFile(new Guid(sDocID)) : web.GetFile(strUrl);
                if (!sPFile.Exists)
                {
                    throw new Exception("File does not exist");
                }

                if (sPFile.Level == sPFileLevel)
                {
                    result = sPFile.OpenBinary();
                }
                else
                {
                    foreach (SPFileVersion sPFileVersion in ((IEnumerable)sPFile.Versions))
                    {
                        if (sPFileVersion.Level == sPFileLevel && sPFileVersion.IsCurrentVersion)
                        {
                            result = sPFileVersion.OpenBinary();
                            return result;
                        }
                    }

                    result = null;
                }
            }

            return result;
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            byte[] result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                byte[] array = null;
                SPFile file = web.GetFile(new Guid(sDocID));
                if (file != null)
                {
                    SPFileVersion versionFromID = file.Versions.GetVersionFromID(iVersion);
                    bool flag = false;
                    bool flag2 = false;
                    if (versionFromID.Properties.ContainsKey("PublishingHidden"))
                    {
                        flag2 = bool.TryParse(versionFromID.Properties["PublishingHidden"].ToString(), out flag);
                    }

                    if (!flag || !flag2)
                    {
                        array = versionFromID.OpenBinary();
                    }
                }

                result = array;
            }

            return result;
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            if (OMAdapter.SupportsDBWriting)
            {
                using (Context context = this.GetContext())
                {
                    return this.GetDBWriter(context.Web).GetDocumentBlobRef(sDocID, sFileDirRef, sFileLeafRef, iLevel);
                }
            }

            return null;
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            if (OMAdapter.SupportsDBWriting)
            {
                using (Context context = this.GetContext())
                {
                    return this.GetDBWriter(context.Web)
                        .GetDocumentVersionBlobRef(sDocID, sFileDirRef, sFileLeafRef, iVersion);
                }
            }

            return null;
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPFile file = web.GetFile(sDocumentServerRelativeUrl);
                if (file != null && file.Exists)
                {
                    return bool.TrueString;
                }
            }

            return bool.FalseString;
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                SPList sPList = web.Lists[new Guid(sListID)];
                SPListItem itemById = sPList.GetItemById(iItemID);
                SPFileCollection files = itemById.ParentList.RootFolder.SubFolders["Attachments"]
                    .SubFolders[itemById.ID.ToString()].Files;
                Hashtable hashtable = OMAdapter.SupportsDBWriting
                    ? this.GetDBWriter(context.Web).GetExternalizationMetadata(null,
                        files.Folder.ServerRelativeUrl.TrimStart(new char[]
                        {
                            '/'
                        }), false)
                    : null;
                xmlTextWriter.WriteStartElement("Attachments");
                foreach (SPFile sPFile in ((IEnumerable)files))
                {
                    xmlTextWriter.WriteStartElement("Attachment");
                    xmlTextWriter.WriteAttributeString("LeafName", sPFile.Name);
                    xmlTextWriter.WriteAttributeString("DocID", sPFile.UniqueId.ToString());
                    DataRow dataRow = null;
                    if (hashtable != null)
                    {
                        string key = sPFile.UniqueId.ToString() + sPFile.UIVersion.ToString();
                        dataRow = (DataRow)(hashtable.ContainsKey(key) ? hashtable[key] : null);
                    }

                    if (dataRow != null)
                    {
                        ExternalizationUtils.WriteExternalizationData(dataRow, xmlTextWriter);
                        xmlTextWriter.WriteAttributeString("_DocFlags", dataRow["_DocFlags"].ToString());
                        xmlTextWriter.WriteAttributeString("_FileSize", dataRow["_FileSize"].ToString());
                    }
                    else
                    {
                        xmlTextWriter.WriteAttributeString("IsExternalized", false.ToString());
                    }

                    xmlTextWriter.WriteAttributeString("BinaryAvailable", true.ToString());
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            if (!OMAdapter.SupportsDBWriting)
            {
                throw new NotImplementedException("Search functionality is not available with this type of connection");
            }

            string result;
            using (Context context = this.GetContext())
            {
                result = this.GetDBWriter(context.Web).SearchForDocument(sSearchTerm, sOptionsXml);
            }

            return result;
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            if (!OMAdapter.SupportsDBWriting)
            {
                throw new NotImplementedException(
                    "Database analysis functionality is not available with this type of connection");
            }

            string result;
            using (Context context = this.GetContext())
            {
                result = this.GetDBWriter(context.Web).AnalyzeChurn(pivotDate, sListID, iItemID, bRecursive);
            }

            return result;
        }

        public string FindUniquePermissions()
        {
            if (!OMAdapter.SupportsDBWriting)
            {
                throw new NotImplementedException(
                    "Database analysis functionality is not available with this type of connection");
            }

            string result;
            using (Context context = this.GetContext())
            {
                result = this.GetDBWriter(context.Web).FindUniquePermissions();
            }

            return result;
        }

        public string FindAlerts()
        {
            if (!OMAdapter.SupportsDBWriting)
            {
                throw new NotImplementedException(
                    "Database analysis functionality is not available with this type of connection");
            }

            string result;
            using (Context context = this.GetContext())
            {
                result = this.GetDBWriter(context.Web).FindAlerts();
            }

            return result;
        }

        public string GetWorkflows(string sListID, int iItemID)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                if (OMAdapter.SupportsPartialDBWriting)
                {
                    using (Context context = this.GetContext())
                    {
                        using (context.Site.OpenWeb(this.ServerRelativeUrl))
                        {
                            IDBWriter dBWriterPartial = this.GetDBWriterPartial(context.Web);
                            if (dBWriterPartial != null)
                            {
                                string workflows = dBWriterPartial.GetWorkflows(sListID, iItemID);
                                operationReporting.LogObjectXml(workflows);
                            }
                            else
                            {
                                operationReporting.LogWarning(Resources.UnableToResolveDBAdapterType,
                                    Resources.UnableToResolveDBAdapterTypeDetails);
                            }
                        }

                        goto IL_7E;
                    }
                }

                OMAdapter.CheckPartialDBWritingAvailabilityWithDetails(operationReporting);
                IL_7E: ;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(Resources.UnableToObtainWorkflowInstances, ex.Message, ex.StackTrace, 0, 0);
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string HasUniquePermissions(string listID, int listItemID)
        {
            throw new NotSupportedException(Resources.HasUniquePermissionsNotNecessary);
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            try
            {
                using (Context context = this.GetContext())
                {
                    using (SPWeb web = context.Web)
                    {
                        Guid uniqueID = new Guid(listId);
                        SPList sPList = web.Lists[uniqueID];
                        SPQuery query = new SPQuery
                        {
                            ViewAttributes = "Scope=\"Recursive\"",
                            Query =
                                "<Where><Or><Eq><FieldRef Name = 'File_x0020_Type' /><Value Type= 'Text'>aspx</Value></Eq>" +
                                "<BeginsWith><FieldRef Name = 'HTML_x0020_File_x0020_Type' /><Value Type= 'Text'>InfoPath</Value></BeginsWith></Or></Where>",
                            ViewFields = "<FieldRef Name='ID' />" + "<FieldRef Name='File_x0020_Type' />" +
                                         "<FieldRef Name='HTML_x0020_File_x0020_Type' />"
                        };
                        SPListItemCollection items = sPList.GetItems(query);
                        if (items.Count > 0)
                        {
                            string result = true.ToString();
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                string result = true.ToString();
                return result;
            }

            return false.ToString();
        }

        public string AddDocument(string sListID, string sFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions options)
        {
            SPFile sPFile;
            return this.AddDocument(sListID, sFolder, sListItemXML, fileContents, options, out sPFile);
        }

        private string AddDocument(string sListID, string sFolder, string sListItemXML, byte[] fileContents,
            AddDocumentOptions options, out SPFile file)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                bool overwrite = options.Overwrite;
                bool correctInvalidNames = options.CorrectInvalidNames;
                bool flag = options.PreserveID && options.AllowDBWriting;
                bool allowDBWriting = options.AllowDBWriting;
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                    int nextAvailableID;
                    string text;
                    SPUser sPUser;
                    SPUser sPUser2;
                    DateTime dateTime;
                    DateTime dt;
                    this.GetListItemFileInfo(xmlNode, out nextAvailableID, out text, out sPUser, out sPUser2,
                        out dateTime, out dt, correctInvalidNames, web);
                    string text2;
                    string sVersionString;
                    SPCheckinType sPCheckinType;
                    string text3;
                    this.GetVersionInfo(xmlNode, out text2, out sVersionString, out sPCheckinType, out text3);
                    string attributeValueAsString = xmlNode.GetAttributeValueAsString("ContentTypeId");
                    bool isAssetLibraryVideo = false;
                    SPList sPList = web.Lists[new Guid(sListID)];
                    SPDocumentLibrary sPDocumentLibrary = (SPDocumentLibrary)sPList;
                    string fullPath = this.GetFullPath(sPList, sFolder);
                    SPFolder folder = web.GetFolder(fullPath);
                    if (options.OverrideCheckouts)
                    {
                        foreach (SPCheckedOutFile current in sPDocumentLibrary.CheckedOutFiles)
                        {
                            if (current.DirName.ToLower() == fullPath.ToLower().TrimStart(new char[]
                                {
                                    '/'
                                }) && current.LeafName.ToLower() == text.ToLower())
                            {
                                current.TakeOverCheckOut();
                                break;
                            }
                        }
                    }

                    file = web.GetFile(fullPath + "\\" + text);
                    if (file.Exists && overwrite)
                    {
                        file.Delete();
                        file = null;
                    }
                    else if (!file.Exists)
                    {
                        file = null;
                    }

                    if (!overwrite && !flag && allowDBWriting)
                    {
                        nextAvailableID = this.GetNextAvailableID(context.Web, sListID);
                    }

                    bool flag2 = file == null;
                    bool flag3 = Utils.IsDocumentWikiPage(xmlNode, "");
                    bool flag4 = sPList.BaseTemplate == SPListTemplateType.HomePageLibrary;
                    if (flag2)
                    {
                        int num = -1;
                        try
                        {
                            if (flag && OMAdapter.GetItemByID(sPList, nextAvailableID, false, false, 0u) == null)
                            {
                                this.ClearRecycleBinItem(sPList, nextAvailableID);
                                num = this.UpdateAvailableItemID(sPList, nextAvailableID);
                            }

                            if (flag3)
                            {
                                file = folder.Files.Add(fullPath + "/" + text, SPTemplateFileType.WikiPage);
                            }
                            else if (flag4)
                            {
                                SPMeeting meetingInformation = SPMeeting.GetMeetingInformation(web);
                                string strUrl = null;
                                int instanceId = -1;
                                string text4 = xmlNode.Attributes["FileLeafRef"].Value;
                                int num2 = text4.LastIndexOf('.');
                                if (num2 >= 0)
                                {
                                    text4 = text4.Substring(0, num2);
                                }

                                string text5 = xmlNode.Attributes["FileDirRef"].Value;
                                int num3 = text5.LastIndexOf('/');
                                if (num3 >= 0)
                                {
                                    text5 = text5.Substring(num3 + 1);
                                }

                                if (int.TryParse(text5, out instanceId))
                                {
                                    meetingInformation.AddPage(text4, instanceId, out strUrl);
                                }
                                else
                                {
                                    meetingInformation.AddPage(text4, -1, out strUrl);
                                }

                                file = web.GetFile(strUrl);
                            }
                            else if (!Utils.IsVideoFile(sPList.BaseTemplate.ToString(), attributeValueAsString, null) ||
                                     !base.SharePointVersion.IsSharePoint2013OrLater)
                            {
                                file = OMAdapter.AddFileToFolder(web, folder, text, new byte[0]);
                            }

                            goto IL_35A;
                        }
                        finally
                        {
                            if (num > nextAvailableID)
                            {
                                this.UpdateAvailableItemID(sPList, num);
                            }
                        }
                    }

                    if (options.OverrideCheckouts && file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        try
                        {
                            file.UndoCheckOut();
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }

                    IL_35A:
                    int[] tempVersions = this.IncrementDocumentVersions(sVersionString, ref file, sPList, flag3, flag4);
                    string sSharePointPath = file.ParentFolder.ParentWeb.Url + "/" + file.ParentFolder.Url;
                    bool flag5 = false;
                    bool flag6 = false;
                    if (options.SideLoadDocumentsToStoragePoint &&
                        bool.TryParse(this.StoragePointAvailable(string.Empty), out flag5) && flag5 &&
                        bool.TryParse(this.StoragePointProfileConfigured(sSharePointPath), out flag6) && flag6)
                    {
                        this.UpdateDocumentData(web, sPList, folder, file, xmlNode, new byte[0], options, flag2, false);
                        this.AddDocumentToStoragePointEndpoint(file, fileContents);
                    }
                    else
                    {
                        this.UpdateDocumentData(web, sPList, folder, file, xmlNode, fileContents, options, flag2,
                            isAssetLibraryVideo);
                    }

                    this.DeleteTempDocumentVersions(file, tempVersions);
                    if (file.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
                    {
                        this.TryBrowserActivateFormTemplate(file);
                    }

                    StringBuilder stringBuilder = new StringBuilder(300);
                    using (StringWriter stringWriter = new StringWriter(stringBuilder))
                    {
                        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                        {
                            xmlTextWriter.WriteStartElement("ListItems");
                            xmlTextWriter.WriteStartElement("ListItem");
                            xmlTextWriter.WriteAttributeString("ID", file.Item.ID.ToString());
                            xmlTextWriter.WriteAttributeString("FileDirRef", folder.ServerRelativeUrl.Trim(new char[]
                            {
                                '/'
                            }));
                            xmlTextWriter.WriteAttributeString("GUID", file.Item["GUID"].ToString());
                            xmlTextWriter.WriteAttributeString("FileLeafRef", file.Name);
                            xmlTextWriter.WriteAttributeString("Modified", Utils.FormatDate(dt));
                            xmlTextWriter.WriteEndElement();
                            xmlTextWriter.WriteEndElement();
                        }
                    }

                    operationReporting.LogObjectXml(stringBuilder.ToString());
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                file = null;
                operationReporting.LogError(ex2, "Main catch");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            string result;
            using (Context context = this.GetContext())
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sFolderXML);
                SPWeb web = context.Web;
                SPFolder folder = web.GetFolder(xmlNode.Attributes["ParentFolderPath"].Value);
                SPFolder folder2 = folder.SubFolders.Add(xmlNode.Attributes["Name"].Value);
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                OMAdapter.AddFolderToXml(xmlWriter, folder2);
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
        {
            string result;
            using (Context context = this.GetContext())
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sFileXML);
                SPWeb web = context.Web;
                SPFolder folder = web.GetFolder(xmlNode.Attributes["ParentFolderPath"].Value);
                SPUser sPUser = this.LookupUser(xmlNode.Attributes["Author"].Value, web);
                SPUser sPUser2 = this.LookupUser(xmlNode.Attributes["ModifiedBy"].Value, web);
                DateTime timeCreated = Utils.ParseDateAsUtc(xmlNode.Attributes["TimeCreated"].Value);
                DateTime timeLastModified = Utils.ParseDateAsUtc(xmlNode.Attributes["TimeLastModified"].Value);
                if (sPUser2 == null)
                {
                    sPUser2 = web.CurrentUser;
                }

                SPFile file = folder.Files.Add(xmlNode.Attributes["Name"].Value, fileContents, sPUser ?? sPUser2,
                    sPUser2, timeCreated, timeLastModified);
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                OMAdapter.AddFileToXml(xmlWriter, file);
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                SPList list = web.Lists[new Guid(sListID)];
                int num;
                string text;
                SPUser sPUser;
                SPUser sPUser2;
                DateTime dateTime;
                DateTime dt;
                this.GetListItemFileInfo(xmlNode, out num, out text, out sPUser, out sPUser2, out dateTime, out dt,
                    false, web);
                string text2;
                string text3;
                SPCheckinType sPCheckinType;
                string text4;
                this.GetVersionInfo(xmlNode, out text2, out text3, out sPCheckinType, out text4);
                string fullPath = this.GetFullPath(list, sParentFolder);
                SPFolder folder = web.GetFolder(fullPath);
                SPFile file = web.GetFile(fullPath + "\\" + sFileLeafRef);
                this.UpdateDocumentData(web, list, folder, file, xmlNode, fileContents, updateOptions, false, false);
                StringBuilder stringBuilder = new StringBuilder(300);
                using (StringWriter stringWriter = new StringWriter(stringBuilder))
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.WriteStartElement("ListItems");
                        xmlTextWriter.WriteStartElement("ListItem");
                        xmlTextWriter.WriteAttributeString("ID", file.Item.ID.ToString());
                        xmlTextWriter.WriteAttributeString("FileDirRef", folder.ServerRelativeUrl.Trim(new char[]
                        {
                            '/'
                        }));
                        xmlTextWriter.WriteAttributeString("GUID", file.Item["GUID"].ToString());
                        xmlTextWriter.WriteAttributeString("FileLeafRef", file.Name);
                        xmlTextWriter.WriteAttributeString("Modified", Utils.FormatDate(dt));
                        xmlTextWriter.WriteEndElement();
                        xmlTextWriter.WriteEndElement();
                    }
                }

                result = stringBuilder.ToString();
            }

            return result;
        }

        private void UpdateDocumentData(SPWeb currentWeb, SPList list, SPFolder folder, SPFile file,
            XmlNode listItemXML, byte[] fileContents, IUpdateDocumentOptions options, bool bAdding,
            bool isAssetLibraryVideo = false)
        {
            bool flag = Utils.IsDocumentWikiPage(listItemXML, "");
            bool flag2 = list.BaseTemplate == SPListTemplateType.HomePageLibrary;
            AddDocumentOptions addDocumentOptions = options as AddDocumentOptions;
            bool bPreserveSharePointDocumentID =
                addDocumentOptions != null && addDocumentOptions.PreserveSharePointDocumentIDs;
            int num;
            string text;
            SPUser createdBy;
            SPUser modifiedBy;
            DateTime createdOn;
            DateTime modifiedOn;
            this.GetListItemFileInfo(listItemXML, out num, out text, out createdBy, out modifiedBy, out createdOn,
                out modifiedOn, false, currentWeb);
            string text2;
            string sVersionString;
            SPCheckinType sPCheckinType;
            string sCheckinComments;
            this.GetVersionInfo(listItemXML, out text2, out sVersionString, out sPCheckinType, out sCheckinComments);
            bool flag3 = false;
            if (file.CheckedOutBy == null)
            {
                this.CheckOutFile(file);
                flag3 = true;
            }

            if (!flag && !flag2 && !isAssetLibraryVideo)
            {
                if (!ExternalizationUtils.IsExternalizedContent(listItemXML.Attributes["IsExternalized"]) ||
                    !options.ShallowCopyExternalizedData)
                {
                    file = OMAdapter.AddFileToFolder(currentWeb, folder, file.Name, fileContents);
                }
                else
                {
                    file = OMAdapter.AddFileToFolder(currentWeb, folder, file.Name, new byte[0]);
                }
            }

            this.UpdateItemMetadata(list, file.Item, listItemXML, false, bPreserveSharePointDocumentID);
            this.AssignCreateModifyProperties(file, createdBy, modifiedBy, createdOn, modifiedOn);
            OMAdapter.CallUpdateItem(currentWeb, file.Item);
            this.CheckInFileByUser(file, sCheckinComments, sPCheckinType, modifiedBy, bAdding && flag3, sVersionString);
            bool flag4 = false;
            if (list.EnableModeration)
            {
                this.UpdateItemModerationStatus(file.Item, listItemXML);
                if ((file.Item.ModerationInformation.Status == SPModerationStatusType.Approved ||
                     file.Item.ModerationInformation.Status == SPModerationStatusType.Denied) &&
                    sPCheckinType == SPCheckinType.MajorCheckIn)
                {
                    file.Item.UpdateOverwriteVersion();
                }
                else
                {
                    file.Item.SystemUpdate();
                }
            }

            if (!flag4)
            {
                this.AssignCreateModifyProperties(file, createdBy, modifiedBy, createdOn, modifiedOn);
                if (list.EnableModeration)
                {
                    string attributeValueAsString = listItemXML.GetAttributeValueAsString("_ModerationComments");
                    file.Item.ModerationInformation.Comment = attributeValueAsString;
                }

                OMAdapter.CallSystemUpdateItem(currentWeb, file.Item, new bool?(false));
            }

            bool flag5 = false;
            if (ExternalizationUtils.IsExternalizedContent(listItemXML.Attributes["IsExternalized"]) &&
                options.ShallowCopyExternalizedData &&
                bool.TryParse(this.StoragePointAvailable(string.Empty), out flag5) && flag5)
            {
                this.UpdateExternalizationMetadataInDB(currentWeb, file, fileContents, listItemXML);
            }
        }

        private void AssignCreateModifyProperties(SPFile file, SPUser createdBy, SPUser modifiedBy, DateTime createdOn,
            DateTime modifiedOn)
        {
            file.Item["Modified_x0020_By"] = modifiedBy;
            file.Item["Created_x0020_By"] = createdBy;
            file.Item["Modified"] = modifiedOn;
            file.Item["Created"] = createdOn;
        }

        private void CheckOutFile(SPFile file)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    file.CheckOut();
                    break;
                }
                catch (SPException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    int errorCode = ex.ErrorCode;
                    if ((errorCode != -2147217873 && errorCode != -2147467259 && errorCode != -2130246376) || i >= 4)
                    {
                        throw;
                    }

                    Thread.Sleep(50);
                }
            }
        }

        private void CallSystemUpdateApprovalVersion(SPWeb currentWeb, SPListItem item)
        {
            using (new OMAdapterHttpContext(currentWeb))
            {
                MethodInfo method = typeof(SPListItem).GetMethod("UpdateInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, OMAdapter.s_updateTypeParameters, null);
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        method.Invoke(item, new object[]
                        {
                            false,
                            false,
                            Guid.Empty,
                            true,
                            false,
                            true,
                            false,
                            false,
                            false,
                            true
                        });
                        break;
                    }
                    catch (TargetInvocationException ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        if (!(ex.InnerException is SPException))
                        {
                            throw ex.InnerException;
                        }

                        int errorCode = ((SPException)ex.InnerException).ErrorCode;
                        if ((errorCode != -2147467259 && errorCode != -2130246376) || i >= 4)
                        {
                            throw ex.InnerException;
                        }

                        Thread.Sleep(50);
                    }
                }
            }
        }

        private static void CallUpdateItem(SPWeb currentWeb, SPListItem item)
        {
            using (new OMAdapterHttpContext(currentWeb))
            {
                OMAdapter.ItemUpdateInternal(item, 5);
            }
        }

        private static void ItemUpdateInternal(SPListItem item, int iTries)
        {
            try
            {
                item.Update();
            }
            catch (SPException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                int errorCode = ex.ErrorCode;
                if ((errorCode != -2147217873 && errorCode != -2147467259 && errorCode != -2130246376) || iTries <= 0)
                {
                    throw;
                }

                Thread.Sleep(50);
                OMAdapter.ItemUpdateInternal(item, iTries - 1);
            }
        }

        private static void CallSystemUpdateItem(SPWeb currentWeb, SPListItem item)
        {
            OMAdapter.CallSystemUpdateItem(currentWeb, item, null);
        }

        private static void CallSystemUpdateItem(SPWeb currentWeb, SPListItem item, bool? bIncrementVersions)
        {
            using (new OMAdapterHttpContext(currentWeb))
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        if (bIncrementVersions.HasValue)
                        {
                            item.SystemUpdate(bIncrementVersions.Value);
                        }
                        else
                        {
                            item.SystemUpdate();
                        }

                        break;
                    }
                    catch (SPException ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        int errorCode = ex.ErrorCode;
                        if ((errorCode != -2147217873 && errorCode != -2147467259 && errorCode != -2130246376) ||
                            i >= 4)
                        {
                            throw;
                        }

                        Thread.Sleep(50);
                    }
                }
            }
        }

        private static SPFile AddFileToFolder(SPWeb currentWeb, SPFolder folder, string sFileName, byte[] fileContents)
        {
            SPFile result;
            using (new OMAdapterHttpContext(currentWeb))
            {
                using (new MemoryStream(fileContents))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            result = folder.Files.Add(sFileName, fileContents, true);
                            return result;
                        }
                        catch (SPException ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            int errorCode = ex.ErrorCode;
                            if ((errorCode != -2147217873 && errorCode != -2147467259 && errorCode != -2130246376) ||
                                i >= 4)
                            {
                                throw;
                            }

                            Thread.Sleep(50);
                        }
                    }

                    result = null;
                }
            }

            return result;
        }

        private void GetVersionInfo(XmlNode listItemXml, out string sLevel, out string sVersionString,
            out SPCheckinType checkInType, out string sCheckInComments)
        {
            sLevel = ((listItemXml.Attributes["_VersionLevel"] == null)
                ? "1"
                : listItemXml.Attributes["_VersionLevel"].Value);
            checkInType = ((sLevel == "2") ? SPCheckinType.MinorCheckIn : SPCheckinType.MajorCheckIn);
            sCheckInComments = ((listItemXml.Attributes["_CheckinComment"] != null)
                ? listItemXml.Attributes["_CheckinComment"].Value
                : "");
            sVersionString = ((listItemXml.Attributes["_VersionString"] == null)
                ? "1.0"
                : listItemXml.Attributes["_VersionString"].Value);
        }

        private void GetListItemFileInfo(XmlNode listItemXml, out int iItemId, out string sFileName,
            out SPUser creationUser, out SPUser modificationUser, out DateTime createdOn, out DateTime modifiedOn,
            bool bCorrectInvalidNames, SPWeb currentWeb)
        {
            iItemId = Convert.ToInt32(listItemXml.Attributes["ID"].Value);
            sFileName = listItemXml.Attributes["FileLeafRef"].Value;
            if (bCorrectInvalidNames)
            {
                int num = 0;
                while (sFileName.Contains(char.ConvertFromUtf32(num)))
                {
                    num++;
                }

                char cReplacement = char.ConvertFromUtf32(num)[0];
                sFileName = Utils.RectifyName(sFileName, cReplacement, this, RectifyClass.SPListItem);
                sFileName.Replace(cReplacement.ToString(), "");
            }

            creationUser = ((listItemXml.Attributes["Author"] == null)
                ? null
                : this.LookupUser(listItemXml.Attributes["Author"].Value, currentWeb));
            modificationUser = ((listItemXml.Attributes["Editor"] == null)
                ? null
                : this.LookupUser(listItemXml.Attributes["Editor"].Value, currentWeb));
            createdOn = ((listItemXml.Attributes["Created"] == null)
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(listItemXml.Attributes["Created"].Value));
            modifiedOn = ((listItemXml.Attributes["Modified"] == null)
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(listItemXml.Attributes["Modified"].Value));
            createdOn = currentWeb.RegionalSettings.TimeZone.UTCToLocalTime(createdOn);
            modifiedOn = currentWeb.RegionalSettings.TimeZone.UTCToLocalTime(modifiedOn);
            if (createdOn >= modifiedOn)
            {
                modifiedOn = createdOn;
            }
        }

        private void DeleteTempDocumentVersions(SPFile file, int[] tempVersions)
        {
            for (int i = 0; i < tempVersions.Length; i++)
            {
                int vid = tempVersions[i];
                try
                {
                    file.Versions.DeleteByID(vid);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            if (file.Item.Level == SPFileLevel.Published || tempVersions.Length > 0)
            {
                for (int j = file.Versions.Count - 1; j >= 0; j--)
                {
                    SPFileVersion sPFileVersion = file.Versions[j];
                    if (sPFileVersion.Level == SPFileLevel.Published && !sPFileVersion.IsCurrentVersion)
                    {
                        if (!(sPFileVersion.CheckInComment == "Temporary Version - (To be deleted)"))
                        {
                            break;
                        }

                        try
                        {
                            file.Versions.DeleteByID(sPFileVersion.ID);
                            break;
                        }
                        catch (Exception ex2)
                        {
                            OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                            break;
                        }
                    }
                }
            }
        }

        private void DeleteTempListItemVersions(SPListItem item, string[] tempVersions)
        {
            for (int i = 0; i < tempVersions.Length; i++)
            {
                string versionLabel = tempVersions[i];
                try
                {
                    SPListItemVersion versionFromLabel = item.Versions.GetVersionFromLabel(versionLabel);
                    versionFromLabel.Delete();
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        private int[] IncrementDocumentVersions(string sVersionString, ref SPFile file, SPList parentList,
            bool isWikiPage, bool isMeetingWorkspacePage)
        {
            int[] result;
            using (new OMAdapterHttpContext(file.ParentFolder.ParentWeb))
            {
                if (!parentList.EnableVersioning)
                {
                    result = new int[0];
                }
                else
                {
                    ArrayList arrayList = new ArrayList();
                    OMAdapter.VersionString versionString = new OMAdapter.VersionString(sVersionString);
                    OMAdapter.VersionString versionString2 = new OMAdapter.VersionString(file.UIVersionLabel);
                    int num = versionString.MajorVersion - versionString2.MajorVersion - 1 +
                              ((versionString.MinorVersion > 0) ? 1 : 0);
                    if (file.CheckedOutBy != null)
                    {
                        file.CheckIn("First version - To be deleted");
                    }

                    for (int i = 0; i < num; i++)
                    {
                        this.CheckOutFile(file);
                        if (!isWikiPage && !isMeetingWorkspacePage)
                        {
                            file.ParentFolder.Files.Add(file.Name, new byte[0], true);
                        }

                        file.CheckIn("Temporary Version - (To be deleted)", SPCheckinType.MajorCheckIn);
                        if (parentList.EnableModeration)
                        {
                            file.Approve("Temp Version Approval");
                        }

                        arrayList.Add(file.UIVersion);
                    }

                    if (parentList.EnableMinorVersions)
                    {
                        versionString2 = new OMAdapter.VersionString(file.UIVersionLabel);
                        int num2 = versionString.MinorVersion - versionString2.MinorVersion - 1;
                        for (int j = 0; j < num2; j++)
                        {
                            if (file.CheckedOutBy == null)
                            {
                                this.CheckOutFile(file);
                            }

                            if (!isWikiPage && !isMeetingWorkspacePage)
                            {
                                file = file.ParentFolder.Files.Add(file.Name, new byte[0], true);
                            }

                            file.CheckIn("Temporary Version - To be deleted", SPCheckinType.MinorCheckIn);
                            arrayList.Add(file.UIVersion);
                        }
                    }

                    int[] array = new int[arrayList.Count];
                    arrayList.CopyTo(array);
                    result = array;
                }
            }

            return result;
        }

        private string[] IncrementListItemVersions(SPList list, XmlNode itemXML, SPListItem item)
        {
            if (!list.EnableVersioning || itemXML.Attributes["_UIVersionString"] == null)
            {
                return new string[0];
            }

            ArrayList arrayList = new ArrayList();
            try
            {
                int num = (int)Convert.ToDouble(itemXML.Attributes["_UIVersionString"].Value, OMAdapter.s_CultureInfo);
                int num2 = (item["_UIVersionString"] == null)
                    ? 0
                    : ((int)Convert.ToDouble(item["_UIVersionString"], OMAdapter.s_CultureInfo));
                int num3 = num - num2 - 1;
                if (num3 > 0)
                {
                    for (int i = 0; i < num3; i++)
                    {
                        item["Modified"] = item["Modified"];
                        item.Update();
                        arrayList.Add(item["_UIVersionString"]);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            string[] array = new string[arrayList.Count];
            arrayList.CopyTo(array);
            return array;
        }

        public string AddFolder(string sListID, string sFolder, string sFolderXML, AddFolderOptions Options)
        {
            bool overwrite = Options.Overwrite;
            bool preserveID = Options.PreserveID;
            string folders;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(sListID)];
                string fullPath = this.GetFullPath(sPList, sFolder);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sFolderXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                string text = (firstChild.Attributes["FileLeafRef"] != null)
                    ? firstChild.Attributes["FileLeafRef"].Value
                    : firstChild.Attributes["Title"].Value;
                text = text.Trim();
                SPFolder folder = web.GetFolder(fullPath);
                SPFolder sPFolder = null;
                try
                {
                    sPFolder = folder.SubFolders[text];
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }

                if (overwrite && sPFolder != null)
                {
                    sPFolder.Delete();
                }

                if (!overwrite && !preserveID && sPFolder != null)
                {
                    folders = this.GetFolders(sListID, sPFolder.Item.ID.ToString(), fullPath);
                }
                else
                {
                    int num = -1;
                    SPListItem sPListItem = null;
                    if (preserveID && firstChild.Attributes["ID"] != null &&
                        !Utils.IsGUID(firstChild.Attributes["ID"].Value))
                    {
                        try
                        {
                            num = Convert.ToInt32(firstChild.Attributes["ID"].Value);
                        }
                        catch (Exception ex2)
                        {
                            OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                            try
                            {
                                new Guid(firstChild.Attributes["ID"].Value);
                            }
                            catch (Exception ex3)
                            {
                                OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                                throw new Exception("ID value for folder XML is invalid. Message: " + ex2.Message, ex2);
                            }
                        }
                    }

                    int num2 = -1;
                    try
                    {
                        if (preserveID && num > 0 && sPList.BaseType == SPBaseType.DocumentLibrary &&
                            OMAdapter.GetItemByID(sPList, num, false, false, 0u) == null)
                        {
                            this.ClearRecycleBinItem(sPList, num);
                            num2 = this.UpdateAvailableItemID(sPList, num);
                        }

                        try
                        {
                            bool flag = sPList.BaseTemplate == (SPListTemplateType)2100;
                            SPFileSystemObjectType underlyingObjectType =
                                flag ? SPFileSystemObjectType.File : SPFileSystemObjectType.Folder;
                            bool flag2 = false;
                            if (base.SharePointVersion.IsSharePoint2010OrLater && this.AreDocumentSetsSupported)
                            {
                                string text2 = (firstChild.Attributes["HTML_x0020_File_x0020_Type"] != null)
                                    ? firstChild.Attributes["HTML_x0020_File_x0020_Type"].Value
                                    : string.Empty;
                                if (text2.Equals("SharePoint.DocumentSet", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    flag2 = true;
                                    try
                                    {
                                        sPListItem = this.AddDocumentSet(sPList, fullPath, firstChild, text);
                                    }
                                    catch (Exception ex4)
                                    {
                                        OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                                        flag2 = false;
                                    }
                                }
                            }

                            if (!flag2)
                            {
                                sPListItem = this.GetEmptyListItemCollection(sPList, false)
                                    .Add(fullPath, underlyingObjectType, text);
                            }
                            else
                            {
                                this.UpdateItemMetadata(sPList, sPListItem, firstChild, flag);
                                sPListItem.Update();
                            }

                            if (preserveID && num > 0 && sPList.BaseType != SPBaseType.DocumentLibrary)
                            {
                                this.SetIDToCreateItemAt(sPListItem, num);
                            }

                            if (!flag2)
                            {
                                this.UpdateItemMetadata(sPList, sPListItem, firstChild, flag);
                                sPListItem.Update();
                            }
                        }
                        catch (Exception ex5)
                        {
                            OMAdapter.LogExceptionDetails(ex5, MethodBase.GetCurrentMethod().Name, null);
                            if (!sPList.MultipleDataList || sPList.BaseType != SPBaseType.DocumentLibrary)
                            {
                                throw this.TransformItemUpdateException(ex5);
                            }

                            try
                            {
                                folder = web.GetFolder(fullPath);
                                sPFolder = folder.SubFolders[text];
                                sPListItem = sPFolder.Item;
                            }
                            catch (Exception ex6)
                            {
                                OMAdapter.LogExceptionDetails(ex6, MethodBase.GetCurrentMethod().Name, null);
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        if (preserveID && num2 > 0 && num2 > num && sPList.BaseType == SPBaseType.DocumentLibrary)
                        {
                            this.UpdateAvailableItemID(sPList, num2);
                        }
                    }

                    folders = this.GetFolders(sListID, sPListItem.ID.ToString(), fullPath);
                }
            }

            return folders;
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList list = web.Lists[new Guid(sListID)];
                string fullPath = this.GetFullPath(list, sFolder);
                SPFolder folder = web.GetFolder(fullPath);
                if (folder != null)
                {
                    folder.Delete();
                }
            }

            return string.Empty;
        }

        private string GetFullPath(SPList list, string sFolder)
        {
            return list.RootFolder.ServerRelativeUrl +
                   ((string.IsNullOrEmpty(sFolder) || sFolder.StartsWith("/", StringComparison.Ordinal)) ? "" : "/") +
                   sFolder;
        }

        public string AddListItem(string sListID, string sFolder, string sListItemXML, string[] attachmentNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions options)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            bool flag = false;
            try
            {
                bool initialVersion = options.InitialVersion;
                bool preserveID = options.PreserveID;
                int num = 0;
                if (options.ParentID.HasValue)
                {
                    num = options.ParentID.Value;
                }

                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    SPList sPList = web.Lists[new Guid(sListID)];
                    string fullPath = this.GetFullPath(sPList, sFolder);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sListItemXML);
                    XmlNode firstChild = xmlDocument.FirstChild;
                    int num2 = -1;
                    if (sPList.BaseType == SPBaseType.Issue &&
                        !string.IsNullOrEmpty(firstChild.GetAttributeValueAsString("IssueID")))
                    {
                        num2 = firstChild.GetAttributeValueAsInt("IssueID");
                    }
                    else if (firstChild.Attributes["ID"] != null)
                    {
                        num2 = firstChild.GetAttributeValueAsInt("ID");
                    }

                    string attributeValueAsString = firstChild.GetAttributeValueAsString("Title");
                    SPListItem sPListItem = null;
                    string[] array = null;
                    bool flag2 = firstChild.GetAttributeValueAsString("FSObjType").Equals("1");
                    string attributeValueAsString2 = firstChild.GetAttributeValueAsString("FileLeafRef");
                    bool flag3 = false;
                    if (num2 >= 0 && (flag2 || preserveID || !initialVersion))
                    {
                        if (flag2 && !string.IsNullOrEmpty(attributeValueAsString2))
                        {
                            try
                            {
                                SPFolder sPFolder = web.GetFolder(fullPath).SubFolders[attributeValueAsString2];
                                sPListItem = sPFolder.Item;
                            }
                            catch (Exception ex)
                            {
                                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            }

                            flag3 = (OMAdapter.GetItemByID(sPList, num2, false, false, 1u) != null);
                        }
                        else
                        {
                            sPListItem = OMAdapter.GetItemByID(sPList, num2, false, false, 1u);
                            flag3 = (sPListItem != null);
                            if (flag3 && !options.Overwrite)
                            {
                                sPListItem = OMAdapter.GetItemByID(sPList, num2, true, false, 1u);
                                flag3 = (sPListItem != null);
                            }
                        }
                    }

                    if (sPListItem != null && options.Overwrite)
                    {
                        sPListItem.Delete();
                        sPListItem = null;
                    }

                    int num3 = -1;
                    try
                    {
                        if (num2 > 0 && preserveID && !flag3 && options.AllowDBWriting &&
                            OMAdapter.SupportsPartialDBWriting)
                        {
                            this.ClearRecycleBinItem(sPList, num2);
                            num3 = this.UpdateAvailableItemID(sPList, num2);
                        }

                        if (base.SharePointVersion.IsSharePoint2013OrLater)
                        {
                            SPListTemplateType arg_22E_0 = sPList.BaseTemplate;
                        }

                        try
                        {
                            bool flag4 = false;
                            if (sPListItem == null)
                            {
                                if (sPList.BaseTemplate == SPListTemplateType.DiscussionBoard)
                                {
                                    if (num == 0)
                                    {
                                        sPListItem = SPUtility.CreateNewDiscussion(
                                            this.GetEmptyListItemCollection(sPList, false), attributeValueAsString);
                                        if (!string.IsNullOrEmpty(attributeValueAsString2))
                                        {
                                            sPListItem["FileLeafRef"] = attributeValueAsString2;
                                        }

                                        if (firstChild.Attributes["BestAnswerId"] != null)
                                        {
                                            sPListItem["BestAnswerId"] = firstChild.Attributes["BestAnswerId"].Value;
                                        }
                                    }
                                    else
                                    {
                                        SPListItem itemById = sPList.GetItemById(num);
                                        sPListItem = SPUtility.CreateNewDiscussionReply(itemById);
                                    }

                                    if (num2 > 0 && preserveID && !flag3 &&
                                        (!options.AllowDBWriting || !OMAdapter.SupportsPartialDBWriting))
                                    {
                                        this.SetIDToCreateItemAt(sPListItem, num2);
                                    }
                                }
                                else
                                {
                                    if (flag2)
                                    {
                                        flag4 = (sPList.BaseTemplate == (SPListTemplateType)2100);
                                        SPFileSystemObjectType underlyingObjectType =
                                            flag4 ? SPFileSystemObjectType.File : SPFileSystemObjectType.Folder;
                                        string text = (firstChild.Attributes["FileLeafRef"] != null)
                                            ? firstChild.Attributes["FileLeafRef"].Value
                                            : firstChild.Attributes["Title"].Value;
                                        bool flag5 = false;
                                        if (base.SharePointVersion.IsSharePoint2010OrLater &&
                                            this.AreDocumentSetsSupported)
                                        {
                                            string text2 = (firstChild.Attributes["HTML_x0020_File_x0020_Type"] != null)
                                                ? firstChild.Attributes["HTML_x0020_File_x0020_Type"].Value
                                                : string.Empty;
                                            if (text2.Equals("SharePoint.DocumentSet",
                                                    StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                flag5 = true;
                                                try
                                                {
                                                    sPListItem = this.AddDocumentSet(sPList, fullPath, firstChild,
                                                        text);
                                                }
                                                catch (Exception ex2)
                                                {
                                                    OMAdapter.LogExceptionDetails(ex2,
                                                        MethodBase.GetCurrentMethod().Name, null);
                                                    flag5 = false;
                                                }
                                            }
                                        }

                                        if (!flag5)
                                        {
                                            sPListItem = this.GetEmptyListItemCollection(sPList, false)
                                                .Add(fullPath, underlyingObjectType, text);
                                        }
                                    }
                                    else
                                    {
                                        sPListItem = this.GetEmptyListItemCollection(sPList, false)
                                            .Add(fullPath, SPFileSystemObjectType.File);
                                    }

                                    if (num2 > 0 && preserveID && !flag3 &&
                                        (!options.AllowDBWriting || !OMAdapter.SupportsPartialDBWriting))
                                    {
                                        this.SetIDToCreateItemAt(sPListItem, num2);
                                    }

                                    array = this.IncrementListItemVersions(sPList, firstChild, sPListItem);
                                }
                            }
                            else if (sPList.BaseTemplate != SPListTemplateType.DiscussionBoard)
                            {
                                array = this.IncrementListItemVersions(sPList, firstChild, sPListItem);
                            }

                            this.UpdateListItemData(web, sPList, sPListItem, firstChild, attachmentNames,
                                attachmentContents, options, flag4);
                            if (array != null)
                            {
                                this.DeleteTempListItemVersions(sPListItem, array);
                            }
                        }
                        finally
                        {
                            if (num2 > 0 && preserveID && !flag3 && num3 > 0 && num3 > num2 && options.AllowDBWriting &&
                                OMAdapter.SupportsPartialDBWriting)
                            {
                                this.UpdateAvailableItemID(sPList, num3);
                            }
                        }
                    }
                    catch (Exception ex3)
                    {
                        OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                        if (!flag2 || !sPList.MultipleDataList || sPList.BaseType != SPBaseType.DocumentLibrary)
                        {
                            throw this.TransformItemUpdateException(ex3);
                        }

                        try
                        {
                            SPFolder folder = web.GetFolder(fullPath);
                            SPFolder sPFolder2 = folder.SubFolders[attributeValueAsString2];
                            sPListItem = sPFolder2.Item;
                        }
                        catch (Exception ex4)
                        {
                            OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                            throw this.TransformItemUpdateException(ex3);
                        }
                    }

                    if (sPListItem != null)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                        xmlTextWriter.WriteStartElement("ListItems");
                        this.GetItemXML(web, xmlTextWriter, new List<string>(new string[]
                        {
                            "ID",
                            "Modified",
                            "FileRef",
                            "FSObjType"
                        }), sPListItem, false, null, new GetListItemOptions());
                        xmlTextWriter.WriteEndElement();
                        xmlTextWriter.Flush();
                        operationReporting.LogObjectXml(stringBuilder.ToString());
                    }
                }
            }
            catch (Exception ex5)
            {
                OMAdapter.LogExceptionDetails(ex5, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex5, "Main catch");
            }
            finally
            {
                if (!flag)
                {
                    operationReporting.End();
                }
            }

            return operationReporting.ResultXml;
        }

        private void UpdateRequestMetaData(SPListItem listItem, XmlNode itemNode, XmlNode versionNode)
        {
            if (listItem != null)
            {
                this.UpdateRequestStatusAndPermission(listItem, itemNode);
                this.UpdateRequestItemAuthorMetaData(listItem, versionNode, false);
                this.CallSystemUpdateApprovalVersion(listItem.Web, listItem);
                this.UpdateRequestVersionsMetaData(listItem, itemNode);
            }
        }

        private void UpdateRequestVersionsMetaData(SPListItem listItem, XmlNode itemNode)
        {
            XmlNodeList xmlNodeList = itemNode.SelectNodes("./Versions/ListItem");
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                for (int i = 1; i < xmlNodeList.Count; i++)
                {
                    XmlNode xmlNode = xmlNodeList[i];
                    string attributeValueAsString = xmlNode.GetAttributeValueAsString("Conversation");
                    if (!string.IsNullOrEmpty(attributeValueAsString))
                    {
                        listItem["Conversation"] = attributeValueAsString;
                        OMAdapter.CallUpdateItem(listItem.Web, listItem);
                        this.UpdateRequestItemAuthorMetaData(listItem, xmlNode, true);
                    }

                    this.UpdateRequestStatusAndPermission(listItem, itemNode);
                    this.CallSystemUpdateApprovalVersion(listItem.Web, listItem);
                }
            }
        }

        private void UpdateRequestItemAuthorMetaData(SPListItem listItem, XmlNode itemNode, bool isConversationUpdated)
        {
            DateTime dateTime = string.IsNullOrEmpty(itemNode.GetAttributeValueAsString("Created"))
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(itemNode.GetAttributeValueAsString("Created"));
            dateTime = listItem.Web.RegionalSettings.TimeZone.UTCToLocalTime(dateTime);
            DateTime dateTime2 = string.IsNullOrEmpty(itemNode.GetAttributeValueAsString("Modified"))
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(itemNode.GetAttributeValueAsString("Modified"));
            dateTime2 = listItem.Web.RegionalSettings.TimeZone.UTCToLocalTime(dateTime2);
            DateTime dateTime3 = string.IsNullOrEmpty(itemNode.GetAttributeValueAsString("Expires"))
                ? DateTime.UtcNow
                : Utils.ParseDateAsUtc(itemNode.GetAttributeValueAsString("Expires"));
            dateTime3 = listItem.Web.RegionalSettings.TimeZone.UTCToLocalTime(dateTime3);
            string attributeValueAsString = itemNode.GetAttributeValueAsString("Editor");
            string attributeValueAsString2 = itemNode.GetAttributeValueAsString("Author");
            listItem["Created"] = dateTime;
            listItem["Expires"] = dateTime3;
            listItem["Modified"] = dateTime2;
            if (isConversationUpdated)
            {
                this.UpdateUserFieldValue(listItem, attributeValueAsString2, "Author");
                this.UpdateUserFieldValue(listItem, attributeValueAsString, "Editor");
            }
        }

        private SPUser AddUserWithLimitedAccess(SPWeb web, string userName)
        {
            return web.EnsureUser(userName);
        }

        private void UpdateRequestStatusAndPermission(SPListItem listItem, XmlNode itemNode)
        {
            string attributeValueAsString = itemNode.GetAttributeValueAsString("PermissionLevelRequested");
            string attributeValueAsString2 = itemNode.GetAttributeValueAsString("PermissionType");
            string attributeValueAsString3 = itemNode.GetAttributeValueAsString("Status");
            listItem["PermissionType"] = attributeValueAsString2;
            if (attributeValueAsString2.Equals("SharePoint Group", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    SPGroup sPGroup = OMAdapter.LookupGroup(attributeValueAsString, listItem.Web);
                    if (sPGroup != null)
                    {
                        listItem["PermissionLevelRequested"] = sPGroup.ID;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(attributeValueAsString))
            {
                listItem["PermissionLevelRequested"] = attributeValueAsString;
            }

            listItem["Status"] = attributeValueAsString3;
            if (attributeValueAsString3.Equals("1"))
            {
                string attributeValueAsString4 = itemNode.GetAttributeValueAsString("ApprovedBy");
                this.UpdateUserFieldValue(listItem, attributeValueAsString4, "Approved By");
            }
        }

        private void UpdateUserFieldValue(SPListItem listItem, string username, string fieldname)
        {
            if (!string.IsNullOrEmpty(username))
            {
                SPUser sPUser = this.LookupUser(username, listItem.Web);
                if (sPUser == null)
                {
                    sPUser = this.AddUserWithLimitedAccess(listItem.Web, username);
                }

                if (sPUser != null)
                {
                    listItem[fieldname] = sPUser;
                }
            }
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPList sPList = context.Web.Lists[new Guid(sListID)];
                SPListItem itemById = sPList.GetItemById(iItemID);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sListItemXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                if (firstChild.Name.Equals(XmlElementNames.UpdateListItemRoleInheritance.ToString()))
                {
                    bool attributeValueAsBoolean =
                        firstChild.GetAttributeValueAsBoolean(XmlAttributeNames.UniquePermissions.ToString());
                    result = OMAdapter.UpdateListItemRoleInheritance(itemById, attributeValueAsBoolean);
                }
                else
                {
                    this.UpdateListItemData(context.Web, sPList, itemById, firstChild, attachmentNames,
                        attachmentContents, updateOptions, false);
                    result = this.GetListItems(sPList.ID.ToString(), itemById.ID.ToString(), null, null, true,
                        ListItemQueryType.ListItem | ListItemQueryType.Folder, null, new GetListItemOptions());
                }
            }

            return result;
        }

        private static string UpdateListItemRoleInheritance(SPListItem listItem, bool breakInheritance)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                if (breakInheritance)
                {
                    listItem.BreakRoleInheritance(true);
                }
                else
                {
                    listItem.ResetRoleInheritance();
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "Failed to update list item role inheritance");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void UpdateListItemData(SPWeb currentWeb, SPList list, SPListItem item, XmlNode itemXML,
            string[] attachmentNames, byte[][] attachmentContents, IUpdateListItemOptions options,
            bool bConvertToFolder)
        {
            if (list.EnableModeration && (!list.EnableVersioning ||
                                          itemXML.Attributes["_ModerationStatus"].Value == "0" ||
                                          itemXML.Attributes["_ModerationStatus"].Value == "1"))
            {
                OMAdapter.CallUpdateItem(currentWeb, item);
            }

            this.UpdateItemMetadata(list, item, itemXML, bConvertToFolder);
            bool flag = attachmentNames != null && attachmentNames.Length > 0;
            if (flag)
            {
                try
                {
                    XmlNodeList nodesToCheck = itemXML.SelectNodes("//Attachments/Attachment");
                    for (int i = 0; i < attachmentNames.Length; i++)
                    {
                        string text = attachmentNames[i];
                        foreach (string a in item.Attachments)
                        {
                            if (a == text)
                            {
                                item.Attachments.Delete(text);
                                break;
                            }
                        }

                        XmlNode xmlNode = XmlUtility.MatchFirstAttributeValue("LeafName", text, nodesToCheck);
                        XmlAttribute externalizedAttr = (xmlNode != null) ? xmlNode.Attributes["IsExternalized"] : null;
                        if (attachmentContents[i] != null)
                        {
                            item.Attachments.Add(text, attachmentContents[i]);
                        }
                        else if (ExternalizationUtils.IsExternalizedContent(externalizedAttr) &&
                                 options.ShallowCopyExternalizedData)
                        {
                            item.Attachments.Add(text, ExternalizationUtils.GetBlankBlobRef());
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception("Error adding attachments: " + ex.Message);
                }
            }

            bool flag2 = false;
            if (list.EnableModeration && (itemXML.Attributes["_ModerationStatus"].Value == "0" ||
                                          itemXML.Attributes["_ModerationStatus"].Value == "1"))
            {
                if (itemXML.Attributes["FSObjType"] != null && itemXML.Attributes["FSObjType"].Value.Equals("1") &&
                    list.BaseType == SPBaseType.DocumentLibrary)
                {
                    OMAdapter.CallUpdateItem(currentWeb, item);
                    flag2 = true;
                }

                this.UpdateItemModerationStatus(item, itemXML);
                if (itemXML.Attributes["FSObjType"] != null && itemXML.Attributes["FSObjType"].Value.Equals("1") &&
                    list.BaseType == SPBaseType.DocumentLibrary)
                {
                    OMAdapter.CallSystemUpdateItem(currentWeb, item, new bool?(false));
                }
            }
            else if (list.EnableModeration && itemXML.Attributes["_ModerationStatus"].Value == "2")
            {
                OMAdapter.CallUpdateItem(currentWeb, item);
                flag2 = true;
                this.UpdateItemModerationStatus(item, itemXML);
                OMAdapter.CallSystemUpdateItem(currentWeb, item, new bool?(false));
            }

            if ((!list.EnableModeration || list.BaseType != SPBaseType.DocumentLibrary) && !flag2)
            {
                OMAdapter.CallUpdateItem(currentWeb, item);
            }

            if (flag && OMAdapter.SupportsDBWriting && options.ShallowCopyExternalizedData)
            {
                SPFileCollection files = item.ParentList.RootFolder.SubFolders["Attachments"]
                    .SubFolders[item.ID.ToString()].Files;
                XmlNodeList nodesToCheck2 = itemXML.SelectNodes("//Attachments/Attachment");
                for (int j = 0; j < attachmentNames.Length; j++)
                {
                    string text2 = attachmentNames[j];
                    SPFile file = files[text2];
                    XmlNode xmlNode2 = XmlUtility.MatchFirstAttributeValue("LeafName", text2, nodesToCheck2);
                    XmlAttribute externalizedAttr2 = (xmlNode2 != null) ? xmlNode2.Attributes["IsExternalized"] : null;
                    if (ExternalizationUtils.IsExternalizedContent(externalizedAttr2))
                    {
                        this.UpdateExternalizationMetadataInDB(currentWeb, file, attachmentContents[j], xmlNode2);
                    }
                }
            }
        }

        private void UpdateExternalizationMetadataInDB(SPWeb currentWeb, SPFile file, byte[] fileContents,
            XmlNode fileXML)
        {
            bool flag = false;
            if (OMAdapter.SupportsDBWriting && bool.TryParse(this.StoragePointAvailable(string.Empty), out flag) &&
                flag)
            {
                byte[] rbsId = null;
                long num = -1L;
                int num2 = -1;
                if (fileXML.Attributes["RbsId"] != null)
                {
                    string value = fileXML.Attributes["RbsId"].Value;
                    rbsId = Utils.HexToDecimal(value);
                }

                if (fileXML.Attributes["_DocFlags"] != null)
                {
                    int.TryParse(fileXML.Attributes["_DocFlags"].Value, out num2);
                }

                if (fileXML.Attributes["_FileSize"] != null)
                {
                    long.TryParse(fileXML.Attributes["_FileSize"].Value, out num);
                }

                byte[] content = this.UpdateBlobRef(fileContents, currentWeb);
                if (num > 0L && num2 > 0)
                {
                    this.GetDBWriter(currentWeb)
                        .UpdateDocstreamContent(file.UniqueId, file.UIVersion, num2, num, content, rbsId);
                }
            }
        }

        private byte[] UpdateBlobRef(byte[] content, SPWeb currentWeb)
        {
            string storagePointProfileConfiguration = this.GetStoragePointProfileConfiguration(currentWeb.Url);
            XmlNode xmlNode = null;
            if (!string.IsNullOrEmpty(storagePointProfileConfiguration))
            {
                xmlNode = XmlUtility.StringToXmlNode(storagePointProfileConfiguration);
            }

            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                throw new InvalidOperationException(string.Format(Resources.FS_ShallowCopyingInvalid,
                    Resources.StoragePointNotConfigured));
            }

            XmlNode xmlNode2 = xmlNode.Attributes["ProfileId"];
            if (xmlNode2 == null || string.IsNullOrEmpty(xmlNode2.Value) || !Utils.IsGUID(xmlNode2.Value))
            {
                throw new InvalidOperationException(string.Format(Resources.FS_ShallowCopyingInvalid,
                    Resources.StoragePointNotConfigured));
            }

            XmlNode xmlNode3 = xmlNode.SelectSingleNode("//ProfileEndpoint/@EndpointId");
            if (xmlNode3 == null || string.IsNullOrEmpty(xmlNode3.Value) || !Utils.IsGUID(xmlNode3.Value))
            {
                throw new InvalidOperationException(string.Format(Resources.FS_ShallowCopyingInvalid,
                    Resources.StoragePointNotConfigured));
            }

            Guid b = new Guid(xmlNode2.Value);
            Guid b2 = new Guid(xmlNode3.Value);
            Guid a;
            Guid a2;
            try
            {
                byte[] array = new byte[16];
                Array.Copy(content, 0, array, 0, 16);
                a = new Guid(array);
                byte[] array2 = new byte[16];
                Array.Copy(content, 33, array2, 0, 16);
                a2 = new Guid(array2);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new InvalidOperationException(string.Format(Resources.FS_ShallowCopyingInvalid,
                    Resources.StoragePointCannotValidateBlobRef, ex));
            }

            byte[] array3 = new byte[content.Length];
            if (a != b)
            {
                byte[] sourceArray = b.ToByteArray();
                Array.Copy(sourceArray, 0, array3, 0, 16);
                Array.Copy(content, 16, array3, 16, 17);
            }
            else
            {
                Array.Copy(content, 0, array3, 0, 33);
            }

            if (a2 != b2)
            {
                byte[] sourceArray2 = b2.ToByteArray();
                Array.Copy(sourceArray2, 0, array3, 33, 16);
                Array.Copy(content, 49, array3, 49, content.Length - 49);
            }
            else
            {
                Array.Copy(content, 33, array3, 33, content.Length - 33);
            }

            return array3;
        }

        private void SetIDToCreateItemAt(SPListItem item, int iID)
        {
            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                this.IncrementItemIDs(item.ParentList, iID);
            }

            this._preventOptimizationVariable = item.Attachments.Count;
            MethodInfo method = item.GetType()
                .GetMethod("SetIDForMigration", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(item, new object[]
            {
                iID
            });
        }

        private void IncrementItemIDs(SPList list, int iTargetID)
        {
            try
            {
                SPListItemCollection emptyListItemCollection = this.GetEmptyListItemCollection(list, false);
                SPListItem sPListItem = emptyListItemCollection.Add();
                sPListItem["Title"] = string.Format("TEMP_{0}", Guid.NewGuid().ToString("N"));
                foreach (SPField sPField in ((IEnumerable)list.Fields))
                {
                    if (Utils.IsWritableColumn(sPField.InternalName, sPField.ReadOnlyField, sPField.TypeAsString,
                            (int)list.BaseTemplate, false, sPField.SchemaXml.Contains(" BdcField=\""), false))
                    {
                        if (sPField.TypeAsString == "ChannelAliasFieldType")
                        {
                            sPListItem[sPField.InternalName] = this.CastStringToFieldType("Default", sPField,
                                list.ParentWeb.RegionalSettings.TimeZone);
                        }
                        else
                        {
                            sPListItem[sPField.InternalName] = this.CastStringToFieldType("", sPField,
                                list.ParentWeb.RegionalSettings.TimeZone);
                        }
                    }
                }

                sPListItem.Update();
                int iD = sPListItem.ID;
                sPListItem.Delete();
                if (iD < iTargetID)
                {
                    string text = list.ID.ToString();
                    int i = iD;
                    while (i < iTargetID)
                    {
                        int num = i;
                        i = ((i + 500 < iTargetID) ? (i + 500) : iTargetID);
                        StringBuilder stringBuilder = new StringBuilder(1000);
                        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        stringBuilder.Append("<ows:Batch OnError=\"Return\">");
                        for (int j = num; j < i; j++)
                        {
                            stringBuilder.Append(string.Concat(new object[]
                            {
                                "<Method ID=\"",
                                j,
                                "\"><SetList>",
                                text,
                                "</SetList>"
                            }));
                            stringBuilder.Append("<SetVar Name=\"ID\">New</SetVar>");
                            stringBuilder.Append("<SetVar Name=\"Cmd\">Save</SetVar>");
                            stringBuilder.Append("<SetVar Name=\"urn:schemas-microsoft-com:office:office#Title\">");
                            stringBuilder.Append(string.Format("TEMP_{0}", Guid.NewGuid().ToString("N")));
                            stringBuilder.Append("</SetVar>");
                            foreach (SPField sPField2 in ((IEnumerable)list.Fields))
                            {
                                if (Utils.IsWritableColumn(sPField2.InternalName, sPField2.ReadOnlyField,
                                        sPField2.TypeAsString, (int)list.BaseTemplate, false,
                                        sPField2.SchemaXml.Contains(" BdcField=\""), false))
                                {
                                    stringBuilder.Append("<Field Name='" + sPField2.InternalName + "'></Field>");
                                }
                            }

                            stringBuilder.Append("</Method>");
                        }

                        stringBuilder.Append("</ows:Batch>");
                        XmlNode xmlNode =
                            XmlUtility.StringToXmlNode(list.ParentWeb.ProcessBatchData(stringBuilder.ToString()));
                        this.CheckBatchProcessError(xmlNode);
                        XmlNodeList xmlNodeList = xmlNode.SelectNodes(".//Result/ID");
                        List<string> list2 = new List<string>(xmlNodeList.Count);
                        foreach (XmlNode xmlNode2 in xmlNodeList)
                        {
                            list2.Add(xmlNode2.InnerText);
                        }

                        this.DeleteItemsByIDs(list, list2.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("Failed to increment item IDs: " + ex.Message);
            }
        }

        private void CheckBatchProcessError(XmlNode result)
        {
            XmlNode xmlNode = result.SelectSingleNode(".//Result[@Code!='0']");
            if (xmlNode == null)
            {
                return;
            }

            string value = xmlNode.Attributes["Code"].Value;
            string text = "Batch Failure. Code: " + value;
            XmlNode xmlNode2 = xmlNode.SelectSingleNode(".//ErrorText");
            if (xmlNode2 != null)
            {
                text = text + " Error: " + xmlNode2.InnerText;
            }

            throw new Exception(text);
        }

        private void UpdateItemMetadata(SPList spList, SPListItem spListItem, XmlNode itemXML)
        {
            this.UpdateItemMetadata(spList, spListItem, itemXML, false);
        }

        private void UpdateItemMetadata(SPList spList, SPListItem spListItem, XmlNode itemXML, bool bConvertToFolder)
        {
            this.UpdateItemMetadata(spList, spListItem, itemXML, bConvertToFolder, false);
        }

        private void UpdateItemMetadata(SPList spList, SPListItem spListItem, XmlNode itemXML, bool bConvertToFolder,
            bool bPreserveSharePointDocumentID)
        {
            SPTimeZone timeZone = spList.ParentWeb.RegionalSettings.TimeZone;
            if ((spListItem.FileSystemObjectType == SPFileSystemObjectType.Folder && itemXML
                    .GetAttributeValueAsString("ProgId")
                    .Equals("OneNote.Notebook", StringComparison.OrdinalIgnoreCase)) || itemXML
                    .GetAttributeValueAsString("HTML_x0020_File_x0020_Type")
                    .Equals("OneNote.Notebook", StringComparison.OrdinalIgnoreCase))
            {
                spListItem["HTML_x0020_File_x0020_Type"] = "OneNote.Notebook";
            }
            else if (spListItem.FileSystemObjectType == SPFileSystemObjectType.Folder && string.Equals(
                         itemXML.GetAttributeValueAsString("ProgId"), "SharePoint.DocumentSet",
                         StringComparison.OrdinalIgnoreCase))
            {
                spListItem["HTML_x0020_File_x0020_Type"] = "SharePoint.DocumentSet";
            }
            else if (spListItem.FileSystemObjectType == SPFileSystemObjectType.Folder && string.Equals(
                         itemXML.GetAttributeValueAsString("ProgId"), "SharePoint.VideoSet",
                         StringComparison.OrdinalIgnoreCase))
            {
                spListItem["HTML_x0020_File_x0020_Type"] = "SharePoint.VideoSet";
            }

            string b;
            string b2;
            this.GetAllDayEventFields(spList, itemXML, out b, out b2);
            foreach (SPField sPField in ((IEnumerable)spList.Fields))
            {
                try
                {
                    bool flag = bConvertToFolder &&
                                sPField.InternalName.Equals("FSObjType", StringComparison.OrdinalIgnoreCase) &&
                                spList.BaseTemplate == (SPListTemplateType)2100;
                    if (flag)
                    {
                        try
                        {
                            spListItem[sPField.InternalName] = SPFileSystemObjectType.Folder;
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            throw new Exception(string.Concat(new string[]
                            {
                                "The folder '",
                                spListItem.Name,
                                "' could not be created properly in slide library '",
                                spList.Title,
                                "'. Message: ",
                                ex.Message
                            }), ex);
                        }
                    }
                    else if (sPField.InternalName == "ContentTypeId" &&
                             spListItem.FileSystemObjectType != SPFileSystemObjectType.Folder &&
                             spList.BaseTemplate != SPListTemplateType.DiscussionBoard)
                    {
                        this.SetContentTypeID(spList, spListItem, sPField, itemXML);
                    }
                    else if (Utils.IsWritableColumn(sPField.InternalName, sPField.ReadOnlyField, sPField.TypeAsString,
                                 (int)spList.BaseTemplate,
                                 spListItem.FileSystemObjectType == SPFileSystemObjectType.Folder,
                                 sPField.SchemaXml.Contains(" BdcField=\""), false))
                    {
                        XmlNode attribute = XmlUtility.GetAttribute(itemXML, null, sPField.InternalName, false);
                        string defaultValue = sPField.DefaultValue;
                        if (attribute != null)
                        {
                            try
                            {
                                if (sPField.InternalName == b ||
                                    (sPField.InternalName == b2 && sPField.TypeAsString == "DateTime"))
                                {
                                    if (string.IsNullOrEmpty(attribute.Value))
                                    {
                                        spListItem[sPField.InternalName] = null;
                                    }
                                    else
                                    {
                                        DateTime dateTime = Utils.ParseDateAsUtc(attribute.Value);
                                        spListItem[sPField.InternalName] = dateTime;
                                    }
                                }
                                else
                                {
                                    spListItem[sPField.InternalName] =
                                        this.CastStringToFieldType(attribute.Value, sPField, timeZone);
                                }

                                continue;
                            }
                            catch (Exception ex2)
                            {
                                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                throw new Exception("Source Value: " + attribute.Value + ", Exception: " + ex2.Message);
                            }
                        }

                        if ((sPField.Required || !string.IsNullOrEmpty(defaultValue)) &&
                            spListItem[sPField.InternalName] == null &&
                            spListItem.FileSystemObjectType != SPFileSystemObjectType.Folder)
                        {
                            spListItem[sPField.InternalName] = (string.IsNullOrEmpty(defaultValue)
                                ? this.CastStringToFieldType("", sPField, timeZone)
                                : sPField.DefaultValueTyped);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(string.Concat(new string[]
                    {
                        "Error setting field data: Target Field Name: ",
                        sPField.InternalName,
                        ", Target Type: ",
                        sPField.TypeAsString,
                        ", ",
                        ex3.Message
                    }));
                }
            }
        }

        private void GetAllDayEventFields(SPList list, XmlNode itemXml, out string sStartDate, out string sEndDate)
        {
            sStartDate = null;
            sEndDate = null;
            SPFieldAllDayEvent sPFieldAllDayEvent = null;
            Type typeFromHandle = typeof(SPFieldAllDayEvent);
            foreach (SPField sPField in ((IEnumerable)list.Fields))
            {
                if (typeFromHandle.IsAssignableFrom(sPField.GetType()))
                {
                    sPFieldAllDayEvent = (sPField as SPFieldAllDayEvent);
                    break;
                }
            }

            if (sPFieldAllDayEvent == null)
            {
                return;
            }

            XmlAttribute xmlAttribute = itemXml.Attributes[sPFieldAllDayEvent.InternalName];
            if (xmlAttribute == null)
            {
                return;
            }

            bool flag;
            if (bool.TryParse(xmlAttribute.Value, out flag))
            {
                if (!flag)
                {
                    return;
                }
            }
            else if (xmlAttribute.Value != "1")
            {
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sPFieldAllDayEvent.SchemaXml);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//FieldRefs/FieldRef[@RefType=\"StartDate\"]");
            if (xmlNode != null && xmlNode.Attributes["Name"] != null)
            {
                sStartDate = xmlNode.Attributes["Name"].Value;
            }

            XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//FieldRefs/FieldRef[@RefType=\"EndDate\"]");
            if (xmlNode2 != null && xmlNode2.Attributes["Name"] != null)
            {
                sEndDate = xmlNode2.Attributes["Name"].Value;
            }
        }

        private void SetContentTypeID(SPList spList, SPListItem spListItem, SPField field, XmlNode itemXML)
        {
            XmlNode attribute = XmlUtility.GetAttribute(itemXML, null, "ContentType", false);
            object obj = null;
            if (attribute != null)
            {
                string value = attribute.Value;
                if (spList.BaseTemplate == SPListTemplateType.Tasks &&
                    (value == "Workflow Task" || value == "Office SharePoint Server Workflow Task"))
                {
                    spListItem[field.InternalName] = "";
                    return;
                }

                foreach (SPContentType sPContentType in ((IEnumerable)spList.ContentTypes))
                {
                    if (sPContentType.Name == value)
                    {
                        obj = sPContentType.Id;
                    }
                }
            }

            if (obj == null)
            {
                XmlNode attribute2 = XmlUtility.GetAttribute(itemXML, null, field.InternalName, false);
                if (attribute2 != null)
                {
                    obj = this.CastStringToFieldType(attribute2.Value, field, null);
                }
            }

            try
            {
                spListItem[field.InternalName] = obj;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("Source Value: " + obj.ToString() + ", Exception: " + ex.Message);
            }
        }

        private void UpdateItemModerationStatus(SPListItem item, XmlNode itemXML)
        {
            if (itemXML.Attributes["_ModerationStatus"] == null ||
                string.IsNullOrEmpty(itemXML.Attributes["_ModerationStatus"].Value))
            {
                return;
            }

            string value = itemXML.Attributes["_ModerationStatus"].Value;
            string comment = (itemXML.Attributes["_ModerationComments"] != null)
                ? itemXML.Attributes["_ModerationComments"].Value
                : null;
            SPModerationStatusType sPModerationStatusType =
                (SPModerationStatusType)Enum.Parse(typeof(SPModerationStatusType), value);
            if (item.ModerationInformation.Status != sPModerationStatusType)
            {
                item.ModerationInformation.Status = sPModerationStatusType;
            }

            item.ModerationInformation.Comment = comment;
        }

        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sItemXML);
                XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sListXML);
                SPUser modifiedByUser = (xmlNode.Attributes["Editor"] == null)
                    ? null
                    : this.LookupUser(xmlNode.Attributes["Editor"].Value, web);
                SPListItem listItem = web.GetListItem("/" + xmlNode.Attributes["FileRef"].Value);
                SPFile file = listItem.File;
                if (bCheckin && !bPublish && !bApprove && file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                {
                    if (xmlNode2.Attributes["EnableVersioning"].Value == "True" &&
                        xmlNode2.Attributes["EnableMinorVersions"].Value == "True")
                    {
                        this.CheckInFileByUser(file, sCheckinComment, SPCheckinType.MinorCheckIn, modifiedByUser);
                    }
                    else
                    {
                        this.CheckInFileByUser(file, sCheckinComment, SPCheckinType.MajorCheckIn, modifiedByUser);
                    }
                }

                if (bPublish)
                {
                    if (file.CheckOutStatus == SPFile.SPCheckOutStatus.None)
                    {
                        this.CheckOutFile(file);
                    }

                    if (file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        if (xmlNode2.Attributes["EnableVersioning"].Value == "True" &&
                            xmlNode2.Attributes["EnableMinorVersions"].Value == "True")
                        {
                            this.CheckInFileByUser(file, sCheckinComment, SPCheckinType.MinorCheckIn, modifiedByUser);
                            file.Publish(sPublishComment);
                        }
                        else
                        {
                            this.CheckInFileByUser(file, sPublishComment, SPCheckinType.MajorCheckIn, modifiedByUser);
                        }
                    }
                }

                if (bApprove)
                {
                    if (file.CheckOutStatus == SPFile.SPCheckOutStatus.None)
                    {
                        this.CheckOutFile(file);
                    }

                    if (file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        this.CheckInFileByUser(file, sCheckinComment, SPCheckinType.MinorCheckIn, modifiedByUser);
                    }

                    file.Item.ModerationInformation.Status = SPModerationStatusType.Approved;
                    file.Item.ModerationInformation.Comment = sApprovalComment;
                    file.Item.Update();
                }
            }

            return string.Empty;
        }

        private object CastStringToFieldType(string sValue, SPField field, SPTimeZone timeZone)
        {
            return this.CastStringToFieldType(sValue, field.TypeAsString, field.InternalName, field.SchemaXml,
                field.ParentList, timeZone);
        }

        private object CastStringToFieldType(string sValue, string fieldType, string fieldName, string fieldSchemaXml,
            SPList list, SPTimeZone timeZone)
        {
            if (string.Equals("WikiField", fieldName))
            {
                return this.ConvertWikiFieldValue(sValue, list);
            }

            switch (fieldType)
            {
                case "DateTime":
                case "PublishingScheduleStartDateFieldType":
                case "PublishingScheduleEndDateFieldType":
                    if (string.IsNullOrEmpty(sValue))
                    {
                        return null;
                    }

                    return OMAdapter.ParseDateString(sValue, timeZone);
                case "User":
                {
                    sValue = Utils.RetriveFirstValuefromColleciton(sValue, ",");
                    object obj = this.LookupUser(sValue, list.ParentWeb);
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(fieldSchemaXml);
                    if (obj == null && xmlNode.Attributes["UserSelectionMode"] != null && xmlNode
                            .Attributes["UserSelectionMode"].Value
                            .Equals(SPFieldUserSelectionMode.PeopleAndGroups.ToString()))
                    {
                        obj = OMAdapter.LookupGroup(sValue, list.ParentWeb);
                    }

                    return obj;
                }
                case "UserMulti":
                {
                    string[] array = sValue.Split(new char[]
                    {
                        ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    string text = null;
                    string[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string text2 = array2[i];
                        string text3 = null;
                        SPUser sPUser = this.LookupUser(text2, list.ParentWeb);
                        if (sPUser != null)
                        {
                            text3 = sPUser.ID + ";#" + sPUser.LoginName;
                        }
                        else
                        {
                            XmlNode xmlNode2 = XmlUtility.StringToXmlNode(fieldSchemaXml);
                            if (xmlNode2.Attributes["UserSelectionMode"] != null && xmlNode2
                                    .Attributes["UserSelectionMode"].Value
                                    .Equals(SPFieldUserSelectionMode.PeopleAndGroups.ToString()))
                            {
                                SPGroup sPGroup = OMAdapter.LookupGroup(text2, list.ParentWeb);
                                if (sPGroup != null)
                                {
                                    text3 = sPGroup.ID + ";#" + sPGroup.Name;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(text3))
                        {
                            if (string.IsNullOrEmpty(text))
                            {
                                text = text3;
                            }
                            else
                            {
                                text = text + ";#" + text3;
                            }
                        }
                    }

                    return text;
                }
                case "LookupMulti":
                {
                    string[] array3 = sValue.Split(new string[]
                    {
                        ";#"
                    }, StringSplitOptions.RemoveEmptyEntries);
                    string text4 = "";
                    for (int j = 0; j < array3.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(text4))
                        {
                            text4 += ";#";
                        }

                        text4 = text4 + array3[j] + ";#";
                    }

                    return text4;
                }
                case "Choice":
                    return Utils.RetriveFirstValuefromColleciton(sValue, ";#");
            }

            return sValue;
        }

        private string ConvertWikiFieldValue(string sValue, SPList list)
        {
            Type type = list.GetType();
            Type type2 = type.Assembly.GetType("Microsoft.SharePoint.Utilities.SPWikiUtility");
            MethodInfo method =
                type2.GetMethod("ConvertHtmlLinkToWikiLink", BindingFlags.Static | BindingFlags.NonPublic);
            PropertyInfo property = type.GetProperty("RootFolderUrl", BindingFlags.Instance | BindingFlags.NonPublic);
            string text = (string)property.GetValue(list, null);
            string str = UrlUtils.StandardizeFormat(text);
            sValue = sValue.Replace(" href=\"" + str, " href=\"" + text);
            if (base.SharePointVersion.IsSharePoint2007)
            {
                return (string)method.Invoke(null, new object[]
                {
                    sValue,
                    text
                });
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                bool success = Regex.Match(sValue,
                        "<a id=\"[0-9]*:[^\"]*:[^\"]*\" class=\"(ms-wikilink|ms-missinglink)\" href=\"[^\"]*\">[^<]*</a>")
                    .Success;
                return (string)method.Invoke(null, new object[]
                {
                    sValue,
                    list,
                    text,
                    !success
                });
            }

            return sValue;
        }

        protected static DateTime ParseDateString(string sDate, SPTimeZone timeZone)
        {
            DateTime result = Utils.ParseDate(sDate);
            if (timeZone != null)
            {
                result = timeZone.UTCToLocalTime(result.ToUniversalTime());
            }

            return result;
        }

        public static SPListItem GetItemByID(SPList list, int iID, bool bFullFields, bool bDatesInUtc,
            uint rowLimitOverride = 0u)
        {
            SPListItem result;
            try
            {
                SPQuery listQuery = OMAdapter.GetListQuery();
                listQuery.Query = "<Where><Eq><FieldRef Name=\"ID\"></FieldRef><Value Type=\"Integer\">" +
                                  iID.ToString() + "</Value></Eq></Where>";
                listQuery.ItemIdQuery = true;
                if (!bFullFields)
                {
                    listQuery.ViewFields = "<FieldRef Name=\"ID\"></FieldRef>";
                }

                listQuery.RowLimit = 2000u;
                if (rowLimitOverride > 0u)
                {
                    listQuery.RowLimit = rowLimitOverride;
                }
                else
                {
                    listQuery.RowLimit = OMAdapter.s_iGetListQueryRowLimit.Value;
                }

                listQuery.DatesInUtc = bDatesInUtc;
                SPListItemCollection items = list.GetItems(listQuery);
                if (items.Count == 1)
                {
                    result = items[0];
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        public static SPRecycleBinItem GetItemFromRecycleBin(SPList list, Guid itemGuid)
        {
            SPRecycleBinQuery sPRecycleBinQuery = new SPRecycleBinQuery();
            SPRecycleBinItem result;
            try
            {
                sPRecycleBinQuery.ItemState = SPRecycleBinItemState.FirstStageRecycleBin;
                result = list.ParentWeb.GetRecycleBinItems(sPRecycleBinQuery).GetItemById(itemGuid);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                try
                {
                    sPRecycleBinQuery.ItemState = SPRecycleBinItemState.SecondStageRecycleBin;
                    result = list.ParentWeb.Site.GetRecycleBinItems(sPRecycleBinQuery).GetItemById(itemGuid);
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    result = null;
                }
            }

            return result;
        }

        private SPUser LookupUser(string sLoginName, SPWeb currentWeb)
        {
            SPUser result;
            if (base.SharePointVersion.IsSharePoint2007OrEarlier)
            {
                try
                {
                    result = currentWeb.SiteUsers[sLoginName];
                    return result;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    result = null;
                    return result;
                }
            }

            string text = Utils.ConvertWinOrFormsUserToClaimString(sLoginName).ToLowerInvariant();
            string text2 = Utils.ConvertClaimStringUserToWinOrFormsUser(sLoginName).ToLowerInvariant();
            try
            {
                result = currentWeb.SiteUsers[this.ClaimsAuthenticationInUse ? text : text2];
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                if (text2 == text)
                {
                    result = null;
                }
                else
                {
                    try
                    {
                        result = currentWeb.SiteUsers[this.ClaimsAuthenticationInUse ? text2 : text];
                    }
                    catch (Exception ex3)
                    {
                        OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                        result = null;
                    }
                }
            }

            return result;
        }

        private static SPGroup LookupGroup(string sGroupName, SPWeb currentWeb)
        {
            SPGroup result;
            try
            {
                result = currentWeb.SiteGroups[sGroupName];
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private Exception TransformItemUpdateException(Exception ex)
        {
            if (ex is SPException && ex.Message.Contains("0x80040E2F"))
            {
                return new Exception(ex.Message +
                                     " This may be caused by an item in the recycle bin occupying the specified ID.");
            }

            return ex;
        }

        private void CheckInFileByUser(SPFile file, string sCheckinComments, SPCheckinType checkinType,
            SPUser modifiedBy, bool bAdding, string sVersionString)
        {
            if (!file.Item.ParentList.EnableVersioning)
            {
                this.CheckInFileByUser(file, sCheckinComments, checkinType, modifiedBy);
                return;
            }

            if (!bAdding)
            {
                this.CheckInFileByUser(file, sCheckinComments, checkinType, modifiedBy);
                return;
            }

            SPFileVersion sPFileVersion = file.Versions[0];
            string text = new OMAdapter.VersionString(sVersionString).ToString();
            string text2 = new OMAdapter.VersionString(sPFileVersion.VersionLabel).ToString();
            if (text == text2 || (!file.Item.ParentList.EnableMinorVersions &&
                                  text.StartsWith("0.", StringComparison.Ordinal) &&
                                  text2.Equals("1.0", StringComparison.Ordinal)))
            {
                this.CheckInFileByUser(file, sCheckinComments, SPCheckinType.OverwriteCheckIn, modifiedBy);
                return;
            }

            this.CheckInFileByUser(file, sCheckinComments, checkinType, modifiedBy);
            if (file.Versions.Count > 0)
            {
                file.Versions.DeleteByID(sPFileVersion.ID);
                return;
            }

            SPListItemVersion versionFromLabel = file.Item.Versions.GetVersionFromLabel(text2);
            if (versionFromLabel != null)
            {
                versionFromLabel.Delete();
            }
        }

        private void CheckInFileByUser(SPFile file, string checkinComment, SPCheckinType checkinType,
            SPUser modifiedByUser)
        {
            using (new OMAdapterHttpContext(file.ParentFolder.ParentWeb))
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        if (base.SharePointVersion.IsSharePoint2016OrLater)
                        {
                            this.CheckInFileByUser2016(file, checkinComment, checkinType, modifiedByUser);
                            break;
                        }

                        this.CheckInFileByUser2013OrEarlier(file, checkinComment, checkinType, modifiedByUser);
                        break;
                    }
                    catch (TargetInvocationException ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        if (!(ex.InnerException is SPException))
                        {
                            throw ex.InnerException;
                        }

                        int errorCode = ((SPException)ex.InnerException).ErrorCode;
                        if ((errorCode != -2147467259 && errorCode != -2130246376) || i >= 4)
                        {
                            throw ex.InnerException;
                        }

                        Thread.Sleep(50);
                    }
                }
            }
        }

        private void CheckInFileByUser2013OrEarlier(SPFile file, string checkinComment, SPCheckinType checkinType,
            SPUser modifiedByUser)
        {
            MethodInfo method = typeof(SPFile).GetMethod("CheckIn", BindingFlags.Instance | BindingFlags.NonPublic,
                null, new Type[]
                {
                    typeof(string),
                    typeof(SPCheckinType),
                    typeof(bool),
                    typeof(SPUser)
                }, null);
            method.Invoke(file, new object[]
            {
                checkinComment,
                checkinType,
                false,
                modifiedByUser
            });
        }

        private void CheckInFileByUser2016(SPFile file, string checkinComment, SPCheckinType checkinType,
            SPUser modifiedByUser)
        {
            MethodInfo method = typeof(SPFile).GetMethod("CheckIn", BindingFlags.Instance | BindingFlags.NonPublic,
                null, new Type[]
                {
                    typeof(string),
                    typeof(SPCheckinType),
                    typeof(bool),
                    typeof(int)
                }, null);
            method.Invoke(file, new object[]
            {
                checkinComment,
                checkinType,
                false,
                (modifiedByUser != null) ? modifiedByUser.ID : 0
            });
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList list = web.Lists[new Guid(sListID)];
                bool flag;
                SPListItemCollection[] fullListItemCollectionsNoMetaData =
                    this.GetFullListItemCollectionsNoMetaData(list, out flag);
                Exception ex = null;
                SPListItemCollection[] array = fullListItemCollectionsNoMetaData;
                for (int i = 0; i < array.Length; i++)
                {
                    SPListItemCollection sPListItemCollection = array[i];
                    try
                    {
                        sPListItemCollection.DeleteItemById(iListItemID);
                        ex = null;
                        break;
                    }
                    catch (ArgumentException ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        ex = ex2;
                    }
                }

                if (ex != null)
                {
                    throw ex;
                }
            }

            return string.Empty;
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            if (string.IsNullOrEmpty(sListID) || (!bDeleteAllItems && string.IsNullOrEmpty(sIDs)))
            {
                return string.Empty;
            }

            try
            {
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    SPList list = web.Lists[new Guid(sListID)];
                    if (bDeleteAllItems)
                    {
                        bool flag;
                        SPListItemCollection[] fullListItemCollectionsNoMetaData =
                            this.GetFullListItemCollectionsNoMetaData(list, out flag);
                        SPListItemCollection[] array = fullListItemCollectionsNoMetaData;
                        for (int i = 0; i < array.Length; i++)
                        {
                            SPListItemCollection items = array[i];
                            this.DeleteAllListItems(items);
                        }
                    }
                    else if (!string.IsNullOrEmpty(sIDs))
                    {
                        this.DeleteItemsByIDs(list, sIDs.Split(new char[]
                        {
                            ','
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return string.Empty;
        }

        protected void DeleteItemsByIDs(SPList list, string[] sIDsToDelete)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            for (int i = 0; i < sIDsToDelete.Length; i++)
            {
                string text = sIDsToDelete[i];
                stringBuilder.Append((stringBuilder.Length > 0) ? "," : "");
                stringBuilder.Append(text.ToString());
                num++;
                if (num >= 50)
                {
                    bool flag;
                    SPListItemCollection[] listItemCollections = this.GetListItemCollections(list,
                        stringBuilder.ToString(), null, true, ListItemQueryType.ListItem | ListItemQueryType.Folder,
                        "<FieldRef Name=\"ID\"></FieldRef>", out flag);
                    if (listItemCollections != null && listItemCollections.Length > 0)
                    {
                        SPListItemCollection[] array = listItemCollections;
                        for (int j = 0; j < array.Length; j++)
                        {
                            SPListItemCollection items = array[j];
                            this.DeleteAllListItems(items);
                        }
                    }

                    stringBuilder = new StringBuilder();
                    num = 0;
                }
            }

            if (stringBuilder.Length > 0)
            {
                bool flag2;
                SPListItemCollection[] listItemCollections = this.GetListItemCollections(list, stringBuilder.ToString(),
                    null, true, ListItemQueryType.ListItem | ListItemQueryType.Folder,
                    "<FieldRef Name=\"ID\"></FieldRef>", out flag2);
                if (listItemCollections != null && listItemCollections.Length > 0)
                {
                    SPListItemCollection[] array2 = listItemCollections;
                    for (int k = 0; k < array2.Length; k++)
                    {
                        SPListItemCollection items2 = array2[k];
                        this.DeleteAllListItems(items2);
                    }
                }
            }
        }

        public SPListItemCollection GetEmptyListItemCollection(SPList list, bool bDatesInUtc)
        {
            SPQuery listQuery = OMAdapter.GetListQuery();
            listQuery.Query = "<Where><Lt><FieldRef Name=\"ID\" /><Value Type=\"Counter\">0</Value></Lt></Where>";
            listQuery.RowLimit = 1u;
            listQuery.DatesInUtc = bDatesInUtc;
            return list.GetItems(listQuery);
        }

        private SPListItemCollection[] GetFullListItemCollectionsNoMetaData(SPList list, out bool bSortOrderViolated)
        {
            string query = Utils.BuildQuery(null, null, false, false,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, null);
            string viewFields = "<FieldRef Name=\"ID\"></FieldRef>" + ((list.BaseType == SPBaseType.DocumentLibrary)
                ? "<FieldRef Name=\"FileRef\"></FieldRef>"
                : "");
            return this.GetListItems(list, query, viewFields, null, null, null, null, null, out bSortOrderViolated);
        }

        private SPListItemCollection[] GetFullListItemCollections(SPList list, out bool bSortOrderViolated)
        {
            string query = Utils.BuildQuery(null, null, false, false,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, null);
            return this.GetListItems(list, query, null, null, null, null, null, null, out bSortOrderViolated);
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPFile sPFile = null;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(fileXml);
                XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes["FileLeafRef"];
                if (xmlAttribute == null)
                {
                    throw new ArgumentException("FileLeafRef cannot be missing");
                }

                string value = xmlAttribute.Value;
                SPList list = context.Web.Lists.GetList(listId, false);
                string fullPath = this.GetFullPath(list, folderPath.TrimEnd(new char[]
                {
                    '/',
                    '\\',
                    ' '
                }) + "/" + value);
                int num;
                string text;
                SPUser sPUser;
                SPUser sPUser2;
                DateTime dateTime;
                DateTime dateTime2;
                this.GetListItemFileInfo(xmlDocument.DocumentElement, out num, out text, out sPUser, out sPUser2,
                    out dateTime, out dateTime2, options.CorrectInvalidNames, context.Web);
                if (fileContents.LongLength > 5242880L)
                {
                    using (MemoryStream memoryStream = new MemoryStream(fileContents))
                    {
                        sPFile = list.RootFolder.Files.Add(fullPath, memoryStream, options.Overwrite);
                        goto IL_117;
                    }
                }

                sPFile = list.RootFolder.Files.Add(fullPath, fileContents, options.Overwrite);
                IL_117:
                sPFile.Item["Author"] = (sPUser ?? context.Web.CurrentUser);
                sPFile.Item["Editor"] = (sPUser2 ?? context.Web.CurrentUser);
                sPFile.Item["Created"] = dateTime;
                sPFile.Item["Modified"] = dateTime2;
                sPFile.Item.UpdateOverwriteVersion();
                if (fieldsLookupCache == null)
                {
                    FieldsLookUp fieldsLookUp = new FieldsLookUp();
                    foreach (SPField sPField in ((IEnumerable)list.Fields))
                    {
                        if (!fieldsLookUp.ContainsKey(sPField.InternalName))
                        {
                            fieldsLookUp.Add(sPField.InternalName, sPField.SchemaXml);
                        }
                    }

                    fieldsLookupCache = fieldsLookUp;
                }

                bool flag = fieldsLookupCache.ContainsKey("_ModerationStatus") &&
                            OMAdapter.IsTargetModerationStatusRequired(fieldsLookupCache["_ModerationStatus"]);
                foreach (XmlAttribute xmlAttribute2 in xmlDocument.DocumentElement.Attributes)
                {
                    if (!string.Equals("Id", xmlAttribute2.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        !string.Equals("_ModerationStatus", xmlAttribute2.Name,
                            StringComparison.InvariantCultureIgnoreCase) &&
                        fieldsLookupCache.ContainsKey(xmlAttribute2.Name))
                    {
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml(fieldsLookupCache[xmlAttribute2.Name]);
                        XmlNode documentElement = xmlDocument2.DocumentElement;
                        XmlAttribute xmlAttribute3 = documentElement.Attributes["Type"];
                        if ((xmlAttribute3.Value == "TaxonomyFieldType" ||
                             xmlAttribute3.Value == "TaxonomyFieldTypeMulti") && Utils.HasTaxonomySupport())
                        {
                            OMAdapter.WriteValueAsManagedMetadata(
                                sPFile.Item.Fields.GetFieldByInternalName(xmlAttribute2.Name), xmlAttribute2.Value,
                                sPFile.Item);
                        }
                        else if (xmlAttribute3.Value == "BusinessData" && OMAdapter.SupportsBcs())
                        {
                            this.WriteValueAsBcsData(sPFile.Item.Fields.GetFieldByInternalName(xmlAttribute2.Name),
                                xmlAttribute2.Value, sPFile.Item);
                        }
                        else
                        {
                            sPFile.Item[xmlAttribute2.Name] = this.CastStringToFieldType(xmlAttribute2.Value,
                                xmlAttribute3.Value, xmlAttribute2.Name, xmlDocument2.OuterXml, list,
                                context.Web.RegionalSettings.TimeZone);
                        }
                    }
                }

                sPFile.Item.SystemUpdate(false);
                if (flag)
                {
                    sPFile.Item["_ModerationStatus"] = 0;
                    sPFile.Item.SystemUpdate(false);
                }

                StringWriter stringWriter = new StringWriter();
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("ListItems");
                    xmlTextWriter.WriteStartElement("ListItem");
                    xmlTextWriter.WriteAttributeString("ID", sPFile.Item.ID.ToString(CultureInfo.InvariantCulture));
                    xmlTextWriter.WriteAttributeString("FileDirRef",
                        sPFile.Item.Url.Substring(0, sPFile.Item.Url.LastIndexOf('/')));
                    xmlTextWriter.WriteAttributeString("FileLeafRef", sPFile.Item.Name);
                    xmlTextWriter.WriteAttributeString("Modified", Utils.FormatDate(sPFile.TimeLastModified));
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }

                result = stringWriter.ToString();
            }

            return result;
        }

        private static bool IsTargetModerationStatusRequired(string fieldLookupValue)
        {
            bool result = false;
            if (fieldLookupValue != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(fieldLookupValue);
                if (xmlDocument.DocumentElement != null)
                {
                    string text = (xmlDocument.DocumentElement.Attributes["Required"] != null)
                        ? xmlDocument.DocumentElement.Attributes["Required"].Value
                        : "";
                    bool.TryParse(text.ToLower(), out result);
                }
            }

            return result;
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPFolder sPFolder = null;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(folderXml);
                XmlAttribute xmlAttribute = xmlDocument.DocumentElement.Attributes["FileLeafRef"];
                if (xmlAttribute == null)
                {
                    throw new ArgumentException("FileLeafRef cannot be missing");
                }

                string value = xmlAttribute.Value;
                SPList list = context.Web.Lists.GetList(listId, false);
                string fullPath = this.GetFullPath(list, folderPath.TrimEnd(new char[]
                {
                    '/',
                    '\\',
                    ' '
                }) + "/" + value);
                if (options.Overwrite)
                {
                    sPFolder = context.Web.GetFolder(fullPath);
                    if (sPFolder.Exists)
                    {
                        goto IL_24E;
                    }

                    sPFolder.Delete();
                    sPFolder = null;
                }

                string fullPath2 = this.GetFullPath(list, folderPath);
                SPFolder folder = context.Web.GetFolder(fullPath2);
                sPFolder = folder.SubFolders.Add(fullPath);
                if (fieldsLookupCache == null)
                {
                    FieldsLookUp fieldsLookUp = new FieldsLookUp();
                    foreach (SPField sPField in ((IEnumerable)list.Fields))
                    {
                        if (!fieldsLookUp.ContainsKey(sPField.InternalName))
                        {
                            fieldsLookUp.Add(sPField.InternalName, sPField.SchemaXml);
                        }
                    }

                    fieldsLookupCache = fieldsLookUp;
                }

                foreach (XmlAttribute xmlAttribute2 in xmlDocument.DocumentElement.Attributes)
                {
                    if (!string.Equals("Id", xmlAttribute2.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        fieldsLookupCache.ContainsKey(xmlAttribute2.Name))
                    {
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml(fieldsLookupCache[xmlAttribute2.Name]);
                        XmlNode documentElement = xmlDocument2.DocumentElement;
                        XmlAttribute xmlAttribute3 = documentElement.Attributes["Type"];
                        sPFolder.Item[xmlAttribute2.Name] = this.CastStringToFieldType(xmlAttribute2.Value,
                            xmlAttribute3.Value, xmlAttribute2.Name, xmlDocument2.OuterXml, list,
                            context.Web.RegionalSettings.TimeZone);
                    }
                }

                sPFolder.Update();
                IL_24E:
                StringWriter stringWriter = new StringWriter();
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.WriteStartElement("Folder");
                    xmlTextWriter.WriteAttributeString("ID", sPFolder.Item.ID.ToString(CultureInfo.InvariantCulture));
                    xmlTextWriter.WriteAttributeString("FileLeafRef", sPFolder.Item["FileLeafRef"].ToString());
                    xmlTextWriter.WriteAttributeString("FileDirRef", sPFolder.Item["FileDirRef"].ToString());
                    xmlTextWriter.WriteAttributeString("ContentTypeId", sPFolder.Item["ContentTypeId"].ToString());
                    xmlTextWriter.WriteAttributeString("Editor", sPFolder.Item["Editor"].ToString());
                    xmlTextWriter.WriteAttributeString("Author", sPFolder.Item["Author"].ToString());
                    xmlTextWriter.WriteAttributeString("Created", Utils.FormatDate((DateTime)sPFolder.Item["Created"]));
                    xmlTextWriter.WriteAttributeString("Modified",
                        Utils.FormatDate((DateTime)sPFolder.Item["Modified"]));
                    xmlTextWriter.WriteAttributeString("HasUniquePermissions",
                        sPFolder.Item.HasUniqueRoleAssignments.ToString());
                    xmlTextWriter.WriteEndElement();
                }

                result = stringWriter.ToString();
            }

            return result;
        }

        private static bool CanLoadOfficeDocumentManagementDLL()
        {
            Type type = null;
            return type != null;
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            string result;
            using (Context context = this.GetContext())
            {
                bool flag = false;
                SPWeb web = context.Web;
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                string[] array = string.IsNullOrEmpty(sIDs)
                    ? new string[0]
                    : sIDs.Split(new char[]
                    {
                        ','
                    });
                List<string> list = new List<string>(array);
                SPList sPList = web.Lists[new Guid(sListID)];
                string text = web.ServerRelativeUrl.Trim(new char[]
                {
                    '/'
                });
                string strUrl =
                    (text.Length > 0 && sParentFolder.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                        ? sParentFolder.Trim(new char[]
                        {
                            '/'
                        }).Substring(text.Length + 1)
                        : sParentFolder;
                SPFolder folder = web.GetFolder(strUrl);
                if (!flag)
                {
                    SPListItemCollection items = sPList.GetItems(new SPQuery
                    {
                        Folder = folder,
                        Query = string.Format(
                            "<Where><Eq><FieldRef Name='FSObjType'/><Value Type='Lookup'>1</Value></Eq></Where>",
                            new object[0])
                    });
                    xmlTextWriter.WriteStartElement("Folders");
                    foreach (SPListItem sPListItem in ((IEnumerable)items))
                    {
                        if (sPListItem != null && (array.Length == 0 || list.Contains(sPListItem.ID.ToString())))
                        {
                            xmlTextWriter.WriteStartElement("Folder");
                            xmlTextWriter.WriteAttributeString("ContentTypeId",
                                this.GetFieldValueByInternalName(web, sPList, sPListItem, "ContentTypeId", false));
                            xmlTextWriter.WriteAttributeString("ID", sPListItem.ID.ToString());
                            xmlTextWriter.WriteAttributeString("FileLeafRef",
                                this.GetFieldValueByInternalName(web, sPList, sPListItem, "FileLeafRef", false));
                            xmlTextWriter.WriteAttributeString("FileDirRef",
                                this.GetFieldValueByInternalName(web, sPList, sPListItem, "FileDirRef", false));
                            string fieldValueByInternalName =
                                this.GetFieldValueByInternalName(web, sPList, sPListItem, "Editor", false);
                            if (fieldValueByInternalName != null)
                            {
                                xmlTextWriter.WriteAttributeString("Editor", fieldValueByInternalName);
                            }

                            string fieldValueByInternalName2 =
                                this.GetFieldValueByInternalName(web, sPList, sPListItem, "Author", false);
                            if (fieldValueByInternalName2 != null)
                            {
                                xmlTextWriter.WriteAttributeString("Author", fieldValueByInternalName2);
                            }

                            xmlTextWriter.WriteAttributeString("Created",
                                Utils.FormatDate(
                                    web.RegionalSettings.TimeZone.LocalTimeToUTC((DateTime)sPListItem["Created"])));
                            xmlTextWriter.WriteAttributeString("Modified",
                                Utils.FormatDate(
                                    web.RegionalSettings.TimeZone.LocalTimeToUTC((DateTime)sPListItem["Modified"])));
                            xmlTextWriter.WriteAttributeString("HasUniquePermissions",
                                sPListItem.HasUniqueRoleAssignments.ToString());
                            xmlTextWriter.WriteEndElement();
                        }
                    }

                    xmlTextWriter.WriteEndElement();
                    result = stringBuilder.ToString();
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        private string GetFieldValueByInternalName(SPWeb currentWeb, SPList list, SPListItem listItem,
            string sInternalFieldName, bool bDatesInUtc)
        {
            object obj = null;
            SPField sPField = null;
            try
            {
                sPField = list.Fields.GetFieldByInternalName(sInternalFieldName);
                obj = listItem[sPField.Id];
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                obj = null;
            }

            if (obj == null)
            {
                return null;
            }

            return this.GetFieldValue(listItem[sPField.Id], sPField, currentWeb, bDatesInUtc);
        }

        public string GetLists()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                SortedCollection sortedCollection =
                    new SortedCollection(web.Lists, TypeDescriptor.GetProperties(typeof(SPList))["Title"]);
                xmlTextWriter.WriteStartElement("Lists");
                foreach (SPList list in sortedCollection)
                {
                    try
                    {
                        this.GetListXML(list, xmlTextWriter, false);
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        private void GetListXML(SPList list, XmlWriter xmlWriter, bool bFullXML)
        {
            xmlWriter.WriteStartElement("List");
            xmlWriter.WriteAttributeString("ID", list.ID.ToString());
            xmlWriter.WriteAttributeString("Name", list.RootFolder.Name);
            xmlWriter.WriteAttributeString("Title", list.Title);
            xmlWriter.WriteAttributeString("BaseTemplate", Convert.ToInt32(list.BaseTemplate).ToString());
            xmlWriter.WriteAttributeString("BaseType", Convert.ToInt32(list.BaseType).ToString());
            xmlWriter.WriteAttributeString("DirName", this.GetDirName(list, list.RootFolder));
            xmlWriter.WriteAttributeString("ItemCount", list.ItemCount.ToString());
            string value = Utils.FormatDate(Utils.MakeTrueUTCDateTime(list.Created));
            xmlWriter.WriteAttributeString("Created", value);
            string value2 = Utils.FormatDate(Utils.MakeTrueUTCDateTime(list.LastItemModifiedDate));
            xmlWriter.WriteAttributeString("Modified", value2);
            xmlWriter.WriteAttributeString("Hidden", list.Hidden.ToString());
            xmlWriter.WriteAttributeString("FeatureId", list.TemplateFeatureId.ToString());
            if (bFullXML)
            {
                try
                {
                    if (list.Author != null)
                    {
                        xmlWriter.WriteAttributeString("Author", list.Author.ToString());
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }

                xmlWriter.WriteAttributeString("Description", list.Description);
                xmlWriter.WriteAttributeString("HasUniquePermissions", list.HasUniqueRoleAssignments.ToString());
                if (list.BaseType != SPBaseType.DocumentLibrary && list.BaseType != SPBaseType.Survey)
                {
                    xmlWriter.WriteAttributeString("EnableAttachments", list.EnableAttachments.ToString());
                }

                xmlWriter.WriteAttributeString("EnableModeration", list.EnableModeration.ToString());
                xmlWriter.WriteAttributeString("EnableAssignToEmail", list.EnableAssignToEmail.ToString());
                xmlWriter.WriteAttributeString("EnableVersioning", list.EnableVersioning.ToString());
                if (list.EnableVersioning)
                {
                    xmlWriter.WriteAttributeString("EnableMinorVersions", list.EnableMinorVersions.ToString());
                    if (list.MajorVersionLimit >= 0)
                    {
                        xmlWriter.WriteAttributeString("MajorVersionLimit", list.MajorVersionLimit.ToString());
                    }

                    if (list.MajorWithMinorVersionsLimit >= 0 && (list.EnableMinorVersions || list.EnableModeration))
                    {
                        xmlWriter.WriteAttributeString("MajorWithMinorVersionsLimit",
                            list.MajorWithMinorVersionsLimit.ToString());
                    }
                }

                xmlWriter.WriteAttributeString("DraftVersionVisibility", ((int)list.DraftVersionVisibility).ToString());
                bool flag = false;
                if (OMAdapter.AttemptToGetListQuickLaunchValue(list, out flag))
                {
                    xmlWriter.WriteAttributeString("OnQuickLaunch", flag.ToString());
                }

                if (list.BaseType == SPBaseType.DocumentLibrary)
                {
                    xmlWriter.WriteAttributeString("ForceCheckout", list.ForceCheckout.ToString());
                    SPDocumentLibrary sPDocumentLibrary = list as SPDocumentLibrary;
                    xmlWriter.WriteAttributeString("IsCatalog", Convert.ToString(sPDocumentLibrary.IsCatalog));
                    xmlWriter.WriteAttributeString("BrowserEnabledDocuments", list.DefaultItemOpen.ToString());
                    xmlWriter.WriteAttributeString("SendToLocationName", list.SendToLocationName);
                    xmlWriter.WriteAttributeString("SendToLocationUrl", list.SendToLocationUrl);
                    if (list.BaseTemplate == SPListTemplateType.DocumentLibrary ||
                        list.BaseTemplate == SPListTemplateType.XMLForm)
                    {
                        string serverRelativeDocumentTemplateUrl = sPDocumentLibrary.ServerRelativeDocumentTemplateUrl;
                        xmlWriter.WriteAttributeString("DocTemplateUrl",
                            string.IsNullOrEmpty(serverRelativeDocumentTemplateUrl)
                                ? ""
                                : ("/" + serverRelativeDocumentTemplateUrl.TrimStart(new char[]
                                {
                                    '/'
                                })));
                        if (list.BaseTemplate == SPListTemplateType.XMLForm)
                        {
                            xmlWriter.WriteAttributeString("BrowserActivatedTemplate",
                                this.IsFormTemplateBrowserActivated(sPDocumentLibrary).ToString());
                        }
                    }
                }

                xmlWriter.WriteAttributeString("AllowMultiResponses", list.AllowMultiResponses.ToString());
                xmlWriter.WriteAttributeString("ShowUser", list.ShowUser.ToString());
                if (!string.IsNullOrEmpty(list.EmailAlias))
                {
                    xmlWriter.WriteAttributeString("EmailAlias", list.EmailAlias);
                }

                if (list.RootFolder.Properties["vti_emailattachmentfolders"] != null)
                {
                    xmlWriter.WriteAttributeString("EmailAttachmentFolder",
                        list.RootFolder.Properties["vti_emailattachmentfolders"].ToString());
                }

                if (list.RootFolder.Properties["vti_emailoverwrite"] != null)
                {
                    xmlWriter.WriteAttributeString("EmailOverWrite",
                        list.RootFolder.Properties["vti_emailoverwrite"].ToString());
                }

                if (list.RootFolder.Properties["vti_emailsaveoriginal"] != null)
                {
                    xmlWriter.WriteAttributeString("EmailSaveOriginal",
                        list.RootFolder.Properties["vti_emailsaveoriginal"].ToString());
                }

                if (list.RootFolder.Properties["vti_emailsavemeetings"] != null)
                {
                    xmlWriter.WriteAttributeString("EmailSaveMeetings",
                        list.RootFolder.Properties["vti_emailsavemeetings"].ToString());
                }

                if (list.RootFolder.Properties["vti_emailusesecurity"] != null)
                {
                    xmlWriter.WriteAttributeString("EmailUseSecurity",
                        list.RootFolder.Properties["vti_emailusesecurity"].ToString());
                }

                xmlWriter.WriteAttributeString("AllowContentTypes", list.AllowContentTypes.ToString());
                xmlWriter.WriteAttributeString("ContentTypesEnabled", list.ContentTypesEnabled.ToString());
                xmlWriter.WriteAttributeString("Folders", list.EnableFolderCreation.ToString());
                xmlWriter.WriteAttributeString("ReadSecurity", list.ReadSecurity.ToString());
                xmlWriter.WriteAttributeString("WriteSecurity", list.WriteSecurity.ToString());
                xmlWriter.WriteAttributeString("NoCrawl", list.NoCrawl.ToString());
                xmlWriter.WriteAttributeString("MultipleDataList", list.MultipleDataList.ToString());
                xmlWriter.WriteAttributeString("IrmEnabled", list.IrmEnabled.ToString());
                xmlWriter.WriteAttributeString("EnableSyndication", list.EnableSyndication.ToString());
                if (list.RootFolder.Properties["vti_rss_LimitDescriptionLength"] != null)
                {
                    xmlWriter.WriteAttributeString("RssLimitDescriptionLength",
                        list.RootFolder.Properties["vti_rss_LimitDescriptionLength"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_ChannelTitle"] != null)
                {
                    xmlWriter.WriteAttributeString("RssChannelTitle",
                        list.RootFolder.Properties["vti_rss_ChannelTitle"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_ChannelDescription"] != null)
                {
                    xmlWriter.WriteAttributeString("RssChannelDescription",
                        list.RootFolder.Properties["vti_rss_ChannelDescription"].ToString().Replace("\r\n", "\\r\\n"));
                }

                if (list.RootFolder.Properties["vti_rss_ChannelImageUrl"] != null)
                {
                    xmlWriter.WriteAttributeString("RssChannelImageUrl",
                        list.RootFolder.Properties["vti_rss_ChannelImageUrl"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_ItemLimit"] != null)
                {
                    xmlWriter.WriteAttributeString("RssItemLimit",
                        list.RootFolder.Properties["vti_rss_ItemLimit"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_DayLimit"] != null)
                {
                    xmlWriter.WriteAttributeString("RssDayLimit",
                        list.RootFolder.Properties["vti_rss_DayLimit"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_DocumentAsEnclosure"] != null)
                {
                    xmlWriter.WriteAttributeString("RssDocumentAsEnclosure",
                        list.RootFolder.Properties["vti_rss_DocumentAsEnclosure"].ToString());
                }

                if (list.RootFolder.Properties["vti_rss_DocumentAsLink"] != null)
                {
                    xmlWriter.WriteAttributeString("RssDocumentAsLink",
                        list.RootFolder.Properties["vti_rss_DocumentAsLink"].ToString());
                }

                if (list.RootFolder.Properties["TimelineDefaultView"] != null)
                {
                    xmlWriter.WriteAttributeString("TimelineDefaultValue",
                        list.RootFolder.Properties["TimelineDefaultView"].ToString());
                }

                if (list.RootFolder.Properties["Timeline_Timeline"] != null)
                {
                    xmlWriter.WriteAttributeString("Timeline",
                        list.RootFolder.Properties["Timeline_Timeline"].ToString());
                }

                if (list.RootFolder.Properties["vti_welcomepage"] != null)
                {
                    xmlWriter.WriteAttributeString("WelcomePage",
                        list.RootFolder.Properties["vti_welcomepage"].ToString());
                }

                this.WriteBCSPropertiesToXml(xmlWriter, list);
                xmlWriter.WriteStartElement("Views");
                foreach (SPView sPView in ((IEnumerable)list.Views))
                {
                    string htmlSchemaXml = sPView.HtmlSchemaXml;
                    xmlWriter.WriteRaw(htmlSchemaXml);
                }

                xmlWriter.WriteEndElement();
                this.WriteFieldXml(list.Fields, list.ParentWeb, xmlWriter);
                if (list != null)
                {
                    xmlWriter.WriteStartElement("ContentTypeOrder");
                    IList<SPContentType> contentTypeOrder = list.RootFolder.ContentTypeOrder;
                    foreach (SPContentType current in contentTypeOrder)
                    {
                        xmlWriter.WriteStartElement("ContentTypeName");
                        xmlWriter.WriteAttributeString("Name", current.Name);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
        }

        private static bool AttemptToGetListQuickLaunchValue(SPList list, out bool value)
        {
            Func<bool> codeBlockToRetry = () => list.OnQuickLaunch;
            value = false;
            try
            {
                value = ExecuteWithRetry.AttemptToExecuteWithRetry<bool>(codeBlockToRetry, null);
                return true;
            }
            catch (ArgumentException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return false;
        }

        private void WriteFieldXml(SPFieldCollection fields, SPWeb currentWeb, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Fields");
            for (int i = 0; i < fields.Count; i++)
            {
                try
                {
                    SPField sPField = fields[i];
                    string text = sPField.SchemaXml;
                    if ((sPField.TypeAsString == "Lookup" || sPField.TypeAsString == "LookupMulti") && !sPField.Hidden)
                    {
                        text = this.AddListNamesToLookups(currentWeb, text);
                    }

                    xmlWriter.WriteRaw(text);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    xmlWriter.WriteComment(string.Format("Reading field at index {0} failed: {1}", i, ex.Message));
                }
            }

            xmlWriter.WriteEndElement();
        }

        private string GetLanguageResourcesForFields(string fieldXML, SPWeb web, SPField field)
        {
            return fieldXML;
        }

        private string GetLanguageResourcesForViews(string viewXML, SPWeb web, SPView view)
        {
            return viewXML;
        }

        private string AddListNamesToLookups(SPWeb web, string sFieldXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFieldXML);
            XmlNode firstChild = xmlDocument.FirstChild;
            if (firstChild.Attributes["List"] == null)
            {
                return sFieldXML;
            }

            string value = firstChild.Attributes["List"].Value;
            if (!Utils.IsGuid(value))
            {
                return sFieldXML;
            }

            string result;
            try
            {
                Guid guid = OMAdapter.s_EmptyGuid;
                XmlAttribute xmlAttribute = firstChild.Attributes["WebId"];
                if (xmlAttribute != null)
                {
                    string value2 = firstChild.Attributes["WebId"].Value;
                    if (Utils.IsGuid(value2))
                    {
                        guid = new Guid(value2);
                    }
                    else
                    {
                        firstChild.Attributes.Remove(xmlAttribute);
                    }
                }

                string text = null;
                string text2 = null;
                string text3 = null;
                if (guid == OMAdapter.s_EmptyGuid || guid == web.ID)
                {
                    SPList sPList = web.Lists[new Guid(value)];
                    text = sPList.RootFolder.Name;
                    if (guid == web.ID)
                    {
                        text2 = web.ServerRelativeUrl;
                        text3 = Utils.GetNameFromURL(web.Url);
                    }
                }
                else
                {
                    using (SPWeb sPWeb = web.Site.OpenWeb(guid))
                    {
                        text2 = sPWeb.ServerRelativeUrl;
                        text3 = Utils.GetNameFromURL(sPWeb.Url);
                        SPList sPList2 = sPWeb.Lists[new Guid(value)];
                        text = sPList2.RootFolder.Name;
                    }
                }

                if (text2 != null)
                {
                    XmlAttribute xmlAttribute2 = firstChild.OwnerDocument.CreateAttribute("TargetWebSRURL");
                    xmlAttribute2.Value = text2;
                    firstChild.Attributes.Append(xmlAttribute2);
                }

                if (text3 != null)
                {
                    XmlAttribute xmlAttribute3 = firstChild.OwnerDocument.CreateAttribute("TargetWebName");
                    xmlAttribute3.Value = text3;
                    firstChild.Attributes.Append(xmlAttribute3);
                }

                if (text != null)
                {
                    XmlAttribute xmlAttribute4 = firstChild.OwnerDocument.CreateAttribute("TargetListName");
                    xmlAttribute4.Value = text;
                    firstChild.Attributes.Append(xmlAttribute4);
                }

                result = xmlDocument.OuterXml;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = sFieldXML;
            }

            return result;
        }

        private string GetDirName(SPList list, SPFolder folder)
        {
            string url = folder.Url;
            int num = url.LastIndexOf("/", StringComparison.Ordinal);
            string text = "";
            if (num > 0)
            {
                text = url.Substring(0, num);
            }

            string text2 = list.ParentWebUrl.TrimStart(new char[]
            {
                '/'
            });
            return text2 + ((text.Length > 0 && text2.Length > 0) ? "/" : "") + text;
        }

        public string GetAlerts(string sListID, int iItemID)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb currentWeb = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.WriteStartElement("AlertCollection");
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sPSite = new SPSite(currentWeb.Site.ID))
                    {
                        using (SPWeb sPWeb = sPSite.OpenWeb(currentWeb.ID))
                        {
                            foreach (SPAlert sPAlert in ((IEnumerable)sPWeb.Alerts))
                            {
                                if (sPAlert.User != null)
                                {
                                    bool flag = string.IsNullOrEmpty(sListID) ||
                                                sListID.Equals(sPAlert.ListID.ToString(),
                                                    StringComparison.OrdinalIgnoreCase);
                                    bool flag2 = iItemID < 0 || (sPAlert.Item != null && sPAlert.ItemID == iItemID);
                                    if (flag && flag2)
                                    {
                                        this.FillAlertXML(xmlWriter, sPAlert, sPWeb.RegionalSettings.TimeZone);
                                    }
                                }
                            }
                        }
                    }
                });
                xmlWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        private void FillAlertXML(XmlWriter xmlWriter, SPAlert alert)
        {
            this.FillAlertXML(xmlWriter, alert, null);
        }

        private void FillAlertXML(XmlWriter xmlWriter, SPAlert alert, SPTimeZone timeZone)
        {
            xmlWriter.WriteStartElement("Alert");
            xmlWriter.WriteAttributeString("Title", alert.Title);
            xmlWriter.WriteAttributeString("AlertTemplate", alert.AlertTemplateName.ToString());
            xmlWriter.WriteAttributeString("ID", alert.ID.ToString());
            xmlWriter.WriteAttributeString("User", alert.User.LoginName);
            xmlWriter.WriteAttributeString("AlertType", alert.AlertType.ToString());
            xmlWriter.WriteAttributeString("ListID", alert.ListID.ToString());
            xmlWriter.WriteAttributeString("AlertFrequency", alert.AlertFrequency.ToString());
            xmlWriter.WriteAttributeString("Status", alert.Status.ToString());
            xmlWriter.WriteAttributeString("DynamicRecipient", alert.DynamicRecipient);
            xmlWriter.WriteAttributeString("ListTitle", alert.List.Title);
            xmlWriter.WriteAttributeString("Filter", alert.Filter);
            xmlWriter.WriteAttributeString("EventType", alert.EventType.ToString());
            if (alert.Item != null)
            {
                xmlWriter.WriteAttributeString("LinkFilename", alert.Item.Name);
                xmlWriter.WriteAttributeString("ItemID", alert.ItemID.ToString());
                xmlWriter.WriteAttributeString("ItemGUID", alert.Item.UniqueId.ToString());
            }

            if (alert.AlertFrequency.ToString() != "Immediate" && timeZone != null)
            {
                DateTime dt = timeZone.LocalTimeToUTC(alert.AlertTime);
                string value = Utils.FormatDate(dt);
                xmlWriter.WriteAttributeString("AlertTime", value);
            }

            xmlWriter.WriteStartElement("PropertyBag");
            SPPropertyBag properties = alert.Properties;
            foreach (DictionaryEntry dictionaryEntry in properties)
            {
                xmlWriter.WriteAttributeString(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("UserDetails");
            string xml = alert.User.Xml;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//User");
            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                xmlWriter.WriteAttributeString(xmlAttribute.Name, xmlAttribute.Value);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        public string GetList(string listId)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    SPList sPList = web.Lists[new Guid(listId)];
                    if (sPList == null)
                    {
                        throw new Exception(string.Format("The list with ID: '{0}' does not exist in on the site: {1}",
                            listId, web.Url));
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    this.GetListXML(sPList, xmlWriter, true);
                    operationReporting.LogObjectXml(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "GetList main");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void GetRecordDeclarationSettingsXml(XmlWriter xmlWriter, SPList list)
        {
            if (list.RootFolder.Properties["ecm_IPRListUseListSpecific"] != null)
            {
                xmlWriter.WriteAttributeString("IPRListUseListSpecific",
                    Convert.ToString(list.RootFolder.Properties["ecm_IPRListUseListSpecific"]));
            }

            if (list.RootFolder.Properties["ecm_ListFieldsReadyForIPR"] != null)
            {
                xmlWriter.WriteAttributeString("ListFieldsReadyForIPR",
                    Convert.ToString(list.RootFolder.Properties["ecm_ListFieldsReadyForIPR"]));
            }

            if (list.RootFolder.Properties["ecm_ListReadyForIPR"] != null)
            {
                xmlWriter.WriteAttributeString("ListReadyForIPR",
                    Convert.ToString(list.RootFolder.Properties["ecm_ListReadyForIPR"]));
            }

            if (list.RootFolder.Properties["ecm_ListReadyForLocking"] != null)
            {
                xmlWriter.WriteAttributeString("ListReadyForLocking",
                    Convert.ToString(list.RootFolder.Properties["ecm_ListReadyForLocking"]));
            }

            if (list.RootFolder.Properties["ecm_AllowManualDeclaration"] != null)
            {
                xmlWriter.WriteAttributeString("AllowManualDeclaration",
                    Convert.ToString(list.RootFolder.Properties["ecm_AllowManualDeclaration"]));
            }

            if (list.RootFolder.Properties["ecm_AutoDeclareRecords"] != null)
            {
                xmlWriter.WriteAttributeString("AutoDeclareRecords",
                    Convert.ToString(list.RootFolder.Properties["ecm_AutoDeclareRecords"]));
            }
        }

        public void GetColumnDefaultSettings(string listId, OperationReporting opResult)
        {
        }

        public string DeleteList(string sListID)
        {
            using (Context context = this.GetContext())
            {
                using (SPWeb web = context.Web)
                {
                    SPList sPList = web.Lists[new Guid(sListID)];
                    if (sPList == null)
                    {
                        throw new Exception("The list with ID: '" + sListID + "' does not exist in on the site: " +
                                            web.Url);
                    }

                    this.DeleteList(sPList);
                }
            }

            return string.Empty;
        }

        private void DeleteList(SPList list)
        {
            if (base.SharePointVersion.IsSharePoint2007)
            {
                this.DeleteAllMultiValueLookupColumns(list);
            }

            list.Delete();
        }

        private void DeleteAllMultiValueLookupColumns(SPList list)
        {
            List<SPField> list2 = new List<SPField>();
            foreach (SPField sPField in ((IEnumerable)list.Fields))
            {
                if ((sPField.CanBeDeleted &&
                     (sPField.Type == SPFieldType.Lookup || sPField.Type == SPFieldType.Invalid) &&
                     sPField.FieldValueType == typeof(SPFieldLookupValueCollection)) ||
                    (sPField.Type == SPFieldType.User && sPField.FieldValueType == typeof(SPFieldUserValueCollection)))
                {
                    list2.Add(sPField);
                }
            }

            foreach (SPField current in list2)
            {
                try
                {
                    current.Delete();
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        public string AddList(string sListXML, AddListOptions options, byte[] documentTemplateFile)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sListXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                if (firstChild.Attributes["Name"] == null || string.IsNullOrEmpty(firstChild.Attributes["Name"].Value))
                {
                    throw new Exception(
                        "Invalid XML was supplied to AddList(). Required attribute 'Name' is either missing or empty.");
                }

                if (firstChild.Attributes["Title"] == null)
                {
                    throw new Exception("Invalid XML was supplied to AddList(). Missing required 'Title' attribute");
                }

                string value = firstChild.Attributes["Name"].Value;
                string text = (firstChild.Attributes["Description"] == null)
                    ? ""
                    : firstChild.Attributes["Description"].Value;
                string text2 = firstChild.Attributes["Title"].Value;
                string value2 = null;
                if (base.SharePointVersion.IsSharePoint2010 &&
                    (firstChild.Attributes["BaseTemplate"].InnerText == "851" ||
                     firstChild.Attributes["BaseTemplate"].InnerText == "1302"))
                {
                    SPListTemplateCollection listTemplates = web.ListTemplates;
                    foreach (SPListTemplate sPListTemplate in ((IEnumerable)listTemplates))
                    {
                        if (firstChild.Attributes["BaseTemplate"].InnerText == "851" &&
                            sPListTemplate.Type.ToString() == "851")
                        {
                            value2 = sPListTemplate.FeatureId.ToString();
                        }
                        else if (firstChild.Attributes["BaseTemplate"].InnerText == "1302" &&
                                 sPListTemplate.Type.ToString() == "1302")
                        {
                            value2 = sPListTemplate.FeatureId.ToString();
                        }
                    }

                    XmlAttribute xmlAttribute = firstChild.Attributes["FeatureId"];
                    if (xmlAttribute == null)
                    {
                        xmlAttribute = firstChild.OwnerDocument.CreateAttribute("FeatureId");
                        firstChild.Attributes.Append(xmlAttribute);
                    }

                    xmlAttribute.Value = value2;
                }

                SPList sPList = (value == text2)
                    ? OMAdapter.GetListByTitle(web, text2)
                    : OMAdapter.GetListByName(web, value);
                if (options.Overwrite && sPList != null)
                {
                    bool flag = false;
                    if (sPList.BaseTemplate != SPListTemplateType.PictureLibrary)
                    {
                        try
                        {
                            sPList.Delete();
                            flag = true;
                            sPList = null;
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }

                    if (!flag)
                    {
                        bool flag2;
                        SPListItemCollection[] fullListItemCollectionsNoMetaData =
                            this.GetFullListItemCollectionsNoMetaData(sPList, out flag2);
                        SPListItemCollection[] array = fullListItemCollectionsNoMetaData;
                        for (int i = 0; i < array.Length; i++)
                        {
                            SPListItemCollection items = array[i];
                            this.DeleteAllListItems(items);
                        }
                    }
                }

                SPList sPList2 = null;
                bool flag3 = false;
                try
                {
                    if (sPList == null)
                    {
                        flag3 = true;
                        if (firstChild.Attributes["BaseTemplate"] == null)
                        {
                            throw new Exception(
                                "Invalid XML was supplied to AddList(). Missing required 'BaseTemplate' attribute");
                        }

                        SPListTemplateType sPListTemplateType =
                            (SPListTemplateType)Enum.Parse(typeof(SPListTemplateType),
                                firstChild.Attributes["BaseTemplate"].Value);
                        Guid uniqueID = Guid.Empty;
                        sPList2 = OMAdapter.GetListByTitle(web, value);
                        if (sPList2 != null)
                        {
                            string text3 = value + "_";
                            while (OMAdapter.GetListByTitle(web, text3) != null)
                            {
                                text3 += "_";
                            }

                            sPList2.Title = text3;
                            sPList2.Update();
                        }

                        string docTemplateType = (firstChild.Attributes["DocTemplateUrl"] != null)
                            ? this.ParseDocumentTemplateType(firstChild.Attributes["DocTemplateUrl"].Value).ToString()
                            : null;
                        string text4 = (firstChild.Attributes["FeatureId"] != null)
                            ? firstChild.Attributes["FeatureId"].InnerText
                            : null;
                        if (sPListTemplateType == SPListTemplateType.ExternalList)
                        {
                            uniqueID = this.CreateExternalList(web, value, text, firstChild);
                        }
                        else
                        {
                            if (base.SharePointVersion.IsSharePoint2013OrLater)
                            {
                                if (sPListTemplateType == SPListTemplateType.AccessRequest)
                                {
                                    goto IL_544;
                                }
                            }

                            try
                            {
                                uniqueID = web.Lists.Add(value, text, null, text4, (int)sPListTemplateType,
                                    docTemplateType);
                            }
                            catch (SPException ex2)
                            {
                                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                bool flag4 = !string.IsNullOrEmpty(text4) &&
                                             ex2.Message.Contains(new Guid(text4).ToString("D"));
                                if (flag4)
                                {
                                    uniqueID = web.Lists.Add(value, text, null, null, (int)sPListTemplateType,
                                        docTemplateType);
                                }
                                else
                                {
                                    if (firstChild.Attributes["BaseType"] == null)
                                    {
                                        throw;
                                    }

                                    SPListTemplateType templateType = SPListTemplateType.GenericList;
                                    switch (Convert.ToInt32(firstChild.Attributes["BaseType"].InnerText))
                                    {
                                        case 0:
                                            templateType = SPListTemplateType.GenericList;
                                            break;
                                        case 1:
                                            templateType = SPListTemplateType.DocumentLibrary;
                                            break;
                                        case 3:
                                            templateType = SPListTemplateType.DiscussionBoard;
                                            break;
                                        case 4:
                                            templateType = SPListTemplateType.Survey;
                                            break;
                                        case 5:
                                            templateType = SPListTemplateType.IssueTracking;
                                            break;
                                    }

                                    uniqueID = web.Lists.Add(value, text, null, null, (int)templateType,
                                        docTemplateType);
                                }
                            }
                        }

                        IL_544:
                        sPList = web.Lists[uniqueID];
                        SPFolder rootFolder = sPList.RootFolder;
                        if (options.EnsureUrlNameMatchesInput && !string.IsNullOrEmpty(value) &&
                            rootFolder.Name != value)
                        {
                            string text5 = rootFolder.ServerRelativeUrl;
                            int num = text5.LastIndexOf('/');
                            if (num < 0)
                            {
                                text5 = value;
                            }
                            else
                            {
                                text5 = text5.Substring(0, num + 1) + value;
                            }

                            rootFolder.MoveTo(text5);
                            SPDocumentLibrary sPDocumentLibrary = sPList as SPDocumentLibrary;
                            if (sPDocumentLibrary != null)
                            {
                                string text6 = sPDocumentLibrary.DocumentTemplateUrl;
                                if (!string.IsNullOrEmpty(text6))
                                {
                                    int num2 = text6.IndexOf('/');
                                    text6 = text6.Substring(num2, text6.Length - num2);
                                    sPDocumentLibrary.DocumentTemplateUrl = value + text6;
                                }
                            }
                        }
                    }
                    else
                    {
                        sPList.Description = text;
                    }

                    int num3 = 1;
                    string str = text2;
                    if (sPList2 != null && value == text2)
                    {
                        text2 = str + num3.ToString();
                        num3++;
                    }

                    SPList listByTitle = OMAdapter.GetListByTitle(web, text2);
                    while (listByTitle != null && listByTitle.ID != sPList.ID &&
                           listByTitle.RootFolder.Name != sPList.RootFolder.Name)
                    {
                        text2 = str + num3.ToString();
                        num3++;
                        listByTitle = OMAdapter.GetListByTitle(web, text2);
                    }

                    sPList.Title = text2;
                    bool multipleDataList = sPList.MultipleDataList;
                    this.UpdateListProperties(sPList, null, firstChild, options, documentTemplateFile);
                    if (flag3 && multipleDataList && !sPList.MultipleDataList &&
                        sPList.BaseType == SPBaseType.DocumentLibrary)
                    {
                        List<SPFolder> list = new List<SPFolder>();
                        foreach (SPFolder sPFolder in ((IEnumerable)sPList.RootFolder.SubFolders))
                        {
                            int num4;
                            if (int.TryParse(sPFolder.Name, out num4))
                            {
                                list.Add(sPFolder);
                            }
                        }

                        foreach (SPFolder current in list)
                        {
                            current.Delete();
                        }
                    }
                }
                finally
                {
                    if (sPList2 != null && sPList2.Title != value)
                    {
                        sPList2.Title = value;
                        sPList2.Update();
                    }
                }

                sPList.RootFolder.Update();
                sPList.Update();
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.GetListXML(sPList, xmlWriter, false);
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string DisableValidationSettings(string listID)
        {
            throw new NotImplementedException();
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            throw new NotImplementedException();
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            throw new NotImplementedException();
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile)
        {
            if (!Utils.IsGuid(sListID))
            {
                throw new ArgumentException("The specified List ID is not a valid GUID");
            }

            string result;
            using (Context context = this.GetContext())
            {
                SPList sPList = context.Web.Lists[new Guid(sListID)];
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sListXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                if (firstChild.Attributes["Title"] != null)
                {
                    string value = firstChild.Attributes["Title"].Value;
                    if (OMAdapter.GetListByTitle(context.Web, value) == null)
                    {
                        sPList.Title = value;
                    }
                }

                if (firstChild.Attributes["Description"] != null)
                {
                    string value2 = firstChild.Attributes["Description"].Value;
                    if (!string.IsNullOrEmpty(value2))
                    {
                        sPList.Description = value2;
                    }
                }

                this.UpdateListProperties(sPList, sViewXml, firstChild, updateOptions, documentTemplateFile);
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.GetListXML(sPList, xmlWriter, false);
                result = stringBuilder.ToString();
            }

            return result;
        }

        private void UpdateListProperties(SPList list, string sViewXml, XmlNode listXML, IUpdateListOptions options,
            byte[] documentTemplateFile)
        {
            if (listXML.Attributes["Description"] != null)
            {
                list.Description = listXML.Attributes["Description"].Value;
            }

            if (list.BaseType != SPBaseType.DocumentLibrary && list.BaseType != SPBaseType.Survey &&
                listXML.Attributes["EnableAttachments"] != null)
            {
                list.EnableAttachments = bool.Parse(listXML.Attributes["EnableAttachments"].Value);
            }

            if ((list.BaseTemplate != SPListTemplateType.PictureLibrary ||
                 !base.SharePointVersion.IsSharePoint2007OrEarlier) && listXML.Attributes["EnableModeration"] != null)
            {
                list.EnableModeration = bool.Parse(listXML.Attributes["EnableModeration"].Value);
            }

            if (options.CopyEnableAssignToEmail && listXML.Attributes["EnableAssignToEmail"] != null)
            {
                list.EnableAssignToEmail = bool.Parse(listXML.Attributes["EnableAssignToEmail"].Value);
            }

            if (listXML.Attributes["OnQuickLaunch"] != null)
            {
                list.OnQuickLaunch = bool.Parse(listXML.Attributes["OnQuickLaunch"].Value);
            }

            if (listXML.Attributes["Hidden"] != null)
            {
                list.Hidden = bool.Parse(listXML.Attributes["Hidden"].Value);
            }

            if (listXML.Attributes["ForceCheckout"] != null)
            {
                list.ForceCheckout = bool.Parse(listXML.Attributes["ForceCheckout"].Value);
            }

            if (listXML.Attributes["DraftVersionVisibility"] != null)
            {
                int num = int.Parse(listXML.Attributes["DraftVersionVisibility"].Value);
                if (num == 0)
                {
                    list.DraftVersionVisibility = DraftVisibilityType.Reader;
                }

                if (num == 1)
                {
                    list.DraftVersionVisibility = DraftVisibilityType.Author;
                }

                if (num == 2)
                {
                    list.DraftVersionVisibility = DraftVisibilityType.Approver;
                }
            }
            else
            {
                list.DraftVersionVisibility = DraftVisibilityType.Approver;
            }

            if (list.BaseTemplate != SPListTemplateType.Survey && listXML.Attributes["EnableVersioning"] != null)
            {
                list.EnableVersioning = bool.Parse(listXML.Attributes["EnableVersioning"].Value);
            }

            if (list.EnableVersioning)
            {
                if (listXML.Attributes["MajorVersionLimit"] != null)
                {
                    list.MajorVersionLimit = int.Parse(listXML.Attributes["MajorVersionLimit"].Value);
                }

                if (listXML.Attributes["EnableMinorVersions"] != null)
                {
                    list.EnableMinorVersions = bool.Parse(listXML.Attributes["EnableMinorVersions"].Value);
                }

                if (listXML.Attributes["MajorWithMinorVersionsLimit"] != null &&
                    listXML.Attributes["EnableModeration"] != null &&
                    (list.EnableModeration || list.EnableMinorVersions))
                {
                    list.MajorWithMinorVersionsLimit =
                        int.Parse(listXML.Attributes["MajorWithMinorVersionsLimit"].Value);
                }
            }

            if (listXML.Attributes["BrowserEnabledDocuments"] != null)
            {
                if (list.BaseTemplate == SPListTemplateType.NoCodeWorkflows && list.Title == "Workflows")
                {
                    list.DefaultItemOpen = DefaultItemOpen.Browser;
                }
                else
                {
                    list.DefaultItemOpen = (DefaultItemOpen)Enum.Parse(typeof(DefaultItemOpen),
                        listXML.Attributes["BrowserEnabledDocuments"].Value);
                }
            }
            else if (list.BaseTemplate == SPListTemplateType.NoCodeWorkflows && list.Title == "Workflows")
            {
                list.DefaultItemOpen = DefaultItemOpen.Browser;
            }

            if (listXML.Attributes["SendToLocationName"] != null)
            {
                list.SendToLocationName = listXML.Attributes["SendToLocationName"].Value;
            }

            if (listXML.Attributes["SendToLocationUrl"] != null)
            {
                list.SendToLocationUrl = listXML.Attributes["SendToLocationUrl"].Value;
            }

            if (listXML.Attributes["EmailAttachmentFolder"] != null)
            {
                list.RootFolder.Properties["vti_emailattachmentfolders"] =
                    listXML.Attributes["EmailAttachmentFolder"].Value;
            }

            if (listXML.Attributes["EmailOverWrite"] != null)
            {
                list.RootFolder.Properties["vti_emailoverwrite"] =
                    int.Parse(listXML.Attributes["EmailOverWrite"].Value);
            }

            if (listXML.Attributes["EmailSaveOriginal"] != null)
            {
                list.RootFolder.Properties["vti_emailsaveoriginal"] =
                    int.Parse(listXML.Attributes["EmailSaveOriginal"].Value);
            }

            if (listXML.Attributes["EmailSaveMeetings"] != null)
            {
                list.RootFolder.Properties["vti_emailsavemeetings"] =
                    int.Parse(listXML.Attributes["EmailSaveMeetings"].Value);
            }

            if (listXML.Attributes["EmailUseSecurity"] != null)
            {
                list.RootFolder.Properties["vti_emailusesecurity"] =
                    int.Parse(listXML.Attributes["EmailUseSecurity"].Value);
            }

            if (listXML.Attributes["ContentTypesEnabled"] != null && list.AllowContentTypes)
            {
                list.ContentTypesEnabled = bool.Parse(listXML.Attributes["ContentTypesEnabled"].Value);
            }

            if (listXML.Attributes["Folders"] != null)
            {
                try
                {
                    list.EnableFolderCreation = bool.Parse(listXML.Attributes["Folders"].Value);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            if (listXML.Attributes["ReadSecurity"] != null)
            {
                list.ReadSecurity = int.Parse(listXML.Attributes["ReadSecurity"].Value);
            }

            if (listXML.Attributes["WriteSecurity"] != null)
            {
                list.WriteSecurity = int.Parse(listXML.Attributes["WriteSecurity"].Value);
            }

            if (list.BaseType == SPBaseType.Survey)
            {
                if (listXML.Attributes["AllowMultiResponses"] != null)
                {
                    list.AllowMultiResponses = bool.Parse(listXML.Attributes["AllowMultiResponses"].Value);
                }

                if (listXML.Attributes["ShowUser"] != null)
                {
                    list.ShowUser = bool.Parse(listXML.Attributes["ShowUser"].Value);
                }
            }

            if (listXML.Attributes["NoCrawl"] != null)
            {
                list.NoCrawl = bool.Parse(listXML.Attributes["NoCrawl"].Value);
            }

            if (listXML.Attributes["MultipleDataList"] != null && SPMeeting.IsMeetingWorkspaceWeb(list.ParentWeb))
            {
                list.MultipleDataList = bool.Parse(listXML.Attributes["MultipleDataList"].Value);
            }

            if (listXML.Attributes["EnableSyndication"] != null)
            {
                list.EnableSyndication = bool.Parse(listXML.Attributes["EnableSyndication"].Value);
            }

            if (listXML.Attributes["RssLimitDescriptionLength"] != null)
            {
                list.RootFolder.Properties["vti_rss_LimitDescriptionLength"] =
                    int.Parse(listXML.Attributes["RssLimitDescriptionLength"].Value);
            }

            if (listXML.Attributes["RssChannelTitle"] != null)
            {
                list.RootFolder.Properties["vti_rss_ChannelTitle"] = listXML.Attributes["RssChannelTitle"].Value;
            }

            if (listXML.Attributes["RssChannelDescription"] != null)
            {
                string text = listXML.Attributes["RssChannelDescription"].Value;
                text = text.Replace("\\r\\n", "\r\n");
                list.RootFolder.Properties["vti_rss_ChannelDescription"] = text;
            }

            if (listXML.Attributes["RssChannelImageUrl"] != null)
            {
                list.RootFolder.Properties["vti_rss_ChannelImageUrl"] = listXML.Attributes["RssChannelImageUrl"].Value;
            }

            if (listXML.Attributes["RssItemLimit"] != null)
            {
                list.RootFolder.Properties["vti_rss_ItemLimit"] = int.Parse(listXML.Attributes["RssItemLimit"].Value);
            }

            if (listXML.Attributes["RssDayLimit"] != null)
            {
                list.RootFolder.Properties["vti_rss_DayLimit"] = int.Parse(listXML.Attributes["RssDayLimit"].Value);
            }

            if (listXML.Attributes["RssDocumentAsEnclosure"] != null)
            {
                list.RootFolder.Properties["vti_rss_DocumentAsEnclosure"] =
                    int.Parse(listXML.Attributes["RssDocumentAsEnclosure"].Value);
            }

            if (listXML.Attributes["RssDocumentAsLink"] != null)
            {
                list.RootFolder.Properties["vti_rss_DocumentAsLink"] =
                    int.Parse(listXML.Attributes["RssDocumentAsLink"].Value);
            }

            if (listXML.Attributes["TimelineDefaultView"] != null)
            {
                list.RootFolder.Properties["TimelineDefaultView"] = listXML.Attributes["TimelineDefaultView"].Value;
            }

            if (listXML.Attributes["Timeline"] != null)
            {
                list.RootFolder.Properties["Timeline_Timeline"] = listXML.Attributes["Timeline"].Value;
            }

            if (listXML.Attributes["WelcomePage"] != null)
            {
                list.RootFolder.Properties["vti_welcomepage"] = listXML.Attributes["WelcomePage"].Value;
            }

            list.Update();
            list.RootFolder.Update();
            try
            {
                if (listXML.Attributes["DocTemplateUrl"] != null &&
                    list.GetType().IsAssignableFrom(typeof(SPDocumentLibrary)) && documentTemplateFile != null &&
                    documentTemplateFile.Length > 0)
                {
                    SPDocumentLibrary sPDocumentLibrary = (SPDocumentLibrary)list;
                    this.SetDocumentLibraryTemplate(sPDocumentLibrary, listXML.Attributes["DocTemplateUrl"].Value,
                        documentTemplateFile);
                    if (listXML.Attributes["BrowserActivatedTemplate"] != null)
                    {
                        bool flag = false;
                        bool flag2 = bool.TryParse(listXML.Attributes["BrowserActivatedTemplate"].Value, out flag);
                        if (flag2 && flag && sPDocumentLibrary.BaseTemplate == SPListTemplateType.XMLForm)
                        {
                            this.TryBrowserActivateFormTemplate(sPDocumentLibrary);
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
            }

            if (listXML.Attributes["EmailAlias"] != null)
            {
                try
                {
                    list.EmailAlias = listXML.Attributes["EmailAlias"].Value;
                    list.Update();
                }
                catch (Exception ex3)
                {
                    OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            XmlNode xmlNode = listXML.SelectSingleNode("./Fields");
            if (xmlNode != null && options.CopyFields)
            {
                this.AddFieldsXML(list.Fields, xmlNode, options.UpdateFieldTypes);
            }

            XmlNode xmlNode2;
            if (sViewXml != null)
            {
                xmlNode2 = XmlUtility.StringToXmlNode(sViewXml);
            }
            else
            {
                xmlNode2 = listXML.SelectSingleNode("//Views");
            }

            if (xmlNode2 != null && options.CopyViews)
            {
                OMAdapter.AddViewsXML(list, xmlNode2, options.DeletePreExistingViews);
            }
        }

        public string AddView(string sListID, string sViewXML)
        {
            if (!Utils.IsGuid(sListID))
            {
                throw new ArgumentException("The specified List ID is not a valid GUID");
            }

            if (string.IsNullOrEmpty(sViewXML))
            {
                throw new ArgumentException("The view XML is null or empty.");
            }

            sViewXML = "<Views>" + sViewXML + "</Views>";
            string htmlSchemaXml;
            using (Context context = this.GetContext())
            {
                SPList list = context.Web.Lists[new Guid(sListID)];
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sViewXML);
                OMAdapter.AddViewsXML(list, xmlNode, false);
                SPView view = OMAdapter.GetView(list, xmlNode.ChildNodes[0].Attributes["Url"].Value);
                htmlSchemaXml = view.HtmlSchemaXml;
            }

            return htmlSchemaXml;
        }

        private int ParseDocumentTemplateType(string sDocTemplateUrl)
        {
            int result = 100;
            if (!string.IsNullOrEmpty(sDocTemplateUrl))
            {
                string fileNameFromPath = Utils.GetFileNameFromPath(sDocTemplateUrl, true);
                if (string.Equals(fileNameFromPath, "template.doc", StringComparison.OrdinalIgnoreCase))
                {
                    result = 101;
                }
                else if (string.Equals(fileNameFromPath, "template.htm", StringComparison.OrdinalIgnoreCase))
                {
                    result = 102;
                }
                else if (string.Equals(fileNameFromPath, "template.xls", StringComparison.OrdinalIgnoreCase))
                {
                    result = 103;
                }
                else if (string.Equals(fileNameFromPath, "template.pot", StringComparison.OrdinalIgnoreCase))
                {
                    result = 104;
                }
                else if (string.Equals(fileNameFromPath, "_basicpage.htm", StringComparison.OrdinalIgnoreCase))
                {
                    result = 105;
                }
                else if (string.Equals(fileNameFromPath, "_webpartpage.htm", StringComparison.OrdinalIgnoreCase))
                {
                    result = 106;
                }
                else if (string.Equals(fileNameFromPath, "template.one", StringComparison.OrdinalIgnoreCase))
                {
                    result = 111;
                }
                else if (string.Equals(fileNameFromPath, "template.dotx", StringComparison.OrdinalIgnoreCase))
                {
                    result = 121;
                }
                else if (string.Equals(fileNameFromPath, "template.xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    result = 122;
                }
                else if (string.Equals(fileNameFromPath, "template.pptx", StringComparison.OrdinalIgnoreCase))
                {
                    result = 123;
                }
                else if (string.Equals(fileNameFromPath, "template.xml", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(fileNameFromPath, "template.xsn", StringComparison.OrdinalIgnoreCase))
                {
                    result = 1000;
                }
            }

            return result;
        }

        private void SetDocumentLibraryTemplate(SPDocumentLibrary targetLibrary, string sSourceTemplateUrl,
            byte[] templateFile)
        {
            if (targetLibrary == null || string.IsNullOrEmpty(sSourceTemplateUrl) || templateFile == null)
            {
                return;
            }

            try
            {
                string fileNameFromPath = Utils.GetFileNameFromPath(sSourceTemplateUrl, true);
                string text;
                if (string.IsNullOrEmpty(targetLibrary.DocumentTemplateUrl))
                {
                    text = targetLibrary.RootFolder.ServerRelativeUrl + "/Forms";
                }
                else
                {
                    text = targetLibrary.DocumentTemplateUrl.Remove(targetLibrary.DocumentTemplateUrl.LastIndexOf('/'));
                }

                string text2 = text + '/' + fileNameFromPath;
                text2 = SPHttpUtility.UrlPathDecode(text2.ToLower(), false);
                SPFolder folder = targetLibrary.ParentWeb.GetFolder(text);
                if (folder != null)
                {
                    folder.Files.Add(text2, templateFile, true);
                    targetLibrary.DocumentTemplateUrl = text2;
                    targetLibrary.Update();
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                string message = "Could not set the document template for library '" + targetLibrary.ToString() +
                                 "': " + ex.Message;
                throw new Exception(message, ex);
            }
        }

        public string ApplyOrUpdateContentType(string sListId, string sParentContentTypeName, string sContentTypeXml,
            bool bMakeDefaultContentType)
        {
            string schemaXml;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(sListId)];
                bool contentTypesEnabled = sPList.ContentTypesEnabled;
                CaseInvariantFieldCollection caseInvariantFieldCollection =
                    new CaseInvariantFieldCollection(sPList.Fields);
                CaseInvariantFieldCollection caseInvariantFieldCollection2 =
                    new CaseInvariantFieldCollection(web.Fields);
                List<string> list = null;
                try
                {
                    if (!sPList.ContentTypesEnabled)
                    {
                        sPList.ContentTypesEnabled = true;
                        sPList.Update();
                    }

                    SPContentType sPContentType = this.GetContentTypeFromListByParent(sParentContentTypeName, sPList);
                    if (sPContentType == null)
                    {
                        SPContentType sPContentType2 = web.AvailableContentTypes[sParentContentTypeName];
                        if (sPContentType2 == null)
                        {
                            throw new ArgumentException("There is no content type \"" + sParentContentTypeName +
                                                        "\" available on this web");
                        }

                        if (!sPList.IsContentTypeAllowed(sPContentType2))
                        {
                            throw new ArgumentException("The content type \"" + sParentContentTypeName +
                                                        "\" is not allowed on this list");
                        }

                        sPList.ContentTypes.Add(sPContentType2);
                        sPContentType = sPList.ContentTypes[sParentContentTypeName];
                    }

                    if (sPContentType == null)
                    {
                        throw new Exception(
                            "The content type was found to be available, but the adapter was unable to find the applied content type");
                    }

                    if (!sPContentType.Sealed && !string.IsNullOrEmpty(sContentTypeXml))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(sContentTypeXml);
                        XmlNode documentElement = xmlDocument.DocumentElement;
                        bool? attributeValueAsNullableBoolean =
                            documentElement.GetAttributeValueAsNullableBoolean("ReadOnly");
                        bool flag = attributeValueAsNullableBoolean.HasValue && sPContentType.ReadOnly &&
                                    !attributeValueAsNullableBoolean.Value;
                        if (!sPContentType.ReadOnly || flag)
                        {
                            if (flag)
                            {
                                sPContentType.ReadOnly = false;
                            }

                            XmlAttribute xmlAttribute = documentElement.Attributes["Name"];
                            if (xmlAttribute != null)
                            {
                                sPContentType.Name = xmlAttribute.Value;
                            }

                            XmlAttribute xmlAttribute2 = documentElement.Attributes["Description"];
                            if (xmlAttribute2 != null)
                            {
                                sPContentType.Description = xmlAttribute2.Value;
                            }

                            XmlAttribute xmlAttribute3 = documentElement.Attributes["RequireClientRenderingOnNew"];
                            if (xmlAttribute3 != null)
                            {
                                bool requireClientRenderingOnNew = false;
                                bool flag2 = bool.TryParse(xmlAttribute3.Value, out requireClientRenderingOnNew);
                                if (flag2)
                                {
                                    sPContentType.RequireClientRenderingOnNew = requireClientRenderingOnNew;
                                }
                            }

                            List<string> list2 = new List<string>();
                            foreach (XmlNode xmlNode in xmlDocument.SelectNodes(".//FieldRef"))
                            {
                                string text = xmlNode.Attributes["Name"].Value;
                                XmlAttribute attribute = XmlUtility.GetAttribute(xmlNode, null, "ID", false);
                                string id = (attribute != null) ? attribute.Value : string.Empty;
                                SPField sPField = OMAdapter.GetField(sPContentType.Fields, text, id);
                                SPFieldLink sPFieldLink = null;
                                if (sPField == null)
                                {
                                    sPField = caseInvariantFieldCollection2.GetFieldByNames(null, text);
                                    if (sPField == null)
                                    {
                                        sPField = caseInvariantFieldCollection.GetFieldByNames(null, text);
                                    }

                                    if (sPField != null)
                                    {
                                        sPFieldLink = new SPFieldLink(sPField);
                                        if (sPContentType.FieldLinks[sPFieldLink.Name] == null)
                                        {
                                            sPContentType.FieldLinks.Add(sPFieldLink);
                                        }
                                    }
                                }

                                if (sPField != null)
                                {
                                    if (sPFieldLink == null)
                                    {
                                        sPFieldLink = sPContentType.FieldLinks[sPField.InternalName];
                                    }

                                    if (sPFieldLink == null)
                                    {
                                        sPFieldLink = sPContentType.FieldLinks[sPField.Id];
                                    }

                                    if (sPFieldLink != null)
                                    {
                                        text = sPFieldLink.Name;
                                        XmlAttribute xmlAttribute4 = xmlNode.Attributes["ReadOnly"];
                                        if (xmlAttribute4 != null)
                                        {
                                            bool flag3 = bool.Parse(xmlAttribute4.Value);
                                            if (flag3 != sPFieldLink.ReadOnly)
                                            {
                                                sPFieldLink.ReadOnly = (flag3 || sPField.ReadOnlyField);
                                            }
                                        }

                                        XmlAttribute xmlAttribute5 = xmlNode.Attributes["Required"];
                                        if (xmlAttribute5 != null)
                                        {
                                            bool flag4 = bool.Parse(xmlAttribute5.Value);
                                            if (flag4 != sPFieldLink.Required)
                                            {
                                                sPFieldLink.Required = (flag4 || sPField.Required);
                                            }
                                        }

                                        XmlAttribute xmlAttribute6 = xmlNode.Attributes["Hidden"];
                                        if (xmlAttribute6 != null)
                                        {
                                            bool flag5 = bool.Parse(xmlAttribute6.Value);
                                            if (flag5 != sPFieldLink.Hidden)
                                            {
                                                sPFieldLink.Hidden = (flag5 || sPField.Hidden);
                                            }
                                        }

                                        this.SetInfoPathXMLDocumentProperties(xmlNode, sPFieldLink);
                                    }

                                    if (!list2.Contains(text))
                                    {
                                        list2.Add(text);
                                    }

                                    this.EnsureTaxonomyRelatedFieldsPresent(sPField, sPContentType, ref list2);
                                }
                            }

                            foreach (XmlNode xmlNode2 in xmlDocument.SelectNodes(".//Field"))
                            {
                                string text2 = (xmlNode2.Attributes["Name"] != null)
                                    ? xmlNode2.Attributes["Name"].Value
                                    : string.Empty;
                                XmlAttribute attribute2 = XmlUtility.GetAttribute(xmlNode2, null, "Id", false);
                                string id2 = (attribute2 != null) ? attribute2.Value : string.Empty;
                                SPField sPField2 = OMAdapter.GetField(sPContentType.Fields, text2, id2);
                                if (sPField2 == null && !string.IsNullOrEmpty(text2))
                                {
                                    sPField2 = caseInvariantFieldCollection2.GetFieldByNames(null, text2);
                                    if (sPField2 == null)
                                    {
                                        sPField2 = caseInvariantFieldCollection.GetFieldByNames(null, text2);
                                    }

                                    if (sPField2 == null)
                                    {
                                        this.AddFieldsXML(sPList.Fields, xmlNode2);
                                        sPField2 = OMAdapter.GetFieldByNames(sPList.Fields, null, text2);
                                    }

                                    if (sPField2 != null)
                                    {
                                        SPFieldLink fieldLink = new SPFieldLink(sPField2);
                                        this.SetInfoPathXMLDocumentProperties(xmlNode2, fieldLink);
                                        sPContentType.FieldLinks.Add(fieldLink);
                                    }
                                }

                                if (sPField2 != null && !list2.Contains(sPField2.InternalName))
                                {
                                    if (!list2.Contains(sPField2.InternalName))
                                    {
                                        list2.Add(sPField2.InternalName);
                                    }

                                    this.EnsureTaxonomyRelatedFieldsPresent(sPField2, sPContentType, ref list2);
                                }
                            }

                            List<SPFieldLink> list3 = new List<SPFieldLink>();
                            foreach (SPFieldLink sPFieldLink2 in ((IEnumerable)sPContentType.FieldLinks))
                            {
                                if (!list2.Contains(sPFieldLink2.Name))
                                {
                                    list3.Add(sPFieldLink2);
                                }
                            }

                            foreach (SPFieldLink current in list3)
                            {
                                sPContentType.FieldLinks.Delete(current.Name);
                            }

                            List<string> list4 = new List<string>();
                            foreach (string current2 in list2)
                            {
                                bool flag6 = false;
                                foreach (SPFieldLink sPFieldLink3 in ((IEnumerable)sPContentType.FieldLinks))
                                {
                                    if (current2 == sPFieldLink3.Name)
                                    {
                                        flag6 = true;
                                        break;
                                    }
                                }

                                if (!flag6)
                                {
                                    list4.Add(current2);
                                }
                            }

                            foreach (string current3 in list4)
                            {
                                list2.Remove(current3);
                            }

                            sPContentType.FieldLinks.Reorder(list2.ToArray());
                            foreach (XmlNode xmlNode3 in xmlDocument.SelectNodes(".//XmlDocument"))
                            {
                                bool flag7 = false;
                                foreach (string a in ((IEnumerable)sPContentType.XmlDocuments))
                                {
                                    if (a == xmlNode3.InnerXml)
                                    {
                                        flag7 = true;
                                        break;
                                    }
                                }

                                if (!flag7)
                                {
                                    XmlDocument xmlDocument2 = new XmlDocument();
                                    xmlDocument2.LoadXml(xmlNode3.InnerXml);
                                    XmlNode documentElement2 = xmlDocument2.DocumentElement;
                                    if (sPContentType.XmlDocuments[documentElement2.NamespaceURI] != null)
                                    {
                                        sPContentType.XmlDocuments.Delete(documentElement2.NamespaceURI);
                                    }

                                    sPContentType.XmlDocuments.Add(xmlDocument2);
                                }
                            }

                            sPContentType.Update();
                            if (sPContentType.Id.ToString()
                                    .StartsWith("0x0120D520", StringComparison.InvariantCultureIgnoreCase) &&
                                this.AreDocumentSetsSupported)
                            {
                                this.UpdateDocumentSetContentType(sPContentType, documentElement, web, sPList,
                                    out list);
                            }
                        }
                    }

                    if (bMakeDefaultContentType)
                    {
                        this.SetListDefaultContentType(sPList, sPContentType.Name);
                    }

                    schemaXml = sPContentType.SchemaXml;
                }
                finally
                {
                    if (!contentTypesEnabled)
                    {
                        sPList.ContentTypesEnabled = contentTypesEnabled;
                        sPList.Update();
                    }
                }
            }

            return schemaXml;
        }

        private void SetInfoPathXMLDocumentProperties(XmlNode fieldRefNode, SPFieldLink fieldLink)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {
                    "Node",
                    "XPath"
                },
                {
                    "PITarget",
                    null
                },
                {
                    "PrimaryPITarget",
                    null
                },
                {
                    "PIAttribute",
                    null
                },
                {
                    "PrimaryPIAttribute",
                    null
                },
                {
                    "Aggregation",
                    "AggregationFunction"
                }
            };
            foreach (KeyValuePair<string, string> current in dictionary)
            {
                string attributeValueAsString = fieldRefNode.GetAttributeValueAsString(current.Key);
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    string name = (current.Value == null) ? current.Key : current.Value;
                    PropertyInfo property = fieldLink.GetType().GetProperty(name);
                    if (property != null)
                    {
                        property.SetValue(fieldLink, attributeValueAsString, null);
                    }
                }
            }
        }

        private void EnsureTaxonomyRelatedFieldsPresent(SPField referencedField, SPContentType targetCT,
            ref List<string> sFieldNameList)
        {
        }

        private string GetRelatedFieldInternalName(SPWeb web, Guid relatedFieldGuid)
        {
            return string.Empty;
        }

        private SPContentType GetContentTypeFromListByParent(string sParentContentTypeName, SPList list)
        {
            foreach (SPContentType sPContentType in ((IEnumerable)list.ContentTypes))
            {
                if (sPContentType.Parent != null && sPContentType.Parent.Name == sParentContentTypeName)
                {
                    return sPContentType;
                }
            }

            return null;
        }

        private void SetListDefaultContentType(SPList list, string sContentTypeName)
        {
            try
            {
                SPContentType newDefault = null;
                foreach (SPContentType sPContentType in ((IEnumerable)list.ContentTypes))
                {
                    if (sPContentType.Name == sContentTypeName)
                    {
                        newDefault = sPContentType;
                        break;
                    }
                }

                if (newDefault != null && !newDefault.Sealed)
                {
                    IList<SPContentType> list2 = new List<SPContentType>();
                    list2 = list.RootFolder.ContentTypeOrder;
                    SPContentType sPContentType2 =
                        list2.SingleOrDefault((SPContentType ct) => ct.Id.Equals(newDefault.Id));
                    if (sPContentType2 != null)
                    {
                        list2.Remove(sPContentType2);
                    }

                    list2.Insert(0, newDefault);
                    list.RootFolder.UniqueContentTypeOrder = list2;
                    list.RootFolder.Update();
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("Failed to change list's default content type: " + ex.Message);
            }
        }

        public string ReorderContentTypes(string sListId, string[] sContentTypes)
        {
            if (sContentTypes.Length < 1)
            {
                return string.Empty;
            }

            string empty;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(sListId)];
                if (sPList == null)
                {
                    throw new ArgumentException("Could not find list to update");
                }

                IList<SPContentType> list = new List<SPContentType>();
                SPContentTypeCollection contentTypes = sPList.ContentTypes;
                for (int i = 0; i < sContentTypes.Length; i++)
                {
                    string name = sContentTypes[i];
                    SPContentType sPContentType = contentTypes[name];
                    if (sPContentType != null && !sPContentType.Sealed)
                    {
                        list.Add(sPContentType);
                    }
                }

                sPList.RootFolder.UniqueContentTypeOrder = list;
                sPList.RootFolder.Update();
                empty = string.Empty;
            }

            return empty;
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPContentTypeCollection contentTypes;
                if (!string.IsNullOrEmpty(sListID))
                {
                    SPList sPList = web.Lists[new Guid(sListID)];
                    contentTypes = sPList.ContentTypes;
                }
                else
                {
                    contentTypes = web.ContentTypes;
                }

                List<SPContentType> list = new List<SPContentType>();
                for (int i = 0; i < contentTypeIDs.Length; i++)
                {
                    string id = contentTypeIDs[i];
                    list.Add(contentTypes[new SPContentTypeId(id)]);
                }

                Dictionary<SPContentType, Exception> dictionary = new Dictionary<SPContentType, Exception>();
                foreach (SPContentType current in list)
                {
                    try
                    {
                        current.Delete();
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        dictionary.Add(current, ex);
                    }
                }

                if (dictionary.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Failed to delete the following content types:\r\n");
                    foreach (KeyValuePair<SPContentType, Exception> current2 in dictionary)
                    {
                        stringBuilder.Append(current2.Key.Name + ": " + current2.Value.Message + "\r\n");
                    }

                    throw new Exception(stringBuilder.ToString());
                }
            }

            return string.Empty;
        }

        private static SPField GetField(SPFieldCollection fields, string fieldName, string Id)
        {
            SPField sPField = null;
            try
            {
                if (fields.ContainsField(fieldName))
                {
                    try
                    {
                        sPField = fields.GetFieldByInternalName(fieldName);
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        sPField = null;
                    }
                }

                if (sPField == null && !string.IsNullOrEmpty(Id))
                {
                    sPField = fields[new Guid(Id)];
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                sPField = null;
            }

            return sPField;
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sAlertXML);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Alert");
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                SPAlertTemplateCollection sPAlertTemplateCollection =
                    new SPAlertTemplateCollection((SPWebService)context.Site.WebApplication.Parent);
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    SPAlert sPAlert = web.Alerts.Add();
                    XmlAttribute xmlAttribute = xmlNode.Attributes["AlertTemplate"];
                    if (xmlAttribute != null && xmlAttribute.Value != null)
                    {
                        sPAlert.AlertTemplate = sPAlertTemplateCollection[xmlAttribute.Value];
                    }

                    string value = xmlNode.Attributes["AlertType"].Value;
                    sPAlert.AlertType = (SPAlertType)Enum.Parse(typeof(SPAlertType), value);
                    if (sPAlert.AlertType == SPAlertType.List)
                    {
                        sPAlert.List = web.Lists[new Guid(xmlNode.Attributes["ListID"].Value)];
                    }

                    if (sPAlert.AlertType == SPAlertType.Item)
                    {
                        SPList sPList = web.Lists[new Guid(xmlNode.Attributes["ListID"].Value)];
                        sPAlert.Item = sPList.GetItemByUniqueId(new Guid(xmlNode.Attributes["ItemGUID"].Value));
                    }

                    sPAlert.Title = xmlNode.Attributes["Title"].Value;
                    sPAlert.Filter = xmlNode.Attributes["Filter"].Value;
                    SPUser sPUser = this.LookupUser(xmlNode.Attributes["User"].Value, web);
                    if (sPUser != null)
                    {
                        sPAlert.User = sPUser;
                    }

                    if (sPAlert.User == null)
                    {
                        xmlTextWriter.WriteElementString("Error", "User can't be added.");
                    }
                    else
                    {
                        string attributeValueAsString = xmlNode.GetAttributeValueAsString("EventType");
                        if (!string.IsNullOrEmpty(attributeValueAsString))
                        {
                            sPAlert.EventType = (SPEventType)Enum.Parse(typeof(SPEventType), attributeValueAsString);
                        }

                        string value2 = xmlNode.Attributes["AlertFrequency"].Value;
                        sPAlert.AlertFrequency = (SPAlertFrequency)Enum.Parse(typeof(SPAlertFrequency), value2);
                        if (sPAlert.AlertFrequency != SPAlertFrequency.Immediate)
                        {
                            sPAlert.AlertTime = OMAdapter.ParseDateString(xmlNode.Attributes["AlertTime"].Value,
                                web.RegionalSettings.TimeZone);
                        }

                        string value3 = xmlNode.Attributes["Status"].Value;
                        sPAlert.Status = (SPAlertStatus)Enum.Parse(typeof(SPAlertStatus), value3);
                        sPAlert.DynamicRecipient = xmlNode.Attributes["DynamicRecipient"].Value;
                        foreach (XmlAttribute xmlAttribute2 in xmlNode.ChildNodes[0].Attributes)
                        {
                            if (xmlAttribute2.LocalName == "eventtypeindex" || xmlAttribute2.LocalName == "filterindex")
                            {
                                sPAlert.Properties.Add(xmlAttribute2.LocalName, xmlAttribute2.Value);
                            }
                        }

                        sPAlert.Update(false);
                        this.FillAlertXML(xmlTextWriter, sPAlert, web.RegionalSettings.TimeZone);
                    }
                }

                result = stringBuilder.ToString();
            }

            return result;
        }

        private static SPViewCollection.SPViewType GetViewTypeValue(string type)
        {
            if (string.Equals(type, SPViewCollection.SPViewType.Calendar.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return (SPViewCollection.SPViewType)532481;
            }

            return (SPViewCollection.SPViewType)Enum.Parse(typeof(SPViewCollection.SPViewType), type, true);
        }

        private static void AddViewsXML(SPList list, XmlNode viewNodeXML, bool bDeletePreExistingViews)
        {
            if (viewNodeXML.ChildNodes.Count == 0)
            {
                return;
            }

            string xml = Utils.CorrectFieldReferencesInListViews(list.Fields.SchemaXml, viewNodeXML.OuterXml);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            viewNodeXML = xmlDocument.DocumentElement;
            if (bDeletePreExistingViews)
            {
                bool flag = false;
                foreach (XmlNode xmlNode in viewNodeXML.ChildNodes)
                {
                    XmlAttribute xmlAttribute = xmlNode.Attributes["DefaultView"];
                    if (xmlAttribute != null && xmlAttribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    XmlNode xmlNode2 = viewNodeXML.ChildNodes[0];
                    XmlAttribute xmlAttribute2 = xmlNode2.OwnerDocument.CreateAttribute("DefaultView");
                    xmlAttribute2.Value = "TRUE";
                    xmlNode2.Attributes.Append(xmlAttribute2);
                }
            }

            List<Guid> list2 = new List<Guid>();
            CaseInvariantFieldCollection caseInvariantFieldCollection = new CaseInvariantFieldCollection(list.Fields);
            foreach (XmlNode xmlNode3 in viewNodeXML.ChildNodes)
            {
                string value = xmlNode3.Attributes["DisplayName"].Value;
                string value2 = xmlNode3.Attributes["Url"].Value;
                string text = value2.Substring(value2.LastIndexOf('/') + 1);
                text = text.Substring(0, text.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase));
                bool bIsDefault;
                if (xmlNode3.Attributes["DefaultView"] != null)
                {
                    bool.TryParse(xmlNode3.Attributes["DefaultView"].Value, out bIsDefault);
                }
                else
                {
                    bIsDefault = false;
                }

                bool bIsPersonal;
                if (xmlNode3.Attributes["Personal"] != null)
                {
                    bool.TryParse(xmlNode3.Attributes["Personal"].Value, out bIsPersonal);
                }
                else
                {
                    bIsPersonal = false;
                }

                XmlNode xmlNode4 = xmlNode3.SelectSingleNode("./Query");
                if (xmlNode4 != null)
                {
                    XmlNode xmlNode5 = xmlNode4.SelectSingleNode("./GroupBy");
                    if (xmlNode5 != null && xmlNode5.ChildNodes.Count == 0)
                    {
                        xmlNode4.RemoveChild(xmlNode5);
                    }

                    XmlNode xmlNode6 = xmlNode4.SelectSingleNode("./OrderBy");
                    if (xmlNode6 != null && xmlNode6.ChildNodes.Count == 0)
                    {
                        xmlNode4.RemoveChild(xmlNode6);
                    }
                }

                string sQuery = (xmlNode4 != null) ? xmlNode4.InnerXml : null;
                XmlNode xmlNode7 = xmlNode3.SelectSingleNode("./RowLimit");
                uint iRowLimit = (xmlNode7 != null) ? Convert.ToUInt32(xmlNode7.InnerText) : 0u;
                bool bPaged = xmlNode7 != null && xmlNode7.Attributes["Paged"] != null &&
                              xmlNode7.Attributes["Paged"].Value == "TRUE";
                XmlNode xmlNode8 = xmlNode3.SelectSingleNode("./Aggregations");
                string text2 = (xmlNode8 != null) ? xmlNode8.InnerXml : null;
                bool bAggregationStatus = text2 != null && xmlNode8.Attributes["Value"] != null &&
                                          xmlNode8.Attributes["Value"].Value == "On";
                XmlNode xmlNode9 = xmlNode3.SelectSingleNode("./CalendarSettings");
                if (xmlNode9 != null)
                {
                    string arg_337_0 = xmlNode9.InnerXml;
                }

                int? iViewStyle = null;
                XmlNode xmlNode10 = xmlNode3.SelectSingleNode("./ViewStyle");
                if (xmlNode10 != null && xmlNode10.Attributes["ID"] != null)
                {
                    iViewStyle = new int?(Convert.ToInt32(xmlNode10.Attributes["ID"].Value));
                }

                SPView sPView = OMAdapter.GetView(list, text);
                if (xmlNode3.Attributes["Type"].Value.Equals("Table", StringComparison.OrdinalIgnoreCase))
                {
                    if (sPView != null)
                    {
                        list2.Add(sPView.ID);
                    }
                }
                else if (string.IsNullOrEmpty(value) || (xmlNode3.Attributes["Hidden"] != null &&
                                                         string.Equals(xmlNode3.Attributes["Hidden"].Value, "true",
                                                             StringComparison.OrdinalIgnoreCase)))
                {
                    if (sPView != null)
                    {
                        list2.Add(sPView.ID);
                    }
                }
                else
                {
                    XmlNodeList xmlNodeList = xmlNode3.SelectNodes("./ViewFields/FieldRef/@Name");
                    StringCollection stringCollection = new StringCollection();
                    foreach (XmlNode xmlNode11 in xmlNodeList)
                    {
                        string text3 = xmlNode11.Value;
                        if (text3 == "Last_x0020_Modified")
                        {
                            text3 = "Modified";
                        }

                        SPField fieldByNames = caseInvariantFieldCollection.GetFieldByNames(null, text3);
                        if (fieldByNames != null)
                        {
                            stringCollection.Add(fieldByNames.InternalName);
                        }
                    }

                    if (sPView == null || (sPView != null && xmlNode3.Attributes["Type"] != null &&
                                           !sPView.Type.Equals(xmlNode3.Attributes["Type"].Value,
                                               StringComparison.OrdinalIgnoreCase)))
                    {
                        if (sPView != null && xmlNode3.Attributes["Type"] != null &&
                            !sPView.Type.Equals(xmlNode3.Attributes["Type"].Value, StringComparison.OrdinalIgnoreCase))
                        {
                            list.Views.Delete(sPView.ID);
                        }

                        string attributeValue = XmlUtility.GetAttributeValue(xmlNode3, "Type");
                        SPViewCollection.SPViewType viewTypeValue = OMAdapter.GetViewTypeValue(attributeValue);
                        SPView sPView2 = OMAdapter.AttemptToAddViewWithRetry(list, text, stringCollection, sQuery,
                            iRowLimit, bPaged, bIsDefault, viewTypeValue, bIsPersonal);
                        if (string.Equals(attributeValue, SPViewCollection.SPViewType.Calendar.ToString(),
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            sPView2.Update();
                            sPView2 = OMAdapter.GetView(list, sPView2.Title);
                        }

                        if (sPView2 != null)
                        {
                            OMAdapter.AddViewSettings(sPView2, value, bIsDefault, sQuery, iRowLimit, bPaged, text2,
                                bAggregationStatus, xmlNode3, iViewStyle);
                            sPView2.Update();
                            sPView = sPView2;
                        }
                    }
                    else if (list.BaseTemplate != SPListTemplateType.DiscussionBoard)
                    {
                        foreach (string current in stringCollection)
                        {
                            if (!OMAdapter.ContainsViewField(sPView, current))
                            {
                                sPView.ViewFields.Add(current);
                            }
                            else
                            {
                                sPView.ViewFields.Delete(current);
                                sPView.ViewFields.Add(current);
                            }
                        }

                        ArrayList arrayList = new ArrayList();
                        foreach (string text4 in ((IEnumerable)sPView.ViewFields))
                        {
                            if (!OMAdapter.ContainsExtraField(text4, stringCollection) &&
                                !sPView.ViewFields.Explicit(text4))
                            {
                                arrayList.Add(text4);
                            }
                        }

                        foreach (string strField in arrayList)
                        {
                            sPView.ViewFields.Delete(strField);
                        }

                        OMAdapter.AddViewSettings(sPView, value, bIsDefault, sQuery, iRowLimit, bPaged, text2,
                            bAggregationStatus, xmlNode3, iViewStyle);
                        sPView.Update();
                    }
                    else if (!value.Equals("Threaded"))
                    {
                        foreach (string current2 in stringCollection)
                        {
                            if (!OMAdapter.ContainsViewField(sPView, current2))
                            {
                                sPView.ViewFields.Add(current2);
                            }
                        }

                        if (!value.Equals("Flat"))
                        {
                            ArrayList arrayList2 = new ArrayList();
                            foreach (string text5 in ((IEnumerable)sPView.ViewFields))
                            {
                                if (!OMAdapter.ContainsExtraField(text5, stringCollection) &&
                                    !sPView.ViewFields.Explicit(text5))
                                {
                                    arrayList2.Add(text5);
                                }
                            }

                            foreach (string strField2 in arrayList2)
                            {
                                sPView.ViewFields.Delete(strField2);
                            }
                        }

                        OMAdapter.AddViewSettings(sPView, value, bIsDefault, sQuery, iRowLimit, bPaged, text2,
                            bAggregationStatus, xmlNode3, iViewStyle);
                        sPView.Update();
                    }
                    else
                    {
                        OMAdapter.UpdateDefaultViewForContentType(sPView, xmlNode3);
                        sPView.Update();
                    }

                    if (sPView != null)
                    {
                        list2.Add(sPView.ID);
                    }
                }
            }

            if (bDeletePreExistingViews)
            {
                List<SPView> list3 = new List<SPView>();
                foreach (SPView sPView3 in ((IEnumerable)list.Views))
                {
                    if (!list2.Contains(sPView3.ID) && !sPView3.Hidden && !string.IsNullOrEmpty(sPView3.Title))
                    {
                        list3.Add(sPView3);
                    }
                }

                foreach (SPView current3 in list3)
                {
                    list.Views.Delete(current3.ID);
                }
            }
        }

        private static SPView AttemptToAddViewWithRetry(SPList list, string sUrlName, StringCollection fieldRefNames,
            string sQuery, uint iRowLimit, bool bPaged, bool bIsDefault, SPViewCollection.SPViewType viewType,
            bool bIsPersonal)
        {
            Func<SPView> codeBlockToRetry = () =>
                list.Views.Add(sUrlName, fieldRefNames, sQuery, iRowLimit, bPaged, bIsDefault, viewType, bIsPersonal);
            return ExecuteWithRetry.AttemptToExecuteWithRetry<SPView>(codeBlockToRetry, null);
        }

        private static void AddViewSettings(SPView view, string sName, bool bIsDefault, string sQuery, uint iRowLimit,
            bool bPaged, string sAggregations, bool bAggregationStatus, XmlNode viewNode, int? iViewStyle)
        {
            view.Title = sName;
            view.DefaultView = bIsDefault;
            view.RowLimit = iRowLimit;
            view.Paged = bPaged;
            view.Query = sQuery;
            view.Aggregations = sAggregations;
            if (bAggregationStatus)
            {
                view.AggregationsStatus = "On";
            }

            if (viewNode.SelectSingleNode("ViewData") != null)
            {
                view.ViewData = viewNode.SelectSingleNode("ViewData").InnerXml;
            }

            if (iViewStyle.HasValue)
            {
                view.ApplyStyle(view.ParentList.ParentWeb.ViewStyles.StyleByID(iViewStyle.Value));
            }

            OMAdapter.AddViewAttributesFromXML(view, viewNode);
        }

        private static void AddViewAttributesFromXML(SPView view, XmlNode viewNode)
        {
            SPViewScope scope = SPViewScope.Default;
            string attributeValueAsString = viewNode.GetAttributeValueAsString("Scope");
            if (!string.IsNullOrEmpty(attributeValueAsString) &&
                Enum.IsDefined(typeof(SPViewScope), attributeValueAsString))
            {
                scope = (SPViewScope)Enum.Parse(typeof(SPViewScope), attributeValueAsString, true);
            }

            view.Scope = scope;
            string text =
                (viewNode.Attributes["ContentTypeID"] != null && viewNode.Attributes["ContentTypeID"].Value != "0x")
                    ? viewNode.Attributes["ContentTypeID"].Value
                    : null;
            if (text != null)
            {
                SPContentTypeId contentTypeId = new SPContentTypeId(text);
                view.ContentTypeId = contentTypeId;
            }

            if (!string.Equals(view.Type, "Grid", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(view.Type, "Gantt", StringComparison.OrdinalIgnoreCase))
            {
                XmlAttribute xmlAttribute = viewNode.Attributes[XmlAttributeNames.MobileView.ToString()];
                XmlAttribute xmlAttribute2 = viewNode.Attributes[XmlAttributeNames.MobileDefaultView.ToString()];
                if (xmlAttribute != null)
                {
                    bool attributeValueAsBoolean =
                        viewNode.Attributes.GetAttributeValueAsBoolean(XmlAttributeNames.MobileView.ToString());
                    if (attributeValueAsBoolean != view.MobileView)
                    {
                        view.MobileView = attributeValueAsBoolean;
                    }
                }

                if (xmlAttribute2 != null)
                {
                    bool attributeValueAsBoolean2 =
                        viewNode.Attributes.GetAttributeValueAsBoolean(XmlAttributeNames.MobileDefaultView.ToString());
                    if (attributeValueAsBoolean2 != view.MobileDefaultView)
                    {
                        view.MobileDefaultView = attributeValueAsBoolean2;
                    }
                }
            }
        }

        private static void UpdateDefaultViewForContentType(SPView view, XmlNode viewNode)
        {
            if (viewNode.Attributes["DefaultViewForContentType"] != null)
            {
                view.DefaultViewForContentType = viewNode.GetAttributeValueAsBoolean("DefaultViewForContentType");
            }
        }

        private static bool ContainsViewField(SPView view, string fieldRefName)
        {
            foreach (string a in ((IEnumerable)view.ViewFields))
            {
                if (a == fieldRefName)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsExtraField(string sViewFieldName, StringCollection fieldRefNames)
        {
            foreach (string current in fieldRefNames)
            {
                if (sViewFieldName == current)
                {
                    return true;
                }
            }

            return false;
        }

        private static SPView GetView(SPList list, string sUrlName)
        {
            if (sUrlName.Trim().EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                sUrlName = sUrlName.Substring(sUrlName.LastIndexOf('/') + 1);
                sUrlName = sUrlName.Substring(0, sUrlName.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase));
            }

            foreach (SPView sPView in ((IEnumerable)list.Views))
            {
                string text = sPView.Url.Substring(sPView.Url.LastIndexOf('/') + 1);
                text = text.Substring(0, text.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase));
                if (text.Equals(sUrlName, StringComparison.OrdinalIgnoreCase))
                {
                    return sPView;
                }
            }

            return null;
        }

        internal void AddFieldsXML(SPFieldCollection fields, XmlNode fieldNodeXML)
        {
            this.AddFieldsXML(fields, fieldNodeXML, false);
        }

        private void AddFieldsXML(SPFieldCollection fields, XmlNode fieldNodeXML, bool bUpdateFieldTypes)
        {
            CaseInvariantFieldCollection invariantFields = new CaseInvariantFieldCollection(fields);
            this.AddFieldsXML(invariantFields, fieldNodeXML, bUpdateFieldTypes);
        }

        private void AddFieldsXML(CaseInvariantFieldCollection invariantFields, XmlNode fieldNodeXML,
            bool updateFieldTypes)
        {
            Queue queue = new Queue();
            if (fieldNodeXML.Name == "Field")
            {
                queue.Enqueue(fieldNodeXML);
            }
            else
            {
                XmlNodeList xmlNodeList = fieldNodeXML.SelectNodes("./Field");
                foreach (XmlNode obj in xmlNodeList)
                {
                    queue.Enqueue(obj);
                }
            }

            Queue queue2 = new Queue();
            Dictionary<XmlNode, SPField> dictionary = new Dictionary<XmlNode, SPField>();
            CaseInvariantFieldCollection caseInvariantFieldCollection =
                new CaseInvariantFieldCollection(invariantFields.Web.AvailableFields);
            int num = 0;
            int num2 = queue.Count * queue.Count;
            if (num2 == 1)
            {
                num2++;
            }

            this.ReorderFieldsByDependency(ref queue);
            this.ReorderFieldsByDependency(ref queue2);
            while (queue.Count > 0 || queue2.Count > 0)
            {
                num++;
                if (num > num2)
                {
                    throw new Exception("A field being added depends on a field that does not exist on the target");
                }

                try
                {
                    XmlNode xmlNode = null;
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    XmlNode languageResourcesNode = null;
                    if (queue.Count > 0)
                    {
                        xmlNode = (XmlNode)queue.Dequeue();
                    }
                    else if (queue2.Count > 0)
                    {
                        xmlNode = (XmlNode)queue2.Dequeue();
                        flag = true;
                    }

                    string attributeValueAsString =
                        xmlNode.GetAttributeValueAsString(FieldNodeAttribute.DisplayName.ToString());
                    string attributeValueAsString2 =
                        xmlNode.GetAttributeValueAsString(FieldNodeAttribute.Type.ToString());
                    bool attributeValueAsBoolean =
                        xmlNode.GetAttributeValueAsBoolean(FieldNodeAttribute.MLSystem.ToString());
                    bool flag4 = base.SharePointVersion.IsSharePoint2007 &&
                                 xmlNode.GetAttributeValueAsString("Type").Equals("BusinessData");
                    bool flag5 = false;
                    if (base.SharePointVersion.IsSharePoint2010 && xmlNode.Attributes["Group"] != null &&
                        xmlNode.Attributes["List"] != null)
                    {
                        flag5 = string.Equals(xmlNode.GetAttributeValueAsString(FieldNodeAttribute.Group.ToString()),
                            "_Hidden", StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrEmpty(attributeValueAsString) && !attributeValueAsString2.In(new string[]
                        {
                            "WorkflowStatus",
                            "Threading"
                        }) && !attributeValueAsBoolean && !flag5)
                    {
                        string text = xmlNode.GetAttributeValueAsString(FieldNodeAttribute.Name.ToString());
                        if (attributeValueAsString2.Equals("Note", StringComparison.InvariantCultureIgnoreCase) &&
                            !string.IsNullOrEmpty(xmlNode.GetAttributeValueAsString("BdcField")) &&
                            invariantFields.List.Fields.ContainsField(text))
                        {
                            SPField fieldByInternalName = invariantFields.List.Fields.GetFieldByInternalName(text);
                            if (fieldByInternalName.Title != attributeValueAsString)
                            {
                                fieldByInternalName.Title = attributeValueAsString;
                                fieldByInternalName.Update();
                            }
                        }
                        else
                        {
                            string attributeValueAsString3 =
                                xmlNode.GetAttributeValueAsString(FieldNodeAttribute.SourceID.ToString());
                            bool attributeValueAsBoolean2 =
                                xmlNode.GetAttributeValueAsBoolean(FieldNodeAttribute.Hidden.ToString());
                            bool attributeValueAsBoolean3 =
                                xmlNode.GetAttributeValueAsBoolean(FieldNodeAttribute.Sealed.ToString());
                            bool attributeValueAsBoolean4 =
                                xmlNode.GetAttributeValueAsBoolean(FieldNodeAttribute.FromTemplate.ToString());
                            bool attributeValueAsBoolean5 =
                                xmlNode.GetAttributeValueAsBoolean(FieldNodeAttribute.IsFromFeature.ToString());
                            SPField sPField = null;
                            if (flag)
                            {
                                if (dictionary.ContainsKey(xmlNode))
                                {
                                    sPField = dictionary[xmlNode];
                                    flag2 = true;
                                }
                            }
                            else
                            {
                                sPField = OMAdapter.TryGetFieldFromCollection(invariantFields, queue2, xmlNode, flag,
                                    attributeValueAsString, text, ref flag2, ref flag3);
                                if (flag3)
                                {
                                    dictionary.Add(xmlNode, sPField);
                                    continue;
                                }
                            }

                            List<string> list = new List<string>();
                            bool flag6 = sPField != null && !flag2;
                            if ((sPField == null || flag6) && text.Length > 32 && invariantFields.List != null &&
                                !Utils.IsGUID(text) &&
                                !Regex.IsMatch(text, "_[0-9a-fA-F]{8}(_[0-9a-fA-F]{4}){3}(_[0-9a-fA-F]{12})") &&
                                !list.Contains(text.ToLower()))
                            {
                                list.Add(text.ToLower());
                            }

                            if (sPField != null)
                            {
                                if (invariantFields.List == null &&
                                    (sPField.InternalName == "FileLeafRef" || attributeValueAsBoolean5))
                                {
                                    continue;
                                }

                                XmlNode xmlNode2 = XmlUtility.StringToXmlNode(sPField.SchemaXml);
                                if (xmlNode2 != null && !OMAdapter.FieldUpdateNecessary(xmlNode, xmlNode2))
                                {
                                    continue;
                                }
                            }

                            if (sPField == null && invariantFields.List == null)
                            {
                                sPField = OMAdapter.TryGetFieldFromCollection(caseInvariantFieldCollection, queue2,
                                    xmlNode, flag, attributeValueAsString, text, ref flag2, ref flag3);
                                flag6 = (sPField != null && !flag2);
                                if (flag3 || flag6)
                                {
                                    dictionary.Add(xmlNode, sPField);
                                    continue;
                                }
                            }

                            if (sPField == null && invariantFields.List != null &&
                                XmlUtility.IsFieldSiteColumn(xmlNode))
                            {
                                SPField sPField2 = null;
                                string attributeValueAsString4 =
                                    xmlNode.GetAttributeValueAsString(FieldNodeAttribute.ID.ToString());
                                if (!string.IsNullOrEmpty(attributeValueAsString4))
                                {
                                    Guid fieldID = new Guid(attributeValueAsString4);
                                    sPField2 = caseInvariantFieldCollection.GetFieldByID(fieldID);
                                }

                                if (sPField2 == null)
                                {
                                    sPField2 = OMAdapter.TryGetFieldFromCollection(caseInvariantFieldCollection, queue2,
                                        xmlNode, flag, attributeValueAsString, text, ref flag2, ref flag3);
                                }

                                if (flag3)
                                {
                                    dictionary.Add(xmlNode, sPField);
                                    continue;
                                }

                                bool flag7 = sPField2 != null && attributeValueAsString2.Equals(sPField2.TypeAsString,
                                    StringComparison.OrdinalIgnoreCase);
                                if (sPField2 != null)
                                {
                                    OMAdapter.CheckIfSourceIdTypesMatch(ref flag7, attributeValueAsString3,
                                        ref sPField2);
                                }

                                if (flag7)
                                {
                                    flag7 = OMAdapter.CheckIfSourceIdsMatch(flag7, attributeValueAsString3, sPField2);
                                }

                                if (flag7)
                                {
                                    text = invariantFields.Add(sPField2);
                                    sPField = invariantFields.GetFieldByInternalName(text);
                                    OMAdapter.SetMissingAttributesInSourceField(xmlNode, sPField);
                                }
                            }

                            bool forceUpdateAsTypesDifferent = sPField != null &&
                                                               !attributeValueAsString2.Equals(sPField.TypeAsString,
                                                                   StringComparison.OrdinalIgnoreCase);
                            bool flag8 = false;
                            if (AddFieldsBusinessLogic.IsAllowedToUpdateCalculatedFieldSchema(sPField,
                                    attributeValueAsString2))
                            {
                                OMAdapter.UpdateCalculatedField(invariantFields, queue, caseInvariantFieldCollection,
                                    xmlNode);
                            }

                            if (sPField == null)
                            {
                                if (flag4 && SPContext.Current == null)
                                {
                                    OMAdapter.SetSharedResourceProviderToUse();
                                }

                                if (list.Count > 0)
                                {
                                    if (list.Contains(text.ToLower()))
                                    {
                                        sPField = OMAdapter.AddFieldWithAutoGeneratedNameToCollection(xmlNode,
                                            invariantFields, caseInvariantFieldCollection, attributeValueAsString,
                                            text);
                                        list.Remove(text.ToLower());
                                        if (flag4)
                                        {
                                            OMAdapter.SetBdcAttributes(xmlNode, sPField);
                                        }
                                    }
                                }
                                else
                                {
                                    flag8 = true;
                                }
                            }
                            else if (sPField.IsUpdatable() && sPField.Type != SPFieldType.Lookup && !flag4)
                            {
                                bool flag9 =
                                    OMAdapter.IsFieldSchemaUpdatable(xmlNode, sPField, forceUpdateAsTypesDifferent);
                                if (flag9 && !attributeValueAsBoolean4 && !flag2)
                                {
                                    this.UpdateFieldFromXML(sPField, xmlNode);
                                }
                                else if (flag2)
                                {
                                    flag8 = true;
                                }
                            }
                            else if (flag2 && !attributeValueAsBoolean2 && !attributeValueAsBoolean3)
                            {
                                flag8 = true;
                            }

                            if (flag8)
                            {
                                sPField = OMAdapter.AddFieldToCollection(xmlNode, invariantFields,
                                    caseInvariantFieldCollection, attributeValueAsString, text);
                                if (flag4)
                                {
                                    OMAdapter.SetBdcAttributes(xmlNode, sPField);
                                }
                            }

                            if (sPField.IsUpdatable() && attributeValueAsString2.In(new string[]
                                {
                                    "Lookup",
                                    "LookupMulti"
                                }))
                            {
                                this.UpdateLookUpField(invariantFields, xmlNode, sPField,
                                    fieldNodeXML.ParentNode.GetAttributeValueAsGuid("ID"));
                            }

                            if (sPField.TypeAsString.In(new string[]
                                {
                                    "User",
                                    "UserMulti"
                                }))
                            {
                                this.UpdateFieldFromXML(sPField, xmlNode);
                            }

                            if (sPField.Title != attributeValueAsString)
                            {
                                OMAdapter.UpdateDisplayName(invariantFields, xmlNode, attributeValueAsString,
                                    attributeValueAsBoolean4, sPField);
                            }

                            this.AddLanguageResourcesForField(invariantFields.Web, invariantFields.List, sPField,
                                xmlNode, languageResourcesNode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        private XmlNode ExtractLanguageResourcesNode(XmlNode fieldNode, XmlNode languageResourcesNode)
        {
            try
            {
                if (fieldNode.ChildNodes != null && fieldNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode xmlNode in fieldNode.ChildNodes)
                    {
                        if (xmlNode.OuterXml.StartsWith("<LanguageResources",
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            languageResourcesNode = xmlNode;
                            break;
                        }
                    }

                    if (languageResourcesNode != null)
                    {
                        fieldNode.RemoveChild(languageResourcesNode);
                    }

                    return languageResourcesNode;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return null;
        }

        private void AddLanguageResourcesForField(SPWeb web, SPList list, SPField targetField, XmlNode fieldNode,
            XmlNode languageResourcesNode)
        {
        }

        private void RetryUpdatingLanguageResourceForField(SPWeb web, SPList list, SPField targetField,
            XmlNode languageResourcesNode)
        {
            try
            {
                SPField sPField = null;
                if (list != null)
                {
                    sPField = list.Fields.GetFieldByInternalName(targetField.InternalName);
                }
                else if (web != null)
                {
                    sPField = web.Fields.GetFieldByInternalName(targetField.InternalName);
                }

                if (sPField != null)
                {
                    this.UpdateFieldResource(sPField, languageResourcesNode);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        private void UpdateFieldResource(SPField targetField, XmlNode languageResourcesNode)
        {
            this.AddLanguageResources(languageResourcesNode, targetField);
            targetField.Update();
        }

        private void ReorderFieldsByDependency(ref Queue fieldXmls)
        {
            if (fieldXmls == null)
            {
                return;
            }

            SortableFieldXmlList sortableFieldXmlList = new SortableFieldXmlList(fieldXmls.Cast<XmlNode>());
            sortableFieldXmlList.SortByDependencies();
            fieldXmls = new Queue(sortableFieldXmlList);
        }

        private static void SetMissingAttributesInSourceField(XmlNode fieldNode, SPField targetField)
        {
            XmlAttribute xmlAttribute = fieldNode.Attributes["SourceID"];
            if (xmlAttribute == null)
            {
                xmlAttribute = fieldNode.OwnerDocument.CreateAttribute("SourceID");
                fieldNode.Attributes.Append(xmlAttribute);
            }

            xmlAttribute.Value = targetField.SourceId;
            XmlAttribute xmlAttribute2 = fieldNode.Attributes["ID"];
            if (xmlAttribute2 == null)
            {
                xmlAttribute2 = fieldNode.OwnerDocument.CreateAttribute("ID");
                fieldNode.Attributes.Append(xmlAttribute2);
            }

            xmlAttribute2.Value = "{" + targetField.Id.ToString() + "}";
        }

        private static bool CheckIfSourceIdsMatch(bool isValidSiteColumn, string sourceId, SPField siteColumn)
        {
            if (!string.IsNullOrEmpty(sourceId) && !string.IsNullOrEmpty(siteColumn.SourceId) &&
                !Utils.IsGuid(sourceId) && !Utils.IsGuid(siteColumn.SourceId))
            {
                isValidSiteColumn = siteColumn.SourceId.Equals(sourceId, StringComparison.OrdinalIgnoreCase);
            }

            return isValidSiteColumn;
        }

        private static void CheckIfSourceIdTypesMatch(ref bool isValidSiteColumn, string sourceId,
            ref SPField siteColumn)
        {
            if (Utils.IsGuid(sourceId) && !Utils.IsGuid(siteColumn.SourceId))
            {
                siteColumn = null;
                isValidSiteColumn = false;
            }
        }

        private static SPField TryGetFieldFromCollection(CaseInvariantFieldCollection invariantFields,
            Queue displayNameMatchedFields, XmlNode fieldNode, bool fieldDrawnFromDisplayNameMatchQueue,
            string displayName, string internalName, ref bool fieldMatchedByDisplayName,
            ref bool addedToDisplayNameQueue)
        {
            SPField sPField = invariantFields.GetFieldByInternalName(internalName);
            if (sPField == null)
            {
                sPField = invariantFields.GetFieldByDisplayName(displayName);
                fieldMatchedByDisplayName = (sPField != null);
                if (fieldMatchedByDisplayName && !fieldDrawnFromDisplayNameMatchQueue)
                {
                    displayNameMatchedFields.Enqueue(fieldNode);
                    addedToDisplayNameQueue = true;
                }
            }

            return sPField;
        }

        private static void UpdateDisplayName(CaseInvariantFieldCollection invariantFields, XmlNode fieldNode,
            string sDisplayName, bool bIsTemplateField, SPField targetField)
        {
            bool flag = fieldNode.Attributes["Type"].Value == targetField.TypeAsString;
            bool flag2 = !bIsTemplateField && (invariantFields.List != null || (!targetField.Sealed &&
                !targetField.Hidden && (!targetField.ReadOnlyField || targetField.Type == SPFieldType.Calculated)));
            if (flag && flag2)
            {
                string title = targetField.Title;
                targetField.Title = sDisplayName;
                targetField.Update();
                invariantFields.UpdateIndexedCollection(title, sDisplayName);
            }
        }

        private void UpdateLookUpField(CaseInvariantFieldCollection invariantFields, XmlNode fieldNode,
            SPField targetField, Guid sourceListId)
        {
            XmlAttribute xmlAttribute = fieldNode.Attributes["TargetListName"];
            XmlAttribute xmlAttribute2 = fieldNode.Attributes["TargetWebName"];
            XmlAttribute xmlAttribute3 = fieldNode.Attributes["WebId"];
            XmlAttribute xmlAttribute4 = fieldNode.Attributes["TargetWebSRURL"];
            Guid guid = fieldNode.GetAttributeValueAsGuid("List");
            bool flag = true;
            if (guid.Equals(sourceListId))
            {
                guid = invariantFields.List.ID;
            }

            if (xmlAttribute != null)
            {
                SPList listByGuid = OMAdapter.GetListByGuid(invariantFields.Web, guid.ToString());
                if (listByGuid != null)
                {
                    string text = listByGuid.ID.ToString();
                    if (!text.StartsWith("{", StringComparison.Ordinal) ||
                        !text.EndsWith("}", StringComparison.Ordinal))
                    {
                        text = "{" + text + "}";
                    }

                    fieldNode.Attributes["List"].Value = text;
                    if (xmlAttribute2 != null && xmlAttribute3 != null)
                    {
                        if (xmlAttribute2.Value.ToLower() == Utils.GetNameFromURL(invariantFields.Web.Url).ToLower())
                        {
                            xmlAttribute3.Value = invariantFields.Web.ID.ToString();
                        }
                        else
                        {
                            flag = false;
                        }

                        fieldNode.Attributes.Remove(xmlAttribute2);
                        if (xmlAttribute4 != null)
                        {
                            fieldNode.Attributes.Remove(xmlAttribute4);
                        }
                    }
                }
                else
                {
                    flag = false;
                }

                fieldNode.Attributes.Remove(xmlAttribute);
                fieldNode.Attributes.Remove(xmlAttribute2);
                fieldNode.Attributes.Remove(xmlAttribute4);
            }

            if (flag)
            {
                this.UpdateFieldFromXML(targetField, fieldNode);
            }
        }

        private static bool IsFieldSchemaUpdatable(XmlNode sourceFieldNode, SPField targetField,
            bool forceUpdateAsTypesDifferent)
        {
            bool flag = targetField.Type.In(new SPFieldType[]
            {
                SPFieldType.Boolean,
                SPFieldType.Currency,
                SPFieldType.DateTime,
                SPFieldType.Number,
                SPFieldType.Text,
                SPFieldType.Choice
            });
            string value = sourceFieldNode.Attributes["Type"].Value;
            bool flag2 = (value.In(new string[]
            {
                "UserMulti"
            }) && targetField.Type == SPFieldType.User) || (value.In(new string[]
            {
                "User"
            }) && targetField.TypeAsString.Equals("UserMulti"));
            bool flag3 = forceUpdateAsTypesDifferent && value.In(new string[]
            {
                "Text"
            }) && targetField.Type == SPFieldType.Choice;
            bool flag4 = forceUpdateAsTypesDifferent && value.In(new string[]
            {
                "Choice"
            });
            bool flag5 = value.Equals(targetField.TypeAsString, StringComparison.OrdinalIgnoreCase);
            bool flag6 = forceUpdateAsTypesDifferent && flag && value.In(new string[]
            {
                "Text",
                "Note"
            });
            return flag2 || flag3 || flag4 || flag5 || flag6;
        }

        private static void UpdateCalculatedField(CaseInvariantFieldCollection invariantFields, Queue fieldsToBeAdded,
            CaseInvariantFieldCollection invariantAvailableFields, XmlNode fieldNode)
        {
            bool flag = true;
            List<XmlNode> dependencies = new List<XmlNode>();
            if (OMAdapter.FieldHasMissingDependencies(invariantFields, invariantAvailableFields, fieldNode,
                    out dependencies))
            {
                if (!OMAdapter.FieldDependenciesInQueue(dependencies, fieldsToBeAdded))
                {
                }

                fieldsToBeAdded.Enqueue(fieldNode);
                flag = false;
            }

            if (flag)
            {
                OMAdapter.UpdateFormulaDefinition(invariantFields, fieldNode);
            }
        }

        private static bool FieldUpdateNecessary(XmlNode sourceField, XmlNode targetField)
        {
            return !XmlUtility.ElementsAreEquivalent((XmlElement)sourceField, (XmlElement)targetField, true,
                new string[]
                {
                    "ID",
                    "Version"
                });
        }

        private static SPField AddFieldToCollection(XmlNode fieldNode, CaseInvariantFieldCollection fields,
            CaseInvariantFieldCollection availableFields, string displayName, string internalName)
        {
            SPField result = null;
            try
            {
                XmlNode xmlNode = fieldNode.Clone();
                if (!displayName.Equals(internalName, StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlNode.Attributes["DisplayName"].Value = internalName;
                }

                internalName =
                    OMAdapter.CreateFieldFromXML(xmlNode, fields, availableFields, displayName, internalName);
                result = fields.GetFieldByInternalName(internalName);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                if (ex.Message == "Index was outside the bounds of the array.")
                {
                    try
                    {
                        fieldNode.Attributes["Name"].Value = displayName;
                        fieldNode.Attributes["DisplayName"].Value = displayName;
                        internalName = OMAdapter.CreateFieldFromXML(fieldNode, fields, availableFields, displayName,
                            internalName);
                        result = fields.GetFieldByDisplayName(displayName);
                        goto IL_F7;
                    }
                    catch (Exception ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        throw new Exception("Cannot add field '" + displayName + "'. " + ex2.Message);
                    }

                    goto IL_DB;
                    IL_F7:
                    return result;
                }

                IL_DB:
                throw new Exception("Cannot add field '" + displayName + "'. " + ex.Message);
            }

            return result;
        }

        private static void SetBdcAttributes(XmlNode fieldNode, SPField addedField)
        {
            string genericSetupPath = SPUtility.GetGenericSetupPath("ISAPI");
            Assembly assembly =
                Assembly.LoadFile(string.Format("{0}\\{1}", genericSetupPath, "microsoft.sharepoint.portal.dll"));
            Type type = assembly.GetType("Microsoft.SharePoint.Portal.WebControls.BusinessDataField");
            object obj = Convert.ChangeType(addedField, type);
            PropertyInfo property = type.GetProperty("BdcFieldName");
            property.SetValue(obj,
                Convert.ChangeType(fieldNode.GetAttributeValueAsString("BdcField"), property.PropertyType), null);
            PropertyInfo property2 = type.GetProperty("SystemInstanceName");
            property2.SetValue(obj,
                Convert.ChangeType(fieldNode.GetAttributeValueAsString("SystemInstance"), property2.PropertyType),
                null);
            PropertyInfo property3 = type.GetProperty("EntityName");
            property3.SetValue(obj,
                Convert.ChangeType(fieldNode.GetAttributeValueAsString("Entity"), property3.PropertyType), null);
            PropertyInfo property4 = type.GetProperty("Profile");
            property4.SetValue(obj,
                Convert.ChangeType(fieldNode.GetAttributeValueAsString("Profile"), property4.PropertyType), null);
            PropertyInfo property5 = type.GetProperty("HasActions");
            property5.SetValue(obj,
                Convert.ChangeType(fieldNode.GetAttributeValueAsBoolean("HasActions"), property5.PropertyType), null);
            SPField sPField = obj as SPField;
            sPField.Update();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sPField.SchemaXml);
            xmlDocument.DocumentElement.SetAttribute("SecondaryFieldBdcNames",
                fieldNode.GetAttributeValueAsString("SecondaryFieldBdcNames"));
            xmlDocument.DocumentElement.SetAttribute("SecondaryFieldWssNames",
                fieldNode.GetAttributeValueAsString("SecondaryFieldWssNames"));
            xmlDocument.DocumentElement.SetAttribute("SecondaryFieldsWssStaticNames",
                fieldNode.GetAttributeValueAsString("SecondaryFieldsWssStaticNames"));
            PropertyInfo property6 = typeof(SPField).GetProperty("SchemaXml");
            property6.SetValue(sPField, xmlDocument.OuterXml, null);
        }

        private static void SetSharedResourceProviderToUse()
        {
            string genericSetupPath = SPUtility.GetGenericSetupPath("ISAPI");
            Assembly assembly =
                Assembly.LoadFile(string.Format("{0}\\{1}", genericSetupPath, "Microsoft.Office.Server.dll"));
            Type type = assembly.GetType("Microsoft.Office.Server.ServerContext");
            FormatterServices.GetUninitializedObject(type);
            PropertyInfo property = type.GetProperty("Default");
            object value = property.GetValue(property, null);
            Assembly assembly2 =
                Assembly.LoadFile(string.Format("{0}\\{1}", genericSetupPath, "microsoft.sharepoint.portal.dll"));
            Type type2 =
                assembly2.GetType("Microsoft.Office.Server.ApplicationRegistry.Infrastructure.SqlSessionProvider");
            FormatterServices.GetUninitializedObject(type2);
            object obj = type2.GetMethod("Instance").Invoke(null, null);
            MethodInfo method = type2.GetMethod("SetSharedResourceProviderToUse", new Type[]
            {
                value.GetType()
            });
            method.Invoke(obj, new object[]
            {
                value
            });
        }

        private static SPField AddFieldWithAutoGeneratedNameToCollection(XmlNode fieldNode,
            CaseInvariantFieldCollection fields, CaseInvariantFieldCollection availableFields, string sDisplayName,
            string sInternalName)
        {
            SPField result = null;
            List<string> list = new List<string>();
            try
            {
                string text = sInternalName.Substring(0, 32);
                int num;
                bool flag = int.TryParse(sInternalName.Substring(32), out num);
                if (flag)
                {
                    string text2 = text;
                    string text3 = text + "MLPlaceHolder";
                    SPField sPField = null;
                    try
                    {
                        SPField arg_69_0;
                        if ((arg_69_0 = fields.GetFieldByInternalName(text2)) == null)
                        {
                            arg_69_0 = (availableFields.GetFieldByInternalName(text2) ??
                                        fields.List.Fields.GetFieldByInternalName(text2));
                        }

                        sPField = arg_69_0;
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        sPField = null;
                    }

                    if (sPField == null)
                    {
                        try
                        {
                            fields.Add(text3, SPFieldType.Text, false);
                            list.Add(text3);
                        }
                        catch (Exception ex2)
                        {
                            OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }

                    for (int i = 0; i < num; i++)
                    {
                        text2 = text + i.ToString();
                        text3 = text + i.ToString() + "MLPlaceHolder";
                        SPField sPField2 = null;
                        try
                        {
                            SPField arg_10D_0;
                            if ((arg_10D_0 = fields.GetFieldByInternalName(text2)) == null)
                            {
                                arg_10D_0 = (availableFields.GetFieldByInternalName(text2) ??
                                             fields.List.Fields.GetFieldByInternalName(text2));
                            }

                            sPField2 = arg_10D_0;
                        }
                        catch (Exception ex3)
                        {
                            OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                            sPField2 = null;
                        }

                        if (sPField2 == null)
                        {
                            try
                            {
                                fields.Add(text3, SPFieldType.Text, false);
                                list.Add(text3);
                            }
                            catch (Exception ex4)
                            {
                                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                            }
                        }
                    }
                }

                result = OMAdapter.AddFieldToCollection(fieldNode, fields, availableFields, sDisplayName,
                    sInternalName);
            }
            catch (Exception ex5)
            {
                OMAdapter.LogExceptionDetails(ex5, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("Cannot add field '" + sDisplayName + "'. " + ex5.Message);
            }
            finally
            {
                for (int j = 0; j < list.Count; j++)
                {
                    fields.Delete(list[j]);
                }
            }

            return result;
        }

        private static string CreateFieldFromXML(XmlNode fieldNode, CaseInvariantFieldCollection fields,
            CaseInvariantFieldCollection availableFields, string sDisplayName, string sInternalName)
        {
            string value = null;
            string text = null;
            string result = null;
            if (fieldNode.Attributes["AddToSiteColumnGroup"] != null &&
                !string.IsNullOrEmpty(fieldNode.Attributes["AddToSiteColumnGroup"].Value))
            {
                value = fieldNode.Attributes["AddToSiteColumnGroup"].Value;
                fieldNode.Attributes.Remove(fieldNode.Attributes["AddToSiteColumnGroup"]);
                if (fieldNode.Attributes["AddToContentType"] != null &&
                    !string.IsNullOrEmpty(fieldNode.Attributes["AddToContentType"].Value))
                {
                    text = fieldNode.Attributes["AddToContentType"].Value;
                    fieldNode.Attributes.Remove(fieldNode.Attributes["AddToContentType"]);
                }
            }

            if (!string.IsNullOrEmpty(value) && fields.List != null)
            {
                SPField sPField = availableFields.GetFieldByNames(sDisplayName, sInternalName);
                if (sPField == null)
                {
                    XmlAttribute xmlAttribute = fieldNode.Attributes["Group"];
                    if (xmlAttribute == null)
                    {
                        xmlAttribute = fieldNode.OwnerDocument.CreateAttribute("Group");
                        fieldNode.Attributes.Append(xmlAttribute);
                    }

                    xmlAttribute.Value = value;
                    result = OMAdapter.AddFieldAsXMLToCollection(fieldNode, availableFields);
                    sPField = fields.List.ParentWeb.Fields[sDisplayName];
                }

                if (sPField != null)
                {
                    fields.Add(sPField);
                    if (!string.IsNullOrEmpty(text))
                    {
                        SPContentType sPContentType = fields.List.ParentWeb.ContentTypes[text];
                        if (sPContentType != null)
                        {
                            SPFieldLink fieldLink = new SPFieldLink(sPField);
                            sPContentType.FieldLinks.Add(fieldLink);
                            sPContentType.Update();
                        }
                    }
                }
            }
            else
            {
                result = OMAdapter.AddFieldAsXMLToCollection(fieldNode, fields);
            }

            return result;
        }

        private static string AddFieldAsXMLToCollection(XmlNode fieldNode, CaseInvariantFieldCollection fields)
        {
            XmlAttribute xmlAttribute = fieldNode.Attributes["Version"];
            if (xmlAttribute != null)
            {
                fieldNode.Attributes.Remove(xmlAttribute);
            }

            XmlAttribute xmlAttribute2 = fieldNode.Attributes["Name"];
            if (xmlAttribute2 == null)
            {
                xmlAttribute2 = fieldNode.Attributes["DisplayName"];
            }

            if (xmlAttribute2 != null)
            {
                xmlAttribute2.Value = Utils.EnsureFieldNameSafety(xmlAttribute2.Value);
            }

            if (fieldNode.Attributes["Viewable"] == null || fieldNode.Attributes["Viewable"].Value.ToUpper() == "TRUE")
            {
                return fields.AddFieldAsXml(fieldNode.OuterXml, true, SPAddFieldOptions.Default);
            }

            return fields.AddFieldAsXml(fieldNode.OuterXml);
        }

        private static bool FieldDependenciesInQueue(Queue fieldQueue, XmlNode calculatedField)
        {
            XmlNodeList xmlNodeList = calculatedField.SelectNodes("./FieldRefs/FieldRef");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string sName = (xmlNode.Attributes["Name"] != null) ? xmlNode.Attributes["Name"].Value : null;
                string sDisplayName = (xmlNode.Attributes["DisplayName"] != null)
                    ? xmlNode.Attributes["DisplayName"].Value
                    : null;
                if (!OMAdapter.FieldInQueueInQueue(fieldQueue, sName, sDisplayName))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldDependenciesInQueue(List<XmlNode> dependencies, Queue fieldQueue)
        {
            foreach (XmlNode current in dependencies)
            {
                string sName = (current.Attributes["Name"] != null) ? current.Attributes["Name"].Value : null;
                string sDisplayName = (current.Attributes["DisplayName"] != null)
                    ? current.Attributes["DisplayName"].Value
                    : null;
                if (!OMAdapter.FieldInQueueInQueue(fieldQueue, sName, sDisplayName))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldInQueueInQueue(Queue fieldQueue, string sName, string sDisplayName)
        {
            foreach (XmlNode xmlNode in fieldQueue)
            {
                if (xmlNode.Attributes["DisplayName"] != null &&
                    xmlNode.Attributes["DisplayName"].Value == sDisplayName)
                {
                    bool result = true;
                    return result;
                }
            }

            foreach (XmlNode xmlNode2 in fieldQueue)
            {
                if (xmlNode2.Attributes["Name"].Value == sName)
                {
                    bool result = true;
                    return result;
                }
            }

            return false;
        }

        private static bool HasFieldReference(XmlNode fieldRef, CaseInvariantFieldCollection fields,
            CaseInvariantFieldCollection availableFields)
        {
            string value = fieldRef.Attributes["Name"].Value;
            string displayName = (fieldRef.Attributes["DisplayName"] != null)
                ? fieldRef.Attributes["DisplayName"].Value
                : null;
            SPField fieldByNames = fields.GetFieldByNames(displayName, value);
            if (fieldByNames == null && fields.List == null)
            {
                fieldByNames = availableFields.GetFieldByNames(displayName, value);
            }

            return fieldByNames != null;
        }

        private static bool FieldHasMissingDependencies(CaseInvariantFieldCollection fields,
            CaseInvariantFieldCollection availableFields, XmlNode calculatedField,
            out List<XmlNode> missingDependencies)
        {
            missingDependencies = new List<XmlNode>();
            XmlNodeList xmlNodeList = calculatedField.SelectNodes("./FieldRefs/FieldRef");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (!OMAdapter.HasFieldReference(xmlNode, fields, availableFields))
                {
                    missingDependencies.Add(xmlNode.Clone());
                }
            }

            return missingDependencies.Count > 0;
        }

        private static void UpdateFormulaDefinition(CaseInvariantFieldCollection fields, XmlNode calculatedField)
        {
            XmlNodeList xmlNodeList = calculatedField.SelectNodes("./FieldRefs/FieldRef");
            Dictionary<string, string> dictionary = new Dictionary<string, string>(xmlNodeList.Count);
            List<string> list = new List<string>(xmlNodeList.Count);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string value = xmlNode.Attributes["Name"].Value;
                if (!dictionary.ContainsKey(value))
                {
                    dictionary.Add(value,
                        (xmlNode.Attributes["DisplayName"] != null) ? xmlNode.Attributes["DisplayName"].Value : null);
                    list.Add(value);
                }
            }

            if (list.Count > 0)
            {
                OMAdapter.UpdateFieldNamesInFormula(fields, calculatedField, list, dictionary);
            }

            bool flag = !fields.Web.Locale.TextInfo.ListSeparator.Equals(",");
            bool flag2 = !fields.Web.Locale.NumberFormat.NumberDecimalSeparator.Equals(".");
            if (flag || flag2)
            {
                string text = calculatedField["Formula"].InnerText;
                string[] array = text.Split(new char[]
                {
                    '"'
                });
                if (array.Length > 1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            stringBuilder.Append(array[i].Replace(",", fields.Web.Locale.TextInfo.ListSeparator)
                                .Replace(".", fields.Web.Locale.NumberFormat.NumberDecimalSeparator));
                        }
                        else
                        {
                            stringBuilder.Append(array[i]);
                        }

                        if (i < array.Length - 1)
                        {
                            stringBuilder.Append("\"");
                        }
                    }

                    text = stringBuilder.ToString();
                }
                else
                {
                    text = text.Replace(",", fields.Web.Locale.TextInfo.ListSeparator);
                }

                calculatedField["Formula"].InnerText = text;
            }
        }

        private static void UpdateFieldNamesInFormula(CaseInvariantFieldCollection fields, XmlNode calculatedField,
            IList<string> fieldNames, IDictionary<string, string> fieldDisplayNames)
        {
            IDictionary<string, string> tokenToFieldNames =
                OMAdapter.ReplaceFieldNamesInFormulaWithTokens(fields, calculatedField, fieldNames, fieldDisplayNames);
            OMAdapter.ReplaceTokensWithFieldNames(calculatedField, tokenToFieldNames);
        }

        private static IDictionary<string, string> ReplaceFieldNamesInFormulaWithTokens(
            CaseInvariantFieldCollection fields, XmlNode calculatedField, IList<string> fieldNames,
            IDictionary<string, string> fieldDisplayNames)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            IEnumerable<string> enumerable = (from name in fieldNames
                orderby name.Length
                select name).Reverse<string>();
            string text = calculatedField["Formula"].InnerText;
            foreach (string current in enumerable)
            {
                string text2 = current;
                string displayName = fieldDisplayNames[text2];
                SPField fieldByNames = fields.GetFieldByNames(displayName, text2);
                string text3 = Guid.NewGuid().ToString("N");
                string value = text2;
                if (fieldByNames != null && text2 != fieldByNames.Title)
                {
                    value = string.Format(OMAdapter.GetBracketResource("FS_Brackets", typeof(SharePointAdapter)),
                        fieldByNames.Title);
                }

                text = text.Replace(text2, text3);
                dictionary.Add(text3, value);
            }

            calculatedField["Formula"].InnerText = text;
            return dictionary;
        }

        private static void ReplaceTokensWithFieldNames(XmlNode calculatedField,
            IDictionary<string, string> tokenToFieldNames)
        {
            string text = calculatedField["Formula"].InnerText;
            foreach (KeyValuePair<string, string> current in tokenToFieldNames)
            {
                text = text.Replace(current.Key, current.Value);
            }

            calculatedField["Formula"].InnerText = text;
        }

        private static string GetBracketResource(string resourceName, Type type)
        {
            if (OMAdapter._brackets == null)
            {
                lock (OMAdapter._bracketLock)
                {
                    if (OMAdapter._brackets == null)
                    {
                        ResourceManager resourceManager =
                            new ResourceManager("Metalogix.SharePoint.Adapters.Properties.Resources", type.Assembly);
                        OMAdapter._brackets = resourceManager.GetString(resourceName);
                    }
                }
            }

            return OMAdapter._brackets;
        }

        private void UpdateFieldFromXML(SPField targetField, XmlNode fieldXML)
        {
            XmlNode targetFieldSchemaXml = XmlUtility.StringToXmlNode(targetField.SchemaXml);
            XmlNode xmlNode = XmlUtility.MergeTargetFieldSchemaXml(targetFieldSchemaXml, fieldXML);
            if (xmlNode.Attributes["Type"].Value == "Note" && targetField.Type != SPFieldType.Note &&
                xmlNode.Attributes["RichText"] != null)
            {
                XmlAttribute node = xmlNode.Attributes["RichText"];
                xmlNode.Attributes.Remove(node);
                targetField.SchemaXml = xmlNode.OuterXml;
                xmlNode.Attributes.Append(node);
                targetField.SchemaXml = xmlNode.OuterXml;
                return;
            }

            targetField.SchemaXml = xmlNode.OuterXml;
        }

        private void DeleteAllListItems(SPListItemCollection items)
        {
            int arg_06_0 = items.Count;
            SPBaseType baseType = items.List.BaseType;
            List<int> list = new List<int>();
            foreach (SPListItem sPListItem in ((IEnumerable)items))
            {
                list.Insert(0, sPListItem.ID);
            }

            foreach (int current in list)
            {
                try
                {
                    try
                    {
                        if (baseType == SPBaseType.DocumentLibrary)
                        {
                            SPListItem itemById = items.GetItemById(current);
                            if (itemById.FileSystemObjectType == SPFileSystemObjectType.File &&
                                itemById.File.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                            {
                                itemById.File.UndoCheckOut();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }

                    items.DeleteItemById(current);
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        private void BatchDeleteAllListItems(SPListItemCollection[] itemCollections)
        {
            try
            {
                if (itemCollections.Length != 0)
                {
                    SPList list = itemCollections[0].List;
                    string listGuid = list.ID.ToString();
                    string text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>";
                    string text2 = "</Batch>";
                    int num = this.CreateDeleteItemBatchCommand(listGuid, 2147483647, "").Length + 256;
                    Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
                    for (int i = 0; i < itemCollections.Length; i++)
                    {
                        SPListItemCollection sPListItemCollection = itemCollections[i];
                        if (sPListItemCollection.Count != 0)
                        {
                            int capacity = text.Length + text2.Length + sPListItemCollection.Count * num;
                            StringBuilder stringBuilder = new StringBuilder(capacity);
                            stringBuilder.Append(text);
                            dictionary.Clear();
                            foreach (SPListItem sPListItem in ((IEnumerable)sPListItemCollection))
                            {
                                try
                                {
                                    try
                                    {
                                        if (list.BaseType == SPBaseType.DocumentLibrary &&
                                            sPListItem.FileSystemObjectType == SPFileSystemObjectType.File &&
                                            sPListItem.File.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                                        {
                                            sPListItem.File.UndoCheckOut();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                    }

                                    string value = this.CreateDeleteItemBatchCommand(listGuid, sPListItem.ID,
                                        sPListItem.File.ServerRelativeUrl);
                                    stringBuilder.Append(value);
                                    dictionary.Add(sPListItem.Web.ServerRelativeUrl.TrimStart(new char[]
                                    {
                                        '/'
                                    }) + '/' + sPListItem.Url, true);
                                }
                                catch (Exception ex2)
                                {
                                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                }
                            }

                            stringBuilder.Append(text2);
                            string xml = list.ParentWeb.ProcessBatchData(stringBuilder.ToString());
                            int num2 = 0;
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(xml);
                            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Result");
                            foreach (XmlNode xmlNode in xmlNodeList)
                            {
                                if (xmlNode.Attributes["Code"] != null && xmlNode.Attributes["Code"].Value.Equals("0"))
                                {
                                    num2++;
                                }
                            }

                            try
                            {
                                SPRecycleBinQuery sPRecycleBinQuery = new SPRecycleBinQuery();
                                sPRecycleBinQuery.OrderBy = SPRecycleBinOrderBy.DeletedDate;
                                sPRecycleBinQuery.IsAscending = false;
                                sPRecycleBinQuery.RowLimit = sPListItemCollection.Count;
                                List<Guid> list2 = new List<Guid>();
                                do
                                {
                                    SPRecycleBinItemCollection recycleBinItems =
                                        list.ParentWeb.GetRecycleBinItems(sPRecycleBinQuery);
                                    foreach (SPRecycleBinItem sPRecycleBinItem in ((IEnumerable)recycleBinItems))
                                    {
                                        string key = sPRecycleBinItem.DirName + '/' + sPRecycleBinItem.LeafName;
                                        if (dictionary.ContainsKey(key))
                                        {
                                            list2.Add(sPRecycleBinItem.ID);
                                        }
                                    }
                                } while (list2.Count < num2 && sPRecycleBinQuery.ItemCollectionPosition != null);

                                list.ParentWeb.RecycleBin.Delete(list2.ToArray());
                            }
                            catch (Exception ex3)
                            {
                                OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex4)
            {
                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        private string CreateDeleteItemBatchCommand(string listGuid, int itemID, string owsfileref)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<Method>");
            stringBuilder.Append("<SetList Scope=\"Request\">" + listGuid + "</SetList>");
            stringBuilder.Append("<SetVar Name=\"ID\">" + itemID + "</SetVar>");
            stringBuilder.Append("<SetVar Name=\"owsfileref\">" + owsfileref + "</SetVar>");
            stringBuilder.Append("<SetVar Name=\"Cmd\">Delete</SetVar>");
            stringBuilder.Append("</Method>");
            return stringBuilder.ToString();
        }

        public static SPField GetFieldByNames(SPFieldCollection fields, string sDisplayName, string sInternalName)
        {
            return OMAdapter.GetFieldByInternalName(fields, sInternalName) ??
                   OMAdapter.GetFieldByDisplayName(fields, sDisplayName);
        }

        private static SPField GetFieldByInternalName(SPFieldCollection fields, string sInternalName)
        {
            if (!string.IsNullOrEmpty(sInternalName))
            {
                string b = sInternalName.ToLower();
                foreach (SPField sPField in ((IEnumerable)fields))
                {
                    if (sPField.InternalName.ToLower() == b)
                    {
                        return sPField;
                    }
                }
            }

            return null;
        }

        private static SPField GetFieldByDisplayName(SPFieldCollection fields, string sDisplayName)
        {
            if (!string.IsNullOrEmpty(sDisplayName))
            {
                foreach (SPField sPField in ((IEnumerable)fields))
                {
                    if (sPField.Title == sDisplayName && sPField.Type != SPFieldType.Computed && !sPField.FromBaseType)
                    {
                        return sPField;
                    }
                }
            }

            return null;
        }

        private static SPField GetFieldById(SPFieldCollection fields, string fieldId)
        {
            SPField result = null;
            try
            {
                if (!string.IsNullOrEmpty(fieldId))
                {
                    result = fields[new Guid(fieldId)];
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private static SPList GetListByGuid(SPWeb web, string sListID)
        {
            foreach (SPList sPList in ((IEnumerable)web.Lists))
            {
                if (sPList.ID.ToString() == sListID)
                {
                    return sPList;
                }
            }

            return null;
        }

        public static SPList GetListByTitle(SPWeb web, string sTitle)
        {
            foreach (SPList sPList in ((IEnumerable)web.Lists))
            {
                try
                {
                    if (sPList.Title == sTitle)
                    {
                        return sPList;
                    }
                }
                catch (SPException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (ex.ErrorCode != -2130575322)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        public static SPList GetListByName(SPWeb web, string sName)
        {
            foreach (SPList sPList in ((IEnumerable)web.Lists))
            {
                try
                {
                    if (sPList.RootFolder.Name == sName)
                    {
                        return sPList;
                    }
                }
                catch (SPException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (ex.ErrorCode != -2130575322)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private void SetRecordDeclarationSettings(SPList list, XmlNode listXml)
        {
        }

        public void SetColumnDefaultSettings(string configurationXml, OperationReporting opResult)
        {
        }

        public string GetWebNavigationStructure()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPNavigationNode navNodeByID = this.GetNavNodeByID(web.Navigation, 0);
                List<string> hiddenGlobalNavUrls = null;
                List<string> hiddenCurrentNavUrls = null;
                this.GetNavNodeHiddenUrls(web, out hiddenGlobalNavUrls, out hiddenCurrentNavUrls);
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.WriteNavigationNodeXML(navNodeByID, xmlWriter, false, false, hiddenGlobalNavUrls,
                    hiddenCurrentNavUrls);
                xmlWriter.Flush();
                result = stringBuilder.ToString();
            }

            return result;
        }

        private void GetNavNodeHiddenGuids(SPWeb web, out Guid[] globalNavHiddenGuids, out Guid[] currentNavHiddenGuids)
        {
            globalNavHiddenGuids = null;
            if (web.AllProperties.ContainsKey("__GlobalNavigationExcludes"))
            {
                string sGuidList = (string)web.AllProperties["__GlobalNavigationExcludes"];
                globalNavHiddenGuids = Utils.SplitWebMetaInfoGuidList(sGuidList);
            }

            currentNavHiddenGuids = null;
            if (web.AllProperties.ContainsKey("__CurrentNavigationExcludes"))
            {
                string sGuidList2 = (string)web.AllProperties["__CurrentNavigationExcludes"];
                currentNavHiddenGuids = Utils.SplitWebMetaInfoGuidList(sGuidList2);
            }
        }

        private void GetNavNodeHiddenUrls(SPWeb web, out List<string> hiddenGlobalNavUrls,
            out List<string> hiddenCurrentNavUrls)
        {
            Guid[] array = null;
            Guid[] array2 = null;
            this.GetNavNodeHiddenGuids(web, out array, out array2);
            hiddenGlobalNavUrls = new List<string>();
            hiddenCurrentNavUrls = new List<string>();
            SPWebCollection subwebsForCurrentUser = web.GetSubwebsForCurrentUser();
            for (int i = 0; i < subwebsForCurrentUser.Count; i++)
            {
                using (SPWeb sPWeb = subwebsForCurrentUser[i])
                {
                    if (array != null)
                    {
                        Guid[] array3 = array;
                        for (int j = 0; j < array3.Length; j++)
                        {
                            Guid a = array3[j];
                            if (a == sPWeb.ID)
                            {
                                StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, sPWeb.Url);
                                hiddenGlobalNavUrls.Add(standardizedUrl.WebRelative);
                                break;
                            }
                        }
                    }

                    if (array2 != null)
                    {
                        Guid[] array4 = array2;
                        for (int k = 0; k < array4.Length; k++)
                        {
                            Guid a2 = array4[k];
                            if (a2 == sPWeb.ID)
                            {
                                StandardizedUrl standardizedUrl2 = StandardizedUrl.StandardizeUrl(this, sPWeb.Url);
                                hiddenCurrentNavUrls.Add(standardizedUrl2.WebRelative);
                                break;
                            }
                        }
                    }
                }
            }

            if ((array == null || array.Length == hiddenGlobalNavUrls.Count) &&
                (array2 == null || array2.Length == hiddenCurrentNavUrls.Count))
            {
                return;
            }

            SPList sPList = null;
            foreach (SPList sPList2 in ((IEnumerable)web.Lists))
            {
                if (sPList2.BaseTemplate == (SPListTemplateType)850)
                {
                    sPList = sPList2;
                    break;
                }
            }

            if (sPList == null)
            {
                return;
            }

            string query = Utils.BuildPagesLibraryGuidFetchingQuery(Utils.GetGuidCollectionUnion(array, array2));
            string viewFields = "<FieldRef Name=\"FileRef\" /><FieldRef Name=\"UniqueId\" />";
            bool flag;
            SPListItemCollection[] listItems =
                this.GetListItems(sPList, query, viewFields, null, null, null, null, null, out flag);
            bool flag2 = false;
            SPListItemCollection[] array5 = listItems;
            for (int l = 0; l < array5.Length; l++)
            {
                SPListItemCollection sPListItemCollection = array5[l];
                if (flag2)
                {
                    return;
                }

                foreach (SPListItem sPListItem in ((IEnumerable)sPListItemCollection))
                {
                    Guid b = (Guid)sPListItem["UniqueId"];
                    string text = sPListItem["FileRef"].ToString();
                    int num = text.IndexOf(";#", StringComparison.Ordinal);
                    if (num >= 0)
                    {
                        num += 2;
                        text = ((num == text.Length - 1) ? null : text.Substring(num));
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        StandardizedUrl standardizedUrl3 = StandardizedUrl.StandardizeUrl(this,
                            "/" + text.TrimStart(new char[]
                            {
                                '/'
                            }));
                        if (array != null)
                        {
                            Guid[] array6 = array;
                            for (int m = 0; m < array6.Length; m++)
                            {
                                Guid a3 = array6[m];
                                if (a3 == b)
                                {
                                    hiddenGlobalNavUrls.Add(standardizedUrl3.WebRelative);
                                    flag2 = true;
                                    break;
                                }
                            }
                        }

                        if (array2 != null)
                        {
                            Guid[] array7 = array2;
                            for (int n = 0; n < array7.Length; n++)
                            {
                                Guid a4 = array7[n];
                                if (a4 == b)
                                {
                                    hiddenCurrentNavUrls.Add(standardizedUrl3.WebRelative);
                                    flag2 = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void WriteNavigationNodeXML(SPNavigationNode node, XmlWriter writer, bool bOnGlobalNav,
            bool bOnCurrentNav, List<string> hiddenGlobalNavUrls, List<string> hiddenCurrentNavUrls)
        {
            string text = node.Url;
            bool flag = true;
            StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, text);
            if (!string.IsNullOrEmpty(standardizedUrl.WebRelative))
            {
                text = standardizedUrl.WebRelative;
                if (bOnGlobalNav && hiddenGlobalNavUrls != null)
                {
                    if (hiddenGlobalNavUrls.Contains(text))
                    {
                        flag = false;
                    }
                }
                else if (bOnCurrentNav && hiddenCurrentNavUrls != null && hiddenCurrentNavUrls.Contains(text))
                {
                    flag = false;
                }
            }

            writer.WriteStartElement("NavNode");
            try
            {
                writer.WriteAttributeString("ID", node.Id.ToString());
                writer.WriteAttributeString("Title", node.Title);
                writer.WriteAttributeString("Url", text);
                writer.WriteAttributeString("IsVisible", flag.ToString());
                writer.WriteAttributeString("IsExternal", node.IsExternal.ToString());
                writer.WriteAttributeString("LastModified",
                    Utils.FormatDate(node.Navigation.Web.RegionalSettings.TimeZone.LocalTimeToUTC(node.LastModified)));
                foreach (object current in node.Properties.Keys)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(current)))
                    {
                        string text2 = current.ToString();
                        if (!text2.Equals("LastModifiedDate", StringComparison.Ordinal) &&
                            !text2.Equals("CreatedDate", StringComparison.Ordinal) &&
                            !text2.Equals("LanguageResources", StringComparison.Ordinal))
                        {
                            object obj = node.Properties[current];
                            string value = (obj == null) ? string.Empty : obj.ToString();
                            writer.WriteAttributeString(text2, value);
                        }
                    }
                }

                foreach (SPNavigationNode node2 in ((IEnumerable)node.Children))
                {
                    this.WriteNavigationNodeXML(node2, writer, bOnGlobalNav || node.Id == 1002,
                        bOnCurrentNav || node.Id == 1025, hiddenGlobalNavUrls, hiddenCurrentNavUrls);
                }
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        private SPNavigationNode GetNavNodeByID(SPNavigation navigation, int navigationId)
        {
            SPNavigationNode result;
            if (navigationId == 0)
            {
                result = (SPNavigationNode)Activator.CreateInstance(typeof(SPNavigationNode),
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new object[]
                    {
                        0,
                        navigation
                    }, null);
            }
            else
            {
                Func<SPNavigationNode> codeBlockToRetry = () => navigation.GetNodeById(navigationId);
                result = ExecuteWithRetry.AttemptToExecuteWithRetry<SPNavigationNode>(codeBlockToRetry, null);
            }

            return result;
        }

        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                try
                {
                    string webNavigationStructure = this.GetWebNavigationStructure();
                    operationReporting.LogInformation("Original Target Nav Structure", webNavigationStructure);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    operationReporting.LogInformation("Error obtaining target original structure", ex.Message);
                }

                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    SPNavigation navigation = web.Navigation;
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(sUpdateXml);
                    XmlNode xmlNode2 = xmlNode.SelectSingleNode("./AdditionsAndUpdates");
                    XmlNode xmlNode3 = xmlNode.SelectSingleNode("./Deletions");
                    if (xmlNode3 != null && xmlNode3.ChildNodes.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        string text = string.Empty;
                        foreach (XmlNode xmlNode4 in xmlNode3.ChildNodes)
                        {
                            try
                            {
                                int num = int.Parse(xmlNode4.Attributes["ID"].Value);
                                if (num > 0)
                                {
                                    SPNavigationNode node = this.GetNavNodeByID(navigation, num);
                                    if (node != null)
                                    {
                                        text = string.Format("Id:{0}, ParentId:{1}, Title:{2}, Url:{3}, IsExternal:{4}",
                                            new object[]
                                            {
                                                node.Id,
                                                node.ParentId,
                                                node.Title,
                                                node.Url,
                                                node.IsExternal
                                            });
                                        Action codeBlockToRetry = delegate { node.Delete(); };
                                        ExecuteWithRetry.AttemptToExecuteWithRetry(codeBlockToRetry, null);
                                        stringBuilder.AppendLine(text);
                                    }
                                }
                            }
                            catch (Exception ex2)
                            {
                                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                operationReporting.LogError(ex2,
                                    string.Format("Failed to delete node: Title:{0}, Id:{1}, TargetNodeDetails [{2}] ",
                                        xmlNode4.GetAttributeValueAsString("Title"),
                                        xmlNode4.GetAttributeValueAsString("ID"), text));
                            }
                        }

                        operationReporting.LogInformation("Target Nav Nodes Deleted", stringBuilder.ToString());
                    }

                    if (xmlNode2 != null && xmlNode2.ChildNodes.Count > 0)
                    {
                        Guid[] array = null;
                        Guid[] array2 = null;
                        this.GetNavNodeHiddenGuids(web, out array, out array2);
                        Dictionary<string, Guid> navNodeUrlToGuidMap = this.GetNavNodeUrlToGuidMap(web);
                        List<Guid> hiddenGlobalNavGuids = (array == null) ? new List<Guid>() : new List<Guid>(array);
                        List<Guid> hiddenCurrentNavGuids = (array2 == null) ? new List<Guid>() : new List<Guid>(array2);
                        foreach (XmlNode nodeXml in xmlNode2.ChildNodes)
                        {
                            this.AddOrUpdateNavNode(nodeXml, navigation, ref hiddenGlobalNavGuids,
                                ref hiddenCurrentNavGuids, navNodeUrlToGuidMap, operationReporting);
                        }

                        StringBuilder stringBuilder2 = new StringBuilder();
                        try
                        {
                            this.UpdateNavigationHiddenGuids(web, hiddenGlobalNavGuids, hiddenCurrentNavGuids,
                                stringBuilder2);
                        }
                        catch (Exception ex3)
                        {
                            OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                            ExceptionUtils.GetExceptionMessage(ex3, stringBuilder2);
                            operationReporting.LogWarning(
                                "UpdateNavigationHiddenGuids() Failed to update hidden ID list.",
                                stringBuilder2.ToString());
                        }
                    }

                    string text2 = string.Empty;
                    text2 = this.GetWebNavigationStructure();
                    if (!operationReporting.HasErrors)
                    {
                        operationReporting.LogObjectXml(text2);
                    }

                    operationReporting.LogInformation("Modified Target Nav Structure", text2);
                }
            }
            catch (Exception ex4)
            {
                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex4, ex4.Message);
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void UpdateNavigationHiddenGuids(SPWeb currentWeb, List<Guid> hiddenGlobalNavGuids,
            List<Guid> hiddenCurrentNavGuids, StringBuilder sbLog)
        {
            string text = "";
            foreach (Guid current in hiddenGlobalNavGuids)
            {
                text = text + current.ToString() + ";";
            }

            string text2 = "";
            foreach (Guid current2 in hiddenCurrentNavGuids)
            {
                text2 = text2 + current2.ToString() + ";";
            }

            sbLog.AppendLine("hiddenGlobalNavGuids: " + text);
            sbLog.AppendLine("hiddenCurrentNavGuids: " + text2);
            if (currentWeb.AllProperties.ContainsKey("__GlobalNavigationExcludes"))
            {
                sbLog.AppendLine("Setting __GlobalNavigationExcludes");
                currentWeb.AllProperties["__GlobalNavigationExcludes"] = text;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                sbLog.AppendLine("Adding __GlobalNavigationExcludes");
                currentWeb.AllProperties.Add("__GlobalNavigationExcludes", text);
            }

            if (currentWeb.AllProperties.ContainsKey("__CurrentNavigationExcludes"))
            {
                sbLog.AppendLine("Setting __CurrentNavigationExcludes");
                currentWeb.AllProperties["__CurrentNavigationExcludes"] = text2;
            }
            else if (!string.IsNullOrEmpty(text2))
            {
                sbLog.AppendLine("Adding __CurrentNavigationExcludes");
                currentWeb.AllProperties.Add("__CurrentNavigationExcludes", text2);
            }

            sbLog.AppendLine("Calling currentWeb.Update");
            currentWeb.Update();
        }

        private void AddOrUpdateNavNode(XmlNode nodeXml, SPNavigation navigation, ref List<Guid> hiddenGlobalNavGuids,
            ref List<Guid> hiddenCurrentNavGuids, Dictionary<string, Guid> urlToGuidMappings,
            OperationReporting operationReporting)
        {
            try
            {
                int navigationId = int.Parse(nodeXml.Attributes["ID"].Value);
                int num = (nodeXml.Attributes["MLOrderIndex"] != null)
                    ? int.Parse(nodeXml.Attributes["MLOrderIndex"].Value)
                    : -1;
                SPNavigationNode sPNavigationNode = this.GetNavNodeByID(navigation, navigationId);
                bool navNodeIsOnQuickLaunch;
                bool navNodeIsOnTopNav;
                if (sPNavigationNode == null)
                {
                    int navigationId2 = int.Parse(nodeXml.Attributes["ParentID"].Value);
                    SPNavigationNode navNodeByID = this.GetNavNodeByID(navigation, navigationId2);
                    if (navNodeByID == null)
                    {
                        throw new Exception("Cannot create node: Invalid parent ID");
                    }

                    navNodeIsOnQuickLaunch = this.GetNavNodeIsOnQuickLaunch(navNodeByID);
                    navNodeIsOnTopNav = this.GetNavNodeIsOnTopNav(navNodeByID);
                    if (navNodeIsOnTopNav && navigation.UseShared)
                    {
                        return;
                    }

                    sPNavigationNode = this.CreateNewNavNode(nodeXml, navNodeByID, ref hiddenGlobalNavGuids,
                        ref hiddenCurrentNavGuids, urlToGuidMappings, navNodeIsOnTopNav, navNodeIsOnQuickLaunch,
                        operationReporting);
                }
                else
                {
                    navNodeIsOnTopNav = this.GetNavNodeIsOnTopNav(sPNavigationNode);
                    navNodeIsOnQuickLaunch = this.GetNavNodeIsOnQuickLaunch(sPNavigationNode);
                    List<Guid> list = null;
                    if (navNodeIsOnTopNav)
                    {
                        list = hiddenGlobalNavGuids;
                    }
                    else if (navNodeIsOnQuickLaunch)
                    {
                        list = hiddenCurrentNavGuids;
                    }

                    this.SetNavNodeProperties(sPNavigationNode, nodeXml, ref list, urlToGuidMappings);
                    if (num >= 0)
                    {
                        SPNavigationNode parent = sPNavigationNode.Parent;
                        if (parent != null)
                        {
                            if (num >= parent.Children.Count)
                            {
                                sPNavigationNode.MoveToLast(parent.Children);
                            }
                            else if (num == 0)
                            {
                                sPNavigationNode.MoveToFirst(parent.Children);
                            }
                            else
                            {
                                SPNavigationNode previousSibling = parent.Children[num - 1];
                                sPNavigationNode.Move(parent.Children, previousSibling);
                            }
                        }
                    }

                    sPNavigationNode.Update();
                }

                if (sPNavigationNode != null)
                {
                    foreach (XmlNode nodeXml2 in nodeXml.ChildNodes)
                    {
                        this.CreateOrUpdateNavChildNode(sPNavigationNode, nodeXml2, ref hiddenGlobalNavGuids,
                            ref hiddenCurrentNavGuids, urlToGuidMappings, navNodeIsOnTopNav, navNodeIsOnQuickLaunch,
                            operationReporting);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex,
                    string.Format("AddOrUpdateNavNode() node: Title:{0}, ID:{1}",
                        nodeXml.GetAttributeValueAsString("Title"), nodeXml.GetAttributeValueAsString("ID")));
            }
        }

        private void CreateOrUpdateNavChildNode(SPNavigationNode parentNode, XmlNode nodeXml,
            ref List<Guid> hiddenGlobalNavGuids, ref List<Guid> hiddenCurrentNavGuids,
            Dictionary<string, Guid> urlToGuidMappings, bool bNavNodeIsOnTopNavBar, bool bNavNodeIsOnQuickLaunch,
            OperationReporting operationReporting)
        {
            try
            {
                SPNavigationNode sPNavigationNode = null;
                int num = int.Parse(nodeXml.Attributes["ID"].Value);
                if (num >= 0)
                {
                    foreach (SPNavigationNode sPNavigationNode2 in ((IEnumerable)parentNode.Children))
                    {
                        if (sPNavigationNode2.Id == num)
                        {
                            sPNavigationNode = sPNavigationNode2;
                            break;
                        }
                    }
                }

                if (sPNavigationNode == null)
                {
                    sPNavigationNode = this.CreateNewNavNode(nodeXml, parentNode, ref hiddenGlobalNavGuids,
                        ref hiddenCurrentNavGuids, urlToGuidMappings, bNavNodeIsOnTopNavBar, bNavNodeIsOnQuickLaunch,
                        operationReporting);
                }
                else
                {
                    List<Guid> list = null;
                    if (bNavNodeIsOnTopNavBar)
                    {
                        list = hiddenGlobalNavGuids;
                    }
                    else if (bNavNodeIsOnQuickLaunch)
                    {
                        list = hiddenCurrentNavGuids;
                    }

                    this.SetNavNodeProperties(sPNavigationNode, nodeXml, ref list, urlToGuidMappings);
                    sPNavigationNode.Update();
                }

                if (sPNavigationNode != null)
                {
                    foreach (XmlNode nodeXml2 in nodeXml.ChildNodes)
                    {
                        this.CreateOrUpdateNavChildNode(sPNavigationNode, nodeXml2, ref hiddenGlobalNavGuids,
                            ref hiddenCurrentNavGuids, urlToGuidMappings, bNavNodeIsOnTopNavBar,
                            bNavNodeIsOnQuickLaunch, operationReporting);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex,
                    string.Format("CreateOrUpdateNavChildNode() node: Title:{0}, ID:{1}",
                        nodeXml.GetAttributeValueAsString("Title"), nodeXml.GetAttributeValueAsString("ID")));
            }
        }

        private bool GetNavNodeIsOnQuickLaunch(SPNavigationNode node)
        {
            return node.Id == 1025 || (node.Parent != null && this.GetNavNodeIsOnQuickLaunch(node.Parent));
        }

        private bool GetNavNodeIsOnTopNav(SPNavigationNode node)
        {
            return node.Id == 1002 || (node.Parent != null && this.GetNavNodeIsOnTopNav(node.Parent));
        }

        private SPNavigationNode CreateNewNavNode(XmlNode nodeXml, SPNavigationNode parentNode,
            ref List<Guid> hiddenGlobalNavGuids, ref List<Guid> hiddenCurrentNavGuids,
            Dictionary<string, Guid> urlToGuidMappings, bool bNavNodeIsOnTopNavBar, bool bNavNodeIsOnQuickLaunch,
            OperationReporting operationReporting)
        {
            string value = nodeXml.Attributes["Title"].Value;
            string text = HttpUtility.UrlDecode(nodeXml.Attributes["Url"].Value);
            bool isExternal = (nodeXml.Attributes["IsExternal"] == null)
                ? text.Contains("://")
                : bool.Parse(nodeXml.Attributes["IsExternal"].Value);
            int num = (nodeXml.Attributes["MLOrderIndex"] != null)
                ? int.Parse(nodeXml.Attributes["MLOrderIndex"].Value)
                : -1;
            SPNavigationNode sPNavigationNode = null;
            try
            {
                sPNavigationNode = new SPNavigationNode(value, text, isExternal);
                try
                {
                    if (num < 0 || num >= parentNode.Children.Count)
                    {
                        sPNavigationNode = parentNode.Children.AddAsLast(sPNavigationNode);
                    }
                    else if (num == 0)
                    {
                        sPNavigationNode = parentNode.Children.AddAsFirst(sPNavigationNode);
                    }
                    else
                    {
                        SPNavigationNode previousNode = parentNode.Children[num - 1];
                        sPNavigationNode = parentNode.Children.Add(sPNavigationNode, previousNode);
                    }
                }
                catch (SPException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    bool flag = false;
                    if (ex.ErrorCode == 2130242547 && !string.IsNullOrEmpty(text))
                    {
                        sPNavigationNode = parentNode.Navigation.GetNodeByUrl(text);
                        if (sPNavigationNode != null)
                        {
                            flag = true;
                            operationReporting.LogWarning(
                                "Found existing node by Url. Using that to prevent duplicate entry", string.Format(
                                    "existing node found: Id:{0}, ParentId:{1}, Title:{2}, Url:{3}, IsExternal:{4}",
                                    new object[]
                                    {
                                        sPNavigationNode.Id,
                                        sPNavigationNode.ParentId,
                                        sPNavigationNode.Title,
                                        sPNavigationNode.Url,
                                        sPNavigationNode.IsExternal
                                    }) + Environment.NewLine + string.Format(
                                    "original node: Title:{0}, Url:{1}, IsExternal:{2}, MLOrderIndex:{3}", new object[]
                                    {
                                        value,
                                        text,
                                        isExternal.ToString(),
                                        num.ToString()
                                    }));
                        }
                    }

                    if (!flag)
                    {
                        throw;
                    }
                }

                List<Guid> list = null;
                if (bNavNodeIsOnTopNavBar)
                {
                    list = hiddenGlobalNavGuids;
                }
                else if (bNavNodeIsOnQuickLaunch)
                {
                    list = hiddenCurrentNavGuids;
                }

                this.SetNavNodeProperties(sPNavigationNode, nodeXml, ref list, urlToGuidMappings);
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                if (ex2.Message.IndexOf("Cannot open", StringComparison.OrdinalIgnoreCase) != -1 &&
                    ex2.Message.IndexOf("no such", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    ExceptionUtils.GetExceptionMessage(ex2, stringBuilder);
                    operationReporting.LogWarning(string.Format(
                        "Unable to add a node as the URI does not exist: Title:{0}, Url:{1}, IsExternal:{2}, MLOrderIndex:{3}",
                        new object[]
                        {
                            value,
                            text,
                            isExternal.ToString(),
                            num.ToString()
                        }), stringBuilder.ToString());
                }
                else
                {
                    operationReporting.LogError(ex2,
                        string.Format("CreateNewNavNode() node: Title:{0}, Url:{1}, IsExternal:{2}, MLOrderIndex:{3}",
                            new object[]
                            {
                                value,
                                text,
                                isExternal.ToString(),
                                num.ToString()
                            }));
                }
            }

            return sPNavigationNode;
        }

        private void SetNavNodeProperties(SPNavigationNode node, XmlNode nodeXml, ref List<Guid> hiddenNavGuids,
            Dictionary<string, Guid> urlToGuidMappings)
        {
            if (nodeXml.Attributes["Title"] != null)
            {
                node.Title = nodeXml.Attributes["Title"].Value;
            }

            if (nodeXml.Attributes["Url"] != null)
            {
                node.Url = HttpUtility.UrlDecode(nodeXml.Attributes["Url"].Value);
            }

            if (hiddenNavGuids != null && nodeXml.Attributes["IsVisible"] != null)
            {
                StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, node.Url);
                if (!string.IsNullOrEmpty(standardizedUrl.WebRelative) &&
                    urlToGuidMappings.ContainsKey(standardizedUrl.WebRelative))
                {
                    Guid item = urlToGuidMappings[standardizedUrl.WebRelative];
                    if (bool.Parse(nodeXml.Attributes["IsVisible"].Value))
                    {
                        hiddenNavGuids.Remove(item);
                    }
                    else if (!hiddenNavGuids.Contains(item))
                    {
                        hiddenNavGuids.Add(item);
                    }
                }
            }

            foreach (XmlAttribute xmlAttribute in nodeXml.Attributes)
            {
                if (xmlAttribute == null || (!xmlAttribute.Name.Equals("Title", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("Url", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("IsExternal", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("IsVisible", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("ID", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("ParentID", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("LastModified", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("MLOrderIndex", StringComparison.Ordinal) &&
                                             !xmlAttribute.Name.Equals("LanguageResources", StringComparison.Ordinal)))
                {
                    bool flag = false;
                    foreach (object current in node.Properties.Keys)
                    {
                        string a = (current == null) ? null : current.ToString();
                        if (a == xmlAttribute.Name)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (xmlAttribute.Value.StartsWith("SW|", StringComparison.InvariantCulture))
                    {
                        xmlAttribute.Value = xmlAttribute.Value.Remove(0, 3);
                    }

                    if (flag)
                    {
                        node.Properties[xmlAttribute.Name] = xmlAttribute.Value;
                    }
                    else
                    {
                        node.Properties.Add(xmlAttribute.Name, xmlAttribute.Value);
                    }
                }
            }

            node.Update();
        }

        private Dictionary<string, Guid> GetNavNodeUrlToGuidMap(SPWeb web)
        {
            Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>();
            SPWebCollection subwebsForCurrentUser = web.GetSubwebsForCurrentUser();
            for (int i = 0; i < subwebsForCurrentUser.Count; i++)
            {
                using (SPWeb sPWeb = subwebsForCurrentUser[i])
                {
                    StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, sPWeb.Url);
                    dictionary.Add(standardizedUrl.WebRelative, sPWeb.ID);
                }
            }

            SPList sPList = null;
            foreach (SPList sPList2 in ((IEnumerable)web.Lists))
            {
                if (sPList2.BaseTemplate == (SPListTemplateType)850)
                {
                    sPList = sPList2;
                    break;
                }
            }

            if (sPList == null)
            {
                return dictionary;
            }

            string query = Utils.BuildPagesLibraryGuidFetchingQuery(null);
            string viewFields = "<FieldRef Name=\"FileRef\" /><FieldRef Name=\"UniqueId\" />";
            bool flag;
            SPListItemCollection[] listItems =
                this.GetListItems(sPList, query, viewFields, null, null, null, null, null, out flag);
            SPListItemCollection[] array = listItems;
            for (int j = 0; j < array.Length; j++)
            {
                SPListItemCollection sPListItemCollection = array[j];
                foreach (SPListItem sPListItem in ((IEnumerable)sPListItemCollection))
                {
                    Guid value = (Guid)sPListItem["UniqueId"];
                    string text = sPListItem["FileRef"].ToString();
                    int num = text.IndexOf(";#", StringComparison.Ordinal);
                    if (num >= 0)
                    {
                        num += 2;
                        text = ((num == text.Length - 1) ? null : text.Substring(num));
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        StandardizedUrl standardizedUrl2 = StandardizedUrl.StandardizeUrl(this,
                            "/" + text.TrimStart(new char[]
                            {
                                '/'
                            }));
                        dictionary.Add(standardizedUrl2.WebRelative, value);
                    }
                }
            }

            return dictionary;
        }

        private static void LoadPublishingDLL()
        {
            OMAdapter.s_publishingType = typeof(PublishingPage);
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            if (OMAdapter.SupportsPublishing && !base.SharePointVersion.IsSharePoint2010)
            {
                this.DefaultPageVersionCorrector(sListID, sListItemXML);
            }

            return string.Empty;
        }

        private void DefaultPageVersionCorrector(string sListID, string sListItemXML)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(sListID)];
                PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(web);
                if (sPList.ItemCount == 1)
                {
                    PageLayout[] availablePageLayouts = publishingWeb.GetAvailablePageLayouts();
                    bool flag;
                    SPListItemCollection[] fullListItemCollectionsNoMetaData =
                        this.GetFullListItemCollectionsNoMetaData(sPList, out flag);
                    PublishingPage publishingPage = null;
                    SPListItemCollection[] array = fullListItemCollectionsNoMetaData;
                    for (int i = 0; i < array.Length; i++)
                    {
                        SPListItemCollection sPListItemCollection = array[i];
                        if (sPListItemCollection.Count > 0)
                        {
                            publishingPage = PublishingPage.GetPublishingPage(sPListItemCollection[0]);
                            break;
                        }
                    }

                    PublishingPage publishingPage2 = publishingWeb.GetPublishingPages()
                        .Add(sPList.ID.ToString() + ".aspx", availablePageLayouts[0]);
                    publishingWeb.DefaultPage = (web.GetFile(publishingPage2.Url));
                    publishingWeb.Update();
                    publishingPage.ListItem.Delete();
                }
                else
                {
                    PublishingPageCollection publishingPages = publishingWeb.GetPublishingPages();
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
                    for (int j = 0; j < publishingPages.Count; j++)
                    {
                        if (publishingPages[j].Name == sPList.ID.ToString() + ".aspx" &&
                            xmlNode.Attributes["FileLeafRef"] != null)
                        {
                            publishingWeb.DefaultPage =
                                (web.GetFile("pages/" + xmlNode.Attributes["FileLeafRef"].Value));
                            publishingWeb.Update();
                            publishingPages[j].ListItem.Delete();
                        }
                    }
                }
            }
        }

        private string GetWelcomePage(SPWeb web)
        {
            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(web);
            string result = null;
            try
            {
                result = publishingWeb.DefaultPage.Url;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private void WelcomePageSetting(string sWelcomePage)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPFile file = web.GetFile(sWelcomePage);
                if (PublishingWeb.IsPublishingWeb(web))
                {
                    PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(web);
                    if (!(publishingWeb.PagesListId == Guid.Empty))
                    {
                        Guid arg_40_0 = publishingWeb.PagesListId;
                        if (publishingWeb.PagesList != null)
                        {
                            goto IL_B4;
                        }
                    }

                    SPList sPList = null;
                    foreach (SPList sPList2 in ((IEnumerable)web.Lists))
                    {
                        if (sPList2.BaseTemplate == (SPListTemplateType)OMAdapter.PAGES_LIBRARY_TEMPLATE_ID)
                        {
                            sPList = sPList2;
                            break;
                        }
                    }

                    if (sPList != null)
                    {
                        publishingWeb.PagesListId = (sPList.ID);
                        publishingWeb.Update();
                    }

                    IL_B4:
                    if (file.Exists)
                    {
                        publishingWeb.DefaultPage = (file);
                        publishingWeb.Update();
                    }
                }
            }
        }

        public bool IsPublishingPage(SPListItem item)
        {
            return item != null &&
                   item.ParentList.BaseTemplate == (SPListTemplateType)OMAdapter.PAGES_LIBRARY_TEMPLATE_ID;
        }

        private static void SetPublishingWebNavigationSettings(SPWeb web, XmlNode webXml, bool? showPagesInCurrentNav,
            bool? showSubSitesInCurrentNav, bool? showPagesInGlobalNav, bool? showSubSitesInGlobalNav)
        {
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            string sDomain = "";
            string text = "";
            string text2 = "";
            string text3 = "";
            string[] array = sUserIdentifier.Split(new char[]
            {
                '\\'
            });
            string sUserIdentifier2;
            if (array.Length == 2)
            {
                sDomain = array[0];
                sUserIdentifier2 = array[1];
            }
            else
            {
                sUserIdentifier2 = sUserIdentifier;
            }

            return string.Concat(new string[]
            {
                "<UserValidation IsValid=\"",
                this.GetUserInformation(sDomain, sUserIdentifier2, out text, out text2, out text3, bCanBeDomainGroup)
                    .ToString(),
                "\" FullName=\"",
                text3,
                "\" />"
            });
        }

        internal bool GetUserInformation(string sDomain, string sUserIdentifier, out string sUserLogin,
            out string sEmail, out string sName, bool bCanBeDomainGroup)
        {
            DirectoryEntry directoryEntry = null;
            DirectoryEntry directoryEntry2 = null;
            DirectorySearcher directorySearcher = null;
            SearchResult searchResult = null;
            bool result = false;
            sUserLogin = "";
            sEmail = "";
            sName = "";
            try
            {
                directoryEntry = ((sDomain.Length > 0) ? new DirectoryEntry("LDAP://" + sDomain) : null);
                try
                {
                    searchResult = directorySearcher.FindOne();
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    searchResult = null;
                }

                if (searchResult != null)
                {
                    directoryEntry2 = searchResult.GetDirectoryEntry();
                    sUserLogin = sUserIdentifier;
                    sEmail = ((directoryEntry2.Properties["mail"].Value != null)
                        ? directoryEntry2.Properties["mail"].Value.ToString()
                        : "");
                    sName = ((directoryEntry2.Properties["name"].Value != null)
                        ? directoryEntry2.Properties["name"].Value.ToString()
                        : sUserIdentifier);
                    result = true;
                }
                else
                {
                    try
                    {
                        searchResult = directorySearcher.FindOne();
                    }
                    catch (Exception ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        searchResult = null;
                    }

                    if (searchResult != null)
                    {
                        directoryEntry2 = searchResult.GetDirectoryEntry();
                        sUserLogin = directoryEntry2.Properties["samaccountname"].Value.ToString();
                        sEmail = ((directoryEntry2.Properties["mail"].Value != null)
                            ? directoryEntry2.Properties["mail"].Value.ToString()
                            : "");
                        sName = sUserIdentifier;
                        result = true;
                    }
                    else
                    {
                        directoryEntry = ((sDomain.Length > 0)
                            ? new DirectoryEntry("WinNT://" + sDomain)
                            : new DirectoryEntry("WinNT://" + Environment.MachineName));
                        int num = 0;
                        foreach (DirectoryEntry directoryEntry3 in directoryEntry.Children)
                        {
                            num++;
                            if (directoryEntry3.SchemaClassName == "User" || bCanBeDomainGroup)
                            {
                                if (sUserIdentifier == directoryEntry3.Name)
                                {
                                    sUserLogin = sUserIdentifier;
                                    sEmail = "";
                                    sName = ((directoryEntry3.Properties["FullName"].Value != null)
                                        ? directoryEntry3.Properties["FullName"].Value.ToString()
                                        : sUserIdentifier);
                                    result = true;
                                    break;
                                }

                                if (directoryEntry3.Properties["FullName"].Value != null && sUserIdentifier ==
                                    directoryEntry3.Properties["FullName"].Value.ToString())
                                {
                                    sUserLogin = directoryEntry3.Name;
                                    sEmail = "";
                                    sName = sUserIdentifier;
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (directoryEntry != null)
                {
                    directoryEntry.Dispose();
                    directoryEntry = null;
                }

                if (directoryEntry2 != null)
                {
                    directoryEntry2.Dispose();
                    directoryEntry2 = null;
                }

                if (directorySearcher != null)
                {
                    directorySearcher.Dispose();
                    directorySearcher = null;
                }

                if (searchResult != null)
                {
                    searchResult = null;
                }
            }

            return result;
        }

        private static void LoadUserProfileDLL()
        {
            OMAdapter.userProfileManager = typeof(UserProfileManager);
        }

        public string GetWebApplications()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement xmlElement = xmlDocument.CreateElement("WebApplicationCollection");
            SPWebService value = SPFarm.Local.Services.GetValue<SPWebService>("");
            string innerXml;
            using (Context context = this.GetContext())
            {
                string text = null;
                if (OMAdapter.SupportsUserProfile)
                {
                    text = OMAdapter.GetMySiteUrl(context);
                }

                foreach (SPWebApplication current in value.WebApplications)
                {
                    XmlElement xmlElement2 = xmlDocument.CreateElement("WebApplication");
                    if (current.Name == context.Site.WebApplication.Name)
                    {
                        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("IsDefault");
                        xmlAttribute.Value = true.ToString();
                        xmlElement2.Attributes.Append(xmlAttribute);
                    }

                    XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("Name");
                    xmlAttribute2.Value = current.Name;
                    xmlElement2.Attributes.Append(xmlAttribute2);
                    XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute("URL");
                    string value2 = current.AlternateUrls.GetResponseUrl(context.Site.Zone, true).Uri.ToString();
                    xmlAttribute3.Value = value2;
                    xmlElement2.Attributes.Append(xmlAttribute3);
                    XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute("MaximumFileSize");
                    xmlAttribute4.Value = current.MaximumFileSize.ToString();
                    xmlElement2.Attributes.Append(xmlAttribute4);
                    if (OMAdapter.SupportsUserProfile && text != null)
                    {
                        bool flag = !string.IsNullOrEmpty(text) && text.Equals(value2);
                        if (flag)
                        {
                            XmlAttribute xmlAttribute5 = xmlDocument.CreateAttribute("IsMySitePortal");
                            xmlAttribute5.Value = flag.ToString();
                            xmlElement2.Attributes.Append(xmlAttribute5);
                        }
                    }

                    foreach (SPPrefix sPPrefix in ((IEnumerable)current.Prefixes))
                    {
                        if (sPPrefix.Name.Length >= 0)
                        {
                            XmlElement xmlElement3 = xmlDocument.CreateElement("Path");
                            XmlAttribute xmlAttribute6 = xmlDocument.CreateAttribute("IsWildcard");
                            xmlAttribute6.Value = (sPPrefix.PrefixType == SPPrefixType.WildcardInclusion).ToString();
                            if (string.IsNullOrEmpty(sPPrefix.Name))
                            {
                                xmlElement3.InnerText = "/";
                            }
                            else
                            {
                                xmlElement3.InnerText = sPPrefix.Name;
                            }

                            xmlElement3.Attributes.Append(xmlAttribute6);
                            xmlElement2.AppendChild(xmlElement3);
                        }
                    }

                    foreach (SPContentDatabase current2 in current.ContentDatabases)
                    {
                        if (!string.IsNullOrEmpty(current2.Name) &&
                            current2.CurrentSiteCount < current2.MaximumSiteCount)
                        {
                            XmlElement xmlElement4 = xmlDocument.CreateElement("ContentDatabase");
                            XmlAttribute xmlAttribute7 = xmlDocument.CreateAttribute("Server");
                            XmlAttribute xmlAttribute8 = xmlDocument.CreateAttribute("Name");
                            XmlAttribute xmlAttribute9 = xmlDocument.CreateAttribute("DisplayName");
                            XmlAttribute xmlAttribute10 = xmlDocument.CreateAttribute("DiskSizeRequired");
                            XmlAttribute xmlAttribute11 = xmlDocument.CreateAttribute("CurrentSiteCount");
                            XmlAttribute xmlAttribute12 = xmlDocument.CreateAttribute("IsReadOnly");
                            XmlAttribute xmlAttribute13 = xmlDocument.CreateAttribute("Status");
                            xmlAttribute7.Value = current2.Server;
                            xmlAttribute8.Value = current2.Name;
                            xmlAttribute9.Value = current2.DisplayName;
                            xmlAttribute10.Value = current2.DiskSizeRequired.ToString();
                            xmlAttribute11.Value = current2.CurrentSiteCount.ToString();
                            xmlAttribute12.Value = current2.IsReadOnly.ToString();
                            xmlAttribute13.Value = current2.Status.ToString();
                            xmlElement4.Attributes.Append(xmlAttribute7);
                            xmlElement4.Attributes.Append(xmlAttribute8);
                            xmlElement4.Attributes.Append(xmlAttribute9);
                            xmlElement4.Attributes.Append(xmlAttribute10);
                            xmlElement4.Attributes.Append(xmlAttribute11);
                            xmlElement4.Attributes.Append(xmlAttribute12);
                            xmlElement4.Attributes.Append(xmlAttribute13);
                            xmlElement2.AppendChild(xmlElement4);
                        }
                    }

                    xmlElement.AppendChild(xmlElement2);
                }

                xmlDocument.AppendChild(xmlElement);
                innerXml = xmlDocument.InnerXml;
            }

            return innerXml;
        }

        private static string GetMySiteUrl(Context ctx)
        {
            string result;
            try
            {
                UserProfileManager userProfileManager = new UserProfileManager(ServerContext.GetContext(ctx.Site));
                result = userProfileManager.MySiteHostUrl;
            }
            catch (UserProfileException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }
            catch (NullReferenceException ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                if (!(ex2.Source == "Microsoft.Office.Server.UserProfiles"))
                {
                    throw;
                }

                result = null;
            }

            return result;
        }

        public string GetLanguagesAndWebTemplates()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("LanguageCollection");
            using (Context context = this.GetContext())
            {
                foreach (SPLanguage sPLanguage in ((IEnumerable)context.Web.RegionalSettings.InstalledLanguages))
                {
                    xmlTextWriter.WriteStartElement("Language");
                    xmlTextWriter.WriteAttributeString("Name", sPLanguage.DisplayName);
                    xmlTextWriter.WriteAttributeString("LCID", sPLanguage.LCID.ToString());
                    this.GetWebTemplateXml(xmlTextWriter, context.Site, Convert.ToUInt32(sPLanguage.LCID), true);
                    xmlTextWriter.WriteEndElement();
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        public string GetSiteQuotaTemplates()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("SiteQuotaTemplates");
            try
            {
                foreach (SPQuotaTemplate sPQuotaTemplate in ((IEnumerable)SPWebService.ContentService.QuotaTemplates))
                {
                    xmlTextWriter.WriteStartElement("SiteQuotaTemplate");
                    xmlTextWriter.WriteAttributeString("QuotaID", sPQuotaTemplate.QuotaID.ToString());
                    xmlTextWriter.WriteAttributeString("QuotaName", sPQuotaTemplate.Name);
                    xmlTextWriter.WriteAttributeString("QuotaStorageLimit",
                        sPQuotaTemplate.StorageMaximumLevel.ToString());
                    xmlTextWriter.WriteAttributeString("QuoteStorageWarning",
                        sPQuotaTemplate.StorageWarningLevel.ToString());
                    xmlTextWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public string GetUserFromProfile()
        {
            string result;
            using (Context context = this.GetContext())
            {
                using (SPWeb sPWeb = context.Site.OpenWeb(context.Site.ServerRelativeUrl))
                {
                    try
                    {
                        string g = sPWeb.Properties[
                            "urn:schemas-microsoft-com:sharepoint:portal:profile:UserProfile_GUID"];
                        UserProfileManager userProfileManager =
                            new UserProfileManager(ServerContext.GetContext(context.Site));
                        UserProfile userProfile = userProfileManager.GetUserProfile(new Guid(g));
                        object obj = userProfile["UserName"];
                        result = obj.ToString();
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        result = null;
                    }
                }
            }

            return result;
        }

        private Microsoft.Office.Server.UserProfiles.UserProfile GetUserProfile(string userName, string siteUrl,
            UserProfileManager profileManager, StringBuilder sbErrors)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            if (string.IsNullOrEmpty(siteUrl))
            {
                throw new ArgumentNullException("siteUrl");
            }

            if (profileManager == null)
            {
                throw new ArgumentNullException("profileManager");
            }

            StringBuilder stringBuilder = new StringBuilder();
            UserProfile userProfile = null;
            try
            {
                try
                {
                    userProfile = profileManager.GetUserProfile(userName);
                }
                catch (UserNotFoundException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (!base.SharePointVersion.IsSharePoint2010OrLater)
                    {
                        throw;
                    }

                    userProfile = null;
                    stringBuilder.AppendLine("EXCEPTION Details:");
                    stringBuilder.AppendLine(ex.GetType().ToString());
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine(ex.StackTrace);
                }

                if (base.SharePointVersion.IsSharePoint2010OrLater && userProfile == null)
                {
                    string text = this.ConvertUserNameToClaimsFormat(userName, siteUrl);
                    stringBuilder.AppendLine(string.Format("Obtain UserProfile using Claims Format for user '{0}'",
                        text));
                    userProfile = profileManager.GetUserProfile(text);
                }
            }
            finally
            {
                if (sbErrors != null && base.SharePointVersion.IsSharePoint2010OrLater && userProfile == null &&
                    stringBuilder.Length > 0)
                {
                    sbErrors.AppendLine(stringBuilder.ToString());
                }
            }

            return userProfile;
        }

        public string GetUserProfiles(string sSiteURL, string sLoginName, out string sErrors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("GetUserProfiles siteURL='{0}', loginName='{1}'", sSiteURL,
                sLoginName));
            string result;
            using (SPSite sPSite = new SPSite(sSiteURL))
            {
                try
                {
                    stringBuilder.AppendLine(string.Format("Obtain UserProfileManager for site '{0}'", sPSite.Url));
                    UserProfileManager profileManager = new UserProfileManager(ServerContext.GetContext(sPSite));
                    stringBuilder.AppendLine(string.Format("Obtain UserProfile for user '{0}'", sLoginName));
                    UserProfile userProfile = this.GetUserProfile(sLoginName, sSiteURL, profileManager, stringBuilder);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder2));
                    xmlTextWriter.WriteStartElement("UserProfile");
                    Type typeFromHandle = typeof(DateTime);
                    xmlTextWriter.WriteStartElement("Sections");
                    stringBuilder.AppendLine("Obtain PropertiesWithSection");
                    foreach (Property property in userProfile.ProfileManager.PropertiesWithSection)
                    {
                        if (property.IsSection)
                        {
                            xmlTextWriter.WriteStartElement(property.Name);
                            xmlTextWriter.WriteAttributeString("DisplayName",
                                (property.DisplayName == null) ? string.Empty : property.DisplayName);
                            xmlTextWriter.WriteEndElement();
                        }
                    }

                    xmlTextWriter.WriteEndElement();
                    stringBuilder.AppendLine("Obtain Properties");
                    foreach (Property property2 in userProfile.ProfileManager.Properties)
                    {
                        if (userProfile[property2.Name] != null)
                        {
                            UserProfileValueCollection userProfileValueCollection = userProfile[property2.Name];
                            if (userProfileValueCollection.Count > 0)
                            {
                                xmlTextWriter.WriteStartElement(property2.Name);
                                xmlTextWriter.WriteAttributeString("Type",
                                    (property2.Type == null) ? string.Empty : property2.Type);
                                xmlTextWriter.WriteAttributeString("DisplayName",
                                    (property2.DisplayName == null) ? string.Empty : property2.DisplayName);
                                xmlTextWriter.WriteAttributeString("Description",
                                    (property2.Description == null) ? string.Empty : property2.Description);
                                xmlTextWriter.WriteAttributeString("Length", property2.Length.ToString());
                                xmlTextWriter.WriteAttributeString("DefaultPrivacy",
                                    property2.DefaultPrivacy.ToString());
                                xmlTextWriter.WriteAttributeString("PrivacyPolicy", property2.PrivacyPolicy.ToString());
                                xmlTextWriter.WriteAttributeString("IsSystem", property2.IsSystem.ToString());
                                xmlTextWriter.WriteAttributeString("IsSection", property2.IsSection.ToString());
                                xmlTextWriter.WriteAttributeString("IsAlias", property2.IsAlias.ToString());
                                xmlTextWriter.WriteAttributeString("IsColleagueEventLog",
                                    property2.IsColleagueEventLog.ToString());
                                xmlTextWriter.WriteAttributeString("IsMultivalued", property2.IsMultivalued.ToString());
                                xmlTextWriter.WriteAttributeString("IsReplicable", property2.IsReplicable.ToString());
                                xmlTextWriter.WriteAttributeString("IsSearchable", property2.IsSearchable.ToString());
                                xmlTextWriter.WriteAttributeString("Separator", property2.Separator.ToString());
                                xmlTextWriter.WriteAttributeString("IsAdminEditable",
                                    property2.IsAdminEditable.ToString());
                                xmlTextWriter.WriteAttributeString("IsUserEditable",
                                    property2.IsUserEditable.ToString());
                                xmlTextWriter.WriteAttributeString("IsVisibleOnEditor",
                                    property2.IsVisibleOnEditor.ToString());
                                xmlTextWriter.WriteAttributeString("IsVisibleOnViewer",
                                    property2.IsVisibleOnViewer.ToString());
                                xmlTextWriter.WriteAttributeString("MaximumShown", property2.MaximumShown.ToString());
                                xmlTextWriter.WriteAttributeString("UserOverridePrivacy",
                                    property2.UserOverridePrivacy.ToString());
                                if (property2.ChoiceType != null)
                                {
                                    try
                                    {
                                        if (property2.ChoiceList != null)
                                        {
                                            string[] allTerms = property2.ChoiceList.GetAllTerms(true);
                                            xmlTextWriter.WriteStartElement("ChoiceList");
                                            xmlTextWriter.WriteAttributeString("ChoiceType",
                                                property2.ChoiceType.ToString());
                                            string[] array = allTerms;
                                            for (int i = 0; i < array.Length; i++)
                                            {
                                                string value = array[i];
                                                xmlTextWriter.WriteElementString("Value", value);
                                            }

                                            xmlTextWriter.WriteEndElement();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                    }
                                }

                                foreach (object current in userProfileValueCollection)
                                {
                                    string value2;
                                    if (typeFromHandle.IsAssignableFrom(current.GetType()))
                                    {
                                        value2 = Utils.FormatDate((DateTime)current);
                                    }
                                    else if (property2.Type.Equals("timezone",
                                                 StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        SPTimeZone sPTimeZone = (SPTimeZone)current;
                                        value2 = sPTimeZone.ID.ToString();
                                    }
                                    else
                                    {
                                        value2 = ((current == null) ? "" : current.ToString());
                                    }

                                    xmlTextWriter.WriteElementString("Value", value2);
                                }

                                xmlTextWriter.WriteEndElement();
                            }
                        }
                    }

                    xmlTextWriter.WriteStartElement("QuickLinks");
                    stringBuilder.AppendLine("Obtain QuickLinks");
                    QuickLink[] items = userProfile.QuickLinks.GetItems();
                    for (int j = 0; j < items.Length; j++)
                    {
                        QuickLink quickLink = items[j];
                        xmlTextWriter.WriteStartElement("QuickLink");
                        xmlTextWriter.WriteAttributeString("Title", quickLink.Title);
                        xmlTextWriter.WriteAttributeString("Url", quickLink.Url);
                        xmlTextWriter.WriteAttributeString("GroupType", quickLink.GroupType.ToString());
                        xmlTextWriter.WriteAttributeString("Group", quickLink.Group);
                        xmlTextWriter.WriteAttributeString("PrivacyLevel", quickLink.PrivacyLevel.ToString());
                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.Flush();
                    sErrors = string.Empty;
                    result = stringBuilder2.ToString();
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    stringBuilder.AppendLine("EXCEPTION Details:");
                    stringBuilder.AppendLine(ex2.Message);
                    stringBuilder.AppendLine(string.Format("Type : {0}", ex2.GetType().ToString()));
                    stringBuilder.AppendLine(ex2.StackTrace);
                    sErrors = string.Format("Failed to get user profile: {0}{1}{2}", ex2.Message, Environment.NewLine,
                        stringBuilder.ToString());
                    result = null;
                }
            }

            return result;
        }

        public string GetAudiences()
        {
            string result;
            try
            {
                result = this.GetAudiencesInternal();
            }
            catch (FileNotFoundException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private string GetAudiencesInternal()
        {
            string result;
            using (Context context = this.GetContext())
            {
                ServerContext context2 = ServerContext.GetContext(context.Site);
                if (context2 == null)
                {
                    result = null;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    AudienceManager audienceManager = new AudienceManager(context2);
                    xmlTextWriter.WriteStartElement("AudienceCollection");
                    foreach (Audience audience in audienceManager.Audiences)
                    {
                        this.WriteAudienceXml(audience, xmlTextWriter);
                    }

                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.Flush();
                    result = stringBuilder.ToString();
                }
            }

            return result;
        }

        private void WriteAudienceXml(Audience audience, XmlWriter writer)
        {
            writer.WriteStartElement("Audience");
            writer.WriteAttributeString("ID", audience.AudienceID.ToString());
            writer.WriteAttributeString("Name", audience.AudienceName);
            writer.WriteAttributeString("Description", audience.AudienceDescription);
            writer.WriteAttributeString("Site", audience.AudienceSite);
            writer.WriteAttributeString("GroupOperation", audience.GroupOperation.ToString());
            writer.WriteAttributeString("Owner", audience.OwnerAccountName);
            writer.WriteAttributeString("MemberShipCount", audience.MemberShipCount.ToString());
            if (audience.AudienceRules != null)
            {
                foreach (object current in audience.AudienceRules)
                {
                    AudienceRuleComponent audienceRuleComponent = (AudienceRuleComponent)current;
                    writer.WriteStartElement("Rule");
                    writer.WriteAttributeString("LeftContent", audienceRuleComponent.LeftContent);
                    writer.WriteAttributeString("Operator", audienceRuleComponent.Operator);
                    writer.WriteAttributeString("RightContent", audienceRuleComponent.RightContent);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        public string GetMySiteData(string sSiteURL)
        {
            SPSite sPSite = null;
            SPSite sPSite2 = null;
            string result;
            try
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("MySiteData");
                sPSite = new SPSite(sSiteURL);
                UserProfileManager userProfileManager = new UserProfileManager(ServerContext.GetContext(sPSite));
                try
                {
                    sPSite2 = new SPSite(userProfileManager.MySiteHostUrl);
                    xmlTextWriter.WriteAttributeString("MySiteHostUrl", sPSite2.WebApplication.Name.ToString());
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }

                xmlTextWriter.WriteAttributeString("PersonalSiteFormat",
                    userProfileManager.PersonalSiteFormat.ToString());
                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                result = ex2.Message;
            }
            finally
            {
                if (sPSite2 != null)
                {
                    sPSite2.Dispose();
                }

                if (sPSite != null)
                {
                    sPSite.Dispose();
                }
            }

            return result;
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string result;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNode = null;
                if (sPropertyXml != null)
                {
                    xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sPropertyXml);
                    xmlNode = xmlDocument.DocumentElement;
                }

                SPSite sPSite = null;
                SPSite sPSite2 = new SPSite(sSiteURL);
                UserProfileManager userProfileManager = new UserProfileManager(ServerContext.GetContext(sPSite2));
                UserProfile userProfile = null;
                try
                {
                    userProfile = userProfileManager.GetUserProfile(sLoginName);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (!(ex is UserNotFoundException) || !bCreateIfNotFound)
                    {
                        result = null;
                        return result;
                    }

                    userProfile = userProfileManager.CreateUserProfile(sLoginName);
                }

                if (userProfile == null)
                {
                    stringBuilder.AppendLine(string.Format(
                        "SetUserProfile -> profile is null, unable to obtain user profile for user '{0}'", sLoginName));
                }

                if (xmlNode == null)
                {
                    stringBuilder.AppendLine("SetUserProfile -> properties (xml) is null.");
                }

                if (xmlNode != null)
                {
                    this.UpdateProfileProperties2007(xmlNode, userProfile, stringBuilder);
                }

                try
                {
                    QuickLinkManager quickLinks = userProfile.QuickLinks;
                    foreach (XmlNode xmlNode2 in xmlNode.SelectNodes(".//QuickLink"))
                    {
                        try
                        {
                            quickLinks.Create(xmlNode2.Attributes["Title"].Value, xmlNode2.Attributes["Url"].Value,
                                (QuickLinkGroupType)Enum.Parse(typeof(QuickLinkGroupType),
                                    xmlNode2.Attributes["GroupType"].Value), xmlNode2.Attributes["Group"].Value,
                                (Privacy)Enum.Parse(typeof(Privacy), xmlNode2.Attributes["PrivacyLevel"].Value));
                        }
                        catch (Exception ex2)
                        {
                            OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                    stringBuilder.AppendLine("Error setting the QuickLinks settings with error: " + ex3.Message);
                }

                userProfile.Commit();
                SPWeb sPWeb = null;
                try
                {
                    sPSite2.CatchAccessDeniedException = false;
                    sPSite2.AllowUnsafeUpdates = true;
                    sPWeb = sPSite2.OpenWeb(sPSite2.ServerRelativeUrl);
                    sPWeb.AllowUnsafeUpdates = true;
                    Type type = userProfile.GetType();
                    try
                    {
                        if (base.SharePointVersion.IsSharePoint2007)
                        {
                            FieldInfo field = type.GetField("m_serverContext",
                                BindingFlags.Instance | BindingFlags.NonPublic);
                            object value = field.GetValue(userProfile);
                            Type type2 = ((ServerContext)value).GetType();
                            PropertyInfo property = type2.GetProperty("MySitePortalUrl",
                                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                            object value2 = property.GetValue(value, null);
                            char[] trimChars = new char[]
                            {
                                '/'
                            };
                            string requestUrl = value2.ToString().TrimEnd(trimChars);
                            sPSite = new SPSite(requestUrl);
                            sPSite2.PortalUrl = sPSite.PortalUrl;
                            sPSite2.PortalName = sPSite.PortalName;
                            sPWeb.Locale = sPSite.RootWeb.Locale;
                        }
                    }
                    catch (Exception ex4)
                    {
                        OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                        stringBuilder.AppendLine("Error setting the profile Portal Url. : " + ex4.Message);
                    }

                    try
                    {
                        SPFarm local = SPFarm.Local;
                        SPWebService value3 = local.Services.GetValue<SPWebService>("");
                        SPQuotaTemplate quota = value3.QuotaTemplates["Personal Site"];
                        sPSite2.Quota = quota;
                    }
                    catch (Exception ex5)
                    {
                        OMAdapter.LogExceptionDetails(ex5, MethodBase.GetCurrentMethod().Name, null);
                        stringBuilder.AppendLine("Error setting the MySite quota. : " + ex5.Message);
                    }

                    if (base.SharePointVersion.IsSharePoint2007 || base.SharePointVersion.IsSharePoint2010)
                    {
                        MethodInfo method = type.GetMethod("UpdatePersonalSiteUrl",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        if (method != null)
                        {
                            string[] array = new string[]
                            {
                                sPSite2.ServerRelativeUrl
                            };
                            method.Invoke(userProfile, (object[])array);
                        }
                        else
                        {
                            stringBuilder.AppendLine(string.Format(
                                "Unable to link profile to MySite, UpdatePersonalSiteUrl method not found",
                                new object[0]));
                        }
                    }
                    else if (base.SharePointVersion.IsSharePoint2013OrLater)
                    {
                        MethodInfo method2 = type.GetMethod("AssociatePersonalSiteUrlAndActivateMySiteCapabilities",
                            BindingFlags.Static | BindingFlags.NonPublic);
                        if (method2 != null)
                        {
                            method2.Invoke(null, new object[]
                            {
                                sPSite2,
                                userProfile
                            });
                        }
                        else
                        {
                            stringBuilder.AppendLine(string.Format(
                                "Unable to link profile to MySite, AssociatePersonalSiteUrlAndActivateMySiteCapabilities method not found",
                                new object[0]));
                        }
                    }

                    type = userProfile.GetType();
                    FieldInfo field2 = type.GetField("m_Guid", BindingFlags.Instance | BindingFlags.NonPublic);
                    object value4 = field2.GetValue(userProfile);
                    sPWeb.Properties["noindex"] = "enumerate";
                    sPWeb.Properties["urn:schemas-microsoft-com:sharepoint:portal:profile:UserProfile_GUID"] =
                        value4.ToString();
                    sPWeb.Properties.Update();
                    sPSite2.AllowUnsafeUpdates = false;
                    sPWeb.Update();
                }
                catch (Exception ex6)
                {
                    OMAdapter.LogExceptionDetails(ex6, MethodBase.GetCurrentMethod().Name, null);
                    stringBuilder.AppendLine(
                        "Failed to Set User profile properties [Quota or Profile Linking] with message: " +
                        ex6.Message);
                }
                finally
                {
                    if (sPWeb != null)
                    {
                        sPWeb.Dispose();
                        sPWeb = null;
                    }

                    if (sPSite2 != null)
                    {
                        sPSite2.Dispose();
                        sPSite2 = null;
                    }

                    if (sPSite != null)
                    {
                        sPSite.Dispose();
                        sPSite = null;
                    }
                }

                result = stringBuilder.ToString();
            }
            catch (Exception ex7)
            {
                OMAdapter.LogExceptionDetails(ex7, MethodBase.GetCurrentMethod().Name, null);
                stringBuilder.AppendLine("Failed to set User Profile properties with message: " + ex7.Message);
                result = stringBuilder.ToString();
            }

            return result;
        }

        private void UpdateProfileProperties2007(XmlNode properties, UserProfile profile, StringBuilder sbErrors)
        {
            XmlNode xmlNode = properties.SelectSingleNode("Sections");
            if (xmlNode != null)
            {
                foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                {
                    try
                    {
                        Property sectionByName =
                            profile.ProfileManager.PropertiesWithSection.GetSectionByName(xmlNode2.Name);
                        if (sectionByName == null)
                        {
                            Property property = profile.ProfileManager.PropertiesWithSection.Create(true);
                            property.Name = (xmlNode2.Name);
                            property.DisplayName = (string.IsNullOrEmpty(xmlNode2.Attributes["DisplayName"].Value)
                                ? xmlNode2.Name
                                : xmlNode2.Attributes["DisplayName"].Value);
                            profile.ProfileManager.PropertiesWithSection.Add(property);
                            sectionByName =
                                profile.ProfileManager.PropertiesWithSection.GetSectionByName(xmlNode2.Name);
                        }

                        if (sectionByName != null)
                        {
                            bool flag = false;
                            string value = xmlNode2.Attributes["DisplayName"].Value;
                            if (sectionByName.DisplayName != value)
                            {
                                sectionByName.DisplayName = (value);
                                flag = true;
                            }

                            if (flag)
                            {
                                sectionByName.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }
                }
            }

            foreach (XmlNode xmlNode3 in properties.ChildNodes)
            {
                if (!(xmlNode3.Name == "QuickLinks") && xmlNode3.Attributes.Count != 0)
                {
                    try
                    {
                        Property propertyByName = profile.ProfileManager.Properties.GetPropertyByName(xmlNode3.Name);
                        XmlNode xmlNode4 = xmlNode3.SelectSingleNode("ChoiceList");
                        if (propertyByName == null)
                        {
                            if (xmlNode3.Attributes["IsSystem"] != null &&
                                !bool.Parse(xmlNode3.Attributes["IsSystem"].Value))
                            {
                                Property property2 = profile.ProfileManager.Properties.Create(false);
                                property2.Name = (xmlNode3.Name);
                                property2.Type = (xmlNode3.Attributes["Type"].Value);
                                property2.IsMultivalued = (bool.Parse(xmlNode3.Attributes["IsMultivalued"].Value));
                                property2.Length = (int.Parse(xmlNode3.Attributes["Length"].Value));
                                property2.DisplayName = (string.IsNullOrEmpty(xmlNode3.Attributes["DisplayName"].Value)
                                    ? xmlNode3.Name
                                    : xmlNode3.Attributes["DisplayName"].Value);
                                this.PopulateUserProfileProperty2007(xmlNode3, property2);
                                if (xmlNode4 != null)
                                {
                                    ChoiceTypes choiceType = (ChoiceTypes)Enum.Parse(typeof(ChoiceTypes),
                                        xmlNode4.Attributes["ChoiceType"].Value);
                                    property2.ChoiceType = (choiceType);
                                    if (property2.ChoiceType != null)
                                    {
                                        foreach (XmlNode xmlNode5 in xmlNode4.ChildNodes)
                                        {
                                            try
                                            {
                                                string[] array = xmlNode5.InnerText.Split(new char[]
                                                {
                                                    ';'
                                                }, StringSplitOptions.RemoveEmptyEntries);
                                                string[] array2 = array;
                                                for (int i = 0; i < array2.Length; i++)
                                                {
                                                    string text = array2[i];
                                                    if (property2.ChoiceList.FindTerms(text, 0).Length == 0)
                                                    {
                                                        property2.ChoiceList.Add(text);
                                                    }
                                                }
                                            }
                                            catch (Exception ex2)
                                            {
                                                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name,
                                                    null);
                                                sbErrors.AppendLine(string.Format(
                                                    "Unable to add parent value to Choice List for user profile property '{0}' : {1}",
                                                    xmlNode3.Name, ex2.Message));
                                            }
                                        }
                                    }
                                }

                                profile.ProfileManager.Properties.Add(property2);
                                propertyByName = profile.ProfileManager.Properties.GetPropertyByName(xmlNode3.Name);
                            }
                        }
                        else
                        {
                            this.PopulateUserProfileProperty2007(xmlNode3, propertyByName);
                            propertyByName.Commit();
                        }

                        if (propertyByName != null && xmlNode3.ChildNodes.Count > 0 && profile[xmlNode3.Name] != null)
                        {
                            UserProfileValueCollection userProfileValueCollection = profile[xmlNode3.Name];
                            XmlNodeList xmlNodeList = xmlNode3.SelectNodes("Value");
                            if (xmlNodeList != null)
                            {
                                foreach (XmlNode xmlNode6 in xmlNodeList)
                                {
                                    string innerText = xmlNode6.InnerText;
                                    try
                                    {
                                        object obj = this.CastToUserProfileValue(innerText, propertyByName.Type);
                                        bool flag2 = true;
                                        foreach (object current in userProfileValueCollection)
                                        {
                                            if (current.Equals(obj))
                                            {
                                                flag2 = false;
                                                break;
                                            }
                                        }

                                        if (flag2)
                                        {
                                            if (!propertyByName.IsMultivalued && !propertyByName.IsSystem)
                                            {
                                                profile[xmlNode3.Name].Clear();
                                            }

                                            profile[xmlNode3.Name].Add(obj);
                                            profile.Commit();
                                        }
                                    }
                                    catch (Exception ex3)
                                    {
                                        OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                                        sbErrors.AppendLine(string.Format(
                                            "Problem adding value '{0}' to user profile property '{1}' : {2}",
                                            innerText, xmlNode3.Name, ex3.Message));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex4)
                    {
                        OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                        sbErrors.AppendLine(string.Format("Problem with user profile property '{0}' : {1}",
                            xmlNode3.Name, ex4.Message));
                    }
                }
            }
        }

        public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
        {
            string result;
            try
            {
                result = this.AddOrUpdateAudienceInternal(sAudienceXml, options);
            }
            catch (FileNotFoundException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private string AddOrUpdateAudienceInternal(string sAudienceXml, AddAudienceOptions options)
        {
            string result;
            using (Context context = this.GetContext())
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sAudienceXml);
                XmlNode documentElement = xmlDocument.DocumentElement;
                ServerContext context2 = ServerContext.GetContext(context.Site);
                AudienceManager audienceManager = new AudienceManager(context2);
                Audience audience = audienceManager.GetAudience(documentElement.Attributes["Name"].Value);
                if (audience != null && options.Overwrite)
                {
                    audienceManager.Audiences.Remove(audience.AudienceID);
                    audience = null;
                }

                if (audience == null)
                {
                    audience = audienceManager.Audiences.Create(documentElement.Attributes["Name"].Value,
                        documentElement.Attributes["Description"].Value);
                }

                string value;
                if ((value = documentElement.Attributes["GroupOperation"].Value) != null)
                {
                    AudienceGroupOperation audienceGroupOperation;
                    if (!(value == "AUDIENCE_AND_OPERATION"))
                    {
                        if (!(value == "AUDIENCE_MIX_OPERATION"))
                        {
                            if (!(value == "AUDIENCE_NOGROUP_OPERATION"))
                            {
                                if (!(value == "AUDIENCE_OR_OPERATION"))
                                {
                                    goto IL_116;
                                }

                                audienceGroupOperation = (AudienceGroupOperation)1;
                            }
                            else
                            {
                                audienceGroupOperation = 0;
                            }
                        }
                        else
                        {
                            audienceGroupOperation = (AudienceGroupOperation)3;
                        }
                    }
                    else
                    {
                        audienceGroupOperation = (AudienceGroupOperation)2;
                    }

                    audience.AudienceDescription = (documentElement.Attributes["Description"].Value);
                    if (audienceGroupOperation != null && audienceGroupOperation != (AudienceGroupOperation)3)
                    {
                        audience.GroupOperation = (audienceGroupOperation);
                    }

                    if (documentElement.Attributes["Owner"] != null)
                    {
                        if (base.SharePointVersion.IsSharePoint2010OrLater)
                        {
                            audience.OwnerAccountName =
                                (this.ConvertUserNameToClaimsFormat(documentElement.Attributes["Owner"].Value,
                                    context.Site.Url));
                        }
                        else
                        {
                            audience.OwnerAccountName = (documentElement.Attributes["Owner"].Value);
                        }
                    }

                    if (audience.AudienceRules == null)
                    {
                        audience.AudienceRules = (new ArrayList());
                    }
                    else
                    {
                        audience.AudienceRules.Clear();
                    }

                    foreach (XmlNode xmlNode in documentElement.ChildNodes)
                    {
                        AudienceRuleComponent audienceRuleComponent = new AudienceRuleComponent();
                        audienceRuleComponent.LeftContent = (xmlNode.Attributes["LeftContent"].Value);
                        audienceRuleComponent.Operator = (xmlNode.Attributes["Operator"].Value);
                        audienceRuleComponent.RightContent = (xmlNode.Attributes["RightContent"].Value);
                        audience.AudienceRules.Add(audienceRuleComponent);
                    }

                    audience.Commit();
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    this.WriteAudienceXml(audience, xmlTextWriter);
                    xmlTextWriter.Flush();
                    result = stringBuilder.ToString();
                    return result;
                }

                IL_116:
                throw new Exception("Invalid GroupOperation");
            }

            return result;
        }

        private string ConvertUserNameToClaimsFormat(string sUserName, string sSiteUrl)
        {
            return sUserName;
        }

        public string DeleteAudience(string sAudienceName)
        {
            try
            {
                this.DeleteAudienceInternal(sAudienceName);
            }
            catch (FileNotFoundException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                return string.Empty;
            }

            return string.Empty;
        }

        private void DeleteAudienceInternal(string sAudienceName)
        {
            using (Context context = this.GetContext())
            {
                AudienceManager audienceManager = new AudienceManager(ServerContext.GetContext(context.Site));
                audienceManager.Audiences.Remove(sAudienceName);
            }
        }

        public string DeleteAllAudiences(string inputXml)
        {
            try
            {
                this.DeleteAllAudiencesInternal();
            }
            catch (FileNotFoundException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                return string.Empty;
            }

            return string.Empty;
        }

        private void DeleteAllAudiencesInternal()
        {
            using (Context context = this.GetContext())
            {
                AudienceManager audienceManager = new AudienceManager(ServerContext.GetContext(context.Site));
                List<Guid> list = new List<Guid>(audienceManager.Audiences.Count);
                foreach (Audience audience in audienceManager.Audiences)
                {
                    if (audience.AudienceID != Guid.Empty)
                    {
                        list.Add(audience.AudienceID);
                    }
                }

                foreach (Guid current in list)
                {
                    audienceManager.Audiences.Remove(current);
                }
            }
        }

        public string BeginCompilingAllAudiences()
        {
            try
            {
                this.BeginCompilingAllAudiencesInternal();
            }
            catch (FileNotFoundException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                return string.Empty;
            }

            return string.Empty;
        }

        private void BeginCompilingAllAudiencesInternal()
        {
            using (Context context = this.GetContext())
            {
                string str = null;
                if (base.SharePointVersion.IsSharePoint2007)
                {
                    str = SearchContext.GetContext(ServerContext.GetContext(context.Site)).Name;
                }
                else
                {
                    foreach (SPService service in SPFarm.Local.Services)
                    {
                        if (service.GetType().Name == "UserProfileService")
                        {
                            foreach (SPServiceApplication application in service.Applications)
                            {
                                str = application.Id.ToString();
                                break;
                            }

                            break;
                        }
                    }
                }

                AudienceJobReturnCode code =
                    (AudienceJobReturnCode)AudienceJob.RunAudienceJob(new string[] { str, "1", "1", null });
                if (code != AudienceJobReturnCode.AUDIENCEJOB_JOBRUN)
                {
                    string[] strArray2 = new string[]
                    {
                        "Failed to run audience compilation: The following error code was returned: ",
                        ((int)code).ToString(), " (", code.ToString(), ")"
                    };
                    throw new Exception(string.Concat(strArray2));
                }
            }
        }

        private object CastToUserProfileValue(string value, string type)
        {
            switch (type)
            {
                case "datenoyear":
                    return DateTime.Parse(value);
                case "date":
                    return DateTime.Parse(value);
                case "unique identifier":
                    return new Guid(value);
                case "integer":
                    return int.Parse(value);
                case "big integer":
                    return long.Parse(value);
                case "boolean":
                    return bool.Parse(value);
                case "float":
                    return float.Parse(value);
            }

            return value;
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            try
            {
                SPWebService value = SPFarm.Local.Services.GetValue<SPWebService>("");
                SPWebApplication sPWebApplication = value.WebApplications[sWebApp];
                sPWebApplication.Sites.Delete(sSiteURL);
            }
            finally
            {
                GC.Collect();
            }

            return string.Empty;
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML,
            AddSiteCollectionOptions addSiteCollOptions)
        {
            SiteCollectionCreator siteCollectionCreator = new SiteCollectionCreator();
            return siteCollectionCreator.CreateSiteCollection(this, sWebApp, sSiteCollectionXML, addSiteCollOptions);
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                using (Context context = this.GetContext())
                {
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(sUpdateXml);
                    if (updateSiteCollectionOptions.UpdateSiteQuota)
                    {
                        this.AttemptToSetSiteQuota(operationReporting, context, xmlNode);
                    }

                    if (updateSiteCollectionOptions.UpdateSiteAuditSettings)
                    {
                        this.SetSiteAuditSettings(context.Web, xmlNode);
                    }

                    if (updateSiteCollectionOptions.UpdateSiteAdmins)
                    {
                        this.AttemptToSetSiteAdministrators(operationReporting, context, xmlNode);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex, "Main exception handler");
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void AttemptToSetSiteAdministrators(OperationReporting operationReporting, Context ctx,
            XmlNode updateXML)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (SPUser sPUser in ((IEnumerable)ctx.Web.SiteUsers))
            {
                try
                {
                    XmlNode xmlNode = updateXML.SelectSingleNode(string.Format(".//SiteAdmin[@LoginName='{0}']",
                        sPUser.LoginName.ToUpper()));
                    if (sPUser.IsSiteAdmin && xmlNode == null)
                    {
                        sPUser.IsSiteAdmin = false;
                        sPUser.Update();
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    stringBuilder.AppendLine(sPUser.LoginName.ToString());
                }
            }

            if (stringBuilder.Length > 0)
            {
                operationReporting.LogWarning("Unable to delete following user(s) from admin list.",
                    stringBuilder.ToString());
            }

            stringBuilder.Length = 0;
            foreach (XmlNode node in updateXML.SelectNodes(".//SiteAdmin[@LoginName]"))
            {
                string attributeValueAsString = node.GetAttributeValueAsString("LoginName");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    try
                    {
                        SPUser sPUser2 = ctx.Web.EnsureUser(attributeValueAsString);
                        sPUser2.IsSiteAdmin = true;
                        sPUser2.Update();
                    }
                    catch (Exception ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        stringBuilder.AppendLine(attributeValueAsString);
                    }
                }
            }

            if (stringBuilder.Length > 0)
            {
                operationReporting.LogWarning("Unable to add the following user(s)", stringBuilder.ToString());
            }
        }

        private void AttemptToSetSiteQuota(OperationReporting operationReporting, Context ctx, XmlNode updateXML)
        {
            if (!base.SharePointVersion.IsSharePoint2010OrLater)
            {
                this.SetSiteQuota(ctx.Site, updateXML);
                return;
            }

            if (ctx.Web.CurrentUser.IsSiteAdmin)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate { this.SetSiteQuota(ctx.Site, updateXML); });
                return;
            }

            operationReporting.LogError("Unable to update site quota limits.",
                string.Format(
                    "The migrating user '{0}' should be a site or farm administrator to perform this operation.",
                    ctx.Web.CurrentUser.LoginName), string.Empty, 0, 0);
        }

        internal void MigrateSiteCollectionAdmins(SPSite site, XmlNode siteXML)
        {
            char[] separator = new char[]
            {
                ';'
            };
            string[] array = (siteXML.Attributes["SiteCollectionAdministrators"] != null)
                ? siteXML.Attributes["SiteCollectionAdministrators"].Value.Split(separator)
                : null;
            using (SPWeb sPWeb = site.OpenWeb())
            {
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    if (!string.IsNullOrEmpty(text))
                    {
                        try
                        {
                            SPUser sPUser = sPWeb.EnsureUser(text);
                            sPUser.IsSiteAdmin = true;
                            sPUser.Update();
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }
                }
            }
        }

        internal void SetSiteQuota(SPSite site, XmlNode siteXML)
        {
            int num = 0;
            long num2 = -1L;
            long num3 = -1L;
            if (siteXML.Attributes["QuotaID"] != null)
            {
                int.TryParse(siteXML.Attributes["QuotaID"].Value, out num);
            }

            if (siteXML.Attributes["QuotaStorageLimit"] != null)
            {
                long.TryParse(siteXML.Attributes["QuotaStorageLimit"].Value, out num2);
            }

            if (siteXML.Attributes["QuotaStorageWarning"] != null)
            {
                long.TryParse(siteXML.Attributes["QuotaStorageWarning"].Value, out num3);
            }

            site.Quota.StorageWarningLevel = 0L;
            site.Quota.StorageMaximumLevel = 0L;
            bool flag = false;
            if (num > 0)
            {
                SPQuotaTemplate quotaTemplate = this.GetQuotaTemplate(num);
                if (quotaTemplate != null)
                {
                    site.Quota.StorageMaximumLevel = quotaTemplate.StorageMaximumLevel;
                    site.Quota.StorageWarningLevel = quotaTemplate.StorageWarningLevel;
                    site.Quota.InvitedUserMaximumLevel = quotaTemplate.InvitedUserMaximumLevel;
                    site.Quota.QuotaID = quotaTemplate.QuotaID;
                    flag = true;
                }
            }

            if (!flag && num2 >= 0L && num3 >= 0L)
            {
                site.Quota.StorageMaximumLevel = num2;
                site.Quota.StorageWarningLevel = num3;
            }
        }

        private void SetSiteAuditSettings(SPWeb rootWeb, XmlNode siteXML)
        {
            if (rootWeb.Site.Audit != null)
            {
                XmlAttribute xmlAttribute = siteXML.Attributes["AuditFlags"];
                if (xmlAttribute != null)
                {
                    rootWeb.Site.Audit.AuditFlags = (SPAuditMaskType)int.Parse(xmlAttribute.Value);
                }

                xmlAttribute = siteXML.Attributes["UseAuditFlagCache"];
                if (xmlAttribute != null)
                {
                    rootWeb.Site.Audit.UseAuditFlagCache = bool.Parse(xmlAttribute.Value);
                }

                rootWeb.Site.Audit.Update();
            }
        }

        private SPQuotaTemplate GetQuotaTemplate(int iQuotaId)
        {
            foreach (SPQuotaTemplate sPQuotaTemplate in ((IEnumerable)SPWebService.ContentService.QuotaTemplates))
            {
                if ((int)sPQuotaTemplate.QuotaID == iQuotaId)
                {
                    return sPQuotaTemplate;
                }
            }

            return null;
        }

        private void PopulateUserProfileProperty2007(XmlNode prop, Property coreProperty)
        {
            string value = prop.Attributes["DisplayName"].Value;
            if (coreProperty.DisplayName != value)
            {
                coreProperty.DisplayName = (value);
            }

            value = prop.Attributes["Description"].Value;
            if (coreProperty.Description != value)
            {
                coreProperty.Description = (value);
            }

            MultiValueSeparator multiValueSeparator =
                (MultiValueSeparator)Enum.Parse(typeof(MultiValueSeparator), prop.Attributes["Separator"].Value);
            if (coreProperty.Separator != multiValueSeparator)
            {
                coreProperty.Separator = (multiValueSeparator);
            }

            Privacy privacy = (Privacy)Enum.Parse(typeof(Privacy), prop.Attributes["DefaultPrivacy"].Value);
            if (coreProperty.DefaultPrivacy != privacy)
            {
                coreProperty.DefaultPrivacy = (privacy);
            }

            PrivacyPolicy privacyPolicy =
                (PrivacyPolicy)Enum.Parse(typeof(PrivacyPolicy), prop.Attributes["PrivacyPolicy"].Value);
            if (coreProperty.PrivacyPolicy != privacyPolicy)
            {
                coreProperty.PrivacyPolicy = (privacyPolicy);
            }

            bool flag = bool.Parse(prop.Attributes["UserOverridePrivacy"].Value);
            if (coreProperty.UserOverridePrivacy != flag)
            {
                coreProperty.UserOverridePrivacy = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsReplicable"].Value);
            if (coreProperty.IsReplicable != flag)
            {
                coreProperty.IsReplicable = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsAlias"].Value);
            if (coreProperty.IsAlias != flag)
            {
                coreProperty.IsAlias = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsSearchable"].Value);
            if (coreProperty.IsSearchable != flag)
            {
                coreProperty.IsSearchable = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsUserEditable"].Value);
            if (coreProperty.IsUserEditable != flag)
            {
                coreProperty.IsUserEditable = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsVisibleOnEditor"].Value);
            if (coreProperty.IsVisibleOnEditor != flag)
            {
                coreProperty.IsVisibleOnEditor = (flag);
            }

            flag = bool.Parse(prop.Attributes["IsVisibleOnViewer"].Value);
            if (coreProperty.IsVisibleOnViewer != flag)
            {
                coreProperty.IsVisibleOnViewer = (flag);
            }

            int num = int.Parse(prop.Attributes["MaximumShown"].Value);
            if (coreProperty.MaximumShown != num)
            {
                coreProperty.MaximumShown = (num);
            }

            flag = bool.Parse(prop.Attributes["IsColleagueEventLog"].Value);
            if (coreProperty.IsColleagueEventLog != flag)
            {
                coreProperty.IsColleagueEventLog = (flag);
            }
        }

        private void GetWebApplicationForExpert(OperationReporting opResult)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
            {
                xmlWriter.WriteStartElement("WebApplicationCollection");
                SPWebService value = SPFarm.Local.Services.GetValue<SPWebService>("");
                using (Context context = this.GetContext())
                {
                    foreach (SPWebApplication current in value.WebApplications)
                    {
                        xmlWriter.WriteStartElement("WebApplication");
                        xmlWriter.WriteAttributeString("Name", current.Name);
                        string value2 = current.AlternateUrls.GetResponseUrl(context.Site.Zone, true).Uri.ToString();
                        xmlWriter.WriteAttributeString("URL", value2);
                        xmlWriter.WriteAttributeString("AlertsEnabled", current.AlertsEnabled.ToString());
                        xmlWriter.WriteAttributeString("AlertsMaximum", current.AlertsMaximum.ToString());
                        xmlWriter.WriteAttributeString("RecycleBinEnabled", current.RecycleBinEnabled.ToString());
                        xmlWriter.WriteAttributeString("RecycleBinCleanupEnabled",
                            current.RecycleBinCleanupEnabled.ToString());
                        xmlWriter.WriteAttributeString("RecycleBinRetentionPeriod",
                            current.RecycleBinRetentionPeriod.ToString());
                        xmlWriter.WriteAttributeString("MaximumFileSize", current.MaximumFileSize.ToString());
                        xmlWriter.WriteAttributeString("SecondStageRecycleBinQuota",
                            current.SecondStageRecycleBinQuota.ToString());
                        xmlWriter.WriteAttributeString("SecurityValidationExpirationEnabled",
                            current.FormDigestSettings.Enabled.ToString());
                        xmlWriter.WriteAttributeString("SecurityValidationExpires",
                            current.FormDigestSettings.Expires.ToString());
                        xmlWriter.WriteAttributeString("SecurityValidationExpiresAfter",
                            current.FormDigestSettings.Timeout.ToString());
                        xmlWriter.WriteAttributeString("ChangeLogExpirationEnabled",
                            current.ChangeLogExpirationEnabled.ToString());
                        xmlWriter.WriteAttributeString("ChangeLogRetentionPeriod",
                            current.ChangeLogRetentionPeriod.ToString());
                        xmlWriter.WriteAttributeString("AlertsLimited", current.AlertsLimited.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }

            opResult.LogObjectXml(stringBuilder.ToString());
        }

        private void GetInstalledLanguagePacksForExpert(OperationReporting opResult)
        {
            SPLanguage globalServerLanguage = SPRegionalSettings.GlobalServerLanguage;
            SPLanguageCollection globalInstalledLanguages = SPRegionalSettings.GlobalInstalledLanguages;
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
                   {
                       OmitXmlDeclaration = true
                   }))
            {
                int lCID = globalServerLanguage.LCID;
                xmlWriter.WriteStartElement("LanguagePacks");
                foreach (SPLanguage sPLanguage in ((IEnumerable)globalInstalledLanguages))
                {
                    try
                    {
                        xmlWriter.WriteStartElement("LanguagePack");
                        xmlWriter.WriteAttributeString("Id", sPLanguage.LCID.ToString());
                        xmlWriter.WriteAttributeString("DisplayName", sPLanguage.DisplayName);
                        if (sPLanguage.LCID == lCID)
                        {
                            xmlWriter.WriteAttributeString("IsDefault", true.ToString());
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("IsDefault", false.ToString());
                        }

                        xmlWriter.WriteEndElement();
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        opResult.LogError(ex, "An error occured while retrieving language pack");
                    }
                }

                xmlWriter.WriteEndElement();
            }

            opResult.LogObjectXml(stringBuilder.ToString());
        }

        private void GetSiteCollRecyclebinStatisticsForExpert(string commandConfigurationXml,
            OperationReporting opResult)
        {
            int num = 0;
            long num2 = 0L;
            string str = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            GetSiteCollRecyclebinStatisticsConfiguration getSiteCollRecyclebinStatisticsConfiguration =
                commandConfigurationXml.Deserialize<GetSiteCollRecyclebinStatisticsConfiguration>();
            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true
                       }))
                {
                    bool includeCombineStatistics =
                        getSiteCollRecyclebinStatisticsConfiguration.IncludeCombineStatistics;
                    using (Context context = this.GetContext())
                    {
                        str = context.Site.Url;
                        xmlWriter.WriteStartElement("SiteCollection");
                        xmlWriter.WriteAttributeString("Id", context.Site.ID.ToString());
                        context.Site.GetRecycleBinStatistics(out num, out num2);
                        xmlWriter.WriteAttributeString("FirstStageItemCount", num.ToString());
                        xmlWriter.WriteAttributeString("FirstStageItemSize", num2.ToString());
                        if (includeCombineStatistics)
                        {
                            int count = context.Site.RecycleBin.Count;
                            long num3 = context.Site.RecycleBin.OfType<SPRecycleBinItem>()
                                .Sum((SPRecycleBinItem rbi) => rbi.Size);
                            xmlWriter.WriteAttributeString("CombinedItemCount", count.ToString());
                            xmlWriter.WriteAttributeString("CombinedItemSize", num3.ToString());
                        }

                        xmlWriter.WriteEndElement();
                    }
                }

                opResult.LogObjectXml(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                opResult.LogError(ex, "An error occured while retrieving recycle bin statistics for " + str);
            }
        }

        private void GetFullTrustSolutionsForExpert(OperationReporting opResult)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
            {
                xmlWriter.WriteStartElement("FeatureCollection");
                try
                {
                    SPSolutionCollection solutions = SPFarm.Local.Solutions;
                    List<SPSolution> list = (solutions != null) ? solutions.ToList<SPSolution>() : null;
                    if (list.Count > 0)
                    {
                        using (Context context = this.GetContext())
                        {
                            SPWebCollection allWebs = context.Site.AllWebs;
                            for (int i = 0; i < allWebs.Count; i++)
                            {
                                using (SPWeb sPWeb = allWebs[i])
                                {
                                    this.GetFeatures(sPWeb.Url, xmlWriter, list, sPWeb.Features, opResult);
                                }
                            }

                            this.GetFeatures(context.Site.Url, xmlWriter, list, context.Site.Features, opResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    opResult.LogError(ex, "An error occured while retrieving Full Trust solutions");
                }

                xmlWriter.WriteEndElement();
            }

            opResult.LogObjectXml(stringBuilder.ToString());
        }

        private void GetFeatures(string siteUrl, XmlWriter xmlWriter, List<SPSolution> fullTrustSolutionCollection,
            SPFeatureCollection featureCollection, OperationReporting opResult)
        {
            try
            {
                foreach (SPFeature sPFeature in ((IEnumerable)featureCollection))
                {
                    if (sPFeature.Definition != null && sPFeature.Definition.SolutionId != Guid.Empty)
                    {
                        Guid featureSolutionId = sPFeature.Definition.SolutionId;
                        SPSolution sPSolution = (from solution in fullTrustSolutionCollection
                            where solution.SolutionId.Equals(featureSolutionId)
                            select solution).FirstOrDefault<SPSolution>();
                        if (sPSolution != null && sPSolution.Deployed)
                        {
                            xmlWriter.WriteStartElement("Feature");
                            xmlWriter.WriteAttributeString("Name", sPFeature.Definition.DisplayName);
                            xmlWriter.WriteAttributeString("FeatureId", featureSolutionId.ToString());
                            xmlWriter.WriteAttributeString("FullTrustSolutionName", sPSolution.DisplayName);
                            xmlWriter.WriteAttributeString("FeatureScope", sPFeature.Definition.Scope.ToString());
                            xmlWriter.WriteAttributeString("Deployed", sPSolution.Deployed.ToString());
                            xmlWriter.WriteAttributeString("DeploymentState", sPSolution.DeploymentState.ToString());
                            xmlWriter.WriteAttributeString("SiteUrl", siteUrl);
                            xmlWriter.WriteEndElement();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                opResult.LogError(ex,
                    string.Format("An error occured while retrieving Full Trust solutions for site '{0}'", siteUrl));
            }
        }

        static OMAdapter()
        {
            OMAdapter.s_IntConverter = new Int32Converter();
            OMAdapter.s_DoubleConverter = new DoubleConverter();
            OMAdapter.s_CultureInfo = new CultureInfo("en-US");
            OMAdapter.tsSecurity = default(TimeSpan);
            OMAdapter.tsDBFetch = default(TimeSpan);
            OMAdapter.s_iGetListQueryRowLimit = null;
            OMAdapter.s_updateTypeParameters = new Type[]
            {
                typeof(bool),
                typeof(bool),
                typeof(Guid),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool)
            };
            OMAdapter.s_EmptyGuid = new Guid("00000000000000000000000000000000");
            OMAdapter.EnterPriseKeywordsFieldId = new Guid("23f27201-bee3-471e-b2e7-b64fd8b7ca38");
            OMAdapter._brackets = null;
            OMAdapter._bracketLock = new object();
            OMAdapter.m_bSupportsPublishing = null;
            OMAdapter.PAGES_LIBRARY_TEMPLATE_ID = 850;
            OMAdapter.s_publishingType = null;
            OMAdapter.isSupportsUserProfile = null;
            OMAdapter.userProfileManager = null;
            OMAdapter.isSPFoundation2013 = null;
            OMAdapter.isSPFoundation2010WithSearchExpress = null;
            OMAdapter.s_oPrimingLock = new object();
            OMAdapter.s_bWebServicesPrimed = false;
            OMAdapter.s_bSupportsDBWriting = null;
            OMAdapter.s_supportsPartialDBWriting = null;
            OMAdapter.C_HASH_TAGS_TERMSET_GUID = new Guid("3CEB0050-69A1-40E7-A427-83E2FAC80C27");
            OMAdapter.CustomWebPartTypes = new ReadOnlyCollection<string>(new List<string>
            {
                "Microsoft.Office.Excel.WebUI.ExcelWebRenderer"
            });
            OMAdapter.m_bSupportsExcelWebAccessServices = null;
            OMAdapter.PUBLISHINGGUID = new Guid("8c6a6980-c3d9-440e-944c-77f93bc65a7e");
            OMAdapter.RECORDSCENTERGUID = new Guid("e0a45587-1069-46bd-bf05-8c8db8620b08");
            OMAdapter.WIKIHOMEPAGEFEATUREGUID = new Guid("00bfea71-d8fe-4fec-8dad-01c19a6e4053");
            OMAdapter.s_overrideSQLAuthenticationHandler = new OverrideSQLAuthenticationHandler();
            OMAdapter.PrimeSharePointSSLHandler();
        }

        private static void ForceSharePointCertificateValidationInitialize()
        {
            try
            {
                AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                AssemblyName assemblyName =
                    referencedAssemblies.FirstOrDefault((AssemblyName name) => name.Name == "Microsoft.SharePoint");
                if (assemblyName != null)
                {
                    if (assemblyName.Version.Major != 14 || ApplicationData.IsWeb)
                    {
                        Assembly assembly = Assembly.Load(assemblyName);
                        if (assembly != null)
                        {
                            Type type = assembly.GetType("Microsoft.SharePoint.SPCertificateValidator");
                            if (type != null)
                            {
                                MethodInfo method = type.GetMethod("SetServicePointManagerCertificateValidationPolicy",
                                    BindingFlags.Static | BindingFlags.NonPublic);
                                if (method != null)
                                {
                                    MulticastDelegate serverCertificateValidationCallback =
                                        ServicePointManager.ServerCertificateValidationCallback;
                                    Delegate[] array = null;
                                    if (serverCertificateValidationCallback != null)
                                    {
                                        array = serverCertificateValidationCallback.GetInvocationList();
                                    }

                                    try
                                    {
                                        method.Invoke(null, null);
                                    }
                                    finally
                                    {
                                        if (serverCertificateValidationCallback != null && array != null &&
                                            array.Length > 0)
                                        {
                                            Delegate[] array2 = array;
                                            for (int i = 0; i < array2.Length; i++)
                                            {
                                                Delegate @delegate = array2[i];
                                                ServicePointManager.ServerCertificateValidationCallback =
                                                    (RemoteCertificateValidationCallback)Delegate.Combine(
                                                        ServicePointManager.ServerCertificateValidationCallback,
                                                        (RemoteCertificateValidationCallback)@delegate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        public static void LogExceptionDetails(Exception ex, string methodName, string category = null)
        {
            Utils.LogExceptionDetails(ex, methodName, "Metalogix.SharePoint.Adapters.OM.OMAdapter", category);
        }

        public static void LogMessageDetails(string message, string methodName, string category = null)
        {
            Utils.LogMessageDetails(message, methodName, "Metalogix.SharePoint.Adapters.OM.OMAdapter", category);
        }

        public OMAdapter()
        {
        }

        public OMAdapter(string sSiteUrl, Credentials credentials)
        {
            this.m_sUrl = sSiteUrl;
            this.m_credentials = credentials;
        }

        public OMAdapter(SPWeb web)
        {
            this.m_sUrl = web.Url;
        }

        [SPDisposeCheckIgnore(SPDisposeCheckID.SPDisposeCheckID_110,
            "Ignore case where a new SPSite() instance is never referenced by the Metalogix SharePoint Extensions Web Service.")]
        internal Context GetContext()
        {
            bool flag = SPContext.Current != null && this.Url != null &&
                        this.Url.ToLowerInvariant() == SPContext.Current.Site.Url.ToLowerInvariant();
            if (flag)
            {
                return new Context(SPContext.Current.Site, SPContext.Current.Site.RootWeb, this, false);
            }

            SPSite currentSite = null;
            if (this.Credentials.IsDefault)
            {
                currentSite = new SPSite(this.Url);
            }
            else
            {
                using (this.Credentials.Impersonate())
                {
                    currentSite = new SPSite(this.Url);
                }
            }

            return new Context(currentSite, this, true);
        }

        [SPDisposeCheckIgnore(SPDisposeCheckID.SPDisposeCheckID_110,
            "Ignore case where a new SPSite() instance is never referenced by the Metalogix SharePoint Extensions Web Service.")]
        private SPSite GetSiteWithElevatedPrivilege()
        {
            SPSite result;
            using (SPSite sPSite = new SPSite(this.Url))
            {
                if (!SPUtility.IsLoginValid(sPSite, this.Credentials.UserName))
                {
                    throw new Exception(string.Format("Invalid UserName '{0}' for site at: '{1}'",
                        this.Credentials.UserName, this.Url));
                }

                SPUserToken userToken = sPSite.RootWeb.EnsureUser(this.Credentials.UserName).UserToken;
                result = new SPSite(this.Url, userToken);
            }

            return result;
        }

        private static string GetServerRelativeUrl(string sUrl)
        {
            if (!sUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !sUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return sUrl;
            }

            int num = sUrl.IndexOf("/", 8, StringComparison.Ordinal);
            if (num < 0)
            {
                return "";
            }

            return sUrl.Substring(num);
        }

        protected static void PrimeSharePointSSLHandler()
        {
            lock (OMAdapter.s_oPrimingLock)
            {
                if (!OMAdapter.s_bWebServicesPrimed)
                {
                    OMAdapter.ForceSharePointCertificateValidationInitialize();
                    OMAdapter.s_bWebServicesPrimed = true;
                }
            }
        }

        public override void CheckConnection()
        {
            this.SetErrorForUnSupportedOMConnections();
            if (!base.CredentialsAreDefault)
            {
                if (this.Credentials.Password.IsNullOrEmpty())
                {
                    goto IL_5C;
                }
            }

            try
            {
                this.GetWeb(false);
                this.ServerAdapterConfiguration.Load();
                return;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                if (ex is UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException("Access Denied");
                }

                throw;
            }

            IL_5C:
            if (string.IsNullOrEmpty(this.Credentials.UserName))
            {
                throw new UnauthorizedAccessException("A username is required");
            }

            throw new UnauthorizedAccessException("A password is required");
        }

        private void SetErrorForUnSupportedOMConnections()
        {
            if (!ApplicationData.IsWeb)
            {
                string format =
                    "Local Connection (SharePoint Object Model) is not supported for SharePoint {0} environment. It is recommended to use MEWS or DB connection instead.";
                if (!AdapterConfigurationVariables.Show2007OMConnection && base.SharePointVersion.IsSharePoint2007)
                {
                    throw new Exception(string.Format(format, "2007"));
                }

                if (base.SharePointVersion.IsSharePoint2010)
                {
                    throw new Exception(string.Format(format, "2010"));
                }
            }
        }

        private static SPUserToken GetUserToken(string sWebURL, string sUserName)
        {
            SPUserToken result;
            using (SPSite sPSite = new SPSite(sWebURL))
            {
                using (SPWeb sPWeb = sPSite.OpenWeb())
                {
                    PropertyInfo property = typeof(SPWeb).GetProperty("Request",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod |
                        BindingFlags.GetProperty);
                    object value = property.GetValue(sPWeb, null);
                    MethodInfo method = value.GetType().GetMethod("GetUserToken",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.InvokeMethod);
                    try
                    {
                        SPUserToken sPUserToken = new SPUserToken((byte[])method.Invoke(value, new object[]
                        {
                            sPWeb.Url,
                            sUserName
                        }));
                        result = sPUserToken;
                    }
                    catch (TargetInvocationException ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        throw ex.InnerException;
                    }
                }
            }

            return result;
        }

        public override SharePointAdapter Clone()
        {
            OMAdapter oMAdapter = new OMAdapter();
            oMAdapter.CloneFrom(this, true);
            return oMAdapter;
        }

        public override SharePointAdapter CloneForNewSiteCollection()
        {
            OMAdapter oMAdapter = new OMAdapter();
            oMAdapter.CloneFrom(this, false);
            return oMAdapter;
        }

        public void CloneFrom(OMAdapter newAdapter, bool bIncludeSiteCollectionSpecificProperties)
        {
            this.IsReadOnlyAdapter = newAdapter.IsReadOnlyAdapter;
            base.IsDataLimitExceededForContentUnderMgmt = newAdapter.IsDataLimitExceededForContentUnderMgmt;
            this.m_sUrl = newAdapter.m_sUrl;
            this.m_credentials = newAdapter.m_credentials;
            if (bIncludeSiteCollectionSpecificProperties)
            {
                this.m_bClaimsAuthenticationInUse = new bool?(newAdapter.ClaimsAuthenticationInUse);
                this.AuthenticationInitializer = newAdapter.AuthenticationInitializer;
            }

            this.AdapterProxy = newAdapter.AdapterProxy;
            this.IncludedCertificates = newAdapter.IncludedCertificates;
            base.SetSystemInfo(newAdapter.SystemInfo.Clone());
            base.SetSharePointVersion(newAdapter.SharePointVersion.Clone());
        }

        private static string OverrideConnectionIfRequired(string connectionString)
        {
            if (OMAdapter.s_overrideSQLAuthenticationHandler.Enabled)
            {
                string text =
                    OMAdapter.s_overrideSQLAuthenticationHandler.ConstructOverriddenConnectionString(connectionString);
                if (OMAdapter.s_overrideSQLAuthenticationHandler.IsConnectable(text))
                {
                    return text;
                }
            }

            return connectionString;
        }

        private static bool CheckDBWritingAvailability()
        {
            if (SPFarm.Local.BuildVersion.Major == 15)
            {
                return false;
            }

            if (SPContext.Current != null)
            {
                string text = OMAdapter.CorrectDatabaseConnectionString(
                    SPContext.Current.Site.ContentDatabase.DatabaseConnectionString,
                    SPContext.Current.Site.ContentDatabase.Server);
                SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                {
                    text,
                    SPContext.Current.Web.ID,
                    SPContext.Current.Site.ID
                });
                try
                {
                    bool result;
                    if (dBAdapter != null)
                    {
                        dBAdapter.CheckConnection();
                        result = true;
                        return result;
                    }

                    result = false;
                    return result;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    bool result = false;
                    return result;
                }
            }

            return SharePointAdapter.GetDBAdapter(new object[0]) != null;
        }

        private static string CorrectDatabaseConnectionString(string sOriginalConnectionString, string sDatabaseServer)
        {
            string text = sOriginalConnectionString;
            int num = text.IndexOf("Server=", StringComparison.InvariantCulture) + 7;
            if (num >= 7)
            {
                int startIndex = text.IndexOf(";", num, StringComparison.Ordinal);
                string text2;
                if (sDatabaseServer.ToUpper().Contains("MICROSOFT##SSEE"))
                {
                    text2 = "\\\\.\\pipe\\MSSQL$MICROSOFT##SSEE\\sql\\query";
                }
                else
                {
                    text2 = sDatabaseServer;
                }

                text = string.Concat(new string[]
                {
                    text.Substring(0, num),
                    text2,
                    text.Substring(startIndex),
                    ";Application Name=",
                    typeof(OMAdapter).Name
                });
            }

            return OMAdapter.OverrideConnectionIfRequired(text);
        }

        protected IDBWriter GetDBWriter(SPWeb web)
        {
            if (base.SharePointVersion.IsSharePoint2013OrLater)
            {
                throw new InvalidOperationException(
                    "Full database support for SharePoint 2013 is not currently supported");
            }

            if (!OMAdapter.SupportsDBWriting)
            {
                throw new Exception("DB Writing is not supported");
            }

            if (this.m_dbAdapter == null || this.m_dbAdapter.WebID != web.ID.ToString())
            {
                string text = OMAdapter.CorrectDatabaseConnectionString(
                    web.Site.ContentDatabase.DatabaseConnectionString, web.Site.ContentDatabase.Server);
                SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                {
                    text,
                    web.ID,
                    web.Site.ID
                });
                if (dBAdapter == null || !typeof(IDBWriter).IsAssignableFrom(dBAdapter.GetType()))
                {
                    throw new Exception("Failed to get DB Adapter");
                }

                IDBWriter iDBWriter = dBAdapter as IDBWriter;
                if (web.Site.HostHeaderIsSiteName && iDBWriter != null)
                {
                    string arg = (web.Site.Port != 80 && web.Site.Port != 443)
                        ? string.Format(":{0}", web.Site.Port)
                        : string.Empty;
                    string hostHeader = string.Format("{0}{1}", web.Site.HostName, arg).Trim();
                    iDBWriter.HostHeader = hostHeader;
                }

                dBAdapter.Url = this.ServerRelativeUrl;
                this.m_dbAdapter = dBAdapter;
            }

            return (IDBWriter)this.m_dbAdapter;
        }

        protected IMigrationExpertReports GetDBReader(SPWeb web)
        {
            if (this._dbReader == null || this._dbReader.WebID != web.ID.ToString())
            {
                string text = OMAdapter.CorrectDatabaseConnectionString(
                    web.Site.ContentDatabase.DatabaseConnectionString, web.Site.ContentDatabase.Server);
                SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                {
                    text,
                    web.ID,
                    web.Site.ID
                });
                if (dBAdapter == null || !typeof(IMigrationExpertReports).IsAssignableFrom(dBAdapter.GetType()))
                {
                    throw new Exception("Failed to get DB Adapter");
                }

                IDBReader iDBReader = dBAdapter as IDBReader;
                if (web.Site.HostHeaderIsSiteName && iDBReader != null)
                {
                    string arg = (web.Site.Port != 80 && web.Site.Port != 443)
                        ? string.Format(":{0}", web.Site.Port)
                        : string.Empty;
                    string hostHeader = string.Format("{0}{1}", web.Site.HostName, arg).Trim();
                    iDBReader.HostHeader = hostHeader;
                }

                dBAdapter.Url = this.ServerRelativeUrl;
                this._dbReader = dBAdapter;
            }

            return (IMigrationExpertReports)this._dbReader;
        }

        private bool Is401UnauthorizedErrorInProgress()
        {
            return HttpContext.Current != null && HttpContext.Current.Response != null &&
                   HttpContext.Current.Response.StatusCode == 401;
        }

        private void Cancel401UnauthorizedError()
        {
            if (this.Is401UnauthorizedErrorInProgress())
            {
                Thread.ResetAbort();
                HttpContext.Current.Response.Status = "200 OK";
                HttpContext.Current.Response.StatusCode = 200;
                HttpContext.Current.Response.StatusDescription = "OK";
                HttpContext.Current.Response.ClearContent();
            }
        }

        private bool HideUnauthorizedAccessError(Exception ex)
        {
            if (ex is ThreadAbortException)
            {
                this.Cancel401UnauthorizedError();
                return true;
            }

            return ex is UnauthorizedAccessException ||
                   ex.Message.Contains("Additions to this Web site have been blocked.");
        }

        public override string GetServerVersion()
        {
            return this.GetServerVersionObject().ToString();
        }

        private Version GetServerVersionObject()
        {
            Version version;
            using (Context context = this.GetContext())
            {
                Assembly assembly = context.Site.GetType().Assembly;
                version = assembly.GetName().Version;
            }

            return version;
        }

        public string GetSharePointVersion()
        {
            string result = null;
            try
            {
                result = SPFarm.Local.BuildVersion.ToString();
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = typeof(SPSite).Assembly.GetName().Version.ToString();
            }

            return result;
        }

        private static bool CheckPartialDBWritingAvailability()
        {
            bool result;
            try
            {
                if (SPContext.Current != null)
                {
                    string text = OMAdapter.CorrectDatabaseConnectionString(
                        SPContext.Current.Site.ContentDatabase.DatabaseConnectionString,
                        SPContext.Current.Site.ContentDatabase.Server);
                    SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                    {
                        text,
                        SPContext.Current.Web.ID,
                        SPContext.Current.Site.ID
                    });
                    try
                    {
                        IDB2013CheckOverride iDB2013CheckOverride = dBAdapter as IDB2013CheckOverride;
                        if (iDB2013CheckOverride != null)
                        {
                            iDB2013CheckOverride.CheckConnectionSkip2013Check();
                            result = true;
                            return result;
                        }

                        if (dBAdapter != null)
                        {
                            dBAdapter.CheckConnection();
                            result = true;
                            return result;
                        }

                        result = false;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        result = false;
                        return result;
                    }
                }

                result = (SharePointAdapter.GetDBAdapter(new object[0]) != null);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        private static void CheckPartialDBWritingAvailabilityWithDetails(OperationReporting opResult)
        {
            if (SPContext.Current != null)
            {
                string text = OMAdapter.CorrectDatabaseConnectionString(
                    SPContext.Current.Site.ContentDatabase.DatabaseConnectionString,
                    SPContext.Current.Site.ContentDatabase.Server);
                SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                {
                    text,
                    SPContext.Current.Web.ID,
                    SPContext.Current.Site.ID
                });
                try
                {
                    IDB2013CheckOverride iDB2013CheckOverride = dBAdapter as IDB2013CheckOverride;
                    if (iDB2013CheckOverride != null)
                    {
                        iDB2013CheckOverride.CheckConnectionSkip2013Check();
                    }
                    else if (dBAdapter != null)
                    {
                        dBAdapter.CheckConnection();
                    }
                    else
                    {
                        opResult.LogInformation(Resources.UnableToResolveDBAdapterType,
                            Resources.UnableToResolveDBAdapterTypeDetails);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    opResult.LogError(Resources.UnableToConnectToDatabase, ex.Message, ex.StackTrace, 0, 0);
                    return;
                }
            }

            if (SharePointAdapter.GetDBAdapter(new object[0]) == null)
            {
                opResult.LogInformation(Resources.UnableToResolveDBAdapterType,
                    Resources.UnableToResolveDBAdapterTypeDetails);
            }
        }

        protected IDBWriter GetDBWriterPartial(SPWeb web)
        {
            if (!OMAdapter.SupportsPartialDBWriting)
            {
                throw new Exception("Partial DB Writing is not supported");
            }

            if (this.m_dbAdapterPartial == null || this.m_dbAdapterPartial.WebID != web.ID.ToString())
            {
                string text = OMAdapter.CorrectDatabaseConnectionString(
                    web.Site.ContentDatabase.DatabaseConnectionString, web.Site.ContentDatabase.Server);
                SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(new object[]
                {
                    text,
                    web.ID,
                    web.Site.ID
                });
                if (dBAdapter == null || !typeof(IDBWriter).IsAssignableFrom(dBAdapter.GetType()))
                {
                    throw new Exception("Failed to get Partial DB Adapter");
                }

                IDBWriter iDBWriter = dBAdapter as IDBWriter;
                if (web.Site.HostHeaderIsSiteName && iDBWriter != null)
                {
                    string arg = (web.Site.Port != 80 && web.Site.Port != 443)
                        ? string.Format(":{0}", web.Site.Port)
                        : string.Empty;
                    string hostHeader = string.Format("{0}{1}", web.Site.HostName, arg).Trim();
                    iDBWriter.HostHeader = hostHeader;
                }

                dBAdapter.Url = this.ServerRelativeUrl;
                this.m_dbAdapterPartial = dBAdapter;
            }

            return (IDBWriter)this.m_dbAdapterPartial;
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                if (string.IsNullOrEmpty(commandName))
                {
                    operationReporting.LogError(Resources.CommandNameIsNull, string.Empty, string.Empty, 0, 0);
                }
                else
                {
                    switch (commandName.GetValueAsEnumValue<SharePointAdapterCommands>())
                    {
                        case SharePointAdapterCommands.Unknown:
                            operationReporting.LogError(string.Format(Resources.InvalidCommandName, commandName),
                                string.Empty, string.Empty, 0, 0);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetListByName:
                            this.GetListByTitle(commandConfigurationXml, operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetWebApplicationForExpert:
                            this.GetWebApplicationForExpert(operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetInstalledLanguagePacksForExpert:
                            this.GetInstalledLanguagePacksForExpert(operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetSiteCollRecyclebinStatisticsForExpert:
                            this.GetSiteCollRecyclebinStatisticsForExpert(commandConfigurationXml, operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetColumnDefaultSettings:
                            this.GetColumnDefaultSettings(commandConfigurationXml, operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.SetColumnDefaultSettings:
                            this.SetColumnDefaultSettings(commandConfigurationXml, operationReporting);
                            goto IL_EB;
                        case SharePointAdapterCommands.GetFullTrustSolutionsForExpert:
                            this.GetFullTrustSolutionsForExpert(operationReporting);
                            goto IL_EB;
                    }

                    operationReporting.LogError(null,
                        string.Format(Resources.CommandNotImplemented, commandName, this.AdapterShortName));
                }

                IL_EB: ;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex,
                    string.Format(Resources.ErrorInCommandExecution, commandName, this.AdapterShortName));
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private void GetListByTitle(string commandConfigurationXml, OperationReporting opResult)
        {
            GetListByNameConfiguration getListByNameConfiguration =
                commandConfigurationXml.Deserialize<GetListByNameConfiguration>();
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[getListByNameConfiguration.ListTitle];
                if (sPList == null)
                {
                    throw new Exception(string.Format("The list with ID: '{0}' does not exist in on the site: {1}",
                        getListByNameConfiguration.ListTitle, web.Url));
                }

                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.GetListXML(sPList, xmlWriter, true);
                opResult.LogObjectXml(stringBuilder.ToString());
            }
        }

        private static bool CheckStoragePointAvailable()
        {
            return OMAdapter.CheckBluethreadAPIAvailable() && OMAdapter.CheckStoragePointSupportAvailable();
        }

        private static bool CheckBluethreadAPIAvailable()
        {
            Type type = Type.GetType(
                "Bluethread.SharePoint.StoragePoint.MigrationSupport, Bluethread.SharePoint.StoragePoint.StoragePointAPI, Version=2.0.0.0, Culture=neutral, PublicKeyToken=141fe4b547d7494f");
            return type != null;
        }

        private static bool CheckStoragePointSupportAvailable()
        {
            Type storagePointSupportType = OMAdapter.GetStoragePointSupportType();
            return storagePointSupportType != null;
        }

        private static Type GetStoragePointSupportType()
        {
            string arg = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Type.GetType(string.Format(
                "Metalogix.SharePoint.Adapters.StoragePoint.StoragePointSupport, Metalogix.SharePoint.Adapters.StoragePoint, Version={0}, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4",
                arg));
        }

        public static IStoragePointSupport GetStoragePointAdapter()
        {
            Type storagePointSupportType = OMAdapter.GetStoragePointSupportType();
            if (storagePointSupportType == null)
            {
                return null;
            }

            object obj = Activator.CreateInstance(storagePointSupportType);
            return obj as IStoragePointSupport;
        }

        public string StoragePointAvailable(string inputXml)
        {
            return Convert.ToString(OMAdapter.CheckStoragePointAvailable());
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            bool flag = false;
            bool.TryParse(this.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }

            object profileConfiguration = this.StoragePointAdapter.GetProfileConfiguration(sSharePointPath);
            return this.StoragePointAdapter.SerializeProfileConfiguration(profileConfiguration);
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            bool flag = false;
            bool.TryParse(this.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }

            return Convert.ToString(this.StoragePointAdapter.ProfileConfigured(sSharePointPath));
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            bool flag = false;
            bool.TryParse(this.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }

            object oProfile =
                this.StoragePointAdapter.ConfigureFileShareEndpointAndProfile(sNetworkPath, sSharePointPath);
            return this.StoragePointAdapter.SerializeProfileConfiguration(oProfile);
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, AddDocumentOptions options)
        {
            bool flag = false;
            bool.TryParse(this.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }

            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists[new Guid(sListID)];
                if (sPList == null)
                {
                    throw new Exception("The list with ID: '" + sListID + "' does not exist in on the site: " +
                                        web.Url);
                }

                string text = sPList.ParentWeb.Url + "/" + sPList.Title + sFolder;
                object profileConfiguration = this.StoragePointAdapter.GetProfileConfiguration(text);
                if (profileConfiguration == null)
                {
                    throw new Exception("Could not load StoragePoint profile for list or folder '" + text + "'.");
                }

                object obj = this.StoragePointAdapter.FindFileSystemEndpointForPath(profileConfiguration, sNetworkPath);
                if (obj == null)
                {
                    throw new Exception("Could not find a File Share Endpoint");
                }

                object obj2 = this.StoragePointAdapter.GenerateBLOBReference(profileConfiguration, obj, sNetworkPath);
                if (obj2 == null)
                {
                    throw new Exception("Failed to generate BLOB ref for '" + sNetworkPath + "'.");
                }

                byte[] fileContents = new byte[0];
                SPFile sPFile;
                string text2 = this.AddDocument(sListID, sFolder, sListItemXml, fileContents, options, out sPFile);
                if (sPFile == null)
                {
                    throw new Exception("Failed to get document");
                }

                this.StoragePointAdapter.SetBLOBReference(obj2, sPFile.Item);
                result = text2;
            }

            return result;
        }

        private void AddDocumentToStoragePointEndpoint(SPFile file, byte[] contents)
        {
            bool flag = false;
            bool.TryParse(this.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }

            string str = file.ParentFolder.ParentWeb.Url + "/";
            string sFileUrl = str + file.Url;
            object oBlobRef = this.StoragePointAdapter.AddDocumentToEndpoint(sFileUrl, contents);
            this.StoragePointAdapter.SetBLOBReference(oBlobRef, file.Item);
        }

        public string GetTermStores()
        {
            if (Utils.HasTaxonomySupport())
            {
                return this.GetTermStoreCollection();
            }

            return this.GetEmptyTermStoreCollection();
        }

        public string GetTermGroups(string sTermStoreId)
        {
            if (Utils.HasTaxonomySupport())
            {
                return this.GetTermGroupCollection(sTermStoreId);
            }

            return this.GetEmptyTermGroupCollection();
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            if (Utils.HasTaxonomySupport())
            {
                return null;
            }

            return null;
        }

        private string GetTermStoreCollection()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermStoreCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetTermGroupCollection(string sTermStoreId)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermGroupCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyTermStoreCollection()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermStoreCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyTermGroupCollection()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermGroupCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyTermSetCollection()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermSetCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private string GetEmptyTermCollection()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            if (!Utils.HasTaxonomySupport())
            {
                return this.GetEmptyTermSetCollection();
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermSetCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            if (!Utils.HasTaxonomySupport())
            {
                return this.GetEmptyTermCollection();
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            new XmlTextWriter(stringWriter);
            return stringWriter.ToString();
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            if (!Utils.HasTaxonomySupport())
            {
                return this.GetEmptyTermCollection();
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetTermSets(string sTermGroupId)
        {
            if (Utils.HasTaxonomySupport())
            {
                return this.GetTermSetCollection(sTermGroupId);
            }

            return this.GetEmptyTermSetCollection();
        }

        private string GetTermSetCollection(string sTermGroupId)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermSetCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            if (Utils.HasTaxonomySupport())
            {
                return this.GetTermCollectionFromTermSet(sTermSetId, bRecursive);
            }

            return this.GetEmptyTermCollection();
        }

        private string GetTermCollectionFromTermSet(string sTermSetId, bool bRecursive)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            if (Utils.HasTaxonomySupport())
            {
                return this.GetTermCollectionFromTermSetItem(sTermSetItemId);
            }

            return this.GetEmptyTermCollection();
        }

        private string GetTermCollectionFromTermSetItem(string sTermSetItemId)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement(TaxonomyClassType.SPTermCollection.ToString());
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private static void WriteValueAsManagedMetadata(SPField field, string sValue, SPListItem itemToUpdate)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                object value = itemToUpdate[field.InternalName];
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                {
                    return;
                }
            }

            if (!Utils.HasTaxonomySupport())
            {
                return;
            }

            string arg_31_0 = field.TypeAsString;
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXml)
        {
            if (Utils.HasTaxonomySupport())
            {
                return null;
            }

            return null;
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        public string AddTermSet(string termSetXml)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        public string AddTerm(string termXml)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        private static bool CanLoadInfoPath2010ServerDLL()
        {
            Type type = null;
            return type != null;
        }

        internal List<int> GetLanguages(SPWeb web)
        {
            if (web != null)
            {
                List<int> list = new List<int>(web.RegionalSettings.InstalledLanguages.Count);
                list.Add((int)web.Language);
                foreach (SPLanguage sPLanguage in ((IEnumerable)web.RegionalSettings.InstalledLanguages))
                {
                    if (!list.Contains(sPLanguage.LCID))
                    {
                        list.Add(sPLanguage.LCID);
                    }
                }

                return list;
            }

            return new List<int>();
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            byte[] template;
            using (Context context = this.GetContext())
            {
                WebPartTemplateResourceManager webPartTemplateResourceManager =
                    new WebPartTemplateResourceManager(WebPartTemplateResourceLocation.SharePointDirectory,
                        this.GetLanguages(context.Web), base.SharePointVersion, null);
                template = webPartTemplateResourceManager.GetTemplate(iTemplateId);
            }

            return template;
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            byte[] dashboardTemplate;
            using (Context context = this.GetContext())
            {
                WebPartTemplateResourceManager webPartTemplateResourceManager =
                    new WebPartTemplateResourceManager(WebPartTemplateResourceLocation.SharePointDirectory,
                        this.GetLanguages(context.Web), base.SharePointVersion, null);
                dashboardTemplate = webPartTemplateResourceManager.GetDashboardTemplate(iTemplateId);
            }

            return dashboardTemplate;
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("WebPartPage");
            string value = "";
            string value2 = "";
            Utils.ParseUrlForLeafName(sWebPartPageServerRelativeUrl, out value, out value2);
            xmlTextWriter.WriteAttributeString("FileDirRef", value);
            xmlTextWriter.WriteAttributeString("FileLeafRef", value2);
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                string text = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                {
                    '/'
                });
                string documentId = this.GetDocumentId(text, web);
                if (!string.IsNullOrEmpty(documentId))
                {
                    xmlTextWriter.WriteAttributeString("UniqueId", documentId);
                }

                string fileAsString = web.GetFileAsString(text);
                if (!string.IsNullOrEmpty(fileAsString))
                {
                    string text2 = "PageLayoutName";
                    Regex regex = new Regex("<.*PublishingPageLayout.*masterpage/(?<" + text2 + ">[^,<]*).*",
                        RegexOptions.IgnoreCase);
                    MatchCollection matchCollection = regex.Matches(fileAsString);
                    if (matchCollection.Count > 0)
                    {
                        Group group = matchCollection[0].Groups[text2];
                        xmlTextWriter.WriteAttributeString("PageLayout", group.Value);
                    }

                    WebPartUtils.ParseWebPartPageToXml(xmlTextWriter, fileAsString);
                }
            }

            string webPartsOnPage = this.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
            xmlTextWriter.WriteRaw(webPartsOnPage);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            bool value = false;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                try
                {
                    string fullOrRelativeUrl = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPLimitedWebPartManager =
                        web.GetLimitedWebPartManager(fullOrRelativeUrl, PersonalizationScope.Shared);
                    if (sPLimitedWebPartManager != null && sPLimitedWebPartManager.WebParts.Count > 0)
                    {
                        value = true;
                    }
                }
                finally
                {
                    if (sPLimitedWebPartManager != null)
                    {
                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                    }
                }
            }

            return Convert.ToString(value);
        }

        public string GetDocumentId(string sDocUrl)
        {
            string result = null;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                result = this.GetDocumentId(sDocUrl, web);
            }

            return result;
        }

        private string GetDocumentId(string sDocUrl, SPWeb fileHostWeb)
        {
            string result = null;
            SPFile file = fileHostWeb.GetFile(sDocUrl);
            if (file != null && file.Exists)
            {
                result = file.UniqueId.ToString("D").ToUpper();
            }

            return result;
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            string result = "";
            bool flag = false;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                try
                {
                    if (base.SharePointVersion.IsSharePoint2010 && HttpContext.Current == null)
                    {
                        HttpRequest request = new HttpRequest("", web.Url, "");
                        HttpContext.Current = new HttpContext(request, new HttpResponse(new StringWriter()));
                        HttpContext.Current.Items["HttpHandlerSPWeb"] = web;
                        flag = true;
                    }

                    string fullOrRelativeUrl = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPLimitedWebPartManager =
                        web.GetLimitedWebPartManager(fullOrRelativeUrl, PersonalizationScope.Shared);
                    if (sPLimitedWebPartManager != null)
                    {
                        result = this.GetWebPartsOnPage(sPLimitedWebPartManager);
                    }
                }
                finally
                {
                    if (sPLimitedWebPartManager != null)
                    {
                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                        if (flag)
                        {
                            HttpContext.Current = null;
                        }
                    }
                }
            }

            return result;
        }

        private string GetWebPartsOnPage(SPLimitedWebPartManager webPartManager)
        {
            if (webPartManager == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("WebParts");
            foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in webPartManager.WebParts)
            {
                try
                {
                    string text = this.ExportWebPart(webPartManager, webPart);
                    if (string.IsNullOrEmpty(text))
                    {
                        text = this.HttpBasedWebPartExport(webPartManager, webPart);
                    }

                    if (string.IsNullOrEmpty(text))
                    {
                        Type type = webPart.GetType();
                        if (type == null || !type.FullName.Equals("Microsoft.SharePoint.WebPartPages.ErrorWebPart",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(string.Concat(new string[]
                            {
                                "An error has occurred during web part exportation on the page '",
                                webPartManager.ServerRelativeUrl,
                                "'. The web part title and type are: [title=",
                                webPart.Title,
                                ", type=",
                                type.FullName,
                                "]."
                            }));
                        }
                    }
                    else
                    {
                        if (webPart.GetType().FullName == "Microsoft.SharePoint.Portal.WebControls.DateFilterWebPart" &&
                            base.SharePointVersion.IsSharePoint2013OrLater)
                        {
                            string newValue =
                                string.Format(
                                    "<data><properties><property name=\"FilterName\" type=\"string\">{0}</property><property name=\"Title\" type=\"string\">{0}</property></properties></data>",
                                    webPart.Title);
                            text = text.Replace("<data />", newValue);
                        }

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(text);
                        XmlNode xmlNode = null;
                        if (xmlDocument.ChildNodes.Count == 1)
                        {
                            xmlNode = xmlDocument.FirstChild;
                        }
                        else if (xmlDocument.ChildNodes.Count > 1)
                        {
                            xmlNode = xmlDocument.ChildNodes[1];
                        }

                        if (xmlNode != null)
                        {
                            if (xmlNode.Attributes["ID"] == null)
                            {
                                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("ID");
                                xmlAttribute.Value = webPartManager.GetStorageKey(webPart).ToString("D");
                                xmlNode.Attributes.Append(xmlAttribute);
                            }

                            Microsoft.SharePoint.WebPartPages.WebPart webPart2 =
                                webPart as Microsoft.SharePoint.WebPartPages.WebPart;
                            if (webPart2 != null)
                            {
                                XmlElement xmlElement = xmlDocument.CreateElement("ConnectionID");
                                xmlElement.InnerText = webPart2.ConnectionID.ToString();
                                xmlNode.AppendChild(xmlElement);
                                XmlElement xmlElement2 = xmlDocument.CreateElement("Connections");
                                xmlElement2.InnerText = webPart2.Connections;
                                xmlNode.AppendChild(xmlElement2);
                            }

                            XmlElement xmlElement3 = xmlDocument.CreateElement("ZoneID");
                            string zoneID = webPartManager.GetZoneID(webPart);
                            xmlElement3.InnerText = zoneID;
                            xmlNode.AppendChild(xmlElement3);
                            XmlElement xmlElement4 = xmlDocument.CreateElement("PartOrder");
                            xmlElement4.InnerText = webPart.ZoneIndex.ToString();
                            xmlNode.AppendChild(xmlElement4);
                            XmlElement xmlElement5 = xmlDocument.CreateElement("IsIncluded");
                            xmlElement5.InnerText = (!webPart.IsClosed).ToString().ToLower();
                            xmlNode.AppendChild(xmlElement5);
                            XmlElement xmlElement6 = xmlDocument.CreateElement("ID");
                            xmlElement6.InnerText = webPart.ID.ToLower();
                            xmlNode.AppendChild(xmlElement6);
                            XmlElement xmlElement7 = xmlDocument.CreateElement("SharePointSourceVersion");
                            xmlElement7.InnerText = base.SharePointVersion.VersionNumberString;
                            xmlNode.AppendChild(xmlElement7);
                            XmlAttribute xmlAttribute2 = xmlNode.Attributes["Embedded"];
                            if (xmlAttribute2 == null)
                            {
                                xmlAttribute2 = xmlDocument.CreateAttribute("Embedded");
                                xmlNode.Attributes.Append(xmlAttribute2);
                            }

                            xmlAttribute2.Value = string.Equals(zoneID, "wpz", StringComparison.OrdinalIgnoreCase)
                                .ToString();
                            XmlNode xmlNode2 = xmlNode.SelectSingleNode("./*[local-name() = 'ListName']");
                            if (xmlNode2 != null)
                            {
                                try
                                {
                                    Guid uniqueID = new Guid(xmlNode2.InnerText);
                                    xmlNode2.InnerText = webPartManager.Web.Lists[uniqueID].Title;
                                }
                                catch (Exception ex)
                                {
                                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                }
                            }

                            xmlTextWriter.WriteRaw(xmlNode.OuterXml);
                        }
                    }
                }
                catch (WebPartPageUserException ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    if (0 > ex2.Message.IndexOf("Web Part is not on this page", StringComparison.OrdinalIgnoreCase))
                    {
                        throw;
                    }
                }
            }

            this.GetWebPartConnections(webPartManager, xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringBuilder.ToString();
        }

        private void GetWebPartConnections(SPLimitedWebPartManager manager, XmlTextWriter xmlWriter)
        {
            SPWebPartConnectionCollection sPWebPartConnections = manager.SPWebPartConnections;
            xmlWriter.WriteStartElement("webPartConnections");
            foreach (SPWebPartConnection sPWebPartConnection in sPWebPartConnections)
            {
                xmlWriter.WriteStartElement("webPartConnection");
                string iD = sPWebPartConnection.ID;
                xmlWriter.WriteAttributeString("Id", iD);
                string providerID = sPWebPartConnection.ProviderID;
                xmlWriter.WriteAttributeString("ProviderID", providerID);
                string providerConnectionPointID = sPWebPartConnection.ProviderConnectionPointID;
                xmlWriter.WriteAttributeString("ProviderConnectionPointID", providerConnectionPointID);
                string consumerID = sPWebPartConnection.ConsumerID;
                xmlWriter.WriteAttributeString("ConsumerID", consumerID);
                string consumerConnectionPointID = sPWebPartConnection.ConsumerConnectionPointID;
                xmlWriter.WriteAttributeString("ConsumerConnectionPointID", consumerConnectionPointID);
                WebPartTransformer transformer = sPWebPartConnection.Transformer;
                if (transformer != null)
                {
                    xmlWriter.WriteStartElement("webPartTransformer");
                    string fullName = transformer.GetType().FullName;
                    xmlWriter.WriteAttributeString("Type", fullName);
                    if (!(fullName == "Microsoft.SharePoint.WebPartPages.TransformableFilterValuesToFieldTransformer"))
                    {
                        if (fullName ==
                            "Microsoft.SharePoint.WebPartPages.TransformableFilterValuesToParametersTransformer")
                        {
                            TransformableFilterValuesToParametersTransformer
                                transformableFilterValuesToParametersTransformer =
                                    transformer as TransformableFilterValuesToParametersTransformer;
                            if (transformableFilterValuesToParametersTransformer != null)
                            {
                                string[] providerFieldNames =
                                    transformableFilterValuesToParametersTransformer.ProviderFieldNames;
                                string text = "";
                                string[] array = providerFieldNames;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    string arg = array[i];
                                    text = text + arg + ',';
                                }

                                text = text.TrimEnd(new char[]
                                {
                                    ','
                                });
                                xmlWriter.WriteAttributeString("ProviderFieldNames", text);
                                string[] consumerFieldNames =
                                    transformableFilterValuesToParametersTransformer.ConsumerFieldNames;
                                string text2 = "";
                                string[] array2 = consumerFieldNames;
                                for (int j = 0; j < array2.Length; j++)
                                {
                                    string arg2 = array2[j];
                                    text2 = text2 + arg2 + ',';
                                }

                                text2 = text2.TrimEnd(new char[]
                                {
                                    ','
                                });
                                xmlWriter.WriteAttributeString("ConsumerFieldNames", text2);
                            }
                        }
                        else if (fullName ==
                                 "Microsoft.SharePoint.WebPartPages.TransformableFilterValuesToFilterValuesTransformer")
                        {
                            TransformableFilterValuesToFilterValuesTransformer
                                transformableFilterValuesToFilterValuesTransformer =
                                    transformer as TransformableFilterValuesToFilterValuesTransformer;
                            if (transformableFilterValuesToFilterValuesTransformer != null)
                            {
                                string mappedConsumerParameterName = transformableFilterValuesToFilterValuesTransformer
                                    .MappedConsumerParameterName;
                                xmlWriter.WriteAttributeString("MappedConsumerParameterName",
                                    mappedConsumerParameterName);
                            }
                        }
                        else
                        {
                            fullName = "Microsoft.SharePoint.WebPartPages.SPRowToParametersTransformer";
                        }
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        private string ExportWebPart(SPLimitedWebPartManager limitedWebPartManager,
            System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            string result = string.Empty;
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            if (webPart.ExportMode != WebPartExportMode.None)
            {
                try
                {
                    limitedWebPartManager.ExportWebPart(webPart, xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                    result = stringBuilder.ToString();
                    return result;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    return result;
                }
            }

            try
            {
                this.ExportWebPartUsingReflection(limitedWebPartManager, webPart, xmlTextWriter);
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringBuilder.ToString();
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private string HttpBasedWebPartExport(SPLimitedWebPartManager limitedWebPartManager,
            System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            string result = string.Empty;
            try
            {
                string pageUrl = limitedWebPartManager.Web.Site.MakeFullUrl(limitedWebPartManager.ServerRelativeUrl);
                string webPartGuid =
                    Utils.ConvertWebPartIDToGuid(((Microsoft.SharePoint.WebPartPages.WebPart)webPart).StorageKey
                        .ToString());
                string text = WebPartUtils.RetrieveWebPartXmlUsingWebRequest(this.AdapterProxy,
                    this.Credentials.NetworkCredentials, this.IncludedCertificates, this.Url, pageUrl, webPartGuid);
                if (!string.IsNullOrEmpty(text))
                {
                    using (StringReader stringReader = new StringReader(text))
                    {
                        using (StringWriter stringWriter = new StringWriter())
                        {
                            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                            {
                                xmlTextWriter.WriteStartDocument();
                                XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
                                xmlTextReader.MoveToContent();
                                while (!xmlTextReader.EOF)
                                {
                                    xmlTextWriter.WriteNode(xmlTextReader, false);
                                    xmlTextReader.Read();
                                }

                                xmlTextWriter.Flush();
                                xmlTextWriter.Close();
                                result = stringWriter.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private void ExportWebPartUsingReflection(SPLimitedWebPartManager limitedWpManager,
            System.Web.UI.WebControls.WebParts.WebPart wp, XmlTextWriter wpWriter)
        {
            PropertyInfo property = limitedWpManager.GetType()
                .GetProperty("WebPartManager", BindingFlags.Instance | BindingFlags.NonPublic);
            Type propertyType = property.PropertyType;
            SPWebPartManager wpManager = (SPWebPartManager)property.GetValue(limitedWpManager, null);
            Assembly assembly = Assembly.GetAssembly(typeof(SPWeb));
            Type type = assembly.GetType("Microsoft.SharePoint.WebPartPages.SerializationTarget");
            MethodInfo method = propertyType.GetMethod("GetEffectiveWebPartType",
                BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
                {
                    typeof(Type),
                    type
                }, null);
            int num = (int)method.Invoke(null, new object[]
            {
                wp.GetType(),
                Enum.Parse(type, "Export")
            });
            Type type2 = assembly.GetType("Microsoft.SharePoint.WebPartPages.EffectiveWebPartType");
            int num2 = (int)Enum.Parse(type2, "SharePoint");
            if (num2 != num)
            {
                this.ExportAspStyleWebPart(wp, limitedWpManager, wpManager, wpWriter);
                return;
            }

            this.ExportSharePointStyleWebPart(wp, wpManager, wpWriter);
        }

        private void ExportAspStyleWebPart(System.Web.UI.WebControls.WebParts.WebPart wp,
            SPLimitedWebPartManager limitedWpManager, SPWebPartManager wpManager, XmlTextWriter wpWriter)
        {
            bool flag = wp.ExportMode == WebPartExportMode.NonSensitiveData &&
                        limitedWpManager.Scope != PersonalizationScope.Shared;
            wpWriter.WriteStartElement("webParts");
            wpWriter.WriteStartElement("webPart");
            wpWriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/WebPart/v3");
            wpWriter.WriteStartElement("metaData");
            wpWriter.WriteStartElement("type");
            MethodInfo method = wp.GetType().GetMethod("ToControl", BindingFlags.Instance | BindingFlags.NonPublic);
            Control control = (Control)method.Invoke(wp, null);
            UserControl userControl = control as UserControl;
            bool flag2 = true;
            if (userControl != null)
            {
                wpWriter.WriteAttributeString("src", userControl.AppRelativeVirtualPath);
                flag2 = false;
            }

            if (flag2)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(System.Web.UI.WebControls.WebParts.WebPart));
                Type type = assembly.GetType("System.Web.UI.WebControls.WebParts.WebPartUtil");
                MethodInfo method2 = type.GetMethod("SerializeType", BindingFlags.Static | BindingFlags.NonPublic);
                string value = (string)method2.Invoke(null, new object[]
                {
                    control.GetType()
                });
                wpWriter.WriteAttributeString("name", value);
            }

            wpWriter.WriteEndElement();
            wpWriter.WriteElementString("importErrorMessage", wp.ImportErrorMessage);
            wpWriter.WriteEndElement();
            wpWriter.WriteStartElement("data");
            MethodInfo method3 = typeof(PersonalizableAttribute).GetMethod("GetPersonalizablePropertyValues",
                BindingFlags.Static | BindingFlags.NonPublic);
            IDictionary dictionary = (IDictionary)method3.Invoke(null, new object[]
            {
                wp,
                PersonalizationScope.Shared,
                flag
            });
            wpWriter.WriteStartElement("properties");
            MethodInfo method4 = wpManager.GetType().BaseType
                .GetMethod("ExportIPersonalizable", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method5 = wpManager.GetType().BaseType.GetMethod("ExportToWriter",
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
                {
                    typeof(IDictionary),
                    typeof(XmlWriter)
                }, null);
            if (wp is GenericWebPart)
            {
                method4.Invoke(wpManager, new object[]
                {
                    wpWriter,
                    control,
                    flag
                });
                IDictionary dictionary2 = (IDictionary)method3.Invoke(null, new object[]
                {
                    control,
                    PersonalizationScope.Shared,
                    flag
                });
                method5.Invoke(wpManager, new object[]
                {
                    dictionary2,
                    wpWriter
                });
                wpWriter.WriteEndElement();
                wpWriter.WriteStartElement("genericWebPartProperties");
                method4.Invoke(wpManager, new object[]
                {
                    wpWriter,
                    wp,
                    flag
                });
                method5.Invoke(wpManager, new object[]
                {
                    dictionary,
                    wpWriter
                });
            }
            else
            {
                method4.Invoke(wpManager, new object[]
                {
                    wpWriter,
                    wp,
                    flag
                });
                method5.Invoke(wpManager, new object[]
                {
                    dictionary,
                    wpWriter
                });
            }

            wpWriter.WriteEndElement();
            wpWriter.WriteEndElement();
            wpWriter.WriteEndElement();
            wpWriter.WriteEndElement();
        }

        private void ExportSharePointStyleWebPart(System.Web.UI.WebControls.WebParts.WebPart wp,
            SPWebPartManager wpManager, XmlWriter wpWriter)
        {
            MethodInfo method = wpManager.GetType().GetMethod("GetWebPartXml",
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
                {
                    typeof(System.Web.UI.WebControls.WebParts.WebPart),
                    typeof(bool)
                }, null);
            string s = (string)method.Invoke(wpManager, new object[]
            {
                wp,
                true
            });
            using (StringReader stringReader = new StringReader(s))
            {
                wpWriter.WriteStartDocument();
                XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
                xmlTextReader.MoveToContent();
                while (!xmlTextReader.EOF)
                {
                    wpWriter.WriteNode(xmlTextReader, false);
                    xmlTextReader.Read();
                }
            }
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageServerRelativeUrl,
            string sEmbeddedHtmlContent)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebPartsXml);
            bool attributeValueAsBoolean =
                xmlNode.GetAttributeValueAsBoolean(XmlAttributeNames.ListViewToXsltViewConversion.ToString());
            XmlNode xmlNode2 = xmlNode.SelectSingleNode("//*[name() = 'webPartConnections']");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (xmlNode2 != null)
            {
                xmlNode.RemoveChild(xmlNode2);
            }

            if (xmlNode.ChildNodes.Count <= 0)
            {
                return string.Empty;
            }

            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                SPFile sPFile = null;
                bool flag = false;
                string comment = string.Empty;
                SPModerationInformation sPModerationInformation = null;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                int vid = -1;
                string text = "";
                string value = "";
                SPUser sPUser = null;
                DateTime dateTime = default(DateTime);
                try
                {
                    string text2 = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPFile = web.GetFile(HttpUtility.UrlPathEncode(text2));
                    if (sPFile.InDocumentLibrary && sPFile.Item != null)
                    {
                        bool flag6 = OMAdapter.SupportsPublishing && this.IsPublishingPage(sPFile.Item);
                        if (sPFile.Item.ModerationInformation != null)
                        {
                            sPModerationInformation = sPFile.Item.ModerationInformation;
                            comment = sPFile.Item.ModerationInformation.Comment;
                            flag = (sPFile.Item.ModerationInformation.Status == SPModerationStatusType.Approved);
                        }

                        object obj = sPFile.Item[SPBuiltInFieldId.Editor];
                        value = ((obj == null) ? null : obj.ToString());
                        dateTime = DateTime.Parse(sPFile.Item[SPBuiltInFieldId.Modified].ToString());
                        if (sPFile.Item.Versions.Count > 0)
                        {
                            sPUser = sPFile.Item.Versions[0].CreatedBy.User;
                        }

                        if (sPFile.Item.ParentList.ForceCheckout)
                        {
                            if (sPFile.Level == SPFileLevel.Published)
                            {
                                text = sPFile.CheckInComment;
                                if (!sPFile.Item.ParentList.EnableVersioning)
                                {
                                    sPFile.Item.ParentList.EnableVersioning = true;
                                    flag3 = true;
                                }

                                if (!sPFile.Item.ParentList.EnableMinorVersions)
                                {
                                    sPFile.Item.ParentList.EnableMinorVersions = true;
                                    sPFile.Item.ParentList.Update();
                                    flag2 = true;
                                }

                                sPFile.UnPublish("");
                                flag5 = true;
                                vid = sPFile.UIVersion;
                            }

                            this.CheckoutPage(sPFile, web);
                        }
                        else if (sPFile.Item.ParentList.EnableMinorVersions)
                        {
                            SharePointVersion sharePointVersion = new SharePointVersion();
                            XmlNode xmlNode3 = xmlNode.ChildNodes[0]
                                .SelectSingleNode("//*[name() = 'SharePointSourceVersion']");
                            if (xmlNode3 != null)
                            {
                                try
                                {
                                    sharePointVersion = new SharePointVersion(xmlNode3.InnerText);
                                }
                                catch (Exception ex)
                                {
                                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                    sharePointVersion = new SharePointVersion();
                                }
                            }

                            if (flag6 && sharePointVersion != null && sharePointVersion.IsSharePoint2007 &&
                                base.SharePointVersion.IsSharePoint2010OrLater && sPFile.Level == SPFileLevel.Published)
                            {
                                text = sPFile.CheckInComment;
                                sPFile.UnPublish("");
                                flag5 = true;
                                vid = sPFile.UIVersion;
                                this.CheckoutPage(sPFile, web);
                            }
                        }
                    }

                    sPLimitedWebPartManager = web.GetLimitedWebPartManager(text2, PersonalizationScope.Shared);
                    Dictionary<Guid, string> dictionary2 = new Dictionary<Guid, string>();
                    bool flag7 = base.SharePointVersion.IsSharePoint2010OrLater && this.SupportsEmbedding(sPFile);
                    List<string> list = null;
                    if (flag7)
                    {
                        string[] webPartZones = this.GetWebPartZones(sPFile);
                        list = new List<string>();
                        if (webPartZones != null)
                        {
                            string[] array = webPartZones;
                            for (int i = 0; i < array.Length; i++)
                            {
                                string text3 = array[i];
                                list.Add(text3.ToLower());
                            }
                        }

                        this.SetEmbeddedWebPartFieldValue(sPFile, sEmbeddedHtmlContent);
                    }

                    bool flag8 = flag7 && (list == null || list.Count == 0);
                    string text4 = "";
                    List<OMAdapter.WebPartToEmbed> list2 = new List<OMAdapter.WebPartToEmbed>();
                    using (new OMAdapterHttpContext(web))
                    {
                        foreach (XmlNode xmlNode4 in xmlNode.ChildNodes)
                        {
                            try
                            {
                                bool flag9 = xmlNode4.Attributes["Embedded"] != null &&
                                             bool.Parse(xmlNode4.Attributes["Embedded"].Value);
                                XmlNode xmlNode5 = xmlNode4.SelectSingleNode(".//*[name() = 'ZoneID']");
                                string text5 = (xmlNode5 != null) ? xmlNode5.InnerText : null;
                                XmlNode xmlNode6 = xmlNode4.SelectSingleNode(".//*[name() = 'ID']");
                                string text6 = "";
                                if (xmlNode6 != null)
                                {
                                    text6 = Utils.ConvertWebPartIDToGuid(xmlNode6.InnerText.ToLower());
                                }

                                bool flag10 = flag8 || (flag7 && (flag9 || !list.Contains(text5.ToLower())));
                                int? partOrder = null;
                                string sZoneId = text5;
                                if (flag10)
                                {
                                    partOrder = new int?(0);
                                    sZoneId = "wpz";
                                }

                                Guid guid;
                                this.AddWebPart(ref sPLimitedWebPartManager, xmlNode4.OuterXml, sZoneId, partOrder,
                                    ref dictionary2, out guid, attributeValueAsBoolean);
                                string text7 = guid.ToString("D").ToUpper();
                                if (flag10)
                                {
                                    bool flag11 = false;
                                    if (flag9 && !string.IsNullOrEmpty(text6))
                                    {
                                        flag11 = this.UpdateEmbeddedWebPartReference(sPFile, text6, text7);
                                    }

                                    if (!flag11)
                                    {
                                        list2.Add(new OMAdapter.WebPartToEmbed(text7, text5));
                                    }
                                }

                                if (!string.IsNullOrEmpty(text6) && !dictionary.ContainsKey(text6.ToLowerInvariant()))
                                {
                                    dictionary.Add(text6.ToLowerInvariant(), text7.ToLowerInvariant());
                                }
                            }
                            catch (Exception ex2)
                            {
                                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                text4 = text4 + ex2.Message + "\n";
                            }
                        }
                    }

                    if (flag7 && list2.Count > 0)
                    {
                        string text8 = "";
                        this.EmbedWebParts(sPFile, list2, out text8);
                        if (!string.IsNullOrEmpty(text8))
                        {
                            text4 = text4 + text8 + "\n";
                        }
                    }

                    if (dictionary2.Count > 0 || xmlNode2 != null)
                    {
                        SPLimitedWebPartManager sPLimitedWebPartManager2 = null;
                        try
                        {
                            sPLimitedWebPartManager2 = web.GetLimitedWebPartManager(text2, PersonalizationScope.Shared);
                            foreach (Guid current in dictionary2.Keys)
                            {
                                System.Web.UI.WebControls.WebParts.WebPart webPart =
                                    sPLimitedWebPartManager2.WebParts[current];
                                webPart.Title = dictionary2[current];
                                sPLimitedWebPartManager2.SaveChanges(webPart);
                            }

                            if (xmlNode2 != null)
                            {
                                this.ConnectWebParts(sPLimitedWebPartManager2, xmlNode2, dictionary);
                            }
                        }
                        finally
                        {
                            if (sPLimitedWebPartManager2 != null)
                            {
                                sPLimitedWebPartManager2.Web.Dispose();
                                sPLimitedWebPartManager2.Dispose();
                                sPLimitedWebPartManager2 = null;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(text4))
                    {
                        throw new Exception(string.Concat(new string[]
                        {
                            "One or more web parts encountered problems while being added to page '",
                            this.Server,
                            text2,
                            "'. These web parts may not appear on the page or may appear inaccurately. Error messages: ",
                            text4
                        }));
                    }
                }
                finally
                {
                    try
                    {
                        if (flag)
                        {
                            if (sPFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                            {
                                this.CheckInFileByUser(sPFile, text, SPCheckinType.OverwriteCheckIn, sPUser, false,
                                    string.Empty);
                            }

                            sPFile.Item.ModerationInformation.Status = SPModerationStatusType.Approved;
                            if (base.SharePointVersion.IsSharePoint2013OrLater)
                            {
                                sPFile.Item[SPBuiltInFieldId.Editor] = value;
                                sPFile.Item[SPBuiltInFieldId.Modified] = dateTime;
                                sPFile.Item.ModerationInformation.Comment = comment;
                                this.CallSystemUpdateApprovalVersion(sPFile.Item.ParentList.ParentWeb, sPFile.Item);
                            }
                            else
                            {
                                sPFile.Item.UpdateOverwriteVersion();
                                sPFile.Item[SPBuiltInFieldId.Editor] = value;
                                sPFile.Item[SPBuiltInFieldId.Modified] = dateTime;
                                sPFile.Item.ModerationInformation.Comment = comment;
                                OMAdapter.CallSystemUpdateItem(sPFile.Item.ParentList.ParentWeb, sPFile.Item);
                            }
                        }

                        if (sPFile != null && sPFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                        {
                            if (flag4)
                            {
                                this.CheckInPage(sPFile, SPCheckinType.MajorCheckIn,
                                    "Checkout required by Content Matrix Console in order to copy web parts.");
                            }
                            else if (flag5)
                            {
                                sPFile.Item[SPBuiltInFieldId.Editor] = value;
                                sPFile.Item[SPBuiltInFieldId.Modified] = dateTime;
                                if (sPFile.Item.ModerationInformation != null)
                                {
                                    sPFile.Item.ModerationInformation.Comment = comment;
                                }

                                sPFile.Item.Update();
                                this.CheckInFileByUser(sPFile, text, SPCheckinType.MajorCheckIn, sPUser);
                                sPFile.Versions.DeleteByID(vid);
                            }
                            else
                            {
                                this.CheckInPage(sPFile, SPCheckinType.OverwriteCheckIn, null);
                                sPFile.Item[SPBuiltInFieldId.Editor] = value;
                                sPFile.Item[SPBuiltInFieldId.Modified] = dateTime;
                                if (sPFile.Item.ModerationInformation != null)
                                {
                                    sPFile.Item.ModerationInformation.Comment = comment;
                                }

                                sPFile.Item.UpdateOverwriteVersion();
                            }
                        }

                        if (sPModerationInformation != null &&
                            sPModerationInformation.Status != sPFile.Item.ModerationInformation.Status)
                        {
                            sPFile.Item.ModerationInformation.Status = sPModerationInformation.Status;
                            sPFile.Item.ModerationInformation.Comment = sPModerationInformation.Comment;
                            sPFile.Item.SystemUpdate(false);
                        }

                        if (sPLimitedWebPartManager != null)
                        {
                            sPLimitedWebPartManager.Web.Dispose();
                            sPLimitedWebPartManager.Dispose();
                            sPLimitedWebPartManager = null;
                        }
                    }
                    finally
                    {
                        if (flag2)
                        {
                            sPFile.Item.ParentList.EnableMinorVersions = false;
                        }

                        if (flag3)
                        {
                            sPFile.Item.ParentList.EnableVersioning = false;
                        }

                        if (flag2 || flag3)
                        {
                            sPFile.Item.ParentList.Update();
                        }
                    }
                }
            }

            return string.Empty;
        }

        private void ConnectWebParts(SPLimitedWebPartManager webPartManager, XmlNode connectionsNode,
            Dictionary<string, string> map)
        {
            foreach (XmlNode xmlNode in connectionsNode.ChildNodes)
            {
                try
                {
                    string text = xmlNode.Attributes["ProviderID"].Value.TrimStart(new char[]
                    {
                        'g',
                        '_'
                    }).Replace('_', '-');
                    System.Web.UI.WebControls.WebParts.WebPart webPartById;
                    if (map.ContainsKey(text.ToLowerInvariant()))
                    {
                        webPartById = this.GetWebPartById(webPartManager, map[text]);
                    }
                    else
                    {
                        webPartById = this.GetWebPartById(webPartManager, text);
                    }

                    string text2 = xmlNode.Attributes["ConsumerID"].Value.TrimStart(new char[]
                    {
                        'g',
                        '_'
                    }).Replace('_', '-');
                    System.Web.UI.WebControls.WebParts.WebPart webPartById2;
                    if (map.ContainsKey(text2.ToLowerInvariant()))
                    {
                        webPartById2 = this.GetWebPartById(webPartManager, map[text2]);
                    }
                    else
                    {
                        webPartById2 = this.GetWebPartById(webPartManager, text2);
                    }

                    string value = xmlNode.Attributes["ProviderConnectionPointID"].Value;
                    ProviderConnectionPoint providerConnectionPoint =
                        webPartManager.GetProviderConnectionPoints(webPartById)[value];
                    string value2 = xmlNode.Attributes["ConsumerConnectionPointID"].Value;
                    ConsumerConnectionPoint consumerConnectionPoint =
                        webPartManager.GetConsumerConnectionPoints(webPartById2)[value2];
                    XmlNode xmlNode2 = xmlNode.SelectSingleNode("webPartTransformer");
                    if (xmlNode2 != null)
                    {
                        string value3 = xmlNode2.Attributes["Type"].Value;
                        if (value3 ==
                            "Microsoft.SharePoint.WebPartPages.TransformableFilterValuesToParametersTransformer")
                        {
                            webPartManager.SPConnectWebParts(webPartById, providerConnectionPoint, webPartById2,
                                consumerConnectionPoint, new TransformableFilterValuesToParametersTransformer
                                {
                                    ProviderFieldNames = new string[]
                                    {
                                        xmlNode2.Attributes["ProviderFieldNames"].Value
                                    },
                                    ConsumerFieldNames = new string[]
                                    {
                                        xmlNode2.Attributes["ConsumerFieldNames"].Value
                                    }
                                });
                        }
                        else if (value3 ==
                                 "Microsoft.SharePoint.WebPartPages.TransformableFilterValuesToFilterValuesTransformer")
                        {
                            webPartManager.SPConnectWebParts(webPartById, providerConnectionPoint, webPartById2,
                                consumerConnectionPoint, new TransformableFilterValuesToFilterValuesTransformer
                                {
                                    MappedConsumerParameterName =
                                        xmlNode2.Attributes["MappedConsumerParameterName"].Value
                                });
                        }
                        else if (value3 == "Microsoft.SharePoint.WebPartPages.SPRowToParametersTransformer")
                        {
                        }
                    }
                    else
                    {
                        webPartManager.SPConnectWebParts(webPartById, providerConnectionPoint, webPartById2,
                            consumerConnectionPoint);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        private static TransformableFilterValuesToParametersTransformer
            ConstructTransformableFilterValuesToParametersTransformer(string consumerFieldName,
                string providerFieldName)
        {
            TransformableFilterValuesToParametersTransformer transformableFilterValuesToParametersTransformer =
                new TransformableFilterValuesToParametersTransformer();
            try
            {
                string[] value = new string[]
                {
                    consumerFieldName
                };
                FieldInfo field =
                    typeof(TransformableFilterValuesToParametersTransformer).GetField("_consumerFieldNames",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(transformableFilterValuesToParametersTransformer, value);
                string[] value2 = new string[]
                {
                    providerFieldName
                };
                FieldInfo field2 =
                    typeof(TransformableFilterValuesToParametersTransformer).GetField("_providerFieldNames",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                field2.SetValue(transformableFilterValuesToParametersTransformer, value2);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception(
                    "Unexpected internal definition for 'TransformableFilterValuesToParametersTransformer'. Please amend private member access code.");
            }

            return transformableFilterValuesToParametersTransformer;
        }

        private System.Web.UI.WebControls.WebParts.WebPart GetWebPartById(SPLimitedWebPartManager webPartManager,
            string webPartId)
        {
            System.Web.UI.WebControls.WebParts.WebPart result = null;
            foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in webPartManager.WebParts)
            {
                if (string.Equals(webPart.ID.TrimStart(new char[]
                    {
                        'g',
                        '_'
                    }).Replace("_", ""), webPartId.Replace("-", "")))
                {
                    result = webPart;
                    break;
                }
            }

            return result;
        }

        private static string GetWebPartType(XmlDocument xmldoc)
        {
            string result = string.Empty;
            if (xmldoc.DocumentElement != null)
            {
                XmlNodeList elementsByTagName = xmldoc.DocumentElement.GetElementsByTagName("type");
                if (elementsByTagName.Count < 1)
                {
                    elementsByTagName = xmldoc.DocumentElement.GetElementsByTagName("TypeName");
                    if (elementsByTagName.Item(0) != null && elementsByTagName.Count > 0)
                    {
                        result = elementsByTagName.Item(0).InnerText;
                    }
                }
                else
                {
                    XmlNode xmlNode = elementsByTagName.Item(0);
                    if (xmlNode != null && xmlNode.Attributes != null && xmlNode.Attributes["name"] != null)
                    {
                        string value = xmlNode.Attributes["name"].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            result = value.Split(new char[]
                            {
                                ','
                            })[0];
                        }
                    }
                }
            }

            return result;
        }

        private static object ParseWebPartPropertyToCorrectType(string typeReadFromXml, string propertyValue)
        {
            switch (typeReadFromXml)
            {
                case "string":
                    return propertyValue;
                case "int":
                    return int.Parse(propertyValue);
                case "single":
                    return float.Parse(propertyValue);
                case "double":
                    return double.Parse(propertyValue);
                case "bool":
                    return bool.Parse(propertyValue.ToLower());
                case "datetime":
                    return DateTime.Parse(propertyValue);
                case "color":
                    return Color.FromName(propertyValue);
                case "unit":
                    return Unit.Parse(propertyValue);
                case "object":
                    return propertyValue;
                case "direction":
                    return Enum.Parse(typeof(ContentDirection), propertyValue, true);
                case "helpmode":
                    return Enum.Parse(typeof(WebPartHelpMode), propertyValue, true);
                case "chromestate":
                    return Enum.Parse(typeof(PartChromeState), propertyValue, true);
                case "chrometype":
                    return Enum.Parse(typeof(PartChromeType), propertyValue, true);
                case "exportmode":
                    return Enum.Parse(typeof(WebPartExportMode), propertyValue, true);
            }

            return null;
        }

        private static Microsoft.SharePoint.WebPartPages.WebPart GetCustomWebPart(string webPartType,
            XmlDocument xmlDoc)
        {
            if (webPartType == null || !(webPartType == "Microsoft.Office.Excel.WebUI.ExcelWebRenderer"))
            {
                return null;
            }

            if (!OMAdapter.SupportsExcelWebAccessService)
            {
                return null;
            }

            return OMAdapter.CreateExcelWebAccessWebPart(xmlDoc);
        }

        private static Microsoft.SharePoint.WebPartPages.WebPart CreateExcelWebAccessWebPart(XmlDocument xmlDoc)
        {
            if (xmlDoc == null || xmlDoc.DocumentElement == null)
            {
                throw new ArgumentNullException();
            }

            ExcelWebRenderer excelWebRenderer = new ExcelWebRenderer();
            XmlNodeList xmlNodeList = xmlDoc.DocumentElement.SelectNodes("//*[local-name()='property']");
            PropertyInfo[] properties = excelWebRenderer.GetType().GetProperties();
            Dictionary<string, object> xmlPropertyNamesandValues = new Dictionary<string, object>();
            string[] source = new string[]
            {
                "TitleUrl"
            };
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string value = xmlNode.Attributes["type"].Value;
                object value2 = OMAdapter.ParseWebPartPropertyToCorrectType(value, xmlNode.InnerText);
                xmlPropertyNamesandValues.Add(xmlNode.Attributes["name"].Value, value2);
            }

            try
            {
                foreach (PropertyInfo current in from propertyInfo in properties
                         where xmlPropertyNamesandValues.ContainsKey(propertyInfo.Name)
                         select propertyInfo)
                {
                    object obj = xmlPropertyNamesandValues[current.Name];
                    if (obj != null && current.PropertyType == obj.GetType() && !source.Contains(current.Name))
                    {
                        current.SetValue(excelWebRenderer, obj, null);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return excelWebRenderer;
        }

        private static void LoadExcelWebUIDLL()
        {
            OMAdapter._excelWebUi = typeof(ExcelWebRenderer);
        }

        private string AddWebPart(ref SPLimitedWebPartManager webPartManager, string sWebPartXml, string sZoneId,
            int? partOrder, ref Dictionary<Guid, string> webPartTitleUpdates, out Guid wpIdProperty,
            bool isListViewToXsltViewConversion)
        {
            string result = null;
            wpIdProperty = Guid.NewGuid();
            string iD = this.StorageKeyToID(wpIdProperty);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sWebPartXml);
            if (string.IsNullOrEmpty(sZoneId))
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[name() = 'ZoneID']");
                if (xmlNode != null)
                {
                    sZoneId = xmlNode.InnerText;
                }

                if (string.IsNullOrEmpty(sZoneId))
                {
                    sZoneId = "Left";
                }
            }

            int zoneIndex = 0;
            if (partOrder.HasValue)
            {
                zoneIndex = partOrder.Value;
            }
            else
            {
                XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//*[name() = 'PartOrder']");
                if (xmlNode2 != null && !int.TryParse(xmlNode2.InnerText, out zoneIndex))
                {
                    throw new WarningException("A web part does not have a valid part order on page: '" +
                                               webPartManager.Web.Url +
                                               "'. The ordering of web parts on this page may be different on the target.");
                }
            }

            XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//*[contains(@name,'BrowserFormWebPart')]");
            if (xmlNode3 != null && webPartManager.WebParts.Count > 0)
            {
                System.Web.UI.WebControls.WebParts.WebPart webPart = webPartManager.WebParts[0];
                if (webPart.GetType().Name.Equals("BrowserFormWebPart", StringComparison.InvariantCultureIgnoreCase))
                {
                    webPartManager.MoveWebPart(webPart, sZoneId, zoneIndex);
                    webPartManager.SaveChanges(webPart);
                    return webPart.ID;
                }
            }

            bool? flag = null;
            XmlNode xmlNode4 = xmlDocument.SelectSingleNode("//*[name() = 'IsIncluded']");
            if (xmlNode4 != null)
            {
                bool flag2 = true;
                if (bool.TryParse(xmlNode4.InnerText, out flag2))
                {
                    flag = new bool?(!flag2);
                }
            }

            SharePointVersion sharePointVersion = new SharePointVersion();
            XmlNode xmlNode5 = xmlDocument.SelectSingleNode("//*[name() = 'SharePointSourceVersion']");
            if (xmlNode5 != null)
            {
                try
                {
                    sharePointVersion = new SharePointVersion(xmlNode5.InnerText);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    sharePointVersion = new SharePointVersion();
                }

                xmlNode5.ParentNode.RemoveChild(xmlNode5);
            }

            XmlNode xmlNode6 = xmlDocument.SelectSingleNode("//*[name() = 'Title']");
            string text = null;
            if (xmlNode6 != null)
            {
                text = xmlNode6.InnerText;
            }

            XmlNode xmlNode7 = xmlDocument.SelectSingleNode("//*[name() = 'webPart']");
            if (xmlNode7 != null)
            {
                xmlDocument.LoadXml("<webParts>" + xmlNode7.OuterXml + "</webParts>");
            }

            XmlNode xmlNode8 = xmlDocument.SelectSingleNode("//*[name() = 'ListName']");
            bool bReferencesBlogPostsList = false;
            if (xmlNode8 != null && !xmlNode8.InnerText.StartsWith("{", StringComparison.Ordinal))
            {
                string innerText = xmlNode8.InnerText;
                string text2 = null;
                foreach (SPList sPList in ((IEnumerable)webPartManager.Web.Lists))
                {
                    if (sPList.Title != null && sPList.Title.Equals(innerText, StringComparison.OrdinalIgnoreCase))
                    {
                        text2 = "{" + sPList.ID.ToString().ToUpper() + "}";
                        bReferencesBlogPostsList = (sPList.BaseTemplate == SPListTemplateType.Posts);
                        break;
                    }
                }

                if (text2 == null)
                {
                    throw new Exception("Cannot add web part. The list: '" + innerText +
                                        "' does not exist on the target site. ");
                }

                xmlNode8.InnerText = text2;
                XmlNode xmlNode9 = xmlDocument.SelectSingleNode("//*[name()='ListId']");
                if (!string.IsNullOrEmpty(text2) && xmlNode9 != null)
                {
                    xmlNode9.InnerText = text2.Trim(new char[]
                    {
                        '{',
                        '}'
                    }).ToLower();
                }
            }

            bool flag3 = false;
            string text3 = null;
            XmlNode xmlNode10 = null;
            SPViewCollection.SPViewType sPViewType = SPViewCollection.SPViewType.Html;
            XmlNode xmlNode11 = null;
            SPView sPView = null;
            if (xmlNode7 != null)
            {
                xmlNode11 = xmlDocument.SelectSingleNode("//*[@name='XmlDefinition']");
            }
            else
            {
                xmlNode11 = xmlDocument.SelectSingleNode("//*[name() = 'ListViewXml']");
            }

            int num = 0;
            if (xmlNode11 != null && !string.IsNullOrEmpty(xmlNode11.InnerText))
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(xmlNode11.InnerText);
                if (xmlDocument2.DocumentElement != null && xmlDocument2.DocumentElement.Name == "View")
                {
                    xmlNode10 = xmlDocument2.FirstChild;
                    if (xmlDocument2.DocumentElement.Attributes["Type"] != null)
                    {
                        this.MakeViewAdjustments(xmlNode10, sharePointVersion, bReferencesBlogPostsList);
                        text3 = xmlNode10.InnerXml;
                        string value = xmlNode10.Attributes["Type"].Value;
                        sPViewType =
                            (SPViewCollection.SPViewType)Enum.Parse(typeof(SPViewCollection.SPViewType), value, true);
                        XmlNodeList xmlNodeList = xmlNode10.SelectNodes("./ViewFields/FieldRef");
                        foreach (XmlNode xmlNode12 in xmlNodeList)
                        {
                            if (xmlNode12.Attributes["Explicit"] != null && xmlNode12.Attributes["Explicit"].Value
                                    .Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                if (xmlNode10.Attributes["BaseViewID"] != null &&
                                    xmlNode10.Attributes["BaseViewID"].Value == "0")
                                {
                                    flag3 = true;
                                    break;
                                }

                                break;
                            }
                        }

                        xmlNode11.ParentNode.RemoveChild(xmlNode11);
                    }

                    if (xmlDocument2.DocumentElement.Attributes["BaseViewID"] != null)
                    {
                        num = ((xmlNode10.Attributes["BaseViewID"] != null)
                            ? Convert.ToInt32(xmlNode10.Attributes["BaseViewID"].Value)
                            : 0);
                    }
                }
            }

            System.Web.UI.WebControls.WebParts.WebPart webPart2 = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(xmlDocument.OuterXml)))
            {
                try
                {
                    string webPartType = OMAdapter.GetWebPartType(xmlDocument);
                    if (OMAdapter.CustomWebPartTypes.Contains(webPartType))
                    {
                        webPart2 = OMAdapter.GetCustomWebPart(webPartType, xmlDocument);
                    }

                    if (webPart2 == null)
                    {
                        string text4;
                        webPart2 = webPartManager.ImportWebPart(xmlReader, out text4);
                    }
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(string.Concat(new string[]
                    {
                        "Web part import failed on page: '",
                        webPartManager.Web.Url,
                        "'. Error: ",
                        ex2.Message,
                        ". Web Part XML:",
                        xmlDocument.OuterXml
                    }));
                }
            }

            if (flag.HasValue && flag.Value)
            {
                webPartManager.CloseWebPart(webPart2);
            }

            webPart2.TitleUrl = HttpUtility.UrlDecode(webPart2.TitleUrl);
            webPart2.HelpUrl = HttpUtility.UrlDecode(webPart2.HelpUrl);
            webPart2.TitleIconImageUrl = HttpUtility.UrlDecode(webPart2.TitleIconImageUrl);
            webPart2.CatalogIconImageUrl = HttpUtility.UrlDecode(webPart2.CatalogIconImageUrl);
            try
            {
                if (xmlNode11 != null && ListViewWebPart.IsListViewWebPart(webPart2))
                {
                    ListViewWebPart listViewWebPart = ListViewWebPart.CreateInstance(webPart2);
                    SPList sPList2 = webPartManager.Web.Lists[new Guid(listViewWebPart.ListName)];
                    if (flag3 && base.SharePointVersion.IsSharePoint2010OrLater &&
                        sPList2.BaseTemplate == SPListTemplateType.Events)
                    {
                        flag3 = false;
                    }

                    if (!flag3)
                    {
                        SPView sPView2 = null;
                        bool flag4 = false;
                        bool flag5 = sharePointVersion.IsSharePoint2003 && num != 1;
                        foreach (SPView sPView3 in ((IEnumerable)sPList2.Views))
                        {
                            if (sPView3.BaseViewID == num.ToString() &&
                                (flag5 || sPView3.Type.ToLower() == sPViewType.ToString().ToLower()))
                            {
                                listViewWebPart.ViewGuid = sPView3.ID.ToString("B").ToUpper();
                                flag4 = true;
                                break;
                            }
                        }

                        if (!flag4)
                        {
                            sPView2 = sPList2.Views.Add("Temp view: " + sPViewType.ToString(), null, null, 0u, false,
                                false, OMAdapter.GetViewTypeValue(sPViewType.ToString()), false);
                            listViewWebPart.ViewGuid = sPView2.ID.ToString("B").ToUpper();
                        }

                        listViewWebPart.WebID = webPartManager.Web.ID;
                        webPart2.ID = iD;
                        webPartManager.AddWebPart(webPart2, sZoneId, zoneIndex);
                        if (sPView2 != null)
                        {
                            sPList2.Views.Delete(sPView2.ID);
                        }
                    }
                    else
                    {
                        webPart2.ID = iD;
                        webPartManager.AddWebPart(webPart2, sZoneId, zoneIndex);
                    }

                    if (!string.IsNullOrEmpty(text3))
                    {
                        bool flag6 = true;
                        try
                        {
                            sPView = listViewWebPart.View;
                        }
                        catch (Exception ex3)
                        {
                            OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                            sPView = null;
                            flag6 = false;
                        }

                        if (sPView == null)
                        {
                            SPList sPList3 = webPartManager.Web.Lists[new Guid(listViewWebPart.ListName)];
                            sPView = sPList3.Views[new Guid(listViewWebPart.ViewGuid)];
                            if (flag6)
                            {
                                listViewWebPart.View = sPView;
                            }
                        }

                        sPView.Title = "";
                        if (xmlNode10 != null)
                        {
                            OMAdapter.AddViewAttributesFromXML(sPView, xmlNode10);
                            sPView.Update();
                        }

                        PropertyInfo property = sPView.GetType()
                            .GetProperty("Node", BindingFlags.Instance | BindingFlags.NonPublic);
                        XmlNode xmlNode13 = property.GetValue(sPView, null) as XmlNode;
                        xmlNode13.InnerXml = text3;
                    }

                    if (text != null)
                    {
                        webPartTitleUpdates.Add(webPartManager.GetStorageKey(webPart2), text);
                    }
                }
                else
                {
                    webPart2.ID = iD;
                    webPartManager.AddWebPart(webPart2, sZoneId, zoneIndex);
                }

                webPartManager.MoveWebPart(webPart2, sZoneId, zoneIndex);
                webPartManager.SaveChanges(webPart2);
                if (sPView != null)
                {
                    sPView.Update();
                }

                result = webPartManager.GetStorageKey(webPart2).ToString("D").ToUpper();
                if (isListViewToXsltViewConversion && sharePointVersion.IsSharePoint2007 &&
                    base.SharePointVersion.IsSharePoint2010OrLater && !webPartManager.Web.WebTemplate.Contains("MPS") &&
                    ListViewWebPart.IsListViewWebPart(webPart2))
                {
                    this.ConvertListViewToXsltListViewWebParts(webPartManager, sWebPartXml, webPart2);
                }
            }
            catch (Exception ex4)
            {
                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception(string.Concat(new string[]
                {
                    "Web part add failed on page: '",
                    webPartManager.Web.Url,
                    "'. Error: ",
                    ex4.Message,
                    ". ",
                    (ex4.InnerException != null) ? ("Inner exception(" + ex4.InnerException.Message + ")") : "",
                    " Web Part XML: ",
                    xmlDocument.OuterXml
                }));
            }

            return result;
        }

        private void ConvertListViewToXsltListViewWebParts(SPLimitedWebPartManager webPartManager, string webPartXml,
            System.Web.UI.WebControls.WebParts.WebPart newWebPart)
        {
            string text = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webPartXml);
            if (xmlDocument.GetElementsByTagName("ListViewXml").Count > 0)
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(xmlDocument.GetElementsByTagName("ListViewXml")[0].InnerText);
                if (xmlDocument2.DocumentElement.Attributes["Type"] != null)
                {
                    text = xmlDocument2.DocumentElement.Attributes["Type"].Value;
                }
            }

            if (!text.Equals("Calendar", StringComparison.InvariantCultureIgnoreCase) &&
                !text.Equals("Gantt", StringComparison.InvariantCultureIgnoreCase))
            {
                PropertyInfo property = newWebPart.GetType().GetProperty("NewWebPartTypeId",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty);
                if (base.SharePointVersion.IsSharePoint2010)
                {
                    property.SetValue(newWebPart, this._xslt_ListView_WebPart_TypeId_SP2010, null);
                }
                else if (base.SharePointVersion.IsSharePoint2013)
                {
                    property.SetValue(newWebPart, this._xslt_ListView_WebPart_TypeId_SP2013, null);
                }
                else if (base.SharePointVersion.IsSharePoint2016OrLater)
                {
                    property.SetValue(newWebPart, this._xslt_ListView_WebPart_TypeId_SP2016, null);
                }

                webPartManager.SaveChanges(newWebPart);
            }
        }

        private void MakeViewAdjustments(XmlNode xmlViewNode, SharePointVersion sourceVersion,
            bool bReferencesBlogPostsList)
        {
            if (xmlViewNode == null)
            {
                return;
            }

            XmlNode xmlNode = xmlViewNode.SelectSingleNode("//Toolbar");
            if (xmlNode == null)
            {
                if (bReferencesBlogPostsList)
                {
                    XmlElement xmlElement = xmlViewNode.OwnerDocument.CreateElement("Toolbar");
                    XmlAttribute xmlAttribute = xmlViewNode.OwnerDocument.CreateAttribute("Type");
                    xmlAttribute.Value = "None";
                    xmlElement.Attributes.Append(xmlAttribute);
                    xmlViewNode.AppendChild(xmlElement);
                    return;
                }
            }
            else if (base.SharePointVersion.IsSharePoint2010OrLater && sourceVersion.IsSharePoint2007OrEarlier &&
                     xmlNode.Attributes["Type"] != null && xmlNode.Attributes["Type"].Value
                         .Equals("Standard", StringComparison.OrdinalIgnoreCase))
            {
                XmlAttribute xmlAttribute2 = xmlNode.Attributes["ShowAlways"];
                if (xmlAttribute2 == null)
                {
                    xmlAttribute2 = xmlViewNode.OwnerDocument.CreateAttribute("ShowAlways");
                }

                xmlAttribute2.Value = "TRUE";
                xmlNode.Attributes.Append(xmlAttribute2);
            }
        }

        private void SetEmbeddedWebPartFieldValue(SPFile wppFile, string sContent)
        {
            if (string.IsNullOrEmpty(sContent) || wppFile == null || wppFile.Item == null)
            {
                return;
            }

            string richTextEmbeddingField = this.GetRichTextEmbeddingField(wppFile);
            if (string.IsNullOrEmpty(richTextEmbeddingField))
            {
                return;
            }

            wppFile.Item[richTextEmbeddingField] = this.CastStringToFieldType(sContent,
                OMAdapter.GetFieldByInternalName(wppFile.Item.Fields, richTextEmbeddingField), null);
        }

        private bool UpdateEmbeddedWebPartReference(SPFile wppFile, string sOldWebPartGuid, string sNewWebPartGuid)
        {
            if (wppFile == null || wppFile.Item == null)
            {
                return false;
            }

            string richTextEmbeddingField = this.GetRichTextEmbeddingField(wppFile);
            if (wppFile.Item[richTextEmbeddingField] != null)
            {
                string text = (string)wppFile.Item[richTextEmbeddingField];
                Regex regex = new Regex(sOldWebPartGuid, RegexOptions.IgnoreCase);
                MatchCollection matchCollection = regex.Matches(text);
                if (matchCollection.Count > 0)
                {
                    foreach (Match match in matchCollection)
                    {
                        text = text.Remove(match.Index, match.Length);
                        text = text.Insert(match.Index, sNewWebPartGuid.ToLower());
                    }

                    wppFile.Item[richTextEmbeddingField] = text;
                    wppFile.Item.SystemUpdate(false);
                    return true;
                }
            }

            return false;
        }

        private void EmbedWebParts(SPFile wppFile, List<OMAdapter.WebPartToEmbed> webPartsToEmbed,
            out string sEmbeddingErrors)
        {
            sEmbeddingErrors = "";
            if (wppFile == null || wppFile.Item == null || webPartsToEmbed.Count <= 0)
            {
                return;
            }

            try
            {
                string richTextEmbeddingField = this.GetRichTextEmbeddingField(wppFile);
                if (wppFile.Item.Properties.ContainsKey(richTextEmbeddingField))
                {
                    string text = (string)wppFile.Item.Properties[richTextEmbeddingField];
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "<div>" + this.GenerateEmbeddedWebPartsHtml(webPartsToEmbed) + "</div>";
                    }
                    else if (0 <= text.IndexOf("ms-rte-layoutszone-inner", StringComparison.OrdinalIgnoreCase))
                    {
                        this.EmbedWebPartsInTeamSite(ref text, webPartsToEmbed);
                    }
                    else
                    {
                        this.EmbedWebPartsInGeneralSite(ref text, webPartsToEmbed);
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        wppFile.Item[richTextEmbeddingField] = text;
                        wppFile.Item.SystemUpdate(false);
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                sEmbeddingErrors = "There was a problem trying to embed web parts in a rich text field. Message: " +
                                   ex.Message;
            }
        }

        private void EmbedWebPartsInTeamSite(ref string sEmbeddedContent,
            List<OMAdapter.WebPartToEmbed> webPartsToEmbed)
        {
            if (webPartsToEmbed.Count <= 0)
            {
                return;
            }

            string text = "";
            string text2 = "";
            foreach (OMAdapter.WebPartToEmbed current in webPartsToEmbed)
            {
                if (0 <= current.Zone.IndexOf("right", StringComparison.OrdinalIgnoreCase))
                {
                    text2 = text2 + this.GetEmbeddedWebpartString(current.Guid) + "\n";
                }
                else
                {
                    text = text + this.GetEmbeddedWebpartString(current.Guid) + "\n";
                }
            }

            try
            {
                XmlNode richTextNode = XmlUtility.StringToXmlNode(sEmbeddedContent);
                sEmbeddedContent = this.AddEmbeddedWebPartReferencesUsingXml(richTextNode, text, text2);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                sEmbeddedContent = this.AddEmbeddedWebPartReferencesUsingRegex(sEmbeddedContent, text, text2);
            }
        }

        private string AddEmbeddedWebPartReferencesUsingRegex(string sEmbeddedContent, string sLeftZoneEmbeddingHtml,
            string sRightZoneEmbeddingHtml)
        {
            string text = sEmbeddedContent;
            string text2 = "Inner";
            string pattern = "<\\s*tbody\\s*>(.*?<\\s*div[^>]+class\\s*=\\s*\"ms-rte-layoutszone-outer\".*?>.*?(?<" +
                             text2 + "><\\s*div[^>]+class\\s*=\\s*\"ms-rte-layoutszone-inner\".*?>))+";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match match = regex.Match(sEmbeddedContent);
            if (match.Success)
            {
                CaptureCollection captures = match.Groups[text2].Captures;
                if (captures != null && captures.Count > 0)
                {
                    int num = captures[0].Index + captures[0].Length;
                    text = sEmbeddedContent.Substring(0, num);
                    text += sLeftZoneEmbeddingHtml;
                    if (captures.Count > 1)
                    {
                        int num2 = captures[1].Index + captures[1].Length;
                        text += sEmbeddedContent.Substring(num, num2 - num);
                        num = num2;
                    }

                    text += sRightZoneEmbeddingHtml;
                    text += sEmbeddedContent.Substring(num);
                }
            }

            return text;
        }

        private string AddEmbeddedWebPartReferencesUsingXml(XmlNode richTextNode, string sLeftZoneEmbeddingHtml,
            string sRightZoneEmbeddingHtml)
        {
            string result = "";
            if (richTextNode != null)
            {
                XmlNodeList xmlNodeList = richTextNode.SelectNodes(
                    ".//tbody//div[@class='ms-rte-layoutszone-outer']/div[@class='ms-rte-layoutszone-inner']");
                if (xmlNodeList != null && xmlNodeList.Count > 0)
                {
                    XmlNode expr_28 = xmlNodeList[0];
                    expr_28.InnerXml += sLeftZoneEmbeddingHtml;
                    if (xmlNodeList.Count > 1)
                    {
                        XmlNode expr_49 = xmlNodeList[1];
                        expr_49.InnerXml += sRightZoneEmbeddingHtml;
                    }
                    else
                    {
                        XmlNode expr_63 = xmlNodeList[0];
                        expr_63.InnerXml += sRightZoneEmbeddingHtml;
                    }
                }

                result = richTextNode.OuterXml;
            }

            return result;
        }

        private void EmbedWebPartsInGeneralSite(ref string sEmbeddedContent,
            List<OMAdapter.WebPartToEmbed> webPartsToEmbed)
        {
            if (webPartsToEmbed.Count <= 0)
            {
                return;
            }

            string text = "<div>" + this.GenerateEmbeddedWebPartsHtml(webPartsToEmbed) + "</div>";
            int num = sEmbeddedContent.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                sEmbeddedContent = sEmbeddedContent.Insert(num, text);
                return;
            }

            sEmbeddedContent += text;
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                SPFile sPFile = null;
                bool flag = false;
                try
                {
                    string text = this.Server + "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                    if (sPLimitedWebPartManager.WebParts.Count > 0)
                    {
                        sPFile = web.GetFile(text);
                        if (sPFile.InDocumentLibrary && sPFile.Item != null)
                        {
                            bool flag2 = OMAdapter.SupportsPublishing && this.IsPublishingPage(sPFile.Item);
                            flag = (sPFile.Item.ParentList.ForceCheckout || flag2);
                            if (sPFile.Item.ParentList.ForceCheckout)
                            {
                                this.CheckoutPage(sPFile, web);
                            }
                        }

                        if (sPLimitedWebPartManager != null)
                        {
                            sPLimitedWebPartManager.Web.Dispose();
                            sPLimitedWebPartManager.Dispose();
                            sPLimitedWebPartManager = null;
                        }

                        sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                        System.Web.UI.WebControls.WebParts.WebPart webPart = null;
                        foreach (System.Web.UI.WebControls.WebParts.WebPart webPart2 in
                                 sPLimitedWebPartManager.WebParts)
                        {
                            if (string.Equals(webPart2.ID.TrimStart(new char[]
                                {
                                    'g',
                                    '_'
                                }).Replace("_", ""), sWebPartId.Replace("-", "")))
                            {
                                webPart = webPart2;
                                break;
                            }
                        }

                        sPLimitedWebPartManager.DeleteWebPart(webPart);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(
                        "Error while deleting existing web parts from site '" + sWebPartPageServerRelativeUrl +
                        "'. Error: " + ex.Message, ex);
                }
                finally
                {
                    if (sPFile != null && sPFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        if (flag)
                        {
                            this.CheckInPage(sPFile, SPCheckinType.MajorCheckIn,
                                "Checkout required by SSMM2010 in order to delete web parts on page before copying source web parts.");
                        }
                        else
                        {
                            this.CheckInPage(sPFile, SPCheckinType.OverwriteCheckIn, null);
                        }
                    }

                    if (sPLimitedWebPartManager != null)
                    {
                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                        sPLimitedWebPartManager = null;
                    }
                }
            }

            return string.Empty;
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                SPFile sPFile = null;
                bool flag = false;
                bool flag2 = false;
                try
                {
                    string text = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                    if (sPLimitedWebPartManager.WebParts.Count > 0)
                    {
                        sPFile = web.GetFile(text);
                        if (sPFile.InDocumentLibrary && sPFile.Item != null)
                        {
                            bool flag3 = OMAdapter.SupportsPublishing && this.IsPublishingPage(sPFile.Item);
                            flag2 = (sPFile.Item.ParentList.ForceCheckout || flag3);
                            if (sPFile.Item.ParentList.ForceCheckout)
                            {
                                this.CheckoutPage(sPFile, web);
                            }
                        }

                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                        sPLimitedWebPartManager = null;
                        if (base.SharePointVersion.IsSharePoint2010)
                        {
                            HttpRequest request = new HttpRequest("", web.Url, "");
                            HttpContext.Current = new HttpContext(request, new HttpResponse(new StringWriter()));
                            HttpContext.Current.Items["HttpHandlerSPWeb"] = web;
                            flag = true;
                        }

                        sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                        List<System.Web.UI.WebControls.WebParts.WebPart> list =
                            new List<System.Web.UI.WebControls.WebParts.WebPart>();
                        foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in sPLimitedWebPartManager.WebParts)
                        {
                            try
                            {
                                string a;
                                if (webPart.Zone == null)
                                {
                                    Type typeFromHandle = typeof(Microsoft.SharePoint.WebPartPages.WebPart);
                                    FieldInfo field = typeFromHandle.GetField("_zoneID",
                                        BindingFlags.Instance | BindingFlags.NonPublic);
                                    a = (string)field.GetValue(webPart);
                                }
                                else
                                {
                                    a = webPart.Zone.ID;
                                }

                                string fullName = webPart.GetType().FullName;
                                if (a == "MeetingSummary" || a == "MeetingNavigator" ||
                                    fullName == "Microsoft.SharePoint.Meetings.CustomToolPaneManager" ||
                                    fullName == "Microsoft.SharePoint.Meetings.PageTabsWebPart" ||
                                    (fullName == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" &&
                                     base.SharePointVersion.IsSharePoint2007OrEarlier) || this.IsViewWebPart(webPart))
                                {
                                    continue;
                                }

                                if (!this.IsWebPartOnPage(sPLimitedWebPartManager, webPart))
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            }

                            list.Add(webPart);
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            sPLimitedWebPartManager.DeleteWebPart(list[i]);
                        }
                    }

                    string welcomePage = this.GetWelcomePage(sPLimitedWebPartManager.Web);
                    if (!string.IsNullOrEmpty(welcomePage) &&
                        sWebPartPageServerRelativeUrl.EndsWith(welcomePage,
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.ClearDefaultSiteData(sPLimitedWebPartManager.Web);
                    }
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(
                        "Error while deleting existing web parts from site '" + sWebPartPageServerRelativeUrl +
                        "'. Error: " + ex2.Message, ex2);
                }
                finally
                {
                    if (sPFile != null && sPFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        if (flag2)
                        {
                            this.CheckInPage(sPFile, SPCheckinType.MajorCheckIn,
                                "Checkout required in order to delete web parts on page before copying source web parts.");
                        }
                        else
                        {
                            this.CheckInPage(sPFile, SPCheckinType.OverwriteCheckIn, null);
                        }
                    }

                    if (sPLimitedWebPartManager != null)
                    {
                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                        if (flag)
                        {
                            HttpContext.Current = null;
                        }
                    }
                }
            }

            return string.Empty;
        }

        private bool IsViewWebPart(System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            Microsoft.SharePoint.WebPartPages.ListViewWebPart listViewWebPart =
                webPart as Microsoft.SharePoint.WebPartPages.ListViewWebPart;
            if (listViewWebPart == null)
            {
                return false;
            }

            string listViewXml = listViewWebPart.ListViewXml;
            if (string.IsNullOrEmpty(listViewXml))
            {
                return false;
            }

            XmlNode xmlNode;
            try
            {
                xmlNode = XmlUtility.StringToXmlNode(listViewXml);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                bool result = false;
                return result;
            }

            XmlAttribute xmlAttribute = xmlNode.Attributes["DisplayName"];
            if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value))
            {
                return false;
            }

            XmlAttribute xmlAttribute2 = xmlNode.Attributes["Hidden"];
            bool flag;
            return xmlAttribute2 == null || !bool.TryParse(xmlAttribute2.Value, out flag) || !flag;
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPFile sPFile = null;
                SPLimitedWebPartManager sPLimitedWebPartManager = null;
                bool flag = false;
                try
                {
                    string text = "/" + sWebPartPageServerRelativeUrl.Trim(new char[]
                    {
                        '/'
                    });
                    sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                    if (sPLimitedWebPartManager.WebParts.Count > 0)
                    {
                        sPFile = web.GetFile(text);
                        if (sPFile.InDocumentLibrary && sPFile.Item != null)
                        {
                            bool flag2 = OMAdapter.SupportsPublishing && this.IsPublishingPage(sPFile.Item);
                            flag = (sPFile.Item.ParentList.ForceCheckout || flag2);
                            if (sPFile.Item.ParentList.ForceCheckout)
                            {
                                this.CheckoutPage(sPFile, web);
                            }
                        }

                        if (sPLimitedWebPartManager != null)
                        {
                            sPLimitedWebPartManager.Web.Dispose();
                            sPLimitedWebPartManager.Dispose();
                            sPLimitedWebPartManager = null;
                        }

                        sPLimitedWebPartManager = web.GetLimitedWebPartManager(text, PersonalizationScope.Shared);
                        SPLimitedWebPartCollection webParts = sPLimitedWebPartManager.WebParts;
                        foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in webParts)
                        {
                            try
                            {
                                string a;
                                if (webPart.Zone == null)
                                {
                                    Type typeFromHandle = typeof(Microsoft.SharePoint.WebPartPages.WebPart);
                                    FieldInfo field = typeFromHandle.GetField("_zoneID",
                                        BindingFlags.Instance | BindingFlags.NonPublic);
                                    a = (string)field.GetValue(webPart);
                                }
                                else
                                {
                                    a = webPart.Zone.ID;
                                }

                                string fullName = webPart.GetType().FullName;
                                if (a == "MeetingSummary" || a == "MeetingNavigator" ||
                                    fullName == "Microsoft.SharePoint.Meetings.CustomToolPaneManager" ||
                                    fullName == "Microsoft.SharePoint.Meetings.PageTabsWebPart" ||
                                    (fullName == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" &&
                                     base.SharePointVersion.IsSharePoint2007OrEarlier) || this.IsViewWebPart(webPart))
                                {
                                    continue;
                                }

                                if (!this.IsWebPartOnPage(sPLimitedWebPartManager, webPart))
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            }

                            sPLimitedWebPartManager.CloseWebPart(webPart);
                            sPLimitedWebPartManager.SaveChanges(webPart);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(
                        "Error while closing existing web parts from site '" + sWebPartPageServerRelativeUrl +
                        "'. Error: " + ex2.Message, ex2);
                }
                finally
                {
                    if (sPFile != null && sPFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                    {
                        if (flag)
                        {
                            this.CheckInPage(sPFile, SPCheckinType.MajorCheckIn,
                                "Checkout required in order to close web parts on page before copying source web parts.");
                        }
                        else
                        {
                            this.CheckInPage(sPFile, SPCheckinType.OverwriteCheckIn, null);
                        }
                    }

                    if (sPLimitedWebPartManager != null)
                    {
                        sPLimitedWebPartManager.Web.Dispose();
                        sPLimitedWebPartManager.Dispose();
                        sPLimitedWebPartManager = null;
                    }
                }
            }

            return string.Empty;
        }

        private bool IsWebPartOnPage(SPLimitedWebPartManager limitedWpManager,
            System.Web.UI.WebControls.WebParts.WebPart wp)
        {
            SPWebPartManager fullWebPartManager = this.GetFullWebPartManager(limitedWpManager);
            MethodInfo method = fullWebPartManager.GetType()
                .GetMethod("IsWebPartOnPage", BindingFlags.Instance | BindingFlags.NonPublic);
            return (bool)method.Invoke(fullWebPartManager, new object[]
            {
                wp
            });
        }

        private void CheckoutPage(SPFile pageFile, SPWeb containingWeb)
        {
            if (pageFile != null)
            {
                if (pageFile.CheckOutStatus != SPFile.SPCheckOutStatus.None)
                {
                    pageFile.UndoCheckOut();
                }

                this.CheckOutFile(pageFile);
                return;
            }

            throw new Exception("Exception trying to check out a null page for web part copying.");
        }

        private void CheckInPage(SPFile file, SPCheckinType checkInType, string sCheckinComment)
        {
            if (file != null && file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
            {
                try
                {
                    string comment = (!string.IsNullOrEmpty(sCheckinComment)) ? sCheckinComment : file.CheckInComment;
                    file.CheckIn(comment, checkInType);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(
                        "A check-in exception occurred while trying to modify web parts on the page '" + file.Url +
                        "'. Message: " + ex.Message, ex);
                }
            }
        }

        private SPWebPartManager GetFullWebPartManager(SPLimitedWebPartManager limitedWpManager)
        {
            if (limitedWpManager != null)
            {
                PropertyInfo property = limitedWpManager.GetType()
                    .GetProperty("WebPartManager", BindingFlags.Instance | BindingFlags.NonPublic);
                return (SPWebPartManager)property.GetValue(limitedWpManager, null);
            }

            return null;
        }

        private bool SupportsEmbedding(SPFile wppFile)
        {
            return this.GetRichTextEmbeddingField(wppFile) != null;
        }

        private string GetRichTextEmbeddingField(SPFile wppFile)
        {
            string result = null;
            if (base.SharePointVersion.IsSharePoint2007OrLater || wppFile != null || wppFile.Item != null)
            {
                string[] array = this.fieldsThatSupportEmbedding;
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i];
                    if (wppFile.Properties.ContainsKey(text))
                    {
                        result = text;
                        break;
                    }
                }
            }

            return result;
        }

        private string[] GetWebPartZones(SPFile file)
        {
            return null;
        }

        private string StorageKeyToID(Guid storageKey)
        {
            if (!(Guid.Empty == storageKey))
            {
                return "g_" + storageKey.ToString().Replace('-', '_');
            }

            return string.Empty;
        }

        private string GetEmbeddedWebpartString(string sWebpartID)
        {
            return string.Format(
                "<div class=\"ms-rtestate-read ms-rte-wpbox\"><div class=\"ms-rtestate-notify ms-rtestate-read {0}\" id=\"div_{0}\"></div><div id=\"vid_{0}\" style=\"display:none\"></div></div>",
                sWebpartID.ToLower());
        }

        private string GenerateEmbeddedWebPartsHtml(List<OMAdapter.WebPartToEmbed> webPartsToEmbed)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            foreach (OMAdapter.WebPartToEmbed current in webPartsToEmbed)
            {
                stringBuilder.Append(this.GetEmbeddedWebpartString(current.Guid) + "\n");
            }

            return stringBuilder.ToString();
        }

        public string GetSiteUsers()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Users");
                foreach (SPUser sPUser in ((IEnumerable)web.SiteUsers))
                {
                    xmlTextWriter.WriteStartElement("User");
                    try
                    {
                        string sid = sPUser.Sid;
                        xmlTextWriter.WriteAttributeString("ID", sPUser.ID.ToString());
                        xmlTextWriter.WriteAttributeString("Sid", sid);
                        xmlTextWriter.WriteAttributeString("Name", sPUser.Name);
                        xmlTextWriter.WriteAttributeString("LoginName", sPUser.LoginName);
                        xmlTextWriter.WriteAttributeString("Email", sPUser.Email);
                        xmlTextWriter.WriteAttributeString("Notes", sPUser.Notes);
                        xmlTextWriter.WriteAttributeString("IsSiteAdmin", sPUser.IsSiteAdmin.ToString());
                        xmlTextWriter.WriteAttributeString("IsDomainGroup", sPUser.IsDomainGroup.ToString());
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    }
                    finally
                    {
                        xmlTextWriter.WriteEndElement();
                    }
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        public string GetListTemplates()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("ListTemplates");
                foreach (SPListTemplate sPListTemplate in ((IEnumerable)web.ListTemplates))
                {
                    string text = sPListTemplate.SchemaXml;
                    text = text.Replace("xmlns=\"http://schemas.microsoft.com/sharepoint/\"", "");
                    xmlTextWriter.WriteRaw(text);
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringWriter.ToString();
            }

            return result;
        }

        private void GetWebTemplateXml(XmlTextWriter xmlWriter, SPSite site, uint lcid, bool bIncludeExperienceVersions)
        {
            if (bIncludeExperienceVersions && base.SharePointVersion.IsSharePoint2013OrLater)
            {
                return;
            }

            this.GetWebTemplateXml(xmlWriter, site.GetWebTemplates(lcid));
        }

        private void GetWebTemplateXml(XmlTextWriter xmlWriter, SPWebTemplateCollection collection)
        {
            foreach (SPWebTemplate sPWebTemplate in ((IEnumerable)collection))
            {
                xmlWriter.WriteStartElement("WebTemplate");
                try
                {
                    string[] array = sPWebTemplate.Name.Split(new char[]
                    {
                        '#'
                    });
                    string value = sPWebTemplate.Name;
                    string value2 = "0";
                    int num = -1;
                    int num2 = -1;
                    if (array.Length == 2)
                    {
                        if (!int.TryParse(array[1], out num))
                        {
                            value = array[1];
                        }
                        else
                        {
                            value2 = array[1];
                            num2 = sPWebTemplate.ID;
                        }
                    }

                    xmlWriter.WriteAttributeString("Name", value);
                    xmlWriter.WriteAttributeString("ID", num2.ToString());
                    xmlWriter.WriteAttributeString("Config", value2);
                    xmlWriter.WriteAttributeString("FullName", sPWebTemplate.Name);
                    xmlWriter.WriteAttributeString("Title", sPWebTemplate.Title);
                    xmlWriter.WriteAttributeString("ImageUrl", sPWebTemplate.ImageUrl);
                    xmlWriter.WriteAttributeString("Description", sPWebTemplate.Description);
                    xmlWriter.WriteAttributeString("IsHidden", sPWebTemplate.IsHidden.ToString());
                    xmlWriter.WriteAttributeString("IsRootWebOnly", sPWebTemplate.IsRootWebOnly.ToString());
                    xmlWriter.WriteAttributeString("IsSubWebOnly", sPWebTemplate.IsSubWebOnly.ToString());
                }
                catch (Exception ex)
                {
                    xmlWriter.WriteAttributeString("Error", ex.Message);
                }

                xmlWriter.WriteEndElement();
            }
        }

        public string GetWebTemplates()
        {
            string result;
            using (Context context = this.GetContext())
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("WebTemplates");
                this.GetWebTemplateXml(xmlTextWriter, context.Site, context.Web.Language, false);
                foreach (SPWebTemplate sPWebTemplate in ((IEnumerable)context.Site.GetCustomWebTemplates(context.Web
                             .Language)))
                {
                    xmlTextWriter.WriteStartElement("WebTemplate");
                    try
                    {
                        string[] array = sPWebTemplate.Name.Split(new char[]
                        {
                            '#'
                        });
                        string value = sPWebTemplate.Name;
                        if (array.Length == 2)
                        {
                            value = array[1];
                        }

                        xmlTextWriter.WriteAttributeString("Name", value);
                        xmlTextWriter.WriteAttributeString("ID", "-1");
                        xmlTextWriter.WriteAttributeString("Config", "0");
                        xmlTextWriter.WriteAttributeString("FullName", sPWebTemplate.Name);
                        xmlTextWriter.WriteAttributeString("Title", sPWebTemplate.Title);
                        xmlTextWriter.WriteAttributeString("ImageUrl", sPWebTemplate.ImageUrl);
                        xmlTextWriter.WriteAttributeString("Description", sPWebTemplate.Description);
                        xmlTextWriter.WriteAttributeString("IsHidden", sPWebTemplate.IsHidden.ToString());
                        xmlTextWriter.WriteAttributeString("IsRootWebOnly", sPWebTemplate.IsRootWebOnly.ToString());
                        xmlTextWriter.WriteAttributeString("IsSubWebOnly", sPWebTemplate.IsSubWebOnly.ToString());
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        xmlTextWriter.WriteAttributeString("Error", ex.Message);
                    }

                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                result = stringWriter.ToString();
            }

            return result;
        }

        public string GetSiteCollectionsOnWebApp(string sWebApp)
        {
            string result;
            using (Context context = this.GetContext())
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("SiteCollections");
                try
                {
                    Hashtable siteHash = new Hashtable();
                    xmlTextWriter.WriteComment("Current User: '" + WindowsIdentity.GetCurrent().Name + "'");
                    SPWebApplication sPWebApplication;
                    if (sWebApp == null)
                    {
                        sPWebApplication = context.Site.WebApplication;
                    }
                    else
                    {
                        sPWebApplication = SPWebService.ContentService.WebApplications[sWebApp];
                        if (sPWebApplication == null)
                        {
                            throw new Exception("Web app: " + sWebApp + "  does not exist");
                        }
                    }

                    OMAdapter.GetSiteCollectionsXML(xmlTextWriter, siteHash, sPWebApplication, context.Site.Zone);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    xmlTextWriter.WriteComment("Error fetching site collections for the given web app: '" + ex.Message);
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        public string GetSiteCollections()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            using (Context context = this.GetContext())
            {
                xmlTextWriter.WriteStartElement("SiteCollections");
                try
                {
                    Hashtable siteHash = new Hashtable();
                    SPWebApplicationCollection webApplications = SPWebService.ContentService.WebApplications;
                    xmlTextWriter.WriteComment("Current User: '" + WindowsIdentity.GetCurrent().Name + "'");
                    xmlTextWriter.WriteComment("Number of WebApps: '" + webApplications.Count + "'");
                    foreach (SPWebApplication current in webApplications)
                    {
                        OMAdapter.GetSiteCollectionsXML(xmlTextWriter, siteHash, current, context.Site.Zone);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    xmlTextWriter.WriteComment("Error connecting to farm: '" + ex.Message);
                }

                xmlTextWriter.WriteEndElement();
            }

            return stringWriter.ToString();
        }

        private static void GetSiteCollectionsXML(XmlTextWriter xmlWriter, Hashtable siteHash, SPWebApplication webApp,
            SPUrlZone zone)
        {
            try
            {
                string[] names = webApp.Sites.Names;
                for (int i = 0; i < names.Length; i++)
                {
                    string text = names[i];
                    if (text != null)
                    {
                        string text2 = webApp.GetResponseUri(zone, text).ToString();
                        if (!siteHash.Contains(text2))
                        {
                            using (SPSite sPSite = webApp.Sites[text])
                            {
                                xmlWriter.WriteStartElement("Site");
                                xmlWriter.WriteAttributeString("Url", text2);
                                if (sPSite.HostHeaderIsSiteName)
                                {
                                    xmlWriter.WriteAttributeString("ServerRelativeUrl", "/");
                                }
                                else
                                {
                                    xmlWriter.WriteAttributeString("ServerRelativeUrl",
                                        text.StartsWith("/", StringComparison.Ordinal) ? text : ("/" + text));
                                }

                                xmlWriter.WriteEndElement();
                            }

                            siteHash.Add(text2, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                xmlWriter.WriteComment("Error retrieving site collections for web app: '" + ex.Message);
            }
        }

        public string GetSubWebs()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Webs");
                try
                {
                    SPWebCollection subwebsForCurrentUser = web.GetSubwebsForCurrentUser();
                    for (int i = 0; i < subwebsForCurrentUser.Count; i++)
                    {
                        try
                        {
                            using (SPWeb sPWeb = subwebsForCurrentUser[i])
                            {
                                xmlTextWriter.WriteStartElement("Web");
                                this.GetWebXML(sPWeb, xmlTextWriter, false);
                                xmlTextWriter.WriteEndElement();
                            }
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        public string GetSite(bool bFetchFullXml)
        {
            string siteXml;
            using (Context context = this.GetContext())
            {
                siteXml = this.GetSiteXml(context.Site, context.Web, bFetchFullXml);
            }

            return siteXml;
        }

        public string GetWeb(bool bFetchFullXml)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                if (!web.Url.Equals(this.Url.Trim(new char[]
                    {
                        '/'
                    }), StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception(string.Format("The site '{0}' could not be found on the SharePoint server",
                        this.Url));
                }

                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlTextWriter.WriteStartElement("Web");
                this.GetWebXML(web, xmlTextWriter, bFetchFullXml);
                xmlTextWriter.WriteEndElement();
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string GetWebNavigationSettings()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlTextWriter.WriteStartElement("Web");
                xmlTextWriter.WriteAttributeString("Name", Utils.GetNameFromURL(web.Url));
                this.GetWebNavigationXML(web, xmlTextWriter);
                xmlTextWriter.WriteEndElement();
                result = stringBuilder.ToString();
            }

            return result;
        }

        internal string GetSiteXml(SPSite site, SPWeb web, bool bFullXML)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
            {
                this.GetSiteXml(site, web, xmlWriter, bFullXML);
                xmlWriter.Flush();
            }

            return stringBuilder.ToString();
        }

        private void GetSiteXml(SPSite site, SPWeb web, XmlWriter xmlWriter, bool bFullXML)
        {
            xmlWriter.WriteStartElement("Site");
            xmlWriter.WriteAttributeString("SiteID", site.ID.ToString());
            xmlWriter.WriteAttributeString("Url", site.Url);
            if (web.UserIsSiteAdmin)
            {
                try
                {
                    string arg_5F_1 = "DiskUsed";
                    long storage = site.Usage.Storage;
                    xmlWriter.WriteAttributeString(arg_5F_1, storage.ToString());
                    xmlWriter.WriteAttributeString("Owner", (site.Owner != null) ? site.Owner.LoginName : "");
                    xmlWriter.WriteAttributeString("SecondaryOwner",
                        (site.SecondaryContact != null) ? site.SecondaryContact.LoginName : "");
                    xmlWriter.WriteAttributeString("SiteCollectionAdministrators",
                        OMAdapter.GetSiteCollectionAdmins(web));
                    xmlWriter.WriteAttributeString("WebApplication", site.WebApplication.Name);
                    if (site.HostHeaderIsSiteName)
                    {
                        xmlWriter.WriteAttributeString("IsHostHeader", "True");
                    }

                    if (site.ServerRelativeUrl == "/")
                    {
                        xmlWriter.WriteAttributeString("ManagedPath", "/");
                    }
                    else
                    {
                        int num = site.ServerRelativeUrl.IndexOf("/", 1, StringComparison.Ordinal);
                        if (num > 0)
                        {
                            xmlWriter.WriteAttributeString("ManagedPath", site.ServerRelativeUrl.Substring(0, num));
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("ManagedPath",
                                site.ServerRelativeUrl.Substring(0, site.ServerRelativeUrl.Length));
                        }
                    }

                    if (bFullXML)
                    {
                        xmlWriter.WriteAttributeString("TotalWebsInSiteCollection", site.AllWebs.Count.ToString());
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            this.GetWebXML(web, xmlWriter, bFullXML);
            xmlWriter.WriteEndElement();
        }

        private static string GetSiteCollectionAdmins(SPWeb web)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (SPUser sPUser in ((IEnumerable)web.SiteUsers))
            {
                if (sPUser.IsSiteAdmin)
                {
                    stringBuilder.Append(sPUser.LoginName);
                    stringBuilder.Append(";");
                }
            }

            return stringBuilder.ToString();
        }

        private void GetWebXML(SPWeb web, XmlWriter xmlWriter, bool bFullXML)
        {
            bool flag = (web.CurrentUser == null) ? web.UserIsSiteAdmin : web.CurrentUser.IsSiteAdmin;
            if (flag)
            {
                try
                {
                    xmlWriter.WriteAttributeString("WebApplicationId", web.Site.WebApplication.Id.ToString());
                    xmlWriter.WriteAttributeString("MaximumFileSize",
                        web.Site.WebApplication.MaximumFileSize.ToString());
                }
                catch (UnauthorizedAccessException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }

            xmlWriter.WriteAttributeString("ID", web.ID.ToString());
            xmlWriter.WriteAttributeString("ServerRelativeUrl", web.ServerRelativeUrl);
            xmlWriter.WriteAttributeString("Name", Utils.GetNameFromURL(web.Url));
            xmlWriter.WriteAttributeString("Title", web.Title);
            xmlWriter.WriteAttributeString("Locale", web.RegionalSettings.LocaleId.ToString());
            xmlWriter.WriteAttributeString("Language", web.Language.ToString());
            xmlWriter.WriteAttributeString("WebTemplateID", web.WebTemplateId.ToString());
            xmlWriter.WriteAttributeString("WebTemplateConfig", web.Configuration.ToString());
            xmlWriter.WriteAttributeString("WebTemplateName", "");
            xmlWriter.WriteAttributeString("IsSearchable", (flag && OMAdapter.SupportsDBWriting).ToString());
            xmlWriter.WriteAttributeString("TimeZone", web.RegionalSettings.TimeZone.ID.ToString());
            xmlWriter.WriteAttributeString("IsReadOnly", web.Site.ReadOnly.ToString());
            if (!bFullXML)
            {
                return;
            }

            xmlWriter.WriteAttributeString("SiteLogoUrl",
                string.IsNullOrEmpty(web.SiteLogoUrl) ? string.Empty : web.SiteLogoUrl);
            xmlWriter.WriteAttributeString("SiteLogoDescription",
                string.IsNullOrEmpty(web.SiteLogoDescription) ? string.Empty : web.SiteLogoDescription);
            xmlWriter.WriteAttributeString("DatabaseServerName", web.Site.ContentDatabase.Server);
            xmlWriter.WriteAttributeString("DatabaseName", web.Site.ContentDatabase.Name);
            xmlWriter.WriteAttributeString("SiteCollectionServerRelativeUrl", web.Site.ServerRelativeUrl);
            xmlWriter.WriteAttributeString("Description", web.Description);
            xmlWriter.WriteAttributeString("IsSiteAdmin", flag.ToString());
            xmlWriter.WriteAttributeString("HasUniquePermissions", web.HasUniqueRoleAssignments.ToString());
            xmlWriter.WriteAttributeString("HasUniqueRoles", web.HasUniqueRoleDefinitions.ToString());
            xmlWriter.WriteAttributeString("RootSiteGUID", web.Site.ID.ToString());
            xmlWriter.WriteAttributeString("CreatedDate", Utils.FormatDate(Utils.MakeTrueUTCDateTime(web.Created)));
            this.GetSupportedUICulture(web, xmlWriter);
            string value = null;
            try
            {
                value = ((web.Author == null) ? "" : web.Author.LoginName);
            }
            catch (SPException ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                value = "";
            }

            xmlWriter.WriteAttributeString("Author", value);
            xmlWriter.WriteAttributeString("LastItemModifiedDate",
                Utils.FormatDate(Utils.MakeTrueUTCDateTime(web.LastItemModifiedDate)));
            try
            {
                this.GetRootWebData(web, xmlWriter);
            }
            catch (Exception ex3)
            {
                OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
            }

            try
            {
                if (flag)
                {
                    xmlWriter.WriteAttributeString("QuotaID", web.Site.Quota.QuotaID.ToString());
                    xmlWriter.WriteAttributeString("QuotaStorageLimit", web.Site.Quota.StorageMaximumLevel.ToString());
                    xmlWriter.WriteAttributeString("QuotaStorageWarning",
                        web.Site.Quota.StorageWarningLevel.ToString());
                }
            }
            catch (Exception ex4)
            {
                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
            }

            try
            {
                xmlWriter.WriteAttributeString("RequestAccessEnabled", web.RequestAccessEnabled.ToString());
                xmlWriter.WriteAttributeString("RequestAccessEmail", web.RequestAccessEmail);
            }
            catch (Exception ex5)
            {
                OMAdapter.LogExceptionDetails(ex5, MethodBase.GetCurrentMethod().Name, null);
            }

            xmlWriter.WriteAttributeString("ParserEnabled", web.ParserEnabled.ToString());
            this.GetWebNavigationXML(web, xmlWriter);
            if (web.AllProperties["__WebTemplates"] != null)
            {
                xmlWriter.WriteAttributeString("AllowedWebTemplates", web.AllProperties["__WebTemplates"].ToString());
            }

            if (web.AllProperties["__InheritWebTemplates"] != null)
            {
                xmlWriter.WriteAttributeString("InheritWebTemplates",
                    web.AllProperties["__InheritWebTemplates"].ToString());
            }

            if (web.AllProperties["__PageLayouts"] != null)
            {
                xmlWriter.WriteAttributeString("AllowedPageLayouts", web.AllProperties["__PageLayouts"].ToString());
            }

            if (web.AllProperties["__DefaultPageLayout"] != null)
            {
                xmlWriter.WriteAttributeString("DefaultPageLayout",
                    web.AllProperties["__DefaultPageLayout"].ToString());
            }

            if (web.AllProperties["__AllowSpacesInNewPageName"] != null)
            {
                xmlWriter.WriteAttributeString("AllowSpacesInNewPageName",
                    web.AllProperties["__AllowSpacesInNewPageName"].ToString());
            }

            if (web.AllProperties["__InheritsMasterUrl"] != null)
            {
                xmlWriter.WriteAttributeString("InheritsMasterPage",
                    web.AllProperties["__InheritsMasterUrl"].ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("InheritsMasterPage", true.ToString());
            }

            if (web.AllProperties["__InheritsCustomMasterUrl"] != null)
            {
                xmlWriter.WriteAttributeString("InheritsCustomMasterPage",
                    web.AllProperties["__InheritsCustomMasterUrl"].ToString());
            }

            if (web.AllProperties["__InheritsAlternateCssUrl"] != null)
            {
                xmlWriter.WriteAttributeString("InheritsAlternateCssUrl",
                    web.AllProperties["__InheritsAlternateCssUrl"].ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("InheritsAlternateCssUrl", false.ToString());
            }

            xmlWriter.WriteAttributeString("MasterPage", web.MasterUrl);
            xmlWriter.WriteAttributeString("CustomMasterPage", web.CustomMasterUrl);
            xmlWriter.WriteAttributeString("AlternateCssUrl", web.AlternateCssUrl.ToString());
            if (flag)
            {
                xmlWriter.WriteAttributeString("NoCrawl", web.NoCrawl.ToString());
            }

            string value2 = string.Empty;
            if (base.SharePointVersion.IsSharePoint2007OrEarlier)
            {
                xmlWriter.WriteAttributeString("SiteTheme", string.IsNullOrEmpty(web.Theme) ? "none" : web.Theme);
                value2 = "3";
            }

            xmlWriter.WriteAttributeString("UIVersion", value2);
            xmlWriter.WriteAttributeString("RegionalSortOrder", web.RegionalSettings.Collation.ToString());
            xmlWriter.WriteAttributeString("Calendar", web.RegionalSettings.CalendarType.ToString());
            xmlWriter.WriteAttributeString("AlternateCalendar", web.RegionalSettings.AlternateCalendarType.ToString());
            xmlWriter.WriteAttributeString("ShowWeeks", web.RegionalSettings.ShowWeeks.ToString());
            xmlWriter.WriteAttributeString("FirstWeekOfYear", web.RegionalSettings.FirstWeekOfYear.ToString());
            xmlWriter.WriteAttributeString("FirstDayOfWeek", web.RegionalSettings.FirstDayOfWeek.ToString());
            xmlWriter.WriteAttributeString("WorkDays", web.RegionalSettings.WorkDays.ToString());
            xmlWriter.WriteAttributeString("WorkDayStartHour", web.RegionalSettings.WorkDayStartHour.ToString());
            xmlWriter.WriteAttributeString("WorkDayEndHour", web.RegionalSettings.WorkDayEndHour.ToString());
            xmlWriter.WriteAttributeString("TimeFormat", web.RegionalSettings.Time24.ToString());
            xmlWriter.WriteAttributeString("AdjustHijriDays", web.RegionalSettings.AdjustHijriDays.ToString());
            xmlWriter.WriteAttributeString("ASPXPageIndexMode", web.ASPXPageIndexMode.ToString());
            xmlWriter.WriteAttributeString("AllowRSSSiteFeeds", web.Site.AllowRssFeeds.ToString());
            xmlWriter.WriteAttributeString("AllowRSSFeeds", web.AllowRssFeeds.ToString());
            if (web.AllProperties["vti_rss_ManagingEditor"] != null)
            {
                xmlWriter.WriteAttributeString("RssManagingEditor",
                    web.AllProperties["vti_rss_ManagingEditor"].ToString());
            }

            if (web.AllProperties["vti_rss_Copyright"] != null)
            {
                xmlWriter.WriteAttributeString("RssCopyright", web.AllProperties["vti_rss_Copyright"].ToString());
            }

            if (web.AllProperties["vti_rss_WebMaster"] != null)
            {
                xmlWriter.WriteAttributeString("RssWebMaster", web.AllProperties["vti_rss_WebMaster"].ToString());
            }

            if (web.AllProperties["vti_rss_TimeToLive"] != null)
            {
                if (string.IsNullOrEmpty(web.AllProperties["vti_rss_TimeToLive"].ToString()))
                {
                    xmlWriter.WriteAttributeString("RssTimeToLive", "-1");
                }
                else
                {
                    xmlWriter.WriteAttributeString("RssTimeToLive", web.AllProperties["vti_rss_TimeToLive"].ToString());
                }
            }

            xmlWriter.WriteAttributeString("OwnerGroup",
                (web.AssociatedOwnerGroup != null) ? web.AssociatedOwnerGroup.Name : "");
            xmlWriter.WriteAttributeString("MemberGroup",
                (web.AssociatedMemberGroup != null) ? web.AssociatedMemberGroup.Name : "");
            xmlWriter.WriteAttributeString("VisitorGroup",
                (web.AssociatedVisitorGroup != null) ? web.AssociatedVisitorGroup.Name : "");
            if (web.AllProperties["vti_associategroups"] != null)
            {
                xmlWriter.WriteAttributeString("AssociateGroups", this.GetAssociateGroupNames(web));
            }

            StringBuilder stringBuilder = new StringBuilder();
            bool flag2 = false;
            foreach (SPFeature sPFeature in ((IEnumerable)web.Features))
            {
                if (sPFeature.DefinitionId.ToString().ToLower() == "22A9EF51-737B-4FF2-9346-694633FE4416".ToLower())
                {
                    flag2 = true;
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(sPFeature.DefinitionId.ToString());
            }

            xmlWriter.WriteAttributeString("SiteFeatures", stringBuilder.ToString());
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (SPFeature sPFeature2 in ((IEnumerable)web.Site.Features))
            {
                if (stringBuilder2.Length > 0)
                {
                    stringBuilder2.Append(",");
                }

                stringBuilder2.Append(sPFeature2.DefinitionId.ToString());
            }

            xmlWriter.WriteAttributeString("SiteCollFeatures", stringBuilder2.ToString());
            xmlWriter.WriteAttributeString("PublishingFeatureActivated", flag2.ToString());
            string value3;
            if (OMAdapter.SupportsPublishing)
            {
                value3 = this.GetWelcomePage(web);
            }
            else
            {
                value3 = this.GetWelcomePageFromRootFolder(web);
            }

            if (!string.IsNullOrEmpty(value3))
            {
                xmlWriter.WriteAttributeString("WelcomePage", value3);
            }

            if (web.WebTemplateId == 2)
            {
                this.GetWebMeetingInstanceXML(xmlWriter, web);
            }
        }

        private void GetSupportedUICulture(SPWeb web, XmlWriter xmlWriter)
        {
        }

        private void GetRecordDeclarationSettingsXML(SPWeb web, XmlWriter xmlWriter)
        {
            if (web.AllProperties["ecm_siterecorddeclarationby"] != null)
            {
                xmlWriter.WriteAttributeString("SiteRecordDeclarationBy",
                    Convert.ToString(web.AllProperties["ecm_siterecorddeclarationby"]));
            }

            if (web.AllProperties["ecm_siterecordundeclarationby"] != null)
            {
                xmlWriter.WriteAttributeString("SiteRecordUndeclarationBy",
                    Convert.ToString(web.AllProperties["ecm_siterecordundeclarationby"]));
            }

            if (web.AllProperties["ecm_siterecordrestrictions"] != null)
            {
                xmlWriter.WriteAttributeString("SiteRecordRestrictions",
                    Convert.ToString(web.AllProperties["ecm_siterecordrestrictions"]));
            }

            if (web.AllProperties["ecm_siterecorddeclarationdefault"] != null)
            {
                xmlWriter.WriteAttributeString("SiteRecordDeclarationDefault",
                    Convert.ToString(web.AllProperties["ecm_siterecorddeclarationdefault"]));
            }

            if (web.AllProperties["ecm_webhasiprenabled"] != null)
            {
                xmlWriter.WriteAttributeString("WebHasIPRenabled",
                    Convert.ToString(web.AllProperties["ecm_webhasiprenabled"]));
            }
        }

        private string GetWelcomePageFromRootFolder(SPWeb web)
        {
            string result = null;
            try
            {
                result = web.RootFolder.WelcomePage;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }

        private string GetAssociateGroupNames(SPWeb web)
        {
            string text = "";
            foreach (SPGroup current in web.AssociatedGroups)
            {
                if (!string.IsNullOrEmpty(current.Name))
                {
                    string name = current.Name;
                    text = text + name + ';';
                }
            }

            return text;
        }

        private void GetRootWebData(SPWeb web, XmlWriter xmlWriter)
        {
            if (SPContext.Current != null && SPContext.Current.Web.ID == web.ID)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sPSite = new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb sPWeb2 = sPSite.OpenWeb(sPSite.ServerRelativeUrl))
                        {
                            this.GetRootWebDataHelper(sPWeb2, xmlWriter);
                        }
                    }
                });
                return;
            }

            using (SPWeb sPWeb = web.Site.OpenWeb(web.Site.ServerRelativeUrl))
            {
                this.GetRootWebDataHelper(sPWeb, xmlWriter);
            }
        }

        private void GetRootWebDataHelper(SPWeb rootWeb, XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("RootWebGUID", rootWeb.ID.ToString());
            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                SPList sPList;
                try
                {
                    sPList = rootWeb.Lists["TaxonomyHiddenList"];
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    sPList = null;
                }

                if (sPList != null)
                {
                    xmlWriter.WriteAttributeString("TaxonomyListGUID", sPList.ID.ToString());
                }
                else
                {
                    xmlWriter.WriteAttributeString("TaxonomyListGUID", "");
                }
            }
            else
            {
                xmlWriter.WriteAttributeString("TaxonomyListGUID", "");
            }

            try
            {
                if (rootWeb.Site.Audit != null)
                {
                    xmlWriter.WriteAttributeString("AuditFlags", ((int)rootWeb.Site.Audit.AuditFlags).ToString());
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        private void GetWebNavigationXML(SPWeb web, XmlWriter xmlWriter)
        {
            if (web == null)
            {
                return;
            }

            if (web.AllProperties["__NavigationOrderingMethod"] != null)
            {
                xmlWriter.WriteAttributeString("NavigationOrderingMethod",
                    web.AllProperties["__NavigationOrderingMethod"].ToString());
            }

            if (web.AllProperties["__NavigationSortAscending"] != null)
            {
                xmlWriter.WriteAttributeString("NavigationSortAscending",
                    web.AllProperties["__NavigationSortAscending"].ToString());
            }

            if (web.AllProperties["__NavigationAutomaticSortingMethod"] != null)
            {
                xmlWriter.WriteAttributeString("NavigationAutomaticSortingMethod",
                    web.AllProperties["__NavigationAutomaticSortingMethod"].ToString());
            }

            if (web.AllProperties["__InheritCurrentNavigation"] != null)
            {
                xmlWriter.WriteAttributeString("InheritCurrentNavigation",
                    web.AllProperties["__InheritCurrentNavigation"].ToString());
            }
            else if (base.SharePointVersion.IsSharePoint2010 && web.AllProperties["__NavigationShowSiblings"] == null)
            {
                xmlWriter.WriteAttributeString("InheritCurrentNavigation", "false");
            }

            if (web.AllProperties["__DisplayShowHideRibbonActionId"] != null)
            {
                xmlWriter.WriteAttributeString("DisplayShowHideRibbonActionId",
                    web.AllProperties["__DisplayShowHideRibbonActionId"].ToString());
            }

            if (web.AllProperties["__NavigationShowSiblings"] != null)
            {
                xmlWriter.WriteAttributeString("NavigationShowSiblings",
                    web.AllProperties["__NavigationShowSiblings"].ToString());
            }

            bool? flag = null;
            bool? flag2 = null;
            bool? flag3 = null;
            bool? flag4 = null;
            bool value;
            if (web.AllProperties.ContainsKey("__IncludePagesInNavigation") &&
                bool.TryParse(web.AllProperties["__IncludePagesInNavigation"].ToString(), out value))
            {
                flag = new bool?(value);
                flag3 = new bool?(value);
            }

            bool value2;
            if (web.AllProperties.ContainsKey("__IncludeSubSitesInNavigation") &&
                bool.TryParse(web.AllProperties["__IncludeSubSitesInNavigation"].ToString(), out value2))
            {
                flag2 = new bool?(value2);
                flag4 = new bool?(value2);
            }

            int num = 0;
            int num2 = 0;
            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                if (web.AllProperties.ContainsKey("__CurrentNavigationIncludeTypes") &&
                    !int.TryParse(web.AllProperties["__CurrentNavigationIncludeTypes"].ToString(), out num2))
                {
                    num2 = 0;
                }

                if (web.AllProperties.ContainsKey("__GlobalNavigationIncludeTypes") &&
                    !int.TryParse(web.AllProperties["__GlobalNavigationIncludeTypes"].ToString(), out num))
                {
                    num = 0;
                }

                if (!flag.HasValue)
                {
                    flag = new bool?((num2 & 2) > 0);
                }

                if (!flag3.HasValue)
                {
                    flag3 = new bool?((num & 2) > 0);
                }

                if (!flag2.HasValue)
                {
                    flag2 = new bool?((num2 & 1) > 0);
                }

                if (!flag4.HasValue)
                {
                    flag4 = new bool?((num & 1) > 0);
                }
            }

            if (base.SharePointVersion.IsSharePoint2007)
            {
                flag |= !flag.HasValue;
                flag3 |= !flag3.HasValue;
            }
            else
            {
                flag = ((!flag.HasValue) ? new bool?(false) : flag);
                flag3 = ((!flag3.HasValue) ? new bool?(false) : flag3);
            }

            flag2 = ((!flag2.HasValue) ? new bool?(false) : flag2);
            flag4 = ((!flag4.HasValue) ? new bool?(false) : flag4);
            xmlWriter.WriteAttributeString("IncludePagesInCurrentNavigation", flag.Value.ToString());
            xmlWriter.WriteAttributeString("IncludePagesInGlobalNavigation", flag3.Value.ToString());
            xmlWriter.WriteAttributeString("IncludeSubSitesInCurrentNavigation", flag2.Value.ToString());
            xmlWriter.WriteAttributeString("IncludeSubSitesInGlobalNavigation", flag4.Value.ToString());
            if (web.AllProperties["__GlobalDynamicChildLimit"] != null)
            {
                xmlWriter.WriteAttributeString("GlobalDynamicChildLimit",
                    web.AllProperties["__GlobalDynamicChildLimit"].ToString());
            }
            else if (base.SharePointVersion.IsSharePoint2007)
            {
                xmlWriter.WriteAttributeString("GlobalDynamicChildLimit", "50");
            }

            if (web.AllProperties["__CurrentDynamicChildLimit"] != null)
            {
                xmlWriter.WriteAttributeString("CurrentDynamicChildLimit",
                    web.AllProperties["__CurrentDynamicChildLimit"].ToString());
            }
            else if (base.SharePointVersion.IsSharePoint2007)
            {
                xmlWriter.WriteAttributeString("CurrentDynamicChildLimit", "50");
            }

            xmlWriter.WriteAttributeString("QuickLaunchEnabled", web.QuickLaunchEnabled.ToString());
            xmlWriter.WriteAttributeString("TreeViewEnabled", web.TreeViewEnabled.ToString());
            if (web.Navigation != null)
            {
                xmlWriter.WriteAttributeString("InheritGlobalNavigation", web.Navigation.UseShared.ToString());
            }
        }

        private void GetWebMeetingInstanceXML(XmlWriter xmlWriter, SPWeb web)
        {
            SPList sPList = null;
            foreach (SPList sPList2 in ((IEnumerable)web.Lists))
            {
                if (sPList2.BaseTemplate == SPListTemplateType.Meetings)
                {
                    sPList = sPList2;
                    break;
                }
            }

            if (sPList == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            XmlWriter xmlWriter2 = new XmlTextWriter(new StringWriter(sb));
            xmlWriter2.WriteStartElement("Fields");
            foreach (SPField sPField in ((IEnumerable)sPList.Fields))
            {
                xmlWriter2.WriteRaw(sPField.SchemaXml);
            }

            xmlWriter2.WriteEndElement();
            xmlWriter2.Flush();
            string listItemsInternal = this.GetListItemsInternal(sPList.ID.ToString(), null, null, null, true,
                ListItemQueryType.ListItem, web, null, new GetListItemOptions());
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(listItemsInternal);
            XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("./ListItem");
            List<XmlAttribute> list = new List<XmlAttribute>();
            List<string> list2 = new List<string>();
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XmlAttribute xmlAttribute = xmlNode.Attributes["EventUID"];
                if (xmlAttribute != null)
                {
                    string value = xmlAttribute.Value;
                    string[] array = value.Split(new char[]
                    {
                        ':'
                    });
                    if (array.Length >= 4)
                    {
                        string s = array[array.Length - 1];
                        string g = array[array.Length - 3];
                        int num = -1;
                        Guid a = Guid.Empty;
                        try
                        {
                            a = new Guid(g);
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }

                        if (int.TryParse(s, out num) && !(a == Guid.Empty))
                        {
                            string text = "";
                            for (int i = 0; i < array.Length - 4; i++)
                            {
                                if (!string.IsNullOrEmpty(text))
                                {
                                    text += ":";
                                }

                                text += array[i];
                            }

                            XmlAttribute xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute("EventUIDPrefix");
                            XmlAttribute xmlAttribute3 = xmlNode.OwnerDocument.CreateAttribute("EventUIDItemID");
                            XmlAttribute xmlAttribute4 = xmlNode.OwnerDocument.CreateAttribute("EventUIDListName");
                            xmlAttribute2.Value = text;
                            xmlAttribute3.Value = num.ToString();
                            xmlAttribute4.Value = a.ToString();
                            xmlNode.Attributes.Remove(xmlAttribute);
                            xmlNode.Attributes.Append(xmlAttribute2);
                            xmlNode.Attributes.Append(xmlAttribute4);
                            xmlNode.Attributes.Append(xmlAttribute3);
                            list.Add(xmlAttribute4);
                            if (!list2.Contains(a.ToString()))
                            {
                                list2.Add(a.ToString());
                            }
                        }
                    }
                }
            }

            if (!web.IsRootWeb && list2.Count > 0)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                using (SPWeb sPWeb = web.Site.OpenWeb(web.ParentWebId))
                {
                    foreach (string current in list2)
                    {
                        foreach (SPList sPList3 in ((IEnumerable)sPWeb.Lists))
                        {
                            if (current == sPList3.ID.ToString())
                            {
                                dictionary.Add(current, sPList3.Title);
                                break;
                            }
                        }
                    }
                }

                foreach (XmlAttribute current2 in list)
                {
                    if (dictionary.ContainsKey(current2.Value))
                    {
                        current2.Value = dictionary[current2.Value];
                    }
                }
            }

            xmlWriter.WriteStartElement("MeetingInstances");
            foreach (XmlNode xmlNode2 in xmlNodeList)
            {
                xmlWriter.WriteRaw(xmlNode2.OuterXml.Replace("<ListItem ", "<MeetingInstance "));
            }

            xmlWriter.WriteEndElement();
        }

        public string GetSystemInfo()
        {
            return new SystemInfo().ToXmlString();
        }

        public string GetGroups()
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Groups");
                SPList siteUserInfoList = web.SiteUserInfoList;
                foreach (SPGroup sPGroup in ((IEnumerable)web.SiteGroups))
                {
                    if (sPGroup.CanCurrentUserViewMembership)
                    {
                        sPGroup.Description = this.GetGroupDescription(siteUserInfoList, sPGroup);
                        this.WriteGroupXML(sPGroup, xmlTextWriter);
                    }
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        private string GetGroupDescription(SPList userInfoList, SPGroup group)
        {
            try
            {
                SPListItem itemById = userInfoList.GetItemById(group.ID);
                SPField fieldByInternalName = itemById.Fields.GetFieldByInternalName("Notes");
                string text = (fieldByInternalName != null)
                    ? fieldByInternalName.GetFieldValueAsHtml(itemById["Notes"])
                    : string.Empty;
                return (!string.IsNullOrEmpty(text)) ? text : group.Description;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            return string.Empty;
        }

        private void WriteGroupXML(SPGroup group, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Group");
            xmlWriter.WriteAttributeString("ID", group.ID.ToString());
            xmlWriter.WriteAttributeString("Name", group.Name);
            xmlWriter.WriteAttributeString("Description", group.Description);
            xmlWriter.WriteAttributeString("OnlyAllowMembersViewMembership",
                group.OnlyAllowMembersViewMembership.ToString());
            xmlWriter.WriteAttributeString("AllowMembersEditMembership", group.AllowMembersEditMembership.ToString());
            xmlWriter.WriteAttributeString("AllowRequestToJoinLeave", group.AllowRequestToJoinLeave.ToString());
            xmlWriter.WriteAttributeString("AutoAcceptRequestToJoinLeave",
                group.AutoAcceptRequestToJoinLeave.ToString());
            xmlWriter.WriteAttributeString("RequestToJoinLeaveEmailSetting", group.RequestToJoinLeaveEmailSetting);
            if (group.Owner is SPUser)
            {
                xmlWriter.WriteAttributeString("OwnerIsUser", true.ToString());
                xmlWriter.WriteAttributeString("Owner", ((SPUser)group.Owner).LoginName);
            }
            else
            {
                xmlWriter.WriteAttributeString("OwnerIsUser", false.ToString());
                xmlWriter.WriteAttributeString("Owner", ((SPGroup)group.Owner).Name);
            }

            foreach (SPUser sPUser in ((IEnumerable)group.Users))
            {
                xmlWriter.WriteStartElement("Member");
                xmlWriter.WriteAttributeString("Login", sPUser.LoginName);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        public string GetRoles(string sListId)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Roles");
                xmlTextWriter.WriteAttributeString("SharePointVersion", base.SharePointVersion.ToString());
                foreach (SPRoleDefinition sPRoleDefinition in ((IEnumerable)web.FirstUniqueRoleDefinitionWeb
                             .RoleDefinitions))
                {
                    xmlTextWriter.WriteStartElement("Role");
                    xmlTextWriter.WriteAttributeString("RoleId", Convert.ToString(sPRoleDefinition.Id));
                    xmlTextWriter.WriteAttributeString("Name", sPRoleDefinition.Name);
                    xmlTextWriter.WriteAttributeString("Description", sPRoleDefinition.Description);
                    xmlTextWriter.WriteAttributeString("Hidden", sPRoleDefinition.Hidden.ToString());
                    xmlTextWriter.WriteAttributeString("PermMask", ((long)sPRoleDefinition.BasePermissions).ToString());
                    xmlTextWriter.WriteAttributeString("RoleOrder", sPRoleDefinition.Order.ToString());
                    xmlTextWriter.WriteAttributeString("Type", ((int)sPRoleDefinition.Type).ToString());
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        public string GetRoleAssignments(string sListId, int iItemId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("RoleAssignments");
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPRoleAssignmentCollection roleAssignments;
                if (sListId != null && sListId.Length > 0)
                {
                    SPList sPList = web.Lists[new Guid(sListId)];
                    if (sPList == null)
                    {
                        throw new ArgumentException("Could not find SharePoint list with ID " + sListId);
                    }

                    if (iItemId >= 0)
                    {
                        SPListItem itemByID = OMAdapter.GetItemByID(sPList, iItemId, true, true, 0u);
                        if (sPList == null)
                        {
                            throw new ArgumentException(string.Concat(new object[]
                            {
                                "Could not find item with ID ",
                                iItemId,
                                " in list ",
                                sPList.Title
                            }));
                        }

                        roleAssignments = itemByID.RoleAssignments;
                    }
                    else
                    {
                        roleAssignments = sPList.RoleAssignments;
                    }
                }
                else
                {
                    roleAssignments = web.RoleAssignments;
                }

                foreach (SPRoleAssignment sPRoleAssignment in ((IEnumerable)roleAssignments))
                {
                    string value = sPRoleAssignment.Member.ToString();
                    foreach (SPRoleDefinition sPRoleDefinition in
                             ((IEnumerable)sPRoleAssignment.RoleDefinitionBindings))
                    {
                        xmlTextWriter.WriteStartElement("RoleAssignment");
                        xmlTextWriter.WriteAttributeString("RoleName", sPRoleDefinition.Name);
                        xmlTextWriter.WriteAttributeString("PrincipalName", value);
                        xmlTextWriter.WriteEndElement();
                    }
                }
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public string GetFields(string sListId, bool bGetAllAvailableFields)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                SPFieldCollection fields;
                if (!string.IsNullOrEmpty(sListId))
                {
                    fields = web.Lists[new Guid(sListId)].Fields;
                }
                else if (bGetAllAvailableFields)
                {
                    fields = web.AvailableFields;
                }
                else
                {
                    fields = web.Fields;
                }

                this.WriteFieldXml(fields, web, xmlWriter);
                result = stringBuilder.ToString();
            }

            return result;
        }

        public string GetContentTypes(string sListId)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                SPContentTypeCollection sPContentTypeCollection;
                if (string.IsNullOrEmpty(sListId))
                {
                    sPContentTypeCollection = web.AvailableContentTypes;
                }
                else
                {
                    SPList sPList = web.Lists[new Guid(sListId)];
                    sPContentTypeCollection = sPList.ContentTypes;
                }

                xmlTextWriter.WriteStartElement("ContentTypes");
                foreach (SPContentType contentType in ((IEnumerable)sPContentTypeCollection))
                {
                    this.GetContentTypeXML(contentType, xmlTextWriter);
                }

                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        private void WriteErrorElement(XmlWriter xmlWriter, string message, string stack)
        {
            if (!string.IsNullOrEmpty(message))
            {
                xmlWriter.WriteStartElement("Error");
                xmlWriter.WriteAttributeString("Message", message);
                if (!string.IsNullOrEmpty(stack))
                {
                    xmlWriter.WriteAttributeString("Stack", message);
                }

                xmlWriter.WriteEndElement();
            }
        }

        public string GetWorkflowAssociations(string sObjectID, string workflowConfigurationXml)
        {
            bool includePreviousWorkflowVersions = false;
            string text = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode xmlNode = null;
            if (!string.IsNullOrEmpty(workflowConfigurationXml) &&
                workflowConfigurationXml.StartsWith("<", StringComparison.Ordinal))
            {
                xmlDocument.LoadXml(workflowConfigurationXml);
                xmlNode = xmlDocument.DocumentElement;
                includePreviousWorkflowVersions = xmlNode.GetAttributeValueAsBoolean("IncludePreviousVersions");
                text = xmlNode.GetAttributeValueAsString("Scope");
            }
            else
            {
                text = workflowConfigurationXml;
            }

            StringBuilder stringBuilder = new StringBuilder(1024);
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
                   {
                       OmitXmlDeclaration = true
                   }))
            {
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    xmlWriter.WriteStartElement("WorkflowAssociations");
                    try
                    {
                        if (text.Equals("list", StringComparison.InvariantCultureIgnoreCase))
                        {
                            SPList sPList = web.Lists[new Guid(sObjectID)];
                            if (sPList == null)
                            {
                                this.WriteErrorElement(xmlWriter,
                                    string.Format("Unable to find list with Id '{0}' in site '{1}'", sObjectID,
                                        web.Url), null);
                            }
                            else
                            {
                                this.GetWorkflowAssociationsXML(sPList, xmlWriter, includePreviousWorkflowVersions);
                            }
                        }
                        else if (text.Equals("web", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.GetWorkflowAssociationsXML(web, xmlWriter, includePreviousWorkflowVersions);
                        }
                        else if (text.Equals("ContentType", StringComparison.InvariantCultureIgnoreCase))
                        {
                            SPContentType sPContentType = web.ContentTypes[new SPContentTypeId(sObjectID)];
                            if (sPContentType == null)
                            {
                                this.WriteErrorElement(xmlWriter,
                                    string.Format("Unable to find content type with Id '{0}' in site '{1}'", sObjectID,
                                        web.Url), null);
                            }
                            else
                            {
                                this.GetWorkflowAssociationsXML(sPContentType, xmlWriter,
                                    includePreviousWorkflowVersions);
                            }
                        }
                        else
                        {
                            if (xmlNode != null && !string.IsNullOrEmpty(workflowConfigurationXml) &&
                                workflowConfigurationXml.StartsWith("<", StringComparison.Ordinal))
                            {
                                text = xmlNode.GetAttributeValueAsString("ListID");
                            }

                            SPList sPList2 = web.Lists[new Guid(text)];
                            SPContentType sPContentType2 = sPList2.ContentTypes[new SPContentTypeId(sObjectID)];
                            if (sPContentType2 == null)
                            {
                                this.WriteErrorElement(xmlWriter,
                                    string.Format(
                                        "Unable to find content type with Id '{0}' in List '{1}' within site '{2}'",
                                        sObjectID,
                                        (sPList2 != null)
                                            ? string.Format("{0} ({1})", sPList2.Title, sPList2.ID)
                                            : text, web.Url), null);
                            }
                            else
                            {
                                this.GetWorkflowAssociationsXML(sPContentType2, xmlWriter,
                                    includePreviousWorkflowVersions);
                            }
                        }
                    }
                    finally
                    {
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.Flush();
                }
            }

            return stringBuilder.ToString();
        }

        private void GetWorkflowAssociationsXML(SPList workflowParent, XmlWriter xmlWriter,
            bool includePreviousWorkflowVersions)
        {
            foreach (SPWorkflowAssociation wfa in ((IEnumerable)workflowParent.WorkflowAssociations))
            {
                this.WriteWorkflowXml(wfa, xmlWriter, workflowParent.ID.ToString(), true,
                    includePreviousWorkflowVersions);
            }
        }

        private void GetWorkflowAssociationsXML(SPWeb workflowParent, XmlWriter xmlWriter,
            bool includePreviousWorkflowVersions)
        {
        }

        private void GetWorkflowAssociationsXML(SPContentType workflowParent, XmlWriter xmlWriter,
            bool includePreviousWorkflowVersions)
        {
            foreach (SPWorkflowAssociation wfa in ((IEnumerable)workflowParent.WorkflowAssociations))
            {
                this.WriteWorkflowXml(wfa, xmlWriter, workflowParent.Id.ToString(), false,
                    includePreviousWorkflowVersions);
            }
        }

        private void WriteWorkflowXml(SPWorkflowAssociation wfa, XmlWriter xmlWriter, string sParentID,
            bool isListLevelWorkflow, bool includePreviousWorkflowVersions = false)
        {
            if (!includePreviousWorkflowVersions && (wfa.MarkedForDelete || !wfa.Enabled))
            {
                return;
            }

            xmlWriter.WriteStartElement("WorkflowAssociation");
            xmlWriter.WriteAttributeString("ParentID", sParentID);
            xmlWriter.WriteAttributeString("AllowAsyncManualStart", wfa.AllowAsyncManualStart.ToString());
            xmlWriter.WriteAttributeString("AllowManual", wfa.AllowManual.ToString());
            xmlWriter.WriteAttributeString("AssociationData", wfa.AssociationData);
            xmlWriter.WriteAttributeString("AutoCleanupDays", wfa.AutoCleanupDays.ToString());
            xmlWriter.WriteAttributeString("AutoStartChange", wfa.AutoStartChange.ToString());
            xmlWriter.WriteAttributeString("AutoStartCreate", wfa.AutoStartCreate.ToString());
            xmlWriter.WriteAttributeString("BaseId", wfa.BaseId.ToString());
            xmlWriter.WriteAttributeString("CompressInstanceData", wfa.CompressInstanceData.ToString());
            xmlWriter.WriteAttributeString("Created", Utils.FormatDate(Utils.MakeTrueUTCDateTime(wfa.Created)));
            xmlWriter.WriteAttributeString("Modified", Utils.FormatDate(Utils.MakeTrueUTCDateTime(wfa.Modified)));
            xmlWriter.WriteAttributeString("Enabled", wfa.Enabled.ToString());
            xmlWriter.WriteAttributeString("GloballyEnabled", wfa.GloballyEnabled.ToString());
            xmlWriter.WriteAttributeString("HistoryListId", wfa.HistoryListId.ToString());
            xmlWriter.WriteAttributeString("HistoryListTitle", wfa.HistoryListTitle.ToString());
            xmlWriter.WriteAttributeString("Id", wfa.Id.ToString());
            xmlWriter.WriteAttributeString("IsDeclarative", wfa.IsDeclarative.ToString());
            xmlWriter.WriteAttributeString("LockItem", wfa.LockItem.ToString());
            xmlWriter.WriteAttributeString("Name", wfa.Name.ToString());
            xmlWriter.WriteAttributeString("PermissionsManual", wfa.PermissionsManual.ToString());
            xmlWriter.WriteAttributeString("StatusColumn", wfa.StatusColumn.ToString());
            xmlWriter.WriteAttributeString("StatusColumnName", this.GetWorkflowAssociationInternalStatusField(wfa));
            xmlWriter.WriteAttributeString("TaskListId", wfa.TaskListId.ToString());
            xmlWriter.WriteAttributeString("TaskListTitle", wfa.TaskListTitle.ToString());
            xmlWriter.WriteAttributeString("UIVersion", this.GetWorkflowAssociationUIVersion(wfa, xmlWriter));
            if (isListLevelWorkflow && wfa.ParentList.BaseType.Equals(SPBaseType.DocumentLibrary) &&
                wfa.ParentList.DefaultContentApprovalWorkflowId != Guid.Empty &&
                wfa.ParentList.DefaultContentApprovalWorkflowId.Equals(wfa.Id))
            {
                xmlWriter.WriteAttributeString("DefaultContentApprovalWorkflowId",
                    wfa.ParentList.DefaultContentApprovalWorkflowId.ToString());
            }

            if (wfa.ParentContentType != null && wfa.ParentContentType.ParentList != null)
            {
                xmlWriter.WriteAttributeString("ContentTypeParentListId",
                    wfa.ParentContentType.ParentList.ID.ToString());
            }

            if (wfa.Description != null)
            {
                xmlWriter.WriteAttributeString("Description", wfa.Description.ToString());
            }

            if (wfa.BaseTemplate != null)
            {
                if (base.SharePointVersion.IsSharePoint2010OrLater)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(wfa.SoapXml);
                    XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(".//Instantiation_FormURI");
                    bool flag = false;
                    if (xmlNode == null && xmlDocument.DocumentElement.Attributes["InstantiationUrl"] != null &&
                        xmlDocument.DocumentElement.Attributes["InstantiationUrl"].Value.Contains("NintexWorkflow"))
                    {
                        flag = true;
                    }

                    if (!flag && (xmlNode == null ||
                                  !xmlNode.InnerText.StartsWith("Workflows", StringComparison.InvariantCulture)))
                    {
                        xmlWriter.WriteAttributeString("BaseTemplate", wfa.BaseTemplate.BaseId.ToString());
                    }
                    else
                    {
                        xmlWriter.WriteAttributeString("BaseTemplate", wfa.BaseTemplate.BaseId.ToString());
                        xmlWriter.WriteAttributeString("Is2010SharePointDesignerWorkflow",
                            wfa.BaseTemplate.BaseId.ToString());
                    }

                    xmlWriter.WriteAttributeString("IsNintex", flag.ToString());
                }
                else
                {
                    xmlWriter.WriteAttributeString("BaseTemplate", wfa.BaseTemplate.BaseId.ToString());
                }

                xmlWriter.WriteAttributeString("BaseTemplateName", wfa.BaseTemplate.Name);
            }

            if (base.SharePointVersion.IsSharePoint2007 && !string.IsNullOrEmpty(wfa.InstantiationUrl) &&
                wfa.InstantiationUrl.Contains("NintexWorkflow/"))
            {
                xmlWriter.WriteAttributeString("IsNintex", "true");
            }

            Guid arg_505_0 = wfa.ParentAssociationId;
            xmlWriter.WriteAttributeString("ParentAssociationId", wfa.ParentAssociationId.ToString());
            if (wfa.ParentContentType != null)
            {
                xmlWriter.WriteAttributeString("ParentContentType", wfa.ParentContentType.Name);
            }

            if (wfa.ParentList != null)
            {
                xmlWriter.WriteAttributeString("ParentListID", wfa.ParentList.ID.ToString());
            }

            if (wfa.UpgradedPersistedProperties != null)
            {
                xmlWriter.WriteAttributeString("UpgradedPersistedProperties",
                    wfa.UpgradedPersistedProperties.ToString());
            }

            xmlWriter.WriteEndElement();
        }

        private SPFolder GetWorkflowFolder(string folderName, SPFolder parentFolder, SPWeb web)
        {
            string strUrl = string.Concat(new string[]
            {
                web.ServerRelativeUrl.TrimEnd(new char[]
                {
                    '/'
                }),
                "/",
                parentFolder.Url,
                "/",
                folderName
            });
            return web.GetFolder(strUrl);
        }

        private string GetWorkflowAssociationUIVersion(SPWorkflowAssociation sourceWorkflowAssociation,
            XmlWriter xmlWriter)
        {
            string text = sourceWorkflowAssociation.Name;
            text = Utils.GetWorkflowName(text);
            SPFile sPFile = null;
            SPList listByName = OMAdapter.GetListByName(sourceWorkflowAssociation.ParentWeb, "Workflows");
            if (listByName != null)
            {
                SPFolder rootFolder = listByName.RootFolder;
                SPFolder workflowFolder = this.GetWorkflowFolder(text, rootFolder, rootFolder.ParentWeb);
                if (workflowFolder.Exists)
                {
                    if (workflowFolder.Files[text + ".xoml.wfconfig.xml"].Exists)
                    {
                        sPFile = workflowFolder.Files[text + ".xoml.wfconfig.xml"];
                    }
                }
                else
                {
                    SPFolder workflowFolder2 =
                        this.GetWorkflowFolder("NintexWorkflow", rootFolder, rootFolder.ParentWeb);
                    if (workflowFolder2.Exists)
                    {
                        workflowFolder = this.GetWorkflowFolder(text, workflowFolder2, rootFolder.ParentWeb);
                        if (workflowFolder.Exists && workflowFolder.Files[text + ".xoml.wfconfig.xml"].Exists)
                        {
                            sPFile = workflowFolder.Files[text + ".xoml.wfconfig.xml"];
                        }
                    }
                }
            }

            string value = string.Empty;
            if (sPFile != null)
            {
                int workflowUIVersionId = Utils.GetWorkflowUIVersionId(sourceWorkflowAssociation.InternalName);
                if (workflowUIVersionId > 0)
                {
                    if (sPFile.UIVersion == workflowUIVersionId)
                    {
                        value = sPFile.UIVersionLabel;
                    }

                    if (string.IsNullOrEmpty(Convert.ToString(value)))
                    {
                        foreach (SPFileVersion sPFileVersion in ((IEnumerable)sPFile.Versions))
                        {
                            if (sPFileVersion.ID == workflowUIVersionId)
                            {
                                value = sPFileVersion.VersionLabel;
                                break;
                            }
                        }
                    }
                }
            }

            return Convert.ToString(value);
        }

        public string GetPortalListingGroups()
        {
            return null;
        }

        public string GetPortalListingIDs()
        {
            return null;
        }

        public string GetPortalListings(string sIDList)
        {
            return null;
        }

        public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPFolder folder = web.GetFolder(sFolderPath ?? "");
                if (!folder.Exists)
                {
                    result = null;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlTextWriter.WriteStartElement("FolderContent");
                    if ((itemTypes & ListItemQueryType.ListItem) == ListItemQueryType.ListItem)
                    {
                        xmlTextWriter.WriteStartElement("Files");
                        foreach (SPFile file in ((IEnumerable)folder.Files))
                        {
                            OMAdapter.AddFileToXml(xmlTextWriter, file);
                        }

                        xmlTextWriter.WriteEndElement();
                    }

                    if ((itemTypes & ListItemQueryType.Folder) == ListItemQueryType.Folder)
                    {
                        xmlTextWriter.WriteStartElement("Folders");
                        foreach (SPFolder folder2 in ((IEnumerable)folder.SubFolders))
                        {
                            OMAdapter.AddFolderToXml(xmlTextWriter, folder2);
                        }

                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                    result = stringBuilder.ToString();
                }
            }

            return result;
        }

        private static void AddFolderToXml(XmlTextWriter xmlWriter, SPFolder folder)
        {
            xmlWriter.WriteStartElement("Folder");
            xmlWriter.WriteAttributeString("Name", folder.Name);
            xmlWriter.WriteAttributeString("Url", folder.Url);
            xmlWriter.WriteAttributeString("ParentListId", folder.ParentListId.ToString());
            xmlWriter.WriteEndElement();
        }

        private static void AddFileToXml(XmlTextWriter xmlWriter, SPFile file)
        {
            xmlWriter.WriteStartElement("File");
            string value = null;
            try
            {
                value = ((file.Author == null) ? string.Empty : file.Author.LoginName);
            }
            catch (SPException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                value = string.Empty;
            }

            xmlWriter.WriteAttributeString("Author", value);
            xmlWriter.WriteAttributeString("CustomizedPageStatus", file.CustomizedPageStatus.ToString());
            string value2 = null;
            try
            {
                value2 = ((file.ModifiedBy == null) ? string.Empty : file.ModifiedBy.LoginName);
            }
            catch (SPException ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                value2 = string.Empty;
            }

            xmlWriter.WriteAttributeString("ModifiedBy", value2);
            xmlWriter.WriteAttributeString("Name", file.Name);
            xmlWriter.WriteAttributeString("TimeCreated", Utils.FormatDate(file.TimeCreated));
            xmlWriter.WriteAttributeString("TimeLastModified", Utils.FormatDate(file.TimeLastModified));
            xmlWriter.WriteAttributeString("UniqueId", file.UniqueId.ToString());
            xmlWriter.WriteAttributeString("Url", file.Url);
            xmlWriter.WriteEndElement();
        }

        public string AddWeb(string sWebXML, AddWebOptions addSPWebOptions)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                using (Context context = this.GetContext())
                {
                    SPWeb web = context.Web;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sWebXML);
                    XmlNode firstChild = xmlDocument.FirstChild;
                    string text = firstChild.Attributes["Name"].Value;
                    string value = firstChild.Attributes["Title"].Value;
                    string text2 = (firstChild.Attributes["Description"] != null)
                        ? firstChild.Attributes["Description"].Value
                        : null;
                    int iLocaleNew = (firstChild.Attributes["Locale"] == null)
                        ? Convert.ToInt32(web.Locale.LCID)
                        : Convert.ToInt32(firstChild.Attributes["Locale"].Value);
                    int num = (firstChild.Attributes["Language"] == null)
                        ? Convert.ToInt32(web.Language)
                        : Convert.ToInt32(firstChild.Attributes["Language"].Value);
                    int num2;
                    if (!int.TryParse(firstChild.Attributes["WebTemplateID"].Value, out num2))
                    {
                        num2 = -1;
                    }

                    int num3 = Convert.ToInt32(firstChild.Attributes["WebTemplateConfig"].Value);
                    string text3 = null;
                    if (firstChild.Attributes["WebTemplateName"] != null)
                    {
                        text3 = firstChild.Attributes["WebTemplateName"].Value;
                    }

                    if (base.SharePointVersion.IsSharePoint2010OrLater && num2 == 14483 && num3 == 0)
                    {
                        num3 = 1;
                    }

                    SPWebTemplate sPWebTemplate = null;
                    if (num3 != -1)
                    {
                        try
                        {
                            sPWebTemplate =
                                this.GetWebTemplateByIDOrName((uint)num, num2, num3, text3, null, context.Site);
                        }
                        catch (LanguageTemplatesMissingException ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            throw new Exception("The templates for the given source language (" +
                                                iLocaleNew.ToString() +
                                                ") cannot be found on the target server. This is most likely due to the source language pack not being installed on the target.");
                        }

                        if (sPWebTemplate == null)
                        {
                            throw new Exception(string.Format(
                                "The template '{0} [{1}]'does not exist on the target server. Please install this template on the target machine or map it to an existing template.",
                                text3 ?? "?", num2));
                        }
                    }

                    if (text == null || text.Length == 0)
                    {
                        text = Utils.CleanSharePointURL(value);
                    }

                    SPWeb sPWeb = OMAdapter.GetWebByName(web, text);
                    try
                    {
                        bool overwrite = addSPWebOptions.Overwrite;
                        if (sPWeb != null && addSPWebOptions.Overwrite)
                        {
                            this.DeleteWeb(sPWeb);
                            sPWeb.Dispose();
                            sPWeb = null;
                        }

                        if (sPWeb == null)
                        {
                            int millisecondsTimeout = 1000;
                            for (int i = 1; i <= 5; i++)
                            {
                                try
                                {
                                    if (num2 != -1)
                                    {
                                        sPWeb = web.Webs.Add(text, value, text2, (uint)num, sPWebTemplate, false,
                                            false);
                                    }
                                    else
                                    {
                                        sPWeb = web.Webs.Add(text, value, text2, (uint)num, (SPWebTemplate)null, false,
                                            false);
                                    }

                                    if (addSPWebOptions.PreserveUIVersion)
                                    {
                                        this.ChangeUIVersion(sPWeb);
                                    }

                                    break;
                                }
                                catch (Exception ex2)
                                {
                                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                                    IList<int> exceptionErrorCodes = ExceptionUtils.GetExceptionErrorCodes(ex2);
                                    string message = ex2.Message;
                                    if (i == 5 ||
                                        !ExecuteWithRetry.ValidRetryCodes.Any(
                                            new Func<int, bool>(exceptionErrorCodes.Contains)) ||
                                        "The SharePoint Server Publishing Infrastructure feature must be activated at the site collection level before the Publishing feature can be activated."
                                            .Equals(message, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        throw;
                                    }

                                    operationReporting.LogInformation(string.Format("Retry attempt:{0}", i),
                                        string.Format("Msg:{0}", ex2.Message));
                                    Thread.Sleep(millisecondsTimeout);
                                    millisecondsTimeout = 1000 * (i + 1);
                                    sPWeb = OMAdapter.GetWebByName(web, text);
                                    if (sPWeb != null)
                                    {
                                        this.DeleteWeb(sPWeb);
                                        sPWeb.Dispose();
                                        sPWeb = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sPWeb.Title = value;
                            sPWeb.Description = text2;
                        }

                        if (addSPWebOptions.CopyFeatures)
                        {
                            bool allowUnsafeUpdates = sPWeb.AllowUnsafeUpdates;
                            try
                            {
                                string sFeatureGuids = (firstChild.Attributes["SiteFeatures"] != null)
                                    ? firstChild.Attributes["SiteFeatures"].Value
                                    : "";
                                this.AddFeatures(sPWeb.Features, sPWeb.Site, sFeatureGuids,
                                    addSPWebOptions.MergeFeatures,
                                    string.Equals(sPWeb.WebTemplate, "ENTERWIKI", StringComparison.OrdinalIgnoreCase));
                            }
                            finally
                            {
                                sPWeb.AllowUnsafeUpdates = allowUnsafeUpdates;
                            }
                        }

                        if (overwrite)
                        {
                            this.ClearDefaultSiteData(sPWeb);
                        }

                        sPWeb.AllProperties["__InheritsMasterUrl"] =
                            ((firstChild.Attributes["InheritsMasterPage"] == null)
                                ? "False"
                                : firstChild.Attributes["InheritsMasterPage"].Value);
                        sPWeb.AllProperties["__InheritsCustomMasterUrl"] =
                            ((firstChild.Attributes["InheritsCustomMasterPage"] == null)
                                ? "False"
                                : firstChild.Attributes["InheritsCustomMasterPage"].Value);
                        sPWeb.AllProperties["__InheritsAlternateCssUrl"] =
                            ((firstChild.Attributes["InheritsAlternateCssUrl"] == null)
                                ? "False"
                                : firstChild.Attributes["InheritsAlternateCssUrl"].Value);
                        if (num2 == -1 && sPWebTemplate != null)
                        {
                            sPWeb.ApplyWebTemplate(sPWebTemplate);
                        }

                        try
                        {
                            this.UpdateWebProperties(sPWeb, firstChild, addSPWebOptions, iLocaleNew,
                                operationReporting);
                        }
                        catch (Exception ex3)
                        {
                            OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
                            operationReporting.LogError(ex3, "UpdateWebProperties in AddWeb");
                        }
                    }
                    finally
                    {
                        if (sPWeb != null)
                        {
                            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                            xmlTextWriter.WriteStartElement("Web");
                            this.GetWebXML(sPWeb, xmlTextWriter, false);
                            xmlTextWriter.WriteEndElement();
                            operationReporting.LogObjectXml(stringWriter.ToString());
                            sPWeb.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex4)
            {
                OMAdapter.LogExceptionDetails(ex4, MethodBase.GetCurrentMethod().Name, null);
                operationReporting.LogError(ex4, "Main catch in AddWeb");
                operationReporting.LogInformation("WebXml", sWebXML);
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sWebXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                int iLocaleNew = (firstChild.Attributes["Locale"] == null)
                    ? Convert.ToInt32(web.Locale.LCID)
                    : Convert.ToInt32(firstChild.Attributes["Locale"].Value);
                if (updateOptions.CopyFeatures)
                {
                    bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
                    try
                    {
                        if (web.ParentWeb == null)
                        {
                            string sFeatureGuids = (firstChild.Attributes["SiteCollFeatures"] != null)
                                ? firstChild.Attributes["SiteCollFeatures"].Value
                                : "";
                            this.AddFeatures(context.Site.Features, context.Site, sFeatureGuids,
                                updateOptions.MergeFeatures, false);
                        }

                        string sFeatureGuids2 = (firstChild.Attributes["SiteFeatures"] != null)
                            ? firstChild.Attributes["SiteFeatures"].Value
                            : "";
                        this.AddFeatures(web.Features, context.Site, sFeatureGuids2, updateOptions.MergeFeatures,
                            string.Equals(web.WebTemplate, "ENTERWIKI", StringComparison.OrdinalIgnoreCase));
                    }
                    finally
                    {
                        web.AllowUnsafeUpdates = allowUnsafeUpdates;
                    }
                }

                if (web.ParentWeb != null)
                {
                    web.AllProperties["__InheritsMasterUrl"] = ((firstChild.Attributes["InheritsMasterPage"] == null)
                        ? "False"
                        : firstChild.Attributes["InheritsMasterPage"].Value);
                    web.AllProperties["__InheritsCustomMasterUrl"] =
                        ((firstChild.Attributes["InheritsCustomMasterPage"] == null)
                            ? "False"
                            : firstChild.Attributes["InheritsCustomMasterPage"].Value);
                }

                this.UpdateWebProperties(web, firstChild, updateOptions, iLocaleNew, null);
                web.Update();
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Web");
                this.GetWebXML(web, xmlTextWriter, false);
                xmlTextWriter.WriteEndElement();
                result = stringWriter.ToString();
            }

            return result;
        }

        internal void ChangeUIVersion(SPWeb web)
        {
        }

        internal void UpdateWebProperties(SPWeb web, XmlNode webXML, IUpdateWebOptions updateSPWebOptions,
            int iLocaleNew, OperationReporting opResult = null)
        {
            if (updateSPWebOptions.CopyNavigation)
            {
                this.ModifyWebNavigationSettings(web, webXML, opResult);
                web.Update();
            }

            try
            {
                if (updateSPWebOptions.CopyAccessRequestSettings && webXML.Attributes["RequestAccessEmail"] != null)
                {
                    this.EnsureInheritanceIsBroken(web);
                    web.RequestAccessEmail = webXML.Attributes["RequestAccessEmail"].Value;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }

            if (updateSPWebOptions.CopyAssociatedGroupSettings)
            {
                List<string> list = new List<string>();
                if (webXML.Attributes["OwnerGroup"] != null)
                {
                    string value = webXML.Attributes["OwnerGroup"].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        SPGroup sPGroup = OMAdapter.LookupGroup(value, web);
                        if (sPGroup != null)
                        {
                            this.EnsureInheritanceIsBroken(web);
                            web.AssociatedOwnerGroup = sPGroup;
                            list.Add(value);
                        }
                    }
                }
                else if (web.AssociatedOwnerGroup != null)
                {
                    list.Add(web.AssociatedOwnerGroup.Name);
                }

                if (webXML.Attributes["MemberGroup"] != null)
                {
                    string value2 = webXML.Attributes["MemberGroup"].Value;
                    if (!string.IsNullOrEmpty(value2))
                    {
                        SPGroup sPGroup2 = OMAdapter.LookupGroup(value2, web);
                        if (sPGroup2 != null)
                        {
                            this.EnsureInheritanceIsBroken(web);
                            web.AssociatedMemberGroup = sPGroup2;
                            list.Add(value2);
                        }
                    }
                }
                else if (web.AssociatedMemberGroup != null)
                {
                    list.Add(web.AssociatedMemberGroup.Name);
                }

                if (webXML.Attributes["VisitorGroup"] != null)
                {
                    string value3 = webXML.Attributes["VisitorGroup"].Value;
                    if (!string.IsNullOrEmpty(value3))
                    {
                        SPGroup sPGroup3 = OMAdapter.LookupGroup(value3, web);
                        if (sPGroup3 != null)
                        {
                            this.EnsureInheritanceIsBroken(web);
                            web.AssociatedVisitorGroup = sPGroup3;
                            list.Add(value3);
                        }
                    }
                }
                else if (web.AssociatedVisitorGroup != null)
                {
                    list.Add(web.AssociatedVisitorGroup.Name);
                }

                if (webXML.Attributes["AssociateGroups"] != null)
                {
                    this.EnsureInheritanceIsBroken(web);
                    this.SetAssociateGroups(web, webXML.Attributes["AssociateGroups"].Value.ToString(), list);
                }
            }

            if (updateSPWebOptions.CopyCoreMetaData)
            {
                XmlNode xmlNode = webXML.SelectSingleNode("//Fields");
                XmlNode xmlNode2 = webXML.SelectSingleNode(".//MeetingInstances");
                string text = (webXML.Attributes["Description"] != null)
                    ? webXML.Attributes["Description"].Value
                    : null;
                if (text != null)
                {
                    web.Description = text;
                }

                string text2 = (webXML.Attributes["Title"] != null) ? webXML.Attributes["Title"].Value : null;
                if (!string.IsNullOrEmpty(text2))
                {
                    web.Title = text2;
                }

                web.NoCrawl = (webXML.Attributes["NoCrawl"] != null &&
                               !(webXML.Attributes["NoCrawl"].Value == "False"));
                CultureInfo locale = new CultureInfo(iLocaleNew);
                web.Locale = locale;
                if (webXML.Attributes["RegionalSortOrder"] != null)
                {
                    web.RegionalSettings.Collation = Convert.ToInt16(webXML.Attributes["RegionalSortOrder"].Value);
                }

                if (webXML.Attributes["TimeZone"] != null)
                {
                    web.RegionalSettings.TimeZone.ID = Convert.ToUInt16(webXML.Attributes["TimeZone"].Value);
                }

                if (webXML.Attributes["Calendar"] != null)
                {
                    web.RegionalSettings.CalendarType = Convert.ToInt16(webXML.Attributes["Calendar"].Value);
                }

                if (webXML.Attributes["AlternateCalendar"] != null)
                {
                    web.RegionalSettings.ShowWeeks = (webXML.Attributes["ShowWeeks"].Value == "True");
                    web.RegionalSettings.AlternateCalendarType =
                        Convert.ToInt16(webXML.Attributes["AlternateCalendar"].Value);
                    web.RegionalSettings.FirstWeekOfYear = Convert.ToInt16(webXML.Attributes["FirstWeekOfYear"].Value);
                    ushort num = Convert.ToUInt16(webXML.Attributes["FirstDayOfWeek"].Value);
                    web.RegionalSettings.FirstDayOfWeek = (uint)(num % 7);
                    web.RegionalSettings.WorkDays = Convert.ToInt16(webXML.Attributes["WorkDays"].Value);
                    web.RegionalSettings.WorkDayStartHour =
                        Convert.ToInt16(webXML.Attributes["WorkDayStartHour"].Value);
                    web.RegionalSettings.WorkDayEndHour = Convert.ToInt16(webXML.Attributes["WorkDayEndHour"].Value);
                }

                if (webXML.Attributes["TimeFormat"] != null)
                {
                    web.RegionalSettings.Time24 = (webXML.Attributes["TimeFormat"].Value == "True");
                }

                if (webXML.Attributes["AdjustHijriDays"] != null &&
                    !string.IsNullOrEmpty(webXML.Attributes["AdjustHijriDays"].Value))
                {
                    web.RegionalSettings.AdjustHijriDays = Convert.ToInt16(webXML.Attributes["AdjustHijriDays"].Value);
                }

                if (webXML.Attributes["ASPXPageIndexMode"] != null)
                {
                    web.ASPXPageIndexMode = ((webXML.Attributes["ASPXPageIndexMode"].Value == "Never")
                        ? WebASPXPageIndexMode.Never
                        : ((webXML.Attributes["ASPXPageIndexMode"].Value == "Always")
                            ? WebASPXPageIndexMode.Always
                            : WebASPXPageIndexMode.Automatic));
                }

                if (webXML.Attributes["AllowedWebTemplates"] != null)
                {
                    web.AllProperties["__WebTemplates"] = webXML.GetAttributeValueAsString("AllowedWebTemplates");
                }

                if (webXML.Attributes["InheritWebTemplates"] != null)
                {
                    web.AllProperties["__InheritWebTemplates"] =
                        webXML.GetAttributeValueAsString("InheritWebTemplates");
                }

                if (webXML.Attributes["AllowedPageLayouts"] != null)
                {
                    web.AllProperties["__PageLayouts"] = webXML.GetAttributeValueAsString("AllowedPageLayouts");
                }

                if (webXML.Attributes["DefaultPageLayout"] != null)
                {
                    web.AllProperties["__DefaultPageLayout"] = webXML.GetAttributeValueAsString("DefaultPageLayout");
                }

                if (webXML.Attributes["AllowSpacesInNewPageName"] != null)
                {
                    web.AllProperties["__AllowSpacesInNewPageName"] =
                        webXML.GetAttributeValueAsString("AllowSpacesInNewPageName");
                }

                if (web.IsRootWeb && webXML.Attributes["AllowRSSSiteFeeds"] != null)
                {
                    web.Site.SyndicationEnabled = webXML.GetAttributeValueAsBoolean("AllowRSSSiteFeeds");
                }

                if (webXML.Attributes["AllowRSSFeeds"] != null)
                {
                    web.SyndicationEnabled = webXML.GetAttributeValueAsBoolean("AllowRSSFeeds");
                }

                if (webXML.Attributes["RssManagingEditor"] != null)
                {
                    web.AllProperties["vti_rss_ManagingEditor"] =
                        webXML.Attributes["RssManagingEditor"].Value.ToString();
                }

                if (webXML.Attributes["RssCopyright"] != null)
                {
                    web.AllProperties["vti_rss_Copyright"] = webXML.Attributes["RssCopyright"].Value.ToString();
                }

                if (webXML.Attributes["RssWebMaster"] != null)
                {
                    web.AllProperties["vti_rss_WebMaster"] = webXML.Attributes["RssWebMaster"].Value.ToString();
                }

                if (webXML.Attributes["RssTimeToLive"] != null)
                {
                    int num2;
                    if (int.TryParse(webXML.Attributes["RssTimeToLive"].Value, out num2))
                    {
                        web.AllProperties["vti_rss_TimeToLive"] = num2;
                    }
                    else
                    {
                        web.AllProperties["vti_rss_TimeToLive"] = -1;
                    }
                }

                web.SiteLogoUrl = webXML.GetAttributeValueAsString("SiteLogoUrl");
                web.SiteLogoDescription = webXML.GetAttributeValueAsString("SiteLogoDescription");
                if (xmlNode != null)
                {
                    this.AddFieldsXML(web.Fields, xmlNode);
                }

                web.Update();
                if (xmlNode2 != null)
                {
                    this.AddMeetingInstances(xmlNode2, web);
                }
            }

            if (base.SharePointVersion.IsSharePoint2013OrLater && web.WebTemplateId.Equals(62))
            {
                OMAdapter.UpdateCommunitySiteProperties(web, webXML);
            }

            web.Update();
            if (updateSPWebOptions.ApplyTheme)
            {
                string text3 = (webXML.Attributes["SiteTheme"] != null) ? webXML.Attributes["SiteTheme"].Value : null;
                if (!string.IsNullOrEmpty(text3) && base.SharePointVersion.IsSharePoint2007OrEarlier)
                {
                    web.ApplyTheme(text3);
                }
            }
        }

        private static void UpdateCommunitySiteProperties(SPWeb web, XmlNode webXML)
        {
            try
            {
                XmlNode xmlNode = webXML.SelectSingleNode("//CommunitySiteProperties");
                if (xmlNode != null)
                {
                    string attributeValueAsString = xmlNode.GetAttributeValueAsString("Community_MembersCount");
                    if (!string.IsNullOrEmpty(attributeValueAsString))
                    {
                        web.AllProperties["Community_MembersCount"] = attributeValueAsString;
                    }

                    string attributeValueAsString2 = xmlNode.GetAttributeValueAsString("Community_TopicsCount");
                    if (!string.IsNullOrEmpty(attributeValueAsString2))
                    {
                        web.AllProperties["Community_TopicsCount"] = attributeValueAsString2;
                    }

                    string attributeValueAsString3 = xmlNode.GetAttributeValueAsString("Community_RepliesCount");
                    if (!string.IsNullOrEmpty(attributeValueAsString3))
                    {
                        web.AllProperties["Community_RepliesCount"] = attributeValueAsString3;
                    }
                }
            }
            catch (Exception arg)
            {
                Trace.WriteLine(string.Format(
                    "Error occurred while updating community site properties for web '{0}'. Error: {1}", web.Url, arg));
            }
        }

        private void AddSupportedUICulture(SPWeb web, XmlNode webXML, OperationReporting opResult)
        {
        }

        private void AddLanguageResources(XmlNode languageResourceNode, object obj)
        {
        }

        private void ModifyRecordCenterProperties(SPWeb web, XmlNode webXml)
        {
            if (webXml.Attributes["SiteRecordDeclarationBy"] != null)
            {
                web.AllProperties["ecm_siterecorddeclarationby"] =
                    webXml.GetAttributeValueAsString("SiteRecordDeclarationBy");
            }

            if (webXml.Attributes["SiteRecordUndeclarationBy"] != null)
            {
                web.AllProperties["ecm_siterecordundeclarationby"] =
                    webXml.GetAttributeValueAsString("SiteRecordUndeclarationBy");
            }

            if (webXml.Attributes["SiteRecordRestrictions"] != null)
            {
                web.AllProperties["ecm_siterecordrestrictions"] =
                    webXml.GetAttributeValueAsString("SiteRecordRestrictions");
            }

            if (webXml.Attributes["SiteRecordDeclarationDefault"] != null)
            {
                web.AllProperties["ecm_siterecorddeclarationdefault"] =
                    webXml.GetAttributeValueAsString("SiteRecordDeclarationDefault");
            }

            if (webXml.Attributes["WebHasIPRenabled"] != null)
            {
                web.AllProperties["ecm_webhasiprenabled"] = webXml.GetAttributeValueAsString("WebHasIPRenabled");
            }
        }

        private void EnsureInheritanceIsBroken(SPWeb web)
        {
            if (!web.HasUniqueRoleAssignments)
            {
                web.Update();
                web.BreakRoleInheritance(true);
            }
        }

        private void SetAssociateGroups(SPWeb web, string groups, List<string> groupsToLeave)
        {
            if (!string.IsNullOrEmpty(groups))
            {
                List<string> list = new List<string>(groups.Split(new char[]
                {
                    ';'
                }, StringSplitOptions.RemoveEmptyEntries));
                List<SPGroup> list2 = new List<SPGroup>();
                foreach (SPGroup current in web.AssociatedGroups)
                {
                    if (list.Contains(current.Name))
                    {
                        list.Remove(current.Name);
                    }
                    else if (!groupsToLeave.Contains(current.Name))
                    {
                        list2.Add(current);
                    }
                }

                foreach (SPGroup current2 in list2)
                {
                    web.AssociatedGroups.Remove(current2);
                }

                foreach (string current3 in list)
                {
                    SPGroup sPGroup = OMAdapter.LookupGroup(current3, web);
                    if (sPGroup != null)
                    {
                        web.AssociatedGroups.Add(sPGroup);
                    }
                }
            }
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            return string.Empty;
        }

        public string SetWelcomePage(string WelcomePage)
        {
            if (OMAdapter.SupportsPublishing)
            {
                this.WelcomePageSetting(WelcomePage);
            }
            else
            {
                using (Context context = this.GetContext())
                {
                    this.SetWelcomePageOnRootFolder(context.Web, WelcomePage);
                }
            }

            return string.Empty;
        }

        private void SetWelcomePageOnRootFolder(SPWeb web, string sWelcomePage)
        {
            SPFile file = web.GetFile(sWelcomePage);
            if (file.Exists)
            {
                web.RootFolder.WelcomePage = sWelcomePage;
            }
        }

        internal void AddFeatures(SPFeatureCollection featureCollection, SPSite site, string sFeatureGuids,
            bool bMergeFeatures, bool bIgnorePublishingFeature)
        {
            string[] array = sFeatureGuids.Split(new char[]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> list = new List<Guid>(array.Length);
            List<Guid> list2 = new List<Guid>();
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string g = array2[i];
                list.Add(new Guid(g));
            }

            bMergeFeatures = (bMergeFeatures || list.Contains(OMAdapter.RECORDSCENTERGUID));
            foreach (SPFeature sPFeature in ((IEnumerable)featureCollection))
            {
                if (list.Contains(sPFeature.DefinitionId))
                {
                    list.Remove(sPFeature.DefinitionId);
                }
                else
                {
                    list2.Add(sPFeature.DefinitionId);
                }
            }

            if (!bMergeFeatures)
            {
                foreach (Guid current in list2)
                {
                    if (this.HasFeature(featureCollection, current))
                    {
                        featureCollection.Remove(current, true);
                    }
                }
            }

            if (bIgnorePublishingFeature && list.Contains(OMAdapter.PUBLISHINGGUID))
            {
                list.Remove(OMAdapter.PUBLISHINGGUID);
            }

            if (list.Contains(OMAdapter.RECORDSCENTERGUID))
            {
                list.Remove(OMAdapter.RECORDSCENTERGUID);
            }

            foreach (Guid current2 in list)
            {
                try
                {
                    featureCollection.Add(current2);
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }
            }
        }

        private bool HasFeature(SPWeb web, Guid featureGuid)
        {
            return web != null && this.HasFeature(web.Features, featureGuid);
        }

        private bool HasFeature(SPFeatureCollection features, Guid featureGuid)
        {
            if (featureGuid != Guid.Empty)
            {
                foreach (SPFeature sPFeature in ((IEnumerable)features))
                {
                    if (sPFeature.DefinitionId == featureGuid)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        internal void ClearDefaultSiteData(SPWeb web)
        {
            if (web == null)
            {
                return;
            }

            this.ClearTeamSiteWikiField(web);
        }

        private void ClearTeamSiteWikiField(SPWeb web)
        {
            if (base.SharePointVersion.IsSharePoint2010OrLater && web.WebTemplate == "STS" &&
                this.HasFeature(web, OMAdapter.WIKIHOMEPAGEFEATUREGUID))
            {
                try
                {
                    SPFile sPFile = null;
                    string text = OMAdapter.SupportsPublishing
                        ? this.GetWelcomePage(web)
                        : this.GetWelcomePageFromRootFolder(web);
                    if (!string.IsNullOrEmpty(text))
                    {
                        sPFile = web.GetFile(text);
                    }

                    if (sPFile == null || !sPFile.Exists)
                    {
                        sPFile = web.GetFile("SitePages/home.aspx");
                    }

                    if (sPFile == null || !sPFile.Exists)
                    {
                        throw new Exception("Could not retrieve default page of site.");
                    }

                    string text2 = "WikiField";
                    if (OMAdapter.HasUnderlyingItem(sPFile) && sPFile.Item.Properties.ContainsKey(text2) &&
                        sPFile.GetLimitedWebPartManager(PersonalizationScope.Shared).WebParts.Count > 0)
                    {
                        string value =
                            "<table id=\"layoutsTable\" style=\"width: 100%\"> \r\n                                                                    <tbody> \r\n                                                                        <tr style=\"vertical-align: top\"> \r\n                                                                            <td style=\"width: 66.6%\"> \r\n                                                                                <div class=\"ms-rte-layoutszone-outer\" style=\"width: 100%\"> \r\n                                                                                    <div class=\"ms-rte-layoutszone-inner\" style=\"min-height: 60px; word-wrap: break-word\">                              \r\n                                                                                    </div> \r\n                                                                                </div> \r\n                                                                            </td> \r\n                                                                            <td style=\"width: 33.3%\"> \r\n                                                                                <div class=\"ms-rte-layoutszone-outer\" style=\"width: 100%\"> \r\n                                                                                    <div class=\"ms-rte-layoutszone-inner\" style=\"min-height: 60px; word-wrap: break-word\">                             \r\n                                                                                    </div> \r\n                                                                                </div> \r\n                                                                            </td> \r\n                                                                        </tr> \r\n                                                                    </tbody> \r\n                                                                </table><span id=\"layoutsData\" style=\"display:none\">false,false,2</span>";
                        sPFile.Item[text2] = value;
                        sPFile.Item.SystemUpdate(false);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception(
                        "The default site data on a team site (" + web.Url + ") could not be cleared. Message: " +
                        ex.Message, ex);
                }
            }
        }

        private static bool HasUnderlyingItem(SPFile file)
        {
            bool result;
            try
            {
                if (file.Item != null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (SPException ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = false;
            }

            return result;
        }

        public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
        {
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                this.ModifyWebNavigationSettings(web, XmlUtility.StringToXmlNode(sWebXML), null);
                web.Update();
            }

            return string.Empty;
        }

        public string SetMasterPage(string sWebXML)
        {
            using (Context context = this.GetContext())
            {
                using (SPWeb sPWeb = context.Site.OpenWeb(context.Site.ServerRelativeUrl))
                {
                    SPWeb web = context.Web;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(sWebXML);
                    XmlNode firstChild = xmlDocument.FirstChild;
                    if (firstChild.Attributes["CustomMasterPage"] != null)
                    {
                        SPFile file = sPWeb.GetFile(this.MakeMasterPageUrlWebRelative(sPWeb,
                            firstChild.Attributes["CustomMasterPage"].Value));
                        if (file.Exists)
                        {
                            web.CustomMasterUrl = file.ServerRelativeUrl;
                        }
                    }

                    if (firstChild.Attributes["MasterPage"] != null)
                    {
                        SPFile file2 =
                            sPWeb.GetFile(this.MakeMasterPageUrlWebRelative(sPWeb,
                                firstChild.Attributes["MasterPage"].Value));
                        if (file2.Exists)
                        {
                            web.AllProperties["OldMasterUrl"] = web.MasterUrl;
                            web.AllProperties["OldCustomMasterUrl"] = web.CustomMasterUrl;
                            if (!string.IsNullOrEmpty(web.Theme))
                            {
                                web.AllProperties["OldThemeName"] = web.Theme;
                                if (web.AllProperties.ContainsKey("vti_themedefault"))
                                {
                                    web.AllProperties["OldThemeProperty"] =
                                        web.AllProperties["vti_themedefault"].ToString();
                                }

                                web.AllProperties["vti_themedefault"] = "none";
                                web.ApplyTheme("none");
                            }

                            web.MasterUrl = file2.ServerRelativeUrl;
                        }
                    }

                    if (firstChild.Attributes["AlternateCssUrl"] != null)
                    {
                        web.AlternateCssUrl = firstChild.Attributes["AlternateCssUrl"].Value;
                    }

                    web.Update();
                }
            }

            return string.Empty;
        }

        private string MakeMasterPageUrlWebRelative(SPWeb rootWeb, string masterPageUrl)
        {
            if (string.IsNullOrEmpty(masterPageUrl))
            {
                return masterPageUrl;
            }

            string text;
            if (!UrlUtils.ReplaceStart(masterPageUrl, rootWeb.ServerRelativeUrl, "", out text))
            {
                return text;
            }

            return text.TrimStart(new char[]
            {
                '/'
            });
        }

        private void ModifyWebNavigationSettings(SPWeb web, XmlNode webXml, OperationReporting opResult = null)
        {
            if (web == null || webXml == null)
            {
                return;
            }

            bool? attributeValueAsNullableBoolean = webXml.GetAttributeValueAsNullableBoolean("QuickLaunchEnabled");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                web.QuickLaunchEnabled = attributeValueAsNullableBoolean.Value;
            }

            attributeValueAsNullableBoolean = webXml.GetAttributeValueAsNullableBoolean("TreeViewEnabled");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                web.TreeViewEnabled = attributeValueAsNullableBoolean.Value;
            }

            try
            {
                attributeValueAsNullableBoolean = webXml.GetAttributeValueAsNullableBoolean("InheritGlobalNavigation");
                if (!web.IsRootWeb && attributeValueAsNullableBoolean.HasValue && web.Navigation != null)
                {
                    bool useShared = attributeValueAsNullableBoolean.Value;
                    Action codeBlockToRetry = delegate { web.Navigation.UseShared = useShared; };
                    ExecuteWithRetry.AttemptToExecuteWithRetry(codeBlockToRetry, null);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                if (opResult == null)
                {
                    throw;
                }

                opResult.LogError(ex,
                    string.Format("Attempting to set web.Navigation.UseShared = {0}",
                        attributeValueAsNullableBoolean.HasValue ? attributeValueAsNullableBoolean.ToString() : "?"));
            }

            attributeValueAsNullableBoolean = webXml.GetAttributeValueAsNullableBoolean("InheritCurrentNavigation");
            if (!web.IsRootWeb && attributeValueAsNullableBoolean.HasValue)
            {
                web.AllProperties["__InheritCurrentNavigation"] = webXml.Attributes["InheritCurrentNavigation"].Value;
            }

            string[] array = new string[]
            {
                "NavigationSortAscending",
                "NavigationShowSiblings",
                "DisplayShowHideRibbonActionId"
            };
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                attributeValueAsNullableBoolean = webXml.GetAttributeValueAsNullableBoolean(text);
                if (attributeValueAsNullableBoolean.HasValue)
                {
                    web.AllProperties["__" + text] = webXml.Attributes[text].Value;
                }
            }

            string[] array3 = new string[]
            {
                "NavigationOrderingMethod",
                "NavigationAutomaticSortingMethod",
                "GlobalDynamicChildLimit",
                "CurrentDynamicChildLimit"
            };
            string[] array4 = array3;
            for (int j = 0; j < array4.Length; j++)
            {
                string text2 = array4[j];
                int num;
                if (XmlUtility.GetIntegerAttributeFromXml(webXml, text2, out num))
                {
                    web.AllProperties["__" + text2] = num;
                }
            }

            bool? showPagesInCurrentNav = null;
            bool? showSubSitesInCurrentNav = null;
            bool? showPagesInGlobalNav = null;
            bool? showSubSitesInGlobalNav = null;
            attributeValueAsNullableBoolean =
                webXml.GetAttributeValueAsNullableBoolean("IncludePagesInCurrentNavigation");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                showPagesInCurrentNav = new bool?(attributeValueAsNullableBoolean.Value);
            }

            attributeValueAsNullableBoolean =
                webXml.GetAttributeValueAsNullableBoolean("IncludeSubSitesInCurrentNavigation");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                showSubSitesInCurrentNav = new bool?(attributeValueAsNullableBoolean.Value);
            }

            attributeValueAsNullableBoolean =
                webXml.GetAttributeValueAsNullableBoolean("IncludePagesInGlobalNavigation");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                showPagesInGlobalNav = new bool?(attributeValueAsNullableBoolean.Value);
            }

            attributeValueAsNullableBoolean =
                webXml.GetAttributeValueAsNullableBoolean("IncludeSubSitesInGlobalNavigation");
            if (attributeValueAsNullableBoolean.HasValue)
            {
                showSubSitesInGlobalNav = new bool?(attributeValueAsNullableBoolean.Value);
            }

            string attributeValueAsString = webXml.Attributes.GetAttributeValueAsString("SiteCollFeatures");
            if (base.SharePointVersion.IsSharePoint2013OrLater && OMAdapter.SupportsPublishing &&
                !string.IsNullOrEmpty(attributeValueAsString) &&
                attributeValueAsString.Contains("f6924d36-2fa8-4f0b-b16d-06b7250180fa"))
            {
                OMAdapter.SetPublishingWebNavigationSettings(web, webXml, showPagesInCurrentNav,
                    showSubSitesInCurrentNav, showPagesInGlobalNav, showSubSitesInGlobalNav);
                return;
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                if (showPagesInCurrentNav.HasValue || showPagesInGlobalNav.HasValue ||
                    showSubSitesInCurrentNav.HasValue || showSubSitesInGlobalNav.HasValue)
                {
                    int num2 = 0;
                    int num3 = 0;
                    if (!showPagesInCurrentNav.HasValue || !showPagesInGlobalNav.HasValue ||
                        !showSubSitesInCurrentNav.HasValue || !showSubSitesInGlobalNav.HasValue)
                    {
                        if (web.AllProperties.ContainsKey("__CurrentNavigationIncludeTypes") &&
                            !int.TryParse(web.AllProperties["__CurrentNavigationIncludeTypes"].ToString(), out num3))
                        {
                            num3 = 0;
                        }

                        if (web.AllProperties.ContainsKey("__GlobalNavigationIncludeTypes") &&
                            !int.TryParse(web.AllProperties["__GlobalNavigationIncludeTypes"].ToString(), out num2))
                        {
                            num2 = 0;
                        }
                    }

                    if (showPagesInCurrentNav.HasValue)
                    {
                        num3 = ((num3 & 1) | (showPagesInCurrentNav.Value ? 2 : 0));
                    }

                    if (showSubSitesInCurrentNav.HasValue)
                    {
                        num3 = ((num3 & 2) | (showSubSitesInCurrentNav.Value ? 1 : 0));
                    }

                    if (showPagesInGlobalNav.HasValue)
                    {
                        num2 = ((num2 & 1) | (showPagesInGlobalNav.Value ? 2 : 0));
                    }

                    if (showSubSitesInGlobalNav.HasValue)
                    {
                        num2 = ((num2 & 2) | (showSubSitesInGlobalNav.Value ? 1 : 0));
                    }

                    web.AllProperties["__GlobalNavigationIncludeTypes"] = num2;
                    web.AllProperties["__CurrentNavigationIncludeTypes"] = num3;
                    if (web.AllProperties.ContainsKey("__IncludePagesInNavigation"))
                    {
                        web.AllProperties.Remove("__IncludePagesInNavigation");
                    }

                    if (web.AllProperties.ContainsKey("__IncludeSubSitesInNavigation"))
                    {
                        web.AllProperties.Remove("__IncludeSubSitesInNavigation");
                        return;
                    }
                }
            }
            else
            {
                bool flag = (showPagesInCurrentNav.HasValue && showPagesInCurrentNav.Value) ||
                            (showPagesInGlobalNav.HasValue && showPagesInGlobalNav.Value);
                bool flag2 = (showSubSitesInCurrentNav.HasValue && showSubSitesInCurrentNav.Value) ||
                             (showSubSitesInGlobalNav.HasValue && showSubSitesInGlobalNav.Value);
                if (showPagesInCurrentNav.HasValue || showPagesInGlobalNav.HasValue)
                {
                    web.AllProperties["__IncludePagesInNavigation"] = flag.ToString();
                }

                if (showSubSitesInCurrentNav.HasValue || showSubSitesInGlobalNav.HasValue)
                {
                    web.AllProperties["__IncludeSubSitesInNavigation"] = flag2.ToString();
                }
            }
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            using (Context context = this.GetContext())
            {
                using (SPWeb sPWeb = context.Site.OpenWeb(sServerRelativeUrl))
                {
                    this.DeleteWeb(sPWeb);
                }
            }

            return string.Empty;
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFieldXML);
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                if (sListID != null)
                {
                    xmlTextWriter.WriteStartElement("Fields");
                    SPFieldCollection fields = web.Lists[new Guid(sListID)].Fields;
                    this.AddFieldsXML(fields, xmlDocument.DocumentElement);
                    foreach (SPField sPField in ((IEnumerable)fields))
                    {
                        xmlTextWriter.WriteRaw(this.AddListNamesToLookups(web, sPField.SchemaXml));
                    }

                    xmlTextWriter.WriteEndElement();
                }
                else
                {
                    SPFieldCollection fields = web.Fields;
                    this.AddFieldsXML(fields, xmlDocument.DocumentElement);
                    xmlTextWriter.WriteStartElement("Fields");
                    foreach (SPField sPField2 in ((IEnumerable)web.AvailableFields))
                    {
                        xmlTextWriter.WriteRaw(this.AddListNamesToLookups(web, sPField2.SchemaXml));
                    }

                    xmlTextWriter.WriteEndElement();
                }
            }

            return stringBuilder.ToString();
        }

        private void AddMeetingInstances(XmlNode meetingInstanceXML, SPWeb web)
        {
            if (!SPMeeting.IsMeetingWorkspaceWeb(web))
            {
                return;
            }

            XmlNodeList xmlNodeList = meetingInstanceXML.SelectNodes(".//MeetingInstance");
            if (xmlNodeList.Count == 0)
            {
                return;
            }

            SPList sPList = null;
            foreach (SPList sPList2 in ((IEnumerable)web.Lists))
            {
                if (sPList2.BaseTemplate == SPListTemplateType.Meetings)
                {
                    sPList = sPList2;
                    break;
                }
            }

            if (sPList == null)
            {
                return;
            }

            XmlNode xmlNode = null;
            foreach (XmlNode xmlNode2 in xmlNodeList)
            {
                if ((xmlNode2.Attributes["RecurrenceData"] != null &&
                     !string.IsNullOrEmpty(xmlNode2.Attributes["RecurrenceData"].Value)) ||
                    (xmlNode2.Attributes["RRule"] != null && !string.IsNullOrEmpty(xmlNode2.Attributes["RRule"].Value)))
                {
                    xmlNode = xmlNode2;
                    break;
                }
            }

            if (xmlNode != null && sPList.ItemCount > 0)
            {
                return;
            }

            List<int> list = new List<int>();
            bool flag;
            SPListItemCollection[] fullListItemCollections = this.GetFullListItemCollections(sPList, out flag);
            SPListItemCollection[] array = fullListItemCollections;
            for (int i = 0; i < array.Length; i++)
            {
                SPListItemCollection sPListItemCollection = array[i];
                foreach (SPListItem sPListItem in ((IEnumerable)sPListItemCollection))
                {
                    list.Add(sPListItem.ID);
                }
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (meetingInstanceXML.SelectSingleNode(".//MeetingInstance/@EventUIDListName") != null)
            {
                using (SPWeb sPWeb = web.Site.OpenWeb(web.ParentWebId))
                {
                    foreach (SPList sPList3 in ((IEnumerable)sPWeb.Lists))
                    {
                        if (!dictionary.ContainsKey(sPList3.Title))
                        {
                            dictionary.Add(sPList3.Title, sPList3.ID.ToString());
                        }
                    }
                }
            }

            if (xmlNode == null)
            {
                IEnumerator enumerator5 = xmlNodeList.GetEnumerator();
                while (enumerator5.MoveNext())
                {
                    XmlNode xmlNode3 = (XmlNode)enumerator5.Current;
                    int num = int.Parse(xmlNode3.Attributes["ID"].Value);
                    if (!list.Contains(num))
                    {
                        if (xmlNode3.Attributes["EventUID"] == null)
                        {
                            this.ReCombineEventUID(xmlNode3, dictionary);
                        }

                        this.CreateMeetingInstance(sPList, xmlNode3, num);
                    }
                }

                return;
            }

            this.ReCombineEventUID(xmlNode, dictionary);
            this.CreateMeetingInstance(sPList, xmlNode, 0);
            List<XmlNode> list2 = new List<XmlNode>();
            foreach (XmlNode xmlNode4 in xmlNodeList)
            {
                if (xmlNode4 != xmlNode)
                {
                    list2.Add(xmlNode4);
                }
            }

            this.InitalizeInstanceIDs(web, list2.ToArray());
        }

        private int CreateMeetingInstance(SPList meetingSeriesList, XmlNode meetingInstance, int iMeetingInstanceID)
        {
            SPListItemCollection emptyListItemCollection = this.GetEmptyListItemCollection(meetingSeriesList, false);
            SPListItem sPListItem = emptyListItemCollection.Add();
            this.UpdateItemMetadata(meetingSeriesList, sPListItem, meetingInstance, false);
            if (iMeetingInstanceID > 0)
            {
                if (base.SharePointVersion.IsSharePoint2010OrLater)
                {
                    this.IncrementMeetingInstanceIDs(emptyListItemCollection, meetingInstance, iMeetingInstanceID);
                }

                MethodInfo method = sPListItem.GetType()
                    .GetMethod("SetIDForMigration", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(sPListItem, new object[]
                {
                    iMeetingInstanceID
                });
            }

            bool flag = false;
            if (sPListItem.Fields["IsOrphaned"] != null && meetingInstance.Attributes["IsOrphaned"] != null &&
                meetingInstance.Attributes["IsOrphaned"].Value.ToLower() == "true")
            {
                sPListItem["IsOrphaned"] = false;
                flag = true;
            }

            bool flag2 = false;
            if (meetingInstance.Attributes["fAllDayEvent"] != null)
            {
                bool flag3;
                bool.TryParse(meetingInstance.Attributes["fAllDayEvent"].Value, out flag3);
                if (meetingInstance.Attributes["fAllDayEvent"].Value == "1" || flag3)
                {
                    flag2 = true;
                }
            }

            if (!flag2 && meetingInstance.Attributes["Duration"] != null &&
                !string.IsNullOrEmpty(meetingInstance.Attributes["Duration"].Value))
            {
                int seconds = int.Parse(meetingInstance.Attributes["Duration"].Value);
                DateTime dateTime = (DateTime)sPListItem["EventDate"];
                dateTime = dateTime.Add(new TimeSpan(0, 0, seconds));
                DateTime dateTime2 = (DateTime)sPListItem["EndDate"];
                dateTime = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime.Hour, dateTime.Minute,
                    dateTime.Second, DateTimeKind.Utc);
                sPListItem["EndDate"] = dateTime;
            }

            try
            {
                sPListItem.Update();
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw this.TransformItemUpdateException(ex);
            }

            if (flag)
            {
                try
                {
                    sPListItem["IsOrphaned"] = true;
                    sPListItem.Update();
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception("Failed to preserve orphaned status: " +
                                        this.TransformItemUpdateException(ex2).Message);
                }
            }

            return sPListItem.ID;
        }

        private void IncrementMeetingInstanceIDs(SPListItemCollection meetingSeriesListItems, XmlNode meetingInstance,
            int iTargetID)
        {
            try
            {
                int num = this.IncrementMeetingIDByOne(meetingSeriesListItems, meetingInstance);
                for (int i = num; i < iTargetID; i++)
                {
                    this.IncrementMeetingIDByOne(meetingSeriesListItems, meetingInstance);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("Failed to increment item IDs: " + ex.Message);
            }
        }

        private int IncrementMeetingIDByOne(SPListItemCollection meetingSeriesListItems, XmlNode meetingInstance)
        {
            SPListItem sPListItem = meetingSeriesListItems.Add();
            this.UpdateItemMetadata(meetingSeriesListItems.List, sPListItem, meetingInstance, false);
            if (sPListItem.Fields["IsOrphaned"] != null)
            {
                sPListItem["IsOrphaned"] = false;
            }

            bool flag = false;
            if (meetingInstance.Attributes["fAllDayEvent"] != null)
            {
                bool flag2;
                bool.TryParse(meetingInstance.Attributes["fAllDayEvent"].Value, out flag2);
                if (meetingInstance.Attributes["fAllDayEvent"].Value == "1" || flag2)
                {
                    flag = true;
                }
            }

            if (!flag && meetingInstance.Attributes["Duration"] != null &&
                meetingInstance.Attributes["Duration"].Value != "0" &&
                !string.IsNullOrEmpty(meetingInstance.Attributes["Duration"].Value))
            {
                int seconds = int.Parse(meetingInstance.Attributes["Duration"].Value);
                DateTime dateTime = (DateTime)sPListItem["EventDate"];
                dateTime = dateTime.Add(new TimeSpan(0, 0, seconds));
                DateTime dateTime2 = (DateTime)sPListItem["EndDate"];
                dateTime = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime.Hour, dateTime.Minute,
                    dateTime.Second, DateTimeKind.Utc);
                sPListItem["EndDate"] = dateTime;
            }

            int result = -1;
            try
            {
                sPListItem.Update();
                result = sPListItem.ID;
                sPListItem.Delete();
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw this.TransformItemUpdateException(ex);
            }

            foreach (SPList sPList in ((IEnumerable)meetingSeriesListItems.List.Lists))
            {
                if (sPList.MultipleDataList && sPList.BaseType == SPBaseType.DocumentLibrary)
                {
                    try
                    {
                        SPFolder sPFolder = sPList.RootFolder.SubFolders[result.ToString()];
                        if (sPFolder != null)
                        {
                            sPFolder.Delete();
                        }
                    }
                    catch (Exception ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    }
                }
            }

            return result;
        }

        private string ReCombineEventUID(XmlNode meetingInstance, Dictionary<string, string> ListTitleToIDMap)
        {
            XmlAttribute xmlAttribute = meetingInstance.Attributes["EventUIDPrefix"];
            XmlAttribute xmlAttribute2 = meetingInstance.Attributes["EventUIDListName"];
            XmlAttribute xmlAttribute3 = meetingInstance.Attributes["EventUIDItemID"];
            if (xmlAttribute == null || xmlAttribute2 == null || xmlAttribute3 == null)
            {
                return null;
            }

            string text = ListTitleToIDMap.ContainsKey(xmlAttribute2.Value)
                ? ListTitleToIDMap[xmlAttribute2.Value]
                : xmlAttribute2.Value;
            if (!text.StartsWith("{", StringComparison.Ordinal) && !text.EndsWith("}", StringComparison.Ordinal))
            {
                text = "{" + text + "}";
            }

            string text2 = string.Concat(new string[]
            {
                xmlAttribute.Value,
                ":List:",
                text,
                ":Item:",
                xmlAttribute3.Value
            });
            XmlAttribute xmlAttribute4 = meetingInstance.OwnerDocument.CreateAttribute("EventUID");
            xmlAttribute4.Value = text2;
            meetingInstance.Attributes.Remove(xmlAttribute);
            meetingInstance.Attributes.Remove(xmlAttribute2);
            meetingInstance.Attributes.Remove(xmlAttribute3);
            meetingInstance.Attributes.Append(xmlAttribute4);
            return text2;
        }

        private void InitalizeInstanceIDs(SPWeb web, XmlNode[] meetingInstances)
        {
            if (meetingInstances == null || meetingInstances.Length == 0)
            {
                return;
            }

            SPList sPList = null;
            foreach (SPList sPList2 in ((IEnumerable)web.Lists))
            {
                if (sPList2.BaseTemplate == SPListTemplateType.Meetings)
                {
                    sPList = sPList2;
                    break;
                }
            }

            if (sPList == null)
            {
                return;
            }

            for (int i = 0; i < meetingInstances.Length; i++)
            {
                XmlNode xmlNode = meetingInstances[i];
                bool flag = false;
                string s = null;
                XmlAttribute xmlAttribute = xmlNode.Attributes["EventType"];
                if (xmlAttribute != null)
                {
                    if (xmlAttribute.Value == "3")
                    {
                        XmlAttribute xmlAttribute2 = xmlNode.Attributes["EndDate"];
                        if (xmlAttribute2 != null && string.IsNullOrEmpty(xmlAttribute2.Value))
                        {
                            XmlAttribute xmlAttribute3 = xmlNode.Attributes["EventDate"];
                            if (xmlAttribute3 != null)
                            {
                                xmlAttribute2.Value = xmlAttribute3.Value;
                            }
                        }
                    }
                    else if (xmlAttribute.Value == "0")
                    {
                        xmlAttribute.Value = "2";
                        s = "0";
                        flag = true;
                    }
                }

                bool flag2 = false;
                XmlAttribute xmlAttribute4 = xmlNode.Attributes["RecurrenceID"];
                if (xmlAttribute4 != null && string.IsNullOrEmpty(xmlAttribute4.Value))
                {
                    xmlAttribute4.Value = xmlNode.Attributes["EventDate"].Value;
                    flag2 = true;
                }

                SPListItem sPListItem = this.GetEmptyListItemCollection(sPList, false).Add();
                this.UpdateItemMetadata(sPList, sPListItem, xmlNode, false);
                sPListItem.Update();
                if (flag || flag2)
                {
                    if (flag)
                    {
                        sPListItem["EventType"] = int.Parse(s);
                    }

                    if (flag2)
                    {
                        sPListItem["RecurrenceID"] = null;
                    }

                    sPListItem.Update();
                }
            }
        }

        public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sContentTypeXML);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//ContentType");
            if (xmlNode == null)
            {
                throw new ArgumentException("The given content type xml is not valid");
            }

            List<string> list = new List<string>();
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                string value = xmlNode.Attributes["Name"].Value;
                SPContentTypeId empty = SPContentTypeId.Empty;
                XmlAttribute xmlAttribute = xmlNode.Attributes["ID"];
                if (xmlAttribute != null)
                {
                    empty = new SPContentTypeId(xmlAttribute.Value);
                }

                SPContentType sPContentType;
                SPContentType sPContentType2;
                this.GetExistingContentTypesFromWeb(value, empty, web, out sPContentType, out sPContentType2);
                if (sPContentType2 == null)
                {
                    SPContentType sPContentType3 = null;
                    if (sPContentType3 == null && sParentContentTypeName != null && sParentContentTypeName.Length != 0)
                    {
                        sPContentType3 = web.AvailableContentTypes[sParentContentTypeName];
                    }

                    if (sPContentType3 == null)
                    {
                        string value2 = xmlNode.Attributes["ID"].Value;
                        int num = 0;
                        FieldInfo[] fields =
                            typeof(SPBuiltInContentTypeId).GetFields(BindingFlags.Static | BindingFlags.Public);
                        FieldInfo[] array = fields;
                        for (int i = 0; i < array.Length; i++)
                        {
                            FieldInfo fieldInfo = array[i];
                            if (fieldInfo.FieldType == typeof(SPContentTypeId))
                            {
                                SPContentTypeId id = (SPContentTypeId)fieldInfo.GetValue(fieldInfo.ReflectedType);
                                string text = id.ToString();
                                if (value2.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) &&
                                    text.Length > num)
                                {
                                    sPContentType3 = web.AvailableContentTypes[id];
                                    num = text.Length;
                                }
                            }
                        }
                    }

                    sPContentType = new SPContentType(sPContentType3, web.ContentTypes, value);
                    sPContentType = web.ContentTypes.Add(sPContentType);
                }

                if (sPContentType != null)
                {
                    XmlNode xmlNode2 = xmlNode.Attributes["Description"];
                    if (xmlNode2 != null)
                    {
                        sPContentType.Description = xmlNode2.Value;
                    }

                    XmlNode xmlNode3 = xmlNode.Attributes["Group"];
                    if (xmlNode3 != null)
                    {
                        sPContentType.Group = xmlNode3.Value;
                    }

                    XmlNode xmlNode4 = xmlNode.SelectSingleNode(".//DocumentTemplate");
                    if (xmlNode4 != null)
                    {
                        XmlAttribute xmlAttribute2 = xmlNode4.Attributes["TargetName"];
                        if (xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value))
                        {
                            sPContentType.DocumentTemplate = xmlAttribute2.Value;
                        }
                    }

                    List<string> list2 = new List<string>();
                    CaseInvariantFieldCollection caseInvariantFieldCollection =
                        new CaseInvariantFieldCollection(web.AvailableFields);
                    XmlNodeList xmlNodeList = xmlNode.SelectNodes(".//FieldRef");
                    foreach (XmlNode xmlNode5 in xmlNodeList)
                    {
                        string text2 = xmlNode5.Attributes["Name"].Value;
                        SPField sPField = OMAdapter.GetFieldById(sPContentType.Fields, xmlNode5.Attributes["ID"].Value);
                        if (sPField == null)
                        {
                            sPField = caseInvariantFieldCollection.GetFieldByNames(null, text2);
                            if (sPField != null)
                            {
                                try
                                {
                                    SPFieldLink sPFieldLink = new SPFieldLink(sPField);
                                    this.UpdateColumnSettings(xmlNode5, sPFieldLink);
                                    this.SetInfoPathXMLDocumentProperties(xmlNode5, sPFieldLink);
                                    sPContentType.FieldLinks.Add(sPFieldLink);
                                }
                                catch (Exception ex)
                                {
                                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                                }

                                this.EnsureTaxonomyRelatedFieldsPresent(sPField, sPContentType, ref list2);
                            }
                        }
                        else
                        {
                            SPFieldLink existingfieldLink = sPContentType.FieldLinks[sPField.Id];
                            this.UpdateColumnSettings(xmlNode5, existingfieldLink);
                        }

                        if (sPField != null &&
                            !sPField.InternalName.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
                        {
                            text2 = sPField.InternalName;
                        }

                        if (!list2.Contains(text2))
                        {
                            list2.Add(text2);
                        }
                    }

                    foreach (XmlNode xmlNode6 in xmlNode.SelectNodes(".//Field"))
                    {
                        string value3 = xmlNode6.Attributes["Name"].Value;
                        string fieldId = (xmlNode6.Attributes["ID"] != null)
                            ? xmlNode6.Attributes["ID"].Value
                            : string.Empty;
                        if (OMAdapter.GetFieldById(sPContentType.Fields, fieldId) != null)
                        {
                            SPField fieldByNames = caseInvariantFieldCollection.GetFieldByNames(null, value3);
                            if (fieldByNames == null)
                            {
                                this.AddFieldsXML(caseInvariantFieldCollection, xmlNode6, false);
                                fieldByNames = caseInvariantFieldCollection.GetFieldByNames(null, value3);
                            }

                            if (fieldByNames != null)
                            {
                                SPFieldLink fieldLink = new SPFieldLink(fieldByNames);
                                this.SetInfoPathXMLDocumentProperties(xmlNode6, fieldLink);
                                sPContentType.FieldLinks.Add(fieldLink);
                                this.EnsureTaxonomyRelatedFieldsPresent(fieldByNames, sPContentType, ref list2);
                            }
                        }

                        if (!list2.Contains(value3))
                        {
                            list2.Add(value3);
                        }
                    }

                    List<SPFieldLink> list3 = new List<SPFieldLink>();
                    foreach (SPFieldLink sPFieldLink2 in ((IEnumerable)sPContentType.FieldLinks))
                    {
                        if (!list2.Contains(sPFieldLink2.Name) &&
                            Utils.GetFieldNodeById(xmlNodeList, sPFieldLink2.Id) == null)
                        {
                            list3.Add(sPFieldLink2);
                        }
                    }

                    foreach (SPFieldLink current in list3)
                    {
                        sPContentType.FieldLinks.Delete(current.Name);
                    }

                    try
                    {
                        sPContentType.FieldLinks.Reorder(list2.ToArray());
                        sPContentType.Update();
                    }
                    catch (Exception ex2)
                    {
                        OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                        if (ex2 is SPContentTypeReadOnlyException || ex2 is SPContentTypeSealedException)
                        {
                            throw new ArgumentException("ML-SPCT-ROS: " + ex2.Message);
                        }

                        throw;
                    }
                }

                if (sPContentType == null && sPContentType2 != null)
                {
                    sPContentType = sPContentType2;
                }

                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlWriter.WriteStartElement("AddOrUpdateContentType");
                xmlWriter.WriteStartElement("Results");
                xmlWriter.WriteAttributeString("Failures", list.Count.ToString());
                foreach (string current2 in list)
                {
                    xmlWriter.WriteStartElement("Failure");
                    xmlWriter.WriteAttributeString("ContentTypeName", current2);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                this.GetContentTypeXML(sPContentType, xmlWriter);
                xmlWriter.WriteEndElement();
                result = stringBuilder.ToString();
            }

            return result;
        }

        private void GetExistingContentTypesFromWeb(string name, SPContentTypeId id, SPWeb targetWeb,
            out SPContentType existing, out SPContentType available)
        {
            existing = null;
            available = null;
            if (id != SPContentTypeId.Empty)
            {
                existing = targetWeb.ContentTypes[id];
                available = targetWeb.AvailableContentTypes[id];
            }

            if (existing != null || available != null)
            {
                return;
            }

            existing = targetWeb.ContentTypes[name];
            available = targetWeb.AvailableContentTypes[name];
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            string text;
            string urlOfFile;
            Utils.ParseUrlForLeafName(url, out text, out urlOfFile);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(cTypeXml);
            string value = xmlNode.Attributes["Name"].Value;
            string text2 = (xmlNode.Attributes["ListID"] != null) ? xmlNode.Attributes["ListID"].Value : null;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPContentType sPContentType;
                bool readOnly;
                if (text2 == null)
                {
                    sPContentType = web.ContentTypes[value];
                    SPFile sPFile = sPContentType.ResourceFolder.Files.Add(url, docTemplate, true);
                    readOnly = sPContentType.ReadOnly;
                    if (sPContentType.ReadOnly)
                    {
                        sPContentType.ReadOnly = false;
                    }

                    sPContentType.DocumentTemplate = sPFile.Name;
                    sPContentType.Update();
                }
                else
                {
                    sPContentType = web.Lists[new Guid(text2)].ContentTypes[value];
                    readOnly = sPContentType.ReadOnly;
                    if (sPContentType.ReadOnly)
                    {
                        sPContentType.ReadOnly = false;
                    }

                    if (docTemplate != null)
                    {
                        SPFile sPFile2 = sPContentType.ResourceFolder.Files.Add(urlOfFile, docTemplate, true);
                        sPContentType.DocumentTemplate = sPFile2.Name;
                        sPContentType.Update();
                    }
                    else if (url.Contains("://"))
                    {
                        sPContentType.DocumentTemplate = url;
                        sPContentType.Update();
                    }
                }

                if (sPContentType != null && sPContentType.ReadOnly != readOnly)
                {
                    sPContentType.ReadOnly = readOnly;
                    sPContentType.Update();
                }
            }

            return string.Empty;
        }

        private string GetContentType(SPContentType contentType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            this.GetContentTypeXML(contentType, xmlWriter);
            return stringBuilder.ToString();
        }

        private void GetContentTypeXML(SPContentType contentType, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ContentType");
            xmlWriter.WriteAttributeString("Name", contentType.Name);
            xmlWriter.WriteAttributeString("ID", contentType.Id.ToString());
            xmlWriter.WriteAttributeString("Description", contentType.Description);
            xmlWriter.WriteAttributeString("Group", contentType.Group);
            xmlWriter.WriteAttributeString("Hidden", contentType.Hidden.ToString());
            xmlWriter.WriteAttributeString("RequireClientRenderingOnNew",
                contentType.RequireClientRenderingOnNew.ToString());
            xmlWriter.WriteAttributeString("ReadOnly", contentType.ReadOnly.ToString());
            try
            {
                xmlWriter.WriteAttributeString("HasWorkflows",
                    (contentType.WorkflowAssociations.Count > 0) ? "true" : "false");
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                if (!this.HideUnauthorizedAccessError(ex))
                {
                    throw;
                }

                xmlWriter.WriteAttributeString("HasWorkflows", "false");
            }

            xmlWriter.WriteStartElement("FieldRefs");
            foreach (SPFieldLink sPFieldLink in ((IEnumerable)contentType.FieldLinks))
            {
                xmlWriter.WriteStartElement("FieldRef");
                Guid arg_13D_0 = sPFieldLink.Id;
                xmlWriter.WriteAttributeString("ID", sPFieldLink.Id.ToString());
                xmlWriter.WriteAttributeString("Name", sPFieldLink.Name);
                xmlWriter.WriteAttributeString("Required", sPFieldLink.Required.ToString());
                xmlWriter.WriteAttributeString("Hidden", sPFieldLink.Hidden.ToString());
                xmlWriter.WriteAttributeString("ReadOnly", sPFieldLink.ReadOnly.ToString());
                if (!string.IsNullOrEmpty(sPFieldLink.XPath))
                {
                    xmlWriter.WriteAttributeString("Node", sPFieldLink.XPath);
                }

                if (!string.IsNullOrEmpty(sPFieldLink.PITarget))
                {
                    xmlWriter.WriteAttributeString("PITarget", sPFieldLink.PITarget);
                }

                if (!string.IsNullOrEmpty(sPFieldLink.PrimaryPITarget))
                {
                    xmlWriter.WriteAttributeString("PrimaryPITarget", sPFieldLink.PrimaryPITarget);
                }

                if (!string.IsNullOrEmpty(sPFieldLink.PIAttribute))
                {
                    xmlWriter.WriteAttributeString("PIAttribute", sPFieldLink.PIAttribute);
                }

                if (!string.IsNullOrEmpty(sPFieldLink.PrimaryPIAttribute))
                {
                    xmlWriter.WriteAttributeString("PrimaryPIAttribute", sPFieldLink.PrimaryPIAttribute);
                }

                if (!string.IsNullOrEmpty(sPFieldLink.AggregationFunction))
                {
                    xmlWriter.WriteAttributeString("Aggregation", sPFieldLink.AggregationFunction);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            try
            {
                xmlWriter.WriteStartElement("DocumentTemplate");
                xmlWriter.WriteAttributeString("TargetName", contentType.DocumentTemplateUrl);
                xmlWriter.WriteEndElement();
                SPFolder resourceFolder = contentType.ResourceFolder;
                if (resourceFolder != null && resourceFolder.Exists)
                {
                    xmlWriter.WriteStartElement("Folder");
                    xmlWriter.WriteAttributeString("TargetName", resourceFolder.ServerRelativeUrl);
                    xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                if (!this.HideUnauthorizedAccessError(ex2))
                {
                    throw;
                }
            }

            xmlWriter.WriteStartElement("XmlDocuments");
            try
            {
                foreach (string data in ((IEnumerable)contentType.XmlDocuments))
                {
                    xmlWriter.WriteStartElement("XmlDocument");
                    xmlWriter.WriteRaw(data);
                    xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex3)
            {
                OMAdapter.LogExceptionDetails(ex3, MethodBase.GetCurrentMethod().Name, null);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private SPContentType GetContentTypeFromParentWeb(SPWeb parentWeb, SPContentTypeId contentTypeId)
        {
            if (parentWeb == null)
            {
                return null;
            }

            SPContentType sPContentType = parentWeb.ContentTypes[contentTypeId];
            if (sPContentType != null)
            {
                return sPContentType;
            }

            return this.GetContentTypeFromParentWeb(parentWeb.ParentWeb, contentTypeId);
        }

        private void DeleteWeb(SPWeb web)
        {
            for (int i = 0; i < web.Webs.Count; i++)
            {
                using (SPWeb sPWeb = web.Webs[i])
                {
                    this.DeleteWeb(sPWeb);
                }
            }

            web.Delete();
        }

        internal SPWebTemplate GetWebTemplateByIDOrName(uint localeID, int iID, int iConfigID, string sSearchName,
            int? iExperienceVersion, SPSite site)
        {
            SPWebTemplateCollection sPWebTemplateCollection = null;
            if (!base.SharePointVersion.IsSharePoint2013OrLater || !iExperienceVersion.HasValue)
            {
                sPWebTemplateCollection = site.GetWebTemplates(localeID);
            }

            foreach (SPWebTemplate sPWebTemplate in ((IEnumerable)sPWebTemplateCollection))
            {
                if (iID == -1)
                {
                    string text = sPWebTemplate.Name;
                    string[] array = sPWebTemplate.Name.Split(new char[]
                    {
                        '#'
                    });
                    int num = -1;
                    if (array.Length == 2 && !int.TryParse(array[1], out num))
                    {
                        text = array[1];
                    }

                    if (text.Equals(sSearchName))
                    {
                        SPWebTemplate result = sPWebTemplate;
                        return result;
                    }
                }
                else
                {
                    string text = sPWebTemplate.Name;
                    int num2 = text.IndexOf("#", StringComparison.Ordinal);
                    int num3 = 0;
                    if (num2 >= 0)
                    {
                        string s = text.Substring(num2 + 1);
                        if (!int.TryParse(s, out num3))
                        {
                            num3 = 0;
                        }
                    }

                    if (sPWebTemplate.ID == iID && num3 == iConfigID)
                    {
                        SPWebTemplate result = sPWebTemplate;
                        return result;
                    }
                }
            }

            if (iID == -1)
            {
                sPWebTemplateCollection = site.GetCustomWebTemplates(localeID);
                foreach (SPWebTemplate sPWebTemplate2 in ((IEnumerable)sPWebTemplateCollection))
                {
                    string[] array2 = sPWebTemplate2.Name.Split(new char[]
                    {
                        '#'
                    });
                    string text = sPWebTemplate2.Name;
                    if (array2.Length == 2)
                    {
                        text = array2[1];
                    }

                    if (text.Equals(sSearchName))
                    {
                        SPWebTemplate result = sPWebTemplate2;
                        return result;
                    }
                }
            }

            if (sPWebTemplateCollection.Count <= 0)
            {
                throw new LanguageTemplatesMissingException();
            }

            return null;
        }

        [SPDisposeCheckIgnore(SPDisposeCheckID.SPDisposeCheckID_130,
            "Ignore case where a new SPWEb() instance is disposed by the caller.")]
        private static SPWeb GetWebByName(SPWeb parentWeb, string sName)
        {
            SPWeb result;
            try
            {
                string[] names = parentWeb.Webs.Names;
                string[] array = names;
                for (int i = 0; i < array.Length; i++)
                {
                    string a = array[i];
                    if (a == sName)
                    {
                        SPWeb sPWeb = null;
                        try
                        {
                            sPWeb = parentWeb.Webs[sName];
                        }
                        catch (SPException ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        }

                        result = sPWeb;
                        return result;
                    }
                }

                result = null;
            }
            catch (Exception ex2)
            {
                OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private static bool HasSubWeb(SPWeb parentWeb, string sName)
        {
            bool result;
            try
            {
                string[] names = parentWeb.Webs.Names;
                string[] array = names;
                for (int i = 0; i < array.Length; i++)
                {
                    string a = array[i];
                    if (a == sName)
                    {
                        result = true;
                        return result;
                    }
                }

                result = false;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = false;
            }

            return result;
        }

        public string AddSiteUser(string sUserXML, AddUserOptions options)
        {
            string xml;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sUserXML);
                XmlNode firstChild = xmlDocument.FirstChild;
                if (firstChild.Attributes["LoginName"] == null)
                {
                    throw new Exception("Could not add user. No login name specified.");
                }

                try
                {
                    SPUser sPUser = web.EnsureUser(firstChild.Attributes["LoginName"].Value);
                    bool flag = false;
                    if (firstChild.Attributes["Email"] != null &&
                        !string.IsNullOrEmpty(firstChild.Attributes["Email"].Value))
                    {
                        sPUser.Email = firstChild.Attributes["Email"].Value;
                        flag = true;
                    }

                    if (firstChild.Attributes["Name"] != null &&
                        !string.IsNullOrEmpty(firstChild.Attributes["Name"].Value))
                    {
                        sPUser.Name = firstChild.Attributes["Name"].Value;
                        flag = true;
                    }

                    if (firstChild.Attributes["Notes"] != null &&
                        !string.IsNullOrEmpty(firstChild.Attributes["Notes"].Value))
                    {
                        sPUser.Notes = firstChild.Attributes["Notes"].Value;
                        flag = true;
                    }

                    if (flag)
                    {
                        sPUser.Update();
                    }

                    xml = sPUser.Xml;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    if (!options.AllowDBWrite || !options.AllowDBWriteEnvironment ||
                        !OMAdapter.SupportsPartialDBWriting)
                    {
                        throw;
                    }

                    IDBWriter dBWriterPartial = this.GetDBWriterPartial(context.Web);
                    if (dBWriterPartial == null)
                    {
                        throw;
                    }

                    if (this.ClaimsAuthenticationInUse)
                    {
                        firstChild.Attributes["LoginName"].Value =
                            Utils.ConvertWinOrFormsUserToClaimString(firstChild.Attributes["LoginName"].Value);
                    }

                    dBWriterPartial.AddSiteUser(firstChild, web.Site.ID, web.ID, web.SiteUserInfoList.ID,
                        web.CurrentUser.LoginName);
                    SPUserCollection siteUsers = web.SiteUsers;
                    SPUser sPUser2 = siteUsers[firstChild.Attributes["LoginName"].Value];
                    xml = sPUser2.Xml;
                }
            }

            return xml;
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPRoleAssignmentCollection sPRoleAssignmentCollection = null;
                if (sListID == null)
                {
                    if (!web.HasUniqueRoleAssignments)
                    {
                        web.BreakRoleInheritance(true);
                    }

                    sPRoleAssignmentCollection = web.RoleAssignments;
                }
                else
                {
                    SPList sPList = web.Lists[new Guid(sListID)];
                    if (iItemId < 0)
                    {
                        if (!sPList.HasUniqueRoleAssignments)
                        {
                            sPList.BreakRoleInheritance(true);
                        }

                        sPRoleAssignmentCollection = sPList.RoleAssignments;
                    }
                    else
                    {
                        SPListItem itemById = sPList.GetItemById(iItemId);
                        if (!itemById.HasUniqueRoleAssignments)
                        {
                            itemById.BreakRoleInheritance(true);
                        }

                        sPRoleAssignmentCollection = itemById.RoleAssignments;
                    }
                }

                SPPrincipal sPPrincipal = bIsGroup
                    ? ((SPPrincipal)web.SiteGroups[sPrincipalName])
                    : this.LookupUser(sPrincipalName, web);
                SPRoleAssignment sPRoleAssignment = null;
                try
                {
                    sPRoleAssignment = sPRoleAssignmentCollection.GetAssignmentByPrincipal(sPPrincipal);
                }
                catch (ArgumentException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    sPRoleAssignment = new SPRoleAssignment(sPPrincipal);
                }

                SPRoleDefinition roleDefinition = web.RoleDefinitions[sRoleName];
                if (!sPRoleAssignment.RoleDefinitionBindings.Contains(roleDefinition))
                {
                    sPRoleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                }

                if (sPRoleAssignment.Parent == null)
                {
                    sPRoleAssignmentCollection.Add(sPRoleAssignment);
                }
                else
                {
                    sPRoleAssignment.Update();
                }

                result = string.Format("<RoleAssignment RoleName=\"{0}\" PrincipalName=\"{1}\" />", sRoleName,
                    sPrincipalName);
            }

            return result;
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                if (!web.HasUniqueRoleDefinitions)
                {
                    web.RoleDefinitions.BreakInheritance(true, true);
                }

                SPRoleDefinitionCollection roleDefinitions = web.RoleDefinitions;
                SPRoleDefinition sPRoleDefinition = null;
                try
                {
                    sPRoleDefinition = roleDefinitions[sName];
                }
                catch (SPException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    sPRoleDefinition = new SPRoleDefinition();
                    sPRoleDefinition.Name = sName;
                    roleDefinitions.Add(sPRoleDefinition);
                    sPRoleDefinition = roleDefinitions[sName];
                }

                SPBasePermissions basePermissions =
                    (SPBasePermissions)Enum.Parse(typeof(SPBasePermissions), lPermissionMask.ToString());
                sPRoleDefinition.Description = sDescription;
                sPRoleDefinition.BasePermissions = basePermissions;
                sPRoleDefinition.Update();
                long basePermissions2 = (long)sPRoleDefinition.BasePermissions;
                string xml = sPRoleDefinition.Xml;
                int startIndex = xml.IndexOf("/>", StringComparison.Ordinal);
                result = xml.Insert(startIndex, "PermMask=\"" + basePermissions2.ToString() + "\" ");
            }

            return result;
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sGroupXml);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Group");
                string att = XmlAdapterUtility.getAtt(xmlNode, "Name");
                string att2 = XmlAdapterUtility.getAtt(xmlNode, "Owner");
                string att3 = XmlAdapterUtility.getAtt(xmlNode, "Description");
                bool boolAtt = XmlAdapterUtility.getBoolAtt(xmlNode, "OwnerIsUser");
                SPUser currentUser = web.CurrentUser;
                SPMember sPMember = null;
                if (att2 == att)
                {
                    sPMember = web.CurrentUser;
                }
                else
                {
                    if (boolAtt)
                    {
                        sPMember = this.LookupUser(att2, web);
                    }
                    else
                    {
                        foreach (SPGroup sPGroup in ((IEnumerable)web.SiteGroups))
                        {
                            if (sPGroup.Name == att2)
                            {
                                sPMember = sPGroup;
                                break;
                            }
                        }
                    }

                    if (sPMember == null)
                    {
                        sPMember = web.CurrentUser;
                    }
                }

                SPGroup sPGroup2 = null;
                foreach (SPGroup sPGroup3 in ((IEnumerable)web.SiteGroups))
                {
                    if (sPGroup3.Name == att)
                    {
                        sPGroup2 = sPGroup3;
                        break;
                    }
                }

                if (sPGroup2 == null)
                {
                    web.SiteGroups.Add(att, sPMember, currentUser, att3);
                    sPGroup2 = web.SiteGroups[att];
                }
                else
                {
                    sPGroup2.Description = att3;
                    sPGroup2.Owner = sPMember;
                    while (sPGroup2.Users.Count > 0)
                    {
                        sPGroup2.Users.Remove(0);
                    }
                }

                bool flag = false;
                bool flag2 = web.CurrentUser.LoginName.Contains("|");
                List<string> list = new List<string>();
                foreach (XmlNode xNode in xmlNode.SelectNodes("./Member"))
                {
                    string att4 = XmlAdapterUtility.getAtt(xNode, "Login");
                    if (!this.ClaimsAuthenticationInUse)
                    {
                        if (att4.ToUpper() == web.CurrentUser.LoginName.ToUpper())
                        {
                            flag = true;
                        }
                    }
                    else if (flag2)
                    {
                        if (Utils.ConvertWinOrFormsUserToClaimString(att4.ToUpper()) ==
                            web.CurrentUser.LoginName.ToUpper())
                        {
                            flag = true;
                        }
                    }
                    else if (att4.ToUpper() == web.CurrentUser.LoginName.ToUpper())
                    {
                        flag = true;
                    }

                    SPUser sPUser = this.LookupUser(att4, web);
                    if (sPUser != null)
                    {
                        try
                        {
                            sPGroup2.AddUser(sPUser);
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            list.Add(sPUser.LoginName);
                        }
                    }
                }

                if (att2 == att)
                {
                    sPGroup2.Owner = sPGroup2;
                }

                if (!flag)
                {
                    sPGroup2.Users.Remove(web.CurrentUser.LoginName);
                }

                if (xmlNode.Attributes != null)
                {
                    if (xmlNode.Attributes["OnlyAllowMembersViewMembership"] != null)
                    {
                        sPGroup2.OnlyAllowMembersViewMembership =
                            XmlAdapterUtility.getBoolAtt(xmlNode, "OnlyAllowMembersViewMembership");
                    }

                    if (xmlNode.Attributes["AllowMembersEditMembership"] != null)
                    {
                        sPGroup2.AllowMembersEditMembership =
                            XmlAdapterUtility.getBoolAtt(xmlNode, "AllowMembersEditMembership");
                    }

                    if (xmlNode.Attributes["AllowRequestToJoinLeave"] != null)
                    {
                        sPGroup2.AllowRequestToJoinLeave =
                            XmlAdapterUtility.getBoolAtt(xmlNode, "AllowRequestToJoinLeave");
                    }

                    if (xmlNode.Attributes["AutoAcceptRequestToJoinLeave"] != null)
                    {
                        sPGroup2.AutoAcceptRequestToJoinLeave =
                            XmlAdapterUtility.getBoolAtt(xmlNode, "AutoAcceptRequestToJoinLeave");
                    }

                    if (xmlNode.Attributes["RequestToJoinLeaveEmailSetting"] != null)
                    {
                        sPGroup2.RequestToJoinLeaveEmailSetting =
                            XmlAdapterUtility.getAtt(xmlNode, "RequestToJoinLeaveEmailSetting");
                    }
                }

                sPGroup2.Update();
                this.SetGroupDescription(web, xmlNode, sPGroup2);
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlWriter.WriteStartElement("AddOrUpdateGroup");
                xmlWriter.WriteStartElement("Results");
                xmlWriter.WriteAttributeString("Failures", list.Count.ToString());
                foreach (string current in list)
                {
                    xmlWriter.WriteStartElement("Failure");
                    xmlWriter.WriteAttributeString("LoginName", current);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                this.WriteGroupXML(sPGroup2, xmlWriter);
                xmlWriter.WriteEndElement();
                result = stringBuilder.ToString();
            }

            return result;
        }

        private void SetGroupDescription(SPWeb currentWeb, XmlNode groupNode, SPGroup group)
        {
            try
            {
                SPListItem itemById = currentWeb.SiteUserInfoList.GetItemById(group.ID);
                itemById["Notes"] = ((groupNode.Attributes["Description"] != null)
                    ? groupNode.GetAttributeValueAsString("Description")
                    : string.Empty);
                itemById.Update();
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        public string UpdateGroupQuickLaunch(string groups)
        {
            using (Context context = this.GetContext())
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(groups);
                string value = xmlNode.Attributes["AssociateGroups"].Value;
                SPWeb web = context.Web;
                List<string> list = new List<string>();
                foreach (SPGroup current in web.AssociatedGroups)
                {
                    list.Add(current.Name);
                }

                this.SetAssociateGroups(web, value, list);
                web.Update();
            }

            return string.Empty;
        }

        public string DeleteGroup(string sGroupName)
        {
            using (Context context = this.GetContext())
            {
                context.Web.SiteGroups.Remove(sGroupName);
            }

            return string.Empty;
        }

        public string DeleteRole(string sRoleName)
        {
            using (Context context = this.GetContext())
            {
                context.Web.RoleDefinitions.Delete(sRoleName);
            }

            return string.Empty;
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            using (Context context = this.GetContext())
            {
                SPWeb currentWeb = context.Web;
                SPRoleAssignmentCollection roleAssignments = null;
                if (sListID == null)
                {
                    if (!currentWeb.HasUniqueRoleAssignments)
                    {
                        currentWeb.BreakRoleInheritance(true);
                    }

                    roleAssignments = currentWeb.RoleAssignments;
                }
                else
                {
                    SPList sPList = currentWeb.Lists[new Guid(sListID)];
                    if (iItemId < 0)
                    {
                        if (!sPList.HasUniqueRoleAssignments)
                        {
                            sPList.BreakRoleInheritance(true);
                        }

                        roleAssignments = sPList.RoleAssignments;
                    }
                    else
                    {
                        SPListItem itemById = sPList.GetItemById(iItemId);
                        if (!itemById.HasUniqueRoleAssignments)
                        {
                            itemById.BreakRoleInheritance(true);
                        }

                        roleAssignments = itemById.RoleAssignments;
                    }
                }

                SPPrincipal principal = bIsGroup
                    ? ((SPPrincipal)currentWeb.SiteGroups[sPrincipalName])
                    : ((SPPrincipal)this.LookupUser(sPrincipalName, currentWeb));
                SPRoleAssignment assignmentByPrincipal;
                try
                {
                    assignmentByPrincipal = roleAssignments.GetAssignmentByPrincipal(principal);
                }
                catch (ArgumentException ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    return string.Empty;
                }

                if (sRoleName != null)
                {
                    SPRoleDefinition roleDefinition = currentWeb.RoleDefinitions[sRoleName];
                    if (assignmentByPrincipal.RoleDefinitionBindings.Contains(roleDefinition))
                    {
                        if (assignmentByPrincipal.RoleDefinitionBindings.Count == 1)
                        {
                            assignmentByPrincipal.RoleDefinitionBindings.RemoveAll();
                        }
                        else
                        {
                            assignmentByPrincipal.RoleDefinitionBindings.Remove(roleDefinition);
                        }

                        assignmentByPrincipal.Update();
                    }
                }
                else
                {
                    Action codeBlockToRetry = delegate
                    {
                        roleAssignments.Remove(principal);
                        if (string.IsNullOrEmpty(sListID))
                        {
                            currentWeb.Update();
                            return;
                        }

                        if (iItemId < 0)
                        {
                            SPList sPList2 = roleAssignments.Parent as SPList;
                            if (sPList2 != null)
                            {
                                sPList2.Update();
                                return;
                            }
                        }
                        else
                        {
                            SPListItem sPListItem = roleAssignments.Parent as SPListItem;
                            if (sPListItem != null)
                            {
                                sPListItem.SystemUpdate(false);
                            }
                        }
                    };
                    ExecuteWithRetry.AttemptToExecuteWithRetry(codeBlockToRetry, null);
                }
            }

            return string.Empty;
        }

        private bool IsRecurringWorkflowXml(SPListItem item)
        {
            bool result;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                byte[] buffer = item.File.OpenBinary();
                Stream inStream = new MemoryStream(buffer);
                xmlDocument.Load(inStream);
                XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode(".//Template");
                if (xmlNode != null && xmlNode.Attributes["Category"] != null &&
                    xmlNode.Attributes["Category"].Value != "List" && xmlNode.Attributes["Category"].Value != "Site")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = false;
            }

            return result;
        }

        private SPListItemCollection[] GetFolderItemCollections(SPList list, SPFolder folder,
            out bool bSortOrderViolated)
        {
            string query = Utils.BuildQuery(null, null, false, false,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, new string[]
                {
                    "FileDirRef",
                    "FileLeafRef"
                });
            int value = -2;
            return this.GetListItems(list, query, null, null, null, folder, null, new int?(value),
                out bSortOrderViolated);
        }

        private void BrowserActivateFormTemplate(SPListItem item)
        {
        }

        public string ActivateReusableWorkflowTemplates()
        {
            string text = "";
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                SPList sPList = web.Lists["Workflows"];
                foreach (SPListItem sPListItem in ((IEnumerable)sPList.Folders))
                {
                    try
                    {
                        bool flag;
                        SPListItemCollection[] folderItemCollections =
                            this.GetFolderItemCollections(sPList, sPListItem.Folder, out flag);
                        SPListItemCollection[] array = folderItemCollections;
                        for (int i = 0; i < array.Length; i++)
                        {
                            SPListItemCollection sPListItemCollection = array[i];
                            bool flag2 = false;
                            foreach (SPListItem sPListItem2 in ((IEnumerable)sPListItemCollection))
                            {
                                if (sPListItem2.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (this.IsRecurringWorkflowXml(sPListItem2))
                                    {
                                        WebPartPagesWebService webPartPagesWebService = new WebPartPagesWebService(web);
                                        webPartPagesWebService.AssociateWorkflowMarkup(sPListItem2.Url,
                                            "V" + sPListItem2["Version"].ToString());
                                        foreach (SPListItem sPListItem3 in ((IEnumerable)sPListItemCollection))
                                        {
                                            if (sPListItem3.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
                                            {
                                                this.BrowserActivateFormTemplate(sPListItem3);
                                            }
                                        }
                                    }

                                    flag2 = true;
                                    break;
                                }
                            }

                            if (flag2)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        string text2 = text;
                        text = string.Concat(new string[]
                        {
                            text2,
                            sPListItem.Name,
                            ";",
                            ex.Message,
                            "\n"
                        });
                    }
                }
            }

            return null;
        }

        private void UpdateWorkflowModifiedDate(string workflowId, string createdTime, string modifiedTime)
        {
            if (OMAdapter.SupportsPartialDBWriting)
            {
                using (Context context = this.GetContext())
                {
                    IDBWriter dBWriterPartial = this.GetDBWriterPartial(context.Web);
                    if (dBWriterPartial != null)
                    {
                        dBWriterPartial.UpdateWorkflowAssociationModifiedTime(workflowId,
                            Utils.ParseDateAsUtc(createdTime).ToString(CultureInfo.InvariantCulture),
                            Utils.ParseDateAsUtc(modifiedTime).ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        public string AddWorkflowAssociation(string sTargetId, string sWorkflowXml, bool bAllowDBWrite)
        {
            string result;
            using (Context context = this.GetContext())
            {
                SPWeb web = context.Web;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sWorkflowXml);
                XmlNode documentElement = xmlDocument.DocumentElement;
                if (documentElement.Attributes["Scope"] == null)
                {
                    throw new Exception(
                        "The workflow association scope attribute was null, cannot add a new workflow association");
                }

                string value = documentElement.Attributes["Scope"].Value;
                if (value == "Web")
                {
                    result = this.AddWebWorkflowAssociation(web, sTargetId, documentElement, bAllowDBWrite);
                }
                else if (value == "ContentType")
                {
                    result = this.AddContentTypeWorkflowAssociation(web, sTargetId, documentElement, bAllowDBWrite);
                }
                else
                {
                    result = this.AddListWorkflowAssociation(web, sTargetId, documentElement, bAllowDBWrite);
                }
            }

            return result;
        }

        private string AddWebWorkflowAssociation(SPWeb Web, string sTargetID, XmlNode ndWorkflowAssociation,
            bool bAllowDBWrite)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }

        private string RetrieveAndActivateSharePointDesignerWorkflow(SPWeb Web, XmlNode ndWorkflowAssociation)
        {
            string text = "";
            string text2 = null;
            string result;
            try
            {
                text = ndWorkflowAssociation.Attributes["Name"].Value;
                SPList list = Web.Lists["Workflows"];
                text = Utils.GetWorkflowName(text);
                if (ndWorkflowAssociation.Attributes["UIVersion"] != null)
                {
                    text2 = ndWorkflowAssociation.GetAttributeValueAsString("UIVersion");
                }

                SPFolder sPFolder = Web.GetFolder(Web.Url + "/Workflows/" + text);
                bool flag;
                SPListItemCollection[] folderItemCollections = this.GetFolderItemCollections(list, sPFolder, out flag);
                int num = this.ItemCollectionsItemCount(folderItemCollections);
                if (num == 0)
                {
                    sPFolder = Web.GetFolder(Web.Url + "/Workflows/NintexWorkflow/" + text);
                    folderItemCollections = this.GetFolderItemCollections(list, sPFolder, out flag);
                }

                if (!sPFolder.Exists && num == 0)
                {
                    SPFolder folder = Web.GetFolder(Web.Url + "/Workflows/NintexWorkflow/");
                    if (folder != null && folder.SubFolders.Count > 0)
                    {
                        foreach (SPFolder sPFolder2 in ((IEnumerable)folder.SubFolders))
                        {
                            if (text.StartsWith(sPFolder2.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                sPFolder = sPFolder2;
                                folderItemCollections = this.GetFolderItemCollections(list, sPFolder, out flag);
                                text = sPFolder.Name;
                                break;
                            }
                        }
                    }
                }

                bool flag2 = false;
                bool flag3 = false;
                SPListItemCollection[] array = folderItemCollections;
                for (int i = 0; i < array.Length; i++)
                {
                    SPListItemCollection sPListItemCollection = array[i];
                    foreach (SPListItem sPListItem in ((IEnumerable)sPListItemCollection))
                    {
                        if (sPListItem.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase))
                        {
                            WebPartPagesWebService webPartPagesWebService = new WebPartPagesWebService(Web);
                            if (text2 == null)
                            {
                                webPartPagesWebService.AssociateWorkflowMarkup(sPListItem.Url,
                                    "V" + Convert.ToString(sPListItem["Version"]));
                            }
                            else
                            {
                                webPartPagesWebService.AssociateWorkflowMarkup(sPListItem.Url,
                                    "V" + Convert.ToString(text2));
                            }

                            flag2 = true;
                            break;
                        }

                        if (sPListItem.Name.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
                        {
                            this.BrowserActivateFormTemplate(sPListItem);
                        }
                    }

                    if (flag3)
                    {
                        break;
                    }
                }

                if (!flag2)
                {
                    throw new Exception("The workflow xsn template for workflow: " + text +
                                        " could not be retrieved from the server. Please ensure the hidden Workflows list copied correctly.");
                }

                result = text;
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception("The workflow template for workflow: " + text +
                                    " could not be retrieved from the server. Please ensure all necessary features are activated.");
            }

            return result;
        }

        private int ItemCollectionsItemCount(SPListItemCollection[] itemCollections)
        {
            int num = 0;
            for (int i = 0; i < itemCollections.Length; i++)
            {
                SPListItemCollection sPListItemCollection = itemCollections[i];
                num += sPListItemCollection.Count;
            }

            return num;
        }

        private string AddContentTypeWorkflowAssociation(SPWeb Web, string sTargetID, XmlNode ndWorkflowAssociation,
            bool bAllowDBWrite)
        {
            string result;
            try
            {
                SPWorkflowTemplate sPWorkflowTemplate = null;
                if (ndWorkflowAssociation.Attributes["BaseTemplate"] != null &&
                    ndWorkflowAssociation.Attributes["BaseTemplate"].Value != null)
                {
                    sPWorkflowTemplate =
                        Web.WorkflowTemplates[new Guid(ndWorkflowAssociation.Attributes["BaseTemplate"].Value)];
                }

                if (sPWorkflowTemplate == null)
                {
                    string attributeValueAsString =
                        ndWorkflowAssociation.Attributes.GetAttributeValueAsString("BaseTemplateName");
                    if (!string.IsNullOrEmpty(attributeValueAsString))
                    {
                        sPWorkflowTemplate = this.GetWorkflowTemplateByName(Web, attributeValueAsString);
                    }
                }

                string attributeValueAsString2 = ndWorkflowAssociation.GetAttributeValueAsString("Name");
                if (sPWorkflowTemplate == null)
                {
                    throw new Exception(Resources.UnableToRetrieveWorkflowTemplate);
                }

                SPContentType sPContentType;
                SPWorkflowAssociation sPWorkflowAssociation;
                if (ndWorkflowAssociation.Attributes["ContentTypeParentListId"] != null)
                {
                    SPList sPList =
                        Web.Lists[new Guid(ndWorkflowAssociation.Attributes["ContentTypeParentListId"].Value)];
                    sPContentType = sPList.ContentTypes[new SPContentTypeId(sTargetID)];
                    if (sPContentType == null)
                    {
                        throw new Exception(Resources.MsgCannotFindContentType);
                    }

                    SPList historyList = this.GetHistoryList(ndWorkflowAssociation, Web);
                    SPList tasksList = this.GetTasksList(ndWorkflowAssociation, Web);
                    if (historyList == null || tasksList == null)
                    {
                        sPWorkflowAssociation = SPWorkflowAssociation.CreateSiteContentTypeAssociation(
                            sPWorkflowTemplate, attributeValueAsString2,
                            ndWorkflowAssociation.Attributes["HistoryListTitle"].Value,
                            ndWorkflowAssociation.Attributes["TaskListTitle"].Value);
                        sPWorkflowAssociation.HistoryListTitle =
                            ndWorkflowAssociation.Attributes["HistoryListTitle"].Value;
                        sPWorkflowAssociation.TaskListTitle = ndWorkflowAssociation.Attributes["TaskListTitle"].Value;
                    }
                    else
                    {
                        sPWorkflowAssociation =
                            SPWorkflowAssociation.CreateListContentTypeAssociation(sPWorkflowTemplate,
                                attributeValueAsString2, tasksList, historyList);
                    }
                }
                else
                {
                    sPContentType = Web.ContentTypes[new SPContentTypeId(sTargetID)];
                    if (sPContentType == null)
                    {
                        throw new Exception(Resources.MsgCannotFindContentType);
                    }

                    sPWorkflowAssociation = SPWorkflowAssociation.CreateSiteContentTypeAssociation(sPWorkflowTemplate,
                        attributeValueAsString2, ndWorkflowAssociation.Attributes["HistoryListTitle"].Value,
                        ndWorkflowAssociation.Attributes["TaskListTitle"].Value);
                    sPWorkflowAssociation.HistoryListTitle = ndWorkflowAssociation.Attributes["HistoryListTitle"].Value;
                    sPWorkflowAssociation.TaskListTitle = ndWorkflowAssociation.Attributes["TaskListTitle"].Value;
                }

                this.SetWorkflowData(sPWorkflowAssociation, ndWorkflowAssociation);
                SPWorkflowAssociation sPWorkflowAssociation2 =
                    sPContentType.WorkflowAssociations.GetAssociationByName(sPWorkflowAssociation.Name,
                        CultureInfo.CurrentCulture);
                if (sPWorkflowAssociation2 == null)
                {
                    sPWorkflowAssociation2 = sPContentType.AddWorkflowAssociation(sPWorkflowAssociation);
                }
                else
                {
                    sPContentType.UpdateWorkflowAssociation(sPWorkflowAssociation);
                }

                string attributeValueAsString3 =
                    ndWorkflowAssociation.GetAttributeValueAsString("StatusColumnInternalName");
                if (!string.IsNullOrEmpty(attributeValueAsString3))
                {
                    this.SetStatusFieldInternalName(sPWorkflowAssociation, ndWorkflowAssociation,
                        attributeValueAsString3, null, sPContentType);
                }

                if (bAllowDBWrite && sPWorkflowAssociation2 != null)
                {
                    this.UpdateWorkflowModifiedDate(sPWorkflowAssociation2.Id.ToString(),
                        ndWorkflowAssociation.Attributes["Created"].Value,
                        ndWorkflowAssociation.Attributes["Modified"].Value);
                }

                StringBuilder stringBuilder = new StringBuilder();
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.WriteWorkflowXml(sPWorkflowAssociation2, xmlWriter, null, false, false);
                result = stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                throw new Exception(string.Format(Resources.FS_UnableToMigrateContentTypeWorkflow, ex.Message));
            }

            return result;
        }

        private SPWorkflowTemplate GetWorkflowTemplateByName(SPWeb web, string worklowTemplateName)
        {
            SPWorkflowTemplate result;
            try
            {
                if (!string.IsNullOrEmpty(worklowTemplateName))
                {
                    SPWorkflowTemplate templateByName =
                        web.WorkflowTemplates.GetTemplateByName(worklowTemplateName, OMAdapter.s_CultureInfo);
                    if (templateByName == null)
                    {
                        OMAdapter.LogMessageDetails(
                            string.Format("Workflow Template '{0}' is not available on target", worklowTemplateName),
                            MethodBase.GetCurrentMethod().Name, null);
                    }

                    result = templateByName;
                }
                else
                {
                    OMAdapter.LogMessageDetails("Source Workflow Template Name is empty",
                        MethodBase.GetCurrentMethod().Name, null);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private string AddListWorkflowAssociation(SPWeb Web, string sTargetID, XmlNode ndWorkflowAssociation,
            bool bAllowDBWrite)
        {
            SPWorkflowAssociation sPWorkflowAssociation = null;
            SPList sPList = Web.Lists[new Guid(sTargetID)];
            SPWorkflowTemplate sPWorkflowTemplate = null;
            if (ndWorkflowAssociation.Attributes["BaseTemplate"] != null &&
                ndWorkflowAssociation.Attributes["BaseTemplate"].Value != null)
            {
                sPWorkflowTemplate =
                    Web.WorkflowTemplates[new Guid(ndWorkflowAssociation.Attributes["BaseTemplate"].Value)];
            }

            if (sPWorkflowTemplate != null)
            {
                SPList historyList = this.GetHistoryList(ndWorkflowAssociation, Web);
                SPList tasksList = this.GetTasksList(ndWorkflowAssociation, Web);
                SPWorkflowAssociation sPWorkflowAssociation2 =
                    SPWorkflowAssociation.CreateListAssociation(sPWorkflowTemplate,
                        ndWorkflowAssociation.Attributes["Name"].Value, tasksList, historyList);
                this.SetWorkflowData(sPWorkflowAssociation2, ndWorkflowAssociation);
                sPWorkflowAssociation =
                    sPList.WorkflowAssociations.GetAssociationByName(sPWorkflowAssociation2.Name,
                        CultureInfo.CurrentCulture);
                if (sPWorkflowAssociation == null)
                {
                    sPWorkflowAssociation = sPList.AddWorkflowAssociation(sPWorkflowAssociation2);
                    if (!string.IsNullOrEmpty(
                            ndWorkflowAssociation.GetAttributeValueAsString("DefaultContentApprovalWorkflowId")))
                    {
                        sPList.DefaultContentApprovalWorkflowId = sPWorkflowAssociation.Id;
                        sPList.Update();
                    }
                }
                else
                {
                    sPList.UpdateWorkflowAssociation(sPWorkflowAssociation2);
                }
            }
            else
            {
                try
                {
                    string b = this.RetrieveAndActivateSharePointDesignerWorkflow(Web, ndWorkflowAssociation);
                    sPList = Web.Lists[new Guid(sTargetID)];
                    foreach (SPWorkflowAssociation sPWorkflowAssociation3 in ((IEnumerable)sPList.WorkflowAssociations))
                    {
                        if (sPWorkflowAssociation3.Name == b)
                        {
                            sPWorkflowAssociation = sPWorkflowAssociation3;
                            break;
                        }
                    }

                    if (sPWorkflowAssociation == null)
                    {
                        throw new Exception(Resources.UnableToFindWorkflowAssociationInList);
                    }

                    if (ndWorkflowAssociation.Attributes["HistoryListId"] != null &&
                        !string.IsNullOrEmpty(ndWorkflowAssociation.Attributes["HistoryListId"].Value) &&
                        sPWorkflowAssociation.HistoryListId !=
                        new Guid(ndWorkflowAssociation.Attributes["HistoryListId"].Value))
                    {
                        SPList historyList2 = this.GetHistoryList(ndWorkflowAssociation, Web);
                        sPWorkflowAssociation.SetHistoryList(historyList2);
                        sPList.UpdateWorkflowAssociation(sPWorkflowAssociation);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    SoapException ex2 = ex as SoapException;
                    if (ex2 != null)
                    {
                        throw new Exception(ex2.Detail.OuterXml);
                    }

                    throw;
                }
            }

            if (bAllowDBWrite && sPWorkflowAssociation != null)
            {
                this.UpdateWorkflowModifiedDate(sPWorkflowAssociation.Id.ToString(),
                    ndWorkflowAssociation.Attributes["Created"].Value,
                    ndWorkflowAssociation.Attributes["Modified"].Value);
            }

            StringBuilder stringBuilder = new StringBuilder();
            if (sPWorkflowAssociation != null)
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                this.WriteWorkflowXml(sPWorkflowAssociation, xmlWriter, sPList.ID.ToString(), true, false);
                return stringBuilder.ToString();
            }

            throw new Exception(Resources.UnableToFindWorkflowAssociationInList);
        }

        private string GetWorkflowAssociationInternalStatusField(SPWorkflowAssociation wfa)
        {
            string result;
            try
            {
                Type type = wfa.GetType();
                PropertyInfo property = type.GetProperty("InternalNameStatusField",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                result = (string)property.GetValue(wfa, null);
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                result = null;
            }

            return result;
        }

        private SPList GetHistoryList(XmlNode ndWorkflowAssociation, SPWeb Web)
        {
            if (ndWorkflowAssociation.Attributes["HistoryListId"] != null &&
                !string.IsNullOrEmpty(ndWorkflowAssociation.Attributes["HistoryListId"].Value))
            {
                if (ndWorkflowAssociation.Attributes["HistoryListId"].Value == Guid.Empty.ToString())
                {
                    return null;
                }

                try
                {
                    SPList result = Web.Lists[new Guid(ndWorkflowAssociation.Attributes["HistoryListId"].Value)];
                    return result;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    try
                    {
                        SPList result = Web.Lists[ndWorkflowAssociation.Attributes["HistoryListTitle"].Value];
                        return result;
                    }
                    catch (Exception)
                    {
                        throw new Exception(
                            "Could not find a suitable history list on the target to apply the workflow association to.");
                    }
                }
            }

            return null;
        }

        private SPList GetTasksList(XmlNode ndWorkflowAssociation, SPWeb Web)
        {
            if (ndWorkflowAssociation.Attributes["TaskListId"] != null &&
                !string.IsNullOrEmpty(ndWorkflowAssociation.Attributes["TaskListId"].Value))
            {
                if (ndWorkflowAssociation.Attributes["TaskListId"].Value == Guid.Empty.ToString())
                {
                    return null;
                }

                try
                {
                    SPList result = Web.Lists[new Guid(ndWorkflowAssociation.Attributes["TaskListId"].Value)];
                    return result;
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    try
                    {
                        SPList result = Web.Lists[ndWorkflowAssociation.Attributes["TaskListTitle"].Value];
                        return result;
                    }
                    catch (Exception)
                    {
                        OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                        throw new Exception(
                            "Could not find a suitable tasks list on the target to apply the workflow association to.");
                    }
                }
            }

            return null;
        }

        private void SetWorkflowData(SPWorkflowAssociation wfa, XmlNode ndWorkflowAssociation)
        {
            if (ndWorkflowAssociation.Attributes["Configuration"] != null)
            {
                SPWorkflowAssociationCollection.Configuration configuration =
                    (SPWorkflowAssociationCollection.Configuration)Enum.Parse(
                        typeof(SPWorkflowAssociationCollection.Configuration),
                        ndWorkflowAssociation.Attributes["Configuration"].Value);
                wfa.AllowAsyncManualStart =
                    ((configuration & SPWorkflowAssociationCollection.Configuration.AllowAsyncManualStart) !=
                     SPWorkflowAssociationCollection.Configuration.None);
                wfa.AllowManual = ((configuration & SPWorkflowAssociationCollection.Configuration.AllowManualStart) !=
                                   SPWorkflowAssociationCollection.Configuration.None);
                wfa.AutoStartChange =
                    ((configuration & SPWorkflowAssociationCollection.Configuration.AutoStartChange) !=
                     SPWorkflowAssociationCollection.Configuration.None);
                wfa.AutoStartCreate = ((configuration & SPWorkflowAssociationCollection.Configuration.AutoStartAdd) !=
                                       SPWorkflowAssociationCollection.Configuration.None);
                wfa.LockItem = ((configuration & SPWorkflowAssociationCollection.Configuration.LockItem) !=
                                SPWorkflowAssociationCollection.Configuration.None);
            }
            else
            {
                wfa.AllowAsyncManualStart =
                    Convert.ToBoolean(ndWorkflowAssociation.Attributes["AllowAsyncManualStart"].Value);
                wfa.AllowManual = Convert.ToBoolean(ndWorkflowAssociation.Attributes["AllowManual"].Value);
                wfa.AutoStartChange = Convert.ToBoolean(ndWorkflowAssociation.Attributes["AutoStartChange"].Value);
                wfa.AutoStartCreate = Convert.ToBoolean(ndWorkflowAssociation.Attributes["AutoStartCreate"].Value);
                wfa.LockItem = Convert.ToBoolean(ndWorkflowAssociation.Attributes["LockItem"].Value);
            }

            wfa.AssociationData = ndWorkflowAssociation.Attributes["AssociationData"].Value;
            wfa.Description = ndWorkflowAssociation.Attributes["Description"].Value;
            wfa.PermissionsManual = (SPBasePermissions)Enum.Parse(typeof(SPBasePermissions),
                ndWorkflowAssociation.Attributes["PermissionsManual"].Value);
            wfa.AutoCleanupDays = Convert.ToInt32(ndWorkflowAssociation.Attributes["AutoCleanupDays"].Value);
        }

        public string AddWorkflow(string sObjectId, string sWorkflowXml)
        {
            if (OMAdapter.SupportsPartialDBWriting)
            {
                using (Context context = this.GetContext())
                {
                    IDBWriter dBWriterPartial = this.GetDBWriterPartial(context.Web);
                    if (dBWriterPartial != null)
                    {
                        string text = null;
                        try
                        {
                            this.MapOMWorkflowDataAndAddListColumn(context, ref sWorkflowXml, ref text);
                        }
                        catch (Exception ex)
                        {
                            OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                            if (ex.Message.Contains("Workflow instance column creation failed"))
                            {
                                throw;
                            }
                        }

                        try
                        {
                            string text2 = dBWriterPartial.AddWorkflow(sObjectId, sWorkflowXml);
                            this.AnalyzeAndTerminateNewWorkflow(context, sWorkflowXml, text2);
                            return text2;
                        }
                        catch (Exception ex2)
                        {
                            OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                            if (text != null)
                            {
                                throw new Exception("WFA Column creation succeeded with ColName:<" + text +
                                                    ">, but further instance addition threw an exception with message: " +
                                                    ex2.Message);
                            }

                            throw;
                        }
                    }
                }
            }

            return null;
        }

        private string AnalyzeAndTerminateNewWorkflow(Context ctx, string sOldWorkflowXML, string sNewWorkflowXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sOldWorkflowXML);
            XmlNode documentElement = xmlDocument.DocumentElement;
            if (documentElement.Attributes["TerminateWorkflow"] != null)
            {
                try
                {
                    xmlDocument.LoadXml(sNewWorkflowXML);
                    XmlNode documentElement2 = xmlDocument.DocumentElement;
                    using (ctx.Site)
                    {
                        using (SPWeb web = ctx.Web)
                        {
                            SPWorkflow sPWorkflow = null;
                            if (!(documentElement2.Attributes["ItemId"].Value == "-1"))
                            {
                                SPListItem itemById = web.Lists[new Guid(documentElement2.Attributes["ListId"].Value)]
                                    .GetItemById(Convert.ToInt32(documentElement2.Attributes["ItemId"].Value));
                                sPWorkflow = itemById.Workflows[new Guid(documentElement2.Attributes["Id"].Value)];
                            }

                            if (sPWorkflow != null)
                            {
                                foreach (SPWorkflowTask t in ((IEnumerable)sPWorkflow.Tasks))
                                {
                                    this.UpdateWorkflowTaskStatus(t);
                                }

                                SPWorkflowManager.CancelWorkflow(sPWorkflow);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                    return ex.Message;
                }
            }

            return null;
        }

        private void UpdateWorkflowTaskStatus(SPWorkflowTask t)
        {
            t["Status"] = "Canceled";
            t.Update();
        }

        private void MapOMWorkflowDataAndAddListColumn(Context ctx, ref string sWorkflowXml, ref string sNewColumnName)
        {
            using (SPWeb web = ctx.Web)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sWorkflowXml);
                XmlNode documentElement = xmlDocument.DocumentElement;
                try
                {
                    string value = documentElement.Attributes["TaskListId"].Value;
                    SPList sPList = web.Lists[new Guid(value)];
                    string schemaXml = sPList.Fields["WorkflowInstanceIDBackup"].SchemaXml;
                    XmlDocument xmlDocument2 = new XmlDocument();
                    xmlDocument2.LoadXml(schemaXml);
                    string value2 = xmlDocument2.DocumentElement.Attributes["ColName"].Value;
                    XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("TaskListWorkflowInstanceIDBackupColumn");
                    xmlAttribute.Value = value2;
                    documentElement.Attributes.Append(xmlAttribute);
                    string attributeValueAsString = documentElement.GetAttributeValueAsString("Author");
                    if (this.ClaimsAuthenticationInUse && !string.IsNullOrEmpty(attributeValueAsString))
                    {
                        documentElement.Attributes["Author"].Value =
                            Utils.ConvertWinOrFormsUserToClaimString(attributeValueAsString);
                    }
                }
                catch (Exception ex)
                {
                    OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
                }

                try
                {
                    if (documentElement.Attributes["StatusColumnName"] != null ||
                        documentElement.Attributes["CreateColumn"] != null)
                    {
                        SPList sPList2 = web.Lists[new Guid(documentElement.Attributes["ListId"].Value)];
                        SPContentType sPContentType = null;
                        SPWorkflowAssociation wfa;
                        if (documentElement.Attributes["TargetContentTypeId"] != null)
                        {
                            sPContentType =
                                sPList2.ContentTypes[
                                    new SPContentTypeId(documentElement.Attributes["TargetContentTypeId"].Value)];
                            wfa = sPContentType.WorkflowAssociations[
                                new Guid(documentElement.Attributes["TemplateId"].Value)];
                        }
                        else
                        {
                            wfa = sPList2.WorkflowAssociations[
                                new Guid(documentElement.Attributes["TemplateId"].Value)];
                        }

                        SPField sPField = null;
                        if (documentElement.Attributes["CreateColumn"] != null)
                        {
                            sPField = this.CreateWorkflowColumn(web, sPList2, sPContentType, documentElement, wfa);
                            sNewColumnName = sPField.InternalName;
                        }

                        if (sPField != null || (documentElement.Attributes["AddStatusFieldToViews"] != null &&
                                                documentElement.Attributes["AddStatusFieldToViews"].Value == "true"))
                        {
                            this.AddColumnToViews(sPList2,
                                (sPField != null)
                                    ? sPField.InternalName
                                    : documentElement.Attributes["StatusColumnTextName"].Value,
                                (documentElement.Attributes["StatusColumnViews"] != null)
                                    ? documentElement.Attributes["StatusColumnViews"].Value
                                    : null);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    OMAdapter.LogExceptionDetails(ex2, MethodBase.GetCurrentMethod().Name, null);
                    throw new Exception("Workflow instance column creation failed with message: " + ex2.Message, ex2);
                }

                sWorkflowXml = documentElement.OuterXml;
            }
        }

        private SPField CreateWorkflowColumn(SPWeb currentWeb, SPList targetList, SPContentType ct, XmlNode ndWf,
            SPWorkflowAssociation wfa)
        {
            string text = (wfa.Name.Length >= 8) ? wfa.Name.Substring(0, 8) : wfa.Name;
            text = XmlUtility.RemoveIllegalCharactersFromXmlAttributeValue(text);
            int num = 0;
            string text2 = text;
            while (targetList.Fields.ContainsField(text2))
            {
                text2 = text + num.ToString();
                num++;
            }

            string strXml = string.Concat(new string[]
            {
                "<Field DisplayName=\"",
                text2,
                "\" Type=\"WorkflowStatus\" Required=\"FALSE\" SourceID=\"{",
                targetList.ID.ToString(),
                "}\"  Name=\"",
                text2,
                "\" WorkflowStatusURL=\"",
                wfa.StatusUrl,
                "\" ReadOnly=\"TRUE\"><CHOICES><CHOICE>Starting</CHOICE><CHOICE>Failed on Start</CHOICE><CHOICE>In Progress</CHOICE><CHOICE>Error Occurred</CHOICE><CHOICE>Canceled</CHOICE><CHOICE>Completed</CHOICE><CHOICE>Failed on Start (retrying)</CHOICE><CHOICE>Error Occurred (retrying)</CHOICE><CHOICE/><CHOICE/><CHOICE/><CHOICE/><CHOICE/><CHOICE/><CHOICE/><CHOICE>Canceled</CHOICE><CHOICE>Approved</CHOICE><CHOICE>Rejected</CHOICE></CHOICES></Field>"
            });
            string text3 = targetList.Fields.AddFieldAsXml(strXml);
            SPField fieldByInternalName = targetList.Fields.GetFieldByInternalName(text3);
            fieldByInternalName.Title = Utils.GetWorkflowName(wfa.Name);
            fieldByInternalName.Update(true);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fieldByInternalName.SchemaXml);
            XmlNode documentElement = xmlDocument.DocumentElement;
            ndWf.Attributes["StatusColumnName"].Value = documentElement.Attributes["ColName"].Value;
            Utils.AddOrUpdateXmlAttribute(ndWf, "StatusColumnInternalName", text3);
            this.SetStatusFieldInternalName(wfa, ndWf, fieldByInternalName.InternalName, targetList, ct);
            this.UpdateWorkflowModifiedDate(wfa.Id.ToString(), ndWf.Attributes["WFACreated"].Value,
                ndWf.Attributes["WFAModified"].Value);
            return fieldByInternalName;
        }

        private void SetStatusFieldInternalName(SPWorkflowAssociation workflowAssociation,
            XmlNode workflowAssociationXmlNode, string statusFieldName, SPList targetList, SPContentType contentType)
        {
            Type type = workflowAssociation.GetType();
            PropertyInfo property = type.GetProperty("InternalNameStatusField",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
            property.SetValue(workflowAssociation, statusFieldName, null);
            property = type.GetProperty("StatusColumnShown",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
            property.SetValue(workflowAssociation, true, null);
            if (targetList == null || (workflowAssociationXmlNode.Attributes["TargetContentTypeId"] != null &&
                                       contentType != null))
            {
                contentType.UpdateWorkflowAssociation(workflowAssociation);
                return;
            }

            targetList.UpdateWorkflowAssociation(workflowAssociation);
        }

        private void AddColumnToViews(SPList targetList, string fieldName, string sViewsString)
        {
            try
            {
                if (sViewsString != null)
                {
                    string[] array = sViewsString.Split(new char[]
                    {
                        ';'
                    });
                    string[] array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        string strTitle = array2[i];
                        if (targetList.Views[strTitle] != null)
                        {
                            SPView view = targetList.Views[strTitle];
                            this.UpdateViewWithField(view, fieldName);
                        }
                    }
                }
                else
                {
                    SPView defaultView = targetList.DefaultView;
                    this.UpdateViewWithField(defaultView, fieldName);
                }
            }
            catch (Exception ex)
            {
                OMAdapter.LogExceptionDetails(ex, MethodBase.GetCurrentMethod().Name, null);
            }
        }

        private void UpdateViewWithField(SPView view, string fieldName)
        {
            bool flag = false;
            foreach (string a in ((IEnumerable)view.ViewFields))
            {
                if (a == fieldName)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                view.ViewFields.Add(fieldName);
                view.Update();
            }
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            using (Context context = this.GetContext())
            {
                if (context.Web.ParserEnabled != bParserEnabled)
                {
                    context.Web.ParserEnabled = bParserEnabled;
                    context.Web.Update();
                }
            }

            return string.Empty;
        }

        private void UpdateColumnSettings(XmlNode fieldRefNode, SPFieldLink existingfieldLink)
        {
            if (fieldRefNode.Attributes["Required"] != null)
            {
                bool attributeValueAsBoolean = fieldRefNode.GetAttributeValueAsBoolean("Required");
                if (existingfieldLink.Required != attributeValueAsBoolean)
                {
                    existingfieldLink.Required = attributeValueAsBoolean;
                }
            }

            if (fieldRefNode.Attributes["Hidden"] != null)
            {
                bool attributeValueAsBoolean2 = fieldRefNode.GetAttributeValueAsBoolean("Hidden");
                if (existingfieldLink.Hidden != attributeValueAsBoolean2)
                {
                    existingfieldLink.Hidden = attributeValueAsBoolean2;
                }
            }
        }
    }
}