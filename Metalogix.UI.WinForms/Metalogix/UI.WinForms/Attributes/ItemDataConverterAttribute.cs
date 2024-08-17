using Metalogix.DataStructures;
using System;

namespace Metalogix.UI.WinForms.Attributes
{
	public class ItemDataConverterAttribute : UIAttribute
	{
		private readonly bool m_enabled;

		private readonly Type m_converterType;

		public Type DataConverterType
		{
			get
			{
				return this.m_converterType;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		public ItemDataConverterAttribute(bool enabled, Type converterType)
		{
			this.m_enabled = enabled;
			this.m_converterType = converterType;
		}

		public IDataConverter<object, string> CreateDataConverter()
		{
			if (this.DataConverterType == null)
			{
				return null;
			}
			return (IDataConverter<object, string>)Activator.CreateInstance(this.DataConverterType);
		}
	}
}