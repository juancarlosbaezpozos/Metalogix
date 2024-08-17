using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCListContentOptionsBasicView : ListContentScopableTabbableControl
    {
        public delegate void AdvancedOptionsStateChangedHandler(EventArgs args, object sender);

        private SPFolderContentOptions _folderOptions;

        private IContainer components;

        private ToggleSwitch tsCopyAllLists;

        private TileControl tcVersions;

        private TileGroup tgAllVersions;

        private TileItem tiAllVersions;

        private TileGroup tgNoVersions;

        private TileItem tiNoVersions;

        private TileItem tiXVersions;

        private TileGroup tgXVersions;

        private TCListContentAdvancedOptionsBasicView tcListContentAdvancedOptions;

        private ToggleSwitch tsCopyListItems;

        private ToggleSwitch tsCopyItemsDocuments;

        private TCAzureOptionsBasicView tcAzureOptionsBasicView;

        private SpinEdit spinXVersions;

        private HelpTipButton helpCopyListItems;

        private HelpTipButton helpCopyAllLists;

        private HelpTipButton helpCopyItemsDocuments;

        public SPFolderContentOptions FolderOptions
        {
            get
            {
                return _folderOptions;
            }
            set
            {
                _folderOptions = value;
                LoadFolderOptions(_folderOptions);
            }
        }

        public bool HasOnlyExternalLists { get; set; }

        public bool IsPreserveListID
        {
            set
            {
                MigrationModeChanged(value);
            }
        }

        public bool IsSetExplicitOptions { get; set; }

        public event TCMigrationOptionsBasicView.AdvancedOptionsMainControlStateChangedHandler AdvancedOptionsMainControlStateChanged;

        public event TCMigrationOptionsBasicView.UpdateOptionsVisibilityHandler OptionsVisibilityChanged;

        public event TCMigrationOptionsBasicView.UpdatePermissionsStateHandler UpdatePermissionsStateChanged;

        public TCListContentOptionsBasicView()
        {
            InitializeComponent();
            InitializeFormControls();
        }

        private void CopyListOrItemsVisibilityChanged(bool? isListEnbaled, bool? isItemEnbaled)
        {
            if (this.OptionsVisibilityChanged != null)
            {
                this.OptionsVisibilityChanged(new UpdateOptionsStateEventArgs(null, isListEnbaled, isItemEnbaled), null);
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

        private bool GetEnabledState()
        {
            bool result = false;
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    result = tsCopyAllLists.IsOn && tsCopyAllLists.Enabled;
                    break;
                case SharePointObjectScope.List:
                case SharePointObjectScope.Folder:
                    result = tsCopyListItems.IsOn && tsCopyListItems.Enabled;
                    break;
                case SharePointObjectScope.Item:
                    result = tsCopyItemsDocuments.IsOn && tsCopyItemsDocuments.Enabled;
                    break;
            }
            return result;
        }

        private ArrayList GetItemLevelSummary()
        {
            ArrayList arrayList = new ArrayList();
            string text = ((!tiAllVersions.Checked) ? (tiNoVersions.Checked ? tiNoVersions.Text : $"Copy {spinXVersions.Value} Version(s)") : tiAllVersions.Text);
            string arg = text;
            if (!tsCopyItemsDocuments.IsOn)
            {
                arrayList.Add(new OptionsSummary($"{tsCopyItemsDocuments.Properties.OnText} : {tsCopyItemsDocuments.IsOn}", 1));
            }
            else
            {
                arrayList.Add(new OptionsSummary($"{tsCopyItemsDocuments.Properties.OnText} : {arg}", 1));
                arrayList.AddRange(tcAzureOptionsBasicView.GetAzureOptionsSummaryDetails());
                arrayList.AddRange(tcListContentAdvancedOptions.GetAdvancedOptionsSummary());
            }
            return arrayList;
        }

        private ArrayList GetListAndFolderLevelSummary()
        {
            ArrayList arrayList = new ArrayList();
            string text = ((!tiAllVersions.Checked) ? (tiNoVersions.Checked ? tiNoVersions.Text : $"Copy {spinXVersions.Value} Version(s)") : tiAllVersions.Text);
            string arg = text;
            if (!tsCopyListItems.IsOn)
            {
                arrayList.Add(new OptionsSummary($"{tsCopyListItems.Properties.OnText} : {tsCopyListItems.IsOn}", 1));
            }
            else
            {
                arrayList.Add(new OptionsSummary($"{tsCopyListItems.Properties.OnText} : {arg}", 1));
                arrayList.AddRange(tcAzureOptionsBasicView.GetAzureOptionsSummaryDetails());
                arrayList.AddRange(tcListContentAdvancedOptions.GetAdvancedOptionsSummary());
            }
            return arrayList;
        }

        private ArrayList GetSiteLevelSummary()
        {
            ArrayList arrayList = new ArrayList();
            string text = ((!tiAllVersions.Checked) ? (tiNoVersions.Checked ? tiNoVersions.Text : $"Copy {spinXVersions.Value} Version(s)") : tiAllVersions.Text);
            string arg = text;
            if (!tsCopyAllLists.IsOn)
            {
                arrayList.Add(new OptionsSummary($"{tsCopyAllLists.Properties.OnText} : {tsCopyAllLists.IsOn}", 1));
            }
            else
            {
                arrayList.Add(new OptionsSummary($"{tsCopyAllLists.Properties.OnText} : {arg}", 1));
                arrayList.AddRange(tcAzureOptionsBasicView.GetAzureOptionsSummaryDetails());
                arrayList.AddRange(tcListContentAdvancedOptions.GetAdvancedOptionsSummary());
            }
            return arrayList;
        }

        private string GetSummaryHeader()
        {
            string result = string.Empty;
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                case SharePointObjectScope.List:
                case SharePointObjectScope.Item:
                    result = "List Content Options";
                    break;
                case SharePointObjectScope.Folder:
                    result = "Folder Content Options";
                    break;
            }
            return result;
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            string summaryHeader = GetSummaryHeader();
            arrayList.Add(new OptionsSummary(summaryHeader, 0));
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    arrayList.AddRange(GetSiteLevelSummary());
                    break;
                case SharePointObjectScope.List:
                case SharePointObjectScope.Folder:
                    arrayList.AddRange(GetListAndFolderLevelSummary());
                    break;
                case SharePointObjectScope.Item:
                    arrayList.AddRange(GetItemLevelSummary());
                    break;
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCListContentOptionsBasicView));
            this.tsCopyAllLists = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcVersions = new DevExpress.XtraEditors.TileControl();
            this.tgAllVersions = new DevExpress.XtraEditors.TileGroup();
            this.tiAllVersions = new DevExpress.XtraEditors.TileItem();
            this.tgNoVersions = new DevExpress.XtraEditors.TileGroup();
            this.tiNoVersions = new DevExpress.XtraEditors.TileItem();
            this.tgXVersions = new DevExpress.XtraEditors.TileGroup();
            this.tiXVersions = new DevExpress.XtraEditors.TileItem();
            this.tsCopyListItems = new DevExpress.XtraEditors.ToggleSwitch();
            this.tcAzureOptionsBasicView = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCAzureOptionsBasicView();
            this.tcListContentAdvancedOptions = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCListContentAdvancedOptionsBasicView();
            this.spinXVersions = new DevExpress.XtraEditors.SpinEdit();
            this.tsCopyItemsDocuments = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpCopyListItems = new TooltipsTest.HelpTipButton();
            this.helpCopyAllLists = new TooltipsTest.HelpTipButton();
            this.helpCopyItemsDocuments = new TooltipsTest.HelpTipButton();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyAllLists.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListItems.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.spinXVersions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyItemsDocuments.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListItems).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyAllLists).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyItemsDocuments).BeginInit();
            base.SuspendLayout();
            this.tsCopyAllLists.Location = new System.Drawing.Point(10, 20);
            this.tsCopyAllLists.Name = "tsCopyAllLists";
            this.tsCopyAllLists.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyAllLists.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyAllLists.Properties.OffText = "Copy all Lists and Libraries";
            this.tsCopyAllLists.Properties.OnText = "Copy all Lists and Libraries";
            this.tsCopyAllLists.Size = new System.Drawing.Size(208, 24);
            this.tsCopyAllLists.TabIndex = 0;
            this.tsCopyAllLists.Toggled += new System.EventHandler(tsCopyAllLists_Toggled);
            this.tcVersions.AllowDrag = false;
            this.tcVersions.AllowDragTilesBetweenGroups = false;
            this.tcVersions.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcVersions.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcVersions.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcVersions.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcVersions.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcVersions.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcVersions.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcVersions.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcVersions.AppearanceItem.Normal.Options.UseFont = true;
            this.tcVersions.AppearanceItem.Pressed.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcVersions.AppearanceItem.Pressed.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcVersions.AppearanceItem.Pressed.Options.UseBackColor = true;
            this.tcVersions.AppearanceItem.Pressed.Options.UseFont = true;
            this.tcVersions.BackColor = System.Drawing.Color.White;
            this.tcVersions.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcVersions.DragSize = new System.Drawing.Size(0, 0);
            this.tcVersions.Groups.Add(this.tgAllVersions);
            this.tcVersions.Groups.Add(this.tgNoVersions);
            this.tcVersions.Groups.Add(this.tgXVersions);
            this.tcVersions.IndentBetweenGroups = 24;
            this.tcVersions.IndentBetweenItems = 35;
            this.tcVersions.ItemSize = 56;
            this.tcVersions.Location = new System.Drawing.Point(56, 36);
            this.tcVersions.MaxId = 10;
            this.tcVersions.Name = "tcVersions";
            this.tcVersions.Size = new System.Drawing.Size(531, 98);
            this.tcVersions.TabIndex = 1;
            this.tcVersions.Text = "tileControl1";
            this.tgAllVersions.Items.Add(this.tiAllVersions);
            this.tgAllVersions.Name = "tgAllVersions";
            this.tgAllVersions.Text = "Copy all Versions";
            this.tiAllVersions.Checked = true;
            tileItemElement.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            tileItemElement.Appearance.Normal.Options.UseFont = true;
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Copy_all_versions;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.Text = "Copy all Versions";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiAllVersions.Elements.Add(tileItemElement);
            this.tiAllVersions.Id = 6;
            this.tiAllVersions.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiAllVersions.Name = "tiAllVersions";
            this.tiAllVersions.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiAllVersions_ItemClick);
            this.tgNoVersions.Items.Add(this.tiNoVersions);
            this.tgNoVersions.Name = "tgNoVersions";
            tileItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            tileItemElement2.Appearance.Normal.Options.UseFont = true;
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Do_not_copy_versions;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Do not Copy Versions";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiNoVersions.Elements.Add(tileItemElement2);
            this.tiNoVersions.Id = 7;
            this.tiNoVersions.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiNoVersions.Name = "tiNoVersions";
            this.tiNoVersions.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiNoVersions_ItemClick);
            this.tgXVersions.Items.Add(this.tiXVersions);
            this.tgXVersions.Name = "tgXVersions";
            tileItemElement3.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            tileItemElement3.Appearance.Normal.Options.UseFont = true;
            tileItemElement3.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Copy_x_number_of_versions;
            tileItemElement3.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement3.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement3.Text = "Copy X Versions";
            tileItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiXVersions.Elements.Add(tileItemElement3);
            this.tiXVersions.Id = 8;
            this.tiXVersions.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiXVersions.Name = "tiXVersions";
            this.tiXVersions.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiXVersions_ItemClick);
            this.tsCopyListItems.EditValue = true;
            this.tsCopyListItems.Location = new System.Drawing.Point(10, 20);
            this.tsCopyListItems.Name = "tsCopyListItems";
            this.tsCopyListItems.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyListItems.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyListItems.Properties.OffText = "Copy List Items and Documents";
            this.tsCopyListItems.Properties.OnText = "Copy List Items and Documents";
            this.tsCopyListItems.Size = new System.Drawing.Size(237, 24);
            this.tsCopyListItems.TabIndex = 0;
            this.tsCopyListItems.Toggled += new System.EventHandler(tsCopyListItems_Toggled);
            this.tcAzureOptionsBasicView.Context = null;
            this.tcAzureOptionsBasicView.ControlName = null;
            this.tcAzureOptionsBasicView.FoldersAvailable = true;
            this.tcAzureOptionsBasicView.Image = null;
            this.tcAzureOptionsBasicView.ImageName = null;
            this.tcAzureOptionsBasicView.ItemsAvailable = true;
            this.tcAzureOptionsBasicView.ListsAvailable = true;
            this.tcAzureOptionsBasicView.Location = new System.Drawing.Point(70, 119);
            this.tcAzureOptionsBasicView.MultiSelectUI = false;
            this.tcAzureOptionsBasicView.Name = "tcAzureOptionsBasicView";
            this.tcAzureOptionsBasicView.ParentTabbableControl = null;
            this.tcAzureOptionsBasicView.Scope = Metalogix.SharePoint.SharePointObjectScope.SiteCollection;
            this.tcAzureOptionsBasicView.SiteCollectionsAvailable = true;
            this.tcAzureOptionsBasicView.SitesAvailable = true;
            this.tcAzureOptionsBasicView.Size = new System.Drawing.Size(350, 60);
            this.tcAzureOptionsBasicView.SourceNodes = null;
            this.tcAzureOptionsBasicView.TabIndex = 3;
            this.tcAzureOptionsBasicView.TargetNodes = null;
            this.tcAzureOptionsBasicView.UseTab = false;
            this.tcListContentAdvancedOptions.Context = null;
            this.tcListContentAdvancedOptions.ControlName = null;
            this.tcListContentAdvancedOptions.Image = null;
            this.tcListContentAdvancedOptions.ImageName = null;
            this.tcListContentAdvancedOptions.Location = new System.Drawing.Point(74, 188);
            this.tcListContentAdvancedOptions.MultiSelectUI = false;
            this.tcListContentAdvancedOptions.Name = "tcListContentAdvancedOptions";
            this.tcListContentAdvancedOptions.ParentTabbableControl = null;
            this.tcListContentAdvancedOptions.Size = new System.Drawing.Size(494, 159);
            this.tcListContentAdvancedOptions.TabIndex = 4;
            this.tcListContentAdvancedOptions.UseTab = false;
            DevExpress.XtraEditors.SpinEdit spinEdit = this.spinXVersions;
            int[] bits = new int[4] { 1, 0, 0, 0 };
            spinEdit.EditValue = new decimal(bits);
            this.spinXVersions.Enabled = false;
            this.spinXVersions.Location = new System.Drawing.Point(575, 74);
            this.spinXVersions.Name = "spinXVersions";
            this.spinXVersions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.spinXVersions.Properties.IsFloatValue = false;
            this.spinXVersions.Properties.Mask.EditMask = "d";
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.spinXVersions.Properties;
            int[] bits2 = new int[4] { 2147483647, 0, 0, 0 };
            properties.MaxValue = new decimal(bits2);
            DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.spinXVersions.Properties;
            int[] bits3 = new int[4] { 1, 0, 0, 0 };
            properties2.MinValue = new decimal(bits3);
            this.spinXVersions.Size = new System.Drawing.Size(54, 20);
            this.spinXVersions.TabIndex = 2;
            this.tsCopyItemsDocuments.EditValue = true;
            this.tsCopyItemsDocuments.Location = new System.Drawing.Point(10, 20);
            this.tsCopyItemsDocuments.Name = "tsCopyItemsDocuments";
            this.tsCopyItemsDocuments.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyItemsDocuments.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyItemsDocuments.Properties.OffText = "Copy Versions for Items and Documents";
            this.tsCopyItemsDocuments.Properties.OnText = "Copy Versions for Items and Documents";
            this.tsCopyItemsDocuments.Size = new System.Drawing.Size(273, 24);
            this.tsCopyItemsDocuments.TabIndex = 0;
            this.tsCopyItemsDocuments.Toggled += new System.EventHandler(tsCopyItemsDocuments_Toggled);
            this.helpCopyListItems.AnchoringControl = null;
            this.helpCopyListItems.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyListItems.CommonParentControl = null;
            this.helpCopyListItems.Image = (System.Drawing.Image)resources.GetObject("helpCopyListItems.Image");
            this.helpCopyListItems.Location = new System.Drawing.Point(246, 22);
            this.helpCopyListItems.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListItems.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyListItems.Name = "helpCopyListItems";
            this.helpCopyListItems.RealOffset = null;
            this.helpCopyListItems.RelativeOffset = null;
            this.helpCopyListItems.Size = new System.Drawing.Size(20, 20);
            this.helpCopyListItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyListItems.TabIndex = 4;
            this.helpCopyListItems.TabStop = false;
            this.helpCopyAllLists.AnchoringControl = null;
            this.helpCopyAllLists.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyAllLists.CommonParentControl = null;
            this.helpCopyAllLists.Image = (System.Drawing.Image)resources.GetObject("helpCopyAllLists.Image");
            this.helpCopyAllLists.Location = new System.Drawing.Point(217, 22);
            this.helpCopyAllLists.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyAllLists.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyAllLists.Name = "helpCopyAllLists";
            this.helpCopyAllLists.RealOffset = null;
            this.helpCopyAllLists.RelativeOffset = null;
            this.helpCopyAllLists.Size = new System.Drawing.Size(20, 20);
            this.helpCopyAllLists.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyAllLists.TabIndex = 9;
            this.helpCopyAllLists.TabStop = false;
            this.helpCopyItemsDocuments.AnchoringControl = null;
            this.helpCopyItemsDocuments.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyItemsDocuments.CommonParentControl = null;
            this.helpCopyItemsDocuments.Image = (System.Drawing.Image)resources.GetObject("helpCopyItemsDocuments.Image");
            this.helpCopyItemsDocuments.Location = new System.Drawing.Point(283, 22);
            this.helpCopyItemsDocuments.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyItemsDocuments.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyItemsDocuments.Name = "helpCopyItemsDocuments";
            this.helpCopyItemsDocuments.RealOffset = null;
            this.helpCopyItemsDocuments.RelativeOffset = null;
            this.helpCopyItemsDocuments.Size = new System.Drawing.Size(20, 20);
            this.helpCopyItemsDocuments.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyItemsDocuments.TabIndex = 10;
            this.helpCopyItemsDocuments.TabStop = false;
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.helpCopyItemsDocuments);
            base.Controls.Add(this.helpCopyAllLists);
            base.Controls.Add(this.helpCopyListItems);
            base.Controls.Add(this.spinXVersions);
            base.Controls.Add(this.tcAzureOptionsBasicView);
            base.Controls.Add(this.tsCopyListItems);
            base.Controls.Add(this.tcListContentAdvancedOptions);
            base.Controls.Add(this.tcVersions);
            base.Controls.Add(this.tsCopyAllLists);
            base.Controls.Add(this.tsCopyItemsDocuments);
            base.Name = "TCListContentOptionsBasicView";
            base.Size = new System.Drawing.Size(621, 337);
            this.UseTab = true;
            ((System.ComponentModel.ISupportInitialize)this.tsCopyAllLists.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyListItems.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.spinXVersions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyItemsDocuments.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyListItems).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyAllLists).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyItemsDocuments).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeFormControls()
        {
            Type type = GetType();
            helpCopyListItems.SetResourceString(type.FullName + tsCopyListItems.Name, type);
            helpCopyListItems.IsBasicViewHelpIcon = true;
            helpCopyAllLists.SetResourceString(type.FullName + tsCopyAllLists.Name, type);
            helpCopyAllLists.IsBasicViewHelpIcon = true;
            helpCopyItemsDocuments.SetResourceString(type.FullName + tsCopyItemsDocuments.Name, type);
            helpCopyItemsDocuments.IsBasicViewHelpIcon = true;
            tcListContentAdvancedOptions.AdvancedOptionsStateChanged += tcListContentAdvancedOptions_AdvancedOptionsStateChanged;
        }

        private void LoadControls()
        {
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    tsCopyListItems.Visible = false;
                    tsCopyItemsDocuments.Visible = false;
                    helpCopyListItems.Visible = false;
                    helpCopyItemsDocuments.Visible = false;
                    tsCopyAllLists.BringToFront();
                    break;
                case SharePointObjectScope.List:
                case SharePointObjectScope.Folder:
                    tsCopyAllLists.Visible = false;
                    tsCopyItemsDocuments.Visible = false;
                    helpCopyAllLists.Visible = false;
                    helpCopyItemsDocuments.Visible = false;
                    tsCopyListItems.BringToFront();
                    break;
                case SharePointObjectScope.Item:
                    tsCopyAllLists.Visible = false;
                    tsCopyListItems.Visible = false;
                    helpCopyListItems.Visible = false;
                    helpCopyAllLists.Visible = false;
                    tsCopyItemsDocuments.BringToFront();
                    break;
            }
        }

        private void LoadFolderOptions(SPFolderContentOptions folderOptions)
        {
            LoadControls();
            if (HasOnlyExternalLists)
            {
                base.Enabled = false;
                return;
            }
            tsCopyListItems.IsOn = folderOptions.CopyListItems;
            tiXVersions.Checked = folderOptions.CopyVersions && folderOptions.CopyMaxVersions;
            tiAllVersions.Checked = folderOptions.CopyVersions && !folderOptions.CopyMaxVersions;
            tiNoVersions.Checked = !folderOptions.CopyVersions;
            spinXVersions.Value = folderOptions.MaximumVersionCount;
            tcListContentAdvancedOptions.SourceNodes = SourceNodes;
            tcListContentAdvancedOptions.TargetNodes = TargetNodes;
            tcListContentAdvancedOptions.IsTargetOMAdapter = base.IsTargetOMAdapter;
            tcListContentAdvancedOptions.IsTargetClientOM = base.IsTargetClientOM;
            if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
            {
                tcListContentAdvancedOptions.IsNWS = ((SPNode)TargetNodes[0]).Adapter.IsNws;
            }
            tcAzureOptionsBasicView.Scope = base.Scope;
            tcAzureOptionsBasicView.SourceNodes = SourceNodes;
            tcAzureOptionsBasicView.TargetNodes = TargetNodes;
            tcListContentAdvancedOptions.Scope = base.Scope;
            tcListContentAdvancedOptions.LoadFolderOptions(folderOptions);
            tcAzureOptionsBasicView.LoadFolderOptions(folderOptions);
            if (!SPUIUtils.IsMigrationPipelineAllowed(base.Scope, tcAzureOptionsBasicView.TargetNodes))
            {
                HideControl(tcAzureOptionsBasicView);
                tcAzureOptionsBasicView.UpdateEnabledState(tcAzureOptionsBasicView.Enabled);
            }
            UpdateEnabledState();
            UpdateAllAvailabilities();
            UpdatePermissionsState();
        }

        private void LoadListContentOptions(SPListContentOptions listContentOptions)
        {
            LoadControls();
            if (HasOnlyExternalLists)
            {
                base.Enabled = false;
                return;
            }
            tsCopyAllLists.IsOn = listContentOptions.CopyLists;
            UpdateControlVisibility(listContentOptions);
            if (base.Scope != SharePointObjectScope.Item)
            {
                tsCopyListItems.IsOn = listContentOptions.CopyListItems;
            }
            else
            {
                tsCopyItemsDocuments.IsOn = base.Options.CopyVersions;
            }
            tiXVersions.Checked = listContentOptions.CopyVersions && listContentOptions.CopyMaxVersions;
            tiAllVersions.Checked = listContentOptions.CopyVersions && !listContentOptions.CopyMaxVersions;
            tiNoVersions.Checked = !listContentOptions.CopyVersions;
            spinXVersions.Value = listContentOptions.MaximumVersionCount;
            tcListContentAdvancedOptions.SourceNodes = SourceNodes;
            tcListContentAdvancedOptions.TargetNodes = TargetNodes;
            tcListContentAdvancedOptions.IsTargetOMAdapter = base.IsTargetOMAdapter;
            tcListContentAdvancedOptions.IsTargetClientOM = base.IsTargetClientOM;
            if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
            {
                tcListContentAdvancedOptions.IsNWS = ((SPNode)TargetNodes[0]).Adapter.IsNws;
            }
            tcAzureOptionsBasicView.Scope = base.Scope;
            tcAzureOptionsBasicView.SourceNodes = SourceNodes;
            tcAzureOptionsBasicView.TargetNodes = TargetNodes;
            tcListContentAdvancedOptions.LoadListContentOptions(listContentOptions);
            tcAzureOptionsBasicView.LoadListContentOptions(listContentOptions);
            if (!SPUIUtils.IsMigrationPipelineAllowed(base.Scope, tcAzureOptionsBasicView.TargetNodes))
            {
                HideControl(tcAzureOptionsBasicView);
                tcAzureOptionsBasicView.UpdateEnabledState(tcAzureOptionsBasicView.Enabled);
            }
            UpdateEnabledState();
            UpdateAllAvailabilities();
            UpdatePermissionsState();
        }

        protected override void LoadUI()
        {
            LoadListContentOptions(base.Options);
        }

        public void MigrationModeChanged(bool isPreserveListID)
        {
            tcListContentAdvancedOptions.MigrationModeChanged(isPreserveListID);
        }

        private bool SaveFolderOptions(SPFolderContentOptions folderOptions)
        {
            folderOptions.CopyListItems = tsCopyListItems.IsOn;
            folderOptions.CopyMaxVersions = tiXVersions.Checked;
            folderOptions.CopyVersions = tiAllVersions.Checked || tiXVersions.Checked;
            folderOptions.MaximumVersionCount = (int)spinXVersions.Value;
            if (IsSetExplicitOptions)
            {
                folderOptions.CopySubFolders = true;
                folderOptions.ApplyNewDocumentSets = false;
                if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
                {
                    folderOptions.ReattachPageLayouts = ((SPNode)SourceNodes[0]).Adapter.SharePointVersion.MajorVersion != ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.MajorVersion;
                }
            }
            if (!tcListContentAdvancedOptions.SaveFolderOptions(folderOptions))
            {
                return false;
            }
            return tcAzureOptionsBasicView.SaveFolderOptions(folderOptions);
        }

        private bool SaveListContentOptions(SPListContentOptions listContentOptions)
        {
            listContentOptions.CopyLists = tsCopyAllLists.IsOn;
            listContentOptions.CopyListItems = tsCopyListItems.IsOn;
            if ((base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && IsSetExplicitOptions && tsCopyAllLists.IsOn)
            {
                listContentOptions.CopyListItems = true;
            }
            listContentOptions.CopyMaxVersions = tiXVersions.Checked;
            bool copyVersions = ((base.Scope != SharePointObjectScope.Item) ? (tiAllVersions.Checked || tiXVersions.Checked) : tsCopyItemsDocuments.IsOn);
            listContentOptions.CopyVersions = copyVersions;
            listContentOptions.MaximumVersionCount = (int)spinXVersions.Value;
            if (IsSetExplicitOptions)
            {
                listContentOptions.CopySubFolders = true;
                listContentOptions.CopyCustomizedFormPages = true;
                listContentOptions.PreserveSharePointDocumentIDs = false;
                listContentOptions.ApplyNewDocumentSets = false;
                if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
                {
                    listContentOptions.ReattachPageLayouts = ((SPNode)SourceNodes[0]).Adapter.SharePointVersion.MajorVersion != ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.MajorVersion;
                }
            }
            if (!tcListContentAdvancedOptions.SaveListContentOptions(listContentOptions))
            {
                return false;
            }
            return tcAzureOptionsBasicView.SaveListContentOptions(listContentOptions);
        }

        public override bool SaveUI()
        {
            tcAzureOptionsBasicView.IsModeSwitched = base.IsModeSwitched;
            if (base.Scope != SharePointObjectScope.Folder)
            {
                return SaveListContentOptions(base.Options);
            }
            return SaveFolderOptions(_folderOptions);
        }

        private void tcListContentAdvancedOptions_AdvancedOptionsStateChanged(EventArgs args, object sender)
        {
            if (this.AdvancedOptionsMainControlStateChanged != null)
            {
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(120, (bool)sender), null);
            }
        }

        private void tiAllVersions_ItemClick(object sender, TileItemEventArgs e)
        {
            UpdateEnabledState();
            tiAllVersions.Checked = true;
            tiNoVersions.Checked = false;
            tiXVersions.Checked = false;
            spinXVersions.Enabled = false;
        }

        private void tiNoVersions_ItemClick(object sender, TileItemEventArgs e)
        {
            UpdateEnabledState();
            tiNoVersions.Checked = true;
            tiAllVersions.Checked = false;
            tiXVersions.Checked = false;
            spinXVersions.Enabled = false;
        }

        private void tiXVersions_ItemClick(object sender, TileItemEventArgs e)
        {
            UpdateEnabledState();
            tiXVersions.Checked = true;
            tiNoVersions.Checked = false;
            tiAllVersions.Checked = false;
            spinXVersions.Enabled = true;
            spinXVersions.Focus();
        }

        private void tsCopyAllLists_Toggled(object sender, EventArgs e)
        {
            UpdateEnabledState();
            if (this.AdvancedOptionsMainControlStateChanged != null)
            {
                int size = (tcListContentAdvancedOptions.IsAdvancedOptionsCollapsed ? 40 : 140);
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(size, !tsCopyAllLists.IsOn), null);
            }
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.List)
            {
                CopyListOrItemsVisibilityChanged(tsCopyAllLists.IsOn, tsCopyAllLists.IsOn ? base.Options.CopyListItems : tsCopyAllLists.IsOn);
            }
            UpdatePermissionsState();
        }

        private void tsCopyItemsDocuments_Toggled(object sender, EventArgs e)
        {
            UpdateEnabledState();
            tcListContentAdvancedOptions.Visible = tsCopyItemsDocuments.IsOn;
            if (this.AdvancedOptionsMainControlStateChanged != null)
            {
                int size = (tcListContentAdvancedOptions.IsAdvancedOptionsCollapsed ? 40 : 140);
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(size, !tsCopyItemsDocuments.IsOn), null);
            }
        }

        private void tsCopyListItems_Toggled(object sender, EventArgs e)
        {
            UpdateEnabledState();
            if (this.AdvancedOptionsMainControlStateChanged != null && base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site)
            {
                int size = ((!tcListContentAdvancedOptions.IsAdvancedOptionsCollapsed) ? 140 : 40);
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(size, !tsCopyListItems.IsOn), null);
            }
            CopyListOrItemsVisibilityChanged(null, tsCopyListItems.IsOn);
            UpdatePermissionsState();
        }

        private void UpdateAllAvailabilities()
        {
            FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.List, tsCopyAllLists.IsOn));
            FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Item, tsCopyAllLists.IsOn && tsCopyListItems.IsOn));
            FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Folder, tsCopyAllLists.IsOn));
        }

        private void UpdateControlVisibility(SPListContentOptions listContentOptions)
        {
            if (this.AdvancedOptionsMainControlStateChanged != null && !tsCopyAllLists.IsOn)
            {
                int size = (tcListContentAdvancedOptions.IsAdvancedOptionsCollapsed ? 40 : 140);
                this.AdvancedOptionsMainControlStateChanged(new AdvancedOptionsMainControlStateEventArgs(size, !tsCopyAllLists.IsOn), null);
            }
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.List)
            {
                CopyListOrItemsVisibilityChanged(tsCopyAllLists.IsOn, tsCopyAllLists.IsOn ? listContentOptions.CopyListItems : tsCopyAllLists.IsOn);
            }
        }

        protected override void UpdateEnabledState()
        {
            bool enabledState = GetEnabledState();
            tcVersions.Enabled = enabledState;
            if (!IsSetExplicitOptions && (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection) && base.Options != null)
            {
                tcVersions.Enabled = enabledState && base.Options.CopyListItems;
            }
            tiAllVersions.Enabled = enabledState;
            tiNoVersions.Enabled = enabledState;
            tiXVersions.Enabled = enabledState;
            tcAzureOptionsBasicView.Enabled = enabledState;
            spinXVersions.Enabled = enabledState && tiXVersions.Checked;
            tcListContentAdvancedOptions.UpdateEnabledState(enabledState);
            tcAzureOptionsBasicView.UpdateEnabledState(enabledState);
            tcListContentAdvancedOptions.Visible = enabledState;
        }

        private void UpdatePermissionsState()
        {
            if (this.UpdatePermissionsStateChanged != null)
            {
                this.UpdatePermissionsStateChanged(null, GetEnabledState());
            }
        }
    }
}
