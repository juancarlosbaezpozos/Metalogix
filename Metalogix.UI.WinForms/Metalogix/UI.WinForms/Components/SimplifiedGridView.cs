using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Metalogix.UI.WinForms.Components
{
    public class SimplifiedGridView : XtraUserControl
    {
        private delegate void BuildTableFromDataSourceDelegate();

        private delegate object[] GetSelectedItemsDelegate();

        private delegate void SetSelectedItemsDelegate(object[] items);

        public delegate void UpdateDataRowDelegate(DataRow row, object obj);

        private Dictionary<string, GridColumn> _columnMap = new Dictionary<string, GridColumn>();

        private Dictionary<object, DataRow> _itemToRowMap = new Dictionary<object, DataRow>();

        private Dictionary<DataRow, object> _rowToItemMap = new Dictionary<DataRow, object>();

        private DataTable _table = new DataTable();

        private IBindingList _dataSource;

        private UpdateDataRowDelegate _rowBuilder = DefaultRowBuilder;

        private bool _updateInProgress;

        private IContainer components;

        private GridControl _grid;

        private GridView _gridView;

        public bool ColumnAutoWidth
        {
            get
		{
			return _gridView.OptionsView.ColumnAutoWidth;
		}
            set
		{
			_gridView.OptionsView.ColumnAutoWidth = value;
		}
        }

        public IBindingList DataSource
        {
            get
		{
			return _dataSource;
		}
            set
		{
			if (_dataSource != null)
			{
				DettachEvents();
			}
			_dataSource = value;
			BuildTableFromDataSource();
			AttachEvents();
		}
        }

        public ContextMenuStrip GridContextMenu
        {
            get
		{
			return _grid.ContextMenuStrip;
		}
            set
		{
			_grid.ContextMenuStrip = value;
		}
        }

        public bool MultiSelect
        {
            get
		{
			return _gridView.OptionsSelection.MultiSelect;
		}
            set
		{
			_gridView.OptionsSelection.MultiSelect = value;
		}
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UpdateDataRowDelegate RowBuilderMethod
        {
            get
		{
			return _rowBuilder;
		}
            set
		{
			if (value == null)
			{
				_rowBuilder = DefaultRowBuilder;
				return;
			}
			_rowBuilder = value;
			BuildTableFromDataSource();
		}
        }

        public object[] SelectedItems
        {
            get
		{
			return GetSelectedItems();
		}
            set
		{
			SetSelectedItems(value);
		}
        }

        public bool ShowColumnHeaders
        {
            get
		{
			return _gridView.OptionsView.ShowColumnHeaders;
		}
            set
		{
			_gridView.OptionsView.ShowColumnHeaders = value;
		}
        }

        public DefaultBoolean ShowGridLines
        {
            get
		{
			return _gridView.OptionsView.ShowHorizontalLines;
		}
            set
		{
			_gridView.OptionsView.ShowHorizontalLines = value;
			_gridView.OptionsView.ShowVerticalLines = value;
		}
        }

        public event EventHandler SelectionChanged;

        public SimplifiedGridView()
	{
		InitializeComponent();
	}

        private void _gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (!_gridView.OptionsSelection.MultiSelect && this.SelectionChanged != null)
		{
			this.SelectionChanged(this, new EventArgs());
		}
	}

        private void _gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_gridView.OptionsSelection.MultiSelect && this.SelectionChanged != null)
		{
			this.SelectionChanged(this, new EventArgs());
		}
	}

        private void AttachEvents()
	{
		IBindingList bindingLists = _dataSource;
		if (bindingLists != null)
		{
			bindingLists.ListChanged += HandleListChanged;
		}
	}

        public void BeginUpdate()
	{
		_updateInProgress = true;
		_grid.BeginUpdate();
	}

        private void BuildTableFromDataSource()
	{
		if (base.InvokeRequired)
		{
			Invoke(new BuildTableFromDataSourceDelegate(BuildTableFromDataSource));
			return;
		}
		bool flag = !_updateInProgress;
		if (flag)
		{
			BeginUpdate();
		}
		_grid.DataSource = null;
		try
		{
			_table.Rows.Clear();
			_rowToItemMap.Clear();
			_itemToRowMap.Clear();
			IBindingList bindingLists = _dataSource;
			if (bindingLists == null)
			{
				return;
			}
			foreach (object obj in bindingLists)
			{
				DataRow dataRow = CreateNewRow(obj);
				_table.Rows.Add(dataRow);
			}
		}
		finally
		{
			_grid.DataSource = _table;
			if (flag)
			{
				EndUpdate();
			}
		}
	}

        public void ClearColumns()
	{
		_gridView.Columns.Clear();
	}

        public void ClearSorting()
	{
		_gridView.SortInfo.ClearSorting();
	}

        public void CreateColumn(string displayName, string fieldName, int width = -1)
	{
		GridColumn gridColumn = new GridColumn
		{
			Caption = displayName,
			FieldName = fieldName,
			Visible = true
		};
		if (width <= 0)
		{
			gridColumn.BestFit();
		}
		else
		{
			gridColumn.Width = width;
		}
		_gridView.Columns.Add(gridColumn);
		_table.Columns.Add(fieldName, typeof(string));
		_columnMap.Add(fieldName, gridColumn);
	}

        public void CreateColumnsFromProperties(Type type, bool onlyWithDisplayName = false, int columnWidth = -1)
	{
		if (type == null)
		{
			return;
		}
		PropertyInfo[] properties = type.GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			DisplayNameAttribute customAttribute = (DisplayNameAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute));
			if (propertyInfo.CanRead && (customAttribute != null || !onlyWithDisplayName))
			{
				CreateColumn((customAttribute != null) ? customAttribute.DisplayName : propertyInfo.Name, propertyInfo.Name, columnWidth);
			}
		}
	}

        private DataRow CreateNewRow(object obj)
	{
		DataRow dataRow = _table.NewRow();
		RowBuilderMethod(dataRow, obj);
		_itemToRowMap.Add(obj, dataRow);
		_rowToItemMap.Add(dataRow, obj);
		return dataRow;
	}

        private static void DefaultRowBuilder(DataRow row, object obj)
	{
		if (obj == null)
		{
			return;
		}
		Type type = obj.GetType();
		foreach (DataColumn column in row.Table.Columns)
		{
			PropertyInfo property = type.GetProperty(column.ColumnName);
			if (!(property == null))
			{
				object value = property.GetValue(obj, null);
				row[column] = ((value == null) ? "" : value.ToString());
			}
		}
	}

        private void DettachEvents()
	{
		IBindingList bindingLists = _dataSource;
		if (bindingLists != null)
		{
			bindingLists.ListChanged -= HandleListChanged;
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

        public void EndUpdate()
	{
		_grid.EndUpdate();
		_updateInProgress = false;
	}

        public GridColumn GetColumnByFieldName(string fieldName)
	{
		if (_columnMap.TryGetValue(fieldName, out var gridColumn))
		{
			return gridColumn;
		}
		return null;
	}

        public GridColumn[] GetColumns()
	{
		GridColumn[] gridColumnArray = new GridColumn[_columnMap.Values.Count];
		_columnMap.Values.CopyTo(gridColumnArray, 0);
		return gridColumnArray;
	}

        public T[] GetSelectedItems<T>()
	{
		return (from i in SelectedItems
			where typeof(T).IsAssignableFrom(i.GetType())
			select (T)i).ToArray();
	}

        private object[] GetSelectedItems()
	{
		if (base.InvokeRequired)
		{
			return Invoke(new GetSelectedItemsDelegate(GetSelectedItems)) as object[];
		}
		int[] selectedRows = _gridView.GetSelectedRows();
		if (selectedRows == null)
		{
			return new object[0];
		}
		List<object> objs = new List<object>(selectedRows.Length);
		int[] numArray = selectedRows;
		foreach (int num in numArray)
		{
			if (_gridView.GetRow(num) is DataRowView row)
			{
				objs.Add(_rowToItemMap[row.Row]);
			}
		}
		return objs.ToArray();
	}

        private void HandleItemAdded(object item, int index)
	{
		DataRow dataRow = CreateNewRow(item);
		if (index >= _table.Rows.Count)
		{
			_table.Rows.Add(dataRow);
		}
		else
		{
			_table.Rows.InsertAt(dataRow, index);
		}
	}

        private void HandleItemChanged(object item)
	{
		RowBuilderMethod(_itemToRowMap[item], item);
	}

        private void HandleItemDelete(int index)
	{
		DataRow item = _table.Rows[index];
		_itemToRowMap.Remove(_rowToItemMap[item]);
		_rowToItemMap.Remove(item);
		_table.Rows.Remove(item);
	}

        private void HandleItemMoved(int oldIndex, int newIndex)
	{
		DataRow item = _table.Rows[oldIndex];
		_table.Rows.RemoveAt(oldIndex);
		if (newIndex >= _table.Rows.Count)
		{
			_table.Rows.Add(item);
		}
		else
		{
			_table.Rows.InsertAt(item, newIndex);
		}
	}

        private void HandleListChanged(object sender, ListChangedEventArgs args)
	{
		if (base.InvokeRequired)
		{
			ListChangedEventHandler listChangedEventHandler = HandleListChanged;
			object[] objArray = new object[2] { sender, args };
			Invoke(listChangedEventHandler, objArray);
			return;
		}
		switch (args.ListChangedType)
		{
		case ListChangedType.Reset:
			HandleReset();
			break;
		case ListChangedType.ItemAdded:
		{
			object item = DataSource[args.NewIndex];
			HandleItemAdded(item, args.NewIndex);
			break;
		}
		case ListChangedType.ItemDeleted:
			HandleItemDelete(args.NewIndex);
			break;
		case ListChangedType.ItemMoved:
			HandleItemMoved(args.OldIndex, args.NewIndex);
			break;
		case ListChangedType.ItemChanged:
			HandleItemChanged(DataSource[args.NewIndex]);
			break;
		}
	}

        private void HandleReset()
	{
		BuildTableFromDataSource();
	}

        private void InitializeComponent()
	{
		this._grid = new DevExpress.XtraGrid.GridControl();
		this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		((System.ComponentModel.ISupportInitialize)this._grid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).BeginInit();
		base.SuspendLayout();
		this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
		this._grid.EmbeddedNavigator.ShowToolTips = false;
		this._grid.Location = new System.Drawing.Point(0, 0);
		this._grid.MainView = this._gridView;
		this._grid.Name = "_grid";
		this._grid.Size = new System.Drawing.Size(480, 320);
		this._grid.TabIndex = 0;
		this._grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridView });
		this._gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this._gridView.GridControl = this._grid;
		this._gridView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
		this._gridView.Name = "_gridView";
		this._gridView.OptionsBehavior.AutoUpdateTotalSummary = false;
		this._gridView.OptionsBehavior.Editable = false;
		this._gridView.OptionsCustomization.AllowColumnMoving = false;
		this._gridView.OptionsCustomization.AllowFilter = false;
		this._gridView.OptionsCustomization.AllowGroup = false;
		this._gridView.OptionsCustomization.AllowSort = false;
		this._gridView.OptionsDetail.AllowZoomDetail = false;
		this._gridView.OptionsDetail.EnableMasterViewMode = false;
		this._gridView.OptionsDetail.ShowDetailTabs = false;
		this._gridView.OptionsDetail.SmartDetailExpand = false;
		this._gridView.OptionsFilter.AllowColumnMRUFilterList = false;
		this._gridView.OptionsFilter.AllowMRUFilterList = false;
		this._gridView.OptionsHint.ShowCellHints = false;
		this._gridView.OptionsHint.ShowColumnHeaderHints = false;
		this._gridView.OptionsHint.ShowFooterHints = false;
		this._gridView.OptionsMenu.EnableColumnMenu = false;
		this._gridView.OptionsMenu.EnableFooterMenu = false;
		this._gridView.OptionsMenu.EnableGroupPanelMenu = false;
		this._gridView.OptionsNavigation.AutoFocusNewRow = true;
		this._gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this._gridView.OptionsView.ColumnAutoWidth = false;
		this._gridView.OptionsView.ShowDetailButtons = false;
		this._gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this._gridView.OptionsView.ShowGroupPanel = false;
		this._gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.OptionsView.ShowIndicator = false;
		this._gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.SynchronizeClones = false;
		this._gridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(_gridView_SelectionChanged);
		this._gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(_gridView_FocusedRowChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this._grid);
		base.Name = "SimplifiedGridView";
		base.Size = new System.Drawing.Size(480, 320);
		((System.ComponentModel.ISupportInitialize)this._grid).EndInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).EndInit();
		base.ResumeLayout(false);
	}

        public void RefreshAllItems()
	{
		_grid.BeginUpdate();
		try
		{
			foreach (KeyValuePair<DataRow, object> keyValuePair in _rowToItemMap)
			{
				RowBuilderMethod(keyValuePair.Key, keyValuePair.Value);
			}
		}
		finally
		{
			_grid.EndUpdate();
		}
	}

        public void RunAutoWidthOnAllColumns()
	{
		_gridView.BestFitColumns();
	}

        public void RunColumnAutoWidth(string fieldName)
	{
		GetColumnByFieldName(fieldName).BestFit();
	}

        private void SetSelectedItems(object[] items)
	{
		if (base.InvokeRequired)
		{
			Invoke(new SetSelectedItemsDelegate(SetSelectedItems), items);
			return;
		}
		_gridView.ClearSelection();
		if (items != null)
		{
			foreach (object obj in items)
			{
				DataRow item = _itemToRowMap[obj];
				_gridView.SelectRow(_gridView.GetRowHandle(_table.Rows.IndexOf(item)));
			}
		}
	}

        public void SortColumns(IList<string> fieldNames, IList<bool> ascending = null)
	{
		if (fieldNames == null || fieldNames.Count == 0)
		{
			return;
		}
		List<GridColumn> gridColumns = new List<GridColumn>(fieldNames.Count);
		foreach (string fieldName in fieldNames)
		{
			GridColumn columnByFieldName = GetColumnByFieldName(fieldName);
			if (columnByFieldName == null)
			{
				throw new Exception($"Failed to sort columns. Could not locate a column with field name '{fieldName}'.");
			}
			gridColumns.Add(columnByFieldName);
		}
		SortColumns(gridColumns, ascending);
	}

        public void SortColumns(IList<GridColumn> columns, IList<bool> ascending = null)
	{
		if (columns != null && columns.Count != 0)
		{
			if (ascending != null && ascending.Count != columns.Count)
			{
				throw new Exception("Failed to sort columns. The supplied ascending list and the column list are not the same length.");
			}
			GridColumnSortInfo[] gridColumnSortInfo = new GridColumnSortInfo[columns.Count];
			for (int i = 0; i < columns.Count; i++)
			{
				gridColumnSortInfo[i] = new GridColumnSortInfo(columns[i], (ascending == null || ascending[i]) ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending);
			}
			_gridView.SortInfo.ClearAndAddRange(gridColumnSortInfo);
		}
	}

        public void UpdateChangedItem(object item)
	{
		if (_itemToRowMap.TryGetValue(item, out var dataRow))
		{
			RowBuilderMethod(dataRow, item);
		}
	}
    }
}
