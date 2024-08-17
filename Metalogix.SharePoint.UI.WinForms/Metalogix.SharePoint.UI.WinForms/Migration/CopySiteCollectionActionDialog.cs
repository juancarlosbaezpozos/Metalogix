using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopySiteCollectionActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCSiteCollectionOptions w_tcSiteCollection = new TCSiteCollectionOptions();

		private TCSiteContentOptions w_tcSiteContent = new TCSiteContentOptions();

		private TCListContentOptions w_tcListContent = new TCListContentOptions();

		private TCPermissionsOptions w_tcPermissions = new TCPermissionsOptions();

		private TCMappingOptions w_tcMapping = new TCMappingOptions();

		private TCFilterOptions w_tcFilters = new TCFilterOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCWebPartsOptions w_tcWebParts = new TCWebPartsOptions();

		private TCTaxonomyOptions w_tcTaxonomy = new TCTaxonomyOptions();

		private TCWorkflowOptions w_tcWorkflows = new TCWorkflowOptions();

		private TCMigrationModeOptions w_tcMigrationMode = new TCMigrationModeOptions();

		private TCStoragePointOptionsMMS w_tcStoragePoint = new TCStoragePointOptionsMMS();

		private bool _targetIsCSOM;

		private IContainer components;

		public PasteSiteCollectionOptions Options
		{
			get
			{
				return Action.Options as PasteSiteCollectionOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopySiteCollectionActionDialog(bool targetCSOM)
		{
			InitializeComponent();
			Text = "Configure Site Collection Copying Options";
			_targetIsCSOM = targetCSOM;
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode, w_tcSiteCollection, w_tcSiteContent, w_tcListContent, w_tcTaxonomy, w_tcWebParts, w_tcPermissions, w_tcMapping, w_tcFilters, w_tcWorkflows };
			if (!_targetIsCSOM)
			{
				list.Add(w_tcStoragePoint);
			}
			list.Add(w_tcGeneral);
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.SiteCollection;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopySiteCollectionActionDialog));
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "CopySiteCollectionActionDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPSiteCollectionOptions sPSiteCollectionOptions = new SPSiteCollectionOptions();
			sPSiteCollectionOptions.SetFromOptions(action.Options);
			w_tcSiteCollection.Options = sPSiteCollectionOptions;
			SPSiteContentOptions sPSiteContentOptions = new SPSiteContentOptions();
			sPSiteContentOptions.SetFromOptions(action.Options);
			w_tcSiteContent.Options = sPSiteContentOptions;
			SPListContentOptions sPListContentOptions = new SPListContentOptions();
			sPListContentOptions.SetFromOptions(action.Options);
			w_tcListContent.Options = sPListContentOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			w_tcTaxonomy.Options = sPTaxonomyOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			w_tcWebParts.Options = sPWebPartOptions;
			w_tcWebParts.ShowCopyWebPartsRecursive = false;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			w_tcPermissions.Options = sPPermissionsOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			w_tcFilters.Options = sPFilterOptions;
			SPWorkflowOptions sPWorkflowOptions = new SPWorkflowOptions();
			sPWorkflowOptions.SetFromOptions(action.Options);
			w_tcWorkflows.Options = sPWorkflowOptions;
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGeneral.Options = sPGeneralOptions;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
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
			if (!base.IsModeSwitched && w_tcMigrationMode.Options.OverwriteSites && w_tcSiteCollection.IsTargetSameAsSource())
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
			action.Options.SetFromOptions(w_tcSiteContent.Options);
			action.Options.SetFromOptions(w_tcWorkflows.Options);
			action.Options.SetFromOptions(w_tcSiteCollection.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			if (_targetIsCSOM)
			{
				action.Options.SetFromOptions(w_tcStoragePoint.Options);
			}
			return true;
		}
	}
}
