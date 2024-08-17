using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MasterPage32x32.ico")]
    [ControlName("Master Page Gallery Options")]
    public class TCMasterPageGalleryOptions : ScopableTabbableControl
    {
        private SPMasterPageGalleryOptions m_options;

        private bool m_bCopyMasterPages = true;

        private bool m_bCopyPageLayouts = true;

        private bool m_bCopyOtherResources = true;

        private bool m_bCorrectLinks;

        private IContainer components;

        internal CheckEdit w_cbCopyOtherResources;

        internal CheckEdit w_cbCopyPageLayouts;

        internal PanelControl w_plAboveItemLevel;

        internal CheckEdit w_cbCorrectLinks;

        internal CheckEdit w_cbCopyMasterPages;

        public bool CopyMasterPages
        {
            get
            {
                return m_bCopyMasterPages;
            }
            set
            {
                m_bCopyMasterPages = value;
            }
        }

        public bool CopyOtherResources
        {
            get
            {
                return m_bCopyOtherResources;
            }
            set
            {
                m_bCopyOtherResources = value;
            }
        }

        public bool CopyPageLayouts
        {
            get
            {
                return m_bCopyPageLayouts;
            }
            set
            {
                m_bCopyPageLayouts = value;
            }
        }

        public bool CorrectMasterPageLinks
        {
            get
            {
                return m_bCorrectLinks;
            }
            set
            {
                m_bCorrectLinks = value;
            }
        }

        public SPMasterPageGalleryOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
                LoadUI();
            }
        }

        public TCMasterPageGalleryOptions()
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCMasterPageGalleryOptions));
            this.w_plAboveItemLevel = new DevExpress.XtraEditors.PanelControl();
            this.w_cbCopyMasterPages = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCorrectLinks = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCopyOtherResources = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCopyPageLayouts = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)this.w_plAboveItemLevel).BeginInit();
            this.w_plAboveItemLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPages.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyOtherResources.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyPageLayouts.Properties).BeginInit();
            base.SuspendLayout();
            this.w_plAboveItemLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plAboveItemLevel.Controls.Add(this.w_cbCopyMasterPages);
            this.w_plAboveItemLevel.Controls.Add(this.w_cbCorrectLinks);
            this.w_plAboveItemLevel.Controls.Add(this.w_cbCopyOtherResources);
            this.w_plAboveItemLevel.Controls.Add(this.w_cbCopyPageLayouts);
            resources.ApplyResources(this.w_plAboveItemLevel, "w_plAboveItemLevel");
            this.w_plAboveItemLevel.Name = "w_plAboveItemLevel";
            resources.ApplyResources(this.w_cbCopyMasterPages, "w_cbCopyMasterPages");
            this.w_cbCopyMasterPages.Name = "w_cbCopyMasterPages";
            this.w_cbCopyMasterPages.Properties.Caption = resources.GetString("w_cbCopyMasterPages.Properties.Caption");
            this.w_cbCopyMasterPages.CheckedChanged += new System.EventHandler(On_CopyMasterPages_OnCheckedChanged);
            this.w_cbCopyMasterPages.Click += new System.EventHandler(On_CopyMasterPages_OnClick);
            resources.ApplyResources(this.w_cbCorrectLinks, "w_cbCorrectLinks");
            this.w_cbCorrectLinks.Name = "w_cbCorrectLinks";
            this.w_cbCorrectLinks.Properties.Caption = resources.GetString("w_cbCorrectLinks.Properties.Caption");
            this.w_cbCorrectLinks.Click += new System.EventHandler(On_CorrectLinks_OnClick);
            resources.ApplyResources(this.w_cbCopyOtherResources, "w_cbCopyOtherResources");
            this.w_cbCopyOtherResources.Name = "w_cbCopyOtherResources";
            this.w_cbCopyOtherResources.Properties.Caption = resources.GetString("w_cbCopyOtherResources.Properties.Caption");
            this.w_cbCopyOtherResources.Click += new System.EventHandler(On_CopyOtherResources_OnClick);
            resources.ApplyResources(this.w_cbCopyPageLayouts, "w_cbCopyPageLayouts");
            this.w_cbCopyPageLayouts.Name = "w_cbCopyPageLayouts";
            this.w_cbCopyPageLayouts.Properties.Caption = resources.GetString("w_cbCopyPageLayouts.Properties.Caption");
            this.w_cbCopyPageLayouts.Click += new System.EventHandler(On_CopyPageLayouts_OnClick);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.w_plAboveItemLevel);
            base.Name = "TCMasterPageGalleryOptions";
            ((System.ComponentModel.ISupportInitialize)this.w_plAboveItemLevel).EndInit();
            this.w_plAboveItemLevel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPages.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyOtherResources.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCopyPageLayouts.Properties).EndInit();
            base.ResumeLayout(false);
        }

        protected override void LoadUI()
        {
            w_cbCopyMasterPages.Checked = Options.CopyMasterPages;
            w_cbCopyPageLayouts.Checked = Options.CopyPageLayouts;
            w_cbCopyOtherResources.Checked = Options.CopyOtherResources;
            w_cbCorrectLinks.Checked = Options.CorrectMasterPageLinks;
        }

        private void On_CopyMasterPages_OnCheckedChanged(object sender, EventArgs e)
        {
            w_cbCorrectLinks.Enabled = w_cbCopyMasterPages.Checked;
        }

        private void On_CopyMasterPages_OnClick(object sender, EventArgs e)
        {
            Options.CopyMasterPages = w_cbCopyMasterPages.Checked;
        }

        private void On_CopyOtherResources_OnClick(object sender, EventArgs e)
        {
            Options.CopyOtherResources = w_cbCopyOtherResources.Checked;
        }

        private void On_CopyPageLayouts_OnClick(object sender, EventArgs e)
        {
            Options.CopyPageLayouts = w_cbCopyPageLayouts.Checked;
        }

        private void On_CorrectLinks_OnClick(object sender, EventArgs e)
        {
            Options.CorrectMasterPageLinks = w_cbCorrectLinks.Checked;
        }

        public override bool SaveUI()
        {
            Options.CopyMasterPages = w_cbCopyMasterPages.Checked;
            Options.CopyPageLayouts = w_cbCopyPageLayouts.Checked;
            Options.CopyOtherResources = w_cbCopyOtherResources.Checked;
            Options.CorrectMasterPageLinks = w_cbCorrectLinks.Checked;
            return true;
        }
    }
}