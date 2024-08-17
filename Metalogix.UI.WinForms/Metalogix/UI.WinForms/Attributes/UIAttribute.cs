using System;

namespace Metalogix.UI.WinForms.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
	public abstract class UIAttribute : Attribute
	{
		protected UIAttribute()
		{
		}
	}
}