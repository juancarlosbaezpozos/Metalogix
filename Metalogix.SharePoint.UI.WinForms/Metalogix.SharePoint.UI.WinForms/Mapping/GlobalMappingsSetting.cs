using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Interfaces;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Mapping
{
	public class GlobalMappingsSetting : IApplicationSetting
	{
		public string DisplayText
		{
			get
			{
				return Resources.AppSetting_ConfigureGlobalMappings;
			}
		}

		public string ImageName
		{
			get
			{
				return "Metalogix.SharePoint.UI.WinForms.Icons.Settings.ConfigureGlobalMappings16.png";
			}
		}

		public string LargeImageName
		{
			get
			{
				return "Metalogix.SharePoint.UI.WinForms.Icons.Settings.ConfigureGlobalMappings32.png";
			}
		}

		public GlobalMappingsSetting()
		{
		}

		public void OnClick(object sender, EventArgs e)
		{
			(new SPGlobalMappingDialog(!UIConfigurationVariables.ShowAdvanced)).ShowDialog();
			StringAccumulator.Message.Send("SettingRibbonClicked", this.DisplayText, false, null);
		}
	}
}