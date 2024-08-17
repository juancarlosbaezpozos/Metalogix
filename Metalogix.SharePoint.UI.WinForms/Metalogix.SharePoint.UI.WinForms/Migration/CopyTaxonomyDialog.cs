using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyTaxonomyDialog : ScopableLeftNavigableTabsForm
	{
		private TCTaxonomyOptions w_tcTaxonomyOptions;

		private TCGeneralOptions w_tcGeneralOptions;

		private IContainer components;

		public PasteTaxonomyOptions Options
		{
			get
			{
				return Action.Options as PasteTaxonomyOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyTaxonomyDialog()
		{
			InitializeComponent();
			w_tcTaxonomyOptions = new TCTaxonomyOptions();
			w_tcGeneralOptions = new TCGeneralOptions();
			Text = "Configure Managed Metadata Options";
			List<TabbableControl> list = new List<TabbableControl> { w_tcTaxonomyOptions, w_tcGeneralOptions };
			foreach (ScopableTabbableControl item in list)
			{
			}
			w_tcGeneralOptions.DisplayVerboseLogging = false;
			w_tcGeneralOptions.DisplayTextFieldLinkCorrection = false;
			w_tcGeneralOptions.DisplayLinkCorrectionOption = false;
			w_tcGeneralOptions.DisplayOverrideCheckouts = false;
			base.Tabs = list;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyTaxonomyDialog));
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "CopyTaxonomyDialog";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Copy Taxonomy";
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			w_tcTaxonomyOptions.Scope = base.Scope;
			w_tcTaxonomyOptions.Options = sPTaxonomyOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneralOptions.Options = sPGeneralOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			((PasteTaxonomyOptions)action.Options).TaxonomyConfigured = true;
			action.Options.SetFromOptions(w_tcGeneralOptions.Options);
			action.Options.SetFromOptions(w_tcTaxonomyOptions.Options);
			return true;
		}
	}
}
