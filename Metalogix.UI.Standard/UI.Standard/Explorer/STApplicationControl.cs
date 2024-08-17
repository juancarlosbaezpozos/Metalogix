using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;
using Metalogix.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.Standard.Explorer
{
    [DesignTimeVisible(true)]
    public partial class STApplicationControl : ApplicationControl
    {
        private IContainer components;

        private MLTabControl w_tabControl;

        private TabPage w_tabPage1;

        private TabPage w_tabPageItemsView;

        private TabPage w_tabPageBrowser;

        private LocationBar w_locationBar;

        private Panel w_plItemsView;

        private SplitContainer w_splitContainerItemsViews;

        private Panel panel1;

        private Label w_lblVersionHistory;

        private Panel panel2;

        private STItemsViewControlFull w_itemsView;

        private BrowserControl w_browserControl;

        private ExplorerControlWithNavigation w_spExplorerControl;

        public override Metalogix.Actions.Action[] Actions
        {
            get
            {
                return this.w_spExplorerControl.Actions;
            }
            set
            {
                this.w_spExplorerControl.Actions = value;
            }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
                this.w_spExplorerControl.ContextMenuStrip = value;
                this.w_itemsView.ContextMenuStrip = value;
            }
        }

        public override NodeCollection DataSource
        {
            get
            {
                return this.w_spExplorerControl.DataSource;
            }
            set
            {
                this.w_spExplorerControl.DataSource = value;
                this.w_itemsView.DataSource = null;
            }
        }

        public override bool ExplorerMultiSelectEnabled
        {
            get
            {
                return this.w_spExplorerControl.MultiSelectEnabled;
            }
            set
            {
                this.w_spExplorerControl.MultiSelectEnabled = value;
            }
        }

        public override EnhancedTreeView.AllowSelectionDelegate ExplorerMultiSelectLimitationMethod
        {
            get
            {
                return this.w_spExplorerControl.MultiSelectLimitationMethod;
            }
            set
            {
                this.w_spExplorerControl.MultiSelectLimitationMethod = value;
            }
        }

        public override IDataConverter<object, string> ItemsViewDataConverter
        {
            get
            {
                return this.w_itemsView.DataConverter;
            }
            set
            {
                this.w_itemsView.DataConverter = value;
            }
        }

        public override List<Type> NodeTypeFilter
        {
            get
            {
                return this.w_spExplorerControl.NodeTypeFilter;
            }
        }

        public override ExplorerTreeNode SelectedNode
        {
            get
            {
                return this.w_spExplorerControl.SelectedNode;
            }
        }

        public override ReadOnlyCollection<ExplorerTreeNode> SelectedNodes
        {
            get
            {
                return this.w_spExplorerControl.SelectedNodes;
            }
        }

        public override IXMLAbleList SelectedObjects
        {
            get
            {
                if (this.w_tabControl.SelectedTab == this.w_tabPage1)
                {
                    return this.w_spExplorerControl.SelectedObjects;
                }
                if (this.w_tabControl.SelectedTab != this.w_tabPageItemsView)
                {
                    return null;
                }
                return this.w_itemsView.SelectedObjects;
            }
        }

        public override bool ShowExplorerCheckBoxes
        {
            get
            {
                return this.w_spExplorerControl.CheckBoxes;
            }
            set
            {
                this.w_spExplorerControl.CheckBoxes = value;
            }
        }

        public STApplicationControl()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            this.w_itemsView.SelectedListItemsChanged += new ItemsViewControlFull.SelectedListItemChangedHandler(this.On_SelectedItems_Changed);
            this.w_locationBar.SelectedNodeChanged += new LocationBar.SelectedNodeChangedHandler(this.FireSelectedNodeChanged);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(STApplicationControl));
            this.w_locationBar = new LocationBar();
            this.w_tabControl = new MLTabControl();
            this.w_tabPage1 = new TabPage();
            this.w_spExplorerControl = new ExplorerControlWithNavigation();
            this.w_tabPageItemsView = new TabPage();
            this.w_splitContainerItemsViews = new SplitContainer();
            this.w_plItemsView = new Panel();
            this.w_itemsView = new STItemsViewControlFull();
            this.panel2 = new Panel();
            this.panel1 = new Panel();
            this.w_lblVersionHistory = new Label();
            this.w_tabPageBrowser = new TabPage();
            this.w_browserControl = new BrowserControl();
            this.w_tabControl.SuspendLayout();
            this.w_tabPage1.SuspendLayout();
            this.w_tabPageItemsView.SuspendLayout();
            this.w_splitContainerItemsViews.Panel1.SuspendLayout();
            this.w_splitContainerItemsViews.Panel2.SuspendLayout();
            this.w_splitContainerItemsViews.SuspendLayout();
            this.w_plItemsView.SuspendLayout();
            this.panel1.SuspendLayout();
            this.w_tabPageBrowser.SuspendLayout();
            base.SuspendLayout();
            this.w_locationBar.BackColor = Color.Transparent;
            this.w_locationBar.Descriptor = "Location:";
            componentResourceManager.ApplyResources(this.w_locationBar, "w_locationBar");
            this.w_locationBar.Name = "w_locationBar";
            this.w_locationBar.Node = null;
            this.w_tabControl.Controls.Add(this.w_tabPage1);
            this.w_tabControl.Controls.Add(this.w_tabPageItemsView);
            this.w_tabControl.Controls.Add(this.w_tabPageBrowser);
            componentResourceManager.ApplyResources(this.w_tabControl, "w_tabControl");
            this.w_tabControl.Multiline = true;
            this.w_tabControl.Name = "w_tabControl";
            this.w_tabControl.SelectedIndex = 0;
            this.w_tabControl.SelectedIndexChanged += new EventHandler(this.On_tabControl_SelectedIndexChanged);
            this.w_tabControl.Deselecting += new TabControlCancelEventHandler(this.On_TabPage_Deselecting);
            this.w_tabPage1.BackColor = Color.White;
            this.w_tabPage1.Controls.Add(this.w_spExplorerControl);
            componentResourceManager.ApplyResources(this.w_tabPage1, "w_tabPage1");
            this.w_tabPage1.Name = "w_tabPage1";
            this.w_spExplorerControl.Actions = new Metalogix.Actions.Action[0];
            this.w_spExplorerControl.BackColor = Color.White;
            this.w_spExplorerControl.CheckBoxes = false;
            this.w_spExplorerControl.DataSource = null;
            componentResourceManager.ApplyResources(this.w_spExplorerControl, "w_spExplorerControl");
            this.w_spExplorerControl.MultiSelectEnabled = false;
            this.w_spExplorerControl.MultiSelectLimitationMethod = null;
            this.w_spExplorerControl.Name = "w_spExplorerControl";
            this.w_spExplorerControl.SelectedNodeChanged += new ExplorerControl.SelectedNodeChangedHandler(this.On_ExplorereNode_Changed);
            this.w_tabPageItemsView.BackColor = Color.White;
            this.w_tabPageItemsView.Controls.Add(this.w_splitContainerItemsViews);
            componentResourceManager.ApplyResources(this.w_tabPageItemsView, "w_tabPageItemsView");
            this.w_tabPageItemsView.Name = "w_tabPageItemsView";
            componentResourceManager.ApplyResources(this.w_splitContainerItemsViews, "w_splitContainerItemsViews");
            this.w_splitContainerItemsViews.Name = "w_splitContainerItemsViews";
            this.w_splitContainerItemsViews.Panel1.Controls.Add(this.w_plItemsView);
            this.w_splitContainerItemsViews.Panel2.Controls.Add(this.panel2);
            this.w_splitContainerItemsViews.Panel2.Controls.Add(this.panel1);
            this.w_splitContainerItemsViews.Panel2Collapsed = true;
            this.w_plItemsView.BackColor = SystemColors.Window;
            this.w_plItemsView.Controls.Add(this.w_itemsView);
            componentResourceManager.ApplyResources(this.w_plItemsView, "w_plItemsView");
            this.w_plItemsView.Name = "w_plItemsView";
            this.w_itemsView.BackColor = Color.White;
            this.w_itemsView.DataConverter = null;
            this.w_itemsView.DataSource = null;
            componentResourceManager.ApplyResources(this.w_itemsView, "w_itemsView");
            this.w_itemsView.ItemsViewContextMenu = null;
            this.w_itemsView.Name = "w_itemsView";
            this.w_itemsView.ViewFields = null;
            componentResourceManager.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            this.panel1.Controls.Add(this.w_lblVersionHistory);
            componentResourceManager.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            componentResourceManager.ApplyResources(this.w_lblVersionHistory, "w_lblVersionHistory");
            this.w_lblVersionHistory.Name = "w_lblVersionHistory";
            this.w_tabPageBrowser.BackColor = Color.White;
            this.w_tabPageBrowser.Controls.Add(this.w_browserControl);
            componentResourceManager.ApplyResources(this.w_tabPageBrowser, "w_tabPageBrowser");
            this.w_tabPageBrowser.Name = "w_tabPageBrowser";
            this.w_browserControl.BackColor = Color.White;
            componentResourceManager.ApplyResources(this.w_browserControl, "w_browserControl");
            this.w_browserControl.Name = "w_browserControl";
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.Controls.Add(this.w_tabControl);
            base.Controls.Add(this.w_locationBar);
            base.Name = "STApplicationControl";
            this.w_tabControl.ResumeLayout(false);
            this.w_tabPage1.ResumeLayout(false);
            this.w_tabPageItemsView.ResumeLayout(false);
            this.w_splitContainerItemsViews.Panel1.ResumeLayout(false);
            this.w_splitContainerItemsViews.Panel2.ResumeLayout(false);
            this.w_splitContainerItemsViews.ResumeLayout(false);
            this.w_plItemsView.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.w_tabPageBrowser.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        public override void NavigateToLocation(Location location)
        {
            this.w_spExplorerControl.NavigateToLocation(location);
        }

        public override void NavigateToLocation(IEnumerable<Location> locations)
        {
            this.w_spExplorerControl.NavigateToLocation(locations);
        }

        public override void NavigateToNode(Node node)
        {
            this.w_spExplorerControl.NavigateToNode(node);
        }

        public override void NavigateToNode(IEnumerable<Node> nodes)
        {
            this.w_spExplorerControl.NavigateToNode(nodes);
        }

        private void On_ExplorereNode_Changed(ReadOnlyCollection<ExplorerTreeNode> selectedNodes)
        {
            Node node = null;
            if (selectedNodes.Count > 0)
            {
                node = selectedNodes[selectedNodes.Count - 1].Node;
            }
            bool flag = false;
            bool flag1 = false;
            if (node != null && selectedNodes.Count == 1)
            {
                if (node is Folder)
                {
                    flag = true;
                }
                if (node.LinkableUrl != null)
                {
                    flag1 = true;
                }
            }
            this.w_locationBar.Node = node;
            this.w_tabControl.TabPages.Remove(this.w_tabPageBrowser);
            this.UpdateTabState(this.w_tabPageItemsView, flag);
            this.UpdateTabState(this.w_tabPageBrowser, flag1);
            if (!flag || this.w_tabControl.SelectedTab != this.w_tabPageItemsView)
            {
                this.w_itemsView.ClearItemsView();
            }
            else
            {
                this.w_itemsView.SetDataSourceAsync((Folder)node);
            }
            if (flag1 && this.w_tabControl.SelectedTab == this.w_tabPageBrowser && node != null && node.LinkableUrl != null && !GUIService.FireBrowserNavigate(this.w_browserControl.Browser, node, node.LinkableUrl))
            {
                this.w_browserControl.Browser.Navigate(node.LinkableUrl);
            }
        }

        private void On_SelectedItems_Changed(ListItemCollection changedItems)
        {
            this.UpdateSelectedItems(changedItems);
        }

        private void On_tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ExplorerTreeNode selectedNode = this.w_spExplorerControl.SelectedNode;
                if (this.w_tabControl.SelectedTab == this.w_tabPageItemsView)
                {
                    if (selectedNode == null || !(selectedNode.Node is Folder))
                    {
                        this.w_itemsView.ClearItemsView();
                    }
                    else
                    {
                        this.w_itemsView.SetDataSourceAsync((Folder)selectedNode.Node);
                    }
                }
                else if (this.w_tabControl.SelectedTab == this.w_tabPageBrowser)
                {
                    Node node = this.w_locationBar.Node;
                    if (node != null && node.LinkableUrl != null && !GUIService.FireBrowserNavigate(this.w_browserControl.Browser, node, node.LinkableUrl))
                    {
                        this.w_browserControl.Browser.Navigate(node.LinkableUrl);
                    }
                }
                else if (selectedNode != null && selectedNode.Node != null && selectedNode.Node.Status == ConnectionStatus.Valid)
                {
                    this.w_locationBar.Node = selectedNode.Node;
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void On_TabPage_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == this.w_tabPageItemsView)
            {
                ListItemCollection dataSource = this.w_itemsView.DataSource;
                if (dataSource != null)
                {
                    this.w_itemsView.ClearItemsView();
                    if (dataSource.ParentFolder is ExplorerNode)
                    {
                        ((ExplorerNode)dataSource.ParentFolder).Dispose();
                    }
                    dataSource = null;
                }
            }
        }

        public override void Redraw()
        {
            this.w_spExplorerControl.Redraw();
        }

        private void UpdateSelectedItems(ListItemCollection changedItems)
        {
            Node node;
            if (base.InvokeRequired)
            {
                Delegate updateSelectedItemsDelegate = new STApplicationControl.UpdateSelectedItemsDelegate(this.UpdateSelectedItems);
                object[] objArray = new object[] { changedItems };
                base.Invoke(updateSelectedItemsDelegate, objArray);
                return;
            }
            if (this.w_tabControl.SelectedTab == this.w_tabPageItemsView)
            {
                if (changedItems == null || changedItems.Count != 1)
                {
                    LocationBar wLocationBar = this.w_locationBar;
                    if (this.w_spExplorerControl.SelectedNode != null)
                    {
                        node = this.w_spExplorerControl.SelectedNode.Node;
                    }
                    else
                    {
                        node = null;
                    }
                    wLocationBar.Node = node;
                    return;
                }
                this.w_locationBar.Node = (Node)changedItems[0];
            }
        }

        private void UpdateTabState(TabPage tab, bool bShowTab)
        {
            if (tab == this.w_tabPage1)
            {
                return;
            }
            if (this.w_tabControl.TabPages.Contains(tab))
            {
                if (!bShowTab)
                {
                    if (this.w_tabControl.SelectedTab == tab)
                    {
                        this.w_tabControl.SelectedTab = this.w_tabPage1;
                    }
                    this.w_tabControl.TabPages.Remove(tab);
                    return;
                }
            }
            else if (bShowTab)
            {
                if (tab == this.w_tabPageItemsView)
                {
                    this.w_tabControl.TabPages.Insert(1, tab);
                    return;
                }
                this.w_tabControl.TabPages.Add(tab);
            }
        }

        private delegate void UpdateSelectedItemsDelegate(ListItemCollection changedItems);
    }
}