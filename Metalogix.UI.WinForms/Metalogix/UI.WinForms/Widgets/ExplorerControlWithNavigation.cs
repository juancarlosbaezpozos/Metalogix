using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Explorer.Attributes;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Properties;
using Metalogix.Utilities;
using Metalogix.Widgets;

namespace Metalogix.UI.WinForms.Widgets
{
    public class ExplorerControlWithNavigation : UserControl, IHasSelectableObjects
    {
        private NodeCollectionExtended _dataSource;

        private IContainer components;

        private ExplorerControl w_explorerControl;

        private XtraBarManagerWithArrows _barManager;

        private Bar _filterBar;

        private BarStaticItem _filterLabel;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarEditItem _filterInputBox;

        private RepositoryItemTextEdit _filterTextEditControl;

        private BarButtonItem _filterButton;

        private BarButtonItem _connectionButton;

        private PopupMenu _connectionsMenu;

        private BarButtonItem _statusButton;

        private PopupMenu _statusMenu;

        private BarStaticItem _sortByLabel;

        private BarEditItem _sortingComboBox;

        private RepositoryItemComboBox _sortByComboEdit;

        private BarCheckItem barCheckItem1;

        private PanelControl _separatorPanel;

        private PanelControl _explorerWhiteSpacePanel;

        private PanelControl _toolBarWhiteSpacePanel;

        public Metalogix.Actions.Action[] Actions
        {
            get
		{
			return w_explorerControl.Actions;
		}
            set
		{
			w_explorerControl.Actions = value;
		}
        }

        public bool CheckBoxes
        {
            get
		{
			return w_explorerControl.CheckBoxes;
		}
            set
		{
			w_explorerControl.CheckBoxes = value;
		}
        }

        public NodeCollection DataSource
        {
            get
		{
			return w_explorerControl.DataSource;
		}
            set
		{
			_dataSource = null;
			w_explorerControl.DataSource = null;
			if (value != null)
			{
				_dataSource = new NodeCollectionExtended(value);
				ResetFilterSort();
				ApplyFilterSort();
				w_explorerControl.DataSource = _dataSource.DataSource;
			}
		}
        }

        public bool MultiSelectEnabled
        {
            get
		{
			return w_explorerControl.MultiSelectEnabled;
		}
            set
		{
			w_explorerControl.MultiSelectEnabled = value;
		}
        }

        public EnhancedTreeView.AllowSelectionDelegate MultiSelectLimitationMethod
        {
            get
		{
			return w_explorerControl.MultiSelectLimitationMethod;
		}
            set
		{
			w_explorerControl.MultiSelectLimitationMethod = value;
		}
        }

        public List<Type> NodeTypeFilter => w_explorerControl.NodeTypeFilter;

        public ExplorerTreeNode SelectedNode => w_explorerControl.SelectedNode;

        public ReadOnlyCollection<ExplorerTreeNode> SelectedNodes => w_explorerControl.SelectedNodes;

        public IXMLAbleList SelectedObjects => w_explorerControl.SelectedObjects;

        public event ExplorerControl.SelectedNodeChangedHandler SelectedNodeChanged;

        public ExplorerControlWithNavigation()
	{
		InitializeComponent();
		Initialize();
	}

        private void _filterButton_ItemClick(object sender, ItemClickEventArgs e)
	{
		ApplyFilterSort();
	}

