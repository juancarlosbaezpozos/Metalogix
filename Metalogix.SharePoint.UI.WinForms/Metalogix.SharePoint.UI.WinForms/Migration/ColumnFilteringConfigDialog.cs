using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Data.Filters;
using Metalogix.Explorer;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class ColumnFilteringConfigDialog : XtraForm
	{
		private bool _showInternalNames;

		private IContainer components;

		private LeftRightSelectionControl w_lrscColumnPicker;

		private SimpleButton w_bCancel;

		private SimpleButton w_bOkay;

		private SimpleButton w_bToggleDispalyedName;

		public IFilterExpression FilterExpression
		{
			get
			{
				SPField[] selectedItems = w_lrscColumnPicker.GetSelectedItems<SPField>();
				if (selectedItems.Length == 0)
				{
					return null;
				}
				if (selectedItems.Length == 1)
				{
					return CreateFilterExpression(selectedItems[0], isRootFilter: true);
				}
				FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And);
				SPField[] array = selectedItems;
				for (int i = 0; i < array.Length; i++)
				{
					filterExpressionList.Add(CreateFilterExpression(array[i], isRootFilter: false));
				}
				return filterExpressionList;
			}
			set
			{
				InitializeFilter(value);
			}
		}

		public ColumnFilteringConfigDialog(SPList sourceList, bool isBasicMode = false)
		{
			InitializeComponent();
			InitializeUI(sourceList);
			if (isBasicMode)
			{
				ApplyBasicModeSkin();
			}
		}

		public ColumnFilteringConfigDialog(SPWeb sourceWeb)
		{
			InitializeComponent();
			InitializeUI(sourceWeb);
		}

		public ColumnFilteringConfigDialog(NodeCollection sourceNodes, bool isBasicMode = false)
		{
			InitializeComponent();
			InitializeUI(sourceNodes);
			if (isBasicMode)
			{
				ApplyBasicModeSkin();
			}
		}

		private void ApplyBasicModeSkin()
		{
			w_bOkay.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			w_bOkay.LookAndFeel.UseDefaultLookAndFeel = false;
			w_bCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			w_bCancel.LookAndFeel.UseDefaultLookAndFeel = false;
			w_bToggleDispalyedName.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			w_bToggleDispalyedName.LookAndFeel.UseDefaultLookAndFeel = false;
			w_bToggleDispalyedName.Width += 16;
		}

		private string ConvertFieldToString(object obj, bool useInternalName)
		{
			if (!(obj is SPField sPField))
			{
				return "";
			}
			if (!useInternalName && !string.IsNullOrEmpty(sPField.DisplayName))
			{
				return sPField.DisplayName;
			}
			return sPField.Name;
		}

		private FilterExpression CreateFilterExpression(SPField field, bool isRootFilter)
		{
			return new FilterExpression(FilterOperand.NotEquals, typeof(SPField), "Name", field.Name, bIsCaseSensitive: true, isRootFilter);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private string GetFieldDisplayName(object obj)
		{
			return ConvertFieldToString(obj, _showInternalNames);
		}

		private string GetFieldSortByString(object obj)
		{
			return ConvertFieldToString(obj, useInternalName: false);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.ColumnFilteringConfigDialog));
			this.w_lrscColumnPicker = new Metalogix.UI.WinForms.Components.LeftRightSelectionControl();
			this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
			this.w_bToggleDispalyedName = new DevExpress.XtraEditors.SimpleButton();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lrscColumnPicker, "w_lrscColumnPicker");
			this.w_lrscColumnPicker.DeselectedItemsLabel = "Columns to Copy";
			this.w_lrscColumnPicker.Name = "w_lrscColumnPicker";
			this.w_lrscColumnPicker.SelectedItemsLabel = "Columns to Exclude";
			this.w_lrscColumnPicker.ItemSorting = Metalogix.UI.WinForms.Components.SortingType.Ascending;
			resources.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Click += new System.EventHandler(w_bCancel_Click);
			resources.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Click += new System.EventHandler(w_bOkay_Click);
			resources.ApplyResources(this.w_bToggleDispalyedName, "w_bToggleDispalyedName");
			this.w_bToggleDispalyedName.Name = "w_bToggleDispalyedName";
			this.w_bToggleDispalyedName.Click += new System.EventHandler(w_bToggleDispalyedName_Click);
			base.AcceptButton = this.w_bOkay;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_bToggleDispalyedName);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_lrscColumnPicker);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ColumnFilteringConfigDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
		}

		private void InitializeFilter(IFilterExpression filter)
		{
			w_lrscColumnPicker.ClearSelection();
			if (filter != null)
			{
				SPField[] selectedItems = (from f in w_lrscColumnPicker.GetAvailableItems<SPField>()
					where !filter.Evaluate(f, new CompareDatesInUtc())
					select f).ToArray();
				w_lrscColumnPicker.SetSelectedItems(selectedItems);
			}
		}

		private void InitializeUI(SPList sourceList)
		{
			w_lrscColumnPicker.ItemNamingMethod = GetFieldDisplayName;
			w_lrscColumnPicker.ItemSortValueMethod = GetFieldSortByString;
			w_lrscColumnPicker.SetAvailableItems(sourceList.FieldCollection);
		}

		private void InitializeUI(SPWeb sourceWeb)
		{
			w_lrscColumnPicker.ItemNamingMethod = GetFieldDisplayName;
			w_lrscColumnPicker.ItemSortValueMethod = GetFieldSortByString;
			w_lrscColumnPicker.SetAvailableItems(sourceWeb.SiteColumns);
		}

		private void InitializeUI(NodeCollection sourceNodes)
		{
			w_lrscColumnPicker.ItemNamingMethod = GetFieldDisplayName;
			w_lrscColumnPicker.ItemSortValueMethod = GetFieldSortByString;
			List<SPField> list = new List<SPField>();
			List<string> list2 = new List<string>();
			foreach (SPNode sourceNode in sourceNodes)
			{
				foreach (SPField availableColumn in (sourceNode as SPWeb).AvailableColumns)
				{
					string name = availableColumn.Name;
					if (!list2.Contains(name))
					{
						list.Add(availableColumn);
						list2.Add(name);
					}
				}
			}
			w_lrscColumnPicker.SetAvailableItems(list);
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void w_bToggleDispalyedName_Click(object sender, EventArgs e)
		{
			if (_showInternalNames)
			{
				_showInternalNames = false;
				w_bToggleDispalyedName.Text = Resources.Show_Internal_Names;
			}
			else
			{
				_showInternalNames = true;
				w_bToggleDispalyedName.Text = Resources.Show_Display_Names;
			}
			w_lrscColumnPicker.RefreshAllItems();
		}
	}
}
