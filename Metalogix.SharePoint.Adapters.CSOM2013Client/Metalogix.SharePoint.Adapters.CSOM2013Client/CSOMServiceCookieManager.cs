using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Collections.Generic;
using System.Net;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client
{
    public class CSOMServiceCookieManager : CookieManager
    {
        private bool? _lockCookies = null;

        protected CSOMClientAdapter CSOMAdapter
        {
            get { return (CSOMClientAdapter)base.Adapter; }
        }

        public override bool IsActive
        {
            get { return this.CSOMAdapter.GetServiceCookieManagerIsActive(); }
            protected set { base.IsActive = value; }
        }

        public override bool LockCookie
        {
            get
            {
                if (!this._lockCookies.HasValue)
                {
                    this._lockCookies = new bool?(this.CSOMAdapter.GetServiceCookieManagerLocksCookies());
                }

                return this._lockCookies.Value;
            }
        }

        public CSOMServiceCookieManager(CSOMClientAdapter adapter) : base(adapter)
        {
        }

        protected override IList<Cookie> GetCookie()
        {
            return this.CSOMAdapter.GetServiceCookieManagerCookies();
        }

        public override void SetCookies(IList<Cookie> cookies)
        {
            this.CSOMAdapter.SetServiceCookieManagerCookies(cookies);
            base.SetCookies(cookies);
        }

        public override void UpdateCookie()
        {
            this.CSOMAdapter.UpdateServiceCookieManagerCookies();
            base.UpdateCookiesInternal();
        }
    }
}