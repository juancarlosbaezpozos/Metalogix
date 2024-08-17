using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("2:Paste Managed Metadata Term Stores...{0-Paste}")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPServer))]
	[TargetType(typeof(SPBaseServer), true)]
	public class CopyTaxonomyFromServerAction : CopyTaxonomyAction
	{
		public CopyTaxonomyFromServerAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!(sourceSelections is ManagedMetadataServicesServerNodeCollection))
			{
				return false;
			}
			if (!base.AppliesTo(sourceSelections, targetSelections) || targetSelections.Count <= 0)
			{
				return false;
			}
			return !(targetSelections[0] is SPTenantMySiteHost);
		}
	}
}