using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointSiteCollection", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Incrementable(true, "Paste Site Collection Incrementally")]
	[MenuText("1:Paste Site Collection...{0-Paste} > 2:Self Service Mode")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste Site Collection")]
	[SourceType(typeof(SPWeb), true)]
	[SupportsThreeStateConfiguration(true)]
	[TargetType(typeof(SPServer))]
	public class PasteSiteCollectionSelfServiceAction : PasteSiteCollectionAction
	{
		public PasteSiteCollectionSelfServiceAction()
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

		public static bool SharePointURLIsRootSite(string sURL)
		{
			string str = sURL.TrimEnd(new char[] { '/' });
			int num = str.IndexOf("://");
			if (num < 0)
			{
				return true;
			}
			return str.IndexOf("/", num + 3) < 0;
		}
	}
}