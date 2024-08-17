using System;

namespace Metalogix.Actions.Incremental
{
    public interface IIncrementableNode
    {
        string Id { get; }

        DateTime ModifiedDate { get; }
    }
}