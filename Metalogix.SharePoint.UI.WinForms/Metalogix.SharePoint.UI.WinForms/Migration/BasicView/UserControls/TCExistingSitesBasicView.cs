// Metalogix.SharePoint.UI.WinForms, Version=8.3.0.3, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4
// Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCExistingSitesBasicView
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCExistingSitesBasicView : ScopableTabbableControl
    {
        private bool _supressEvents;

        private IContainer components;

        private LayoutControl lcExistingSites;

        private GroupControl gbxExistingSites;

        private LayoutControlGroup lcgExistingSites;

        private LayoutControlItem lciExistingSites;

        private SimpleButton btnUpdateSites;

        private ToggleSwitch tsUpdateSites;

        private TileControl tcExistingSites;

        private TileGroup tgOverwriteSites;

        private TileItem tiOverwriteSites;

        private TileGroup tgPreserveSites;

        private TileItem tiPreserveSites;

        private HelpTipButton helpUpdateSites;

        public bool IsPreserveSitesOptionChecked => tiPreserveSites.Checked;

        public int UpdateSiteOptions { get; private set; }

        public TCExistingSitesBasicView()
        {
            UpdateSiteOptions = 0;
            InitializeComponent();
            InitializeControls();
            tiOverwriteSites.Checked = true;
        }

        private void btnUpdateSites_Click(object sender, EventArgs e)
        {
            OpenUpdateSitesDialog();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public new ArrayList GetSummaryScreenDetails()
        {
            List<OptionsSummary> list = new List<OptionsSummary>
        {
            new OptionsSummary($"{gbxExistingSites.Text} : {(tiOverwriteSites.Checked ? tiOverwriteSites.Text : tiPreserveSites.Text)}", 2)
        };
            List<OptionsSummary> list2 = list;
            if (tiPreserveSites.Checked)
            {
                list2.Add(new OptionsSummary($"{tsUpdateSites.Properties.OnText} : {tsUpdateSites.IsOn}", 3));
            }
            return new ArrayList(list2);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCExistingSitesBasicView));
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.lcExistingSites = new DevExpress.XtraLayout.LayoutControl();
            this.gbxExistingSites = new DevExpress.XtraEditors.GroupControl();
            this.helpUpdateSites = new TooltipsTest.HelpTipButton();
            this.btnUpdateSites = new DevExpress.XtraEditors.SimpleButton();
            this.tsUpdateSites = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcExistingSites = new DevExpress.XtraEditors.TileControl();
            this.tgOverwriteSites = new DevExpress.XtraEditors.TileGroup();
            this.tiOverwriteSites = new DevExpress.XtraEditors.TileItem();
            this.tgPreserveSites = new DevExpress.XtraEditors.TileGroup();
            this.tiPreserveSites = new DevExpress.XtraEditors.TileItem();
            this.lcgExistingSites = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExistingSites = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)this.lcExistingSites).BeginInit();
            this.lcExistingSites.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingSites).BeginInit();
            this.gbxExistingSites.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateSites).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateSites.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingSites).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingSites).BeginInit();
            base.SuspendLayout();
            this.lcExistingSites.Controls.Add(this.gbxExistingSites);
            this.lcExistingSites.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcExistingSites.Location = new System.Drawing.Point(0, 0);
            this.lcExistingSites.Name = "lcExistingSites";
            this.lcExistingSites.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(565, 291, 250, 350);
            this.lcExistingSites.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcExistingSites.Root = this.lcgExistingSites;
            this.lcExistingSites.Size = new System.Drawing.Size(454, 135);
            this.lcExistingSites.TabIndex = 0;
            this.lcExistingSites.TabStop = false;
            this.lcExistingSites.Text = "layoutControl1";
            this.gbxExistingSites.Appearance.BackColor = System.Drawing.Color.White;
            this.gbxExistingSites.Appearance.Options.UseBackColor = true;
            this.gbxExistingSites.AppearanceCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.gbxExistingSites.AppearanceCaption.Options.UseFont = true;
            this.gbxExistingSites.Controls.Add(this.helpUpdateSites);
            this.gbxExistingSites.Controls.Add(this.btnUpdateSites);
            this.gbxExistingSites.Controls.Add(this.tsUpdateSites);
            this.gbxExistingSites.Controls.Add(this.tcExistingSites);
            this.gbxExistingSites.Location = new System.Drawing.Point(5, 5);
            this.gbxExistingSites.Name = "gbxExistingSites";
            this.gbxExistingSites.Size = new System.Drawing.Size(444, 125);
            this.gbxExistingSites.TabIndex = 0;
            this.gbxExistingSites.Text = "Existing Sites";
            this.helpUpdateSites.AnchoringControl = null;
            this.helpUpdateSites.BackColor = System.Drawing.Color.Transparent;
            this.helpUpdateSites.CommonParentControl = null;
            this.helpUpdateSites.Image = (System.Drawing.Image)resources.GetObject("helpUpdateSites.Image");
            this.helpUpdateSites.Location = new System.Drawing.Point(391, 95);
            this.helpUpdateSites.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateSites.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateSites.Name = "helpUpdateSites";
            this.helpUpdateSites.RealOffset = null;
            this.helpUpdateSites.RelativeOffset = null;
            this.helpUpdateSites.Size = new System.Drawing.Size(20, 20);
            this.helpUpdateSites.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpUpdateSites.TabIndex = 3;
            this.helpUpdateSites.TabStop = false;
            this.btnUpdateSites.Enabled = false;
            this.btnUpdateSites.Location = new System.Drawing.Point(361, 93);
            this.btnUpdateSites.Name = "btnUpdateSites";
            this.btnUpdateSites.Size = new System.Drawing.Size(23, 23);
            this.btnUpdateSites.TabIndex = 2;
            this.btnUpdateSites.Text = "...";
            this.btnUpdateSites.Click += new System.EventHandler(btnUpdateSites_Click);
            this.tsUpdateSites.Enabled = false;
            this.tsUpdateSites.Location = new System.Drawing.Point(203, 93);
            this.tsUpdateSites.Name = "tsUpdateSites";
            this.tsUpdateSites.Properties.Appearance.Options.UseTextOptions = true;
            this.tsUpdateSites.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsUpdateSites.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tsUpdateSites.Properties.OffText = "Update Sites";
            this.tsUpdateSites.Properties.OnText = "Update Sites";
            this.tsUpdateSites.Size = new System.Drawing.Size(152, 24);
            this.tsUpdateSites.TabIndex = 1;
            this.tsUpdateSites.Toggled += new System.EventHandler(tsUpdateSites_Toggled);
            this.tcExistingSites.AllowDrag = false;
            this.tcExistingSites.AllowDragTilesBetweenGroups = false;
            this.tcExistingSites.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingSites.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingSites.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingSites.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcExistingSites.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcExistingSites.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcExistingSites.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingSites.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcExistingSites.AppearanceItem.Normal.Options.UseFont = true;
            this.tcExistingSites.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcExistingSites.DragSize = new System.Drawing.Size(0, 0);
            this.tcExistingSites.Groups.Add(this.tgOverwriteSites);
            this.tcExistingSites.Groups.Add(this.tgPreserveSites);
            this.tcExistingSites.IndentBetweenGroups = 40;
            this.tcExistingSites.IndentBetweenItems = 30;
            this.tcExistingSites.ItemCheckMode = DevExpress.XtraEditors.TileItemCheckMode.Single;
            this.tcExistingSites.ItemSize = 50;
            this.tcExistingSites.Location = new System.Drawing.Point(10, 21);
            this.tcExistingSites.MaxId = 3;
            this.tcExistingSites.Name = "tcExistingSites";
            this.tcExistingSites.Padding = new System.Windows.Forms.Padding(12);
            this.tcExistingSites.Size = new System.Drawing.Size(350, 75);
            this.tcExistingSites.TabIndex = 0;
            this.tcExistingSites.Text = "tileControl1";
            this.tgOverwriteSites.Items.Add(this.tiOverwriteSites);
            this.tgOverwriteSites.Name = "tgOverwriteSites";
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Overwrite_Sites;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.StretchHorizontal = true;
            tileItemElement.Text = "Overwrite Sites";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiOverwriteSites.Elements.Add(tileItemElement);
            this.tiOverwriteSites.Id = 0;
            this.tiOverwriteSites.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiOverwriteSites.Name = "tiOverwriteSites";
            this.tiOverwriteSites.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiOverwriteItems_ItemClick);
            this.tgPreserveSites.Items.Add(this.tiPreserveSites);
            this.tgPreserveSites.Name = "tgPreserveSites";
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Preserve_Sites;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Preserve Sites";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiPreserveSites.Elements.Add(tileItemElement2);
            this.tiPreserveSites.Id = 1;
            this.tiPreserveSites.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiPreserveSites.Name = "tiPreserveSites";
            this.tiPreserveSites.CheckedChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveSites_CheckedChanged);
            this.tiPreserveSites.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveSites_ItemClick);
            this.lcgExistingSites.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgExistingSites.GroupBordersVisible = false;
            this.lcgExistingSites.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciExistingSites });
            this.lcgExistingSites.Location = new System.Drawing.Point(0, 0);
            this.lcgExistingSites.Name = "lcgExistingSites";
            this.lcgExistingSites.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            this.lcgExistingSites.Size = new System.Drawing.Size(454, 135);
            this.lcgExistingSites.TextVisible = false;
            this.lciExistingSites.Control = this.gbxExistingSites;
            this.lciExistingSites.Location = new System.Drawing.Point(0, 0);
            this.lciExistingSites.Name = "lciExistingSites";
            this.lciExistingSites.Size = new System.Drawing.Size(448, 129);
            this.lciExistingSites.TextSize = new System.Drawing.Size(0, 0);
            this.lciExistingSites.TextVisible = false;
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.lcExistingSites);
            base.Name = "TCExistingSitesBasicView";
            base.Size = new System.Drawing.Size(454, 135);
            this.UseTab = true;
            ((System.ComponentModel.ISupportInitialize)this.lcExistingSites).EndInit();
            this.lcExistingSites.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingSites).EndInit();
            this.gbxExistingSites.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateSites).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateSites.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingSites).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingSites).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            Type type = GetType();
            helpUpdateSites.SetResourceString(type.FullName + tsUpdateSites.Name, type);
            helpUpdateSites.IsBasicViewHelpIcon = true;
        }

        public void LoadUI(SPMigrationModeOptions migrationModeOptions)
        {
            bool flag = base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site;
            if (flag || base.Scope == SharePointObjectScope.List)
            {
                if (!flag)
                {
                    tiPreserveSites.Checked = true;
                }
                else
                {
                    tiOverwriteSites.Checked = migrationModeOptions.OverwriteSites;
                    tiPreserveSites.Checked = !migrationModeOptions.OverwriteSites;
                }
            }
            UpdateSiteOptions = migrationModeOptions.UpdateSiteOptionsBitField;
            _supressEvents = true;
            tsUpdateSites.IsOn = migrationModeOptions.UpdateSites;
            tsUpdateSites.Enabled = tiPreserveSites.Checked;
            btnUpdateSites.Enabled = tsUpdateSites.IsOn;
            _supressEvents = false;
        }

        private void OpenUpdateSitesDialog()
        {
            UpdateSiteOptionsDialogBasicView updateSiteOptionsDialogBasicView = new UpdateSiteOptionsDialogBasicView(UpdateSiteOptions);
            if (updateSiteOptionsDialogBasicView.ShowDialog() == DialogResult.OK)
            {
                UpdateSiteOptions = updateSiteOptionsDialogBasicView.UpdateSiteOptionsBitField;
            }
            if (UpdateSiteOptions == 0)
            {
                tsUpdateSites.IsOn = false;
            }
        }

        public void SaveUI(SPMigrationModeOptions migrationModeOptions)
        {
            migrationModeOptions.OverwriteSites = tiOverwriteSites.Checked;
            migrationModeOptions.UpdateSites = tsUpdateSites.IsOn;
            migrationModeOptions.UpdateSiteOptionsBitField = UpdateSiteOptions;
        }

        private void tiOverwriteItems_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveSites.Checked = false;
            tiOverwriteSites.Checked = true;
            ToggleSwitch toggleSwitch = tsUpdateSites;
            SimpleButton simpleButton = btnUpdateSites;
            bool enabled = (simpleButton.Enabled = tiPreserveSites.Checked);
            toggleSwitch.Enabled = enabled;
        }

        private void tiPreserveSites_CheckedChanged(object sender, TileItemEventArgs e)
        {
            if (base.ParentForm != null)
            {
                Control[] array = base.ParentForm.Controls.Find("TCMigrationModeOptionsBasicView", searchAllChildren: true);
                if (array.Length != 0 && array[1] is TCMigrationModeOptionsBasicView)
                {
                    TCMigrationModeOptionsBasicView tCMigrationModeOptionsBasicView = (TCMigrationModeOptionsBasicView)array[1];
                    tCMigrationModeOptionsBasicView.AdvancedOptions.UpdateTableControls(e.Item.Checked, isExistingSite: true);
                    int size = ((!e.Item.Checked) ? 32 : (tCMigrationModeOptionsBasicView.AdvancedOptions.IsPreserveListsOptionChecked ? 85 : 57));
                    tCMigrationModeOptionsBasicView.ChangeSize(size);
                }
            }
        }

        private void tiPreserveSites_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveSites.Checked = true;
            tiOverwriteSites.Checked = false;
            tsUpdateSites.Enabled = tiPreserveSites.Checked;
            btnUpdateSites.Enabled = tsUpdateSites.IsOn;
        }

        private void tsUpdateSites_Toggled(object sender, EventArgs e)
        {
            btnUpdateSites.Enabled = tsUpdateSites.IsOn;
            if (tsUpdateSites.IsOn && !_supressEvents)
            {
                OpenUpdateSitesDialog();
            }
        }
    }
}