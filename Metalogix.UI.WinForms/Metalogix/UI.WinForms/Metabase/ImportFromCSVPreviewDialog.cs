using Metalogix.Metabase.DataTypes;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	public class ImportFromCSVPreviewDialog : Form
	{
		private ListViewColumnSorter m_listViewColumnSorter;

		private bool m_bNewColumnsCreatable;

		private List<string> m_hiddenColumns = new List<string>();

		private List<KeyValuePair<PropertyDescriptor, Type>> m_mappedColumns;

		private List<KeyValuePair<string, Type>> m_unmappedColumns;

		private bool _isExcelFile;

		private IContainer components;

		private Button w_bCancel;

		private Button w_bOk;

		private OpenFileDialog w_openFileDialog;

		private Label label3;

		private ListView w_listViewColumnsPreview;

		private ColumnHeader w_chColumnName;

		private ColumnHeader w_chColumnCalculatedType;

		private ColumnHeader w_chColumnStatus;

		private ColumnHeader w_chColumnWorkspaceType;

		public List<string> HiddenColumns
		{
			get
			{
				return this.m_hiddenColumns;
			}
		}

		public bool IsExcelFile
		{
			get
			{
				return this._isExcelFile;
			}
			set
			{
				this._isExcelFile = value;
				if (value)
				{
					this.Text = "Import from Excel Preview";
					this.w_chColumnCalculatedType.Text = "Excel Column Type";
					return;
				}
				this.Text = "Import from CSV Preview";
				this.w_chColumnCalculatedType.Text = "CSV Column Type";
			}
		}

		public List<KeyValuePair<PropertyDescriptor, Type>> MappedColumns
		{
			get
			{
				return this.m_mappedColumns;
			}
			set
			{
				this.m_mappedColumns = value;
			}
		}

		public bool NewColumnsCreatable
		{
			get
			{
				return this.m_bNewColumnsCreatable;
			}
			set
			{
				this.m_bNewColumnsCreatable = value;
			}
		}

		public List<KeyValuePair<string, Type>> UnmappedColumns
		{
			get
			{
				return this.m_unmappedColumns;
			}
			set
			{
				this.m_unmappedColumns = value;
			}
		}

		public ImportFromCSVPreviewDialog(bool newColumnsCreatable)
		{
			this.InitializeComponent();
			this.NewColumnsCreatable = newColumnsCreatable;
			this.m_listViewColumnSorter = new ListViewColumnSorter();
			this.w_listViewColumnsPreview.ListViewItemSorter = this.m_listViewColumnSorter;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private string GetStatusMessage(Type csvType, Type workspaceType, bool isReadOnly = false)
		{
			string empty = string.Empty;
			if (csvType == null)
			{
				empty = string.Empty;
			}
			else if (workspaceType == null)
			{
				empty = (this.NewColumnsCreatable ? "New Column - Will be created" : "New Column - Will not be imported");
			}
			else if (isReadOnly)
			{
				empty = "Read Only - Will not be imported";
			}
			else if (csvType == workspaceType || csvType == typeof(int) && workspaceType == typeof(decimal) || csvType == typeof(string) && typeof(ISmartDataType).IsAssignableFrom(workspaceType) || csvType != typeof(TextMoniker) && workspaceType == typeof(string) || workspaceType == typeof(TextMoniker))
			{
				empty = "OK";
			}
			if (string.IsNullOrEmpty(empty))
			{
				empty = "Column Type Mismatch";
			}
			return empty;
		}

		private void ImportFromCSVPreviewDialog_Shown(object sender, EventArgs e)
		{
			if (this.MappedColumns != null)
			{
				foreach (KeyValuePair<PropertyDescriptor, Type> mappedColumn in this.MappedColumns)
				{
					PropertyDescriptor key = mappedColumn.Key;
					if (this.HiddenColumns.Contains(key.Name))
					{
						continue;
					}
					ListViewItem listViewItem = new ListViewItem(key.Name);
					Type propertyType = key.PropertyType;
					Type value = mappedColumn.Value;
					if (value == null)
					{
						listViewItem.SubItems.Add(string.Empty);
					}
					else
					{
						listViewItem.SubItems.Add(value.ToString());
					}
					if (propertyType == null)
					{
						listViewItem.SubItems.Add(string.Empty);
					}
					else
					{
						listViewItem.SubItems.Add(propertyType.ToString());
					}
					listViewItem.SubItems.Add(this.GetStatusMessage(value, propertyType, (key.Name == "SourceURL" ? false : key.IsReadOnly)));
					this.w_listViewColumnsPreview.Items.Add(listViewItem);
				}
			}
			if (this.UnmappedColumns != null)
			{
				foreach (KeyValuePair<string, Type> mUnmappedColumn in this.m_unmappedColumns)
				{
					string str = mUnmappedColumn.Key;
					if (this.HiddenColumns.Contains(str))
					{
						continue;
					}
					ListViewItem listViewItem1 = new ListViewItem(str);
					Type type = mUnmappedColumn.Value;
					if (type == null)
					{
						listViewItem1.SubItems.Add(string.Empty);
					}
					else
					{
						listViewItem1.SubItems.Add(mUnmappedColumn.Value.ToString());
					}
					listViewItem1.SubItems.Add("N/A");
					listViewItem1.SubItems.Add(this.GetStatusMessage(type, null, false));
					this.w_listViewColumnsPreview.Items.Add(listViewItem1);
				}
			}
		}

		private void InitializeComponent()
		{
			this.w_bCancel = new Button();
			this.w_bOk = new Button();
			this.w_openFileDialog = new OpenFileDialog();
			this.label3 = new Label();
			this.w_listViewColumnsPreview = new ListView();
			this.w_chColumnName = new ColumnHeader("(none)");
			this.w_chColumnCalculatedType = new ColumnHeader();
			this.w_chColumnWorkspaceType = new ColumnHeader();
			this.w_chColumnStatus = new ColumnHeader();
			base.SuspendLayout();
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Location = new Point(682, 326);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 3;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_bOk.Location = new Point(591, 326);
			this.w_bOk.Name = "w_bOk";
			this.w_bOk.Size = new System.Drawing.Size(85, 23);
			this.w_bOk.TabIndex = 2;
			this.w_bOk.Text = "Import";
			this.w_bOk.UseVisualStyleBackColor = true;
			this.w_openFileDialog.DefaultExt = "csv";
			this.w_openFileDialog.Filter = "CSV Files|*.csv|All files|*.*";
			this.label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(391, 331);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(190, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Would you like to proceed with import?";
			this.w_listViewColumnsPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			ListView.ColumnHeaderCollection columns = this.w_listViewColumnsPreview.Columns;
			ColumnHeader[] wChColumnName = new ColumnHeader[] { this.w_chColumnName, this.w_chColumnCalculatedType, this.w_chColumnWorkspaceType, this.w_chColumnStatus };
			columns.AddRange(wChColumnName);
			this.w_listViewColumnsPreview.FullRowSelect = true;
			this.w_listViewColumnsPreview.Location = new Point(12, 12);
			this.w_listViewColumnsPreview.Name = "w_listViewColumnsPreview";
			this.w_listViewColumnsPreview.Size = new System.Drawing.Size(745, 308);
			this.w_listViewColumnsPreview.TabIndex = 0;
			this.w_listViewColumnsPreview.UseCompatibleStateImageBehavior = false;
			this.w_listViewColumnsPreview.View = View.Details;
			this.w_listViewColumnsPreview.ColumnClick += new ColumnClickEventHandler(this.w_listViewColumnsPreview_ColumnClick);
			this.w_chColumnName.Text = "Name";
			this.w_chColumnName.Width = 183;
			this.w_chColumnCalculatedType.Text = "CSV Column Type";
			this.w_chColumnCalculatedType.Width = 185;
			this.w_chColumnWorkspaceType.Text = "Workspace Column Type";
			this.w_chColumnWorkspaceType.Width = 177;
			this.w_chColumnStatus.Text = "Status";
			this.w_chColumnStatus.Width = 181;
			base.AcceptButton = this.w_bOk;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(769, 361);
			base.Controls.Add(this.w_listViewColumnsPreview);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.w_bOk);
			base.Controls.Add(this.w_bCancel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 320);
			base.Name = "ImportFromCSVPreviewDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Import From CSV Preview";
			base.Shown += new EventHandler(this.ImportFromCSVPreviewDialog_Shown);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void w_listViewColumnsPreview_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column != this.m_listViewColumnSorter.SortColumn)
			{
				this.m_listViewColumnSorter.SortColumn = e.Column;
				this.m_listViewColumnSorter.Order = SortOrder.Ascending;
			}
			else if (this.m_listViewColumnSorter.Order != SortOrder.Ascending)
			{
				this.m_listViewColumnSorter.Order = SortOrder.Ascending;
			}
			else
			{
				this.m_listViewColumnSorter.Order = SortOrder.Descending;
			}
			this.w_listViewColumnsPreview.Sort();
		}
	}
}