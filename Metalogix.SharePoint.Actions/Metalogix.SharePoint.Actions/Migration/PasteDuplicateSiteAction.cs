using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(false)]
	[CmdletEnabled(true, "Copy-MLSharePointSite", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[MenuText("1:Paste Duplicate Site... {0-Paste}")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste Duplicate Site")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class PasteDuplicateSiteAction : PasteSiteAction
	{
		public PasteDuplicateSiteAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!Metalogix.Actions.Action.AppliesToBase(this, sourceSelections, targetSelections) || base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (sourceSelections.Count == 1 && targetSelections.Count == 1 && sourceSelections[0] is SPWeb && targetSelections[0] is SPWeb)
			{
				SPWeb item = sourceSelections[0] as SPWeb;
				SPWeb sPWeb = targetSelections[0] as SPWeb;
				if (item.Parent != null)
				{
					return item.Parent.DisplayUrl == sPWeb.DisplayUrl;
				}
			}
			return false;
		}
	}
}