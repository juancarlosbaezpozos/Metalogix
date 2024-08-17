using Metalogix.SharePoint.Database;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class InitializeFileLocationsHandler : IShowDialogPromptHandler
	{
		public InitializeFileLocationsHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			if (FlatXtraMessageBox.Show("Unable to determine the default location for SQL file creation.\n You will need to specify locations for the data file and log file to be created before continuing.", "Could not determine default location", MessageBoxButtons.OK, MessageBoxIcon.Asterisk) != DialogResult.OK)
			{
				throw new ArgumentException("Could not restore backup: Could not determine default locaiton for SQL file location. ");
			}
			return true;
		}
	}
}