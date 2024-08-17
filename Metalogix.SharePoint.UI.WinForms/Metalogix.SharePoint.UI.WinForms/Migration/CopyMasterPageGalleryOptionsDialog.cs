using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyMasterPageGalleryOptionsDialog : XtraForm
	{
		private IContainer components;

		private CheckEdit w_cbCopyMasterPages;

		private CheckEdit w_cbCopyPageLayouts;

		private CheckEdit w_cbCopyOtherResources;

		private SimpleButton w_btnOK;

		private SimpleButton w_bCancel;

		private CheckEdit w_cbCorrectLinks;

		public bool CopyMasterPages
		{
			get
			{
				return w_cbCopyMasterPages.Checked;
			}
			set
			{
				w_cbCopyMasterPages.Checked = value;
			}
		}

		public bool CopyOtherResources
		{
			get
			{
				return w_cbCopyOtherResources.Checked;
			}
			set
			{
				w_cbCopyOtherResources.Checked = value;
			}
		}

		public bool CopyPageLayouts
		{
			get
			{
				return w_cbCopyPageLayouts.Checked;
			}
			set
			{
				w_cbCopyPageLayouts.Checked = value;
			}
		}

		public bool CorrectMasterPageLinks
		{
			get
			{
				return w_cbCorrectLinks.Checked;
			}
			set
			{
				w_cbCorrectLinks.Checked = value;
			}
		}

		public CopyMasterPageGalleryOptionsDialog()
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

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyMasterPageGalleryOptionsDialog));
			this.w_cbCopyMasterPages = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyPageLayouts = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyOtherResources = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCorrectLinks = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPages.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyPageLayouts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyOtherResources.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbCopyMasterPages, "w_cbCopyMasterPages");
			this.w_cbCopyMasterPages.Name = "w_cbCopyMasterPages";
			this.w_cbCopyMasterPages.Properties.Caption = resources.GetString("w_cbCopyMasterPages.Properties.Caption");
			this.w_cbCopyMasterPages.Properties.CheckedChanged += new System.EventHandler(w_cbCopyMasterPages_Properties_CheckedChanged);
			resources.ApplyResources(this.w_cbCopyPageLayouts, "w_cbCopyPageLayouts");
			this.w_cbCopyPageLayouts.Name = "w_cbCopyPageLayouts";
			this.w_cbCopyPageLayouts.Properties.Caption = resources.GetString("w_cbCopyPageLayouts.Properties.Caption");
			resources.ApplyResources(this.w_cbCopyOtherResources, "w_cbCopyOtherResources");
			this.w_cbCopyOtherResources.Name = "w_cbCopyOtherResources";
			this.w_cbCopyOtherResources.Properties.Caption = resources.GetString("w_cbCopyOtherResources.Properties.Caption");
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(On_OK_Clicked);
			resources.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			resources.ApplyResources(this.w_cbCorrectLinks, "w_cbCorrectLinks");
			this.w_cbCorrectLinks.Name = "w_cbCorrectLinks";
			this.w_cbCorrectLinks.Properties.Caption = resources.GetString("w_cbCorrectLinks.Properties.Caption");
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_cbCorrectLinks);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_cbCopyOtherResources);
			base.Controls.Add(this.w_cbCopyPageLayouts);
			base.Controls.Add(this.w_cbCopyMasterPages);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CopyMasterPageGalleryOptionsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPages.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyPageLayouts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyOtherResources.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void On_OK_Clicked(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void w_cbCopyMasterPages_Properties_CheckedChanged(object sender, EventArgs e)
		{
			w_cbCorrectLinks.Enabled = w_cbCopyMasterPages.Checked;
		}
	}
}
