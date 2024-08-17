using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.DeleteAgent16.png")]
	[MenuText("Remove Agent {4-Remove Agent}")]
	[Name("Remove Agent")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	public class RemoveAgentAction : AgentAction
	{
		public RemoveAgentAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target.Count > 0 && target[0] is Agent)
			{
				Agent item = (Agent)target[0];
				try
				{
					string deleteAgentMessage = Resources.Delete_Agent_Message;
					MessageBoxIcon messageBoxIcon = MessageBoxIcon.Question;
					if (item.Status == AgentStatus.Configuring)
					{
						deleteAgentMessage = Resources.Delete_Configuring_Agent_Message;
						messageBoxIcon = MessageBoxIcon.Exclamation;
					}
					if (FlatXtraMessageBox.Show(deleteAgentMessage, Resources.Delete_Agent_Confirm, MessageBoxButtons.YesNo, messageBoxIcon) == DialogResult.Yes && item != null)
					{
						item.Parent.Delete(item);
					}
					IRemoteWorker remoteWorker = new RemoteWorker(item);
					string str = string.Format("/delete:{0}", item.MachineName);
					remoteWorker.AddRemoveCredentials(str);
				}
				catch (Exception exception)
				{
					LogHelper.LogMessage(item, exception, "An error occurred while deleting agent.");
				}
			}
		}
	}
}