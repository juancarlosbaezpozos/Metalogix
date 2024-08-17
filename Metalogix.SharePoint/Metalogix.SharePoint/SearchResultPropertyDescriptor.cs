using System;
using System.ComponentModel;

namespace Metalogix.SharePoint
{
	public class SearchResultPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor m_innerPropertyDescriptor = null;

		public override AttributeCollection Attributes
		{
			get
			{
				return this.m_innerPropertyDescriptor.Attributes;
			}
		}

		public override string Category
		{
			get
			{
				return this.m_innerPropertyDescriptor.Category;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(SPSearchResult);
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				return this.m_innerPropertyDescriptor.Converter;
			}
		}

		public override string Description
		{
			get
			{
				return this.m_innerPropertyDescriptor.Description;
			}
		}

		public override string DisplayName
		{
			get
			{
				return this.m_innerPropertyDescriptor.DisplayName;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.m_innerPropertyDescriptor.PropertyType;
			}
		}

		public SearchResultPropertyDescriptor(PropertyDescriptor nodePropertyDescriptor) : base(nodePropertyDescriptor.Name, null)
		{
			this.m_innerPropertyDescriptor = nodePropertyDescriptor;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			object obj;
			obj = (!(component is SPSearchResult) ? null : this.m_innerPropertyDescriptor.GetValue(((SPSearchResult)component).Node));
			return obj;
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}