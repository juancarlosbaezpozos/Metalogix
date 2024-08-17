using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Metalogix.Actions.AllowsSameSourceTarget(false)]
	[MenuText("1:Paste Tenant Managed Metadata... {0-Paste}")]
	[Name("Paste Tenant Managed Metadata")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPTenant))]
	[TargetType(typeof(SPTenant))]
	public class PasteTaxonomyFromTenantAction : CopyTaxonomyAction
	{
		public PasteTaxonomyFromTenantAction()
		{
		}
	}
}