using Metalogix.Actions.Blocker;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public class HealthScoreBlockerWorker : IActionBlocker
	{
		private readonly HealthScoreBlockerSettings _healthScoreBlockerSettings;

		private readonly IHealthScoreProvider _healthProvider;

		private bool _shouldBlock;

		public string BlockedReason
		{
			get
			{
				return "server is under load";
			}
		}

		public HealthScoreBlockerWorker(IHealthScoreProvider healthProvider, HealthScoreBlockerSettings blockerSettings)
		{
			this._healthProvider = healthProvider;
			this._healthScoreBlockerSettings = blockerSettings;
		}

		public void Block()
		{
			this.BlockUntil(() => !this.ShouldBlock());
		}

		public void BlockUntil(Func<bool> condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			while (!condition())
			{
				Thread.Sleep(60000);
			}
		}

		private void CheckBlockOrResume(int? healthScore)
		{
			if (this._shouldBlock)
			{
				this.CheckForResume(healthScore);
				return;
			}
			this.CheckForBlock(healthScore);
		}

		private void CheckForBlock(int? healthScore)
		{
			if (healthScore.HasValue)
			{
				int? nullable = healthScore;
				int pauseValueUpToMaxValue = this._healthScoreBlockerSettings.GetPauseValueUpToMaxValue();
				if ((nullable.GetValueOrDefault() < pauseValueUpToMaxValue ? false : nullable.HasValue))
				{
					this._shouldBlock = true;
				}
			}
		}

		private void CheckForResume(int? healthScore)
		{
			if (healthScore.HasValue)
			{
				int? nullable = healthScore;
				int restartValueUpToMaxValue = this._healthScoreBlockerSettings.GetRestartValueUpToMaxValue();
				if ((nullable.GetValueOrDefault() > restartValueUpToMaxValue ? false : nullable.HasValue))
				{
					this._shouldBlock = false;
				}
			}
		}

		public bool ShouldBlock()
		{
			if (this._healthProvider != null)
			{
				this.CheckBlockOrResume(new int?(this._healthProvider.GetHealthScore()));
			}
			return this._shouldBlock;
		}
	}
}