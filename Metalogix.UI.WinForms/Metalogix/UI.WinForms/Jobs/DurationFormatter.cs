using System;

namespace Metalogix.UI.WinForms.Jobs
{
	public static class DurationFormatter
	{
		public static string FormatDuration(TimeSpan duration)
		{
			return string.Format("{0:00}:{1:00}:{2:00}", duration.Hours, duration.Minutes, duration.Seconds);
		}
	}
}