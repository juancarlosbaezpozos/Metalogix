using Metalogix.DataStructures;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.SiteData;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public static class WebBrowserLoginUtils
    {
        private const int INTERNET_COOKIE_HTTPONLY = 8192;

        private static string GetCookies(string sUrl, Version internetExplorerVersion)
        {
            string str;
            int num = (AdapterConfigurationVariables.AllowIE7WebBrowserAuthentication ? 7 : 8);
            if (internetExplorerVersion.Major < num)
            {
                throw new InsufficentIEVersionException(internetExplorerVersion.Major, num);
            }

            int num1 = 256;
            StringBuilder stringBuilder = new StringBuilder(num1);
            try
            {
                if (!WebBrowserLoginUtils.NativeMethods.InternetGetCookieEx(sUrl, null, stringBuilder, ref num1, 8192,
                        IntPtr.Zero))
                {
                    if (num1 >= 0)
                    {
                        stringBuilder = new StringBuilder(num1);
                        if (!WebBrowserLoginUtils.NativeMethods.InternetGetCookieEx(sUrl, null, stringBuilder, ref num1,
                                8192, IntPtr.Zero))
                        {
                            str = null;
                            return str;
                        }
                    }
                    else
                    {
                        str = null;
                        return str;
                    }
                }

                return stringBuilder.ToString();
            }
            catch
            {
                throw new Exception("Failed to get cookies");
            }

            return str;
        }

        public static Version GetInternetExplorerVersion()
        {
            return (new WebBrowser()).Version;
        }

        public static Cookie[] ReadBrowserCookies(string sSiteUrl, Version internetExplorerVersion)
        {
            string cookies = WebBrowserLoginUtils.GetCookies(sSiteUrl, internetExplorerVersion);
            if (string.IsNullOrEmpty(cookies))
            {
                return new Cookie[0];
            }

            string domainFromURL = AuthenticationUtilities.GetDomainFromURL(sSiteUrl);
            string[] strArrays = new string[] { "; " };
            string[] strArrays1 = cookies.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
            List<Cookie> cookies1 = new List<Cookie>((int)strArrays1.Length);
            string[] strArrays2 = strArrays1;
            for (int i = 0; i < (int)strArrays2.Length; i++)
            {
                string str = strArrays2[i];
                char[] chrArray = new char[] { '=' };
                string[] strArrays3 = str.Split(chrArray, 2, StringSplitOptions.None);
                if ((int)strArrays3.Length == 2)
                {
                    string str1 = strArrays3[0];
                    string str2 = strArrays3[1];
                    cookies1.Add(new Cookie(str1, str2, "/", domainFromURL));
                }
            }

            return cookies1.ToArray();
        }

        public static bool TestSharePointConnection(string sSiteUrl, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates, IEnumerable<Cookie> cookies,
            out Exception failureException)
        {
            string str;
            string[] strArrays;
            string[] strArrays1;
            bool flag;
            failureException = null;
            try
            {
                Metalogix.SharePoint.Adapters.SiteData.SiteData siteDatum =
                    new Metalogix.SharePoint.Adapters.SiteData.SiteData();
                if (webProxy != null)
                {
                    siteDatum.Proxy = webProxy;
                }

                if (includedCertificates != null)
                {
                    includedCertificates.CopyCertificatesToCollection(siteDatum.ClientCertificates);
                }

                string url = siteDatum.Url;
                string str1 = string.Concat(sSiteUrl, "/");
                siteDatum.Url = (new Regex("http://[^/]*/")).Replace(url, str1);
                siteDatum.Timeout = 10000;
                siteDatum.Credentials = CredentialCache.DefaultCredentials;
                if (cookies != null)
                {
                    siteDatum.CookieContainer = new CookieContainer();
                    foreach (Cookie cooky in cookies)
                    {
                        siteDatum.CookieContainer.Add(cooky);
                    }
                }

                _sWebMetadata _sWebMetadatum = null;
                _sWebWithTime[] _sWebWithTimeArray = null;
                _sListWithTime[] _sListWithTimeArray = null;
                _sFPUrl[] _sFPUrlArray = null;
                siteDatum.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray, out _sListWithTimeArray, out _sFPUrlArray,
                    out str, out strArrays, out strArrays1);
                flag = true;
            }
            catch (Exception exception)
            {
                failureException = exception;
                flag = false;
            }

            return flag;
        }

        internal static class NativeMethods
        {
            [DllImport("wininet.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
            public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData,
                ref int size, int flags, IntPtr pReserved);
        }
    }
}