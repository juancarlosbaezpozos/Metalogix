using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
    public partial class ExplorerControlWithLocation : UserControl, IHasSelectableObjects
    {
        private IContainer components;

        private ExplorerControlWithNavigation w_spExplorerControl;

        private LocationBar w_locationBar;

        public Metalogix.Actions.Action[] Actions
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

        public bool CheckBoxes
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

        public NodeCollection DataSource
        {
            get
            {
                return this.w_spExplorerControl.DataSource;
            }
            set
            {
                this.w_spExplorerControl.DataSource = value;
            }
        }

        public int LocationBarHeight
        {
            get
            {
                return this.w_locationBar.Height - 1;
            }
        }

        public string LocationDescriptor
        {
            get
            {
                return this.w_locationBar.Descriptor;
            }
            set
            {
                this.w_locationBar.Descriptor = value;
            }
        }

        public bool MultiSelectEnabled
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

        public EnhancedTreeView.AllowSelectionDelegate MultiSelectLimitationMethod
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

        public List<Type> NodeTypeFilter
        {
            get
            {
                return this.w_spExplorerControl.NodeTypeFilter;
            }
        }

        public ExplorerTreeNode SelectedNode
        {
            get
            {
                return this.w_spExplorerControl.SelectedNode;
            }
        }

        public ReadOnlyCollection<ExplorerTreeNode> SelectedNodes
        {
            get
            {
                return this.w_spExplorerControl.SelectedNodes;
            }
        }

        public IXMLAbleList SelectedObjects
        {
            get
            {
                return this.w_spExplorerControl.SelectedObjects;
            }
        }

        public ExplorerControlWithLocation()
        {
            this.InitializeComponent();
        }

        public void AddToolStripItem(ToolStripItem item)
        {
            this.w_locationBar.AddToolStripItem(item);
        }

        public ExplorerTreeNode CreateTreeNode(Node node)
        {
            return this.w_spExplorerControl.CreateTreeNode(node);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Fire_SelectedNodeChanged()
        {
            if (this.SelectedNodeChanged != null)
            {
                this.SelectedNodeChanged(this.SelectedNodes);
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ExplorerControlWithLocation));
            this.w_locationBar = new LocationBar();
            this.w_spExplorerControl = new ExplorerControlWithNavigation();
            base.SuspendLayout();
            this.w_locationBar.BackColor = Color.Transparent;
            this.w_locationBar.Descriptor = "Location:";
            componentResourceManager.ApplyResources(this.w_locationBar, "w_locationBar");
            this.w_locationBar.Name = "w_locationBar";
            this.w_locationBar.Node = null;
            this.w_spExplorerControl.Actions = new Metalogix.Actions.Action[0];
            this.w_spExplorerControl.BackColor = Color.White;
            this.w_spExplorerControl.CheckBoxes = false;
            this.w_spExplorerControl.DataSource = null;
            componentResourceManager.ApplyResources(this.w_spExplorerControl, "w_spExplorerControl");
            this.w_spExplorerControl.MultiSelectEnabled = false;
            this.w_spExplorerControl.MultiSelectLimitationMethod = null;
            this.w_spExplorerControl.Name = "w_spExplorerControl";
            this.w_spExplorerControl.SelectedNodeChanged += new ExplorerControl.SelectedNodeChangedHandler(this.On_Explorer_SelectedNodeChanged);
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.Controls.Add(this.w_spExplorerControl);
            base.Controls.Add(this.w_locationBar);
            base.Name = "ExplorerControlWithLocation";
            base.ResumeLayout(false);
        }

        public void NavigateToLocation(Location location)
        {
            this.w_spExplorerControl.NavigateToLocation(location);
        }

        public void NavigateToNode(Node node)
        {
            this.w_spExplorerControl.NavigateToNode(node);
        }

        private void On_Explorer_SelectedNodeChanged(ReadOnlyCollection<ExplorerTreeNode> selectedNodes)
        {
            ExplorerTreeNode item = null;
            if (selectedNodes.Count > 0)
            {
                item = selectedNodes[selectedNodes.Count - 1];
            }
            if (item == null || item.Node == null || item.Node is DummyNode)
            {
                this.w_locationBar.Node = null;
            }
            else
            {
                this.w_locationBar.Node = item.Node;
            }
            this.Fire_SelectedNodeChanged();
        }

        public void RemoveToolSTripItem(ToolStripItem item)
        {
            this.w_locationBar.RemoveToolSTripItem(item);
        }

        public event ExplorerControl.SelectedNodeChangedHandler SelectedNodeChanged;
    }
}