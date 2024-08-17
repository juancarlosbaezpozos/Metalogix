using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Metabase;
using Metalogix.Metabase.Adapters;
using Metalogix.Metabase.Options;
using Metalogix.UI.WinForms.Database;

namespace Metalogix.UI.WinForms.Metabase
{
    public class MetabaseSettingsDialog : XtraForm
    {
        private MetabaseSettingsOptions m_options;

        private IContainer components;

        private SimpleButton buttonCancel;

        private SimpleButton buttonOK;

        private LabelControl label1;

        private GroupControl groupBox1;

        private SimpleButton w_buttonSqlCe;

        private MemoEdit w_richTextBoxSqlServer;

        private TextEdit w_textBoxSqlCe;

        private CheckEdit w_radioButtonSQLServer;

        private CheckEdit w_radioButtonSqlCE;

        private SimpleButton w_buttonSqlServer;

        private CheckEdit w_radioButtonUseDefault;

        public MetabaseSettingsOptions Options
        {
            get
		{
			if (m_options != null)
			{
				if (w_radioButtonUseDefault.Checked)
				{
					m_options.UseDefault = true;
					return m_options;
				}
				m_options.UseDefault = false;
				if (w_radioButtonSqlCE.Checked)
				{
					m_options.MetabaseType = DatabaseAdapterType.SqlCe.ToString();
					m_options.MetabaseContext = w_textBoxSqlCe.Text;
				}
				if (w_radioButtonSQLServer.Checked)
				{
					m_options.MetabaseType = DatabaseAdapterType.SqlServer.ToString();
					m_options.MetabaseContext = GetSqlServerContext();
				}
			}
			return m_options;
		}
            set
		{
			m_options = value;
			if (m_options != null && m_options.UseDefault)
			{
				w_radioButtonUseDefault.Checked = true;
				if (ConfigurationVariables.DefaultMetabaseAdapter == DatabaseAdapterType.SqlCe.ToString())
				{
					w_textBoxSqlCe.Text = (ConfigurationVariables.AutoProvisionNewMetabaseFile ? Path.Combine(Path.GetDirectoryName(ConfigurationVariables.FileMetabaseContext), string.Concat(Guid.NewGuid(), ".sdf")) : ConfigurationVariables.FileMetabaseContext);
				}
				if (ConfigurationVariables.DefaultMetabaseAdapter == DatabaseAdapterType.SqlServer.ToString())
				{
					SetSqlServerContext(ConfigurationVariables.SQLMetabaseContext);
				}
			}
			else if (m_options != null)
			{
				w_radioButtonUseDefault.Checked = false;
				w_radioButtonSqlCE.Checked = m_options.MetabaseType == DatabaseAdapterType.SqlCe.ToString();
				w_radioButtonSQLServer.Checked = m_options.MetabaseType == DatabaseAdapterType.SqlServer.ToString();
				if (w_radioButtonSqlCE.Checked)
				{
					w_textBoxSqlCe.Text = m_options.MetabaseContext;
				}
				if (w_radioButtonSQLServer.Checked)
				{
					SetSqlServerContext(m_options.MetabaseContext);
				}
			}
		}
        }

        private string SqlServerText { get; set; }

        public MetabaseSettingsDialog()
	{
		InitializeComponent();
	}

