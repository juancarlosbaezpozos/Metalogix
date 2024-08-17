using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.UserAndPermissionsOptions.png")]
	[ControlName("Permissions Options")]
	public class TCPermissionsOptions : MigrationElementsScopabbleTabbableControl
	{
		private SPPermissionsOptions m_options;

		private bool m_bIndependant;

		private bool m_bForceOverwriteExistingPermissions;

		private IContainer components;

		internal CheckEdit w_cbItemPermissions;

		private CheckEdit w_cbSitePermissions;

		private CheckEdit w_cbListPermissions;

		private CheckEdit w_cbFolderPermissions;

		internal PanelControl w_plSites;

		internal PanelControl w_plLists;

		internal PanelControl w_plFolders;

		internal PanelControl w_plListItems;

		private CheckEdit w_cbOverrideRoleMappings;

		internal SimpleButton w_btnEditRoleMappings;

		internal CheckEdit w_cbClearRoleAssignments;

		internal CheckEdit w_cbMapRolesByName;

		internal CheckEdit w_cbCopyRootPermissions;

		private CheckEdit w_cbCopyAccessRequests;

		private CheckEdit w_cbCopyPermissionLevels;

		internal PanelControl w_plRecurseSites;

		private CheckEdit w_cbRecurseSites;

		internal PanelControl w_plRecursiveFolders;

		private CheckEdit w_cbRecurseSubfolders;

		private CheckEdit w_cbCopyAssociatedGroups;

		private PanelControl _siteSettingOptionsPanel;

		private bool ForceOverwriteExistingPermissions
		{
			get
			{
				return m_bForceOverwriteExistingPermissions;
			}
			set
			{
				m_bForceOverwriteExistingPermissions = value;
			}
		}

		public bool IndependantlyScoped
		{
			get
			{
				return m_bIndependant;
			}
			set
			{
				m_bIndependant = value;
			}
		}

		public SPPermissionsOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public TCPermissionsOptions()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private FilterBuilderType[] GetFilterTypes()
		{
			if (IndependantlyScoped)
			{
				return null;
			}
			List<FilterBuilderType> list = new List<FilterBuilderType>();
			switch (base.Scope)
			{
			case SharePointObjectScope.SiteCollection:
			{
				Type[] collection15 = new Type[1] { typeof(SPNode) };
				list.Add(new FilterBuilderType(new List<Type>(collection15), bAllowFreeFormEntry: false));
				Type[] collection16 = new Type[1] { typeof(SPWeb) };
				list.Add(new FilterBuilderType(new List<Type>(collection16), bAllowFreeFormEntry: true));
				Type[] collection17 = new Type[1] { typeof(SPList) };
				list.Add(new FilterBuilderType(new List<Type>(collection17), bAllowFreeFormEntry: true));
				Type[] collection18 = new Type[1] { typeof(SPFolder) };
				list.Add(new FilterBuilderType(new List<Type>(collection18), bAllowFreeFormEntry: true));
				Type[] collection19 = new Type[1] { typeof(SPListItem) };
				list.Add(new FilterBuilderType(new List<Type>(collection19), bAllowFreeFormEntry: true));
				break;
			}
			case SharePointObjectScope.Site:
			{
				Type[] collection10 = new Type[1] { typeof(SPNode) };
				list.Add(new FilterBuilderType(new List<Type>(collection10), bAllowFreeFormEntry: false));
				Type[] collection11 = new Type[1] { typeof(SPWeb) };
				list.Add(new FilterBuilderType(new List<Type>(collection11), bAllowFreeFormEntry: true));
				Type[] collection12 = new Type[1] { typeof(SPList) };
				list.Add(new FilterBuilderType(new List<Type>(collection12), bAllowFreeFormEntry: true));
				Type[] collection13 = new Type[1] { typeof(SPFolder) };
				list.Add(new FilterBuilderType(new List<Type>(collection13), bAllowFreeFormEntry: true));
				Type[] collection14 = new Type[1] { typeof(SPListItem) };
				list.Add(new FilterBuilderType(new List<Type>(collection14), bAllowFreeFormEntry: true));
				break;
			}
			case SharePointObjectScope.List:
			{
				Type[] collection6 = new Type[1] { typeof(SPNode) };
				list.Add(new FilterBuilderType(new List<Type>(collection6), bAllowFreeFormEntry: false));
				Type[] collection7 = new Type[1] { typeof(SPList) };
				list.Add(new FilterBuilderType(new List<Type>(collection7), bAllowFreeFormEntry: true));
				Type[] collection8 = new Type[1] { typeof(SPFolder) };
				list.Add(new FilterBuilderType(new List<Type>(collection8), bAllowFreeFormEntry: true));
				Type[] collection9 = new Type[1] { typeof(SPListItem) };
				list.Add(new FilterBuilderType(new List<Type>(collection9), bAllowFreeFormEntry: true));
				break;
			}
			case SharePointObjectScope.Folder:
			{
				Type[] collection3 = new Type[1] { typeof(SPNode) };
				list.Add(new FilterBuilderType(new List<Type>(collection3), bAllowFreeFormEntry: false));
				Type[] collection4 = new Type[1] { typeof(SPFolder) };
				list.Add(new FilterBuilderType(new List<Type>(collection4), bAllowFreeFormEntry: true));
				Type[] collection5 = new Type[1] { typeof(SPListItem) };
				list.Add(new FilterBuilderType(new List<Type>(collection5), bAllowFreeFormEntry: true));
				break;
			}
			case SharePointObjectScope.Item:
			{
				Type[] collection = new Type[1] { typeof(SPNode) };
				list.Add(new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: false));
				Type[] collection2 = new Type[1] { typeof(SPListItem) };
				list.Add(new FilterBuilderType(new List<Type>(collection2), bAllowFreeFormEntry: true));
				break;
			}
			}
			return list.ToArray();
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			if (sMessage == "MigrationModeChanged")
			{
				if (((MigrationModeChangedInfo)oValue).NewMigrationMode != 0)
				{
					ForceOverwriteExistingPermissions = false;
				}
				else
				{
					ForceOverwriteExistingPermissions = true;
				}
				UpdateEnabledState();
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCPermissionsOptions));
			this.w_plSites = new DevExpress.XtraEditors.PanelControl();
			this._siteSettingOptionsPanel = new DevExpress.XtraEditors.PanelControl();
			this.w_cbCopyAccessRequests = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyAssociatedGroups = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyPermissionLevels = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbSitePermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_plLists = new DevExpress.XtraEditors.PanelControl();
			this.w_cbListPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_plFolders = new DevExpress.XtraEditors.PanelControl();
			this.w_cbFolderPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_plListItems = new DevExpress.XtraEditors.PanelControl();
			this.w_cbItemPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnEditRoleMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbOverrideRoleMappings = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbClearRoleAssignments = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbMapRolesByName = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyRootPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_plRecurseSites = new DevExpress.XtraEditors.PanelControl();
			this.w_cbRecurseSites = new DevExpress.XtraEditors.CheckEdit();
			this.w_plRecursiveFolders = new DevExpress.XtraEditors.PanelControl();
			this.w_cbRecurseSubfolders = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_plSites).BeginInit();
			this.w_plSites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._siteSettingOptionsPanel).BeginInit();
			this._siteSettingOptionsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAccessRequests.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAssociatedGroups.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyPermissionLevels.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSitePermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLists).BeginInit();
			this.w_plLists.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbListPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plFolders).BeginInit();
			this.w_plFolders.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbFolderPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plListItems).BeginInit();
			this.w_plListItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbItemPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverrideRoleMappings.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbClearRoleAssignments.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapRolesByName.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyRootPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plRecurseSites).BeginInit();
			this.w_plRecurseSites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbRecurseSites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plRecursiveFolders).BeginInit();
			this.w_plRecursiveFolders.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbRecurseSubfolders.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_plSites, "w_plSites");
			this.w_plSites.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plSites.Appearance.BackColor");
			this.w_plSites.Appearance.Options.UseBackColor = true;
			this.w_plSites.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSites.Controls.Add(this._siteSettingOptionsPanel);
			this.w_plSites.Controls.Add(this.w_cbCopyPermissionLevels);
			this.w_plSites.Controls.Add(this.w_cbSitePermissions);
			this.w_plSites.Name = "w_plSites";
			resources.ApplyResources(this._siteSettingOptionsPanel, "_siteSettingOptionsPanel");
			this._siteSettingOptionsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._siteSettingOptionsPanel.Controls.Add(this.w_cbCopyAccessRequests);
			this._siteSettingOptionsPanel.Controls.Add(this.w_cbCopyAssociatedGroups);
			this._siteSettingOptionsPanel.Name = "_siteSettingOptionsPanel";
			resources.ApplyResources(this.w_cbCopyAccessRequests, "w_cbCopyAccessRequests");
			this.w_cbCopyAccessRequests.Name = "w_cbCopyAccessRequests";
			this.w_cbCopyAccessRequests.Properties.AutoWidth = true;
			this.w_cbCopyAccessRequests.Properties.Caption = resources.GetString("w_cbCopyAccessRequests.Properties.Caption");
			resources.ApplyResources(this.w_cbCopyAssociatedGroups, "w_cbCopyAssociatedGroups");
			this.w_cbCopyAssociatedGroups.Name = "w_cbCopyAssociatedGroups";
			this.w_cbCopyAssociatedGroups.Properties.AutoWidth = true;
			this.w_cbCopyAssociatedGroups.Properties.Caption = resources.GetString("w_cbCopyAssociatedGroups.Properties.Caption");
			resources.ApplyResources(this.w_cbCopyPermissionLevels, "w_cbCopyPermissionLevels");
			this.w_cbCopyPermissionLevels.Name = "w_cbCopyPermissionLevels";
			this.w_cbCopyPermissionLevels.Properties.AutoWidth = true;
			this.w_cbCopyPermissionLevels.Properties.Caption = resources.GetString("w_cbCopyPermissionLevels.Properties.Caption");
			resources.ApplyResources(this.w_cbSitePermissions, "w_cbSitePermissions");
			this.w_cbSitePermissions.Name = "w_cbSitePermissions";
			this.w_cbSitePermissions.Properties.AutoWidth = true;
			this.w_cbSitePermissions.Properties.Caption = resources.GetString("w_cbSitePermissions.Properties.Caption");
			this.w_cbSitePermissions.CheckedChanged += new System.EventHandler(On_Permissions_CheckedChanged);
			resources.ApplyResources(this.w_plLists, "w_plLists");
			this.w_plLists.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plLists.Appearance.BackColor");
			this.w_plLists.Appearance.Options.UseBackColor = true;
			this.w_plLists.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plLists.Controls.Add(this.w_cbListPermissions);
			this.w_plLists.Name = "w_plLists";
			resources.ApplyResources(this.w_cbListPermissions, "w_cbListPermissions");
			this.w_cbListPermissions.Name = "w_cbListPermissions";
			this.w_cbListPermissions.Properties.AutoWidth = true;
			this.w_cbListPermissions.Properties.Caption = resources.GetString("w_cbListPermissions.Properties.Caption");
			this.w_cbListPermissions.CheckedChanged += new System.EventHandler(On_Permissions_CheckedChanged);
			resources.ApplyResources(this.w_plFolders, "w_plFolders");
			this.w_plFolders.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plFolders.Appearance.BackColor");
			this.w_plFolders.Appearance.Options.UseBackColor = true;
			this.w_plFolders.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plFolders.Controls.Add(this.w_cbFolderPermissions);
			this.w_plFolders.Name = "w_plFolders";
			resources.ApplyResources(this.w_cbFolderPermissions, "w_cbFolderPermissions");
			this.w_cbFolderPermissions.Name = "w_cbFolderPermissions";
			this.w_cbFolderPermissions.Properties.AutoWidth = true;
			this.w_cbFolderPermissions.Properties.Caption = resources.GetString("w_cbFolderPermissions.Properties.Caption");
			this.w_cbFolderPermissions.CheckedChanged += new System.EventHandler(On_Permissions_CheckedChanged);
			resources.ApplyResources(this.w_plListItems, "w_plListItems");
			this.w_plListItems.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plListItems.Appearance.BackColor");
			this.w_plListItems.Appearance.Options.UseBackColor = true;
			this.w_plListItems.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plListItems.Controls.Add(this.w_cbItemPermissions);
			this.w_plListItems.Name = "w_plListItems";
			resources.ApplyResources(this.w_cbItemPermissions, "w_cbItemPermissions");
			this.w_cbItemPermissions.Name = "w_cbItemPermissions";
			this.w_cbItemPermissions.Properties.AutoWidth = true;
			this.w_cbItemPermissions.Properties.Caption = resources.GetString("w_cbItemPermissions.Properties.Caption");
			this.w_cbItemPermissions.CheckedChanged += new System.EventHandler(On_Permissions_CheckedChanged);
			resources.ApplyResources(this.w_btnEditRoleMappings, "w_btnEditRoleMappings");
			this.w_btnEditRoleMappings.Name = "w_btnEditRoleMappings";
			this.w_btnEditRoleMappings.Click += new System.EventHandler(On_EditRoleMappingOverrides_Clicked);
			resources.ApplyResources(this.w_cbOverrideRoleMappings, "w_cbOverrideRoleMappings");
			this.w_cbOverrideRoleMappings.Name = "w_cbOverrideRoleMappings";
			this.w_cbOverrideRoleMappings.Properties.AutoWidth = true;
			this.w_cbOverrideRoleMappings.Properties.Caption = resources.GetString("w_cbOverrideRoleMappings.Properties.Caption");
			this.w_cbOverrideRoleMappings.CheckedChanged += new System.EventHandler(On_OverrideRoleMappings_CheckedChanged);
			resources.ApplyResources(this.w_cbClearRoleAssignments, "w_cbClearRoleAssignments");
			this.w_cbClearRoleAssignments.Name = "w_cbClearRoleAssignments";
			this.w_cbClearRoleAssignments.Properties.AutoWidth = true;
			this.w_cbClearRoleAssignments.Properties.Caption = resources.GetString("w_cbClearRoleAssignments.Properties.Caption");
			resources.ApplyResources(this.w_cbMapRolesByName, "w_cbMapRolesByName");
			this.w_cbMapRolesByName.Name = "w_cbMapRolesByName";
			this.w_cbMapRolesByName.Properties.AutoWidth = true;
			this.w_cbMapRolesByName.Properties.Caption = resources.GetString("w_cbMapRolesByName.Properties.Caption");
			resources.ApplyResources(this.w_cbCopyRootPermissions, "w_cbCopyRootPermissions");
			this.w_cbCopyRootPermissions.Name = "w_cbCopyRootPermissions";
			this.w_cbCopyRootPermissions.Properties.AutoWidth = true;
			this.w_cbCopyRootPermissions.Properties.Caption = resources.GetString("w_cbCopyRootPermissions.Properties.Caption");
			resources.ApplyResources(this.w_plRecurseSites, "w_plRecurseSites");
			this.w_plRecurseSites.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plRecurseSites.Appearance.BackColor");
			this.w_plRecurseSites.Appearance.Options.UseBackColor = true;
			this.w_plRecurseSites.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plRecurseSites.Controls.Add(this.w_cbRecurseSites);
			this.w_plRecurseSites.Name = "w_plRecurseSites";
			resources.ApplyResources(this.w_cbRecurseSites, "w_cbRecurseSites");
			this.w_cbRecurseSites.Name = "w_cbRecurseSites";
			this.w_cbRecurseSites.Properties.AutoWidth = true;
			this.w_cbRecurseSites.Properties.Caption = resources.GetString("w_cbRecurseSites.Properties.Caption");
			resources.ApplyResources(this.w_plRecursiveFolders, "w_plRecursiveFolders");
			this.w_plRecursiveFolders.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_plRecursiveFolders.Appearance.BackColor");
			this.w_plRecursiveFolders.Appearance.Options.UseBackColor = true;
			this.w_plRecursiveFolders.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plRecursiveFolders.Controls.Add(this.w_cbRecurseSubfolders);
			this.w_plRecursiveFolders.Name = "w_plRecursiveFolders";
			resources.ApplyResources(this.w_cbRecurseSubfolders, "w_cbRecurseSubfolders");
			this.w_cbRecurseSubfolders.Name = "w_cbRecurseSubfolders";
			this.w_cbRecurseSubfolders.Properties.AutoWidth = true;
			this.w_cbRecurseSubfolders.Properties.Caption = resources.GetString("w_cbRecurseSubfolders.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_plRecursiveFolders);
			base.Controls.Add(this.w_plRecurseSites);
			base.Controls.Add(this.w_cbCopyRootPermissions);
			base.Controls.Add(this.w_cbMapRolesByName);
			base.Controls.Add(this.w_cbClearRoleAssignments);
			base.Controls.Add(this.w_btnEditRoleMappings);
			base.Controls.Add(this.w_cbOverrideRoleMappings);
			base.Controls.Add(this.w_plListItems);
			base.Controls.Add(this.w_plSites);
			base.Controls.Add(this.w_plFolders);
			base.Controls.Add(this.w_plLists);
			base.Name = "TCPermissionsOptions";
			((System.ComponentModel.ISupportInitialize)this.w_plSites).EndInit();
			this.w_plSites.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._siteSettingOptionsPanel).EndInit();
			this._siteSettingOptionsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAccessRequests.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAssociatedGroups.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyPermissionLevels.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSitePermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLists).EndInit();
			this.w_plLists.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbListPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plFolders).EndInit();
			this.w_plFolders.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbFolderPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plListItems).EndInit();
			this.w_plListItems.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbItemPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverrideRoleMappings.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbClearRoleAssignments.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapRolesByName.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyRootPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plRecurseSites).EndInit();
			this.w_plRecurseSites.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbRecurseSites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plRecursiveFolders).EndInit();
			this.w_plRecursiveFolders.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbRecurseSubfolders.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbSitePermissions.Checked = m_options.CopySitePermissions;
			w_cbCopyPermissionLevels.Checked = m_options.CopyPermissionLevels && w_cbCopyPermissionLevels.Enabled;
			w_cbCopyAccessRequests.Checked = m_options.CopyAccessRequestSettings && w_cbCopyAccessRequests.Enabled;
			w_cbCopyAssociatedGroups.Checked = m_options.CopyAssociatedGroups && w_cbCopyAssociatedGroups.Enabled;
			w_cbListPermissions.Checked = m_options.CopyListPermissions;
			w_cbFolderPermissions.Checked = m_options.CopyFolderPermissions;
			w_cbItemPermissions.Checked = m_options.CopyItemPermissions;
			w_cbRecurseSites.Checked = m_options.RecursiveSites;
			w_cbRecurseSubfolders.Checked = m_options.RecursiveFolders;
			w_cbCopyRootPermissions.Checked = m_options.CopyRootPermissions;
			w_cbClearRoleAssignments.Checked = m_options.ClearRoleAssignments;
			w_cbMapRolesByName.Checked = m_options.MapRolesByName;
			w_cbOverrideRoleMappings.Checked = m_options.OverrideRoleMappings;
			w_btnEditRoleMappings.Tag = m_options.RoleAssignmentMappings.Clone();
			w_btnEditRoleMappings.Enabled = w_cbOverrideRoleMappings.Checked;
		}

		private void On_EditRoleMappingOverrides_Clicked(object sender, EventArgs e)
		{
			CopyRoleAssignmentsDialog copyRoleAssignmentsDialog = new CopyRoleAssignmentsDialog();
			CopyRoleAssignmentsOptions options = new CopyRoleAssignmentsOptions
			{
				RoleAssignmentMappings = (ConditionalMappingCollection)w_btnEditRoleMappings.Tag
			};
			copyRoleAssignmentsDialog.Options = options;
			copyRoleAssignmentsDialog.AvailableTypes = GetFilterTypes();
			copyRoleAssignmentsDialog.ShowDialog();
			if (copyRoleAssignmentsDialog.DialogResult == DialogResult.OK)
			{
				w_btnEditRoleMappings.Tag = copyRoleAssignmentsDialog.Options.RoleAssignmentMappings;
			}
		}

		private void On_OverrideRoleMappings_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void On_Permissions_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		public override bool SaveUI()
		{
			m_options.CopySitePermissions = w_cbSitePermissions.Checked;
			m_options.CopyPermissionLevels = w_cbCopyPermissionLevels.Checked && w_cbCopyPermissionLevels.Enabled;
			m_options.CopyAccessRequestSettings = w_cbCopyAccessRequests.Checked && w_cbCopyAccessRequests.Enabled;
			m_options.CopyAssociatedGroups = w_cbCopyAssociatedGroups.Checked && w_cbCopyAccessRequests.Enabled;
			m_options.CopyListPermissions = w_cbListPermissions.Checked;
			m_options.CopyFolderPermissions = w_cbFolderPermissions.Checked;
			m_options.CopyItemPermissions = w_cbItemPermissions.Checked;
			m_options.RecursiveSites = w_cbRecurseSites.Checked;
			m_options.RecursiveFolders = w_cbRecurseSubfolders.Checked;
			m_options.CopyRootPermissions = w_cbCopyRootPermissions.Checked && w_cbCopyRootPermissions.Enabled;
			m_options.ClearRoleAssignments = (w_cbClearRoleAssignments.Checked && w_cbClearRoleAssignments.Enabled) || ForceOverwriteExistingPermissions;
			m_options.MapRolesByName = w_cbMapRolesByName.Checked;
			m_options.OverrideRoleMappings = w_cbOverrideRoleMappings.Checked;
			m_options.RoleAssignmentMappings = (ConditionalMappingCollection)w_btnEditRoleMappings.Tag;
			return true;
		}

		protected override void UpdateEnabledState()
		{
			bool flag = base.Scope != SharePointObjectScope.List || (!BCSHelper.HasExternalListsOnly(SourceNodes) && !BCSHelper.HasExternalListsOnly(TargetNodes));
			bool flag2 = flag;
			w_cbSitePermissions.Enabled = base.SitesAvailable;
			w_cbListPermissions.Enabled = base.ListsAvailable;
			w_cbFolderPermissions.Enabled = base.FoldersAvailable && flag2;
			w_cbItemPermissions.Enabled = base.ItemsAvailable && flag2;
			w_cbFolderPermissions.Checked = w_cbFolderPermissions.Enabled && w_cbFolderPermissions.Checked;
			w_cbItemPermissions.Checked = w_cbItemPermissions.Enabled && w_cbItemPermissions.Checked;
			CheckEdit checkEdit = w_cbClearRoleAssignments;
			bool enabled = !ForceOverwriteExistingPermissions && (IndependantlyScoped || (w_cbSitePermissions.Checked && w_cbSitePermissions.Enabled) || (w_cbListPermissions.Checked && w_cbListPermissions.Enabled) || (w_cbItemPermissions.Checked && w_cbItemPermissions.Enabled) || (w_cbFolderPermissions.Checked && w_cbFolderPermissions.Enabled));
			checkEdit.Enabled = enabled;
			w_cbClearRoleAssignments.Checked = w_cbClearRoleAssignments.Checked || ForceOverwriteExistingPermissions;
			CheckEdit checkEdit2 = w_cbCopyRootPermissions;
			bool enabled2 = IndependantlyScoped || ((base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection) && w_cbSitePermissions.Checked && w_cbSitePermissions.Enabled) || (base.Scope == SharePointObjectScope.List && w_cbListPermissions.Checked && w_cbListPermissions.Enabled) || (base.Scope == SharePointObjectScope.Folder && w_cbFolderPermissions.Checked && w_cbFolderPermissions.Enabled) || (base.Scope == SharePointObjectScope.Item && w_cbItemPermissions.Checked && w_cbItemPermissions.Enabled);
			checkEdit2.Enabled = enabled2;
			w_cbCopyPermissionLevels.Enabled = w_cbSitePermissions.Checked && w_cbSitePermissions.Enabled;
			bool flag3 = false;
			if (TargetNodes != null && TargetNodes.Count > 0)
			{
				flag3 = (TargetNodes[0] as SPNode).CanWriteCreatedModifiedMetaInfo;
			}
			w_cbCopyAccessRequests.Enabled = w_cbSitePermissions.Checked && w_cbSitePermissions.Enabled && flag3;
			CheckEdit checkEdit3 = w_cbCopyAssociatedGroups;
			bool enabled3 = w_cbSitePermissions.Checked && w_cbSitePermissions.Enabled && (base.SourceWeb == null || base.SourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater);
			checkEdit3.Enabled = enabled3;
			w_btnEditRoleMappings.Enabled = w_cbOverrideRoleMappings.Checked;
		}

		protected override void UpdateScope()
		{
			if (base.Scope == SharePointObjectScope.Permissions)
			{
				HideControl(w_plSites);
				HideControl(w_plLists);
				HideControl(w_plFolders);
				HideControl(w_plListItems);
				HideControl(w_plRecurseSites);
				HideControl(w_plRecursiveFolders);
				return;
			}
			if (IndependantlyScoped)
			{
				if (base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site)
				{
					HideControl(w_plRecurseSites);
				}
				if (base.Scope != SharePointObjectScope.Folder)
				{
					HideControl(w_plRecursiveFolders);
				}
				HideControl(_siteSettingOptionsPanel);
			}
			else
			{
				HideControl(w_plRecurseSites);
				HideControl(w_plRecursiveFolders);
			}
			if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
			{
				return;
			}
			HideControl(w_plSites);
			if (base.Scope != SharePointObjectScope.List)
			{
				HideControl(w_plLists);
				if (base.Scope != SharePointObjectScope.Folder)
				{
					HideControl(w_plFolders);
				}
			}
		}
	}
}
