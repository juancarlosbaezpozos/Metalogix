using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(NodePropertiesAction) })]
	public class NodePropertiesConfig : IActionConfig
	{
		public NodePropertiesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Node item = (Node)context.ActionContext.Targets[0];
			NodePropertiesDialog nodePropertiesDialog = new NodePropertiesDialog();
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				nodePropertiesDialog.Node = item;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
			nodePropertiesDialog.ShowDialog();
			if (item is IDisposable)
			{
				((IDisposable)item).Dispose();
			}
			return ConfigurationResult.Run;
		}
	}
}