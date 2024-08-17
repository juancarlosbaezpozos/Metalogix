using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public static class AuthenticationUtilities
    {
        private const string UserAgent =
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; MS-RTC LM 8)";

        private static void CorrectCookieDomains(List<Cookie> cookies)
        {
            string domain = null;
            foreach (Cookie cooky in cookies)
            {
                if (cooky.Name != "FedAuth")
                {
                    continue;
                }

                domain = cooky.Domain;
                break;
            }

            foreach (Cookie cookie in cookies)
            {
                if (!string.IsNullOrEmpty(domain))
                {
                    cookie.Domain = domain;
                }

                cookie.Discard = true;
            }
        }

        private static HttpWebResponse FollowPostPath(string sPostUrl, string sPostBody,
            CookieContainer cookieContainer, string sReferer, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            string end;
            string str;
            string str1;
            HttpWebResponse httpWebResponse = AuthenticationUtilities.HttpPost(sPostUrl, sPostBody, cookieContainer,
                sReferer, webProxy, includedCertificates);
            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    end = streamReader.ReadToEnd();
                }
            }

            string loginPageError = AuthenticationUtilities.GetLoginPageError(end);
            if (!string.IsNullOrEmpty(loginPageError))
            {
                throw new Exception(loginPageError);
            }

            if (string.IsNullOrEmpty(end) ||
                end.IndexOf("<body onload=\"javascript:DoSubmit();\">", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return httpWebResponse;
            }

            AuthenticationUtilities.GetPostInstructions(end, out str, out str1);
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception("ADFS Login failed. Unable to parse DoSubmit post instructions.");
            }

            CookieContainer cookiesFromResponse = AuthenticationUtilities.GetCookiesFromResponse(httpWebResponse);
            string str2 = httpWebResponse.ResponseUri.ToString();
            return AuthenticationUtilities.FollowPostPath(str, str1, cookiesFromResponse, str2, webProxy,
                includedCertificates);
        }

        public static bool ForwardsToSharePointOnlineLoginPage(SharePointAdapter adapter)
        {
            return AuthenticationUtilities.ForwardsToSharePointOnlineLoginPage(adapter.Url, adapter.Credentials,
                adapter.AdapterProxy, adapter.IncludedCertificates);
        }

        public static bool ForwardsToSharePointOnlineLoginPage(string siteUrl, Credentials credentials,
            WebProxy webProxy, X509CertificateWrapperCollection includedCertificates)
        {
            string str;
            string str1;
            CookieContainer cookieContainer;
            string str2;
            string str3;
            bool flag;
            try
            {
                if (AuthenticationUtilities.GetForwardedToLoginPage(siteUrl, credentials, webProxy,
                        includedCertificates, out str))
                {
                    AuthenticationUtilities.GetLoginPageInfo(str, out str2, out str3, out str1, out cookieContainer,
                        webProxy, includedCertificates);
                    flag = (UrlUtils.StartsWith(str1, "https://login.microsoftonline.com")
                        ? true
                        : UrlUtils.StartsWith(str1, "http://login.microsoftonline.com"));
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

        private static string GetADFSLoginPageUrl(string sFedServiceUrl, string sFedSQ, string sLCID, string sUserName)
        {
            sFedServiceUrl = AuthenticationUtilities.InsertUrlPropVal(sFedServiceUrl, "username", sUserName);
            sFedServiceUrl = AuthenticationUtilities.InsertUrlPropVal(sFedServiceUrl, "lc", sLCID);
            sFedServiceUrl = AuthenticationUtilities.InsertUrlPropVal(sFedServiceUrl, "cbcxt", "mai");
            return string.Concat(sFedServiceUrl, '&', sFedSQ);
        }

        private static Uri GetBaseUri(string sServer)
        {
            int num = sServer.IndexOf("://", StringComparison.Ordinal);
            if (num >= 0)
            {
                num = sServer.IndexOf("/", num + 3, StringComparison.Ordinal);
                if (num >= 0)
                {
                    sServer = sServer.Substring(0, num);
                }
            }

            return new Uri(sServer);
        }

        private static CookieContainer GetCookiesFromResponse(HttpWebResponse response)
        {
            string[] values = response.Headers.GetValues("Set-Cookie");
            Uri baseUri = AuthenticationUtilities.GetBaseUri(response.ResponseUri.ToString());
            CookieContainer cookieContainer = new CookieContainer();
            string[] strArrays = values;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                int num = str.IndexOf("=", StringComparison.Ordinal);
                int num1 = str.IndexOf(";", StringComparison.Ordinal);
                if (num >= 0 && num1 >= 0 && num <= num1)
                {
                    string str1 = str.Substring(0, num);
                    string str2 = str.Substring(num + 1, num1 - num - 1);
                    Cookie cookie = new Cookie(str1, string.Concat("\"", str2, "\""));
                    cookieContainer.Add(baseUri, cookie);
                }
            }

            return cookieContainer;
        }

        private static List<Cookie> GetCookiesFromWindowsLoginPage(string sSiteUrl, Credentials credentials,
            WebProxy webProxy, X509CertificateWrapperCollection includedCertificates)
        {
            HttpWebRequest cookieContainer = (HttpWebRequest)WebRequest.Create(sSiteUrl);
            cookieContainer.CookieContainer = new CookieContainer();
            cookieContainer.AllowAutoRedirect = false;
            if (credentials != null)
            {
                cookieContainer.Credentials = credentials.NetworkCredentials;
            }

            if (webProxy != null)
            {
                cookieContainer.Proxy = webProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(cookieContainer.ClientCertificates);
            }

            HttpWebResponse response = (HttpWebResponse)cookieContainer.GetResponse();
            List<Cookie> cookies = new List<Cookie>(response.Cookies.Count);
            foreach (Cookie cooky in response.Cookies)
            {
                cookies.Add(cooky);
            }

            return cookies;
        }

        public static string GetDomainFromURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            string str = url;
            int num = str.IndexOf("://", StringComparison.Ordinal);
            if (num >= 0)
            {
                str = str.Remove(0, num + 3);
            }

            int num1 = str.IndexOf("/", StringComparison.Ordinal);
            if (num1 >= 0)
            {
                str = str.Substring(0, num1);
            }

            int num2 = str.IndexOf(":", StringComparison.Ordinal);
            if (num2 >= 0)
            {
                str = str.Substring(0, num2);
            }

            return str;
        }

        private static bool GetForwardedToLoginPage(string sPageUrl, Credentials credentials, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates, out string sLoginUrl)
        {
            HttpWebResponse response;
            HttpWebRequest networkCredentials = (HttpWebRequest)WebRequest.Create(sPageUrl);
            if (credentials != null)
            {
                networkCredentials.Credentials = credentials.NetworkCredentials;
            }

            if (webProxy != null)
            {
                networkCredentials.Proxy = webProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(networkCredentials.ClientCertificates);
            }

            try
            {
                response = (HttpWebResponse)networkCredentials.GetResponse();
            }
            catch (WebException webException1)
            {
                WebException webException = webException1;
                HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
                if (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.Forbidden ||
                    string.IsNullOrEmpty(httpWebResponse.Headers["X-FORMS_BASED_AUTH_REQUIRED"]))
                {
                    throw;
                }
                else
                {
                    response = (HttpWebResponse)webException.Response;
                }
            }

            if (string.IsNullOrEmpty(response.Headers["X-FORMS_BASED_AUTH_REQUIRED"]))
            {
                sLoginUrl = null;
                return false;
            }

            sLoginUrl = response.Headers["X-FORMS_BASED_AUTH_REQUIRED"];
            return true;
        }

        private static string GetHttpPostParameters(Dictionary<string, string> dictParametersToSet)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in dictParametersToSet)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append("&");
                }

                stringBuilder.Append(HttpUtility.UrlPathEncode(keyValuePair.Key));
                stringBuilder.Append("=");
                stringBuilder.Append(HttpUtility.UrlPathEncode(keyValuePair.Value));
            }

            return stringBuilder.ToString();
        }

        private static string GetLoginPageError(string loginResponse)
        {
            string responseProp = AuthenticationUtilities.GetResponseProp(loginResponse, "srf_sErr");
            if (string.IsNullOrEmpty(responseProp))
            {
                responseProp = null;
            }

            if (string.IsNullOrEmpty(responseProp))
            {
                string str = "<div[^>]* id=\"cta_error_message_text\"[^>]*>(?<tagValue>.*?)</div>";
                string str1 = "<[^<]+?>";
                Match match = Regex.Match(loginResponse, str, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups["tagValue"].Success)
                {
                    responseProp = Regex.Replace(match.Groups["tagValue"].Value, str1, Environment.NewLine).Trim();
                }
            }
            else
            {
                foreach (Match match1 in Regex.Matches(responseProp, "<!--.*-->"))
                {
                    responseProp = responseProp.Replace(match1.Value, "");
                }
            }

            return responseProp;
        }

        private static void GetLoginPageInfo(string sLoginPageUrl, out string sPostUrl, out string sPPFT,
            out string sReferer, out CookieContainer cookieContainer, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sLoginPageUrl);
            httpWebRequest.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; MS-RTC LM 8)";
            httpWebRequest.Method = "GET";
            if (webProxy != null)
            {
                httpWebRequest.Proxy = webProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(httpWebRequest.ClientCertificates);
            }

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            string end = (new StreamReader(response.GetResponseStream())).ReadToEnd();
            sReferer = response.ResponseUri.ToString();
            sPostUrl = AuthenticationUtilities.GetPostUrlFromResponse(end);
            sPPFT = AuthenticationUtilities.GetPPFTFromResponse(end);
            cookieContainer = AuthenticationUtilities.GetCookiesFromResponse(response);
        }

        private static void GetPostInstructions(HttpWebResponse response, out string sPostUrl,
            out CookieContainer cookies, out string sPostBody, out string sResponsePageOut)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    sResponsePageOut = streamReader.ReadToEnd();
                }
            }

            cookies = AuthenticationUtilities.GetCookiesFromResponse(response);
            AuthenticationUtilities.GetPostInstructions(sResponsePageOut, out sPostUrl, out sPostBody);
        }

        private static void GetPostInstructions(string sResponsePage, out string sPostUrl, out string sPostBody)
        {
            sPostUrl = null;
            sPostBody = null;
            sPostUrl = AuthenticationUtilities.GetResponseProp(sResponsePage, " action");
            int num = sResponsePage.IndexOf("<input ", StringComparison.Ordinal);
            Dictionary<string, string> strs = new Dictionary<string, string>();
            while (num >= 0)
            {
                sResponsePage = sResponsePage.Substring(num);
                string responseProp = AuthenticationUtilities.GetResponseProp(sResponsePage, " name");
                string str =
                    HttpUtility.UrlEncode(
                        HttpUtility.HtmlDecode(AuthenticationUtilities.GetResponseProp(sResponsePage, " value")));
                if (!string.IsNullOrEmpty(responseProp))
                {
                    strs.Add(responseProp, str);
                }

                num = sResponsePage.IndexOf("<input ", 1, StringComparison.Ordinal);
            }

            sPostBody = AuthenticationUtilities.GetHttpPostParameters(strs);
        }

        private static string GetPostUrlFromResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException("response");
            }

            string responseProp = AuthenticationUtilities.GetResponseProp(response, "srf_uPost");
            if (string.IsNullOrEmpty(responseProp))
            {
                string str = "<form[^>]* id=\"credentials\"[^>]*>";
                string str1 = " action=\"(?<actionAttrVal>[^\"]*)\"";
                Match match = Regex.Match(response, str, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Match match1 = Regex.Match(match.Value, str1, RegexOptions.IgnoreCase);
                    if (match1.Groups["actionAttrVal"].Success)
                    {
                        responseProp = match1.Groups["actionAttrVal"].Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(responseProp))
            {
                Exception exception =
                    new Exception(
                        "Unable to obtain Post Url from SharePoint Online Login Page [GetPostUrlFromResponse()]");
                exception.Data.Add("HTMLResponse", response);
                throw exception;
            }

            return responseProp;
        }

        private static string GetPPFTFromResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException("response");
            }

            string responseProp = AuthenticationUtilities.GetResponseProp(response, "srf_sFT");
            if (string.IsNullOrEmpty(responseProp))
            {
                string str = "<input[^>]* name=\"+PPFT+\"[^>]*/>";
                string str1 = " value=\"(?<valueAttrVal>[^\"]*)\"";
                Match match = Regex.Match(response, str, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Match match1 = Regex.Match(match.Value, str1, RegexOptions.IgnoreCase);
                    if (match1.Groups["valueAttrVal"].Success)
                    {
                        responseProp = match1.Groups["valueAttrVal"].Value;
                    }
                }
            }
            else
            {
                responseProp = AuthenticationUtilities.GetResponseProp(responseProp, "value");
            }

            if (string.IsNullOrEmpty(responseProp))
            {
                Exception exception =
                    new Exception("Unable to obtain PPFT from SharePoint Online Login Page [GetPPFTFromResponse()]");
                exception.Data.Add("HTMLResponse", response);
                throw exception;
            }

            return responseProp;
        }

        private static string GetResponseProp(string sResponse, string sProp)
        {
            sProp = string.Concat(sProp, "=");
            int length = sResponse.IndexOf(sProp, StringComparison.InvariantCultureIgnoreCase);
            if (length >= 0)
            {
                length += sProp.Length;
                char chr = sResponse[length];
                int num = sResponse.IndexOf(chr, length + 1);
                if (num >= 0)
                {
                    return sResponse.Substring(length + 1, num - length - 1);
                }
            }

            return null;
        }

        private static HttpWebResponse HttpPost(string uri, string parameters, CookieContainer cookieContainer,
            string sReferer, WebProxy webProxy, X509CertificateWrapperCollection includedCertificates)
        {
            HttpWebResponse response;
            HttpWebRequest length = (HttpWebRequest)WebRequest.Create(uri);
            length.Method = "POST";
            if (!string.IsNullOrEmpty(sReferer))
            {
                length.Referer = sReferer;
            }

            length.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; MS-RTC LM 8)";
            length.ContentType = "application/x-www-form-urlencoded";
            length.CookieContainer = cookieContainer;
            length.AllowAutoRedirect = false;
            if (webProxy != null)
            {
                length.Proxy = webProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(length.ClientCertificates);
            }

            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            Stream requestStream = null;
            try
            {
                try
                {
                    length.ContentLength = (long)((int)bytes.Length);
                    requestStream = length.GetRequestStream();
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
                response = length.GetResponse() as HttpWebResponse;
            }
            catch (WebException webException1)
            {
                return null;
            }

            return response;
        }

        private static string InsertUrlPropVal(string sUrl, string sPropName, string sValue)
        {
            int num = sUrl.IndexOf(string.Concat("&", sPropName), StringComparison.InvariantCultureIgnoreCase);
            if (num < 0)
            {
                num = sUrl.IndexOf(string.Concat("?", sPropName), StringComparison.InvariantCultureIgnoreCase);
                if (num < 0)
                {
                    return sUrl;
                }
            }

            num = sUrl.IndexOf('=', num);
            if (num < 0)
            {
                return sUrl;
            }

            if (num == sUrl.Length - 1)
            {
                return string.Concat(sUrl, HttpUtility.UrlEncode(sValue));
            }

            return sUrl.Insert(num + 1, HttpUtility.UrlEncode(sValue));
        }

        public static Cookie[] LoginThroughWebBrowser(string sWebUrl, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            if (UrlUtils.GetType(sWebUrl) != UrlType.Full)
            {
                throw new ArgumentException(string.Format(Resources.Not_A_Valid_Absolute_URL, sWebUrl));
            }

            AuthenticationUtilities.CookieValidationOptions cookieValidationOption =
                new AuthenticationUtilities.CookieValidationOptions(sWebUrl, webProxy, includedCertificates);
            Thread thread = new Thread(new ParameterizedThreadStart(AuthenticationUtilities.LoginThroughWebBrowser));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(cookieValidationOption);
            thread.Join();
            if (cookieValidationOption.Exception != null)
            {
                throw cookieValidationOption.Exception;
            }

            return cookieValidationOption.Cookies;
        }

        private static void LoginThroughWebBrowser(object oValidationOptions)
        {
            Exception exception;
            AuthenticationUtilities.CookieValidationOptions lastAttemptException =
                oValidationOptions as AuthenticationUtilities.CookieValidationOptions;
            if (lastAttemptException == null)
            {
                throw new ArgumentException("The parameter is not of the type CookieValidationOptions",
                    "oValidationOptions");
            }

            string webUrl = lastAttemptException.WebUrl;
            WebProxy proxy = lastAttemptException.Proxy;
            X509CertificateWrapperCollection includedCertificates = lastAttemptException.IncludedCertificates;
            Cookie[] cookieArray =
                WebBrowserLoginUtils.ReadBrowserCookies(webUrl, WebBrowserLoginUtils.GetInternetExplorerVersion());
            if (WebBrowserLoginUtils.TestSharePointConnection(webUrl, proxy, includedCertificates, cookieArray,
                    out exception))
            {
                lastAttemptException.Cookies = cookieArray;
                return;
            }

            AuthenticationWebBrowser authenticationWebBrowser =
                new AuthenticationWebBrowser(webUrl, proxy, includedCertificates);
            if (authenticationWebBrowser.ShowDialog() != DialogResult.OK)
            {
                if (authenticationWebBrowser.LastAttemptException == null)
                {
                    lastAttemptException.Exception = new Exception(Resources.No_Connections_Attempted);
                }
                else
                {
                    lastAttemptException.Exception = authenticationWebBrowser.LastAttemptException;
                }
            }

            if (authenticationWebBrowser.Cookies == null)
            {
                lastAttemptException.Cookies = new Cookie[0];
                return;
            }

            cookieArray = new Cookie[authenticationWebBrowser.Cookies.Count];
            authenticationWebBrowser.Cookies.CopyTo(cookieArray, 0);
            lastAttemptException.Cookies = cookieArray;
        }

        public static Cookie LoginToFBA(string sWebUrl, ICredentials credentials, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            Metalogix.SharePoint.Adapters.Authentication.Authentication authentication =
                new Metalogix.SharePoint.Adapters.Authentication.Authentication();
            char[] chrArray = new char[] { '/' };
            authentication.Url = string.Concat(sWebUrl.Trim(chrArray), "/_vti_bin/Authentication.asmx");
            authentication.CookieContainer = new CookieContainer();
            if (webProxy != null)
            {
                authentication.Proxy = webProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(authentication.ClientCertificates);
            }

            string userName = credentials.GetCredential(new Uri(sWebUrl), "Forms").UserName;
            string password = credentials.GetCredential(new Uri(sWebUrl), "Forms").Password;
            LoginResult loginResult = authentication.Login(userName, password);
            if (loginResult.ErrorCode != LoginErrorCode.NoError)
            {
                throw new Exception(string.Concat("FBA login failed. Error code: ", loginResult.ErrorCode.ToString()));
            }

            CookieCollection cookies = authentication.CookieContainer.GetCookies(new Uri(authentication.Url));
            return cookies[loginResult.CookieName];
        }

        public static List<Cookie> LoginToSharePointOnline(SharePointAdapter adapter)
        {
            return AuthenticationUtilities.LoginToSharePointOnline(adapter.Url, adapter.Credentials,
                adapter.AdapterProxy, adapter.IncludedCertificates);
        }

        public static List<Cookie> LoginToSharePointOnline(string sSiteUrl, Credentials credentials, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            string str;
            string str1;
            CookieContainer cookieContainer;
            string str2;
            string str3;
            if (!AuthenticationUtilities.GetForwardedToLoginPage(sSiteUrl, credentials, webProxy, includedCertificates,
                    out str))
            {
                throw new Exception("Failed to login to page. Page did not forward to the login page.");
            }

            AuthenticationUtilities.GetLoginPageInfo(str, out str2, out str3, out str1, out cookieContainer, webProxy,
                includedCertificates);
            List<Cookie> loginUrl = AuthenticationUtilities.PostToLoginUrl(str2, credentials.UserName,
                credentials.Password.ToInsecureString(), str1, str3, cookieContainer, webProxy, includedCertificates);
            AuthenticationUtilities.CorrectCookieDomains(loginUrl);
            return loginUrl;
        }

        public static bool LoginWithWindowsAuthenticationThroughClaims(SharePointAdapter adapter,
            out List<Cookie> cookies)
        {
            return AuthenticationUtilities.LoginWithWindowsAuthenticationThroughClaims(adapter.Url, adapter.Credentials,
                adapter.AdapterProxy, adapter.IncludedCertificates, out cookies);
        }

        public static bool LoginWithWindowsAuthenticationThroughClaims(string sSiteUrl, Credentials credentials,
            WebProxy webProxy, X509CertificateWrapperCollection includedCertificates, out List<Cookie> cookies)
        {
            string str;
            cookies = null;
            if (!AuthenticationUtilities.GetForwardedToLoginPage(sSiteUrl, credentials, webProxy, includedCertificates,
                    out str))
            {
                return false;
            }

            string str1 = str;
            int num = str1.IndexOf("/_login/", StringComparison.Ordinal);
            if (num < 0)
            {
                return false;
            }

            str1 = string.Concat((num > 0 ? str1.Substring(0, num) : ""), "/_windows/default.aspx?ReturnUrl=%2f");
            cookies = AuthenticationUtilities.GetCookiesFromWindowsLoginPage(str1, credentials, webProxy,
                includedCertificates);
            if (cookies == null || cookies.Count == 0)
            {
                return false;
            }

            AuthenticationUtilities.CorrectCookieDomains(cookies);
            return true;
        }

        public static Cookie MakeCookie(string cookie, string siteURLToUseCookieOn)
        {
            //if(string.IsNullOrEmpty(cookie))
            //{
            //    throw new ArgumentNullException("cookie");
            //}

            //char[] chrArray = new char[] { '=' };
            //string[] strArrays = cookie.Split(chrArray, 2);
            //return new Cookie(strArrays[0], strArrays[1], "/",
            //    AuthenticationUtilities.GetDomainFromURL(siteURLToUseCookieOn));

            return new Cookie();
        }

        private static List<Cookie> PostToADFSLoginUrl(string sADFSLoginPage, string sUserName, string sPassword,
            WebProxy webProxy, X509CertificateWrapperCollection includedCertificates)
        {
            HttpWebResponse response;
            string str;
            string str1;
            CookieContainer cookieContainer;
            string str2;
            HttpWebRequest networkCredential = (HttpWebRequest)WebRequest.Create(sADFSLoginPage);
            networkCredential.Credentials = new NetworkCredential(sUserName, sPassword);
            networkCredential.Method = "GET";
            networkCredential.KeepAlive = true;
            networkCredential.PreAuthenticate = true;
            networkCredential.Proxy = webProxy;
            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(networkCredential.ClientCertificates);
            }

            try
            {
                response = (HttpWebResponse)networkCredential.GetResponse();
            }
            catch (WebException webException)
            {
                response = webException.Response as HttpWebResponse;
                if (response == null)
                {
                    throw;
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                networkCredential = (HttpWebRequest)WebRequest.Create(response.ResponseUri);
                networkCredential.KeepAlive = true;
                networkCredential.Method = "GET";
                networkCredential.Credentials = new NetworkCredential(sUserName, sPassword);
                networkCredential.PreAuthenticate = true;
                networkCredential.Proxy = webProxy;
                if (includedCertificates != null)
                {
                    includedCertificates.CopyCertificatesToCollection(networkCredential.ClientCertificates);
                }

                response = (HttpWebResponse)networkCredential.GetResponse();
            }

            string str3 = response.ResponseUri.ToString();
            AuthenticationUtilities.GetPostInstructions(response, out str, out cookieContainer, out str1, out str2);
            if (string.IsNullOrEmpty(str))
            {
                string loginPageError = AuthenticationUtilities.GetLoginPageError(str2);
                if (string.IsNullOrEmpty(loginPageError))
                {
                    throw new Exception("Login failed. Post back from ADFS server failed.");
                }

                throw new Exception(loginPageError);
            }

            response = AuthenticationUtilities.FollowPostPath(str, str1, cookieContainer, str3, webProxy,
                includedCertificates);
            List<Cookie> cookies = new List<Cookie>();
            foreach (Cookie cooky in response.Cookies)
            {
                if (string.IsNullOrEmpty(cooky.Value))
                {
                    continue;
                }

                cookies.Add(cooky);
            }

            if (cookies.Count == 0)
            {
                throw new Exception("ADFS Login Failed. No cookie was returned.");
            }

            return cookies;
        }

        private static List<Cookie> PostToLoginUrl(string sPostUrl, string sUserName, string sPassword, string sReferer,
            string sPPFT, CookieContainer cookieContainer, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            string str;
            string str1;
            CookieContainer cookieContainer1;
            string str2;
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "login", HttpUtility.UrlEncode(sUserName) },
                { "passwd", HttpUtility.UrlEncode(sPassword) },
                { "LoginOptions", "2" },
                { "PPSX", "Pass" },
                { "PPFT", HttpUtility.UrlEncode(sPPFT) },
                { "PwdPad", "IfYouAreReadingThisYouHaveTooMuc" }
            };
            string httpPostParameters = AuthenticationUtilities.GetHttpPostParameters(strs);
            HttpWebResponse httpWebResponse = AuthenticationUtilities.HttpPost(sPostUrl, httpPostParameters,
                cookieContainer, sReferer, webProxy, includedCertificates);
            string str3 = httpWebResponse.ResponseUri.ToString();
            AuthenticationUtilities.GetPostInstructions(httpWebResponse, out str, out cookieContainer1, out str1,
                out str2);
            if (string.IsNullOrEmpty(str))
            {
                string loginPageError = AuthenticationUtilities.GetLoginPageError(str2);
                if (!string.IsNullOrEmpty(loginPageError))
                {
                    throw new Exception(loginPageError);
                }

                string responseProp = AuthenticationUtilities.GetResponseProp(str2, "srf_sFedURL");
                if (string.IsNullOrEmpty(responseProp))
                {
                    throw new Exception("Login failed. Post to login page failed.");
                }

                string responseProp1 = AuthenticationUtilities.GetResponseProp(str2, "srf_sFedQS");
                string responseProp2 = AuthenticationUtilities.GetResponseProp(str2, "srf_sLCID");
                string aDFSLoginPageUrl =
                    AuthenticationUtilities.GetADFSLoginPageUrl(responseProp, responseProp1, responseProp2, sUserName);
                return AuthenticationUtilities.PostToADFSLoginUrl(aDFSLoginPageUrl, sUserName, sPassword, webProxy,
                    includedCertificates);
            }

            HttpWebResponse httpWebResponse1 =
                AuthenticationUtilities.FollowPostPath(str, str1, cookieContainer1, str3, webProxy,
                    includedCertificates);
            List<Cookie> cookies = new List<Cookie>();
            CookieCollection cookieCollections = null;
            string item = httpWebResponse1.Headers["Location"];
            if (httpWebResponse1.Cookies != null && httpWebResponse1.Cookies["DcCTX"] != null &&
                !string.IsNullOrEmpty(item) && item != "/")
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(item);
                httpWebRequest.Method = "GET";
                httpWebRequest.Referer = sPostUrl;
                httpWebRequest.UserAgent =
                    "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; MS-RTC LM 8)";
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.CookieContainer.Add(httpWebResponse.Cookies);
                if (webProxy != null)
                {
                    httpWebRequest.Proxy = webProxy;
                }

                if (includedCertificates != null)
                {
                    includedCertificates.CopyCertificatesToCollection(httpWebRequest.ClientCertificates);
                }

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                Uri baseUri = AuthenticationUtilities.GetBaseUri(item);
                cookieCollections = httpWebRequest.CookieContainer.GetCookies(baseUri);
            }
            else if (httpWebResponse1 != null)
            {
                cookieCollections = httpWebResponse1.Cookies;
            }

            if (cookieCollections != null)
            {
                foreach (Cookie cooky in cookieCollections)
                {
                    if (string.IsNullOrEmpty(cooky.Value))
                    {
                        continue;
                    }

                    cookies.Add(cooky);
                }
            }

            if (cookies.Count == 0)
            {
                throw new Exception("Login Failed. No cookie was returned.");
            }

            return cookies;
        }

        public static bool TestForClaimsLoginChallenge(SharePointAdapter adapter)
        {
            return AuthenticationUtilities.TestForClaimsLoginChallenge(adapter.Url, adapter.Credentials,
                adapter.AdapterProxy, adapter.IncludedCertificates);
        }

        public static bool TestForClaimsLoginChallenge(string sSiteUrl, Credentials credentials, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            string str;
            bool flag;
            try
            {
                flag = (AuthenticationUtilities.GetForwardedToLoginPage(sSiteUrl, credentials, webProxy,
                    includedCertificates, out str)
                    ? !string.IsNullOrEmpty(str)
                    : false);
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        private class CookieValidationOptions
        {
            private string m_sWebUrl;

            private WebProxy m_webProxy;

            private X509CertificateWrapperCollection m_includedCertificates;

            private Cookie[] m_cookies;

            private Exception m_ex;

            public Cookie[] Cookies
            {
                get { return this.m_cookies; }
                set { this.m_cookies = value; }
            }

            public Exception Exception
            {
                get { return this.m_ex; }
                set { this.m_ex = value; }
            }

            public X509CertificateWrapperCollection IncludedCertificates
            {
                get { return this.m_includedCertificates; }
            }

            public WebProxy Proxy
            {
                get { return this.m_webProxy; }
            }

            public string WebUrl
            {
                get { return this.m_sWebUrl; }
            }

            public CookieValidationOptions(string sWebUrl, WebProxy proxy,
                X509CertificateWrapperCollection includedCertificates)
            {
                this.m_sWebUrl = sWebUrl;
                this.m_webProxy = proxy;
                this.m_includedCertificates = includedCertificates;
            }
        }
    }
}