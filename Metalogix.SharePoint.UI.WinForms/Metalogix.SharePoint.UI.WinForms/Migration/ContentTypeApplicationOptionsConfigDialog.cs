using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Mapping;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class ContentTypeApplicationOptionsConfigDialog : XtraForm
	{
		private const int CONTROL_SEPARATED_DIST = 22;

		private ColumnMappings m_columnMappings;

		private IEnumerable<SPContentType> m_contentTypes;

		private SPList m_sourceList;

		private bool m_bNWSTarget = true;

		private ContentTypeApplicationOptionsCollection m_options;

		private bool m_bHideColumnMappingOption;

		private ColumnMappingDialog m_columnMappingDlg;

		private IContainer components;

		private LeftRightSelectionControl w_lrscContentTypeSelector;

		private CheckEdit w_cbMapColumns;

		private SimpleButton w_bMapColumns;

		private CheckEdit w_cbMapItems;

		private SimpleButton w_bMapItems;

		private CheckEdit w_cbChangeDefaultContentType;

		private ComboBoxEdit w_cbNewDefaultContentType;

		private CheckEdit w_cbAddUnmappedToDefault;

		private SimpleButton w_bCancel;

		private SimpleButton w_bOkay;

		private CheckEdit w_cbRemoveAllOther;

		public bool HideColumnMappingOption
		{
			get
			{
				return m_bHideColumnMappingOption;
			}
			set
			{
				m_bHideColumnMappingOption = value;
				UpdateUI();
			}
		}

		public ContentTypeApplicationOptionsCollection Options
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

		public ContentTypeApplicationOptionsConfigDialog(IEnumerable<SPContentType> targetContentTypes)
		{
			InitializeComponent();
			m_sourceList = null;
			m_contentTypes = targetContentTypes;
			IEnumerator<SPContentType> enumerator = m_contentTypes.GetEnumerator();
			if (enumerator.MoveNext())
			{
				m_bNWSTarget = !enumerator.Current.ParentCollection.ParentWeb.CanWriteCreatedModifiedMetaInfo;
			}
			InitializeSelectionControl();
			UpdateUI();
		}

		public ContentTypeApplicationOptionsConfigDialog(SPList sourceList, IEnumerable<SPContentType> targetContentTypes)
		{
			InitializeComponent();
			m_sourceList = sourceList;
			m_contentTypes = targetContentTypes;
			IEnumerator<SPContentType> enumerator = m_contentTypes.GetEnumerator();
			if (enumerator.MoveNext())
			{
				m_bNWSTarget = !enumerator.Current.ParentCollection.ParentWeb.CanWriteCreatedModifiedMetaInfo;
			}
			InitializeSelectionControl();
			UpdateUI();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private string GetApplicationOptionsName(object obj)
		{
			if (!(obj is ContentTypeApplicationOptions contentTypeApplicationOptions))
			{
				return "";
			}
			return contentTypeApplicationOptions.ContentTypeName;
		}

		private IEnumerable<SPContentType> GetContentTypes(IEnumerable<ContentTypeApplicationOptions> items)
		{
			return from o in items
				from c in m_contentTypes
				where o.ContentTypeName == c.Name
				select c;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.ContentTypeApplicationOptionsConfigDialog));
			this.w_lrscContentTypeSelector = new Metalogix.UI.WinForms.Components.LeftRightSelectionControl();
			this.w_cbMapColumns = new DevExpress.XtraEditors.CheckEdit();
			this.w_bMapColumns = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbMapItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_bMapItems = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbChangeDefaultContentType = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbNewDefaultContentType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_cbAddUnmappedToDefault = new DevExpress.XtraEditors.CheckEdit();
			this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbRemoveAllOther = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapColumns.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbChangeDefaultContentType.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbNewDefaultContentType.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbAddUnmappedToDefault.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbRemoveAllOther.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lrscContentTypeSelector, "w_lrscContentTypeSelector");
			this.w_lrscContentTypeSelector.DeselectedItemsLabel = "Content types on target site";
			this.w_lrscContentTypeSelector.ItemSorting = Metalogix.UI.WinForms.Components.SortingType.Ascending;
			this.w_lrscContentTypeSelector.Name = "w_lrscContentTypeSelector";
			this.w_lrscContentTypeSelector.SelectedItemsLabel = "Content types to apply";
			this.w_lrscContentTypeSelector.SelectionChanged += new Metalogix.UI.WinForms.Components.LeftRightSelectionControl.SelectionChangedDelegate(w_lrscContentTypeSelector_SelectionChanged);
			resources.ApplyResources(this.w_cbMapColumns, "w_cbMapColumns");
			this.w_cbMapColumns.Name = "w_cbMapColumns";
			this.w_cbMapColumns.Properties.AutoWidth = true;
			this.w_cbMapColumns.Properties.Caption = resources.GetString("w_cbMapColumns.Properties.Caption");
			this.w_cbMapColumns.CheckedChanged += new System.EventHandler(w_cbMapColumns_CheckedChanged);
			resources.ApplyResources(this.w_bMapColumns, "w_bMapColumns");
			this.w_bMapColumns.Name = "w_bMapColumns";
			this.w_bMapColumns.Click += new System.EventHandler(w_bMapColumns_Click);
			resources.ApplyResources(this.w_cbMapItems, "w_cbMapItems");
			this.w_cbMapItems.Name = "w_cbMapItems";
			this.w_cbMapItems.Properties.AutoWidth = true;
			this.w_cbMapItems.Properties.Caption = resources.GetString("w_cbMapItems.Properties.Caption");
			this.w_cbMapItems.CheckedChanged += new System.EventHandler(w_cbMapItems_CheckedChanged);
			resources.ApplyResources(this.w_bMapItems, "w_bMapItems");
			this.w_bMapItems.Name = "w_bMapItems";
			this.w_bMapItems.Click += new System.EventHandler(w_bMapItems_Click);
			resources.ApplyResources(this.w_cbChangeDefaultContentType, "w_cbChangeDefaultContentType");
			this.w_cbChangeDefaultContentType.Name = "w_cbChangeDefaultContentType";
			this.w_cbChangeDefaultContentType.Properties.AutoWidth = true;
			this.w_cbChangeDefaultContentType.Properties.Caption = resources.GetString("w_cbChangeDefaultContentType.Properties.Caption");
			this.w_cbChangeDefaultContentType.CheckedChanged += new System.EventHandler(w_cbChangeDefaultContentType_CheckedChanged);
			resources.ApplyResources(this.w_cbNewDefaultContentType, "w_cbNewDefaultContentType");
			this.w_cbNewDefaultContentType.Name = "w_cbNewDefaultContentType";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_cbNewDefaultContentType.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons.AddRange(buttons2);
			this.w_cbNewDefaultContentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.w_cbAddUnmappedToDefault, "w_cbAddUnmappedToDefault");
			this.w_cbAddUnmappedToDefault.Name = "w_cbAddUnmappedToDefault";
			this.w_cbAddUnmappedToDefault.Properties.AutoWidth = true;
			this.w_cbAddUnmappedToDefault.Properties.Caption = resources.GetString("w_cbAddUnmappedToDefault.Properties.Caption");
			this.w_cbAddUnmappedToDefault.CheckedChanged += new System.EventHandler(w_cbAddUnmappedToDefault_CheckedChanged);
			resources.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Click += new System.EventHandler(w_bCancel_Click);
			resources.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Click += new System.EventHandler(w_bOkay_Click);
			resources.ApplyResources(this.w_cbRemoveAllOther, "w_cbRemoveAllOther");
			this.w_cbRemoveAllOther.Name = "w_cbRemoveAllOther";
			this.w_cbRemoveAllOther.Properties.AutoWidth = true;
			this.w_cbRemoveAllOther.Properties.Caption = resources.GetString("w_cbRemoveAllOther.Properties.Caption");
			base.AcceptButton = this.w_bOkay;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_cbRemoveAllOther);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_cbAddUnmappedToDefault);
			base.Controls.Add(this.w_cbNewDefaultContentType);
			base.Controls.Add(this.w_cbChangeDefaultContentType);
			base.Controls.Add(this.w_bMapItems);
			base.Controls.Add(this.w_cbMapItems);
			base.Controls.Add(this.w_bMapColumns);
			base.Controls.Add(this.w_cbMapColumns);
			base.Controls.Add(this.w_lrscContentTypeSelector);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ContentTypeApplicationOptionsConfigDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(On_Form_Closed);
			((System.ComponentModel.ISupportInitialize)this.w_cbMapColumns.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbChangeDefaultContentType.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbNewDefaultContentType.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbAddUnmappedToDefault.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbRemoveAllOther.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void InitializeSelectionControl()
		{
			w_lrscContentTypeSelector.ItemNamingMethod = GetApplicationOptionsName;
		}

		private void LoadUI()
		{
			w_lrscContentTypeSelector.ClearSelection();
			if (Options == null)
			{
				UpdateDropDownContents();
				UpdateUI();
				return;
			}
			List<string> strs = new List<string>(m_contentTypes.Select((SPContentType c) => c.Name));
			ContentTypeApplicationOptions[] array = (from o in Options.Data
				where strs.Contains(o.ContentTypeName)
				select o.Clone()).ToArray();
			List<string> strs1 = new List<string>();
			bool @checked = false;
			string value = null;
			ContentTypeApplicationOptions[] array2 = array;
			foreach (ContentTypeApplicationOptions contentTypeApplicationOptions in array2)
			{
				if (contentTypeApplicationOptions.MapItemsFilter != null)
				{
					@checked = true;
				}
				if (contentTypeApplicationOptions.MakeDefault)
				{
					value = contentTypeApplicationOptions.ContentTypeName;
				}
				strs1.Add(contentTypeApplicationOptions.ContentTypeName);
			}
			IEnumerable<ContentTypeApplicationOptions> second = from c in m_contentTypes
				where !strs1.Contains(c.Name)
				select new ContentTypeApplicationOptions(c.Name);
			w_lrscContentTypeSelector.SetAvailableItems(array.Concat(second));
			w_lrscContentTypeSelector.SetSelectedItems(array);
			UpdateDropDownContents();
			ColumnMappings columnMappings = ((Options.ColumnMappings != null) ? ((ColumnMappings)Options.ColumnMappings.Clone()) : null);
			m_columnMappings = columnMappings;
			w_cbMapColumns.Checked = Options.ColumnMappings != null && Options.ColumnMappings.Count > 0;
			w_cbMapItems.Checked = @checked;
			if (string.IsNullOrEmpty(value))
			{
				w_cbChangeDefaultContentType.Checked = false;
			}
			else
			{
				w_cbChangeDefaultContentType.Checked = true;
				w_cbNewDefaultContentType.Text = value;
			}
			w_cbAddUnmappedToDefault.Checked = Options.AddUnmappedItemsToDefaultContentType;
			w_cbRemoveAllOther.Checked = Options.RemoveAllOtherContentTypes;
			UpdateUI();
		}

		private void On_Form_Closed(object sender, FormClosedEventArgs e)
		{
			if (m_columnMappingDlg != null)
			{
				m_columnMappingDlg.Dispose();
				m_columnMappingDlg = null;
			}
		}

		private void RemoveInvalidColumnMappings(IEnumerable<ContentTypeApplicationOptions> removedOptions)
		{
			if (m_columnMappings == null || m_columnMappings.Count <= 0)
			{
				return;
			}
			IEnumerable<SPContentType> contentTypes = GetContentTypes(removedOptions);
			IEnumerable<SPContentType> contentTypes2 = GetContentTypes(w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>());
			List<string> list = new List<string>();
			foreach (SPContentType item in contentTypes)
			{
				foreach (SPField definedField in item.GetDefinedFields())
				{
					if (!list.Contains(definedField.Name))
					{
						list.Add(definedField.Name);
					}
				}
			}
			foreach (SPContentType item2 in contentTypes2)
			{
				foreach (SPField definedField2 in item2.GetDefinedFields())
				{
					list.Remove(definedField2.Name);
				}
			}
			List<ListSummaryItem> list2 = new List<ListSummaryItem>();
			foreach (ListSummaryItem columnMapping in m_columnMappings)
			{
				if (list.Contains(columnMapping.Target.Target))
				{
					list2.Add(columnMapping);
				}
			}
			if (list2.Count > 0)
			{
				m_columnMappings.RemoveRange(list2.ToArray());
			}
		}

		private bool SaveUI()
		{
			ContentTypeApplicationOptions[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>();
			Options.Data.Clear();
			ContentTypeApplicationOptions[] array = selectedItems;
			foreach (ContentTypeApplicationOptions contentTypeApplicationOptions in array)
			{
				if (!w_cbMapItems.Checked)
				{
					contentTypeApplicationOptions.MapItemsFilter = null;
				}
				Options.Data.Add(contentTypeApplicationOptions);
			}
			if (!w_cbMapColumns.Checked || m_columnMappings == null || m_columnMappings.Count <= 0)
			{
				Options.ColumnMappings = null;
			}
			else
			{
				Options.ColumnMappings = m_columnMappings;
			}
			foreach (ContentTypeApplicationOptions datum in Options.Data)
			{
				if (!w_cbNewDefaultContentType.Enabled || !(datum.ContentTypeName == w_cbNewDefaultContentType.Text))
				{
					datum.MakeDefault = false;
				}
				else
				{
					datum.MakeDefault = true;
				}
			}
			Options.AddUnmappedItemsToDefaultContentType = w_cbAddUnmappedToDefault.Checked;
			Options.RemoveAllOtherContentTypes = w_cbRemoveAllOther.Checked;
			return true;
		}

		private void UpdateDropDownContents()
		{
			string text = w_cbNewDefaultContentType.EditValue as string;
			List<string> list = new List<string>();
			bool flag = true;
			ContentTypeApplicationOptions[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>();
			foreach (ContentTypeApplicationOptions contentTypeApplicationOptions in selectedItems)
			{
				list.Add(contentTypeApplicationOptions.ContentTypeName);
				if (contentTypeApplicationOptions.ContentTypeName == text)
				{
					flag = false;
				}
			}
			w_cbNewDefaultContentType.Properties.Items.Clear();
			w_cbNewDefaultContentType.Properties.Items.AddRange(list.ToArray());
			if (!flag)
			{
				w_cbNewDefaultContentType.EditValue = text;
			}
			else if (w_cbNewDefaultContentType.Properties.Items.Count == 0)
			{
				w_cbNewDefaultContentType.EditValue = "";
			}
			else
			{
				w_cbNewDefaultContentType.EditValue = w_cbNewDefaultContentType.Properties.Items[0];
			}
		}

		private void UpdateUI()
		{
			ContentTypeApplicationOptions[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>();
			w_cbMapColumns.Enabled = !HideColumnMappingOption && selectedItems.Length != 0;
			w_cbMapColumns.Checked = w_cbMapColumns.Checked && w_cbMapColumns.Enabled;
			w_cbMapColumns.Visible = !HideColumnMappingOption;
			w_bMapColumns.Enabled = w_cbMapColumns.Enabled && w_cbMapColumns.Checked;
			w_bMapColumns.Visible = !HideColumnMappingOption;
			w_cbMapItems.Enabled = selectedItems.Length != 0;
			w_cbMapItems.Checked = w_cbMapItems.Checked && w_cbMapItems.Enabled;
			w_bMapItems.Enabled = w_cbMapItems.Enabled && w_cbMapItems.Checked;
			w_cbRemoveAllOther.Enabled = selectedItems.Length != 0;
			w_cbRemoveAllOther.Checked = w_cbRemoveAllOther.Checked && w_cbRemoveAllOther.Enabled;
			w_cbChangeDefaultContentType.Enabled = selectedItems.Length != 0 && !m_bNWSTarget;
			w_cbChangeDefaultContentType.Checked = w_cbChangeDefaultContentType.Checked && w_cbChangeDefaultContentType.Enabled;
			w_cbNewDefaultContentType.Enabled = w_cbChangeDefaultContentType.Checked && w_cbChangeDefaultContentType.Enabled;
			w_cbAddUnmappedToDefault.Enabled = w_cbChangeDefaultContentType.Checked;
			w_cbAddUnmappedToDefault.Checked = w_cbAddUnmappedToDefault.Checked && w_cbAddUnmappedToDefault.Enabled;
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void w_bMapColumns_Click(object sender, EventArgs e)
		{
			IEnumerable<SPContentType> contentTypes = GetContentTypes(w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>());
			ColumnMappingDialog columnMappingDialog = null;
			if (m_columnMappingDlg == null)
			{
				columnMappingDialog = (m_columnMappingDlg = new ColumnMappingDialog(m_sourceList, contentTypes)
				{
					AllowCreation = false,
					AllowDeletion = false
				});
			}
			else
			{
				columnMappingDialog = m_columnMappingDlg;
				columnMappingDialog.UpdateSiteContentTypes(contentTypes);
			}
			if (m_columnMappings != null)
			{
				columnMappingDialog.SummaryItems = m_columnMappings.ToArray();
			}
			if (columnMappingDialog.ShowDialog() == DialogResult.OK)
			{
				m_columnMappings = new ColumnMappings(columnMappingDialog.SummaryItems);
			}
		}

		private void w_bMapItems_Click(object sender, EventArgs e)
		{
			ContentTypeApplicationOptions[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>();
			if (selectedItems.Length != 0)
			{
				List<ContentTypeApplicationOptions> list = new List<ContentTypeApplicationOptions>();
				ContentTypeApplicationOptions[] array = selectedItems;
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i]);
				}
				ContentTypeToListItemMappingDialog contentTypeToListItemMappingDialog = new ContentTypeToListItemMappingDialog();
				contentTypeToListItemMappingDialog.Options = list;
				contentTypeToListItemMappingDialog.ShowDialog();
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

		private void w_cbAddUnmappedToDefault_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void w_cbChangeDefaultContentType_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void w_cbMapColumns_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void w_cbMapItems_CheckedChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void w_lrscContentTypeSelector_SelectionChanged(object[] changedItems)
		{
			ContentTypeApplicationOptions[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ContentTypeApplicationOptions>();
			ContentTypeApplicationOptions[] array = (from o in changedItems
				where !selectedItems.Contains((ContentTypeApplicationOptions)o)
				select (ContentTypeApplicationOptions)o).ToArray();
			if (array.Length != 0)
			{
				RemoveInvalidColumnMappings(array);
			}
			UpdateDropDownContents();
			UpdateUI();
		}
	}
}
