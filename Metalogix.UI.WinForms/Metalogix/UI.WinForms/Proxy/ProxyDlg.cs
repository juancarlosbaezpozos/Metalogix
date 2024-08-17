using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Connectivity.Proxy;
using Metalogix.Permissions;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.UI.WinForms.Proxy
{
    public class ProxyDlg : CollapsableForm
    {
        private IContainer components;

        protected Button roundedButtonClose;

        protected Button roundedButtonSet;

        private Panel gradientPanel1;

        private TCProxyConfig w_pcpProxyConfig;

        private GroupBox groupBox;

        public MLProxy Proxy
        {
            get
		{
			return w_pcpProxyConfig.Proxy;
		}
            set
		{
			w_pcpProxyConfig.Proxy = value;
		}
        }

        public bool ShowEnabled
        {
            get
		{
			return w_pcpProxyConfig.ShowEnabled;
		}
            set
		{
			w_pcpProxyConfig.ShowEnabled = value;
		}
        }

        public ProxyDlg()
	{
		InitializeComponent();
	}

        public ProxyDlg(bool bProxyEnabled, string sServer, string sPort, Credentials credentials)
	{
		InitializeComponent();
		MLProxy mLProxy = new MLProxy
		{
			Enabled = bProxyEnabled,
			Server = sServer,
			Port = sPort,
			Credentials = credentials
		};
		Proxy = mLProxy;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Proxy.ProxyDlg));
		Metalogix.Connectivity.Proxy.MLProxy mLProxy = new Metalogix.Connectivity.Proxy.MLProxy();
		Metalogix.Permissions.Credentials credential = new Metalogix.Permissions.Credentials();
		this.roundedButtonClose = new System.Windows.Forms.Button();
		this.roundedButtonSet = new System.Windows.Forms.Button();
		this.gradientPanel1 = new System.Windows.Forms.Panel();
		this.w_pcpProxyConfig = new Metalogix.UI.WinForms.Proxy.TCProxyConfig();
		this.groupBox = new System.Windows.Forms.GroupBox();
		this.gradientPanel1.SuspendLayout();
		this.groupBox.SuspendLayout();
		base.SuspendLayout();
		this.roundedButtonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.roundedButtonClose.BackColor = System.Drawing.Color.Transparent;
		this.roundedButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.roundedButtonClose.Location = new System.Drawing.Point(375, 12);
		this.roundedButtonClose.Name = "roundedButtonClose";
		this.roundedButtonClose.Size = new System.Drawing.Size(75, 23);
		this.roundedButtonClose.TabIndex = 1;
		this.roundedButtonClose.Text = "Close";
		this.roundedButtonClose.UseVisualStyleBackColor = false;
		this.roundedButtonSet.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.roundedButtonSet.BackColor = System.Drawing.Color.Transparent;
		this.roundedButtonSet.Location = new System.Drawing.Point(294, 12);
		this.roundedButtonSet.Name = "roundedButtonSet";
		this.roundedButtonSet.Size = new System.Drawing.Size(75, 23);
		this.roundedButtonSet.TabIndex = 0;
		this.roundedButtonSet.Text = "Set";
		this.roundedButtonSet.UseVisualStyleBackColor = false;
		this.roundedButtonSet.Click += new System.EventHandler(roundedButtonSet_Click);
		this.gradientPanel1.Controls.Add(this.roundedButtonClose);
		this.gradientPanel1.Controls.Add(this.roundedButtonSet);
		this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.gradientPanel1.Location = new System.Drawing.Point(0, 221);
		this.gradientPanel1.Name = "gradientPanel1";
		this.gradientPanel1.Size = new System.Drawing.Size(462, 44);
		this.gradientPanel1.TabIndex = 1;
		this.gradientPanel1.Text = "gradientPanel1";
		this.w_pcpProxyConfig.Context = null;
		this.w_pcpProxyConfig.ControlName = "Proxy Options";
		this.w_pcpProxyConfig.Dock = System.Windows.Forms.DockStyle.Fill;
		this.w_pcpProxyConfig.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_pcpProxyConfig.Image");
		this.w_pcpProxyConfig.ImageName = "Metalogix.UI.WinForms.Icons.Proxy32.ico";
		this.w_pcpProxyConfig.Location = new System.Drawing.Point(3, 16);
		this.w_pcpProxyConfig.MaximumSize = new System.Drawing.Size(1000, 174);
		this.w_pcpProxyConfig.MinimumSize = new System.Drawing.Size(267, 174);
		this.w_pcpProxyConfig.Name = "w_pcpProxyConfig";
		this.w_pcpProxyConfig.Options = null;
		mLProxy.Credentials = credential;
		mLProxy.Enabled = false;
		mLProxy.Port = "";
		mLProxy.Server = "";
		this.w_pcpProxyConfig.Proxy = mLProxy;
		this.w_pcpProxyConfig.ShowEnabled = true;
		this.w_pcpProxyConfig.Size = new System.Drawing.Size(434, 174);
		this.w_pcpProxyConfig.TabIndex = 0;
		this.w_pcpProxyConfig.UseTab = false;
		this.groupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox.Controls.Add(this.w_pcpProxyConfig);
		this.groupBox.Location = new System.Drawing.Point(12, 12);
		this.groupBox.Name = "groupBox";
		this.groupBox.Size = new System.Drawing.Size(440, 198);
		this.groupBox.TabIndex = 0;
		this.groupBox.TabStop = false;
		base.AcceptButton = this.roundedButtonSet;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.CancelButton = this.roundedButtonClose;
		base.ClientSize = new System.Drawing.Size(462, 265);
		base.Controls.Add(this.groupBox);
		base.Controls.Add(this.gradientPanel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ProxyDlg";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Proxy server";
		this.gradientPanel1.ResumeLayout(false);
		this.groupBox.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        private void roundedButtonSet_Click(object sender, EventArgs e)
	{
		if (ValidateInput(out var str))
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
		else if (!string.IsNullOrEmpty(str))
		{
			FlatXtraMessageBox.Show(this, str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

        protected virtual bool ValidateInput(out string sWarningMessage)
	{
		return w_pcpProxyConfig.ValidateInput(out sWarningMessage);
	}
    }
}
