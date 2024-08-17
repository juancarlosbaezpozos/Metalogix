using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[LaunchAsJob(false)]
	[MenuText("Create Site Collection... {2-Create} > 2:Self Service Mode...")]
	[Name("Create Site Collection - Self Service Mode")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class CreateSiteCollectionSelfServiceMode : CreateSiteCollection
	{
		public CreateSiteCollectionSelfServiceMode()
		{
		}

		public override bool Configure(IXMLAbleList source, IXMLAbleList target)
		{
			this.ActionOptions.SelfServiceCreateMode = true;
			return base.Configure(source, target);
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			SPServer item = (SPServer)targetSelections[0];
			return PasteSiteCollectionSelfServiceAction.SharePointURLIsRootSite(item.Adapter.Url);
		}
	}
}