        private void buttonOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (w_radioButtonSqlCE.Checked && string.IsNullOrEmpty(Path.GetFileName(w_textBoxSqlCe.Text)))
			{
				throw new FileNotFoundException("Invalid Sql Compact Database file specified.");
			}
			if (w_radioButtonSQLServer.Checked && string.IsNullOrEmpty(GetSqlServerContext()))
			{
				throw new Exception("Sql Server connection string cannot be blank.");
			}
		}
		catch (Exception exception)
		{
			FlatXtraMessageBox.Show(exception.Message);
			base.DialogResult = DialogResult.None;
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

        private string GetSqlServerContext()
	{
		return SqlServerText;
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Metabase.MetabaseSettingsDialog));
		this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.label1 = new DevExpress.XtraEditors.LabelControl();
		this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
		this.w_radioButtonUseDefault = new DevExpress.XtraEditors.CheckEdit();
		this.w_buttonSqlServer = new DevExpress.XtraEditors.SimpleButton();
		this.w_buttonSqlCe = new DevExpress.XtraEditors.SimpleButton();
		this.w_richTextBoxSqlServer = new DevExpress.XtraEditors.MemoEdit();
		this.w_textBoxSqlCe = new DevExpress.XtraEditors.TextEdit();
		this.w_radioButtonSQLServer = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioButtonSqlCE = new DevExpress.XtraEditors.CheckEdit();
		((System.ComponentModel.ISupportInitialize)this.groupBox1).BeginInit();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonUseDefault.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_richTextBoxSqlServer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxSqlCe.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonSQLServer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonSqlCE.Properties).BeginInit();
		base.SuspendLayout();
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(388, 275);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 3;
		this.buttonCancel.Text = "Cancel";
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(307, 275);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 2;
		this.buttonOK.Text = "OK";
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(451, 39);
		this.label1.TabIndex = 0;
		this.label1.Text = "Metalogix Metabase provides support for SQL Server Compact and SQL Server databases.  NOTE: It is highly recommended that large migrations use a full-featured SQL Server database.";
		this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox1.Controls.Add(this.w_radioButtonUseDefault);
		this.groupBox1.Controls.Add(this.w_buttonSqlServer);
		this.groupBox1.Controls.Add(this.w_buttonSqlCe);
		this.groupBox1.Controls.Add(this.w_richTextBoxSqlServer);
		this.groupBox1.Controls.Add(this.w_textBoxSqlCe);
		this.groupBox1.Controls.Add(this.w_radioButtonSQLServer);
		this.groupBox1.Controls.Add(this.w_radioButtonSqlCE);
		this.groupBox1.Location = new System.Drawing.Point(12, 54);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(451, 209);
		this.groupBox1.TabIndex = 1;
		this.groupBox1.Text = "Metabase Storage Configuration";
		this.w_radioButtonUseDefault.EditValue = true;
		this.w_radioButtonUseDefault.Location = new System.Drawing.Point(23, 28);
		this.w_radioButtonUseDefault.Name = "w_radioButtonUseDefault";
		this.w_radioButtonUseDefault.Properties.Caption = "Use default database";
		this.w_radioButtonUseDefault.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonUseDefault.Properties.RadioGroupIndex = 1;
		this.w_radioButtonUseDefault.Size = new System.Drawing.Size(126, 19);
		this.w_radioButtonUseDefault.TabIndex = 0;
		this.w_buttonSqlServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_buttonSqlServer.Enabled = false;
		this.w_buttonSqlServer.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_buttonSqlServer.Image");
		this.w_buttonSqlServer.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.w_buttonSqlServer.Location = new System.Drawing.Point(408, 123);
		this.w_buttonSqlServer.Name = "w_buttonSqlServer";
		this.w_buttonSqlServer.Size = new System.Drawing.Size(25, 25);
		this.w_buttonSqlServer.TabIndex = 6;
		this.w_buttonSqlServer.Click += new System.EventHandler(w_buttonSqlServer_Click);
		this.w_buttonSqlCe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_buttonSqlCe.Enabled = false;
		this.w_buttonSqlCe.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_buttonSqlCe.Image");
		this.w_buttonSqlCe.Location = new System.Drawing.Point(408, 71);
		this.w_buttonSqlCe.Name = "w_buttonSqlCe";
		this.w_buttonSqlCe.Size = new System.Drawing.Size(25, 25);
		this.w_buttonSqlCe.TabIndex = 3;
		this.w_buttonSqlCe.Click += new System.EventHandler(w_buttonSqlCe_Click);
		this.w_richTextBoxSqlServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_richTextBoxSqlServer.EditValue = "";
		this.w_richTextBoxSqlServer.Enabled = false;
		this.w_richTextBoxSqlServer.Location = new System.Drawing.Point(44, 123);
		this.w_richTextBoxSqlServer.Name = "w_richTextBoxSqlServer";
		this.w_richTextBoxSqlServer.Properties.ReadOnly = true;
		this.w_richTextBoxSqlServer.Size = new System.Drawing.Size(358, 69);
		this.w_richTextBoxSqlServer.TabIndex = 5;
		this.w_textBoxSqlCe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_textBoxSqlCe.Enabled = false;
		this.w_textBoxSqlCe.Location = new System.Drawing.Point(42, 74);
		this.w_textBoxSqlCe.Name = "w_textBoxSqlCe";
		this.w_textBoxSqlCe.Properties.ReadOnly = true;
		this.w_textBoxSqlCe.Size = new System.Drawing.Size(360, 20);
		this.w_textBoxSqlCe.TabIndex = 2;
		this.w_radioButtonSQLServer.Location = new System.Drawing.Point(23, 100);
		this.w_radioButtonSQLServer.Name = "w_radioButtonSQLServer";
		this.w_radioButtonSQLServer.Properties.Caption = "Use SQL Server database:";
		this.w_radioButtonSQLServer.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonSQLServer.Properties.RadioGroupIndex = 1;
		this.w_radioButtonSQLServer.Size = new System.Drawing.Size(152, 19);
		this.w_radioButtonSQLServer.TabIndex = 4;
		this.w_radioButtonSQLServer.TabStop = false;
		this.w_radioButtonSQLServer.CheckedChanged += new System.EventHandler(w_radioButtonSQLServer_CheckedChanged);
		this.w_radioButtonSqlCE.Location = new System.Drawing.Point(23, 51);
		this.w_radioButtonSqlCE.Name = "w_radioButtonSqlCE";
		this.w_radioButtonSqlCE.Properties.Caption = "Use SQL Server Compact database:";
		this.w_radioButtonSqlCE.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonSqlCE.Properties.RadioGroupIndex = 1;
		this.w_radioButtonSqlCE.Size = new System.Drawing.Size(197, 19);
		this.w_radioButtonSqlCE.TabIndex = 1;
		this.w_radioButtonSqlCE.TabStop = false;
		this.w_radioButtonSqlCE.CheckedChanged += new System.EventHandler(w_radioButtonSqlCE_CheckedChanged);
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(475, 310);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(483, 337);
		base.Name = "MetabaseSettingsDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Change Metabase Storage";
		((System.ComponentModel.ISupportInitialize)this.groupBox1).EndInit();
		this.groupBox1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonUseDefault.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_richTextBoxSqlServer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxSqlCe.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonSQLServer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonSqlCE.Properties).EndInit();
		base.ResumeLayout(false);
	}

        private void SetSqlServerContext(string value)
	{
		SqlServerText = value;
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(SqlServerText);
		w_richTextBoxSqlServer.Text = ((!string.IsNullOrEmpty(sqlConnectionStringBuilder.Password)) ? sqlConnectionStringBuilder.ConnectionString.Replace(sqlConnectionStringBuilder.Password, "********") : sqlConnectionStringBuilder.ConnectionString);
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
		if (!string.IsNullOrEmpty(w_textBoxSqlCe.Text))
		{
			saveFileDialog.InitialDirectory = Path.GetDirectoryName(w_textBoxSqlCe.Text);
			saveFileDialog.FileName = Path.GetFileName(w_textBoxSqlCe.Text);
		}
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			w_textBoxSqlCe.Text = saveFileDialog.FileName;
		}
	}

        private void w_buttonSqlServer_Click(object sender, EventArgs e)
	{
		DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog
		{
			AllowBrowsingNetworkServers = true,
			AllowBrowsingDatabases = true,
			AllowDatabaseCreation = true,
			AllowDatabaseDeletion = true,
			EnableRememberMe = false,
			RememberMe = true
		};
		databaseConnectDialog.SetConnectionString(GetSqlServerContext());
		if (databaseConnectDialog.ShowDialog() == DialogResult.OK)
		{
			SetSqlServerContext(databaseConnectDialog.GetConnectionString());
		}
	}

        private void w_radioButtonSqlCE_CheckedChanged(object sender, EventArgs e)
	{
		w_buttonSqlCe.Enabled = w_radioButtonSqlCE.Checked;
		w_textBoxSqlCe.Enabled = w_radioButtonSqlCE.Checked;
	}

        private void w_radioButtonSQLServer_CheckedChanged(object sender, EventArgs e)
	{
		w_buttonSqlServer.Enabled = w_radioButtonSQLServer.Checked;
		w_richTextBoxSqlServer.Enabled = w_radioButtonSQLServer.Checked;
	}
    }
}
