using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs
{
	public class JobGroupingEditorDialog : Form
	{
		private IContainer components;

		private ListView w_lvJobWindow;

		private GroupBox w_gbJobs;

		private GroupBox w_gbSummary;

		private Button w_bOkay;

		private Button w_bCancel;

		public JobGroupingEditorDialog()
		{
			this.InitializeComponent();
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
			this.w_lvJobWindow = new ListView();
			this.w_gbJobs = new GroupBox();
			this.w_gbSummary = new GroupBox();
			this.w_bOkay = new Button();
			this.w_bCancel = new Button();
			this.w_gbJobs.SuspendLayout();
			base.SuspendLayout();
			this.w_lvJobWindow.Dock = DockStyle.Fill;
			this.w_lvJobWindow.Location = new Point(5, 18);
			this.w_lvJobWindow.Margin = new System.Windows.Forms.Padding(6);
			this.w_lvJobWindow.Name = "w_lvJobWindow";
			this.w_lvJobWindow.Size = new System.Drawing.Size(172, 241);
			this.w_lvJobWindow.TabIndex = 0;
			this.w_lvJobWindow.UseCompatibleStateImageBehavior = false;
			this.w_gbJobs.Controls.Add(this.w_lvJobWindow);
			this.w_gbJobs.Location = new Point(12, 12);
			this.w_gbJobs.Name = "w_gbJobs";
			this.w_gbJobs.Padding = new System.Windows.Forms.Padding(5);
			this.w_gbJobs.Size = new System.Drawing.Size(182, 264);
			this.w_gbJobs.TabIndex = 1;
			this.w_gbJobs.TabStop = false;
			this.w_gbJobs.Text = "Job Groupings";
			this.w_gbSummary.Location = new Point(200, 12);
			this.w_gbSummary.Name = "w_gbSummary";
			this.w_gbSummary.Size = new System.Drawing.Size(188, 264);
			this.w_gbSummary.TabIndex = 2;
			this.w_gbSummary.TabStop = false;
			this.w_gbSummary.Text = "Summary";
			this.w_bOkay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_bOkay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bOkay.Location = new Point(231, 288);
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Size = new System.Drawing.Size(75, 23);
			this.w_bOkay.TabIndex = 15;
			this.w_bOkay.Text = "OK";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bCancel.Location = new Point(312, 288);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 14;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			base.AcceptButton = this.w_bOkay;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(400, 323);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_gbSummary);
			base.Controls.Add(this.w_gbJobs);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			base.Name = "JobGroupingEditorDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Edit Job Groupings";
			this.w_gbJobs.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}