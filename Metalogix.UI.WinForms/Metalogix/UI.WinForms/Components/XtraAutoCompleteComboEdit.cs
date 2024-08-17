using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraAutoCompleteComboEdit : ComboBoxEdit
	{
		public XtraAutoCompleteComboEdit()
		{
		}

		protected override void ProcessAutoSearchChar(KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b')
			{
				return;
			}
			base.ProcessAutoSearchChar(e);
		}
	}
}