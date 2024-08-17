using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Data
{
	public class FilterRow : IEditableObject
	{
		protected Hashtable _storedProperties;

		public string AndOr
		{
			get;
			set;
		}

		public string Field
		{
			get;
			set;
		}

		public string Operator
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public FilterRow()
		{
		}

		public void BeginEdit()
		{
			PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			this._storedProperties = new Hashtable((int)properties.Length);
			PropertyInfo[] propertyInfoArray = properties;
			for (int i = 0; i < (int)propertyInfoArray.Length; i++)
			{
				PropertyInfo propertyInfo = propertyInfoArray[i];
				if (propertyInfo.GetSetMethod() != null)
				{
					object value = propertyInfo.GetValue(this, null);
					this._storedProperties.Add(propertyInfo.Name, value);
				}
			}
		}

		public void CancelEdit()
		{
			if (this._storedProperties == null)
			{
				return;
			}
			PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.GetSetMethod() != null)
				{
					object item = this._storedProperties[propertyInfo.Name];
					propertyInfo.SetValue(this, item, null);
				}
			}
			this._storedProperties = null;
		}

		public void EndEdit()
		{
			this._storedProperties = null;
		}
	}
}