using Microsoft.SharePoint;
using System;
using System.Web;

namespace Metalogix.SharePoint.Adapters.OM
{
    internal sealed class OMAdapterHttpContext : IDisposable
    {
        private bool _bContextDidExist;

        private bool _bExistingContextMatchCurrentSite;

        private object _origHttpContext;

        public OMAdapterHttpContext(SPWeb currentWeb)
        {
            this._bContextDidExist = HttpContext.Current != null;
            this._bExistingContextMatchCurrentSite =
                (!this._bContextDidExist || SPContext.Current == null || currentWeb.Site.Url == null
                    ? false
                    : currentWeb.Site.Url.ToLowerInvariant() == SPContext.Current.Site.Url.ToLowerInvariant());
            this._origHttpContext = HttpContext.Current;
            if (!this._bContextDidExist || !this._bExistingContextMatchCurrentSite)
            {
                HttpContext.Current =
                    FakeSharePointWorkerRequest.GetFakeHttpContextForSharePoint(currentWeb,
                        !this._bExistingContextMatchCurrentSite);
            }

            currentWeb.AllowUnsafeUpdates = (true);
        }

        public void Dispose()
        {
            if (!this._bContextDidExist || !this._bExistingContextMatchCurrentSite)
            {
                HttpContext.Current = (HttpContext)this._origHttpContext;
            }
        }
    }
}