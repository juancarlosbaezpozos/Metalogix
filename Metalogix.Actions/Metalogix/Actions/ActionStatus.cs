using System;
using System.ComponentModel;

namespace Metalogix.Actions
{
    public enum ActionStatus
    {
        [Description("Not Running")] NotRunning,
        Running,
        Paused,
        Aborted,
        Aborting,
        Done,
        Warning,
        Failed,
        Queued
    }
}