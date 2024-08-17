using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;
using Metalogix.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Metalogix.UI.Standard.Explorer
{
    public partial class STItemCollectionView : ItemCollectionView
    {
        protected static Hashtable ColumnWidthHash;

        private bool m_bUpdatingColumnWidths;

        private Dictionary<ListItem, STItemCollectionView.ItemCollectionViewListItem> m_itemHash = new Dictionary<ListItem, STItemCollectionView.ItemCollectionViewListItem>();

        private ContextMenuStrip _additionalContextMenu;

        private ItemViewFilter m_filter;

        private IContainer components;

        private FlickerFreeListView w_listview;

        private System.Windows.Forms.Timer w_selectedIndexTimer;

        protected ContextMenuStrip contextMenuStripColumns;

        protected ToolStripMenuItem addColumnToolStripMenuItem;

        protected ToolStripMenuItem removeColumnToolStripMenuItem;

        protected ToolStripMenuItem resetDefaultsToolStripMenuItem;

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return this._additionalContextMenu;
            }
            set
            {
                this._additionalContextMenu = value;
            }
        }

        public override IDataConverter<object, string> DataConverter
        {
            get
            {
                return base.DataConverter;
            }
            set
            {
                base.DataConverter = value;
                this.UpdateUI();
            }
        }

        public override ListItemCollection DataSource
        {
            get
            {
                return this.m_dataSource;
            }
            set
            {
                if (this.m_dataSource != null)
                {
                    this.m_dataSource.OnNodeCollectionChanged -= this.m_handlerCollectionChanged;
                    this.m_handlerCollectionChanged = null;
                }
                this.m_dataSource = value;
                this.w_propertyGrid.DataSource = null;
                if (this.m_dataSource != null)
                {
                    STItemCollectionView sTItemCollectionView = this;
                    this.m_handlerCollectionChanged = new NodeCollectionChangedHandler(sTItemCollectionView.On_dataSource_CollectionChanged);
                    this.m_dataSource.OnNodeCollectionChanged += this.m_handlerCollectionChanged;
                    base.DataSourceIsVersionCollection = (this.m_dataSource is ListItemVersionCollection ? true : false);
                }
                this.UpdateUI();
            }
        }

        public ItemViewFilter Filter
        {
            get
            {
                return this.m_filter;
            }
            set
            {
                if (this.m_filter != value)
                {
                    this.m_filter = value;
                    this.UpdateRows();
                }
            }
        }

        public bool MultiSelect
        {
            get
            {
                return this.w_listview.MultiSelect;
            }
            set
            {
                this.w_listview.MultiSelect = value;
            }
        }

        public override IXMLAbleList SelectedObjects
        {
            get
            {
                return this.GetSelectedObjects();
            }
        }

        static STItemCollectionView()
        {
            STItemCollectionView.ColumnWidthHash = new Hashtable();
        }

        public STItemCollectionView()
        {
            this.InitializeComponent();
        }

        private void AddNewColumn_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem == null || toolStripMenuItem.Tag == null || !(toolStripMenuItem.Tag is IManagableField))
            {
                return;
            }
            ((IManagableField)toolStripMenuItem.Tag).Visible = true;
            this.UpdateUI();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ColumnHeader columnHeader;
            if (!(new ListViewHeaderHandler()).IsHeaderClicked(this.w_listview, out columnHeader))
            {
                if (this._additionalContextMenu != null)
                {
                    this._additionalContextMenu.Show(this, base.PointToClient(Control.MousePosition));
                }
                e.Cancel = true;
                return;
            }
            bool flag = false;
            if (columnHeader != null && this.ViewFields != null)
            {
                foreach (Field viewField in this.ViewFields)
                {
                    if (string.Compare(viewField.DisplayName, columnHeader.Text, StringComparison.Ordinal) != 0)
                    {
                        continue;
                    }
                    this.removeColumnToolStripMenuItem.Tag = viewField;
                    flag = viewField is IManagableField;
                    break;
                }
            }
            this.removeColumnToolStripMenuItem.Enabled = (columnHeader == null ? false : flag);
            if (columnHeader != null && flag)
            {
                this.removeColumnToolStripMenuItem.Text = string.Format("Remove column '{0}'", columnHeader.Text);
            }
            if (this.ViewFields != null)
            {
                this.addColumnToolStripMenuItem.DropDownItems.Clear();
                foreach (Field field in this.ViewFields)
                {
                    if (!(field is IManagableField))
                    {
                        continue;
                    }
                    bool flag1 = false;
                    foreach (ColumnHeader column in this.w_listview.Columns)
                    {
                        if (column.Text != field.DisplayName)
                        {
                            continue;
                        }
                        flag1 = true;
                        break;
                    }
                    if (flag1)
                    {
                        continue;
                    }
                    ToolStripItem toolStripMenuItem = new ToolStripMenuItem(field.DisplayName)
                    {
                        Tag = field
                    };
                    toolStripMenuItem.Click += new EventHandler(this.AddNewColumn_Click);
                    this.addColumnToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
                }
            }
            this.addColumnToolStripMenuItem.Enabled = this.addColumnToolStripMenuItem.DropDownItems.Count > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillColumnHeadings()
        {
            object underlyingType;
            this.m_bUpdatingColumnWidths = true;
            this.w_listview.Columns.Clear();
            if (base.DataSourceIsVersionCollection)
            {
                this.w_listview.Columns.Add("Version").Tag = typeof(float);
                this.w_listview.Columns.Add("Version Comments");
            }
            if (this.ViewFields != null)
            {
                foreach (Field viewField in this.ViewFields)
                {
                    if (viewField is IManagableField && !((IManagableField)viewField).Visible || this.w_listview.Columns.ContainsKey(viewField.Name))
                    {
                        continue;
                    }
                    string str = (viewField.DisplayName == null ? viewField.Name : viewField.DisplayName);
                    ColumnHeader columnHeader = null;
                    columnHeader = (!STItemCollectionView.ColumnWidthHash.ContainsKey(viewField.Name) ? this.w_listview.Columns.Add(viewField.Name, str) : this.w_listview.Columns.Add(viewField.Name, str, (int)STItemCollectionView.ColumnWidthHash[viewField.Name]));
                    TypedField typedField = viewField as TypedField;
                    ColumnHeader columnHeader1 = columnHeader;
                    if (typedField != null)
                    {
                        underlyingType = typedField.UnderlyingType;
                    }
                    else
                    {
                        underlyingType = null;
                    }
                    columnHeader1.Tag = underlyingType;
                }
            }
            this.m_bUpdatingColumnWidths = false;
        }

        private void FillListView()
        {
            this.FillColumnHeadings();
            this.FillRows();
        }

        private void FillRows()
        {
            this.w_listview.Items.Clear();
            this.m_itemHash.Clear();
            bool flag = (this.Filter == null || string.IsNullOrEmpty(this.Filter.FilterExpression) ? false : !string.IsNullOrEmpty(this.Filter.ColumnName));
            if (this.DataSource != null)
            {
                List<STItemCollectionView.ItemCollectionViewListItem> itemCollectionViewListItems = new List<STItemCollectionView.ItemCollectionViewListItem>();
                foreach (ListItem dataSource in this.DataSource)
                {
                    if (flag)
                    {
                        string item = dataSource[this.Filter.ColumnName];
                        if (string.IsNullOrEmpty(item) || item.IndexOf(this.Filter.FilterExpression, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }
                    }
                    STItemCollectionView.ItemCollectionViewListItem itemCollectionViewListItem = new STItemCollectionView.ItemCollectionViewListItem(dataSource, this.w_listview, base.DataSourceIsVersionCollection, this.DataConverter);
                    itemCollectionViewListItems.Add(itemCollectionViewListItem);
                    this.m_itemHash.Add(dataSource, itemCollectionViewListItem);
                }
                this.UpdateUIAddRange(itemCollectionViewListItems.ToArray());
            }
        }

        private void FireStatusChanged(ItemCollectionViewStatus status)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(status);
            }
        }

        protected override ListItemCollection GetDataSourceTypedItemCollection(ListItemCollection items)
        {
            if (this.DataSource == null)
            {
                return null;
            }
            Type type = this.DataSource.GetType();
            Type[] typeArray = new Type[] { items.GetType() };
            if (type.GetConstructor(typeArray) == null)
            {
                return items;
            }
            object[] objArray = new object[] { items };
            return (ListItemCollection)Activator.CreateInstance(type, objArray);
        }

        private IXMLAbleList GetSelectedObjects()
        {
            ListItemCollection listItemCollection;
            if (base.InvokeRequired)
            {
                return base.Invoke(new ItemCollectionView.GetSelectedObjectsDelegate(this.GetSelectedObjects)) as IXMLAbleList;
            }
            Node[] item = new Node[this.w_listview.SelectedIndices.Count];
            for (int i = 0; i < this.w_listview.SelectedIndices.Count; i++)
            {
                item[i] = ((STItemCollectionView.ItemCollectionViewListItem)this.w_listview.SelectedItems[i]).Item;
            }
            List parent = null;
            Folder folder = null;
            if ((int)item.Length <= 0)
            {
                if (this.DataSource != null && this.DataSource.ParentFolder != null && this.DataSource.ParentFolder is Node && !base.DataSourceIsVersionCollection)
                {
                    ItemViewFolder[] itemViewFolder = new ItemViewFolder[] { new ItemViewFolder(this.DataSource.ParentFolder) };
                    return new ItemViewFolderList(itemViewFolder);
                }
            }
            else if (item[0].Parent is List)
            {
                parent = item[0].Parent as List;
            }
            else if (item[0].Parent is Folder)
            {
                folder = item[0].Parent as Folder;
            }
            if (this.DataSource != null && parent == null)
            {
                parent = this.DataSource.ParentList;
            }
            if (!base.DataSourceIsVersionCollection)
            {
                listItemCollection = new ListItemCollection(parent, folder, item);
            }
            else
            {
                listItemCollection = new ListItemVersionCollection(((ListItemVersionCollection)this.DataSource).ParentItem, parent, folder, item);
            }
            return this.GetDataSourceTypedItemCollection(listItemCollection);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(STItemCollectionView));
            this.w_listview = new FlickerFreeListView();
            this.contextMenuStripColumns = new ContextMenuStrip(this.components);
            this.addColumnToolStripMenuItem = new ToolStripMenuItem();
            this.removeColumnToolStripMenuItem = new ToolStripMenuItem();
            this.resetDefaultsToolStripMenuItem = new ToolStripMenuItem();
            this.w_selectedIndexTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripColumns.SuspendLayout();
            base.SuspendLayout();
            this.w_splitContainer.BackColor = Color.White;
            this.w_splitContainer.Panel2.Controls.Add(this.w_listview);
            this.w_imageFileTypes.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("w_imageFileTypes.ImageStream");
            this.w_imageFileTypes.Images.SetKeyName(0, "Desc.ico");
            this.w_imageFileTypes.Images.SetKeyName(1, "Asc.ico");
            componentResourceManager.ApplyResources(this.w_listview, "w_listview");
            this.w_listview.BorderStyle = BorderStyle.None;
            this.w_listview.ClearingBackground = false;
            this.w_listview.ContextMenuStrip = this.contextMenuStripColumns;
            this.w_listview.FullRowSelect = true;
            this.w_listview.HideSelection = false;
            this.w_listview.Name = "w_listview";
            this.w_listview.SmallImageList = this.w_imageFileTypes;
            this.w_listview.UseCompatibleStateImageBehavior = false;
            this.w_listview.UseDoubleBuffering = true;
            this.w_listview.View = View.Details;
            this.w_listview.ColumnClick += new ColumnClickEventHandler(this.On_listView_ColumnClick);
            this.w_listview.ColumnWidthChanged += new ColumnWidthChangedEventHandler(this.On_Column_Width_Changed);
            this.w_listview.SelectedIndexChanged += new EventHandler(this.On_listView_SelectionChanged);
            this.w_listview.KeyDown += new KeyEventHandler(this.On_listview_KeyDown);
            ToolStripItemCollection items = this.contextMenuStripColumns.Items;
            ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.addColumnToolStripMenuItem, this.removeColumnToolStripMenuItem, this.resetDefaultsToolStripMenuItem };
            items.AddRange(toolStripItemArray);
            this.contextMenuStripColumns.Name = "contextMenuStrip1";
            componentResourceManager.ApplyResources(this.contextMenuStripColumns, "contextMenuStripColumns");
            this.contextMenuStripColumns.Opening += new CancelEventHandler(this.contextMenuStrip1_Opening);
            this.addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
            componentResourceManager.ApplyResources(this.addColumnToolStripMenuItem, "addColumnToolStripMenuItem");
            this.removeColumnToolStripMenuItem.Name = "removeColumnToolStripMenuItem";
            componentResourceManager.ApplyResources(this.removeColumnToolStripMenuItem, "removeColumnToolStripMenuItem");
            this.removeColumnToolStripMenuItem.Tag = "";
            this.removeColumnToolStripMenuItem.Click += new EventHandler(this.removeColumnToolStripMenuItem_Click);
            this.resetDefaultsToolStripMenuItem.Name = "resetDefaultsToolStripMenuItem";
            componentResourceManager.ApplyResources(this.resetDefaultsToolStripMenuItem, "resetDefaultsToolStripMenuItem");
            this.resetDefaultsToolStripMenuItem.Click += new EventHandler(this.resetDefaultsToolStripMenuItem_Click);
            this.w_selectedIndexTimer.Interval = 10;
            this.w_selectedIndexTimer.Tick += new EventHandler(this.On_selectedIndex_Tick);
            componentResourceManager.ApplyResources(this, "$this");
            this.BackColor = Color.White;
            base.Name = "STItemCollectionView";
            base.Controls.SetChildIndex(this.w_splitContainer, 0);
            this.contextMenuStripColumns.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void On_Column_Width_Changed(object sender, ColumnWidthChangedEventArgs e)
        {
            if (!this.m_bUpdatingColumnWidths)
            {
                ColumnHeader item = this.w_listview.Columns[e.ColumnIndex];
                if (STItemCollectionView.ColumnWidthHash.ContainsKey(item.Name))
                {
                    STItemCollectionView.ColumnWidthHash[item.Name] = item.Width;
                    return;
                }
                STItemCollectionView.ColumnWidthHash.Add(item.Name, item.Width);
            }
        }

        protected override void On_dataSource_CollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
            if (changeType == NodeCollectionChangeType.FullReset)
            {
                this.UpdateUI();
                return;
            }
            if (changeType == NodeCollectionChangeType.NodeAdded)
            {
                this.UpdateUI();
                return;
            }
            if (changeType == NodeCollectionChangeType.NodeRemoved)
            {
                this.UpdateUIDeleteItem((ListItem)changedNode);
                return;
            }
            if (changeType == NodeCollectionChangeType.NodeChanged)
            {
                this.UpdateUIRefreshItem((ListItem)changedNode);
            }
        }

        private void On_listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            bool flag = true;
            if (this.w_listview.ListViewItemSorter != null)
            {
                ListViewColumnComparer listViewItemSorter = this.w_listview.ListViewItemSorter as ListViewColumnComparer;
                if (listViewItemSorter != null)
                {
                    if (listViewItemSorter.IndexOfSortingColumn != e.Column)
                    {
                        this.w_listview.Columns[listViewItemSorter.IndexOfSortingColumn].ImageKey = "";
                        this.w_listview.Columns[listViewItemSorter.IndexOfSortingColumn].TextAlign = HorizontalAlignment.Left;
                    }
                    else if (listViewItemSorter.IsAscendingSort)
                    {
                        flag = false;
                    }
                }
            }
            Type tag = this.w_listview.Columns[e.Column].Tag as Type;
            if (!flag)
            {
                this.w_listview.Columns[e.Column].ImageKey = "Desc.ico";
            }
            else
            {
                this.w_listview.Columns[e.Column].ImageKey = "Asc.ico";
            }
            this.w_listview.ListViewItemSorter = new ListViewColumnComparer(e.Column, flag, tag);
        }

        private void On_listview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 65 && e.Control)
            {
                this.w_selectedIndexTimer.Enabled = true;
                this.SelectAll();
                e.Handled = true;
            }
        }

        private void On_listView_SelectionChanged(object sender, EventArgs e)
        {
            this.w_selectedIndexTimer.Enabled = true;
        }

        private void On_selectedIndex_Tick(object sender, EventArgs e)
        {
            this.w_selectedIndexTimer.Enabled = false;
            Thread thread = new Thread(new ThreadStart(this.UpdateSelected));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void removeColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.removeColumnToolStripMenuItem.Tag == null || !(this.removeColumnToolStripMenuItem.Tag is IManagableField))
            {
                return;
            }
            foreach (ColumnHeader column in this.w_listview.Columns)
            {
                if (string.Compare(column.Text, ((Field)this.removeColumnToolStripMenuItem.Tag).DisplayName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    continue;
                }
                ((IManagableField)this.removeColumnToolStripMenuItem.Tag).Visible = false;
                break;
            }
            this.UpdateUI();
        }

        private void resetDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewFields != null)
            {
                foreach (Field viewField in this.ViewFields)
                {
                    if (!(viewField is IManagableField))
                    {
                        continue;
                    }
                    ((IManagableField)viewField).Visible = ((IManagableField)viewField).DefaultVisible;
                }
                this.UpdateUI();
            }
        }

        private void SelectAll()
        {
            try
            {
                this.w_listview.BeginUpdate();
                for (int i = 0; i < this.w_listview.Items.Count; i++)
                {
                    this.w_listview.Items[i].Selected = true;
                }
            }
            finally
            {
                this.w_listview.EndUpdate();
            }
        }

        public void UpdateColumnWidths(string sColumnNames, int iColumnWidthValue)
        {
            this.UpdateColumnWidths(sColumnNames, iColumnWidthValue, new char[] { ',' });
        }

        public void UpdateColumnWidths(string sColumnNames, int iColumnWidthValue, char[] columnNameDelimiters)
        {
            string[] strArrays = null;
            if (sColumnNames != null)
            {
                strArrays = sColumnNames.Split(columnNameDelimiters);
            }
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str = strArrays1[i];
                if (!STItemCollectionView.ColumnWidthHash.ContainsKey(str))
                {
                    STItemCollectionView.ColumnWidthHash.Add(str, iColumnWidthValue);
                }
                else
                {
                    STItemCollectionView.ColumnWidthHash[str] = iColumnWidthValue;
                }
            }
            this.m_bUpdatingColumnWidths = true;
            foreach (ColumnHeader column in this.w_listview.Columns)
            {
                bool flag = false;
                if (strArrays == null)
                {
                    continue;
                }
                string[] strArrays2 = strArrays;
                int num = 0;
                while (num < (int)strArrays2.Length)
                {
                    string str1 = strArrays2[num];
                    if (column.Name.ToUpper() != str1.ToUpper())
                    {
                        num++;
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    continue;
                }
                column.Width = iColumnWidthValue;
            }
            this.m_bUpdatingColumnWidths = false;
        }

        private void UpdateRows()
        {
            this.w_listview.Items.Clear();
            bool flag = (this.Filter == null || string.IsNullOrEmpty(this.Filter.FilterExpression) ? false : !string.IsNullOrEmpty(this.Filter.ColumnName));
            List<STItemCollectionView.ItemCollectionViewListItem> itemCollectionViewListItems = new List<STItemCollectionView.ItemCollectionViewListItem>();
            if (!flag)
            {
                itemCollectionViewListItems.AddRange(this.m_itemHash.Values);
            }
            else
            {
                foreach (KeyValuePair<ListItem, STItemCollectionView.ItemCollectionViewListItem> mItemHash in this.m_itemHash)
                {
                    string item = mItemHash.Key[this.Filter.ColumnName];
                    if (string.IsNullOrEmpty(item) || item.IndexOf(this.Filter.FilterExpression, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                    itemCollectionViewListItems.Add(mItemHash.Value);
                }
            }
            this.UpdateUIAddRange(itemCollectionViewListItems.ToArray());
            this.w_selectedIndexTimer.Enabled = true;
        }

        private void UpdateSelected()
        {
            ListItemCollection selectedObjects = this.SelectedObjects as ListItemCollection;
            this.w_propertyGrid.DataSource = selectedObjects;
            this.FireSelectedItemChangedEvent(selectedObjects);
        }

        private void UpdateUI()
        {
            if (base.IsDisposed)
            {
                return;
            }
            if (base.ParentForm == null)
            {
                return;
            }
            if (base.InvokeRequired)
            {
                base.Invoke(new STItemCollectionView.UpdateUIDelegate(this.UpdateUI));
                return;
            }
            try
            {
                this.FireStatusChanged(ItemCollectionViewStatus.Loading);
                this.w_listview.BeginUpdate();
                this.w_listview.Clear();
                this.w_listview.ListViewItemSorter = null;
                if (this.DataSource != null)
                {
                    this.FillListView();
                }
                UIUtils.FitColumnsToContent(this.w_listview);
                UIUtils.ReduceColumnsWidth(this.w_listview);
            }
            finally
            {
                this.w_listview.EndUpdate();
                this.FireStatusChanged(ItemCollectionViewStatus.Loaded);
            }
        }

        private void UpdateUIAddItem(ListItem addedItem)
        {
            if (base.InvokeRequired)
            {
                Delegate granularUIUpdateDelegate = new STItemCollectionView.GranularUIUpdateDelegate(this.UpdateUIAddItem);
                object[] objArray = new object[] { addedItem };
                base.Invoke(granularUIUpdateDelegate, objArray);
                return;
            }
            STItemCollectionView.ItemCollectionViewListItem itemCollectionViewListItem = new STItemCollectionView.ItemCollectionViewListItem(addedItem, this.w_listview, base.DataSourceIsVersionCollection, this.DataConverter);
            this.m_itemHash.Add(addedItem, itemCollectionViewListItem);
            this.w_listview.Items.Add(itemCollectionViewListItem);
        }

        private void UpdateUIAddRange(STItemCollectionView.ItemCollectionViewListItem[] addedItems)
        {
            if (!base.InvokeRequired)
            {
                this.w_listview.SuspendLayout();
                this.w_listview.Items.AddRange(addedItems);
                this.w_listview.ResumeLayout();
                return;
            }
            Delegate rangedUIUpdateDelegate = new STItemCollectionView.RangedUIUpdateDelegate(this.UpdateUIAddRange);
            object[] objArray = new object[] { addedItems };
            base.Invoke(rangedUIUpdateDelegate, objArray);
        }

        private void UpdateUIDeleteItem(ListItem deletedItem)
        {
            if (base.InvokeRequired)
            {
                Delegate granularUIUpdateDelegate = new STItemCollectionView.GranularUIUpdateDelegate(this.UpdateUIDeleteItem);
                object[] objArray = new object[] { deletedItem };
                base.Invoke(granularUIUpdateDelegate, objArray);
                return;
            }
            if (!this.m_itemHash.ContainsKey(deletedItem))
            {
                return;
            }
            STItemCollectionView.ItemCollectionViewListItem item = this.m_itemHash[deletedItem];
            this.w_listview.Items.Remove(item);
            this.m_itemHash.Remove(deletedItem);
            if (this.w_listview.SelectedItems.Count <= 0)
            {
                this.UpdateSelected();
            }
        }

        private void UpdateUIRefreshItem(ListItem updatedItem)
        {
            if (!base.InvokeRequired)
            {
                this.m_itemHash[updatedItem].Item = updatedItem;
                return;
            }
            Delegate granularUIUpdateDelegate = new STItemCollectionView.GranularUIUpdateDelegate(this.UpdateUIRefreshItem);
            object[] objArray = new object[] { updatedItem };
            base.Invoke(granularUIUpdateDelegate, objArray);
        }

        public event StatusChangedEventHandler StatusChanged;

        private delegate void GranularUIUpdateDelegate(ListItem item);

        private class ItemCollectionViewListItem : ListViewItem
        {
            private ListItem m_item;

            private ListView m_listView;

            private IDataConverter<object, string> m_dataConverter;

            private bool m_bAddVersionFieldData;

            public ListItem Item
            {
                get
                {
                    return this.m_item;
                }
                set
                {
                    this.m_item = value;
                    this.UpdateUI();
                }
            }

            public ItemCollectionViewListItem(ListItem item, ListView listView, bool bListViewIsVersioned, IDataConverter<object, string> dataConverter)
            {
                this.m_item = item;
                this.m_listView = listView;
                this.m_bAddVersionFieldData = bListViewIsVersioned;
                this.m_dataConverter = dataConverter;
                this.UpdateUI();
            }

            public void UpdateUI()
            {
                ListViewItem.ListViewSubItem item;
                string str;
                try
                {
                    int num = 0;
                    if (this.m_bAddVersionFieldData && this.Item is ListItemVersion)
                    {
                        base.Text = ((ListItemVersion)this.Item).VersionString;
                        ListViewItem.ListViewSubItem listViewSubItem = new ListViewItem.ListViewSubItem()
                        {
                            Text = ((ListItemVersion)this.Item).VersionComments
                        };
                        base.SubItems.Add(listViewSubItem);
                        num = 2;
                    }
                    PropertyDescriptorCollection properties = this.m_item.GetProperties();
                    for (int i = num; i < this.m_listView.Columns.Count; i++)
                    {
                        ColumnHeader columnHeader = this.m_listView.Columns[i];
                        if (base.SubItems.Count >= i + 1)
                        {
                            item = base.SubItems[i];
                        }
                        else
                        {
                            item = new ListViewItem.ListViewSubItem();
                            base.SubItems.Add(item);
                        }
                        PropertyDescriptor propertyDescriptor = properties[columnHeader.Name];
                        object obj = (propertyDescriptor != null ? propertyDescriptor.GetValue(this.m_item) : null);
                        if (this.m_dataConverter == null)
                        {
                            str = (obj == null ? "" : obj.ToString());
                        }
                        else
                        {
                            str = this.m_dataConverter.Convert(obj);
                        }
                        item.Text = str;
                    }
                    if (this.m_item.ImageName != null)
                    {
                        if (!this.m_listView.SmallImageList.Images.ContainsKey(this.m_item.ImageName))
                        {
                            this.m_listView.SmallImageList.Images.Add(this.m_item.ImageName, this.m_item.Image);
                        }
                        base.ImageKey = this.m_item.ImageName;
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        private delegate void RangedUIUpdateDelegate(STItemCollectionView.ItemCollectionViewListItem[] items);

        private delegate void UpdateUIDelegate();
    }
}