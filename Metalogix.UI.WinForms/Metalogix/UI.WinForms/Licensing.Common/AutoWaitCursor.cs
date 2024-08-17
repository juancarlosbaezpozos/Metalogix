using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	public class AutoWaitCursor : IDisposable
	{
		private Cursor _oldCursor;

		private Control _control;

		protected AutoWaitCursor() : this(null)
		{
		}

		protected AutoWaitCursor(Control control)
		{
			this._control = control;
			if (this._control == null)
			{
				this._oldCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
				return;
			}
			if (!this._control.InvokeRequired)
			{
				this._oldCursor = this._control.Cursor;
				this._control.Cursor = Cursors.WaitCursor;
				return;
			}
			this._control.Invoke(new MethodInvoker(() => {
				this._oldCursor = this._control.Cursor;
				this._control.Cursor = Cursors.WaitCursor;
			}));
		}

		public static AutoWaitCursor Create()
		{
			return new AutoWaitCursor();
		}

		public static AutoWaitCursor Create(Control control)
		{
			return new AutoWaitCursor(control);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (this._oldCursor != null)
			{
				if (this._control == null)
				{
					Cursor.Current = this._oldCursor;
				}
				else if (!this._control.IsDisposed)
				{
					if (!this._control.InvokeRequired)
					{
						this._control.Cursor = this._oldCursor;
					}
					else
					{
						this._control.Invoke(new MethodInvoker(() => this._control.Cursor = this._oldCursor));
					}
				}
				this._oldCursor = null;
				this._control = null;
			}
		}

		~AutoWaitCursor()
		{
			this.Dispose(false);
		}
	}
}