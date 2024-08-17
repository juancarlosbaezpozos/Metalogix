using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class MdfCreateNewLogFileDialog : Form
	{
		private IContainer components;

		private Label w_lblCannotFindLog;

		private Label w_lblExpectedLogFile;

		private Label w_lblExpectedLogFileLocation;

		private Label w_lblNewLogFile;

		private Label w_lblSupport;

		private Label w_lblCreateNewLogFileQuestion;

		private Button w_btnYes;

		private Button w_btnNo;

		private Label w_lblDatabaseFile;

		public MdfCreateNewLogFileDialog(string sBackupFile, string sExpectedLogFile)
		{
			this.InitializeComponent();
			this.w_lblDatabaseFile.Text = sBackupFile;
			this.w_lblExpectedLogFileLocation.Text = sExpectedLogFile;
			int right = (this.w_lblDatabaseFile.Right > this.w_lblExpectedLogFileLocation.Right ? this.w_lblDatabaseFile.Right : this.w_lblExpectedLogFileLocation.Right);
			if (right < this.w_lblNewLogFile.Right)
			{
				right = this.w_lblNewLogFile.Right;
			}
			if (right > base.Width)
			{
				base.Width = right + 20;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MdfCreateNewLogFileDialog));
			this.w_lblCannotFindLog = new Label();
			this.w_lblExpectedLogFile = new Label();
			this.w_lblExpectedLogFileLocation = new Label();
			this.w_lblNewLogFile = new Label();
			this.w_lblSupport = new Label();
			this.w_lblCreateNewLogFileQuestion = new Label();
			this.w_btnYes = new Button();
			this.w_btnNo = new Button();
			this.w_lblDatabaseFile = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_lblCannotFindLog, "w_lblCannotFindLog");
			this.w_lblCannotFindLog.Name = "w_lblCannotFindLog";
			componentResourceManager.ApplyResources(this.w_lblExpectedLogFile, "w_lblExpectedLogFile");
			this.w_lblExpectedLogFile.Name = "w_lblExpectedLogFile";
			componentResourceManager.ApplyResources(this.w_lblExpectedLogFileLocation, "w_lblExpectedLogFileLocation");
			this.w_lblExpectedLogFileLocation.Name = "w_lblExpectedLogFileLocation";
			componentResourceManager.ApplyResources(this.w_lblNewLogFile, "w_lblNewLogFile");
			this.w_lblNewLogFile.Name = "w_lblNewLogFile";
			componentResourceManager.ApplyResources(this.w_lblSupport, "w_lblSupport");
			this.w_lblSupport.Name = "w_lblSupport";
			componentResourceManager.ApplyResources(this.w_lblCreateNewLogFileQuestion, "w_lblCreateNewLogFileQuestion");
			this.w_lblCreateNewLogFileQuestion.Name = "w_lblCreateNewLogFileQuestion";
			componentResourceManager.ApplyResources(this.w_btnYes, "w_btnYes");
			this.w_btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.w_btnYes.Name = "w_btnYes";
			this.w_btnYes.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_btnNo, "w_btnNo");
			this.w_btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.w_btnNo.Name = "w_btnNo";
			this.w_btnNo.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_lblDatabaseFile, "w_lblDatabaseFile");
			this.w_lblDatabaseFile.Name = "w_lblDatabaseFile";
			base.AcceptButton = this.w_btnYes;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnNo;
			base.Controls.Add(this.w_lblDatabaseFile);
			base.Controls.Add(this.w_btnNo);
			base.Controls.Add(this.w_btnYes);
			base.Controls.Add(this.w_lblCreateNewLogFileQuestion);
			base.Controls.Add(this.w_lblSupport);
			base.Controls.Add(this.w_lblNewLogFile);
			base.Controls.Add(this.w_lblExpectedLogFileLocation);
			base.Controls.Add(this.w_lblExpectedLogFile);
			base.Controls.Add(this.w_lblCannotFindLog);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MdfCreateNewLogFileDialog";
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}