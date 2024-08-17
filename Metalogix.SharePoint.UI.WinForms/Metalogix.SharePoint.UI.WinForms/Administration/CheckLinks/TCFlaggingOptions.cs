using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Administration.CheckLinks;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.LinkFlagging32.ico")]
    [ControlName("Link Flagging")]
    public class TCFlaggingOptions : ScopableTabbableControl
    {
        private class FlagItem
        {
            public ExpressionLogic Logic { get; private set; }

            public StringFilterExpression StringFilter { get; private set; }

            public FlagItem(ExpressionLogic logic, StringFilterExpression stringFilter)
            {
                Logic = logic;
                StringFilter = stringFilter;
            }
        }

        public const string ContainsOperator = "Contains";

        public const string NotContainsOperator = "Does not contain";

        public const string StartsWithOperator = "Starts with";

        public const string NotStartsWithOperator = "Does not start with";

        public const string EndsWithOperator = "Ends with";

        public const string NotEndsWithOperator = "Does not end with";

        private IDictionary<DataRow, FlagItem> _dataRowToFlagLookup;

        private DataTable _dataTable;

        private SPFlaggingOptions m_Options;

        private IContainer components;

        private BarManager w_toolStrip;

        private Bar barControl;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarButtonItem w_btnAdd;

        private BarButtonItem w_btnEdit;

        private BarButtonItem w_btnRemove;

        private ColumnHeader w_chFlagType;

        private ColumnHeader w_chCondition;

        private ColumnHeader w_chValue;

        private BarButtonItem barButtonItem1;

        private GridControl gridControlFlagsList;

        private GridView gridViewFlagsList;

        public SPFlaggingOptions Options
        {
            get
            {
                return m_Options;
            }
            set
            {
                m_Options = value;
                LoadUI();
            }
        }

        public TCFlaggingOptions()
        {
            InitializeComponent();
            InitializeDataGrid();
        }

        private void AddNewFlagFilter(ExpressionLogic flagType, StringFilterExpression filter)
        {
            if (!(Options.FlagFilterList is FilterExpressionList filterExpressionList))
            {
                return;
            }
            if (flagType == ExpressionLogic.And)
            {
                filterExpressionList.Add(filter);
                return;
            }
            FilterExpressionList filterExpressionList2 = null;
            foreach (IFilterExpression item in filterExpressionList)
            {
                if (!(item is FilterExpressionList filterExpressionList3) || filterExpressionList3.Logic != ExpressionLogic.Or)
                {
                    continue;
                }
                filterExpressionList2 = filterExpressionList3;
                break;
            }
            if (filterExpressionList2 == null)
            {
                filterExpressionList2 = new FilterExpressionList(ExpressionLogic.Or);
                filterExpressionList.Add(filterExpressionList2);
            }
            filterExpressionList2.Add(filter);
        }

        private void AppendToDataTable(ExpressionLogic logic, StringFilterExpression filterExpression)
        {
            AppendToDataTable(new FlagItem(logic, filterExpression));
        }

        private void AppendToDataTable(FlagItem item)
        {
            DataRowCollection rows = _dataTable.Rows;
            object[] values = new object[3]
            {
            item.Logic,
            ConvertFilterOperandToFlagConditionOperator(item.StringFilter.Operand),
            item.StringFilter.Pattern
            };
            DataRow key = rows.Add(values);
            _dataRowToFlagLookup.Add(key, item);
        }

        public static string ConvertFilterOperandToFlagConditionOperator(FilterOperand operand)
        {
            switch (operand)
            {
                case FilterOperand.StartsWith:
                    return "Starts with";
                case FilterOperand.NotStartsWith:
                    return "Does not start with";
                case FilterOperand.EndsWith:
                    return "Ends with";
                case FilterOperand.NotEndsWith:
                    return "Does not end with";
                case FilterOperand.Contains:
                    return "Contains";
                case FilterOperand.NotContains:
                    return "Does not contain";
                default:
                    return "Contains";
            }
        }

        public static FilterOperand ConvertFlagConditionOperatorToFilterOperand(string sFlagConditionOperator)
        {
            if (sFlagConditionOperator != null)
            {
                switch (sFlagConditionOperator)
                {
                    case "Contains":
                        return FilterOperand.Contains;
                    case "Does not contain":
                        return FilterOperand.NotContains;
                    case "Starts with":
                        return FilterOperand.StartsWith;
                    case "Does not start with":
                        return FilterOperand.NotStartsWith;
                    case "Ends with":
                        return FilterOperand.EndsWith;
                    case "Does not end with":
                        return FilterOperand.NotEndsWith;
                }
            }
            return FilterOperand.Contains;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void gridViewFlagsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEnabledState();
        }

        private void InitializeComponent()
        {
            this.components = new global::System.ComponentModel.Container();
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks.TCFlaggingOptions));
            this.w_toolStrip = new global::DevExpress.XtraBars.BarManager(this.components);
            this.barControl = new global::DevExpress.XtraBars.Bar();
            this.w_btnAdd = new global::DevExpress.XtraBars.BarButtonItem();
            this.w_btnEdit = new global::DevExpress.XtraBars.BarButtonItem();
            this.w_btnRemove = new global::DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new global::DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new global::DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new global::DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new global::DevExpress.XtraBars.BarDockControl();
            this.w_chFlagType = new global::System.Windows.Forms.ColumnHeader();
            this.w_chCondition = new global::System.Windows.Forms.ColumnHeader();
            this.w_chValue = new global::System.Windows.Forms.ColumnHeader();
            this.gridControlFlagsList = new global::DevExpress.XtraGrid.GridControl();
            this.gridViewFlagsList = new global::DevExpress.XtraGrid.Views.Grid.GridView();
            ((global::System.ComponentModel.ISupportInitialize)this.w_toolStrip).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.gridControlFlagsList).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.gridViewFlagsList).BeginInit();
            base.SuspendLayout();
            this.w_toolStrip.Bars.AddRange(new global::DevExpress.XtraBars.Bar[1] { this.barControl });
            this.w_toolStrip.DockControls.Add(this.barDockControlTop);
            this.w_toolStrip.DockControls.Add(this.barDockControlBottom);
            this.w_toolStrip.DockControls.Add(this.barDockControlLeft);
            this.w_toolStrip.DockControls.Add(this.barDockControlRight);
            this.w_toolStrip.Form = this;
            global::DevExpress.XtraBars.BarItems items = this.w_toolStrip.Items;
            global::DevExpress.XtraBars.BarItem[] items2 = new global::DevExpress.XtraBars.BarItem[3] { this.w_btnAdd, this.w_btnEdit, this.w_btnRemove };
            items.AddRange(items2);
            this.w_toolStrip.MaxItemId = 4;
            this.barControl.BarName = "Link Controls";
            this.barControl.DockCol = 0;
            this.barControl.DockRow = 0;
            this.barControl.DockStyle = global::DevExpress.XtraBars.BarDockStyle.Bottom;
            global::DevExpress.XtraBars.LinksInfo linksPersistInfo = this.barControl.LinksPersistInfo;
            global::DevExpress.XtraBars.LinkPersistInfo[] links = new global::DevExpress.XtraBars.LinkPersistInfo[3]
            {
            new global::DevExpress.XtraBars.LinkPersistInfo(this.w_btnAdd),
            new global::DevExpress.XtraBars.LinkPersistInfo(this.w_btnEdit),
            new global::DevExpress.XtraBars.LinkPersistInfo(this.w_btnRemove)
            };
            linksPersistInfo.AddRange(links);
            this.barControl.OptionsBar.AllowQuickCustomization = false;
            this.barControl.OptionsBar.DrawDragBorder = false;
            this.barControl.OptionsBar.UseWholeRow = true;
            resources.ApplyResources(this.barControl, "barControl");
            resources.ApplyResources(this.w_btnAdd, "w_btnAdd");
            this.w_btnAdd.Glyph = global::Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
            this.w_btnAdd.Id = 0;
            this.w_btnAdd.Name = "w_btnAdd";
            this.w_btnAdd.PaintStyle = global::DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.w_btnAdd.ItemClick += new global::DevExpress.XtraBars.ItemClickEventHandler(On_btnAdd_Click);
            resources.ApplyResources(this.w_btnEdit, "w_btnEdit");
            this.w_btnEdit.Enabled = false;
            this.w_btnEdit.Glyph = global::Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
            this.w_btnEdit.Id = 1;
            this.w_btnEdit.Name = "w_btnEdit";
            this.w_btnEdit.PaintStyle = global::DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.w_btnEdit.ItemClick += new global::DevExpress.XtraBars.ItemClickEventHandler(On_btnEdit_Click);
            resources.ApplyResources(this.w_btnRemove, "w_btnRemove");
            this.w_btnRemove.Enabled = false;
            this.w_btnRemove.Glyph = global::Metalogix.SharePoint.UI.WinForms.Properties.Resources.Minus16;
            this.w_btnRemove.Id = 2;
            this.w_btnRemove.Name = "w_btnRemove";
            this.w_btnRemove.PaintStyle = global::DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.w_btnRemove.ItemClick += new global::DevExpress.XtraBars.ItemClickEventHandler(On_btnRemove_Click);
            this.barDockControlTop.CausesValidation = false;
            resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
            this.barDockControlBottom.CausesValidation = false;
            resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            this.barDockControlLeft.CausesValidation = false;
            resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            this.barDockControlRight.CausesValidation = false;
            resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
            resources.ApplyResources(this.w_chFlagType, "w_chFlagType");
            resources.ApplyResources(this.w_chCondition, "w_chCondition");
            resources.ApplyResources(this.w_chValue, "w_chValue");
            resources.ApplyResources(this.gridControlFlagsList, "gridControlFlagsList");
            this.gridControlFlagsList.MainView = this.gridViewFlagsList;
            this.gridControlFlagsList.MenuManager = this.w_toolStrip;
            this.gridControlFlagsList.Name = "gridControlFlagsList";
            this.gridControlFlagsList.ViewCollection.AddRange(new global::DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridViewFlagsList });
            this.gridViewFlagsList.FocusRectStyle = global::DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewFlagsList.GridControl = this.gridControlFlagsList;
            this.gridViewFlagsList.GroupFooterShowMode = global::DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this.gridViewFlagsList.Name = "gridViewFlagsList";
            this.gridViewFlagsList.OptionsBehavior.Editable = false;
            this.gridViewFlagsList.OptionsBehavior.ReadOnly = true;
            this.gridViewFlagsList.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewFlagsList.OptionsCustomization.AllowFilter = false;
            this.gridViewFlagsList.OptionsCustomization.AllowGroup = false;
            this.gridViewFlagsList.OptionsMenu.EnableColumnMenu = false;
            this.gridViewFlagsList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewFlagsList.OptionsSelection.MultiSelect = true;
            this.gridViewFlagsList.OptionsView.ShowGroupPanel = false;
            this.gridViewFlagsList.OptionsView.ShowIndicator = false;
            this.gridViewFlagsList.SelectionChanged += new global::DevExpress.Data.SelectionChangedEventHandler(gridViewFlagsList_SelectionChanged);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.gridControlFlagsList);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.Name = "TCFlaggingOptions";
            ((global::System.ComponentModel.ISupportInitialize)this.w_toolStrip).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.gridControlFlagsList).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.gridViewFlagsList).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeDataGrid()
        {
            _dataRowToFlagLookup = new Dictionary<DataRow, FlagItem>();
            _dataTable = new DataTable("FlagsList");
            _dataTable.Columns.Add("Flag Type");
            _dataTable.Columns.Add("Condition");
            _dataTable.Columns.Add("Value");
            gridControlFlagsList.DataSource = _dataTable;
        }

        protected override void LoadUI()
        {
            UpdateUI();
        }

        private void On_btnAdd_Click(object sender, ItemClickEventArgs e)
        {
            CheckLinksFlagDialog checkLinksFlagDialog = new CheckLinksFlagDialog();
            if (checkLinksFlagDialog.ShowDialog() == DialogResult.OK && checkLinksFlagDialog.Filter != null)
            {
                AddNewFlagFilter(checkLinksFlagDialog.FlagType, checkLinksFlagDialog.Filter);
                UpdateUI();
            }
        }

        private void On_btnEdit_Click(object sender, ItemClickEventArgs e)
        {
            if (gridViewFlagsList.SelectedRowsCount != 1)
            {
                return;
            }
            int rowHandle = gridViewFlagsList.GetSelectedRows().First();
            DataRow dataRow = gridViewFlagsList.GetDataRow(rowHandle);
            FlagItem flagItem = _dataRowToFlagLookup[dataRow];
            ExpressionLogic logic = flagItem.Logic;
            StringFilterExpression stringFilter = flagItem.StringFilter;
            CheckLinksFlagDialog checkLinksFlagDialog = new CheckLinksFlagDialog
            {
                FlagType = logic,
                Filter = stringFilter
            };
            if (checkLinksFlagDialog.ShowDialog() == DialogResult.OK)
            {
                StringFilterExpression stringFilter2 = flagItem.StringFilter;
                if (checkLinksFlagDialog.FlagType == logic)
                {
                    stringFilter2.Operand = checkLinksFlagDialog.Filter.Operand;
                    stringFilter2.Pattern = checkLinksFlagDialog.Filter.Pattern;
                }
                else
                {
                    RemoveFlagFilter(stringFilter2);
                    AddNewFlagFilter(checkLinksFlagDialog.FlagType, checkLinksFlagDialog.Filter);
                }
                UpdateUI();
            }
        }

        private void On_btnRemove_Click(object sender, ItemClickEventArgs e)
        {
            if (gridViewFlagsList.SelectedRowsCount <= 0)
            {
                return;
            }
            IFilterExpression flagFilterList = Options.FlagFilterList;
            List<IFilterExpression> list = new List<IFilterExpression>();
            int[] selectedRows = gridViewFlagsList.GetSelectedRows();
            foreach (int rowHandle in selectedRows)
            {
                DataRow dataRow = gridViewFlagsList.GetDataRow(rowHandle);
                list.Add(_dataRowToFlagLookup[dataRow].StringFilter);
            }
            foreach (IFilterExpression item in list)
            {
                RemoveFlagFilter(item);
            }
            UpdateUI();
        }

        private void RefreshDataTable(FilterExpressionList filterList)
        {
            _dataRowToFlagLookup.Clear();
            _dataTable.Rows.Clear();
            if (filterList == null)
            {
                return;
            }
            foreach (IFilterExpression filter in filterList)
            {
                Type type = filter.GetType();
                if (typeof(FilterExpressionList).IsAssignableFrom(type))
                {
                    FilterExpressionList filterExpressionList = (FilterExpressionList)filter;
                    if (filterExpressionList.Logic != ExpressionLogic.Or)
                    {
                        FlatXtraMessageBox.Show("An error has occurred internally while accessing the link flags. The check links operation will still work but flagging may not work as expected. Please contact support@metalogix.net with this error message. Message: Inner flag lists can only use the OR operator.", "Flag Filter Internal Error", MessageBoxButtons.OK);
                    }
                    foreach (IFilterExpression item in filterExpressionList)
                    {
                        if (!typeof(StringFilterExpression).IsAssignableFrom(item.GetType()))
                        {
                            FlatXtraMessageBox.Show("An error has occurred internally while accessing the link flags. The check links operation will still work but flagging may not work as expected. Please contact support@metalogix.net with this error message. Message: Only string filters are allowed inside the inner OR list.", "Flag Filter Internal Error", MessageBoxButtons.OK);
                        }
                        else
                        {
                            AppendToDataTable(ExpressionLogic.Or, item as StringFilterExpression);
                        }
                    }
                }
                else if (!typeof(StringFilterExpression).IsAssignableFrom(type))
                {
                    string text = "An error has occurred internally while accessing the link flags. The check links operation will still work but flagging may not work as expected. Please contact support@metalogix.net with this error message. Message: Unsupported filter type '" + type.ToString() + "'inside flag list.";
                    FlatXtraMessageBox.Show(text, "Flag Filter Internal Error", MessageBoxButtons.OK);
                }
                else
                {
                    AppendToDataTable(ExpressionLogic.And, filter as StringFilterExpression);
                }
            }
        }

        private void RemoveFlagFilter(IFilterExpression filter)
        {
            if (!(Options.FlagFilterList is FilterExpressionList filterExpressionList) || filterExpressionList.Remove(filter))
            {
                return;
            }
            foreach (IFilterExpression item in filterExpressionList)
            {
                if (!(item is FilterExpressionList filterExpressionList2) || !filterExpressionList2.Remove(filter))
                {
                    continue;
                }
                try
                {
                    if (filterExpressionList2.Count <= 0)
                    {
                        filterExpressionList.Remove(filterExpressionList2);
                    }
                    break;
                }
                catch
                {
                    break;
                }
            }
        }

        protected override void UpdateEnabledState()
        {
            w_btnEdit.Enabled = gridViewFlagsList.SelectedRowsCount == 1;
            w_btnRemove.Enabled = gridViewFlagsList.SelectedRowsCount > 0;
        }

        private void UpdateUI()
        {
            try
            {
                RefreshDataTable(Options.FlagFilterList as FilterExpressionList);
            }
            catch (Exception)
            {
            }
        }
    }
}