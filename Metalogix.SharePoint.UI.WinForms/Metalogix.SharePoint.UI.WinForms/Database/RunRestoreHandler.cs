using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class RunRestoreHandler : IShowDialogPromptHandler
	{
		public RunRestoreHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			DatabaseRestoreDialog databaseRestoreDialog = new DatabaseRestoreDialog((string)inputs[0], (string)inputs[1], (string)inputs[2], (string)inputs[3], (List<DatabaseBackup>)inputs[4], (Credentials)inputs[5], (bool)inputs[6])
			{
				Text = "Creating Temporary Database"
			};
			DialogResult dialogResult = databaseRestoreDialog.ShowDialog();
			if (databaseRestoreDialog.Exception != null)
			{
				throw databaseRestoreDialog.Exception;
			}
			if (dialogResult != DialogResult.OK)
			{
				return false;
			}
			return true;
		}
	}
}