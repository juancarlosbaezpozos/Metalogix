using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.Actions;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	[ActionConfig(new Type[] { typeof(LoadFromCSVAction) })]
	public class LoadFromCSVConfig : IActionConfig
	{
		public LoadFromCSVConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Node item = context.ActionContext.Targets[0] as Node;
			if (item == null)
			{
				throw new Exception("Node is invalid");
			}
			LoadFromCSVDialog loadFromCSVDialog = new LoadFromCSVDialog()
			{
				Node = item,
				Workspace = item.Record.ParentWorkspace
			};
			loadFromCSVDialog.ShowDialog();
			item.ReleaseChildren();
			return ConfigurationResult.Cancel;
		}
	}
}