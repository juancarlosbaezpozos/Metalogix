using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Metabase;
using Metalogix.UI.WinForms.Database;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Metabase
{
    public class MetabaseDefaultConfiguration : XtraForm
    {
        private IContainer components;

        private LabelControl label1;

        private GroupControl w_gbSettings;

        private SimpleButton w_btnSqlServer;

        private SimpleButton w_btnAutoSqlCe;

        private MemoEdit w_rtbSqlServer;

        private TextEdit w_tbAutoSqlCe;

        private CheckEdit w_rbSQLServer;

        private CheckEdit w_rbAutoSqlCe;

        private SimpleButton buttonOK;

        private SimpleButton buttonCancel;

        private SimpleButton w_btnSqlCE;

        private TextEdit w_tbSqlCe;

        private CheckEdit w_rbSqlCe;

        private string SqlServerText { get; set; }

        public MetabaseDefaultConfiguration()
	{
		InitializeComponent();
		LoadUI();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private string GetSqlServerContext()
	{
		return SqlServerText;
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Metabase.MetabaseDefaultConfiguration));
		this.label1 = new DevExpress.XtraEditors.LabelControl();
		this.w_gbSettings = new DevExpress.XtraEditors.GroupControl();
		this.w_btnSqlCE = new DevExpress.XtraEditors.SimpleButton();
		this.w_tbSqlCe = new DevExpress.XtraEditors.TextEdit();
		this.w_rbSqlCe = new DevExpress.XtraEditors.CheckEdit();
		this.w_btnSqlServer = new DevExpress.XtraEditors.SimpleButton();
		this.w_btnAutoSqlCe = new DevExpress.XtraEditors.SimpleButton();
		this.w_rtbSqlServer = new DevExpress.XtraEditors.MemoEdit();
		this.w_tbAutoSqlCe = new DevExpress.XtraEditors.TextEdit();
		this.w_rbSQLServer = new DevExpress.XtraEditors.CheckEdit();
		this.w_rbAutoSqlCe = new DevExpress.XtraEditors.CheckEdit();
		this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.w_gbSettings).BeginInit();
		this.w_gbSettings.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_tbSqlCe.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbSqlCe.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_rtbSqlServer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_tbAutoSqlCe.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbSQLServer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbAutoSqlCe.Properties).BeginInit();
		base.SuspendLayout();
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(451, 39);
		this.label1.TabIndex = 0;
		this.label1.Text = "Metalogix Metabase provides support for SQL Server Compact and SQL Server databases.  NOTE: It is highly recommended that large migrations use a full-featured SQL Server database.";
		this.w_gbSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_gbSettings.Controls.Add(this.w_btnSqlCE);
		this.w_gbSettings.Controls.Add(this.w_tbSqlCe);
		this.w_gbSettings.Controls.Add(this.w_rbSqlCe);
		this.w_gbSettings.Controls.Add(this.w_btnSqlServer);
		this.w_gbSettings.Controls.Add(this.w_btnAutoSqlCe);
		this.w_gbSettings.Controls.Add(this.w_rtbSqlServer);
		this.w_gbSettings.Controls.Add(this.w_tbAutoSqlCe);
		this.w_gbSettings.Controls.Add(this.w_rbSQLServer);
		this.w_gbSettings.Controls.Add(this.w_rbAutoSqlCe);
		this.w_gbSettings.Location = new System.Drawing.Point(12, 60);
		this.w_gbSettings.Name = "w_gbSettings";
		this.w_gbSettings.Size = new System.Drawing.Size(451, 248);
		this.w_gbSettings.TabIndex = 1;
		this.w_gbSettings.Text = "Metabase Storage Configuration";
		this.w_btnSqlCE.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_btnSqlCE.Enabled = false;
		this.w_btnSqlCE.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_btnSqlCE.Image");
		this.w_btnSqlCE.Location = new System.Drawing.Point(417, 102);
		this.w_btnSqlCE.Name = "w_btnSqlCE";
		this.w_btnSqlCE.Size = new System.Drawing.Size(25, 25);
		this.w_btnSqlCE.TabIndex = 6;
		this.w_btnSqlCE.Click += new System.EventHandler(w_buttonSqlCe_Click);
		this.w_tbSqlCe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_tbSqlCe.Enabled = false;
		this.w_tbSqlCe.Location = new System.Drawing.Point(42, 105);
		this.w_tbSqlCe.Name = "w_tbSqlCe";
		this.w_tbSqlCe.Properties.ReadOnly = true;
		this.w_tbSqlCe.Size = new System.Drawing.Size(369, 20);
		this.w_tbSqlCe.TabIndex = 5;
		this.w_rbSqlCe.Location = new System.Drawing.Point(23, 82);
		this.w_rbSqlCe.Name = "w_rbSqlCe";
		this.w_rbSqlCe.Properties.Caption = "Use SQL Server Compact database:";
		this.w_rbSqlCe.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_rbSqlCe.Properties.RadioGroupIndex = 1;
		this.w_rbSqlCe.Size = new System.Drawing.Size(197, 19);
		this.w_rbSqlCe.TabIndex = 4;
		this.w_rbSqlCe.TabStop = false;
		this.w_rbSqlCe.CheckedChanged += new System.EventHandler(On_SqlCE_CheckedChanged);
		this.w_btnSqlServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_btnSqlServer.Enabled = false;
		this.w_btnSqlServer.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_btnSqlServer.Image");
		this.w_btnSqlServer.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.w_btnSqlServer.Location = new System.Drawing.Point(417, 154);
		this.w_btnSqlServer.Name = "w_btnSqlServer";
		this.w_btnSqlServer.Size = new System.Drawing.Size(25, 25);
		this.w_btnSqlServer.TabIndex = 0;
		this.w_btnSqlServer.Click += new System.EventHandler(w_buttonSqlServer_Click);
		this.w_btnAutoSqlCe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_btnAutoSqlCe.Image = Metalogix.UI.WinForms.Properties.Resources.SqlCeFolder;
		this.w_btnAutoSqlCe.Location = new System.Drawing.Point(417, 53);
		this.w_btnAutoSqlCe.Name = "w_btnAutoSqlCe";
		this.w_btnAutoSqlCe.Size = new System.Drawing.Size(25, 25);
		this.w_btnAutoSqlCe.TabIndex = 2;
		this.w_btnAutoSqlCe.Click += new System.EventHandler(w_buttonAutoSqlCe_Click);
		this.w_rtbSqlServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_rtbSqlServer.EditValue = "";
		this.w_rtbSqlServer.Enabled = false;
		this.w_rtbSqlServer.Location = new System.Drawing.Point(42, 154);
		this.w_rtbSqlServer.Name = "w_rtbSqlServer";
		this.w_rtbSqlServer.Properties.ReadOnly = true;
		this.w_rtbSqlServer.Size = new System.Drawing.Size(369, 80);
		this.w_rtbSqlServer.TabIndex = 8;
		this.w_tbAutoSqlCe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_tbAutoSqlCe.Location = new System.Drawing.Point(42, 56);
		this.w_tbAutoSqlCe.Name = "w_tbAutoSqlCe";
		this.w_tbAutoSqlCe.Properties.ReadOnly = true;
		this.w_tbAutoSqlCe.Size = new System.Drawing.Size(369, 20);
		this.w_tbAutoSqlCe.TabIndex = 1;
		this.w_rbSQLServer.Location = new System.Drawing.Point(23, 131);
		this.w_rbSQLServer.Name = "w_rbSQLServer";
		this.w_rbSQLServer.Properties.Caption = "Use SQL Server database:";
		this.w_rbSQLServer.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_rbSQLServer.Properties.RadioGroupIndex = 1;
		this.w_rbSQLServer.Size = new System.Drawing.Size(152, 19);
		this.w_rbSQLServer.TabIndex = 7;
		this.w_rbSQLServer.TabStop = false;
		this.w_rbSQLServer.CheckedChanged += new System.EventHandler(On_SqlServer_CheckedChanged);
		this.w_rbAutoSqlCe.EditValue = true;
		this.w_rbAutoSqlCe.Location = new System.Drawing.Point(23, 33);
		this.w_rbAutoSqlCe.Name = "w_rbAutoSqlCe";
		this.w_rbAutoSqlCe.Properties.Caption = "Provision New SQL Server Compact database:";
		this.w_rbAutoSqlCe.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_rbAutoSqlCe.Properties.RadioGroupIndex = 1;
		this.w_rbAutoSqlCe.Size = new System.Drawing.Size(246, 19);
		this.w_rbAutoSqlCe.TabIndex = 0;
		this.w_rbAutoSqlCe.CheckedChanged += new System.EventHandler(On_AutoSqlCe_CheckedChanged);
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(307, 314);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 2;
		this.buttonOK.Text = "OK";
		this.buttonOK.Click += new System.EventHandler(On_OK_Clicked);
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(388, 314);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 3;
		this.buttonCancel.Text = "Cancel";
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(475, 349);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.w_gbSettings);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(483, 376);
		base.Name = "MetabaseDefaultConfiguration";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Change Metabase Default Settings";
		((System.ComponentModel.ISupportInitialize)this.w_gbSettings).EndInit();
		this.w_gbSettings.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.w_tbSqlCe.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbSqlCe.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_rtbSqlServer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_tbAutoSqlCe.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbSQLServer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_rbAutoSqlCe.Properties).EndInit();
		base.ResumeLayout(false);
	}

        private void LoadUI()
	{
		string fileMetabaseContext = ConfigurationVariables.FileMetabaseContext;
		string sQLMetabaseContext = ConfigurationVariables.SQLMetabaseContext;
		string defaultMetabaseAdapter = ConfigurationVariables.DefaultMetabaseAdapter;
		string str = defaultMetabaseAdapter;
		if (defaultMetabaseAdapter != null)
		{
			if (str == "SqlServer")
			{
				w_rbSQLServer.Checked = true;
				SetSqlServerContext(sQLMetabaseContext);
				return;
			}
			if (str == "SqlCe")
			{
				if (!ConfigurationVariables.AutoProvisionNewMetabaseFile)
				{
					w_rbSqlCe.Checked = true;
					w_tbAutoSqlCe.Text = Path.GetDirectoryName(fileMetabaseContext);
					w_tbSqlCe.Text = fileMetabaseContext;
				}
				else
				{
					w_rbAutoSqlCe.Checked = true;
					w_tbAutoSqlCe.Text = fileMetabaseContext;
					TextEdit wTbSqlCe = w_tbSqlCe;
					wTbSqlCe.Text = Path.Combine(fileMetabaseContext, Guid.NewGuid().ToString() + ".sdf");
				}
				return;
			}
		}
		w_rbAutoSqlCe.Checked = true;
		w_tbAutoSqlCe.Text = ApplicationData.ApplicationPath;
		CheckEdit wRbSqlCe = w_rbSqlCe;
		string applicationPath = ApplicationData.ApplicationPath;
		wRbSqlCe.Text = Path.Combine(applicationPath, default(Guid).ToString() + ".sdf");
	}

        private void On_AutoSqlCe_CheckedChanged(object sender, EventArgs e)
	{
		w_btnAutoSqlCe.Enabled = w_rbAutoSqlCe.Checked;
		w_tbAutoSqlCe.Enabled = w_rbAutoSqlCe.Checked;
	}

        private void On_OK_Clicked(object sender, EventArgs e)
	{
		if (!SaveUI())
		{
			base.DialogResult = DialogResult.None;
		}
	}

        private void On_SqlCE_CheckedChanged(object sender, EventArgs e)
	{
		w_btnSqlCE.Enabled = w_rbSqlCe.Checked;
		w_tbSqlCe.Enabled = w_rbSqlCe.Checked;
	}

        private void On_SqlServer_CheckedChanged(object sender, EventArgs e)
	{
		w_btnSqlServer.Enabled = w_rbSQLServer.Checked;
		w_rtbSqlServer.Enabled = w_rbSQLServer.Checked;
	}

        private bool SaveUI()
	{
		if (w_rbSqlCe.Checked && string.IsNullOrEmpty(w_tbSqlCe.Text))
		{
			FlatXtraMessageBox.Show("Invalid Sql Compact Database file specified.", "Invalid Settings", MessageBoxButtons.OK);
			return false;
		}
		if (w_rbAutoSqlCe.Checked && string.IsNullOrEmpty(w_tbAutoSqlCe.Text))
		{
			FlatXtraMessageBox.Show("Invalid folder specified.", "Invalid Settings", MessageBoxButtons.OK);
			return false;
		}
		if (w_rbSQLServer.Checked && string.IsNullOrEmpty(GetSqlServerContext()))
		{
			FlatXtraMessageBox.Show("Sql Server connection string cannot be blank.", "Invalid Settings", MessageBoxButtons.OK);
			return false;
		}
		ConfigurationVariables.DefaultMetabaseAdapter = (w_rbSQLServer.Checked ? "SqlServer" : "SqlCe");
		ConfigurationVariables.AutoProvisionNewMetabaseFile = !w_rbSqlCe.Checked;
		ConfigurationVariables.FileMetabaseContext = (w_rbSqlCe.Checked ? w_tbSqlCe.Text : w_tbAutoSqlCe.Text);
		ConfigurationVariables.SQLMetabaseContext = GetSqlServerContext();
		return true;
	}

        private void SetSqlServerContext(string value)
	{
		SqlServerText = value;
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(SqlServerText);
		w_rtbSqlServer.Text = ((!string.IsNullOrEmpty(sqlConnectionStringBuilder.Password)) ? sqlConnectionStringBuilder.ConnectionString.Replace(sqlConnectionStringBuilder.Password, "********") : sqlConnectionStringBuilder.ConnectionString);
	}

        private void w_buttonAutoSqlCe_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
		{
			ShowNewFolderButton = true
		};
		if (!string.IsNullOrEmpty(w_tbAutoSqlCe.Text))
		{
			folderBrowserDialog.SelectedPath = Path.GetDirectoryName(w_tbAutoSqlCe.Text);
		}
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			w_tbAutoSqlCe.Text = folderBrowserDialog.SelectedPath;
		}
	}

        private void w_buttonSqlCe_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Title = "Create or Open Metabase Sql Server Compact database...",
			AddExtension = true,
			DefaultExt = ".sdf",
			Filter = "Sql Server Compact database (*.sdf) | *.sdf",
			CheckFileExists = false,
			OverwritePrompt = false
		};
		if (!string.IsNullOrEmpty(w_tbAutoSqlCe.Text))
		{
			saveFileDialog.InitialDirectory = Path.GetDirectoryName(w_tbAutoSqlCe.Text);
		}
		if (!string.IsNullOrEmpty(w_tbAutoSqlCe.Text))
		{
			saveFileDialog.FileName = Path.GetFileName(w_tbAutoSqlCe.Text);
		}
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			w_tbSqlCe.Text = saveFileDialog.FileName;
		}
	}

        private void w_buttonSqlServer_Click(object sender, EventArgs e)
	{
		DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog
		{
			AllowDatabaseCreation = true,
			AllowDatabaseDeletion = true,
			AllowBrowsingNetworkServers = true,
			AllowBrowsingDatabases = true,
			EnableRememberMe = false,
			RememberMe = true
		};
		databaseConnectDialog.SetConnectionString(GetSqlServerContext());
		if (databaseConnectDialog.ShowDialog() == DialogResult.OK)
		{
			SetSqlServerContext(databaseConnectDialog.GetConnectionString());
		}
	}
    }
}
