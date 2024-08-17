using Metalogix.SharePoint.Options.Administration;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public class AnalyzeChurnConfigurationDialog : Form
	{
		private AnalyzeChurnOptions m_options;

		private IContainer components;

		private Button w_bOkay;

		private Button w_bCancel;

		private Label w_lblAnalyzeFrom;

		private DateTimePicker w_dtPicker;

		private CheckBox w_cbRecusive;

		public AnalyzeChurnOptions Options
		{
			get
			{
				return this.m_options;
			}
			set
			{
				this.m_options = value;
				this.LoadUI();
			}
		}

		public AnalyzeChurnConfigurationDialog()
		{
			this.InitializeComponent();
			this.w_dtPicker.Format = DateTimePickerFormat.Custom;
			this.w_dtPicker.CustomFormat = string.Concat(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, " ", CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
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
			this.w_dtPicker = new DateTimePicker();
			this.w_cbRecusive = new CheckBox();
			base.SuspendLayout();
			this.w_bOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_bOkay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bOkay.Location = new Point(117, 95);
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Size = new System.Drawing.Size(75, 23);
			this.w_bOkay.TabIndex = 3;
			this.w_bOkay.Text = "OK";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bOkay.Click += new EventHandler(this.On_Okay);
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bCancel.Location = new Point(198, 95);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 4;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_lblAnalyzeFrom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_lblAnalyzeFrom.Location = new Point(12, 9);
			this.w_lblAnalyzeFrom.Name = "w_lblAnalyzeFrom";
			this.w_lblAnalyzeFrom.Size = new System.Drawing.Size(264, 23);
			this.w_lblAnalyzeFrom.TabIndex = 0;
			this.w_lblAnalyzeFrom.Text = "Analyze churn since";
			this.w_lblAnalyzeFrom.TextAlign = ContentAlignment.MiddleLeft;
			this.w_dtPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_dtPicker.Location = new Point(15, 35);
			this.w_dtPicker.Name = "w_dtPicker";
			this.w_dtPicker.Size = new System.Drawing.Size(258, 20);
			this.w_dtPicker.TabIndex = 1;
			this.w_cbRecusive.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_cbRecusive.Location = new Point(15, 61);
			this.w_cbRecusive.Name = "w_cbRecusive";
			this.w_cbRecusive.Size = new System.Drawing.Size(261, 24);
			this.w_cbRecusive.TabIndex = 2;
			this.w_cbRecusive.Text = "Analyze Recursively";
			this.w_cbRecusive.UseVisualStyleBackColor = true;
			base.AcceptButton = this.w_bOkay;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(288, 130);
			base.Controls.Add(this.w_cbRecusive);
			base.Controls.Add(this.w_dtPicker);
			base.Controls.Add(this.w_lblAnalyzeFrom);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "AnalysisConfigurationDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Configure Churn Analysis";
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			this.w_dtPicker.Value = this.Options.PivotDate;
			this.w_cbRecusive.Checked = this.Options.AnalyzeRecursively;
		}

		private void On_Okay(object sender, EventArgs e)
		{
			this.SaveUI();
		}

		private void SaveUI()
		{
			this.Options.PivotDate = this.w_dtPicker.Value;
			this.Options.AnalyzeRecursively = this.w_cbRecusive.Checked;
		}
	}
}