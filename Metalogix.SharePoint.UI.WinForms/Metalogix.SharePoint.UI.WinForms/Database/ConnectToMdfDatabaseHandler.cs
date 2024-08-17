using Metalogix.SharePoint.Database;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class ConnectToMdfDatabaseHandler : IShowDialogPromptHandler
	{
		public ConnectToMdfDatabaseHandler()
		{
		}

		public bool Handle(out object response, params object[] inputs)
		{
			response = null;
			if ((new MdfFileCreationDialog((string)inputs[0], (string)inputs[1], (string)inputs[2], (bool)inputs[3])).ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			return true;
		}
	}
}