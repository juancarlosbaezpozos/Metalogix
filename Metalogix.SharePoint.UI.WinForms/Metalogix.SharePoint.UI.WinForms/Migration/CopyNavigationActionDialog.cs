using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyNavigationActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCNavigationOptions w_tcNavigation = new TCNavigationOptions();

		private TCNavigationMappingOptions w_tcMapping = new TCNavigationMappingOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private IContainer components;

		public PasteNavigationOptions Options
		{
			get
			{
				return Action.Options as PasteNavigationOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyNavigationActionDialog()
		{
			InitializeComponent();
			Text = "Configure Site Copying Options";
			base.Size = new Size(550, 215);
			w_tcGeneral.DisplayLinkCorrectionOption = false;
			w_tcGeneral.DisplayVerboseLogging = false;
			w_tcGeneral.DisplayCheckResults = false;
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcNavigation, w_tcMapping, w_tcGeneral };
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyNavigationActionDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyNavigationActionDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "CopyNavigationActionDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPNavigationOptions sPNavigationOptions = new SPNavigationOptions();
			sPNavigationOptions.SetFromOptions(action.Options);
			w_tcNavigation.Options = sPNavigationOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcNavigation.Options);
			action.Options.SetFromOptions(w_tcMapping.Options);
			action.Options.SetFromOptions(w_tcGeneral.Options);
			return true;
		}
	}
}
