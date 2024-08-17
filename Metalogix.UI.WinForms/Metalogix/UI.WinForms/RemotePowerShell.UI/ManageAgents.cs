using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Actions.Remoting.Database;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class ManageAgents : XtraForm, IHasSelectableObjects
    {
        private delegate void VoidDelegate();

        private AgentCollection _agents;

        private readonly ReaderWriterLockSlim _uiUpdateLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private IContainer components;

        private XtraBarManagerWithArrows _menuBarManager;

        private Bar _menuBar;

        private BarButtonItem btnAddAgent;

        private BarButtonItem btnViewAgent;

        private BarButtonItem btnEditAgent;

        private BarButtonItem btnUpdateAgent;

        private BarButtonItem btnRemoveAgent;

        private StandaloneBarDockControl _menuBarDockLocation;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private XtraParentSelectableGrid gridAgentParentSelectable;

        private GridView gridAgents;

        private GridColumn colAgentName;

        private GridColumn colCMVersion;

        private GridColumn colStatus;

        private GridColumn colLogMessage;

        private GridColumn colRunAs;

        private GridColumn colAgentId;

        private GridColumn colMachineIP;

        private GridColumn colOSVersion;

        private GridColumn colPassword;

        private ActionPaletteControl actionPallet;

        private SimpleButton btnOk;

        private BarButtonItem btnRefreshAgent;

        public AgentCollection Agents
        {
            get
		{
			if (_agents == null)
			{
				IAgentDb agentDb = new AgentDb(JobsSettings.AdapterContext.ToInsecureString());
				AgentCollection agentCollection = new AgentCollection(agentDb);
				agentCollection.FetchData();
				_agents = agentCollection;
			}
			return _agents;
		}
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Agent[] SelectedAgents => GetCurrentlySelectedItems().ToArray();

        public IXMLAbleList SelectedObjects => new CommonSerializableList<Agent>(SelectedAgents);

        public ManageAgents()
	{
		InitializeComponent();
		LoadGrid();
		_agents.AgentListChanged += _agents_AgentListChanged;
	}

        private void _agents_AgentListChanged(object sender, EventArgs e)
	{
		ReLoadUI();
		UpdateUI();
	}

        private void btnAddAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new AddAgentAction());
	}

        private void btnEditAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new EditAgentAction());
	}

        private void btnOk_Click(object sender, EventArgs e)
	{
		Close();
	}

        private void btnRefreshAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new RefreshAgentAction());
	}

        private void btnRemoveAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new RemoveAgentAction());
	}

        private void btnUpdateAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new UpdateAgentAction());
	}

        private void btnViewAgent_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnActionClick(new ViewAgentAction());
	}

        private void DetachGridSource()
	{
		gridAgentParentSelectable.DataSource = null;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private List<Agent> GetCurrentlySelectedItems()
	{
		int[] selectedRows = gridAgents.GetSelectedRows();
		if (selectedRows == null)
		{
			return new List<Agent>();
		}
		List<Agent> agents = new List<Agent>(selectedRows.Length);
		int[] numArray = selectedRows;
		foreach (int num in numArray)
		{
			if (gridAgents.GetRow(num) is DataRowView row)
			{
				Agent remoteContextFromId = _agents.GetRemoteContextFromId(new Guid(Convert.ToString(row[Constants.AgentID])));
				if (remoteContextFromId != null)
				{
					agents.Add(remoteContextFromId);
				}
			}
		}
		return agents;
	}

        private DataTable GetDataTable(List<Agent> agents)
	{
		if (agents == null)
		{
			return null;
		}
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add(Constants.MachineName);
		dataTable.Columns.Add(Constants.CMVersion);
		dataTable.Columns.Add(Constants.UserName);
		dataTable.Columns.Add(Constants.Status);
		dataTable.Columns.Add(Constants.Details);
		dataTable.Columns.Add(Constants.AgentID);
		dataTable.Columns.Add(Constants.MachineIP);
		dataTable.Columns.Add(Constants.OSVersion);
		dataTable.Columns.Add(Constants.Password);
		foreach (Agent agent in agents)
		{
			DataRowCollection rows = dataTable.Rows;
			object[] machineName = new object[9] { agent.MachineName, agent.CMVersion, agent.UserName, agent.Status, null, null, null, null, null };
			machineName[4] = agent.Details[0].Value;
			machineName[5] = agent.AgentID;
			machineName[6] = agent.MachineIP;
			machineName[7] = agent.OSVersion;
			machineName[8] = agent.Password;
			rows.Add(machineName);
		}
		return dataTable;
	}

        private void gridAgents_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		btnUpdateAgent.Enabled = HasAgentStatus(AgentStatus.Available);
		btnRefreshAgent.Enabled = !HasAgentStatus(AgentStatus.Configuring);
		btnEditAgent.Enabled = HasAgentStatus(AgentStatus.Available) || HasAgentStatus(AgentStatus.Error);
	}

        private void gridAgents_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		actionPallet.HostingControl = this;
		actionPallet.BuildActionMenu();
	}

        private bool HasAgentStatus(AgentStatus status)
	{
		string str = Convert.ToString(gridAgents.GetFocusedRowCellValue(Constants.Status));
		if (!string.IsNullOrEmpty(str) && str == status.ToString())
		{
			return true;
		}
		return false;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		Metalogix.DataStructures.Generic.CommonSerializableList<object> commonSerializableList = new Metalogix.DataStructures.Generic.CommonSerializableList<object>();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.ManageAgents));
		this.actionPallet = new Metalogix.UI.WinForms.Actions.ActionPaletteControl();
		this._menuBarManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
		this._menuBar = new DevExpress.XtraBars.Bar();
		this.btnAddAgent = new DevExpress.XtraBars.BarButtonItem();
		this.btnViewAgent = new DevExpress.XtraBars.BarButtonItem();
		this.btnEditAgent = new DevExpress.XtraBars.BarButtonItem();
		this.btnUpdateAgent = new DevExpress.XtraBars.BarButtonItem();
		this.btnRefreshAgent = new DevExpress.XtraBars.BarButtonItem();
		this.btnRemoveAgent = new DevExpress.XtraBars.BarButtonItem();
		this._menuBarDockLocation = new DevExpress.XtraBars.StandaloneBarDockControl();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.gridAgentParentSelectable = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
		this.gridAgents = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.colAgentName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colCMVersion = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colRunAs = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colLogMessage = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colAgentId = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colMachineIP = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colOSVersion = new DevExpress.XtraGrid.Columns.GridColumn();
		this.colPassword = new DevExpress.XtraGrid.Columns.GridColumn();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridAgentParentSelectable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridAgents).BeginInit();
		base.SuspendLayout();
		this.actionPallet.HostingControl = null;
		this.actionPallet.LegalType = null;
		this.actionPallet.Name = "_actionPallet";
		this.actionPallet.Size = new System.Drawing.Size(61, 4);
		this.actionPallet.SourceOverride = commonSerializableList;
		this.actionPallet.UseSourceOverride = false;
		this._menuBarManager.AllowCustomization = false;
		this._menuBarManager.AllowMoveBarOnToolbar = false;
		this._menuBarManager.AllowQuickCustomization = false;
		this._menuBarManager.AllowShowToolbarsPopup = false;
		this._menuBarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this._menuBar });
		this._menuBarManager.DockControls.Add(this.barDockControlTop);
		this._menuBarManager.DockControls.Add(this.barDockControlBottom);
		this._menuBarManager.DockControls.Add(this.barDockControlLeft);
		this._menuBarManager.DockControls.Add(this.barDockControlRight);
		this._menuBarManager.DockControls.Add(this._menuBarDockLocation);
		this._menuBarManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._menuBarManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[6] { this.btnAddAgent, this.btnViewAgent, this.btnEditAgent, this.btnUpdateAgent, this.btnRemoveAgent, this.btnRefreshAgent };
		items.AddRange(barItemArray);
		this._menuBarManager.MainMenu = this._menuBar;
		this._menuBarManager.MaxItemId = 58;
		this._menuBar.BarItemHorzIndent = 3;
		this._menuBar.BarName = "Main menu";
		this._menuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Standalone;
		this._menuBar.DockCol = 0;
		this._menuBar.DockRow = 0;
		this._menuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
		this._menuBar.FloatLocation = new System.Drawing.Point(106, 183);
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this._menuBar.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAddAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnViewAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnEditAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnUpdateAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRefreshAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRemoveAgent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this._menuBar.OptionsBar.AllowQuickCustomization = false;
		this._menuBar.OptionsBar.DrawBorder = false;
		this._menuBar.OptionsBar.DrawDragBorder = false;
		this._menuBar.OptionsBar.MultiLine = true;
		this._menuBar.OptionsBar.UseWholeRow = true;
		this._menuBar.StandaloneBarDockControl = this._menuBarDockLocation;
		this._menuBar.Text = "Main menu";
		this.btnAddAgent.Caption = "Add Agent";
		this.btnAddAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.AddAgent16;
		this.btnAddAgent.Id = 8;
		this.btnAddAgent.Name = "btnAddAgent";
		this.btnAddAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnAddAgent_ItemClick);
		this.btnViewAgent.Caption = "View Agent";
		this.btnViewAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.ViewAgent16;
		this.btnViewAgent.Id = 8;
		this.btnViewAgent.Name = "btnViewAgent";
		this.btnViewAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnViewAgent_ItemClick);
		this.btnEditAgent.Caption = "Edit Agent";
		this.btnEditAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.EditAgent16;
		this.btnEditAgent.Id = 8;
		this.btnEditAgent.Name = "btnEditAgent";
		this.btnEditAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnEditAgent_ItemClick);
		this.btnUpdateAgent.Caption = "Update Agent";
		this.btnUpdateAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.UpdateAgent16;
		this.btnUpdateAgent.Id = 9;
		this.btnUpdateAgent.Name = "btnUpdateAgent";
		this.btnUpdateAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnUpdateAgent_ItemClick);
		this.btnRefreshAgent.Caption = "Refresh Agent";
		this.btnRefreshAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.RefreshAgent16;
		this.btnRefreshAgent.Id = 57;
		this.btnRefreshAgent.Name = "btnRefreshAgent";
		this.btnRefreshAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnRefreshAgent_ItemClick);
		this.btnRemoveAgent.Caption = "Remove Agent";
		this.btnRemoveAgent.Glyph = Metalogix.UI.WinForms.Properties.Resources.DeleteAgent16;
		this.btnRemoveAgent.Id = 26;
		this.btnRemoveAgent.Name = "btnRemoveAgent";
		this.btnRemoveAgent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnRemoveAgent_ItemClick);
		this._menuBarDockLocation.CausesValidation = false;
		this._menuBarDockLocation.Dock = System.Windows.Forms.DockStyle.Top;
		this._menuBarDockLocation.Location = new System.Drawing.Point(0, 0);
		this._menuBarDockLocation.Name = "_menuBarDockLocation";
		this._menuBarDockLocation.Size = new System.Drawing.Size(937, 25);
		this._menuBarDockLocation.Text = "standaloneBarDockControl1";
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Size = new System.Drawing.Size(937, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 661);
		this.barDockControlBottom.Size = new System.Drawing.Size(937, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 661);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(937, 0);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 661);
		this.gridAgentParentSelectable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridAgentParentSelectable.ContextMenuStrip = this.actionPallet;
		this.gridAgentParentSelectable.Location = new System.Drawing.Point(0, 25);
		this.gridAgentParentSelectable.MainView = this.gridAgents;
		this.gridAgentParentSelectable.MenuManager = this._menuBarManager;
		this.gridAgentParentSelectable.Name = "gridAgentParentSelectable";
		this.gridAgentParentSelectable.Size = new System.Drawing.Size(937, 600);
		this.gridAgentParentSelectable.TabIndex = 8;
		this.gridAgentParentSelectable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridAgents });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gridAgents.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray = new DevExpress.XtraGrid.Columns.GridColumn[9] { this.colAgentName, this.colCMVersion, this.colRunAs, this.colStatus, this.colLogMessage, this.colAgentId, this.colMachineIP, this.colOSVersion, this.colPassword };
		columns.AddRange(gridColumnArray);
		this.gridAgents.GridControl = this.gridAgentParentSelectable;
		this.gridAgents.Name = "gridAgents";
		this.gridAgents.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridAgents.OptionsView.ShowGroupPanel = false;
		this.gridAgents.OptionsMenu.EnableColumnMenu = false;
		this.gridAgents.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
		this.gridAgents.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(gridAgents_FocusedRowChanged);
		this.colAgentName.Caption = "Agent Name";
		this.colAgentName.FieldName = "MachineName";
		this.colAgentName.Name = "colAgentName";
		this.colAgentName.OptionsColumn.AllowEdit = false;
		this.colAgentName.OptionsColumn.AllowMove = false;
		this.colAgentName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.colAgentName.OptionsFilter.AllowFilter = false;
		this.colAgentName.Visible = true;
		this.colAgentName.VisibleIndex = 0;
		this.colAgentName.Width = 195;
		this.colCMVersion.Caption = "CM Version";
		this.colCMVersion.FieldName = "CMVersion";
		this.colCMVersion.Name = "colCMVersion";
		this.colCMVersion.OptionsColumn.AllowEdit = false;
		this.colCMVersion.OptionsColumn.AllowMove = false;
		this.colCMVersion.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colCMVersion.OptionsFilter.AllowFilter = false;
		this.colCMVersion.Visible = true;
		this.colCMVersion.VisibleIndex = 2;
		this.colRunAs.Caption = "Run As";
		this.colRunAs.FieldName = "UserName";
		this.colRunAs.Name = "colRunAs";
		this.colRunAs.OptionsColumn.AllowEdit = false;
		this.colRunAs.OptionsColumn.AllowMove = false;
		this.colRunAs.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colRunAs.OptionsFilter.AllowFilter = false;
		this.colRunAs.Visible = true;
		this.colRunAs.VisibleIndex = 3;
		this.colRunAs.Width = 175;
		this.colStatus.Caption = "Status";
		this.colStatus.FieldName = "Status";
		this.colStatus.Name = "colStatus";
		this.colStatus.OptionsColumn.AllowEdit = false;
		this.colStatus.OptionsColumn.AllowMove = false;
		this.colStatus.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colStatus.OptionsFilter.AllowFilter = false;
		this.colStatus.Visible = true;
		this.colStatus.VisibleIndex = 4;
		this.colStatus.Width = 100;
		this.colLogMessage.Caption = "Log Message";
		this.colLogMessage.FieldName = "Details";
		this.colLogMessage.Name = "colLogMessage";
		this.colLogMessage.OptionsColumn.AllowEdit = false;
		this.colLogMessage.OptionsColumn.AllowMove = false;
		this.colLogMessage.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colLogMessage.OptionsFilter.AllowFilter = false;
		this.colLogMessage.Visible = true;
		this.colLogMessage.VisibleIndex = 5;
		this.colLogMessage.Width = 273;
		this.colAgentId.Caption = "Agent ID";
		this.colAgentId.FieldName = "AgentID";
		this.colAgentId.Name = "colAgentId";
		this.colAgentId.OptionsColumn.AllowEdit = false;
		this.colAgentId.OptionsColumn.AllowMove = false;
		this.colAgentId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colAgentId.OptionsFilter.AllowFilter = false;
		this.colMachineIP.Caption = "Machine IP";
		this.colMachineIP.FieldName = "MachineIP";
		this.colMachineIP.Name = "colMachineIP";
		this.colMachineIP.OptionsColumn.AllowEdit = false;
		this.colMachineIP.OptionsColumn.AllowMove = false;
		this.colMachineIP.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colMachineIP.OptionsFilter.AllowFilter = false;
		this.colOSVersion.Caption = "OS Version";
		this.colOSVersion.FieldName = "OSVersion";
		this.colOSVersion.Name = "colOSVersion";
		this.colOSVersion.OptionsColumn.AllowEdit = false;
		this.colOSVersion.OptionsColumn.AllowMove = false;
		this.colOSVersion.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colOSVersion.OptionsFilter.AllowFilter = false;
		this.colOSVersion.Visible = true;
		this.colOSVersion.VisibleIndex = 1;
		this.colPassword.Caption = "Password";
		this.colPassword.FieldName = "Password";
		this.colPassword.Name = "colPassword";
		this.colPassword.OptionsColumn.AllowEdit = false;
		this.colPassword.OptionsColumn.AllowMove = false;
		this.colPassword.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colPassword.OptionsFilter.AllowFilter = false;
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOk.Location = new System.Drawing.Point(850, 631);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 14;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(btnOk_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(937, 661);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.gridAgentParentSelectable);
		base.Controls.Add(this._menuBarDockLocation);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(500, 250);
		base.Name = "ManageAgents";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Manage Agents";
		((System.ComponentModel.ISupportInitialize)this._menuBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridAgentParentSelectable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridAgents).EndInit();
		base.ResumeLayout(false);
	}

        private void LoadGrid()
	{
		try
		{
			gridAgents.ClearSelection();
			gridAgentParentSelectable.DataSource = null;
			List<Agent> list = Agents.GetList();
			if (list != null)
			{
				DataTable dataTable = GetDataTable(list);
				if (dataTable != null)
				{
					gridAgentParentSelectable.DataSource = dataTable;
				}
			}
			UpdateUI();
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			string str = "An error occurred while retrieving agents.";
			GlobalServices.ErrorHandler.HandleException("Manage Agent", str, exception, ErrorIcon.Error);
			Logging.LogExceptionToTextFileWithEventLogBackup(exception, str);
		}
	}

        private void OnActionClick(Metalogix.Actions.Action action)
	{
		CommonSerializableList<AgentCollection> commonSerializableList = new CommonSerializableList<AgentCollection> { Agents };
		ActionPaletteControl.ActionClick(action, commonSerializableList, SelectedObjects);
	}

        private void ReattachGridSource()
	{
		if (gridAgentParentSelectable.DataSource != null)
		{
			return;
		}
		List<Agent> list = Agents.GetList();
		if (list != null)
		{
			DataTable dataTable = GetDataTable(list);
			if (dataTable != null)
			{
				gridAgentParentSelectable.DataSource = dataTable;
			}
		}
	}

        private void ReLoadUI()
	{
		CurrencyDataController.DisableThreadingProblemsDetection = true;
		if (base.InvokeRequired)
		{
			Invoke(new VoidDelegate(ReLoadUI));
			return;
		}
		_uiUpdateLock.EnterWriteLock();
		try
		{
			SuspendLayout();
			gridAgentParentSelectable.BeginUpdate();
			try
			{
				DetachGridSource();
				ReattachGridSource();
				gridAgents.ClearSelection();
				ScrollToBottom();
			}
			finally
			{
				ReattachGridSource();
				gridAgentParentSelectable.EndUpdate();
				ResumeLayout();
			}
		}
		finally
		{
			_uiUpdateLock.ExitWriteLock();
		}
	}

        private void ScrollToBottom()
	{
		if (gridAgents.DataRowCount > 0)
		{
			gridAgents.TopRowIndex = gridAgents.DataRowCount - 1;
		}
	}

        private void UpdateUI()
	{
		bool count = Agents.GetList().Count > 0;
		BarButtonItem barButtonItem = btnEditAgent;
		bool flag = count && (HasAgentStatus(AgentStatus.Available) || HasAgentStatus(AgentStatus.Error));
		barButtonItem.Enabled = flag;
		btnUpdateAgent.Enabled = count && HasAgentStatus(AgentStatus.Available);
		btnViewAgent.Enabled = count;
		btnRemoveAgent.Enabled = count;
		btnRefreshAgent.Enabled = count && !HasAgentStatus(AgentStatus.Configuring);
		XtraParentSelectableGrid xtraParentSelectableGrid = gridAgentParentSelectable;
		ContextMenuStrip contextMenuStrip = ((!count) ? null : actionPallet);
		xtraParentSelectableGrid.ContextMenuStrip = contextMenuStrip;
	}
    }
}
