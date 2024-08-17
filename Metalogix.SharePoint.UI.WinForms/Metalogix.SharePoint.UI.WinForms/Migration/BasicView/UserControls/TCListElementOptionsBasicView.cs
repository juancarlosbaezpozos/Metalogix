using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCListElementOptionsBasicView : MigrationElementsScopabbleTabbableControl
    {
        private CommonSerializableTable<object, object> _workingMap;

        private MigrationMode _currentMigrationMode;

        private bool _isTargetAdapterSharePointOnline;

        private bool _areAllPermissionsReset;

        private bool _isCopyListItemsChecked;

        private bool _isCopySubFoldersChecked;

        private bool _isIgnorePermissionsOptionDisabled;

        private SPMappingOptions _mappingOptions;

        private SPWebPartOptions _webPartOptions;

        private SPPermissionsOptions _permissionsOptions;

        private SPTaxonomyOptions _mmdOptions;

        private SPWorkflowOptions _workflowOptions;

        private IContainer components;

        private TableLayoutPanel tlpOuter;

        private PanelControl pnlMMD;

        private PanelControl pnlWebPart;

        private SimpleButton btnMapMMD;

        private ToggleSwitch tsMapMMD;

        private ToggleSwitch tsCopyWebParts;

        private PanelControl pnlPermissions;

        private ToggleSwitch tsPermissions;

        private PanelControl pnlWorkflow;

        private ToggleSwitch tsWorkflow;

        private PanelControl pnlPermissionsAdvanced;

        private LayoutControl lcAdvancedPermissions;

        private GroupControl gcAdvancedPermissions;

        private SimpleButton btnOpenUserMapping;

        private TextEdit txtMapMissingUsers;

        private ToggleSwitch tsOverwriteExistingGroups;

        private ToggleSwitch tsMapGroupsByName;

        private ToggleSwitch tsMapPermissions;

        private ToggleSwitch tsCopyContentPermissions;

        private ToggleSwitch tsCopyListPermissions;

        private ToggleSwitch tsMigrateBrokenSitePermissions;

        private ToggleSwitch tsCopySitePermissions;

        private LayoutControlGroup lcgAdvancedPermissions;

        private EmptySpaceItem esAdvancedPermissions;

        private LayoutControlGroup lcgOuterAdvancedPermissions;

        private LayoutControlItem lciAdvancedPermissions;

        private HelpTipButton helpMMD;

        private HelpTipButton helpWebParts;

        private HelpTipButton helpPermissions;

        private HelpTipButton helpMapMissingUsers;

        private HelpTipButton helpWorkflows;

        private HelpTipButton helpGlobalMapping;

        private ToggleSwitch tsMapMissingUsers;

        private ToggleSwitch tsIgnorePermissions;

        private ToggleSwitch tsCopyClosedWebParts;

        private HelpTipButton helpMapPermissions;

        private HelpTipButton helpCopyContentPermissions;

        private HelpTipButton helpMigrateBrokenSitePermissions;

        private HelpTipButton helMigrateBrokenSitePermissions;

        private HelpTipButton helpCopyClosedWebParts;

        private HelpTipButton helpOverwriteExistingGroups;

        private HelpTipButton helpMapGroupsByName;

        private HelpTipButton helpCopyListPermissions;

        private HelpTipButton helpCopySitePermissions;

        private HelpTipButton helpIgnorePermissions;

        public bool HasOnlyExternalLists { get; set; }

        public override bool IsBasicMode => true;

        public bool IsSetExplicitOptions { get; set; }

        public SPMappingOptions MappingOptions
        {
            get
            {
                return _mappingOptions;
            }
            set
            {
                _mappingOptions = value;
                LoadMappings();
            }
        }

        public SPTaxonomyOptions MMDOptions
        {
            get
            {
                return _mmdOptions;
            }
            set
            {
                _mmdOptions = value;
                LoadManagedMetadata();
            }
        }

        public SPPermissionsOptions PermissionsOptions
        {
            get
            {
                return _permissionsOptions;
            }
            set
            {
                _permissionsOptions = value;
                LoadPermissions();
            }
        }

        public SPWebPartOptions WebPartOptions
        {
            get
            {
                return _webPartOptions;
            }
            set
            {
                _webPartOptions = value;
                LoadWebParts();
            }
        }

        public SPWorkflowOptions WorkflowOptions
        {
            get
            {
                return _workflowOptions;
            }
            set
            {
                _workflowOptions = value;
                LoadWorkflows();
            }
        }

        public event TCMigrationOptionsBasicView.AdvancedOptionsMainControlStateChangedHandler AdvancedOptionsMainControlStateChanged;

        public TCListElementOptionsBasicView()
        {
            InitializeComponent();
            Type type = GetType();
            helpGlobalMapping.SetResourceString(type.FullName + btnOpenUserMapping.Name, type);
            helpMapMissingUsers.SetResourceString(type.FullName + tsMapMissingUsers.Name, type);
            helpCopyContentPermissions.SetResourceString(type.FullName + tsCopyContentPermissions.Name, type);
            helpMigrateBrokenSitePermissions.SetResourceString(type.FullName + tsMigrateBrokenSitePermissions.Name, type);
            helpMapPermissions.SetResourceString(type.FullName + tsMapPermissions.Name, type);
            helpWorkflows.SetResourceString(type.FullName + tsWorkflow.Name, type);
            helpMMD.SetResourceString(type.FullName + tsMapMMD.Name, type);
            helpWebParts.SetResourceString(type.FullName + tsCopyWebParts.Name, type);
            helpPermissions.SetResourceString(type.FullName + tsPermissions.Name, type);
            helpCopyClosedWebParts.SetResourceString(type.FullName + tsCopyClosedWebParts.Name, type);
            helpMapGroupsByName.SetResourceString(type.FullName + tsMapGroupsByName.Name, type);
            helpOverwriteExistingGroups.SetResourceString(type.FullName + tsOverwriteExistingGroups.Name, type);
            helpCopyListPermissions.SetResourceString(type.FullName + tsCopyListPermissions.Name, type);
            helpIgnorePermissions.SetResourceString(type.FullName + tsIgnorePermissions.Name, type);
            helpCopySitePermissions.SetResourceString(type.FullName + tsCopySitePermissions.Name, type);
            helpGlobalMapping.IsBasicViewHelpIcon = true;
            helpMapMissingUsers.IsBasicViewHelpIcon = true;
            helpCopyContentPermissions.IsBasicViewHelpIcon = true;
            helpMigrateBrokenSitePermissions.IsBasicViewHelpIcon = true;
            helpMapPermissions.IsBasicViewHelpIcon = true;
            helpWorkflows.IsBasicViewHelpIcon = true;
            helpMMD.IsBasicViewHelpIcon = true;
            helpWebParts.IsBasicViewHelpIcon = true;
            helpPermissions.IsBasicViewHelpIcon = true;
            helpCopyClosedWebParts.IsBasicViewHelpIcon = true;
            helpMapGroupsByName.IsBasicViewHelpIcon = true;
            helpOverwriteExistingGroups.IsBasicViewHelpIcon = true;
            helpCopyListPermissions.IsBasicViewHelpIcon = true;
            helpIgnorePermissions.IsBasicViewHelpIcon = true;
            helpCopySitePermissions.IsBasicViewHelpIcon = true;
        }

        private void btnMapMMD_Click(object sender, EventArgs e)
        {
            OpenMapTermsStoresDialog();
        }

        private void btnOpenUserMapping_Click(object sender, EventArgs e)
        {
            OpenUserMappingDialog();
        }

        private void DisableUI()
        {
            tsWorkflow.Enabled = false;
            tsWorkflow.IsOn = false;
            _isTargetAdapterSharePointOnline = false;
        }

        private void DisableWebpartOptions()
        {
            tsCopyWebParts.IsOn = false;
            tsCopyWebParts.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void EnableWorkflowSwitch(bool value)
        {
            tsWorkflow.Enabled = value;
            if (!tsWorkflow.Enabled)
            {
                tsWorkflow.IsOn = false;
            }
        }

        private void FireControlSizeChanged(int changedSize, bool isDecreaseSize)
        {
            if (this.AdvancedOptionsMainControlStateChanged != null)
            {
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(changedSize, isDecreaseSize), null);
            }
        }

        private ArrayList GetPermissionSummary()
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(new OptionsSummary($"{tsPermissions.Properties.OnText} : {tsPermissions.IsOn}", 1));
            ArrayList arrayList2 = arrayList;
            if (!tsPermissions.IsOn)
            {
                return arrayList2;
            }
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                arrayList2.Add(new OptionsSummary($"{tsCopySitePermissions.Properties.OnText} : {tsCopySitePermissions.IsOn}", 2));
                if (tsCopySitePermissions.IsOn)
                {
                    arrayList2.Add(new OptionsSummary($"{tsMigrateBrokenSitePermissions.Properties.OnText} : {tsMigrateBrokenSitePermissions.IsOn}", 3));
                }
            }
            arrayList2.Add(new OptionsSummary($"{tsCopyListPermissions.Properties.OnText} : {tsCopyListPermissions.IsOn}", 2));
            if (tsCopyListPermissions.IsOn && base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site)
            {
                arrayList2.Add(new OptionsSummary($"{tsIgnorePermissions.Properties.OnText} : {tsIgnorePermissions.IsOn}", 3));
            }
            if (base.Scope != SharePointObjectScope.Item && base.Scope != SharePointObjectScope.Folder)
            {
                arrayList2.Add(new OptionsSummary($"{tsCopyContentPermissions.Properties.OnText} : {tsCopyContentPermissions.IsOn}", 2));
            }
            arrayList2.Add(new OptionsSummary($"{tsMapPermissions.Properties.OnText} : {tsMapPermissions.IsOn}", 2));
            arrayList2.Add(new OptionsSummary($"{tsMapGroupsByName.Properties.OnText} : {tsMapGroupsByName.IsOn}", 2));
            if (tsMapGroupsByName.IsOn)
            {
                arrayList2.Add(new OptionsSummary($"{tsOverwriteExistingGroups.Properties.OnText} : {tsOverwriteExistingGroups.IsOn}", 3));
            }
            arrayList2.Add(tsMapMissingUsers.IsOn ? new OptionsSummary(string.Format("{0} : {1}", tsMapMissingUsers.Properties.OnText.Replace(":", ""), string.IsNullOrEmpty(txtMapMissingUsers.Text.Trim()) ? "  -" : txtMapMissingUsers.Text), 2) : new OptionsSummary($"Map Missing User : {tsMapMissingUsers.IsOn}", 2));
            return arrayList2;
        }

        private string GetSummaryHeader()
        {
            string result = string.Empty;
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    result = "Site Element Options";
                    break;
                case SharePointObjectScope.List:
                    result = "List Element Options";
                    break;
                case SharePointObjectScope.Folder:
                    result = "Folder Element Options";
                    break;
                case SharePointObjectScope.Item:
                    result = "Item Element Options";
                    break;
            }
            return result;
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            string summaryHeader = GetSummaryHeader();
            arrayList.Add(new OptionsSummary(summaryHeader, 0));
            if (tsMapMMD.Enabled)
            {
                arrayList.Add(new OptionsSummary($"{tsMapMMD.Properties.OnText} : {tsMapMMD.IsOn}", 1));
            }
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site || ShowHideWorkflowForListScope())
            {
                arrayList.Add(new OptionsSummary($"{tsWorkflow.Properties.OnText} : {tsWorkflow.IsOn}", 1));
            }
            arrayList.Add(new OptionsSummary($"{tsCopyWebParts.Properties.OnText} : {tsCopyWebParts.IsOn}", 1));
            if (tsCopyWebParts.IsOn)
            {
                arrayList.Add(new OptionsSummary($"{tsCopyClosedWebParts.Properties.OnText} : {tsCopyClosedWebParts.IsOn}", 2));
            }
            arrayList.AddRange(GetPermissionSummary());
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCListElementOptionsBasicView));
            this.tlpOuter = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMMD = new DevExpress.XtraEditors.PanelControl();
            this.helpMMD = new TooltipsTest.HelpTipButton();
            this.btnMapMMD = new DevExpress.XtraEditors.SimpleButton();
            this.tsMapMMD = new DevExpress.XtraEditors.ToggleSwitch();
            this.pnlWorkflow = new DevExpress.XtraEditors.PanelControl();
            this.helpWorkflows = new TooltipsTest.HelpTipButton();
            this.tsWorkflow = new DevExpress.XtraEditors.ToggleSwitch();
            this.pnlWebPart = new DevExpress.XtraEditors.PanelControl();
            this.helpCopyClosedWebParts = new TooltipsTest.HelpTipButton();
            this.tsCopyClosedWebParts = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpWebParts = new TooltipsTest.HelpTipButton();
            this.tsCopyWebParts = new DevExpress.XtraEditors.ToggleSwitch();
            this.pnlPermissions = new DevExpress.XtraEditors.PanelControl();
            this.helpPermissions = new TooltipsTest.HelpTipButton();
            this.tsPermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.pnlPermissionsAdvanced = new DevExpress.XtraEditors.PanelControl();
            this.lcAdvancedPermissions = new DevExpress.XtraLayout.LayoutControl();
            this.gcAdvancedPermissions = new DevExpress.XtraEditors.GroupControl();
            this.helpIgnorePermissions = new TooltipsTest.HelpTipButton();
            this.helpCopyListPermissions = new TooltipsTest.HelpTipButton();
            this.helpCopySitePermissions = new TooltipsTest.HelpTipButton();
            this.helpOverwriteExistingGroups = new TooltipsTest.HelpTipButton();
            this.helpMapGroupsByName = new TooltipsTest.HelpTipButton();
            this.helpMapPermissions = new TooltipsTest.HelpTipButton();
            this.helpCopyContentPermissions = new TooltipsTest.HelpTipButton();
            this.helpMigrateBrokenSitePermissions = new TooltipsTest.HelpTipButton();
            this.tsIgnorePermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsMapMissingUsers = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpGlobalMapping = new TooltipsTest.HelpTipButton();
            this.helpMapMissingUsers = new TooltipsTest.HelpTipButton();
            this.btnOpenUserMapping = new DevExpress.XtraEditors.SimpleButton();
            this.txtMapMissingUsers = new DevExpress.XtraEditors.TextEdit();
            this.tsOverwriteExistingGroups = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsMapGroupsByName = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsMapPermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyContentPermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyListPermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsMigrateBrokenSitePermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopySitePermissions = new DevExpress.XtraEditors.ToggleSwitch();
            this.lcgAdvancedPermissions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.esAdvancedPermissions = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcgOuterAdvancedPermissions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAdvancedPermissions = new DevExpress.XtraLayout.LayoutControlItem();
            this.helMigrateBrokenSitePermissions = new TooltipsTest.HelpTipButton();
            this.tlpOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlMMD).BeginInit();
            this.pnlMMD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpMMD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapMMD.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlWorkflow).BeginInit();
            this.pnlWorkflow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpWorkflows).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsWorkflow.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlWebPart).BeginInit();
            this.pnlWebPart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyClosedWebParts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyClosedWebParts.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpWebParts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyWebParts.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlPermissions).BeginInit();
            this.pnlPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlPermissionsAdvanced).BeginInit();
            this.pnlPermissionsAdvanced.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedPermissions).BeginInit();
            this.lcAdvancedPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gcAdvancedPermissions).BeginInit();
            this.gcAdvancedPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpIgnorePermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopySitePermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpOverwriteExistingGroups).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapGroupsByName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyContentPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMigrateBrokenSitePermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsIgnorePermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapMissingUsers.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpGlobalMapping).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapMissingUsers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.txtMapMissingUsers.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsOverwriteExistingGroups.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapGroupsByName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapPermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyContentPermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListPermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMigrateBrokenSitePermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySitePermissions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgOuterAdvancedPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedPermissions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helMigrateBrokenSitePermissions).BeginInit();
            base.SuspendLayout();
            this.tlpOuter.ColumnCount = 1;
            this.tlpOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpOuter.Controls.Add(this.pnlMMD, 0, 0);
            this.tlpOuter.Controls.Add(this.pnlWorkflow, 0, 1);
            this.tlpOuter.Controls.Add(this.pnlWebPart, 0, 2);
            this.tlpOuter.Controls.Add(this.pnlPermissions, 0, 3);
            this.tlpOuter.Controls.Add(this.pnlPermissionsAdvanced, 0, 4);
            this.tlpOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOuter.Location = new System.Drawing.Point(7, 11);
            this.tlpOuter.Name = "tlpOuter";
            this.tlpOuter.RowCount = 5;
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOuter.Size = new System.Drawing.Size(624, 539);
            this.tlpOuter.TabIndex = 0;
            this.pnlMMD.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlMMD.Controls.Add(this.helpMMD);
            this.pnlMMD.Controls.Add(this.btnMapMMD);
            this.pnlMMD.Controls.Add(this.tsMapMMD);
            this.pnlMMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMMD.Location = new System.Drawing.Point(3, 3);
            this.pnlMMD.Name = "pnlMMD";
            this.pnlMMD.Size = new System.Drawing.Size(618, 27);
            this.pnlMMD.TabIndex = 0;
            this.helpMMD.AnchoringControl = null;
            this.helpMMD.BackColor = System.Drawing.Color.Transparent;
            this.helpMMD.CommonParentControl = null;
            this.helpMMD.Image = (System.Drawing.Image)resources.GetObject("helpMMD.Image");
            this.helpMMD.Location = new System.Drawing.Point(244, 5);
            this.helpMMD.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMMD.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMMD.Name = "helpMMD";
            this.helpMMD.RealOffset = null;
            this.helpMMD.RelativeOffset = null;
            this.helpMMD.Size = new System.Drawing.Size(20, 20);
            this.helpMMD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMMD.TabIndex = 4;
            this.helpMMD.TabStop = false;
            this.btnMapMMD.Enabled = false;
            this.btnMapMMD.Location = new System.Drawing.Point(212, 4);
            this.btnMapMMD.Name = "btnMapMMD";
            this.btnMapMMD.Size = new System.Drawing.Size(23, 23);
            this.btnMapMMD.TabIndex = 1;
            this.btnMapMMD.Text = "...";
            this.btnMapMMD.Click += new System.EventHandler(btnMapMMD_Click);
            this.tsMapMMD.Location = new System.Drawing.Point(3, 4);
            this.tsMapMMD.Name = "tsMapMMD";
            this.tsMapMMD.Properties.Appearance.Options.UseTextOptions = true;
            this.tsMapMMD.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsMapMMD.Properties.OffText = "Map Managed Metadata";
            this.tsMapMMD.Properties.OnText = "Map Managed Metadata";
            this.tsMapMMD.Size = new System.Drawing.Size(207, 24);
            this.tsMapMMD.TabIndex = 0;
            this.tsMapMMD.Toggled += new System.EventHandler(tsMapMMD_Toggled);
            this.pnlWorkflow.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlWorkflow.Controls.Add(this.helpWorkflows);
            this.pnlWorkflow.Controls.Add(this.tsWorkflow);
            this.pnlWorkflow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWorkflow.Location = new System.Drawing.Point(3, 36);
            this.pnlWorkflow.Name = "pnlWorkflow";
            this.pnlWorkflow.Size = new System.Drawing.Size(618, 27);
            this.pnlWorkflow.TabIndex = 1;
            this.helpWorkflows.AnchoringControl = null;
            this.helpWorkflows.BackColor = System.Drawing.Color.Transparent;
            this.helpWorkflows.CommonParentControl = null;
            this.helpWorkflows.Image = (System.Drawing.Image)resources.GetObject("helpWorkflows.Image");
            this.helpWorkflows.Location = new System.Drawing.Point(173, 4);
            this.helpWorkflows.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpWorkflows.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpWorkflows.Name = "helpWorkflows";
            this.helpWorkflows.RealOffset = null;
            this.helpWorkflows.RelativeOffset = null;
            this.helpWorkflows.Size = new System.Drawing.Size(20, 20);
            this.helpWorkflows.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpWorkflows.TabIndex = 2;
            this.helpWorkflows.TabStop = false;
            this.tsWorkflow.EditValue = true;
            this.tsWorkflow.Location = new System.Drawing.Point(3, 3);
            this.tsWorkflow.Name = "tsWorkflow";
            this.tsWorkflow.Properties.Appearance.Options.UseTextOptions = true;
            this.tsWorkflow.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsWorkflow.Properties.OffText = "Copy Workflows";
            this.tsWorkflow.Properties.OnText = "Copy Workflows";
            this.tsWorkflow.Size = new System.Drawing.Size(171, 24);
            this.tsWorkflow.TabIndex = 0;
            this.pnlWebPart.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlWebPart.Controls.Add(this.helpCopyClosedWebParts);
            this.pnlWebPart.Controls.Add(this.tsCopyClosedWebParts);
            this.pnlWebPart.Controls.Add(this.helpWebParts);
            this.pnlWebPart.Controls.Add(this.tsCopyWebParts);
            this.pnlWebPart.Location = new System.Drawing.Point(3, 69);
            this.pnlWebPart.Name = "pnlWebPart";
            this.pnlWebPart.Size = new System.Drawing.Size(618, 60);
            this.pnlWebPart.TabIndex = 2;
            this.helpCopyClosedWebParts.AnchoringControl = null;
            this.helpCopyClosedWebParts.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyClosedWebParts.CommonParentControl = null;
            this.helpCopyClosedWebParts.Image = (System.Drawing.Image)resources.GetObject("helpCopyClosedWebParts.Image");
            this.helpCopyClosedWebParts.Location = new System.Drawing.Point(269, 36);
            this.helpCopyClosedWebParts.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyClosedWebParts.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyClosedWebParts.Name = "helpCopyClosedWebParts";
            this.helpCopyClosedWebParts.RealOffset = null;
            this.helpCopyClosedWebParts.RelativeOffset = null;
            this.helpCopyClosedWebParts.Size = new System.Drawing.Size(20, 20);
            this.helpCopyClosedWebParts.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyClosedWebParts.TabIndex = 4;
            this.helpCopyClosedWebParts.TabStop = false;
            this.tsCopyClosedWebParts.Location = new System.Drawing.Point(68, 33);
            this.tsCopyClosedWebParts.Margin = new System.Windows.Forms.Padding(3, 3, 3, 14);
            this.tsCopyClosedWebParts.Name = "tsCopyClosedWebParts";
            this.tsCopyClosedWebParts.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyClosedWebParts.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyClosedWebParts.Properties.OffText = "Copy Closed Web Parts";
            this.tsCopyClosedWebParts.Properties.OnText = "Copy Closed Web Parts";
            this.tsCopyClosedWebParts.Size = new System.Drawing.Size(202, 24);
            this.tsCopyClosedWebParts.TabIndex = 1;
            this.helpWebParts.AnchoringControl = null;
            this.helpWebParts.BackColor = System.Drawing.Color.Transparent;
            this.helpWebParts.CommonParentControl = null;
            this.helpWebParts.Image = (System.Drawing.Image)resources.GetObject("helpWebParts.Image");
            this.helpWebParts.Location = new System.Drawing.Point(173, 4);
            this.helpWebParts.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpWebParts.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpWebParts.Name = "helpWebParts";
            this.helpWebParts.RealOffset = null;
            this.helpWebParts.RelativeOffset = null;
            this.helpWebParts.Size = new System.Drawing.Size(20, 20);
            this.helpWebParts.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpWebParts.TabIndex = 2;
            this.helpWebParts.TabStop = false;
            this.tsCopyWebParts.EditValue = true;
            this.tsCopyWebParts.Location = new System.Drawing.Point(3, 2);
            this.tsCopyWebParts.Name = "tsCopyWebParts";
            this.tsCopyWebParts.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyWebParts.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyWebParts.Properties.OffText = "Copy Web Parts";
            this.tsCopyWebParts.Properties.OnText = "Copy Web Parts";
            this.tsCopyWebParts.Size = new System.Drawing.Size(171, 24);
            this.tsCopyWebParts.TabIndex = 0;
            this.tsCopyWebParts.Toggled += new System.EventHandler(tsCopyWebParts_Toggled);
            this.pnlPermissions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlPermissions.Controls.Add(this.helpPermissions);
            this.pnlPermissions.Controls.Add(this.tsPermissions);
            this.pnlPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPermissions.Location = new System.Drawing.Point(3, 135);
            this.pnlPermissions.Name = "pnlPermissions";
            this.pnlPermissions.Size = new System.Drawing.Size(618, 24);
            this.pnlPermissions.TabIndex = 3;
            this.pnlPermissions.TabStop = true;
            this.helpPermissions.AnchoringControl = null;
            this.helpPermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpPermissions.CommonParentControl = null;
            this.helpPermissions.Image = (System.Drawing.Image)resources.GetObject("helpPermissions.Image");
            this.helpPermissions.Location = new System.Drawing.Point(233, 1);
            this.helpPermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpPermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpPermissions.Name = "helpPermissions";
            this.helpPermissions.RealOffset = null;
            this.helpPermissions.RelativeOffset = null;
            this.helpPermissions.Size = new System.Drawing.Size(20, 20);
            this.helpPermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpPermissions.TabIndex = 2;
            this.helpPermissions.TabStop = false;
            this.tsPermissions.EditValue = true;
            this.tsPermissions.Location = new System.Drawing.Point(3, -1);
            this.tsPermissions.Name = "tsPermissions";
            this.tsPermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsPermissions.Properties.OffText = "Copy Permission and Security";
            this.tsPermissions.Properties.OnText = "Copy Permission and Security";
            this.tsPermissions.Size = new System.Drawing.Size(232, 24);
            this.tsPermissions.TabIndex = 0;
            this.tsPermissions.Toggled += new System.EventHandler(tsPermissions_Toggled);
            this.pnlPermissionsAdvanced.AutoSize = true;
            this.pnlPermissionsAdvanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlPermissionsAdvanced.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlPermissionsAdvanced.Controls.Add(this.lcAdvancedPermissions);
            this.pnlPermissionsAdvanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPermissionsAdvanced.Location = new System.Drawing.Point(3, 165);
            this.pnlPermissionsAdvanced.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.pnlPermissionsAdvanced.Name = "pnlPermissionsAdvanced";
            this.pnlPermissionsAdvanced.Size = new System.Drawing.Size(618, 374);
            this.pnlPermissionsAdvanced.TabIndex = 4;
            this.pnlPermissionsAdvanced.TabStop = true;
            this.lcAdvancedPermissions.Controls.Add(this.gcAdvancedPermissions);
            this.lcAdvancedPermissions.Location = new System.Drawing.Point(68, 7);
            this.lcAdvancedPermissions.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.lcAdvancedPermissions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lcAdvancedPermissions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lcAdvancedPermissions.Name = "lcAdvancedPermissions";
            this.lcAdvancedPermissions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(586, 224, 250, 350);
            this.lcAdvancedPermissions.OptionsFocus.EnableAutoTabOrder = false;
            this.lcAdvancedPermissions.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcAdvancedPermissions.Root = this.lcgAdvancedPermissions;
            this.lcAdvancedPermissions.Size = new System.Drawing.Size(555, 359);
            this.lcAdvancedPermissions.TabIndex = 0;
            this.lcAdvancedPermissions.Text = "layoutControl1";
            this.lcAdvancedPermissions.GroupExpandChanged += new DevExpress.XtraLayout.Utils.LayoutGroupEventHandler(lcAdvancedPermissions_GroupExpandChanged);
            this.gcAdvancedPermissions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gcAdvancedPermissions.Controls.Add(this.helpIgnorePermissions);
            this.gcAdvancedPermissions.Controls.Add(this.helpCopyListPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.helpCopySitePermissions);
            this.gcAdvancedPermissions.Controls.Add(this.helpOverwriteExistingGroups);
            this.gcAdvancedPermissions.Controls.Add(this.helpMapGroupsByName);
            this.gcAdvancedPermissions.Controls.Add(this.helpMapPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.helpCopyContentPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.helpMigrateBrokenSitePermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsIgnorePermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsMapMissingUsers);
            this.gcAdvancedPermissions.Controls.Add(this.helpGlobalMapping);
            this.gcAdvancedPermissions.Controls.Add(this.helpMapMissingUsers);
            this.gcAdvancedPermissions.Controls.Add(this.btnOpenUserMapping);
            this.gcAdvancedPermissions.Controls.Add(this.txtMapMissingUsers);
            this.gcAdvancedPermissions.Controls.Add(this.tsOverwriteExistingGroups);
            this.gcAdvancedPermissions.Controls.Add(this.tsMapGroupsByName);
            this.gcAdvancedPermissions.Controls.Add(this.tsMapPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsCopyContentPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsCopyListPermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsMigrateBrokenSitePermissions);
            this.gcAdvancedPermissions.Controls.Add(this.tsCopySitePermissions);
            this.gcAdvancedPermissions.Location = new System.Drawing.Point(14, 34);
            this.gcAdvancedPermissions.LookAndFeel.SkinName = "Metalogix 2017";
            this.gcAdvancedPermissions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gcAdvancedPermissions.Margin = new System.Windows.Forms.Padding(0);
            this.gcAdvancedPermissions.Name = "gcAdvancedPermissions";
            this.gcAdvancedPermissions.Size = new System.Drawing.Size(527, 300);
            this.gcAdvancedPermissions.TabIndex = 0;
            this.gcAdvancedPermissions.TabStop = true;
            this.gcAdvancedPermissions.Text = "groupControl1";
            this.helpIgnorePermissions.AnchoringControl = null;
            this.helpIgnorePermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpIgnorePermissions.CommonParentControl = null;
            this.helpIgnorePermissions.Image = (System.Drawing.Image)resources.GetObject("helpIgnorePermissions.Image");
            this.helpIgnorePermissions.Location = new System.Drawing.Point(352, 87);
            this.helpIgnorePermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpIgnorePermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpIgnorePermissions.Name = "helpIgnorePermissions";
            this.helpIgnorePermissions.RealOffset = null;
            this.helpIgnorePermissions.RelativeOffset = null;
            this.helpIgnorePermissions.Size = new System.Drawing.Size(20, 20);
            this.helpIgnorePermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpIgnorePermissions.TabIndex = 19;
            this.helpIgnorePermissions.TabStop = false;
            this.helpCopyListPermissions.AnchoringControl = null;
            this.helpCopyListPermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyListPermissions.CommonParentControl = null;
            this.helpCopyListPermissions.Image = (System.Drawing.Image)resources.GetObject("helpCopyListPermissions.Image");
            this.helpCopyListPermissions.Location = new System.Drawing.Point(216, 58);
            this.helpCopyListPermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListPermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListPermissions.Name = "helpCopyListPermissions";
            this.helpCopyListPermissions.RealOffset = null;
            this.helpCopyListPermissions.RelativeOffset = null;
            this.helpCopyListPermissions.Size = new System.Drawing.Size(20, 20);
            this.helpCopyListPermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyListPermissions.TabIndex = 18;
            this.helpCopyListPermissions.TabStop = false;
            this.helpCopySitePermissions.AnchoringControl = null;
            this.helpCopySitePermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpCopySitePermissions.CommonParentControl = null;
            this.helpCopySitePermissions.Image = (System.Drawing.Image)resources.GetObject("helpCopySitePermissions.Image");
            this.helpCopySitePermissions.Location = new System.Drawing.Point(217, 7);
            this.helpCopySitePermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopySitePermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopySitePermissions.Name = "helpCopySitePermissions";
            this.helpCopySitePermissions.RealOffset = null;
            this.helpCopySitePermissions.RelativeOffset = null;
            this.helpCopySitePermissions.Size = new System.Drawing.Size(20, 20);
            this.helpCopySitePermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopySitePermissions.TabIndex = 17;
            this.helpCopySitePermissions.TabStop = false;
            this.helpOverwriteExistingGroups.AnchoringControl = null;
            this.helpOverwriteExistingGroups.BackColor = System.Drawing.Color.Transparent;
            this.helpOverwriteExistingGroups.CommonParentControl = null;
            this.helpOverwriteExistingGroups.Image = (System.Drawing.Image)resources.GetObject("helpOverwriteExistingGroups.Image");
            this.helpOverwriteExistingGroups.Location = new System.Drawing.Point(352, 207);
            this.helpOverwriteExistingGroups.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpOverwriteExistingGroups.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpOverwriteExistingGroups.Name = "helpOverwriteExistingGroups";
            this.helpOverwriteExistingGroups.RealOffset = null;
            this.helpOverwriteExistingGroups.RelativeOffset = null;
            this.helpOverwriteExistingGroups.Size = new System.Drawing.Size(20, 20);
            this.helpOverwriteExistingGroups.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpOverwriteExistingGroups.TabIndex = 16;
            this.helpOverwriteExistingGroups.TabStop = false;
            this.helpMapGroupsByName.AnchoringControl = null;
            this.helpMapGroupsByName.BackColor = System.Drawing.Color.Transparent;
            this.helpMapGroupsByName.CommonParentControl = null;
            this.helpMapGroupsByName.Image = (System.Drawing.Image)resources.GetObject("helpMapGroupsByName.Image");
            this.helpMapGroupsByName.Location = new System.Drawing.Point(217, 177);
            this.helpMapGroupsByName.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMapGroupsByName.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMapGroupsByName.Name = "helpMapGroupsByName";
            this.helpMapGroupsByName.RealOffset = null;
            this.helpMapGroupsByName.RelativeOffset = null;
            this.helpMapGroupsByName.Size = new System.Drawing.Size(20, 20);
            this.helpMapGroupsByName.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMapGroupsByName.TabIndex = 15;
            this.helpMapGroupsByName.TabStop = false;
            this.helpMapPermissions.AnchoringControl = null;
            this.helpMapPermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpMapPermissions.CommonParentControl = null;
            this.helpMapPermissions.Image = (System.Drawing.Image)resources.GetObject("helpMapPermissions.Image");
            this.helpMapPermissions.Location = new System.Drawing.Point(272, 147);
            this.helpMapPermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMapPermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMapPermissions.Name = "helpMapPermissions";
            this.helpMapPermissions.RealOffset = null;
            this.helpMapPermissions.RelativeOffset = null;
            this.helpMapPermissions.Size = new System.Drawing.Size(20, 20);
            this.helpMapPermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMapPermissions.TabIndex = 14;
            this.helpMapPermissions.TabStop = false;
            this.helpCopyContentPermissions.AnchoringControl = null;
            this.helpCopyContentPermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyContentPermissions.CommonParentControl = null;
            this.helpCopyContentPermissions.Image = (System.Drawing.Image)resources.GetObject("helpCopyContentPermissions.Image");
            this.helpCopyContentPermissions.Location = new System.Drawing.Point(241, 118);
            this.helpCopyContentPermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyContentPermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyContentPermissions.Name = "helpCopyContentPermissions";
            this.helpCopyContentPermissions.RealOffset = null;
            this.helpCopyContentPermissions.RelativeOffset = null;
            this.helpCopyContentPermissions.Size = new System.Drawing.Size(20, 20);
            this.helpCopyContentPermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyContentPermissions.TabIndex = 13;
            this.helpCopyContentPermissions.TabStop = false;
            this.helpMigrateBrokenSitePermissions.AnchoringControl = null;
            this.helpMigrateBrokenSitePermissions.BackColor = System.Drawing.Color.Transparent;
            this.helpMigrateBrokenSitePermissions.CommonParentControl = null;
            this.helpMigrateBrokenSitePermissions.Image = (System.Drawing.Image)resources.GetObject("helpMigrateBrokenSitePermissions.Image");
            this.helpMigrateBrokenSitePermissions.Location = new System.Drawing.Point(352, 37);
            this.helpMigrateBrokenSitePermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMigrateBrokenSitePermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMigrateBrokenSitePermissions.Name = "helpMigrateBrokenSitePermissions";
            this.helpMigrateBrokenSitePermissions.RealOffset = null;
            this.helpMigrateBrokenSitePermissions.RelativeOffset = null;
            this.helpMigrateBrokenSitePermissions.Size = new System.Drawing.Size(20, 20);
            this.helpMigrateBrokenSitePermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMigrateBrokenSitePermissions.TabIndex = 12;
            this.helpMigrateBrokenSitePermissions.TabStop = false;
            this.tsIgnorePermissions.EditValue = true;
            this.tsIgnorePermissions.Location = new System.Drawing.Point(93, 84);
            this.tsIgnorePermissions.Name = "tsIgnorePermissions";
            this.tsIgnorePermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsIgnorePermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsIgnorePermissions.Properties.OffText = "Migrate List with Broken Inheritance";
            this.tsIgnorePermissions.Properties.OnText = "Migrate List with Broken Inheritance";
            this.tsIgnorePermissions.Size = new System.Drawing.Size(265, 24);
            this.tsIgnorePermissions.TabIndex = 3;
            this.tsMapMissingUsers.Location = new System.Drawing.Point(28, 234);
            this.tsMapMissingUsers.Name = "tsMapMissingUsers";
            this.tsMapMissingUsers.Properties.Appearance.Options.UseTextOptions = true;
            this.tsMapMissingUsers.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsMapMissingUsers.Properties.OffText = "Map Missing Users to:";
            this.tsMapMissingUsers.Properties.OnText = "Map Missing Users to:";
            this.tsMapMissingUsers.Size = new System.Drawing.Size(195, 24);
            this.tsMapMissingUsers.TabIndex = 8;
            this.tsMapMissingUsers.Toggled += new System.EventHandler(tsMapMissingUsers_Toggled);
            this.helpGlobalMapping.AnchoringControl = null;
            this.helpGlobalMapping.BackColor = System.Drawing.Color.Transparent;
            this.helpGlobalMapping.CommonParentControl = null;
            this.helpGlobalMapping.Image = (System.Drawing.Image)resources.GetObject("helpGlobalMapping.Image");
            this.helpGlobalMapping.Location = new System.Drawing.Point(200, 271);
            this.helpGlobalMapping.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpGlobalMapping.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpGlobalMapping.Name = "helpGlobalMapping";
            this.helpGlobalMapping.RealOffset = null;
            this.helpGlobalMapping.RelativeOffset = null;
            this.helpGlobalMapping.Size = new System.Drawing.Size(20, 20);
            this.helpGlobalMapping.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpGlobalMapping.TabIndex = 11;
            this.helpGlobalMapping.TabStop = false;
            this.helpMapMissingUsers.AnchoringControl = null;
            this.helpMapMissingUsers.BackColor = System.Drawing.Color.Transparent;
            this.helpMapMissingUsers.CommonParentControl = null;
            this.helpMapMissingUsers.Image = (System.Drawing.Image)resources.GetObject("helpMapMissingUsers.Image");
            this.helpMapMissingUsers.Location = new System.Drawing.Point(439, 238);
            this.helpMapMissingUsers.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMapMissingUsers.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMapMissingUsers.Name = "helpMapMissingUsers";
            this.helpMapMissingUsers.RealOffset = null;
            this.helpMapMissingUsers.RelativeOffset = null;
            this.helpMapMissingUsers.Size = new System.Drawing.Size(20, 20);
            this.helpMapMissingUsers.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMapMissingUsers.TabIndex = 10;
            this.helpMapMissingUsers.TabStop = false;
            this.btnOpenUserMapping.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btnOpenUserMapping.Appearance.Options.UseFont = true;
            this.btnOpenUserMapping.Location = new System.Drawing.Point(29, 270);
            this.btnOpenUserMapping.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.btnOpenUserMapping.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOpenUserMapping.Name = "btnOpenUserMapping";
            this.btnOpenUserMapping.Size = new System.Drawing.Size(163, 23);
            this.btnOpenUserMapping.TabIndex = 10;
            this.btnOpenUserMapping.Text = "Open Global Mappings";
            this.btnOpenUserMapping.Click += new System.EventHandler(btnOpenUserMapping_Click);
            this.txtMapMissingUsers.Enabled = false;
            this.txtMapMissingUsers.Location = new System.Drawing.Point(226, 238);
            this.txtMapMissingUsers.Name = "txtMapMissingUsers";
            this.txtMapMissingUsers.Size = new System.Drawing.Size(204, 20);
            this.txtMapMissingUsers.TabIndex = 9;
            this.tsOverwriteExistingGroups.EditValue = true;
            this.tsOverwriteExistingGroups.Location = new System.Drawing.Point(92, 204);
            this.tsOverwriteExistingGroups.Name = "tsOverwriteExistingGroups";
            this.tsOverwriteExistingGroups.Properties.Appearance.Options.UseTextOptions = true;
            this.tsOverwriteExistingGroups.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsOverwriteExistingGroups.Properties.OffText = "Overwrite Existing Groups at Target";
            this.tsOverwriteExistingGroups.Properties.OnText = "Overwrite Existing Groups at Target";
            this.tsOverwriteExistingGroups.Size = new System.Drawing.Size(265, 24);
            this.tsOverwriteExistingGroups.TabIndex = 7;
            this.tsMapGroupsByName.EditValue = true;
            this.tsMapGroupsByName.Location = new System.Drawing.Point(28, 174);
            this.tsMapGroupsByName.Name = "tsMapGroupsByName";
            this.tsMapGroupsByName.Properties.Appearance.Options.UseTextOptions = true;
            this.tsMapGroupsByName.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsMapGroupsByName.Properties.OffText = "Map Groups by Name";
            this.tsMapGroupsByName.Properties.OnText = "Map Groups by Name";
            this.tsMapGroupsByName.Size = new System.Drawing.Size(191, 24);
            this.tsMapGroupsByName.TabIndex = 6;
            this.tsMapGroupsByName.Toggled += new System.EventHandler(tsMapGroupsByName_Toggled);
            this.tsMapPermissions.EditValue = true;
            this.tsMapPermissions.Location = new System.Drawing.Point(28, 144);
            this.tsMapPermissions.Name = "tsMapPermissions";
            this.tsMapPermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsMapPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsMapPermissions.Properties.OffText = "Map Permissions Levels by Name";
            this.tsMapPermissions.Properties.OnText = "Map Permissions Levels by Name";
            this.tsMapPermissions.Size = new System.Drawing.Size(247, 24);
            this.tsMapPermissions.TabIndex = 5;
            this.tsMapPermissions.Toggled += new System.EventHandler(On_Toggle_Changed);
            this.tsCopyContentPermissions.EditValue = true;
            this.tsCopyContentPermissions.Location = new System.Drawing.Point(28, 114);
            this.tsCopyContentPermissions.Name = "tsCopyContentPermissions";
            this.tsCopyContentPermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyContentPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyContentPermissions.Properties.OffText = "Copy Content Permissions";
            this.tsCopyContentPermissions.Properties.OnText = "Copy Content Permissions";
            this.tsCopyContentPermissions.Size = new System.Drawing.Size(215, 24);
            this.tsCopyContentPermissions.TabIndex = 4;
            this.tsCopyContentPermissions.Toggled += new System.EventHandler(On_Toggle_Changed);
            this.tsCopyListPermissions.EditValue = true;
            this.tsCopyListPermissions.Location = new System.Drawing.Point(28, 56);
            this.tsCopyListPermissions.Name = "tsCopyListPermissions";
            this.tsCopyListPermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyListPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyListPermissions.Properties.OffText = "Copy List Permissions";
            this.tsCopyListPermissions.Properties.OnText = "Copy List Permissions";
            this.tsCopyListPermissions.Size = new System.Drawing.Size(188, 24);
            this.tsCopyListPermissions.TabIndex = 2;
            this.tsCopyListPermissions.Toggled += new System.EventHandler(tsCopyListPermissions_Toggled);
            this.tsMigrateBrokenSitePermissions.EditValue = true;
            this.tsMigrateBrokenSitePermissions.Location = new System.Drawing.Point(93, 33);
            this.tsMigrateBrokenSitePermissions.Name = "tsMigrateBrokenSitePermissions";
            this.tsMigrateBrokenSitePermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsMigrateBrokenSitePermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsMigrateBrokenSitePermissions.Properties.OffText = "Migrate Site with Broken Inheritance";
            this.tsMigrateBrokenSitePermissions.Properties.OnText = "Migrate Site with Broken Inheritance";
            this.tsMigrateBrokenSitePermissions.Size = new System.Drawing.Size(261, 24);
            this.tsMigrateBrokenSitePermissions.TabIndex = 1;
            this.tsCopySitePermissions.EditValue = true;
            this.tsCopySitePermissions.Location = new System.Drawing.Point(29, 5);
            this.tsCopySitePermissions.Name = "tsCopySitePermissions";
            this.tsCopySitePermissions.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopySitePermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopySitePermissions.Properties.OffText = "Copy Site Permissions";
            this.tsCopySitePermissions.Properties.OnText = "Copy Site Permissions";
            this.tsCopySitePermissions.Size = new System.Drawing.Size(190, 24);
            this.tsCopySitePermissions.TabIndex = 0;
            this.tsCopySitePermissions.Toggled += new System.EventHandler(tsCopySitePermissions_Toggled);
            this.lcgAdvancedPermissions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgAdvancedPermissions.GroupBordersVisible = false;
            DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this.lcgAdvancedPermissions.Items;
            DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[2] { this.esAdvancedPermissions, this.lcgOuterAdvancedPermissions };
            items.AddRange(items2);
            this.lcgAdvancedPermissions.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvancedPermissions.Name = "Root";
            this.lcgAdvancedPermissions.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgAdvancedPermissions.Size = new System.Drawing.Size(555, 359);
            this.lcgAdvancedPermissions.TextVisible = false;
            this.esAdvancedPermissions.AllowHotTrack = false;
            this.esAdvancedPermissions.Location = new System.Drawing.Point(0, 348);
            this.esAdvancedPermissions.Name = "esAdvancedPermissions";
            this.esAdvancedPermissions.Size = new System.Drawing.Size(555, 11);
            this.esAdvancedPermissions.TextSize = new System.Drawing.Size(0, 0);
            this.lcgOuterAdvancedPermissions.AppearanceGroup.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lcgOuterAdvancedPermissions.AppearanceGroup.Options.UseFont = true;
            this.lcgOuterAdvancedPermissions.ExpandButtonVisible = true;
            this.lcgOuterAdvancedPermissions.ExpandOnDoubleClick = true;
            this.lcgOuterAdvancedPermissions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciAdvancedPermissions });
            this.lcgOuterAdvancedPermissions.Location = new System.Drawing.Point(0, 0);
            this.lcgOuterAdvancedPermissions.Name = "lcgOuterAdvancedPermissions";
            this.lcgOuterAdvancedPermissions.Size = new System.Drawing.Size(555, 348);
            this.lcgOuterAdvancedPermissions.Text = "Advanced Permission Options";
            this.lciAdvancedPermissions.Control = this.gcAdvancedPermissions;
            this.lciAdvancedPermissions.Location = new System.Drawing.Point(0, 0);
            this.lciAdvancedPermissions.Name = "lciAdvancedPermissions";
            this.lciAdvancedPermissions.Size = new System.Drawing.Size(531, 304);
            this.lciAdvancedPermissions.TextSize = new System.Drawing.Size(0, 0);
            this.lciAdvancedPermissions.TextVisible = false;
            this.helMigrateBrokenSitePermissions.AnchoringControl = null;
            this.helMigrateBrokenSitePermissions.BackColor = System.Drawing.Color.Transparent;
            this.helMigrateBrokenSitePermissions.CommonParentControl = null;
            this.helMigrateBrokenSitePermissions.Image = (System.Drawing.Image)resources.GetObject("helMigrateBrokenSitePermissions.Image");
            this.helMigrateBrokenSitePermissions.Location = new System.Drawing.Point(362, 37);
            this.helMigrateBrokenSitePermissions.MaximumSize = new System.Drawing.Size(20, 20);
            this.helMigrateBrokenSitePermissions.MinimumSize = new System.Drawing.Size(20, 20);
            this.helMigrateBrokenSitePermissions.Name = "helMigrateBrokenSitePermissions";
            this.helMigrateBrokenSitePermissions.RealOffset = null;
            this.helMigrateBrokenSitePermissions.RelativeOffset = null;
            this.helMigrateBrokenSitePermissions.Size = new System.Drawing.Size(20, 20);
            this.helMigrateBrokenSitePermissions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helMigrateBrokenSitePermissions.TabIndex = 12;
            this.helMigrateBrokenSitePermissions.TabStop = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.tlpOuter);
            base.Name = "TCListElementOptionsBasicView";
            base.Padding = new System.Windows.Forms.Padding(7, 11, 0, 0);
            base.Size = new System.Drawing.Size(631, 550);
            this.UseTab = true;
            this.tlpOuter.ResumeLayout(false);
            this.tlpOuter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlMMD).EndInit();
            this.pnlMMD.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpMMD).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapMMD.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlWorkflow).EndInit();
            this.pnlWorkflow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpWorkflows).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsWorkflow.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlWebPart).EndInit();
            this.pnlWebPart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpCopyClosedWebParts).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyClosedWebParts.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpWebParts).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyWebParts.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlPermissions).EndInit();
            this.pnlPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlPermissionsAdvanced).EndInit();
            this.pnlPermissionsAdvanced.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedPermissions).EndInit();
            this.lcAdvancedPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gcAdvancedPermissions).EndInit();
            this.gcAdvancedPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpIgnorePermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopySitePermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpOverwriteExistingGroups).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapGroupsByName).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyContentPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMigrateBrokenSitePermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsIgnorePermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapMissingUsers.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpGlobalMapping).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMapMissingUsers).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.txtMapMissingUsers.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsOverwriteExistingGroups.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapGroupsByName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMapPermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyContentPermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListPermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsMigrateBrokenSitePermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySitePermissions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgOuterAdvancedPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedPermissions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helMigrateBrokenSitePermissions).EndInit();
            base.ResumeLayout(false);
        }

        private bool IsFormWebPartsOptionAvailable()
        {
            if (!(base.SourceSharePointVersion != null) || !(base.TargetSharePointVersion != null))
            {
                return false;
            }
            return base.SourceSharePointVersion.MajorVersion == base.TargetSharePointVersion.MajorVersion;
        }

        private bool IsSupported()
        {
            if (_currentMigrationMode != 0)
            {
                return false;
            }
            SPNode sPNode = SourceNodes[0] as SPNode;
            SPNode sPNode2 = TargetNodes[0] as SPNode;
            if ((sPNode != null && !sPNode.Adapter.SharePointVersion.IsSharePoint2007OrLater) || (sPNode2 != null && !sPNode2.Adapter.SharePointVersion.IsSharePoint2007OrLater))
            {
                return false;
            }
            if (sPNode == null || !sPNode.Adapter.SupportsWorkflows || sPNode2 == null)
            {
                return false;
            }
            return sPNode2.Adapter.SupportsWorkflows;
        }

        private void lcAdvancedPermissions_GroupExpandChanged(object sender, LayoutGroupEventArgs e)
        {
            int num = 0;
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    num = 285;
                    break;
                case SharePointObjectScope.List:
                    num = 264;
                    break;
                case SharePointObjectScope.Folder:
                case SharePointObjectScope.Item:
                    num = 230;
                    break;
            }
            int num2 = lcAdvancedPermissions.Height;
            lcAdvancedPermissions.Height = (e.Group.Expanded ? (num2 + num) : (num2 - num));
            FireControlSizeChanged(num, !e.Group.Expanded);
        }

        private void LoadManagedMetadata()
        {
            if (HasOnlyExternalLists)
            {
                tsMapMMD.Enabled = false;
                return;
            }
            tsMapMMD.Toggled -= tsMapMMD_Toggled;
            if (!IsTaxonomySupported())
            {
                tsMapMMD.Enabled = false;
                tsMapMMD.IsOn = false;
            }
            else
            {
                tsMapMMD.IsOn = _mmdOptions.ResolveManagedMetadataByName || _mmdOptions.MapTermStores || _mmdOptions.CopyReferencedManagedMetadata;
                SimpleButton simpleButton = btnMapMMD;
                bool enabled = tsMapMMD.IsOn && (IsSetExplicitOptions || _mmdOptions.MapTermStores);
                simpleButton.Enabled = enabled;
            }
            if (SourceAndTargetAreTenant() && TermstoresCanBeAutomaticallyMapped())
            {
                CommonSerializableTable<object, object> workingMap = new CommonSerializableTable<object, object> {
                {
                    base.SourceTermstores[0],
                    base.TargetTermstores[0]
                } };
                _workingMap = workingMap;
            }
            tsMapMMD.Toggled += tsMapMMD_Toggled;
        }

        private void LoadMappings()
        {
            tsMapMissingUsers.IsOn = _mappingOptions.MapMissingUsers;
            txtMapMissingUsers.Text = _mappingOptions.MapMissingUsersToLoginName;
            tsMapGroupsByName.IsOn = _mappingOptions.MapGroupsByName;
            tsOverwriteExistingGroups.IsOn = _mappingOptions.OverwriteGroups;
        }

        private void LoadPermissions()
        {
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    tsCopySitePermissions.IsOn = _permissionsOptions.CopySitePermissions;
                    tsMigrateBrokenSitePermissions.IsOn = _permissionsOptions.CopyRootPermissions;
                    tsCopyListPermissions.IsOn = _permissionsOptions.CopyListPermissions;
                    break;
                case SharePointObjectScope.List:
                    tsCopyListPermissions.IsOn = _permissionsOptions.CopyListPermissions;
                    break;
                case SharePointObjectScope.Folder:
                    tsCopyListPermissions.IsOn = _permissionsOptions.CopyFolderPermissions || _permissionsOptions.CopyItemPermissions;
                    break;
                case SharePointObjectScope.Item:
                    tsCopyListPermissions.IsOn = _permissionsOptions.CopyItemPermissions;
                    break;
            }
            tsCopyContentPermissions.IsOn = _permissionsOptions.CopyFolderPermissions || _permissionsOptions.CopyItemPermissions;
            tsIgnorePermissions.IsOn = _permissionsOptions.CopyRootPermissions;
            if (base.Scope == SharePointObjectScope.Folder && !IsSetExplicitOptions)
            {
                tsIgnorePermissions.Enabled = !_isIgnorePermissionsOptionDisabled;
            }
            tsMapPermissions.IsOn = _permissionsOptions.MapRolesByName;
        }

        private void LoadWebParts()
        {
            bool flag = base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection;
            tsCopyWebParts.IsOn = _webPartOptions.CopyDocumentWebParts || (flag && _webPartOptions.CopySiteWebParts) || _webPartOptions.CopyClosedWebParts || _webPartOptions.CopyContentZoneContent;
            tsCopyClosedWebParts.IsOn = _webPartOptions.CopyClosedWebParts;
            UpdateEnabledState();
            bool flag2 = !(base.ActionType == null) && base.ActionType == typeof(PasteMySitesAction);
            if (base.SourceSharePointVersion.IsSharePointOnline && base.TargetSharePointVersion.IsSharePointOnline && flag2)
            {
                DisableWebpartOptions();
            }
        }

        private void LoadWorkflows()
        {
            if (!HasOnlyExternalLists)
            {
                UpdateUI();
            }
            else
            {
                tsWorkflow.Enabled = false;
            }
        }

        private void MapToTermstoreMappingTable()
        {
            try
            {
                if (_mmdOptions == null || base.SourceTermstores == null || base.TargetTermstores == null)
                {
                    return;
                }
                CommonSerializableTable<object, object> commonSerializableTable = new CommonSerializableTable<object, object>();
                if (_workingMap != null)
                {
                    foreach (object key in _workingMap.Keys)
                    {
                        Guid id = ((SPTermStore)key).Id;
                        Guid id2 = ((SPTermStore)_workingMap[key]).Id;
                        SPTermStore sPTermStore = base.SourceTermstores[id];
                        SPTermStore sPTermStore2 = base.TargetTermstores[id2];
                        if (sPTermStore != null && sPTermStore2 != null)
                        {
                            commonSerializableTable.Add(sPTermStore, sPTermStore2);
                        }
                    }
                }
                else
                {
                    foreach (string key2 in _mmdOptions.TermstoreNameMappingTable.Keys)
                    {
                        string g = _mmdOptions.TermstoreNameMappingTable[key2];
                        SPTermStore sPTermStore3 = base.SourceTermstores[new Guid(key2)];
                        SPTermStore sPTermStore4 = base.TargetTermstores[new Guid(g)];
                        if (sPTermStore3 != null && sPTermStore4 != null)
                        {
                            commonSerializableTable.Add(sPTermStore3, sPTermStore4);
                        }
                    }
                }
                _workingMap = commonSerializableTable;
            }
            catch
            {
                _workingMap = new CommonSerializableTable<object, object>();
            }
        }

        private void On_Toggle_Changed(object sender, EventArgs e)
        {
            bool flag = false;
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                flag = tsCopySitePermissions.IsOn;
            }
            bool flag2 = false;
            if (base.Scope != SharePointObjectScope.Folder && base.Scope != SharePointObjectScope.Item)
            {
                flag2 = tsCopyContentPermissions.IsOn;
            }
            if (!(flag || tsCopyListPermissions.IsOn || flag2) && !tsMapGroupsByName.IsOn && !tsMapMissingUsers.IsOn && !tsMapPermissions.IsOn)
            {
                tsPermissions.IsOn = false;
            }
        }

        private void OpenMapTermsStoresDialog()
        {
            using (SerializableTermstoreMapper serializableTermstoreMapper = new SerializableTermstoreMapper(isBasicMode: true))
            {
                MapToTermstoreMappingTable();
                serializableTermstoreMapper.Mappings = (SerializableTable<object, object>)_workingMap.Clone();
                serializableTermstoreMapper.SourceTermstoreCollection = base.SourceTermstores;
                serializableTermstoreMapper.TargetTermstoreCollection = base.TargetTermstores;
                serializableTermstoreMapper.ShowDialog();
                if (serializableTermstoreMapper.DialogResult == DialogResult.OK)
                {
                    _workingMap = (CommonSerializableTable<object, object>)serializableTermstoreMapper.Mappings;
                }
            }
        }

        private void SaveManagedMetadata()
        {
            bool flag = !_mmdOptions.MapTermStores && !_mmdOptions.ResolveManagedMetadataByName && !_mmdOptions.CopyReferencedManagedMetadata;
            if (IsSetExplicitOptions || flag)
            {
                _mmdOptions.MapTermStores = tsMapMMD.IsOn;
                _mmdOptions.ResolveManagedMetadataByName = tsMapMMD.IsOn;
                _mmdOptions.CopyReferencedManagedMetadata = tsMapMMD.IsOn;
            }
            else if (!tsMapMMD.IsOn)
            {
                _mmdOptions.MapTermStores = false;
                _mmdOptions.ResolveManagedMetadataByName = false;
                _mmdOptions.CopyReferencedManagedMetadata = false;
            }
            if (_workingMap == null)
            {
                MapToTermstoreMappingTable();
            }
            else if (!_mmdOptions.MapTermStores)
            {
                _workingMap.Clear();
            }
            _mmdOptions.TermstoreNameMappingTable.Clear();
            if (_workingMap == null || _workingMap.Count <= 0)
            {
                return;
            }
            foreach (SPTermStore key in _workingMap.Keys)
            {
                SPTermStore sPTermStore2 = (SPTermStore)_workingMap[key];
                if (!_mmdOptions.TermstoreNameMappingTable.ContainsKey(key.Id.ToString()))
                {
                    _mmdOptions.TermstoreNameMappingTable.Add(key.Id.ToString(), sPTermStore2.Id.ToString());
                }
                else
                {
                    _mmdOptions.TermstoreNameMappingTable[key.Id.ToString()] = sPTermStore2.Id.ToString();
                }
            }
        }

        private void SaveMappings()
        {
            if (!tsPermissions.IsOn)
            {
                _mappingOptions.MapGroupsByName = false;
                _mappingOptions.OverwriteGroups = false;
                _mappingOptions.MapMissingUsers = false;
                _mappingOptions.MapMissingUsersToLoginName = string.Empty;
            }
            else
            {
                _mappingOptions.MapGroupsByName = tsMapGroupsByName.IsOn && tsMapGroupsByName.Enabled;
                _mappingOptions.OverwriteGroups = tsOverwriteExistingGroups.IsOn && tsOverwriteExistingGroups.Enabled;
                _mappingOptions.MapMissingUsers = tsMapMissingUsers.Enabled && tsMapMissingUsers.IsOn;
                SPMappingOptions mappingOptions = _mappingOptions;
                string mapMissingUsersToLoginName = ((!_mappingOptions.MapMissingUsers) ? null : txtMapMissingUsers.Text);
                mappingOptions.MapMissingUsersToLoginName = mapMissingUsersToLoginName;
            }
        }

        private void SavePermissions()
        {
            if (!tsPermissions.IsOn)
            {
                _permissionsOptions.CopySitePermissions = false;
                _permissionsOptions.CopyPermissionLevels = false;
                _permissionsOptions.CopyAccessRequestSettings = false;
                _permissionsOptions.CopyAssociatedGroups = false;
                _permissionsOptions.CopyListPermissions = false;
                _permissionsOptions.CopyRootPermissions = false;
                _permissionsOptions.MapRolesByName = false;
                _permissionsOptions.CopyFolderPermissions = false;
                _permissionsOptions.CopyItemPermissions = false;
                _permissionsOptions.ClearRoleAssignments = false;
                return;
            }
            bool flag = base.Scope != SharePointObjectScope.List || (!BCSHelper.HasExternalListsOnly(SourceNodes) && !BCSHelper.HasExternalListsOnly(TargetNodes));
            bool flag2 = flag;
            bool flag3 = IsSetExplicitOptions || _areAllPermissionsReset;
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                bool flag4 = tsCopySitePermissions.IsOn && base.SitesAvailable;
                bool flag5 = !_permissionsOptions.CopySitePermissions && !_permissionsOptions.CopyPermissionLevels && !_permissionsOptions.CopyAccessRequestSettings && !_permissionsOptions.CopyAssociatedGroups;
                _permissionsOptions.CopySitePermissions = flag4;
                if (flag3 || flag5)
                {
                    _permissionsOptions.CopyPermissionLevels = flag4;
                    bool flag6 = false;
                    if (TargetNodes != null && TargetNodes.Count > 0 && TargetNodes[0] is SPNode sPNode)
                    {
                        flag6 = sPNode.CanWriteCreatedModifiedMetaInfo;
                    }
                    _permissionsOptions.CopyAccessRequestSettings = flag4 && flag6;
                    SPPermissionsOptions permissionsOptions = _permissionsOptions;
                    bool copyAssociatedGroups = flag4 && (base.SourceWeb == null || base.SourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater);
                    permissionsOptions.CopyAssociatedGroups = copyAssociatedGroups;
                }
            }
            if (base.Scope != SharePointObjectScope.Folder)
            {
                if (base.Scope != SharePointObjectScope.Item)
                {
                    _permissionsOptions.CopyListPermissions = tsCopyListPermissions.IsOn;
                    if (flag3 || (!_permissionsOptions.CopyFolderPermissions && !_permissionsOptions.CopyItemPermissions))
                    {
                        _permissionsOptions.CopyFolderPermissions = tsCopyContentPermissions.IsOn && base.FoldersAvailable && flag2 && tsCopyContentPermissions.Enabled && _isCopySubFoldersChecked;
                        _permissionsOptions.CopyItemPermissions = tsCopyContentPermissions.IsOn && base.ItemsAvailable && flag2 && tsCopyContentPermissions.Enabled && _isCopyListItemsChecked;
                    }
                    else if (!tsCopyContentPermissions.IsOn)
                    {
                        _permissionsOptions.CopyFolderPermissions = false;
                        _permissionsOptions.CopyItemPermissions = false;
                    }
                }
                else
                {
                    _permissionsOptions.CopyItemPermissions = tsCopyListPermissions.IsOn && base.ItemsAvailable && flag2 && tsCopyListPermissions.Enabled;
                }
            }
            else if (flag3 || (!_permissionsOptions.CopyFolderPermissions && !_permissionsOptions.CopyItemPermissions))
            {
                _permissionsOptions.CopyFolderPermissions = tsCopyListPermissions.IsOn && base.FoldersAvailable && tsCopyListPermissions.Enabled;
                _permissionsOptions.CopyItemPermissions = tsCopyListPermissions.IsOn && base.ItemsAvailable && tsCopyListPermissions.Enabled;
            }
            else if (!tsCopyListPermissions.IsOn)
            {
                _permissionsOptions.CopyFolderPermissions = false;
                _permissionsOptions.CopyItemPermissions = false;
            }
            bool copyRootPermissions = ((base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site) ? ((base.Scope != SharePointObjectScope.List && base.Scope != SharePointObjectScope.Folder) ? (_permissionsOptions.CopyItemPermissions && tsIgnorePermissions.Enabled && tsIgnorePermissions.IsOn) : (_permissionsOptions.CopyListPermissions && tsIgnorePermissions.Enabled && tsIgnorePermissions.IsOn)) : (_permissionsOptions.CopySitePermissions && tsMigrateBrokenSitePermissions.Enabled && tsMigrateBrokenSitePermissions.IsOn));
            _permissionsOptions.CopyRootPermissions = copyRootPermissions;
            _permissionsOptions.MapRolesByName = tsMapPermissions.IsOn;
            if (IsSetExplicitOptions && _areAllPermissionsReset)
            {
                _permissionsOptions.ClearRoleAssignments = true;
            }
        }

        public override bool SaveUI()
        {
            SaveManagedMetadata();
            SaveWebParts();
            SavePermissions();
            SaveMappings();
            if (base.Scope != SharePointObjectScope.Item && base.Scope != SharePointObjectScope.Folder)
            {
                SaveWorkflows();
            }
            return true;
        }

        private void SaveWebParts()
        {
            bool flag = ((base.Scope != SharePointObjectScope.Site && base.Scope != SharePointObjectScope.SiteCollection) || !_webPartOptions.CopySiteWebParts) && !_webPartOptions.CopyFormWebParts && !_webPartOptions.CopyDocumentWebParts && !_webPartOptions.CopyContentZoneContent;
            if (IsSetExplicitOptions || flag)
            {
                _webPartOptions.CopySiteWebParts = (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection) && tsCopyWebParts.IsOn;
                if (IsFormWebPartsOptionAvailable())
                {
                    _webPartOptions.CopyFormWebParts = true;
                }
                _webPartOptions.CopyDocumentWebParts = tsCopyWebParts.IsOn && base.ItemsAvailable;
                _webPartOptions.CopyContentZoneContent = tsCopyWebParts.IsOn;
            }
            else if (!tsCopyWebParts.IsOn)
            {
                _webPartOptions.CopySiteWebParts = false;
                _webPartOptions.CopyFormWebParts = false;
                _webPartOptions.CopyDocumentWebParts = false;
                _webPartOptions.CopyContentZoneContent = false;
            }
            _webPartOptions.CopyClosedWebParts = tsCopyClosedWebParts.IsOn;
        }

        private void SaveWorkflows()
        {
            bool flag = base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.List;
            bool flag2 = !_workflowOptions.CopyWebOOBWorkflowAssociations && !_workflowOptions.CopyWebSharePointDesignerNintexWorkflowAssociations && !_workflowOptions.CopyListOOBWorkflowAssociations && !_workflowOptions.CopyListSharePointDesignerNintexWorkflowAssociations && !_workflowOptions.CopyContentTypeOOBWorkflowAssociations && !_workflowOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations && !_workflowOptions.CopyGloballyReusableWorkflowTemplates;
            if (!flag || !tsWorkflow.IsOn)
            {
                _workflowOptions.CopyListOOBWorkflowAssociations = false;
                _workflowOptions.CopyWebOOBWorkflowAssociations = false;
                _workflowOptions.CopyContentTypeOOBWorkflowAssociations = false;
                _workflowOptions.CopyListSharePointDesignerNintexWorkflowAssociations = false;
                _workflowOptions.CopyWebSharePointDesignerNintexWorkflowAssociations = false;
                _workflowOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = false;
                _workflowOptions.CopyGloballyReusableWorkflowTemplates = false;
            }
            else if ((IsSetExplicitOptions || flag2) && SourceNodes[0] is SPNode sPNode)
            {
                SharePointAdapter adapter = sPNode.Adapter;
                if (TargetNodes[0] is SPNode sPNode2)
                {
                    SharePointAdapter adapter2 = sPNode2.Adapter;
                    bool isClientOM = adapter.IsClientOM;
                    bool isClientOM2 = adapter2.IsClientOM;
                    bool isSharePoint2010OrLater = adapter.SharePointVersion.IsSharePoint2010OrLater;
                    bool isSharePoint2010OrLater2 = adapter2.SharePointVersion.IsSharePoint2010OrLater;
                    bool isSharePoint2013OrLater = adapter2.SharePointVersion.IsSharePoint2013OrLater;
                    bool isSharePoint2013OrLater2 = adapter.SharePointVersion.IsSharePoint2013OrLater;
                    _isTargetAdapterSharePointOnline = adapter2.SharePointVersion.IsSharePointOnline;
                    bool isSharePointOnline = adapter.SharePointVersion.IsSharePointOnline;
                    bool flag3 = base.Scope == SharePointObjectScope.List;
                    bool flag4 = base.Scope == SharePointObjectScope.SiteCollection && !adapter2.SharePointVersion.IsSharePoint2007;
                    _workflowOptions.CopyWebOOBWorkflowAssociations = !(!isSharePoint2010OrLater || !isSharePoint2010OrLater2 || isClientOM || (isClientOM2 && adapter2.SharePointVersion.IsSharePoint2013OrLater) || flag3) && tsWorkflow.IsOn;
                    bool flag5 = !((!isSharePointOnline || !_isTargetAdapterSharePointOnline) && isClientOM) && !flag3;
                    bool flag6 = (isSharePointOnline || !isClientOM) && _isTargetAdapterSharePointOnline && flag3;
                    bool flag7 = isSharePoint2013OrLater2 && isSharePoint2013OrLater && !adapter.IsNws && !adapter.IsDB && !adapter2.IsNws && !adapter2.IsDB;
                    _workflowOptions.CopyWebSharePointDesignerNintexWorkflowAssociations = !(!isSharePoint2010OrLater || !isSharePoint2010OrLater2 || (!flag5 && !flag7) || flag3) && tsWorkflow.IsOn;
                    _workflowOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = (flag5 || flag6) && tsWorkflow.IsOn;
                    _workflowOptions.CopyListSharePointDesignerNintexWorkflowAssociations = (flag5 || flag6 || flag7) && tsWorkflow.IsOn;
                    _workflowOptions.CopyContentTypeOOBWorkflowAssociations = (flag5 || flag6) && tsWorkflow.IsOn;
                    _workflowOptions.CopyListOOBWorkflowAssociations = (flag5 || flag6) && tsWorkflow.IsOn;
                    _workflowOptions.CopyGloballyReusableWorkflowTemplates = flag4 && tsWorkflow.IsOn;
                }
            }
        }

        private bool ShowHideWorkflowForListScope()
        {
            if (base.Scope == SharePointObjectScope.List)
            {
                SPNode sPNode = SourceNodes[0] as SPNode;
                SPNode sPNode2 = TargetNodes[0] as SPNode;
                if (sPNode != null && sPNode2 != null)
                {
                    SharePointAdapter adapter = sPNode.Adapter;
                    SharePointAdapter adapter2 = sPNode2.Adapter;
                    if (adapter.IsClientOM || adapter.IsDB || adapter.IsNws || !adapter2.IsClientOM)
                    {
                        HideControl(tsWorkflow);
                        HideControl(helpWorkflows);
                        return false;
                    }
                }
            }
            return true;
        }

        private void tsCopyListPermissions_Toggled(object sender, EventArgs e)
        {
            if (base.Scope != SharePointObjectScope.Folder || (base.Scope == SharePointObjectScope.Folder && !_isIgnorePermissionsOptionDisabled))
            {
                tsIgnorePermissions.Enabled = tsCopyListPermissions.IsOn;
            }
            On_Toggle_Changed(sender, e);
        }

        private void tsCopySitePermissions_Toggled(object sender, EventArgs e)
        {
            tsMigrateBrokenSitePermissions.Enabled = tsCopySitePermissions.IsOn;
            On_Toggle_Changed(sender, e);
        }

        private void tsCopyWebParts_Toggled(object sender, EventArgs e)
        {
            if (!tsCopyWebParts.IsOn)
            {
                tsCopyClosedWebParts.IsOn = false;
            }
            tsCopyClosedWebParts.Enabled = tsCopyWebParts.IsOn;
        }

        private void tsMapGroupsByName_Toggled(object sender, EventArgs e)
        {
            tsOverwriteExistingGroups.Enabled = tsMapGroupsByName.IsOn;
            On_Toggle_Changed(sender, e);
        }

        private void tsMapMissingUsers_Toggled(object sender, EventArgs e)
        {
            txtMapMissingUsers.Enabled = tsMapMissingUsers.IsOn;
            On_Toggle_Changed(sender, e);
        }

        private void tsMapMMD_Toggled(object sender, EventArgs e)
        {
            btnMapMMD.Enabled = tsMapMMD.IsOn;
            bool flag = !_mmdOptions.MapTermStores && !_mmdOptions.ResolveManagedMetadataByName && !_mmdOptions.CopyReferencedManagedMetadata;
            if (tsMapMMD.IsOn && (flag || _mmdOptions.MapTermStores))
            {
                OpenMapTermsStoresDialog();
                return;
            }
            if (!tsMapMMD.IsOn && (_mmdOptions.ResolveManagedMetadataByName || flag) && FlatXtraMessageBox.Show(string.Format(Resources.FS_DisablingResolveByNameWarning, "Resolve managed metadata by name and hierarchy", Environment.NewLine), Resources.WarningCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                tsMapMMD.Toggled -= tsMapMMD_Toggled;
                tsMapMMD.IsOn = true;
                tsMapMMD.Toggled += tsMapMMD_Toggled;
            }
            SimpleButton simpleButton = btnMapMMD;
            bool enabled = (IsSetExplicitOptions ? tsMapMMD.IsOn : (tsMapMMD.IsOn && (_mmdOptions.MapTermStores || flag)));
            simpleButton.Enabled = enabled;
        }

        private void tsPermissions_Toggled(object sender, EventArgs e)
        {
            lcAdvancedPermissions.Visible = tsPermissions.IsOn;
            FireControlSizeChanged(lcAdvancedPermissions.Height, !lcAdvancedPermissions.Visible);
            if (!tsPermissions.IsOn)
            {
                return;
            }
            bool flag = false;
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                flag = tsCopySitePermissions.IsOn;
            }
            bool flag2 = false;
            if (base.Scope != SharePointObjectScope.Folder && base.Scope != SharePointObjectScope.Item)
            {
                flag2 = tsCopyContentPermissions.IsOn;
            }
            if (!(flag || tsCopyListPermissions.IsOn || flag2) && !tsMapGroupsByName.IsOn && !tsMapMissingUsers.IsOn && !tsMapPermissions.IsOn)
            {
                if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
                {
                    tsCopySitePermissions.IsOn = true;
                    tsMigrateBrokenSitePermissions.IsOn = true;
                }
                tsCopyListPermissions.IsOn = true;
                tsCopyContentPermissions.IsOn = true;
                tsIgnorePermissions.IsOn = true;
                tsMapPermissions.IsOn = true;
                tsMapGroupsByName.IsOn = true;
                tsOverwriteExistingGroups.IsOn = true;
                tsMapMissingUsers.IsOn = false;
                _areAllPermissionsReset = true;
            }
        }

        public void UpdateFolderPermissionsState(bool isEnabled, SPFolderContentOptions folderOptions)
        {
            if (!IsSetExplicitOptions)
            {
                tsCopyListPermissions.Enabled = isEnabled || folderOptions.CopySubFolders;
                tsIgnorePermissions.Enabled = folderOptions.CopySubFolders;
                _isIgnorePermissionsOptionDisabled = !folderOptions.CopySubFolders;
            }
        }

        public void UpdatePermissionsState(bool isEnabled, SPListContentOptions listContentOptions)
        {
            if (base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site)
            {
                if (base.Scope == SharePointObjectScope.List)
                {
                    if (!IsSetExplicitOptions)
                    {
                        tsCopyContentPermissions.Enabled = isEnabled || listContentOptions.CopySubFolders;
                        _isCopyListItemsChecked = isEnabled;
                        _isCopySubFoldersChecked = listContentOptions.CopySubFolders;
                    }
                    else
                    {
                        _isCopyListItemsChecked = isEnabled;
                        _isCopySubFoldersChecked = true;
                    }
                }
                return;
            }
            tsCopyListPermissions.Enabled = isEnabled;
            tsCopyContentPermissions.Enabled = isEnabled;
            if (!isEnabled)
            {
                tsCopyContentPermissions.IsOn = false;
            }
            if (IsSetExplicitOptions)
            {
                _isCopyListItemsChecked = true;
                _isCopySubFoldersChecked = true;
                return;
            }
            ToggleSwitch toggleSwitch = tsCopyContentPermissions;
            bool enabled = isEnabled && (listContentOptions.CopyListItems || listContentOptions.CopySubFolders);
            toggleSwitch.Enabled = enabled;
            _isCopyListItemsChecked = listContentOptions.CopyListItems;
            _isCopySubFoldersChecked = listContentOptions.CopySubFolders;
        }

        protected override void UpdateScope()
        {
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    {
                        HideControl(tsIgnorePermissions);
                        helpIgnorePermissions.Visible = false;
                        tsWorkflow.BringToFront();
                        ToggleSwitch toggleSwitch2 = tsCopyListPermissions;
                        int num2 = tsCopyListPermissions.Location.X;
                        toggleSwitch2.Location = new Point(num2, tsCopyListPermissions.Location.Y + 4);
                        HelpTipButton helpTipButton3 = helpCopyListPermissions;
                        int num3 = helpCopyListPermissions.Location.X;
                        helpTipButton3.Location = new Point(num3, helpCopyListPermissions.Location.Y + 4);
                        FireControlSizeChanged(tsIgnorePermissions.Height, isDecreaseSize: true);
                        break;
                    }
                case SharePointObjectScope.List:
                    {
                        Type type = GetType();
                        helpWorkflows.SetResourceString(type.FullName + tsWorkflow.Name + "ListScope", type);
                        HideControl(tsCopySitePermissions);
                        HideControl(tsMigrateBrokenSitePermissions);
                        helpMigrateBrokenSitePermissions.Visible = false;
                        helpCopySitePermissions.Visible = false;
                        FireControlSizeChanged(tsWorkflow.Height, isDecreaseSize: true);
                        break;
                    }
                case SharePointObjectScope.Folder:
                    {
                        tsCopyListPermissions.Properties.OffText = "Copy Items and Folders Permissions";
                        tsCopyListPermissions.Properties.OnText = "Copy Items and Folders Permissions";
                        tsCopyListPermissions.Width = 257;
                        tsIgnorePermissions.Properties.OffText = "Migrate Item and Folder with Broken Inheritance";
                        tsIgnorePermissions.Properties.OnText = "Migrate Item and Folder with Broken Inheritance";
                        tsIgnorePermissions.Width = 320;
                        helpMigrateBrokenSitePermissions.Visible = false;
                        helpCopySitePermissions.Visible = false;
                        HideControl(tsWorkflow);
                        HideControl(helpWorkflows);
                        HideControl(tsCopyContentPermissions);
                        HideControl(tsCopySitePermissions);
                        HideControl(tsMigrateBrokenSitePermissions);
                        FireControlSizeChanged(tsWorkflow.Height, isDecreaseSize: true);
                        Type type = GetType();
                        helpCopyListPermissions.SetResourceString(type.FullName + "tsCopyFolderPermissions", type);
                        helpIgnorePermissions.SetResourceString(type.FullName + "tsFolderIgnorePermissions", type);
                        HelpTipButton helpTipButton = helpCopyListPermissions;
                        Point location = helpCopyListPermissions.Location;
                        Point location2 = helpCopyListPermissions.Location;
                        helpTipButton.Location = new Point(location.X + 64, location2.Y);
                        HelpTipButton helpTipButton2 = helpIgnorePermissions;
                        Point location3 = helpIgnorePermissions.Location;
                        Point location4 = helpIgnorePermissions.Location;
                        helpTipButton2.Location = new Point(location3.X + 58, location4.Y);
                        helpCopyContentPermissions.Visible = false;
                        helpMigrateBrokenSitePermissions.Visible = false;
                        helpCopySitePermissions.Visible = false;
                        break;
                    }
                case SharePointObjectScope.Item:
                    {
                        HideControl(tsWorkflow);
                        HideControl(helpWorkflows);
                        HideControl(tsCopySitePermissions);
                        HideControl(tsMigrateBrokenSitePermissions);
                        HideControl(tsCopyContentPermissions);
                        helpMigrateBrokenSitePermissions.Visible = false;
                        helpCopySitePermissions.Visible = false;
                        helpCopyContentPermissions.Visible = false;
                        FireControlSizeChanged(tsCopyContentPermissions.Height, isDecreaseSize: true);
                        tsCopyListPermissions.Properties.OnText = "Copy Item Permissions";
                        tsCopyListPermissions.Properties.OffText = "Copy Item Permissions";
                        tsIgnorePermissions.Properties.OnText = "Migrate Item with Broken Inheritance";
                        tsIgnorePermissions.Properties.OffText = "Migrate Item with Broken Inheritance";
                        ToggleSwitch toggleSwitch = tsIgnorePermissions;
                        int num = tsIgnorePermissions.Location.X;
                        toggleSwitch.Location = new Point(num, tsIgnorePermissions.Location.Y + 3);
                        Type type = GetType();
                        helpCopyListPermissions.SetResourceString(type.FullName + "tsCopyItemPermissions", type);
                        helpIgnorePermissions.SetResourceString(type.FullName + "tsItemIgnorePermissions", type);
                        break;
                    }
            }
        }

        private void UpdateUI()
        {
            if (!IsSupported())
            {
                DisableUI();
            }
            else if (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.List)
            {
                tsWorkflow.IsOn = _workflowOptions.CopyWebOOBWorkflowAssociations || _workflowOptions.CopyWebSharePointDesignerNintexWorkflowAssociations || _workflowOptions.CopyListOOBWorkflowAssociations || _workflowOptions.CopyListSharePointDesignerNintexWorkflowAssociations || _workflowOptions.CopyContentTypeOOBWorkflowAssociations || _workflowOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations || _workflowOptions.CopyGloballyReusableWorkflowTemplates;
                if (base.Scope == SharePointObjectScope.List)
                {
                    ShowHideWorkflowForListScope();
                }
            }
            else
            {
                DisableUI();
            }
        }
    }
}
