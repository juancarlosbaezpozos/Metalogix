using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Metalogix.SharePoint;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MigrationMode.png")]
    [ControlName("Summary")]
    public class TCSummaryBasicView : ScopableTabbableControl
    {
        private IContainer components;

        private LabelControl lblMigrationDetails;

        private LabelControl lblOptionsDetails;

        private TreeList tlSummary;

        private TreeListColumn tlcSummary;

        private LabelControl lblSourceURLText;

        private LabelControl lblSourceURL;

        private LabelControl lblTargetURL;

        private LabelControl lblTargetURLText;

        private SimpleButton btnCollapseAll;

        private SimpleButton btnExpandAll;

        private PanelControl pnlMigrationDetails;

        private List<TabbableControl> Tabs => ((LeftNavigableTabsForm)base.ParentForm).Tabs;

        public TCSummaryBasicView()
        {
            InitializeComponent();
            pnlMigrationDetails.Width -= 16;
            tlSummary.Width -= 16;
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            tlSummary.CollapseAll();
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            tlSummary.ExpandAll();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public new void GetSummaryScreenDetails()
        {
            tlSummary.ClearNodes();
            ArrayList arrayList = new ArrayList();
            foreach (TabbableControl tab in Tabs)
            {
                ArrayList summaryScreenDetails = tab.GetSummaryScreenDetails();
                if (summaryScreenDetails != null && summaryScreenDetails.Count > 0)
                {
                    arrayList.AddRange(summaryScreenDetails);
                }
            }
            LoadUI(arrayList.Cast<OptionsSummary>().ToList());
            tlSummary.ExpandToLevel(1);
            LoadURLs();
        }

        private void InitializeComponent()
        {
            this.lblMigrationDetails = new DevExpress.XtraEditors.LabelControl();
            this.lblOptionsDetails = new DevExpress.XtraEditors.LabelControl();
            this.tlSummary = new DevExpress.XtraTreeList.TreeList();
            this.tlcSummary = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.lblSourceURLText = new DevExpress.XtraEditors.LabelControl();
            this.lblSourceURL = new DevExpress.XtraEditors.LabelControl();
            this.lblTargetURL = new DevExpress.XtraEditors.LabelControl();
            this.lblTargetURLText = new DevExpress.XtraEditors.LabelControl();
            this.btnCollapseAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnExpandAll = new DevExpress.XtraEditors.SimpleButton();
            this.pnlMigrationDetails = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)this.tlSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlMigrationDetails).BeginInit();
            this.pnlMigrationDetails.SuspendLayout();
            base.SuspendLayout();
            this.lblMigrationDetails.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lblMigrationDetails.Appearance.Options.UseFont = true;
            this.lblMigrationDetails.Location = new System.Drawing.Point(18, 4);
            this.lblMigrationDetails.Name = "lblMigrationDetails";
            this.lblMigrationDetails.Size = new System.Drawing.Size(99, 13);
            this.lblMigrationDetails.TabStop = true;
            this.lblMigrationDetails.Text = "Migration Details:";
            this.lblOptionsDetails.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lblOptionsDetails.Appearance.Options.UseFont = true;
            this.lblOptionsDetails.Location = new System.Drawing.Point(18, 81);
            this.lblOptionsDetails.Name = "lblOptionsDetails";
            this.lblOptionsDetails.Size = new System.Drawing.Size(88, 13);
            this.lblOptionsDetails.TabStop = true;
            this.lblOptionsDetails.Text = "Options Details:";
            this.tlSummary.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.tlcSummary });
            this.tlSummary.Location = new System.Drawing.Point(18, 103);
            this.tlSummary.Name = "tlSummary";
            this.tlSummary.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.tlSummary.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.tlSummary.OptionsView.ShowColumns = false;
            this.tlSummary.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
            this.tlSummary.OptionsView.ShowHorzLines = false;
            this.tlSummary.OptionsView.ShowIndicator = false;
            this.tlSummary.OptionsView.ShowVertLines = false;
            this.tlSummary.Size = new System.Drawing.Size(647, 514);
            this.tlSummary.TabStop = true;
            this.tlSummary.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Dark;
            this.tlSummary.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(tlSummary_NodeCellStyle);
            this.tlcSummary.MinWidth = 88;
            this.tlcSummary.Name = "tlcSummary";
            this.tlcSummary.OptionsColumn.AllowEdit = false;
            this.tlcSummary.OptionsColumn.AllowFocus = false;
            this.tlcSummary.Visible = true;
            this.tlcSummary.VisibleIndex = 0;
            this.tlcSummary.Width = 88;
            this.lblSourceURLText.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lblSourceURLText.Appearance.Options.UseFont = true;
            this.lblSourceURLText.Location = new System.Drawing.Point(22, 6);
            this.lblSourceURLText.Name = "lblSourceURLText";
            this.lblSourceURLText.Size = new System.Drawing.Size(67, 13);
            this.lblSourceURLText.TabStop = true;
            this.lblSourceURLText.Text = "Source URL:";
            this.lblSourceURL.Location = new System.Drawing.Point(100, 6);
            this.lblSourceURL.Name = "lblSourceURL";
            this.lblSourceURL.Size = new System.Drawing.Size(0, 13);
            this.lblSourceURL.TabStop = true;
            this.lblTargetURL.Location = new System.Drawing.Point(100, 26);
            this.lblTargetURL.Name = "lblTargetURL";
            this.lblTargetURL.Size = new System.Drawing.Size(0, 13);
            this.lblTargetURL.TabStop = true;
            this.lblTargetURLText.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.lblTargetURLText.Appearance.Options.UseFont = true;
            this.lblTargetURLText.Location = new System.Drawing.Point(22, 26);
            this.lblTargetURLText.Name = "lblTargetURLText";
            this.lblTargetURLText.Size = new System.Drawing.Size(66, 13);
            this.lblTargetURLText.TabStop = true;
            this.lblTargetURLText.Text = "Target URL:";
            this.btnExpandAll.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.btnExpandAll.Appearance.Options.UseFont = true;
            this.btnExpandAll.Location = new System.Drawing.Point(113, 78);
            this.btnExpandAll.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.btnExpandAll.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(20, 20);
            this.btnExpandAll.TabIndex = 1;
            this.btnExpandAll.Text = "+";
            this.btnExpandAll.ToolTip = "Expand All";
            this.btnExpandAll.Click += new System.EventHandler(btnExpandAll_Click);
            this.btnCollapseAll.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
            this.btnCollapseAll.Appearance.Options.UseFont = true;
            this.btnCollapseAll.Location = new System.Drawing.Point(141, 78);
            this.btnCollapseAll.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.btnCollapseAll.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(20, 20);
            this.btnCollapseAll.TabIndex = 2;
            this.btnCollapseAll.Text = "-";
            this.btnCollapseAll.ToolTip = "Collapse All";
            this.btnCollapseAll.Click += new System.EventHandler(btnCollapseAll_Click);
            this.pnlMigrationDetails.Appearance.BackColor = System.Drawing.Color.White;
            this.pnlMigrationDetails.Appearance.BackColor2 = System.Drawing.Color.White;
            this.pnlMigrationDetails.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlMigrationDetails.Controls.Add(this.lblSourceURLText);
            this.pnlMigrationDetails.Controls.Add(this.lblSourceURL);
            this.pnlMigrationDetails.Controls.Add(this.lblTargetURLText);
            this.pnlMigrationDetails.Controls.Add(this.lblTargetURL);
            this.pnlMigrationDetails.Location = new System.Drawing.Point(18, 23);
            this.pnlMigrationDetails.Name = "pnlMigrationDetails";
            this.pnlMigrationDetails.Size = new System.Drawing.Size(647, 49);
            this.pnlMigrationDetails.TabStop = true;
            this.pnlMigrationDetails.Paint += new System.Windows.Forms.PaintEventHandler(pnlMigrationDetails_Paint);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.pnlMigrationDetails);
            base.Controls.Add(this.btnExpandAll);
            base.Controls.Add(this.btnCollapseAll);
            base.Controls.Add(this.tlSummary);
            base.Controls.Add(this.lblOptionsDetails);
            base.Controls.Add(this.lblMigrationDetails);
            base.Name = "TCSummaryBasicView";
            base.Size = new System.Drawing.Size(747, 722);
            ((System.ComponentModel.ISupportInitialize)this.tlSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pnlMigrationDetails).EndInit();
            this.pnlMigrationDetails.ResumeLayout(false);
            this.pnlMigrationDetails.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadUI(List<OptionsSummary> summary)
        {
            TreeListNode treeListNode = null;
            foreach (OptionsSummary item in summary)
            {
                switch (item.Level)
                {
                    case 0:
                        {
                            TreeListNodes nodes5 = tlSummary.Nodes;
                            object[] nodeData5 = new object[1] { item.Text };
                            treeListNode = nodes5.Add(nodeData5);
                            break;
                        }
                    case 1:
                        {
                            TreeListNodes nodes4 = treeListNode.Nodes;
                            object[] nodeData4 = new object[1] { item.Text };
                            nodes4.Add(nodeData4);
                            break;
                        }
                    case 2:
                        {
                            TreeListNodes nodes3 = treeListNode.LastNode.Nodes;
                            object[] nodeData3 = new object[1] { item.Text };
                            nodes3.Add(nodeData3);
                            break;
                        }
                    case 3:
                        {
                            TreeListNodes nodes2 = treeListNode.LastNode.LastNode.Nodes;
                            object[] nodeData2 = new object[1] { item.Text };
                            nodes2.Add(nodeData2);
                            break;
                        }
                    case 4:
                        {
                            TreeListNodes nodes = treeListNode.LastNode.LastNode.LastNode.Nodes;
                            object[] nodeData = new object[1] { item.Text };
                            nodes.Add(nodeData);
                            break;
                        }
                }
            }
        }

        private void LoadURLs()
        {
            lblSourceURL.Text = (SourceNodes[0] as SPNode).Url;
            lblTargetURL.Text = (TargetNodes[0] as SPNode).Url;
            int interval = 120;
            int num = 90;
            if (lblSourceURL.Text.Length > num)
            {
                lblSourceURL.ToolTip = SPUtils.InsertStringAtInterval(lblSourceURL.Text, "\n", interval);
            }
            if (lblTargetURL.Text.Length > num)
            {
                lblTargetURL.ToolTip = SPUtils.InsertStringAtInterval(lblTargetURL.Text, "\n", interval);
            }
            if (lblSourceURL.Text.Length > num)
            {
                lblSourceURL.Text = lblSourceURL.Text.Substring(0, num) + "...";
            }
            if (lblTargetURL.Text.Length > num)
            {
                lblTargetURL.Text = lblTargetURL.Text.Substring(0, num) + "...";
            }
        }

        private void pnlMigrationDetails_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, pnlMigrationDetails.Width, pnlMigrationDetails.Height), Color.FromArgb(173, 170, 173), 1, ButtonBorderStyle.Solid, Color.FromArgb(173, 170, 173), 1, ButtonBorderStyle.Solid, Color.FromArgb(173, 170, 173), 1, ButtonBorderStyle.Solid, Color.FromArgb(173, 170, 173), 1, ButtonBorderStyle.Solid);
        }

        private void tlSummary_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level < 1)
            {
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }
    }
}