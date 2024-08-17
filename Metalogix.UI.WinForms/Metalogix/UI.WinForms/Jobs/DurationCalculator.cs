using System;

namespace Metalogix.UI.WinForms.Jobs
{
	public static class DurationCalculator
	{
		public static TimeSpan CalculateDuration(DateTime startTime, DateTime endTime)
		{
			if (endTime < startTime)
			{
				throw new ArgumentException("Start time cannot be after end time");
			}
			return endTime.Subtract(startTime);
		}
	}
}