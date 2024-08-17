using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteAudiencesAction) })]
	public class PasteAudiencesConfig : IActionConfig
	{
		public PasteAudiencesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			SPServer item = context.ActionContext.Sources[0] as SPServer;
			SPServer sPServer = context.ActionContext.Targets[0] as SPServer;
			if (item.DisplayUrl == sPServer.DisplayUrl)
			{
				FlatXtraMessageBox.Show("Cannot Copy: Target is the same as the Source", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return ConfigurationResult.Cancel;
			}
			try
			{
				if (item.Audiences == null)
				{
					FlatXtraMessageBox.Show("Cannot Copy: Source server does not support audiences.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					configurationResult = ConfigurationResult.Cancel;
				}
				else if (sPServer.Audiences != null)
				{
					CopyAudiencesDialog copyAudiencesDialog = new CopyAudiencesDialog()
					{
						Options = context.GetActionOptions<CopyAudiencesOptions>()
					};
					copyAudiencesDialog.ShowDialog();
					return copyAudiencesDialog.ConfigurationResult;
				}
				else
				{
					FlatXtraMessageBox.Show("Cannot Copy: Target server does not support audiences.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					configurationResult = ConfigurationResult.Cancel;
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
				configurationResult = ConfigurationResult.Cancel;
			}
			return configurationResult;
		}
	}
}