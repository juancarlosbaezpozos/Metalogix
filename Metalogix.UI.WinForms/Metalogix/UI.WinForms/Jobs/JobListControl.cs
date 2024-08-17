#define TRACE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Jobs.Actions;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Jobs
{
    public class JobListControl : XtraUserControl, IHasSelectableObjects
    {
        private class DisplayedJob
        {
            public Job Job { get; set; }

            public JobListControl Parent { get; set; }

            public DataRow Row { get; set; }

            private string CalculateDuration()
		{
			DateTime? started = Job.Started;
			DateTime? finished = Job.Finished;
			if (!started.HasValue || !finished.HasValue)
			{
				return null;
			}
			TimeSpan timeSpan = DurationCalculator.CalculateDuration(started.Value, finished.Value);
			return DurationFormatter.FormatDuration(timeSpan);
		}

            private void EnsureRowExists()
		{
			if (Row == null)
			{
				Row = Parent._table.NewRow();
				Parent._table.Rows.Add(Row);
				Parent._dataRowMap.Add(Row, this);
			}
		}

            public void UpdateRow()
		{
			EnsureRowExists();
			CurrencyDataController.DisableThreadingProblemsDetection = true;
			Row["Title"] = Job.Title;
			Row["Source"] = Job.Source;
			Row["Target"] = Job.Target;
			if (Job.Started.HasValue)
			{
				Row["Started"] = Job.Started.Value;
			}
			else
			{
				Row["Started"] = DBNull.Value;
			}
			Row["Status"] = Job.StatusMessage;
			Row["LogSummary"] = Job.ResultsSummary;
			Row["DataMigrated"] = Job.GetFormattedLicensedData();
			if (Job.Finished.HasValue)
			{
				Row["Finished"] = Job.Finished.Value;
			}
			else
			{
				Row["Finished"] = DBNull.Value;
			}
			Row["Duration"] = CalculateDuration();
			Row["UserName"] = Job.UserName;
			Row["MachineName"] = Job.MachineName;
			UpdateStatusImage();
		}

            private void UpdateStatusImage()
		{
			StatusImageMap.TryGetValue(Job.Status, out var image);
			Row["StatusImage"] = image;
		}
        }

        public delegate void JobsUpdatedHandler();

        public delegate void SelectedJobChangedHandler();

        private delegate void SelectJobsDelegate(IEnumerable<Job> jobs);

        private delegate void SetDataSourceDelegate(JobCollection value);

        public delegate void SortOrderChanged(bool sortingApplied);

        private delegate void VoidDelegate();

        private const string COLUMN_NAME_STATUSIMAGE = "StatusImage";

        private const string COLUMN_NAME_TITLE = "Title";

        private const string COLUMN_NAME_SOURCE = "Source";

        private const string COLUMN_NAME_TARGET = "Target";

        private const string COLUMN_NAME_STARTED = "Started";

        private const string COLUMN_NAME_STATUS = "Status";

        private const string COLUMN_NAME_LOGSUMMARY = "LogSummary";

        private const string COLUMN_NAME_DATAMIGRATED = "DataMigrated";

        private const string COLUMN_NAME_FINISHED = "Finished";

        private const string COLUMN_NAME_DURATION = "Duration";

        private const string COLUMN_NAME_USERNAME = "UserName";

        private const string COLUMN_NAME_MACHINENAME = "MachineName";

        private const int UPDATE_CLOCK_DELAY = 500;

        private static Dictionary<ActionStatus, Image> s_statusImageMap;

        private static readonly object s_statusImageMapLock;

        private readonly ReaderWriterLockSlim _dataSourceLock = new ReaderWriterLockSlim();

        private readonly ReaderWriterLockSlim _uiUpdateLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly ReaderWriterLockSlim _changeTrackerLock = new ReaderWriterLockSlim();

        private bool _autoScrollToNewItem = true;

        private JobCollection.ListChangedHandler _jobChangedHandler;

        private JobCollection _dataSource;

        private LogItemViewer _logItemViewer;

        private object _logItemViewerLock = new object();

        private bool _allowNextEditorPast;

        private System.Timers.Timer _updateClock;

        private bool _updateClockTicking;

        private bool _updateClockPaused;

        private readonly ThreadSafeDictionary<string, Job> _changedItems = new ThreadSafeDictionary<string, Job>();

        private readonly ThreadSafeDictionary<string, Job> _removedItems = new ThreadSafeDictionary<string, Job>();

        private volatile bool _dataSourceReset;

        private Dictionary<string, DisplayedJob> _jobMap;

        private Dictionary<DataRow, DisplayedJob> _dataRowMap;

        private DataTable _table;

        private Dictionary<string, Job> _jobsToSelect;

        private IContainer components;

        private XtraParentSelectableGrid _grid;

        private GridView _gridView;

        private GridColumn _statusImage;

        private RepositoryItemPictureEdit _statusPictureDisplay;

        private GridColumn _titleColumn;

        private GridColumn _sourceColumn;

        private GridColumn _targetColumn;

        private GridColumn _startedColumn;

        private GridColumn _statusColumn;

        private GridColumn _logSummaryColumn;

        private GridColumn _dataMigratedColumn;

        private GridColumn _finishedColumn;

        private GridColumn _durationColumn;

        private GridColumn _userNameColumn;

        private GridColumn _machineNameColumn;

        private ActionPaletteControl _actionPallet;

        private RepositoryItemTextEdit _titleColumnEditor;

        public bool AutoScrollToNewItem
        {
            get
		{
			return _autoScrollToNewItem;
		}
            set
		{
			_autoScrollToNewItem = value;
		}
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public JobCollection DataSource
        {
            get
		{
			if (!_dataSourceLock.IsWriteLockHeld)
			{
				_dataSourceLock.EnterReadLock();
			}
			try
			{
				return _dataSource;
			}
			finally
			{
				if (!_dataSourceLock.IsWriteLockHeld)
				{
					_dataSourceLock.ExitReadLock();
				}
			}
		}
            set
		{
			SetDataSource(value);
		}
        }

        public bool HasPausedSelectedAction
        {
            get
		{
			bool flag = false;
			RunInReadLock(_uiUpdateLock, delegate
			{
				Job[] selectedJobs = SelectedJobs;
				foreach (Job job in selectedJobs)
				{
					if (job.Action != null && job.Action.Status == ActionStatus.Paused)
					{
						flag = true;
						break;
					}
				}
			});
			return flag;
		}
        }

        public bool HasRunningAction => DataSource?.HasRunningActions ?? false;

        public bool HasRunningSelectedAction
        {
            get
		{
			bool flag = false;
			RunInReadLock(_uiUpdateLock, delegate
			{
				Job[] selectedJobs = SelectedJobs;
				foreach (Job job in selectedJobs)
				{
					if (job.Action != null && (job.Action.Status == ActionStatus.Running || job.Action.Status == ActionStatus.Aborting))
					{
						flag = true;
						break;
					}
				}
			});
			return flag;
		}
        }

        internal LogItemViewer LogItemViewer
        {
            get
		{
			LogItemViewer logItemViewer = _logItemViewer;
			if (logItemViewer == null || logItemViewer.IsDisposed)
			{
				lock (_logItemViewerLock)
				{
					if (_logItemViewer == null || _logItemViewer.IsDisposed)
					{
						_logItemViewer = new LogItemViewer();
						int x = base.ParentForm.Location.X;
						int width = base.ParentForm.Size.Width;
						int num = x + (width - _logItemViewer.Size.Width) / 2;
						int y = base.ParentForm.Location.Y;
						int height = base.ParentForm.Size.Height;
						int height1 = y + (height - _logItemViewer.Size.Height) / 2;
						_logItemViewer.StartLocation = new Point(num, height1);
					}
					logItemViewer = _logItemViewer;
				}
			}
			return logItemViewer;
		}
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Job[] SelectedJobs
        {
            get
		{
			List<Job> jobs = new List<Job>();
			RunInReadLock(_uiUpdateLock, delegate
			{
				jobs = GetCurrentlySelectedItems();
			});
			return jobs.ToArray();
		}
            set
		{
			SelectJobs(value);
		}
        }

        public IXMLAbleList SelectedObjects => new CommonSerializableList<Job>(SelectedJobs);

        public bool ShowLicenseUsage
        {
            get
		{
			return _dataMigratedColumn.Visible;
		}
            set
		{
			_dataMigratedColumn.Visible = value;
		}
        }

        protected static Dictionary<ActionStatus, Image> StatusImageMap
        {
            get
		{
			Dictionary<ActionStatus, Image> sStatusImageMap = s_statusImageMap;
			if (sStatusImageMap == null)
			{
				lock (s_statusImageMapLock)
				{
					sStatusImageMap = s_statusImageMap;
					if (sStatusImageMap == null)
					{
						InitializeStatusImageMap();
						sStatusImageMap = s_statusImageMap;
					}
				}
			}
			return sStatusImageMap;
		}
        }

        public event JobsUpdatedHandler JobsUpdated;

        public event SelectedJobChangedHandler SelectedJobChanged;

        public event SortOrderChanged SortingChanged;

        static JobListControl()
	{
		s_statusImageMapLock = new object();
	}

        public JobListControl()
	{
		InitializeComponent();
		CommonSerializableList<JobListControl> commonSerializableList = new CommonSerializableList<JobListControl> { this };
		_actionPallet.SourceOverride = commonSerializableList;
		InitializeGridBindings();
		InitializeUpdateClock();
	}

        private void _gridView_DoubleClick(object sender, EventArgs e)
	{
		if (GetMouseInGridRow())
		{
			ViewLogsForJobs viewLogsForJob = new ViewLogsForJobs();
			JobListControl[] jobListControlArray = new JobListControl[1] { this };
			viewLogsForJob.Run(new CommonSerializableList<JobListControl>(jobListControlArray), SelectedObjects);
		}
	}

        private void _gridView_EndSorting(object sender, EventArgs e)
	{
		FireStortingChanged(_gridView.SortedColumns.Count > 0);
	}

        private void _gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_actionPallet.HostingControl = this;
		_actionPallet.BuildActionMenu();
		FireSelectedJobChanged();
	}

        private void _gridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = !_allowNextEditorPast;
		_allowNextEditorPast = false;
	}

        private void _titleColumnEditor_Enter(object sender, EventArgs e)
	{
		BaseEdit white = sender as BaseEdit;
		white.BackColor = Color.White;
		white.BorderStyle = BorderStyles.Simple;
	}

        private void _titleColumnEditor_Leave(object sender, EventArgs e)
	{
		if (sender is BaseEdit baseEdit && _gridView.GetRow(_gridView.FocusedRowHandle) is DataRowView row && _dataRowMap.TryGetValue(row.Row, out var editValue))
		{
			editValue.Job.Title = (string)baseEdit.EditValue;
		}
	}

        private void AddColumnsToDataTable(DataTable table)
	{
		table.Columns.Add("StatusImage", typeof(Image));
		table.Columns.Add("Title", typeof(string));
		table.Columns.Add("Source", typeof(string));
		table.Columns.Add("Target", typeof(string));
		table.Columns.Add("Started", typeof(DateTime));
		table.Columns.Add("Status", typeof(string));
		table.Columns.Add("LogSummary", typeof(string));
		table.Columns.Add("DataMigrated", typeof(string));
		table.Columns.Add("Finished", typeof(DateTime));
		table.Columns.Add("Duration", typeof(string));
		table.Columns.Add("UserName", typeof(string));
		table.Columns.Add("MachineName", typeof(string));
	}

        private void AddJobToGrid(Job job)
	{
		DisplayedJob displayedJob = new DisplayedJob
		{
			Parent = this,
			Job = job
		};
		displayedJob.UpdateRow();
		_jobMap.Add(job.JobID, displayedJob);
		if (_jobsToSelect.Count > 0)
		{
			if (_jobsToSelect.ContainsKey(job.JobID))
			{
				int rowHandle = _gridView.GetRowHandle(_table.Rows.IndexOf(displayedJob.Row));
				_gridView.SelectRow(rowHandle);
			}
		}
		else if (AutoScrollToNewItem)
		{
			int num = _gridView.GetRowHandle(_table.Rows.IndexOf(displayedJob.Row));
			_gridView.ClearSelection();
			_gridView.SelectRow(num);
			_gridView.FocusedRowHandle = num;
			_gridView.TopRowIndex = _gridView.GetVisibleIndex(num);
		}
	}

        private void AddOrUpdateJob(Job job)
	{
		if (_jobMap.TryGetValue(job.JobID, out var displayedJob))
		{
			displayedJob.UpdateRow();
		}
		else
		{
			AddJobToGrid(job);
		}
	}

        public void BeginRenameSelectedJob()
	{
		_gridView.FocusedColumn = _titleColumn;
		_allowNextEditorPast = true;
		_gridView.ShowEditor();
	}

        private void CommitChangesToUI()
	{
		if (base.InvokeRequired)
		{
			try
			{
				Invoke(new VoidDelegate(CommitChangesToUI));
				return;
			}
			catch (InvalidOperationException)
			{
				return;
			}
		}
		bool flag = false;
		_changeTrackerLock.EnterWriteLock();
		List<Job> jobs;
		List<Job> jobs1;
		try
		{
			jobs = new List<Job>(_changedItems.Values);
			_changedItems.Clear();
			jobs1 = new List<Job>(_removedItems.Values);
			_removedItems.Clear();
			flag = _dataSourceReset;
			_dataSourceReset = false;
		}
		finally
		{
			_changeTrackerLock.ExitWriteLock();
		}
		if (flag)
		{
			ReloadUI();
		}
		else
		{
			if (jobs.Count == 0 && jobs1.Count == 0)
			{
				return;
			}
			foreach (Job job in jobs1)
			{
				if (jobs.Contains(job))
				{
					jobs.Remove(job);
				}
			}
			_uiUpdateLock.EnterWriteLock();
			try
			{
				SuspendLayout();
				_grid.BeginUpdate();
				try
				{
					foreach (Job job1 in jobs)
					{
						AddOrUpdateJob(job1);
					}
					foreach (Job job2 in jobs1)
					{
						if (_jobMap.TryGetValue(job2.JobID, out var displayedJob))
						{
							_jobMap.Remove(job2.JobID);
							if (displayedJob.Row != null)
							{
								_dataRowMap.Remove(displayedJob.Row);
							}
							displayedJob.Row.Delete();
						}
					}
					if (_jobsToSelect.Count > 0)
					{
						ScrollAndFocusTopSelectedItem();
						_jobsToSelect.Clear();
					}
				}
				finally
				{
					_grid.EndUpdate();
					ResumeLayout();
				}
			}
			finally
			{
				_uiUpdateLock.ExitWriteLock();
			}
			FireJobsUpdated();
		}
	}

        private void DetachGridSource()
	{
		_grid.DataSource = null;
	}

        protected override void Dispose(bool disposing)
	{
		if (_updateClock != null)
		{
			_updateClock.Stop();
			_updateClock.Dispose();
		}
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void FireJobsUpdated()
	{
		if (this.JobsUpdated != null)
		{
			this.JobsUpdated();
		}
	}

        private void FireSelectedJobChanged()
	{
		if (this.SelectedJobChanged != null)
		{
			this.SelectedJobChanged();
		}
	}

        private void FireStortingChanged(bool sortingApplied)
	{
		if (this.SortingChanged != null)
		{
			this.SortingChanged(sortingApplied);
		}
	}

        private List<Job> GetCurrentlySelectedItems()
	{
		int[] selectedRows = _gridView.GetSelectedRows();
		if (selectedRows == null)
		{
			return new List<Job>();
		}
		List<Job> jobs = new List<Job>(selectedRows.Length);
		int[] numArray = selectedRows;
		foreach (int num in numArray)
		{
			if (_gridView.GetRow(num) is DataRowView row)
			{
				jobs.Add(_dataRowMap[row.Row].Job);
			}
		}
		return jobs;
	}

        private bool GetMouseInGridRow()
	{
		Point client = _gridView.GridControl.PointToClient(Control.MousePosition);
		GridHitInfo gridHitInfo = _gridView.CalcHitInfo(client);
		if (gridHitInfo.InRow)
		{
			return true;
		}
		return gridHitInfo.InRowCell;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		Metalogix.DataStructures.Generic.CommonSerializableList<object> commonSerializableList = new Metalogix.DataStructures.Generic.CommonSerializableList<object>();
		this._grid = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
		this._actionPallet = new Metalogix.UI.WinForms.Actions.ActionPaletteControl(this.components);
		this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this._statusImage = new DevExpress.XtraGrid.Columns.GridColumn();
		this._statusPictureDisplay = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this._titleColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._titleColumnEditor = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this._sourceColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._targetColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._startedColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._statusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._logSummaryColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._dataMigratedColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._finishedColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._durationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._userNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this._machineNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		((System.ComponentModel.ISupportInitialize)this._grid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._statusPictureDisplay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._titleColumnEditor).BeginInit();
		base.SuspendLayout();
		this._grid.ContextMenuStrip = this._actionPallet;
		this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
		this._grid.EmbeddedNavigator.ShowToolTips = false;
		this._grid.Location = new System.Drawing.Point(0, 0);
		this._grid.MainView = this._gridView;
		this._grid.Name = "_grid";
		DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._grid.RepositoryItems;
		DevExpress.XtraEditors.Repository.RepositoryItem[] repositoryItemArray = new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this._statusPictureDisplay, this._titleColumnEditor };
		repositoryItems.AddRange(repositoryItemArray);
		this._grid.Size = new System.Drawing.Size(613, 374);
		this._grid.TabIndex = 0;
		this._grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridView });
		this._actionPallet.HostingControl = null;
		this._actionPallet.Name = "_actionPallet";
		this._actionPallet.Size = new System.Drawing.Size(61, 4);
		this._actionPallet.SourceOverride = commonSerializableList;
		this._actionPallet.UseSourceOverride = true;
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this._gridView.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray = new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this._statusImage, this._titleColumn, this._sourceColumn, this._targetColumn, this._startedColumn, this._statusColumn, this._logSummaryColumn, this._dataMigratedColumn, this._finishedColumn, this._durationColumn,
			this._userNameColumn, this._machineNameColumn
		};
		columns.AddRange(gridColumnArray);
		this._gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this._gridView.GridControl = this._grid;
		this._gridView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
		this._gridView.Name = "_gridView";
		this._gridView.OptionsBehavior.AutoUpdateTotalSummary = false;
		this._gridView.OptionsCustomization.AllowColumnMoving = false;
		this._gridView.OptionsCustomization.AllowFilter = false;
		this._gridView.OptionsCustomization.AllowGroup = false;
		this._gridView.OptionsDetail.AllowZoomDetail = false;
		this._gridView.OptionsDetail.EnableMasterViewMode = false;
		this._gridView.OptionsDetail.ShowDetailTabs = false;
		this._gridView.OptionsDetail.SmartDetailExpand = false;
		this._gridView.OptionsFilter.AllowColumnMRUFilterList = false;
		this._gridView.OptionsFilter.AllowMRUFilterList = false;
		this._gridView.OptionsHint.ShowCellHints = false;
		this._gridView.OptionsHint.ShowColumnHeaderHints = false;
		this._gridView.OptionsHint.ShowFooterHints = false;
		this._gridView.OptionsMenu.EnableColumnMenu = false;
		this._gridView.OptionsMenu.EnableFooterMenu = false;
		this._gridView.OptionsMenu.EnableGroupPanelMenu = false;
		this._gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this._gridView.OptionsSelection.MultiSelect = true;
		this._gridView.OptionsSelection.UseIndicatorForSelection = false;
		this._gridView.OptionsView.ColumnAutoWidth = false;
		this._gridView.OptionsView.ShowDetailButtons = false;
		this._gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this._gridView.OptionsView.ShowGroupPanel = false;
		this._gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.OptionsView.ShowIndicator = false;
		this._gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.SynchronizeClones = false;
		this._gridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(_gridView_SelectionChanged);
		this._gridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(_gridView_ShowingEditor);
		this._gridView.EndSorting += new System.EventHandler(_gridView_EndSorting);
		this._gridView.DoubleClick += new System.EventHandler(_gridView_DoubleClick);
		this._statusImage.Caption = "Status Image";
		this._statusImage.ColumnEdit = this._statusPictureDisplay;
		this._statusImage.FieldName = "StatusImage";
		this._statusImage.Name = "_statusImage";
		this._statusImage.OptionsColumn.AllowEdit = false;
		this._statusImage.OptionsColumn.AllowFocus = false;
		this._statusImage.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
		this._statusImage.OptionsColumn.AllowIncrementalSearch = false;
		this._statusImage.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
		this._statusImage.OptionsColumn.AllowMove = false;
		this._statusImage.OptionsColumn.AllowShowHide = false;
		this._statusImage.OptionsColumn.AllowSize = false;
		this._statusImage.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this._statusImage.OptionsColumn.FixedWidth = true;
		this._statusImage.OptionsColumn.ShowCaption = false;
		this._statusImage.OptionsColumn.ShowInCustomizationForm = false;
		this._statusImage.OptionsColumn.ShowInExpressionEditor = false;
		this._statusImage.OptionsColumn.TabStop = false;
		this._statusImage.Visible = true;
		this._statusImage.VisibleIndex = 0;
		this._statusImage.Width = 20;
		this._statusPictureDisplay.Name = "_statusPictureDisplay";
		this._titleColumn.Caption = "Job Name";
		this._titleColumn.ColumnEdit = this._titleColumnEditor;
		this._titleColumn.FieldName = "Title";
		this._titleColumn.Name = "_titleColumn";
		this._titleColumn.OptionsColumn.AllowMove = false;
		this._titleColumn.OptionsColumn.AllowShowHide = false;
		this._titleColumn.Visible = true;
		this._titleColumn.VisibleIndex = 1;
		this._titleColumn.Width = 160;
		this._titleColumnEditor.AutoHeight = false;
		this._titleColumnEditor.Name = "_titleColumnEditor";
		this._titleColumnEditor.Enter += new System.EventHandler(_titleColumnEditor_Enter);
		this._titleColumnEditor.Leave += new System.EventHandler(_titleColumnEditor_Leave);
		this._sourceColumn.Caption = "Source";
		this._sourceColumn.FieldName = "Source";
		this._sourceColumn.Name = "_sourceColumn";
		this._sourceColumn.OptionsColumn.AllowEdit = false;
		this._sourceColumn.OptionsColumn.AllowMove = false;
		this._sourceColumn.OptionsColumn.AllowShowHide = false;
		this._sourceColumn.Visible = true;
		this._sourceColumn.VisibleIndex = 2;
		this._sourceColumn.Width = 100;
		this._targetColumn.Caption = "Target Container";
		this._targetColumn.FieldName = "Target";
		this._targetColumn.Name = "_targetColumn";
		this._targetColumn.OptionsColumn.AllowEdit = false;
		this._targetColumn.OptionsColumn.AllowMove = false;
		this._targetColumn.OptionsColumn.AllowShowHide = false;
		this._targetColumn.Visible = true;
		this._targetColumn.VisibleIndex = 3;
		this._targetColumn.Width = 120;
		this._startedColumn.Caption = "Started";
		this._startedColumn.DisplayFormat.FormatString = "G";
		this._startedColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this._startedColumn.FieldName = "Started";
		this._startedColumn.Name = "_startedColumn";
		this._startedColumn.OptionsColumn.AllowEdit = false;
		this._startedColumn.OptionsColumn.AllowMove = false;
		this._startedColumn.OptionsColumn.AllowShowHide = false;
		this._startedColumn.Visible = true;
		this._startedColumn.VisibleIndex = 4;
		this._startedColumn.Width = 121;
		this._statusColumn.Caption = "Status";
		this._statusColumn.FieldName = "Status";
		this._statusColumn.Name = "_statusColumn";
		this._statusColumn.OptionsColumn.AllowEdit = false;
		this._statusColumn.OptionsColumn.AllowMove = false;
		this._statusColumn.OptionsColumn.AllowShowHide = false;
		this._statusColumn.Visible = true;
		this._statusColumn.VisibleIndex = 5;
		this._statusColumn.Width = 123;
		this._logSummaryColumn.Caption = "Log Summary";
		this._logSummaryColumn.FieldName = "LogSummary";
		this._logSummaryColumn.Name = "_logSummaryColumn";
		this._logSummaryColumn.OptionsColumn.AllowEdit = false;
		this._logSummaryColumn.OptionsColumn.AllowMove = false;
		this._logSummaryColumn.OptionsColumn.AllowShowHide = false;
		this._logSummaryColumn.Visible = true;
		this._logSummaryColumn.VisibleIndex = 6;
		this._logSummaryColumn.Width = 160;
		this._dataMigratedColumn.Caption = "Data Migrated";
		this._dataMigratedColumn.FieldName = "DataMigrated";
		this._dataMigratedColumn.Name = "_dataMigratedColumn";
		this._dataMigratedColumn.OptionsColumn.AllowEdit = false;
		this._dataMigratedColumn.OptionsColumn.AllowMove = false;
		this._dataMigratedColumn.OptionsColumn.AllowShowHide = false;
		this._dataMigratedColumn.Visible = true;
		this._dataMigratedColumn.VisibleIndex = 7;
		this._dataMigratedColumn.Width = 130;
		this._finishedColumn.Caption = "Finished";
		this._finishedColumn.DisplayFormat.FormatString = "G";
		this._finishedColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this._finishedColumn.FieldName = "Finished";
		this._finishedColumn.Name = "_finishedColumn";
		this._finishedColumn.OptionsColumn.AllowEdit = false;
		this._finishedColumn.OptionsColumn.AllowMove = false;
		this._finishedColumn.OptionsColumn.AllowShowHide = false;
		this._finishedColumn.Visible = true;
		this._finishedColumn.VisibleIndex = 8;
		this._finishedColumn.Width = 130;
		this._durationColumn.Caption = "Duration";
		this._durationColumn.FieldName = "Duration";
		this._durationColumn.Name = "_durationColumn";
		this._durationColumn.OptionsColumn.AllowEdit = false;
		this._durationColumn.OptionsColumn.AllowMove = false;
		this._durationColumn.OptionsColumn.AllowShowHide = false;
		this._durationColumn.Visible = true;
		this._durationColumn.VisibleIndex = 9;
		this._durationColumn.Width = 80;
		this._userNameColumn.Caption = "User Name";
		this._userNameColumn.FieldName = "UserName";
		this._userNameColumn.Name = "_userNameColumn";
		this._userNameColumn.OptionsColumn.AllowEdit = false;
		this._userNameColumn.OptionsColumn.AllowMove = false;
		this._userNameColumn.OptionsColumn.AllowShowHide = true;
		this._userNameColumn.Visible = true;
		this._userNameColumn.VisibleIndex = 10;
		this._userNameColumn.Width = 100;
		this._machineNameColumn.Caption = "Machine Name";
		this._machineNameColumn.FieldName = "MachineName";
		this._machineNameColumn.Name = "_machineNameColumn";
		this._machineNameColumn.OptionsColumn.AllowEdit = false;
		this._machineNameColumn.OptionsColumn.AllowMove = false;
		this._machineNameColumn.OptionsColumn.AllowShowHide = true;
		this._machineNameColumn.Visible = true;
		this._machineNameColumn.VisibleIndex = 11;
		this._machineNameColumn.Width = 100;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this._grid);
		base.Name = "JobListControl";
		base.Size = new System.Drawing.Size(613, 374);
		((System.ComponentModel.ISupportInitialize)this._grid).EndInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this._statusPictureDisplay).EndInit();
		((System.ComponentModel.ISupportInitialize)this._titleColumnEditor).EndInit();
		base.ResumeLayout(false);
	}

        private void InitializeGridBindings()
	{
		_jobMap = new Dictionary<string, DisplayedJob>();
		_dataRowMap = new Dictionary<DataRow, DisplayedJob>();
		_jobsToSelect = new Dictionary<string, Job>();
		_table = new DataTable();
		AddColumnsToDataTable(_table);
		ReattachGridSource();
	}

        private static void InitializeStatusImageMap()
	{
		s_statusImageMap = new Dictionary<ActionStatus, Image>
		{
			{
				ActionStatus.Aborted,
				Resources.Item_Status_Aborted
			},
			{
				ActionStatus.Aborting,
				Resources.Item_Status_Aborted
			},
			{
				ActionStatus.Done,
				Resources.Item_Status_Completed
			},
			{
				ActionStatus.Failed,
				Resources.Item_Status_Failed
			},
			{
				ActionStatus.NotRunning,
				Resources.ItemStatus_Blank
			},
			{
				ActionStatus.Paused,
				Resources.Item_Status_Paused
			},
			{
				ActionStatus.Running,
				Resources.Item_Status_Running
			},
			{
				ActionStatus.Warning,
				Resources.Item_Status_Warning
			}
		};
	}

        private void InitializeUpdateClock()
	{
		_updateClock = new System.Timers.Timer(500.0);
		_updateClock.Elapsed += UpdateClockTick;
		_updateClock.AutoReset = false;
		_updateClock.Enabled = false;
		_updateClockTicking = false;
		_updateClockPaused = false;
	}

        private void On_dataSource_ListChanged(ChangeType changeType, Job[] jobs)
	{
		if (changeType != ChangeType.Reset && jobs.Length == 0)
		{
			return;
		}
		_changeTrackerLock.EnterReadLock();
		try
		{
			if (changeType != ChangeType.Reset)
			{
				foreach (Job job in jobs)
				{
					if (changeType != ChangeType.ItemDeleted)
					{
						_changedItems[job.JobID] = job;
					}
					else
					{
						_removedItems[job.JobID] = job;
					}
				}
			}
			else
			{
				_dataSourceReset = true;
			}
		}
		finally
		{
			_changeTrackerLock.ExitReadLock();
		}
	}

        public void PauseUpdating()
	{
		lock (_updateClock)
		{
			_updateClockPaused = true;
		}
	}

        private void ReattachGridSource()
	{
		if (_grid.DataSource == null)
		{
			_grid.DataSource = _table;
		}
	}

        private void ReloadUI()
	{
		if (base.InvokeRequired)
		{
			Invoke(new VoidDelegate(ReloadUI));
			return;
		}
		_uiUpdateLock.EnterWriteLock();
		try
		{
			SuspendLayout();
			_grid.BeginUpdate();
			try
			{
				DetachGridSource();
				_jobMap.Clear();
				_table.Rows.Clear();
				_jobsToSelect.Clear();
				if (DataSource != null)
				{
					foreach (Job dataSource in DataSource)
					{
						AddOrUpdateJob(dataSource);
					}
				}
				ReattachGridSource();
				_gridView.ClearSelection();
				ScrollToBottom();
			}
			finally
			{
				ReattachGridSource();
				_grid.EndUpdate();
				ResumeLayout();
			}
		}
		finally
		{
			_uiUpdateLock.ExitWriteLock();
		}
		FireJobsUpdated();
	}

        public void ResetSort()
	{
		if (base.InvokeRequired)
		{
			Invoke(new VoidDelegate(ResetSort));
			return;
		}
		_gridView.SortInfo.ClearSorting();
		FireStortingChanged(_gridView.SortedColumns.Count > 0);
	}

        public void ResumeUpdating()
	{
		lock (_updateClock)
		{
			_updateClockPaused = false;
			if (_updateClockTicking)
			{
				_updateClock.Start();
			}
		}
	}

        public void RunErrorReportingTool(string exeLocation)
	{
		FileInfo fileInfo = new FileInfo(JobsSettings.JobHistorySettings.FileName);
		DirectoryInfo directory = new FileInfo(ApplicationData.MainAssembly.FullName).Directory;
		if (directory == null)
		{
			Trace.WriteLine("Unable to find run location");
			return;
		}
		List<Job> currentlySelectedItems = GetCurrentlySelectedItems();
		if (currentlySelectedItems.Count <= 0)
		{
			Trace.WriteLine("No items selected in RunErrorReportingTool");
			return;
		}
		Job item = currentlySelectedItems[0];
		ProcessStartInfo processStartInfo = new ProcessStartInfo($"\"{exeLocation}\"");
		string[] strArrays = new string[6]
		{
			"-x ",
			$"\"{fileInfo.FullName}\"",
			" -j ",
			$"\"{item.JobID}\"",
			" -l ",
			$"\"{directory.FullName}\""
		};
		processStartInfo.Arguments = string.Concat(strArrays);
		Process.Start(processStartInfo);
	}

        private void RunInReadLock(ReaderWriterLockSlim readWriteLock, VoidDelegate method)
	{
		if (!readWriteLock.IsWriteLockHeld)
		{
			readWriteLock.EnterReadLock();
		}
		try
		{
			method();
		}
		finally
		{
			if (!readWriteLock.IsWriteLockHeld)
			{
				readWriteLock.ExitReadLock();
			}
		}
	}

        private void ScrollAndFocusTopSelectedItem()
	{
		int? nullable = null;
		int visibleIndex = -1;
		int[] selectedRows = _gridView.GetSelectedRows();
		foreach (int num in selectedRows)
		{
			if (!nullable.HasValue)
			{
				nullable = num;
				visibleIndex = _gridView.GetVisibleIndex(num);
			}
			int visibleIndex1 = _gridView.GetVisibleIndex(num);
			if (visibleIndex1 < visibleIndex)
			{
				nullable = num;
				visibleIndex = visibleIndex1;
			}
		}
		if (nullable.HasValue)
		{
			_gridView.FocusedRowHandle = nullable.Value;
			_gridView.TopRowIndex = visibleIndex;
		}
	}

        private void ScrollToBottom()
	{
		if (_gridView.DataRowCount > 0)
		{
			_gridView.TopRowIndex = _gridView.DataRowCount - 1;
		}
	}

        private void SelectJobs(IEnumerable<Job> jobs)
	{
		if (base.InvokeRequired)
		{
			SelectJobsDelegate selectJobsDelegate = SelectJobs;
			object[] objArray = new object[1] { jobs };
			Invoke(selectJobsDelegate, objArray);
		}
		_uiUpdateLock.EnterWriteLock();
		_gridView.BeginUpdate();
		try
		{
			_gridView.ClearSelection();
			if (jobs == null)
			{
				return;
			}
			int? nullable = null;
			foreach (Job job in jobs)
			{
				if (_jobMap.TryGetValue(job.JobID, out var displayedJob))
				{
					int rowHandle = _gridView.GetRowHandle(_table.Rows.IndexOf(displayedJob.Row));
					_gridView.SelectRow(rowHandle);
					if (!nullable.HasValue)
					{
						nullable = rowHandle;
					}
				}
				else
				{
					_jobsToSelect[job.JobID] = job;
				}
			}
			if (nullable.HasValue)
			{
				_gridView.FocusedRowHandle = nullable.Value;
				_gridView.TopRowIndex = _gridView.GetVisibleIndex(nullable.Value);
			}
		}
		finally
		{
			_gridView.EndUpdate();
			_uiUpdateLock.ExitWriteLock();
		}
	}

        private void SetDataSource(JobCollection value)
	{
		if (base.InvokeRequired)
		{
			SetDataSourceDelegate setDataSourceDelegate = SetDataSource;
			object[] objArray = new object[1] { value };
			Invoke(setDataSourceDelegate, objArray);
			return;
		}
		_dataSourceLock.EnterWriteLock();
		try
		{
			if (_dataSource != null)
			{
				_dataSource.ListChanged -= _jobChangedHandler;
				_jobChangedHandler = null;
				StopUpdateClock();
				lock (_logItemViewerLock)
				{
					if (_logItemViewer != null)
					{
						if (_logItemViewer.Visible)
						{
							_logItemViewer.Close();
						}
						_logItemViewer.DataSource = null;
					}
				}
			}
			_dataSource = value;
			if (_dataSource != null)
			{
				_jobChangedHandler = On_dataSource_ListChanged;
				_dataSource.ListChanged += _jobChangedHandler;
			}
			ReloadUI();
			if (_dataSource != null)
			{
				StartUpdateClock();
			}
		}
		finally
		{
			_dataSourceLock.ExitWriteLock();
		}
	}

        private void StartUpdateClock()
	{
		lock (_updateClock)
		{
			if (!_updateClockPaused)
			{
				_updateClockTicking = true;
				_updateClock.Start();
			}
		}
	}

        private void StopUpdateClock()
	{
		lock (_updateClock)
		{
			_updateClock.Stop();
			_updateClockTicking = false;
		}
	}

        private void UpdateClockTick(object sender, ElapsedEventArgs e)
	{
		try
		{
			if (!base.IsDisposed)
			{
				CommitChangesToUI();
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			lock (_updateClock)
			{
				if (_updateClockTicking && !_updateClockPaused)
				{
					try
					{
						_updateClock.Start();
					}
					catch (ObjectDisposedException)
					{
					}
				}
			}
		}
	}
    }
}
