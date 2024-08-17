using System;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	public class ActivationProgressHandler
	{
		private int _iHandle;

		private ActivationProgress _dlg;

		private object _disposeLock = new object();

		private string _currentMessage;

		private bool _dialogIsPopped;

		private Thread _uiThread;

		private bool _disposed;

		private bool _showDialog;

		public ActivationProgressHandler(int iHandle)
		{
			this._showDialog = true;
			this._iHandle = iHandle;
			this._uiThread = new Thread(new ThreadStart(this.ThreadProc));
			this._uiThread.Start();
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			lock (this._disposeLock)
			{
				this.ShutdownUI();
				this.ShutdownThread();
			}
			this._disposed = true;
		}

		private void ShutdownThread()
		{
			try
			{
				this._disposed = true;
				if (this._uiThread != null && !this._uiThread.Join(500))
				{
					this._uiThread.Abort();
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void ShutdownUI()
		{
			if (this._dlg.IsDisposed || this._dlg.Disposing)
			{
				return;
			}
			try
			{
				if (!this._dlg.InvokeRequired)
				{
					this._dlg.Close();
				}
				else
				{
					this._dlg.Invoke(new ActivationProgressHandler.ShutdownCallback(this.ShutdownUI));
				}
			}
			catch (Exception exception)
			{
				this._dialogIsPopped = false;
			}
		}

		private void ThreadProc()
		{
			this._dlg = new ActivationProgress()
			{
				TopMost = true
			};
			this._dialogIsPopped = true;
			if (this._iHandle < 1)
			{
				this._dlg.ShowDialog();
				return;
			}
			this._dlg.ShowDialog(new WindowWrapper(this._iHandle));
		}

		public void UpdateStatus(string message)
		{
			if (!this._dialogIsPopped)
			{
				this._currentMessage = message;
				return;
			}
			if (!this._dlg.InvokeRequired)
			{
				this._dlg.Status = message;
				return;
			}
			ActivationProgress activationProgress = this._dlg;
			ActivationProgressHandler.ActivationProgressCallback activationProgressCallback = new ActivationProgressHandler.ActivationProgressCallback(this.UpdateStatus);
			object[] objArray = new object[] { message };
			activationProgress.Invoke(activationProgressCallback, objArray);
		}

		public delegate void ActivationProgressCallback(string message);

		private delegate void ShutdownCallback();
	}
}