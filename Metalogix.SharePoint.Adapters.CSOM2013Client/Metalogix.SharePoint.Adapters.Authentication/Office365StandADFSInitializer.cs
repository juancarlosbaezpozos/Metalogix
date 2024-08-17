using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.ObjectModel;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AutoDetectPriority(2)]
    [Metalogix.SharePoint.Adapters.Authentication.CredentialEntryStyle(EntryStyles.ForceEntry)]
    [DisallowSharePointAdapter("OM")]
    [IsDefaultO365Authenticator(true)]
    [MenuOrder(4)]
    [MenuText("Office 365 Standard/ADFS Authentication")]
    public class Office365StandADFSInitializer : AuthenticationInitializer
    {
        public Office365StandADFSInitializer()
        {
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            adapter.CookieManager = new Office365AuthenticationManager(adapter);
        }

        public override bool TestAuthenticationSetup(SharePointAdapter adapter)
        {
            if ((new Office365AuthenticationManager(adapter)).Cookies.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}