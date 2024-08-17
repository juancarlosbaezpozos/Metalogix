using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Permissions;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.UI.WinForms.Components;
using Metalogix.Utilities;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ConnectionOptions.png")]
	[ControlName("Connection Options")]
	[UsesGroupBox(true)]
	public class TCStoragePointConnectionConfig : TabbableControl
	{
		private const string UrlRelative = "/_vti_bin/StoragePoint/RestoreBLOB.asmx";

		private IContainer components;

		private LabelControl w_lblAddress;

		private GroupControl w_groupBox;

		private CheckEdit w_cbRememberMe;

		private TextEdit w_txtPassword;

		private LabelControl w_labelPwd;

		private TextEdit w_txtDifferentUser;

		private CheckEdit w_radioButtonNewUser;

		private CheckEdit w_radioButtonCurrentUser;

		private SimpleButton buttonRemove;

		private SimpleButton buttonDefault;

		private TextEdit w_txtUrl;

		public StoragePointExternalConnection Connection
		{
			set
			{
				Url = value.Url;
				Credentials = value.Credentials;
			}
		}

		public Credentials Credentials
		{
			get
			{
				if (w_radioButtonCurrentUser.Checked)
				{
					return new Credentials();
				}
				return new Credentials(w_txtDifferentUser.Text, w_txtPassword.Text.ToSecureString(), w_cbRememberMe.Checked);
			}
			set
			{
				if (value == null || value.IsDefault)
				{
					w_radioButtonCurrentUser.Checked = true;
					return;
				}
				w_radioButtonNewUser.Checked = true;
				w_txtDifferentUser.Text = value.UserName;
				w_txtPassword.Text = value.Password.ToInsecureString();
				w_cbRememberMe.Checked = value.SavePassword;
			}
		}

		public string Url
		{
			get
			{
				string text = w_txtUrl.Text.Trim().Trim('/');
				if (string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = Regex.Replace(text, "/_vti_bin/StoragePoint/RestoreBLOB.asmx$", "", RegexOptions.IgnoreCase);
				return text + "/_vti_bin/StoragePoint/RestoreBLOB.asmx";
			}
			set
			{
				w_txtUrl.Text = Regex.Replace(value, "/_vti_bin/StoragePoint/RestoreBLOB.asmx$", "", RegexOptions.IgnoreCase);
			}
		}

		public TCStoragePointConnectionConfig()
		{
			InitializeComponent();
			w_radioButtonCurrentUser.Text = WindowsIdentity.GetCurrent().Name;
		}

		private void buttonDefault_Click(object sender, EventArgs e)
		{
			SPConnection sPConnection = (SPConnection)Context.Targets[0];
			w_txtUrl.Text = sPConnection.Adapter.ServerUrl;
			w_txtUrl.Select();
			w_txtUrl.DeselectAll();
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			w_txtUrl.Text = string.Empty;
			w_txtUrl.Select();
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
			this.w_lblAddress = new DevExpress.XtraEditors.LabelControl();
			this.w_groupBox = new DevExpress.XtraEditors.GroupControl();
			this.w_cbRememberMe = new DevExpress.XtraEditors.CheckEdit();
			this.w_txtPassword = new DevExpress.XtraEditors.TextEdit();
			this.w_labelPwd = new DevExpress.XtraEditors.LabelControl();
			this.w_txtDifferentUser = new DevExpress.XtraEditors.TextEdit();
			this.w_radioButtonNewUser = new DevExpress.XtraEditors.CheckEdit();
			this.w_radioButtonCurrentUser = new DevExpress.XtraEditors.CheckEdit();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonDefault = new DevExpress.XtraEditors.SimpleButton();
			this.w_txtUrl = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)this.w_groupBox).BeginInit();
			this.w_groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtPassword.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtDifferentUser.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_radioButtonNewUser.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_radioButtonCurrentUser.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtUrl.Properties).BeginInit();
			base.SuspendLayout();
			this.w_lblAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_lblAddress.Location = new System.Drawing.Point(13, 21);
			this.w_lblAddress.Name = "w_lblAddress";
			this.w_lblAddress.Size = new System.Drawing.Size(39, 13);
			this.w_lblAddress.TabIndex = 0;
			this.w_lblAddress.Text = "Address";
			this.w_groupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_groupBox.Controls.Add(this.w_cbRememberMe);
			this.w_groupBox.Controls.Add(this.w_txtPassword);
			this.w_groupBox.Controls.Add(this.w_labelPwd);
			this.w_groupBox.Controls.Add(this.w_txtDifferentUser);
			this.w_groupBox.Controls.Add(this.w_radioButtonNewUser);
			this.w_groupBox.Controls.Add(this.w_radioButtonCurrentUser);
			this.w_groupBox.Location = new System.Drawing.Point(13, 86);
			this.w_groupBox.Name = "w_groupBox";
			this.w_groupBox.Size = new System.Drawing.Size(420, 128);
			this.w_groupBox.TabIndex = 4;
			this.w_groupBox.Text = "Connect As";
			this.w_cbRememberMe.Enabled = false;
			this.w_cbRememberMe.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbRememberMe.Location = new System.Drawing.Point(25, 102);
			this.w_cbRememberMe.Name = "w_cbRememberMe";
			this.w_cbRememberMe.Properties.AutoWidth = true;
			this.w_cbRememberMe.Properties.Caption = "Remember my password";
			this.w_cbRememberMe.Size = new System.Drawing.Size(140, 19);
			this.w_cbRememberMe.TabIndex = 5;
			this.w_txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_txtPassword.Enabled = false;
			this.w_txtPassword.Location = new System.Drawing.Point(112, 76);
			this.w_txtPassword.Name = "w_txtPassword";
			this.w_txtPassword.Properties.PasswordChar = '*';
			this.w_txtPassword.Size = new System.Drawing.Size(286, 20);
			this.w_txtPassword.TabIndex = 4;
			this.w_labelPwd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_labelPwd.Location = new System.Drawing.Point(29, 79);
			this.w_labelPwd.Name = "w_labelPwd";
			this.w_labelPwd.Size = new System.Drawing.Size(46, 13);
			this.w_labelPwd.TabIndex = 3;
			this.w_labelPwd.Text = "Password";
			this.w_txtDifferentUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_txtDifferentUser.Enabled = false;
			this.w_txtDifferentUser.Location = new System.Drawing.Point(112, 50);
			this.w_txtDifferentUser.Name = "w_txtDifferentUser";
			this.w_txtDifferentUser.Size = new System.Drawing.Size(286, 20);
			this.w_txtDifferentUser.TabIndex = 2;
			this.w_radioButtonNewUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_radioButtonNewUser.Location = new System.Drawing.Point(8, 50);
			this.w_radioButtonNewUser.Name = "w_radioButtonNewUser";
			this.w_radioButtonNewUser.Properties.AutoWidth = true;
			this.w_radioButtonNewUser.Properties.Caption = "Different user";
			this.w_radioButtonNewUser.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_radioButtonNewUser.Properties.RadioGroupIndex = 1;
			this.w_radioButtonNewUser.Size = new System.Drawing.Size(90, 19);
			this.w_radioButtonNewUser.TabIndex = 1;
			this.w_radioButtonNewUser.TabStop = false;
			this.w_radioButtonNewUser.CheckedChanged += new System.EventHandler(w_radioButtonNewUser_CheckedChanged);
			this.w_radioButtonCurrentUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_radioButtonCurrentUser.EditValue = true;
			this.w_radioButtonCurrentUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_radioButtonCurrentUser.Location = new System.Drawing.Point(8, 26);
			this.w_radioButtonCurrentUser.Name = "w_radioButtonCurrentUser";
			this.w_radioButtonCurrentUser.Properties.AutoWidth = true;
			this.w_radioButtonCurrentUser.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_radioButtonCurrentUser.Properties.RadioGroupIndex = 1;
			this.w_radioButtonCurrentUser.Size = new System.Drawing.Size(52, 19);
			this.w_radioButtonCurrentUser.TabIndex = 0;
			this.buttonRemove.Location = new System.Drawing.Point(176, 43);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.Click += new System.EventHandler(buttonRemove_Click);
			this.buttonDefault.Location = new System.Drawing.Point(68, 43);
			this.buttonDefault.Name = "buttonDefault";
			this.buttonDefault.Size = new System.Drawing.Size(102, 23);
			this.buttonDefault.TabIndex = 2;
			this.buttonDefault.Text = "Fill in default URL";
			this.buttonDefault.Click += new System.EventHandler(buttonDefault_Click);
			this.w_txtUrl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_txtUrl.Location = new System.Drawing.Point(68, 17);
			this.w_txtUrl.Name = "w_txtUrl";
			this.w_txtUrl.Size = new System.Drawing.Size(365, 20);
			this.w_txtUrl.TabIndex = 1;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_lblAddress);
			base.Controls.Add(this.w_groupBox);
			base.Controls.Add(this.buttonRemove);
			base.Controls.Add(this.buttonDefault);
			base.Controls.Add(this.w_txtUrl);
			base.Name = "TCStoragePointConnectionConfig";
			base.Size = new System.Drawing.Size(447, 230);
			((System.ComponentModel.ISupportInitialize)this.w_groupBox).EndInit();
			this.w_groupBox.ResumeLayout(false);
			this.w_groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtPassword.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtDifferentUser.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_radioButtonNewUser.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_radioButtonCurrentUser.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtUrl.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void UpdateUI()
		{
			w_txtDifferentUser.Enabled = w_radioButtonNewUser.Checked;
			w_txtPassword.Enabled = w_radioButtonNewUser.Checked;
			w_cbRememberMe.Enabled = w_radioButtonNewUser.Checked;
		}

		private void w_radioButtonNewUser_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}
	}
}
