using System;

namespace Metalogix.UI.WinForms.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public class ControlNameAttribute : Attribute
	{
		private string m_value;

		public string Name
		{
			get
			{
				return this.m_value;
			}
		}

		public ControlNameAttribute(string val)
		{
			this.m_value = val;
		}
	}
}