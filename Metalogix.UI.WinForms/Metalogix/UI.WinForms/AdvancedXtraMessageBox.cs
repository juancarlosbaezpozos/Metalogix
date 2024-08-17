using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms
{
    public class AdvancedXtraMessageBox : XtraForm
    {
        private bool _hideInFutureChecked;

        private IContainer components;

        private CheckBox cbHideInFuture;

        private PictureBox pbIcon;

        private Label lblMessage;

        private SimpleButton btnOk;

        public bool HideInFutureChecked
        {
            get
		{
			return _hideInFutureChecked;
		}
            set
		{
			_hideInFutureChecked = value;
		}
        }

        public AdvancedXtraMessageBox(string message, string caption, MessageBoxIcon icon, bool checkDoNotShowCheckbox = true)
	{
		InitializeComponent();
		lblMessage.Text = message;
		Text = caption;
		cbHideInFuture.Checked = checkDoNotShowCheckbox;
		SetIcon(icon);
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		HideInFutureChecked = cbHideInFuture.Checked;
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

        private void InitializeComponent()
	{
		this.cbHideInFuture = new System.Windows.Forms.CheckBox();
		this.pbIcon = new System.Windows.Forms.PictureBox();
		this.lblMessage = new System.Windows.Forms.Label();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.pbIcon).BeginInit();
		base.SuspendLayout();
		this.cbHideInFuture.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cbHideInFuture.AutoSize = true;
		this.cbHideInFuture.Checked = true;
		this.cbHideInFuture.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbHideInFuture.Location = new System.Drawing.Point(9, 120);
		this.cbHideInFuture.Name = "cbHideInFuture";
		this.cbHideInFuture.Size = new System.Drawing.Size(180, 17);
		this.cbHideInFuture.TabIndex = 8;
		this.cbHideInFuture.Text = "Do not show this message again";
		this.cbHideInFuture.UseVisualStyleBackColor = true;
		this.pbIcon.Location = new System.Drawing.Point(9, 23);
		this.pbIcon.Margin = new System.Windows.Forms.Padding(0);
		this.pbIcon.Name = "pbIcon";
		this.pbIcon.Size = new System.Drawing.Size(32, 32);
		this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pbIcon.TabIndex = 10;
		this.pbIcon.TabStop = false;
		this.lblMessage.AutoSize = true;
		this.lblMessage.Location = new System.Drawing.Point(46, 23);
		this.lblMessage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblMessage.MaximumSize = new System.Drawing.Size(375, 300);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(71, 13);
		this.lblMessage.TabIndex = 11;
		this.lblMessage.Text = "Content here";
		this.btnOk.Location = new System.Drawing.Point(370, 121);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 12;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(457, 155);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.pbIcon);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.cbHideInFuture);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AdvancedXtraMessageBox";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "AdvancedXtraMessageBox";
		((System.ComponentModel.ISupportInitialize)this.pbIcon).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void SetIcon(MessageBoxIcon icon)
	{
		switch (icon)
		{
		case MessageBoxIcon.None:
			pbIcon.Visible = false;
			break;
		case MessageBoxIcon.Hand:
			pbIcon.Image = SystemIcons.Error.ToBitmap();
			break;
		case MessageBoxIcon.Question:
			pbIcon.Image = SystemIcons.Question.ToBitmap();
			break;
		case MessageBoxIcon.Exclamation:
			pbIcon.Image = Resources.JobStatus_Warning_32;
			break;
		case MessageBoxIcon.Asterisk:
			pbIcon.Image = SystemIcons.Information.ToBitmap();
			break;
		}
	}

        public void SetOkBtnVerticalLocation(int newLocation)
	{
		SimpleButton point = btnOk;
		point.Location = new Point(btnOk.Location.X, newLocation);
	}
    }
}
