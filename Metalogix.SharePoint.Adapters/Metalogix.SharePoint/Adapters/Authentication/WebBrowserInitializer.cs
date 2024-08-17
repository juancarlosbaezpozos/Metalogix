using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [Metalogix.SharePoint.Adapters.Authentication.AutomaticAuthenticationEnabled(false)]
    [Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle(EntryStyles.None)]
    [DisallowSharePointAdapter("OM")]
    [MenuOrder(5)]
    [MenuText("Web Browser Authentication (Not Auto Detected)")]
    public class WebBrowserInitializer : AuthenticationInitializer
    {
        public WebBrowserInitializer()
        {
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            adapter.CookieManager = new WebBrowserAuthenticationManager(adapter);
        }
    }
}