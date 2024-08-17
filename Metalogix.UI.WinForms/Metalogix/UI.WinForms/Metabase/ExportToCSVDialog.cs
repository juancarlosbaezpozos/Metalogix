using Metalogix.Metabase.Options;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	public class ExportToCSVDialog : Form
	{
		private IContainer components;

		private Button w_btnCancel;

		private Button w_btnOk;

		private SaveFileDialog w_saveFileDialog;

		private ExportToCSVControl w_exportToCSVControl;

		public bool IsExcelFile
		{
			set
			{
				this.w_exportToCSVControl.IsExcelFile = value;
				if (value)
				{
					this.Text = "Export to Excel File";
					return;
				}
				this.Text = "Export to CSV File";
			}
		}

		public ExportToCSVOptions Options
		{
			get
			{
				return this.w_exportToCSVControl.SaveOptions();
			}
			set
			{
				this.w_exportToCSVControl.Options = value;
			}
		}

		public new PropertyDescriptorCollection Properties
		{
			set
			{
				this.w_exportToCSVControl.Properties = value;
			}
		}

		public ExportToCSVDialog(bool isExcelFile)
		{
			this.InitializeComponent();
			this.IsExcelFile = isExcelFile;
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
			this.w_btnCancel = new Button();
			this.w_btnOk = new Button();
			this.w_saveFileDialog = new SaveFileDialog();
			this.w_exportToCSVControl = new ExportToCSVControl();
			base.SuspendLayout();
			this.w_btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Location = new Point(399, 317);
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.w_btnCancel.TabIndex = 2;
			this.w_btnCancel.Text = "&Cancel";
			this.w_btnCancel.UseVisualStyleBackColor = true;
			this.w_btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOk.Location = new Point(318, 317);
			this.w_btnOk.Name = "w_btnOk";
			this.w_btnOk.Size = new System.Drawing.Size(75, 23);
			this.w_btnOk.TabIndex = 1;
			this.w_btnOk.Text = "&OK";
			this.w_btnOk.UseVisualStyleBackColor = true;
			this.w_saveFileDialog.DefaultExt = "csv";
			this.w_saveFileDialog.Filter = "CSV files|*.csv|All files|*.*";
			this.w_saveFileDialog.Title = "Save CSV Export As";
			this.w_exportToCSVControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_exportToCSVControl.Location = new Point(12, 12);
			this.w_exportToCSVControl.Name = "w_exportToCSVControl";
			this.w_exportToCSVControl.Size = new System.Drawing.Size(466, 299);
			this.w_exportToCSVControl.TabIndex = 0;
			base.AcceptButton = this.w_btnOk;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.ClientSize = new System.Drawing.Size(486, 352);
			base.Controls.Add(this.w_exportToCSVControl);
			base.Controls.Add(this.w_btnOk);
			base.Controls.Add(this.w_btnCancel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(502, 390);
			base.Name = "ExportToCSVDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Export to CSV File";
			base.ResumeLayout(false);
		}
	}
}