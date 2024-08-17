using Metalogix;
using Metalogix.Interfaces;
using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class BackupVersionPickerDialog : Form
	{
		private List<DatabaseBackup> m_backups;

		private string m_sSqlServer;

		private Credentials m_Creds;

		private SortOrder m_order = SortOrder.Descending;

		private IContainer components;

		private Label w_lblInstructions;

		private ListView w_lvBackupVersions;

		private Button w_buttonOK;

		private Button w_buttonCancel;

		private ColumnHeader w_chName;

		private ColumnHeader w_chDate;

		private ColumnHeader w_chDesc;

		private ColumnHeader w_chType;

		private Button w_buttonAddFile;

		public List<DatabaseBackup> Backups
		{
			get
			{
				return this.m_backups;
			}
			set
			{
				this.m_backups = value;
				this.UpdateUI();
			}
		}

		public DatabaseBackup SelectedBackup
		{
			get
			{
				return (DatabaseBackup)this.w_lvBackupVersions.SelectedItems[0].Tag;
			}
		}

		public int SelectedBackupIndex
		{
			get
			{
				if (this.w_lvBackupVersions.SelectedItems.Count <= 0)
				{
					return -1;
				}
				return this.m_backups.IndexOf(this.SelectedBackup);
			}
		}

		public SortOrder SortingOrder
		{
			get
			{
				return this.m_order;
			}
		}

		public BackupVersionPickerDialog(string sSqlServer, Credentials creds)
		{
			this.InitializeComponent();
			this.m_sSqlServer = sSqlServer;
			this.m_Creds = creds;
		}

		private void AddBackupItem(DatabaseBackup backup)
		{
			ListViewItem listViewItem = new ListViewItem(backup.StartDate.ToString("G"));
			listViewItem.SubItems.Add(backup.Name);
			listViewItem.SubItems.Add(backup.Type.ToString());
			listViewItem.SubItems.Add(backup.Description);
			listViewItem.Tag = backup;
			this.w_lvBackupVersions.Items.Add(listViewItem);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BackupVersionPickerDialog));
			this.w_lblInstructions = new Label();
			this.w_lvBackupVersions = new ListView();
			this.w_chDate = new ColumnHeader();
			this.w_chName = new ColumnHeader();
			this.w_chType = new ColumnHeader();
			this.w_chDesc = new ColumnHeader();
			this.w_buttonOK = new Button();
			this.w_buttonCancel = new Button();
			this.w_buttonAddFile = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_lblInstructions, "w_lblInstructions");
			this.w_lblInstructions.Name = "w_lblInstructions";
			componentResourceManager.ApplyResources(this.w_lvBackupVersions, "w_lvBackupVersions");
			ListView.ColumnHeaderCollection columns = this.w_lvBackupVersions.Columns;
			ColumnHeader[] wChDate = new ColumnHeader[] { this.w_chDate, this.w_chName, this.w_chType, this.w_chDesc };
			columns.AddRange(wChDate);
			this.w_lvBackupVersions.FullRowSelect = true;
			this.w_lvBackupVersions.MultiSelect = false;
			this.w_lvBackupVersions.Name = "w_lvBackupVersions";
			this.w_lvBackupVersions.UseCompatibleStateImageBehavior = false;
			this.w_lvBackupVersions.View = View.Details;
			this.w_lvBackupVersions.SelectedIndexChanged += new EventHandler(this.On_Backup_Selected);
			this.w_lvBackupVersions.ColumnClick += new ColumnClickEventHandler(this.On_Column_Clicked);
			componentResourceManager.ApplyResources(this.w_chDate, "w_chDate");
			componentResourceManager.ApplyResources(this.w_chName, "w_chName");
			componentResourceManager.ApplyResources(this.w_chType, "w_chType");
			componentResourceManager.ApplyResources(this.w_chDesc, "w_chDesc");
			componentResourceManager.ApplyResources(this.w_buttonOK, "w_buttonOK");
			this.w_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_buttonOK.Name = "w_buttonOK";
			this.w_buttonOK.UseVisualStyleBackColor = true;
			this.w_buttonOK.Click += new EventHandler(this.On_buttonOK_Click);
			componentResourceManager.ApplyResources(this.w_buttonCancel, "w_buttonCancel");
			this.w_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_buttonCancel.Name = "w_buttonCancel";
			this.w_buttonCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_buttonAddFile, "w_buttonAddFile");
			this.w_buttonAddFile.Name = "w_buttonAddFile";
			this.w_buttonAddFile.UseVisualStyleBackColor = true;
			this.w_buttonAddFile.Click += new EventHandler(this.On_buttonAddFile_Click);
			base.AcceptButton = this.w_buttonOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_buttonCancel;
			base.Controls.Add(this.w_buttonAddFile);
			base.Controls.Add(this.w_buttonOK);
			base.Controls.Add(this.w_buttonCancel);
			base.Controls.Add(this.w_lvBackupVersions);
			base.Controls.Add(this.w_lblInstructions);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BackupVersionPickerDialog";
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_Backup_Selected(object sender, EventArgs e)
		{
			this.w_buttonOK.Enabled = this.w_lvBackupVersions.SelectedIndices.Count > 0;
		}

		private void On_buttonAddFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				Multiselect = true,
				AddExtension = true,
				DefaultExt = ".bak",
				Filter = "Database Files (*.bak)|*.bak",
				Title = "Select the SQL Server backup file(s)"
			};
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string[] fileNames = openFileDialog.FileNames;
				for (int i = 0; i < (int)fileNames.Length; i++)
				{
					string str = fileNames[i];
					if (str.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
					{
						bool flag = false;
						foreach (DatabaseBackup backup in this.Backups)
						{
							if (!string.Equals(str, backup.SourceFile, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
							flag = true;
							break;
						}
						if (!flag)
						{
							List<DatabaseBackup> databaseBackupsFromFile = null;
							try
							{
								databaseBackupsFromFile = DatabaseRestorationManager.GetDatabaseBackupsFromFile(this.m_sSqlServer, str, this.m_Creds);
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								GlobalServices.ErrorHandler.HandleException("Error Retrieving Backup", exception.Message, exception, ErrorIcon.Error);
								return;
							}
							bool flag1 = true;
							if (this.Backups.Count > 0)
							{
								foreach (DatabaseBackup databaseBackup in databaseBackupsFromFile)
								{
									if (DatabaseRestorationManager.BakFilesAreConsistent(databaseBackup, this.Backups[0]))
									{
										continue;
									}
									object[] sourceFile = new object[] { databaseBackup.SourceFile, databaseBackup.SourceServerName, databaseBackup.SourceDatabaseName, this.Backups[0].SourceServerName, this.Backups[0].SourceDatabaseName };
									string str1 = string.Format("The backup file '{0}' contains a backup that is inconsistent with the existing backups being used. None of the backups from this file will be added to the list of usable backups.\n\nThe inconsistent backup has server = '{1}' and database = '{2}', while the already chosen backups have server = '{3}' and database = '{4}'.", sourceFile);
									FlatXtraMessageBox.Show(str1, "Inconsistent Backup", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
									flag1 = false;
									break;
								}
							}
							if (flag1)
							{
								this.Backups.AddRange(databaseBackupsFromFile);
							}
						}
					}
					else
					{
						FlatXtraMessageBox.Show(string.Concat("The chosen file '", str, "' is not a .bak file and will not be used to add new backups to the list of backups."), "Wrong File Type", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
				}
				this.UpdateUI();
			}
		}

		private void On_buttonOK_Click(object sender, EventArgs e)
		{
			if (this.SelectedBackupIndex < 0)
			{
				FlatXtraMessageBox.Show("Please select a backup point in time.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				base.DialogResult = System.Windows.Forms.DialogResult.None;
				return;
			}
			if (this.SelectedBackup.Type == DatabaseBackup.BackupType.Differential || this.SelectedBackup.Type == DatabaseBackup.BackupType.TransactionLog)
			{
				foreach (DatabaseBackup backup in this.Backups)
				{
					if (backup.Type != DatabaseBackup.BackupType.Full || !backup.IsBefore(this.SelectedBackup))
					{
						continue;
					}
					return;
				}
				base.DialogResult = System.Windows.Forms.DialogResult.None;
				FlatXtraMessageBox.Show("Differential and transaction log backups need to be based off of a full backup. No full backup, that has an earlier timestamp than the chosen backup, could be found in the chosen backup files.\n\nMake sure that all of the Sharepoint database backup files have been included. Additional backup files can be added by using the 'Add backup file' button.", "Cannot Find Earlier Full Backup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.w_lvBackupVersions.SelectedItems.Clear();
			}
		}

		private void On_Column_Clicked(object sender, ColumnClickEventArgs e)
		{
			if (this.w_lvBackupVersions.Columns[e.Column] == this.w_chDate)
			{
				if (this.m_order != SortOrder.Descending)
				{
					this.m_order = SortOrder.Descending;
				}
				else
				{
					this.m_order = SortOrder.Ascending;
				}
				this.UpdateUI();
			}
		}

		public void UpdateUI()
		{
			base.SuspendLayout();
			this.w_lvBackupVersions.Items.Clear();
			DatabaseBackup.SortBackups(this.Backups, (this.m_order == SortOrder.Ascending ? DatabaseBackup.BackupSortingOrder.EarliestToMostRecent : DatabaseBackup.BackupSortingOrder.MostRecentToEarliest));
			if (this.m_backups.Count > 0)
			{
				foreach (DatabaseBackup mBackup in this.m_backups)
				{
					this.AddBackupItem(mBackup);
				}
			}
			base.ResumeLayout();
			this.On_Backup_Selected(null, null);
		}
	}
}