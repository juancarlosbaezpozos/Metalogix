using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public class HealthCheckTimer : IDisposable
	{
		private System.Timers.Timer _timer;

		private readonly ActionContext _context;

		public HealthCheckTimer(ActionContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this._context = context;
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.FireHealthChecked();
		}

		public void Dispose()
		{
			if (this._timer != null)
			{
				this._timer.Dispose();
				this._timer = null;
			}
		}

		private void FireHealthChecked()
		{
			if (this.OnHealthChecked == null)
			{
				return;
			}
			IDictionary<string, ServerHealthInformation> serverHealthScores = this.GetServerHealthScores();
			this.OnHealthChecked(serverHealthScores);
		}

		private IDictionary<string, ServerHealthInformation> GetServerHealthScores()
		{
			Dictionary<string, ServerHealthInformation> strs = new Dictionary<string, ServerHealthInformation>();
			this.GetServerHealthScores(this._context.Sources, strs);
			this.GetServerHealthScores(this._context.Targets, strs);
			return strs;
		}

		private void GetServerHealthScores(IXMLAbleList nodes, IDictionary<string, ServerHealthInformation> serverHealthScores)
		{
			foreach (Node node in nodes)
			{
				SPSite sharePointSiteFromNode = this.GetSharePointSiteFromNode(node as SPNode);
				if (sharePointSiteFromNode == null)
				{
					continue;
				}
				string lowerInvariant = sharePointSiteFromNode.Url.ToLowerInvariant();
				if (serverHealthScores.ContainsKey(lowerInvariant))
				{
					continue;
				}
				serverHealthScores.Add(lowerInvariant, sharePointSiteFromNode.GetServerHealth());
			}
		}

		private SPSite GetSharePointSiteFromNode(Node node)
		{
			for (Node i = node; i != null; i = i.Parent)
			{
				SPWeb sPWeb = i as SPWeb;
				if (sPWeb != null)
				{
					return sPWeb.RootSite;
				}
			}
			return null;
		}

		public void Start(IHealthCheckTimerSettings settings)
		{
			this._timer = new System.Timers.Timer()
			{
				Interval = (double)settings.ServerHealthCheckInterval,
				AutoReset = true
			};
			this._timer.Elapsed += new ElapsedEventHandler(this._timer_Elapsed);
			this._timer.Enabled = settings.ServerHealthCheckEnabled;
		}

		public void Stop()
		{
			if (this._timer == null)
			{
				throw new InvalidOperationException("Health check has not started yet.");
			}
			this._timer.Stop();
		}

		public event HealthCheckTimer.HealthCheckHandler OnHealthChecked;

		public delegate void HealthCheckHandler(IDictionary<string, ServerHealthInformation> healthInformations);
	}
}