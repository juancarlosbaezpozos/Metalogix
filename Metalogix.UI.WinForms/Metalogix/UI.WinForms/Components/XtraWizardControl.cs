using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace Metalogix.UI.WinForms.Components
{
    public class XtraWizardControl : XtraUserControl
    {
        private IContainer components;

        private TableLayoutPanel baseTableLayoutPanel;

        private XtraTabControl wizardTabControl;

        private TreeView tvWizard;

        public XtraWizardControl()
	{
		InitializeComponent();
		tvWizard.TabStop = false;
	}

        public void AddTab(TabbableControl control, int nodeIndex, bool isFirstChildNode)
	{
		wizardTabControl.AddTab(control);
		AddTreeNode(control.TabName, nodeIndex, isFirstChildNode);
	}

        private void AddTreeNode(string nodeName, int index, bool isFirstChildNode)
	{
		string str = ((index == -1) ? string.Empty : $"{index}. ");
		if (isFirstChildNode)
		{
			TreeNode treeNode = tvWizard.Nodes.Add($"{str}{nodeName}");
			tvWizard.SelectedNode = treeNode;
		}
		else
		{
			TreeNode selectedNode = tvWizard.SelectedNode;
			selectedNode.Nodes.Add($"{str}{nodeName}");
			selectedNode.Expand();
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

        public int GetSelectedTabIndex()
	{
		return wizardTabControl.SelectedTabPageIndex;
	}

        public int GetTabsCount()
	{
		return wizardTabControl.TabPages.Count;
	}

        public void GoToNextTab()
	{
		if (wizardTabControl.SelectedTabPageIndex < 0)
		{
			return;
		}
		XtraWizardTabbableControl selectedPageControl = wizardTabControl.GetSelectedPageControl();
		if (selectedPageControl == null)
		{
			throw new Exception("Control not found on current tab.");
		}
		if (!selectedPageControl.ValidatePage())
		{
			return;
		}
		selectedPageControl.SaveUI();
		wizardTabControl.SelectedTabPageIndex += 1;
		UpdateTreeNodeColor(tvWizard.SelectedNode.NextVisibleNode);
		if (wizardTabControl.SelectedTabPageIndex >= 0)
		{
			selectedPageControl = wizardTabControl.GetSelectedPageControl();
			if (selectedPageControl == null)
			{
				throw new Exception("Control not found on next tab.");
			}
			selectedPageControl.LoadUI();
		}
	}

        public void GoToPreviousTab()
	{
		wizardTabControl.SelectedTabPageIndex -= 1;
		UpdateTreeNodeColor(tvWizard.SelectedNode.PrevVisibleNode);
	}

        public void GoToSpecificTab(string controlName)
	{
		int num = 0;
		foreach (XtraTabPage tabPage in wizardTabControl.TabPages)
		{
			if (tabPage.Controls != null && tabPage.Controls[0].Name.Equals(controlName, StringComparison.InvariantCultureIgnoreCase))
			{
				break;
			}
			num++;
		}
		if (num > 0)
		{
			wizardTabControl.SelectedTabPageIndex = num;
			if (wizardTabControl.SelectedTabPageIndex == wizardTabControl.TabPages.Count - 1)
			{
				UpdateTreeNodeColor(tvWizard.Nodes[tvWizard.Nodes.Count - 1]);
			}
			else
			{
				UpdateTreeNodeColor(tvWizard.SelectedNode.PrevNode);
			}
		}
	}

        private void InitializeComponent()
	{
		this.baseTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.tvWizard = new System.Windows.Forms.TreeView();
		this.wizardTabControl = new Metalogix.UI.WinForms.Components.XtraTabControl();
		this.baseTableLayoutPanel.SuspendLayout();
		base.SuspendLayout();
		this.baseTableLayoutPanel.ColumnCount = 2;
		this.baseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25f));
		this.baseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75f));
		this.baseTableLayoutPanel.Controls.Add(this.wizardTabControl, 1, 0);
		this.baseTableLayoutPanel.Controls.Add(this.tvWizard, 0, 0);
		this.baseTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.baseTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.baseTableLayoutPanel.Name = "baseTableLayoutPanel";
		this.baseTableLayoutPanel.RowCount = 1;
		this.baseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.baseTableLayoutPanel.Size = new System.Drawing.Size(612, 432);
		this.baseTableLayoutPanel.TabIndex = 0;
		this.tvWizard.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tvWizard.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tvWizard.Enabled = false;
		this.tvWizard.Location = new System.Drawing.Point(3, 3);
		this.tvWizard.Name = "tvWizard";
		this.tvWizard.ShowLines = false;
		this.tvWizard.ShowPlusMinus = false;
		this.tvWizard.ShowRootLines = false;
		this.tvWizard.Size = new System.Drawing.Size(147, 426);
		this.tvWizard.TabIndex = 1;
		this.wizardTabControl.Appearance.BackColor = System.Drawing.Color.White;
		this.wizardTabControl.Appearance.Options.UseBackColor = true;
		this.wizardTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wizardTabControl.Location = new System.Drawing.Point(156, 3);
		this.wizardTabControl.Name = "wizardTabControl";
		this.wizardTabControl.SelectedTabPageIndex = -1;
		this.wizardTabControl.Size = new System.Drawing.Size(453, 426);
		this.wizardTabControl.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.baseTableLayoutPanel);
		base.Name = "XtraWizardControl";
		base.Size = new System.Drawing.Size(612, 432);
		base.Load += new System.EventHandler(XtraWizardControl_Load);
		this.baseTableLayoutPanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        private void SetNodesColor(TreeNode node)
	{
		do
		{
			node.BackColor = Color.White;
			node.ForeColor = Color.Black;
			node = node.NextVisibleNode;
		}
		while (node != null);
	}

        private void UpdateTreeNodeColor(TreeNode node)
	{
		if (tvWizard.SelectedNode != null)
		{
			tvWizard.SelectedNode.NodeFont = new Font(tvWizard.Font, FontStyle.Regular);
			tvWizard.SelectedNode.ForeColor = Color.Black;
		}
		tvWizard.SelectedNode = node;
		tvWizard.SelectedNode.NodeFont = new Font(tvWizard.Font, FontStyle.Bold);
		tvWizard.SelectedNode.ForeColor = Color.DarkBlue;
		tvWizard.SelectedNode.Text = tvWizard.SelectedNode.Text;
	}

        private void XtraWizardControl_Load(object sender, EventArgs e)
	{
		if (tvWizard.GetNodeCount(includeSubTrees: false) > 0)
		{
			SetNodesColor(tvWizard.Nodes[0]);
			Font font = new Font(tvWizard.Font, FontStyle.Regular);
			tvWizard.Font = font;
			UpdateTreeNodeColor(tvWizard.Nodes[0]);
		}
	}
    }
}
