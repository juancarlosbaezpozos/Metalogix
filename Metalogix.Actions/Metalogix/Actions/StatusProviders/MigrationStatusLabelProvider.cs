using Metalogix.Actions;
using Metalogix.Actions.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions.StatusProviders
{
    public class MigrationStatusLabelProvider : ActionStatusLabelProvider
    {
        public MigrationStatusLabelProvider()
        {
            this.StatusMessageMap[ActionStatus.Aborted] = Resources.StatusLabel_MigrationAborted;
            this.StatusMessageMap[ActionStatus.Aborting] = Resources.StatusLabel_MigrationAborted;
            this.StatusMessageMap[ActionStatus.Done] = Resources.StatusLabel_MigrationComplete;
            this.StatusMessageMap[ActionStatus.Failed] = Resources.StatusLabel_MigrationFailed;
            this.StatusMessageMap[ActionStatus.NotRunning] = Resources.StatusLabel_MigrationNotRunning;
            this.StatusMessageMap[ActionStatus.Paused] = Resources.StatusLabel_MigrationPaused;
            this.StatusMessageMap[ActionStatus.Running] = Resources.StatusLabel_MigrationRunning;
            this.StatusMessageMap[ActionStatus.Warning] = Resources.StatusLabel_MigrationCompletedWithWarning;
        }
    }
}