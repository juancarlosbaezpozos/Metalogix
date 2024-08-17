using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.CSOM2013.Authentication;
using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
	public class Office365StandADFSInitializer : AuthenticationInitializer
	{
		public Office365StandADFSInitializer()
		{
		}

		protected override void SpecializedInitActions(SharePointAdapter adapter)
		{
			adapter.CookieManager = new O365CSOMAuthenticationManager(adapter);
		}
	}
}