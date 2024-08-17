using Metalogix.Data.Mapping;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public partial class ListPickerControl : UserControl
    {
        private bool m_bShowSource = true;

        private IContainer components;

        private SplitContainer w_splitContainerH;

        private Panel panelTop;

        private ToolStrip w_toolStrip;

        private ListGroupControl w_listGroupControlTop;

        private Panel panelBottom;

        private ListGroupControl w_listGroupControlBottom;

        private ToolStripButton w_toolStripButtonDelete;

        private GroupBox groupBox;

        private ToolStripButton w_toolStripButtonAdd;

        public object[] Items
        {
            get
            {
                return this.w_listGroupControlTop.Items;
            }
            set
            {
                this.w_listGroupControlTop.Items = value;
            }
        }

        public ListPickerItem[] SelectedItems
        {
            get
            {
                List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
                ListPickerItem[] items = this.w_listGroupControlBottom.GetItems();
                for (int i = 0; i < (int)items.Length; i++)
                {
                    listPickerItems.Add(items[i]);
                }
                return listPickerItems.ToArray();
            }
            set
            {
                ListPickerItem[] listPickerItemArray = value;
                this.w_listGroupControlBottom.ClearItems();
                if (listPickerItemArray != null)
                {
                    ListPickerItem[] listPickerItemArray1 = listPickerItemArray;
                    for (int i = 0; i < (int)listPickerItemArray1.Length; i++)
                    {
                        ListPickerItem listPickerItem = listPickerItemArray1[i];
                        IListPickerComparer listPickerComparer = null;
                        List<IListPickerComparer>.Enumerator enumerator = ListCache.ListPickerComparers.GetEnumerator();
                        try
                        {
                            do
                            {
                                if (!enumerator.MoveNext())
                                {
                                    break;
                                }
                                IListPickerComparer current = enumerator.Current;
                                if (!current.AppliesTo(listPickerItem, this.w_listGroupControlTop.FirstItem))
                                {
                                    continue;
                                }
                                listPickerComparer = current;
                            }
                            while (listPickerComparer == null);
                        }
                        finally
                        {
                            ((IDisposable)enumerator).Dispose();
                        }
                        this.AddItem(this.w_listGroupControlTop.FindItem(listPickerItem, listPickerComparer));
                    }
                }
            }
        }

        public object SelectedSource
        {
            get
            {
                return this.w_listGroupControlTop.SelectedSource;
            }
            set
            {
                this.w_listGroupControlTop.SelectedSource = value;
            }
        }

        public bool ShowSource
        {
            get
            {
                return this.m_bShowSource;
            }
            set
            {
                this.m_bShowSource = value;
                this.w_listGroupControlTop.ShowSource = this.m_bShowSource;
            }
        }

        public object[] Sources
        {
            get
            {
                return this.w_listGroupControlTop.Sources;
            }
            set
            {
                this.w_listGroupControlTop.Sources = value;
            }
        }

        public ListPickerControl()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public void AddCustomColumn(string columnName, string headerText)
        {
            this.w_listGroupControlTop.AddCustomColumn(columnName, headerText);
            this.w_listGroupControlBottom.AddCustomColumn(columnName, headerText);
        }

        private void AddItem(ListPickerItem source)
        {
            if (source == null)
            {
                return;
            }
            this.w_listGroupControlTop.DeleteItem(source);
            this.w_listGroupControlBottom.AddItem(source);
        }

        private void DeleteItem(ListPickerItem target)
        {
            if (target == null)
            {
                return;
            }
            this.w_listGroupControlBottom.DeleteItem(target);
            this.w_listGroupControlTop.AddItem(target);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            this.w_listGroupControlTop.OnSourceChanged += new SourceChangedEventHandler(this.w_listGroupControlTop_OnSourceChanged);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ListPickerControl));
            this.w_splitContainerH = new SplitContainer();
            this.panelTop = new Panel();
            this.w_listGroupControlTop = new ListGroupControl();
            this.panelBottom = new Panel();
            this.w_listGroupControlBottom = new ListGroupControl();
            this.w_toolStrip = new ToolStrip();
            this.w_toolStripButtonAdd = new ToolStripButton();
            this.w_toolStripButtonDelete = new ToolStripButton();
            this.groupBox = new GroupBox();
            this.w_splitContainerH.Panel1.SuspendLayout();
            this.w_splitContainerH.Panel2.SuspendLayout();
            this.w_splitContainerH.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.w_toolStrip.SuspendLayout();
            this.groupBox.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_splitContainerH, "w_splitContainerH");
            this.w_splitContainerH.Name = "w_splitContainerH";
            this.w_splitContainerH.Panel1.Controls.Add(this.panelTop);
            this.w_splitContainerH.Panel2.Controls.Add(this.panelBottom);
            this.w_splitContainerH.Panel2.Controls.Add(this.w_toolStrip);
            this.panelTop.Controls.Add(this.w_listGroupControlTop);
            componentResourceManager.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            componentResourceManager.ApplyResources(this.w_listGroupControlTop, "w_listGroupControlTop");
            this.w_listGroupControlTop.Items = null;
            this.w_listGroupControlTop.MultiSelect = true;
            this.w_listGroupControlTop.Name = "w_listGroupControlTop";
            this.w_listGroupControlTop.SelectedSource = null;
            this.w_listGroupControlTop.ShowSource = true;
            this.w_listGroupControlTop.Sources = null;
            this.panelBottom.Controls.Add(this.w_listGroupControlBottom);
            componentResourceManager.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            componentResourceManager.ApplyResources(this.w_listGroupControlBottom, "w_listGroupControlBottom");
            this.w_listGroupControlBottom.Items = null;
            this.w_listGroupControlBottom.MultiSelect = true;
            this.w_listGroupControlBottom.Name = "w_listGroupControlBottom";
            this.w_listGroupControlBottom.SelectedSource = null;
            this.w_listGroupControlBottom.ShowSource = false;
            this.w_listGroupControlBottom.Sources = null;
            componentResourceManager.ApplyResources(this.w_toolStrip, "w_toolStrip");
            this.w_toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            ToolStripItemCollection items = this.w_toolStrip.Items;
            ToolStripItem[] wToolStripButtonAdd = new ToolStripItem[] { this.w_toolStripButtonAdd, this.w_toolStripButtonDelete };
            items.AddRange(wToolStripButtonAdd);
            this.w_toolStrip.Name = "w_toolStrip";
            this.w_toolStrip.RenderMode = ToolStripRenderMode.System;
            componentResourceManager.ApplyResources(this.w_toolStripButtonAdd, "w_toolStripButtonAdd");
            this.w_toolStripButtonAdd.Name = "w_toolStripButtonAdd";
            this.w_toolStripButtonAdd.Click += new EventHandler(this.w_toolStripButtonAdd_Click);
            componentResourceManager.ApplyResources(this.w_toolStripButtonDelete, "w_toolStripButtonDelete");
            this.w_toolStripButtonDelete.Name = "w_toolStripButtonDelete";
            this.w_toolStripButtonDelete.Click += new EventHandler(this.w_toolStripButtonDelete_Click);
            this.groupBox.Controls.Add(this.w_splitContainerH);
            componentResourceManager.ApplyResources(this.groupBox, "groupBox");
            this.groupBox.Name = "groupBox";
            this.groupBox.TabStop = false;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.groupBox);
            base.Name = "ListPickerControl";
            this.w_splitContainerH.Panel1.ResumeLayout(false);
            this.w_splitContainerH.Panel2.ResumeLayout(false);
            this.w_splitContainerH.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.w_toolStrip.ResumeLayout(false);
            this.w_toolStrip.PerformLayout();
            this.groupBox.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void w_listGroupControlTop_OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (this.OnSourceChanged != null)
            {
                this.OnSourceChanged(sender, e);
            }
        }

        private void w_toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            ListPickerItem[] selectedItems = this.w_listGroupControlTop.GetSelectedItems();
            if (selectedItems == null || (int)selectedItems.Length == 0)
            {
                FlatXtraMessageBox.Show("Please select 1 or more source item(s) from the left to add.");
                return;
            }
            if (selectedItems != null)
            {
                Stack<ListPickerItem> listPickerItems = new Stack<ListPickerItem>(selectedItems);
                while (listPickerItems.Count > 0)
                {
                    this.AddItem(listPickerItems.Pop());
                }
            }
        }

        private void w_toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            ListPickerItem[] selectedItems = this.w_listGroupControlBottom.GetSelectedItems();
            if (selectedItems == null || (int)selectedItems.Length == 0)
            {
                FlatXtraMessageBox.Show("Please select 1 or more target item(s) from the right to delete.");
                return;
            }
            if (selectedItems != null)
            {
                Stack<ListPickerItem> listPickerItems = new Stack<ListPickerItem>(selectedItems);
                while (listPickerItems.Count > 0)
                {
                    this.DeleteItem(listPickerItems.Pop());
                }
            }
        }

        public event SourceChangedEventHandler OnSourceChanged;
    }
}