using System;

namespace Metalogix.UI.WinForms.Interfaces
{
	public interface IApplicationSetting
	{
		string DisplayText
		{
			get;
		}

		string ImageName
		{
			get;
		}

		string LargeImageName
		{
			get;
		}

		void OnClick(object sender, EventArgs e);
	}
}