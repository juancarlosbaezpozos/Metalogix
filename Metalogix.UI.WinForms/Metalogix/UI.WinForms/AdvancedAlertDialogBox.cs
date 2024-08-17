using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms
{
    public class AdvancedAlertDialogBox : XtraForm
    {
        private bool _isWarningMessage;

        private IContainer components;

        private LabelControl lblHeader;

        private LabelControl lblFirstMessage;

        private LabelControl lblFirstInformation;

        private LabelControl lblContactInfo;

        private CheckEdit cbxSuppressDialog;

        private SimpleButton btnOk;

        private LabelControl lblSecondMessage;

        private LabelControl lblSecondInformation;

        private LabelControl lblThirdMessage;

        private LabelControl lblThirdInformation;

        public AdvancedAlertDialogBox(string dialogTitle, string headerMessage, string displayMessage, string informationMessage, string contactSupportMessage, string suppressMessage)
	{
		InitializeComponent();
		base.Height = 200;
		base.Width = 395;
		Point point = new Point
		{
			X = base.Width / 2 - btnOk.Width / 2,
			Y = btnOk.Location.Y
		};
		btnOk.Location = point;
		SetLabelControls(item: new KeyValuePair<string, string>(displayMessage, informationMessage), messageLabel: lblFirstMessage, informationLabel: lblFirstInformation);
		SetDefaultControls(dialogTitle, headerMessage, contactSupportMessage, suppressMessage != null, suppressMessage);
	}

        public AdvancedAlertDialogBox(Dictionary<string, string> alertMessages, string dialogTitle, string headerMessage, string contactSupportMessage, string suppressMessage)
	{
		InitializeComponent();
		if (alertMessages.Count <= 0)
		{
			return;
		}
		SetLabelControls(lblFirstMessage, lblFirstInformation, alertMessages.ElementAt(0));
		base.Height = 210;
		if (alertMessages.Count > 1)
		{
			SetLabelControls(lblSecondMessage, lblSecondInformation, alertMessages.ElementAt(1));
			Height += 65;
			if (alertMessages.Count > 2)
			{
				SetLabelControls(lblThirdMessage, lblThirdInformation, alertMessages.ElementAt(2));
				Height += 65;
			}
		}
		SetDefaultControls(dialogTitle, headerMessage, contactSupportMessage, _isWarningMessage, suppressMessage);
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		if (cbxSuppressDialog.Checked)
		{
			UIConfigurationVariables.DateToShowLicenseAlert = DateTime.UtcNow.AddDays(Convert.ToDouble(UIConfigurationVariables.DaysToSuppressLicenseAlert)).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}
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
		this.lblHeader = new DevExpress.XtraEditors.LabelControl();
		this.lblFirstInformation = new DevExpress.XtraEditors.LabelControl();
		this.lblFirstMessage = new DevExpress.XtraEditors.LabelControl();
		this.lblContactInfo = new DevExpress.XtraEditors.LabelControl();
		this.cbxSuppressDialog = new DevExpress.XtraEditors.CheckEdit();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.lblSecondInformation = new DevExpress.XtraEditors.LabelControl();
		this.lblSecondMessage = new DevExpress.XtraEditors.LabelControl();
		this.lblThirdInformation = new DevExpress.XtraEditors.LabelControl();
		this.lblThirdMessage = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.cbxSuppressDialog.Properties).BeginInit();
		base.SuspendLayout();
		this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblHeader.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblHeader.Appearance.ForeColor = System.Drawing.Color.Red;
		this.lblHeader.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblHeader.Location = new System.Drawing.Point(14, 9);
		this.lblHeader.Name = "lblHeader";
		this.lblHeader.Size = new System.Drawing.Size(41, 13);
		this.lblHeader.TabIndex = 19;
		this.lblHeader.Text = "Header";
		this.lblFirstInformation.AllowHtmlString = true;
		this.lblFirstInformation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblFirstInformation.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblFirstInformation.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblFirstInformation.Location = new System.Drawing.Point(120, 65);
		this.lblFirstInformation.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblFirstInformation.Name = "lblFirstInformation";
		this.lblFirstInformation.Size = new System.Drawing.Size(96, 13);
		this.lblFirstInformation.TabIndex = 14;
		this.lblFirstInformation.Text = "First Information";
		this.lblFirstMessage.AllowHtmlString = true;
		this.lblFirstMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblFirstMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblFirstMessage.Location = new System.Drawing.Point(14, 33);
		this.lblFirstMessage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblFirstMessage.Name = "lblFirstMessage";
		this.lblFirstMessage.Size = new System.Drawing.Size(66, 13);
		this.lblFirstMessage.TabIndex = 11;
		this.lblFirstMessage.Text = "First Message";
		this.lblContactInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblContactInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblContactInfo.Location = new System.Drawing.Point(14, 233);
		this.lblContactInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblContactInfo.Name = "lblContactInfo";
		this.lblContactInfo.Size = new System.Drawing.Size(138, 13);
		this.lblContactInfo.TabIndex = 13;
		this.lblContactInfo.Text = "Support Contact Information";
		this.cbxSuppressDialog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cbxSuppressDialog.Location = new System.Drawing.Point(11, 254);
		this.cbxSuppressDialog.Name = "cbxSuppressDialog";
		this.cbxSuppressDialog.Properties.Appearance.Options.UseTextOptions = true;
		this.cbxSuppressDialog.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.cbxSuppressDialog.Size = new System.Drawing.Size(387, 19);
		this.cbxSuppressDialog.TabIndex = 19;
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.Location = new System.Drawing.Point(172, 278);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 12;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		this.lblSecondInformation.AllowHtmlString = true;
		this.lblSecondInformation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblSecondInformation.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblSecondInformation.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblSecondInformation.Location = new System.Drawing.Point(120, 129);
		this.lblSecondInformation.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblSecondInformation.Name = "lblSecondInformation";
		this.lblSecondInformation.Size = new System.Drawing.Size(112, 13);
		this.lblSecondInformation.TabIndex = 17;
		this.lblSecondInformation.Text = "Second Information";
		this.lblSecondInformation.Visible = false;
		this.lblSecondMessage.AllowHtmlString = true;
		this.lblSecondMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblSecondMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblSecondMessage.Location = new System.Drawing.Point(14, 96);
		this.lblSecondMessage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblSecondMessage.Name = "lblSecondMessage";
		this.lblSecondMessage.Size = new System.Drawing.Size(80, 13);
		this.lblSecondMessage.TabIndex = 18;
		this.lblSecondMessage.Text = "Second Message";
		this.lblSecondMessage.Visible = false;
		this.lblThirdInformation.AllowHtmlString = true;
		this.lblThirdInformation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblThirdInformation.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblThirdInformation.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblThirdInformation.Location = new System.Drawing.Point(120, 192);
		this.lblThirdInformation.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblThirdInformation.Name = "lblThirdInformation";
		this.lblThirdInformation.Size = new System.Drawing.Size(100, 13);
		this.lblThirdInformation.TabIndex = 17;
		this.lblThirdInformation.Text = "Third Information";
		this.lblThirdInformation.Visible = false;
		this.lblThirdMessage.AllowHtmlString = true;
		this.lblThirdMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblThirdMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.lblThirdMessage.Location = new System.Drawing.Point(14, 158);
		this.lblThirdMessage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.lblThirdMessage.Name = "lblThirdMessage";
		this.lblThirdMessage.Size = new System.Drawing.Size(69, 13);
		this.lblThirdMessage.TabIndex = 18;
		this.lblThirdMessage.Text = "Third Message";
		this.lblThirdMessage.Visible = false;
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(414, 311);
		base.ControlBox = false;
		base.Controls.Add(this.lblSecondMessage);
		base.Controls.Add(this.lblSecondInformation);
		base.Controls.Add(this.lblThirdMessage);
		base.Controls.Add(this.lblThirdInformation);
		base.Controls.Add(this.lblHeader);
		base.Controls.Add(this.lblFirstInformation);
		base.Controls.Add(this.lblContactInfo);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.lblFirstMessage);
		base.Controls.Add(this.cbxSuppressDialog);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AdvancedAlertDialogBox";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		((System.ComponentModel.ISupportInitialize)this.cbxSuppressDialog.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void SetDefaultControls(string dialogTitle, string headerMessage, string contactSupportMessage, bool isSuppress, string suppressMessage)
	{
		Text = dialogTitle;
		lblHeader.Text = headerMessage;
		lblContactInfo.Text = contactSupportMessage;
		if (!isSuppress)
		{
			cbxSuppressDialog.Visible = false;
			return;
		}
		cbxSuppressDialog.Visible = true;
		cbxSuppressDialog.Text = suppressMessage;
	}

        private void SetLabelControls(LabelControl messageLabel, LabelControl informationLabel, KeyValuePair<string, string> item)
	{
		messageLabel.Visible = true;
		informationLabel.Visible = true;
		messageLabel.Text = item.Key;
		informationLabel.Text = item.Value;
		Point point2 = default(Point);
		point2.X = base.Width / 2 - informationLabel.Width / 2;
		point2.Y = informationLabel.Location.Y;
		Point point = point2;
		informationLabel.Location = point;
		if (!item.Key.StartsWith("Warning", StringComparison.Ordinal))
		{
			informationLabel.ForeColor = Color.Red;
		}
		else
		{
			_isWarningMessage = true;
		}
	}
    }
}
