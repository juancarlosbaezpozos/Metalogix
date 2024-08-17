using Metalogix.Actions;
using Metalogix.Actions.Blocker;
using System;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public class HealthScoreBlocker : ILoggableActionBlocker, IActionBlocker
	{
		private readonly HealthScoreBlockerWorker _sourceBlocker;

		private readonly HealthScoreBlockerWorker _targetBlocker;

		public string BlockedReason
		{
			get
			{
				if (this._sourceBlocker.ShouldBlock())
				{
					return "source server is under load";
				}
				if (this._targetBlocker.ShouldBlock())
				{
					return "target server is under load";
				}
				return "server is under load";
			}
		}

		public HealthScoreBlocker(IHealthScoreProvider sourceHealth, IHealthScoreProvider targetHealth, HealthScoreBlockerSettings blockerSettings)
		{
			this._sourceBlocker = new HealthScoreBlockerWorker(sourceHealth, blockerSettings);
			this._targetBlocker = new HealthScoreBlockerWorker(targetHealth, blockerSettings);
		}

		public void Block()
		{
			this._sourceBlocker.Block();
			this._targetBlocker.Block();
		}

		public void BlockUntil(Func<bool> condition)
		{
			this._sourceBlocker.BlockUntil(condition);
			this._targetBlocker.BlockUntil(condition);
		}

		public LogItem CreateLogItem()
		{
			LogItem logItem = new LogItem("Pausing Job", "Server Health Issue", string.Empty, string.Empty, ActionOperationStatus.Warning)
			{
				Information = "Pausing job to avoid being throttled by any Client-Side Object Model (CSOM) connections such as SharePoint Online/Office 365.  This job will resume once the server health score is back to a healthy state.",
				Details = "To identify the problem server, enable server health score logging by changing 'ServerHealthLoggingEnabled' setting to 'True' under '<AppData>\\Roaming\\Metalogix\\UserSettings.xml or Configuration Database."
			};
			return logItem;
		}

		public bool ShouldBlock()
		{
			if (this._sourceBlocker.ShouldBlock())
			{
				return true;
			}
			return this._targetBlocker.ShouldBlock();
		}
	}
}