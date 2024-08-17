using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[BasicModeViewAllowed(true)]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RunAsync(true)]
	[SourceCardinality(Cardinality.Zero)]
	public abstract class DeleteBase : SharePointAction<SharePointActionOptions>
	{
		protected DeleteBase()
		{
		}

		protected static void DeleteSPSite(SPSite site)
		{
			if (site != null)
			{
				if (site.IsHostHeader)
				{
					((SPBaseServer)site.Parent).Sites.DeleteHostHeaderSiteCollection(site);
					return;
				}
				((SPBaseServer)site.Parent).Sites.DeleteSiteCollection(site);
			}
		}
	}
}