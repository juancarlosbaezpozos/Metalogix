using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Metalogix.Database;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Database
{
    public class SqlDatabaseBrowserDialog : XtraForm
    {
        private BackgroundWorker m_bwUpdateUI;

        private IContainer components;

        private ImageList w_ilTreeNodeIcon;

        private TreeView w_treeviewDatabases;

        private SimpleButton w_buttonOK;

        private SimpleButton w_buttonCancel;

        private BarManager _menuBarManager;

        private Bar bar2;

        private BarButtonItem _newDatabaseButton;

        private BarButtonItem _deleteDatabaseButton;

        private Bar bar3;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarStaticItem _spacerLabel;

        public bool AllowCreation
        {
            get
		{
			return _newDatabaseButton.Enabled;
		}
            set
		{
			_newDatabaseButton.Enabled = value;
		}
        }

        public bool AllowDeletion
        {
            get
		{
			return _deleteDatabaseButton.Enabled;
		}
            set
		{
			_deleteDatabaseButton.Enabled = value;
		}
        }

        public string ConnectionString { get; set; }

        public string SelectedDatabase
        {
            get
		{
			if (w_treeviewDatabases.SelectedNode == null)
			{
				return null;
			}
			return ((SQLDatabase)w_treeviewDatabases.SelectedNode.Tag).Name;
		}
        }

        public SqlDatabaseBrowserDialog()
	{
		InitializeComponent();
	}

        private void _deleteDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(SelectedDatabase))
			{
				throw new Exception("No database has been selected.");
			}
			if (FlatXtraMessageBox.Show(string.Format("You are about to delete database '{0}' permanently." + Environment.NewLine + Environment.NewLine + "Do you wish to continue?", SelectedDatabase), "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				DatabaseBrowser.DeleteSQLDatabase(ConnectionString, SelectedDatabase);
				UpdateUI();
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

        private void _newDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			NewDatabaseDialog newDatabaseDialog = new NewDatabaseDialog();
			if (newDatabaseDialog.ShowDialog() == DialogResult.OK)
			{
				DatabaseBrowser.CreateSQLDatabase(ConnectionString, newDatabaseDialog.DatabaseName);
				UpdateUI();
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Database.SqlDatabaseBrowserDialog));
		this.w_ilTreeNodeIcon = new System.Windows.Forms.ImageList(this.components);
		this.w_treeviewDatabases = new System.Windows.Forms.TreeView();
		this.w_buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this._menuBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.bar2 = new DevExpress.XtraBars.Bar();
		this._newDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
		this._deleteDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
		this.bar3 = new DevExpress.XtraBars.Bar();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this._spacerLabel = new DevExpress.XtraBars.BarStaticItem();
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).BeginInit();
		base.SuspendLayout();
		this.w_ilTreeNodeIcon.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("w_ilTreeNodeIcon.ImageStream");
		this.w_ilTreeNodeIcon.TransparentColor = System.Drawing.Color.White;
		this.w_ilTreeNodeIcon.Images.SetKeyName(0, "DB.ico");
		componentResourceManager.ApplyResources(this.w_treeviewDatabases, "w_treeviewDatabases");
		this.w_treeviewDatabases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.w_treeviewDatabases.ImageList = this.w_ilTreeNodeIcon;
		this.w_treeviewDatabases.Name = "w_treeviewDatabases";
		componentResourceManager.ApplyResources(this.w_buttonOK, "w_buttonOK");
		this.w_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.w_buttonOK.Name = "w_buttonOK";
		componentResourceManager.ApplyResources(this.w_buttonCancel, "w_buttonCancel");
		this.w_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_buttonCancel.Name = "w_buttonCancel";
		DevExpress.XtraBars.Bars bars = this._menuBarManager.Bars;
		DevExpress.XtraBars.Bar[] barArray = new DevExpress.XtraBars.Bar[2] { this.bar2, this.bar3 };
		bars.AddRange(barArray);
		this._menuBarManager.DockControls.Add(this.barDockControlTop);
		this._menuBarManager.DockControls.Add(this.barDockControlBottom);
		this._menuBarManager.DockControls.Add(this.barDockControlLeft);
		this._menuBarManager.DockControls.Add(this.barDockControlRight);
		this._menuBarManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._menuBarManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[3] { this._newDatabaseButton, this._deleteDatabaseButton, this._spacerLabel };
		items.AddRange(barItemArray);
		this._menuBarManager.MainMenu = this.bar2;
		this._menuBarManager.MaxItemId = 3;
		this.bar2.BarItemHorzIndent = 0;
		this.bar2.BarItemVertIndent = 3;
		this.bar2.BarName = "Main menu";
		this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
		this.bar2.DockCol = 0;
		this.bar2.DockRow = 0;
		this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this.bar2.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._newDatabaseButton),
			new DevExpress.XtraBars.LinkPersistInfo(this._spacerLabel),
			new DevExpress.XtraBars.LinkPersistInfo(this._deleteDatabaseButton)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this.bar2.OptionsBar.AllowQuickCustomization = false;
		this.bar2.OptionsBar.DisableClose = true;
		this.bar2.OptionsBar.DisableCustomization = true;
		this.bar2.OptionsBar.DrawBorder = false;
		this.bar2.OptionsBar.DrawDragBorder = false;
		this.bar2.OptionsBar.UseWholeRow = true;
		componentResourceManager.ApplyResources(this.bar2, "bar2");
		componentResourceManager.ApplyResources(this._newDatabaseButton, "_newDatabaseButton");
		this._newDatabaseButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.AddButton;
		this._newDatabaseButton.Id = 0;
		this._newDatabaseButton.Name = "_newDatabaseButton";
		this._newDatabaseButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this._newDatabaseButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_newDatabaseButton_ItemClick);
		componentResourceManager.ApplyResources(this._deleteDatabaseButton, "_deleteDatabaseButton");
		this._deleteDatabaseButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.RemoveButton;
		this._deleteDatabaseButton.Id = 1;
		this._deleteDatabaseButton.Name = "_deleteDatabaseButton";
		this._deleteDatabaseButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this._deleteDatabaseButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_deleteDatabaseButton_ItemClick);
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
		componentResourceManager.ApplyResources(this._spacerLabel, "_spacerLabel");
		this._spacerLabel.Id = 2;
		this._spacerLabel.Name = "_spacerLabel";
		this._spacerLabel.TextAlignment = System.Drawing.StringAlignment.Near;
		base.AcceptButton = this.w_buttonOK;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_buttonCancel;
		base.Controls.Add(this.w_buttonOK);
		base.Controls.Add(this.w_buttonCancel);
		base.Controls.Add(this.w_treeviewDatabases);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SqlDatabaseBrowserDialog";
		base.ShowInTaskbar = false;
		base.Shown += new System.EventHandler(SqlDatabaseBrowserDialog_Shown);
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).EndInit();
		base.ResumeLayout(false);
	}

        private void SqlDatabaseBrowserDialog_Shown(object sender, EventArgs e)
	{
		try
		{
			UpdateUI();
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void UpdateUI()
	{
		if (!string.IsNullOrEmpty(ConnectionString))
		{
			w_treeviewDatabases.Enabled = false;
			w_treeviewDatabases.Nodes.Clear();
			w_treeviewDatabases.Nodes.Add("fetching", "fetching SQL databases...");
			if (m_bwUpdateUI != null)
			{
				m_bwUpdateUI.CancelAsync();
			}
			m_bwUpdateUI = new BackgroundWorker
			{
				WorkerSupportsCancellation = true
			};
			m_bwUpdateUI.DoWork += UpdateUI_DoWork;
			m_bwUpdateUI.RunWorkerCompleted += UpdateUI_RunWorkerCompleted;
			m_bwUpdateUI.RunWorkerAsync();
		}
	}

        private void UpdateUI_DoWork(object sender, DoWorkEventArgs e)
	{
		e.Result = DatabaseBrowser.GetSQLDatabases(ConnectionString);
	}

        private void UpdateUI_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			SQLDatabaseCollection sQLDatabases = DatabaseBrowser.GetSQLDatabases(ConnectionString);
			w_treeviewDatabases.TreeViewNodeSorter = null;
			w_treeviewDatabases.Nodes.Clear();
			if (sQLDatabases == null)
			{
				return;
			}
			TreeNode[] treeNode = new TreeNode[sQLDatabases.Count];
			for (int i = 0; i < sQLDatabases.Count; i++)
			{
				treeNode[i] = new TreeNode();
				treeNode[i].Text = sQLDatabases[i].GetDisplayName();
				treeNode[i].Tag = sQLDatabases[i];
			}
			w_treeviewDatabases.TreeViewNodeSorter = new SqlDatabaseBrowserDialogSorter();
			w_treeviewDatabases.Nodes.AddRange(treeNode);
			w_treeviewDatabases.Enabled = true;
		}
		else
		{
			GlobalServices.ErrorHandler.HandleException(e.Error);
		}
		sender = null;
	}

        private void w_buttonOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (w_treeviewDatabases.SelectedNode == null)
			{
				throw new Exception("No database has been selected.");
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			base.DialogResult = DialogResult.None;
		}
	}
    }
}
