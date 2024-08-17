using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Data.Mapping;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public class ListSummaryControl : UserControl
    {
        private class ListSummaryViewItem : ListViewItem
        {
            private readonly ListView m_owner;

            public ListSummaryViewItem(ListView owner, ListSummaryItem item)
		{
			m_owner = owner;
			base.Tag = item;
			RenderView();
		}

            public ListSummaryViewItem(ListView owner, ListSummaryItem item, ListViewGroup group)
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
			ListSummaryItem tag = (ListSummaryItem)base.Tag;
			base.SubItems.Clear();
			if (view != null && view.AppliesTo(tag))
			{
				string str = view.Render(tag);
				string[] strArrays = new string[1] { "</>" };
				string[] strArrays1 = str.Split(strArrays, StringSplitOptions.None);
				string str1 = view.RenderType(tag);
				string[] strArrays2 = new string[1] { "</>" };
				string[] strArrays3 = str1.Split(strArrays2, StringSplitOptions.None);
				string str2 = view.RenderGroup(tag);
				base.Text = strArrays1[0];
				base.SubItems.Add(strArrays3[0]);
				base.SubItems.Add(strArrays1[1]);
				base.SubItems.Add(strArrays3[1]);
				tag.Source.Target = strArrays1[0];
				tag.Source.TargetType = strArrays3[0];
				tag.Target.Target = strArrays1[1];
				tag.Target.TargetType = strArrays3[1];
				tag.Group = str2;
				ListViewSubItem name = null;
				if (m_owner.Columns.Count <= 4)
				{
					return;
				}
				int num = 4;
				if (m_owner.Columns[4].Text.Equals("Group"))
				{
					name = base.SubItems.Add(str2);
					name.Name = base.ListView.Columns[4].Name;
					num++;
				}
				for (int i = num; i < m_owner.Columns.Count; i++)
				{
					string name1 = base.ListView.Columns[i].Name;
					string str3 = view.RenderColumn(tag, name1);
					name = base.SubItems.Add(str3);
					name.Name = base.ListView.Columns[i].Name;
					if (tag.CustomColumns.ContainsKey(name1))
					{
						tag.CustomColumns[name1] = str3;
					}
				}
			}
			else
			{
				if (tag.Source == null || tag.Target == null)
				{
					return;
				}
				base.Text = tag.Source.Target;
				base.SubItems.Add(tag.Source.TargetType);
				base.SubItems.Add(tag.Target.Target);
				base.SubItems.Add(tag.Target.TargetType);
				ListViewSubItem listViewSubItem = null;
				if (m_owner.Columns.Count > 4)
				{
					int num1 = 4;
					if (m_owner.Columns[4].Text.Equals("Group"))
					{
						listViewSubItem = base.SubItems.Add((base.Group != null) ? base.Group.Header : "");
						listViewSubItem.Name = m_owner.Columns[4].Name;
						num1++;
					}
					for (int j = num1; j < m_owner.Columns.Count; j++)
					{
						listViewSubItem = base.SubItems.Add(tag.CustomColumns.ContainsKey(m_owner.Columns[j].Name) ? ((string)tag.CustomColumns[m_owner.Columns[j].Name]) : "");
						listViewSubItem.Name = m_owner.Columns[j].Name;
					}
				}
			}
		}
        }

        private bool m_bMultiSelect;

        private IListView m_currentView;

        private List<ListSummaryViewItem> m_hiddenViewItems = new List<ListSummaryViewItem>();

        private ListViewColumnSorter m_sorter;

        private IContainer components;

        private ListView w_listView;

        private ColumnHeader Source;

        private ColumnHeader SourceType;

        private ColumnHeader Target;

        private ColumnHeader TargetType;

        private ToolStrip w_toolStrip;

        private ToolStripLabel toolStripLabel1;

        private ToolStripSplitButton w_toolStripSplitButtonFilter;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripDropDownButton w_toolStripDropDownButtonViews;

        private ToolStripTextBox w_toolStripTextBoxFilter;

        private ToolStripButton w_toolStripButtonFilter;

        private ToolStrip w_toolStripBottom;

        private Panel panelContent;

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

        public ListSummaryItem FirstItem
        {
            get
		{
			if (w_listView.Items == null || w_listView.Items.Count <= 0)
			{
				return null;
			}
			return (ListSummaryItem)(w_listView.Items[0] as ListSummaryViewItem).Tag;
		}
        }

        public ListSummaryItem[] Items
        {
            get
		{
			List<ListSummaryItem> listSummaryItems = new List<ListSummaryItem>();
			if (w_listView.Items != null)
			{
				foreach (ListSummaryViewItem item in w_listView.Items)
				{
					listSummaryItems.Add((ListSummaryItem)item.Tag);
				}
			}
			foreach (ListSummaryViewItem mHiddenViewItem in m_hiddenViewItems)
			{
				listSummaryItems.Add((ListSummaryItem)mHiddenViewItem.Tag);
			}
			return listSummaryItems.ToArray();
		}
            set
		{
			w_listView.Items.Clear();
			m_hiddenViewItems.Clear();
			if (value != null)
			{
				AddRange(value);
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

        public bool ShowBottomToolStrip
        {
            get
		{
			return w_toolStripBottom.Visible;
		}
            set
		{
			w_toolStripBottom.Visible = value;
		}
        }

        public bool ShowGroups
        {
            get
		{
			return w_listView.ShowGroups;
		}
            set
		{
			w_listView.ShowGroups = value;
		}
        }

        public event ListSummaryItemEventHandler OnListSummaryItemAdded;

        public event ListSummaryItemEventHandler OnListSummaryItemRemoved;

        public ListSummaryControl()
	{
		InitializeComponent();
		Initialize();
	}

        public void Add(ListSummaryItem item)
	{
		try
		{
			ListViewGroup listViewGroup = null;
			if (string.IsNullOrEmpty(item.Group))
			{
				IListGrouper listGrouper = null;
				foreach (IListGrouper listGrouper1 in ListCache.ListGroupers)
				{
					if (listGrouper1.AppliesTo(item))
					{
						listGrouper = listGrouper1;
					}
				}
				item.Group = ((listGrouper == null) ? "default" : listGrouper.Group(item));
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
			((ListSummaryViewItem)w_listView.Items.Add((listViewGroup == null) ? new ListSummaryViewItem(w_listView, item) : new ListSummaryViewItem(w_listView, item, listViewGroup))).RenderView(m_currentView);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
		finally
		{
			RefreshViews(item);
			RefreshFilters(item);
			if (this.OnListSummaryItemAdded != null)
			{
				ListSummaryItemEventArgs listSummaryItemEventArg = new ListSummaryItemEventArgs(item);
				this.OnListSummaryItemAdded(this, listSummaryItemEventArg);
			}
		}
	}

        public void AddCustomColumn(string columnName, string headerText)
	{
		if (!w_listView.Columns.ContainsKey(columnName))
		{
			w_listView.Columns.Add(columnName, headerText);
			RenderView(m_currentView);
		}
	}

        public void AddRange(IEnumerable<ListSummaryItem> items)
	{
		try
		{
			if (items == null)
			{
				return;
			}
			List<ListViewItem> listViewItems = new List<ListViewItem>();
			foreach (ListSummaryItem item in items)
			{
				ListViewGroup listViewGroup = null;
				if (string.IsNullOrEmpty(item.Group))
				{
					IListGrouper listGrouper = null;
					foreach (IListGrouper listGrouper1 in ListCache.ListGroupers)
					{
						if (listGrouper1.AppliesTo(item))
						{
							listGrouper = listGrouper1;
						}
					}
					item.Group = ((listGrouper == null) ? "default" : listGrouper.Group(item));
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
				ListSummaryViewItem listSummaryViewItem = ((listViewGroup == null) ? new ListSummaryViewItem(w_listView, item) : new ListSummaryViewItem(w_listView, item, listViewGroup));
				if (!string.IsNullOrEmpty(listSummaryViewItem.Text))
				{
					listSummaryViewItem.RenderView(m_currentView);
					listViewItems.Add(listSummaryViewItem);
				}
			}
			w_listView.Items.AddRange(listViewItems.ToArray());
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void Clear()
	{
		w_listView.Items.Clear();
		m_hiddenViewItems.Clear();
	}

        public void DisableSort()
	{
		w_listView.ListViewItemSorter = null;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        public void EnableSort()
	{
		w_listView.ListViewItemSorter = m_sorter;
	}

        public ListSummaryItem[] GetItems()
	{
		List<ListSummaryItem> listSummaryItems = new List<ListSummaryItem>();
		if (w_listView.Items != null)
		{
			foreach (ListSummaryViewItem item in w_listView.Items)
			{
				listSummaryItems.Add((ListSummaryItem)item.Tag);
			}
		}
		return listSummaryItems.ToArray();
	}

        public ListSummaryItem[] GetSelectedItems()
	{
		List<ListSummaryItem> listSummaryItems = new List<ListSummaryItem>();
		if (w_listView.SelectedItems != null)
		{
			foreach (ListSummaryViewItem selectedItem in w_listView.SelectedItems)
			{
				ListSummaryItem tag = (ListSummaryItem)selectedItem.Tag;
				listSummaryItems.Add(tag);
				if (w_listView.Columns.Count <= 4)
				{
					continue;
				}
				for (int i = 4; i < w_listView.Columns.Count; i++)
				{
					ColumnHeader item = w_listView.Columns[i];
					if (tag.CustomColumns.ContainsKey(item.Name))
					{
						tag.CustomColumns.Remove(item.Name);
					}
					tag.CustomColumns.Add(item.Name, selectedItem.SubItems[item.Index].Text);
				}
			}
		}
		return listSummaryItems.ToArray();
	}

        private void Initialize()
	{
		m_sorter = new ListViewColumnSorter();
		if (!Application.RenderWithVisualStyles)
		{
			w_listView.Columns.Add("Group", "Group", 120);
		}
		w_listView.ListViewItemSorter = m_sorter;
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Data.Mapping.ListSummaryControl));
		this.w_listView = new System.Windows.Forms.ListView();
		this.Source = new System.Windows.Forms.ColumnHeader();
		this.SourceType = new System.Windows.Forms.ColumnHeader();
		this.Target = new System.Windows.Forms.ColumnHeader();
		this.TargetType = new System.Windows.Forms.ColumnHeader();
		this.w_toolStrip = new System.Windows.Forms.ToolStrip();
		this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
		this.w_toolStripTextBoxFilter = new System.Windows.Forms.ToolStripTextBox();
		this.w_toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
		this.w_toolStripSplitButtonFilter = new System.Windows.Forms.ToolStripSplitButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.w_toolStripDropDownButtonViews = new System.Windows.Forms.ToolStripDropDownButton();
		this.w_toolStripBottom = new System.Windows.Forms.ToolStrip();
		this.panelContent = new System.Windows.Forms.Panel();
		this.w_toolStrip.SuspendLayout();
		this.panelContent.SuspendLayout();
		base.SuspendLayout();
		this.w_listView.BackColor = System.Drawing.Color.White;
		this.w_listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
		System.Windows.Forms.ListView.ColumnHeaderCollection columns = this.w_listView.Columns;
		System.Windows.Forms.ColumnHeader[] source = new System.Windows.Forms.ColumnHeader[4] { this.Source, this.SourceType, this.Target, this.TargetType };
		columns.AddRange(source);
		componentResourceManager.ApplyResources(this.w_listView, "w_listView");
		this.w_listView.FullRowSelect = true;
		this.w_listView.Name = "w_listView";
		this.w_listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.w_listView.UseCompatibleStateImageBehavior = false;
		this.w_listView.View = System.Windows.Forms.View.Details;
		this.w_listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(w_listView_ColumnClick);
		this.w_listView.SelectedIndexChanged += new System.EventHandler(w_listView_SelectedIndexChanged);
		this.w_listView.KeyDown += new System.Windows.Forms.KeyEventHandler(w_listView_KeyDown);
		componentResourceManager.ApplyResources(this.Source, "Source");
		componentResourceManager.ApplyResources(this.SourceType, "SourceType");
		componentResourceManager.ApplyResources(this.Target, "Target");
		componentResourceManager.ApplyResources(this.TargetType, "TargetType");
		this.w_toolStrip.BackColor = System.Drawing.Color.White;
		this.w_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.w_toolStrip.Items;
		System.Windows.Forms.ToolStripItem[] wToolStripTextBoxFilter = new System.Windows.Forms.ToolStripItem[6] { this.toolStripLabel1, this.w_toolStripTextBoxFilter, this.w_toolStripButtonFilter, this.w_toolStripSplitButtonFilter, this.toolStripSeparator1, this.w_toolStripDropDownButtonViews };
		items.AddRange(wToolStripTextBoxFilter);
		componentResourceManager.ApplyResources(this.w_toolStrip, "w_toolStrip");
		this.w_toolStrip.Name = "w_toolStrip";
		this.w_toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStripLabel1.Name = "toolStripLabel1";
		componentResourceManager.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
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
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.w_toolStripDropDownButtonViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		componentResourceManager.ApplyResources(this.w_toolStripDropDownButtonViews, "w_toolStripDropDownButtonViews");
		this.w_toolStripDropDownButtonViews.Name = "w_toolStripDropDownButtonViews";
		this.w_toolStripBottom.BackColor = System.Drawing.Color.White;
		componentResourceManager.ApplyResources(this.w_toolStripBottom, "w_toolStripBottom");
		this.w_toolStripBottom.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.w_toolStripBottom.Name = "w_toolStripBottom";
		this.panelContent.Controls.Add(this.w_listView);
		componentResourceManager.ApplyResources(this.panelContent, "panelContent");
		this.panelContent.Name = "panelContent";
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		base.Controls.Add(this.panelContent);
		base.Controls.Add(this.w_toolStripBottom);
		base.Controls.Add(this.w_toolStrip);
		base.Name = "ListSummaryControl";
		this.w_toolStrip.ResumeLayout(false);
		this.w_toolStrip.PerformLayout();
		this.panelContent.ResumeLayout(false);
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

        public void RefreshFilters(ListSummaryItem targetItem)
	{
		try
		{
			foreach (IListFilter listFilter in ListCache.ListFilters)
			{
				if (w_toolStripSplitButtonFilter.DropDownItems.ContainsKey(listFilter.Name) || !listFilter.AppliesTo(targetItem))
				{
					continue;
				}
				ToolStripItem name = w_toolStripSplitButtonFilter.DropDownItems.Add(listFilter.Name);
				name.Name = listFilter.Name;
				name.Tag = listFilter;
				name.Image = Resources.Filter;
				name.Click += delegate(object sender, EventArgs e)
				{
					if (sender != null)
					{
						ToolStripItem toolStripItem = sender as ToolStripItem;
						if (toolStripItem.Tag != null && toolStripItem.Tag is IListFilter)
						{
							IListFilter listFilter2 = toolStripItem.Tag as IListFilter;
							foreach (ListSummaryViewItem current in m_hiddenViewItems)
							{
								if (listFilter2.AppliesTo(current.Tag) && listFilter2.Filter(current.Tag))
								{
									w_listView.Items.Add(current);
								}
							}
							foreach (ListSummaryViewItem listSummaryViewItem in w_listView.Items)
							{
								if (listFilter2.AppliesTo(listSummaryViewItem.Tag) && !listFilter2.Filter(listSummaryViewItem.Tag))
								{
									m_hiddenViewItems.Add(listSummaryViewItem);
								}
							}
							foreach (ListSummaryViewItem current2 in m_hiddenViewItems)
							{
								if (w_listView.Items.Contains(current2))
								{
									w_listView.Items.Remove(current2);
								}
							}
							foreach (ListSummaryViewItem item in w_listView.Items)
							{
								if (m_hiddenViewItems.Contains(item))
								{
									m_hiddenViewItems.Remove(item);
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

        public void RefreshViews(ListSummaryItem targetItem)
	{
		try
		{
			foreach (IListView listView in ListCache.ListViews)
			{
				if (w_toolStripDropDownButtonViews.DropDownItems.ContainsKey(listView.Name) || !listView.AppliesTo(targetItem))
				{
					continue;
				}
				ToolStripItem name = w_toolStripDropDownButtonViews.DropDownItems.Add(listView.Name);
				name.Name = listView.Name;
				name.Tag = listView;
				name.Image = Resources.View.ToBitmap();
				name.Click += delegate(object sender, EventArgs e)
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
				if (m_currentView == null && listView != null && listView.GetType().GetCustomAttributes(typeof(DefaultAttribute), inherit: true).Length != 0)
				{
					name.PerformClick();
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

        public void Remove(ListSummaryItem item)
	{
		try
		{
			if (w_listView.Items == null)
			{
				return;
			}
			ListSummaryViewItem listSummaryViewItem = null;
			foreach (ListSummaryViewItem listSummaryViewItem1 in w_listView.Items)
			{
				if (!item.Equals(listSummaryViewItem1.Tag))
				{
					continue;
				}
				listSummaryViewItem = listSummaryViewItem1;
				break;
			}
			try
			{
				if (listSummaryViewItem == null)
				{
					foreach (ListSummaryViewItem mHiddenViewItem in m_hiddenViewItems)
					{
						if (!item.Equals(mHiddenViewItem.Tag))
						{
							continue;
						}
						listSummaryViewItem = mHiddenViewItem;
						break;
					}
					if (listSummaryViewItem != null)
					{
						m_hiddenViewItems.Remove(listSummaryViewItem);
					}
				}
				else
				{
					listSummaryViewItem.Remove();
				}
			}
			finally
			{
				if (this.OnListSummaryItemRemoved != null)
				{
					ListSummaryItemEventArgs listSummaryItemEventArg = new ListSummaryItemEventArgs((ListSummaryItem)listSummaryViewItem.Tag);
					this.OnListSummaryItemRemoved(this, listSummaryItemEventArg);
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(exception);
		}
	}

        public void RemoveCustomColumn(string columnName)
	{
		if (w_listView.Columns.ContainsKey(columnName))
		{
			w_listView.Columns.RemoveByKey(columnName);
			RenderView(m_currentView);
		}
	}

        public void RenderView(IListView view)
	{
		try
		{
			m_currentView = view;
			foreach (ListSummaryViewItem item in w_listView.Items)
			{
				ListSummaryItem tag = item.Tag as ListSummaryItem;
				if (m_currentView.AppliesTo(tag))
				{
					item.RenderView(m_currentView);
				}
			}
			foreach (ListSummaryViewItem mHiddenViewItem in m_hiddenViewItems)
			{
				ListSummaryItem listSummaryItem = mHiddenViewItem.Tag as ListSummaryItem;
				if (m_currentView.AppliesTo(listSummaryItem))
				{
					mHiddenViewItem.RenderView(m_currentView);
				}
			}
		}
		catch (Exception)
		{
		}
	}

        public void Update(ListSummaryItem item)
	{
		if (w_listView.Items == null)
		{
			return;
		}
		foreach (ListSummaryViewItem listSummaryViewItem in w_listView.Items)
		{
			ListSummaryItem tag = (ListSummaryItem)listSummaryViewItem.Tag;
			object obj = tag.Source.Tag;
			object source = ((item.Source.Tag != null) ? item.Source.Tag : item.Source);
			if (obj.Equals(source))
			{
				object tag1 = tag.Target.Tag;
				object target = ((item.Target.Tag != null) ? item.Target.Tag : item.Target);
				if (tag1.Equals(target))
				{
					listSummaryViewItem.RenderView();
					break;
				}
			}
		}
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
		w_toolStripBottom.Items.Clear();
		if (w_listView.SelectedItems == null || w_listView.SelectedItems.Count != 1)
		{
			return;
		}
		foreach (IListSummaryAction listSummaryAction in ListCache.ListSummaryActions)
		{
			try
			{
				bool flag = true;
				ListSummaryItem[] selectedItems = GetSelectedItems();
				ListSummaryItem[] listSummaryItemArray1 = selectedItems;
				for (int num = 0; num < listSummaryItemArray1.Length; num++)
				{
					if (!listSummaryAction.AppliesTo(listSummaryItemArray1[num]))
					{
						flag = false;
					}
				}
				if (!flag)
				{
					continue;
				}
				ToolStripMenuItem item = null;
				string name = listSummaryAction.Name;
				char[] chrArray = new char[1] { '>' };
				string[] strArrays = name.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < strArrays.Length; j++)
				{
					string str = strArrays[j].Trim();
					if (item != null)
					{
						item = (ToolStripMenuItem)(item.DropDownItems.ContainsKey(str) ? item.DropDownItems[str] : item.DropDownItems.Add(str));
					}
					else
					{
						if (!w_toolStripBottom.Items.ContainsKey(str))
						{
							int num1 = w_toolStripBottom.Items.Add(new ToolStripMenuItem(str));
							w_toolStripBottom.Items[num1].Name = str;
						}
						item = (ToolStripMenuItem)w_toolStripBottom.Items[str];
					}
					item.Name = str;
				}
				item.Image = listSummaryAction.Image;
				item.Tag = listSummaryAction;
				item.Click += delegate
				{
					IListSummaryAction listSummaryAction2 = (IListSummaryAction)item.Tag;
					ListSummaryItem[] array = selectedItems;
					foreach (ListSummaryItem item2 in array)
					{
						try
						{
							listSummaryAction2.RunAction(item2);
						}
						catch (Exception exc)
						{
							GlobalServices.ErrorHandler.HandleException(exc);
						}
						finally
						{
							Update(item2);
						}
					}
				};
			}
			catch (Exception)
			{
			}
		}
	}

        private void w_toolStripSplitButtonFilter_ButtonClick(object sender, EventArgs e)
	{
		try
		{
			int num = 0;
			while (m_hiddenViewItems.Count != num)
			{
				ListSummaryViewItem item = m_hiddenViewItems[num];
				if (!string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) && item.SubItems[0].Text != null && item.SubItems[0].Text.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) < 0 && item.SubItems[2].Text != null && item.SubItems[2].Text.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					num++;
					continue;
				}
				if (item.Group == null)
				{
					ListSummaryItem tag = (ListSummaryItem)item.Tag;
					if (!string.IsNullOrEmpty(tag.Group))
					{
						item.Group = w_listView.Groups[tag.Group];
					}
				}
				w_listView.Items.Add(item);
				m_hiddenViewItems.Remove(item);
			}
			foreach (ListSummaryViewItem listSummaryViewItem in w_listView.Items)
			{
				if (!string.IsNullOrEmpty(w_toolStripTextBoxFilter.Text) && listSummaryViewItem.Text != null && listSummaryViewItem.Text.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) < 0 && listSummaryViewItem.SubItems[2].Text != null && listSummaryViewItem.SubItems[2].Text.IndexOf(w_toolStripTextBoxFilter.Text, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					((ListSummaryItem)listSummaryViewItem.Tag).Group = ((listSummaryViewItem.Group != null) ? listSummaryViewItem.Group.Name : listSummaryViewItem.SubItems["Group"].Text);
					m_hiddenViewItems.Add(listSummaryViewItem);
				}
			}
			foreach (ListSummaryViewItem mHiddenViewItem in m_hiddenViewItems)
			{
				if (w_listView.Items.Contains(mHiddenViewItem))
				{
					w_listView.Items.Remove(mHiddenViewItem);
				}
			}
			foreach (ListSummaryViewItem item1 in w_listView.Items)
			{
				if (m_hiddenViewItems.Contains(item1))
				{
					m_hiddenViewItems.Remove(item1);
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
