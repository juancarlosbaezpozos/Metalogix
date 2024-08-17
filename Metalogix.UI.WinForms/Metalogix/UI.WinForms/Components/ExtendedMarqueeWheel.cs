using DevExpress.XtraEditors;
using DevExpress.XtraWaitForm;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class ExtendedMarqueeWheel : ProgressPanel
	{
		private Image _overlayImage;

		public Image OverlayImage
		{
			get
			{
				return this._overlayImage;
			}
			set
			{
				this._overlayImage = value;
				base.Invalidate();
			}
		}

		public ExtendedMarqueeWheel()
		{
			base.Paint += new PaintEventHandler(this.On_Paint);
		}

		private Point CalculateImageCorner()
		{
			return new Point(0, base.Height / 2 - 16);
		}

		private void On_Paint(object sender, PaintEventArgs args)
		{
			Image overlayImage = this.OverlayImage;
			if (overlayImage == null)
			{
				return;
			}
			Control parent = this;
			Color backColor = this.BackColor;
			while (parent != null && backColor == Color.Transparent)
			{
				backColor = parent.BackColor;
				parent = parent.Parent;
			}
			Point point = this.CalculateImageCorner();
			args.Graphics.FillRectangle(new SolidBrush(backColor), point.X, point.Y, 32, 32);
			args.Graphics.DrawImage(overlayImage, point.X, point.Y, 32, 32);
		}
	}
}