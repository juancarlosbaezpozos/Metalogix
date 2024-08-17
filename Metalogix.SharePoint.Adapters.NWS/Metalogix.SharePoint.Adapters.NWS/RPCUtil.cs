using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS
{
    public static class RPCUtil
    {
        private const string _ADMIN_DLL_PATH = "/_vti_bin/_vti_adm/admin.dll";

        public static string CheckinDocument(NWSAdapter callingAdapter, string sAddress, string sFileURL,
            string sComment)
        {
            string str = string.Format("method={0}&service_name=&document_name={1}&comment={2}&keep_checked_out=false",
                HttpUtility.UrlEncode("checkin document:6.0.2.6356", Encoding.UTF8),
                HttpUtility.UrlEncode(sFileURL, Encoding.UTF8), HttpUtility.UrlEncode(sComment, Encoding.UTF8));
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
            string str1 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_aut/author.dll"),
                memoryStream);
            return str1;
        }

        public static bool CheckOutDocument(NWSAdapter callingAdapter, string sAddress, string sDocumentUrl)
        {
            string str;
            string str1;
            RPCUtil.UrlToWebUrl(callingAdapter, sDocumentUrl, out str, out str1);
            string str2 = string.Format("method=checkout+document&service_name=/&document_name={0}&force=0&timeout=0",
                HttpUtility.UrlEncode(str1));
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.ASCII.GetBytes(str2), 0, str2.Length);
            RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_aut/author.dll"), memoryStream);
            return true;
        }

        public static bool Create2007Site(NWSAdapter callingAdapter, string sAddress, string sSiteURLName,
            string sSiteTitle, string sSiteDescription, string sTemplateName)
        {
            string str;
            string str1 = null;
            RPCUtil.UrlToWebUrl(callingAdapter, sAddress, out str1, out str);
            if (str1.EndsWith("/"))
            {
                str1 = str1.Remove(str1.Length - 1, 1);
            }

            str1 = HttpUtility.UrlEncode(string.Concat(str1, "/", sSiteURLName));
            string str2 = string.Concat("method=create+service:6.0.2.6356&service_name=", str1);
            string str3 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_adm/admin.dll"),
                str2);
            if (str3.Contains("The underlying connection was closed: The connection was closed unexpectedly."))
            {
                throw new Exception(string.Concat("Could not create site: the URL '", sSiteURLName,
                    "' may contain invalid characaters."));
            }

            string returnValue = RPCUtil.GetReturnValue(str3, "msg");
            if (returnValue != null)
            {
                throw new Exception(returnValue);
            }

            string str4 = string.Concat("method=set+service+meta%2dinfo:6.0.2.6356&service_name=", str1,
                "&meta_info=[vti_showhiddenpages;IR|1]");
            string str5 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_aut/author.dll"),
                str4);
            string returnValue1 = RPCUtil.GetReturnValue(str5, "msg");
            if (returnValue1 != null)
            {
                throw new Exception(returnValue1);
            }

            if (sTemplateName != null)
            {
                string str6 = string.Format(
                    "Cmd=DisplayPost&PostBody=<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Method ID=\"0,SiteProvision\">\n<SetVar Name=\"Cmd\">SiteProvision</SetVar><SetVar Name=\"CreateLists\">True</SetVar><SetVar Name=\"SiteTemplate\">{0}</SetVar></Method>",
                    sTemplateName);
                RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/", sSiteURLName, "/_vti_bin/owssvr.dll"),
                    str6);
            }

            string str7 = string.Format(
                "Cmd=DisplayPost&PostBody=<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Method ID=\"0,UpdateProject\"><SetVar Name=\"Cmd\">UpdateProject</SetVar><SetVar Name=\"NewListTitle\">{0}</SetVar><SetVar Name=\"Description\">{1}</SetVar><SetVar Name=\"owshiddenversion\">-1</SetVar></Method>",
                HttpUtility.UrlEncode(sSiteTitle), HttpUtility.UrlEncode(sSiteDescription));
            RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/", sSiteURLName, "/_vti_bin/owssvr.dll"),
                str7);
            return true;
        }

        public static bool CreateSite2003(NWSAdapter callingAdapter, string sAddress, string sSiteURLName,
            string sSiteTitle, string sSiteDescription, string sTemplateName)
        {
            string str;
            string str1 = null;
            RPCUtil.UrlToWebUrl(callingAdapter, sAddress, out str1, out str);
            if (str1.EndsWith("/"))
            {
                str1 = str1.Remove(str1.Length - 1, 1);
            }

            str1 = string.Concat(str1, "/", HttpUtility.UrlEncode(sSiteURLName));
            string str2 = string.Concat("method=create+service&service_name=", str1);
            string str3 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_adm/admin.dll"),
                str2);
            string returnValue = RPCUtil.GetReturnValue(str3, "msg");
            if (returnValue != null)
            {
                throw new Exception(returnValue);
            }

            string str4 = string.Concat("method=get+manifest&service_name=", str1, "&options=structure");
            string str5 = RPCUtil.SendRequest(callingAdapter,
                string.Concat(sAddress, "/", sSiteURLName, "/_vti_bin/_vti_adm/admin.dll"), str4);
            int num = str5.IndexOf("<?xml");
            if (num < 0)
            {
                return true;
            }

            str5 = str5.Substring(num);
            str5 = str5.Replace("<Title></Title>", string.Concat("<Title>", sSiteTitle, "</Title>"));
            str5 = str5.Replace("<Description></Description>",
                string.Concat("<Description>", sSiteDescription, "</Description>"));
            string str6 = string.Concat("method=put+manifest&service_name=", str1, "&options=first+pass&prefix=\n",
                str5);
            RPCUtil.SendRequest(callingAdapter,
                string.Concat(sAddress, "/", sSiteURLName, "/_vti_bin/_vti_adm/admin.dll"), str6);
            string str7 = string.Format(
                "Cmd=DisplayPost&PostBody=<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Method ID=\"0,SiteProvision\">\n<SetVar Name=\"Cmd\">SiteProvision</SetVar><SetVar Name=\"CreateLists\">True</SetVar><SetVar Name=\"SiteTemplate\">{0}</SetVar></Method>",
                sTemplateName);
            RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/", sSiteURLName, "/_vti_bin/owssvr.dll"),
                str7);
            return true;
        }

        private static string DecodeSharePointPropertyName(string sPropName)
        {
            string str = sPropName;
            Match match = null;
            while (true)
            {
                Match match1 = Regex.Match(str, "_x[a-zA-Z0-9]{4}_");
                match = match1;
                if (match1 == Match.Empty)
                {
                    break;
                }

                string str1 = string.Concat("%", match.Value.Substring(2, 2), "%", match.Value.Substring(4, 2));
                string str2 = HttpUtility.UrlDecode(str1, Encoding.BigEndianUnicode);
                str = string.Concat(str.Substring(0, match.Index), str2, str.Substring(match.Index + match.Length));
            }

            return str;
        }

        public static void DeleteDocument(NWSAdapter callingAdapter, string sDocUrl)
        {
            string str = string.Concat("method=remove+documents:12.0.0.6415&service_name=&url_list=[",
                HttpUtility.UrlEncode(sDocUrl), "]");
            string str1 = Utils.JoinUrl(callingAdapter.ServerUrl, callingAdapter.ServerRelativeUrl);
            char[] chrArray = new char[] { '/' };
            string str2 = RPCUtil.SendRequest(callingAdapter,
                string.Concat(str1.TrimEnd(chrArray), "/_vti_bin/_vti_adm/admin.dll"), str);
            string returnValue = RPCUtil.GetReturnValue(str2, "msg");
            if (returnValue != null)
            {
                throw new Exception(string.Concat("Failed to delete document: ", returnValue));
            }
        }

        public static bool DeleteWeb(NWSAdapter callingAdapter, string sAddress)
        {
            string str = null;
            string str1 = null;
            RPCUtil.UrlToWebUrl(callingAdapter, sAddress, out str, out str1);
            str = HttpUtility.UrlEncode(str);
            string str2 = string.Concat("method=remove+service:6.0.2.6356&service_name=", str);
            string str3 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_adm/admin.dll"),
                str2);
            string returnValue = RPCUtil.GetReturnValue(str3, "msg");
            string returnValue1 = RPCUtil.GetReturnValue(str3, "<li>status");
            if (returnValue != null)
            {
                Exception exception = new Exception(returnValue);
                if (!string.IsNullOrEmpty(returnValue1))
                {
                    exception.Data.Add("Status", int.Parse(returnValue1));
                }

                throw exception;
            }

            return true;
        }

        public static bool DocumentHasAssociatedListItem(SharePointAdapter adapter, string sDocUrl)
        {
            StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(adapter, sDocUrl);
            if (UrlUtils.Equal(adapter.Url, UrlUtils.RemoveAfterLastSlash(standardizedUrl.Full)))
            {
                return false;
            }

            return RPCUtil.GetFileRPCMetaInfo(adapter, sDocUrl).Contains("<li>ContentType");
        }

        public static string EncodeStringForFPRequest(string str)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            char[] chrArray = new char[] { '\\', ';', '=', ']', '[' };
            for (int i = 0; i < str.Length; i++)
            {
                if (-1 == str[i].ToString().IndexOfAny(chrArray))
                {
                    char chr = str[i];
                    stringBuilder.Append(HttpUtility.UrlEncode(chr.ToString()));
                }
                else
                {
                    char chr1 = str[i];
                    stringBuilder.Append(HttpUtility.UrlEncode(string.Concat("\\", chr1.ToString())));
                }
            }

            return stringBuilder.ToString();
        }

        public static string EscapeAndEncodeValue(string sText)
        {
            if (sText == null)
            {
                return null;
            }

            sText = RPCUtil.RPCEscape(sText);
            return HttpUtility.UrlEncode(sText);
        }

        public static string FormatFieldForRPC(string sName, string sValue, Type fieldType,
            TimeZoneInformation timeZone)
        {
            string str;
            try
            {
                sName = HttpUtility.UrlEncode(RPCUtil.DecodeSharePointPropertyName(sName));
                string str1 = null;
                if (fieldType.Equals(typeof(float)))
                {
                    str1 = "IX";
                }
                else if (fieldType.Equals(typeof(bool)))
                {
                    str1 = "BR";
                }
                else if (fieldType.Equals(typeof(DateTime)))
                {
                    str1 = "TW";
                    DateTime dateTime = Utils.ParseDate(sValue, timeZone);
                    sValue = (string.IsNullOrEmpty(sValue) ? "" : dateTime.ToString("dd+MMM+yyyy+HH:mm:ss"));
                }
                else if (!fieldType.Equals(typeof(int)))
                {
                    str1 = "SW";
                    sValue = (string.IsNullOrEmpty(sValue) ? "" : RPCUtil.EncodeStringForFPRequest(sValue));
                }
                else
                {
                    str1 = "IW";
                    sValue = (string.IsNullOrEmpty(sValue) ? "" : RPCUtil.EncodeStringForFPRequest(sValue));
                }

                string[] strArrays = new string[] { sName, ";", str1, "|", sValue };
                str = string.Concat(strArrays);
            }
            catch
            {
                str = null;
            }

            return str;
        }

        public static string FormatFrontPageRPCMessage(string sWebRelativePath, string sMetaInfo, bool bOverwrite,
            bool bKeepCheckedOut, string sVersionString)
        {
            if (sMetaInfo == null)
            {
                sMetaInfo = "";
            }

            sVersionString = (sVersionString == null ? "6.0.2.6356" : sVersionString);
            string str = HttpUtility.UrlEncode(string.Concat("put document:", sVersionString), Encoding.UTF8);
            sWebRelativePath = RPCUtil.ValidFileName(sWebRelativePath);
            string str1 = RPCUtil.RPCEscape(sWebRelativePath);
            str1 = HttpUtility.UrlEncode(str1, Encoding.UTF8);
            object[] lower = new object[] { str, str1, sMetaInfo, null, null };
            lower[3] = string.Concat((bOverwrite ? "overwrite" : "edit"), ",migrationsemantics");
            lower[4] = bKeepCheckedOut.ToString().ToLower();
            return string.Format(
                "method={0}&service_name=/&document=[document_name={1};meta_info=[{2}]]&put_option={3},migrationsemantics&comment=&keep_checked_out={4}\n",
                lower);
        }

        public static string FormatItemMetadataForRPC(XmlNode itemXml, string sOws)
        {
            string value = "";
            string str = "";
            DateTime now = DateTime.Now;
            DateTime dateTime = DateTime.Now;
            string str1 = "";
            string str2 = "SW";
            string str3 = "TW";
            IFormatProvider cultureInfo = new CultureInfo("");
            if (itemXml.Attributes[string.Concat(sOws, "Created")] != null)
            {
                now = DateTime.Parse(itemXml.Attributes[string.Concat(sOws, "Created")].Value, cultureInfo);
            }

            if (itemXml.Attributes[string.Concat(sOws, "Modified")] != null)
            {
                string value1 = itemXml.Attributes[string.Concat(sOws, "Modified")].Value;
                dateTime = DateTime.Parse(itemXml.Attributes[string.Concat(sOws, "Modified")].Value, cultureInfo);
            }

            if (itemXml.Attributes[string.Concat(sOws, "Author")] != null)
            {
                value = itemXml.Attributes[string.Concat(sOws, "Author")].Value;
            }

            if (itemXml.Attributes[string.Concat(sOws, "Editor")] != null)
            {
                str = itemXml.Attributes[string.Concat(sOws, "Editor")].Value;
            }

            string str4 = dateTime.ToString();
            str4 = dateTime.ToString("dd+MMM+yyyy+HH:mm:ss");
            str1 = string.Concat(str1, (str1.Length > 0 ? ";" : ""));
            string str5 = str1;
            string[] strArrays = new string[] { str5, "vti_timelastmodified;", str3, "|", str4 };
            str1 = string.Concat(strArrays);
            string str6 = now.ToString("dd+MMM+yyyy+HH:mm:ss");
            str1 = string.Concat(str1, (str1.Length > 0 ? ";" : ""));
            string str7 = str1;
            string[] strArrays1 = new string[] { str7, "vti_timecreated;", str3, "|", str6 };
            str1 = string.Concat(strArrays1);
            str1 = string.Concat(str1, (str1.Length > 0 ? ";" : ""));
            string str8 = str1;
            string[] strArrays2 = new string[]
                { str8, "vti_modifiedby;", str2, "|", RPCUtil.EncodeStringForFPRequest(str) };
            str1 = string.Concat(strArrays2);
            str1 = string.Concat(str1, (str1.Length > 0 ? ";" : ""));
            string str9 = str1;
            string[] strArrays3 = new string[]
                { str9, "vti_author;", str2, "|", RPCUtil.EncodeStringForFPRequest(value) };
            str1 = string.Concat(strArrays3);
            return str1;
        }

        public static string GetFileRPCMetaInfo(SharePointAdapter adapter, string sDocUrl)
        {
            sDocUrl = HttpUtility.UrlPathEncode(sDocUrl);
            string str = string.Format("method=getDocsMetaInfo&url_list=[{0}]", sDocUrl);
            string[] url = new string[] { adapter.Url, "/_vti_bin/_vti_adm/admin.dll" };
            return RPCUtil.SendRequest(adapter, UrlUtils.ConcatUrls(url), str);
        }

        public static XmlNode GetFolderContentFromWeb(NWSAdapter callingAdapter, string sRelativeAddress,
            ListItemQueryType itemTypes)
        {
            XmlNode documentElement;
            try
            {
                if (callingAdapter != null)
                {
                    bool flag = (itemTypes & ListItemQueryType.Folder) == ListItemQueryType.Folder;
                    string str = string.Concat(
                        "method=list+documents&service_name=&listHiddenDocs=true&listExplorerDocs=false&listRecurse=false&listFiles=false&listFolders=",
                        flag.ToString().ToLower(),
                        "&listLinkInfo=false&listIncludeParent=false&listDerived=false&listBorders=false&listChildWebs=false&listThickets=false&initialUrl=",
                        HttpUtility.UrlEncode(sRelativeAddress));
                    string url = callingAdapter.Url;
                    char[] chrArray = new char[] { '/' };
                    string str1 = RPCUtil.SendRequest(callingAdapter,
                        string.Concat(url.TrimEnd(chrArray), "/_vti_bin/_vti_aut/author.dll"), str);
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
                    xmlTextWriter.WriteStartElement("FolderContent");
                    RPCUtil.WriteFilesXmlFromRPCResponse(xmlTextWriter, str1);
                    if ((itemTypes & ListItemQueryType.Folder) == ListItemQueryType.Folder)
                    {
                        RPCUtil.WriteFoldersXmlFromRPCResponse(xmlTextWriter, str1);
                    }

                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.Flush();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(stringBuilder.ToString());
                    documentElement = xmlDocument.DocumentElement;
                }
                else
                {
                    documentElement = null;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Failed to fetch files on site root: ", exception.Message));
            }

            return documentElement;
        }

        public static string GetReturnValue(string response, string key)
        {
            string valueFromResponse = RPCUtil.GetValueFromResponse(response, key);
            if (string.IsNullOrEmpty(valueFromResponse))
            {
                return valueFromResponse;
            }

            return HttpUtility.HtmlDecode(valueFromResponse);
        }

        private static string GetReturnValueOfUrl(string response, string key)
        {
            int num;
            string valueFromResponse = RPCUtil.GetValueFromResponse(response, key);
            if (string.IsNullOrEmpty(valueFromResponse))
            {
                return valueFromResponse;
            }

            using (StringWriter stringWriter = new StringWriter())
            {
                string str = "&#(?<1>\\d+);";
                int index = 0;
                foreach (Match match in (new Regex(str, RegexOptions.IgnoreCase)).Matches(valueFromResponse))
                {
                    if (!int.TryParse(match.Groups[1].Value, out num))
                    {
                        continue;
                    }

                    stringWriter.Write(valueFromResponse.Substring(index, match.Index - index));
                    stringWriter.Write("%");
                    stringWriter.Write(num.ToString("X"));
                    index = match.Index + match.Length;
                }

                if (index < valueFromResponse.Length)
                {
                    stringWriter.Write(valueFromResponse.Substring(index));
                }

                valueFromResponse = stringWriter.ToString();
            }

            return HttpUtility.UrlDecode(valueFromResponse);
        }

        public static string GetRPCErrorMessage(string sResponse)
        {
            if (!sResponse.StartsWith("<html>"))
            {
                return sResponse;
            }

            int length = sResponse.IndexOf("<li>status=");
            if (length < 0)
            {
                return null;
            }

            length += "<li>status=".Length;
            if (sResponse.IndexOf("<", length) < 0)
            {
                return null;
            }

            if (RPCUtil.GetRPCResponsePropertyValue(sResponse, "<li>status=", 0) == "0")
            {
                return null;
            }

            return RPCUtil.GetRPCResponsePropertyValue(sResponse, "<li>msg=", 0);
        }

        public static string GetRPCResponsePropertyValue(string sRPCResponse, string sPropertyPrefix, int iStartIndex)
        {
            if (iStartIndex >= sRPCResponse.Length)
            {
                return null;
            }

            int length = sRPCResponse.IndexOf(sPropertyPrefix, iStartIndex);
            if (length < 0)
            {
                return null;
            }

            length += sPropertyPrefix.Length;
            int num = sRPCResponse.IndexOf("<", length);
            if (num < 0)
            {
                return null;
            }

            return sRPCResponse.Substring(length, num - length).Trim();
        }

        public static string GetSoapError(XmlNode errorNode)
        {
            if (errorNode == null)
            {
                return "";
            }

            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(errorNode.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("default", "http://schemas.microsoft.com/sharepoint/soap/");
            XmlNode xmlNodes = errorNode.SelectSingleNode("//default:errorstring", xmlNamespaceManagers);
            if (xmlNodes == null)
            {
                return errorNode.OuterXml;
            }

            string innerText = xmlNodes.InnerText;
            innerText = innerText.Trim(new char[] { '\n', ' ', '\t' });
            int num = innerText.IndexOf("\n\n\t");
            if (num > 0)
            {
                innerText = innerText.Substring(0, num);
            }

            return innerText;
        }

        public static string GetSoapErrorCode(XmlNode errorNode)
        {
            if (errorNode == null)
            {
                return "";
            }

            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(errorNode.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("default", "http://schemas.microsoft.com/sharepoint/soap/");
            XmlNode xmlNodes = errorNode.SelectSingleNode("//default:errorcode", xmlNamespaceManagers);
            if (xmlNodes == null)
            {
                return "";
            }

            return xmlNodes.InnerText.Trim();
        }

        private static string GetStringBetweenTags(string response, int tagIndex)
        {
            int num = response.IndexOf(">", tagIndex) + 1;
            int num1 = response.IndexOf("<", num);
            string str = response.Substring(num, num1 - num);
            return HttpUtility.HtmlDecode(str.Trim());
        }

        public static string GetValueFromResponse(string response, string key)
        {
            int length = response.IndexOf(string.Concat(key, "="));
            if (length < 0)
            {
                return null;
            }

            length = length + key.Length + 1;
            int num = response.IndexOf("\n", length);
            return response.Substring(length, num - length);
        }

        public static string RPCEscape(string s)
        {
            string str = s.Replace("\\", "\\\\");
            str = str.Replace("|", "\\|");
            str = str.Replace("=", "\\=");
            str = str.Replace(";", "\\;");
            return str.Replace("[", "\\[").Replace("]", "\\]");
        }

        public static string SendRequest(SharePointAdapter callingAdapter, string sUri, string sPostBody)
        {
            Encoding uTF8Encoding = new UTF8Encoding();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(uTF8Encoding.GetBytes(sPostBody), 0, sPostBody.Length);
            return RPCUtil.SendRequest(callingAdapter, sUri, memoryStream);
        }

        public static string SendRequest(SharePointAdapter callingAdapter, string sUri, Stream postStream)
        {
            return RPCUtil.SendRequest(callingAdapter, sUri, postStream, false, false);
        }

        public static string SendRequest(SharePointAdapter callingAdapter, string sUri, Stream postStream,
            bool bAlwaysUseNetworkCallRepition, bool bDisableCallRepition)
        {
            return RPCUtil.SendRequest(callingAdapter, sUri, postStream, "application/x-www-form-urlencoded",
                bAlwaysUseNetworkCallRepition, bDisableCallRepition);
        }

        public static string SendRequest(SharePointAdapter callingAdapter, string sUri, Stream postStream,
            string sContentType, bool bAlwaysUseNetworkCallRepition, bool bDisableCallRepition)
        {
            string end = null;
            Stream requestStream = null;
            WebRequest networkCredentials = null;
            byte[] numArray = null;
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader streamReader = null;
            if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
            {
                callingAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                bool flag = false;
                bool flag1 = true;
                do
                {
                    flag = false;
                    networkCredentials = WebRequest.Create(sUri);
                    networkCredentials.Timeout = 180000;
                    networkCredentials.Method = "POST";
                    networkCredentials.ContentType = sContentType;
                    networkCredentials.Headers.Add("X-Vermeer-Content-Type", "application/x-www-form-urlencoded");
                    networkCredentials.Credentials = callingAdapter.Credentials.NetworkCredentials;
                    HttpWebRequest cookieContainer = networkCredentials as HttpWebRequest;
                    cookieContainer.KeepAlive = false;
                    callingAdapter.IncludedCertificates.CopyCertificatesToCollection(cookieContainer
                        .ClientCertificates);
                    if (callingAdapter.AdapterProxy != null)
                    {
                        networkCredentials.Proxy = callingAdapter.AdapterProxy;
                    }

                    if (callingAdapter.HasActiveCookieManager)
                    {
                        cookieContainer.CookieContainer = new CookieContainer();
                        callingAdapter.CookieManager.AddCookiesTo(cookieContainer.CookieContainer);
                    }

                    requestStream = networkCredentials.GetRequestStream();
                    int num = 1;
                    int num1 = 4096;
                    numArray = new byte[num1];
                    postStream.Seek((long)0, SeekOrigin.Begin);
                    while (num > 0)
                    {
                        num = postStream.Read(numArray, 0, num1);
                        requestStream.Write(numArray, 0, num);
                    }

                    requestStream.Close();
                    response = (HttpWebResponse)networkCredentials.GetResponse();
                    streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    end = streamReader.ReadToEnd();
                    response.Close();
                    streamReader.Close();
                    if (callingAdapter.HasActiveCookieManager && flag1)
                    {
                        Regex regex = new Regex("<html><head><title>.*</title></head>\n<body>\n<p>method=.*");
                        Regex regex1 = new Regex("<Result.*Code=\"0\".*");
                        Regex regex2 = new Regex("<li>msg=Error");
                        Match match = regex.Match(end);
                        Match match1 = regex1.Match(end);
                        Match match2 = regex2.Match(end);
                        if (!match.Success && !match1.Success || match.Success && match2.Success)
                        {
                            callingAdapter.CookieManager.UpdateCookie();
                            flag = true;
                        }
                    }

                    flag1 = false;
                } while (flag);
            }
            finally
            {
                if (callingAdapter.HasActiveCookieManager && callingAdapter.CookieManager.LockCookie)
                {
                    callingAdapter.CookieManager.ReleaseCookieLock();
                }

                requestStream = null;
                networkCredentials = null;
                numArray = null;
                response = null;
                stream = null;
                streamReader = null;
                GC.Collect();
            }

            return end;
        }

        public static void SetMetaInfoRPC(NWSAdapter callingAdapter, string sAddress, string sFileRef,
            string sRelativePath, string sMetaInfo)
        {
            string str;
            string str1;
            if (sMetaInfo == null)
            {
                sMetaInfo = "";
            }

            if (sFileRef == null || sFileRef.Length == 0)
            {
                str1 = string.Concat("service_name=", HttpUtility.UrlEncode(RPCUtil.RPCEscape(sRelativePath)));
                str = HttpUtility.UrlEncode("set service meta-info:6.0.2.6356");
            }
            else
            {
                string str2 = string.Concat(RPCUtil.RPCEscape(sRelativePath), "/", RPCUtil.RPCEscape(sFileRef));
                str1 = string.Concat("document_name=", HttpUtility.UrlEncode(str2));
                str = HttpUtility.UrlEncode("set document meta-info:6.0.2.6356");
            }

            string str3 = string.Format("method={0}&{1}&meta_info=[{2}]", str, str1, sMetaInfo);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.ASCII.GetBytes(str3), 0, str3.Length);
            memoryStream.Flush();
            string str4 = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_aut/author.dll"),
                memoryStream);
            memoryStream.Close();
            memoryStream = null;
            GC.Collect();
            if (str4.StartsWith("The underlying connection was closed: The connection was closed unexpectedly."))
            {
                throw new Exception("Setting meta info via FrontPage RPC failed");
            }

            string returnValue = RPCUtil.GetReturnValue(str4, "msg");
            if (returnValue != null)
            {
                throw new Exception(returnValue);
            }
        }

        public static string UploadDocumentUsingFrontPageRPC(NWSAdapter callingAdapter, string sAddress,
            string sPostBody, byte[] fileContents)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.ASCII.GetBytes(sPostBody), 0, sPostBody.Length);
            memoryStream.Write(fileContents, 0, (int)fileContents.Length);
            memoryStream.Flush();
            string str = null;
            str = RPCUtil.SendRequest(callingAdapter, string.Concat(sAddress, "/_vti_bin/_vti_aut/author.dll"),
                memoryStream);
            memoryStream.Close();
            memoryStream = null;
            GC.Collect();
            string returnValue = RPCUtil.GetReturnValue(str, "msg");
            if (returnValue != null)
            {
                throw new Exception(returnValue);
            }

            string returnValue1 = RPCUtil.GetReturnValue(str, "message");
            if (returnValue1 == null)
            {
                throw new Exception(string.Concat("Server error: ", str));
            }

            return returnValue1;
        }

        public static void UrlToWebUrl(NWSAdapter callingAdapter, string sUri, out string sWebUrl, out string sFileUrl)
        {
            Uri uri = new Uri(sUri);
            string str = string.Format("method=url+to+web+url:6.0.2.6356&url={0}&flags=0", uri.AbsolutePath);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
            string str1 = RPCUtil.SendRequest(callingAdapter,
                string.Concat(uri.GetLeftPart(UriPartial.Authority), "/_vti_bin/shtml.dll/_vti_rpc"), memoryStream);
            sWebUrl = RPCUtil.GetReturnValueOfUrl(str1, "webUrl");
            sFileUrl = RPCUtil.GetReturnValueOfUrl(str1, "fileUrl");
        }

        public static string ValidFileName(string s)
        {
            string str = s.Replace("&", "_26");
            str = str.Replace("~", "_7E");
            str = str.Replace("#", "_23");
            return str.Replace("{", "_7B").Replace("}", "_7D");
        }

        private static void WriteFilesXmlFromRPCResponse(XmlTextWriter writer, string response)
        {
            writer.WriteStartElement("Files");
            for (int i = response.IndexOf("<li>document_name=");
                 i >= 0;
                 i = response.IndexOf("<li>document_name=", i + 1))
            {
                string stringBetweenTags = RPCUtil.GetStringBetweenTags(response, i);
                stringBetweenTags = stringBetweenTags.Substring("document_name=".Length);
                while (stringBetweenTags.EndsWith("/"))
                {
                    stringBetweenTags = stringBetweenTags.Substring(0, stringBetweenTags.Length - 1);
                }

                writer.WriteStartElement("File");
                writer.WriteAttributeString("Url", stringBetweenTags);
                int num = stringBetweenTags.LastIndexOf("/");
                if (num >= 0)
                {
                    stringBetweenTags = stringBetweenTags.Substring(num + 1);
                }

                writer.WriteAttributeString("Name", stringBetweenTags);
                int num1 = response.IndexOf("</ul>", i);
                int num2 = response.IndexOf("<li>vti_", i);
                List<string> strs = new List<string>();
                while (num2 >= 0 && num2 < num1)
                {
                    string str = RPCUtil.GetStringBetweenTags(response, num2);
                    if (!strs.Contains(str))
                    {
                        strs.Add(str);
                        int num3 = response.IndexOf("<li>", num2 + 1);
                        string stringBetweenTags1 = RPCUtil.GetStringBetweenTags(response, num3);
                        writer.WriteAttributeString(XmlConvert.EncodeLocalName(str), stringBetweenTags1.Substring(3));
                        num2 = response.IndexOf("<li>vti_", num2 + 1);
                    }
                    else
                    {
                        num2 = response.IndexOf("<li>vti_", num2 + 1);
                    }
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void WriteFoldersXmlFromRPCResponse(XmlTextWriter writer, string response)
        {
            writer.WriteStartElement("Folders");
            Regex regex = new Regex("<li>url=(?<match1>.*)");
            Regex regex1 = new Regex("<li>SR\\|&#123;(?<match1>.*)&#125;");
            using (StringReader stringReader = new StringReader(response))
            {
                bool flag = false;
                while (true)
                {
                    string str = stringReader.ReadLine();
                    string str1 = str;
                    if (str == null)
                    {
                        break;
                    }

                    Match match = regex.Match(str1);
                    if (match.Success)
                    {
                        if (flag)
                        {
                            writer.WriteEndElement();
                        }

                        flag = true;
                        writer.WriteStartElement("Folder");
                        writer.WriteAttributeString("Name", Path.GetFileName(match.Groups["match1"].Value));
                        writer.WriteAttributeString("Url", match.Groups["match1"].Value);
                    }
                    else if (str1 == "<li>vti_listname")
                    {
                        str1 = stringReader.ReadLine();
                        writer.WriteAttributeString("ParentListId", regex1.Match(str1).Groups["match1"].Value);
                    }
                }

                if (flag)
                {
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }
    }
}