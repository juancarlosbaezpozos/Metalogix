using System;

namespace Metalogix.UI.WinForms
{
	public class CancelableEventArgs : EventArgs
	{
		private bool _isCanceled;

		public bool IsCanceled
		{
			get
			{
				return this._isCanceled;
			}
			set
			{
				this._isCanceled = value;
			}
		}

		public CancelableEventArgs()
		{
		}
	}
}