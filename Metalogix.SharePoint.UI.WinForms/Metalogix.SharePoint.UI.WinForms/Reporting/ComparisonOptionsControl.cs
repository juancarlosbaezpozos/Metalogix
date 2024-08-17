using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Reporting;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Filters;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
    public class ComparisonOptionsControl : CollapsableControl
    {
        public enum ComparisonLevel
        {
            Site,
            List,
            Item
        }

        private ComparisonLevel m_ComparisonLevel;

        private CompareSiteOptions m_options;

        private bool m_bSitesExpanded = true;

        private bool m_bListsExpanded = true;

        private bool m_bItemsExpanded = true;

        private IContainer components;

        private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcSites;

        private PanelControl w_boxSites;

        private CheckEdit w_cbRecursive;

        private SimpleButton w_btnAdvancedSites;

        private PanelControl w_plSites;

        private PanelControl w_plLists;

        private SimpleButton w_btnAdvancedLists;

        private PanelControl w_boxLists;

        private CheckEdit w_cbCompareFolders;

        private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcLists;

        private CheckEdit w_cbCompareLists;

        private PanelControl w_plItems;

        private SimpleButton w_btnAdvancedItems;

        private PanelControl w_boxItems;

        private CheckEdit w_cbVersions;

        private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcItems;

        private CheckEdit w_cbItems;

        private CheckEdit w_cbHaltIfDifferent;

        private CheckEdit w_cbVerbose;

        private CheckEdit w_cbCompareMetadata;

        public CompareSiteOptions CompareSiteOptions
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

        public ComparisonLevel Level
        {
            get
            {
                return m_ComparisonLevel;
            }
            set
            {
                m_ComparisonLevel = value;
                UpdateLevel();
            }
        }

        public ComparisonOptionsControl()
        {
            InitializeComponent();
            Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl = w_fcSites;
            Type[] collection = new Type[1] { typeof(SPWeb) };
            filterControl.FilterType = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: false);
            w_fcSites.TypeHeader = "Sites";
            Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl2 = w_fcLists;
            Type[] collection2 = new Type[1] { typeof(SPList) };
            filterControl2.FilterType = new FilterBuilderType(new List<Type>(collection2), bAllowFreeFormEntry: false);
            w_fcLists.TypeHeader = "Lists";
            Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl3 = w_fcItems;
            Type[] collection3 = new Type[1] { typeof(SPListItem) };
            filterControl3.FilterType = new FilterBuilderType(new List<Type>(collection3), bAllowFreeFormEntry: true);
            w_fcItems.TypeHeader = "Items";
        }

        private void CollapseAll()
        {
            CollapseSites();
            CollapseLists();
            CollapseItems();
        }

        private void CollapseItems()
        {
            if (m_bItemsExpanded && w_plItems.Visible)
            {
                SuspendLayout();
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                w_btnAdvancedItems.Text = "Advanced >>";
                HideControl(w_boxItems);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bItemsExpanded = false;
            }
        }

        private void CollapseLists()
        {
            if (m_bListsExpanded && w_plLists.Visible)
            {
                SuspendLayout();
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                w_btnAdvancedLists.Text = "Advanced >>";
                HideControl(w_boxLists);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bListsExpanded = false;
            }
        }

        private void CollapseSites()
        {
            if (m_bSitesExpanded && w_plSites.Visible)
            {
                SuspendLayout();
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                w_btnAdvancedSites.Text = "Advanced >>";
                HideControl(w_boxSites);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bSitesExpanded = false;
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

        private void ExpandAll()
        {
            ExpandSites();
            ExpandLists();
            ExpandItems();
        }

        private void ExpandItems()
        {
            if (!m_bItemsExpanded)
            {
                SuspendLayout();
                w_btnAdvancedItems.Text = "Advanced <<";
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                ShowControl(w_boxItems, w_plItems);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bItemsExpanded = true;
            }
        }

        private void ExpandLists()
        {
            if (!m_bListsExpanded)
            {
                SuspendLayout();
                w_btnAdvancedLists.Text = "Advanced <<";
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                ShowControl(w_boxLists, w_plLists);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bListsExpanded = true;
            }
        }

        private void ExpandSites()
        {
            if (!m_bSitesExpanded)
            {
                SuspendLayout();
                w_btnAdvancedSites.Text = "Advanced <<";
                MaximumSize = new Size(0, 0);
                MinimumSize = new Size(0, 0);
                ShowControl(w_boxSites, w_plSites);
                MaximumSize = new Size(1024, base.Size.Height);
                MinimumSize = new Size(346, base.Size.Height);
                ResumeLayout(performLayout: false);
                PerformLayout();
                m_bSitesExpanded = true;
            }
        }

        private void InitializeComponent()
        {
            this.w_boxSites = new DevExpress.XtraEditors.PanelControl();
            this.w_fcSites = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
            this.w_cbRecursive = new DevExpress.XtraEditors.CheckEdit();
            this.w_btnAdvancedSites = new DevExpress.XtraEditors.SimpleButton();
            this.w_plSites = new DevExpress.XtraEditors.PanelControl();
            this.w_plLists = new DevExpress.XtraEditors.PanelControl();
            this.w_btnAdvancedLists = new DevExpress.XtraEditors.SimpleButton();
            this.w_boxLists = new DevExpress.XtraEditors.PanelControl();
            this.w_cbCompareFolders = new DevExpress.XtraEditors.CheckEdit();
            this.w_fcLists = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
            this.w_cbCompareLists = new DevExpress.XtraEditors.CheckEdit();
            this.w_plItems = new DevExpress.XtraEditors.PanelControl();
            this.w_btnAdvancedItems = new DevExpress.XtraEditors.SimpleButton();
            this.w_boxItems = new DevExpress.XtraEditors.PanelControl();
            this.w_cbVersions = new DevExpress.XtraEditors.CheckEdit();
            this.w_fcItems = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
            this.w_cbItems = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbHaltIfDifferent = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbVerbose = new DevExpress.XtraEditors.CheckEdit();
            this.w_cbCompareMetadata = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)this.w_boxSites).BeginInit();
            this.w_boxSites.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSites).BeginInit();
            this.w_plSites.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_plLists).BeginInit();
            this.w_plLists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_boxLists).BeginInit();
            this.w_boxLists.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareFolders.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareLists.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plItems).BeginInit();
            this.w_plItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_boxItems).BeginInit();
            this.w_boxItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbItems.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbHaltIfDifferent.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbVerbose.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareMetadata.Properties).BeginInit();
            base.SuspendLayout();
            this.w_boxSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_boxSites.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_boxSites.Appearance.Options.UseBackColor = true;
            this.w_boxSites.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_boxSites.Controls.Add(this.w_fcSites);
            this.w_boxSites.Location = new System.Drawing.Point(3, 25);
            this.w_boxSites.Name = "w_boxSites";
            this.w_boxSites.Padding = new System.Windows.Forms.Padding(3);
            this.w_boxSites.Size = new System.Drawing.Size(363, 48);
            this.w_boxSites.TabIndex = 0;
            this.w_fcSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_fcSites.Filters = null;
            this.w_fcSites.IsFiltered = false;
            this.w_fcSites.Location = new System.Drawing.Point(8, -2);
            this.w_fcSites.Name = "w_fcSites";
            this.w_fcSites.Padding = new System.Windows.Forms.Padding(5);
            this.w_fcSites.Size = new System.Drawing.Size(347, 50);
            this.w_fcSites.TabIndex = 2;
            this.w_fcSites.TypeHeader = "Object";
            this.w_cbRecursive.Location = new System.Drawing.Point(0, 0);
            this.w_cbRecursive.Name = "w_cbRecursive";
            this.w_cbRecursive.Properties.AutoWidth = true;
            this.w_cbRecursive.Properties.Caption = "Recursively Compare Subsites";
            this.w_cbRecursive.Size = new System.Drawing.Size(167, 19);
            this.w_cbRecursive.TabIndex = 0;
            this.w_cbRecursive.CheckedChanged += new System.EventHandler(On_RecurseSites_CheckChanged);
            this.w_btnAdvancedSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_btnAdvancedSites.Location = new System.Drawing.Point(267, 1);
            this.w_btnAdvancedSites.Name = "w_btnAdvancedSites";
            this.w_btnAdvancedSites.Size = new System.Drawing.Size(98, 23);
            this.w_btnAdvancedSites.TabIndex = 1;
            this.w_btnAdvancedSites.Text = "Advanced <<";
            this.w_btnAdvancedSites.Click += new System.EventHandler(On_AdvancedSites_Click);
            this.w_plSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_plSites.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_plSites.Appearance.Options.UseBackColor = true;
            this.w_plSites.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plSites.Controls.Add(this.w_btnAdvancedSites);
            this.w_plSites.Controls.Add(this.w_cbRecursive);
            this.w_plSites.Controls.Add(this.w_boxSites);
            this.w_plSites.Location = new System.Drawing.Point(12, 12);
            this.w_plSites.Name = "w_plSites";
            this.w_plSites.Size = new System.Drawing.Size(366, 74);
            this.w_plSites.TabIndex = 0;
            this.w_plLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_plLists.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_plLists.Appearance.Options.UseBackColor = true;
            this.w_plLists.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plLists.Controls.Add(this.w_btnAdvancedLists);
            this.w_plLists.Controls.Add(this.w_boxLists);
            this.w_plLists.Controls.Add(this.w_cbCompareLists);
            this.w_plLists.Location = new System.Drawing.Point(12, 92);
            this.w_plLists.Name = "w_plLists";
            this.w_plLists.Size = new System.Drawing.Size(366, 98);
            this.w_plLists.TabIndex = 1;
            this.w_btnAdvancedLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_btnAdvancedLists.Location = new System.Drawing.Point(267, 1);
            this.w_btnAdvancedLists.Name = "w_btnAdvancedLists";
            this.w_btnAdvancedLists.Size = new System.Drawing.Size(98, 23);
            this.w_btnAdvancedLists.TabIndex = 1;
            this.w_btnAdvancedLists.Text = "Advanced <<";
            this.w_btnAdvancedLists.Click += new System.EventHandler(On_AdvancedLists_Click);
            this.w_boxLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_boxLists.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_boxLists.Appearance.Options.UseBackColor = true;
            this.w_boxLists.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_boxLists.Controls.Add(this.w_cbCompareFolders);
            this.w_boxLists.Controls.Add(this.w_fcLists);
            this.w_boxLists.Location = new System.Drawing.Point(3, 25);
            this.w_boxLists.Name = "w_boxLists";
            this.w_boxLists.Padding = new System.Windows.Forms.Padding(3);
            this.w_boxLists.Size = new System.Drawing.Size(363, 72);
            this.w_boxLists.TabIndex = 71;
            this.w_cbCompareFolders.Location = new System.Drawing.Point(7, 2);
            this.w_cbCompareFolders.Name = "w_cbCompareFolders";
            this.w_cbCompareFolders.Properties.AutoWidth = true;
            this.w_cbCompareFolders.Properties.Caption = "Compare Folders";
            this.w_cbCompareFolders.Size = new System.Drawing.Size(104, 19);
            this.w_cbCompareFolders.TabIndex = 0;
            this.w_fcLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_fcLists.Filters = null;
            this.w_fcLists.IsFiltered = false;
            this.w_fcLists.Location = new System.Drawing.Point(8, 19);
            this.w_fcLists.Name = "w_fcLists";
            this.w_fcLists.Padding = new System.Windows.Forms.Padding(5);
            this.w_fcLists.Size = new System.Drawing.Size(349, 50);
            this.w_fcLists.TabIndex = 1;
            this.w_fcLists.TypeHeader = "Object";
            this.w_fcLists.Load += new System.EventHandler(On_Load);
            this.w_cbCompareLists.Location = new System.Drawing.Point(0, 0);
            this.w_cbCompareLists.Name = "w_cbCompareLists";
            this.w_cbCompareLists.Properties.AutoWidth = true;
            this.w_cbCompareLists.Properties.Caption = "Compare Lists";
            this.w_cbCompareLists.Size = new System.Drawing.Size(90, 19);
            this.w_cbCompareLists.TabIndex = 0;
            this.w_cbCompareLists.CheckedChanged += new System.EventHandler(On_CompareLists_CheckChanged);
            this.w_plItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_plItems.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_plItems.Appearance.Options.UseBackColor = true;
            this.w_plItems.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plItems.Controls.Add(this.w_btnAdvancedItems);
            this.w_plItems.Controls.Add(this.w_boxItems);
            this.w_plItems.Controls.Add(this.w_cbItems);
            this.w_plItems.Location = new System.Drawing.Point(12, 196);
            this.w_plItems.Name = "w_plItems";
            this.w_plItems.Size = new System.Drawing.Size(366, 94);
            this.w_plItems.TabIndex = 2;
            this.w_btnAdvancedItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_btnAdvancedItems.Location = new System.Drawing.Point(267, 1);
            this.w_btnAdvancedItems.Name = "w_btnAdvancedItems";
            this.w_btnAdvancedItems.Size = new System.Drawing.Size(98, 23);
            this.w_btnAdvancedItems.TabIndex = 1;
            this.w_btnAdvancedItems.Text = "Advanced <<";
            this.w_btnAdvancedItems.Click += new System.EventHandler(On_AdvancedItems_Click);
            this.w_boxItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_boxItems.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.w_boxItems.Appearance.Options.UseBackColor = true;
            this.w_boxItems.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_boxItems.Controls.Add(this.w_cbVersions);
            this.w_boxItems.Controls.Add(this.w_fcItems);
            this.w_boxItems.Location = new System.Drawing.Point(3, 25);
            this.w_boxItems.Name = "w_boxItems";
            this.w_boxItems.Padding = new System.Windows.Forms.Padding(3);
            this.w_boxItems.Size = new System.Drawing.Size(363, 67);
            this.w_boxItems.TabIndex = 2;
            this.w_cbVersions.Location = new System.Drawing.Point(7, 1);
            this.w_cbVersions.Name = "w_cbVersions";
            this.w_cbVersions.Properties.AutoWidth = true;
            this.w_cbVersions.Properties.Caption = "Compare Versions";
            this.w_cbVersions.Size = new System.Drawing.Size(109, 19);
            this.w_cbVersions.TabIndex = 0;
            this.w_fcItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_fcItems.Filters = null;
            this.w_fcItems.IsFiltered = false;
            this.w_fcItems.Location = new System.Drawing.Point(8, 17);
            this.w_fcItems.Name = "w_fcItems";
            this.w_fcItems.Padding = new System.Windows.Forms.Padding(5);
            this.w_fcItems.Size = new System.Drawing.Size(349, 50);
            this.w_fcItems.TabIndex = 1;
            this.w_fcItems.TypeHeader = "Object";
            this.w_cbItems.Location = new System.Drawing.Point(0, 0);
            this.w_cbItems.Name = "w_cbItems";
            this.w_cbItems.Properties.AutoWidth = true;
            this.w_cbItems.Properties.Caption = "Compare Items";
            this.w_cbItems.Size = new System.Drawing.Size(95, 19);
            this.w_cbItems.TabIndex = 0;
            this.w_cbItems.CheckedChanged += new System.EventHandler(On_CompareItems_CheckChanged);
            this.w_cbHaltIfDifferent.Location = new System.Drawing.Point(13, 319);
            this.w_cbHaltIfDifferent.Name = "w_cbHaltIfDifferent";
            this.w_cbHaltIfDifferent.Properties.AutoWidth = true;
            this.w_cbHaltIfDifferent.Properties.Caption = "Halt if Different";
            this.w_cbHaltIfDifferent.Size = new System.Drawing.Size(97, 19);
            this.w_cbHaltIfDifferent.TabIndex = 4;
            this.w_cbVerbose.Location = new System.Drawing.Point(13, 342);
            this.w_cbVerbose.Name = "w_cbVerbose";
            this.w_cbVerbose.Properties.AutoWidth = true;
            this.w_cbVerbose.Properties.Caption = "Enable Verbose Logging";
            this.w_cbVerbose.Size = new System.Drawing.Size(137, 19);
            this.w_cbVerbose.TabIndex = 5;
            this.w_cbCompareMetadata.Location = new System.Drawing.Point(13, 296);
            this.w_cbCompareMetadata.Name = "w_cbCompareMetadata";
            this.w_cbCompareMetadata.Properties.AutoWidth = true;
            this.w_cbCompareMetadata.Properties.Caption = "Compare Metadata";
            this.w_cbCompareMetadata.Size = new System.Drawing.Size(115, 19);
            this.w_cbCompareMetadata.TabIndex = 3;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.w_cbCompareMetadata);
            base.Controls.Add(this.w_cbVerbose);
            base.Controls.Add(this.w_cbHaltIfDifferent);
            base.Controls.Add(this.w_plItems);
            base.Controls.Add(this.w_plLists);
            base.Controls.Add(this.w_plSites);
            base.Name = "ComparisonOptionsControl";
            base.Size = new System.Drawing.Size(390, 373);
            ((System.ComponentModel.ISupportInitialize)this.w_boxSites).EndInit();
            this.w_boxSites.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plSites).EndInit();
            this.w_plSites.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_plLists).EndInit();
            this.w_plLists.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_boxLists).EndInit();
            this.w_boxLists.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareFolders.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareLists.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_plItems).EndInit();
            this.w_plItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_boxItems).EndInit();
            this.w_boxItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbItems.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbHaltIfDifferent.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbVerbose.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_cbCompareMetadata.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void LoadUI()
        {
            if (CompareSiteOptions != null)
            {
                w_cbCompareMetadata.Checked = CompareSiteOptions.CheckResults;
                w_cbHaltIfDifferent.Checked = CompareSiteOptions.HaltIfDifferent;
                w_cbRecursive.Checked = CompareSiteOptions.Recursive;
                w_cbCompareLists.Checked = CompareSiteOptions.CompareLists;
                w_cbVerbose.Checked = CompareSiteOptions.Verbose;
                w_cbCompareFolders.Checked = CompareSiteOptions.CompareFolders;
                w_cbItems.Checked = CompareSiteOptions.CompareItems;
                w_cbVersions.Checked = CompareSiteOptions.CompareVersions;
                w_fcSites.IsFiltered = CompareSiteOptions.FilterSites;
                w_fcSites.Filters = FilterExpression.ParseExpression(CompareSiteOptions.SiteFilterExpression);
                w_fcLists.IsFiltered = CompareSiteOptions.FilterLists;
                w_fcLists.Filters = FilterExpression.ParseExpression(CompareSiteOptions.ListFilterExpression);
                w_fcItems.IsFiltered = CompareSiteOptions.FilterItems;
                w_fcItems.Filters = FilterExpression.ParseExpression(CompareSiteOptions.ItemFilterExpression);
            }
        }

        private void On_AdvancedItems_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            ToggleItems();
            ResumeLayout(performLayout: false);
            PerformLayout();
        }

        private void On_AdvancedLists_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            ToggleLists();
            ResumeLayout(performLayout: false);
            PerformLayout();
        }

        private void On_AdvancedSites_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            ToggleSites();
            ResumeLayout(performLayout: false);
            PerformLayout();
        }

        private void On_CompareItems_CheckChanged(object sender, EventArgs e)
        {
            CheckEdit checkEdit = (CheckEdit)sender;
            w_boxItems.Enabled = checkEdit.Checked;
        }

        private void On_CompareLists_CheckChanged(object sender, EventArgs e)
        {
            CheckEdit checkEdit = (CheckEdit)sender;
            w_boxLists.Enabled = checkEdit.Checked;
            if (Level == ComparisonLevel.Site)
            {
                w_plItems.Enabled = checkEdit.Checked;
            }
        }

        private void On_Load(object sender, EventArgs e)
        {
            w_btnAdvancedItems.Hide();
            w_btnAdvancedLists.Hide();
            w_btnAdvancedSites.Hide();
            if (Level == ComparisonLevel.Site)
            {
                w_boxLists.Enabled = w_cbCompareLists.Checked;
                w_boxSites.Enabled = w_cbRecursive.Checked;
                w_boxItems.Enabled = w_cbItems.Checked;
                w_plItems.Enabled = w_cbCompareLists.Checked;
            }
            else if (Level != ComparisonLevel.List)
            {
                if (Level == ComparisonLevel.Item)
                {
                    w_boxItems.Enabled = w_cbVersions.Checked;
                }
            }
            else
            {
                w_boxLists.Enabled = w_cbCompareFolders.Checked;
                w_boxItems.Enabled = w_cbItems.Checked;
            }
        }

        private void On_Okay(object sender, EventArgs e)
        {
            SaveUI();
        }

        private void On_RecurseSites_CheckChanged(object sender, EventArgs e)
        {
            w_boxSites.Enabled = w_cbRecursive.Checked;
        }

        public void SaveUI()
        {
            if (CompareSiteOptions != null)
            {
                CompareSiteOptions.CheckResults = w_cbCompareMetadata.Checked;
                CompareSiteOptions.HaltIfDifferent = w_cbHaltIfDifferent.Checked;
                CompareSiteOptions.Recursive = w_cbRecursive.Checked;
                CompareSiteOptions.CompareLists = w_cbCompareLists.Checked;
                CompareSiteOptions.Verbose = w_cbVerbose.Checked;
                CompareSiteOptions.CompareFolders = w_cbCompareFolders.Checked;
                CompareSiteOptions.CompareItems = w_cbItems.Checked;
                CompareSiteOptions.CompareVersions = w_cbVersions.Checked;
                CompareSiteOptions.FilterSites = w_fcSites.IsFiltered;
                CompareSiteOptions.SiteFilterExpression = w_fcSites.Filters.ToXML();
                CompareSiteOptions.FilterLists = w_fcLists.IsFiltered;
                CompareSiteOptions.ListFilterExpression = w_fcLists.Filters.ToXML();
                CompareSiteOptions.FilterItems = w_fcItems.IsFiltered;
                CompareSiteOptions.ItemFilterExpression = w_fcItems.Filters.ToXML();
                SharePointConfigurationVariables.AllowCheckResults = CompareSiteOptions.CheckResults;
            }
        }

        private void ToggleAll()
        {
            ToggleSites();
            ToggleLists();
            ToggleItems();
        }

        private void ToggleItems()
        {
            if (m_bItemsExpanded)
            {
                CollapseItems();
            }
            else
            {
                ExpandItems();
            }
        }

        private void ToggleLists()
        {
            if (m_bListsExpanded)
            {
                CollapseLists();
            }
            else
            {
                ExpandLists();
            }
        }

        private void ToggleSites()
        {
            if (m_bSitesExpanded)
            {
                CollapseSites();
            }
            else
            {
                ExpandSites();
            }
        }

        private void UpdateLevel()
        {
            SuspendLayout();
            if (m_ComparisonLevel != 0)
            {
                if (m_ComparisonLevel == ComparisonLevel.List)
                {
                    HideControl(w_plSites);
                    HideControl(w_cbCompareFolders);
                    w_boxLists.Controls.Remove(w_cbCompareFolders);
                    Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl = w_fcLists;
                    Type[] collection = new Type[1] { typeof(SPFolder) };
                    filterControl.FilterType = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: false);
                    w_fcLists.TypeHeader = "Folders";
                    w_cbCompareFolders.CheckedChanged += On_CompareLists_CheckChanged;
                    w_cbCompareFolders.Location = w_cbCompareLists.Location;
                    w_cbCompareFolders.Size = w_cbCompareLists.Size;
                    ShowControl(w_cbCompareFolders, w_plLists);
                    HideControl(w_cbCompareLists);
                    w_plLists.Controls.Remove(w_cbCompareLists);
                }
                else if (m_ComparisonLevel == ComparisonLevel.Item)
                {
                    HideControl(w_plSites);
                    HideControl(w_plLists);
                    HideControl(w_cbVersions);
                    w_boxItems.Controls.Remove(w_cbVersions);
                    Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl2 = w_fcItems;
                    Type[] collection2 = new Type[1] { typeof(SPListItemVersion) };
                    filterControl2.FilterType = new FilterBuilderType(new List<Type>(collection2), bAllowFreeFormEntry: true);
                    w_fcItems.TypeHeader = "Versions";
                    w_cbVersions.CheckedChanged += On_CompareItems_CheckChanged;
                    w_cbVersions.Location = w_cbItems.Location;
                    w_cbVersions.Size = w_cbItems.Size;
                    ShowControl(w_cbVersions, w_plItems);
                    HideControl(w_cbItems);
                    w_plItems.Controls.Remove(w_cbItems);
                }
            }
            ResumeLayout(performLayout: false);
            PerformLayout();
        }
    }
}
