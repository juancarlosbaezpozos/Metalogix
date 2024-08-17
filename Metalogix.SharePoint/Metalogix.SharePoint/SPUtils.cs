using Metalogix.Actions;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Common;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Schema;

namespace Metalogix.SharePoint
{
    public class SPUtils
    {
        public const string LIMITED_SITE_COLLECTION_SCHEMA_XML_RESOURCE_FILE_NAME = "Metalogix.SharePoint.LimitedSiteCollectionXml.xsd";

        public const string SITE_COLLECTION_URL_REGEX = "\\b(http|https)://(\\w+)\\b";

        public const string INVALID_URL_FORMAT = "Invalid Url format";

        public const string URL_NOT_FOUND = "Url is not part of the current connection";

        public SPUtils()
        {
        }

        public static string ConvertWikiLinksToHtmlLinks(string wikiContent, SPFolder targetFolder)
        {
            string str;
            if (!string.IsNullOrEmpty(wikiContent))
            {
                if (targetFolder == null)
                {
                    throw new ArgumentNullException("Target folder cannot be null.");
                }
                MatchCollection matchCollections = Regex.Matches(wikiContent, "\\[\\[.*?\\]\\]", RegexOptions.Singleline);
                Dictionary<string, string> strs = new Dictionary<string, string>();
                foreach (Match match in matchCollections)
                {
                    string value = match.Value;
                    if (!strs.ContainsKey(value))
                    {
                        char[] chrArray = new char[] { '[', ']' };
                        string str1 = value.Trim(chrArray);
                        int num = str1.IndexOf('|');
                        string str2 = (num < 0 ? str1 : str1.Substring(0, num));
                        str2 = Regex.Replace(str2, "(&.*?;)|(<.*?>)|^\\.*|\\.$|\\.\\.+", "");
                        str2 = Regex.Replace(str2, "[/\\\\:*?\"<>#\\t{}%~&]", "");
                        string str3 = (num < 0 ? str1 : str1.Substring(num + 1));
                        string[] strArrays = new string[] { "/", null, null, null, null };
                        string serverRelativeUrl = targetFolder.ServerRelativeUrl;
                        chrArray = new char[] { '/' };
                        strArrays[1] = serverRelativeUrl.Trim(chrArray);
                        strArrays[2] = "/";
                        strArrays[3] = str2;
                        strArrays[4] = ".aspx";
                        string str4 = HttpUtility.UrlPathEncode(string.Concat(strArrays));
                        string str5 = string.Format("<a class=\"{0}\" href=\"{1}\">{2}</a>", (targetFolder.Adapter.SharePointVersion.IsSharePoint2010OrLater ? "ms-missinglink" : "ms-wikilink"), str4, str3);
                        strs.Add(value, str5);
                    }
                }
                foreach (KeyValuePair<string, string> keyValuePair in strs)
                {
                    wikiContent = wikiContent.Replace(keyValuePair.Key, keyValuePair.Value);
                }
                str = wikiContent;
            }
            else
            {
                str = wikiContent;
            }
            return str;
        }

        public static ListPickerItem CreateUserItem(string loginName)
        {
            ListPickerItem listPickerItem = new ListPickerItem()
            {
                Target = loginName,
                Tag = SPUtils.GetSPUserObject(loginName),
                TargetType = "SPUser"
            };
            return listPickerItem;
        }

        public static ActionOperationStatus EvaluateLog(LogItem operation)
        {
            ActionOperationStatus actionOperationStatu;
            ActionOperationStatus actionOperationStatu1;
            if (operation == null)
            {
                actionOperationStatu = ActionOperationStatus.Failed;
            }
            else
            {
                if (operation.Status == ActionOperationStatus.Running || operation.Status == ActionOperationStatus.Completed)
                {
                    actionOperationStatu1 = ActionOperationStatus.Completed;
                }
                else if (operation.Status == ActionOperationStatus.Skipped)
                {
                    actionOperationStatu1 = ActionOperationStatus.Skipped;
                }
                else if (operation.Status == ActionOperationStatus.Different)
                {
                    actionOperationStatu1 = ActionOperationStatus.Different;
                }
                else
                {
                    actionOperationStatu1 = (operation.Status == ActionOperationStatus.Same ? ActionOperationStatus.Same : ActionOperationStatus.Warning);
                }
                actionOperationStatu = actionOperationStatu1;
            }
            return actionOperationStatu;
        }