        private void ApplyFilterSort()
	{
		if (_dataSource == null)
		{
			return;
		}
		try
		{
			w_explorerControl.SuspendLayout();
			string editValue = _filterInputBox.EditValue as string;
			List<Type> types = new List<Type>();
			foreach (BarItemLink itemLink in _connectionsMenu.ItemLinks)
			{
				BarCheckItem item = (BarCheckItem)itemLink.Item;
				if (item.Checked)
				{
					types.AddRange(item.Tag as IEnumerable<Type>);
				}
			}
			List<ConnectionStatus> connectionStatuses = new List<ConnectionStatus>();
			foreach (BarItemLink barItemLink in _statusMenu.ItemLinks)
			{
				BarCheckItem barCheckItem = (BarCheckItem)barItemLink.Item;
				if (barCheckItem.Checked)
				{
					ConnectionStatus tag = (ConnectionStatus)barCheckItem.Tag;
					if (!connectionStatuses.Contains(tag))
					{
						connectionStatuses.Add(tag);
					}
				}
			}
			NodeCollectionSortBy nodeCollectionSortBy = (NodeCollectionSortBy)Enum.Parse(typeof(NodeCollectionSortBy), _sortingComboBox.EditValue as string);
			_dataSource.ApplyFilterSort(editValue, types, connectionStatuses, nodeCollectionSortBy);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			w_explorerControl.ResumeLayout();
		}
	}

