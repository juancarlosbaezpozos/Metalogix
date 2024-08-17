using Metalogix.DataStructures;
using System;
using System.Globalization;

namespace Metalogix.SharePoint.Utilities
{
	public class ItemsViewLocalTimeDataConverter : IDataConverter<object, string>
	{
		private readonly static CultureInfo s_engUSCulture;

		static ItemsViewLocalTimeDataConverter()
		{
			ItemsViewLocalTimeDataConverter.s_engUSCulture = new CultureInfo("en-US", false);
		}

		public ItemsViewLocalTimeDataConverter()
		{
		}

		public string Convert(object oValue)
		{
			string str;
			if (oValue == null)
			{
				str = null;
			}
			else if (typeof(DateTime).IsAssignableFrom(oValue.GetType()))
			{
				DateTime localTime = (DateTime)oValue;
				if (localTime.Kind == DateTimeKind.Utc)
				{
					localTime = localTime.ToLocalTime();
				}
				str = localTime.ToString(ItemsViewLocalTimeDataConverter.s_engUSCulture);
			}
			else
			{
				str = oValue.ToString();
			}
			return str;
		}
	}
}