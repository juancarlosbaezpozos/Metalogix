using System;

namespace Metalogix.Actions.Incremental
{
    public interface IIncrementalCheck
    {
        IIncrementableNode Source { get; set; }

        IIncrementableNode Target { get; set; }

        bool CanIncrement();
    }
}