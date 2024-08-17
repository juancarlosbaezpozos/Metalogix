using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Database.StatusProviders
{
	public class RestorationStatusLabelProvider : ActionStatusLabelProvider
	{
		public RestorationStatusLabelProvider()
		{
			this.StatusMessageMap[ActionStatus.Aborted] = Resources.StatusLabel_RestorationAborted;
			this.StatusMessageMap[ActionStatus.Aborting] = Resources.StatusLabel_RestorationAborted;
			this.StatusMessageMap[ActionStatus.Done] = Resources.StatusLabel_RestorationComplete;
			this.StatusMessageMap[ActionStatus.Failed] = Resources.StatusLabel_RestorationFailed;
			this.StatusMessageMap[ActionStatus.NotRunning] = Resources.StatusLabel_RestorationNotRunning;
			this.StatusMessageMap[ActionStatus.Paused] = Resources.StatusLabel_RestorationPaused;
			this.StatusMessageMap[ActionStatus.Running] = Resources.StatusLabel_RestorationRunning;
			this.StatusMessageMap[ActionStatus.Warning] = Resources.StatusLabel_RestorationWarning;
		}
	}
}