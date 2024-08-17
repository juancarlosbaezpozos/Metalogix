using Metalogix;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Migration
{
    public class LinkCorrector
    {
        private const string RootFolderParameter = "RootFolder";

        private Hashtable m_UrlMappings = new Hashtable();

        private Hashtable m_SiteCollectionMappings = new Hashtable();

        private Dictionary<string, string> m_GuidMappings = new Dictionary<string, string>();

        private LinkCorrectionScope m_Scope = LinkCorrectionScope.SiteCollection;

        private bool m_MappingsHaveBeenPrepopulated = false;

        private static List<LinkCorrector.HtmlLinkTypeNode> defaultHtmlSearchList;

        private bool IsSiteCollectionScoped
        {
            get
            {
                return this.m_Scope == LinkCorrectionScope.SiteCollection;
            }
        }

        public bool MappingsFullyGenerated
        {
            get
            {
                return this.m_MappingsHaveBeenPrepopulated;
            }
        }

        public LinkCorrectionScope Scope
        {
            get
            {
                return this.m_Scope;
            }
            set
            {
                this.m_Scope = value;
            }
        }

        public Hashtable SiteCollectionMappings
        {
            get
            {
                return this.m_SiteCollectionMappings;
            }
        }

        static LinkCorrector()
        {
            List<LinkCorrector.HtmlLinkTypeNode> htmlLinkTypeNodes = new List<LinkCorrector.HtmlLinkTypeNode>()
            {
                new LinkCorrector.HtmlLinkTypeNode("img", "src"),
                new LinkCorrector.HtmlLinkTypeNode("img", "href"),
                new LinkCorrector.HtmlLinkTypeNode("a", "href"),
                new LinkCorrector.HtmlLinkTypeNode("link", "href"),
                new LinkCorrector.HtmlLinkTypeNode("area", "href")
            };
            LinkCorrector.defaultHtmlSearchList = htmlLinkTypeNodes;
        }

        public LinkCorrector()
        {
        }

        public void AddFlattenedFolderMappings(SPListItem source, SPListItem target)
        {
            this.AddMapping(source.ServerRelativeFolderLeafRef, target.ServerRelativeFolderLeafRef);
            this.AddMapping(source.ParentFolder.ServerRelativeUrl, target.ParentFolder.ServerRelativeUrl);
        }

        private void AddFolderMappings(SPListItemCollection folders, TransformationTaskCollection renameTasks)
        {
            if ((folders == null || folders.Count <= 0 || renameTasks == null ? false : renameTasks.Count > 0))
            {
                foreach (SPListItem folder in folders)
                {
                    if (folder.ItemType == SPListItemType.Folder)
                    {
                        string transformationValue = renameTasks.GetTransformationValue(folder, "FileLeafRef", new CompareDatesInUtc());
                        if (!string.IsNullOrEmpty(transformationValue))
                        {
                            string str = this.CorrectUrl(folder.FileDirRef);
                            string str1 = Utils.JoinUrl(folder.Adapter.ServerUrl, folder.ServerRelativeUrl);
                            string str2 = this.CorrectUrl(str1);
                            string str3 = string.Concat("/", transformationValue);
                            string serverRelativeUrl = folder.ServerRelativeUrl;
                            char[] chrArray = new char[] { '/' };
                            this.AddMapping(serverRelativeUrl, string.Concat(str.Trim(chrArray), str3));
                            int num = str2.LastIndexOf(folder.FileLeafRef, StringComparison.OrdinalIgnoreCase);
                            if (num >= 0)
                            {
                                string str4 = str2.Remove(num);
                                chrArray = new char[] { '/' };
                                this.AddMapping(str1, string.Concat(str4.Trim(chrArray), str3));
                            }
                        }
                    }
                }
            }
        }

        public void AddGuidMapping(string sSourceGuid, string sTargetGuid)
        {
            char[] chrArray;
            if ((sSourceGuid == null ? true : sSourceGuid.Length <= 0))
            {
                throw new ArgumentException("", "sSourceGuid");
            }
            if ((sTargetGuid == null ? true : sTargetGuid.Length <= 0))
            {
                throw new ArgumentException("", "sTargetGuid");
            }
            string str = this.StandardizeGuid(sSourceGuid);
            if (this.HasGuidMapping(str))
            {
                Dictionary<string, string> mGuidMappings = this.m_GuidMappings;
                chrArray = new char[] { '{', '}' };
                mGuidMappings[str] = sTargetGuid.Trim(chrArray);
            }
            else
            {
                Dictionary<string, string> strs = this.m_GuidMappings;
                chrArray = new char[] { '{', '}' };
                strs.Add(str, sTargetGuid.Trim(chrArray));
            }
        }

        private void AddListItemMappings(SPListItemCollection sourceItems, SPFolder targetFolder)
        {
            if (targetFolder.Adapter.ServerUrl != null)
            {
                this.AddListItemMappings(sourceItems, string.Concat(this.StandardizeURL(targetFolder.Adapter.ServerUrl, false), "/", this.StandardizeURL(targetFolder.ServerRelativeUrl, false)), targetFolder.ServerRelativeUrl);
            }
        }

        private void AddListItemMappings(SPListItemCollection sourceItems, string sTargetFolderFullUrl, string sTargetFolderServerRelativeUrl)
        {
            foreach (SPListItem sourceItem in sourceItems)
            {
                string lastNodeInUrl = this.GetLastNodeInUrl(sourceItem.ServerRelativeUrl);
                if (sourceItem.Adapter.ServerUrl != null)
                {
                    string str = string.Concat(this.StandardizeURL(sourceItem.Adapter.ServerUrl, true), "/", this.StandardizeURL(sourceItem.ServerRelativeUrl, true));
                    string str1 = string.Concat(this.StandardizeURL(sTargetFolderFullUrl, false), "/", lastNodeInUrl);
                    this.AddMapping(str, str1);
                }
                string str2 = string.Concat(this.StandardizeURL(sTargetFolderServerRelativeUrl, false), "/", lastNodeInUrl);
                this.AddMapping(sourceItem.ServerRelativeUrl, str2);
            }
        }

        private void AddListMappings(SPList sourceList, string sRelativeTargetContainerUrl, string sFullTargetContainerUrl, TransformationTaskCollection renameTasks, bool bIncludeSubFolders)
        {
            char[] chrArray;
            if ((sourceList == null ? false : renameTasks != null))
            {
                string str = "";
                if (!sourceList.WebRelativeUrl.Equals(sourceList.Name, StringComparison.OrdinalIgnoreCase))
                {
                    string str1 = sourceList.WebRelativeUrl.Remove(sourceList.WebRelativeUrl.Length - sourceList.Name.Length);
                    chrArray = new char[] { '/' };
                    str = string.Concat(str1.Trim(chrArray), "/");
                }
                string transformationValue = renameTasks.GetTransformationValue(sourceList, "Name", new CompareDatesInUtc());
                if (string.IsNullOrEmpty(transformationValue))
                {
                    transformationValue = sourceList.Name;
                }
                string str2 = string.Concat("/", str, transformationValue);
                chrArray = new char[] { '/' };
                string str3 = string.Concat(sRelativeTargetContainerUrl.Trim(chrArray), str2);
                chrArray = new char[] { '/' };
                string str4 = string.Concat(sFullTargetContainerUrl.Trim(chrArray), str2);
                this.AddMapping(sourceList.ServerRelativeUrl, str3);
                this.AddMapping(sourceList.LinkableUrl, str4);
                if (bIncludeSubFolders)
                {
                    SPListItemCollection items = sourceList.GetItems(true, ListItemQueryType.Folder);
                    if (items != null)
                    {
                        this.AddFolderMappings(items, renameTasks);
                    }
                }
            }
        }

        public void AddMapping(string sSourceUrl, string sTargetUrl)
        {
            this.AddMapping(sSourceUrl, sTargetUrl, true);
        }

        public void AddMapping(string sSourceUrl, string sTargetUrl, bool bReplaceExistingMapping)
        {
            if ((string.IsNullOrEmpty(sSourceUrl) ? false : sTargetUrl != null))
            {
                string str = this.StandardizeURL(sSourceUrl, true);
                string str1 = this.StandardizeURL(sTargetUrl, false);
                if ((bReplaceExistingMapping || !this.HasMapping(str, true, false) ? false : !string.Equals(this.GetTargetMapping(str, true, false), str1, StringComparison.OrdinalIgnoreCase)))
                {
                    string[] strArrays = new string[] { "A link correction mapping for '", sSourceUrl, "' already exists and cannot be changed to '", sTargetUrl, "'. The existing target mapping is '", this.GetTargetMapping(str, true, false), "'." };
                    throw new ArgumentException(string.Concat(strArrays));
                }
                if ((!bReplaceExistingMapping ? false : this.HasMapping(str, true, false)))
                {
                    this.m_UrlMappings.Remove(str);
                }
                if (!this.HasMapping(str, true, false))
                {
                    this.m_UrlMappings.Add(str.ToLower(), str1);
                }
            }
        }

        public void AddMappingForPublishingPageLayoutCatalog(SPListItem sourcePublishingPage, SPFolder targetFolder)
        {
            this.AddSiteCollectionMappingsForWebs(sourcePublishingPage.ParentList.ParentWeb, targetFolder.ParentList.ParentWeb);
        }

        public void AddSiteCollectionMapping(string sSourceSiteCollUrl, string sTargetSiteCollUrl)
        {
            if ((string.IsNullOrEmpty(sSourceSiteCollUrl) ? false : sTargetSiteCollUrl != null))
            {
                string str = this.StandardizeURL(sSourceSiteCollUrl, true);
                string str1 = this.StandardizeURL(sTargetSiteCollUrl, false);
                if ((!this.HasMapping(str, false, true) ? false : !string.Equals(this.GetTargetMapping(str, false, true), str1, StringComparison.OrdinalIgnoreCase)))
                {
                    string[] strArrays = new string[] { "A site collection link correction mapping for '", sSourceSiteCollUrl, "' already exists and cannot be changed to '", sTargetSiteCollUrl, "'. The existing target mapping is '", this.GetTargetMapping(str, false, true), "'." };
                    throw new Exception(string.Concat(strArrays));
                }
                if (!this.HasMapping(str, false, true))
                {
                    this.m_SiteCollectionMappings.Add(str.ToLower(), str1);
                }
            }
        }

        private void AddSiteCollectionMappingsForLists(SPList sourceList, SPList targetList)
        {
            if ((sourceList == null || targetList == null || sourceList.ParentWeb == null ? false : targetList.ParentWeb != null))
            {
                this.AddSiteCollectionMappingsForWebs(sourceList.ParentWeb, targetList.ParentWeb);
            }
        }

        private void AddSiteCollectionMappingsForWebs(SPWeb sourceWeb, SPWeb targetWeb)
        {
            if ((sourceWeb == null ? false : targetWeb != null))
            {
                this.AddSiteCollectionMapping(sourceWeb.SiteCollectionServerRelativeUrl, targetWeb.SiteCollectionServerRelativeUrl);
                if (sourceWeb.Adapter.ServerUrl != null)
                {
                    string serverUrl = sourceWeb.Adapter.ServerUrl;
                    char[] chrArray = new char[] { '/' };
                    string str = serverUrl.TrimEnd(chrArray);
                    object obj = '/';
                    string siteCollectionServerRelativeUrl = sourceWeb.SiteCollectionServerRelativeUrl;
                    chrArray = new char[] { '/' };
                    string str1 = string.Concat(str, obj, siteCollectionServerRelativeUrl.TrimStart(chrArray));
                    string serverUrl1 = targetWeb.Adapter.ServerUrl;
                    chrArray = new char[] { '/' };
                    string str2 = serverUrl1.TrimEnd(chrArray);
                    object obj1 = '/';
                    string siteCollectionServerRelativeUrl1 = targetWeb.SiteCollectionServerRelativeUrl;
                    chrArray = new char[] { '/' };
                    this.AddSiteCollectionMapping(str1, string.Concat(str2, obj1, siteCollectionServerRelativeUrl1.TrimStart(chrArray)));
                }
            }
        }

        public void AddStringMapping(string sSourceString, string sTargetString)
        {
            if ((sSourceString == null ? true : sSourceString.Length <= 0))
            {
                throw new ArgumentException("", "sSourceString");
            }
            if ((sTargetString == null ? true : sTargetString.Length <= 0))
            {
                throw new ArgumentException("", "sTargetString");
            }
            if ((!this.HasStringMapping(sSourceString) ? false : this.GetStringMapping(sSourceString).ToLower() != sTargetString.ToLower()))
            {
                string[] strArrays = new string[] { "A string mapping already exists for the source string {", sSourceString, "} and cannot be changed to {", sTargetString, "}. The existing target string mapping is {", this.GetStringMapping(sSourceString), "}." };
                throw new Exception(string.Concat(strArrays));
            }
            if (!this.HasStringMapping(sTargetString))
            {
                this.m_GuidMappings.Add(sSourceString, sTargetString);
            }
        }

        public void AddUserSpecifiedMapping(string sSourceUrl, string sTargetUrl, SharePointAdapter sourceAdapter, SharePointAdapter targetAdapter)
        {
            if (!this.HasMapping(sSourceUrl, false, false))
            {
                this.AddMapping(sSourceUrl, sTargetUrl);
                if ((sourceAdapter == null || targetAdapter == null || string.IsNullOrEmpty(sourceAdapter.ServerUrl) || string.IsNullOrEmpty(targetAdapter.ServerUrl) || !this.FromSameServer(sSourceUrl, sourceAdapter.ServerUrl) ? false : this.FromSameServer(sTargetUrl, targetAdapter.ServerUrl)))
                {
                    string serverRelativeUrlPart = Utils.GetServerRelativeUrlPart(sSourceUrl);
                    string str = Utils.GetServerRelativeUrlPart(sTargetUrl);
                    if (!this.HasMapping(serverRelativeUrlPart, false, true))
                    {
                        this.AddMapping(serverRelativeUrlPart, str);
                    }
                }
            }
        }

        public void AddWebMappings(SPWeb sourceWeb, SPWeb targetWeb)
        {
            if (sourceWeb.Adapter.ServerUrl != null)
            {
                string str = string.Concat(this.StandardizeURL(sourceWeb.Adapter.ServerUrl, true), "/", this.StandardizeURL(sourceWeb.ServerRelativeUrl, true));
                string str1 = string.Concat(this.StandardizeURL(targetWeb.Adapter.ServerUrl, false), "/", this.StandardizeURL(targetWeb.ServerRelativeUrl, false));
                this.AddMapping(str, str1);
            }
            this.AddMapping(sourceWeb.ServerRelativeUrl, targetWeb.ServerRelativeUrl);
        }

        private void AddWebMappings(SPWeb sourceWeb, string sTargetContainerFullUrl, string sTargetContainerRelativeUrl, TransformationTaskCollection renameTasks, bool bRecursive)
        {
            if ((sourceWeb == null || string.IsNullOrEmpty(sTargetContainerFullUrl) ? false : !string.IsNullOrEmpty(sTargetContainerRelativeUrl)))
            {
                string targetWebName = this.GetTargetWebName(sourceWeb, renameTasks);
                char[] chrArray = new char[] { '/' };
                string str = string.Concat(sTargetContainerFullUrl.TrimEnd(chrArray), "/", targetWebName);
                this.AddMapping(sourceWeb.LinkableUrl, str);
                chrArray = new char[] { '/' };
                string str1 = string.Concat(sTargetContainerRelativeUrl.Trim(chrArray), "/", targetWebName);
                this.AddMapping(sourceWeb.ServerRelativeUrl, str1);
                if (bRecursive)
                {
                    if ((renameTasks == null ? false : renameTasks.Count > 0))
                    {
                        foreach (SPList list in sourceWeb.Lists)
                        {
                            this.AddListMappings(list, str1, str, renameTasks, true);
                        }
                    }
                    foreach (SPWeb subWeb in sourceWeb.SubWebs)
                    {
                        this.AddWebMappings(subWeb, str, str1, renameTasks, true);
                    }
                }
            }
        }

        private string AttemptToCorrectRootFolderParameter(bool checkWebLevelMappings, bool checkSiteCollectionLevelMappings, string standardizedUrl)
        {
            Uri uri;
            StringBuilder stringBuilder = null;
            string str = HttpUtility.UrlDecode(standardizedUrl);
            if (str.Contains("RootFolder"))
            {
                stringBuilder = new StringBuilder(str);
                string str1 = null;
                if (!Uri.TryCreate(str, UriKind.Absolute, out uri))
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(str);
                    foreach (string key in nameValueCollection.Keys)
                    {
                        if ((string.IsNullOrEmpty(key) ? false : key.EndsWith("RootFolder")))
                        {
                            str1 = nameValueCollection.Get(key);
                            break;
                        }
                    }
                }
                else
                {
                    str1 = HttpUtility.ParseQueryString(uri.Query).Get("RootFolder");
                }
                if (!string.IsNullOrEmpty(str1))
                {
                    int num = str1.LastIndexOf("/", StringComparison.Ordinal);
                    while (num >= 0)
                    {
                        string str2 = str1.Substring(0, num);
                        string targetMapping = this.GetTargetMapping(str2, checkWebLevelMappings, checkSiteCollectionLevelMappings);
                        if (targetMapping == null)
                        {
                            num = str2.LastIndexOf("/", StringComparison.Ordinal);
                        }
                        else
                        {
                            string str3 = string.Concat(targetMapping, str1.Substring(num));
                            if ((!str1.StartsWith("/") ? false : !str3.StartsWith("/")))
                            {
                                str3 = string.Concat("/", str3);
                            }
                            stringBuilder.Replace(string.Format("{0}={1}", "RootFolder", str1), string.Format("{0}={1}", "RootFolder", str3));
                            break;
                        }
                    }
                }
            }
            return (stringBuilder != null ? stringBuilder.ToString() : str);
        }

        public string AttemptToRemoveRootFolderRelatedParameters(string url)
        {
            bool flag;
            if (url == null)
            {
                throw new ArgumentNullException("url", "cannot be null");
            }
            StringBuilder stringBuilder = null;
            if (!url.Contains("RootFolder"))
            {
                flag = true;
            }
            else
            {
                flag = (url.Contains("amp;FolderCTID") ? false : !url.Contains("amp;View"));
            }
            if (!flag)
            {
                stringBuilder = new StringBuilder(url);
                NameValueCollection nameValueCollection = null;
                string str = null;
                nameValueCollection = HttpUtility.ParseQueryString(url);
                str = nameValueCollection.Get("amp;View");
                if (str != null)
                {
                    stringBuilder.Replace(string.Concat("amp;View=", str), string.Empty);
                }
                str = nameValueCollection.Get("amp;FolderCTID");
                if (str != null)
                {
                    stringBuilder.Replace(string.Concat("amp;FolderCTID=", str), string.Empty);
                }
            }
            return (stringBuilder != null ? stringBuilder.ToString() : url);
        }

        public void ClearMappings()
        {
            this.m_UrlMappings.Clear();
            this.m_GuidMappings.Clear();
            this.m_SiteCollectionMappings.Clear();
            this.m_MappingsHaveBeenPrepopulated = false;
        }

        public string CorrectDelimitedString(string sStringToCorrect, string sDelimitor)
        {
            return this.UpdateLinksInDelimitedString(sStringToCorrect, sDelimitor, false);
        }

        public string CorrectHtml(string sHtml)
        {
            return this.UpdateHtmlLinks(sHtml);
        }

        public static string CorrectOutOfScopeUrl(string sSourceUrl, SPWeb sourceWeb, SPWeb targetWeb)
        {
            string serverRelative;
            string full;
            string str;
            string serverRelative1;
            string full1;
            string str1;
            char[] chrArray;
            bool flag = sSourceUrl.Contains("://");
            if ((flag ? true : sSourceUrl.StartsWith("/")))
            {
                StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, sSourceUrl);
                StandardizedUrl standardizedUrl1 = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, sourceWeb.ServerRelativeUrl);
                StandardizedUrl standardizedUrl2 = StandardizedUrl.StandardizeUrl(sourceWeb.RootWeb.Adapter, sourceWeb.RootWeb.ServerRelativeUrl);
                StandardizedUrl standardizedUrl3 = StandardizedUrl.StandardizeUrl(targetWeb.Adapter, targetWeb.ServerRelativeUrl);
                StandardizedUrl standardizedUrl4 = StandardizedUrl.StandardizeUrl(targetWeb.RootWeb.Adapter, targetWeb.RootWeb.ServerRelativeUrl);
                if (!flag)
                {
                    serverRelative = standardizedUrl.ServerRelative;
                    full = standardizedUrl1.ServerRelative;
                    str = standardizedUrl2.ServerRelative;
                    serverRelative1 = standardizedUrl3.ServerRelative;
                    full1 = standardizedUrl4.ServerRelative;
                }
                else
                {
                    serverRelative = standardizedUrl.Full;
                    full = standardizedUrl1.Full;
                    str = standardizedUrl2.Full;
                    serverRelative1 = standardizedUrl3.Full;
                    full1 = standardizedUrl4.Full;
                }
                if (LinkCorrector.UrlStartsWithUrl(serverRelative, full))
                {
                    chrArray = new char[] { '/' };
                    string str2 = serverRelative1.TrimEnd(chrArray);
                    string str3 = serverRelative.Remove(0, full.Length);
                    chrArray = new char[] { '/' };
                    str1 = string.Concat(str2, "/", str3.TrimStart(chrArray));
                }
                else if (!LinkCorrector.UrlStartsWithUrl(serverRelative, str))
                {
                    if (flag)
                    {
                        string str4 = UrlUtils.StandardizeFormat(sourceWeb.Adapter.ServerUrl);
                        if (LinkCorrector.UrlStartsWithUrl(serverRelative, str4))
                        {
                            string str5 = UrlUtils.StandardizeFormat(sourceWeb.Adapter.ServerUrl);
                            chrArray = new char[] { '/' };
                            string str6 = str5.TrimEnd(chrArray);
                            string str7 = serverRelative.Remove(0, str4.Length);
                            chrArray = new char[] { '/' };
                            str1 = string.Concat(str6, "/", str7.TrimStart(chrArray));
                            return str1;
                        }
                    }
                    str1 = sSourceUrl;
                }
                else
                {
                    chrArray = new char[] { '/' };
                    string str8 = full1.TrimEnd(chrArray);
                    string str9 = serverRelative.Remove(0, str.Length);
                    chrArray = new char[] { '/' };
                    str1 = string.Concat(str8, "/", str9.TrimStart(chrArray));
                }
            }
            else
            {
                str1 = sSourceUrl;
            }
            return str1;
        }

        public string CorrectSiteCollectionUrl(string sUrl)
        {
            string str = this.CorrectUrlCore(sUrl, false, true);
            if (str.Equals(sUrl, StringComparison.OrdinalIgnoreCase))
            {
                if ((!sUrl.StartsWith("/") ? false : this.HasMapping("", false, true)))
                {
                    string targetMapping = this.GetTargetMapping("", false, true);
                    char[] chrArray = new char[] { '/' };
                    string str1 = string.Concat(targetMapping.Trim(chrArray), sUrl);
                    chrArray = new char[] { '/' };
                    str = string.Concat("/", str1.TrimStart(chrArray));
                }
            }
            return str;
        }

        public string CorrectText(string propertyValue)
        {
            string str;
            string str1 = "link";
            string[] strArrays = new string[] { "(<((?<!>).)*>)" };
            string[] strArrays1 = strArrays;
            string str2 = null;
            string[] strArrays2 = strArrays1;
            for (int i = 0; i < (int)strArrays2.Length; i++)
            {
                str2 = string.Concat(str2, strArrays2[i]);
            }
            str2 = string.Concat("(?<tagMatches>", str2, ")");
            strArrays = new string[] { "(?<", str1, ">http://(", str2, "|[^\\\\\\*\\?<>\\|#\\{\\}&\\+\\s])*)" };
            Regex regex = new Regex(string.Concat(strArrays), RegexOptions.IgnoreCase);
            MatchCollection matchCollections = regex.Matches(propertyValue);
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;
            for (int j = 0; j < matchCollections.Count; j++)
            {
                Match item = matchCollections[j];
                Group group = item.Groups[str1];
                Group item1 = item.Groups["tagMatches"];
                if (group.Success)
                {
                    string value = group.Value;
                    string str3 = "";
                    foreach (Capture capture in item1.Captures)
                    {
                        value = value.Replace(capture.Value, "");
                        str3 = string.Concat(str3, capture.Value);
                    }
                    stringBuilder.Append(propertyValue.Substring(index, group.Index - index));
                    stringBuilder.Append(this.CorrectUrl(value));
                    stringBuilder.Append(str3);
                    index = group.Index + group.Length;
                }
            }
            stringBuilder.Append(propertyValue.Substring(index));
            str = (stringBuilder.Length <= 0 ? propertyValue : stringBuilder.ToString());
            return str;
        }

        public string CorrectUrl(string sUrl)
        {
            return this.CorrectUrlCore(sUrl, true, this.IsSiteCollectionScoped);
        }

        public string CorrectUrl(string sUrl, SPWeb sourceWeb, SPWeb targetWeb)
        {
            string str;
            char[] chrArray;
            if (string.IsNullOrEmpty(sUrl))
            {
                str = sUrl;
            }
            else if (!(sUrl.Contains("://") ? false : !sUrl.StartsWith("/")))
            {
                str = this.CorrectUrl(sUrl);
            }
            else if ((sourceWeb == null ? false : targetWeb != null))
            {
                string str1 = Utils.JoinUrl(sourceWeb.ServerRelativeUrl, sUrl);
                if (!str1.StartsWith("/"))
                {
                    str1 = string.Concat("/", str1);
                }
                string str2 = this.CorrectUrl(str1);
                string str3 = (string.IsNullOrEmpty(targetWeb.ServerRelativeUrl) ? "/" : string.Concat("/", this.StandardizeURL(targetWeb.ServerRelativeUrl, false)));
                if (!(string.IsNullOrEmpty(str3) ? false : !(str3 == "/")))
                {
                    chrArray = new char[] { '/' };
                    str = str2.TrimStart(chrArray);
                }
                else if (!str2.StartsWith(str3))
                {
                    str = str2;
                }
                else
                {
                    string str4 = str2.Remove(0, str3.Length);
                    chrArray = new char[] { '/' };
                    str = str4.TrimStart(chrArray);
                }
            }
            else
            {
                str = sUrl;
            }
            return str;
        }

        private string CorrectUrlCore(string url, bool checkWebLevelMappings, bool checkSiteCollectionLevelMappings)
        {
            string str;
            string str1 = url;
            if (url == null)
            {
                throw new Exception("A null URL cannot be link corrected.");
            }
            if (!url.Trim().Equals(string.Empty))
            {
                bool flag = url.StartsWith("/");
                string targetMapping = this.GetTargetMapping(url, checkWebLevelMappings, checkSiteCollectionLevelMappings);
                if (targetMapping == null)
                {
                    string correctRootFolderParameter = this.StandardizeURL(url, false);
                    if ((!flag ? false : UrlUtils.GetType(correctRootFolderParameter) != UrlType.Full))
                    {
                        char[] chrArray = new char[] { '/' };
                        correctRootFolderParameter = string.Concat("/", correctRootFolderParameter.TrimStart(chrArray));
                    }
                    correctRootFolderParameter = this.AttemptToCorrectRootFolderParameter(checkWebLevelMappings, checkSiteCollectionLevelMappings, correctRootFolderParameter);
                    int num = correctRootFolderParameter.LastIndexOf("/", StringComparison.Ordinal);
                    while (num >= 0)
                    {
                        string str2 = correctRootFolderParameter.Substring(0, num);
                        targetMapping = this.GetTargetMapping(str2, checkWebLevelMappings, checkSiteCollectionLevelMappings);
                        if (targetMapping == null)
                        {
                            num = str2.LastIndexOf("/", StringComparison.Ordinal);
                        }
                        else
                        {
                            str1 = string.Concat(targetMapping, correctRootFolderParameter.Substring(num));
                            break;
                        }
                    }
                }
                else
                {
                    str1 = targetMapping;
                }
                if ((!flag ? false : !str1.StartsWith("/")))
                {
                    str1 = string.Concat("/", str1);
                }
                str = str1;
            }
            else
            {
                str = url;
            }
            return str;
        }

        public string CorrectWebUrlByRelativePosition(string sWebUrl, string sReferenceWebUrl)
        {
            string str;
            char[] chrArray;
            bool flag;
            string str1;
            if ((string.IsNullOrEmpty(sWebUrl) || string.IsNullOrEmpty(sReferenceWebUrl) || !this.HasMapping(sReferenceWebUrl, true, true) ? false : sWebUrl.StartsWith("/") == sReferenceWebUrl.StartsWith("/")))
            {
                string str2 = null;
                string str3 = null;
                string str4 = null;
                if (!Utils.GetCommonPathPrefix(sWebUrl, sReferenceWebUrl, out str3, out str4).EndsWith("://"))
                {
                    int length = 0;
                    if (string.IsNullOrEmpty(str4))
                    {
                        length = 0;
                    }
                    else if (str4.Contains("/"))
                    {
                        chrArray = new char[] { '/' };
                        string str5 = str4.Trim(chrArray);
                        chrArray = new char[] { '/' };
                        length = (int)str5.Split(chrArray).Length;
                    }
                    else
                    {
                        length = 1;
                    }
                    string str6 = this.CorrectUrl(sReferenceWebUrl);
                    string str7 = null;
                    int num = str6.IndexOf("://");
                    if (num >= 0)
                    {
                        num += "://".Length;
                        str7 = str6.Substring(0, num);
                        str6 = str6.Substring(num);
                    }
                    chrArray = new char[] { '/' };
                    string str8 = str6.Trim(chrArray);
                    chrArray = new char[] { '/' };
                    string[] strArrays = str8.Split(chrArray);
                    if ((int)strArrays.Length < length)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = (string.IsNullOrEmpty(str7) ? true : (int)strArrays.Length > length);
                    }
                    if (flag)
                    {
                        StringBuilder stringBuilder = new StringBuilder(256);
                        stringBuilder.Append("/");
                        for (int i = 0; i < (int)strArrays.Length - length; i++)
                        {
                            stringBuilder.Append(string.Concat(strArrays[i], "/"));
                        }
                        chrArray = new char[] { '/' };
                        stringBuilder.Append(str3.Trim(chrArray));
                        if (str7 == null)
                        {
                            if (stringBuilder.ToString().Equals("/"))
                            {
                                str1 = stringBuilder.ToString();
                            }
                            else
                            {
                                string str9 = stringBuilder.ToString();
                                chrArray = new char[] { '/' };
                                str1 = str9.TrimEnd(chrArray);
                            }
                            str2 = str1;
                        }
                        else
                        {
                            string str10 = stringBuilder.ToString();
                            chrArray = new char[] { '/' };
                            str2 = string.Concat(str7, str10.Trim(chrArray));
                        }
                    }
                    else
                    {
                        str2 = null;
                    }
                    str = str2;
                }
                else
                {
                    str = null;
                }
            }
            else
            {
                str = null;
            }
            return str;
        }

        private string CreateCommaSafeSharePointURL(string commaUnsafeURL)
        {
            return commaUnsafeURL.Replace(", ", ",, ");
        }

        public string DecodeURL(string sUrl)
        {
            string str;
            if (sUrl != null)
            {
                sUrl = sUrl.Trim(new char[] { '/' });
                str = WebUtils.UrlPathDecode(sUrl);
            }
            else
            {
                str = "";
            }
            return str;
        }

        private bool FromSameServer(string sFirstUrl, string sSecondUrl)
        {
            if ((string.IsNullOrEmpty(sFirstUrl) ? true : string.IsNullOrEmpty(sSecondUrl)))
            {
                throw new Exception("Cannot compare server sources of null or empty strings.");
            }
            int num = sFirstUrl.IndexOf("://");
            int num1 = sSecondUrl.IndexOf("://");
            if ((num < 0 ? true : num1 < 0))
            {
                throw new Exception("Cannot compare server source of relative urls.");
            }
            num = sFirstUrl.IndexOf('/', num + 3);
            string str = (num < 0 ? sFirstUrl : sFirstUrl.Substring(0, num));
            num1 = sSecondUrl.IndexOf('/', num1 + 3);
            string str1 = (num1 < 0 ? sSecondUrl : sSecondUrl.Substring(0, num1));
            bool flag = UrlUtils.StandardizeFormat(str).Equals(UrlUtils.StandardizeFormat(str1));
            return flag;
        }

        private string GetGuidMapping(string sSourceGuid)
        {
            string item;
            string str = this.StandardizeGuid(sSourceGuid);
            if (!this.HasGuidMapping(str))
            {
                item = null;
            }
            else
            {
                item = this.m_GuidMappings[str];
            }
            return item;
        }

        private string GetLastNodeInUrl(string sUrl)
        {
            string str = null;
            if (sUrl.EndsWith("/"))
            {
                sUrl = sUrl.Substring(0, sUrl.Length - 1);
            }
            int num = sUrl.LastIndexOf("/");
            str = (num < 0 ? sUrl : sUrl.Substring(num + 1));
            return str;
        }
        
        public CommonSerializableTable<string, string> GetMappings(bool bIncludeWebLevelMappings, bool bIncludeSiteCollectionLevelMappings)
        {
            CommonSerializableTable<string, string> commonSerializableTable = new CommonSerializableTable<string, string>();
            if (bIncludeWebLevelMappings)
            {
                foreach (object current in this.m_UrlMappings.Keys)
                {
                    string text = current.ToString();
                    if (UrlUtils.GetType(text) == UrlType.Full && !commonSerializableTable.ContainsKey(text))
                    {
                        commonSerializableTable.Add(text, this.m_UrlMappings[current].ToString());
                    }
                }
            }
            if (bIncludeSiteCollectionLevelMappings)
            {
                foreach (object current in this.m_SiteCollectionMappings.Keys)
                {
                    string text = current.ToString();
                    if (UrlUtils.GetType(text) == UrlType.Full && !commonSerializableTable.ContainsKey(text))
                    {
                        commonSerializableTable.Add(text, this.m_SiteCollectionMappings[current].ToString());
                    }
                }
            }
            return commonSerializableTable;
        }

        private string GetStringMapping(string sSourceString)
        {
            string item;
            if (!this.HasStringMapping(sSourceString))
            {
                item = null;
            }
            else
            {
                item = this.m_GuidMappings[sSourceString];
            }
            return item;
        }

        private string GetTargetMapping(string sSourceUrl, bool bCheckWebLevelMappings, bool bCheckSiteCollectionLevelMappings)
        {
            string item = null;
            string str = this.StandardizeURL(sSourceUrl, true);
            if (this.HasMapping(str, bCheckWebLevelMappings, bCheckSiteCollectionLevelMappings))
            {
                if (bCheckWebLevelMappings)
                {
                    item = (string)this.m_UrlMappings[str];
                }
                if ((item != null ? false : bCheckSiteCollectionLevelMappings))
                {
                    item = (string)this.m_SiteCollectionMappings[str];
                }
            }
            return item;
        }

        private string GetTargetWebName(SPWeb sourceWeb, TransformationTaskCollection renameTasks)
        {
            string transformationValue;
            if (renameTasks != null)
            {
                transformationValue = renameTasks.GetTransformationValue(sourceWeb, "Name", new CompareDatesInUtc());
            }
            else
            {
                transformationValue = null;
            }
            string str = transformationValue;
            if (str == null)
            {
                str = (!string.IsNullOrEmpty(sourceWeb.WebName) ? sourceWeb.WebName : sourceWeb.Title);
            }
            return str;
        }

        private bool HasGuidMapping(string sSourceGuid)
        {
            return this.m_GuidMappings.ContainsKey(this.StandardizeGuid(sSourceGuid));
        }

        private bool HasMapping(string sSourceUrl, bool bCheckWebLevelMappings, bool bCheckSiteCollectionLevelMappings)
        {
            bool flag;
            string str = this.StandardizeURL(sSourceUrl, true);
            if (!bCheckWebLevelMappings || !this.m_UrlMappings.Contains(str))
            {
                flag = (!bCheckSiteCollectionLevelMappings ? false : this.m_SiteCollectionMappings.Contains(str));
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        private bool HasStringMapping(string sSourceString)
        {
            return this.m_GuidMappings.ContainsKey(sSourceString);
        }

        public string MapGuid(string sSourceGuid)
        {
            string guidMapping;
            if (sSourceGuid == null)
            {
                throw new ArgumentException("Cannot map a null GUID.", "sSourceGuid");
            }
            if (!this.HasGuidMapping(sSourceGuid))
            {
                guidMapping = null;
            }
            else
            {
                guidMapping = this.GetGuidMapping(sSourceGuid);
            }
            return guidMapping;
        }

        public string MapString(string sSourceString)
        {
            string item;
            if (sSourceString == null)
            {
                throw new ArgumentException("Cannot map a null string.", "sSourceString");
            }
            if (!this.HasStringMapping(sSourceString))
            {
                item = null;
            }
            else
            {
                item = this.m_GuidMappings[sSourceString];
            }
            return item;
        }

        public void PopulateForFolderCopy(SPFolder sourceFolder, SPFolder targetFolder, TransformationTaskCollection renameTasks, bool bPrepopulateMappings)
        {
            if ((sourceFolder == null || targetFolder == null || sourceFolder.ParentList == null ? false : targetFolder.ParentList != null))
            {
                if ((!bPrepopulateMappings ? true : !this.m_MappingsHaveBeenPrepopulated))
                {
                    this.PopulateForListCopy(sourceFolder.ParentList, targetFolder.ParentList, renameTasks, bPrepopulateMappings);
                }
            }
        }

        public void PopulateForItemCopy(SPListItemCollection sourceItems, SPFolder targetFolder)
        {
            if ((sourceItems == null ? false : targetFolder != null))
            {
                if (this.IsSiteCollectionScoped)
                {
                    this.AddSiteCollectionMappingsForLists(sourceItems.ParentSPList, targetFolder.ParentList);
                }
                if ((sourceItems.ParentSPList == null ? false : targetFolder.ParentList != null))
                {
                    this.PopulateForListCopy(sourceItems.ParentSPList, targetFolder.ParentList);
                }
            }
        }

        public void PopulateForListCopy(SPList sourceList, SPList targetList, TransformationTaskCollection renameTasks, bool bPrepopulateMappings)
        {
            this.PopulateForListCopy(sourceList, targetList);
            if ((!bPrepopulateMappings || this.m_MappingsHaveBeenPrepopulated ? false : renameTasks != null))
            {
                SPListItemCollection items = sourceList.GetItems(true, ListItemQueryType.Folder);
                if (items != null)
                {
                    this.AddFolderMappings(items, renameTasks);
                }
                this.m_MappingsHaveBeenPrepopulated = true;
            }
        }

        public void PopulateForListCopy(SPList sourceList, SPList targetList)
        {
            if ((sourceList == null ? false : targetList != null))
            {
                if ((!this.IsSiteCollectionScoped || sourceList.ParentWeb == null ? false : targetList.ParentWeb != null))
                {
                    this.AddSiteCollectionMappingsForWebs(sourceList.ParentWeb, targetList.ParentWeb);
                }
                if (sourceList.BaseTemplate == ListTemplateType.O12Pages)
                {
                    this.AddWebMappings(sourceList.ParentWeb, targetList.ParentWeb);
                }
                string str = this.StandardizeURL(sourceList.ServerRelativeUrl, true);
                string str1 = this.StandardizeURL(targetList.ServerRelativeUrl, false);
                if ((string.IsNullOrEmpty(sourceList.Adapter.ServerUrl) ? false : !string.IsNullOrEmpty(targetList.Adapter.ServerUrl)))
                {
                    this.AddMapping(string.Concat(this.StandardizeURL(sourceList.Adapter.ServerUrl, true), "/", str), string.Concat(this.StandardizeURL(targetList.Adapter.ServerUrl, false), "/", str1), true);
                }
                this.AddMapping(str, str1, true);
                this.AddGuidMapping(sourceList.ID, targetList.ID);
            }
        }

        public void PopulateForSiteCollectionCopy(SPWeb sourceWeb, SPWebApplication targetWebApp, string sSiteCollectionWebAppRelativeUrl, TransformationTaskCollection renameTasks, bool bPrepopulateMappings, string hostHeaderURL = null)
        {
            if ((sourceWeb == null ? false : !(targetWebApp == null)))
            {
                string str = null;
                if (sourceWeb.Adapter.ServerUrl != null)
                {
                    string str1 = this.StandardizeURL(sourceWeb.Adapter.ServerUrl, true);
                    if (!string.IsNullOrEmpty(this.StandardizeURL(sourceWeb.ServerRelativeUrl, true)))
                    {
                        str1 = string.Concat(str1, "/", this.StandardizeURL(sourceWeb.ServerRelativeUrl, true));
                    }
                    if (!string.IsNullOrEmpty(hostHeaderURL))
                    {
                        str = this.StandardizeURL(hostHeaderURL, false);
                    }
                    else
                    {
                        str = this.StandardizeURL(targetWebApp.Url, false);
                        if (!string.IsNullOrEmpty(this.StandardizeURL(sSiteCollectionWebAppRelativeUrl, false)))
                        {
                            str = string.Concat(str, "/", this.StandardizeURL(sSiteCollectionWebAppRelativeUrl, false));
                        }
                    }
                    this.AddSiteCollectionMapping(str1, str);
                    this.AddMapping(str1, str);
                }
                this.AddSiteCollectionMapping(sourceWeb.ServerRelativeUrl, sSiteCollectionWebAppRelativeUrl);
                this.AddMapping(sourceWeb.ServerRelativeUrl, sSiteCollectionWebAppRelativeUrl);
                if ((!bPrepopulateMappings ? false : !this.m_MappingsHaveBeenPrepopulated))
                {
                    foreach (SPWeb subWeb in sourceWeb.SubWebs)
                    {
                        this.AddWebMappings(subWeb, str, sSiteCollectionWebAppRelativeUrl, renameTasks, bPrepopulateMappings);
                    }
                    this.m_MappingsHaveBeenPrepopulated = true;
                }
            }
        }

        public void PopulateForSiteContentCopy(SPWeb sourceWeb, SPWeb targetContainer, TransformationTaskCollection renameTasks, bool bPrepopulateMappings)
        {
            if ((sourceWeb == null ? false : targetContainer != null))
            {
                if ((this.IsSiteCollectionScoped ? true : sourceWeb.IsPublishingTemplate))
                {
                    this.AddSiteCollectionMappingsForWebs(sourceWeb, targetContainer);
                }
                if ((!bPrepopulateMappings ? true : !this.m_MappingsHaveBeenPrepopulated))
                {
                    string serverRelativeUrl = targetContainer.ServerRelativeUrl;
                    char[] chrArray = new char[] { '/' };
                    string str = serverRelativeUrl.TrimEnd(chrArray);
                    string serverUrl = targetContainer.Adapter.ServerUrl;
                    chrArray = new char[] { '/' };
                    string str1 = serverUrl.TrimEnd(chrArray);
                    chrArray = new char[] { '/' };
                    string str2 = string.Concat(str1, "/", str.Trim(chrArray));
                    this.AddMapping(sourceWeb.LinkableUrl, str2);
                    this.AddMapping(sourceWeb.ServerRelativeUrl, str);
                    if (bPrepopulateMappings)
                    {
                        if ((renameTasks == null ? false : renameTasks.Count > 0))
                        {
                            foreach (SPList list in sourceWeb.Lists)
                            {
                                this.AddListMappings(list, str, str2, renameTasks, true);
                            }
                        }
                        foreach (SPWeb subWeb in sourceWeb.SubWebs)
                        {
                            this.AddWebMappings(subWeb, str2, str, renameTasks, true);
                        }
                    }
                    this.m_MappingsHaveBeenPrepopulated = bPrepopulateMappings;
                }
            }
        }

        public void PopulateForSiteCopy(SPWeb sourceWeb, SPWeb targetContainer, TransformationTaskCollection renameTasks, bool bPrepopulateMappings)
        {
            char[] chrArray;
            string serverRelativeUrl;
            if ((sourceWeb == null ? false : targetContainer != null))
            {
                if ((this.IsSiteCollectionScoped ? true : sourceWeb.IsPublishingTemplate))
                {
                    this.AddSiteCollectionMappingsForWebs(sourceWeb, targetContainer);
                }
                if ((!bPrepopulateMappings ? true : !this.m_MappingsHaveBeenPrepopulated))
                {
                    if (targetContainer.ServerRelativeUrl == "/")
                    {
                        serverRelativeUrl = targetContainer.ServerRelativeUrl;
                    }
                    else
                    {
                        string str = targetContainer.ServerRelativeUrl;
                        chrArray = new char[] { '/' };
                        serverRelativeUrl = str.TrimEnd(chrArray);
                    }
                    string str1 = serverRelativeUrl;
                    string serverUrl = targetContainer.Adapter.ServerUrl;
                    chrArray = new char[] { '/' };
                    string str2 = serverUrl.TrimEnd(chrArray);
                    chrArray = new char[] { '/' };
                    string str3 = string.Concat(str2, "/", str1.Trim(chrArray));
                    this.AddWebMappings(sourceWeb, str3, str1, renameTasks, (bPrepopulateMappings ? true : false));
                    this.m_MappingsHaveBeenPrepopulated = bPrepopulateMappings;
                }
            }
        }

        private string RemoveCommaSafetyFromSharePointURL(string commaSafeURL)
        {
            return commaSafeURL.Replace(",, ", ", ");
        }

        public void RemoveGuidMapping(string sSourceGuid)
        {
            string str = this.StandardizeGuid(sSourceGuid);
            if (this.HasGuidMapping(str))
            {
                this.m_GuidMappings.Remove(str);
            }
        }

        public void RemoveMapping(string sSourceUrl)
        {
            string str = this.StandardizeURL(sSourceUrl, true);
            if (this.HasMapping(str, true, false))
            {
                this.m_UrlMappings.Remove(str);
            }
        }

        public void RemoveStringMapping(string sSourceString)
        {
            if (this.HasGuidMapping(sSourceString))
            {
                this.m_GuidMappings.Remove(sSourceString);
            }
        }

        private void SeparateSharePointURL(string sourceField, out string fieldURL, out string fieldDescription)
        {
            string str;
            Match match = (new Regex("[^,], ")).Match(sourceField);
            if (!match.Success)
            {
                fieldURL = sourceField;
                fieldDescription = null;
            }
            else
            {
                int index = match.Index + 1;
                fieldURL = sourceField.Substring(0, index);
                int num = index + 2;
                if (num < sourceField.Length)
                {
                    str = sourceField.Substring(num, sourceField.Length - num);
                }
                else
                {
                    str = null;
                }
                fieldDescription = str;
            }
        }

        private string StandardizeGuid(string sGuid)
        {
            string lower;
            if (sGuid != null)
            {
                string str = sGuid.Replace("-", "");
                char[] chrArray = new char[] { '{', '}' };
                lower = str.Trim(chrArray).ToLower();
            }
            else
            {
                lower = "";
            }
            return lower;
        }

        public string StandardizeURL(string sUrl, bool bRemoveCase)
        {
            string str;
            if (sUrl != null)
            {
                sUrl = sUrl.Trim(new char[] { '/' });
                if (bRemoveCase)
                {
                    sUrl = sUrl.ToLower();
                }
                str = HttpUtility.UrlPathEncode(WebUtils.UrlPathDecode(sUrl));
            }
            else
            {
                str = "";
            }
            return str;
        }

        private void Update2010PublishingFields(XmlNode ndItemXml)
        {
            if (ndItemXml.Attributes["PublishingPageImage"] != null)
            {
                ndItemXml.Attributes["PublishingPageImage"].Value = ndItemXml.Attributes["PublishingPageImage"].Value.Replace("home.gif", "home.jpg");
            }
        }

        private string UpdateHtmlLinks(string sHtml)
        {
            return this.UpdateHtmlLinks(sHtml, null);
        }

        private string UpdateHtmlLinks(string sHtml, List<LinkCorrector.HtmlLinkTypeNode> searchList)
        {
            string str;
            if (!string.IsNullOrEmpty(sHtml))
            {
                string str1 = sHtml;
                if (searchList == null)
                {
                    searchList = LinkCorrector.defaultHtmlSearchList;
                }
                foreach (LinkCorrector.HtmlLinkTypeNode htmlLinkTypeNode in searchList)
                {
                    string str2 = "link";
                    string[] tag = new string[] { "<\\s*", htmlLinkTypeNode.Tag, "\\s+([^>]*[\\s'\"]+)?", htmlLinkTypeNode.Attribute, "\\s*=\\s*['\"](?<", str2, ">[^'\"]*)['\"]" };
                    Regex regex = new Regex(string.Concat(tag), RegexOptions.IgnoreCase);
                    MatchCollection matchCollections = regex.Matches(str1);
                    string str3 = "";
                    int index = 0;
                    for (int i = 0; i < matchCollections.Count; i++)
                    {
                        Match item = matchCollections[i];
                        Group group = item.Groups[str2];
                        if (group.Success)
                        {
                            string removeRootFolderRelatedParameters = this.CorrectUrl(group.Value);
                            removeRootFolderRelatedParameters = this.AttemptToRemoveRootFolderRelatedParameters(removeRootFolderRelatedParameters);
                            int num = group.Index - item.Index;
                            string str4 = string.Concat(item.Value.Substring(0, num), removeRootFolderRelatedParameters, item.Value.Substring(num + group.Length));
                            int index1 = item.Index - index;
                            str3 = string.Concat(str3, str1.Substring(index, index1), str4);
                            index = item.Index + item.Length;
                        }
                    }
                    str1 = string.Concat(str3, str1.Substring(index));
                }
                str = str1;
            }
            else
            {
                str = sHtml;
            }
            return str;
        }

        private void UpdateHtmlLinksInAttribute(XmlNode ndXml, string sAttributeName)
        {
            this.UpdateHtmlLinksInAttribute(ndXml, sAttributeName, LinkCorrector.defaultHtmlSearchList);
        }

        private void UpdateHtmlLinksInAttribute(XmlNode ndXml, string sAttributeName, List<LinkCorrector.HtmlLinkTypeNode> searchList)
        {
            if (ndXml.Attributes[sAttributeName] != null)
            {
                ndXml.Attributes[sAttributeName].Value = this.UpdateHtmlLinks(ndXml.Attributes[sAttributeName].Value, searchList);
            }
        }

        private string UpdateLinksInDelimitedString(string sDelimitedUpdate, string sDelimiter, bool bSiteCollectionUrl)
        {
            int i;
            string str = "";
            string[] strArrays = new string[] { sDelimiter };
            string[] strArrays1 = sDelimitedUpdate.Split(strArrays, StringSplitOptions.None);
            string[] strArrays2 = new string[(int)strArrays1.Length];
            for (i = 0; i < (int)strArrays1.Length; i++)
            {
                strArrays2[i] = (bSiteCollectionUrl ? this.CorrectSiteCollectionUrl(strArrays1[i].Trim()) : this.CorrectUrl(strArrays1[i].Trim()));
            }
            for (i = 0; i < (int)strArrays2.Length; i++)
            {
                str = string.Concat(str, strArrays2[i], (i < (int)strArrays2.Length - 1 ? string.Concat(sDelimiter, " ") : ""));
            }
            return str;
        }

        public string UpdateLinksInList(SPList list, SPWeb targetWeb, string sListXml)
        {
            string str = (!string.IsNullOrEmpty(sListXml) ? sListXml : list.XML);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sListXml);
            if (xmlNode.Attributes["DocTemplateUrl"] != null)
            {
                xmlNode.Attributes["DocTemplateUrl"].Value = this.CorrectUrlCore(xmlNode.Attributes["DocTemplateUrl"].Value, true, false);
            }
            return xmlNode.OuterXml;
        }

        public string UpdateLinksInListItem(SPListItem listItem, SPFolder targetFolder, string sListItemXml, bool bRecurseIntoVersions, bool bLinkCorrectTextFields)
        {
            string str;
            if (!string.IsNullOrEmpty(sListItemXml))
            {
                str = sListItemXml;
            }
            else
            {
                str = (bRecurseIntoVersions ? listItem.XMLWithVersions : listItem.XML);
            }
            XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
            SPFieldCollection fields = (SPFieldCollection)listItem.ParentList.Fields;
            this.UpdateLinksInListItem(listItem, targetFolder, xmlNode, fields, bLinkCorrectTextFields);
            if (bRecurseIntoVersions)
            {
                foreach (XmlNode xmlNodes in xmlNode.SelectNodes("Versions//ListItem"))
                {
                    this.UpdateLinksInListItemXml(xmlNodes, fields, bLinkCorrectTextFields);
                }
            }
            return xmlNode.OuterXml;
        }

        public void UpdateLinksInListItem(SPListItem listItem, SPFolder targetFolder, XmlNode listItemXml, SPFieldCollection fields, bool bLinkCorrectTextFields)
        {
            bool hasPublishingPageLayout = listItem.HasPublishingPageLayout;
            if (hasPublishingPageLayout)
            {
                this.AddMappingForPublishingPageLayoutCatalog(listItem, targetFolder);
            }
            this.UpdateLinksInListItemXml(listItemXml, fields, bLinkCorrectTextFields);
            if ((!listItem.Adapter.SharePointVersion.IsSharePoint2007 || !targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater ? false : hasPublishingPageLayout))
            {
                this.Update2010PublishingFields(listItemXml);
            }
        }

        public void UpdateLinksInListItemXml(XmlNode listItemXml, SPFieldCollection fields, bool bLinkCorrectTextFields)
        {
            bool flag;
            foreach (SPField field in fields)
            {
                if (listItemXml.Attributes[field.Name] != null)
                {
                    if (field.Name.Equals("PublishingPageLayout", StringComparison.OrdinalIgnoreCase))
                    {
                        listItemXml.Attributes[field.Name].Value = this.UpdateLinksInDelimitedString(listItemXml.Attributes[field.Name].Value, ",", true);
                    }
                    else if (field.Type.Equals("URL", StringComparison.OrdinalIgnoreCase))
                    {
                        bool flag1 = (fields.ParentWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater ? true : fields.ParentWeb.Adapter.IsDB);
                        string str = this.UpdateLinksInSharePointURL(listItemXml.Attributes[field.Name].Value, flag1);
                        Guid guid = new Guid("58ddda52-c2a3-4650-9178-3bbc1f6e36da");
                        if (field.ID == guid)
                        {
                            string[] strArrays = new string[] { ", " };
                            string[] strArrays1 = str.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
                            if ((int)strArrays1.Length > 0)
                            {
                                string str1 = strArrays1[0].Replace(",", HttpUtility.UrlEncode(",")).Replace("%20", " ");
                                str = str.Replace(strArrays1[0], str1);
                            }
                        }
                        listItemXml.Attributes[field.Name].Value = str;
                    }
                    else if (field.Name.Equals("TemplateUrl", StringComparison.OrdinalIgnoreCase))
                    {
                        listItemXml.Attributes[field.Name].Value = this.CorrectUrl(listItemXml.Attributes[field.Name].Value);
                    }
                    else if ((field.Type.Equals("HTML", StringComparison.OrdinalIgnoreCase) || field.Type.Equals("Image", StringComparison.OrdinalIgnoreCase) || field.Type.Equals("SummaryLinks", StringComparison.OrdinalIgnoreCase) || field.Type.Equals("PublishingPageImage", StringComparison.OrdinalIgnoreCase) ? false : !field.Type.Equals("PublishingRollupImage", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!bLinkCorrectTextFields)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = (field.Type.Equals("Text", StringComparison.OrdinalIgnoreCase) ? false : !field.Type.Equals("Note", StringComparison.OrdinalIgnoreCase));
                        }
                        if (!flag)
                        {
                            this.UpdateLinksInTextField(listItemXml, field.Name);
                        }
                        else if (field.Name.Equals("WikiField", StringComparison.OrdinalIgnoreCase))
                        {
                            this.UpdateHtmlLinksInAttribute(listItemXml, field.Name);
                        }
                        else if (field.Name.Equals("FormURN", StringComparison.OrdinalIgnoreCase))
                        {
                            listItemXml.Attributes[field.Name].Value = this.CorrectUrl(listItemXml.Attributes[field.Name].Value);
                        }
                    }
                    else
                    {
                        this.UpdateHtmlLinksInAttribute(listItemXml, field.Name);
                    }
                }
            }
        }

        public string UpdateLinksInPublishingPage(SPListItem publishingPageDocument, SPFolder targetFolder, string sPublishingPageXml)
        {
            return this.UpdateLinksInListItem(publishingPageDocument, targetFolder, sPublishingPageXml, true, false);
        }

        private string UpdateLinksInSharePointURL(string UrlFieldValue, bool CommaSafeSharePointURL)
        {
            string str;
            string str1;
            string str2;
            this.SeparateSharePointURL(UrlFieldValue, out str, out str1);
            if (CommaSafeSharePointURL)
            {
                str = this.RemoveCommaSafetyFromSharePointURL(str);
            }
            bool flag = str.Equals(str1);
            str = this.CorrectUrl(str);
            if (flag)
            {
                str1 = str;
            }
            str = this.CreateCommaSafeSharePointURL(str);
            str2 = (str1 == null ? str : string.Concat(str, ", ", str1));
            return str2;
        }

        private void UpdateLinksInTextField(XmlNode ndXml, string sAttributeName)
        {
            if (ndXml.Attributes[sAttributeName] != null)
            {
                this.UpdateHtmlLinksInAttribute(ndXml, sAttributeName);
                string value = ndXml.Attributes[sAttributeName].Value;
                string str = "link";
                string str1 = "\\\\\\*\\\"<>\\|\\{\\}~";
                string str2 = "[^{0}{1}]*";
                string[] strArrays = new string[] { "(?<", str, ">(http|https)://", string.Format(str2, str1, "/"), string.Format(str2, str1, ":"), ")" };
                Regex regex = new Regex(string.Concat(strArrays), RegexOptions.IgnoreCase);
                MatchCollection matchCollections = regex.Matches(value);
                StringBuilder stringBuilder = new StringBuilder();
                int index = 0;
                for (int i = 0; i < matchCollections.Count; i++)
                {
                    Group item = matchCollections[i].Groups[str];
                    if (item.Success)
                    {
                        stringBuilder.Append(value.Substring(index, item.Index - index));
                        stringBuilder.Append(this.CorrectUrl(item.Value));
                        index = item.Index + item.Length;
                    }
                }
                stringBuilder.Append(value.Substring(index));
                if (stringBuilder.Length > 0)
                {
                    ndXml.Attributes[sAttributeName].Value = stringBuilder.ToString();
                }
            }
        }

        public string UpdateLinksInWeb(SPWeb web, string sWebXml)
        {
            string str = (!string.IsNullOrEmpty(sWebXml) ? sWebXml : web.XML);
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sWebXml);
            XmlAttribute itemOf = xmlNode.Attributes["AlternateCssUrl"];
            if (itemOf != null)
            {
                itemOf.Value = this.CorrectUrl(itemOf.Value);
            }
            XmlAttribute xmlAttribute = xmlNode.Attributes["AuditLogReportStorageLocation"];
            if (xmlAttribute != null)
            {
                xmlAttribute.Value = this.CorrectUrl(xmlAttribute.Value);
            }
            foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//MeetingInstances/MeetingInstance/@EventUrl"))
            {
                if (!string.IsNullOrEmpty(xmlNodes.Value))
                {
                    xmlNodes.Value = this.UpdateLinksInDelimitedString(xmlNodes.Value, ",", false);
                }
            }
            XmlAttribute itemOf1 = xmlNode.Attributes["SiteLogoUrl"];
            if (itemOf1 != null)
            {
                itemOf1.Value = this.CorrectUrl(itemOf1.Value);
            }
            return xmlNode.OuterXml;
        }

        private static bool UrlStartsWithUrl(string sUrl, string sPrefixUrlToCheck)
        {
            bool flag;
            char[] chrArray = new char[] { '/' };
            sUrl = sUrl.TrimEnd(chrArray);
            chrArray = new char[] { '/' };
            sPrefixUrlToCheck = sPrefixUrlToCheck.TrimEnd(chrArray);
            if (sUrl.StartsWith(sPrefixUrlToCheck))
            {
                flag = ((sUrl.Length == sPrefixUrlToCheck.Length ? false : sUrl[sPrefixUrlToCheck.Length] != '/') ? false : true);
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        private struct HtmlLinkTypeNode
        {
            public string Tag;

            public string Attribute;

            public HtmlLinkTypeNode(string sTagName, string sAttributeName)
            {
                this.Tag = sTagName;
                this.Attribute = sAttributeName;
            }
        }
    }
}