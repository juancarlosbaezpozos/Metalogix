using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Connectivity.Proxy;
using Metalogix.Explorer.Proxy;

namespace Metalogix.UI.WinForms.Proxy
{
    public class ManageProxyControl : XtraUserControl
    {
        private IContainer components;

        private CheckEdit w_radioButtonDefaultProxy;

        private SimpleButton w_buttonEditProxy;

        private CheckEdit w_radioButtonApplicationProxy;

        private CheckEdit w_radioButtonNoProxy;

        public Type ConnectionType { get; set; }

        public ProxyServerOptions ProxyOption
        {
            get
		{
			if (w_radioButtonNoProxy.Checked)
			{
				return ProxyServerOptions.None;
			}
			if (!w_radioButtonDefaultProxy.Checked)
			{
				return ProxyServerOptions.Application;
			}
			return ProxyServerOptions.Default;
		}
            set
		{
			w_radioButtonNoProxy.Checked = value == ProxyServerOptions.None;
			w_radioButtonDefaultProxy.Checked = value == ProxyServerOptions.Default;
			w_radioButtonApplicationProxy.Checked = value == ProxyServerOptions.Application;
		}
        }

        public ManageProxyControl()
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
		this.w_radioButtonDefaultProxy = new DevExpress.XtraEditors.CheckEdit();
		this.w_buttonEditProxy = new DevExpress.XtraEditors.SimpleButton();
		this.w_radioButtonApplicationProxy = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioButtonNoProxy = new DevExpress.XtraEditors.CheckEdit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDefaultProxy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonApplicationProxy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonNoProxy.Properties).BeginInit();
		base.SuspendLayout();
		this.w_radioButtonDefaultProxy.Location = new System.Drawing.Point(3, 28);
		this.w_radioButtonDefaultProxy.Name = "w_radioButtonDefaultProxy";
		this.w_radioButtonDefaultProxy.Properties.Caption = "Use default web proxy";
		this.w_radioButtonDefaultProxy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonDefaultProxy.Properties.RadioGroupIndex = 1;
		this.w_radioButtonDefaultProxy.Size = new System.Drawing.Size(210, 19);
		this.w_radioButtonDefaultProxy.TabIndex = 14;
		this.w_radioButtonDefaultProxy.TabStop = false;
		this.w_buttonEditProxy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_buttonEditProxy.Enabled = false;
		this.w_buttonEditProxy.Location = new System.Drawing.Point(225, 50);
		this.w_buttonEditProxy.Name = "w_buttonEditProxy";
		this.w_buttonEditProxy.Size = new System.Drawing.Size(160, 24);
		this.w_buttonEditProxy.TabIndex = 16;
		this.w_buttonEditProxy.Text = "Edit Proxy Settings";
		this.w_buttonEditProxy.Click += new System.EventHandler(w_buttonEditProxy_Click);
		this.w_radioButtonApplicationProxy.Location = new System.Drawing.Point(3, 53);
		this.w_radioButtonApplicationProxy.Name = "w_radioButtonApplicationProxy";
		this.w_radioButtonApplicationProxy.Properties.Caption = "Use application proxy server";
		this.w_radioButtonApplicationProxy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonApplicationProxy.Properties.RadioGroupIndex = 1;
		this.w_radioButtonApplicationProxy.Size = new System.Drawing.Size(230, 19);
		this.w_radioButtonApplicationProxy.TabIndex = 15;
		this.w_radioButtonApplicationProxy.TabStop = false;
		this.w_radioButtonApplicationProxy.CheckedChanged += new System.EventHandler(w_radioButtonApplicationProxy_CheckedChanged);
		this.w_radioButtonNoProxy.EditValue = true;
		this.w_radioButtonNoProxy.Location = new System.Drawing.Point(3, 3);
		this.w_radioButtonNoProxy.Name = "w_radioButtonNoProxy";
		this.w_radioButtonNoProxy.Properties.Caption = "Do not use proxy server";
		this.w_radioButtonNoProxy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonNoProxy.Properties.RadioGroupIndex = 1;
		this.w_radioButtonNoProxy.Size = new System.Drawing.Size(210, 19);
		this.w_radioButtonNoProxy.TabIndex = 13;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_radioButtonDefaultProxy);
		base.Controls.Add(this.w_buttonEditProxy);
		base.Controls.Add(this.w_radioButtonApplicationProxy);
		base.Controls.Add(this.w_radioButtonNoProxy);
		base.Name = "ManageProxyControl";
		base.Size = new System.Drawing.Size(400, 87);
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDefaultProxy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonApplicationProxy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonNoProxy.Properties).EndInit();
		base.ResumeLayout(false);
	}

        private void w_buttonEditProxy_Click(object sender, EventArgs e)
	{
		try
		{
			EditProxyDialog editProxyDialog = new EditProxyDialog
			{
				Options = ProxyManager.Instance.GetOrCreateSettings(ConnectionType)
			};
			if (editProxyDialog.ShowDialog() == DialogResult.OK)
			{
				ProxyManager.Instance.SetSettings(ConnectionType, editProxyDialog.Options);
				ProxyManager.Instance.Save();
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void w_radioButtonApplicationProxy_CheckedChanged(object sender, EventArgs e)
	{
		w_buttonEditProxy.Enabled = w_radioButtonApplicationProxy.Checked;
	}
    }
}
