using System;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class MdfFileCreationDialog : CollapsableForm
	{
		private IContainer components;

		private Label w_lblConnecting;

		private Label w_lblSqlServer;

		private Label w_lblDatabaseFile;

		private Label w_lblNewDatabaseName;

		private Label label1;

		private Button w_btnCancel;

		private Button w_btnOK;

		private Panel w_pnlNewLogFile;

		private Label w_lblNewLogFileName;

		private Label w_lblNewLogFileMsg;

		public MdfFileCreationDialog(string sMdfBackupFile, string sSqlServer, string sNewSqlDatabaseName, bool bNewLogFile)
		{
			InitializeComponent();
			w_lblDatabaseFile.Text = sMdfBackupFile;
			w_lblNewDatabaseName.Text = sNewSqlDatabaseName;
			w_lblSqlServer.Text = $"will create a new database on SQL Server '{sSqlServer}'.";
			int num = ((w_lblDatabaseFile.Right > w_lblNewDatabaseName.Right) ? w_lblDatabaseFile.Right : w_lblNewDatabaseName.Right);
			if (!bNewLogFile)
			{
				HideControl(w_pnlNewLogFile);
			}
			else
			{
				w_lblNewLogFileName.Text = sNewSqlDatabaseName + "_log.ldf";
				if (num < w_lblNewLogFileName.Right + w_pnlNewLogFile.Left)
				{
					num = w_lblNewLogFileName.Right + w_pnlNewLogFile.Left;
				}
			}
			if (base.Width < num)
			{
				base.Width = num + 20;
				w_pnlNewLogFile.Width = num - w_pnlNewLogFile.Left;
			}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Database.MdfFileCreationDialog));
			this.w_lblConnecting = new System.Windows.Forms.Label();
			this.w_lblSqlServer = new System.Windows.Forms.Label();
			this.w_lblDatabaseFile = new System.Windows.Forms.Label();
			this.w_lblNewDatabaseName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.w_btnCancel = new System.Windows.Forms.Button();
			this.w_btnOK = new System.Windows.Forms.Button();
			this.w_pnlNewLogFile = new System.Windows.Forms.Panel();
			this.w_lblNewLogFileName = new System.Windows.Forms.Label();
			this.w_lblNewLogFileMsg = new System.Windows.Forms.Label();
			this.w_pnlNewLogFile.SuspendLayout();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lblConnecting, "w_lblConnecting");
			this.w_lblConnecting.Name = "w_lblConnecting";
			resources.ApplyResources(this.w_lblSqlServer, "w_lblSqlServer");
			this.w_lblSqlServer.Name = "w_lblSqlServer";
			resources.ApplyResources(this.w_lblDatabaseFile, "w_lblDatabaseFile");
			this.w_lblDatabaseFile.Name = "w_lblDatabaseFile";
			resources.ApplyResources(this.w_lblNewDatabaseName, "w_lblNewDatabaseName");
			this.w_lblNewDatabaseName.Name = "w_lblNewDatabaseName";
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.UseVisualStyleBackColor = true;
			this.w_pnlNewLogFile.Controls.Add(this.w_lblNewLogFileName);
			this.w_pnlNewLogFile.Controls.Add(this.w_lblNewLogFileMsg);
			resources.ApplyResources(this.w_pnlNewLogFile, "w_pnlNewLogFile");
			this.w_pnlNewLogFile.Name = "w_pnlNewLogFile";
			this.w_pnlNewLogFile.Resize += new System.EventHandler(On_pnlNewLogFile_Resize);
			resources.ApplyResources(this.w_lblNewLogFileName, "w_lblNewLogFileName");
			this.w_lblNewLogFileName.Name = "w_lblNewLogFileName";
			resources.ApplyResources(this.w_lblNewLogFileMsg, "w_lblNewLogFileMsg");
			this.w_lblNewLogFileMsg.Name = "w_lblNewLogFileMsg";
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.w_pnlNewLogFile);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_lblNewDatabaseName);
			base.Controls.Add(this.w_lblDatabaseFile);
			base.Controls.Add(this.w_lblSqlServer);
			base.Controls.Add(this.w_lblConnecting);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MdfFileCreationDialog";
			base.ShowInTaskbar = false;
			this.w_pnlNewLogFile.ResumeLayout(false);
			this.w_pnlNewLogFile.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_pnlNewLogFile_Resize(object sender, EventArgs e)
		{
			w_lblNewLogFileMsg.Width = w_pnlNewLogFile.Width;
		}
	}
}
