using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ActionConfigRequired(true)]
	[BasicModeViewAllowed(true)]
	[IsConnectivityAction(true)]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.Zero)]
	public abstract class ConnectAction : Metalogix.Actions.Action, IConnectAction
	{
		protected ConnectAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (targetSelections.Count == 0)
			{
				return true;
			}
			if (targetSelections.Count == 1 && ((SPNode)targetSelections[0]).Parent == null)
			{
				return true;
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}