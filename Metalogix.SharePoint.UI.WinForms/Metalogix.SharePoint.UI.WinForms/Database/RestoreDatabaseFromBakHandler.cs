using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class RestoreDatabaseFromBakHandler : IShowDialogPromptHandler
	{
		public RestoreDatabaseFromBakHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			BackupFileCreationDialog backupFileCreationDialog = new BackupFileCreationDialog((string)inputs[0], (string)inputs[1], (List<DatabaseBackup>)inputs[2], (Credentials)inputs[3]);
			backupFileCreationDialog.ShowDialog();
			if (backupFileCreationDialog.DialogResult == DialogResult.Cancel)
			{
				return false;
			}
			Dictionary<string, object> strs = new Dictionary<string, object>()
			{
				{ "DataFileLocation", backupFileCreationDialog.DataFileLocation },
				{ "LogFileLocation", backupFileCreationDialog.LogFileLocation },
				{ "UseRedGate", backupFileCreationDialog.UseRedGate }
			};
			response = strs;
			return true;
		}
	}
}