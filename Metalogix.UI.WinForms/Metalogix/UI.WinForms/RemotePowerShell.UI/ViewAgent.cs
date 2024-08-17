using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions.Remoting;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class ViewAgent : XtraForm
    {
        private IContainer components;

        private LabelControl lblAgentName;

        private LabelControl lblAgentNameValue;

        private LabelControl lblIPAddressValue;

        private LabelControl lblIPAddress;

        private LabelControl lblCMVersionValue;

        private LabelControl lblCMVersion;

        private LabelControl lblConnectedAsValue;

        private LabelControl lblConnectedAs;

        private LabelControl lblAgentOutput;

        private MemoEdit memoAgentOutput;

        private SimpleButton btnClose;

        private LabelControl lblOSVersion;

        private LabelControl lblOSVersionValue;

        public ViewAgent()
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

        private string GetAgentLogDetails(List<KeyValuePair<DateTime, string>> logDetails)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<DateTime, string> logDetail in logDetails)
		{
			stringBuilder.Append(logDetail.Key).Append(": ").AppendLine(logDetail.Value)
				.AppendLine();
		}
		return stringBuilder.ToString();
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.ViewAgent));
		this.lblAgentName = new DevExpress.XtraEditors.LabelControl();
		this.lblAgentNameValue = new DevExpress.XtraEditors.LabelControl();
		this.lblIPAddressValue = new DevExpress.XtraEditors.LabelControl();
		this.lblIPAddress = new DevExpress.XtraEditors.LabelControl();
		this.lblCMVersionValue = new DevExpress.XtraEditors.LabelControl();
		this.lblCMVersion = new DevExpress.XtraEditors.LabelControl();
		this.lblConnectedAsValue = new DevExpress.XtraEditors.LabelControl();
		this.lblConnectedAs = new DevExpress.XtraEditors.LabelControl();
		this.lblAgentOutput = new DevExpress.XtraEditors.LabelControl();
		this.memoAgentOutput = new DevExpress.XtraEditors.MemoEdit();
		this.btnClose = new DevExpress.XtraEditors.SimpleButton();
		this.lblOSVersion = new DevExpress.XtraEditors.LabelControl();
		this.lblOSVersionValue = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.memoAgentOutput.Properties).BeginInit();
		base.SuspendLayout();
		this.lblAgentName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblAgentName.Location = new System.Drawing.Point(12, 27);
		this.lblAgentName.Name = "lblAgentName";
		this.lblAgentName.Size = new System.Drawing.Size(72, 13);
		this.lblAgentName.TabIndex = 0;
		this.lblAgentName.Text = "Agent Name:";
		this.lblAgentNameValue.Location = new System.Drawing.Point(116, 27);
		this.lblAgentNameValue.Name = "lblAgentNameValue";
		this.lblAgentNameValue.Size = new System.Drawing.Size(29, 13);
		this.lblAgentNameValue.TabIndex = 1;
		this.lblAgentNameValue.Text = "Agent";
		this.lblIPAddressValue.Location = new System.Drawing.Point(116, 46);
		this.lblIPAddressValue.Name = "lblIPAddressValue";
		this.lblIPAddressValue.Size = new System.Drawing.Size(29, 13);
		this.lblIPAddressValue.TabIndex = 3;
		this.lblIPAddressValue.Text = "Agent";
		this.lblIPAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblIPAddress.Location = new System.Drawing.Point(12, 46);
		this.lblIPAddress.Name = "lblIPAddress";
		this.lblIPAddress.Size = new System.Drawing.Size(64, 13);
		this.lblIPAddress.TabIndex = 2;
		this.lblIPAddress.Text = "IP Address:";
		this.lblCMVersionValue.Location = new System.Drawing.Point(116, 82);
		this.lblCMVersionValue.Name = "lblCMVersionValue";
		this.lblCMVersionValue.Size = new System.Drawing.Size(29, 13);
		this.lblCMVersionValue.TabIndex = 5;
		this.lblCMVersionValue.Text = "Agent";
		this.lblCMVersion.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblCMVersion.Location = new System.Drawing.Point(12, 82);
		this.lblCMVersion.Name = "lblCMVersion";
		this.lblCMVersion.Size = new System.Drawing.Size(65, 13);
		this.lblCMVersion.TabIndex = 4;
		this.lblCMVersion.Text = "CM Version:";
		this.lblConnectedAsValue.Location = new System.Drawing.Point(116, 101);
		this.lblConnectedAsValue.Name = "lblConnectedAsValue";
		this.lblConnectedAsValue.Size = new System.Drawing.Size(29, 13);
		this.lblConnectedAsValue.TabIndex = 7;
		this.lblConnectedAsValue.Text = "Agent";
		this.lblConnectedAs.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblConnectedAs.Location = new System.Drawing.Point(12, 101);
		this.lblConnectedAs.Name = "lblConnectedAs";
		this.lblConnectedAs.Size = new System.Drawing.Size(80, 13);
		this.lblConnectedAs.TabIndex = 6;
		this.lblConnectedAs.Text = "Connected As:";
		this.lblAgentOutput.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblAgentOutput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.lblAgentOutput.Location = new System.Drawing.Point(12, 131);
		this.lblAgentOutput.Name = "lblAgentOutput";
		this.lblAgentOutput.Size = new System.Drawing.Size(79, 13);
		this.lblAgentOutput.TabIndex = 8;
		this.lblAgentOutput.Text = "Agent Output:";
		this.memoAgentOutput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoAgentOutput.Location = new System.Drawing.Point(12, 150);
		this.memoAgentOutput.Name = "memoAgentOutput";
		this.memoAgentOutput.Properties.ReadOnly = true;
		this.memoAgentOutput.Size = new System.Drawing.Size(513, 302);
		this.memoAgentOutput.TabIndex = 9;
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.btnClose.Location = new System.Drawing.Point(450, 459);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(75, 23);
		this.btnClose.TabIndex = 10;
		this.btnClose.Text = "Close";
		this.lblOSVersion.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblOSVersion.Location = new System.Drawing.Point(11, 63);
		this.lblOSVersion.Name = "lblOSVersion";
		this.lblOSVersion.Size = new System.Drawing.Size(63, 13);
		this.lblOSVersion.TabIndex = 11;
		this.lblOSVersion.Text = "OS Version:";
		this.lblOSVersionValue.Location = new System.Drawing.Point(116, 63);
		this.lblOSVersionValue.Name = "lblOSVersionValue";
		this.lblOSVersionValue.Size = new System.Drawing.Size(29, 13);
		this.lblOSVersionValue.TabIndex = 12;
		this.lblOSVersionValue.Text = "Agent";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnClose;
		base.ClientSize = new System.Drawing.Size(537, 510);
		base.Controls.Add(this.lblOSVersionValue);
		base.Controls.Add(this.lblOSVersion);
		base.Controls.Add(this.btnClose);
		base.Controls.Add(this.lblAgentOutput);
		base.Controls.Add(this.memoAgentOutput);
		base.Controls.Add(this.lblConnectedAsValue);
		base.Controls.Add(this.lblConnectedAs);
		base.Controls.Add(this.lblCMVersionValue);
		base.Controls.Add(this.lblCMVersion);
		base.Controls.Add(this.lblIPAddressValue);
		base.Controls.Add(this.lblIPAddress);
		base.Controls.Add(this.lblAgentNameValue);
		base.Controls.Add(this.lblAgentName);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(553, 549);
		base.Name = "ViewAgent";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Agent Details";
		((System.ComponentModel.ISupportInitialize)this.memoAgentOutput.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        public void LoadUI(Agent context)
	{
		memoAgentOutput.DeselectAll();
		lblAgentNameValue.Text = context.MachineName;
		lblIPAddressValue.Text = context.MachineIP;
		lblOSVersionValue.Text = context.OSVersion;
		lblCMVersionValue.Text = context.CMVersion;
		lblConnectedAsValue.Text = context.UserName;
		memoAgentOutput.Text = GetAgentLogDetails(context.Details);
	}
    }
}
