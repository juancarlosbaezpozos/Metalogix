using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Data;
using Metalogix.Data.Filters;

namespace Metalogix.UI.WinForms.Data.Filters
{
    public class FilterExpressionEditorControl : UserControl
    {
        public class BindingListWithRemoving<T> : BindingList<T>
        {
            public event EventHandler<RemovingEventArgs> RemovingItem;

            protected override void RemoveItem(int index)
            {
                if (this.RemovingItem != null)
                {
                    this.RemovingItem(this, new RemovingEventArgs(base[index]));
                }
                base.RemoveItem(index);
            }
        }

        public class RemovingEventArgs : EventArgs
        {
            public object RemovingObject { get; set; }

            public RemovingEventArgs(object removingObject)
            {
                RemovingObject = removingObject;
            }
        }

        protected IFilterExpression _filterExpression;

        protected BindingListWithRemoving<FilterRow> _dataSource = new BindingListWithRemoving<FilterRow>();

        protected FilterNode _filterTree;

        protected List<FilterNode[]> _groupGrid = new List<FilterNode[]>();

        protected string _fieldDividerItem = "";

        protected bool _isInitializingData;

        protected DXMenuItem addItem = new DXMenuItem("Add Row");

        protected DXMenuItem deleteitem = new DXMenuItem("Delete Selected Rows");

        protected DXMenuItem groupItem = new DXMenuItem("Group Rows");

        protected DXMenuItem ungroupItem = new DXMenuItem("Remove Group");

        protected static string[] _commonFields;

        protected static string[] _supportedTypes;

        private string[] _noValueRequiredOperators = new string[4] { "IsNull", "NotNull", "IsNullOrBlank", "NotNullAndNotBlank" };

        private Dictionary<string, FilterProperty> _filterPropertyCollection;

        private FilterBuilderType _filterTypes;

        private bool m_bLabelTextSetManually;

        private IContainer components;

        private LabelControl w_lblDialogText;

        private SimpleButton w_bGroupFilters;

        private SimpleButton w_bRemoveGroup;

        private SimpleButton w_bAddFilter;

        private SimpleButton w_bDeleteFilter;

        private MemoEdit mem_filterExpressionText;

        private GridControl gridControl;

        private GridView gridView;

        private GridColumn AndOrColumn;

        private GridColumn FieldColumn;

        private GridColumn OperatorColumn;

        private GridColumn ValueColumn;

        private RepositoryItemComboBox AndOrEditor;

        private RepositoryItemComboBox FieldEditor;

        private RepositoryItemComboBox OperatorGeneralEditor;

        private RepositoryItemDateEdit ValueCalendarEditor;

        private RepositoryItemTextEdit ValueTextEditor;

        private RepositoryItemComboBox ValueComboBoxEditor;

        private RepositoryItemComboBox OperatorBooleanEditor;

        private RepositoryItemComboBox OperatorDateTimeEditor;

        private RepositoryItemSpinEdit ValueNumberEditor;

        private RepositoryItemComboBox ValueBooleanEditor;

        private LabelControl lbl_CustomField;

        public FilterBuilderType FilterableTypes
        {
            get
            {
                return _filterTypes;
            }
            set
            {
                _filterTypes = value;
                if (!_filterTypes.AllowFreeFormEntry)
                {
                    FieldEditor.TextEditStyle = TextEditStyles.DisableTextEditor;
                    lbl_CustomField.Visible = false;
                }
                else
                {
                    FieldEditor.TextEditStyle = TextEditStyles.Standard;
                    lbl_CustomField.Visible = true;
                }
                _filterPropertyCollection = new Dictionary<string, FilterProperty>();
                foreach (Type objectType in value.ObjectTypes)
                {
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(objectType))
                    {
                        if (_supportedTypes.Contains(property.PropertyType.ToString()) && !_filterPropertyCollection.ContainsKey(property.Name))
                        {
                            _filterPropertyCollection.Add(property.Name, new FilterProperty(property.Name, property.PropertyType, property.Attributes.Contains(new IsSystemAttribute(val: true))));
                        }
                    }
                }
                IEnumerable<string> keys = from k in _filterPropertyCollection.Keys
                                           where _commonFields.Contains(k)
                                           orderby _commonFields.ToList().IndexOf(k)
                                           select k;
                IEnumerable<string> strs = from k in _filterPropertyCollection.Keys
                                           where !_commonFields.Contains(k)
                                           orderby k
                                           select k;
                _fieldDividerItem = keys.LastOrDefault();
                FieldEditor.Items.AddRange(keys.Cast<object>().ToArray());
                FieldEditor.Items.AddRange(strs.Cast<object>().ToArray());
                UpdateUI();
            }
        }

        public IFilterExpression FilterExpression
        {
            get
            {
                return _filterExpression;
            }
            set
            {
                AddFilterExpression(value);
            }
        }

