using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteRolesAction) })]
	public class CopyRolesConfig : IActionConfig
	{
		public CopyRolesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			CopyRolesDialog copyRolesDialog = new CopyRolesDialog()
			{
				Options = context.GetActionOptions<PasteRolesOptions>()
			};
			copyRolesDialog.ShowDialog();
			return copyRolesDialog.ConfigurationResult;
		}
	}
}