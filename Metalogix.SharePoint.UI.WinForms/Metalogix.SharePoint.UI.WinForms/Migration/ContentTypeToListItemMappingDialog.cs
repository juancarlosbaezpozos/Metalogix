using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Data.Filters;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class ContentTypeToListItemMappingDialog : XtraForm
	{
		private const string NONE = "None";

		private const string ITEMS = "Items";

		private const string FOLDERS = "Folders";

		private const string BOTH = "Both";

		private const string FIELD_CONTENTYPE = "ContentType";

		private const string FIELD_APPCONDITION = "ApplicationCondition";

		private const string FIELD_ITEMTYPE = "ItemType";

		private int m_iContentTypeColMinWidth;

		private int m_iApplicationConditionColMinWidth;

		private int m_iItemTypeColMinWidth;

		private static FilterExpression ITEMS_FILTER;

		private static FilterExpression FOLDERS_FILTER;

		private static FilterExpression LEGACY_ITEMS_FILTER;

		private IEnumerable<ContentTypeApplicationOptions> m_options;

		private DataTable _dataSource;

		private Dictionary<DataRow, IFilterExpression> _filterMap;

		private Color _defaultCellForeColor;

		private Color _defaultCellBackColor;

		private IContainer components;

		private LabelControl w_lbDescription;

		private SimpleButton w_bEdit;

		private SimpleButton w_bClear;

		private SimpleButton w_bOkay;

		private SimpleButton w_bCancel;

		private GridControl _mappingGrid;

		private GridView _mappingGridView;

		private GridColumn _contentTypeColumn;

		private GridColumn _applicationConditionColumn;

		private GridColumn _itemTypeColumn;

		private RepositoryItemComboBox _itemTypeComboBox;

		public IEnumerable<ContentTypeApplicationOptions> Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		static ContentTypeToListItemMappingDialog()
		{
			ITEMS_FILTER = new FilterExpression(FilterOperand.NotEquals, "Metalogix.SharePoint.SPListItem", "FSObjType", "1", bIsCaseSensitive: true, bIsBaseFilter: false);
			FOLDERS_FILTER = new FilterExpression(FilterOperand.Equals, "Metalogix.SharePoint.SPListItem", "FSObjType", "1", bIsCaseSensitive: true, bIsBaseFilter: false);
			LEGACY_ITEMS_FILTER = new FilterExpression(FilterOperand.Equals, "Metalogix.SharePoint.SPListItem", "FSObjType", "0", bIsCaseSensitive: true, bIsBaseFilter: false);
		}

		public ContentTypeToListItemMappingDialog()
		{
			InitializeComponent();
			InitializeGridView();
		}

		private void _itemTypeComboBox_EditValueChanged(object sender, EventArgs e)
		{
			_mappingGridView.PostEditor();
			_mappingGridView.LayoutChanged();
			UpdateEnabled();
		}

		private void _mappingGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
		{
			UpdateEnabled();
		}

		private void _mappingGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
		{
			if (e.Column.FieldName != "ApplicationCondition")
			{
				e.Appearance.ForeColor = _defaultCellForeColor;
				e.Appearance.BackColor = _defaultCellBackColor;
				return;
			}
			string text = (string)_mappingGridView.GetRowCellValue(e.RowHandle, "ItemType");
			if (string.IsNullOrEmpty(text) || text == "None")
			{
				e.Appearance.ForeColor = Color.Gray;
				e.Appearance.BackColor = Control.DefaultBackColor;
			}
			else
			{
				e.Appearance.ForeColor = _defaultCellForeColor;
				e.Appearance.BackColor = _defaultCellBackColor;
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

		private DataTable GetNewTableForDataSource()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ContentType");
			dataTable.Columns.Add("ApplicationCondition");
			dataTable.Columns.Add("ItemType");
			return dataTable;
		}

		private DataRow GetSelectedRow()
		{
			if (_mappingGridView.FocusedRowHandle < 0)
			{
				return null;
			}
			return ((DataRowView)_mappingGridView.GetRow(_mappingGridView.FocusedRowHandle)).Row;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.ContentTypeToListItemMappingDialog));
			this.w_lbDescription = new DevExpress.XtraEditors.LabelControl();
			this.w_bEdit = new DevExpress.XtraEditors.SimpleButton();
			this.w_bClear = new DevExpress.XtraEditors.SimpleButton();
			this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
			this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
			this._mappingGrid = new DevExpress.XtraGrid.GridControl();
			this._mappingGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this._contentTypeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this._applicationConditionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this._itemTypeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this._itemTypeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)this._mappingGrid).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._mappingGridView).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._itemTypeComboBox).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lbDescription, "w_lbDescription");
			this.w_lbDescription.Name = "w_lbDescription";
			resources.ApplyResources(this.w_bEdit, "w_bEdit");
			this.w_bEdit.Name = "w_bEdit";
			this.w_bEdit.Click += new System.EventHandler(w_bEdit_Click);
			resources.ApplyResources(this.w_bClear, "w_bClear");
			this.w_bClear.Name = "w_bClear";
			this.w_bClear.Click += new System.EventHandler(w_bClear_Click);
			resources.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Click += new System.EventHandler(w_bOkay_Click);
			resources.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Click += new System.EventHandler(w_bCancel_Click);
			resources.ApplyResources(this._mappingGrid, "_mappingGrid");
			this._mappingGrid.EmbeddedNavigator.ShowToolTips = false;
			this._mappingGrid.MainView = this._mappingGridView;
			this._mappingGrid.Name = "_mappingGrid";
			this._mappingGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this._itemTypeComboBox });
			this._mappingGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._mappingGridView });
			DevExpress.XtraGrid.Columns.GridColumnCollection columns = this._mappingGridView.Columns;
			DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new DevExpress.XtraGrid.Columns.GridColumn[3] { this._contentTypeColumn, this._applicationConditionColumn, this._itemTypeColumn };
			columns.AddRange(columns2);
			this._mappingGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this._mappingGridView.GridControl = this._mappingGrid;
			this._mappingGridView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
			this._mappingGridView.Name = "_mappingGridView";
			this._mappingGridView.OptionsBehavior.AutoUpdateTotalSummary = false;
			this._mappingGridView.OptionsCustomization.AllowColumnMoving = false;
			this._mappingGridView.OptionsCustomization.AllowFilter = false;
			this._mappingGridView.OptionsCustomization.AllowGroup = false;
			this._mappingGridView.OptionsCustomization.AllowSort = false;
			this._mappingGridView.OptionsDetail.AllowZoomDetail = false;
			this._mappingGridView.OptionsDetail.EnableMasterViewMode = false;
			this._mappingGridView.OptionsDetail.ShowDetailTabs = false;
			this._mappingGridView.OptionsDetail.SmartDetailExpand = false;
			this._mappingGridView.OptionsFilter.AllowColumnMRUFilterList = false;
			this._mappingGridView.OptionsFilter.AllowMRUFilterList = false;
			this._mappingGridView.OptionsHint.ShowCellHints = false;
			this._mappingGridView.OptionsHint.ShowColumnHeaderHints = false;
			this._mappingGridView.OptionsHint.ShowFooterHints = false;
			this._mappingGridView.OptionsMenu.EnableColumnMenu = false;
			this._mappingGridView.OptionsMenu.EnableFooterMenu = false;
			this._mappingGridView.OptionsMenu.EnableGroupPanelMenu = false;
			this._mappingGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
			this._mappingGridView.OptionsView.ShowDetailButtons = false;
			this._mappingGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this._mappingGridView.OptionsView.ShowGroupPanel = false;
			this._mappingGridView.OptionsView.ShowIndicator = false;
			this._mappingGridView.SynchronizeClones = false;
			this._mappingGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(_mappingGridView_RowCellStyle);
			this._mappingGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(_mappingGridView_FocusedRowChanged);
			resources.ApplyResources(this._contentTypeColumn, "_contentTypeColumn");
			this._contentTypeColumn.FieldName = "ContentType";
			this._contentTypeColumn.Name = "_contentTypeColumn";
			this._contentTypeColumn.OptionsColumn.AllowEdit = false;
			this._contentTypeColumn.OptionsColumn.FixedWidth = true;
			this._contentTypeColumn.OptionsColumn.ReadOnly = true;
			this._contentTypeColumn.OptionsColumn.TabStop = false;
			resources.ApplyResources(this._applicationConditionColumn, "_applicationConditionColumn");
			this._applicationConditionColumn.FieldName = "ApplicationCondition";
			this._applicationConditionColumn.Name = "_applicationConditionColumn";
			this._applicationConditionColumn.OptionsColumn.AllowEdit = false;
			this._applicationConditionColumn.OptionsColumn.ReadOnly = true;
			this._applicationConditionColumn.OptionsColumn.TabStop = false;
			resources.ApplyResources(this._itemTypeColumn, "_itemTypeColumn");
			this._itemTypeColumn.ColumnEdit = this._itemTypeComboBox;
			this._itemTypeColumn.FieldName = "ItemType";
			this._itemTypeColumn.Name = "_itemTypeColumn";
			this._itemTypeColumn.OptionsColumn.FixedWidth = true;
			this._itemTypeColumn.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
			resources.ApplyResources(this._itemTypeComboBox, "_itemTypeComboBox");
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this._itemTypeComboBox.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons.AddRange(buttons2);
			DevExpress.XtraEditors.Controls.ComboBoxItemCollection items = this._itemTypeComboBox.Items;
			object[] items2 = new object[4]
			{
				resources.GetString("_itemTypeComboBox.Items"),
				resources.GetString("_itemTypeComboBox.Items1"),
				resources.GetString("_itemTypeComboBox.Items2"),
				resources.GetString("_itemTypeComboBox.Items3")
			};
			items.AddRange(items2);
			this._itemTypeComboBox.Name = "_itemTypeComboBox";
			this._itemTypeComboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this._itemTypeComboBox.EditValueChanged += new System.EventHandler(_itemTypeComboBox_EditValueChanged);
			base.AcceptButton = this.w_bOkay;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this._mappingGrid);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bClear);
			base.Controls.Add(this.w_bEdit);
			base.Controls.Add(this.w_lbDescription);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ContentTypeToListItemMappingDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)this._mappingGrid).EndInit();
			((System.ComponentModel.ISupportInitialize)this._mappingGridView).EndInit();
			((System.ComponentModel.ISupportInitialize)this._itemTypeComboBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void InitializeGridView()
		{
			_dataSource = GetNewTableForDataSource();
			_mappingGrid.DataSource = _dataSource;
			_filterMap = new Dictionary<DataRow, IFilterExpression>();
			Skin skin = GridSkins.GetSkin(_mappingGrid.LookAndFeel);
			SkinElement skinElement = skin[GridSkins.SkinGridRow];
			_defaultCellBackColor = skinElement.Color.BackColor;
			_defaultCellForeColor = skinElement.Color.ForeColor;
		}

		private void LoadUI()
		{
			DataTable newTableForDataSource = GetNewTableForDataSource();
			_filterMap.Clear();
			if (Options == null)
			{
				_mappingGrid.DataSource = newTableForDataSource;
				return;
			}
			foreach (ContentTypeApplicationOptions option in Options)
			{
				string value = "None";
				IFilterExpression filterExpression = null;
				if (option.MapItemsFilter != null)
				{
					FilterExpressionList filterExpressionList = (FilterExpressionList)option.MapItemsFilter;
					if (filterExpressionList.Count > 0)
					{
						FilterExpressionList filterExpressionList2 = new FilterExpressionList(ExpressionLogic.Or) { ITEMS_FILTER, FOLDERS_FILTER };
						FilterExpressionList filterExpressionList3 = new FilterExpressionList(ExpressionLogic.Or) { LEGACY_ITEMS_FILTER, FOLDERS_FILTER };
						if (filterExpressionList.Equals(filterExpressionList2) || filterExpressionList.Equals(filterExpressionList3))
						{
							value = "Both";
						}
						else
						{
							foreach (IFilterExpression item in filterExpressionList)
							{
								if (item.Equals(filterExpressionList2) || item.Equals(filterExpressionList3))
								{
									value = "Both";
								}
								else if (item.Equals(ITEMS_FILTER) || item.Equals(LEGACY_ITEMS_FILTER))
								{
									value = "Items";
								}
								else if (!item.Equals(FOLDERS_FILTER))
								{
									filterExpression = item;
								}
								else
								{
									value = "Folders";
								}
							}
						}
					}
				}
				string value2 = ((filterExpression == null) ? "" : filterExpression.GetLogicString());
				DataRow dataRow = newTableForDataSource.NewRow();
				dataRow["ContentType"] = option.ContentTypeName;
				dataRow["ApplicationCondition"] = value2;
				dataRow["ItemType"] = value;
				newTableForDataSource.Rows.Add(dataRow);
				_filterMap.Add(dataRow, filterExpression);
			}
			_dataSource = newTableForDataSource;
			_mappingGrid.DataSource = _dataSource;
			UpdateEnabled();
		}

		private bool SaveUI()
		{
			if (Options == null)
			{
				return true;
			}
			foreach (ContentTypeApplicationOptions option in Options)
			{
				foreach (DataRow row in _dataSource.Rows)
				{
					if (!row["ContentType"].Equals(option.ContentTypeName))
					{
						continue;
					}
					option.MapItemsFilter = _filterMap[row];
					if (row["ItemType"].Equals("Items"))
					{
						if (option.MapItemsFilter is FilterExpressionList)
						{
							option.MapItemsFilter = (FilterExpressionList)option.MapItemsFilter & ITEMS_FILTER;
						}
						else if (!(option.MapItemsFilter is FilterExpression))
						{
							option.MapItemsFilter = new FilterExpressionList(ExpressionLogic.And) { ITEMS_FILTER };
						}
						else
						{
							option.MapItemsFilter = (FilterExpression)option.MapItemsFilter & ITEMS_FILTER;
						}
					}
					else if (!row["ItemType"].Equals("Folders"))
					{
						if (!row["ItemType"].Equals("Both"))
						{
							if (row["ItemType"].Equals("None"))
							{
								option.MapItemsFilter = null;
							}
							break;
						}
						FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.Or) { ITEMS_FILTER, FOLDERS_FILTER };
						if (option.MapItemsFilter is FilterExpressionList)
						{
							option.MapItemsFilter = (FilterExpressionList)option.MapItemsFilter & filterExpressionList;
						}
						else if (!(option.MapItemsFilter is FilterExpression))
						{
							option.MapItemsFilter = filterExpressionList;
						}
						else
						{
							option.MapItemsFilter = (FilterExpression)option.MapItemsFilter & filterExpressionList;
						}
					}
					else if (option.MapItemsFilter is FilterExpressionList)
					{
						option.MapItemsFilter = (FilterExpressionList)option.MapItemsFilter & FOLDERS_FILTER;
					}
					else if (!(option.MapItemsFilter is FilterExpression))
					{
						option.MapItemsFilter = new FilterExpressionList(ExpressionLogic.And) { FOLDERS_FILTER };
					}
					else
					{
						option.MapItemsFilter = (FilterExpression)option.MapItemsFilter & FOLDERS_FILTER;
					}
					break;
				}
			}
			return true;
		}

		private void UpdateEnabled()
		{
			DataRow selectedRow = GetSelectedRow();
			if (selectedRow == null)
			{
				w_bClear.Enabled = false;
				w_bEdit.Enabled = false;
			}
			else if (selectedRow["ItemType"] != null && !selectedRow["ItemType"].Equals("None"))
			{
				w_bClear.Enabled = true;
				w_bEdit.Enabled = true;
			}
			else
			{
				w_bClear.Enabled = false;
				w_bEdit.Enabled = false;
			}
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void w_bClear_Click(object sender, EventArgs e)
		{
			DataRow selectedRow = GetSelectedRow();
			if (selectedRow != null)
			{
				_filterMap[selectedRow] = null;
				selectedRow["ApplicationCondition"] = "";
			}
		}

		private void w_bEdit_Click(object sender, EventArgs e)
		{
			DataRow selectedRow = GetSelectedRow();
			if (selectedRow == null)
			{
				return;
			}
			IFilterExpression filterExpression = _filterMap[selectedRow];
			FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog
			{
				Title = "Item Application Conditions"
			};
			Type[] collection = new Type[1] { typeof(SPListItem) };
			filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: true);
			if (filterExpression != null)
			{
				filterExpressionEditorDialog.FilterExpression = filterExpression;
			}
			filterExpressionEditorDialog.ShowDialog();
			if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
			{
				IFilterExpression filterExpression2 = filterExpressionEditorDialog.FilterExpression;
				_filterMap[selectedRow] = filterExpression2;
				if (filterExpression2 == null)
				{
					selectedRow["ApplicationCondition"] = null;
				}
				else
				{
					selectedRow["ApplicationCondition"] = filterExpression2.GetLogicString();
				}
			}
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			if (SaveUI())
			{
				base.DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
