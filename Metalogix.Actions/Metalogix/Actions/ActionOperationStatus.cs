using System;

namespace Metalogix.Actions
{
    public enum ActionOperationStatus
    {
        Idle,
        Running,
        Completed,
        Warning,
        Failed,
        Same,
        Different,
        MissingOnSource,
        MissingOnTarget,
        Skipped,
        SkippedInEvaluationLicense
    }
}