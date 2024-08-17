using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class MarqueeBar : Control
	{
		private const int MinInterval = 100;

		private const int SizeIncrement = 2;

		private Timer _cycle;

		private int _currentActiveItem;

		private System.Windows.Forms.BorderStyle _borderStyle;

		private int _cycleSpeed;

		private int _shapeCount;

		private int _shapeSize;

		private int _shapeSpacing;

		private ElementStyle _shapeToDraw;

		[Category("Appearance")]
		[DefaultValue(System.Windows.Forms.BorderStyle.None)]
		public System.Windows.Forms.BorderStyle BorderStyle
		{
			get
			{
				return this._borderStyle;
			}
			set
			{
				if (value != this._borderStyle)
				{
					this._borderStyle = value;
					base.Invalidate();
				}
			}
		}

		private Timer Cycle
		{
			get
			{
				return this._cycle;
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler eventHandler = new EventHandler(this.cycle_Tick);
				if (this._cycle != null)
				{
					this._cycle.Tick -= eventHandler;
				}
				this._cycle = value;
				if (this._cycle != null)
				{
					this._cycle.Tick += eventHandler;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(1000)]
		public int CycleSpeed
		{
			get
			{
				return this._cycleSpeed;
			}
			set
			{
				if (this._cycleSpeed != value)
				{
					this._cycleSpeed = value;
					this._shapeCount = 0;
					this.RecalcCountAndInterval();
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(5)]
		public int ShapeSize
		{
			get
			{
				return this._shapeSize;
			}
			set
			{
				if (value != this._shapeSize && value > 0)
				{
					this._shapeSize = value;
					this._currentActiveItem = 0;
					this.RecalcCountAndInterval();
					base.Invalidate();
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(5)]
		public int ShapeSpacing
		{
			get
			{
				return this._shapeSpacing;
			}
			set
			{
				if (value != this._shapeSpacing && value > 0)
				{
					this._shapeSpacing = value;
					this._currentActiveItem = 0;
					this.RecalcCountAndInterval();
					base.Invalidate();
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(ElementStyle.Circle)]
		public ElementStyle ShapeToDraw
		{
			get
			{
				return this._shapeToDraw;
			}
			set
			{
				if (value != this._shapeToDraw)
				{
					this._shapeToDraw = value;
					this._currentActiveItem = 0;
					base.Invalidate();
				}
			}
		}

		public MarqueeBar()
		{
			this._shapeToDraw = ElementStyle.Circle;
			this._shapeSize = 5;
			this._shapeSpacing = 5;
			this._cycleSpeed = 1000;
			this._borderStyle = System.Windows.Forms.BorderStyle.None;
			this._currentActiveItem = 0;
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			Timer timer = new Timer()
			{
				Interval = this._cycleSpeed,
				Enabled = true
			};
			this.Cycle = timer;
		}

		private Rectangle CalcItemRectangle(int i)
		{
			Point point = this.CalculateItemPosition(i);
			return new Rectangle(point.X - 2 - 2, point.Y - 2 - 2, this._shapeSize + 4 + 4, this._shapeSize + 4 + 4);
		}

		private Point CalculateItemPosition(int index)
		{
			return new Point(this._shapeSpacing * index + this._shapeSize * (index - 1), base.Height / 2 - this._shapeSize / 2);
		}

		private void cycle_Tick(object sender, EventArgs e)
		{
			int num = this._currentActiveItem;
			if (this._currentActiveItem < this._shapeCount)
			{
				this._currentActiveItem++;
			}
			else
			{
				this._currentActiveItem = 1;
			}
			base.Invalidate(this.CalcItemRectangle(num));
			base.Invalidate(this.CalcItemRectangle(this._currentActiveItem));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this._cycle != null)
			{
				this._cycle.Dispose();
				this._cycle = null;
			}
			base.Dispose(disposing);
		}

		private void DrawShape(Graphics g, ElementStyle shape, int x, int y, int size)
		{
			Rectangle rectangle = new Rectangle(x, y, size, size);
			using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle, this.ForeColor, ControlPaint.Dark(this.ForeColor), LinearGradientMode.Vertical))
			{
				using (Pen pen = new Pen(ControlPaint.Dark(this.ForeColor)))
				{
					switch (shape)
					{
						case ElementStyle.Circle:
						{
							g.FillEllipse(linearGradientBrush, rectangle);
							g.DrawEllipse(pen, rectangle);
							break;
						}
						case ElementStyle.Square:
						{
							g.FillRectangle(linearGradientBrush, rectangle);
							g.DrawRectangle(pen, rectangle);
							break;
						}
					}
				}
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			this.Cycle.Enabled = (!base.Enabled ? false : base.Visible);
			base.OnEnabledChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			switch (this._borderStyle)
			{
				case System.Windows.Forms.BorderStyle.FixedSingle:
				{
					ControlPaint.DrawBorder3D(graphics, base.ClientRectangle, Border3DStyle.Flat);
					break;
				}
				case System.Windows.Forms.BorderStyle.Fixed3D:
				{
					ControlPaint.DrawBorder3D(graphics, base.ClientRectangle, Border3DStyle.Sunken);
					break;
				}
			}
			for (int i = 1; i <= this._shapeCount; i++)
			{
				Point point = this.CalculateItemPosition(i);
				int x = point.X;
				int y = point.Y;
				if (i != this._currentActiveItem)
				{
					this.DrawShape(graphics, this._shapeToDraw, x, y, this._shapeSize);
				}
				else
				{
					this.DrawShape(graphics, this._shapeToDraw, x - 2, y - 2, this._shapeSize + 4);
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			this.RecalcCountAndInterval();
			base.OnResize(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			this.Cycle.Enabled = (!base.Enabled ? false : base.Visible);
			base.OnVisibleChanged(e);
		}

		private void RecalcCountAndInterval()
		{
			int num;
			int width = base.Width;
			num = (!(this._shapeSize > 0 & this._shapeSpacing > 0) ? 1 : (int)Math.Round(Math.Floor((double)(width - this._shapeSpacing) / (double)(this._shapeSize + this._shapeSpacing))));
			if (num != this._shapeCount && num > 0)
			{
				int num1 = this._cycleSpeed / num;
				this.Cycle.Interval = (num1 >= 100 ? num1 : 100);
				this._shapeCount = num;
			}
		}
	}
}