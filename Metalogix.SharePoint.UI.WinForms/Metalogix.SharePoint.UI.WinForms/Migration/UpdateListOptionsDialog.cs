using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class UpdateListOptionsDialog : XtraForm
	{
		private int m_iUpdateOptionsBitField;

		private IContainer components;

		private SimpleButton w_btnCancel;

		private SimpleButton w_btnOK;

		private CheckEdit w_cbCoreMetadata;

		private CheckEdit w_cbFields;

		private CheckEdit w_cbViews;

		private CheckEdit w_cbContentTypes;

		private CheckEdit w_cbPermissions;

		public int UpdateListOptionsBitField => m_iUpdateOptionsBitField;

		public UpdateListOptionsDialog(int iUpdateListBitField)
		{
			InitializeComponent();
			m_iUpdateOptionsBitField = iUpdateListBitField;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.UpdateListOptionsDialog));
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCoreMetadata = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbFields = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbViews = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbContentTypes = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPermissions = new DevExpress.XtraEditors.CheckEdit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(On_btnOK_Click);
			resources.ApplyResources(this.w_cbCoreMetadata, "w_cbCoreMetadata");
			this.w_cbCoreMetadata.Name = "w_cbCoreMetadata";
			resources.ApplyResources(this.w_cbFields, "w_cbFields");
			this.w_cbFields.Name = "w_cbFields";
			resources.ApplyResources(this.w_cbViews, "w_cbViews");
			this.w_cbViews.Name = "w_cbViews";
			resources.ApplyResources(this.w_cbContentTypes, "w_cbContentTypes");
			this.w_cbContentTypes.Name = "w_cbContentTypes";
			resources.ApplyResources(this.w_cbPermissions, "w_cbPermissions");
			this.w_cbPermissions.Name = "w_cbPermissions";
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.ControlBox = false;
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_cbPermissions);
			base.Controls.Add(this.w_cbContentTypes);
			base.Controls.Add(this.w_cbViews);
			base.Controls.Add(this.w_cbFields);
			base.Controls.Add(this.w_cbCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateListOptionsDialog";
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
			w_cbFields.Checked = (2 & m_iUpdateOptionsBitField) > 0;
			w_cbPermissions.Checked = (8 & m_iUpdateOptionsBitField) > 0;
			w_cbViews.Checked = (4 & m_iUpdateOptionsBitField) > 0;
		}

		private void On_btnOK_Click(object sender, EventArgs e)
		{
			SaveUI();
		}

		private void SaveUI()
		{
			m_iUpdateOptionsBitField = 0;
			m_iUpdateOptionsBitField |= (w_cbCoreMetadata.Checked ? 1 : 0);
			m_iUpdateOptionsBitField |= (w_cbContentTypes.Checked ? 16 : 0);
			m_iUpdateOptionsBitField |= (w_cbFields.Checked ? 2 : 0);
			m_iUpdateOptionsBitField |= (w_cbPermissions.Checked ? 8 : 0);
			m_iUpdateOptionsBitField |= (w_cbViews.Checked ? 4 : 0);
		}

		private void UpdateEnabledState()
		{
		}
	}
}
