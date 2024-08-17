
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions.Properties;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.SiteOptions.png")]
    [ControlName("Site Options")]
    public class TCSiteContentOptions : SiteContentOptionsScopableTabbableControl
    {
        private const string SITENAMELABEL = "Change Site Name To";

        private const string SITETITLELABEL = "Change Site Title To";

        private bool m_bSuspendRenameEvents = true;

        private static readonly string[] RestrictedSiteTemplates;

        public static readonly Expression<Func<SPWebTemplate, bool>> SiteOnlyTemplates;

        private string m_sTemplateName;

        private string m_sNewSiteName;

        private string m_sNewSiteTitle;

        private SPSiteContentOptions m_options;

        private SPWebTemplateCollection m_targetTemplates;

        private IContainer components;

        private PanelControl w_plSiteScoped;

        private CheckEdit w_cbChangeTemplate;

        private CheckEdit w_cbRenameSite;

        private CheckEdit w_cbContentTypes;

        private PanelControl w_plFeatures;

        private CheckEdit w_rbClearTargetFeatures;

        private CheckEdit w_rbPreserveOnTarget;

        private CheckEdit w_cbSiteFeatures;

        private CheckEdit w_cbCopySubsites;

        private CheckEdit w_cbApplyTheme;

        private CheckEdit w_cbPreserveMasterPage;

        private CheckEdit w_cbNavigation;

        private PanelControl w_plSingleSitePanel;

        private CheckEdit w_cbRunNavigationStructureCopy;

        private PanelControl w_plNavStructCopy;

        private CheckEdit w_rbCopyOnlyTopNavBar;

        private CheckEdit w_rbCopyOnlyQuickLaunch;

        private CheckEdit w_rbCopyBothQuickLaunchAndTopNav;

        internal CheckEdit w_cbCopyPortalListings;

        private PanelControl w_plCopyPortalListings;

        private PanelControl w_plMasterPage;

        internal CheckEdit w_cbCopyCustomContent;

        private PanelControl w_plUIVersion;

        private CheckEdit w_cbPreserveUIVersion;

        private HelpTipButton w_helpClearTargetFeatures;

        private HelpTipButton helpTipCopyOnlyQuickLaunch;

        private HelpTipButton w_helpApplyTheme;

        private HelpTipButton w_helpCopySubsites;

        private HelpTipButton w_helpPreserveOnTarget;

        private CheckEdit _rbAllThemes;

        private CheckEdit _rbCurrentTheme;

        private PanelControl _plThemes;

        private ComboBoxEdit w_cmbTemplateName;

        private CheckEdit w_cbCopyUncustomizedFiles;

        private TextEdit _teNewSiteTitle;

        private TextEdit _teNewSiteName;

        private LabelControl _lblNewSiteTitle;

        private LabelControl _lblNewSiteName;

        public string NewSiteName
        {
            get
            {
                return m_sNewSiteName;
            }
            set
            {
                m_sNewSiteName = value;
                UpdateRenameUI();
            }
        }

        public string NewSiteTitle
        {
            get
            {
                return m_sNewSiteTitle;
            }
            set
            {
                m_sNewSiteTitle = value;
                UpdateRenameUI();
            }
        }

        public SPSiteContentOptions Options
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

        public override NodeCollection TargetNodes
        {
            get
            {
                return base.TargetNodes;
            }
            set
            {
                base.TargetNodes = value;
                if (base.TargetNodes != null && base.TargetNodes.Count > 0 && base.TargetNodes[0] is SPWeb)
                {
                    TargetWebTemplates = ((SPWeb)base.TargetNodes[0]).Templates;
                }
            }
        }

        private SPWebTemplateCollection TargetWebTemplates
        {
            get
            {
                return m_targetTemplates;
            }
            set
            {
                m_targetTemplates = value;
            }
        }

        public string TemplateName
        {
            get
            {
                return m_sTemplateName;
            }
            set
            {
                m_sTemplateName = value;
                UpdateTemplateUI();
            }
        }

        static TCSiteContentOptions()
        {
            string[] restrictedSiteTemplates = new string[4] { "ACCSRV#0", "ACCSRV#1", "DOCMARKETPLACESITE#0", "EDISC#1" };
            RestrictedSiteTemplates = restrictedSiteTemplates;
            SiteOnlyTemplates = (SPWebTemplate template) => !template.IsRootWebOnly && !template.IsHidden;
        }

        public TCSiteContentOptions()
        {
            InitializeComponent();
            Type type = GetType();
            w_helpApplyTheme.SetResourceString(type.FullName + w_cbApplyTheme.Name, type);
            w_helpClearTargetFeatures.SetResourceString(type.FullName + w_rbClearTargetFeatures.Name, type);
            w_helpCopySubsites.SetResourceString(type.FullName + w_cbCopySubsites.Name, type);
            w_helpPreserveOnTarget.SetResourceString(type.FullName + w_rbPreserveOnTarget.Name, type);
            helpTipCopyOnlyQuickLaunch.SetResourceString(type.FullName + w_rbCopyOnlyQuickLaunch.Name, type);
            helpTipCopyOnlyQuickLaunch.VisibleChanged += helpTipCopyOnlyQuickLaunch_VisibleChanged;
        }

        private void _teNewSiteName_EditValueChanged(object sender, EventArgs e)
        {
            if (!m_bSuspendRenameEvents)
            {
                m_sNewSiteName = _teNewSiteName.Text;
                FireSiteRenameStateChanged();
            }
        }

        private void _teNewSiteName_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (!m_bSuspendRenameEvents)
            {
                if (string.IsNullOrEmpty(_teNewSiteName.Text))
                {
                    XtraMessageBox.Show("Value cannot be empty.");
                    e.Cancel = true;
                }
                if (!Utils.IsValidSharePointURL(_teNewSiteName.Text))
                {
                    XtraMessageBox.Show(Resources.ValidSiteURL + " " + Utils.IllegalCharactersForSiteUrl);
                    e.Cancel = true;
                }
            }
        }

        private void _teNewSiteTitle_EditValueChanged(object sender, EventArgs e)
        {
            if (!m_bSuspendRenameEvents)
            {
                m_sNewSiteTitle = _teNewSiteTitle.Text;
                FireSiteRenameStateChanged();
            }
        }

        private void _teNewSiteTitle_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (!m_bSuspendRenameEvents)
            {
                if (string.IsNullOrEmpty(_teNewSiteTitle.Text))
                {
                    XtraMessageBox.Show("Value cannot be empty.");
                    e.Cancel = true;
                }
                if (!Utils.IsValidSharePointURL(_teNewSiteTitle.Text))
                {
                    XtraMessageBox.Show(Resources.ValidSiteURL + " " + Utils.IllegalCharactersForSiteUrl);
                    e.Cancel = true;
                }
            }
        }

        private void ApplyTheme_CheckChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        private void DisableNavigationOptions()
        {
            w_cbNavigation.Checked = false;
            w_cbRunNavigationStructureCopy.Checked = false;
            w_rbCopyBothQuickLaunchAndTopNav.Checked = false;
            w_rbCopyOnlyQuickLaunch.Checked = false;
            w_rbCopyOnlyTopNavBar.Checked = false;
            w_cbNavigation.Enabled = false;
            w_cbRunNavigationStructureCopy.Enabled = false;
            w_rbCopyBothQuickLaunchAndTopNav.Enabled = false;
            w_rbCopyOnlyQuickLaunch.Enabled = false;
            w_rbCopyOnlyTopNavBar.Enabled = false;
        }

        private void DisableOnlyQuickLaunchAndOnlyTopLinkOption()
        {
            if (SPUIUtils.IsOptionsCopyingOnlyTopLinkAndQuickLaunchToBeDisabled(SourceNodes, TargetNodes))
            {
                w_rbCopyOnlyQuickLaunch.Enabled = false;
                w_rbCopyOnlyTopNavBar.Enabled = false;
                w_rbCopyBothQuickLaunchAndTopNav.Checked = true;
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

        private void FireSiteRenameStateChanged()
        {
            RenameInfo renameInfo = default(RenameInfo);
            if (!w_cbRenameSite.Checked)
            {
                renameInfo.Name = null;
                renameInfo.Title = null;
            }
            else
            {
                renameInfo.Name = NewSiteName;
                renameInfo.Title = NewSiteTitle;
            }
            SendMessage("TopLevelNodeRenamed", renameInfo);
        }

        public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
        {
            if (sMessage != "TopLevelNodeRenamed")
            {
                if (sMessage == "AvailableTemplatesChanged")
                {
                    TargetWebTemplates = oValue as SPWebTemplateCollection;
                }
            }
            else
            {
                RenameInfo renameInfo = (RenameInfo)oValue;
                SiteRenamingChanged(renameInfo.Name, renameInfo.Title);
            }
        }

        private void helpTipCopyOnlyQuickLaunch_VisibleChanged(object sender, EventArgs e)
        {
            helpTipCopyOnlyQuickLaunch.Visible = SPUIUtils.IsOptionsCopyingOnlyTopLinkAndQuickLaunchToBeDisabled(SourceNodes, TargetNodes);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCSiteContentOptions));
            this._plThemes = new DevExpress.XtraEditors.PanelControl();
            this._rbAllThemes = new DevExpress.XtraEditors.CheckEdit();
            this._rbCurrentTheme = new DevExpress.XtraEditors.CheckEdit();
            this.w_helpApplyTheme = new TooltipsTest.HelpTipButton();
            this.w_cbApplyTheme = new DevExpress.XtraEditors.CheckEdit();
            this.w_plUIVersion = new DevExpress.XtraEditors.PanelControl();
            this.w_cbPreserveUIVersion = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCopyCustomContent = new DevExpress.XtraEditors.CheckEdit();
            this.w_plMasterPage = new DevExpress.XtraEditors.PanelControl();
            this.w_cbPreserveMasterPage = new DevExpress.XtraEditors.CheckEdit();
            this.w_plCopyPortalListings = new DevExpress.XtraEditors.PanelControl();
            this.w_cbCopyPortalListings = new DevExpress.XtraEditors.CheckEdit();
            this.w_plNavStructCopy = new DevExpress.XtraEditors.PanelControl();
            this.w_rbCopyOnlyTopNavBar = new DevExpress.XtraEditors.CheckEdit();
            this.helpTipCopyOnlyQuickLaunch = new TooltipsTest.HelpTipButton();
            this.w_rbCopyOnlyQuickLaunch = new DevExpress.XtraEditors.CheckEdit();
            this.w_rbCopyBothQuickLaunchAndTopNav = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbRunNavigationStructureCopy = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbNavigation = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCopySubsites = new DevExpress.XtraEditors.CheckEdit();
            this.w_plFeatures = new DevExpress.XtraEditors.PanelControl();
            this.w_helpPreserveOnTarget = new TooltipsTest.HelpTipButton();
            this.w_rbPreserveOnTarget = new DevExpress.XtraEditors.CheckEdit();
            this.w_helpClearTargetFeatures = new TooltipsTest.HelpTipButton();
            this.w_rbClearTargetFeatures = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbSiteFeatures = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbContentTypes = new DevExpress.XtraEditors.CheckEdit();
            this.w_plSiteScoped = new DevExpress.XtraEditors.PanelControl();
            this.w_cmbTemplateName = new DevExpress.XtraEditors.ComboBoxEdit();
            this.w_plSingleSitePanel = new DevExpress.XtraEditors.PanelControl();
            this._lblNewSiteTitle = new DevExpress.XtraEditors.LabelControl();
            this._lblNewSiteName = new DevExpress.XtraEditors.LabelControl();
            this._teNewSiteTitle = new DevExpress.XtraEditors.TextEdit();
            this._teNewSiteName = new DevExpress.XtraEditors.TextEdit();
            this.w_cbRenameSite = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbChangeTemplate = new DevExpress.XtraEditors.CheckEdit();
            this.w_helpCopySubsites = new TooltipsTest.HelpTipButton();
            this.w_cbCopyUncustomizedFiles = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)this._plThemes).BeginInit();
            this._plThemes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._rbAllThemes.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._rbCurrentTheme.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpApplyTheme).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbApplyTheme.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plUIVersion).BeginInit();
            this.w_plUIVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbPreserveUIVersion.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyCustomContent.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plMasterPage).BeginInit();
            this.w_plMasterPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbPreserveMasterPage.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plCopyPortalListings).BeginInit();
            this.w_plCopyPortalListings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyPortalListings.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plNavStructCopy).BeginInit();
            this.w_plNavStructCopy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyTopNavBar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTipCopyOnlyQuickLaunch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyQuickLaunch.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyBothQuickLaunchAndTopNav.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRunNavigationStructureCopy.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbNavigation.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopySubsites.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plFeatures).BeginInit();
            this.w_plFeatures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_helpPreserveOnTarget).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbPreserveOnTarget.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpClearTargetFeatures).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbClearTargetFeatures.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbSiteFeatures.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbContentTypes.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSiteScoped).BeginInit();
            this.w_plSiteScoped.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cmbTemplateName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSingleSitePanel).BeginInit();
            this.w_plSingleSitePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._teNewSiteTitle.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._teNewSiteName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRenameSite.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbChangeTemplate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpCopySubsites).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyUncustomizedFiles.Properties).BeginInit();
            base.SuspendLayout();
            this._plThemes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._plThemes.Controls.Add(this._rbAllThemes);
            this._plThemes.Controls.Add(this._rbCurrentTheme);
            this._plThemes.Controls.Add(this.w_helpApplyTheme);
            resources.ApplyResources(this._plThemes, "_plThemes");
            this._plThemes.Name = "_plThemes";
            resources.ApplyResources(this._rbAllThemes, "_rbAllThemes");
            this._rbAllThemes.Name = "_rbAllThemes";
            this._rbAllThemes.Properties.AutoWidth = true;
            this._rbAllThemes.Properties.Caption = resources.GetString("_rbAllThemes.Properties.Caption");
            this._rbAllThemes.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this._rbAllThemes.Properties.RadioGroupIndex = 3;
            this._rbAllThemes.TabStop = false;
            resources.ApplyResources(this._rbCurrentTheme, "_rbCurrentTheme");
            this._rbCurrentTheme.Name = "_rbCurrentTheme";
            this._rbCurrentTheme.Properties.AutoWidth = true;
            this._rbCurrentTheme.Properties.Caption = resources.GetString("_rbCurrentTheme.Properties.Caption");
            this._rbCurrentTheme.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this._rbCurrentTheme.Properties.RadioGroupIndex = 3;
            this._rbCurrentTheme.TabStop = false;
            this.w_helpApplyTheme.AnchoringControl = this.w_cbApplyTheme;
            this.w_helpApplyTheme.BackColor = System.Drawing.Color.Transparent;
            this.w_helpApplyTheme.CommonParentControl = null;
            resources.ApplyResources(this.w_helpApplyTheme, "w_helpApplyTheme");
            this.w_helpApplyTheme.Name = "w_helpApplyTheme";
            this.w_helpApplyTheme.TabStop = false;
            resources.ApplyResources(this.w_cbApplyTheme, "w_cbApplyTheme");
            this.w_cbApplyTheme.Name = "w_cbApplyTheme";
            this.w_cbApplyTheme.Properties.AutoWidth = true;
            this.w_cbApplyTheme.Properties.Caption = resources.GetString("w_cbApplyTheme.Properties.Caption");
            this.w_cbApplyTheme.CheckedChanged += new System.EventHandler(ApplyTheme_CheckChanged);
            this.w_plUIVersion.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plUIVersion.Controls.Add(this.w_cbPreserveUIVersion);
            resources.ApplyResources(this.w_plUIVersion, "w_plUIVersion");
            this.w_plUIVersion.Name = "w_plUIVersion";
            resources.ApplyResources(this.w_cbPreserveUIVersion, "w_cbPreserveUIVersion");
            this.w_cbPreserveUIVersion.Name = "w_cbPreserveUIVersion";
            this.w_cbPreserveUIVersion.Properties.AutoWidth = true;
            this.w_cbPreserveUIVersion.Properties.Caption = resources.GetString("w_cbPreserveUIVersion.Properties.Caption");
            resources.ApplyResources(this.w_cbCopyCustomContent, "w_cbCopyCustomContent");
            this.w_cbCopyCustomContent.Name = "w_cbCopyCustomContent";
            this.w_cbCopyCustomContent.Properties.AutoWidth = true;
            this.w_cbCopyCustomContent.Properties.Caption = resources.GetString("w_cbCopyCustomContent.Properties.Caption");
            this.w_cbCopyCustomContent.CheckedChanged += new System.EventHandler(On_CopyCustomContentChanged);
            this.w_plMasterPage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plMasterPage.Controls.Add(this.w_cbPreserveMasterPage);
            resources.ApplyResources(this.w_plMasterPage, "w_plMasterPage");
            this.w_plMasterPage.Name = "w_plMasterPage";
            resources.ApplyResources(this.w_cbPreserveMasterPage, "w_cbPreserveMasterPage");
            this.w_cbPreserveMasterPage.Name = "w_cbPreserveMasterPage";
            this.w_cbPreserveMasterPage.Properties.AutoWidth = true;
            this.w_cbPreserveMasterPage.Properties.Caption = resources.GetString("w_cbPreserveMasterPage.Properties.Caption");
            this.w_plCopyPortalListings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plCopyPortalListings.Controls.Add(this.w_cbCopyPortalListings);
            resources.ApplyResources(this.w_plCopyPortalListings, "w_plCopyPortalListings");
            this.w_plCopyPortalListings.Name = "w_plCopyPortalListings";
            resources.ApplyResources(this.w_cbCopyPortalListings, "w_cbCopyPortalListings");
            this.w_cbCopyPortalListings.Name = "w_cbCopyPortalListings";
            this.w_cbCopyPortalListings.Properties.AutoWidth = true;
            this.w_cbCopyPortalListings.Properties.Caption = resources.GetString("w_cbCopyPortalListings.Properties.Caption");
            this.w_cbCopyPortalListings.CheckedChanged += new System.EventHandler(On_CopyPortalListings_CheckedChanged);
            this.w_cbCopyPortalListings.VisibleChanged += new System.EventHandler(On_CopyPortalListings_VisibleChanged);
            resources.ApplyResources(this.w_plNavStructCopy, "w_plNavStructCopy");
            this.w_plNavStructCopy.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plNavStructCopy.Controls.Add(this.w_rbCopyOnlyTopNavBar);
            this.w_plNavStructCopy.Controls.Add(this.helpTipCopyOnlyQuickLaunch);
            this.w_plNavStructCopy.Controls.Add(this.w_rbCopyOnlyQuickLaunch);
            this.w_plNavStructCopy.Controls.Add(this.w_rbCopyBothQuickLaunchAndTopNav);
            this.w_plNavStructCopy.Controls.Add(this.w_cbRunNavigationStructureCopy);
            this.w_plNavStructCopy.Name = "w_plNavStructCopy";
            resources.ApplyResources(this.w_rbCopyOnlyTopNavBar, "w_rbCopyOnlyTopNavBar");
            this.w_rbCopyOnlyTopNavBar.Name = "w_rbCopyOnlyTopNavBar";
            this.w_rbCopyOnlyTopNavBar.Properties.AutoWidth = true;
            this.w_rbCopyOnlyTopNavBar.Properties.Caption = resources.GetString("w_rbCopyOnlyTopNavBar.Properties.Caption");
            this.w_rbCopyOnlyTopNavBar.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbCopyOnlyTopNavBar.Properties.RadioGroupIndex = 1;
            this.w_rbCopyOnlyTopNavBar.TabStop = false;
            resources.ApplyResources(this.w_rbCopyOnlyQuickLaunch, "w_rbCopyOnlyQuickLaunch");
            this.w_rbCopyOnlyQuickLaunch.Name = "w_rbCopyOnlyQuickLaunch";
            this.w_rbCopyOnlyQuickLaunch.Properties.AutoWidth = true;
            this.w_rbCopyOnlyQuickLaunch.Properties.Caption = resources.GetString("w_rbCopyOnlyQuickLaunch.Properties.Caption");
            this.w_rbCopyOnlyQuickLaunch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbCopyOnlyQuickLaunch.Properties.RadioGroupIndex = 1;
            this.w_rbCopyOnlyQuickLaunch.TabStop = false;
            this.helpTipCopyOnlyQuickLaunch.AnchoringControl = this.w_rbCopyOnlyQuickLaunch;
            this.helpTipCopyOnlyQuickLaunch.BackColor = System.Drawing.Color.Transparent;
            this.helpTipCopyOnlyQuickLaunch.CommonParentControl = null;
            resources.ApplyResources(this.helpTipCopyOnlyQuickLaunch, "helpTipCopyOnlyQuickLaunch");
            this.helpTipCopyOnlyQuickLaunch.Name = "w_helpCopyOnlyQuickLaunch";
            this.helpTipCopyOnlyQuickLaunch.TabStop = false;
            resources.ApplyResources(this.w_rbCopyBothQuickLaunchAndTopNav, "w_rbCopyBothQuickLaunchAndTopNav");
            this.w_rbCopyBothQuickLaunchAndTopNav.Name = "w_rbCopyBothQuickLaunchAndTopNav";
            this.w_rbCopyBothQuickLaunchAndTopNav.Properties.AutoWidth = true;
            this.w_rbCopyBothQuickLaunchAndTopNav.Properties.Caption = resources.GetString("w_rbCopyBothQuickLaunchAndTopNav.Properties.Caption");
            this.w_rbCopyBothQuickLaunchAndTopNav.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbCopyBothQuickLaunchAndTopNav.Properties.RadioGroupIndex = 1;
            this.w_rbCopyBothQuickLaunchAndTopNav.TabStop = false;
            resources.ApplyResources(this.w_cbRunNavigationStructureCopy, "w_cbRunNavigationStructureCopy");
            this.w_cbRunNavigationStructureCopy.Name = "w_cbRunNavigationStructureCopy";
            this.w_cbRunNavigationStructureCopy.Properties.AutoWidth = true;
            this.w_cbRunNavigationStructureCopy.Properties.Caption = resources.GetString("w_cbRunNavigationStructureCopy.Properties.Caption");
            this.w_cbRunNavigationStructureCopy.CheckedChanged += new System.EventHandler(On_CheckedChanged);
            resources.ApplyResources(this.w_cbNavigation, "w_cbNavigation");
            this.w_cbNavigation.Name = "w_cbNavigation";
            this.w_cbNavigation.Properties.AutoWidth = true;
            this.w_cbNavigation.Properties.Caption = resources.GetString("w_cbNavigation.Properties.Caption");
            this.w_cbNavigation.CheckedChanged += new System.EventHandler(On_CheckedChanged);
            resources.ApplyResources(this.w_cbCopySubsites, "w_cbCopySubsites");
            this.w_cbCopySubsites.Name = "w_cbCopySubsites";
            this.w_cbCopySubsites.Properties.AutoWidth = true;
            this.w_cbCopySubsites.Properties.Caption = resources.GetString("w_cbCopySubsites.Properties.Caption");
            this.w_cbCopySubsites.CheckedChanged += new System.EventHandler(On_CopySubSitesChanged);
            this.w_plFeatures.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plFeatures.Controls.Add(this.w_helpPreserveOnTarget);
            this.w_plFeatures.Controls.Add(this.w_helpClearTargetFeatures);
            this.w_plFeatures.Controls.Add(this.w_rbClearTargetFeatures);
            this.w_plFeatures.Controls.Add(this.w_rbPreserveOnTarget);
            resources.ApplyResources(this.w_plFeatures, "w_plFeatures");
            this.w_plFeatures.Name = "w_plFeatures";
            this.w_helpPreserveOnTarget.AnchoringControl = this.w_rbPreserveOnTarget;
            this.w_helpPreserveOnTarget.BackColor = System.Drawing.Color.Transparent;
            this.w_helpPreserveOnTarget.CommonParentControl = null;
            resources.ApplyResources(this.w_helpPreserveOnTarget, "w_helpPreserveOnTarget");
            this.w_helpPreserveOnTarget.Name = "w_helpPreserveOnTarget";
            this.w_helpPreserveOnTarget.TabStop = false;
            resources.ApplyResources(this.w_rbPreserveOnTarget, "w_rbPreserveOnTarget");
            this.w_rbPreserveOnTarget.Name = "w_rbPreserveOnTarget";
            this.w_rbPreserveOnTarget.Properties.AutoWidth = true;
            this.w_rbPreserveOnTarget.Properties.Caption = resources.GetString("w_rbPreserveOnTarget.Properties.Caption");
            this.w_rbPreserveOnTarget.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbPreserveOnTarget.Properties.RadioGroupIndex = 2;
            this.w_helpClearTargetFeatures.AnchoringControl = this.w_rbClearTargetFeatures;
            this.w_helpClearTargetFeatures.BackColor = System.Drawing.Color.Transparent;
            this.w_helpClearTargetFeatures.CommonParentControl = null;
            resources.ApplyResources(this.w_helpClearTargetFeatures, "w_helpClearTargetFeatures");
            this.w_helpClearTargetFeatures.Name = "w_helpClearTargetFeatures";
            this.w_helpClearTargetFeatures.TabStop = false;
            resources.ApplyResources(this.w_rbClearTargetFeatures, "w_rbClearTargetFeatures");
            this.w_rbClearTargetFeatures.Name = "w_rbClearTargetFeatures";
            this.w_rbClearTargetFeatures.Properties.AutoWidth = true;
            this.w_rbClearTargetFeatures.Properties.Caption = resources.GetString("w_rbClearTargetFeatures.Properties.Caption");
            this.w_rbClearTargetFeatures.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbClearTargetFeatures.Properties.RadioGroupIndex = 2;
            this.w_rbClearTargetFeatures.TabStop = false;
            resources.ApplyResources(this.w_cbSiteFeatures, "w_cbSiteFeatures");
            this.w_cbSiteFeatures.Name = "w_cbSiteFeatures";
            this.w_cbSiteFeatures.Properties.AutoWidth = true;
            this.w_cbSiteFeatures.Properties.Caption = resources.GetString("w_cbSiteFeatures.Properties.Caption");
            this.w_cbSiteFeatures.CheckedChanged += new System.EventHandler(On_CopySiteFeatures_Checked);
            resources.ApplyResources(this.w_cbContentTypes, "w_cbContentTypes");
            this.w_cbContentTypes.Name = "w_cbContentTypes";
            this.w_cbContentTypes.Properties.AutoWidth = true;
            this.w_cbContentTypes.Properties.Caption = resources.GetString("w_cbContentTypes.Properties.Caption");
            resources.ApplyResources(this.w_plSiteScoped, "w_plSiteScoped");
            this.w_plSiteScoped.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plSiteScoped.Controls.Add(this.w_cmbTemplateName);
            this.w_plSiteScoped.Controls.Add(this.w_plSingleSitePanel);
            this.w_plSiteScoped.Controls.Add(this.w_cbChangeTemplate);
            this.w_plSiteScoped.Name = "w_plSiteScoped";
            resources.ApplyResources(this.w_cmbTemplateName, "w_cmbTemplateName");
            this.w_cmbTemplateName.Name = "w_cmbTemplateName";
            DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_cmbTemplateName.Properties.Buttons;
            DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_cmbTemplateName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_cmbTemplateName.SelectedIndexChanged += new System.EventHandler(On_SelectedTemplate_Changed);
            this.w_plSingleSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plSingleSitePanel.Controls.Add(this._lblNewSiteTitle);
            this.w_plSingleSitePanel.Controls.Add(this._lblNewSiteName);
            this.w_plSingleSitePanel.Controls.Add(this._teNewSiteTitle);
            this.w_plSingleSitePanel.Controls.Add(this._teNewSiteName);
            this.w_plSingleSitePanel.Controls.Add(this.w_cbRenameSite);
            resources.ApplyResources(this.w_plSingleSitePanel, "w_plSingleSitePanel");
            this.w_plSingleSitePanel.Name = "w_plSingleSitePanel";
            resources.ApplyResources(this._lblNewSiteTitle, "_lblNewSiteTitle");
            this._lblNewSiteTitle.Name = "_lblNewSiteTitle";
            resources.ApplyResources(this._lblNewSiteName, "_lblNewSiteName");
            this._lblNewSiteName.Name = "_lblNewSiteName";
            resources.ApplyResources(this._teNewSiteTitle, "_teNewSiteTitle");
            this._teNewSiteTitle.Name = "_teNewSiteTitle";
            this._teNewSiteTitle.EditValueChanged += new System.EventHandler(_teNewSiteTitle_EditValueChanged);
            this._teNewSiteTitle.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(_teNewSiteTitle_EditValueChanging);
            resources.ApplyResources(this._teNewSiteName, "_teNewSiteName");
            this._teNewSiteName.Name = "_teNewSiteName";
            this._teNewSiteName.EditValueChanged += new System.EventHandler(_teNewSiteName_EditValueChanged);
            this._teNewSiteName.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(_teNewSiteName_EditValueChanging);
            resources.ApplyResources(this.w_cbRenameSite, "w_cbRenameSite");
            this.w_cbRenameSite.Name = "w_cbRenameSite";
            this.w_cbRenameSite.Properties.AutoWidth = true;
            this.w_cbRenameSite.Properties.Caption = resources.GetString("w_cbRenameSite.Properties.Caption");
            this.w_cbRenameSite.CheckedChanged += new System.EventHandler(On_Rename_CheckedChanged);
            resources.ApplyResources(this.w_cbChangeTemplate, "w_cbChangeTemplate");
            this.w_cbChangeTemplate.Name = "w_cbChangeTemplate";
            this.w_cbChangeTemplate.Properties.AutoWidth = true;
            this.w_cbChangeTemplate.Properties.Caption = resources.GetString("w_cbChangeTemplate.Properties.Caption");
            this.w_cbChangeTemplate.CheckedChanged += new System.EventHandler(On_ChangeTemplate_Clicked);
            this.w_helpCopySubsites.AnchoringControl = this.w_cbCopySubsites;
            this.w_helpCopySubsites.BackColor = System.Drawing.Color.Transparent;
            this.w_helpCopySubsites.CommonParentControl = null;
            resources.ApplyResources(this.w_helpCopySubsites, "w_helpCopySubsites");
            this.w_helpCopySubsites.Name = "w_helpCopySubsites";
            this.w_helpCopySubsites.TabStop = false;
            resources.ApplyResources(this.w_cbCopyUncustomizedFiles, "w_cbCopyUncustomizedFiles");
            this.w_cbCopyUncustomizedFiles.Name = "w_cbCopyUncustomizedFiles";
            this.w_cbCopyUncustomizedFiles.Properties.Caption = resources.GetString("w_cbCopyUncustomizedFiles.Properties.Caption");
            base.Controls.Add(this.w_cbCopyUncustomizedFiles);
            base.Controls.Add(this.w_helpCopySubsites);
            base.Controls.Add(this._plThemes);
            base.Controls.Add(this.w_plUIVersion);
            base.Controls.Add(this.w_cbCopyCustomContent);
            base.Controls.Add(this.w_plMasterPage);
            base.Controls.Add(this.w_plCopyPortalListings);
            base.Controls.Add(this.w_plNavStructCopy);
            base.Controls.Add(this.w_cbNavigation);
            base.Controls.Add(this.w_cbApplyTheme);
            base.Controls.Add(this.w_cbCopySubsites);
            base.Controls.Add(this.w_plFeatures);
            base.Controls.Add(this.w_cbSiteFeatures);
            base.Controls.Add(this.w_cbContentTypes);
            base.Controls.Add(this.w_plSiteScoped);
            base.Name = "TCSiteContentOptions";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)this._plThemes).EndInit();
            this._plThemes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._rbAllThemes.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._rbCurrentTheme.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpApplyTheme).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbApplyTheme.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plUIVersion).EndInit();
            this.w_plUIVersion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbPreserveUIVersion.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyCustomContent.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plMasterPage).EndInit();
            this.w_plMasterPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbPreserveMasterPage.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plCopyPortalListings).EndInit();
            this.w_plCopyPortalListings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyPortalListings.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plNavStructCopy).EndInit();
            this.w_plNavStructCopy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyTopNavBar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTipCopyOnlyQuickLaunch).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyQuickLaunch.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbCopyBothQuickLaunchAndTopNav.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRunNavigationStructureCopy.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbNavigation.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopySubsites.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plFeatures).EndInit();
            this.w_plFeatures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_helpPreserveOnTarget).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbPreserveOnTarget.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpClearTargetFeatures).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_rbClearTargetFeatures.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbSiteFeatures.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbContentTypes.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSiteScoped).EndInit();
            this.w_plSiteScoped.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cmbTemplateName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSingleSitePanel).EndInit();
            this.w_plSingleSitePanel.ResumeLayout(false);
            this.w_plSingleSitePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._teNewSiteTitle.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._teNewSiteName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRenameSite.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbChangeTemplate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_helpCopySubsites).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyUncustomizedFiles.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void LoadThemeUI()
        {
            w_cbApplyTheme.Checked = Options.ApplyThemeToWeb;
            if (base.SourceAdapter == null || base.TargetAdapter == null || !base.SourceAdapter.SharePointVersion.IsSharePoint2013OrLater || !base.TargetAdapter.SharePointVersion.IsSharePoint2013OrLater)
            {
                HideControl(_plThemes);
                return;
            }
            bool @checked = w_cbApplyTheme.Checked;
            _rbAllThemes.Enabled = @checked;
            _rbCurrentTheme.Enabled = @checked;
            _rbAllThemes.Checked = Options.CopyAllThemes;
            _rbCurrentTheme.Checked = !Options.CopyAllThemes;
        }

        protected override void LoadUI()
        {
            if (base.ActionType != null && base.ActionType == typeof(PasteSiteContentAction))
            {
                HideControl(w_plSiteScoped);
            }
            w_cbChangeTemplate.Checked = Options.ChangeWebTemplate;
            m_sTemplateName = Options.WebTemplateName;
            w_cbContentTypes.Checked = Options.CopyContentTypes;
            w_cbCopySubsites.Checked = Options.RecursivelyCopySubsites;
            w_cbNavigation.Checked = Options.CopyNavigation;
            w_cbSiteFeatures.Checked = Options.CopySiteFeatures;
            w_rbClearTargetFeatures.Checked = !Options.MergeSiteFeatures;
            w_rbPreserveOnTarget.Checked = Options.MergeSiteFeatures;
            if (base.SourceAdapter.SharePointVersion.IsSharePoint2003 && base.TargetAdapter.SharePointVersion.IsSharePoint2010OrLater)
            {
                w_rbPreserveOnTarget.Checked = true;
                w_rbClearTargetFeatures.Checked = false;
                w_rbClearTargetFeatures.Enabled = false;
            }
            w_cbRenameSite.Checked = Options.RenameSite;
            m_sNewSiteName = Options.NewSiteName;
            m_sNewSiteTitle = Options.NewSiteTitle;
            w_cbPreserveMasterPage.Checked = Options.PreserveMasterPage;
            LoadThemeUI();
            if (base.TargetAdapter != null && base.TargetAdapter.SharePointVersion.IsSharePoint2013OrLater)
            {
                Options.PreserveUIVersion = false;
                w_cbPreserveUIVersion.Checked = false;
                w_cbPreserveUIVersion.Visible = false;
            }
            else if (base.SourceWeb == null || !(base.SourceWeb.UIVersion == "3") || base.TargetAdapter == null || !base.TargetAdapter.SharePointVersion.IsSharePoint2010)
            {
                w_plUIVersion.Enabled = false;
                w_cbPreserveUIVersion.Checked = false;
                if (base.TargetAdapter != null && !base.TargetAdapter.SharePointVersion.IsSharePoint2010)
                {
                    HideControl(w_plUIVersion);
                }
            }
            else
            {
                w_cbPreserveUIVersion.Checked = Options.PreserveUIVersion;
            }
            w_cbRunNavigationStructureCopy.Checked = Options.RunNavigationStructureCopy && Options.CopyNavigation;
            if (base.SourceAdapter != null && base.SourceAdapter.SharePointVersion.IsSharePoint2003)
            {
                w_rbCopyOnlyQuickLaunch.Checked = true;
            }
            else if (!Options.CopyCurrentNavigation)
            {
                w_rbCopyOnlyTopNavBar.Checked = true;
            }
            else if (Options.CopyGlobalNavigation)
            {
                w_rbCopyBothQuickLaunchAndTopNav.Checked = true;
            }
            else
            {
                w_rbCopyOnlyQuickLaunch.Checked = true;
            }
            if (base.SourceAdapter == null || base.SourceAdapter.SharePointVersion.IsSharePoint2003)
            {
                w_cbPreserveMasterPage.Enabled = false;
                HideControl(w_plMasterPage);
            }
            if (base.SourceAdapter == null || !base.SourceAdapter.IsPortal2003Connection)
            {
                w_cbCopyPortalListings.Enabled = false;
                HideControl(w_plCopyPortalListings);
            }
            w_cbCopyPortalListings.Checked = Options.CopyPortalListings;
            w_cbCopyCustomContent.Checked = Options.CopyCustomContent;
            w_cbCopyUncustomizedFiles.Checked = Options.CopyUncustomizedFiles;
            bool flag = base.FirstTargetNode != null && base.FirstTargetNode is SPTenant;
            bool flag2 = !(base.ActionType == null) && base.ActionType == typeof(PasteMySitesAction);
            if (flag2 && flag)
            {
                w_rbClearTargetFeatures.Checked = false;
                w_rbClearTargetFeatures.Enabled = false;
                w_rbPreserveOnTarget.Checked = true;
            }
            UpdateUI();
            if (base.SourceAdapter.SharePointVersion.IsSharePointOnline && base.TargetAdapter.SharePointVersion.IsSharePointOnline && flag2)
            {
                DisableNavigationOptions();
            }
            DisableOnlyQuickLaunchAndOnlyTopLinkOption();
        }

        protected override void MultiSelectUISetup()
        {
            if (base.MultiSelectUI)
            {
                base.Scope = SharePointObjectScope.Site;
                HideControl(w_plSingleSitePanel);
            }
        }

        private void On_ChangeTemplate_Clicked(object sender, EventArgs e)
        {
            w_cmbTemplateName.Enabled = w_cbChangeTemplate.Checked;
        }

        private void On_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
            SendMessage("CopyNavigationLinksAndHeadingsChanged", w_cbRunNavigationStructureCopy.Checked && w_cbRunNavigationStructureCopy.Enabled);
        }

        private void On_CopyCustomContentChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
            SendMessage("CopyCustContentChanged", w_cbCopyCustomContent.Checked);
        }

        private void On_CopyPortalListings_CheckedChanged(object sender, EventArgs e)
        {
            SendMessage("CopyPortalListingsChanged", w_cbCopyPortalListings.Checked && w_cbCopyPortalListings.Visible);
        }

        private void On_CopyPortalListings_VisibleChanged(object sender, EventArgs e)
        {
            SendMessage("CopyPortalListingsChanged", w_cbCopyPortalListings.Checked && w_cbCopyPortalListings.Visible);
        }

        private void On_CopySiteFeatures_Checked(object sender, EventArgs e)
        {
            w_plFeatures.Enabled = w_cbSiteFeatures.Checked;
        }

        private void On_CopySubSitesChanged(object sender, EventArgs e)
        {
            SendMessage("CopySubSitesChanged", w_cbCopySubsites.Checked);
        }

        private void On_Rename_CheckedChanged(object sender, EventArgs e)
        {
            _teNewSiteName.Enabled = w_cbRenameSite.Checked;
            _teNewSiteTitle.Enabled = w_cbRenameSite.Checked;
            if (!w_cbRenameSite.Checked)
            {
                m_sNewSiteName = null;
                m_sNewSiteTitle = null;
            }
            else
            {
                m_sNewSiteName = (string.IsNullOrEmpty(m_sNewSiteName) ? base.SourceWeb.WebName : NewSiteName);
                m_sNewSiteTitle = (string.IsNullOrEmpty(m_sNewSiteTitle) ? base.SourceWeb.Title : NewSiteTitle);
            }
            FireSiteRenameStateChanged();
        }

        private void On_SelectedTemplate_Changed(object sender, EventArgs e)
        {
            m_sTemplateName = ((SPWebTemplate)w_cmbTemplateName.SelectedItem).Name;
        }

        public override bool SaveUI()
        {
            Options.ChangeWebTemplate = w_cbChangeTemplate.Checked && w_cbChangeTemplate.Enabled;
            Options.WebTemplateName = m_sTemplateName;
            Options.CopyContentTypes = w_cbContentTypes.Checked;
            Options.RecursivelyCopySubsites = w_cbCopySubsites.Checked;
            Options.RenameSite = w_cbRenameSite.Checked;
            Options.NewSiteName = m_sNewSiteName;
            Options.NewSiteTitle = m_sNewSiteTitle;
            Options.CopyNavigation = w_cbNavigation.Checked;
            Options.CopySiteFeatures = w_cbSiteFeatures.Checked;
            Options.MergeSiteFeatures = w_rbPreserveOnTarget.Checked;
            Options.ApplyThemeToWeb = w_cbApplyTheme.Checked;
            Options.CopyAllThemes = _plThemes.Enabled && _rbAllThemes.Checked;
            Options.PreserveMasterPage = w_cbPreserveMasterPage.Checked && w_cbPreserveMasterPage.Enabled;
            Options.PreserveUIVersion = w_cbPreserveUIVersion.Checked;
            Options.RunNavigationStructureCopy = w_cbRunNavigationStructureCopy.Checked && w_cbRunNavigationStructureCopy.Enabled;
            Options.CopyGlobalNavigation = !w_rbCopyOnlyQuickLaunch.Checked;
            Options.CopyCurrentNavigation = !w_rbCopyOnlyTopNavBar.Checked;
            Options.CopyPortalListings = w_cbCopyPortalListings.Checked;
            Options.CopyCustomContent = w_cbCopyCustomContent.Checked;
            Options.CopyUncustomizedFiles = w_cbCopyUncustomizedFiles.Checked;
            return true;
        }

        private void SiteRenamingChanged(string sNewName, string sNewTitle)
        {
            m_sNewSiteName = sNewName;
            m_sNewSiteTitle = sNewTitle;
            UpdateRenameUI();
        }

        protected override void UpdateEnabledState()
        {
            w_cbSiteFeatures.Enabled = base.TargetIsOMAdapter;
            w_cbSiteFeatures.Checked = w_cbSiteFeatures.Checked && w_cbSiteFeatures.Enabled;
            w_plFeatures.Enabled = w_cbSiteFeatures.Checked;
            w_cmbTemplateName.Enabled = w_cbChangeTemplate.Checked;
            _teNewSiteName.Enabled = w_cbRenameSite.Checked;
            _teNewSiteTitle.Enabled = w_cbRenameSite.Checked;
            w_cbRunNavigationStructureCopy.Enabled = w_cbNavigation.Checked && w_cbNavigation.Enabled;
            CheckEdit checkEdit = w_rbCopyBothQuickLaunchAndTopNav;
            bool enabled = w_cbRunNavigationStructureCopy.Checked && w_cbRunNavigationStructureCopy.Enabled && (base.SourceAdapter == null || base.SourceAdapter.SharePointVersion.IsSharePoint2007OrLater);
            checkEdit.Enabled = enabled;
            w_rbCopyOnlyQuickLaunch.Enabled = w_rbCopyBothQuickLaunchAndTopNav.Enabled;
            w_rbCopyOnlyTopNavBar.Enabled = w_rbCopyBothQuickLaunchAndTopNav.Enabled;
            w_cbCopyCustomContent.Enabled = base.Scope == SharePointObjectScope.Server || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site;
            bool @checked = w_cbApplyTheme.Checked;
            _plThemes.Enabled = @checked;
            _rbAllThemes.Enabled = @checked;
            _rbCurrentTheme.Enabled = @checked;
            w_cbCopyUncustomizedFiles.Enabled = w_cbCopyCustomContent.Checked;
            DisableOnlyQuickLaunchAndOnlyTopLinkOption();
        }

        private void UpdateRenameUI()
        {
            m_bSuspendRenameEvents = true;
            w_cbRenameSite.Checked = NewSiteName != null || NewSiteTitle != null;
            TextEdit teNewSiteName = _teNewSiteName;
            string text = ((!string.IsNullOrEmpty(NewSiteName)) ? NewSiteName : ((base.SourceWeb != null) ? base.SourceWeb.WebName : string.Empty));
            teNewSiteName.Text = text;
            TextEdit teNewSiteTitle = _teNewSiteTitle;
            string text2 = ((!string.IsNullOrEmpty(NewSiteTitle)) ? NewSiteTitle : ((base.SourceWeb != null) ? base.SourceWeb.Title : string.Empty));
            teNewSiteTitle.Text = text2;
            m_bSuspendRenameEvents = false;
        }

        protected override void UpdateScope()
        {
            if (base.Scope == SharePointObjectScope.SiteCollection)
            {
                HideControl(w_plSiteScoped);
            }
        }

        private void UpdateTemplateUI()
        {
            bool flag = false;
            w_cmbTemplateName.Properties.Items.Clear();
            if (TargetWebTemplates == null)
            {
                return;
            }
            IEnumerable<SPWebTemplate> source = TargetWebTemplates.Where(SiteOnlyTemplates.Compile());
            if (base.SourceWeb != null)
            {
                if (RestrictedSiteTemplates.Contains(base.SourceWeb.Template.Name))
                {
                    IEnumerable<string> restrictedSiteTemplates = RestrictedSiteTemplates.Where((string item) => !item.Contains(base.SourceWeb.Template.Name.Split('#')[0]));
                    source = source.Where((SPWebTemplate template) => restrictedSiteTemplates.All((string item) => template.Name != item));
                }
                else
                {
                    source = source.Where((SPWebTemplate template) => RestrictedSiteTemplates.All((string item) => template.Name != item));
                }
            }
            source = source.OrderBy((SPWebTemplate template) => template.Title);
            foreach (SPWebTemplate item in source)
            {
                w_cmbTemplateName.Properties.Items.Add(item);
                if (!(item.Name != m_sTemplateName))
                {
                    w_cmbTemplateName.SelectedItem = item;
                    flag = true;
                }
            }
            if (!flag && w_cmbTemplateName.Properties.Items.Count > 0)
            {
                w_cmbTemplateName.SelectedIndex = 0;
            }
        }

        private void UpdateUI()
        {
            UpdateTemplateUI();
            UpdateRenameUI();
            UpdateEnabledState();
        }
    }
}