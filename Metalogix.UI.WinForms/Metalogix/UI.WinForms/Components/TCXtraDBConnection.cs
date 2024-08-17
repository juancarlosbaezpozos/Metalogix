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
using Metalogix.UI.WinForms.Database;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.Components
{
    [ControlName("Connect to Database")]
    public class TCXtraDBConnection : XtraUserControl
    {
        private bool _showDatabaseBrowser = true;

        private ArrayList _serverHistory;

        private bool _allowBrowsingNetworkServers = true;

        private bool _enableRememberMe = true;

        private ArrayList _sqlLocalServers = new ArrayList();

        private ArrayList _sqlNetworkServers = new ArrayList();

        private System.Action<SqlConnectionStringBuilder> _checkServerConnection;

        private System.Action<SqlConnectionStringBuilder> _checkDatabaseConnection;

        private IContainer components;

        private GroupControl grpAuthentication;

        private CheckEdit cbRememberPassword;

        private TextEdit tbxLogin;

        private CheckEdit rbDifferentAuth;

        private CheckEdit rbCurrentAuth;

        private LabelControl lblPassword;

        private TextEdit tbxPassword;

        private LabelControl lblSqlServer;

        private SimpleButton btnBrowseSQL;

        private ComboBoxEdit cmbServerList;

        private TextEdit tbxDatabase;

        private SimpleButton btnBrowseDatabase;

        private LabelControl lblDatabase;

        public bool AllowBrowsingDatabases
        {
            get
            {
                return _showDatabaseBrowser;
            }
            set
            {
                _showDatabaseBrowser = value;
                if (!_showDatabaseBrowser)
                {
                    lblDatabase.Visible = false;
                    tbxDatabase.Visible = false;
                    btnBrowseDatabase.Visible = false;
                    base.Height = 262;
                }
            }
        }

        public bool AllowBrowsingNetworkServers
        {
            get
            {
                return _allowBrowsingNetworkServers;
            }
            set
            {
                _allowBrowsingNetworkServers = value;
                btnBrowseSQL.Visible = AllowBrowsingNetworkServers;
                cmbServerList.Properties.TextEditStyle = ((!AllowBrowsingNetworkServers) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
            }
        }

        public bool AllowDatabaseCreation { get; set; }

        public bool AllowDatabaseDeletion { get; set; }

        public Credentials Credentials
        {
            get
            {
                if (rbCurrentAuth.Checked)
                {
                    return new Credentials();
                }
                return new Credentials(tbxLogin.Text, tbxPassword.Text.ToSecureString(), cbRememberPassword.Checked);
            }
            private set
            {
                if (value != null)
                {
                    rbDifferentAuth.Checked = !value.IsDefault;
                    tbxLogin.Text = value.UserName;
                    tbxPassword.Text = value.Password.ToInsecureString();
                    cbRememberPassword.Checked = value.SavePassword;
                }
            }
        }

        public bool EnableLocationEditing
        {
            get
            {
                return cmbServerList.Enabled;
            }
            set
            {
                cmbServerList.Enabled = value;
                btnBrowseSQL.Enabled = value;
            }
        }

        public bool EnableRememberMe
        {
            get
            {
                return _enableRememberMe;
            }
            set
            {
                _enableRememberMe = value;
                cbRememberPassword.Enabled = _enableRememberMe;
            }
        }

        public bool RememberMe
        {
            get
            {
                return cbRememberPassword.Checked;
            }
            set
            {
                cbRememberPassword.Checked = value;
            }
        }

        public ArrayList ServerHistory
        {
            get
            {
                return _serverHistory;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    return;
                }
                _serverHistory = value;
                cmbServerList.Properties.Items.Clear();
                foreach (string str in value)
                {
                    cmbServerList.Properties.Items.Add(str);
                }
            }
        }

        public string SqlDatabaseName
        {
            get
            {
                return tbxDatabase.Text;
            }
            set
            {
                tbxDatabase.Text = value;
                if (value != null)
                {
                    tbxDatabase.Enabled = false;
                    btnBrowseDatabase.Enabled = false;
                }
            }
        }

        public ArrayList SqlLocalServers
        {
            get
            {
                return _sqlLocalServers;
            }
            set
            {
                _sqlLocalServers = value;
            }
        }

        public ArrayList SqlNetworkServers
        {
            get
            {
                return _sqlNetworkServers;
            }
            set
            {
                _sqlNetworkServers = value;
            }
        }

        public bool SqlServerIsLocal => DatabaseBrowser.GetLocalSQLServers().Contains(SqlServerName);

        public string SqlServerName
        {
            get
            {
                return cmbServerList.Text;
            }
            set
            {
                cmbServerList.Text = value;
                if (!string.IsNullOrEmpty(value))
                {
                    cmbServerList.Enabled = false;
                    btnBrowseSQL.Enabled = false;
                }
            }
        }

        public IEnumerable VisitedServers
        {
            get
            {
                return cmbServerList.Properties.Items;
            }
            set
            {
                cmbServerList.Properties.Items.Clear();
                if (value == null)
                {
                    return;
                }
                foreach (object obj in value)
                {
                    cmbServerList.Properties.Items.Add(obj);
                }
                if (string.IsNullOrEmpty(cmbServerList.Text) && cmbServerList.Properties.Items.Count > 0)
                {
                    cmbServerList.SelectedIndex = 0;
                }
            }
        }

        public TCXtraDBConnection()
        {
            InitializeComponent();
            rbCurrentAuth.Text = "Use Current Windows User: " + WindowsIdentity.GetCurrent().Name;
        }

        private void btnBrowseDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                SqlDatabaseBrowserDialog sqlDatabaseBrowserDialog = new SqlDatabaseBrowserDialog
                {
                    AllowCreation = AllowDatabaseCreation,
                    AllowDeletion = AllowDatabaseDeletion
                };
                SqlDatabaseBrowserDialog connectionString = sqlDatabaseBrowserDialog;
                SqlConnectionStringBuilder connectionBuilder = GetConnectionBuilder();
                connectionBuilder.InitialCatalog = string.Empty;
                connectionString.ConnectionString = connectionBuilder.ConnectionString;
                if (connectionString.ShowDialog() == DialogResult.OK)
                {
                    tbxDatabase.Text = connectionString.SelectedDatabase;
                    GetCheckDatabaseConnection()(GetConnectionBuilder());
                    tbxDatabase.BackColor = Color.White;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                tbxDatabase.BackColor = Color.Pink;
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void btnBrowseSQL_Click(object sender, EventArgs e)
        {
            try
            {
                SqlServerBrowserDialog sqlServerBrowserDialog = new SqlServerBrowserDialog
                {
                    SqlLocalServers = SqlLocalServers,
                    SqlNetworkServers = SqlNetworkServers,
                    ShowNetworkServersTab = AllowBrowsingNetworkServers
                };
                SqlServerBrowserDialog sqlServerBrowserDialog1 = sqlServerBrowserDialog;
                DialogResult dialogResult = sqlServerBrowserDialog1.ShowDialog();
                SqlLocalServers = sqlServerBrowserDialog1.SqlLocalServers;
                SqlNetworkServers = sqlServerBrowserDialog1.SqlNetworkServers;
                if (dialogResult != DialogResult.Cancel)
                {
                    if (!cmbServerList.Properties.Items.Contains(sqlServerBrowserDialog1.SelectedServer))
                    {
                        cmbServerList.Properties.Items.Add(sqlServerBrowserDialog1.SelectedServer);
                    }
                    cmbServerList.SelectedItem = sqlServerBrowserDialog1.SelectedServer;
                    GetCheckServerConnection()(GetConnectionBuilder());
                    cmbServerList.BackColor = Color.White;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                cmbServerList.BackColor = Color.Pink;
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void cmbServerList_TextChanged(object sender, EventArgs e)
        {
            tbxDatabase.Enabled = cmbServerList.Enabled && !string.IsNullOrEmpty(cmbServerList.Text);
            btnBrowseDatabase.Enabled = tbxDatabase.Enabled;
            if (!btnBrowseDatabase.Enabled && !string.IsNullOrEmpty(tbxDatabase.Text))
            {
                tbxDatabase.Text = string.Empty;
            }
        }

        public bool ConnectToDatabase(string caption)
        {
            if (string.IsNullOrEmpty(SqlServerName))
            {
                FlatXtraMessageBox.Show("Server name cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbServerList.Focus();
                return false;
            }
            if (AllowBrowsingDatabases && string.IsNullOrEmpty(SqlDatabaseName))
            {
                FlatXtraMessageBox.Show("Database name cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbxDatabase.Focus();
                return false;
            }
            if (rbDifferentAuth.Checked)
            {
                if (string.IsNullOrEmpty(tbxLogin.Text))
                {
                    FlatXtraMessageBox.Show("Login cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbxLogin.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(tbxPassword.Text))
                {
                    FlatXtraMessageBox.Show("Password cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbxPassword.Focus();
                    return false;
                }
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
                    return true;
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
            return false;
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
            if (_checkDatabaseConnection == null)
            {
                _checkDatabaseConnection = delegate
                {
                };
            }
            return _checkDatabaseConnection;
        }

        public System.Action<SqlConnectionStringBuilder> GetCheckServerConnection()
        {
            if (_checkServerConnection == null)
            {
                _checkServerConnection = delegate
                {
                };
            }
            return _checkServerConnection;
        }

        private SqlConnectionStringBuilder GetConnectionBuilder()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = SqlServerName
            };
            if (AllowBrowsingDatabases)
            {
                sqlConnectionStringBuilder.InitialCatalog = tbxDatabase.Text;
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
            this.grpAuthentication = new DevExpress.XtraEditors.GroupControl();
            this.cbRememberPassword = new DevExpress.XtraEditors.CheckEdit();
            this.tbxLogin = new DevExpress.XtraEditors.TextEdit();
            this.rbDifferentAuth = new DevExpress.XtraEditors.CheckEdit();
            this.rbCurrentAuth = new DevExpress.XtraEditors.CheckEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.tbxPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblSqlServer = new DevExpress.XtraEditors.LabelControl();
            this.btnBrowseSQL = new DevExpress.XtraEditors.SimpleButton();
            this.cmbServerList = new DevExpress.XtraEditors.ComboBoxEdit();
            this.tbxDatabase = new DevExpress.XtraEditors.TextEdit();
            this.btnBrowseDatabase = new DevExpress.XtraEditors.SimpleButton();
            this.lblDatabase = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)this.grpAuthentication).BeginInit();
            this.grpAuthentication.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.cbRememberPassword.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxLogin.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.rbDifferentAuth.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.rbCurrentAuth.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbServerList.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxDatabase.Properties).BeginInit();
            base.SuspendLayout();
            this.grpAuthentication.Controls.Add(this.cbRememberPassword);
            this.grpAuthentication.Controls.Add(this.tbxLogin);
            this.grpAuthentication.Controls.Add(this.rbDifferentAuth);
            this.grpAuthentication.Controls.Add(this.rbCurrentAuth);
            this.grpAuthentication.Controls.Add(this.lblPassword);
            this.grpAuthentication.Controls.Add(this.tbxPassword);
            this.grpAuthentication.Location = new System.Drawing.Point(8, 77);
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Size = new System.Drawing.Size(424, 132);
            this.grpAuthentication.TabIndex = 106;
            this.grpAuthentication.Text = "Authentication";
            this.cbRememberPassword.Enabled = false;
            this.cbRememberPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbRememberPassword.Location = new System.Drawing.Point(155, 106);
            this.cbRememberPassword.Name = "cbRememberPassword";
            this.cbRememberPassword.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.cbRememberPassword.Properties.Appearance.Options.UseFont = true;
            this.cbRememberPassword.Properties.AutoWidth = true;
            this.cbRememberPassword.Properties.Caption = "Remember my password";
            this.cbRememberPassword.Size = new System.Drawing.Size(138, 19);
            this.cbRememberPassword.TabIndex = 5;
            this.tbxLogin.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tbxLogin.Enabled = false;
            this.tbxLogin.Location = new System.Drawing.Point(157, 54);
            this.tbxLogin.Name = "tbxLogin";
            this.tbxLogin.Size = new System.Drawing.Size(253, 20);
            this.tbxLogin.TabIndex = 2;
            this.rbDifferentAuth.Location = new System.Drawing.Point(17, 54);
            this.rbDifferentAuth.Name = "rbDifferentAuth";
            this.rbDifferentAuth.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.rbDifferentAuth.Properties.Appearance.Options.UseFont = true;
            this.rbDifferentAuth.Properties.AutoWidth = true;
            this.rbDifferentAuth.Properties.Caption = "Use SQL Server Login:";
            this.rbDifferentAuth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbDifferentAuth.Properties.RadioGroupIndex = 1;
            this.rbDifferentAuth.Size = new System.Drawing.Size(132, 19);
            this.rbDifferentAuth.TabIndex = 1;
            this.rbDifferentAuth.TabStop = false;
            this.rbDifferentAuth.CheckedChanged += new System.EventHandler(rdDifferentAuth_CheckedChanged);
            this.rbCurrentAuth.EditValue = true;
            this.rbCurrentAuth.Location = new System.Drawing.Point(17, 29);
            this.rbCurrentAuth.Name = "rbCurrentAuth";
            this.rbCurrentAuth.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.rbCurrentAuth.Properties.Appearance.Options.UseFont = true;
            this.rbCurrentAuth.Properties.AutoWidth = true;
            this.rbCurrentAuth.Properties.Caption = "Use Windows Authentication ";
            this.rbCurrentAuth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbCurrentAuth.Properties.RadioGroupIndex = 1;
            this.rbCurrentAuth.Size = new System.Drawing.Size(163, 19);
            this.rbCurrentAuth.TabIndex = 0;
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.lblPassword.Location = new System.Drawing.Point(95, 83);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(49, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            this.tbxPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tbxPassword.Enabled = false;
            this.tbxPassword.Location = new System.Drawing.Point(157, 80);
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.Properties.PasswordChar = '*';
            this.tbxPassword.Size = new System.Drawing.Size(253, 20);
            this.tbxPassword.TabIndex = 4;
            this.lblSqlServer.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f);
            this.lblSqlServer.Location = new System.Drawing.Point(8, 19);
            this.lblSqlServer.Name = "lblSqlServer";
            this.lblSqlServer.Size = new System.Drawing.Size(65, 15);
            this.lblSqlServer.TabIndex = 107;
            this.lblSqlServer.Text = "SQL Server:";
            this.btnBrowseSQL.Location = new System.Drawing.Point(367, 16);
            this.btnBrowseSQL.Name = "btnBrowseSQL";
            this.btnBrowseSQL.Size = new System.Drawing.Size(65, 23);
            this.btnBrowseSQL.TabIndex = 103;
            this.btnBrowseSQL.Text = "Browse...";
            this.btnBrowseSQL.Click += new System.EventHandler(btnBrowseSQL_Click);
            this.cmbServerList.Location = new System.Drawing.Point(101, 18);
            this.cmbServerList.Name = "cmbServerList";
            this.cmbServerList.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.cmbServerList.Properties.Appearance.Options.UseFont = true;
            this.cmbServerList.Size = new System.Drawing.Size(260, 20);
            this.cmbServerList.TabIndex = 102;
            this.cmbServerList.TextChanged += new System.EventHandler(cmbServerList_TextChanged);
            this.tbxDatabase.Enabled = false;
            this.tbxDatabase.Location = new System.Drawing.Point(101, 49);
            this.tbxDatabase.Name = "tbxDatabase";
            this.tbxDatabase.Size = new System.Drawing.Size(260, 20);
            this.tbxDatabase.TabIndex = 104;
            this.btnBrowseDatabase.Enabled = false;
            this.btnBrowseDatabase.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBrowseDatabase.Location = new System.Drawing.Point(367, 47);
            this.btnBrowseDatabase.Name = "btnBrowseDatabase";
            this.btnBrowseDatabase.Size = new System.Drawing.Size(65, 23);
            this.btnBrowseDatabase.TabIndex = 105;
            this.btnBrowseDatabase.Text = "Browse...";
            this.btnBrowseDatabase.Click += new System.EventHandler(btnBrowseDatabase_Click);
            this.lblDatabase.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f);
            this.lblDatabase.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDatabase.Location = new System.Drawing.Point(8, 50);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(83, 15);
            this.lblDatabase.TabIndex = 108;
            this.lblDatabase.Text = "SQL Database:";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.grpAuthentication);
            base.Controls.Add(this.lblSqlServer);
            base.Controls.Add(this.btnBrowseSQL);
            base.Controls.Add(this.cmbServerList);
            base.Controls.Add(this.tbxDatabase);
            base.Controls.Add(this.btnBrowseDatabase);
            base.Controls.Add(this.lblDatabase);
            base.Name = "TCXtraDBConnection";
            base.Size = new System.Drawing.Size(441, 225);
            base.Load += new System.EventHandler(TCXtraDBConnection_Load);
            ((System.ComponentModel.ISupportInitialize)this.grpAuthentication).EndInit();
            this.grpAuthentication.ResumeLayout(false);
            this.grpAuthentication.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.cbRememberPassword.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxLogin.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.rbDifferentAuth.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.rbCurrentAuth.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbServerList.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxDatabase.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void rdDifferentAuth_CheckedChanged(object sender, EventArgs e)
        {
            tbxLogin.Enabled = rbDifferentAuth.Checked;
            tbxPassword.Enabled = rbDifferentAuth.Checked;
            cbRememberPassword.Enabled = EnableRememberMe && rbDifferentAuth.Checked;
            if (!tbxLogin.Enabled && !string.IsNullOrEmpty(tbxLogin.Text))
            {
                tbxLogin.Text = string.Empty;
            }
            if (!tbxPassword.Enabled && !string.IsNullOrEmpty(tbxPassword.Text))
            {
                tbxPassword.Text = string.Empty;
            }
        }

        public void SetCheckDatabaseConnection(System.Action<SqlConnectionStringBuilder> action)
        {
            _checkDatabaseConnection = action;
        }

        public void SetCheckServerConnection(System.Action<SqlConnectionStringBuilder> action)
        {
            _checkServerConnection = action;
        }

        public void SetConnectionString(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };
                cmbServerList.Text = sqlConnectionStringBuilder.DataSource;
                tbxDatabase.Text = sqlConnectionStringBuilder.InitialCatalog;
                if (sqlConnectionStringBuilder.IntegratedSecurity)
                {
                    rbCurrentAuth.Checked = true;
                    return;
                }
                rbDifferentAuth.Checked = true;
                tbxLogin.Text = sqlConnectionStringBuilder.UserID;
                tbxPassword.Text = sqlConnectionStringBuilder.Password;
            }
        }

        private void TCXtraDBConnection_Load(object sender, EventArgs e)
        {
            try
            {
                cmbServerList.Focus();
                VisitedServers = DatabaseSettings.VisitedSQLServers;
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }
    }
}
