using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms
{
    public sealed class ErrorHandlerForm : XtraForm
    {
        private const int DETAILS_HEIGHT = 180;

        private MemoEdit _txtDetails;

        private Size _initialSize;

        private PictureEdit _icon;

        private MemoEdit _txtMessage;

        private SimpleButton _btnClose;

        private CheckEdit _cbDetails;

        private LabelControl _lblBackground;

        private Panel _panel;

        internal ErrorHandlerForm(Exception exc)
            : this("Error", (exc != null) ? exc.Message : string.Empty, exc, ErrorIcon.Error)
	{
	}

        internal ErrorHandlerForm(string message, Exception exc)
            : this("Error", message, exc, ErrorIcon.Error)
	{
	}

        internal ErrorHandlerForm(string caption, string message, ErrorIcon icon)
            : this(caption, message, null, icon)
	{
	}

        internal ErrorHandlerForm(string caption, string message, Exception exc, ErrorIcon icon)
	{
		InitializeComponent();
		_lblBackground.SendToBack();
		switch (icon)
		{
		case ErrorIcon.Warning:
			base.Icon = Icon.FromHandle(Resources.Item_Status_Warning.GetHicon());
			_icon.Image = Resources.JobStatus_Warning_32;
			break;
		case ErrorIcon.Information:
			base.Icon = Icon.FromHandle(Resources.About16.GetHicon());
			_icon.Image = Resources.About32;
			break;
		default:
			base.Icon = Icon.FromHandle(Resources.Item_Status_Failed.GetHicon());
			_icon.Image = Resources.JobStatus_Failed_32;
			break;
		}
		Text = caption ?? string.Empty;
		_txtMessage.Text = message ?? string.Empty;
		_txtDetails.Text = ((exc == null) ? string.Empty : GlobalServices.ErrorFormatter.FormatException(exc));
		MaximumSize = (MinimumSize = base.Size);
	}

        private void checkBoxDetails_CheckedChanged(object sender, EventArgs e)
	{
		if (!_cbDetails.Checked)
		{
			Size size = (MinimumSize = new Size(base.Width, base.Height - 180));
			MaximumSize = size;
			_txtDetails.Visible = false;
		}
		else
		{
			_txtDetails.Visible = true;
			Size size1 = (base.Size = (MaximumSize = new Size(base.Width, base.Height + 180)));
			MinimumSize = size1;
		}
	}

        private static int GetTextEditLineCount(TextEdit textBox)
	{
		return SendMessage(textBox.Handle, 186, 0, 0);
	}

        private void InitializeComponent()
	{
		this._txtDetails = new DevExpress.XtraEditors.MemoEdit();
		this._panel = new System.Windows.Forms.Panel();
		this._txtMessage = new DevExpress.XtraEditors.MemoEdit();
		this._icon = new DevExpress.XtraEditors.PictureEdit();
		this._btnClose = new DevExpress.XtraEditors.SimpleButton();
		this._cbDetails = new DevExpress.XtraEditors.CheckEdit();
		this._lblBackground = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this._txtDetails.Properties).BeginInit();
		this._panel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this._txtMessage.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._icon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._cbDetails.Properties).BeginInit();
		base.SuspendLayout();
		this._txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
		this._txtDetails.Location = new System.Drawing.Point(0, 123);
		this._txtDetails.Name = "_txtDetails";
		this._txtDetails.Properties.ReadOnly = true;
		this._txtDetails.Size = new System.Drawing.Size(457, 0);
		this._txtDetails.TabIndex = 2;
		this._txtDetails.Visible = false;
		this._txtDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(textBoxBody_KeyDown);
		this._panel.BackColor = System.Drawing.SystemColors.Window;
		this._panel.Controls.Add(this._txtMessage);
		this._panel.Controls.Add(this._icon);
		this._panel.Controls.Add(this._btnClose);
		this._panel.Controls.Add(this._cbDetails);
		this._panel.Controls.Add(this._lblBackground);
		this._panel.Dock = System.Windows.Forms.DockStyle.Top;
		this._panel.Location = new System.Drawing.Point(0, 0);
		this._panel.Name = "_panel";
		this._panel.Size = new System.Drawing.Size(457, 123);
		this._panel.TabIndex = 1;
		this._txtMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this._txtMessage.Location = new System.Drawing.Point(50, 12);
		this._txtMessage.Name = "_txtMessage";
		this._txtMessage.Properties.Appearance.BackColor = System.Drawing.SystemColors.Window;
		this._txtMessage.Properties.Appearance.Options.UseBackColor = true;
		this._txtMessage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._txtMessage.Properties.ReadOnly = true;
		this._txtMessage.Size = new System.Drawing.Size(400, 66);
		this._txtMessage.TabIndex = 9;
		this._txtMessage.TabStop = false;
		this._icon.EditValue = Metalogix.UI.WinForms.Properties.Resources.JobStatus_Failed_32;
		this._icon.Location = new System.Drawing.Point(12, 12);
		this._icon.Name = "_icon";
		this._icon.Properties.Appearance.BackColor = System.Drawing.SystemColors.Window;
		this._icon.Properties.Appearance.Options.UseBackColor = true;
		this._icon.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this._icon.Properties.ShowMenu = false;
		this._icon.Size = new System.Drawing.Size(36, 36);
		this._icon.TabIndex = 9;
		this._btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this._btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this._btnClose.Location = new System.Drawing.Point(379, 95);
		this._btnClose.Name = "_btnClose";
		this._btnClose.Size = new System.Drawing.Size(75, 23);
		this._btnClose.TabIndex = 1;
		this._btnClose.Text = "Close";
		this._cbDetails.Location = new System.Drawing.Point(10, 99);
		this._cbDetails.Name = "_cbDetails";
		this._cbDetails.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this._cbDetails.Properties.Appearance.Options.UseBackColor = true;
		this._cbDetails.Properties.Caption = "Show details";
		this._cbDetails.Size = new System.Drawing.Size(88, 19);
		this._cbDetails.TabIndex = 2;
		this._cbDetails.CheckedChanged += new System.EventHandler(checkBoxDetails_CheckedChanged);
		this._lblBackground.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this._lblBackground.Dock = System.Windows.Forms.DockStyle.Bottom;
		this._lblBackground.Location = new System.Drawing.Point(0, 110);
		this._lblBackground.Name = "_lblBackground";
		this._lblBackground.Size = new System.Drawing.Size(0, 13);
		this._lblBackground.TabIndex = 10;
		base.AcceptButton = this._btnClose;
		base.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Appearance.Options.UseFont = true;
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
		base.CancelButton = this._btnClose;
		base.ClientSize = new System.Drawing.Size(457, 123);
		base.Controls.Add(this._txtDetails);
		base.Controls.Add(this._panel);
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(465, 150);
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(465, 150);
		base.Name = "ErrorHandlerForm";
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		((System.ComponentModel.ISupportInitialize)this._txtDetails.Properties).EndInit();
		this._panel.ResumeLayout(false);
		this._panel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this._txtMessage.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this._icon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this._cbDetails.Properties).EndInit();
		base.ResumeLayout(false);
	}

        protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		ToggleVScroll();
		_cbDetails.Visible = _txtDetails.Text.Length > 0;
		_cbDetails.Enabled = _cbDetails.Visible;
	}

        protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (_txtDetails != null && _txtDetails.Visible)
		{
			MemoEdit height = _txtDetails;
			height.Height = base.ClientSize.Height - _txtDetails.Top;
		}
		if (_txtMessage != null)
		{
			ToggleVScroll();
		}
	}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((keyData & Keys.C) == Keys.C && (keyData & Keys.Control) == Keys.Control)
		{
			TextEdit textEdit = (_txtMessage.Focused ? _txtMessage : ((!_txtDetails.Focused) ? null : _txtDetails));
			TextEdit textEdit1 = textEdit;
			if (textEdit1 == null)
			{
				Clipboard.Clear();
				object[] text = new object[4]
				{
					_txtMessage.Text,
					Environment.NewLine,
					Environment.NewLine,
					_txtDetails.Text
				};
				Clipboard.SetText(string.Format("{0}{1}{2}{3}", text), TextDataFormat.Text);
			}
			else if (textEdit1.SelectionLength > 0)
			{
				textEdit1.SelectAll();
				textEdit1.Copy();
				textEdit1.Select(0, 0);
				return true;
			}
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void textBoxBody_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Control && e.KeyCode == Keys.A)
		{
			_txtDetails.SelectAll();
		}
	}

        private void ToggleVScroll()
	{
		int textEditLineCount = GetTextEditLineCount(_txtMessage);
		_txtMessage.Properties.ScrollBars = ((textEditLineCount * _txtMessage.Font.Height > _txtMessage.Height) ? ScrollBars.Vertical : ScrollBars.None);
	}
    }
}
