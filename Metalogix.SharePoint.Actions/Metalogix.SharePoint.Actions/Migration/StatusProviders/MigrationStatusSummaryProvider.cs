using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.Jobs.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Migration.StatusProviders
{
	public class MigrationStatusSummaryProvider : ActionStatusSummaryProvider
	{
		private bool? _showSkippedItems = null;

		public MigrationStatusSummaryProvider()
		{
		}

		protected override bool ShouldShowSkippedItems(Job job)
		{
			if (!this._showSkippedItems.HasValue)
			{
				JobRunner action = job.Action as JobRunner;
				if (action == null)
				{
					SharePointActionOptions options = job.Action.Options as SharePointActionOptions;
					this._showSkippedItems = new bool?((options == null ? false : options.LogSkippedItems));
				}
				else if (action.Jobs == null || (int)action.Jobs.Length == 0)
				{
					this._showSkippedItems = new bool?(false);
				}
				else
				{
					Job[] jobs = action.Jobs;
					int num = 0;
					while (num < (int)jobs.Length)
					{
						SharePointActionOptions sharePointActionOption = jobs[num].Action.Options as SharePointActionOptions;
						if (sharePointActionOption == null || !sharePointActionOption.LogSkippedItems)
						{
							num++;
						}
						else
						{
							this._showSkippedItems = new bool?(true);
							break;
						}
					}
					if (!this._showSkippedItems.HasValue)
					{
						this._showSkippedItems = new bool?(false);
					}
				}
			}
			return this._showSkippedItems.Value;
		}
	}
}