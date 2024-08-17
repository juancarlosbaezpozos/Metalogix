using Metalogix.Jobs;
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class ListViewColumnComparer : IComparer
	{
		private int m_iColumnToSortOn;

		private bool m_bAscendingSort = true;

		private Type m_UnderlyingType = typeof(string);

		private ListViewColumnComparer.AscendingCompareMethod m_Comparer;

		private CultureInfo usDateTimeCultureInfo = CultureInfo.GetCultureInfo("en-US");

		public int IndexOfSortingColumn
		{
			get
			{
				return this.m_iColumnToSortOn;
			}
		}

		public bool IsAscendingSort
		{
			get
			{
				return this.m_bAscendingSort;
			}
			set
			{
				this.m_bAscendingSort = value;
			}
		}

		public ListViewColumnComparer(int indexOfSortColumn, bool sortAscending)
		{
			this.m_iColumnToSortOn = indexOfSortColumn;
			this.m_bAscendingSort = sortAscending;
			this.SetComparer();
		}

		public ListViewColumnComparer(int indexOfSortColumn, bool sortAscending, Type underlyingType)
		{
			this.m_iColumnToSortOn = indexOfSortColumn;
			this.m_bAscendingSort = sortAscending;
			this.m_UnderlyingType = (underlyingType != null ? underlyingType : typeof(string));
			this.SetComparer();
		}

		public int Compare(object a, object b)
		{
			string text = ((ListViewItem)a).SubItems[this.m_iColumnToSortOn].Text;
			string str = ((ListViewItem)b).SubItems[this.m_iColumnToSortOn].Text;
			int num = 0;
			num = (text.Length == 0 || str.Length == 0 ? this.StringComparer(text, str) : this.m_Comparer(text, str));
			if (this.m_bAscendingSort)
			{
				return num;
			}
			return -num;
		}

		private int DateTimeComparer(string a, string b)
		{
			DateTime now = DateTime.Now;
			DateTime dateTime = DateTime.Now;
			if (DateTime.TryParse(a, out now) && DateTime.TryParse(b, out dateTime))
			{
				return DateTime.Compare(now, dateTime);
			}
			return this.StringComparer(a, b);
		}

		private int FloatingComparer(string a, string b)
		{
			double num = double.Parse(a);
			return num.CompareTo(double.Parse(b));
		}

		private int IntComparer(string a, string b)
		{
			long num = long.Parse(a);
			return num.CompareTo(long.Parse(b));
		}

		private int LicenseStringComparer(string a, string b)
		{
			LicenseString licenseString = new LicenseString(a);
			LicenseString licenseString1 = new LicenseString(b);
			if (licenseString.DataUsedValue > licenseString1.DataUsedValue)
			{
				return 1;
			}
			if (licenseString.DataUsedValue < licenseString1.DataUsedValue)
			{
				return -1;
			}
			return 0;
		}

		private void SetComparer()
		{
			if (this.m_UnderlyingType == typeof(string) || this.m_UnderlyingType == null)
			{
				this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.StringComparer);
				return;
			}
			if (this.m_UnderlyingType == typeof(DateTime))
			{
				this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.DateTimeComparer);
				return;
			}
			if (this.m_UnderlyingType == typeof(double) || this.m_UnderlyingType == typeof(float))
			{
				this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.FloatingComparer);
				return;
			}
			if (this.m_UnderlyingType == typeof(int) || this.m_UnderlyingType == typeof(long) || this.m_UnderlyingType == typeof(uint) || this.m_UnderlyingType == typeof(byte) || this.m_UnderlyingType == typeof(sbyte) || this.m_UnderlyingType == typeof(short) || this.m_UnderlyingType == typeof(ushort) || this.m_UnderlyingType == typeof(ulong))
			{
				this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.IntComparer);
				return;
			}
			if (this.m_UnderlyingType == typeof(LicenseString))
			{
				this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.LicenseStringComparer);
				return;
			}
			this.m_Comparer = new ListViewColumnComparer.AscendingCompareMethod(this.StringComparer);
		}

		private int StringComparer(string a, string b)
		{
			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
		}

		private delegate int AscendingCompareMethod(string a, string b);
	}
}