        public string LabelText
        {
            get
            {
                return w_lblDialogText.Text;
            }
            set
            {
                w_lblDialogText.Text = value;
                m_bLabelTextSetManually = true;
            }
        }

        static FilterExpressionEditorControl()
        {
            string[] strArrays = new string[11]
            {
            "Name", "DisplayName", "Title", "Author", "Created", "Modified", "CreatedBy", "ModifiedBy", "Url", "CreatedDate",
            "CreatedByUserName"
            };
            _commonFields = strArrays;
            string[] strArrays1 = new string[5] { "System.String", "System.DateTime", "System.Boolean", "System.Int32", "System.Nullable`1[System.Boolean]" };
            _supportedTypes = strArrays1;
        }

        public FilterExpressionEditorControl()
        {
            InitializeComponent();
            gridControl.DataSource = _dataSource;
            _dataSource.ListChanged += DataSourceOnListChanged;
            _dataSource.RemovingItem += DataSourceRemovingItem;
            Enum.GetNames(typeof(ExpressionLogic)).ToList().ForEach(delegate (string ex)
            {
                AndOrEditor.Items.Add(ex);
            });
            Enum.GetNames(typeof(FilterOperand)).ToList().ForEach(delegate (string op)
            {
                OperatorGeneralEditor.Items.Add(op);
            });
            OperatorDateTimeEditor.Items.Add(FilterOperand.Equals.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.NotEquals.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.GreaterThan.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.GreaterThanOrEqualTo.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.LessThan.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.LessThanOrEqualTo.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.IsNull.ToString());
            OperatorDateTimeEditor.Items.Add(FilterOperand.NotNull.ToString());
            OperatorBooleanEditor.Items.Add(FilterOperand.Equals.ToString());
            OperatorBooleanEditor.Items.Add(FilterOperand.NotEquals.ToString());
            addItem.Click += w_bAddFilter_Click;
            deleteitem.Click += w_DeleteFilters_Click;
            groupItem.Click += w_bGroupFilters_Click;
            ungroupItem.Click += w_bRemoveGroup_Click;
        }

        private void AddFilterExpression(IFilterExpression filterExpression)
        {
            _dataSource.Clear();
            _filterTree = null;
            _isInitializingData = true;
            if (filterExpression is FilterExpression)
            {
                _filterTree = AddSingleFilterExpression((FilterExpression)filterExpression, "", null);
            }
            if (filterExpression is FilterExpressionList filterExpressionList && filterExpressionList.Count > 0)
            {
                FilterNode filterNode = new FilterNode
                {
                    Parent = null,
                    Row = null,
                    Children = new List<FilterNode>()
                };
                _filterTree = filterNode;
                AddFilterExpressionListRecursive(filterExpressionList, "", _filterTree);
            }
            _filterExpression = null;
            _isInitializingData = false;
            UpdateFilterText();
            UpdateGroupColumns();
            UpdateButtonStatus();
        }

        private void AddFilterExpressionListRecursive(FilterExpressionList filterList, string previousLogic, FilterNode parent)
        {
            string str = filterList.Logic.ToString();
            int num = 0;
            foreach (IFilterExpression filterExpression in filterList)
            {
                if (!(filterExpression is FilterExpression))
                {
                    FilterNode filterNode = parent;
                    if (!((FilterExpressionList)filterExpression).IsImplicitGroup)
                    {
                        FilterNode filterNode1 = new FilterNode
                        {
                            Parent = parent,
                            Row = null,
                            Children = new List<FilterNode>()
                        };
                        FilterNode filterNode2 = filterNode1;
                        parent.Children.Add(filterNode2);
                        filterNode = filterNode2;
                    }
                    AddFilterExpressionListRecursive((FilterExpressionList)filterExpression, (num == 0) ? previousLogic : str, filterNode);
                }
                else
                {
                    FilterNode filterNode3 = AddSingleFilterExpression((FilterExpression)filterExpression, (num == 0) ? previousLogic : str, parent);
                    parent.Children.Add(filterNode3);
                }
                num++;
            }
        }

        private FilterNode AddSingleFilterExpression(FilterExpression filter, string logic, FilterNode parent)
        {
            FilterRow filterRow = new FilterRow
            {
                AndOr = logic,
                Field = filter.Property,
                Operator = filter.Operand.ToString(),
                Value = filter.Pattern
            };
            FilterRow filterRow1 = filterRow;
            if (_dataSource.Count == 0)
            {
                filterRow1.AndOr = "";
            }
            _dataSource.Add(filterRow1);
            return new FilterNode
            {
                Parent = parent,
                Row = filterRow1,
                Children = null
            };
        }

