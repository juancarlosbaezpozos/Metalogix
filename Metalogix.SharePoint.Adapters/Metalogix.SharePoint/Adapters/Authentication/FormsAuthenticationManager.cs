using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Net;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class FormsAuthenticationManager : CookieManager
    {
        public override bool LockCookie
        {
            get { return false; }
        }

        public FormsAuthenticationManager(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            Cookie[] fBA = new Cookie[]
            {
                AuthenticationUtilities.LoginToFBA(base.Adapter.Url, base.Adapter.Credentials.NetworkCredentials,
                    base.Adapter.AdapterProxy, base.Adapter.IncludedCertificates)
            };
            return new List<Cookie>(fBA);
        }
    }
}