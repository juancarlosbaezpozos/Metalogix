using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Jobs
{
    public class LogItemDetailCountLabel : XtraUserControl
    {
        private delegate void UpdateUIDelegate();

        private const int DEFAULT_NAME_VALUE_SEPARATION = 20;

        private int _nameValueSeparation = 20;

        private IContainer components;

        private LabelControl _nameLabel;

        private LabelControl _valueLabel;

        public new string Name
        {
            get
		{
			return _nameLabel.Text.Substring(0, _nameLabel.Text.Length - 1);
		}
            set
		{
			_nameLabel.Text = value + ":";
			UpdateUI();
		}
        }

        public int NameValueSeparation
        {
            get
		{
			return _nameValueSeparation;
		}
            set
		{
			_nameValueSeparation = value;
			UpdateUI();
		}
        }

        public long Value
        {
            get
		{
			return long.Parse(_valueLabel.Text);
		}
            set
		{
			_valueLabel.Text = value.ToString();
		}
        }

        public int ValueLabelWidth
        {
            get
		{
			return _valueLabel.Width;
		}
            set
		{
			_valueLabel.Width = value;
			UpdateUI();
		}
        }

        public LogItemDetailCountLabel()
	{
		InitializeComponent();
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

        private void InitializeComponent()
	{
		this._nameLabel = new DevExpress.XtraEditors.LabelControl();
		this._valueLabel = new DevExpress.XtraEditors.LabelControl();
		base.SuspendLayout();
		this._nameLabel.Location = new System.Drawing.Point(0, 5);
		this._nameLabel.Name = "_nameLabel";
		this._nameLabel.Size = new System.Drawing.Size(31, 13);
		this._nameLabel.TabIndex = 0;
		this._nameLabel.Text = "Name:";
		this._valueLabel.AutoEllipsis = true;
		this._valueLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this._valueLabel.Location = new System.Drawing.Point(51, 0);
		this._valueLabel.Name = "_valueLabel";
		this._valueLabel.Size = new System.Drawing.Size(42, 23);
		this._valueLabel.TabIndex = 1;
		this._valueLabel.Text = "0";
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this._valueLabel);
		base.Controls.Add(this._nameLabel);
		base.Margin = new System.Windows.Forms.Padding(0);
		this.Name = "ProgressDetailLabel";
		base.Size = new System.Drawing.Size(93, 23);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        public void SetSeparationForWidth(int width)
	{
		NameValueSeparation = width - (_nameLabel.Width + _valueLabel.Width);
	}

        private void UpdateUI()
	{
		if (base.InvokeRequired)
		{
			Invoke(new UpdateUIDelegate(UpdateUI));
			return;
		}
		int height = _valueLabel.Height;
		int width = _nameLabel.Width + NameValueSeparation;
		int num = _nameLabel.Width + NameValueSeparation + _valueLabel.Width;
		_valueLabel.Location = new Point(width, 0);
		base.Size = new Size(num, height);
	}
    }
}
