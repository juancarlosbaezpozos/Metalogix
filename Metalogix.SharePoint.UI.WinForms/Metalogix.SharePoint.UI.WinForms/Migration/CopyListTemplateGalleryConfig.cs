using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyListTemplateGalleryAction) })]
	public class CopyListTemplateGalleryConfig : IActionConfig
	{
		public CopyListTemplateGalleryConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (FlatXtraMessageBox.Show(string.Format("Are you sure you wish to copy the contents of the source list template gallery to the target?{0}{0}If any list template file name is identical on the source and target, then the target list template will be overwritten.", Environment.NewLine), "Copy List Template Gallery", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
			{
				return ConfigurationResult.Cancel;
			}
			return ConfigurationResult.Run;
		}
	}
}