using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [Metalogix.SharePoint.Adapters.Authentication.AutomaticAuthenticationEnabled(false)]
    [Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle(EntryStyles.None)]
    [DisallowSharePointAdapter("OM")]
    [MenuOrder(6)]
    [MenuText("Office 365 Web Browser Authentication (Not Auto Detected)")]
    public class Office365WebBrowserInitializer : AuthenticationInitializer
    {
        public Office365WebBrowserInitializer()
        {
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            adapter.CookieManager = new Office365WebBrowserAuthenticationManager(adapter);
        }
    }
}