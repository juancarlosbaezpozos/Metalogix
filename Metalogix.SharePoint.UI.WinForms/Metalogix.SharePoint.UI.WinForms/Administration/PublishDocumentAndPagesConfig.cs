using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Collections;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(PublishDocumentsAndPagesAction) })]
	public class PublishDocumentAndPagesConfig : IActionConfig
	{
		public PublishDocumentAndPagesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			bool flag = false;
			foreach (SPList target in context.ActionContext.Targets)
			{
				if (!target.EnableModeration)
				{
					continue;
				}
				flag = true;
				break;
			}
			PublishDocumentsandPagesDialog publishDocumentsandPagesDialog = new PublishDocumentsandPagesDialog()
			{
				ApprovalAvailable = flag,
				Options = context.GetActionOptions<PublishDocumentsAndPagesOptions>()
			};
			publishDocumentsandPagesDialog.ShowDialog();
			if (publishDocumentsandPagesDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = publishDocumentsandPagesDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}