using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyListItemActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCListContentOptions w_tcListContent = new TCListContentOptions();

		private TCPermissionsOptions w_tcPermissions = new TCPermissionsOptions();

		private TCMappingOptions w_tcMapping = new TCMappingOptions();

		private TCFilterOptions w_tcFilters = new TCFilterOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCWebPartsOptions w_tcWebParts = new TCWebPartsOptions();

		private TCTaxonomyOptions w_tcTaxonomy = new TCTaxonomyOptions();

		private TCMigrationModeOptions w_tcMigrationMode = new TCMigrationModeOptions();

		private TCStoragePointOptionsMMS w_tcStoragePoint = new TCStoragePointOptionsMMS();

		private bool _targetIsCSOM;

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

		public CopyListItemActionDialog(bool targetCSOM = false)
		{
			InitializeComponent();
			Text = "Configure Item Copying Options";
			_targetIsCSOM = targetCSOM;
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode, w_tcListContent, w_tcTaxonomy, w_tcWebParts, w_tcPermissions, w_tcMapping, w_tcFilters };
			if (!_targetIsCSOM)
			{
				list.Add(w_tcStoragePoint);
			}
			list.Add(w_tcGeneral);
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.Item;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyListItemActionDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyListItemActionDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListItemActionDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			w_tcFilters.Options = sPFilterOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			w_tcPermissions.Options = sPPermissionsOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			w_tcWebParts.Options = sPWebPartOptions;
			SPListContentOptions sPListContentOptions = new SPListContentOptions();
			sPListContentOptions.SetFromOptions(action.Options);
			w_tcListContent.Options = sPListContentOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			w_tcTaxonomy.Options = sPTaxonomyOptions;
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
			action.Options.SetFromOptions(w_tcWebParts.Options);
			action.Options.SetFromOptions(w_tcTaxonomy.Options);
			action.Options.SetFromOptions(w_tcListContent.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			if (!_targetIsCSOM)
			{
				action.Options.SetFromOptions(w_tcStoragePoint.Options);
			}
			return true;
		}
	}
}
