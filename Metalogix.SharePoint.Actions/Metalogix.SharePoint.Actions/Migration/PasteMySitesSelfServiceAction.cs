using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointMySites", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Incrementable(true, "Paste MySites Incrementally")]
	[MenuText("1:Paste MySites...{0-Paste} > Self Service Mode")]
	[Name("Paste MySites")]
	[SourceType(typeof(SPServer), true)]
	[SubActionTypes(typeof(PasteNavigationAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetType(typeof(SPServer))]
	public class PasteMySitesSelfServiceAction : PasteMySitesAction
	{
		public PasteMySitesSelfServiceAction()
		{
			base.SharePointOptions.SelfServiceCreateMode = true;
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!this.AppliesTo(sourceSelections, targetSelections) || !base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			return PasteSiteCollectionSelfServiceAction.SharePointURLIsRootSite((targetSelections[0] as SPServer).Adapter.Url);
		}
	}
}