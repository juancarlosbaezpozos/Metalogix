using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(CreateListAction) })]
	public class CreateListConfig : IActionConfig
	{
		public CreateListConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			using (SPWeb item = (SPWeb)context.ActionContext.Targets[0])
			{
				CreateListDialog createListDialog = new CreateListDialog(item)
				{
					Options = context.GetActionOptions<CreateListOptions>()
				};
				createListDialog.ShowDialog();
				if (createListDialog.DialogResult != DialogResult.OK)
				{
					return ConfigurationResult.Cancel;
				}
				else
				{
					context.ActionOptions = createListDialog.Options;
					configurationResult = ConfigurationResult.Run;
				}
			}
			return configurationResult;
		}
	}
}