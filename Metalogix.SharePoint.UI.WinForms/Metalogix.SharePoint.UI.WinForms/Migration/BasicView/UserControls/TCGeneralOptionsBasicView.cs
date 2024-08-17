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
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Actions.BasicView;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MigrationMode.png")]
    [ControlName("General Options")]
    public class TCGeneralOptionsBasicView : ScopableTabbableControl
    {
        private bool _supressEvents;

        private SPGeneralOptions _options;

        private SendEmailOptions _sendEmailOptions = new SendEmailOptions();

        private bool _isUpdateCorrectLinks;

        private IContainer components;

        private TableLayoutPanel tlpGeneralOptions;

        private PanelControl pnlCorrectLinks;

        private ToggleSwitch tsCorrectLinks;

        private TileControl tcCorrectLinksScope;

        private TileGroup tgSiteScope;

        private TileItem tiSiteScopeLinks;

        private TileGroup tgCopyOnlyScope;

        private TileItem tiCopyOnlyScope;

        private LayoutControl lcAdvancedOptions;

        private LayoutControlGroup lcgAdvancedOptionsMain;

        private PanelControl pnlSendEmail;

        private ToggleSwitch tsSendEmail;

        private HelpTipButton helpCorrectLinks;

        private SimpleButton btnSendEmail;

        private HelpTipButton helpRenamedContent;

        private ToggleSwitch tsTextFieldLinks;

        private ToggleSwitch tsRenamedContentLinks;

        private EmptySpaceItem esAdvancedOptions;

        private LayoutControlGroup lcgAdvancedOptions;

        private PanelControl pnlAdvancedOptions;

        private LayoutControlItem lciAdvancedOptions;

        private HelpTipButton helpTextFields;

        private HelpTipButton helpSendEmail;

        public bool IsUpdateCorrectLinks
        {
            set
            {
                _isUpdateCorrectLinks = value;
                UpdateCorrectLinksOption();
            }
        }

        public SPGeneralOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                LoadUI();
            }
        }

        public TCGeneralOptionsBasicView()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            ConfigureEmailSettingsDialogBasicView configureEmailSettingsDialogBasicView = new ConfigureEmailSettingsDialogBasicView
            {
                Options = _sendEmailOptions
            };
            configureEmailSettingsDialogBasicView.ShowDialog();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(new OptionsSummary("General Options", 0));
            if (!tsCorrectLinks.IsOn)
            {
                arrayList.Add(new OptionsSummary($"{tsCorrectLinks.Properties.OnText} : {tsCorrectLinks.IsOn}", 1));
            }
            else
            {
                arrayList.Add(new OptionsSummary($"{tsCorrectLinks.Properties.OnText} : {(tiCopyOnlyScope.Checked ? tiCopyOnlyScope.Text : tiSiteScopeLinks.Text)}", 1));
                if (tsTextFieldLinks.Enabled)
                {
                    arrayList.Add(new OptionsSummary($"{tsTextFieldLinks.Properties.OnText} : {tsTextFieldLinks.IsOn}", 2));
                }
                if (base.Scope != SharePointObjectScope.Item && tsRenamedContentLinks.Enabled)
                {
                    arrayList.Add(new OptionsSummary($"{tsRenamedContentLinks.Properties.OnText} : {tsRenamedContentLinks.IsOn}", 2));
                }
            }
            arrayList.Add(new OptionsSummary($"{tsSendEmail.Properties.OnText} : {tsSendEmail.IsOn}", 1));
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCGeneralOptionsBasicView));
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.tlpGeneralOptions = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSendEmail = new DevExpress.XtraEditors.PanelControl();
            this.helpSendEmail = new TooltipsTest.HelpTipButton();
            this.btnSendEmail = new DevExpress.XtraEditors.SimpleButton();
            this.tsSendEmail = new DevExpress.XtraEditors.ToggleSwitch();
            this.pnlCorrectLinks = new DevExpress.XtraEditors.PanelControl();
            this.helpCorrectLinks = new TooltipsTest.HelpTipButton();
            this.tsCorrectLinks = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcCorrectLinksScope = new DevExpress.XtraEditors.TileControl();
            this.tgSiteScope = new DevExpress.XtraEditors.TileGroup();
            this.tiSiteScopeLinks = new DevExpress.XtraEditors.TileItem();
            this.tgCopyOnlyScope = new DevExpress.XtraEditors.TileGroup();
            this.tiCopyOnlyScope = new DevExpress.XtraEditors.TileItem();
            this.lcAdvancedOptions = new DevExpress.XtraLayout.LayoutControl();
            this.pnlAdvancedOptions = new DevExpress.XtraEditors.PanelControl();
            this.helpTextFields = new TooltipsTest.HelpTipButton();
            this.helpRenamedContent = new TooltipsTest.HelpTipButton();
            this.tsTextFieldLinks = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsRenamedContentLinks = new DevExpress.XtraEditors.ToggleSwitch();
            this.lcgAdvancedOptionsMain = new DevExpress.XtraLayout.LayoutControlGroup();
            this.esAdvancedOptions = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcgAdvancedOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAdvancedOptions = new DevExpress.XtraLayout.LayoutControlItem();
            this.tlpGeneralOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlSendEmail).BeginInit();
            this.pnlSendEmail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpSendEmail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsSendEmail.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlCorrectLinks).BeginInit();
            this.pnlCorrectLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpCorrectLinks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCorrectLinks.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedOptions).BeginInit();
            this.lcAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).BeginInit();
            this.pnlAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpTextFields).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpRenamedContent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsTextFieldLinks.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsRenamedContentLinks.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptionsMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).BeginInit();
            base.SuspendLayout();
            this.tlpGeneralOptions.AutoSize = true;
            this.tlpGeneralOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpGeneralOptions.ColumnCount = 1;
            this.tlpGeneralOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpGeneralOptions.Controls.Add(this.pnlSendEmail, 0, 3);
            this.tlpGeneralOptions.Controls.Add(this.pnlCorrectLinks, 0, 0);
            this.tlpGeneralOptions.Controls.Add(this.tcCorrectLinksScope, 0, 1);
            this.tlpGeneralOptions.Controls.Add(this.lcAdvancedOptions, 0, 2);
            this.tlpGeneralOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpGeneralOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpGeneralOptions.Name = "tlpGeneralOptions";
            this.tlpGeneralOptions.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.tlpGeneralOptions.RowCount = 5;
            this.tlpGeneralOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneralOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneralOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneralOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneralOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneralOptions.Size = new System.Drawing.Size(565, 588);
            this.tlpGeneralOptions.TabIndex = 0;
            this.pnlSendEmail.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSendEmail.Controls.Add(this.helpSendEmail);
            this.pnlSendEmail.Controls.Add(this.btnSendEmail);
            this.pnlSendEmail.Controls.Add(this.tsSendEmail);
            this.pnlSendEmail.Location = new System.Drawing.Point(0, 265);
            this.pnlSendEmail.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSendEmail.Name = "pnlSendEmail";
            this.pnlSendEmail.Size = new System.Drawing.Size(559, 33);
            this.pnlSendEmail.TabIndex = 3;
            this.helpSendEmail.AnchoringControl = null;
            this.helpSendEmail.BackColor = System.Drawing.Color.Transparent;
            this.helpSendEmail.CommonParentControl = null;
            this.helpSendEmail.Image = (System.Drawing.Image)resources.GetObject("helpSendEmail.Image");
            this.helpSendEmail.Location = new System.Drawing.Point(283, 3);
            this.helpSendEmail.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpSendEmail.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpSendEmail.Name = "helpSendEmail";
            this.helpSendEmail.RealOffset = null;
            this.helpSendEmail.RelativeOffset = null;
            this.helpSendEmail.Size = new System.Drawing.Size(20, 20);
            this.helpSendEmail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpSendEmail.TabIndex = 2;
            this.helpSendEmail.TabStop = false;
            this.btnSendEmail.Enabled = false;
            this.btnSendEmail.Location = new System.Drawing.Point(254, 2);
            this.btnSendEmail.Name = "btnSendEmail";
            this.btnSendEmail.Size = new System.Drawing.Size(23, 23);
            this.btnSendEmail.TabIndex = 1;
            this.btnSendEmail.Text = "...";
            this.btnSendEmail.Click += new System.EventHandler(btnSendEmail_Click);
            this.tsSendEmail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.tsSendEmail.Location = new System.Drawing.Point(10, 2);
            this.tsSendEmail.Name = "tsSendEmail";
            this.tsSendEmail.Properties.Appearance.Options.UseTextOptions = true;
            this.tsSendEmail.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsSendEmail.Properties.OffText = "Send Email upon Job Completion";
            this.tsSendEmail.Properties.OnText = "Send Email upon Job Completion";
            this.tsSendEmail.Size = new System.Drawing.Size(240, 24);
            this.tsSendEmail.TabIndex = 0;
            this.tsSendEmail.Toggled += new System.EventHandler(tsSendEmail_Toggled);
            this.pnlCorrectLinks.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlCorrectLinks.Controls.Add(this.helpCorrectLinks);
            this.pnlCorrectLinks.Controls.Add(this.tsCorrectLinks);
            this.pnlCorrectLinks.Location = new System.Drawing.Point(0, 20);
            this.pnlCorrectLinks.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCorrectLinks.Name = "pnlCorrectLinks";
            this.pnlCorrectLinks.Size = new System.Drawing.Size(559, 33);
            this.pnlCorrectLinks.TabIndex = 1;
            this.helpCorrectLinks.AnchoringControl = null;
            this.helpCorrectLinks.BackColor = System.Drawing.Color.Transparent;
            this.helpCorrectLinks.CommonParentControl = null;
            this.helpCorrectLinks.Image = (System.Drawing.Image)resources.GetObject("helpCorrectLinks.Image");
            this.helpCorrectLinks.Location = new System.Drawing.Point(236, 2);
            this.helpCorrectLinks.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCorrectLinks.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCorrectLinks.Name = "helpCorrectLinks";
            this.helpCorrectLinks.RealOffset = null;
            this.helpCorrectLinks.RelativeOffset = null;
            this.helpCorrectLinks.Size = new System.Drawing.Size(20, 20);
            this.helpCorrectLinks.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCorrectLinks.TabIndex = 1;
            this.helpCorrectLinks.TabStop = false;
            this.tsCorrectLinks.EditValue = true;
            this.tsCorrectLinks.Location = new System.Drawing.Point(10, 0);
            this.tsCorrectLinks.Name = "tsCorrectLinks";
            this.tsCorrectLinks.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCorrectLinks.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCorrectLinks.Properties.OffText = "Correct Links during Migration";
            this.tsCorrectLinks.Properties.OnText = "Correct Links during Migration";
            this.tsCorrectLinks.Size = new System.Drawing.Size(228, 24);
            this.tsCorrectLinks.TabIndex = 0;
            this.tsCorrectLinks.Toggled += new System.EventHandler(tsCorrectLinks_Toggled);
            this.tcCorrectLinksScope.AllowDrag = false;
            this.tcCorrectLinksScope.AllowDragTilesBetweenGroups = false;
            this.tcCorrectLinksScope.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcCorrectLinksScope.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcCorrectLinksScope.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcCorrectLinksScope.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcCorrectLinksScope.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcCorrectLinksScope.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcCorrectLinksScope.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcCorrectLinksScope.BackColor = System.Drawing.Color.White;
            this.tcCorrectLinksScope.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcCorrectLinksScope.Dock = System.Windows.Forms.DockStyle.Left;
            this.tcCorrectLinksScope.DragSize = new System.Drawing.Size(0, 0);
            this.tcCorrectLinksScope.Groups.Add(this.tgSiteScope);
            this.tcCorrectLinksScope.Groups.Add(this.tgCopyOnlyScope);
            this.tcCorrectLinksScope.IndentBetweenGroups = 40;
            this.tcCorrectLinksScope.IndentBetweenItems = 35;
            this.tcCorrectLinksScope.ItemSize = 60;
            this.tcCorrectLinksScope.Location = new System.Drawing.Point(0, 53);
            this.tcCorrectLinksScope.Margin = new System.Windows.Forms.Padding(0);
            this.tcCorrectLinksScope.MaxId = 2;
            this.tcCorrectLinksScope.Name = "tcCorrectLinksScope";
            this.tcCorrectLinksScope.Padding = new System.Windows.Forms.Padding(5);
            this.tcCorrectLinksScope.Size = new System.Drawing.Size(503, 81);
            this.tcCorrectLinksScope.TabIndex = 1;
            this.tcCorrectLinksScope.Text = "tileControl1";
            this.tgSiteScope.Items.Add(this.tiSiteScopeLinks);
            this.tgSiteScope.Name = "tgSiteScope";
            this.tiSiteScopeLinks.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tiSiteScopeLinks.AppearanceItem.Normal.Options.UseFont = true;
            this.tiSiteScopeLinks.Checked = true;
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Correct_links_wth_scope_of_site;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.Text = "Within Site Collection Scope";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiSiteScopeLinks.Elements.Add(tileItemElement);
            this.tiSiteScopeLinks.Id = 0;
            this.tiSiteScopeLinks.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiSiteScopeLinks.Name = "tiSiteScopeLinks";
            this.tiSiteScopeLinks.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiSiteScopeLinks_ItemClick);
            this.tgCopyOnlyScope.Items.Add(this.tiCopyOnlyScope);
            this.tgCopyOnlyScope.Name = "tgCopyOnlyScope";
            this.tiCopyOnlyScope.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tiCopyOnlyScope.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Correct_links_with_scope_of_copy_only;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Within Scope of Copy";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiCopyOnlyScope.Elements.Add(tileItemElement2);
            this.tiCopyOnlyScope.Id = 1;
            this.tiCopyOnlyScope.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiCopyOnlyScope.Name = "tiCopyOnlyScope";
            this.tiCopyOnlyScope.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiCopyOnlyScope_ItemClick);
            this.lcAdvancedOptions.AutoScroll = false;
            this.lcAdvancedOptions.Controls.Add(this.pnlAdvancedOptions);
            this.lcAdvancedOptions.Font = new System.Drawing.Font("Tahoma", 8.25f);
            this.lcAdvancedOptions.Location = new System.Drawing.Point(74, 139);
            this.lcAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.lcAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lcAdvancedOptions.Margin = new System.Windows.Forms.Padding(74, 5, 2, 2);
            this.lcAdvancedOptions.Name = "lcAdvancedOptions";
            this.lcAdvancedOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(725, 67, 250, 350);
            this.lcAdvancedOptions.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcAdvancedOptions.Root = this.lcgAdvancedOptionsMain;
            this.lcAdvancedOptions.Size = new System.Drawing.Size(354, 124);
            this.lcAdvancedOptions.TabIndex = 2;
            this.lcAdvancedOptions.Text = "layoutControl1";
            this.lcAdvancedOptions.GroupExpandChanged += new DevExpress.XtraLayout.Utils.LayoutGroupEventHandler(lcAdvancedOptions_GroupExpandChanged);
            this.pnlAdvancedOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlAdvancedOptions.Controls.Add(this.helpTextFields);
            this.pnlAdvancedOptions.Controls.Add(this.helpRenamedContent);
            this.pnlAdvancedOptions.Controls.Add(this.tsTextFieldLinks);
            this.pnlAdvancedOptions.Controls.Add(this.tsRenamedContentLinks);
            this.pnlAdvancedOptions.Location = new System.Drawing.Point(14, 34);
            this.pnlAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017";
            this.pnlAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlAdvancedOptions.Name = "pnlAdvancedOptions";
            this.pnlAdvancedOptions.Size = new System.Drawing.Size(318, 66);
            this.pnlAdvancedOptions.TabIndex = 4;
            this.helpTextFields.AnchoringControl = null;
            this.helpTextFields.BackColor = System.Drawing.Color.Transparent;
            this.helpTextFields.CommonParentControl = null;
            this.helpTextFields.Image = (System.Drawing.Image)resources.GetObject("helpTextFields.Image");
            this.helpTextFields.Location = new System.Drawing.Point(234, 8);
            this.helpTextFields.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpTextFields.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpTextFields.Name = "helpTextFields";
            this.helpTextFields.RealOffset = null;
            this.helpTextFields.RelativeOffset = null;
            this.helpTextFields.Size = new System.Drawing.Size(20, 20);
            this.helpTextFields.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpTextFields.TabIndex = 6;
            this.helpTextFields.TabStop = false;
            this.helpRenamedContent.AnchoringControl = null;
            this.helpRenamedContent.BackColor = System.Drawing.Color.Transparent;
            this.helpRenamedContent.CommonParentControl = null;
            this.helpRenamedContent.Image = (System.Drawing.Image)resources.GetObject("helpRenamedContent.Image");
            this.helpRenamedContent.Location = new System.Drawing.Point(276, 38);
            this.helpRenamedContent.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpRenamedContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpRenamedContent.Name = "helpRenamedContent";
            this.helpRenamedContent.RealOffset = null;
            this.helpRenamedContent.RelativeOffset = null;
            this.helpRenamedContent.Size = new System.Drawing.Size(20, 20);
            this.helpRenamedContent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpRenamedContent.TabIndex = 5;
            this.helpRenamedContent.TabStop = false;
            this.tsTextFieldLinks.Location = new System.Drawing.Point(22, 7);
            this.tsTextFieldLinks.Margin = new System.Windows.Forms.Padding(0);
            this.tsTextFieldLinks.Name = "tsTextFieldLinks";
            this.tsTextFieldLinks.Properties.Appearance.Options.UseTextOptions = true;
            this.tsTextFieldLinks.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsTextFieldLinks.Properties.OffText = "Correct Links in Text Fields";
            this.tsTextFieldLinks.Properties.OnText = "Correct Links in Text Fields";
            this.tsTextFieldLinks.Size = new System.Drawing.Size(215, 24);
            this.tsTextFieldLinks.TabIndex = 3;
            this.tsRenamedContentLinks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.tsRenamedContentLinks.Location = new System.Drawing.Point(22, 37);
            this.tsRenamedContentLinks.Name = "tsRenamedContentLinks";
            this.tsRenamedContentLinks.Properties.Appearance.Options.UseTextOptions = true;
            this.tsRenamedContentLinks.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsRenamedContentLinks.Properties.OffText = "Correct Links for Renamed Content";
            this.tsRenamedContentLinks.Properties.OnText = "Correct Links for Renamed Content";
            this.tsRenamedContentLinks.Size = new System.Drawing.Size(254, 24);
            this.tsRenamedContentLinks.TabIndex = 4;
            this.lcgAdvancedOptionsMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.lcgAdvancedOptionsMain.GroupBordersVisible = false;
            DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this.lcgAdvancedOptionsMain.Items;
            DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[2] { this.esAdvancedOptions, this.lcgAdvancedOptions };
            items.AddRange(items2);
            this.lcgAdvancedOptionsMain.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvancedOptionsMain.Name = "Root";
            this.lcgAdvancedOptionsMain.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.lcgAdvancedOptionsMain.Size = new System.Drawing.Size(346, 124);
            this.lcgAdvancedOptionsMain.TextVisible = false;
            this.esAdvancedOptions.AllowHotTrack = false;
            this.esAdvancedOptions.Location = new System.Drawing.Point(0, 114);
            this.esAdvancedOptions.Name = "esAdvancedOptions";
            this.esAdvancedOptions.Size = new System.Drawing.Size(346, 10);
            this.esAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lcgAdvancedOptions.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lcgAdvancedOptions.AppearanceGroup.Options.UseFont = true;
            this.lcgAdvancedOptions.ExpandButtonVisible = true;
            this.lcgAdvancedOptions.ExpandOnDoubleClick = true;
            this.lcgAdvancedOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciAdvancedOptions });
            this.lcgAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvancedOptions.Name = "lcgAdvancedOptions";
            this.lcgAdvancedOptions.Size = new System.Drawing.Size(346, 114);
            this.lcgAdvancedOptions.Text = "Advanced Options";
            this.lciAdvancedOptions.Control = this.pnlAdvancedOptions;
            this.lciAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lciAdvancedOptions.Name = "lciAdvancedOptions";
            this.lciAdvancedOptions.Size = new System.Drawing.Size(322, 70);
            this.lciAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lciAdvancedOptions.TextVisible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.tlpGeneralOptions);
            base.Name = "TCGeneralOptionsBasicView";
            base.Size = new System.Drawing.Size(565, 588);
            this.tlpGeneralOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pnlSendEmail).EndInit();
            this.pnlSendEmail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpSendEmail).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsSendEmail.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlCorrectLinks).EndInit();
            this.pnlCorrectLinks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpCorrectLinks).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCorrectLinks.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcAdvancedOptions).EndInit();
            this.lcAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).EndInit();
            this.pnlAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpTextFields).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpRenamedContent).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsTextFieldLinks.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsRenamedContentLinks.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptionsMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.esAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeControls()
        {
            Type type = GetType();
            helpCorrectLinks.SetResourceString(type.FullName + tsCorrectLinks.Name, type);
            helpCorrectLinks.IsBasicViewHelpIcon = true;
            helpRenamedContent.SetResourceString(type.FullName + tsRenamedContentLinks.Name, type);
            helpRenamedContent.IsBasicViewHelpIcon = true;
            helpTextFields.SetResourceString(type.FullName + tsTextFieldLinks.Name, type);
            helpTextFields.IsBasicViewHelpIcon = true;
            helpSendEmail.SetResourceString(type.FullName + tsSendEmail.Name, type);
            helpSendEmail.IsBasicViewHelpIcon = true;
        }

        private void lcAdvancedOptions_GroupExpandChanged(object sender, LayoutGroupEventArgs e)
        {
            lcAdvancedOptions.Height = (e.Group.Expanded ? (lcAdvancedOptions.Height + 82) : (lcAdvancedOptions.Height - 82));
        }

        protected override void LoadUI()
        {
            tsCorrectLinks.IsOn = Options.CorrectingLinks;
            if (Options.LinkCorrectionScope == LinkCorrectionScope.SiteCollection)
            {
                tiSiteScopeLinks.Checked = true;
                tiCopyOnlyScope.Checked = false;
            }
            else if (Options.LinkCorrectionScope == LinkCorrectionScope.MigrationOnly)
            {
                tiSiteScopeLinks.Checked = false;
                tiCopyOnlyScope.Checked = true;
            }
            if (base.Scope == SharePointObjectScope.List && BCSHelper.HasExternalListsOnly(SourceNodes))
            {
                pnlCorrectLinks.Visible = false;
                tcCorrectLinksScope.Visible = false;
                lcAdvancedOptions.Visible = false;
            }
            if (base.Scope == SharePointObjectScope.Item)
            {
                helpRenamedContent.Visible = false;
                HideControl(tsRenamedContentLinks);
            }
            tsTextFieldLinks.IsOn = Options.LinkCorrectTextFields;
            if (base.Scope != SharePointObjectScope.Item)
            {
                tsRenamedContentLinks.IsOn = Options.UseComprehensiveLinkCorrection;
            }
            _supressEvents = true;
            tsSendEmail.IsOn = Options.SendEmail;
            btnSendEmail.Enabled = tsSendEmail.IsOn;
            _sendEmailOptions.SetFromOptions(Options);
            _supressEvents = false;
        }

        public override bool SaveUI()
        {
            if (base.Scope != SharePointObjectScope.Permissions)
            {
                Options.CorrectingLinks = tsCorrectLinks.IsOn;
                Options.LinkCorrectTextFields = tsTextFieldLinks.IsOn;
                if (base.Scope != SharePointObjectScope.Item)
                {
                    Options.UseComprehensiveLinkCorrection = tsRenamedContentLinks.IsOn;
                }
                Options.LinkCorrectionScope = (tiCopyOnlyScope.Checked ? LinkCorrectionScope.MigrationOnly : LinkCorrectionScope.SiteCollection);
            }
            else
            {
                Options.CorrectingLinks = false;
                Options.LinkCorrectTextFields = false;
                Options.UseComprehensiveLinkCorrection = false;
            }
            Options.SendEmail = tsSendEmail.IsOn;
            Options.SetFromOptions(_sendEmailOptions);
            Options.EnableSslForEmail = AdapterConfigurationVariables.EnableSslForEmail;
            return true;
        }

        private void tiCopyOnlyScope_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!tiCopyOnlyScope.Checked)
            {
                tiCopyOnlyScope.Checked = true;
                tiSiteScopeLinks.Checked = false;
            }
        }

        private void tiSiteScopeLinks_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!tiSiteScopeLinks.Checked)
            {
                tiSiteScopeLinks.Checked = true;
                tiCopyOnlyScope.Checked = false;
            }
        }

        private void tsCorrectLinks_Toggled(object sender, EventArgs e)
        {
            if (tsCorrectLinks.IsOn)
            {
                tcCorrectLinksScope.Visible = true;
                lcAdvancedOptions.Visible = true;
            }
            else
            {
                tcCorrectLinksScope.Visible = false;
                lcAdvancedOptions.Visible = false;
            }
        }

        private void tsSendEmail_Toggled(object sender, EventArgs e)
        {
            if (_supressEvents)
            {
                return;
            }
            btnSendEmail.Enabled = tsSendEmail.IsOn;
            if (tsSendEmail.IsOn)
            {
                ConfigureEmailSettingsDialogBasicView configureEmailSettingsDialogBasicView = new ConfigureEmailSettingsDialogBasicView
                {
                    Options = _sendEmailOptions
                };
                configureEmailSettingsDialogBasicView.ShowDialog();
                if (configureEmailSettingsDialogBasicView.DialogResult == DialogResult.Cancel && string.IsNullOrEmpty(configureEmailSettingsDialogBasicView.Options.EmailServer))
                {
                    tsSendEmail.IsOn = false;
                }
            }
        }

        private void UpdateCorrectLinksOption()
        {
            if (!_isUpdateCorrectLinks)
            {
                tsCorrectLinks.Enabled = true;
                return;
            }
            tsCorrectLinks.IsOn = true;
            tsCorrectLinks.Enabled = false;
        }
    }
}