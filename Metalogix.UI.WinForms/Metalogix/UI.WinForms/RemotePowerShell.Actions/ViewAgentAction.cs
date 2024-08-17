using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.ViewAgent16.png")]
	[MenuText("View Agent {0-View Agent}")]
	[Name("View Agent")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	public class ViewAgentAction : AgentAction
	{
		public ViewAgentAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target != null && target.Count > 0 && target[0] is Agent)
			{
				Agent item = (Agent)target[0];
				item.Parent.UpdateLatestLogOnUI(item);
			}
		}
	}
}