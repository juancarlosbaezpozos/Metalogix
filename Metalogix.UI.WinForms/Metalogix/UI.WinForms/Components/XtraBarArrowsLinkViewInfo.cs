using DevExpress.XtraBars;
using DevExpress.XtraBars.ViewInfo;
using System;

namespace Metalogix.UI.WinForms.Components
{
	internal class XtraBarArrowsLinkViewInfo : BarButtonLinkViewInfo
	{
		protected override bool IsNeedOpenArrow
		{
			get
			{
				if (this.Link.Item.DropDownControl == null)
				{
					return false;
				}
				return this.Link.Item.ButtonStyle == BarButtonStyle.DropDown;
			}
		}

		public XtraBarArrowsLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link)
		{
		}
	}
}