using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Utilities;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCSiteCollectionOptionsBasicView : SiteCollectionOptionsScopableTabbableControl
    {
        private string _webAppName;

        private string _templateName;

        private SPWebApplication _selectedWebApplication;

        private SPLanguage _selectedLanguage;

        private string _selectedExperienceVersion = string.Empty;

        private Dictionary<string, int> _experienceLableToVersionMap = new Dictionary<string, int>();

        private SPSiteCollectionOptions _options;

        private string _sourceUrl = string.Empty;

        private bool _isTargetSharePointOnline;

        private IContainer components;

        private TableLayoutPanel tlpSiteCollectionOptions;

        private PanelControl pnlWebApplication;

        private LabelControl lblWebApplication;

        private ComboBoxEdit cmbWebApplication;

        private LayoutControl lcAdvancedOptions;

        private GroupControl gcAdvancedOptions;

        private TableLayoutPanel tlpAdvancedOptions;

        private PanelControl pnlTemplate;

        private LabelControl lblTemplate;

        private LayoutControlGroup lcgAdvancedOptions;

        private EmptySpaceItem esAdvancedOptions;

        private LayoutControlGroup lcgAdvanced;

        private LayoutControlItem lciAdvancedOptions;

        private PanelControl pnlAdvancedOptions;

        private ComboBoxEdit cmbTemplate;

        private PanelControl pnlQuota;

        private LabelControl lblStorageQuota;

        private LabelControl lblServerResourceQuota;

        private SpinEdit spinResourceQuota;

        private SpinEdit spinStorageQuota;

        private ToggleSwitch tsCopySiteCollectionAdmin;

        private ToggleSwitch tsCopyListTemplateGallery;

        private ToggleSwitch tsCopyMasterPageGallery;

        private PanelControl pnlSetSiteQuota;

        private ToggleSwitch tsSetSiteQuota;

        private TextEdit tESiteCollectionAdministrator;

        private LabelControl lblSiteCollectionAdministrator;

        private TextEdit tETargetSiteUrl;

        private ComboBoxEdit cmbPath;

        private LabelControl lblSiteUrl;

        private LabelControl lblSiteCollectionURL;

        private ToggleSwitch tsCopyAuditSettings;

        private HelpTipButton helpMasterPageGallery;

        private HelpTipButton helpCopyAuditSettings;

        private LabelControl lblMb;

        private HelpTipButton helpSiteCollectionAdministrator;

        private HelpTipButton helpSiteCollectionURL;

        private HelpTipButton helpWebApplication;

        private HelpTipButton helpCopySiteCollectionAdmin;

        private HelpTipButton helpCopyListTemplateGallery;

        private HelpTipButton helpTemplate;

        private HelpTipButton helpSetSiteQuota;

        public bool IsTargetSharePointOnline
        {
            set
            {
                _isTargetSharePointOnline = value;
                if (_isTargetSharePointOnline)
                {
                    pnlSetSiteQuota.Hide();
                    lcAdvancedOptions.Height -= 47;
                }
                else
                {
                    pnlQuota.Hide();
                    lcAdvancedOptions.Height -= 78;
                }
            }
        }

        public SPSiteCollectionOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                if (value != null)
                {
                    LoadUI();
                }
            }
        }

        public string SourceUrl
        {
            get
            {
                return _sourceUrl;
            }
            set
            {
                _sourceUrl = value;
                if (_sourceUrl != null)
                {
                    tETargetSiteUrl.Text = _sourceUrl;
                }
            }
        }

        public event TCMigrationOptionsBasicView.AdvancedOptionsMainControlStateChangedHandler AdvancedOptionsMainControlStateChanged;

        public TCSiteCollectionOptionsBasicView()
        {
            InitializeComponent();
            Type type = GetType();
            helpMasterPageGallery.SetResourceString(type.FullName + tsCopyMasterPageGallery.Name, type);
            helpMasterPageGallery.IsBasicViewHelpIcon = true;
            helpCopyAuditSettings.SetResourceString(type.FullName + tsCopyAuditSettings.Name, type);
            helpCopyAuditSettings.IsBasicViewHelpIcon = true;
            helpCopyListTemplateGallery.SetResourceString(type.FullName + tsCopyListTemplateGallery.Name, type);
            helpCopyListTemplateGallery.IsBasicViewHelpIcon = true;
            helpCopySiteCollectionAdmin.SetResourceString(type.FullName + tsCopySiteCollectionAdmin.Name, type);
            helpCopySiteCollectionAdmin.IsBasicViewHelpIcon = true;
            helpSetSiteQuota.SetResourceString(type.FullName + tsSetSiteQuota.Name, type);
            helpSetSiteQuota.IsBasicViewHelpIcon = true;
            helpTemplate.SetResourceString(type.FullName + cmbTemplate.Name, type);
            helpTemplate.IsBasicViewHelpIcon = true;
            helpSiteCollectionAdministrator.SetResourceString(type.FullName + tESiteCollectionAdministrator.Name, type);
            helpSiteCollectionAdministrator.IsBasicViewHelpIcon = true;
            helpWebApplication.SetResourceString(type.FullName + cmbWebApplication.Name, type);
            helpWebApplication.IsBasicViewHelpIcon = true;
            helpSiteCollectionURL.SetResourceString(type.FullName + tETargetSiteUrl.Name, type);
            helpSiteCollectionURL.IsBasicViewHelpIcon = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private ArrayList GetAdvanceOptionsSummary()
        {
            ArrayList arrayList = new ArrayList();
            string text = lblTemplate.Text;
            char[] trimChars = new char[1] { ':' };
            arrayList.Add(new OptionsSummary($"{text.TrimEnd(trimChars)} : {cmbTemplate.Text}", 1));
            arrayList.Add(new OptionsSummary($"{tsCopyMasterPageGallery.Properties.OnText} : {tsCopyMasterPageGallery.IsOn}", 1));
            ArrayList arrayList2 = arrayList;
            if (tsCopyListTemplateGallery.Enabled)
            {
                arrayList2.Add(new OptionsSummary($"{tsCopyListTemplateGallery.Properties.OnText} : {tsCopyListTemplateGallery.IsOn}", 1));
            }
            arrayList2.Add(new OptionsSummary($"{tsCopySiteCollectionAdmin.Properties.OnText} : {tsCopySiteCollectionAdmin.IsOn}", 1));
            arrayList2.Add(new OptionsSummary($"{tsCopyAuditSettings.Properties.OnText} : {tsCopyAuditSettings.IsOn}", 1));
            if (!(base.Target is SPTenant))
            {
                arrayList2.Add(new OptionsSummary($"{tsSetSiteQuota.Properties.OnText} : {tsSetSiteQuota.IsOn}", 1));
            }
            else
            {
                string text2 = lblStorageQuota.Text;
                char[] trimChars2 = new char[1] { ':' };
                arrayList2.Add(new OptionsSummary($"{text2.TrimEnd(trimChars2)} : {spinStorageQuota.Text}", 1));
                string text3 = lblServerResourceQuota.Text;
                char[] trimChars3 = new char[1] { ':' };
                arrayList2.Add(new OptionsSummary($"{text3.TrimEnd(trimChars3)} : {spinResourceQuota.Text}", 1));
            }
            return arrayList2;
        }

        private string GetSiteUrl()
        {
            return ((lblSiteUrl.ToolTip == string.Empty) ? lblSiteUrl.Text : lblSiteUrl.ToolTip).Replace("\n", string.Empty);
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            string siteUrl = GetSiteUrl();
            string arg = (Options.IsHostHeader ? $"{siteUrl}{tETargetSiteUrl.Text}" : $"{siteUrl}{cmbPath.Text}{tETargetSiteUrl.Text}");
            string arg2 = (string.IsNullOrEmpty(tESiteCollectionAdministrator.Text) ? "  -" : tESiteCollectionAdministrator.Text);
            arrayList.Add(new OptionsSummary("Site Collection Options", 0));
            string text = lblWebApplication.Text;
            char[] trimChars = new char[1] { ':' };
            arrayList.Add(new OptionsSummary($"{text.TrimEnd(trimChars)} : {cmbWebApplication.Text}", 1));
            string text2 = lblSiteCollectionURL.Text;
            char[] trimChars2 = new char[1] { ':' };
            arrayList.Add(new OptionsSummary($"{text2.TrimEnd(trimChars2)} : {arg}", 1));
            string text3 = lblSiteCollectionAdministrator.Text;
            char[] trimChars3 = new char[1] { ':' };
            arrayList.Add(new OptionsSummary($"{text3.TrimEnd(trimChars3)} : {arg2}", 1));
            arrayList.AddRange(GetAdvanceOptionsSummary());
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCSiteCollectionOptionsBasicView));
            this.tlpSiteCollectionOptions = new System.Windows.Forms.TableLayoutPanel();
            this.pnlWebApplication = new DevExpress.XtraEditors.PanelControl();
            this.helpSiteCollectionAdministrator = new TooltipsTest.HelpTipButton();
            this.helpSiteCollectionURL = new TooltipsTest.HelpTipButton();
            this.helpWebApplication = new TooltipsTest.HelpTipButton();
            this.tESiteCollectionAdministrator = new DevExpress.XtraEditors.TextEdit();
            this.lblSiteCollectionAdministrator = new DevExpress.XtraEditors.LabelControl();
            this.tETargetSiteUrl = new DevExpress.XtraEditors.TextEdit();
            this.cmbPath = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblSiteUrl = new DevExpress.XtraEditors.LabelControl();
            this.lblSiteCollectionURL = new DevExpress.XtraEditors.LabelControl();
            this.cmbWebApplication = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblWebApplication = new DevExpress.XtraEditors.LabelControl();
            this.pnlAdvancedOptions = new DevExpress.XtraEditors.PanelControl();
            this.lcAdvancedOptions = new DevExpress.XtraLayout.LayoutControl();
            this.gcAdvancedOptions = new DevExpress.XtraEditors.GroupControl();
            this.tlpAdvancedOptions = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTemplate = new DevExpress.XtraEditors.PanelControl();
            this.helpCopySiteCollectionAdmin = new TooltipsTest.HelpTipButton();
            this.helpCopyListTemplateGallery = new TooltipsTest.HelpTipButton();
            this.helpTemplate = new TooltipsTest.HelpTipButton();
            this.helpCopyAuditSettings = new TooltipsTest.HelpTipButton();
            this.helpMasterPageGallery = new TooltipsTest.HelpTipButton();
            this.tsCopyAuditSettings = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopySiteCollectionAdmin = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyListTemplateGallery = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyMasterPageGallery = new DevExpress.XtraEditors.ToggleSwitch();
            this.cmbTemplate = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblTemplate = new DevExpress.XtraEditors.LabelControl();
            this.pnlQuota = new DevExpress.XtraEditors.PanelControl();
            this.lblMb = new DevExpress.XtraEditors.LabelControl();
            this.spinResourceQuota = new DevExpress.XtraEditors.SpinEdit();
            this.spinStorageQuota = new DevExpress.XtraEditors.SpinEdit();
            this.lblServerResourceQuota = new DevExpress.XtraEditors.LabelControl();
            this.lblStorageQuota = new DevExpress.XtraEditors.LabelControl();
            this.pnlSetSiteQuota = new DevExpress.XtraEditors.PanelControl();
            this.helpSetSiteQuota = new TooltipsTest.HelpTipButton();
            this.tsSetSiteQuota = new DevExpress.XtraEditors.ToggleSwitch();
            this.lcgAdvancedOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.esAdvancedOptions = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcgAdvanced = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAdvancedOptions = new DevExpress.XtraLayout.LayoutControlItem();
            this.tlpSiteCollectionOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlWebApplication).BeginInit();
            this.pnlWebApplication.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpSiteCollectionAdministrator).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpSiteCollectionURL).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpWebApplication).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tESiteCollectionAdministrator.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tETargetSiteUrl.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbPath.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbWebApplication.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).BeginInit();
            this.pnlAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedOptions).BeginInit();
            this.lcAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gcAdvancedOptions).BeginInit();
            this.gcAdvancedOptions.SuspendLayout();
            this.tlpAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlTemplate).BeginInit();
            this.pnlTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpCopySiteCollectionAdmin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListTemplateGallery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTemplate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyAuditSettings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMasterPageGallery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyAuditSettings.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySiteCollectionAdmin.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListTemplateGallery.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyMasterPageGallery.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbTemplate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlQuota).BeginInit();
            this.pnlQuota.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.spinResourceQuota.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.spinStorageQuota.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlSetSiteQuota).BeginInit();
            this.pnlSetSiteQuota.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpSetSiteQuota).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsSetSiteQuota.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvanced).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).BeginInit();
            base.SuspendLayout();
            this.tlpSiteCollectionOptions.ColumnCount = 1;
            this.tlpSiteCollectionOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpSiteCollectionOptions.Controls.Add(this.pnlWebApplication, 0, 0);
            this.tlpSiteCollectionOptions.Controls.Add(this.pnlAdvancedOptions, 0, 1);
            this.tlpSiteCollectionOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSiteCollectionOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpSiteCollectionOptions.Name = "tlpSiteCollectionOptions";
            this.tlpSiteCollectionOptions.RowCount = 2;
            this.tlpSiteCollectionOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSiteCollectionOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSiteCollectionOptions.Size = new System.Drawing.Size(623, 467);
            this.tlpSiteCollectionOptions.TabIndex = 0;
            this.pnlWebApplication.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlWebApplication.Controls.Add(this.helpSiteCollectionAdministrator);
            this.pnlWebApplication.Controls.Add(this.helpSiteCollectionURL);
            this.pnlWebApplication.Controls.Add(this.helpWebApplication);
            this.pnlWebApplication.Controls.Add(this.tESiteCollectionAdministrator);
            this.pnlWebApplication.Controls.Add(this.lblSiteCollectionAdministrator);
            this.pnlWebApplication.Controls.Add(this.tETargetSiteUrl);
            this.pnlWebApplication.Controls.Add(this.cmbPath);
            this.pnlWebApplication.Controls.Add(this.lblSiteUrl);
            this.pnlWebApplication.Controls.Add(this.lblSiteCollectionURL);
            this.pnlWebApplication.Controls.Add(this.cmbWebApplication);
            this.pnlWebApplication.Controls.Add(this.lblWebApplication);
            this.pnlWebApplication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWebApplication.Location = new System.Drawing.Point(3, 3);
            this.pnlWebApplication.Name = "pnlWebApplication";
            this.pnlWebApplication.Size = new System.Drawing.Size(617, 94);
            this.pnlWebApplication.TabIndex = 0;
            this.helpSiteCollectionAdministrator.AnchoringControl = null;
            this.helpSiteCollectionAdministrator.BackColor = System.Drawing.Color.Transparent;
            this.helpSiteCollectionAdministrator.CommonParentControl = null;
            this.helpSiteCollectionAdministrator.Image = (System.Drawing.Image)resources.GetObject("helpSiteCollectionAdministrator.Image");
            this.helpSiteCollectionAdministrator.Location = new System.Drawing.Point(577, 69);
            this.helpSiteCollectionAdministrator.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionAdministrator.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionAdministrator.Name = "helpSiteCollectionAdministrator";
            this.helpSiteCollectionAdministrator.RealOffset = null;
            this.helpSiteCollectionAdministrator.RelativeOffset = null;
            this.helpSiteCollectionAdministrator.Size = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionAdministrator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpSiteCollectionAdministrator.TabIndex = 10;
            this.helpSiteCollectionAdministrator.TabStop = false;
            this.helpSiteCollectionURL.AnchoringControl = null;
            this.helpSiteCollectionURL.BackColor = System.Drawing.Color.Transparent;
            this.helpSiteCollectionURL.CommonParentControl = null;
            this.helpSiteCollectionURL.Image = (System.Drawing.Image)resources.GetObject("helpSiteCollectionURL.Image");
            this.helpSiteCollectionURL.Location = new System.Drawing.Point(578, 39);
            this.helpSiteCollectionURL.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionURL.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionURL.Name = "helpSiteCollectionURL";
            this.helpSiteCollectionURL.RealOffset = null;
            this.helpSiteCollectionURL.RelativeOffset = null;
            this.helpSiteCollectionURL.Size = new System.Drawing.Size(20, 20);
            this.helpSiteCollectionURL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpSiteCollectionURL.TabIndex = 9;
            this.helpSiteCollectionURL.TabStop = false;
            this.helpWebApplication.AnchoringControl = null;
            this.helpWebApplication.BackColor = System.Drawing.Color.Transparent;
            this.helpWebApplication.CommonParentControl = null;
            this.helpWebApplication.Image = (System.Drawing.Image)resources.GetObject("helpWebApplication.Image");
            this.helpWebApplication.Location = new System.Drawing.Point(578, 9);
            this.helpWebApplication.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpWebApplication.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpWebApplication.Name = "helpWebApplication";
            this.helpWebApplication.RealOffset = null;
            this.helpWebApplication.RelativeOffset = null;
            this.helpWebApplication.Size = new System.Drawing.Size(20, 20);
            this.helpWebApplication.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpWebApplication.TabIndex = 8;
            this.helpWebApplication.TabStop = false;
            this.tESiteCollectionAdministrator.Location = new System.Drawing.Point(159, 69);
            this.tESiteCollectionAdministrator.Name = "tESiteCollectionAdministrator";
            this.tESiteCollectionAdministrator.Size = new System.Drawing.Size(413, 20);
            this.tESiteCollectionAdministrator.TabIndex = 7;
            this.lblSiteCollectionAdministrator.Location = new System.Drawing.Point(3, 71);
            this.lblSiteCollectionAdministrator.Name = "lblSiteCollectionAdministrator";
            this.lblSiteCollectionAdministrator.Size = new System.Drawing.Size(138, 13);
            this.lblSiteCollectionAdministrator.TabIndex = 6;
            this.lblSiteCollectionAdministrator.Text = "Site Collection Administrator:";
            this.tETargetSiteUrl.Location = new System.Drawing.Point(374, 39);
            this.tETargetSiteUrl.Name = "tETargetSiteUrl";
            this.tETargetSiteUrl.Size = new System.Drawing.Size(198, 20);
            this.tETargetSiteUrl.TabIndex = 5;
            this.cmbPath.Location = new System.Drawing.Point(301, 39);
            this.cmbPath.Name = "cmbPath";
            this.cmbPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.cmbPath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbPath.Size = new System.Drawing.Size(69, 20);
            this.cmbPath.TabIndex = 4;
            this.cmbPath.SelectedIndexChanged += new System.EventHandler(On_Path_Changed);
            this.lblSiteUrl.Location = new System.Drawing.Point(159, 41);
            this.lblSiteUrl.Name = "lblSiteUrl";
            this.lblSiteUrl.Size = new System.Drawing.Size(138, 13);
            this.lblSiteUrl.TabIndex = 3;
            this.lblSiteUrl.Text = "http://wellington_10:175268";
            this.lblSiteCollectionURL.Location = new System.Drawing.Point(3, 41);
            this.lblSiteCollectionURL.Name = "lblSiteCollectionURL";
            this.lblSiteCollectionURL.Size = new System.Drawing.Size(73, 13);
            this.lblSiteCollectionURL.TabIndex = 2;
            this.lblSiteCollectionURL.Text = "Target Site Url:";
            this.cmbWebApplication.Location = new System.Drawing.Point(159, 9);
            this.cmbWebApplication.Name = "cmbWebApplication";
            this.cmbWebApplication.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.cmbWebApplication.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbWebApplication.Size = new System.Drawing.Size(413, 20);
            this.cmbWebApplication.TabIndex = 1;
            this.cmbWebApplication.SelectedIndexChanged += new System.EventHandler(On_WebApp_Changed);
            this.lblWebApplication.Location = new System.Drawing.Point(3, 11);
            this.lblWebApplication.Name = "lblWebApplication";
            this.lblWebApplication.Size = new System.Drawing.Size(116, 13);
            this.lblWebApplication.TabIndex = 0;
            this.lblWebApplication.Text = "Target Web Application:";
            this.pnlAdvancedOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlAdvancedOptions.Controls.Add(this.lcAdvancedOptions);
            this.pnlAdvancedOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAdvancedOptions.Location = new System.Drawing.Point(3, 103);
            this.pnlAdvancedOptions.Name = "pnlAdvancedOptions";
            this.pnlAdvancedOptions.Size = new System.Drawing.Size(617, 361);
            this.pnlAdvancedOptions.TabIndex = 0;
            this.lcAdvancedOptions.Controls.Add(this.gcAdvancedOptions);
            this.lcAdvancedOptions.Location = new System.Drawing.Point(148, -3);
            this.lcAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.lcAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lcAdvancedOptions.Name = "lcAdvancedOptions";
            this.lcAdvancedOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(582, 352, 338, 350);
            this.lcAdvancedOptions.OptionsFocus.EnableAutoTabOrder = false;
            this.lcAdvancedOptions.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcAdvancedOptions.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            this.lcAdvancedOptions.Root = this.lcgAdvancedOptions;
            this.lcAdvancedOptions.Size = new System.Drawing.Size(435, 359);
            this.lcAdvancedOptions.TabIndex = 0;
            this.lcAdvancedOptions.Text = "layoutControl1";
            this.lcAdvancedOptions.GroupExpandChanged += new DevExpress.XtraLayout.Utils.LayoutGroupEventHandler(lcAdvancedOptions_GroupExpandChanged);
            this.gcAdvancedOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gcAdvancedOptions.Controls.Add(this.tlpAdvancedOptions);
            this.gcAdvancedOptions.Location = new System.Drawing.Point(24, 44);
            this.gcAdvancedOptions.Name = "gcAdvancedOptions";
            this.gcAdvancedOptions.Size = new System.Drawing.Size(387, 278);
            this.gcAdvancedOptions.TabIndex = 0;
            this.gcAdvancedOptions.Text = "groupControl1";
            this.tlpAdvancedOptions.ColumnCount = 1;
            this.tlpAdvancedOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpAdvancedOptions.Controls.Add(this.pnlTemplate, 0, 0);
            this.tlpAdvancedOptions.Controls.Add(this.pnlQuota, 0, 2);
            this.tlpAdvancedOptions.Controls.Add(this.pnlSetSiteQuota, 0, 1);
            this.tlpAdvancedOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpAdvancedOptions.Name = "tlpAdvancedOptions";
            this.tlpAdvancedOptions.RowCount = 3;
            this.tlpAdvancedOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAdvancedOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAdvancedOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAdvancedOptions.Size = new System.Drawing.Size(387, 278);
            this.tlpAdvancedOptions.TabIndex = 0;
            this.pnlTemplate.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlTemplate.Controls.Add(this.helpCopySiteCollectionAdmin);
            this.pnlTemplate.Controls.Add(this.helpCopyListTemplateGallery);
            this.pnlTemplate.Controls.Add(this.helpTemplate);
            this.pnlTemplate.Controls.Add(this.helpCopyAuditSettings);
            this.pnlTemplate.Controls.Add(this.helpMasterPageGallery);
            this.pnlTemplate.Controls.Add(this.tsCopyAuditSettings);
            this.pnlTemplate.Controls.Add(this.tsCopySiteCollectionAdmin);
            this.pnlTemplate.Controls.Add(this.tsCopyListTemplateGallery);
            this.pnlTemplate.Controls.Add(this.tsCopyMasterPageGallery);
            this.pnlTemplate.Controls.Add(this.cmbTemplate);
            this.pnlTemplate.Controls.Add(this.lblTemplate);
            this.pnlTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTemplate.Location = new System.Drawing.Point(3, 3);
            this.pnlTemplate.Name = "pnlTemplate";
            this.pnlTemplate.Size = new System.Drawing.Size(381, 161);
            this.pnlTemplate.TabIndex = 0;
            this.helpCopySiteCollectionAdmin.AnchoringControl = null;
            this.helpCopySiteCollectionAdmin.BackColor = System.Drawing.Color.Transparent;
            this.helpCopySiteCollectionAdmin.CommonParentControl = null;
            this.helpCopySiteCollectionAdmin.Image = (System.Drawing.Image)resources.GetObject("helpCopySiteCollectionAdmin.Image");
            this.helpCopySiteCollectionAdmin.Location = new System.Drawing.Point(310, 106);
            this.helpCopySiteCollectionAdmin.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopySiteCollectionAdmin.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopySiteCollectionAdmin.Name = "helpCopySiteCollectionAdmin";
            this.helpCopySiteCollectionAdmin.RealOffset = null;
            this.helpCopySiteCollectionAdmin.RelativeOffset = null;
            this.helpCopySiteCollectionAdmin.Size = new System.Drawing.Size(20, 20);
            this.helpCopySiteCollectionAdmin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopySiteCollectionAdmin.TabIndex = 14;
            this.helpCopySiteCollectionAdmin.TabStop = false;
            this.helpCopyListTemplateGallery.AnchoringControl = null;
            this.helpCopyListTemplateGallery.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyListTemplateGallery.CommonParentControl = null;
            this.helpCopyListTemplateGallery.Image = (System.Drawing.Image)resources.GetObject("helpCopyListTemplateGallery.Image");
            this.helpCopyListTemplateGallery.Location = new System.Drawing.Point(221, 74);
            this.helpCopyListTemplateGallery.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListTemplateGallery.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListTemplateGallery.Name = "helpCopyListTemplateGallery";
            this.helpCopyListTemplateGallery.RealOffset = null;
            this.helpCopyListTemplateGallery.RelativeOffset = null;
            this.helpCopyListTemplateGallery.Size = new System.Drawing.Size(20, 20);
            this.helpCopyListTemplateGallery.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyListTemplateGallery.TabIndex = 13;
            this.helpCopyListTemplateGallery.TabStop = false;
            this.helpTemplate.AnchoringControl = null;
            this.helpTemplate.BackColor = System.Drawing.Color.Transparent;
            this.helpTemplate.CommonParentControl = null;
            this.helpTemplate.Image = (System.Drawing.Image)resources.GetObject("helpTemplate.Image");
            this.helpTemplate.Location = new System.Drawing.Point(361, 3);
            this.helpTemplate.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpTemplate.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpTemplate.Name = "helpTemplate";
            this.helpTemplate.RealOffset = null;
            this.helpTemplate.RelativeOffset = null;
            this.helpTemplate.Size = new System.Drawing.Size(20, 20);
            this.helpTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpTemplate.TabIndex = 12;
            this.helpTemplate.TabStop = false;
            this.helpCopyAuditSettings.AnchoringControl = null;
            this.helpCopyAuditSettings.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyAuditSettings.CommonParentControl = null;
            this.helpCopyAuditSettings.Image = (System.Drawing.Image)resources.GetObject("helpCopyAuditSettings.Image");
            this.helpCopyAuditSettings.Location = new System.Drawing.Point(191, 138);
            this.helpCopyAuditSettings.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyAuditSettings.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyAuditSettings.Name = "helpCopyAuditSettings";
            this.helpCopyAuditSettings.RealOffset = null;
            this.helpCopyAuditSettings.RelativeOffset = null;
            this.helpCopyAuditSettings.Size = new System.Drawing.Size(20, 20);
            this.helpCopyAuditSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyAuditSettings.TabIndex = 11;
            this.helpCopyAuditSettings.TabStop = false;
            this.helpMasterPageGallery.AnchoringControl = null;
            this.helpMasterPageGallery.BackColor = System.Drawing.Color.Transparent;
            this.helpMasterPageGallery.CommonParentControl = null;
            this.helpMasterPageGallery.Image = (System.Drawing.Image)resources.GetObject("helpMasterPageGallery.Image");
            this.helpMasterPageGallery.Location = new System.Drawing.Point(217, 42);
            this.helpMasterPageGallery.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpMasterPageGallery.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpMasterPageGallery.Name = "helpMasterPageGallery";
            this.helpMasterPageGallery.RealOffset = null;
            this.helpMasterPageGallery.RelativeOffset = null;
            this.helpMasterPageGallery.Size = new System.Drawing.Size(20, 20);
            this.helpMasterPageGallery.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpMasterPageGallery.TabIndex = 7;
            this.helpMasterPageGallery.TabStop = false;
            this.tsCopyAuditSettings.Location = new System.Drawing.Point(11, 135);
            this.tsCopyAuditSettings.Margin = new System.Windows.Forms.Padding(11, 10, 3, 3);
            this.tsCopyAuditSettings.Name = "tsCopyAuditSettings";
            this.tsCopyAuditSettings.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyAuditSettings.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyAuditSettings.Properties.OffText = "Copy Audit Settings";
            this.tsCopyAuditSettings.Properties.OnText = "Copy Audit Settings";
            this.tsCopyAuditSettings.Size = new System.Drawing.Size(183, 24);
            this.tsCopyAuditSettings.TabIndex = 5;
            this.tsCopySiteCollectionAdmin.Location = new System.Drawing.Point(11, 103);
            this.tsCopySiteCollectionAdmin.Margin = new System.Windows.Forms.Padding(11, 10, 3, 3);
            this.tsCopySiteCollectionAdmin.Name = "tsCopySiteCollectionAdmin";
            this.tsCopySiteCollectionAdmin.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopySiteCollectionAdmin.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopySiteCollectionAdmin.Properties.OffText = "Copy all Source Site Collection Administrators";
            this.tsCopySiteCollectionAdmin.Properties.OnText = "Copy all Source Site Collection Administrators";
            this.tsCopySiteCollectionAdmin.Size = new System.Drawing.Size(301, 24);
            this.tsCopySiteCollectionAdmin.TabIndex = 4;
            this.tsCopyListTemplateGallery.Location = new System.Drawing.Point(11, 71);
            this.tsCopyListTemplateGallery.Margin = new System.Windows.Forms.Padding(11, 10, 3, 3);
            this.tsCopyListTemplateGallery.Name = "tsCopyListTemplateGallery";
            this.tsCopyListTemplateGallery.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyListTemplateGallery.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyListTemplateGallery.Properties.OffText = "Copy List Template Gallery";
            this.tsCopyListTemplateGallery.Properties.OnText = "Copy List Template Gallery";
            this.tsCopyListTemplateGallery.Size = new System.Drawing.Size(213, 24);
            this.tsCopyListTemplateGallery.TabIndex = 3;
            this.tsCopyMasterPageGallery.Location = new System.Drawing.Point(11, 39);
            this.tsCopyMasterPageGallery.Margin = new System.Windows.Forms.Padding(11, 10, 3, 3);
            this.tsCopyMasterPageGallery.Name = "tsCopyMasterPageGallery";
            this.tsCopyMasterPageGallery.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyMasterPageGallery.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyMasterPageGallery.Properties.AppearanceDisabled.Options.UseTextOptions = true;
            this.tsCopyMasterPageGallery.Properties.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyMasterPageGallery.Properties.OffText = "Copy Master Page Gallery";
            this.tsCopyMasterPageGallery.Properties.OnText = "Copy Master Page Gallery";
            this.tsCopyMasterPageGallery.Size = new System.Drawing.Size(210, 24);
            this.tsCopyMasterPageGallery.TabIndex = 2;
            this.cmbTemplate.Location = new System.Drawing.Point(82, 3);
            this.cmbTemplate.Name = "cmbTemplate";
            this.cmbTemplate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.cmbTemplate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbTemplate.Size = new System.Drawing.Size(274, 20);
            this.cmbTemplate.TabIndex = 1;
            this.cmbTemplate.SelectedIndexChanged += new System.EventHandler(On_WebTemplate_Changed);
            this.lblTemplate.Location = new System.Drawing.Point(11, 4);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(48, 13);
            this.lblTemplate.TabIndex = 0;
            this.lblTemplate.Text = "Template:";
            this.pnlQuota.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlQuota.Controls.Add(this.lblMb);
            this.pnlQuota.Controls.Add(this.spinResourceQuota);
            this.pnlQuota.Controls.Add(this.spinStorageQuota);
            this.pnlQuota.Controls.Add(this.lblServerResourceQuota);
            this.pnlQuota.Controls.Add(this.lblStorageQuota);
            this.pnlQuota.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlQuota.Location = new System.Drawing.Point(3, 213);
            this.pnlQuota.Name = "pnlQuota";
            this.pnlQuota.Size = new System.Drawing.Size(381, 62);
            this.pnlQuota.TabIndex = 9;
            this.lblMb.Location = new System.Drawing.Point(278, 8);
            this.lblMb.Name = "lblMb";
            this.lblMb.Size = new System.Drawing.Size(14, 13);
            this.lblMb.TabIndex = 4;
            this.lblMb.Text = "Mb";
            this.spinResourceQuota.EditValue = new decimal(new int[4]);
            this.spinResourceQuota.Location = new System.Drawing.Point(144, 34);
            this.spinResourceQuota.Name = "spinResourceQuota";
            this.spinResourceQuota.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.spinResourceQuota.Properties;
            int[] bits = new int[4] { 10, 0, 0, 0 };
            properties.Increment = new decimal(bits);
            this.spinResourceQuota.Properties.IsFloatValue = false;
            this.spinResourceQuota.Properties.Mask.EditMask = "N00";
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.spinResourceQuota.Properties;
            int[] bits2 = new int[4] { 10000, 0, 0, 0 };
            properties2.MaxValue = new decimal(bits2);
            this.spinResourceQuota.Size = new System.Drawing.Size(125, 20);
            this.spinResourceQuota.TabIndex = 3;
            DevExpress.XtraEditors.SpinEdit spinEdit = this.spinStorageQuota;
            int[] bits3 = new int[4] { 110, 0, 0, 0 };
            spinEdit.EditValue = new decimal(bits3);
            this.spinStorageQuota.Location = new System.Drawing.Point(144, 4);
            this.spinStorageQuota.Name = "spinStorageQuota";
            this.spinStorageQuota.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties3 = this.spinStorageQuota.Properties;
            int[] bits4 = new int[4] { 10, 0, 0, 0 };
            properties3.Increment = new decimal(bits4);
            this.spinStorageQuota.Properties.IsFloatValue = false;
            this.spinStorageQuota.Properties.Mask.EditMask = "N00";
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties4 = this.spinStorageQuota.Properties;
            int[] bits5 = new int[4] { 1000000, 0, 0, 0 };
            properties4.MaxValue = new decimal(bits5);
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties5 = this.spinStorageQuota.Properties;
            int[] bits6 = new int[4] { 110, 0, 0, 0 };
            properties5.MinValue = new decimal(bits6);
            this.spinStorageQuota.Size = new System.Drawing.Size(125, 20);
            this.spinStorageQuota.TabIndex = 1;
            this.lblServerResourceQuota.Location = new System.Drawing.Point(11, 38);
            this.lblServerResourceQuota.Name = "lblServerResourceQuota";
            this.lblServerResourceQuota.Size = new System.Drawing.Size(117, 13);
            this.lblServerResourceQuota.TabIndex = 2;
            this.lblServerResourceQuota.Text = "Server Resource Quota:";
            this.lblStorageQuota.Location = new System.Drawing.Point(11, 8);
            this.lblStorageQuota.Name = "lblStorageQuota";
            this.lblStorageQuota.Size = new System.Drawing.Size(75, 13);
            this.lblStorageQuota.TabIndex = 0;
            this.lblStorageQuota.Text = "Storage Quota:";
            this.pnlSetSiteQuota.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSetSiteQuota.Controls.Add(this.helpSetSiteQuota);
            this.pnlSetSiteQuota.Controls.Add(this.tsSetSiteQuota);
            this.pnlSetSiteQuota.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSetSiteQuota.Location = new System.Drawing.Point(3, 170);
            this.pnlSetSiteQuota.Name = "pnlSetSiteQuota";
            this.pnlSetSiteQuota.Size = new System.Drawing.Size(381, 37);
            this.pnlSetSiteQuota.TabIndex = 8;
            this.helpSetSiteQuota.AnchoringControl = null;
            this.helpSetSiteQuota.BackColor = System.Drawing.Color.Transparent;
            this.helpSetSiteQuota.CommonParentControl = null;
            this.helpSetSiteQuota.Image = (System.Drawing.Image)resources.GetObject("helpSetSiteQuota.Image");
            this.helpSetSiteQuota.Location = new System.Drawing.Point(164, 4);
            this.helpSetSiteQuota.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpSetSiteQuota.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpSetSiteQuota.Name = "helpSetSiteQuota";
            this.helpSetSiteQuota.RealOffset = null;
            this.helpSetSiteQuota.RelativeOffset = null;
            this.helpSetSiteQuota.Size = new System.Drawing.Size(20, 20);
            this.helpSetSiteQuota.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpSetSiteQuota.TabIndex = 1;
            this.helpSetSiteQuota.TabStop = false;
            this.tsSetSiteQuota.Location = new System.Drawing.Point(11, 1);
            this.tsSetSiteQuota.Margin = new System.Windows.Forms.Padding(11, 10, 3, 3);
            this.tsSetSiteQuota.Name = "tsSetSiteQuota";
            this.tsSetSiteQuota.Properties.Appearance.Options.UseTextOptions = true;
            this.tsSetSiteQuota.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsSetSiteQuota.Properties.OffText = "Set Site Quota";
            this.tsSetSiteQuota.Properties.OnText = "Set Site Quota";
            this.tsSetSiteQuota.Size = new System.Drawing.Size(157, 24);
            this.tsSetSiteQuota.TabIndex = 0;
            this.lcgAdvancedOptions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgAdvancedOptions.GroupBordersVisible = false;
            DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this.lcgAdvancedOptions.Items;
            DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[2] { this.esAdvancedOptions, this.lcgAdvanced };
            items.AddRange(items2);
            this.lcgAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvancedOptions.Name = "lcgAdvancedOptions";
            this.lcgAdvancedOptions.Size = new System.Drawing.Size(435, 359);
            this.lcgAdvancedOptions.Text = "Advanced Options";
            this.lcgAdvancedOptions.TextVisible = false;
            this.esAdvancedOptions.AllowHotTrack = false;
            this.esAdvancedOptions.Location = new System.Drawing.Point(0, 326);
            this.esAdvancedOptions.Name = "esAdvancedOptions";
            this.esAdvancedOptions.Size = new System.Drawing.Size(415, 13);
            this.esAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lcgAdvanced.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lcgAdvanced.AppearanceGroup.Options.UseFont = true;
            this.lcgAdvanced.ExpandButtonVisible = true;
            this.lcgAdvanced.ExpandOnDoubleClick = true;
            this.lcgAdvanced.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciAdvancedOptions });
            this.lcgAdvanced.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvanced.Name = "lcgAdvanced";
            this.lcgAdvanced.Size = new System.Drawing.Size(415, 326);
            this.lcgAdvanced.Text = "Advanced Options";
            this.lciAdvancedOptions.Control = this.gcAdvancedOptions;
            this.lciAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lciAdvancedOptions.Name = "lciAdvancedOptions";
            this.lciAdvancedOptions.Size = new System.Drawing.Size(391, 282);
            this.lciAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lciAdvancedOptions.TextVisible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.tlpSiteCollectionOptions);
            base.Name = "TCSiteCollectionOptionsBasicView";
            base.Size = new System.Drawing.Size(623, 467);
            this.tlpSiteCollectionOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pnlWebApplication).EndInit();
            this.pnlWebApplication.ResumeLayout(false);
            this.pnlWebApplication.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpSiteCollectionAdministrator).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpSiteCollectionURL).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpWebApplication).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tESiteCollectionAdministrator.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tETargetSiteUrl.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbPath.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbWebApplication.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).EndInit();
            this.pnlAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedOptions).EndInit();
            this.lcAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gcAdvancedOptions).EndInit();
            this.gcAdvancedOptions.ResumeLayout(false);
            this.tlpAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pnlTemplate).EndInit();
            this.pnlTemplate.ResumeLayout(false);
            this.pnlTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpCopySiteCollectionAdmin).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListTemplateGallery).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpTemplate).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyAuditSettings).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpMasterPageGallery).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyAuditSettings.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySiteCollectionAdmin.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListTemplateGallery.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyMasterPageGallery.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.cmbTemplate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlQuota).EndInit();
            this.pnlQuota.ResumeLayout(false);
            this.pnlQuota.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.spinResourceQuota.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.spinStorageQuota.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlSetSiteQuota).EndInit();
            this.pnlSetSiteQuota.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpSetSiteQuota).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsSetSiteQuota.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvanced).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).EndInit();
            base.ResumeLayout(false);
        }

        public bool IsTargetSameAsSource()
        {
            if (!base.IsModeSwitched && !AdapterConfigurationVariables.AllowDuplicateSiteCollection && SourceNodes[0] is SPWeb sPWeb)
            {
                string siteUrl = GetSiteUrl();
                string value = $"{siteUrl.Trim()}{_options.URL.Trim()}";
                if (sPWeb.Url.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    FlatXtraMessageBox.Show(Metalogix.Actions.Properties.Resources.CannotRunTargetIsSameAsSource);
                    return true;
                }
            }
            return false;
        }

        private bool IsUIValidated()
        {
            if (string.IsNullOrEmpty(tETargetSiteUrl.Text) && ((SPPath)cmbPath.SelectedItem).IsWildcard)
            {
                FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgSiteCollectionUrl, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            if (!Utils.IsValidSharePointURL(tETargetSiteUrl.Text, _options.IsHostHeader))
            {
                string text = $"{Metalogix.Actions.Properties.Resources.ValidSiteCollectionURL} {(_options.IsHostHeader ? Utils.illegalCharactersForHostHeaderSiteUrl : Utils.IllegalCharactersForSiteUrl)}";
                FlatXtraMessageBox.Show(text, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            if (string.IsNullOrEmpty(tESiteCollectionAdministrator.Text))
            {
                FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgSiteCollectionOwner, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }
            if (_options.IsSameServer && ((SPWebApplication)cmbWebApplication.SelectedItem).Name == _options.SourceWebApplication)
            {
                string text2 = cmbPath.Text.Trim('/');
                string sourcePath = _options.SourcePath;
                char[] trimChars = new char[1] { '/' };
                if (text2 == sourcePath.Trim(trimChars))
                {
                    if ((cmbPath.SelectedItem as SPPath).IsWildcard)
                    {
                        string text3 = Utils.JoinUrl(cmbPath.Text, tETargetSiteUrl.Text);
                        char[] trimChars2 = new char[1] { '/' };
                        if (text3.Trim(trimChars2) != _options.SourceUrlName.Trim('/'))
                        {
                            return false;
                        }
                    }
                    FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgInvalidSiteCollectionSelection, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return true;
                }
            }
            return false;
        }

        private void lcAdvancedOptions_GroupExpandChanged(object sender, LayoutGroupEventArgs e)
        {
            int size = (_isTargetSharePointOnline ? 253 : 219);
            if (this.AdvancedOptionsMainControlStateChanged != null)
            {
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(size, !e.Group.Expanded), null);
            }
        }

        protected override void LoadUI()
        {
            tETargetSiteUrl.Text = string.Empty;
            ComboBoxEdit comboBoxEdit = cmbWebApplication;
            bool enabled = Options.WebApplicationName == null || (!Options.SelfServiceCreateMode && base.Target != null && base.Target.ShowAllSites);
            comboBoxEdit.Enabled = enabled;
            tsSetSiteQuota.Enabled = !Options.SelfServiceCreateMode;
            if (!string.IsNullOrEmpty(_options.OwnerLogin))
            {
                tESiteCollectionAdministrator.Text = _options.OwnerLogin;
                if (base.Target is SPTenant || base.SourceAdapter.SharePointVersion.IsSharePointOnline)
                {
                    tESiteCollectionAdministrator.Text = string.Empty;
                }
            }
            tsCopyMasterPageGallery.IsOn = Options.CopyMasterPageGallery;
            tsCopyListTemplateGallery.IsOn = Options.CopyListTemplateGallery;
            if (base.Target != null && base.SourceAdapter.SharePointVersion.MajorVersion != base.Target.Adapter.SharePointVersion.MajorVersion)
            {
                tsCopyListTemplateGallery.IsOn = false;
                tsCopyListTemplateGallery.Enabled = false;
            }
            _webAppName = _options.WebApplicationName;
            if (base.Target != null && base.Target.Languages.Count != 0 && base.Target.Languages[0].HasMultipleExperienceVersions && !base.Target.Adapter.SharePointVersion.IsSharePoint2016)
            {
                int num = -1;
                string text = null;
                _experienceLableToVersionMap.Clear();
                foreach (int experienceVersion in base.Target.Languages[0].ExperienceVersions)
                {
                    string experinceVersionLabel = GetExperinceVersionLabel(experienceVersion);
                    _experienceLableToVersionMap.Add(experinceVersionLabel, experienceVersion);
                    if (experienceVersion > num)
                    {
                        text = experinceVersionLabel;
                        num = experienceVersion;
                    }
                }
                if (base.Target.Languages[0].ExperienceVersions.Contains(Options.ExperienceVersion))
                {
                    _selectedExperienceVersion = GetExperinceVersionLabel(Options.ExperienceVersion);
                }
                else if (text != null)
                {
                    _selectedExperienceVersion = text;
                }
            }
            _webAppName = _options.WebApplicationName;
            if (cmbWebApplication.Properties.Items.Count > 0)
            {
                foreach (object item in cmbWebApplication.Properties.Items)
                {
                    SPWebApplication sPWebApplication = item as SPWebApplication;
                    if (!(sPWebApplication != null) || !(sPWebApplication.Name == _webAppName))
                    {
                        continue;
                    }
                    cmbWebApplication.SelectedItem = item;
                    break;
                }
            }
            if (base.Target != null)
            {
                foreach (SPLanguage language in base.Target.Languages)
                {
                    if (language != null && language.LCID.ToString().Equals(_options.LanguageCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _selectedLanguage = language;
                        UpdateAvailableTemplates();
                    }
                }
            }
            _templateName = _options.WebTemplateName;
            if (cmbTemplate.Properties.Items.Count > 0)
            {
                foreach (object item2 in cmbTemplate.Properties.Items)
                {
                    if (!(item2 is SPWebTemplate sPWebTemplate) || !(sPWebTemplate.Name == _templateName))
                    {
                        continue;
                    }
                    cmbTemplate.SelectedItem = item2;
                    break;
                }
            }
            tsCopySiteCollectionAdmin.IsOn = _options.CopySiteAdmins;
            tsSetSiteQuota.IsOn = _options.CopySiteQuota;
            if (!(base.Target is SPTenant))
            {
                UpdateWebAppData(_options.IsHostHeader);
            }
            else
            {
                spinStorageQuota.Value = _options.StorageQuota;
                spinResourceQuota.Value = Convert.ToDecimal(_options.ResourceQuota);
            }
            foreach (object item3 in cmbPath.Properties.Items)
            {
                if (item3.ToString() != _options.Path)
                {
                    continue;
                }
                cmbPath.SelectedItem = item3;
                break;
            }
            tsCopyAuditSettings.IsOn = _options.CopyAuditSettings;
            SetPathUrl();
        }

        private void On_Path_Changed(object sender, EventArgs e)
        {
            SetPathUrl();
        }

        private void On_WebApp_Changed(object sender, EventArgs e)
        {
            _webAppName = ((SPWebApplication)cmbWebApplication.SelectedItem).Name;
            if (cmbWebApplication.SelectedItem is SPWebApplication)
            {
                _selectedWebApplication = (SPWebApplication)cmbWebApplication.SelectedItem;
                UpdateWebAppData(Options != null && Options.IsHostHeader);
            }
        }

        private void On_WebTemplate_Changed(object sender, EventArgs e)
        {
            if (cmbTemplate.SelectedItem is SPWebTemplate sPWebTemplate)
            {
                _templateName = sPWebTemplate.Name;
            }
        }

        public override bool SaveUI()
        {
            if (!base.IsModeSwitched && IsUIValidated())
            {
                return false;
            }
            _options.LanguageCode = _selectedLanguage.LCID.ToString();
            _options.Name = tETargetSiteUrl.Text;
            _options.WebTemplateName = _templateName;
            _options.Path = cmbPath.Text;
            _options.URL = cmbPath.Text + tETargetSiteUrl.Text;
            _options.OwnerLogin = tESiteCollectionAdministrator.Text;
            _webAppName = ((SPWebApplication)cmbWebApplication.SelectedItem).Name;
            _options.WebApplicationName = _webAppName;
            _options.CopyMasterPageGallery = tsCopyMasterPageGallery.IsOn;
            _options.CopyListTemplateGallery = tsCopyListTemplateGallery.IsOn;
            _options.CopyAuditSettings = tsCopyAuditSettings.IsOn;
            _options.CopySiteAdmins = tsCopySiteCollectionAdmin.IsOn;
            _options.CopySiteQuota = tsSetSiteQuota.IsOn;
            if (base.Target is SPTenant)
            {
                _options.StorageQuota = Convert.ToInt64(spinStorageQuota.Value);
                _options.ResourceQuota = Convert.ToDouble(spinResourceQuota.Value);
            }
            _options.ExperienceVersion = (_experienceLableToVersionMap.TryGetValue(_selectedExperienceVersion, out var value) ? value : (-1));
            return true;
        }

        private void SetPathUrl()
        {
            if (cmbPath.SelectedItem is SPPath sPPath)
            {
                TextEdit textEdit = tETargetSiteUrl;
                bool enabled = sPPath.IsWildcard || (Options != null && Options.IsHostHeader);
                textEdit.Enabled = enabled;
                if (!sPPath.IsWildcard && Options != null && !Options.IsHostHeader)
                {
                    tETargetSiteUrl.Text = string.Empty;
                }
                else if (string.IsNullOrEmpty(tETargetSiteUrl.Text) && Options != null)
                {
                    tETargetSiteUrl.Text = Options.Name ?? SourceUrl;
                }
            }
        }

        private void TrimSiteUrl()
        {
            lblSiteUrl.ToolTip = string.Empty;
            if (lblSiteUrl.Text.Length > 30)
            {
                lblSiteUrl.ToolTip = SPUtils.InsertStringAtInterval(lblSiteUrl.Text, "\n", 120);
            }
            if (lblSiteUrl.Text.Length > 30)
            {
                lblSiteUrl.Text = lblSiteUrl.Text.Substring(0, 30) + "...";
            }
        }

        private void UpdateAvailableTemplates()
        {
            if (_experienceLableToVersionMap.Count <= 1 || !_experienceLableToVersionMap.ContainsKey(_selectedExperienceVersion))
            {
                base.TargetWebTemplates = _selectedLanguage.Templates;
            }
            else
            {
                base.TargetWebTemplates = _selectedLanguage.GetTemplatesForExperienceVersion(_experienceLableToVersionMap[_selectedExperienceVersion]);
            }
        }

        protected override string UpdateServerData()
        {
            string result = string.Empty;
            cmbWebApplication.Properties.Items.Clear();
            if (base.Target != null)
            {
                if (base.Target.WebApplications.Count <= 0)
                {
                    result = "Could not enumerate target web applications";
                }
                else
                {
                    foreach (SPWebApplication webApplication in base.Target.WebApplications)
                    {
                        cmbWebApplication.Properties.Items.Add(webApplication);
                    }
                    if (cmbWebApplication.Properties.Items.Count > 0)
                    {
                        cmbWebApplication.SelectedIndex = 0;
                    }
                }
            }
            return result;
        }

        protected override void UpdateTemplateUI()
        {
            SuspendLayout();
            bool flag = false;
            cmbTemplate.Properties.Items.Clear();
            IEnumerable<SPWebTemplate> source = base.TargetWebTemplates.Where(CreateSiteCollectionControl.RestrictedTemplates.Compile());
            if (base.Target.Adapter.SharePointVersion.IsSharePoint2013OrLater)
            {
                source = source.Where(CreateSiteCollectionControl.RestrictedTemplatesInSp2013.Compile());
            }
            source.OrderBy((SPWebTemplate template) => template.Title).ToList().ForEach(delegate (SPWebTemplate template)
            {
                cmbTemplate.Properties.Items.Add(template);
                if (template.Name == _templateName)
                {
                    cmbTemplate.SelectedItem = template;
                    flag = true;
                }
            });
            if (!flag)
            {
                int num = cmbTemplate.FindString("Blank Site");
                cmbTemplate.SelectedIndex = ((num != -1) ? num : 0);
            }
            ResumeLayout();
        }

        private void UpdateWebAppData(bool isHostHeaderCheck)
        {
            SuspendLayout();
            if (!isHostHeaderCheck)
            {
                cmbPath.Properties.Items.Clear();
                _selectedWebApplication.Paths.ForEach(delegate (SPPath path)
                {
                    cmbPath.Properties.Items.Add(path);
                });
            }
            if (!isHostHeaderCheck)
            {
                lblSiteUrl.Text = _selectedWebApplication.Url;
                TrimSiteUrl();
                cmbPath.SetIdealComboBoxEditSize();
                cmbPath.Width += 10;
                cmbPath.Show();
                cmbPath.Left = lblSiteUrl.Right + 6;
                tETargetSiteUrl.Left = cmbPath.Right + 6;
                tETargetSiteUrl.Width = cmbWebApplication.Right - (cmbPath.Right + 6);
            }
            else
            {
                lblSiteUrl.Text = (string.IsNullOrEmpty(Options.Name) ? Options.HostHeaderURL : Options.HostHeaderURL.Replace(Options.Name, string.Empty));
                cmbPath.Hide();
                TrimSiteUrl();
                tETargetSiteUrl.Left = lblSiteUrl.Right + 6;
                tETargetSiteUrl.Width = cmbWebApplication.Right - (lblSiteUrl.Right + 6);
            }
            SPPath sPPath = _selectedWebApplication.Paths.FirstOrDefault((SPPath path) => path.IsWildcard);
            if (!isHostHeaderCheck)
            {
                if (sPPath == null)
                {
                    cmbPath.SelectedIndex = 0;
                }
                else
                {
                    cmbPath.SelectedItem = sPPath;
                }
            }
            ResumeLayout();
        }
    }
}
