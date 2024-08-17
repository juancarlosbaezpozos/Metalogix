using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    public class GetPassword : XtraForm
    {
        private IContainer components;

        private TextEdit tbxPassword;

        private LabelControl lblPassword;

        private SimpleButton btnOk;

        private LabelControl lblDescription;

        private SimpleButton btnCancel;

        public string Password { get; private set; }

        public GetPassword()
	{
		InitializeComponent();
	}

        private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		if (ValidateField())
		{
			Password = tbxPassword.Text;
			base.DialogResult = DialogResult.OK;
			Close();
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

        private void InitializeComponent()
	{
		this.tbxPassword = new DevExpress.XtraEditors.TextEdit();
		this.lblPassword = new DevExpress.XtraEditors.LabelControl();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.lblDescription = new DevExpress.XtraEditors.LabelControl();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).BeginInit();
		base.SuspendLayout();
		this.tbxPassword.Location = new System.Drawing.Point(80, 43);
		this.tbxPassword.Name = "tbxPassword";
		this.tbxPassword.Properties.PasswordChar = '*';
		this.tbxPassword.Size = new System.Drawing.Size(210, 20);
		this.tbxPassword.TabIndex = 0;
		this.lblPassword.Location = new System.Drawing.Point(16, 46);
		this.lblPassword.Name = "lblPassword";
		this.lblPassword.Size = new System.Drawing.Size(50, 13);
		this.lblPassword.TabIndex = 12;
		this.lblPassword.Text = "Password:";
		this.btnOk.Location = new System.Drawing.Point(80, 76);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 1;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		this.lblDescription.Location = new System.Drawing.Point(16, 11);
		this.lblDescription.Name = "lblDescription";
		this.lblDescription.Size = new System.Drawing.Size(189, 13);
		this.lblDescription.TabIndex = 17;
		this.lblDescription.Text = "Type the password for the private key.";
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(161, 76);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(302, 109);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.lblDescription);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.tbxPassword);
		base.Controls.Add(this.lblPassword);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(318, 148);
		base.Name = "GetPassword";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Certificate Password";
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private bool ValidateField()
	{
		if (string.IsNullOrEmpty(tbxPassword.Text))
		{
			FlatXtraMessageBox.Show("Password cannot be blank.", "Certificate Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		if (AgentHelper.IsPasswordContainsQuotes(tbxPassword.Text, "Certificate Password"))
		{
			return true;
		}
		tbxPassword.Focus();
		return false;
	}
    }
}
