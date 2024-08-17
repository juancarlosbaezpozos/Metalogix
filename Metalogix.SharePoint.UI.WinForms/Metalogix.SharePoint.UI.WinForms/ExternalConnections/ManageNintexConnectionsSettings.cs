using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms.Interfaces;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	public class ManageNintexConnectionsSettings : IApplicationSetting
	{
		public string DisplayText
		{
			get
			{
				return Resources.AppSetting_ManageNintexWorkflowDatabases;
			}
		}

		public string ImageName
		{
			get
			{
				return "Metalogix.SharePoint.UI.WinForms.Icons.Settings.ManageNintexWorkflowDatabases16x16.png";
			}
		}

		public string LargeImageName
		{
			get
			{
				return "Metalogix.SharePoint.UI.WinForms.Icons.Settings.ManageNintexWorkflowDatabases32x32.png";
			}
		}

		public ManageNintexConnectionsSettings()
		{
		}

		public void OnClick(object sender, EventArgs e)
		{
			(new ManageNintexConnectionsDialog()).ShowDialog();
			StringAccumulator.Message.Send("SettingRibbonClicked", this.DisplayText, false, null);
		}
	}
}