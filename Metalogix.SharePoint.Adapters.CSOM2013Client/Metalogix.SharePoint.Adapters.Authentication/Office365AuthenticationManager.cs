using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.CSOM2013Client;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Net;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class Office365AuthenticationManager : Office365WebBrowserAuthenticationManager
    {
        public Office365AuthenticationManager(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            IList<Cookie> cookies;
            lock (CookieRepository.O365SingleAccessLock)
            {
                IList<Cookie> cookies1 = CookieRepository.GetCookies(base.GetKey());
                if (cookies1 != null && !this.GetCookiesExpired(cookies1))
                {
                    cookies = cookies1;
                }
                else if (!base.HasCookie || this.GetCookiesExpired(base.Cookies))
                {
                    if (cookies1 != null)
                    {
                        foreach (Cookie cooky in cookies1)
                        {
                            cooky.Expires = DateTime.Now.AddDays(-1);
                        }
                    }

                    Cookie[] sharePointOnlineCookie = new Cookie[]
                    {
                        CSOMClientAdapter.GetSharePointOnlineCookie(base.Adapter.Url, base.Adapter.Credentials.UserName,
                            base.Adapter.Credentials.Password.ToInsecureString())
                    };
                    cookies1 = sharePointOnlineCookie;
                    CookieRepository.StoreCookies(base.GetKey(), cookies1);
                    cookies = cookies1;
                }
                else
                {
                    cookies = base.Cookies;
                }
            }

            return cookies;
        }

        private bool GetCookiesExpired(IList<Cookie> cookies)
        {
            Exception exception;
            return !WebBrowserLoginUtils.TestSharePointConnection(base.Adapter.Url, base.Adapter.AdapterProxy,
                base.Adapter.IncludedCertificates, cookies, out exception);
        }
    }
}