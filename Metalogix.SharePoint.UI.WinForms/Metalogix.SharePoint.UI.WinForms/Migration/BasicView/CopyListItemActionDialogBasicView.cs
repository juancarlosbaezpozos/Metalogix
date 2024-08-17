using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class CopyListItemActionDialogBasicView : ScopableLeftNavigableTabsFormBasicView
	{
		private TCMigrationModeOptionsBasicView _tcMigrationModeOptionsBasicView;

		private TCMigrationOptionsBasicView _tcMigrationOptionsBasicView;

		private TCGeneralOptionsBasicView _tcGeneralOptionsBasicView;

		private TCSummaryBasicView _tcSummaryBasicView;

		private IContainer components;

		public PasteListItemOptions Options
		{
			get
			{
				return Action.Options as PasteListItemOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyListItemActionDialogBasicView()
		{
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.CopyListItemActionDialogBasicView));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(base._loadActionTemplateButton, "_loadActionTemplateButton");
			resources.ApplyResources(base._saveActionTemplateButton, "_saveActionTemplateButton");
			resources.ApplyResources(base.w_btnCancel, "w_btnCancel");
			resources.ApplyResources(base.w_btnOK, "w_btnOK");
			resources.ApplyResources(base.w_btnSave, "w_btnSave");
			base.tabControl.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("tabControl.Appearance.BackColor");
			base.tabControl.Appearance.Options.UseBackColor = true;
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			resources.ApplyResources(base.tabControl, "tabControl");
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyListItemActionDialogBasicView.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListItemActionDialogBasicView";
			base.ShowIcon = true;
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
				((ScopableTabbableControl)item).Scope = SharePointObjectScope.Item;
			}
			base.Tabs = list2;
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			_tcMigrationOptionsBasicView.IsSetExplicitOptions = SPUIUtils.ShouldSetExplicitOptions(action);
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			_tcMigrationModeOptionsBasicView.Options = sPMigrationModeOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			_tcGeneralOptionsBasicView.Options = sPGeneralOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MappingOptions = sPMappingOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.FilterOptions = sPFilterOptions;
			SPListContentOptions sPListContentOptions = new SPListContentOptions();
			sPListContentOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.ListContentOptions = sPListContentOptions;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.PermissionsOptions = sPPermissionsOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MMDOptions = sPTaxonomyOptions;
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
			action.Options.SetFromOptions(_tcMigrationModeOptionsBasicView.Options);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.ListContentOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.FilterOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MappingOptions);
			action.Options.SetFromOptions(_tcGeneralOptionsBasicView.Options);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.PermissionsOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MMDOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WebPartOptions);
			return true;
		}
	}
}
