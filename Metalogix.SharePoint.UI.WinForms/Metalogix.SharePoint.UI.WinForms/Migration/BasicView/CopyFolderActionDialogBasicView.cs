using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class CopyFolderActionDialogBasicView : ScopableLeftNavigableTabsFormBasicView
	{
		private TCMigrationModeOptionsBasicView _tcMigrationModeOptionsBasicView;

		private TCMigrationOptionsBasicView _tcMigrationOptionsBasicView;

		private TCGeneralOptionsBasicView _tcGeneralOptionsBasicView;

		private TCSummaryBasicView _tcSummaryBasicView;

		private IContainer components;

		public PasteFolderOptions Options
		{
			get
			{
				return Action.Options as PasteFolderOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyFolderActionDialogBasicView()
		{
			InitializeComponent();
			Text = "Configure Folder Copying Options";
			InitializeForm();
			ChangeFormSize();
		}

		private void ChangeFormSize()
		{
			base.Size = new Size(750, 805);
			MinimumSize = new Size(750, 805);
			MaximumSize = new Size(750, 805);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.CopyFolderActionDialogBasicView));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyFolderActionDialogBasicView.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "CopyFolderActionDialogBasicView";
			base.ShowIcon = true;
			this.Text = "Configure Folder Copying Options";
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		private void InitializeForm()
		{
			_tcMigrationModeOptionsBasicView = new TCMigrationModeOptionsBasicView();
			_tcMigrationOptionsBasicView = new TCMigrationOptionsBasicView(hasOnlyExternalLists: false);
			_tcGeneralOptionsBasicView = new TCGeneralOptionsBasicView();
			_tcSummaryBasicView = new TCSummaryBasicView();
			List<TabbableControl> list = new List<TabbableControl> { _tcMigrationModeOptionsBasicView, _tcMigrationOptionsBasicView, _tcGeneralOptionsBasicView, _tcSummaryBasicView };
			List<TabbableControl> list2 = list;
			foreach (TabbableControl item in list2)
			{
				((ScopableTabbableControl)item).Scope = SharePointObjectScope.Folder;
			}
			base.Tabs = list2;
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			_tcMigrationOptionsBasicView.IsSetExplicitOptions = SPUIUtils.ShouldSetExplicitOptions(action);
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			_tcGeneralOptionsBasicView.Options = sPGeneralOptions;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			_tcMigrationModeOptionsBasicView.Options = sPMigrationModeOptions;
			SPFolderContentOptions sPFolderContentOptions = new SPFolderContentOptions();
			sPFolderContentOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.FolderOptions = sPFolderContentOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MMDOptions = sPTaxonomyOptions;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.PermissionsOptions = sPPermissionsOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MappingOptions = sPMappingOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.FilterOptions = sPFilterOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.WebPartOptions = sPWebPartOptions;
		}

		protected override bool SaveUI(Action action)
		{
			_tcMigrationOptionsBasicView.IsSetExplicitOptions = SPUIUtils.ShouldSetExplicitOptions(action);
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(_tcGeneralOptionsBasicView.Options);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.FilterOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MappingOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.PermissionsOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WebPartOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MMDOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.FolderOptions);
			action.Options.SetFromOptions(_tcMigrationModeOptionsBasicView.Options);
			return true;
		}
	}
}
