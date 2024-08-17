using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyFolderActionDialog : ScopableLeftNavigableTabsForm
	{
		private TCFolderContentOptions w_tcFolderContent = new TCFolderContentOptions();

		private TCTaxonomyOptions w_tcTaxonomyOptions = new TCTaxonomyOptions();

		private TCPermissionsOptions w_tcPermissions = new TCPermissionsOptions();

		private TCMappingOptions w_tcMapping = new TCMappingOptions();

		private TCFilterOptions w_tcFilters = new TCFilterOptions();

		private TCGeneralOptions w_tcGeneral = new TCGeneralOptions();

		private TCWebPartsOptions w_tcWebParts = new TCWebPartsOptions();

		private TCMigrationModeOptions w_tcMigrationMode = new TCMigrationModeOptions();

		private TCStoragePointOptionsMMS w_tcStoragePoint = new TCStoragePointOptionsMMS();

		private bool _targetIsCSOM;

		private IContainer components;

		public bool HideRootNodeSpecificOptions
		{
			get
			{
				return w_tcFolderContent.MultiSelectUI;
			}
			set
			{
				w_tcFolderContent.MultiSelectUI = value;
			}
		}

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

		public CopyFolderActionDialog(bool targetIsCSOM = false)
		{
			InitializeComponent();
			_targetIsCSOM = targetIsCSOM;
			Text = "Configure Folder Copying Options";
			List<TabbableControl> list = new List<TabbableControl> { w_tcMigrationMode, w_tcFolderContent, w_tcTaxonomyOptions, w_tcWebParts, w_tcPermissions, w_tcMapping, w_tcFilters };
			if (!targetIsCSOM)
			{
				list.Add(w_tcStoragePoint);
			}
			list.Add(w_tcGeneral);
			foreach (ScopableTabbableControl item in list)
			{
				item.Scope = SharePointObjectScope.Folder;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyFolderActionDialog));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyFolderActionDialog";
			base.ShowIcon = true;
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Metalogix.Actions.Action action)
		{
			base.LoadUI(action);
			w_tcFolderContent.w_cbCopySubfolders.CheckedChanged += w_cbCopySubfolders_CheckedChanged;
			SPMigrationModeOptions sPMigrationModeOptions = new SPMigrationModeOptions();
			sPMigrationModeOptions.SetFromOptions(action.Options);
			w_tcMigrationMode.Options = sPMigrationModeOptions;
			SPFolderContentOptions sPFolderContentOptions = new SPFolderContentOptions();
			sPFolderContentOptions.SetFromOptions(action.Options);
			w_tcFolderContent.Options = sPFolderContentOptions;
			SPTaxonomyOptions sPTaxonomyOptions = new SPTaxonomyOptions();
			sPTaxonomyOptions.SetFromOptions(action.Options);
			w_tcTaxonomyOptions.Options = sPTaxonomyOptions;
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			w_tcWebParts.Options = sPWebPartOptions;
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			w_tcPermissions.Options = sPPermissionsOptions;
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcMapping.Options = sPMappingOptions;
			SPFilterOptions sPFilterOptions = new SPFilterOptions();
			sPFilterOptions.SetFromOptions(action.Options);
			w_tcFilters.Options = sPFilterOptions;
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

		protected override bool SaveUI(Metalogix.Actions.Action action)
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
			action.Options.SetFromOptions(w_tcTaxonomyOptions.Options);
			action.Options.SetFromOptions(w_tcFolderContent.Options);
			action.Options.SetFromOptions(w_tcMigrationMode.Options);
			if (!_targetIsCSOM)
			{
				action.Options.SetFromOptions(w_tcStoragePoint.Options);
			}
			return true;
		}

		private void w_cbCopySubfolders_CheckedChanged(object sender, EventArgs e)
		{
			w_tcFilters.SubFoldersAvailable = w_tcFolderContent.w_cbCopySubfolders.Checked;
		}
	}
}
