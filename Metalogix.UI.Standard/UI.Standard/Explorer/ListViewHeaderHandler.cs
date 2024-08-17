using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Metalogix.UI.Standard.Explorer
{
	internal class ListViewHeaderHandler
	{
		private Rectangle _headerRect;

		public ListViewHeaderHandler()
		{
		}

		[DllImport("user32.Dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern int EnumChildWindows(IntPtr hWndParent, ListViewHeaderHandler.EnumWinCallBack callBackFunc, IntPtr lParam);

		private bool EnumWindowCallBack(IntPtr hwnd, IntPtr lParam)
		{
			ListViewHeaderHandler.RECT rECT;
			if (ListViewHeaderHandler.GetWindowRect(hwnd, out rECT))
			{
				this._headerRect = new Rectangle(rECT.Left, rECT.Top, rECT.Right - rECT.Left, rECT.Bottom - rECT.Top);
			}
			else
			{
				this._headerRect = Rectangle.Empty;
			}
			return false;
		}

		private static ColumnHeader[] GetOrderedHeaders(ListView lv)
		{
			ColumnHeader[] columnHeaderArray = new ColumnHeader[lv.Columns.Count];
			foreach (ColumnHeader column in lv.Columns)
			{
				columnHeaderArray[column.DisplayIndex] = column;
			}
			return columnHeaderArray;
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool GetWindowRect(IntPtr hWnd, out ListViewHeaderHandler.RECT lpRect);

		public bool IsHeaderClicked(ListView listView1, out ColumnHeader selectedHeader)
		{
			selectedHeader = null;
			ListViewHeaderHandler.EnumChildWindows(listView1.Handle, new ListViewHeaderHandler.EnumWinCallBack(this.EnumWindowCallBack), IntPtr.Zero);
			Point mousePosition = Control.MousePosition;
			if (!this._headerRect.Contains(mousePosition))
			{
				return false;
			}
			int x = mousePosition.X - this._headerRect.Left;
			int width = 0;
			ColumnHeader[] orderedHeaders = ListViewHeaderHandler.GetOrderedHeaders(listView1);
			int num = 0;
			while (num < (int)orderedHeaders.Length)
			{
				ColumnHeader columnHeader = orderedHeaders[num];
				width += columnHeader.Width;
				if (width <= x)
				{
					num++;
				}
				else
				{
					selectedHeader = columnHeader;
					break;
				}
			}
			return true;
		}

		private delegate bool EnumWinCallBack(IntPtr hwnd, IntPtr lParam);

		private struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}
	}
}