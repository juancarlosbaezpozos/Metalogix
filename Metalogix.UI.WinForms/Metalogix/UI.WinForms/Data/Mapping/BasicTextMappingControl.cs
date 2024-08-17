using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Core.Properties;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Data.Mapping
{
    public class BasicTextMappingControl : UserControl
    {
        private class ComparisionWrapper<T> : Comparer<T>
        {
            private Comparison<T> m_comparison;

            public ComparisionWrapper(Comparison<T> comparison)
		{
			m_comparison = comparison;
		}

            public override int Compare(T x, T y)
		{
			return m_comparison(x, y);
		}
        }

        private BasicTextMappingInputValidatorDelegate m_sourceInputValidator;

        private BasicTextMappingInputValidatorDelegate m_targetInputValidator;

        private Comparison<string> m_sourceUniquenessComparison;

        private SortedList<string, string> m_currentMappings;

        private bool m_bFlagCellInvalidAfterValidation;

        private IContainer components;

        private DataGridView w_dgvMappings;

        private DataGridViewTextBoxColumn w_dgvtbcSource;

        private DataGridViewTextBoxColumn w_dgvtbcTarget;

        private ContextMenuStrip _contextMenu;

        private ToolStripMenuItem _deleteSelectedButton;

        public bool ContainsInvalidMappings
        {
            get
		{
			foreach (DataGridViewRow current in (IEnumerable)w_dgvMappings.Rows)
			{
				if (!((current.Cells[0].Tag == null) ^ (current.Cells[1].Tag == null)) || (current.Cells[0].Tag == null && current.Cells[0].Style.ForeColor == Color.Gray && current.Cells[1].Tag != null && string.IsNullOrEmpty((string)current.Cells[1].Value)))
				{
					continue;
				}
				return true;
			}
			return false;
		}
        }

        public SerializableTable<string, string> Mappings
        {
            get
		{
			SerializableTable<string, string> commonSerializableTable = new CommonSerializableTable<string, string>();
			foreach (DataGridViewRow row in (IEnumerable)w_dgvMappings.Rows)
			{
				if (row.Cells[0].Tag != null && row.Cells[1].Tag != null)
				{
					commonSerializableTable.Add((string)row.Cells[0].Value, (string)row.Cells[1].Value);
				}
			}
			return commonSerializableTable;
		}
            set
		{
			ImportMappings(value);
		}
        }

        public string SourceColumnName
        {
            get
		{
			return w_dgvtbcSource.HeaderText;
		}
            set
		{
			w_dgvtbcSource.HeaderText = value;
		}
        }

        public BasicTextMappingInputValidatorDelegate SourceInputValidator
        {
            get
		{
			return m_sourceInputValidator;
		}
            set
		{
			m_sourceInputValidator = value;
			UpdateColumnValidState(0);
		}
        }

        public Comparison<string> SourceUniquenessComparison
        {
            get
		{
			return m_sourceUniquenessComparison;
		}
            set
		{
			m_sourceUniquenessComparison = value;
			RebuildSortedDictionary();
		}
        }

        public string TargetColumnName
        {
            get
		{
			return w_dgvtbcTarget.HeaderText;
		}
            set
		{
			w_dgvtbcTarget.HeaderText = value;
		}
        }

        public BasicTextMappingInputValidatorDelegate TargetInputValidator
        {
            get
		{
			return m_targetInputValidator;
		}
            set
		{
			m_targetInputValidator = value;
			UpdateColumnValidState(1);
		}
        }

        public BasicTextMappingControl()
	{
		InitializeComponent();
		m_sourceInputValidator = null;
		m_targetInputValidator = null;
		m_sourceUniquenessComparison = null;
		m_bFlagCellInvalidAfterValidation = false;
		_deleteSelectedButton.Image = Metalogix.UI.WinForms.Properties.Resources.Delete.ToBitmap();
		RebuildSortedDictionary();
		if (!base.DesignMode)
		{
			UpdateColumnValidState(0);
			UpdateColumnValidState(1);
		}
	}

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
	{
		bool flag = w_dgvMappings.EditingControl != null || (w_dgvMappings.SelectedCells.Count == 0 && w_dgvMappings.SelectedRows.Count == 0);
		e.Cancel = flag;
	}

        private DataGridViewRow AddMapping(string sSource, string sTarget)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
		dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
		dataGridViewRow.Cells[0].Value = sSource;
		dataGridViewRow.Cells[1].Value = sTarget;
		return dataGridViewRow;
	}

        public void AppendMappings(IEnumerable<KeyValuePair<string, string>> mappingCollection)
	{
		if (mappingCollection == null)
		{
			return;
		}
		List<DataGridViewRow> dataGridViewRows = new List<DataGridViewRow>();
		SortedList<string, string> strs = ((SourceUniquenessComparison == null) ? new SortedList<string, string>() : new SortedList<string, string>(new ComparisionWrapper<string>(SourceUniquenessComparison)));
		foreach (KeyValuePair<string, string> keyValuePair in mappingCollection)
		{
			if ((string.IsNullOrEmpty(keyValuePair.Key) && string.IsNullOrEmpty(keyValuePair.Value)) || (!string.IsNullOrEmpty(keyValuePair.Key) && m_currentMappings.ContainsKey(keyValuePair.Key)))
			{
				continue;
			}
			if (!string.IsNullOrEmpty(keyValuePair.Key))
			{
				if (strs.ContainsKey(keyValuePair.Key))
				{
					continue;
				}
				strs.Add(keyValuePair.Key, null);
			}
			dataGridViewRows.Add(AddMapping(keyValuePair.Key, keyValuePair.Value));
		}
		w_dgvMappings.Rows.AddRange(dataGridViewRows.ToArray());
		UpdateColumnValidState(0);
		UpdateColumnValidState(1);
	}

        private void ClearCurrentData()
	{
		w_dgvMappings.Rows.Clear();
		m_currentMappings.Clear();
	}

        private void DeleteSelectedMappings()
	{
		List<DataGridViewRow> dataGridViewRows = new List<DataGridViewRow>();
		foreach (DataGridViewCell selectedCell in w_dgvMappings.SelectedCells)
		{
			if (!dataGridViewRows.Contains(selectedCell.OwningRow))
			{
				dataGridViewRows.Add(selectedCell.OwningRow);
			}
		}
		foreach (DataGridViewRow selectedRow in w_dgvMappings.SelectedRows)
		{
			if (!dataGridViewRows.Contains(selectedRow))
			{
				dataGridViewRows.Add(selectedRow);
			}
		}
		if (dataGridViewRows.Count == 0 || (dataGridViewRows.Count == 1 && dataGridViewRows[0].IsNewRow) || FlatXtraMessageBox.Show(Metalogix.UI.WinForms.Properties.Resources.Delete_Mappings_Confirmation, Metalogix.Core.Properties.Resources.Warning, MessageBoxButtons.OKCancel) != DialogResult.OK)
		{
			return;
		}
		w_dgvMappings.SuspendLayout();
		try
		{
			foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
			{
				if (!dataGridViewRow.IsNewRow)
				{
					DataGridViewCell item = dataGridViewRow.Cells[0];
					if (item.Tag != null)
					{
						m_currentMappings.Remove((string)item.Tag);
					}
					w_dgvMappings.Rows.Remove(dataGridViewRow);
				}
			}
		}
		finally
		{
			w_dgvMappings.ResumeLayout();
		}
	}

        private void deleteSelectedMappingsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		DeleteSelectedMappings();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void FlagCellInvalid(DataGridViewCell cell)
	{
		if (cell.Style.ForeColor == Color.Gray || string.IsNullOrEmpty((string)cell.Value))
		{
			cell.Style.BackColor = Color.White;
			cell.Style.ForeColor = Color.Gray;
			cell.Style.Font = new Font(w_dgvMappings.Font, FontStyle.Italic);
			cell.Value = "<input required>";
			return;
		}
		if (cell.Style.ForeColor == Color.Gray)
		{
			cell.Value = null;
			cell.Style.Font = null;
		}
		cell.Style.BackColor = Color.White;
		cell.Style.ForeColor = Color.Red;
	}

        private void FlagCellValid(DataGridViewCell cell)
	{
		if (cell.Style.ForeColor == Color.Gray)
		{
			cell.Value = null;
			cell.Style.Font = null;
		}
		cell.Style.BackColor = Color.White;
		cell.Style.ForeColor = Color.Black;
	}

        public void ImportMappings(IEnumerable<KeyValuePair<string, string>> mappingCollection)
	{
		ClearCurrentData();
		if (mappingCollection == null)
		{
			return;
		}
		List<DataGridViewRow> dataGridViewRows = new List<DataGridViewRow>();
		foreach (KeyValuePair<string, string> keyValuePair in mappingCollection)
		{
			if (!string.IsNullOrEmpty(keyValuePair.Key) || !string.IsNullOrEmpty(keyValuePair.Value))
			{
				dataGridViewRows.Add(AddMapping(keyValuePair.Key, keyValuePair.Value));
			}
		}
		w_dgvMappings.Rows.AddRange(dataGridViewRows.ToArray());
		UpdateColumnValidState(0);
		UpdateColumnValidState(1);
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.w_dgvMappings = new System.Windows.Forms.DataGridView();
		this.w_dgvtbcSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.w_dgvtbcTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this._deleteSelectedButton = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.w_dgvMappings).BeginInit();
		this._contextMenu.SuspendLayout();
		base.SuspendLayout();
		this.w_dgvMappings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_dgvMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.w_dgvMappings.BackgroundColor = System.Drawing.Color.White;
		this.w_dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		System.Windows.Forms.DataGridViewColumnCollection columns = this.w_dgvMappings.Columns;
		System.Windows.Forms.DataGridViewColumn[] wDgvtbcSource = new System.Windows.Forms.DataGridViewColumn[2] { this.w_dgvtbcSource, this.w_dgvtbcTarget };
		columns.AddRange(wDgvtbcSource);
		this.w_dgvMappings.ContextMenuStrip = this._contextMenu;
		this.w_dgvMappings.Location = new System.Drawing.Point(0, 0);
		this.w_dgvMappings.Name = "w_dgvMappings";
		this.w_dgvMappings.Size = new System.Drawing.Size(389, 253);
		this.w_dgvMappings.TabIndex = 0;
		this.w_dgvMappings.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(w_dgvMappings_CellBeginEdit);
		this.w_dgvMappings.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(w_dgvMappings_CellMouseDown);
		this.w_dgvMappings.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(w_dgvMappings_CellValidated);
		this.w_dgvMappings.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(w_dgvMappings_CellValidating);
		this.w_dgvMappings.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(w_dgvMappings_UserAddedRow);
		this.w_dgvMappings.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(w_dgvMappings_UserDeletedRow);
		this.w_dgvMappings.KeyUp += new System.Windows.Forms.KeyEventHandler(w_dgvMappings_KeyUp);
		this.w_dgvtbcSource.HeaderText = "Source";
		this.w_dgvtbcSource.Name = "w_dgvtbcSource";
		this.w_dgvtbcTarget.HeaderText = "Target";
		this.w_dgvtbcTarget.Name = "w_dgvtbcTarget";
		this._contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this._deleteSelectedButton });
		this._contextMenu.Name = "_contextMenu";
		this._contextMenu.Size = new System.Drawing.Size(211, 26);
		this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(_contextMenu_Opening);
		this._deleteSelectedButton.Name = "_deleteSelectedButton";
		this._deleteSelectedButton.Size = new System.Drawing.Size(210, 22);
		this._deleteSelectedButton.Text = "Delete Selected Mappings";
		this._deleteSelectedButton.Click += new System.EventHandler(deleteSelectedMappingsToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_dgvMappings);
		base.Name = "BasicTextMappingControl";
		base.Size = new System.Drawing.Size(389, 253);
		((System.ComponentModel.ISupportInitialize)this.w_dgvMappings).EndInit();
		this._contextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        private void RebuildSortedDictionary()
	{
		SortedList<string, string> mCurrentMappings = m_currentMappings;
		if (SourceUniquenessComparison != null)
		{
			m_currentMappings = new SortedList<string, string>(new ComparisionWrapper<string>(SourceUniquenessComparison));
		}
		else
		{
			m_currentMappings = new SortedList<string, string>();
		}
		if (mCurrentMappings == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> mCurrentMapping in mCurrentMappings)
		{
			m_currentMappings.Add(mCurrentMapping.Key, mCurrentMapping.Value);
		}
	}

        private void UpdateColumnValidState(int iColumnIndex)
	{
		bool flag = iColumnIndex == 0;
		foreach (DataGridViewRow row in (IEnumerable)w_dgvMappings.Rows)
		{
			DataGridViewCell item = row.Cells[iColumnIndex];
			if (!ValidateCellInput(item, (string)item.Value, out var _))
			{
				FlagCellInvalid(item);
				if (!flag)
				{
					item.Tag = null;
				}
				else if (item.Tag != null)
				{
					m_currentMappings.Remove((string)item.Tag);
					item.Tag = null;
				}
			}
			else
			{
				FlagCellValid(item);
				if (!flag)
				{
					item.Tag = true;
				}
				else if (item.Tag == null)
				{
					m_currentMappings.Add((string)item.Value, null);
					item.Tag = (string)item.Value;
				}
			}
		}
	}

        private bool ValidateCellInput(DataGridViewCell cell, string sCellValue, out string sFailureMessage)
	{
		if (cell.Style.ForeColor == Color.Gray)
		{
			sCellValue = null;
		}
		bool columnIndex = cell.ColumnIndex == 0;
		BasicTextMappingInputValidatorDelegate basicTextMappingInputValidatorDelegate = (columnIndex ? SourceInputValidator : TargetInputValidator);
		sFailureMessage = null;
		bool flag = basicTextMappingInputValidatorDelegate?.Invoke(sCellValue, out sFailureMessage) ?? true;
		if ((columnIndex && string.IsNullOrEmpty(sCellValue)) || !flag)
		{
			return false;
		}
		return true;
	}

        private void w_dgvMappings_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
	{
		if (!base.DesignMode)
		{
			FlagCellValid(w_dgvMappings.Rows[e.RowIndex].Cells[e.ColumnIndex]);
		}
	}

        private void w_dgvMappings_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right || (e.ColumnIndex < 0 && e.RowIndex < 0) || e.ColumnIndex >= w_dgvMappings.Columns.Count || e.RowIndex >= w_dgvMappings.Rows.Count)
		{
			return;
		}
		if (e.ColumnIndex >= 0 && e.RowIndex < 0)
		{
			DataGridViewColumn item = w_dgvMappings.Columns[e.ColumnIndex];
			if (!w_dgvMappings.SelectedColumns.Contains(item))
			{
				w_dgvMappings.ClearSelection();
				item.Selected = true;
			}
		}
		else if (e.RowIndex >= 0 && e.ColumnIndex < 0)
		{
			DataGridViewRow dataGridViewRow = w_dgvMappings.Rows[e.RowIndex];
			if (!w_dgvMappings.SelectedRows.Contains(dataGridViewRow))
			{
				w_dgvMappings.ClearSelection();
				dataGridViewRow.Selected = true;
			}
		}
		else
		{
			DataGridViewCell dataGridViewCell = w_dgvMappings.Rows[e.RowIndex].Cells[e.ColumnIndex];
			if (!w_dgvMappings.SelectedCells.Contains(dataGridViewCell))
			{
				w_dgvMappings.ClearSelection();
				dataGridViewCell.Selected = true;
			}
		}
	}

        private void w_dgvMappings_CellValidated(object sender, DataGridViewCellEventArgs e)
	{
		if (!base.DesignMode && m_bFlagCellInvalidAfterValidation)
		{
			FlagCellInvalid(w_dgvMappings.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			m_bFlagCellInvalidAfterValidation = false;
		}
	}

        private void w_dgvMappings_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
	{
		if (base.DesignMode || e.RowIndex < 0 || e.ColumnIndex < 0)
		{
			return;
		}
		bool columnIndex = e.ColumnIndex == 0;
		DataGridViewCell item = w_dgvMappings.Rows[e.RowIndex].Cells[e.ColumnIndex];
		string formattedValue = ((!(item.Style.ForeColor == Color.Gray)) ? ((string)e.FormattedValue) : null);
		string str3 = formattedValue;
		string tag = (string)w_dgvMappings.Rows[e.RowIndex].Cells[0].Tag;
		if (!ValidateCellInput(item, str3, out var str))
		{
			if (columnIndex && item.Tag != null)
			{
				m_currentMappings.Remove((string)item.Tag);
				item.Tag = null;
			}
			if (string.IsNullOrEmpty(str3))
			{
				m_bFlagCellInvalidAfterValidation = true;
				if (!columnIndex)
				{
					item.Tag = null;
					return;
				}
				if (item.Tag != null)
				{
					m_currentMappings.Remove((string)item.Tag);
				}
				item.Tag = null;
			}
			else
			{
				if (!string.IsNullOrEmpty(str))
				{
					FlatXtraMessageBox.Show(str, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				if (w_dgvMappings.EditingControl is TextBox editingControl)
				{
					editingControl.SelectAll();
				}
				e.Cancel = true;
			}
			return;
		}
		if (!columnIndex)
		{
			item.Tag = true;
			DataGridViewCell dataGridViewCell = w_dgvMappings.Rows[e.RowIndex].Cells[0];
			if (dataGridViewCell.Tag == null)
			{
				string value = ((!(dataGridViewCell.Style.ForeColor == Color.Gray)) ? ((string)dataGridViewCell.Value) : null);
				if (ValidateCellInput(dataGridViewCell, value, out var _))
				{
					FlagCellValid(dataGridViewCell);
					dataGridViewCell.Tag = (string)dataGridViewCell.Value;
				}
				else
				{
					FlagCellInvalid(dataGridViewCell);
				}
			}
			return;
		}
		if (string.IsNullOrEmpty(tag))
		{
			if (m_currentMappings.ContainsKey(str3))
			{
				FlatXtraMessageBox.Show("A mapping already exists for the specified source value", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				if (w_dgvMappings.EditingControl is TextBox textBox)
				{
					textBox.SelectAll();
				}
				e.Cancel = true;
				return;
			}
			m_currentMappings.Add(str3, null);
			item.Tag = str3;
		}
		else if (tag != str3)
		{
			m_currentMappings.Remove(tag);
			if (m_currentMappings.ContainsKey(str3))
			{
				FlatXtraMessageBox.Show("A mapping already exists for the specified source value", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				if (w_dgvMappings.EditingControl is TextBox editingControl1)
				{
					editingControl1.SelectAll();
				}
				e.Cancel = true;
				return;
			}
			m_currentMappings.Add(str3, null);
			item.Tag = str3;
		}
		DataGridViewCell item1 = w_dgvMappings.Rows[e.RowIndex].Cells[1];
		if (item1.Tag == null)
		{
			if (!ValidateCellInput(item1, (string)item1.Value, out var _))
			{
				FlagCellInvalid(item1);
				return;
			}
			FlagCellValid(item1);
			item1.Tag = true;
		}
	}

        private void w_dgvMappings_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
		{
			DeleteSelectedMappings();
		}
	}

        private void w_dgvMappings_UserAddedRow(object sender, DataGridViewRowEventArgs e)
	{
		if (!base.DesignMode)
		{
			FlagCellInvalid(e.Row.Cells[0]);
			if (!ValidateCellInput(e.Row.Cells[1], (string)e.Row.Cells[1].Value, out var _))
			{
				FlagCellInvalid(e.Row.Cells[1]);
			}
		}
	}

        private void w_dgvMappings_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
	{
		if (!base.DesignMode)
		{
			DataGridViewCell item = e.Row.Cells[0];
			if (item.Tag != null)
			{
				m_currentMappings.Remove((string)item.Tag);
			}
		}
	}
    }
}
