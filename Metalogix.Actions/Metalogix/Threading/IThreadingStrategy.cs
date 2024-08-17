using System;

namespace Metalogix.Threading
{
    public interface IThreadingStrategy
    {
        bool ThreadingEnabled { get; set; }

        bool AllowThreadAllocation(ThreadManager threadManager);

        event Metalogix.Threading.ThreadingStrategyStateChanged ThreadingStrategyStateChanged;
    }
}