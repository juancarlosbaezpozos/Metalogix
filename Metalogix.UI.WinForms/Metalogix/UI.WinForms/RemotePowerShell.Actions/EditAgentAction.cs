using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.EditAgent16.png")]
	[MenuText("Edit Agent {1-Edit Agent}")]
	[Name("Edit Agent")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	public class EditAgentAction : AgentAction
	{
		public EditAgentAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList source, IXMLAbleList target)
		{
			if (target == null)
			{
				return false;
			}
			return target.Cast<Agent>().Any<Agent>((Agent agent) => {
				if (agent.Status == AgentStatus.Available)
				{
					return true;
				}
				return agent.Status == AgentStatus.Error;
			});
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target != null && target.Count > 0 && target[0] is Agent)
			{
				Agent item = (Agent)target[0];
				try
				{
					try
					{
						item.Parent.UpdateCredentials(this.ActionOptions, this.ActionOptions.UserName, this.ActionOptions.Password);
						LogHelper.LogMessage(this.ActionOptions, "Updated Successfully.");
					}
					catch (Exception exception)
					{
						LogHelper.LogMessage(this.ActionOptions, exception, "An error occurred while updating details.");
					}
				}
				finally
				{
					item.Parent.UpdateLatestLogOnUI(item);
				}
			}
		}
	}
}