using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class CopySiteActionDialogBasicView : ScopableLeftNavigableTabsFormBasicView
	{
		private TCMigrationModeOptionsBasicView _tcMigrationModeOptionsBasicView;

		private TCMigrationOptionsBasicView _tcMigrationOptionsBasicView;

		private TCGeneralOptionsBasicView _tcGeneralOptionsBasicView;

		private TCSummaryBasicView _tcSummaryBasicView;

		private IContainer components;

		public PasteSiteOptions Options
		{
			get
			{
				return Action.Options as PasteSiteOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopySiteActionDialogBasicView()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.CopySiteActionDialogBasicView));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(base._loadActionTemplateButton, "_loadActionTemplateButton");
			base._loadActionTemplateButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base._loadActionTemplateButton.LookAndFeel.UseDefaultLookAndFeel = false;
			resources.ApplyResources(base._saveActionTemplateButton, "_saveActionTemplateButton");
			base._saveActionTemplateButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base._saveActionTemplateButton.LookAndFeel.UseDefaultLookAndFeel = false;
			resources.ApplyResources(base.w_btnCancel, "w_btnCancel");
			resources.ApplyResources(base.w_btnOK, "w_btnOK");
			base.w_btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base.w_btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
			resources.ApplyResources(base.w_btnSave, "w_btnSave");
			base.w_btnSave.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			base.w_btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
			base.tabControl.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("tabControl.Appearance.BackColor");
			base.tabControl.Appearance.Options.UseBackColor = true;
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			resources.ApplyResources(base.tabControl, "tabControl");
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopySiteActionDialogBasicView.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopySiteActionDialogBasicView";
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
			_tcMigrationOptionsBasicView.ActionType = Action.GetType();
			foreach (TabbableControl item in list2)
			{
				((ScopableTabbableControl)item).Scope = SharePointObjectScope.Site;
			}
			base.Tabs = list2;
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			_tcMigrationOptionsBasicView.IsSetExplicitOptions = SPUIUtils.ShouldSetExplicitOptions(action);
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.FilterOptions = sPFilterOptions;
			SPSiteContentOptions sPSiteContentOptions = new SPSiteContentOptions();
			sPSiteContentOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.SiteContentOptions = sPSiteContentOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			_tcGeneralOptionsBasicView.Options = sPGeneralOptions;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			_tcMigrationModeOptionsBasicView.Options = sPMigrationModeOptions;
			SPListContentOptions sPListContentOptions = new SPListContentOptions();
			sPListContentOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.ListContentOptions = sPListContentOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MappingOptions = sPMappingOptions;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.PermissionsOptions = sPPermissionsOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.WebPartOptions = sPWebPartOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.MMDOptions = sPTaxonomyOptions;
			SPWorkflowOptions sPWorkflowOptions = new SPWorkflowOptions();
			sPWorkflowOptions.SetFromOptions(action.Options);
			_tcMigrationOptionsBasicView.WorkflowOptions = sPWorkflowOptions;
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
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.SiteContentOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.FilterOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MappingOptions);
			action.Options.SetFromOptions(_tcGeneralOptionsBasicView.Options);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WebPartOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.PermissionsOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.MMDOptions);
			action.Options.SetFromOptions(_tcMigrationOptionsBasicView.WorkflowOptions);
			return true;
		}
	}
}
