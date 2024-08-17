using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CERTENROLLLib;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    public class CertificateDetails : XtraForm
    {
        private readonly string _dialogName = "Certificate Details";

        private IContainer components;

        private FolderBrowserDialog folderBrowserDialog;

        private SimpleButton btnBrowse;

        private TextEdit tbxExportLocation;

        private LabelControl lblExportPath;

        private TextEdit tbxPassword;

        private LabelControl lblPassword;

        private TextEdit tbxCertificateName;

        private LabelControl lblCertificateName;

        private SimpleButton btnOk;

        private SimpleButton btnCancel;

        public string CertificateExportPath { get; private set; }

        public string Password { get; private set; }

        public CertificateDetails()
	{
		InitializeComponent();
	}

        private void btnBrowse_Click(object sender, EventArgs e)
	{
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			tbxExportLocation.Text = folderBrowserDialog.SelectedPath;
		}
	}

        private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		if (!ValidateFields())
		{
			return;
		}
		X509Certificate2 x509Certificate2 = CreateSelfSignedCertificate();
		if (x509Certificate2 != null)
		{
			try
			{
				string str = string.Format("{0}{1}", Path.Combine(tbxExportLocation.Text, tbxCertificateName.Text), ".pfx");
				File.WriteAllBytes(str, x509Certificate2.Export(X509ContentType.Pfx, tbxPassword.Text));
				CertificateExportPath = str;
				Password = tbxPassword.Text;
				base.DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException("Error", "Error occurred while attempting to export certificate.", exception, ErrorIcon.Error);
			}
		}
	}

        private X509Certificate2 CreateSelfSignedCertificate()
	{
		try
		{
			if (IfCertificateNotExists())
			{
				CX500DistinguishedName variable = (CX500DistinguishedName)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2003-217D-11DA-B2A4-000E7BBB2B09")));
				variable.Encode("CN=" + tbxCertificateName.Text);
				CX509PrivateKey variable1 = (CX509PrivateKey)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E200C-217D-11DA-B2A4-000E7BBB2B09")));
				variable1.ProviderName = "Microsoft RSA SChannel Cryptographic Provider";
				variable1.MachineContext = false;
				variable1.Length = 2048;
				variable1.KeySpec = X509KeySpec.XCN_AT_KEYEXCHANGE;
				variable1.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
				CX509PrivateKey variable2 = variable1;
				variable2.Create();
				CObjectId variable3 = (CObjectId)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2000-217D-11DA-B2A4-000E7BBB2B09")));
				variable3.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID, ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY, AlgorithmFlags.AlgorithmFlagsNone, "SHA1");
				CX509CertificateRequestCertificate now = (CX509CertificateRequestCertificate)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2043-217D-11DA-B2A4-000E7BBB2B09")));
				now.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextUser, variable2, string.Empty);
				now.Subject = variable;
				now.Issuer = variable;
				now.NotBefore = DateTime.Now;
				now.NotAfter = now.NotBefore.AddYears(23);
				now.HashAlgorithm = variable3;
				now.Encode();
				CX509Enrollment variable4 = (CX509Enrollment)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2046-217D-11DA-B2A4-000E7BBB2B09")));
				variable4.InitializeFromRequest(now);
				variable4.CertificateFriendlyName = "Metalogix Agent Encryption";
				string str = variable4.CreateRequest();
				variable4.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate, str, EncodingType.XCN_CRYPT_STRING_BASE64, string.Empty);
				string str1 = variable4.CreatePFX(string.Empty, PFXExportOptions.PFXExportChainWithRoot);
				return new X509Certificate2(Convert.FromBase64String(str1), string.Empty, X509KeyStorageFlags.Exportable);
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException("Error", "Error occurred while attempting to create certificate.", exception, ErrorIcon.Error);
		}
		return null;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private bool IfCertificateNotExists()
	{
		X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
		x509Store.Open(OpenFlags.ReadOnly);
		X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, tbxCertificateName.Text, validOnly: false);
		if (x509Certificate2Collection.Count > 0)
		{
			X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Certificate2 current = enumerator.Current;
				if (current.SubjectName.Name == null || !current.SubjectName.Name.Equals("CN=" + tbxCertificateName.Text, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				FlatXtraMessageBox.Show("Certificate with the same name already exists. Please provide a different name.", _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				tbxCertificateName.Focus();
				return false;
			}
		}
		return true;
	}

        private void InitializeComponent()
	{
		this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
		this.btnBrowse = new DevExpress.XtraEditors.SimpleButton();
		this.tbxExportLocation = new DevExpress.XtraEditors.TextEdit();
		this.lblExportPath = new DevExpress.XtraEditors.LabelControl();
		this.tbxPassword = new DevExpress.XtraEditors.TextEdit();
		this.lblPassword = new DevExpress.XtraEditors.LabelControl();
		this.tbxCertificateName = new DevExpress.XtraEditors.TextEdit();
		this.lblCertificateName = new DevExpress.XtraEditors.LabelControl();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.tbxExportLocation.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tbxCertificateName.Properties).BeginInit();
		base.SuspendLayout();
		this.btnBrowse.Location = new System.Drawing.Point(393, 80);
		this.btnBrowse.Name = "btnBrowse";
		this.btnBrowse.Size = new System.Drawing.Size(65, 23);
		this.btnBrowse.TabIndex = 3;
		this.btnBrowse.Text = "Browse...";
		this.btnBrowse.Click += new System.EventHandler(btnBrowse_Click);
		this.tbxExportLocation.Location = new System.Drawing.Point(117, 82);
		this.tbxExportLocation.Name = "tbxExportLocation";
		this.tbxExportLocation.Properties.ReadOnly = true;
		this.tbxExportLocation.Size = new System.Drawing.Size(270, 20);
		this.tbxExportLocation.TabIndex = 2;
		this.lblExportPath.Location = new System.Drawing.Point(15, 85);
		this.lblExportPath.Name = "lblExportPath";
		this.lblExportPath.Size = new System.Drawing.Size(79, 13);
		this.lblExportPath.TabIndex = 12;
		this.lblExportPath.Text = "Export Location:";
		this.tbxPassword.Location = new System.Drawing.Point(117, 51);
		this.tbxPassword.Name = "tbxPassword";
		this.tbxPassword.Properties.PasswordChar = '*';
		this.tbxPassword.Size = new System.Drawing.Size(341, 20);
		this.tbxPassword.TabIndex = 1;
		this.lblPassword.Location = new System.Drawing.Point(15, 54);
		this.lblPassword.Name = "lblPassword";
		this.lblPassword.Size = new System.Drawing.Size(50, 13);
		this.lblPassword.TabIndex = 10;
		this.lblPassword.Text = "Password:";
		this.tbxCertificateName.Location = new System.Drawing.Point(117, 20);
		this.tbxCertificateName.Name = "tbxCertificateName";
		this.tbxCertificateName.Properties.MaxLength = 100;
		this.tbxCertificateName.Size = new System.Drawing.Size(341, 20);
		this.tbxCertificateName.TabIndex = 0;
		this.lblCertificateName.Location = new System.Drawing.Point(15, 23);
		this.lblCertificateName.Name = "lblCertificateName";
		this.lblCertificateName.Size = new System.Drawing.Size(84, 13);
		this.lblCertificateName.TabIndex = 7;
		this.lblCertificateName.Text = "Certificate Name:";
		this.btnOk.Location = new System.Drawing.Point(117, 113);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(198, 113);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(470, 148);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.tbxExportLocation);
		base.Controls.Add(this.btnBrowse);
		base.Controls.Add(this.lblExportPath);
		base.Controls.Add(this.tbxPassword);
		base.Controls.Add(this.lblPassword);
		base.Controls.Add(this.tbxCertificateName);
		base.Controls.Add(this.lblCertificateName);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(486, 187);
		base.MinimizeBox = false;
		base.Name = "CertificateDetails";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Certificate Details";
		((System.ComponentModel.ISupportInitialize)this.tbxExportLocation.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tbxCertificateName.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private bool ValidateFields()
	{
		tbxCertificateName.Text = tbxCertificateName.Text.Trim();
		string str = "{0} cannot be blank.";
		if (string.IsNullOrEmpty(tbxCertificateName.Text))
		{
			str = string.Format(str, "Certificate Name");
			FlatXtraMessageBox.Show(str, _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			tbxCertificateName.Focus();
			return false;
		}
		if (!Regex.IsMatch(tbxCertificateName.Text, "^\\b[a-zA-Z0-9 _.-]+\\b$"))
		{
			FlatXtraMessageBox.Show("Special characters are not allowed except '.', '-' and '_'.", _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			tbxCertificateName.Focus();
			return false;
		}
		if (string.IsNullOrEmpty(tbxPassword.Text))
		{
			str = string.Format(str, "Password");
			FlatXtraMessageBox.Show(str, _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			tbxPassword.Focus();
			return false;
		}
		if (!AgentHelper.IsPasswordContainsQuotes(tbxPassword.Text, _dialogName))
		{
			tbxPassword.Focus();
			return false;
		}
		if (!string.IsNullOrEmpty(tbxExportLocation.Text))
		{
			return true;
		}
		str = string.Format(str, "Export Location");
		FlatXtraMessageBox.Show(str, _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		tbxExportLocation.Focus();
		return false;
	}
    }
}
