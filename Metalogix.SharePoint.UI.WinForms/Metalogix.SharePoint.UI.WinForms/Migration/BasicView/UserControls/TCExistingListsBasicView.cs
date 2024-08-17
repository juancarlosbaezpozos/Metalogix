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
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCExistingListsBasicView : ScopableTabbableControl
    {
        private bool _supressEvents;

        private IContainer components;

        private LayoutControl lcExistingLists;

        private GroupControl gbxExistingLists;

        private LayoutControlGroup lcgExistingLists;

        private LayoutControlItem lciExistingLists;

        private SimpleButton btnUpdateLists;

        private ToggleSwitch tsModifiedLists;

        private ToggleSwitch tsUpdateLists;

        private TileControl tcExistingLists;

        private TileGroup tgOverwriteLists;

        private TileItem tiOverwriteLists;

        private TileGroup tgPreserveLists;

        private TileItem tiPreserveLists;

        private HelpTipButton helpUpdateLists;

        private HelpTipButton helpModifiedDateLists;

        public bool IsPreserveListsOptionChecked => tiPreserveLists.Checked;

        public int UpdateListOptions { get; private set; }

        public TCExistingListsBasicView()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void btnUpdateLists_Click(object sender, EventArgs e)
        {
            OpenUpdateListDialog();
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
                new OptionsSummary($"{gbxExistingLists.Text} : {(tiOverwriteLists.Checked ? tiOverwriteLists.Text : tiPreserveLists.Text)}", 2)
            };
            if (tiPreserveLists.Checked)
            {
                list.Add(new OptionsSummary($"{tsUpdateLists.Properties.OnText} : {tsUpdateLists.IsOn}", 3));
                list.Add(new OptionsSummary($"{tsModifiedLists.Properties.OnText} : {tsModifiedLists.IsOn}", 3));
            }
            return new ArrayList(list);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCExistingListsBasicView));
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.lcExistingLists = new DevExpress.XtraLayout.LayoutControl();
            this.gbxExistingLists = new DevExpress.XtraEditors.GroupControl();
            this.helpModifiedDateLists = new TooltipsTest.HelpTipButton();
            this.helpUpdateLists = new TooltipsTest.HelpTipButton();
            this.btnUpdateLists = new DevExpress.XtraEditors.SimpleButton();
            this.tsModifiedLists = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsUpdateLists = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcExistingLists = new DevExpress.XtraEditors.TileControl();
            this.tgOverwriteLists = new DevExpress.XtraEditors.TileGroup();
            this.tiOverwriteLists = new DevExpress.XtraEditors.TileItem();
            this.tgPreserveLists = new DevExpress.XtraEditors.TileGroup();
            this.tiPreserveLists = new DevExpress.XtraEditors.TileItem();
            this.lcgExistingLists = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExistingLists = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)this.lcExistingLists).BeginInit();
            this.lcExistingLists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingLists).BeginInit();
            this.gbxExistingLists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpModifiedDateLists).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateLists).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsModifiedLists.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateLists.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingLists).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingLists).BeginInit();
            base.SuspendLayout();
            this.lcExistingLists.Controls.Add(this.gbxExistingLists);
            this.lcExistingLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcExistingLists.Location = new System.Drawing.Point(0, 0);
            this.lcExistingLists.LookAndFeel.SkinName = "Metalogix 2017";
            this.lcExistingLists.Name = "lcExistingLists";
            this.lcExistingLists.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(565, 291, 250, 350);
            this.lcExistingLists.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcExistingLists.Root = this.lcgExistingLists;
            this.lcExistingLists.Size = new System.Drawing.Size(488, 165);
            this.lcExistingLists.TabIndex = 0;
            this.lcExistingLists.TabStop = false;
            this.lcExistingLists.Text = "layoutControl1";
            this.gbxExistingLists.Appearance.BackColor = System.Drawing.Color.White;
            this.gbxExistingLists.Appearance.Options.UseBackColor = true;
            this.gbxExistingLists.AppearanceCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.gbxExistingLists.AppearanceCaption.Options.UseFont = true;
            this.gbxExistingLists.Controls.Add(this.helpModifiedDateLists);
            this.gbxExistingLists.Controls.Add(this.helpUpdateLists);
            this.gbxExistingLists.Controls.Add(this.btnUpdateLists);
            this.gbxExistingLists.Controls.Add(this.tsModifiedLists);
            this.gbxExistingLists.Controls.Add(this.tsUpdateLists);
            this.gbxExistingLists.Controls.Add(this.tcExistingLists);
            this.gbxExistingLists.Location = new System.Drawing.Point(5, 5);
            this.gbxExistingLists.Name = "gbxExistingLists";
            this.gbxExistingLists.Size = new System.Drawing.Size(478, 155);
            this.gbxExistingLists.TabIndex = 0;
            this.gbxExistingLists.Text = "Existing Lists";
            this.helpModifiedDateLists.AnchoringControl = null;
            this.helpModifiedDateLists.BackColor = System.Drawing.Color.Transparent;
            this.helpModifiedDateLists.CommonParentControl = null;
            this.helpModifiedDateLists.Image = (System.Drawing.Image)resources.GetObject("helpModifiedDateLists.Image");
            this.helpModifiedDateLists.Location = new System.Drawing.Point(434, 125);
            this.helpModifiedDateLists.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpModifiedDateLists.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpModifiedDateLists.Name = "helpModifiedDateLists";
            this.helpModifiedDateLists.RealOffset = null;
            this.helpModifiedDateLists.RelativeOffset = null;
            this.helpModifiedDateLists.Size = new System.Drawing.Size(20, 20);
            this.helpModifiedDateLists.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpModifiedDateLists.TabIndex = 4;
            this.helpModifiedDateLists.TabStop = false;
            this.helpUpdateLists.AnchoringControl = null;
            this.helpUpdateLists.BackColor = System.Drawing.Color.Transparent;
            this.helpUpdateLists.CommonParentControl = null;
            this.helpUpdateLists.Image = (System.Drawing.Image)resources.GetObject("helpUpdateLists.Image");
            this.helpUpdateLists.Location = new System.Drawing.Point(386, 95);
            this.helpUpdateLists.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateLists.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateLists.Name = "helpUpdateLists";
            this.helpUpdateLists.RealOffset = null;
            this.helpUpdateLists.RelativeOffset = null;
            this.helpUpdateLists.Size = new System.Drawing.Size(20, 20);
            this.helpUpdateLists.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpUpdateLists.TabIndex = 3;
            this.helpUpdateLists.TabStop = false;
            this.btnUpdateLists.Enabled = false;
            this.btnUpdateLists.Location = new System.Drawing.Point(355, 93);
            this.btnUpdateLists.Name = "btnUpdateLists";
            this.btnUpdateLists.Size = new System.Drawing.Size(23, 23);
            this.btnUpdateLists.TabIndex = 2;
            this.btnUpdateLists.Text = "...";
            this.btnUpdateLists.Click += new System.EventHandler(btnUpdateLists_Click);
            this.tsModifiedLists.EditValue = true;
            this.tsModifiedLists.Enabled = false;
            this.tsModifiedLists.Location = new System.Drawing.Point(203, 123);
            this.tsModifiedLists.Name = "tsModifiedLists";
            this.tsModifiedLists.Properties.Appearance.Options.UseTextOptions = true;
            this.tsModifiedLists.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsModifiedLists.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tsModifiedLists.Properties.OffText = "Check modified dates for Lists";
            this.tsModifiedLists.Properties.OnText = "Check modified dates for Lists";
            this.tsModifiedLists.Size = new System.Drawing.Size(231, 24);
            this.tsModifiedLists.TabIndex = 3;
            this.tsUpdateLists.Enabled = false;
            this.tsUpdateLists.Location = new System.Drawing.Point(203, 93);
            this.tsUpdateLists.Name = "tsUpdateLists";
            this.tsUpdateLists.Properties.Appearance.Options.UseTextOptions = true;
            this.tsUpdateLists.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsUpdateLists.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tsUpdateLists.Properties.OffText = "Update Lists";
            this.tsUpdateLists.Properties.OnText = "Update Lists";
            this.tsUpdateLists.Size = new System.Drawing.Size(149, 24);
            this.tsUpdateLists.TabIndex = 1;
            this.tsUpdateLists.Toggled += new System.EventHandler(tsUpdateLists_Toggled);
            this.tcExistingLists.AllowDrag = false;
            this.tcExistingLists.AllowDragTilesBetweenGroups = false;
            this.tcExistingLists.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingLists.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingLists.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingLists.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcExistingLists.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcExistingLists.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcExistingLists.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingLists.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcExistingLists.AppearanceItem.Normal.Options.UseFont = true;
            this.tcExistingLists.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcExistingLists.DragSize = new System.Drawing.Size(0, 0);
            this.tcExistingLists.Groups.Add(this.tgOverwriteLists);
            this.tcExistingLists.Groups.Add(this.tgPreserveLists);
            this.tcExistingLists.IndentBetweenGroups = 40;
            this.tcExistingLists.IndentBetweenItems = 30;
            this.tcExistingLists.ItemContentAnimation = DevExpress.XtraEditors.TileItemContentAnimationType.Fade;
            this.tcExistingLists.ItemSize = 50;
            this.tcExistingLists.Location = new System.Drawing.Point(10, 21);
            this.tcExistingLists.MaxId = 3;
            this.tcExistingLists.Name = "tcExistingLists";
            this.tcExistingLists.Padding = new System.Windows.Forms.Padding(12);
            this.tcExistingLists.Size = new System.Drawing.Size(350, 75);
            this.tcExistingLists.TabIndex = 0;
            this.tcExistingLists.Text = "tileControl1";
            this.tgOverwriteLists.Items.Add(this.tiOverwriteLists);
            this.tgOverwriteLists.Name = "tgOverwriteLists";
            this.tiOverwriteLists.Checked = true;
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Overwrite_List;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.Text = "Overwrite Lists";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiOverwriteLists.Elements.Add(tileItemElement);
            this.tiOverwriteLists.Id = 0;
            this.tiOverwriteLists.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiOverwriteLists.Name = "tiOverwriteLists";
            this.tiOverwriteLists.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiOverwriteLists_ItemClick);
            this.tgPreserveLists.Items.Add(this.tiPreserveLists);
            this.tgPreserveLists.Name = "tgPreserveLists";
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Preserve_List;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Preserve Lists";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiPreserveLists.Elements.Add(tileItemElement2);
            this.tiPreserveLists.Id = 1;
            this.tiPreserveLists.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiPreserveLists.Name = "tiPreserveLists";
            this.tiPreserveLists.CheckedChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveLists_CheckedChanged);
            this.tiPreserveLists.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveLists_ItemClick);
            this.lcgExistingLists.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgExistingLists.GroupBordersVisible = false;
            this.lcgExistingLists.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciExistingLists });
            this.lcgExistingLists.Location = new System.Drawing.Point(0, 0);
            this.lcgExistingLists.Name = "Root";
            this.lcgExistingLists.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            this.lcgExistingLists.Size = new System.Drawing.Size(488, 165);
            this.lcgExistingLists.TextVisible = false;
            this.lciExistingLists.Control = this.gbxExistingLists;
            this.lciExistingLists.Location = new System.Drawing.Point(0, 0);
            this.lciExistingLists.Name = "lciExistingLists";
            this.lciExistingLists.Size = new System.Drawing.Size(482, 159);
            this.lciExistingLists.TextSize = new System.Drawing.Size(0, 0);
            this.lciExistingLists.TextVisible = false;
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.lcExistingLists);
            base.Name = "TCExistingListsBasicView";
            base.Size = new System.Drawing.Size(488, 165);
            this.UseTab = true;
            ((System.ComponentModel.ISupportInitialize)this.lcExistingLists).EndInit();
            this.lcExistingLists.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingLists).EndInit();
            this.gbxExistingLists.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpModifiedDateLists).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateLists).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsModifiedLists.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateLists.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingLists).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingLists).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            Type type = GetType();
            helpUpdateLists.SetResourceString(type.FullName + tsUpdateLists.Name, type);
            helpUpdateLists.IsBasicViewHelpIcon = true;
            helpModifiedDateLists.SetResourceString(type.FullName + tsModifiedLists.Name, type);
            helpModifiedDateLists.IsBasicViewHelpIcon = true;
            tiOverwriteLists.Checked = true;
        }

        public void LoadUI(SPMigrationModeOptions migrationModeOptions)
        {
            bool flag = base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.List;
            _supressEvents = true;
            if (!flag)
            {
                tiPreserveLists.Checked = true;
            }
            else
            {
                tiOverwriteLists.Checked = migrationModeOptions.OverwriteLists;
                tiPreserveLists.Checked = !migrationModeOptions.OverwriteLists;
            }
            tsModifiedLists.IsOn = migrationModeOptions.CheckModifiedDatesForLists;
            UpdateListOptions = migrationModeOptions.UpdateListOptionsBitField;
            tsUpdateLists.IsOn = migrationModeOptions.UpdateLists;
            ToggleSwitch toggleSwitch = tsUpdateLists;
            ToggleSwitch toggleSwitch2 = tsModifiedLists;
            bool enabled = (toggleSwitch2.Enabled = tiPreserveLists.Checked);
            toggleSwitch.Enabled = enabled;
            btnUpdateLists.Enabled = tsUpdateLists.IsOn;
            _supressEvents = false;
        }

        private void OpenUpdateListDialog()
        {
            btnUpdateLists.Enabled = tsUpdateLists.IsOn;
            if (tsUpdateLists.IsOn)
            {
                UpdateListOptionsDialogBasicView updateListOptionsDialogBasicView = new UpdateListOptionsDialogBasicView(UpdateListOptions);
                if (updateListOptionsDialogBasicView.ShowDialog() == DialogResult.OK)
                {
                    UpdateListOptions = updateListOptionsDialogBasicView.UpdateListOptions;
                }
                tsUpdateLists.IsOn = UpdateListOptions != 0;
            }
            UpdateEnabledState();
        }

        public void SaveUI(SPMigrationModeOptions migrationModeOptions)
        {
            migrationModeOptions.OverwriteLists = tiOverwriteLists.Checked;
            migrationModeOptions.UpdateLists = tsUpdateLists.IsOn;
            migrationModeOptions.CheckModifiedDatesForLists = tsModifiedLists.IsOn;
            migrationModeOptions.UpdateListOptionsBitField = UpdateListOptions;
        }

        private void tiOverwriteLists_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveLists.Checked = false;
            tiOverwriteLists.Checked = true;
            ToggleSwitch toggleSwitch = tsUpdateLists;
            ToggleSwitch toggleSwitch2 = tsModifiedLists;
            SimpleButton simpleButton = btnUpdateLists;
            bool flag = (simpleButton.Enabled = tiPreserveLists.Checked);
            bool enabled = (toggleSwitch2.Enabled = flag);
            toggleSwitch.Enabled = enabled;
        }

        private void tiPreserveLists_CheckedChanged(object sender, TileItemEventArgs e)
        {
            if (base.ParentForm != null)
            {
                Control[] array = base.ParentForm.Controls.Find("TCMigrationModeOptionsBasicView", searchAllChildren: true);
                if (array.Length != 0 && array[1] is TCMigrationModeOptionsBasicView)
                {
                    TCMigrationModeOptionsBasicView tCMigrationModeOptionsBasicView = (TCMigrationModeOptionsBasicView)array[1];
                    tCMigrationModeOptionsBasicView.AdvancedOptions.UpdateTableControls(e.Item.Checked, isExistingSite: false);
                    int size = ((base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site) ? (e.Item.Checked ? 62 : 36) : (e.Item.Checked ? 85 : 57));
                    tCMigrationModeOptionsBasicView.ChangeSize(size);
                }
            }
        }

        private void tiPreserveLists_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveLists.Checked = true;
            tiOverwriteLists.Checked = false;
            ToggleSwitch toggleSwitch = tsUpdateLists;
            ToggleSwitch toggleSwitch2 = tsModifiedLists;
            bool enabled = (toggleSwitch2.Enabled = tiPreserveLists.Checked);
            toggleSwitch.Enabled = enabled;
            btnUpdateLists.Enabled = tsUpdateLists.IsOn;
        }

        private void tsUpdateLists_Toggled(object sender, EventArgs e)
        {
            if (!_supressEvents)
            {
                OpenUpdateListDialog();
            }
        }
    }
}
