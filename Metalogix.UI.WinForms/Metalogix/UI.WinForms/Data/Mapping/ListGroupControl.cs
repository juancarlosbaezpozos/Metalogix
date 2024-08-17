using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Data.Mapping;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public class ListGroupControl : UserControl
    {
        private class ListPickerViewItem : ListViewItem
        {
            private ListView m_owner;

            public ListPickerViewItem(ListView owner, ListPickerItem item)
		{
			m_owner = owner;
			base.Tag = item;
			RenderView();
		}

            public ListPickerViewItem(ListView owner, ListPickerItem item, ListViewGroup group)
                : base(group)
		{
			m_owner = owner;
			base.Tag = item;
			RenderView();
		}

            public void RenderView()
		{
			RenderView(null);
		}

            public void RenderView(IListView view)
		{
			ListPickerItem tag = (ListPickerItem)base.Tag;
			base.SubItems.Clear();
			ListViewSubItem name = null;
			if (view == null || !view.AppliesTo(tag.Tag))
			{
				base.Text = tag.Target;
				base.SubItems[0].Name = "Item";
				name = base.SubItems.Add(tag.TargetType);
				name.Name = "ItemType";
				if (m_owner.Columns.Count > 2)
				{
					int num = 2;
					if (m_owner.Columns[2].Text.Equals("Group"))
					{
						name = base.SubItems.Add((base.Group != null) ? base.Group.Header : "");
						name.Name = m_owner.Columns[2].Name;
						num++;
					}
					for (int i = num; i < m_owner.Columns.Count; i++)
					{
						name = base.SubItems.Add(tag.CustomColumns.ContainsKey(m_owner.Columns[i].Name) ? ((string)tag.CustomColumns[m_owner.Columns[i].Name]) : "");
						name.Name = m_owner.Columns[i].Name;
					}
				}
				return;
			}
			base.Text = view.Render(tag.Tag);
			base.SubItems[0].Name = "Item";
			name = base.SubItems.Add(view.RenderType(tag.Tag));
			name.Name = "ItemType";
			if (m_owner.Columns.Count <= 2)
			{
				return;
			}
			int num1 = 2;
			if (m_owner.Columns[2].Text.Equals("Group"))
			{
				string str = view.RenderGroup(tag.Tag);
				if (string.Equals(str, "default", StringComparison.InvariantCultureIgnoreCase))
				{
					str = ((tag.Group != null) ? tag.Group : str);
				}
				name = base.SubItems.Add(str);
				name.Name = base.ListView.Columns[2].Name;
				num1++;
			}
			for (int j = num1; j < m_owner.Columns.Count; j++)
			{
				name = base.SubItems.Add(view.RenderColumn(tag.Tag, base.ListView.Columns[j].Name));
				name.Name = base.ListView.Columns[j].Name;
			}
		}
        }

        private bool m_bShowSource;

        private bool m_bMultiSelect;

        private object[] m_availables;

        private object[] m_sources;

        private IListView m_currentView;

        private IListFilter m_currentFilter;

        private List<ListPickerItem> m_hiddenViewItems = new List<ListPickerItem>();

        private List<ListPickerItem> m_filter = new List<ListPickerItem>();

        private List<ListPickerItem> m_tempFilter = new List<ListPickerItem>();

        private ListViewColumnSorter m_sorter;

        private IContainer components;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripTextBox w_toolStripTextBoxFilter;

        private ListView w_listView;

        private ColumnHeader Item;

        private ColumnHeader ItemType;

        private ToolStripSplitButton w_toolStripSplitButtonFilter;

        private ToolStripDropDownButton w_toolStripDropDownButtonViews;

        private ToolStrip w_toolStrip;

        private ToolStripLabel w_toolStripLabelSource;

        private ToolStripComboBox w_toolStripComboBoxSource;

        private ContextMenuStrip w_contextMenuStrip;

        private ToolStripDropDownButton w_toolStripDropDownButtonColumns;

        private ToolStripButton w_toolStripButtonFilter;

        public string[] CustomColumnNames
        {
            get
		{
			List<string> strs = new List<string>();
			if (w_listView.Columns.Count > 2)
			{
				int num = 2;
				if (w_listView.Columns[2].Text.Equals("Group"))
				{
					num++;
				}
				for (int i = num; i < w_listView.Columns.Count; i++)
				{
					strs.Add(w_listView.Columns[i].Name);
				}
			}
			return strs.ToArray();
		}
        }

        public ListPickerItem FirstItem
        {
            get
		{
			if (w_listView.Items == null || w_listView.Items.Count <= 0)
			{
				return null;
			}
			ListPickerViewItem item = w_listView.Items[0] as ListPickerViewItem;
			ListPickerItem text = ((item.Tag == null || !(item.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)item.Tag));
			text.Target = item.SubItems[0].Text;
			text.TargetType = item.SubItems[1].Text;
			if (w_listView.Columns.Count > 2)
			{
				int num = 2;
				if (w_listView.Columns[2].Text.Equals("Group"))
				{
					num++;
				}
				for (int i = num; i < w_listView.Columns.Count; i++)
				{
					ColumnHeader columnHeader = w_listView.Columns[i];
					if (text.CustomColumns.ContainsKey(columnHeader.Name))
					{
						text.CustomColumns.Remove(columnHeader.Name);
					}
					text.CustomColumns.Add(columnHeader.Name, item.SubItems[columnHeader.Index].Text);
				}
			}
			text.Group = ((item.Group != null) ? item.Group.Name : item.SubItems["Group"].Text);
			return text;
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
				w_listView.BeginUpdate();
				RefreshList();
				RefreshViews();
				RefreshFilters();
			}
			finally
			{
				w_listView.EndUpdate();
			}
		}
        }

        public bool MultiSelect
        {
            get
		{
			return m_bMultiSelect;
		}
            set
		{
			m_bMultiSelect = value;
			w_listView.MultiSelect = m_bMultiSelect;
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

        public ListGroupControl()
	{
		InitializeComponent();
		Initialize();
	}

        public void AddCustomColumn(string columnName, string headerText)
	{
		if (!w_listView.Columns.ContainsKey(columnName))
		{
			w_listView.Columns.Add(columnName, headerText);
			if (!w_toolStripDropDownButtonColumns.DropDownItems.ContainsKey(columnName))
			{
				ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)w_toolStripDropDownButtonColumns.DropDownItems.Add(headerText);
				toolStripMenuItem.Name = columnName;
				toolStripMenuItem.CheckOnClick = true;
			}
			RenderView(m_currentView);
		}
	}

        public void AddItem(ListPickerItem item)
	{
		try
		{
			ListViewGroup listViewGroup = null;
			if (string.IsNullOrEmpty(item.Group) && listViewGroup == null)
			{
				IListGrouper listGrouper = null;
				foreach (IListGrouper listGrouper1 in ListCache.ListGroupers)
				{
					if (listGrouper1.AppliesTo(item.Tag))
					{
						listGrouper = listGrouper1;
					}
				}
				item.Group = ((listGrouper == null) ? "default" : listGrouper.Group(item.Tag));
			}
			if (!string.IsNullOrEmpty(item.Group))
			{
				foreach (ListViewGroup group in w_listView.Groups)
				{
					if (!group.Name.Equals(item.Group, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					listViewGroup = group;
					break;
				}
				if (listViewGroup == null)
				{
					listViewGroup = new ListViewGroup(item.Group, item.Group);
					w_listView.Groups.Add(listViewGroup);
				}
			}
			((ListPickerViewItem)w_listView.Items.Add((listViewGroup == null) ? new ListPickerViewItem(w_listView, item) : new ListPickerViewItem(w_listView, item, listViewGroup))).RenderView(m_currentView);
			if (m_currentFilter != null && m_currentFilter.AppliesTo(item.Tag) && !m_currentFilter.Filter(item.Tag))
			{
				HideItem(item);
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void AddItems(IEnumerable<ListPickerItem> items)
	{
		try
		{
			if (items == null)
			{
				return;
			}
			foreach (ListPickerItem item in items)
			{
				AddItem(item);
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void ClearItems()
	{
		w_listView.Items.Clear();
		m_hiddenViewItems.Clear();
	}

        public void DeleteItem(ListPickerItem item)
	{
		try
		{
			if (w_listView.Items == null)
			{
				return;
			}
			ListPickerViewItem listPickerViewItem = null;
			foreach (ListPickerViewItem listPickerViewItem1 in w_listView.Items)
			{
				if (!item.Equals(listPickerViewItem1.Tag))
				{
					continue;
				}
				listPickerViewItem = listPickerViewItem1;
				break;
			}
			if (listPickerViewItem == null)
			{
				ListPickerItem listPickerItem = null;
				foreach (ListPickerItem mHiddenViewItem in m_hiddenViewItems)
				{
					if (!item.Equals(mHiddenViewItem))
					{
						continue;
					}
					listPickerItem = mHiddenViewItem;
					break;
				}
				if (listPickerItem != null)
				{
					m_hiddenViewItems.Remove(listPickerItem);
				}
			}
			else
			{
				listPickerViewItem.Remove();
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
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
			if (m_currentFilter == null)
			{
				return;
			}
			w_listView.BeginUpdate();
			foreach (ListPickerItem mHiddenViewItem in m_hiddenViewItems)
			{
				if (m_currentFilter.AppliesTo(mHiddenViewItem.Tag) && m_currentFilter.Filter(mHiddenViewItem.Tag))
				{
					AddItem(mHiddenViewItem);
				}
			}
			foreach (ListPickerViewItem item in w_listView.Items)
			{
				if (m_currentFilter.AppliesTo(item.Tag) && !m_currentFilter.Filter(item.Tag) && !m_hiddenViewItems.Contains((ListPickerItem)item.Tag))
				{
					m_hiddenViewItems.Add((ListPickerItem)item.Tag);
				}
			}
			foreach (ListPickerItem listPickerItem in m_hiddenViewItems)
			{
				HideItem(listPickerItem);
			}
			foreach (ListPickerViewItem listPickerViewItem in w_listView.Items)
			{
				if (m_hiddenViewItems.Contains((ListPickerItem)listPickerViewItem.Tag))
				{
					m_hiddenViewItems.Remove((ListPickerItem)listPickerViewItem.Tag);
				}
			}
		}
		finally
		{
			w_listView.EndUpdate();
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
			if (listPickerComparer1.AppliesTo(item, FirstItem))
			{
				listPickerComparer = listPickerComparer1;
			}
		}
		return FindItem(item, listPickerComparer, onlyVisibleItems);
	}

        public ListPickerItem FindItem(ListPickerItem item, IListPickerComparer comparer)
	{
		return FindItem(item, comparer, onlyVisibleItems: false);
	}

        public ListPickerItem FindItem(ListPickerItem item, IListPickerComparer comparer, bool onlyVisibleItems)
	{
		if (!onlyVisibleItems && m_hiddenViewItems != null)
		{
			foreach (ListPickerItem mHiddenViewItem in m_hiddenViewItems)
			{
				if (comparer == null)
				{
					ListPickerItem listPickerItem1 = ((mHiddenViewItem.Tag == null || !(mHiddenViewItem.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)mHiddenViewItem.Tag));
					if (listPickerItem1.Equals(item) || (listPickerItem1.Tag != null && item.Tag != null && listPickerItem1.Tag.Equals(item.Tag)))
					{
						return mHiddenViewItem;
					}
				}
				else if (comparer.Compare(item, mHiddenViewItem) == 0)
				{
					return mHiddenViewItem;
				}
			}
		}
		if (w_listView.Items != null)
		{
			foreach (ListPickerViewItem current in w_listView.Items)
			{
				if (comparer == null)
				{
					if (current.Tag != null && item.Tag != null && current.Tag.Equals(item.Tag))
					{
						ListPickerItem text = ((current.Tag == null || !(current.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)current.Tag));
						text.Target = current.SubItems[0].Text;
						text.TargetType = current.SubItems[1].Text;
						text.Tag = text.Tag;
						if (w_listView.Columns.Count > 2)
						{
							int num = 2;
							if (w_listView.Columns[2].Text.Equals("Group"))
							{
								num++;
							}
							for (int i = num; i < w_listView.Columns.Count; i++)
							{
								ColumnHeader columnHeader = w_listView.Columns[i];
								if (text.CustomColumns.ContainsKey(columnHeader.Name))
								{
									text.CustomColumns.Remove(columnHeader.Name);
								}
								text.CustomColumns.Add(columnHeader.Name, current.SubItems[columnHeader.Index].Text);
							}
						}
						text.Group = ((current.Group != null) ? current.Group.Name : current.SubItems["Group"].Text);
						return text;
					}
				}
				else
				{
					ListPickerItem tag = ((current.Tag == null || !(current.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)current.Tag));
					tag.Target = current.SubItems[0].Text;
					tag.TargetType = current.SubItems[1].Text;
					tag.Tag = tag.Tag;
					if (w_listView.Columns.Count > 2)
					{
						int num1 = 2;
						if (w_listView.Columns[2].Text.Equals("Group"))
						{
							num1++;
						}
						for (int j = num1; j < w_listView.Columns.Count; j++)
						{
							ColumnHeader columnHeader1 = w_listView.Columns[j];
							if (tag.CustomColumns.ContainsKey(columnHeader1.Name))
							{
								tag.CustomColumns.Remove(columnHeader1.Name);
							}
							tag.CustomColumns.Add(columnHeader1.Name, current.SubItems[columnHeader1.Index].Text);
						}
					}
					tag.Group = ((current.Group != null) ? current.Group.Name : current.SubItems["Group"].Text);
					if (comparer.Compare(item, tag) == 0)
					{
						return tag;
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
		if (w_listView.Items != null)
		{
			foreach (ListPickerViewItem item in w_listView.Items)
			{
				ListPickerItem text = ((item.Tag == null || !(item.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)item.Tag));
				text.Target = item.SubItems[0].Text;
				text.TargetType = item.SubItems[1].Text;
				if (w_listView.Columns.Count > 2)
				{
					int num = 2;
					if (w_listView.Columns[2].Text.Equals("Group"))
					{
						num++;
					}
					for (int i = num; i < w_listView.Columns.Count; i++)
					{
						ColumnHeader columnHeader = w_listView.Columns[i];
						if (text.CustomColumns.ContainsKey(columnHeader.Name))
						{
							text.CustomColumns.Remove(columnHeader.Name);
						}
						text.CustomColumns.Add(columnHeader.Name, item.SubItems[columnHeader.Index].Text);
					}
				}
				text.Group = ((item.Group != null) ? item.Group.Name : item.SubItems["Group"].Text);
				listPickerItems.Add(text);
			}
		}
		return listPickerItems.ToArray();
	}

        public ListPickerItem[] GetSelectedItems()
	{
		List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
		if (w_listView.SelectedItems != null)
		{
			foreach (ListPickerViewItem selectedItem in w_listView.SelectedItems)
			{
				ListPickerItem text = ((selectedItem.Tag == null || !(selectedItem.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)selectedItem.Tag));
				text.Target = selectedItem.SubItems[0].Text;
				text.TargetType = selectedItem.SubItems[1].Text;
				if (w_listView.Columns.Count > 2)
				{
					int num = 2;
					if (w_listView.Columns[2].Text.Equals("Group"))
					{
						num++;
					}
					for (int i = num; i < w_listView.Columns.Count; i++)
					{
						ColumnHeader item = w_listView.Columns[i];
						if (text.CustomColumns.ContainsKey(item.Name))
						{
							text.CustomColumns.Remove(item.Name);
						}
						text.CustomColumns.Add(item.Name, selectedItem.SubItems[item.Index].Text);
					}
				}
				text.Group = ((selectedItem.Group != null) ? selectedItem.Group.Name : selectedItem.SubItems["Group"].Text);
				listPickerItems.Add(text);
			}
		}
		return listPickerItems.ToArray();
	}

        public void HideItem(ListPickerItem item)
	{
		ListPickerItem text = null;
		if (w_listView.Items == null)
		{
			return;
		}
		ListPickerViewItem listPickerViewItem = null;
		foreach (ListPickerViewItem listPickerViewItem1 in w_listView.Items)
		{
			if (!item.Equals(listPickerViewItem1.Tag))
			{
				continue;
			}
			text = ((listPickerViewItem1.Tag == null || !(listPickerViewItem1.Tag is ListPickerItem)) ? new ListPickerItem() : ((ListPickerItem)listPickerViewItem1.Tag));
			text.Target = listPickerViewItem1.SubItems[0].Text;
			text.TargetType = listPickerViewItem1.SubItems[1].Text;
			if (w_listView.Columns.Count > 2)
			{
				int num = 2;
				if (w_listView.Columns[2].Text.Equals("Group"))
				{
					num++;
				}
				for (int i = num; i < w_listView.Columns.Count; i++)
				{
					ColumnHeader columnHeader = w_listView.Columns[i];
					if (text.CustomColumns.ContainsKey(columnHeader.Name))
					{
						text.CustomColumns.Remove(columnHeader.Name);
					}
					text.CustomColumns.Add(columnHeader.Name, listPickerViewItem1.SubItems[columnHeader.Index].Text);
				}
			}
			text.Group = ((listPickerViewItem1.Group != null) ? listPickerViewItem1.Group.Name : listPickerViewItem1.SubItems["Group"].Text);
			listPickerViewItem = listPickerViewItem1;
			break;
		}
		listPickerViewItem?.Remove();
		if (text != null && !m_hiddenViewItems.Contains(text))
		{
			m_hiddenViewItems.Add(text);
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
		if (!Application.RenderWithVisualStyles)
		{
			AddCustomColumn("Group", "Group");
		}
		m_sorter = new ListViewColumnSorter();
		w_listView.ListViewItemSorter = m_sorter;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Data.Mapping.ListGroupControl));
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.w_toolStripTextBoxFilter = new System.Windows.Forms.ToolStripTextBox();
		this.w_listView = new System.Windows.Forms.ListView();
		this.Item = new System.Windows.Forms.ColumnHeader();
		this.ItemType = new System.Windows.Forms.ColumnHeader();
		this.w_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.w_toolStripSplitButtonFilter = new System.Windows.Forms.ToolStripSplitButton();
		this.w_toolStripDropDownButtonViews = new System.Windows.Forms.ToolStripDropDownButton();
		this.w_toolStrip = new System.Windows.Forms.ToolStrip();
		this.w_toolStripDropDownButtonColumns = new System.Windows.Forms.ToolStripDropDownButton();
		this.w_toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
		this.w_toolStripComboBoxSource = new System.Windows.Forms.ToolStripComboBox();
		this.w_toolStripLabelSource = new System.Windows.Forms.ToolStripLabel();
		this.w_toolStrip.SuspendLayout();
		base.SuspendLayout();
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.w_toolStripTextBoxFilter.Name = "w_toolStripTextBoxFilter";
		componentResourceManager.ApplyResources(this.w_toolStripTextBoxFilter, "w_toolStripTextBoxFilter");
		this.w_toolStripTextBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(w_toolStripTextBoxFilter_KeyDown);
		System.Windows.Forms.ListView.ColumnHeaderCollection columns = this.w_listView.Columns;
		System.Windows.Forms.ColumnHeader[] item = new System.Windows.Forms.ColumnHeader[2] { this.Item, this.ItemType };
		columns.AddRange(item);
		this.w_listView.ContextMenuStrip = this.w_contextMenuStrip;
		componentResourceManager.ApplyResources(this.w_listView, "w_listView");
		this.w_listView.FullRowSelect = true;
		this.w_listView.Name = "w_listView";
		this.w_listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.w_listView.UseCompatibleStateImageBehavior = false;
		this.w_listView.View = System.Windows.Forms.View.Details;
		this.w_listView.SelectedIndexChanged += new System.EventHandler(w_listView_SelectedIndexChanged);
		this.w_listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(w_listView_ColumnClick);
		this.w_listView.KeyDown += new System.Windows.Forms.KeyEventHandler(w_listView_KeyDown);
		this.Item.Name = "Item";
		componentResourceManager.ApplyResources(this.Item, "Item");
		this.ItemType.Name = "ItemType";
		componentResourceManager.ApplyResources(this.ItemType, "ItemType");
		this.w_contextMenuStrip.Name = "w_contextMenuStrip";
		componentResourceManager.ApplyResources(this.w_contextMenuStrip, "w_contextMenuStrip");
		this.w_contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(w_contextMenuStrip_Opening);
		this.w_toolStripSplitButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_toolStripSplitButtonFilter.Image = Metalogix.UI.WinForms.Properties.Resources.Filter16;
		componentResourceManager.ApplyResources(this.w_toolStripSplitButtonFilter, "w_toolStripSplitButtonFilter");
		this.w_toolStripSplitButtonFilter.Name = "w_toolStripSplitButtonFilter";
		this.w_toolStripSplitButtonFilter.ButtonClick += new System.EventHandler(w_toolStripSplitButtonFilter_ButtonClick);
		this.w_toolStripDropDownButtonViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		componentResourceManager.ApplyResources(this.w_toolStripDropDownButtonViews, "w_toolStripDropDownButtonViews");
		this.w_toolStripDropDownButtonViews.Name = "w_toolStripDropDownButtonViews";
		this.w_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.w_toolStrip.Items;
		System.Windows.Forms.ToolStripItem[] wToolStripDropDownButtonColumns = new System.Windows.Forms.ToolStripItem[8] { this.w_toolStripDropDownButtonColumns, this.w_toolStripTextBoxFilter, this.w_toolStripButtonFilter, this.w_toolStripSplitButtonFilter, this.toolStripSeparator1, this.w_toolStripDropDownButtonViews, this.w_toolStripComboBoxSource, this.w_toolStripLabelSource };
		items.AddRange(wToolStripDropDownButtonColumns);
		componentResourceManager.ApplyResources(this.w_toolStrip, "w_toolStrip");
		this.w_toolStrip.Name = "w_toolStrip";
		this.w_toolStripDropDownButtonColumns.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		componentResourceManager.ApplyResources(this.w_toolStripDropDownButtonColumns, "w_toolStripDropDownButtonColumns");
		this.w_toolStripDropDownButtonColumns.Name = "w_toolStripDropDownButtonColumns";
		this.w_toolStripButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_toolStripButtonFilter.Image = Metalogix.UI.WinForms.Properties.Resources.Filter16;
		componentResourceManager.ApplyResources(this.w_toolStripButtonFilter, "w_toolStripButtonFilter");
		this.w_toolStripButtonFilter.Name = "w_toolStripButtonFilter";
		this.w_toolStripButtonFilter.Click += new System.EventHandler(w_toolStripSplitButtonFilter_ButtonClick);
		this.w_toolStripComboBoxSource.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.w_toolStripComboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.w_toolStripComboBoxSource.Name = "w_toolStripComboBoxSource";
		componentResourceManager.ApplyResources(this.w_toolStripComboBoxSource, "w_toolStripComboBoxSource");
		this.w_toolStripComboBoxSource.SelectedIndexChanged += new System.EventHandler(w_toolStripComboBoxSource_SelectedIndexChanged);
		this.w_toolStripLabelSource.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		this.w_toolStripLabelSource.Name = "w_toolStripLabelSource";
		componentResourceManager.ApplyResources(this.w_toolStripLabelSource, "w_toolStripLabelSource");
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_listView);
		base.Controls.Add(this.w_toolStrip);
		base.Name = "ListGroupControl";
		this.w_toolStrip.ResumeLayout(false);
		this.w_toolStrip.PerformLayout();
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

        private void RefreshFilters()
	{
		try
		{
			w_toolStripSplitButtonFilter.DropDownItems.Clear();
			foreach (IListFilter listFilter in ListCache.ListFilters)
			{
				if (w_listView.Items == null || w_listView.Items.Count <= 0 || !listFilter.AppliesTo(((ListPickerItem)w_listView.Items[0].Tag).Tag))
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
							foreach (ListPickerItem current in m_tempFilter)
							{
								if (listFilter2.AppliesTo(current.Tag) && listFilter2.Filter(current) && !m_hiddenViewItems.Contains(current) && !m_filter.Contains(current))
								{
									AddItem(current);
								}
							}
							foreach (ListPickerViewItem listPickerViewItem in w_listView.Items)
							{
								ListPickerItem listPickerItem = (ListPickerItem)listPickerViewItem.Tag;
								if (listFilter2.AppliesTo(listPickerItem.Tag) && (!listFilter2.Filter(listPickerItem) || m_hiddenViewItems.Contains(listPickerItem) || m_filter.Contains(listPickerItem)))
								{
									m_tempFilter.Add(listPickerItem);
								}
							}
							foreach (ListPickerItem current2 in m_tempFilter)
							{
								HideItem(current2);
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

        private void RefreshList()
	{
		try
		{
			m_hiddenViewItems.Clear();
			w_listView.Items.Clear();
			if (m_availables == null)
			{
				return;
			}
			List<object> objs = new List<object>(m_availables);
			if (w_listView.Items != null)
			{
				List<ListPickerViewItem> listPickerViewItems = new List<ListPickerViewItem>();
				foreach (ListPickerViewItem item in w_listView.Items)
				{
					if (objs.Contains(item.Tag))
					{
						objs.Remove(item.Tag);
					}
					else
					{
						listPickerViewItems.Add(item);
					}
				}
				foreach (ListPickerViewItem listPickerViewItem in listPickerViewItems)
				{
					listPickerViewItem.Remove();
				}
				foreach (ListPickerItem mHiddenViewItem in m_hiddenViewItems)
				{
					if (objs.Contains(mHiddenViewItem.Tag) && FindItem(mHiddenViewItem) == null)
					{
						AddItem(mHiddenViewItem);
						objs.Remove(mHiddenViewItem.Tag);
					}
				}
			}
			foreach (object obj in objs)
			{
				if (obj != null)
				{
					ListPickerItem listPickerItem = new ListPickerItem
					{
						Target = ((obj != null) ? obj.ToString() : ""),
						TargetType = ((obj != null) ? obj.GetType().Name : ""),
						Tag = obj
					};
					AddItem(listPickerItem);
				}
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
			if (w_toolStripComboBoxSource.Items.Count > 0)
			{
				w_toolStripComboBoxSource.SelectedIndex = 0;
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
				if (w_listView.Items == null || w_listView.Items.Count <= 0 || !listView.AppliesTo(((ListPickerItem)w_listView.Items[0].Tag).Tag))
				{
					continue;
				}
				ToolStripItem bitmap = w_toolStripDropDownButtonViews.DropDownItems.Add(listView.Name);
				bitmap.Tag = listView;
				bitmap.Image = Resources.View.ToBitmap();
				bitmap.Click += delegate(object sender, EventArgs e)
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
					bitmap.PerformClick();
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			w_toolStripDropDownButtonViews.Visible = w_toolStripDropDownButtonViews.HasDropDownItems;
		}
	}

        public void RemoveCustomColumn(string columnName)
	{
		if (w_listView.Columns.ContainsKey(columnName))
		{
			w_listView.Columns.RemoveByKey(columnName);
			if (w_toolStripDropDownButtonColumns.DropDownItems.ContainsKey(columnName))
			{
				w_toolStripDropDownButtonColumns.DropDownItems.RemoveByKey(columnName);
			}
			RenderView(m_currentView);
		}
	}

        public void RenderView(IListView view)
	{
		m_currentView = view;
		if (m_currentView == null)
		{
			return;
		}
		foreach (ListPickerViewItem item in w_listView.Items)
		{
			ListPickerItem tag = (ListPickerItem)item.Tag;
			if (m_currentView.AppliesTo(tag.Tag))
			{
				item.RenderView(m_currentView);
			}
		}
	}

        public void UpdateItem(ListPickerItem item)
	{
		if (m_hiddenViewItems != null)
		{
			int num = -1;
			foreach (ListPickerItem current in m_hiddenViewItems)
			{
				if (current.Equals(item) || current.Tag == item.Tag)
				{
					num = m_hiddenViewItems.IndexOf(current);
					break;
				}
			}
			if (num != -1)
			{
				m_hiddenViewItems[num] = item;
				return;
			}
		}
		if (w_listView.Items == null)
		{
			return;
		}
		foreach (ListPickerViewItem listPickerViewItem in w_listView.Items)
		{
			if (!listPickerViewItem.Tag.Equals(item) && !listPickerViewItem.Tag.Equals((item.Tag == null) ? item : item.Tag))
			{
				continue;
			}
			listPickerViewItem.Tag = item;
			try
			{
				ListViewGroup listViewGroup = null;
				if (!string.IsNullOrEmpty(item.Group))
				{
					foreach (ListViewGroup listViewGroup2 in w_listView.Groups)
					{
						if (listViewGroup2.Name.Equals(item.Group, StringComparison.InvariantCultureIgnoreCase))
						{
							listViewGroup = listViewGroup2;
							break;
						}
					}
					if (listViewGroup == null)
					{
						listViewGroup = new ListViewGroup(item.Group, item.Group);
						w_listView.Groups.Add(listViewGroup);
					}
				}
				if (listViewGroup != null)
				{
					listPickerViewItem.Group = listViewGroup;
				}
				break;
			}
			finally
			{
				listPickerViewItem.RenderView(m_currentView);
			}
		}
	}

        private void w_contextMenuStrip_Opening(object sender, CancelEventArgs e)
	{
		e.Cancel = w_listView.SelectedItems == null || w_listView.SelectedItems.Count == 0;
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

        private void w_listView_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		if (e.Column != -1)
		{
			if (e.Column != m_sorter.SortColumn)
			{
				m_sorter.SortColumn = e.Column;
				m_sorter.Order = SortOrder.Ascending;
			}
			else if (m_sorter.Order != SortOrder.Ascending)
			{
				m_sorter.Order = SortOrder.Ascending;
			}
			else
			{
				m_sorter.Order = SortOrder.Descending;
			}
			w_listView.Sort();
		}
	}

        private void w_listView_KeyDown(object sender, KeyEventArgs e)
	{
		if (!e.Control || e.KeyCode != Keys.A)
		{
			return;
		}
		if (w_listView.MultiSelect && w_listView.Items != null)
		{
			foreach (ListViewItem item in w_listView.Items)
			{
				item.Selected = true;
			}
		}
		e.Handled = true;
	}

        private void w_listView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (w_listView.SelectedItems != null && w_listView.SelectedItems.Count > 0 && this.OnSelectionChanged != null)
		{
			ListViewItem item = w_listView.SelectedItems[0];
			SourceChangedEventArgs sourceChangedEventArg = new SourceChangedEventArgs(item.Tag);
			this.OnSelectionChanged(this, sourceChangedEventArg);
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
			int num = 0;
			while (m_hiddenViewItems.Count != num)
			{
				ListPickerItem item = m_hiddenViewItems[num];
				bool flag2 = false;
				foreach (ToolStripMenuItem current in w_toolStripDropDownButtonColumns.DropDownItems)
				{
					if (!current.Checked)
					{
						continue;
					}
					string target = null;
					string name = current.Name;
					string str = name;
					if (name == null)
					{
						goto IL_00bd;
					}
					if (str == "Item")
					{
						target = item.Target;
					}
					else
					{
						if (str != "ItemType")
						{
							goto IL_00bd;
						}
						target = item.TargetType;
					}
					goto IL_00f1;
					IL_00bd:
					if (item.CustomColumns.ContainsKey(current.Name))
					{
						target = (string)item.CustomColumns[current.Name];
					}
					goto IL_00f1;
					IL_00f1:
					bool flag = target == null || target.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
					flag2 = flag;
					if (flag2)
					{
						break;
					}
				}
				if (!string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) && !flag2)
				{
					num++;
					continue;
				}
				AddItem(item);
				m_hiddenViewItems.Remove(item);
			}
			foreach (ListPickerViewItem listPickerViewItem in w_listView.Items)
			{
				bool flag3 = false;
				foreach (ToolStripMenuItem toolStripMenuItem in w_toolStripDropDownButtonColumns.DropDownItems)
				{
					if (toolStripMenuItem.Checked)
					{
						bool flag1 = !listPickerViewItem.SubItems.ContainsKey(toolStripMenuItem.Name) || (listPickerViewItem.SubItems[toolStripMenuItem.Name].Text != null && listPickerViewItem.SubItems[toolStripMenuItem.Name].Text.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
						flag3 = flag1;
						if (flag3)
						{
							break;
						}
					}
				}
				if (!string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) && !flag3 && !m_hiddenViewItems.Contains((ListPickerItem)listPickerViewItem.Tag))
				{
					m_hiddenViewItems.Add((ListPickerItem)listPickerViewItem.Tag);
				}
			}
			foreach (ListPickerItem mHiddenViewItem in m_hiddenViewItems)
			{
				HideItem(mHiddenViewItem);
			}
			foreach (ListPickerViewItem item1 in w_listView.Items)
			{
				if (m_hiddenViewItems.Contains((ListPickerItem)item1.Tag))
				{
					m_hiddenViewItems.Remove((ListPickerItem)item1.Tag);
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
