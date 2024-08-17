using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(false)]
	[CmdletEnabled(true, "Copy-MLSharePointList", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[MenuText("1:Paste Duplicate List... {0-Paste}")]
	[Name("Paste Duplicate List")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPList))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class PasteDuplicateListAction : PasteListAction
	{
		public PasteDuplicateListAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!Metalogix.Actions.Action.AppliesToBase(this, sourceSelections, targetSelections) || base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (sourceSelections.Count == 1 && targetSelections.Count == 1 && sourceSelections[0] is SPList && targetSelections[0] is SPWeb)
			{
				SPList item = sourceSelections[0] as SPList;
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