using Metalogix.Metabase;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms.Interfaces;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	public class MetabaseSettingsEditorSetting : IApplicationSetting
	{
		public string DisplayText
		{
			get
			{
				return Resources.AppSetting_ConfigureDefaultMetabaseSettings;
			}
		}

		public string ImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.ConfigureDefaultMetabaseSettings16.png";
			}
		}

		public string LargeImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.ConfigureDefaultMetabaseSettings32.png";
			}
		}

		public MetabaseSettingsEditorSetting()
		{
		}

		public void OnClick(object sender, EventArgs e)
		{
			(new MetabaseDefaultConfiguration()).ShowDialog();
			StringAccumulator.Message.Send("SettingRibbonClicked", this.DisplayText, ConfigurationVariables.DefaultMetabaseAdapter, false, null);
		}
	}
}