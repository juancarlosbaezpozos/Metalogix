using Metalogix;
using Metalogix.Permissions;
using Metalogix.SharePoint.Database;
using Metalogix.UI.WinForms;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class BackupFileCreationDialog : Form
	{
		private string m_sRedGateServiceStatus = "Not Installed";

		private string m_sSqlServer;

		private string m_sNewDatabaseName;

		private List<DatabaseBackup> m_backups;

		private string m_sUserName;

		private string m_sPassword;

		private string m_sDatabaseName;

		private string m_sDataFileLocation;

		private string m_sLogFileLocation;

		private bool m_bUseRedGate;

		private List<FileDescription> m_FileList = new List<FileDescription>();

		private IContainer components;

		private Label w_lblDatabaseRestored;

		private Label w_lblFilesToCreate;

		private Button w_buttonOK;

		private Button w_buttonCancel;

		private Label label1;

		private Label label2;

		private ListView w_listViewFiles;

		private ColumnHeader w_columnHeader1;

		private ColumnHeader w_columnHeader2;

		private ColumnHeader w_columnHeader3;

		private TextBox w_txtSQLServer;

		private TextBox w_txtTempDatabase;

		private TextBox w_txtBackupFile;

		private LinkLabel w_linkEditLocations;

		private LinkLabel linkLabel1;

		private LinkLabel linkLabel2;

		private GroupBox groupBox1;

		private RadioButton w_radioButtonRedGate;

		private RadioButton w_radioButtonNativeSQL;

		private Label w_lblRedGateState;

		private string DatabaseName
		{
			set
			{
				this.m_sDatabaseName = value;
			}
		}

		public string DataFileLocation
		{
			get
			{
				return this.m_sDataFileLocation;
			}
			set
			{
				this.m_sDataFileLocation = value;
			}
		}

		public string LogFileLocation
		{
			get
			{
				return this.m_sLogFileLocation;
			}
			set
			{
				this.m_sLogFileLocation = value;
			}
		}

		private string RedGateServiceStatus
		{
			get
			{
				return this.m_sRedGateServiceStatus;
			}
		}

		public bool UseRedGate
		{
			get
			{
				return this.m_bUseRedGate;
			}
		}

		public BackupFileCreationDialog(string sSqlServer, string sNewDatabaseName, List<DatabaseBackup> backups, Credentials creds)
		{
			string userName;
			string insecureString;
			this.InitializeComponent();
			this.m_sSqlServer = sSqlServer;
			this.m_sNewDatabaseName = sNewDatabaseName;
			this.m_backups = backups;
			if (creds.IsDefault)
			{
				userName = null;
			}
			else
			{
				userName = creds.UserName;
			}
			this.m_sUserName = userName;
			if (creds.IsDefault)
			{
				insecureString = null;
			}
			else
			{
				insecureString = creds.Password.ToInsecureString();
			}
			this.m_sPassword = insecureString;
			this.InitializeFileLocations();
			foreach (DatabaseBackup mBackup in this.m_backups)
			{
				this.GetRestorationFileInformation(mBackup);
			}
			this.UpdateUI();
		}

		private SqlConnectionStringBuilder CreateSqlConnection()
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
			sqlConnectionStringBuilder["Data Source"] = this.m_sSqlServer;
			if (this.m_sUserName != null)
			{
				sqlConnectionStringBuilder.UserID = this.m_sUserName;
				sqlConnectionStringBuilder.Password = this.m_sPassword;
			}
			else
			{
				sqlConnectionStringBuilder["integrated Security"] = true;
			}
			sqlConnectionStringBuilder.AsynchronousProcessing = true;
			sqlConnectionStringBuilder.InitialCatalog = "master";
			return sqlConnectionStringBuilder;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void GetRestorationFileInformation(DatabaseBackup dbBackup)
		{
			long? nullable;
			using (SqlConnection sqlConnection = new SqlConnection(this.CreateSqlConnection().ConnectionString))
			{
				SqlDataReader sqlDataReader = null;
				try
				{
					SqlCommand sqlCommand = new SqlCommand(string.Concat("RESTORE FILELISTONLY FROM DISK='", dbBackup.SourceFile, "' WITH FILE=", dbBackup.Position.ToString()), sqlConnection)
					{
						CommandTimeout = 0
					};
					sqlConnection.Open();
					IAsyncResult asyncResult = sqlCommand.BeginExecuteReader();
					while (!asyncResult.IsCompleted)
					{
					}
					sqlDataReader = sqlCommand.EndExecuteReader(asyncResult);
					while (sqlDataReader.Read())
					{
						string str = sqlDataReader["PhysicalName"].ToString();
						string str1 = str.Substring(0, str.LastIndexOf("\\") + 1);
						string str2 = str.Substring(str.LastIndexOf("\\") + 1);
						if (sqlDataReader["Size"] == DBNull.Value || sqlDataReader["Size"] == null)
						{
							nullable = null;
						}
						else
						{
							nullable = new long?(Convert.ToInt64(sqlDataReader["Size"]));
						}
						long? nullable1 = nullable;
						string str3 = string.Concat(str1, this.m_sNewDatabaseName, str2);
						FileDescription fileDescription = new FileDescription(sqlDataReader["LogicalName"].ToString(), str3, nullable1, sqlDataReader["Type"].ToString());
						if (!this.m_FileList.Contains(fileDescription))
						{
							this.m_FileList.Add(fileDescription);
						}
						if (sqlDataReader["FileGroupName"].ToString() != "PRIMARY")
						{
							continue;
						}
						this.DatabaseName = sqlDataReader["LogicalName"].ToString();
					}
				}
				finally
				{
					if (sqlDataReader != null && !sqlDataReader.IsClosed)
					{
						sqlDataReader.Close();
					}
					sqlConnection.Close();
				}
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BackupFileCreationDialog));
			this.w_lblDatabaseRestored = new Label();
			this.w_lblFilesToCreate = new Label();
			this.w_buttonOK = new Button();
			this.w_buttonCancel = new Button();
			this.label1 = new Label();
			this.label2 = new Label();
			this.w_listViewFiles = new ListView();
			this.w_columnHeader1 = new ColumnHeader();
			this.w_columnHeader2 = new ColumnHeader();
			this.w_columnHeader3 = new ColumnHeader();
			this.w_txtSQLServer = new TextBox();
			this.w_txtTempDatabase = new TextBox();
			this.w_txtBackupFile = new TextBox();
			this.w_linkEditLocations = new LinkLabel();
			this.linkLabel1 = new LinkLabel();
			this.linkLabel2 = new LinkLabel();
			this.groupBox1 = new GroupBox();
			this.w_lblRedGateState = new Label();
			this.w_radioButtonRedGate = new RadioButton();
			this.w_radioButtonNativeSQL = new RadioButton();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_lblDatabaseRestored, "w_lblDatabaseRestored");
			this.w_lblDatabaseRestored.Name = "w_lblDatabaseRestored";
			componentResourceManager.ApplyResources(this.w_lblFilesToCreate, "w_lblFilesToCreate");
			this.w_lblFilesToCreate.Name = "w_lblFilesToCreate";
			componentResourceManager.ApplyResources(this.w_buttonOK, "w_buttonOK");
			this.w_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_buttonOK.Name = "w_buttonOK";
			this.w_buttonOK.UseVisualStyleBackColor = true;
			this.w_buttonOK.Click += new EventHandler(this.On_buttonOK_Click);
			componentResourceManager.ApplyResources(this.w_buttonCancel, "w_buttonCancel");
			this.w_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_buttonCancel.Name = "w_buttonCancel";
			this.w_buttonCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.w_listViewFiles, "w_listViewFiles");
			ListView.ColumnHeaderCollection columns = this.w_listViewFiles.Columns;
			ColumnHeader[] wColumnHeader1 = new ColumnHeader[] { this.w_columnHeader1, this.w_columnHeader2, this.w_columnHeader3 };
			columns.AddRange(wColumnHeader1);
			this.w_listViewFiles.FullRowSelect = true;
			this.w_listViewFiles.MultiSelect = false;
			this.w_listViewFiles.Name = "w_listViewFiles";
			this.w_listViewFiles.UseCompatibleStateImageBehavior = false;
			this.w_listViewFiles.View = View.Details;
			componentResourceManager.ApplyResources(this.w_columnHeader1, "w_columnHeader1");
			componentResourceManager.ApplyResources(this.w_columnHeader2, "w_columnHeader2");
			componentResourceManager.ApplyResources(this.w_columnHeader3, "w_columnHeader3");
			componentResourceManager.ApplyResources(this.w_txtSQLServer, "w_txtSQLServer");
			this.w_txtSQLServer.Name = "w_txtSQLServer";
			this.w_txtSQLServer.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_txtTempDatabase, "w_txtTempDatabase");
			this.w_txtTempDatabase.Name = "w_txtTempDatabase";
			this.w_txtTempDatabase.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_txtBackupFile, "w_txtBackupFile");
			this.w_txtBackupFile.Name = "w_txtBackupFile";
			this.w_txtBackupFile.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_linkEditLocations, "w_linkEditLocations");
			this.w_linkEditLocations.Name = "w_linkEditLocations";
			this.w_linkEditLocations.TabStop = true;
			this.w_linkEditLocations.LinkClicked += new LinkLabelLinkClickedEventHandler(this.On_linkEditLocations_LinkClicked);
			componentResourceManager.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.On_linkLabel1_LinkClicked);
			componentResourceManager.ApplyResources(this.linkLabel2, "linkLabel2");
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.TabStop = true;
			this.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
			componentResourceManager.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.w_lblRedGateState);
			this.groupBox1.Controls.Add(this.w_radioButtonRedGate);
			this.groupBox1.Controls.Add(this.w_radioButtonNativeSQL);
			this.groupBox1.Controls.Add(this.linkLabel2);
			this.groupBox1.Controls.Add(this.linkLabel1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			componentResourceManager.ApplyResources(this.w_lblRedGateState, "w_lblRedGateState");
			this.w_lblRedGateState.Name = "w_lblRedGateState";
			componentResourceManager.ApplyResources(this.w_radioButtonRedGate, "w_radioButtonRedGate");
			this.w_radioButtonRedGate.Name = "w_radioButtonRedGate";
			this.w_radioButtonRedGate.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_radioButtonNativeSQL, "w_radioButtonNativeSQL");
			this.w_radioButtonNativeSQL.Checked = true;
			this.w_radioButtonNativeSQL.Name = "w_radioButtonNativeSQL";
			this.w_radioButtonNativeSQL.TabStop = true;
			this.w_radioButtonNativeSQL.UseVisualStyleBackColor = true;
			this.w_radioButtonNativeSQL.CheckedChanged += new EventHandler(this.On_radioButtonNativeSQL_CheckedChanged);
			base.AcceptButton = this.w_buttonOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_buttonCancel;
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.w_linkEditLocations);
			base.Controls.Add(this.w_txtBackupFile);
			base.Controls.Add(this.w_txtTempDatabase);
			base.Controls.Add(this.w_txtSQLServer);
			base.Controls.Add(this.w_listViewFiles);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_buttonOK);
			base.Controls.Add(this.w_buttonCancel);
			base.Controls.Add(this.w_lblFilesToCreate);
			base.Controls.Add(this.w_lblDatabaseRestored);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BackupFileCreationDialog";
			base.ShowInTaskbar = false;
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void InitializeFileLocations()
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.CreateSqlConnection().ConnectionString))
			{
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(new SqlCommand("declare @SmoDefaultFile nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'DefaultData', @SmoDefaultFile OUTPUT\ndeclare @SmoDefaultLog nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'DefaultLog', @SmoDefaultLog OUTPUT\ndeclare @SmoSetupPath nvarchar(512)\nexec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\Setup', N'SQLPath', @SmoSetupPath OUTPUT\nSELECT @SmoDefaultFile AS [DefaultFile], @SmoDefaultLog  AS [DefaultLog], @SmoSetupPath AS [SetupPath]\n", sqlConnection));
				DataTable dataTable = new DataTable("FileLocations");
				sqlDataAdapter.Fill(dataTable);
				if (dataTable.Rows.Count == 0 && FlatXtraMessageBox.Show("Unable to determine the default location for SQL file creation.\n You will need to specify locations for the data file and log file to be created before continuing.", "Could not determine default location", MessageBoxButtons.OK, MessageBoxIcon.Asterisk) != System.Windows.Forms.DialogResult.OK)
				{
					throw new ArgumentException("Could not restore backup: Could not determine default locaiton for SQL file location. ");
				}
				this.m_sDataFileLocation = dataTable.Rows[0]["DefaultFile"].ToString();
				this.m_sLogFileLocation = dataTable.Rows[0]["DefaultLog"].ToString();
				string str = string.Concat(dataTable.Rows[0]["SetupPath"].ToString(), "\\DATA");
				if (this.m_sDataFileLocation == null || this.m_sDataFileLocation == string.Empty)
				{
					this.m_sDataFileLocation = str;
				}
				if (this.m_sLogFileLocation == null || this.m_sLogFileLocation == string.Empty)
				{
					this.m_sLogFileLocation = str;
				}
			}
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process process = new Process();
			process.StartInfo.FileName = "https://www.red-gate.com/dynamic/downloads/downloadform.aspx?download=sqlvirtualrestore";
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private void On_buttonOK_Click(object sender, EventArgs e)
		{
			if (this.w_radioButtonRedGate.Checked && this.m_sRedGateServiceStatus != "Running")
			{
				FlatXtraMessageBox.Show("SQL Virtual Restore is not running on the target database server. Please ensure SQL Virtual Restore is installed and running properly, or choose the Native SQL Restore option.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				base.DialogResult = System.Windows.Forms.DialogResult.None;
			}
		}

		private void On_linkEditLocations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string[] strArrays = new string[] { "Data Files", "Log Files" };
			string[] dataFileLocation = new string[] { this.DataFileLocation, this.LogFileLocation };
			Dictionary<string, object> strs = MLShortTextEntryDialog.ShowDialog("Edit Physical Locations for Database Files", strArrays, dataFileLocation, TextBoxSize.Large);
			if (strs != null)
			{
				char[] chrArray = new char[] { '\\' };
				this.DataFileLocation = strs["Data Files"].ToString().TrimEnd(chrArray);
				this.LogFileLocation = strs["Log Files"].ToString().TrimEnd(chrArray);
				this.UpdateUI();
			}
		}

		private void On_linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process process = new Process();
			process.StartInfo.FileName = "http://www.red-gate.com/products/SQL_Virtual_Restore/";
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private void On_radioButtonNativeSQL_CheckedChanged(object sender, EventArgs e)
		{
			this.m_bUseRedGate = !this.w_radioButtonNativeSQL.Checked;
			this.UpdateUI();
		}

		private void UpdateServiceStatus(object oStatus)
		{
			string str = null;
			if (oStatus != null)
			{
				str = oStatus.ToString();
			}
			else
			{
				string mSSqlServer = this.m_sSqlServer;
				if (mSSqlServer.IndexOf("\\") >= 0)
				{
					mSSqlServer = mSSqlServer.Substring(0, mSSqlServer.IndexOf("\\"));
				}
				try
				{
					ServiceController serviceController = new ServiceController("HyperBacSrv", mSSqlServer);
					str = serviceController.Status.ToString();
				}
				catch
				{
					str = "Not Installed";
				}
			}
			if (!base.InvokeRequired)
			{
				this.m_sRedGateServiceStatus = str;
				this.w_lblRedGateState.Text = string.Concat("[", this.m_sRedGateServiceStatus, "]");
				return;
			}
			BackupFileCreationDialog.UpdateServiceStatusDelegate updateServiceStatusDelegate = new BackupFileCreationDialog.UpdateServiceStatusDelegate(this.UpdateServiceStatus);
			object[] objArray = new object[] { str };
			base.BeginInvoke(updateServiceStatusDelegate, objArray);
		}

		private void UpdateUI()
		{
			long? nullable;
			long? nullable1;
			string str;
			this.w_txtBackupFile.Text = this.m_sDatabaseName;
			this.w_txtTempDatabase.Text = this.m_sNewDatabaseName;
			this.w_txtSQLServer.Text = this.m_sSqlServer;
			long? nullable2 = new long?((long)0);
			this.w_listViewFiles.Items.Clear();
			foreach (FileDescription mFileList in this.m_FileList)
			{
				ListViewItem listViewItem = new ListViewItem()
				{
					Text = mFileList.LogicalFilename
				};
				listViewItem.SubItems.Add(Format.FormatSize(mFileList.PhysicalFileSize));
				listViewItem.SubItems.Add(DatabaseRestorationManager.GetPhysicalFileName(mFileList.FileType, this.LogFileLocation, this.DataFileLocation, this.m_sNewDatabaseName, mFileList.LogicalFilename, this.m_bUseRedGate));
				this.w_listViewFiles.Items.Add(listViewItem);
				long? nullable3 = nullable2;
				long? physicalFileSize = mFileList.PhysicalFileSize;
				if (nullable3.HasValue & physicalFileSize.HasValue)
				{
					nullable = new long?(nullable3.GetValueOrDefault() + physicalFileSize.GetValueOrDefault());
				}
				else
				{
					nullable = null;
				}
				nullable2 = nullable;
			}
			RadioButton wRadioButtonNativeSQL = this.w_radioButtonNativeSQL;
			long? nullable4 = nullable2;
			wRadioButtonNativeSQL.Text = string.Concat("Use Native SQL Server Restore", ((nullable4.GetValueOrDefault() <= (long)0 ? false : nullable4.HasValue) ? string.Concat(" (Requires approx: ", Format.FormatSize(nullable2), " free space)") : ""));
			RadioButton wRadioButtonRedGate = this.w_radioButtonRedGate;
			long? nullable5 = nullable2;
			if ((nullable5.GetValueOrDefault() <= (long)0 ? false : nullable5.HasValue))
			{
				long? nullable6 = nullable2;
				if (nullable6.HasValue)
				{
					nullable1 = new long?(nullable6.GetValueOrDefault() / (long)100);
				}
				else
				{
					nullable1 = null;
				}
				str = string.Concat(" (Requires approx: ", Format.FormatSize(nullable1), " free space)");
			}
			else
			{
				str = "";
			}
			wRadioButtonRedGate.Text = string.Concat("Use Red-Gate SQL Virtual Restore", str);
			(new Thread(new ParameterizedThreadStart(this.UpdateServiceStatus))).Start(null);
		}

		private delegate void UpdateServiceStatusDelegate(string sStatus);
	}
}