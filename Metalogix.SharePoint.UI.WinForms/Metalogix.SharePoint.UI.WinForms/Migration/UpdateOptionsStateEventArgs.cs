using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class UpdateOptionsStateEventArgs : EventArgs
	{
		public bool? IsItemEnabled
		{
			get;
			private set;
		}

		public bool? IsListEnabled
		{
			get;
			private set;
		}

		public bool? IsSiteEnabled
		{
			get;
			private set;
		}

		public UpdateOptionsStateEventArgs(bool? isSiteEnabled, bool? isListEnabled = null, bool? isItemEnabled = null)
		{
			this.IsSiteEnabled = isSiteEnabled;
			this.IsListEnabled = isListEnabled;
			this.IsItemEnabled = isItemEnabled;
		}
	}
}