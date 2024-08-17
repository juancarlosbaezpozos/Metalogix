using Metalogix;
using Metalogix.UI.WinForms;
using System;

namespace Metalogix.UI.WinForms.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public class IsAdvancedPreconditionAttribute : SystemPreconditionAttribute
	{
		private bool _isAdvanced = true;

		public IsAdvancedPreconditionAttribute(bool isAdvanced = true)
		{
			this._isAdvanced = true;
		}

		public override bool IsPreconditionTrue()
		{
			if (this._isAdvanced)
			{
				return UIConfigurationVariables.ShowAdvanced;
			}
			return true;
		}
	}
}