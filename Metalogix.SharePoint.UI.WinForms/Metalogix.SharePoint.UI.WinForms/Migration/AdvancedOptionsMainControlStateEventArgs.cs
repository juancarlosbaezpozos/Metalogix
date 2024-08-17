using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class AdvancedOptionsMainControlStateEventArgs : EventArgs
	{
		public bool IsDecreaseSize
		{
			get;
			private set;
		}

		public int Size
		{
			get;
			private set;
		}

		public AdvancedOptionsMainControlStateEventArgs(int size, bool isDecreaseSize)
		{
			this.Size = size;
			this.IsDecreaseSize = isDecreaseSize;
		}
	}
}