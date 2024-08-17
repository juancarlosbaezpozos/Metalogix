using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ActionConfigRequired(true)]
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Properties16.png")]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Properties... {5-Properties}")]
	[Name("Properties")]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Properties)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetType(typeof(Node))]
	public class NodePropertiesAction : Metalogix.Actions.Action
	{
		public NodePropertiesAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			SPConnection item = targetSelections[0] as SPConnection;
			if (item != null && item.Status == ConnectionStatus.Invalid)
			{
				return false;
			}
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}