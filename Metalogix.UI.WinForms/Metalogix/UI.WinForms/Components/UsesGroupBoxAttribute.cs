using System;

namespace Metalogix.UI.WinForms.Components
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class UsesGroupBoxAttribute : Attribute
	{
		private readonly bool m_enabled;

		private readonly string m_groupName;

		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		public string GroupName
		{
			get
			{
				return this.m_groupName;
			}
		}

		public UsesGroupBoxAttribute(bool enabled) : this(enabled, null)
		{
		}

		public UsesGroupBoxAttribute(bool enabled, string groupName)
		{
			this.m_enabled = enabled;
			this.m_groupName = groupName;
		}
	}
}