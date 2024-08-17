using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.Database;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Database
{
    public class SqlServerBrowserDialog : XtraForm
    {
        private delegate void UpdateTreeAction(TreeView treeView, ArrayList sqlServer);

        private BackgroundWorker m_bwLocal;

        private BackgroundWorker m_bwNetwork;

        private ArrayList m_alSqlLocalServers;

        private ArrayList m_alSqlNetworkServers;

        private IContainer components;

        private TreeView w_treeviewLocal;

        private TreeView w_treeviewNetwork;

        private ImageList w_ilTreeNodeIcon;

        private SimpleButton w_buttonOK;

        private SimpleButton w_buttonCancel;

        private SplitContainerControl splitContainer1;

        private BarManager _menuBarManager;

        private Bar bar2;

        private BarButtonItem _refreshButton;

        private BarEditItem _sourceComboBoxMenuItem;

        private RepositoryItemComboBox _sourceComboBox;

        private Bar bar3;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private RepositoryItemTextEdit repositoryItemTextEdit1;

        public string SelectedServer
        {
            get
		{
			object editValue = _sourceComboBoxMenuItem.EditValue;
			if (editValue == null)
			{
				editValue = "";
			}
			string str = editValue.ToString();
			string str1 = str;
			if (str != null && !(str1 == "") && str1 == "Network")
			{
				if (w_treeviewNetwork.SelectedNode == null)
				{
					return "";
				}
				return w_treeviewNetwork.SelectedNode.Text;
			}
			if (w_treeviewLocal.SelectedNode == null)
			{
				return "";
			}
			return w_treeviewLocal.SelectedNode.Text;
		}
        }

        public bool ShowNetworkServersTab
        {
            get
		{
			object editValue = _sourceComboBoxMenuItem.EditValue;
			if (editValue == null)
			{
				editValue = "";
			}
			return editValue.ToString() == "Network";
		}
            set
		{
			if (value && _sourceComboBox.Items.Count == 1)
			{
				_sourceComboBox.Items.Add("Network");
			}
			else if (!value && _sourceComboBox.Items.Count == 2)
			{
				_sourceComboBox.Items.RemoveAt(1);
			}
		}
        }

        public ArrayList SqlLocalServers
        {
            get
		{
			return m_alSqlLocalServers;
		}
            set
		{
			if (value != null)
			{
				m_alSqlLocalServers = value;
				UpdateTree(w_treeviewLocal, m_alSqlLocalServers);
			}
		}
        }

        public ArrayList SqlNetworkServers
        {
            get
		{
			return m_alSqlNetworkServers;
		}
            set
		{
			if (value != null)
			{
				m_alSqlNetworkServers = value;
				UpdateTree(w_treeviewNetwork, m_alSqlNetworkServers);
			}
		}
        }

        public SqlServerBrowserDialog()
	{
		InitializeComponent();
	}

        private void _refreshButton_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			object editValue = _sourceComboBoxMenuItem.EditValue;
			if (editValue == null)
			{
				editValue = "";
			}
			string str = editValue.ToString();
			string str1 = str;
			if (str == null || str1 == "" || !(str1 == "Network"))
			{
				w_treeviewLocal.Enabled = false;
				w_treeviewLocal.Nodes.Clear();
				w_treeviewLocal.Nodes.Add("fetching", "fetching local SQL servers...");
				InitializeLocalBackgoundWorker();
			}
			else
			{
				w_treeviewNetwork.Enabled = false;
				w_treeviewNetwork.Nodes.Clear();
				w_treeviewNetwork.Nodes.Add("fetching", "fetching network SQL servers...");
				InitializeNetworkBackgoundWorker();
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			GlobalServices.ErrorHandler.HandleException("Fail to update tree: " + exception.Message, exception);
		}
	}

        private void _sourceComboBoxMenuItem_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			object editValue = _sourceComboBoxMenuItem.EditValue;
			if (editValue == null)
			{
				editValue = "";
			}
			string str = editValue.ToString();
			string str1 = str;
			if (str == null || str1 == "" || !(str1 == "Network"))
			{
				splitContainer1.PanelVisibility = SplitPanelVisibility.Panel1;
				if (m_bwLocal == null && w_treeviewLocal.Nodes.Count == 0)
				{
					w_treeviewLocal.Enabled = false;
					w_treeviewLocal.Nodes.Add("fetching", "fetching local SQL servers...");
					InitializeLocalBackgoundWorker();
				}
			}
			else
			{
				splitContainer1.PanelVisibility = SplitPanelVisibility.Panel2;
				if (m_bwNetwork == null && w_treeviewNetwork.Nodes.Count == 0)
				{
					w_treeviewNetwork.Enabled = false;
					w_treeviewNetwork.Nodes.Add("fetching", "fetching network SQL servers...");
					InitializeNetworkBackgoundWorker();
				}
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			GlobalServices.ErrorHandler.HandleException("Fail to update tree: " + exception.Message, exception);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Database.SqlServerBrowserDialog));
		this.w_treeviewLocal = new System.Windows.Forms.TreeView();
		this.w_ilTreeNodeIcon = new System.Windows.Forms.ImageList(this.components);
		this.w_treeviewNetwork = new System.Windows.Forms.TreeView();
		this.w_buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this.splitContainer1 = new DevExpress.XtraEditors.SplitContainerControl();
		this._menuBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.bar2 = new DevExpress.XtraBars.Bar();
		this._refreshButton = new DevExpress.XtraBars.BarButtonItem();
		this._sourceComboBoxMenuItem = new DevExpress.XtraBars.BarEditItem();
		this._sourceComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.bar3 = new DevExpress.XtraBars.Bar();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._sourceComboBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		base.SuspendLayout();
		this.w_treeviewLocal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(this.w_treeviewLocal, "w_treeviewLocal");
		this.w_treeviewLocal.ImageList = this.w_ilTreeNodeIcon;
		this.w_treeviewLocal.Name = "w_treeviewLocal";
		this.w_ilTreeNodeIcon.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("w_ilTreeNodeIcon.ImageStream");
		this.w_ilTreeNodeIcon.TransparentColor = System.Drawing.Color.Transparent;
		this.w_ilTreeNodeIcon.Images.SetKeyName(0, "sqlserver.ico");
		this.w_treeviewNetwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(this.w_treeviewNetwork, "w_treeviewNetwork");
		this.w_treeviewNetwork.ImageList = this.w_ilTreeNodeIcon;
		this.w_treeviewNetwork.Name = "w_treeviewNetwork";
		componentResourceManager.ApplyResources(this.w_buttonOK, "w_buttonOK");
		this.w_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.w_buttonOK.Name = "w_buttonOK";
		componentResourceManager.ApplyResources(this.w_buttonCancel, "w_buttonCancel");
		this.w_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_buttonCancel.Name = "w_buttonCancel";
		componentResourceManager.ApplyResources(this.splitContainer1, "splitContainer1");
		this.splitContainer1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.w_treeviewLocal);
		this.splitContainer1.Panel2.Controls.Add(this.w_treeviewNetwork);
		DevExpress.XtraBars.Bars bars = this._menuBarManager.Bars;
		DevExpress.XtraBars.Bar[] barArray = new DevExpress.XtraBars.Bar[2] { this.bar2, this.bar3 };
		bars.AddRange(barArray);
		this._menuBarManager.DockControls.Add(this.barDockControlTop);
		this._menuBarManager.DockControls.Add(this.barDockControlBottom);
		this._menuBarManager.DockControls.Add(this.barDockControlLeft);
		this._menuBarManager.DockControls.Add(this.barDockControlRight);
		this._menuBarManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._menuBarManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[2] { this._refreshButton, this._sourceComboBoxMenuItem };
		items.AddRange(barItemArray);
		this._menuBarManager.MainMenu = this.bar2;
		this._menuBarManager.MaxItemId = 3;
		DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._menuBarManager.RepositoryItems;
		DevExpress.XtraEditors.Repository.RepositoryItem[] repositoryItemArray = new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemTextEdit1, this._sourceComboBox };
		repositoryItems.AddRange(repositoryItemArray);
		this.bar2.BarItemHorzIndent = 0;
		this.bar2.BarItemVertIndent = 3;
		this.bar2.BarName = "Main menu";
		this.bar2.DockCol = 0;
		this.bar2.DockRow = 0;
		this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this.bar2.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._refreshButton),
			new DevExpress.XtraBars.LinkPersistInfo(this._sourceComboBoxMenuItem)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this.bar2.OptionsBar.DrawBorder = false;
		this.bar2.OptionsBar.DrawDragBorder = false;
		this.bar2.OptionsBar.UseWholeRow = true;
		componentResourceManager.ApplyResources(this.bar2, "bar2");
		componentResourceManager.ApplyResources(this._refreshButton, "_refreshButton");
		this._refreshButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.RefreshButton1;
		this._refreshButton.Id = 0;
		this._refreshButton.ImageIndex = 0;
		this._refreshButton.Name = "_refreshButton";
		this._refreshButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this._refreshButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_refreshButton_ItemClick);
		this._sourceComboBoxMenuItem.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
		componentResourceManager.ApplyResources(this._sourceComboBoxMenuItem, "_sourceComboBoxMenuItem");
		this._sourceComboBoxMenuItem.Edit = this._sourceComboBox;
		this._sourceComboBoxMenuItem.Id = 2;
		this._sourceComboBoxMenuItem.Name = "_sourceComboBoxMenuItem";
		this._sourceComboBoxMenuItem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
		this._sourceComboBoxMenuItem.EditValueChanged += new System.EventHandler(_sourceComboBoxMenuItem_EditValueChanged);
		componentResourceManager.ApplyResources(this._sourceComboBox, "_sourceComboBox");
		DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this._sourceComboBox.Buttons;
		DevExpress.XtraEditors.Controls.EditorButton[] editorButton = new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton((DevExpress.XtraEditors.Controls.ButtonPredefines)componentResourceManager.GetObject("_sourceComboBox.Buttons"))
		};
		buttons.AddRange(editorButton);
		DevExpress.XtraEditors.Controls.ComboBoxItemCollection comboBoxItemCollection = this._sourceComboBox.Items;
		object[] str = new object[2]
		{
			componentResourceManager.GetString("_sourceComboBox.Items"),
			componentResourceManager.GetString("_sourceComboBox.Items1")
		};
		comboBoxItemCollection.AddRange(str);
		this._sourceComboBox.Name = "_sourceComboBox";
		this._sourceComboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.bar3.BarName = "Status bar";
		this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
		this.bar3.DockCol = 0;
		this.bar3.DockRow = 0;
		this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
		this.bar3.OptionsBar.AllowQuickCustomization = false;
		this.bar3.OptionsBar.DrawDragBorder = false;
		this.bar3.OptionsBar.UseWholeRow = true;
		componentResourceManager.ApplyResources(this.bar3, "bar3");
		this.barDockControlTop.CausesValidation = false;
		componentResourceManager.ApplyResources(this.barDockControlTop, "barDockControlTop");
		this.barDockControlBottom.CausesValidation = false;
		componentResourceManager.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
		this.barDockControlLeft.CausesValidation = false;
		componentResourceManager.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
		this.barDockControlRight.CausesValidation = false;
		componentResourceManager.ApplyResources(this.barDockControlRight, "barDockControlRight");
		componentResourceManager.ApplyResources(this.repositoryItemTextEdit1, "repositoryItemTextEdit1");
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		base.AcceptButton = this.w_buttonOK;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_buttonCancel;
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.w_buttonCancel);
		base.Controls.Add(this.w_buttonOK);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SqlServerBrowserDialog";
		base.ShowInTaskbar = false;
		base.Shown += new System.EventHandler(SqlServerBrowserDialog_Shown);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this._sourceComboBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		base.ResumeLayout(false);
	}

        private void InitializeLocalBackgoundWorker()
	{
		if (m_bwLocal != null)
		{
			m_bwLocal.CancelAsync();
		}
		m_bwLocal = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		m_bwLocal.DoWork += LocalBackgroundWorker_DoWork;
		m_bwLocal.RunWorkerCompleted += LocalBackgroundWorker_RunWorkerCompleted;
		m_bwLocal.RunWorkerAsync();
	}

        private void InitializeNetworkBackgoundWorker()
	{
		if (m_bwNetwork != null)
		{
			m_bwNetwork.CancelAsync();
		}
		m_bwNetwork = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		m_bwNetwork.DoWork += NetworkBackgroundWorker_DoWork;
		m_bwNetwork.RunWorkerCompleted += NetworkBackgroundWorker_RunWorkerCompleted;
		m_bwNetwork.RunWorkerAsync();
	}

        private void LocalBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		e.Result = DatabaseBrowser.GetLocalSQLServers();
	}

        private void LocalBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			SqlLocalServers = (ArrayList)e.Result;
		}
		else
		{
			GlobalServices.ErrorHandler.HandleException(e.Error);
		}
		sender = null;
	}

        private void NetworkBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		e.Result = DatabaseBrowser.GetNetworkSQLServers();
	}

        private void NetworkBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			SqlNetworkServers = (ArrayList)e.Result;
		}
		else
		{
			GlobalServices.ErrorHandler.HandleException(e.Error);
		}
		sender = null;
	}

        private void SqlServerBrowserDialog_Shown(object sender, EventArgs e)
	{
		_sourceComboBoxMenuItem.EditValue = "Local";
	}

        private void UpdateTree(TreeView treeView, ArrayList sqlServer)
	{
		if (treeView.InvokeRequired)
		{
			UpdateTreeAction updateTreeAction = UpdateTree;
			object[] objArray = new object[2] { treeView, sqlServer };
			treeView.Invoke(updateTreeAction, objArray);
		}
		else if (treeView.Nodes.Count <= 0 || treeView.Nodes[0].Text.Contains("fetching"))
		{
			TreeNode[] treeNode = new TreeNode[sqlServer.Count];
			for (int i = 0; i < sqlServer.Count; i++)
			{
				treeNode[i] = new TreeNode(sqlServer[i].ToString());
			}
			treeView.Nodes.Clear();
			treeView.Nodes.AddRange(treeNode);
			treeView.Enabled = true;
		}
	}
    }
}
