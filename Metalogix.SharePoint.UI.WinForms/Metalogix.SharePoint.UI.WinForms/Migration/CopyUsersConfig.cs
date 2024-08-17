using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyUsersAction) })]
	public class CopyUsersConfig : IActionConfig
	{
		public CopyUsersConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			CopyUsersActionDialog copyUsersActionDialog = new CopyUsersActionDialog();
			copyUsersActionDialog.Initialize(context, context.GetActionOptions<CopyUserOptions>());
			copyUsersActionDialog.ShowDialog();
			if (copyUsersActionDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.GetActionOptions<CopyUserOptions>().AllowDBUserWriting = copyUsersActionDialog.AllowSql;
			List<SPUser> selectedLoginNames = copyUsersActionDialog.getSelectedLoginNames();
			context.GetActionOptions<CopyUserOptions>().SourceUsers = new SPUserCollection(selectedLoginNames.ToArray());
			return copyUsersActionDialog.ConfigurationResult;
		}
	}
}