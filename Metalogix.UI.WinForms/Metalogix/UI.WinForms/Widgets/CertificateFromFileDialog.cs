using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.DataStructures;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.Widgets
{
    public class CertificateFromFileDialog : XtraForm
    {
        private X509CertificateWrapper m_certificate;

        private IContainer components;

        private SimpleButton w_bCancel;

        private SimpleButton w_bOK;

        private LabelControl w_lbFilePath;

        private TextEdit w_tbFilePath;

        private SimpleButton w_bBrowse;

        private TextEdit w_mtbPassword;

        private LabelControl w_lbPassword;

        public X509CertificateWrapper Certificate => m_certificate;

        public CertificateFromFileDialog()
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Widgets.CertificateFromFileDialog));
		this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
		this.w_bOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_lbFilePath = new DevExpress.XtraEditors.LabelControl();
		this.w_tbFilePath = new DevExpress.XtraEditors.TextEdit();
		this.w_bBrowse = new DevExpress.XtraEditors.SimpleButton();
		this.w_mtbPassword = new DevExpress.XtraEditors.TextEdit();
		this.w_lbPassword = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.w_tbFilePath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_mtbPassword.Properties).BeginInit();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
		this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_bCancel.Name = "w_bCancel";
		componentResourceManager.ApplyResources(this.w_bOK, "w_bOK");
		this.w_bOK.Name = "w_bOK";
		this.w_bOK.Click += new System.EventHandler(On_Okay_Clicked);
		componentResourceManager.ApplyResources(this.w_lbFilePath, "w_lbFilePath");
		this.w_lbFilePath.Name = "w_lbFilePath";
		componentResourceManager.ApplyResources(this.w_tbFilePath, "w_tbFilePath");
		this.w_tbFilePath.Name = "w_tbFilePath";
		componentResourceManager.ApplyResources(this.w_bBrowse, "w_bBrowse");
		this.w_bBrowse.Name = "w_bBrowse";
		this.w_bBrowse.Click += new System.EventHandler(On_Browse_Clicked);
		componentResourceManager.ApplyResources(this.w_mtbPassword, "w_mtbPassword");
		this.w_mtbPassword.Name = "w_mtbPassword";
		this.w_mtbPassword.Properties.PasswordChar = '*';
		componentResourceManager.ApplyResources(this.w_lbPassword, "w_lbPassword");
		this.w_lbPassword.Name = "w_lbPassword";
		base.AcceptButton = this.w_bOK;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_bCancel;
		base.Controls.Add(this.w_lbPassword);
		base.Controls.Add(this.w_mtbPassword);
		base.Controls.Add(this.w_bBrowse);
		base.Controls.Add(this.w_tbFilePath);
		base.Controls.Add(this.w_lbFilePath);
		base.Controls.Add(this.w_bOK);
		base.Controls.Add(this.w_bCancel);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "CertificateFromFileDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		((System.ComponentModel.ISupportInitialize)this.w_tbFilePath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_mtbPassword.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void On_Browse_Clicked(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			CheckFileExists = true,
			CheckPathExists = true,
			Multiselect = false,
			ShowHelp = false,
			ShowReadOnly = false
		};
		if (openFileDialog.ShowDialog() != DialogResult.Cancel)
		{
			w_tbFilePath.Text = openFileDialog.FileName;
		}
	}

        private void On_Okay_Clicked(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(w_tbFilePath.Text))
			{
				FlatXtraMessageBox.Show("No certificate file specified.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			if (!File.Exists(w_tbFilePath.Text))
			{
				FlatXtraMessageBox.Show("The specified file does not exist.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			X509Certificate x509Certificate = ((!string.IsNullOrEmpty(w_mtbPassword.Text)) ? new X509Certificate(w_tbFilePath.Text, w_mtbPassword.Text) : new X509Certificate(w_tbFilePath.Text));
			m_certificate = new X509CertificateWrapper(x509Certificate, w_tbFilePath.Text, w_mtbPassword.Text.ToSecureString());
			base.DialogResult = DialogResult.OK;
			Close();
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show(exception.Message, "Failed to load certificate", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
    }
}
