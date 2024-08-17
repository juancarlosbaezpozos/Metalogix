using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCMigrationModeAdvancedOptionsBasicView : MigrationModeScopableTabbableControl
    {
        private TCExistingItemsBasicView _tcExistingItemsBasicView = new TCExistingItemsBasicView();

        private TCExistingListsBasicView _tcExistingListsBasicView = new TCExistingListsBasicView();

        private TCExistingSitesBasicView _tcExistingSitesBasicView = new TCExistingSitesBasicView();

        private IContainer components;

        private LayoutControl lcMainAdvancedOptions;

        private GroupControl gbxAdvancedOptions;

        private LayoutControlGroup lcgMainAdvancedOptions;

        private EmptySpaceItem esItem;

        private LayoutControlGroup lcgAdvancedOptions;

        private LayoutControlItem lcItemAdvancedOptions;

        private TableLayoutPanel tlpAdvancedOptions;

        public bool IsPreserveListsOptionChecked => _tcExistingListsBasicView.IsPreserveListsOptionChecked;

        public bool IsPreserveSitesOptionChecked => _tcExistingSitesBasicView.IsPreserveSitesOptionChecked;

        public bool OverwriteItems => _tcExistingItemsBasicView.OverwriteItems;

        public bool UpdateItems => _tcExistingItemsBasicView.UpdateItems;

        public TCMigrationModeAdvancedOptionsBasicView()
        {
            InitializeComponent();
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
            ArrayList arrayList = new ArrayList();
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    arrayList.AddRange(_tcExistingSitesBasicView.GetSummaryScreenDetails());
                    if (IsPreserveSitesOptionChecked)
                    {
                        arrayList.AddRange(_tcExistingListsBasicView.GetSummaryScreenDetails());
                    }
                    if (IsPreserveListsOptionChecked)
                    {
                        arrayList.AddRange(_tcExistingItemsBasicView.GetSummaryScreenDetails());
                    }
                    break;
                case SharePointObjectScope.List:
                    arrayList.AddRange(_tcExistingListsBasicView.GetSummaryScreenDetails());
                    if (IsPreserveListsOptionChecked)
                    {
                        arrayList.AddRange(_tcExistingItemsBasicView.GetSummaryScreenDetails());
                    }
                    break;
                case SharePointObjectScope.Folder:
                case SharePointObjectScope.Item:
                    arrayList.AddRange(_tcExistingItemsBasicView.GetSummaryScreenDetails());
                    break;
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            this.lcMainAdvancedOptions = new DevExpress.XtraLayout.LayoutControl();
            this.gbxAdvancedOptions = new DevExpress.XtraEditors.GroupControl();
            this.tlpAdvancedOptions = new System.Windows.Forms.TableLayoutPanel();
            this.lcgMainAdvancedOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.esItem = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcgAdvancedOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcItemAdvancedOptions = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)this.lcMainAdvancedOptions).BeginInit();
            this.lcMainAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbxAdvancedOptions).BeginInit();
            this.gbxAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.lcgMainAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.esItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcItemAdvancedOptions).BeginInit();
            base.SuspendLayout();
            this.lcMainAdvancedOptions.Controls.Add(this.gbxAdvancedOptions);
            this.lcMainAdvancedOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcMainAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcMainAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.lcMainAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lcMainAdvancedOptions.Name = "lcMainAdvancedOptions";
            this.lcMainAdvancedOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(628, 379, 250, 350);
            this.lcMainAdvancedOptions.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcMainAdvancedOptions.Root = this.lcgMainAdvancedOptions;
            this.lcMainAdvancedOptions.Size = new System.Drawing.Size(510, 590);
            this.lcMainAdvancedOptions.TabIndex = 0;
            this.lcMainAdvancedOptions.Text = "layoutControl1";
            this.gbxAdvancedOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gbxAdvancedOptions.Controls.Add(this.tlpAdvancedOptions);
            this.gbxAdvancedOptions.Location = new System.Drawing.Point(13, 33);
            this.gbxAdvancedOptions.Name = "gbxAdvancedOptions";
            this.gbxAdvancedOptions.ShowCaption = false;
            this.gbxAdvancedOptions.Size = new System.Drawing.Size(484, 526);
            this.gbxAdvancedOptions.TabIndex = 4;
            this.gbxAdvancedOptions.Text = "groupControl1";
            this.tlpAdvancedOptions.ColumnCount = 1;
            this.tlpAdvancedOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpAdvancedOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpAdvancedOptions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAdvancedOptions.MinimumSize = new System.Drawing.Size(482, 100);
            this.tlpAdvancedOptions.Name = "tlpAdvancedOptions";
            this.tlpAdvancedOptions.Size = new System.Drawing.Size(484, 526);
            this.tlpAdvancedOptions.TabIndex = 0;
            this.lcgMainAdvancedOptions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgMainAdvancedOptions.ExpandButtonVisible = true;
            this.lcgMainAdvancedOptions.GroupBordersVisible = false;
            DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this.lcgMainAdvancedOptions.Items;
            DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[2] { this.esItem, this.lcgAdvancedOptions };
            items.AddRange(items2);
            this.lcgMainAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcgMainAdvancedOptions.Name = "lcgMainAdvancedOptions";
            this.lcgMainAdvancedOptions.Size = new System.Drawing.Size(510, 590);
            this.lcgMainAdvancedOptions.TextVisible = false;
            this.esItem.AllowHotTrack = false;
            this.esItem.Location = new System.Drawing.Point(0, 552);
            this.esItem.Name = "esItem";
            this.esItem.Size = new System.Drawing.Size(490, 18);
            this.esItem.TextSize = new System.Drawing.Size(0, 0);
            this.lcgAdvancedOptions.AppearanceGroup.BackColor = System.Drawing.Color.White;
            this.lcgAdvancedOptions.AppearanceGroup.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lcgAdvancedOptions.AppearanceGroup.Options.UseBackColor = true;
            this.lcgAdvancedOptions.AppearanceGroup.Options.UseFont = true;
            this.lcgAdvancedOptions.ExpandButtonVisible = true;
            this.lcgAdvancedOptions.ExpandOnDoubleClick = true;
            this.lcgAdvancedOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lcItemAdvancedOptions });
            this.lcgAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcgAdvancedOptions.Name = "lcgAdvancedOptions";
            this.lcgAdvancedOptions.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgAdvancedOptions.Size = new System.Drawing.Size(490, 552);
            this.lcgAdvancedOptions.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgAdvancedOptions.Text = "Advanced Options";
            this.lcItemAdvancedOptions.Control = this.gbxAdvancedOptions;
            this.lcItemAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcItemAdvancedOptions.Name = "lcItemAdvancedOptions";
            this.lcItemAdvancedOptions.Size = new System.Drawing.Size(488, 530);
            this.lcItemAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lcItemAdvancedOptions.TextVisible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            base.Controls.Add(this.lcMainAdvancedOptions);
            base.Name = "TCMigrationModeAdvancedOptionsBasicView";
            base.Size = new System.Drawing.Size(510, 590);
            ((System.ComponentModel.ISupportInitialize)this.lcMainAdvancedOptions).EndInit();
            this.lcMainAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.gbxAdvancedOptions).EndInit();
            this.gbxAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.lcgMainAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.esItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcItemAdvancedOptions).EndInit();
            base.ResumeLayout(false);
        }

        public void LoadUI(SPMigrationModeOptions migrationModeOptions)
        {
            _tcExistingSitesBasicView.LoadUI(migrationModeOptions);
            _tcExistingListsBasicView.LoadUI(migrationModeOptions);
            _tcExistingItemsBasicView.LoadUI(migrationModeOptions);
        }

        public void SaveUI(SPMigrationModeOptions migrationModeOptions)
        {
            _tcExistingSitesBasicView.SaveUI(migrationModeOptions);
            _tcExistingListsBasicView.SaveUI(migrationModeOptions);
            _tcExistingItemsBasicView.SaveUI(migrationModeOptions);
        }

        protected override void UpdateScope()
        {
            _tcExistingSitesBasicView.Scope = base.Scope;
            _tcExistingListsBasicView.Scope = base.Scope;
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    tlpAdvancedOptions.RowCount = 1;
                    tlpAdvancedOptions.Controls.Add(_tcExistingSitesBasicView, 0, 0);
                    _tcExistingSitesBasicView.Dock = DockStyle.Fill;
                    break;
                case SharePointObjectScope.List:
                    tlpAdvancedOptions.RowCount = 1;
                    tlpAdvancedOptions.Controls.Add(_tcExistingListsBasicView, 0, 0);
                    _tcExistingListsBasicView.Dock = DockStyle.Fill;
                    break;
                case SharePointObjectScope.Folder:
                    tlpAdvancedOptions.RowCount = 1;
                    tlpAdvancedOptions.Controls.Add(_tcExistingItemsBasicView, 0, 0);
                    _tcExistingItemsBasicView.Dock = DockStyle.Fill;
                    break;
                case SharePointObjectScope.Item:
                    tlpAdvancedOptions.RowCount = 1;
                    tlpAdvancedOptions.Controls.Add(_tcExistingItemsBasicView, 0, 0);
                    _tcExistingItemsBasicView.Dock = DockStyle.Fill;
                    break;
            }
        }

        public void UpdateTableControls(bool isChecked, bool isExistingSite)
        {
            if (isChecked)
            {
                if (!isExistingSite)
                {
                    tlpAdvancedOptions.RowCount++;
                    tlpAdvancedOptions.Controls.Add(_tcExistingItemsBasicView, 0, tlpAdvancedOptions.RowCount - 1);
                    _tcExistingItemsBasicView.Dock = DockStyle.Fill;
                    return;
                }
                tlpAdvancedOptions.RowCount++;
                tlpAdvancedOptions.Controls.Add(_tcExistingListsBasicView, 0, tlpAdvancedOptions.RowCount - 1);
                _tcExistingListsBasicView.Dock = DockStyle.Fill;
                if (IsPreserveListsOptionChecked)
                {
                    tlpAdvancedOptions.RowCount++;
                    tlpAdvancedOptions.Controls.Add(_tcExistingItemsBasicView, 0, tlpAdvancedOptions.RowCount - 1);
                    _tcExistingItemsBasicView.Dock = DockStyle.Fill;
                }
            }
            else
            {
                if (tlpAdvancedOptions.RowCount <= 0)
                {
                    return;
                }
                int num = tlpAdvancedOptions.RowCount - 1;
                if (!isExistingSite)
                {
                    tlpAdvancedOptions.Controls.RemoveAt(num);
                    tlpAdvancedOptions.RowCount = num;
                    return;
                }
                tlpAdvancedOptions.Controls.RemoveAt(num);
                tlpAdvancedOptions.RowCount = num;
                if (IsPreserveListsOptionChecked)
                {
                    num--;
                    tlpAdvancedOptions.Controls.RemoveAt(num);
                    tlpAdvancedOptions.RowCount = num;
                }
            }
        }
    }
}