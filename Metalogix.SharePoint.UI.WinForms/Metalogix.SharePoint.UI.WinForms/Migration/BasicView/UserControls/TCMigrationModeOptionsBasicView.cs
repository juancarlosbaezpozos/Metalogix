using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MigrationMode.png")]
    [ControlName("Migration Mode")]
    public class TCMigrationModeOptionsBasicView : MigrationModeScopableTabbableControl
    {
        public TCMigrationModeAdvancedOptionsBasicView AdvancedOptions = new TCMigrationModeAdvancedOptionsBasicView();

        private SPMigrationModeOptions _options;

        private IContainer components;

        private TableLayoutPanel tlpMigrationModeOptions;

        private TileControl tcMigrationModes;

        private TileGroup tgFullCopy;

        private TileItem tiFullCopy;

        private TileGroup tgIncrementalCopy;

        private TileItem tiIncrementalCopy;

        private TileGroup tgCustomCopy;

        private TileItem tiCustomCopy;

        private TileItemElement tileItemElement1;

        private TileItemElement tileItemElement2;

        private TileItemElement tileItemElement3;

        public SPMigrationModeOptions Options
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

        public TCMigrationModeOptionsBasicView()
        {
            InitializeComponent();
        }

        public void ChangeSize(int size)
        {
            tlpMigrationModeOptions.RowStyles[0].SizeType = SizeType.Absolute;
            tlpMigrationModeOptions.RowStyles[1].SizeType = SizeType.Absolute;
            tlpMigrationModeOptions.RowStyles[0].Height = 0f;
            tlpMigrationModeOptions.RowStyles[1].Height = 96f;
            tlpMigrationModeOptions.RowStyles[2].Height = size;
            tlpMigrationModeOptions.RowStyles[3].Height = 85 - size;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EnableControlState()
        {
            if (!tiCustomCopy.Checked)
            {
                tcMigrationModes.ItemSize = 130;
                TileItem tileItem = tiFullCopy;
                TileItem tileItem2 = tiIncrementalCopy;
                int num = 2;
                TileItemSize tileItemSize = (TileItemSize)num;
                tiCustomCopy.ItemSize = (TileItemSize)num;
                TileItemSize itemSize = (tileItem2.ItemSize = tileItemSize);
                tileItem.ItemSize = itemSize;
                tiFullCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Default;
                tiIncrementalCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Default;
                tiCustomCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Default;
                tlpMigrationModeOptions.RowStyles[0].SizeType = SizeType.Percent;
                tlpMigrationModeOptions.RowStyles[1].SizeType = SizeType.Percent;
                tlpMigrationModeOptions.RowStyles[0].Height = 36.26f;
                tlpMigrationModeOptions.RowStyles[1].Height = 27.47f;
                tlpMigrationModeOptions.RowStyles[2].Height = 36.26f;
                tlpMigrationModeOptions.RowStyles[3].Height = 0f;
                tlpMigrationModeOptions.Controls.Remove(tlpMigrationModeOptions.GetControlFromPosition(0, 2));
            }
            else
            {
                tcMigrationModes.ItemSize = 75;
                TileItem tileItem3 = tiFullCopy;
                TileItem tileItem4 = tiIncrementalCopy;
                int num2 = 3;
                TileItemSize tileItemSize3 = (TileItemSize)num2;
                tiCustomCopy.ItemSize = (TileItemSize)num2;
                TileItemSize itemSize2 = (tileItem4.ItemSize = tileItemSize3);
                tileItem3.ItemSize = itemSize2;
                tiFullCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Left;
                tiIncrementalCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Left;
                tiCustomCopy.ImageToTextAlignment = TileControlImageToTextAlignment.Left;
                tiFullCopy.Checked = false;
                tiIncrementalCopy.Checked = false;
                AdvancedOptions.Dock = DockStyle.Fill;
                tlpMigrationModeOptions.Controls.Add(AdvancedOptions, 0, 2);
                tlpMigrationModeOptions.RowStyles[0].Height = 0f;
                tlpMigrationModeOptions.RowStyles[1].Height = 15f;
                switch (base.Scope)
                {
                    case SharePointObjectScope.SiteCollection:
                    case SharePointObjectScope.Site:
                        {
                            int size = ((!AdvancedOptions.IsPreserveListsOptionChecked) ? (AdvancedOptions.IsPreserveSitesOptionChecked ? 57 : 32) : 85);
                            ChangeSize(size);
                            break;
                        }
                    case SharePointObjectScope.List:
                        ChangeSize(AdvancedOptions.IsPreserveListsOptionChecked ? 62 : 36);
                        break;
                    case SharePointObjectScope.Folder:
                        ChangeSize(36);
                        break;
                    case SharePointObjectScope.Item:
                        ChangeSize(36);
                        break;
                }
            }
            MigrationModeOptionsChanged();
            tcMigrationModes.Refresh();
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            string text = ((!tiFullCopy.Checked) ? (tiIncrementalCopy.Checked ? tiIncrementalCopy.Text : tiCustomCopy.Text) : tiFullCopy.Text);
            string text2 = text;
            ArrayList arrayList = new ArrayList();
            arrayList.Add(new OptionsSummary("Migration Mode", 0));
            arrayList.Add(new OptionsSummary(text2, 1));
            if (tiCustomCopy.Checked)
            {
                arrayList.AddRange(AdvancedOptions.GetSummaryScreenDetails());
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            this.tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            this.tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
            this.tlpMigrationModeOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tcMigrationModes = new DevExpress.XtraEditors.TileControl();
            this.tgFullCopy = new DevExpress.XtraEditors.TileGroup();
            this.tiFullCopy = new DevExpress.XtraEditors.TileItem();
            this.tgIncrementalCopy = new DevExpress.XtraEditors.TileGroup();
            this.tiIncrementalCopy = new DevExpress.XtraEditors.TileItem();
            this.tgCustomCopy = new DevExpress.XtraEditors.TileGroup();
            this.tiCustomCopy = new DevExpress.XtraEditors.TileItem();
            this.tlpMigrationModeOptions.SuspendLayout();
            base.SuspendLayout();
            this.tlpMigrationModeOptions.ColumnCount = 1;
            this.tlpMigrationModeOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpMigrationModeOptions.Controls.Add(this.tcMigrationModes, 0, 1);
            this.tlpMigrationModeOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMigrationModeOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpMigrationModeOptions.Name = "tlpMigrationModeOptions";
            this.tlpMigrationModeOptions.RowCount = 4;
            this.tlpMigrationModeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35f));
            this.tlpMigrationModeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
            this.tlpMigrationModeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35f));
            this.tlpMigrationModeOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0f));
            this.tlpMigrationModeOptions.Size = new System.Drawing.Size(550, 642);
            this.tlpMigrationModeOptions.TabIndex = 0;
            this.tcMigrationModes.AllowDrag = false;
            this.tcMigrationModes.AllowDragTilesBetweenGroups = false;
            this.tcMigrationModes.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcMigrationModes.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcMigrationModes.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcMigrationModes.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcMigrationModes.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcMigrationModes.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcMigrationModes.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcMigrationModes.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcMigrationModes.AppearanceItem.Normal.Options.UseFont = true;
            this.tcMigrationModes.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcMigrationModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMigrationModes.DragSize = new System.Drawing.Size(0, 0);
            this.tcMigrationModes.Groups.Add(this.tgFullCopy);
            this.tcMigrationModes.Groups.Add(this.tgIncrementalCopy);
            this.tcMigrationModes.Groups.Add(this.tgCustomCopy);
            this.tcMigrationModes.IndentBetweenGroups = 60;
            this.tcMigrationModes.ItemContentAnimation = DevExpress.XtraEditors.TileItemContentAnimationType.Fade;
            this.tcMigrationModes.ItemSize = 130;
            this.tcMigrationModes.Location = new System.Drawing.Point(3, 227);
            this.tcMigrationModes.MaxId = 11;
            this.tcMigrationModes.Name = "tcMigrationModes";
            this.tcMigrationModes.Size = new System.Drawing.Size(544, 186);
            this.tcMigrationModes.TabIndex = 3;
            this.tcMigrationModes.Text = "tileControl1";
            this.tgFullCopy.Items.Add(this.tiFullCopy);
            this.tgFullCopy.Name = "tgFullCopy";
            this.tiFullCopy.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this.tiFullCopy.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tiFullCopy.AppearanceItem.Normal.Options.UseFont = true;
            this.tiFullCopy.Checked = true;
            this.tileItemElement1.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Full_Copy_Migration_Small;
            this.tileItemElement1.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
            this.tileItemElement1.Text = "Full Copy Migration";
            this.tileItemElement1.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiFullCopy.Elements.Add(this.tileItemElement1);
            this.tiFullCopy.Id = 8;
            this.tiFullCopy.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tiFullCopy.Name = "tiFullCopy";
            this.tiFullCopy.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiFullCopy_ItemClick);
            this.tgIncrementalCopy.Items.Add(this.tiIncrementalCopy);
            this.tgIncrementalCopy.Name = "tgIncrementalCopy";
            this.tiIncrementalCopy.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tiIncrementalCopy.AppearanceItem.Normal.Options.UseFont = true;
            this.tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Incremental_Migration_Small;
            this.tileItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
            this.tileItemElement2.Text = "Incremental Migration";
            this.tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiIncrementalCopy.Elements.Add(this.tileItemElement2);
            this.tiIncrementalCopy.Id = 9;
            this.tiIncrementalCopy.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tiIncrementalCopy.Name = "tiIncrementalCopy";
            this.tiIncrementalCopy.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiIncrementalCopy_ItemClick);
            this.tgCustomCopy.Items.Add(this.tiCustomCopy);
            this.tgCustomCopy.Name = "tgCustomCopy";
            this.tiCustomCopy.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tiCustomCopy.AppearanceItem.Normal.Options.UseFont = true;
            this.tileItemElement3.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Custom_Copy_Migration_Small;
            this.tileItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
            this.tileItemElement3.Text = "Custom Copy Migration";
            this.tileItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiCustomCopy.Elements.Add(this.tileItemElement3);
            this.tiCustomCopy.Id = 10;
            this.tiCustomCopy.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tiCustomCopy.Name = "tiCustomCopy";
            this.tiCustomCopy.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiCustomCopy_ItemClick);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            base.Controls.Add(this.tlpMigrationModeOptions);
            base.Name = "TCMigrationModeOptionsBasicView";
            base.Size = new System.Drawing.Size(550, 642);
            this.tlpMigrationModeOptions.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        protected override void LoadUI()
        {
            LoadIncrementalMode(Options);
            tiFullCopy.Checked = Options.MigrationMode == MigrationMode.Full;
            tiIncrementalCopy.Checked = Options.MigrationMode == MigrationMode.Incremental;
            tiCustomCopy.Checked = Options.MigrationMode == MigrationMode.Custom || Options.MigrationMode == MigrationMode.BackwardsCompatibility;
            EnableControlState();
            if (tiCustomCopy.Checked)
            {
                AdvancedOptions.LoadUI(Options);
            }
            UpdateEnabledState();
        }

        private void MigrationModeOptionsChanged()
        {
            if (!(base.ParentForm is ScopableLeftNavigableTabsFormBasicView))
            {
                return;
            }
            ScopableLeftNavigableTabsFormBasicView scopableLeftNavigableTabsFormBasicView = (ScopableLeftNavigableTabsFormBasicView)base.ParentForm;
            Control[] array = scopableLeftNavigableTabsFormBasicView.Controls.Find("TCMigrationOptionsBasicView", searchAllChildren: true);
            if (array.Length > 1 && array[1] is TCMigrationOptionsBasicView)
            {
                TCMigrationOptionsBasicView tCMigrationOptionsBasicView = (TCMigrationOptionsBasicView)array[1];
                MigrationMode migrationMode = MigrationMode.Custom;
                if (tiFullCopy.Checked)
                {
                    migrationMode = MigrationMode.Full;
                }
                else if (tiIncrementalCopy.Checked)
                {
                    migrationMode = MigrationMode.Incremental;
                }
                bool flag = migrationMode == MigrationMode.Custom && (AdvancedOptions.OverwriteItems || AdvancedOptions.UpdateItems);
                bool flag2 = flag;
                bool isPreserveListID = migrationMode == MigrationMode.Incremental || (migrationMode == MigrationMode.Custom && flag2);
                tCMigrationOptionsBasicView.IsPreserveListID = isPreserveListID;
                tCMigrationOptionsBasicView.EnableWorkflowSwitch = migrationMode == MigrationMode.Full;
            }
        }

        private void SaveCustomModeSettings()
        {
            Options.MigrationMode = MigrationMode.Custom;
            AdvancedOptions.SaveUI(Options);
        }

        public override bool SaveUI()
        {
            if (tiFullCopy.Checked)
            {
                SaveFullModeSettings(Options);
            }
            else if (tiIncrementalCopy.Checked)
            {
                SaveIncrementalModeSettings(Options);
            }
            else if (tiCustomCopy.Checked)
            {
                SaveCustomModeSettings();
            }
            return true;
        }

        private void tiCustomCopy_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!tiCustomCopy.Checked)
            {
                tiFullCopy.Checked = false;
                tiIncrementalCopy.Checked = false;
                tiCustomCopy.Checked = true;
                EnableControlState();
            }
        }

        private void tiFullCopy_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!tiFullCopy.Checked)
            {
                tiFullCopy.Checked = true;
                tiIncrementalCopy.Checked = false;
                tiCustomCopy.Checked = false;
                EnableControlState();
            }
        }

        private void tiIncrementalCopy_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!tiIncrementalCopy.Checked)
            {
                tiFullCopy.Checked = false;
                tiIncrementalCopy.Checked = true;
                tiCustomCopy.Checked = false;
                EnableControlState();
            }
        }

        protected override void UpdateScope()
        {
            AdvancedOptions.Scope = base.Scope;
        }
    }
}
