using Metalogix.Core.OperationLog;
using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.NWS.Areas;
using Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices;
using Metalogix.SharePoint.Adapters.NWS.People;
using Metalogix.SharePoint.Adapters.NWS.Properties;
using Metalogix.SharePoint.Adapters.NWS.SiteData;
using Metalogix.SharePoint.Adapters.NWS.Sites;
using Metalogix.SharePoint.Adapters.NWS.WebPartPages;
using Metalogix.SharePoint.Adapters.NWS.WrappedServices;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Common;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;

namespace Metalogix.SharePoint.Adapters.NWS
{
    [AdapterDisplayName("Remote Connection (Native SharePoint Web Service)")]
    [AdapterShortName("NW")]
    [AdapterSupports(AdapterSupportedFlags.SiteScope | AdapterSupportedFlags.LegacyLicense |
                     AdapterSupportedFlags.CurrentLicense)]
    [MenuOrder(4)]
    [ShowInMenu(true)]
    public class NWSAdapter : SharePointAdapter, ISharePointReader, ISharePointWriter, ISharePointAdapterCommand
    {
        private const string RPC_DATETIME_FORMAT = "dd MMM yyyy HH:mm:ss zzz";

        private const string WEB_STRUCTURE_UPDATE_STRING =
            "[eidMod={0};eidParent={1};eidRef=-2;DTLP={2};eType={3};mType={4};name={5};url={6};meta-info=[{7}]]";

        private const int MAX_PAGE_SIZE = 5000;

        private const string METAINFO_OWNERKEY = "associateownergroup";

        private const string METAINFO_MEMBERKEY = "associatemembergroup";

        private const string METAINFO_VISITORKEY = "associatevisitorgroup";

        private const int SHAREPOINT_VERSION_ID_INCREMENT_VALUE = 512;

        private const string UNSPECIFIED_VERSION_NUMBER = "0";

        private const string QUERY_THROTTLE_EXCEPTION_CODE = "0x80070024";

        private const string VERSION_IDENTIFIER = "vti_extenderversion";

        private const string EditPage = "editprms";

        private const string BasePage = "user";

        private const string AddPage = "aclinv";

        private const string RolePage = "role";

        private const string LIST_DOES_NOT_EXIST_2007 = "0x80070002";

        private const string LIST_DOES_NOT_EXIST_2010 = "0x82000006";

        private const string LIST_CANNOT_BE_DELETED = "0x8102003d";

        private const string MINOR_CHECKIN = "0";

        private const string MAJOR_CHECKIN = "1";

        private const string OVERWRITE_CHECKIN = "2";

        private const string HIDDEN_WEBPART_ZONE = "wpz";

        private const string DETACHED_LAYOUT = "http://www.microsoft.com/publishing?DisconnectedPublishingPage";

        private const string XSL_Based_WebParts_Query_XPATH =
            "//*[contains(@name,'Microsoft.SharePoint.WebPartPages.XsltListViewWebPart')] | //*[contains(@name,'Microsoft.SharePoint.Publishing.WebControls.SummaryLinkWebPart')]";

        private const string _PAGELAYOUT_NOT_CREATED_URL_FORMAT = "^/[0-9]+/[^/]+\\.aspx$";

        private Dictionary<Guid, string> m_SubWebGuidDict;

        private readonly static ICollection<string> WebpageFileExtensions;

        private string m_sUrl;

        private string m_sServerRelativeUrl;

        private string m_sSiteCollectionUrl;

        private string m_sWebID;

        private string m_sSiteId;

        private bool m_sIsPortal2003;

        private string m_sLanguage;

        private int m_iRegionalCulture = -1;

        private System.Globalization.CultureInfo m_Culture;

        private TimeZoneInformation m_timeZone;

        private static DictionaryEntry[] TeamSiteTemplateVariations;

        private static DictionaryEntry[] MeetingSiteTemplateVariations;

        private static bool m_ClientOMLoadTried;

        private static Type m_ClientOMType;

        private static object m_ClientOMObject;

        private Dictionary<string, string> m_UserMap;

        private Dictionary<string, string> m_ReverseUserMap;

        private Dictionary<string, string> m_GroupMap;

        private Dictionary<string, string> m_ReverseGroupMap;

        private Dictionary<Guid, string> m_GlobalWebGuidDict;

        private Dictionary<string, string> m_ContentTypeDict;

        private string m_SharePointVersionString;

        private Metalogix.Permissions.Credentials m_credentials = new Metalogix.Permissions.Credentials();

        private string m_sSiteCollectionWebID;

        private string m_sSiteCollectionID;

        private string m_sTaxonomyListID;

        private XmlNamespaceManager m_remapperNametable;

        private WebPartTemplateResourceManager m_webPartPageTemplateManager;

        private string Base2003
        {
            get { return string.Concat(this.Language, "/ShrOpt"); }
        }

        public bool ClientOMAvailable
        {
            get
            {
                if (this.ClientOMObject == null)
                {
                    return false;
                }

                return base.SharePointVersion.IsSharePoint2010;
            }
        }

        private object ClientOMObject
        {
            get
            {
                if (NWSAdapter.m_ClientOMObject == null)
                {
                    Type clientOMType = this.ClientOMType;
                }

                return NWSAdapter.m_ClientOMObject;
            }
        }

        private Type ClientOMType
        {
            get
            {
                if (!NWSAdapter.m_ClientOMLoadTried && NWSAdapter.m_ClientOMType == null)
                {
                    NWSAdapter.m_ClientOMLoadTried = true;
                    try
                    {
                        Assembly executingAssembly = Assembly.GetExecutingAssembly();
                        FileInfo fileInfo = new FileInfo(executingAssembly.Location);
                        string str = string.Concat(fileInfo.DirectoryName,
                            "\\Metalogix.SharePoint.Adapters.ClientOM.dll");
                        Assembly assembly = null;
                        if (!File.Exists(str))
                        {
                            fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
                            str = string.Concat(fileInfo.DirectoryName, "\\Metalogix.SharePoint.Adapters.ClientOM.dll");
                        }

                        assembly = (!File.Exists(str)
                            ? Assembly.Load(string.Format(
                                "Metalogix.SharePoint.Adapters.ClientOM, Version={0}, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4",
                                executingAssembly.GetName().Version.ToString()))
                            : Assembly.LoadFrom(str));
                        NWSAdapter.m_ClientOMType =
                            assembly.GetType("Metalogix.SharePoint.Adapters.ClientOM.ClientOMTools");
                        if (NWSAdapter.m_ClientOMType != null)
                        {
                            NWSAdapter.m_ClientOMObject = Activator.CreateInstance(NWSAdapter.m_ClientOMType);
                            if (NWSAdapter.m_ClientOMObject != null)
                            {
                                object[] url = new object[] { this.Url };
                                NWSAdapter.m_ClientOMType.InvokeMember("TestClientOMTools", BindingFlags.InvokeMethod,
                                    null, NWSAdapter.m_ClientOMObject, url);
                            }
                            else
                            {
                                NWSAdapter.m_ClientOMType = null;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        NWSAdapter.m_ClientOMType = null;
                        NWSAdapter.m_ClientOMObject = null;
                    }
                }

                return NWSAdapter.m_ClientOMType;
            }
        }

        protected Dictionary<string, string> ContentTypeDictionary
        {
            get
            {
                if (this.m_ContentTypeDict == null)
                {
                    this.m_ContentTypeDict = new Dictionary<string, string>();
                    string contentTypes = this.GetContentTypes(null);
                    foreach (XmlNode xmlNodes in XmlUtility.StringToXmlNode(contentTypes).SelectNodes("//ContentType"))
                    {
                        if (this.m_ContentTypeDict.ContainsKey(xmlNodes.Attributes["Name"].Value))
                        {
                            continue;
                        }

                        this.m_ContentTypeDict.Add(xmlNodes.Attributes["Name"].Value, xmlNodes.Attributes["ID"].Value);
                    }
                }

                return this.m_ContentTypeDict;
            }
        }

        public override Metalogix.Permissions.Credentials Credentials
        {
            get { return this.m_credentials; }
            set { this.m_credentials = value; }
        }

        private System.Globalization.CultureInfo CultureInfo
        {
            get
            {
                if (this.m_Culture == null)
                {
                    this.m_Culture = new System.Globalization.CultureInfo(this.RegionalCulture);
                }

                return this.m_Culture;
            }
        }

        public override Metalogix.SharePoint.Adapters.ExternalizationSupport ExternalizationSupport
        {
            get { return Metalogix.SharePoint.Adapters.ExternalizationSupport.NotSupported; }
        }

        protected Dictionary<Guid, string> GlobalWebGuidDictionary
        {
            get
            {
                if (this.m_GlobalWebGuidDict == null)
                {
                    this.m_GlobalWebGuidDict = new Dictionary<Guid, string>();
                }

                return this.m_GlobalWebGuidDict;
            }
        }

        public override bool IsClientOM
        {
            get { return this.ClientOMAvailable; }
        }

        public override bool IsNws
        {
            get { return true; }
        }

        public override bool IsPortal2003Connection
        {
            get { return this.m_sIsPortal2003; }
        }

        public override bool IsReadOnlyAdapter
        {
            get
            {
                if (base.IsReadOnlyAdapter)
                {
                    return true;
                }

                return base.SharePointVersion.IsSharePoint2003;
            }
            set { base.IsReadOnlyAdapter = value; }
        }

        private string Language
        {
            get
            {
                if (this.m_sLanguage == null)
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter());
                    xmlTextWriter.WriteStartElement("Web");
                    this.FillWebXML(xmlTextWriter, this.Url, null, true, true);
                    xmlTextWriter.WriteEndElement();
                }

                return this.m_sLanguage;
            }
        }

        public override ISharePointReader Reader
        {
            get { return SharePointReader.GetSharePointReader(this); }
        }

        private int RegionalCulture
        {
            get
            {
                if (this.m_iRegionalCulture < 0)
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter());
                    xmlTextWriter.WriteStartElement("Web");
                    this.FillWebXML(xmlTextWriter, this.Url, null, true, true);
                    xmlTextWriter.WriteEndElement();
                }

                return this.m_iRegionalCulture;
            }
        }

        public override string Server
        {
            get { return NWSAdapter.GetServerPart(this.Url); }
        }

        public override string ServerRelativeUrl
        {
            get { return this.m_sServerRelativeUrl; }
        }

        public override string ServerType
        {
            get { return "Microsoft SharePoint Native WebService"; }
        }

        public override string ServerUrl
        {
            get { return NWSAdapter.GetServerPart(this.Url); }
        }

        public string SiteCollectionID
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_sSiteCollectionID))
                {
                    if (base.SharePointVersion.IsSharePoint2003)
                    {
                        return string.Empty;
                    }

                    SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.SiteCollectionUrl);
                    string str = null;
                    string str1 = null;
                    webServiceForSiteData.GetSiteUrl(this.SiteCollectionUrl, out str, out str1);
                    this.m_sSiteCollectionID = NWSAdapter.GetJustGUID(str1);
                }

                return this.m_sSiteCollectionID;
            }
        }

        public string SiteCollectionServerRelativeUrl
        {
            get
            {
                if (this.SiteCollectionUrl == null)
                {
                    return "";
                }

                string str = this.SiteCollectionUrl.Replace(this.Server, "");
                if (string.IsNullOrEmpty(str))
                {
                    str = "/";
                }

                return str;
            }
        }

        public string SiteCollectionUrl
        {
            get
            {
                string str;
                string str1;
                if (this.m_sSiteCollectionUrl == null)
                {
                    SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.Url);
                    webServiceForSiteData.GetSiteAndWeb(this.Url, out str, out str1);
                    this.m_sSiteCollectionUrl = str;
                }

                return this.m_sSiteCollectionUrl;
            }
        }

        public string SiteCollectionWebID
        {
            get
            {
                string str;
                string[] strArrays;
                string[] strArrays1;
                if (this.m_sSiteCollectionWebID == null)
                {
                    SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.SiteCollectionUrl);
                    _sWebMetadata _sWebMetadatum = null;
                    _sWebWithTime[] _sWebWithTimeArray = null;
                    _sListWithTime[] _sListWithTimeArray = null;
                    _sFPUrl[] _sFPUrlArray = null;
                    webServiceForSiteData.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray, out _sListWithTimeArray,
                        out _sFPUrlArray, out str, out strArrays, out strArrays1);
                    this.m_sSiteCollectionWebID = NWSAdapter.GetJustGUID(_sWebMetadatum.WebID);
                }

                return this.m_sSiteCollectionWebID;
            }
        }

        public string SiteId
        {
            get { return this.m_sSiteId; }
        }

        protected Dictionary<Guid, string> SubWebGuidDict
        {
            get
            {
                if (this.m_SubWebGuidDict == null)
                {
                    this.UpdateSubWebGuidDict();
                }

                return this.m_SubWebGuidDict;
            }
        }

        public override bool SupportsIDPreservation
        {
            get { return false; }
        }

        public string TaxonomyListID
        {
            get
            {
                if (this.m_sTaxonomyListID == null)
                {
                    if (!base.SharePointVersion.IsSharePoint2010OrLater)
                    {
                        this.m_sTaxonomyListID = "";
                    }
                    else
                    {
                        ListsService webServiceForLists = this.GetWebServiceForLists(this.SiteCollectionUrl);
                        XmlNode xmlNodes = XmlUtility.MatchFirstAttributeValue("Title", "TaxonomyHiddenList",
                            webServiceForLists.GetListCollection().ChildNodes);
                        if (xmlNodes == null)
                        {
                            this.m_sTaxonomyListID = "";
                        }
                        else
                        {
                            this.m_sTaxonomyListID = NWSAdapter.GetJustGUID(xmlNodes.Attributes["ID"].Value);
                        }
                    }
                }

                return this.m_sTaxonomyListID;
            }
        }

        public TimeZoneInformation TimeZone
        {
            get
            {
                if (this.m_timeZone == null)
                {
                    try
                    {
                        int webTimeZoneFromPage = this.GetWebTimeZoneFromPage(this.Url);
                        this.m_timeZone = this.GetTimeZoneFromID(webTimeZoneFromPage);
                    }
                    catch
                    {
                    }

                    if (this.m_timeZone == null)
                    {
                        this.m_timeZone = TimeZoneInformation.GetLocalTimeZone();
                    }
                }

                return this.m_timeZone;
            }
        }

        public override string Url
        {
            get { return this.m_sUrl; }
            set { this.SetUrl(value); }
        }

        public override string WebID
        {
            get { return this.m_sWebID; }
            set { this.m_sWebID = value; }
        }

        internal WebPartTemplateResourceManager WebPartPageTemplateManager
        {
            get
            {
                if (this.m_webPartPageTemplateManager == null)
                {
                    this.m_webPartPageTemplateManager = new WebPartTemplateResourceManager(
                        WebPartTemplateResourceLocation.EmbeddedWithinAssembly, base.SharePointVersion, null);
                }

                return this.m_webPartPageTemplateManager;
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

        static NWSAdapter()
        {
            HashSet<string> strs = new HashSet<string>();
            strs.Add(".asp");
            strs.Add(".aspx");
            strs.Add(".htm");
            strs.Add(".html");
            strs.Add(".xhtml");
            NWSAdapter.WebpageFileExtensions = strs;
            DictionaryEntry[] dictionaryEntry = new DictionaryEntry[]
                { new DictionaryEntry("STS#1", new ServerTemplate[0]), default(DictionaryEntry) };
            ServerTemplate[] serverTemplateArray = new ServerTemplate[]
            {
                ServerTemplate.Announcements, ServerTemplate.Events, ServerTemplate.DiscussionBoard,
                ServerTemplate.Links, ServerTemplate.DocumentLibrary, ServerTemplate.Tasks
            };
            dictionaryEntry[1] = new DictionaryEntry("STS#0", serverTemplateArray);
            NWSAdapter.TeamSiteTemplateVariations = dictionaryEntry;
            DictionaryEntry[] dictionaryEntryArray = new DictionaryEntry[5];
            ServerTemplate[] serverTemplateArray1 = new ServerTemplate[] { ServerTemplate.Attendees };
            dictionaryEntryArray[0] = new DictionaryEntry("MPS#1", serverTemplateArray1);
            ServerTemplate[] serverTemplateArray2 = new ServerTemplate[]
                { ServerTemplate.Attendees, ServerTemplate.Agenda, ServerTemplate.Objectives };
            dictionaryEntryArray[1] = new DictionaryEntry("MPS#4", serverTemplateArray2);
            ServerTemplate[] serverTemplateArray3 = new ServerTemplate[]
            {
                ServerTemplate.Attendees, ServerTemplate.Agenda, ServerTemplate.Objectives,
                ServerTemplate.DocumentLibrary
            };
            dictionaryEntryArray[2] = new DictionaryEntry("MPS#0", serverTemplateArray3);
            ServerTemplate[] serverTemplateArray4 = new ServerTemplate[]
            {
                ServerTemplate.Attendees, ServerTemplate.Agenda, ServerTemplate.Objectives,
                ServerTemplate.DocumentLibrary, ServerTemplate.Decisons, ServerTemplate.Tasks
            };
            dictionaryEntryArray[3] = new DictionaryEntry("MPS#2", serverTemplateArray4);
            ServerTemplate[] serverTemplateArray5 = new ServerTemplate[]
            {
                ServerTemplate.Attendees, ServerTemplate.Directions, ServerTemplate.DiscussionBoard,
                ServerTemplate.PictureLibrary, ServerTemplate.ThingsToBring
            };
            dictionaryEntryArray[4] = new DictionaryEntry("MPS#3", serverTemplateArray5);
            NWSAdapter.MeetingSiteTemplateVariations = dictionaryEntryArray;
            NWSAdapter.m_ClientOMLoadTried = false;
            NWSAdapter.m_ClientOMType = null;
            NWSAdapter.m_ClientOMObject = null;
        }

        public NWSAdapter()
        {
        }

        public NWSAdapter(string sUrl, Metalogix.Permissions.Credentials creds)
        {
            this.SetUrl(sUrl);
            this.m_credentials = creds;
        }

        public string ActivateReusableWorkflowTemplates()
        {
            return null;
        }

        public string AddAlerts(string sSiteUrl, string sWebId, string sAlertXML)
        {
            throw new NotImplementedException();
        }

        private string AddAndRemoveContentTypeXmlFields(Dictionary<string, string> PropertiesToUpdate,
            List<XmlNode> FieldsToAddToContentType, ListsService listsService, string sListId, string sContentTypeID)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            int num = 1;
            foreach (XmlNode fieldsToAddToContentType in FieldsToAddToContentType)
            {
                xmlTextWriter.WriteStartElement("Method");
                xmlTextWriter.WriteAttributeString("ID", num.ToString());
                xmlTextWriter.WriteRaw(fieldsToAddToContentType.OuterXml);
                xmlTextWriter.WriteEndElement();
                num++;
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            StringBuilder stringBuilder1 = new StringBuilder();
            XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder1));
            xmlWriter.WriteStartElement("ContentType");
            foreach (KeyValuePair<string, string> propertiesToUpdate in PropertiesToUpdate)
            {
                xmlWriter.WriteAttributeString(propertiesToUpdate.Key, propertiesToUpdate.Value);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(stringBuilder1.ToString());
            XmlDocument xmlDocument2 = new XmlDocument();
            xmlDocument2.LoadXml("<Fields />");
            XmlDocument xmlDocument3 = new XmlDocument();
            xmlDocument3.LoadXml("<Fields />");
            listsService.UpdateContentType(sListId, sContentTypeID, xmlDocument1.DocumentElement,
                xmlDocument.DocumentElement, xmlDocument2.DocumentElement, xmlDocument3.DocumentElement, "false");
            XmlNode listContentType = listsService.GetListContentType(sListId, sContentTypeID);
            StringBuilder stringBuilder2 = new StringBuilder();
            XmlTextWriter xmlTextWriter1 = new XmlTextWriter(new StringWriter(stringBuilder2));
            this.WriteContentTypeXml(sListId, xmlTextWriter1, new Hashtable(), listContentType);
            xmlTextWriter1.Flush();
            return stringBuilder2.ToString();
        }

        public XmlNode AddBCSList(string listName, string description, XmlNode listXml)
        {
            XmlNode xmlNode;
            if (!this.ClientOMAvailable)
            {
                throw new Exception(
                    "Unable to create external list, because the Client Object Model is not available.");
            }

            object[] objArray = new object[] { listName, description, null, null, null, null, null };
            objArray[2] = (listXml.Attributes["Entity"] != null ? listXml.Attributes["Entity"].Value : string.Empty);
            objArray[3] = (listXml.Attributes["EntityNamespace"] != null
                ? listXml.Attributes["EntityNamespace"].Value
                : string.Empty);
            objArray[4] = (listXml.Attributes["LobSystemInstance"] != null
                ? listXml.Attributes["LobSystemInstance"].Value
                : string.Empty);
            objArray[5] = (listXml.Attributes["SpecificFinder"] != null
                ? listXml.Attributes["SpecificFinder"].Value
                : string.Empty);
            objArray[6] = this;
            object[] objArray1 = objArray;
            try
            {
                string str = (string)this.ExecuteClientOMMethod("AddExternalList", objArray1);
                xmlNode = XmlUtility.StringToXmlNode(this.GetList(str));
            }
            catch (Exception exception)
            {
                throw;
            }

            return xmlNode;
        }

        public string AddDocument(string sListID, string sParentFolder, string sListItemXML, byte[] fileContents,
            string listSettingsXml, AddDocumentOptions Options)
        {
            XmlNode xmlNodes;
            string str;
            bool flag;
            string value;
            if (this.ClientOMAvailable)
            {
                object[] objArray = new object[] { sListID, sParentFolder, sListItemXML, fileContents, Options, this };
                return (string)this.ExecuteClientOMMethod("AddDocument", objArray);
            }

            bool overwrite = Options.Overwrite;
            string value1 = null;
            string str1 = null;
            ListsService webServiceForLists = this.GetWebServiceForLists();
            xmlNodes = (!string.IsNullOrEmpty(listSettingsXml)
                ? XmlUtility.StringToXmlNode(listSettingsXml)
                : webServiceForLists.GetList(sListID));
            ListType listBaseType = this.GetListBaseType(xmlNodes);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
            string str2 = (xmlNode.Attributes["ows_FileLeafRef"] != null ? "ows_" : "");
            string rootFolder = this.GetRootFolder(xmlNodes.Attributes);
            int num = rootFolder.LastIndexOf('/');
            string str3 = rootFolder.Substring(num + 1);
            bool flag1 = (xmlNodes.Attributes["EnableVersioning"] != null
                ? bool.Parse(xmlNodes.Attributes["EnableVersioning"].Value)
                : false);
            bool flag2 = Utils.IsDocumentWikiPage(xmlNode, str2);
            bool itemOf = xmlNode.Attributes[string.Concat(str2, "PublishingPageLayout")] != null;
            string listServerTemplate = this.GetListServerTemplate(xmlNodes);
            bool flag3 = listServerTemplate == "212";
            bool flag4 = listServerTemplate == "116";
            if (flag2)
            {
                fileContents = this.WebPartPageTemplateManager.GetWikiTemplate();
                if (!string.IsNullOrEmpty(rootFolder))
                {
                    string str4 = UrlUtils.StandardizeFormat(rootFolder);
                    xmlNode.Attributes[string.Concat(str2, "WikiField")].Value = xmlNode
                        .Attributes[string.Concat(str2, "WikiField")].Value.Replace(string.Concat(" href=\"", str4),
                            string.Concat(" href=\"", rootFolder));
                }
            }

            string str5 = (string.IsNullOrEmpty(str2)
                ? xmlNode.Attributes["FileLeafRef"].Value
                : xmlNode.Attributes["ows_FileLeafRef"].Value);
            string str6 =
                ((xmlNode.Attributes[string.Concat(str2, "_VersionLevel")] == null
                    ? "1"
                    : xmlNode.Attributes[string.Concat(str2, "_VersionLevel")].Value) == "2"
                    ? "0"
                    : "1");
            if (!flag1)
            {
                str = "0.1";
            }
            else
            {
                str = (xmlNode.Attributes[string.Concat(str2, "_VersionString")] == null
                    ? "1"
                    : xmlNode.Attributes[string.Concat(str2, "_VersionString")].Value);
            }

            this.UpdateContentTypeID(xmlNode);
            string str7 = string.Concat(str3, sParentFolder, "/", str5);
            if (flag4)
            {
                str7 = string.Concat("_catalogs/", str7);
                str3 = string.Concat("_catalogs/", str3);
            }

            string str8 = null;
            string[] strArrays = new string[] { "ows_FileLeafRef", "ows_FileDirRef" };
            string[] strArrays1 = new string[] { "Text", "Text" };
            string[] strArrays2 = new string[] { str5, null };
            string str9 = string.Concat(rootFolder, sParentFolder);
            char[] chrArray = new char[] { '/' };
            strArrays2[1] = str9.Trim(chrArray);
            string[] strArrays3 = strArrays2;
            XmlNode xmlNodes1 = this.ListItemQuery(sListID, listBaseType, strArrays, strArrays1, strArrays3,
                webServiceForLists);
            bool flag5 = xmlNodes1 == null;
            if (flag5)
            {
                flag = true;
            }
            else
            {
                flag = (flag5 ? false : overwrite);
            }

            flag5 = flag;
            if (overwrite && xmlNodes1 != null && xmlNodes1.Attributes["ows_FileRef"] != null)
            {
                string value2 = xmlNodes1.Attributes["ows_FileRef"].Value;
                int num1 = value2.IndexOf(";#");
                if (num1 >= 0)
                {
                    value2 = value2.Remove(0, num1 + 2);
                }

                if (!value2.StartsWith("/"))
                {
                    value2 = string.Concat("/", value2);
                }

                RPCUtil.DeleteDocument(this, value2);
            }

            string str10 = null;
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(xmlNodes, "//sp:Fields/sp:Field | //List/Fields/Field");
            string str11 = null;
            this.ReMapManagedMetadataFields(xmlNodeLists, ref xmlNode);
            if (flag3)
            {
                if (flag5)
                {
                    this.AddMeetingWorkspacePage(str7, overwrite, null);
                }

                xmlNodes1 = this.ListItemQuery(sListID, listBaseType, strArrays, strArrays1, strArrays3,
                    webServiceForLists);
                value1 = xmlNodes1.Attributes["ows_ID"].Value;
                return this.GetListItems(sListID, value1, str1, null, false, ListItemQueryType.ListItem, null,
                    new GetListItemOptions());
            }

            if (!flag5)
            {
                string server = this.Server;
                string value3 = xmlNodes1.Attributes["ows_FileRef"].Value;
                char[] chrArray1 = new char[] { '#' };
                str11 = string.Concat(server, "/", value3.Split(chrArray1)[1]);
                webServiceForLists.CheckOutFile(str11, "true", null);
                xmlNodes1 = this.ListItemQuery(sListID, listBaseType, strArrays, strArrays1, strArrays3,
                    webServiceForLists);
            }
            else
            {
                if (flag2 || itemOf)
                {
                    str10 = this.BuildItemMetadataRPC(xmlNodeLists, xmlNode, str2);
                }

                str8 = RPCUtil.FormatFrontPageRPCMessage(str7, str10, true, overwrite,
                    base.SharePointVersion.VersionNumberString);
                RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str8,
                    new byte[0]);
                xmlNodes1 = this.ListItemQuery(sListID, listBaseType, strArrays, strArrays1, strArrays3,
                    webServiceForLists);
                if (xmlNodes1 == null)
                {
                    throw new Exception("RPC call did not upload document");
                }
            }

            value1 = xmlNodes1.Attributes["ows_ID"].Value;
            string server1 = this.Server;
            string value4 = xmlNodes1.Attributes["ows_FileRef"].Value;
            char[] chrArray2 = new char[] { '#' };
            str11 = string.Concat(server1, "/", value4.Split(chrArray2)[1]);
            string[] strArrays4 = null;
            strArrays4 = this.IncrementDocumentVersions(sListID, listBaseType, str, flag5, xmlNodes, xmlNodes1,
                webServiceForLists);
            str10 = this.BuildItemMetadataRPC(xmlNodeLists, xmlNode, str2);
            if (xmlNode.Attributes["_CheckinComment"] != null)
            {
                value = xmlNode.Attributes["_CheckinComment"].Value;
            }
            else
            {
                value = (xmlNode.Attributes["_CheckinComment"] != null
                    ? xmlNode.Attributes["_CheckinComment"].Value
                    : "");
            }

            string str12 = value;
            str8 = RPCUtil.FormatFrontPageRPCMessage(str7, str10, true, false,
                base.SharePointVersion.VersionNumberString);
            RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str8,
                fileContents);
            if (flag5 && (int)strArrays4.Length == 0)
            {
                webServiceForLists.CheckOutFile(str11, "true", null);
                str6 = "2";
            }

            RPCUtil.SetMetaInfoRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str5,
                string.Concat(str3, sParentFolder), str10);
            this.CheckInFinalVersion(xmlNodes, str, str12, str11, str7, str6, true, flag5, webServiceForLists);
            if (xmlNodes.Attributes["EnableModeration"] != null &&
                bool.Parse(xmlNodes.Attributes["EnableModeration"].Value) &&
                xmlNode.Attributes["_ModerationStatus"] != null && xmlNode.Attributes["_ModerationStatus"].Value != "3")
            {
                XmlNode xmlNodes2 = this.UpdateModerationStatus(webServiceForLists, xmlNodes, xmlNode, sListID, value1);
                if (xmlNodes2.InnerText != "0x00000000")
                {
                    throw new Exception(string.Concat("Add failed! ", xmlNodes2.InnerText));
                }
            }

            this.DeleteTempDocumentVersions(str7, strArrays4);
            if (xmlNode.Attributes["_IsCurrentVersion"] != null &&
                bool.Parse(xmlNode.Attributes["_IsCurrentVersion"].Value))
            {
                this.DeleteAllTemporaryVersions(str7);
            }

            return this.GetListItems(sListID, value1, str1, null, false, ListItemQueryType.ListItem, null,
                new GetListItemOptions());
        }

        public string AddDocumentOptimistically(Guid listId, string listName, string folderPath, string fileXml,
            byte[] fileContents, AddDocumentOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            if (!this.ClientOMAvailable)
            {
                return this.AddDocument(listId.ToString(), folderPath, fileXml, fileContents, null, options);
            }

            object[] objArray = new object[]
                { listId, listName, folderPath, fileXml, fileContents, options, fieldsLookupCache, this };
            object[] objArray1 = objArray;
            string str = (string)this.ExecuteClientOMMethod("AddDocumentOptimistically", objArray1);
            fieldsLookupCache = (FieldsLookUp)objArray1[(int)objArray1.Length - 2];
            return str;
        }

        public string AddDocumentSetVersions(string listName, string listItemID, string updatedTargetMetaInfo)
        {
            throw new NotImplementedException();
        }

        public string AddDocumentTemplatetoContentType(byte[] docTemplate, string cTypeXml, string url)
        {
            this.GetWebServiceForWebs();
            string str = null;
            string str1 =
                RPCUtil.FormatFrontPageRPCMessage(url, str, true, false, base.SharePointVersion.VersionNumberString);
            RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str1,
                docTemplate);
            return string.Empty;
        }

        public string AddFields(string sListID, string sFieldXML)
        {
            string value;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFieldXML);
            XmlNodeList xmlNodeLists = xmlDocument.DocumentElement.SelectNodes("./Field");
            List<XmlNode> xmlNodes = new List<XmlNode>();
            foreach (XmlNode xmlNodes1 in xmlNodeLists)
            {
                if (xmlNodes1.Attributes["DisplayName"] != null)
                {
                    value = xmlNodes1.Attributes["DisplayName"].Value;
                }
                else
                {
                    value = null;
                }

                string str = value;
                bool flag = false;
                if (xmlNodes1.Attributes["MLSystem"] != null)
                {
                    bool.TryParse(xmlNodes1.Attributes["MLSystem"].Value, out flag);
                }

                bool value1 = false;
                if (xmlNodes1.Attributes["Type"] != null)
                {
                    value1 = "Threading" == xmlNodes1.Attributes["Type"].Value;
                }

                if (str == null || flag || value1)
                {
                    continue;
                }

                xmlNodes.Add(xmlNodes1);
            }

            XmlNodeList xmlNodeLists1 = null;
            if (sListID == null)
            {
                WebsService webServiceForWebs = this.GetWebServiceForWebs();
                XmlNodeList xmlNodeLists2 = XmlUtility.RunXPathQuery(webServiceForWebs.GetColumns(), ".//sp:Field");
                List<XmlNode> xmlNodes2 = new List<XmlNode>();
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    XmlAttribute itemOf = xmlNode.Attributes["Name"];
                    if (itemOf == null)
                    {
                        continue;
                    }

                    foreach (XmlNode xmlNodes3 in xmlNodeLists2)
                    {
                        XmlAttribute xmlAttribute = xmlNodes3.Attributes["Name"];
                        if (xmlAttribute == null || !(xmlAttribute.Value == itemOf.Value))
                        {
                            continue;
                        }

                        xmlNodes2.Add(xmlNode);
                        break;
                    }
                }

                foreach (XmlNode xmlNode1 in xmlNodes2)
                {
                    xmlNodes.Remove(xmlNode1);
                }

                this.AddSiteColumns(webServiceForWebs, xmlNodes);
                xmlNodeLists1 = this.FetchSiteColumns(webServiceForWebs);
            }
            else
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                XmlNodeList xmlNodeLists3 = this.FetchListFields(sListID, webServiceForLists);
                this.AddListFields(sListID, webServiceForLists, xmlNodes, xmlNodeLists3);
                xmlNodeLists1 = this.FetchListFields(sListID, webServiceForLists);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            if (xmlNodeLists1 != null)
            {
                foreach (XmlNode xmlNodes4 in xmlNodeLists1)
                {
                    xmlTextWriter.WriteRaw(
                        xmlNodes4.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\"", ""));
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        private void AddFieldToSiteContentType(string sContentTypeName, XmlNode webContentTypes, XmlNode field,
            WebsService websService)
        {
            if (string.IsNullOrEmpty(sContentTypeName))
            {
                return;
            }

            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(webContentTypes,
                string.Concat(".//sp:ContentType[@Name=\"", sContentTypeName, "\"]"));
            if (xmlNodeLists == null || xmlNodeLists.Count == 0)
            {
                return;
            }

            XmlNode itemOf = xmlNodeLists[0];
            XmlAttribute xmlAttribute = itemOf.Attributes["ID"];
            if (xmlAttribute == null)
            {
                return;
            }

            string value = xmlAttribute.Value;
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            xmlTextWriter.WriteStartElement("Method");
            xmlTextWriter.WriteAttributeString("ID", "1");
            xmlTextWriter.WriteRaw(field.OuterXml);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            websService.UpdateContentType(value, itemOf, xmlDocument.DocumentElement, null, null);
        }

        public string AddFileToFolder(string sFileXML, byte[] fileContents, AddDocumentOptions Options)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sFileXML);
            string[] strArrays = new string[]
            {
                RPCUtil.FormatFieldForRPC("vti_author", xmlNode.Attributes["Author"].Value, typeof(string),
                    this.TimeZone),
                ";",
                RPCUtil.FormatFieldForRPC("vti_modifiedby", xmlNode.Attributes["ModifiedBy"].Value, typeof(string),
                    this.TimeZone),
                ";",
                RPCUtil.FormatFieldForRPC("vti_timecreated", xmlNode.Attributes["TimeCreated"].Value, typeof(DateTime),
                    this.TimeZone),
                ";",
                RPCUtil.FormatFieldForRPC("vti_timelastmodified", xmlNode.Attributes["TimeLastModified"].Value,
                    typeof(DateTime), this.TimeZone)
            };
            string str = string.Concat(strArrays);
            string str1 = RPCUtil.FormatFrontPageRPCMessage(xmlNode.Attributes["Url"].Value, str, true, false,
                base.SharePointVersion.VersionNumberString);
            RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str1,
                fileContents);
            XmlNode folderContentFromWeb = RPCUtil.GetFolderContentFromWeb(this,
                xmlNode.Attributes["ParentFolderPath"].Value, ListItemQueryType.ListItem);
            XmlNode xmlNodes = folderContentFromWeb.SelectSingleNode(string.Concat("/FolderContent/Files/File[@Url='",
                xmlNode.Attributes["Url"].Value, "']"));
            StringBuilder stringBuilder = new StringBuilder();
            NWSAdapter.AddFileToXml(new XmlTextWriter(new StringWriter(stringBuilder)), xmlNodes);
            return stringBuilder.ToString();
        }

        private static void AddFileToXml(XmlTextWriter xmlWriter, XmlNode node)
        {
            xmlWriter.WriteStartElement("File");
            xmlWriter.WriteAttributeString("Author", NWSAdapter.GetXmlNodeAttributeValue(node, "vti_author"));
            if (node.Attributes["vti_hasdefaultcontent"] == null ||
                !bool.Parse(node.Attributes["vti_hasdefaultcontent"].Value))
            {
                xmlWriter.WriteAttributeString("CustomizedPageStatus", "None");
            }
            else
            {
                xmlWriter.WriteAttributeString("CustomizedPageStatus", "Uncustomized");
            }

            xmlWriter.WriteAttributeString("ModifiedBy", NWSAdapter.GetXmlNodeAttributeValue(node, "vti_modifiedby"));
            xmlWriter.WriteAttributeString("Name", NWSAdapter.GetXmlNodeAttributeValue(node, "Name"));
            DateTime dateTime = DateTime.Parse(NWSAdapter.GetXmlNodeAttributeValue(node, "vti_timecreated"), null,
                DateTimeStyles.AssumeUniversal);
            xmlWriter.WriteAttributeString("TimeCreated", Utils.FormatDate(dateTime.ToUniversalTime()));
            dateTime = DateTime.Parse(NWSAdapter.GetXmlNodeAttributeValue(node, "vti_timelastmodified"), null,
                DateTimeStyles.AssumeUniversal);
            xmlWriter.WriteAttributeString("TimeLastModified", Utils.FormatDate(dateTime.ToUniversalTime()));
            if (node.Attributes["vti_etag"] == null)
            {
                xmlWriter.WriteAttributeString("UniqueId", Guid.Empty.ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("UniqueId",
                    (new Regex("{(?<match1>.*)}")).Match(node.Attributes["vti_etag"].Value).Groups["match1"].Value);
            }

            xmlWriter.WriteAttributeString("Url", NWSAdapter.GetXmlNodeAttributeValue(node, "Url"));
            xmlWriter.WriteEndElement();
        }

        public string AddFolder(string sListID, string sParentFolder, string sFolderXML, AddFolderOptions Options)
        {
            if (this.ClientOMAvailable)
            {
                object[] objArray = new object[] { sFolderXML, sParentFolder, sListID, Options, this };
                return (string)this.ExecuteClientOMMethod("AddFolder", objArray);
            }

            string innerText = null;
            string str = null;
            try
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                XmlNode xmlNode = XmlUtility.StringToXmlNode(sFolderXML);
                string str1 = (xmlNode.Attributes["FileLeafRef"] != null
                    ? xmlNode.Attributes["FileLeafRef"].Value
                    : xmlNode.Attributes["Title"].Value);
                if (Options.Overwrite)
                {
                    string str2 = null;
                    int num = -1;
                    this.GetExistingFolderInfo(sListID, sParentFolder, webServiceForLists, str1, out num, out str2);
                    if (num >= 0)
                    {
                        this.DeleteFolder(sListID, num, null);
                    }
                }

                XmlDocument xmlDocument = new XmlDocument();
                XmlElement xmlElement = xmlDocument.CreateElement("Batch");
                xmlElement.SetAttribute("OnError", "Continue");
                xmlElement.SetAttribute("ListVersion", "1");
                if (!string.IsNullOrEmpty(sParentFolder))
                {
                    XmlNode list = webServiceForLists.GetList(sListID);
                    string str3 = string.Concat(this.GetRootFolder(list.Attributes), sParentFolder);
                    xmlElement.SetAttribute("RootFolder", str3);
                }

                XmlElement xmlElement1 = xmlDocument.CreateElement("Method");
                xmlElement1.SetAttribute("ID", "1");
                xmlElement1.SetAttribute("Cmd", "New");
                XmlElement xmlElement2 = xmlDocument.CreateElement("Field");
                xmlElement2.SetAttribute("Name", "ID");
                xmlElement2.InnerText = "New";
                xmlDocument.CreateElement("Field");
                XmlElement xmlElement3 = xmlDocument.CreateElement("Field");
                xmlElement3.SetAttribute("Name", "FSObjType");
                xmlElement3.InnerText = "1";
                XmlElement xmlElement4 = xmlDocument.CreateElement("Field");
                xmlElement4.SetAttribute("Name", "BaseName");
                xmlElement4.InnerText = str1;
                xmlElement1.AppendChild(xmlElement2);
                xmlElement1.AppendChild(xmlElement3);
                xmlElement1.AppendChild(xmlElement4);
                xmlElement.AppendChild(xmlElement1);
                XmlNode xmlNodes = webServiceForLists.UpdateListItems(sListID, xmlElement);
                if (xmlNodes.InnerText == "0x00000000")
                {
                    foreach (XmlNode xmlNodes1 in XmlUtility.RunXPathQuery(xmlNodes, "//z:row"))
                    {
                        XmlAttributeCollection attributes = xmlNodes1.Attributes;
                        innerText = attributes["ows_ID"].InnerText;
                        string innerText1 = attributes["ows_FileDirRef"].InnerText;
                        char[] chrArray = new char[] { '#' };
                        str = innerText1.Split(chrArray)[1];
                    }
                }
                else
                {
                    try
                    {
                        int num1 = -1;
                        this.GetExistingFolderInfo(sListID, sParentFolder, webServiceForLists, str1, out num1, out str);
                        if (num1 < 0)
                        {
                            throw new Exception(string.Concat("AddFolder Failed! ", xmlNodes.InnerText));
                        }

                        innerText = num1.ToString();
                    }
                    catch
                    {
                        throw new Exception(string.Concat("AddFolder Failed! ", xmlNodes.InnerText));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            if (str == null)
            {
                throw new Exception("AddFolder Failed! Could not find FileDirRef.");
            }

            return this.GetFolders(sListID, innerText, str);
        }

        public string AddFolderOptimistically(Guid listId, string listName, string folderPath, string folderXml,
            AddFolderOptions options, ref FieldsLookUp fieldsLookupCache)
        {
            if (!this.ClientOMAvailable)
            {
                return this.AddFolder(listId.ToString(), folderPath, folderXml, options);
            }

            object[] objArray = new object[]
                { listId, listName, folderPath, folderXml, options, fieldsLookupCache, this };
            object[] objArray1 = objArray;
            string str = (string)this.ExecuteClientOMMethod("AddFolderOptimistically", objArray1);
            fieldsLookupCache = (FieldsLookUp)objArray1[(int)objArray1.Length - 2];
            return str;
        }

        public string AddFolderToFolder(string sFolderXML)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sFolderXML);
            string str = string.Format(
                "method=create url-directories:{0}&service_name={1}&urldirs=[[url={2};meta_info=[]]]",
                (base.SharePointVersion.VersionNumberString == null
                    ? "6.0.2.6356"
                    : base.SharePointVersion.VersionNumberString), this.ServerRelativeUrl,
                xmlNode.Attributes["Url"].Value);
            RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str,
                new byte[0]);
            XmlNode folderContentFromWeb = RPCUtil.GetFolderContentFromWeb(this,
                xmlNode.Attributes["ParentFolderPath"].Value, ListItemQueryType.ListItem | ListItemQueryType.Folder);
            XmlNode xmlNodes = folderContentFromWeb.SelectSingleNode(
                string.Concat("/FolderContent/Folders/Folder[@Url='", xmlNode.Attributes["Url"].Value, "']"));
            StringBuilder stringBuilder = new StringBuilder();
            NWSAdapter.AddFolderToXml(new XmlTextWriter(new StringWriter(stringBuilder)), xmlNodes);
            return stringBuilder.ToString();
        }

        private static void AddFolderToXml(XmlTextWriter xmlWriter, XmlNode node)
        {
            xmlWriter.WriteStartElement("Folder");
            xmlWriter.WriteAttributeString("Name", NWSAdapter.GetXmlNodeAttributeValue(node, "Name"));
            xmlWriter.WriteAttributeString("ParentListId",
                (node.Attributes["ParentListId"] != null
                    ? node.Attributes["ParentListId"].Value
                    : Guid.Empty.ToString()));
            xmlWriter.WriteAttributeString("Url", NWSAdapter.GetXmlNodeAttributeValue(node, "Url"));
            xmlWriter.WriteEndElement();
        }

        public string AddFormTemplateToContentType(string targetListID, byte[] docTemplate, string cTypeXml,
            string changedFields)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            if (!base.SharePointVersion.IsSharePoint2010OrLater)
            {
                operationReporting.LogWarning("Target environment is not SharePoint 2010 or later", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            XmlNode xmlNode = XmlUtility.StringToXmlNode(cTypeXml);
            string value = "";
            if (xmlNode != null)
            {
                XmlAttribute attribute = XmlUtility.GetAttribute(xmlNode, "", "ID", false);
                if (attribute != null)
                {
                    value = attribute.Value;
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                operationReporting.LogWarning("Content Type Id is null or empty.", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            if (string.IsNullOrEmpty(targetListID))
            {
                operationReporting.LogWarning("Target List Id is null or empty.", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            FormsService webServiceForForms = this.GetWebServiceForForms();
            if (webServiceForForms == null)
            {
                operationReporting.LogWarning("Infopath Forms Service not present", string.Empty);
                operationReporting.End();
                return operationReporting.ResultXml;
            }

            try
            {
                try
                {
                    DesignCheckerInformation designCheckerInformation = webServiceForForms.SetFormsForListItem(1033,
                        Convert.ToBase64String(docTemplate), "Content Matrix Console", targetListID, value);
                    if (designCheckerInformation.Messages != null && (int)designCheckerInformation.Messages.Length > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        Message[] messages = designCheckerInformation.Messages;
                        for (int i = 0; i < (int)messages.Length; i++)
                        {
                            Message message = messages[i];
                            stringBuilder.Length = 0;
                            object[] str = new object[]
                            {
                                message.Id.ToString(), message.Feature.ToString(), message.Category.ToString(),
                                message.Type.ToString()
                            };
                            stringBuilder.AppendLine(string.Format("Id:{0}, Feature:{1}, Category:{2}, Type:{3}", str));
                            if (!string.IsNullOrEmpty(message.DetailedMessage))
                            {
                                stringBuilder.AppendLine(string.Format("DetailedMessage:{0}", message.DetailedMessage));
                            }

                            switch (message.Type)
                            {
                                case MessageType.Error:
                                {
                                    operationReporting.LogError(message.ShortMessage, stringBuilder.ToString(),
                                        string.Empty, 0, 0);
                                    break;
                                }
                                case MessageType.Information:
                                {
                                    operationReporting.LogInformation(message.ShortMessage, stringBuilder.ToString());
                                    break;
                                }
                                case MessageType.Warning:
                                {
                                    operationReporting.LogWarning(message.ShortMessage, stringBuilder.ToString());
                                    break;
                                }
                                default:
                                {
                                    goto case MessageType.Information;
                                }
                            }

                            operationReporting.LogInformation(message.ShortMessage, stringBuilder.ToString());
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    operationReporting.LogError("Exception occured in method AddFormTemplateToContentType",
                        exception.Message, exception.StackTrace, 0, 0);
                }
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        public string AddList(string sListXML, AddListOptions options, byte[] documentTemplateFile)
        {
            XmlNode xmlNodes;
            int num;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sListXML);
            if (xmlNode.Attributes["Name"] == null || string.IsNullOrEmpty(xmlNode.Attributes["Name"].Value))
            {
                throw new Exception(
                    "Invalid XML was supplied to AddList(). Required attribute 'Name' is either missing or empty.");
            }

            if (xmlNode.Attributes["BaseTemplate"] == null)
            {
                throw new Exception("Invalid XML was supplied to AddList(). Missing required 'BaseTemplate' attribute");
            }

            if (xmlNode.Attributes["Title"] == null)
            {
                throw new Exception("Invalid XML was supplied to AddList(). Missing required 'Title' attribute");
            }

            string justGUID = null;
            XmlAttributeCollection attributes = xmlNode.Attributes;
            string innerText = xmlNode.Attributes["Name"].InnerText;
            string str = (xmlNode.Attributes["Description"] == null ? "" : xmlNode.Attributes["Description"].InnerText);
            string innerText1 = xmlNode.Attributes["Title"].InnerText;
            if (xmlNode.Attributes["BaseTemplate"] == null)
            {
                throw new Exception("The required attribute: 'BaseTemplate' was missing");
            }

            if (base.SharePointVersion.IsSharePoint2010 && (xmlNode.Attributes["BaseTemplate"].InnerText == "851" ||
                                                            xmlNode.Attributes["BaseTemplate"].InnerText == "1302"))
            {
                XmlNode xmlNode1 = XmlUtility.StringToXmlNode(this.GetListTemplates());
                string str1 = null;
                foreach (XmlNode xmlNodes1 in xmlNode1)
                {
                    if (!(xmlNode.Attributes["BaseTemplate"].InnerText == "851") ||
                        !(xmlNodes1.Attributes["Type"].InnerText == "851"))
                    {
                        if (!(xmlNode.Attributes["BaseTemplate"].InnerText == "1302") ||
                            !(xmlNodes1.Attributes["Type"].InnerText == "1302"))
                        {
                            continue;
                        }

                        str1 = xmlNodes1.Attributes["FeatureId"].InnerText;
                        break;
                    }
                    else
                    {
                        str1 = xmlNodes1.Attributes["FeatureId"].InnerText;
                        break;
                    }
                }

                XmlAttribute itemOf = xmlNode.Attributes["FeatureId"];
                if (itemOf == null)
                {
                    itemOf = xmlNode.OwnerDocument.CreateAttribute("FeatureId");
                    xmlNode.Attributes.Append(itemOf);
                }

                itemOf.Value = str1;
            }

            int num1 = Convert.ToInt32((ServerTemplate)Enum.Parse(typeof(ServerTemplate),
                xmlNode.Attributes["BaseTemplate"].Value));
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNode xmlNodes2 = null;
            XmlNode xmlNodes3 = null;
            string value = null;
            XmlNode xmlNode2 = XmlUtility.StringToXmlNode(this.GetLists());
            if (xmlNodes2 == null)
            {
                try
                {
                    xmlNodes2 = XmlUtility.MatchFirstAttributeValue("Name", innerText, xmlNode2.ChildNodes);
                }
                catch
                {
                }
            }

            if (xmlNodes2 != null)
            {
                justGUID = NWSAdapter.GetJustGUID(xmlNodes2.Attributes["ID"].InnerText);
                XmlAttribute xmlAttribute = xmlNodes2.Attributes["BaseTemplate"];
                int num2 = -1;
                if (xmlAttribute != null && int.TryParse(xmlAttribute.Value, out num2) && num2 == 212)
                {
                    return this.GetList(justGUID);
                }

                if (options.Overwrite)
                {
                    bool flag = false;
                    try
                    {
                        if (num2 != 109)
                        {
                            webServiceForLists.DeleteList(justGUID);
                            xmlNode2 = XmlUtility.StringToXmlNode(this.GetLists());
                            xmlNodes2 = null;
                            justGUID = null;
                            flag = true;
                        }
                    }
                    catch (Exception exception)
                    {
                    }

                    if (!flag)
                    {
                        try
                        {
                            this.DeleteItems(justGUID, true, null);
                        }
                        catch (Exception exception1)
                        {
                        }
                    }
                }
            }

            bool flag1 = false;
            ListType listType = ListType.Unknown;
            try
            {
                if (xmlNodes2 == null)
                {
                    try
                    {
                        try
                        {
                            xmlNodes3 = XmlUtility.MatchFirstAttributeValue("Title", innerText, xmlNode2.ChildNodes);
                        }
                        catch
                        {
                        }

                        if (xmlNodes3 != null)
                        {
                            string str2 = string.Concat(innerText, "_");
                            value = xmlNodes3.Attributes["ID"].Value;
                            while (XmlUtility.MatchFirstAttributeValue("Title", str2, xmlNode2.ChildNodes) != null)
                            {
                                str2 = string.Concat(str2, "_");
                            }

                            webServiceForLists.UpdateList(value,
                                XmlUtility.StringToXmlNode(string.Concat("<List Title=\"", str2, "\" />")), null, null,
                                null, null);
                        }

                        if (xmlNode.Attributes["BaseTemplate"].Value == "600")
                        {
                            xmlNodes = this.AddBCSList(innerText, str, xmlNode);
                        }
                        else if (xmlNode.Attributes["FeatureId"] == null ||
                                 string.IsNullOrEmpty(xmlNode.Attributes["FeatureId"].Value))
                        {
                            xmlNodes = webServiceForLists.AddList(innerText, str, num1);
                        }
                        else
                        {
                            Guid guid = new Guid(xmlNode.Attributes["FeatureId"].Value);
                            xmlNodes = webServiceForLists.AddListFromFeature(innerText, str, guid, num1);
                        }
                    }
                    catch
                    {
                        ServerTemplate serverTemplate = ServerTemplate.GenericList;
                        switch (Convert.ToInt32(xmlNode.Attributes["BaseType"].InnerText))
                        {
                            case 0:
                            {
                                serverTemplate = ServerTemplate.GenericList;
                                goto case 2;
                            }
                            case 1:
                            {
                                serverTemplate = ServerTemplate.DocumentLibrary;
                                goto case 2;
                            }
                            case 2:
                            {
                                xmlNodes = webServiceForLists.AddList(innerText, str, (int)serverTemplate);
                                break;
                            }
                            case 3:
                            {
                                serverTemplate = ServerTemplate.DiscussionBoard;
                                goto case 2;
                            }
                            case 4:
                            {
                                serverTemplate = ServerTemplate.Survey;
                                goto case 2;
                            }
                            case 5:
                            {
                                serverTemplate = ServerTemplate.Issues;
                                goto case 2;
                            }
                            default:
                            {
                                goto case 2;
                            }
                        }
                    }

                    justGUID = NWSAdapter.GetJustGUID(xmlNodes.Attributes["ID"].InnerText);
                    xmlNode2 = XmlUtility.StringToXmlNode(this.GetLists());
                    if (xmlNodes.Attributes["MultipleDataList"] != null &&
                        !bool.TryParse(xmlNodes.Attributes["MultipleDataList"].Value, out flag1))
                    {
                        flag1 = false;
                    }

                    if (xmlNodes.Attributes["BaseType"] != null &&
                        int.TryParse(xmlNodes.Attributes["BaseType"].Value, out num))
                    {
                        listType = (ListType)num;
                    }
                }

                int num3 = 1;
                string str3 = innerText1;
                if (xmlNodes3 != null && innerText == innerText1)
                {
                    innerText1 = string.Concat(str3, num3.ToString());
                    num3++;
                }

                for (XmlNode i = XmlUtility.MatchFirstAttributeValue("Title", innerText1, xmlNode2.ChildNodes);
                     i != null && i.Attributes["Name"].Value != innerText;
                     i = XmlUtility.MatchFirstAttributeValue("Title", innerText1, xmlNode2.ChildNodes))
                {
                    innerText1 = string.Concat(str3, num3.ToString());
                    num3++;
                }

                this.UpdateListProperties(justGUID, innerText, innerText1, null, xmlNode, options, documentTemplateFile,
                    webServiceForLists, xmlNode2);
                if (xmlNode.Attributes["MultipleDataList"] != null)
                {
                    bool flag2 = bool.Parse(xmlNode.Attributes["MultipleDataList"].Value);
                    if (flag1 && !flag2 && listType == ListType.DocumentLibrary)
                    {
                        this.DeleteMeetingInstanceFolders(justGUID);
                    }
                }
            }
            finally
            {
                if (xmlNodes3 != null)
                {
                    webServiceForLists.UpdateList(value,
                        XmlUtility.StringToXmlNode(string.Concat("<List Title=\"", innerText, "\" />")), null, null,
                        null, null);
                }
            }

            return this.GetList(justGUID);
        }

        private void AddListFields(string sListId, ListsService listsService, List<XmlNode> FieldsToAddToList,
            XmlNodeList listFields)
        {
            int num;
            int num1;
            int num2;
            XmlNode xmlNodes;
            XmlNode xmlNodes1;
            XmlNode xmlNodes2;
            XmlNode xmlNodes3;
            XmlNode xmlNodes4;
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            foreach (XmlNode fieldsToAddToList in FieldsToAddToList)
            {
                xmlTextWriter.WriteRaw(fieldsToAddToList.OuterXml);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(this.GetLists());
            this.WriteFieldUpdateXml(childNodes, listFields, out num, out num1, out num2, out xmlNodes, out xmlNodes1,
                out xmlNodes2, xmlNode);
            ListsService listsService1 = listsService;
            string str = sListId;
            if (num > 0)
            {
                xmlNodes3 = xmlNodes;
            }
            else
            {
                xmlNodes3 = null;
            }

            if (num1 > 0)
            {
                xmlNodes4 = xmlNodes1;
            }
            else
            {
                xmlNodes4 = null;
            }

            listsService1.UpdateList(str, null, xmlNodes3, xmlNodes4, null, null);
        }

        private List<XmlNode> AddListFieldsAsSiteColumns(List<XmlNode> sourceFieldsToAdd, WebsService websService)
        {
            List<XmlNode> xmlNodes = new List<XmlNode>();
            List<XmlNode> xmlNodes1 = new List<XmlNode>();
            XmlNode columns = websService.GetColumns();
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            foreach (XmlNode xmlNodes2 in sourceFieldsToAdd)
            {
                XmlNode itemOf = xmlNodes2.Attributes["Name"];
                if (itemOf != null)
                {
                    XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(columns,
                        string.Concat(".//sp:Fields/sp:Field[@Name=\"", itemOf.Value, "\"]"));
                    if (xmlNodeLists != null && xmlNodeLists.Count > 0)
                    {
                        xmlNodes.Add(xmlNodeLists[0]);
                        xmlNodes1.Add(xmlNodes2);
                        continue;
                    }
                }

                if (xmlNodes2.Attributes["AddToSiteColumnGroup"] != null)
                {
                    XmlAttribute value = xmlNodes2.Attributes["Group"];
                    if (value == null)
                    {
                        value = xmlNodes2.OwnerDocument.CreateAttribute("Group");
                        xmlNodes2.Attributes.Append(value);
                    }

                    value.Value = xmlNodes2.Attributes["AddToSiteColumnGroup"].Value;
                    xmlNodes2.Attributes.Remove(xmlNodes2.Attributes["AddToSiteColumnGroup"]);
                    XmlAttribute xmlAttribute = xmlNodes2.Attributes["AddToContentType"];
                    if (xmlAttribute != null)
                    {
                        xmlNodes2.Attributes.Remove(xmlAttribute);
                    }

                    xmlTextWriter.WriteRaw(xmlNodes2.OuterXml);
                    if (xmlAttribute == null)
                    {
                        continue;
                    }

                    xmlNodes2.Attributes.Append(xmlAttribute);
                }
                else
                {
                    xmlNodes.Add(xmlNodes2);
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            string str = this.AddFields(null, stringBuilder.ToString());
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            XmlNode contentTypes = null;
            foreach (XmlNode xmlNodes3 in sourceFieldsToAdd)
            {
                if (xmlNodes1.Contains(xmlNodes3))
                {
                    continue;
                }

                XmlAttribute itemOf1 = xmlNodes3.Attributes["Name"];
                if (itemOf1 == null)
                {
                    continue;
                }

                XmlNode xmlNodes4 =
                    xmlDocument.DocumentElement.SelectSingleNode(
                        string.Concat("./Field[@Name=\"", itemOf1.Value, "\"]"));
                if (xmlNodes4 == null)
                {
                    continue;
                }

                xmlNodes.Add(xmlNodes4);
                XmlAttribute xmlAttribute1 = xmlNodes3.Attributes["AddToContentType"];
                if (xmlAttribute1 == null)
                {
                    continue;
                }

                if (contentTypes == null)
                {
                    contentTypes = websService.GetContentTypes();
                }

                this.AddFieldToSiteContentType(xmlAttribute1.Value, contentTypes, xmlNodes4, websService);
            }

            return xmlNodes;
        }

        public string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options)
        {
            return this.AddListItem(sListID, sParentFolder, sListItemXML, attachementNames, attachmentContents,
                listSettingsXml, Options, this.Url, false);
        }

        private string AddListItem(string sListID, string sParentFolder, string sListItemXML, string[] attachementNames,
            byte[][] attachmentContents, string listSettingsXml, AddListItemOptions Options, string sWebUrl,
            bool bIsMeetingWorkSpace)
        {
            string value;
            XmlNode xmlNode = null;
            if (!string.IsNullOrEmpty(listSettingsXml))
            {
                xmlNode = XmlUtility.StringToXmlNode(listSettingsXml);
            }

            if (this.ClientOMAvailable && !bIsMeetingWorkSpace && !this.GetTargetIsMultipleDataList(xmlNode))
            {
                object[] objArray = new object[]
                {
                    sListItemXML, sParentFolder, sListID, attachementNames, attachmentContents, Options, sWebUrl, this
                };
                string str = (string)this.ExecuteClientOMMethod("AddListItem", objArray);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }

            ListsService webServiceForLists = this.GetWebServiceForLists(sWebUrl);
            bool overwrite = Options.Overwrite;
            int num = 0;
            if (Options.ParentID.HasValue)
            {
                num = Options.ParentID.Value;
            }

            bool initialVersion = Options.InitialVersion;
            XmlNode xmlNodes = XmlUtility.StringToXmlNode(sListItemXML);
            if (xmlNodes.Attributes["ID"] != null)
            {
                value = xmlNodes.Attributes["ID"].Value;
            }
            else
            {
                value = null;
            }

            string str1 = value;
            int num1 = -1;
            int.TryParse(str1, out num1);
            bool flag = (initialVersion ? true : string.IsNullOrEmpty(str1));
            if (!initialVersion && !string.IsNullOrEmpty(str1) && this.ListItemQuery(sListID, ListType.Unknown,
                    "ows_ID", "Counter", str1, webServiceForLists) == null)
            {
                flag = true;
            }

            return this.UpdateListItemData(sListID, sParentFolder, num1, xmlNodes, attachementNames, attachmentContents,
                xmlNode, Options, sWebUrl, flag, num, webServiceForLists);
        }

        public void AddListItemAttachments(string[] attachmentNames, byte[][] attachmentContents, string sListID,
            string sID)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists(this.Url);
            this.AddListItemAttachments(attachmentNames, attachmentContents, sListID, sID, webServiceForLists);
        }

        public void AddListItemAttachments(string[] attachmentNames, byte[][] attachmentContents, string sListID,
            string sID, ListsService listsSvs)
        {
            try
            {
                if (attachmentNames != null && (int)attachmentNames.Length > 0)
                {
                    XmlNode attachmentCollection = listsSvs.GetAttachmentCollection(sListID, sID);
                    for (int i = 0; i < (int)attachmentNames.Length; i++)
                    {
                        foreach (XmlNode xmlNodes in attachmentCollection.SelectNodes(
                                     ".//*[local-name() = 'Attachment']"))
                        {
                            if (attachmentNames[i] !=
                                xmlNodes.InnerText.Substring(xmlNodes.InnerText.LastIndexOf("/") + 1))
                            {
                                continue;
                            }

                            listsSvs.DeleteAttachment(sListID, sID, xmlNodes.InnerText);
                            break;
                        }

                        listsSvs.AddAttachment(sListID, sID, attachmentNames[i], attachmentContents[i]);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Error adding attachments: ", exception.Message));
            }
        }

        private void AddListTitlesToLookups(XmlNode xmlFieldNode)
        {
            if (xmlFieldNode.Attributes["Type"].Value != "Lookup" &&
                xmlFieldNode.Attributes["Type"].Value != "LookupMulti")
            {
                return;
            }

            if (xmlFieldNode.Attributes["Hidden"] != null)
            {
                return;
            }

            if (xmlFieldNode.Attributes["List"] == null)
            {
                return;
            }

            string str =
                (xmlFieldNode.Attributes["List"] != null ? xmlFieldNode.Attributes["List"].Value : string.Empty);
            if (!Utils.IsGuid(str))
            {
                return;
            }

            try
            {
                string str1 = null;
                string str2 = null;
                string serverRelative = null;
                string nameFromURL = null;
                if (xmlFieldNode.Attributes["WebId"] == null || !Utils.IsGuid(xmlFieldNode.Attributes["WebId"].Value))
                {
                    XmlNode list = this.GetWebServiceForLists().GetList(str);
                    this.GetDirListName(list.Attributes, out str2, out str1);
                    XmlAttribute itemOf = xmlFieldNode.Attributes["WebId"];
                    if (itemOf != null)
                    {
                        xmlFieldNode.Attributes.Remove(itemOf);
                    }
                }
                else
                {
                    Guid guid = new Guid(xmlFieldNode.Attributes["WebId"].Value);
                    string str3 = null;
                    string item = null;
                    string url = this.Url;
                    XmlDocument xmlDocument = new XmlDocument();
                    while (item == null && url.Length > 7)
                    {
                        if (!this.GlobalWebGuidDictionary.ContainsKey(guid))
                        {
                            SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(url);
                            string content = webServiceForSiteData.GetContent(ObjectType.Site, null, null, null, false,
                                false, ref str3);
                            xmlDocument.LoadXml(content);
                            XmlNode xmlNodes = xmlDocument.SelectSingleNode("./Web/Metadata");
                            if (xmlNodes != null)
                            {
                                Guid guid1 = new Guid(xmlNodes.Attributes["ID"].Value);
                                string value = xmlNodes.Attributes["URL"].Value;
                                if (!this.GlobalWebGuidDictionary.ContainsKey(guid1))
                                {
                                    this.GlobalWebGuidDictionary.Add(guid1, value);
                                }

                                if (guid1 == guid)
                                {
                                    item = value;
                                }
                            }
                        }
                        else
                        {
                            item = this.GlobalWebGuidDictionary[guid];
                        }

                        url = url.Substring(0, url.LastIndexOf("/"));
                    }

                    if (item != null)
                    {
                        StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, item);
                        serverRelative = standardizedUrl.ServerRelative;
                        nameFromURL = Utils.GetNameFromURL(standardizedUrl.ServerRelative);
                        XmlNode list1 = this.GetWebServiceForLists(item).GetList(str);
                        this.GetDirListName(list1.Attributes, out str2, out str1);
                    }
                }

                if (serverRelative != null)
                {
                    XmlAttribute xmlAttribute = xmlFieldNode.OwnerDocument.CreateAttribute("TargetWebSRURL");
                    xmlAttribute.Value = serverRelative;
                    xmlFieldNode.Attributes.Append(xmlAttribute);
                }

                if (nameFromURL != null)
                {
                    XmlAttribute xmlAttribute1 = xmlFieldNode.OwnerDocument.CreateAttribute("TargetWebName");
                    xmlAttribute1.Value = nameFromURL;
                    xmlFieldNode.Attributes.Append(xmlAttribute1);
                }

                if (str1 != null)
                {
                    XmlAttribute xmlAttribute2 = xmlFieldNode.OwnerDocument.CreateAttribute("TargetListName");
                    xmlAttribute2.Value = str1;
                    xmlFieldNode.Attributes.Append(xmlAttribute2);
                }
            }
            catch
            {
            }
        }

        private void AddMajorVersionOfCurrentDraftVersion(VersionDataTrackingTable versionResults,
            List<DataRow> limitedVersions, int lastVersionIndex)
        {
            if (limitedVersions.Count > 0)
            {
                DataRow item = limitedVersions[limitedVersions.Count - 1];
                int num = 0;
                int.TryParse(item["_Level"].ToString(), out num);
                if (num == 2)
                {
                    string str = item["_UIVersionString"].ToString();
                    if (!string.IsNullOrEmpty(str) && !str.EndsWith(".0"))
                    {
                        int num1 = str.IndexOf(".");
                        string str1 = string.Format("{0}.{1}", str.Substring(0, num1), 0);
                        for (int i = lastVersionIndex; i >= 0; i--)
                        {
                            DataRow dataRow = versionResults.Rows[i];
                            if (dataRow["_UIVersionString"].ToString() == str1)
                            {
                                limitedVersions.Add(dataRow);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private bool AddMeetingInstance(XmlNode meetingInstance, ref int nextAvailableID,
            Dictionary<string, string> ListTitleToIDMap, string meetingSeriesListID,
            ref string meetingSeriesListVersion, ListsService listsWebService, MeetingsService meetingsService,
            string sWebUrl)
        {
            int num = int.Parse(meetingInstance.Attributes["ID"].Value);
            string str = (meetingInstance.Attributes["Organizer"] == null
                ? ""
                : meetingInstance.Attributes["Organizer"].Value);
            string str1 = (meetingInstance.Attributes["DTStamp"] == null
                ? ""
                : meetingInstance.Attributes["DTStamp"].Value);
            string str2 = (meetingInstance.Attributes["Title"] == null
                ? ""
                : meetingInstance.Attributes["Title"].Value);
            string str3 = (meetingInstance.Attributes["Location"] == null
                ? ""
                : meetingInstance.Attributes["Location"].Value);
            string str4 = (meetingInstance.Attributes["EventDate"] == null
                ? ""
                : meetingInstance.Attributes["EventDate"].Value);
            string str5 = (meetingInstance.Attributes["EndDate"] == null
                ? ""
                : meetingInstance.Attributes["EndDate"].Value);
            DateTime utcNow = DateTime.UtcNow;
            try
            {
                utcNow = Utils.ParseDateAsUtc(str1);
            }
            catch
            {
                utcNow = DateTime.UtcNow;
            }

            DateTime dateTime = DateTime.UtcNow;
            try
            {
                dateTime = Utils.ParseDateAsUtc(str4);
            }
            catch
            {
                dateTime = DateTime.UtcNow;
            }

            DateTime utcNow1 = DateTime.UtcNow;
            try
            {
                utcNow1 = Utils.ParseDateAsUtc(str5);
            }
            catch
            {
                utcNow1 = DateTime.UtcNow;
            }

            string str6 = null;
            str6 = (meetingInstance.Attributes["EventUID"] == null
                ? this.ReCombineEventUID(meetingInstance, ListTitleToIDMap)
                : meetingInstance.Attributes["EventUID"].Value);
            if (nextAvailableID < num)
            {
                for (int i = nextAvailableID; i < num; i++)
                {
                    int num1 = this.CreateMeetingInstance(meetingsService, str, str6, utcNow, str2, str3, dateTime,
                        utcNow1);
                    this.DeleteItem(meetingSeriesListID, num1, sWebUrl);
                    i = num1;
                    if (i >= num)
                    {
                        return false;
                    }
                }
            }

            int num2 = this.CreateMeetingInstance(meetingsService, str, str6, utcNow, str2, str3, dateTime, utcNow1);
            meetingSeriesListVersion = null;
            nextAvailableID = num2 + 1;
            if (num2 != num)
            {
                this.DeleteItem(meetingSeriesListID, num2, sWebUrl);
                return false;
            }

            this.AddListItem(meetingSeriesListID, "", meetingInstance.OuterXml, null, null, null,
                new AddListItemOptions(), sWebUrl, true);
            return true;
        }

        private void AddMeetingInstances(XmlNode meetingInstanceXML, string sWebURL)
        {
            XmlNodeList xmlNodeLists = meetingInstanceXML.SelectNodes(".//MeetingInstance");
            if (xmlNodeLists.Count == 0)
            {
                return;
            }

            ListsService webServiceForLists = this.GetWebServiceForLists(sWebURL);
            XmlNodeList xmlNodeLists1 = XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(),
                ".//sp:List[@ServerTemplate='200']");
            if (xmlNodeLists1.Count != 1)
            {
                return;
            }

            XmlNode itemOf = xmlNodeLists1[0];
            string value = itemOf.Attributes["ID"].Value;
            string str = itemOf.Attributes["Version"].Value;
            int num = 1;
            int num1 = 0;
            string listItemIDs = this.GetListItemIDs(value, null, true,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, sWebURL);
            if (listItemIDs != null)
            {
                char[] chrArray = new char[] { ',' };
                string[] strArrays = listItemIDs.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str1 = strArrays[i];
                    int num2 = 0;
                    if (int.TryParse(str1, out num2))
                    {
                        if (num < num2)
                        {
                            num = num2;
                        }

                        num1++;
                    }
                }
            }

            XmlNode xmlNodes = null;
            foreach (XmlNode xmlNodes1 in xmlNodeLists)
            {
                if (xmlNodes1.Attributes["RecurrenceData"] == null ||
                    string.IsNullOrEmpty(xmlNodes1.Attributes["RecurrenceData"].Value))
                {
                    continue;
                }

                xmlNodes = xmlNodes1;
                break;
            }

            if (xmlNodes != null && num1 > 0)
            {
                return;
            }

            Dictionary<string, string> parentWebListIDs =
                this.GetParentWebListIDs(sWebURL) ?? new Dictionary<string, string>();
            MeetingsService webServiceForMeetings = this.GetWebServiceForMeetings(sWebURL);
            if (xmlNodes != null)
            {
                this.CreateRecurringMeeting(value, xmlNodes, parentWebListIDs, webServiceForMeetings, sWebURL);
                string str2 = null;
                DateTime now = DateTime.Now;
                List<string> strs = new List<string>();
                foreach (XmlNode xmlNodes2 in xmlNodeLists)
                {
                    if (xmlNodes2 == xmlNodes)
                    {
                        continue;
                    }

                    XmlAttribute xmlAttribute = xmlNodes2.Attributes["EventType"];
                    if (xmlAttribute != null && xmlAttribute.Value == "3")
                    {
                        if (str2 == null)
                        {
                            this.GetRecurringMeetingUIDAndCreateDate(value, itemOf, out str2, out now);
                            if (str2 == null)
                            {
                                throw new Exception(
                                    "Could not located newly created recurring event in meeting workspace.");
                            }
                        }

                        this.DeleteRecurringMeetingInstance(webServiceForMeetings, xmlNodes2, str2, now);
                    }

                    strs.Add(xmlNodes2.Attributes["InstanceID"].Value);
                }

                string str3 = null;
                string str4 = null;
                this.GetAttendeesList(webServiceForLists, out str3, out str4);
                this.CreateItemsAtInstanceIDs(str3, str4, strs.ToArray(), webServiceForLists);
                this.ClearAttendeesList(sWebURL, null, webServiceForLists);
            }
            else
            {
                List<int> nums = new List<int>();
                foreach (XmlNode xmlNodes3 in xmlNodeLists)
                {
                    int num3 = int.Parse(xmlNodes3.Attributes["ID"].Value);
                    if (num3 < num || !this.AddMeetingInstance(xmlNodes3, ref num, parentWebListIDs, value, ref str,
                            webServiceForLists, webServiceForMeetings, sWebURL))
                    {
                        continue;
                    }

                    nums.Add(num3);
                }

                if (nums.Count > 0)
                {
                    this.ClearAttendeesList(sWebURL, nums.ToArray(), webServiceForLists);
                    return;
                }
            }
        }

        public void AddMeetingWorkspacePage(string sWebRelativePath, bool bOverwrite,
            ISharePointWriter callingAdapter = null)
        {
            ISharePointWriter sharePointWriter;
            if (callingAdapter == null)
            {
                sharePointWriter = this;
            }
            else
            {
                sharePointWriter = callingAdapter;
            }

            callingAdapter = sharePointWriter;
            string str = "move document:6.0.2.5530";
            string str1 = string.Concat(this.Server, this.ServerRelativeUrl);
            string str2 =
                "method={0}&service_name={1}&oldUrl={2}&newUrl={3}&url_list=%5b%5d&rename_option=nochangeall&put_option={4}&docopy={5}";
            str = HttpUtility.UrlEncode(str);
            str1 = HttpUtility.UrlEncode(str1);
            string str3 = str2;
            object[] objArray = new object[] { str, str1, "default.aspx", sWebRelativePath, null, null };
            objArray[4] = (bOverwrite ? "overwrite" : "edit");
            objArray[5] = "true";
            str2 = string.Format(str3, objArray);
            str2 = str2.Replace(".", "%2e");
            str2 = str2.Replace("_", "%5f");
            byte[] bytes = Encoding.UTF8.GetBytes(str2);
            byte[] numArray = new byte[(int)bytes.Length + 1];
            bytes.CopyTo(numArray, 0);
            numArray[(int)bytes.Length] = 10;
            CookieAwareWebClient webClient = null;
            try
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.AquireCookieLock();
                }

                try
                {
                    if (base.HasActiveCookieManager)
                    {
                        this.CookieManager.UpdateCookie();
                    }

                    webClient = this.GetWebClient();
                    webClient.Headers.Add("Content-Type", "application/x-vermeer-urlencoded");
                    webClient.Headers.Add("X-Vermeer-Content-Type", "application/x-vermeer-urlencoded");
                    webClient.UploadData(
                        string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll"), "POST",
                        numArray);
                }
                finally
                {
                    if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                    {
                        this.CookieManager.ReleaseCookieLock();
                    }
                }
            }
            catch (Exception exception)
            {
            }

            string str4 = Utils.JoinUrl(this.ServerRelativeUrl, sWebRelativePath);
            callingAdapter.DeleteWebParts(str4);
        }

        private void AddNumberOfLatestVersionsIntoList(int numberOfLatestVersions,
            VersionDataTrackingTable versionResults, List<DataRow> limitedVersions, ref int versionsAdded,
            ref int lastVersionIndex, ref int fileLevel)
        {
            for (int i = versionResults.Rows.Count - 1; i >= 0; i--)
            {
                if (versionsAdded >= numberOfLatestVersions)
                {
                    lastVersionIndex = i;
                    return;
                }

                DataRow item = versionResults.Rows[i];
                limitedVersions.Add(item);
                fileLevel = 0;
                int.TryParse(item["_Level"].ToString(), out fileLevel);
                if (fileLevel != 255)
                {
                    versionsAdded++;
                }
            }
        }

        public string AddOrUpdate2007Group(string sGroupXml)
        {
            string innerText;
            string str;
            string innerText1;
            UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sGroupXml);
            string value = xmlNode.Attributes["Name"].Value;
            string value1 = xmlNode.Attributes["Description"].Value;
            string str1 = xmlNode.Attributes["Owner"].Value;
            bool upper = xmlNode.Attributes["OwnerIsUser"].Value.ToUpper() == "TRUE";
            if (upper)
            {
                str1 = this.EnsureCorrectUserFormat(str1);
            }

            try
            {
                webServiceForUserGroup.RemoveGroup(value);
            }
            catch
            {
            }

            Exception exception = null;
            try
            {
                webServiceForUserGroup.AddGroup(value, str1, (upper ? "user" : "group"), str1, value1);
            }
            catch (Exception exception6)
            {
                Exception exception1 = exception6;
                string userName = null;
                try
                {
                    string str2 = this.EnsureCorrectUserFormat(this.Credentials.UserName);
                    webServiceForUserGroup.AddGroup(value, str2, "user", str2, value1);
                    userName = this.Credentials.UserName;
                }
                catch (Exception exception5)
                {
                    Exception exception2 = exception5;
                    try
                    {
                        XmlNode userCollectionFromSite = webServiceForUserGroup.GetUserCollectionFromSite();
                        XmlNodeList xmlNodeLists =
                            XmlUtility.RunXPathQuery(userCollectionFromSite.FirstChild, ".//d:User[@ID='1']");
                        if (xmlNodeLists.Count == 0)
                        {
                            xmlNodeLists = XmlUtility.RunXPathQuery(userCollectionFromSite.FirstChild, ".//d:User");
                        }

                        if (xmlNodeLists.Count <= 0)
                        {
                            throw new Exception("No users found in the site collection.");
                        }

                        userName = xmlNodeLists[0].Attributes["LoginName"].Value;
                        webServiceForUserGroup.AddGroup(value, userName, "user", userName, value1);
                    }
                    catch (Exception exception4)
                    {
                        Exception exception3 = exception4;
                        SoapException soapException = exception1 as SoapException;
                        SoapException soapException1 = exception2 as SoapException;
                        SoapException soapException2 = exception3 as SoapException;
                        if (soapException != null)
                        {
                            innerText = soapException.Detail.InnerText;
                        }
                        else
                        {
                            innerText = null;
                        }

                        string str3 = innerText;
                        if (soapException1 != null)
                        {
                            str = soapException1.Detail.InnerText;
                        }
                        else
                        {
                            str = null;
                        }

                        string str4 = str;
                        if (soapException2 != null)
                        {
                            innerText1 = soapException1.Detail.InnerText;
                        }
                        else
                        {
                            innerText1 = null;
                        }

                        string str5 = innerText1;
                        if (str3 != null && str4 != null)
                        {
                            string[] strArrays = new string[]
                            {
                                "Group adding failed:\n  Method 1 - :", str3, "\n  Method2 - ", str4, "\n Method3 -",
                                str5
                            };
                            exception = new Exception(string.Concat(strArrays));
                        }
                    }
                }

                try
                {
                    if (!string.IsNullOrEmpty(userName))
                    {
                        webServiceForUserGroup.RemoveUserFromGroup(value, userName);
                        webServiceForUserGroup.UpdateGroupInfo(value, value, str1, (upper ? "user" : "group"), value1);
                    }
                }
                catch
                {
                }
            }

            if (exception != null)
            {
                throw exception;
            }

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                string str6 = this.EnsureCorrectUserFormat(childNode.Attributes["Login"].Value);
                try
                {
                    webServiceForUserGroup.AddUserToGroup(value, str6, str6, "", "");
                }
                catch
                {
                }
            }

            this.BuildGroupMap();
            XmlNode xmlNodes = XmlUtility.StringToXmlNode(this.GetGroups());
            XmlNode xmlNodes1 =
                XmlUtility.MatchFirstAttributeValue("Name", value, xmlNodes.SelectNodes("/Groups/Group"));
            return xmlNodes1.OuterXml;
        }

        public string AddOrUpdate2010Group(string sGroupXml)
        {
            object[] objArray = new object[] { this, sGroupXml };
            return this.ExecuteClientOMMethod("AddorUpdateGroup", objArray).ToString();
        }

        public string AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
        {
            return null;
        }

        public string AddOrUpdateContentType(string sContentTypeXML, string sParentContentTypeName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sContentTypeXML);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode(".//ContentType");
            if (xmlNodes.Attributes["Name"] == null || string.IsNullOrEmpty(xmlNodes.Attributes["Name"].Value))
            {
                throw new Exception("Cannot add content type. Content type has no name.");
            }

            string value = xmlNodes.Attributes["Name"].Value;
            string str = (xmlNodes.Attributes["DisplayName"] != null
                ? xmlNodes.Attributes["DisplayName"].Value
                : value);
            XmlAttribute itemOf = xmlNodes.Attributes["ID"];
            string value1 = null;
            if (itemOf != null)
            {
                value1 = xmlNodes.Attributes["ID"].Value;
            }

            WebsService webServiceForWebs = this.GetWebServiceForWebs();
            XmlNode contentTypes = webServiceForWebs.GetContentTypes();
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(contentTypes, ".//sp:ContentType");
            XmlNode contentType = null;
            if (!string.IsNullOrEmpty(value1))
            {
                contentType = XmlUtility.MatchFirstAttributeValue("ID", value1, xmlNodeLists);
            }

            if (contentType == null)
            {
                contentType = XmlUtility.MatchFirstAttributeValue("Name", value, xmlNodeLists);
            }

            if (contentType != null)
            {
                contentType = webServiceForWebs.GetContentType(contentType.Attributes["ID"].Value);
            }
            else
            {
                XmlNodeList xmlNodeLists1 = XmlUtility.RunXPathQuery(contentTypes, ".//sp:ContentType");
                XmlNode xmlNodes1 = XmlUtility.MatchFirstAttributeValue("Name", sParentContentTypeName, xmlNodeLists1);
                if (xmlNodes1 == null)
                {
                    if (itemOf == null)
                    {
                        throw new Exception(
                            "Failed to add content type: Parent content type could not be located and no ID was provided.");
                    }

                    int length = 0;
                    foreach (XmlNode xmlNodes2 in XmlUtility.RunXPathQuery(contentTypes, ".//sp:ContentType"))
                    {
                        if (xmlNodes2.Attributes["ID"] == null ||
                            !value1.StartsWith(xmlNodes2.Attributes["ID"].Value) ||
                            xmlNodes2.Attributes["ID"].Value.Length <= length)
                        {
                            continue;
                        }

                        xmlNodes1 = xmlNodes2;
                        length = xmlNodes2.Attributes["ID"].Value.Length;
                    }

                    if (xmlNodes1 == null)
                    {
                        throw new Exception(
                            "Failed to add content type: Parent content type could not be located and no closest match to an existing content type could be found.");
                    }
                }

                XmlDocument xmlDocument1 = new XmlDocument();
                xmlDocument1.LoadXml("<FieldRefs />");
                string str1 = webServiceForWebs.CreateContentType(str, xmlNodes1.Attributes["ID"].Value,
                    xmlDocument1.DocumentElement, xmlNodes);
                contentType = webServiceForWebs.GetContentType(str1);
            }

            List<XmlNode> xmlNodes3 = new List<XmlNode>();
            List<XmlNode> xmlNodes4 = new List<XmlNode>();
            List<XmlNode> xmlNodes5 = new List<XmlNode>();
            List<XmlNode> xmlNodes6 = new List<XmlNode>();
            List<string> strs = new List<string>();
            XmlNodeList xmlNodeLists2 = this.FetchSiteColumns(webServiceForWebs);
            XmlNodeList xmlNodeLists3 = XmlUtility.RunXPathQuery(contentType, ".//sp:Field");
            XmlNodeList xmlNodeLists4 = XmlUtility.RunXPathQuery(xmlNodes, ".//Field");
            foreach (XmlNode xmlNodes7 in XmlUtility.RunXPathQuery(xmlNodes, ".//FieldRef"))
            {
                if (xmlNodes7.Attributes["Name"] == null)
                {
                    continue;
                }

                string value2 = xmlNodes7.Attributes["Name"].Value;
                if (!strs.Contains(value2))
                {
                    strs.Add(value2);
                }

                bool flag = false;
                foreach (XmlNode xmlNodes8 in xmlNodeLists3)
                {
                    if (xmlNodes8.Attributes["Name"] == null || !(xmlNodes8.Attributes["Name"].Value == value2))
                    {
                        continue;
                    }

                    flag = true;
                    this.UpdateColumnSettings(xmlNodes7, xmlNodes8);
                    xmlNodes6.Add(xmlNodes8);
                    break;
                }

                if (flag)
                {
                    continue;
                }

                foreach (XmlNode xmlNodes9 in xmlNodeLists2)
                {
                    if (xmlNodes9.Attributes["Name"] == null || !(xmlNodes9.Attributes["Name"].Value == value2))
                    {
                        continue;
                    }

                    this.UpdateColumnSettings(xmlNodes7, xmlNodes9);
                    xmlNodes4.Add(xmlNodes9);
                }
            }

            foreach (XmlNode xmlNodes10 in xmlNodeLists4)
            {
                if (xmlNodes10.Attributes["Name"] == null)
                {
                    continue;
                }

                string str2 = xmlNodes10.Attributes["Name"].Value;
                if (!strs.Contains(str2))
                {
                    strs.Add(str2);
                }

                bool flag1 = false;
                foreach (XmlNode xmlNodes11 in xmlNodeLists3)
                {
                    if (xmlNodes11.Attributes["Name"] == null || !(xmlNodes11.Attributes["Name"].Value == str2))
                    {
                        continue;
                    }

                    flag1 = true;
                    break;
                }

                if (flag1)
                {
                    continue;
                }

                bool flag2 = false;
                foreach (XmlNode xmlNodes12 in xmlNodeLists2)
                {
                    if (xmlNodes12.Attributes["Name"] == null || !(xmlNodes12.Attributes["Name"].Value == str2))
                    {
                        continue;
                    }

                    flag2 = true;
                    xmlNodes4.Add(xmlNodes12);
                    break;
                }

                if (flag2)
                {
                    continue;
                }

                xmlNodes3.Add(xmlNodes10);
                xmlNodes4.Add(xmlNodes10);
            }

            foreach (XmlNode xmlNodes13 in xmlNodeLists3)
            {
                if (xmlNodes13.Attributes["Name"] == null || strs.Contains(xmlNodes13.Attributes["Name"].Value))
                {
                    continue;
                }

                xmlNodes5.Add(xmlNodes13);
            }

            if (xmlNodes3.Count > 0)
            {
                this.AddSiteColumns(webServiceForWebs, xmlNodes3);
                xmlNodeLists2 = this.FetchSiteColumns(webServiceForWebs);
                foreach (XmlNode xmlNode in xmlNodes4)
                {
                    string value3 = xmlNode.Attributes["Name"].Value;
                    foreach (XmlNode xmlNodes14 in xmlNodeLists2)
                    {
                        if (xmlNodes14.Attributes["Name"] == null || xmlNodes14.Attributes["ID"] == null ||
                            !(xmlNodes14.Attributes["Name"].Value == value3))
                        {
                            continue;
                        }

                        XmlAttribute xmlAttribute = xmlNode.Attributes["ID"];
                        if (xmlAttribute == null)
                        {
                            xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("ID");
                            xmlNode.Attributes.Append(xmlAttribute);
                        }

                        xmlAttribute.Value = xmlNodes14.Attributes["ID"].Value;
                    }
                }
            }

            XmlDocument xmlDocument2 = this.GetXmlDocument(xmlNodes4);
            XmlDocument xmlDocument3 = this.GetXmlDocument(xmlNodes5);
            XmlDocument xmlDocument4 = this.GetXmlDocument(xmlNodes6);
            webServiceForWebs.UpdateContentType(contentType.Attributes["ID"].Value, xmlNodes,
                xmlDocument2.DocumentElement, xmlDocument4.DocumentElement, xmlDocument3.DocumentElement);
            XmlNode contentType1 = webServiceForWebs.GetContentType(contentType.Attributes["ID"].Value);
            this.m_ContentTypeDict = null;
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            this.WriteContentTypeXml(null, xmlTextWriter, new Hashtable(), contentType1);
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public string AddOrUpdateGroup(string sGroupXml)
        {
            string str = null;
            str = (!base.SharePointVersion.IsSharePoint2007OrEarlier
                ? this.AddOrUpdate2010Group(sGroupXml)
                : this.AddOrUpdate2007Group(sGroupXml));
            return str;
        }

        public string AddOrUpdateRole(string sName, string sDescription, long lPermissionMask)
        {
            this.BreakRoleDefinitionInheritance();
            UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
            try
            {
                webServiceForUserGroup.GetRoleInfo(sName);
                webServiceForUserGroup.UpdateRoleDefInfo(sName, sName, sDescription, (ulong)lPermissionMask);
            }
            catch
            {
                webServiceForUserGroup.AddRoleDef(sName, sDescription, (ulong)lPermissionMask);
            }

            XmlNode rolesAndPermissionsForSite = webServiceForUserGroup.GetRolesAndPermissionsForSite();
            XmlNamespaceManager xmlNamespaceManagers =
                new XmlNamespaceManager(rolesAndPermissionsForSite.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("SP", "http://schemas.microsoft.com/sharepoint/soap/directory/");
            XmlNode xmlNodes = XmlUtility.MatchFirstAttributeValue("Name", sName,
                rolesAndPermissionsForSite.SelectNodes(".//SP:Role", xmlNamespaceManagers));
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Role");
            if (xmlNodes.Attributes["Name"] != null)
            {
                xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Name"].Value);
            }

            if (xmlNodes.Attributes["Description"] != null)
            {
                xmlTextWriter.WriteAttributeString("Description", xmlNodes.Attributes["Description"].Value);
            }

            if (xmlNodes.Attributes["BasePermissions"] != null)
            {
                xmlTextWriter.WriteAttributeString("PermMask", xmlNodes.Attributes["BasePermissions"].Value);
            }

            if (xmlNodes.Attributes["Hidden"] != null)
            {
                xmlTextWriter.WriteAttributeString("Hidden", xmlNodes.Attributes["Hidden"].Value);
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public string AddReferencedTaxonomyData(string sReferencedTaxonomyXML)
        {
            throw new NotImplementedException();
        }

        public string AddReusedTerms(string sTargetTermStoreGuid, string sParentTermCollectionXML)
        {
            throw new NotImplementedException();
        }

        public string AddRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            if (!bIsGroup)
            {
                sPrincipalName = this.EnsureCorrectUserFormat(sPrincipalName);
            }

            this.BreakRoleInheritance(sListID, iItemId);
            if (!string.IsNullOrEmpty(sListID))
            {
                this.AddRoleAssignmentViaHttpPost(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
            }
            else
            {
                UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
                if (!bIsGroup)
                {
                    webServiceForUserGroup.AddUserToRole(sRoleName, sPrincipalName, sPrincipalName, "", "");
                }
                else
                {
                    webServiceForUserGroup.AddGroupToRole(sRoleName, sPrincipalName);
                }
            }

            return string.Format("<RoleAssignment RoleName=\"{0}\" PrincipalName=\"{1}\" />", sRoleName,
                sPrincipalName);
        }

        private void AddRoleAssignmentViaHttpPost(string sPrincipalName, bool bIsGroup, string sRoleName,
            string sListID, int iItemId)
        {
            string permissionsUrl = this.GetPermissionsUrl(sListID, iItemId, "aclinv");
            XmlDocument xmlDocument = new XmlDocument();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            string value = null;
            string str = this.HttpGet(permissionsUrl);
            string str1 = str.Replace("/><", "/>\n<").Replace("</label><br", "</label>\n<br");
            string str2 = (base.SharePointVersion.IsSharePoint2010OrLater
                ? "ctl00$PlaceHolderMain$ctl01$RptControls$btnOK"
                : "ctl00$PlaceHolderMain$ctl02$RptControls$btnOK");
            Regex regex =
                new Regex(string.Format("<label for=\"ctl00_PlaceHolderMain_{0}_ctl00_cblRoles_[0-9]+\".*/label>",
                    (base.SharePointVersion.IsSharePoint2010OrLater ? "IfsGivePermissions" : "ctl01")));
            foreach (Match match in regex.Matches(str1))
            {
                xmlDocument.LoadXml(match.Value);
                string innerText = xmlDocument.FirstChild.InnerText;
                if (innerText.Substring(0, innerText.IndexOf('-')).Trim() != sRoleName)
                {
                    continue;
                }

                value = xmlDocument.FirstChild.Attributes["for"].Value;
                break;
            }

            strs.Add("__EVENTTARGET", str2);
            strs.Add("__EVENTARGUMENT", "");
            strs.Add("ctl00$PlaceHolderMain$ctl01$ctl00$AddToGroupOrRole", "RadAddToRole");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$hiddenSpanData", sPrincipalName);
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$OriginalEntities", "<Entities />");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$HiddenEntityKey", "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$HiddenEntityDisplayText", "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$downlevelTextBox", "");
            strs.Add(value.Replace("_", "$"), "on");
            string sharePointHttpPostParameters = this.GetSharePointHttpPostParameters(str, "<input.*__.*/>", strs);
            this.HttpPost(permissionsUrl, sharePointHttpPostParameters);
        }

        public string AddSiteCollection(string sWebApp, string sSiteCollectionXML, AddSiteCollectionOptions addOptions)
        {
            throw new NotImplementedException();
        }

        private void AddSiteColumns(WebsService websService, List<XmlNode> siteColumnsToCreate)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            XmlNode xmlNode = XmlUtility.StringToXmlNode(this.GetLists());
            xmlTextWriter.WriteStartElement("Fields");
            int num = 1;
            foreach (XmlNode xmlNodes in siteColumnsToCreate)
            {
                xmlTextWriter.WriteStartElement("Method");
                xmlTextWriter.WriteAttributeString("ID", num.ToString());
                if (xmlNodes.Attributes["Type"].Value == "Lookup" || xmlNodes.Attributes["Type"].Value == "LookupMulti")
                {
                    this.UpdateFieldLookupListNames(xmlNodes, xmlNode);
                }

                xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                xmlTextWriter.WriteEndElement();
                num++;
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            websService.UpdateColumns(xmlDocument.DocumentElement, null, null);
        }

        public string AddSiteUser(string sUserXML, AddUserOptions options)
        {
            string str;
            string value;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sUserXML);
            XmlNode firstChild = xmlDocument.FirstChild;
            if (firstChild.Attributes["LoginName"] != null)
            {
                value = firstChild.Attributes["LoginName"].Value;
            }
            else
            {
                value = null;
            }

            string str1 = value;
            if (firstChild.Attributes["Name"] != null)
            {
                string value1 = firstChild.Attributes["Name"].Value;
            }

            if (firstChild.Attributes["Email"] != null)
            {
                string value2 = firstChild.Attributes["Email"].Value;
            }

            if (firstChild.Attributes["Notes"] != null)
            {
                string str2 = firstChild.Attributes["Notes"].Value;
            }

            if (this.m_ReverseUserMap.ContainsKey(str1.ToLowerInvariant()))
            {
                return sUserXML;
            }

            string str3 = string.Concat(this.SiteCollectionUrl, "/_layouts/aclinv.aspx");
            XmlDocument xmlDocument1 = new XmlDocument();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            string str4 = this.HttpGet(str3);
            str4.Replace("/><", "/>\n<").Replace("</label><br", "</label>\n<br");
            string str5 = (base.SharePointVersion.IsSharePoint2010OrLater
                ? "ctl00$PlaceHolderMain$ctl01$RptControls$btnOK"
                : "ctl00$PlaceHolderMain$ctl02$RptControls$btnOK");
            string str6 = (base.SharePointVersion.IsSharePoint2010OrLater
                ? "ctl00$PlaceHolderMain$IfsGivePermissions$ctl00"
                : "ctl00$PlaceHolderMain$ctl01$ctl00");
            strs.Add("__EVENTTARGET", str5);
            strs.Add("__EVENTARGUMENT", "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$hiddenSpanData", str1 ?? "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$OriginalEntities", "<Entities />");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$HiddenEntityKey", "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$HiddenEntityDisplayText", "");
            strs.Add("ctl00$PlaceHolderMain$ctl00$ctl01$userPicker$downlevelTextBox", "");
            strs.Add(string.Concat(str6, "$AddToGroupOrRole"), "RadAddToRole");
            strs.Add(string.Concat(str6, "$cblRoles$0"), "on");
            string sharePointHttpPostParameters = this.GetSharePointHttpPostParameters(str4, "<input.*__.*/>", strs);
            this.HttpPost(str3, sharePointHttpPostParameters);
            string siteUsers = this.GetSiteUsers();
            try
            {
                string dFromUser = this.GetIDFromUser(str1);
                if (string.IsNullOrEmpty(dFromUser))
                {
                    throw new Exception("Failed to locate new user in user collection.");
                }

                str3 = string.Concat(this.SiteCollectionUrl, "/_layouts/user.aspx");
                strs.Clear();
                strs.Add("ctl00$PlaceHolderMain$hdnPrincipalsToDelete", dFromUser);
                strs.Add("ctl00$PlaceHolderMain$hdnOperation", "deleteUser");
                sharePointHttpPostParameters =
                    this.GetSharePointHttpPostParameters(this.HttpGet(str3), "<input.*/>", strs);
                this.HttpPost(str3, sharePointHttpPostParameters);
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(siteUsers);
                XmlNode xmlNodes =
                    xmlDocument2.DocumentElement.SelectSingleNode(string.Concat(".//User[@ID='", dFromUser, "']"));
                str = (xmlNodes == null ? sUserXML : xmlNodes.OuterXml);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string[] message = new string[]
                {
                    "A problem occured while adding the user: ", exception.Message,
                    "\nThis is expected behaviour if the user no longer exists in the authentication provider.\nIf that is not the case, you may wish to confirm that no unexpected permissions have been granted to ",
                    str1, " on ", this.SiteCollectionUrl
                };
                throw new Exception(string.Concat(message));
            }

            return str;
        }

        public string AddTerm(string termXml)
        {
            throw new NotImplementedException();
        }

        public string AddTermGroup(string targetTermStoreGuid, string termGroupXml, bool includeGroupXmlInResult)
        {
            throw new NotImplementedException();
        }

        public string AddTermSet(string termSetXml)
        {
            throw new NotImplementedException();
        }

        public string AddTermstoreLanguages(string sTargetTermStoreGuid, string sLangaugesXML)
        {
            throw new NotImplementedException();
        }

        public string AddView(string sListID, string sViewXML)
        {
            sViewXML = string.Concat("<Views>", sViewXML, "</Views>");
            XmlNode xmlNode = XmlUtility.StringToXmlNode(this.GetList(sListID));
            XmlNode xmlNodes = XmlUtility.StringToXmlNode(sViewXML);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlNode.OuterXml);
            XmlNode xmlNodes1 = xmlDocument.ImportNode(xmlNodes, true);
            XmlNode xmlNodes2 = xmlDocument.SelectSingleNode("//Views");
            if (xmlNodes2 != null)
            {
                xmlDocument.DocumentElement.RemoveChild(xmlNodes2);
                xmlDocument.DocumentElement.AppendChild(xmlNodes1);
            }

            ListsService webServiceForLists = this.GetWebServiceForLists();
            this.AddViewsToList(sListID, null, xmlDocument.FirstChild, null, null, false, webServiceForLists);
            ViewsService webServiceForViews = this.GetWebServiceForViews();
            XmlNode viewHtml = webServiceForViews.GetViewHtml(sListID, xmlNodes.ChildNodes[0].Attributes["Name"].Value);
            return viewHtml.OuterXml;
        }

        private void AddViewsToList(string sListID, string sViewXml, XmlNode listXML, XmlNode listProperties,
            XmlNode newFields, bool bDeletePreExistingViews, ListsService listsSvs)
        {
            XmlNode xmlNodes;
            string value;
            ViewsService webServiceForViews = this.GetWebServiceForViews();
            XmlNodeList xmlNodeLists =
                XmlUtility.RunXPathQuery(webServiceForViews.GetViewCollection(sListID), "//sp:View");
            XmlNodeList xmlNodeLists1 = null;
            xmlNodeLists1 = (sViewXml == null
                ? listXML.SelectNodes("//View")
                : XmlUtility.StringToXmlNode(sViewXml).SelectNodes("//View"));
            if (xmlNodeLists1.Count == 0)
            {
                return;
            }

            List<string> strs = new List<string>();
            if (bDeletePreExistingViews)
            {
                bool flag = false;
                foreach (XmlNode xmlNodes1 in xmlNodeLists1)
                {
                    XmlAttribute itemOf = xmlNodes1.Attributes["DefaultView"];
                    if (itemOf == null || !itemOf.Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    XmlNode itemOf1 = xmlNodeLists1[0];
                    XmlAttribute xmlAttribute = itemOf1.OwnerDocument.CreateAttribute("DefaultView");
                    xmlAttribute.Value = "TRUE";
                    itemOf1.Attributes.Append(xmlAttribute);
                }

                foreach (XmlNode xmlNodes2 in xmlNodeLists)
                {
                    XmlAttribute xmlAttribute1 = xmlNodes2.Attributes["Name"];
                    XmlAttribute itemOf2 = xmlNodes2.Attributes["Hidden"];
                    XmlAttribute xmlAttribute2 = xmlNodes2.Attributes["DisplayName"];
                    if (xmlAttribute1 == null ||
                        itemOf2 != null && itemOf2.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                        xmlAttribute2 == null || string.IsNullOrEmpty(xmlAttribute2.Value))
                    {
                        continue;
                    }

                    strs.Add(xmlAttribute1.Value);
                }
            }

            int num = 0;
            foreach (XmlNode xmlNodes3 in xmlNodeLists1)
            {
                if (string.IsNullOrEmpty(xmlNodes3.Attributes["DisplayName"].Value) ||
                    xmlNodes3.Attributes["Hidden"] != null && string.Equals(xmlNodes3.Attributes["Hidden"].Value,
                        "true", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string str = xmlNodes3.Attributes["Type"].Value;
                if (str.Equals("Table", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool flag1 = str.Equals("GANTT", StringComparison.OrdinalIgnoreCase);
                bool flag2 = (xmlNodes3.Attributes["ContentTypeID"] == null
                    ? false
                    : xmlNodes3.Attributes["ContentTypeID"].Value != "0x");
                string value1 = xmlNodes3.Attributes["DisplayName"].Value;
                string str1 = xmlNodes3.Attributes["Url"].Value;
                string str2 = str1.Substring(str1.LastIndexOf('/') + 1);
                str2 = str2.Substring(0, str2.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase));
                XmlNode xmlNodes4 = null;
                foreach (XmlNode xmlNodes5 in xmlNodeLists)
                {
                    string value2 = xmlNodes5.Attributes["Url"].Value;
                    string str3 = value2.Substring(value2.LastIndexOf('/') + 1);
                    str3 = str3.Substring(0, str3.IndexOf(".aspx", StringComparison.OrdinalIgnoreCase));
                    if (!str2.Equals(str3, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    xmlNodes4 = xmlNodes5;
                    if (!bDeletePreExistingViews)
                    {
                        break;
                    }

                    XmlAttribute itemOf3 = xmlNodes4.Attributes["Name"];
                    if (itemOf3 == null)
                    {
                        break;
                    }

                    strs.Remove(itemOf3.Value);
                    break;
                }

                if (xmlNodes4 == null && flag1 && !this.IsClientOM)
                {
                    continue;
                }

                if (xmlNodes4 != null && !flag1 && base.SharePointVersion.IsSharePoint2007OrLater &&
                    (listXML.Attributes["BaseTemplate"].Value == "101" && listXML.Attributes["BaseType"].Value == "1" ||
                     listXML.Attributes["BaseType"].Value == "3"))
                {
                    string innerText = xmlNodes4.Attributes["Name"].InnerText;
                    XmlNode view = webServiceForViews.GetView(sListID, innerText);
                    XmlNode xmlNodes6 = xmlNodes3.SelectSingleNode("./ViewFields");
                    if (xmlNodes6 != null && xmlNodes6.HasChildNodes)
                    {
                        foreach (XmlNode value3 in xmlNodes6.SelectNodes("./FieldRef"))
                        {
                            if (!(listXML.Attributes["BaseType"].Value == "3") ||
                                !(value3.Attributes["Name"].Value == "ThreadingNoIndent"))
                            {
                                if (!(listXML.Attributes["BaseTemplate"].Value == "101") ||
                                    !(listXML.Attributes["BaseType"].Value == "1") ||
                                    !(value3.Attributes["Name"].InnerText == "ImageWidth") &&
                                    !(value3.Attributes["Name"].InnerText == "ImageHeight"))
                                {
                                    continue;
                                }

                                foreach (XmlNode xmlNodes7 in XmlUtility.RunXPathQuery(view,
                                             "//sp:ViewFields/sp:FieldRef"))
                                {
                                    if (!xmlNodes7.Attributes["Name"].InnerText.Contains("ImageWidth"))
                                    {
                                        if (!xmlNodes7.Attributes["Name"].InnerText.Contains("ImageHeight"))
                                        {
                                            continue;
                                        }

                                        value3.Attributes["Name"].Value = xmlNodes7.Attributes["Name"].Value;
                                        break;
                                    }
                                    else
                                    {
                                        value3.Attributes["Name"].Value = xmlNodes7.Attributes["Name"].Value;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                value3.Attributes["Name"].Value = "Threading";
                            }
                        }
                    }
                }

                num++;
                XmlNode xmlNode = xmlNodes3.SelectSingleNode("./ViewFields");
                XmlNode xmlNodes8 = xmlNodes3.SelectSingleNode("./ViewFields");
                if (xmlNode == null)
                {
                    xmlNode = XmlUtility.StringToXmlNode("<ViewFields/>");
                }

                XmlNode xmlNode1 = xmlNodes3.SelectSingleNode("./Query");
                if (xmlNode1 != null)
                {
                    XmlNode xmlNodes9 = xmlNode1.SelectSingleNode("./GroupBy");
                    if (xmlNodes9 != null && xmlNodes9.ChildNodes.Count == 0)
                    {
                        xmlNode1.RemoveChild(xmlNodes9);
                    }

                    XmlNode xmlNodes10 = xmlNode1.SelectSingleNode("./OrderBy");
                    if (xmlNodes10 != null && xmlNodes10.ChildNodes.Count == 0)
                    {
                        xmlNode1.RemoveChild(xmlNodes10);
                    }
                }
                else
                {
                    xmlNode1 = XmlUtility.StringToXmlNode("<Query/>");
                }

                XmlNode xmlNodes11 = xmlNodes3.SelectSingleNode("./RowLimit");
                string innerText1 = "FALSE";
                bool flag3 = false;
                XmlAttribute xmlAttribute3 = xmlNodes3.Attributes["DefaultView"];
                if (xmlAttribute3 != null)
                {
                    innerText1 = xmlAttribute3.InnerText;
                }

                if (innerText1 == "TRUE" || innerText1 == "1")
                {
                    flag3 = true;
                }

                List<XmlNode> xmlNodes12 = new List<XmlNode>();
                List<int> nums = new List<int>();
                if (listXML.Attributes["BaseTemplate"].Value == "106" && newFields != null)
                {
                    for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                    {
                        int num1 = 0;
                        while (num1 < newFields.ChildNodes.Count)
                        {
                            if (!(xmlNode.ChildNodes[i].Attributes["Name"].Value == newFields.ChildNodes[num1]
                                    .FirstChild.Attributes["DisplayName"].Value) ||
                                !(newFields.ChildNodes[num1].FirstChild.Attributes["Type"].Value == "Calculated"))
                            {
                                num1++;
                            }
                            else
                            {
                                string str4 = string.Concat("<Fields>", newFields.ChildNodes[num1].OuterXml,
                                    "</Fields>");
                                xmlNodes12.Add(XmlUtility.StringToXmlNode(str4));
                                xmlNode.RemoveChild(xmlNodes8.ChildNodes[i]);
                                nums.Add(i);
                                break;
                            }
                        }
                    }
                }

                try
                {
                    string value4 = (xmlNodes4 == null || xmlNodes4.Attributes["Name"] == null
                        ? value1
                        : xmlNodes4.Attributes["Name"].Value);
                    if (xmlNodes3.Attributes["ContentTypeID"] != null)
                    {
                        value = xmlNodes3.Attributes["ContentTypeID"].Value;
                    }
                    else
                    {
                        value = null;
                    }

                    string str5 = value;
                    bool flag4 = false;
                    if (xmlNodes4 == null)
                    {
                        xmlNodes = (flag1 || flag2 && this.ClientOMAvailable
                            ? this.AddViewThroughClientOM(sListID, str2, xmlNode, xmlNode1, xmlNodes11,
                                xmlNodes3.SelectSingleNode("ViewData"), str, flag3, str5)
                            : webServiceForViews.AddView(sListID, str2, xmlNode, xmlNode1, xmlNodes11, str, flag3));
                        value4 = xmlNodes.Attributes["Name"].Value;
                        flag4 = true;
                    }

                    if (listXML.Attributes["BaseTemplate"].Value == "106")
                    {
                        for (int j = 0; j < xmlNodes12.Count; j++)
                        {
                            xmlNodes12[j].FirstChild.Attributes["AddToView"].Value = value4;
                            listsSvs.UpdateList(sListID, listProperties, xmlNodes12[j], null, null, null);
                        }
                    }

                    XmlNode xmlNodes13 = xmlNodes3.Clone();
                    while (xmlNodes13.HasChildNodes)
                    {
                        xmlNodes13.RemoveChild(xmlNodes13.FirstChild);
                    }

                    XmlNode xmlNodes14 = xmlNodes3.SelectSingleNode("./Aggregations");
                    XmlNode xmlNodes15 = xmlNodes3.SelectSingleNode("./Formats");
                    webServiceForViews.UpdateView(sListID, value4, xmlNodes13, xmlNode1, xmlNode, xmlNodes14,
                        xmlNodes15, xmlNodes11);
                    if (this.ClientOMAvailable && flag2 && !flag4)
                    {
                        this.UpdateViewSettingsThroughClientOM(sListID, value4, str5);
                    }
                }
                catch (SoapException soapException)
                {
                    throw new Exception(RPCUtil.GetSoapError(soapException.Detail));
                }
            }

            if (bDeletePreExistingViews)
            {
                foreach (string str6 in strs)
                {
                    webServiceForViews.DeleteView(sListID, str6);
                }
            }
        }

        private XmlNode AddViewThroughClientOM(string sListID, string sWantedViewUrlName, XmlNode nodeViewFields,
            XmlNode nodeQuery, XmlNode nodeRowLimit, XmlNode viewData, string sType, bool bDefaultView,
            string sContentTypeId)
        {
            object[] objArray = new object[]
            {
                sListID, sWantedViewUrlName, nodeViewFields, nodeQuery, nodeRowLimit, viewData, sType, bDefaultView,
                sContentTypeId, this
            };
            return (XmlNode)this.ExecuteClientOMMethod("AddView", objArray);
        }

        public string AddWeb(string sWebXML, AddWebOptions addOptions)
        {
            int num;
            this.m_SubWebGuidDict = null;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXML);
            string value = xmlNode.Attributes["Name"].Value;
            string str = (xmlNode.Attributes["Title"] == null || xmlNode.Attributes["Title"].Value.Length == 0
                ? value
                : xmlNode.Attributes["Title"].Value);
            string str1 = (xmlNode.Attributes["Description"] != null ? xmlNode.Attributes["Description"].Value : "");
            if (!int.TryParse(xmlNode.Attributes["WebTemplateID"].Value, out num))
            {
                num = -1;
            }

            string value1 = xmlNode.Attributes["WebTemplateConfig"].Value;
            string str2 = string.Concat(this.Server, this.ServerRelativeUrl,
                (this.ServerRelativeUrl.EndsWith("/") ? "" : "/"), value);
            StringWriter stringWriter = new StringWriter(new StringBuilder(512));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            bool flag = false;
            try
            {
                xmlTextWriter.WriteStartElement("Web");
                this.FillWebXML(xmlTextWriter, str2, null, true, true);
                xmlTextWriter.WriteEndElement();
                flag = true;
            }
            catch (Exception exception)
            {
                if (exception.Message.IndexOf("404") == -1)
                {
                    throw;
                }
                else
                {
                    flag = false;
                }
            }

            if (flag && addOptions.Overwrite)
            {
                XmlNode xmlNodes = XmlUtility.StringToXmlNode(stringWriter.ToString());
                string value2 = xmlNodes.Attributes["ServerRelativeUrl"].Value;
                this.DeleteWeb(value2);
                flag = false;
            }

            string value3 = null;
            if (value1 != "-1")
            {
                XmlNode xmlNodes1 = null;
                XmlNode xmlNode1 = XmlUtility.StringToXmlNode(this.GetWebTemplates());
                if (num != -1)
                {
                    xmlNodes1 = xmlNode1.SelectSingleNode(
                        string.Format("//WebTemplate[@ID=\"{0}\"][@Config=\"{1}\"]/@Name", num, value1));
                }
                else
                {
                    value3 = xmlNode.Attributes["WebTemplateName"].Value;
                    xmlNodes1 = xmlNode1.SelectSingleNode(string.Format("//WebTemplate[@Name=\"{0}\"]/@FullName",
                        value3));
                }

                value3 = (xmlNodes1 == null ? "STS#1" : xmlNodes1.Value);
            }

            if (!flag)
            {
                RPCUtil.Create2007Site(this, string.Concat(this.Server, this.ServerRelativeUrl), value, str, str1,
                    value3);
            }

            if (this.ClientOMAvailable && addOptions.PreserveUIVersion)
            {
                object[] objArray = new object[]
                    { string.Concat(this.Server, this.ServerRelativeUrl, "/", value), this };
                string str3 = (string)this.ExecuteClientOMMethod("UpdateUIVersion", objArray);
            }

            if (this.ClientOMAvailable && addOptions.Overwrite && value3 != null && value3.Equals("STS#0"))
            {
                string welcomePage = this.GetWelcomePage(string.Concat(this.Url, "/", value));
                if (string.IsNullOrEmpty(welcomePage))
                {
                    welcomePage = "/SitePages/Home.aspx";
                }

                object[] objArray1 = new object[]
                    { string.Concat(this.Server, this.ServerRelativeUrl, "/", value), welcomePage, this };
                string str4 = (string)this.ExecuteClientOMMethod("ClearTeamSiteWikiField", objArray1);
            }

            stringWriter = new StringWriter(new StringBuilder(512));
            xmlTextWriter = new XmlTextWriter(stringWriter);
            if (this.ClientOMAvailable && num == -1 && value3 != null)
            {
                object[] objArray2 = new object[] { str2, value3, this };
                this.ExecuteClientOMMethod("ApplyWebTemplate", objArray2);
            }

            this.UpdateWebProperties(str2, xmlNode, addOptions, true);
            xmlTextWriter.WriteStartElement("Web");
            this.FillWebXML(xmlTextWriter, str2, null, true, true);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private void AddWebPart(XmlNode webPartNode, string sWebPartPageUrl, WebPartPagesService wppService,
            ref Dictionary<string, string> listTitleToIdMap)
        {
            Guid empty = Guid.Empty;
            this.AddWebPart(webPartNode, sWebPartPageUrl, true, wppService, ref listTitleToIdMap, out empty, false);
        }

        private void AddWebPart(XmlNode webPartNode, string sWebPartPageUrl, bool bCheckoutWebPartPage,
            WebPartPagesService wppService, ref Dictionary<string, string> listTitleToIdMap, out Guid webPartGuid,
            bool bAddToHiddenZone)
        {
            XmlNode xmlNodes;
            string str;
            XmlNode xmlNodes1;
            if (webPartNode == null || string.IsNullOrEmpty(sWebPartPageUrl) || wppService == null)
            {
                throw new ArgumentException("One of the necessary parameters passed for adding a web part is null.");
            }

            string innerText = "DefaultZone";
            XmlNode xmlNodes2 = webPartNode.SelectSingleNode(".//*[name() = 'ZoneID']");
            if (xmlNodes2 != null)
            {
                innerText = xmlNodes2.InnerText;
            }

            int num = 0;
            XmlNode xmlNodes3 = webPartNode.SelectSingleNode(".//*[name() = 'PartOrder']");
            if (xmlNodes3 != null)
            {
                num = Convert.ToInt32(xmlNodes3.InnerText);
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                XmlNode xmlNodes4 = webPartNode.SelectSingleNode(".//*[name() = 'TypeName']");
                if (xmlNodes4 != null && xmlNodes4.InnerText.Equals("Microsoft.SharePoint.WebPartPages.ListViewWebPart",
                        StringComparison.OrdinalIgnoreCase))
                {
                    XmlNode str1 = webPartNode.SelectSingleNode(".//*[name() = 'WebId']");
                    if (str1 != null)
                    {
                        str1.InnerText = Guid.Empty.ToString();
                    }
                }
            }

            string str2 = null;
            XmlNode xmlNodes5 = webPartNode.SelectSingleNode(".//*[name() = 'webPart']");
            if (xmlNodes5 == null)
            {
                xmlNodes = webPartNode.SelectSingleNode(".//*[name() = 'ListName']");
                if (xmlNodes != null)
                {
                    if (listTitleToIdMap == null)
                    {
                        listTitleToIdMap = this.GetListIDTitleMap(false);
                    }

                    bool flag = false;
                    if (Utils.IsGUID(xmlNodes.InnerText))
                    {
                        string str3 = xmlNodes.InnerText.TrimStart(new char[] { '{' });
                        char[] chrArray = new char[] { '}' };
                        if (listTitleToIdMap.ContainsValue(str3.TrimEnd(chrArray)))
                        {
                            flag = true;
                        }
                    }
                    else if (listTitleToIdMap.TryGetValue(xmlNodes.InnerText.ToLower(), out str))
                    {
                        xmlNodes.InnerText = string.Concat("{", str, "}");
                        flag = true;
                        XmlNode lower = webPartNode.SelectSingleNode(".//*[name() = 'ListId']");
                        if (lower != null)
                        {
                            lower.InnerText = str.ToLower();
                        }
                    }

                    if (!flag)
                    {
                        throw new Exception(string.Concat("The list '", xmlNodes.InnerText,
                            "' referenced by a web part on this page could not be found."));
                    }
                }
            }
            else
            {
                xmlNodes = webPartNode.SelectSingleNode(".//*[@name='ListName']");
            }

            xmlNodes1 = (xmlNodes5 == null
                ? webPartNode.SelectSingleNode(".//*[name() = 'ListViewXml']")
                : webPartNode.SelectSingleNode(".//*[@name='XmlDefinition']"));
            if (xmlNodes1 != null)
            {
                xmlNodes1.ParentNode.RemoveChild(xmlNodes1);
            }

            str2 = (xmlNodes5 == null
                ? webPartNode.OuterXml
                : string.Concat("<webParts>", xmlNodes5.OuterXml, "</webParts>"));
            Guid empty = Guid.Empty;
            string str4 = Utils.JoinUrl(this.Server, sWebPartPageUrl);
            bool flag1 = false;
            ListsService webServiceForLists = this.GetWebServiceForLists();
            if (bCheckoutWebPartPage)
            {
                flag1 = webServiceForLists.CheckOutFile(str4, "true", null);
            }

            try
            {
                if (!this.ClientOMAvailable || !bAddToHiddenZone)
                {
                    empty = wppService.AddWebPartToZone(str4, str2, Storage.Shared, innerText, num);
                }
                else
                {
                    try
                    {
                        str2 = this.RemoveIDPropertyFromWebPartXml(str2);
                        object[] objArray = new object[] { sWebPartPageUrl, str2, num, this };
                        empty = (Guid)this.ExecuteClientOMMethod("AddWebPartToHiddenZone", objArray);
                        XmlNamespaceManager xmlNamespaceManagers =
                            new XmlNamespaceManager(webPartNode.OwnerDocument.NameTable);
                        xmlNamespaceManagers.AddNamespace("ns1", webPartNode.NamespaceURI);
                        foreach (XmlNode xmlNodes6 in webPartNode.SelectNodes("./ZoneID|./ns1:ZoneID",
                                     xmlNamespaceManagers))
                        {
                            xmlNodes6.InnerXml = "wpz";
                        }

                        str2 = (xmlNodes5 == null
                            ? webPartNode.OuterXml
                            : string.Concat("<webParts>", xmlNodes5.OuterXml, "</webParts>"));
                        str2 = this.RemoveIDPropertyFromWebPartXml(str2);
                    }
                    catch
                    {
                        empty = wppService.AddWebPartToZone(str4, str2, Storage.Shared, innerText, num);
                    }
                }
            }
            catch
            {
                try
                {
                    empty = wppService.AddWebPart(str4, str2, Storage.Shared);
                }
                catch (SoapException soapException1)
                {
                    SoapException soapException = soapException1;
                    string soapError = RPCUtil.GetSoapError(soapException.Detail);
                    throw new Exception(string.Concat("Soap exception: ", soapError, " Web Part XML: ", str2),
                        soapException);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    throw new Exception(string.Concat("Could not add web part to page: (", exception.Message,
                        "). Web Part XML: ", str2));
                }
            }

            if (xmlNodes5 == null &&
                webPartNode.SelectSingleNode(
                    "//*[contains(@name,'Microsoft.SharePoint.WebPartPages.XsltListViewWebPart')] | //*[contains(@name,'Microsoft.SharePoint.Publishing.WebControls.SummaryLinkWebPart')]") ==
                null)
            {
                try
                {
                    this.UpdateWebPart(empty, str2, str4);
                }
                catch
                {
                }
            }

            webPartGuid = empty;
            if (xmlNodes1 != null && !string.IsNullOrEmpty(xmlNodes1.InnerText) && empty != Guid.Empty)
            {
                string webPart2 = wppService.GetWebPart2(str4, empty, Storage.Shared, SPWebServiceBehavior.Version3);
                webPart2 = webPart2.Substring(webPart2.IndexOf("<WebPart", StringComparison.OrdinalIgnoreCase));
                XmlNode xmlNode = XmlUtility.StringToXmlNode(webPart2);
                XmlNamespaceManager xmlNamespaceManagers1 =
                    new XmlNamespaceManager(xmlNode.FirstChild.OwnerDocument.NameTable);
                xmlNamespaceManagers1.AddNamespace("v2LV", "http://schemas.microsoft.com/WebPart/v2/ListView");
                xmlNamespaceManagers1.AddNamespace("v2", "http://schemas.microsoft.com/WebPart/v2");
                XmlNode xmlNodes7 = xmlNode.SelectSingleNode("./v2LV:ListViewXml", xmlNamespaceManagers1);
                XmlNode xmlNode1 = null;
                if (xmlNodes7 == null)
                {
                    XmlNode xmlNodes8 = xmlNode.SelectSingleNode(".//*[@name='XmlDefinition']");
                    if (xmlNodes8 != null && xmlNodes8.FirstChild != null)
                    {
                        xmlNode1 = XmlUtility.StringToXmlNode(xmlNodes8.FirstChild.InnerText);
                    }
                }
                else
                {
                    xmlNode1 = XmlUtility.StringToXmlNode(xmlNodes7.FirstChild.InnerText);
                }

                if (xmlNode1 != null && xmlNode1.Name == "View" && xmlNode1.Attributes["Type"] != null)
                {
                    ViewsService webServiceForViews = this.GetWebServiceForViews();
                    XmlNode value = XmlUtility.StringToXmlNode(xmlNodes1.InnerText);
                    string str5 = "Name";
                    if (value.Attributes[str5] == null)
                    {
                        XmlAttribute xmlAttribute = value.OwnerDocument.CreateAttribute(str5);
                        value.Attributes.Append(xmlAttribute);
                    }

                    value.Attributes[str5].Value = xmlNode1.Attributes[str5].Value;
                    string str6 = "Url";
                    if (value.Attributes[str6] == null)
                    {
                        XmlAttribute xmlAttribute1 = value.OwnerDocument.CreateAttribute(str6);
                        value.Attributes.Append(xmlAttribute1);
                    }

                    value.Attributes[str6].Value = xmlNode1.Attributes[str6].Value;
                    XmlNodeList xmlNodeLists = value.SelectNodes("./ViewFields/FieldRef");
                    bool flag2 = false;
                    foreach (XmlNode xmlNodes9 in xmlNodeLists)
                    {
                        if (xmlNodes9.Attributes["Explicit"] == null ||
                            !(xmlNodes9.Attributes["Explicit"].Value.ToLower() == "true") ||
                            value.Attributes["BaseViewID"] == null || !(value.Attributes["BaseViewID"].Value == "0"))
                        {
                            continue;
                        }

                        flag2 = true;
                        break;
                    }

                    if (!flag2)
                    {
                        XmlNode xmlNodes10 = value;
                        XmlNode xmlNodes11 = value.SelectSingleNode("./*[local-name() = 'Toolbar']");
                        XmlNode xmlNodes12 = value.SelectSingleNode("./*[local-name() = 'ViewHeader']");
                        XmlNode xmlNodes13 = value.SelectSingleNode("./*[local-name() = 'ViewBody']");
                        XmlNode xmlNodes14 = value.SelectSingleNode("./*[local-name() = 'ViewFooter']");
                        XmlNode xmlNodes15 = value.SelectSingleNode("./*[local-name() = 'ViewEmpty']");
                        XmlNode xmlNodes16 = value.SelectSingleNode("./*[local-name() = 'RowLimitExceeded']");
                        XmlNode xmlNodes17 = value.SelectSingleNode("./*[local-name() = 'Query']");
                        XmlNode xmlNodes18 = value.SelectSingleNode("./*[local-name() = 'ViewFields']");
                        XmlNode xmlNodes19 = value.SelectSingleNode("./*[local-name() = 'Aggregations']");
                        XmlNode xmlNodes20 = value.SelectSingleNode("./*[local-name() = 'Formats']");
                        XmlNode xmlNodes21 = value.SelectSingleNode("./*[local-name() = 'RowLimit']");
                        try
                        {
                            webServiceForViews.UpdateViewHtml(xmlNodes.InnerText, value.Attributes["Name"].Value,
                                xmlNodes10, xmlNodes11, xmlNodes12, xmlNodes13, xmlNodes14, xmlNodes15, xmlNodes16,
                                xmlNodes17, xmlNodes18, xmlNodes19, xmlNodes20, xmlNodes21);
                        }
                        catch
                        {
                            try
                            {
                                webServiceForViews.UpdateView(xmlNodes.InnerText, value.Attributes["Name"].Value,
                                    xmlNodes10, xmlNodes17, xmlNodes18, xmlNodes19, xmlNodes20, xmlNodes21);
                            }
                            catch (SoapException soapException3)
                            {
                                SoapException soapException2 = soapException3;
                                string soapError1 = RPCUtil.GetSoapError(soapException2.Detail);
                                throw new Exception(string.Concat("Soap exception: ", soapError1), soapException2);
                            }
                            catch (Exception exception3)
                            {
                                Exception exception2 = exception3;
                                string[] strArrays = new string[]
                                {
                                    "There was a problem updating the view of a web part that references list: ",
                                    xmlNodes.InnerText, " (", exception2.Message, ")."
                                };
                                throw new Exception(string.Concat(strArrays));
                            }
                        }
                    }
                }
            }

            if (flag1)
            {
                try
                {
                    webServiceForLists.CheckInFile(str4, "", "2");
                }
                catch
                {
                    webServiceForLists.CheckInFile(str4, "Added web part", "0");
                }
            }
        }

        public string AddWebParts(string sWebPartsXml, string sWebPartPageUrl, string sEmbeddedHtmlContent)
        {
            Guid guid;
            bool flag;
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebPartsXml);
            string str = Utils.JoinUrl(this.Server, sWebPartPageUrl);
            ListsService webServiceForLists = this.GetWebServiceForLists();
            bool flag1 = false;
            if (xmlNode.ChildNodes.Count > 0)
            {
                flag1 = webServiceForLists.CheckOutFile(str, "true", null);
            }

            bool flag2 = false;
            if (this.ClientOMAvailable)
            {
                object[] objArray = new object[] { sWebPartPageUrl, this };
                flag2 = (bool)this.ExecuteClientOMMethod("SupportsEmbedding", objArray);
            }

            string webPartPageFromWebService = this.GetWebPartPageFromWebService(sWebPartPageUrl);
            int num = webPartPageFromWebService.IndexOf("<li>PublishingPageLayout\n");
            if (num >= 0)
            {
                webPartPageFromWebService = this.GetPublishingPageContent(webPartPageFromWebService, num);
            }

            int num1 = webPartPageFromWebService.IndexOf("</html>") + "</html>".Length;
            if (num1 >= 0)
            {
                webPartPageFromWebService = webPartPageFromWebService.Substring(num1 + 1);
            }

            List<string> strs = WebPartUtils.ParseWebPartPageForZones(webPartPageFromWebService);
            List<string> list = (
                from zone in strs
                select zone.ToLower()).ToList<string>();
            if (!flag2)
            {
                flag = false;
            }
            else
            {
                flag = (strs == null ? true : strs.Count == 0);
            }

            bool flag3 = flag;
            string str1 = "";
            List<NWSAdapter.EmbeddedWebPart> embeddedWebParts = new List<NWSAdapter.EmbeddedWebPart>();
            Dictionary<string, string> strs1 = null;
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                try
                {
                    bool flag4 = (childNode.Attributes["Embedded"] == null
                        ? false
                        : bool.Parse(childNode.Attributes["Embedded"].Value));
                    string innerText = "DefaultZone";
                    XmlNode xmlNodes = childNode.SelectSingleNode(".//*[name() = 'ZoneID']");
                    if (xmlNodes != null)
                    {
                        innerText = xmlNodes.InnerText;
                    }

                    string innerText1 = null;
                    XmlNode xmlNodes1 = childNode.SelectSingleNode(".//*[name() = 'ID']");
                    if (xmlNodes1 != null)
                    {
                        innerText1 = xmlNodes1.InnerText;
                    }

                    if (!this.ClientOMAvailable || !flag3 && (!flag2 || !flag4 && list.Contains(innerText.ToLower())))
                    {
                        this.AddWebPart(childNode, sWebPartPageUrl, !flag1, webServiceForWebParts, ref strs1, out guid,
                            false);
                    }
                    else
                    {
                        this.AddWebPart(childNode, sWebPartPageUrl, !flag1, webServiceForWebParts, ref strs1, out guid,
                            true);
                        NWSAdapter.EmbeddedWebPart embeddedWebPart = new NWSAdapter.EmbeddedWebPart()
                        {
                            SourceID = Utils.ConvertWebPartIDToGuid(innerText1),
                            TargetID = guid.ToString(),
                            ZoneID = innerText
                        };
                        embeddedWebParts.Add(embeddedWebPart);
                    }
                }
                catch (SoapException soapException)
                {
                    string soapError = RPCUtil.GetSoapError(soapException.Detail);
                    str1 = string.Concat(str1, "Soap error (", soapError, ").");
                }
                catch (Exception exception)
                {
                    str1 = string.Concat(str1, exception.Message);
                }
            }

            if (flag1)
            {
                try
                {
                    webServiceForLists.CheckInFile(str,
                        "Checkout required in order to add web parts to page during migration.", "2");
                }
                catch
                {
                    webServiceForLists.CheckInFile(str,
                        "Checkout required in order to add web parts to page during migration.", "0");
                }
            }

            if (this.ClientOMAvailable && flag2 && embeddedWebParts.Count > 0)
            {
                object[] objArray1 = new object[] { sWebPartPageUrl, embeddedWebParts, sEmbeddedHtmlContent, this };
                this.ExecuteClientOMMethod("EmbedWebParts", objArray1);
            }

            if (!string.IsNullOrEmpty(str1))
            {
                throw new Exception(string.Concat(
                    "One or more web parts encountered problems while being added to page '", sWebPartPageUrl,
                    "'. These web parts may not appear on the page or may appear inaccurately. Error messages: ",
                    str1));
            }

            return string.Empty;
        }

        public string AddWorkflow(string sListId, string sWorkflowXml)
        {
            return null;
        }

        public string AddWorkflowAssociation(string targetID, string workflowXml, bool allowDBWrite)
        {
            return null;
        }

        public string AnalyzeChurn(DateTime pivotDate, string sListID, int iItemID, bool bRecursive)
        {
            throw new NotImplementedException(
                "Database analysis functionality is not available with this type of connection");
        }

        public string Apply2013Theme(string colorPaletteUrl, string spFontUrl, string bgImageUrl)
        {
            return string.Empty;
        }

        public string ApplyOrUpdateContentType(string sListId, string sParentContentTypeName, string sContentTypeXml,
            bool bMakeDefaultContentType)
        {
            string value;
            string str;
            string value1;
            string str1;
            string value2;
            string str2;
            string value3;
            string str3;
            string value4;
            string str4;
            WebsService webServiceForWebs = this.GetWebServiceForWebs();
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNodeList xmlNodeLists =
                XmlUtility.RunXPathQuery(webServiceForWebs.GetContentTypes(), ".//sp:ContentType");
            XmlNode xmlNodes = XmlUtility.MatchFirstAttributeValue("Name", sParentContentTypeName, xmlNodeLists);
            if (xmlNodes == null)
            {
                throw new ArgumentException(string.Concat("There is no content type \"", sParentContentTypeName,
                    "\" available on this web"));
            }

            Exception exception = null;
            string value5 = xmlNodes.Attributes["ID"].Value;
            try
            {
                webServiceForLists.ApplyContentTypeToList(this.Url, value5, sListId);
            }
            catch (Exception exception1)
            {
                exception = exception1;
            }

            XmlNode listContentTypes = webServiceForLists.GetListContentTypes(sListId, "0");
            xmlNodeLists = XmlUtility.RunXPathQuery(listContentTypes, ".//sp:ContentType");
            XmlNode listContentTypeFromWebContentType = this.GetListContentTypeFromWebContentType(value5, xmlNodeLists);
            if (listContentTypeFromWebContentType == null)
            {
                if (exception == null)
                {
                    throw new Exception("Failed to locate content type on target list.");
                }

                throw exception;
            }

            string str5 = listContentTypeFromWebContentType.Attributes["ID"].Value;
            List<XmlNode> xmlNodes1 = new List<XmlNode>();
            List<XmlNode> xmlNodes2 = new List<XmlNode>();
            List<XmlNode> xmlNodes3 = new List<XmlNode>();
            List<string> strs = new List<string>();
            Dictionary<string, string> strs1 = new Dictionary<string, string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sContentTypeXml);
            XmlNode documentElement = xmlDocument.DocumentElement;
            XmlNodeList xmlNodeLists1 = XmlUtility.RunXPathQuery(xmlDocument.DocumentElement, ".//FieldRef");
            XmlNodeList xmlNodeLists2 = XmlUtility.RunXPathQuery(xmlDocument.DocumentElement, ".//Field");
            listContentTypeFromWebContentType = webServiceForLists.GetListContentType(sListId, str5);
            XmlNodeList xmlNodeLists3 = this.FetchSiteColumns(webServiceForWebs);
            XmlNodeList xmlNodeLists4 = this.FetchListFields(sListId, webServiceForLists);
            XmlNodeList xmlNodeLists5 =
                XmlUtility.RunXPathQuery(listContentTypeFromWebContentType, "//sp:Fields/sp:Field");
            if (listContentTypeFromWebContentType.Attributes["Sealed"] != null)
            {
                bool flag = false;
                if (bool.TryParse(listContentTypeFromWebContentType.Attributes["Sealed"].Value, out flag) && flag)
                {
                    string value6 = listContentTypeFromWebContentType.Attributes["ID"].Value;
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    Hashtable hashtables = new Hashtable();
                    this.WriteContentTypeXml(sListId, xmlTextWriter, hashtables,
                        webServiceForLists.GetListContentType(sListId, value6));
                    xmlTextWriter.Flush();
                    return stringBuilder.ToString();
                }
            }

            XmlNode itemOf = documentElement.Attributes["Name"];
            if (itemOf != null)
            {
                strs1.Add("Name", itemOf.Value);
            }

            XmlNode itemOf1 = documentElement.Attributes["Description"];
            if (itemOf1 != null)
            {
                strs1.Add("Description", itemOf1.Value);
            }

            XmlNode itemOf2 = documentElement.Attributes["RequireClientRenderingOnNew"];
            if (itemOf2 != null)
            {
                strs1.Add("RequireClientRenderingOnNew", itemOf2.Value);
            }

            foreach (XmlNode xmlNodes4 in xmlNodeLists1)
            {
                if (xmlNodes4.Attributes["Name"] == null)
                {
                    continue;
                }

                if (xmlNodes4.Attributes["Name"].Value == null)
                {
                    value = null;
                }
                else
                {
                    value = xmlNodes4.Attributes["Name"].Value;
                }

                string str6 = value;
                if (xmlNodes4.Attributes["ID"] == null)
                {
                    str = null;
                }
                else
                {
                    str = xmlNodes4.Attributes["ID"].Value;
                }

                string str7 = str;
                if (!strs.Contains(str6))
                {
                    strs.Add(str6);
                }

                bool flag1 = false;
                XmlNode xmlNodes5 = null;
                foreach (XmlNode xmlNodes6 in xmlNodeLists5)
                {
                    if (xmlNodes6.Attributes["Name"] == null)
                    {
                        continue;
                    }

                    if (xmlNodes6.Attributes["Name"].Value == null)
                    {
                        value1 = null;
                    }
                    else
                    {
                        value1 = xmlNodes6.Attributes["Name"].Value;
                    }

                    if (value1 != str6)
                    {
                        if (xmlNodes6.Attributes["ID"] == null || string.IsNullOrEmpty(str7) ||
                            string.IsNullOrEmpty(xmlNodes6.Attributes["ID"].Value) ||
                            !(xmlNodes6.Attributes["ID"].Value == str7))
                        {
                            continue;
                        }

                        xmlNodes5 = xmlNodes6;
                    }
                    else
                    {
                        flag1 = true;
                        break;
                    }
                }

                if (flag1 || xmlNodes5 != null)
                {
                    continue;
                }

                bool flag2 = false;
                foreach (XmlNode xmlNodes7 in xmlNodeLists4)
                {
                    if (xmlNodes7.Attributes["Name"] == null)
                    {
                        continue;
                    }

                    if (xmlNodes7.Attributes["Name"].Value == null)
                    {
                        str1 = null;
                    }
                    else
                    {
                        str1 = xmlNodes7.Attributes["Name"].Value;
                    }

                    if (str1 != str6)
                    {
                        if (xmlNodes7.Attributes["ID"] == null || string.IsNullOrEmpty(str7) ||
                            string.IsNullOrEmpty(xmlNodes7.Attributes["ID"].Value) ||
                            !(xmlNodes7.Attributes["ID"].Value == str7))
                        {
                            continue;
                        }

                        xmlNodes5 = xmlNodes7;
                    }
                    else
                    {
                        flag2 = true;
                        xmlNodes2.Add(xmlNodes7);
                        break;
                    }
                }

                if (flag2 || str7 == "{fa564e0f-0c70-4ab9-b863-0177e6ddd247}")
                {
                    continue;
                }

                if (xmlNodes5 == null)
                {
                    foreach (XmlNode xmlNodes8 in xmlNodeLists3)
                    {
                        if (xmlNodes8.Attributes["Name"] == null)
                        {
                            continue;
                        }

                        if (xmlNodes8.Attributes["Name"].Value == null)
                        {
                            value2 = null;
                        }
                        else
                        {
                            value2 = xmlNodes8.Attributes["Name"].Value;
                        }

                        if (value2 != str6)
                        {
                            continue;
                        }

                        xmlNodes1.Add(xmlNodes8);
                        xmlNodes2.Add(xmlNodes8);
                        break;
                    }
                }
                else
                {
                    xmlNodes2.Add(xmlNodes5);
                }
            }

            foreach (XmlNode xmlNodes9 in xmlNodeLists2)
            {
                if (xmlNodes9.Attributes["Name"] == null)
                {
                    continue;
                }

                if (xmlNodes9.Attributes["Name"] == null)
                {
                    str2 = null;
                }
                else
                {
                    str2 = xmlNodes9.Attributes["Name"].Value;
                }

                string str8 = str2;
                if (xmlNodes9.Attributes["ID"] == null)
                {
                    value3 = null;
                }
                else
                {
                    value3 = xmlNodes9.Attributes["ID"].Value;
                }

                string str9 = value3;
                if (!strs.Contains(str8))
                {
                    strs.Add(str8);
                }

                bool flag3 = false;
                XmlNode xmlNodes10 = null;
                foreach (XmlNode xmlNodes11 in xmlNodeLists5)
                {
                    if (xmlNodes11.Attributes["Name"] == null)
                    {
                        continue;
                    }

                    if (xmlNodes11.Attributes["Name"].Value == null)
                    {
                        str3 = null;
                    }
                    else
                    {
                        str3 = xmlNodes11.Attributes["Name"].Value;
                    }

                    if (str3 != str8)
                    {
                        if (xmlNodes11.Attributes["ID"] == null || string.IsNullOrEmpty(str9) ||
                            string.IsNullOrEmpty(xmlNodes11.Attributes["ID"].Value) ||
                            !(xmlNodes11.Attributes["ID"].Value == str9))
                        {
                            continue;
                        }

                        xmlNodes10 = xmlNodes11;
                    }
                    else
                    {
                        flag3 = true;
                        break;
                    }
                }

                if (flag3 || xmlNodes10 != null)
                {
                    continue;
                }

                bool flag4 = false;
                foreach (XmlNode xmlNodes12 in xmlNodeLists4)
                {
                    if (xmlNodes12.Attributes["Name"] == null)
                    {
                        continue;
                    }

                    if (xmlNodes12.Attributes["Name"].Value == null)
                    {
                        value4 = null;
                    }
                    else
                    {
                        value4 = xmlNodes12.Attributes["Name"].Value;
                    }

                    if (value4 != str8)
                    {
                        if (xmlNodes12.Attributes["ID"] == null || string.IsNullOrEmpty(str9) ||
                            string.IsNullOrEmpty(xmlNodes12.Attributes["ID"].Value) ||
                            !(xmlNodes12.Attributes["ID"].Value == str9))
                        {
                            continue;
                        }

                        xmlNodes10 = xmlNodes12;
                    }
                    else
                    {
                        flag4 = true;
                        xmlNodes2.Add(xmlNodes12);
                        break;
                    }
                }

                if (flag4)
                {
                    continue;
                }

                if (xmlNodes10 == null)
                {
                    bool flag5 = false;
                    foreach (XmlNode xmlNodes13 in xmlNodeLists3)
                    {
                        if (xmlNodes13.Attributes["Name"] == null)
                        {
                            continue;
                        }

                        if (xmlNodes13.Attributes["Name"].Value == null)
                        {
                            str4 = null;
                        }
                        else
                        {
                            str4 = xmlNodes13.Attributes["Name"].Value;
                        }

                        if (str4 != str8)
                        {
                            continue;
                        }

                        flag5 = true;
                        xmlNodes1.Add(xmlNodes13);
                        xmlNodes2.Add(xmlNodes13);
                        break;
                    }

                    if (flag5)
                    {
                        continue;
                    }

                    xmlNodes1.Add(xmlNodes9);
                    xmlNodes2.Add(xmlNodes9);
                }
                else
                {
                    xmlNodes2.Add(xmlNodes10);
                }
            }

            if (xmlNodes1.Count > 0)
            {
                this.AddListFields(sListId, webServiceForLists, xmlNodes1, xmlNodeLists4);
                xmlNodeLists4 = this.FetchListFields(sListId, webServiceForLists);
                this.RemoveFields(xmlNodes2, xmlNodeLists4, strs);
            }

            return this.AddAndRemoveContentTypeXmlFields(strs1, xmlNodes2, webServiceForLists, sListId, str5);
        }

        public void AssociateWorkflowAssociation(string configFileUrl, string configVersion)
        {
            WebPartPagesService webPartPagesService = new WebPartPagesService(this);
            webPartPagesService.AssociateWorkflowMarkup(configFileUrl, string.Concat("V", configVersion));
        }

        public string BeginCompilingAllAudiences()
        {
            return string.Empty;
        }

        private void BreakRoleDefinitionInheritance()
        {
            string permissionsUrl = this.GetPermissionsUrl(null, -1, "role");
            string str = this.HttpGet(permissionsUrl);
            if (str.Contains("<a href=\"javascript:delRoles()\""))
            {
                return;
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                throw new Exception(
                    "Cannot break permission level inheritance via the native web services for SharePoint 2010");
            }

            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { HttpUtility.UrlDecode("ctl00%24PlaceHolderMain%24hdnOperation"), "CopyRoles" }
            };
            string sharePointHttpPostParameters = this.GetSharePointHttpPostParameters(str, "<input.*/>", strs);
            this.HttpPost(permissionsUrl, sharePointHttpPostParameters.ToString());
        }

        private void BreakRoleInheritance(string sListID, int iItemId)
        {
            string permissionsUrl = this.GetPermissionsUrl(sListID, iItemId, "user");
            string str = this.HttpGet(permissionsUrl);
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { HttpUtility.UrlDecode("ctl00%24PlaceHolderMain%24hdnOperation"), "copyPermission" }
            };
            string sharePointHttpPostParameters = this.GetSharePointHttpPostParameters(str, "<input.*/>", strs);
            this.HttpPost(permissionsUrl, sharePointHttpPostParameters.ToString());
        }

        private string BuildBatchListUpdateCommand(string sListID, string sListVersion,
            Dictionary<string, string> propertiesToSet)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            xmlTextWriter.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlTextWriter.WriteStartElement("ows:Batch");
            xmlTextWriter.WriteAttributeString("OnError", "Continue");
            xmlTextWriter.WriteAttributeString("Version", "6.0.2.6551");
            xmlTextWriter.WriteAttributeString("xmlns:ows", "http://schemas.microsoft.com/sharepoint/soap/");
            xmlTextWriter.WriteStartElement("Method");
            xmlTextWriter.WriteAttributeString("ID", "0,MODLISTSETTINGS");
            xmlTextWriter.WriteStartElement("SetList");
            xmlTextWriter.WriteAttributeString("Scope", "Request");
            xmlTextWriter.WriteString(sListID);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("SetVar");
            xmlTextWriter.WriteAttributeString("Name", "Cmd");
            xmlTextWriter.WriteString("MODLISTSETTINGS");
            xmlTextWriter.WriteEndElement();
            foreach (string key in propertiesToSet.Keys)
            {
                xmlTextWriter.WriteStartElement("SetVar");
                xmlTextWriter.WriteAttributeString("Name", key);
                xmlTextWriter.WriteString(propertiesToSet[key]);
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteStartElement("SetVar");
            xmlTextWriter.WriteAttributeString("Name", "owshiddenversion");
            xmlTextWriter.WriteRaw(sListVersion);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public void BuildCommandUpdateItemMetadata(ref StringBuilder sb, XmlNode listXML, XmlNode itemXML,
            XmlNodeList fields)
        {
            string str;
            string str1;
            if (fields == null)
            {
                fields = XmlUtility.RunXPathQuery(listXML, "//sp:Field");
            }

            bool flag = (itemXML.Attributes["FSObjType"] == null
                ? false
                : itemXML.Attributes["FSObjType"].Value == "1");
            this.GetAllDayEventFields(fields, itemXML, out str, out str1);
            if (flag)
            {
                string str2 = (itemXML.Attributes["FileLeafRef"] != null
                    ? itemXML.Attributes["FileLeafRef"].Value
                    : itemXML.Attributes["Title"].Value);
                sb.Append(string.Concat("<Field Name='FSObjType'>1</Field><Field Name='BaseName'>", str2, "</Field>"));
            }

            foreach (XmlNode field in fields)
            {
                try
                {
                    string str3 = (itemXML.Attributes["ows_FileLeafRef"] != null ? "ows_" : "");
                    string value = field.Attributes["Name"].Value;
                    string str4 = XmlUtility.EncodeNameStartChars(value);
                    string value1 = field.Attributes["Type"].Value;
                    if (NWSAdapter.IsWritableColumn(value,
                            (field.Attributes["ReadOnly"] == null
                                ? false
                                : Convert.ToBoolean(field.Attributes["ReadOnly"].Value)), value1,
                            Convert.ToInt32(this.GetListServerTemplate(listXML)), flag,
                            field.Attributes["BdcField"] != null))
                    {
                        if (itemXML.Attributes[str4] != null)
                        {
                            string value2 = itemXML.Attributes[str4].Value;
                            if (string.IsNullOrEmpty(value2))
                            {
                                sb.Append(string.Concat("<Field Name='", value, "'></Field>"));
                                continue;
                            }
                            else if (value == str || value == str1)
                            {
                                value2 = Utils.ParseDateAsUtc(value2).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
                                string[] strArrays = new string[]
                                    { "<Field Name='", value, "'>", HttpUtility.HtmlEncode(value2), "</Field>" };
                                sb.Append(string.Concat(strArrays));
                            }
                            else if (value != string.Concat(str3, "WikiField"))
                            {
                                if (value1 == "User")
                                {
                                    string dFromUser = this.GetIDFromUser(value2) ?? this.GetIDFromGroup(value2);
                                    value2 = dFromUser;
                                }

                                if (value1 == "UserMulti")
                                {
                                    char[] chrArray = new char[] { ',' };
                                    string[] strArrays1 = value2.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
                                    string str5 = null;
                                    string[] strArrays2 = strArrays1;
                                    for (int i = 0; i < (int)strArrays2.Length; i++)
                                    {
                                        string str6 = strArrays2[i];
                                        string str7 = null;
                                        string dFromUser1 = this.GetIDFromUser(str6);
                                        if (string.IsNullOrEmpty(dFromUser1))
                                        {
                                            string dFromGroup = this.GetIDFromGroup(str6);
                                            if (!string.IsNullOrEmpty(dFromGroup))
                                            {
                                                str7 = string.Concat(dFromGroup, ";#", str6);
                                            }
                                        }
                                        else
                                        {
                                            str7 = string.Concat(dFromUser1, ";#", str6);
                                        }

                                        if (!string.IsNullOrEmpty(str7))
                                        {
                                            str5 = (!string.IsNullOrEmpty(str5)
                                                ? string.Concat(str5, ";#", str7)
                                                : str7);
                                        }
                                    }

                                    value2 = str5;
                                }

                                if (value1 == "LookupMulti")
                                {
                                    string[] strArrays3 = new string[] { ";#" };
                                    string[] strArrays4 =
                                        value2.Split(strArrays3, StringSplitOptions.RemoveEmptyEntries);
                                    string str8 = "";
                                    for (int j = 0; j < (int)strArrays4.Length; j++)
                                    {
                                        if (!string.IsNullOrEmpty(str8))
                                        {
                                            str8 = string.Concat(str8, ";#");
                                        }

                                        str8 = string.Concat(str8, strArrays4[j], ";#");
                                    }

                                    value2 = str8;
                                }

                                if (value1 == "Date" || value1 == "DateTime" ||
                                    value1 == "PublishingScheduleStartDateFieldType" ||
                                    value1 == "PublishingScheduleEndDateFieldType")
                                {
                                    DateTime dateTime = Utils.ParseDate(value2, this.TimeZone);
                                    value2 = dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
                                }

                                string[] strArrays5 = new string[]
                                    { "<Field Name='", value, "'>", HttpUtility.HtmlEncode(value2), "</Field>" };
                                sb.Append(string.Concat(strArrays5));
                            }
                        }
                        else
                        {
                            XmlNode xmlNodes = field.SelectSingleNode("./Default");
                            if ((xmlNodes != null || field.Attributes["Required"] != null &&
                                    bool.Parse(field.Attributes["Required"].Value)) && !flag)
                            {
                                string str9 = (xmlNodes != null ? xmlNodes.InnerText : "");
                                sb.Append(string.Format("<Field Name='{0}'>{1}</Field>", value, str9));
                            }

                            continue;
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    string[] value3 = new string[]
                    {
                        "Error accessing data: Target Field Name: ", field.Attributes["Name"].Value, ", Target Type: ",
                        field.Attributes["Type"].Value, ", ", exception.Message
                    };
                    throw new Exception(string.Concat(value3));
                }
            }
        }

        public void BuildCommandUpdateItemModeration(ref StringBuilder sb, XmlNode itemXML)
        {
            string value;
            if (itemXML.Attributes["_ModerationStatus"] == null)
            {
                return;
            }

            string str = itemXML.Attributes["_ModerationStatus"].Value;
            if (itemXML.Attributes["_ModerationComments"] != null)
            {
                value = itemXML.Attributes["_ModerationComments"].Value;
            }
            else
            {
                value = null;
            }

            string str1 = value;
            sb.Append(string.Concat("<Field Name='_ModerationStatus'>", HttpUtility.HtmlEncode(str), "</Field>"));
            sb.Append(string.Concat("<Field Name='_ModerationComments'>", HttpUtility.HtmlEncode(str1), "</Field>"));
        }

        private void BuildGroupMap()
        {
            if (this.m_GroupMap != null)
            {
                this.m_GroupMap.Clear();
            }
            else
            {
                this.m_GroupMap = new Dictionary<string, string>();
            }

            if (this.m_ReverseGroupMap != null)
            {
                this.m_ReverseGroupMap.Clear();
            }
            else
            {
                this.m_ReverseGroupMap = new Dictionary<string, string>();
            }

            string innerText = null;
            string str = null;
            foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(
                         this.GetWebServiceForUserGroup().GetGroupCollectionFromSite(), "//d:Group"))
            {
                innerText = xmlNodes.Attributes["ID"].InnerText;
                str = xmlNodes.Attributes["Name"].InnerText;
                this.m_GroupMap[innerText] = str;
                this.m_ReverseGroupMap[str] = innerText;
            }
        }

        public string BuildItemMetadataRPC(XmlNodeList fields, XmlNode itemXml, string sOws)
        {
            string str;
            string str1;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(RPCUtil.FormatItemMetadataForRPC(itemXml, sOws));
            foreach (XmlNode field in fields)
            {
                if (field.Attributes["Name"] == null)
                {
                    continue;
                }

                string value = field.Attributes["Name"].Value;
                string str2 = XmlUtility.EncodeNameStartChars(value);
                if ((field.Attributes["ReadOnly"] != null ? bool.Parse(field.Attributes["ReadOnly"].Value) : false) &&
                    value != "PublishingPageLayout" || value == "Created" || value == "Modified" || value == "Author" ||
                    value == "Editor")
                {
                    continue;
                }

                string value1 = field.Attributes["Type"].Value;
                if (itemXml.Attributes[str2] == null)
                {
                    continue;
                }

                string value2 = itemXml.Attributes[str2].Value;
                if (value == "Title")
                {
                    stringBuilder.Append(string.Concat(";",
                        RPCUtil.FormatFieldForRPC("vti_title", value2, typeof(string), this.TimeZone)));
                }
                else if (value != "Description")
                {
                    string str3 = value1;
                    string str4 = str3;
                    if (str3 != null)
                    {
                        if (str4 == "WikiField")
                        {
                            string str5 = RPCUtil.FormatFieldForRPC("WikiField", value2, typeof(string), this.TimeZone);
                            if (str5 == null)
                            {
                                continue;
                            }

                            stringBuilder.Append(string.Concat(";", str5));
                            continue;
                        }
                        else if (str4 == "User")
                        {
                            string str6 = null;
                            if (field.Attributes["ReadOnly"] == null ||
                                field.Attributes["ReadOnly"].Value.Equals("False"))
                            {
                                value2 = this.EnsureCorrectUserFormat(value2);
                                if (this.m_ReverseUserMap.TryGetValue(value2.ToLowerInvariant(), out str))
                                {
                                    value2 = string.Concat(str, ";#", value2);
                                    str6 = RPCUtil.FormatFieldForRPC(value, value2, typeof(int), this.TimeZone);
                                }
                            }

                            if (str6 == null)
                            {
                                continue;
                            }

                            stringBuilder.Append(string.Concat(";", str6));
                            continue;
                        }
                        else if (str4 == "UserMulti")
                        {
                            string[] strArrays = value2.Split(new char[] { ',' });
                            value2 = "";
                            string[] strArrays1 = strArrays;
                            for (int i = 0; i < (int)strArrays1.Length; i++)
                            {
                                string str7 = strArrays1[i];
                                this.EnsureCorrectUserFormat(str7);
                                if (this.m_ReverseUserMap.TryGetValue(str7.ToLowerInvariant(), out str1))
                                {
                                    if (!string.IsNullOrEmpty(value2))
                                    {
                                        value2 = string.Concat(value2, ";#");
                                    }

                                    value2 = string.Concat(value2, str1, ";#", str7);
                                }
                            }

                            string str8 = RPCUtil.FormatFieldForRPC(value, value2, typeof(int), this.TimeZone);
                            if (str8 == null)
                            {
                                continue;
                            }

                            stringBuilder.Append(string.Concat(";", str8));
                            continue;
                        }
                    }

                    string str9 = RPCUtil.FormatFieldForRPC(value, value2, NWSAdapter.TypeFromString(value1),
                        this.TimeZone);
                    if (str9 == null)
                    {
                        continue;
                    }

                    stringBuilder.Append(string.Concat(";", str9));
                }
                else
                {
                    stringBuilder.Append(string.Concat(";",
                        RPCUtil.FormatFieldForRPC("vti_description", value2, typeof(string), this.TimeZone)));
                }
            }

            return stringBuilder.ToString();
        }

        private string BuildNavNodeAddition(XmlNode node, XmlNode currentWebStructure, ref int iAddtionModNum,
            bool bWatchForIsVisibleChanges, Dictionary<Guid, string> pagesLibGuidMap, ref List<Guid> hiddenGlobalNodes,
            ref List<Guid> hiddenCurrentNodes, ref bool bHiddenChangesMade, bool bIsOnGlobalNav, bool bIsOnCurrentNav)
        {
            return this.BuildNavNodeAddition(node, currentWebStructure, ref iAddtionModNum,
                node.Attributes["ParentID"].Value, bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes,
                ref hiddenCurrentNodes, ref bHiddenChangesMade, bIsOnGlobalNav, bIsOnCurrentNav);
        }

        private string BuildNavNodeAddition(XmlNode node, XmlNode currentWebStructure, ref int iAddtionModNum,
            string sParentEid, bool bWatchForIsVisibleChanges, Dictionary<Guid, string> pagesLibGuidMap,
            ref List<Guid> hiddenGlobalNodes, ref List<Guid> hiddenCurrentNodes, ref bool bHiddenChangesMade,
            bool bIsOnGlobalNav, bool bIsOnCurrentNav)
        {
            string str = (node.Attributes["IsExternal"].Value.ToLower() == "true" ? "link" : "page");
            string str1 = RPCUtil.EscapeAndEncodeValue(node.Attributes["Title"].Value);
            string str2 = RPCUtil.EscapeAndEncodeValue(HttpUtility.UrlDecode(node.Attributes["Url"].Value));
            string str3 = this.BuildNavNodeMetaInfoEntry(node);
            if (sParentEid == "-1")
            {
                sParentEid = "0";
            }

            object[] objArray = new object[]
            {
                iAddtionModNum.ToString(), sParentEid, "01 Jan 1970 00:00:00 -0000", str, "addExistingPage", str1, str2,
                str3
            };
            string str4 =
                string.Format(
                    "[eidMod={0};eidParent={1};eidRef=-2;DTLP={2};eType={3};mType={4};name={5};url={6};meta-info=[{7}]]",
                    objArray);
            string str5 = iAddtionModNum.ToString();
            iAddtionModNum++;
            if (bWatchForIsVisibleChanges && node.Attributes["IsVisible"] != null &&
                node.Attributes["IsVisible"].Value.ToLower() == "false")
            {
                if (int.Parse(sParentEid) >= 1000)
                {
                    bIsOnGlobalNav = (bIsOnGlobalNav || sParentEid == "1002"
                        ? true
                        : this.GetNavNodeIsInGlobalNav(sParentEid, currentWebStructure));
                    bIsOnCurrentNav = (bIsOnCurrentNav || sParentEid == "1025"
                        ? true
                        : this.GetNavNodeIsInQuickLaunch(sParentEid, currentWebStructure));
                }

                Guid pageOrWebGuidFromUrl = this.GetPageOrWebGuidFromUrl(node.Attributes["Url"].Value, pagesLibGuidMap);
                if (pageOrWebGuidFromUrl != Guid.Empty)
                {
                    if (bIsOnGlobalNav)
                    {
                        if (!hiddenGlobalNodes.Contains(pageOrWebGuidFromUrl))
                        {
                            hiddenGlobalNodes.Add(pageOrWebGuidFromUrl);
                            bHiddenChangesMade = true;
                        }
                    }
                    else if (bIsOnCurrentNav && !hiddenCurrentNodes.Contains(pageOrWebGuidFromUrl))
                    {
                        hiddenCurrentNodes.Add(pageOrWebGuidFromUrl);
                        bHiddenChangesMade = true;
                    }
                }
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                str4 = string.Concat(str4,
                    this.BuildNavNodeAddition(childNode, currentWebStructure, ref iAddtionModNum, str5,
                        bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                        ref bHiddenChangesMade, bIsOnGlobalNav, bIsOnCurrentNav));
                iAddtionModNum++;
            }

            return str4;
        }

        private string BuildNavNodeDeletion(XmlNode node)
        {
            return this.GetNavNodeEditRequest(node, null, "delete");
        }

        private string BuildNavNodeMetaInfoEntry(XmlNode node)
        {
            string str = "";
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.Name == "Title" || attribute.Name == "Url" || attribute.Name == "IsExternal" ||
                    attribute.Name == "IsVisible" || attribute.Name == "ID" || attribute.Name == "ParentID" ||
                    attribute.Name == "LastModified" || attribute.Name == "MLOrderIndex")
                {
                    continue;
                }

                string value = attribute.Value;
                if (value.IndexOf("|") < 0)
                {
                    value = string.Concat("SW|", RPCUtil.EscapeAndEncodeValue(value));
                }
                else
                {
                    int num = value.IndexOf("|");
                    string str1 = value.Substring(0, num + 1);
                    string str2 = "";
                    if (value.Length > num + 1)
                    {
                        str2 = value.Substring(num + 1);
                    }

                    str2 = RPCUtil.EscapeAndEncodeValue(str2);
                    value = string.Concat(str1, str2);
                }

                string str3 = str;
                string[] name = new string[] { str3, null, null, null, null };
                name[1] = (string.IsNullOrEmpty(str) ? "" : ";");
                name[2] = attribute.Name;
                name[3] = ";";
                name[4] = value;
                str = string.Concat(name);
            }

            return str;
        }

        private string BuildNavNodeUpdate(XmlNode node, XmlNode currentWebStructure, ref int iAddtionModNum,
            bool bWatchForIsVisibleChanges, Dictionary<Guid, string> pagesLibGuidMap, ref List<Guid> hiddenGlobalNodes,
            ref List<Guid> hiddenCurrentNodes, ref bool bHiddenChangesMade)
        {
            XmlNode xmlNodes =
                currentWebStructure.SelectSingleNode(string.Concat("//NavNode[@ID=\"", node.Attributes["ID"].Value,
                    "\"]"));
            string str = "";
            if (xmlNodes != null)
            {
                if (node.Attributes["Title"] != null)
                {
                    str = string.Concat(str, this.GetNavNodeEditRequest(node, xmlNodes, "changeLabel"));
                }
                else if (node.Attributes["Url"] != null)
                {
                    str = string.Concat(str, this.GetNavNodeEditRequest(node, xmlNodes, "changeUrl"));
                }

                str = string.Concat(str, this.GetNavNodeEditRequest(node, xmlNodes, "changeMetaInfo"));
                if (bWatchForIsVisibleChanges)
                {
                    XmlAttribute itemOf = xmlNodes.Attributes["IsVisible"];
                    XmlAttribute xmlAttribute = node.Attributes["IsVisible"];
                    if (itemOf != null && xmlAttribute != null)
                    {
                        if (itemOf.Value.ToLower() == "true" && xmlAttribute.Value.ToLower() == "false")
                        {
                            Guid pageOrWebGuidFromUrl =
                                this.GetPageOrWebGuidFromUrl(node.Attributes["Url"].Value, pagesLibGuidMap);
                            if (pageOrWebGuidFromUrl != Guid.Empty)
                            {
                                if (this.GetNavNodeIsInQuickLaunch(node.Attributes["ID"].Value, currentWebStructure))
                                {
                                    if (!hiddenCurrentNodes.Contains(pageOrWebGuidFromUrl))
                                    {
                                        hiddenCurrentNodes.Add(pageOrWebGuidFromUrl);
                                        bHiddenChangesMade = true;
                                    }
                                }
                                else if (this.GetNavNodeIsInGlobalNav(node.Attributes["ID"].Value,
                                             currentWebStructure) && !hiddenGlobalNodes.Contains(pageOrWebGuidFromUrl))
                                {
                                    hiddenGlobalNodes.Add(pageOrWebGuidFromUrl);
                                    bHiddenChangesMade = true;
                                }
                            }
                        }
                        else if (itemOf.Value.ToLower() == "false" && xmlAttribute.Value.ToLower() == "true")
                        {
                            Guid guid = this.GetPageOrWebGuidFromUrl(node.Attributes["Url"].Value, pagesLibGuidMap);
                            if (guid != Guid.Empty)
                            {
                                if (this.GetNavNodeIsInQuickLaunch(node.Attributes["ID"].Value, currentWebStructure))
                                {
                                    if (hiddenCurrentNodes.Remove(guid))
                                    {
                                        bHiddenChangesMade = true;
                                    }
                                }
                                else if (this.GetNavNodeIsInGlobalNav(node.Attributes["ID"].Value,
                                             currentWebStructure) && hiddenGlobalNodes.Remove(guid))
                                {
                                    bHiddenChangesMade = true;
                                }
                            }
                        }
                    }
                }
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                str = (!this.GetNavNodeExists(childNode, currentWebStructure)
                    ? string.Concat(str,
                        this.BuildNavNodeAddition(childNode, currentWebStructure, ref iAddtionModNum,
                            bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                            ref bHiddenChangesMade, false, false))
                    : string.Concat(str,
                        this.BuildNavNodeUpdate(childNode, currentWebStructure, ref iAddtionModNum,
                            bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                            ref bHiddenChangesMade)));
            }

            return str;
        }

        public static string BuildQuery(string sIDs, string sParentFolder, bool bRecursive, bool bIs2003IssuesList,
            ListItemQueryType itemTypes, string[] sOrderByFields)
        {
            if (sParentFolder != null)
            {
                sParentFolder = sParentFolder.TrimStart(new char[] { '/' });
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            bool flag = (sIDs == null ? false : sIDs.Length > 0);
            bool flag1 = (sParentFolder == null ? false : sParentFolder.Length > 0);
            bool flag2 = itemTypes != (ListItemQueryType.ListItem | ListItemQueryType.Folder);
            xmlTextWriter.WriteStartElement("Where");
            if (flag1)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (flag2)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (bIs2003IssuesList)
            {
                xmlTextWriter.WriteStartElement("And");
            }

            if (!flag)
            {
                xmlTextWriter.WriteStartElement("Gt");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "ID");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Counter");
                xmlTextWriter.WriteValue("0");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }
            else
            {
                string[] strArrays = sIDs.Split(new char[] { ',' });
                int length = (int)strArrays.Length - 1;
                for (int i = 1; i < (int)strArrays.Length; i++)
                {
                    xmlTextWriter.WriteStartElement("Or");
                }

                bool flag3 = false;
                string[] strArrays1 = strArrays;
                for (int j = 0; j < (int)strArrays1.Length; j++)
                {
                    string str = strArrays1[j];
                    int num = str.IndexOf('#');
                    string str1 = (num >= 0 ? str.Substring(0, num) : str);
                    xmlTextWriter.WriteStartElement("Eq");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "ID");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Counter");
                    xmlTextWriter.WriteValue(str1);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                    if (flag3 && length > 0)
                    {
                        xmlTextWriter.WriteEndElement();
                        length--;
                    }

                    flag3 = true;
                }
            }

            if (bIs2003IssuesList)
            {
                xmlTextWriter.WriteStartElement("Neq");
                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "IsCurrent");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Boolean");
                xmlTextWriter.WriteValue("False");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }

            if (flag2)
            {
                string str2 = null;
                if (itemTypes == ListItemQueryType.Folder)
                {
                    str2 = "1";
                }
                else if (itemTypes == ListItemQueryType.ListItem)
                {
                    str2 = "0";
                }

                if (str2 != null)
                {
                    xmlTextWriter.WriteStartElement("Eq");
                    xmlTextWriter.WriteStartElement("FieldRef");
                    xmlTextWriter.WriteAttributeString("Name", "FSObjType");
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Value");
                    xmlTextWriter.WriteAttributeString("Type", "Counter");
                    xmlTextWriter.WriteValue(str2);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }
            }

            if (flag1)
            {
                if (!bRecursive)
                {
                    xmlTextWriter.WriteStartElement("Eq");
                }
                else
                {
                    xmlTextWriter.WriteStartElement("BeginsWith");
                }

                xmlTextWriter.WriteStartElement("FieldRef");
                xmlTextWriter.WriteAttributeString("Name", "FileDirRef");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Value");
                xmlTextWriter.WriteAttributeString("Type", "Text");
                xmlTextWriter.WriteValue(sParentFolder);
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            if (sOrderByFields != null && (int)sOrderByFields.Length > 0)
            {
                stringBuilder.Append("<OrderBy>");
                string[] strArrays2 = sOrderByFields;
                for (int k = 0; k < (int)strArrays2.Length; k++)
                {
                    string str3 = strArrays2[k];
                    stringBuilder.Append("<FieldRef Ascending = 'TRUE' Name = '");
                    stringBuilder.Append(str3);
                    stringBuilder.Append("' />");
                }

                stringBuilder.Append("</OrderBy>");
            }

            return stringBuilder.ToString();
        }

        private string BuildQueryFieldsXml(XmlNodeList fields, bool bIsDocLib, IEnumerable<string> fieldsToEnsureAdded)
        {
            List<string> strs = (fieldsToEnsureAdded == null
                ? new List<string>()
                : new List<string>(fieldsToEnsureAdded));
            if (!strs.Contains("FileDirRef") && (base.SharePointVersion.IsSharePoint2007OrLater ||
                                                 base.SharePointVersion.IsSharePoint2003 && bIsDocLib))
            {
                strs.Add("FileDirRef");
            }

            StringBuilder stringBuilder = new StringBuilder(500);
            foreach (XmlNode field in fields)
            {
                XmlAttribute itemOf = field.Attributes["Name"];
                if (itemOf == null)
                {
                    continue;
                }

                string value = itemOf.Value;
                if (value != "FileDirRef" && value != "FileLeafRef" || base.SharePointVersion.IsSharePoint2007OrLater ||
                    bIsDocLib)
                {
                    stringBuilder.Append("<FieldRef Name='");
                    stringBuilder.Append(value);
                    stringBuilder.Append("'/>");
                }

                if (!strs.Contains(value))
                {
                    continue;
                }

                strs.Remove(value);
            }

            foreach (string str in strs)
            {
                stringBuilder.Append("<FieldRef Name='");
                stringBuilder.Append(str);
                stringBuilder.Append("'/>");
            }

            return stringBuilder.ToString();
        }

        public XmlElement BuildUpdateItemCommand(XmlNode listXML, XmlNode listItemXML, XmlNodeList fields, string sID,
            bool bAdding, string sRootFolder)
        {
            XmlElement str = (new XmlDocument()).CreateElement("Batch");
            str.SetAttribute("OnError", "Continue");
            str.SetAttribute("ListVersion", "1");
            str.SetAttribute("RootFolder", sRootFolder);
            StringBuilder stringBuilder = new StringBuilder(500);
            string str1 = (bAdding ? "New" : "Update");
            stringBuilder.Append(string.Concat("<Method ID='1' Cmd='", str1, "'>"));
            if (str1 == "Update")
            {
                stringBuilder.Append(string.Concat("<Field Name='ID'>", sID, "</Field>"));
            }

            this.BuildCommandUpdateItemMetadata(ref stringBuilder, listXML, listItemXML, fields);
            if (listXML.Attributes["EnableModeration"] != null &&
                bool.Parse(listXML.Attributes["EnableModeration"].Value))
            {
                this.BuildCommandUpdateItemModeration(ref stringBuilder, listItemXML);
            }

            stringBuilder.Append("</Method>");
            str.InnerXml = stringBuilder.ToString();
            return str;
        }

        public XmlElement BuildUpdateModerationCommand(XmlNode listXML, XmlNode listItemXML, string sID)
        {
            XmlElement str = (new XmlDocument()).CreateElement("Batch");
            str.SetAttribute("OnError", "Continue");
            str.SetAttribute("ListVersion", "1");
            StringBuilder stringBuilder = new StringBuilder(500);
            string str1 = "Moderate";
            stringBuilder.Append(string.Concat("<Method ID='1' Cmd='", str1, "'>"));
            if (str1 == "Moderate")
            {
                stringBuilder.Append(string.Concat("<Field Name='ID'>", sID, "</Field>"));
            }

            if (listXML.Attributes["EnableModeration"] != null &&
                bool.Parse(listXML.Attributes["EnableModeration"].Value))
            {
                this.BuildCommandUpdateItemModeration(ref stringBuilder, listItemXML);
            }

            stringBuilder.Append("</Method>");
            str.InnerXml = stringBuilder.ToString();
            return str;
        }

        private void BuildUserMap()
        {
            if (this.m_UserMap != null)
            {
                this.m_UserMap.Clear();
            }
            else
            {
                this.m_UserMap = new Dictionary<string, string>();
            }

            if (this.m_ReverseUserMap != null)
            {
                this.m_ReverseUserMap.Clear();
            }
            else
            {
                this.m_ReverseUserMap = new Dictionary<string, string>();
            }

            string innerText = null;
            string lowerInvariant = null;
            foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(
                         this.GetWebServiceForUserGroup().GetUserCollectionFromSite(), "//d:User"))
            {
                innerText = xmlNodes.Attributes["ID"].InnerText;
                lowerInvariant = xmlNodes.Attributes["LoginName"].InnerText.ToLowerInvariant();
                this.m_UserMap[innerText] = lowerInvariant;
                this.m_ReverseUserMap[lowerInvariant] = innerText;
            }
        }

        private string BuildWebStructureUpdate(XmlNode additionsAndUpdatesNode, XmlNode deletionsNode,
            XmlNode currentWebStructure, bool bWatchForIsVisibleChanges, Dictionary<Guid, string> pagesLibGuidMap,
            ref List<Guid> hiddenGlobalNodes, ref List<Guid> hiddenCurrentNodes, out bool bHiddenChangesMade)
        {
            bHiddenChangesMade = false;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("method=put+web+struct&service_name=&changes=[");
            if (additionsAndUpdatesNode != null)
            {
                int num = 1;
                foreach (XmlNode childNode in additionsAndUpdatesNode.ChildNodes)
                {
                    if (!this.GetNavNodeExists(childNode, currentWebStructure))
                    {
                        stringBuilder.Append(this.BuildNavNodeAddition(childNode, currentWebStructure, ref num,
                            bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                            ref bHiddenChangesMade, false, false));
                    }
                    else
                    {
                        stringBuilder.Append(this.BuildNavNodeUpdate(childNode, currentWebStructure, ref num,
                            bWatchForIsVisibleChanges, pagesLibGuidMap, ref hiddenGlobalNodes, ref hiddenCurrentNodes,
                            ref bHiddenChangesMade));
                    }
                }
            }

            if (deletionsNode != null)
            {
                foreach (XmlNode xmlNodes in deletionsNode.ChildNodes)
                {
                    stringBuilder.Append(this.BuildNavNodeDeletion(xmlNodes));
                }
            }

            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public string CatalogDocumentToStoragePointFileShareEndpoint(string sNetworkPath, string sListID,
            string sFolder, string sListItemXml, AddDocumentOptions options)
        {
            throw new NotSupportedException("This method is not supported on NWS connections");
        }

        public override void CheckConnection()
        {
            if (!base.CredentialsAreDefault)
            {
                if (this.Credentials.Password.IsNullOrEmpty())
                {
                    throw new UnauthorizedAccessException("A password is required");
                }
            }

            try
            {
                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                }

                SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.Url);
                if (webServiceForSiteData == null)
                {
                    throw new Exception(string.Format("Could not connect to remote SharePoint server: '{0}'",
                        this.Url));
                }

                string text = null;
                string text2 = webServiceForSiteData.Url.Substring(this.Url.Length);
                string sSiteCollectionUrl;
                string text3;
                try
                {
                    webServiceForSiteData.GetSiteAndWeb(this.Url, out sSiteCollectionUrl, out text3);
                }
                catch (WebException ex)
                {
                    HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
                    if (!Utils.ResponseIsRedirect(httpWebResponse))
                    {
                        throw;
                    }

                    string text4 = httpWebResponse.Headers["Location"];
                    if (text4 == null)
                    {
                        throw;
                    }

                    if (!text4.EndsWith(text2))
                    {
                        throw;
                    }

                    text = text4;
                    sSiteCollectionUrl = null;
                    text3 = null;
                }

                if (text != null)
                {
                    string text5 = text.Substring(0, text.Length - text2.Length);
                    webServiceForSiteData.Url = text;
                    webServiceForSiteData.GetSiteAndWeb(text5, out sSiteCollectionUrl, out text3);
                    base.SetUrlForRedirect(text5);
                }

                if (base.SharePointVersion.IsSharePoint2013OrLater)
                {
                    throw new NotSupportedException(Properties.Resources.SharePoint2013_Detected);
                }

                this.m_sSiteCollectionUrl = sSiteCollectionUrl;
                this.m_sUrl = text3;
                this.m_sServerRelativeUrl = text3.Substring(this.Server.Length);
                if (string.IsNullOrEmpty(this.m_sServerRelativeUrl))
                {
                    this.m_sServerRelativeUrl = "/";
                }

                _sWebMetadata sWebMetadata = null;
                _sWebWithTime[] array = null;
                _sListWithTime[] array2 = null;
                _sFPUrl[] array3 = null;
                string text6;
                string[] array4;
                string[] array5;
                webServiceForSiteData.GetWeb(out sWebMetadata, out array, out array2, out array3, out text6, out array4,
                    out array5);
                this.m_sWebID = NWSAdapter.GetJustGUID(sWebMetadata.WebID);
                if (base.SharePointVersion.IsSharePoint2003)
                {
                    this.m_sIsPortal2003 = this.CheckIsPortal2003();
                }

                try
                {
                    string sSiteId;
                    webServiceForSiteData.GetSiteUrl(this.Url, out sSiteCollectionUrl, out sSiteId);
                    this.m_sSiteId = sSiteId;
                    SharePointVersion arg_1C5_0 = base.SharePointVersion;
                    this.ServerAdapterConfiguration.Load();
                }
                catch
                {
                }

                return;
            }
            catch (Exception ex2)
            {
                if (ex2 is UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException("Access Denied");
                }

                throw;
            }
        }

        private void CheckInFinalVersion(XmlNode listXML, string sVersionString, string sVersionComment,
            string sFullFilePath, string sFilePath, string checkInType, bool bCheckedOutManually, bool bAdded,
            ListsService listsSvs)
        {
            VersionsService webServiceForVersions = this.GetWebServiceForVersions();
            if (!bool.Parse(listXML.Attributes["EnableVersioning"].Value) || bCheckedOutManually || !bAdded)
            {
                listsSvs.CheckInFile(sFullFilePath, sVersionComment, checkInType);
                return;
            }

            string str = null;
            XmlNodeList xmlNodeLists =
                XmlUtility.RunXPathQuery(webServiceForVersions.GetVersions(sFilePath), ".//sp:result");
            float single = -1f;
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                string value = xmlNodes.Attributes["version"].Value;
                if (value.StartsWith("@"))
                {
                    char[] chrArray = new char[] { '@' };
                    value = value.Split(chrArray)[1];
                }

                if (single == -1f)
                {
                    str = value;
                    single = float.Parse(str);
                }
                else
                {
                    float single1 = float.Parse(value);
                    if (single1 >= single)
                    {
                        continue;
                    }

                    str = value;
                    single = single1;
                }
            }

            if (sVersionString == str || !bool.Parse(listXML.Attributes["EnableMinorVersion"].Value) &&
                sVersionString.StartsWith("0.") && str == "1.0")
            {
                listsSvs.CheckInFile(sFullFilePath, sVersionComment, "2");
                return;
            }

            listsSvs.CheckInFile(sFullFilePath, sVersionComment, checkInType);
            webServiceForVersions.DeleteVersion(sFilePath, str);
        }

        private bool CheckIsPortal2003()
        {
            bool flag;
            try
            {
                Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService webServiceForAreas =
                    this.GetWebServiceForAreas();
                webServiceForAreas.GetSubAreas(new Guid(this.WebID));
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (!(exception is WebException))
                {
                    flag = (!(exception is SoapException) ? false : true);
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }

        private void ClearAttendeesList(string sWebUrl, int[] instanceIDsToClear, ListsService listsWebService)
        {
            this.ClearAttendeesList(sWebUrl, null, instanceIDsToClear, listsWebService);
        }

        private void ClearAttendeesList(string sWebUrl, string sAttendeesListID, int[] instanceIDsToClear,
            ListsService listsWebService)
        {
            if (sAttendeesListID == null)
            {
                string str = null;
                this.GetAttendeesList(listsWebService, out sAttendeesListID, out str);
            }

            string listItems = this.GetListItems(sAttendeesListID, null,
                "<Fields><Field Name=\"ID\" /><Field Name=\"InstanceID\" /></Fields>", "", true,
                ListItemQueryType.ListItem, null, new GetListItemOptions());
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(listItems);
            List<int> nums = new List<int>();
            if (instanceIDsToClear != null)
            {
                int[] numArray = instanceIDsToClear;
                for (int i = 0; i < (int)numArray.Length; i++)
                {
                    int num = numArray[i];
                    XmlNodeList xmlNodeLists =
                        xmlDocument.DocumentElement.SelectNodes(string.Concat(".//ListItem[@InstanceID='", num, "']"));
                    foreach (XmlNode xmlNodes in xmlNodeLists)
                    {
                        nums.Add(int.Parse(xmlNodes.Attributes["ID"].Value));
                    }
                }
            }
            else
            {
                foreach (XmlNode xmlNodes1 in xmlDocument.DocumentElement.SelectNodes(".//ListItem"))
                {
                    nums.Add(int.Parse(xmlNodes1.Attributes["ID"].Value));
                }
            }

            foreach (int num1 in nums)
            {
                this.DeleteItem(sAttendeesListID, num1, sWebUrl);
            }
        }

        private void ClearUserMap()
        {
            this.m_UserMap = null;
            this.m_ReverseUserMap = null;
        }

        public override SharePointAdapter Clone()
        {
            SharePointAdapter sharePointAdapter;
            NWSAdapter nWSAdapter = null;
            try
            {
                nWSAdapter = new NWSAdapter();
                nWSAdapter.CloneFrom(this, true);
                sharePointAdapter = nWSAdapter;
            }
            catch (Exception exception)
            {
                return nWSAdapter;
            }

            return sharePointAdapter;
        }

        public override SharePointAdapter CloneForNewSiteCollection()
        {
            SharePointAdapter sharePointAdapter;
            NWSAdapter nWSAdapter = null;
            try
            {
                nWSAdapter = new NWSAdapter();
                nWSAdapter.CloneFrom(this, false);
                sharePointAdapter = nWSAdapter;
            }
            catch (Exception exception)
            {
                return nWSAdapter;
            }

            return sharePointAdapter;
        }

        private void CloneFrom(NWSAdapter adapter, bool bIncludeSiteCollectionSpecificProperties)
        {
            this.m_sUrl = adapter.m_sUrl;
            this.m_sServerRelativeUrl = adapter.m_sServerRelativeUrl;
            this.IsReadOnlyAdapter = adapter.IsReadOnlyAdapter;
            this.m_sIsPortal2003 = adapter.m_sIsPortal2003;
            this.m_credentials = adapter.m_credentials;
            base.IsDataLimitExceededForContentUnderMgmt = adapter.IsDataLimitExceededForContentUnderMgmt;
            base.SetSystemInfo(adapter.SystemInfo.Clone());
            base.SetSharePointVersion(adapter.SharePointVersion.Clone());
            this.AdapterProxy = adapter.AdapterProxy;
            this.IncludedCertificates = adapter.IncludedCertificates;
            if (bIncludeSiteCollectionSpecificProperties)
            {
                this.m_sWebID = adapter.m_sWebID;
                this.m_sSiteId = adapter.m_sSiteId;
                this.CookieManager = adapter.CookieManager;
                this.m_GlobalWebGuidDict = adapter.GlobalWebGuidDictionary;
                this.m_sSiteCollectionUrl = adapter.m_sSiteCollectionUrl;
                if (adapter.m_UserMap == null)
                {
                    adapter.BuildUserMap();
                }

                if (adapter.m_GroupMap == null)
                {
                    adapter.BuildGroupMap();
                }

                this.m_UserMap = adapter.m_UserMap;
                this.m_ReverseUserMap = adapter.m_ReverseUserMap;
                this.m_GroupMap = adapter.m_GroupMap;
                this.m_ReverseGroupMap = adapter.m_ReverseGroupMap;
                this.m_sSiteCollectionWebID = adapter.SiteCollectionWebID;
                this.m_sTaxonomyListID = adapter.TaxonomyListID;
                this.AuthenticationInitializer = adapter.AuthenticationInitializer;
            }
        }

        public string CloseWebParts(string sWebPartPageServerRelativeUrl)
        {
            string webPartsOnPage = this.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
            if (string.IsNullOrEmpty(webPartsOnPage))
            {
                return string.Empty;
            }

            XmlNodeList xmlNodeLists =
                XmlUtility.StringToXmlNode(webPartsOnPage).SelectNodes("./*[local-name() = 'WebPart']");
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            string str = Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl);
            ListsService webServiceForLists = this.GetWebServiceForLists();
            bool flag = false;
            if (xmlNodeLists.Count > 0)
            {
                flag = webServiceForLists.CheckOutFile(str, "true", null);
            }

            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                Guid guid = (xmlNodes.Attributes["ID"] != null
                    ? new Guid(xmlNodes.Attributes["ID"].Value)
                    : Guid.Empty);
                if (guid == Guid.Empty)
                {
                    continue;
                }

                string str1 = null;
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("*[local-name() = 'ZoneID']") ??
                                    xmlNodes.SelectSingleNode(".//*[local-name() = 'property'][@name='ZoneID']");
                str1 = (xmlNodes1 == null ? "" : xmlNodes1.InnerText);
                string str2 = null;
                XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("*[local-name() = 'TypeName']");
                if (xmlNodes2 == null)
                {
                    xmlNodes2 = xmlNodes.SelectSingleNode(
                        ".//*[local-name() = 'metaData']/*[local-name() = 'type']/@name");
                    if (xmlNodes2 != null)
                    {
                        int num = xmlNodes2.InnerText.IndexOf(",");
                        xmlNodes2.InnerText = (num >= 0 ? xmlNodes2.InnerText.Substring(0, num) : xmlNodes2.InnerText);
                    }
                }

                str2 = (xmlNodes2 == null ? "" : xmlNodes2.InnerText);
                if (str1 == "MeetingSummary" || str1 == "MeetingNavigator" ||
                    str2 == "Microsoft.SharePoint.Meetings.CustomToolPaneManager" ||
                    str2 == "Microsoft.SharePoint.Meetings.PageTabsWebPart" ||
                    str2 == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" &&
                    base.SharePointVersion.IsSharePoint2007OrEarlier || this.IsViewWebPart(str2, xmlNodes))
                {
                    continue;
                }

                string outerXml = null;
                XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./*[local-name() = 'IsIncluded']");
                if (xmlNodes3 != null && xmlNodes3.InnerText.ToLower() != "false")
                {
                    xmlNodes3.InnerText = "false";
                    outerXml = xmlNodes.OuterXml;
                }

                if (string.IsNullOrEmpty(outerXml))
                {
                    continue;
                }

                string str3 = this.Server.TrimEnd(new char[] { '/' });
                char[] chrArray = new char[] { '/' };
                string str4 = string.Concat(str3, "/", sWebPartPageServerRelativeUrl.TrimStart(chrArray));
                try
                {
                    webServiceForWebParts.SaveWebPart2(str4, guid, outerXml, Storage.Shared, false);
                }
                catch
                {
                    try
                    {
                        webServiceForWebParts.SaveWebPart(str4, guid, outerXml, Storage.Shared);
                    }
                    catch (SoapException soapException1)
                    {
                        SoapException soapException = soapException1;
                        string soapError = RPCUtil.GetSoapError(soapException.Detail);
                        throw new Exception(
                            string.Concat("The web parts on the page '", str4, "' could not be closed. Message: ",
                                soapError), soapException);
                    }
                }
            }

            if (flag)
            {
                try
                {
                    webServiceForLists.CheckInFile(str, "Web parts closed on page before web part copying.", "2");
                }
                catch
                {
                    webServiceForLists.CheckInFile(str,
                        "Checkout required in order to close web parts on page before copying source web parts.", "0");
                }
            }

            return string.Empty;
        }

        public string ConfigureStoragePointFileShareEndpointAndProfile(string sNetworkPath, string sSharePointPath)
        {
            throw new NotSupportedException("This method is not supported on NWS connections");
        }

        private string ConvertWebNavigationToRpcString(string sUrl, XmlNode xWebNode)
        {
            bool flag;
            bool flag1;
            bool flag2;
            if (xWebNode == null)
            {
                return string.Empty;
            }

            string str = "";
            int num = -1;
            bool flag3 = false;
            string[] strArrays = new string[]
            {
                "NavigationSortAscending", "InheritCurrentNavigation", "NavigationShowSiblings",
                "DisplayShowHideRibbonActionId"
            };
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str1 = strArrays[i];
                if (XmlUtility.GetBooleanAttributeFromXml(xWebNode, str1, out flag3))
                {
                    string str2 = str;
                    string[] value = new string[] { str2, "__", str1, ";SW|", xWebNode.Attributes[str1].Value, ";" };
                    str = string.Concat(value);
                }
            }

            string[] strArrays1 = new string[]
            {
                "NavigationOrderingMethod", "NavigationAutomaticSortingMethod", "GlobalDynamicChildLimit",
                "CurrentDynamicChildLimit"
            };
            for (int j = 0; j < (int)strArrays1.Length; j++)
            {
                string str3 = strArrays1[j];
                if (XmlUtility.GetIntegerAttributeFromXml(xWebNode, str3, out num))
                {
                    string str4 = str;
                    string[] value1 = new string[] { str4, "__", str3, ";IW|", xWebNode.Attributes[str3].Value, ";" };
                    str = string.Concat(value1);
                }
            }

            bool? nullable = null;
            bool? nullable1 = null;
            bool? nullable2 = null;
            bool? nullable3 = null;
            if (XmlUtility.GetBooleanAttributeFromXml(xWebNode, "IncludePagesInCurrentNavigation", out flag))
            {
                nullable = new bool?(flag);
            }

            if (XmlUtility.GetBooleanAttributeFromXml(xWebNode, "IncludeSubSitesInCurrentNavigation", out flag))
            {
                nullable1 = new bool?(flag);
            }

            if (XmlUtility.GetBooleanAttributeFromXml(xWebNode, "IncludePagesInGlobalNavigation", out flag))
            {
                nullable2 = new bool?(flag);
            }

            if (XmlUtility.GetBooleanAttributeFromXml(xWebNode, "IncludeSubSitesInGlobalNavigation", out flag))
            {
                nullable3 = new bool?(flag);
            }

            if (!base.SharePointVersion.IsSharePoint2010OrLater)
            {
                if (!nullable.HasValue || !nullable.Value)
                {
                    flag1 = (!nullable2.HasValue ? false : nullable2.Value);
                }
                else
                {
                    flag1 = true;
                }

                bool flag4 = flag1;
                if (!nullable1.HasValue || !nullable1.Value)
                {
                    flag2 = (!nullable3.HasValue ? false : nullable3.Value);
                }
                else
                {
                    flag2 = true;
                }

                bool flag5 = flag2;
                if (nullable.HasValue || nullable2.HasValue)
                {
                    object obj = str;
                    object[] objArray = new object[] { obj, "__IncludePagesInNavigation;SW|", flag4, ";" };
                    str = string.Concat(objArray);
                }

                if (nullable1.HasValue || nullable3.HasValue)
                {
                    object obj1 = str;
                    object[] objArray1 = new object[] { obj1, "__IncludeSubSitesInNavigation;SW|", flag5, ";" };
                    str = string.Concat(objArray1);
                }
            }
            else
            {
                int num1 = 0;
                int num2 = 0;
                if (!nullable.HasValue || !nullable2.HasValue || !nullable1.HasValue || !nullable3.HasValue)
                {
                    Hashtable hashtables = this.RPCProperties(sUrl);
                    if (hashtables.ContainsKey("CurrentNavigationIncludeTypes") &&
                        !int.TryParse(hashtables["CurrentNavigationIncludeTypes"].ToString(), out num2))
                    {
                        num2 = 0;
                    }

                    if (hashtables.ContainsKey("GlobalNavigationIncludeTypes") &&
                        !int.TryParse(hashtables["GlobalNavigationIncludeTypes"].ToString(), out num1))
                    {
                        num1 = 0;
                    }
                }

                if (nullable.HasValue)
                {
                    num2 = num2 & 1 | (nullable.Value ? 2 : 0);
                }

                if (nullable1.HasValue)
                {
                    num2 = num2 & 2 | (nullable1.Value ? 1 : 0);
                }

                if (nullable2.HasValue)
                {
                    num1 = num1 & 1 | (nullable2.Value ? 2 : 0);
                }

                if (nullable3.HasValue)
                {
                    num1 = num1 & 2 | (nullable3.Value ? 1 : 0);
                }

                if (nullable2.HasValue || nullable3.HasValue)
                {
                    object obj2 = str;
                    object[] objArray2 = new object[] { obj2, "__GlobalNavigationIncludeTypes;IW|", num1, ";" };
                    str = string.Concat(objArray2);
                }

                if (nullable.HasValue || nullable1.HasValue)
                {
                    object obj3 = str;
                    object[] objArray3 = new object[] { obj3, "__CurrentNavigationIncludeTypes;IW|", num2, ";" };
                    str = string.Concat(objArray3);
                }
            }

            return str;
        }

        public string ConvertWSFields(XmlNodeList fieldNodes)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Fields");
            Hashtable hashtables = new Hashtable();
            ArrayList arrayLists = new ArrayList();
            foreach (XmlNode fieldNode in fieldNodes)
            {
                if (hashtables.ContainsKey(fieldNode.Attributes["Name"].Value))
                {
                    continue;
                }

                xmlTextWriter.WriteStartElement("Field");
                foreach (XmlAttribute attribute in fieldNode.Attributes)
                {
                    xmlTextWriter.WriteAttributeString(attribute.Name, attribute.Value);
                }

                xmlTextWriter.WriteEndElement();
                hashtables.Add(fieldNode.Attributes["Name"].Value, "");
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        private XmlNode CorrectConnectionNodes(XmlNode wppNode)
        {
            return XmlUtility.StringToXmlNode(this.CorrectConnectionNodes(wppNode.OuterXml));
        }

        private string CorrectConnectionNodes(string wppOuterXml)
        {
            wppOuterXml = wppOuterXml.Replace("<ConnectionID>", "<ConnectionID xmlns=\"\">");
            wppOuterXml = wppOuterXml.Replace("<Connections>", "<Connections xmlns=\"\">");
            return wppOuterXml;
        }

        public string CorrectDefaultPageVersions(string sListID, string sFolder, string sListItemXML)
        {
            return string.Empty;
        }

        private string CreateDeleteItemCAML(string sItemID, string sFileRef)
        {
            string str = string.Concat("<Method ID='1' Cmd='Delete'><Field Name='ID'>", sItemID, "</Field>");
            if (sFileRef != null)
            {
                str = string.Concat(str, "<Field Name='FileRef'>", sFileRef, "</Field>");
            }

            str = string.Concat(str, "</Method>");
            return str;
        }

        private void CreateItemsAtInstanceIDs(string sListID, string sListVersion, string[] IDsToCreate,
            ListsService listsWebService)
        {
            if (IDsToCreate == null || (int)IDsToCreate.Length == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(sListVersion))
            {
                XmlNode list = listsWebService.GetList(sListID);
                sListVersion = list.Attributes["Version"].Value;
            }

            string[] dsToCreate = IDsToCreate;
            for (int i = 0; i < (int)dsToCreate.Length; i++)
            {
                string str = dsToCreate[i];
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlTextWriter.WriteStartElement("Batch");
                xmlTextWriter.WriteAttributeString("OnError", "Continue");
                xmlTextWriter.WriteAttributeString("ListVersion", "1");
                xmlTextWriter.WriteStartElement("Method");
                xmlTextWriter.WriteAttributeString("ID", "1");
                xmlTextWriter.WriteAttributeString("Cmd", "New");
                xmlTextWriter.WriteStartElement("Field");
                xmlTextWriter.WriteAttributeString("Name", "Title");
                xmlTextWriter.WriteString("Temp");
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteStartElement("Field");
                xmlTextWriter.WriteAttributeString("Name", "InstanceID");
                xmlTextWriter.WriteString(str);
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(stringBuilder.ToString());
                if (XmlUtility.RunXPathQuery(listsWebService.UpdateListItems(sListID, xmlDocument.DocumentElement),
                        ".//z:row/@ows_ID").Count == 0)
                {
                    throw new Exception("Failed to create temporary list items.");
                }
            }
        }

        private int CreateMeetingInstance(MeetingsService meetingsService, string sOrganizerEmail, string sEventUID,
            DateTime dateStamp, string sTitle, string sLocation, DateTime dateStart, DateTime dateEnd)
        {
            return this.CreateMeetingInstance(this.Url, meetingsService, sOrganizerEmail, sEventUID, dateStamp, sTitle,
                sLocation, dateStart, dateEnd);
        }

        private int CreateMeetingInstance(string sWebURL, MeetingsService meetingsService, string sOrganizerEmail,
            string sEventUID, DateTime dateStamp, string sTitle, string sLocation, DateTime dateStart, DateTime dateEnd)
        {
            if (meetingsService == null)
            {
                meetingsService = this.GetWebServiceForMeetings(sWebURL);
            }

            XmlNode xmlNodes = meetingsService.AddMeeting(sOrganizerEmail, sEventUID, 0, dateStamp, sTitle, sLocation,
                dateStart, dateEnd, false);
            XmlAttribute itemOf = xmlNodes.Attributes["Url"];
            if (itemOf == null)
            {
                throw new Exception("Failed to add meeting instance. No Url value retuned");
            }

            string value = itemOf.Value;
            int length = value.IndexOf("?InstanceID=");
            if (length < 0)
            {
                throw new Exception("Failed to add meeting isntance. Url not returned in recognized format.");
            }

            length += "?InstanceID=".Length;
            value = value.Substring(length);
            return int.Parse(value);
        }

        private void CreateRecurringMeeting(string meetingSeriesListID, XmlNode meetingInstance,
            Dictionary<string, string> ListTitleToIDMap, MeetingsService meetingsService, string sWebUrl)
        {
            meetingsService.AddMeetingFromICal("",
                "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nMETHOD:REQUEST\r\nBEGIN:VEVENT\r\nCLASS:PUBLIC\r\nCREATED:20100127T191621Z\r\nDTEND:20100127T120000\r\nDTSTAMP:20100127T191621Z\r\nDTSTART:20100127T113000\r\nLAST-MODIFIED:20100127T191621Z\r\nRRULE:FREQ=DAILY;COUNT=10;INTERVAL=3\r\nSEQUENCE:0\r\nSUMMARY;LANGUAGE=en-ca:\r\nTRANSP:OPAQUE\r\nUID:040000008200E00074C5B7101A82E0080000000090A49B27429FCA01000000000000000\r\n\t010000000DD1635B97E26DF44ADD2AA1A1371D2AE\r\nEND:VEVENT\r\nEND:VCALENDAR");
            string listItemIDs =
                this.GetListItemIDs(meetingSeriesListID, "", true, ListItemQueryType.ListItem, sWebUrl);
            if (listItemIDs != null)
            {
                char[] chrArray = new char[] { ',' };
                string[] strArrays = listItemIDs.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    if (str != "1")
                    {
                        this.DeleteItem(meetingSeriesListID, int.Parse(str), sWebUrl);
                    }
                }
            }

            this.ReCombineEventUID(meetingInstance, ListTitleToIDMap);
            if (meetingInstance.Attributes["Duration"] != null && meetingInstance.Attributes["EventDate"] != null &&
                meetingInstance.Attributes["EndDate"] != null)
            {
                int num = int.Parse(meetingInstance.Attributes["Duration"].Value);
                DateTime dateTime = Utils.ParseDate(meetingInstance.Attributes["EventDate"].Value, this.TimeZone);
                DateTime dateTime1 = Utils.ParseDate(meetingInstance.Attributes["EndDate"].Value, this.TimeZone);
                DateTime dateTime2 = dateTime.Add(new TimeSpan(0, 0, num));
                dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime2.Hour,
                    dateTime2.Minute, dateTime2.Second);
                meetingInstance.Attributes["EndDate"].Value = Utils.FormatDateToUTC(dateTime2, this.TimeZone);
            }

            if (meetingInstance.Attributes["EndDate"] != null)
            {
                DateTime dateTime3 = Utils.ParseDate(meetingInstance.Attributes["EndDate"].Value, this.TimeZone);
                if (dateTime3.Year > 8900)
                {
                    dateTime3 = new DateTime(8900, 12, 31, dateTime3.Hour, dateTime3.Minute, dateTime3.Second);
                    meetingInstance.Attributes["EndDate"].Value = Utils.FormatDateToUTC(dateTime3, this.TimeZone);
                }
            }

            this.AddListItem(meetingSeriesListID, "", meetingInstance.OuterXml, null, null, null,
                new AddListItemOptions(), sWebUrl, true);
        }

        public string DeleteAllAudiences(string inputXml)
        {
            return string.Empty;
        }

        private void DeleteAllMultiValueLookupColumns(string sListID, ListsService listsSvs)
        {
            try
            {
                XmlNode list = listsSvs.GetList(sListID);
                XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(list,
                    ".//sp:Fields/sp:Field[@Type=\"UserMulti\" or @Type=\"LookupMulti\"]");
                if (xmlNodeLists.Count != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder(1000);
                    XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    int num = 1;
                    xmlTextWriter.WriteStartElement("Fields");
                    foreach (XmlNode xmlNodes in xmlNodeLists)
                    {
                        xmlTextWriter.WriteStartElement("Method");
                        xmlTextWriter.WriteAttributeString("ID", num.ToString());
                        xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                        xmlTextWriter.WriteEndElement();
                        num++;
                    }

                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.Flush();
                    listsSvs.UpdateList(sListID, null, null, null, XmlUtility.StringToXmlNode(stringBuilder.ToString()),
                        list.Attributes["Version"].Value);
                }
            }
            catch
            {
            }
        }

        private void DeleteAllTemporaryVersions(string sRelativePath)
        {
            VersionsService webServiceForVersions = this.GetWebServiceForVersions();
            string str = "- To be deleted";
            foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(webServiceForVersions.GetVersions(sRelativePath),
                         "//sp:result"))
            {
                try
                {
                    string value = xmlNodes.Attributes["version"].Value;
                    if (!value.StartsWith("@"))
                    {
                        if (xmlNodes.Attributes["comments"].Value.Contains(str))
                        {
                            webServiceForVersions.DeleteVersion(sRelativePath, value);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public string DeleteAudience(string sAudienceName)
        {
            return string.Empty;
        }

        public string DeleteContentTypes(string sListID, string[] contentTypeIDs)
        {
            Dictionary<string, Exception> strs = new Dictionary<string, Exception>();
            if (!string.IsNullOrEmpty(sListID))
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                string[] strArrays = contentTypeIDs;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    try
                    {
                        webServiceForLists.DeleteContentType(sListID, str);
                    }
                    catch (Exception exception)
                    {
                        strs.Add(str, exception);
                    }
                }
            }
            else
            {
                WebsService webServiceForWebs = this.GetWebServiceForWebs();
                string[] strArrays1 = contentTypeIDs;
                for (int j = 0; j < (int)strArrays1.Length; j++)
                {
                    string str1 = strArrays1[j];
                    try
                    {
                        webServiceForWebs.DeleteContentType(str1);
                    }
                    catch (Exception exception1)
                    {
                        strs.Add(str1, exception1);
                    }
                }
            }

            if (strs.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Failed to delete the following content types:\r\n");
                foreach (KeyValuePair<string, Exception> keyValuePair in strs)
                {
                    string message = keyValuePair.Value.Message;
                    SoapException value = keyValuePair.Value as SoapException;
                    if (value != null)
                    {
                        message = string.Concat(message, " ", value.Detail.OuterXml);
                    }

                    stringBuilder.Append(string.Concat(keyValuePair.Key, ": ", message, "\r\n"));
                }

                throw new Exception(stringBuilder.ToString());
            }

            return string.Empty;
        }

        public string DeleteFolder(string sListID, int iListItemID, string sFolder)
        {
            try
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                string str = iListItemID.ToString();
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
                xmlNodes.InnerXml = string.Concat("<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>", str,
                    "</Value></Eq></Where>");
                XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
                xmlNodes1.InnerXml = "<FieldRef Name='FileRef'/>";
                XmlNode xmlNodes2 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                xmlNodes2.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><Folder></Folder><MeetingInstanceID>-2</MeetingInstanceID>";
                XmlNode listItems =
                    webServiceForLists.GetListItems(sListID, null, xmlNodes, xmlNodes1, null, xmlNodes2, null);
                XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(listItems, "//z:row");
                string str1 = null;
                foreach (XmlNode xmlNodes3 in xmlNodeLists)
                {
                    string innerText = xmlNodes3.Attributes["ows_FileRef"].InnerText;
                    char[] chrArray = new char[] { '#' };
                    str1 = innerText.Split(chrArray)[1];
                }

                if (str1 == null)
                {
                    throw new Exception("Delete Failed! Could not find FileRef.");
                }

                XmlElement xmlElement = (new XmlDocument()).CreateElement("Batch");
                xmlElement.SetAttribute("OnError", "Continue");
                xmlElement.SetAttribute("ListVersion", "1");
                string[] strArrays = new string[]
                {
                    "<Method ID='1' Cmd='Delete'><Field Name='ID'>", str, "</Field><Field Name='FileRef'>", str1,
                    "</Field></Method>"
                };
                xmlElement.InnerXml = string.Concat(strArrays);
                XmlNode xmlNodes4 = webServiceForLists.UpdateListItems(sListID, xmlElement);
                if (xmlNodes4.InnerText != "0x00000000")
                {
                    throw new Exception(string.Concat("Delete Failed! ", xmlNodes4.InnerText));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return string.Empty;
        }

        public string DeleteGroup(string sGroupName)
        {
            this.GetWebServiceForUserGroup().RemoveGroup(sGroupName);
            this.BuildGroupMap();
            return string.Empty;
        }

        public string DeleteItem(string sListID, int iListItemID)
        {
            try
            {
                this.DeleteItem(sListID, iListItemID, this.Url);
            }
            catch
            {
                throw new Exception("Could not delete item from list.");
            }

            return string.Empty;
        }

        private void DeleteItem(string sListID, int iListItemID, string sWebUrl)
        {
            try
            {
                ListsService webServiceForLists = this.GetWebServiceForLists(sWebUrl);
                string str = iListItemID.ToString();
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
                xmlNodes.InnerXml = string.Concat("<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>", str,
                    "</Value></Eq></Where>");
                XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
                xmlNodes1.InnerXml = "<FieldRef Name='FileRef'/>";
                XmlNode xmlNodes2 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                xmlNodes2.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><Folder></Folder><MeetingInstanceID>-2</MeetingInstanceID>";
                XmlNode listItems =
                    webServiceForLists.GetListItems(sListID, null, xmlNodes, xmlNodes1, null, xmlNodes2, null);
                XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(listItems, "//z:row");
                string str1 = null;
                foreach (XmlNode xmlNodes3 in xmlNodeLists)
                {
                    if (xmlNodes3.Attributes["ows_FileRef"] == null)
                    {
                        continue;
                    }

                    string value = xmlNodes3.Attributes["ows_FileRef"].Value;
                    char[] chrArray = new char[] { '#' };
                    str1 = value.Split(chrArray)[1];
                }

                XmlElement xmlElement = (new XmlDocument()).CreateElement("Batch");
                xmlElement.SetAttribute("OnError", "Continue");
                xmlElement.SetAttribute("ListVersion", "1");
                xmlElement.InnerXml = this.CreateDeleteItemCAML(iListItemID.ToString(), str1);
                XmlNode xmlNodes4 = webServiceForLists.UpdateListItems(sListID, xmlElement);
                if (xmlNodes4.InnerText != "0x00000000")
                {
                    throw new Exception(string.Concat("Delete Failed! ", xmlNodes4.InnerText));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public string DeleteItems(string sListID, bool bDeleteAllItems, string sIDs)
        {
            this.DeleteItems(sListID, bDeleteAllItems, sIDs, this.Url);
            return string.Empty;
        }

        public void DeleteItems(string sListID, bool bDeleteAllItems, string sIDs, string sWebUrl)
        {
            string str;
            string str1;
            try
            {
                string[] strArrays = null;
                if (!bDeleteAllItems)
                {
                    strArrays = sIDs.Split(new char[] { ',' });
                }
                else
                {
                    string str2 = sListID;
                    if (bDeleteAllItems)
                    {
                        str1 = null;
                    }
                    else
                    {
                        str1 = sIDs;
                    }

                    string listItems = this.GetListItems(str2, str1,
                        "<Fields><Field Name=\"ID\" /><Field Name=\"FileRef\" /></Fields>", null, true,
                        ListItemQueryType.ListItem | ListItemQueryType.Folder, null, new GetListItemOptions());
                    XmlNodeList xmlNodeLists =
                        XmlUtility.RunXPathQuery(XmlUtility.StringToXmlNode(listItems), "//ListItem");
                    strArrays = new string[xmlNodeLists.Count];
                    for (int i = 0; i < xmlNodeLists.Count; i++)
                    {
                        XmlNode itemOf = xmlNodeLists[i];
                        strArrays[xmlNodeLists.Count - 1 - i] = string.Concat(itemOf.Attributes["ID"].Value,
                            (itemOf.Attributes["FileRef"] != null
                                ? string.Concat("#", itemOf.Attributes["FileRef"].Value)
                                : ""));
                    }
                }

                if ((int)strArrays.Length > 0)
                {
                    int num = 5000;
                    int num1 = 0;
                    while (num1 < (int)strArrays.Length)
                    {
                        try
                        {
                            XmlElement xmlElement = (new XmlDocument()).CreateElement("Batch");
                            xmlElement.SetAttribute("OnError", "Continue");
                            xmlElement.SetAttribute("ListVersion", "1");
                            StringBuilder stringBuilder = new StringBuilder(512);
                            for (int j = num1; j < num1 + num && j < (int)strArrays.Length; j++)
                            {
                                string str3 = strArrays[j];
                                int num2 = str3.IndexOf('#');
                                string str4 = (num2 >= 0 ? str3.Substring(0, num2) : str3);
                                if (num2 >= 0)
                                {
                                    str = str3.Substring(num2 + 1);
                                }
                                else
                                {
                                    str = null;
                                }

                                stringBuilder.Append(this.CreateDeleteItemCAML(str4, str));
                            }

                            xmlElement.InnerXml = stringBuilder.ToString();
                            XmlNode xmlNodes = this.GetWebServiceForLists(sWebUrl).UpdateListItems(sListID, xmlElement);
                            if (xmlNodes.InnerText.Replace("0x00000000", "").Length > 0)
                            {
                                throw new Exception(string.Concat("Delete Failed! ", xmlNodes.InnerText));
                            }

                            num1 += num;
                        }
                        catch (WebException webException)
                        {
                            if (num < 500)
                            {
                                throw;
                            }

                            num /= 2;
                        }
                    }
                }
            }
            catch (SoapException soapException)
            {
                string soapError = RPCUtil.GetSoapError(soapException.Detail);
                string[] strArrays1 = new string[]
                {
                    "An error occurred deleting an item from list with ID '", sListID, "' on web '", sWebUrl,
                    "'. Error: ", soapError
                };
                throw new Exception(string.Concat(strArrays1));
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public string DeleteList(string sListID)
        {
            try
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                if (base.SharePointVersion.IsSharePoint2007)
                {
                    this.DeleteAllMultiValueLookupColumns(sListID, webServiceForLists);
                }

                try
                {
                    webServiceForLists.DeleteList(sListID);
                }
                catch (SoapException soapException1)
                {
                    SoapException soapException = soapException1;
                    string soapExceptionErrorCode = this.GetSoapExceptionErrorCode(soapException);
                    if (soapExceptionErrorCode == null || !(soapExceptionErrorCode == "0x80070024"))
                    {
                        if (soapExceptionErrorCode != null && soapExceptionErrorCode == "0x82000006" ||
                            soapException.Detail.InnerText.Contains("0x80070002"))
                        {
                            throw new Exception(string.Concat("The list with ID: '", sListID,
                                "' does not exist. The page you selected contains a list that does not exist. It may have been deleted by another user. "));
                        }

                        if (soapExceptionErrorCode != null && soapExceptionErrorCode == "0x8102003d")
                        {
                            throw new Exception(string.Format("The list with ID: '{0}' cannot be deleted.", sListID));
                        }

                        throw;
                    }
                    else
                    {
                        this.DeleteItems(sListID, true, null);
                        webServiceForLists.DeleteList(sListID);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return string.Empty;
        }

        private void DeleteListItem(string sItemID, string sListName, ListsService listsService)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Batch");
            xmlTextWriter.WriteAttributeString("OnError", "Continue");
            xmlTextWriter.WriteAttributeString("ListVersion", "1");
            xmlTextWriter.WriteStartElement("Method");
            xmlTextWriter.WriteAttributeString("ID", "1");
            xmlTextWriter.WriteAttributeString("Cmd", "Delete");
            xmlTextWriter.WriteStartElement("Field");
            xmlTextWriter.WriteAttributeString("Name", "ID");
            xmlTextWriter.WriteString(sItemID);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            XmlNode xmlNodes = listsService.UpdateListItems(sListName, xmlDocument.DocumentElement);
            if (xmlNodes.InnerText != "0x00000000")
            {
                throw new Exception(string.Concat("Failed to delete existing item! ", xmlNodes.InnerText));
            }
        }

        private void DeleteMeetingInstanceFolders(string sListID)
        {
            int num;
            string listItems = this.GetListItems(sListID, null,
                "<Fields><Field Name=\"ID\" /><Field Name=\"FileRef\" /><Field Name=\"FileLeafRef\" /><Field Name=\"FSObjType\" /></Fields>",
                null, true, ListItemQueryType.Folder, null, new GetListItemOptions());
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(XmlUtility.StringToXmlNode(listItems), "//ListItem");
            StringBuilder stringBuilder = new StringBuilder(100);
            bool flag = true;
            for (int i = 0; i < xmlNodeLists.Count; i++)
            {
                if (!(xmlNodeLists[i].Attributes["FSObjType"].Value != "1") &&
                    int.TryParse(xmlNodeLists[i].Attributes["FileLeafRef"].Value, out num))
                {
                    if (!flag)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append(xmlNodeLists[i].Attributes["ID"].Value);
                    stringBuilder.Append("#");
                    stringBuilder.Append(xmlNodeLists[i].Attributes["FileRef"].Value);
                    flag = false;
                }
            }

            string str = stringBuilder.ToString();
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            this.DeleteItems(sListID, false, str);
        }

        private void DeleteRecurringMeetingInstance(MeetingsService meetingsService, XmlNode meetingInstance,
            string sEventUID, DateTime createdDate)
        {
            uint num;
            XmlAttribute itemOf = meetingInstance.Attributes["InstanceID"];
            if (itemOf == null)
            {
                return;
            }

            if (!uint.TryParse(itemOf.Value, NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture,
                    out num))
            {
                return;
            }

            meetingsService.RemoveMeeting(num, sEventUID, 1, createdDate, true);
        }

        public string DeleteRole(string sRoleName)
        {
            string empty;
            UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
            try
            {
                webServiceForUserGroup.GetRoleInfo(sRoleName);
                this.BreakRoleDefinitionInheritance();
                webServiceForUserGroup.RemoveRole(sRoleName);
                return string.Empty;
            }
            catch
            {
                empty = string.Empty;
            }

            return empty;
        }

        public string DeleteRoleAssignment(string sPrincipalName, bool bIsGroup, string sRoleName, string sListID,
            int iItemId)
        {
            if (!bIsGroup)
            {
                sPrincipalName = this.EnsureCorrectUserFormat(sPrincipalName);
            }

            this.BreakRoleInheritance(sListID, iItemId);
            if (!string.IsNullOrEmpty(sListID))
            {
                this.DeleteRoleAssignmentViaHttpPost(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
            }
            else
            {
                UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
                if (bIsGroup)
                {
                    if (sRoleName != null)
                    {
                        webServiceForUserGroup.RemoveGroupFromRole(sRoleName, sPrincipalName);
                    }
                    else
                    {
                        this.DeleteRoleAssignmentViaHttpPost(sPrincipalName, bIsGroup, sRoleName, sListID, iItemId);
                    }
                }
                else if (sRoleName != null)
                {
                    webServiceForUserGroup.RemoveUserFromRole(sRoleName, sPrincipalName);
                }
                else
                {
                    webServiceForUserGroup.RemoveUserFromWeb(sPrincipalName);
                }
            }

            return string.Empty;
        }

        private void DeleteRoleAssignmentViaHttpPost(string sPrincipalName, bool bIsGroup, string sRoleName,
            string sListID, int iItemId)
        {
            string permissionsUrl = this.GetPermissionsUrl(sListID, iItemId, "editprms");
            string str = null;
            str = (!bIsGroup ? this.GetIDFromUser(sPrincipalName) : this.GetIDFromGroup(sPrincipalName));
            permissionsUrl = string.Concat(permissionsUrl, "&sel=", str);
            XmlDocument xmlDocument = new XmlDocument();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            string value = null;
            string str1 = null;
            string sharePointHttpPostParameters = null;
            if (sRoleName == null)
            {
                permissionsUrl = this.GetPermissionsUrl(sListID, iItemId, "user");
                strs.Add("ctl00$PlaceHolderMain$hdnPrincipalsToDelete", str);
                strs.Add("ctl00$PlaceHolderMain$hdnOperation", "deleteUser");
                sharePointHttpPostParameters =
                    this.GetSharePointHttpPostParameters(this.HttpGet(permissionsUrl), "<input.*/>", strs);
            }
            else
            {
                str1 = this.HttpGet(permissionsUrl);
                string str2 = str1.Replace("/><", "/>\n<").Replace("</label><br", "</label>\n<br");
                foreach (Match match in (new Regex(
                             "<label for=\"ctl00_PlaceHolderMain_ctl01_ctl00_cblRoles_[0-9]+\".*/label>"))
                         .Matches(str2))
                {
                    xmlDocument.LoadXml(match.Value);
                    string innerText = xmlDocument.FirstChild.InnerText;
                    if (innerText.Substring(0, innerText.IndexOf('-')).Trim() != sRoleName)
                    {
                        continue;
                    }

                    value = xmlDocument.FirstChild.Attributes["for"].Value;
                    break;
                }

                foreach (Match match1 in (new Regex(
                             "<input id=\"ctl00_PlaceHolderMain_ctl01_ctl00_cblRoles_[0-9]+\".*/>")).Matches(str2))
                {
                    xmlDocument.LoadXml(match1.Value);
                    if ((xmlDocument.FirstChild.Attributes["checked"] == null
                            ? true
                            : xmlDocument.FirstChild.Attributes["checked"].Value.ToLower() != "checked"))
                    {
                        continue;
                    }

                    string value1 = xmlDocument.FirstChild.Attributes["id"].Value;
                    if (value1 == value)
                    {
                        continue;
                    }

                    strs.Add(value1.Replace("_", "$"), "on");
                }

                if (strs.Count <= 0)
                {
                    permissionsUrl = this.GetPermissionsUrl(sListID, iItemId, "user");
                    strs.Add("ctl00$PlaceHolderMain$hdnPrincipalsToDelete", str);
                    strs.Add("ctl00$PlaceHolderMain$hdnOperation", "deleteUser");
                    sharePointHttpPostParameters =
                        this.GetSharePointHttpPostParameters(this.HttpGet(permissionsUrl), "<input.*/>", strs);
                }
                else
                {
                    strs.Add("__EVENTTARGET", "ctl00$PlaceHolderMain$ctl02$RptControls$btnOK");
                    strs.Add("__EVENTARGUMENT", "");
                    sharePointHttpPostParameters = this.GetSharePointHttpPostParameters(str1, "<input.*__.*/>", strs);
                }
            }

            this.HttpPost(permissionsUrl, sharePointHttpPostParameters);
        }

        public string DeleteSiteCollection(string sSiteURL, string sWebApp)
        {
            throw new NotImplementedException();
        }

        private void DeleteTempDocumentVersions(string sRelativePath, string[] tempVersions)
        {
            VersionsService webServiceForVersions = this.GetWebServiceForVersions();
            string[] strArrays = tempVersions;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                try
                {
                    webServiceForVersions.DeleteVersion(sRelativePath, str);
                }
                catch
                {
                }
            }
        }

        public string DeleteWeb(string sServerRelativeUrl)
        {
            this.m_SubWebGuidDict = null;
            this.DeleteWebByFullUrl(string.Concat(this.Server, sServerRelativeUrl));
            return string.Empty;
        }

        private void DeleteWebByFullUrl(string sUrl)
        {
            int? item;
            foreach (XmlNode childNode in this.GetWebServiceForWebs(sUrl).GetWebCollection().ChildNodes)
            {
                this.DeleteWebByFullUrl(childNode.Attributes["Url"].Value);
            }

            try
            {
                RPCUtil.DeleteWeb(this, sUrl);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (!exception.Data.Contains("Status") || !(exception.Data["Status"] is int))
                {
                    item = null;
                }
                else
                {
                    item = (int?)(exception.Data["Status"] as int?);
                }

                int? nullable = item;
                if (!nullable.HasValue || nullable.Value != 1966171)
                {
                    throw exception;
                }

                XmlNode listCollection = this.GetWebServiceForLists(sUrl).GetListCollection();
                foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(listCollection, "//sp:List"))
                {
                    if (xmlNodes.Attributes["AllowDeletion"] == null || !xmlNodes.Attributes["AllowDeletion"].Value
                            .Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    this.DeleteItems(xmlNodes.Attributes["ID"].Value, true, null, sUrl);
                    this.DeleteList(xmlNodes.Attributes["ID"].Value);
                }

                RPCUtil.DeleteWeb(this, sUrl);
            }
        }

        public string DeleteWebPart(string sWebPartPageServerRelativeUrl, string sWebPartId)
        {
            if (string.IsNullOrEmpty(sWebPartId))
            {
                return string.Empty;
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                object[] guids = new object[] { sWebPartPageServerRelativeUrl, null, null };
                Guid[] guid = new Guid[] { new Guid(sWebPartId) };
                guids[1] = new List<Guid>(guid);
                guids[2] = this;
                this.ExecuteClientOMMethod("DeleteWebParts", guids);
                return string.Empty;
            }

            string webPartsOnPage = this.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
            if (string.IsNullOrEmpty(webPartsOnPage))
            {
                return string.Empty;
            }

            XmlNodeList xmlNodeLists =
                XmlUtility.StringToXmlNode(webPartsOnPage).SelectNodes("./*[local-name() = 'WebPart']");
            string str = Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl);
            ListsService webServiceForLists = this.GetWebServiceForLists();
            bool flag = false;
            if (xmlNodeLists.Count > 0)
            {
                flag = webServiceForLists.CheckOutFile(str, "true", null);
            }

            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                Guid guid1 =
                    (xmlNodes.Attributes["ID"] != null ? new Guid(xmlNodes.Attributes["ID"].Value) : Guid.Empty);
                if (!string.Equals(guid1.ToString(), sWebPartId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
                string str1 = this.Server.TrimEnd(new char[] { '/' });
                char[] chrArray = new char[] { '/' };
                string str2 = string.Concat(str1, "/", sWebPartPageServerRelativeUrl.TrimStart(chrArray));
                try
                {
                    webServiceForWebParts.DeleteWebPart(str2, guid1, Storage.Shared);
                    break;
                }
                catch (SoapException soapException1)
                {
                    SoapException soapException = soapException1;
                    string soapError = RPCUtil.GetSoapError(soapException.Detail);
                    string[] strArrays = new string[]
                    {
                        "The web part (GUID: ", guid1.ToString(), ") on the page '", str2,
                        "' could not be deleted. Message: ", soapError
                    };
                    throw new Exception(string.Concat(strArrays), soapException);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    string[] strArrays1 = new string[]
                    {
                        "The web part (GUID: ", guid1.ToString(), ") on the page '", str2,
                        "' could not be deleted. Message: ", exception.Message
                    };
                    throw new Exception(string.Concat(strArrays1), exception);
                }
            }

            if (flag)
            {
                try
                {
                    webServiceForLists.CheckInFile(str, "Web part deleted.", "2");
                }
                catch
                {
                    webServiceForLists.CheckInFile(str,
                        "Checkout required in order to delete web parts on page before copying source web parts.", "0");
                }
            }

            return string.Empty;
        }

        public string DeleteWebParts(string sWebPartPageServerRelativeUrl)
        {
            string webPartsOnPage = this.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
            if (string.IsNullOrEmpty(webPartsOnPage))
            {
                return string.Empty;
            }

            XmlNodeList xmlNodeLists =
                XmlUtility.StringToXmlNode(webPartsOnPage).SelectNodes("./*[local-name() = 'WebPart']");
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            string str = Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl);
            ListsService webServiceForLists = this.GetWebServiceForLists();
            bool flag = false;
            if (xmlNodeLists.Count > 0)
            {
                flag = webServiceForLists.CheckOutFile(str, "true", null);
            }

            List<Guid> guids = new List<Guid>();
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                Guid guid = (xmlNodes.Attributes["ID"] != null
                    ? new Guid(xmlNodes.Attributes["ID"].Value)
                    : Guid.Empty);
                if (guid == Guid.Empty)
                {
                    continue;
                }

                string str1 = null;
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("*[local-name() = 'ZoneID']") ??
                                    xmlNodes.SelectSingleNode(".//*[local-name() = 'property'][@name='ZoneID']");
                str1 = (xmlNodes1 == null ? "" : xmlNodes1.InnerText);
                string str2 = null;
                XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("*[local-name() = 'TypeName']");
                if (xmlNodes2 == null)
                {
                    xmlNodes2 = xmlNodes.SelectSingleNode(
                        ".//*[local-name() = 'metaData']/*[local-name() = 'type']/@name");
                    if (xmlNodes2 != null)
                    {
                        int num = xmlNodes2.InnerText.IndexOf(",");
                        xmlNodes2.InnerText = (num >= 0 ? xmlNodes2.InnerText.Substring(0, num) : xmlNodes2.InnerText);
                    }
                }

                str2 = (xmlNodes2 == null ? "" : xmlNodes2.InnerText);
                if (str1 == "MeetingSummary" || str1 == "MeetingNavigator" ||
                    str2 == "Microsoft.SharePoint.Meetings.CustomToolPaneManager" ||
                    str2 == "Microsoft.SharePoint.Meetings.PageTabsWebPart" ||
                    str2 == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" &&
                    base.SharePointVersion.IsSharePoint2007OrEarlier || this.IsViewWebPart(str2, xmlNodes))
                {
                    continue;
                }

                if (!base.SharePointVersion.IsSharePoint2007OrEarlier)
                {
                    guids.Add(guid);
                }
                else
                {
                    string str3 = this.Server.TrimEnd(new char[] { '/' });
                    char[] chrArray = new char[] { '/' };
                    string str4 = string.Concat(str3, "/", sWebPartPageServerRelativeUrl.TrimStart(chrArray));
                    try
                    {
                        webServiceForWebParts.DeleteWebPart(str4, guid, Storage.Shared);
                    }
                    catch (SoapException soapException1)
                    {
                        SoapException soapException = soapException1;
                        string soapError = RPCUtil.GetSoapError(soapException.Detail);
                        throw new Exception(
                            string.Concat("The web parts on the page '", str4, "' could not be deleted. Message: ",
                                soapError), soapException);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        throw new Exception(
                            string.Concat("The web parts on the page '", str4, "' could not be deleted. Message: ",
                                exception.Message), exception);
                    }
                }
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater && guids.Count > 0)
            {
                object[] objArray = new object[] { sWebPartPageServerRelativeUrl, guids, this };
                this.ExecuteClientOMMethod("DeleteWebParts", objArray);
            }

            if (flag)
            {
                try
                {
                    webServiceForLists.CheckInFile(str, "Web parts deleted on page before web part copying.", "2");
                }
                catch
                {
                    webServiceForLists.CheckInFile(str,
                        "Checkout required in order to delete web parts on page before copying source web parts.", "0");
                }
            }

            return string.Empty;
        }

        public string DisableValidationSettings(string listID)
        {
            throw new NotImplementedException();
        }

        private byte[] DownloadDocumentToByteArray(string sFileDirRef, string sFileLeafRef, bool bWebRelative,
            bool bAttemptSpecialDownloadUrl)
        {
            byte[] numArray;
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                }

                CookieAwareWebClient webClient = this.GetWebClient();
                if (bAttemptSpecialDownloadUrl)
                {
                    string documentDownloadUrl =
                        this.GetDocumentDownloadUrl(sFileDirRef, sFileLeafRef, bWebRelative, true);
                    byte[] numArray1 = webClient.DownloadData(documentDownloadUrl);
                    if (!string.IsNullOrEmpty(webClient.ResponseHeaders["Content-Disposition"]))
                    {
                        numArray = numArray1;
                        return numArray;
                    }
                }

                string str = this.GetDocumentDownloadUrl(sFileDirRef, sFileLeafRef, bWebRelative, false);
                numArray = webClient.DownloadData(str);
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return numArray;
        }

        private byte[] DownloadURLToByteArray(string sUrl)
        {
            byte[] numArray;
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                }

                numArray = this.GetWebClient().DownloadData(sUrl);
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return numArray;
        }

        private string DownloadURLToString(string sUrl)
        {
            string str;
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                }

                str = this.GetWebClient().DownloadString(sUrl);
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return str;
        }

        public string EnableValidationSettings(string validationNodeFieldsXml)
        {
            throw new NotImplementedException();
        }

        private string EnsureCorrectUserFormat(string sUserName)
        {
            string str;
            if (base.SharePointVersion.IsSharePoint2007OrEarlier || string.IsNullOrEmpty(sUserName))
            {
                return sUserName;
            }

            if (this.m_ReverseUserMap == null)
            {
                this.BuildUserMap();
            }

            string lowerInvariant = sUserName.ToLowerInvariant();
            if (this.m_ReverseUserMap.ContainsKey(lowerInvariant))
            {
                return sUserName;
            }

            if (Utils.SwitchUserNameFormat(lowerInvariant, out str) && this.m_ReverseUserMap.ContainsKey(str))
            {
                return str;
            }

            return sUserName;
        }

        private string EnsureWebRelativeFileDirRef(string sFileDirRef)
        {
            string str = sFileDirRef.TrimStart(new char[] { '/' });
            string serverRelativeUrl = this.ServerRelativeUrl;
            char[] chrArray = new char[] { '/' };
            if (UrlUtils.ReplaceStart(str, serverRelativeUrl.TrimStart(chrArray), string.Empty, out str))
            {
                sFileDirRef = str;
            }

            return sFileDirRef;
        }

        protected object ExecuteClientOMMethod(string sMethodName, object[] parameters)
        {
            object obj;
            if (!this.ClientOMAvailable)
            {
                throw new Exception(string.Concat("The client object model is not available. Cannot execute method \"",
                    sMethodName, "\""));
            }

            try
            {
                obj = this.ClientOMType.InvokeMember(sMethodName, BindingFlags.InvokeMethod, null, this.ClientOMObject,
                    parameters);
            }
            catch (Exception exception)
            {
                throw NWSAdapter.GetExecutedClientOMMethodException(exception);
            }

            return obj;
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            OperationReporting operationReporting = new OperationReporting();
            operationReporting.Start();
            try
            {
                try
                {
                    if (!string.IsNullOrEmpty(commandName))
                    {
                        switch (commandName.GetValueAsEnumValue<SharePointAdapterCommands>())
                        {
                            case SharePointAdapterCommands.Unknown:
                            {
                                operationReporting.LogError(
                                    string.Format(Metalogix.SharePoint.Adapters.Properties.Resources.InvalidCommandName,
                                        commandName), string.Empty, string.Empty, 0, 0);
                                break;
                            }
                            case SharePointAdapterCommands.GetListByName:
                            {
                                this.GetListByTitle(commandConfigurationXml, operationReporting);
                                break;
                            }
                            default:
                            {
                                operationReporting.LogError(null,
                                    string.Format(
                                        Metalogix.SharePoint.Adapters.Properties.Resources.CommandNotImplemented,
                                        commandName, this.AdapterShortName));
                                break;
                            }
                        }
                    }
                    else
                    {
                        operationReporting.LogError(
                            Metalogix.SharePoint.Adapters.Properties.Resources.CommandNameIsNull, string.Empty,
                            string.Empty, 0, 0);
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    operationReporting.LogError(exception,
                        string.Format(Metalogix.SharePoint.Adapters.Properties.Resources.ErrorInCommandExecution,
                            commandName, this.AdapterShortName));
                }
            }
            finally
            {
                operationReporting.End();
            }

            return operationReporting.ResultXml;
        }

        private string ExtractDocumentGuid(string sWebServiceResponse)
        {
            string str = "DocGUID";
            string str1 = string.Format("<li>\\s*vti_rtag\\s*<li>\\s*SW\\s*\\|\\s*rt\\s*:\\s*(?<{0}>[\\w-]*)[@\\r\\n]",
                str);
            Match match = (new Regex(str1, RegexOptions.IgnoreCase)).Match(sWebServiceResponse);
            if (!match.Success)
            {
                return null;
            }

            return match.Groups[str].Value;
        }

        private void FetchAndWritePortalListingTerseData(Guid listingID, XmlWriter writer,
            Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService areaService)
        {
            AreaListingData areaListingData = areaService.GetAreaListingData(listingID);
            writer.WriteStartElement("Listing");
            writer.WriteAttributeString("ListingID", areaListingData.ListingID.ToString());
            writer.WriteAttributeString("Group", this.GetPortalListingGroupFromID(areaListingData.GroupID));
            writer.WriteAttributeString("Order", areaListingData.Order.ToString());
            writer.WriteAttributeString("Modified",
                Utils.FormatDate(Utils.MakeTrueUTCDateTime(areaListingData.LastModified)));
            writer.WriteEndElement();
        }

        private XmlNodeList FetchListFields(string sListID, ListsService listsService)
        {
            return XmlUtility.RunXPathQuery(listsService.GetList(sListID), ".//sp:Fields/sp:Field");
        }

        private XmlNodeList FetchSiteColumns(WebsService websService)
        {
            return XmlUtility.RunXPathQuery(websService.GetColumns(), ".//sp:Fields/sp:Field");
        }

        private void FillFolderXML(XmlWriter xmlWriter, XmlAttributeCollection itemAttribs, string sFileDirRef,
            string sListID)
        {
            try
            {
                xmlWriter.WriteStartElement("Folder");
                xmlWriter.WriteAttributeString("ContentTypeId",
                    (itemAttribs["ows_ContentTypeId"] != null
                        ? itemAttribs["ows_ContentTypeId"].InnerText
                        : string.Empty));
                xmlWriter.WriteAttributeString("ID", itemAttribs["ows_ID"].InnerText);
                DateTime dateTime = DateTime.Parse(itemAttribs["ows_Modified"].InnerText, null,
                    DateTimeStyles.AssumeUniversal);
                xmlWriter.WriteAttributeString("Modified", Utils.FormatDate(dateTime.ToUniversalTime()));
                DateTime dateTime1 = DateTime.Parse(itemAttribs["ows_Created"].InnerText, null,
                    DateTimeStyles.AssumeUniversal);
                xmlWriter.WriteAttributeString("Created", Utils.FormatDate(dateTime1.ToUniversalTime()));
                try
                {
                    string innerText = itemAttribs["ows_Author"].InnerText;
                    int num = innerText.IndexOf('#');
                    if (num > 0)
                    {
                        innerText = innerText.Substring(0, num - 1);
                    }

                    innerText = this.GetUserFromID(innerText);
                    if (innerText == null)
                    {
                        string str = itemAttribs["ows_Author"].InnerText;
                        char[] chrArray = new char[] { '#' };
                        innerText = str.Split(chrArray)[1];
                    }

                    xmlWriter.WriteAttributeString("CreatedBy", innerText);
                }
                catch
                {
                }

                try
                {
                    string userFromID = itemAttribs["ows_Editor"].InnerText;
                    int num1 = userFromID.IndexOf('#');
                    if (num1 > 0)
                    {
                        userFromID = userFromID.Substring(0, num1 - 1);
                    }

                    userFromID = this.GetUserFromID(userFromID);
                    if (userFromID == null)
                    {
                        string innerText1 = itemAttribs["ows_Editor"].InnerText;
                        char[] chrArray1 = new char[] { '#' };
                        userFromID = innerText1.Split(chrArray1)[1];
                    }

                    xmlWriter.WriteAttributeString("ModifiedBy", userFromID);
                }
                catch
                {
                }

                xmlWriter.WriteAttributeString("owshiddenversion", itemAttribs["ows_owshiddenversion"].InnerText);
                xmlWriter.WriteAttributeString("_ModerationStatus", itemAttribs["ows__ModerationStatus"].InnerText);
                if (base.SharePointVersion.IsSharePoint2007OrLater)
                {
                    xmlWriter.WriteAttributeString("MetaInfo", itemAttribs["ows_MetaInfo"].InnerText);
                    xmlWriter.WriteAttributeString("_Level", itemAttribs["ows__Level"].InnerText);
                    xmlWriter.WriteAttributeString("UniqueId", itemAttribs["ows_UniqueId"].InnerText);
                }

                xmlWriter.WriteAttributeString("FSObjType", itemAttribs["ows_FSObjType"].InnerText);
                xmlWriter.WriteAttributeString("FileRef", itemAttribs["ows_FileRef"].InnerText);
                string str1 = itemAttribs["ows_FileLeafRef"].InnerText;
                char[] chrArray2 = new char[] { '#' };
                xmlWriter.WriteAttributeString("FileLeafRef", str1.Split(chrArray2)[1]);
                xmlWriter.WriteAttributeString("FileDirRef", sFileDirRef);
                xmlWriter.WriteEndElement();
            }
            catch (Exception exception)
            {
            }
        }

        public void FillItemXML(XmlWriter xmlWriter, XmlAttributeCollection itemAttribs, IEnumerable nodesField,
            string sListID, GetListItemOptions getOptions, bool bWriteAttributesOnly)
        {
            DateTime dateTime;
            string value;
            string str;
            string value1;
            string str1;
            if (!bWriteAttributesOnly)
            {
                xmlWriter.WriteStartElement("ListItem");
            }

            string value2 = null;
            if (getOptions != null && getOptions.IncludeExternalizationData)
            {
                xmlWriter.WriteAttributeString("IsExternalized", false.ToString());
                xmlWriter.WriteAttributeString("BinaryAvailable", true.ToString());
            }

            if (getOptions != null && getOptions.IncludePermissionsInheritance)
            {
                xmlWriter.WriteAttributeString("HasUniquePermissions",
                    this.HasUniquePermissions(sListID, int.Parse(itemAttribs["ows_ID"].Value)).ToString());
            }

            foreach (XmlNode xmlNodes in nodesField)
            {
                value2 = xmlNodes.Attributes["Name"].Value;
                string str2 = XmlUtility.EncodeNameStartChars(value2);
                bool flag = (xmlNodes.Attributes["FromBaseType"] == null
                    ? false
                    : xmlNodes.Attributes["FromBaseType"].Value == "TRUE");
                if (xmlNodes.Attributes["Type"] != null)
                {
                    value = xmlNodes.Attributes["Type"].Value;
                }
                else
                {
                    value = null;
                }

                string str3 = value;
                try
                {
                    if (itemAttribs[string.Concat("ows_", value2)] != null)
                    {
                        str = itemAttribs[string.Concat("ows_", value2)].Value;
                    }
                    else
                    {
                        str = null;
                    }

                    string justGUID = str;
                    if (string.IsNullOrEmpty(justGUID))
                    {
                        if (itemAttribs[value2] != null)
                        {
                            str1 = itemAttribs[value2].Value;
                        }
                        else
                        {
                            str1 = null;
                        }

                        justGUID = str1;
                    }

                    if (string.IsNullOrEmpty(justGUID))
                    {
                        justGUID = "";
                    }
                    else if (value2 == "UniqueId")
                    {
                        justGUID = NWSAdapter.GetJustGUID(justGUID);
                    }
                    else if (str3 == "User")
                    {
                        if (xmlNodes.Attributes["ForcedDisplay"] == null ||
                            !(xmlNodes.Attributes["ForcedDisplay"].Value == "***"))
                        {
                            int num = justGUID.IndexOf('#');
                            if (num > 0)
                            {
                                justGUID = justGUID.Substring(0, num - 1);
                            }

                            justGUID = this.GetUserFromID(justGUID);
                            if (justGUID == null)
                            {
                                if (itemAttribs[string.Concat("ows_", value2)] != null)
                                {
                                    string value3 = itemAttribs[string.Concat("ows_", value2)].Value;
                                    char[] chrArray = new char[] { '#' };
                                    value1 = value3.Split(chrArray)[1];
                                }
                                else
                                {
                                    value1 = itemAttribs[value2].Value;
                                }

                                justGUID = value1;
                            }
                        }
                        else
                        {
                            justGUID = "";
                        }
                    }
                    else if (str3 == "UserMulti")
                    {
                        string str4 = justGUID.ToString();
                        string[] strArrays = new string[] { ";#" };
                        string[] strArrays1 = str4.Split(strArrays, StringSplitOptions.None);
                        int num1 = -1;
                        if (int.TryParse(strArrays1[0], out num1))
                        {
                            string str5 = null;
                            for (int i = 0; i < (int)strArrays1.Length; i += 2)
                            {
                                string userFromID = this.GetUserFromID(strArrays1[i]);
                                if (string.IsNullOrEmpty(userFromID))
                                {
                                    userFromID = ((int)strArrays1.Length <= i + 1 ? strArrays1[i] : strArrays1[i + 1]);
                                }

                                str5 = (!string.IsNullOrEmpty(str5)
                                    ? string.Concat(str5, ",", userFromID)
                                    : userFromID);
                            }

                            justGUID = str5;
                        }
                        else
                        {
                            justGUID = strArrays1[0];
                        }
                    }
                    else if (str3 == "Lookup" && !flag)
                    {
                        int num2 = justGUID.IndexOf(";#");
                        if (num2 > 0)
                        {
                            justGUID = justGUID.Substring(0, num2);
                        }
                    }
                    else if (str3 == "LookupMulti")
                    {
                        if ((itemAttribs[string.Concat("ows_", value2)] != null ? true : false))
                        {
                            string idListFromMultiValueLookupColumn =
                                this.GetIdListFromMultiValueLookupColumn(justGUID);
                            if (!string.IsNullOrEmpty(idListFromMultiValueLookupColumn))
                            {
                                justGUID = idListFromMultiValueLookupColumn;
                            }
                        }
                    }
                    else if (str3 == "Boolean" || str3 == "Attachments")
                    {
                        justGUID = (justGUID == "0" ? "False" : "True");
                    }
                    else if (str3 == "DateTime" || str3 == "PublishingScheduleStartDateFieldType" ||
                             str3 == "PublishingScheduleEndDateFieldType")
                    {
                        DateTime dateTime1 = DateTime.Parse(justGUID, null, DateTimeStyles.AssumeUniversal);
                        justGUID = Utils.FormatDate(dateTime1.ToUniversalTime());
                    }
                    else if (str3 == "Calculated" && (justGUID.StartsWith("datetime;#") ||
                                                      Utils.TryParseDateAsUtc(justGUID, out dateTime)))
                    {
                        if (justGUID.StartsWith("datetime;#"))
                        {
                            justGUID = justGUID.Remove(0, 10);
                        }

                        if (!string.IsNullOrEmpty(justGUID))
                        {
                            DateTime dateTime2 = DateTime.Parse(justGUID, null, DateTimeStyles.AssumeUniversal);
                            justGUID = Utils.FormatDate(dateTime2.ToUniversalTime());
                        }
                    }
                    else if (str3 == "URL")
                    {
                        if (justGUID.Trim() == ",")
                        {
                            justGUID = "";
                        }
                    }
                    else if (!(str3 == "Text") && !(str3 == "Note") && !(str3 == "HTML"))
                    {
                        int num3 = justGUID.IndexOf(";#");
                        if (num3 > 0)
                        {
                            justGUID = justGUID.Substring(num3 + 2);
                        }
                    }

                    xmlWriter.WriteAttributeString(str2, justGUID);
                }
                catch
                {
                    if (xmlNodes.Attributes["Type"].Value == "Text")
                    {
                        xmlWriter.WriteAttributeString(str2, "");
                    }
                }
            }

            if (!bWriteAttributesOnly)
            {
                xmlWriter.WriteEndElement();
            }
        }

        private void FillListXML(XmlWriter xmlWriter, XmlAttributeCollection listAttribs, XmlNodeList nodesField,
            XmlNodeList nodesView, string sWebStruct, Hashtable listSettings, XmlNode externalDataSource)
        {
            string str;
            string str1;
            string str2;
            string str3;
            bool flag;
            bool flag1;
            bool flag2;
            string str4;
            xmlWriter.WriteStartElement("List");
            bool flag3 = (nodesField != null ? true : nodesView != null);
            string innerText = listAttribs["ID"].InnerText;
            string innerText1 = listAttribs["Title"].InnerText;
            this.GetDirListName(listAttribs, out str, out str1);
            this.ParseCreatedModified(listAttribs, out str2, out str3);
            xmlWriter.WriteAttributeString("ID", NWSAdapter.GetJustGUID(innerText));
            xmlWriter.WriteAttributeString("Name", str1);
            xmlWriter.WriteAttributeString("Title", innerText1);
            xmlWriter.WriteAttributeString("BaseTemplate", listAttribs["ServerTemplate"].InnerText);
            xmlWriter.WriteAttributeString("BaseType", listAttribs["BaseType"].InnerText);
            xmlWriter.WriteAttributeString("DirName", str);
            xmlWriter.WriteAttributeString("ItemCount", listAttribs["ItemCount"].InnerText);
            xmlWriter.WriteAttributeString("Description", listAttribs["Description"].InnerText);
            xmlWriter.WriteAttributeString("Created", str2);
            xmlWriter.WriteAttributeString("Modified", str3);
            xmlWriter.WriteAttributeString("EnableModeration", listAttribs["EnableModeration"].InnerText);
            xmlWriter.WriteAttributeString("Hidden",
                (listAttribs["Hidden"] != null ? listAttribs["Hidden"].Value : "False"));
            if (listAttribs["FeatureId"] != null)
            {
                string innerText2 = listAttribs["FeatureId"].InnerText;
                xmlWriter.WriteAttributeString("FeatureId",
                    (string.IsNullOrEmpty(innerText2) ? Guid.Empty.ToString() : innerText2));
            }

            if (flag3)
            {
                long num = Convert.ToInt64(listAttribs["Flags"].Value);
                int num1 = Convert.ToInt32(listAttribs["BaseType"].Value);
                if (num1 != 1 && num1 != 4)
                {
                    xmlWriter.WriteAttributeString("EnableAttachments", listAttribs["EnableAttachments"].InnerText);
                }

                if ((num & (long)128) > (long)0)
                {
                    flag1 = true;
                }
                else
                {
                    flag1 = (!base.SharePointVersion.IsSharePoint2003 ? false : num1 == 5);
                }

                bool flag4 = flag1;
                bool flag5 = (num & (long)524288) > (long)0;
                xmlWriter.WriteAttributeString("EnableVersioning", flag4.ToString());
                if (flag4)
                {
                    xmlWriter.WriteAttributeString("EnableMinorVersions",
                        (flag4 ? flag5.ToString() : false.ToString()));
                    xmlWriter.WriteAttributeString("MajorVersionLimit",
                        (listAttribs["MajorVersionLimit"] == null ||
                         string.IsNullOrEmpty(listAttribs["MajorVersionLimit"].Value)
                            ? "0"
                            : listAttribs["MajorVersionLimit"].Value));
                    if (bool.Parse(listAttribs["EnableModeration"].InnerText))
                    {
                        xmlWriter.WriteAttributeString("MajorWithMinorVersionsLimit",
                            (listAttribs["MajorWithMinorVersionsLimit"] == null ||
                             string.IsNullOrEmpty(listAttribs["MajorWithMinorVersionsLimit"].Value)
                                ? "0"
                                : listAttribs["MajorWithMinorVersionsLimit"].Value));
                    }
                    else if (flag5)
                    {
                        xmlWriter.WriteAttributeString("MajorWithMinorVersionsLimit",
                            (listAttribs["MajorWithMinorVersionsLimit"] == null ||
                             string.IsNullOrEmpty(listAttribs["MajorWithMinorVersionsLimit"].Value)
                                ? "0"
                                : listAttribs["MajorWithMinorVersionsLimit"].Value));
                    }
                }

                if (listAttribs["AllowMultiResponses"] != null)
                {
                    string value = listAttribs["AllowMultiResponses"].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        xmlWriter.WriteAttributeString("AllowMultiResponses", value);
                    }
                }

                if (listAttribs["ShowUser"] != null)
                {
                    string value1 = listAttribs["ShowUser"].Value;
                    if (!string.IsNullOrEmpty(value1))
                    {
                        xmlWriter.WriteAttributeString("ShowUser", value1);
                    }
                }

                if (listAttribs["EmailAlias"] != null && !string.IsNullOrEmpty(listAttribs["EmailAlias"].InnerText))
                {
                    xmlWriter.WriteAttributeString("EmailAlias", listAttribs["EmailAlias"].InnerText);
                }

                if (listAttribs["EnableAssignedToEmail"] != null)
                {
                    string value2 = listAttribs["EnableAssignedToEmail"].Value;
                    if (!string.IsNullOrEmpty(value2))
                    {
                        xmlWriter.WriteAttributeString("EnableAssignToEmail", value2);
                    }
                }

                if (listAttribs["ReadSecurity"] != null)
                {
                    xmlWriter.WriteAttributeString("ReadSecurity", listAttribs["ReadSecurity"].InnerText);
                }

                if (listAttribs["WriteSecurity"] != null)
                {
                    xmlWriter.WriteAttributeString("WriteSecurity", listAttribs["WriteSecurity"].InnerText);
                }

                if (num1 != 1)
                {
                    flag2 = false;
                }
                else
                {
                    flag2 = (int.Parse(listAttribs["ServerTemplate"].Value) == 101
                        ? true
                        : int.Parse(listAttribs["ServerTemplate"].Value) == 115);
                }

                if (flag2 && listAttribs["DocTemplateUrl"] != null)
                {
                    string str5 = (string.IsNullOrEmpty(listAttribs["DocTemplateUrl"].Value)
                        ? ""
                        : listAttribs["DocTemplateUrl"].Value);
                    if (!string.IsNullOrEmpty(str5) && !str5.StartsWith("/"))
                    {
                        string str6 = this.ServerRelativeUrl.Trim(new char[] { '/' });
                        char[] chrArray = new char[] { '/' };
                        str5 = string.Concat("/", str6, "/", str5.TrimStart(chrArray));
                    }

                    xmlWriter.WriteAttributeString("DocTemplateUrl", str5);
                }

                bool listOnQuickLaunch = false;
                if (!base.SharePointVersion.IsSharePoint2010OrLater)
                {
                    listOnQuickLaunch = this.GetListOnQuickLaunch(listAttribs, sWebStruct);
                    xmlWriter.WriteAttributeString("OnQuickLaunch", listOnQuickLaunch.ToString());
                }
                else
                {
                    string listPropertiesClientOM = this.GetListPropertiesClientOM(innerText1);
                    if (listPropertiesClientOM != null)
                    {
                        XmlNode xmlNode = XmlUtility.StringToXmlNode(listPropertiesClientOM);
                        xmlWriter.WriteAttributeString("OnQuickLaunch", xmlNode.Attributes["OnQuickLaunch"].Value);
                    }
                }

                bool flag6 = false;
                bool.TryParse(this.HasUniquePermissions(innerText, -1), out flag6);
                xmlWriter.WriteAttributeString("HasUniquePermissions", flag6.ToString());
                bool flag7 = false;
                if (listAttribs["MultipleDataList"] == null ||
                    !bool.TryParse(listAttribs["MultipleDataList"].Value, out flag7))
                {
                    xmlWriter.WriteAttributeString("MultipleDataList", "False");
                }
                else
                {
                    xmlWriter.WriteAttributeString("MultipleDataList", flag7.ToString());
                }

                if (num1 == 1 || num1 == 4)
                {
                    xmlWriter.WriteAttributeString("BrowserEnabledDocuments",
                        ((num & (long)268435456) <= (long)0 ? "PreferClient" : "Browser"));
                    xmlWriter.WriteAttributeString("ForceCheckout",
                        (listAttribs["RequireCheckout"] != null
                            ? listAttribs["RequireCheckout"].Value
                            : false.ToString()));
                    bool flag8 = (num & (long)16) > (long)0;
                    xmlWriter.WriteAttributeString("IsCatalog", flag8.ToString());
                    if (listAttribs["SendToLocation"] != null)
                    {
                        string innerText3 = listAttribs["SendToLocation"].InnerText;
                        if (innerText3.IndexOf("|") <= 0)
                        {
                            xmlWriter.WriteAttributeString("SendToLocationName", "");
                            xmlWriter.WriteAttributeString("SendToLocationUrl", "");
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("SendToLocationName",
                                innerText3.Substring(0, innerText3.IndexOf("|")));
                            if (innerText3.IndexOf("|") + 1 <= innerText3.Length - 1)
                            {
                                xmlWriter.WriteAttributeString("SendToLocationUrl",
                                    innerText3.Substring(innerText3.IndexOf("|") + 1));
                            }
                        }
                    }
                }

                if (listSettings != null)
                {
                    if (listSettings.Contains("emailattachmentfolders"))
                    {
                        xmlWriter.WriteAttributeString("EmailAttachmentFolder",
                            listSettings["emailattachmentfolders"].ToString());
                    }

                    if (listSettings.Contains("emailoverwrite"))
                    {
                        xmlWriter.WriteAttributeString("EmailOverWrite", listSettings["emailoverwrite"].ToString());
                    }

                    if (listSettings.Contains("emailsaveoriginal"))
                    {
                        xmlWriter.WriteAttributeString("EmailSaveOriginal",
                            listSettings["emailsaveoriginal"].ToString());
                    }

                    if (listSettings.Contains("emailsavemeetings"))
                    {
                        xmlWriter.WriteAttributeString("EmailSaveMeetings",
                            listSettings["emailsavemeetings"].ToString());
                    }

                    if (listSettings.Contains("emailusesecurity"))
                    {
                        xmlWriter.WriteAttributeString("EmailUseSecurity", listSettings["emailusesecurity"].ToString());
                    }

                    if (listSettings.Contains("rss_ChannelTitle"))
                    {
                        xmlWriter.WriteAttributeString("RssChannelTitle", listSettings["rss_ChannelTitle"].ToString());
                    }

                    if (listSettings.Contains("rss_ChannelDescription"))
                    {
                        xmlWriter.WriteAttributeString("RssChannelDescription",
                            listSettings["rss_ChannelDescription"].ToString());
                    }

                    if (listSettings.Contains("rss_LimitDescriptionLength"))
                    {
                        xmlWriter.WriteAttributeString("RssLimitDescriptionLength",
                            listSettings["rss_LimitDescriptionLength"].ToString());
                    }

                    if (listSettings.Contains("rss_ChannelImageUrl"))
                    {
                        xmlWriter.WriteAttributeString("RssChannelImageUrl",
                            listSettings["rss_ChannelImageUrl"].ToString());
                    }

                    if (listSettings.Contains("rss_ItemLimit"))
                    {
                        xmlWriter.WriteAttributeString("RssItemLimit", listSettings["rss_ItemLimit"].ToString());
                    }

                    if (listSettings.Contains("rss_DayLimit"))
                    {
                        xmlWriter.WriteAttributeString("RssDayLimit", listSettings["rss_DayLimit"].ToString());
                    }

                    if (listSettings.Contains("rss_DocumentAsEnclosure"))
                    {
                        xmlWriter.WriteAttributeString("RssDocumentAsEnclosure",
                            listSettings["rss_DocumentAsEnclosure"].ToString());
                    }

                    if (listSettings.Contains("rss_DocumentAsLink"))
                    {
                        xmlWriter.WriteAttributeString("RssDocumentAsLink",
                            listSettings["rss_DocumentAsLink"].ToString());
                    }

                    if (listSettings.Contains("welcomepage"))
                    {
                        xmlWriter.WriteAttributeString("WelcomePage", listSettings["welcomepage"].ToString());
                    }
                }

                if (!base.SharePointVersion.IsSharePoint2003 || num1 == 1)
                {
                    bool flag9 = (num & (long)536870912) <= (long)0;
                    xmlWriter.WriteAttributeString("Folders", flag9.ToString());
                }
                else
                {
                    xmlWriter.WriteAttributeString("Folders", false.ToString());
                }

                if (!base.SharePointVersion.IsSharePoint2003)
                {
                    XmlWriter xmlWriter1 = xmlWriter;
                    if ((num & (long)1048576) != (long)0 || (num & (long)2097152) != (long)0)
                    {
                        str4 = ((num & (long)1048576) == (long)0 ? "2" : "1");
                    }
                    else
                    {
                        str4 = "0";
                    }

                    xmlWriter1.WriteAttributeString("DraftVersionVisibility", str4);
                }

                bool flag10 = (num & (long)4194304) > (long)0;
                xmlWriter.WriteAttributeString("ContentTypesEnabled", flag10.ToString());
                XmlUtility.GetBooleanAttributeFromXml(listAttribs, "HasExternalDataSource", out flag);
                if (externalDataSource != null && flag)
                {
                    this.WriteBCSProperties(xmlWriter, externalDataSource);
                }

                if (nodesField != null)
                {
                    xmlWriter.WriteStartElement("Fields");
                    foreach (XmlNode xmlNodes in nodesField)
                    {
                        if (base.SharePointVersion.IsSharePoint2003 && listAttribs["ServerTemplate"] != null &&
                            int.Parse(listAttribs["ServerTemplate"].Value) == 109 &&
                            xmlNodes.Attributes["Name"] != null &&
                            xmlNodes.Attributes["Name"].Value.Equals("RequiredField"))
                        {
                            continue;
                        }

                        this.AddListTitlesToLookups(xmlNodes);
                        xmlWriter.WriteRaw(
                            xmlNodes.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\"", ""));
                    }

                    xmlWriter.WriteEndElement();
                }

                if (nodesView != null)
                {
                    xmlWriter.WriteStartElement("Views");
                    ViewsService webServiceForViews = this.GetWebServiceForViews();
                    bool flag11 = false;
                    if (base.SharePointVersion.IsSharePoint2003)
                    {
                        XmlAttribute itemOf = listAttribs["BaseType"];
                        if (itemOf != null && (ListType)Enum.Parse(typeof(ListType), itemOf.Value) ==
                            ListType.DocumentLibrary)
                        {
                            flag11 = true;
                        }
                    }

                    foreach (XmlNode xmlNodes1 in nodesView)
                    {
                        XmlNode viewHtml =
                            webServiceForViews.GetViewHtml(innerText, xmlNodes1.Attributes["Name"].InnerText);
                        if (flag11)
                        {
                            this.SwapMetaInfoColumnsInViews(viewHtml);
                        }

                        if (viewHtml.Attributes["Type"] != null && viewHtml.Attributes["Type"].Value
                                .Equals("Table", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (base.SharePointVersion.IsSharePoint2003 && listAttribs["ServerTemplate"] != null &&
                            listAttribs["ServerTemplate"].Value == "106" && viewHtml.Attributes["BaseViewID"] != null &&
                            viewHtml.Attributes["BaseViewID"].Value == "2" && viewHtml.Attributes["Type"] != null)
                        {
                            viewHtml.Attributes["Type"].Value = "CALENDAR";
                        }

                        xmlWriter.WriteRaw(
                            viewHtml.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\"", ""));
                    }

                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
        }

        private void FillWebXML(XmlWriter xmlWriter, string sUrl, string sTitle, bool bFullXML,
            bool bRequiresWebId = true)
        {
            string str;
            string str1;
            string[] strArrays;
            string[] strArrays1;
            int num;
            string str2;
            string str3 = sUrl.Substring(this.Server.Length);
            if (string.IsNullOrEmpty(str3))
            {
                str3 = "/";
            }

            str = (sUrl.Length <= this.Server.Length ? "" : sUrl.Substring(sUrl.LastIndexOf('/') + 1));
            xmlWriter.WriteAttributeString("Name", str);
            if (sTitle != null)
            {
                xmlWriter.WriteAttributeString("Title", sTitle);
            }

            xmlWriter.WriteAttributeString("ServerRelativeUrl", str3);
            xmlWriter.WriteAttributeString("DatabaseServerName", "");
            xmlWriter.WriteAttributeString("DatabaseName", "");
            xmlWriter.WriteAttributeString("SiteCollectionServerRelativeUrl", this.SiteCollectionServerRelativeUrl);
            xmlWriter.WriteAttributeString("IsSiteAdmin", "True");
            xmlWriter.WriteAttributeString("IsSearchable", "False");
            if (!bFullXML && !bRequiresWebId)
            {
                return;
            }

            SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(sUrl);
            _sWebMetadata _sWebMetadatum = null;
            _sWebWithTime[] _sWebWithTimeArray = null;
            _sListWithTime[] _sListWithTimeArray = null;
            _sFPUrl[] _sFPUrlArray = null;
            webServiceForSiteData.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray, out _sListWithTimeArray,
                out _sFPUrlArray, out str1, out strArrays, out strArrays1);
            xmlWriter.WriteAttributeString("ID", NWSAdapter.GetJustGUID(_sWebMetadatum.WebID));
            if (!bFullXML)
            {
                return;
            }

            string str4 = "0";
            string str5 = "0";
            string str6 = "";
            Hashtable hashtables = this.RPCProperties(sUrl);
            string siteTheme = this.GetSiteTheme(sUrl);
            this.GetWebProperties(sUrl, hashtables, out str4, out str5, out str6);
            if (sTitle == null)
            {
                xmlWriter.WriteAttributeString("Title", _sWebMetadatum.Title);
            }

            xmlWriter.WriteAttributeString("Description", _sWebMetadatum.Description);
            if (this.m_iRegionalCulture < 0)
            {
                this.m_iRegionalCulture = this.GetLocale(sUrl, _sWebMetadatum.Language);
            }

            xmlWriter.WriteAttributeString("Locale", this.m_iRegionalCulture.ToString());
            this.m_sLanguage = (hashtables.ContainsKey("language")
                ? hashtables["language"].ToString()
                : _sWebMetadatum.Language.ToString());
            xmlWriter.WriteAttributeString("Language", this.m_sLanguage);
            try
            {
                num = (!this.Url.Equals(sUrl, StringComparison.OrdinalIgnoreCase)
                    ? this.GetWebTimeZoneFromPage(sUrl)
                    : this.TimeZone.ID);
            }
            catch
            {
                num = -1;
            }

            xmlWriter.WriteAttributeString("TimeZone", num.ToString());
            xmlWriter.WriteAttributeString("RootWebGUID", this.SiteCollectionWebID);
            xmlWriter.WriteAttributeString("RootSiteGUID", this.SiteCollectionID);
            xmlWriter.WriteAttributeString("TaxonomyListGUID", this.TaxonomyListID);
            xmlWriter.WriteAttributeString("WebTemplateID", str4);
            xmlWriter.WriteAttributeString("WebTemplateConfig", str5);
            xmlWriter.WriteAttributeString("WebTemplateName", "");
            xmlWriter.WriteAttributeString("HasUniquePermissions", (!_sWebMetadatum.InheritedSecurity).ToString());
            bool flag = this.HasUniqueRoles(sUrl);
            xmlWriter.WriteAttributeString("HasUniqueRoles", flag.ToString());
            bool requestAccessEmail =
                this.GetRequestAccessEmail((_sWebMetadatum.InheritedSecurity ? _sWebMetadatum.Permissions : sUrl),
                    out str2);
            xmlWriter.WriteAttributeString("RequestAccessEnabled", requestAccessEmail.ToString());
            xmlWriter.WriteAttributeString("RequestAccessEmail", str2);
            if (!string.IsNullOrEmpty(str6))
            {
                xmlWriter.WriteAttributeString("MasterPage", str6);
            }

            if (hashtables.ContainsKey("custommasterurl"))
            {
                xmlWriter.WriteAttributeString("CustomMasterPage", hashtables["custommasterurl"].ToString());
            }

            this.GetWebNavigationXML(hashtables, xmlWriter);
            if (hashtables.ContainsKey("rss_Copyright"))
            {
                xmlWriter.WriteAttributeString("RssCopyright", hashtables["rss_Copyright"].ToString());
            }

            if (hashtables.ContainsKey("rss_ManagingEditor"))
            {
                xmlWriter.WriteAttributeString("RssManagingEditor", hashtables["rss_ManagingEditor"].ToString());
            }

            if (hashtables.ContainsKey("rss_WebMaster"))
            {
                xmlWriter.WriteAttributeString("RssWebMaster", hashtables["rss_WebMaster"].ToString());
            }

            if (hashtables.ContainsKey("rss_TimeToLive"))
            {
                xmlWriter.WriteAttributeString("RssTimeToLive", hashtables["rss_TimeToLive"].ToString());
            }

            this.WriteAssociatedGroupData(hashtables, xmlWriter);
            if (hashtables.ContainsKey("PublishingFeatureActivated"))
            {
                xmlWriter.WriteAttributeString("PublishingFeatureActivated",
                    hashtables["PublishingFeatureActivated"].ToString());
            }

            if (!string.IsNullOrEmpty(siteTheme))
            {
                xmlWriter.WriteAttributeString("SiteTheme", siteTheme);
            }

            string empty = string.Empty;
            if (base.SharePointVersion.IsSharePoint2007)
            {
                empty = "3";
            }
            else if (this.ClientOMAvailable)
            {
                object[] objArray = new object[] { this };
                empty = (string)this.ExecuteClientOMMethod("GetUIVersion", objArray);
            }

            xmlWriter.WriteAttributeString("UIVersion", empty);
            if (base.SharePointVersion.IsSharePoint2007OrLater)
            {
                string welcomePage = this.GetWelcomePage(this.Url);
                if (welcomePage != null)
                {
                    xmlWriter.WriteAttributeString("WelcomePage", welcomePage);
                }
            }

            try
            {
                string activatedFeatures = this.GetWebServiceForWebs().GetActivatedFeatures();
                string[] strArrays2 = activatedFeatures.Split(new char[] { '\t' });
                strArrays2[0] = strArrays2[0].Substring(0, strArrays2[0].Length - 1);
                xmlWriter.WriteAttributeString("SiteFeatures", strArrays2[0]);
                strArrays2[1] = strArrays2[1].Substring(0, strArrays2[1].Length - 1);
                xmlWriter.WriteAttributeString("SiteCollFeatures", strArrays2[1]);
            }
            catch (Exception exception)
            {
                xmlWriter.WriteComment(string.Concat("Couldn't fetch site features: ", exception.Message));
            }

            if (str4 == "2")
            {
                try
                {
                    this.GetWebMeetingInstanceXML(xmlWriter);
                }
                catch (Exception exception1)
                {
                    xmlWriter.WriteComment(string.Concat("Couldn't fetch Meeting Instances: ", exception1.Message));
                }
            }
        }

        public string FindAlerts()
        {
            throw new NotImplementedException(
                "Database analysis functionality is not available with this type of connection");
        }

        public string FindUniquePermissions()
        {
            throw new NotImplementedException(
                "Database analysis functionality is not available with this type of connection");
        }

        private string Get2003IssueVersions(string sListID, int iItemID, string viewID, XmlNodeList fieldNodes,
            ListsService listsSvs)
        {
            bool flag;
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Query");
            xmlTextWriter.WriteStartElement("Where");
            xmlTextWriter.WriteStartElement("Eq");
            xmlTextWriter.WriteStartElement("FieldRef");
            xmlTextWriter.WriteAttributeString("Name", "IssueID");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("Value");
            xmlTextWriter.WriteAttributeString("Type", "Counter");
            xmlTextWriter.WriteValue(iItemID.ToString());
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("OrderBy");
            xmlTextWriter.WriteStartElement("FieldRef");
            xmlTextWriter.WriteAttributeString("Ascending", "TRUE");
            xmlTextWriter.WriteAttributeString("Name", "owshiddenversion");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(stringBuilder.ToString());
            StringBuilder stringBuilder1 = new StringBuilder();
            XmlTextWriter xmlTextWriter1 = new XmlTextWriter(new StringWriter(stringBuilder1));
            xmlTextWriter1.WriteStartElement("ViewFields");
            string[] strArrays = new string[] { "owshiddenversion", "IsCurrent" };
            xmlTextWriter1.WriteRaw(this.BuildQueryFieldsXml(fieldNodes, false, strArrays));
            xmlTextWriter1.WriteEndElement();
            XmlNode xmlNodes = XmlUtility.StringToXmlNode(stringBuilder1.ToString());
            XmlNode xmlNode1 = XmlUtility.StringToXmlNode(
                "<QueryOptions><IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><MeetingInstanceID>-2</MeetingInstanceID></QueryOptions>");
            List<XmlNode> xmlNodes1 = this.RunListItemQuery(sListID, ListType.IssuesList, xmlNode, xmlNodes, xmlNode1,
                listsSvs, out flag, viewID);
            StringBuilder stringBuilder2 = new StringBuilder();
            XmlTextWriter xmlTextWriter2 = new XmlTextWriter(new StringWriter(stringBuilder2));
            xmlTextWriter2.WriteStartElement("ListItems");
            foreach (XmlNode xmlNodes2 in xmlNodes1)
            {
                XmlAttribute itemOf = xmlNodes2.Attributes["ows_owshiddenversion"];
                if (itemOf == null)
                {
                    continue;
                }

                XmlAttribute xmlAttribute = xmlNodes2.Attributes["ows_IsCurrent"];
                if (xmlAttribute == null)
                {
                    continue;
                }

                XmlAttribute itemOf1 = xmlNodes2.Attributes["ows_Comment"];
                string str = (itemOf1 == null ? "" : itemOf1.Value);
                string str1 = string.Concat(itemOf.Value, ".0");
                xmlTextWriter2.WriteStartElement("ListItem");
                this.FillItemXML(xmlTextWriter2, xmlNodes2.Attributes, fieldNodes, sListID, new GetListItemOptions(),
                    true);
                xmlTextWriter2.WriteAttributeString("_VersionIsCurrent", xmlAttribute.Value);
                xmlTextWriter2.WriteAttributeString("_CheckinComment", str);
                xmlTextWriter2.WriteAttributeString("_VersionString", str1);
                xmlTextWriter2.WriteAttributeString("_UIVersionString", str1);
                int num = int.Parse(itemOf.Value) * 512;
                xmlTextWriter2.WriteAttributeString("_UIVersion", num.ToString());
                xmlTextWriter2.WriteAttributeString("_VersionNumber", itemOf.Value);
                xmlTextWriter2.WriteEndElement();
            }

            xmlTextWriter2.WriteEndElement();
            return stringBuilder2.ToString();
        }

        private string Get2007Groups()
        {
            string value;
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Groups");
            try
            {
                UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
                Dictionary<string, string> strs = new Dictionary<string, string>();
                XmlNodeList xmlNodeLists =
                    XmlUtility.RunXPathQuery(webServiceForUserGroup.GetUserCollectionFromSite(), "//d:User");
                string innerText = null;
                string str = null;
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    innerText = xmlNodes.Attributes["ID"].InnerText;
                    str = xmlNodes.Attributes["LoginName"].InnerText;
                    strs[innerText] = str;
                }

                XmlNodeList xmlNodeLists1 =
                    XmlUtility.RunXPathQuery(webServiceForUserGroup.GetGroupCollectionFromSite(), "//d:Group");
                foreach (XmlNode xmlNodes1 in xmlNodeLists1)
                {
                    innerText = xmlNodes1.Attributes["ID"].InnerText;
                    str = xmlNodes1.Attributes["Name"].InnerText;
                    strs[innerText] = str;
                }

                string innerText1 = null;
                string item = null;
                string str1 = null;
                foreach (XmlNode xmlNodes2 in xmlNodeLists1)
                {
                    str1 = xmlNodes2.Attributes["ID"].InnerText;
                    str = xmlNodes2.Attributes["Name"].InnerText;
                    innerText1 = xmlNodes2.Attributes["OwnerID"].InnerText;
                    item = strs[innerText1];
                    xmlTextWriter.WriteStartElement("Group");
                    xmlTextWriter.WriteAttributeString("ID", str1);
                    xmlTextWriter.WriteAttributeString("Name", str);
                    xmlTextWriter.WriteAttributeString("Description", xmlNodes2.Attributes["Description"].InnerText);
                    xmlTextWriter.WriteAttributeString("OwnerIsUser", xmlNodes2.Attributes["OwnerIsUser"].InnerText);
                    xmlTextWriter.WriteAttributeString("Owner", item);
                    xmlTextWriter.WriteAttributeString("OnlyAllowMembersViewMembership", "");
                    xmlTextWriter.WriteAttributeString("AllowMembersEditMembership", "");
                    xmlTextWriter.WriteAttributeString("AllowRequestToJoinLeave", "");
                    xmlTextWriter.WriteAttributeString("AutoAcceptRequestToJoinLeave", "");
                    xmlTextWriter.WriteAttributeString("RequestToJoinLeaveEmailSetting", "");
                    foreach (XmlNode xmlNodes3 in XmlUtility.RunXPathQuery(
                                 webServiceForUserGroup.GetUserCollectionFromGroup(str), "//d:User"))
                    {
                        if (xmlNodes3.Attributes["LoginName"] != null)
                        {
                            value = xmlNodes3.Attributes["LoginName"].Value;
                        }
                        else
                        {
                            value = null;
                        }

                        string str2 = value;
                        if (str2 == null)
                        {
                            continue;
                        }

                        xmlTextWriter.WriteStartElement("Member");
                        xmlTextWriter.WriteAttributeString("Login", str2);
                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteEndElement();
                }
            }
            finally
            {
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }

            return stringWriter.ToString();
        }

        private string Get2010Groups()
        {
            object[] objArray = new object[] { this };
            return this.ExecuteClientOMMethod("GetGroups", objArray).ToString();
        }

        public string GetAlerts(string sListID, int iItemID)
        {
            throw new NotImplementedException();
        }

        private void GetAllDayEventFields(XmlNodeList fields, XmlNode itemXml, out string sStartDate,
            out string sEndDate)
        {
            bool flag;
            sStartDate = null;
            sEndDate = null;
            XmlNode xmlNodes = null;
            foreach (XmlNode field in fields)
            {
                if (field.Attributes["Type"].Value != "AllDayEvent")
                {
                    continue;
                }

                xmlNodes = field;
                break;
            }

            if (xmlNodes == null)
            {
                return;
            }

            XmlAttribute itemOf = itemXml.Attributes[xmlNodes.Attributes["Name"].Value];
            if (itemOf == null)
            {
                return;
            }

            if (bool.TryParse(itemOf.Value, out flag))
            {
                if (!flag)
                {
                    return;
                }
            }
            else if (itemOf.Value != "1")
            {
                return;
            }

            XmlNode xmlNodes1 =
                XmlUtility.RunXPathQuerySelectSingle(xmlNodes, "//sp:FieldRefs/sp:FieldRef[@RefType=\"StartDate\"]");
            if (xmlNodes1 != null && xmlNodes1.Attributes["Name"] != null)
            {
                sStartDate = xmlNodes1.Attributes["Name"].Value;
            }

            XmlNode xmlNodes2 =
                XmlUtility.RunXPathQuerySelectSingle(xmlNodes, "//sp:FieldRefs/sp:FieldRef[@RefType=\"EndDate\"]");
            if (xmlNodes2 != null && xmlNodes2.Attributes["Name"] != null)
            {
                sEndDate = xmlNodes2.Attributes["Name"].Value;
            }
        }

        private string GetAssociateGroupIDs(string groups)
        {
            if (string.IsNullOrEmpty(groups))
            {
                return "";
            }

            string str = groups.Trim().TrimEnd(new char[] { ';' });
            char[] chrArray = new char[] { ';' };
            return this.GetAssociateGroupIDs(new List<string>(str.Split(chrArray)));
        }

        private string GetAssociateGroupIDs(List<string> groupNames)
        {
            StringBuilder stringBuilder = new StringBuilder(groupNames.Count * 5);
            foreach (string groupName in groupNames)
            {
                string dFromGroup = this.GetIDFromGroup(groupName);
                if (string.IsNullOrEmpty(dFromGroup))
                {
                    continue;
                }

                stringBuilder.AppendFormat("{0};", dFromGroup);
            }

            StringBuilder length = stringBuilder;
            length.Length = length.Length - 1;
            return stringBuilder.ToString();
        }

        private string GetAssociateGroupNames(string groups)
        {
            string str = "";
            if (!string.IsNullOrEmpty(groups))
            {
                string str1 = groups.Trim();
                char[] chrArray = new char[] { ';' };
                string[] strArrays = str1.TrimEnd(chrArray).Split(new char[] { ';' });
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string groupFromID = this.GetGroupFromID(strArrays[i]);
                    if (!string.IsNullOrEmpty(groupFromID))
                    {
                        str = string.Concat(str, groupFromID, ';');
                    }
                }
            }

            return str.TrimEnd(new char[] { ';' });
        }

        public string GetAttachments(string sListID, int iItemID)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNode attachmentCollection = webServiceForLists.GetAttachmentCollection(sListID, iItemID.ToString());
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Attachments");
            foreach (XmlNode childNode in attachmentCollection.ChildNodes)
            {
                xmlTextWriter.WriteStartElement("Attachment");
                xmlTextWriter.WriteAttributeString("LeafName", NWSAdapter.GetFileNameFromUrl(childNode.InnerText));
                xmlTextWriter.WriteAttributeString("IsExternalized", false.ToString());
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private void GetAttendeesList(ListsService listsWebService, out string sListID, out string sListVersion)
        {
            sListID = null;
            sListVersion = null;
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(listsWebService.GetListCollection(),
                ".//sp:List[@ServerTemplate='202']");
            if (xmlNodeLists.Count != 1)
            {
                return;
            }

            XmlNode itemOf = xmlNodeLists[0];
            sListID = itemOf.Attributes["ID"].Value;
            sListVersion = itemOf.Attributes["Version"].Value;
        }

        public string GetAudiences()
        {
            return null;
        }

        public string GetContentTypes(string sListId)
        {
            if (!base.SharePointVersion.IsSharePoint2007OrLater)
            {
                return "<ContentTypes />";
            }

            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
            {
                xmlWriter.WriteStartElement("ContentTypes");
                XmlNode contentTypes = null;
                Hashtable hashtables = new Hashtable();
                if (sListId == null || sListId.Length == 0)
                {
                    if (!this.ClientOMAvailable)
                    {
                        contentTypes = this.GetWebServiceForWebs().GetContentTypes();
                    }
                    else
                    {
                        try
                        {
                            object[] objArray = new object[] { this };
                            string str = (string)this.ExecuteClientOMMethod("GetWebContentTypes", objArray);
                            contentTypes = XmlUtility.StringToXmlNode(str);
                        }
                        catch (Exception exception)
                        {
                        }
                    }

                    foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(contentTypes, "//sp:ContentType"))
                    {
                        hashtables.Clear();
                        this.WriteContentTypeXml(sListId, xmlWriter, hashtables, xmlNodes);
                    }
                }
                else
                {
                    contentTypes = this.GetWebServiceForLists().GetListContentTypes(sListId, "0");
                    foreach (XmlNode xmlNodes1 in XmlUtility.RunXPathQuery(contentTypes, "//sp:ContentType"))
                    {
                        hashtables.Clear();
                        this.WriteContentTypeXml(sListId, xmlWriter, hashtables, xmlNodes1);
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }

            return stringBuilder.ToString();
        }

        public byte[] GetDashboardPageTemplate(int iTemplateId)
        {
            return this.WebPartPageTemplateManager.GetDashboardTemplate(iTemplateId);
        }

        private string GetDecimalFormatSID(string SID)
        {
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(SID);
            byte[] numArray = new byte[securityIdentifier.BinaryLength];
            securityIdentifier.GetBinaryForm(numArray, 0);
            string str = "0x";
            byte[] numArray1 = numArray;
            for (int i = 0; i < (int)numArray1.Length; i++)
            {
                byte num = numArray1[i];
                str = string.Concat(str, num.ToString("X2"));
            }

            return str;
        }

        private void GetDirListName(XmlAttributeCollection listAttribs, out string sDirName, out string sListName)
        {
            string rootFolder = this.GetRootFolder(listAttribs);
            if (string.IsNullOrEmpty(rootFolder))
            {
                throw new Exception(Metalogix.SharePoint.Adapters.NWS.Properties.Resources
                    .Error_unable_to_determine_list_root_folder);
            }

            rootFolder = rootFolder.TrimStart(new char[] { '/' });
            int num = rootFolder.LastIndexOf('/');
            if (num <= 0)
            {
                sDirName = "";
                sListName = rootFolder;
                return;
            }

            sDirName = rootFolder.Substring(0, num);
            sListName = rootFolder.Substring(num + 1);
        }

        public byte[] GetDocument(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            byte[] byteArray;
            sFileDirRef = this.EnsureWebRelativeFileDirRef(sFileDirRef);
            try
            {
                byteArray = this.DownloadDocumentToByteArray(sFileDirRef, sFileLeafRef, true,
                    base.SharePointVersion.IsSharePoint2007OrLater);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Could not retrieve Document. Exception: ", exception.Message));
            }

            return byteArray;
        }

        public byte[] GetDocumentBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iLevel)
        {
            return null;
        }

        private string GetDocumentDownloadUrl(string sFileDirRef, string sFileLeafRef, bool bWebRelative,
            bool bUseSpecialDownloadUrl)
        {
            string str = sFileDirRef.Trim(new char[] { '/' });
            char[] chrArray = new char[] { '/' };
            string str1 = string.Concat(str, "/", sFileLeafRef.Trim(chrArray));
            if (!bUseSpecialDownloadUrl)
            {
                string str2 = ((bWebRelative ? this.Url : this.ServerUrl)).Trim(new char[] { '/' });
                char[] chrArray1 = new char[] { '/' };
                return string.Concat(str2, "/", str1.Trim(chrArray1));
            }

            string url = this.Url;
            char[] chrArray2 = new char[] { '/' };
            return string.Concat(url.TrimEnd(chrArray2), "/_layouts/download.aspx?SourceUrl=",
                HttpUtility.UrlEncode(string.Concat((bWebRelative ? "" : "/"), str1)));
        }

        public string GetDocumentId(string sDocUrl)
        {
            if (sDocUrl == null)
            {
                return null;
            }

            string str = null;
            try
            {
                string str1 = sDocUrl.Replace(this.ServerRelativeUrl, "");
                char[] chrArray = new char[] { '/', '\\' };
                string str2 = string.Format("method=getDocsMetaInfo&url_list=[{0}]", str1.TrimStart(chrArray));
                string str3 = RPCUtil.SendRequest(this,
                    string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll"), str2);
                str = this.ExtractDocumentGuid(str3);
            }
            catch
            {
            }

            return str;
        }

        public byte[] GetDocumentVersion(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            byte[] byteArray;
            sFileDirRef = this.EnsureWebRelativeFileDirRef(sFileDirRef);
            try
            {
                byteArray = this.DownloadDocumentToByteArray(sFileDirRef, sFileLeafRef, true, false);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Could not retrieve Document. Exception: ", exception.Message));
            }

            return byteArray;
        }

        public byte[] GetDocumentVersionBlobRef(string sDocID, string sFileDirRef, string sFileLeafRef, int iVersion)
        {
            return null;
        }

        private static Exception GetExecutedClientOMMethodException(Exception ex)
        {
            if (!(ex is TargetInvocationException) || ex.InnerException == null)
            {
                return ex;
            }

            return ex.InnerException;
        }

        private void GetExistingFolderInfo(string sListID, string sParentFolder, ListsService listsSvs,
            string sFileLeafRef, out int iID, out string sFileDirRef)
        {
            string value;
            sFileDirRef = null;
            iID = -1;
            XmlNode list = listsSvs.GetList(sListID);
            string rootFolder = this.GetRootFolder(list.Attributes);
            char[] chrArray = new char[] { '/' };
            string str = Utils.JoinUrl(rootFolder.TrimStart(chrArray), sParentFolder).TrimEnd(new char[] { '/' });
            string folders = this.GetFolders(sListID, null, str);
            foreach (XmlNode xmlNodes in XmlUtility.StringToXmlNode(folders).SelectNodes("//Folder"))
            {
                if (xmlNodes.Attributes["FileLeafRef"].InnerText != sFileLeafRef)
                {
                    continue;
                }

                if (xmlNodes.Attributes["FileDirRef"] == null)
                {
                    value = null;
                }
                else
                {
                    value = xmlNodes.Attributes["FileDirRef"].Value;
                }

                sFileDirRef = value;
                iID = Convert.ToInt32(xmlNodes.Attributes["ID"].InnerText);
                break;
            }
        }

        public string GetExternalContentTypeOperations(string sExtContentTypeNamespace, string sExtContentTypeName)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeOperationCollection");
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetExternalContentTypes()
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SPExternalContentTypeCollection");
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetExternalItems(string sExtContentTypeNamespace, string sExtContentTypeName,
            string sExtContentTypeOperation, string sListID)
        {
            string str;
            if (!this.ClientOMAvailable)
            {
                throw new Exception("Unable to load external items, because the Client Object Model is not available.");
            }

            object[] objArray = new object[]
                { sExtContentTypeNamespace, sExtContentTypeName, sExtContentTypeOperation, sListID, this };
            object[] objArray1 = objArray;
            try
            {
                str = (string)this.ExecuteClientOMMethod("GetExternalItems", objArray1);
            }
            catch (Exception exception)
            {
                throw;
            }

            return str;
        }

        public string GetFields(string sListID, bool bGetAllAvailableFields)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("Fields");
            XmlNodeList xmlNodeLists = null;
            if (!string.IsNullOrEmpty(sListID))
            {
                xmlNodeLists = this.FetchListFields(sListID, this.GetWebServiceForLists());
            }
            else if (base.SharePointVersion.IsSharePoint2007OrLater)
            {
                xmlNodeLists = this.FetchSiteColumns(this.GetWebServiceForWebs());
            }

            if (xmlNodeLists != null)
            {
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    this.AddListTitlesToLookups(xmlNodes);
                    xmlTextWriter.WriteRaw(
                        xmlNodes.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\"", ""));
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringBuilder.ToString();
        }

        public static string GetFileNameFromUrl(string sFileName)
        {
            string str;
            try
            {
                str = sFileName.Substring(sFileName.LastIndexOf("/") + 1);
            }
            catch
            {
                str = sFileName;
            }

            return str;
        }

        public string GetFilePropertiesClientOM(string sServerRelativeUrl)
        {
            object[] objArray = new object[] { sServerRelativeUrl, this };
            string str = null;
            if (this.ClientOMAvailable)
            {
                try
                {
                    str = (string)this.ExecuteClientOMMethod("GetFileProperties", objArray);
                }
                catch (Exception exception)
                {
                }
            }

            return str;
        }

        public string GetFiles(string sFolderPath, ListItemQueryType itemTypes)
        {
            XmlNode folderContentFromWeb = RPCUtil.GetFolderContentFromWeb(this, sFolderPath, itemTypes);
            XmlNodeList xmlNodeLists = folderContentFromWeb.SelectNodes("/FolderContent/Files/File");
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("FolderContent");
            if ((itemTypes & ListItemQueryType.ListItem) == ListItemQueryType.ListItem)
            {
                xmlTextWriter.WriteStartElement("Files");
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    NWSAdapter.AddFileToXml(xmlTextWriter, xmlNodes);
                }

                xmlTextWriter.WriteEndElement();
            }

            if ((itemTypes & ListItemQueryType.Folder) == ListItemQueryType.Folder)
            {
                XmlNodeList xmlNodeLists1 = folderContentFromWeb.SelectNodes("/FolderContent/Folders/Folder");
                xmlTextWriter.WriteStartElement("Folders");
                foreach (XmlNode xmlNodes1 in xmlNodeLists1)
                {
                    NWSAdapter.AddFolderToXml(xmlTextWriter, xmlNodes1);
                }

                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        private string GetFileUrl(string webUrl, string sFolderPath, string fileName)
        {
            if (string.IsNullOrEmpty(sFolderPath))
            {
                char[] chrArray = new char[] { '/' };
                return string.Concat(webUrl.TrimEnd(chrArray), "/", fileName);
            }

            string[] strArrays = new string[5];
            char[] chrArray1 = new char[] { '/' };
            strArrays[0] = webUrl.TrimEnd(chrArray1);
            strArrays[1] = "/";
            char[] chrArray2 = new char[] { '/' };
            strArrays[2] = sFolderPath.TrimEnd(chrArray2);
            strArrays[3] = "/";
            strArrays[4] = fileName;
            return string.Concat(strArrays);
        }

        public string GetFolders(string sListID, string sIDs, string sParentFolder)
        {
            bool flag;
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            try
            {
                try
                {
                    xmlTextWriter.WriteStartElement("Folders");
                    ListsService webServiceForLists = this.GetWebServiceForLists();
                    bool flag1 = true;
                    ListType listBaseType = ListType.Unknown;
                    if (base.SharePointVersion.IsSharePoint2003)
                    {
                        listBaseType = this.GetListBaseType(webServiceForLists.GetList(sListID));
                        if (listBaseType != ListType.DocumentLibrary)
                        {
                            flag1 = false;
                        }
                    }

                    if (flag1)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
                        string[] strArrays = new string[] { "FileDirRef", "FileLeafRef" };
                        xmlNodes.InnerXml = Utils.BuildQuery(sIDs, sParentFolder, false, false,
                            ListItemQueryType.Folder, strArrays);
                        XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
                        xmlNodes1.InnerXml =
                            "<FieldRef Name='ID'/><FieldRef Name='Title'/><FieldRef Name='Created'/><FieldRef Name='Author'/><FieldRef Name='Modified'/><FieldRef Name='Editor'/><FieldRef Name='FileLeafRef'/><FieldRef Name='FileDirRef'/><FieldRef Name='_ModerationStatus'/>";
                        if (base.SharePointVersion.IsSharePoint2007OrLater)
                        {
                            XmlNode xmlNodes2 = xmlNodes1;
                            xmlNodes2.InnerXml = string.Concat(xmlNodes2.InnerXml, "<FieldRef Name='ContentTypeId'/>");
                        }

                        XmlNode xmlNodes3 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                        xmlNodes3.InnerXml =
                            "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><ViewAttributes Scope='RecursiveAll'/><MeetingInstanceID>-2</MeetingInstanceID>";
                        List<XmlNode> xmlNodes4 = this.RunListItemQuery(sListID, listBaseType, xmlNodes, xmlNodes1,
                            xmlNodes3, webServiceForLists, out flag, null);
                        sParentFolder.TrimStart(new char[] { '/' });
                        foreach (XmlNode xmlNodes5 in xmlNodes4)
                        {
                            this.FillFolderXML(xmlTextWriter, xmlNodes5.Attributes, sParentFolder, sListID);
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }
            finally
            {
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }

            return stringWriter.ToString();
        }

        private string GetGroupFromID(string sID)
        {
            if (this.m_GroupMap == null)
            {
                this.BuildGroupMap();
            }

            string item = null;
            if (this.m_GroupMap.ContainsKey(sID))
            {
                item = this.m_GroupMap[sID];
            }

            return item;
        }

        public string GetGroups()
        {
            string str = null;
            str = (!base.SharePointVersion.IsSharePoint2007OrEarlier ? this.Get2010Groups() : this.Get2007Groups());
            return str;
        }

        public string GetIDFromGroup(string sGroup)
        {
            if (this.m_ReverseGroupMap == null)
            {
                this.BuildGroupMap();
            }

            string item = null;
            if (this.m_ReverseGroupMap.ContainsKey(sGroup))
            {
                item = this.m_ReverseGroupMap[sGroup];
            }

            return item;
        }

        public string GetIDFromUser(string sUser)
        {
            string str;
            string str1;
            if (this.m_ReverseUserMap == null)
            {
                this.BuildUserMap();
            }

            sUser = sUser.ToLowerInvariant();
            if (this.m_ReverseUserMap.TryGetValue(sUser, out str))
            {
                return str;
            }

            if (base.SharePointVersion.IsSharePoint2010OrLater && Utils.SwitchUserNameFormat(sUser, out str1) &&
                this.m_ReverseUserMap.TryGetValue(str1, out str))
            {
                return str;
            }

            return null;
        }

        private string GetIdListFromMultiValueLookupColumn(string strValue)
        {
            int nextColonPound = this.GetNextColonPound(strValue, 0);
            if (nextColonPound == -1)
            {
                return strValue;
            }

            string str = strValue.Substring(0, nextColonPound);
            try
            {
                while (nextColonPound >= 0)
                {
                    nextColonPound = this.GetNextColonPound(strValue, nextColonPound + 1);
                    if (nextColonPound == -1)
                    {
                        continue;
                    }

                    str = string.Concat(str, ";#");
                    int num = this.GetNextColonPound(strValue, nextColonPound + 1);
                    str = (num != -1
                        ? string.Concat(str, strValue.Substring(nextColonPound + 2, num - nextColonPound - 2))
                        : string.Concat(str, strValue.Substring(nextColonPound + 2)));
                    nextColonPound = num;
                }
            }
            catch
            {
            }

            return str;
        }

        private Dictionary<string, int> GetIndexsOfNavNodeChunksByEid(string[] lines)
        {
            Dictionary<string, int> strs = new Dictionary<string, int>();
            int num = -1;
            string str = null;
            int num1 = -1;
            for (int i = 0; i < (int)lines.Length; i++)
            {
                if (num == 0)
                {
                    num1 = i;
                }

                if (lines[i].Contains("<ul>"))
                {
                    num++;
                }
                else if (lines[i].Contains("</ul>"))
                {
                    num--;
                    if (num == 0 && !string.IsNullOrEmpty(str) && !strs.ContainsKey(str))
                    {
                        strs.Add(str, num1);
                    }
                    else if (num < 0)
                    {
                        return strs;
                    }
                }
                else if (lines[i].Contains("<li>eid="))
                {
                    str = this.GetStringAfterCharacter(lines[i], '=').Trim();
                }
            }

            return strs;
        }

        private void GetIsIncludedForV3WebParts(string sWppHtml, Dictionary<string, bool> isIncludedDict)
        {
            if (!string.IsNullOrEmpty(sWppHtml))
            {
                string str = "WebPartId";
                string str1 = "Included";
                string str2 = "((IsIncluded\\s*=\\s*\"(?<{0}>[\\w]*)\")|(ID\\s*=\\s*\"g_(?<{1}>[\\w_]*)\"))";
                string str3 = string.Concat("<WpNs.*", str2, ".*", str2);
                string str4 = string.Concat("<WebPartPages:XsltListViewWebPart.*", str2, ".*", str2);
                string[] strArrays = new string[] { "(", str3, "|", str4, ")" };
                string str5 = string.Format(string.Concat(strArrays), str1, str);
                foreach (Match match in (new Regex(str5, RegexOptions.IgnoreCase)).Matches(sWppHtml))
                {
                    bool flag = false;
                    string upper = match.Groups[str].Value.Replace("_", "-").ToUpper();
                    if (isIncludedDict.ContainsKey(upper) || !bool.TryParse(match.Groups[str1].Value, out flag))
                    {
                        continue;
                    }

                    isIncludedDict.Add(upper, flag);
                }
            }
        }

        public static string GetJustGUID(string sGUID)
        {
            if (sGUID == null)
            {
                return null;
            }

            int num = sGUID.IndexOf("{");
            int num1 = sGUID.IndexOf("}");
            if (num < 0 || num1 < 0 || num1 <= num + 1)
            {
                return sGUID;
            }

            return sGUID.Substring(num + 1, num1 - num - 1);
        }

        public string GetLanguagesAndWebTemplates()
        {
            throw new NotImplementedException();
        }

        public string GetList(string sListID)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            string str = null;
            if (base.SharePointVersion.IsSharePoint2007OrEarlier)
            {
                string str1 = "method=get+web+struct&service_name=&eidHead=0&levels=-1&includeHead=true";
                str = RPCUtil.SendRequest(this,
                    string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll"), str1);
            }

            ListsService webServiceForLists = this.GetWebServiceForLists();
            ViewsService webServiceForViews = this.GetWebServiceForViews();
            XmlNode list = webServiceForLists.GetList(sListID);
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(list, "//sp:Fields/sp:Field");
            XmlNodeList xmlNodeLists1 =
                XmlUtility.RunXPathQuery(webServiceForViews.GetViewCollection(sListID), "//sp:View");
            Hashtable listSettings = this.GetListSettings(list.Attributes["RootFolder"].Value);
            XmlNode xmlNodes = XmlUtility.RunXPathQuerySelectSingle(list, "//sp:DataSource");
            this.FillListXML(xmlTextWriter, list.Attributes, xmlNodeLists, xmlNodeLists1, str, listSettings, xmlNodes);
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        private ListType GetListBaseType(XmlNode listSettingsNode)
        {
            try
            {
                if (listSettingsNode != null)
                {
                    XmlAttribute itemOf = listSettingsNode.Attributes["BaseType"];
                    if (itemOf != null)
                    {
                        return (ListType)Enum.Parse(typeof(ListType), itemOf.Value);
                    }
                }
            }
            catch
            {
            }

            return ListType.Unknown;
        }

        private void GetListByTitle(string commandConfigurationXml, OperationReporting opResult)
        {
            GetListByNameConfiguration getListByNameConfiguration =
                commandConfigurationXml.Deserialize<GetListByNameConfiguration>();
            opResult.LogObjectXml(this.GetList(getListByNameConfiguration.ListTitle));
        }

        private XmlNode GetListContentTypeFromWebContentType(string sWebContentTypeID, XmlNodeList listContentTypes)
        {
            XmlNode xmlNodes;
            int length = sWebContentTypeID.Length + 34;
            IEnumerator enumerator = listContentTypes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    XmlAttribute itemOf = current.Attributes["ID"];
                    if (itemOf == null ||
                        !itemOf.Value.StartsWith(sWebContentTypeID, StringComparison.OrdinalIgnoreCase) ||
                        itemOf.Value.Length != length)
                    {
                        continue;
                    }

                    xmlNodes = current;
                    return xmlNodes;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return xmlNodes;
        }

        private string GetListFieldXML(string sListID, ListsService listsWebService)
        {
            XmlNodeList xmlNodeLists =
                XmlUtility.RunXPathQuery(listsWebService.GetList(sListID), "//sp:Fields/sp:Field");
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            if (xmlNodeLists != null)
            {
                xmlTextWriter.WriteStartElement("Fields");
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    string value = xmlNodes.Attributes["Name"].Value;
                    xmlTextWriter.WriteRaw(
                        xmlNodes.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\"", ""));
                }

                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        private Dictionary<string, string> GetListIDTitleMap(bool bUseIdAsKey)
        {
            ListsService webServiceForLists = null;
            webServiceForLists = this.GetWebServiceForLists();
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(), "//sp:List");
            Dictionary<string, string> strs = new Dictionary<string, string>(xmlNodeLists.Count);
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                string value = xmlNodes.Attributes["ID"].Value;
                char[] chrArray = new char[] { '{', '}' };
                string upperInvariant = value.Trim(chrArray).ToUpperInvariant();
                string lower = xmlNodes.Attributes["Title"].Value;
                if (!bUseIdAsKey)
                {
                    lower = lower.ToLower();
                    if (strs.ContainsKey(lower))
                    {
                        continue;
                    }

                    strs.Add(lower, upperInvariant);
                }
                else
                {
                    strs.Add(upperInvariant, lower);
                }
            }

            return strs;
        }

        private string GetListItemCollectionPositionNext(XmlNode xmlNodeList)
        {
            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlNodeList.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            XmlNode xmlNodes = xmlNodeList.SelectSingleNode("./rs:data", xmlNamespaceManagers);
            if (xmlNodes.Attributes["ListItemCollectionPositionNext"] == null)
            {
                return null;
            }

            return xmlNodes.Attributes["ListItemCollectionPositionNext"].Value;
        }

        public string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive, ListItemQueryType itemTypes)
        {
            return this.GetListItemIDs(sListID, sParentFolder, bRecursive, itemTypes, this.Url);
        }

        private string GetListItemIDs(string sListID, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sWebUrl)
        {
            bool flag;
            StringBuilder stringBuilder = null;
            try
            {
                stringBuilder = new StringBuilder(1024);
                ListsService webServiceForLists = this.GetWebServiceForLists(sWebUrl);
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode list = webServiceForLists.GetList(sListID);
                bool flag1 = true;
                bool flag2 = (!base.SharePointVersion.IsSharePoint2003 || list.Attributes["BaseType"] == null
                    ? false
                    : list.Attributes["BaseType"].Value == "5");
                if (base.SharePointVersion.IsSharePoint2003 && list.Attributes["BaseType"].InnerText != "1")
                {
                    flag1 = false;
                }

                XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
                if (!flag1)
                {
                    string[] strArrays = new string[] { "ID" };
                    xmlNodes.InnerXml = Utils.BuildQuery(null, "", false, flag2,
                        ListItemQueryType.ListItem | ListItemQueryType.Folder, strArrays);
                    xmlNodes1.InnerXml = "<FieldRef Name='ID'/>";
                }
                else
                {
                    string[] strArrays1 = new string[] { "ID" };
                    xmlNodes.InnerXml = Utils.BuildQuery(null, sParentFolder, bRecursive, false, itemTypes, strArrays1);
                    xmlNodes1.InnerXml = "<FieldRef Name='ID'/>";
                }

                XmlNode xmlNodes2 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                xmlNodes2.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><ViewAttributes Scope='RecursiveAll' /><MeetingInstanceID>-2</MeetingInstanceID>";
                XmlNode[] listItems = this.GetListItems(webServiceForLists, sListID, null, xmlNodes, xmlNodes1,
                    xmlNodes2, null, out flag);
                XmlNode[] xmlNodeArrays = listItems;
                for (int i = 0; i < (int)xmlNodeArrays.Length; i++)
                {
                    foreach (XmlNode xmlNodes3 in XmlUtility.RunXPathQuery(xmlNodeArrays[i], "//z:row"))
                    {
                        string innerText = xmlNodes3.Attributes["ows_ID"].InnerText;
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(",");
                        }

                        stringBuilder.Append(innerText);
                    }
                }
            }
            catch (Exception exception)
            {
            }

            if (stringBuilder.Length == 0)
            {
                return null;
            }

            return stringBuilder.ToString();
        }

        public string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sListSettings, GetListItemOptions getOptions)
        {
            return this.GetListItems(sListID, sIDs, sFields, sParentFolder, bRecursive, itemTypes, this.Url,
                sListSettings, getOptions);
        }

        private string GetListItems(string sListID, string sIDs, string sFields, string sParentFolder, bool bRecursive,
            ListItemQueryType itemTypes, string sWebUrl, string sListSettings, GetListItemOptions getOptions)
        {
            ListsService listsService;
            XmlNode xmlNodes;
            string str;
            this.PrepareForItemFetch(sWebUrl, sListID, sListSettings, out listsService, out xmlNodes);
            itemTypes = (xmlNodes.SelectSingleNode("@ServerTemplate | @BaseTemplate").Value ==
                         Convert.ToInt32(ServerTemplate.DiscussionBoard).ToString()
                ? itemTypes | ListItemQueryType.Folder
                : itemTypes);
            ListType listBaseType = this.GetListBaseType(xmlNodes);
            bool flag = listBaseType == ListType.DocumentLibrary;
            bool flag1 = (!base.SharePointVersion.IsSharePoint2003 ? false : listBaseType == ListType.IssuesList);
            if (!base.SharePointVersion.IsSharePoint2003 || flag)
            {
                string[] strArrays = new string[] { "ID" };
                str = Utils.BuildQuery(sIDs, sParentFolder, bRecursive, false, itemTypes, strArrays);
            }
            else
            {
                string[] strArrays1 = new string[] { "ID" };
                str = Utils.BuildQuery(sIDs, "", bRecursive, flag1,
                    ListItemQueryType.ListItem | ListItemQueryType.Folder, strArrays1);
            }

            return this.GetListItemsByQueryInternal(sListID, sFields, str, listsService, xmlNodes, getOptions);
        }

        // Metalogix.SharePoint.Adapters.NWS.NWSAdapter
        private XmlNode[] GetListItems(ListsService listsSvs, string listName, string viewName, XmlNode query,
            XmlNode viewFields, XmlNode queryOptions, string webID, out bool bSortOrderViolated)
        {
            bSortOrderViolated = false;
            string text = 4294967295u.ToString();
            List<XmlNode> list = new List<XmlNode>();
            try
            {
                XmlNode listItems =
                    listsSvs.GetListItems(listName, viewName, query, viewFields, text, queryOptions, webID);
                list.Add(listItems);
            }
            catch (Exception ex)
            {
                if (!this.GetListItemsRequiresPaging(ex))
                {
                    throw;
                }

                if (base.SharePointVersion.IsSharePoint2010OrLater &&
                    !Utils.GetQueryIsUsingThresholdApprovedSorting(query))
                {
                    Utils.ReplaceOrderByInQuery(query, new string[]
                    {
                        "FileDirRef",
                        "FileLeafRef"
                    });
                    bSortOrderViolated = true;
                }

                text = "5000";
                string text2;
                do
                {
                    try
                    {
                        XmlNode listItems2 = listsSvs.GetListItems(listName, viewName, query, viewFields, text,
                            queryOptions, webID);
                        list.Add(listItems2);
                        text2 = this.GetListItemCollectionPositionNext(listItems2);
                        NWSAdapter.InsertOrUpdatePagingInfo(queryOptions, text2);
                    }
                    catch (Exception ex2)
                    {
                        if (!this.GetListItemsRequiresPaging(ex2))
                        {
                            throw;
                        }

                        int num = int.Parse(text);
                        if (num > 5000)
                        {
                            text = "5000";
                        }
                        else if (num > 2000)
                        {
                            text = "2000";
                        }
                        else
                        {
                            if (num <= 500)
                            {
                                throw;
                            }

                            text = (num / 2).ToString();
                        }

                        text2 = "Try Again";
                    }
                } while (!string.IsNullOrEmpty(text2));
            }

            return list.ToArray();
        }

        public string GetListItemsByQuery(string listID, string fields, string query, string listSettings,
            GetListItemOptions getOptions)
        {
            ListsService listsService;
            XmlNode xmlNodes;
            this.PrepareForItemFetch(this.Url, listID, listSettings, out listsService, out xmlNodes);
            return this.GetListItemsByQueryInternal(listID, fields, query, listsService, xmlNodes, getOptions);
        }

        private string GetListItemsByQueryInternal(string sListID, string sFields, string query, ListsService listsSvs,
            XmlNode nodeList, GetListItemOptions getOptions)
        {
            bool flag;
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("ListItems");
            ListType listBaseType = this.GetListBaseType(nodeList);
            bool flag1 = listBaseType == ListType.DocumentLibrary;
            string moderatorViewID = this.GetModeratorViewID(nodeList);
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
            xmlNodes.InnerXml = query;
            XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNodeList xmlNodeLists = null;
            if (sFields != null && sFields.Length > 0)
            {
                XmlDocument xmlDocument1 = new XmlDocument();
                xmlDocument1.LoadXml(sFields);
                xmlNodeLists = xmlDocument1.SelectNodes("/Fields/Field");
                xmlNodes1.InnerXml = this.BuildQueryFieldsXml(xmlNodeLists, flag1, null);
            }

            if ((xmlNodeLists == null ? true : xmlNodeLists.Count <= 0) && string.IsNullOrEmpty(sFields))
            {
                if (XmlUtility.RunXPathQuery(nodeList, "//sp:List/sp:Fields | //List/Fields").Count == 0)
                {
                    nodeList = listsSvs.GetList(sListID);
                }

                xmlNodeLists = XmlUtility.RunXPathQuery(nodeList, "//sp:List/sp:Fields/sp:Field | //List/Fields/Field");
            }

            XmlNode xmlNodes2 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
            xmlNodes2.InnerXml =
                "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><MeetingInstanceID>-2</MeetingInstanceID>";
            if (string.IsNullOrEmpty(moderatorViewID))
            {
                XmlNode xmlNodes3 = xmlNodes2;
                xmlNodes3.InnerXml = string.Concat(xmlNodes3.InnerXml, "<ViewAttributes Scope='RecursiveAll' />");
            }

            List<XmlNode> xmlNodes4 = this.RunListItemQuery(sListID, listBaseType, xmlNodes, xmlNodes1, xmlNodes2,
                listsSvs, out flag, moderatorViewID);
            if (flag || base.SharePointVersion.IsSharePoint2003)
            {
                xmlNodes4.Sort((XmlNode source, XmlNode target) =>
                {
                    int num;
                    int num1;
                    XmlAttribute itemOf = source.Attributes["ows_ID"];
                    XmlAttribute xmlAttribute = target.Attributes["ows_ID"];
                    if (itemOf == null || !int.TryParse(itemOf.Value, out num))
                    {
                        return 0;
                    }

                    if (xmlAttribute == null || !int.TryParse(xmlAttribute.Value, out num1))
                    {
                        return 0;
                    }

                    return num.CompareTo(num1);
                });
            }

            foreach (XmlNode xmlNodes5 in xmlNodes4)
            {
                this.FillItemXML(xmlTextWriter, xmlNodes5.Attributes, xmlNodeLists, sListID, getOptions, false);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        private bool GetListItemsRequiresPaging(Exception ex)
        {
            if (!(ex is SoapException))
            {
                if (!(ex is WebException))
                {
                    return false;
                }

                return (ex as WebException).Status == WebExceptionStatus.Timeout;
            }

            SoapException soapException = ex as SoapException;
            if (RPCUtil.GetSoapErrorCode(soapException.Detail).Equals("0x80070024", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (RPCUtil.GetSoapError(soapException.Detail).Contains("System.OutOfMemoryException"))
            {
                return true;
            }

            return false;
        }

        public string GetListItemVersions(string sListID, int iItemID, string sFields, string configurationXml)
        {
            XmlNode list;
            DateTime dateTime;
            IEnumerable rows;
            char[] chrArray;
            int attributeValueAsInt = 0;
            if (!string.IsNullOrEmpty(configurationXml))
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(configurationXml);
                attributeValueAsInt =
                    xmlNode.GetAttributeValueAsInt(XmlAttributeNames.NoOfLatestVersionsToGet.ToString());
            }

            if (attributeValueAsInt < 0)
            {
                throw new ArgumentOutOfRangeException("noOfLatestVersionsToGet", "value is less than zero");
            }

            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sFields);
            XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("/Fields/Field");
            List<string> strs = new List<string>();
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                string value = xmlNodes.Attributes["Name"].Value;
                if (strs.Contains(value))
                {
                    continue;
                }

                strs.Add(value);
            }

            if (base.SharePointVersion.IsSharePoint2003)
            {
                if (string.IsNullOrEmpty(configurationXml))
                {
                    list = webServiceForLists.GetList(sListID);
                }
                else
                {
                    XmlNode xmlNode1 = XmlUtility.StringToXmlNode(configurationXml);
                    list = xmlNode1.SelectSingleNode(string.Format("//{0}", XmlElementNames.List.ToString()));
                }

                string moderatorViewID = this.GetModeratorViewID(list);
                if (list.Attributes["BaseType"] != null && list.Attributes["BaseType"].Value == "5")
                {
                    return this.Get2003IssueVersions(sListID, iItemID, moderatorViewID, xmlNodeLists,
                        webServiceForLists);
                }
            }

            string listItems = this.GetListItems(sListID, iItemID.ToString(), sFields, null, false,
                ListItemQueryType.ListItem, configurationXml, new GetListItemOptions());
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(listItems);
            XmlNode xmlNodes1 = xmlDocument1.SelectSingleNode("//ListItem");
            VersionDataTrackingTable versionDataTrackingTables = new VersionDataTrackingTable();
            if (base.SharePointVersion.IsSharePoint2007OrLater)
            {
                Dictionary<string, string> timeToVersionMapAndInitializeData =
                    this.GetTimeToVersionMapAndInitializeData(sListID, iItemID.ToString(), versionDataTrackingTables,
                        webServiceForLists);
                foreach (XmlNode xmlNodes2 in xmlNodeLists)
                {
                    try
                    {
                        if (xmlNodes2.Attributes["ColName"] != null)
                        {
                            string str = xmlNodes2.Attributes["Name"].Value;
                            string value1 = xmlNodes2.Attributes["Type"].Value;
                            bool flag = (xmlNodes2.Attributes["FromBaseType"] == null
                                ? false
                                : xmlNodes2.Attributes["FromBaseType"].Value == "TRUE");
                            if (!(str == "_UIVersionString") && !(str == "Modified"))
                            {
                                XmlNode versionCollection =
                                    webServiceForLists.GetVersionCollection(sListID, iItemID.ToString(), str);
                                if (versionCollection != null && versionCollection.ChildNodes.Count != 0)
                                {
                                    List<string> strs1 = new List<string>(versionCollection.ChildNodes.Count);
                                    foreach (XmlNode childNode in versionCollection.ChildNodes)
                                    {
                                        string versionCellData = this.GetVersionCellData(value1, flag,
                                            childNode.Attributes[str].Value);
                                        string str1 = childNode.Attributes["Modified"].Value;
                                        int num = 0;
                                        while (strs1.Contains(str1))
                                        {
                                            str1 = string.Concat(childNode.Attributes["Modified"].Value,
                                                num.ToString());
                                            num++;
                                        }

                                        strs1.Add(str1);
                                        versionDataTrackingTables.SetValue(timeToVersionMapAndInitializeData[str1], str,
                                            versionCellData);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (xmlNodes1.Attributes["EncodedAbsUrl"] != null)
            {
                if (!strs.Contains("_CheckinComment"))
                {
                    strs.Add("_CheckinComment");
                }

                if (!strs.Contains("Editor"))
                {
                    strs.Add("Editor");
                }

                if (!strs.Contains("_IsCurrentVersion"))
                {
                    strs.Add("_IsCurrentVersion");
                }

                if (!strs.Contains("_UIVersionString"))
                {
                    strs.Add("_UIVersionString");
                }

                if (!strs.Contains("_UIVersion"))
                {
                    strs.Add("_UIVersion");
                }

                if (!strs.Contains("Modified"))
                {
                    strs.Add("Modified");
                }

                if (!base.SharePointVersion.IsSharePoint2007OrEarlier)
                {
                    string str2 = string.Concat("/", xmlNodes1.Attributes["FileRef"].Value);
                    string filePropertiesClientOM = this.GetFilePropertiesClientOM(str2);
                    if (filePropertiesClientOM != null)
                    {
                        XmlNode xmlNode2 = XmlUtility.StringToXmlNode(filePropertiesClientOM);
                        if (xmlNode2 != null)
                        {
                            XmlNodeList xmlNodeLists1 = xmlNode2.SelectNodes("//Version");
                            chrArray = new char[] { '@' };
                            char[] chrArray1 = chrArray;
                            for (int i = xmlNodeLists1.Count - 1; i >= 0; i--)
                            {
                                XmlNode itemOf = xmlNodeLists1[i];
                                string value2 = itemOf.Attributes["Comments"].Value;
                                string value3 = itemOf.Attributes["CreatedBy"].Value;
                                string str3 = (itemOf.Attributes["VersionNumber"].Value.StartsWith("@")
                                    ? "True"
                                    : "False");
                                string str4 = itemOf.Attributes["VersionNumber"].Value.TrimStart(chrArray1);
                                string str5 = itemOf.Attributes["VersionString"].Value.TrimStart(chrArray1);
                                versionDataTrackingTables.SetValue(str5, "_CheckinComment", value2, false);
                                versionDataTrackingTables.SetValue(str5, "Editor", value3, false);
                                versionDataTrackingTables.SetValue(str5, "_IsCurrentVersion", str3, false);
                                versionDataTrackingTables.SetValue(str5, "_UIVersionString", str5, false);
                                versionDataTrackingTables.SetValue(str5, "_UIVersion", str4, false);
                                versionDataTrackingTables.SetValue(str5, "_VersionNumber", str4, false);
                            }
                        }
                    }
                }
                else
                {
                    string value4 = xmlNodes1.Attributes["EncodedAbsUrl"].Value;
                    VersionsService webServiceForVersions = this.GetWebServiceForVersions();
                    XmlNodeList xmlNodeLists2 =
                        webServiceForVersions.GetVersions(value4).SelectNodes("//*[name()='result']");
                    for (int j = xmlNodeLists2.Count - 1; j >= 0; j--)
                    {
                        XmlNode itemOf1 = xmlNodeLists2[j];
                        string value5 = itemOf1.Attributes["version"].Value;
                        chrArray = new char[] { '@' };
                        string str6 = value5.TrimStart(chrArray);
                        if (!base.SharePointVersion.IsSharePoint2007OrLater ||
                            versionDataTrackingTables.ExistingVersions.Contains(new Version(str6)))
                        {
                            string value6 = itemOf1.Attributes["comments"].Value;
                            string value7 = itemOf1.Attributes["createdBy"].Value;
                            string str7 = (itemOf1.Attributes["version"].Value.StartsWith("@") ? "True" : "False");
                            string value8 = itemOf1.Attributes["created"].Value;
                            if (base.SharePointVersion.IsSharePoint2003)
                            {
                                if (!DateTime.TryParseExact(value8, "dd/MM/yyyy HH:mm", this.CultureInfo,
                                        DateTimeStyles.AssumeLocal, out dateTime) && !DateTime.TryParse(value8,
                                        this.CultureInfo, DateTimeStyles.AssumeLocal, out dateTime))
                                {
                                    object[] lCID = new object[]
                                    {
                                        "Failed to parse date time (", value8, ") using detected locale (",
                                        this.CultureInfo.LCID, ")"
                                    };
                                    throw new Exception(string.Concat(lCID));
                                }

                                value8 = Utils.FormatDate(this.TimeZone.LocalTimeToUtc(dateTime));
                            }

                            versionDataTrackingTables.SetValue(str6, "_CheckinComment", value6, false);
                            versionDataTrackingTables.SetValue(str6, "Editor", value7, false);
                            versionDataTrackingTables.SetValue(str6, "_IsCurrentVersion", str7, false);
                            versionDataTrackingTables.SetValue(str6, "_UIVersionString", str6, false);
                            versionDataTrackingTables.SetValue(str6, "_UIVersion", str6, false);
                            versionDataTrackingTables.SetValue(str6, "Modified", value8, false);
                        }
                    }
                }
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("ListItems");
            if (attributeValueAsInt <= 0)
            {
                rows = versionDataTrackingTables.Rows;
            }
            else
            {
                rows = this.GetNumberOfLatestItemVersions(attributeValueAsInt, versionDataTrackingTables);
            }

            foreach (DataRow row in rows)
            {
                if (base.SharePointVersion.IsSharePoint2010OrLater &&
                    !object.Equals(row["_VersionNumber"], row["_UIVersion"]))
                {
                    continue;
                }

                Dictionary<string, string> strs2 = new Dictionary<string, string>();
                xmlTextWriter.WriteStartElement("ListItem");
                xmlTextWriter.WriteAttributeString("IsExternalized", false.ToString());
                if (base.SharePointVersion.IsSharePoint2010OrLater && row.Table.Columns.Contains("_VersionNumber"))
                {
                    xmlTextWriter.WriteAttributeString("_VersionNumber", row["_VersionNumber"] as string);
                }

                foreach (string str8 in strs)
                {
                    if (!row.Table.Columns.Contains(str8))
                    {
                        continue;
                    }

                    string item = row[str8] as string;
                    if (strs2.ContainsKey(str8))
                    {
                        continue;
                    }

                    this.WriteVersionCell(xmlTextWriter, str8, item);
                    strs2.Add(str8, null);
                }

                foreach (string str9 in strs)
                {
                    if (strs2.ContainsKey(str9) || xmlNodes1.Attributes[str9] == null ||
                        str9.Equals("ContentType", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    xmlTextWriter.WriteAttributeString(xmlNodes1.Attributes[str9].Name,
                        xmlNodes1.Attributes[str9].Value);
                    strs2.Add(str9, null);
                }

                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public void GetListItemVersionsCSOM(XmlNodeList xmlFieldCollection, int iItemID, string sListID,
            XmlNode xmlItemNode, out VersionDataTrackingTable versionDataTable)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            int count = xmlFieldCollection.Count;
            versionDataTable = new VersionDataTrackingTable();
            Dictionary<string, string> timeToVersionMapAndInitializeData =
                this.GetTimeToVersionMapAndInitializeData(sListID, iItemID.ToString(), versionDataTable,
                    webServiceForLists);
            foreach (XmlNode xmlNodes in xmlFieldCollection)
            {
                try
                {
                    if (xmlNodes.Attributes["ColName"] != null)
                    {
                        string value = xmlNodes.Attributes["Name"].Value;
                        string str = xmlNodes.Attributes["Type"].Value;
                        bool flag = (xmlNodes.Attributes["FromBaseType"] == null
                            ? false
                            : xmlNodes.Attributes["FromBaseType"].Value == "TRUE");
                        if (!(value == "_UIVersionString") && !(value == "Modified"))
                        {
                            XmlNode versionCollection =
                                webServiceForLists.GetVersionCollection(sListID, iItemID.ToString(), value);
                            if (versionCollection != null && versionCollection.ChildNodes.Count != 0)
                            {
                                List<string> strs = new List<string>(versionCollection.ChildNodes.Count);
                                foreach (XmlNode childNode in versionCollection.ChildNodes)
                                {
                                    string versionCellData =
                                        this.GetVersionCellData(str, flag, childNode.Attributes[value].Value);
                                    string value1 = childNode.Attributes["Modified"].Value;
                                    int num = 0;
                                    while (strs.Contains(value1))
                                    {
                                        value1 = string.Concat(childNode.Attributes["Modified"].Value, num.ToString());
                                        num++;
                                    }

                                    strs.Add(value1);
                                    versionDataTable.SetValue(timeToVersionMapAndInitializeData[value1], value,
                                        versionCellData);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private bool GetListOnQuickLaunch(XmlAttributeCollection listAttribs, string sWebStruct)
        {
            StandardizedUrl standardizedUrl =
                StandardizedUrl.StandardizeUrl(this, listAttribs["DefaultViewUrl"].InnerText);
            int num = sWebStruct.IndexOf("<li>eType=");
            while (num < sWebStruct.Length && num >= 0)
            {
                string rPCValue = this.GetRPCValue(sWebStruct, num + "<li>eType=".Length);
                if (string.IsNullOrEmpty(rPCValue) || rPCValue.ToLower() != "page")
                {
                    num = sWebStruct.IndexOf("<li>eType=", num + "<li>eType=".Length);
                }
                else
                {
                    int num1 = sWebStruct.IndexOf("<li>url=", num);
                    if (num1 < 0)
                    {
                        return false;
                    }

                    string str = this.GetRPCValue(sWebStruct, num1 + "<li>url=".Length);
                    if (!string.IsNullOrEmpty(str))
                    {
                        StandardizedUrl standardizedUrl1 = StandardizedUrl.StandardizeUrl(this, str);
                        if (standardizedUrl.WebRelative != null && standardizedUrl1.WebRelative != null)
                        {
                            if (this.IsSubUrl(standardizedUrl.WebRelative, standardizedUrl1.WebRelative))
                            {
                                return true;
                            }
                        }
                        else if (standardizedUrl.ServerRelative != null && standardizedUrl1.ServerRelative != null &&
                                 this.IsSubUrl(standardizedUrl.ServerRelative, standardizedUrl1.ServerRelative))
                        {
                            return true;
                        }

                        num = sWebStruct.IndexOf("<li>eType=", num + "<li>eType=".Length);
                    }
                    else
                    {
                        num = sWebStruct.IndexOf("<li>eType=", num + "<li>eType=".Length);
                    }
                }
            }

            return false;
        }

        public string GetListPropertiesClientOM(string sListTitle)
        {
            object[] objArray = new object[] { sListTitle, this };
            string str = null;
            if (this.ClientOMAvailable)
            {
                try
                {
                    str = (string)this.ExecuteClientOMMethod("GetListProperties", objArray);
                }
                catch (Exception exception)
                {
                }
            }

            return str;
        }

        public void GetListPropertiesForCSOM(string sListID, out XmlNode nodeList, out Hashtable listSettings,
            out XmlNode externalDataSource)
        {
            nodeList = null;
            listSettings = new Hashtable();
            externalDataSource = null;
            try
            {
                ListsService webServiceForLists = this.GetWebServiceForLists();
                try
                {
                    nodeList = webServiceForLists.GetList(sListID);
                    listSettings = this.GetListSettings(nodeList.Attributes["RootFolder"].Value);
                    externalDataSource = XmlUtility.RunXPathQuerySelectSingle(nodeList, "//sp:DataSource");
                }
                catch (Exception exception)
                {
                }
            }
            catch (Exception exception1)
            {
            }
        }

        public string GetLists()
        {
            StringWriter stringWriter = null;
            try
            {
                stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Lists");
                string str = null;
                if (base.SharePointVersion.IsSharePoint2007OrEarlier)
                {
                    string str1 = "method=get+web+struct&service_name=&eidHead=0&levels=-1&includeHead=true";
                    str = RPCUtil.SendRequest(this,
                        string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll"), str1);
                }

                ListsService webServiceForLists = null;
                webServiceForLists = this.GetWebServiceForLists();
                try
                {
                    foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(),
                                 "//sp:List"))
                    {
                        this.FillListXML(xmlTextWriter, xmlNodes.Attributes, null, null, str, null, null);
                    }
                }
                catch (Exception exception)
                {
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (Exception exception1)
            {
            }

            return stringWriter.ToString();
        }

        private string GetListServerTemplate(XmlNode list)
        {
            return this.GetListServerTemplate(list.Attributes);
        }

        private string GetListServerTemplate(XmlAttributeCollection listAtrs)
        {
            if (listAtrs["ServerTemplate"] == null)
            {
                return listAtrs["BaseTemplate"].Value;
            }

            return listAtrs["ServerTemplate"].Value;
        }

        public Hashtable GetListSettings(string rootFolder)
        {
            string str = rootFolder.Replace(this.ServerRelativeUrl, "");
            Hashtable hashtables = new Hashtable();
            if (str.Length > 0)
            {
                string str1 = string.Concat("method=getDocsMetaInfo&url_list=[", str.Substring(1), "]");
                string str2 = RPCUtil.SendRequest(this, string.Concat(this.Url, "/_vti_bin/_vti_aut/author.dll"), str1);
                char[] chrArray = new char[] { '\r', '\n' };
                string[] strArrays = str2.Split(chrArray);
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str3 = strArrays[i];
                    if ((str3.StartsWith("<li>vti_") ? true : str3.StartsWith("<li>__")) &
                        i < (int)strArrays.Length - 1)
                    {
                        string str4 = str3.Substring((str3.StartsWith("<li>vti_") ? 8 : 6));
                        string str5 = strArrays[i + 1].Substring(7);
                        if (!hashtables.Contains(str4))
                        {
                            hashtables.Add(str4, str5);
                            i++;
                        }
                    }
                }
            }

            return hashtables;
        }

        public string GetListTemplates()
        {
            StringWriter stringWriter = null;
            try
            {
                stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("ListTemplates");
                foreach (XmlNode childNode in this.GetWebServiceForWebs().GetListTemplates().ChildNodes)
                {
                    xmlTextWriter.WriteRaw(
                        childNode.OuterXml.Replace(" xmlns=\"http://schemas.microsoft.com/sharepoint/\"", ""));
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (Exception exception)
            {
            }

            return stringWriter.ToString();
        }

        public XmlNode GetListXMLCSOM(string listID)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            return XmlUtility.StringToXmlNode(webServiceForLists.GetList(listID).OuterXml);
        }

        private int GetLocale(string sUrl, uint iLanguage)
        {
            int num = (int)iLanguage;
            if (!this.IsPortal2003Connection && !string.IsNullOrEmpty(sUrl))
            {
                try
                {
                    string str = string.Concat(sUrl, "/_layouts",
                        (base.SharePointVersion.IsSharePoint2003 ? string.Concat("/", iLanguage.ToString()) : ""),
                        "/regionalsetng.aspx");
                    string str1 = this.DownloadURLToString(str);
                    if (!string.IsNullOrEmpty(str1))
                    {
                        string str2 = string.Concat("<\\s*select[^>]+id\\s*=\\s*\"",
                            (base.SharePointVersion.IsSharePoint2007OrLater
                                ? "ctl00_PlaceHolderMain_ctl00_ctl00_DdlwebLCID"
                                : "DdlwebLCID"), "\".*?>.*?</select");
                        Match match = Regex.Match(str1, str2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        if (match.Success)
                        {
                            string str3 = "Locale";
                            string str4 = string.Concat("(selected\\s*=\\s*\"(selected|true)\"|value\\s*=\\s*\"(?<",
                                str3, ">\\d+)\")");
                            Regex regex = new Regex(string.Concat("<\\s*option\\s*", str4, "[^>]*", str4));
                            Match match1 = regex.Match(str1, match.Index, match.Length);
                            if (match1.Success)
                            {
                                int num1 = 0;
                                if (int.TryParse(match1.Groups[str3].Value, out num1))
                                {
                                    num = num1;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return num;
        }

        private string GetModeratorViewID(XmlNode listNode)
        {
            try
            {
                if (base.SharePointVersion.IsSharePoint2003 && listNode != null &&
                    listNode.GetAttributeValueAsBoolean("EnableModeration"))
                {
                    XmlNode xmlNodes = listNode.SelectSingleNode("//Views/View[@ModerationType='Moderator']/@Name");
                    if (xmlNodes != null && !string.IsNullOrEmpty(xmlNodes.Value))
                    {
                        if (new Guid(xmlNodes.Value) != Guid.Empty)
                        {
                            return xmlNodes.Value;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
            }

            return null;
        }

        public string GetMySiteData(string sSiteURL)
        {
            return null;
        }

        public string GetNameFromID(string sID)
        {
            if (this.m_UserMap == null)
            {
                this.BuildUserMap();
            }

            string item = null;
            if (this.m_UserMap.ContainsKey(sID))
            {
                item = this.m_UserMap[sID];
            }

            return item;
        }

        private string GetNavigationStructureXMLFromWebStructureString(string sWebStructure,
            string[] globalNavHiddenUrls, string[] currentNavHiddenUrls)
        {
            char[] chrArray = new char[] { '\n' };
            string[] strArrays = sWebStructure.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> indexsOfNavNodeChunksByEid = this.GetIndexsOfNavNodeChunksByEid(strArrays);
            if (!indexsOfNavNodeChunksByEid.ContainsKey("0"))
            {
                throw new Exception(
                    string.Concat("Failed to fetch web struucture. Could not locate root node. Result: ",
                        sWebStructure));
            }

            int item = indexsOfNavNodeChunksByEid["0"];
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            this.WriteNavNodeXml(xmlTextWriter, item, strArrays, indexsOfNavNodeChunksByEid, globalNavHiddenUrls,
                currentNavHiddenUrls, false, false);
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        private bool GetNavNodeAdditionsAndUpdatesRequireIsVisibleChange(XmlNode additionsAndUpdatesNode,
            XmlNode currentNavStruct)
        {
            bool flag;
            if (additionsAndUpdatesNode == null)
            {
                return false;
            }

            IEnumerator enumerator = additionsAndUpdatesNode.ChildNodes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    if (!this.GetNavNodeExists(current, currentNavStruct))
                    {
                        XmlAttribute itemOf = current.Attributes["IsVisible"];
                        if (itemOf == null || !(itemOf.Value.ToLower() == "false"))
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }
                    else
                    {
                        flag = true;
                        return flag;
                    }
                }

                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        private bool GetNavNodeAdditionsAndUpdatesRequireIsVisibleChange2010(XmlNode additionsAndUpdatesNode)
        {
            bool flag;
            if (additionsAndUpdatesNode == null)
            {
                return false;
            }

            IEnumerator enumerator = additionsAndUpdatesNode.SelectNodes("//NavNode").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    if (int.Parse(current.Attributes["ID"].Value) >= 0)
                    {
                        flag = true;
                        return flag;
                    }
                    else
                    {
                        if (current.Attributes["IsVisible"].Value.ToLower() != "false")
                        {
                            continue;
                        }

                        flag = true;
                        return flag;
                    }
                }

                return false;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        private string GetNavNodeDTLPForUpdate(string sDateValue)
        {
            DateTime dateTime = DateTime.Parse(sDateValue, new System.Globalization.CultureInfo("en-US"));
            string str = dateTime.ToString("dd MMM yyyy HH:mm:ss zzz");
            return str.Remove(str.Length - 3, 1);
        }

        private string GetNavNodeEditRequest(XmlNode node, XmlNode existingNode, string sMType)
        {
            string value;
            string str;
            string value1 = node.Attributes["ID"].Value;
            string str1 = node.Attributes["ParentID"].Value;
            string str2 = (node.Attributes["IsExternal"].Value.ToLower() == "true" ? "link" : "page");
            if (node.Attributes["Title"] != null)
            {
                value = node.Attributes["Title"].Value;
            }
            else
            {
                value = (existingNode == null ? "" : existingNode.Attributes["Title"].Value);
            }

            string str3 = RPCUtil.EscapeAndEncodeValue(value);
            if (node.Attributes["Url"] != null)
            {
                str = node.Attributes["Url"].Value;
            }
            else
            {
                str = (existingNode == null ? "" : existingNode.Attributes["Title"].Value);
            }

            string str4 = RPCUtil.EscapeAndEncodeValue(HttpUtility.UrlDecode(str));
            string navNodeDTLPForUpdate = this.GetNavNodeDTLPForUpdate(node.Attributes["LastModified"].Value);
            string str5 = this.BuildNavNodeMetaInfoEntry(node);
            object[] objArray = new object[] { value1, str1, navNodeDTLPForUpdate, str2, sMType, str3, str4, str5 };
            return string.Format(
                "[eidMod={0};eidParent={1};eidRef=-2;DTLP={2};eType={3};mType={4};name={5};url={6};meta-info=[{7}]]",
                objArray);
        }

        private bool GetNavNodeExists(XmlNode node, XmlNode currentNavStructure)
        {
            return currentNavStructure.SelectSingleNode(string.Concat("//NavNode[@ID=\"", node.Attributes["ID"].Value,
                "\"]")) != null;
        }

        private void GetNavNodeHiddenGuids(out Guid[] allHiddenGuids, out Guid[] globalNavHiddenGuids,
            out Guid[] currentNavHiddenGuids)
        {
            string str;
            string str1;
            Hashtable hashtables = this.RPCProperties(this.Url);
            string item = (string)hashtables["GlobalNavigationExcludes"];
            string item1 = (string)hashtables["CurrentNavigationExcludes"];
            if (string.IsNullOrEmpty(item))
            {
                str = null;
            }
            else
            {
                str = HttpUtility.HtmlDecode(item);
            }

            globalNavHiddenGuids = Utils.SplitWebMetaInfoGuidList(str);
            if (string.IsNullOrEmpty(item1))
            {
                str1 = null;
            }
            else
            {
                str1 = HttpUtility.HtmlDecode(item1);
            }

            currentNavHiddenGuids = Utils.SplitWebMetaInfoGuidList(str1);
            allHiddenGuids = Utils.GetGuidCollectionUnion(globalNavHiddenGuids, currentNavHiddenGuids);
        }

        private void GetNavNodeHiddenUrls(out string[] globalNavHiddenUrls, out string[] currentNavHiddenUrls)
        {
            Guid[] guidArray;
            Guid[] guidArray1;
            Guid[] guidArray2;
            globalNavHiddenUrls = null;
            currentNavHiddenUrls = null;
            if (base.SharePointVersion.IsSharePoint2003)
            {
                return;
            }

            this.GetNavNodeHiddenGuids(out guidArray2, out guidArray, out guidArray1);
            if ((int)guidArray2.Length == 0)
            {
                return;
            }

            List<string> strs = new List<string>();
            Guid[] guidArray3 = guidArray;
            for (int i = 0; i < (int)guidArray3.Length; i++)
            {
                Guid guid = guidArray3[i];
                if (this.SubWebGuidDict.ContainsKey(guid))
                {
                    strs.Add(this.SubWebGuidDict[guid]);
                }
            }

            List<string> strs1 = new List<string>();
            Guid[] guidArray4 = guidArray1;
            for (int j = 0; j < (int)guidArray4.Length; j++)
            {
                Guid guid1 = guidArray4[j];
                if (this.SubWebGuidDict.ContainsKey(guid1))
                {
                    strs1.Add(this.SubWebGuidDict[guid1]);
                }
            }

            if (strs.Count == (int)guidArray.Length && strs1.Count == (int)guidArray1.Length)
            {
                globalNavHiddenUrls = strs.ToArray();
                currentNavHiddenUrls = strs1.ToArray();
                return;
            }

            Dictionary<Guid, string> pagesLibraryDocIDToURLMap = this.GetPagesLibraryDocIDToURLMap(guidArray2);
            Guid[] guidArray5 = guidArray;
            for (int k = 0; k < (int)guidArray5.Length; k++)
            {
                Guid guid2 = guidArray5[k];
                if (pagesLibraryDocIDToURLMap.ContainsKey(guid2))
                {
                    strs.Add(pagesLibraryDocIDToURLMap[guid2]);
                }
            }

            Guid[] guidArray6 = guidArray1;
            for (int l = 0; l < (int)guidArray6.Length; l++)
            {
                Guid guid3 = guidArray6[l];
                if (pagesLibraryDocIDToURLMap.ContainsKey(guid3))
                {
                    strs1.Add(pagesLibraryDocIDToURLMap[guid3]);
                }
            }

            globalNavHiddenUrls = strs.ToArray();
            currentNavHiddenUrls = strs1.ToArray();
        }

        private void GetNavNodeHiddenUrls(out string[] globalNavHiddenUrls, out string[] currentNavHiddenUrls,
            Guid[] globalNavHiddenGuids, Guid[] currentNavHiddenGuids, Dictionary<Guid, string> pagesLibDocs)
        {
            List<string> strs = new List<string>();
            Guid[] guidArray = globalNavHiddenGuids;
            for (int i = 0; i < (int)guidArray.Length; i++)
            {
                Guid guid = guidArray[i];
                if (this.SubWebGuidDict.ContainsKey(guid))
                {
                    strs.Add(this.SubWebGuidDict[guid]);
                }
                else if (pagesLibDocs.ContainsKey(guid))
                {
                    strs.Add(pagesLibDocs[guid]);
                }
            }

            List<string> strs1 = new List<string>();
            Guid[] guidArray1 = currentNavHiddenGuids;
            for (int j = 0; j < (int)guidArray1.Length; j++)
            {
                Guid guid1 = guidArray1[j];
                if (this.SubWebGuidDict.ContainsKey(guid1))
                {
                    strs1.Add(this.SubWebGuidDict[guid1]);
                }
                else if (pagesLibDocs.ContainsKey(guid1))
                {
                    strs1.Add(pagesLibDocs[guid1]);
                }
            }

            globalNavHiddenUrls = strs.ToArray();
            currentNavHiddenUrls = strs1.ToArray();
        }

        private bool GetNavNodeIsChildOf(string sEidOfChild, string sEidOfParent, XmlNode currentNavStructure)
        {
            string[] strArrays = new string[]
                { "//NavNode[@ID=\"", sEidOfParent, "\"]//NavNode[@ID=\"", sEidOfChild, "\"]" };
            return currentNavStructure.SelectSingleNode(string.Concat(strArrays)) != null;
        }

        private bool GetNavNodeIsHidden(string sNavNodeUrl, string[] hiddenUrls)
        {
            if (string.IsNullOrEmpty(sNavNodeUrl) || hiddenUrls == null)
            {
                return false;
            }

            string webRelative = StandardizedUrl.StandardizeUrl(this, sNavNodeUrl).WebRelative;
            string[] strArrays = hiddenUrls;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                if (strArrays[i] == webRelative)
                {
                    return true;
                }
            }

            return false;
        }

        private bool GetNavNodeIsInGlobalNav(string sEid, XmlNode currentNavStructure)
        {
            return this.GetNavNodeIsChildOf(sEid, "1002", currentNavStructure);
        }

        private bool GetNavNodeIsInQuickLaunch(string sEid, XmlNode currentNavStructure)
        {
            if (!base.SharePointVersion.IsSharePoint2003)
            {
                return this.GetNavNodeIsChildOf(sEid, "1025", currentNavStructure);
            }

            if (sEid == "1000")
            {
                return false;
            }

            return !this.GetNavNodeIsChildOf(sEid, "1002", currentNavStructure);
        }

        private string GetNavNodePropertyValue(string sLine)
        {
            return HttpUtility.HtmlDecode(this.GetStringAfterCharacter(sLine, '='));
        }

        private int GetNextColonPound(string strVal, int start)
        {
            int num;
            int num1 = start;
            int num2 = -1;
            try
            {
                while (num1 < strVal.Length)
                {
                    num1 = strVal.IndexOf(';', num1);
                    if (num1 > num2)
                    {
                        num2 = num1;
                        if (strVal[num1 + 1] != '#')
                        {
                            num1 += 2;
                        }
                        else
                        {
                            num = num1;
                            return num;
                        }
                    }
                    else
                    {
                        num = -1;
                        return num;
                    }
                }

                return -1;
            }
            catch
            {
                return -1;
            }

            return num;
        }

        public List<DataRow> GetNumberOfLatestItemVersions(int numberOfLatestVersions,
            VersionDataTrackingTable versionResults)
        {
            if (numberOfLatestVersions < 1)
            {
                throw new ArgumentOutOfRangeException("numberOfLatestVersions", "value is less than one");
            }

            if (versionResults == null)
            {
                throw new ArgumentNullException("versionResults");
            }

            List<DataRow> dataRows = null;
            dataRows = new List<DataRow>();
            int num = 0;
            int num1 = 0;
            int num2 = 0;
            this.AddNumberOfLatestVersionsIntoList(numberOfLatestVersions, versionResults, dataRows, ref num, ref num1,
                ref num2);
            this.AddMajorVersionOfCurrentDraftVersion(versionResults, dataRows, num1);
            dataRows.Reverse();
            return dataRows;
        }

        private Guid GetPageOrWebGuidFromUrl(string sUrl, Dictionary<Guid, string> pagesLibMap)
        {
            Guid key;
            if (string.IsNullOrEmpty(sUrl))
            {
                return Guid.Empty;
            }

            string webRelative = StandardizedUrl.StandardizeUrl(this, sUrl).WebRelative;
            foreach (KeyValuePair<Guid, string> subWebGuidDict in this.SubWebGuidDict)
            {
                if (subWebGuidDict.Value != webRelative)
                {
                    continue;
                }

                key = subWebGuidDict.Key;
                return key;
            }

            Dictionary<Guid, string>.Enumerator enumerator = pagesLibMap.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<Guid, string> current = enumerator.Current;
                    if (current.Value != webRelative)
                    {
                        continue;
                    }

                    key = current.Key;
                    return key;
                }

                return Guid.Empty;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return key;
        }

        private Dictionary<Guid, string> GetPagesLibraryDocIDToURLMap(Guid[] idsToFetch)
        {
            bool flag;
            string str;
            Dictionary<Guid, string> guids = new Dictionary<Guid, string>();
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(),
                "//sp:List[@ServerTemplate=\"850\"]");
            XmlNode[] listItems = null;
            if (xmlNodeLists.Count > 0)
            {
                string value = xmlNodeLists[0].Attributes["ID"].Value;
                string str1 = Utils.BuildPagesLibraryGuidFetchingQuery(idsToFetch);
                string str2 = "<ViewFields><FieldRef Name=\"FileRef\" /><FieldRef Name=\"UniqueId\" /></ViewFields>";
                string str3 =
                    "<QueryOptions><IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><ViewAttributes Scope='RecursiveAll' /><MeetingInstanceID>-2</MeetingInstanceID></QueryOptions>";
                listItems = this.GetListItems(webServiceForLists, value, null, XmlUtility.StringToXmlNode(str1),
                    XmlUtility.StringToXmlNode(str2), XmlUtility.StringToXmlNode(str3), null, out flag);
            }

            if (listItems == null || (int)listItems.Length == 0)
            {
                return guids;
            }

            XmlNode[] xmlNodeArrays = listItems;
            for (int i = 0; i < (int)xmlNodeArrays.Length; i++)
            {
                foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(xmlNodeArrays[i], "//z:row"))
                {
                    XmlAttribute itemOf = xmlNodes.Attributes["ows_FileRef"];
                    if (itemOf == null)
                    {
                        continue;
                    }

                    XmlAttribute xmlAttribute = xmlNodes.Attributes["ows_UniqueId"];
                    if (xmlAttribute == null)
                    {
                        continue;
                    }

                    string value1 = itemOf.Value;
                    int num = value1.IndexOf(";#");
                    if (num >= 0)
                    {
                        num += 2;
                        if (num == value1.Length - 1)
                        {
                            str = null;
                        }
                        else
                        {
                            str = value1.Substring(num);
                        }

                        value1 = str;
                    }

                    if (string.IsNullOrEmpty(value1))
                    {
                        continue;
                    }

                    Guid guid = new Guid(NWSAdapter.GetJustGUID(xmlAttribute.Value));
                    StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, string.Concat("/", value1));
                    guids.Add(guid, standardizedUrl.WebRelative);
                }
            }

            return guids;
        }

        private Dictionary<string, string> GetParentWebListIDs(string sWebURL)
        {
            XmlNodeList parentWebListXML = this.GetParentWebListXML(sWebURL);
            if (parentWebListXML == null)
            {
                return null;
            }

            Dictionary<string, string> strs = new Dictionary<string, string>();
            foreach (XmlNode xmlNodes in parentWebListXML)
            {
                XmlAttribute itemOf = xmlNodes.Attributes["ID"];
                if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
                {
                    continue;
                }

                XmlAttribute xmlAttribute = xmlNodes.Attributes["Title"];
                if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value))
                {
                    continue;
                }

                strs.Add(xmlAttribute.Value, itemOf.Value);
            }

            return strs;
        }

        private Dictionary<string, string> GetParentWebListTitles(string[] sListIDsToGet)
        {
            XmlNodeList parentWebListXML = this.GetParentWebListXML(this.Url);
            if (parentWebListXML == null)
            {
                return null;
            }

            Dictionary<string, string> strs = new Dictionary<string, string>();
            string[] strArrays = sListIDsToGet;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                char[] chrArray = new char[] { '{', '}' };
                string lower = str.Trim(chrArray).ToLower();
                foreach (XmlNode xmlNodes in parentWebListXML)
                {
                    XmlAttribute itemOf = xmlNodes.Attributes["ID"];
                    if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
                    {
                        continue;
                    }

                    string value = itemOf.Value;
                    char[] chrArray1 = new char[] { '{', '}' };
                    if (lower != value.Trim(chrArray1).ToLower())
                    {
                        continue;
                    }

                    XmlAttribute xmlAttribute = xmlNodes.Attributes["Title"];
                    if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value))
                    {
                        break;
                    }

                    strs.Add(str, xmlAttribute.Value);
                    break;
                }
            }

            return strs;
        }

        private XmlNodeList GetParentWebListXML(string sWebUrl)
        {
            string str = sWebUrl.TrimEnd(new char[] { '/' });
            int num = str.LastIndexOf('/');
            if (num < 0)
            {
                return null;
            }

            int num1 = str.IndexOf("://");
            if (num1 < 0)
            {
                return null;
            }

            if (num == num1 + 2)
            {
                return null;
            }

            str = str.Substring(0, num);
            ListsService webServiceForLists = this.GetWebServiceForLists(str);
            return XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(), ".//sp:List");
        }

        private string GetPermissionsUrl(string sListID, int iItemId, string sOpUrl)
        {
            return this.GetPermissionsUrl(this.Url, sListID, iItemId, sOpUrl);
        }

        private string GetPermissionsUrl(string sUrl, string sListID, int iItemId, string sOpUrl)
        {
            string str = string.Concat(this.Url, "/_layouts/", sOpUrl, ".aspx");
            if (!string.IsNullOrEmpty(sListID))
            {
                char[] chrArray = new char[] { '{', '}' };
                string str1 = string.Concat("{", sListID.Trim(chrArray), "}");
                string str2 = null;
                if (iItemId <= 0)
                {
                    str2 = string.Concat("?obj=", str1, ",list");
                }
                else
                {
                    string[] strArrays = new string[] { "?obj=", str1, ",", iItemId.ToString(), ",listitem" };
                    str2 = string.Concat(strArrays);
                }

                str = string.Concat(str, str2, "&List=", str1);
            }

            return str;
        }

        private string GetPortalListingGroupFromID(int iValue)
        {
            switch (iValue)
            {
                case 0:
                {
                    return "General";
                }
                case 1:
                {
                    return "Highlight";
                }
                case 2:
                {
                    return "Expert";
                }
            }

            return iValue.ToString();
        }

        public string GetPortalListingGroups()
        {
            return null;
        }

        public string GetPortalListingIDs()
        {
            if (!this.IsPortal2003Connection)
            {
                return null;
            }

            Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService webServiceForAreas =
                this.GetWebServiceForAreas();
            Guid[] areaListings = webServiceForAreas.GetAreaListings(new Guid(this.WebID));
            StringBuilder stringBuilder = new StringBuilder(1000);
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("PortalListings");
            Guid[] guidArray = areaListings;
            for (int i = 0; i < (int)guidArray.Length; i++)
            {
                this.FetchAndWritePortalListingTerseData(guidArray[i], xmlTextWriter, webServiceForAreas);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public string GetPortalListings(string sIDList)
        {
            if (!this.IsPortal2003Connection || string.IsNullOrEmpty(sIDList))
            {
                return null;
            }

            char[] chrArray = new char[] { ',' };
            string[] strArrays = sIDList.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            Guid[] guid = new Guid[(int)strArrays.Length];
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                guid[i] = new Guid(strArrays[i]);
            }

            Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService webServiceForAreas =
                this.GetWebServiceForAreas();
            StringBuilder stringBuilder = new StringBuilder(1000);
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("PortalListings");
            Guid[] guidArray = guid;
            for (int j = 0; j < (int)guidArray.Length; j++)
            {
                AreaListingData areaListingData = webServiceForAreas.GetAreaListingData(guidArray[j]);
                xmlTextWriter.WriteStartElement("Listing");
                xmlTextWriter.WriteAttributeString("ListingID", areaListingData.ListingID.ToString());
                xmlTextWriter.WriteAttributeString("PublishingStartDate",
                    (areaListingData.AppearanceDate.Year == 1753
                        ? ""
                        : Utils.FormatDate(Utils.MakeTrueUTCDateTime(areaListingData.AppearanceDate))));
                xmlTextWriter.WriteAttributeString("Created",
                    Utils.FormatDate(this.TimeZone.LocalTimeToUtc(areaListingData.CreationDate)));
                xmlTextWriter.WriteAttributeString("Comments", areaListingData.Description);
                xmlTextWriter.WriteAttributeString("PublishingExpirationDate",
                    (areaListingData.ExpirationDate.Year == 9999
                        ? ""
                        : Utils.FormatDate(Utils.MakeTrueUTCDateTime(areaListingData.ExpirationDate))));
                xmlTextWriter.WriteAttributeString("Group", this.GetPortalListingGroupFromID(areaListingData.GroupID));
                xmlTextWriter.WriteAttributeString("Image", areaListingData.LargeIconUrl);
                xmlTextWriter.WriteAttributeString("Modified",
                    Utils.FormatDate(Utils.MakeTrueUTCDateTime(areaListingData.LastModified)));
                xmlTextWriter.WriteAttributeString("Editor", areaListingData.LastModifiedBy);
                xmlTextWriter.WriteAttributeString("Order", areaListingData.Order.ToString());
                xmlTextWriter.WriteAttributeString("PersonSID", Utils.SidToString(areaListingData.PersonSID));
                xmlTextWriter.WriteAttributeString("Icon", areaListingData.SmallIconUrl);
                xmlTextWriter.WriteAttributeString("ApprovalStatus",
                    this.GetPortalListingStatusString(areaListingData.Status));
                xmlTextWriter.WriteAttributeString("Title", areaListingData.Title);
                xmlTextWriter.WriteAttributeString("Url", areaListingData.url);
                xmlTextWriter.WriteAttributeString("Author", "");
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        private string GetPortalListingStatusString(ListingStatus status)
        {
            switch (status)
            {
                case ListingStatus.Approved:
                {
                    return "Approved";
                }
                case ListingStatus.Rejected:
                {
                    return "Rejected";
                }
                case ListingStatus.Archived:
                {
                    return "Archived";
                }
            }

            return "Pending";
        }

        private string GetPublishingPageContent(string sWpp, int iPageLayoutIndex)
        {
            int num = sWpp.IndexOf("<li>", iPageLayoutIndex + 1) + "<li>".Length;
            num = sWpp.IndexOf("|", num) + 1;
            int num1 = sWpp.IndexOf("<", num);
            string serverRelative = sWpp.Substring(num, num1 - num).Trim();
            char[] chrArray = new char[] { ',' };
            string[] strArrays = serverRelative.Split(chrArray, 2);
            if ((int)strArrays.Length > 1)
            {
                serverRelative = strArrays[0];
            }

            if (!serverRelative.StartsWith("http://www.microsoft.com/publishing?DisconnectedPublishingPage"))
            {
                serverRelative = UrlUtils.ConvertFullUrlToServerRelative(serverRelative);
                if (Regex.IsMatch(serverRelative, "^/[0-9]+/[^/]+\\.aspx$", RegexOptions.IgnoreCase) &&
                    !UrlUtils.StartsWith(serverRelative, this.SiteCollectionServerRelativeUrl))
                {
                    throw new Exception(Metalogix.SharePoint.Adapters.NWS.Properties.Resources
                        .PageLayoutNotCreatedFromTemplateError);
                }

                sWpp = this.GetWebPartPageFromWebService(serverRelative);
            }

            return sWpp;
        }

        private void GetRecurringMeetingUIDAndCreateDate(string sMeetingInstanceListId, XmlNode meetingSeriesListNode,
            out string sUid, out DateTime createDate)
        {
            sUid = null;
            createDate = DateTime.Now;
            string listItems = this.GetListItems(sMeetingInstanceListId, "1",
                "<Fields><Field Name=\"EventUID\" Type=\"Text\" /><Field Name=\"Created\" Type=\"DateTime\" /></Fields>",
                null, true, ListItemQueryType.ListItem, meetingSeriesListNode.OuterXml, new GetListItemOptions());
            XmlNode xmlNode = XmlUtility.StringToXmlNode(listItems);
            if (xmlNode.ChildNodes.Count == 0)
            {
                return;
            }

            xmlNode = xmlNode.ChildNodes[0];
            XmlAttribute itemOf = xmlNode.Attributes["EventUID"];
            if (itemOf != null)
            {
                sUid = itemOf.Value;
            }

            itemOf = xmlNode.Attributes["Created"];
            if (itemOf != null)
            {
                Utils.TryParseDateAsUtc(itemOf.Value, out createDate);
            }
        }

        public string GetReferencedTaxonomyFullXml(string sReferencedTaxonomyXml)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        private bool GetRequestAccessEmail(string sPermissionsUrl, out string requestEmail)
        {
            bool flag;
            string str = string.Concat(sPermissionsUrl, "/_layouts/",
                (base.SharePointVersion.IsSharePoint2003 ? string.Concat(this.Language, "/") : ""),
                "setrqacc.aspx?type=web");
            try
            {
                string str1 = this.HttpGet(str);
                if (str1 != null)
                {
                    string str2 = (base.SharePointVersion.IsSharePoint2003
                        ? "<input name=\"txtEmail\""
                        : "<input name=\"ctl00$PlaceHolderMain$ctl00$txtEmail\"");
                    string str3 = "/>";
                    int num = str1.IndexOf(str2);
                    if (num >= 0)
                    {
                        int num1 = str1.IndexOf(str3, num);
                        string str4 = str1.Substring(num, num1 - num + str3.Length);
                        Match match = Regex.Match(str4, "\\bdisabled=\"(.*?)\"",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Match match1 = Regex.Match(str4, "\\bvalue=\"(.*?)\"",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Group item = match1.Groups[1];
                        if (!match.Success && item.Success)
                        {
                            requestEmail = item.Value;
                            flag = true;
                            return flag;
                        }
                    }

                    requestEmail = "";
                    flag = false;
                }
                else
                {
                    requestEmail = string.Empty;
                    flag = false;
                }
            }
            catch (WebException webException)
            {
                requestEmail = string.Empty;
                flag = false;
            }

            return flag;
        }

        public string GetRoleAssignments(string sListID, int iItemId)
        {
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("RoleAssignments");
            try
            {
                if (base.SharePointVersion.IsSharePoint2003)
                {
                    this.GetRoleAssignmentsForSPS2003(sListID, xmlTextWriter);
                }
                else if (!string.IsNullOrEmpty(sListID))
                {
                    this.GetRoleAssignmentsViaHttpGet(sListID, iItemId, xmlTextWriter);
                }
                else
                {
                    this.GetRoleAssignmentsViaWebServices(xmlTextWriter);
                }
            }
            finally
            {
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }

            return stringWriter.ToString();
        }

        private void GetRoleAssignmentsForSPS2003(string sListID, XmlTextWriter xmlWriter)
        {
            List<string> strs = null;
            if (!string.IsNullOrEmpty(sListID))
            {
                strs = new List<string>();
                PermissionsService webServiceForPermissions = this.GetWebServiceForPermissions();
                this.GetWebServiceForUserGroup();
                XmlNode list = this.GetWebServiceForLists().GetList(sListID);
                string value = list.Attributes["Title"].Value;
                XmlNode permissionCollection = webServiceForPermissions.GetPermissionCollection(value, "List");
                foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(permissionCollection,
                             "//d:Permission[@MemberIsUser=\"True\"]"))
                {
                    xmlWriter.WriteStartElement("RoleAssignment");
                    xmlWriter.WriteAttributeString("RoleName", xmlNodes.Attributes["UserLogin"].Value);
                    xmlWriter.WriteAttributeString("PrincipalName", xmlNodes.Attributes["UserLogin"].Value);
                    xmlWriter.WriteEndElement();
                }

                foreach (XmlNode xmlNodes1 in XmlUtility.RunXPathQuery(permissionCollection,
                             "//d:Permission[@MemberIsUser=\"False\"]"))
                {
                    strs.Add(xmlNodes1.Attributes["RoleName"].Value);
                }
            }

            this.GetRoleAssignmentsViaWebServices(xmlWriter, strs);
        }

        private void GetRoleAssignmentsViaHttpGet(string sListID, int iItemId, XmlWriter xmlWriter)
        {
            string permissionsUrl = this.GetPermissionsUrl(sListID, iItemId,
                (base.SharePointVersion.IsSharePoint2007OrLater ? "user" : this.Base2003));
            string str = this.HttpGet(permissionsUrl);
            int num = str.IndexOf("<div id=\"__gvctl00_PlaceHolderMain_rptrUsers__div");
            int num1 = str.IndexOf("</div>", num);
            string str1 = str.Substring(num, num1 - num);
            str1 = str1.Replace("<INPUT type=\"hidden\"   name=PrincipalType  value='", "");
            Regex regex = new Regex("(^.*<.*\n)|(^.*=.*\n)", RegexOptions.Multiline);
            str1 = regex.Replace(str1, "", str1.Length, 0);
            str1 = str1.Replace("\r", "");
            str1 = str1.Replace("\t", "");
            str1 = str1.Replace("' >", "");
            string[] strArrays = new string[] { "\n" };
            string[] strArrays1 = str1.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
            char[] chrArray = new char[] { ',' };
            if (!base.SharePointVersion.IsSharePoint2010OrLater)
            {
                for (int i = 0; i < (int)strArrays1.Length; i += 4)
                {
                    string str2 = strArrays1[i];
                    string str3 = strArrays1[i + 1];
                    string str4 = strArrays1[i + 2];
                    string[] strArrays2 = strArrays1[i + 3].Split(chrArray);
                    for (int j = 0; j < (int)strArrays2.Length; j++)
                    {
                        string str5 = strArrays2[j];
                        xmlWriter.WriteStartElement("RoleAssignment");
                        xmlWriter.WriteAttributeString("PrincipalName", str4.Trim());
                        xmlWriter.WriteAttributeString("RoleName", str5.Trim());
                        xmlWriter.WriteEndElement();
                    }
                }

                return;
            }

            for (int k = 0; k < (int)strArrays1.Length; k += 2)
            {
                string str6 = strArrays1[k];
                string str7 = str6;
                if (str6.Contains("("))
                {
                    int num2 = str6.IndexOf('(') + 1;
                    int num3 = str6.IndexOf(')');
                    str7 = str6.Substring(num2, num3 - num2);
                }

                string[] strArrays3 = strArrays1[k + 1].Split(chrArray);
                for (int l = 0; l < (int)strArrays3.Length; l++)
                {
                    string str8 = strArrays3[l];
                    xmlWriter.WriteStartElement("RoleAssignment");
                    xmlWriter.WriteAttributeString("PrincipalName", str7.Trim());
                    xmlWriter.WriteAttributeString("RoleName", str8.Trim());
                    xmlWriter.WriteEndElement();
                }
            }
        }

        private void GetRoleAssignmentsViaWebServices(XmlWriter xmlWriter)
        {
            this.GetRoleAssignmentsViaWebServices(xmlWriter, null);
        }

        private void GetRoleAssignmentsViaWebServices(XmlWriter xmlWriter, List<string> sVisibleRoleNames)
        {
            string value;
            string str;
            string value1;
            UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
            foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(webServiceForUserGroup.GetRoleCollectionFromWeb(),
                         "//d:Role"))
            {
                if (xmlNodes.Attributes["Name"] != null)
                {
                    value = xmlNodes.Attributes["Name"].Value;
                }
                else
                {
                    value = null;
                }

                string str1 = value;
                if (str1 == null || sVisibleRoleNames != null && !sVisibleRoleNames.Contains(str1))
                {
                    continue;
                }

                foreach (XmlNode xmlNodes1 in XmlUtility.RunXPathQuery(
                             webServiceForUserGroup.GetUserCollectionFromRole(str1), "//d:User"))
                {
                    if (xmlNodes1.Attributes["LoginName"] != null)
                    {
                        str = xmlNodes1.Attributes["LoginName"].Value;
                    }
                    else
                    {
                        str = null;
                    }

                    string str2 = str;
                    if (str2 == null)
                    {
                        continue;
                    }

                    xmlWriter.WriteStartElement("RoleAssignment");
                    xmlWriter.WriteAttributeString("RoleName", str1);
                    xmlWriter.WriteAttributeString("PrincipalName", str2);
                    xmlWriter.WriteEndElement();
                }

                foreach (XmlNode xmlNodes2 in XmlUtility.RunXPathQuery(
                             webServiceForUserGroup.GetGroupCollectionFromRole(str1), "//d:Group"))
                {
                    if (xmlNodes2.Attributes["Name"] != null)
                    {
                        value1 = xmlNodes2.Attributes["Name"].Value;
                    }
                    else
                    {
                        value1 = null;
                    }

                    string str3 = value1;
                    if (str3 == null)
                    {
                        continue;
                    }

                    xmlWriter.WriteStartElement("RoleAssignment");
                    xmlWriter.WriteAttributeString("RoleName", str1);
                    xmlWriter.WriteAttributeString("PrincipalName", str3);
                    xmlWriter.WriteEndElement();
                }
            }
        }

        public string GetRoles(string sListId)
        {
            string str;
            string[] strArrays;
            string[] strArrays1;
            string value;
            string value1;
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Roles");
            xmlTextWriter.WriteAttributeString("SharePointVersion", base.SharePointVersion.ToString());
            try
            {
                UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
                if (!base.SharePointVersion.IsSharePoint2003)
                {
                    foreach (XmlNode xmlNodes in XmlUtility.RunXPathQuery(
                                 webServiceForUserGroup.GetRolesAndPermissionsForSite(), "//d:Role"))
                    {
                        xmlTextWriter.WriteStartElement("Role");
                        xmlTextWriter.WriteAttributeString("RoleId", Convert.ToString(xmlNodes.Attributes["ID"].Value));
                        xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Name"].Value);
                        xmlTextWriter.WriteAttributeString("Description", xmlNodes.Attributes["Description"].Value);
                        xmlTextWriter.WriteAttributeString("PermMask", xmlNodes.Attributes["BasePermissions"].Value);
                        xmlTextWriter.WriteAttributeString("Hidden", xmlNodes.Attributes["Hidden"].Value);
                        xmlTextWriter.WriteEndElement();
                    }
                }
                else
                {
                    XmlNode roleCollectionFromWeb = webServiceForUserGroup.GetRoleCollectionFromWeb();
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter1 = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlTextWriter1.WriteStartElement("Roles");
                    foreach (XmlNode xmlNodes1 in XmlUtility.RunXPathQuery(roleCollectionFromWeb, "//d:Role"))
                    {
                        if (xmlNodes1.Attributes["Name"] != null)
                        {
                            value = xmlNodes1.Attributes["Name"].Value;
                        }
                        else
                        {
                            value = null;
                        }

                        string str1 = value;
                        if (str1 == null)
                        {
                            continue;
                        }

                        xmlTextWriter1.WriteStartElement("Role");
                        xmlTextWriter1.WriteAttributeString("RoleName", str1);
                        xmlTextWriter1.WriteEndElement();
                    }

                    xmlTextWriter1.WriteEndElement();
                    PermissionsService webServiceForPermissions = this.GetWebServiceForPermissions();
                    string title = null;
                    if (sListId == null)
                    {
                        SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.Url);
                        _sWebMetadata _sWebMetadatum = null;
                        _sWebWithTime[] _sWebWithTimeArray = null;
                        _sListWithTime[] _sListWithTimeArray = null;
                        _sFPUrl[] _sFPUrlArray = null;
                        webServiceForSiteData.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray,
                            out _sListWithTimeArray, out _sFPUrlArray, out str, out strArrays, out strArrays1);
                        title = _sWebMetadatum.Title;
                    }
                    else
                    {
                        XmlNode list = this.GetWebServiceForLists().GetList(sListId);
                        title = list.Attributes["Title"].Value;
                    }

                    XmlNode permissionCollection =
                        webServiceForPermissions.GetPermissionCollection(title, (sListId == null ? "Web" : "List"));
                    XmlNode roleCollection =
                        webServiceForUserGroup.GetRoleCollection(XmlUtility.StringToXmlNode(stringBuilder.ToString()));
                    foreach (XmlNode xmlNodes2 in XmlUtility.RunXPathQuery(roleCollection, "//d:Role"))
                    {
                        if (xmlNodes2.Attributes["Name"] != null)
                        {
                            value1 = xmlNodes2.Attributes["Name"].Value;
                        }
                        else
                        {
                            value1 = null;
                        }

                        string str2 = value1;
                        if (str2 == null)
                        {
                            continue;
                        }

                        xmlTextWriter.WriteStartElement("Role");
                        xmlTextWriter.WriteAttributeString("RoleId",
                            Convert.ToString(xmlNodes2.Attributes["ID"].Value));
                        xmlTextWriter.WriteAttributeString("Name", str2);
                        xmlTextWriter.WriteAttributeString("Description", xmlNodes2.Attributes["Description"].Value);
                        XmlNode xmlNodes3 = XmlUtility.MatchFirstAttributeValue("RoleName", str2,
                            XmlUtility.RunXPathQuery(permissionCollection, "//d:Permission"));
                        if (xmlNodes3 == null || xmlNodes3.Attributes["Mask"] == null)
                        {
                            xmlTextWriter.WriteAttributeString("PermMask", "0");
                        }
                        else
                        {
                            xmlTextWriter.WriteAttributeString("PermMask", xmlNodes3.Attributes["Mask"].Value);
                        }

                        bool flag = xmlNodes2.Attributes["Name"].Value == "Guest";
                        xmlTextWriter.WriteAttributeString("Hidden", flag.ToString());
                        xmlTextWriter.WriteEndElement();
                    }

                    foreach (XmlNode xmlNodes4 in XmlUtility.RunXPathQuery(permissionCollection,
                                 "//d:Permission[@MemberIsUser=\"True\"]"))
                    {
                        xmlTextWriter.WriteStartElement("Role");
                        xmlTextWriter.WriteAttributeString("Name", xmlNodes4.Attributes["UserLogin"].Value);
                        xmlTextWriter.WriteAttributeString("Description", "");
                        xmlTextWriter.WriteAttributeString("Hidden", false.ToString());
                        xmlTextWriter.WriteAttributeString("PermMask", xmlNodes4.Attributes["Mask"].Value);
                        xmlTextWriter.WriteEndElement();
                    }
                }
            }
            finally
            {
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }

            return stringWriter.ToString();
        }

        private string GetRootFolder(XmlAttributeCollection listAttribs)
        {
            string str;
            string str1;
            try
            {
                if (listAttribs["DirName"] == null || listAttribs["Name"] == null)
                {
                    string innerText = "";
                    if (base.SharePointVersion.IsSharePoint2007OrLater && listAttribs["RootFolder"] != null)
                    {
                        innerText = listAttribs["RootFolder"].InnerText;
                    }

                    if (string.IsNullOrEmpty(innerText))
                    {
                        string str2 = (listAttribs["DefaultViewUrl"] != null
                            ? listAttribs["DefaultViewUrl"].InnerText
                            : "");
                        int length = str2.Length;
                        if (length <= 0)
                        {
                            innerText = (this.GetListServerTemplate(listAttribs) != "212"
                                ? ""
                                : Utils.CombineUrls(this.ServerRelativeUrl, "pages"));
                        }
                        else
                        {
                            str = (!base.SharePointVersion.IsSharePoint2007OrLater
                                ? this.Url.Substring(this.Server.Length)
                                : listAttribs["WebFullUrl"].InnerText);
                            string str3 = "";
                            string str4 = "";
                            int num = str.Length;
                            if (num == 1)
                            {
                                if (length > num + 10)
                                {
                                    str3 = str2.Substring(num, 6);
                                    str4 = str2.Substring(num, 10);
                                }
                                else if (length > num + 6)
                                {
                                    str3 = str2.Substring(num, 6);
                                }
                            }
                            else if (length > num + 11)
                            {
                                str3 = str2.Substring(num + 1, 6);
                                str4 = str2.Substring(num + 1, 10);
                            }
                            else if (length > num + 7)
                            {
                                str3 = str2.Substring(num + 1, 6);
                            }

                            if (str3 == "Lists/" || str4 == "_catalogs/")
                            {
                                int num1 = str2.IndexOf('/', num + 1);
                                int num2 = str2.IndexOf('/', num1 + 1);
                                innerText = str2.Substring(0, num2);
                            }
                            else
                            {
                                int num3 = str2.IndexOf('/', num + 1);
                                innerText = str2.Substring(0, num3);
                            }
                        }
                    }

                    str1 = UrlUtils.EnsureLeadingSlash(innerText);
                }
                else
                {
                    string[] value = new string[] { listAttribs["DirName"].Value, listAttribs["Name"].Value };
                    str1 = UrlUtils.EnsureLeadingSlash(UrlUtils.ConcatUrls(value));
                }
            }
            catch
            {
                str1 = null;
            }

            return str1;
        }

        private string GetRPCValue(string rpcCall, int startIdx)
        {
            int num = rpcCall.IndexOf('<', startIdx);
            if (num < 0)
            {
                throw new Exception("No RPC value at specified index");
            }

            string str = rpcCall.Substring(startIdx, num - startIdx);
            if (str == null)
            {
                return null;
            }

            return str.Trim();
        }

        private static string GetServerPart(string sUrl)
        {
            if (sUrl == null)
            {
                return null;
            }

            string str = null;
            str = (!sUrl.StartsWith("https") ? "http://" : "https://");
            int num = sUrl.IndexOf('/', str.Length);
            if (num < 0)
            {
                return sUrl;
            }

            return sUrl.Substring(0, num);
        }

        public bool GetServerRelativeUrlWithinSiteCollection(string sServerRelativeUrl)
        {
            string str;
            string str1;
            bool flag;
            SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(this.Url);
            string str2 = this.ServerUrl.TrimEnd(new char[] { '/' });
            char[] chrArray = new char[] { '/' };
            string str3 = string.Concat(str2, "/", sServerRelativeUrl.TrimStart(chrArray));
            try
            {
                webServiceForSiteData.GetSiteUrl(str3, out str, out str1);
                if (!string.IsNullOrEmpty(str1))
                {
                    string webID = this.WebID;
                    char[] chrArray1 = new char[] { '{', '}' };
                    string str4 = webID.Trim(chrArray1);
                    char[] chrArray2 = new char[] { '{', '}' };
                    if (str4.Equals(str1.Trim(chrArray2), StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                        return flag;
                    }
                }

                flag = false;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        public override string GetServerVersion()
        {
            return "0";
        }

        private string GetSharePointHttpPostParameters(string sSharePointPageHtml, string sRegExNodePattern,
            Dictionary<string, string> dictParametersToSet)
        {
            string value;
            MatchCollection matchCollections = (new Regex(sRegExNodePattern)).Matches(sSharePointPageHtml);
            Dictionary<string, string> strs = new Dictionary<string, string>(dictParametersToSet);
            foreach (Match match in matchCollections)
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(match.Value);
                    string str = xmlDocument.DocumentElement.Attributes["name"].Value;
                    if (xmlDocument.DocumentElement.Attributes["value"] != null)
                    {
                        value = xmlDocument.DocumentElement.Attributes["value"].Value;
                    }
                    else
                    {
                        value = null;
                    }

                    string str1 = value;
                    if (str != null && str1 != null && !strs.ContainsKey(str))
                    {
                        strs.Add(str, str1);
                    }
                }
                catch
                {
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in strs)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append("&");
                }

                stringBuilder.Append(HttpUtility.UrlEncode(keyValuePair.Key));
                stringBuilder.Append("=");
                stringBuilder.Append(HttpUtility.UrlEncode(keyValuePair.Value));
            }

            return stringBuilder.ToString();
        }

        public string GetSharePointVersion()
        {
            if (string.IsNullOrEmpty(this.m_SharePointVersionString))
            {
                this.m_SharePointVersionString = NWSAdapter.GetVersionStringFromRPC(this);
            }

            return this.m_SharePointVersionString;
        }

        public string GetSite(bool bFetchFullXml)
        {
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Site");
            xmlTextWriter.WriteAttributeString("SiteID", this.SiteId);
            xmlTextWriter.WriteAttributeString("Url", this.SiteCollectionServerRelativeUrl);
            this.FillWebXML(xmlTextWriter, this.SiteCollectionUrl, null, bFetchFullXml, true);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string GetSiteCollections()
        {
            throw new NotImplementedException(
                "A Local Object Model or Metalogix SharePoint Web Service Extensions connection is required to browse site collections.");
        }

        public string GetSiteCollectionsOnWebApp(string sWebAppName)
        {
            throw new NotImplementedException();
        }

        private XmlNode GetSiteColumnXml(XmlNode sourceField, XmlNode targetExistingColumns)
        {
            XmlNode xmlNodes;
            if (sourceField.Attributes["Name"] == null || sourceField.Attributes["SourceID"] == null ||
                !Utils.IsGuid(sourceField.Attributes["SourceID"].Value) || sourceField.Attributes["Type"] == null ||
                sourceField.Attributes["Group"] == null)
            {
                return sourceField;
            }

            string value = sourceField.Attributes["Group"].Value;
            string str = sourceField.Attributes["Type"].Value;
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(targetExistingColumns,
                string.Concat(".//sp:Fields/sp:Field[@Name=\"", sourceField.Attributes["Name"].Value, "\"]"));
            if (xmlNodeLists == null || xmlNodeLists.Count == 0)
            {
                return sourceField;
            }

            IEnumerator enumerator = xmlNodeLists.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    if (current.Attributes["SourceID"] == null || !Utils.IsGuid(current.Attributes["SourceID"].Value) ||
                        current.Attributes["Type"] == null || current.Attributes["Group"] == null)
                    {
                        continue;
                    }

                    string value1 = current.Attributes["Group"].Value;
                    string str1 = current.Attributes["Type"].Value;
                    if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(str) ||
                        string.IsNullOrEmpty(str1) || str != str1 || value != value1)
                    {
                        continue;
                    }

                    xmlNodes = current;
                    return xmlNodes;
                }

                return sourceField;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return xmlNodes;
        }

        public string GetSiteQuotaTemplates()
        {
            throw new NotImplementedException();
        }

        public string GetSiteTheme(string sUrl)
        {
            string str = "method=list+themes";
            string str1 = RPCUtil.SendRequest(this, Utils.JoinUrl(sUrl, "_vti_bin/_vti_aut/author.dll"), str);
            int num = str1.IndexOf("document_name");
            if (num < 0)
            {
                return null;
            }

            int num1 = str1.IndexOf("=", num) + 1;
            int num2 = str1.IndexOf("<", num1);
            if (num1 < 0 || num2 < 0)
            {
                return null;
            }

            return str1.Substring(num1, num2 - num1).Trim();
        }

        public string GetSiteUsers()
        {
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Users");
            UserGroupService webServiceForUserGroup = this.GetWebServiceForUserGroup();
            XmlNodeList xmlNodeLists =
                XmlUtility.RunXPathQuery(webServiceForUserGroup.GetUserCollectionFromSite(), "//d:User");
            string innerText = null;
            string str = null;
            if (this.m_UserMap == null)
            {
                this.m_UserMap = new Dictionary<string, string>();
                this.m_ReverseUserMap = new Dictionary<string, string>();
            }

            this.m_UserMap.Clear();
            this.m_ReverseUserMap.Clear();
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                innerText = xmlNodes.Attributes["ID"].InnerText;
                str = xmlNodes.Attributes["LoginName"].InnerText;
                this.m_UserMap[innerText] = str.ToLowerInvariant();
                this.m_ReverseUserMap[str.ToLowerInvariant()] = innerText;
                xmlTextWriter.WriteStartElement("User");
                xmlTextWriter.WriteAttributeString("ID", innerText);
                xmlTextWriter.WriteAttributeString("SID", xmlNodes.Attributes["Sid"].InnerText);
                xmlTextWriter.WriteAttributeString("IsSiteAdmin", xmlNodes.Attributes["IsSiteAdmin"].InnerText);
                xmlTextWriter.WriteAttributeString("IsDomainGroup", xmlNodes.Attributes["IsDomainGroup"].InnerText);
                xmlTextWriter.WriteAttributeString("LoginName", str);
                xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Name"].InnerText);
                xmlTextWriter.WriteAttributeString("Email", xmlNodes.Attributes["Email"].InnerText);
                xmlTextWriter.WriteAttributeString("Notes", xmlNodes.Attributes["Notes"].InnerText);
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        private string GetSoapExceptionErrorCode(SoapException ex)
        {
            if (ex.Detail == null)
            {
                return null;
            }

            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(ex.Detail.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("non-microsoft-prefix", "http://schemas.microsoft.com/sharepoint/soap/");
            XmlNode xmlNodes = ex.Detail.SelectSingleNode("//non-microsoft-prefix:errorcode", xmlNamespaceManagers);
            if (xmlNodes == null)
            {
                return null;
            }

            return xmlNodes.InnerText;
        }

        public string GetSpecificWebPartsOnPage(string webPartPageServerRelativeUrl, IEnumerable<Guid> webPartIDs)
        {
            XmlNode xmlNode;
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            List<XmlNode> xmlNodes = new List<XmlNode>(100);
            foreach (Guid webPartID in webPartIDs)
            {
                string webPart2 = null;
                try
                {
                    webPart2 = webServiceForWebParts.GetWebPart2(webPartPageServerRelativeUrl, webPartID,
                        Storage.Shared, SPWebServiceBehavior.Version3);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    try
                    {
                        webPart2 = webServiceForWebParts.GetWebPart(webPartPageServerRelativeUrl, webPartID,
                            Storage.Shared);
                    }
                    catch
                    {
                        throw exception;
                    }
                }

                if (!string.IsNullOrEmpty(webPart2))
                {
                    webPart2 = webPart2.Replace("xmlns=\"\"", "");
                    if (webPart2.StartsWith("<?xml ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int num = webPart2.IndexOf('>');
                        if (num >= 0)
                        {
                            webPart2 = webPart2.Remove(0, num + 1);
                        }
                    }
                }

                try
                {
                    xmlNode = XmlUtility.StringToXmlNode(webPart2);
                }
                catch
                {
                    throw new Exception(string.Format(
                        Metalogix.SharePoint.Adapters.NWS.Properties.Resources.FailedToReadSpecificWebPartFromPage,
                        webPartID, webPartPageServerRelativeUrl, webPart2));
                }

                if (xmlNode.Attributes["ID"] == null)
                {
                    XmlAttribute str = xmlNode.OwnerDocument.CreateAttribute("ID");
                    str.Value = webPartID.ToString();
                    xmlNode.Attributes.Append(str);
                }

                xmlNodes.Add(xmlNode);
            }

            if (xmlNodes.Count > 0)
            {
                this.PostProcessWebPartXml(xmlNodes);
                this.UpdateWebPartsWithInfoFromPage(webPartPageServerRelativeUrl, webServiceForWebParts, xmlNodes);
            }

            StringBuilder stringBuilder = new StringBuilder(1000);
            stringBuilder.Append("<WebParts>");
            foreach (XmlNode xmlNode1 in xmlNodes)
            {
                stringBuilder.Append(xmlNode1.OuterXml);
            }

            stringBuilder.Append("</WebParts>");
            return stringBuilder.ToString();
        }

        public string GetStoragePointProfileConfiguration(string sSharePointPath)
        {
            throw new NotSupportedException("This method is not supported on NWS connections");
        }

        private string GetStringAfterCharacter(string sString, char cCharacter)
        {
            int num = sString.IndexOf(cCharacter);
            if (num < 0)
            {
                return sString;
            }

            if (num == sString.Length - 1)
            {
                return "";
            }

            return sString.Substring(num + 1);
        }

        public string GetSubWebs()
        {
            StringWriter stringWriter = null;
            bool flag = false;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Webs");
            XmlNode xmlNodes = null;
            if (!this.IsPortal2003Connection)
            {
                this.GetWebs(xmlNodes, xmlTextWriter);
            }
            else
            {
                Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService webServiceForAreas =
                    this.GetWebServiceForAreas();
                try
                {
                    Guid[] subAreas = webServiceForAreas.GetSubAreas(new Guid(this.WebID));
                    for (int i = 0; i < (int)subAreas.Length; i++)
                    {
                        AreaData areaData = null;
                        areaData = webServiceForAreas.GetAreaData(subAreas[i]);
                        if (!areaData.System)
                        {
                            xmlTextWriter.WriteStartElement("Web");
                            xmlTextWriter.WriteAttributeString("ID", subAreas[i].ToString());
                            this.FillWebXML(xmlTextWriter, string.Concat(this.SiteCollectionUrl, areaData.WebUrl),
                                areaData.AreaName, false, false);
                            xmlTextWriter.WriteEndElement();
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    if (exception.GetType().IsAssignableFrom(typeof(SoapException)))
                    {
                        if (!((SoapException)exception).Message.Contains(
                                "Object reference not set to an instance of an object"))
                        {
                            throw;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
            }

            if (flag)
            {
                this.GetWebs(xmlNodes, xmlTextWriter);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        public string GetSystemInfo()
        {
            return (new Metalogix.SharePoint.Adapters.SystemInfo(false)).ToXmlString();
        }

        private bool GetTargetIsMultipleDataList(XmlNode listNode)
        {
            bool flag;
            if (listNode == null)
            {
                return false;
            }

            XmlAttribute itemOf = listNode.Attributes["MultipleDataList"];
            if (itemOf != null && bool.TryParse(itemOf.Value, out flag))
            {
                return flag;
            }

            return false;
        }

        public string GetTermCollectionFromTerm(string sTermStoreId, string sTermGroupId, string sTermSetId,
            string sTermId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermCollectionFromTermSet(string sTermStoreId, string sTermGroupId, string sTermSetId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermGroups(string sTermStoreId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermSetCollection(string sTermStoreId, string sTermGroupId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermSets(string sTermGroupId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermsFromTermSet(string sTermSetId, bool bRecursive)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermsFromTermSetItem(string sTermSetItemId)
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        public string GetTermStores()
        {
            throw new NotImplementedException("This method has not been implemented on this adapter type.");
        }

        private Dictionary<string, string> GetTimeToVersionMapAndInitializeData(string listIDString,
            string itemIDString, VersionDataTrackingTable versionDataTable, ListsService listsSvs)
        {
            XmlNode versionCollection = listsSvs.GetVersionCollection(listIDString, itemIDString, "_UIVersionString");
            Dictionary<string, string> strs = new Dictionary<string, string>(versionCollection.ChildNodes.Count);
            foreach (XmlNode childNode in versionCollection.ChildNodes)
            {
                string value = childNode.Attributes["Modified"].Value;
                int num = 0;
                while (strs.ContainsKey(value))
                {
                    value = string.Concat(childNode.Attributes["Modified"].Value, num.ToString());
                    num++;
                }

                string versionCellData =
                    this.GetVersionCellData("Text", true, childNode.Attributes["_UIVersionString"].Value);
                strs.Add(value, versionCellData);
                versionDataTable.SetValue(versionCellData, "_UIVersionString", versionCellData);
                versionDataTable.SetValue(versionCellData, "Modified",
                    this.GetVersionCellData("DateTime", false, childNode.Attributes["Modified"].Value));
            }

            return strs;
        }

        private TimeZoneInformation GetTimeZoneFromID(int iID)
        {
            try
            {
                if (iID >= 0)
                {
                    return TimeZoneInformation.GetTimeZone(iID);
                }
            }
            catch
            {
            }

            return TimeZoneInformation.GetLocalTimeZone();
        }

        private string GetUserFromID(string sID)
        {
            if (this.m_UserMap == null)
            {
                this.BuildUserMap();
            }

            string item = null;
            try
            {
                item = this.m_UserMap[sID];
            }
            catch
            {
            }

            return item;
        }

        public string GetUserFromProfile()
        {
            return null;
        }

        public int GetUserProfileByIndex(int profileIndex, out string personalSiteUrl)
        {
            return this.GetWebServiceForUserProfile().GetUserProfileByIndex(profileIndex, out personalSiteUrl);
        }

        public long GetUserProfileCount()
        {
            return this.GetWebServiceForUserProfile().GetUserProfileCount();
        }

        public string GetUserProfiles(string siteURL, string loginName, out string errors)
        {
            errors = string.Empty;
            return this.GetWebServiceForUserProfile().GetUserProfileByName(siteURL, loginName, out errors);
        }

        private string GetVersionCellData(string sFieldType, bool bFromBaseType, string sFieldValue)
        {
            if (sFieldType == "DateTime" || sFieldType == "PublishingScheduleStartDateFieldType" ||
                sFieldType == "PublishingScheduleEndDateFieldType")
            {
                DateTime dateTime = DateTime.Parse(sFieldValue, this.CultureInfo, DateTimeStyles.AssumeUniversal);
                return Utils.FormatDate(dateTime.ToUniversalTime());
            }

            if (sFieldType == "User")
            {
                string winOrFormsUser = null;
                int num = sFieldValue.IndexOf(";#");
                if (num <= 0)
                {
                    winOrFormsUser = Utils.ConvertClaimStringUserToWinOrFormsUser(sFieldValue);
                }
                else
                {
                    string str = sFieldValue.Substring(0, num);
                    string str1 = sFieldValue.Substring(num + 2);
                    winOrFormsUser = this.GetUserFromID(str);
                    if (string.IsNullOrEmpty(winOrFormsUser))
                    {
                        string[] strArrays = new string[] { ",#" };
                        string[] strArrays1 = str1.Split(strArrays, StringSplitOptions.None);
                        winOrFormsUser = ((int)strArrays1.Length <= 1
                            ? Utils.ConvertClaimStringUserToWinOrFormsUser(str1)
                            : Utils.ConvertClaimStringUserToWinOrFormsUser(strArrays1[1]));
                        if (string.IsNullOrEmpty(winOrFormsUser))
                        {
                            winOrFormsUser = sFieldValue;
                        }
                    }
                }

                return winOrFormsUser;
            }

            if (sFieldType == "UserMulti")
            {
                string str2 = sFieldValue.ToString();
                string[] strArrays2 = new string[] { ";#" };
                string[] strArrays3 = str2.Split(strArrays2, StringSplitOptions.None);
                int num1 = -1;
                if (int.TryParse(strArrays3[0], out num1))
                {
                    string str3 = null;
                    for (int i = 0; i < (int)strArrays3.Length; i += 2)
                    {
                        string userFromID = this.GetUserFromID(strArrays3[i]);
                        if (string.IsNullOrEmpty(userFromID))
                        {
                            userFromID = ((int)strArrays3.Length <= i + 1 ? strArrays3[i] : strArrays3[i + 1]);
                        }

                        str3 = (!string.IsNullOrEmpty(str3) ? string.Concat(str3, ",", userFromID) : userFromID);
                    }

                    sFieldValue = str3;
                }
                else
                {
                    sFieldValue = strArrays3[0];
                }

                return sFieldValue;
            }

            if (sFieldType == "Lookup" && !bFromBaseType)
            {
                int num2 = sFieldValue.IndexOf(";#");
                if (num2 > 0)
                {
                    sFieldValue = sFieldValue.Substring(0, num2);
                }

                return sFieldValue;
            }

            if (sFieldType != "LookupMulti")
            {
                if (sFieldType == "Text" || sFieldType == "Note" || sFieldType == "HTML")
                {
                    return sFieldValue;
                }

                int num3 = sFieldValue.IndexOf(";#");
                if (num3 > 0)
                {
                    sFieldValue = sFieldValue.Substring(num3 + 2);
                }

                return sFieldValue;
            }

            string[] strArrays4 = new string[] { ";#" };
            string[] strArrays5 = sFieldValue.Split(strArrays4, StringSplitOptions.None);
            string str4 = "";
            for (int j = 0; j < (int)strArrays5.Length; j += 2)
            {
                int num4 = -1;
                if (!string.IsNullOrEmpty(strArrays5[j]) && int.TryParse(strArrays5[j], out num4))
                {
                    if (!string.IsNullOrEmpty(str4))
                    {
                        str4 = string.Concat(str4, ";#");
                    }

                    str4 = string.Concat(str4, strArrays5[j]);
                }
            }

            if (!string.IsNullOrEmpty(str4))
            {
                sFieldValue = str4;
            }

            return sFieldValue;
        }

        public static string GetVersionStringFromRPC(SharePointAdapter adapter)
        {
            string str = "method=open+service";
            string str1 =
                RPCUtil.SendRequest(adapter, Utils.JoinUrl(adapter.Url, "/_vti_bin/_vti_aut/author.dll"), str);
            string str2 = null;
            int num = str1.IndexOf("vti_extenderversion");
            if (num >= 0)
            {
                int num1 = str1.IndexOf("|", num + "vti_extenderversion".Length);
                if (num1 >= 0)
                {
                    int num2 = str1.IndexOf("\n", num1);
                    if (num2 >= 0)
                    {
                        str2 = str1.Substring(num1 + 1, num2 - num1 - 1);
                    }
                }
            }

            return str2;
        }

        public string GetWeb(bool bFetchFullXml)
        {
            StringWriter stringWriter = null;
            stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Web");
            this.FillWebXML(xmlTextWriter, this.Url, null, bFetchFullXml, true);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        public string GetWebApplications()
        {
            throw new NotImplementedException();
        }

        private CookieAwareWebClient GetWebClient()
        {
            CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient()
            {
                Credentials = this.Credentials.NetworkCredentials
            };
            this.IncludedCertificates.CopyCertificatesToCollection(cookieAwareWebClient.ClientCertificates);
            if (this.AdapterProxy != null)
            {
                cookieAwareWebClient.Proxy = this.AdapterProxy;
            }

            if (base.HasActiveCookieManager)
            {
                CookieContainer cookieContainer = new CookieContainer();
                this.CookieManager.AddCookiesTo(cookieContainer);
                cookieAwareWebClient.UseDefaultCredentials = true;
                cookieAwareWebClient.CookieContainer = cookieContainer;
            }

            return cookieAwareWebClient;
        }

        private void GetWebMeetingInstanceXML(XmlWriter xmlWriter)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(),
                ".//sp:List[@ServerTemplate='200']");
            if (xmlNodeLists.Count != 1)
            {
                return;
            }

            string value = xmlNodeLists[0].Attributes["ID"].Value;
            string listFieldXML = this.GetListFieldXML(value, webServiceForLists);
            string listItems = this.GetListItems(value, null, listFieldXML, null, true, ListItemQueryType.ListItem,
                null, new GetListItemOptions());
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(listItems);
            XmlNodeList xmlNodeLists1 = xmlDocument.DocumentElement.SelectNodes("./ListItem");
            List<XmlAttribute> xmlAttributes = new List<XmlAttribute>();
            List<string> strs = new List<string>();
            foreach (XmlNode xmlNodes in xmlNodeLists1)
            {
                XmlAttribute itemOf = xmlNodes.Attributes["EventUID"];
                if (itemOf == null)
                {
                    continue;
                }

                string str = itemOf.Value;
                string[] strArrays = str.Split(new char[] { ':' });
                if ((int)strArrays.Length < 4)
                {
                    continue;
                }

                string str1 = strArrays[(int)strArrays.Length - 1];
                string str2 = strArrays[(int)strArrays.Length - 3];
                int num = -1;
                Guid empty = Guid.Empty;
                try
                {
                    empty = new Guid(str2);
                }
                catch
                {
                }

                if (!int.TryParse(str1, out num) || empty == Guid.Empty)
                {
                    continue;
                }

                string str3 = "";
                for (int i = 0; i < (int)strArrays.Length - 4; i++)
                {
                    if (!string.IsNullOrEmpty(str3))
                    {
                        str3 = string.Concat(str3, ":");
                    }

                    str3 = string.Concat(str3, strArrays[i]);
                }

                XmlAttribute xmlAttribute = xmlNodes.OwnerDocument.CreateAttribute("EventUIDPrefix");
                XmlAttribute xmlAttribute1 = xmlNodes.OwnerDocument.CreateAttribute("EventUIDItemID");
                XmlAttribute xmlAttribute2 = xmlNodes.OwnerDocument.CreateAttribute("EventUIDListName");
                xmlAttribute.Value = str3;
                xmlAttribute1.Value = num.ToString();
                xmlAttribute2.Value = empty.ToString();
                xmlNodes.Attributes.Remove(itemOf);
                xmlNodes.Attributes.Append(xmlAttribute);
                xmlNodes.Attributes.Append(xmlAttribute2);
                xmlNodes.Attributes.Append(xmlAttribute1);
                xmlAttributes.Add(xmlAttribute2);
                if (strs.Contains(empty.ToString()))
                {
                    continue;
                }

                strs.Add(empty.ToString());
            }

            if (strs.Count > 0)
            {
                Dictionary<string, string> parentWebListTitles = this.GetParentWebListTitles(strs.ToArray());
                if (parentWebListTitles != null)
                {
                    foreach (XmlAttribute item in xmlAttributes)
                    {
                        if (!parentWebListTitles.ContainsKey(item.Value))
                        {
                            continue;
                        }

                        item.Value = parentWebListTitles[item.Value];
                    }
                }
            }

            xmlWriter.WriteStartElement("MeetingInstances");
            foreach (XmlNode xmlNodes1 in xmlNodeLists1)
            {
                xmlWriter.WriteRaw(xmlNodes1.OuterXml.Replace("<ListItem ", "<MeetingInstance "));
            }

            xmlWriter.WriteEndElement();
        }

        public string GetWebNavigationSettings()
        {
            StringWriter stringWriter = null;
            try
            {
                stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("Web");
                xmlTextWriter.WriteAttributeString("Name", Utils.GetNameFromURL(this.Url));
                this.GetWebNavigationXML(this.RPCProperties(this.Url), xmlTextWriter);
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (Exception exception)
            {
            }

            return stringWriter.ToString();
        }

        public string GetWebNavigationStructure()
        {
            string[] strArrays;
            string[] strArrays1;
            this.GetNavNodeHiddenUrls(out strArrays, out strArrays1);
            if (!base.SharePointVersion.IsSharePoint2007OrEarlier)
            {
                return this.GetWebNavigationStructureClientOM(this, strArrays, strArrays1);
            }

            string webStructure = this.GetWebStructure();
            if (webStructure.IndexOf("<li>eid=") < 0)
            {
                throw new Exception(string.Concat("Failed to fetch web structure. Result: ", webStructure));
            }

            return this.GetNavigationStructureXMLFromWebStructureString(webStructure, strArrays, strArrays1);
        }

        public string GetWebNavigationStructureClientOM(SharePointAdapter callingAdapter, string[] hiddenGlobalUrls,
            string[] hiddenCurrentUrls)
        {
            object[] objArray = new object[] { callingAdapter, hiddenGlobalUrls, hiddenCurrentUrls };
            return (string)this.ExecuteClientOMMethod("GetWebNavigationStructure", objArray);
        }

        public void GetWebNavigationXML(Hashtable webRpcProperties, XmlWriter xmlWriter)
        {
            bool flag;
            bool flag1;
            if (webRpcProperties == null)
            {
                return;
            }

            if (webRpcProperties.ContainsKey("NavigationOrderingMethod"))
            {
                xmlWriter.WriteAttributeString("NavigationOrderingMethod",
                    webRpcProperties["NavigationOrderingMethod"].ToString());
            }

            if (webRpcProperties.ContainsKey("NavigationSortAscending"))
            {
                xmlWriter.WriteAttributeString("NavigationSortAscending",
                    webRpcProperties["NavigationSortAscending"].ToString());
            }

            if (webRpcProperties.ContainsKey("NavigationShowSiblings"))
            {
                xmlWriter.WriteAttributeString("NavigationShowSiblings",
                    webRpcProperties["NavigationShowSiblings"].ToString());
            }

            if (webRpcProperties.ContainsKey("NavigationAutomaticSortingMethod"))
            {
                xmlWriter.WriteAttributeString("NavigationAutomaticSortingMethod",
                    webRpcProperties["NavigationAutomaticSortingMethod"].ToString());
            }

            if (webRpcProperties.ContainsKey("InheritCurrentNavigation"))
            {
                xmlWriter.WriteAttributeString("InheritCurrentNavigation",
                    webRpcProperties["InheritCurrentNavigation"].ToString());
            }

            bool? nullable = null;
            bool? nullable1 = null;
            bool? nullable2 = null;
            bool? nullable3 = null;
            if (webRpcProperties.ContainsKey("IncludePagesInNavigation") &&
                bool.TryParse(webRpcProperties["IncludePagesInNavigation"].ToString(), out flag))
            {
                nullable = new bool?(flag);
                nullable2 = new bool?(flag);
            }

            if (webRpcProperties.ContainsKey("IncludeSubSitesInNavigation") &&
                bool.TryParse(webRpcProperties["IncludeSubSitesInNavigation"].ToString(), out flag1))
            {
                nullable1 = new bool?(flag1);
                nullable3 = new bool?(flag1);
            }

            int num = 0;
            int num1 = 0;
            if (base.SharePointVersion.IsSharePoint2010OrLater)
            {
                if (webRpcProperties.ContainsKey("CurrentNavigationIncludeTypes") &&
                    !int.TryParse(webRpcProperties["CurrentNavigationIncludeTypes"].ToString(), out num1))
                {
                    num1 = 0;
                }

                if (webRpcProperties.ContainsKey("GlobalNavigationIncludeTypes") &&
                    !int.TryParse(webRpcProperties["GlobalNavigationIncludeTypes"].ToString(), out num))
                {
                    num = 0;
                }

                if (!nullable.HasValue)
                {
                    nullable = new bool?((num1 & 2) > 0);
                }

                if (!nullable2.HasValue)
                {
                    nullable2 = new bool?((num & 2) > 0);
                }

                if (!nullable1.HasValue)
                {
                    nullable1 = new bool?((num1 & 1) > 0);
                }

                if (!nullable3.HasValue)
                {
                    nullable3 = new bool?((num & 1) > 0);
                }
            }

            nullable = (!nullable.HasValue ? new bool?(false) : nullable);
            nullable2 = (!nullable2.HasValue ? new bool?(false) : nullable2);
            nullable1 = (!nullable1.HasValue ? new bool?(false) : nullable1);
            nullable3 = (!nullable3.HasValue ? new bool?(false) : nullable3);
            xmlWriter.WriteAttributeString("IncludePagesInCurrentNavigation", nullable.Value.ToString());
            xmlWriter.WriteAttributeString("IncludePagesInGlobalNavigation", nullable2.Value.ToString());
            xmlWriter.WriteAttributeString("IncludeSubSitesInCurrentNavigation", nullable1.Value.ToString());
            xmlWriter.WriteAttributeString("IncludeSubSitesInGlobalNavigation", nullable3.Value.ToString());
            if (webRpcProperties.ContainsKey("DisplayShowHideRibbonActionId"))
            {
                xmlWriter.WriteAttributeString("DisplayShowHideRibbonActionId",
                    webRpcProperties["DisplayShowHideRibbonActionId"].ToString());
            }

            if (webRpcProperties.ContainsKey("CurrentDynamicChildLimit"))
            {
                xmlWriter.WriteAttributeString("CurrentDynamicChildLimit",
                    webRpcProperties["CurrentDynamicChildLimit"].ToString());
            }
            else if (base.SharePointVersion.IsSharePoint2007)
            {
                xmlWriter.WriteAttributeString("CurrentDynamicChildLimit", "50");
            }

            if (webRpcProperties.ContainsKey("GlobalDynamicChildLimit"))
            {
                xmlWriter.WriteAttributeString("GlobalDynamicChildLimit",
                    webRpcProperties["GlobalDynamicChildLimit"].ToString());
                return;
            }

            if (base.SharePointVersion.IsSharePoint2007)
            {
                xmlWriter.WriteAttributeString("GlobalDynamicChildLimit", "50");
            }
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl)
        {
            return this.GetWebPartPage(sWebPartPageServerRelativeUrl, true, this);
        }

        public string GetWebPartPage(string sWebPartPageServerRelativeUrl, bool bIncludeWebParts,
            ISharePointReader webPartFetchAdapter)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("WebPartPage");
            if (!string.IsNullOrEmpty(sWebPartPageServerRelativeUrl))
            {
                string str = "";
                string str1 = "";
                Utils.ParseUrlForLeafName(sWebPartPageServerRelativeUrl, out str, out str1);
                string webPartPageFromWebService = this.GetWebPartPageFromWebService(sWebPartPageServerRelativeUrl);
                if (webPartPageFromWebService != null)
                {
                    xmlTextWriter.WriteAttributeString("FileDirRef", str);
                    xmlTextWriter.WriteAttributeString("FileLeafRef", str1);
                    string str2 = this.ExtractDocumentGuid(webPartPageFromWebService);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        xmlTextWriter.WriteAttributeString("UniqueId", str2);
                    }

                    int num = webPartPageFromWebService.IndexOf("<li>vti_setuppath");
                    if (num < 0)
                    {
                        num = webPartPageFromWebService.IndexOf("<li>PublishingPageLayout\n");
                        if (num >= 0)
                        {
                            int num1 = webPartPageFromWebService.IndexOf("<li>", num + 1) + "<li>".Length;
                            string str3 = webPartPageFromWebService.Substring(num1);
                            char[] chrArray = new char[] { ',' };
                            str3 = str3.Substring(0, str3.IndexOfAny(chrArray));
                            int num2 = str3.LastIndexOf('/');
                            str3 = str3.Substring(num2 + 1, str3.Length - 1 - num2);
                            xmlTextWriter.WriteAttributeString("PageLayout", str3);
                        }
                    }
                    else
                    {
                        int num3 = webPartPageFromWebService.IndexOf("<li>", num + 1) + "<li>".Length;
                        num3 = webPartPageFromWebService.IndexOf("|", num3 + 1) + 1;
                        int num4 = webPartPageFromWebService.IndexOf("\n", num3) - 1;
                        string str4 = webPartPageFromWebService.Substring(num3, num4 - num3 + 1);
                        xmlTextWriter.WriteAttributeString("TemplateFile", HttpUtility.HtmlDecode(str4));
                    }

                    int num5 = webPartPageFromWebService.IndexOf("<li>PublishingPageLayout\n");
                    if (num5 >= 0)
                    {
                        webPartPageFromWebService = this.GetPublishingPageContent(webPartPageFromWebService, num5);
                    }

                    int num6 = webPartPageFromWebService.IndexOf("</html>") + "</html>".Length;
                    if (num6 >= 0)
                    {
                        WebPartUtils.ParseWebPartPageToXml(xmlTextWriter,
                            webPartPageFromWebService.Substring(num6 + 1));
                    }
                }

                if (bIncludeWebParts)
                {
                    string webPartsOnPage = webPartFetchAdapter.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
                    try
                    {
                        string str5 = Regex.Replace(webPartPageFromWebService, "\\s__designer:\\w*?=\".*?\"", "",
                            RegexOptions.Singleline);
                        MatchCollection matchCollections =
                            (new Regex("<WebPartPages:SPUserCodeWebPart.*?(/>|</WebPartPages:SPUserCodeWebPart>)",
                                RegexOptions.Singleline)).Matches(str5);
                        StringWriter stringWriter1 = new StringWriter(new StringBuilder());
                        XmlTextWriter xmlTextWriter1 = new XmlTextWriter(stringWriter1);
                        xmlTextWriter1.WriteStartElement("WebPartPages");
                        foreach (Match match in matchCollections)
                        {
                            xmlTextWriter1.WriteRaw(match.Value.Replace("WebPartPages:SPUserCodeWebPart",
                                "WebPartPages_SPUserCodeWebPart"));
                        }

                        xmlTextWriter1.WriteEndElement();
                        XmlNode xmlNode = XmlUtility.StringToXmlNode(stringWriter1.ToString());
                        XmlNode xmlNodes = XmlUtility.StringToXmlNode(webPartsOnPage);
                        XmlNamespaceManager xmlNamespaceManagers =
                            new XmlNamespaceManager(xmlNodes.OwnerDocument.NameTable);
                        xmlNamespaceManagers.AddNamespace("ns1", "http://microsoft.com/sharepoint/webpartpages");
                        xmlNamespaceManagers.AddNamespace("ns2", "http://schemas.microsoft.com/WebPart/v3");
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            string value = childNode.Attributes["__WebPartId"].Value;
                            char[] chrArray1 = new char[] { '{', '}' };
                            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(
                                string.Concat("/ns1:WebParts/ns1:WebPart[@ID=\"", value.Trim(chrArray1).ToLower(),
                                    "\"]/ns2:webPart/ns2:metaData"), xmlNamespaceManagers);
                            if (xmlNodes1 == null)
                            {
                                continue;
                            }

                            xmlNodes1.SelectSingleNode("ns2:type", xmlNamespaceManagers).Attributes["name"].Value =
                                string.Concat(childNode.Attributes["TypeFullName"].Value, ",",
                                    childNode.Attributes["AssemblyFullName"].Value);
                            XmlNode xmlNodes2 = xmlNodes1.AppendChild(
                                xmlNodes1.OwnerDocument.CreateNode(XmlNodeType.Element, "Solution",
                                    "http://schemas.microsoft.com/sharepoint/"));
                            ((XmlElement)xmlNodes2).SetAttribute("SolutionId",
                                childNode.Attributes["SolutionId"].Value);
                        }

                        xmlNodes.WriteTo(xmlTextWriter);
                    }
                    catch
                    {
                        xmlTextWriter.WriteRaw(webPartsOnPage);
                    }
                }
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            return stringWriter.ToString();
        }

        private string GetWebPartPageFromWebService(string sWebPartPageServerRelativeUrl)
        {
            string webPartPageDocument = null;
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            string str = Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl);
            try
            {
                webPartPageDocument = webServiceForWebParts.GetWebPartPageDocument(str);
            }
            catch (Exception exception3)
            {
                Exception exception = exception3;
                string str1 = string.Concat("The web part page '", str,
                    "' could not be retrieved through web services. Error: ");
                if (!base.SharePointVersion.IsSharePoint2007OrLater)
                {
                    throw new Exception(string.Concat(str1, exception.Message), exception);
                }

                try
                {
                    webPartPageDocument = webServiceForWebParts.GetWebPartPage(str, SPWebServiceBehavior.Version3);
                }
                catch (Exception exception2)
                {
                    Exception exception1 = exception2;
                    throw new Exception(string.Concat(str1, exception1.Message), exception1);
                }
            }

            return webPartPageDocument;
        }

        public byte[] GetWebPartPageTemplate(int iTemplateId)
        {
            return this.WebPartPageTemplateManager.GetTemplate(iTemplateId);
        }

        public string GetWebPartsOnPage(string sWebPartPageServerRelativeUrl)
        {
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            XmlNode webPartProperties2 = null;
            try
            {
                webPartProperties2 = webServiceForWebParts.GetWebPartProperties2(
                    Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl), Storage.Shared,
                    SPWebServiceBehavior.Version3);
            }
            catch
            {
                try
                {
                    webPartProperties2 =
                        webServiceForWebParts.GetWebPartProperties(
                            Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl), Storage.Shared);
                }
                catch
                {
                    if (webPartProperties2.LastChild is XmlComment)
                    {
                        throw new Exception(string.Concat("Fetching web parts for site ",
                            Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl), " failed. ",
                            ((XmlComment)webPartProperties2.LastChild).Value));
                    }
                }
            }

            XmlComment lastChild = webPartProperties2.LastChild as XmlComment;
            if (lastChild != null)
            {
                throw new Exception(lastChild.InnerText);
            }

            XmlNamespaceManager xmlNamespaceManagers = null;
            if (webPartProperties2.HasChildNodes)
            {
                xmlNamespaceManagers = new XmlNamespaceManager(webPartProperties2.FirstChild.OwnerDocument.NameTable);
                xmlNamespaceManagers.AddNamespace("v2LV", "http://schemas.microsoft.com/WebPart/v2/ListView");
                xmlNamespaceManagers.AddNamespace("v2", "http://schemas.microsoft.com/WebPart/v2");
            }

            this.PostProcessWebPartXml(webPartProperties2);
            this.UpdateWebPartsWithInfoFromPage(sWebPartPageServerRelativeUrl, webServiceForWebParts,
                webPartProperties2);
            webPartProperties2 =
                XmlUtility.StringToXmlNode(webPartProperties2.OuterXml.ToString().Replace("xmlns=\"\"", ""));
            webPartProperties2 = this.CorrectConnectionNodes(webPartProperties2);
            return webPartProperties2.OuterXml;
        }

        private void GetWebProperties(string sUrl, Hashtable ht, out string sWebTemplateID,
            out string sWebTemplateConfig, out string sMasterPageUrl)
        {
            sWebTemplateID = "";
            sWebTemplateConfig = "0";
            sMasterPageUrl = "";
            try
            {
                if (base.SharePointVersion.IsSharePoint2007OrLater)
                {
                    sMasterPageUrl = ht["masterurl"].ToString();
                }

                sWebTemplateID = ht["webtemplate"].ToString();
                int num = Convert.ToInt32(sWebTemplateID);
                if (num == 1)
                {
                    ListsService webServiceForLists = this.GetWebServiceForLists(sUrl);
                    XmlNodeList xmlNodeLists =
                        XmlUtility.RunXPathQuery(webServiceForLists.GetListCollection(), "//sp:List");
                    string key = "STS#0";
                    DictionaryEntry[] teamSiteTemplateVariations = NWSAdapter.TeamSiteTemplateVariations;
                    for (int i = 0; i < (int)teamSiteTemplateVariations.Length; i++)
                    {
                        DictionaryEntry dictionaryEntry = teamSiteTemplateVariations[i];
                        if (this.ListsContainsTemplates((ServerTemplate[])dictionaryEntry.Value, xmlNodeLists))
                        {
                            key = (string)dictionaryEntry.Key;
                        }
                    }

                    if (key == "STS#0")
                    {
                        try
                        {
                            if (this.WebPartPageZonesContains("Top"))
                            {
                                key = "STS#2";
                            }
                        }
                        catch
                        {
                        }
                    }

                    char[] chrArray = new char[] { '#' };
                    sWebTemplateConfig = key.Split(chrArray)[1];
                }
                else if (num != 2)
                {
                    sWebTemplateConfig = "0";
                }
                else
                {
                    ListsService listsService = this.GetWebServiceForLists(sUrl);
                    XmlNodeList xmlNodeLists1 = XmlUtility.RunXPathQuery(listsService.GetListCollection(), "//sp:List");
                    string str = "MPS#0";
                    DictionaryEntry[] meetingSiteTemplateVariations = NWSAdapter.MeetingSiteTemplateVariations;
                    for (int j = 0; j < (int)meetingSiteTemplateVariations.Length; j++)
                    {
                        DictionaryEntry dictionaryEntry1 = meetingSiteTemplateVariations[j];
                        if (this.ListsContainsTemplates((ServerTemplate[])dictionaryEntry1.Value, xmlNodeLists1))
                        {
                            str = (string)dictionaryEntry1.Key;
                        }
                    }

                    char[] chrArray1 = new char[] { '#' };
                    sWebTemplateConfig = str.Split(chrArray1)[1];
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void GetWebs(XmlNode sites, XmlTextWriter xmlWriter)
        {
            sites = this.GetWebServiceForWebs().GetWebCollection();
            foreach (XmlNode childNode in sites.ChildNodes)
            {
                try
                {
                    xmlWriter.WriteStartElement("Web");
                    this.FillWebXML(xmlWriter, childNode.Attributes["Url"].Value, childNode.Attributes["Title"].Value,
                        false, false);
                    xmlWriter.WriteEndElement();
                }
                catch (Exception exception)
                {
                }
            }
        }

        private Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService GetWebServiceForAreas(string sUrl)
        {
            Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService areaService = null;
            try
            {
                areaService = new Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService(this)
                {
                    Url = Utils.JoinUrl(sUrl, "_vti_bin/AreaService.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return areaService;
        }

        private Metalogix.SharePoint.Adapters.NWS.WrappedServices.AreaService GetWebServiceForAreas()
        {
            return this.GetWebServiceForAreas(this.Url);
        }

        private FormsService GetWebServiceForForms()
        {
            FormsService formsService = null;
            try
            {
                formsService = new FormsService(this);
            }
            catch (Exception exception)
            {
            }

            return formsService;
        }

        private ListsService GetWebServiceForLists(string url)
        {
            ListsService listsService = null;
            try
            {
                listsService = new ListsService(this)
                {
                    Url = Utils.JoinUrl(url, "_vti_bin/Lists.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return listsService;
        }

        private ListsService GetWebServiceForLists()
        {
            return this.GetWebServiceForLists(this.Url);
        }

        private ListsSPV2Service GetWebServiceForListsSPV2(string url)
        {
            ListsSPV2Service listsSPV2Service = null;
            try
            {
                listsSPV2Service = new ListsSPV2Service(this)
                {
                    Url = Utils.JoinUrl(url, "_vti_bin/Lists.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return listsSPV2Service;
        }

        private ListsSPV2Service GetWebServiceForListsSPV2()
        {
            return this.GetWebServiceForListsSPV2(this.Url);
        }

        private MeetingsService GetWebServiceForMeetings(string url)
        {
            MeetingsService meetingsService = null;
            try
            {
                meetingsService = new MeetingsService(this)
                {
                    Url = Utils.JoinUrl(url, "_vti_bin/Meetings.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return meetingsService;
        }

        private MeetingsService GetWebServiceForMeetings()
        {
            return this.GetWebServiceForMeetings(this.Url);
        }

        private PeopleService GetWebServiceForPeople()
        {
            PeopleService peopleService = null;
            try
            {
                peopleService = new PeopleService(this);
            }
            catch (Exception exception)
            {
            }

            return peopleService;
        }

        private PermissionsService GetWebServiceForPermissions()
        {
            PermissionsService permissionsService = null;
            try
            {
                permissionsService = new PermissionsService(this);
            }
            catch (Exception exception)
            {
            }

            return permissionsService;
        }

        private SiteDataService GetWebServiceForSiteData(string url)
        {
            SiteDataService siteDataService = null;
            try
            {
                siteDataService = new SiteDataService(this)
                {
                    Url = Utils.JoinUrl(url, "_vti_bin/SiteData.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return siteDataService;
        }

        private SitesService GetWebServiceForSites()
        {
            SitesService sitesService = null;
            try
            {
                sitesService = new SitesService(this);
            }
            catch (Exception exception)
            {
            }

            return sitesService;
        }

        private UserGroupService GetWebServiceForUserGroup()
        {
            UserGroupService userGroupService = null;
            try
            {
                userGroupService = new UserGroupService(this);
            }
            catch (Exception exception)
            {
            }

            return userGroupService;
        }

        private UserProfileService GetWebServiceForUserProfile()
        {
            UserProfileService userProfileService = null;
            try
            {
                userProfileService = new UserProfileService(this);
            }
            catch (Exception exception)
            {
            }

            return userProfileService;
        }

        private VersionsService GetWebServiceForVersions()
        {
            VersionsService versionsService = null;
            try
            {
                versionsService = new VersionsService(this);
            }
            catch (Exception exception)
            {
            }

            return versionsService;
        }

        private ViewsService GetWebServiceForViews()
        {
            ViewsService viewsService = null;
            try
            {
                viewsService = new ViewsService(this);
            }
            catch (Exception exception)
            {
            }

            return viewsService;
        }

        private WebPartPagesService GetWebServiceForWebParts()
        {
            WebPartPagesService webPartPagesService = null;
            try
            {
                webPartPagesService = new WebPartPagesService(this);
            }
            catch (Exception exception)
            {
            }

            return webPartPagesService;
        }

        private WebsService GetWebServiceForWebs(string sUrl)
        {
            WebsService websService = null;
            try
            {
                websService = new WebsService(this)
                {
                    Url = Utils.JoinUrl(sUrl, "_vti_bin/Webs.asmx")
                };
            }
            catch (Exception exception)
            {
            }

            return websService;
        }

        private WebsService GetWebServiceForWebs()
        {
            return this.GetWebServiceForWebs(this.Url);
        }

        private string GetWebStructure()
        {
            string str = "method=get+web+struct&service_name=&eidHead=0&levels=-1&includeHead=true";
            return RPCUtil.SendRequest(this, Utils.JoinUrl(this.Url, "/_vti_bin/_vti_aut/author.dll"), str);
        }

        public string GetWebTemplates()
        {
            Template[] templateArray;
            StringWriter stringWriter = null;
            try
            {
                stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("WebTemplates");
                if (this.GetWebServiceForSites()
                        .GetSiteTemplates(Convert.ToUInt32(this.RPCProperties(this.Url)["language"].ToString()),
                            out templateArray) == 0)
                {
                    Template[] templateArray1 = templateArray;
                    for (int i = 0; i < (int)templateArray1.Length; i++)
                    {
                        Template template = templateArray1[i];
                        xmlTextWriter.WriteStartElement("WebTemplate");
                        string[] strArrays = template.Name.Split(new char[] { '#' });
                        string name = template.Name;
                        string str = "0";
                        int num = -1;
                        int d = -1;
                        if ((int)strArrays.Length == 2)
                        {
                            if (int.TryParse(strArrays[1], out num))
                            {
                                str = strArrays[1];
                                d = template.ID;
                            }
                            else
                            {
                                name = strArrays[1];
                            }
                        }

                        xmlTextWriter.WriteAttributeString("Name", name);
                        xmlTextWriter.WriteAttributeString("ID", d.ToString());
                        xmlTextWriter.WriteAttributeString("Config", str);
                        xmlTextWriter.WriteAttributeString("FullName", template.Name);
                        xmlTextWriter.WriteAttributeString("Title", template.Title);
                        xmlTextWriter.WriteAttributeString("ImageUrl", template.ImageUrl);
                        xmlTextWriter.WriteAttributeString("Description", template.Description);
                        xmlTextWriter.WriteAttributeString("IsHidden", template.IsHidden.ToString());
                        xmlTextWriter.WriteAttributeString("IsRootWebOnly", template.IsRootWebOnly.ToString());
                        xmlTextWriter.WriteAttributeString("IsSubWebOnly", template.IsSubWebOnly.ToString());
                        xmlTextWriter.WriteEndElement();
                    }
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (SoapException soapException)
            {
                throw new Exception(RPCUtil.GetSoapError(soapException.Detail));
            }

            return stringWriter.ToString();
        }

        private int GetWebTimeZoneFromPage(string sWebUrl)
        {
            string str;
            int num;
            int num1;
            try
            {
                str = (!base.SharePointVersion.IsSharePoint2003
                    ? "/_layouts/regionalsetng.aspx"
                    : string.Concat("/_layouts/", this.Language, "/RegionalSetng.aspx"));
                char[] chrArray = new char[] { '/' };
                string str1 = string.Concat(sWebUrl.Trim(chrArray), str);
                string str2 = this.HttpGet(str1);
                Match match = Regex.Match(str2,
                    "<select[^>]*id=\"(DdlwebTimeZone|ctl00_PlaceHolderMain_ctl01_ctl00_DdlwebTimeZone)\"",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match == null || !match.Success)
                {
                    num1 = -1;
                }
                else
                {
                    int index = match.Index;
                    int num2 = str2.IndexOf("</select>", index, StringComparison.OrdinalIgnoreCase);
                    if (num2 >= index)
                    {
                        string str3 = str2.Substring(index, num2 - index);
                        Match match1 = Regex.Match(str3, "<option[^>]*selected=\"[^>]*>",
                            RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        if (match1 == null || !match1.Success)
                        {
                            num1 = -1;
                        }
                        else
                        {
                            Match match2 = Regex.Match(match1.Value, "value=\"(?<Value>[^\"]*)\"",
                                RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            if (match2 == null || !match2.Success)
                            {
                                num1 = -1;
                            }
                            else
                            {
                                num1 = (!int.TryParse(match2.Groups["Value"].Value, out num) ? -1 : num);
                            }
                        }
                    }
                    else
                    {
                        num1 = -1;
                    }
                }
            }
            catch
            {
                num1 = -1;
            }

            return num1;
        }

        private string GetWelcomePage(string sUrl)
        {
            string str = null;
            string str1 = "method=getDocsMetaInfo&url_list=[]";
            string str2 = RPCUtil.SendRequest(this, string.Concat(sUrl, "/_vti_bin/_vti_aut/author.dll"), str1);
            int num = str2.IndexOf("<li>vti_welcomepage", StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                string str3 = "<li>SW|";
                int length = str2.IndexOf(str3, num + 1, StringComparison.OrdinalIgnoreCase);
                if (length >= 0)
                {
                    length += str3.Length;
                    char[] chrArray = new char[] { '\r', '\n' };
                    int num1 = str2.IndexOfAny(chrArray, length);
                    str = (num1 < 0 ? str2.Substring(length) : str2.Substring(length, num1 - length));
                }
            }

            return str;
        }

        public string GetWorkflowAssociations(string sObjectId, string sObjectType)
        {
            return null;
        }

        public string GetWorkflows(string sListId, int sItemId)
        {
            return null;
        }

        private XmlDocument GetXmlDocument(List<XmlNode> nodeList)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (nodeList.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                xmlTextWriter.WriteStartElement("Fields");
                int num = 1;
                foreach (XmlNode xmlNodes in nodeList)
                {
                    xmlTextWriter.WriteStartElement("Method");
                    xmlTextWriter.WriteAttributeString("ID", num.ToString());
                    xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                    xmlTextWriter.WriteEndElement();
                    num++;
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlDocument.LoadXml(stringBuilder.ToString());
            }
            else
            {
                xmlDocument.LoadXml("<Fields />");
            }

            return xmlDocument;
        }

        private static string GetXmlNodeAttributeValue(XmlNode node, string attributeName)
        {
            if (node.Attributes[attributeName] == null)
            {
                return null;
            }

            return node.Attributes[attributeName].Value;
        }

        public string HasDocument(string sDocumentServerRelativeUrl)
        {
            string str = sDocumentServerRelativeUrl.ToLower().Replace(this.ServerRelativeUrl.ToLower(), "");
            char[] chrArray = new char[] { '/', '\\' };
            string str1 = string.Format("method=getDocsMetaInfo&url_list=[{0}]", str.TrimStart(chrArray));
            if (RPCUtil.SendRequest(this,
                        string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll"), str1)
                    .IndexOf("vti_timelastmodified") >= 0)
            {
                return bool.TrueString;
            }

            return bool.FalseString;
        }

        public string HasUniquePermissions(string sListId, int iItemId)
        {
            string falseString;
            try
            {
                if (iItemId < 0 || !base.SharePointVersion.IsSharePoint2003)
                {
                    string permissionsUrl = this.GetPermissionsUrl(sListId, iItemId,
                        (base.SharePointVersion.IsSharePoint2007OrLater ? "user" : this.Base2003));
                    string str = this.HttpGet(permissionsUrl);
                    if (str == null)
                    {
                        falseString = bool.FalseString;
                    }
                    else if (!base.SharePointVersion.IsSharePoint2003)
                    {
                        falseString = (!str.Contains("<a href=\"javascript:uniquePerms()\"")
                            ? bool.TrueString
                            : bool.FalseString);
                    }
                    else
                    {
                        falseString = Convert.ToString(str.Contains("<a href=\"javascript:ResetOpts()\""));
                    }
                }
                else
                {
                    falseString = bool.FalseString;
                }
            }
            catch
            {
                return bool.FalseString;
            }

            return falseString;
        }

        public bool HasUniqueRoles(string sUrl)
        {
            string permissionsUrl = this.GetPermissionsUrl(sUrl, null, -1, "role");
            string str = this.HttpGet(permissionsUrl);
            if (str != null && str.Contains("<a href=\"javascript:delRoles()\""))
            {
                return true;
            }

            return false;
        }

        public string HasWebParts(string sWebPartPageServerRelativeUrl)
        {
            bool flag = false;
            string webPartsOnPage = this.GetWebPartsOnPage(sWebPartPageServerRelativeUrl);
            if (!string.IsNullOrEmpty(webPartsOnPage) && XmlUtility.StringToXmlNode(webPartsOnPage)
                    .SelectNodes("./*[local-name() = 'WebPart']").Count > 0)
            {
                flag = true;
            }

            return Convert.ToString(flag);
        }

        public string HasWorkflows(string sListID, string sItemID)
        {
            return bool.FalseString;
        }

        private string HttpGet(string uri)
        {
            string end;
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                HttpWebRequest networkCredentials = (HttpWebRequest)WebRequest.Create(uri);
                networkCredentials.Credentials = this.Credentials.NetworkCredentials;
                networkCredentials.ContentType = "application/x-www-form-urlencoded";
                networkCredentials.Method = "GET";
                this.IncludedCertificates.CopyCertificatesToCollection(networkCredentials.ClientCertificates);
                if (this.AdapterProxy != null)
                {
                    networkCredentials.Proxy = this.AdapterProxy;
                }

                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                    networkCredentials.CookieContainer = new CookieContainer();
                    this.CookieManager.AddCookiesTo(networkCredentials.CookieContainer);
                }

                try
                {
                    WebResponse response = networkCredentials.GetResponse();
                    if (response != null)
                    {
                        end = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                        return end;
                    }
                    else
                    {
                        end = null;
                        return end;
                    }
                }
                catch (WebException webException)
                {
                }

                end = null;
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return end;
        }

        private string HttpPost(string uri, string parameters)
        {
            string end;
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                HttpWebRequest networkCredentials = (HttpWebRequest)WebRequest.Create(uri);
                networkCredentials.Credentials = this.Credentials.NetworkCredentials;
                networkCredentials.ContentType = "application/x-www-form-urlencoded";
                networkCredentials.Method = "POST";
                this.IncludedCertificates.CopyCertificatesToCollection(networkCredentials.ClientCertificates);
                if (this.AdapterProxy != null)
                {
                    networkCredentials.Proxy = this.AdapterProxy;
                }

                if (base.HasActiveCookieManager)
                {
                    this.CookieManager.UpdateCookie();
                    networkCredentials.CookieContainer = new CookieContainer();
                    this.CookieManager.AddCookiesTo(networkCredentials.CookieContainer);
                }

                byte[] bytes = Encoding.ASCII.GetBytes(parameters);
                Stream requestStream = null;
                try
                {
                    try
                    {
                        networkCredentials.ContentLength = (long)((int)bytes.Length);
                        requestStream = networkCredentials.GetRequestStream();
                        requestStream.Write(bytes, 0, (int)bytes.Length);
                    }
                    catch (WebException webException)
                    {
                    }
                }
                finally
                {
                    if (requestStream != null)
                    {
                        requestStream.Close();
                    }
                }

                try
                {
                    WebResponse response = networkCredentials.GetResponse();
                    if (response != null)
                    {
                        end = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                        return end;
                    }
                    else
                    {
                        end = null;
                        return end;
                    }
                }
                catch (WebException webException1)
                {
                }

                end = null;
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }

            return end;
        }

        private string[] IncrementDocumentVersions(string sListID, ListType listType, string sVersionString,
            bool bAdding, XmlNode listXML, XmlNode targetItemXML, ListsService listsSvs)
        {
            if (!bool.Parse(listXML.Attributes["EnableVersioning"].Value))
            {
                return new string[0];
            }

            string value = targetItemXML.Attributes["ows_ID"].Value;
            string str = targetItemXML.Attributes["ows__UIVersionString"].Value;
            string.Concat(this.Server, this.ServerRelativeUrl);
            ArrayList arrayLists = new ArrayList();
            int[] numArray = this.ParseVersionString(sVersionString);
            int[] numArray1 = this.ParseVersionString(str);
            int num = numArray[0] - numArray1[0];
            int num1 = (num == 0 ? numArray[1] - numArray1[1] : numArray[1]);
            string server = this.Server;
            string value1 = targetItemXML.Attributes["ows_FileRef"].Value;
            char[] chrArray = new char[] { '#' };
            string str1 = string.Concat(server, "/", value1.Split(chrArray)[1]);
            string value2 = targetItemXML.Attributes["ows_FileLeafRef"].Value;
            string[] strArrays = value2.Split(new char[] { '#' });
            if ((int)strArrays.Length > 1)
            {
                value2 = strArrays[1];
            }

            if (num < 0 || num == 0 && num1 <= 0 || !bAdding && num == 1 && num1 <= 0)
            {
                return new string[0];
            }

            if (bAdding && num == 1 && num1 <= 0)
            {
                listsSvs.CheckOutFile(str1, "true", targetItemXML.Attributes["ows_Modified"].Value);
                arrayLists.Add(targetItemXML.Attributes["ows__UIVersionString"].Value);
                string[] strArrays1 = new string[arrayLists.Count];
                arrayLists.CopyTo(strArrays1);
                return strArrays1;
            }

            listsSvs.CheckInFile(str1, "First version - to be deleted", "0");
            arrayLists.Add(targetItemXML.Attributes["ows__UIVersionString"].Value);
            for (int i = 0; i < num; i++)
            {
                listsSvs.CheckOutFile(str1, "true", targetItemXML.Attributes["ows_Modified"].Value);
                targetItemXML = this.ListItemQuery(sListID, listType, "ows_ID", "Counter", value, listsSvs);
                if (targetItemXML.Attributes["ows__UIVersionString"].Value != sVersionString)
                {
                    listsSvs.CheckInFile(str1, "Temporary Version - To be deleted", "1");
                    targetItemXML = this.ListItemQuery(sListID, listType, "ows_ID", "Counter", value, listsSvs);
                    arrayLists.Add(targetItemXML.Attributes["ows__UIVersionString"].Value);
                }
            }

            if (listXML.Attributes["EnableMinorVersion"] != null &&
                bool.Parse(listXML.Attributes["EnableMinorVersion"].Value))
            {
                str = targetItemXML.Attributes["ows__UIVersionString"].Value;
                numArray1 = this.ParseVersionString(str);
                num1 = numArray[1] - numArray1[1];
                for (int j = 0; j < num1; j++)
                {
                    listsSvs.CheckOutFile(str1, "true", null);
                    targetItemXML = this.ListItemQuery(sListID, listType, "ows_ID", "Counter", value, listsSvs);
                    if (targetItemXML.Attributes["ows__UIVersionString"].Value != sVersionString)
                    {
                        listsSvs.CheckInFile(str1, "Temporary Version - To be deleted", "0");
                        targetItemXML = this.ListItemQuery(sListID, listType, "ows_ID", "Counter", value, listsSvs);
                        arrayLists.Add(targetItemXML.Attributes["ows__UIVersionString"].Value);
                    }
                }
            }

            string[] strArrays2 = new string[arrayLists.Count];
            arrayLists.CopyTo(strArrays2);
            return strArrays2;
        }

        private static void InsertOrUpdatePagingInfo(XmlNode QueryOptions, string GetListItemCollectionPosition)
        {
            string str = "Paging";
            string str1 = "ListItemCollectionPositionNext";
            XmlNode xmlNodes = QueryOptions.SelectSingleNode(string.Concat("./", str));
            if (xmlNodes == null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, str, QueryOptions.NamespaceURI);
                xmlNodes = QueryOptions.OwnerDocument.ImportNode(xmlNodes, true);
                QueryOptions.AppendChild(xmlNodes);
            }

            (xmlNodes as XmlElement).SetAttribute(str1, GetListItemCollectionPosition);
        }

        public string IsListContainsInfoPathOrAspxItem(string listId)
        {
            throw new NotImplementedException();
        }

        private bool IsSubUrl(string subUrl, string greaterUrl)
        {
            char[] chrArray = new char[] { '/' };
            string[] strArrays = subUrl.Split(chrArray, StringSplitOptions.None);
            char[] chrArray1 = new char[] { '/' };
            string[] strArrays1 = greaterUrl.Split(chrArray1, StringSplitOptions.None);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                if (i >= (int)strArrays1.Length)
                {
                    return false;
                }

                if (strArrays[i] != strArrays1[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsViewWebPart(string sTypeName, XmlNode webPartXml)
        {
            XmlNode xmlNodes;
            bool flag;
            bool flag1;
            if (string.IsNullOrEmpty(sTypeName) ||
                !sTypeName.Equals("Microsoft.SharePoint.WebPartPages.ListViewWebPart",
                    StringComparison.OrdinalIgnoreCase) && !sTypeName.Equals(
                    "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            xmlNodes =
                (!sTypeName.Equals("Microsoft.SharePoint.WebPartPages.XsltListViewWebPart",
                    StringComparison.OrdinalIgnoreCase)
                    ? webPartXml.SelectSingleNode("*[local-name() = 'ListViewXml']")
                    : webPartXml.SelectSingleNode(".//*[local-name() = 'property'][@name='XmlDefinition']"));
            if (xmlNodes == null)
            {
                return false;
            }

            try
            {
                XmlNode xmlNode = XmlUtility.StringToXmlNode(xmlNodes.InnerText);
                XmlAttribute itemOf = xmlNode.Attributes["DisplayName"];
                if (itemOf == null || string.IsNullOrEmpty(itemOf.Value))
                {
                    return false;
                }

                XmlAttribute xmlAttribute = xmlNode.Attributes["Hidden"];
                if (xmlAttribute != null && bool.TryParse(xmlAttribute.Value, out flag) && flag)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                flag1 = false;
            }

            return flag1;
        }

        private static bool IsWebpageFile(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            return NWSAdapter.WebpageFileExtensions.Contains(extension.ToLower());
        }

        public static bool IsWritableColumn(string sFieldInternalName, bool bFieldReadOnly, string sFieldTypeAsString,
            int iListBaseTemplate, bool bItemIsFolder, bool bIsBDCField)
        {
            bool flag = Utils.IsWritableColumn(sFieldInternalName, bFieldReadOnly, sFieldTypeAsString,
                iListBaseTemplate, bItemIsFolder, bIsBDCField, false);
            if (flag)
            {
                flag = (sFieldTypeAsString == "TaxonomyFieldType"
                    ? false
                    : sFieldTypeAsString != "TaxonomyFieldTypeMulti");
            }

            return flag;
        }

        private XmlNode ListItemQuery(string sListID, ListType listType, string sField, string sType, string sValue,
            ListsService listsSvs)
        {
            string[] strArrays = new string[] { sField };
            string[] strArrays1 = new string[] { sType };
            string[] strArrays2 = new string[] { sValue };
            return this.ListItemQuery(sListID, listType, strArrays, strArrays1, strArrays2, listsSvs);
        }

        private XmlNode ListItemQuery(string sListID, ListType listType, string[] sFields, string[] sTypes,
            string[] sValues, ListsService listsSvs)
        {
            bool flag;
            XmlNode item;
            try
            {
                StringBuilder stringBuilder = new StringBuilder("<Where>");
                StringBuilder stringBuilder1 =
                    new StringBuilder("<FieldRef Name='ID'/><FieldRef Name='_UIVersionString'/>");
                int num = 0;
                string[] strArrays = sFields;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    stringBuilder1.Append(string.Concat("<FieldRef Name='", str, "'/>"));
                    if ((int)sFields.Length - num > 1)
                    {
                        stringBuilder.Append("<And>");
                    }

                    stringBuilder.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='{1}'>{2}</Value></Eq>",
                        (str.StartsWith("ows_") ? str.Substring(4) : str), sTypes[num], sValues[num]));
                    num++;
                }

                for (int j = 1; j < (int)sFields.Length; j++)
                {
                    stringBuilder.Append("</And>");
                }

                stringBuilder.Append("</Where>");
                stringBuilder.Append(
                    "<OrderBy><FieldRef Ascending = 'TRUE' Name = 'FileDirRef' /><FieldRef Ascending = 'TRUE' Name = 'FileLeafRef' /></OrderBy>");
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlNodes = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", "");
                xmlNodes.InnerXml = stringBuilder1.ToString();
                XmlNode xmlNodes1 = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                xmlNodes1.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc><Folder></Folder><MeetingInstanceID>-2</MeetingInstanceID><ViewAttributes Scope='RecursiveAll' />";
                XmlNode str1 = xmlDocument.CreateNode(XmlNodeType.Element, "Query", "");
                str1.InnerXml = stringBuilder.ToString();
                List<XmlNode> xmlNodes2 = this.RunListItemQuery(sListID, listType, str1, xmlNodes, xmlNodes1, listsSvs,
                    out flag, null);
                if (xmlNodes2.Count != 0)
                {
                    item = xmlNodes2[0];
                }
                else
                {
                    item = null;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Error querying list items: ", exception.Message));
            }

            return item;
        }

        private bool ListsContainsTemplates(ServerTemplate[] templates, XmlNodeList nodesLists)
        {
            ServerTemplate[] serverTemplateArray = templates;
            for (int i = 0; i < (int)serverTemplateArray.Length; i++)
            {
                string str = serverTemplateArray[i].ToString();
                bool flag = false;
                foreach (XmlNode nodesList in nodesLists)
                {
                    if (this.GetListServerTemplate(nodesList) != str)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    return flag;
                }
            }

            return true;
        }

        public static bool MatchWebPartPropertiesWithRegex(string sWebPartPage, DataTable webpartTable)
        {
            MatchCollection matchCollections = (new Regex(
                "(?<WebPart><WebPartPages:)|PartOrder\\s*=\\s*\"(?<PartOrder>\\d*)\"|(?<!KpiListWebPart\\sid=\"kpiListWebPartControl\"\\srunat=\"server\"\\sWebPart=\"true\"\\s)__WebPartId\\s*=\\s*\"{(?<WebPartStorageKey>[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12})}\"|ZoneID=\"(?<WebPartZone>\\w*)\"|(?<!Consumer|Provider)ID=\"(?<WebPartID>g_[0-9a-f]{8}_[0-9a-f]{4}_[0-9a-f]{4}_[0-9a-f]{4}_[0-9a-f]{12})\"|<ZoneID>(?<WebPartZone>\\w*)</ZoneID>\\s*<PartOrder>(?<PartOrder>\\d*)</PartOrder>|<ID>(?<WebPartID>g_[0-9a-f]{8}_[0-9a-f]{4}_[0-9a-f]{4}_[0-9a-f]{4}_[0-9a-f]{12})</ID>",
                RegexOptions.IgnoreCase)).Matches(sWebPartPage);
            int count = matchCollections.Count;
            if (count == 0)
            {
                return false;
            }

            webpartTable.Columns.Add("WebPartStorageKey");
            webpartTable.Columns.Add("PartOrder");
            webpartTable.Columns.Add("WebPartZone");
            webpartTable.Columns.Add("WebPartID");
            List<string> strs = new List<string>();
            DataRow dataRow = null;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(matchCollections[i].Groups["WebPart"].Value))
                {
                    if (dataRow == null)
                    {
                        dataRow = webpartTable.NewRow();
                    }

                    string value = matchCollections[i].Groups["WebPartStorageKey"].Value;
                    if (!string.IsNullOrEmpty(value) && !strs.Contains(value))
                    {
                        strs.Add(value);
                        dataRow["WebPartStorageKey"] = value;
                    }

                    string str = matchCollections[i].Groups["PartOrder"].Value;
                    if (!string.IsNullOrEmpty(str))
                    {
                        dataRow["PartOrder"] = str;
                    }

                    string value1 = matchCollections[i].Groups["WebPartZone"].Value;
                    if (!string.IsNullOrEmpty(value1))
                    {
                        dataRow["WebPartZone"] = value1;
                    }

                    string str1 = matchCollections[i].Groups["WebPartID"].Value;
                    if (!string.IsNullOrEmpty(str1))
                    {
                        dataRow["WebPartID"] = str1;
                    }

                    if (i == count - 1 && dataRow != null &&
                        !string.IsNullOrEmpty(dataRow["WebPartStorageKey"].ToString()))
                    {
                        webpartTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    if (dataRow != null && !string.IsNullOrEmpty(dataRow["WebPartStorageKey"].ToString()))
                    {
                        webpartTable.Rows.Add(dataRow);
                    }

                    dataRow = null;
                }
            }

            return true;
        }

        public string ModifyWebNavigationSettings(string sWebXML, ModifyNavigationOptions ModNavOptions)
        {
            if (string.IsNullOrEmpty(sWebXML))
            {
                return string.Empty;
            }

            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXML);
            string rpcString = this.ConvertWebNavigationToRpcString(this.Url, xmlNode);
            if (!string.IsNullOrEmpty(rpcString))
            {
                try
                {
                    string str = string.Concat("method=set+service+meta-info&service_name=&meta_info=[",
                        rpcString.Substring(0, rpcString.Length - 1), "]");
                    RPCUtil.SendRequest(this,
                        string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll "), str);
                }
                catch (SoapException soapException1)
                {
                    SoapException soapException = soapException1;
                    string soapError = RPCUtil.GetSoapError(soapException.Detail);
                    throw new Exception(
                        string.Concat("Could not change navigation settings for web '", this.ServerRelativeUrl,
                            "'. Soap Exception: ", soapError), soapException);
                }
            }

            return string.Empty;
        }

        private string ParentSiteTemplateCollection(string sTemplate)
        {
            string webTemplates = this.GetWebTemplates();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(webTemplates);
            string value = "";
            foreach (XmlNode documentElement in xmlDocument.DocumentElement)
            {
                if (documentElement.Attributes["Name"].Value != sTemplate)
                {
                    continue;
                }

                value = documentElement.Attributes["Title"].Value;
                break;
            }

            return value;
        }

        private void ParseCreatedModified(XmlAttributeCollection listAttribs, out string sCreated, out string sModified)
        {
            string innerText = listAttribs["Created"].InnerText;
            DateTime dateTime = DateTime.ParseExact(innerText, "yyyyMMdd HH:mm:ss",
                new System.Globalization.CultureInfo("", false), DateTimeStyles.AssumeUniversal);
            sCreated = Utils.FormatDate(dateTime.ToUniversalTime());
            string str = listAttribs["Modified"].InnerText;
            DateTime dateTime1 = DateTime.ParseExact(str, "yyyyMMdd HH:mm:ss",
                new System.Globalization.CultureInfo("", false), DateTimeStyles.AssumeUniversal);
            sModified = Utils.FormatDate(dateTime1.ToUniversalTime());
        }

        private int[] ParseVersionString(string sVersionString)
        {
            int[] numArray = new int[2];
            int num = sVersionString.IndexOf('.');
            numArray[0] = int.Parse((num >= 0 ? sVersionString.Substring(0, num) : sVersionString));
            numArray[1] = (num >= 0 ? int.Parse(sVersionString.Substring(num + 1)) : 0);
            return numArray;
        }

        private void PostProcessWebPartXml(IEnumerable webPartNodes)
        {
            string str;
            Dictionary<string, string> listIDTitleMap = null;
            foreach (XmlNode webPartNode in webPartNodes)
            {
                if (webPartNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                XmlElement versionNumberString = webPartNode.OwnerDocument.CreateElement("SharePointSourceVersion");
                versionNumberString.InnerText = base.SharePointVersion.VersionNumberString;
                webPartNode.AppendChild(versionNumberString);
                XmlNode xmlNodes = webPartNode.SelectSingleNode(".//*[local-name() = 'ListName']") ??
                                   webPartNode.SelectSingleNode(".//*[local-name() = 'property'][@name='ListName']");
                if (xmlNodes == null)
                {
                    continue;
                }

                if (listIDTitleMap == null)
                {
                    listIDTitleMap = this.GetListIDTitleMap(true);
                }

                string innerText = xmlNodes.InnerText;
                char[] chrArray = new char[] { '{', '}' };
                string upperInvariant = innerText.Trim(chrArray).ToUpperInvariant();
                if (string.IsNullOrEmpty(upperInvariant) || !Utils.IsGUID(upperInvariant))
                {
                    continue;
                }

                if (!listIDTitleMap.TryGetValue(upperInvariant, out str))
                {
                    throw new Exception(string.Concat("Failed to write web part XML: Could not locate list in web. (",
                        upperInvariant, ")"));
                }

                xmlNodes.InnerText = str;
            }
        }

        private void PrepareForItemFetch(string webUrl, string listID, string configurationXml,
            out ListsService listsSvs, out XmlNode nodeList)
        {
            listsSvs = this.GetWebServiceForLists(webUrl);
            if (string.IsNullOrEmpty(configurationXml))
            {
                nodeList = listsSvs.GetList(listID);
                return;
            }

            XmlNode xmlNode = XmlUtility.StringToXmlNode(configurationXml);
            nodeList = xmlNode.SelectSingleNode(string.Format("//{0}", XmlElementNames.List.ToString()));
        }

        public void PublishInfoPathForm(string formTemplateFullLocation)
        {
            (new FormsService(this)).BrowserEnableUserFormTemplate(formTemplateFullLocation);
        }

        private string[] ReadNavNodeChidren(string[] lines, ref int iCurrentIdx)
        {
            List<string> strs = new List<string>();
            while (iCurrentIdx < (int)lines.Length && !lines[iCurrentIdx].Contains("</ul>"))
            {
                string str = lines[iCurrentIdx].Trim();
                if (str.StartsWith("<li>"))
                {
                    strs.Add(this.GetStringAfterCharacter(str, '>'));
                }

                iCurrentIdx++;
            }

            return strs.ToArray();
        }

        // Metalogix.SharePoint.Adapters.NWS.NWSAdapter
        private void ReadNavNodeDataFromArray(int iNodeIdx, string[] lines, string[] globalNavHiddenUrls,
            string[] currentNavHiddenUrls, bool bOnGlobalNav, bool bOnCurrentNav, out string sEid, out string sTitle,
            out string sUrl, out string sLastModified, out bool bIsVisible, out bool bIsExternal,
            out Dictionary<string, string> properties, out string[] childEids)
        {
            sEid = null;
            sTitle = null;
            sUrl = null;
            bIsVisible = true;
            bIsExternal = false;
            sLastModified = null;
            properties = null;
            childEids = null;
            int num = iNodeIdx;
            while (num < lines.Length && !lines[num].Contains("</ul>"))
            {
                string text = lines[num].Trim();
                if (text.StartsWith("<li>eid="))
                {
                    sEid = this.GetNavNodePropertyValue(text);
                }
                else if (text.StartsWith("<li>eidChildren="))
                {
                    num++;
                    childEids = this.ReadNavNodeChidren(lines, ref num);
                }
                else if (text.StartsWith("<li>DTLP="))
                {
                    string navNodePropertyValue = this.GetNavNodePropertyValue(text);
                    if (!string.IsNullOrEmpty(navNodePropertyValue))
                    {
                        DateTime dt = DateTime.ParseExact(navNodePropertyValue, "dd MMM yyyy HH:mm:ss zzz", null);
                        sLastModified = Utils.FormatDate(dt);
                    }
                    else
                    {
                        sLastModified = Utils.FormatDate(DateTime.UtcNow);
                    }
                }
                else if (text.StartsWith("<li>eType="))
                {
                    string navNodePropertyValue2 = this.GetNavNodePropertyValue(text);
                    bIsExternal = (navNodePropertyValue2.ToLower() == "link");
                }
                else if (text.StartsWith("<li>url="))
                {
                    sUrl = this.GetNavNodePropertyValue(text);
                }
                else if (text.StartsWith("<li>name="))
                {
                    sTitle = this.GetNavNodePropertyValue(text);
                }
                else if (text.StartsWith("<li>meta-info="))
                {
                    num++;
                    properties = this.ReadNavNodeProperties(lines, ref num);
                }

                num++;
            }

            if (!bIsExternal && (bOnCurrentNav || bOnGlobalNav))
            {
                bIsVisible = !(bOnGlobalNav
                    ? this.GetNavNodeIsHidden(sUrl, globalNavHiddenUrls)
                    : this.GetNavNodeIsHidden(sUrl, currentNavHiddenUrls));
            }
        }

        private Dictionary<string, string> ReadNavNodeProperties(string[] lines, ref int iCurrentIdx)
        {
            Dictionary<string, string> strs = new Dictionary<string, string>();
            while (iCurrentIdx < (int)lines.Length && !lines[iCurrentIdx].Contains("</ul>"))
            {
                string str = lines[iCurrentIdx].Trim();
                if (str.StartsWith("<li>"))
                {
                    string stringAfterCharacter = this.GetStringAfterCharacter(str, '>');
                    iCurrentIdx++;
                    string str1 = HttpUtility.HtmlDecode(this.GetStringAfterCharacter(lines[iCurrentIdx].Trim(), '>'));
                    if (stringAfterCharacter != "LastModifiedDate")
                    {
                        strs.Add(stringAfterCharacter, str1);
                    }
                }

                iCurrentIdx++;
            }

            return strs;
        }

        private string ReCombineEventUID(XmlNode meetingInstance, Dictionary<string, string> ListTitleToIDMap)
        {
            XmlAttribute itemOf = meetingInstance.Attributes["EventUIDPrefix"];
            XmlAttribute xmlAttribute = meetingInstance.Attributes["EventUIDListName"];
            XmlAttribute itemOf1 = meetingInstance.Attributes["EventUIDItemID"];
            if (itemOf == null || xmlAttribute == null || itemOf1 == null)
            {
                return null;
            }

            string str = (ListTitleToIDMap.ContainsKey(xmlAttribute.Value)
                ? ListTitleToIDMap[xmlAttribute.Value]
                : xmlAttribute.Value);
            if (!str.StartsWith("{") && !str.EndsWith("}"))
            {
                str = string.Concat("{", str, "}");
            }

            string[] value = new string[] { itemOf.Value, ":List:", str, ":Item:", itemOf1.Value };
            string str1 = string.Concat(value);
            XmlAttribute xmlAttribute1 = meetingInstance.OwnerDocument.CreateAttribute("EventUID");
            xmlAttribute1.Value = str1;
            meetingInstance.Attributes.Remove(itemOf);
            meetingInstance.Attributes.Remove(xmlAttribute);
            meetingInstance.Attributes.Remove(itemOf1);
            meetingInstance.Attributes.Append(xmlAttribute1);
            return str1;
        }

        public override void Refresh()
        {
            this.m_ContentTypeDict = null;
            this.m_timeZone = null;
            this.BuildUserMap();
            this.BuildGroupMap();
        }

        private void ReMapManagedMetadataFields(XmlNodeList fields, ref XmlNode listItemXml)
        {
            Dictionary<Guid, XmlNode> guids = new Dictionary<Guid, XmlNode>(fields.Count);
            Dictionary<Guid, string> guids1 = new Dictionary<Guid, string>();
            foreach (XmlNode field in fields)
            {
                try
                {
                    if (field.Attributes["ID"] != null)
                    {
                        Guid guid = new Guid(field.Attributes["ID"].Value);
                        if (!guids.ContainsKey(guid))
                        {
                            guids.Add(guid, field);
                        }

                        if (field.Attributes["Type"] != null)
                        {
                            string value = field.Attributes["Type"].Value;
                            if (!(value != "TaxonomyFieldType") || !(value != "TaxonomyFieldTypeMulti"))
                            {
                                if (field.Attributes["Name"] != null)
                                {
                                    string str = field.Attributes["Name"].Value;
                                    XmlAttribute itemOf = listItemXml.Attributes[str];
                                    if (itemOf != null)
                                    {
                                        if (this.m_remapperNametable == null)
                                        {
                                            this.m_remapperNametable =
                                                new XmlNamespaceManager(field.OwnerDocument.NameTable);
                                        }

                                        if (!this.m_remapperNametable.HasNamespace("ns"))
                                        {
                                            this.m_remapperNametable.AddNamespace("ns",
                                                "http://schemas.microsoft.com/sharepoint/soap/");
                                        }

                                        XmlNode xmlNodes = field.SelectSingleNode(
                                            "./ns:Customization/ns:ArrayOfProperty/ns:Property/ns:Name[text()='TextField']/../ns:Value",
                                            this.m_remapperNametable);
                                        if (xmlNodes != null)
                                        {
                                            Guid guid1 = new Guid(xmlNodes.InnerText);
                                            if (!guids1.ContainsKey(guid1))
                                            {
                                                guids1.Add(guid1, itemOf.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }

            if (guids1.Count == 0)
            {
                return;
            }

            foreach (Guid key in guids1.Keys)
            {
                if (!guids.ContainsKey(key))
                {
                    continue;
                }

                XmlNode item = guids[key];
                if (item == null || item.Attributes["Name"] == null)
                {
                    continue;
                }

                string value1 = item.Attributes["Name"].Value;
                if (value1 == null || listItemXml.Attributes[value1] != null)
                {
                    continue;
                }

                string item1 = guids1[key];
                if (item1 == null)
                {
                    continue;
                }

                XmlAttribute xmlAttribute = listItemXml.OwnerDocument.CreateAttribute(value1);
                xmlAttribute.Value = item1;
                listItemXml.Attributes.Append(xmlAttribute);
            }
        }

        private void RemoveFields(List<XmlNode> FieldsToAddToContentType, XmlNodeList listFields,
            List<string> FieldNames)
        {
            List<XmlNode> xmlNodes = new List<XmlNode>();
            foreach (XmlNode fieldsToAddToContentType in FieldsToAddToContentType)
            {
                string value = fieldsToAddToContentType.Attributes["Name"].Value;
                bool flag = false;
                foreach (XmlNode listField in listFields)
                {
                    if (listField.Attributes["Name"] == null || listField.Attributes["ID"] == null ||
                        !(listField.Attributes["Name"].Value == value))
                    {
                        continue;
                    }

                    flag = true;
                    XmlAttribute itemOf = fieldsToAddToContentType.Attributes["ID"];
                    if (itemOf == null)
                    {
                        itemOf = fieldsToAddToContentType.OwnerDocument.CreateAttribute("ID");
                        fieldsToAddToContentType.Attributes.Append(itemOf);
                    }

                    itemOf.Value = listField.Attributes["ID"].Value;
                    break;
                }

                if (flag || fieldsToAddToContentType.Attributes["Name"] == null ||
                    !FieldNames.Contains(fieldsToAddToContentType.Attributes["Name"].Value))
                {
                    continue;
                }

                xmlNodes.Add(fieldsToAddToContentType);
            }

            foreach (XmlNode xmlNode in xmlNodes)
            {
                FieldsToAddToContentType.Remove(xmlNode);
            }
        }

        private string RemoveIDPropertyFromWebPartXml(string sWebPartXml)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebPartXml);
            XmlNodeList xmlNodeLists = xmlNode.SelectNodes("./*[local-name() = 'ID']");
            if (xmlNodeLists != null)
            {
                foreach (XmlNode xmlNodes in xmlNodeLists)
                {
                    xmlNode.RemoveChild(xmlNodes);
                }
            }

            return xmlNode.OuterXml;
        }

        public string ReorderContentTypes(string sListId, string[] sContentTypes)
        {
            throw new ArgumentException("Ordering of content types is not available using an NWS connection");
        }

        public bool ResolvePrincipals(string principal)
        {
            PeopleService webServiceForPeople = this.GetWebServiceForPeople();
            string[] strArrays = new string[] { principal };
            PrincipalInfo[] principalInfoArray = webServiceForPeople.ResolvePrincipals(strArrays,
                SPPrincipalType.User | SPPrincipalType.SecurityGroup, false);
            if (principalInfoArray != null && (int)principalInfoArray.Length > 0 && principalInfoArray[0].IsResolved)
            {
                return true;
            }

            return false;
        }

        public Hashtable RPCProperties(string sUrl)
        {
            Hashtable hashtables = new Hashtable();
            string str = "method=open+service";
            string str1 = RPCUtil.SendRequest(this, Utils.JoinUrl(sUrl, "_vti_bin/_vti_aut/author.dll"), str);
            char[] chrArray = new char[] { '\r', '\n' };
            string[] strArrays = str1.Split(chrArray);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str2 = strArrays[i];
                if ((str2.StartsWith("<li>vti_") ? true : str2.StartsWith("<li>__")) & i < (int)strArrays.Length - 1)
                {
                    string str3 = str2.Substring((str2.StartsWith("<li>vti_") ? 8 : 6));
                    string str4 = strArrays[i + 1].Substring(7);
                    if (!hashtables.ContainsKey(str3))
                    {
                        hashtables.Add(str3, str4);
                        i++;
                    }
                }
            }

            return hashtables;
        }

        private List<XmlNode> RunListItemQuery(string sListID, ListType listType, XmlNode nodeQuery,
            XmlNode nodeViewFields, XmlNode nodeQueryOptions, ListsService listsSvs, out bool bSortingOrderViolated,
            string viewID = null)
        {
            List<XmlNode> xmlNodes = new List<XmlNode>();
            List<XmlNode> xmlNodes1 = new List<XmlNode>();
            XmlNode[] listItems = this.GetListItems(listsSvs, sListID, viewID, nodeQuery, nodeViewFields,
                nodeQueryOptions, null, out bSortingOrderViolated);
            XmlNode[] xmlNodeArrays = listItems;
            for (int i = 0; i < (int)xmlNodeArrays.Length; i++)
            {
                foreach (XmlNode xmlNodes2 in XmlUtility.RunXPathQuery(xmlNodeArrays[i], "//z:row"))
                {
                    if (base.SharePointVersion.IsSharePoint2003 && listType == ListType.DocumentLibrary &&
                        AdapterConfigurationVariables.Swap2003DocMetaInfoColumns)
                    {
                        this.SwapDocumentAndItemMetaInfoColumns(xmlNodes2);
                    }

                    xmlNodes.Add(xmlNodes2);
                }
            }

            return xmlNodes;
        }

        public string SearchForDocument(string sSearchTerm, string sOptionsXml)
        {
            throw new NotImplementedException(
                "Search analysis functionality is not available with this type of connection");
        }

        public void SetContentTypeXmlDocument(string xmlDocument, string contentTypeId, string listTitle)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(xmlDocument);
            webServiceForLists.UpdateContentTypeXmlDocument(listTitle, contentTypeId, xmlDocument1.DocumentElement);
        }

        private void SetDocumentLibraryTemplate(string sListID, string sListRootFolder, string sDocTemplateUrl,
            string sListVersion, string sSourceTemplateUrl, byte[] templateFile)
        {
            if (string.IsNullOrEmpty(sListID) || string.IsNullOrEmpty(sListRootFolder) ||
                string.IsNullOrEmpty(sListVersion) || string.IsNullOrEmpty(sSourceTemplateUrl) || templateFile == null)
            {
                return;
            }

            try
            {
                string fileNameFromPath = Utils.GetFileNameFromPath(sSourceTemplateUrl, true);
                string str = null;
                string str1 = null;
                str1 = (!string.IsNullOrEmpty(sDocTemplateUrl)
                    ? sDocTemplateUrl.Remove(sDocTemplateUrl.LastIndexOf('/'))
                    : Utils.CombineUrls(sListRootFolder, "Forms"));
                str = Utils.CombineUrls(str1, fileNameFromPath);
                char[] chrArray = new char[] { '/' };
                str = string.Concat("/", str.TrimStart(chrArray));
                str = HttpUtility.UrlDecode(StandardizedUrl.StandardizeUrl(this, str).WebRelative);
                string str2 = RPCUtil.FormatFrontPageRPCMessage(str, "", true, false,
                    base.SharePointVersion.VersionNumberString);
                RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str2,
                    templateFile);
                Dictionary<string, string> strs = new Dictionary<string, string>()
                {
                    { "NewListTemplate", str }
                };
                string str3 = this.BuildBatchListUpdateCommand(sListID, sListVersion, strs);
                Encoding uTF8Encoding = new UTF8Encoding();
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(uTF8Encoding.GetBytes(str3), 0, str3.Length);
                RPCUtil.SendRequest(this,
                    Utils.CombineUrls(string.Concat(this.Server, this.ServerRelativeUrl),
                        "_vti_bin/owssvr.dll?Cmd=DisplayPost"), memoryStream, "application/xml", false, false);
            }
            catch (Exception exception)
            {
                string.Concat("Could not set the document template for library: ", exception.Message);
            }
        }

        public string SetDocumentParsing(bool bParserEnabled)
        {
            return string.Empty;
        }

        public string SetMasterPage(string sWebXML)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXML);
            string str = "";
            string server = this.Server;
            string value = xmlNode.Attributes["CustomMasterPage"].Value;
            char[] chrArray = new char[] { '/' };
            string str1 = string.Concat(server, "/_layouts/download.aspx?SourceUrl=", value.TrimStart(chrArray));
            string str2 = null;
            str2 = this.DownloadURLToString(str1);
            if (xmlNode.Attributes["CustomMasterPage"] != null && !str2.Contains("error.aspx"))
            {
                str = string.Concat(str, "vti_custommasterurl;SX|", xmlNode.Attributes["CustomMasterPage"].Value, ";");
            }

            string server1 = this.Server;
            string value1 = xmlNode.Attributes["MasterPage"].Value;
            char[] chrArray1 = new char[] { '/' };
            str1 = string.Concat(server1, "/_layouts/download.aspx?SourceUrl=", value1.TrimStart(chrArray1));
            str2 = this.DownloadURLToString(str1);
            if (xmlNode.Attributes["MasterPage"] != null && !str2.Contains("error.aspx"))
            {
                str = string.Concat(str, "vti_masterurl;SX|", xmlNode.Attributes["MasterPage"].Value, ";");
            }

            str = string.Concat(str, "__InheritsMasterUrl;SW|False;");
            if (!string.IsNullOrEmpty(str))
            {
                string str3 = string.Concat("method=set+service+meta-info&service_name=&meta_info=[",
                    str.Substring(0, str.Length - 1), "]");
                RPCUtil.SendRequest(this,
                    string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/_vti_aut/author.dll "), str3);
            }

            return string.Empty;
        }

        private bool SetMinID(int iMinID, XmlNode ndQuery)
        {
            bool flag;
            try
            {
                if (ndQuery != null)
                {
                    string innerXml = ndQuery.InnerXml;
                    string str = "<Gt><FieldRef Name=\"ID\" /><Value Type=\"Counter\">";
                    int length = innerXml.IndexOf(str);
                    if (length >= 0)
                    {
                        length += str.Length;
                        int num = innerXml.IndexOf("</Value></Gt>", length);
                        if (num >= 0)
                        {
                            innerXml = string.Concat(innerXml.Substring(0, length), iMinID.ToString(),
                                innerXml.Substring(num));
                            ndQuery.InnerXml = innerXml;
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception exception)
            {
                flag = false;
            }

            return flag;
        }

        private void SetQuickLaunch(string sGUID, bool bOn)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            xmlTextWriter.WriteRaw("Cmd=DisplayPost&PostBody=\n");
            xmlTextWriter.WriteRaw("<?xml version='1.0' encoding='UTF-8'?>");
            xmlTextWriter.WriteStartElement("Batch");
            xmlTextWriter.WriteAttributeString("OnError", "Continue");
            xmlTextWriter.WriteAttributeString("Version", "6.0.2.6551");
            xmlTextWriter.WriteStartElement("Method");
            xmlTextWriter.WriteAttributeString("ID", "0,MODLISTSETTINGS");
            xmlTextWriter.WriteStartElement("SetList");
            xmlTextWriter.WriteAttributeString("Scope", "Request");
            xmlTextWriter.WriteString(sGUID);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("SetVar");
            xmlTextWriter.WriteAttributeString("Name", "Cmd");
            xmlTextWriter.WriteString("MODLISTSETTINGS");
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("SetVar");
            xmlTextWriter.WriteAttributeString("Name", "displayOnLeft");
            xmlTextWriter.WriteString(bOn.ToString());
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            string str = stringBuilder.ToString();
            RPCUtil.SendRequest(this, string.Concat(this.Server, this.ServerRelativeUrl, "/_vti_bin/owssvr.dll"), str);
        }

        private void SetUrl(string sUrl)
        {
            string lower = sUrl.ToLower();
            string serverPart = NWSAdapter.GetServerPart(this.m_sUrl);
            if (UrlUtils.GetType(lower) != UrlType.Full)
            {
                string[] strArrays = new string[] { serverPart, sUrl };
                this.m_sUrl = UrlUtils.ConcatUrls(strArrays);
                this.m_sServerRelativeUrl = UrlUtils.EnsureLeadingSlash(sUrl);
                return;
            }

            this.m_sUrl = (NWSAdapter.IsWebpageFile(sUrl) ? NWSAdapter.StripFileNameFromUrl(sUrl) : lower);
            serverPart = NWSAdapter.GetServerPart(this.m_sUrl);
            this.m_sServerRelativeUrl = UrlUtils.EnsureLeadingSlash(sUrl.Substring(serverPart.Length));
        }

        public string SetUserProfile(string sSiteURL, string sLoginName, string sPropertyXml, bool bCreateIfNotFound)
        {
            return null;
        }

        public string SetWelcomePage(string WelcomePage)
        {
            return string.Empty;
        }

        private void SortFieldsWithInternalNameDependancy(XmlNodeList fieldsList, out XmlNodeList sortedFields,
            out List<string> fieldPlaceHoldersList)
        {
            int num;
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Fields");
            StringWriter stringWriter1 = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter1 = new XmlTextWriter(stringWriter1);
            xmlTextWriter1.WriteStartElement("Fields");
            List<string> strs = new List<string>();
            fieldPlaceHoldersList = new List<string>();
            for (int i = 0; i < fieldsList.Count; i++)
            {
                XmlNode itemOf = fieldsList[i];
                string value = itemOf.Attributes["Name"].Value;
                strs.Add(value.ToLower());
                if (value.Length <= 32)
                {
                    xmlTextWriter.WriteRaw(itemOf.OuterXml);
                }
                else if (Utils.IsGUID(value))
                {
                    xmlTextWriter.WriteRaw(itemOf.OuterXml);
                }
                else
                {
                    xmlTextWriter1.WriteRaw(itemOf.OuterXml);
                }
            }

            xmlTextWriter1.WriteEndElement();
            xmlTextWriter1.Flush();
            xmlTextWriter1.Close();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(stringWriter1.ToString());
            StringReader stringReader = new StringReader(xmlNode.OuterXml);
            XPathNavigator xPathNavigator = (new XPathDocument(stringReader)).CreateNavigator();
            XPathExpression xPathExpression = xPathNavigator.Compile("//Field");
            xPathExpression.AddSort("@Name", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
            foreach (XPathNavigator xPathNavigator1 in xPathNavigator.Select(xPathExpression))
            {
                XmlNode xmlNodes = XmlUtility.StringToXmlNode(xPathNavigator1.OuterXml);
                string str = xmlNodes.Attributes["Name"].Value;
                if (int.TryParse(str.Substring(32), out num))
                {
                    string str1 = str.Substring(0, 32);
                    string str2 = str1;
                    string str3 = string.Concat(str1, "MLPlaceHolder");
                    if (!strs.Contains(str2.ToLower()))
                    {
                        strs.Add(str2.ToLower());
                        str3 = string.Concat(str1, "MLPlaceHolder");
                        xmlNodes.Attributes["Name"].Value = str3;
                        xmlNodes.Attributes["DisplayName"].Value = str3;
                        xmlNodes.Attributes["StaticName"].Value = str3;
                        xmlNodes.Attributes.RemoveNamedItem("ID");
                        fieldPlaceHoldersList.Add(str3);
                        xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                    }

                    for (int j = 0; j < num; j++)
                    {
                        str2 = string.Concat(str1, j.ToString());
                        if (!strs.Contains(str2.ToLower()))
                        {
                            strs.Add(str2.ToLower());
                            str3 = string.Concat(str1, j.ToString(), "MLPlaceHolder");
                            xmlNodes.Attributes["Name"].Value = str3;
                            xmlNodes.Attributes["DisplayName"].Value = str3;
                            xmlNodes.Attributes["StaticName"].Value = str3;
                            xmlNodes.Attributes.RemoveNamedItem("ID");
                            fieldPlaceHoldersList.Add(str3);
                            xmlTextWriter.WriteRaw(xmlNodes.OuterXml);
                        }
                    }
                }

                xmlTextWriter.WriteRaw(xPathNavigator1.OuterXml);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            XmlNode xmlNode1 = XmlUtility.StringToXmlNode(stringWriter.ToString());
            sortedFields = xmlNode1.ChildNodes;
        }

        public string StoragePointAvailable(string inputXml)
        {
            return bool.FalseString;
        }

        public string StoragePointProfileConfigured(string sSharePointPath)
        {
            throw new NotSupportedException("This method is not supported on NWS connections");
        }

        public static string StripFileNameFromUrl(string sUrl)
        {
            string str;
            try
            {
                if (sUrl != null)
                {
                    Uri uri = (new UriBuilder(sUrl)).Uri;
                    string segments = uri.Segments[(int)uri.Segments.Length - 1];
                    str = (segments.IndexOf(".") <= 0 ? sUrl : sUrl.Substring(0, sUrl.Length - segments.Length));
                }
                else
                {
                    str = null;
                }
            }
            catch
            {
                str = sUrl;
            }

            return str;
        }

        private void SwapDocumentAndItemMetaInfoColumns(XmlNode itemXml)
        {
            XmlAttribute itemOf = itemXml.Attributes["ows_Created"];
            XmlAttribute xmlAttribute = itemXml.Attributes["ows_Modified"];
            XmlAttribute itemOf1 = itemXml.Attributes["ows_Created_x0020_Date"];
            XmlAttribute xmlAttribute1 = itemXml.Attributes["ows_Last_x0020_Modified"];
            if (itemOf == null || xmlAttribute == null || itemOf1 == null || xmlAttribute1 == null)
            {
                return;
            }

            string str = "";
            string value = itemOf1.Value;
            string[] strArrays = new string[] { ";#" };
            string[] strArrays1 = value.Split(strArrays, 2, StringSplitOptions.None);
            if ((int)strArrays1.Length == 2)
            {
                str = string.Concat(strArrays1[0], ";#");
                value = strArrays1[1];
            }

            itemOf1.Value = string.Concat(str, itemOf.Value);
            itemOf.Value = value;
            str = "";
            value = xmlAttribute1.Value;
            string[] strArrays2 = new string[] { ";#" };
            strArrays1 = value.Split(strArrays2, 2, StringSplitOptions.None);
            if ((int)strArrays1.Length == 2)
            {
                str = string.Concat(strArrays1[0], ";#");
                value = strArrays1[1];
            }

            xmlAttribute1.Value = string.Concat(str, xmlAttribute.Value);
            xmlAttribute.Value = value;
        }

        private void SwapMetaInfoColumnsInViews(XmlNode viewDetailsXml)
        {
            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(viewDetailsXml.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/soap/");
            if (viewDetailsXml.SelectSingleNode(".//sp:ViewFields", xmlNamespaceManagers) != null)
            {
                XmlNode xmlNodes = viewDetailsXml.SelectSingleNode(".//sp:FieldRef[@Name='Created_x0020_Date']",
                    xmlNamespaceManagers);
                if (xmlNodes != null)
                {
                    xmlNodes.Attributes["Name"].Value = "Created";
                }

                XmlNode xmlNodes1 = viewDetailsXml.SelectSingleNode(".//sp:FieldRef[@Name='Last_x0020_Modified']",
                    xmlNamespaceManagers);
                if (xmlNodes1 != null)
                {
                    xmlNodes1.Attributes["Name"].Value = "Modified";
                }
            }
        }

        public static Type TypeFromString(string sType)
        {
            string upper = sType.ToUpper();
            string str = upper;
            if (upper != null)
            {
                switch (str)
                {
                    case "TEXT":
                    {
                        return typeof(string);
                    }
                    case "NOTE":
                    {
                        return typeof(string);
                    }
                    case "DATETIME":
                    case "PUBLISHINGSCHEDULESTARTDATEFIELDTYPE":
                    case "PUBLISHINGSCHEDULEENDDATEFIELDTYPE":
                    {
                        return typeof(DateTime);
                    }
                    case "FILE":
                    {
                        return typeof(string);
                    }
                    case "NUMBER":
                    {
                        return typeof(float);
                    }
                    case "INTEGER":
                    {
                        return typeof(int);
                    }
                    case "URL":
                    {
                        return typeof(string);
                    }
                    case "ATTACHMENTS":
                    {
                        return typeof(string);
                    }
                    case "CHOICE":
                    {
                        return typeof(string);
                    }
                    case "LOOKUP":
                    {
                        return typeof(string);
                    }
                    case "COMPUTED":
                    {
                        return typeof(string);
                    }
                    case "IMAGE":
                    {
                        return typeof(string);
                    }
                    case "CURRENCY":
                    {
                        return typeof(float);
                    }
                }
            }

            return typeof(string);
        }

        private void UpdateColumnSettings(XmlNode fieldRef, XmlNode siteColumn)
        {
            if (fieldRef.Attributes["Hidden"] != null)
            {
                if (siteColumn.Attributes["Hidden"] == null)
                {
                    siteColumn.Attributes.Append(siteColumn.OwnerDocument.CreateAttribute("Hidden"));
                }

                siteColumn.Attributes["Hidden"].Value = fieldRef.Attributes["Hidden"].Value.ToUpper();
            }
        }

        private void UpdateContentTypeID(XmlNode listItemXml)
        {
            if (listItemXml.Attributes["ContentType"] != null)
            {
                string value = listItemXml.Attributes["ContentType"].Value;
                if (this.ContentTypeDictionary.ContainsKey(value))
                {
                    string item = this.ContentTypeDictionary[value];
                    if (listItemXml.Attributes["ContentTypeId"] == null)
                    {
                        XmlAttribute xmlAttribute = listItemXml.OwnerDocument.CreateAttribute("ContentTypeId");
                        xmlAttribute.Value = item;
                        listItemXml.Attributes.Append(xmlAttribute);
                        return;
                    }

                    listItemXml.Attributes["ContentTypeId"].Value = item;
                }
            }
        }

        public string UpdateDocument(string sListID, string sParentFolder, string sFileLeafRef, string sListItemXML,
            byte[] fileContents, UpdateDocumentOptions updateOptions)
        {
            string str;
            string value;
            string str1 = null;
            string str2 = null;
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNode list = webServiceForLists.GetList(sListID);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sListItemXML);
            string str3 = (xmlNode.Attributes["ows_FileLeafRef"] != null ? "ows_" : "");
            string rootFolder = this.GetRootFolder(list.Attributes);
            int num = rootFolder.LastIndexOf('/');
            string str4 = rootFolder.Substring(num + 1);
            bool flag = (list.Attributes["EnableVersioning"] != null
                ? bool.Parse(list.Attributes["EnableVersioning"].Value)
                : false);
            string str5 =
                ((xmlNode.Attributes[string.Concat(str3, "_VersionLevel")] == null
                    ? "1"
                    : xmlNode.Attributes[string.Concat(str3, "_VersionLevel")].Value) == "2"
                    ? "0"
                    : "1");
            if (!flag)
            {
                str = "0.1";
            }
            else
            {
                str = (xmlNode.Attributes[string.Concat(str3, "_VersionString")] == null
                    ? "1"
                    : xmlNode.Attributes[string.Concat(str3, "_VersionString")].Value);
            }

            this.UpdateContentTypeID(xmlNode);
            string str6 = string.Concat(str4, sParentFolder, "/", sFileLeafRef);
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(list, "//sp:Fields/sp:Field");
            string str7 = string.Concat(this.Server, "/", str6);
            webServiceForLists.CheckOutFile(str7, "true", null);
            string str8 = this.BuildItemMetadataRPC(xmlNodeLists, xmlNode, str3);
            if (xmlNode.Attributes["_CheckinComment"] != null)
            {
                value = xmlNode.Attributes["_CheckinComment"].Value;
            }
            else
            {
                value = (xmlNode.Attributes["_CheckinComment"] != null
                    ? xmlNode.Attributes["_CheckinComment"].Value
                    : "");
            }

            string str9 = value;
            string str10 =
                RPCUtil.FormatFrontPageRPCMessage(str6, str8, true, false, base.SharePointVersion.VersionNumberString);
            RPCUtil.UploadDocumentUsingFrontPageRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), str10,
                fileContents);
            RPCUtil.SetMetaInfoRPC(this, string.Concat(this.Server, this.ServerRelativeUrl), sFileLeafRef,
                string.Concat(str4, sParentFolder), str8);
            this.CheckInFinalVersion(list, str, str9, str7, str6, str5, true, false, webServiceForLists);
            if (list.Attributes["EnableModeration"] != null && bool.Parse(list.Attributes["EnableModeration"].Value) &&
                xmlNode.Attributes["_ModerationStatus"] != null && xmlNode.Attributes["_ModerationStatus"].Value != "3")
            {
                XmlNode xmlNodes =
                    webServiceForLists.UpdateListItems(sListID, this.BuildUpdateModerationCommand(list, xmlNode, str1));
                if (xmlNodes.InnerText != "0x00000000")
                {
                    throw new Exception(string.Concat("Add failed! ", xmlNodes.InnerText));
                }
            }

            return this.GetListItems(sListID, str1, str2, null, false, ListItemQueryType.ListItem, null,
                new GetListItemOptions());
        }

        private void UpdateFieldLookupListNames(XmlNode nodeWantedField, XmlNode nodeLists)
        {
            XmlAttribute itemOf = nodeWantedField.Attributes["TargetListName"];
            if (itemOf != null)
            {
                XmlAttribute xmlAttribute = nodeWantedField.Attributes["TargetWebName"];
                XmlAttribute webID = nodeWantedField.Attributes["WebId"];
                XmlAttribute itemOf1 = nodeWantedField.Attributes["TargetWebSRURL"];
                string value = itemOf.Value;
                XmlNode xmlNodes = XmlUtility.MatchFirstAttributeValue("Name", value, nodeLists.ChildNodes);
                if (xmlNodes != null)
                {
                    string str = xmlNodes.Attributes["ID"].Value;
                    if (!str.StartsWith("{") || !str.EndsWith("}"))
                    {
                        str = string.Concat("{", str, "}");
                    }

                    nodeWantedField.Attributes["List"].Value = str;
                    if (xmlAttribute != null && webID != null)
                    {
                        if (xmlAttribute.Value.ToLower() == Utils.GetNameFromURL(this.Url).ToLower())
                        {
                            webID.Value = this.WebID;
                        }

                        nodeWantedField.Attributes.Remove(xmlAttribute);
                        if (itemOf1 != null)
                        {
                            nodeWantedField.Attributes.Remove(itemOf1);
                        }
                    }
                }

                nodeWantedField.Attributes.Remove(itemOf);
                nodeWantedField.Attributes.Remove(xmlAttribute);
                nodeWantedField.Attributes.Remove(itemOf1);
            }
        }

        public string UpdateGroupQuickLaunch(string groups)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(groups);
            string value = xmlNode.Attributes["Url"].Value;
            string associateGroupIDs = xmlNode.Attributes["AssociateGroups"].Value;
            string str = "";
            if (!string.IsNullOrEmpty(groups))
            {
                associateGroupIDs = this.GetAssociateGroupIDs(associateGroupIDs);
                str = string.Concat("vti_associategroups;SW|", RPCUtil.EscapeAndEncodeValue(associateGroupIDs), ";");
            }

            if (!string.IsNullOrEmpty(str))
            {
                string str1 = string.Concat("method=set+service+meta-info&service_name=&meta_info=[",
                    str.Substring(0, str.Length - 1), "]");
                RPCUtil.SendRequest(this, string.Concat(value, "/_vti_bin/_vti_aut/author.dll "), str1);
            }

            return string.Empty;
        }

        public string UpdateList(string sListID, string sListXML, string sViewXml, UpdateListOptions updateOptions,
            byte[] documentTemplateFile)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(this.GetLists());
            XmlNode xmlNodes = XmlUtility.MatchFirstAttributeValue("ID", sListID, xmlNode.ChildNodes);
            string value = xmlNodes.Attributes["Name"].Value;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sListXML);
            XmlNode firstChild = xmlDocument.FirstChild;
            string str = (firstChild.Attributes["Title"] != null ? firstChild.Attributes["Title"].Value : value);
            int num = 1;
            string str1 = str;
            for (XmlNode i = XmlUtility.MatchFirstAttributeValue("Title", str, xmlNode.ChildNodes);
                 i != null && i.Attributes["Name"].Value != value;
                 i = XmlUtility.MatchFirstAttributeValue("Title", str, xmlNode.ChildNodes))
            {
                str = string.Concat(str1, num.ToString());
                num++;
            }

            this.UpdateListProperties(sListID, value, str, sViewXml, firstChild, updateOptions, documentTemplateFile,
                webServiceForLists, xmlNode);
            return this.GetList(sListID);
        }

        public void UpdateListAllowMultiResponses(string sListID, string sWebUrl, bool bAllowMultiResponses)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("List");
            if (!bAllowMultiResponses)
            {
                xmlTextWriter.WriteAttributeString("AllowMultiResponses", "FALSE");
            }
            else
            {
                xmlTextWriter.WriteAttributeString("AllowMultiResponses", "TRUE");
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(stringWriter.ToString());
            ListsService webServiceForLists = this.GetWebServiceForLists(sWebUrl);
            webServiceForLists.UpdateList(sListID, xmlNode, null, null, null, null);
        }

        public string UpdateListItem(string sListID, string sParentFolder, int iItemID, string sListItemXML,
            string[] attachmentNames, byte[][] attachmentContents, UpdateListItemOptions updateOptions)
        {
            if (!this.ClientOMAvailable)
            {
                return this.UpdateListItemData(sListID, sParentFolder, iItemID,
                    XmlUtility.StringToXmlNode(sListItemXML), attachmentNames, attachmentContents, null, updateOptions,
                    this.Url, false, 0, this.GetWebServiceForLists(this.Url));
            }

            object[] objArray = new object[]
            {
                sListID, sParentFolder, iItemID, sListItemXML, attachmentNames, attachmentContents, updateOptions, this
            };
            return (string)this.ExecuteClientOMMethod("UpdateListItem", objArray);
        }

        private string UpdateListItemData(string sListID, string sParentFolder, int iItemID, XmlNode listItemXML,
            string[] attachementNames, byte[][] attachmentContents, XmlNode listXML, IUpdateListItemOptions Options,
            string sWebUrl, bool bAdding, int iParentID, ListsService listsSvs)
        {
            string str;
            string str1;
            string str2;
            string str3 = null;
            if (listXML == null)
            {
                listXML = listsSvs.GetList(sListID);
            }

            this.GetDirListName(listXML.Attributes, out str, out str1);
            if (str.Equals(""))
            {
                str2 = string.Concat('/', str1, sParentFolder);
            }
            else
            {
                object[] objArray = new object[] { '/', str, '/', str1, sParentFolder };
                str2 = string.Concat(objArray);
            }

            string str4 = str2;
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(listXML, "//sp:Fields/sp:Field");
            if (xmlNodeLists.Count == 0)
            {
                xmlNodeLists = XmlUtility.RunXPathQuery(listXML, "//List/Fields/Field");
            }

            str3 = this.ConvertWSFields(xmlNodeLists);
            if (this.GetListServerTemplate(listXML) == "108")
            {
                throw new OperationNotSupportedByAdapterException("Copying discussion items",
                    "Native SharePoint Web Services Adapter");
            }

            this.UpdateContentTypeID(listItemXML);
            XmlNode itemOf = listsSvs.UpdateListItems(sListID,
                this.BuildUpdateItemCommand(listXML, listItemXML, xmlNodeLists, iItemID.ToString(), bAdding, str4));
            if (itemOf.InnerText != "0x00000000")
            {
                throw new Exception(string.Concat("Add failed! ", itemOf.InnerText));
            }

            itemOf = XmlUtility.RunXPathQuery(itemOf, "//z:row")[0];
            string value = itemOf.Attributes["ows_ID"].Value;
            this.AddListItemAttachments(attachementNames, attachmentContents, sListID, value, listsSvs);
            return this.GetListItems(sListID, value, str3, null, true,
                ListItemQueryType.ListItem | ListItemQueryType.Folder, sWebUrl, listXML.OuterXml,
                new GetListItemOptions());
        }

        public string UpdateListItemStatus(bool bPublish, bool bCheckin, bool bApprove, string sItemXML,
            string sListXML, string sItemID, string sCheckinComment, string sPublishComment, string sApprovalComment)
        {
            string value;
            string str;
            ListsService webServiceForLists = this.GetWebServiceForLists();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sItemXML);
            XmlNode xmlNodes = XmlUtility.StringToXmlNode(sListXML);
            string str1 = (xmlNode.Attributes["ows_FileLeafRef"] != null ? "ows_" : "");
            if (xmlNode.Attributes[string.Concat(str1, "_Level")] != null)
            {
                value = xmlNode.Attributes[string.Concat(str1, "_Level")].Value;
            }
            else
            {
                value = (xmlNode.Attributes[string.Concat(str1, "_VersionLevel")] == null
                    ? "1"
                    : xmlNode.Attributes[string.Concat(str1, "_VersionLevel")].Value);
            }

            string str2 = value;
            string server = this.Server;
            if (xmlNode.Attributes[string.Concat(str1, "_FileRef")] != null)
            {
                string value1 = xmlNode.Attributes[string.Concat(str1, "_FileRef")].Value;
                char[] chrArray = new char[] { '#' };
                str = value1.Split(chrArray)[1];
            }
            else
            {
                str = xmlNode.Attributes[string.Concat(str1, "FileRef")].Value;
            }

            string str3 = string.Concat(server, "/", str);
            string str4 = (str2 == "2" ? "0" : "1");
            if (bPublish)
            {
                str4 = "1";
            }

            if (bCheckin && !bPublish && !bApprove)
            {
                webServiceForLists.CheckInFile(str3, sCheckinComment, str4);
            }

            if (bPublish)
            {
                if (string.IsNullOrEmpty(xmlNode.Attributes["CheckoutUser"].Value))
                {
                    webServiceForLists.CheckOutFile(str3, "true", null);
                }

                webServiceForLists.CheckInFile(str3, sPublishComment, str4);
            }

            if (bApprove)
            {
                if (string.IsNullOrEmpty(xmlNode.Attributes["CheckoutUser"].Value))
                {
                    webServiceForLists.CheckOutFile(str3, "true", null);
                }

                webServiceForLists.CheckInFile(str3, "", "1");
                xmlNode.Attributes["_ModerationStatus"].Value = "0";
                xmlNode.Attributes["_ModerationComments"].Value = sApprovalComment;
                string value2 = xmlNodes.Attributes["ID"].Value;
                webServiceForLists.UpdateListItems(value2,
                    this.BuildUpdateModerationCommand(xmlNodes, xmlNode, sItemID));
            }

            return string.Empty;
        }

        public void UpdateListProperties(string sListID, string sName, string sTitle, string sViewXml, XmlNode listXML,
            IUpdateListOptions options, byte[] documentTemplateFile, XmlNode currentLists)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists();
            this.UpdateListProperties(sListID, sName, sTitle, sViewXml, listXML, options, documentTemplateFile,
                webServiceForLists, currentLists);
        }

        private void UpdateListProperties(string sListID, string sName, string sTitle, string sViewXml, XmlNode listXML,
            IUpdateListOptions options, byte[] documentTemplateFile, ListsService listsSvs, XmlNode currentLists)
        {
            string str;
            int num;
            int num1;
            int num2;
            XmlNode xmlNodes;
            XmlNode xmlNodes1;
            string value;
            string value1;
            XmlNode xmlNodes2;
            XmlNode xmlNodes3;
            XmlNode xmlNodes4;
            string rootFolder = null;
            string str1 = null;
            string value2 = null;
            XmlNode xmlNodes5 = XmlUtility.MatchFirstAttributeValue("ID", sListID, currentLists.ChildNodes);
            if (xmlNodes5 == null)
            {
                return;
            }

            rootFolder = this.GetRootFolder(xmlNodes5.Attributes);
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("List");
            if (listXML.Attributes["Description"] != null)
            {
                value = listXML.Attributes["Description"].Value;
            }
            else
            {
                value = null;
            }

            string str2 = value;
            if (listXML.Attributes["BaseTemplate"] == null || !(listXML.Attributes["BaseTemplate"].Value != "109"))
            {
                str = "";
            }
            else
            {
                str = (listXML.Attributes["EnableModeration"] == null
                    ? ""
                    : listXML.Attributes["EnableModeration"].InnerText);
            }

            string str3 = (listXML.Attributes["EnableVersioning"] == null
                ? ""
                : listXML.Attributes["EnableVersioning"].InnerText);
            string innerText = "FALSE";
            string innerText1 = "0";
            string innerText2 = "0";
            if (base.SharePointVersion.IsSharePoint2007OrLater)
            {
                XmlNode itemOf = listXML.Attributes["EnableMinorVersions"];
                if (itemOf != null)
                {
                    innerText = itemOf.InnerText;
                }

                itemOf = listXML.Attributes["MajorVersionLimit"];
                if (itemOf != null)
                {
                    innerText1 = itemOf.InnerText;
                }

                itemOf = listXML.Attributes["MajorWithMinorVersionsLimit"];
                if (itemOf != null)
                {
                    innerText2 = itemOf.InnerText;
                }
            }

            xmlTextWriter.WriteAttributeString("Title", sTitle);
            if (str2 != null)
            {
                xmlTextWriter.WriteAttributeString("Description", str2);
            }

            if (str.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                xmlTextWriter.WriteAttributeString("EnableModeration", str);
            }

            if (str3.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                xmlTextWriter.WriteAttributeString("EnableVersioning", str3);
            }

            if (base.SharePointVersion.IsSharePoint2007OrLater)
            {
                if (innerText.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    xmlTextWriter.WriteAttributeString("EnableMinorVersion", innerText);
                }

                if (!string.IsNullOrEmpty(innerText1))
                {
                    xmlTextWriter.WriteAttributeString("MajorVersionLimit", innerText1);
                }

                if (!string.IsNullOrEmpty(innerText2))
                {
                    xmlTextWriter.WriteAttributeString("MajorWithMinorVersionsLimit", innerText2);
                }
            }

            if (listXML.Attributes["OnQuickLaunch"] == null ||
                !(listXML.Attributes["OnQuickLaunch"].InnerText.ToLower() == "true"))
            {
                xmlTextWriter.WriteAttributeString("OnQuickLaunch", "FALSE");
            }
            else
            {
                xmlTextWriter.WriteAttributeString("OnQuickLaunch", "TRUE");
            }

            if (listXML.Attributes["ForceCheckout"] != null)
            {
                xmlTextWriter.WriteAttributeString("RequireCheckout", listXML.Attributes["ForceCheckout"].InnerText);
            }

            if (listXML.Attributes["EnableAttachments"] != null)
            {
                xmlTextWriter.WriteAttributeString("EnableAttachments",
                    listXML.Attributes["EnableAttachments"].InnerText);
            }

            if (listXML.Attributes["AllowMultiResponses"] != null)
            {
                xmlTextWriter.WriteAttributeString("AllowMultiResponses",
                    listXML.Attributes["AllowMultiResponses"].InnerText);
            }

            if (listXML.Attributes["ShowUser"] != null)
            {
                xmlTextWriter.WriteAttributeString("ShowUser", listXML.Attributes["ShowUser"].InnerText);
            }

            if (listXML.Attributes["Hidden"] != null)
            {
                xmlTextWriter.WriteAttributeString("Hidden", listXML.Attributes["Hidden"].InnerText);
            }

            if (listXML.Attributes["MultipleDataList"] != null &&
                currentLists.SelectSingleNode(".//List[@BaseTemplate='200']") != null)
            {
                xmlTextWriter.WriteAttributeString("MultipleDataList", listXML.Attributes["MultipleDataList"].Value);
            }

            if (options.CopyEnableAssignToEmail && listXML.Attributes["EnableAssignToEmail"] != null)
            {
                xmlTextWriter.WriteAttributeString("EnableAssignedToEmail",
                    listXML.Attributes["EnableAssignToEmail"].InnerText);
            }

            if (listXML.Attributes["BrowserEnabledDocuments"] != null)
            {
                xmlTextWriter.WriteAttributeString("BrowserEnabledDocuments",
                    listXML.Attributes["BrowserEnabledDocuments"].InnerText);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            XmlNode xmlNode = XmlUtility.StringToXmlNode(stringWriter.ToString());
            XmlNode xmlNodes6 = null;
            XmlNode xmlNodes7 = null;
            if (!options.CopyFields)
            {
                xmlNodes6 = listsSvs.UpdateList(sListID, xmlNode, null, null, null, null);
            }
            else
            {
                XmlNodeList xmlNodeLists = listXML.SelectNodes("//List/Fields/Field");
                XmlNodeList xmlNodeLists1 =
                    XmlUtility.RunXPathQuery(listsSvs.GetList(sListID), "//sp:List/sp:Fields/sp:Field");
                this.WriteFieldUpdateXml(xmlNodeLists, xmlNodeLists1, out num, out num1, out num2, out xmlNodes7,
                    out xmlNodes, out xmlNodes1, currentLists);
                ListsService listsService = listsSvs;
                string str4 = sListID;
                XmlNode xmlNodes8 = xmlNode;
                if (num > 0)
                {
                    xmlNodes2 = xmlNodes7;
                }
                else
                {
                    xmlNodes2 = null;
                }

                if (num1 > 0)
                {
                    xmlNodes3 = xmlNodes;
                }
                else
                {
                    xmlNodes3 = null;
                }

                if (num2 > 0)
                {
                    xmlNodes4 = xmlNodes1;
                }
                else
                {
                    xmlNodes4 = null;
                }

                xmlNodes6 = listsService.UpdateList(str4, xmlNodes8, xmlNodes2, xmlNodes3, xmlNodes4, null);
            }

            foreach (XmlNode xmlNodes9 in XmlUtility.RunXPathQuery(xmlNodes6, "//sp:ErrorCode"))
            {
            }

            XmlNodeList xmlNodeLists2 = XmlUtility.RunXPathQuery(xmlNodes6, "//sp:ListProperties");
            if (xmlNodeLists2.Count > 0)
            {
                XmlNode itemOf1 = xmlNodeLists2[0];
                XmlAttribute xmlAttribute = itemOf1.Attributes["Version"];
                if (xmlAttribute != null)
                {
                    str1 = xmlAttribute.Value;
                }

                XmlAttribute xmlAttribute1 = itemOf1.Attributes["DocTemplateUrl"];
                if (xmlAttribute1 != null)
                {
                    value2 = xmlAttribute1.Value;
                }

                rootFolder = this.GetRootFolder(itemOf1.Attributes);
            }

            if (listXML.Attributes["DocTemplateUrl"] != null && documentTemplateFile != null)
            {
                this.SetDocumentLibraryTemplate(sListID, rootFolder, value2, str1,
                    listXML.Attributes["DocTemplateUrl"].Value, documentTemplateFile);
                if (listXML.Attributes["BrowserActivatedTemplate"] != null)
                {
                    bool flag = false;
                    if (bool.TryParse(listXML.Attributes["BrowserActivatedTemplate"].Value, out flag) && flag && listXML
                            .Attributes["DocTemplateUrl"].Value.EndsWith(".xsn", StringComparison.OrdinalIgnoreCase))
                    {
                        FormsService webServiceForForms = this.GetWebServiceForForms();
                        Uri uri = new Uri(this.Url);
                        Uri uri1 = new Uri(uri, listXML.Attributes["DocTemplateUrl"].Value);
                        MessagesResponse messagesResponse =
                            webServiceForForms.BrowserEnableUserFormTemplate(uri1.AbsoluteUri);
                        if (messagesResponse.Messages != null && (int)messagesResponse.Messages.Length > 0)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            Message[] messages = messagesResponse.Messages;
                            for (int i = 0; i < (int)messages.Length; i++)
                            {
                                stringBuilder.AppendLine(messages[i].DetailedMessage);
                            }

                            throw new Exception(string.Concat("Failed to activate form template : ", stringBuilder));
                        }
                    }
                }
            }

            if (options.CopyViews)
            {
                this.AddViewsToList(sListID, sViewXml, listXML, xmlNode, xmlNodes7, options.DeletePreExistingViews,
                    listsSvs);
            }

            if (listXML.Attributes["OnQuickLaunch"] == null ||
                !(listXML.Attributes["OnQuickLaunch"].InnerText.ToLower() == "true"))
            {
                this.SetQuickLaunch(sListID, false);
            }
            else
            {
                this.SetQuickLaunch(sListID, true);
            }

            string str5 = "";
            if (listXML.Attributes["EmailAttachmentFolder"] != null)
            {
                str5 = string.Concat(str5, "vti_emailattachmentfolders;SW|",
                    listXML.Attributes["EmailAttachmentFolder"].Value, ";");
            }

            if (listXML.Attributes["EmailOverWrite"] != null)
            {
                str5 = string.Concat(str5, "vti_emailoverwrite;IW|", listXML.Attributes["EmailOverWrite"].Value, ";");
            }

            if (listXML.Attributes["EmailSaveOriginal"] != null)
            {
                str5 = string.Concat(str5, "vti_emailsaveoriginal;IW|", listXML.Attributes["EmailSaveOriginal"].Value,
                    ";");
            }

            if (listXML.Attributes["EmailSaveMeetings"] != null)
            {
                str5 = string.Concat(str5, "vti_emailsavemeetings;IW|", listXML.Attributes["EmailSaveMeetings"].Value,
                    ";");
            }

            if (listXML.Attributes["EmailUseSecurity"] != null)
            {
                str5 = string.Concat(str5, "vti_emailusesecurity;IW|", listXML.Attributes["EmailUseSecurity"].Value,
                    ";");
            }

            if (listXML.Attributes["RssChannelTitle"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_ChannelTitle;SW|",
                    RPCUtil.EscapeAndEncodeValue(listXML.Attributes["RssChannelTitle"].Value), ";");
            }

            if (listXML.Attributes["RssChannelDescription"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_ChannelDescription;SW|",
                    RPCUtil.EscapeAndEncodeValue(listXML.Attributes["RssChannelDescription"].Value), ";");
            }

            if (listXML.Attributes["RssLimitDescriptionLength"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_LimitDescriptionLength;IW|",
                    listXML.Attributes["RssLimitDescriptionLength"].Value, ";");
            }

            if (listXML.Attributes["RssChannelImageUrl"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_ChannelImageUrl;SW|",
                    listXML.Attributes["RssChannelImageUrl"].Value, ";");
            }

            if (listXML.Attributes["RssItemLimit"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_ItemLimit;IW|", listXML.Attributes["RssItemLimit"].Value, ";");
            }

            if (listXML.Attributes["RssDayLimit"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_DayLimit;IW|", listXML.Attributes["RssDayLimit"].Value, ";");
            }

            if (listXML.Attributes["RssDocumentAsEnclosure"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_DocumentAsEnclosure;IW|",
                    listXML.Attributes["RssDocumentAsEnclosure"].Value, ";");
            }

            if (listXML.Attributes["RssDocumentAsLink"] != null)
            {
                str5 = string.Concat(str5, "vti_rss_DocumentAsLink;IW|", listXML.Attributes["RssDocumentAsLink"].Value,
                    ";");
            }

            if (listXML.Attributes["WelcomePage"] != null)
            {
                str5 = string.Concat(str5, "vti_welcomepage;SW|", listXML.Attributes["WelcomePage"].Value, ";");
            }

            if (str5.Length > 0)
            {
                string str6 = sName;
                foreach (XmlNode xmlNodes10 in xmlNodes6)
                {
                    if (xmlNodes10.Name != "ListProperties")
                    {
                        continue;
                    }

                    string rootFolder1 = this.GetRootFolder(xmlNodes10.Attributes);
                    str6 = rootFolder1.Replace(string.Concat(this.ServerRelativeUrl, "/"), "");
                    break;
                }

                string[] strArrays = new string[]
                {
                    "method=set+document+meta-info&service_name=&document_name=", str6, "&meta_info=[",
                    str5.Substring(0, str5.Length - 1), "]"
                };
                string str7 = string.Concat(strArrays);
                RPCUtil.SendRequest(this, string.Concat(this.Url, "/_vti_bin/_vti_aut/author.dll"), str7);
            }

            if (listXML.Attributes["ContentTypesEnabled"] != null)
            {
                value1 = listXML.Attributes["ContentTypesEnabled"].Value;
            }
            else
            {
                value1 = null;
            }

            string str8 = value1;
            if (!string.IsNullOrEmpty(str8))
            {
                bool flag1 = false;
                if (bool.TryParse(str8, out flag1) && this.ClientOMAvailable)
                {
                    object[] objArray = new object[] { flag1, sListID, this };
                    this.ExecuteClientOMMethod("SetContentTypesEnabled", objArray);
                }
            }
        }

        public XmlNode UpdateModerationStatus(XmlNode listXML, XmlNode listItemXML, string sListID, string sID)
        {
            ListsService webServiceForLists = this.GetWebServiceForLists(this.Url);
            return webServiceForLists.UpdateListItems(sListID,
                this.BuildUpdateModerationCommand(listXML, listItemXML, sID));
        }

        public XmlNode UpdateModerationStatus(ListsService listsSvs, XmlNode listXML, XmlNode listItemXML,
            string sListID, string sID)
        {
            return listsSvs.UpdateListItems(sListID, this.BuildUpdateModerationCommand(listXML, listItemXML, sID));
        }

        public string UpdateSiteCollectionSettings(string sUpdateXml,
            UpdateSiteCollectionOptions updateSiteCollectionOptions)
        {
            throw new NotImplementedException();
        }

        private void UpdateSubWebGuidDict()
        {
            string str;
            string[] strArrays;
            string[] strArrays1;
            if (base.SharePointVersion.IsSharePoint2003)
            {
                return;
            }

            XmlNode webCollection = this.GetWebServiceForWebs().GetWebCollection();
            Dictionary<Guid, string> guids = new Dictionary<Guid, string>();
            foreach (XmlNode childNode in webCollection.ChildNodes)
            {
                string value = childNode.Attributes["Url"].Value;
                SiteDataService webServiceForSiteData = this.GetWebServiceForSiteData(value);
                _sWebMetadata _sWebMetadatum = null;
                _sWebWithTime[] _sWebWithTimeArray = null;
                _sListWithTime[] _sListWithTimeArray = null;
                _sFPUrl[] _sFPUrlArray = null;
                webServiceForSiteData.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray, out _sListWithTimeArray,
                    out _sFPUrlArray, out str, out strArrays, out strArrays1);
                Guid guid = new Guid(NWSAdapter.GetJustGUID(_sWebMetadatum.WebID));
                StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this, value);
                guids.Add(guid, standardizedUrl.WebRelative);
            }

            this.m_SubWebGuidDict = guids;
        }

        private XmlNode UpdateViewSettingsThroughClientOM(string sListID, string sViewName, string sContentTypeId)
        {
            object[] objArray = new object[] { sListID, sViewName, sContentTypeId, this };
            return (XmlNode)this.ExecuteClientOMMethod("UpdateViewSettings", objArray);
        }

        public string UpdateWeb(string sWebXML, UpdateWebOptions updateOptions)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXML);
            string value = xmlNode.Attributes["Name"].Value;
            string str = string.Concat(this.Server, this.ServerRelativeUrl);
            StringWriter stringWriter = new StringWriter(new StringBuilder(512));
            XmlWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            this.UpdateWebProperties(str, xmlNode, updateOptions, true);
            xmlTextWriter.WriteStartElement("Web");
            this.FillWebXML(xmlTextWriter, str, null, true, true);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        private void UpdateWebHiddenNavNodeGuids(Guid[] globalNavHiddenIds, Guid[] currentNavHiddenIds)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("method=set+service+meta-info&service_name=&meta_info=[");
            stringBuilder.Append("__GlobalNavigationExcludes;SW|");
            if (globalNavHiddenIds != null)
            {
                Guid[] guidArray = globalNavHiddenIds;
                for (int i = 0; i < (int)guidArray.Length; i++)
                {
                    Guid guid = guidArray[i];
                    stringBuilder.Append(string.Concat(guid.ToString(), "\\;"));
                }
            }

            stringBuilder.Append(";__CurrentNavigationExcludes;SW|");
            if (currentNavHiddenIds != null)
            {
                Guid[] guidArray1 = currentNavHiddenIds;
                for (int j = 0; j < (int)guidArray1.Length; j++)
                {
                    Guid guid1 = guidArray1[j];
                    stringBuilder.Append(string.Concat(guid1.ToString(), "\\;"));
                }
            }

            stringBuilder.Append("]");
            RPCUtil.SendRequest(this, Utils.JoinUrl(this.Url, "/_vti_bin/_vti_aut/author.dll"),
                stringBuilder.ToString());
        }

        public string UpdateWebNavigationStructure(string sUpdateXml)
        {
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sUpdateXml);
            XmlNode xmlNodes = xmlNode.SelectSingleNode("./AdditionsAndUpdates");
            XmlNode xmlNodes1 = xmlNode.SelectSingleNode("./Deletions");
            if (!base.SharePointVersion.IsSharePoint2007OrEarlier)
            {
                this.UpdateWebNavigationStructure2010(xmlNodes, xmlNodes1);
            }
            else
            {
                this.UpdateWebNavigationStructure2007(xmlNodes, xmlNodes1);
            }

            return this.GetWebNavigationStructure();
        }

        private void UpdateWebNavigationStructure2007(XmlNode additionsAndUpdatesNode, XmlNode deletionsNode)
        {
            string[] strArrays;
            string[] strArrays1;
            string webStructure = this.GetWebStructure();
            XmlNode xmlNode =
                XmlUtility.StringToXmlNode(
                    this.GetNavigationStructureXMLFromWebStructureString(webStructure, null, null));
            Guid[] guidArray = null;
            Guid[] guidArray1 = null;
            Dictionary<Guid, string> pagesLibraryDocIDToURLMap = null;
            bool flag = false;
            if (this.GetNavNodeAdditionsAndUpdatesRequireIsVisibleChange(additionsAndUpdatesNode, xmlNode))
            {
                Guid[] guidArray2 = null;
                this.GetNavNodeHiddenGuids(out guidArray2, out guidArray, out guidArray1);
                pagesLibraryDocIDToURLMap = this.GetPagesLibraryDocIDToURLMap(null);
                this.GetNavNodeHiddenUrls(out strArrays, out strArrays1, guidArray, guidArray1,
                    pagesLibraryDocIDToURLMap);
                xmlNode = XmlUtility.StringToXmlNode(
                    this.GetNavigationStructureXMLFromWebStructureString(webStructure, strArrays, strArrays1));
                flag = true;
            }

            List<Guid> guids = (guidArray == null ? new List<Guid>() : new List<Guid>(guidArray));
            List<Guid> guids1 = (guidArray1 == null ? new List<Guid>() : new List<Guid>(guidArray1));
            bool flag1 = false;
            string str = this.BuildWebStructureUpdate(additionsAndUpdatesNode, deletionsNode, xmlNode, flag,
                pagesLibraryDocIDToURLMap, ref guids, ref guids1, out flag1);
            if (flag1)
            {
                this.UpdateWebHiddenNavNodeGuids(guids.ToArray(), guids1.ToArray());
            }

            string str1 = RPCUtil.SendRequest(this, Utils.JoinUrl(this.Url, "/_vti_bin/_vti_aut/author.dll"), str);
            string rPCErrorMessage = RPCUtil.GetRPCErrorMessage(str1);
            if (!string.IsNullOrEmpty(rPCErrorMessage))
            {
                throw new Exception(rPCErrorMessage);
            }
        }

        private void UpdateWebNavigationStructure2010(XmlNode additionsAndUpdatesNode, XmlNode deletionsNode)
        {
            Guid[] guidArray = null;
            Guid[] guidArray1 = null;
            string[] strArrays = null;
            string[] strArrays1 = null;
            Dictionary<Guid, string> pagesLibraryDocIDToURLMap = null;
            bool flag = false;
            if (this.GetNavNodeAdditionsAndUpdatesRequireIsVisibleChange2010(additionsAndUpdatesNode))
            {
                Guid[] guidArray2 = null;
                this.GetNavNodeHiddenGuids(out guidArray2, out guidArray, out guidArray1);
                pagesLibraryDocIDToURLMap = this.GetPagesLibraryDocIDToURLMap(null);
                this.GetNavNodeHiddenUrls(out strArrays, out strArrays1, guidArray, guidArray1,
                    pagesLibraryDocIDToURLMap);
                flag = true;
            }

            List<Guid> guids = (guidArray == null ? new List<Guid>() : new List<Guid>(guidArray));
            List<Guid> guids1 = (guidArray1 == null ? new List<Guid>() : new List<Guid>(guidArray1));
            bool flag1 = false;
            this.UpdateWebNavigationStructureClientOM(additionsAndUpdatesNode, deletionsNode, flag, strArrays,
                strArrays1, pagesLibraryDocIDToURLMap, ref guids, ref guids1, out flag1);
            if (flag1)
            {
                this.UpdateWebHiddenNavNodeGuids(guids.ToArray(), guids1.ToArray());
            }
        }

        public string UpdateWebNavigationStructureClientOM(XmlNode additionsAndUpdates, XmlNode deletions,
            bool bWatchForIsVisibleChanges, string[] hiddenGlobalUrls, string[] hiddenCurrentUrls,
            Dictionary<Guid, string> pagesLibIDMap, ref List<Guid> hiddenGlobalNodes, ref List<Guid> hiddenCurrentNodes,
            out bool bHiddenChangesMade)
        {
            object[] objArray = new object[]
            {
                additionsAndUpdates, deletions, this, bWatchForIsVisibleChanges, hiddenGlobalUrls, hiddenCurrentUrls,
                pagesLibIDMap, this.SubWebGuidDict, hiddenGlobalNodes, hiddenCurrentNodes, false
            };
            object[] objArray1 = objArray;
            string str = (string)this.ExecuteClientOMMethod("UpdateWebNavigationStructure", objArray1);
            hiddenGlobalNodes = (List<Guid>)objArray1[8];
            hiddenCurrentNodes = (List<Guid>)objArray1[9];
            bHiddenChangesMade = (bool)objArray1[10];
            return str;
        }

        private void UpdateWebPart(Guid webPartGuid, string sWebPartXml, string sPageUrlContainingWebPart)
        {
            WebPartPagesService webServiceForWebParts = this.GetWebServiceForWebParts();
            try
            {
                webServiceForWebParts.SaveWebPart(sPageUrlContainingWebPart, webPartGuid, sWebPartXml, Storage.Shared);
            }
            catch (SoapException soapException1)
            {
                SoapException soapException = soapException1;
                string soapError = RPCUtil.GetSoapError(soapException.Detail);
                throw new Exception(
                    string.Concat("Soap exception while updating web part on page '", sPageUrlContainingWebPart,
                        "', error: ", soapError), soapException);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                throw new Exception(
                    string.Concat("A problem was encountered updating a web part on page '", sPageUrlContainingWebPart,
                        "'. Error: ", exception.Message), exception);
            }
        }

        private void UpdateWebPartsWithInfoFromPage(string sWebPartPageServerRelativeUrl,
            WebPartPagesService wppService, IEnumerable webPartNodes)
        {
            string webPartPage = null;
            try
            {
                string str = Utils.JoinUrl(this.Server, sWebPartPageServerRelativeUrl);
                webPartPage = wppService.GetWebPartPage(str, SPWebServiceBehavior.Version3);
            }
            catch
            {
                return;
            }

            Dictionary<string, bool> strs = new Dictionary<string, bool>();
            this.GetIsIncludedForV3WebParts(webPartPage, strs);
            DataTable dataTable = new DataTable();
            NWSAdapter.MatchWebPartPropertiesWithRegex(webPartPage, dataTable);
            foreach (XmlNode webPartNode in webPartNodes)
            {
                string value = webPartNode.Attributes["ID"].Value;
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!value.Equals(row["WebPartStorageKey"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (webPartNode.OwnerDocument == null)
                    {
                        break;
                    }

                    XmlElement xmlElement = webPartNode.OwnerDocument.CreateElement("ZoneID");
                    string str1 = row["WebPartZone"].ToString();
                    xmlElement.InnerText = str1;
                    XmlElement xmlElement1 = webPartNode.OwnerDocument.CreateElement("PartOrder");
                    string str2 = row["PartOrder"].ToString();
                    if (string.IsNullOrEmpty(str2))
                    {
                        str2 = "0";
                    }

                    xmlElement1.InnerText = str2;
                    XmlElement xmlElement2 = webPartNode.OwnerDocument.CreateElement("ID");
                    xmlElement2.InnerText = row["WebPartID"].ToString();
                    webPartNode.AppendChild(xmlElement);
                    webPartNode.AppendChild(xmlElement1);
                    webPartNode.AppendChild(xmlElement2);
                    XmlAttribute itemOf = webPartNode.Attributes["Embedded"];
                    if (itemOf == null)
                    {
                        itemOf = webPartNode.OwnerDocument.CreateAttribute("Embedded");
                        webPartNode.Attributes.Append(itemOf);
                    }

                    bool flag = string.Equals(str1, "wpz", StringComparison.OrdinalIgnoreCase);
                    itemOf.Value = flag.ToString();
                    break;
                }

                if (!strs.ContainsKey(value.ToUpper()))
                {
                    continue;
                }

                XmlElement lower = webPartNode.OwnerDocument.CreateElement("IsIncluded");
                bool item = strs[value.ToUpper()];
                lower.InnerText = item.ToString().ToLower();
                webPartNode.AppendChild(lower);
            }
        }

        public void UpdateWebProperties(string sUrl, XmlNode xWebNode, IUpdateWebOptions updateWebOptions,
            bool updateRSSSetting = true)
        {
            string str = "";
            if (updateWebOptions.CopyNavigation)
            {
                str = string.Concat(str, this.ConvertWebNavigationToRpcString(sUrl, xWebNode));
            }

            if (updateWebOptions.CopyCoreMetaData && updateRSSSetting)
            {
                if (xWebNode.Attributes["RssCopyright"] != null)
                {
                    str = string.Concat(str, "vti_rss_Copyright;SW|",
                        RPCUtil.EscapeAndEncodeValue(xWebNode.Attributes["RssCopyright"].Value), ";");
                }

                if (xWebNode.Attributes["RssManagingEditor"] != null)
                {
                    str = string.Concat(str, "vti_rss_ManagingEditor;SW|",
                        RPCUtil.EscapeAndEncodeValue(xWebNode.Attributes["RssManagingEditor"].Value), ";");
                }

                if (xWebNode.Attributes["RssWebMaster"] != null)
                {
                    str = string.Concat(str, "vti_rss_WebMaster;SW|",
                        RPCUtil.EscapeAndEncodeValue(xWebNode.Attributes["RssWebMaster"].Value), ";");
                }

                if (xWebNode.Attributes["RssTimeToLive"] != null)
                {
                    str = string.Concat(str, "vti_rss_TimeToLive;IW|", xWebNode.Attributes["RssTimeToLive"].Value, ";");
                }
            }

            if (updateWebOptions.CopyAssociatedGroupSettings)
            {
                List<string> strs = new List<string>();
                if (xWebNode.Attributes["OwnerGroup"] != null &&
                    !string.IsNullOrEmpty(xWebNode.Attributes["OwnerGroup"].Value))
                {
                    str = string.Concat(str, "vti_associateownergroup;SW|",
                        this.GetIDFromGroup(xWebNode.Attributes["OwnerGroup"].Value), ";");
                    strs.Add(xWebNode.Attributes["OwnerGroup"].Value);
                }

                if (xWebNode.Attributes["MemberGroup"] != null &&
                    !string.IsNullOrEmpty(xWebNode.Attributes["MemberGroup"].Value))
                {
                    str = string.Concat(str, "vti_associatemembergroup;SW|",
                        this.GetIDFromGroup(xWebNode.Attributes["MemberGroup"].Value), ";");
                    strs.Add(xWebNode.Attributes["MemberGroup"].Value);
                }

                if (xWebNode.Attributes["VisitorGroup"] != null &&
                    !string.IsNullOrEmpty(xWebNode.Attributes["VisitorGroup"].Value))
                {
                    str = string.Concat(str, "vti_associatevisitorgroup;SW|",
                        this.GetIDFromGroup(xWebNode.Attributes["VisitorGroup"].Value), ";");
                    strs.Add(xWebNode.Attributes["VisitorGroup"].Value);
                }

                if (xWebNode.Attributes["AssociateGroups"] != null &&
                    !string.IsNullOrEmpty(xWebNode.Attributes["AssociateGroups"].Value))
                {
                    string[] strArrays = xWebNode.Attributes["AssociateGroups"].Value.Trim().Split(new char[] { ';' });
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = strArrays[i];
                        if (!strs.Contains(str1))
                        {
                            strs.Add(str1);
                        }
                    }

                    str = string.Concat(str, "vti_associategroups;SW|",
                        RPCUtil.EscapeAndEncodeValue(this.GetAssociateGroupIDs(strs)), ";");
                }
            }

            if (!string.IsNullOrEmpty(str))
            {
                string str2 = string.Concat("method=set+service+meta-info&service_name=&meta_info=[",
                    str.Substring(0, str.Length - 1), "]");
                RPCUtil.SendRequest(this, string.Concat(sUrl, "/_vti_bin/_vti_aut/author.dll "), str2);
            }

            if (updateWebOptions.CopyCoreMetaData)
            {
                XmlNode xmlNodes = xWebNode.SelectSingleNode(".//MeetingInstances");
                if (xmlNodes != null)
                {
                    this.AddMeetingInstances(xmlNodes, sUrl);
                }
            }
        }

        public string ValidateUserInfo(string sUserIdentifier, bool bCanBeDomainGroup)
        {
            throw new NotImplementedException();
        }

        private bool WebPartPageZonesContains(string sQueryZone)
        {
            string welcomePage = this.GetWelcomePage(this.Url);
            if (welcomePage == null)
            {
                string serverRelativeUrl = this.ServerRelativeUrl;
                char[] chrArray = new char[] { '/' };
                welcomePage = string.Concat(serverRelativeUrl.TrimEnd(chrArray), "/default.aspx");
            }

            string webPartPage = this.GetWebPartPage(welcomePage, false, this);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(webPartPage);
            if (xmlNode != null)
            {
                XmlNode xmlNodes = xmlNode.SelectSingleNode(".//ZonesFromAspx");
                if (xmlNodes != null)
                {
                    string[] strArrays = xmlNodes.InnerText.Split(new char[] { ',' });
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        if (string.Equals(sQueryZone, strArrays[i], StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void WriteAddField(XmlNode nodeWantedField, XmlWriter xmlWriterNewFields,
            XmlWriter xmlWriterUpdateFields, ref List<XmlNode> delayedUpdates, ref int iMethodID, ref int iNewFields,
            ref int iUpdateFields, XmlNode nodeLists)
        {
            if (nodeWantedField.Attributes["Type"].Value == "Lookup" ||
                nodeWantedField.Attributes["Type"].Value == "LookupMulti")
            {
                this.UpdateFieldLookupListNames(nodeWantedField, nodeLists);
            }

            XmlNode innerText = nodeWantedField.Clone();
            bool flag = false;
            bool flag1 = false;
            if (innerText.Attributes["Name"] == null || innerText.Attributes["DisplayName"] == null ||
                !(innerText.Attributes["Name"].InnerText != innerText.Attributes["DisplayName"].InnerText))
            {
                XmlAttribute itemOf = innerText.Attributes["Name"] ?? innerText.Attributes["DisplayName"];
                if (itemOf != null)
                {
                    itemOf.Value = Utils.EnsureFieldNameSafety(itemOf.Value);
                }
            }
            else
            {
                innerText.Attributes["DisplayName"].InnerText = innerText.Attributes["Name"].InnerText;
                innerText.Attributes["Name"].Value = Utils.EnsureFieldNameSafety(innerText.Attributes["Name"].Value);
                flag1 = true;
            }

            if (innerText.Attributes["Type"].Value.Equals("calculated", StringComparison.OrdinalIgnoreCase))
            {
                XmlAttribute xmlAttribute = innerText.Attributes["ResultType"];
                if (xmlAttribute != null)
                {
                    xmlAttribute.Value = "Text";
                }

                if (innerText.Attributes["Format"] != null)
                {
                    xmlAttribute.Value = "DateOnly";
                }

                innerText.InnerXml = "<Formula>=0</Formula><FormulaDisplayNames>=0</FormulaDisplayNames>";
                flag = true;
            }

            iMethodID++;
            iNewFields++;
            xmlWriterNewFields.WriteStartElement("Method");
            xmlWriterNewFields.WriteAttributeString("ID", iMethodID.ToString());
            xmlWriterNewFields.WriteAttributeString("AddToView", "");
            xmlWriterNewFields.WriteRaw(innerText.OuterXml);
            xmlWriterNewFields.WriteEndElement();
            if (flag)
            {
                XmlNode value = nodeWantedField.Clone();
                if (value.Attributes["DisplayName"] != null && innerText.Attributes["DisplayName"] != null)
                {
                    value.Attributes["DisplayName"].Value = innerText.Attributes["DisplayName"].Value;
                }

                iMethodID++;
                iUpdateFields++;
                xmlWriterUpdateFields.WriteStartElement("Method");
                xmlWriterUpdateFields.WriteAttributeString("ID", iMethodID.ToString());
                xmlWriterUpdateFields.WriteAttributeString("AddToView", "");
                xmlWriterUpdateFields.WriteRaw(value.OuterXml);
                xmlWriterUpdateFields.WriteEndElement();
            }

            if (flag1)
            {
                delayedUpdates.Add(nodeWantedField);
            }
        }

        private void WriteAssociatedGroupData(Hashtable ht, XmlWriter xmlWriter)
        {
            string str = "";
            string str1 = "";
            string str2 = "";
            string str3 = "";
            if (ht.ContainsKey("associateownergroup"))
            {
                string str4 = ht["associateownergroup"].ToString();
                string groupFromID = this.GetGroupFromID(str4);
                if (groupFromID != null)
                {
                    str = groupFromID;
                }
            }

            if (ht.ContainsKey("associatemembergroup"))
            {
                string str5 = ht["associatemembergroup"].ToString();
                string groupFromID1 = this.GetGroupFromID(str5);
                if (groupFromID1 != null)
                {
                    str1 = groupFromID1;
                }
            }

            if (ht.ContainsKey("associatevisitorgroup"))
            {
                string str6 = ht["associatevisitorgroup"].ToString();
                string groupFromID2 = this.GetGroupFromID(str6);
                if (groupFromID2 != null)
                {
                    str2 = groupFromID2;
                }
            }

            if (ht.ContainsKey("associategroups"))
            {
                str3 = string.Concat(
                    this.GetAssociateGroupNames(ht["associategroups"].ToString().Replace("&#59;", ";")), ';');
            }

            xmlWriter.WriteAttributeString("OwnerGroup", str);
            xmlWriter.WriteAttributeString("MemberGroup", str1);
            xmlWriter.WriteAttributeString("VisitorGroup", str2);
            xmlWriter.WriteAttributeString("AssociateGroups", str3);
        }

        public void WriteBCSProperties(XmlWriter xmlWriter, XmlNode dataSource)
        {
            if (dataSource != null)
            {
                XmlNode xmlNodes =
                    XmlUtility.RunXPathQuerySelectSingle(dataSource, "//sp:Property[@Name='LobSystemInstance']");
                XmlNode xmlNodes1 =
                    XmlUtility.RunXPathQuerySelectSingle(dataSource, "//sp:Property[@Name='EntityNamespace']");
                XmlNode xmlNodes2 = XmlUtility.RunXPathQuerySelectSingle(dataSource, "//sp:Property[@Name='Entity']");
                XmlNode xmlNodes3 =
                    XmlUtility.RunXPathQuerySelectSingle(dataSource, "//sp:Property[@Name='SpecificFinder']");
                xmlWriter.WriteAttributeString("LobSystemInstance", xmlNodes.Attributes["Value"].Value);
                xmlWriter.WriteAttributeString("EntityNamespace", xmlNodes1.Attributes["Value"].Value);
                xmlWriter.WriteAttributeString("Entity", xmlNodes2.Attributes["Value"].Value);
                xmlWriter.WriteAttributeString("SpecificFinder", xmlNodes3.Attributes["Value"].Value);
            }
        }

        private void WriteContentTypeXml(string sListId, XmlWriter xmlWriter, Hashtable fieldHash, XmlNode node)
        {
            XmlNodeList xmlNodeLists = XmlUtility.RunXPathQuery(node, "./sp:Field");
            if (xmlNodeLists.Count == 0)
            {
                if (sListId == null || sListId.Length == 0)
                {
                    WebsService webServiceForWebs = this.GetWebServiceForWebs();
                    node = webServiceForWebs.GetContentType(node.Attributes["ID"].Value);
                }
                else
                {
                    ListsService webServiceForLists = this.GetWebServiceForLists();
                    node = webServiceForLists.GetListContentType(sListId, node.Attributes["ID"].Value);
                }

                xmlNodeLists = XmlUtility.RunXPathQuery(node, ".//sp:Fields/sp:Field");
            }

            xmlWriter.WriteStartElement("ContentType");
            xmlWriter.WriteAttributeString("Name", node.Attributes["Name"].Value);
            xmlWriter.WriteAttributeString("ID", node.Attributes["ID"].Value);
            if (node.Attributes["Group"] != null)
            {
                xmlWriter.WriteAttributeString("Group", node.Attributes["Group"].Value);
            }

            if (node.Attributes["Description"] != null)
            {
                xmlWriter.WriteAttributeString("Description", node.Attributes["Description"].Value);
            }

            if (node.Attributes["RequireClientRenderingOnNew"] != null)
            {
                xmlWriter.WriteAttributeString("RequireClientRenderingOnNew",
                    node.Attributes["RequireClientRenderingOnNew"].Value);
            }

            bool? attributeValueAsNullableBoolean = node.GetAttributeValueAsNullableBoolean("ReadOnly");
            xmlWriter.WriteAttributeString("ReadOnly",
                (attributeValueAsNullableBoolean.HasValue
                    ? attributeValueAsNullableBoolean.Value.ToString()
                    : false.ToString()));
            xmlWriter.WriteStartElement("FieldRefs");
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                string value = xmlNodes.Attributes["Name"].Value;
                if (fieldHash.ContainsKey(value))
                {
                    continue;
                }

                fieldHash.Add(value, null);
                xmlWriter.WriteStartElement("FieldRef");
                xmlWriter.WriteAttributeString("Name", value);
                if (xmlNodes.Attributes["ID"] != null)
                {
                    xmlWriter.WriteAttributeString("ID", xmlNodes.Attributes["ID"].Value);
                }

                XmlAttribute itemOf = xmlNodes.Attributes["ReadOnly"];
                if (itemOf != null)
                {
                    xmlWriter.WriteAttributeString("ReadOnly", itemOf.Value);
                }

                XmlAttribute xmlAttribute = xmlNodes.Attributes["Required"];
                if (xmlAttribute != null)
                {
                    xmlWriter.WriteAttributeString("Required", xmlAttribute.Value);
                }

                XmlAttribute itemOf1 = xmlNodes.Attributes["Hidden"];
                if (itemOf1 != null)
                {
                    xmlWriter.WriteAttributeString("Hidden", itemOf1.Value);
                }

                string attributeValueAsString = xmlNodes.GetAttributeValueAsString("Node");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("Node", attributeValueAsString);
                }

                attributeValueAsString = xmlNodes.GetAttributeValueAsString("PITarget");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("PITarget", attributeValueAsString);
                }

                attributeValueAsString = xmlNodes.GetAttributeValueAsString("PrimaryPITarget");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("PrimaryPITarget", attributeValueAsString);
                }

                attributeValueAsString = xmlNodes.GetAttributeValueAsString("PIAttribute");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("PIAttribute", attributeValueAsString);
                }

                attributeValueAsString = xmlNodes.GetAttributeValueAsString("PrimaryPIAttribute");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("PrimaryPIAttribute", attributeValueAsString);
                }

                attributeValueAsString = xmlNodes.GetAttributeValueAsString("Aggregation");
                if (!string.IsNullOrEmpty(attributeValueAsString))
                {
                    xmlWriter.WriteAttributeString("Aggregation", attributeValueAsString);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            XmlNode xmlNodes1 = XmlUtility.RunXPathQuerySelectSingle(node, "//sp:DocumentTemplate");
            if (xmlNodes1 != null && xmlNodes1.Attributes["TargetName"] != null)
            {
                xmlWriter.WriteStartElement("DocumentTemplate");
                xmlWriter.WriteAttributeString("TargetName", xmlNodes1.Attributes["TargetName"].Value);
                xmlWriter.WriteEndElement();
            }

            XmlNode xmlNodes2 = XmlUtility.RunXPathQuerySelectSingle(node, "//sp:Folder");
            if (xmlNodes2 != null && xmlNodes2.Attributes["TargetName"] != null)
            {
                xmlWriter.WriteStartElement("Folder");
                xmlWriter.WriteAttributeString("TargetName", xmlNodes2.Attributes["TargetName"].Value);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        private void WriteFieldUpdateXml(XmlNodeList nodesWantedFields, XmlNodeList nodesActualFields,
            out int iNewFields, out int iUpdateFields, out int iDeleteFields, out XmlNode newFields,
            out XmlNode updateFields, out XmlNode deleteFields, XmlNode nodeLists)
        {
            bool flag;
            bool flag1;
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("Fields");
            StringWriter stringWriter1 = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter1 = new XmlTextWriter(stringWriter1);
            xmlTextWriter1.WriteStartElement("Fields");
            StringWriter stringWriter2 = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter2);
            xmlTextWriter2.WriteStartElement("Fields");
            List<string> strs = new List<string>();
            try
            {
                XmlNodeList xmlNodeLists = null;
                this.SortFieldsWithInternalNameDependancy(nodesWantedFields, out xmlNodeLists, out strs);
                nodesWantedFields = xmlNodeLists;
            }
            catch
            {
                strs = null;
            }

            int num = 0;
            iNewFields = 0;
            iUpdateFields = 0;
            iDeleteFields = 0;
            List<XmlNode> xmlNodes = new List<XmlNode>();
            List<XmlNode> xmlNodes1 = new List<XmlNode>();
            List<XmlNode> xmlNodes2 = new List<XmlNode>();
            foreach (XmlNode nodesWantedField in nodesWantedFields)
            {
                if (nodesWantedField.Attributes["MLSystem"] != null)
                {
                    continue;
                }

                bool flag2 = false;
                string innerText = nodesWantedField.Attributes["Name"].InnerText;
                foreach (XmlNode nodesActualField in nodesActualFields)
                {
                    if (innerText != nodesActualField.Attributes["Name"].InnerText)
                    {
                        continue;
                    }

                    flag2 = true;
                    bool flag3 = true;
                    bool flag4 = false;
                    bool flag5 = false;
                    bool flag6 = false;
                    bool flag7 = false;
                    if (nodesActualField.Attributes["SourceID"] != null &&
                        nodesActualField.Attributes["SourceID"].Value
                            .StartsWith("http://schemas.microsoft.com/sharepoint") &&
                        (nodesWantedField.Attributes["SourceID"] == null ||
                         !(nodesActualField.Attributes["SourceID"].Value ==
                           nodesWantedField.Attributes["SourceID"].Value)))
                    {
                        flag7 = true;
                    }

                    if (nodesActualField.Attributes["Sealed"] != null)
                    {
                        bool.TryParse(nodesActualField.Attributes["Sealed"].Value, out flag4);
                    }

                    if (nodesActualField.Attributes["Hidden"] != null)
                    {
                        bool.TryParse(nodesActualField.Attributes["Hidden"].Value, out flag5);
                    }

                    if (nodesActualField.Attributes["ReadOnly"] != null)
                    {
                        bool.TryParse(nodesActualField.Attributes["ReadOnly"].Value, out flag6);
                    }

                    if (flag7 || flag4 || flag5)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = (!flag6 ? true : !(nodesActualField.Attributes["Type"].Value != "Calculated"));
                    }

                    flag3 = flag;
                    if (nodesActualField.Attributes["Type"].InnerText != "Choice" &&
                        nodesWantedField.Attributes["Type"].InnerText == "Choice")
                    {
                        if (flag4 || flag5)
                        {
                            flag1 = false;
                        }
                        else
                        {
                            flag1 = (!flag6 ? true : !(nodesActualField.Attributes["Type"].Value != "Calculated"));
                        }

                        flag3 = flag1;
                    }

                    if (!flag3)
                    {
                        break;
                    }

                    if (nodesWantedField.Attributes["AddToSiteColumnGroup"] != null)
                    {
                        nodesWantedField.Attributes.Remove(nodesWantedField.Attributes["AddToSiteColumnGroup"]);
                    }

                    if (nodesWantedField.Attributes["AddToContentType"] != null)
                    {
                        nodesWantedField.Attributes.Remove(nodesWantedField.Attributes["AddToContentType"]);
                    }

                    if (nodesWantedField.Attributes["Type"].Value == "Lookup" ||
                        nodesWantedField.Attributes["Type"].Value == "LookupMulti")
                    {
                        this.UpdateFieldLookupListNames(nodesWantedField, nodeLists);
                    }

                    num++;
                    iUpdateFields++;
                    xmlTextWriter1.WriteStartElement("Method");
                    xmlTextWriter1.WriteAttributeString("ID", num.ToString());
                    xmlTextWriter1.WriteAttributeString("AddToView", "");
                    xmlTextWriter1.WriteRaw(nodesWantedField.OuterXml);
                    xmlTextWriter1.WriteEndElement();
                    break;
                }

                if (flag2)
                {
                    continue;
                }

                if (nodesWantedField.Attributes["AddToSiteColumnGroup"] != null)
                {
                    xmlNodes1.Add(nodesWantedField);
                }
                else if (nodesWantedField.Attributes["SourceID"] == null ||
                         !Utils.IsGuid(nodesWantedField.Attributes["SourceID"].Value))
                {
                    this.WriteAddField(nodesWantedField, xmlTextWriter, xmlTextWriter1, ref xmlNodes2, ref num,
                        ref iNewFields, ref iUpdateFields, nodeLists);
                }
                else
                {
                    xmlNodes.Add(nodesWantedField);
                }
            }

            if (xmlNodes1.Count > 0 || xmlNodes.Count > 0)
            {
                WebsService webServiceForWebs = this.GetWebServiceForWebs();
                XmlNode columns = webServiceForWebs.GetColumns();
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    XmlNode siteColumnXml = this.GetSiteColumnXml(xmlNode, columns);
                    this.WriteAddField(siteColumnXml, xmlTextWriter, xmlTextWriter1, ref xmlNodes2, ref num,
                        ref iNewFields, ref iUpdateFields, nodeLists);
                }

                foreach (XmlNode xmlNodes3 in this.AddListFieldsAsSiteColumns(xmlNodes1, webServiceForWebs))
                {
                    this.WriteAddField(xmlNodes3, xmlTextWriter, xmlTextWriter1, ref xmlNodes2, ref num, ref iNewFields,
                        ref iUpdateFields, nodeLists);
                }
            }

            foreach (XmlNode xmlNode1 in xmlNodes2)
            {
                num++;
                iUpdateFields++;
                xmlTextWriter1.WriteStartElement("Method");
                xmlTextWriter1.WriteAttributeString("ID", num.ToString());
                xmlTextWriter1.WriteAttributeString("AddToView", "");
                xmlTextWriter1.WriteRaw(xmlNode1.OuterXml);
                xmlTextWriter1.WriteEndElement();
            }

            foreach (string str in strs)
            {
                num++;
                iDeleteFields++;
                xmlTextWriter2.WriteStartElement("Method");
                xmlTextWriter2.WriteAttributeString("ID", num.ToString());
                xmlTextWriter2.WriteAttributeString("AddToView", "");
                xmlTextWriter2.WriteRaw(string.Concat("<Field Name=\"", str, "\" />"));
                xmlTextWriter2.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            newFields = XmlUtility.StringToXmlNode(stringWriter.ToString());
            xmlTextWriter1.WriteEndElement();
            xmlTextWriter1.Flush();
            xmlTextWriter1.Close();
            updateFields = XmlUtility.StringToXmlNode(stringWriter1.ToString());
            xmlTextWriter2.WriteEndElement();
            xmlTextWriter2.Flush();
            xmlTextWriter2.Close();
            deleteFields = XmlUtility.StringToXmlNode(stringWriter2.ToString());
        }

        private void WriteNavNodeXml(XmlWriter writer, int iNodeIdx, string[] lines,
            Dictionary<string, int> nodeIndexes, string[] globalNavHiddenUrls, string[] currentNavHiddenUrls,
            bool bOnGlobalNav, bool bOnCurrentNav)
        {
            string str;
            string str1;
            string str2;
            string str3;
            bool flag;
            bool flag1;
            Dictionary<string, string> strs;
            string[] strArrays;
            this.ReadNavNodeDataFromArray(iNodeIdx, lines, globalNavHiddenUrls, currentNavHiddenUrls, bOnGlobalNav,
                bOnCurrentNav, out str, out str1, out str2, out str3, out flag, out flag1, out strs, out strArrays);
            writer.WriteStartElement("NavNode");
            writer.WriteAttributeString("ID", str);
            writer.WriteAttributeString("Title", str1);
            writer.WriteAttributeString("Url", str2);
            writer.WriteAttributeString("IsVisible", flag.ToString());
            writer.WriteAttributeString("IsExternal", flag1.ToString());
            writer.WriteAttributeString("LastModified", str3);
            foreach (string key in strs.Keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                string str4 = XmlConvert.EncodeLocalName(key);
                if (str4 == "LastModifiedDate" || str4 == "CreatedDate")
                {
                    continue;
                }

                writer.WriteAttributeString(str4, strs[key]);
            }

            if (strArrays != null)
            {
                bool flag2 = (bOnGlobalNav ? true : str == "1002");
                bool flag3 = (bOnCurrentNav ? true : str == "1025");
                string[] strArrays1 = strArrays;
                for (int i = 0; i < (int)strArrays1.Length; i++)
                {
                    string str5 = strArrays1[i];
                    if (nodeIndexes.ContainsKey(str5))
                    {
                        int item = nodeIndexes[str5];
                        this.WriteNavNodeXml(writer, item, lines, nodeIndexes, globalNavHiddenUrls,
                            currentNavHiddenUrls, flag2, flag3);
                    }
                }
            }

            writer.WriteEndElement();
        }

        private void WriteVersionCell(XmlWriter xmlWriter, string sFieldName, string sFieldValue)
        {
            int num;
            if (sFieldName == "_Level")
            {
                xmlWriter.WriteAttributeString("_VersionLevel", sFieldValue);
            }
            else if (sFieldName == "_UIVersionString")
            {
                if (sFieldValue.IndexOf(".") < 0)
                {
                    sFieldValue = string.Concat(sFieldValue, ".0");
                }

                xmlWriter.WriteAttributeString("_VersionString", sFieldValue);
            }

            if (sFieldName == "_UIVersion" && !base.SharePointVersion.IsSharePoint2010OrLater)
            {
                if (base.SharePointVersion.IsSharePoint2003 && int.TryParse(sFieldValue, out num))
                {
                    sFieldValue = (num * 512).ToString();
                }

                xmlWriter.WriteAttributeString("_VersionNumber", sFieldValue);
            }

            xmlWriter.WriteAttributeString(sFieldName, sFieldValue);
        }

        public struct EmbeddedWebPart
        {
            private string m_sSourceID;

            private string m_sTargetID;

            private string m_sZoneID;

            public string SourceID
            {
                get { return this.m_sSourceID; }
                set { this.m_sSourceID = value; }
            }

            public string TargetID
            {
                get { return this.m_sTargetID; }
                set { this.m_sTargetID = value; }
            }

            public string ZoneID
            {
                get { return this.m_sZoneID; }
                set { this.m_sZoneID = value; }
            }
        }
    }
}