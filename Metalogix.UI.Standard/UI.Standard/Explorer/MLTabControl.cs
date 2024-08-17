using System;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.Standard.Explorer
{
	public class MLTabControl : TabControl
	{
		public MLTabControl()
		{
			base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (base.TabCount <= 0)
			{
				return;
			}
			Pen pen = new Pen(Color.FromArgb(169, 169, 169));
			SolidBrush solidBrush = new SolidBrush(Color.White);
			SolidBrush solidBrush1 = new SolidBrush(Color.Black);
			StringFormat stringFormat = new StringFormat()
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			System.Drawing.Font font = new System.Drawing.Font(this.Font.FontFamily, 24f, FontStyle.Regular, GraphicsUnit.Pixel);
			TabPage item = base.TabPages[base.SelectedIndex];
			Rectangle tabRect = base.GetTabRect(base.SelectedIndex);
			Rectangle clientRectangle = base.ClientRectangle;
			e.Graphics.FillRectangle(solidBrush, clientRectangle);
			Rectangle rectangle = new Rectangle(clientRectangle.Left, tabRect.Bottom, clientRectangle.Width - 1, clientRectangle.Height);
			e.Graphics.DrawRectangle(pen, rectangle);
			for (int i = 0; i < base.TabCount; i++)
			{
				TabPage tabPage = base.TabPages[i];
				Rectangle tabRect1 = base.GetTabRect(i);
				if (i == base.SelectedIndex)
				{
					tabRect1.Inflate(0, 1);
					tabRect1.Offset(0, -1);
				}
				e.Graphics.DrawLine(pen, tabRect1.Left, tabRect1.Top, tabRect1.Right, tabRect1.Top);
				e.Graphics.DrawLine(pen, tabRect1.Left, tabRect1.Top, tabRect1.Left, tabRect1.Bottom);
				e.Graphics.DrawLine(pen, tabRect1.Right, tabRect1.Top, tabRect1.Right, tabRect1.Bottom);
				if (!tabPage.Enabled)
				{
					ControlPaint.DrawStringDisabled(e.Graphics, tabPage.Text, this.Font, Color.Gray, tabRect1, stringFormat);
				}
				else
				{
					e.Graphics.DrawString(tabPage.Text, this.Font, solidBrush1, tabRect1, stringFormat);
				}
			}
		}
	}
}