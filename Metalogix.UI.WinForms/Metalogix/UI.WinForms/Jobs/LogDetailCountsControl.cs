using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Jobs
{
    public class LogDetailCountsControl : XtraUserControl
    {
        private delegate void UpdateProgressDetailsDelegate(IEnumerable<KeyValuePair<string, long>> details);

        private delegate void VoidDelegate();

        private const int DEFAULT_ROW_NUMBER = 3;

        private const int COLUMN_SEPARATION = 3;

        private static readonly Point FIRST_ITEM_POSITION;

        private Dictionary<string, LogItemDetailCountLabel> _labelMap = new Dictionary<string, LogItemDetailCountLabel>();

        private List<LogItemDetailCountLabel> _labels = new List<LogItemDetailCountLabel>();

        private int _rows = 3;

        private int _maximumColumns = -1;

        private IContainer components;

        private LabelControl _titleLabel;

        private int Columns => (_labels.Count - 1) / Rows + 1;

        public int MaximumColumns
        {
            get
		{
			return _maximumColumns;
		}
            set
		{
			if (_maximumColumns != value)
			{
				_maximumColumns = value;
				RefreshColumns(0);
				UpdateSize();
			}
		}
        }

        private int NextItemColumn => GetColumnByListIndex(_labels.Count);

        private int NextItemRow => GetRowByListIndex(_labels.Count);

        private int RowHeight
        {
            get
		{
			if (_labels.Count == 0)
			{
				return 23;
			}
			return _labels[0].Height;
		}
        }

        public int Rows
        {
            get
		{
			return _rows;
		}
            set
		{
			if (_rows != value)
			{
				_rows = value;
				RefreshColumns(0);
				UpdateSize();
			}
		}
        }

        public string Title
        {
            get
		{
			return _titleLabel.Text;
		}
            set
		{
			_titleLabel.Text = value;
		}
        }

        static LogDetailCountsControl()
	{
		FIRST_ITEM_POSITION = new Point(0, 19);
	}

        public LogDetailCountsControl()
	{
		InitializeComponent();
	}

        private void AddLabel(LogItemDetailCountLabel label)
	{
		int nextItemColumn = NextItemColumn;
		label.Location = GetPosition(nextItemColumn, NextItemRow);
		_labels.Add(label);
		_labelMap.Add(label.Name, label);
		UpdateColumnWidth(nextItemColumn);
		base.Controls.Add(label);
		UpdateSize();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private List<LogItemDetailCountLabel> GetColumn(int column)
	{
		int num = column * Rows;
		List<LogItemDetailCountLabel> logItemDetailCountLabels = new List<LogItemDetailCountLabel>(Rows);
		for (int i = 0; i < Rows && num + i < _labels.Count; i++)
		{
			logItemDetailCountLabels.Add(_labels[num + i]);
		}
		return logItemDetailCountLabels;
	}

        private int GetColumnByListIndex(int idx)
	{
		return idx / Rows;
	}

        private int GetColumnWidth(List<LogItemDetailCountLabel> column)
	{
		if (column.Count == 0)
		{
			return 0;
		}
		return column[0].Width;
	}

        private int GetColumnWidth(int column)
	{
		return GetColumnWidth(GetColumn(column));
	}

        private Point GetIntendedLabelPosition(LogItemDetailCountLabel label)
	{
		int num = _labels.IndexOf(label);
		int rowByListIndex = GetRowByListIndex(num);
		return GetPosition(GetColumnByListIndex(num), rowByListIndex);
	}

        private Point GetPosition(int column, int row)
	{
		int num = row * RowHeight + FIRST_ITEM_POSITION.Y;
		int x = FIRST_ITEM_POSITION.X;
		for (int i = 0; i < column; i++)
		{
			x = x + GetColumnWidth(i) + 3;
		}
		return new Point(x, num);
	}

        private int GetRowByListIndex(int idx)
	{
		return idx % Rows;
	}

        private void InitializeComponent()
	{
		this._titleLabel = new DevExpress.XtraEditors.LabelControl();
		base.SuspendLayout();
		this._titleLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this._titleLabel.Location = new System.Drawing.Point(0, 0);
		this._titleLabel.Name = "_titleLabel";
		this._titleLabel.Size = new System.Drawing.Size(59, 13);
		this._titleLabel.TabIndex = 0;
		this._titleLabel.Text = "PROGRESS";
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this._titleLabel);
		base.Name = "LogDetailCountsControl";
		base.Size = new System.Drawing.Size(129, 88);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void InsertLabel(LogItemDetailCountLabel label, int index)
	{
		int columnByListIndex = GetColumnByListIndex(index);
		if (MaximumColumns <= -1 || columnByListIndex < MaximumColumns)
		{
			if (index >= _labels.Count)
			{
				AddLabel(label);
				return;
			}
			_labels.Insert(index, label);
			_labelMap.Add(label.Name, label);
			RefreshColumns(columnByListIndex);
			base.Controls.Add(label);
			UpdateSize();
		}
	}

        private void RefreshColumns(int startingColumn)
	{
		for (int i = startingColumn; i < Columns; i++)
		{
			foreach (LogItemDetailCountLabel column in GetColumn(i))
			{
				if (MaximumColumns <= -1 || i < MaximumColumns)
				{
					column.Location = GetIntendedLabelPosition(column);
					column.Visible = true;
				}
				else
				{
					column.Visible = false;
				}
			}
			UpdateColumnWidth(i);
		}
	}

        private void UpdateColumnWidth(int column)
	{
		List<LogItemDetailCountLabel> logItemDetailCountLabels = GetColumn(column);
		int width = 0;
		foreach (LogItemDetailCountLabel logItemDetailCountLabel in logItemDetailCountLabels)
		{
			if (logItemDetailCountLabel.Width > width)
			{
				width = logItemDetailCountLabel.Width;
			}
		}
		foreach (LogItemDetailCountLabel logItemDetailCountLabel1 in logItemDetailCountLabels)
		{
			if (logItemDetailCountLabel1.Width < width)
			{
				logItemDetailCountLabel1.SetSeparationForWidth(width);
			}
		}
	}

        public void UpdateDetails(IEnumerable<KeyValuePair<string, long>> details)
	{
		if (base.InvokeRequired)
		{
			Invoke(new UpdateProgressDetailsDelegate(UpdateDetails));
			return;
		}
		SuspendLayout();
		try
		{
			int num = 0;
			foreach (KeyValuePair<string, long> detail in details)
			{
				if (!_labelMap.TryGetValue(detail.Key, out var logItemDetailCountLabel))
				{
					logItemDetailCountLabel = new LogItemDetailCountLabel
					{
						Name = detail.Key,
						Value = detail.Value
					};
					InsertLabel(logItemDetailCountLabel, num);
					num++;
				}
				else
				{
					logItemDetailCountLabel.Value = detail.Value;
					num++;
				}
			}
		}
		finally
		{
			ResumeLayout();
		}
	}

        private void UpdateSize()
	{
		if (base.InvokeRequired)
		{
			Invoke(new VoidDelegate(UpdateSize));
		}
		else if (!base.DesignMode)
		{
			int x = FIRST_ITEM_POSITION.X;
			int num = ((MaximumColumns > -1) ? Math.Min(MaximumColumns, Columns) : Columns);
			for (int i = 0; i < num; i++)
			{
				x += GetColumnWidth(i);
			}
			int y = FIRST_ITEM_POSITION.Y + RowHeight * Rows;
			if (base.Size.Width != x || base.Size.Height != y)
			{
				base.Size = new Size(x, y);
			}
		}
	}
    }
}
