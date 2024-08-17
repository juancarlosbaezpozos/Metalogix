using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class MigrationModeScopableTabbableControl : ScopableTabbableControl
	{
		private IContainer components;

		public MigrationModeScopableTabbableControl()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		protected void LoadIncrementalMode(SPMigrationModeOptions options)
		{
			if (options.MigrationMode == MigrationMode.Incremental)
			{
				if (!options.UpdateItems || options.PropagateItemDeletions || base.Scope == SharePointObjectScope.List && !options.UpdateLists || (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection) && (!options.UpdateSites || !options.UpdateLists))
				{
					options.MigrationMode = MigrationMode.Custom;
					return;
				}
				bool flag = ((options.UpdateSiteOptionsBitField & 1) <= 0 || (options.UpdateSiteOptionsBitField & 16) <= 0 || (options.UpdateSiteOptionsBitField & 2048) <= 0 || (options.UpdateSiteOptionsBitField & 32) <= 0 || (options.UpdateSiteOptionsBitField & 2) <= 0 ? false : (options.UpdateSiteOptionsBitField & 64) > 0);
				bool flag1 = ((options.UpdateListOptionsBitField & 1) <= 0 || (options.UpdateListOptionsBitField & 16) <= 0 || (options.UpdateListOptionsBitField & 2) <= 0 ? false : (options.UpdateListOptionsBitField & 4) > 0);
				if ((options.UpdateItemOptionsBitField & 1) <= 0 || base.Scope == SharePointObjectScope.List && !flag1 || (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection) && (!flag || !flag1))
				{
					options.MigrationMode = MigrationMode.Custom;
				}
			}
		}

		protected void SaveFullModeSettings(SPMigrationModeOptions options)
		{
			options.MigrationMode = MigrationMode.Full;
			options.OverwriteSites = true;
			options.OverwriteLists = true;
			options.ItemCopyingMode = ListItemCopyMode.Overwrite;
			options.UpdateSites = false;
			options.UpdateLists = false;
			options.UpdateItems = false;
			options.PropagateItemDeletions = false;
			options.UpdateSiteOptionsBitField = 0;
			options.UpdateListOptionsBitField = 0;
			options.UpdateItemOptionsBitField = 0;
		}

		protected void SaveIncrementalModeSettings(SPMigrationModeOptions options)
		{
			options.MigrationMode = MigrationMode.Incremental;
			options.OverwriteSites = false;
			options.OverwriteLists = false;
			options.ItemCopyingMode = ListItemCopyMode.Preserve;
			options.UpdateSites = true;
			options.UpdateLists = true;
			options.CheckModifiedDatesForLists = true;
			options.UpdateItems = true;
			options.CheckModifiedDatesForItemsDocuments = true;
			options.UpdateSiteOptionsBitField = 10367;
			options.UpdateListOptionsBitField = 31;
			options.UpdateItemOptionsBitField = 3;
			options.PropagateItemDeletions = false;
		}
	}
}