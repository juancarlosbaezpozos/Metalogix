using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class UpdateSiteOptionsDialog : XtraForm
	{
		private int m_iUpdateOptionsBitField;

		private IContainer components;

		private CheckEdit w_cbCoreMetadata;

		private CheckEdit w_cbSiteColumns;

		private CheckEdit w_cbContentTypes;

		private CheckEdit w_cbSiteTheme;

		private CheckEdit w_cbFeatures;

		private CheckEdit w_cbNavigation;

		private CheckEdit w_cbWebParts;

		private CheckEdit w_cbRequestAccessSettings;

		private CheckEdit w_cbMasterPage;

		private CheckEdit w_cbPermissionLevels;

		private CheckEdit w_cbPermissions;

		private CheckEdit w_cbWorkflows;

		private SimpleButton w_btnCancel;

		private SimpleButton w_btnOK;

		private CheckEdit w_cbAssociatedGroupSettings;

		public int UpdateSiteOptionsBitField => m_iUpdateOptionsBitField;

		public UpdateSiteOptionsDialog(int iUpdateSiteBitField)
		{
			InitializeComponent();
			m_iUpdateOptionsBitField = iUpdateSiteBitField;
			LoadUI();
			UpdateEnabledState();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.UpdateSiteOptionsDialog));
			this.w_cbCoreMetadata = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbSiteColumns = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbContentTypes = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbSiteTheme = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbFeatures = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbNavigation = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbRequestAccessSettings = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbMasterPage = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPermissionLevels = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbWorkflows = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbAssociatedGroupSettings = new DevExpress.XtraEditors.CheckEdit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbCoreMetadata, "w_cbCoreMetadata");
			this.w_cbCoreMetadata.Name = "w_cbCoreMetadata";
			resources.ApplyResources(this.w_cbSiteColumns, "w_cbSiteColumns");
			this.w_cbSiteColumns.Name = "w_cbSiteColumns";
			resources.ApplyResources(this.w_cbContentTypes, "w_cbContentTypes");
			this.w_cbContentTypes.Name = "w_cbContentTypes";
			resources.ApplyResources(this.w_cbSiteTheme, "w_cbSiteTheme");
			this.w_cbSiteTheme.Name = "w_cbSiteTheme";
			resources.ApplyResources(this.w_cbFeatures, "w_cbFeatures");
			this.w_cbFeatures.Name = "w_cbFeatures";
			resources.ApplyResources(this.w_cbNavigation, "w_cbNavigation");
			this.w_cbNavigation.Name = "w_cbNavigation";
			resources.ApplyResources(this.w_cbWebParts, "w_cbWebParts");
			this.w_cbWebParts.Name = "w_cbWebParts";
			resources.ApplyResources(this.w_cbRequestAccessSettings, "w_cbRequestAccessSettings");
			this.w_cbRequestAccessSettings.Name = "w_cbRequestAccessSettings";
			resources.ApplyResources(this.w_cbMasterPage, "w_cbMasterPage");
			this.w_cbMasterPage.Name = "w_cbMasterPage";
			resources.ApplyResources(this.w_cbPermissionLevels, "w_cbPermissionLevels");
			this.w_cbPermissionLevels.Name = "w_cbPermissionLevels";
			resources.ApplyResources(this.w_cbPermissions, "w_cbPermissions");
			this.w_cbPermissions.Name = "w_cbPermissions";
			this.w_cbPermissions.CheckedChanged += new System.EventHandler(On_cbPermissions_CheckedChanged);
			resources.ApplyResources(this.w_cbWorkflows, "w_cbWorkflows");
			this.w_cbWorkflows.Name = "w_cbWorkflows";
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(On_btnOK_Click);
			resources.ApplyResources(this.w_cbAssociatedGroupSettings, "w_cbAssociatedGroupSettings");
			this.w_cbAssociatedGroupSettings.Name = "w_cbAssociatedGroupSettings";
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.ControlBox = false;
			base.Controls.Add(this.w_cbAssociatedGroupSettings);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_cbPermissionLevels);
			base.Controls.Add(this.w_cbPermissions);
			base.Controls.Add(this.w_cbWorkflows);
			base.Controls.Add(this.w_cbWebParts);
			base.Controls.Add(this.w_cbRequestAccessSettings);
			base.Controls.Add(this.w_cbMasterPage);
			base.Controls.Add(this.w_cbSiteTheme);
			base.Controls.Add(this.w_cbFeatures);
			base.Controls.Add(this.w_cbNavigation);
			base.Controls.Add(this.w_cbContentTypes);
			base.Controls.Add(this.w_cbSiteColumns);
			base.Controls.Add(this.w_cbCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateSiteOptionsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			w_cbCoreMetadata.Checked = (1 & m_iUpdateOptionsBitField) > 0;
			w_cbContentTypes.Checked = (0x10 & m_iUpdateOptionsBitField) > 0;
			w_cbFeatures.Checked = (0x800 & m_iUpdateOptionsBitField) > 0;
			w_cbNavigation.Checked = (0x20 & m_iUpdateOptionsBitField) > 0;
			w_cbSiteColumns.Checked = (2 & m_iUpdateOptionsBitField) > 0;
			w_cbWebParts.Checked = (0x40 & m_iUpdateOptionsBitField) > 0;
			w_cbMasterPage.Checked = false;
			w_cbPermissionLevels.Checked = (8 & m_iUpdateOptionsBitField) > 0;
			w_cbPermissions.Checked = (4 & m_iUpdateOptionsBitField) > 0;
			w_cbAssociatedGroupSettings.Checked = (0x2000 & m_iUpdateOptionsBitField) > 0;
			w_cbRequestAccessSettings.Checked = false;
			w_cbSiteTheme.Checked = false;
			w_cbWorkflows.Checked = false;
		}

		private void On_btnOK_Click(object sender, EventArgs e)
		{
			SaveUI();
		}

		private void On_cbPermissions_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void SaveUI()
		{
			m_iUpdateOptionsBitField = 0;
			m_iUpdateOptionsBitField |= (w_cbCoreMetadata.Checked ? 1 : 0);
			m_iUpdateOptionsBitField |= (w_cbContentTypes.Checked ? 16 : 0);
			m_iUpdateOptionsBitField |= (w_cbFeatures.Checked ? 2048 : 0);
			m_iUpdateOptionsBitField |= (w_cbMasterPage.Checked ? 128 : 0);
			m_iUpdateOptionsBitField |= (w_cbNavigation.Checked ? 32 : 0);
			m_iUpdateOptionsBitField |= (w_cbPermissionLevels.Checked ? 8 : 0);
			m_iUpdateOptionsBitField |= (w_cbPermissions.Checked ? 4 : 0);
			m_iUpdateOptionsBitField |= (w_cbRequestAccessSettings.Checked ? 1024 : 0);
			m_iUpdateOptionsBitField |= (w_cbSiteColumns.Checked ? 2 : 0);
			m_iUpdateOptionsBitField |= (w_cbSiteTheme.Checked ? 512 : 0);
			m_iUpdateOptionsBitField |= (w_cbWebParts.Checked ? 64 : 0);
			m_iUpdateOptionsBitField |= (w_cbWorkflows.Checked ? 4096 : 0);
			m_iUpdateOptionsBitField |= (w_cbAssociatedGroupSettings.Checked ? 8192 : 0);
		}

		private void UpdateEnabledState()
		{
			w_cbPermissionLevels.Enabled = w_cbPermissions.Checked;
			w_cbPermissionLevels.Checked = w_cbPermissionLevels.Checked && w_cbPermissionLevels.Enabled;
		}
	}
}
