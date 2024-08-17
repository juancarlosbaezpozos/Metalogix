using Metalogix.SharePoint.Database;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class RestoreDatabaseFromMdfHandler : IShowDialogPromptHandler
	{
		public RestoreDatabaseFromMdfHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			if ((int)inputs.Length == 0)
			{
				FlatXtraMessageBox.Show((string)inputs[0], "Log file missing", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return true;
			}
			if ((new MdfCreateNewLogFileDialog((string)inputs[0], (string)inputs[1])).ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			return true;
		}
	}
}