using Metalogix.DataStructures.Generic;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Taxonomy.Generic
{
	public abstract class SPTaxonomyIndexedCollection<T> : SerializableIndexedList<T>
	where T : class
	{
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public T this[Guid index]
		{
			get
			{
				return base[index];
			}
		}

		internal SPTaxonomyIndexedCollection(string sIndexName) : base(sIndexName)
		{
			Type type = typeof(T);
			PropertyInfo property = type.GetProperty(sIndexName);
			if (property == null)
			{
				throw new ArgumentException(string.Concat("The specified property does not exist for type ", type.FullName), "sIndexName");
			}
			if (property.PropertyType != typeof(Guid))
			{
				throw new ArgumentException(string.Concat("The specified property type is not System.Guid for type ", type.FullName), "sIndexName");
			}
		}
	}
}