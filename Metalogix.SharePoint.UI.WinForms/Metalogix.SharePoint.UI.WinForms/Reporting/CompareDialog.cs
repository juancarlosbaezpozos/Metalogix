using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Reporting;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
    public class CompareDialog : XtraForm
    {
        private delegate void UpdateExplorerToggleButtonDelegate();

        private delegate void UpdateSpitterDistanceDelegate(int i);

        public static string SHOW_EXPLORER;

        public static string HIDE_EXPLORER;

        private ToolStripButton w_btnShowExplorer = new ToolStripButton();

        private SPNode m_sourceNode;

        private SPNode m_targetNode;

        private CompareSiteOptions m_options;

        private bool m_bMovingExplorers;

        private IContainer components;

        private ExplorerControlWithLocation w_targetExplorer;

        private ComparisonOptionsControl w_comparisonOptionsControl;

        protected SimpleButton w_btnCancel;

        protected SimpleButton w_btnOK;

        private SplitContainer w_splitter;

        private ExplorerControlWithLocation w_sourceExplorer;

        private PanelControl w_plComparison;

        public CompareSiteOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
                w_comparisonOptionsControl.CompareSiteOptions = (CompareSiteOptions)value.Clone();
            }
        }

        public SPNode SourceNode
        {
            get
            {
                return m_sourceNode;
            }
            set
            {
                m_sourceNode = value;
            }
        }

        public SPNode TargetNode
        {
            get
            {
                return m_targetNode;
            }
            set
            {
                m_targetNode = value;
            }
        }

        static CompareDialog()
        {
            SHOW_EXPLORER = "Show Explorer";
            HIDE_EXPLORER = "Hide Explorer";
        }

        public CompareDialog()
        {
            InitializeComponent();
            w_splitter.Panel1MinSize = w_sourceExplorer.LocationBarHeight;
            w_splitter.SplitterDistance = w_sourceExplorer.LocationBarHeight;
            w_splitter.Panel2MinSize = w_targetExplorer.LocationBarHeight;
            w_btnShowExplorer.Overflow = ToolStripItemOverflow.Never;
            w_btnShowExplorer.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            w_btnShowExplorer.Text = SHOW_EXPLORER;
            w_btnShowExplorer.AutoSize = false;
            w_btnShowExplorer.Width = 96;
            w_btnShowExplorer.Image = Resources.SmallAdd.ToBitmap();
            w_btnShowExplorer.Click += w_btnShowExplorer_Click;
            w_sourceExplorer.AddToolStripItem(w_btnShowExplorer);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Reporting.CompareDialog));
            this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.w_splitter = new System.Windows.Forms.SplitContainer();
            this.w_sourceExplorer = new Metalogix.UI.WinForms.Widgets.ExplorerControlWithLocation();
            this.w_targetExplorer = new Metalogix.UI.WinForms.Widgets.ExplorerControlWithLocation();
            this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_plComparison = new DevExpress.XtraEditors.PanelControl();
            this.w_comparisonOptionsControl = new Metalogix.SharePoint.UI.WinForms.Reporting.ComparisonOptionsControl();
            this.w_splitter.Panel1.SuspendLayout();
            this.w_splitter.Panel2.SuspendLayout();
            this.w_splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_plComparison).BeginInit();
            this.w_plComparison.SuspendLayout();
            base.SuspendLayout();
            this.w_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Enabled = false;
            this.w_btnOK.Location = new System.Drawing.Point(444, 367);
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Size = new System.Drawing.Size(75, 23);
            this.w_btnOK.TabIndex = 30;
            this.w_btnOK.Text = "&OK";
            this.w_btnOK.Click += new System.EventHandler(On_Ok_Clicked);
            this.w_splitter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.w_splitter.IsSplitterFixed = true;
            this.w_splitter.Location = new System.Drawing.Point(1, 0);
            this.w_splitter.Name = "w_splitter";
            this.w_splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.w_splitter.Panel1.Controls.Add(this.w_sourceExplorer);
            this.w_splitter.Panel2.Controls.Add(this.w_targetExplorer);
            this.w_splitter.Size = new System.Drawing.Size(331, 402);
            this.w_splitter.SplitterDistance = 25;
            this.w_splitter.SplitterWidth = 1;
            this.w_splitter.TabIndex = 1;
            this.w_sourceExplorer.Actions = new Metalogix.SharePoint.Actions.SharePointAction[0];
            this.w_sourceExplorer.BackColor = System.Drawing.Color.White;
            this.w_sourceExplorer.CheckBoxes = false;
            this.w_sourceExplorer.DataSource = null;
            this.w_sourceExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.w_sourceExplorer.Location = new System.Drawing.Point(0, 0);
            this.w_sourceExplorer.LocationDescriptor = "Compare:";
            this.w_sourceExplorer.MultiSelectEnabled = false;
            this.w_sourceExplorer.MultiSelectLimitationMethod = null;
            this.w_sourceExplorer.Name = "w_sourceExplorer";
            this.w_sourceExplorer.Size = new System.Drawing.Size(331, 25);
            this.w_sourceExplorer.TabIndex = 0;
            this.w_sourceExplorer.SelectedNodeChanged += new Metalogix.UI.WinForms.Widgets.ExplorerControl.SelectedNodeChangedHandler(On_SourceNode_Changed);
            this.w_targetExplorer.Actions = new Metalogix.SharePoint.Actions.SharePointAction[0];
            this.w_targetExplorer.BackColor = System.Drawing.Color.White;
            this.w_targetExplorer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.w_targetExplorer.CheckBoxes = false;
            this.w_targetExplorer.DataSource = null;
            this.w_targetExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.w_targetExplorer.Location = new System.Drawing.Point(0, 0);
            this.w_targetExplorer.LocationDescriptor = "To:";
            this.w_targetExplorer.MultiSelectEnabled = false;
            this.w_targetExplorer.MultiSelectLimitationMethod = null;
            this.w_targetExplorer.Name = "w_targetExplorer";
            this.w_targetExplorer.Size = new System.Drawing.Size(331, 376);
            this.w_targetExplorer.TabIndex = 1;
            this.w_targetExplorer.SelectedNodeChanged += new Metalogix.UI.WinForms.Widgets.ExplorerControl.SelectedNodeChangedHandler(On_Selected_Node_Changed);
            this.w_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Location = new System.Drawing.Point(525, 367);
            this.w_btnCancel.Name = "w_btnCancel";
            this.w_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.w_btnCancel.TabIndex = 40;
            this.w_btnCancel.Text = "&Cancel";
            this.w_plComparison.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.w_plComparison.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plComparison.Controls.Add(this.w_comparisonOptionsControl);
            this.w_plComparison.Location = new System.Drawing.Point(333, 0);
            this.w_plComparison.Name = "w_plComparison";
            this.w_plComparison.Size = new System.Drawing.Size(299, 361);
            this.w_plComparison.TabIndex = 24;
            this.w_comparisonOptionsControl.CompareSiteOptions = null;
            this.w_comparisonOptionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.w_comparisonOptionsControl.Level = Metalogix.SharePoint.UI.WinForms.Reporting.ComparisonOptionsControl.ComparisonLevel.Site;
            this.w_comparisonOptionsControl.Location = new System.Drawing.Point(0, 0);
            this.w_comparisonOptionsControl.Name = "w_comparisonOptionsControl";
            this.w_comparisonOptionsControl.Size = new System.Drawing.Size(299, 361);
            this.w_comparisonOptionsControl.TabIndex = 2;
            base.AcceptButton = this.w_btnOK;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.ClientSize = new System.Drawing.Size(630, 411);
            base.Controls.Add(this.w_plComparison);
            base.Controls.Add(this.w_splitter);
            base.Controls.Add(this.w_btnOK);
            base.Controls.Add(this.w_btnCancel);
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(638, 438);
            base.Name = "CompareDialog";
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CompareDialog";
            base.Load += new System.EventHandler(On_Load);
            this.w_splitter.Panel1.ResumeLayout(false);
            this.w_splitter.Panel2.ResumeLayout(false);
            this.w_splitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_plComparison).EndInit();
            this.w_plComparison.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void LoadUI()
        {
            SuspendLayout();
            if (m_sourceNode != null)
            {
                w_sourceExplorer.NavigateToNode(m_sourceNode);
                UpdateSourceUI();
            }
            if (m_targetNode != null)
            {
                w_targetExplorer.NavigateToNode(m_targetNode);
            }
            ResumeLayout();
        }

        private void On_Load(object sender, EventArgs e)
        {
            w_targetExplorer.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
            w_sourceExplorer.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
            LoadUI();
        }

        private void On_Ok_Clicked(object sender, EventArgs e)
        {
            SaveUI();
        }

        private void On_Selected_Node_Changed(ReadOnlyCollection<ExplorerTreeNode> selectedNodes)
        {
            UpdateComparable();
        }

        private void On_Selected_Node_Changed()
        {
        }

        private void On_SourceNode_Changed(ReadOnlyCollection<ExplorerTreeNode> node)
        {
            UpdateSourceUI();
            UpdateComparable();
        }

        private void On_SourceNode_Changed()
        {
        }

        private void SaveUI()
        {
            m_targetNode = (SPNode)w_targetExplorer.SelectedNode.Node;
            m_sourceNode = (SPNode)w_sourceExplorer.SelectedNode.Node;
            w_comparisonOptionsControl.SaveUI();
            m_options = w_comparisonOptionsControl.CompareSiteOptions;
        }

        private void ToggleExplorerState()
        {
            if (m_bMovingExplorers)
            {
                return;
            }
            m_bMovingExplorers = true;
            int splitterDistance = w_splitter.SplitterDistance;
            int num = 0;
            if (w_splitter.SplitterDistance != w_sourceExplorer.LocationBarHeight)
            {
                num = w_targetExplorer.LocationBarHeight;
                w_splitter.FixedPanel = FixedPanel.Panel1;
            }
            else
            {
                num = w_splitter.Height - w_sourceExplorer.LocationBarHeight;
                w_splitter.FixedPanel = FixedPanel.Panel2;
            }
            if (!UISettings.IsRemoteSession)
            {
                int num2 = w_splitter.Height / 25;
                int millisecondsTimeout = 200 / num2;
                int num3 = (num - splitterDistance) / num2 * 2;
                float num4 = (float)num3 / (float)num2;
                for (int i = 0; i < num2; i++)
                {
                    int i2 = w_splitter.SplitterDistance + (num3 - (int)((float)i * num4));
                    UpdateSplitterDistance(i2);
                    Thread.Sleep(millisecondsTimeout);
                }
            }
            else
            {
                UpdateSplitterDistance(num);
            }
            UpdateExplorerToggleButton();
            m_bMovingExplorers = false;
        }

        private void UpdateComparable()
        {
            bool enabled = false;
            if (w_targetExplorer.SelectedNode != null && w_sourceExplorer.SelectedNode != null)
            {
                SPNode sPNode = (SPNode)w_sourceExplorer.SelectedNode.Node;
                SPNode sPNode2 = (SPNode)w_targetExplorer.SelectedNode.Node;
                if (sPNode2 != null && sPNode != null)
                {
                    Type type = sPNode.GetType();
                    Type type2 = sPNode2.GetType();
                    if (type == typeof(SPSite) || type == typeof(SPWeb))
                    {
                        enabled = type2 == typeof(SPSite) || type2 == typeof(SPWeb);
                    }
                    else if (typeof(SPFolder).IsAssignableFrom(type) && typeof(SPFolder).IsAssignableFrom(type2))
                    {
                        enabled = !(type != type2) && ((SPFolder)sPNode).ParentList.BaseTemplate == ((SPFolder)sPNode2).ParentList.BaseTemplate;
                    }
                }
            }
            w_btnOK.Enabled = enabled;
        }

        private void UpdateExplorerToggleButton()
        {
            if (base.InvokeRequired)
            {
                Delegate method = new UpdateExplorerToggleButtonDelegate(UpdateExplorerToggleButton);
                w_splitter.Invoke(method);
            }
            else if (w_btnShowExplorer.Text == SHOW_EXPLORER)
            {
                w_btnShowExplorer.Text = HIDE_EXPLORER;
                w_btnShowExplorer.Image = Resources.SmallSubtract.ToBitmap();
            }
            else
            {
                w_btnShowExplorer.Text = SHOW_EXPLORER;
                w_btnShowExplorer.Image = Resources.SmallAdd.ToBitmap();
            }
        }

        private void UpdateSourceUI()
        {
            if (w_sourceExplorer.SelectedNodes.Count == 0)
            {
                return;
            }
            SPNode sPNode = (SPNode)w_sourceExplorer.SelectedNode.Node;
            if (sPNode == null)
            {
                w_comparisonOptionsControl.Level = ComparisonOptionsControl.ComparisonLevel.Site;
                return;
            }
            ComparisonOptionsControl.ComparisonLevel level = w_comparisonOptionsControl.Level;
            Type type = sPNode.GetType();
            if (typeof(SPWeb).IsAssignableFrom(type))
            {
                if (level != 0)
                {
                    ComparisonOptionsControl comparisonOptionsControl = new ComparisonOptionsControl();
                    w_comparisonOptionsControl.SaveUI();
                    comparisonOptionsControl.Anchor = w_comparisonOptionsControl.Anchor;
                    comparisonOptionsControl.CompareSiteOptions = w_comparisonOptionsControl.CompareSiteOptions;
                    comparisonOptionsControl.Location = w_comparisonOptionsControl.Location;
                    comparisonOptionsControl.Level = ComparisonOptionsControl.ComparisonLevel.Site;
                    comparisonOptionsControl.Size = w_comparisonOptionsControl.Size;
                    comparisonOptionsControl.Dock = w_comparisonOptionsControl.Dock;
                    w_plComparison.Controls.Remove(w_comparisonOptionsControl);
                    w_comparisonOptionsControl = comparisonOptionsControl;
                    w_plComparison.Controls.Add(w_comparisonOptionsControl);
                }
                Text = "Compare Site";
            }
            else if (typeof(SPFolder).IsAssignableFrom(type))
            {
                if (type != typeof(SPFolder))
                {
                    Text = "Compare List";
                }
                else
                {
                    Text = "Compare Folder";
                }
                if (level != ComparisonOptionsControl.ComparisonLevel.List)
                {
                    ComparisonOptionsControl comparisonOptionsControl2 = new ComparisonOptionsControl();
                    w_comparisonOptionsControl.SaveUI();
                    comparisonOptionsControl2.Anchor = w_comparisonOptionsControl.Anchor;
                    comparisonOptionsControl2.CompareSiteOptions = w_comparisonOptionsControl.CompareSiteOptions;
                    comparisonOptionsControl2.Location = w_comparisonOptionsControl.Location;
                    comparisonOptionsControl2.Level = ComparisonOptionsControl.ComparisonLevel.List;
                    comparisonOptionsControl2.Size = w_comparisonOptionsControl.Size;
                    comparisonOptionsControl2.Dock = w_comparisonOptionsControl.Dock;
                    w_plComparison.Controls.Remove(w_comparisonOptionsControl);
                    w_comparisonOptionsControl = comparisonOptionsControl2;
                    w_plComparison.Controls.Add(w_comparisonOptionsControl);
                }
            }
            MinimumSize = new Size(515, base.Height);
            Location location = null;
            if (w_targetExplorer.SelectedNode != null && w_targetExplorer.SelectedNode.Node != null)
            {
                location = w_targetExplorer.SelectedNode.Node.Location;
            }
            bool flag = false;
            if (w_targetExplorer.DataSource != null)
            {
                w_targetExplorer.DataSource = null;
                flag = true;
            }
            w_targetExplorer.NodeTypeFilter.Clear();
            w_targetExplorer.NodeTypeFilter.Add(typeof(SPServer));
            w_targetExplorer.NodeTypeFilter.Add(typeof(SPSite));
            w_targetExplorer.NodeTypeFilter.Add(typeof(SPWeb));
            if (typeof(SPFolder).IsAssignableFrom(type))
            {
                w_targetExplorer.NodeTypeFilter.Add(typeof(SPFolder));
                w_targetExplorer.NodeTypeFilter.Add(typeof(SPList));
                w_targetExplorer.NodeTypeFilter.Add(typeof(SPDiscussionList));
            }
            if (flag)
            {
                w_targetExplorer.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
                if (location != null)
                {
                    w_targetExplorer.NavigateToLocation(location);
                }
            }
        }

        private void UpdateSplitterDistance(int i)
        {
            if (!base.InvokeRequired)
            {
                w_splitter.SplitterDistance = i;
                return;
            }
            Delegate method = new UpdateSpitterDistanceDelegate(UpdateSplitterDistance);
            SplitContainer splitContainer = w_splitter;
            object[] args = new object[1] { i };
            splitContainer.Invoke(method, args);
        }

        private void w_btnShowExplorer_Click(object sender, EventArgs e)
        {
            new Thread(ToggleExplorerState).Start();
        }
    }
}
