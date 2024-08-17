using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
    public partial class NodeChooserDialog : XtraForm
    {
        private Node m_node;

        private IContainer components;

        private ExplorerControl w_spExplorerControl;

        private SimpleButton w_btnOK;

        private SimpleButton w_btnCancel;

        private LabelControl w_labelMessage;

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

        public string Message
        {
            get
            {
                return this.w_labelMessage.Text;
            }
            set
            {
                this.w_labelMessage.Text = value;
            }
        }

        public List<Type> NodeTypeFilter
        {
            get
            {
                return this.w_spExplorerControl.NodeTypeFilter;
            }
        }

        public Node SelectedNode
        {
            get
            {
                return this.m_node;
            }
            set
            {
                this.w_spExplorerControl.NavigateToLocation(value.Location);
            }
        }

        public NodeChooserDialog()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NodeChooserDialog));
            this.w_btnOK = new SimpleButton();
            this.w_btnCancel = new SimpleButton();
            this.w_labelMessage = new LabelControl();
            this.w_spExplorerControl = new ExplorerControl();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new EventHandler(this.On_btnOK_Click);
            componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            componentResourceManager.ApplyResources(this.w_labelMessage, "w_labelMessage");
            this.w_labelMessage.Name = "w_labelMessage";
            this.w_spExplorerControl.Actions = new Metalogix.Actions.Action[0];
            componentResourceManager.ApplyResources(this.w_spExplorerControl, "w_spExplorerControl");
            this.w_spExplorerControl.BackColor = Color.Transparent;
            this.w_spExplorerControl.BorderStyle = BorderStyle.FixedSingle;
            this.w_spExplorerControl.CheckBoxes = false;
            this.w_spExplorerControl.DataSource = null;
            this.w_spExplorerControl.MultiSelectEnabled = false;
            this.w_spExplorerControl.MultiSelectLimitationMethod = null;
            this.w_spExplorerControl.Name = "w_spExplorerControl";
            this.w_spExplorerControl.SelectedNodeChanged += new ExplorerControl.SelectedNodeChangedHandler(this.On_Explorer_SelectedNodeChanged);
            base.AcceptButton = this.w_btnOK;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_labelMessage);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.Controls.Add(this.w_spExplorerControl);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "NodeChooserDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_btnOK_Click(object sender, EventArgs e)
        {
            if (this.SelectedNode == null)
            {
                FlatXtraMessageBox.Show("No node was selected", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
                return;
            }
            if (!(this.SelectedNode is Connection) || this.SelectedNode.Status == ConnectionStatus.Valid)
            {
                this.w_spExplorerControl.SelectedNodeChanged -= new ExplorerControl.SelectedNodeChangedHandler(this.On_Explorer_SelectedNodeChanged);
                return;
            }
            FlatXtraMessageBox.Show(string.Concat("Invalid connection: ", this.SelectedNode.ErrorDescription), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            base.DialogResult = DialogResult.None;
        }

        private void On_Explorer_SelectedNodeChanged(ReadOnlyCollection<ExplorerTreeNode> nodes)
        {
            Node node;
            if (nodes.Count <= 0)
            {
                this.m_node = null;
                return;
            }
            ExplorerTreeNode item = nodes[nodes.Count - 1];
            if (item == null)
            {
                node = null;
            }
            else
            {
                node = item.Node;
            }
            this.m_node = node;
        }
    }
}