using System;

namespace Metalogix.UI.WinForms.Attributes
{
	public class FormTypeAttribute : UIAttribute
	{
		private readonly Type m_type;

		public Type FormType
		{
			get
			{
				return this.m_type;
			}
		}

		public FormTypeAttribute(Type formType)
		{
			this.m_type = formType;
		}
	}
}