        public void ApplyBasicModeSkin()
        {
            w_bAddFilter.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bAddFilter.Width += 16;
            SimpleButton point = w_bAddFilter;
            Point location = w_bAddFilter.Location;
            Point location1 = w_bAddFilter.Location;
            point.Location = new Point(location.X - 16, location1.Y);
            w_bAddFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            w_bDeleteFilter.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bDeleteFilter.Width += 16;
            SimpleButton simpleButton = w_bDeleteFilter;
            Point point1 = w_bDeleteFilter.Location;
            Point location2 = w_bDeleteFilter.Location;
            simpleButton.Location = new Point(point1.X - 16, location2.Y);
            w_bDeleteFilter.LookAndFeel.UseDefaultLookAndFeel = false;
            w_bGroupFilters.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bGroupFilters.Width += 16;
            SimpleButton wBGroupFilters1 = w_bGroupFilters;
            Point point2 = w_bGroupFilters.Location;
            Point location3 = w_bGroupFilters.Location;
            wBGroupFilters1.Location = new Point(point2.X - 16, location3.Y);
            w_bGroupFilters.LookAndFeel.UseDefaultLookAndFeel = false;
            w_bRemoveGroup.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bRemoveGroup.Width += 16;
            SimpleButton wBRemoveGroup1 = w_bRemoveGroup;
            Point point3 = w_bRemoveGroup.Location;
            Point location4 = w_bRemoveGroup.Location;
            wBRemoveGroup1.Location = new Point(point3.X - 16, location4.Y);
            w_bRemoveGroup.LookAndFeel.UseDefaultLookAndFeel = false;
            gridControl.Width -= 16;
        }

        private void BuildExclusionString(IFilterExpression iFilter, StringBuilder sb)
        {
            if (!(iFilter is FilterExpressionList))
            {
                FilterExpression filterExpression = (FilterExpression)iFilter;
                sb.Append("\"" + filterExpression.Property + "\" must " + FilterControl.Translator[filterExpression.Operand]);
                if (filterExpression.Pattern != null)
                {
                    sb.Append(" \"" + filterExpression.Pattern + "\"");
                }
                return;
            }
            FilterExpressionList filterExpressionList = (FilterExpressionList)iFilter;
            string str = " " + filterExpressionList.Logic.ToString() + " \n";
            int num = 0;
            foreach (IFilterExpression filterExpression1 in filterExpressionList)
            {
                BuildExclusionString(filterExpression1, sb);
                num++;
                if (num != filterExpressionList.Count)
                {
                    sb.Append(str);
                }
            }
        }

        private IFilterExpression BuildFilterExpression()
        {
            if (_filterTree == null)
            {
                return null;
            }
            return BuildFilterExpressionRecursive(_filterTree).First();
        }

        private FilterExpressionList BuildFilterExpressionRecursive(FilterNode node, bool isRootGroup = true)
        {
            if (node.Children == null)
            {
                return BuildSingleFilterExpression(node);
            }
            FilterExpressionList filterExpressionList = null;
            IFilterExpression filterExpression = null;
            bool flag = true;
            foreach (FilterNode child in node.Children)
            {
                FilterExpressionList filterExpressionList1 = BuildFilterExpressionRecursive(child, isRootGroup: false);
                if (!flag)
                {
                    filterExpressionList1.Insert(0, filterExpression);
                    filterExpression = filterExpressionList1;
                    continue;
                }
                flag = false;
                filterExpressionList = filterExpressionList1;
                filterExpression = filterExpressionList1.First();
                filterExpressionList.Clear();
            }
            filterExpressionList.Add(filterExpression);
            if (!isRootGroup && filterExpression is FilterExpressionList)
            {
                ((FilterExpressionList)filterExpression).IsImplicitGroup = false;
            }
            return filterExpressionList;
        }

        private FilterExpressionList BuildSingleFilterExpression(FilterNode node)
        {
            FilterOperand filterOperand = (FilterOperand)Enum.Parse(typeof(FilterOperand), node.Row.Operator);
            FilterExpression filterExpression = new FilterExpression(filterOperand, FilterableTypes.ObjectTypes, node.Row.Field, node.Row.Value, bIsCaseSensitive: true, bIsBaseFilter: false, CultureInfo.CurrentCulture);
            ExpressionLogic expressionLogic = ExpressionLogic.And;
            if (node.Row.AndOr == "Or")
            {
                expressionLogic = ExpressionLogic.Or;
            }
            return new FilterExpressionList(expressionLogic) { filterExpression };
        }

