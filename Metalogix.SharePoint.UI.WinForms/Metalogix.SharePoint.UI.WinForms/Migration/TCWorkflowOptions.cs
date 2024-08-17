
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Documentation;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.WorkflowOptions.png")]
    [ControlName("Workflow Options")]
    public class TCWorkflowOptions : MigrationElementsScopabbleTabbableControl
    {
        private SPWorkflowOptions m_options;

        private bool m_bInitializing;

        private MigrationMode m_currentMigrationMode;

        private bool m_bOriginalWebWorkflowsValue;

        private bool m_bOriginalListWorkflowsValue;

        private bool m_bOriginalContentTypeWorkflowsValue;

        private bool m_bOriginalListSharePointDesignerNintexWorkflowsValue;

        private bool m_bOriginalWebSharePointDesignerNintexWorkflowsValue;

        private bool m_bOriginalContentTypeSharePointDesignerNintexWorkflowsValue;

        private bool m_bOriginalWorkflowInstanceDataValue;

        private bool m_bOriginalInProgressWorkflowsValue;

        private bool m_bOriginalCancelWorkflowsValue;

        private bool _originalPreviousVersionOfWorkflowInstancesValue;

        private bool m_bOriginalMigrateNintexDatabaseEntriesValue;

        private bool _isTargetAdapterSharePointOnline;

        private bool _isTargetAdapterOM;

        private bool _isSourceAdapterOM;

        private bool _originalGloballyReusableWorkflowTemplate;

        private bool m_bSuppressWorkflowDataChangedEvent;

        private IContainer components;

        private CheckEdit _cbWebOOB;

        private CheckEdit _cbWorkflowInstanceData;

        private CheckEdit _cbWebSPD;

        private GroupControl gbWebScope;

        private GroupControl gbAssociationOptions;

        private CheckEdit _cbListOOB;

        private GroupControl gbListScope;

        private CheckEdit _cbListSPD;

        private GroupControl gbContentTypeScope;

        private CheckEdit _cbContentTypeOOB;

        private CheckEdit _cbContentTypeSPD;

        private CheckEdit _cbNintexDatabaseInstanceEntries;

        private CheckEdit _rbWorkflowInstanceRunningStatus;

        private CheckEdit _rbWorkflowInstanceCancelStatus;

        private LabelControl labelControlHelp;

        private HelpTipButton helpTipButton;

        private CheckEdit _cbWorkflowPreviousVersionInstanceData;

        private HelpTipButton helpIncludePreviousVersionInstancesButton;

        private HelpTipButton helpWebSPD2013OMButton;

        private HelpTipButton helpListSPD2013OMButton;

        private GroupControl gbxGloballyReusable;

        private CheckEdit cbxGloballyReusableWorkflowTemplate;

        public SPWorkflowOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
                LoadUI();
            }
        }

        public new SPWeb SourceWeb
        {
            get
            {
                if (SourceNodes == null || SourceNodes.Count == 0)
                {
                    return null;
                }
                SPWeb result = null;
                if (SourceNodes[0] is SPWeb)
                {
                    result = SourceNodes[0] as SPWeb;
                }
                else if (SourceNodes[0] is SPList)
                {
                    result = (SourceNodes[0] as SPList).ParentWeb;
                }
                return result;
            }
        }

        public SPWeb TargetWeb
        {
            get
            {
                if (TargetNodes == null || TargetNodes.Count == 0)
                {
                    return null;
                }
                SPWeb result = null;
                if (TargetNodes[0] is SPWeb)
                {
                    result = TargetNodes[0] as SPWeb;
                }
                else if (TargetNodes[0] is SPList)
                {
                    result = (TargetNodes[0] as SPList).ParentWeb;
                }
                return result;
            }
        }

        public TCWorkflowOptions()
        {
            InitializeComponent();
            InitializeResources();
        }

        private void _cbWorkflowInstanceData_OnCheckedChanged(object sender, EventArgs e)
        {
            if (_cbWorkflowInstanceData.Checked && !m_bInitializing && !m_bSuppressWorkflowDataChangedEvent && FlatXtraMessageBox.Show(Metalogix.SharePoint.UI.WinForms.Properties.Resources.WorkflowInstanceDataWarning, Metalogix.SharePoint.UI.WinForms.Properties.Resources.WarningCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                _cbWorkflowInstanceData.Checked = false;
            }
            GenericWorkflowOnCheckedChanged(sender, e);
        }

        private bool CheckPrerequisitesFor13StyleWorkflow()
        {
            if (m_bInitializing || (!SourceWeb.Adapter.IsMEWS && !SourceWeb.Adapter.IsClientOM))
            {
                return false;
            }
            return SourceWeb.IsReadOnly;
        }

        private bool CheckPrerequisitesForNintexWorkflow()
        {
            if (m_bInitializing || m_bSuppressWorkflowDataChangedEvent || !_isTargetAdapterSharePointOnline)
            {
                return false;
            }
            return IsNintexFeatureActivateOnSource();
        }

        private void CheckStatusForSPDandNintexWorkflows(object sender, EventArgs e, bool isResuableNintexWorkflow)
        {
            CheckEdit checkEdit = sender as CheckEdit;
            bool flag = false;
            if (checkEdit != null && checkEdit.Checked && CheckPrerequisitesForNintexWorkflow())
            {
                DialogResult dialogResult = DialogResult.None;
                string text = string.Empty;
                if (!_isSourceAdapterOM)
                {
                    if (!_isSourceAdapterOM)
                    {
                        text = string.Format(Metalogix.SharePoint.UI.WinForms.Properties.Resources.NintexWorkflowWarning, Environment.NewLine);
                        text = GetUpdatedMessage(text);
                    }
                }
                else if (isResuableNintexWorkflow)
                {
                    text = string.Format(Metalogix.SharePoint.UI.WinForms.Properties.Resources.ResuableNintexWorkflowWarning, Environment.NewLine);
                    text = GetUpdatedMessage(text);
                }
                else if (UIConfigurationVariables.ShowNintexAppWarning)
                {
                    text = string.Format(SourceWeb.Adapter.SharePointVersion.IsSharePoint2016 ? Metalogix.SharePoint.UI.WinForms.Properties.Resources.Nintex2016WorkflowWarning : Metalogix.SharePoint.Actions.Properties.Resources.NintexWorkflowAppWarning, Environment.NewLine);
                    string updatedMessage = GetUpdatedMessage(text);
                    AdvancedXtraMessageBox advancedXtraMessageBox = new AdvancedXtraMessageBox(updatedMessage, "Warning", MessageBoxIcon.Exclamation, checkDoNotShowCheckbox: false);
                    if (!updatedMessage.Equals(text, StringComparison.OrdinalIgnoreCase))
                    {
                        advancedXtraMessageBox.Height = 260;
                        advancedXtraMessageBox.SetOkBtnVerticalLocation(180);
                    }
                    advancedXtraMessageBox.ShowDialog();
                    UIConfigurationVariables.ShowNintexAppWarning = !advancedXtraMessageBox.HideInFutureChecked;
                    flag = true;
                }
                else if (CheckPrerequisitesFor13StyleWorkflow())
                {
                    text = GetUpdatedMessage(text);
                }
                if (!string.IsNullOrEmpty(text) && !flag)
                {
                    dialogResult = FlatXtraMessageBox.Show(text, Metalogix.SharePoint.UI.WinForms.Properties.Resources.WarningCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    flag = true;
                }
                if (dialogResult == DialogResult.Cancel)
                {
                    checkEdit.Checked = false;
                }
            }
            if (checkEdit != null && checkEdit.Checked && !m_bInitializing && !m_bSuppressWorkflowDataChangedEvent && _isTargetAdapterOM)
            {
                if (FlatXtraMessageBox.Show(GetUpdatedMessage(Metalogix.SharePoint.UI.WinForms.Properties.Resources.SP2013WorkflowSuppressEventsOM), Metalogix.SharePoint.UI.WinForms.Properties.Resources.WarningCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    checkEdit.Checked = false;
                }
            }
            else if (checkEdit != null && checkEdit.Checked && !m_bInitializing && CheckPrerequisitesFor13StyleWorkflow() && !flag)
            {
                ShowSP2013WorkflowsAlertMessage(checkEdit);
            }
            GenericWorkflowOnCheckedChanged(sender, e);
        }

        private void DisableUI()
        {
            _cbWebOOB.Checked = false;
            _cbWebOOB.Enabled = false;
            _cbWebSPD.Checked = false;
            _cbWebSPD.Enabled = false;
            _cbListOOB.Checked = false;
            _cbListOOB.Enabled = false;
            _cbListSPD.Checked = false;
            _cbListSPD.Enabled = false;
            _cbContentTypeOOB.Checked = false;
            _cbContentTypeOOB.Enabled = false;
            _cbContentTypeSPD.Checked = false;
            _cbContentTypeSPD.Enabled = false;
            _cbWorkflowInstanceData.Checked = false;
            _cbWorkflowInstanceData.Enabled = false;
            _rbWorkflowInstanceRunningStatus.Enabled = false;
            _rbWorkflowInstanceRunningStatus.Checked = false;
            _rbWorkflowInstanceCancelStatus.Enabled = false;
            _rbWorkflowInstanceCancelStatus.Checked = false;
            _cbWorkflowPreviousVersionInstanceData.Enabled = false;
            _cbWorkflowPreviousVersionInstanceData.Checked = false;
            _cbNintexDatabaseInstanceEntries.Enabled = false;
            _cbNintexDatabaseInstanceEntries.Checked = false;
            _isTargetAdapterSharePointOnline = false;
            _isTargetAdapterOM = false;
            _isSourceAdapterOM = false;
            cbxGloballyReusableWorkflowTemplate.Checked = false;
            cbxGloballyReusableWorkflowTemplate.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GenericWorkflowOnCheckedChanged(object sender, EventArgs e)
        {
            if (!m_bSuppressWorkflowDataChangedEvent)
            {
                UpdateUI();
            }
        }

        private string GetUpdatedMessage(string message)
        {
            string text = message;
            if (CheckPrerequisitesFor13StyleWorkflow())
            {
                string text2;
                if (string.IsNullOrEmpty(text))
                {
                    text2 = string.Format(Metalogix.SharePoint.UI.WinForms.Properties.Resources.SP13StyleWorkflowWarning, Environment.NewLine, Environment.NewLine);
                }
                else
                {
                    object[] args = new object[4]
                    {
                    message,
                    Environment.NewLine,
                    Environment.NewLine,
                    string.Format(Metalogix.SharePoint.UI.WinForms.Properties.Resources.SP13StyleWorkflowWarning, Environment.NewLine, Environment.NewLine)
                    };
                    text2 = string.Format("{0}{1}{2}{3}", args);
                }
                text = text2;
            }
            return text;
        }

        public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
        {
            if (sMessage == "MigrationModeChanged")
            {
                MigrationModeChangedInfo migrationModeChangedInfo = (MigrationModeChangedInfo)oValue;
                MigrationModeChanged(migrationModeChangedInfo.NewMigrationMode, migrationModeChangedInfo.OverwritingOrUpdatingItems, migrationModeChangedInfo.PropagatingItemDeletions);
            }
        }

        private void helpTipButton_Click(object sender, EventArgs e)
        {
            try
            {
                DocumentationHelper.ShowHelp(this, "configuring_workflow_options.html");
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                GlobalServices.ErrorHandler.HandleException("Help Error", $"Error opening Help file : {ex2.Message}", ex2, ErrorIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCWorkflowOptions));
            this._cbWebOOB = new DevExpress.XtraEditors.CheckEdit();
            this._cbWorkflowInstanceData = new DevExpress.XtraEditors.CheckEdit();
            this._cbWebSPD = new DevExpress.XtraEditors.CheckEdit();
            this.gbWebScope = new DevExpress.XtraEditors.GroupControl();
            this.helpWebSPD2013OMButton = new TooltipsTest.HelpTipButton();
            this.gbAssociationOptions = new DevExpress.XtraEditors.GroupControl();
            this.helpIncludePreviousVersionInstancesButton = new TooltipsTest.HelpTipButton();
            this._cbWorkflowPreviousVersionInstanceData = new DevExpress.XtraEditors.CheckEdit();
            this._rbWorkflowInstanceCancelStatus = new DevExpress.XtraEditors.CheckEdit();
            this._rbWorkflowInstanceRunningStatus = new DevExpress.XtraEditors.CheckEdit();
            this._cbNintexDatabaseInstanceEntries = new DevExpress.XtraEditors.CheckEdit();
            this._cbListOOB = new DevExpress.XtraEditors.CheckEdit();
            this.gbListScope = new DevExpress.XtraEditors.GroupControl();
            this.helpListSPD2013OMButton = new TooltipsTest.HelpTipButton();
            this._cbListSPD = new DevExpress.XtraEditors.CheckEdit();
            this.gbContentTypeScope = new DevExpress.XtraEditors.GroupControl();
            this._cbContentTypeOOB = new DevExpress.XtraEditors.CheckEdit();
            this._cbContentTypeSPD = new DevExpress.XtraEditors.CheckEdit();
            this.labelControlHelp = new DevExpress.XtraEditors.LabelControl();
            this.helpTipButton = new TooltipsTest.HelpTipButton();
            this.gbxGloballyReusable = new DevExpress.XtraEditors.GroupControl();
            this.cbxGloballyReusableWorkflowTemplate = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)this._cbWebOOB.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWorkflowInstanceData.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWebSPD.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbWebScope).BeginInit();
            this.gbWebScope.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpWebSPD2013OMButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbAssociationOptions).BeginInit();
            this.gbAssociationOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpIncludePreviousVersionInstancesButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWorkflowPreviousVersionInstanceData.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._rbWorkflowInstanceCancelStatus.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._rbWorkflowInstanceRunningStatus.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbNintexDatabaseInstanceEntries.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbListOOB.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbListScope).BeginInit();
            this.gbListScope.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpListSPD2013OMButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbListSPD.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbContentTypeScope).BeginInit();
            this.gbContentTypeScope.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._cbContentTypeOOB.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbContentTypeSPD.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTipButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbxGloballyReusable).BeginInit();
            this.gbxGloballyReusable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.cbxGloballyReusableWorkflowTemplate.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this._cbWebOOB, "_cbWebOOB");
            this._cbWebOOB.Name = "_cbWebOOB";
            this._cbWebOOB.Properties.AutoWidth = true;
            this._cbWebOOB.Properties.Caption = resources.GetString("_cbWebOOB.Properties.Caption");
            this._cbWebOOB.CheckedChanged += new System.EventHandler(GenericWorkflowOnCheckedChanged);
            resources.ApplyResources(this._cbWorkflowInstanceData, "_cbWorkflowInstanceData");
            this._cbWorkflowInstanceData.Name = "_cbWorkflowInstanceData";
            this._cbWorkflowInstanceData.Properties.AutoWidth = true;
            this._cbWorkflowInstanceData.Properties.Caption = resources.GetString("_cbWorkflowInstanceData.Properties.Caption");
            this._cbWorkflowInstanceData.CheckedChanged += new System.EventHandler(_cbWorkflowInstanceData_OnCheckedChanged);
            resources.ApplyResources(this._cbWebSPD, "_cbWebSPD");
            this._cbWebSPD.Name = "_cbWebSPD";
            this._cbWebSPD.Properties.AutoWidth = true;
            this._cbWebSPD.Properties.Caption = resources.GetString("_cbWebSPD.Properties.Caption");
            this._cbWebSPD.CheckedChanged += new System.EventHandler(SPDWorkflowCheckBox_OnCheckedChangeEvent);
            resources.ApplyResources(this.gbWebScope, "gbWebScope");
            this.gbWebScope.Controls.Add(this.helpWebSPD2013OMButton);
            this.gbWebScope.Controls.Add(this._cbWebSPD);
            this.gbWebScope.Controls.Add(this._cbWebOOB);
            this.gbWebScope.Name = "gbWebScope";
            this.helpWebSPD2013OMButton.AnchoringControl = this.helpWebSPD2013OMButton;
            this.helpWebSPD2013OMButton.BackColor = System.Drawing.Color.Transparent;
            this.helpWebSPD2013OMButton.CommonParentControl = null;
            resources.ApplyResources(this.helpWebSPD2013OMButton, "helpWebSPD2013OMButton");
            this.helpWebSPD2013OMButton.Name = "helpWebSPD2013OMButton";
            this.helpWebSPD2013OMButton.TabStop = false;
            resources.ApplyResources(this.gbAssociationOptions, "gbAssociationOptions");
            this.gbAssociationOptions.Controls.Add(this.helpIncludePreviousVersionInstancesButton);
            this.gbAssociationOptions.Controls.Add(this._cbWorkflowPreviousVersionInstanceData);
            this.gbAssociationOptions.Controls.Add(this._rbWorkflowInstanceCancelStatus);
            this.gbAssociationOptions.Controls.Add(this._rbWorkflowInstanceRunningStatus);
            this.gbAssociationOptions.Controls.Add(this._cbNintexDatabaseInstanceEntries);
            this.gbAssociationOptions.Controls.Add(this._cbWorkflowInstanceData);
            this.gbAssociationOptions.Name = "gbAssociationOptions";
            this.helpIncludePreviousVersionInstancesButton.AnchoringControl = this.helpIncludePreviousVersionInstancesButton;
            this.helpIncludePreviousVersionInstancesButton.BackColor = System.Drawing.Color.Transparent;
            this.helpIncludePreviousVersionInstancesButton.CommonParentControl = null;
            resources.ApplyResources(this.helpIncludePreviousVersionInstancesButton, "helpIncludePreviousVersionInstancesButton");
            this.helpIncludePreviousVersionInstancesButton.Name = "helpIncludePreviousVersionInstancesButton";
            this.helpIncludePreviousVersionInstancesButton.TabStop = false;
            resources.ApplyResources(this._cbWorkflowPreviousVersionInstanceData, "_cbWorkflowPreviousVersionInstanceData");
            this._cbWorkflowPreviousVersionInstanceData.Name = "_cbWorkflowPreviousVersionInstanceData";
            this._cbWorkflowPreviousVersionInstanceData.Properties.AutoWidth = true;
            this._cbWorkflowPreviousVersionInstanceData.Properties.Caption = resources.GetString("_cbWorkflowPreviousVersionInstanceData.Properties.Caption");
            resources.ApplyResources(this._rbWorkflowInstanceCancelStatus, "_rbWorkflowInstanceCancelStatus");
            this._rbWorkflowInstanceCancelStatus.Name = "_rbWorkflowInstanceCancelStatus";
            this._rbWorkflowInstanceCancelStatus.Properties.AutoWidth = true;
            this._rbWorkflowInstanceCancelStatus.Properties.Caption = resources.GetString("_rbWorkflowInstanceCancelStatus.Properties.Caption");
            this._rbWorkflowInstanceCancelStatus.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this._rbWorkflowInstanceCancelStatus.Properties.RadioGroupIndex = 1;
            this._rbWorkflowInstanceCancelStatus.TabStop = false;
            resources.ApplyResources(this._rbWorkflowInstanceRunningStatus, "_rbWorkflowInstanceRunningStatus");
            this._rbWorkflowInstanceRunningStatus.Name = "_rbWorkflowInstanceRunningStatus";
            this._rbWorkflowInstanceRunningStatus.Properties.AutoWidth = true;
            this._rbWorkflowInstanceRunningStatus.Properties.Caption = resources.GetString("_rbWorkflowInstanceRunningStatus.Properties.Caption");
            this._rbWorkflowInstanceRunningStatus.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this._rbWorkflowInstanceRunningStatus.Properties.RadioGroupIndex = 1;
            this._rbWorkflowInstanceRunningStatus.TabStop = false;
            resources.ApplyResources(this._cbNintexDatabaseInstanceEntries, "_cbNintexDatabaseInstanceEntries");
            this._cbNintexDatabaseInstanceEntries.Name = "_cbNintexDatabaseInstanceEntries";
            this._cbNintexDatabaseInstanceEntries.Properties.AutoWidth = true;
            this._cbNintexDatabaseInstanceEntries.Properties.Caption = resources.GetString("_cbNintexDatabaseInstanceEntries.Properties.Caption");
            this._cbNintexDatabaseInstanceEntries.CheckedChanged += new System.EventHandler(GenericWorkflowOnCheckedChanged);
            resources.ApplyResources(this._cbListOOB, "_cbListOOB");
            this._cbListOOB.Name = "_cbListOOB";
            this._cbListOOB.Properties.AutoWidth = true;
            this._cbListOOB.Properties.Caption = resources.GetString("_cbListOOB.Properties.Caption");
            this._cbListOOB.CheckedChanged += new System.EventHandler(GenericWorkflowOnCheckedChanged);
            resources.ApplyResources(this.gbListScope, "gbListScope");
            this.gbListScope.Controls.Add(this.helpListSPD2013OMButton);
            this.gbListScope.Controls.Add(this._cbListSPD);
            this.gbListScope.Controls.Add(this._cbListOOB);
            this.gbListScope.Name = "gbListScope";
            this.helpListSPD2013OMButton.AnchoringControl = this.helpListSPD2013OMButton;
            this.helpListSPD2013OMButton.BackColor = System.Drawing.Color.Transparent;
            this.helpListSPD2013OMButton.CommonParentControl = null;
            resources.ApplyResources(this.helpListSPD2013OMButton, "helpListSPD2013OMButton");
            this.helpListSPD2013OMButton.Name = "helpListSPD2013OMButton";
            this.helpListSPD2013OMButton.TabStop = false;
            resources.ApplyResources(this._cbListSPD, "_cbListSPD");
            this._cbListSPD.Name = "_cbListSPD";
            this._cbListSPD.Properties.AutoWidth = true;
            this._cbListSPD.Properties.Caption = resources.GetString("_cbListSPD.Properties.Caption");
            this._cbListSPD.CheckedChanged += new System.EventHandler(SPDWorkflowCheckBox_OnCheckedChangeEvent);
            resources.ApplyResources(this.gbContentTypeScope, "gbContentTypeScope");
            this.gbContentTypeScope.Controls.Add(this._cbContentTypeOOB);
            this.gbContentTypeScope.Controls.Add(this._cbContentTypeSPD);
            this.gbContentTypeScope.Name = "gbContentTypeScope";
            resources.ApplyResources(this._cbContentTypeOOB, "_cbContentTypeOOB");
            this._cbContentTypeOOB.Name = "_cbContentTypeOOB";
            this._cbContentTypeOOB.Properties.AutoWidth = true;
            this._cbContentTypeOOB.Properties.Caption = resources.GetString("_cbContentTypeOOB.Properties.Caption");
            this._cbContentTypeOOB.CheckedChanged += new System.EventHandler(GenericWorkflowOnCheckedChanged);
            resources.ApplyResources(this._cbContentTypeSPD, "_cbContentTypeSPD");
            this._cbContentTypeSPD.Name = "_cbContentTypeSPD";
            this._cbContentTypeSPD.Properties.AutoWidth = true;
            this._cbContentTypeSPD.Properties.Caption = resources.GetString("_cbContentTypeSPD.Properties.Caption");
            this._cbContentTypeSPD.CheckedChanged += new System.EventHandler(SPDCTWorkflowCheckBox_OnCheckedChangeEvent);
            resources.ApplyResources(this.labelControlHelp, "labelControlHelp");
            this.labelControlHelp.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControlHelp.Name = "labelControlHelp";
            this.helpTipButton.AnchoringControl = null;
            this.helpTipButton.BackColor = System.Drawing.Color.Transparent;
            this.helpTipButton.CommonParentControl = null;
            resources.ApplyResources(this.helpTipButton, "helpTipButton");
            this.helpTipButton.Name = "helpTipButton";
            this.helpTipButton.RealOffset = null;
            this.helpTipButton.RelativeOffset = null;
            this.helpTipButton.TabStop = false;
            this.helpTipButton.Click += new System.EventHandler(helpTipButton_Click);
            resources.ApplyResources(this.gbxGloballyReusable, "gbxGloballyReusable");
            this.gbxGloballyReusable.Controls.Add(this.cbxGloballyReusableWorkflowTemplate);
            this.gbxGloballyReusable.Name = "gbxGloballyReusable";
            resources.ApplyResources(this.cbxGloballyReusableWorkflowTemplate, "cbxGloballyReusableWorkflowTemplate");
            this.cbxGloballyReusableWorkflowTemplate.Name = "cbxGloballyReusableWorkflowTemplate";
            this.cbxGloballyReusableWorkflowTemplate.Properties.AutoWidth = true;
            this.cbxGloballyReusableWorkflowTemplate.Properties.Caption = resources.GetString("cbxGloballyReusableWorkflowTemplate.Properties.Caption");
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.gbxGloballyReusable);
            base.Controls.Add(this.helpTipButton);
            base.Controls.Add(this.labelControlHelp);
            base.Controls.Add(this.gbContentTypeScope);
            base.Controls.Add(this.gbListScope);
            base.Controls.Add(this.gbAssociationOptions);
            base.Controls.Add(this.gbWebScope);
            this.MinimumSize = new System.Drawing.Size(420, 363);
            base.Name = "TCWorkflowOptions";
            ((System.ComponentModel.ISupportInitialize)this._cbWebOOB.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWorkflowInstanceData.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWebSPD.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbWebScope).EndInit();
            this.gbWebScope.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpWebSPD2013OMButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbAssociationOptions).EndInit();
            this.gbAssociationOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpIncludePreviousVersionInstancesButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbWorkflowPreviousVersionInstanceData.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._rbWorkflowInstanceCancelStatus.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._rbWorkflowInstanceRunningStatus.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbNintexDatabaseInstanceEntries.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbListOOB.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbListScope).EndInit();
            this.gbListScope.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpListSPD2013OMButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbListSPD.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbContentTypeScope).EndInit();
            this.gbContentTypeScope.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._cbContentTypeOOB.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbContentTypeSPD.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTipButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbxGloballyReusable).EndInit();
            this.gbxGloballyReusable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.cbxGloballyReusableWorkflowTemplate.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeResources()
        {
            Type type = GetType();
            helpTipButton.SetResourceString(type.FullName + helpTipButton.Name, type);
            helpIncludePreviousVersionInstancesButton.SetResourceString(type.FullName + helpIncludePreviousVersionInstancesButton.Name, type);
            helpWebSPD2013OMButton.SetResourceString(type.FullName + helpWebSPD2013OMButton.Name, type);
            helpListSPD2013OMButton.SetResourceString(type.FullName + helpListSPD2013OMButton.Name, type);
        }

        private bool IsNintexFeatureActivateOnSource()
        {
            if (SourceWeb.Adapter.SharePointVersion.IsSharePointOnline)
            {
                return true;
            }
            return SourceWeb.HasNintextFeature;
        }

        private bool IsSupported()
        {
            if (m_currentMigrationMode != 0)
            {
                return false;
            }
            SPNode sPNode = SourceNodes[0] as SPNode;
            SPNode sPNode2 = TargetNodes[0] as SPNode;
            if (!sPNode.Adapter.SharePointVersion.IsSharePoint2007OrLater || !sPNode2.Adapter.SharePointVersion.IsSharePoint2007OrLater)
            {
                return false;
            }
            if (sPNode.Adapter.SupportsWorkflows && sPNode2.Adapter.SupportsWorkflows)
            {
                return true;
            }
            return false;
        }

        private void LoadOptionConfiguration()
        {
            _cbWebOOB.Checked = m_bOriginalWebWorkflowsValue;
            _cbListOOB.Checked = m_bOriginalListWorkflowsValue;
            _cbContentTypeOOB.Checked = m_bOriginalContentTypeWorkflowsValue;
            _cbListSPD.Checked = m_bOriginalListSharePointDesignerNintexWorkflowsValue;
            _cbWebSPD.Checked = m_bOriginalWebSharePointDesignerNintexWorkflowsValue;
            _cbContentTypeSPD.Checked = m_bOriginalContentTypeSharePointDesignerNintexWorkflowsValue;
            _cbWorkflowInstanceData.Checked = m_bOriginalWorkflowInstanceDataValue;
            _rbWorkflowInstanceRunningStatus.Checked = m_bOriginalInProgressWorkflowsValue;
            _rbWorkflowInstanceCancelStatus.Checked = m_bOriginalCancelWorkflowsValue;
            _cbWorkflowPreviousVersionInstanceData.Checked = _originalPreviousVersionOfWorkflowInstancesValue;
            _cbNintexDatabaseInstanceEntries.Checked = m_bOriginalMigrateNintexDatabaseEntriesValue;
            cbxGloballyReusableWorkflowTemplate.Checked = _originalGloballyReusableWorkflowTemplate;
        }

        protected override void LoadUI()
        {
            m_bInitializing = true;
            UpdateUI();
            m_bInitializing = false;
        }

        public void MigrationModeChanged(MigrationMode newMigrationMode, bool bOverwritingOrUpdatingItems, bool bPropagatingItemDeletions)
        {
            m_bSuppressWorkflowDataChangedEvent = true;
            m_currentMigrationMode = newMigrationMode;
            if (newMigrationMode == MigrationMode.Full)
            {
                UpdateUI(bLoadConfigurationFromSaved: true);
            }
            else
            {
                UpdateUI();
            }
            m_bSuppressWorkflowDataChangedEvent = false;
        }

        private void SaveOptionConfiguration()
        {
            m_bOriginalWebWorkflowsValue = _cbWebOOB.Checked;
            m_bOriginalListWorkflowsValue = _cbListOOB.Checked;
            m_bOriginalContentTypeWorkflowsValue = _cbContentTypeOOB.Checked;
            m_bOriginalListSharePointDesignerNintexWorkflowsValue = _cbListSPD.Checked;
            m_bOriginalWebSharePointDesignerNintexWorkflowsValue = _cbWebSPD.Checked;
            m_bOriginalContentTypeSharePointDesignerNintexWorkflowsValue = _cbContentTypeSPD.Checked;
            m_bOriginalWorkflowInstanceDataValue = _cbWorkflowInstanceData.Checked;
            m_bOriginalInProgressWorkflowsValue = _rbWorkflowInstanceRunningStatus.Checked;
            m_bOriginalCancelWorkflowsValue = _rbWorkflowInstanceCancelStatus.Checked;
            _originalPreviousVersionOfWorkflowInstancesValue = _cbWorkflowPreviousVersionInstanceData.Checked;
            m_bOriginalMigrateNintexDatabaseEntriesValue = _cbNintexDatabaseInstanceEntries.Checked;
            _originalGloballyReusableWorkflowTemplate = cbxGloballyReusableWorkflowTemplate.Checked;
        }

        public override bool SaveUI()
        {
            Options.CopyListOOBWorkflowAssociations = _cbListOOB.Checked;
            Options.CopyWebOOBWorkflowAssociations = _cbWebOOB.Checked;
            Options.CopyContentTypeOOBWorkflowAssociations = _cbContentTypeOOB.Checked;
            Options.CopyListSharePointDesignerNintexWorkflowAssociations = _cbListSPD.Checked;
            Options.CopyWebSharePointDesignerNintexWorkflowAssociations = _cbWebSPD.Checked;
            Options.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = _cbContentTypeSPD.Checked;
            Options.CopyWorkflowInstanceData = _cbWorkflowInstanceData.Checked;
            Options.CopyInProgressWorkflows = _rbWorkflowInstanceRunningStatus.Checked;
            Options.CopyPreviousVersionOfWorkflowInstances = _cbWorkflowPreviousVersionInstanceData.Checked;
            Options.CopyNintexDatabaseEntries = _cbNintexDatabaseInstanceEntries.Checked;
            Options.CopyGloballyReusableWorkflowTemplates = cbxGloballyReusableWorkflowTemplate.Checked;
            return true;
        }

        private void ShowSP2013WorkflowsAlertMessage(CheckEdit cbSpd)
        {
            if (FlatXtraMessageBox.Show(GetUpdatedMessage(string.Empty), Metalogix.SharePoint.UI.WinForms.Properties.Resources.WarningCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
            {
                cbSpd.Checked = false;
            }
        }

        private void SPDCTWorkflowCheckBox_OnCheckedChangeEvent(object sender, EventArgs e)
        {
            CheckStatusForSPDandNintexWorkflows(sender, e, isResuableNintexWorkflow: true);
        }

        private void SPDWorkflowCheckBox_OnCheckedChangeEvent(object sender, EventArgs e)
        {
            CheckStatusForSPDandNintexWorkflows(sender, e, isResuableNintexWorkflow: false);
        }

        private void UpdateUI()
        {
            UpdateUI(bLoadConfigurationFromSaved: false);
        }

        private void UpdateUI(bool bLoadConfigurationFromSaved)
        {
            m_bSuppressWorkflowDataChangedEvent = true;
            if (!IsSupported())
            {
                DisableUI();
                m_bSuppressWorkflowDataChangedEvent = false;
                return;
            }
            _cbListOOB.Enabled = true;
            if (m_bInitializing)
            {
                _cbListOOB.Checked = Options.CopyListOOBWorkflowAssociations;
                _cbWebOOB.Checked = Options.CopyWebOOBWorkflowAssociations;
                _cbContentTypeOOB.Checked = Options.CopyContentTypeOOBWorkflowAssociations;
                _cbListSPD.Checked = Options.CopyListSharePointDesignerNintexWorkflowAssociations;
                _cbWebSPD.Checked = Options.CopyWebSharePointDesignerNintexWorkflowAssociations;
                _cbContentTypeSPD.Checked = Options.CopyContentTypeSharePointDesignerNintexWorkflowAssociations;
                _cbWorkflowInstanceData.Checked = Options.CopyWorkflowInstanceData;
                _rbWorkflowInstanceRunningStatus.Checked = Options.CopyInProgressWorkflows;
                _rbWorkflowInstanceCancelStatus.Checked = !Options.CopyInProgressWorkflows;
                _cbWorkflowPreviousVersionInstanceData.Checked = Options.CopyPreviousVersionOfWorkflowInstances;
                _cbNintexDatabaseInstanceEntries.Checked = Options.CopyNintexDatabaseEntries;
                cbxGloballyReusableWorkflowTemplate.Checked = Options.CopyGloballyReusableWorkflowTemplates;
            }
            if (bLoadConfigurationFromSaved)
            {
                LoadOptionConfiguration();
            }
            if (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.List)
            {
                SharePointAdapter adapter = (SourceNodes[0] as SPNode).Adapter;
                SharePointAdapter adapter2 = (TargetNodes[0] as SPNode).Adapter;
                bool isClientOM = adapter.IsClientOM;
                bool isClientOM2 = adapter2.IsClientOM;
                bool isSharePoint2010OrLater = adapter.SharePointVersion.IsSharePoint2010OrLater;
                bool isSharePoint2010OrLater2 = adapter2.SharePointVersion.IsSharePoint2010OrLater;
                bool isSharePoint2013OrLater = adapter2.SharePointVersion.IsSharePoint2013OrLater;
                bool isSharePoint2013OrLater2 = adapter.SharePointVersion.IsSharePoint2013OrLater;
                _isTargetAdapterSharePointOnline = adapter2.SharePointVersion.IsSharePointOnline;
                bool isSharePointOnline = adapter.SharePointVersion.IsSharePointOnline;
                _isSourceAdapterOM = !adapter.IsClientOM && !adapter.IsNws && !adapter.IsDB;
                _isTargetAdapterOM = !adapter2.IsClientOM && !adapter2.IsNws && !adapter2.IsDB && isSharePoint2013OrLater2 && isSharePoint2013OrLater;
                bool flag = base.Scope == SharePointObjectScope.List;
                bool flag2 = base.Scope == SharePointObjectScope.SiteCollection && !adapter2.SharePointVersion.IsSharePoint2007;
                _cbWebOOB.Enabled = !(!isSharePoint2010OrLater || !isSharePoint2010OrLater2 || isClientOM) && (!isClientOM2 || !adapter2.SharePointVersion.IsSharePoint2013OrLater) && !flag;
                _cbWebOOB.Checked = _cbWebOOB.Enabled && _cbWebOOB.Checked;
                bool enabled = (_cbWebOOB.Checked || _cbContentTypeOOB.Checked || _cbListOOB.Checked || _cbListSPD.Checked || _cbWebSPD.Checked || _cbContentTypeSPD.Checked) && SharePointConfigurationVariables.AllowDBWriting && !isClientOM2 && !isClientOM;
                gbAssociationOptions.Enabled = enabled;
                _cbWorkflowInstanceData.Enabled = enabled;
                _rbWorkflowInstanceRunningStatus.Enabled = _cbWorkflowInstanceData.Checked && _cbWorkflowInstanceData.Enabled;
                _rbWorkflowInstanceCancelStatus.Enabled = _cbWorkflowInstanceData.Checked;
                _cbWorkflowPreviousVersionInstanceData.Enabled = _cbWorkflowInstanceData.Checked;
                bool flag3 = !((!isSharePointOnline || !_isTargetAdapterSharePointOnline) && isClientOM) && !flag;
                bool flag4 = (isSharePointOnline || !isClientOM) && _isTargetAdapterSharePointOnline && flag;
                bool flag5 = isSharePoint2013OrLater2 && isSharePoint2013OrLater && !adapter.IsNws && !adapter.IsDB && !adapter2.IsNws && !adapter2.IsDB;
                _cbWebSPD.Enabled = isSharePoint2010OrLater && isSharePoint2010OrLater2 && (flag3 || flag5) && !flag;
                _cbWebSPD.Checked = _cbWebSPD.Enabled && _cbWebSPD.Checked;
                _cbContentTypeSPD.Enabled = flag3 || flag4;
                _cbContentTypeSPD.Checked = _cbContentTypeSPD.Enabled && _cbContentTypeSPD.Checked;
                _cbListSPD.Enabled = flag3 || flag4 || flag5;
                _cbListSPD.Checked = _cbListSPD.Enabled && _cbListSPD.Checked;
                _cbContentTypeOOB.Enabled = flag3 || flag4;
                _cbContentTypeOOB.Checked = _cbContentTypeOOB.Enabled && _cbContentTypeOOB.Checked;
                _cbListOOB.Enabled = flag3 || flag4;
                _cbListOOB.Checked = _cbListOOB.Enabled && _cbListOOB.Checked;
                if (isClientOM2 && !_isTargetAdapterSharePointOnline)
                {
                    string sPDOnlyCheckBoxCaption = Metalogix.SharePoint.UI.WinForms.Properties.Resources.SPDOnlyCheckBoxCaption;
                    _cbWebSPD.Properties.Caption = sPDOnlyCheckBoxCaption;
                    _cbListSPD.Properties.Caption = sPDOnlyCheckBoxCaption;
                    _cbContentTypeSPD.Properties.Caption = sPDOnlyCheckBoxCaption;
                }
                _cbNintexDatabaseInstanceEntries.Enabled = (_cbWebSPD.Checked || _cbListSPD.Checked || _cbContentTypeSPD.Checked) && SharePointConfigurationVariables.AllowDBWriting && !isClientOM2;
                if (!flag2)
                {
                    cbxGloballyReusableWorkflowTemplate.Enabled = flag2;
                    cbxGloballyReusableWorkflowTemplate.Checked = flag2;
                    HideControl(gbxGloballyReusable);
                    GroupControl groupControl = gbAssociationOptions;
                    groupControl.Location = new Point(gbAssociationOptions.Location.X, 285);
                }
            }
            else
            {
                DisableUI();
            }
            _cbWorkflowInstanceData.Checked = _cbWorkflowInstanceData.Checked && _cbWorkflowInstanceData.Enabled;
            _cbWorkflowPreviousVersionInstanceData.Checked = _cbWorkflowPreviousVersionInstanceData.Checked && _cbWorkflowPreviousVersionInstanceData.Enabled;
            _cbNintexDatabaseInstanceEntries.Checked = _cbNintexDatabaseInstanceEntries.Enabled && _cbNintexDatabaseInstanceEntries.Checked;
            SaveOptionConfiguration();
            m_bSuppressWorkflowDataChangedEvent = false;
        }
    }
}