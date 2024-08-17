using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.Database;
using Metalogix.Permissions;
using Metalogix.UI.WinForms.Actions;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.Database
{
    public class DatabaseConnectDialog : XtraForm
    {
        private bool m_showDatabaseBrowser = true;

        private ArrayList m_alServerHistory;

        private bool m_bAllowBrowsingNetworkServers = true;

        private bool m_enableRememberMe = true;

        private ArrayList m_alSqlLocalServers = new ArrayList();

        private ArrayList m_alSqlNetworkServers = new ArrayList();

        private System.Action<SqlConnectionStringBuilder> m_checkServerConnection;

        private System.Action<SqlConnectionStringBuilder> m_checkDatabaseConnection;

        private IContainer components;

        private ComboBoxEdit w_comboboxServerList;

        private LabelControl w_labelSiteAddress;

        private SimpleButton w_buttonConnect;

        private SimpleButton w_buttonCancel;

        private SimpleButton w_buttonBrowse;

        private GroupControl w_groupAuthentication;

        private CheckEdit w_radioDifferentAuth;

        private CheckEdit w_radioCurrentAuth;

        private LabelControl w_labelPassword;

        private TextEdit w_maskedtextboxPassword;

        private TextEdit w_textboxUsername;

        private CheckEdit w_cbRememberMe;

        private LabelControl w_labelDatabase;

        private SimpleButton w_buttonBrowseDatabase;

        private TextEdit w_textBoxDatabase;

        public bool AllowBrowsingDatabases
        {
            get
            {
                return m_showDatabaseBrowser;
            }
            set
            {
                m_showDatabaseBrowser = value;
                if (!m_showDatabaseBrowser)
                {
                    w_labelDatabase.Visible = false;
                    w_textBoxDatabase.Visible = false;
                    w_buttonBrowseDatabase.Visible = false;
                    base.Height = 262;
                }
            }
        }

        public bool AllowBrowsingNetworkServers
        {
            get
            {
                return m_bAllowBrowsingNetworkServers;
            }
            set
            {
                m_bAllowBrowsingNetworkServers = value;
                w_buttonBrowse.Visible = AllowBrowsingNetworkServers;
                w_comboboxServerList.Properties.TextEditStyle = ((!AllowBrowsingNetworkServers) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
            }
        }

        public bool AllowDatabaseCreation { get; set; }

        public bool AllowDatabaseDeletion { get; set; }

        public Credentials Credentials
        {
            get
            {
                if (w_radioCurrentAuth.Checked)
                {
                    return new Credentials();
                }
                return new Credentials(w_textboxUsername.Text, w_maskedtextboxPassword.Text.ToSecureString(), w_cbRememberMe.Checked);
            }
            set
            {
                if (value != null)
                {
                    w_radioDifferentAuth.Checked = !value.IsDefault;
                    w_textboxUsername.Text = value.UserName;
                    w_maskedtextboxPassword.Text = value.Password.ToInsecureString();
                    w_cbRememberMe.Checked = value.SavePassword;
                }
            }
        }

        public bool EnableLocationEditing
        {
            get
            {
                return w_comboboxServerList.Enabled;
            }
            set
            {
                w_comboboxServerList.Enabled = value;
                w_buttonBrowse.Enabled = value;
            }
        }

        public bool EnableRememberMe
        {
            get
            {
                return m_enableRememberMe;
            }
            set
            {
                m_enableRememberMe = value;
                w_cbRememberMe.Enabled = m_enableRememberMe;
            }
        }

        public bool RememberMe
        {
            get
            {
                return w_cbRememberMe.Checked;
            }
            set
            {
                w_cbRememberMe.Checked = value;
            }
        }

        public ArrayList ServerHistory
        {
            get
            {
                return m_alServerHistory;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    return;
                }
                m_alServerHistory = value;
                w_comboboxServerList.Properties.Items.Clear();
                foreach (string str in value)
                {
                    w_comboboxServerList.Properties.Items.Add(str);
                }
            }
        }

        public string SqlDatabaseName
        {
            get
            {
                return w_textBoxDatabase.Text;
            }
            set
            {
                w_textBoxDatabase.Text = value;
                if (value != null)
                {
                    w_textBoxDatabase.Enabled = false;
                    w_buttonBrowseDatabase.Enabled = false;
                }
            }
        }

        public ArrayList SqlLocalServers
        {
            get
            {
                return m_alSqlLocalServers;
            }
            set
            {
                m_alSqlLocalServers = value;
            }
        }

        public ArrayList SqlNetworkServers
        {
            get
            {
                return m_alSqlNetworkServers;
            }
            set
            {
                m_alSqlNetworkServers = value;
            }
        }

        public bool SqlServerIsLocal => DatabaseBrowser.GetLocalSQLServers().Contains(SqlServerName);

        public string SqlServerName
        {
            get
            {
                return w_comboboxServerList.Text;
            }
            set
            {
                w_comboboxServerList.Text = value;
                if (value != null)
                {
                    w_comboboxServerList.Enabled = false;
                    w_buttonBrowse.Enabled = false;
                }
            }
        }

        public IEnumerable VisitedServers
        {
            get
            {
                return w_comboboxServerList.Properties.Items;
            }
            set
            {
                w_comboboxServerList.Properties.Items.Clear();
                if (value == null)
                {
                    return;
                }
                foreach (object obj in value)
                {
                    w_comboboxServerList.Properties.Items.Add(obj);
                }
                if (string.IsNullOrEmpty(w_comboboxServerList.Text) && w_comboboxServerList.Properties.Items.Count > 0)
                {
                    w_comboboxServerList.SelectedIndex = 0;
                }
            }
        }

        public DatabaseConnectDialog()
        {
            InitializeComponent();
            w_radioCurrentAuth.Text = "Use Current Windows User: " + WindowsIdentity.GetCurrent().Name;
        }

        private void DatabaseConnectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (base.DialogResult == DialogResult.OK)
                {
                    if (DatabaseSettings.VisitedSQLServers.Contains(SqlServerName))
                    {
                        DatabaseSettings.VisitedSQLServers.AddToFront(SqlServerName.ToUpper());
                        return;
                    }
                    DatabaseSettings.VisitedSQLServers.AddToFront(SqlServerName.ToUpper());
                    DatabaseSettings.SaveVisitedSQLServers();
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void DatabaseConnectDialog_Load(object sender, EventArgs e)
        {
            try
            {
                VisitedServers = DatabaseSettings.VisitedSQLServers;
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
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

        public System.Action<SqlConnectionStringBuilder> GetCheckDatabaseConnection()
        {
            if (m_checkDatabaseConnection == null)
            {
                m_checkDatabaseConnection = delegate
                {
                };
            }
            return m_checkDatabaseConnection;
        }

        public System.Action<SqlConnectionStringBuilder> GetCheckServerConnection()
        {
            if (m_checkServerConnection == null)
            {
                m_checkServerConnection = delegate
                {
                };
            }
            return m_checkServerConnection;
        }

        private SqlConnectionStringBuilder GetConnectionBuilder()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = SqlServerName
            };
            if (AllowBrowsingDatabases)
            {
                sqlConnectionStringBuilder.InitialCatalog = w_textBoxDatabase.Text;
            }
            sqlConnectionStringBuilder.IntegratedSecurity = Credentials.IsDefault;
            if (!sqlConnectionStringBuilder.IntegratedSecurity)
            {
                sqlConnectionStringBuilder.UserID = Credentials.UserName;
                sqlConnectionStringBuilder.Password = Credentials.Password.ToInsecureString();
            }
            return sqlConnectionStringBuilder;
        }

        public string GetConnectionString()
        {
            return GetConnectionBuilder().ConnectionString;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Database.DatabaseConnectDialog));
            this.w_comboboxServerList = new DevExpress.XtraEditors.ComboBoxEdit();
            this.w_labelSiteAddress = new DevExpress.XtraEditors.LabelControl();
            this.w_buttonConnect = new DevExpress.XtraEditors.SimpleButton();
            this.w_buttonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_buttonBrowse = new DevExpress.XtraEditors.SimpleButton();
            this.w_groupAuthentication = new DevExpress.XtraEditors.GroupControl();
            this.w_cbRememberMe = new DevExpress.XtraEditors.CheckEdit();
            this.w_textboxUsername = new DevExpress.XtraEditors.TextEdit();
            this.w_radioDifferentAuth = new DevExpress.XtraEditors.CheckEdit();
            this.w_radioCurrentAuth = new DevExpress.XtraEditors.CheckEdit();
            this.w_labelPassword = new DevExpress.XtraEditors.LabelControl();
            this.w_maskedtextboxPassword = new DevExpress.XtraEditors.TextEdit();
            this.w_labelDatabase = new DevExpress.XtraEditors.LabelControl();
            this.w_buttonBrowseDatabase = new DevExpress.XtraEditors.SimpleButton();
            this.w_textBoxDatabase = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)this.w_comboboxServerList.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_groupAuthentication).BeginInit();
            this.w_groupAuthentication.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_textboxUsername.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_radioDifferentAuth.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_radioCurrentAuth.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_maskedtextboxPassword.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_textBoxDatabase.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_comboboxServerList, "w_comboboxServerList");
            this.w_comboboxServerList.Name = "w_comboboxServerList";
            this.w_comboboxServerList.Properties.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_comboboxServerList.Properties.Appearance.Font");
            this.w_comboboxServerList.Properties.Appearance.Options.UseFont = true;
            this.w_comboboxServerList.TextChanged += new System.EventHandler(w_comboboxServerList_TextChanged);
            this.w_labelSiteAddress.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_labelSiteAddress.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_labelSiteAddress, "w_labelSiteAddress");
            this.w_labelSiteAddress.Name = "w_labelSiteAddress";
            componentResourceManager.ApplyResources(this.w_buttonConnect, "w_buttonConnect");
            this.w_buttonConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_buttonConnect.Name = "w_buttonConnect";
            this.w_buttonConnect.Click += new System.EventHandler(On_buttonConnect_Click);
            componentResourceManager.ApplyResources(this.w_buttonCancel, "w_buttonCancel");
            this.w_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_buttonCancel.Name = "w_buttonCancel";
            componentResourceManager.ApplyResources(this.w_buttonBrowse, "w_buttonBrowse");
            this.w_buttonBrowse.Name = "w_buttonBrowse";
            this.w_buttonBrowse.Click += new System.EventHandler(On_buttonBrowse_Click);
            componentResourceManager.ApplyResources(this.w_groupAuthentication, "w_groupAuthentication");
            this.w_groupAuthentication.Controls.Add(this.w_cbRememberMe);
            this.w_groupAuthentication.Controls.Add(this.w_textboxUsername);
            this.w_groupAuthentication.Controls.Add(this.w_radioDifferentAuth);
            this.w_groupAuthentication.Controls.Add(this.w_radioCurrentAuth);
            this.w_groupAuthentication.Controls.Add(this.w_labelPassword);
            this.w_groupAuthentication.Controls.Add(this.w_maskedtextboxPassword);
            this.w_groupAuthentication.Name = "w_groupAuthentication";
            componentResourceManager.ApplyResources(this.w_cbRememberMe, "w_cbRememberMe");
            this.w_cbRememberMe.Name = "w_cbRememberMe";
            this.w_cbRememberMe.Properties.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_cbRememberMe.Properties.Appearance.Font");
            this.w_cbRememberMe.Properties.Appearance.Options.UseFont = true;
            this.w_cbRememberMe.Properties.AutoWidth = true;
            this.w_cbRememberMe.Properties.Caption = componentResourceManager.GetString("w_cbRememberMe.Properties.Caption");
            componentResourceManager.ApplyResources(this.w_textboxUsername, "w_textboxUsername");
            this.w_textboxUsername.Name = "w_textboxUsername";
            componentResourceManager.ApplyResources(this.w_radioDifferentAuth, "w_radioDifferentAuth");
            this.w_radioDifferentAuth.Name = "w_radioDifferentAuth";
            this.w_radioDifferentAuth.Properties.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_radioDifferentAuth.Properties.Appearance.Font");
            this.w_radioDifferentAuth.Properties.Appearance.Options.UseFont = true;
            this.w_radioDifferentAuth.Properties.AutoWidth = true;
            this.w_radioDifferentAuth.Properties.Caption = componentResourceManager.GetString("w_radioDifferentAuth.Properties.Caption");
            this.w_radioDifferentAuth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioDifferentAuth.Properties.RadioGroupIndex = 1;
            this.w_radioDifferentAuth.TabStop = false;
            this.w_radioDifferentAuth.CheckedChanged += new System.EventHandler(w_radioDifferentAuth_CheckedChanged);
            componentResourceManager.ApplyResources(this.w_radioCurrentAuth, "w_radioCurrentAuth");
            this.w_radioCurrentAuth.Name = "w_radioCurrentAuth";
            this.w_radioCurrentAuth.Properties.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_radioCurrentAuth.Properties.Appearance.Font");
            this.w_radioCurrentAuth.Properties.Appearance.Options.UseFont = true;
            this.w_radioCurrentAuth.Properties.AutoWidth = true;
            this.w_radioCurrentAuth.Properties.Caption = componentResourceManager.GetString("w_radioCurrentAuth.Properties.Caption");
            this.w_radioCurrentAuth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_radioCurrentAuth.Properties.RadioGroupIndex = 1;
            this.w_labelPassword.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_labelPassword.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_labelPassword, "w_labelPassword");
            this.w_labelPassword.Name = "w_labelPassword";
            componentResourceManager.ApplyResources(this.w_maskedtextboxPassword, "w_maskedtextboxPassword");
            this.w_maskedtextboxPassword.Name = "w_maskedtextboxPassword";
            this.w_maskedtextboxPassword.Properties.PasswordChar = '*';
            this.w_labelDatabase.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_labelDatabase.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_labelDatabase, "w_labelDatabase");
            this.w_labelDatabase.Name = "w_labelDatabase";
            componentResourceManager.ApplyResources(this.w_buttonBrowseDatabase, "w_buttonBrowseDatabase");
            this.w_buttonBrowseDatabase.Name = "w_buttonBrowseDatabase";
            this.w_buttonBrowseDatabase.Click += new System.EventHandler(w_buttonBrowseDatabase_Click);
            componentResourceManager.ApplyResources(this.w_textBoxDatabase, "w_textBoxDatabase");
            this.w_textBoxDatabase.Name = "w_textBoxDatabase";
            base.AcceptButton = this.w_buttonConnect;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_buttonCancel;
            base.Controls.Add(this.w_textBoxDatabase);
            base.Controls.Add(this.w_buttonBrowseDatabase);
            base.Controls.Add(this.w_labelDatabase);
            base.Controls.Add(this.w_groupAuthentication);
            base.Controls.Add(this.w_buttonBrowse);
            base.Controls.Add(this.w_buttonCancel);
            base.Controls.Add(this.w_buttonConnect);
            base.Controls.Add(this.w_labelSiteAddress);
            base.Controls.Add(this.w_comboboxServerList);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "DatabaseConnectDialog";
            base.ShowInTaskbar = false;
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DatabaseConnectDialog_FormClosing);
            base.Load += new System.EventHandler(DatabaseConnectDialog_Load);
            ((System.ComponentModel.ISupportInitialize)this.w_comboboxServerList.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_groupAuthentication).EndInit();
            this.w_groupAuthentication.ResumeLayout(false);
            this.w_groupAuthentication.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRememberMe.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_textboxUsername.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_radioDifferentAuth.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_radioCurrentAuth.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_maskedtextboxPassword.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_textBoxDatabase.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                SqlServerBrowserDialog sqlServerBrowserDialog = new SqlServerBrowserDialog
                {
                    SqlLocalServers = SqlLocalServers,
                    SqlNetworkServers = SqlNetworkServers,
                    ShowNetworkServersTab = AllowBrowsingNetworkServers
                };
                DialogResult dialogResult = sqlServerBrowserDialog.ShowDialog();
                SqlLocalServers = sqlServerBrowserDialog.SqlLocalServers;
                SqlNetworkServers = sqlServerBrowserDialog.SqlNetworkServers;
                if (dialogResult != DialogResult.Cancel)
                {
                    if (!w_comboboxServerList.Properties.Items.Contains(sqlServerBrowserDialog.SelectedServer))
                    {
                        w_comboboxServerList.Properties.Items.Add(sqlServerBrowserDialog.SelectedServer);
                    }
                    w_comboboxServerList.SelectedItem = sqlServerBrowserDialog.SelectedServer;
                    GetCheckServerConnection()(GetConnectionBuilder());
                    w_comboboxServerList.BackColor = Color.White;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                w_comboboxServerList.BackColor = Color.Pink;
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void On_buttonConnect_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.None;
            if (SqlServerName == null || SqlServerName.Trim() == "")
            {
                FlatXtraMessageBox.Show("Server name cannot be blank", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (AllowBrowsingDatabases && string.IsNullOrEmpty(SqlDatabaseName))
            {
                FlatXtraMessageBox.Show("Database name cannot be blank", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string sqlServerName = SqlServerName;
            string connectionString = GetConnectionString();
            try
            {
                if (PleaseWaitDialog.ShowWaitDialog("Connecting to " + sqlServerName + " ...", delegate (BackgroundWorker worker)
                {
                    if (worker.CancellationPending)
                    {
                        return;
                    }
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                    }
                }))
                {
                    base.DialogResult = DialogResult.OK;
                }
            }
            catch (ArgumentException argumentException)
            {
                ConditionalDetailException conditionalDetailException = new ConditionalDetailException("Server name is too long, please use a shorter name.", argumentException);
                GlobalServices.ErrorHandler.HandleException(conditionalDetailException.Message, conditionalDetailException);
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("The server was not found"))
                {
                    FlatXtraMessageBox.Show("Database on " + sqlServerName + " is unreachable. This may be because the hostname has been incorrectly entered, or it is not configured for network connection.\n\nTo enable network connections:\n1. Click Start, point to Programs, point to Microsoft SQL Server 2005, point to Configuration Tools, and then click SQL Server Surface Area Configuration.\n\n2. On the SQL Server 2005 Surface Area Configuration page, click Surface Area Configuration for Services and Connections.\n\n3. On the Surface Area Configuration for Services and Connections page, expand Database Engine, click Remote Connections, \n click Local and remote connections, click the appropriate protocol to enable for your environment, and then click Apply.\n\n  Note Click OK when you receive the following message:\n  Changes to Connection Settings will not take effect until you restart the Database Engine service.\n\n4. On the Surface Area Configuration for Services and Connections page, expand Database Engine, click Service, click Stop,  wait until the MSSQLSERVER service stops, and then click Start to restart the MSSQLSERVER service.\n\nFor more information, see http://support.microsoft.com/kb/914277", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else if (!exception.Message.Contains("Login failed for user"))
                {
                    GlobalServices.ErrorHandler.HandleException(exception);
                }
                else
                {
                    FlatXtraMessageBox.Show("The SQL authentication used was invalid.\n\nIt might be the case that Windows login account information is being used to try to connect to the SQL Server, but the server does not recognize the Windows authentication as an SQL authentication account.\n\nCheck your SQL Server settings to see if the authentication entered is a valid login authentication for SQL Server.For more information on how to do this, contact support@metalogix.net.\n\nThe original error from SQL server is: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        public void SetCheckDatabaseConnection(System.Action<SqlConnectionStringBuilder> action)
        {
            m_checkDatabaseConnection = action;
        }

        public void SetCheckServerConnection(System.Action<SqlConnectionStringBuilder> action)
        {
            m_checkServerConnection = action;
        }

        public void SetConnectionString(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };
                w_comboboxServerList.Text = sqlConnectionStringBuilder.DataSource;
                w_textBoxDatabase.Text = sqlConnectionStringBuilder.InitialCatalog;
                if (sqlConnectionStringBuilder.IntegratedSecurity)
                {
                    w_radioCurrentAuth.Checked = true;
                    return;
                }
                w_radioDifferentAuth.Checked = true;
                w_textboxUsername.Text = sqlConnectionStringBuilder.UserID;
                w_maskedtextboxPassword.Text = sqlConnectionStringBuilder.Password;
            }
        }

        private void w_buttonBrowseDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                SqlDatabaseBrowserDialog sqlDatabaseBrowserDialog = new SqlDatabaseBrowserDialog
                {
                    AllowCreation = AllowDatabaseCreation,
                    AllowDeletion = AllowDatabaseDeletion
                };
                SqlConnectionStringBuilder connectionBuilder = GetConnectionBuilder();
                connectionBuilder.InitialCatalog = string.Empty;
                sqlDatabaseBrowserDialog.ConnectionString = connectionBuilder.ConnectionString;
                if (sqlDatabaseBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    w_textBoxDatabase.Text = sqlDatabaseBrowserDialog.SelectedDatabase;
                    GetCheckDatabaseConnection()(GetConnectionBuilder());
                    w_textBoxDatabase.BackColor = Color.White;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                w_textBoxDatabase.BackColor = Color.Pink;
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void w_comboboxServerList_TextChanged(object sender, EventArgs e)
        {
            w_textBoxDatabase.Enabled = w_comboboxServerList.Enabled && !string.IsNullOrEmpty(w_comboboxServerList.Text);
            w_buttonBrowseDatabase.Enabled = w_textBoxDatabase.Enabled;
        }

        private void w_radioDifferentAuth_CheckedChanged(object sender, EventArgs e)
        {
            w_textboxUsername.Enabled = w_radioDifferentAuth.Checked;
            w_maskedtextboxPassword.Enabled = w_radioDifferentAuth.Checked;
            w_cbRememberMe.Enabled = EnableRememberMe && w_radioDifferentAuth.Checked;
        }
    }
}
