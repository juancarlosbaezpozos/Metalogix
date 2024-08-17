using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraWaitForm;
using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Blocker;
using Metalogix.Actions.Properties;
using Metalogix.Jobs;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs
{
    public partial class JobStatusDialog : XtraForm
    {
        private const int SHOWN_DETAILS_MAXIMUM_HEIGHT = 1500;

        private const int ABSOLUTE_MINIMUM_WIDTH = 500;

        private const int WIDTH_MINUS_VARIABLE_SIZE_CONTROLS = 196;

        private static Dictionary<ActionStatus, Image> s_actonStatusImageMap;

        private readonly static object s_actionStatusImageMapLock;

        private int _hiddenDetailsHeight = 301;

        private int _showDetailsMinimumHeight = 400;

        private int _showDetailsDefaultHeight = 600;

        private int _oldStatusHeight;

        private FormWindowState? _prevWindowState = null;

        private ActionStatusChangedHandler m_actionStatusChanged;

        private ActionEventHandler m_actionOperationChanged;

        private ActionStartedEventHandler m_actionStarted;

        private ActionLinkChangedHandler m_actionSourceLinkChanged;

        private ActionLinkChangedHandler m_actionTargetLinkChanged;

        private ActionBlockerHandler m_actionBlocked;

        private bool _isActionBlocked;

        private IContainer components;

        private LabelControl _jobLabel;

        private LabelControl _jobNameLabel;

        private LabelControl _dataMigratedValueLabel;

        private LabelControl _dataMigratedLabel;

        private LabelControl _startTimeLabel;

        private LabelControl _startTimeValueLabel;

        private LabelControl _elapsedTimeLabel;

        private LabelControl _sourceLabel;

        private HyperLinkEdit _sourceValueLabel;

        private LabelControl _targetLabel;

        private HyperLinkEdit _targetValueLabel;

        private PanelControl _marqueePanel;

        private SimpleButton _cancelCloseButton;

        private SimpleButton _pauseResumeButton;

        private SimpleButton _detailsButton;

        private LogDetailCountsControl _progressDetailsControl;

        private LogItemListControl _logItemDetailsControl;

        private ExtendedMarqueeWheel _marqueeWheel;

        private SimpleIntervalClock _intervalClock;

        private LogDetailCountsControl _statusDetailControl;

        private PanelControl panelControl1;

        protected Metalogix.Actions.Action Action
        {
            get
            {
                return this.Job.Action;
            }
        }

        protected static Dictionary<ActionStatus, Image> ActionStatusImageMap
        {
            get
            {
                Dictionary<ActionStatus, Image> sActonStatusImageMap = JobStatusDialog.s_actonStatusImageMap;
                if (sActonStatusImageMap == null)
                {
                    lock (JobStatusDialog.s_actionStatusImageMapLock)
                    {
                        sActonStatusImageMap = JobStatusDialog.s_actonStatusImageMap;
                        if (sActonStatusImageMap == null)
                        {
                            JobStatusDialog.InitializeActionStatusImageMap();
                            sActonStatusImageMap = JobStatusDialog.s_actonStatusImageMap;
                        }
                    }
                }
                return sActonStatusImageMap;
            }
        }

        protected bool DetailsVisible
        {
            get
            {
                return this.MinimumSize.Height != this.MaximumSize.Height;
            }
        }

        protected Job Job
        {
            get;
            private set;
        }

        protected CompletionDetailsOrderProvider ProgressOrderProvider
        {
            get;
            set;
        }

        protected ActionStatusLabelProvider StatusLabelProvider
        {
            get;
            set;
        }

        protected ActionStatusSummaryProvider SummaryProvider
        {
            get;
            set;
        }

        static JobStatusDialog()
        {
            JobStatusDialog.s_actionStatusImageMapLock = new object();
        }

        public JobStatusDialog(Job job)
        {
            this.InitializeComponent();
            this.Job = job;
            this.StatusLabelProvider = this.Action.GetStatusLabelProvider();
            this.SummaryProvider = this.Action.GetStatusSummaryProvider();
            this.ProgressOrderProvider = this.Action.GetCompletionDetailsOrderProvider();
            this._oldStatusHeight = this._statusDetailControl.Height;
            this._logItemDetailsControl.ChangeLogItemStatusFilter(ActionOperationStatus.Skipped, false, true);
            this.AttachToJobEvents();
            this.UpdateUIStatus();
            this.UpdateUIOperation();
            this.UpdateMarqueeMessage();
            if (!base.DesignMode)
            {
                this.CollapseDetailsSection();
                this.UpdateProgressDetails();
            }
        }

        private void _cancelCloseButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void _detailsButton_Click(object sender, EventArgs e)
        {
            if (this.DetailsVisible)
            {
                this.CollapseDetailsSection();
                return;
            }
            if (this._logItemDetailsControl.DataSource == null)
            {
                this._logItemDetailsControl.DataSource = this.Job.Log;
            }
            this.ExpandDetailsSection();
        }

        private void _marqueeWheel_SizeChanged(object sender, EventArgs e)
        {
            int width = (this._marqueePanel.Width - this._marqueeWheel.Width) / 2;
            int height = (this._marqueePanel.Height - this._marqueeWheel.Height) / 2;
            this._marqueeWheel.Location = new Point(width, height);
        }

        private void _pauseResumeButton_Click(object sender, EventArgs e)
        {
            if (this.Action.Status == ActionStatus.Running)
            {
                this._intervalClock.StopClock();
                this.Action.Pause();
                return;
            }
            if (this.Action.Status == ActionStatus.Paused)
            {
                this.Action.Resume();
                this._intervalClock.StartClock();
            }
        }

        private void _statusDetailControl_Resize(object sender, EventArgs e)
        {
            this.UpdateControlsForStatusSummaryResize(this._statusDetailControl.Height - this._oldStatusHeight);
            this._oldStatusHeight = this._statusDetailControl.Height;
        }

        private void AttachToJobEvents()
        {
            this.m_actionStatusChanged = new ActionStatusChangedHandler(this.On_action_StatusChanged);
            this.Job.ActionStatusChanged += this.m_actionStatusChanged;
            this.m_actionOperationChanged = new ActionEventHandler(this.On_action_OperationChanged);
            this.Job.ActionOperationChanged += this.m_actionOperationChanged;
            this.m_actionStarted = new ActionStartedEventHandler(this.On_action_Started);
            this.Job.ActionStarting += this.m_actionStarted;
            this.m_actionSourceLinkChanged = new ActionLinkChangedHandler(this.On_action_sourceLinkChanged);
            this.Job.Action.SourceLinkChanged += this.m_actionSourceLinkChanged;
            this.m_actionTargetLinkChanged = new ActionLinkChangedHandler(this.On_action_targetLinkChanged);
            this.Job.Action.TargetLinkChanged += this.m_actionTargetLinkChanged;
            this.m_actionBlocked = new ActionBlockerHandler(this.On_action_Blocked);
            this.Job.ActionBlocked += this.m_actionBlocked;
        }

        private void CollapseDetailsSection()
        {
            if (this.DetailsVisible)
            {
                this._detailsButton.Text = string.Concat(Metalogix.Actions.Properties.Resources.DetailsButtonText, " >>");
                Size size = new Size(this.MaximumSize.Width, this._hiddenDetailsHeight);
                Size minimumSize = this.MinimumSize;
                this.MinimumSize = new Size(minimumSize.Width, this._hiddenDetailsHeight);
                this.MaximumSize = size;
                this._logItemDetailsControl.Visible = false;
            }
        }

        private void DetachFromJobEvents()
        {
            this.Job.ActionStatusChanged -= this.m_actionStatusChanged;
            this.Job.ActionOperationChanged -= this.m_actionOperationChanged;
            this.Job.ActionStarting -= this.m_actionStarted;
            this.Job.Action.SourceLinkChanged -= this.m_actionSourceLinkChanged;
            this.Job.Action.TargetLinkChanged -= this.m_actionTargetLinkChanged;
            this.Job.ActionBlocked -= this.m_actionBlocked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ExpandDetailsSection()
        {
            if (!this.DetailsVisible)
            {
                this._detailsButton.Text = string.Concat("<< ", Metalogix.Actions.Properties.Resources.DetailsButtonText);
                Size size = new Size(this.MaximumSize.Width, 1500);
                Size size1 = new Size(base.Size.Width, this._showDetailsDefaultHeight);
                Size size2 = new Size(this.MinimumSize.Width, this._showDetailsMinimumHeight);
                this.MaximumSize = size;
                base.Size = size1;
                this.MinimumSize = size2;
                LogItemListControl logItemListControl = this._logItemDetailsControl;
                int width = this._logItemDetailsControl.Size.Width;
                int y = this._detailsButton.Location.Y;
                Point location = this._logItemDetailsControl.Location;
                logItemListControl.Size = new Size(width, y - location.Y - 6);
                this._logItemDetailsControl.Visible = true;
            }
        }

        private static void InitializeActionStatusImageMap()
        {
            JobStatusDialog.s_actonStatusImageMap = new Dictionary<ActionStatus, Image>()
            {
                { ActionStatus.Aborted,Metalogix.UI.WinForms.Properties.Resources.JobStatus_Aborted_32 },
                { ActionStatus.Aborting, Metalogix.UI.WinForms.Properties.Resources.JobStatus_Aborted_32 },
                { ActionStatus.Done, Metalogix.UI.WinForms.Properties.Resources.JobStatus_Complete_32 },
                { ActionStatus.Failed, Metalogix.UI.WinForms.Properties.Resources.JobStatus_Failed_32 },
                { ActionStatus.NotRunning, null },
                { ActionStatus.Paused, Metalogix.UI.WinForms.Properties.Resources.JobStatus_Paused_32 },
                { ActionStatus.Running, null },
                { ActionStatus.Warning, Metalogix.UI.WinForms.Properties.Resources.JobStatus_Warning_32 }
            };
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(JobStatusDialog));
            this._jobLabel = new LabelControl();
            this._jobNameLabel = new LabelControl();
            this._dataMigratedValueLabel = new LabelControl();
            this._dataMigratedLabel = new LabelControl();
            this._startTimeLabel = new LabelControl();
            this._startTimeValueLabel = new LabelControl();
            this._elapsedTimeLabel = new LabelControl();
            this._sourceLabel = new LabelControl();
            this._sourceValueLabel = new HyperLinkEdit();
            this._targetLabel = new LabelControl();
            this._targetValueLabel = new HyperLinkEdit();
            this._marqueePanel = new PanelControl();
            this._cancelCloseButton = new SimpleButton();
            this._pauseResumeButton = new SimpleButton();
            this._detailsButton = new SimpleButton();
            this.panelControl1 = new PanelControl();
            this._marqueeWheel = new ExtendedMarqueeWheel();
            this._progressDetailsControl = new LogDetailCountsControl();
            this._logItemDetailsControl = new LogItemListControl();
            this._intervalClock = new SimpleIntervalClock();
            this._statusDetailControl = new LogDetailCountsControl();
            ((ISupportInitialize)this._sourceValueLabel.Properties).BeginInit();
            ((ISupportInitialize)this._targetValueLabel.Properties).BeginInit();
            ((ISupportInitialize)this._marqueePanel).BeginInit();
            this._marqueePanel.SuspendLayout();
            ((ISupportInitialize)this.panelControl1).BeginInit();
            base.SuspendLayout();
            this._jobLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_jobLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._jobLabel, "_jobLabel");
            this._jobLabel.Name = "_jobLabel";
            this._jobNameLabel.AutoEllipsis = true;
            componentResourceManager.ApplyResources(this._jobNameLabel, "_jobNameLabel");
            this._jobNameLabel.Name = "_jobNameLabel";
            this._dataMigratedValueLabel.AutoEllipsis = true;
            componentResourceManager.ApplyResources(this._dataMigratedValueLabel, "_dataMigratedValueLabel");
            this._dataMigratedValueLabel.Name = "_dataMigratedValueLabel";
            this._dataMigratedLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_dataMigratedLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._dataMigratedLabel, "_dataMigratedLabel");
            this._dataMigratedLabel.Name = "_dataMigratedLabel";
            this._startTimeLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_startTimeLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._startTimeLabel, "_startTimeLabel");
            this._startTimeLabel.Name = "_startTimeLabel";
            this._startTimeValueLabel.AutoEllipsis = true;
            componentResourceManager.ApplyResources(this._startTimeValueLabel, "_startTimeValueLabel");
            this._startTimeValueLabel.Name = "_startTimeValueLabel";
            this._elapsedTimeLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_elapsedTimeLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._elapsedTimeLabel, "_elapsedTimeLabel");
            this._elapsedTimeLabel.Name = "_elapsedTimeLabel";
            this._sourceLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_sourceLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._sourceLabel, "_sourceLabel");
            this._sourceLabel.Name = "_sourceLabel";
            componentResourceManager.ApplyResources(this._sourceValueLabel, "_sourceValueLabel");
            this._sourceValueLabel.Name = "_sourceValueLabel";
            this._sourceValueLabel.Properties.AllowFocused = false;
            this._sourceValueLabel.Properties.AllowMouseWheel = false;
            this._sourceValueLabel.Properties.AppearanceDisabled.BackColor = (Color)componentResourceManager.GetObject("_sourceValueLabel.Properties.AppearanceDisabled.BackColor");
            this._sourceValueLabel.Properties.AppearanceDisabled.BackColor2 = (Color)componentResourceManager.GetObject("_sourceValueLabel.Properties.AppearanceDisabled.BackColor2");
            this._sourceValueLabel.Properties.AppearanceDisabled.BorderColor = (Color)componentResourceManager.GetObject("_sourceValueLabel.Properties.AppearanceDisabled.BorderColor");
            this._sourceValueLabel.Properties.AppearanceDisabled.ForeColor = (Color)componentResourceManager.GetObject("_sourceValueLabel.Properties.AppearanceDisabled.ForeColor");
            this._sourceValueLabel.Properties.AppearanceDisabled.Options.UseBackColor = true;
            this._sourceValueLabel.Properties.AppearanceDisabled.Options.UseBorderColor = true;
            this._sourceValueLabel.Properties.AppearanceDisabled.Options.UseForeColor = true;
            this._sourceValueLabel.Properties.BorderStyle = BorderStyles.NoBorder;
            this._sourceValueLabel.EditValueChanged += new EventHandler(this.On_LinkValueChanged);
            this._targetLabel.Appearance.Font = (Font)componentResourceManager.GetObject("_targetLabel.Appearance.Font");
            componentResourceManager.ApplyResources(this._targetLabel, "_targetLabel");
            this._targetLabel.Name = "_targetLabel";
            componentResourceManager.ApplyResources(this._targetValueLabel, "_targetValueLabel");
            this._targetValueLabel.Name = "_targetValueLabel";
            this._targetValueLabel.Properties.AllowFocused = false;
            this._targetValueLabel.Properties.AppearanceDisabled.BackColor = (Color)componentResourceManager.GetObject("_targetValueLabel.Properties.AppearanceDisabled.BackColor");
            this._targetValueLabel.Properties.AppearanceDisabled.BackColor2 = (Color)componentResourceManager.GetObject("_targetValueLabel.Properties.AppearanceDisabled.BackColor2");
            this._targetValueLabel.Properties.AppearanceDisabled.BorderColor = (Color)componentResourceManager.GetObject("_targetValueLabel.Properties.AppearanceDisabled.BorderColor");
            this._targetValueLabel.Properties.AppearanceDisabled.ForeColor = (Color)componentResourceManager.GetObject("_targetValueLabel.Properties.AppearanceDisabled.ForeColor");
            this._targetValueLabel.Properties.AppearanceDisabled.Options.UseBackColor = true;
            this._targetValueLabel.Properties.AppearanceDisabled.Options.UseBorderColor = true;
            this._targetValueLabel.Properties.AppearanceDisabled.Options.UseForeColor = true;
            this._targetValueLabel.Properties.BorderStyle = BorderStyles.NoBorder;
            this._targetValueLabel.EditValueChanged += new EventHandler(this.On_LinkValueChanged);
            componentResourceManager.ApplyResources(this._marqueePanel, "_marqueePanel");
            this._marqueePanel.BorderStyle = BorderStyles.NoBorder;
            this._marqueePanel.Controls.Add(this._marqueeWheel);
            this._marqueePanel.Name = "_marqueePanel";
            componentResourceManager.ApplyResources(this._cancelCloseButton, "_cancelCloseButton");
            this._cancelCloseButton.DialogResult = DialogResult.Cancel;
            this._cancelCloseButton.Name = "_cancelCloseButton";
            this._cancelCloseButton.Click += new EventHandler(this._cancelCloseButton_Click);
            componentResourceManager.ApplyResources(this._pauseResumeButton, "_pauseResumeButton");
            this._pauseResumeButton.Name = "_pauseResumeButton";
            this._pauseResumeButton.Click += new EventHandler(this._pauseResumeButton_Click);
            componentResourceManager.ApplyResources(this._detailsButton, "_detailsButton");
            this._detailsButton.Name = "_detailsButton";
            this._detailsButton.Click += new EventHandler(this._detailsButton_Click);
            this.panelControl1.Appearance.BackColor = (Color)componentResourceManager.GetObject("panelControl1.Appearance.BackColor");
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = BorderStyles.NoBorder;
            componentResourceManager.ApplyResources(this.panelControl1, "panelControl1");
            this.panelControl1.Name = "panelControl1";
            componentResourceManager.ApplyResources(this._marqueeWheel, "_marqueeWheel");
            this._marqueeWheel.Appearance.BackColor = (Color)componentResourceManager.GetObject("_marqueeWheel.Appearance.BackColor");
            this._marqueeWheel.Appearance.Options.UseBackColor = true;
            this._marqueeWheel.AppearanceCaption.Font = (Font)componentResourceManager.GetObject("resource.Font");
            this._marqueeWheel.AppearanceCaption.Options.UseFont = true;
            this._marqueeWheel.AppearanceDescription.Options.UseFont = true;
            this._marqueeWheel.AutoHeight = true;
            this._marqueeWheel.AutoWidth = true;
            this._marqueeWheel.Name = "_marqueeWheel";
            this._marqueeWheel.OverlayImage = null;
            this._marqueeWheel.SizeChanged += new EventHandler(this._marqueeWheel_SizeChanged);
            this._progressDetailsControl.Appearance.BackColor = (Color)componentResourceManager.GetObject("_progressDetailsControl.Appearance.BackColor");
            this._progressDetailsControl.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this._progressDetailsControl, "_progressDetailsControl");
            this._progressDetailsControl.MaximumColumns = 2;
            this._progressDetailsControl.Name = "_progressDetailsControl";
            this._progressDetailsControl.Rows = 3;
            this._progressDetailsControl.Title = "PROGRESS";
            componentResourceManager.ApplyResources(this._logItemDetailsControl, "_logItemDetailsControl");
            this._logItemDetailsControl.Appearance.BackColor = (Color)componentResourceManager.GetObject("_logItemDetailsControl.Appearance.BackColor");
            this._logItemDetailsControl.Appearance.Options.UseBackColor = true;
            this._logItemDetailsControl.AutoScrollToBottom = true;
            this._logItemDetailsControl.DataSource = null;
            this._logItemDetailsControl.Name = "_logItemDetailsControl";
            this._intervalClock.AutoEllipsis = true;
            componentResourceManager.ApplyResources(this._intervalClock, "_intervalClock");
            this._intervalClock.Name = "_intervalClock";
            this._statusDetailControl.Appearance.BackColor = (Color)componentResourceManager.GetObject("_statusDetailControl.Appearance.BackColor");
            this._statusDetailControl.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this._statusDetailControl, "_statusDetailControl");
            this._statusDetailControl.MaximumColumns = 1;
            this._statusDetailControl.Name = "_statusDetailControl";
            this._statusDetailControl.Rows = 3;
            this._statusDetailControl.Title = "STATUS";
            this._statusDetailControl.Resize += new EventHandler(this._statusDetailControl_Resize);
            base.Appearance.BackColor = (Color)componentResourceManager.GetObject("JobStatusDialog.Appearance.BackColor");
            base.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this._cancelCloseButton;
            base.Controls.Add(this.panelControl1);
            base.Controls.Add(this._intervalClock);
            base.Controls.Add(this._logItemDetailsControl);
            base.Controls.Add(this._progressDetailsControl);
            base.Controls.Add(this._statusDetailControl);
            base.Controls.Add(this._detailsButton);
            base.Controls.Add(this._pauseResumeButton);
            base.Controls.Add(this._cancelCloseButton);
            base.Controls.Add(this._marqueePanel);
            base.Controls.Add(this._targetValueLabel);
            base.Controls.Add(this._targetLabel);
            base.Controls.Add(this._sourceValueLabel);
            base.Controls.Add(this._sourceLabel);
            base.Controls.Add(this._elapsedTimeLabel);
            base.Controls.Add(this._startTimeValueLabel);
            base.Controls.Add(this._startTimeLabel);
            base.Controls.Add(this._dataMigratedLabel);
            base.Controls.Add(this._dataMigratedValueLabel);
            base.Controls.Add(this._jobNameLabel);
            base.Controls.Add(this._jobLabel);
            base.LookAndFeel.SkinName = "Office 2013";
            base.MaximizeBox = false;
            base.Name = "JobStatusDialog";
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.FormClosing += new FormClosingEventHandler(this.JobStatusDialog_FormClosing);
            base.Move += new EventHandler(this.JobStatusDialog_Move);
            ((ISupportInitialize)this._sourceValueLabel.Properties).EndInit();
            ((ISupportInitialize)this._targetValueLabel.Properties).EndInit();
            ((ISupportInitialize)this._marqueePanel).EndInit();
            this._marqueePanel.ResumeLayout(false);
            ((ISupportInitialize)this.panelControl1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsActionBlocked()
        {
            return this._isActionBlocked;
        }

        private bool IsActionRunning()
        {
            if (this.Action.Status != ActionStatus.Running)
            {
                return false;
            }
            return !this.IsActionBlocked();
        }

        private void JobStatusDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Action.Status != ActionStatus.Running && this.Action.Status != ActionStatus.Paused)
            {
                this.DetachFromJobEvents();
                return;
            }
            bool flag = false;
            if (this.IsActionRunning())
            {
                this._intervalClock.StopClock();
                this.Action.Pause();
                flag = true;
            }
            if (FlatXtraMessageBox.Show(this,Metalogix.Actions.Properties.Resources.CancelActionConfirmation, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                if (flag)
                {
                    this.Action.Resume();
                    this._intervalClock.StartClock();
                }
                e.Cancel = true;
                return;
            }
            this._intervalClock.StopClock();
            this.DetachFromJobEvents();
            this.Action.Cancel();
            base.DialogResult = DialogResult.Cancel;
        }

        private void JobStatusDialog_Move(object sender, EventArgs e)
        {
            if (this._prevWindowState.HasValue && this._prevWindowState.Value == FormWindowState.Minimized && base.WindowState != FormWindowState.Minimized)
            {
                this.UpdateUIStatus();
                this.UpdateUIOperation();
            }
            this._prevWindowState = new FormWindowState?(base.WindowState);
        }

        private void On_action_Blocked(object sender, ActionBlockerEventArgs args)
        {
            if (!base.InvokeRequired)
            {
                switch (args.ChangeType)
                {
                    case ActionBlockerChangeType.Blocked:
                    {
                        this.SetActionBlocked(args.Message);
                        return;
                    }
                    case ActionBlockerChangeType.Unblocked:
                    {
                        this.SetActionUnblocked(args.Message);
                        return;
                    }
                    default:
                    {
                        return;
                    }
                }
            }
            ActionBlockerHandler actionBlockerHandler = new ActionBlockerHandler(this.On_action_Blocked);
            object[] objArray = new object[] { sender, args };
            base.Invoke(actionBlockerHandler, objArray);
        }

        private void On_action_OperationChanged(LogItem operation)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            this.UpdateUIOperation();
        }

        private void On_action_sourceLinkChanged(string link)
        {
            if (!base.InvokeRequired)
            {
                this._sourceValueLabel.Text = link;
                return;
            }
            JobStatusDialog.SetLabelDelegate setLabelDelegate = new JobStatusDialog.SetLabelDelegate(this.On_action_sourceLinkChanged);
            object[] objArray = new object[] { link };
            base.Invoke(setLabelDelegate, objArray);
        }

        private void On_action_Started(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
        {
            if (!base.InvokeRequired)
            {
                this._sourceValueLabel.Text = sSourceString;
                this._targetValueLabel.Text = sTargetString;
                this._intervalClock.StartClock();
                return;
            }
            ActionStartedEventHandler actionStartedEventHandler = new ActionStartedEventHandler(this.On_action_Started);
            object[] objArray = new object[] { sender, sSourceString, sTargetString };
            base.Invoke(actionStartedEventHandler, objArray);
        }

        private void On_action_StatusChanged(ActionStatus status)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            this.UpdateUIStatus();
        }

        private void On_action_targetLinkChanged(string link)
        {
            if (!base.InvokeRequired)
            {
                this._targetValueLabel.Text = link;
                return;
            }
            JobStatusDialog.SetLabelDelegate setLabelDelegate = new JobStatusDialog.SetLabelDelegate(this.On_action_targetLinkChanged);
            object[] objArray = new object[] { link };
            base.Invoke(setLabelDelegate, objArray);
        }

        private void On_LinkValueChanged(object sender, EventArgs e)
        {
            if (base.InvokeRequired)
            {
                EventHandler eventHandler = new EventHandler(this.On_LinkValueChanged);
                object[] objArray = new object[] { sender, e };
                base.Invoke(eventHandler, objArray);
                return;
            }
            HyperLinkEdit hyperLinkEdit = sender as HyperLinkEdit;
            if (hyperLinkEdit == null)
            {
                return;
            }
            if (!WebUtils.ValidLink(hyperLinkEdit.Text, false))
            {
                hyperLinkEdit.Enabled = false;
                return;
            }
            hyperLinkEdit.Enabled = true;
        }

        private void SetActionBlocked(string message)
        {
            this._isActionBlocked = true;
            this._marqueeWheel.Caption = "Migration Paused";
            this._marqueeWheel.Description = message;
            this._marqueeWheel.OverlayImage = JobStatusDialog.ActionStatusImageMap[ActionStatus.Paused];
        }

        private void SetActionUnblocked(string message)
        {
            this._isActionBlocked = false;
            this._marqueeWheel.Description = message;
            this.UpdateMarqueeMessage();
        }

        private void UpdateButtons()
        {
            string closeButtonText;
            this._pauseResumeButton.Enabled = (this.Job.Status == ActionStatus.Running ? true : this.Action.Status == ActionStatus.Paused);
            this._pauseResumeButton.Text = (this.Job.Status == ActionStatus.Paused ? "Continue" : "Pause");
            if (this.Job.Status == ActionStatus.Done || this.Job.Status == ActionStatus.Failed || this.Job.Status == ActionStatus.Warning || this.Job.Status == ActionStatus.Aborted)
            {
                this._intervalClock.StopClock();
                closeButtonText = Metalogix.Actions.Properties.Resources.CloseButtonText;
            }
            else
            {
                closeButtonText = Metalogix.Actions.Properties.Resources.CancelButtonText;
            }
            this._cancelCloseButton.Text = closeButtonText;
        }

        private void UpdateControlsForStatusSummaryResize(int adjustment)
        {
            if (base.InvokeRequired)
            {
                JobStatusDialog.UpdateControlsForSkipLabelVisibilityChangeDelegate updateControlsForSkipLabelVisibilityChangeDelegate = new JobStatusDialog.UpdateControlsForSkipLabelVisibilityChangeDelegate(this.UpdateControlsForStatusSummaryResize);
                object[] objArray = new object[] { adjustment };
                base.Invoke(updateControlsForSkipLabelVisibilityChangeDelegate, objArray);
                return;
            }
            if (adjustment == 0)
            {
                return;
            }
            bool detailsVisible = !this.DetailsVisible;
            int width = this.MaximumSize.Width;
            Size maximumSize = this.MaximumSize;
            Size size = new Size(width, maximumSize.Height + adjustment);
            int num = this.MinimumSize.Width;
            Size minimumSize = this.MinimumSize;
            Size size1 = new Size(num, minimumSize.Height + adjustment);
            int width1 = base.Size.Width;
            Size size2 = base.Size;
            Size size3 = new Size(width1, size2.Height + adjustment);
            if (adjustment <= 0)
            {
                base.SuspendLayout();
                this.MinimumSize = size1;
                base.ResumeLayout(true);
                base.Size = size3;
                if (detailsVisible)
                {
                    this.MaximumSize = size;
                }
            }
            else
            {
                if (detailsVisible)
                {
                    base.SuspendLayout();
                    this.MaximumSize = size;
                    base.ResumeLayout(true);
                }
                base.Size = size3;
                this.MinimumSize = size1;
            }
            base.SuspendLayout();
            try
            {
                LogDetailCountsControl rows = this._progressDetailsControl;
                rows.Rows = rows.Rows + (adjustment > 0 ? 1 : -1);
                PanelControl height = this._marqueePanel;
                height.Height = height.Height + adjustment;
                LogItemListControl point = this._logItemDetailsControl;
                int x = this._logItemDetailsControl.Location.X;
                Point location = this._logItemDetailsControl.Location;
                point.Location = new Point(x, location.Y + adjustment);
                LogItemListControl logItemListControl = this._logItemDetailsControl;
                logItemListControl.Height = logItemListControl.Height - adjustment;
                this._hiddenDetailsHeight += adjustment;
                this._showDetailsMinimumHeight += adjustment;
                this._showDetailsDefaultHeight += adjustment;
            }
            finally
            {
                base.ResumeLayout();
            }
        }

        protected void UpdateMarqueeMessage()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new JobStatusDialog.VoidDelegate(this.UpdateMarqueeMessage));
                return;
            }
            ActionStatus status = this.Action.Status;
            string statusMessage = this.StatusLabelProvider.GetStatusMessage(status);
            Image item = JobStatusDialog.ActionStatusImageMap[status];
            if (this._marqueeWheel.Caption != statusMessage)
            {
                this._marqueeWheel.Caption = statusMessage;
                this._marqueeWheel.OverlayImage = item;
                this.UpdateMinimumWidth();
            }
            if (!string.IsNullOrEmpty(this._marqueeWheel.Description))
            {
                this._marqueeWheel.Description = string.Empty;
            }
        }

        private void UpdateMarqueePanelWidth()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new JobStatusDialog.VoidDelegate(this.UpdateMarqueePanelWidth));
                return;
            }
            Point location = this._progressDetailsControl.Location;
            int x = location.X + this._progressDetailsControl.Width;
            if (!this._progressDetailsControl.Visible)
            {
                x = this._progressDetailsControl.Location.X;
            }
            int num = this._marqueePanel.Location.X - x;
            if (num == 0)
            {
                return;
            }
            base.SuspendLayout();
            try
            {
                PanelControl point = this._marqueePanel;
                Point location1 = this._marqueePanel.Location;
                point.Location = new Point(x, location1.Y);
                PanelControl width = this._marqueePanel;
                width.Width = width.Width + num;
                this.UpdateMinimumWidth();
            }
            finally
            {
                base.ResumeLayout();
            }
        }

        private void UpdateMinimumWidth()
        {
            int width = 196 + this._marqueeWheel.Width;
            if (this._progressDetailsControl.Visible)
            {
                width += this._progressDetailsControl.Width;
            }
            width = Math.Max(width, 500);
            if (width > base.Width)
            {
                base.Width = width;
            }
            this.MinimumSize = new Size(width, this.MinimumSize.Height);
        }

        private void UpdateProgressDetails()
        {
            Dictionary<string, long> completionDetails = this.Job.Log.CompletionDetails;
            object keyValuePairs = this.ProgressOrderProvider.OrderCompletionDetails(this.Job, completionDetails);
            if (keyValuePairs == null)
            {
                keyValuePairs = new List<KeyValuePair<string, long>>();
            }
            IEnumerable<KeyValuePair<string, long>> keyValuePairs1 = (IEnumerable<KeyValuePair<string, long>>)keyValuePairs;
            if (base.InvokeRequired)
            {
                JobStatusDialog.UpdateProgressDetailsDelegate updateProgressDetailsDelegate = new JobStatusDialog.UpdateProgressDetailsDelegate(this.UpdateProgressDetails);
                object[] array = new object[] { keyValuePairs1.ToArray<KeyValuePair<string, long>>() };
                base.Invoke(updateProgressDetailsDelegate, array);
            }
        }

        private void UpdateProgressDetails(KeyValuePair<string, long>[] progress)
        {
            if ((int)progress.Length == 0)
            {
                if (this._progressDetailsControl.Visible)
                {
                    this._progressDetailsControl.Visible = false;
                    this.UpdateMarqueePanelWidth();
                }
                return;
            }
            int width = this._progressDetailsControl.Width;
            bool visible = this._progressDetailsControl.Visible;
            if (!this._progressDetailsControl.Visible)
            {
                this._progressDetailsControl.Visible = true;
            }
            this._progressDetailsControl.UpdateDetails(progress);
            if (!visible || width != this._progressDetailsControl.Width)
            {
                this.UpdateMarqueePanelWidth();
            }
        }

        private void UpdateUIOperation()
        {
            IEnumerable<KeyValuePair<string, long>> statusSummary = this.SummaryProvider.GetStatusSummary(this.Job);
            object keyValuePairs = statusSummary;
            if (keyValuePairs == null)
            {
                keyValuePairs = new List<KeyValuePair<string, long>>();
            }
            statusSummary = (IEnumerable<KeyValuePair<string, long>>)keyValuePairs;
            Dictionary<string, long> completionDetails = this.Job.Log.CompletionDetails;
            object obj = this.ProgressOrderProvider.OrderCompletionDetails(this.Job, completionDetails);
            if (obj == null)
            {
                obj = new List<KeyValuePair<string, long>>();
            }
            KeyValuePair<string, long>[] array = ((IEnumerable<KeyValuePair<string, long>>)obj).ToArray<KeyValuePair<string, long>>();
            this.UpdateUIOperation(this.Job.Started, this.Job.GetFormattedLicensedData(false), statusSummary, array);
        }

        private void UpdateUIOperation(DateTime? startTime, string licensedDataUsed, IEnumerable<KeyValuePair<string, long>> status, KeyValuePair<string, long>[] progress)
        {
            if (base.InvokeRequired)
            {
                JobStatusDialog.UpdateUIOperationDelegate updateUIOperationDelegate = new JobStatusDialog.UpdateUIOperationDelegate(this.UpdateUIOperation);
                object[] objArray = new object[] { startTime, licensedDataUsed, status, progress };
                base.Invoke(updateUIOperationDelegate, objArray);
                return;
            }
            if (string.IsNullOrEmpty(this._startTimeValueLabel.Text) && startTime.HasValue)
            {
                this._startTimeValueLabel.Text = startTime.Value.ToString("G");
            }
            this._dataMigratedValueLabel.Text = licensedDataUsed;
            this._statusDetailControl.Rows = status.Count<KeyValuePair<string, long>>();
            this._progressDetailsControl.Rows = this._statusDetailControl.Rows;
            this._statusDetailControl.UpdateDetails(status);
            this.UpdateProgressDetails(progress);
        }

        private void UpdateUIStatus()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new JobStatusDialog.VoidDelegate(this.UpdateUIStatus));
                return;
            }
            this.Text = string.Concat(this.Action.Name, " (", this.Job.Status.ToString(), ")");
            this._jobNameLabel.Text = this.Action.Name;
            this.UpdateButtons();
            if (!this.IsActionBlocked())
            {
                this.UpdateMarqueeMessage();
            }
        }

        private delegate void SetLabelDelegate(string value);

        private delegate void UpdateControlsForSkipLabelVisibilityChangeDelegate(int adjustment);

        private delegate void UpdateProgressDetailsDelegate(KeyValuePair<string, long>[] progress);

        private delegate void UpdateUIOperationDelegate(DateTime? startTime, string licensedDataUsed, IEnumerable<KeyValuePair<string, long>> status, KeyValuePair<string, long>[] progress);

        private delegate void VoidDelegate();
    }
}