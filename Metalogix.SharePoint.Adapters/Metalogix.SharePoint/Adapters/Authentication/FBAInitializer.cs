using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.ObjectModel;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle(EntryStyles.ForceEntry)]
    [DisallowSharePointAdapter("OM")]
    [MenuOrder(3)]
    [MenuText("Forms Based Authentication")]
    public class FBAInitializer : AuthenticationInitializer
    {
        public FBAInitializer()
        {
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            adapter.CookieManager = new FormsAuthenticationManager(adapter);
        }

        public override bool TestAuthenticationSetup(SharePointAdapter adapter)
        {
            if ((new FormsAuthenticationManager(adapter)).Cookies.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}