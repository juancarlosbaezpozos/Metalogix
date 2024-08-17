using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Metalogix.UI.WinForms.Documentation.Native
{
	public static class NativeMethods
	{
		internal const int SW_RESTORE = 9;

		internal const int SW_HIDE = 5;

		internal const int SW_SHOW = 5;

		internal const uint MF_ENABLED = 0;

		internal const uint MF_GRAYED = 1;

		internal const uint MF_DISABLED = 2;

		internal const uint MF_BYCOMMAND = 0;

		internal const uint SC_CLOSE = 61536;

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

		[DllImport("user32.Dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int EnumWindows(Metalogix.UI.WinForms.Documentation.Native.NativeMethods.EnumWinCallBack callBackFunc, int lParam);

		[DllImport("user32", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool FreeConsole();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("User32.Dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern void GetWindowText(int hWnd, StringBuilder str, int nMaxCount);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool MessageBeep(Metalogix.UI.WinForms.Documentation.Native.NativeMethods.BeepType beepType);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		internal enum BeepType
		{
			SimpleBeep = -1,
			Ok = 0,
			IconHand = 16,
			IconQuestion = 32,
			IconExclamation = 48,
			IconAsterisk = 64
		}

		internal delegate bool EnumWinCallBack(int hwnd, int lParam);
	}
}