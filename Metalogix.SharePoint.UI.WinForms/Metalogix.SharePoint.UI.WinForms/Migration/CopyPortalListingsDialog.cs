using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyPortalListingsDialog : ScopableLeftNavigableTabsForm
	{
		private TCMigrationModeOptions w_tcMigration = new TCMigrationModeOptions();

		private TCNavigationMappingOptions w_tcMapping = new TCNavigationMappingOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private PastePortalListingsOptions m_options;

		private IContainer components;

		public PastePortalListingsOptions Options
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

		public CopyPortalListingsDialog()
		{
			InitializeComponent();
			Text = "Configure Portal Listings Copying Options";
			base.Size = new Size(550, 400);
			w_tcMigration.ShowUpdateLists = false;
			w_tcMigration.ShowUpdateListsElipseButton = false;
			w_tcMigration.ShowUpdateItemsElipseButton = false;
			w_tcGeneral.DisplayLinkCorrectionOption = false;
			w_tcGeneral.DisplayVerboseLogging = false;
			w_tcGeneral.DisplayCheckResults = false;
			w_tcGeneral.DisplayOverrideCheckouts = false;
			w_tcGeneral.DisplayLogSkippedItems = false;
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigration, w_tcMapping, w_tcGeneral };
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.List;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyPortalListingsDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyPortalListingsDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyPortalListingsDialog";
			base.ShowIcon = true;
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(Options);
			w_tcMigration.Options = sPMigrationModeOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(Options);
			w_tcMapping.Options = sPMappingOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(Options);
			w_tcGeneral.Options = sPGeneralOptions;
		}

		protected override bool SaveUI()
		{
			if (!base.SaveUI())
			{
				return false;
			}
			Options.SetFromOptions(w_tcMigration.Options);
			Options.SetFromOptions(w_tcMapping.Options);
			Options.SetFromOptions(w_tcGeneral.Options);
			return true;
		}
	}
}
