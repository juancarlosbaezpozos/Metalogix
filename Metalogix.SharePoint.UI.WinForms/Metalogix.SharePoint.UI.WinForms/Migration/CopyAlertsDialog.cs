using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyAlertsDialog : ScopableLeftNavigableTabsForm
	{
		private TCAlertOptions w_tcAlertOptions = new TCAlertOptions();

		private AlertOptions m_options;

		private IContainer components;

		public AlertOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public CopyAlertsDialog(bool bIndividuallyScoped, bool isTargetSPO)
		{
			InitializeComponent();
			Text = "Configure Alert Copying Options";
			base.Size = new Size(500, 200);
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcAlertOptions };
			w_tcAlertOptions.IndividuallyScoped = bIndividuallyScoped;
			w_tcAlertOptions.IsTargetSPO = isTargetSPO;
			base.Tabs = tabs;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyAlertsDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyAlertsDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyAlertsDialog";
			base.ShowIcon = true;
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			SPAlertOptions sPAlertOptions = new SPAlertOptions();
			sPAlertOptions.SetFromOptions(Options);
			w_tcAlertOptions.Options = sPAlertOptions;
		}

		protected override bool SaveUI()
		{
			if (!base.SaveUI())
			{
				return false;
			}
			Options.SetFromOptions(w_tcAlertOptions.Options);
			return true;
		}
	}
}
