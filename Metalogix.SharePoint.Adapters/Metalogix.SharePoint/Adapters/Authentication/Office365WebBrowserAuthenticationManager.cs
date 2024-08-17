using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class Office365WebBrowserAuthenticationManager : CookieManager
    {
        private static bool? bLockCookie;

        public override object CookieLockObject
        {
            get
            {
                object cookieLock = CookieRepository.GetCookieLock(this.GetKey());
                if (cookieLock == null)
                {
                    ReadOnlyCollection<Cookie> cookies = base.Cookies;
                    cookieLock = CookieRepository.GetCookieLock(this.GetKey());
                }

                return cookieLock;
            }
        }

        public override bool LockCookie
        {
            get
            {
                if (!Office365WebBrowserAuthenticationManager.bLockCookie.HasValue)
                {
                    Office365WebBrowserAuthenticationManager.bLockCookie =
                        new bool?(!AdapterConfigurationVariables.EnableConcurrentNWSOffice365Connections);
                }

                return Office365WebBrowserAuthenticationManager.bLockCookie.Value;
            }
        }

        public override bool UsesCookieRepository
        {
            get { return true; }
        }

        static Office365WebBrowserAuthenticationManager()
        {
            Office365WebBrowserAuthenticationManager.bLockCookie = null;
        }

        public Office365WebBrowserAuthenticationManager(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            Exception exception;
            Exception exception1;
            IList<Cookie> cookies;
            lock (CookieRepository.O365SingleAccessLock)
            {
                IList<Cookie> cookies1 = CookieRepository.GetCookies(this.GetKey());
                if (cookies1 != null)
                {
                    if (WebBrowserLoginUtils.TestSharePointConnection(base.Adapter.Url, base.Adapter.AdapterProxy,
                            base.Adapter.IncludedCertificates, cookies1, out exception))
                    {
                        cookies = cookies1;
                        return cookies;
                    }
                }
                else if (base.HasCookie && WebBrowserLoginUtils.TestSharePointConnection(base.Adapter.Url,
                             base.Adapter.AdapterProxy, base.Adapter.IncludedCertificates, base.Cookies,
                             out exception1))
                {
                    cookies = base.Cookies;
                    return cookies;
                }

                cookies1 = new List<Cookie>(AuthenticationUtilities.LoginThroughWebBrowser(base.Adapter.Url,
                    base.Adapter.AdapterProxy, base.Adapter.IncludedCertificates));
                CookieRepository.StoreCookies(this.GetKey(), cookies1);
                cookies = cookies1;
            }

            return cookies;
        }

        protected string GetKey()
        {
            if (base.Adapter == null)
            {
                return "No Adapter Yet";
            }

            return CookieRepository.GetKey(base.Adapter);
        }
    }
}