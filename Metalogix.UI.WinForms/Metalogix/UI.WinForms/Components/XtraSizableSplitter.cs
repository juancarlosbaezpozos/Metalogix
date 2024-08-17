using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using System;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraSizableSplitter : SplitContainerControl
	{
		private int _splitterWidth = 2;

		public int SplitterWidth
		{
			get
			{
				return this._splitterWidth;
			}
			set
			{
				this._splitterWidth = value;
			}
		}

		public XtraSizableSplitter()
		{
		}

		protected override SplitContainerViewInfo CreateContainerInfo()
		{
			return new XtraSizableSplitterViewInfo(this);
		}
	}
}