        public static SPList FindListByNameOrTitle(SPWeb target, string name, string title)
        {
            SPList sPList;
            bool flag;
            bool flag1;
            if (target == null)
            {
                sPList = null;
            }
            else if ((!string.IsNullOrEmpty(name) ? true : !string.IsNullOrEmpty(title)))
            {
                if (target.Lists != null)
                {
                    foreach (SPList list in target.Lists)
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = (name.Equals(list.Title, StringComparison.InvariantCultureIgnoreCase) ? false : !name.Equals(list.Name, StringComparison.InvariantCultureIgnoreCase));
                        }
                        if (flag)
                        {
                            if (string.IsNullOrEmpty(title))
                            {
                                flag1 = true;
                            }
                            else
                            {
                                flag1 = (title.Equals(list.Title, StringComparison.InvariantCultureIgnoreCase) ? false : !title.Equals(list.Name, StringComparison.InvariantCultureIgnoreCase));
                            }
                            if (!flag1)
                            {
                                sPList = list;
                                return sPList;
                            }
                        }
                        else
                        {
                            sPList = list;
                            return sPList;
                        }
                    }
                }
                sPList = null;
            }
            else
            {
                sPList = null;
            }
            return sPList;
        }

        public static string GenerateUniqueName(string targetName, NodeCollection targetCollection)
        {
            return SPUtils.GenerateUniqueName(targetName, targetCollection, false);
        }

        // Metalogix.SharePoint.SPUtils
        public static string GenerateUniqueName(string targetName, NodeCollection targetCollection, bool hasExtensions)
        {
            string result;
            if (!string.IsNullOrEmpty(targetName) && targetCollection != null)
            {
                bool flag = true;
                foreach (Node current in targetCollection)
                {
                    if (current != null)
                    {
                        if (current is SPList)
                        {
                            SPList sPList = (SPList)current;
                            if (targetName.Equals(sPList.Name, StringComparison.InvariantCultureIgnoreCase) || targetName.Equals(sPList.Title, StringComparison.InvariantCultureIgnoreCase))
                            {
                                flag = false;
                                break;
                            }
                        }
                        else if (targetName.Equals(current.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    result = targetName;
                }
                else
                {
                    int num = hasExtensions ? targetName.LastIndexOf('.') : -1;
                    string text = (num > 0) ? targetName.Substring(num) : "";
                    string text2 = (num > 0) ? targetName.Remove(num) : targetName;
                    int num2 = 1;
                    while (true)
                    {
                        try
                        {
                            flag = true;
                            string text3 = string.Concat(new object[]
                            {
                                text2,
                                "_",
                                num2,
                                text
                            });
                            foreach (Node current in targetCollection)
                            {
                                if (current != null)
                                {
                                    if (current is SPList)
                                    {
                                        SPList sPList = (SPList)current;
                                        if (text3.Equals(sPList.Name, StringComparison.InvariantCultureIgnoreCase) || text3.Equals(sPList.Title, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    else if (text3.Equals(current.Name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                result = text3;
                                break;
                            }
                        }
                        finally
                        {
                            num2++;
                        }
                    }
                }
            }
            else
            {
                result = targetName;
            }
            return result;
        }

        public static string GetQueryStringNodeValue(string url, string param)
        {
            int num = url.IndexOf('?');
            return HttpUtility.ParseQueryString((num >= 0 ? url.Substring(num) : string.Empty)).Get(param);
        }

        public static SecurityPrincipalCollection GetReferencedPrincipals(Hashtable uniqueUsers, SecurityPrincipalCollection principals, bool areGroupsAllowed)
        {
            List<SecurityPrincipal> securityPrincipals = new List<SecurityPrincipal>();
            foreach (string key in uniqueUsers.Keys)
            {
                SecurityPrincipal item = principals[key];
                if (item == null)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode xmlNodes = xmlDocument.CreateElement("User");
                    XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("LoginName");
                    xmlNodes.Attributes.Append(xmlAttribute);
                    xmlAttribute.Value = key;
                    item = new SPUser(xmlNodes);
                }
                securityPrincipals.Add(item);
                if ((!areGroupsAllowed ? false : item is SPGroup))
                {
                    SPGroup owner = (SPGroup)item;
                    while (true)
                    {
                        if ((owner.Owner == null ? true : securityPrincipals.Contains(owner.Owner)))
                        {
                            break;
                        }
                        if (!owner.OwnerIsUser)
                        {
                            owner = (SPGroup)owner.Owner;
                            securityPrincipals.Add(owner);
                        }
                        else
                        {
                            securityPrincipals.Add((SPUser)owner.Owner);
                        }
                    }
                }
            }
            return new SecurityPrincipalCollection(securityPrincipals.ToArray());
        }

        public static List<string> GetSiteCollectionsFromXml(string xmlFileContent, out string validationMessage, out Dictionary<string, string> siteCollection, string connectionUrl = null, string connectionScope = null)
        {
            XmlDocument xmlDocument;
            siteCollection = new Dictionary<string, string>();
            List<string> strs = new List<string>();
            validationMessage = string.Empty;
            if (SPUtils.IsValidXml(xmlFileContent, "Metalogix.SharePoint.LimitedSiteCollectionXml.xsd", out xmlDocument, out validationMessage))
            {
                foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("SiteCollections/Url"))
                {
                    string str = xmlNodes.InnerText.TrimEnd(new char[] { '/' });
                    if (!siteCollection.ContainsKey(str))
                    {
                        if (!SPUtils.IsValidSiteCollectionUrl(str))
                        {
                            siteCollection.Add(str, "Invalid Url format");
                        }
                        else if ((string.IsNullOrEmpty(connectionUrl) ? true : SPUtils.IsValidWebAppUrl(connectionUrl, str, connectionScope)))
                        {
                            siteCollection.Add(str, string.Empty);
                        }
                        else
                        {
                            siteCollection.Add(str, "Url is not part of the current connection");
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, string> keyValuePair in siteCollection)
            {
                if (keyValuePair.Value.Equals(string.Empty))
                {
                    strs.Add(keyValuePair.Key);
                }
            }
            return strs;
        }

        public static SPUser GetSPUserObject(string loginName)
        {
            XmlElement xmlElement = (new XmlDocument()).CreateElement("SPUser");
            xmlElement.SetAttribute("LoginName", loginName);
            xmlElement.SetAttribute("Name", string.Empty);
            xmlElement.SetAttribute("Email", string.Empty);
            xmlElement.SetAttribute("Notes", string.Empty);
            return new SPUser(xmlElement);
        }

        public static string GetTargetList(Dictionary<Guid, Guid> guidMappings, string sourceListId, SPWeb sourceWeb, SPWeb targetWeb)
        {
            string d;
            if (!guidMappings.ContainsKey(new Guid(sourceListId)))
            {
                string str = Convert.ToString(new Guid(sourceListId));
                SPList listByGuid = sourceWeb.Lists.GetListByGuid(str);
                if (listByGuid != null)
                {
                    SPList item = targetWeb.Lists[listByGuid.Name];
                    if (item != null)
                    {
                        d = item.ID;
                        return d;
                    }
                }
                d = str;
            }
            else
            {
                d = guidMappings[new Guid(sourceListId)].ToString();
            }
            return d;
        }

        public static string GetUpdatedOneNoteFolderName(SPWeb web)
        {
            return (!web.Adapter.SharePointVersion.IsSharePointOnline || !web.Template.Name.Equals("STS#0") || !web.IsRootWeb ? string.Concat(web.Title, " Notebook") : "Team Site Notebook");
        }

        public static string InsertStringAtInterval(string source, string insert, int interval)
        {
            string str = source;
            for (int i = interval; i < str.Length; i += interval)
            {
                str = str.Insert(i, insert);
            }
            return str;
        }

        public static bool IsDefaultOneNoteFolder(SPListItem listItem)
        {
            bool flag;
            if ((listItem.ItemType != SPListItemType.Folder || listItem.ParentList.BaseTemplate != ListTemplateType.DocumentLibrary || !listItem.ParentList.Name.Equals("SiteAssets", StringComparison.InvariantCultureIgnoreCase) ? true : !listItem.IsOneNoteFolder))
            {
                flag = false;
            }
            else
            {
                string updatedOneNoteFolderName = SPUtils.GetUpdatedOneNoteFolderName(listItem.ParentList.ParentWeb);
                flag = listItem.Title.Equals(updatedOneNoteFolderName, StringComparison.InvariantCultureIgnoreCase);
            }
            return flag;
        }

        public static bool IsDefaultOneNoteFolder(SPFolder folder)
        {
            bool flag;
            if ((folder.ParentList.BaseTemplate != ListTemplateType.DocumentLibrary || !folder.ParentList.Name.Equals("SiteAssets", StringComparison.InvariantCultureIgnoreCase) ? true : !folder.IsOneNoteFolder))
            {
                flag = false;
            }
            else
            {
                string updatedOneNoteFolderName = SPUtils.GetUpdatedOneNoteFolderName(folder.ParentList.ParentWeb);
                flag = folder.Name.Equals(updatedOneNoteFolderName, StringComparison.InvariantCultureIgnoreCase);
            }
            return flag;
        }

        public static bool IsDefaultOneNoteItem(SPListItem listItem)
        {
            return (listItem.ParentList.BaseTemplate != ListTemplateType.DocumentLibrary || !listItem.ParentList.Name.Equals("SiteAssets", StringComparison.InvariantCultureIgnoreCase) ? false : SPUtils.IsOneNoteItem(listItem));
        }

        public static bool IsGlobalUserMappingNeeded(SPNode source, SPNode target)
        {
            return ((source.Adapter.AdapterShortName.Equals("DB") || !source.Adapter.ServerUrl.Equals(target.Adapter.ServerUrl, StringComparison.InvariantCultureIgnoreCase) ? SPGlobalMappings.GlobalUserMappings.Count >= 1 : true) ? false : true);
        }

        public static bool IsNintexWorkflow(SPList parentList, string sourceItemName)
        {
            bool flag;
            try
            {
                if ((parentList == null ? false : string.Equals(parentList.Name, "Workflows", StringComparison.InvariantCultureIgnoreCase)))
                {
                    SPListCollection lists = parentList.ParentWeb.Lists;
                    if (lists.Count > 0)
                    {
                        SPList item = lists["NintexWorkflows"];
                        if ((item == null ? false : item.SubFolders[sourceItemName] != null))
                        {
                            flag = true;
                            return flag;
                        }
                    }
                }
                flag = false;
            }
            catch (Exception exception)
            {
                flag = true;
            }
            return flag;
        }

        public static bool IsOneNoteFeatureEnabled(SPFolder folder)
        {
            bool flag;
            if ((folder == null ? true : !folder.Adapter.SharePointVersion.IsSharePoint2013OrLater))
            {
                flag = false;
            }
            else
            {
                SPList sPList = folder as SPList;
                if (sPList == null)
                {
                    flag = (folder.ParentList == null || folder.ParentList.ParentWeb == null ? false : folder.ParentList.ParentWeb.HasOneNote2010NotebookFeature);
                }
                else
                {
                    flag = (sPList.ParentWeb == null ? false : sPList.ParentWeb.HasOneNote2010NotebookFeature);
                }
            }
            return flag;
        }

        public static bool IsOneNoteItem(SPListItem item)
        {
            bool flag;
            if (item.ItemType != SPListItemType.Folder)
            {
                if (!(item.ParentFolder == null ? true : !item.ParentFolder.IsOneNoteFolder))
                {
                    flag = true;
                    return flag;
                }
                else if ((item.ParentList == null || item.ParentList.ParentWeb == null || !item.ParentList.ParentWeb.HasOneNote2010NotebookFeature ? false : item.ParentRelativePath == SPUtils.GetUpdatedOneNoteFolderName(item.ParentList.ParentWeb)))
                {
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        public static bool IsValidSiteCollectionUrl(string siteCollectionUrl)
        {
            return (!Uri.IsWellFormedUriString(siteCollectionUrl, UriKind.Absolute) ? false : Regex.IsMatch(siteCollectionUrl, "\\b(http|https)://(\\w+)\\b"));
        }

        private static bool IsValidWebAppUrl(string connectionUrl, string siteCollectionUrl, string scopeOfConnection)
        {
            bool port;
            Uri uri = new Uri(connectionUrl);
            if (scopeOfConnection != null)
            {
                Uri uri1 = new Uri(siteCollectionUrl);
                if (scopeOfConnection.Equals(Convert.ToString(ConnectionScope.Farm), StringComparison.InvariantCultureIgnoreCase))
                {
                    port = uri.Host.Equals(uri1.Host, StringComparison.InvariantCultureIgnoreCase);
                }
                else if (!scopeOfConnection.Equals(Convert.ToString(ConnectionScope.Tenant), StringComparison.InvariantCultureIgnoreCase))
                {
                    if (scopeOfConnection.Equals(Convert.ToString(ConnectionScope.WebApp), StringComparison.InvariantCultureIgnoreCase))
                    {
                        port = uri.Port == uri1.Port;
                        return port;
                    }
                    port = false;
                    return port;
                }
                else
                {
                    TenantSetting tenantSetting = TenantSettingManager.GetTenantSetting(connectionUrl);
                    string[] mySiteHostPath = new string[] { tenantSetting.MySiteHostPath, tenantSetting.MySiteManagedPath };
                    if (!siteCollectionUrl.StartsWith(UrlUtils.ConcatUrls(mySiteHostPath), StringComparison.InvariantCultureIgnoreCase))
                    {
                        string str = Regex.Replace(uri.Host, "-admin", string.Empty, RegexOptions.IgnoreCase);
                        port = str.Equals(uri1.Host, StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        port = true;
                    }
                }
            }
            else
            {
                port = false;
                return port;
            }
            return port;
        }

        public static bool IsValidXml(string xmlFileContent, string schemaXmlResourceFileName, out XmlDocument xmlDoc, out string validationMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            validationMessage = string.Empty;
            xmlDoc = new XmlDocument();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(schemaXmlResourceFileName);
            if (manifestResourceStream != null)
            {
                try
                {
                    xmlDoc.LoadXml(xmlFileContent);
                    xmlDoc.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
                    xmlDoc.Validate((object s, ValidationEventArgs args) =>
                    {
                        if (args.Severity == XmlSeverityType.Error)
                        {
                            stringBuilder.AppendLine(args.Message);
                        }
                    });
                }
                catch (Exception exception)
                {
                    stringBuilder.AppendLine(exception.Message);
                }
            }
            else
            {
                stringBuilder.AppendLine(string.Format("The resource: '{0}' did not exist within the assembly: '{1}'", schemaXmlResourceFileName, executingAssembly.Location));
            }
            if (stringBuilder.Length > 0)
            {
                validationMessage = string.Format("An error occurred while loading xml file for limited site collection connection.\nError: {0} \nPlease refer sample xml file for correct format.", stringBuilder.ToString());
            }
            return string.IsNullOrEmpty(validationMessage);
        }

        public static void MapBaseNameToEmptyTitle(ref string sTargetXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sTargetXml);
            XmlNode documentElement = xmlDocument.DocumentElement;
            XmlAttribute itemOf = documentElement.Attributes["BaseName"];
            XmlAttribute value = documentElement.Attributes["Title"];
            if ((itemOf == null ? false : value != null))
            {
                if ((!string.IsNullOrEmpty(value.Value) ? false : !string.IsNullOrEmpty(itemOf.Value)))
                {
                    value.Value = itemOf.Value;
                    sTargetXml = xmlDocument.OuterXml;
                }
            }
        }

        public static string OneNoteFolderDisplayName(SPFolder targetFolder, bool fullUrl = false)
        {
            string empty = string.Empty;
            if (targetFolder != null)
            {
                if (!targetFolder.IsOneNoteFolder)
                {
                    empty = SPUtils.GetUpdatedOneNoteFolderName(targetFolder.ParentList.ParentWeb) ?? string.Empty;
                    if (fullUrl)
                    {
                        empty = string.Concat(targetFolder.DisplayUrl, "/", empty);
                    }
                }
                else
                {
                    empty = targetFolder.Name;
                    if (fullUrl)
                    {
                        empty = targetFolder.DisplayUrl;
                    }
                }
            }
            return empty;
        }

        public static string RemoveClaimString(string value)
        {
            return (string.IsNullOrEmpty(value) ? value : value.Substring(value.LastIndexOf('|') + 1));
        }

        public static string SanitizeInput(string input)
        {
            HashSet<char> chrs = new HashSet<char>("/\\[]:|<>+=;,?*'@");
            StringBuilder stringBuilder = new StringBuilder(input.Length);
            string str = input;
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (!chrs.Contains(chr))
                {
                    stringBuilder.Append(chr);
                }
            }
            return stringBuilder.ToString();
        }

        public static void SetLanguageResourcesCollection(string listXML, SPWeb web)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(listXML);
                string attributeValueAsString = xmlDocument.DocumentElement.GetAttributeValueAsString("Title");
                XmlNode xmlNodes = xmlDocument.SelectSingleNode("/List/Views");
                if (xmlNodes != null)
                {
                    XmlDocument xmlDocument1 = new XmlDocument();
                    foreach (XmlNode childNode in xmlNodes.ChildNodes)
                    {
                        xmlDocument1.LoadXml(childNode.OuterXml);
                        XmlNode xmlNodes1 = xmlDocument1.SelectSingleNode("//View");
                        if (xmlNodes1 != null)
                        {
                            string str = xmlNodes1.GetAttributeValueAsString("DisplayName");
                            string attributeValueAsString1 = xmlNodes1.GetAttributeValueAsString("Type");
                            bool attributeValueAsBoolean = xmlNodes1.GetAttributeValueAsBoolean("Hidden");
                            if ((string.IsNullOrEmpty(str) || attributeValueAsBoolean ? false : !attributeValueAsString1.Equals("Table", StringComparison.OrdinalIgnoreCase)))
                            {
                                XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode("//LanguageResources");
                                if (xmlNodes2 != null)
                                {
                                    foreach (XmlNode childNode1 in xmlNodes2.ChildNodes)
                                    {
                                        ViewLanguageResource viewLanguageResource = new ViewLanguageResource()
                                        {
                                            ListTitle = attributeValueAsString,
                                            ViewUrl = xmlNodes1.GetAttributeValueAsString("Url"),
                                            Language = childNode1.GetAttributeValueAsString("Culture"),
                                            ViewTitleResource = childNode1.GetAttributeValueAsString("Title")
                                        };
                                        if (!web.LanguageResourcesForViews.Contains(viewLanguageResource))
                                        {
                                            web.LanguageResourcesForViews.Add(viewLanguageResource);
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
    }
}