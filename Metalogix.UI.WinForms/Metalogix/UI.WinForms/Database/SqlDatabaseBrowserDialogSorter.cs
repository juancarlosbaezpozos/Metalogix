using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Database
{
	public class SqlDatabaseBrowserDialogSorter : IComparer
	{
		public SqlDatabaseBrowserDialogSorter()
		{
		}

		public int Compare(object x, object y)
		{
			return string.Compare((x as TreeNode).Text, (y as TreeNode).Text);
		}
	}
}