using System;
using System.Collections;
using System.ComponentModel;
using System.DirectoryServices;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class AgentsBrowserDialog : XtraForm
    {
        private delegate void UpdateTreeAction(TreeView treeView, ArrayList sqlServer);

        private BackgroundWorker _machineWorker;

        private IContainer components;

        private ImageList imgListTreeNode;

        private SimpleButton btnOk;

        private SimpleButton btnCancel;

        private TreeView tvMachines;

        public string SelectedAgent
        {
            get
		{
			if (tvMachines.SelectedNode == null)
			{
				return string.Empty;
			}
			return tvMachines.SelectedNode.Text;
		}
        }

        public AgentsBrowserDialog()
	{
		InitializeComponent();
		ResetTreeView();
		InitializeMachineBackgoundWorker();
	}

        private void _machineWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		e.Result = LoadMachines();
	}

        private void _machineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			GlobalServices.ErrorHandler.HandleException(e.Error);
		}
		else
		{
			UpdateTree(tvMachines, (ArrayList)e.Result);
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

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.AgentsBrowserDialog));
		this.imgListTreeNode = new System.Windows.Forms.ImageList();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.tvMachines = new System.Windows.Forms.TreeView();
		base.SuspendLayout();
		this.imgListTreeNode.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imgListTreeNode.ImageStream");
		this.imgListTreeNode.TransparentColor = System.Drawing.Color.Transparent;
		this.imgListTreeNode.Images.SetKeyName(0, "sqlserver.ico");
		componentResourceManager.ApplyResources(this.btnOk, "btnOk");
		this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnOk.Name = "btnOk";
		componentResourceManager.ApplyResources(this.btnCancel, "btnCancel");
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Name = "btnCancel";
		componentResourceManager.ApplyResources(this.tvMachines, "tvMachines");
		this.tvMachines.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.tvMachines.ImageList = this.imgListTreeNode;
		this.tvMachines.Name = "tvMachines";
		base.AcceptButton = this.btnOk;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.Controls.Add(this.tvMachines);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AgentsBrowserDialog";
		base.ShowInTaskbar = false;
		base.ResumeLayout(false);
	}

        private void InitializeMachineBackgoundWorker()
	{
		if (_machineWorker != null)
		{
			_machineWorker.CancelAsync();
		}
		_machineWorker = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		_machineWorker.DoWork += _machineWorker_DoWork;
		_machineWorker.RunWorkerCompleted += _machineWorker_RunWorkerCompleted;
		_machineWorker.RunWorkerAsync();
	}

        private ArrayList LoadMachines()
	{
		ArrayList arrayLists = new ArrayList();
		foreach (DirectoryEntry child in new DirectoryEntry("WinNT:").Children)
		{
			foreach (DirectoryEntry directoryEntry in child.Children)
			{
				if (directoryEntry.SchemaClassName.Equals("Computer", StringComparison.InvariantCultureIgnoreCase))
				{
					arrayLists.Add(directoryEntry.Name);
				}
			}
		}
		return arrayLists;
	}

        private void ResetTreeView()
	{
		tvMachines.Enabled = false;
		tvMachines.Nodes.Clear();
		tvMachines.Nodes.Add("fetching", "fetching network machines...");
	}

        private void UpdateTree(TreeView treeView, ArrayList machines)
	{
		if (treeView.InvokeRequired)
		{
			UpdateTreeAction updateTreeAction = UpdateTree;
			object[] objArray = new object[2] { treeView, machines };
			treeView.Invoke(updateTreeAction, objArray);
		}
		else if (treeView.Nodes.Count <= 0 || treeView.Nodes[0].Text.Contains("fetching"))
		{
			TreeNode[] treeNode = new TreeNode[machines.Count];
			for (int i = 0; i < machines.Count; i++)
			{
				treeNode[i] = new TreeNode(machines[i].ToString());
			}
			treeView.Nodes.Clear();
			treeView.Nodes.AddRange(treeNode);
			treeView.Enabled = true;
		}
	}
    }
}
