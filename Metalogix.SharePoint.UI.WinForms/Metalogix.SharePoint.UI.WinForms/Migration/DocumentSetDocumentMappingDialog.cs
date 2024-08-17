using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Data.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class DocumentSetDocumentMappingDialog : Form
	{
		private int m_iContentTypeColMinWidth;

		private int m_iApplicationConditionColMinWidth;

		private int m_iDocSetColMinWidth;

		private IEnumerable<SPContentType> m_documentSets;

		private DocumentSetApplicationOptionsCollection m_Options;

		private IContainer components;

		private Label w_lbDescription;

		private Button w_bEdit;

		private Button w_bClear;

		private Button w_bOkay;

		private Button w_bCancel;

		private DataGridView w_dgvOptions;

		private Button w_bAdd;

		private DataGridViewTextBoxColumn w_chDocSetName;

		private DataGridViewComboBoxColumn w_chContentType;

		private DataGridViewTextBoxColumn w_chCondition;

		private Button w_bRemove;

		public DocumentSetApplicationOptionsCollection Options
		{
			get
			{
				return this.m_Options;
			}
			set
			{
				this.m_Options = value;
				this.LoadUI();
			}
		}

		public DocumentSetDocumentMappingDialog(IEnumerable<SPContentType> targetDocumentSets)
		{
			this.InitializeComponent();
			this.m_documentSets = targetDocumentSets;
			this.m_iApplicationConditionColMinWidth = this.w_chCondition.Width;
			this.m_iContentTypeColMinWidth = this.w_chContentType.Width;
			this.m_iDocSetColMinWidth = this.w_chDocSetName.Width;
			int num = 0;
			foreach (SPContentType mDocumentSet in this.m_documentSets)
			{
				((DataGridViewComboBoxColumn)this.w_dgvOptions.Columns[1]).Items.Add(mDocumentSet.Name);
				num++;
			}
			if (num == 0)
			{
				FlatXtraMessageBox.Show("There are no Document Set content types on the target site.  You must create atleast 1 Document Set content type to continue.", "No Document Set Content Types", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EditConditionData(DataGridViewRow rowToEdit)
		{
			IFilterExpression tag = (IFilterExpression)rowToEdit.Tag;
			FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog(false)
			{
				Title = "Document Filter Conditions"
			};
			Type[] typeArray = new Type[] { typeof(SPListItem) };
			filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type>(typeArray), true);
			filterExpressionEditorDialog.LabelText = "Specify filters to determine which documents will be added to the document set.";
			if (tag != null)
			{
				filterExpressionEditorDialog.FilterExpression = tag;
			}
			filterExpressionEditorDialog.ShowDialog();
			if (filterExpressionEditorDialog.DialogResult == System.Windows.Forms.DialogResult.OK)
			{
				rowToEdit.Tag = filterExpressionEditorDialog.FilterExpression;
				if (rowToEdit.Tag == null)
				{
					rowToEdit.Cells[2].Value = null;
					return;
				}
				rowToEdit.Cells[2].Value = filterExpressionEditorDialog.FilterExpression.GetLogicString();
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DocumentSetDocumentMappingDialog));
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			this.w_lbDescription = new Label();
			this.w_bEdit = new Button();
			this.w_bClear = new Button();
			this.w_bOkay = new Button();
			this.w_bCancel = new Button();
			this.w_dgvOptions = new DataGridView();
			this.w_chDocSetName = new DataGridViewTextBoxColumn();
			this.w_chContentType = new DataGridViewComboBoxColumn();
			this.w_chCondition = new DataGridViewTextBoxColumn();
			this.w_bAdd = new Button();
			this.w_bRemove = new Button();
			((ISupportInitialize)this.w_dgvOptions).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_lbDescription, "w_lbDescription");
			this.w_lbDescription.Name = "w_lbDescription";
			componentResourceManager.ApplyResources(this.w_bEdit, "w_bEdit");
			this.w_bEdit.Name = "w_bEdit";
			this.w_bEdit.UseVisualStyleBackColor = true;
			this.w_bEdit.Click += new EventHandler(this.w_bEdit_Click);
			componentResourceManager.ApplyResources(this.w_bClear, "w_bClear");
			this.w_bClear.Name = "w_bClear";
			this.w_bClear.UseVisualStyleBackColor = true;
			this.w_bClear.Click += new EventHandler(this.w_bClear_Click);
			componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bOkay.Click += new EventHandler(this.w_bOkay_Click);
			componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bCancel.Click += new EventHandler(this.w_bCancel_Click);
			this.w_dgvOptions.AllowUserToAddRows = false;
			this.w_dgvOptions.AllowUserToResizeRows = false;
			componentResourceManager.ApplyResources(this.w_dgvOptions, "w_dgvOptions");
			this.w_dgvOptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.w_dgvOptions.BackgroundColor = Color.White;
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Control;
			dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.w_dgvOptions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
			this.w_dgvOptions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			DataGridViewColumnCollection columns = this.w_dgvOptions.Columns;
			DataGridViewColumn[] wChDocSetName = new DataGridViewColumn[] { this.w_chDocSetName, this.w_chContentType, this.w_chCondition };
			columns.AddRange(wChDocSetName);
			this.w_dgvOptions.EditMode = DataGridViewEditMode.EditOnEnter;
			this.w_dgvOptions.Name = "w_dgvOptions";
			this.w_dgvOptions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.w_dgvOptions.ShowEditingIcon = false;
			this.w_dgvOptions.SelectionChanged += new EventHandler(this.w_dgvMappings_SelectionChanged);
			this.w_chDocSetName.FillWeight = 40f;
			componentResourceManager.ApplyResources(this.w_chDocSetName, "w_chDocSetName");
			this.w_chDocSetName.Name = "w_chDocSetName";
			this.w_chContentType.FillWeight = 44.67574f;
			componentResourceManager.ApplyResources(this.w_chContentType, "w_chContentType");
			this.w_chContentType.Name = "w_chContentType";
			this.w_chContentType.Resizable = DataGridViewTriState.True;
			this.w_chContentType.SortMode = DataGridViewColumnSortMode.Automatic;
			this.w_chCondition.FillWeight = 76.14214f;
			componentResourceManager.ApplyResources(this.w_chCondition, "w_chCondition");
			this.w_chCondition.Name = "w_chCondition";
			this.w_chCondition.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_bAdd, "w_bAdd");
			this.w_bAdd.Name = "w_bAdd";
			this.w_bAdd.UseVisualStyleBackColor = true;
			this.w_bAdd.Click += new EventHandler(this.w_bAdd_Click);
			componentResourceManager.ApplyResources(this.w_bRemove, "w_bRemove");
			this.w_bRemove.Name = "w_bRemove";
			this.w_bRemove.UseVisualStyleBackColor = true;
			this.w_bRemove.Click += new EventHandler(this.w_bRemove_Click);
			base.AcceptButton = this.w_bOkay;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_bRemove);
			base.Controls.Add(this.w_bAdd);
			base.Controls.Add(this.w_dgvOptions);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bClear);
			base.Controls.Add(this.w_bEdit);
			base.Controls.Add(this.w_lbDescription);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DocumentSetDocumentMappingDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			((ISupportInitialize)this.w_dgvOptions).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			this.w_dgvOptions.Rows.Clear();
			if (this.Options == null)
			{
				return;
			}
			foreach (DocumentSetApplicationOptions datum in this.Options.Data)
			{
				Color gray = Color.Gray;
				Color defaultBackColor = Control.DefaultBackColor;
				string str = (datum.MapItemsFilter == null ? "" : datum.MapItemsFilter.GetLogicString());
				DataGridViewRowCollection rows = this.w_dgvOptions.Rows;
				object[] docSetName = new object[] { datum.DocSetName, datum.ContentTypeName, str };
				int mapItemsFilter = rows.Add(docSetName);
				this.w_dgvOptions.Rows[mapItemsFilter].Tag = datum.MapItemsFilter;
				this.w_dgvOptions.Rows[mapItemsFilter].Cells[1].Style.ForeColor = gray;
				this.w_dgvOptions.Rows[mapItemsFilter].Cells[1].Style.BackColor = defaultBackColor;
			}
			this.UpdateEnabled();
			this.UpdateColumnWidths();
		}

		private bool SaveUI()
		{
			this.Options.Data.Clear();
			if (this.w_dgvOptions.Rows.Count == 0)
			{
				return true;
			}
			DocumentSetApplicationOptions documentSetApplicationOption = null;
			foreach (DataGridViewRow row in (IEnumerable)this.w_dgvOptions.Rows)
			{
				documentSetApplicationOption = new DocumentSetApplicationOptions(row.Cells[1].Value.ToString())
				{
					DocSetName = row.Cells[0].Value.ToString(),
					MapItemsFilter = (IFilterExpression)row.Tag
				};
				this.Options.Data.Add(documentSetApplicationOption);
			}
			return true;
		}

		private void UpdateColumnWidths()
		{
			this.w_chDocSetName.Width = -1;
			if (this.w_chDocSetName.Width < this.m_iDocSetColMinWidth)
			{
				this.w_chDocSetName.Width = this.m_iDocSetColMinWidth;
			}
			this.w_chContentType.Width = -1;
			if (this.w_chContentType.Width < this.m_iContentTypeColMinWidth)
			{
				this.w_chContentType.Width = this.m_iContentTypeColMinWidth;
			}
			this.w_chCondition.Width = -1;
			if (this.w_chCondition.Width < this.m_iApplicationConditionColMinWidth)
			{
				this.w_chCondition.Width = this.m_iApplicationConditionColMinWidth;
			}
		}

		private void UpdateEnabled()
		{
			this.w_bClear.Enabled = false;
			this.w_bEdit.Enabled = false;
			this.w_bRemove.Enabled = false;
			if (this.w_dgvOptions.SelectedRows.Count > 0)
			{
				this.w_bRemove.Enabled = true;
				if (this.w_dgvOptions.SelectedRows.Count == 1 && this.w_dgvOptions.SelectedRows[0] != null)
				{
					this.w_bClear.Enabled = true;
					this.w_bEdit.Enabled = true;
				}
			}
		}

		private void w_bAdd_Click(object sender, EventArgs e)
		{
			List<string> strs = new List<string>();
			foreach (DataGridViewRow row in (IEnumerable)this.w_dgvOptions.Rows)
			{
				strs.Add(row.Cells[0].Value.ToString());
			}
			InputBox inputBox = new InputBox("Enter Name", "Enter a document set name", strs);
			if (inputBox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				int num = this.w_dgvOptions.Rows.Add();
				DataGridViewRow item = this.w_dgvOptions.Rows[num];
				item.Cells[0].Value = inputBox.InputValue;
				((DataGridViewComboBoxCell)item.Cells[1]).Value = ((DataGridViewComboBoxColumn)this.w_dgvOptions.Columns[1]).Items[0].ToString();
				this.w_dgvOptions.SelectionChanged -= new EventHandler(this.w_dgvMappings_SelectionChanged);
				while (this.w_dgvOptions.SelectedRows.Count > 0)
				{
					this.w_dgvOptions.SelectedRows[0].Selected = false;
				}
				this.w_dgvOptions.SelectionChanged += new EventHandler(this.w_dgvMappings_SelectionChanged);
				item.Selected = true;
				this.EditConditionData(item);
				this.w_dgvOptions.CurrentCell = item.Cells[1];
				this.w_dgvOptions.Select();
			}
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void w_bClear_Click(object sender, EventArgs e)
		{
			if (this.w_dgvOptions.SelectedRows.Count == 0)
			{
				return;
			}
			DataGridViewRow item = this.w_dgvOptions.SelectedRows[0];
			item.Tag = null;
			item.Cells[2].Value = "";
			this.UpdateColumnWidths();
		}

		private void w_bEdit_Click(object sender, EventArgs e)
		{
			if (this.w_dgvOptions.SelectedRows.Count == 0)
			{
				return;
			}
			this.EditConditionData(this.w_dgvOptions.SelectedRows[0]);
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			if (this.SaveUI())
			{
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				base.Close();
			}
		}

		private void w_bRemove_Click(object sender, EventArgs e)
		{
			if (this.w_dgvOptions.SelectedRows.Count > 0)
			{
				foreach (DataGridViewRow selectedRow in this.w_dgvOptions.SelectedRows)
				{
					this.w_dgvOptions.Rows.Remove(selectedRow);
				}
			}
		}

		private void w_dgvMappings_SelectionChanged(object sender, EventArgs e)
		{
			this.UpdateEnabled();
		}
	}
}