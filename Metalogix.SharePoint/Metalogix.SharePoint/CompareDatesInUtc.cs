using System;
using System.Collections;

namespace Metalogix.SharePoint
{
	public class CompareDatesInUtc : IComparer
	{
		public CompareDatesInUtc()
		{
		}

		public int Compare(object oSource, object oTarget)
		{
			int num;
			Type type = typeof(DateTime);
			if ((!type.IsAssignableFrom(oSource.GetType()) ? false : type.IsAssignableFrom(oTarget.GetType())))
			{
				DateTime universalTime = (DateTime)oSource;
				DateTime dateTime = (DateTime)oTarget;
				if (universalTime.Kind != DateTimeKind.Utc)
				{
					universalTime = universalTime.ToUniversalTime();
				}
				if (dateTime.Kind != DateTimeKind.Utc)
				{
					dateTime = dateTime.ToUniversalTime();
				}
				num = universalTime.CompareTo(dateTime);
			}
			else
			{
				num = ((IComparable)oSource).CompareTo(oTarget);
			}
			return num;
		}
	}
}