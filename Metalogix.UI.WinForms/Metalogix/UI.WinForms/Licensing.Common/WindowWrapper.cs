using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	public class WindowWrapper : IWin32Window
	{
		private readonly IntPtr _handle;

		public IntPtr Handle
		{
			get
			{
				return this._handle;
			}
		}

		public WindowWrapper(int handle) : this(new IntPtr(handle))
		{
		}

		public WindowWrapper(IntPtr handle)
		{
			this._handle = handle;
		}
	}
}