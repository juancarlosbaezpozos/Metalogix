using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class LogItemListControl : XtraUserControl, IHasSelectableObjects
    {
        private class DisplayedLogItem
        {
            private bool InRunningTable { get; set; }

            public LogItem Item { get; set; }

            public LogItemListControl Parent { get; set; }

            private DataRow Row { get; set; }

            private void EnsureCorrectRowExists()
            {
                ActionOperationStatus status = Item.Status;
                if (Row != null && ((InRunningTable && (status != ActionOperationStatus.Running || Parent.LogItemsMerged)) || (!InRunningTable && !Parent.LogItemsMerged && status == ActionOperationStatus.Running)))
                {
                    Parent._dataRowMap.Remove(Row);
                    Row.Delete();
                    Row = null;
                }
                if (Row == null)
                {
                    if (status != ActionOperationStatus.Running || Parent.LogItemsMerged)
                    {
                        Row = Parent._mainTable.NewRow();
                        Parent._mainTable.Rows.Add(Row);
                        InRunningTable = false;
                    }
                    else
                    {
                        Row = Parent._runningTable.NewRow();
                        Parent._runningTable.Rows.Add(Row);
                        InRunningTable = true;
                    }
                    Parent._dataRowMap.Add(Row, this);
                }
            }

            public void UpdateRow()
            {
                EnsureCorrectRowExists();
                Row["Time"] = Item.TimeStamp;
                Row["Item"] = Item.ItemName;
                Row["Operation"] = Item.Operation;
                Row["Source"] = Item.Source;
                Row["Target"] = Item.Target;
                Row["Information"] = Item.Information;
                Row["TextStatus"] = Enum.GetName(typeof(ActionOperationStatus), Item.Status);
                Row["StatusEnum"] = Item.Status;
                UpdateStatusImage();
            }

            private void UpdateStatusImage()
            {
                StatusImageMap.TryGetValue(Item.Status, out var image);
                Row["Status"] = image;
            }
        }

        public delegate void UIUpdatedHandler();

        private delegate void VoidDelegate();

        private const string COLUMN_NAME_STATUS = "Status";

        private const string COLUMN_NAME_TIME = "Time";

        private const string COLUMN_NAME_OPERATION = "Operation";

        private const string COLUMN_NAME_ITEM = "Item";

        private const string COLUMN_NAME_SOURCE = "Source";

        private const string COLUMN_NAME_TARGET = "Target";

        private const string COLUMN_NAME_INFORMATION = "Information";

        private const string COLUMN_NAME_TEXTSTATUS = "TextStatus";

        private const string COLUMN_NAME_STATUSENUM = "StatusEnum";

        private static Dictionary<ActionOperationStatus, Image> s_statusImageMap;

        private static readonly object s_statusImageMapLock;

        private readonly ReaderWriterLockSlim _dataSourceLock = new ReaderWriterLockSlim();

        private readonly ReaderWriterLockSlim _uiUpdateLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly ReaderWriterLockSlim _changeTrackerLock = new ReaderWriterLockSlim();

        private LogItemCollection.ListChangedHandler _logItemChangedHandler;

        private LogItemCollection _dataSource;

        private bool _autoScrollToBottom = true;

        private System.Timers.Timer _updateClock;

        private bool _updateClockTicking;

        private bool _updateClockPaused;

        private readonly ThreadSafeDictionary<string, LogItem> _changedItems = new ThreadSafeDictionary<string, LogItem>();

        private List<ActionOperationStatus> _allowedStatuses;

        private bool _autoScrollActive = true;

        private Dictionary<string, DisplayedLogItem> _logItemMap;

        private Dictionary<DataRow, DisplayedLogItem> _dataRowMap;

        private DataTable _mainTable;

        private DataTable _runningTable;

        private IContainer components;

        private DevExpress.XtraTab.XtraTabControl _tabControl;

        private XtraTabPage _eventLogTabPage;

        private XtraTabPage _currentlyRunningTabPage;

        private GridView _jobEventView;

        private GridColumn _statusColumn;

        private GridColumn _timeColumn;

        private GridColumn _operationColumn;

        private GridColumn _itemColumn;

        private GridColumn _sourceColumn;

        private GridColumn _targetColumn;

        private GridColumn _informationColumn;

        private GridView _runningView;

        private GridColumn _runningStatusColumn;

        private GridColumn _runningStartTimeColumn;

        private GridColumn _runningOperationColumn;

        private GridColumn _runningItemColumn;

        private GridColumn _runningSourceColumn;

        private GridColumn _runningTargetColumn;

        private GridColumn _runningInformationColumn;

        private ActionPaletteControl _actionPalletControl;

        private XtraParentSelectableGrid _mainGrid;

        private XtraParentSelectableGrid _runningGrid;

        private GridColumn _statusTextColumn;

        private GridColumn _runningHiddenStatusColumn;

        private GridColumn _runningTextStatusColumn;

        private RepositoryItemPictureEdit repositoryItemPictureEdit1;

        private RepositoryItemPictureEdit repositoryItemPictureEdit2;

        private RepositoryItemMemoEdit repositoryItemMemoEdit1;

        private RepositoryItemMemoEdit repositoryItemMemoEdit2;

        private RepositoryItemMemoEdit repositoryItemMemoEdit3;

        public bool AutoScrollToBottom
        {
            get
            {
                return _autoScrollToBottom;
            }
            set
            {
                _autoScrollToBottom = value;
            }
        }

        public bool CanSelectNextIndex
        {
            get
            {
                bool nextVisibleRow = false;
                RunInReadLock(_uiUpdateLock, delegate
                {
                    GridView gridView = (GridView)_mainGrid.MainView;
                    if (gridView != null && gridView.FocusedRowHandle != int.MinValue)
                    {
                        nextVisibleRow = gridView.GetNextVisibleRow(gridView.GetVisibleIndex(gridView.FocusedRowHandle)) != int.MinValue;
                    }
                });
                return nextVisibleRow;
            }
        }

        public bool CanSelectPreviousIndex
        {
            get
            {
                bool prevVisibleRow = false;
                RunInReadLock(_uiUpdateLock, delegate
                {
                    GridView gridView = (GridView)_mainGrid.MainView;
                    if (gridView != null && gridView.FocusedRowHandle != int.MinValue)
                    {
                        prevVisibleRow = gridView.GetPrevVisibleRow(gridView.GetVisibleIndex(gridView.FocusedRowHandle)) != int.MinValue;
                    }
                });
                return prevVisibleRow;
            }
        }

        public LogItemCollection DataSource
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
                _dataSourceLock.EnterWriteLock();
                try
                {
                    if (_dataSource != null)
                    {
                        _dataSource.ListChanged -= _logItemChangedHandler;
                        _logItemChangedHandler = null;
                        StopUpdateClock();
                    }
                    _dataSource = value;
                    if (_dataSource != null)
                    {
                        _logItemChangedHandler = On_dataSource_ListChanged;
                        _dataSource.ListChanged += _logItemChangedHandler;
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
        }

        public bool LogItemsMerged { get; private set; }

        public LogItem SelectedItem
        {
            get
            {
                List<LogItem> logItems = new List<LogItem>();
                RunInReadLock(_uiUpdateLock, delegate
                {
                    logItems = GetCurrentlySelectedItems();
                });
                if (logItems.Count == 0)
                {
                    return null;
                }
                return logItems[0];
            }
        }

        public IXMLAbleList SelectedObjects
        {
            get
            {
                List<LogItem> logItems = new List<LogItem>();
                RunInReadLock(_uiUpdateLock, delegate
                {
                    logItems = GetCurrentlySelectedItems();
                });
                return new CommonSerializableList<LogItem>(logItems.ToArray());
            }
        }

        protected static Dictionary<ActionOperationStatus, Image> StatusImageMap
        {
            get
            {
                Dictionary<ActionOperationStatus, Image> sStatusImageMap = s_statusImageMap;
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

        public event UIUpdatedHandler UIUpdated;

        static LogItemListControl()
        {
            s_statusImageMapLock = new object();
        }

        public LogItemListControl()
        {
            InitializeComponent();
            LogItemsMerged = false;
            _actionPalletControl.LegalType = typeof(ILogAction);
            InitializeUpdateClock();
            InitializeGridBindings();
            InitializeStatusFilter();
        }

        private void _jobEventView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
        }

        private void _jobEventView_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView mainView = (GridView)_mainGrid.MainView;
            if (mainView != null && mainView.GetRow(e.RowHandle) is DataRowView row)
            {
                switch ((ActionOperationStatus)row.Row["StatusEnum"])
                {
                    case ActionOperationStatus.Failed:
                        e.Appearance.ForeColor = Color.Red;
                        break;
                    case ActionOperationStatus.Same:
                        break;
                    case ActionOperationStatus.Different:
                        e.Appearance.ForeColor = Color.Blue;
                        break;
                    case ActionOperationStatus.MissingOnSource:
                        e.Appearance.ForeColor = Color.Red;
                        break;
                    case ActionOperationStatus.MissingOnTarget:
                        e.Appearance.ForeColor = Color.Red;
                        break;
                }
            }
        }

        private void _jobEventView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LogItemsMerged || _tabControl.SelectedTabPage == _eventLogTabPage)
            {
                FireUIUpdated();
            }
        }

        private void _jobEventView_TopRowChanged(object sender, EventArgs e)
        {
            GridView mainView = (GridView)_mainGrid.MainView;
            if (mainView != null && mainView.IsRowVisible(GetLastRowHandle(mainView)) == RowVisibleState.Visible)
            {
                UpdateAutoScrollingOptions(activate: true);
            }
            else
            {
                UpdateAutoScrollingOptions(activate: false);
            }
        }

        private void _runningView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (!LogItemsMerged && _tabControl.SelectedTabPage != _eventLogTabPage)
            {
                FireUIUpdated();
            }
        }

        private void _tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            FireUIUpdated();
        }

        private void AddColumnsToDataTable(DataTable table)
        {
            table.Columns.Add("Status", typeof(Image));
            table.Columns.Add("Time", typeof(DateTime));
            table.Columns.Add("Operation", typeof(string));
            table.Columns.Add("Item", typeof(string));
            table.Columns.Add("Source", typeof(string));
            table.Columns.Add("Target", typeof(string));
            table.Columns.Add("Information", typeof(string));
            table.Columns.Add("TextStatus", typeof(string));
            table.Columns.Add("StatusEnum", typeof(ActionOperationStatus));
        }

        private void AddItemToGrids(LogItem item)
        {
            DisplayedLogItem displayedLogItem = new DisplayedLogItem
            {
                Parent = this,
                Item = item
            };
            displayedLogItem.UpdateRow();
            _logItemMap.Add(item.ID, displayedLogItem);
        }

        private void AddOrUpdateLogItem(LogItem item)
        {
            if (_logItemMap.TryGetValue(item.ID, out var displayedLogItem))
            {
                displayedLogItem.UpdateRow();
            }
            else
            {
                AddItemToGrids(item);
            }
        }

        private void AutoSizeInformationColumns()
        {
            _informationColumn.BestFit();
            _runningInformationColumn.BestFit();
        }

        public void ChangeLogItemStatusFilter(ActionOperationStatus actionOperationStatus, bool bAddToFilter, bool bSynchronousChange)
        {
            if (bAddToFilter && !_allowedStatuses.Contains(actionOperationStatus))
            {
                _allowedStatuses.Add(actionOperationStatus);
            }
            else if (!bAddToFilter)
            {
                _allowedStatuses.Remove(actionOperationStatus);
            }
            if (bSynchronousChange)
            {
                CommitLogItemStatusFilterUpdates();
            }
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
            _uiUpdateLock.EnterWriteLock();
            try
            {
                _changeTrackerLock.EnterWriteLock();
                LogItem[] logItemArray;
                try
                {
                    logItemArray = new LogItem[_changedItems.Count];
                    _changedItems.Values.CopyTo(logItemArray, 0);
                    _changedItems.Clear();
                }
                finally
                {
                    _changeTrackerLock.ExitWriteLock();
                }
                if (logItemArray.Length == 0)
                {
                    return;
                }
                SuspendLayout();
                _mainGrid.BeginUpdate();
                _runningGrid.BeginUpdate();
                try
                {
                    LogItem[] logItemArray1 = logItemArray;
                    for (int i = 0; i < logItemArray1.Length; i++)
                    {
                        AddOrUpdateLogItem(logItemArray1[i]);
                    }
                    ScrollToBottom();
                    if (logItemArray.Length != 0)
                    {
                        AutoSizeInformationColumns();
                    }
                }
                finally
                {
                    _runningGrid.EndUpdate();
                    _mainGrid.EndUpdate();
                    ResumeLayout();
                }
                FireUIUpdated();
                UpdateRunningColumnName();
            }
            finally
            {
                _uiUpdateLock.ExitWriteLock();
            }
        }

        public void CommitLogItemStatusFilterUpdates()
        {
            _uiUpdateLock.EnterWriteLock();
            try
            {
                _mainGrid.BeginUpdate();
                _runningGrid.BeginUpdate();
                try
                {
                    GridView mainView = (GridView)_mainGrid.MainView;
                    GridView gridView = (GridView)_runningGrid.MainView;
                    if (mainView == null || gridView == null)
                    {
                        return;
                    }
                    mainView.ActiveFilter.Clear();
                    gridView.ActiveFilter.Clear();
                    Array values = Enum.GetValues(typeof(ActionOperationStatus));
                    if (_allowedStatuses.Count != values.Length)
                    {
                        StringBuilder stringBuilder = new StringBuilder(1000);
                        bool flag = true;
                        foreach (ActionOperationStatus value in values)
                        {
                            if (!_allowedStatuses.Contains(value))
                            {
                                if (!flag)
                                {
                                    stringBuilder.Append(" AND ");
                                }
                                stringBuilder.Append("[TextStatus] <> '");
                                stringBuilder.Append(Enum.GetName(typeof(ActionOperationStatus), value));
                                stringBuilder.Append("'");
                                flag = false;
                            }
                        }
                        mainView.ActiveFilter.Add(_statusTextColumn, new ColumnFilterInfo(stringBuilder.ToString(), ""));
                        gridView.ActiveFilter.Add(_runningHiddenStatusColumn, new ColumnFilterInfo(stringBuilder.ToString(), ""));
                    }
                }
                finally
                {
                    _runningGrid.EndUpdate();
                    _mainGrid.EndUpdate();
                }
            }
            finally
            {
                _uiUpdateLock.ExitWriteLock();
            }
            FireUIUpdated();
        }

        private void DetachGridSources()
        {
            _mainGrid.DataSource = null;
            _runningGrid.DataSource = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            if (_dataSource != null)
            {
                _dataSource.ListChanged -= _logItemChangedHandler;
            }
            base.Dispose(disposing);
        }

        private void FireUIUpdated()
        {
            if (this.UIUpdated != null)
            {
                this.UIUpdated();
            }
        }

        private GridControl GetCurrentGridControl()
        {
            if (!LogItemsMerged && _tabControl.SelectedTabPage != _eventLogTabPage)
            {
                return _runningGrid;
            }
            return _mainGrid;
        }

        private List<LogItem> GetCurrentlySelectedItems()
        {
            GridView mainView = (GridView)_mainGrid.MainView;
            if (mainView == null)
            {
                return new List<LogItem>();
            }
            int[] selectedRows = mainView.GetSelectedRows();
            if (selectedRows == null)
            {
                return new List<LogItem>();
            }
            List<LogItem> logItems = new List<LogItem>(selectedRows.Length);
            int[] numArray = selectedRows;
            for (int i = 0; i < numArray.Length; i++)
            {
                if (mainView.GetRow(numArray[i]) is DataRowView row)
                {
                    logItems.Add(_dataRowMap[row.Row].Item);
                }
            }
            return logItems;
        }

        private int GetLastRowHandle(GridView view)
        {
            return view.GetRowHandle(view.DataRowCount - 1);
        }

        private bool GetMouseInGridRow(GridView clickedView)
        {
            if (clickedView == null)
            {
                return false;
            }
            Point client = clickedView.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo gridHitInfo = clickedView.CalcHitInfo(client);
            if (gridHitInfo.InRow)
            {
                return true;
            }
            return gridHitInfo.InRowCell;
        }

        private void InitializeComponent()
        {
            Metalogix.DataStructures.Generic.CommonSerializableList<object> commonSerializableList = new Metalogix.DataStructures.Generic.CommonSerializableList<object>();
            this._tabControl = new DevExpress.XtraTab.XtraTabControl();
            this._eventLogTabPage = new DevExpress.XtraTab.XtraTabPage();
            this._mainGrid = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
            this._actionPalletControl = new Metalogix.UI.WinForms.Actions.ActionPaletteControl();
            this._jobEventView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._statusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this._timeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._operationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._itemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._sourceColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._targetColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._statusTextColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._informationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this._currentlyRunningTabPage = new DevExpress.XtraTab.XtraTabPage();
            this._runningGrid = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
            this._runningView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._runningStatusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this._runningStartTimeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningOperationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningItemColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningSourceColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningTargetColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningInformationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._runningTextStatusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)this._tabControl).BeginInit();
            this._tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._mainGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._jobEventView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._runningGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._runningView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit2).BeginInit();
            base.SuspendLayout();
            this._tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._tabControl.Location = new System.Drawing.Point(0, 0);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedTabPage = this._eventLogTabPage;
            this._tabControl.Size = new System.Drawing.Size(613, 374);
            this._tabControl.TabIndex = 0;
            DevExpress.XtraTab.XtraTabPageCollection tabPages = this._tabControl.TabPages;
            DevExpress.XtraTab.XtraTabPage[] xtraTabPageArray = new DevExpress.XtraTab.XtraTabPage[2] { this._eventLogTabPage, this._currentlyRunningTabPage };
            tabPages.AddRange(xtraTabPageArray);
            this._tabControl.TabIndexChanged += new System.EventHandler(_tabControl_TabIndexChanged);
            this._eventLogTabPage.Controls.Add(this._mainGrid);
            this._eventLogTabPage.Enabled = true;
            this._eventLogTabPage.Name = "_eventLogTabPage";
            this._eventLogTabPage.Size = new System.Drawing.Size(607, 346);
            this._eventLogTabPage.Text = "JOB EVENT LOG";
            this._mainGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._mainGrid.ContextMenuStrip = this._actionPalletControl;
            this._mainGrid.Location = new System.Drawing.Point(3, 3);
            this._mainGrid.MainView = this._jobEventView;
            this._mainGrid.Name = "_mainGrid";
            DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._mainGrid.RepositoryItems;
            DevExpress.XtraEditors.Repository.RepositoryItem[] repositoryItemArray = new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemPictureEdit1, this.repositoryItemMemoEdit1, this.repositoryItemMemoEdit2, this.repositoryItemMemoEdit3 };
            repositoryItems.AddRange(repositoryItemArray);
            this._mainGrid.Size = new System.Drawing.Size(601, 340);
            this._mainGrid.TabIndex = 0;
            this._mainGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._jobEventView });
            this._mainGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(On_GridView_MouseDown);
            this._actionPalletControl.HostingControl = this;
            this._actionPalletControl.LegalType = null;
            this._actionPalletControl.Name = "_actionPalletControl";
            this._actionPalletControl.Size = new System.Drawing.Size(61, 4);
            this._actionPalletControl.SourceOverride = commonSerializableList;
            this._actionPalletControl.UseSourceOverride = false;
            DevExpress.XtraGrid.Columns.GridColumnCollection columns = this._jobEventView.Columns;
            DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray = new DevExpress.XtraGrid.Columns.GridColumn[8] { this._statusColumn, this._timeColumn, this._operationColumn, this._itemColumn, this._sourceColumn, this._targetColumn, this._statusTextColumn, this._informationColumn };
            columns.AddRange(gridColumnArray);
            this._jobEventView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this._jobEventView.GridControl = this._mainGrid;
            this._jobEventView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this._jobEventView.Name = "_jobEventView";
            this._jobEventView.OptionsBehavior.AutoSelectAllInEditor = false;
            this._jobEventView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this._jobEventView.OptionsBehavior.Editable = false;
            this._jobEventView.OptionsCustomization.AllowColumnMoving = false;
            this._jobEventView.OptionsCustomization.AllowFilter = false;
            this._jobEventView.OptionsCustomization.AllowGroup = false;
            this._jobEventView.OptionsDetail.AllowZoomDetail = false;
            this._jobEventView.OptionsDetail.EnableMasterViewMode = false;
            this._jobEventView.OptionsDetail.ShowDetailTabs = false;
            this._jobEventView.OptionsFilter.AllowMRUFilterList = false;
            this._jobEventView.OptionsHint.ShowCellHints = false;
            this._jobEventView.OptionsHint.ShowColumnHeaderHints = false;
            this._jobEventView.OptionsHint.ShowFooterHints = false;
            this._jobEventView.OptionsMenu.EnableColumnMenu = false;
            this._jobEventView.OptionsMenu.EnableFooterMenu = false;
            this._jobEventView.OptionsMenu.EnableGroupPanelMenu = false;
            this._jobEventView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this._jobEventView.OptionsSelection.MultiSelect = true;
            this._jobEventView.OptionsView.ColumnAutoWidth = false;
            this._jobEventView.OptionsView.ShowDetailButtons = false;
            this._jobEventView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this._jobEventView.OptionsView.ShowGroupPanel = false;
            this._jobEventView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this._jobEventView.OptionsView.ShowIndicator = false;
            this._jobEventView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this._jobEventView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this._jobEventView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(_jobEventView_RowStyle);
            this._jobEventView.TopRowChanged += new System.EventHandler(_jobEventView_TopRowChanged);
            this._jobEventView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(_jobEventView_SelectionChanged);
            this._jobEventView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(_jobEventView_FocusedRowChanged);
            this._jobEventView.DoubleClick += new System.EventHandler(On_GridVew_DoubleClicked);
            this._statusColumn.Caption = "Status";
            this._statusColumn.ColumnEdit = this.repositoryItemPictureEdit1;
            this._statusColumn.FieldName = "Status";
            this._statusColumn.ImageAlignment = System.Drawing.StringAlignment.Center;
            this._statusColumn.Name = "_statusColumn";
            this._statusColumn.OptionsColumn.AllowMove = false;
            this._statusColumn.OptionsColumn.AllowShowHide = false;
            this._statusColumn.OptionsColumn.AllowSize = false;
            this._statusColumn.OptionsColumn.ShowCaption = false;
            this._statusColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._statusColumn.Visible = true;
            this._statusColumn.VisibleIndex = 0;
            this._statusColumn.Width = 20;
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this._timeColumn.Caption = "Time";
            this._timeColumn.DisplayFormat.FormatString = "G";
            this._timeColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this._timeColumn.FieldName = "Time";
            this._timeColumn.Name = "_timeColumn";
            this._timeColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._timeColumn.Visible = true;
            this._timeColumn.VisibleIndex = 1;
            this._timeColumn.Width = 153;
            this._operationColumn.Caption = "Operation";
            this._operationColumn.FieldName = "Operation";
            this._operationColumn.Name = "_operationColumn";
            this._operationColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._operationColumn.Visible = true;
            this._operationColumn.VisibleIndex = 2;
            this._operationColumn.Width = 108;
            this._itemColumn.Caption = "Item";
            this._itemColumn.FieldName = "Item";
            this._itemColumn.Name = "_itemColumn";
            this._itemColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._itemColumn.Visible = true;
            this._itemColumn.VisibleIndex = 3;
            this._itemColumn.Width = 100;
            this._sourceColumn.Caption = "Source";
            this._sourceColumn.FieldName = "Source";
            this._sourceColumn.Name = "_sourceColumn";
            this._sourceColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._sourceColumn.Visible = true;
            this._sourceColumn.VisibleIndex = 4;
            this._sourceColumn.Width = 150;
            this._targetColumn.Caption = "Target";
            this._targetColumn.FieldName = "Target";
            this._targetColumn.Name = "_targetColumn";
            this._targetColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._targetColumn.Visible = true;
            this._targetColumn.VisibleIndex = 5;
            this._targetColumn.Width = 150;
            this._statusTextColumn.Caption = "Status";
            this._statusTextColumn.FieldName = "TextStatus";
            this._statusTextColumn.Name = "_statusTextColumn";
            this._statusTextColumn.Visible = true;
            this._statusTextColumn.VisibleIndex = 6;
            this._statusTextColumn.Width = 123;
            this._informationColumn.Caption = "Information";
            this._informationColumn.FieldName = "Information";
            this._informationColumn.Name = "_informationColumn";
            this._informationColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._informationColumn.Visible = true;
            this._informationColumn.VisibleIndex = 7;
            this._informationColumn.Width = 71;
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
            this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
            this._currentlyRunningTabPage.Controls.Add(this._runningGrid);
            this._currentlyRunningTabPage.Enabled = true;
            this._currentlyRunningTabPage.Name = "_currentlyRunningTabPage";
            this._currentlyRunningTabPage.Size = new System.Drawing.Size(607, 346);
            this._currentlyRunningTabPage.Text = "CURRENTLY RUNNING";
            this._runningGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._runningGrid.EmbeddedNavigator.ShowToolTips = false;
            this._runningGrid.Location = new System.Drawing.Point(3, 3);
            this._runningGrid.MainView = this._runningView;
            this._runningGrid.Name = "_runningGrid";
            this._runningGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemPictureEdit2 });
            this._runningGrid.Size = new System.Drawing.Size(601, 340);
            this._runningGrid.TabIndex = 0;
            this._runningGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._runningView });
            this._runningGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(On_GridView_MouseDown);
            DevExpress.XtraGrid.Columns.GridColumnCollection gridColumnCollections = this._runningView.Columns;
            DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray1 = new DevExpress.XtraGrid.Columns.GridColumn[8] { this._runningStatusColumn, this._runningStartTimeColumn, this._runningOperationColumn, this._runningItemColumn, this._runningSourceColumn, this._runningTargetColumn, this._runningInformationColumn, this._runningTextStatusColumn };
            gridColumnCollections.AddRange(gridColumnArray1);
            this._runningView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this._runningView.GridControl = this._runningGrid;
            this._runningView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this._runningView.Name = "_runningView";
            this._runningView.OptionsBehavior.AutoSelectAllInEditor = false;
            this._runningView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this._runningView.OptionsBehavior.Editable = false;
            this._runningView.OptionsCustomization.AllowColumnMoving = false;
            this._runningView.OptionsCustomization.AllowFilter = false;
            this._runningView.OptionsCustomization.AllowGroup = false;
            this._runningView.OptionsDetail.AllowZoomDetail = false;
            this._runningView.OptionsDetail.EnableMasterViewMode = false;
            this._runningView.OptionsDetail.ShowDetailTabs = false;
            this._runningView.OptionsDetail.SmartDetailExpand = false;
            this._runningView.OptionsFilter.AllowColumnMRUFilterList = false;
            this._runningView.OptionsFilter.AllowMRUFilterList = false;
            this._runningView.OptionsHint.ShowCellHints = false;
            this._runningView.OptionsHint.ShowColumnHeaderHints = false;
            this._runningView.OptionsHint.ShowFooterHints = false;
            this._runningView.OptionsMenu.EnableColumnMenu = false;
            this._runningView.OptionsMenu.EnableFooterMenu = false;
            this._runningView.OptionsMenu.EnableGroupPanelMenu = false;
            this._runningView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this._runningView.OptionsSelection.EnableAppearanceFocusedRow = false;
            this._runningView.OptionsSelection.EnableAppearanceHideSelection = false;
            this._runningView.OptionsView.ColumnAutoWidth = false;
            this._runningView.OptionsView.ShowDetailButtons = false;
            this._runningView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this._runningView.OptionsView.ShowGroupPanel = false;
            this._runningView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this._runningView.OptionsView.ShowIndicator = false;
            this._runningView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this._runningView.SynchronizeClones = false;
            this._runningView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(_runningView_FocusedRowChanged);
            this._runningStatusColumn.Caption = "Status";
            this._runningStatusColumn.ColumnEdit = this.repositoryItemPictureEdit2;
            this._runningStatusColumn.FieldName = "Status";
            this._runningStatusColumn.Name = "_runningStatusColumn";
            this._runningStatusColumn.OptionsColumn.AllowMove = false;
            this._runningStatusColumn.OptionsColumn.AllowShowHide = false;
            this._runningStatusColumn.OptionsColumn.AllowSize = false;
            this._runningStatusColumn.OptionsColumn.ShowCaption = false;
            this._runningStatusColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningStatusColumn.Visible = true;
            this._runningStatusColumn.VisibleIndex = 0;
            this._runningStatusColumn.Width = 20;
            this.repositoryItemPictureEdit2.Name = "repositoryItemPictureEdit2";
            this._runningStartTimeColumn.Caption = "Started";
            this._runningStartTimeColumn.DisplayFormat.FormatString = "G";
            this._runningStartTimeColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this._runningStartTimeColumn.FieldName = "Time";
            this._runningStartTimeColumn.Name = "_runningStartTimeColumn";
            this._runningStartTimeColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningStartTimeColumn.Visible = true;
            this._runningStartTimeColumn.VisibleIndex = 1;
            this._runningStartTimeColumn.Width = 153;
            this._runningOperationColumn.Caption = "Operation";
            this._runningOperationColumn.FieldName = "Operation";
            this._runningOperationColumn.Name = "_runningOperationColumn";
            this._runningOperationColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningOperationColumn.Visible = true;
            this._runningOperationColumn.VisibleIndex = 2;
            this._runningOperationColumn.Width = 108;
            this._runningItemColumn.Caption = "Item";
            this._runningItemColumn.FieldName = "Item";
            this._runningItemColumn.Name = "_runningItemColumn";
            this._runningItemColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningItemColumn.Visible = true;
            this._runningItemColumn.VisibleIndex = 3;
            this._runningItemColumn.Width = 100;
            this._runningSourceColumn.Caption = "Source";
            this._runningSourceColumn.FieldName = "Source";
            this._runningSourceColumn.Name = "_runningSourceColumn";
            this._runningSourceColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningSourceColumn.Visible = true;
            this._runningSourceColumn.VisibleIndex = 4;
            this._runningSourceColumn.Width = 100;
            this._runningTargetColumn.Caption = "Target";
            this._runningTargetColumn.FieldName = "Target";
            this._runningTargetColumn.Name = "_runningTargetColumn";
            this._runningTargetColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningTargetColumn.Visible = true;
            this._runningTargetColumn.VisibleIndex = 5;
            this._runningTargetColumn.Width = 100;
            this._runningInformationColumn.Caption = "Information";
            this._runningInformationColumn.FieldName = "Information";
            this._runningInformationColumn.Name = "_runningInformationColumn";
            this._runningInformationColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this._runningInformationColumn.Visible = true;
            this._runningInformationColumn.VisibleIndex = 6;
            this._runningInformationColumn.Width = 71;
            this._runningTextStatusColumn.Caption = "TextStatus";
            this._runningTextStatusColumn.FieldName = "TextStatus";
            this._runningTextStatusColumn.Name = "_runningTextStatusColumn";
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this._tabControl);
            base.Name = "LogItemListControl";
            base.Size = new System.Drawing.Size(613, 374);
            ((System.ComponentModel.ISupportInitialize)this._tabControl).EndInit();
            this._tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._mainGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._jobEventView).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._runningGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._runningView).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit2).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeGridBindings()
        {
            _logItemMap = new Dictionary<string, DisplayedLogItem>();
            _dataRowMap = new Dictionary<DataRow, DisplayedLogItem>();
            _mainTable = new DataTable();
            _runningTable = new DataTable();
            AddColumnsToDataTable(_mainTable);
            AddColumnsToDataTable(_runningTable);
            ReattachGridSources();
        }

        private void InitializeStatusFilter()
        {
            _allowedStatuses = new List<ActionOperationStatus>();
            foreach (ActionOperationStatus value in Enum.GetValues(typeof(ActionOperationStatus)))
            {
                _allowedStatuses.Add(value);
            }
        }

        private static void InitializeStatusImageMap()
        {
            s_statusImageMap = new Dictionary<ActionOperationStatus, Image>
            {
                {
                    ActionOperationStatus.Completed,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Completed
                },
                {
                    ActionOperationStatus.Different,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Different
                },
                {
                    ActionOperationStatus.Failed,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Failed
                },
                {
                    ActionOperationStatus.Idle,
                    null
                },
                {
                    ActionOperationStatus.MissingOnSource,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_MissingOnSource
                },
                {
                    ActionOperationStatus.MissingOnTarget,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_MissingOnTarget
                },
                {
                    ActionOperationStatus.Running,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Running
                },
                {
                    ActionOperationStatus.Same,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Same
                },
                {
                    ActionOperationStatus.Skipped,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Skipped
                },
                {
                    ActionOperationStatus.SkippedInEvaluationLicense,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Skipped
                },
                {
                    ActionOperationStatus.Warning,
                    Metalogix.UI.WinForms.Properties.Resources.Item_Status_Warning
                }
            };
        }

        private void InitializeUpdateClock()
        {
            _updateClock = new System.Timers.Timer(1000.0);
            _updateClock.Elapsed += UpdateClockTick;
            _updateClock.AutoReset = false;
            _updateClock.Enabled = false;
            _updateClockTicking = false;
            _updateClockPaused = false;
        }

        public void MergeRunningAndFinished()
        {
            SuspendLayout();
            try
            {
                _mainGrid.Parent = this;
                _mainGrid.Location = new Point(0, 0);
                _mainGrid.Size = base.Size;
                _tabControl.Visible = false;
                LogItemsMerged = true;
            }
            finally
            {
                ResumeLayout();
            }
            ReloadUI();
        }

        private void On_dataSource_ListChanged(ChangeType changeType, LogItem item)
        {
            if (item == null)
            {
                return;
            }
            _changeTrackerLock.EnterReadLock();
            try
            {
                _changedItems[item.ID] = item;
            }
            finally
            {
                _changeTrackerLock.ExitReadLock();
            }
        }

        private void On_GridVew_DoubleClicked(object sender, EventArgs e)
        {
            if (GetMouseInGridRow((GridView)sender))
            {
                LogItemDetailsAction logItemDetailsAction = new LogItemDetailsAction();
                IXMLAbleList controlCollection = new Metalogix.UI.WinForms.Components.ControlCollection(this);
                IXMLAbleList selectedObjects = SelectedObjects;
                if (logItemDetailsAction.AppliesTo(controlCollection, selectedObjects) && logItemDetailsAction.EnabledOn(controlCollection, selectedObjects))
                {
                    logItemDetailsAction.Run(controlCollection, selectedObjects);
                }
            }
        }

        private void On_GridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ClipBoard.SetDataObject(new Metalogix.UI.WinForms.Components.ControlCollection(this));
            }
            else if (e.Button == MouseButtons.Left && GetCurrentGridControl() == _mainGrid && GetMouseInGridRow((GridView)_mainGrid.MainView))
            {
                UpdateAutoScrollingOptions(activate: false);
            }
        }

        public void PauseUpdating()
        {
            lock (_updateClock)
            {
                _updateClockPaused = true;
            }
        }

        private void ReattachGridSources()
        {
            if (_mainGrid.DataSource == null)
            {
                _mainGrid.DataSource = _mainTable;
            }
            if (_runningGrid.DataSource == null)
            {
                _runningGrid.DataSource = _runningTable;
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
                _mainGrid.BeginUpdate();
                _runningGrid.BeginUpdate();
                try
                {
                    DetachGridSources();
                    _logItemMap.Clear();
                    _mainTable.Rows.Clear();
                    _runningTable.Rows.Clear();
                    if (DataSource != null)
                    {
                        LogItem[] currentLogItems = DataSource.GetCurrentLogItems();
                        for (int i = 0; i < currentLogItems.Length; i++)
                        {
                            AddOrUpdateLogItem(currentLogItems[i]);
                        }
                    }
                    FireUIUpdated();
                    UpdateRunningColumnName();
                    AutoSizeInformationColumns();
                }
                finally
                {
                    ReattachGridSources();
                    _runningGrid.EndUpdate();
                    _mainGrid.EndUpdate();
                    ResumeLayout();
                }
            }
            finally
            {
                _uiUpdateLock.ExitWriteLock();
            }
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

        private void ScrollToBottom()
        {
            if (AutoScrollToBottom && _autoScrollActive)
            {
                GridView mainView = (GridView)_mainGrid.MainView;
                if (mainView != null && mainView.DataRowCount > 0)
                {
                    mainView.TopRowIndex = mainView.DataRowCount - 1;
                }
            }
        }

        public void SelectNextIndex()
        {
            RunInReadLock(_uiUpdateLock, delegate
            {
                GridView gridView = (GridView)_mainGrid.MainView;
                if (gridView != null && gridView.FocusedRowHandle != int.MinValue)
                {
                    int nextVisibleRow = gridView.GetNextVisibleRow(gridView.GetVisibleIndex(gridView.FocusedRowHandle));
                    if (nextVisibleRow != int.MinValue)
                    {
                        int visibleRowHandle = gridView.GetVisibleRowHandle(nextVisibleRow);
                        gridView.ClearSelection();
                        gridView.SelectRow(visibleRowHandle);
                        gridView.FocusedRowHandle = visibleRowHandle;
                    }
                }
            });
        }

        public void SelectPreviousIndex()
        {
            RunInReadLock(_uiUpdateLock, delegate
            {
                GridView gridView = (GridView)_mainGrid.MainView;
                if (gridView != null && gridView.FocusedRowHandle != int.MinValue)
                {
                    int prevVisibleRow = gridView.GetPrevVisibleRow(gridView.GetVisibleIndex(gridView.FocusedRowHandle));
                    if (prevVisibleRow != int.MinValue)
                    {
                        int visibleRowHandle = gridView.GetVisibleRowHandle(prevVisibleRow);
                        gridView.ClearSelection();
                        gridView.SelectRow(visibleRowHandle);
                        gridView.FocusedRowHandle = visibleRowHandle;
                    }
                }
            });
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

        private void UpdateAutoScrollingOptions(bool activate)
        {
            _autoScrollActive = activate;
        }

        private void UpdateClockTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!base.IsDisposed && base.IsHandleCreated)
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
                    if (!base.IsDisposed && _updateClockTicking && !_updateClockPaused)
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

        private void UpdateRunningColumnName()
        {
            int count = _runningTable.Rows.Count;
            if (count == 0)
            {
                _currentlyRunningTabPage.Text = Metalogix.Actions.Properties.Resources.CurrentlyRunningTabName;
            }
            else
            {
                _currentlyRunningTabPage.Text = $"{Metalogix.Actions.Properties.Resources.CurrentlyRunningTabName} ({count.ToString(CultureInfo.InvariantCulture)})";
            }
        }
    }
}
