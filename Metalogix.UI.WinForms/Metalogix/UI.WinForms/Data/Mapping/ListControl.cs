using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Data.Mapping;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public class ListControl : UserControl
    {
        private bool m_bShowSource;

        private bool m_bAllowEdit;

        private bool m_bMultiSelect;

        private bool m_bAllowNewTagCreation;

        private Action<DataGridViewRow> m_rowAddedAction;

        private Action<DataGridViewRow> m_rowRemovedAction;

        private IListView m_currentView;

        private IListFilter m_currentFilter;

        private object[] m_sources;

        private object[] m_availables;

        private List<DataGridViewRow> m_hidden = new List<DataGridViewRow>();

        private List<DataGridViewRow> m_new = new List<DataGridViewRow>();

        private List<DataGridViewRow> m_filter = new List<DataGridViewRow>();

        private IContainer components;

        private ToolStrip w_toolStrip;

        private ToolStripDropDownButton w_toolStripDropDownButtonViews;

        private ToolStripTextBox w_toolStripTextBoxFilter;

        private ToolStripSeparator toolStripSeparator2;

        private DataGridView w_dataGridView;

        private ToolStripComboBox w_toolStripComboBoxSource;

        private ToolStripLabel w_toolStripLabelSource;

        private ToolStripSplitButton w_toolStripSplitButtonFilter;

        private ContextMenuStrip w_contextMenuStrip;

        private ToolStripDropDownButton w_toolStripDropDownButtonColumns;

        private ToolStripButton w_toolStripButtonFilter;

        private DataGridViewTextBoxColumn Item;

        private DataGridViewTextBoxColumn ItemType;

        public bool AllowEdit
        {
            get
		{
			return m_bAllowEdit;
		}
            set
		{
			m_bAllowEdit = value;
			w_dataGridView.AllowUserToAddRows = m_bAllowEdit;
			w_dataGridView.AllowUserToDeleteRows = m_bAllowEdit;
		}
        }

        public bool AllowNewTagCreation
        {
            get
		{
			return m_bAllowNewTagCreation;
		}
            set
		{
			m_bAllowNewTagCreation = value;
		}
        }

        public IListFilter CurrentFilter
        {
            get
		{
			return m_currentFilter;
		}
            set
		{
			m_currentFilter = value;
		}
        }

        public IListView CurrentView
        {
            get
		{
			return m_currentView;
		}
            set
		{
			m_currentView = value;
		}
        }

        public string[] CustomColumnNames
        {
            get
		{
			List<string> strs = new List<string>();
			if (w_dataGridView.Columns.Count > 2)
			{
				for (int i = 2; i < w_dataGridView.Columns.Count; i++)
				{
					strs.Add(w_dataGridView.Columns[i].Name);
				}
			}
			return strs.ToArray();
		}
        }

        public ListPickerItem FirstItem
        {
            get
		{
			if (w_dataGridView.Rows != null && w_dataGridView.Rows.Count > 0)
			{
				DataGridViewRow item = w_dataGridView.Rows[0];
				if (!item.IsNewRow)
				{
					ListPickerItem value = ((item.Tag == null || !(item.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)item.Tag));
					value.Target = (string)item.Cells["Item"].Value;
					value.TargetType = (string)item.Cells["ItemType"].Value;
					value.Tag = item.Tag;
					if (w_dataGridView.Columns.Count > 2)
					{
						for (int i = 2; i < w_dataGridView.Columns.Count; i++)
						{
							DataGridViewColumn dataGridViewColumn = w_dataGridView.Columns[i];
							if (value.CustomColumns.ContainsKey(dataGridViewColumn.Name))
							{
								value.CustomColumns.Remove(dataGridViewColumn.Name);
							}
							value.CustomColumns.Add(dataGridViewColumn.Name, item.Cells[dataGridViewColumn.Index].Value);
						}
					}
					return value;
				}
			}
			return null;
		}
        }

        public object[] Items
        {
            get
		{
			return m_availables;
		}
            set
		{
			m_availables = value;
			try
			{
				w_dataGridView.SuspendLayout();
				RefreshList();
				RefreshViews();
				RefreshFilters();
				RefreshCustomColumns();
			}
			finally
			{
				w_dataGridView.ResumeLayout();
			}
		}
        }

        public ListPickerItem LastItem
        {
            get
		{
			DataGridViewRow item = null;
			if (w_dataGridView.Rows != null && w_dataGridView.Rows.Count > 0)
			{
				DataGridViewRow i = w_dataGridView.Rows[w_dataGridView.Rows.Count - 1];
				while (i != null && i.IsNewRow)
				{
					item = ((i.Index - 1 <= 0) ? null : w_dataGridView.Rows[i.Index - 1]);
					i = item;
				}
				if (i != null)
				{
					ListPickerItem value = ((i.Tag == null || !(i.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)i.Tag));
					value.Target = (string)i.Cells["Item"].Value;
					value.TargetType = (string)i.Cells["ItemType"].Value;
					value.Tag = i.Tag;
					if (w_dataGridView.Columns.Count > 2)
					{
						for (int j = 2; j < w_dataGridView.Columns.Count; j++)
						{
							DataGridViewColumn dataGridViewColumn = w_dataGridView.Columns[j];
							if (value.CustomColumns.ContainsKey(dataGridViewColumn.Name))
							{
								value.CustomColumns.Remove(dataGridViewColumn.Name);
							}
							value.CustomColumns.Add(dataGridViewColumn.Name, i.Cells[dataGridViewColumn.Index].Value);
						}
					}
					return value;
				}
			}
			return null;
		}
        }

        public DataGridView LocalDataGridView => w_dataGridView;

        public bool MultiSelect
        {
            get
		{
			return m_bMultiSelect;
		}
            set
		{
			m_bMultiSelect = value;
			w_dataGridView.MultiSelect = m_bMultiSelect;
			w_dataGridView.MultiSelect = m_bMultiSelect;
		}
        }

        public List<ListPickerItem> NewRows
        {
            get
		{
			List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
			foreach (DataGridViewRow mNew in m_new)
			{
				ListPickerItem listPickerItem = new ListPickerItem
				{
					Target = (string)mNew.Cells["Item"].Value,
					TargetType = (string)mNew.Cells["ItemType"].Value,
					Tag = mNew.Tag
				};
				listPickerItems.Add(listPickerItem);
			}
			return listPickerItems;
		}
        }

        public object SelectedSource
        {
            get
		{
			if (w_toolStripComboBoxSource.SelectedItem == null)
			{
				return null;
			}
			return w_toolStripComboBoxSource.SelectedItem;
		}
            set
		{
			if (value != null && w_toolStripComboBoxSource.Items != null && w_toolStripComboBoxSource.Items.Contains(value))
			{
				w_toolStripComboBoxSource.SelectedItem = value;
			}
		}
        }

        public bool ShowSource
        {
            get
		{
			return m_bShowSource;
		}
            set
		{
			m_bShowSource = value;
			w_toolStripLabelSource.Visible = m_bShowSource;
			w_toolStripComboBoxSource.Visible = m_bShowSource;
		}
        }

        public object[] Sources
        {
            get
		{
			return m_sources;
		}
            set
		{
			m_sources = value;
			RefreshSources();
		}
        }

        public event SourceChangedEventHandler OnSelectionChanged;

        public event SourceChangedEventHandler OnSourceChanged;

        public ListControl()
	{
		InitializeComponent();
		Initialize();
	}

        public void AddCustomColumn(string columnName, string headerText)
	{
		AddCustomColumn(columnName, headerText, isReadOnly: false);
	}

        public void AddCustomColumn(string columnName, string headerText, bool isReadOnly)
	{
		int index = -1;
		if (w_dataGridView.Columns.Contains(columnName))
		{
			index = w_dataGridView.Columns[columnName].Index;
		}
		else
		{
			index = w_dataGridView.Columns.Add(columnName, headerText);
			if (!w_toolStripDropDownButtonColumns.DropDownItems.ContainsKey(columnName))
			{
				ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)w_toolStripDropDownButtonColumns.DropDownItems.Add(headerText);
				toolStripMenuItem.Name = columnName;
				toolStripMenuItem.CheckOnClick = true;
			}
			RefreshCustomColumns();
		}
		if (index != -1)
		{
			w_dataGridView.Columns[index].ReadOnly = isReadOnly;
		}
	}

        public void AddItem(object oItem)
	{
		object[] objArray = new object[m_availables.Length + 1];
		m_availables.CopyTo(objArray, 0);
		int num = w_dataGridView.Rows.Add();
		DataGridViewRow item = w_dataGridView.Rows[num];
		item.Tag = oItem;
		if (m_currentFilter != null && m_currentFilter.AppliesTo(oItem) && !m_currentFilter.Filter(oItem))
		{
			m_filter.Add(item);
		}
		if (m_currentView.AppliesTo(item.Tag))
		{
			item.Cells["Item"].Value = m_currentView.Render(item.Tag);
			item.Cells["ItemType"].Value = m_currentView.RenderType(item.Tag);
			string[] customColumnNames = CustomColumnNames;
			foreach (string str in customColumnNames)
			{
				if (item.DataGridView.Columns.Contains(str))
				{
					item.Cells[str].Value = m_currentView.RenderColumn(item.Tag, str);
				}
			}
		}
		item.ReadOnly = true;
		DataGridViewRow dataGridViewRow = item;
		bool flag = !m_hidden.Contains(item) && !m_filter.Contains(item) && (string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) || item.Cells["Item"].Value == null || item.Cells["Item"].Value.ToString().IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
		dataGridViewRow.Visible = flag;
		objArray[m_availables.Length] = oItem;
		m_availables = objArray;
	}

        public void AddItem(ListPickerItem item)
	{
		AddItem(item, bReadOnly: false);
	}

        public void AddItem(ListPickerItem item, bool bReadOnly)
	{
		try
		{
			int num = w_dataGridView.Rows.Add();
			DataGridViewRow aliceBlue = w_dataGridView.Rows[num];
			RefreshItem(item, aliceBlue);
			aliceBlue.DefaultCellStyle.BackColor = Color.AliceBlue;
			if (!bReadOnly)
			{
				aliceBlue.ReadOnly = false;
			}
			else
			{
				aliceBlue.ReadOnly = true;
			}
			if (!AllowNewTagCreation)
			{
				aliceBlue.Tag = item;
			}
			else
			{
				aliceBlue.Tag = item.Tag;
			}
			if (m_currentFilter != null && m_currentFilter.AppliesTo(item) && !m_currentFilter.Filter(item))
			{
				m_filter.Add(aliceBlue);
			}
			DataGridViewRow dataGridViewRow = aliceBlue;
			bool flag = !m_hidden.Contains(aliceBlue) && !m_filter.Contains(aliceBlue) && (string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) || aliceBlue.Cells["Item"].Value == null || aliceBlue.Cells["Item"].Value.ToString().IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
			dataGridViewRow.Visible = flag;
			m_new.Add(aliceBlue);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void DeleteItem(ListPickerItem item)
	{
		if (w_dataGridView.Rows == null)
		{
			return;
		}
		DataGridViewRow dataGridViewRow = null;
		foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
		{
			if (row.IsNewRow || row.Tag == null || !row.Tag.Equals(item.Tag))
			{
				continue;
			}
			dataGridViewRow = row;
			break;
		}
		if (dataGridViewRow != null)
		{
			if (w_dataGridView.Rows.Contains(dataGridViewRow))
			{
				w_dataGridView.Rows.Remove(dataGridViewRow);
			}
			if (m_new.Contains(dataGridViewRow))
			{
				m_new.Remove(dataGridViewRow);
			}
			dataGridViewRow.Dispose();
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

        public void Filter(IListFilter filter)
	{
		try
		{
			m_currentFilter = filter;
			w_dataGridView.SuspendLayout();
			int num = 0;
			while (m_filter.Count != num)
			{
				bool flag = true;
				DataGridViewRow item = m_filter[num];
				if (filter.AppliesTo(item.Tag))
				{
					flag = filter.Filter(item.Tag);
				}
				if (flag)
				{
					m_filter.Remove(item);
				}
				item.Visible = flag && !m_hidden.Contains(item);
				if (!flag)
				{
					num++;
				}
			}
			if (w_dataGridView.Rows == null)
			{
				return;
			}
			foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
			{
				if (row.IsNewRow || m_filter.Contains(row))
				{
					continue;
				}
				bool flag1 = true;
				if (filter.AppliesTo(row.Tag))
				{
					flag1 = filter.Filter(row.Tag);
					if (!flag1)
					{
						row.Selected = false;
						m_filter.Add(row);
					}
					row.Visible = flag1 && !m_hidden.Contains(row);
				}
			}
		}
		finally
		{
			w_dataGridView.ResumeLayout();
		}
	}

        public ListPickerItem FindItem(ListPickerItem item)
	{
		return FindItem(item, onlyVisibleItems: false);
	}

        public ListPickerItem FindItem(ListPickerItem item, bool onlyVisibleItems)
	{
		IListPickerComparer listPickerComparer = null;
		foreach (IListPickerComparer listPickerComparer1 in ListCache.ListPickerComparers)
		{
			if (!listPickerComparer1.AppliesTo(item, FirstItem))
			{
				continue;
			}
			listPickerComparer = listPickerComparer1;
			break;
		}
		return FindItem(item, listPickerComparer, onlyVisibleItems);
	}

        public ListPickerItem FindItem(ListPickerItem item, IListPickerComparer comparer)
	{
		return FindItem(item, comparer, onlyVisibleItems: false);
	}

        public ListPickerItem FindItem(ListPickerItem item, IListPickerComparer comparer, bool onlyVisibleItems)
	{
		if (w_dataGridView.Rows != null)
		{
			foreach (DataGridViewRow current in (IEnumerable)w_dataGridView.Rows)
			{
				if (!current.IsNewRow && (!onlyVisibleItems || current.Visible))
				{
					if (comparer == null)
					{
						if (current.Tag != null && item.Tag != null && current.Tag.Equals(item.Tag))
						{
							ListPickerItem value = ((current.Tag == null || !(current.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)current.Tag));
							value.Target = (string)current.Cells["Item"].Value;
							value.TargetType = (string)current.Cells["ItemType"].Value;
							value.Tag = current.Tag;
							if (w_dataGridView.Columns.Count > 2)
							{
								for (int i = 2; i < w_dataGridView.Columns.Count; i++)
								{
									DataGridViewColumn dataGridViewColumn = w_dataGridView.Columns[i];
									if (value.CustomColumns.ContainsKey(dataGridViewColumn.Name))
									{
										value.CustomColumns.Remove(dataGridViewColumn.Name);
									}
									value.CustomColumns.Add(dataGridViewColumn.Name, current.Cells[dataGridViewColumn.Index].Value);
								}
							}
							return value;
						}
					}
					else
					{
						ListPickerItem tag = ((current.Tag == null || !(current.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)current.Tag));
						tag.Target = (string)current.Cells["Item"].Value;
						tag.TargetType = (string)current.Cells["ItemType"].Value;
						tag.Tag = current.Tag;
						if (w_dataGridView.Columns.Count > 2)
						{
							for (int j = 2; j < w_dataGridView.Columns.Count; j++)
							{
								DataGridViewColumn dataGridViewColumn1 = w_dataGridView.Columns[j];
								if (tag.CustomColumns.ContainsKey(dataGridViewColumn1.Name))
								{
									tag.CustomColumns.Remove(dataGridViewColumn1.Name);
								}
								tag.CustomColumns.Add(dataGridViewColumn1.Name, current.Cells[dataGridViewColumn1.Index].Value);
							}
						}
						if (comparer.Compare(item, tag) == 0)
						{
							return tag;
						}
					}
				}
			}
			return null;
		}
		return null;
	}

        public ListPickerItem[] GetItems()
	{
		List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
		if (w_dataGridView.Rows != null)
		{
			foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
			{
				if (row.IsNewRow || !row.Visible)
				{
					continue;
				}
				ListPickerItem value = ((row.Tag == null || !(row.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)row.Tag));
				value.Target = (string)row.Cells["Item"].Value;
				value.TargetType = (string)row.Cells["ItemType"].Value;
				value.Tag = row.Tag;
				if (w_dataGridView.Columns.Count > 2)
				{
					for (int i = 2; i < w_dataGridView.Columns.Count; i++)
					{
						DataGridViewColumn item = w_dataGridView.Columns[i];
						if (value.CustomColumns.ContainsKey(item.Name))
						{
							value.CustomColumns.Remove(item.Name);
						}
						value.CustomColumns.Add(item.Name, row.Cells[item.Index].Value);
					}
				}
				listPickerItems.Add(value);
			}
		}
		return listPickerItems.ToArray();
	}

        public ListPickerItem[] GetNewItems()
	{
		List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
		if (w_dataGridView.Rows != null)
		{
			foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
			{
				if (row.IsNewRow || row.ReadOnly)
				{
					continue;
				}
				ListPickerItem listPickerItem = new ListPickerItem
				{
					Target = (string)row.Cells["Item"].Value,
					TargetType = (string)row.Cells["ItemType"].Value,
					Tag = row.Tag
				};
				if (w_dataGridView.Columns.Count > 2)
				{
					for (int i = 2; i < w_dataGridView.Columns.Count; i++)
					{
						DataGridViewColumn item = w_dataGridView.Columns[i];
						if (listPickerItem.CustomColumns.ContainsKey(item.Name))
						{
							listPickerItem.CustomColumns.Remove(item.Name);
						}
						listPickerItem.CustomColumns.Add(item.Name, row.Cells[item.Index].Value);
					}
				}
				listPickerItems.Add(listPickerItem);
			}
		}
		return listPickerItems.ToArray();
	}

        public ListPickerItem[] GetSelectedItems()
	{
		List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
		if (w_dataGridView.SelectedRows != null)
		{
			foreach (DataGridViewRow selectedRow in w_dataGridView.SelectedRows)
			{
				if (selectedRow.IsNewRow || !selectedRow.Visible)
				{
					continue;
				}
				ListPickerItem value = ((selectedRow.Tag == null || !(selectedRow.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)selectedRow.Tag));
				value.Target = (string)selectedRow.Cells["Item"].Value;
				value.TargetType = (string)selectedRow.Cells["ItemType"].Value;
				value.Tag = selectedRow.Tag;
				if (w_dataGridView.Columns.Count > 2)
				{
					for (int i = 2; i < w_dataGridView.Columns.Count; i++)
					{
						DataGridViewColumn item = w_dataGridView.Columns[i];
						if (value.CustomColumns.ContainsKey(item.Name))
						{
							value.CustomColumns.Remove(item.Name);
						}
						value.CustomColumns.Add(item.Name, selectedRow.Cells[item.Index].Value);
					}
				}
				listPickerItems.Add(value);
			}
		}
		return listPickerItems.ToArray();
	}

        public object GetSelectedSource()
	{
		if (w_toolStripComboBoxSource.SelectedItem == null)
		{
			return null;
		}
		return w_toolStripComboBoxSource.SelectedItem;
	}

        public void HideItem(ListPickerItem item)
	{
		if (item == null || w_dataGridView.Rows.Count <= 0)
		{
			return;
		}
		foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
		{
			if (row.IsNewRow || row.Tag == null || !row.Tag.Equals(item.Tag))
			{
				continue;
			}
			row.Selected = false;
			row.Visible = false;
			if (!m_hidden.Contains(row))
			{
				m_hidden.Add(row);
			}
			return;
		}
		item = FindItem(item, onlyVisibleItems: true);
		if (item == null)
		{
			return;
		}
		foreach (DataGridViewRow current in (IEnumerable)w_dataGridView.Rows)
		{
			if (!current.Tag.Equals(item.Tag))
			{
				continue;
			}
			current.Selected = false;
			current.Visible = false;
			if (!m_hidden.Contains(current))
			{
				m_hidden.Add(current);
			}
			break;
		}
	}

        public void HideSelectedItems()
	{
		if (w_dataGridView.SelectedRows == null)
		{
			return;
		}
		foreach (DataGridViewRow selectedRow in w_dataGridView.SelectedRows)
		{
			if (!selectedRow.IsNewRow)
			{
				selectedRow.Selected = false;
				selectedRow.Visible = false;
				if (!m_hidden.Contains(selectedRow))
				{
					m_hidden.Add(selectedRow);
				}
			}
		}
	}

        private void Initialize()
	{
		ToolStripMenuItem toolStripMenuItem = null;
		toolStripMenuItem = (ToolStripMenuItem)w_toolStripDropDownButtonColumns.DropDownItems.Add("Item");
		toolStripMenuItem.Name = "Item";
		toolStripMenuItem.CheckOnClick = true;
		toolStripMenuItem.PerformClick();
		toolStripMenuItem = (ToolStripMenuItem)w_toolStripDropDownButtonColumns.DropDownItems.Add("Item Type");
		toolStripMenuItem.Name = "ItemType";
		toolStripMenuItem.CheckOnClick = true;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Data.Mapping.ListControl));
		this.w_toolStrip = new System.Windows.Forms.ToolStrip();
		this.w_toolStripDropDownButtonColumns = new System.Windows.Forms.ToolStripDropDownButton();
		this.w_toolStripTextBoxFilter = new System.Windows.Forms.ToolStripTextBox();
		this.w_toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
		this.w_toolStripSplitButtonFilter = new System.Windows.Forms.ToolStripSplitButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.w_toolStripDropDownButtonViews = new System.Windows.Forms.ToolStripDropDownButton();
		this.w_toolStripComboBoxSource = new System.Windows.Forms.ToolStripComboBox();
		this.w_toolStripLabelSource = new System.Windows.Forms.ToolStripLabel();
		this.w_dataGridView = new System.Windows.Forms.DataGridView();
		this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ItemType = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.w_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.w_toolStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_dataGridView).BeginInit();
		base.SuspendLayout();
		this.w_toolStrip.BackColor = System.Drawing.SystemColors.Control;
		this.w_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.w_toolStrip.Items;
		System.Windows.Forms.ToolStripItem[] wToolStripDropDownButtonColumns = new System.Windows.Forms.ToolStripItem[8] { this.w_toolStripDropDownButtonColumns, this.w_toolStripTextBoxFilter, this.w_toolStripButtonFilter, this.w_toolStripSplitButtonFilter, this.toolStripSeparator2, this.w_toolStripDropDownButtonViews, this.w_toolStripComboBoxSource, this.w_toolStripLabelSource };
		items.AddRange(wToolStripDropDownButtonColumns);
		componentResourceManager.ApplyResources(this.w_toolStrip, "w_toolStrip");
		this.w_toolStrip.Name = "w_toolStrip";
		this.w_toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.w_toolStripDropDownButtonColumns.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		componentResourceManager.ApplyResources(this.w_toolStripDropDownButtonColumns, "w_toolStripDropDownButtonColumns");
		this.w_toolStripDropDownButtonColumns.Name = "w_toolStripDropDownButtonColumns";
		this.w_toolStripTextBoxFilter.Name = "w_toolStripTextBoxFilter";
		componentResourceManager.ApplyResources(this.w_toolStripTextBoxFilter, "w_toolStripTextBoxFilter");
		this.w_toolStripTextBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(w_toolStripTextBoxFilter_KeyDown);
		this.w_toolStripButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_toolStripButtonFilter.Image = Metalogix.UI.WinForms.Properties.Resources.Filter16;
		componentResourceManager.ApplyResources(this.w_toolStripButtonFilter, "w_toolStripButtonFilter");
		this.w_toolStripButtonFilter.Name = "w_toolStripButtonFilter";
		this.w_toolStripButtonFilter.Click += new System.EventHandler(w_toolStripSplitButtonFilter_ButtonClick);
		this.w_toolStripSplitButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_toolStripSplitButtonFilter.Image = Metalogix.UI.WinForms.Properties.Resources.Filter16;
		componentResourceManager.ApplyResources(this.w_toolStripSplitButtonFilter, "w_toolStripSplitButtonFilter");
		this.w_toolStripSplitButtonFilter.Name = "w_toolStripSplitButtonFilter";
		this.w_toolStripSplitButtonFilter.ButtonClick += new System.EventHandler(w_toolStripSplitButtonFilter_ButtonClick);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		componentResourceManager.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		this.w_toolStripDropDownButtonViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		componentResourceManager.ApplyResources(this.w_toolStripDropDownButtonViews, "w_toolStripDropDownButtonViews");
		this.w_toolStripDropDownButtonViews.Name = "w_toolStripDropDownButtonViews";
		this.w_toolStripComboBoxSource.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.w_toolStripComboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.w_toolStripComboBoxSource.Name = "w_toolStripComboBoxSource";
		componentResourceManager.ApplyResources(this.w_toolStripComboBoxSource, "w_toolStripComboBoxSource");
		this.w_toolStripComboBoxSource.SelectedIndexChanged += new System.EventHandler(w_toolStripComboBoxSource_SelectedIndexChanged);
		this.w_toolStripLabelSource.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.w_toolStripLabelSource.Name = "w_toolStripLabelSource";
		componentResourceManager.ApplyResources(this.w_toolStripLabelSource, "w_toolStripLabelSource");
		this.w_dataGridView.AllowUserToResizeColumns = false;
		this.w_dataGridView.AllowUserToResizeRows = false;
		this.w_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
		this.w_dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
		this.w_dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.w_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		System.Windows.Forms.DataGridViewColumnCollection columns = this.w_dataGridView.Columns;
		System.Windows.Forms.DataGridViewColumn[] item = new System.Windows.Forms.DataGridViewColumn[2] { this.Item, this.ItemType };
		columns.AddRange(item);
		this.w_dataGridView.ContextMenuStrip = this.w_contextMenuStrip;
		componentResourceManager.ApplyResources(this.w_dataGridView, "w_dataGridView");
		this.w_dataGridView.Name = "w_dataGridView";
		this.w_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
		this.w_dataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		this.w_dataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		this.w_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.w_dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(w_dataGridView_RowsAdded);
		this.w_dataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(w_dataGridView_RowsRemoved);
		this.w_dataGridView.SelectionChanged += new System.EventHandler(w_dataGridView_SelectionChanged);
		this.Item.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		componentResourceManager.ApplyResources(this.Item, "Item");
		this.Item.Name = "Item";
		this.ItemType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		componentResourceManager.ApplyResources(this.ItemType, "ItemType");
		this.ItemType.Name = "ItemType";
		this.ItemType.ReadOnly = true;
		this.ItemType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.w_contextMenuStrip.Name = "w_contextMenuStrip";
		componentResourceManager.ApplyResources(this.w_contextMenuStrip, "w_contextMenuStrip");
		this.w_contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(w_contextMenuStrip_Opening);
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		base.Controls.Add(this.w_dataGridView);
		base.Controls.Add(this.w_toolStrip);
		this.ForeColor = System.Drawing.SystemColors.ControlText;
		base.Name = "ListControl";
		this.w_toolStrip.ResumeLayout(false);
		this.w_toolStrip.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.w_dataGridView).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (msg.Msg != 256 || keyData != Keys.Return || !w_toolStripTextBoxFilter.Focused)
		{
			return base.ProcessCmdKey(ref msg, keyData);
		}
		w_toolStripButtonFilter.PerformClick();
		w_toolStripSplitButtonFilter.PerformButtonClick();
		return true;
	}

        private void RefreshCustomColumns()
	{
		if (w_dataGridView.Rows == null || w_dataGridView.Columns.Count <= 2)
		{
			return;
		}
		for (int i = 2; i < w_dataGridView.Columns.Count; i++)
		{
			DataGridViewColumn item = w_dataGridView.Columns[i];
			foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
			{
				if (!row.IsNewRow)
				{
					DataGridViewCell dataGridViewCell = row.Cells[item.Index];
					object value = ((m_currentView != null && row.Tag != null && m_currentView.AppliesTo(row.Tag)) ? m_currentView.RenderColumn(row.Tag, item.Name) : row.Cells[item.Index].Value);
					dataGridViewCell.Value = value;
				}
			}
		}
	}

        private void RefreshFilters()
	{
		try
		{
			w_toolStripSplitButtonFilter.DropDownItems.Clear();
			foreach (IListFilter listFilter in ListCache.ListFilters)
			{
				if (w_dataGridView.Rows == null || w_dataGridView.Rows.Count <= 0 || !listFilter.AppliesTo(w_dataGridView.Rows[0].Tag))
				{
					continue;
				}
				ToolStripItem filter = w_toolStripSplitButtonFilter.DropDownItems.Add(listFilter.Name);
				filter.Tag = listFilter;
				filter.Image = Resources.Filter;
				filter.Click += delegate(object sender, EventArgs e)
				{
					if (sender != null)
					{
						ToolStripItem toolStripItem = sender as ToolStripItem;
						if (toolStripItem.Tag != null && toolStripItem.Tag is IListFilter)
						{
							IListFilter listFilter2 = toolStripItem.Tag as IListFilter;
							foreach (DataGridViewRow dataGridViewRow in (IEnumerable)w_dataGridView.Rows)
							{
								if (!dataGridViewRow.IsNewRow && listFilter2.AppliesTo(dataGridViewRow.Tag))
								{
									dataGridViewRow.Visible = listFilter2.Filter(dataGridViewRow.Tag) && !m_hidden.Contains(dataGridViewRow) && !m_filter.Contains(dataGridViewRow);
									if (!dataGridViewRow.Visible)
									{
										dataGridViewRow.Selected = false;
									}
								}
							}
						}
					}
				};
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			w_toolStripSplitButtonFilter.Visible = w_toolStripSplitButtonFilter.HasDropDownItems;
			w_toolStripButtonFilter.Visible = !w_toolStripSplitButtonFilter.HasDropDownItems;
		}
	}

        private void RefreshItem(ListPickerItem item, DataGridViewRow row)
	{
		if (row == null || item == null)
		{
			return;
		}
		row.Tag = item;
		row.Cells["Item"].Value = ((m_currentView == null || item.Tag == null || !m_currentView.AppliesTo(item.Tag)) ? item.Target : m_currentView.Render(item.Tag));
		row.Cells["ItemType"].Value = ((m_currentView == null || item.Tag == null || !m_currentView.AppliesTo(item.Tag)) ? item.TargetType : m_currentView.RenderType(item.Tag));
		if (w_dataGridView.Columns.Count > 2)
		{
			for (int i = 2; i < w_dataGridView.Columns.Count; i++)
			{
				DataGridViewColumn dataGridViewColumn = w_dataGridView.Columns[i];
				DataGridViewCell dataGridViewCell = row.Cells[dataGridViewColumn.Index];
				object value = ((!item.CustomColumns.ContainsKey(dataGridViewColumn.Name)) ? ((m_currentView != null && item.Tag != null) ? m_currentView.RenderColumn(item.Tag, dataGridViewColumn.Name) : row.Cells[dataGridViewColumn.Index].Value) : item.CustomColumns[dataGridViewColumn.Name]);
				dataGridViewCell.Value = value;
			}
		}
	}

        private void RefreshList()
	{
		try
		{
			if (m_availables != null)
			{
				List<object> objs = new List<object>(m_availables);
				List<DataGridViewRow> dataGridViewRows = new List<DataGridViewRow>();
				if (w_dataGridView.Rows != null)
				{
					List<DataGridViewRow> dataGridViewRows1 = new List<DataGridViewRow>();
					foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
					{
						if (!row.IsNewRow)
						{
							if (objs.Contains(row.Tag) || m_new.Contains(row))
							{
								objs.Remove(row.Tag);
							}
							else
							{
								dataGridViewRows1.Add(row);
							}
						}
					}
					if (dataGridViewRows1.Count != w_dataGridView.Rows.Count)
					{
						foreach (DataGridViewRow dataGridViewRow in dataGridViewRows1)
						{
							w_dataGridView.Rows.Remove(dataGridViewRow);
						}
					}
					else
					{
						w_dataGridView.Rows.Clear();
					}
					foreach (DataGridViewRow mHidden in m_hidden)
					{
						if (objs.Contains(mHidden.Tag) && !w_dataGridView.Rows.Contains(mHidden))
						{
							dataGridViewRows.Add(mHidden);
							objs.Remove(mHidden.Tag);
						}
					}
				}
				foreach (object obj in objs)
				{
					if (obj != null)
					{
						DataGridViewRow dataGridViewRow1 = new DataGridViewRow
						{
							Tag = obj,
							ReadOnly = true
						};
						dataGridViewRows.Add(dataGridViewRow1);
					}
				}
				w_dataGridView.Rows.AddRange(dataGridViewRows.ToArray());
			}
			else
			{
				w_dataGridView.Rows.Clear();
				m_hidden.Clear();
				m_filter.Clear();
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void RefreshSources()
	{
		w_toolStripComboBoxSource.Items.Clear();
		if (m_sources != null)
		{
			object[] mSources = m_sources;
			foreach (object obj in mSources)
			{
				w_toolStripComboBoxSource.Items.Add(obj);
				string str = ((obj == null) ? "" : obj.ToString());
				ToolStripComboBox wToolStripComboBoxSource = w_toolStripComboBoxSource;
				int dropDownWidth = w_toolStripComboBoxSource.DropDownWidth;
				wToolStripComboBoxSource.DropDownWidth = Math.Max(dropDownWidth, TextRenderer.MeasureText(str, w_toolStripComboBoxSource.Font).Width);
			}
		}
	}

        private void RefreshViews()
	{
		try
		{
			w_toolStripDropDownButtonViews.DropDownItems.Clear();
			foreach (IListView listView in ListCache.ListViews)
			{
				if (w_dataGridView.Rows == null || w_dataGridView.Rows.Count <= 0)
				{
					continue;
				}
				DataGridViewRow item = w_dataGridView.Rows[0];
				foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
				{
					if (row.Tag is ListPickerItem)
					{
						continue;
					}
					item = row;
					break;
				}
				if (!listView.AppliesTo(item.Tag))
				{
					continue;
				}
				ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)w_toolStripDropDownButtonViews.DropDownItems.Add(listView.Name);
				toolStripMenuItem.Tag = listView;
				toolStripMenuItem.Click += delegate(object sender, EventArgs e)
				{
					if (sender != null)
					{
						ToolStripItem toolStripItem = sender as ToolStripItem;
						if (toolStripItem.Tag != null && toolStripItem.Tag is IListView)
						{
							RenderView(toolStripItem.Tag as IListView);
						}
					}
				};
				if (listView != null && listView.GetType().GetCustomAttributes(typeof(DefaultAttribute), inherit: true).Length != 0)
				{
					toolStripMenuItem.PerformClick();
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			w_toolStripDropDownButtonViews.Visible = w_toolStripDropDownButtonViews.HasDropDownItems && w_toolStripDropDownButtonViews.DropDownItems.Count > 1;
		}
	}

        public void RegisterRowAddedHandler(Action<DataGridViewRow> action)
	{
		m_rowAddedAction = action;
	}

        public void RegisterRowRemovedHandler(Action<DataGridViewRow> action)
	{
		m_rowRemovedAction = action;
	}

        public void RemoveCustomColumn(string columnName)
	{
		if (w_dataGridView.Columns.Contains(columnName))
		{
			w_dataGridView.Columns.Remove(columnName);
			if (w_toolStripDropDownButtonColumns.DropDownItems.ContainsKey(columnName))
			{
				w_toolStripDropDownButtonColumns.DropDownItems.RemoveByKey(columnName);
			}
		}
	}

        public void RenderView(IListView view)
	{
		m_currentView = view;
		DataGridViewRow[] dataGridViewRowArray = new DataGridViewRow[w_dataGridView.Rows.Count];
		w_dataGridView.Rows.CopyTo(dataGridViewRowArray, 0);
		w_dataGridView.Rows.Clear();
		Queue<DataGridViewRow> dataGridViewRows = new Queue<DataGridViewRow>(dataGridViewRowArray);
		while (dataGridViewRows.Count != 0)
		{
			DataGridViewRow dataGridViewRow = dataGridViewRows.Dequeue();
			if (dataGridViewRow.IsNewRow || !m_currentView.AppliesTo(dataGridViewRow.Tag))
			{
				continue;
			}
			dataGridViewRow.CreateCells(w_dataGridView);
			dataGridViewRow.Cells[w_dataGridView.Columns.IndexOf(w_dataGridView.Columns["Item"])].Value = m_currentView.Render(dataGridViewRow.Tag);
			dataGridViewRow.Cells[w_dataGridView.Columns.IndexOf(w_dataGridView.Columns["ItemType"])].Value = m_currentView.RenderType(dataGridViewRow.Tag);
			string[] customColumnNames = CustomColumnNames;
			foreach (string str in customColumnNames)
			{
				if (w_dataGridView.Columns.Contains(str))
				{
					dataGridViewRow.Cells[w_dataGridView.Columns.IndexOf(w_dataGridView.Columns[str])].Value = m_currentView.RenderColumn(dataGridViewRow.Tag, str);
				}
			}
		}
		w_dataGridView.Rows.AddRange(dataGridViewRowArray);
		UpdateViewDropDown();
	}

        public void SelectItem(ListPickerItem item)
	{
		SelectItem(item, accumulate: false);
	}

        public void SelectItem(ListPickerItem item, bool accumulate)
	{
		if (!accumulate && w_dataGridView.SelectedRows != null)
		{
			foreach (DataGridViewRow selectedRow in w_dataGridView.SelectedRows)
			{
				selectedRow.Selected = false;
			}
		}
		if (item == null || w_dataGridView.Rows == null)
		{
			return;
		}
		foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
		{
			if (row.IsNewRow || row.Tag == null || !row.Tag.Equals(item.Tag))
			{
				continue;
			}
			if (row.Visible)
			{
				row.Selected = true;
				w_dataGridView.FirstDisplayedScrollingRowIndex = row.Index;
			}
			break;
		}
	}

        public void ShowItem(ListPickerItem item)
	{
		if (w_dataGridView.Rows.Count <= 0)
		{
			return;
		}
		foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
		{
			if (row.IsNewRow || row.Tag == null || !row.Tag.Equals(item.Tag))
			{
				continue;
			}
			row.Visible = !m_filter.Contains(row);
			if (m_hidden.Contains(row))
			{
				m_hidden.Remove(row);
			}
			return;
		}
		item = FindItem(item, onlyVisibleItems: false);
		if (item == null)
		{
			return;
		}
		foreach (DataGridViewRow current in (IEnumerable)w_dataGridView.Rows)
		{
			if (!current.Tag.Equals(item.Tag))
			{
				continue;
			}
			current.Visible = !m_filter.Contains(current);
			if (m_hidden.Contains(current))
			{
				m_hidden.Remove(current);
			}
			break;
		}
	}

        public void ShowItems(IEnumerable<ListPickerItem> items)
	{
		try
		{
			w_dataGridView.SuspendLayout();
			foreach (ListPickerItem item in items)
			{
				ShowItem(item);
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			w_dataGridView.ResumeLayout();
		}
	}

        public void UpdateItem(ListPickerItem item)
	{
		if (w_dataGridView.Rows == null)
		{
			return;
		}
		foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
		{
			if (row.IsNewRow)
			{
				continue;
			}
			object obj = row.Tag;
			object tag = ((item.Tag != null) ? item.Tag : item);
			if (!obj.Equals(tag))
			{
				continue;
			}
			RefreshItem(item, row);
			if (!AllowNewTagCreation)
			{
				row.Tag = item;
			}
			else
			{
				row.Tag = item.Tag;
			}
			break;
		}
	}

        private void UpdateViewDropDown()
	{
		foreach (ToolStripMenuItem dropDownItem in w_toolStripDropDownButtonViews.DropDownItems)
		{
			if (m_currentView == null || !(m_currentView.Name == dropDownItem.Text))
			{
				dropDownItem.Checked = false;
			}
			else
			{
				dropDownItem.Checked = true;
			}
		}
	}

        private void w_contextMenuStrip_Opening(object sender, CancelEventArgs e)
	{
		e.Cancel = w_dataGridView.SelectedRows == null || w_dataGridView.SelectedRows.Count == 0;
		if (!e.Cancel)
		{
			w_contextMenuStrip.Items.Clear();
			foreach (IListPickerAction listPickerAction in ListCache.ListPickerActions)
			{
				try
				{
					bool flag = true;
					ListPickerItem[] selectedItems = GetSelectedItems();
					ListPickerItem[] listPickerItemArray1 = selectedItems;
					for (int num = 0; num < listPickerItemArray1.Length; num++)
					{
						if (!listPickerAction.AppliesTo(listPickerItemArray1[num]))
						{
							flag = false;
						}
					}
					if (!flag)
					{
						continue;
					}
					ToolStripMenuItem toolStripMenuItem = null;
					string name = listPickerAction.Name;
					char[] chrArray = new char[1] { '>' };
					string[] strArrays = name.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < strArrays.Length; j++)
					{
						string str = strArrays[j].Trim();
						toolStripMenuItem = ((toolStripMenuItem != null) ? ((ToolStripMenuItem)(toolStripMenuItem.DropDownItems.ContainsKey(str) ? toolStripMenuItem.DropDownItems[str] : toolStripMenuItem.DropDownItems.Add(str))) : ((ToolStripMenuItem)(w_contextMenuStrip.Items.ContainsKey(str) ? w_contextMenuStrip.Items[str] : w_contextMenuStrip.Items.Add(str))));
						toolStripMenuItem.Name = str;
					}
					toolStripMenuItem.Tag = listPickerAction;
					toolStripMenuItem.Click += delegate
					{
						IListPickerAction listPickerAction2 = (IListPickerAction)toolStripMenuItem.Tag;
						ListPickerItem[] array = selectedItems;
						foreach (ListPickerItem item in array)
						{
							try
							{
								listPickerAction2.RunAction(item);
							}
							catch (Exception exc)
							{
								GlobalServices.ErrorHandler.HandleException(exc);
							}
							finally
							{
								UpdateItem(item);
							}
						}
					};
				}
				catch (Exception)
				{
				}
			}
		}
		e.Cancel = w_contextMenuStrip.Items.Count == 0;
	}

        private void w_dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
	{
		if (m_rowAddedAction != null && e.RowIndex != -1)
		{
			for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
			{
				DataGridViewRow item = w_dataGridView.Rows[i];
				m_rowAddedAction(item);
			}
		}
	}

        private void w_dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
	{
		if (m_rowRemovedAction != null && e.RowIndex != -1)
		{
			for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
			{
				DataGridViewRow item = w_dataGridView.Rows[i];
				m_rowRemovedAction(item);
			}
		}
	}

        private void w_dataGridView_SelectionChanged(object sender, EventArgs e)
	{
		if (this.OnSelectionChanged != null)
		{
			object tag = null;
			if (w_dataGridView.SelectedRows.Count > 0)
			{
				tag = w_dataGridView.SelectedRows[0].Tag;
			}
			this.OnSelectionChanged(this, new SourceChangedEventArgs(tag));
		}
	}

        private void w_toolStripComboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (this.OnSourceChanged != null)
		{
			SourceChangedEventArgs sourceChangedEventArg = new SourceChangedEventArgs(w_toolStripComboBoxSource.SelectedItem);
			this.OnSourceChanged(this, sourceChangedEventArg);
		}
	}

        private void w_toolStripSplitButtonFilter_ButtonClick(object sender, EventArgs e)
	{
		try
		{
			foreach (DataGridViewRow row in (IEnumerable)w_dataGridView.Rows)
			{
				bool flag1 = false;
				foreach (ToolStripMenuItem current in w_toolStripDropDownButtonColumns.DropDownItems)
				{
					if (current.Checked)
					{
						flag1 = row.Cells[current.Name].Value == null || row.Cells[current.Name].Value.ToString().IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
						if (flag1)
						{
							break;
						}
					}
				}
				DataGridViewRow dataGridViewRow = row;
				bool flag = !m_hidden.Contains(row) && !m_filter.Contains(row) && (string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) || flag1);
				dataGridViewRow.Visible = flag;
				if (!row.Visible)
				{
					row.Selected = false;
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        private void w_toolStripTextBoxFilter_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			w_toolStripSplitButtonFilter.PerformClick();
			e.Handled = true;
		}
		if (e.Control && e.KeyCode == Keys.A)
		{
			w_toolStripTextBoxFilter.SelectAll();
			e.Handled = true;
		}
	}
    }
}
