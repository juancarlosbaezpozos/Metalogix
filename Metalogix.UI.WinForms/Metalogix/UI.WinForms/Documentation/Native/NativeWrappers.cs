using System;

namespace Metalogix.UI.WinForms.Documentation.Native
{
	internal static class NativeWrappers
	{
		internal static void UpdateWindowCloseMenu(IntPtr hWnd, bool enabled)
		{
			IntPtr systemMenu = Metalogix.UI.WinForms.Documentation.Native.NativeMethods.GetSystemMenu(hWnd, false);
			if (!enabled)
			{
				Metalogix.UI.WinForms.Documentation.Native.NativeMethods.EnableMenuItem(systemMenu, 61536, 1);
				return;
			}
			Metalogix.UI.WinForms.Documentation.Native.NativeMethods.EnableMenuItem(systemMenu, 61536, 0);
		}
	}
}