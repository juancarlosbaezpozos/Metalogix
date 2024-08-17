using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
    [DesignTimeVisible(false)]
    public partial class ExplorerControl : HasSelectableObjects
    {
        protected NodeCollectionChangedHandler m_collectionChangedHandler;

        protected NodeCollection m_dataSource;

        protected Metalogix.Actions.Action[] m_actions = new Metalogix.Actions.Action[0];

        private IContainer components;

        private TreeViewX w_treeView;

        private ImageList w_imageList;

        public Metalogix.Actions.Action[] Actions
        {
            get
            {
                return this.m_actions;
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Value cannot be null");
                }
                this.m_actions = value;
            }
        }

        public bool CheckBoxes
        {
            get
            {
                return this.w_treeView.CheckBoxesEnhanced;
            }
            set
            {
                this.w_treeView.CheckBoxesEnhanced = value;
            }
        }

        public NodeCollection DataSource
        {
            get
            {
                return this.m_dataSource;
            }
            set
            {
                if (this.m_dataSource != null)
                {
                    this.m_dataSource.OnNodeCollectionChanged -= this.m_collectionChangedHandler;
                    this.m_collectionChangedHandler = null;
                }
                this.m_dataSource = value;
                if (this.m_dataSource != null)
                {
                    this.m_collectionChangedHandler = new NodeCollectionChangedHandler(this.OnCollectionChanged);
                    this.m_dataSource.OnNodeCollectionChanged += this.m_collectionChangedHandler;
                }
                this.UpdateUI();
            }
        }

        public bool MultiSelectEnabled
        {
            get
            {
                return this.w_treeView.MultiSelectEnabled;
            }
            set
            {
                this.w_treeView.MultiSelectEnabled = value;
            }
        }

        public EnhancedTreeView.AllowSelectionDelegate MultiSelectLimitationMethod
        {
            get
            {
                return this.w_treeView.MultiSelectLimitationMethod;
            }
            set
            {
                this.w_treeView.MultiSelectLimitationMethod = value;
            }
        }

        public List<Type> NodeTypeFilter
        {
            get
            {
                return this.w_treeView.NodeTypeFilter;
            }
        }

        public ExplorerTreeNode SelectedNode
        {
            get
            {
                ReadOnlyCollection<ExplorerTreeNode> selectedNodes = this.SelectedNodes;
                if (selectedNodes.Count == 0)
                {
                    return null;
                }
                return selectedNodes[selectedNodes.Count - 1];
            }
        }

        public ReadOnlyCollection<ExplorerTreeNode> SelectedNodes
        {
            get
            {
                List<ExplorerTreeNode> explorerTreeNodes = new List<ExplorerTreeNode>(this.w_treeView.SelectedNodes.Count);
                foreach (ExplorerTreeNode selectedNode in this.w_treeView.SelectedNodes)
                {
                    explorerTreeNodes.Add(selectedNode);
                }
                return new ReadOnlyCollection<ExplorerTreeNode>(explorerTreeNodes);
            }
        }

        public override IXMLAbleList SelectedObjects
        {
            get
            {
                if (this.SelectedNodes.Count <= 0)
                {
                    return new NodeCollection();
                }
                ReadOnlyCollection<ExplorerTreeNode> selectedNodes = this.SelectedNodes;
                Node[] node = new Node[selectedNodes.Count];
                for (int i = 0; i < selectedNodes.Count; i++)
                {
                    node[i] = selectedNodes[i].Node;
                }
                return new NodeCollection(node);
            }
        }

        public ExplorerControl()
        {
            this.InitializeComponent();
        }

        protected void AddNodeToUI(Node newNode)
        {
            if (base.InvokeRequired)
            {
                ExplorerControl.NodeChangeDelegate nodeChangeDelegate = new ExplorerControl.NodeChangeDelegate(this.AddNodeToUI);
                object[] objArray = new object[] { newNode };
                base.Invoke(nodeChangeDelegate, objArray);
                return;
            }
            if (newNode == null || this.NodeTypeFilter != null && this.NodeTypeFilter.Contains(newNode.GetType()))
            {
                return;
            }
            if (this.FindTargetRootNode(newNode) != null)
            {
                this.UpdateNodeInUI(newNode);
                return;
            }
            ExplorerTreeNode explorerTreeNode = this.CreateTreeNode(newNode, false);
            this.InsertNodeAtIndex(this.DataSource.IndexOf(newNode), explorerTreeNode);
            explorerTreeNode.UpdateUI();
            ExplorerControl.UpdateChildNodesUIAsync(explorerTreeNode);
        }

        protected void ClearNodes()
        {
            foreach (ExplorerTreeNode node in this.w_treeView.Nodes)
            {
                node.Dispose();
            }
            this.w_treeView.Nodes.Clear();
            this.Fire_SelectedNodeChanged();
            GC.Collect();
        }

        public ExplorerTreeNode CreateTreeNode(Node node)
        {
            return this.CreateTreeNode(node, true);
        }

        protected ExplorerTreeNode CreateTreeNode(Node node, bool bUpdateUI)
        {
            return new ExplorerTreeNode(node, this.w_treeView, bUpdateUI);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ExpandRoot()
        {
            if (this.w_treeView.Nodes.Count > 0)
            {
                this.w_treeView.SelectedNode = this.w_treeView.Nodes[0];
                this.w_treeView.SelectedNode.Expand();
            }
        }

        protected TreeNode FindNodeTarget(Node node)
        {
            if (node == null)
            {
                return null;
            }
            Stack<Node> nodes = new Stack<Node>();
            for (Node i = node; i != null; i = i.Parent)
            {
                nodes.Push(i);
            }
            ExplorerTreeNode explorerTreeNode = null;
            TreeNodeCollection treeNodeCollections = this.w_treeView.Nodes;
            base.SuspendLayout();
            while (nodes.Count > 0)
            {
                Node node1 = nodes.Pop();
                foreach (TreeNode treeNode in treeNodeCollections)
                {
                    if (!(treeNode is ExplorerTreeNode) || !object.Equals(((ExplorerTreeNode)treeNode).Node, node1))
                    {
                        continue;
                    }
                    explorerTreeNode = (ExplorerTreeNode)treeNode;
                    break;
                }
                if (explorerTreeNode == null)
                {
                    continue;
                }
                if (explorerTreeNode.Node.Connection.Status == ConnectionStatus.Invalid)
                {
                    break;
                }
                if (explorerTreeNode == null)
                {
                    continue;
                }
                explorerTreeNode.UpdateChildrenUI();
                if (treeNodeCollections == explorerTreeNode.Nodes)
                {
                    break;
                }
                treeNodeCollections = explorerTreeNode.Nodes;
            }
            return explorerTreeNode;
        }

        private ExplorerTreeNode FindTargetRootNode(Node node)
        {
            ExplorerTreeNode explorerTreeNode;
            if (node == null)
            {
                return null;
            }
            IEnumerator enumerator = this.w_treeView.Nodes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ExplorerTreeNode current = (ExplorerTreeNode)enumerator.Current;
                    if (current.Node != node)
                    {
                        continue;
                    }
                    explorerTreeNode = current;
                    return explorerTreeNode;
                }
                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return explorerTreeNode;
        }

        protected void Fire_SelectedNodeChanged()
        {
            if (this.SelectedNodeChanged != null)
            {
                this.SelectedNodeChanged(this.SelectedNodes);
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ExplorerControl));
            this.w_imageList = new ImageList(this.components);
            this.w_treeView = new TreeViewX();
            base.SuspendLayout();
            this.w_imageList.ColorDepth = ColorDepth.Depth32Bit;
            componentResourceManager.ApplyResources(this.w_imageList, "w_imageList");
            this.w_imageList.TransparentColor = Color.Transparent;
            this.w_treeView.BorderStyle = BorderStyle.None;
            this.w_treeView.CheckBoxesEnhanced = false;
            this.w_treeView.DeferErase = true;
            componentResourceManager.ApplyResources(this.w_treeView, "w_treeView");
            this.w_treeView.HideSelection = false;
            this.w_treeView.ImageList = this.w_imageList;
            this.w_treeView.ItemHeight = 18;
            this.w_treeView.MultiSelectEnabled = false;
            this.w_treeView.MultiSelectLimitationMethod = null;
            this.w_treeView.Name = "w_treeView";
            this.w_treeView.SelectedNodes = (ReadOnlyCollection<TreeNode>)componentResourceManager.GetObject("w_treeView.SelectedNodes");
            this.w_treeView.ShowLines = false;
            this.w_treeView.SmudgeProtection = false;
            this.w_treeView.UseDoubleBuffering = true;
            this.w_treeView.AfterMultiSelect += new EnhancedTreeView.MultiselectAfterEventDelegate(this.On_treeView_AfterMultiSelect);
            this.w_treeView.AfterExpand += new TreeViewEventHandler(this.On_treeView_AfterExpand);
            this.w_treeView.MouseDown += new MouseEventHandler(this.On_treeView_MouseDown);
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Transparent;
            base.Controls.Add(this.w_treeView);
            base.Name = "ExplorerControl";
            base.Load += new EventHandler(this.On_Load);
            base.Resize += new EventHandler(this.On_Explorer_Resized);
            base.ResumeLayout(false);
        }

        protected void InsertNodeAtIndex(int targetIdx, TreeNode node)
        {
            if (node == null)
            {
                return;
            }
            if (targetIdx >= 0 && targetIdx < this.w_treeView.Nodes.Count)
            {
                this.w_treeView.Nodes.Insert(targetIdx, node);
                return;
            }
            this.w_treeView.Nodes.Add(node);
        }

        protected void MoveNodeToIndex(int targetIdx, TreeNode node)
        {
            if (node == null || targetIdx < 0 || node.Index == targetIdx || node.Index == this.w_treeView.Nodes.Count - 1 && targetIdx >= this.w_treeView.Nodes.Count)
            {
                return;
            }
            node.Remove();
            if (targetIdx < this.w_treeView.Nodes.Count)
            {
                this.w_treeView.Nodes.Insert(targetIdx, node);
                return;
            }
            this.w_treeView.Nodes.Add(node);
        }

        public void NavigateToLocation(Location location)
        {
            this.NavigateToNode(location.GetNode());
        }

        public void NavigateToLocation(IEnumerable<Location> locations)
        {
            List<Node> nodes = new List<Node>();
            foreach (Location location in locations)
            {
                Node node = location.GetNode();
                if (nodes.Contains(node))
                {
                    continue;
                }
                nodes.Add(node);
            }
            this.NavigateToNode(nodes);
        }

        public void NavigateToNode(Node node)
        {
            this.NavigateToNode(new Node[] { node });
        }

        public void NavigateToNode(IEnumerable<Node> nodes)
        {
            this.Cursor = Cursors.WaitCursor;
            base.SuspendLayout();
            List<TreeNode> treeNodes = new List<TreeNode>();
            foreach (Node node in nodes)
            {
                TreeNode treeNode = this.FindNodeTarget(node);
                if (treeNode == null || treeNodes.Contains(treeNode))
                {
                    continue;
                }
                treeNodes.Add(treeNode);
            }
            if (treeNodes.Count > 0)
            {
                this.w_treeView.SelectedNodes = new ReadOnlyCollection<TreeNode>(treeNodes);
                this.Fire_SelectedNodeChanged();
            }
            base.ResumeLayout();
            this.Cursor = Cursors.Default;
        }

        protected void On_Explorer_Resized(object sender, EventArgs e)
        {
            this.w_treeView.Size = base.Size;
        }

        protected void On_Load(object sender, EventArgs e)
        {
            if (base.ParentForm != null)
            {
                base.ParentForm.FormClosed += new FormClosedEventHandler(this.On_Parent_Closed);
            }
        }

        protected void On_Parent_Closed(object sender, FormClosedEventArgs e)
        {
            this.ClearNodes();
            this.DataSource = null;
        }

        protected void On_treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            ExplorerControl.UpdateChildNodesUIAsync(e.Node.Nodes);
        }

        protected void On_treeView_AfterMultiSelect(EnhancedTreeView.MultiselectAfterEventArgs e)
        {
            try
            {
                TreeNode[] newlySelectedNodes = e.NewlySelectedNodes;
                this.Fire_SelectedNodeChanged();
                TreeNode[] treeNodeArray = newlySelectedNodes;
                for (int i = 0; i < (int)treeNodeArray.Length; i++)
                {
                    ExplorerTreeNode explorerTreeNode = (ExplorerTreeNode)treeNodeArray[i];
                    if (!explorerTreeNode.ChildrenUIUpToDate)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        explorerTreeNode.UpdateChildrenUIAsync();
                        this.Cursor = Cursors.Default;
                    }
                }
            }
            catch (WebException webException)
            {
                string str = string.Concat("Problem with server: ", webException.Message);
                FlatXtraMessageBox.Show(str, "Web Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch (Exception exception)
            {
            }
        }

        protected void On_treeView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    TreeNode nodeAt = this.w_treeView.GetNodeAt(e.X, e.Y);
                    if (nodeAt != null)
                    {
                        if (!this.w_treeView.SelectedNodes.Contains(nodeAt))
                        {
                            this.w_treeView.SelectedNodes = new ReadOnlyCollection<TreeNode>(new TreeNode[] { nodeAt });
                            this.Fire_SelectedNodeChanged();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
            }
        }

        protected void OnCollectionChanged(NodeCollectionChangeType changeType, Node node)
        {
            switch (changeType)
            {
                case NodeCollectionChangeType.NodeAdded:
                {
                    this.AddNodeToUI(node);
                    return;
                }
                case NodeCollectionChangeType.NodeRemoved:
                {
                    this.RemoveNodeFromUI(node);
                    return;
                }
                case NodeCollectionChangeType.NodeChanged:
                {
                    this.UpdateNodeInUI(node);
                    return;
                }
                case NodeCollectionChangeType.FullReset:
                {
                    this.UpdateUI();
                    return;
                }
                default:
                {
                    return;
                }
            }
        }

        public void Redraw()
        {
            try
            {
                this.w_treeView.BeginUpdate();
                foreach (ExplorerTreeNode node in this.w_treeView.Nodes)
                {
                    if (node == null)
                    {
                        continue;
                    }
                    node.Redraw();
                }
            }
            finally
            {
                this.w_treeView.EndUpdate();
            }
        }

        protected void RemoveNodeFromUI(Node removedNode)
        {
            if (!base.InvokeRequired)
            {
                ExplorerTreeNode explorerTreeNode = this.FindTargetRootNode(removedNode);
                if (explorerTreeNode != null)
                {
                    explorerTreeNode.Remove();
                }
                return;
            }
            ExplorerControl.NodeChangeDelegate nodeChangeDelegate = new ExplorerControl.NodeChangeDelegate(this.RemoveNodeFromUI);
            object[] objArray = new object[] { removedNode };
            base.Invoke(nodeChangeDelegate, objArray);
        }

        public void SelectNode(TreeNode node)
        {
            this.w_treeView.SelectedNode = node;
        }

        internal static void UpdateChildNodesUI(object treeNodesObj)
        {
            TreeNodeCollection treeNodeCollections = treeNodesObj as TreeNodeCollection;
            if (treeNodeCollections == null)
            {
                ExplorerTreeNode explorerTreeNode = treeNodesObj as ExplorerTreeNode;
                if (explorerTreeNode != null)
                {
                    ExplorerControl.UpdateChildNodeUI(explorerTreeNode);
                }
            }
            else
            {
                foreach (ExplorerTreeNode explorerTreeNode1 in treeNodeCollections)
                {
                    ExplorerControl.UpdateChildNodeUI(explorerTreeNode1);
                }
            }
        }

        internal static void UpdateChildNodesUIAsync(TreeNodeCollection treeNodes)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ExplorerControl.UpdateChildNodesUI));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(treeNodes);
        }

        internal static void UpdateChildNodesUIAsync(TreeNode treeNode)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ExplorerControl.UpdateChildNodesUI));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(treeNode);
        }

        internal static void UpdateChildNodeUI(ExplorerTreeNode treeNode)
        {
            if (treeNode != null && treeNode.Node != null && treeNode.Node.LoadAutomatically)
            {
                treeNode.UpdateChildrenUI();
            }
        }

        protected void UpdateNodeInUI(Node updatedNode)
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new ExplorerControl.NodeChangeDelegate(this.UpdateNodeInUI));
                return;
            }
            ExplorerTreeNode explorerTreeNode = this.FindTargetRootNode(updatedNode);
            if (explorerTreeNode == null)
            {
                return;
            }
            this.w_treeView.BeginUpdate();
            try
            {
                this.MoveNodeToIndex(this.DataSource.IndexOf(updatedNode), explorerTreeNode);
                explorerTreeNode.UpdateUI();
            }
            finally
            {
                this.w_treeView.EndUpdate();
            }
            ExplorerControl.UpdateChildNodesUIAsync(explorerTreeNode.Nodes);
        }

        protected void UpdateUI()
        {
            if (base.InvokeRequired)
            {
                base.Invoke(new ExplorerControl.UpdateUIDelegate(this.UpdateUI));
                return;
            }
            if (this.DataSource == null || this.DataSource.Count == 0)
            {
                this.ClearNodes();
                return;
            }
            this.w_treeView.BeginUpdate();
            try
            {
                ExplorerTreeNode[] explorerTreeNodeArray = new ExplorerTreeNode[this.w_treeView.Nodes.Count];
                this.w_treeView.Nodes.CopyTo(explorerTreeNodeArray, 0);
                ExplorerTreeNode[] explorerTreeNodeArray1 = explorerTreeNodeArray;
                for (int i = 0; i < (int)explorerTreeNodeArray1.Length; i++)
                {
                    ExplorerTreeNode explorerTreeNode = explorerTreeNodeArray1[i];
                    if (!this.DataSource.Contains(explorerTreeNode.Node))
                    {
                        explorerTreeNode.Remove();
                        explorerTreeNode.Dispose();
                    }
                }
                for (int j = 0; j < this.DataSource.Count; j++)
                {
                    Node item = this.DataSource[j] as Node;
                    if (item != null)
                    {
                        ExplorerTreeNode explorerTreeNode1 = this.FindTargetRootNode(item);
                        if (this.NodeTypeFilter != null && this.NodeTypeFilter.Count != 0 && !this.NodeTypeFilter.Contains(item.GetType()))
                        {
                            if (explorerTreeNode1 != null)
                            {
                                explorerTreeNode1.Remove();
                                explorerTreeNode1.Dispose();
                                explorerTreeNode1 = null;
                            }
                        }
                        else if (explorerTreeNode1 == null)
                        {
                            explorerTreeNode1 = this.CreateTreeNode(item, false);
                            this.InsertNodeAtIndex(j, explorerTreeNode1);
                        }
                        else if (explorerTreeNode1.Index != j)
                        {
                            this.MoveNodeToIndex(j, explorerTreeNode1);
                        }
                        if (explorerTreeNode1 != null)
                        {
                            explorerTreeNode1.UpdateUI();
                        }
                    }
                }
            }
            finally
            {
                this.w_treeView.EndUpdate();
            }
            ExplorerControl.UpdateChildNodesUIAsync(this.w_treeView.Nodes);
        }

        public event ExplorerControl.SelectedNodeChangedHandler SelectedNodeChanged;

        protected delegate void NodeChangeDelegate(Node changedNode);

        public delegate void SelectedNodeChangedHandler(ReadOnlyCollection<ExplorerTreeNode> selectedNodes);

        protected delegate void UpdateUIDelegate();
    }
}