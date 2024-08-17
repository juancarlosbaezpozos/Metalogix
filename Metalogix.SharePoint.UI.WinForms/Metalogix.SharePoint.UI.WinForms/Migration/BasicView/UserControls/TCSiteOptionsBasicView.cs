using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCSiteOptionsBasicView : SiteContentOptionsScopableTabbableControl
    {
        private SPSiteContentOptions _siteContentOptions;

        private IContainer components;

        private ToggleSwitch tsCopyChildSites;

        private ToggleSwitch tsCopyNavigation;

        private ToggleSwitch tsCopySiteFeatures;

        private TileControl tcSiteFeatures;

        private TileGroup tgPreserveDefaultFeatures;

        private TileItem tiPreserveDefaultFeatures;

        private TileGroup tgClearDefaultFeatures;

        private TileItem tiClearDefaultFeatures;

        private HelpTipButton helpCopyChildSites;

        private HelpTipButton helpSiteFeatures;

        private HelpTipButton helpCopyNavigation;

        public bool IsSetExplicitOptions { get; set; }

        public SPSiteContentOptions SiteContentOptions
        {
            get
            {
                return _siteContentOptions;
            }
            set
            {
                _siteContentOptions = value;
                LoadUI();
            }
        }

        public event TCMigrationOptionsBasicView.UpdateNavigationOptionHandler NavigationOptionChanged;

        public event TCMigrationOptionsBasicView.UpdateOptionsVisibilityHandler OptionsVisibilityChanged;

        public TCSiteOptionsBasicView()
        {
            InitializeComponent();
            IntializeFormControls();
        }

        private void CopyChildSitesVisibilityChanged()
        {
            if (this.OptionsVisibilityChanged != null)
            {
                this.OptionsVisibilityChanged(new UpdateOptionsStateEventArgs(tsCopyChildSites.IsOn), null);
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

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            arrayList.Add(new OptionsSummary("Site Options", 0));
            arrayList.Add(new OptionsSummary($"{tsCopyChildSites.Properties.OnText} : {tsCopyChildSites.IsOn}", 1));
            arrayList.Add(new OptionsSummary($"{tsCopyNavigation.Properties.OnText} : {tsCopyNavigation.IsOn}", 1));
            ArrayList arrayList2 = arrayList;
            if (!tsCopySiteFeatures.Enabled)
            {
                return arrayList2;
            }
            string arg = tsCopySiteFeatures.IsOn.ToString();
            if (tsCopySiteFeatures.IsOn)
            {
                arg = (tiPreserveDefaultFeatures.Checked ? tiPreserveDefaultFeatures.Text : tiClearDefaultFeatures.Text);
            }
            arrayList2.Add(new OptionsSummary($"{tsCopySiteFeatures.Properties.OnText} : {arg}", 1));
            return arrayList2;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCSiteOptionsBasicView));
            DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.helpSiteFeatures = new TooltipsTest.HelpTipButton();
            this.helpCopyChildSites = new TooltipsTest.HelpTipButton();
            this.tcSiteFeatures = new DevExpress.XtraEditors.TileControl();
            this.tgPreserveDefaultFeatures = new DevExpress.XtraEditors.TileGroup();
            this.tiPreserveDefaultFeatures = new DevExpress.XtraEditors.TileItem();
            this.tgClearDefaultFeatures = new DevExpress.XtraEditors.TileGroup();
            this.tiClearDefaultFeatures = new DevExpress.XtraEditors.TileItem();
            this.tsCopySiteFeatures = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyNavigation = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsCopyChildSites = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpCopyNavigation = new TooltipsTest.HelpTipButton();
            ((System.ComponentModel.ISupportInitialize)this.helpSiteFeatures).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyChildSites).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySiteFeatures.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyNavigation.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyChildSites.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyNavigation).BeginInit();
            base.SuspendLayout();
            this.helpSiteFeatures.AnchoringControl = null;
            this.helpSiteFeatures.BackColor = System.Drawing.Color.Transparent;
            this.helpSiteFeatures.CommonParentControl = null;
            this.helpSiteFeatures.Image = (System.Drawing.Image)resources.GetObject("helpSiteFeatures.Image");
            this.helpSiteFeatures.Location = new System.Drawing.Point(269, 81);
            this.helpSiteFeatures.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpSiteFeatures.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpSiteFeatures.Name = "helpSiteFeatures";
            this.helpSiteFeatures.RealOffset = null;
            this.helpSiteFeatures.RelativeOffset = null;
            this.helpSiteFeatures.Size = new System.Drawing.Size(20, 20);
            this.helpSiteFeatures.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpSiteFeatures.TabIndex = 5;
            this.helpSiteFeatures.TabStop = false;
            this.helpCopyChildSites.AnchoringControl = null;
            this.helpCopyChildSites.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyChildSites.CommonParentControl = null;
            this.helpCopyChildSites.Image = (System.Drawing.Image)resources.GetObject("helpCopyChildSites.Image");
            this.helpCopyChildSites.Location = new System.Drawing.Point(175, 21);
            this.helpCopyChildSites.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyChildSites.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyChildSites.Name = "helpCopyChildSites";
            this.helpCopyChildSites.RealOffset = null;
            this.helpCopyChildSites.RelativeOffset = null;
            this.helpCopyChildSites.Size = new System.Drawing.Size(20, 20);
            this.helpCopyChildSites.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyChildSites.TabIndex = 4;
            this.helpCopyChildSites.TabStop = false;
            this.tcSiteFeatures.AllowDrag = false;
            this.tcSiteFeatures.AllowDragTilesBetweenGroups = false;
            this.tcSiteFeatures.AppearanceItem.Hovered.BackColor = System.Drawing.Color.FromArgb(98, 181, 229);
            this.tcSiteFeatures.AppearanceItem.Hovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcSiteFeatures.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcSiteFeatures.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcSiteFeatures.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            this.tcSiteFeatures.AppearanceItem.Normal.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.tcSiteFeatures.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcSiteFeatures.AppearanceItem.Normal.Options.UseFont = true;
            this.tcSiteFeatures.Cursor = System.Windows.Forms.Cursors.Default;
            this.tcSiteFeatures.DragSize = new System.Drawing.Size(0, 0);
            this.tcSiteFeatures.Groups.Add(this.tgPreserveDefaultFeatures);
            this.tcSiteFeatures.Groups.Add(this.tgClearDefaultFeatures);
            this.tcSiteFeatures.IndentBetweenGroups = 40;
            this.tcSiteFeatures.IndentBetweenItems = 30;
            this.tcSiteFeatures.ItemSize = 55;
            this.tcSiteFeatures.Location = new System.Drawing.Point(11, 103);
            this.tcSiteFeatures.MaxId = 6;
            this.tcSiteFeatures.Name = "tcSiteFeatures";
            this.tcSiteFeatures.Size = new System.Drawing.Size(460, 78);
            this.tcSiteFeatures.TabIndex = 3;
            this.tcSiteFeatures.Text = "tileControl1";
            this.tgPreserveDefaultFeatures.Items.Add(this.tiPreserveDefaultFeatures);
            this.tgPreserveDefaultFeatures.Name = "tgPreserveDefaultFeatures";
            this.tiPreserveDefaultFeatures.Checked = true;
            tileItemElement.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Preserve_the_Default_Features;
            tileItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement.Text = "Preserve the Default Features";
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiPreserveDefaultFeatures.Elements.Add(tileItemElement);
            this.tiPreserveDefaultFeatures.Id = 4;
            this.tiPreserveDefaultFeatures.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiPreserveDefaultFeatures.Name = "tiPreserveDefaultFeatures";
            this.tiPreserveDefaultFeatures.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiPreserveDefaultFeatures_ItemClick);
            this.tgClearDefaultFeatures.Items.Add(this.tiClearDefaultFeatures);
            this.tgClearDefaultFeatures.Name = "tgClearDefaultFeatures";
            tileItemElement2.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Clear_the_Default_Features;
            tileItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Clear the Default Features";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
            this.tiClearDefaultFeatures.Elements.Add(tileItemElement2);
            this.tiClearDefaultFeatures.Id = 5;
            this.tiClearDefaultFeatures.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            this.tiClearDefaultFeatures.Name = "tiClearDefaultFeatures";
            this.tiClearDefaultFeatures.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(tiClearDefaultFeatures_ItemClick);
            this.tsCopySiteFeatures.Location = new System.Drawing.Point(11, 80);
            this.tsCopySiteFeatures.Name = "tsCopySiteFeatures";
            this.tsCopySiteFeatures.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopySiteFeatures.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopySiteFeatures.Properties.OffText = "Copy Site Features from the Source";
            this.tsCopySiteFeatures.Properties.OnText = "Copy Site Features from the Source";
            this.tsCopySiteFeatures.Size = new System.Drawing.Size(261, 24);
            this.tsCopySiteFeatures.TabIndex = 2;
            this.tsCopySiteFeatures.Toggled += new System.EventHandler(tsCopySiteFeatures_Toggled);
            this.tsCopyNavigation.Location = new System.Drawing.Point(11, 50);
            this.tsCopyNavigation.Name = "tsCopyNavigation";
            this.tsCopyNavigation.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyNavigation.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyNavigation.Properties.OffText = "Copy Navigation";
            this.tsCopyNavigation.Properties.OnText = "Copy Navigation";
            this.tsCopyNavigation.Size = new System.Drawing.Size(165, 24);
            this.tsCopyNavigation.TabIndex = 1;
            this.tsCopyNavigation.Toggled += new System.EventHandler(tsCopyNavigation_Toggled);
            this.tsCopyChildSites.Location = new System.Drawing.Point(11, 20);
            this.tsCopyChildSites.Name = "tsCopyChildSites";
            this.tsCopyChildSites.Properties.Appearance.Options.UseTextOptions = true;
            this.tsCopyChildSites.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsCopyChildSites.Properties.OffText = "Copy Child Sites";
            this.tsCopyChildSites.Properties.OnText = "Copy Child Sites";
            this.tsCopyChildSites.Size = new System.Drawing.Size(164, 24);
            this.tsCopyChildSites.TabIndex = 0;
            this.tsCopyChildSites.Toggled += new System.EventHandler(tsCopyChildSites_Toggled);
            this.helpCopyNavigation.AnchoringControl = null;
            this.helpCopyNavigation.BackColor = System.Drawing.Color.Transparent;
            this.helpCopyNavigation.CommonParentControl = null;
            this.helpCopyNavigation.Image = (System.Drawing.Image)resources.GetObject("helpCopyNavigation.Image");
            this.helpCopyNavigation.Location = new System.Drawing.Point(176, 52);
            this.helpCopyNavigation.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpCopyNavigation.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpCopyNavigation.Name = "helpCopyNavigation";
            this.helpCopyNavigation.RealOffset = null;
            this.helpCopyNavigation.RelativeOffset = null;
            this.helpCopyNavigation.Size = new System.Drawing.Size(20, 20);
            this.helpCopyNavigation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpCopyNavigation.TabIndex = 6;
            this.helpCopyNavigation.TabStop = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.helpCopyNavigation);
            base.Controls.Add(this.helpSiteFeatures);
            base.Controls.Add(this.helpCopyChildSites);
            base.Controls.Add(this.tcSiteFeatures);
            base.Controls.Add(this.tsCopySiteFeatures);
            base.Controls.Add(this.tsCopyNavigation);
            base.Controls.Add(this.tsCopyChildSites);
            base.Name = "TCSiteOptionsBasicView";
            base.Size = new System.Drawing.Size(517, 195);
            ((System.ComponentModel.ISupportInitialize)this.helpSiteFeatures).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyChildSites).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopySiteFeatures.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyNavigation.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsCopyChildSites.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpCopyNavigation).EndInit();
            base.ResumeLayout(false);
        }

        private void IntializeFormControls()
        {
            Type type = GetType();
            helpCopyChildSites.SetResourceString(type.FullName + tsCopyChildSites.Name, type);
            helpSiteFeatures.SetResourceString(type.FullName + tsCopySiteFeatures.Name, type);
            helpCopyNavigation.SetResourceString(type.FullName + tsCopyNavigation.Name, type);
            helpCopyChildSites.IsBasicViewHelpIcon = true;
            helpSiteFeatures.IsBasicViewHelpIcon = true;
            helpCopyNavigation.IsBasicViewHelpIcon = true;
        }

        protected override void LoadUI()
        {
            tsCopyChildSites.IsOn = SiteContentOptions.RecursivelyCopySubsites;
            CopyChildSitesVisibilityChanged();
            tsCopyNavigation.IsOn = SiteContentOptions.CopyNavigation;
            tsCopySiteFeatures.IsOn = SiteContentOptions.CopySiteFeatures;
            if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
            {
                tsCopySiteFeatures.Enabled = !((SPNode)TargetNodes[0]).Adapter.IsNws;
                tcSiteFeatures.Enabled = !((SPNode)TargetNodes[0]).Adapter.IsNws;
            }
            tiClearDefaultFeatures.Checked = !SiteContentOptions.MergeSiteFeatures;
            tiPreserveDefaultFeatures.Checked = SiteContentOptions.MergeSiteFeatures;
            if (base.SourceAdapter.SharePointVersion.IsSharePoint2003 && base.TargetAdapter.SharePointVersion.IsSharePoint2010OrLater)
            {
                tiPreserveDefaultFeatures.Checked = true;
                tiClearDefaultFeatures.Checked = false;
                tiClearDefaultFeatures.Enabled = false;
            }
            UpdateEnabledState();
        }

        public override bool SaveUI()
        {
            SiteContentOptions.RecursivelyCopySubsites = tsCopyChildSites.IsOn;
            SiteContentOptions.CopySiteFeatures = tsCopySiteFeatures.IsOn;
            SiteContentOptions.MergeSiteFeatures = tiPreserveDefaultFeatures.Checked;
            if (IsSetExplicitOptions)
            {
                SiteContentOptions.CopyContentTypes = true;
            }
            if (IsSetExplicitOptions || (!SiteContentOptions.RunNavigationStructureCopy && !SiteContentOptions.CopyNavigation))
            {
                SiteContentOptions.RunNavigationStructureCopy = tsCopyNavigation.IsOn;
                SiteContentOptions.CopyGlobalNavigation = tsCopyNavigation.IsOn;
                SiteContentOptions.CopyCurrentNavigation = tsCopyNavigation.IsOn;
            }
            SiteContentOptions.CopyNavigation = tsCopyNavigation.IsOn;
            return true;
        }

        private void tiClearDefaultFeatures_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveDefaultFeatures.Checked = false;
            tiClearDefaultFeatures.Checked = true;
        }

        private void tiPreserveDefaultFeatures_ItemClick(object sender, TileItemEventArgs e)
        {
            tiPreserveDefaultFeatures.Checked = true;
            tiClearDefaultFeatures.Checked = false;
        }

        private void tsCopyChildSites_Toggled(object sender, EventArgs e)
        {
            CopyChildSitesVisibilityChanged();
        }

        private void tsCopyNavigation_Toggled(object sender, EventArgs e)
        {
            if (this.NavigationOptionChanged != null)
            {
                bool flag = tsCopyNavigation.IsOn && ((!_siteContentOptions.CopyNavigation && !_siteContentOptions.RunNavigationStructureCopy) || _siteContentOptions.RunNavigationStructureCopy);
                TCMigrationOptionsBasicView.UpdateNavigationOptionHandler navigationOptionChanged = this.NavigationOptionChanged;
                bool value = tsCopyNavigation.IsOn && flag;
                navigationOptionChanged(new UpdateOptionsStateEventArgs(value), null);
            }
        }

        private void tsCopySiteFeatures_Toggled(object sender, EventArgs e)
        {
            tcSiteFeatures.Enabled = tsCopySiteFeatures.IsOn;
        }

        protected override void UpdateEnabledState()
        {
            tsCopySiteFeatures.Enabled = base.TargetIsOMAdapter;
            tsCopySiteFeatures.IsOn = tsCopySiteFeatures.IsOn && tsCopySiteFeatures.Enabled;
            tcSiteFeatures.Enabled = tsCopySiteFeatures.IsOn;
        }
    }
}