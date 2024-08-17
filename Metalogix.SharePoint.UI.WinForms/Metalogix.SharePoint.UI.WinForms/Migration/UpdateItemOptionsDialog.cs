using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class UpdateItemOptionsDialog : XtraForm
	{
		private int m_iUpdateOptionsBitField;

		private IContainer components;

		private CheckEdit w_cbCoreMetadata;

		private CheckEdit w_cbPermissions;

		private SimpleButton w_btnCancel;

		private SimpleButton w_btnOK;

		public int UpdateItemOptionsBitField => m_iUpdateOptionsBitField;

		public UpdateItemOptionsDialog(int iUpdateItemBitField)
		{
			InitializeComponent();
			m_iUpdateOptionsBitField = iUpdateItemBitField;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.UpdateItemOptionsDialog));
			this.w_cbCoreMetadata = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPermissions = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbCoreMetadata, "w_cbCoreMetadata");
			this.w_cbCoreMetadata.Name = "w_cbCoreMetadata";
			resources.ApplyResources(this.w_cbPermissions, "w_cbPermissions");
			this.w_cbPermissions.Name = "w_cbPermissions";
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(On_btnOK_Click);
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.ControlBox = false;
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_cbPermissions);
			base.Controls.Add(this.w_cbCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateItemOptionsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			w_cbCoreMetadata.Checked = (1 & m_iUpdateOptionsBitField) > 0;
			w_cbPermissions.Checked = (2 & m_iUpdateOptionsBitField) > 0;
		}

		private void On_btnOK_Click(object sender, EventArgs e)
		{
			SaveUI();
		}

		private void SaveUI()
		{
			m_iUpdateOptionsBitField = 0;
			m_iUpdateOptionsBitField |= (w_cbCoreMetadata.Checked ? 1 : 0);
			m_iUpdateOptionsBitField |= (w_cbPermissions.Checked ? 2 : 0);
		}

		private void UpdateEnabledState()
		{
		}
	}
}
