using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using System;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraSizableSplitterViewInfo : SplitContainerViewInfo
	{
		private XtraSizableSplitter _container;

		public XtraSizableSplitterViewInfo(SplitContainerControl container) : base(container)
		{
			this._container = container as XtraSizableSplitter;
		}

		protected override int GetSplitterSize()
		{
			if (this._container == null)
			{
				return base.GetSplitterSize();
			}
			return this._container.SplitterWidth;
		}
	}
}