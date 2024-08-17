using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(ViewAgentAction) })]
	public class ViewAgentConfig : IActionConfig
	{
		public ViewAgentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			IXMLAbleList targets = context.ActionContext.Targets;
			if (targets != null && targets.Count > 0 && targets[0] is Agent)
			{
				Agent item = (Agent)targets[0];
				try
				{
					List<KeyValuePair<DateTime, string>> agentLogDetails = item.Parent.GetAgentLogDetails(item.AgentID, false, false);
					if (agentLogDetails == null)
					{
						return ConfigurationResult.Cancel;
					}
					else
					{
						ViewAgent viewAgent = new ViewAgent();
						item.Details = agentLogDetails;
						viewAgent.LoadUI(item);
						viewAgent.ShowDialog();
						configurationResult = ConfigurationResult.Run;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string str = "An error occurred while retrieving log details.";
					GlobalServices.ErrorHandler.HandleException("View Agent", str, exception, ErrorIcon.Error);
					this.LogMessage(item, exception, str);
					return ConfigurationResult.Cancel;
				}
				return configurationResult;
			}
			return ConfigurationResult.Cancel;
		}

		private void LogMessage(Agent agent, string message)
		{
			agent.Parent.AddLog(agent.AgentID, message);
		}

		private void LogMessage(Agent agent, Exception ex, string message)
		{
			message = string.Format("{0}. Error: '{1}'", message, ex);
			this.LogMessage(agent, message);
		}
	}
}