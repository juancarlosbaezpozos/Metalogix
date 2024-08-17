using System;

namespace Metalogix.UI.WinForms.Interfaces
{
	public interface IBooleanApplicationSetting : IApplicationSetting
	{
		bool Value
		{
			get;
			set;
		}
	}
}