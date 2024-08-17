using System;

namespace Metalogix.Office365
{
    public enum ReportingQueueMessageEvents
    {
        Unknown,
        JobQueued,
        JobLogFileCreate,
        JobStart,
        JobProgress,
        JobEnd,
        JobRestart,
        JobImportant,
        JobCancel,
        JobError,
        JobFatalError,
        JobWarning
    }
}