using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Licensing;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.One)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Agent))]
	public class AgentAction : Metalogix.Actions.Action<Agent>
	{
		public AgentAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}