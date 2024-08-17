using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.UI.WinForms;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(RemoveConnection) })]
	public class RemoveConnectionConfig : IActionConfig
	{
		public RemoveConnectionConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			string str;
			string str1;
			if (FlatXtraMessageBox.Show("Do you wish to disconnect?", "Confirm disconnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return ConfigurationResult.Cancel;
			}
			bool flag = false;
			foreach (SPConnection target in context.ActionContext.Targets)
			{
				if (!target.IsBackupConnection || target.BackupType != SPConnection.BackupConnectionType.Bak)
				{
					continue;
				}
				flag = true;
				break;
			}
			bool flag1 = false;
			if (flag)
			{
				if (context.ActionContext.Targets.Count <= 1)
				{
					str = "Would you like to delete the temporary SQL database that was created in order to connect to this backup?";
					str1 = "Delete Temporary Database?";
				}
				else
				{
					str = "One or more of these connections are to backup files. Would you like to delete the temporary SQL databases that were created in order to connect to these backups?";
					str1 = "Delete Temporary Databases?";
				}
				if (FlatXtraMessageBox.Show(str, str1, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					context.GetAction<RemoveConnection>().DeleteTempDBs = flag1;
				}
			}
			return ConfigurationResult.Run;
		}
	}
}