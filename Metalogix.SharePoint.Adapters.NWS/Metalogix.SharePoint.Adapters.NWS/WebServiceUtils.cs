using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.NWS.SiteData;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS
{
    public class WebServiceUtils
    {
        public WebServiceUtils()
        {
        }

        public static void TestRemoteSharepoint(SharePointAdapter adapter, out string sSite, out string sWeb)
        {
            Regex regex = new Regex("http://[^/]*/");
            if (adapter.HasActiveCookieManager && adapter.CookieManager.LockCookie)
            {
                adapter.CookieManager.AquireCookieLock();
            }

            try
            {
                Metalogix.SharePoint.Adapters.NWS.SiteData.SiteData siteDatum =
                    new Metalogix.SharePoint.Adapters.NWS.SiteData.SiteData();
                if (adapter.AdapterProxy != null)
                {
                    siteDatum.Proxy = adapter.AdapterProxy;
                }

                adapter.IncludedCertificates.CopyCertificatesToCollection(siteDatum.ClientCertificates);
                string url = siteDatum.Url;
                string str = string.Concat(adapter.Url, "/");
                str = regex.Replace(url, str);
                siteDatum.Url = str;
                siteDatum.Credentials = adapter.Credentials.NetworkCredentials;
                siteDatum.Timeout = 10000;
                if (adapter.HasActiveCookieManager)
                {
                    siteDatum.CookieContainer = new CookieContainer();
                    adapter.CookieManager.AddCookiesTo(siteDatum.CookieContainer);
                }

                string str1 = null;
                string str2 = str.Substring(adapter.Url.Length);
                try
                {
                    siteDatum.GetSiteAndWeb(adapter.Url, out sSite, out sWeb);
                }
                catch (WebException webException)
                {
                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    if (!Utils.ResponseIsRedirect(response))
                    {
                        throw;
                    }
                    else
                    {
                        string item = response.Headers["Location"];
                        if (item == null)
                        {
                            throw;
                        }

                        if (!item.EndsWith(str2))
                        {
                            throw;
                        }

                        str1 = item;
                        sSite = null;
                        sWeb = null;
                    }
                }

                if (str1 != null)
                {
                    string str3 = str1.Substring(0, str1.Length - str2.Length);
                    siteDatum.Url = str1;
                    siteDatum.GetSiteAndWeb(str3, out sSite, out sWeb);
                    adapter.SetUrlForRedirect(str3);
                }
            }
            finally
            {
                if (adapter.HasActiveCookieManager && adapter.CookieManager.LockCookie)
                {
                    adapter.CookieManager.ReleaseCookieLock();
                }
            }
        }
    }
}