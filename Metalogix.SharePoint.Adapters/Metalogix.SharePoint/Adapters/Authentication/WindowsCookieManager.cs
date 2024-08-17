using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Net;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class WindowsCookieManager : CookieManager
    {
        private bool? m_bConnectionRequiresCookies = null;

        public override bool LockCookie
        {
            get { return false; }
        }

        public WindowsCookieManager(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            List<Cookie> cookies;
            if (!this.m_bConnectionRequiresCookies.HasValue)
            {
                this.m_bConnectionRequiresCookies =
                    new bool?(AuthenticationUtilities.TestForClaimsLoginChallenge(base.Adapter));
                if (!this.m_bConnectionRequiresCookies.Value)
                {
                    this.IsActive = false;
                }
            }

            if (!this.m_bConnectionRequiresCookies.Value)
            {
                return null;
            }

            AuthenticationUtilities.LoginWithWindowsAuthenticationThroughClaims(base.Adapter, out cookies);
            return cookies;
        }
    }
}