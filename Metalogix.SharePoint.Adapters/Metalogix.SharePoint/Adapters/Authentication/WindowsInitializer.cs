using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.SiteData;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [MenuOrder(2)]
    [MenuText("Windows Authentication")]
    public class WindowsInitializer : AuthenticationInitializer
    {
        public WindowsInitializer()
        {
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            adapter.CookieManager = new WindowsCookieManager(adapter);
        }

        public override bool TestAuthenticationSetup(SharePointAdapter adapter)
        {
            string str;
            string[] strArrays;
            string[] strArrays1;
            bool flag;
            WindowsCookieManager windowsCookieManager = new WindowsCookieManager(adapter);
            try
            {
                if (windowsCookieManager == null || windowsCookieManager.Cookies.Count == 0)
                {
                    Regex regex = new Regex("http://[^/]*/");
                    Metalogix.SharePoint.Adapters.SiteData.SiteData siteDatum =
                        new Metalogix.SharePoint.Adapters.SiteData.SiteData();
                    if (adapter.AdapterProxy != null)
                    {
                        siteDatum.Proxy = adapter.AdapterProxy;
                    }

                    if (adapter.IncludedCertificates != null)
                    {
                        adapter.IncludedCertificates.CopyCertificatesToCollection(siteDatum.ClientCertificates);
                    }

                    string url = siteDatum.Url;
                    string str1 = string.Concat(adapter.Url, "/");
                    siteDatum.Url = regex.Replace(url, str1);
                    siteDatum.Credentials = adapter.Credentials.NetworkCredentials;
                    siteDatum.Timeout = 10000;
                    _sWebMetadata _sWebMetadatum = null;
                    _sWebWithTime[] _sWebWithTimeArray = null;
                    _sListWithTime[] _sListWithTimeArray = null;
                    _sFPUrl[] _sFPUrlArray = null;
                    siteDatum.GetWeb(out _sWebMetadatum, out _sWebWithTimeArray, out _sListWithTimeArray,
                        out _sFPUrlArray, out str, out strArrays, out strArrays1);
                    return true;
                }
                else
                {
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }
    }
}