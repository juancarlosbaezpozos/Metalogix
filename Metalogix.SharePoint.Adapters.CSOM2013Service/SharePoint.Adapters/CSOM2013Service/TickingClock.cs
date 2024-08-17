using System;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	public class TickingClock
	{
		private const int DEFAULT_TTL = 3600000;

		private static System.Threading.Timer s_timer;

		private static DateTime s_dtLastTouched;

		static TickingClock()
		{
			TickingClock.s_timer = null;
			TickingClock.s_dtLastTouched = DateTime.Now;
		}

		public TickingClock()
		{
		}

		internal static void CheckTimer(object state)
		{
			int num = (int)state;
			if ((DateTime.Now - TickingClock.s_dtLastTouched).TotalMilliseconds > (double)num)
			{
				Application.Exit();
			}
		}

		internal static void ResetTimer()
		{
			TickingClock.s_dtLastTouched = DateTime.Now;
		}

		internal static void StartClock()
		{
			TickingClock.StartClock(3600000);
		}

		internal static void StartClock(int iTimeToLive)
		{
			if (TickingClock.s_timer != null)
			{
				TickingClock.s_timer.Dispose();
				TickingClock.s_timer = null;
			}
			TickingClock.s_dtLastTouched = DateTime.Now;
			TickingClock.s_timer = new System.Threading.Timer(new TimerCallback(TickingClock.CheckTimer), (object)iTimeToLive, iTimeToLive, iTimeToLive / 12);
		}
	}
}