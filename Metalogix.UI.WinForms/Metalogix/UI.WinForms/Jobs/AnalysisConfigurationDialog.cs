using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs
{
	public class AnalysisConfigurationDialog : Form
	{
		private IContainer components;

		private Button w_bOkay;

		private Button w_bCancel;

		private Label w_lblAnalyzeFrom;

		private Panel w_pDateTimeSelector;

		private RadioButton w_rbAfterDate;

		private RadioButton w_rbFromLastRun;

		private DateTimePicker w_dtPicker;

		public DateTime? PivotDate
		{
			get
			{
				if (!this.w_rbAfterDate.Checked || !this.w_rbAfterDate.Enabled)
				{
					return null;
				}
				return new DateTime?(this.w_dtPicker.Value);
			}
		}

		public AnalysisConfigurationDialog()
		{
			this.InitializeComponent();
			this.w_rbFromLastRun.Checked = true;
			this.w_dtPicker.Format = DateTimePickerFormat.Custom;
			this.w_dtPicker.CustomFormat = string.Concat(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, " ", CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
			this.w_dtPicker.Enabled = (!this.w_rbAfterDate.Enabled ? false : this.w_rbAfterDate.Checked);
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
			this.w_bOkay = new Button();
			this.w_bCancel = new Button();
			this.w_lblAnalyzeFrom = new Label();
			this.w_pDateTimeSelector = new Panel();
			this.w_dtPicker = new DateTimePicker();
			this.w_rbAfterDate = new RadioButton();
			this.w_rbFromLastRun = new RadioButton();
			this.w_pDateTimeSelector.SuspendLayout();
			base.SuspendLayout();
			this.w_bOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_bOkay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bOkay.Location = new Point(146, 95);
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Size = new System.Drawing.Size(75, 23);
			this.w_bOkay.TabIndex = 2;
			this.w_bOkay.Text = "OK";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bCancel.Location = new Point(227, 95);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 3;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_lblAnalyzeFrom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_lblAnalyzeFrom.Location = new Point(12, 9);
			this.w_lblAnalyzeFrom.Name = "w_lblAnalyzeFrom";
			this.w_lblAnalyzeFrom.Size = new System.Drawing.Size(293, 23);
			this.w_lblAnalyzeFrom.TabIndex = 0;
			this.w_lblAnalyzeFrom.Text = "Analyze changes";
			this.w_lblAnalyzeFrom.TextAlign = ContentAlignment.MiddleLeft;
			this.w_pDateTimeSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_pDateTimeSelector.Controls.Add(this.w_dtPicker);
			this.w_pDateTimeSelector.Controls.Add(this.w_rbAfterDate);
			this.w_pDateTimeSelector.Controls.Add(this.w_rbFromLastRun);
			this.w_pDateTimeSelector.Location = new Point(16, 32);
			this.w_pDateTimeSelector.Name = "w_pDateTimeSelector";
			this.w_pDateTimeSelector.Size = new System.Drawing.Size(286, 57);
			this.w_pDateTimeSelector.TabIndex = 1;
			this.w_dtPicker.Location = new Point(71, 24);
			this.w_dtPicker.Name = "w_dtPicker";
			this.w_dtPicker.Size = new System.Drawing.Size(212, 20);
			this.w_dtPicker.TabIndex = 2;
			this.w_rbAfterDate.AutoSize = true;
			this.w_rbAfterDate.Location = new Point(18, 26);
			this.w_rbAfterDate.Name = "w_rbAfterDate";
			this.w_rbAfterDate.Size = new System.Drawing.Size(47, 17);
			this.w_rbAfterDate.TabIndex = 1;
			this.w_rbAfterDate.TabStop = true;
			this.w_rbAfterDate.Text = "After";
			this.w_rbAfterDate.UseVisualStyleBackColor = true;
			this.w_rbAfterDate.CheckedChanged += new EventHandler(this.On_RadioButton_CheckedChanged);
			this.w_rbFromLastRun.AutoSize = true;
			this.w_rbFromLastRun.Location = new Point(18, 3);
			this.w_rbFromLastRun.Name = "w_rbFromLastRun";
			this.w_rbFromLastRun.Size = new System.Drawing.Size(128, 17);
			this.w_rbFromLastRun.TabIndex = 0;
			this.w_rbFromLastRun.TabStop = true;
			this.w_rbFromLastRun.Text = "Since job was last run";
			this.w_rbFromLastRun.UseVisualStyleBackColor = true;
			base.AcceptButton = this.w_bOkay;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(317, 130);
			base.Controls.Add(this.w_pDateTimeSelector);
			base.Controls.Add(this.w_lblAnalyzeFrom);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "AnalysisConfigurationDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Configure Job Analysis";
			this.w_pDateTimeSelector.ResumeLayout(false);
			this.w_pDateTimeSelector.PerformLayout();
			base.ResumeLayout(false);
		}

		private void On_RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.w_dtPicker.Enabled = (!this.w_rbAfterDate.Enabled ? false : this.w_rbAfterDate.Checked);
		}
	}
}