using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Utilities
{
	public static class DevExpressExtensions
	{
		public static int FindString(this ComboBoxEdit comboBoxEdit, string searchString)
		{
			int num = -1;
			int num1 = 0;
			foreach (object item in comboBoxEdit.Properties.Items)
			{
				if (!item.ToString().StartsWith(searchString))
				{
					num1++;
				}
				else
				{
					num = num1;
				}
			}
			return num;
		}

		public static int FindStringExact(this ComboBoxEdit comboBoxEdit, string searchString)
		{
			int num = -1;
			int num1 = 0;
			foreach (object item in comboBoxEdit.Properties.Items)
			{
				if (!item.ToString().Equals(searchString))
				{
					num1++;
				}
				else
				{
					num = num1;
				}
			}
			return num;
		}

		public static void SetIdealComboBoxEditSize(this ComboBoxEdit comboBoxEdit)
		{
			Graphics graphic = comboBoxEdit.CreateGraphics();
			int num = 0;
			foreach (object item in comboBoxEdit.Properties.Items)
			{
				Size stringSize = TextUtils.GetStringSize(graphic, item.ToString(), comboBoxEdit.Font);
				num = Math.Max(num, stringSize.Width);
			}
			comboBoxEdit.Width = num + 22;
		}
	}
}