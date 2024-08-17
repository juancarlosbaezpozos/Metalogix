using Metalogix.UI.WinForms.Properties;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class ShowAdvancedSetting : BooleanApplicationSetting
	{
		public override string DisplayText
		{
			get
			{
				return Resources.AppSetting_ShowAdvanced;
			}
		}

		public override string ImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EnableAdvancedMode16.png";
			}
		}

		public override string LargeImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EnableAdvancedMode32.png";
			}
		}

		public override bool Value
		{
			get
			{
				return UIConfigurationVariables.ShowAdvanced;
			}
			set
			{
				UIConfigurationVariables.ShowAdvanced = value;
			}
		}

		public ShowAdvancedSetting()
		{
		}

		public new void OnClick(object sender, EventArgs e)
		{
			(new ThreadSettingsEditorForm()).ShowDialog();
		}
	}
}