using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class SimpleIntervalClock : LabelControl
	{
		private System.Timers.Timer _timer;

		private TimeSpan _elapsedTime;

		private readonly object _clockLock = new object();

		public SimpleIntervalClock()
		{
			this.Text = "00:00:00";
			this._timer = new System.Timers.Timer(1000);
			this._timer.Elapsed += new ElapsedEventHandler(this.Tick);
			this._timer.AutoReset = true;
			this._elapsedTime = new TimeSpan();
		}

		protected override void Dispose(bool disposing)
		{
			this._timer.Dispose();
			base.Dispose(disposing);
		}

		public void ResetClock()
		{
			this._elapsedTime = new TimeSpan();
		}

		public void StartClock()
		{
			this._timer.Start();
		}

		public void StopClock()
		{
			this._timer.Stop();
		}

		private void Tick(object sender, ElapsedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				ElapsedEventHandler elapsedEventHandler = new ElapsedEventHandler(this.Tick);
				object[] objArray = new object[] { sender, e };
				base.Invoke(elapsedEventHandler, objArray);
				return;
			}
			this._elapsedTime = this._elapsedTime.Add(new TimeSpan(0, 0, 1));
			this.Text = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(this._elapsedTime.TotalHours), this._elapsedTime.Minutes, this._elapsedTime.Seconds);
		}
	}
}