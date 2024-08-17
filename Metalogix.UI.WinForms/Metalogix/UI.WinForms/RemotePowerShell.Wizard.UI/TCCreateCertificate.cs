using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Create Certificate")]
    public class TCCreateCertificate : AgentWizardTabbableControl
    {
        private readonly string _dialogName = "Create Certificate";

        private string _password = string.Empty;

        private IContainer components;

        private SimpleButton btnGenerateNew;

        private LabelControl lblOR;

        private LabelControl lblBrowseCertificate;

        private SimpleButton btnBrowse;

        private TextEdit tbxBrowseCertificate;

        private OpenFileDialog w_openFileDialog;

        private RichTextBox tbxMessage;

        public TCCreateCertificate()
	{
		InitializeComponent();
	}

        private void btnBrowse_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Title = Resources.TCCreateCertificate_FileDialogTitle,
			Filter = Resources.TCCreateCertificate_FileDialogFilter,
			AddExtension = true,
			CheckFileExists = true
		};
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		X509Certificate2 x509Certificate2 = null;
		try
		{
			x509Certificate2 = new X509Certificate2(openFileDialog.FileName);
			_password = string.Empty;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			if (!exception.Message.Contains("The specified network password is not correct."))
			{
				GlobalServices.ErrorHandler.HandleException("Error", "Error occurred while retrieving certificate. Please select a different certificate.", exception, ErrorIcon.Error);
			}
			else
			{
				x509Certificate2 = GetCertificateObject(openFileDialog.FileName, exception);
			}
		}
		if (x509Certificate2 != null)
		{
			InstallCertificateIfNotExist(x509Certificate2);
			tbxBrowseCertificate.Text = openFileDialog.FileName;
		}
	}

        private void btnGenerateCertificate_Click(object sender, EventArgs e)
	{
		CertificateDetails certificateDetail = new CertificateDetails();
		if (certificateDetail.ShowDialog() == DialogResult.OK)
		{
			tbxBrowseCertificate.Text = certificateDetail.CertificateExportPath;
			_password = certificateDetail.Password;
		}
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private X509Certificate2 GetCertificateObject(string fileName, Exception ex)
	{
		X509Certificate2 x509Certificate2 = null;
		GetPassword getPassword = new GetPassword();
		if (getPassword.ShowDialog() == DialogResult.OK)
		{
			try
			{
				x509Certificate2 = new X509Certificate2(fileName, getPassword.Password);
				_password = getPassword.Password;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!exception.Message.Contains("The specified network password is not correct."))
				{
					GlobalServices.ErrorHandler.HandleException("Error", "Error occurred while retrieving certificate. Please select a different certificate.", ex, ErrorIcon.Error);
				}
				else
				{
					FlatXtraMessageBox.Show("Invalid password.", "Certificate Password", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					x509Certificate2 = GetCertificateObject(fileName, exception);
				}
			}
		}
		return x509Certificate2;
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCCreateCertificate));
		this.btnGenerateNew = new DevExpress.XtraEditors.SimpleButton();
		this.lblOR = new DevExpress.XtraEditors.LabelControl();
		this.lblBrowseCertificate = new DevExpress.XtraEditors.LabelControl();
		this.btnBrowse = new DevExpress.XtraEditors.SimpleButton();
		this.tbxBrowseCertificate = new DevExpress.XtraEditors.TextEdit();
		this.w_openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		((System.ComponentModel.ISupportInitialize)this.tbxBrowseCertificate.Properties).BeginInit();
		base.SuspendLayout();
		this.btnGenerateNew.Location = new System.Drawing.Point(185, 97);
		this.btnGenerateNew.Name = "btnGenerateNew";
		this.btnGenerateNew.Size = new System.Drawing.Size(160, 30);
		this.btnGenerateNew.TabIndex = 0;
		this.btnGenerateNew.Text = "Generate New Certificate";
		this.btnGenerateNew.Click += new System.EventHandler(btnGenerateCertificate_Click);
		this.lblOR.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblOR.Location = new System.Drawing.Point(250, 155);
		this.lblOR.Name = "lblOR";
		this.lblOR.Size = new System.Drawing.Size(29, 13);
		this.lblOR.TabIndex = 1;
		this.lblOR.Text = "- OR -";
		this.lblBrowseCertificate.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblBrowseCertificate.Location = new System.Drawing.Point(10, 190);
		this.lblBrowseCertificate.Name = "lblBrowseCertificate";
		this.lblBrowseCertificate.Size = new System.Drawing.Size(158, 13);
		this.lblBrowseCertificate.TabIndex = 11;
		this.lblBrowseCertificate.Text = "Browse to an existing certificate:";
		this.btnBrowse.Location = new System.Drawing.Point(442, 218);
		this.btnBrowse.Name = "btnBrowse";
		this.btnBrowse.Size = new System.Drawing.Size(65, 23);
		this.btnBrowse.TabIndex = 2;
		this.btnBrowse.Text = "Browse...";
		this.btnBrowse.Click += new System.EventHandler(btnBrowse_Click);
		this.tbxBrowseCertificate.Location = new System.Drawing.Point(10, 220);
		this.tbxBrowseCertificate.Name = "tbxBrowseCertificate";
		this.tbxBrowseCertificate.Properties.ReadOnly = true;
		this.tbxBrowseCertificate.Size = new System.Drawing.Size(426, 20);
		this.tbxBrowseCertificate.TabIndex = 1;
		this.w_openFileDialog.FileName = "openFileDialog1";
		this.tbxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(475, 81);
		this.tbxMessage.TabIndex = 105;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tbxMessage);
		base.Controls.Add(this.lblBrowseCertificate);
		base.Controls.Add(this.btnBrowse);
		base.Controls.Add(this.tbxBrowseCertificate);
		base.Controls.Add(this.lblOR);
		base.Controls.Add(this.btnGenerateNew);
		base.Name = "TCCreateCertificate";
		base.Size = new System.Drawing.Size(525, 266);
		((System.ComponentModel.ISupportInitialize)this.tbxBrowseCertificate.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void InstallCertificateIfNotExist(X509Certificate2 certificate)
	{
		X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
		try
		{
			x509Store.Open(OpenFlags.ReadWrite);
			if (x509Store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, validOnly: false).Count == 0)
			{
				x509Store.Add(certificate);
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException("Error", "Error occurred while installing certificate. Please select a different certificate.", exception, ErrorIcon.Error);
		}
		finally
		{
			x509Store.Close();
		}
	}

        public override bool SaveUI()
	{
		AgentWizardTabbableControl.AgentDetails.CertificateLocation = tbxBrowseCertificate.Text;
		AgentWizardTabbableControl.AgentDetails.CertificatePassword = _password;
		return true;
	}

        public override bool ValidatePage()
	{
		if (!string.IsNullOrEmpty(tbxBrowseCertificate.Text))
		{
			return true;
		}
		FlatXtraMessageBox.Show("Please provide certificate before moving ahead.", _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		return false;
	}
    }
}
