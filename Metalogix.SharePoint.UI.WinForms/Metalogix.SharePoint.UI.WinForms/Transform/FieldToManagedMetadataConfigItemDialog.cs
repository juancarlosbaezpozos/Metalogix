using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Transformers;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Data.Filters;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Transform
{
    public class FieldToManagedMetadataConfigItemDialog : XtraForm
    {
        private TransformerConfigContext _context;

        private BindingList<BindableSubstitutionValues> _dataSource = new BindingList<BindableSubstitutionValues>();

        protected DXMenuItem _addItem = new DXMenuItem("Add New Row");

        protected DXMenuItem _deleteitem = new DXMenuItem("Delete Selected Rows");

        private bool _updatingValueInCode;

        private bool _updatedValueFromDialog;

        private bool _isNew;

        private bool _loaded;

        private bool _modified;

        private readonly bool _readOnly;

        private SPTermStoreCollection _tsCollection;

        private IContainer components;

        private SimpleButton _btnCancel;

        private SimpleButton _btnOk;

        private ComboBoxEdit _cbTermSet;

        private ComboBoxEdit _cbGroup;

        private ComboBoxEdit _cbTermStore;

        private LabelControl _lblTermStore;

        private LabelControl _lblFieldName;

        private LabelControl _lblListName;

        private LabelControl _lblTermGroup;

        private LabelControl _lblTermSet;

        private CheckEdit _cbCreateNewField;

        private SimpleButton _btnEditListFilter;

        private SimpleButton _btnEditListFieldFilter;

        private TextEdit _txtNewFieldName;

        private TextEdit _txtNewDisplayName;

        private LabelControl _lblNewName;

        private LabelControl _lblNewDisplayName;

        private PanelControl _pnlWait;

        private LabelControl _lblLoading;

        private LabelControl _lblAnchor;

        private TextEdit _txtAnchor;

        private GroupControl _groupTarget;

        private GroupControl _groupSource;

        private LabelControl _lblSubstitute;

        private TextEdit _txtListName;

        private TextEdit _txtFieldName;

        private XtraTabControl _tabControl;

        private XtraTabPage _pageTransform;

        private XtraTabPage _pageSubstitute;

        private HelpTipButton _helpCreateNewField;

        private HelpTipButton _helpAnchor;

        private HelpTipButton _helpTermSet;

        private HelpTipButton _helpGroup;

        private GridControl _gridSubstitute;

        private GridView _gridViewSubstitute;

        private GridColumn _valueColumn;

        private GridColumn _substituteColumn;

        private RepositoryItemButtonEdit _btnEditValue;

        private RepositoryItemTextEdit _txtEditSubstitute;

        private GridColumn _filterColumn;

        private LabelControl _lblTransform;

        private PictureEdit _pbInfo;

        public FieldToManagedMetadataOption Option { get; set; }

        private SPTermStoreCollection TermStoreCollection
        {
            get
            {
                if (_tsCollection == null)
                {
                    if (_context.ActionContext.Targets[0] is SPServer)
                    {
                        _tsCollection = (_context.ActionContext.Targets[0] as SPServer).TermStores;
                    }
                    else if (_context.ActionContext.Targets[0] is SPTenant)
                    {
                        _tsCollection = (_context.ActionContext.Targets[0] as SPTenant).TermStores;
                    }
                    else if (_context.ActionContext.Targets[0] is SPWeb)
                    {
                        _tsCollection = (_context.ActionContext.Targets[0] as SPWeb).TermStores;
                    }
                    else if (_context.ActionContext.Targets[0] is SPList)
                    {
                        if ((_context.ActionContext.Targets[0] as SPList).ParentWeb != null)
                        {
                            _tsCollection = (_context.ActionContext.Targets[0] as SPList).ParentWeb.TermStores;
                        }
                    }
                    else if (_context.ActionContext.Targets[0] is SPFolder && (_context.ActionContext.Targets[0] as SPFolder).ParentList != null && (_context.ActionContext.Targets[0] as SPFolder).ParentList.ParentWeb != null)
                    {
                        _tsCollection = (_context.ActionContext.Targets[0] as SPFolder).ParentList.ParentWeb.TermStores;
                    }
                    if (_tsCollection == null)
                    {
                        throw new ArgumentNullException("Unable to obtain Target Termstores. _tsCollection is null.");
                    }
                }
                return _tsCollection;
            }
        }

        public FieldToManagedMetadataConfigItemDialog(bool isNewItem, TransformerConfigContext context, FieldToManagedMetadataOption option, bool readOnly)
        {
            _readOnly = readOnly;
            _isNew = isNewItem;
            Option = option;
            _context = context;
            BaseEdit.DefaultErrorIconAlignment = ErrorIconAlignment.MiddleRight;
            InitializeComponent();
            Type type = GetType();
            _helpGroup.SetResourceString(type.FullName + _cbGroup.Name, type);
            _helpTermSet.SetResourceString(type.FullName + _cbTermSet.Name, type);
            _helpAnchor.SetResourceString(type.FullName + _txtAnchor.Name, type);
            _helpCreateNewField.SetResourceString(type.FullName + _cbCreateNewField.Name, type);
            _pbInfo.Visible = _readOnly;
            _pbInfo.Properties.ShowMenu = false;
            if (_readOnly)
            {
                _pbInfo.ToolTip = string.Format(Resources.FS_UnableToEditConfiguration, Resources.SiteColumnToMMDTransformerName, Environment.NewLine, Resources.FMMDCEditing);
                _btnOk.Enabled = false;
                _btnEditListFieldFilter.Enabled = false;
                _btnEditListFilter.Enabled = false;
                _cbCreateNewField.Enabled = false;
                _cbTermStore.Enabled = false;
                _cbGroup.Enabled = false;
                _cbTermSet.Enabled = false;
                _txtAnchor.Enabled = false;
                _txtNewDisplayName.Enabled = false;
                _txtNewFieldName.Enabled = false;
                _gridViewSubstitute.OptionsBehavior.Editable = false;
            }
            _pnlWait.Dock = DockStyle.Fill;
            _pnlWait.BringToFront();
            _lblLoading.Text = string.Format(Resources.FS_LoadingConfiguation, Environment.NewLine);
            _cbCreateNewField.Checked = false;
            _txtNewFieldName.Enabled = false;
            _txtNewDisplayName.Enabled = false;
            _txtNewFieldName.Properties.ReadOnly = true;
            _txtNewDisplayName.Properties.ReadOnly = true;
            _cbGroup.Properties.MaxLength = 250;
            _cbTermSet.Properties.MaxLength = 250;
            _txtAnchor.Properties.MaxLength = 250;
            _txtNewDisplayName.Properties.MaxLength = 250;
            _txtNewFieldName.Properties.MaxLength = 250;
            _cbGroup.Leave += cbGroup_OnGenericChangeOrLeave;
            _cbGroup.SelectedValueChanged += cbGroup_OnGenericChangeOrLeave;
            _cbTermStore.SelectedValueChanged += cbTermStore_OnGenericChangeOrLeave;
            _cbTermStore.Leave += cbTermStore_OnGenericChangeOrLeave;
            _addItem.Click += w_bAddFilter_Click;
            _deleteitem.Click += w_DeleteFilters_Click;
        }

        private void _gridViewSubstitute_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column != _valueColumn)
            {
                return;
            }
            if (_updatedValueFromDialog)
            {
                _updatedValueFromDialog = false;
            }
            else if (!_updatingValueInCode)
            {
                string value = "";
                IFilterExpression filterExpression = null;
                if (!string.IsNullOrEmpty((string)e.Value))
                {
                    filterExpression = CreateDefaultFilterExpression((string)e.Value);
                    value = filterExpression.GetLogicString();
                }
                _updatingValueInCode = true;
                _gridViewSubstitute.SetRowCellValue(_gridViewSubstitute.FocusedRowHandle, _valueColumn, value);
                _updatingValueInCode = false;
                _gridViewSubstitute.SetRowCellValue(_gridViewSubstitute.FocusedRowHandle, _filterColumn, filterExpression);
                _modified = true;
            }
        }

        private void _gridViewSubstitute_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            IFilterExpression filterExpression = (IFilterExpression)_gridViewSubstitute.GetRowCellValue(e.FocusedRowHandle, _filterColumn);
            if (filterExpression != null && !IsDefaultFilterExpression(filterExpression))
            {
                _btnEditValue.TextEditStyle = TextEditStyles.DisableTextEditor;
            }
            else
            {
                _btnEditValue.TextEditStyle = TextEditStyles.Standard;
            }
        }

        private void _gridViewSubstitute_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
            XtraMessageBox.Show(Resources.FMMDCInvalidRowMessage, Resources.FMMDCInvalidRowTitle, MessageBoxButtons.OK);
            _gridViewSubstitute.Focus();
            _gridViewSubstitute.ShowEditor();
        }

        private void _gridViewSubstitute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _gridViewSubstitute.DeleteSelectedRows();
                e.Handled = true;
                _modified = true;
            }
            if (e.KeyCode == Keys.Escape && _gridViewSubstitute.HasColumnErrors)
            {
                e.Handled = true;
            }
        }

        private void _gridViewSubstitute_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (_readOnly || e.MenuType != GridMenuType.Row)
            {
                e.Allow = false;
                return;
            }
            e.Menu.Items.Clear();
            e.Menu.Items.Add(_addItem);
            e.Menu.Items.Add(_deleteitem);
        }

        private void _gridViewSubstitute_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            if ((IFilterExpression)_gridViewSubstitute.GetRowCellValue(e.RowHandle, _filterColumn) == null)
            {
                e.Valid = false;
                _gridViewSubstitute.SetColumnError(_valueColumn, Resources.FMMDCEmptyValueError);
            }
            if (string.IsNullOrEmpty((string)_gridViewSubstitute.GetRowCellValue(e.RowHandle, _substituteColumn)))
            {
                e.Valid = false;
                _gridViewSubstitute.SetColumnError(_substituteColumn, Resources.FMMDCEmptySubstitutionError);
            }
        }

        private void _txtItemEditor_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            using (FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog())
            {
                filterExpressionEditorDialog.Title = Resources.FMMDCFieldValueFilterTitle;
                Type[] collection = new Type[1] { typeof(FilterString) };
                filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: true);
                filterExpressionEditorDialog.LabelText = Resources.FMMDCFieldValueFilterLabel;
                if (!_gridViewSubstitute.IsNewItemRow(_gridViewSubstitute.FocusedRowHandle))
                {
                    BindableSubstitutionValues bindableSubstitutionValues = (BindableSubstitutionValues)_gridViewSubstitute.GetRow(_gridViewSubstitute.FocusedRowHandle);
                    filterExpressionEditorDialog.FilterExpression = bindableSubstitutionValues.Filter;
                }
                filterExpressionEditorDialog.ShowDialog();
                if (filterExpressionEditorDialog.DialogResult == DialogResult.OK && filterExpressionEditorDialog.FilterExpression != null)
                {
                    ButtonEdit buttonEdit = sender as ButtonEdit;
                    _updatedValueFromDialog = true;
                    buttonEdit.EditValue = filterExpressionEditorDialog.FilterExpression.GetLogicString();
                    buttonEdit.Properties.ReadOnly = true;
                    _gridViewSubstitute.SetRowCellValue(_gridViewSubstitute.FocusedRowHandle, _valueColumn, filterExpressionEditorDialog.FilterExpression.GetLogicString());
                    _gridViewSubstitute.SetRowCellValue(_gridViewSubstitute.FocusedRowHandle, _filterColumn, filterExpressionEditorDialog.FilterExpression);
                    _modified = true;
                }
            }
        }

        private void _txtValueEditor_Enter(object sender, EventArgs e)
        {
            ButtonEdit buttonEdit = sender as ButtonEdit;
            IFilterExpression filterExpression = (IFilterExpression)_gridViewSubstitute.GetRowCellValue(_gridViewSubstitute.FocusedRowHandle, _filterColumn);
            if (filterExpression != null && IsDefaultFilterExpression(filterExpression))
            {
                FilterExpression filterExpression2 = (FilterExpression)filterExpression;
                _updatingValueInCode = true;
                buttonEdit.EditValue = filterExpression2.Pattern;
                _updatingValueInCode = false;
                buttonEdit.SelectAll();
                _modified = true;
            }
        }

        private void btnCancel_OnClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnEditListFieldFilter_OnClick(object sender, EventArgs e)
        {
            using (FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog())
            {
                filterExpressionEditorDialog.Title = Resources.FMMDCFieldFilterTitle;
                Type[] collection = new Type[1] { typeof(SPField) };
                filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: true);
                filterExpressionEditorDialog.LabelText = Resources.FMMDCFieldFilterLabel;
                if (Option == null || Option.ListFieldFilterExpression == null)
                {
                    filterExpressionEditorDialog.FilterExpression = new FilterExpression(FilterOperand.Equals, typeof(SPField), "DisplayName", "", bIsCaseSensitive: true, bIsBaseFilter: false);
                }
                else
                {
                    filterExpressionEditorDialog.FilterExpression = Option.ListFieldFilterExpression;
                }
                filterExpressionEditorDialog.ShowDialog();
                if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
                {
                    Option.ListFieldFilterExpression = filterExpressionEditorDialog.FilterExpression;
                    _txtFieldName.Text = Option.FieldFilter;
                    _txtFieldName.IsModified = true;
                    _txtFieldName.DoValidate();
                    _modified = true;
                }
            }
        }

        private void btnEditListFilter_OnClick(object sender, EventArgs e)
        {
            using (FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog())
            {
                filterExpressionEditorDialog.Title = Resources.FMMDCListFilterTitle;
                Type[] collection = new Type[1] { typeof(SPList) };
                filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: true);
                filterExpressionEditorDialog.LabelText = Resources.FMMDCListFilterLabel;
                if (Option != null)
                {
                    filterExpressionEditorDialog.FilterExpression = Option.ListFilterExpression;
                }
                filterExpressionEditorDialog.ShowDialog();
                if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
                {
                    Option.ListFilterExpression = filterExpressionEditorDialog.FilterExpression;
                    _txtListName.Text = Option.ListFilter;
                    _txtListName.IsModified = true;
                    _txtListName.DoValidate();
                    _modified = true;
                }
            }
        }

        private void btnOk_OnClick(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SaveUI();
                base.DialogResult = DialogResult.OK;
                _modified = false;
                Close();
            }
        }

        private void cbCreateNewField_OnCheckedChanged(object sender, EventArgs e)
        {
            _txtNewFieldName.Enabled = _cbCreateNewField.Checked && !_readOnly;
            _txtNewDisplayName.Enabled = _cbCreateNewField.Checked && !_readOnly;
            _txtNewDisplayName.Properties.ReadOnly = !_cbCreateNewField.Checked;
            _txtNewFieldName.Properties.ReadOnly = !_cbCreateNewField.Checked;
            if (!_cbCreateNewField.Checked)
            {
                _txtNewFieldName.ErrorText = string.Empty;
                _txtNewDisplayName.ErrorText = string.Empty;
            }
        }

        private void cbGroup_OnGenericChangeOrLeave(object sender, EventArgs e)
        {
            try
            {
                _cbGroup.Leave -= cbGroup_OnGenericChangeOrLeave;
                _cbGroup.SelectedValueChanged -= cbGroup_OnGenericChangeOrLeave;
                Cursor = Cursors.WaitCursor;
                ConfigureGroupChange(clearSelection: false);
            }
            finally
            {
                Cursor = Cursors.Default;
                _cbGroup.Leave += cbGroup_OnGenericChangeOrLeave;
                _cbGroup.SelectedValueChanged += cbGroup_OnGenericChangeOrLeave;
            }
        }

        private void cbTermStore_OnGenericChangeOrLeave(object sender, EventArgs e)
        {
            try
            {
                _cbTermStore.Leave -= cbTermStore_OnGenericChangeOrLeave;
                _cbTermStore.SelectedValueChanged -= cbTermStore_OnGenericChangeOrLeave;
                Cursor = Cursors.WaitCursor;
                ConfigureTermStoreChange(clearSelection: false);
            }
            finally
            {
                _cbTermStore.SelectedValueChanged += cbTermStore_OnGenericChangeOrLeave;
                _cbTermStore.Leave += cbTermStore_OnGenericChangeOrLeave;
                Cursor = Cursors.Default;
            }
        }

        private void ConfigureForEdit()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                ConfigureTermStoreChange(clearSelection: false);
                ConfigureGroupChange(clearSelection: false);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ConfigureGroupChange(bool clearSelection)
        {
            string termStoreName = ((_cbTermStore.SelectedItem != null) ? _cbTermStore.SelectedItem.ToString() : _cbTermStore.Text);
            string text = ((_cbGroup.SelectedItem != null) ? _cbGroup.SelectedItem.ToString() : _cbGroup.Text);
            if (string.IsNullOrEmpty(text))
            {
                _cbTermSet.Properties.Items.Clear();
                return;
            }
            _cbTermSet.Properties.Items.Clear();
            if (clearSelection)
            {
                _cbTermSet.SelectedItem = null;
                _cbTermSet.Text = string.Empty;
            }
            SPTermGroup sPTermGroup = TermStoreCollection[termStoreName]?.Groups[text];
            SPTermGroup sPTermGroup2 = sPTermGroup;
            if (sPTermGroup2 == null)
            {
                return;
            }
            _cbTermSet.Properties.ReadOnly = false;
            _cbTermSet.Properties.Items.BeginUpdate();
            try
            {
                foreach (SPTermSet item in (IEnumerable<SPTermSet>)sPTermGroup2.TermSets)
                {
                    _cbTermSet.Properties.Items.Add(item.Name);
                }
            }
            finally
            {
                _cbTermSet.Properties.ReadOnly = _readOnly;
                _cbTermSet.Properties.Items.EndUpdate();
            }
        }

        private void ConfigureTermStoreChange(bool clearSelection)
        {
            string text = ((_cbTermStore.SelectedItem != null) ? _cbTermStore.SelectedItem.ToString() : _cbTermStore.Text);
            if (string.IsNullOrEmpty(text))
            {
                _cbGroup.Properties.Items.Clear();
                _cbTermSet.Properties.Items.Clear();
                return;
            }
            _cbGroup.Properties.Items.Clear();
            if (clearSelection)
            {
                _cbGroup.SelectedItem = null;
                _cbTermSet.SelectedItem = null;
                _cbTermSet.Text = string.Empty;
                _cbGroup.Text = string.Empty;
            }
            SPTermStore sPTermStore = TermStoreCollection[text];
            _cbGroup.Enabled = false;
            _cbGroup.Properties.Items.BeginUpdate();
            try
            {
                foreach (SPTermGroup item in (IEnumerable<SPTermGroup>)sPTermStore.Groups)
                {
                    _cbGroup.Properties.Items.Add(item.Name);
                }
            }
            finally
            {
                _cbGroup.Enabled = !_readOnly;
                _cbGroup.Properties.Items.EndUpdate();
            }
        }

        private IFilterExpression CreateDefaultFilterExpression(string text)
        {
            return new FilterExpression(FilterOperand.Equals, typeof(FilterString), "FieldValue", text, bIsCaseSensitive: true, bIsBaseFilter: false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form_OnClosing(object sender, FormClosingEventArgs e)
        {
            if (_modified && !_readOnly)
            {
                e.Cancel = FlatXtraMessageBox.Show(Resources.FMMDCConfirmChangesMsg, Resources.FMMDCConfirmChangesCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
            }
        }

        private void Form_OnShown(object sender, EventArgs e)
        {
            if (_loaded)
            {
                return;
            }
            _loaded = true;
            _pnlWait.Refresh();
            try
            {
                LoadTermStores();
                if (Option != null)
                {
                    LoadItem();
                }
            }
            finally
            {
                _pnlWait.Visible = false;
                _pnlWait.Dock = DockStyle.None;
                _pnlWait.Refresh();
            }
        }

        private void InitializeComponent()
        {
            DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Transform.FieldToManagedMetadataConfigItemDialog));
            DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
            this._btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this._btnOk = new DevExpress.XtraEditors.SimpleButton();
            this._cbCreateNewField = new DevExpress.XtraEditors.CheckEdit();
            this._lblTermSet = new DevExpress.XtraEditors.LabelControl();
            this._lblTermGroup = new DevExpress.XtraEditors.LabelControl();
            this._lblTermStore = new DevExpress.XtraEditors.LabelControl();
            this._lblFieldName = new DevExpress.XtraEditors.LabelControl();
            this._lblListName = new DevExpress.XtraEditors.LabelControl();
            this._cbTermSet = new DevExpress.XtraEditors.ComboBoxEdit();
            this._cbGroup = new DevExpress.XtraEditors.ComboBoxEdit();
            this._cbTermStore = new DevExpress.XtraEditors.ComboBoxEdit();
            this._btnEditListFilter = new DevExpress.XtraEditors.SimpleButton();
            this._btnEditListFieldFilter = new DevExpress.XtraEditors.SimpleButton();
            this._txtNewFieldName = new DevExpress.XtraEditors.TextEdit();
            this._txtNewDisplayName = new DevExpress.XtraEditors.TextEdit();
            this._lblNewName = new DevExpress.XtraEditors.LabelControl();
            this._lblNewDisplayName = new DevExpress.XtraEditors.LabelControl();
            this._pnlWait = new DevExpress.XtraEditors.PanelControl();
            this._lblLoading = new DevExpress.XtraEditors.LabelControl();
            this._lblAnchor = new DevExpress.XtraEditors.LabelControl();
            this._txtAnchor = new DevExpress.XtraEditors.TextEdit();
            this._groupSource = new DevExpress.XtraEditors.GroupControl();
            this._txtListName = new DevExpress.XtraEditors.TextEdit();
            this._txtFieldName = new DevExpress.XtraEditors.TextEdit();
            this._groupTarget = new DevExpress.XtraEditors.GroupControl();
            this._pbInfo = new DevExpress.XtraEditors.PictureEdit();
            this._helpCreateNewField = new TooltipsTest.HelpTipButton();
            this._helpAnchor = new TooltipsTest.HelpTipButton();
            this._helpTermSet = new TooltipsTest.HelpTipButton();
            this._helpGroup = new TooltipsTest.HelpTipButton();
            this._lblSubstitute = new DevExpress.XtraEditors.LabelControl();
            this._lblTransform = new DevExpress.XtraEditors.LabelControl();
            this._tabControl = new DevExpress.XtraTab.XtraTabControl();
            this._pageTransform = new DevExpress.XtraTab.XtraTabPage();
            this._pageSubstitute = new DevExpress.XtraTab.XtraTabPage();
            this._gridSubstitute = new DevExpress.XtraGrid.GridControl();
            this._gridViewSubstitute = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._valueColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._btnEditValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this._substituteColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this._txtEditSubstitute = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this._filterColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)this._cbCreateNewField.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbTermSet.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbGroup.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._cbTermStore.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._txtNewFieldName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._txtNewDisplayName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._pnlWait).BeginInit();
            this._pnlWait.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._txtAnchor.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._groupSource).BeginInit();
            this._groupSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._txtListName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._txtFieldName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._groupTarget).BeginInit();
            this._groupTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._pbInfo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._helpCreateNewField).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._helpAnchor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._helpTermSet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._helpGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._tabControl).BeginInit();
            this._tabControl.SuspendLayout();
            this._pageTransform.SuspendLayout();
            this._pageSubstitute.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._gridSubstitute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._gridViewSubstitute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._btnEditValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._txtEditSubstitute).BeginInit();
            base.SuspendLayout();
            this._btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(332, 406);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.Click += new System.EventHandler(btnCancel_OnClick);
            this._btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this._btnOk.Location = new System.Drawing.Point(250, 406);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(75, 23);
            this._btnOk.TabIndex = 1;
            this._btnOk.Text = "OK";
            this._btnOk.Click += new System.EventHandler(btnOk_OnClick);
            this._cbCreateNewField.Location = new System.Drawing.Point(80, 142);
            this._cbCreateNewField.Name = "_cbCreateNewField";
            this._cbCreateNewField.Properties.AutoWidth = true;
            this._cbCreateNewField.Properties.Caption = "Transform to New or Existing Column";
            this._cbCreateNewField.Size = new System.Drawing.Size(200, 19);
            this._cbCreateNewField.TabIndex = 8;
            this._cbCreateNewField.CheckedChanged += new System.EventHandler(cbCreateNewField_OnCheckedChanged);
            this._lblTermSet.Location = new System.Drawing.Point(8, 83);
            this._lblTermSet.Name = "_lblTermSet";
            this._lblTermSet.Size = new System.Drawing.Size(43, 13);
            this._lblTermSet.TabIndex = 4;
            this._lblTermSet.Text = "Term Set";
            this._lblTermGroup.Location = new System.Drawing.Point(8, 57);
            this._lblTermGroup.Name = "_lblTermGroup";
            this._lblTermGroup.Size = new System.Drawing.Size(56, 13);
            this._lblTermGroup.TabIndex = 2;
            this._lblTermGroup.Text = "Term Group";
            this._lblTermStore.Location = new System.Drawing.Point(8, 31);
            this._lblTermStore.Name = "_lblTermStore";
            this._lblTermStore.Size = new System.Drawing.Size(53, 13);
            this._lblTermStore.TabIndex = 0;
            this._lblTermStore.Text = "Term Store";
            this._lblFieldName.Location = new System.Drawing.Point(8, 57);
            this._lblFieldName.Name = "_lblFieldName";
            this._lblFieldName.Size = new System.Drawing.Size(54, 13);
            this._lblFieldName.TabIndex = 3;
            this._lblFieldName.Text = "List Column";
            this._lblListName.Location = new System.Drawing.Point(8, 32);
            this._lblListName.Name = "_lblListName";
            this._lblListName.Size = new System.Drawing.Size(16, 13);
            this._lblListName.TabIndex = 0;
            this._lblListName.Text = "List";
            this._cbTermSet.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._cbTermSet.Location = new System.Drawing.Point(82, 80);
            this._cbTermSet.Name = "_cbTermSet";
            this._cbTermSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this._cbTermSet.Properties.MaxLength = 250;
            this._cbTermSet.Size = new System.Drawing.Size(256, 20);
            this._cbTermSet.TabIndex = 5;
            this._cbTermSet.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmptyAndInvalidTaxonomy);
            this._cbGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._cbGroup.Location = new System.Drawing.Point(82, 54);
            this._cbGroup.Name = "_cbGroup";
            this._cbGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this._cbGroup.Properties.MaxLength = 250;
            this._cbGroup.Size = new System.Drawing.Size(256, 20);
            this._cbGroup.TabIndex = 3;
            this._cbGroup.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmptyAndInvalidTaxonomy);
            this._cbTermStore.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._cbTermStore.Location = new System.Drawing.Point(82, 28);
            this._cbTermStore.Name = "_cbTermStore";
            this._cbTermStore.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this._cbTermStore.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this._cbTermStore.Size = new System.Drawing.Size(256, 20);
            this._cbTermStore.TabIndex = 1;
            this._cbTermStore.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmpty);
            this._btnEditListFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this._btnEditListFilter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this._btnEditListFilter.Location = new System.Drawing.Point(344, 29);
            this._btnEditListFilter.Name = "_btnEditListFilter";
            this._btnEditListFilter.Size = new System.Drawing.Size(27, 20);
            toolTipTitleItem.Text = "Source List Filter Condition";
            toolTipItem.LeftIndent = 6;
            toolTipItem.Text = "Define the filter condition to use to find the source list.";
            superToolTip.Items.Add(toolTipTitleItem);
            superToolTip.Items.Add(toolTipItem);
            this._btnEditListFilter.SuperTip = superToolTip;
            this._btnEditListFilter.TabIndex = 2;
            this._btnEditListFilter.Text = "...";
            this._btnEditListFilter.Click += new System.EventHandler(btnEditListFilter_OnClick);
            this._btnEditListFieldFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this._btnEditListFieldFilter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this._btnEditListFieldFilter.Location = new System.Drawing.Point(344, 56);
            this._btnEditListFieldFilter.Name = "_btnEditListFieldFilter";
            this._btnEditListFieldFilter.Size = new System.Drawing.Size(27, 20);
            toolTipTitleItem2.Text = "Source List Column Filter Condition";
            toolTipItem2.LeftIndent = 6;
            toolTipItem2.Text = "Define the filter condition to use to find the source list column.";
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this._btnEditListFieldFilter.SuperTip = superToolTip2;
            this._btnEditListFieldFilter.TabIndex = 5;
            this._btnEditListFieldFilter.Text = "...";
            this._btnEditListFieldFilter.Click += new System.EventHandler(btnEditListFieldFilter_OnClick);
            this._txtNewFieldName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._txtNewFieldName.Location = new System.Drawing.Point(180, 171);
            this._txtNewFieldName.Name = "_txtNewFieldName";
            this._txtNewFieldName.Properties.MaxLength = 32;
            this._txtNewFieldName.Size = new System.Drawing.Size(158, 20);
            this._txtNewFieldName.TabIndex = 10;
            this._txtNewFieldName.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmptyAndInvalidColumnName);
            this._txtNewDisplayName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._txtNewDisplayName.Location = new System.Drawing.Point(180, 197);
            this._txtNewDisplayName.Name = "_txtNewDisplayName";
            this._txtNewDisplayName.Properties.MaxLength = 50;
            this._txtNewDisplayName.Size = new System.Drawing.Size(158, 20);
            this._txtNewDisplayName.TabIndex = 12;
            this._txtNewDisplayName.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmpty);
            this._lblNewName.Location = new System.Drawing.Point(110, 174);
            this._lblNewName.Name = "_lblNewName";
            this._lblNewName.Size = new System.Drawing.Size(27, 13);
            this._lblNewName.TabIndex = 9;
            this._lblNewName.Text = "Name";
            this._lblNewDisplayName.Location = new System.Drawing.Point(110, 200);
            this._lblNewDisplayName.Name = "_lblNewDisplayName";
            this._lblNewDisplayName.Size = new System.Drawing.Size(64, 13);
            this._lblNewDisplayName.TabIndex = 11;
            this._lblNewDisplayName.Text = "Display Name";
            this._pnlWait.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._pnlWait.Controls.Add(this._lblLoading);
            this._pnlWait.Location = new System.Drawing.Point(12, 12);
            this._pnlWait.Name = "_pnlWait";
            this._pnlWait.Size = new System.Drawing.Size(400, 388);
            this._pnlWait.TabIndex = 37;
            this._lblLoading.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this._lblLoading.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this._lblLoading.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this._lblLoading.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this._lblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblLoading.Location = new System.Drawing.Point(2, 2);
            this._lblLoading.Name = "_lblLoading";
            this._lblLoading.Size = new System.Drawing.Size(396, 384);
            this._lblLoading.TabIndex = 0;
            this._lblLoading.Text = "Loading Configuration\r\n\r\nPlease wait ...\r\n";
            this._lblAnchor.Location = new System.Drawing.Point(8, 109);
            this._lblAnchor.Name = "_lblAnchor";
            this._lblAnchor.Size = new System.Drawing.Size(61, 13);
            this._lblAnchor.TabIndex = 6;
            this._lblAnchor.Text = "Term Anchor";
            this._txtAnchor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._txtAnchor.Location = new System.Drawing.Point(82, 106);
            this._txtAnchor.Name = "_txtAnchor";
            this._txtAnchor.Properties.MaxLength = 255;
            this._txtAnchor.Size = new System.Drawing.Size(256, 20);
            this._txtAnchor.TabIndex = 7;
            this._groupSource.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._groupSource.Controls.Add(this._lblListName);
            this._groupSource.Controls.Add(this._lblFieldName);
            this._groupSource.Controls.Add(this._txtListName);
            this._groupSource.Controls.Add(this._btnEditListFilter);
            this._groupSource.Controls.Add(this._btnEditListFieldFilter);
            this._groupSource.Controls.Add(this._txtFieldName);
            this._groupSource.Location = new System.Drawing.Point(7, 24);
            this._groupSource.Name = "_groupSource";
            this._groupSource.Size = new System.Drawing.Size(384, 87);
            this._groupSource.TabIndex = 1;
            this._groupSource.Text = "Source";
            this._txtListName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._txtListName.Location = new System.Drawing.Point(82, 29);
            this._txtListName.Name = "_txtListName";
            this._txtListName.Properties.ReadOnly = true;
            this._txtListName.Size = new System.Drawing.Size(256, 20);
            this._txtListName.TabIndex = 1;
            this._txtListName.TabStop = false;
            this._txtListName.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmpty);
            this._txtFieldName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._txtFieldName.Location = new System.Drawing.Point(82, 56);
            this._txtFieldName.Name = "_txtFieldName";
            this._txtFieldName.Properties.ReadOnly = true;
            this._txtFieldName.Size = new System.Drawing.Size(256, 20);
            this._txtFieldName.TabIndex = 4;
            this._txtFieldName.TabStop = false;
            this._txtFieldName.Validating += new System.ComponentModel.CancelEventHandler(ValidateForEmpty);
            this._groupTarget.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._groupTarget.Controls.Add(this._pbInfo);
            this._groupTarget.Controls.Add(this._helpCreateNewField);
            this._groupTarget.Controls.Add(this._helpAnchor);
            this._groupTarget.Controls.Add(this._helpTermSet);
            this._groupTarget.Controls.Add(this._helpGroup);
            this._groupTarget.Controls.Add(this._cbTermStore);
            this._groupTarget.Controls.Add(this._lblTermSet);
            this._groupTarget.Controls.Add(this._cbCreateNewField);
            this._groupTarget.Controls.Add(this._lblAnchor);
            this._groupTarget.Controls.Add(this._lblTermStore);
            this._groupTarget.Controls.Add(this._lblNewName);
            this._groupTarget.Controls.Add(this._txtAnchor);
            this._groupTarget.Controls.Add(this._lblTermGroup);
            this._groupTarget.Controls.Add(this._txtNewFieldName);
            this._groupTarget.Controls.Add(this._cbTermSet);
            this._groupTarget.Controls.Add(this._lblNewDisplayName);
            this._groupTarget.Controls.Add(this._txtNewDisplayName);
            this._groupTarget.Controls.Add(this._cbGroup);
            this._groupTarget.Location = new System.Drawing.Point(7, 117);
            this._groupTarget.Name = "_groupTarget";
            this._groupTarget.Size = new System.Drawing.Size(384, 240);
            this._groupTarget.TabIndex = 2;
            this._groupTarget.Text = "Target";
            this._pbInfo.EditValue = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Warning32;
            this._pbInfo.Location = new System.Drawing.Point(11, 200);
            this._pbInfo.Name = "_pbInfo";
            this._pbInfo.Properties.AllowFocused = false;
            this._pbInfo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this._pbInfo.Properties.Appearance.Options.UseBackColor = true;
            this._pbInfo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._pbInfo.Properties.ReadOnly = true;
            this._pbInfo.Size = new System.Drawing.Size(32, 32);
            this._pbInfo.TabIndex = 45;
            this._pbInfo.Visible = false;
            this._helpCreateNewField.AnchoringControl = this._cbCreateNewField;
            this._helpCreateNewField.BackColor = System.Drawing.Color.Transparent;
            this._helpCreateNewField.CommonParentControl = null;
            this._helpCreateNewField.Image = (System.Drawing.Image)resources.GetObject("_helpCreateNewField.Image");
            this._helpCreateNewField.Location = new System.Drawing.Point(285, 141);
            this._helpCreateNewField.MaximumSize = new System.Drawing.Size(20, 20);
            this._helpCreateNewField.MinimumSize = new System.Drawing.Size(20, 20);
            this._helpCreateNewField.Name = "_helpCreateNewField";
            this._helpCreateNewField.Size = new System.Drawing.Size(20, 20);
            this._helpCreateNewField.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._helpCreateNewField.TabIndex = 44;
            this._helpCreateNewField.TabStop = false;
            this._helpAnchor.AnchoringControl = this._txtAnchor;
            this._helpAnchor.BackColor = System.Drawing.Color.Transparent;
            this._helpAnchor.CommonParentControl = null;
            this._helpAnchor.Image = (System.Drawing.Image)resources.GetObject("_helpAnchor.Image");
            this._helpAnchor.Location = new System.Drawing.Point(344, 106);
            this._helpAnchor.MaximumSize = new System.Drawing.Size(20, 20);
            this._helpAnchor.MinimumSize = new System.Drawing.Size(20, 20);
            this._helpAnchor.Name = "_helpAnchor";
            this._helpAnchor.Size = new System.Drawing.Size(20, 20);
            this._helpAnchor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._helpAnchor.TabIndex = 43;
            this._helpAnchor.TabStop = false;
            this._helpTermSet.AnchoringControl = this._cbTermSet;
            this._helpTermSet.BackColor = System.Drawing.Color.Transparent;
            this._helpTermSet.CommonParentControl = null;
            this._helpTermSet.Image = (System.Drawing.Image)resources.GetObject("_helpTermSet.Image");
            this._helpTermSet.Location = new System.Drawing.Point(344, 80);
            this._helpTermSet.MaximumSize = new System.Drawing.Size(20, 20);
            this._helpTermSet.MinimumSize = new System.Drawing.Size(20, 20);
            this._helpTermSet.Name = "_helpTermSet";
            this._helpTermSet.Size = new System.Drawing.Size(20, 20);
            this._helpTermSet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._helpTermSet.TabIndex = 42;
            this._helpTermSet.TabStop = false;
            this._helpGroup.AnchoringControl = this._cbGroup;
            this._helpGroup.BackColor = System.Drawing.Color.Transparent;
            this._helpGroup.CommonParentControl = null;
            this._helpGroup.Image = (System.Drawing.Image)resources.GetObject("_helpGroup.Image");
            this._helpGroup.Location = new System.Drawing.Point(344, 54);
            this._helpGroup.MaximumSize = new System.Drawing.Size(20, 20);
            this._helpGroup.MinimumSize = new System.Drawing.Size(20, 20);
            this._helpGroup.Name = "_helpGroup";
            this._helpGroup.Size = new System.Drawing.Size(20, 20);
            this._helpGroup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._helpGroup.TabIndex = 41;
            this._helpGroup.TabStop = false;
            this._lblSubstitute.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._lblSubstitute.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this._lblSubstitute.Location = new System.Drawing.Point(7, 5);
            this._lblSubstitute.Name = "_lblSubstitute";
            this._lblSubstitute.Size = new System.Drawing.Size(384, 26);
            this._lblSubstitute.TabIndex = 47;
            this._lblSubstitute.Text = "Substitute source item values with new values that will be used as managed metadata terms when the filter condition matches.";
            this._lblTransform.Location = new System.Drawing.Point(11, 5);
            this._lblTransform.Name = "_lblTransform";
            this._lblTransform.Size = new System.Drawing.Size(307, 13);
            this._lblTransform.TabIndex = 0;
            this._lblTransform.Text = "Transform Source List Columns into Managed Metadata Columns";
            this._tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._tabControl.Location = new System.Drawing.Point(12, 12);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedTabPage = this._pageTransform;
            this._tabControl.Size = new System.Drawing.Size(400, 388);
            this._tabControl.TabIndex = 0;
            DevExpress.XtraTab.XtraTabPageCollection tabPages = this._tabControl.TabPages;
            DevExpress.XtraTab.XtraTabPage[] pages = new DevExpress.XtraTab.XtraTabPage[2] { this._pageTransform, this._pageSubstitute };
            tabPages.AddRange(pages);
            this._pageTransform.Controls.Add(this._lblTransform);
            this._pageTransform.Controls.Add(this._groupSource);
            this._pageTransform.Controls.Add(this._groupTarget);
            this._pageTransform.Name = "_pageTransform";
            this._pageTransform.Size = new System.Drawing.Size(394, 360);
            this._pageTransform.Text = "Transform Columns";
            this._pageSubstitute.Controls.Add(this._gridSubstitute);
            this._pageSubstitute.Controls.Add(this._lblSubstitute);
            this._pageSubstitute.Name = "_pageSubstitute";
            this._pageSubstitute.Size = new System.Drawing.Size(394, 360);
            this._pageSubstitute.Text = "Substitute Source Item Values";
            this._gridSubstitute.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this._gridSubstitute.Location = new System.Drawing.Point(3, 35);
            this._gridSubstitute.MainView = this._gridViewSubstitute;
            this._gridSubstitute.Name = "_gridSubstitute";
            DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this._gridSubstitute.RepositoryItems;
            DevExpress.XtraEditors.Repository.RepositoryItem[] items = new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this._btnEditValue, this._txtEditSubstitute };
            repositoryItems.AddRange(items);
            this._gridSubstitute.Size = new System.Drawing.Size(388, 322);
            this._gridSubstitute.TabIndex = 48;
            this._gridSubstitute.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridViewSubstitute });
            DevExpress.XtraGrid.Columns.GridColumnCollection columns = this._gridViewSubstitute.Columns;
            DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new DevExpress.XtraGrid.Columns.GridColumn[3] { this._valueColumn, this._substituteColumn, this._filterColumn };
            columns.AddRange(columns2);
            this._gridViewSubstitute.GridControl = this._gridSubstitute;
            this._gridViewSubstitute.Name = "_gridViewSubstitute";
            this._gridViewSubstitute.OptionsCustomization.AllowFilter = false;
            this._gridViewSubstitute.OptionsCustomization.AllowGroup = false;
            this._gridViewSubstitute.OptionsSelection.MultiSelect = true;
            this._gridViewSubstitute.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this._gridViewSubstitute.OptionsView.ShowGroupPanel = false;
            this._gridViewSubstitute.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(_gridViewSubstitute_PopupMenuShowing);
            this._gridViewSubstitute.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(_gridViewSubstitute_FocusedRowChanged);
            this._gridViewSubstitute.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(_gridViewSubstitute_CellValueChanged);
            this._gridViewSubstitute.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(_gridViewSubstitute_InvalidRowException);
            this._gridViewSubstitute.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(_gridViewSubstitute_ValidateRow);
            this._gridViewSubstitute.KeyDown += new System.Windows.Forms.KeyEventHandler(_gridViewSubstitute_KeyDown);
            this._valueColumn.Caption = "Value";
            this._valueColumn.ColumnEdit = this._btnEditValue;
            this._valueColumn.FieldName = "ValueFilter";
            this._valueColumn.Name = "_valueColumn";
            this._valueColumn.Visible = true;
            this._valueColumn.VisibleIndex = 0;
            this._btnEditValue.AutoHeight = false;
            DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this._btnEditValue.Buttons;
            DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Metalogix.SharePoint.UI.WinForms.Properties.Resources.Filter16, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, "", null, null, true)
            };
            buttons.AddRange(buttons2);
            this._btnEditValue.Name = "_btnEditValue";
            this._btnEditValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(_txtItemEditor_ButtonClick);
            this._btnEditValue.Enter += new System.EventHandler(_txtValueEditor_Enter);
            this._substituteColumn.Caption = "Substitute";
            this._substituteColumn.ColumnEdit = this._txtEditSubstitute;
            this._substituteColumn.FieldName = "Substitute";
            this._substituteColumn.Name = "_substituteColumn";
            this._substituteColumn.Visible = true;
            this._substituteColumn.VisibleIndex = 1;
            this._txtEditSubstitute.AutoHeight = false;
            this._txtEditSubstitute.Name = "_txtEditSubstitute";
            this._filterColumn.Caption = "Value Filter";
            this._filterColumn.FieldName = "Filter";
            this._filterColumn.Name = "_filterColumn";
            this._filterColumn.OptionsColumn.ReadOnly = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this._btnCancel;
            base.ClientSize = new System.Drawing.Size(424, 441);
            base.Controls.Add(this._tabControl);
            base.Controls.Add(this._btnOk);
            base.Controls.Add(this._btnCancel);
            base.Controls.Add(this._pnlWait);
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 480);
            base.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 480);
            base.Name = "FieldToManagedMetadataConfigItemDialog";
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Transformation for a List Column";
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form_OnClosing);
            base.Shown += new System.EventHandler(Form_OnShown);
            ((System.ComponentModel.ISupportInitialize)this._cbCreateNewField.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbTermSet.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbGroup.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._cbTermStore.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._txtNewFieldName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._txtNewDisplayName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._pnlWait).EndInit();
            this._pnlWait.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._txtAnchor.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._groupSource).EndInit();
            this._groupSource.ResumeLayout(false);
            this._groupSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._txtListName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._txtFieldName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._groupTarget).EndInit();
            this._groupTarget.ResumeLayout(false);
            this._groupTarget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this._pbInfo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._helpCreateNewField).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._helpAnchor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._helpTermSet).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._helpGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._tabControl).EndInit();
            this._tabControl.ResumeLayout(false);
            this._pageTransform.ResumeLayout(false);
            this._pageTransform.PerformLayout();
            this._pageSubstitute.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._gridSubstitute).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._gridViewSubstitute).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._btnEditValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._txtEditSubstitute).EndInit();
            base.ResumeLayout(false);
        }

        private bool IsDefaultFilterExpression(IFilterExpression expression)
        {
            if (!(expression is FilterExpression))
            {
                return false;
            }
            FilterExpression filterExpression = (FilterExpression)expression;
            if (filterExpression.Operand != 0)
            {
                return false;
            }
            return filterExpression.Property == "FieldValue";
        }

        private void LoadItem()
        {
            _txtListName.Text = Option.ListFilter;
            _txtFieldName.Text = Option.FieldFilter;
            _cbTermStore.Text = Option.TargetTermstore;
            _cbGroup.Text = Option.TargetGroup;
            _cbTermSet.Text = Option.TargetTermSet;
            _txtAnchor.Text = Option.TargetAnchor;
            _cbCreateNewField.Checked = Option.CreateNewField;
            _txtNewFieldName.Text = Option.NewFieldName;
            _txtNewDisplayName.Text = Option.NewFieldDisplayName;
            if (!_isNew)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    ConfigureTermStoreChange(clearSelection: false);
                    ConfigureGroupChange(clearSelection: false);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            if (Option.ItemFieldValueFilterCollection != null && Option.ItemFieldValueFilterCollection.Count > 0)
            {
                _dataSource = BindableSubstitutionValues.CopyFromItemFieldValueFilterList(Option.ItemFieldValueFilterCollection);
            }
            _gridSubstitute.DataSource = _dataSource;
        }

        private void LoadTermStores()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                int count = TermStoreCollection.Count;
                _cbTermStore.Properties.Items.Clear();
                _cbTermStore.SelectedIndex = -1;
                foreach (SPTermStore item in (IEnumerable<SPTermStore>)TermStoreCollection)
                {
                    _cbTermStore.Properties.Items.Add(item.Name);
                }
                _cbGroup.Properties.Items.Clear();
                _cbGroup.SelectedIndex = -1;
                _cbTermSet.Properties.Items.Clear();
                _cbTermSet.SelectedIndex = -1;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SaveUI()
        {
            Option.TargetTermstore = _cbTermStore.Text.Trim();
            Option.TargetGroup = _cbGroup.Text.Trim();
            Option.TargetTermSet = _cbTermSet.Text.Trim();
            Option.TargetAnchor = _txtAnchor.Text.Trim();
            Option.CreateNewField = _cbCreateNewField.Checked;
            Option.NewFieldName = (_cbCreateNewField.Checked ? _txtNewFieldName.Text.Trim() : string.Empty);
            Option.NewFieldDisplayName = (_cbCreateNewField.Checked ? _txtNewDisplayName.Text.Trim() : string.Empty);
            Option.ItemFieldValueFilterCollection = BindableSubstitutionValues.CopyToItemFieldValueFilterList(_dataSource);
        }

        private void ValidateForEmpty(object sender, CancelEventArgs e)
        {
            BaseEdit baseEdit = (BaseEdit)sender;
            string value = ((!string.IsNullOrEmpty(baseEdit.Text)) ? baseEdit.Text.Trim() : null);
            if (!_readOnly && string.IsNullOrEmpty(value))
            {
                baseEdit.ErrorText = Resources.FMMDCMissingValuesMsg;
            }
            else
            {
                baseEdit.ErrorText = string.Empty;
            }
        }

        private void ValidateForEmptyAndInvalidColumnName(object sender, CancelEventArgs e)
        {
            ValidateForEmpty(sender, e);
            if (!(((BaseEdit)sender).ErrorText != string.Empty))
            {
                ValidateForInvalidColumnName(sender, e);
            }
        }

        private void ValidateForEmptyAndInvalidTaxonomy(object sender, CancelEventArgs e)
        {
            ValidateForEmpty(sender, e);
            if (!(((BaseEdit)sender).ErrorText != string.Empty))
            {
                ValidateForInvalidTaxonomy(sender, e);
            }
        }

        private void ValidateForInvalidColumnName(object sender, CancelEventArgs e)
        {
            BaseEdit baseEdit = (BaseEdit)sender;
            if (_readOnly || Regex.IsMatch(baseEdit.Text.Trim(), "^[A-Za-z\\x5F][0-9A-Za-z\\x5F]+$"))
            {
                baseEdit.ErrorText = string.Empty;
            }
            else
            {
                baseEdit.ErrorText = $"{Resources.FMMDCInvalidCharactersExist}{Environment.NewLine}{Resources.FMMDCValidInternalNameChars}";
            }
        }

        private void ValidateForInvalidTaxonomy(object sender, CancelEventArgs e)
        {
            BaseEdit baseEdit = (BaseEdit)sender;
            if (_readOnly || !Regex.IsMatch(baseEdit.Text.Trim(), "[\\x22\\x3B\\x3C\\x3E\\x7C\\x09]"))
            {
                baseEdit.ErrorText = string.Empty;
            }
            else
            {
                baseEdit.ErrorText = $"{Resources.FMMDCInvalidCharactersExist}{Environment.NewLine}{Resources.FMMDCInvalidTaxonomyChars}";
            }
        }

        private bool ValidateInput()
        {
            bool flag = true;
            List<BaseEdit> list = new List<BaseEdit>();
            BaseEdit[] collection = new BaseEdit[5] { _txtListName, _txtFieldName, _cbTermStore, _cbGroup, _cbTermSet };
            list.AddRange(collection);
            if (!_cbCreateNewField.Checked)
            {
                _txtNewFieldName.ErrorText = string.Empty;
                _txtNewDisplayName.ErrorText = string.Empty;
            }
            else
            {
                BaseEdit[] collection2 = new BaseEdit[2] { _txtNewFieldName, _txtNewDisplayName };
                list.AddRange(collection2);
            }
            list.ForEach(delegate (BaseEdit ctrl)
            {
                CancelEventArgs e3 = new CancelEventArgs();
                ValidateForEmpty(ctrl, e3);
                flag &= ctrl.ErrorText == string.Empty;
            });
            if (flag)
            {
                _txtAnchor.Text = TransformUtils.SanitiseTaxonomyAnchorConfiguration(_txtAnchor.Text);
                ComboBoxEdit[] collection3 = new ComboBoxEdit[2] { _cbGroup, _cbTermSet };
                new List<BaseEdit>(collection3).ForEach(delegate (BaseEdit ctrl)
                {
                    CancelEventArgs e2 = new CancelEventArgs();
                    ValidateForInvalidTaxonomy(ctrl, e2);
                    flag &= ctrl.ErrorText == string.Empty;
                });
                if (!_cbCreateNewField.Checked)
                {
                    _txtNewFieldName.ErrorText = string.Empty;
                }
                else
                {
                    CancelEventArgs e = new CancelEventArgs();
                    ValidateForInvalidColumnName(_txtNewFieldName, e);
                    flag &= _txtNewFieldName.ErrorText == string.Empty;
                }
            }
            if (!flag)
            {
                _pageTransform.Show();
            }
            return flag;
        }

        private void w_bAddFilter_Click(object sender, EventArgs e)
        {
            _gridViewSubstitute.AddNewRow();
            _gridViewSubstitute.Focus();
            _gridViewSubstitute.ShowEditor();
        }

        private void w_DeleteFilters_Click(object sender, EventArgs e)
        {
            _gridViewSubstitute.DeleteSelectedRows();
            _modified = true;
        }
    }
}
