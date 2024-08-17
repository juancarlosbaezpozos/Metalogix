using Metalogix.UI.WinForms.Interfaces;
using Metalogix.Widgets;
using System;

namespace Metalogix.UI.WinForms.Attributes
{
	public class MultiSelectAttribute : UIAttribute
	{
		private readonly bool m_enabled;

		private readonly Type m_limiterType;

		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		public Type LimiterType
		{
			get
			{
				return this.m_limiterType;
			}
		}

		public MultiSelectAttribute(bool enabled) : this(enabled, null)
		{
		}

		public MultiSelectAttribute(bool enabled, Type limiterType)
		{
			this.m_enabled = enabled;
			this.m_limiterType = limiterType;
		}

		public IMultiSelectLimiter CreateLimiter()
		{
			if (this.LimiterType == null)
			{
				return null;
			}
			return (IMultiSelectLimiter)Activator.CreateInstance(this.LimiterType);
		}

		public EnhancedTreeView.AllowSelectionDelegate CreateLimiterDelegate()
		{
			IMultiSelectLimiter multiSelectLimiter = this.CreateLimiter();
			if (multiSelectLimiter == null)
			{
				return null;
			}
			IMultiSelectLimiter multiSelectLimiter1 = multiSelectLimiter;
			return new EnhancedTreeView.AllowSelectionDelegate(multiSelectLimiter1.Check);
		}
	}
}