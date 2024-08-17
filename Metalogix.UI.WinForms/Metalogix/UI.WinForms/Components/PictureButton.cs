using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class PictureButton : PictureEdit
	{
		private bool _mouseOver;

		private bool _mouseDown;

		private Color _mouseOverColor = Color.FromArgb(255, 227, 236, 255);

		private Color _clickColor = Color.FromArgb(255, 178, 194, 228);

		private Color _backgroundColor = Color.Transparent;

		public Color BackgroundColor
		{
			get
			{
				return this._backgroundColor;
			}
			set
			{
				this._backgroundColor = value;
				this.UpdateBackgroundColor();
			}
		}

		public Color ClickColor
		{
			get
			{
				return this._clickColor;
			}
			set
			{
				this._clickColor = value;
			}
		}

		public Color MouseOverColor
		{
			get
			{
				return this._mouseOverColor;
			}
			set
			{
				this._mouseOverColor = value;
			}
		}

		public PictureButton()
		{
			this.InitializeSettings();
		}

		private void InitializeSettings()
		{
			base.Properties.AllowFocused = false;
			base.Properties.Appearance.Options.UseBackColor = true;
			base.Properties.BorderStyle = BorderStyles.NoBorder;
			base.Properties.ReadOnly = true;
			base.Properties.ShowMenu = false;
			base.Properties.ShowZoomSubMenu = DefaultBoolean.False;
			this.BackColor = this.BackgroundColor;
		}

		protected override void OnClick(EventArgs e)
		{
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this._mouseDown = true;
				this.UpdateBackgroundColor();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			this._mouseOver = true;
			this.UpdateBackgroundColor();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this._mouseOver = false;
			this._mouseDown = false;
			this.UpdateBackgroundColor();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this._mouseDown && !base.ClientRectangle.Contains(base.PointToClient(Control.MousePosition)))
			{
				this._mouseOver = false;
				this._mouseDown = false;
				this.UpdateBackgroundColor();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			bool flag = false;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				flag = this._mouseDown;
				this._mouseDown = false;
				this.UpdateBackgroundColor();
			}
			base.OnMouseUp(e);
			if (flag)
			{
				base.OnClick(new EventArgs());
			}
		}

		private void UpdateBackgroundColor()
		{
			if (this._mouseDown)
			{
				this.BackColor = this.ClickColor;
				return;
			}
			if (this._mouseOver)
			{
				this.BackColor = this.MouseOverColor;
				return;
			}
			this.BackColor = this.BackgroundColor;
		}
	}
}