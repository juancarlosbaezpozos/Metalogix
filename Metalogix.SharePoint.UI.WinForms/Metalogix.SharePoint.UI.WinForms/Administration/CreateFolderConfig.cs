using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(CreateFolderAction) })]
	public class CreateFolderConfig : IActionConfig
	{
		public CreateFolderConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			CreateFolderDialog createFolderDialog = new CreateFolderDialog()
			{
				Context = context.ActionContext,
				Options = context.GetActionOptions<CreateFolderOptions>()
			};
			if (createFolderDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = createFolderDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}