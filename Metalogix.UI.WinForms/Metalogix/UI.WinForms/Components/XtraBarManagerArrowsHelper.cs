using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.ViewInfo;
using System;

namespace Metalogix.UI.WinForms.Components
{
	internal class XtraBarManagerArrowsHelper : BarManagerHelpers
	{
		public XtraBarManagerArrowsHelper(BarManager manager) : base(manager)
		{
		}

		public override BarLinkViewInfo CreateLinkViewInfo(BarItemLink link)
		{
			if (!(link.Item is BarButtonItem))
			{
				return base.CreateLinkViewInfo(link);
			}
			BarItemInfo item = this.PaintStyle.ItemInfoCollection[link.GetType()];
			if (item == null)
			{
				throw new Exception(string.Concat(link.GetType(), " viewInfo not found"));
			}
			return new XtraBarArrowsLinkViewInfo(item.Collection.PaintStyle.DrawParameters, link);
		}
	}
}