using DevExpress.XtraBars;
using DevExpress.XtraBars.Utils;
using System;
using System.ComponentModel;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraBarManagerWithArrows : BarManager
	{
		public XtraBarManagerWithArrows(IContainer container) : base(container)
		{
		}

		protected override BarManagerHelpers CreateHelpers()
		{
			return new XtraBarManagerArrowsHelper(this);
		}
	}
}