using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Connect... {3-Connect}")]
	[Name("Connect")]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.ZeroOrOne)]
	public class ReConnect : Metalogix.SharePoint.Actions.Administration.ConnectAction
	{
		public ReConnect()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (targetSelections.Count != 1)
			{
				return false;
			}
			SPConnection item = targetSelections[0] as SPConnection;
			if (item == null)
			{
				return false;
			}
			if (item.Parent != null)
			{
				return false;
			}
			if (item.Status != ConnectionStatus.Invalid)
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