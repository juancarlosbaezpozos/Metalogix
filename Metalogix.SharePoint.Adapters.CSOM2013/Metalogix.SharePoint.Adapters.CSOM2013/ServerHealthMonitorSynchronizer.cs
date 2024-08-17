using System;
using System.ComponentModel;
using System.Timers;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class ServerHealthMonitorSynchronizer : IDisposable
	{
		private const int RefreshInterval = 300000;

		private readonly Timer _timer;

		private bool _canRefresh;

		public ServerHealthMonitorSynchronizer()
		{
			this._timer = this.NewTimer();
			this._canRefresh = true;
		}

		public bool CanRefresh()
		{
			return this._canRefresh;
		}

		public void Dispose()
		{
			this._timer.Dispose();
		}

		private Timer NewTimer()
		{
			Timer timer = new Timer(300000)
			{
				AutoReset = true
			};
			timer.Elapsed += new ElapsedEventHandler(this.TimerElapsed);
			return timer;
		}

		public void Reset()
		{
			this._canRefresh = false;
		}

		public void Start()
		{
			this._timer.Start();
		}

		public void Stop()
		{
			this._timer.Stop();
		}

		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			this._canRefresh = true;
		}
	}
}