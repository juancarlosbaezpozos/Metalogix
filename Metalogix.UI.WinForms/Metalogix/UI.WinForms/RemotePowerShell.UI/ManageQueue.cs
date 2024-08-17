using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class ManageQueue : XtraForm, IHasSelectableObjects
    {
        private delegate void ReloadUIDelegate();

        private const int UpdateClockDelay = 10000;

        private JobCollection _jobCollection;

        private readonly ReaderWriterLockSlim _uiUpdateLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private System.Timers.Timer _updateClock;

        private DevExpress.XtraEditors.VScrollBar _verticalScrollBar;

        private IContainer components;

        private XtraBarManagerWithArrows _menuBarManager;

        private Bar _menuBar;

        private BarButtonItem btnDeleteJob;

        private BarButtonItem btnRefreshJob;

        private StandaloneBarDockControl _menuBarDockLocation;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private XtraParentSelectableGrid gridJobParentSelectable;

        private GridView gridJobs;

        private GridColumn colJobID;

        private GridColumn colJobName;

        private GridColumn colCreated;

        private GridColumn colSourceUrl;

        private GridColumn colMachineName;

        private GridColumn colStatus;

        private GridColumn colTargetUrl;

        private SimpleButton btnClose;

        public JobCollection JobCollection
        {
            get
		{
			if (_jobCollection == null)
			{
				JobHistoryDb jobHistoryDb = JobFactory.CreateJobHistoryDb(JobsSettings.AdapterType, JobsSettings.AdapterContext.ToInsecureString());
				_jobCollection = new JobCollection(jobHistoryDb);
			}
			return _jobCollection;
		}
        }

        public List<Job> Jobs => RemoteJobScheduler.Instance.Jobs;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Job[] SelectedJobs => GetCurrentlySelectedItems().ToArray();

        public IXMLAbleList SelectedObjects => new CommonSerializableList<Job>(SelectedJobs);

        public ManageQueue()
	{
		InitializeComponent();
		LoadGrid();
		RemoteJobScheduler.Instance.JobListChanged += Jobs_JobListChanged;
		InitializeClock();
		InitializeVerticalScrollbar();
	}

        private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
	}

        private void btnDeleteJob_ItemClick(object sender, ItemClickEventArgs e)
	{
		StopTimer();
		OnActionClick(new DeleteJobAction());
		StartTimer();
	}

        private void btnRefreshJob_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new RefreshJobAction());
	}

        private void DetachGridSource()
	{
		gridJobParentSelectable.DataSource = null;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private List<Job> GetCurrentlySelectedItems()
	{
		int[] selectedRows = gridJobs.GetSelectedRows();
		if (selectedRows == null)
		{
			return new List<Job>();
		}
		List<Job> jobs = new List<Job>(selectedRows.Length);
		int[] numArray = selectedRows;
		foreach (int num in numArray)
		{
			if (gridJobs.GetRow(num) is DataRowView row)
			{
				Job job = JobCollection.GetJob(Convert.ToString(row["JobID"]));
				if (job != null)
				{
					jobs.Add(job);
				}
			}
		}
		return jobs;
	}

        private DataTable GetDataTable(List<Job> jobs)
	{
		if (jobs == null)
		{
			return null;
		}
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add(Constants.JobID);
		dataTable.Columns.Add(Constants.Title);
		dataTable.Columns.Add(Constants.Created);
		dataTable.Columns.Add(Constants.Source);
		dataTable.Columns.Add(Constants.Target);
		dataTable.Columns.Add(Constants.MachineName);
		dataTable.Columns.Add(Constants.Status);
		foreach (Job job in jobs)
		{
			string machineName = "-";
			if (!job.Status.Equals(ActionStatus.NotRunning) && !job.Status.Equals(ActionStatus.Queued))
			{
				machineName = job.MachineName;
			}
			DataRowCollection rows = dataTable.Rows;
			object[] jobID = new object[7] { job.JobID, job.Title, job.Created, job.Source, job.Target, machineName, job.Status };
			rows.Add(jobID);
		}
		return dataTable;
	}

        private void gridJobs_RowClick(object sender, EventArgs e)
	{
		UpdateUI();
	}

        private void InitializeClock()
	{
		_updateClock = new System.Timers.Timer(10000.0);
		_updateClock.Elapsed += UpdateClockTick;
		if (IsAnyJobPending())
		{
			StartTimer();
		}
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this._menuBarManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.ManageQueue));
		this._menuBar = new DevExpress.XtraBars.Bar();
		this.btnDeleteJob = new DevExpress.XtraBars.BarButtonItem();
		this.btnRefreshJob = new DevExpress.XtraBars.BarButtonItem();
		this._menuBarDockLocation = new DevExpress.XtraBars.StandaloneBarDockControl();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.gridJobParentSelectable = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
		this.gridJobs = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.colJobID = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colJobName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colCreated = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colSourceUrl = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colTargetUrl = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colMachineName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
		this.btnClose = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridJobParentSelectable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridJobs).BeginInit();
		base.SuspendLayout();
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
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[2] { this.btnDeleteJob, this.btnRefreshJob };
		items.AddRange(barItemArray);
		this._menuBarManager.MainMenu = this._menuBar;
		this._menuBarManager.MaxItemId = 57;
		this._menuBar.BarItemHorzIndent = 3;
		this._menuBar.BarName = "Main menu";
		this._menuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Standalone;
		this._menuBar.DockCol = 0;
		this._menuBar.DockRow = 0;
		this._menuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
		this._menuBar.FloatLocation = new System.Drawing.Point(106, 183);
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this._menuBar.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDeleteJob, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRefreshJob, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this._menuBar.OptionsBar.AllowQuickCustomization = false;
		this._menuBar.OptionsBar.DrawBorder = false;
		this._menuBar.OptionsBar.DrawDragBorder = false;
		this._menuBar.OptionsBar.MultiLine = true;
		this._menuBar.OptionsBar.UseWholeRow = true;
		this._menuBar.StandaloneBarDockControl = this._menuBarDockLocation;
		this._menuBar.Text = "Main menu";
		this.btnDeleteJob.Caption = "Delete Job";
		this.btnDeleteJob.Glyph = Metalogix.UI.WinForms.Properties.Resources.Delete16;
		this.btnDeleteJob.Id = 8;
		this.btnDeleteJob.Name = "btnDeleteJob";
		this.btnDeleteJob.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnDeleteJob_ItemClick);
		this.btnRefreshJob.Caption = "Refresh";
		this.btnRefreshJob.Glyph = Metalogix.UI.WinForms.Properties.Resources.RefreshButton1;
		this.btnRefreshJob.Id = 8;
		this.btnRefreshJob.Name = "btnRefreshJob";
		this.btnRefreshJob.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnRefreshJob_ItemClick);
		this._menuBarDockLocation.CausesValidation = false;
		this._menuBarDockLocation.Dock = System.Windows.Forms.DockStyle.Top;
		this._menuBarDockLocation.Location = new System.Drawing.Point(0, 0);
		this._menuBarDockLocation.Name = "_menuBarDockLocation";
		this._menuBarDockLocation.Size = new System.Drawing.Size(910, 25);
		this._menuBarDockLocation.Text = "standaloneBarDockControl1";
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Size = new System.Drawing.Size(910, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 316);
		this.barDockControlBottom.Size = new System.Drawing.Size(910, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 316);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(910, 0);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 316);
		this.gridJobParentSelectable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridJobParentSelectable.Location = new System.Drawing.Point(0, 25);
		this.gridJobParentSelectable.MainView = this.gridJobs;
		this.gridJobParentSelectable.MenuManager = this._menuBarManager;
		this.gridJobParentSelectable.Name = "gridJobParentSelectable";
		this.gridJobParentSelectable.Size = new System.Drawing.Size(910, 255);
		this.gridJobParentSelectable.TabIndex = 8;
		this.gridJobParentSelectable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridJobs });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gridJobs.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray = new DevExpress.XtraGrid.Columns.GridColumn[7] { this.colJobID, this.colJobName, this.colCreated, this.colSourceUrl, this.colTargetUrl, this.colMachineName, this.colStatus };
		columns.AddRange(gridColumnArray);
		this.gridJobs.GridControl = this.gridJobParentSelectable;
		this.gridJobs.Name = "gridJobs";
		this.gridJobs.OptionsMenu.EnableColumnMenu = false;
		this.gridJobs.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridJobs.OptionsView.ShowGroupPanel = false;
		this.gridJobs.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
		this.gridJobs.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(gridJobs_RowClick);
		this.gridJobs.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(gridJobs_RowClick);
		this.colJobID.Caption = "Job ID";
		this.colJobID.FieldName = "JobID";
		this.colJobID.Name = "colJobID";
		this.colJobID.OptionsColumn.AllowEdit = false;
		this.colJobID.OptionsColumn.AllowMove = false;
		this.colJobID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.colJobID.OptionsFilter.AllowFilter = false;
		this.colJobID.Visible = true;
		this.colJobID.VisibleIndex = 0;
		this.colJobID.Width = 116;
		this.colJobName.Caption = "Job Name";
		this.colJobName.FieldName = "Title";
		this.colJobName.Name = "colJobName";
		this.colJobName.OptionsColumn.AllowEdit = false;
		this.colJobName.OptionsColumn.AllowMove = false;
		this.colJobName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.colJobName.OptionsFilter.AllowFilter = false;
		this.colJobName.Visible = true;
		this.colJobName.VisibleIndex = 1;
		this.colJobName.Width = 130;
		this.colCreated.Caption = "Created";
		this.colCreated.FieldName = "Created";
		this.colCreated.Name = "colCreated";
		this.colCreated.OptionsColumn.AllowEdit = false;
		this.colCreated.OptionsColumn.AllowMove = false;
		this.colCreated.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colCreated.OptionsFilter.AllowFilter = false;
		this.colCreated.Visible = true;
		this.colCreated.VisibleIndex = 2;
		this.colCreated.Width = 130;
		this.colSourceUrl.Caption = "Source Url";
		this.colSourceUrl.FieldName = "Source";
		this.colSourceUrl.Name = "colSourceUrl";
		this.colSourceUrl.OptionsColumn.AllowEdit = false;
		this.colSourceUrl.OptionsColumn.AllowMove = false;
		this.colSourceUrl.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colSourceUrl.OptionsFilter.AllowFilter = false;
		this.colSourceUrl.Visible = true;
		this.colSourceUrl.VisibleIndex = 3;
		this.colSourceUrl.Width = 135;
		this.colTargetUrl.Caption = "Target Url";
		this.colTargetUrl.FieldName = "Target";
		this.colTargetUrl.Name = "colTargetUrl";
		this.colTargetUrl.OptionsColumn.AllowEdit = false;
		this.colTargetUrl.OptionsColumn.AllowMove = false;
		this.colTargetUrl.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colTargetUrl.OptionsFilter.AllowFilter = false;
		this.colTargetUrl.Visible = true;
		this.colTargetUrl.VisibleIndex = 4;
		this.colTargetUrl.Width = 142;
		this.colMachineName.Caption = "Agent Name";
		this.colMachineName.FieldName = "MachineName";
		this.colMachineName.Name = "colMachineName";
		this.colMachineName.OptionsColumn.AllowEdit = false;
		this.colMachineName.OptionsColumn.AllowMove = false;
		this.colMachineName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colMachineName.OptionsFilter.AllowFilter = false;
		this.colMachineName.Visible = true;
		this.colMachineName.VisibleIndex = 5;
		this.colMachineName.Width = 116;
		this.colStatus.Caption = "Status";
		this.colStatus.FieldName = "Status";
		this.colStatus.Name = "colStatus";
		this.colStatus.OptionsColumn.AllowEdit = false;
		this.colStatus.OptionsColumn.AllowMove = false;
		this.colStatus.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colStatus.OptionsFilter.AllowFilter = false;
		this.colStatus.Visible = true;
		this.colStatus.VisibleIndex = 6;
		this.colStatus.Width = 123;
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnClose.Location = new System.Drawing.Point(823, 286);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(75, 23);
		this.btnClose.TabIndex = 14;
		this.btnClose.Text = "Close";
		this.btnClose.Click += new System.EventHandler(btnClose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnClose;
		base.ClientSize = new System.Drawing.Size(910, 316);
		base.Controls.Add(this.btnClose);
		base.Controls.Add(this.gridJobParentSelectable);
		base.Controls.Add(this._menuBarDockLocation);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(500, 250);
		base.Name = "ManageQueue";
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Manage Queue";
		base.Enter += new System.EventHandler(ManageQueue_Enter);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ManageQueue_FormClosing);
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridJobParentSelectable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridJobs).EndInit();
		base.ResumeLayout(false);
	}

        private void InitializeVerticalScrollbar()
	{
		if (gridJobParentSelectable.Controls[1] is DevExpress.XtraEditors.VScrollBar)
		{
			_verticalScrollBar = (DevExpress.XtraEditors.VScrollBar)gridJobParentSelectable.Controls[1];
		}
	}

        private bool IsAnyJobPending()
	{
		return Jobs.Any((Job job) => job.Status == ActionStatus.Queued || job.Status == ActionStatus.Running);
	}

        private bool IsJobRunning()
	{
		string str = Convert.ToString(gridJobs.GetFocusedRowCellValue(Constants.Status));
		if (!string.IsNullOrEmpty(str) && str.Equals(ActionStatus.Running.ToString(), StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		return false;
	}

        private void Jobs_JobListChanged(object sender, EventArgs e)
	{
		ReLoadUI();
		UpdateUI();
	}

        private void LoadGrid()
	{
		try
		{
			gridJobs.ClearSelection();
			gridJobParentSelectable.DataSource = null;
			List<Job> jobs = Jobs;
			if (jobs != null)
			{
				DataTable dataTable = GetDataTable(jobs);
				if (dataTable != null)
				{
					gridJobParentSelectable.DataSource = dataTable;
				}
			}
			UpdateUI();
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			string str = "An error occurred while retrieving queued jobs.";
			GlobalServices.ErrorHandler.HandleException("Manage Queue", str, exception, ErrorIcon.Error);
			Logging.LogExceptionToTextFileWithEventLogBackup(exception, str);
		}
	}

        private void ManageQueue_Enter(object sender, EventArgs e)
	{
		base.StartPosition = FormStartPosition.CenterParent;
	}

        private void ManageQueue_FormClosing(object sender, FormClosingEventArgs e)
	{
		StopTimer();
	}

        private void OnActionClick(Metalogix.Actions.Action action)
	{
		CommonSerializableList<JobCollection> commonSerializableList = new CommonSerializableList<JobCollection> { JobCollection };
		ActionPaletteControl.ActionClick(action, commonSerializableList, SelectedObjects);
	}

        private void ReattachGridSource()
	{
		if (gridJobParentSelectable.DataSource != null)
		{
			return;
		}
		List<Job> jobs = Jobs;
		if (jobs != null)
		{
			DataTable dataTable = GetDataTable(jobs);
			if (dataTable != null)
			{
				gridJobParentSelectable.DataSource = dataTable;
			}
		}
	}

        public void ReLoadUI()
	{
		CurrencyDataController.DisableThreadingProblemsDetection = true;
		if (base.InvokeRequired)
		{
			Invoke(new ReloadUIDelegate(ReLoadUI));
			return;
		}
		_uiUpdateLock.EnterWriteLock();
		try
		{
			int num = ((_verticalScrollBar != null) ? _verticalScrollBar.Value : (-1));
			int focusedRowHandle = gridJobs.FocusedRowHandle;
			SuspendLayout();
			gridJobParentSelectable.BeginUpdate();
			try
			{
				if (!IsAnyJobPending())
				{
					StopTimer();
				}
				else
				{
					StartTimer();
				}
				DetachGridSource();
				ReattachGridSource();
				gridJobs.ClearSelection();
			}
			finally
			{
				ReattachGridSource();
				gridJobParentSelectable.EndUpdate();
				if (focusedRowHandle != gridJobs.RowCount)
				{
					gridJobs.FocusedRowHandle = focusedRowHandle;
				}
				else
				{
					gridJobs.FocusedRowHandle = focusedRowHandle - 1;
				}
				if (_verticalScrollBar != null && num != -1)
				{
					_verticalScrollBar.Value = num;
				}
				ResumeLayout();
			}
		}
		finally
		{
			_uiUpdateLock.ExitWriteLock();
		}
	}

        private void StartTimer()
	{
		if (!_updateClock.Enabled)
		{
			_updateClock.Start();
		}
	}

        private void StopTimer()
	{
		if (_updateClock.Enabled)
		{
			_updateClock.Stop();
		}
	}

        private void UpdateClockTick(object sender, ElapsedEventArgs e)
	{
		lock (_updateClock)
		{
			if (!base.IsDisposed)
			{
				RemoteJobScheduler.Instance.RefreshJobs();
			}
		}
	}

        private void UpdateUI()
	{
		bool count = Jobs.Count > 0;
		btnDeleteJob.Enabled = count && !IsJobRunning();
		btnRefreshJob.Enabled = count;
	}
    }
}
