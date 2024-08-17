using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.UI.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class LogItemViewer : XtraForm
    {
        private BarSubItemLink _showItemsLink;

        private BarSubItemLink _withStatusLink;

        private Point _startLocation;

        private IContainer components;

        private LogItemListControl _logItemListControl;

        private Button w_btnCancel;

        private BarManager _barManager;

        private Bar _topBar;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarSubItem _barShowLogItems;

        private BarSubItem _barWithStatus;

        private BarCheckItem _barCheckFailure;

        private BarCheckItem _barCheckDifferent;

        private BarCheckItem _barCheckMissingOnSource;

        private BarCheckItem _barCheckMissingOnTarget;

        private BarCheckItem _barCheckSkipped;

        private BarCheckItem _barCheckWarning;

        private BarCheckItem _barCheckCompleted;

        private BarLargeButtonItem _barBtnShowAll;

        private BarLargeButtonItem _barBtnShowNone;

        private BarCheckItem barCheckItem1;

        public LogItemCollection DataSource
        {
            get
            {
                return this._logItemListControl.DataSource;
            }
            set
            {
                this._logItemListControl.DataSource = value;
            }
        }

        public Point StartLocation
        {
            get
            {
                return this._startLocation;
            }
            set
            {
                this._startLocation.X = value.X;
                this._startLocation.Y = value.Y;
            }
        }

        public LogItemViewer()
        {
            this.InitializeComponent();
            base.Icon = Icon.FromHandle(Resources.ViewLogs16.GetHicon());
            this.StartLocation = base.Location;
            this._logItemListControl.MergeRunningAndFinished();
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
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LogItemViewer));
            this.w_btnCancel = new Button();
            this._logItemListControl = new LogItemListControl();
            this._barManager = new BarManager(this.components);
            this._topBar = new Bar();
            this._barShowLogItems = new BarSubItem();
            this._barWithStatus = new BarSubItem();
            this._barCheckFailure = new BarCheckItem();
            this._barCheckDifferent = new BarCheckItem();
            this._barCheckMissingOnSource = new BarCheckItem();
            this._barCheckMissingOnTarget = new BarCheckItem();
            this._barCheckSkipped = new BarCheckItem();
            this._barCheckWarning = new BarCheckItem();
            this._barCheckCompleted = new BarCheckItem();
            this._barBtnShowAll = new BarLargeButtonItem();
            this._barBtnShowNone = new BarLargeButtonItem();
            this.barDockControlTop = new BarDockControl();
            this.barDockControlBottom = new BarDockControl();
            this.barDockControlLeft = new BarDockControl();
            this.barDockControlRight = new BarDockControl();
            this.barCheckItem1 = new BarCheckItem();
            ((ISupportInitialize)this._barManager).BeginInit();
            base.SuspendLayout();
            this.w_btnCancel.DialogResult = DialogResult.Cancel;
            componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.Name = "w_btnCancel";
            this.w_btnCancel.UseVisualStyleBackColor = true;
            this._logItemListControl.AutoScrollToBottom = false;
            this._logItemListControl.DataSource = null;
            componentResourceManager.ApplyResources(this._logItemListControl, "_logItemListControl");
            this._logItemListControl.Name = "_logItemListControl";
            this._barManager.AllowCustomization = false;
            this._barManager.AllowMoveBarOnToolbar = false;
            this._barManager.AllowQuickCustomization = false;
            this._barManager.Bars.AddRange(new Bar[] { this._topBar });
            this._barManager.DockControls.Add(this.barDockControlTop);
            this._barManager.DockControls.Add(this.barDockControlBottom);
            this._barManager.DockControls.Add(this.barDockControlLeft);
            this._barManager.DockControls.Add(this.barDockControlRight);
            this._barManager.Form = this;
            BarItems items = this._barManager.Items;
            BarItem[] barItemArray = new BarItem[] { this._barShowLogItems, this._barWithStatus, this.barCheckItem1, this._barCheckFailure, this._barCheckDifferent, this._barCheckMissingOnSource, this._barCheckMissingOnTarget, this._barCheckSkipped, this._barCheckWarning, this._barCheckCompleted, this._barBtnShowAll, this._barBtnShowNone };
            items.AddRange(barItemArray);
            this._barManager.MaxItemId = 13;
            this._topBar.BarName = "Tools";
            this._topBar.DockCol = 0;
            this._topBar.DockRow = 0;
            this._topBar.DockStyle = BarDockStyle.Top;
            LinksInfo linksPersistInfo = this._topBar.LinksPersistInfo;
            LinkPersistInfo[] linkPersistInfo = new LinkPersistInfo[] { new LinkPersistInfo(this._barShowLogItems) };
            linksPersistInfo.AddRange(linkPersistInfo);
            this._topBar.OptionsBar.DrawDragBorder = false;
            componentResourceManager.ApplyResources(this._topBar, "_topBar");
            componentResourceManager.ApplyResources(this._barShowLogItems, "_barShowLogItems");
            this._barShowLogItems.Id = 0;
            LinksInfo linksInfo = this._barShowLogItems.LinksPersistInfo;
            LinkPersistInfo[] linkPersistInfoArray = new LinkPersistInfo[] { new LinkPersistInfo(this._barWithStatus) };
            linksInfo.AddRange(linkPersistInfoArray);
            this._barShowLogItems.Name = "_barShowLogItems";
            componentResourceManager.ApplyResources(this._barWithStatus, "_barWithStatus");
            this._barWithStatus.Id = 2;
            LinksInfo linksPersistInfo1 = this._barWithStatus.LinksPersistInfo;
            LinkPersistInfo[] linkPersistInfo1 = new LinkPersistInfo[] { new LinkPersistInfo(this._barCheckFailure), new LinkPersistInfo(this._barCheckDifferent), new LinkPersistInfo(this._barCheckMissingOnSource), new LinkPersistInfo(this._barCheckMissingOnTarget), new LinkPersistInfo(this._barCheckSkipped), new LinkPersistInfo(this._barCheckWarning), new LinkPersistInfo(this._barCheckCompleted), new LinkPersistInfo(this._barBtnShowAll, true), new LinkPersistInfo(this._barBtnShowNone, true) };
            linksPersistInfo1.AddRange(linkPersistInfo1);
            this._barWithStatus.Name = "_barWithStatus";
            componentResourceManager.ApplyResources(this._barCheckFailure, "_barCheckFailure");
            this._barCheckFailure.Checked = true;
            this._barCheckFailure.Id = 4;
            this._barCheckFailure.Name = "_barCheckFailure";
            this._barCheckFailure.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckDifferent, "_barCheckDifferent");
            this._barCheckDifferent.Checked = true;
            this._barCheckDifferent.Id = 5;
            this._barCheckDifferent.Name = "_barCheckDifferent";
            this._barCheckDifferent.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckMissingOnSource, "_barCheckMissingOnSource");
            this._barCheckMissingOnSource.Checked = true;
            this._barCheckMissingOnSource.Id = 6;
            this._barCheckMissingOnSource.Name = "_barCheckMissingOnSource";
            this._barCheckMissingOnSource.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckMissingOnTarget, "_barCheckMissingOnTarget");
            this._barCheckMissingOnTarget.Checked = true;
            this._barCheckMissingOnTarget.Id = 7;
            this._barCheckMissingOnTarget.Name = "_barCheckMissingOnTarget";
            this._barCheckMissingOnTarget.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckSkipped, "_barCheckSkipped");
            this._barCheckSkipped.Checked = true;
            this._barCheckSkipped.Id = 8;
            this._barCheckSkipped.Name = "_barCheckSkipped";
            this._barCheckSkipped.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckWarning, "_barCheckWarning");
            this._barCheckWarning.Checked = true;
            this._barCheckWarning.Id = 9;
            this._barCheckWarning.Name = "_barCheckWarning";
            this._barCheckWarning.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barCheckCompleted, "_barCheckCompleted");
            this._barCheckCompleted.Checked = true;
            this._barCheckCompleted.Id = 10;
            this._barCheckCompleted.Name = "_barCheckCompleted";
            this._barCheckCompleted.ItemClick += new ItemClickEventHandler(this.On_StatusFilterFlag_Clicked);
            componentResourceManager.ApplyResources(this._barBtnShowAll, "_barBtnShowAll");
            this._barBtnShowAll.Id = 11;
            this._barBtnShowAll.Name = "_barBtnShowAll";
            this._barBtnShowAll.ItemClick += new ItemClickEventHandler(this.On_ShowAll_Clicked);
            componentResourceManager.ApplyResources(this._barBtnShowNone, "_barBtnShowNone");
            this._barBtnShowNone.Id = 12;
            this._barBtnShowNone.Name = "_barBtnShowNone";
            this._barBtnShowNone.ItemClick += new ItemClickEventHandler(this.On_ShowNone_Clicked);
            this.barDockControlTop.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlTop, "barDockControlTop");
            this.barDockControlBottom.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            this.barDockControlLeft.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            this.barDockControlRight.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlRight, "barDockControlRight");
            componentResourceManager.ApplyResources(this.barCheckItem1, "barCheckItem1");
            this.barCheckItem1.Id = 3;
            this.barCheckItem1.Name = "barCheckItem1";
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this._logItemListControl);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.Name = "LogItemViewer";
            base.FormClosing += new FormClosingEventHandler(this.On_Closing);
            base.Load += new EventHandler(this.On_Load);
            base.Shown += new EventHandler(this.On_Shown);
            ((ISupportInitialize)this._barManager).EndInit();
            base.ResumeLayout(false);
        }

        private void On_Closing(object sender, FormClosingEventArgs e)
        {
            this.StartLocation = base.Location;
        }

        private void On_Load(object sender, EventArgs e)
        {
            base.Location = this.StartLocation;
        }

        private void On_ShowAll_Clicked(object sender, ItemClickEventArgs e)
        {
            this.SetAllFilterFlags(true);
        }

        private void On_Shown(object sender, EventArgs e)
        {
            this._showItemsLink = this._barShowLogItems.Links[0] as BarSubItemLink;
            this._withStatusLink = this._barWithStatus.Links[0] as BarSubItemLink;
        }

        private void On_ShowNone_Clicked(object sender, ItemClickEventArgs e)
        {
            this.SetAllFilterFlags(false);
        }

        private void On_StatusFilterFlag_Clicked(object sender, ItemClickEventArgs e)
        {
            this._showItemsLink.OpenMenu();
            this._withStatusLink.OpenMenu();
            this.UpdateLogItemStatusFilter();
        }

        private void SetAllFilterFlags(bool value)
        {
            this._barCheckFailure.Checked = value;
            this._barCheckDifferent.Checked = value;
            this._barCheckMissingOnSource.Checked = value;
            this._barCheckMissingOnTarget.Checked = value;
            this._barCheckSkipped.Checked = value;
            this._barCheckWarning.Checked = value;
            this._barCheckCompleted.Checked = value;
            this._showItemsLink.OpenMenu();
            this._withStatusLink.OpenMenu();
            this.UpdateLogItemStatusFilter();
        }

        private void UpdateLogItemStatusFilter()
        {
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.Failed, this._barCheckFailure.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.Different, this._barCheckDifferent.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.MissingOnSource, this._barCheckMissingOnSource.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.MissingOnTarget, this._barCheckMissingOnTarget.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.Skipped, this._barCheckSkipped.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.SkippedInEvaluationLicense, this._barCheckSkipped.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.Warning, this._barCheckWarning.Checked, false);
            this._logItemListControl.ChangeLogItemStatusFilter(ActionOperationStatus.Completed, this._barCheckCompleted.Checked, false);
            this._logItemListControl.CommitLogItemStatusFilterUpdates();
        }
    }
}