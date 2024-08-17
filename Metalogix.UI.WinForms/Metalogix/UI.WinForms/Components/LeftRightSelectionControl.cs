using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Components
{
    public class LeftRightSelectionControl : XtraUserControl
    {
        public delegate void SelectionChangedDelegate(object[] changedItems);

        public delegate string StringParserDelegate(object item);

        private const string DISPLAY_COLUMN_NAME = "Name";

        private const string SORT_COLUMN_NAME = "Sort";

        private const int BUTTON_GRID_SEPARATION = 6;

        private BindingList<object> _deselectedItems;

        private BindingList<object> _selectedItems;

        private StringParserDelegate _itemNamingMethod;

        private StringParserDelegate _itemSortValueMethod;

        private SortingType _sortType;

        private IContainer components;

        private SimplifiedGridView _availableItemsGrid;

        private SimplifiedGridView _selectedItemsGrid;

        private LabelControl _selectedLabel;

        private LabelControl _deselectedLabel;

        private PictureButton _selectButton;

        private PictureButton _deselectButton;

        public string DeselectedItemsLabel
        {
            get
		{
			return _deselectedLabel.Text;
		}
            set
		{
			_deselectedLabel.Text = value;
		}
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StringParserDelegate ItemNamingMethod
        {
            get
		{
			return _itemNamingMethod;
		}
            set
		{
			_itemNamingMethod = value ?? new StringParserDelegate(DefaultItemNamingMethod);
			_availableItemsGrid.RefreshAllItems();
			_selectedItemsGrid.RefreshAllItems();
		}
        }

        public SortingType ItemSorting
        {
            get
		{
			return _sortType;
		}
            set
		{
			if (_sortType != value)
			{
				_sortType = value;
				if (_sortType == SortingType.Ascending)
				{
					SimplifiedGridView simplifiedGridView = _availableItemsGrid;
					string[] strArrays = new string[1] { "Sort" };
					simplifiedGridView.SortColumns(strArrays, new bool[1] { true });
					SimplifiedGridView simplifiedGridView1 = _selectedItemsGrid;
					string[] strArrays1 = new string[1] { "Sort" };
					simplifiedGridView1.SortColumns(strArrays1, new bool[1] { true });
				}
				else if (_sortType != SortingType.Descending)
				{
					_availableItemsGrid.ClearSorting();
					_selectedItemsGrid.ClearSorting();
				}
				else
				{
					SimplifiedGridView simplifiedGridView2 = _availableItemsGrid;
					string[] strArrays2 = new string[1] { "Sort" };
					simplifiedGridView2.SortColumns(strArrays2, new bool[1]);
					SimplifiedGridView simplifiedGridView3 = _selectedItemsGrid;
					string[] strArrays3 = new string[1] { "Sort" };
					simplifiedGridView3.SortColumns(strArrays3, new bool[1]);
				}
			}
		}
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StringParserDelegate ItemSortValueMethod
        {
            get
		{
			return _itemSortValueMethod;
		}
            set
		{
			_itemSortValueMethod = value;
			_availableItemsGrid.RefreshAllItems();
			_selectedItemsGrid.RefreshAllItems();
		}
        }

        public string SelectedItemsLabel
        {
            get
		{
			return _selectedLabel.Text;
		}
            set
		{
			_selectedLabel.Text = value;
		}
        }

        public event SelectionChangedDelegate SelectionChanged;

        public LeftRightSelectionControl()
	{
		InitializeComponent();
		InitializeDataGrids();
		RecalculateControlSizingAndPositions();
		UpdateEnabledStates();
	}

        private void _availableItemsGrid_SelectionChanged(object sender, EventArgs e)
	{
		UpdateEnabledStates();
	}

        private void _deselectButton_Click(object sender, EventArgs e)
	{
		ShiftSelectedItems(_selectedItemsGrid, _availableItemsGrid);
	}

        private void _selectButton_Click(object sender, EventArgs e)
	{
		ShiftSelectedItems(_availableItemsGrid, _selectedItemsGrid);
	}

        private void _selectedItemsGrid_SelectionChanged(object sender, EventArgs e)
	{
		UpdateEnabledStates();
	}

        public void ClearSelection()
	{
		SetSelectedItems<object>(null);
	}

        private string DefaultItemNamingMethod(object item)
	{
		if (item == null)
		{
			return "<NULL>";
		}
		return item.ToString();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void FireSelectionChanged(object[] changedItems)
	{
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged(changedItems);
		}
	}

        public T[] GetAvailableItems<T>()
	{
		return (from o in _deselectedItems
			where o is T
			select (T)o).Concat(GetSelectedItems<T>()).ToArray();
	}

        public T[] GetSelectedItems<T>()
	{
		return (from o in _selectedItems
			where o is T
			select (T)o).ToArray();
	}

        private void InitializeComponent()
	{
		this._selectedLabel = new DevExpress.XtraEditors.LabelControl();
		this._deselectedLabel = new DevExpress.XtraEditors.LabelControl();
		this._deselectButton = new Metalogix.UI.WinForms.Components.PictureButton();
		this._selectButton = new Metalogix.UI.WinForms.Components.PictureButton();
		this._selectedItemsGrid = new Metalogix.UI.WinForms.Components.SimplifiedGridView();
		this._availableItemsGrid = new Metalogix.UI.WinForms.Components.SimplifiedGridView();
		((System.ComponentModel.ISupportInitialize)this._deselectButton.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._selectButton.Properties).BeginInit();
		base.SuspendLayout();
		this._selectedLabel.Location = new System.Drawing.Point(240, 0);
		this._selectedLabel.Name = "_selectedLabel";
		this._selectedLabel.Size = new System.Drawing.Size(41, 13);
		this._selectedLabel.TabIndex = 3;
		this._selectedLabel.Text = "Selected";
		this._deselectedLabel.Location = new System.Drawing.Point(0, 0);
		this._deselectedLabel.Name = "_deselectedLabel";
		this._deselectedLabel.Size = new System.Drawing.Size(53, 13);
		this._deselectedLabel.TabIndex = 4;
		this._deselectedLabel.Text = "Deselected";
		this._deselectButton.BackgroundColor = System.Drawing.Color.Transparent;
		this._deselectButton.ClickColor = System.Drawing.Color.FromArgb(178, 194, 228);
		this._deselectButton.EditValue = Metalogix.UI.WinForms.Properties.Resources.Left16;
		this._deselectButton.Location = new System.Drawing.Point(202, 248);
		this._deselectButton.MouseOverColor = System.Drawing.Color.FromArgb(227, 236, 255);
		this._deselectButton.Name = "_deselectButton";
		this._deselectButton.Properties.AllowFocused = false;
		this._deselectButton.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this._deselectButton.Properties.Appearance.Options.UseBackColor = true;
		this._deselectButton.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._deselectButton.Properties.ReadOnly = true;
		this._deselectButton.Properties.ShowMenu = false;
		this._deselectButton.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
		this._deselectButton.Size = new System.Drawing.Size(32, 24);
		this._deselectButton.TabIndex = 6;
		this._deselectButton.Click += new System.EventHandler(_deselectButton_Click);
		this._selectButton.BackgroundColor = System.Drawing.Color.Transparent;
		this._selectButton.ClickColor = System.Drawing.Color.FromArgb(178, 194, 228);
		this._selectButton.EditValue = Metalogix.UI.WinForms.Properties.Resources.Right16;
		this._selectButton.Location = new System.Drawing.Point(202, 128);
		this._selectButton.MouseOverColor = System.Drawing.Color.FromArgb(227, 236, 255);
		this._selectButton.Name = "_selectButton";
		this._selectButton.Properties.AllowFocused = false;
		this._selectButton.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this._selectButton.Properties.Appearance.Options.UseBackColor = true;
		this._selectButton.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._selectButton.Properties.ReadOnly = true;
		this._selectButton.Properties.ShowMenu = false;
		this._selectButton.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
		this._selectButton.Size = new System.Drawing.Size(32, 24);
		this._selectButton.TabIndex = 5;
		this._selectButton.Click += new System.EventHandler(_selectButton_Click);
		this._selectedItemsGrid.ColumnAutoWidth = true;
		this._selectedItemsGrid.DataSource = null;
		this._selectedItemsGrid.GridContextMenu = null;
		this._selectedItemsGrid.Location = new System.Drawing.Point(240, 19);
		this._selectedItemsGrid.MultiSelect = true;
		this._selectedItemsGrid.Name = "_selectedItemsGrid";
		this._selectedItemsGrid.SelectedItems = new object[0];
		this._selectedItemsGrid.ShowColumnHeaders = false;
		this._selectedItemsGrid.ShowGridLines = DevExpress.Utils.DefaultBoolean.False;
		this._selectedItemsGrid.Size = new System.Drawing.Size(196, 360);
		this._selectedItemsGrid.TabIndex = 3;
		this._selectedItemsGrid.SelectionChanged += new System.EventHandler(_selectedItemsGrid_SelectionChanged);
		this._availableItemsGrid.ColumnAutoWidth = true;
		this._availableItemsGrid.DataSource = null;
		this._availableItemsGrid.GridContextMenu = null;
		this._availableItemsGrid.Location = new System.Drawing.Point(0, 19);
		this._availableItemsGrid.MultiSelect = true;
		this._availableItemsGrid.Name = "_availableItemsGrid";
		this._availableItemsGrid.SelectedItems = new object[0];
		this._availableItemsGrid.ShowColumnHeaders = false;
		this._availableItemsGrid.ShowGridLines = DevExpress.Utils.DefaultBoolean.False;
		this._availableItemsGrid.Size = new System.Drawing.Size(196, 360);
		this._availableItemsGrid.TabIndex = 0;
		this._availableItemsGrid.SelectionChanged += new System.EventHandler(_availableItemsGrid_SelectionChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this._deselectButton);
		base.Controls.Add(this._selectButton);
		base.Controls.Add(this._deselectedLabel);
		base.Controls.Add(this._selectedLabel);
		base.Controls.Add(this._selectedItemsGrid);
		base.Controls.Add(this._availableItemsGrid);
		base.Name = "LeftRightSelectionControl";
		base.Size = new System.Drawing.Size(436, 379);
		base.Resize += new System.EventHandler(LeftRightSelectionControl_Resize);
		((System.ComponentModel.ISupportInitialize)this._deselectButton.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this._selectButton.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void InitializeDataGrids()
	{
		_itemNamingMethod = DefaultItemNamingMethod;
		_itemSortValueMethod = null;
		_sortType = SortingType.None;
		_deselectedItems = new BindingList<object>();
		_selectedItems = new BindingList<object>();
		InitializeSpecificGrid(_availableItemsGrid, _deselectedItems);
		InitializeSpecificGrid(_selectedItemsGrid, _selectedItems);
	}

        private void InitializeSpecificGrid(SimplifiedGridView grid, BindingList<object> dataSource)
	{
		grid.CreateColumn("Name", "Name");
		grid.CreateColumn("Sort", "Sort");
		grid.GetColumnByFieldName("Sort").Visible = false;
		grid.RowBuilderMethod = RowBuilderMethod;
		grid.DataSource = dataSource;
	}

        private void LeftRightSelectionControl_Resize(object sender, EventArgs e)
	{
		RecalculateControlSizingAndPositions();
	}

        private void RecalculateControlSizingAndPositions()
	{
		int width = base.Width;
		int height = base.Height;
		int num = _selectButton.Width;
		int height1 = _selectButton.Height;
		int num1 = width - num - 12;
		int num2 = num1 / 2;
		int num3 = num1 % 2;
		int y = height - _availableItemsGrid.Location.Y;
		int num4 = y / 3;
		int num5 = num2 + num3 + 6;
		int num6 = num5 + num + 6;
		SuspendLayout();
		try
		{
			_availableItemsGrid.Size = new Size(num2 + num3, y);
			PictureButton point = _selectButton;
			point.Location = new Point(num5, _availableItemsGrid.Location.Y + num4 - height1 / 2);
			PictureButton pictureButton = _deselectButton;
			pictureButton.Location = new Point(num5, _availableItemsGrid.Location.Y + num4 * 2 - height1 / 2);
			LabelControl labelControl = _selectedLabel;
			labelControl.Location = new Point(num6, _selectedLabel.Location.Y);
			SimplifiedGridView simplifiedGridView = _selectedItemsGrid;
			simplifiedGridView.Location = new Point(num6, _availableItemsGrid.Location.Y);
			_selectedItemsGrid.Size = new Size(num2, y);
		}
		finally
		{
			ResumeLayout();
		}
	}

        public void RefreshAllItems()
	{
		_availableItemsGrid.RefreshAllItems();
		_selectedItemsGrid.RefreshAllItems();
	}

        public void RefreshItem(object item)
	{
		if (_deselectedItems.Contains(item))
		{
			_availableItemsGrid.UpdateChangedItem(item);
		}
		else if (_selectedItems.Contains(item))
		{
			_selectedItemsGrid.UpdateChangedItem(item);
		}
	}

        private void RowBuilderMethod(DataRow row, object item)
	{
		row["Name"] = ItemNamingMethod(item);
		object itemSortValueMethod = ((ItemSortValueMethod != null) ? ItemSortValueMethod(item) : row["Name"]);
		row["Sort"] = itemSortValueMethod;
	}

        public void SetAvailableItems<T>(IEnumerable<T> items)
	{
		_availableItemsGrid.DataSource = null;
		_availableItemsGrid.BeginUpdate();
		_selectedItemsGrid.BeginUpdate();
		try
		{
			_deselectedItems.Clear();
			_selectedItems.Clear();
			foreach (T item in items)
			{
				object obj = item;
				_deselectedItems.Add(obj);
			}
		}
		finally
		{
			_availableItemsGrid.EndUpdate();
			_selectedItemsGrid.EndUpdate();
			_availableItemsGrid.DataSource = _deselectedItems;
		}
	}

        public void SetSelectedItems<T>(IEnumerable<T> items)
	{
		object[] array;
		object[] objArray;
		if (items == null)
		{
			array = new object[0];
			objArray = _selectedItems.ToArray();
		}
		else
		{
			array = _deselectedItems.Where((object o) => o is T && items.Contains((T)o)).ToArray();
			objArray = _selectedItems.Where((object o) => !(o is T) || !items.Contains((T)o)).ToArray();
		}
		_availableItemsGrid.BeginUpdate();
		_selectedItemsGrid.BeginUpdate();
		try
		{
			object[] objArray1 = array;
			foreach (object obj in objArray1)
			{
				_deselectedItems.Remove(obj);
				_selectedItems.Add(obj);
			}
			object[] objArray2 = objArray;
			foreach (object obj1 in objArray2)
			{
				_selectedItems.Remove(obj1);
				_deselectedItems.Add(obj1);
			}
		}
		finally
		{
			_availableItemsGrid.EndUpdate();
			_selectedItemsGrid.EndUpdate();
		}
		FireSelectionChanged(array.Concat(objArray).ToArray());
	}

        private void ShiftSelectedItems(SimplifiedGridView sourceView, SimplifiedGridView targetView)
	{
		object[] selectedItems = sourceView.SelectedItems;
		if (selectedItems.Length == 0)
		{
			return;
		}
		BindingList<object> dataSource = (BindingList<object>)sourceView.DataSource;
		BindingList<object> objs = (BindingList<object>)targetView.DataSource;
		sourceView.BeginUpdate();
		targetView.BeginUpdate();
		try
		{
			object[] objArray = selectedItems;
			foreach (object obj in objArray)
			{
				dataSource.Remove(obj);
				objs.Add(obj);
			}
			targetView.SelectedItems = selectedItems;
		}
		finally
		{
			sourceView.EndUpdate();
			targetView.EndUpdate();
		}
		FireSelectionChanged(selectedItems);
	}

        private void UpdateEnabledStates()
	{
		_selectButton.Enabled = _availableItemsGrid.GetSelectedItems<object>().Length != 0;
		_deselectButton.Enabled = _selectedItemsGrid.GetSelectedItems<object>().Length != 0;
	}
    }
}
