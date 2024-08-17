using Metalogix;
using Metalogix.Licensing.SK;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.SK
{
	public class FormProxySetup : Form
	{
		private LicenseProxy _proxy;

		private IContainer components;

		private TextBox textBoxPort;

		private TextBox textBoxPass;

		private Label label4;

		private TextBox textBoxUser;

		private TextBox textBoxServer;

		private Label label2;

		private Label label1;

		private CheckBox checkBoxProxy;

		private Label lblKey;

		private Button buttonCancel;

		private Button buttonOk;

		public LicenseProxy Proxy
		{
			get
			{
				return this._proxy;
			}
		}

		public FormProxySetup(LicenseProxy proxy)
		{
			this.InitializeComponent();
			this.textBoxServer.Text = proxy.Server;
			this.textBoxPort.Text = proxy.Port;
			this.textBoxUser.Text = proxy.User;
			this.textBoxPass.Text = proxy.Pass;
			this.checkBoxProxy.Checked = proxy.Enabled;
			this.checkBoxProxy_CheckedChanged(null, null);
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.checkBoxProxy.Checked)
				{
					if (this.textBoxServer.Text == "" || this.textBoxServer.Text.Length == 0)
					{
						FlatXtraMessageBox.Show(this, "The server name must be specified.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					else if (this.textBoxPort.Text != "" && this.textBoxPort.Text.Length != 0)
					{
						try
						{
							Convert.ToInt32(this.textBoxPort.Text);
						}
						catch
						{
							FlatXtraMessageBox.Show(this, "The port must be a number.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return;
						}
					}
				}
				this._proxy = new LicenseProxy(this.checkBoxProxy.Checked, this.textBoxServer.Text, this.textBoxPort.Text, this.textBoxUser.Text, this.textBoxPass.Text);
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				base.Close();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Concat("FormProxySetup >> buttonOk_Click:", exception.ToString()));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this.Text, "Failed to set proxy setup", exception, ErrorIcon.Error);
			}
		}

		private void checkBoxProxy_CheckedChanged(object sender, EventArgs e)
		{
			TextBox textBox = this.textBoxServer;
			TextBox textBox1 = this.textBoxPort;
			TextBox textBox2 = this.textBoxUser;
			TextBox textBox3 = this.textBoxPass;
			bool @checked = this.checkBoxProxy.Checked;
			bool flag = @checked;
			textBox3.Enabled = @checked;
			bool flag1 = flag;
			bool flag2 = flag1;
			textBox2.Enabled = flag1;
			bool flag3 = flag2;
			bool flag4 = flag3;
			textBox1.Enabled = flag3;
			textBox.Enabled = flag4;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormProxySetup));
			this.textBoxPort = new TextBox();
			this.textBoxPass = new TextBox();
			this.label4 = new Label();
			this.textBoxUser = new TextBox();
			this.textBoxServer = new TextBox();
			this.label2 = new Label();
			this.label1 = new Label();
			this.checkBoxProxy = new CheckBox();
			this.lblKey = new Label();
			this.buttonCancel = new Button();
			this.buttonOk = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.textBoxPort, "textBoxPort");
			this.textBoxPort.Name = "textBoxPort";
			componentResourceManager.ApplyResources(this.textBoxPass, "textBoxPass");
			this.textBoxPass.Name = "textBoxPass";
			componentResourceManager.ApplyResources(this.label4, "label4");
			this.label4.BackColor = Color.Transparent;
			this.label4.Name = "label4";
			componentResourceManager.ApplyResources(this.textBoxUser, "textBoxUser");
			this.textBoxUser.Name = "textBoxUser";
			componentResourceManager.ApplyResources(this.textBoxServer, "textBoxServer");
			this.textBoxServer.Name = "textBoxServer";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.BackColor = Color.Transparent;
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.BackColor = Color.Transparent;
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.checkBoxProxy, "checkBoxProxy");
			this.checkBoxProxy.BackColor = Color.Transparent;
			this.checkBoxProxy.Name = "checkBoxProxy";
			this.checkBoxProxy.UseVisualStyleBackColor = false;
			this.checkBoxProxy.CheckedChanged += new EventHandler(this.checkBoxProxy_CheckedChanged);
			componentResourceManager.ApplyResources(this.lblKey, "lblKey");
			this.lblKey.BackColor = Color.Transparent;
			this.lblKey.Name = "lblKey";
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOk);
			base.Controls.Add(this.textBoxPort);
			base.Controls.Add(this.textBoxPass);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.textBoxUser);
			base.Controls.Add(this.textBoxServer);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.checkBoxProxy);
			base.Controls.Add(this.lblKey);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "FormProxySetup";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}