        private void DataSourceOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
        {
            if (_isInitializingData)
            {
                return;
            }
            switch (listChangedEventArgs.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    FilterNode.AddNewRow(ref _filterTree, _dataSource[listChangedEventArgs.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    if (_dataSource.Count > 0)
                    {
                        _dataSource[0].AndOr = string.Empty;
                    }
                    break;
            }
        }

        private void DataSourceRemovingItem(object sender, RemovingEventArgs removingEventArgs)
        {
            FilterNode.RemoveRow(ref _filterTree, (FilterRow)removingEventArgs.RemovingObject);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FieldEditor_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            if (e.Item.ToString() == _fieldDividerItem)
            {
                Point point = new Point(e.Bounds.Left, e.Bounds.Bottom);
                Point point1 = new Point(e.Bounds.Width, e.Bounds.Bottom);
                e.Graphics.DrawLine(e.Cache.GetPen(Color.Gray), point, point1);
            }
        }

        private void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView gridView = (GridView)sender;
            string focusedRowCellValue = gridView.GetFocusedRowCellValue(FieldColumn) as string;
            string str = gridView.GetFocusedRowCellValue(OperatorColumn) as string;
            if (e.Column == FieldColumn)
            {
                UpdateOperatorEditors(focusedRowCellValue, e.RowHandle, clear: true);
                UpdateViewEditors(focusedRowCellValue, str, e.RowHandle, clear: true);
            }
            if (e.Column == OperatorColumn)
            {
                UpdateViewEditors(focusedRowCellValue, str, e.RowHandle, clear: true);
            }
        }

        private void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (!e.Column.Name.StartsWith("Unbound"))
            {
                return;
            }
            GridView gridView = (GridView)sender;
            if (!int.TryParse(e.Column.Name.Replace("Unbound", ""), out var num) || num < 0 || num >= _groupGrid.Count)
            {
                return;
            }
            FilterNode[] item = _groupGrid[num];
            int num1 = _dataSource.IndexOf((FilterRow)gridView.GetRow(e.RowHandle));
            if (num1 < 0 || num1 >= item.Count())
            {
                return;
            }
            FilterNode filterNode = item[num1];
            if (filterNode != null)
            {
                Graphics graphics = e.Graphics;
                Pen pen = new Pen(Color.Gray);
                Rectangle bounds = e.Bounds;
                bounds.Inflate(-1, 1);
                if (num1 == 0 || item[num1 - 1] != filterNode)
                {
                    graphics.DrawLine(pen, bounds.Left + 4, bounds.Top + 3, bounds.Right, bounds.Top + 3);
                    graphics.DrawLine(pen, bounds.Left + 4, bounds.Top + 3, bounds.Left + 4, bounds.Bottom);
                }
                else if (num1 >= item.Count() - 1 || item[num1 + 1] != filterNode)
                {
                    graphics.DrawLine(pen, bounds.Left + 4, bounds.Bottom - 3, bounds.Right, bounds.Bottom - 3);
                    graphics.DrawLine(pen, bounds.Left + 4, bounds.Top, bounds.Left + 4, bounds.Bottom - 3);
                }
                else
                {
                    graphics.DrawLine(pen, bounds.Left + 4, bounds.Top, bounds.Left + 4, bounds.Bottom);
                }
            }
        }

