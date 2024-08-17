using Metalogix.Actions;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms.Interfaces;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class ThreadSettingsEditorSetting : IApplicationSetting
	{
		public string DisplayText
		{
			get
			{
				return Resources.AppSetting_EditResourceUtilization;
			}
		}

		public string ImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EditResourceUtilizationSettings16.png";
			}
		}

		public string LargeImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EditResourceUtilizationSettings32.png";
			}
		}

		public ThreadSettingsEditorSetting()
		{
		}

		public void OnClick(object sender, EventArgs e)
		{
			(new ThreadSettingsEditorForm()).ShowDialog();
			string displayText = this.DisplayText;
			int threadsPerActionLimit = ActionConfigurationVariables.ThreadsPerActionLimit;
			StringAccumulator.Message.Send("SettingRibbonClicked", displayText, threadsPerActionLimit.ToString(), false, null);
		}
	}
}