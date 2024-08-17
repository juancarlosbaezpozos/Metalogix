using System;

namespace Metalogix.UI.WinForms.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class UsesTabControlAttribute : Attribute
	{
		private readonly bool m_enabled;

		private readonly string m_tabName;

		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		public string TabName
		{
			get
			{
				return this.m_tabName;
			}
		}

		public UsesTabControlAttribute(bool enabled) : this(enabled, null)
		{
		}

		public UsesTabControlAttribute(bool enabled, string tabName)
		{
			this.m_enabled = enabled;
			this.m_tabName = tabName;
		}
	}
}