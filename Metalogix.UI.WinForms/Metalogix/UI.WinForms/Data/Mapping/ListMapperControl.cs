using Metalogix;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public partial class ListMapperControl : UserControl
    {
        private bool m_bShowSourceOnSource = true;

        private bool m_bShowSourceOnTarget = true;

        private bool m_bAllowNewSource = true;

        private bool m_bAllowNewTarget = true;

        private bool m_bAllowDeletion = true;

        private bool m_bAllowAutoMap = true;

        private bool m_bRemoveSourceOnMap = true;

        private bool m_bRemoveTargetOnMap = true;

        private bool m_bAllowNewTagCreation;

        private List<IListPickerComparer> _sourceComparers;

        private List<IListPickerComparer> _targetComparers;

        private IContainer components;

        private SplitContainer w_splitContainerH;

        private SplitContainer w_splitContainerV;

        private ListControl w_listPickerControlSource;

        private ListControl w_listPickerControlTarget;

        private ListSummaryControl w_listSummaryControl;

        private Panel w_panelTop;

        private ToolStrip w_toolStripPicker;

        private ToolStrip w_toolStripRemover;

        private GroupBox groupBox;

        private ToolStripButton w_toolStripButtonMap;

        private ToolStripButton w_toolStripButtonUnmap;

        private ToolStripButton w_toolStripButtonNewTarget;

        private ToolStripButton w_toolStripButtonAuto;

        private ToolStripButton w_toolStripButtonDelete;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripButton w_toolStripButtonClear;

        private ToolStripSeparator toolStripSeparator2;

        private ToolStripButton w_toolStripButtonNewSource;

        public bool AllowAutomap
        {
            get
            {
                return this.m_bAllowAutoMap;
            }
            set
            {
                this.m_bAllowAutoMap = value;
                this.w_toolStripButtonAuto.Visible = this.m_bAllowAutoMap;
            }
        }

        public bool AllowDeletion
        {
            get
            {
                return this.m_bAllowDeletion;
            }
            set
            {
                this.m_bAllowDeletion = value;
                this.w_toolStripButtonDelete.Visible = this.m_bAllowDeletion;
            }
        }

        public bool AllowNewSource
        {
            get
            {
                return this.m_bAllowNewSource;
            }
            set
            {
                this.m_bAllowNewSource = value;
                this.w_toolStripButtonNewSource.Visible = this.m_bAllowNewSource;
            }
        }

        public bool AllowNewTagCreation
        {
            get
            {
                return this.m_bAllowNewTagCreation;
            }
            set
            {
                this.w_listPickerControlSource.AllowNewTagCreation = value;
                this.w_listPickerControlTarget.AllowNewTagCreation = value;
                this.m_bAllowNewTagCreation = value;
            }
        }

        public bool AllowNewTarget
        {
            get
            {
                return this.m_bAllowNewTarget;
            }
            set
            {
                this.m_bAllowNewTarget = value;
                this.w_toolStripButtonNewTarget.Visible = this.m_bAllowNewTarget;
            }
        }

        public string NewSourceLabel
        {
            get
            {
                return this.w_toolStripButtonNewSource.Text;
            }
            set
            {
                this.w_toolStripButtonNewSource.Text = value;
                this.w_toolStripRemover.Width = this.w_toolStripPicker.Width;
            }
        }

        public string NewTargetLabel
        {
            get
            {
                return this.w_toolStripButtonNewTarget.Text;
            }
            set
            {
                this.w_toolStripButtonNewTarget.Text = value;
                this.w_toolStripRemover.Width = this.w_toolStripPicker.Width;
            }
        }

        public bool RemoveSourceOnMap
        {
            get
            {
                return this.m_bRemoveSourceOnMap;
            }
            set
            {
                this.m_bRemoveSourceOnMap = value;
            }
        }

        public bool RemoveTargetOnMap
        {
            get
            {
                return this.m_bRemoveTargetOnMap;
            }
            set
            {
                this.m_bRemoveTargetOnMap = value;
            }
        }

        public object SelectedSource
        {
            get
            {
                return this.w_listPickerControlSource.SelectedSource;
            }
            set
            {
                this.w_listPickerControlSource.SelectedSource = value;
            }
        }

        public object SelectedTarget
        {
            get
            {
                return this.w_listPickerControlTarget.SelectedSource;
            }
            set
            {
                this.w_listPickerControlTarget.SelectedSource = value;
            }
        }

        public bool ShowBottomToolStrip
        {
            get
            {
                return this.w_listSummaryControl.ShowBottomToolStrip;
            }
            set
            {
                this.w_listSummaryControl.ShowBottomToolStrip = value;
            }
        }

        public bool ShowGroups
        {
            get
            {
                return this.w_listSummaryControl.ShowGroups;
            }
            set
            {
                this.w_listSummaryControl.ShowGroups = value;
            }
        }

        public bool ShowSourceOnSource
        {
            get
            {
                return this.m_bShowSourceOnSource;
            }
            set
            {
                this.m_bShowSourceOnSource = value;
                this.w_listPickerControlSource.ShowSource = this.m_bShowSourceOnSource;
            }
        }

        public bool ShowSourceOnTarget
        {
            get
            {
                return this.m_bShowSourceOnTarget;
            }
            set
            {
                this.m_bShowSourceOnTarget = value;
                this.w_listPickerControlTarget.ShowSource = this.m_bShowSourceOnTarget;
            }
        }

        public object[] SourceItems
        {
            get
            {
                return this.w_listPickerControlSource.Items;
            }
            set
            {
                this.w_listPickerControlSource.Items = value;
            }
        }

        public object[] SourceNewItems
        {
            get
            {
                return this.w_listPickerControlSource.NewRows.ToArray();
            }
        }

        public object[] Sources
        {
            get
            {
                return this.w_listPickerControlSource.Sources;
            }
            set
            {
                this.w_listPickerControlSource.Sources = value;
            }
        }

        public ListSummaryItem[] SummaryItems
        {
            get
            {
                return this.w_listSummaryControl.Items;
            }
            set
            {
                ListSummaryItem[] listSummaryItemArray = value;
                this.w_listSummaryControl.Clear();
                if (listSummaryItemArray != null && (int)listSummaryItemArray.Length > 0)
                {
                    List<ListMapperControl.MapTuple> mapTuples = new List<ListMapperControl.MapTuple>();
                    ListSummaryItem[] listSummaryItemArray1 = listSummaryItemArray;
                    for (int i = 0; i < (int)listSummaryItemArray1.Length; i++)
                    {
                        ListSummaryItem listSummaryItem = listSummaryItemArray1[i];
                        foreach (IListPickerComparer listPickerComparer in ListCache.ListPickerComparers)
                        {
                            if (listPickerComparer.AppliesTo(listSummaryItem.Source, this.w_listPickerControlSource.FirstItem))
                            {
                                this.AddSourceComparer(listPickerComparer);
                            }
                            if (!listPickerComparer.AppliesTo(listSummaryItem.Target, this.w_listPickerControlTarget.FirstItem))
                            {
                                continue;
                            }
                            this.AddTargetComparer(listPickerComparer);
                        }
                        ListPickerItem source = null;
                        if (listSummaryItem.Source != null)
                        {
                            List<IListPickerComparer>.Enumerator enumerator = this._sourceComparers.GetEnumerator();
                            try
                            {
                                do
                                {
                                    if (!enumerator.MoveNext())
                                    {
                                        break;
                                    }
                                    IListPickerComparer current = enumerator.Current;
                                    source = this.w_listPickerControlSource.FindItem(listSummaryItem.Source, current);
                                }
                                while (source == null);
                            }
                            finally
                            {
                                ((IDisposable)enumerator).Dispose();
                            }
                            if (source == null && listSummaryItem.Source.IsNew)
                            {
                                this.w_listPickerControlSource.AddItem(listSummaryItem.Source);
                                source = listSummaryItem.Source;
                            }
                            if (this.AllowNewTagCreation)
                            {
                                source = listSummaryItem.Source;
                            }
                        }
                        ListPickerItem target = null;
                        if (listSummaryItem.Target != null)
                        {
                            List<IListPickerComparer>.Enumerator enumerator1 = this._targetComparers.GetEnumerator();
                            try
                            {
                                do
                                {
                                    if (!enumerator1.MoveNext())
                                    {
                                        break;
                                    }
                                    IListPickerComparer current1 = enumerator1.Current;
                                    target = this.w_listPickerControlTarget.FindItem(listSummaryItem.Target, current1);
                                }
                                while (target == null);
                            }
                            finally
                            {
                                ((IDisposable)enumerator1).Dispose();
                            }
                            if (target == null && listSummaryItem.Target.IsNew)
                            {
                                this.w_listPickerControlTarget.AddItem(listSummaryItem.Target);
                                target = listSummaryItem.Target;
                            }
                            if (target == null && this.AllowNewTagCreation)
                            {
                                target = listSummaryItem.Target;
                            }
                        }
                        ListMapperControl.MapTuple mapTuple = new ListMapperControl.MapTuple()
                        {
                            CustomColumns = listSummaryItem.CustomColumns,
                            Group = listSummaryItem.Group,
                            Source = source,
                            Target = target
                        };
                        mapTuples.Add(mapTuple);
                    }
                    this.MapRange(mapTuples);
                }
            }
        }

        public object[] TargetItems
        {
            get
            {
                return this.w_listPickerControlTarget.Items;
            }
            set
            {
                this.w_listPickerControlTarget.Items = value;
            }
        }

        public object[] TargetNewItems
        {
            get
            {
                return this.w_listPickerControlTarget.NewRows.ToArray();
            }
        }

        public object[] Targets
        {
            get
            {
                return this.w_listPickerControlTarget.Sources;
            }
            set
            {
                this.w_listPickerControlTarget.Sources = value;
            }
        }

        public ListMapperControl()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public void AddSourceColumn(string columnName, string headerText, bool isReadonly)
        {
            this.w_listPickerControlSource.AddCustomColumn(columnName, headerText, isReadonly);
        }

        public void AddSourceComparer(IListPickerComparer comparer)
        {
            if (comparer == null)
            {
                return;
            }
            if (this._sourceComparers.Contains(comparer))
            {
                return;
            }
            this._sourceComparers.Add(comparer);
        }

        public void AddSourceItem(ListPickerItem item)
        {
            if (item != null)
            {
                this.w_listPickerControlSource.AddItem(item);
            }
        }

        public void AddSummaryColumn(string columnName, string headerText)
        {
            this.w_listSummaryControl.AddCustomColumn(columnName, headerText);
        }

        public void AddTargetColumn(string columnName, string headerText, bool isReadonly)
        {
            this.w_listPickerControlTarget.AddCustomColumn(columnName, headerText, isReadonly);
        }

        public void AddTargetComparer(IListPickerComparer comparer)
        {
            if (comparer == null)
            {
                return;
            }
            if (this._targetComparers.Contains(comparer))
            {
                return;
            }
            this._targetComparers.Add(comparer);
        }

        public void AddTargetItem(object oItem)
        {
            if (oItem != null)
            {
                this.w_listPickerControlTarget.AddItem(oItem);
            }
        }

        public void AddTargetItem(ListPickerItem item)
        {
            this.AddTargetItem(item, false);
        }

        public void AddTargetItem(ListPickerItem item, bool bReadOnly)
        {
            if (item != null)
            {
                this.w_listPickerControlTarget.AddItem(item, bReadOnly);
            }
        }

        public void DisableSort()
        {
            this.w_listSummaryControl.DisableSort();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void EnableSort()
        {
            this.w_listSummaryControl.EnableSort();
        }

        public void FilterSource(IListFilter filter)
        {
            if (filter != null)
            {
                this.w_listPickerControlSource.Filter(filter);
            }
        }

        public void FilterTarget(IListFilter filter)
        {
            if (filter != null)
            {
                this.w_listPickerControlTarget.Filter(filter);
            }
        }

        private bool GetSelectionCanBeDeleted()
        {
            bool flag;
            DataGridViewSelectedRowCollection selectedRows = this.w_listPickerControlTarget.LocalDataGridView.SelectedRows;
            if (selectedRows.Count == 0)
            {
                return false;
            }
            IEnumerator enumerator = selectedRows.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (((DataGridViewRow)enumerator.Current).DefaultCellStyle.BackColor == Color.AliceBlue)
                    {
                        continue;
                    }
                    flag = false;
                    return flag;
                }
                return true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return flag;
        }

        private void Initialize()
        {
            this._sourceComparers = new List<IListPickerComparer>();
            this._targetComparers = new List<IListPickerComparer>();
            this.w_listPickerControlSource.OnSourceChanged += new SourceChangedEventHandler(this.w_listPickerControlSource_OnSourceChanged);
            this.w_listPickerControlTarget.OnSourceChanged += new SourceChangedEventHandler(this.w_listPickerControlTarget_OnSourceChanged);
            this.w_listSummaryControl.OnListSummaryItemAdded += new ListSummaryItemEventHandler(this.w_listSummaryControl_OnListSummaryItemAdded);
            this.w_listSummaryControl.OnListSummaryItemRemoved += new ListSummaryItemEventHandler(this.w_listSummaryControl_OnListSummaryItemRemoved);
            this.w_toolStripRemover.Width = this.w_toolStripPicker.Width;
            this.UpdateUI();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ListMapperControl));
            this.w_splitContainerH = new SplitContainer();
            this.w_panelTop = new Panel();
            this.w_splitContainerV = new SplitContainer();
            this.w_listPickerControlSource = new ListControl();
            this.w_listPickerControlTarget = new ListControl();
            this.w_toolStripPicker = new ToolStrip();
            this.w_toolStripButtonMap = new ToolStripButton();
            this.w_toolStripButtonAuto = new ToolStripButton();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.w_toolStripButtonNewSource = new ToolStripButton();
            this.w_toolStripButtonNewTarget = new ToolStripButton();
            this.w_toolStripButtonDelete = new ToolStripButton();
            this.w_listSummaryControl = new ListSummaryControl();
            this.w_toolStripRemover = new ToolStrip();
            this.w_toolStripButtonUnmap = new ToolStripButton();
            this.w_toolStripButtonClear = new ToolStripButton();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.groupBox = new GroupBox();
            this.w_splitContainerH.Panel1.SuspendLayout();
            this.w_splitContainerH.Panel2.SuspendLayout();
            this.w_splitContainerH.SuspendLayout();
            this.w_panelTop.SuspendLayout();
            this.w_splitContainerV.Panel1.SuspendLayout();
            this.w_splitContainerV.Panel2.SuspendLayout();
            this.w_splitContainerV.SuspendLayout();
            this.w_toolStripPicker.SuspendLayout();
            this.w_toolStripRemover.SuspendLayout();
            this.groupBox.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_splitContainerH, "w_splitContainerH");
            this.w_splitContainerH.Name = "w_splitContainerH";
            this.w_splitContainerH.Panel1.Controls.Add(this.w_panelTop);
            this.w_splitContainerH.Panel1.Controls.Add(this.w_toolStripPicker);
            this.w_splitContainerH.Panel2.Controls.Add(this.w_listSummaryControl);
            this.w_splitContainerH.Panel2.Controls.Add(this.w_toolStripRemover);
            this.w_panelTop.Controls.Add(this.w_splitContainerV);
            componentResourceManager.ApplyResources(this.w_panelTop, "w_panelTop");
            this.w_panelTop.Name = "w_panelTop";
            componentResourceManager.ApplyResources(this.w_splitContainerV, "w_splitContainerV");
            this.w_splitContainerV.Name = "w_splitContainerV";
            this.w_splitContainerV.Panel1.Controls.Add(this.w_listPickerControlSource);
            this.w_splitContainerV.Panel2.Controls.Add(this.w_listPickerControlTarget);
            this.w_listPickerControlSource.AllowEdit = false;
            this.w_listPickerControlSource.AllowNewTagCreation = false;
            this.w_listPickerControlSource.BackColor = Color.White;
            this.w_listPickerControlSource.BorderStyle = BorderStyle.FixedSingle;
            this.w_listPickerControlSource.CurrentFilter = null;
            this.w_listPickerControlSource.CurrentView = null;
            componentResourceManager.ApplyResources(this.w_listPickerControlSource, "w_listPickerControlSource");
            this.w_listPickerControlSource.ForeColor = SystemColors.ControlText;
            this.w_listPickerControlSource.Items = null;
            this.w_listPickerControlSource.MultiSelect = true;
            this.w_listPickerControlSource.Name = "w_listPickerControlSource";
            this.w_listPickerControlSource.SelectedSource = null;
            this.w_listPickerControlSource.ShowSource = false;
            this.w_listPickerControlSource.Sources = null;
            this.w_listPickerControlTarget.AllowEdit = false;
            this.w_listPickerControlTarget.AllowNewTagCreation = false;
            this.w_listPickerControlTarget.BackColor = Color.White;
            this.w_listPickerControlTarget.BorderStyle = BorderStyle.FixedSingle;
            this.w_listPickerControlTarget.CurrentFilter = null;
            this.w_listPickerControlTarget.CurrentView = null;
            componentResourceManager.ApplyResources(this.w_listPickerControlTarget, "w_listPickerControlTarget");
            this.w_listPickerControlTarget.ForeColor = SystemColors.ControlText;
            this.w_listPickerControlTarget.Items = null;
            this.w_listPickerControlTarget.MultiSelect = true;
            this.w_listPickerControlTarget.Name = "w_listPickerControlTarget";
            this.w_listPickerControlTarget.SelectedSource = null;
            this.w_listPickerControlTarget.ShowSource = true;
            this.w_listPickerControlTarget.Sources = null;
            this.w_listPickerControlTarget.OnSelectionChanged += new SourceChangedEventHandler(this.w_listPickerControlTarget_OnSelectionChanged);
            componentResourceManager.ApplyResources(this.w_toolStripPicker, "w_toolStripPicker");
            this.w_toolStripPicker.GripStyle = ToolStripGripStyle.Hidden;
            ToolStripItemCollection items = this.w_toolStripPicker.Items;
            ToolStripItem[] wToolStripButtonMap = new ToolStripItem[] { this.w_toolStripButtonMap, this.w_toolStripButtonAuto, this.toolStripSeparator1, this.w_toolStripButtonNewSource, this.w_toolStripButtonNewTarget, this.w_toolStripButtonDelete };
            items.AddRange(wToolStripButtonMap);
            this.w_toolStripPicker.Name = "w_toolStripPicker";
            this.w_toolStripPicker.RenderMode = ToolStripRenderMode.System;
            this.w_toolStripButtonMap.Image = Resources.Map16;
            componentResourceManager.ApplyResources(this.w_toolStripButtonMap, "w_toolStripButtonMap");
            this.w_toolStripButtonMap.Name = "w_toolStripButtonMap";
            this.w_toolStripButtonMap.Click += new EventHandler(this.w_toolStripButtonMap_Click);
            componentResourceManager.ApplyResources(this.w_toolStripButtonAuto, "w_toolStripButtonAuto");
            this.w_toolStripButtonAuto.Name = "w_toolStripButtonAuto";
            this.w_toolStripButtonAuto.Click += new EventHandler(this.w_toolStripButtonAuto_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            componentResourceManager.ApplyResources(this.w_toolStripButtonNewSource, "w_toolStripButtonNewSource");
            this.w_toolStripButtonNewSource.Name = "w_toolStripButtonNewSource";
            this.w_toolStripButtonNewSource.Click += new EventHandler(this.w_toolStripButtonNewSource_Click);
            componentResourceManager.ApplyResources(this.w_toolStripButtonNewTarget, "w_toolStripButtonNewTarget");
            this.w_toolStripButtonNewTarget.Name = "w_toolStripButtonNewTarget";
            this.w_toolStripButtonNewTarget.Click += new EventHandler(this.w_toolStripButtonNewTarget_Click);
            this.w_toolStripButtonDelete.Image = Resources.Delete16;
            componentResourceManager.ApplyResources(this.w_toolStripButtonDelete, "w_toolStripButtonDelete");
            this.w_toolStripButtonDelete.Name = "w_toolStripButtonDelete";
            this.w_toolStripButtonDelete.Click += new EventHandler(this.w_toolStripButtonDelete_Click);
            this.w_listSummaryControl.BackColor = Color.White;
            this.w_listSummaryControl.BorderStyle = BorderStyle.FixedSingle;
            componentResourceManager.ApplyResources(this.w_listSummaryControl, "w_listSummaryControl");
            this.w_listSummaryControl.Items = new ListSummaryItem[0];
            this.w_listSummaryControl.MultiSelect = true;
            this.w_listSummaryControl.Name = "w_listSummaryControl";
            this.w_listSummaryControl.ShowBottomToolStrip = false;
            this.w_listSummaryControl.ShowGroups = true;
            componentResourceManager.ApplyResources(this.w_toolStripRemover, "w_toolStripRemover");
            this.w_toolStripRemover.GripStyle = ToolStripGripStyle.Hidden;
            ToolStripItemCollection toolStripItemCollections = this.w_toolStripRemover.Items;
            ToolStripItem[] wToolStripButtonUnmap = new ToolStripItem[] { this.w_toolStripButtonUnmap, this.w_toolStripButtonClear, this.toolStripSeparator2 };
            toolStripItemCollections.AddRange(wToolStripButtonUnmap);
            this.w_toolStripRemover.Name = "w_toolStripRemover";
            this.w_toolStripRemover.RenderMode = ToolStripRenderMode.System;
            this.w_toolStripButtonUnmap.Image = Resources.Map16;
            componentResourceManager.ApplyResources(this.w_toolStripButtonUnmap, "w_toolStripButtonUnmap");
            this.w_toolStripButtonUnmap.Name = "w_toolStripButtonUnmap";
            this.w_toolStripButtonUnmap.Click += new EventHandler(this.w_toolStripButtonUnmap_Click);
            this.w_toolStripButtonClear.Image = Resources.Delete16;
            componentResourceManager.ApplyResources(this.w_toolStripButtonClear, "w_toolStripButtonClear");
            this.w_toolStripButtonClear.Name = "w_toolStripButtonClear";
            this.w_toolStripButtonClear.Click += new EventHandler(this.w_toolStripButtonClear_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            componentResourceManager.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.groupBox.BackColor = Color.White;
            this.groupBox.Controls.Add(this.w_splitContainerH);
            componentResourceManager.ApplyResources(this.groupBox, "groupBox");
            this.groupBox.Name = "groupBox";
            this.groupBox.TabStop = false;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.groupBox);
            base.Name = "ListMapperControl";
            this.w_splitContainerH.Panel1.ResumeLayout(false);
            this.w_splitContainerH.Panel1.PerformLayout();
            this.w_splitContainerH.Panel2.ResumeLayout(false);
            this.w_splitContainerH.ResumeLayout(false);
            this.w_panelTop.ResumeLayout(false);
            this.w_splitContainerV.Panel1.ResumeLayout(false);
            this.w_splitContainerV.Panel2.ResumeLayout(false);
            this.w_splitContainerV.ResumeLayout(false);
            this.w_toolStripPicker.ResumeLayout(false);
            this.w_toolStripPicker.PerformLayout();
            this.w_toolStripRemover.ResumeLayout(false);
            this.w_toolStripRemover.PerformLayout();
            this.groupBox.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        public void Map(ListPickerItem source, ListPickerItem target)
        {
            this.Map(source, target, null, null);
        }

        public void Map(ListPickerItem source, ListPickerItem target, string group, CommonSerializableTable<string, object> customColumns)
        {
            if (source == null || target == null)
            {
                return;
            }
            ListSummaryItem listSummaryItem = new ListSummaryItem();
            if (!string.IsNullOrEmpty(group))
            {
                listSummaryItem.Group = group;
            }
            else if (source.Group == target.Group)
            {
                listSummaryItem.Group = source.Group;
            }
            listSummaryItem.Source = source;
            listSummaryItem.Target = target;
            listSummaryItem.CustomColumns = customColumns;
            this.w_listSummaryControl.Add(listSummaryItem);
            if (this.RemoveSourceOnMap)
            {
                this.w_listPickerControlSource.HideItem(listSummaryItem.Source);
            }
            if (this.RemoveTargetOnMap)
            {
                this.w_listPickerControlTarget.HideItem(listSummaryItem.Target);
            }
        }

        public void MapRange(IList<ListMapperControl.MapTuple> mappings)
        {
            List<ListSummaryItem> listSummaryItems = new List<ListSummaryItem>(mappings.Count);
            string group = null;
            foreach (ListMapperControl.MapTuple mapping in mappings)
            {
                if (mapping.Source == null || mapping.Target == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(mapping.Group))
                {
                    group = mapping.Group;
                }
                else if (mapping.Source.Group == mapping.Target.Group)
                {
                    group = mapping.Source.Group;
                }
                ListSummaryItem listSummaryItem = new ListSummaryItem()
                {
                    CustomColumns = mapping.CustomColumns,
                    Group = group,
                    Source = mapping.Source,
                    Target = mapping.Target
                };
                listSummaryItems.Add(listSummaryItem);
                if (this.RemoveSourceOnMap)
                {
                    this.w_listPickerControlSource.HideItem(mapping.Source);
                }
                if (!this.RemoveTargetOnMap)
                {
                    continue;
                }
                this.w_listPickerControlTarget.HideItem(mapping.Target);
            }
            try
            {
                this.w_listSummaryControl.SuspendLayout();
                this.w_listSummaryControl.DisableSort();
                this.w_listSummaryControl.AddRange(listSummaryItems);
            }
            finally
            {
                this.w_listSummaryControl.EnableSort();
                this.w_listSummaryControl.ResumeLayout();
            }
        }

        public void RemoveSourceColumn(string columnName)
        {
            this.w_listPickerControlSource.RemoveCustomColumn(columnName);
        }

        public void RemoveTargetColumn(string columnName)
        {
            this.w_listPickerControlTarget.RemoveCustomColumn(columnName);
        }

        public void RenderViewSource(IListView view)
        {
            if (view != null)
            {
                this.w_listPickerControlSource.RenderView(view);
            }
        }

        public void RenderViewSummary(IListView view)
        {
            if (view != null)
            {
                this.w_listSummaryControl.RenderView(view);
            }
        }

        public void RenderViewTarget(IListView view)
        {
            if (view != null)
            {
                this.w_listPickerControlTarget.RenderView(view);
            }
        }

        public void Unmap(ListSummaryItem item)
        {
            if (item != null)
            {
                this.w_listSummaryControl.Remove(item);
                if (this.RemoveSourceOnMap)
                {
                    this.w_listPickerControlSource.ShowItem(item.Source);
                }
                if (this.RemoveTargetOnMap)
                {
                    this.w_listPickerControlTarget.ShowItem(item.Target);
                }
            }
        }

        public void Unmap(IEnumerable<ListSummaryItem> items)
        {
            List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
            List<ListPickerItem> listPickerItems1 = new List<ListPickerItem>();
            foreach (ListSummaryItem listSummaryItem in 
                from item in items
                where item != null
                select item)
            {
                if (this.RemoveSourceOnMap)
                {
                    listPickerItems.Add(listSummaryItem.Source);
                }
                if (this.RemoveTargetOnMap)
                {
                    listPickerItems1.Add(listSummaryItem.Target);
                }
                this.w_listSummaryControl.Remove(listSummaryItem);
            }
            this.w_listPickerControlSource.ShowItems(listPickerItems);
            this.w_listPickerControlTarget.ShowItems(listPickerItems1);
        }

        private void UpdateUI()
        {
            this.w_toolStripButtonDelete.Enabled = this.GetSelectionCanBeDeleted();
        }

        public void UpdateUserIcon()
        {
            this.w_toolStripButtonNewSource.Image = Resources.CreateNewUser16;
        }

        private void w_listPickerControlSource_OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (this.OnSourceChanged != null)
            {
                this.OnSourceChanged(sender, e);
            }
        }

        private void w_listPickerControlTarget_OnSelectionChanged(object sender, SourceChangedEventArgs e)
        {
            this.UpdateUI();
        }

        private void w_listPickerControlTarget_OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (this.OnTargetChanged != null)
            {
                this.OnTargetChanged(sender, e);
            }
        }

        private void w_listSummaryControl_OnListSummaryItemAdded(object sender, ListSummaryItemEventArgs e)
        {
            if (this.OnMapped != null)
            {
                this.OnMapped(sender, e);
            }
        }

        private void w_listSummaryControl_OnListSummaryItemRemoved(object sender, ListSummaryItemEventArgs e)
        {
            if (this.OnUnmapped != null)
            {
                this.OnUnmapped(sender, e);
            }
        }

        private void w_toolStripButtonAuto_Click(object sender, EventArgs e)
        {
            try
            {
                List<IListPickerComparer> listPickerComparers = new List<IListPickerComparer>();
                ListPickerItem[] selectedItems = this.w_listPickerControlSource.GetSelectedItems();
                for (int i = 0; i < (int)selectedItems.Length; i++)
                {
                    ListPickerItem listPickerItem = selectedItems[i];
                    foreach (IListPickerComparer listPickerComparer in ListCache.ListPickerComparers)
                    {
                        if (listPickerComparer.AppliesTo(listPickerItem, this.w_listPickerControlTarget.FirstItem) && !listPickerComparers.Contains(listPickerComparer))
                        {
                            listPickerComparers.Add(listPickerComparer);
                        }
                        if (!listPickerComparer.AppliesTo(listPickerItem, this.w_listPickerControlTarget.LastItem) || listPickerComparers.Contains(listPickerComparer))
                        {
                            continue;
                        }
                        listPickerComparers.Add(listPickerComparer);
                    }
                    ListPickerItem listPickerItem1 = listPickerItem;
                    ListPickerItem listPickerItem2 = null;
                    List<IListPickerComparer>.Enumerator enumerator = listPickerComparers.GetEnumerator();
                    try
                    {
                        do
                        {
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }
                            IListPickerComparer current = enumerator.Current;
                            listPickerItem2 = this.w_listPickerControlTarget.FindItem(listPickerItem, current, true);
                        }
                        while (listPickerItem2 == null);
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    this.Map(listPickerItem1, listPickerItem2, null, null);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void w_toolStripButtonClear_Click(object sender, EventArgs e)
        {
            ListSummaryItem[] items = this.w_listSummaryControl.Items;
            if (items == null || (int)items.Length == 0)
            {
                return;
            }
            if (FlatXtraMessageBox.Show("Are you sure you want to clear all visible mappings?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Unmap(items);
            }
            if (this.OnClearClicked != null)
            {
                EventArgs eventArg = new EventArgs();
                this.OnClearClicked(this, eventArg);
            }
        }

        private void w_toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            ListPickerItem[] selectedItems = this.w_listPickerControlTarget.GetSelectedItems();
            if ((int)selectedItems.Length > 0)
            {
                if (!this.GetSelectionCanBeDeleted())
                {
                    FlatXtraMessageBox.Show("Cannot delete selected target items because 1 or more item(s) is not a new item.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (FlatXtraMessageBox.Show("Are you sure you want to delete the selected target items?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ListPickerItem[] listPickerItemArray = selectedItems;
                    for (int i = 0; i < (int)listPickerItemArray.Length; i++)
                    {
                        ListPickerItem listPickerItem = listPickerItemArray[i];
                        this.w_listPickerControlTarget.DeleteItem(listPickerItem);
                    }
                }
            }
        }

        private void w_toolStripButtonMap_Click(object sender, EventArgs e)
        {
            ListPickerItem[] selectedItems = this.w_listPickerControlSource.GetSelectedItems();
            ListPickerItem[] listPickerItemArray = this.w_listPickerControlTarget.GetSelectedItems();
            if ((int)listPickerItemArray.Length > 1)
            {
                FlatXtraMessageBox.Show("Please select only 1 target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if ((int)selectedItems.Length > 0 && (int)listPickerItemArray.Length == 1)
            {
                if ((int)selectedItems.Length > 1 && this.RemoveTargetOnMap)
                {
                    FlatXtraMessageBox.Show("Please select only 1 source.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                ListPickerItem[] listPickerItemArray1 = selectedItems;
                for (int i = 0; i < (int)listPickerItemArray1.Length; i++)
                {
                    ListPickerItem listPickerItem = listPickerItemArray1[i];
                    this.Map(listPickerItem, listPickerItemArray[0], null, null);
                }
            }
        }

        private void w_toolStripButtonNewSource_Click(object sender, EventArgs e)
        {
            this.w_listPickerControlSource.SelectItem(null);
            ListPickerItem[] selectedItems = this.w_listPickerControlTarget.GetSelectedItems();
            if (this.OnNewSourceButtonClicked != null)
            {
                this.OnNewSourceButtonClicked(sender, selectedItems);
            }
        }

        private void w_toolStripButtonNewTarget_Click(object sender, EventArgs e)
        {
            this.w_listPickerControlTarget.SelectItem(null);
            ListPickerItem[] selectedItems = this.w_listPickerControlSource.GetSelectedItems();
            if (this.OnNewTargetButtonClicked != null)
            {
                this.OnNewTargetButtonClicked(sender, selectedItems);
            }
        }

        private void w_toolStripButtonUnmap_Click(object sender, EventArgs e)
        {
            ListSummaryItem[] selectedItems = this.w_listSummaryControl.GetSelectedItems();
            for (int i = 0; i < (int)selectedItems.Length; i++)
            {
                this.Unmap(selectedItems[i]);
            }
        }

        public event EventHandler OnClearClicked;

        public event ListSummaryItemEventHandler OnMapped;

        public event ListMapperControl.NewButtonClickedEventHandler OnNewSourceButtonClicked;

        public event ListMapperControl.NewButtonClickedEventHandler OnNewTargetButtonClicked;

        public event SourceChangedEventHandler OnSourceChanged;

        public event SourceChangedEventHandler OnTargetChanged;

        public event ListSummaryItemEventHandler OnUnmapped;

        public struct MapTuple
        {
            public CommonSerializableTable<string, object> CustomColumns
            {
                get;
                set;
            }

            public string Group
            {
                get;
                set;
            }

            public ListPickerItem Source
            {
                get;
                set;
            }

            public ListPickerItem Target
            {
                get;
                set;
            }
        }

        public delegate void NewButtonClickedEventHandler(object sender, ListPickerItem[] items);
    }
}