using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Options;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class UpdateAgent : XtraForm
    {
        private IContainer components;

        private RadioGroup rbtnUpdateOptions;

        private SimpleButton btnOk;

        private LabelControl lblOptionText;

        private SimpleButton btnCancel;

        private PictureBox imgHelpIcon;

        public UpdateAgentOptions UpdateOptions { get; set; }

        public UpdateAgent()
	{
		InitializeComponent();
		imgHelpIcon.Image = ImageCache.GetImage("Metalogix.UI.WinForms.Icons.Help16.png", GetType().Assembly);
	}

        private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		UpdateOptions.IsUpdateContentMatrix = rbtnUpdateOptions.SelectedIndex == 0;
		base.DialogResult = DialogResult.OK;
		Close();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void imgHelpIcon_MouseHover(object sender, EventArgs e)
	{
		new ToolTip().SetToolTip(imgHelpIcon, Metalogix.UI.WinForms.Properties.Tooltips.UpdateAgentHelpTooltip);
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.UpdateAgent));
		this.rbtnUpdateOptions = new DevExpress.XtraEditors.RadioGroup();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.lblOptionText = new DevExpress.XtraEditors.LabelControl();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.imgHelpIcon = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.rbtnUpdateOptions.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imgHelpIcon).BeginInit();
		base.SuspendLayout();
		this.rbtnUpdateOptions.EditValue = (short)0;
		this.rbtnUpdateOptions.Location = new System.Drawing.Point(12, 30);
		this.rbtnUpdateOptions.Name = "rbtnUpdateOptions";
		this.rbtnUpdateOptions.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rbtnUpdateOptions.Properties.Appearance.Options.UseBackColor = true;
		this.rbtnUpdateOptions.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		DevExpress.XtraEditors.Controls.RadioGroupItemCollection items = this.rbtnUpdateOptions.Properties.Items;
		DevExpress.XtraEditors.Controls.RadioGroupItem[] radioGroupItem = new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem((short)0, "Update Content Matrix Consoles"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem((short)1, "Update Application Mappings only")
		};
		items.AddRange(radioGroupItem);
		this.rbtnUpdateOptions.Size = new System.Drawing.Size(283, 66);
		this.rbtnUpdateOptions.TabIndex = 0;
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOk.Location = new System.Drawing.Point(155, 95);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 1;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		this.lblOptionText.Location = new System.Drawing.Point(19, 16);
		this.lblOptionText.Name = "lblOptionText";
		this.lblOptionText.Size = new System.Drawing.Size(86, 13);
		this.lblOptionText.TabIndex = 2;
		this.lblOptionText.Text = "Would you like to:";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(244, 96);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		this.imgHelpIcon.ImageLocation = "";
		this.imgHelpIcon.Location = new System.Drawing.Point(210, 41);
		this.imgHelpIcon.Name = "imgHelpIcon";
		this.imgHelpIcon.Size = new System.Drawing.Size(20, 20);
		this.imgHelpIcon.TabIndex = 4;
		this.imgHelpIcon.TabStop = false;
		this.imgHelpIcon.MouseHover += new System.EventHandler(imgHelpIcon_MouseHover);
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(334, 128);
		base.Controls.Add(this.imgHelpIcon);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.lblOptionText);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.rbtnUpdateOptions);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "UpdateAgent";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Update Agent";
		((System.ComponentModel.ISupportInitialize)this.rbtnUpdateOptions.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imgHelpIcon).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
    }
}
