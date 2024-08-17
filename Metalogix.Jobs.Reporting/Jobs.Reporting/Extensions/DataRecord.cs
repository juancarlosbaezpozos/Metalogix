using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Reporting.Extensions
{
	internal static class DataRecord
	{
		internal static T SafeGetValue<T>(this IDataRecord reader, int ordinal)
		{
			if (reader.IsDBNull(ordinal))
			{
				return default(T);
			}
			return (T)reader.GetValue(ordinal);
		}
	}
}