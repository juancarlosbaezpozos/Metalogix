using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.Winforms.Resources.ManageAgents16.png")]
	[LargeImage("Metalogix.UI.Winforms.Resources.ManageAgents32.png")]
	[MenuText("Manage Agents {5-Manage Agents}")]
	[Name("Manage Agents")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.Zero)]
	public class ManageAgentsAction : AgentAction
	{
		public ManageAgentsAction()
		{
		}
	}
}