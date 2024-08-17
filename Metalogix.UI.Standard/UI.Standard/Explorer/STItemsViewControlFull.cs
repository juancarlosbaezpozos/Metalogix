using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.Standard.Explorer
{
    [DesignTimeVisible(true)]
    public class STItemsViewControlFull : ItemsViewControlFull
    {
        private bool m_bSuspendQuickSearchChangedEvents;
        private bool m_bShowingSearchCues = true;
        private STItemsViewControlFull.QuickFilterState m_filterState;
        private object m_oLoadLock = new object();
        private IContainer components;
        private ToolStrip w_toolStrip;
        private ToolStripButton w_btnShowPropertyGrids;
        private ToolStripButton w_btnShowVersionHistory;
        private ToolStripTextBox w_tsTextBoxQuickSearch;
        private ToolStripSplitButton w_tsButtonQuickSearch;
        private ToolStripComboBox w_tsColumnSelector;
        private ToolStripButton w_tsButtonRefresh;
        private STItemCollectionView w_listItemsView;
        private Panel panelMarque;
        private Label labelLoading;
        private MarqueeBar marqueeBar1;
        private STItemCollectionView w_VersionHistoryView;

        public override ContextMenuStrip ContextMenuStrip
        {
            get => base.ContextMenuStrip;
            set
            {
                this.w_listItemsView.ContextMenuStrip = value;
                this.w_VersionHistoryView.ContextMenuStrip = value;
            }
        }

        public override ListItemCollection DataSource
        {
            get => this.w_listItemsView.DataSource;
            set
            {
                if (value != null && this.w_listItemsView.DataSource == value)
                    return;
                this.w_listItemsView.DataSource = value;
                ToolStripTextBox textBoxQuickSearch = this.w_tsTextBoxQuickSearch;
                ToolStripSplitButton buttonQuickSearch = this.w_tsButtonQuickSearch;
                bool flag1 = value != null && value.Count > 0;
                bool flag2 = flag1;
                buttonQuickSearch.Visible = flag1;
                textBoxQuickSearch.Visible = flag2;
                this.w_VersionHistoryView.DataSource = (ListItemCollection)null;
                if (value == null || !value.ParentList.EnableVersioning)
                {
                    this.ShowingVersionHistory = false;
                    this.w_btnShowVersionHistory.Enabled = false;
                }
                else
                    this.w_btnShowVersionHistory.Enabled = true;
                if (this.m_ViewFields == null)
                    this.UpdateQueryColumnSelector();
                this.On_ItemsViewSelectionChanged((ListItemCollection)null);
            }
        }

        public override HasSelectableObjects FocusedSelectableContainer
        {
            get
            {
                return this.w_tsTextBoxQuickSearch.Focused ? (HasSelectableObjects)null : base.FocusedSelectableContainer;
            }
        }

        private string QuickQuery => this.w_tsTextBoxQuickSearch.Text.Trim();

        private string QuickQueryColumn
        {
            get => !(this.w_tsColumnSelector.SelectedItem is Field selectedItem) ? "" : selectedItem.Name;
        }

        public override IXMLAbleList SelectedObjects
        {
            get
            {
                HasSelectableObjects selectableContainer = this.FocusedSelectableContainer;
                if (selectableContainer != null && selectableContainer != this)
                    return selectableContainer.SelectedObjects;
                return this.m_bShowingVersionHistory && !this.m_bItemsViewLastFocused ? this.w_VersionHistoryView.SelectedObjects : this.w_listItemsView.SelectedObjects;
            }
        }

        protected virtual bool ShowingPropertyGrids
        {
            get => this.m_bShowingPropertyGrids;
            set
            {
                if (this.ListItemControl == null || this.ListItemVersionControl == null)
                    return;
                this.m_bShowingPropertyGrids = value;
                if (this.m_bShowingPropertyGrids)
                {
                    this.w_btnShowPropertyGrids.Text = "Hide Properties";
                    this.w_btnShowPropertyGrids.ToolTipText = "Hide Item Properties";
                    this.ListItemControl.ShowPropertyGrid = true;
                    this.ListItemVersionControl.ShowPropertyGrid = true;
                }
                else
                {
                    this.w_btnShowPropertyGrids.Text = "Show Properties";
                    this.w_btnShowPropertyGrids.ToolTipText = "Show Item Properties";
                    this.ListItemControl.ShowPropertyGrid = false;
                    this.ListItemVersionControl.ShowPropertyGrid = false;
                }
            }
        }

        protected virtual bool ShowingVersionHistory
        {
            get => this.m_bShowingVersionHistory;
            set
            {
                this.m_bShowingVersionHistory = value;
                if (this.m_bShowingVersionHistory)
                {
                    this.w_btnShowVersionHistory.Text = "Hide Version History";
                    this.w_btnShowVersionHistory.ToolTipText = "Hide Item Version History";
                    this.w_splitContainer.Panel2Collapsed = false;
                }
                else
                {
                    this.w_btnShowVersionHistory.Text = "Show Version History";
                    this.w_btnShowVersionHistory.ToolTipText = "Show Item Version History";
                    this.w_splitContainer.Panel2Collapsed = true;
                }
            }
        }

        public override FieldCollection ViewFields
        {
            get
            {
                if (this.m_ViewFields != null)
                    return this.m_ViewFields;
                return this.DataSource == null || this.DataSource.ParentList == null ? (FieldCollection)null : this.DataSource.ParentList.Fields;
            }
            set
            {
                this.m_ViewFields = value;
                this.w_listItemsView.ViewFields = value;
                this.w_VersionHistoryView.ViewFields = value;
                this.UpdateQueryColumnSelector();
            }
        }

        public STItemsViewControlFull()
        {
            this.InitializeComponent();
            this.ShowingPropertyGrids = false;
            this.ShowingVersionHistory = false;
            this.w_listItemsView.MultiSelect = true;
            this.w_VersionHistoryView.MultiSelect = false;
            this.w_listItemsView.SelectedItemsChanged += new ItemCollectionView.SelectedItemsChangedHandler(this.On_ItemsViewSelectionChanged);
            // ISSUE: reference to a compiler-generated method
            //this.w_listItemsView.Enter += new EventHandler(((ItemsViewControlFull)this).On_ItemViewEntered);
            // ISSUE: reference to a compiler-generated method
            //this.w_VersionHistoryView.Enter += new EventHandler(((ItemsViewControlFull)this).On_ItemViewEntered);
            this.w_listItemsView.StatusChanged += new StatusChangedEventHandler(this.SetStatus);
            // ISSUE: reference to a compiler-generated field
            this.w_tsButtonQuickSearch.Image = this.imageListQuickSearch.Images[0];
        }

        public void ClearItemsView()
        {
            this.FillItemsView((ListItemCollection)null, (FieldCollection)null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            // ISSUE: reference to a compiler-generated method
            base.Dispose(disposing);
        }

        private void FetchItems(object folderObj)
        {
            Folder folder = (Folder)folderObj;
            FieldCollection viewFields = (FieldCollection)null;
            ListItemCollection items = (ListItemCollection)null;
            lock (this.m_oLoadLock)
            {
                if (this.DataSource != null && this.DataSource.ParentFolder == folder)
                    return;
                try
                {
                    this.SetStatus(ItemCollectionViewStatus.Loading);
                    items = folder.GetItems();
                }
                catch (Exception ex)
                {
                    GlobalServices.ErrorHandler.HandleException("Error fetching items: " + ex.Message, ex);
                }
                this.SetStatus(ItemCollectionViewStatus.Loaded);
                this.FillItemsView(items, viewFields);
            }
        }

        private void FillItemsView(ListItemCollection items, FieldCollection viewFields)
        {
            if (!this.InvokeRequired)
            {
                try
                {
                    this.ViewFields = viewFields;
                    this.DataSource = items;
                }
                catch (Exception ex)
                {
                }
            }
            else
                this.Invoke((Delegate)new STItemsViewControlFull.FillItemsViewDelegate(this.FillItemsView), (object)items, (object)viewFields);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(STItemsViewControlFull));
            this.w_toolStrip = new ToolStrip();
            this.w_btnShowPropertyGrids = new ToolStripButton();
            this.w_btnShowVersionHistory = new ToolStripButton();
            this.w_tsTextBoxQuickSearch = new ToolStripTextBox();
            this.w_tsButtonQuickSearch = new ToolStripSplitButton();
            this.w_tsColumnSelector = new ToolStripComboBox();
            this.w_tsButtonRefresh = new ToolStripButton();
            // ISSUE: object of a compiler-generated type is created
            this.w_listItemsView = new STItemCollectionView();
            this.panelMarque = new Panel();
            this.labelLoading = new Label();
            this.marqueeBar1 = new MarqueeBar();
            // ISSUE: object of a compiler-generated type is created
            this.w_VersionHistoryView = new STItemCollectionView();
            this.w_toolStrip.SuspendLayout();
            this.panelMarque.SuspendLayout();
            this.SuspendLayout();
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.BackColor = Color.White;
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.BorderStyle = BorderStyle.None;
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.Panel1.Controls.Add((Control)this.panelMarque);
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.Panel1.Controls.Add((Control)this.w_listItemsView);
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.Panel1.Controls.Add((Control)this.w_toolStrip);
            // ISSUE: reference to a compiler-generated field
            this.w_splitContainer.Panel2.Controls.Add((Control)this.w_VersionHistoryView);
            // ISSUE: reference to a compiler-generated field
            this.imageListQuickSearch.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListQuickSearch.ImageStream");
            // ISSUE: reference to a compiler-generated field
            this.imageListQuickSearch.Images.SetKeyName(0, "QuickSearchCancel.ico");
            // ISSUE: reference to a compiler-generated field
            this.imageListQuickSearch.Images.SetKeyName(1, "QuickSearch.ico");
            this.w_toolStrip.AutoSize = false;
            this.w_toolStrip.BackColor = Color.FromArgb(230, 230, 230);
            this.w_toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            this.w_toolStrip.Items.AddRange(new ToolStripItem[5]
            {
        (ToolStripItem) this.w_btnShowPropertyGrids,
        (ToolStripItem) this.w_btnShowVersionHistory,
        (ToolStripItem) this.w_tsTextBoxQuickSearch,
        (ToolStripItem) this.w_tsButtonQuickSearch,
        (ToolStripItem) this.w_tsButtonRefresh
            });
            this.w_toolStrip.Location = new Point(0, 0);
            this.w_toolStrip.Name = "w_toolStrip";
            this.w_toolStrip.RenderMode = ToolStripRenderMode.System;
            this.w_toolStrip.RightToLeft = RightToLeft.No;
            this.w_toolStrip.Size = new Size(668, 25);
            this.w_toolStrip.TabIndex = 3;
            this.w_toolStrip.Text = "toolStrip1";
            this.w_btnShowPropertyGrids.Alignment = ToolStripItemAlignment.Right;
            this.w_btnShowPropertyGrids.Image = (Image)componentResourceManager.GetObject("w_btnShowPropertyGrids.Image");
            this.w_btnShowPropertyGrids.ImageTransparentColor = Color.Magenta;
            this.w_btnShowPropertyGrids.Name = "w_btnShowPropertyGrids";
            this.w_btnShowPropertyGrids.RightToLeft = RightToLeft.No;
            this.w_btnShowPropertyGrids.Size = new Size(105, 22);
            this.w_btnShowPropertyGrids.Text = "Show Properties";
            this.w_btnShowPropertyGrids.ToolTipText = "Show Property Grid";
            this.w_btnShowPropertyGrids.Click += new EventHandler(this.On_btnShowPropertyGrids_Click);
            this.w_btnShowVersionHistory.Alignment = ToolStripItemAlignment.Right;
            this.w_btnShowVersionHistory.Image = (Image)Metalogix.UI.Standard.Properties.Resources.ShowVersionHistory;
            this.w_btnShowVersionHistory.ImageAlign = ContentAlignment.MiddleLeft;
            this.w_btnShowVersionHistory.ImageTransparentColor = Color.Magenta;
            this.w_btnShowVersionHistory.Name = "w_btnShowVersionHistory";
            this.w_btnShowVersionHistory.RightToLeft = RightToLeft.No;
            this.w_btnShowVersionHistory.Size = new Size(128, 22);
            this.w_btnShowVersionHistory.Text = "Show Version History";
            this.w_btnShowVersionHistory.Click += new EventHandler(this.On_btnShowVersionHistory_Click);
            this.w_tsTextBoxQuickSearch.Font = new Font("Tahoma", 8.25f, FontStyle.Italic);
            this.w_tsTextBoxQuickSearch.ForeColor = SystemColors.GrayText;
            this.w_tsTextBoxQuickSearch.Name = "w_tsTextBoxQuickSearch";
            this.w_tsTextBoxQuickSearch.Size = new Size(160, 25);
            this.w_tsTextBoxQuickSearch.Visible = false;
            this.w_tsTextBoxQuickSearch.Enter += new EventHandler(this.On_tsTextBoxQuickSearch_Entered);
            this.w_tsTextBoxQuickSearch.Leave += new EventHandler(this.On_tsTextBoxQuickSeach_Left);
            this.w_tsTextBoxQuickSearch.TextChanged += new EventHandler(this.toolStripTextBoxQuickSearch_TextChanged);
            this.w_tsButtonQuickSearch.BackColor = Color.Transparent;
            this.w_tsButtonQuickSearch.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.w_tsButtonQuickSearch.DropDownItems.AddRange(new ToolStripItem[1]
            {
        (ToolStripItem) this.w_tsColumnSelector
            });
            this.w_tsButtonQuickSearch.Image = (Image)componentResourceManager.GetObject("w_tsButtonQuickSearch.Image");
            this.w_tsButtonQuickSearch.ImageTransparentColor = Color.Magenta;
            this.w_tsButtonQuickSearch.Name = "w_tsButtonQuickSearch";
            this.w_tsButtonQuickSearch.Size = new Size(32, 22);
            this.w_tsButtonQuickSearch.Text = "Quick search";
            this.w_tsButtonQuickSearch.Visible = false;
            this.w_tsButtonQuickSearch.Click += new EventHandler(this.toolStripButtonQuickSearch_Click);
            this.w_tsColumnSelector.AutoSize = false;
            this.w_tsColumnSelector.Name = "w_tsColumnSelector";
            this.w_tsColumnSelector.Size = new Size(200, 21);
            this.w_tsColumnSelector.SelectedIndexChanged += new EventHandler(this.On_ColumnSelector_SelectedChanged);
            this.w_tsColumnSelector.KeyPress += new KeyPressEventHandler(this.On_ColumnSelector_KeyPressed);
            this.w_tsButtonRefresh.Alignment = ToolStripItemAlignment.Right;
            this.w_tsButtonRefresh.Image = (Image)componentResourceManager.GetObject("w_tsButtonRefresh.Image");
            this.w_tsButtonRefresh.ImageTransparentColor = Color.Magenta;
            this.w_tsButtonRefresh.Name = "w_tsButtonRefresh";
            this.w_tsButtonRefresh.Size = new Size(65, 22);
            this.w_tsButtonRefresh.Text = "Refresh";
            this.w_tsButtonRefresh.Click += new EventHandler(this.On_tsButtonRefresh_Click);
            this.w_listItemsView.BackColor = Color.White;
            this.w_listItemsView.DataConverter = (IDataConverter<object, string>)null;
            this.w_listItemsView.DataSource = (ListItemCollection)null;
            this.w_listItemsView.Dock = DockStyle.Fill;
            this.w_listItemsView.Filter = (ItemViewFilter)null;
            this.w_listItemsView.Location = new Point(0, 25);
            this.w_listItemsView.MultiSelect = true;
            this.w_listItemsView.Name = "w_listItemsView";
            this.w_listItemsView.ShowPropertyGrid = false;
            this.w_listItemsView.Size = new Size(668, 209);
            this.w_listItemsView.TabIndex = 4;
            this.w_listItemsView.ViewFields = (FieldCollection)null;
            this.panelMarque.Controls.Add((Control)this.labelLoading);
            this.panelMarque.Controls.Add((Control)this.marqueeBar1);
            this.panelMarque.Dock = DockStyle.Bottom;
            this.panelMarque.Location = new Point(0, 219);
            this.panelMarque.Name = "panelMarque";
            this.panelMarque.Size = new Size(668, 15);
            this.panelMarque.TabIndex = 6;
            this.labelLoading.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.labelLoading.AutoSize = true;
            this.labelLoading.BackColor = SystemColors.Control;
            this.labelLoading.ImeMode = ImeMode.NoControl;
            this.labelLoading.Location = new Point(545, -1);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new Size(45, 13);
            this.labelLoading.TabIndex = 3;
            this.labelLoading.Text = "Loading";
            this.labelLoading.TextAlign = ContentAlignment.MiddleCenter;
            this.marqueeBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.marqueeBar1.ForeColor = SystemColors.ControlLight;
            this.marqueeBar1.Location = new Point(592, -1);
            this.marqueeBar1.Name = "marqueeBar1";
            this.marqueeBar1.ShapeSpacing = 8;
            this.marqueeBar1.Size = new Size(75, 15);
            this.marqueeBar1.TabIndex = 4;
            this.marqueeBar1.Text = "marqueeBar1";
            this.w_VersionHistoryView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.w_VersionHistoryView.BackColor = Color.White;
            this.w_VersionHistoryView.DataConverter = (IDataConverter<object, string>)null;
            this.w_VersionHistoryView.DataSource = (ListItemCollection)null;
            this.w_VersionHistoryView.Filter = (ItemViewFilter)null;
            this.w_VersionHistoryView.Location = new Point(-1, 6);
            this.w_VersionHistoryView.MultiSelect = true;
            this.w_VersionHistoryView.Name = "w_VersionHistoryView";
            this.w_VersionHistoryView.ShowPropertyGrid = false;
            this.w_VersionHistoryView.Size = new Size(670, 205);
            this.w_VersionHistoryView.TabIndex = 1;
            this.w_VersionHistoryView.ViewFields = (FieldCollection)null;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ListItemControl = (ItemCollectionView)this.w_listItemsView;
            this.ListItemVersionControl = (ItemCollectionView)this.w_VersionHistoryView;
            this.Name = nameof(STItemsViewControlFull);
            this.w_toolStrip.ResumeLayout(false);
            this.w_toolStrip.PerformLayout();
            this.panelMarque.ResumeLayout(false);
            this.panelMarque.PerformLayout();
            this.ResumeLayout(false);
        }

        protected virtual void On_btnShowPropertyGrids_Click(object sender, EventArgs e)
        {
            this.ShowingPropertyGrids = !this.ShowingPropertyGrids;
        }

        protected virtual void On_btnShowVersionHistory_Click(object sender, EventArgs e)
        {
            if (!this.ShowingVersionHistory && this.ListItemControl.SelectedObjects is ListItemCollection selectedObjects)
            {
                // ISSUE: reference to a compiler-generated method
                this.UpdateVersioningAsync(selectedObjects);
            }
            this.ShowingVersionHistory = !this.ShowingVersionHistory;
        }

        private void On_ColumnSelector_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\n' && e.KeyChar != '\r')
                return;
            this.w_tsButtonQuickSearch.DropDown.Close();
            e.Handled = true;
        }

        private void On_ColumnSelector_SelectedChanged(object sender, EventArgs e)
        {
            this.SetSearchCues();
        }

        protected virtual void On_ItemsViewSelectionChanged(ListItemCollection selectedItems)
        {
            if (this.ShowingVersionHistory)
            {
                // ISSUE: reference to a compiler-generated method
                this.UpdateVersioningAsync(selectedItems);
            }
            // ISSUE: reference to a compiler-generated method
            this.FireSelectedListItemsChanged(selectedItems);
        }

        private void On_tsButtonRefresh_Click(object sender, EventArgs e)
        {
            if (this.DataSource == null)
                return;
            Folder parentFolder = this.DataSource.ParentFolder;
            this.ClearItemsView();
            this.SetDataSourceAsync(parentFolder);
        }

        private void On_tsTextBoxQuickSeach_Left(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.QuickQuery))
                return;
            this.m_bShowingSearchCues = true;
            this.SetSearchCues();
        }

        private void On_tsTextBoxQuickSearch_Entered(object sender, EventArgs e)
        {
            if (!this.m_bShowingSearchCues)
                return;
            this.m_bSuspendQuickSearchChangedEvents = true;
            this.w_tsTextBoxQuickSearch.Text = "";
            this.w_tsTextBoxQuickSearch.Font = new Font(this.w_tsTextBoxQuickSearch.Font, FontStyle.Regular);
            this.w_tsTextBoxQuickSearch.ForeColor = SystemColors.WindowText;
            this.m_bShowingSearchCues = false;
            this.m_bSuspendQuickSearchChangedEvents = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData != Keys.Return && keyData != Keys.Return)
                {
                    if (keyData == Keys.Escape && this.m_filterState != 0)
                    {
                        this.SetFilterState(STItemsViewControlFull.QuickFilterState.Clean);
                        return true;
                    }
                }
                else if (this.m_filterState == STItemsViewControlFull.QuickFilterState.Query)
                {
                    this.SetFilterState(STItemsViewControlFull.QuickFilterState.Filter);
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            catch (Exception ex)
            {
                GlobalServices.ErrorHandler.HandleException(ex);
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        public void SetDataSourceAsync(Folder folder) => this.FetchItems((object)folder);

        private void SetFilterState(STItemsViewControlFull.QuickFilterState state)
        {
            if (state == this.m_filterState)
                return;
            this.m_filterState = state;
            if (state != 0)
            {
                switch (state)
                {
                    case STItemsViewControlFull.QuickFilterState.Query:
                        // ISSUE: reference to a compiler-generated field
                        this.w_tsButtonQuickSearch.Image = this.imageListQuickSearch.Images[0];
                        this.w_tsButtonQuickSearch.Text = "Quick search";
                        break;
                    case STItemsViewControlFull.QuickFilterState.Filter:
                        this.w_listItemsView.Filter = new ItemViewFilter(this.QuickQuery, this.QuickQueryColumn);
                        // ISSUE: reference to a compiler-generated field
                        this.w_tsButtonQuickSearch.Image = this.imageListQuickSearch.Images[1];
                        this.w_tsButtonQuickSearch.Text = "Cancel search";
                        break;
                }
            }
            else
            {
                this.w_listItemsView.Filter = (ItemViewFilter)null;
                // ISSUE: reference to a compiler-generated field
                this.w_tsButtonQuickSearch.Image = this.imageListQuickSearch.Images[0];
                this.w_tsButtonQuickSearch.Text = "Quick search";
                this.w_tsTextBoxQuickSearch.Text = (string)null;
                if (!this.w_tsTextBoxQuickSearch.Focused)
                {
                    this.m_bShowingSearchCues = true;
                    this.SetSearchCues();
                }
            }
        }

        private void SetSearchCues()
        {
            if (this.w_tsColumnSelector.SelectedItem == null || !this.m_bShowingSearchCues)
                return;
            this.m_bSuspendQuickSearchChangedEvents = true;
            this.w_tsTextBoxQuickSearch.Text = string.Format(Metalogix.UI.WinForms.Properties.Resources.Search_In_Column, (object)(this.w_tsColumnSelector.SelectedItem as Field).DisplayName);
            this.w_tsTextBoxQuickSearch.Font = new Font(this.w_tsTextBoxQuickSearch.Font, FontStyle.Italic);
            this.w_tsTextBoxQuickSearch.ForeColor = SystemColors.GrayText;
            this.m_bSuspendQuickSearchChangedEvents = false;
        }

        public void SetStatus(ItemCollectionViewStatus status)
        {
            if (!this.InvokeRequired)
            {
                this.w_listItemsView.Enabled = status != 0;
                this.panelMarque.Visible = status == ItemCollectionViewStatus.Loading;
            }
            else
                this.Invoke((Delegate)new StatusChangedEventHandler(this.SetStatus), (object)status);
        }

        private void toolStripButtonQuickSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.m_filterState == STItemsViewControlFull.QuickFilterState.Query)
                {
                    this.SetFilterState(STItemsViewControlFull.QuickFilterState.Filter);
                }
                else
                {
                    if (this.m_filterState != STItemsViewControlFull.QuickFilterState.Filter)
                        return;
                    this.SetFilterState(STItemsViewControlFull.QuickFilterState.Clean);
                }
            }
            catch (Exception ex)
            {
                GlobalServices.ErrorHandler.HandleException(ex);
            }
        }

        private void toolStripTextBoxQuickSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.m_bSuspendQuickSearchChangedEvents)
                return;
            try
            {
                this.SetFilterState(this.QuickQuery.Length > 0 ? STItemsViewControlFull.QuickFilterState.Query : STItemsViewControlFull.QuickFilterState.Clean);
            }
            catch (Exception ex)
            {
                GlobalServices.ErrorHandler.HandleException(ex);
            }
        }

        private void UpdateQueryColumnSelector()
        {
            this.w_tsColumnSelector.Items.Clear();
            if (this.ViewFields == null)
                return;
            foreach (Field viewField in this.ViewFields)
                this.w_tsColumnSelector.Items.Add((object)viewField);
            if (this.w_tsColumnSelector.Items.Count > 0)
                this.w_tsColumnSelector.SelectedIndex = 0;
        }

        protected override void UpdateVersioningAsync(ListItemCollection itemsSelected)
        {
            // ISSUE: reference to a compiler-generated method
            this.UpdateVersioning((object)itemsSelected);
        }

        private delegate void FillItemsViewDelegate(
          ListItemCollection items,
          FieldCollection viewFields);

        private enum QuickFilterState
        {
            Clean,
            Query,
            Filter,
        }
    }
}
