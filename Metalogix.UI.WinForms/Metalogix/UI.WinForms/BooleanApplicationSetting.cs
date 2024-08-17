using DevExpress.XtraBars;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms.Interfaces;
using System;

namespace Metalogix.UI.WinForms
{
	public abstract class BooleanApplicationSetting : IBooleanApplicationSetting, IApplicationSetting
	{
		public abstract string DisplayText
		{
			get;
		}

		public abstract string ImageName
		{
			get;
		}

		public abstract string LargeImageName
		{
			get;
		}

		public abstract bool Value
		{
			get;
			set;
		}

		protected BooleanApplicationSetting()
		{
		}

		public void OnClick(object sender, EventArgs e)
		{
			this.Value = ((e as ItemClickEventArgs).Item as BarCheckItem).Checked;
			string displayText = this.DisplayText;
			bool value = this.Value;
			StringAccumulator.Message.Send("SettingRibbonClicked", displayText, value.ToString(), false, null);
		}
	}
}