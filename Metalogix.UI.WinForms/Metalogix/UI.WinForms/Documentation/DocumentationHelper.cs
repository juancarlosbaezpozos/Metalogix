using Metalogix.UI.WinForms.Documentation.Native;
using Metalogix.WebHelp;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Documentation
{
	public static class DocumentationHelper
	{
		public static void ShowHelp(Control control, string topic)
		{
			IntPtr intPtr;
			bool flag;
			string empty = topic;
			if (string.IsNullOrEmpty(topic))
			{
				empty = string.Empty;
			}
			try
			{
				if (control != null)
				{
					control.Cursor = Cursors.WaitCursor;
				}
				flag = WebHelpLauncher.TryShowWebHelp(empty, out intPtr);
			}
			finally
			{
				if (control != null)
				{
					control.Cursor = Cursors.Default;
				}
			}
			if (flag)
			{
				Metalogix.UI.WinForms.Documentation.Native.NativeMethods.SetForegroundWindow(intPtr);
			}
		}
	}
}