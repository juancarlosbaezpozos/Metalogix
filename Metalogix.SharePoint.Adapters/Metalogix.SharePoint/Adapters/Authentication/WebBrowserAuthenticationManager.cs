using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Net;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class WebBrowserAuthenticationManager : CookieManager
    {
        private object m_singleAccessLock = new object();

        public override bool LockCookie
        {
            get { return false; }
        }

        public WebBrowserAuthenticationManager(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            Exception exception;
            IList<Cookie> cookies;
            lock (this.m_singleAccessLock)
            {
                if (!base.HasCookie || !WebBrowserLoginUtils.TestSharePointConnection(base.Adapter.Url,
                        base.Adapter.AdapterProxy, base.Adapter.IncludedCertificates, base.Cookies, out exception))
                {
                    cookies = AuthenticationUtilities.LoginThroughWebBrowser(base.Adapter.Url,
                        base.Adapter.AdapterProxy, base.Adapter.IncludedCertificates);
                }
                else
                {
                    cookies = base.Cookies;
                }
            }

            return cookies;
        }
    }
}