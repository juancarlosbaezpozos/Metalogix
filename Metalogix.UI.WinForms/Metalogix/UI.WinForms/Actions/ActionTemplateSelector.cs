using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class ActionTemplateSelector : XtraForm
    {
        private readonly bool _isSimplifiedMode;

        private ActionOptionsProvider _provider;

        private Type _actionType;

        private BindingList<ActionOptionsTemplate> _dataSource;

        private ActionOptionsProvider.ActionTemplateDelegate _templateAddedHandler;

        private ActionOptionsProvider.ActionTemplatesDelegate _templateDeletedHandler;

        private ActionOptionsProvider.ActionTemplatesClearedDelegate _templatesClearedHandler;

        private IContainer components;

        private ContextMenuStrip _templateContextMenu;

        private ToolStripMenuItem _exportContextMenuItem;

        private ToolStripMenuItem _deleteContextMenuItem;

        private SimpleButton _importButton;

        private SimpleButton _okayButton;

        private SimpleButton _cancelButton;

        private GridControl _gridControl;

        private GridView _gridView;

        private GridColumn _nameColumn;

        public ActionOptionsTemplate SelectedTemplate { get; private set; }

        public ActionTemplateSelector(ActionOptionsProvider provider, Type actionType, bool isSimplifiedMode = false)
        {
            _isSimplifiedMode = isSimplifiedMode;
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (actionType == null)
            {
                throw new ArgumentNullException("actionType");
            }
            InitializeComponent();
            if (isSimplifiedMode)
            {
                ApplyBasicModeSkin();
            }
            _provider = provider;
            _actionType = actionType;
            LoadUI();
            InitializeProviderEventLinks();
        }

        private void _gridControl_DoubleClick(object sender, EventArgs e)
        {
            ActionOptionsTemplate itemUnderMouse = GetItemUnderMouse();
            if (itemUnderMouse != null)
            {
                SelectedTemplate = itemUnderMouse;
                if (!OpenJobConfigurationWarning())
                {
                    base.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void _importButton_Click(object sender, EventArgs e)
        {
            try
            {
                ActionTemplateUIHelper.ImportTemplate(_provider, _actionType);
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void _okayButton_Click(object sender, EventArgs e)
        {
            List<ActionOptionsTemplate> currentlySelectedTemplates = GetCurrentlySelectedTemplates();
            if (currentlySelectedTemplates.Count == 0)
            {
                FlatXtraMessageBox.Show("No job configuration selected.");
                return;
            }
            SelectedTemplate = currentlySelectedTemplates[0];
            if (!OpenJobConfigurationWarning())
            {
                base.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _templateContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = GetItemUnderMouse() == null;
        }

        private void _templateList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItemWithConfirmation();
            }
        }

        private void ActionTemplateSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            _provider.TemplateAdded -= _templateAddedHandler;
            _provider.TemplatesDeleted -= _templateDeletedHandler;
            _provider.TemplatesCleared -= _templatesClearedHandler;
        }

        private void ApplyBasicModeSkin()
        {
            _importButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            _importButton.LookAndFeel.UseDefaultLookAndFeel = false;
            _okayButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            _okayButton.LookAndFeel.UseDefaultLookAndFeel = false;
            _cancelButton.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            _cancelButton.LookAndFeel.UseDefaultLookAndFeel = false;
        }

        private void DeleteSelectedItemWithConfirmation()
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedItemWithConfirmation();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
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
                if (_gridView.GetRow(num) is ActionOptionsTemplate row)
                {
                    actionOptionsTemplates.Add(row);
                }
            }
            return actionOptionsTemplates;
        }

        private ActionOptionsTemplate GetItemUnderMouse()
        {
            Point client = _gridView.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo gridHitInfo = _gridView.CalcHitInfo(client);
            if (gridHitInfo.RowHandle < 0)
            {
                return null;
            }
            return _gridView.GetRow(gridHitInfo.RowHandle) as ActionOptionsTemplate;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Actions.ActionTemplateSelector));
            this._templateContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._exportContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._importButton = new DevExpress.XtraEditors.SimpleButton();
            this._okayButton = new DevExpress.XtraEditors.SimpleButton();
            this._cancelButton = new DevExpress.XtraEditors.SimpleButton();
            this._gridControl = new DevExpress.XtraGrid.GridControl();
            this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._nameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._templateContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._gridView).BeginInit();
            base.SuspendLayout();
            System.Windows.Forms.ToolStripItemCollection items = this._templateContextMenu.Items;
            System.Windows.Forms.ToolStripItem[] toolStripItemArray = new System.Windows.Forms.ToolStripItem[2] { this._exportContextMenuItem, this._deleteContextMenuItem };
            items.AddRange(toolStripItemArray);
            this._templateContextMenu.Name = "contextMenuStrip1";
            componentResourceManager.ApplyResources(this._templateContextMenu, "_templateContextMenu");
            this._templateContextMenu.Opening += new System.ComponentModel.CancelEventHandler(_templateContextMenu_Opening);
            this._exportContextMenuItem.Image = Metalogix.UI.WinForms.Properties.Resources.Export;
            this._exportContextMenuItem.Name = "_exportContextMenuItem";
            componentResourceManager.ApplyResources(this._exportContextMenuItem, "_exportContextMenuItem");
            this._exportContextMenuItem.Click += new System.EventHandler(exportToolStripMenuItem_Click);
            this._deleteContextMenuItem.Image = Metalogix.UI.WinForms.Properties.Resources.Delete1;
            this._deleteContextMenuItem.Name = "_deleteContextMenuItem";
            componentResourceManager.ApplyResources(this._deleteContextMenuItem, "_deleteContextMenuItem");
            this._deleteContextMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
            componentResourceManager.ApplyResources(this._importButton, "_importButton");
            this._importButton.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("_importButton.Appearance.BackColor");
            this._importButton.Appearance.Options.UseBackColor = true;
            this._importButton.LookAndFeel.SkinName = "Office 2013";
            this._importButton.Name = "_importButton";
            this._importButton.Click += new System.EventHandler(_importButton_Click);
            componentResourceManager.ApplyResources(this._okayButton, "_okayButton");
            this._okayButton.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("_okayButton.Appearance.BackColor");
            this._okayButton.Appearance.Options.UseBackColor = true;
            this._okayButton.LookAndFeel.SkinName = "Office 2013";
            this._okayButton.Name = "_okayButton";
            this._okayButton.Click += new System.EventHandler(_okayButton_Click);
            componentResourceManager.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("_cancelButton.Appearance.BackColor");
            this._cancelButton.Appearance.Options.UseBackColor = true;
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.LookAndFeel.SkinName = "Office 2013";
            this._cancelButton.Name = "_cancelButton";
            componentResourceManager.ApplyResources(this._gridControl, "_gridControl");
            this._gridControl.ContextMenuStrip = this._templateContextMenu;
            this._gridControl.MainView = this._gridView;
            this._gridControl.Name = "_gridControl";
            this._gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridView });
            this._gridControl.DoubleClick += new System.EventHandler(_gridControl_DoubleClick);
            this._gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this._nameColumn });
            this._gridView.GridControl = this._gridControl;
            this._gridView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
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
            this._gridView.OptionsView.ShowColumnHeaders = false;
            this._gridView.OptionsView.ShowDetailButtons = false;
            this._gridView.OptionsView.ShowGroupPanel = false;
            this._gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this._gridView.OptionsView.ShowIndicator = false;
            this._gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this._gridView.RowHeight = 32;
            componentResourceManager.ApplyResources(this._nameColumn, "_nameColumn");
            this._nameColumn.FieldName = "TemplateName";
            this._nameColumn.Name = "_nameColumn";
            base.AcceptButton = this._okayButton;
            base.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("ActionTemplateSelector.Appearance.BackColor");
            base.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this._cancelButton;
            base.Controls.Add(this._gridControl);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._okayButton);
            base.Controls.Add(this._importButton);
            base.LookAndFeel.SkinName = "Office 2013";
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ActionTemplateSelector";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(ActionTemplateSelector_FormClosed);
            this._templateContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._gridView).EndInit();
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
            ActionOptionsTemplate[] optionsTemplatesForAction = _provider.GetOptionsTemplatesForAction(_actionType);
            _dataSource = new BindingList<ActionOptionsTemplate>();
            ActionOptionsTemplate[] actionOptionsTemplateArray = optionsTemplatesForAction;
            foreach (ActionOptionsTemplate actionOptionsTemplate in actionOptionsTemplateArray)
            {
                _dataSource.Add(actionOptionsTemplate);
            }
            _gridControl.DataSource = _dataSource;
        }

        private void On_ActionTemplateAdded(ActionOptionsTemplate template)
        {
            if (base.InvokeRequired)
            {
                ActionOptionsProvider.ActionTemplateDelegate actionTemplateDelegate = On_ActionTemplateAdded;
                object[] objArray = new object[1] { template };
                Invoke(actionTemplateDelegate, objArray);
            }
            else if (template.ActionTypeName == _actionType.FullName)
            {
                _dataSource.Add(template);
                int rowHandle = _gridView.GetRowHandle(_dataSource.Count - 1);
                _gridView.ClearSelection();
                _gridView.SelectRow(rowHandle);
                _gridView.FocusedRowHandle = rowHandle;
                _gridView.TopRowIndex = _gridView.GetVisibleIndex(rowHandle);
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
            _gridControl.BeginUpdate();
            try
            {
                foreach (ActionOptionsTemplate actionOptionsTemplate in templates)
                {
                    _dataSource.Remove(actionOptionsTemplate);
                }
            }
            finally
            {
                _gridControl.EndUpdate();
            }
        }

        private bool OpenJobConfigurationWarning()
        {
            string confirmActionTemplateApplication = Metalogix.Actions.Properties.Resources.ConfirmActionTemplateApplication;
            if (_isSimplifiedMode && !SelectedTemplate.TemplateName.EndsWith(UIUtils.SimplifiedMode, StringComparison.InvariantCultureIgnoreCase))
            {
                object[] newLine = new object[4]
                {
                    confirmActionTemplateApplication,
                    Environment.NewLine,
                    Environment.NewLine,
                    Metalogix.Actions.Properties.Resources.ConfirmActionTemplateApplication_AdvancedToSimplified
                };
                confirmActionTemplateApplication = string.Format("{0}{1}{2}Note: {3}", newLine);
            }
            if (FlatXtraMessageBox.Show(confirmActionTemplateApplication, Metalogix.Actions.Properties.Resources.ApplyConfiguration, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return true;
            }
            return false;
        }
    }
}
