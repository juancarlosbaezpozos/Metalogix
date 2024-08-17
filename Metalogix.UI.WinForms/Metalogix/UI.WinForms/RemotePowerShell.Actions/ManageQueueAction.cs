using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.Licensing;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.Zero)]
	[TargetType(typeof(Job))]
	public class ManageQueueAction : Metalogix.Actions.Action
	{
		public ManageQueueAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}