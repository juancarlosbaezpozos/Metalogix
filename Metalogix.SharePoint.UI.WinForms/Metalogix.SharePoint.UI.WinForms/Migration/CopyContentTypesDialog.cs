using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyContentTypesDialog : ScopableLeftNavigableTabsForm
	{
		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCTaxonomyOptions w_tcTaxonomy = new TCTaxonomyOptions();

		private TCContentTypeOptions w_tcContentTypes = new TCContentTypeOptions();

		private IContainer components;

		public PasteContentTypesOptions Options
		{
			get
			{
				return Action.Options as PasteContentTypesOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyContentTypesDialog()
		{
			InitializeComponent();
			Text = "Configure Content Types Copying Options";
			List<TabbableControl> list = new List<TabbableControl> { w_tcContentTypes, w_tcTaxonomy, w_tcGeneral };
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.Site;
			}
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
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyContentTypesDialog";
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			w_tcTaxonomy.VisOptions = TaxonomySetupOptions.MapTermstores | TaxonomySetupOptions.ReferencedMMD | TaxonomySetupOptions.ResolveByName;
			w_tcTaxonomy.Options = sPTaxonomyOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
			PasteContentTypesOptions pasteContentTypesOptions = new PasteContentTypesOptions();
			pasteContentTypesOptions.SetFromOptions(action.Options);
			w_tcContentTypes.Options = pasteContentTypesOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcContentTypes.Options);
			action.Options.SetFromOptions(w_tcTaxonomy.Options);
			action.Options.SetFromOptions(w_tcGeneral.Options);
			return true;
		}
	}
}
