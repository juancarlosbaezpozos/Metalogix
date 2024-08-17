using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Metalogix.UI.CommandLine
{
	public class ConsoleHandler : IDisposable
	{
		private bool _consoleAttached;

		public ConsoleHandler()
		{
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AllocConsole();

		[DllImport("kernel32", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AttachConsole(int dwProcessId);

		public void AttachConsole()
		{
			int num;
			if (this._consoleAttached)
			{
				return;
			}
			ConsoleHandler.GetWindowThreadProcessId(ConsoleHandler.GetForegroundWindow(), out num);
			Process processById = Process.GetProcessById(num);
			if (processById.ProcessName != "cmd")
			{
				ConsoleHandler.AllocConsole();
				Console.WriteLine("Created a console ...");
			}
			else
			{
				ConsoleHandler.AttachConsole(processById.Id);
				Console.WriteLine("Run in console ...");
			}
			this._consoleAttached = true;
		}

		public void DetachConsole()
		{
			if (this._consoleAttached)
			{
				ConsoleHandler.FreeConsole();
				this._consoleAttached = false;
			}
		}

		public void Dispose()
		{
			this.DetachConsole();
		}

		~ConsoleHandler()
		{
			this.Dispose();
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool FreeConsole();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
	}
}