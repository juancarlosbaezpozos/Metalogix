using Metalogix.Actions;
using Metalogix.Licensing;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.Actions
{
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.Zero)]
	public class AgentWizardAction : Metalogix.Actions.Action
	{
		public AgentWizardAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}