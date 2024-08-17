using Metalogix.Actions;
using Metalogix.Explorer;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[MenuText("Refresh All Connections {4-Refresh}")]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.Zero)]
	public class RefreshAllAction : RefreshAction
	{
		public RefreshAllAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return base.AppliesTo(sourceSelections, targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Metalogix.Explorer.Settings.RefreshActiveConnections();
		}
	}
}