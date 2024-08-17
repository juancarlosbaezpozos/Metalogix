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
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCListContentAdvancedOptionsBasicView : ScopableTabbableControl
    {
        public bool IsAdvancedOptionsCollapsed;

        private bool _originalPreserveItemIdsValue;

        private bool _suppressPreserveIdsChangedEvent;

        private IContainer components;

        private LayoutControl lcMainListAdvancedOptions;

        private LayoutControlGroup lcgOuterGroup;

        private EmptySpaceItem esListAdvancedOptions;

        private LayoutControlGroup lcgListAdvancedOptions;

        private PanelControl pnlAdvancedOptions;

        private ToggleSwitch tsPreserveLibraryID;

        private ToggleSwitch tsScanOfficeDocs;

        private LayoutControlItem lciAdvancedOptions;

        private ToggleSwitch tsPreserveListID;

        private HelpTipButton helpScanOfficeDocs;

        private HelpTipButton helpPreserveLibraryID;

        private HelpTipButton helpPreserveListID;

        public bool IsNWS { get; set; }

        public bool IsTargetClientOM { get; set; }

        public bool IsTargetOMAdapter { get; set; }

        public event TCListContentOptionsBasicView.AdvancedOptionsStateChangedHandler AdvancedOptionsStateChanged;

        public TCListContentAdvancedOptionsBasicView()
        {
            InitializeComponent();
            InitializeControls();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public ArrayList GetAdvancedOptionsSummary()
        {
            ArrayList arrayList = new ArrayList();
            if (tsPreserveListID.Enabled)
            {
                arrayList.Add(new OptionsSummary($"{tsPreserveListID.Properties.OnText} : {tsPreserveListID.IsOn}", 2));
            }
            if (tsPreserveLibraryID.Enabled)
            {
                arrayList.Add(new OptionsSummary($"{tsPreserveLibraryID.Properties.OnText} : {tsPreserveLibraryID.IsOn}", 2));
            }
            if (tsScanOfficeDocs.Enabled)
            {
                arrayList.Add(new OptionsSummary($"{tsScanOfficeDocs.Properties.OnText} : {tsScanOfficeDocs.IsOn}", 2));
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCListContentAdvancedOptionsBasicView));
            this.lcMainListAdvancedOptions = new DevExpress.XtraLayout.LayoutControl();
            this.pnlAdvancedOptions = new DevExpress.XtraEditors.PanelControl();
            this.tsPreserveListID = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpScanOfficeDocs = new TooltipsTest.HelpTipButton();
            this.helpPreserveLibraryID = new TooltipsTest.HelpTipButton();
            this.helpPreserveListID = new TooltipsTest.HelpTipButton();
            this.tsPreserveLibraryID = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsScanOfficeDocs = new DevExpress.XtraEditors.ToggleSwitch();
            this.lcgOuterGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this.esListAdvancedOptions = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcgListAdvancedOptions = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAdvancedOptions = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)this.lcMainListAdvancedOptions).BeginInit();
            this.lcMainListAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).BeginInit();
            this.pnlAdvancedOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.tsPreserveListID.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpScanOfficeDocs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpPreserveLibraryID).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpPreserveListID).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsPreserveLibraryID.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsScanOfficeDocs.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgOuterGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.esListAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgListAdvancedOptions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).BeginInit();
            base.SuspendLayout();
            this.lcMainListAdvancedOptions.Controls.Add(this.pnlAdvancedOptions);
            this.lcMainListAdvancedOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcMainListAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcMainListAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.lcMainListAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lcMainListAdvancedOptions.Name = "lcMainListAdvancedOptions";
            this.lcMainListAdvancedOptions.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(226, 121, 250, 350);
            this.lcMainListAdvancedOptions.OptionsView.UseDefaultDragAndDropRendering = false;
            this.lcMainListAdvancedOptions.Root = this.lcgOuterGroup;
            this.lcMainListAdvancedOptions.Size = new System.Drawing.Size(494, 156);
            this.lcMainListAdvancedOptions.TabIndex = 0;
            this.lcMainListAdvancedOptions.Text = "layoutControl1";
            this.lcMainListAdvancedOptions.GroupExpandChanged += new DevExpress.XtraLayout.Utils.LayoutGroupEventHandler(lcMainListAdvancedOptions_GroupExpandChanged);
            this.pnlAdvancedOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlAdvancedOptions.Controls.Add(this.tsPreserveListID);
            this.pnlAdvancedOptions.Controls.Add(this.helpScanOfficeDocs);
            this.pnlAdvancedOptions.Controls.Add(this.helpPreserveLibraryID);
            this.pnlAdvancedOptions.Controls.Add(this.helpPreserveListID);
            this.pnlAdvancedOptions.Controls.Add(this.tsPreserveLibraryID);
            this.pnlAdvancedOptions.Controls.Add(this.tsScanOfficeDocs);
            this.pnlAdvancedOptions.Location = new System.Drawing.Point(14, 34);
            this.pnlAdvancedOptions.LookAndFeel.SkinName = "Metalogix 2017";
            this.pnlAdvancedOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlAdvancedOptions.Name = "pnlAdvancedOptions";
            this.pnlAdvancedOptions.Size = new System.Drawing.Size(466, 98);
            this.pnlAdvancedOptions.TabIndex = 4;
            this.tsPreserveListID.EditValue = true;
            this.tsPreserveListID.Location = new System.Drawing.Point(28, 7);
            this.tsPreserveListID.Name = "tsPreserveListID";
            this.tsPreserveListID.Properties.Appearance.Options.UseTextOptions = true;
            this.tsPreserveListID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsPreserveListID.Properties.OffText = "Preserve IDs for List Content";
            this.tsPreserveListID.Properties.OnText = "Preserve IDs for List Content";
            this.tsPreserveListID.Size = new System.Drawing.Size(228, 24);
            this.tsPreserveListID.TabIndex = 1;
            this.tsPreserveListID.Toggled += new System.EventHandler(tsPreserveListID_Toggled);
            this.helpScanOfficeDocs.AnchoringControl = null;
            this.helpScanOfficeDocs.BackColor = System.Drawing.Color.Transparent;
            this.helpScanOfficeDocs.CommonParentControl = null;
            this.helpScanOfficeDocs.Image = (System.Drawing.Image)resources.GetObject("helpScanOfficeDocs.Image");
            this.helpScanOfficeDocs.Location = new System.Drawing.Point(313, 73);
            this.helpScanOfficeDocs.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpScanOfficeDocs.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpScanOfficeDocs.Name = "helpScanOfficeDocs";
            this.helpScanOfficeDocs.RealOffset = null;
            this.helpScanOfficeDocs.RelativeOffset = null;
            this.helpScanOfficeDocs.Size = new System.Drawing.Size(20, 20);
            this.helpScanOfficeDocs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpScanOfficeDocs.TabIndex = 2;
            this.helpScanOfficeDocs.TabStop = false;
            this.helpPreserveLibraryID.AnchoringControl = null;
            this.helpPreserveLibraryID.BackColor = System.Drawing.Color.Transparent;
            this.helpPreserveLibraryID.CommonParentControl = null;
            this.helpPreserveLibraryID.Image = (System.Drawing.Image)resources.GetObject("helpPreserveLibraryID.Image");
            this.helpPreserveLibraryID.Location = new System.Drawing.Point(270, 41);
            this.helpPreserveLibraryID.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpPreserveLibraryID.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpPreserveLibraryID.Name = "helpPreserveLibraryID";
            this.helpPreserveLibraryID.RealOffset = null;
            this.helpPreserveLibraryID.RelativeOffset = null;
            this.helpPreserveLibraryID.Size = new System.Drawing.Size(20, 20);
            this.helpPreserveLibraryID.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpPreserveLibraryID.TabIndex = 3;
            this.helpPreserveLibraryID.TabStop = false;
            this.helpPreserveListID.AnchoringControl = null;
            this.helpPreserveListID.BackColor = System.Drawing.Color.Transparent;
            this.helpPreserveListID.CommonParentControl = null;
            this.helpPreserveListID.Image = (System.Drawing.Image)resources.GetObject("helpPreserveListID.Image");
            this.helpPreserveListID.Location = new System.Drawing.Point(256, 9);
            this.helpPreserveListID.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpPreserveListID.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpPreserveListID.Name = "helpPreserveListID";
            this.helpPreserveListID.RealOffset = null;
            this.helpPreserveListID.RelativeOffset = null;
            this.helpPreserveListID.Size = new System.Drawing.Size(20, 20);
            this.helpPreserveListID.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpPreserveListID.TabIndex = 4;
            this.helpPreserveListID.TabStop = false;
            this.tsPreserveLibraryID.Location = new System.Drawing.Point(28, 39);
            this.tsPreserveLibraryID.Name = "tsPreserveLibraryID";
            this.tsPreserveLibraryID.Properties.Appearance.Options.UseTextOptions = true;
            this.tsPreserveLibraryID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsPreserveLibraryID.Properties.OffText = "Preserve IDs for Library Content";
            this.tsPreserveLibraryID.Properties.OnText = "Preserve IDs for Library Content";
            this.tsPreserveLibraryID.Size = new System.Drawing.Size(243, 24);
            this.tsPreserveLibraryID.TabIndex = 2;
            this.tsPreserveLibraryID.Toggled += new System.EventHandler(tsPreserveLibraryID_Toggled);
            this.tsScanOfficeDocs.EditValue = true;
            this.tsScanOfficeDocs.Location = new System.Drawing.Point(28, 71);
            this.tsScanOfficeDocs.Name = "tsScanOfficeDocs";
            this.tsScanOfficeDocs.Properties.Appearance.Options.UseTextOptions = true;
            this.tsScanOfficeDocs.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsScanOfficeDocs.Properties.OffText = "Scan Office Docs for Additional Metadata";
            this.tsScanOfficeDocs.Properties.OnText = "Scan Office Docs for Additional Metadata";
            this.tsScanOfficeDocs.Size = new System.Drawing.Size(285, 24);
            this.tsScanOfficeDocs.TabIndex = 3;
            this.lcgOuterGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.lcgOuterGroup.GroupBordersVisible = false;
            DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this.lcgOuterGroup.Items;
            DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[2] { this.esListAdvancedOptions, this.lcgListAdvancedOptions };
            items.AddRange(items2);
            this.lcgOuterGroup.Location = new System.Drawing.Point(0, 0);
            this.lcgOuterGroup.Name = "Root";
            this.lcgOuterGroup.Size = new System.Drawing.Size(494, 156);
            this.lcgOuterGroup.TextVisible = false;
            this.esListAdvancedOptions.AllowHotTrack = false;
            this.esListAdvancedOptions.Location = new System.Drawing.Point(0, 146);
            this.esListAdvancedOptions.Name = "esListAdvancedOptions";
            this.esListAdvancedOptions.Size = new System.Drawing.Size(494, 10);
            this.esListAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lcgListAdvancedOptions.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lcgListAdvancedOptions.AppearanceGroup.Options.UseFont = true;
            this.lcgListAdvancedOptions.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgListAdvancedOptions.ExpandButtonVisible = true;
            this.lcgListAdvancedOptions.ExpandOnDoubleClick = true;
            this.lcgListAdvancedOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.lciAdvancedOptions });
            this.lcgListAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lcgListAdvancedOptions.Name = "lcgListAdvancedOptions";
            this.lcgListAdvancedOptions.Size = new System.Drawing.Size(494, 146);
            this.lcgListAdvancedOptions.Text = "Advanced Options";
            this.lciAdvancedOptions.Control = this.pnlAdvancedOptions;
            this.lciAdvancedOptions.Location = new System.Drawing.Point(0, 0);
            this.lciAdvancedOptions.Name = "lciAdvancedOptions";
            this.lciAdvancedOptions.Size = new System.Drawing.Size(470, 102);
            this.lciAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
            this.lciAdvancedOptions.TextVisible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.lcMainListAdvancedOptions);
            base.Name = "TCListContentAdvancedOptionsBasicView";
            base.Size = new System.Drawing.Size(494, 156);
            ((System.ComponentModel.ISupportInitialize)this.lcMainListAdvancedOptions).EndInit();
            this.lcMainListAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pnlAdvancedOptions).EndInit();
            this.pnlAdvancedOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.tsPreserveListID.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpScanOfficeDocs).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpPreserveLibraryID).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpPreserveListID).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsPreserveLibraryID.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsScanOfficeDocs.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgOuterGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.esListAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lcgListAdvancedOptions).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.lciAdvancedOptions).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            if (!SharePointConfigurationVariables.AllowDBWriting)
            {
                tsPreserveLibraryID.Enabled = false;
            }
            Type type = GetType();
            helpPreserveListID.SetResourceString(type.FullName + tsPreserveListID.Name, type);
            helpPreserveLibraryID.SetResourceString(type.FullName + tsPreserveLibraryID.Name, type);
            helpScanOfficeDocs.SetResourceString(type.FullName + tsScanOfficeDocs.Name, type);
            helpPreserveListID.IsBasicViewHelpIcon = true;
            helpPreserveLibraryID.IsBasicViewHelpIcon = true;
            helpScanOfficeDocs.IsBasicViewHelpIcon = true;
        }

        private bool IsNWSFolderAction()
        {
            if (!IsNWS)
            {
                return false;
            }
            return base.Scope == SharePointObjectScope.Folder;
        }

        private void lcMainListAdvancedOptions_GroupExpandChanged(object sender, LayoutGroupEventArgs e)
        {
            if (this.AdvancedOptionsStateChanged != null)
            {
                IsAdvancedOptionsCollapsed = !e.Group.Expanded;
                this.AdvancedOptionsStateChanged(null, IsAdvancedOptionsCollapsed);
            }
        }

        public void LoadFolderOptions(SPFolderContentOptions folderOptions)
        {
            tsPreserveListID.IsOn = folderOptions.PreserveItemIDs;
            _originalPreserveItemIdsValue = folderOptions.PreserveItemIDs;
            tsPreserveLibraryID.IsOn = folderOptions.PreserveDocumentIDs && SharePointConfigurationVariables.AllowDBWriting;
            tsScanOfficeDocs.IsOn = !folderOptions.DisableDocumentParsing;
        }

        public void LoadListContentOptions(SPListContentOptions listContentOptions)
        {
            tsPreserveListID.IsOn = listContentOptions.PreserveItemIDs;
            _originalPreserveItemIdsValue = listContentOptions.PreserveItemIDs;
            tsPreserveLibraryID.IsOn = listContentOptions.PreserveDocumentIDs && SharePointConfigurationVariables.AllowDBWriting;
            tsScanOfficeDocs.IsOn = !listContentOptions.DisableDocumentParsing;
        }

        public void MigrationModeChanged(bool isPreserveListID)
        {
            if (IsTargetOMAdapter && !IsTargetClientOM)
            {
                _suppressPreserveIdsChangedEvent = true;
                if (!isPreserveListID)
                {
                    tsPreserveListID.Enabled = true;
                    tsPreserveListID.IsOn = _originalPreserveItemIdsValue;
                }
                else
                {
                    tsPreserveListID.Enabled = true;
                    tsPreserveListID.IsOn = true;
                }
                _suppressPreserveIdsChangedEvent = false;
            }
        }

        public bool SaveFolderOptions(SPFolderContentOptions folderOptions)
        {
            folderOptions.PreserveItemIDs = (IsTargetOMAdapter || IsTargetClientOM) && tsPreserveListID.IsOn;
            folderOptions.PreserveDocumentIDs = tsPreserveLibraryID.IsOn && tsPreserveLibraryID.Enabled;
            folderOptions.DisableDocumentParsing = !tsScanOfficeDocs.IsOn;
            return true;
        }

        public bool SaveListContentOptions(SPListContentOptions listContentOptions)
        {
            listContentOptions.PreserveItemIDs = (IsTargetOMAdapter || IsTargetClientOM) && tsPreserveListID.IsOn;
            listContentOptions.PreserveDocumentIDs = tsPreserveLibraryID.IsOn && tsPreserveLibraryID.Enabled;
            listContentOptions.DisableDocumentParsing = !tsScanOfficeDocs.IsOn;
            return true;
        }

        private void tsPreserveLibraryID_Toggled(object sender, EventArgs e)
        {
            if (tsPreserveLibraryID.IsOn && base.ContainsFocus && FlatXtraMessageBox.Show(string.Format(Resources.Feature_Requires_DBWriting, Resources.Preserve_DocumentIDs), Resources.Information, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
            {
                tsPreserveLibraryID.IsOn = false;
            }
        }

        private void tsPreserveListID_Toggled(object sender, EventArgs e)
        {
            if (!_suppressPreserveIdsChangedEvent)
            {
                _originalPreserveItemIdsValue = tsPreserveListID.IsOn;
            }
        }

        public void UpdateEnabledState(bool copyItems)
        {
            tsPreserveListID.Enabled = copyItems && (IsTargetOMAdapter || IsTargetClientOM) && !IsNWSFolderAction();
            tsPreserveLibraryID.Enabled = copyItems && SharePointConfigurationVariables.AllowDBWriting && IsTargetOMAdapter && !IsTargetClientOM;
            tsScanOfficeDocs.Enabled = copyItems && !IsNWS;
        }
    }
}