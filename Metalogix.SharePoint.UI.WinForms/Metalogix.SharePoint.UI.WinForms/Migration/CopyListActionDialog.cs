using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyListActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCListContentOptions w_tcListContent = new TCListContentOptions();

		private TCPermissionsOptions w_tcPermissions = new TCPermissionsOptions();

		private TCMappingOptions w_tcMapping = new TCMappingOptions();

		private TCFilterOptions w_tcFilters = new TCFilterOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCWebPartsOptions w_tcWebParts = new TCWebPartsOptions();

		private TCWorkflowOptions w_tcWorkflows = new TCWorkflowOptions();

		private TCTaxonomyOptions w_tcTaxonomy = new TCTaxonomyOptions();

		private TCMigrationModeOptions w_tcMigrationMode = new TCMigrationModeOptions();

		private TCStoragePointOptionsMMS w_tcStoragePoint = new TCStoragePointOptionsMMS();

		private bool _targetIsCSOM;

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

		public CopyListActionDialog(bool onlyExternal, bool targetCSOM = false)
		{
			InitializeComponent();
			Text = "Configure List Copying Options";
			_targetIsCSOM = targetCSOM;
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode };
			if (!onlyExternal)
			{
				list.Add(w_tcListContent);
				list.Add(w_tcTaxonomy);
			}
			list.Add(w_tcWebParts);
			list.Add(w_tcPermissions);
			list.Add(w_tcMapping);
			list.Add(w_tcFilters);
			if (!onlyExternal)
			{
				list.Add(w_tcWorkflows);
			}
			if (!_targetIsCSOM)
			{
				list.Add(w_tcStoragePoint);
			}
			list.Add(w_tcGeneral);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyListActionDialog));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyListActionDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListActionDialog";
			base.ShowIcon = true;
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			w_tcPermissions.Options = sPPermissionsOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			w_tcFilters.Options = sPFilterOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			w_tcWebParts.Options = sPWebPartOptions;
			if (!BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				SPListContentOptions sPListContentOptions = new SPListContentOptions();
				sPListContentOptions.SetFromOptions(action.Options);
				w_tcListContent.Options = sPListContentOptions;
				SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
				sPTaxonomyOptions.SetFromOptions(action.Options);
				w_tcTaxonomy.Options = sPTaxonomyOptions;
				SPWorkflowOptions sPWorkflowOptions = new SPWorkflowOptions();
				sPWorkflowOptions.SetFromOptions(action.Options);
				w_tcWorkflows.Options = sPWorkflowOptions;
			}
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			if (!_targetIsCSOM)
			{
				ExternalizationOptions externalizationOptions = new ExternalizationOptions();
				externalizationOptions.SetFromOptions(action.Options);
				w_tcStoragePoint.Options = externalizationOptions;
			}
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcGeneral.Options);
			action.Options.SetFromOptions(w_tcFilters.Options);
			action.Options.SetFromOptions(w_tcMapping.Options);
			action.Options.SetFromOptions(w_tcPermissions.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			action.Options.SetFromOptions(w_tcWebParts.Options);
			if (!_targetIsCSOM)
			{
				action.Options.SetFromOptions(w_tcStoragePoint.Options);
			}
			if (BCSHelper.HasExternalListsOnly(SourceNodes))
			{
				action.Options.SetFromOptions(new SPListContentOptions());
				action.Options.SetFromOptions(new SPTaxonomyOptions());
				action.Options.SetFromOptions(new SPWorkflowOptions());
			}
			else
			{
				action.Options.SetFromOptions(w_tcListContent.Options);
				action.Options.SetFromOptions(w_tcTaxonomy.Options);
				action.Options.SetFromOptions(w_tcWorkflows.Options);
			}
			return true;
		}
	}
}
