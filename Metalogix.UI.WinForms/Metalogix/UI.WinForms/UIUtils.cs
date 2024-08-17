using System;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public static class UIUtils
	{
		public const string SharePointEditionConsole = "Content Matrix Console - SharePoint Edition";

		public const string SimplifiedModeSkin = "Metalogix 2017 Explicit";

		public const string Simplified = "Simplified Mode";

		public const string AdvancedMode = "_AdvancedMode";

		public readonly static string SimplifiedMode;

		private static int MAX_COLUMN_WIDTH;

		static UIUtils()
		{
			UIUtils.SimplifiedMode = string.Format("_{0}", "Simplified Mode".Replace(" ", string.Empty));
			UIUtils.MAX_COLUMN_WIDTH = 250;
		}

		public static void CenterParent(Control c)
		{
			UIUtils.CenterParent(c, UIUtils.CenterMode.Both, false);
		}

		public static void CenterParent(Control c, UIUtils.CenterMode centerMode, bool doDynamicResize)
		{
			if (c == null)
			{
				return;
			}
			if (c.Parent != null)
			{
				int num = (centerMode == UIUtils.CenterMode.Both || centerMode == UIUtils.CenterMode.Vertical ? c.Parent.Width / 2 - c.Width / 2 : c.Location.X);
				c.Location = new Point(num, (centerMode == UIUtils.CenterMode.Both || centerMode == UIUtils.CenterMode.Horizontal ? c.Parent.Height / 2 - c.Height / 2 : c.Location.Y));
			}
			if (doDynamicResize)
			{
				UIUtils.CenterObject centerObject = new UIUtils.CenterObject(c, centerMode);
			}
		}

		public static void FitColumnsToContent(ListView lv)
		{
			int[] width = new int[lv.Columns.Count];
			int num = (lv.SmallImageList != null ? lv.SmallImageList.ImageSize.Width : 0);
			for (int i = 0; i < lv.Columns.Count; i++)
			{
				if (!string.IsNullOrEmpty(lv.Columns[i].Text))
				{
					lv.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
					width[i] = lv.Columns[i].Width + num;
				}
				else
				{
					width[i] = num + 8;
				}
			}
			for (int j = 0; j < lv.Columns.Count; j++)
			{
				lv.Columns[j].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
				lv.Columns[j].Width = Math.Max(width[j], lv.Columns[j].Width);
			}
		}

		public static Size MeasureText(Graphics g, Font font, string text)
		{
			Size size = Size.Ceiling(g.MeasureString(text, font));
			Size size1 = TextRenderer.MeasureText(g, text, font);
			return new Size(Math.Max(size.Width, size1.Width), Math.Max(size.Height, size1.Height));
		}

		public static Size MeasureText(Graphics g, Font font, string text, int maxWidth)
		{
			Size size = UIUtils.MeasureText(g, font, text);
			if (size.Width > maxWidth)
			{
				size.Width = maxWidth;
			}
			return size;
		}

		public static int MeasureTextWidth(Graphics g, Font font, string text)
		{
			SizeF sizeF = g.MeasureString(text, font);
			int num = (int)Math.Ceiling((double)sizeF.Width);
			Size size = TextRenderer.MeasureText(g, text, font);
			return Math.Max(num, size.Width);
		}

		public static int MeasureTextWidth(Graphics g, Font font, string text, int maxWidth)
		{
			int num = UIUtils.MeasureTextWidth(g, font, text);
			if (num <= maxWidth)
			{
				return num;
			}
			return maxWidth;
		}

		public static void ReduceColumnsWidth(ListView lv, int maxWidth)
		{
			for (int i = 0; i < lv.Columns.Count; i++)
			{
				lv.Columns[i].Width = Math.Min(maxWidth, lv.Columns[i].Width);
			}
		}

		public static void ReduceColumnsWidth(ListView lv)
		{
			UIUtils.ReduceColumnsWidth(lv, UIUtils.MAX_COLUMN_WIDTH);
		}

		public enum CenterMode
		{
			Vertical,
			Horizontal,
			Both
		}

		private class CenterObject
		{
			private UIUtils.CenterMode _centerMode;

			private Control _c;

			public CenterObject(Control c, UIUtils.CenterMode mode)
			{
				this._centerMode = mode;
				this._c = c;
				c.Parent.Resize += new EventHandler(this.c_Resize);
			}

			private void c_Resize(object sender, EventArgs e)
			{
				UIUtils.CenterParent(this._c, this._centerMode, false);
			}
		}
	}
}