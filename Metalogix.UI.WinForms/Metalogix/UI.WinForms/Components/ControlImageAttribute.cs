using System;

namespace Metalogix.UI.WinForms.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public class ControlImageAttribute : Attribute
	{
		private string m_value;

		public string ImageName
		{
			get
			{
				return this.m_value;
			}
		}

		public ControlImageAttribute(string val)
		{
			this.m_value = val;
		}
	}
}