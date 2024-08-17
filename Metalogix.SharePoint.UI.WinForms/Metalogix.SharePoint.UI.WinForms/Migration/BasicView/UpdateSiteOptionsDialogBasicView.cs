using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class UpdateSiteOptionsDialogBasicView : XtraForm
	{
		private int _updateOptionsBitField;

		private IContainer components;

		private ToggleSwitch tsCoreMetadata;

		private ToggleSwitch tsSiteColumns;

		private ToggleSwitch tsContentTypes;

		private ToggleSwitch tsFeatures;

		private ToggleSwitch tsNavigation;

		private ToggleSwitch tsWebParts;

		private ToggleSwitch tsPermissionLevels;

		private ToggleSwitch tsPermissions;

		private SimpleButton btnCancel;

		private SimpleButton btnOK;

		private ToggleSwitch tsAssociatedGroupSettings;

		public int UpdateSiteOptionsBitField => _updateOptionsBitField;

		public UpdateSiteOptionsDialogBasicView(int updateSiteBitField)
		{
			InitializeComponent();
			InitializeFormControls(updateSiteBitField);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UpdateSiteOptionsDialogBasicView));
			this.tsCoreMetadata = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsSiteColumns = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsContentTypes = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsFeatures = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsNavigation = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsWebParts = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsPermissionLevels = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsPermissions = new DevExpress.XtraEditors.ToggleSwitch();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.tsAssociatedGroupSettings = new DevExpress.XtraEditors.ToggleSwitch();
			((System.ComponentModel.ISupportInitialize)this.tsCoreMetadata.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsSiteColumns.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsContentTypes.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsFeatures.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsNavigation.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsPermissionLevels.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsAssociatedGroupSettings.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.tsCoreMetadata, "tsCoreMetadata");
			this.tsCoreMetadata.Name = "tsCoreMetadata";
			this.tsCoreMetadata.Properties.Appearance.Options.UseTextOptions = true;
			this.tsCoreMetadata.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsCoreMetadata.Properties.OffText = resources.GetString("tsCoreMetadata.Properties.OffText");
			this.tsCoreMetadata.Properties.OnText = resources.GetString("tsCoreMetadata.Properties.OnText");
			resources.ApplyResources(this.tsSiteColumns, "tsSiteColumns");
			this.tsSiteColumns.Name = "tsSiteColumns";
			this.tsSiteColumns.Properties.Appearance.Options.UseTextOptions = true;
			this.tsSiteColumns.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsSiteColumns.Properties.OffText = resources.GetString("tsSiteColumns.Properties.OffText");
			this.tsSiteColumns.Properties.OnText = resources.GetString("tsSiteColumns.Properties.OnText");
			resources.ApplyResources(this.tsContentTypes, "tsContentTypes");
			this.tsContentTypes.Name = "tsContentTypes";
			this.tsContentTypes.Properties.Appearance.Options.UseTextOptions = true;
			this.tsContentTypes.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsContentTypes.Properties.OffText = resources.GetString("tsContentTypes.Properties.OffText");
			this.tsContentTypes.Properties.OnText = resources.GetString("tsContentTypes.Properties.OnText");
			resources.ApplyResources(this.tsFeatures, "tsFeatures");
			this.tsFeatures.Name = "tsFeatures";
			this.tsFeatures.Properties.Appearance.Options.UseTextOptions = true;
			this.tsFeatures.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsFeatures.Properties.OffText = resources.GetString("tsFeatures.Properties.OffText");
			this.tsFeatures.Properties.OnText = resources.GetString("tsFeatures.Properties.OnText");
			resources.ApplyResources(this.tsNavigation, "tsNavigation");
			this.tsNavigation.Name = "tsNavigation";
			this.tsNavigation.Properties.Appearance.Options.UseTextOptions = true;
			this.tsNavigation.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsNavigation.Properties.OffText = resources.GetString("tsNavigation.Properties.OffText");
			this.tsNavigation.Properties.OnText = resources.GetString("tsNavigation.Properties.OnText");
			resources.ApplyResources(this.tsWebParts, "tsWebParts");
			this.tsWebParts.Name = "tsWebParts";
			this.tsWebParts.Properties.Appearance.Options.UseTextOptions = true;
			this.tsWebParts.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsWebParts.Properties.OffText = resources.GetString("tsWebParts.Properties.OffText");
			this.tsWebParts.Properties.OnText = resources.GetString("tsWebParts.Properties.OnText");
			resources.ApplyResources(this.tsPermissionLevels, "tsPermissionLevels");
			this.tsPermissionLevels.Name = "tsPermissionLevels";
			this.tsPermissionLevels.Properties.Appearance.Options.UseTextOptions = true;
			this.tsPermissionLevels.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsPermissionLevels.Properties.OffText = resources.GetString("tsPermissionLevels.Properties.OffText");
			this.tsPermissionLevels.Properties.OnText = resources.GetString("tsPermissionLevels.Properties.OnText");
			resources.ApplyResources(this.tsPermissions, "tsPermissions");
			this.tsPermissions.Name = "tsPermissions";
			this.tsPermissions.Properties.Appearance.Options.UseTextOptions = true;
			this.tsPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsPermissions.Properties.OffText = resources.GetString("tsPermissions.Properties.OffText");
			this.tsPermissions.Properties.OnText = resources.GetString("tsPermissions.Properties.OnText");
			this.tsPermissions.Toggled += new System.EventHandler(tsPermissions_Toggled);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(On_btnOK_Click);
			resources.ApplyResources(this.tsAssociatedGroupSettings, "tsAssociatedGroupSettings");
			this.tsAssociatedGroupSettings.Name = "tsAssociatedGroupSettings";
			this.tsAssociatedGroupSettings.Properties.Appearance.Options.UseTextOptions = true;
			this.tsAssociatedGroupSettings.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsAssociatedGroupSettings.Properties.OffText = resources.GetString("tsAssociatedGroupSettings.Properties.OffText");
			this.tsAssociatedGroupSettings.Properties.OnText = resources.GetString("tsAssociatedGroupSettings.Properties.OnText");
			base.AcceptButton = this.btnOK;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("UpdateSiteOptionsDialogBasicView.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ControlBox = false;
			base.Controls.Add(this.tsAssociatedGroupSettings);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.tsPermissionLevels);
			base.Controls.Add(this.tsPermissions);
			base.Controls.Add(this.tsWebParts);
			base.Controls.Add(this.tsFeatures);
			base.Controls.Add(this.tsNavigation);
			base.Controls.Add(this.tsContentTypes);
			base.Controls.Add(this.tsSiteColumns);
			base.Controls.Add(this.tsCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateSiteOptionsDialogBasicView";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)this.tsCoreMetadata.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsSiteColumns.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsContentTypes.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsFeatures.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsNavigation.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsPermissionLevels.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsAssociatedGroupSettings.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void InitializeFormControls(int updateSiteBitField)
		{
			_updateOptionsBitField = updateSiteBitField;
			LoadUI();
			UpdateEnabledState();
		}

		private void LoadUI()
		{
			tsCoreMetadata.IsOn = (1 & _updateOptionsBitField) > 0;
			tsContentTypes.IsOn = (0x10 & _updateOptionsBitField) > 0;
			tsFeatures.IsOn = (0x800 & _updateOptionsBitField) > 0;
			tsNavigation.IsOn = (0x20 & _updateOptionsBitField) > 0;
			tsSiteColumns.IsOn = (2 & _updateOptionsBitField) > 0;
			tsWebParts.IsOn = (0x40 & _updateOptionsBitField) > 0;
			tsPermissionLevels.IsOn = (8 & _updateOptionsBitField) > 0;
			tsPermissions.IsOn = (4 & _updateOptionsBitField) > 0;
			tsAssociatedGroupSettings.IsOn = (0x2000 & _updateOptionsBitField) > 0;
		}

		private void On_btnOK_Click(object sender, EventArgs e)
		{
			SaveUI();
		}

		private void SaveUI()
		{
			_updateOptionsBitField = 0;
			_updateOptionsBitField |= (tsCoreMetadata.IsOn ? 1 : 0);
			_updateOptionsBitField |= (tsContentTypes.IsOn ? 16 : 0);
			_updateOptionsBitField |= (tsFeatures.IsOn ? 2048 : 0);
			_updateOptionsBitField |= (tsNavigation.IsOn ? 32 : 0);
			_updateOptionsBitField |= (tsPermissionLevels.IsOn ? 8 : 0);
			_updateOptionsBitField |= (tsPermissions.IsOn ? 4 : 0);
			_updateOptionsBitField |= (tsSiteColumns.IsOn ? 2 : 0);
			_updateOptionsBitField |= (tsWebParts.IsOn ? 64 : 0);
			_updateOptionsBitField |= (tsAssociatedGroupSettings.IsOn ? 8192 : 0);
		}

		private void tsPermissions_Toggled(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void UpdateEnabledState()
		{
			tsPermissionLevels.Enabled = tsPermissions.IsOn;
			tsPermissionLevels.IsOn = tsPermissionLevels.IsOn && tsPermissionLevels.Enabled;
		}
	}
}
