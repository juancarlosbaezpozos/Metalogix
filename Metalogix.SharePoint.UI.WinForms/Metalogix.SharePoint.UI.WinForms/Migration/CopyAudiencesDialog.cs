using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyAudiencesDialog : ScopableLeftNavigableTabsForm
	{
		private TCAudienceOptions w_tcAudience = new TCAudienceOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private IContainer components;

		public CopyAudiencesOptions Options
		{
			get
			{
				return Action.Options as CopyAudiencesOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyAudiencesDialog()
		{
			InitializeComponent();
			Text = "Configure Audience Copying Options";
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcAudience, w_tcGeneral };
			w_tcGeneral.DisplayLinkCorrectionOption = false;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyAudiencesDialog));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyAudiencesDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyAudiencesDialog";
			base.ShowIcon = true;
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPAudienceOptions sPAudienceOptions = new SPAudienceOptions();
			sPAudienceOptions.SetFromOptions(action.Options);
			w_tcAudience.Options = sPAudienceOptions;
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
			action.Options.SetFromOptions(w_tcGeneral.Options);
			action.Options.SetFromOptions(w_tcAudience.Options);
			return true;
		}
	}
}