        private void gridView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            GridView gridView = (GridView)sender;
            string focusedRowCellValue = gridView.GetFocusedRowCellValue(FieldColumn) as string;
            string str = gridView.GetFocusedRowCellValue(OperatorColumn) as string;
            if (focusedRowCellValue != null)
            {
                if (gridView.FocusedColumn == ValueColumn)
                {
                    UpdateViewEditors(focusedRowCellValue, str, gridView.FocusedRowHandle);
                }
                if (gridView.FocusedColumn == OperatorColumn)
                {
                    UpdateOperatorEditors(focusedRowCellValue, gridView.FocusedRowHandle);
                }
            }
        }

        private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView gridView = (GridView)sender;
            string focusedRowCellValue = gridView.GetFocusedRowCellValue(FieldColumn) as string;
            string str = gridView.GetFocusedRowCellValue(OperatorColumn) as string;
            if (focusedRowCellValue != null)
            {
                if (gridView.FocusedColumn == ValueColumn)
                {
                    UpdateViewEditors(focusedRowCellValue, str, e.FocusedRowHandle);
                }
                if (gridView.FocusedColumn == OperatorColumn)
                {
                    UpdateOperatorEditors(focusedRowCellValue, e.FocusedRowHandle);
                }
            }
        }

        private void gridView_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
            GridView gridView = sender as GridView;
            if (XtraMessageBox.Show("Click OK to correct or Cancel to revert the edits.", "Empty cells in row", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                e.ExceptionMode = ExceptionMode.Ignore;
            }
            else if (gridView != null)
            {
                gridView.Focus();
                gridView.ShowEditor();
            }
        }

        private void gridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row)
            {
                e.Allow = false;
                return;
            }
            List<FilterRow> list = (from handle in gridView.GetSelectedRows()
                                    select (FilterRow)gridView.GetRow(handle) into row
                                    orderby _dataSource.IndexOf(row)
                                    select row).ToList();
            groupItem.Enabled = FilterNode.IsGroupable(_filterTree, list);
            ungroupItem.Enabled = FilterNode.IsUngroupable(_filterTree, list);
            e.Menu.Items.Clear();
            e.Menu.Items.Add(addItem);
            e.Menu.Items.Add(deleteitem);
            e.Menu.Items.Add(groupItem);
            e.Menu.Items.Add(ungroupItem);
        }

        private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Column == AndOrColumn && ((gridView.IsNewItemRow(e.RowHandle) && gridView.RowCount == 1) || (e.RowHandle == 0 && gridView.RowCount > 1)))
            {
                e.Appearance.BackColor = Color.Silver;
            }
        }

        private void gridView_RowUpdated(object sender, RowObjectEventArgs e)
        {
            UpdateUI();
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonStatus();
        }

        private void gridView_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (gridView.FocusedColumn == AndOrColumn)
            {
                if ((gridView.IsNewItemRow(gridView.FocusedRowHandle) && gridView.RowCount == 1) || (gridView.FocusedRowHandle == 0 && gridView.RowCount > 1))
                {
                    AndOrColumn.ColumnEdit = null;
                    e.Cancel = true;
                }
                else if (AndOrColumn.ColumnEdit == null)
                {
                    AndOrColumn.ColumnEdit = AndOrEditor;
                }
            }
            if (gridView.FocusedColumn == ValueColumn)
            {
                string focusedRowCellValue = gridView.GetFocusedRowCellValue(OperatorColumn) as string;
                if (_noValueRequiredOperators.Contains(focusedRowCellValue))
                {
                    ValueColumn.ColumnEdit = null;
                    e.Cancel = true;
                }
            }
        }

        private void gridView_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView gridView = sender as GridView;
            string rowCellValue = (string)gridView.GetRowCellValue(e.RowHandle, AndOrColumn);
            string str = (string)gridView.GetRowCellValue(e.RowHandle, FieldColumn);
            string rowCellValue1 = (string)gridView.GetRowCellValue(e.RowHandle, OperatorColumn);
            if (string.IsNullOrEmpty(rowCellValue) && ((e.RowHandle == -2147483647 && _dataSource.Count > 1) || e.RowHandle > 0))
            {
                gridView.SetColumnError(AndOrColumn, "You must enter in a logic operand.");
                e.Valid = false;
            }
            if (string.IsNullOrEmpty(str))
            {
                gridView.SetColumnError(FieldColumn, "You must enter in a field.");
                e.Valid = false;
            }
            if (string.IsNullOrEmpty(rowCellValue1))
            {
                gridView.SetColumnError(OperatorColumn, "You must enter in an operator.");
                e.Valid = false;
            }
        }

        private void InitializeComponent()
        {
            this.w_lblDialogText = new DevExpress.XtraEditors.LabelControl();
            this.w_bGroupFilters = new DevExpress.XtraEditors.SimpleButton();
            this.w_bRemoveGroup = new DevExpress.XtraEditors.SimpleButton();
            this.w_bAddFilter = new DevExpress.XtraEditors.SimpleButton();
            this.w_bDeleteFilter = new DevExpress.XtraEditors.SimpleButton();
            this.mem_filterExpressionText = new DevExpress.XtraEditors.MemoEdit();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.AndOrColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.AndOrEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.FieldColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FieldEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.OperatorColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OperatorGeneralEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.ValueColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ValueCalendarEditor = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.ValueTextEditor = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.ValueComboBoxEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.OperatorBooleanEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.OperatorDateTimeEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.ValueNumberEditor = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.ValueBooleanEditor = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.lbl_CustomField = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)this.mem_filterExpressionText.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.AndOrEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.FieldEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorGeneralEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueCalendarEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueCalendarEditor.VistaTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueTextEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueComboBoxEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorBooleanEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorDateTimeEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueNumberEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueBooleanEditor).BeginInit();
            base.SuspendLayout();
            this.w_lblDialogText.Location = new System.Drawing.Point(15, 11);
            this.w_lblDialogText.Name = "w_lblDialogText";
            this.w_lblDialogText.Size = new System.Drawing.Size(0, 13);
            this.w_lblDialogText.TabIndex = 0;
            this.w_bGroupFilters.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_bGroupFilters.Enabled = false;
            this.w_bGroupFilters.Location = new System.Drawing.Point(505, 176);
            this.w_bGroupFilters.Name = "w_bGroupFilters";
            this.w_bGroupFilters.Size = new System.Drawing.Size(120, 22);
            this.w_bGroupFilters.TabIndex = 5;
            this.w_bGroupFilters.Text = "Group Rows";
            this.w_bGroupFilters.Click += new System.EventHandler(w_bGroupFilters_Click);
            this.w_bRemoveGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_bRemoveGroup.Enabled = false;
            this.w_bRemoveGroup.Location = new System.Drawing.Point(505, 204);
            this.w_bRemoveGroup.Name = "w_bRemoveGroup";
            this.w_bRemoveGroup.Size = new System.Drawing.Size(120, 22);
            this.w_bRemoveGroup.TabIndex = 6;
            this.w_bRemoveGroup.Text = "Remove Grouping";
            this.w_bRemoveGroup.Click += new System.EventHandler(w_bRemoveGroup_Click);
            this.w_bAddFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_bAddFilter.Location = new System.Drawing.Point(505, 120);
            this.w_bAddFilter.Name = "w_bAddFilter";
            this.w_bAddFilter.Size = new System.Drawing.Size(120, 22);
            this.w_bAddFilter.TabIndex = 3;
            this.w_bAddFilter.Text = "Add Row";
            this.w_bAddFilter.Click += new System.EventHandler(w_bAddFilter_Click);
            this.w_bDeleteFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_bDeleteFilter.Location = new System.Drawing.Point(506, 148);
            this.w_bDeleteFilter.Name = "w_bDeleteFilter";
            this.w_bDeleteFilter.Size = new System.Drawing.Size(120, 22);
            this.w_bDeleteFilter.TabIndex = 4;
            this.w_bDeleteFilter.Text = "Delete Selected Rows";
            this.w_bDeleteFilter.Click += new System.EventHandler(w_DeleteFilters_Click);
            this.mem_filterExpressionText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.mem_filterExpressionText.Location = new System.Drawing.Point(15, 30);
            this.mem_filterExpressionText.Name = "mem_filterExpressionText";
            this.mem_filterExpressionText.Properties.ReadOnly = true;
            this.mem_filterExpressionText.Size = new System.Drawing.Size(610, 70);
            this.mem_filterExpressionText.TabIndex = 1;
            this.mem_filterExpressionText.TabStop = false;
            this.gridControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.gridControl.Location = new System.Drawing.Point(15, 120);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            DevExpress.XtraEditors.Repository.RepositoryItemCollection repositoryItems = this.gridControl.RepositoryItems;
            DevExpress.XtraEditors.Repository.RepositoryItem[] andOrEditor = new DevExpress.XtraEditors.Repository.RepositoryItem[10] { this.AndOrEditor, this.FieldEditor, this.OperatorGeneralEditor, this.ValueCalendarEditor, this.ValueTextEditor, this.ValueComboBoxEditor, this.OperatorBooleanEditor, this.OperatorDateTimeEditor, this.ValueNumberEditor, this.ValueBooleanEditor };
            repositoryItems.AddRange(andOrEditor);
            this.gridControl.Size = new System.Drawing.Size(475, 264);
            this.gridControl.TabIndex = 2;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
            DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gridView.Columns;
            DevExpress.XtraGrid.Columns.GridColumn[] andOrColumn = new DevExpress.XtraGrid.Columns.GridColumn[4] { this.AndOrColumn, this.FieldColumn, this.OperatorColumn, this.ValueColumn };
            columns.AddRange(andOrColumn);
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsCustomization.AllowSort = false;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridView_CustomDrawCell);
            this.gridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(gridView_RowCellStyle);
            this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_PopupMenuShowing);
            this.gridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(gridView_SelectionChanged);
            this.gridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(gridView_ShowingEditor);
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(gridView_FocusedRowChanged);
            this.gridView.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(gridView_FocusedColumnChanged);
            this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gridView_CellValueChanged);
            this.gridView.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(gridView_InvalidRowException);
            this.gridView.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(gridView_ValidateRow);
            this.gridView.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(gridView_RowUpdated);
            this.AndOrColumn.Caption = "And/Or";
            this.AndOrColumn.ColumnEdit = this.AndOrEditor;
            this.AndOrColumn.FieldName = "AndOr";
            this.AndOrColumn.Name = "AndOrColumn";
            this.AndOrColumn.Visible = true;
            this.AndOrColumn.VisibleIndex = 0;
            this.AndOrEditor.AutoHeight = false;
            this.AndOrEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.AndOrEditor.Name = "AndOrEditor";
            this.AndOrEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.FieldColumn.Caption = "Field";
            this.FieldColumn.ColumnEdit = this.FieldEditor;
            this.FieldColumn.FieldName = "Field";
            this.FieldColumn.Name = "FieldColumn";
            this.FieldColumn.Visible = true;
            this.FieldColumn.VisibleIndex = 1;
            this.FieldEditor.AutoHeight = false;
            this.FieldEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.FieldEditor.DropDownRows = 12;
            this.FieldEditor.Name = "FieldEditor";
            this.FieldEditor.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(FieldEditor_DrawItem);
            this.OperatorColumn.Caption = "Operator";
            this.OperatorColumn.ColumnEdit = this.OperatorGeneralEditor;
            this.OperatorColumn.FieldName = "Operator";
            this.OperatorColumn.Name = "OperatorColumn";
            this.OperatorColumn.Visible = true;
            this.OperatorColumn.VisibleIndex = 2;
            this.OperatorGeneralEditor.AutoHeight = false;
            this.OperatorGeneralEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.OperatorGeneralEditor.DropDownRows = 12;
            this.OperatorGeneralEditor.Name = "OperatorGeneralEditor";
            this.OperatorGeneralEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.ValueColumn.Caption = "Value";
            this.ValueColumn.FieldName = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.Visible = true;
            this.ValueColumn.VisibleIndex = 3;
            this.ValueCalendarEditor.AutoHeight = false;
            this.ValueCalendarEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.ValueCalendarEditor.DisplayFormat.FormatString = "g";
            this.ValueCalendarEditor.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.ValueCalendarEditor.EditFormat.FormatString = "g";
            this.ValueCalendarEditor.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.ValueCalendarEditor.Mask.EditMask = "g";
            this.ValueCalendarEditor.Name = "ValueCalendarEditor";
            this.ValueCalendarEditor.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.ValueCalendarEditor.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
            this.ValueCalendarEditor.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton()
            });
            this.ValueTextEditor.AutoHeight = false;
            this.ValueTextEditor.Name = "ValueTextEditor";
            this.ValueComboBoxEditor.AutoHeight = false;
            this.ValueComboBoxEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.ValueComboBoxEditor.DropDownRows = 12;
            this.ValueComboBoxEditor.Name = "ValueComboBoxEditor";
            this.OperatorBooleanEditor.AutoHeight = false;
            this.OperatorBooleanEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.OperatorBooleanEditor.Name = "OperatorBooleanEditor";
            this.OperatorBooleanEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.OperatorDateTimeEditor.AutoHeight = false;
            this.OperatorDateTimeEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.OperatorDateTimeEditor.Name = "OperatorDateTimeEditor";
            this.OperatorDateTimeEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.ValueNumberEditor.Appearance.Options.UseTextOptions = true;
            this.ValueNumberEditor.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ValueNumberEditor.AutoHeight = false;
            this.ValueNumberEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton()
            });
            this.ValueNumberEditor.Mask.EditMask = "d";
            this.ValueNumberEditor.Name = "ValueNumberEditor";
            this.ValueBooleanEditor.AutoHeight = false;
            this.ValueBooleanEditor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
            });
            this.ValueBooleanEditor.Items.AddRange(new object[2] { "True", "False" });
            this.ValueBooleanEditor.Name = "ValueBooleanEditor";
            this.ValueBooleanEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.lbl_CustomField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.lbl_CustomField.Location = new System.Drawing.Point(15, 390);
            this.lbl_CustomField.Name = "lbl_CustomField";
            this.lbl_CustomField.Size = new System.Drawing.Size(251, 13);
            this.lbl_CustomField.TabIndex = 0;
            this.lbl_CustomField.Text = "To enter a custom field, type it into the field column.";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.gridControl);
            base.Controls.Add(this.mem_filterExpressionText);
            base.Controls.Add(this.w_bRemoveGroup);
            base.Controls.Add(this.w_bDeleteFilter);
            base.Controls.Add(this.lbl_CustomField);
            base.Controls.Add(this.w_lblDialogText);
            base.Controls.Add(this.w_bGroupFilters);
            base.Controls.Add(this.w_bAddFilter);
            this.MinimumSize = new System.Drawing.Size(640, 420);
            base.Name = "FilterExpressionEditorControl";
            base.Size = new System.Drawing.Size(640, 420);
            ((System.ComponentModel.ISupportInitialize)this.mem_filterExpressionText.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.AndOrEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.FieldEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorGeneralEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueCalendarEditor.VistaTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueCalendarEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueTextEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueComboBoxEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorBooleanEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.OperatorDateTimeEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueNumberEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.ValueBooleanEditor).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void UpdateButtonStatus()
        {
            List<FilterRow> list = (from handle in gridView.GetSelectedRows()
                                    select (FilterRow)gridView.GetRow(handle) into row
                                    orderby _dataSource.IndexOf(row)
                                    select row).ToList();
            w_bGroupFilters.Enabled = FilterNode.IsGroupable(_filterTree, list);
            w_bRemoveGroup.Enabled = FilterNode.IsUngroupable(_filterTree, list);
        }

        private void UpdateFilterText()
        {
            _filterExpression = BuildFilterExpression();
            if (_filterExpression == null)
            {
                mem_filterExpressionText.Text = "No filter expression.";
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            BuildExclusionString(_filterExpression, stringBuilder);
            stringBuilder.Replace("\n", Environment.NewLine);
            mem_filterExpressionText.Text = stringBuilder.ToString();
        }

        private void UpdateGroupColumns()
        {
            GridView view = gridControl.MainView as GridView;
            _groupGrid = FilterNode.GetGroupGrid(_filterTree, _dataSource.ToList());
            view.BeginUpdate();
            List<GridColumn> list = new List<GridColumn>();
            foreach (GridColumn gridColumn in view.Columns)
            {
                if (gridColumn.UnboundType == UnboundColumnType.String)
                {
                    list.Add(gridColumn);
                }
            }
            list.ForEach(delegate (GridColumn c)
            {
                view.Columns.Remove(c);
            });
            List<GridColumn> list2 = new List<GridColumn>();
            int num = 0;
            foreach (FilterNode[] arg_D2_0 in _groupGrid)
            {
                GridColumn gridColumn2 = new GridColumn();
                gridColumn2.Name = "Unbound" + num;
                gridColumn2.FieldName = gridColumn2.Name;
                gridColumn2.UnboundType = UnboundColumnType.String;
                gridColumn2.OptionsColumn.AllowEdit = false;
                gridColumn2.OptionsColumn.ReadOnly = true;
                gridColumn2.OptionsColumn.AllowFocus = false;
                gridColumn2.OptionsColumn.AllowSize = false;
                gridColumn2.OptionsColumn.FixedWidth = true;
                gridColumn2.MaxWidth = 7;
                gridColumn2.MinWidth = 7;
                gridColumn2.Width = 7;
                gridColumn2.VisibleIndex = 0;
                gridColumn2.Caption = " ";
                gridColumn2.Visible = num != 0;
                list2.Add(gridColumn2);
                num++;
            }
            list2.Reverse();
            list2.ForEach(delegate (GridColumn c)
            {
                view.Columns.Insert(0, c);
            });
            view.EndUpdate();
        }

        private void UpdateOperatorEditors(string fieldValue, int rowHandle, bool clear = false)
        {
            RepositoryItem operatorGeneralEditor = OperatorGeneralEditor;
            FilterProperty filterProperty = null;
            bool flag = false;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                flag = _filterPropertyCollection.TryGetValue(fieldValue, out filterProperty);
            }
            if (flag)
            {
                if (filterProperty.PropertyType.ToString() == "System.String")
                {
                    operatorGeneralEditor = OperatorGeneralEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.Int32")
                {
                    operatorGeneralEditor = OperatorGeneralEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.DateTime")
                {
                    operatorGeneralEditor = OperatorDateTimeEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.Boolean")
                {
                    operatorGeneralEditor = OperatorBooleanEditor;
                }
            }
            if (OperatorColumn.ColumnEdit != operatorGeneralEditor)
            {
                if (clear)
                {
                    gridView.SetRowCellValue(rowHandle, OperatorColumn, string.Empty);
                }
                OperatorColumn.ColumnEdit = operatorGeneralEditor;
            }
        }

        private void UpdateUI()
        {
            UpdateFilterText();
            UpdateButtonStatus();
            UpdateGroupColumns();
        }

        private void UpdateViewEditors(string fieldValue, string operatorValue, int rowHandle, bool clear = false)
        {
            if (_noValueRequiredOperators.Contains(operatorValue))
            {
                gridView.SetRowCellValue(rowHandle, ValueColumn, string.Empty);
                ValueColumn.ColumnEdit = null;
                return;
            }
            RepositoryItem valueTextEditor = ValueTextEditor;
            FilterProperty filterProperty = null;
            bool flag = false;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                flag = _filterPropertyCollection.TryGetValue(fieldValue, out filterProperty);
            }
            if (flag)
            {
                if (filterProperty.PropertyType.ToString() == "System.String")
                {
                    valueTextEditor = ValueTextEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.Int32")
                {
                    valueTextEditor = ValueNumberEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.DateTime")
                {
                    valueTextEditor = ValueCalendarEditor;
                }
                if (filterProperty.PropertyType.ToString() == "System.Boolean")
                {
                    valueTextEditor = ValueBooleanEditor;
                }
            }
            if (ValueColumn.ColumnEdit != valueTextEditor)
            {
                if (clear)
                {
                    gridView.SetRowCellValue(rowHandle, ValueColumn, string.Empty);
                }
                ValueColumn.ColumnEdit = valueTextEditor;
            }
        }

        private void w_bAddFilter_Click(object sender, EventArgs e)
        {
            gridView.AddNewRow();
            gridView.FocusedColumn = ((gridView.RowCount == 1) ? FieldColumn : AndOrColumn);
            gridView.Focus();
            gridView.ShowEditor();
        }

        private void w_bGroupFilters_Click(object sender, EventArgs e)
        {
            List<FilterRow> list = (from handle in gridView.GetSelectedRows()
                                    select (FilterRow)gridView.GetRow(handle) into row
                                    orderby _dataSource.IndexOf(row)
                                    select row).ToList();
            FilterNode.GroupNodes(ref _filterTree, list);
            UpdateUI();
        }

        private void w_bRemoveGroup_Click(object sender, EventArgs e)
        {
            List<FilterRow> list = (from handle in gridView.GetSelectedRows()
                                    select (FilterRow)gridView.GetRow(handle) into row
                                    orderby _dataSource.IndexOf(row)
                                    select row).ToList();
            FilterNode.RemoveGroup(ref _filterTree, list);
            UpdateUI();
        }

        private void w_DeleteFilters_Click(object sender, EventArgs e)
        {
            foreach (int num in gridView.GetSelectedRows().Reverse())
            {
                gridView.DeleteRow(num);
            }
            UpdateUI();
        }
    }
}
