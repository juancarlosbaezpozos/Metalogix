using System;

namespace Metalogix.Threading
{
    public delegate void TaskFailed(WorkerThread thread, string taskName, Exception exception_0);
}