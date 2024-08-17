using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class DatabaseRestoreDialog : Form
	{
		private string m_sSqlServer;

		private string m_sNewDataBaseName;

		private string m_sDataFileLocation;

		private string m_sLogFileLocation;

		private string m_sUserId;

		private string m_sPassword;

		private bool m_bRestoreRun;

		private bool m_bSqlAuthentication;

		private List<DatabaseBackup> m_databaseBackups;

		private bool m_bUseRedGate;

		private Semaphore m_semCancleLock = new Semaphore(1, 1);

		private SqlCommand m_restoreFileList;

		private object m_restoreFileListLock = new object();

		private SqlCommand m_restoreCommand;

		private object m_restoreCommandLock = new object();

		private SqlCommand m_finalRestoreCommand;

		private object m_finalRestoreCommandLock = new object();

		private bool m_bCanceling;

		private object m_cancelingLock = new object();

		private int m_iTotalBackupsToRestore;

		private object m_iTotalBackupsToRestoreLock = new object();

		private int m_iCurrentRestoringBackupNumber;

		private object m_iCurrentRestoringBackupNumberLock = new object();

		private Thread dbRestoreThread;

		private System.Exception m_exception;

		private string[] m_restoredFiles;

		private IContainer components;

		private ProgressBar w_pbRestoreStatus;

		private Label w_lbRestoring;

		private Label w_lbToDataBase;

		private Button w_bCancel;

		private Label w_lbSourceFile;

		private bool Canceling
		{
			get
			{
				bool mBCanceling;
				lock (this.m_cancelingLock)
				{
					mBCanceling = this.m_bCanceling;
				}
				return mBCanceling;
			}
			set
			{
				lock (this.m_cancelingLock)
				{
					this.m_bCanceling = value;
				}
			}
		}

		private int CurrentRestoringBackupNumber
		{
			get
			{
				int mICurrentRestoringBackupNumber;
				lock (this.m_iCurrentRestoringBackupNumberLock)
				{
					mICurrentRestoringBackupNumber = this.m_iCurrentRestoringBackupNumber;
				}
				return mICurrentRestoringBackupNumber;
			}
			set
			{
				lock (this.m_iCurrentRestoringBackupNumberLock)
				{
					this.m_iCurrentRestoringBackupNumber = value;
				}
			}
		}

		public System.Exception Exception
		{
			get
			{
				return this.m_exception;
			}
		}

		private SqlCommand FinalRestoreCommand
		{
			get
			{
				SqlCommand mFinalRestoreCommand;
				lock (this.m_finalRestoreCommandLock)
				{
					mFinalRestoreCommand = this.m_finalRestoreCommand;
				}
				return mFinalRestoreCommand;
			}
			set
			{
				lock (this.m_finalRestoreCommandLock)
				{
					this.m_finalRestoreCommand = value;
				}
			}
		}

		private RestoreParameters Parameters
		{
			get
			{
				RestoreParameters restoreParameter = new RestoreParameters()
				{
					sSqlServer = this.m_sSqlServer,
					sNewDatabaseName = this.m_sNewDataBaseName,
					sDatabaseDataFileLocation = this.m_sDataFileLocation,
					sDatabaseLogFileLocation = this.m_sLogFileLocation,
					databaseBackups = new List<DatabaseBackup>(this.m_databaseBackups.ToArray()),
					sUserId = this.m_sUserId,
					sPassword = this.m_sPassword,
					bSqlAuthentication = this.m_bSqlAuthentication,
					UseRedGate = this.m_bUseRedGate
				};
				return restoreParameter;
			}
		}

		private SqlCommand RestoreCommand
		{
			get
			{
				SqlCommand mRestoreCommand;
				lock (this.m_restoreCommandLock)
				{
					mRestoreCommand = this.m_restoreCommand;
				}
				return mRestoreCommand;
			}
			set
			{
				lock (this.m_restoreCommandLock)
				{
					this.m_restoreCommand = value;
				}
			}
		}

		public string[] RestoredFiles
		{
			get
			{
				return this.m_restoredFiles;
			}
		}

		private SqlCommand RestoreFileList
		{
			get
			{
				SqlCommand mRestoreFileList;
				lock (this.m_restoreFileListLock)
				{
					mRestoreFileList = this.m_restoreFileList;
				}
				return mRestoreFileList;
			}
			set
			{
				lock (this.m_restoreFileListLock)
				{
					this.m_restoreFileList = value;
				}
			}
		}

		private int TotalBackupsToRestore
		{
			get
			{
				int mITotalBackupsToRestore;
				lock (this.m_iTotalBackupsToRestoreLock)
				{
					mITotalBackupsToRestore = this.m_iTotalBackupsToRestore;
				}
				return mITotalBackupsToRestore;
			}
			set
			{
				lock (this.m_iTotalBackupsToRestoreLock)
				{
					this.m_iTotalBackupsToRestore = value;
				}
			}
		}

		public DatabaseRestoreDialog(string sSqlServer, string sNewDataBaseName, string sDataFilelocation, string sLogFileLocation, List<DatabaseBackup> dbBackups, Credentials creds, bool bUseRedGate)
		{
			string userName;
			string insecureString;
			this.InitializeComponent();
			this.m_sSqlServer = sSqlServer;
			this.m_sNewDataBaseName = sNewDataBaseName;
			this.m_sDataFileLocation = sDataFilelocation;
			this.m_sLogFileLocation = sLogFileLocation;
			this.m_databaseBackups = dbBackups;
			if (creds.IsDefault)
			{
				userName = null;
			}
			else
			{
				userName = creds.UserName;
			}
			this.m_sUserId = userName;
			if (creds.IsDefault)
			{
				insecureString = null;
			}
			else
			{
				insecureString = creds.Password.ToInsecureString();
			}
			this.m_sPassword = insecureString;
			this.m_bSqlAuthentication = false;
			this.m_bUseRedGate = bUseRedGate;
			this.UpdateLabels("", "");
		}

		private void DatabaseRestoreDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.dbRestoreThread.Join();
		}

		private void DatabaseRestoreForm_VisibleChanged(object sender, EventArgs e)
		{
			if (base.Visible && !this.m_bRestoreRun)
			{
				this.m_bRestoreRun = true;
				this.dbRestoreThread = new Thread(new ParameterizedThreadStart(this.FullRestoreDatabase));
				this.dbRestoreThread.Start(this.Parameters);
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

		private void FullRestoreDatabase(object oParameters)
		{
			try
			{
				RestoreParameters restoreParameter = (RestoreParameters)oParameters;
				this.TotalBackupsToRestore = restoreParameter.databaseBackups.Count;
				int num = 1;
				foreach (DatabaseBackup databaseBackup in restoreParameter.databaseBackups)
				{
					this.CurrentRestoringBackupNumber = num;
					this.UpdateLabels(databaseBackup.Name, databaseBackup.SourceFile);
					this.UpdateProgressBar(0);
					this.UpdateTitle(0);
					DatabaseRestorationManager.RestoreDatabase(this.RestoreCommand, this.FinalRestoreCommand, oParameters, databaseBackup, num == restoreParameter.databaseBackups.Count, new SqlInfoMessageEventHandler(this.SqlInfoMessageHandler), this.m_semCancleLock, ref this.m_bCanceling, out this.m_restoredFiles);
					num++;
				}
				this.RestoreCompleted(null, false);
			}
			catch (System.Exception exception1)
			{
				System.Exception exception = exception1;
				if (!this.Canceling)
				{
					this.RestoreCompleted(exception, false);
				}
				else
				{
					this.RestoreCompleted(null, true);
				}
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DatabaseRestoreDialog));
			this.w_pbRestoreStatus = new ProgressBar();
			this.w_lbRestoring = new Label();
			this.w_lbToDataBase = new Label();
			this.w_bCancel = new Button();
			this.w_lbSourceFile = new Label();
			base.SuspendLayout();
			this.w_pbRestoreStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_pbRestoreStatus.Location = new Point(12, 81);
			this.w_pbRestoreStatus.Name = "w_pbRestoreStatus";
			this.w_pbRestoreStatus.Size = new System.Drawing.Size(299, 23);
			this.w_pbRestoreStatus.Style = ProgressBarStyle.Continuous;
			this.w_pbRestoreStatus.TabIndex = 0;
			this.w_lbRestoring.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_lbRestoring.AutoEllipsis = true;
			this.w_lbRestoring.Location = new Point(12, 9);
			this.w_lbRestoring.Name = "w_lbRestoring";
			this.w_lbRestoring.Size = new System.Drawing.Size(299, 23);
			this.w_lbRestoring.TabIndex = 1;
			this.w_lbRestoring.Text = "Opening::";
			this.w_lbToDataBase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_lbToDataBase.AutoEllipsis = true;
			this.w_lbToDataBase.Location = new Point(12, 55);
			this.w_lbToDataBase.Name = "w_lbToDataBase";
			this.w_lbToDataBase.Size = new System.Drawing.Size(299, 23);
			this.w_lbToDataBase.TabIndex = 2;
			this.w_lbToDataBase.Text = "To Database:";
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Location = new Point(236, 110);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 3;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bCancel.Click += new EventHandler(this.w_bCancel_Click);
			this.w_lbSourceFile.AutoEllipsis = true;
			this.w_lbSourceFile.Location = new Point(12, 32);
			this.w_lbSourceFile.Name = "w_lbSourceFile";
			this.w_lbSourceFile.Size = new System.Drawing.Size(299, 23);
			this.w_lbSourceFile.TabIndex = 4;
			this.w_lbSourceFile.Text = "From: ";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(323, 145);
			base.Controls.Add(this.w_lbSourceFile);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_lbToDataBase);
			base.Controls.Add(this.w_lbRestoring);
			base.Controls.Add(this.w_pbRestoreStatus);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DatabaseRestoreDialog";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Opening Database";
			base.VisibleChanged += new EventHandler(this.DatabaseRestoreForm_VisibleChanged);
			base.FormClosing += new FormClosingEventHandler(this.DatabaseRestoreDialog_FormClosing);
			base.ResumeLayout(false);
		}

		private string ReplaceLabelText(string currentText, string newText)
		{
			int num = currentText.IndexOf(":");
			if (num >= 0)
			{
				currentText = currentText.Substring(0, num + 1);
				currentText = string.Concat(currentText, " ", newText);
			}
			return currentText;
		}

		private void RestoreCompleted(System.Exception ex, bool bCanceled)
		{
			if (!base.InvokeRequired)
			{
				if (ex != null)
				{
					this.m_exception = ex;
					base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
					return;
				}
				if (bCanceled)
				{
					base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
					return;
				}
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				return;
			}
			DatabaseRestoreDialog.RestoreCompletedDelegate restoreCompletedDelegate = new DatabaseRestoreDialog.RestoreCompletedDelegate(this.RestoreCompleted);
			object[] objArray = new object[] { ex, bCanceled };
			base.BeginInvoke(restoreCompletedDelegate, objArray);
		}

		private void SqlInfoMessageHandler(object sender, SqlInfoMessageEventArgs args)
		{
			string message = args.Message;
			int num = message.IndexOf(" ");
			if (num >= 0)
			{
				message = message.Substring(0, num);
				int num1 = -1;
				if (int.TryParse(message, out num1) && num1 >= 0 && num1 <= 100)
				{
					this.UpdateProgressBar(num1);
					this.UpdateTitle(num1);
				}
			}
		}

		internal System.Windows.Forms.DialogResult StartRestoration()
		{
			this.FullRestoreDatabase(this.Parameters);
			return base.DialogResult;
		}

		private void UpdateLabels(string sRestoring, string sSourceFile)
		{
			if (base.InvokeRequired)
			{
				DatabaseRestoreDialog.UpdateLabelsDelegate updateLabelsDelegate = new DatabaseRestoreDialog.UpdateLabelsDelegate(this.UpdateLabels);
				object[] objArray = new object[] { sRestoring, sSourceFile };
				base.BeginInvoke(updateLabelsDelegate, objArray);
				return;
			}
			this.w_lbSourceFile.Text = this.ReplaceLabelText(this.w_lbSourceFile.Text, sSourceFile);
			this.w_lbRestoring.Text = this.ReplaceLabelText(this.w_lbRestoring.Text, sRestoring);
			this.w_lbToDataBase.Text = this.ReplaceLabelText(this.w_lbToDataBase.Text, this.m_sNewDataBaseName);
		}

		private void UpdateProgressBar(int value)
		{
			if (!base.InvokeRequired)
			{
				this.w_pbRestoreStatus.Value = value;
			}
			else
			{
				try
				{
					DatabaseRestoreDialog.UpdateProgressBarDelegate updateProgressBarDelegate = new DatabaseRestoreDialog.UpdateProgressBarDelegate(this.UpdateProgressBar);
					object[] objArray = new object[] { value };
					base.BeginInvoke(updateProgressBarDelegate, objArray);
				}
				catch
				{
				}
			}
		}

		private void UpdateTitle(int value)
		{
			if (!base.InvokeRequired)
			{
				string text = this.Text;
				int num = text.LastIndexOf(" - ");
				if (num >= 0)
				{
					text = text.Remove(num);
				}
				object obj = text;
				object[] currentRestoringBackupNumber = new object[] { obj, " - ", this.CurrentRestoringBackupNumber, "/", this.TotalBackupsToRestore, ", ", value.ToString(), "%" };
				text = string.Concat(currentRestoringBackupNumber);
				this.Text = text;
			}
			else
			{
				try
				{
					DatabaseRestoreDialog.UpdateTitleDelegate updateTitleDelegate = new DatabaseRestoreDialog.UpdateTitleDelegate(this.UpdateTitle);
					object[] objArray = new object[] { value };
					base.BeginInvoke(updateTitleDelegate, objArray);
				}
				catch
				{
				}
			}
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			try
			{
				this.m_semCancleLock.WaitOne();
				this.Canceling = true;
				if (this.RestoreFileList != null)
				{
					this.RestoreFileList.Cancel();
				}
				if (this.RestoreCommand != null)
				{
					this.RestoreCommand.Cancel();
				}
				if (this.FinalRestoreCommand != null)
				{
					this.FinalRestoreCommand.Cancel();
				}
				if (this.dbRestoreThread != null)
				{
					this.dbRestoreThread.Abort();
				}
				this.m_semCancleLock.Release();
			}
			catch (System.Exception exception)
			{
				this.m_exception = exception;
				base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
		}

		private delegate void RestoreCompletedDelegate(System.Exception ex, bool bCanceled);

		private delegate void UpdateLabelsDelegate(string sRestoring, string sSourceFile);

		private delegate void UpdateProgressBarDelegate(int value);

		private delegate void UpdateTitleDelegate(int value);
	}
}