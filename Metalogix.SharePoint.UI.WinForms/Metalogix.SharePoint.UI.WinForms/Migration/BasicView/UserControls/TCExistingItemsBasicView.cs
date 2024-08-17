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
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCExistingItemsBasicView : ScopableTabbableControl
    {
        private bool _supressEvents;

        private IContainer components;

        private LayoutControl lcExistingItems;

        private GroupControl gbxExistingItems;

        private LayoutControlGroup lcgExistingItems;

        private LayoutControlItem lciExistingItems;

        private SimpleButton btnUpdateItems;

        private ToggleSwitch tsModifiedItems;

        private ToggleSwitch tsUpdateItems;

        private TileControl tcExistingItems;

        private TileGroup tgOverwriteItems;

        private TileItem tiOverwriteItems;

        private TileGroup tgPreserveItems;

        private TileItem tiPreserveItems;

        private HelpTipButton helpUpdateItems;

        private HelpTipButton helpModifiedDateItems;

        public bool OverwriteItems => tiOverwriteItems.Checked;

        public int UpdateItemOptions { get; private set; }

        public bool UpdateItems => tsUpdateItems.IsOn;

        public TCExistingItemsBasicView()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void btnUpdateItems_Click(object sender, EventArgs e)
        {
            OpenUpdateItemsDialog();
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
                new OptionsSummary($"{gbxExistingItems.Text} : {(tiOverwriteItems.Checked ? tiOverwriteItems.Text : tiPreserveItems.Text)}", 2)
            };
            if (tiPreserveItems.Checked)
            {
                list.Add(new OptionsSummary($"{tsUpdateItems.Properties.OnText} : {tsUpdateItems.IsOn}", 3));
                list.Add(new OptionsSummary($"{tsModifiedItems.Properties.OnText} : {tsModifiedItems.IsOn}", 3));
            }
            return new ArrayList(list);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCExistingItemsBasicView));
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.lcExistingItems = new DevExpress.XtraLayout.LayoutControl();
            this.gbxExistingItems = new DevExpress.XtraEditors.GroupControl();
            this.helpModifiedDateItems = new TooltipsTest.HelpTipButton();
            this.helpUpdateItems = new TooltipsTest.HelpTipButton();
            this.btnUpdateItems = new DevExpress.XtraEditors.SimpleButton();
            this.tsModifiedItems = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsUpdateItems = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcExistingItems = new DevExpress.XtraEditors.TileControl();
            this.tgOverwriteItems = new DevExpress.XtraEditors.TileGroup();
            this.tiOverwriteItems = new DevExpress.XtraEditors.TileItem();
            this.tgPreserveItems = new DevExpress.XtraEditors.TileGroup();
            this.tiPreserveItems = new DevExpress.XtraEditors.TileItem();
            this.lcgExistingItems = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExistingItems = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)this.lcExistingItems).BeginInit();
            this.lcExistingItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingItems).BeginInit();
            this.gbxExistingItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.helpModifiedDateItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsModifiedItems.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateItems.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingItems).BeginInit();
            base.SuspendLayout();
            this.lcExistingItems.Controls.Add(this.gbxExistingItems);
            this.lcExistingItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcExistingItems.Location = new System.Drawing.Point(0, 0);
            this.lcExistingItems.Name = "lcExistingItems";
            this.lcExistingItems.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(565, 291, 250, 350);
            this.lcExistingItems.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcExistingItems.Root = this.lcgExistingItems;
            this.lcExistingItems.Size = new System.Drawing.Size(550, 165);
            this.lcExistingItems.TabIndex = 0;
            this.lcExistingItems.TabStop = false;
            this.lcExistingItems.Text = "layoutControl1";
            this.gbxExistingItems.Appearance.BackColor = System.Drawing.Color.White;
            this.gbxExistingItems.Appearance.Options.UseBackColor = true;
            this.gbxExistingItems.AppearanceCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.gbxExistingItems.AppearanceCaption.Options.UseFont = true;
            this.gbxExistingItems.Controls.Add(this.helpModifiedDateItems);
            this.gbxExistingItems.Controls.Add(this.helpUpdateItems);
            this.gbxExistingItems.Controls.Add(this.btnUpdateItems);
            this.gbxExistingItems.Controls.Add(this.tsModifiedItems);
            this.gbxExistingItems.Controls.Add(this.tsUpdateItems);
            this.gbxExistingItems.Controls.Add(this.tcExistingItems);
            this.gbxExistingItems.Location = new System.Drawing.Point(5, 5);
            this.gbxExistingItems.Name = "gbxExistingItems";
            this.gbxExistingItems.Size = new System.Drawing.Size(540, 155);
            this.gbxExistingItems.TabIndex = 0;
            this.gbxExistingItems.Text = "Existing Items";
            this.helpModifiedDateItems.AnchoringControl = null;
            this.helpModifiedDateItems.BackColor = System.Drawing.Color.Transparent;
            this.helpModifiedDateItems.CommonParentControl = null;
            this.helpModifiedDateItems.Image = (System.Drawing.Image)resources.GetObject("helpModifiedDateItems.Image");
            this.helpModifiedDateItems.Location = new System.Drawing.Point(494, 125);
            this.helpModifiedDateItems.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpModifiedDateItems.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpModifiedDateItems.Name = "helpModifiedDateItems";
            this.helpModifiedDateItems.RealOffset = null;
            this.helpModifiedDateItems.RelativeOffset = null;
            this.helpModifiedDateItems.Size = new System.Drawing.Size(20, 20);
            this.helpModifiedDateItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpModifiedDateItems.TabIndex = 4;
            this.helpModifiedDateItems.TabStop = false;
            this.helpUpdateItems.AnchoringControl = null;
            this.helpUpdateItems.BackColor = System.Drawing.Color.Transparent;
            this.helpUpdateItems.CommonParentControl = null;
            this.helpUpdateItems.Image = (System.Drawing.Image)resources.GetObject("helpUpdateItems.Image");
            this.helpUpdateItems.Location = new System.Drawing.Point(391, 95);
            this.helpUpdateItems.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateItems.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpUpdateItems.Name = "helpUpdateItems";
            this.helpUpdateItems.RealOffset = null;
            this.helpUpdateItems.RelativeOffset = null;
            this.helpUpdateItems.Size = new System.Drawing.Size(20, 20);
            this.helpUpdateItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpUpdateItems.TabIndex = 3;
            this.helpUpdateItems.TabStop = false;
            this.btnUpdateItems.Enabled = false;
            this.btnUpdateItems.Location = new System.Drawing.Point(361, 93);
            this.btnUpdateItems.Name = "btnUpdateItems";
            this.btnUpdateItems.Size = new System.Drawing.Size(23, 23);
            this.btnUpdateItems.TabIndex = 2;
            this.btnUpdateItems.Text = "...";
            this.btnUpdateItems.Click += new System.EventHandler(btnUpdateItems_Click);
            this.tsModifiedItems.Enabled = false;
            this.tsModifiedItems.Location = new System.Drawing.Point(203, 123);
            this.tsModifiedItems.Name = "tsModifiedItems";
            this.tsModifiedItems.Properties.Appearance.Options.UseTextOptions = true;
            this.tsModifiedItems.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsModifiedItems.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tsModifiedItems.Properties.OffText = "Check modified dates for Items/Documents";
            this.tsModifiedItems.Properties.OnText = "Check modified dates for Items/Documents";
            this.tsModifiedItems.Size = new System.Drawing.Size(294, 24);
            this.tsModifiedItems.TabIndex = 3;
            this.tsModifiedItems.IsOn = true;
            this.tsUpdateItems.Enabled = false;
            this.tsUpdateItems.Location = new System.Drawing.Point(203, 93);
            this.tsUpdateItems.Name = "tsUpdateItems";
            this.tsUpdateItems.Properties.Appearance.Options.UseTextOptions = true;
            this.tsUpdateItems.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsUpdateItems.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tsUpdateItems.Properties.OffText = "Update Items";
            this.tsUpdateItems.Properties.OnText = "Update Items";
            this.tsUpdateItems.Size = new System.Drawing.Size(155, 24);
            this.tsUpdateItems.TabIndex = 1;
            this.tsUpdateItems.Toggled += new System.EventHandler(tsUpdateItems_Toggled);
            this.tcExistingItems.AllowDrag = false;
            this.tcExistingItems.AllowDragTilesBetweenGroups = false;
            this.tcExistingItems.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingItems.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcExistingItems.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingItems.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcExistingItems.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcExistingItems.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcExistingItems.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcExistingItems.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcExistingItems.AppearanceItem.Normal.Options.UseFont = true;
            this.tcExistingItems.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcExistingItems.DragSize = new System.Drawing.Size(0, 0);
            this.tcExistingItems.Groups.Add(this.tgOverwriteItems);
            this.tcExistingItems.Groups.Add(this.tgPreserveItems);
            this.tcExistingItems.IndentBetweenGroups = 40;
            this.tcExistingItems.IndentBetweenItems = 30;
            this.tcExistingItems.ItemContentAnimation = DevExpress.XtraEditors.TileItemContentAnimationType.Fade;
            this.tcExistingItems.ItemSize = 50;
            this.tcExistingItems.Location = new System.Drawing.Point(10, 21);
            this.tcExistingItems.MaxId = 3;
            this.tcExistingItems.Name = "tcExistingItems";
            this.tcExistingItems.Padding = new System.Windows.Forms.Padding(12);
            this.tcExistingItems.Size = new System.Drawing.Size(350, 75);
            this.tcExistingItems.TabIndex = 0;
            this.tcExistingItems.Text = "tileControl1";
            this.tgOverwriteItems.Items.Add(this.tiOverwriteItems);
            this.tgOverwriteItems.Name = "tgOverwriteItems";
            this.tiOverwriteItems.Checked = true;
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Overwrite_Items;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.StretchHorizontal = true;
            tileItemElement.Text = "Overwrite Items";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiOverwriteItems.Elements.Add(tileItemElement);
            this.tiOverwriteItems.Id = 0;
            this.tiOverwriteItems.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiOverwriteItems.Name = "tiOverwriteItems";
            this.tiOverwriteItems.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiOverwriteItems_ItemClick);
            this.tgPreserveItems.Items.Add(this.tiPreserveItems);
            this.tgPreserveItems.Name = "tgPreserveItems";
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Preserve_Items;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Preserve Items";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiPreserveItems.Elements.Add(tileItemElement2);
            this.tiPreserveItems.Id = 1;
            this.tiPreserveItems.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiPreserveItems.Name = "tiPreserveItems";
            this.tiPreserveItems.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveItems_ItemClick);
            this.lcgExistingItems.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgExistingItems.GroupBordersVisible = false;
            this.lcgExistingItems.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciExistingItems });
            this.lcgExistingItems.Location = new System.Drawing.Point(0, 0);
            this.lcgExistingItems.Name = "lcgExistingItems";
            this.lcgExistingItems.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            this.lcgExistingItems.Size = new System.Drawing.Size(550, 165);
            this.lcgExistingItems.TextVisible = false;
            this.lciExistingItems.Control = this.gbxExistingItems;
            this.lciExistingItems.Location = new System.Drawing.Point(0, 0);
            this.lciExistingItems.Name = "lciExistingItems";
            this.lciExistingItems.Size = new System.Drawing.Size(544, 159);
            this.lciExistingItems.TextSize = new System.Drawing.Size(0, 0);
            this.lciExistingItems.TextVisible = false;
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.lcExistingItems);
            base.Name = "TCExistingItemsBasicView";
            base.Size = new System.Drawing.Size(550, 165);
            this.UseTab = true;
            ((System.ComponentModel.ISupportInitialize)this.lcExistingItems).EndInit();
            this.lcExistingItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gbxExistingItems).EndInit();
            this.gbxExistingItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.helpModifiedDateItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpUpdateItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsModifiedItems.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsUpdateItems.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgExistingItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciExistingItems).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            UpdateItemOptions = 0;
            Type type = GetType();
            helpUpdateItems.SetResourceString(type.FullName + tsUpdateItems.Name, type);
            helpUpdateItems.IsBasicViewHelpIcon = true;
            helpModifiedDateItems.SetResourceString(type.FullName + tsModifiedItems.Name, type);
            helpModifiedDateItems.IsBasicViewHelpIcon = true;
            tiOverwriteItems.Checked = true;
        }

        public void LoadUI(SPMigrationModeOptions migrationModeOptions)
        {
            tiOverwriteItems.Checked = migrationModeOptions.ItemCopyingMode == ListItemCopyMode.Overwrite;
            tiPreserveItems.Checked = migrationModeOptions.ItemCopyingMode != ListItemCopyMode.Overwrite;
            tsModifiedItems.IsOn = migrationModeOptions.CheckModifiedDatesForItemsDocuments;
            UpdateItemOptions = migrationModeOptions.UpdateItemOptionsBitField;
            _supressEvents = true;
            tsUpdateItems.IsOn = migrationModeOptions.UpdateItems;
            ToggleSwitch toggleSwitch = tsUpdateItems;
            ToggleSwitch toggleSwitch2 = tsModifiedItems;
            bool enabled = (toggleSwitch2.Enabled = tiPreserveItems.Checked);
            toggleSwitch.Enabled = enabled;
            btnUpdateItems.Enabled = tsUpdateItems.IsOn;
            _supressEvents = false;
        }

        private void OpenUpdateItemsDialog()
        {
            btnUpdateItems.Enabled = tsUpdateItems.IsOn;
            if (tsUpdateItems.IsOn)
            {
                UpdateItemOptionsDialogBasicView updateItemOptionsDialogBasicView = new UpdateItemOptionsDialogBasicView(UpdateItemOptions);
                if (updateItemOptionsDialogBasicView.ShowDialog() == DialogResult.OK)
                {
                    UpdateItemOptions = updateItemOptionsDialogBasicView.UpdateItemOptionsBitField;
                }
                tsUpdateItems.IsOn = UpdateItemOptions != 0;
            }
            UpdateEnabledState();
        }

        public void SaveUI(SPMigrationModeOptions migrationModeOptions)
        {
            migrationModeOptions.ItemCopyingMode = ((!tiOverwriteItems.Checked) ? ListItemCopyMode.Preserve : ListItemCopyMode.Overwrite);
            migrationModeOptions.UpdateItemOptionsBitField = UpdateItemOptions;
            migrationModeOptions.UpdateItems = tsUpdateItems.IsOn;
            migrationModeOptions.CheckModifiedDatesForItemsDocuments = tsModifiedItems.IsOn;
        }

        private void tiOverwriteItems_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveItems.Checked = false;
            tiOverwriteItems.Checked = true;
            ToggleSwitch toggleSwitch = tsUpdateItems;
            ToggleSwitch toggleSwitch2 = tsModifiedItems;
            SimpleButton simpleButton = btnUpdateItems;
            bool flag = (simpleButton.Enabled = tiPreserveItems.Checked);
            bool enabled = (toggleSwitch2.Enabled = flag);
            toggleSwitch.Enabled = enabled;
        }

        private void tiPreserveItems_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveItems.Checked = true;
            tiOverwriteItems.Checked = false;
            ToggleSwitch toggleSwitch = tsUpdateItems;
            ToggleSwitch toggleSwitch2 = tsModifiedItems;
            bool enabled = (toggleSwitch2.Enabled = tiPreserveItems.Checked);
            toggleSwitch.Enabled = enabled;
            btnUpdateItems.Enabled = tsUpdateItems.IsOn;
        }

        private void tsUpdateItems_Toggled(object sender, EventArgs e)
        {
            if (!_supressEvents)
            {
                OpenUpdateItemsDialog();
            }
        }
    }
}
