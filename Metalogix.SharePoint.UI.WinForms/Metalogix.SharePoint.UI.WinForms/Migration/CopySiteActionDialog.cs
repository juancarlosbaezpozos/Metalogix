using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopySiteActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCSiteContentOptions w_tcSiteContent = new TCSiteContentOptions();

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

		private bool m_bRequireRootRename;

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

		public bool RequireRootRename
		{
			get
			{
				return m_bRequireRootRename;
			}
			set
			{
				m_bRequireRootRename = value;
			}
		}

		public CopySiteActionDialog(bool targetCSOM = false)
		{
			InitializeComponent();
			Text = "Configure Site Copying Options";
			_targetIsCSOM = targetCSOM;
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode };
			w_tcSiteContent.ActionType = Action.GetType();
			list.Add(w_tcSiteContent);
			list.Add(w_tcListContent);
			list.Add(w_tcTaxonomy);
			list.Add(w_tcWebParts);
			list.Add(w_tcPermissions);
			list.Add(w_tcMapping);
			list.Add(w_tcFilters);
			list.Add(w_tcWorkflows);
			if (!_targetIsCSOM)
			{
				list.Add(w_tcStoragePoint);
			}
			list.Add(w_tcGeneral);
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.Site;
			}
			base.Tabs = list;
		}

		private bool HasValidRenameConfiguration()
		{
			if (!w_tcMapping.Options.RenameSpecificNodes || w_tcMapping.Options.TaskCollection == null)
			{
				return false;
			}
			SPWeb sPWeb = ((SourceNodes != null && SourceNodes.Count != 0) ? (SourceNodes[0] as SPWeb) : null);
			SPWeb sPWeb2 = sPWeb;
			if (sPWeb2 == null)
			{
				return true;
			}
			TransformationTask task = w_tcMapping.Options.TaskCollection.GetTask(sPWeb2, new CompareDatesInUtc());
			if (task == null || !task.ChangeOperations.ContainsKey("Name") || string.IsNullOrEmpty(task.ChangeOperations["Name"]))
			{
				return false;
			}
			return task.ChangeOperations["Name"].ToLower() != sPWeb2.Name.ToLower();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopySiteActionDialog));
			base.SuspendLayout();
			resources.ApplyResources(this, "$this");
			base.Name = "CopySiteActionDialog";
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
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
			if (!_targetIsCSOM)
			{
				ExternalizationOptions externalizationOptions = new ExternalizationOptions();
				externalizationOptions.SetFromOptions(action.Options);
				w_tcStoragePoint.Options = externalizationOptions;
			}
		}

		protected override bool SaveUI(Action action)
		{
			if (RequireRootRename && w_tcMapping.SaveUI() && !HasValidRenameConfiguration())
			{
				FlatXtraMessageBox.Show(Resources.Force_Site_Rename_Warning, Resources.Invalid_Options_Configuration, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcGeneral.Options);
			action.Options.SetFromOptions(w_tcFilters.Options);
			action.Options.SetFromOptions(w_tcMapping.Options);
			action.Options.SetFromOptions(w_tcPermissions.Options);
			action.Options.SetFromOptions(w_tcWebParts.Options);
			action.Options.SetFromOptions(w_tcWorkflows.Options);
			action.Options.SetFromOptions(w_tcTaxonomy.Options);
			action.Options.SetFromOptions(w_tcListContent.Options);
			action.Options.SetFromOptions(w_tcSiteContent.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			if (!_targetIsCSOM)
			{
				action.Options.SetFromOptions(w_tcStoragePoint.Options);
			}
			return true;
		}
	}
}
