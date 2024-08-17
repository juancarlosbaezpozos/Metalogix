using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class CopyListActionDialogBasicView : ScopableLeftNavigableTabsFormBasicView
	{
		private TCMigrationModeOptionsBasicView _tcMigrationModeOptionsBasicView;

		private TCMigrationOptionsBasicView _tcMigrationOptionsBasicView;

		private TCGeneralOptionsBasicView _tcGeneralOptionsBasicView;

		private TCSummaryBasicView _tcSummaryBasicView;

		private bool _isTargetCSOM;

		private IContainer components;

		public PasteListOptions Options
		{
			get
			{
				return Action.Options as PasteListOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyListActionDialogBasicView(bool hasOnlyExternal, bool isTargetCSOM = false)
		{
			InitializeComponent();
			InitializeForm(hasOnlyExternal, isTargetCSOM);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.CopyListActionDialogBasicView));
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
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyListActionDialogBasicView.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListActionDialogBasicView";
			base.ShowIcon = true;
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		private void InitializeForm(bool hasOnlyExternal, bool isTargetCSOM)
		{
			_tcMigrationModeOptionsBasicView = new TCMigrationModeOptionsBasicView();
			_tcMigrationOptionsBasicView = new TCMigrationOptionsBasicView(hasOnlyExternal);
			_tcGeneralOptionsBasicView = new TCGeneralOptionsBasicView();
			_tcSummaryBasicView = new TCSummaryBasicView();
			_isTargetCSOM = isTargetCSOM;
			List<TabbableControl> list = new List<TabbableControl> { _tcMigrationModeOptionsBasicView, _tcMigrationOptionsBasicView, _tcGeneralOptionsBasicView, _tcSummaryBasicView };
			List<TabbableControl> list2 = list;
			foreach (TabbableControl item in list2)
			{
				((ScopableTabbableControl)item).Scope = SharePointObjectScope.List;
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
			if (!BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				SPListContentOptions sPListContentOptions = new SPListContentOptions();
				sPListContentOptions.SetFromOptions(action.Options);
				_tcMigrationOptionsBasicView.ListContentOptions = sPListContentOptions;
				SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
				sPTaxonomyOptions.SetFromOptions(action.Options);
				_tcMigrationOptionsBasicView.MMDOptions = sPTaxonomyOptions;
				SPWorkflowOptions sPWorkflowOptions = new SPWorkflowOptions();
				sPWorkflowOptions.SetFromOptions(action.Options);
				_tcMigrationOptionsBasicView.WorkflowOptions = sPWorkflowOptions;
			}
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
			action.Options.SetFromOptions(_tcMigrationModeOptionsBasicView.Options);
			if (BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				action.Options.SetFromOptions(new SPListContentOptions());
				action.Options.SetFromOptions(new SPTaxonomyOptions());
				action.Options.SetFromOptions(new SPWorkflowOptions());
			}
			else
			{
				action.Options.SetFromOptions(_tcMigrationOptionsBasicView.ListContentOptions);
				action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MMDOptions);
				action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WorkflowOptions);
			}
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.FilterOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MappingOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WebPartOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.PermissionsOptions);
			return true;
		}
	}
}