        public ExplorerTreeNode CreateTreeNode(Node node)
	{
		return w_explorerControl.CreateTreeNode(node);
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void Initialize()
	{
		InitializeTypesList();
		InitializeStatusesList();
		InitializeSortList();
		w_explorerControl.SelectedNodeChanged += delegate(ReadOnlyCollection<ExplorerTreeNode> nodes)
		{
			if (this.SelectedNodeChanged != null)
			{
				this.SelectedNodeChanged(nodes);
			}
		};
		_filterTextEditControl.KeyDown += delegate(object sender, KeyEventArgs args)
		{
			if (args.KeyCode == Keys.Return)
			{
				_filterInputBox.EditValue = _barManager.ActiveEditor.EditValue;
				ApplyFilterSort();
			}
		};
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.w_explorerControl = new Metalogix.UI.WinForms.Widgets.ExplorerControl();
		this._barManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
		this._filterBar = new DevExpress.XtraBars.Bar();
		this._filterLabel = new DevExpress.XtraBars.BarStaticItem();
		this._filterInputBox = new DevExpress.XtraBars.BarEditItem();
		this._filterTextEditControl = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this._filterButton = new DevExpress.XtraBars.BarButtonItem();
		this._connectionButton = new DevExpress.XtraBars.BarButtonItem();
		this._connectionsMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this._statusButton = new DevExpress.XtraBars.BarButtonItem();
		this._statusMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this._sortByLabel = new DevExpress.XtraBars.BarStaticItem();
		this._sortingComboBox = new DevExpress.XtraBars.BarEditItem();
		this._sortByComboEdit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barCheckItem1 = new DevExpress.XtraBars.BarCheckItem();
		this._separatorPanel = new DevExpress.XtraEditors.PanelControl();
		this._toolBarWhiteSpacePanel = new DevExpress.XtraEditors.PanelControl();
		this._explorerWhiteSpacePanel = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._filterTextEditControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._connectionsMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._statusMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._sortByComboEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._separatorPanel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._toolBarWhiteSpacePanel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._explorerWhiteSpacePanel).BeginInit();
		base.SuspendLayout();
		this.w_explorerControl.Actions = new Metalogix.Actions.Action[0];
		this.w_explorerControl.BackColor = System.Drawing.Color.White;
		this.w_explorerControl.CheckBoxes = false;
		this.w_explorerControl.DataSource = null;
		this.w_explorerControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.w_explorerControl.Location = new System.Drawing.Point(0, 30);
		this.w_explorerControl.MultiSelectEnabled = false;
		this.w_explorerControl.MultiSelectLimitationMethod = null;
		this.w_explorerControl.Name = "w_explorerControl";
		this.w_explorerControl.Size = new System.Drawing.Size(594, 451);
		this.w_explorerControl.TabIndex = 1;
		this._barManager.AllowCustomization = false;
		this._barManager.AllowMoveBarOnToolbar = false;
		this._barManager.AllowQuickCustomization = false;
		this._barManager.AllowShowToolbarsPopup = false;
		this._barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this._filterBar });
		this._barManager.DockControls.Add(this.barDockControlTop);
		this._barManager.DockControls.Add(this.barDockControlBottom);
		this._barManager.DockControls.Add(this.barDockControlLeft);
		this._barManager.DockControls.Add(this.barDockControlRight);
		this._barManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._barManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[8] { this._filterLabel, this._filterInputBox, this._filterButton, this._connectionButton, this._statusButton, this._sortByLabel, this._sortingComboBox, this.barCheckItem1 };
		items.AddRange(barItemArray);
		this._barManager.MainMenu = this._filterBar;
		this._barManager.MaxItemId = 9;
		DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._barManager.RepositoryItems;
		DevExpress.XtraEditors.Repository.RepositoryItem[] repositoryItemArray = new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this._filterTextEditControl, this._sortByComboEdit };
		repositoryItems.AddRange(repositoryItemArray);
		this._barManager.ShowScreenTipsInToolbars = false;
		this._barManager.ShowShortcutInScreenTips = false;
		this._filterBar.BarItemHorzIndent = 3;
		this._filterBar.BarItemVertIndent = 0;
		this._filterBar.BarName = "Main menu";
		this._filterBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
		this._filterBar.DockCol = 0;
		this._filterBar.DockRow = 0;
		this._filterBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this._filterBar.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._filterLabel),
			new DevExpress.XtraBars.LinkPersistInfo(this._filterInputBox),
			new DevExpress.XtraBars.LinkPersistInfo(this._filterButton),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._connectionButton, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard),
			new DevExpress.XtraBars.LinkPersistInfo(this._statusButton),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._sortByLabel, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(this._sortingComboBox)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this._filterBar.OptionsBar.AllowQuickCustomization = false;
		this._filterBar.OptionsBar.DrawBorder = false;
		this._filterBar.OptionsBar.DrawDragBorder = false;
		this._filterBar.OptionsBar.RotateWhenVertical = false;
		this._filterBar.OptionsBar.UseWholeRow = true;
		this._filterBar.Text = "Filter Menu";
		this._filterLabel.Caption = "Filter:";
		this._filterLabel.Id = 1;
		this._filterLabel.Name = "_filterLabel";
		this._filterLabel.TextAlignment = System.Drawing.StringAlignment.Near;
		this._filterInputBox.Caption = "_filterInputBox";
		this._filterInputBox.Edit = this._filterTextEditControl;
		this._filterInputBox.Id = 2;
		this._filterInputBox.Name = "_filterInputBox";
		this._filterInputBox.Width = 150;
		this._filterTextEditControl.AutoHeight = false;
		this._filterTextEditControl.Name = "_filterTextEditControl";
		this._filterButton.Caption = "Filter";
		this._filterButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Filter2;
		this._filterButton.Id = 3;
		this._filterButton.Name = "_filterButton";
		this._filterButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_filterButton_ItemClick);
		this._connectionButton.ActAsDropDown = true;
		this._connectionButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
		this._connectionButton.Caption = "Connection";
		this._connectionButton.DropDownControl = this._connectionsMenu;
		this._connectionButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ConnectionType16;
		this._connectionButton.Id = 4;
		this._connectionButton.Name = "_connectionButton";
		this._connectionsMenu.Manager = this._barManager;
		this._connectionsMenu.Name = "_connectionsMenu";
		this._statusButton.ActAsDropDown = true;
		this._statusButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
		this._statusButton.Caption = "Status";
		this._statusButton.DropDownControl = this._statusMenu;
		this._statusButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.ConnectionStatus16;
		this._statusButton.Id = 5;
		this._statusButton.Name = "_statusButton";
		this._statusMenu.Manager = this._barManager;
		this._statusMenu.Name = "_statusMenu";
		this._sortByLabel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
		this._sortByLabel.Caption = "Sort By:";
		this._sortByLabel.Glyph = Metalogix.UI.WinForms.Properties.Resources.SortBy16;
		this._sortByLabel.Id = 6;
		this._sortByLabel.Name = "_sortByLabel";
		this._sortByLabel.TextAlignment = System.Drawing.StringAlignment.Near;
		this._sortingComboBox.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
		this._sortingComboBox.AllowRightClickInMenu = false;
		this._sortingComboBox.Caption = "SortBy";
		this._sortingComboBox.Edit = this._sortByComboEdit;
		this._sortingComboBox.Id = 7;
		this._sortingComboBox.Name = "_sortingComboBox";
		this._sortingComboBox.Width = 75;
		this._sortByComboEdit.AutoComplete = false;
		this._sortByComboEdit.AutoHeight = false;
		this._sortByComboEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this._sortByComboEdit.Name = "_sortByComboEdit";
		this._sortByComboEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Size = new System.Drawing.Size(594, 22);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 481);
		this.barDockControlBottom.Size = new System.Drawing.Size(594, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 459);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(594, 22);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 459);
		this.barCheckItem1.Caption = "Check Test";
		this.barCheckItem1.Id = 8;
		this.barCheckItem1.Name = "barCheckItem1";
		this._separatorPanel.Appearance.BackColor = System.Drawing.Color.Silver;
		this._separatorPanel.Appearance.Options.UseBackColor = true;
		this._separatorPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._separatorPanel.Dock = System.Windows.Forms.DockStyle.Top;
		this._separatorPanel.Location = new System.Drawing.Point(0, 26);
		this._separatorPanel.Name = "_separatorPanel";
		this._separatorPanel.Size = new System.Drawing.Size(594, 1);
		this._separatorPanel.TabIndex = 6;
		this._toolBarWhiteSpacePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._toolBarWhiteSpacePanel.Dock = System.Windows.Forms.DockStyle.Top;
		this._toolBarWhiteSpacePanel.Location = new System.Drawing.Point(0, 22);
		this._toolBarWhiteSpacePanel.Name = "_toolBarWhiteSpacePanel";
		this._toolBarWhiteSpacePanel.Size = new System.Drawing.Size(594, 4);
		this._toolBarWhiteSpacePanel.TabIndex = 11;
		this._explorerWhiteSpacePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._explorerWhiteSpacePanel.Dock = System.Windows.Forms.DockStyle.Top;
		this._explorerWhiteSpacePanel.Location = new System.Drawing.Point(0, 27);
		this._explorerWhiteSpacePanel.Name = "_explorerWhiteSpacePanel";
		this._explorerWhiteSpacePanel.Size = new System.Drawing.Size(594, 3);
		this._explorerWhiteSpacePanel.TabIndex = 12;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.w_explorerControl);
		base.Controls.Add(this._explorerWhiteSpacePanel);
		base.Controls.Add(this._separatorPanel);
		base.Controls.Add(this._toolBarWhiteSpacePanel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ExplorerControlWithNavigation";
		base.Size = new System.Drawing.Size(594, 481);
		((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this._filterTextEditControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this._connectionsMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this._statusMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this._sortByComboEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this._separatorPanel).EndInit();
		((System.ComponentModel.ISupportInitialize)this._toolBarWhiteSpacePanel).EndInit();
		((System.ComponentModel.ISupportInitialize)this._explorerWhiteSpacePanel).EndInit();
		base.ResumeLayout(false);
	}

        private void InitializeList<T>(PopupMenu dropDown, List<KeyValuePair<string, T>> items)
	{
		try
		{
			if (dropDown == null)
			{
				return;
			}
			if (items != null)
			{
				IEnumerator<KeyValuePair<string, T>> enumerator = items.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, T> current = enumerator.Current;
					BarCheckItem barCheckItem = new BarCheckItem
					{
						Caption = current.Key,
						Checked = true,
						Tag = current.Value
					};
					barCheckItem.CheckedChanged += delegate
					{
						ApplyFilterSort();
					};
					dropDown.ItemLinks.Add(barCheckItem);
				}
			}
			else
			{
				dropDown.ClearLinks();
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void InitializeSortList()
	{
		try
		{
			_sortByComboEdit.Items.AddRange(Enum.GetNames(typeof(NodeCollectionSortBy)));
			if (_sortByComboEdit.Items.Count > 0)
			{
				_sortingComboBox.EditValue = _sortByComboEdit.Items[0];
			}
			_sortByComboEdit.SelectedIndexChanged += delegate
			{
				_sortingComboBox.EditValue = _barManager.ActiveEditor.EditValue;
				ApplyFilterSort();
			};
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void InitializeStatusesList()
	{
		try
		{
			List<KeyValuePair<string, ConnectionStatus>> keyValuePairs = new List<KeyValuePair<string, ConnectionStatus>>();
			string[] names = Enum.GetNames(typeof(ConnectionStatus));
			foreach (string str in names)
			{
				keyValuePairs.Add(new KeyValuePair<string, ConnectionStatus>(str, (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), str)));
			}
			InitializeList(_statusMenu, keyValuePairs);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void InitializeTypesList()
	{
		try
		{
			Dictionary<string, List<Type>> strs = new Dictionary<string, List<Type>>();
			List<KeyValuePair<string, List<Type>>> keyValuePairs = new List<KeyValuePair<string, List<Type>>>();
			foreach (Type typesOfInterface in ReflectionUtils.GetTypesOfInterface<Connection>(ApplicationData.MainAssembly, searchReferences: true))
			{
				ExplorerNavigationFilterableAttribute singleAttributeFromType = ReflectionUtils.GetSingleAttributeFromType<ExplorerNavigationFilterableAttribute>(typesOfInterface);
				if (singleAttributeFromType == null || singleAttributeFromType.IsFilterable)
				{
					UserFriendlyNodeNameAttribute userFriendlyNodeNameAttribute = ReflectionUtils.GetSingleAttributeFromType<UserFriendlyNodeNameAttribute>(typesOfInterface);
					string str = ((userFriendlyNodeNameAttribute == null) ? typesOfInterface.Name : userFriendlyNodeNameAttribute.Name);
					if (!strs.ContainsKey(str))
					{
						List<Type> types = new List<Type>(new Type[1] { typesOfInterface });
						strs.Add(str, types);
						keyValuePairs.Add(new KeyValuePair<string, List<Type>>(str, types));
					}
					else
					{
						strs[str].Add(typesOfInterface);
					}
				}
			}
			keyValuePairs.Sort((KeyValuePair<string, List<Type>> pair, KeyValuePair<string, List<Type>> valuePair) => string.CompareOrdinal(pair.Key, valuePair.Key));
			InitializeList(_connectionsMenu, keyValuePairs);
		}
		catch (ReflectionTypeLoadException ex)
		{
			StringBuilder sb = new StringBuilder();
			Exception[] loaderExceptions = ex.LoaderExceptions;
			foreach (Exception exSub in loaderExceptions)
			{
				sb.AppendLine(exSub.Message);
				if (exSub is FileNotFoundException exFileNotFound && !string.IsNullOrEmpty(exFileNotFound.FusionLog))
				{
					sb.AppendLine("Fusion Log:");
					sb.AppendLine(exFileNotFound.FusionLog);
				}
				sb.AppendLine();
			}
			string errorMessage = sb.ToString();
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void NavigateToLocation(Location location)
	{
		w_explorerControl.NavigateToLocation(location);
	}

        public void NavigateToLocation(IEnumerable<Location> locations)
	{
		w_explorerControl.NavigateToLocation(locations);
	}

        public void NavigateToNode(Node node)
	{
		w_explorerControl.NavigateToNode(node);
	}

        public void NavigateToNode(IEnumerable<Node> nodes)
	{
		w_explorerControl.NavigateToNode(nodes);
	}

        public void Redraw()
	{
		w_explorerControl.Redraw();
	}

        private void ResetFilterSort()
	{
		_filterInputBox.EditValue = string.Empty;
		foreach (BarItemLink itemLink in _connectionsMenu.ItemLinks)
		{
			((BarCheckItem)itemLink.Item).Checked = true;
		}
		foreach (BarItemLink barItemLink in _statusMenu.ItemLinks)
		{
			((BarCheckItem)barItemLink.Item).Checked = true;
		}
		if (_sortByComboEdit.Items.Count > 0)
		{
			_sortingComboBox.EditValue = _sortByComboEdit.Items[0];
		}
	}
    }
}
