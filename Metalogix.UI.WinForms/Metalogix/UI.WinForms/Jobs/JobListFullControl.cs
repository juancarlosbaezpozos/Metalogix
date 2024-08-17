using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Core.ConfigVariables;
using Metalogix.Core.ObjectResolution;
using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Reporting.Actions;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Database;
using Metalogix.UI.WinForms.Jobs.Actions;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.Jobs
{
    public class JobListFullControl : XtraUserControl
    {
        private delegate void UpdateUIDelegate();

        private readonly string _resourceTableKeyName = $"{typeof(ResourceTable).Name}{typeof(ResourceTableLink).Name}";

        private readonly string _restartApplicationWarningForFileSystemToDB = "For remote job migrations, the application, user, and environmental settings will be referenced from the Agent Database. You must restart the application for this change to take effect.";

        private readonly string _restartApplicationWarningForDBToFileSystem = "For local job migrations, the application, user, and environmental settings will be referenced from the local file system. You must restart the application for this change to take effect.";

        private JobHistoryDb _dataSource;

        private bool _useMostRecentlyUsedList = true;

        private LinkPersistInfo[] _defaultFileMenuItems;

        private IContainer components;

        private Panel _jobSourcePanel;

        private StandaloneBarDockControl _menuBarDockLocation;

        private XtraBarManagerWithArrows _menuBarManager;

        private Bar _menuBar;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarButtonItem _runJobsButton;

        private BarButtonItem btnRunJobsRemotely;

        private BarButtonItem _deleteJobsButton;

        private BarButtonItem _viewLogButton;

        private BarButtonItem _changeConfigurationButton;

        private BarButtonItem _exportExcelButton;

        private BarButtonItem _copyJobsButton;

        private BarButtonItem _removeSortButton;

        private JobListControl _jobList;

        private LabelControl _jobSourceLabel;

        private TextEdit _jobSourceValue;

        private BarButtonItem barButtonItem1;

        private BarButtonItem _fileMenu;

        private BarButtonItem btnConnectToAgentDb;

        private BarButtonItem _connectToDBButton;

        private PopupMenu _filePopupMenu;

        private BarButtonItem _newJobListButtons;

        private BarButtonItem _openJobListButton;

        private BarButtonItem _saveJobListAsButton;

        private BarButtonItem _importFromXmlButton;

        private BarButtonItem _refreshButton;

        private BarButtonItem _genPSMenu;

        private PopupMenu _genPSPopupMenu;

        private BarButtonItem _genPSUserButton;

        private BarButtonItem _genPSMachineButton;

        private BarButtonItem _genPSCertificateButton;

        private BarButtonItem _schedTaskMenu;

        private PopupMenu _scheduleTaskPopupMenu;

        private BarButtonItem _schedUserButton;

        private BarButtonItem _schedMachineButton;

        private BarButtonItem _schedCertificateButton;

        private BarButtonItem _navMenu;

        private PopupMenu _navigatePopupMenu;

        private BarButtonItem _navSourceButton;

        private BarButtonItem _navTargetButton;

        private BarButtonItem _launchTaskSchedulerButton;

        private TextEditContextMenu _jobListContextMenu;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobHistoryDb DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
                _dataSource = value;
                JobListControl jobListControl = _jobList;
                JobCollection jobs = ((_dataSource != null) ? _dataSource.Jobs : null);
                jobListControl.DataSource = jobs;
                if (this.OnDataSourceChanged != null)
                {
                    this.OnDataSourceChanged(_dataSource.AdapterType.Equals("Agent", StringComparison.InvariantCultureIgnoreCase), null);
                }
                JobsSettings.AdapterType = _dataSource.AdapterType;
                JobsSettings.AdapterContext = _dataSource.AdapterContext.ToSecureString();
                if (_dataSource != null)
                {
                    _dataSource.Jobs.ListChanged += On_Jobs_ListChanged;
                    if (UseMostRecentlyUsedList && string.Equals("SqlCe", _dataSource.AdapterType, StringComparison.OrdinalIgnoreCase))
                    {
                        JobsSettings.JobHistoryMRU.Add(_dataSource.AdapterContext);
                    }
                }
                UpdateUI();
            }
        }

        public JobListControl JobList
        {
            get
            {
                return _jobList;
            }
            set
            {
                _jobList = value;
            }
        }

        internal bool ReLoadJobsList { get; set; }

        public bool ShowLicenseUsage
        {
            get
            {
                return _jobList.ShowLicenseUsage;
            }
            set
            {
                _jobList.ShowLicenseUsage = value;
            }
        }

        public bool UseMostRecentlyUsedList
        {
            get
            {
                return _useMostRecentlyUsedList;
            }
            set
            {
                _useMostRecentlyUsedList = value;
            }
        }

        public event EventHandler OnDataSourceChanged;

        public event JobListControl.SelectedJobChangedHandler SelectedJobChanged;

        public JobListFullControl()
        {
            InitializeComponent();
            InitalizeMRUList();
            InitializeDataSource();
        }

        private void _changeConfigurationButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new ReconfigureJobs());
        }

        private void _connectToDBButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ShowDisconnectAgentDialog())
            {
                return;
            }
            bool flag = JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
            string str = ConnectDBAndGetConnectionString(new DatabaseConnectDialog());
            if (str != null && (!JobsSettings.AdapterType.Equals(JobHistoryAdapterType.SqlServer.ToString(), StringComparison.InvariantCultureIgnoreCase) || !str.Equals(JobsSettings.AdapterContext.ToInsecureString())))
            {
                ConnectToDB(Resources.FS_OpeningJobWaitMessage, str, delegate
                {
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlServer, str);
                });
                if (flag)
                {
                    SaveAllSettingsToFileSystem();
                    FlatXtraMessageBox.Show(_restartApplicationWarningForDBToFileSystem, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void _copyJobsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new CopyJobToClipboard());
        }

        private void _deleteJobsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new DeleteSelectedJobs());
        }

        private void _exportExcelButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnActionClick(new ExportJobsToExcel());
        }

        private void _filePopupMenu_BeforePopup(object sender, CancelEventArgs e)
        {
            try
            {
                bool dataSource = DataSource != null;
                bool flag = !dataSource || !_jobList.HasRunningAction;
                _connectToDBButton.Enabled = flag;
                _newJobListButtons.Enabled = flag;
                _openJobListButton.Enabled = flag;
                _saveJobListAsButton.Enabled = flag && dataSource && DataSource.AdapterType == "SqlCe";
                _importFromXmlButton.Enabled = dataSource;
                _refreshButton.Enabled = dataSource && flag;
                _filePopupMenu.ClearLinks();
                for (int i = 0; i < _defaultFileMenuItems.Length; i++)
                {
                    _filePopupMenu.ItemLinks.Add(_defaultFileMenuItems[i].Item, _defaultFileMenuItems[i].BeginGroup);
                }
                if (!UseMostRecentlyUsedList || JobsSettings.JobHistoryMRU.Count < 1)
                {
                    return;
                }
                for (int j = 0; j < JobsSettings.JobHistoryMRU.Count; j++)
                {
                    string item = (string)JobsSettings.JobHistoryMRU[j];
                    string name = item;
                    if (name.Length > 50)
                    {
                        FileInfo fileInfo = new FileInfo(name);
                        DirectoryInfo directory = fileInfo.Directory;
                        name = fileInfo.Name;
                        while (name.Length < 50 && directory.Parent != null && directory.Name != fileInfo.Directory.Root.Name)
                        {
                            name = directory.Name + "\\" + name;
                            directory = directory.Parent;
                        }
                        name = string.Concat(fileInfo.Directory.Root, "...\\", name);
                    }
                    name = j + ": " + name;
                    BarButtonItem barButtonItem = new BarButtonItem(_menuBarManager, name)
                    {
                        Tag = item
                    };
                    barButtonItem.ItemClick += On_mruButton_ItemClick;
                    _filePopupMenu.ItemLinks.Add(barButtonItem, j == 0);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        private void _genPSCertificateButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new GeneratePowerShellScriptForCertificate());
        }

        private void _genPSMachineButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new GeneratePowerShellScriptForMachine());
        }

        private void _genPSUserButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new GeneratePowerShellScriptForUser());
        }

        private void _importFromXmlButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DataSource == null)
            {
                FlatXtraMessageBox.Show("Please connect to a valid job list before continuing.", "Job list not found.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.RestoreDirectory = false;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.Filter = "XML Document (*.xml) | *.xml";
                    openFileDialog.Multiselect = false;
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(openFileDialog.FileName);
                    if (!Job.TryCreateJobs(xmlDocument.DocumentElement, out var jobs))
                    {
                        return;
                    }
                    foreach (Job job in jobs)
                    {
                        DataSource.Jobs.Add(job);
                    }
                    DataSource.Jobs.Update();
                }
            }
            catch (Exception exception)
            {
                DataSource.Jobs.FetchData();
                GlobalServices.ErrorHandler.HandleException("Failed to import 1 or more jobs", exception);
            }
        }

        private void _jobList_JobsUpdated()
        {
            UpdateUI();
        }

        private void _jobList_SelectedJobChanged()
        {
            UpdateUI();
        }

        private void _jobList_SortingChanged(bool sortingApplied)
        {
            _removeSortButton.Enabled = sortingApplied;
        }

        private void _launchTaskSchedulerButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process.Start("taskschd.msc");
        }

        private void _navSourceButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new NavigateToJobSourceAction());
        }

        private void _navTargetButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new NavigateToJobTargetAction());
        }

        private void _newJobListButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateAndConnectNewJobList();
        }

        private void _openJobListButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenJobListFile();
        }

        private void _refreshButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                PleaseWaitDialog.ShowWaitDialog(Resources.RefreshJobList, delegate
                {
                    DataSource = JobFactory.CreateJobHistoryDb(DataSource.AdapterType, DataSource.AdapterContext);
                }, cancellable: false);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Invalid job file: " + exception.Message, exception);
            }
        }

        private void _removeSortButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            _jobList.ResetSort();
        }

        private void _runJobsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new RunSelectedJobs());
        }

        private void _saveJobListAsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveJobListAs();
        }

        private void _schedCertificateButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new ScheduleTaskForCertificate());
        }

        private void _schedMachineButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new ScheduleTaskForCurrentMachine());
        }

        private void _schedUserButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new ScheduleTaskForCurrentUser());
        }

        private void _viewLogButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunJobListAction(new ViewLogsForJobs());
        }

        private void btnConnectToAgentDb_ItemClick(object sender, ItemClickEventArgs e)
        {
            DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog
            {
                Text = Resources.ConnectAgentDatabase
            };
            Bitmap connectToAgentDatabase16 = Resources.ConnectToAgentDatabase16;
            databaseConnectDialog.Icon = Icon.FromHandle(connectToAgentDatabase16.GetHicon());
            string str = ConnectDBAndGetConnectionString(databaseConnectDialog);
            bool flag = JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(str))
            {
                ConnectToAgentDb(str, isSaveAllSettingsToDb: true, flag);
            }
        }

        private void btnRunJobsRemotely_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnActionClick(new RunSelectedJobsRemotely());
        }

        private static string ConnectDBAndGetConnectionString(DatabaseConnectDialog dlg)
        {
            dlg.AllowDatabaseCreation = true;
            dlg.AllowDatabaseDeletion = true;
            dlg.AllowBrowsingNetworkServers = true;
            dlg.AllowBrowsingDatabases = true;
            dlg.EnableRememberMe = false;
            dlg.RememberMe = true;
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            return dlg.GetConnectionString();
        }

        public bool ConnectToAgentDb(string agentDbConnectionString, bool isSaveAllSettingsToDb = false, bool isAgentDBConnected = false)
        {
            if (agentDbConnectionString != null && (!JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase) || !agentDbConnectionString.Equals(JobsSettings.AdapterContext.ToInsecureString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                if (ShowDisconnectAgentDialog())
                {
                    return false;
                }
                ConnectToDB(Resources.OpeningAgentDBWaitMessage, agentDbConnectionString, delegate
                {
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.Agent, agentDbConnectionString);
                });
                if (isSaveAllSettingsToDb)
                {
                    string configurationSettingsFileSystemToAgentDatabase = Resources.ConfigurationSettingsFileSystemToAgentDatabase;
                    if (isAgentDBConnected)
                    {
                        configurationSettingsFileSystemToAgentDatabase = Resources.ConfigurationSettingsAgentDbToAgentDb;
                    }
                    if (FlatXtraMessageBox.Show(configurationSettingsFileSystemToAgentDatabase, Resources.ConfigurationSettings, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SaveAllSettingsToDb(agentDbConnectionString);
                    }
                }
                DefaultResolverSettings.SaveSetting(_resourceTableKeyName, typeof(ResourceDatabaseTableResolver).AssemblyQualifiedName);
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(agentDbConnectionString);
                if (!sqlConnectionStringBuilder.IntegratedSecurity)
                {
                    sqlConnectionStringBuilder.Password = Cryptography.EncryptTextusingAES(sqlConnectionStringBuilder.Password.ToSecureString());
                }
                DefaultResolverSettings.SaveSetting(ResourceDatabaseTableResolver.SettingsKey, sqlConnectionStringBuilder.ConnectionString.Replace("\"", ""));
                if (isSaveAllSettingsToDb)
                {
                    FlatXtraMessageBox.Show(_restartApplicationWarningForFileSystemToDB, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            return true;
        }

        private static void ConnectToDB(string waitMessage, string connectionString, System.Action<BackgroundWorker> action)
        {
            try
            {
                PleaseWaitDialog.ShowWaitDialog(string.Format(waitMessage, connectionString), action, cancellable: false);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Unable to connect to database", exception.Message, exception);
            }
        }

        private void CopyValuesToTargetResolver(ResourceScope resourceScope, DataResolver targetResolver, List<string> settings = null)
        {
            foreach (ResourceTableLink validResourceTable in ConfigurationVariables.GetValidResourceTables(resourceScope))
            {
                ResourceTable resourceTable = validResourceTable.Resolve();
                IEnumerable<string> availableResources = null;
                availableResources = ((settings == null) ? resourceTable.GetAvailableResources() : settings);
                foreach (string availableResource in availableResources)
                {
                    targetResolver.WriteStringDataAtKey(availableResource, resourceTable.GetResource(availableResource));
                }
            }
        }

        private void CreateAndConnectNewJobList()
        {
            if (ShowDisconnectAgentDialog())
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Create new job list file",
                Filter = "Job List files (*.lst)|*.lst",
                InitialDirectory = ApplicationData.ApplicationPath,
                CheckFileExists = false
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = openFileDialog.FileName;
            try
            {
                bool flag = JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
                PleaseWaitDialog.ShowWaitDialog(string.Format(Resources.FS_OpeningJobWaitMessage, fileName), delegate
                {
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, fileName);
                }, cancellable: false);
                if (flag)
                {
                    SaveAllSettingsToFileSystem();
                    FlatXtraMessageBox.Show(_restartApplicationWarningForDBToFileSystem, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
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

        internal void HandleUnableToOpenJobList()
        {
            if (JobsSettings.AdapterType.Equals(JobHistoryAdapterType.SqlServer.ToString()))
            {
                FlatXtraMessageBox.Show(base.ParentForm, Resources.UnableToOpenSQLJobDatabaseMessageBoxDetails, Resources.Unable_to_Open_Job_History, MessageBoxButtons.OK);
                CreateAndConnectNewJobList();
            }
            else
            {
                FlatXtraMessageBox.Show(base.ParentForm, Resources.JobListFullControl_JobListFullControl_Could_not_open_job_history_database_with_stored_credentials__Reopen_file_to_refresh_credentials_, Resources.Unable_to_Open_Job_History, MessageBoxButtons.OK);
                OpenJobListFile();
            }
        }

        private void InitalizeMRUList()
        {
            _defaultFileMenuItems = new LinkPersistInfo[_filePopupMenu.LinksPersistInfo.Count];
            for (int i = 0; i < _defaultFileMenuItems.Length; i++)
            {
                _defaultFileMenuItems[i] = _filePopupMenu.LinksPersistInfo[i];
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._jobSourcePanel = new System.Windows.Forms.Panel();
            this._jobSourceValue = new DevExpress.XtraEditors.TextEdit();
            this._jobListContextMenu = new Metalogix.UI.WinForms.TextEditContextMenu();
            this._jobSourceLabel = new DevExpress.XtraEditors.LabelControl();
            this._menuBarManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
            this._menuBar = new DevExpress.XtraBars.Bar();
            this._fileMenu = new DevExpress.XtraBars.BarButtonItem();
            this._filePopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this.btnConnectToAgentDb = new DevExpress.XtraBars.BarButtonItem();
            this._connectToDBButton = new DevExpress.XtraBars.BarButtonItem();
            this._newJobListButtons = new DevExpress.XtraBars.BarButtonItem();
            this._openJobListButton = new DevExpress.XtraBars.BarButtonItem();
            this._saveJobListAsButton = new DevExpress.XtraBars.BarButtonItem();
            this._importFromXmlButton = new DevExpress.XtraBars.BarButtonItem();
            this._refreshButton = new DevExpress.XtraBars.BarButtonItem();
            this._runJobsButton = new DevExpress.XtraBars.BarButtonItem();
            this.btnRunJobsRemotely = new DevExpress.XtraBars.BarButtonItem();
            this._copyJobsButton = new DevExpress.XtraBars.BarButtonItem();
            this._deleteJobsButton = new DevExpress.XtraBars.BarButtonItem();
            this._exportExcelButton = new DevExpress.XtraBars.BarButtonItem();
            this._viewLogButton = new DevExpress.XtraBars.BarButtonItem();
            this._changeConfigurationButton = new DevExpress.XtraBars.BarButtonItem();
            this._genPSMenu = new DevExpress.XtraBars.BarButtonItem();
            this._genPSPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this._genPSUserButton = new DevExpress.XtraBars.BarButtonItem();
            this._genPSMachineButton = new DevExpress.XtraBars.BarButtonItem();
            this._genPSCertificateButton = new DevExpress.XtraBars.BarButtonItem();
            this._schedTaskMenu = new DevExpress.XtraBars.BarButtonItem();
            this._scheduleTaskPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this._schedUserButton = new DevExpress.XtraBars.BarButtonItem();
            this._schedMachineButton = new DevExpress.XtraBars.BarButtonItem();
            this._schedCertificateButton = new DevExpress.XtraBars.BarButtonItem();
            this._launchTaskSchedulerButton = new DevExpress.XtraBars.BarButtonItem();
            this._navMenu = new DevExpress.XtraBars.BarButtonItem();
            this._navigatePopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this._navSourceButton = new DevExpress.XtraBars.BarButtonItem();
            this._navTargetButton = new DevExpress.XtraBars.BarButtonItem();
            this._removeSortButton = new DevExpress.XtraBars.BarButtonItem();
            this._menuBarDockLocation = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this._jobList = new Metalogix.UI.WinForms.Jobs.JobListControl();
            this._jobSourcePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._jobSourceValue.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._menuBarManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._filePopupMenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._genPSPopupMenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._scheduleTaskPopupMenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._navigatePopupMenu).BeginInit();
            base.SuspendLayout();
            this._jobSourcePanel.BackColor = System.Drawing.Color.Silver;
            this._jobSourcePanel.Controls.Add(this._jobSourceValue);
            this._jobSourcePanel.Controls.Add(this._jobSourceLabel);
            this._jobSourcePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._jobSourcePanel.Location = new System.Drawing.Point(0, 0);
            this._jobSourcePanel.Name = "_jobSourcePanel";
            this._jobSourcePanel.Size = new System.Drawing.Size(1379, 18);
            this._jobSourcePanel.TabIndex = 0;
            this._jobSourceValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._jobSourceValue.EditValue = "job list address";
            this._jobSourceValue.Location = new System.Drawing.Point(68, 0);
            this._jobSourceValue.Name = "_jobSourceValue";
            this._jobSourceValue.Properties.Appearance.BackColor = System.Drawing.Color.Silver;
            this._jobSourceValue.Properties.Appearance.Options.UseBackColor = true;
            this._jobSourceValue.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._jobSourceValue.Properties.ContextMenuStrip = this._jobListContextMenu;
            this._jobSourceValue.Properties.ReadOnly = true;
            this._jobSourceValue.Size = new System.Drawing.Size(1308, 18);
            this._jobSourceValue.TabIndex = 1;
            this._jobListContextMenu.Name = "TextEditContextMenu";
            this._jobListContextMenu.Size = new System.Drawing.Size(118, 148);
            this._jobSourceLabel.Location = new System.Drawing.Point(2, 2);
            this._jobSourceLabel.Name = "_jobSourceLabel";
            this._jobSourceLabel.Size = new System.Drawing.Size(53, 13);
            this._jobSourceLabel.TabIndex = 0;
            this._jobSourceLabel.Text = "Job Source";
            this._menuBarManager.AllowCustomization = false;
            this._menuBarManager.AllowMoveBarOnToolbar = false;
            this._menuBarManager.AllowQuickCustomization = false;
            this._menuBarManager.AllowShowToolbarsPopup = false;
            this._menuBarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this._menuBar });
            this._menuBarManager.DockControls.Add(this.barDockControlTop);
            this._menuBarManager.DockControls.Add(this.barDockControlBottom);
            this._menuBarManager.DockControls.Add(this.barDockControlLeft);
            this._menuBarManager.DockControls.Add(this.barDockControlRight);
            this._menuBarManager.DockControls.Add(this._menuBarDockLocation);
            this._menuBarManager.Form = this;
            DevExpress.XtraBars.BarItems items = this._menuBarManager.Items;
            DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[29]
            {
                this._runJobsButton, this.btnRunJobsRemotely, this._deleteJobsButton, this._viewLogButton, this._changeConfigurationButton, this._exportExcelButton, this._copyJobsButton, this._removeSortButton, this.barButtonItem1, this._fileMenu,
                this.btnConnectToAgentDb, this._connectToDBButton, this._newJobListButtons, this._openJobListButton, this._saveJobListAsButton, this._importFromXmlButton, this._refreshButton, this._genPSUserButton, this._genPSMachineButton, this._genPSCertificateButton,
                this._genPSMenu, this._schedUserButton, this._schedMachineButton, this._schedCertificateButton, this._schedTaskMenu, this._navSourceButton, this._navTargetButton, this._navMenu, this._launchTaskSchedulerButton
            };
            items.AddRange(barItemArray);
            this._menuBarManager.MainMenu = this._menuBar;
            this._menuBarManager.MaxItemId = 53;
            this._menuBar.BarItemHorzIndent = 3;
            this._menuBar.BarName = "Main menu";
            this._menuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Standalone;
            this._menuBar.DockCol = 0;
            this._menuBar.DockRow = 0;
            this._menuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this._menuBar.FloatLocation = new System.Drawing.Point(106, 183);
            DevExpress.XtraBars.LinksInfo linksPersistInfo = this._menuBar.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[12]
            {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._fileMenu, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._runJobsButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRunJobsRemotely, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._copyJobsButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._deleteJobsButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._exportExcelButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._viewLogButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._changeConfigurationButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._genPSMenu, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._schedTaskMenu, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._navMenu, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._removeSortButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksPersistInfo.AddRange(linkPersistInfo);
            this._menuBar.OptionsBar.AllowQuickCustomization = false;
            this._menuBar.OptionsBar.DrawBorder = false;
            this._menuBar.OptionsBar.DrawDragBorder = false;
            this._menuBar.OptionsBar.MultiLine = true;
            this._menuBar.OptionsBar.UseWholeRow = true;
            this._menuBar.StandaloneBarDockControl = this._menuBarDockLocation;
            this._menuBar.Text = "Main menu";
            this._fileMenu.ActAsDropDown = true;
            this._fileMenu.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this._fileMenu.Caption = "File";
            this._fileMenu.CloseSubMenuOnClick = false;
            this._fileMenu.DropDownControl = this._filePopupMenu;
            this._fileMenu.Glyph = Metalogix.UI.WinForms.Properties.Resources.File16;
            this._fileMenu.Id = 32;
            this._fileMenu.Name = "_fileMenu";
            DevExpress.XtraBars.LinksInfo linksInfo = this._filePopupMenu.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfoArray = new DevExpress.XtraBars.LinkPersistInfo[7]
            {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConnectToAgentDb, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._connectToDBButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._newJobListButtons, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._openJobListButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._saveJobListAsButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._importFromXmlButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._refreshButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksInfo.AddRange(linkPersistInfoArray);
            this._filePopupMenu.Manager = this._menuBarManager;
            this._filePopupMenu.Name = "_filePopupMenu";
            this._filePopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(_filePopupMenu_BeforePopup);
            this.btnConnectToAgentDb.Caption = "Connect to Agent Database...";
            this.btnConnectToAgentDb.Glyph = Metalogix.UI.WinForms.Properties.Resources.ConnectToAgentDatabase16;
            this.btnConnectToAgentDb.Id = 33;
            this.btnConnectToAgentDb.Name = "btnConnectToAgentDb";
            this.btnConnectToAgentDb.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(btnConnectToAgentDb_ItemClick);
            this._connectToDBButton.Caption = "Connect to Job Database...";
            this._connectToDBButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ConnectToJobDatabase16;
            this._connectToDBButton.Id = 33;
            this._connectToDBButton.Name = "_connectToDBButton";
            this._connectToDBButton.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_connectToDBButton_ItemClick);
            this._newJobListButtons.Caption = "New Job List...";
            this._newJobListButtons.Glyph = Metalogix.UI.WinForms.Properties.Resources.NewJobList16;
            this._newJobListButtons.Id = 34;
            this._newJobListButtons.Name = "_newJobListButtons";
            this._newJobListButtons.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_newJobListButton_ItemClick);
            this._openJobListButton.Caption = "Open Job List...";
            this._openJobListButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.OpenJobList16;
            this._openJobListButton.Id = 35;
            this._openJobListButton.Name = "_openJobListButton";
            this._openJobListButton.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_openJobListButton_ItemClick);
            this._saveJobListAsButton.Caption = "Save Job List as...";
            this._saveJobListAsButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.SaveJobList16;
            this._saveJobListAsButton.Id = 36;
            this._saveJobListAsButton.Name = "_saveJobListAsButton";
            this._saveJobListAsButton.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_saveJobListAsButton_ItemClick);
            this._importFromXmlButton.Caption = "Import Job from Xml";
            this._importFromXmlButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.document_save;
            this._importFromXmlButton.Id = 37;
            this._importFromXmlButton.Name = "_importFromXmlButton";
            this._importFromXmlButton.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_importFromXmlButton_ItemClick);
            this._refreshButton.Caption = "Refresh";
            this._refreshButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.RefreshJobList16;
            this._refreshButton.Id = 38;
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(_refreshButton_ItemClick);
            this._runJobsButton.Caption = "Run Jobs Locally";
            this._runJobsButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.RunJobs16;
            this._runJobsButton.Id = 8;
            this._runJobsButton.Name = "_runJobsButton";
            this._runJobsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_runJobsButton_ItemClick);
            this.btnRunJobsRemotely.Caption = "Run Jobs Remotely";
            this.btnRunJobsRemotely.Glyph = Metalogix.UI.WinForms.Properties.Resources.RunJobsRemotely16;
            this.btnRunJobsRemotely.Id = 8;
            this.btnRunJobsRemotely.Name = "btnRunJobsRemotely";
            this.btnRunJobsRemotely.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnRunJobsRemotely_ItemClick);
            this._copyJobsButton.Caption = "Copy Jobs";
            this._copyJobsButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.CopySelectedJobsToClipboard16;
            this._copyJobsButton.Id = 27;
            this._copyJobsButton.Name = "_copyJobsButton";
            this._copyJobsButton.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this._copyJobsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_copyJobsButton_ItemClick);
            this._deleteJobsButton.Caption = "Delete Jobs";
            this._deleteJobsButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.DeleteJobs16;
            this._deleteJobsButton.Id = 9;
            this._deleteJobsButton.Name = "_deleteJobsButton";
            this._deleteJobsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_deleteJobsButton_ItemClick);
            this._exportExcelButton.Caption = "Export Excel";
            this._exportExcelButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ExportToExcel16;
            this._exportExcelButton.Id = 26;
            this._exportExcelButton.Name = "_exportExcelButton";
            this._exportExcelButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_exportExcelButton_ItemClick);
            this._viewLogButton.Caption = "View Log";
            this._viewLogButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ViewLogs16;
            this._viewLogButton.Id = 10;
            this._viewLogButton.Name = "_viewLogButton";
            this._viewLogButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_viewLogButton_ItemClick);
            this._changeConfigurationButton.Caption = "Change Configuration";
            this._changeConfigurationButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ChangeConfiguration16;
            this._changeConfigurationButton.Id = 11;
            this._changeConfigurationButton.Name = "_changeConfigurationButton";
            this._changeConfigurationButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_changeConfigurationButton_ItemClick);
            this._genPSMenu.ActAsDropDown = true;
            this._genPSMenu.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this._genPSMenu.Caption = "Generate PowerShell Script";
            this._genPSMenu.DropDownControl = this._genPSPopupMenu;
            this._genPSMenu.Glyph = Metalogix.UI.WinForms.Properties.Resources.GeneratePowershellScript16;
            this._genPSMenu.Id = 42;
            this._genPSMenu.Name = "_genPSMenu";
            DevExpress.XtraBars.LinksInfo linksPersistInfo1 = this._genPSPopupMenu.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo1 = new DevExpress.XtraBars.LinkPersistInfo[3]
            {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._genPSUserButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._genPSMachineButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._genPSCertificateButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksPersistInfo1.AddRange(linkPersistInfo1);
            this._genPSPopupMenu.Manager = this._menuBarManager;
            this._genPSPopupMenu.Name = "_genPSPopupMenu";
            this._genPSUserButton.Caption = "For Current User and Machine";
            this._genPSUserButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ForCurrentUser16;
            this._genPSUserButton.Id = 39;
            this._genPSUserButton.Name = "_genPSUserButton";
            this._genPSUserButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_genPSUserButton_ItemClick);
            this._genPSMachineButton.Caption = "For Current Machine";
            this._genPSMachineButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ForCurrentMachine16;
            this._genPSMachineButton.Id = 40;
            this._genPSMachineButton.Name = "_genPSMachineButton";
            this._genPSMachineButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_genPSMachineButton_ItemClick);
            this._genPSCertificateButton.Caption = "For Certificate";
            this._genPSCertificateButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Certificate16;
            this._genPSCertificateButton.Id = 41;
            this._genPSCertificateButton.Name = "_genPSCertificateButton";
            this._genPSCertificateButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_genPSCertificateButton_ItemClick);
            this._schedTaskMenu.ActAsDropDown = true;
            this._schedTaskMenu.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this._schedTaskMenu.Caption = "Schedule Task";
            this._schedTaskMenu.DropDownControl = this._scheduleTaskPopupMenu;
            this._schedTaskMenu.Glyph = Metalogix.UI.WinForms.Properties.Resources.ScheduleTask16;
            this._schedTaskMenu.Id = 46;
            this._schedTaskMenu.Name = "_schedTaskMenu";
            DevExpress.XtraBars.LinksInfo linksInfo1 = this._scheduleTaskPopupMenu.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfoArray1 = new DevExpress.XtraBars.LinkPersistInfo[4]
            {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._schedUserButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._schedMachineButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._schedCertificateButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._launchTaskSchedulerButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksInfo1.AddRange(linkPersistInfoArray1);
            this._scheduleTaskPopupMenu.Manager = this._menuBarManager;
            this._scheduleTaskPopupMenu.Name = "_scheduleTaskPopupMenu";
            this._schedUserButton.Caption = "For Current User and Machine";
            this._schedUserButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ForCurrentUser16;
            this._schedUserButton.Id = 43;
            this._schedUserButton.Name = "_schedUserButton";
            this._schedUserButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_schedUserButton_ItemClick);
            this._schedMachineButton.Caption = "For Current Machine";
            this._schedMachineButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ForCurrentMachine16;
            this._schedMachineButton.Id = 44;
            this._schedMachineButton.Name = "_schedMachineButton";
            this._schedMachineButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_schedMachineButton_ItemClick);
            this._schedCertificateButton.Caption = "For Certificate";
            this._schedCertificateButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Certificate16;
            this._schedCertificateButton.Id = 45;
            this._schedCertificateButton.Name = "_schedCertificateButton";
            this._schedCertificateButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_schedCertificateButton_ItemClick);
            this._launchTaskSchedulerButton.Caption = "View Task Scheduler";
            this._launchTaskSchedulerButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ScheduleTask16;
            this._launchTaskSchedulerButton.Id = 50;
            this._launchTaskSchedulerButton.Name = "_launchTaskSchedulerButton";
            this._launchTaskSchedulerButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_launchTaskSchedulerButton_ItemClick);
            this._navMenu.ActAsDropDown = true;
            this._navMenu.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this._navMenu.Caption = "Navigate";
            this._navMenu.DropDownControl = this._navigatePopupMenu;
            this._navMenu.Glyph = Metalogix.UI.WinForms.Properties.Resources.Navigate16;
            this._navMenu.Id = 49;
            this._navMenu.Name = "_navMenu";
            DevExpress.XtraBars.LinksInfo linksPersistInfo2 = this._navigatePopupMenu.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo2 = new DevExpress.XtraBars.LinkPersistInfo[2]
            {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._navSourceButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._navTargetButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksPersistInfo2.AddRange(linkPersistInfo2);
            this._navigatePopupMenu.Manager = this._menuBarManager;
            this._navigatePopupMenu.Name = "_navigatePopupMenu";
            this._navSourceButton.Caption = "To Source";
            this._navSourceButton.Id = 47;
            this._navSourceButton.Name = "_navSourceButton";
            this._navSourceButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_navSourceButton_ItemClick);
            this._navTargetButton.Caption = "To Target";
            this._navTargetButton.Id = 48;
            this._navTargetButton.Name = "_navTargetButton";
            this._navTargetButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_navTargetButton_ItemClick);
            this._removeSortButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this._removeSortButton.Caption = "Remove Sort";
            this._removeSortButton.Enabled = false;
            this._removeSortButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.RemoveSort16;
            this._removeSortButton.Id = 28;
            this._removeSortButton.Name = "_removeSortButton";
            this._removeSortButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_removeSortButton_ItemClick);
            this._menuBarDockLocation.CausesValidation = false;
            this._menuBarDockLocation.Dock = System.Windows.Forms.DockStyle.Top;
            this._menuBarDockLocation.Location = new System.Drawing.Point(0, 18);
            this._menuBarDockLocation.Name = "_menuBarDockLocation";
            this._menuBarDockLocation.Size = new System.Drawing.Size(1379, 25);
            this._menuBarDockLocation.Text = "standaloneBarDockControl1";
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1379, 0);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 295);
            this.barDockControlBottom.Size = new System.Drawing.Size(1379, 0);
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 295);
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1379, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 295);
            this.barButtonItem1.Caption = "asdfawer";
            this.barButtonItem1.Id = 30;
            this.barButtonItem1.Name = "barButtonItem1";
            this._jobList.AutoScrollToNewItem = true;
            this._jobList.DataSource = null;
            this._jobList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._jobList.Location = new System.Drawing.Point(0, 43);
            this._jobList.Name = "_jobList";
            this._jobList.SelectedJobs = new Metalogix.Jobs.Job[0];
            this._jobList.ShowLicenseUsage = true;
            this._jobList.Size = new System.Drawing.Size(1379, 252);
            this._jobList.TabIndex = 5;
            this._jobList.SelectedJobChanged += new Metalogix.UI.WinForms.Jobs.JobListControl.SelectedJobChangedHandler(_jobList_SelectedJobChanged);
            this._jobList.SortingChanged += new Metalogix.UI.WinForms.Jobs.JobListControl.SortOrderChanged(_jobList_SortingChanged);
            this._jobList.JobsUpdated += new Metalogix.UI.WinForms.Jobs.JobListControl.JobsUpdatedHandler(_jobList_JobsUpdated);
            base.Appearance.BackColor = System.Drawing.SystemColors.Control;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this._jobList);
            base.Controls.Add(this._menuBarDockLocation);
            base.Controls.Add(this._jobSourcePanel);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.Name = "JobListFullControl";
            base.Size = new System.Drawing.Size(1379, 295);
            this._jobSourcePanel.ResumeLayout(false);
            this._jobSourcePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._jobSourceValue.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._menuBarManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._filePopupMenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._genPSPopupMenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._scheduleTaskPopupMenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._navigatePopupMenu).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeDataSource()
        {
            ReLoadJobsList = false;
            if (ApplicationData.IsDesignMode())
            {
                return;
            }
            if (!string.IsNullOrEmpty(JobsSettings.AdapterType))
            {
                try
                {
                    if (JobsSettings.AdapterContext.IsNullOrEmpty())
                    {
                        ReLoadJobsList = true;
                    }
                    else
                    {
                        DataSource = JobFactory.CreateJobHistoryDb(JobsSettings.AdapterType, JobsSettings.AdapterContext.ToInsecureString());
                    }
                    return;
                }
                catch (CryptographicException)
                {
                    ReLoadJobsList = true;
                    return;
                }
                catch (Exception exception)
                {
                    GlobalServices.ErrorHandler.AsyncHandleException(string.Format(Resources.UnableToLoadJobList, exception.Message), exception);
                    return;
                }
            }
            try
            {
                DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, JobsSettings.CurrentJobHistoryFile);
            }
            catch (Exception exception2)
            {
                GlobalServices.ErrorHandler.AsyncHandleException(string.Format(Resources.UnableToLoadJobList, exception2.Message), exception2);
            }
        }

        private void On_Jobs_ListChanged(ChangeType changeType, Job[] itemsChanged)
        {
            if (changeType != ChangeType.ItemUpdated)
            {
                UpdateUI();
            }
        }

        private void On_mruButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ShowDisconnectAgentDialog())
            {
                return;
            }
            string tag = (string)e.Item.Tag;
            try
            {
                bool flag = JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
                PleaseWaitDialog.ShowWaitDialog(string.Format(Resources.FS_OpeningJobWaitMessage, tag), delegate
                {
                    if (!File.Exists(tag))
                    {
                        throw new ConditionalDetailException("The file '" + tag + "' does not exist");
                    }
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, tag);
                }, cancellable: false);
                if (flag)
                {
                    SaveAllSettingsToFileSystem();
                    FlatXtraMessageBox.Show(_restartApplicationWarningForDBToFileSystem, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                JobsSettings.JobHistoryMRU.Delete(tag);
                GlobalServices.ErrorHandler.HandleException("Cannot open file: " + exception.Message, exception);
            }
        }

        private void OnActionClick(Metalogix.Actions.Action action)
        {
            CommonSerializableList<JobListControl> commonSerializableList = new CommonSerializableList<JobListControl> { _jobList };
            ActionPaletteControl.ActionClick(action, commonSerializableList, _jobList.SelectedObjects);
        }

        private void OpenJobListFile()
        {
            if (ShowDisconnectAgentDialog())
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open job list file",
                Filter = "Job List files (*.lst)|*.lst",
                InitialDirectory = ApplicationData.ApplicationPath
            };
            OpenFileDialog openFileDialog1 = openFileDialog;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = openFileDialog1.FileName;
            try
            {
                bool flag = JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
                PleaseWaitDialog.ShowWaitDialog(string.Format(Resources.FS_OpeningJobWaitMessage, fileName), delegate
                {
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, fileName);
                }, cancellable: false);
                if (flag)
                {
                    SaveAllSettingsToFileSystem();
                    FlatXtraMessageBox.Show(_restartApplicationWarningForDBToFileSystem, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException("Invalid job file: " + exception.Message, exception);
            }
        }

        private void RunJobListAction(Metalogix.Actions.Action action)
        {
            CommonSerializableList<JobListControl> commonSerializableList = new CommonSerializableList<JobListControl> { _jobList };
            action.Run(commonSerializableList, _jobList.SelectedObjects);
        }

        public void SaveAllSettingsToDb(string connectionString)
        {
            SaveValuesToDb(ResourceScope.EnvironmentSpecific, connectionString);
            SaveValuesToDb(ResourceScope.ApplicationAndUserSpecific, connectionString);
            SaveValuesToDb(ResourceScope.UserSpecific, connectionString);
        }

        private void SaveAllSettingsToFileSystem()
        {
            if (FlatXtraMessageBox.Show(Resources.ConfigurationSettingsAgentDatabaseToFileSystem, Resources.ConfigurationSettings, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string str = Path.Combine(ApplicationData.CommonDataPath, ConfigurationVariables.EnvironmentVariables.Name + ".xml");
                SaveValuesToFileSystem(ResourceScope.EnvironmentSpecific, str);
                string str1 = Path.Combine(ApplicationData.ApplicationPath, ConfigurationVariables.ApplicationAndUserSpecificVariables.Name + ".xml");
                SaveValuesToFileSystem(ResourceScope.ApplicationAndUserSpecific, str1);
                string str2 = Path.Combine(ApplicationData.CompanyPath, ConfigurationVariables.UserSpecificVariables.Name + ".xml");
                SaveValuesToFileSystem(ResourceScope.UserSpecific, str2);
            }
        }

        private void SaveJobListAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save job list file as",
                Filter = "Job List files (*.lst)|*.lst",
                InitialDirectory = ApplicationData.ApplicationPath
            };
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = saveFileDialog.FileName;
            if (string.Equals(DataSource.AdapterContext, fileName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            try
            {
                PleaseWaitDialog.ShowWaitDialog(string.Format(Resources.FS_SaveJobWaitMessage, fileName), delegate
                {
                    File.Copy(DataSource.AdapterContext, fileName, overwrite: true);
                    DataSource = JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, fileName);
                }, cancellable: false);
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
            }
        }

        public void SaveValuesToDb(ResourceScope resourceScope, string connectionString, List<string> settings = null)
        {
            DatabaseTableDataResolverOptions databaseTableDataResolverOption = new DatabaseTableDataResolverOptions
            {
                ConnectionString = connectionString,
                Scope = resourceScope.ToString()
            };
            CopyValuesToTargetResolver(resourceScope, new DatabaseTableDataResolver(databaseTableDataResolverOption), settings);
        }

        private void SaveValuesToFileSystem(ResourceScope resourceScope, string filePath)
        {
            CopyValuesToTargetResolver(resourceScope, new FileTableDataResolver(filePath));
        }

        private bool ShowDisconnectAgentDialog()
        {
            if (DataSource != null && DataSource.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                if (FlatXtraMessageBox.Show(Resources.DisconnectAgentDatabaseMessage, Resources.DisconnectAgentDatabase, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return true;
                }
                DefaultResolverSettings.SaveSetting(_resourceTableKeyName, typeof(ResourceFileTableResolver).AssemblyQualifiedName);
            }
            RemoteJobScheduler.Instance = null;
            return false;
        }

        private void UpdateUI()
        {
            if (base.InvokeRequired)
            {
                Invoke(new UpdateUIDelegate(UpdateUI));
                return;
            }
            bool hasRunningSelectedAction = _jobList.HasRunningSelectedAction;
            bool hasPausedSelectedAction = _jobList.HasPausedSelectedAction;
            bool hasRunningAction = _jobList.HasRunningAction;
            Job[] selectedJobs = _jobList.SelectedJobs;
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            if (selectedJobs.Length == 0 || selectedJobs[0].Action == null)
            {
                flag3 = false;
                flag1 = false;
                flag2 = false;
            }
            else
            {
                Type type = selectedJobs[0].Action.GetType();
                Job[] jobArray = selectedJobs;
                foreach (Job job in jobArray)
                {
                    if (job.Action == null)
                    {
                        flag1 = false;
                        flag2 = false;
                        flag3 = false;
                        break;
                    }
                    if (job.Action.GetType() != type)
                    {
                        flag1 = false;
                        if (!job.Action.CmdletEnabled)
                        {
                            flag2 = false;
                        }
                        break;
                    }
                    if (!job.Action.CmdletEnabled)
                    {
                        flag2 = false;
                        break;
                    }
                }
            }
            bool length = selectedJobs.Length == 1;
            BarButtonItem barButtonItem = _navMenu;
            bool flag = length && (!string.IsNullOrEmpty(selectedJobs[0].Source) || !string.IsNullOrEmpty(selectedJobs[0].Target));
            barButtonItem.Enabled = flag;
            if (length)
            {
                _navSourceButton.Enabled = !string.IsNullOrEmpty(selectedJobs[0].Source);
                _navTargetButton.Enabled = !string.IsNullOrEmpty(selectedJobs[0].Target);
            }
            bool flag4 = DataSource.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase);
            _changeConfigurationButton.Enabled = flag1;
            _genPSMenu.Enabled = flag2;
            _schedCertificateButton.Enabled = flag2;
            _schedMachineButton.Enabled = flag2;
            _schedUserButton.Enabled = flag2;
            _runJobsButton.Enabled = !(selectedJobs.Length == 0 || hasRunningSelectedAction) && flag3 && !hasPausedSelectedAction;
            btnRunJobsRemotely.Enabled = flag2 && selectedJobs.Length != 0 && flag4;
            _deleteJobsButton.Enabled = !(selectedJobs.Length == 0 || hasRunningSelectedAction) && !hasPausedSelectedAction;
            _exportExcelButton.Enabled = selectedJobs.Length != 0 && !hasRunningSelectedAction;
            _copyJobsButton.Enabled = selectedJobs.Length != 0;
            _viewLogButton.Enabled = selectedJobs.Length != 0;
            _jobSourceLabel.Text = ((DataSource == null) ? "Job Source" : "Job Source: ");
            string str = ((DataSource == null) ? "" : JobUtils.MaskAdapterContext(DataSource.AdapterContext));
            _jobSourceValue.Text = ((!flag4) ? str : $"{str} | AgentDB: Yes");
            _fileMenu.Enabled = !hasRunningAction;
        }
    }
}
