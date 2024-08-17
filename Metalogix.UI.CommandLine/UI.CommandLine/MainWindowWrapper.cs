using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.CommandLine
{
	public class MainWindowWrapper : IDisposable
	{
		private Form _mainForm;

		private Metalogix.UI.CommandLine.ConsoleHandler _console;

		private bool _isConsole;

		public Metalogix.UI.CommandLine.ConsoleHandler ConsoleHandler
		{
			get
			{
				return this._console;
			}
		}

		public bool IsConsoleApplication
		{
			get
			{
				return this._isConsole;
			}
		}

		public bool IsDisposed
		{
			get
			{
				if (this._isConsole || this._mainForm == null)
				{
					return false;
				}
				return this._mainForm.IsDisposed;
			}
		}

		public Form MainForm
		{
			get
			{
				return this._mainForm;
			}
		}

		public MainWindowWrapper(Form frm)
		{
			if (frm == null)
			{
				throw new ArgumentNullException("frm");
			}
			this._mainForm = frm;
			this._console = null;
			this._isConsole = false;
		}

		public MainWindowWrapper()
		{
			this._mainForm = null;
			this._console = new Metalogix.UI.CommandLine.ConsoleHandler();
			this._console.AttachConsole();
			this._isConsole = true;
		}

		public void Close()
		{
			if (!this._isConsole && !this.IsDisposed)
			{
				this._mainForm.Close();
			}
		}

		public void Dispose()
		{
			if (this._isConsole)
			{
				this._console.Dispose();
			}
			else if (this._mainForm != null && !this._mainForm.IsDisposed)
			{
				this._mainForm.Dispose();
				return;
			}
		}

		public void ShowError(string msg)
		{
			if (this._isConsole)
			{
				Console.WriteLine("*** APPLICATION ERROR ***");
				Console.WriteLine(msg);
				return;
			}
			MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
}