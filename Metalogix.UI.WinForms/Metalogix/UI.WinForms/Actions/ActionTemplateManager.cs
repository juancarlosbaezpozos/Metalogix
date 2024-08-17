using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Metalogix.Actions;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class ActionTemplateManager : XtraForm
    {
        private class ActionTemplateWrapper
        {
            public string Action => ActionTemplateUIHelper.GetActionDisplayName(Template.ActionTypeName);

            public string Name => Template.TemplateName;

            public ActionOptionsTemplate Template { get; set; }

            public ActionTemplateWrapper()
            {
            }

            public ActionTemplateWrapper(ActionOptionsTemplate template)
            {
                Template = template;
            }
        }

        private ActionOptionsProvider _provider;

        private BindingList<ActionTemplateWrapper> _dataSource;

        private ActionOptionsProvider.ActionTemplateDelegate _templateAddedHandler;

        private ActionOptionsProvider.ActionTemplatesDelegate _templateDeletedHandler;

        private ActionOptionsProvider.ActionTemplatesClearedDelegate _templatesClearedHandler;

        private IContainer components;

        private Panel _bottomPanel;

        private SimpleButton _closeButton;

        private ContextMenuStrip _templateContextMenu;

        private ToolStripMenuItem _exportContextButton;

        private ToolStripMenuItem _deleteContextButton;

        private GridControl _gridControl;

        private GridView _gridView;

        private GridColumn _nameColumn;

        private GridColumn _actionColumn;

        private GridColumn _nameGridColumn;

        private BarManager _barManager;

        private BarButtonItem _importButton;

        private BarButtonItem _exportButton;

        private BarButtonItem _deleteButton;

        private BarEditItem _filterInput;

        private RepositoryItemTextEdit _filterTextEdit;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private RepositoryItemTextEdit repositoryItemTextEdit1;

        private BarButtonItem _filterButton;

        private Bar _menuBar;

        private BarStaticItem _filterLabel;

        public ActionTemplateManager(ActionOptionsProvider provider)
        {
            InitializeComponent();
            _provider = provider;
            LoadUI();
            InitializeProviderEventLinks();
        }

        private void _closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _deleteButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void _deleteContextButton_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void _exportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            ExportSelected();
        }

        private void _exportContextButton_Click(object sender, EventArgs e)
        {
            ExportSelected();
        }

        private void _filterButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            UpdateFilter();
        }

        private void _gridControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItems();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void _gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                return;
            }
            int[] selectedRows = _gridView.GetSelectedRows();
            foreach (int num in selectedRows)
            {
                if (_gridView.IsGroupRow(num))
                {
                    _gridView.UnselectRow(num);
                }
            }
        }

        private void _importButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ActionTemplateUIHelper.ImportTemplate(_provider);
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void _templateContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !GetMouseOverTemplate();
        }

        private void ActionTemplateManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            _provider.TemplateAdded -= _templateAddedHandler;
            _provider.TemplatesDeleted -= _templateDeletedHandler;
            _provider.TemplatesCleared -= _templatesClearedHandler;
        }

        private void ActionTemplateManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ActionTemplateManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdateFilter();
                e.Handled = true;
                e.SuppressKeyPress = true;
                _filterInput.Links[0].Focus();
            }
        }

        private ActionTemplateWrapper AddTemplateToView(ActionOptionsTemplate template)
        {
            ActionTemplateWrapper actionTemplateWrapper = new ActionTemplateWrapper(template);
            _dataSource.Add(actionTemplateWrapper);
            return actionTemplateWrapper;
        }

        private void DeleteSelectedItems()
        {
            try
            {
                ActionTemplateUIHelper.DeleteTemplates(GetCurrentlySelectedTemplates());
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

        private string EscapeFilterString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            StringBuilder stringBuilder = new StringBuilder(value.Length * 3);
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '%' || value[i] == '_' || value[i] == '[')
                {
                    stringBuilder.Append("[" + value[i] + "]");
                }
                else
                {
                    stringBuilder.Append(value[i]);
                }
            }
            return stringBuilder.ToString();
        }

        private void ExportSelected()
        {
            try
            {
                ActionTemplateUIHelper.ExportTemplate(GetCurrentlySelectedTemplates());
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private List<ActionOptionsTemplate> GetCurrentlySelectedTemplates()
        {
            int[] selectedRows = _gridView.GetSelectedRows();
            if (selectedRows == null)
            {
                return new List<ActionOptionsTemplate>();
            }
            List<ActionOptionsTemplate> actionOptionsTemplates = new List<ActionOptionsTemplate>(selectedRows.Length);
            int[] numArray = selectedRows;
            foreach (int num in numArray)
            {
                if (!_gridView.IsGroupRow(num) && _gridView.GetRow(num) is ActionTemplateWrapper row)
                {
                    actionOptionsTemplates.Add(row.Template);
                }
            }
            return actionOptionsTemplates;
        }

        private bool GetMouseOverTemplate()
        {
            Point client = _gridView.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo gridHitInfo = _gridView.CalcHitInfo(client);
            if (!gridHitInfo.InRow)
            {
                return false;
            }
            return !_gridView.IsGroupRow(gridHitInfo.RowHandle);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._bottomPanel = new System.Windows.Forms.Panel();
            this._closeButton = new DevExpress.XtraEditors.SimpleButton();
            this._templateContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._exportContextButton = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteContextButton = new System.Windows.Forms.ToolStripMenuItem();
            this._gridControl = new DevExpress.XtraGrid.GridControl();
            this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._actionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._barManager = new DevExpress.XtraBars.BarManager(this.components);
            this._menuBar = new DevExpress.XtraBars.Bar();
            this._importButton = new DevExpress.XtraBars.BarButtonItem();
            this._exportButton = new DevExpress.XtraBars.BarButtonItem();
            this._deleteButton = new DevExpress.XtraBars.BarButtonItem();
            this._filterLabel = new DevExpress.XtraBars.BarStaticItem();
            this._filterInput = new DevExpress.XtraBars.BarEditItem();
            this._filterTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this._filterButton = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this._bottomPanel.SuspendLayout();
            this._templateContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._gridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._filterTextEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
            base.SuspendLayout();
            this._bottomPanel.Controls.Add(this._closeButton);
            this._bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._bottomPanel.Location = new System.Drawing.Point(0, 381);
            this._bottomPanel.Name = "_bottomPanel";
            this._bottomPanel.Size = new System.Drawing.Size(434, 41);
            this._bottomPanel.TabIndex = 3;
            this._closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this._closeButton.Appearance.BackColor = System.Drawing.Color.White;
            this._closeButton.Appearance.Options.UseBackColor = true;
            this._closeButton.Location = new System.Drawing.Point(347, 6);
            this._closeButton.LookAndFeel.SkinName = "Office 2013";
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 5;
            this._closeButton.Text = "&Close";
            this._closeButton.Click += new System.EventHandler(_closeButton_Click);
            System.Windows.Forms.ToolStripItemCollection items = this._templateContextMenu.Items;
            System.Windows.Forms.ToolStripItem[] toolStripItemArray = new System.Windows.Forms.ToolStripItem[2] { this._exportContextButton, this._deleteContextButton };
            items.AddRange(toolStripItemArray);
            this._templateContextMenu.Name = "_templateContextMenu";
            this._templateContextMenu.Size = new System.Drawing.Size(108, 48);
            this._templateContextMenu.Opening += new System.ComponentModel.CancelEventHandler(_templateContextMenu_Opening);
            this._exportContextButton.Image = Metalogix.UI.WinForms.Properties.Resources.Export;
            this._exportContextButton.Name = "_exportContextButton";
            this._exportContextButton.Size = new System.Drawing.Size(107, 22);
            this._exportContextButton.Text = "Export";
            this._exportContextButton.Click += new System.EventHandler(_exportContextButton_Click);
            this._deleteContextButton.Image = Metalogix.UI.WinForms.Properties.Resources.Delete1;
            this._deleteContextButton.Name = "_deleteContextButton";
            this._deleteContextButton.Size = new System.Drawing.Size(107, 22);
            this._deleteContextButton.Text = "Delete";
            this._deleteContextButton.Click += new System.EventHandler(_deleteContextButton_Click);
            this._gridControl.ContextMenuStrip = this._templateContextMenu;
            this._gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridControl.EmbeddedNavigator.ShowToolTips = false;
            this._gridControl.Location = new System.Drawing.Point(0, 26);
            this._gridControl.MainView = this._gridView;
            this._gridControl.Name = "_gridControl";
            this._gridControl.Size = new System.Drawing.Size(434, 355);
            this._gridControl.TabIndex = 4;
            this._gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridView });
            this._gridControl.KeyUp += new System.Windows.Forms.KeyEventHandler(_gridControl_KeyUp);
            DevExpress.XtraGrid.Columns.GridColumnCollection columns = this._gridView.Columns;
            DevExpress.XtraGrid.Columns.GridColumn[] gridColumnArray = new DevExpress.XtraGrid.Columns.GridColumn[2] { this._nameGridColumn, this._actionColumn };
            columns.AddRange(gridColumnArray);
            this._gridView.GridControl = this._gridControl;
            this._gridView.GroupCount = 1;
            this._gridView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this._gridView.GroupFormat = "{1}";
            this._gridView.Name = "_gridView";
            this._gridView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this._gridView.OptionsBehavior.Editable = false;
            this._gridView.OptionsCustomization.AllowColumnMoving = false;
            this._gridView.OptionsCustomization.AllowColumnResizing = false;
            this._gridView.OptionsCustomization.AllowFilter = false;
            this._gridView.OptionsCustomization.AllowGroup = false;
            this._gridView.OptionsCustomization.AllowSort = false;
            this._gridView.OptionsDetail.AllowZoomDetail = false;
            this._gridView.OptionsDetail.EnableMasterViewMode = false;
            this._gridView.OptionsDetail.ShowDetailTabs = false;
            this._gridView.OptionsDetail.SmartDetailExpand = false;
            this._gridView.OptionsFilter.AllowColumnMRUFilterList = false;
            this._gridView.OptionsHint.ShowCellHints = false;
            this._gridView.OptionsHint.ShowColumnHeaderHints = false;
            this._gridView.OptionsHint.ShowFooterHints = false;
            this._gridView.OptionsMenu.EnableColumnMenu = false;
            this._gridView.OptionsMenu.EnableFooterMenu = false;
            this._gridView.OptionsMenu.EnableGroupPanelMenu = false;
            this._gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this._gridView.OptionsSelection.MultiSelect = true;
            this._gridView.OptionsView.ShowColumnHeaders = false;
            this._gridView.OptionsView.ShowDetailButtons = false;
            this._gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this._gridView.OptionsView.ShowGroupPanel = false;
            this._gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this._gridView.OptionsView.ShowIndicator = false;
            this._gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            DevExpress.XtraGrid.Columns.GridColumnSortInfoCollection sortInfo = this._gridView.SortInfo;
            DevExpress.XtraGrid.Columns.GridColumnSortInfo[] gridColumnSortInfo = new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
            {
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this._actionColumn, DevExpress.Data.ColumnSortOrder.Ascending),
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this._nameGridColumn, DevExpress.Data.ColumnSortOrder.Ascending)
            };
            sortInfo.AddRange(gridColumnSortInfo);
            this._gridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(_gridView_SelectionChanged);
            this._nameGridColumn.Caption = "Template Name";
            this._nameGridColumn.FieldName = "Name";
            this._nameGridColumn.Name = "_nameGridColumn";
            this._nameGridColumn.Visible = true;
            this._nameGridColumn.VisibleIndex = 0;
            this._actionColumn.Caption = "Action";
            this._actionColumn.FieldName = "Action";
            this._actionColumn.Name = "_actionColumn";
            this._barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this._menuBar });
            this._barManager.DockControls.Add(this.barDockControlTop);
            this._barManager.DockControls.Add(this.barDockControlBottom);
            this._barManager.DockControls.Add(this.barDockControlLeft);
            this._barManager.DockControls.Add(this.barDockControlRight);
            this._barManager.Form = this;
            DevExpress.XtraBars.BarItems barItem = this._barManager.Items;
            DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[6] { this._importButton, this._exportButton, this._deleteButton, this._filterInput, this._filterButton, this._filterLabel };
            barItem.AddRange(barItemArray);
            this._barManager.MainMenu = this._menuBar;
            this._barManager.MaxItemId = 10;
            DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._barManager.RepositoryItems;
            DevExpress.XtraEditors.Repository.RepositoryItem[] repositoryItemArray = new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemTextEdit1, this._filterTextEdit };
            repositoryItems.AddRange(repositoryItemArray);
            this._menuBar.BarItemHorzIndent = 3;
            this._menuBar.BarItemVertIndent = 3;
            this._menuBar.BarName = "Custom 3";
            this._menuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this._menuBar.DockCol = 0;
            this._menuBar.DockRow = 0;
            this._menuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this._menuBar.FloatLocation = new System.Drawing.Point(481, 154);
            DevExpress.XtraBars.LinksInfo linksPersistInfo = this._menuBar.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[6]
            {
                new DevExpress.XtraBars.LinkPersistInfo(this._importButton),
                new DevExpress.XtraBars.LinkPersistInfo(this._exportButton),
                new DevExpress.XtraBars.LinkPersistInfo(this._deleteButton, true),
                new DevExpress.XtraBars.LinkPersistInfo(this._filterLabel),
                new DevExpress.XtraBars.LinkPersistInfo(this._filterInput),
                new DevExpress.XtraBars.LinkPersistInfo(this._filterButton)
            };
            linksPersistInfo.AddRange(linkPersistInfo);
            this._menuBar.OptionsBar.AllowQuickCustomization = false;
            this._menuBar.OptionsBar.DrawBorder = false;
            this._menuBar.OptionsBar.DrawDragBorder = false;
            this._menuBar.OptionsBar.UseWholeRow = true;
            this._menuBar.Text = "MenuBar";
            this._importButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this._importButton.AllowRightClickInMenu = false;
            this._importButton.Caption = "Import";
            this._importButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Import;
            this._importButton.Id = 0;
            this._importButton.Name = "_importButton";
            this._importButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this._importButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_importButton_ItemClick);
            this._exportButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this._exportButton.AllowRightClickInMenu = false;
            this._exportButton.Caption = "Export";
            this._exportButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Export;
            this._exportButton.Id = 1;
            this._exportButton.Name = "_exportButton";
            this._exportButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this._exportButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_exportButton_ItemClick);
            this._deleteButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this._deleteButton.AllowRightClickInMenu = false;
            this._deleteButton.Caption = "Delete";
            this._deleteButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Delete1;
            this._deleteButton.Id = 2;
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this._deleteButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_deleteButton_ItemClick);
            this._filterLabel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this._filterLabel.AllowRightClickInMenu = false;
            this._filterLabel.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._filterLabel.Caption = "Filter:";
            this._filterLabel.Id = 9;
            this._filterLabel.Name = "_filterLabel";
            this._filterLabel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this._filterLabel.TextAlignment = System.Drawing.StringAlignment.Near;
            this._filterInput.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this._filterInput.AllowRightClickInMenu = false;
            this._filterInput.AutoHideEdit = false;
            this._filterInput.Caption = "_filterInput";
            this._filterInput.Edit = this._filterTextEdit;
            this._filterInput.Id = 5;
            this._filterInput.Name = "_filterInput";
            this._filterInput.Width = 107;
            this._filterTextEdit.AutoHeight = false;
            this._filterTextEdit.Name = "_filterTextEdit";
            this._filterButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this._filterButton.Caption = "Filter";
            this._filterButton.Glyph = Metalogix.UI.WinForms.Properties.Resources.Filter2;
            this._filterButton.Id = 8;
            this._filterButton.Name = "_filterButton";
            this._filterButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_filterButton_ItemClick);
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(434, 26);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 422);
            this.barDockControlBottom.Size = new System.Drawing.Size(434, 0);
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 26);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 396);
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(434, 26);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 396);
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(434, 422);
            base.Controls.Add(this._gridControl);
            base.Controls.Add(this._bottomPanel);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.KeyPreview = true;
            base.LookAndFeel.SkinName = "Office 2013";
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(395, 112);
            base.Name = "ActionTemplateManager";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Job Configurations";
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(ActionTemplateManager_FormClosed);
            base.KeyDown += new System.Windows.Forms.KeyEventHandler(ActionTemplateManager_KeyDown);
            base.KeyUp += new System.Windows.Forms.KeyEventHandler(ActionTemplateManager_KeyUp);
            this._bottomPanel.ResumeLayout(false);
            this._templateContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._gridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._filterTextEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeProviderEventLinks()
        {
            _templateAddedHandler = On_ActionTemplateAdded;
            _templateDeletedHandler = On_ActionTemplatesDeleted;
            _templatesClearedHandler = On_ActionTemplatesCleared;
            _provider.TemplateAdded += _templateAddedHandler;
            _provider.TemplatesDeleted += _templateDeletedHandler;
            _provider.TemplatesCleared += _templatesClearedHandler;
        }

        private void LoadUI()
        {
            ActionTemplateWrapper[] array = (from t in _provider.GetAllOptionsTemplates()
                select new ActionTemplateWrapper(t)).ToArray();
            _dataSource = new BindingList<ActionTemplateWrapper>();
            ActionTemplateWrapper[] actionTemplateWrapperArray = array;
            foreach (ActionTemplateWrapper actionTemplateWrapper in actionTemplateWrapperArray)
            {
                _dataSource.Add(actionTemplateWrapper);
            }
            _gridControl.BeginUpdate();
            try
            {
                _gridControl.DataSource = _dataSource;
            }
            finally
            {
                _gridControl.EndUpdate();
            }
        }

        private void On_ActionTemplateAdded(ActionOptionsTemplate template)
        {
            if (base.InvokeRequired)
            {
                ActionOptionsProvider.ActionTemplateDelegate actionTemplateDelegate = On_ActionTemplateAdded;
                object[] objArray = new object[1] { template };
                Invoke(actionTemplateDelegate, objArray);
                return;
            }
            _gridControl.BeginUpdate();
            try
            {
                ActionTemplateWrapper view = AddTemplateToView(template);
                int rowHandle = _gridView.GetRowHandle(_dataSource.IndexOf(view));
                _gridView.MakeRowVisible(rowHandle);
                _gridView.ClearSelection();
                _gridView.SelectRow(rowHandle);
                _gridView.FocusedRowHandle = rowHandle;
                _gridView.TopRowIndex = _gridView.GetVisibleIndex(rowHandle);
            }
            finally
            {
                _gridControl.EndUpdate();
            }
        }

        private void On_ActionTemplatesCleared()
        {
            if (!base.InvokeRequired)
            {
                _dataSource.Clear();
            }
            else
            {
                Invoke(new ActionOptionsProvider.ActionTemplatesClearedDelegate(On_ActionTemplatesCleared));
            }
        }

        private void On_ActionTemplatesDeleted(ActionOptionsTemplate[] templates)
        {
            if (base.InvokeRequired)
            {
                Invoke(new ActionOptionsProvider.ActionTemplatesDelegate(On_ActionTemplatesDeleted), templates);
                return;
            }
            ActionTemplateWrapper[] array = (from w in _dataSource
                from t in templates
                where w.Template.StorageKey == t.StorageKey
                select w).ToArray();
            _gridControl.BeginUpdate();
            try
            {
                ActionTemplateWrapper[] actionTemplateWrapperArray = array;
                foreach (ActionTemplateWrapper actionTemplateWrapper in actionTemplateWrapperArray)
                {
                    _dataSource.Remove(actionTemplateWrapper);
                }
            }
            finally
            {
                _gridControl.EndUpdate();
            }
        }

        private void UpdateFilter()
        {
            _gridControl.BeginUpdate();
            try
            {
                string editValue = _filterInput.EditValue as string;
                _gridView.ActiveFilter.Clear();
                if (!string.IsNullOrEmpty(editValue))
                {
                    string str = $"[Name] LIKE '%{EscapeFilterString(editValue)}%'";
                    _gridView.ActiveFilter.Add(_nameGridColumn, new ColumnFilterInfo(str));
                }
            }
            finally
            {
                _gridControl.EndUpdate();
            }
        }
    }
}
