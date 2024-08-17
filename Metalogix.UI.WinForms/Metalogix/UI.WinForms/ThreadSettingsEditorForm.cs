using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.Threading;
using Metalogix.UI.WinForms.Documentation;

namespace Metalogix.UI.WinForms
{
    public class ThreadSettingsEditorForm : XtraForm
    {
        private IContainer components;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        private LabelControl w_ThreadCountLabel;

        private LabelControl w_executingThreads;

        private TextEdit w_tbThreadCount;

        private Timer w_threadTimer;

        private TrackBarControl w_slider;

        private LabelControl w_lblLow;

        private MemoEdit w_tbExplanation;

        private HyperLinkEdit w_linkLblResourceHelp;

        public ThreadSettingsEditorForm()
	{
		InitializeComponent();
		w_slider.Properties.Maximum = Environment.ProcessorCount * 4;
		w_slider.Properties.Minimum = 0;
		w_slider.Properties.LargeChange = Environment.ProcessorCount;
		w_slider.Properties.TickFrequency = (int)Math.Ceiling((float)Environment.ProcessorCount / 2f);
		w_slider.Value = ActionConfigurationVariables.ThreadsPerActionLimit;
		w_executingThreads.Visible = false;
		w_tbThreadCount.Visible = false;
		w_linkLblResourceHelp.Visible = true;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.ThreadSettingsEditorForm));
		this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
		this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
		this.w_ThreadCountLabel = new DevExpress.XtraEditors.LabelControl();
		this.w_executingThreads = new DevExpress.XtraEditors.LabelControl();
		this.w_tbThreadCount = new DevExpress.XtraEditors.TextEdit();
		this.w_threadTimer = new System.Windows.Forms.Timer(this.components);
		this.w_slider = new DevExpress.XtraEditors.TrackBarControl();
		this.w_lblLow = new DevExpress.XtraEditors.LabelControl();
		this.w_tbExplanation = new DevExpress.XtraEditors.MemoEdit();
		this.w_linkLblResourceHelp = new DevExpress.XtraEditors.HyperLinkEdit();
		((System.ComponentModel.ISupportInitialize)this.w_tbThreadCount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_slider).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_slider.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_tbExplanation.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_linkLblResourceHelp.Properties).BeginInit();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
		this.w_bOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.w_bOkay.Name = "w_bOkay";
		this.w_bOkay.Click += new System.EventHandler(On_Okay);
		componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
		this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_bCancel.Name = "w_bCancel";
		componentResourceManager.ApplyResources(this.w_ThreadCountLabel, "w_ThreadCountLabel");
		this.w_ThreadCountLabel.Name = "w_ThreadCountLabel";
		componentResourceManager.ApplyResources(this.w_executingThreads, "w_executingThreads");
		this.w_executingThreads.Name = "w_executingThreads";
		componentResourceManager.ApplyResources(this.w_tbThreadCount, "w_tbThreadCount");
		this.w_tbThreadCount.Name = "w_tbThreadCount";
		this.w_tbThreadCount.Properties.ReadOnly = true;
		this.w_threadTimer.Interval = 10000;
		this.w_threadTimer.Tick += new System.EventHandler(On_Timer_Ticks);
		componentResourceManager.ApplyResources(this.w_slider, "w_slider");
		this.w_slider.Name = "w_slider";
		this.w_slider.Value = 5;
		componentResourceManager.ApplyResources(this.w_lblLow, "w_lblLow");
		this.w_lblLow.Name = "w_lblLow";
		componentResourceManager.ApplyResources(this.w_tbExplanation, "w_tbExplanation");
		this.w_tbExplanation.Name = "w_tbExplanation";
		this.w_tbExplanation.Properties.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_tbExplanation.Properties.Appearance.Font");
		this.w_tbExplanation.Properties.Appearance.Options.UseFont = true;
		this.w_tbExplanation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.w_tbExplanation.Properties.ReadOnly = true;
		this.w_tbExplanation.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
		componentResourceManager.ApplyResources(this.w_linkLblResourceHelp, "w_linkLblResourceHelp");
		this.w_linkLblResourceHelp.Name = "w_linkLblResourceHelp";
		this.w_linkLblResourceHelp.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(On_Link_Clicked);
		base.AcceptButton = this.w_bOkay;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_bCancel;
		base.Controls.Add(this.w_linkLblResourceHelp);
		base.Controls.Add(this.w_tbExplanation);
		base.Controls.Add(this.w_lblLow);
		base.Controls.Add(this.w_slider);
		base.Controls.Add(this.w_tbThreadCount);
		base.Controls.Add(this.w_ThreadCountLabel);
		base.Controls.Add(this.w_bOkay);
		base.Controls.Add(this.w_bCancel);
		base.Controls.Add(this.w_executingThreads);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		base.Name = "ThreadSettingsEditorForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(On_ThreadDialog_Closing);
		base.Shown += new System.EventHandler(On_ThreadDialog_Shown);
		((System.ComponentModel.ISupportInitialize)this.w_tbThreadCount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_slider.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_slider).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_tbExplanation.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_linkLblResourceHelp.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void On_Link_Clicked(object sender, OpenLinkEventArgs e)
	{
		try
		{
			DocumentationHelper.ShowHelp(this, "resource_utilization_settings.html");
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			GlobalServices.ErrorHandler.HandleException("Help Error", $"Error opening Help file : {exception.Message}", exception, ErrorIcon.Error);
		}
	}

        private void On_Okay(object sender, EventArgs e)
	{
		ActionConfigurationVariables.ThreadsPerActionLimit = w_slider.Value;
	}

        private void On_ThreadDialog_Closing(object sender, FormClosingEventArgs e)
	{
	}

        private void On_ThreadDialog_Shown(object sender, EventArgs e)
	{
	}

        private void On_Timer_Ticks(object sender, EventArgs e)
	{
		SetThreadCount();
	}

        private void SetThreadCount()
	{
		w_tbThreadCount.Text = WorkerThread.Count.ToString();
	}
    }
}
