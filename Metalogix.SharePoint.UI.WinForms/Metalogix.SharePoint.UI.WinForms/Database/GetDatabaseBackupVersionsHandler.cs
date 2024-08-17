using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class GetDatabaseBackupVersionsHandler : IShowDialogPromptHandler
	{
		public GetDatabaseBackupVersionsHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			BackupVersionPickerDialog backupVersionPickerDialog = new BackupVersionPickerDialog((string)inputs[0], (Credentials)inputs[1])
			{
				Backups = (List<DatabaseBackup>)inputs[2]
			};
			if (backupVersionPickerDialog.ShowDialog() == DialogResult.Cancel)
			{
				return false;
			}
			response = backupVersionPickerDialog.SelectedBackup;
			return true;
		}
	}
}